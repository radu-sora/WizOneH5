using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace WizOne.Module
{
    public class Calcul
    {
        public static void CalculInOut(DataRow ent, bool salveaza = false, bool recalcul = false)
        {
            try
            {
                string valStrOri = General.Nz(ent["ValStr"], "").ToString();

                if (valStrOri != "" && Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""Ptj_tblAbsente"" WHERE ""DenumireScurta""='" + valStrOri + "'"), 0)) != 0)
                    return;

                GolimCampurile(ent);
                
                DamiFirstInLastOut(ent);

                //Florin 2019.10.03
                //int idProg = DamiProgramDeLucru(ent);
                if ((ent.Table.Columns.Contains("ModifProgram")) && Convert.ToInt32(General.Nz(ent["ModifProgram"], 0)) == 1)
                {
                    //NOP
                    //daca programul este modificat manual nu se mai calculeaza din nou
                }
                else
                    DamiProgramDeLucru(ent);

                CalculTimpLucrat(ent);

                //first in paid
                CalculIntrare(ent);

                //last out paid
                CalculIesire(ent);

                int timpPauzaDedusa = CalculPauzaBis(ent);

                //Florin 2020.09.09
                int oreLucrate = 0;
                int minuteIN = 0;
                int minuteOUT = 0;

                CalculOreLucrateBis(ent, timpPauzaDedusa, out oreLucrate, out minuteIN, out minuteOUT);

                int on = CalculOreNormale(ent, oreLucrate);
                int os = CalculOreSuplimentare(ent, oreLucrate);

                CalculAlteOre(ent, minuteIN, minuteOUT);

                CalculOreNoapte(ent, minuteIN, minuteOUT);

                ent["USER_NO"] = -93;
                ent["TIME"] = DateTime.Now;

                string sqlSal = CreeazaSintaxaUpdate(ent);
                General.ExecutaNonQuery(sqlSal);

                int f10003 = Convert.ToInt32(ent["F10003"]);
                DateTime ziua = Convert.ToDateTime(ent["Ziua"]);
                string filtru = $@"ent.F10003={f10003} AND ent.""Ziua""={General.ToDataUniv(ziua)}";

                General.CalculFormulePeZi(filtru);
                filtru = filtru.Replace("ent.", "");

                ent = General.IncarcaDR($@"SELECT ent.* FROM ""Ptj_Intrari"" ent WHERE {filtru}");

                string cmp = "";
                //if (General.Nz(ent["NormaProgram"], "").ToString() != "") cmp += ",\"NormaProgram\"=" + ent["NormaProgram"];
                cmp += @", ""NormaProgram""=(SELECT ""Norma"" FROM ""Ptj_Programe"" WHERE ""Id""=COALESCE(""IdProgram"",-99))";

                cmp += VerificareCereriH5(f10003, ziua, Convert.ToInt32(General.Nz(ent["ZiSapt"], 1)), Convert.ToInt32(General.Nz(ent["ziLibera"], 0)), Convert.ToInt32(General.Nz(ent["ziLiberaLegala"], 0)));

                if (!cmp.Contains("ValStr") && General.Nz(ent["ValStr"], "").ToString() != "") cmp += ",\"ValStr\"='" + ent["ValStr"] + "'";
                if (!cmp.Contains("Val0") && General.Nz(ent["Val0"], "").ToString() != "") cmp += ",\"Val0\"=" + ent["Val0"];

                string sqlUpd = "";
                if (cmp != "")
                    sqlUpd = $@"UPDATE ""Ptj_Intrari"" SET {cmp.Substring(1)} WHERE {filtru};";

                string sqlValStr = "";
                if (Dami.ValoareParam("SintaxaValStr","") == "")
                    General.MemoreazaEroarea("Lipseste parametrul 'SintaxaValStr'", "Calcul Ceasuri", "CalculInOut");
                else
                    sqlValStr = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr""={Dami.ValoareParam("SintaxaValStr", "")} WHERE {filtru};";

                General.ExecutaNonQuery(
                    "BEGIN " + Environment.NewLine +
                    sqlUpd + Environment.NewLine +
                    sqlValStr + Environment.NewLine +
                    " END;");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculInOut");
            }
        }

        public static bool AlocaContract(int f10003 = -99, DateTime? zi = null)
        {
            bool ras = false;

            try
            {
                if (f10003 == -99)
                {
                    string strSql = @"UPDATE A 
                                SET IdContract=(SELECT MAX(IdContract) AS IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CAST(B.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(B.DataSfarsit AS Date))
                                FROM Ptj_Intrari A
                                WHERE A.IdContract IS NULL";
                    if (Constante.tipBD == 2) strSql = @"UPDATE ""Ptj_Intrari"" A SET A.""IdContract""=(SELECT B.""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= TRUNC(A.""Ziua"") AND TRUNC(A.""Ziua"") <= TRUNC(B.""DataSfarsit"") AND ROWNUM<=1) WHERE A.""IdContract"" is null";

                    General.ExecutaNonQuery(strSql);
                }
                else
                {
                    string strSql = @"UPDATE A 
                                SET IdContract=(SELECT MAX(IdContract) AS IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CAST(B.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(B.DataSfarsit AS Date))
                                FROM Ptj_Intrari A
                                WHERE A.IdContract IS NULL AND A.F10003={0} AND YEAR(A.Ziua)=YEAR({1}) AND MONTH(A.Ziua)=MONTH({1})";
                    if (Constante.tipBD == 2) strSql = @"UPDATE ""Ptj_Intrari"" A SET A.""IdContract""=(SELECT B.""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= TRUNC(A.""Ziua"") AND TRUNC(A.""Ziua"") <= TRUNC(B.""DataSfarsit"") AND ROWNUM<=1) WHERE A.""IdContract"" is null AND A.F10003={0} AND to_char(A.""Ziua"",'mm/yyyy')=to_char({1},'mm/yyyy')";

                    strSql = string.Format(strSql, f10003, General.ToDataUniv(zi));
                    General.ExecutaNonQuery(strSql);
                }

                ras = true;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "AlocaContract");
            }

            return ras;
        }

        public static int TransformaInMinute(object dt)
        {
            int min = 0;
            try
            {
                if (dt != null && dt.ToString() != "") min = (Convert.ToDateTime(dt).Hour * 60) + Convert.ToDateTime(dt).Minute;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "General", "TransformaInMinute");
            }

            return min;
        }

        public static void AlocaZile(int f10003, DateTime ziua)
        {
            try
            {
                string filtru = "";
                if (f10003 != -99) filtru = "WHERE F10003=" + f10003 + " AND Ziua=" + General.ToDataUniv(ziua) + "";

                string strSql = @"UPDATE A SET a.ZiLiberaLegala = (CASE WHEN (SELECT COUNT(*) FROM HOLIDAYS WHERE CAST(DAY AS Date) = CAST(A.Ziua AS Date))>0 THEN 1 ELSE 0 END) FROM Ptj_Intrari A {0};
                                    UPDATE A SET a.ZiLibera = (case when (datepart(dw,Ziua)=1 OR datepart(dw,Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE CAST(DAY AS Date) = CAST(A.Ziua AS Date))>0) then 1 else 0 end) FROM Ptj_Intrari A {0};
                                    UPDATE A SET a.ZiSapt = (case when datepart(dw,Ziua) -1 = 0 then 7 else datepart(dw,Ziua) -1 end) FROM Ptj_Intrari A {0};";
                if (Constante.tipBD == 2)
                {
                    strSql = @"BEGIN
                                UPDATE ""Ptj_Intrari"" SET ""ZiLiberaLegala"" = (CASE WHEN (SELECT COUNT(*) FROM HOLIDAYS B WHERE TRUNC(B.DAY) = TRUNC(""Ziua""))>0 THEN 1 ELSE 0 END) {0};
                                UPDATE ""Ptj_Intrari"" SET ""ZiLibera"" = (case when ((1 + TRUNC(""Ziua"") - TRUNC(""Ziua"", 'IW'))=6 OR (1 + TRUNC(""Ziua"") - TRUNC(""Ziua"", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS B WHERE TRUNC(B.DAY) = TRUNC(""Ziua""))>0) then 1 else 0 end) {0};
                                UPDATE ""Ptj_Intrari"" SET ""ZiSapt"" = 1 + TRUNC(""Ziua"") - TRUNC(""Ziua"", 'IW') {0};
                                END;";
                }

                General.ExecutaNonQuery(string.Format(strSql, filtru));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "AlocaZile");
            }
        }

        private static void DamiFirstInLastOut(DataRow ent)
        {
            DateTime? firstIn = null;
            DateTime? lastOut = null;

            try
            {
                if (ent != null)
                {
                    //Prima intrare
                    DateTime?[] arrIn = { (DateTime?)General.Nz(ent["In1"], null), (DateTime?)General.Nz(ent["In2"], null), (DateTime?)General.Nz(ent["In3"], null), (DateTime?)General.Nz(ent["In4"], null), (DateTime?)General.Nz(ent["In5"], null), (DateTime?)General.Nz(ent["In6"], null), (DateTime?)General.Nz(ent["In7"], null), (DateTime?)General.Nz(ent["In8"], null), (DateTime?)General.Nz(ent["In9"], null), (DateTime?)General.Nz(ent["In10"], null), (DateTime?)General.Nz(ent["In11"], null), (DateTime?)General.Nz(ent["In12"], null), (DateTime?)General.Nz(ent["In13"], null), (DateTime?)General.Nz(ent["In14"], null), (DateTime?)General.Nz(ent["In15"], null), (DateTime?)General.Nz(ent["In16"], null), (DateTime?)General.Nz(ent["In17"], null), (DateTime?)General.Nz(ent["In18"], null), (DateTime?)General.Nz(ent["In19"], null), (DateTime?)General.Nz(ent["In20"], null) };
                    firstIn = arrIn.Min();

                    //Ultima iesire
                    DateTime?[] arrOut = { (DateTime?)General.Nz(ent["Out1"], null), (DateTime?)General.Nz(ent["Out2"], null), (DateTime?)General.Nz(ent["Out3"], null), (DateTime?)General.Nz(ent["Out4"], null), (DateTime?)General.Nz(ent["Out5"], null), (DateTime?)General.Nz(ent["Out6"], null), (DateTime?)General.Nz(ent["Out7"], null), (DateTime?)General.Nz(ent["Out8"], null), (DateTime?)General.Nz(ent["Out9"], null), (DateTime?)General.Nz(ent["Out10"], null), (DateTime?)General.Nz(ent["Out11"], null), (DateTime?)General.Nz(ent["Out12"], null), (DateTime?)General.Nz(ent["Out13"], null), (DateTime?)General.Nz(ent["Out14"], null), (DateTime?)General.Nz(ent["Out15"], null), (DateTime?)General.Nz(ent["Out16"], null), (DateTime?)General.Nz(ent["Out17"], null), (DateTime?)General.Nz(ent["Out18"], null), (DateTime?)General.Nz(ent["Out19"], null), (DateTime?)General.Nz(ent["Out20"], null) };
                    lastOut = arrOut.Max();

                    if (firstIn != null)
                        ent["FirstIn"] = firstIn;
                    else
                        ent["FirstIn"] = DBNull.Value;

                    if (lastOut != null)
                        ent["LastOut"] = lastOut;
                    else
                        ent["LastOut"] = DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "DamiFirstInLastOut");
            }
        }

        private static int DamiProgramDeLucru(DataRow ent)
        {
            int idProg = -99;
            int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));
            DateTime? dt = null;
            if (ent["Ziua"] != DBNull.Value) dt = Convert.ToDateTime(ent["Ziua"]);
            DateTime? firstIn = null;
            if (ent["FirstIn"] != DBNull.Value) firstIn = Convert.ToDateTime(ent["FirstIn"]);
            DateTime? lastOut = null;
            if (ent["LastOut"] != DBNull.Value) lastOut = Convert.ToDateTime(ent["LastOut"]);

            try
            {
                DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
                
                //Florin 2020.04.16 - nu se mai foloseste odata cu optimizarea contracte si programe
                ////contract ciclic
                //if (dtCtr.Rows.Count > 0 && Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipContract"], -99)) == 2)
                //{
                //    if (dt != null && dtCtr.Rows[0]["CicluDataInceput"].ToString() != "" && dt >= Convert.ToDateTime(dtCtr.Rows[0]["CicluDataInceput"]))
                //    {
                //        TimeSpan ts = Convert.ToDateTime(dt) - Convert.ToDateTime(dtCtr.Rows[0]["CicluDataInceput"]);
                //        int nrZile = Convert.ToInt32(ts.TotalDays);
                //        int lung = 1;
                //        if (dtCtr.Rows[0]["CicluLungime"].ToString() != "") lung = Convert.ToInt32(dtCtr.Rows[0]["CicluLungime"]);
                //        int zi = (nrZile % lung) + 1;
                //        DataTable dtCic = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ContracteCiclice"" WHERE ""IdContract""={0} AND ""ZiCiclu"" = {1}", idCtr, zi));
                //        if (dtCic.Rows.Count > 0 && dtCtr.Rows[0]["IdContractZilnic"].ToString() != "") idCtr = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["IdContractZilnic"], -99));
                //        dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
                //    }
                //}

                int ziLibera = Convert.ToInt32(General.ExecutaScalar(string.Format(@"SELECT COUNT(*) FROM HOLIDAYS WHERE DAY={0}", General.ToDataUniv(dt))));
                int sapt = Dami.ZiSapt(dt.Value.DayOfWeek.ToString());

                //aflam programul de lucru
                if (dtCtr.Rows.Count > 0)
                {
                    int tipSch0 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb0"], -99));
                    int tipSch1 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb1"], -99));
                    int tipSch2 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb2"], -99));
                    int tipSch3 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb3"], -99));
                    int tipSch4 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb4"], -99));
                    int tipSch5 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb5"], -99));
                    int tipSch6 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb6"], -99));
                    int tipSch7 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb7"], -99));
                    int tipSch8 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["TipSchimb8"], -99));
                    int prg0 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program0"], -99));
                    int prg1 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program1"], -99));
                    int prg2 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program2"], -99));
                    int prg3 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program3"], -99));
                    int prg4 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program4"], -99));
                    int prg5 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program5"], -99));
                    int prg6 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program6"], -99));
                    int prg7 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program7"], -99));
                    int prg8 = Convert.ToInt32(General.Nz(dtCtr.Rows[0]["Program8"], -99));


                    //sarbatoare legala
                    //if (ziLibera == 1 && sapt != 6 && sapt != 7)
                    //Radu 07.02.2017
                    if (ziLibera == 1)
                    {
                        if (tipSch8 == -99)
                        {
                            if (tipSch0 == 1) idProg = prg0;
                            if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                        }
                        else
                        {
                            if (tipSch8 == 1) idProg = prg8;
                            if (tipSch8 == 2) idProg = CalcProg(idCtr, 8, firstIn, lastOut);
                        }
                    }
                    else         //zi din sapt
                    {
                        switch (sapt)
                        {
                            case 1:
                                if (tipSch1 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch1 == 1) idProg = prg1;
                                    if (tipSch1 == 2) idProg = CalcProg(idCtr, 1, firstIn, lastOut);
                                }
                                break;
                            case 2:
                                if (tipSch2 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch2 == 1) idProg = prg2;
                                    if (tipSch2 == 2) idProg = CalcProg(idCtr, 2, firstIn, lastOut);
                                }
                                break;
                            case 3:
                                if (tipSch3 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch3 == 1) idProg = prg3;
                                    if (tipSch3 == 2) idProg = CalcProg(idCtr, 3, firstIn, lastOut);
                                }
                                break;
                            case 4:
                                if (tipSch4 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch4 == 1) idProg = prg4;
                                    if (tipSch4 == 2) idProg = CalcProg(idCtr, 4, firstIn, lastOut);
                                }
                                break;
                            case 5:
                                if (tipSch5 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch5 == 1) idProg = prg5;
                                    if (tipSch5 == 2) idProg = CalcProg(idCtr, 5, firstIn, lastOut);
                                }
                                break;
                            case 6:
                                if (tipSch6 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch6 == 1) idProg = prg6;
                                    if (tipSch6 == 2) idProg = CalcProg(idCtr, 6, firstIn, lastOut);
                                }
                                break;
                            case 7:
                                if (tipSch7 == -99)
                                {
                                    if (tipSch0 == 1) idProg = prg0;
                                    if (tipSch0 == 2) idProg = CalcProg(idCtr, 0, firstIn, lastOut);
                                }
                                else
                                {
                                    if (tipSch7 == 1) idProg = prg7;
                                    if (tipSch7 == 2) idProg = CalcProg(idCtr, 7, firstIn, lastOut);
                                }
                                break;
                        }
                    }

                }

                ent["IdProgram"] = idProg;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "DamiProgramDeLucru");
            }

            return idProg;
        }

        private static int CalcProg(int idCtr, int ziSapt, object objFirstIn, object objLastOut)
        {
            int idProg = -99;
            DateTime? firstIn = null;
            DateTime? lastOut = null;

            try
            {
                if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
                if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);
            }
            catch (Exception)
            {
            }


            try
            {
                DataTable dt = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""={0} AND ""TipSchimb""={1}", idCtr, ziSapt));

                if (dt.Rows.Count > 0 && firstIn != null && lastOut != null)
                {
                    foreach (DataRow rand in dt.Rows)
                    {
                        //Florin 2020.03.04
                        if (rand["OraInceputDeLa"] == DBNull.Value || rand["OraInceputLa"] == DBNull.Value || rand["OraSfarsitDeLa"] == DBNull.Value || rand["OraSfarsitLa"] == DBNull.Value)
                            continue;

                        //ora de inceput
                        DateTime oraIncDeLa = Convert.ToDateTime(rand["OraInceputDeLa"]);
                        DateTime oraIncLa = Convert.ToDateTime(rand["OraInceputLa"]);

                        oraIncDeLa = new DateTime(firstIn.Value.Year, firstIn.Value.Month, firstIn.Value.Day, oraIncDeLa.Hour, oraIncDeLa.Minute, oraIncDeLa.Second);
                        oraIncLa = new DateTime(firstIn.Value.Year, firstIn.Value.Month, firstIn.Value.Day, oraIncLa.Hour, oraIncLa.Minute, oraIncLa.Second);

                        if (oraIncDeLa > oraIncLa) oraIncLa = new DateTime(firstIn.Value.AddDays(1).Year, firstIn.Value.AddDays(1).Month, firstIn.Value.AddDays(1).Day, oraIncLa.Hour, oraIncLa.Minute, oraIncLa.Second);

                        //ora de sfarsit
                        DateTime oraSfDeLa = Convert.ToDateTime(rand["OraSfarsitDeLa"]);
                        DateTime oraSfLa = Convert.ToDateTime(rand["OraSfarsitLa"]);

                        oraSfDeLa = new DateTime(lastOut.Value.Year, lastOut.Value.Month, lastOut.Value.Day, oraSfDeLa.Hour, oraSfDeLa.Minute, oraSfDeLa.Second);
                        oraSfLa = new DateTime(lastOut.Value.Year, lastOut.Value.Month, lastOut.Value.Day, oraSfLa.Hour, oraSfLa.Minute, oraSfLa.Second);

                        if (oraSfDeLa > oraSfLa) oraSfLa = new DateTime(lastOut.Value.AddDays(1).Year, lastOut.Value.AddDays(1).Month, lastOut.Value.AddDays(1).Day, oraSfLa.Hour, oraSfLa.Minute, oraSfLa.Second);

                        switch (Convert.ToInt32(General.Nz(rand["ModVerificare"], -99)))
                        {
                            case 1:         //intrare
                                if ((Convert.ToDateTime(firstIn) - oraIncDeLa).TotalMinutes >= 0 && (oraIncLa - Convert.ToDateTime(firstIn)).TotalMinutes >= 0) idProg = Convert.ToInt32(General.Nz(rand["IdProgram"], -99));
                                break;
                            case 2:         //iesire
                                if ((Convert.ToDateTime(lastOut) - oraSfDeLa).TotalMinutes >= 0 && (oraSfLa - Convert.ToDateTime(lastOut)).TotalMinutes >= 0) idProg = Convert.ToInt32(General.Nz(rand["IdProgram"], -99));
                                break;
                            case 3:         //intrare si iesire
                                if (((Convert.ToDateTime(firstIn) - oraIncDeLa).TotalMinutes >= 0 && (oraIncLa - Convert.ToDateTime(firstIn)).TotalMinutes >= 0) && ((Convert.ToDateTime(lastOut) - oraSfDeLa).TotalMinutes >= 0 && (oraSfLa - Convert.ToDateTime(lastOut)).TotalMinutes >= 0)) idProg = Convert.ToInt32(General.Nz(rand["IdProgram"], -99));
                                break;
                            case 4:         //intrare sau iesire
                                if (((Convert.ToDateTime(firstIn) - oraIncDeLa).TotalMinutes >= 0 && (oraIncLa - Convert.ToDateTime(firstIn)).TotalMinutes >= 0) || ((Convert.ToDateTime(lastOut) - oraSfDeLa).TotalMinutes >= 0 && (oraSfLa - Convert.ToDateTime(lastOut)).TotalMinutes >= 0)) idProg = Convert.ToInt32(General.Nz(rand["IdProgram"], -99));
                                break;
                        }

                        if (idProg != -99) break;
                    }
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalcProg");
            }

            return idProg;
        }

        private static void CalculIntrare(DataRow ent)
        {
            int dif1 = 0;
            int dif2 = 0;
            bool varTmp = true;

            DateTime? firstInRap = null;
            DateTime? firstInPaid = null;

            DateTime? firstIn = null;

            DateTime? objFirstIn = null;
            if (ent["FirstIn"] != DBNull.Value) objFirstIn = Convert.ToDateTime(ent["FirstIn"]);
            DateTime? zi = null;
            if (ent["Ziua"] != DBNull.Value) zi = Convert.ToDateTime(ent["Ziua"]);
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));

            if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);

            try
            {
                DateTime? startTime = null;

                DataTable dt = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                DataRow entPrg = null;

                if (dt.Rows.Count > 0)
                {

                    entPrg = dt.Rows[0];
                    if (entPrg["OraIntrare"].ToString() != "" && firstIn != null) startTime = new DateTime(firstIn.Value.Year, firstIn.Value.Month, firstIn.Value.Day, Convert.ToDateTime(entPrg["OraIntrare"]).Hour, Convert.ToDateTime(entPrg["OraIntrare"]).Minute, Convert.ToDateTime(entPrg["OraIntrare"]).Second);

                    if (Convert.ToInt32(General.Nz(entPrg["Flexibil"], -99)) == 1)
                    {
                        if (entPrg["OraIntrare"].ToString() != "" && entPrg["OraIesire"].ToString() != "" && firstIn != null)
                        {
                            DateTime oraInc = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, Convert.ToDateTime(entPrg["OraIntrare"]).Hour, Convert.ToDateTime(entPrg["OraIntrare"]).Minute, Convert.ToDateTime(entPrg["OraIntrare"]).Second);
                            DateTime oraSf = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);

                            if (Convert.ToDateTime(entPrg["OraIntrare"]) > Convert.ToDateTime(entPrg["OraIesire"])) oraSf = new DateTime(zi.Value.AddDays(1).Year, zi.Value.AddDays(1).Month, zi.Value.AddDays(1).Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);

                            if (Convert.ToDateTime(firstIn) < oraInc) startTime = oraInc;
                            if (oraInc < Convert.ToDateTime(firstIn) && Convert.ToDateTime(firstIn) < oraSf) startTime = firstIn;
                            if (oraSf < Convert.ToDateTime(firstIn)) startTime = oraSf;
                        }
                        else
                        {
                            startTime = firstIn;
                        }
                    }
                }


                if (firstIn != null && startTime != null)
                {
                    if (firstIn.Value.Hour == startTime.Value.Hour && firstIn.Value.Minute == startTime.Value.Minute)
                    {
                        firstInPaid = firstIn;
                        firstInRap = firstIn;
                    }
                    else
                    {
                        if (firstIn < startTime)                        //intrare anticipata
                        {
                            varTmp = false;

                            //int dif = Convert.ToInt32((Convert.ToDateTime(startTime) - Convert.ToDateTime(firstIn)).TotalMinutes);
                            int dif = CalculIntervale(ent, Convert.ToDateTime(firstIn), Convert.ToDateTime(startTime));

                            int difRap = 0;
                            if (entPrg["INSubDiferentaRaportare"].ToString() != "") difRap = TransformaInMinute(Convert.ToDateTime(entPrg["INSubDiferentaRaportare"]));
                            if (dif <= difRap)
                                firstInRap = TransformaInData(firstIn, startTime.Value.Hour, startTime.Value.Minute);
                            else
                                firstInRap = firstIn;

                            int valMin = 0;
                            int valMax = 999999999;
                            if (entPrg["INSubMinPlata"].ToString() != "") valMin = TransformaInMinute(Convert.ToDateTime(entPrg["INSubMinPlata"]));
                            if (entPrg["INSubMaxPlata"].ToString() != "") valMax = TransformaInMinute(Convert.ToDateTime(entPrg["INSubMaxPlata"]));

                            if (dif < valMin) firstInPaid = TransformaInData(firstIn, startTime.Value.Hour, startTime.Value.Minute);
                            if (valMin <= dif && dif <= valMax) firstInPaid = TransformaInData(firstIn, firstIn.Value.Hour, firstIn.Value.Minute);
                            DateTime? dtPen = startTime.Value.AddMinutes(-1 * valMax);
                            if (dif > valMax) firstInPaid = TransformaInData(firstIn, dtPen.Value.Hour, dtPen.Value.Minute);

                            //dif1 = Convert.ToInt32((Convert.ToDateTime(startTime) - Convert.ToDateTime(firstInPaid)).TotalMinutes);
                            //dif2 = Convert.ToInt32((Convert.ToDateTime(firstInPaid) - Convert.ToDateTime(firstIn)).TotalMinutes);
                            dif1 = CalculIntervale(ent, Convert.ToDateTime(firstInPaid), Convert.ToDateTime(startTime));
                            dif2 = CalculIntervale(ent, Convert.ToDateTime(firstIn), Convert.ToDateTime(firstInPaid));
                        }
                        else                                          //intrare tarzie
                        {
                            int dif = Convert.ToInt32((Convert.ToDateTime(firstIn) - Convert.ToDateTime(startTime)).TotalMinutes);

                            int difRap = 0;
                            if (entPrg["INPesteDiferentaRaportare"].ToString() != "") difRap = TransformaInMinute(Convert.ToDateTime(entPrg["INPesteDiferentaRaportare"]));
                            if (dif <= difRap)
                                firstInRap = TransformaInData(firstIn, startTime.Value.Hour, startTime.Value.Minute);
                            else
                                firstInRap = firstIn;

                            int valPlata = 0;
                            if (entPrg["INPesteDiferentaPlata"].ToString() != "") valPlata = TransformaInMinute(Convert.ToDateTime(entPrg["INPesteDiferentaPlata"]));

                            if (dif <= valPlata) firstInPaid = TransformaInData(firstIn, startTime.Value.Hour, startTime.Value.Minute);
                            if (dif > valPlata)
                            {
                                firstInPaid = TransformaInData(firstIn, startTime.Value.Hour, startTime.Value.Minute);
                                int valCalc = 0;
                                DataTable dtCalc = null;

                                string sqlTrepte = @"SELECT * FROM Ptj_ProgrameTrepte WHERE IdProgram={0} AND TipInOut = 'InPeste' AND (DATEPART(hh,OraInceput) * 60 + DATEPART(mi,OraInceput)) <= {1} AND {1} <= (DATEPART(hh,OraSfarsit) * 60 + DATEPART(mi,OraSfarsit))";
                                if (Constante.tipBD == 2) sqlTrepte = @"SELECT * FROM ""Ptj_ProgrameTrepte"" WHERE ""IdProgram""={0} AND ""TipInOut"" = 'InPeste' AND (to_number(to_char(""OraInceput"",'hh')) * 60 + to_number(to_char(""OraInceput"",'mi'))) <= {1} AND {1} <= (to_number(to_char(""OraSfarsit"",'hh')) * 60 + to_number(to_char(""OraSfarsit"",'mi')))";

                                dtCalc = General.IncarcaDT(string.Format(sqlTrepte, idProg, dif));
                                if (dtCalc.Rows.Count > 0)
                                {
                                    valCalc = TransformaInMinute(Convert.ToDateTime(dtCalc.Rows[0]["Valoare"]));
                                    DateTime? dtPen = startTime.Value.AddMinutes(valCalc);
                                    firstInPaid = TransformaInData(firstIn, dtPen.Value.Hour, dtPen.Value.Minute);
                                }
                                else
                                {
                                    firstInPaid = firstIn;
                                }
                            }

                            dif1 = Convert.ToInt32((Convert.ToDateTime(firstInPaid) - Convert.ToDateTime(startTime)).TotalMinutes);
                            dif2 = Convert.ToInt32((Convert.ToDateTime(firstInPaid) - Convert.ToDateTime(firstIn)).TotalMinutes);

                        }
                    }


                    if (varTmp)
                    {
                        if (General.Nz(entPrg["INPesteCampPlatit"], "").ToString() != "" && dif1 >= 0) ent[entPrg["INPesteCampPlatit"].ToString()] = dif1;
                        if (General.Nz(entPrg["INPesteCampNeplatit"], "").ToString() != "" && dif2 >= 0) ent[entPrg["INPesteCampNeplatit"].ToString()] = dif2;
                    }
                    else
                    {
                        if (General.Nz(entPrg["INSubCampPlatit"], "").ToString() != "" && dif1 >= 0) ent[entPrg["INSubCampPlatit"].ToString()] = dif1;
                        if (General.Nz(entPrg["INSubCampNeplatit"], "").ToString() != "" && dif2 >= 0) ent[entPrg["INSubCampNeplatit"].ToString()] = dif2;
                    }
                }

                if (firstInRap == null)
                    ent["FirstInRap"] = DBNull.Value;
                else
                    ent["FirstInRap"] = firstInRap;

                if (firstInPaid == null)
                    ent["FirstInPaid"] = DBNull.Value;
                else
                    ent["FirstInPaid"] = firstInPaid;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculIntrare");
            }
        }

        private static void CalculIesire(DataRow ent)
        {
            int dif1 = 0;
            int dif2 = 0;
            bool varTmp = true;

            DateTime? lastOutRap = null;
            DateTime? lastOutPaid = null;

            DateTime? firstIn = null;
            DateTime? lastOut = null;

            DateTime? objFirstIn = null;
            if (ent["FirstIn"] != DBNull.Value) objFirstIn = Convert.ToDateTime(ent["FirstIn"]);
            DateTime? objLastOut = null;
            if (ent["LastOut"] != DBNull.Value) objLastOut = Convert.ToDateTime(ent["LastOut"]);
            DateTime? zi = null;
            if (ent["Ziua"] != DBNull.Value) zi = Convert.ToDateTime(ent["Ziua"]);
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
            int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));

            if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
            if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);


            try
            {
                DateTime? dtSch = null;

                DateTime? startTime = null;
                DateTime? endTime = null;

                DataTable dt = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                DataRow entPrg = null;
                DataRow entCtr = null;

                DateTime? oraInSchimbare = new DateTime(2100, 1, 1, 9, 0, 0);
                DateTime? oraOutSchimbare = new DateTime(2100, 1, 1, 9, 0, 0);

                if (dt.Rows.Count > 0)
                {
                    entPrg = dt.Rows[0];

                    //Radu 31.01.2019
                    DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
                    if (dtCtr.Rows.Count > 0)
                    {
                        entCtr = dtCtr.Rows[0];
                        if (entCtr["OraInSchimbare"] != null && entCtr["OraInSchimbare"].ToString().Length > 0)
                            oraInSchimbare = new DateTime(2100, 1, 1, Convert.ToDateTime(entCtr["OraInSchimbare"]).Hour, Convert.ToDateTime(entCtr["OraInSchimbare"]).Minute, 0);
                        if (entCtr["OraOutSchimbare"] != null && entCtr["OraOutSchimbare"].ToString().Length > 0)
                            oraOutSchimbare = new DateTime(2100, 1, 1, Convert.ToDateTime(entCtr["OraOutSchimbare"]).Hour, Convert.ToDateTime(entCtr["OraOutSchimbare"]).Minute, 0);
                    }

                    if (Convert.ToInt32(General.Nz(entPrg["Flexibil"], -99)) == 1)
                    {
                        if (entPrg["OraIntrare"].ToString() != "" && entPrg["OraIesire"].ToString() != "" && firstIn != null)
                        {
                            DateTime oraInc = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, Convert.ToDateTime(entPrg["OraIntrare"]).Hour, Convert.ToDateTime(entPrg["OraIntrare"]).Minute, Convert.ToDateTime(entPrg["OraIntrare"]).Second);
                            DateTime oraSf = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);

                            if (Convert.ToDateTime(entPrg["OraIntrare"]) > Convert.ToDateTime(entPrg["OraIesire"])) oraSf = new DateTime(zi.Value.AddDays(1).Year, zi.Value.AddDays(1).Month, zi.Value.AddDays(1).Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);

                            if (Convert.ToDateTime(firstIn) < oraInc) startTime = oraInc;
                            if (oraInc < Convert.ToDateTime(firstIn) && Convert.ToDateTime(firstIn) < oraSf) startTime = firstIn;
                            if (oraSf < Convert.ToDateTime(firstIn)) startTime = oraSf;
                        }
                        else
                        {
                            startTime = firstIn;
                        }

                        if (startTime == null)
                            endTime = null;
                        else
                            endTime = startTime.Value.AddMinutes(Convert.ToInt32(General.Nz(entPrg["Norma"], -99)));
                    }
                }


                if (lastOut != null)
                {
                    //Florin 2020.04.21 - Am adaugat conditia flexibil=1
                    if (entPrg != null && Convert.ToInt32(General.Nz(entPrg["Flexibil"], -99)) == 1)
                    {
                        dtSch = lastOut;
                    }
                    else
                    {
                        if (entPrg != null && entPrg["OraIesire"] != null && entPrg["OraIesire"].ToString() != "")
                        {
                            if (new DateTime(2100, 1, 1, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, 0) <= oraOutSchimbare)
                            {
                                dtSch = lastOut;                //este schimbul 3
                            }
                            else
                            {
                                //este schimbul 1,2
                                //2016.02.22 Florin - la schimbul 2 poate sa iasa si la ora 2 a doua zi
                                if (firstIn != null && new DateTime(2100, 1, 1, lastOut.Value.Hour, lastOut.Value.Minute, 0) <= oraOutSchimbare
                                    && new DateTime(2100, 1, 1, firstIn.Value.Hour, firstIn.Value.Minute, 0) > oraInSchimbare)
                                {
                                    //dtSch = lastOut;
                                    //Radu 20.02.2017
                                    dtSch = new DateTime(zi.Value.AddDays(1).Year, zi.Value.AddDays(1).Month, zi.Value.AddDays(1).Day, lastOut.Value.Hour, lastOut.Value.Minute, lastOut.Value.Second);
                                }
                                else
                                {
                                    dtSch = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, lastOut.Value.Hour, lastOut.Value.Minute, lastOut.Value.Second);
                                }
                            }

                            if (dtSch != null)
                            {
                                //Radu 20.02.2017
                                if (new DateTime(2100, 1, 1, lastOut.Value.Hour, lastOut.Value.Minute, 0) <= oraOutSchimbare && Convert.ToDateTime(entPrg["OraIesire"]).Hour > Convert.ToDateTime(entPrg["OraIntrare"]).Hour)   //Radu 10.10.2019 - am inlocuit a doua conditie (sa fie schimbul 1 sau 2)
                                    endTime = new DateTime(dtSch.Value.AddDays(-1).Year, dtSch.Value.AddDays(-1).Month, dtSch.Value.AddDays(-1).Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);
                                else if (firstIn != null && Convert.ToDateTime(entPrg["OraIesire"]).Hour < Convert.ToDateTime(entPrg["OraIntrare"]).Hour && firstIn.Value.Day == lastOut.Value.Day)  //Radu 12.09.2019 - prima conditie = sa fie program de noapte (schimb 3), a doua conditie - nu a lucrat peste miezul noptii
                                    endTime = new DateTime(dtSch.Value.AddDays(1).Year, dtSch.Value.AddDays(1).Month, dtSch.Value.AddDays(1).Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);
                                else
                                    endTime = new DateTime(dtSch.Value.Year, dtSch.Value.Month, dtSch.Value.Day, Convert.ToDateTime(entPrg["OraIesire"]).Hour, Convert.ToDateTime(entPrg["OraIesire"]).Minute, Convert.ToDateTime(entPrg["OraIesire"]).Second);
                            }
                        }
                    }

                    if (endTime == null) return;

                    if (lastOut.Value.Hour == endTime.Value.Hour && lastOut.Value.Minute == endTime.Value.Minute)
                    {
                        lastOutPaid = lastOut;
                        lastOutRap = lastOut;
                    }
                    else
                    {
                        if (lastOut > endTime)                        //iesire tarzie
                        {
                            varTmp = false;
                            //int dif = Convert.ToInt32((Convert.ToDateTime(lastOut) - Convert.ToDateTime(endTime)).TotalMinutes);
                            int dif = CalculIntervale(ent, Convert.ToDateTime(endTime), Convert.ToDateTime(lastOut));

                            int difRap = 0;
                            if (entPrg["OUTPesteDiferentaRaportare"].ToString() != "") difRap = TransformaInMinute(Convert.ToDateTime(entPrg["OUTPesteDiferentaRaportare"]));
                            if (dif <= difRap)
                                lastOutRap = TransformaInData(dtSch, endTime.Value.Hour, endTime.Value.Minute);
                            else
                                lastOutRap = dtSch;

                            int valMin = 0;
                            int valMax = 999999999;
                            if (entPrg["OUTPesteMinPlata"].ToString() != "") valMin = TransformaInMinute(Convert.ToDateTime(entPrg["OUTPesteMinPlata"]));
                            if (entPrg["OUTPesteMaxPlata"].ToString() != "") valMax = TransformaInMinute(Convert.ToDateTime(entPrg["OUTPesteMaxPlata"]));

                            //if (dif < valMin) lastOutPaid = TransformaInData(dtSch, endTime.Value.Hour, endTime.Value.Minute);
                            //if (valMin <= dif && dif <= valMax) lastOutPaid = TransformaInData(dtSch, dtSch.Value.Hour, dtSch.Value.Minute);
                            //DateTime? dtPen = endTime.Value.AddMinutes(valMax);
                            //if (dif > valMax) lastOutPaid = TransformaInData(dtSch, dtPen.Value.Hour, dtPen.Value.Minute);

                            //Radu 20.02.2017
                            //Florin 2018.09.03  Am adaugat conditia  Convert.ToDateTime(entPrg["OraIesire"]).Hour > 23
                            if (dif < valMin) lastOutPaid = TransformaInData((lastOut.Value.Hour <= 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 23 ? dtSch.Value.AddDays(-1) : dtSch), endTime.Value.Hour, endTime.Value.Minute);
                            if (valMin <= dif && dif <= valMax) lastOutPaid = TransformaInData((lastOut.Value.Hour <= 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 23 ? dtSch.Value.AddDays(-1) : dtSch), dtSch.Value.Hour, dtSch.Value.Minute);
                            DateTime? dtPen = endTime.Value.AddMinutes(valMax);
                            if (dif > valMax) lastOutPaid = TransformaInData((lastOut.Value.Hour <= 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 9 && Convert.ToDateTime(entPrg["OraIesire"]).Hour > 23 ? dtSch.Value.AddDays(-1) : dtSch), dtPen.Value.Hour, dtPen.Value.Minute);

                            //dif1 = Convert.ToInt32((Convert.ToDateTime(lastOutPaid) - Convert.ToDateTime(endTime)).TotalMinutes);
                            //dif2 = Convert.ToInt32((Convert.ToDateTime(lastOut) - Convert.ToDateTime(lastOutPaid)).TotalMinutes);
                            dif1 = CalculIntervale(ent, Convert.ToDateTime(endTime), Convert.ToDateTime(lastOutPaid));
                            dif2 = CalculIntervale(ent, Convert.ToDateTime(lastOutPaid), Convert.ToDateTime(lastOut));
                        }
                        else                                          //iesire anticipata
                        {

                            int dif = Convert.ToInt32((Convert.ToDateTime(endTime) - Convert.ToDateTime(lastOut)).TotalMinutes);

                            int difRap = 0;
                            if (entPrg["OUTSubDiferentaRaportare"].ToString() != "") difRap = TransformaInMinute(Convert.ToDateTime(entPrg["OUTSubDiferentaRaportare"]));
                            if (dif <= difRap)
                                lastOutRap = TransformaInData(dtSch, endTime.Value.Hour, endTime.Value.Minute);
                            else
                                lastOutRap = dtSch;

                            int valPlata = 0;
                            if (entPrg["OUTSubDiferentaPlata"].ToString() != "") valPlata = TransformaInMinute(Convert.ToDateTime(entPrg["OUTSubDiferentaPlata"]));

                            if (dif <= valPlata) lastOutPaid = TransformaInData(dtSch, endTime.Value.Hour, endTime.Value.Minute);
                            if (dif > valPlata)
                            {
                                lastOutPaid = TransformaInData(dtSch, endTime.Value.Hour, endTime.Value.Minute);
                                int valCalc = 0;
                                DataTable dtCalc = null;

                                string sqlTrepte = @"SELECT * FROM Ptj_ProgrameTrepte WHERE IdProgram={0} AND TipInOut = 'OUTSub' AND (DATEPART(hh,OraInceput) * 60 + DATEPART(mi,OraInceput)) <= {1} AND {1} <= (DATEPART(hh,OraSfarsit) * 60 + DATEPART(mi,OraSfarsit))";
                                if (Constante.tipBD == 2) sqlTrepte = @"SELECT * FROM ""Ptj_ProgrameTrepte"" WHERE ""IdProgram""={0} AND ""TipInOut"" = 'OUTSub' AND (to_number(to_char(""OraInceput"",'hh')) * 60 + to_number(to_char(""OraInceput"",'mi'))) <= {1} AND {1} <= (to_number(to_char(""OraSfarsit"",'hh')) * 60 + to_number(to_char(""OraSfarsit"",'mi')))";

                                dtCalc = General.IncarcaDT(string.Format(sqlTrepte, idProg, dif));

                                if (dtCalc.Rows.Count > 0)
                                {
                                    valCalc = TransformaInMinute(Convert.ToDateTime(dtCalc.Rows[0]["Valoare"]));
                                    DateTime? dtPen = endTime.Value.AddMinutes(-1 * valCalc);
                                    lastOutPaid = TransformaInData(dtSch, dtPen.Value.Hour, dtPen.Value.Minute);
                                }
                                else
                                {
                                    lastOutPaid = dtSch;
                                }
                            }

                            dif1 = Convert.ToInt32((Convert.ToDateTime(endTime) - Convert.ToDateTime(lastOutPaid)).TotalMinutes);
                            dif2 = Convert.ToInt32((Convert.ToDateTime(lastOut) - Convert.ToDateTime(lastOutPaid)).TotalMinutes);
                        }
                    }


                    if (varTmp)
                    {

                        if (General.Nz(entPrg["OUTSubCampPlatit"], "").ToString() != "" && dif1 >= 0) ent[entPrg["OUTSubCampPlatit"].ToString()] = dif1;
                        if (General.Nz(entPrg["OUTSubCampNeplatit"], "").ToString() != "" && dif2 >= 0) ent[entPrg["OUTSubCampNeplatit"].ToString()] = dif2;
                    }
                    else
                    {

                        if (General.Nz(entPrg["OUTPesteCampPlatit"], "").ToString() != "" && dif1 >= 0) ent[entPrg["OUTPesteCampPlatit"].ToString()] = dif1;
                        if (General.Nz(entPrg["OUTPesteCampNeplatit"], "").ToString() != "" && dif2 >= 0) ent[entPrg["OUTPesteCampNeplatit"].ToString()] = dif2;
                    }
                }

                if (lastOutRap == null)
                    ent["LastOutRap"] = DBNull.Value;
                else
                    ent["LastOutRap"] = lastOutRap;

                if (lastOutPaid == null)
                    ent["LastOutPaid"] = DBNull.Value;
                else
                    ent["LastOutPaid"] = lastOutPaid;


            }
            catch (Exception ex)
            {
                //General.MemoreazaEroarea("marca=" + marca.ToString() + " ############ " + "Q=" + q.ToString() + " ############ " + ex.ToString(), "Calcul Ceasuri", "CalculIesire");
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculIesire");
            }
        }

        private static int CalculPauzaBis(DataRow ent)
        {
            DateTime? zi = (DateTime?)ent["Ziua"];
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));

            int tp = 0;
            int tpd = 0;

            try
            {
                DataRow entProg = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                DataTable dtPauza = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ProgramePauza"" WHERE ""IdProgram""={0}", idProg));

                //Florin 2020.06.22
                if (entProg == null) return tpd;

                int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));
                DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
                object rap = 1;
                if (dtCtr.Rows.Count > 0)
                {
                    rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                    if (rap != null)
                        rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                    else
                        rap = 1;
                }

                try
                {
                    if (entProg["PauzaTimp"] != DBNull.Value) tp = TransformaInMinute(Convert.ToDateTime(entProg["PauzaTimp"]));
                    if (Convert.ToInt32(General.Nz(entProg["PauzaDedusa"], -99)) == 1) tpd = TransformaInMinute(Convert.ToDateTime(entProg["PauzaTimp"]));
                }
                catch (Exception)
                {
                }

                //calculam pauza scutita
                int tps = 0;
                DataRow drScut = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                if (drScut != null && drScut["PauzaScutita"] != null && drScut["PauzaScutita"].ToString() != "") tps = TransformaInMinute(Convert.ToDateTime(entProg["PauzaScutita"]));
                if (Convert.ToInt32(ent["TimpPauzaReal"]) < tps) tps = Convert.ToInt32(ent["TimpPauzaReal"]);


                int tpInterval = 0;
                int tpdInterval = 0;

                for (int i = 1; i < 20; i++)
                {
                    var gridIn = ent["In" + (i + 1)];
                    var gridOut = ent["Out" + i];

                    if (gridIn.ToString() != "" && gridOut.ToString() != "")
                    {
                        //in cazul in care nu este data valida
                        try
                        {
                            DataRow entFil = null;
                            foreach (DataRow dr in dtPauza.Rows)
                            {
                                try
                                {
                                    if (Convert.ToInt32(General.Nz(dr["FaraMarja"], -99)) == 1)
                                    {
                                        if (Convert.ToDateTime(dr["OraInceput"]) <= Convert.ToDateTime(gridIn) && Convert.ToDateTime(gridOut) <= Convert.ToDateTime(dr["OraSfarsit"]))
                                        {
                                            DateTime tmp1 = Convert.ToDateTime(dr["OraInceput"]);
                                            if (Convert.ToDateTime(dr["OraInceput"]) < Convert.ToDateTime(gridOut)) tmp1 = Convert.ToDateTime(gridOut);

                                            DateTime tmp2 = Convert.ToDateTime(dr["OraSfarsit"]);
                                            if (Convert.ToDateTime(gridIn) < Convert.ToDateTime(dr["OraSfarsit"])) tmp2 = Convert.ToDateTime(gridIn);

                                            int varTp = Convert.ToInt32((tmp2 - tmp1).TotalMinutes);
                                            tpInterval += varTp;
                                            if (Convert.ToInt32(General.Nz(dr["TimpDedus"], -99)) == 1)
                                                tpdInterval += varTp;
                                            else
                                                tpdInterval += (-1 * varTp);

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        DateTime? incDeLa = TransformaInData(zi, dr["OraInceputDeLa"].ToString());
                                        DateTime? incLa = TransformaInData(zi, dr["OraInceputLa"].ToString());
                                        DateTime? sfDeLa = TransformaInData(zi, dr["OraSfarsitDeLa"].ToString());
                                        DateTime? sfLa = TransformaInData(zi, dr["OraSfarsitLa"].ToString());

                                        if (dr["DeNoapte"].ToString() == "1")
                                        {
                                            if (Convert.ToInt32(rap) == 1)
                                            {
                                                //ce trece de miezul noptii inseamna ca este a doua zi (pt schimbul de noapte)
                                                DateTime dt00 = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, 23, 59, 59);
                                                if (incDeLa > dt00) incDeLa = TransformaInData(zi.Value.AddDays(1), dr["OraInceputDeLa"].ToString());
                                                if (incLa > dt00) incDeLa = TransformaInData(zi.Value.AddDays(1), dr["OraInceputLa"].ToString());
                                                if (sfDeLa > dt00) incDeLa = TransformaInData(zi.Value.AddDays(1), dr["OraSfarsitDeLa"].ToString());
                                                if (sfLa > dt00) incDeLa = TransformaInData(zi.Value.AddDays(1), dr["OraSfarsitLa"].ToString());
                                            }
                                            else
                                            {
                                                DateTime dt00 = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, 23, 59, 59);
                                                if (incDeLa > dt00) incDeLa = TransformaInData(zi.Value, dr["OraInceputDeLa"].ToString());
                                                if (incLa > dt00) incDeLa = TransformaInData(zi.Value, dr["OraInceputLa"].ToString());
                                                if (sfDeLa > dt00) incDeLa = TransformaInData(zi.Value, dr["OraSfarsitDeLa"].ToString());
                                                if (sfLa > dt00) incDeLa = TransformaInData(zi.Value, dr["OraSfarsitLa"].ToString());
                                            }
                                        }

                                        if (incDeLa <= Convert.ToDateTime(gridOut) && Convert.ToDateTime(gridOut) <= incLa && sfDeLa <= Convert.ToDateTime(gridIn) && Convert.ToDateTime(gridIn) <= sfLa)
                                        {
                                            entFil = dr;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (entFil != null)
                            {
                                int varTp = 0;
                                varTp = TransformaInMinute(Convert.ToDateTime(gridIn)) - TransformaInMinute(Convert.ToDateTime(gridOut));
                                if (varTp < TransformaInMinute(Convert.ToDateTime(entFil["TimpMin"]))) varTp = TransformaInMinute(Convert.ToDateTime(entFil["TimpMin"]));
                                if (varTp > TransformaInMinute(Convert.ToDateTime(entFil["TimpMax"]))) varTp = TransformaInMinute(Convert.ToDateTime(entFil["TimpMax"]));

                                tpInterval += varTp;
                                if (Convert.ToInt32(General.Nz(entFil["TimpDedus"], -99)) == 1)
                                    tpdInterval += varTp;
                                else
                                    tpdInterval += (-1 * varTp);
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                tp = tp + tpInterval;
                tpd = tpd + tpdInterval - tps;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculPauzaBis");
            }

            return tpd;
        }

        private static void CalculOreLucrateBis(DataRow ent, int timpPauzaDedusa, out int rez, out int minuteIN, out int minuteOUT)
        {
            minuteIN = 0;
            minuteOUT = 0;

            rez = 0;
            int norma = 0;
            int pontare = 1;

            DateTime? firstIn = null;
            DateTime? lastOut = null;

            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
            object objFirstIn = ent["FirstIn"] == DBNull.Value ? null : ent["FirstIn"];
            object objLastOut = ent["LastOut"] == DBNull.Value ? null : ent["LastOut"];
            object firstInPaid = ent["FIrstInPaid"] == DBNull.Value ? null : ent["FIrstInPaid"];
            object lastOutPaid = ent["LastOutPaid"] == DBNull.Value ? null : ent["LastOutPaid"];

            try
            {
                if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
                if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);
            }
            catch (Exception)
            {
            }

            try
            {
                if (firstInPaid == null) firstInPaid = firstIn;
                if (lastOutPaid == null)
                    lastOutPaid = lastOut;

                DateTime zi = new DateTime(1900, 1, 1);
                if (ent["Ziua"].ToString() != "")
                {
                    try
                    {
                        DateTime dtTmp = Convert.ToDateTime(ent["Ziua"]);
                        zi = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day);
                    }
                    catch (Exception)
                    {
                    }
                }

                DataRow entPont = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));

                if (entPont != null)
                {
                    norma = Convert.ToInt32(General.Nz(entPont["Norma"], -99));
                    pontare = Convert.ToInt32(General.Nz(entPont["TipPontare"], -99));
                }

                switch (pontare)
                {
                    case 1:                     //pontare automata
                        rez = norma;
                        break;
                    case 2:                     //Pontare automata la minim o citire card
                        if (firstIn != null || lastOut != null) rez = norma;
                        break;
                    case 3:                     //Pontare doar prima intrare si ultima iesire
                        try
                        {
                            if (firstIn != null && lastOut != null)
                            {
                                int tp = Convert.ToInt32((Convert.ToDateTime(lastOutPaid) - Convert.ToDateTime(firstInPaid)).TotalMinutes);
                                try
                                {
                                    if (entPont["OreLucrateMin"].ToString() != "" && TransformaInMinute(Convert.ToDateTime(entPont["OreLucrateMin"])) <= tp) tp = tp - timpPauzaDedusa;
                                }
                                catch (Exception) { }
                                if (tp >= 0) rez = tp;
                            }
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case 4:                     //Pontare toate intrarile si iesirile
                        int total = 0;
                        int os = 0;

                        if (firstIn != null && lastOut != null)
                        {
                            for (int i = 1; i <= 20; i++)
                            {
                                if (ent["In" + i] != DBNull.Value && ent["Out" + i] != DBNull.Value)
                                {
                                    try
                                    {
                                        //Florin 2016.11.07 Begin ###################################
                                        //aplicam gratierea

                                        DataTable dtProg = General.IncarcaDT(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""=" + idProg);
                                        if (dtProg.Rows.Count > 0 && dtProg.Rows[0]["OraIntrare"] != DBNull.Value && dtProg.Rows[0]["OraIesire"] != DBNull.Value)
                                        {
                                            //gratiere pentru intrare tarzie
                                            if (Convert.ToDateTime(ent["In" + i]) == firstIn && TransformaInMinute(Convert.ToDateTime(ent["In" + i])) > TransformaInMinute(Convert.ToDateTime(dtProg.Rows[0]["OraIntrare"])))
                                            {
                                                int dif = TransformaInMinute(Convert.ToDateTime(ent["In" + i])) - TransformaInMinute(Convert.ToDateTime(dtProg.Rows[0]["OraIntrare"]));
                                                int grat = TransformaInMinute(dtProg.Rows[0]["InPesteDiferentaPlata"]);
                                                if (dif <= grat)
                                                {
                                                    minuteIN = dif;
                                                }
                                            }

                                            //gratiere pentru iesire anticipata
                                            if (Convert.ToDateTime(ent["Out" + i]) == lastOut && TransformaInMinute(Convert.ToDateTime(ent["Out" + i])) < TransformaInMinute(Convert.ToDateTime(dtProg.Rows[0]["OraIesire"])))
                                            {
                                                int dif = TransformaInMinute(Convert.ToDateTime(dtProg.Rows[0]["OraIesire"])) - TransformaInMinute(Convert.ToDateTime(ent["Out" + i]));
                                                int grat = TransformaInMinute(dtProg.Rows[0]["OutSubDiferentaPlata"]);
                                                if (dif <= grat)
                                                {
                                                    minuteOUT = dif;
                                                }
                                            }
                                        }


                                        //Florin 2016.12.06  Begin
                                        //s-a adaugat else-ul si s-a modificat si prima ramura pentru a calcula timpul lucrat din afara intervalului orar FirstInPaid - LatOutPaid
                                        //Este cazul de la Vrancart unde vor sa stie cate ore au lucrat in timpul programului si cate suplimentar fara compensare
                                        //de ex: s-a lucart 6 ore intre 07 - 15:30 si apoi a mai lucrat 2 ore dupa 15:30; deci 6 ore lucrate si 2 ore suplimnetare

                                        //intersectam fiecare interval In-Out cu intervalul de plata firstInPaid - lastOutPaid
                                        if (Convert.ToDateTime(ent["In" + i]).AddMinutes(-1 * minuteIN) <= Convert.ToDateTime(lastOutPaid) && Convert.ToDateTime(firstInPaid) <= Convert.ToDateTime(ent["Out" + i]).AddMinutes(minuteOUT))
                                        {
                                            DateTime gridIn = Convert.ToDateTime(ent["In" + i]).AddMinutes(-1 * minuteIN) > Convert.ToDateTime(firstInPaid) ? Convert.ToDateTime(ent["In" + i]).AddMinutes(-1 * minuteIN) : Convert.ToDateTime(firstInPaid);
                                            DateTime gridOut = Convert.ToDateTime(ent["Out" + i]).AddMinutes(minuteOUT) < Convert.ToDateTime(lastOutPaid) ? Convert.ToDateTime(ent["Out" + i]).AddMinutes(minuteOUT) : Convert.ToDateTime(lastOutPaid);

                                            int difTmp = Convert.ToInt32((Convert.ToDateTime(gridOut) - Convert.ToDateTime(gridIn)).TotalMinutes);
                                            if (difTmp > 0) total += difTmp;

                                            int difOs = Convert.ToInt32((Convert.ToDateTime(ent["Out" + i]) - Convert.ToDateTime(ent["In" + i])).TotalMinutes) - difTmp;
                                            if (difOs > 0) os += difOs;
                                        }
                                        else
                                        {
                                            int difOs = Convert.ToInt32((Convert.ToDateTime(ent["Out" + i]) - Convert.ToDateTime(ent["In" + i])).TotalMinutes);
                                            if (difOs > 0) os += difOs;
                                        }

                                        //Florin 2020.05.28
                                        if (ent.Table.Columns["TimpOSReal"] != null)
                                            ent["TimpOSReal"] = os;

                                        //Florin 2016.12.06  End
                                        //Florin 2016.11.07 End  ###################################
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                            try
                            {
                                if (entPont["OreLucrateMin"].ToString() != "" && TransformaInMinute(Convert.ToDateTime(entPont["OreLucrateMin"])) <= total) total = total - timpPauzaDedusa;
                            }
                            catch (Exception) { }

                            if (total > 0) rez = total;
                        }
                        break;
                    case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
                        int pz = 0;
                        object pzIn = DBNull.Value;
                        object pzOut = DBNull.Value;

                        if (firstIn != null && lastOut != null)
                        {
                            for (int i = 1; i < 20; i++)
                            {
                                pzIn = ent["In" + (i + 1)];
                                pzOut = ent["Out" + i];

                                if (pzIn != DBNull.Value && pzOut != DBNull.Value)
                                {
                                    try
                                    {
                                        int tmpPz = Convert.ToInt32((Convert.ToDateTime(pzIn) - Convert.ToDateTime(pzOut)).TotalMinutes);
                                        int tmpMin = TransformaInMinute(Convert.ToDateTime(entPont["PauzaMin"]));
                                        if (tmpPz >= tmpMin) pz += tmpPz;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }


                            try
                            {
                                int tp = Convert.ToInt32((Convert.ToDateTime(lastOutPaid) - Convert.ToDateTime(firstInPaid)).TotalMinutes - pz);
                                try
                                {
                                    if (entPont["OreLucrateMin"].ToString() != "" && TransformaInMinute(Convert.ToDateTime(entPont["OreLucrateMin"])) <= tp) tp = tp - timpPauzaDedusa;
                                }
                                catch (Exception) { }
                                if (tp >= 0) rez = tp;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculOreLucrateBis");
            }

            //return rez;
        }

        private static int CalculOreNormale(DataRow ent, int oreLucrate)
        {
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));

            int oreCalc = 0;
            int norma = 0;
            int rotunjire = 1;

            try
            {
                DataRow entPrg = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));

                if (entPrg != null)
                {
                    norma = Convert.ToInt32(General.Nz(entPrg["Norma"], -99));
                    rotunjire = Convert.ToInt32(General.Nz(entPrg["ONRotunjire"], -99));
                }

                if (oreLucrate >= norma)
                {
                    oreCalc = norma;
                }
                else
                {
                    switch (rotunjire)
                    {
                        case 1:                             //rotunjire la minute
                            oreCalc = oreLucrate;
                            break;
                        case 2:                             //rotunjire la ora
                            {
                                if ((oreLucrate % 60) >= 30)
                                    oreCalc = ((oreLucrate / 60) + 1) * 60;
                                else
                                    oreCalc = (oreLucrate / 60) * 60;
                            }
                            break;
                        case 3:                             //trunchiere la ora
                            oreCalc = (oreLucrate / 60) * 60;
                            break;
                        case 4:                             //rotunjire la 45 minute
                            {
                                if ((oreLucrate % 60) >= 45)
                                    oreCalc = ((oreLucrate / 60) + 1) * 60;
                                else
                                    oreCalc = (oreLucrate / 60) * 60;
                            }
                            break;
                        case 5:                             //rotunjire la 10 minute
                            oreCalc = TrunchiereLa10Minute(oreLucrate);
                            break;
                        case 6:                             //rotunjire la 5 minute
                            oreCalc = TrunchiereLa5Minute(oreLucrate);
                            break;
                    }
                }

                try
                {
                    if (entPrg != null && entPrg["ONCamp"] != null && entPrg["ONCamp"].ToString() != "") ent[entPrg["ONCamp"].ToString()] = oreCalc;
                }
                catch (Exception)
                {
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculOreNormale");
            }

            return oreCalc;
        }

        private static int CalculOreSuplimentare(DataRow ent, int oreLucrate)
        {
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
            DateTime? zi = (DateTime?)ent["Ziua"];

            int sub = 0;
            int n = 0;
            int peste = 0;
            int norma = 0;
            int rotunjire = 1;

            try
            {
                DataRow entPrg = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));

                if (entPrg != null)
                {
                    norma = Convert.ToInt32(General.Nz(entPrg["Norma"], -99));
                    rotunjire = Convert.ToInt32(General.Nz(entPrg["OSRotunjire"], -99));
                }

                int os = Convert.ToInt32(oreLucrate - norma);
                if (os > 0)
                {
                    var valMin = entPrg["OSValMin"].ToString() == "" ? 0 : TransformaInMinute(Convert.ToDateTime(entPrg["OSValMin"]));
                    var valMax = entPrg["OSValMax"].ToString() == "" ? 999999999 : TransformaInMinute(Convert.ToDateTime(entPrg["OSValMax"]));

                    if (os < valMin) sub = os;

                    if (valMin <= os && os <= valMax) n = os;

                    if (valMax < os)
                    {
                        peste = os - valMax;
                        n = valMax;
                    }

                    switch (rotunjire)
                    {
                        case 1:                             //rotunjire la minute
                            //NOP
                            break;
                        case 2:                             //rotunjire la ora
                            try
                            {
                                if ((sub % 60) >= 30)
                                    sub = ((sub / 60) + 1) * 60;
                                else
                                    sub = (sub / 60) * 60;

                                if ((Convert.ToInt32(n) % 60) >= 30)
                                    n = ((n / 60) + 1) * 60;
                                else
                                    n = (n / 60) * 60;

                                if ((Convert.ToInt32(peste) % 60) >= 30)
                                    peste = ((peste / 60) + 1) * 60;
                                else
                                    peste = (peste / 60) * 60;
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        case 3:                             //trunchiere la ora
                            sub = (sub / 60) * 60;
                            n = (n / 60) * 60;
                            peste = (peste / 60) * 60;
                            break;
                        case 4:                             //rotunjire la 45 minute
                            try
                            {
                                if ((sub % 60) >= 45)
                                    sub = ((sub / 60) + 1) * 60;
                                else
                                    sub = (sub / 60) * 60;

                                if ((Convert.ToInt32(n) % 60) >= 45)
                                    n = ((n / 60) + 1) * 60;
                                else
                                    n = (n / 60) * 60;

                                if ((Convert.ToInt32(peste) % 60) >= 45)
                                    peste = ((peste / 60) + 1) * 60;
                                else
                                    peste = (peste / 60) * 60;
                            }
                            catch (Exception)
                            {
                            }
                            break;
                        case 5:                             //rotunjire la 10 minute
                            sub = TrunchiereLa10Minute(sub);
                            n = TrunchiereLa10Minute(n);
                            peste = TrunchiereLa10Minute(peste);
                            break;
                        case 6:                             //rotunjire la 5 minute
                            sub = TrunchiereLa5Minute(sub);
                            n = TrunchiereLa5Minute(n);
                            peste = TrunchiereLa5Minute(peste);
                            break;

                    }

                    if (entPrg["OSCampSub"].ToString() != "") ent[entPrg["OSCampSub"].ToString()] = sub;
                    if (entPrg["OSCamp"].ToString() != "") ent[entPrg["OSCamp"].ToString()] = n;
                    if (entPrg["OSCampPeste"].ToString() != "") ent[entPrg["OSCampPeste"].ToString()] = peste;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculOreSuplimentare");
            }

            return n;
        }

        private static void CalculOreNoapte(DataRow ent, int minuteIN, int minuteOUT)
        {
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
            var ert = ent["FirstInPaid"].GetType();

            int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));
            DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
            object rap = 1;
            if (dtCtr.Rows.Count > 0)
            {
                rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                if (rap != null)
                    rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                else
                    rap = 1;
            }

            DateTime? objFirstIn = null;
            if (ent["FirstIn"] != DBNull.Value) objFirstIn = Convert.ToDateTime(ent["FirstIn"]);
            DateTime? objLastOut = null;
            if (ent["LastOut"] != DBNull.Value) objLastOut = Convert.ToDateTime(ent["LastOut"]);
            DateTime? firstInPaid = null;
            if (ent["FirstInPaid"] != DBNull.Value) firstInPaid = Convert.ToDateTime(ent["FirstInPaid"]);
            DateTime? lastOutPaid = null;
            if (ent["LastOutPaid"] != DBNull.Value) lastOutPaid = Convert.ToDateTime(ent["LastOutPaid"]);


            int pontare = 1;

            DateTime? firstIn = null;
            DateTime? lastOut = null;

            try
            {
                if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
                if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);
            }
            catch (Exception)
            {
            }

            try
            {
                if (idProg == -99) return;

                //obtinem intervalul de intersectat
                if (firstInPaid == null) firstInPaid = firstIn;
                if (lastOutPaid == null) lastOutPaid = lastOut;

                DataRow entPont = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                if (entPont != null) pontare = Convert.ToInt32(General.Nz(entPont["TipPontare"], -99));

                switch (pontare)
                {
                    case 1:                     //pontare automata
                    case 2:                     //Pontare automata la minim o citire card
                        try
                        {
                            DateTime oraInc = Convert.ToDateTime(entPont["OraIntrare"]);
                            DateTime oraSf = Convert.ToDateTime(entPont["OraIesire"]);
                            DateTime dt = Convert.ToDateTime(ent["Ziua"]);

                            oraInc = new DateTime(dt.Year, dt.Month, dt.Day, oraInc.Hour, oraInc.Minute, oraInc.Second);
                            oraSf = new DateTime(dt.Year, dt.Month, dt.Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

                            if (oraInc > oraSf) oraSf = new DateTime(dt.AddDays(1).Year, dt.AddDays(1).Month, dt.AddDays(1).Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

                            firstInPaid = oraInc;
                            lastOutPaid = oraSf;
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case 3:                     //Pontare doar prima intrare si ultima iesire
                        break;
                    case 4:                     //Pontare toate intrarile si iesirile
                        break;
                    case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
                        break;
                }


                //pt fiecare linie din alte ore facem, intersectia
                DataTable dtAlte = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ProgrameOreNoapte"" WHERE ""IdProgram""={0}", idProg));

                foreach (DataRow entOre in dtAlte.Rows)
                {
                    switch (pontare)
                    {
                        case 1:                     //pontare automata
                        case 2:                     //Pontare automata la minim o citire card
                        case 3:                     //Pontare doar prima intrare si ultima iesire
                            if (firstInPaid != null && lastOutPaid != null && entOre["OraInceput"].ToString() != "" && entOre["OraSfarsit"].ToString() != "")
                            {
                                int timp = CalculTimp(Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap));
                                if (timp > 0) CalculMinute(ent, entOre, timp);
                            }
                            break;
                        case 4:                     //Pontare toate intrarile si iesirile
                            if (firstInPaid != null && lastOutPaid != null)
                            {
                                int total = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap), minuteIN, minuteOUT);
                                if (total > 0) CalculMinute(ent, entOre, total);
                            }
                            break;
                        case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
                            if (firstIn != null && lastOut != null && firstInPaid != null && lastOutPaid != null)
                            {
                                int interv = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap), minuteIN, minuteOUT);
                                int pz = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), 2, minuteIN, minuteOUT, TransformaInMinute(Convert.ToDateTime(entPont["PauzaMin"])), Convert.ToInt32(rap));
                                int total = interv + pz;

                                if (total > 0) CalculMinute(ent, entOre, total);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculAlteOre");
            }
        }

        //private static void CalculAlteOreH5(DataRow ent)
        //{
        //    int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
        //    var ert = ent["FirstInPaid"].GetType();

        //    int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));
        //    DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
        //    object rap = 1;
        //    if (dtCtr.Rows.Count > 0)
        //    {
        //        rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
        //        if (rap != null)
        //            rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
        //        else
        //            rap = 1;
        //    }

        //    DateTime? objFirstIn = null;
        //    if (ent["FirstIn"] != DBNull.Value) objFirstIn = Convert.ToDateTime(ent["FirstIn"]);
        //    DateTime? objLastOut = null;
        //    if (ent["LastOut"] != DBNull.Value) objLastOut = Convert.ToDateTime(ent["LastOut"]);
        //    DateTime? firstInPaid = null;
        //    if (ent["FirstInPaid"] != DBNull.Value) firstInPaid = Convert.ToDateTime(ent["FirstInPaid"]);
        //    DateTime? lastOutPaid = null;
        //    if (ent["LastOutPaid"] != DBNull.Value) lastOutPaid = Convert.ToDateTime(ent["LastOutPaid"]);

        //    int pontare = 1;

        //    DateTime? firstIn = null;
        //    DateTime? lastOut = null;

        //    try
        //    {
        //        if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
        //        if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    try
        //    {
        //        if (idProg == -99) return;

        //        //obtinem intervalul de intersectat
        //        if (firstInPaid == null) firstInPaid = firstIn;
        //        if (lastOutPaid == null) lastOutPaid = lastOut;

        //        DataRow entPont = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
        //        if (entPont != null) pontare = Convert.ToInt32(General.Nz(entPont["TipPontare"], -99));

        //        switch (pontare)
        //        {
        //            case 1:                     //pontare automata
        //            case 2:                     //Pontare automata la minim o citire card
        //                try
        //                {
        //                    DateTime oraInc = Convert.ToDateTime(entPont["OraIntrare"]);
        //                    DateTime oraSf = Convert.ToDateTime(entPont["OraIesire"]);
        //                    DateTime dt = Convert.ToDateTime(ent["Ziua"]);

        //                    oraInc = new DateTime(dt.Year, dt.Month, dt.Day, oraInc.Hour, oraInc.Minute, oraInc.Second);
        //                    oraSf = new DateTime(dt.Year, dt.Month, dt.Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

        //                    if (oraInc > oraSf) oraSf = new DateTime(dt.AddDays(1).Year, dt.AddDays(1).Month, dt.AddDays(1).Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

        //                    firstInPaid = oraInc;
        //                    lastOutPaid = oraSf;
        //                }
        //                catch (Exception)
        //                {
        //                }
        //                break;
        //            case 3:                     //Pontare doar prima intrare si ultima iesire
        //                break;
        //            case 4:                     //Pontare toate intrarile si iesirile
        //                break;
        //            case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
        //                break;
        //        }


        //        //pt fiecare linie din alte ore facem, intersectia
        //        DataTable dtAlte = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ProgrameAlteOre"" WHERE ""IdProgram""={0}", idProg));

        //        foreach (DataRow entOre in dtAlte.Rows)
        //        {
        //            switch (pontare)
        //            {
        //                case 1:                     //pontare automata
        //                case 2:                     //Pontare automata la minim o citire card
        //                case 3:                     //Pontare doar prima intrare si ultima iesire
        //                    if (firstInPaid != null && lastOutPaid != null && entOre["OraInceput"].ToString() != "" && entOre["OraSfarsit"].ToString() != "")
        //                    {
        //                        int timp = CalculTimp(Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap));
        //                        if (timp > 0) CalculMinute(ent, entOre, timp);
        //                    }
        //                    break;
        //                case 4:                     //Pontare toate intrarile si iesirile
        //                    if (firstInPaid != null && lastOutPaid != null)
        //                    {
        //                        int total = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap));
        //                        if (total > 0) CalculMinute(ent, entOre, total);
        //                    }
        //                    break;
        //                case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
        //                    if (firstIn != null && lastOut != null && firstInPaid != null && lastOutPaid != null)
        //                    {
        //                        int interv = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap));
        //                        int pz = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), 2, TransformaInMinute(Convert.ToDateTime(entPont["PauzaMin"])), Convert.ToInt32(rap));
        //                        int total = interv + pz;

        //                        if (total > 0) CalculMinute(ent, entOre, total);
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculAlteOre");
        //    }
        //}

        private static void CalculAlteOre(DataRow ent, int minuteIN, int minuteOUT)
        {
            int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
            var ert = ent["FirstInPaid"].GetType();

            int idCtr = Convert.ToInt32(General.Nz(ent["IdContract"], -99));
            DataTable dtCtr = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""={0}", idCtr));
            object rap = 1;
            if (dtCtr.Rows.Count > 0)
            {
                rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                if (rap != null)
                    rap = dtCtr.Rows[0]["TipRaportareOreNoapte"];
                else
                    rap = 1;
            }

            DateTime? objFirstIn = null;
            if (ent["FirstIn"] != DBNull.Value) objFirstIn = Convert.ToDateTime(ent["FirstIn"]);
            DateTime? objLastOut = null;
            if (ent["LastOut"] != DBNull.Value) objLastOut = Convert.ToDateTime(ent["LastOut"]);
            DateTime? firstInPaid = null;
            if (ent["FirstInPaid"] != DBNull.Value) firstInPaid = Convert.ToDateTime(ent["FirstInPaid"]);
            DateTime? lastOutPaid = null;
            if (ent["LastOutPaid"] != DBNull.Value) lastOutPaid = Convert.ToDateTime(ent["LastOutPaid"]);

            int pontare = 1;

            DateTime? firstIn = null;
            DateTime? lastOut = null;

            try
            {
                if (objFirstIn != null) firstIn = Convert.ToDateTime(objFirstIn);
                if (objLastOut != null) lastOut = Convert.ToDateTime(objLastOut);
            }
            catch (Exception)
            {
            }

            try
            {
                if (idProg == -99) return;

                //obtinem intervalul de intersectat
                if (firstInPaid == null) firstInPaid = firstIn;
                if (lastOutPaid == null) lastOutPaid = lastOut;

                DataRow entPont = General.IncarcaDR(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                if (entPont != null) pontare = Convert.ToInt32(General.Nz(entPont["TipPontare"], -99));

                switch (pontare)
                {
                    case 1:                     //pontare automata
                    case 2:                     //Pontare automata la minim o citire card
                        try
                        {
                            DateTime oraInc = Convert.ToDateTime(entPont["OraIntrare"]);
                            DateTime oraSf = Convert.ToDateTime(entPont["OraIesire"]);
                            DateTime dt = Convert.ToDateTime(ent["Ziua"]);

                            oraInc = new DateTime(dt.Year, dt.Month, dt.Day, oraInc.Hour, oraInc.Minute, oraInc.Second);
                            oraSf = new DateTime(dt.Year, dt.Month, dt.Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

                            if (oraInc > oraSf) oraSf = new DateTime(dt.AddDays(1).Year, dt.AddDays(1).Month, dt.AddDays(1).Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

                            firstInPaid = oraInc;
                            lastOutPaid = oraSf;
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case 3:                     //Pontare doar prima intrare si ultima iesire
                        break;
                    case 4:                     //Pontare toate intrarile si iesirile
                        break;
                    case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
                        break;
                }


                //pt fiecare linie din alte ore facem, intersectia
                DataTable dtAlte = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_ProgrameAlteOre"" WHERE ""IdProgram""={0}", idProg));

                foreach (DataRow entOre in dtAlte.Rows)
                {
                    switch (pontare)
                    {
                        case 1:                     //pontare automata
                        case 2:                     //Pontare automata la minim o citire card
                        case 3:                     //Pontare doar prima intrare si ultima iesire
                            if (firstInPaid != null && lastOutPaid != null && entOre["OraInceput"].ToString() != "" && entOre["OraSfarsit"].ToString() != "")
                            {
                                int timp = CalculTimp(Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap));
                                if (timp > 0) CalculMinute(ent, entOre, timp);
                            }
                            break;
                        case 4:                     //Pontare toate intrarile si iesirile
                            if (firstInPaid != null && lastOutPaid != null)
                            {
                                int total = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap), minuteIN, minuteOUT);
                                if (total > 0) CalculMinute(ent, entOre, total);
                            }
                            break;
                        case 5:                     //Pontare prima intrare, ultima iesire minus pauzele mai mari de x minute
                            if (firstIn != null && lastOut != null && firstInPaid != null && lastOutPaid != null)
                            {
                                int interv = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), Convert.ToInt32(rap), minuteIN, minuteOUT);
                                int pz = CalculIntervale(ent, Convert.ToDateTime(entOre["OraInceput"]), Convert.ToDateTime(entOre["OraSfarsit"]), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(firstInPaid), Convert.ToDateTime(lastOutPaid), 2, minuteIN, minuteOUT, TransformaInMinute(Convert.ToDateTime(entPont["PauzaMin"])), Convert.ToInt32(rap));
                                int total = interv + pz;

                                if (total > 0) CalculMinute(ent, entOre, total);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculAlteOre");
            }
        }

        private static int CalculIntervale(DataRow ent, DateTime OraInceput, DateTime OraSfarsit, DateTime ziua, DateTime firstInPaid, DateTime lastOutPaid, int rap, int minuteIN, int minuteOUT, int tip = 1, int pauzaMin = 0)
        {
            //tip 1 = intervale IN OUT   -   ore lucrate
            //tip 2 = intervale OUT IN < pauz  - pauze

            int rez = 0;

            int ultima = 0;
            //bool prima = true;

            int dif = 0;
            int total = 0;
            object gridIn = null;
            object gridOut = null;

            object ultIn = null;
            object ultOut = null;

            try
            {
                for (int i = 1; i <= 20; i++)
                {
                    if (tip == 2 && i == 20) continue;

                    gridIn = ent["In" + i];
                    if (tip == 2) gridIn = ent["In" + (i + 1).ToString()];

                    gridOut = ent["Out" + i];

                    //Florin 2020.06.22
                    if (gridIn != DBNull.Value && Convert.ToDateTime(gridIn).AddMinutes(-1 * minuteIN) <= firstInPaid)
                        gridIn = firstInPaid;

                    if (gridOut != DBNull.Value && Convert.ToDateTime(gridOut).AddMinutes(minuteOUT) >= lastOutPaid)
                        gridOut = lastOutPaid;

                    if (gridIn != DBNull.Value && gridOut != DBNull.Value && Convert.ToDateTime(gridIn) < Convert.ToDateTime(gridOut))
                    {
                        try
                        {
                            if (tip == 1)
                            {
                                dif = CalculTimp(Convert.ToDateTime(OraInceput), Convert.ToDateTime(OraSfarsit), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(gridIn), Convert.ToDateTime(gridOut), rap);

                                if (dif > 0)
                                {
                                    total += dif;
                                    ultima = dif;
                                    ultIn = gridIn;
                                    ultOut = gridOut;
                                }
                            }
                            else
                            {
                                dif = CalculTimp(Convert.ToDateTime(OraInceput), Convert.ToDateTime(OraSfarsit), Convert.ToDateTime(ent["Ziua"]), Convert.ToDateTime(gridOut), Convert.ToDateTime(gridIn), rap);
                                if (dif > 0 && dif <= pauzaMin) total += dif;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (total > 0) rez = total;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculIntervale");
            }

            return rez;
        }

        private static int CalculTimp(DateTime OraInceput, DateTime OraSfarsit, DateTime ziua, DateTime firstInPaid, DateTime lastOutPaid, int rap)
        {
            int rez = 0;
            int rez2 = 0;

            try
            {
                //ora de inceput
                DateTime oraInc = Convert.ToDateTime(OraInceput);
                DateTime oraSf = Convert.ToDateTime(OraSfarsit);
                DateTime oraInc2 = Convert.ToDateTime(OraInceput);
                DateTime oraSf2 = Convert.ToDateTime(OraSfarsit);

                oraInc = new DateTime(ziua.Year, ziua.Month, ziua.Day, oraInc.Hour, oraInc.Minute, oraInc.Second);
                if (rap == 2) oraInc = oraInc.AddDays(-1);
                oraSf = new DateTime(ziua.Year, ziua.Month, ziua.Day, oraSf.Hour, oraSf.Minute, oraSf.Second);

                if (oraInc > oraSf)
                {//Radu 19.02.2019
                    oraSf = new DateTime(ziua.AddDays(1).Year, ziua.AddDays(1).Month, ziua.AddDays(1).Day, oraSf.Hour, oraSf.Minute, oraSf.Second);
                    //if (oraInc.Date == oraSf.Date)
                    {
                        oraInc2 = oraInc.AddDays(-1);
                        oraSf2 = oraSf.AddDays(-1);
                    }

                    DateTime inc2;
                    if ((oraInc2 - Convert.ToDateTime(firstInPaid)).TotalMinutes >= 0)
                        inc2 = oraInc2;
                    else
                        inc2 = Convert.ToDateTime(firstInPaid);


                    DateTime sf2;
                    if ((oraSf2 - Convert.ToDateTime(lastOutPaid)).TotalMinutes >= 0)
                        sf2 = Convert.ToDateTime(lastOutPaid);
                    else
                        sf2 = oraSf2;


                    rez2 = Convert.ToInt32((sf2 - inc2).TotalMinutes);
                }

                DateTime inc;
                if ((oraInc - Convert.ToDateTime(firstInPaid)).TotalMinutes >= 0)
                    inc = oraInc;
                else
                    inc = Convert.ToDateTime(firstInPaid);

                DateTime sf;
                if ((oraSf - Convert.ToDateTime(lastOutPaid)).TotalMinutes >= 0)
                    sf = Convert.ToDateTime(lastOutPaid);
                else
                    sf = oraSf;

                rez = Convert.ToInt32((sf - inc).TotalMinutes);
                if (rez < 0)
                    rez = 0;
                if (rez2 < 0)
                    rez2 = 0;
                rez += rez2;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculTimp");
            }

            return rez;
        }

        private static void CalculMinute(DataRow ent, DataRow entAlte, int timp)
        {
            try
            {
                //rotunjire
                switch (Convert.ToInt32(General.Nz(entAlte["Rotunjire"], -99)))
                {
                    case 1:                             //rotunjire la minute
                        //NOP
                        break;
                    case 2:                             //rotunjire la ora
                        if ((timp % 60) >= 30)
                            timp = ((timp / 60) + 1) * 60;
                        else
                            timp = (timp / 60) * 60;
                        break;
                    case 3:                             //trunchiere la ora
                        timp = (timp / 60) * 60;
                        break;
                    case 4:                             //rotunjire la 45 minute
                        if ((timp % 60) >= 45)
                            timp = ((timp / 60) + 1) * 60;
                        else
                            timp = (timp / 60) * 60;
                        break;
                    case 5:                             //rotunjire la 10 minute
                        timp = TrunchiereLa10Minute(timp);
                        break;
                    case 6:                             //rotunjire la 5 minute
                        timp = TrunchiereLa5Minute(timp);
                        break;
                }

                //se afla intre min si max
                if (timp < TransformaInMinute(Convert.ToDateTime(entAlte["ValMin"]))) timp = 0;
                if (TransformaInMinute(Convert.ToDateTime(entAlte["ValMax"])) < timp) timp = TransformaInMinute(Convert.ToDateTime(entAlte["ValMax"]));

                //valoare fixa
                if (entAlte["ValFixa"] != DBNull.Value && TransformaInMinute(Convert.ToDateTime(entAlte["ValFixa"])) > 0 && timp > 0) timp = TransformaInMinute(Convert.ToDateTime(entAlte["ValFixa"]));

                //multiplicator
                if (Convert.ToInt32(General.Nz(entAlte["Multiplicator"], -99)) > 0) timp = Convert.ToInt32(timp * Convert.ToInt32(General.Nz(entAlte["Multiplicator"], -99)));

                string ert = entAlte["Camp"].ToString();
                if ((entAlte["Camp"].ToString()) != "" && timp > 0) ent[entAlte["Camp"].ToString()] = timp;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculMinute");
            }
        }

        private static void CalculTimpLucrat(DataRow ent)
        {
            object gridIn = DBNull.Value;
            object gridOut = DBNull.Value;
            int timpLucrat = 0;
            int timpPauza = 0;

            try
            {
                for (int i = 1; i <= 20; i++)
                {
                    gridIn = ent["In" + i];
                    gridOut = ent["Out" + i];

                    if (gridIn != DBNull.Value && gridOut != DBNull.Value)
                    {
                        try
                        {
                            TimeSpan ts = Convert.ToDateTime(gridOut) - Convert.ToDateTime(gridIn);
                            if (ts.TotalMinutes > 0) timpLucrat += Convert.ToInt32(ts.TotalMinutes);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (ent["FirstIn"].ToString() != "" && ent["LastOut"].ToString() != "")
                {
                    try
                    {
                        TimeSpan ts = Convert.ToDateTime(ent["LastOut"]) - Convert.ToDateTime(ent["FirstIn"]);
                        //Radu 14.02.2019
                        int idProg = Convert.ToInt32(General.Nz(ent["IdProgram"], -99));
                        DataTable dt = General.IncarcaDT(string.Format(@"SELECT * FROM ""Ptj_Programe"" WHERE ""Id""={0}", idProg));
                        DataRow entPrg = null;
                        if (dt.Rows.Count > 0)
                        {
                            entPrg = dt.Rows[0];
                            if (entPrg["PauzaTimp"] != null)
                            {
                                DateTime pauza = Convert.ToDateTime(entPrg["PauzaTimp"]);
                                if (pauza.Hour > 0 || pauza.Minute > 0)
                                    timpLucrat -= (60 * pauza.Hour + pauza.Minute);
                                if (timpLucrat < 0)
                                    timpLucrat = 0;
                            }
                        }

                        timpPauza = Convert.ToInt32(ts.TotalMinutes - timpLucrat);
                    }
                    catch (Exception)
                    {
                    }
                }

                ent["TimpLucratReal"] = timpLucrat;
                ent["TimpPauzaReal"] = timpPauza;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "CalculTimpLucrat");
            }
        }

        private static DateTime? TransformaInData(DateTime? zi, string dataMinute)
        {
            DateTime? dt = null;
            try
            {
                int h = 0;
                int m = 0;

                if (dataMinute != null && dataMinute != "")
                {
                    h = Convert.ToDateTime(dataMinute).Hour;
                    m = Convert.ToDateTime(dataMinute).Minute;
                }

                if (zi != null)
                    dt = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, h, m, 0);
                else
                    dt = new DateTime(1900, 1, 1, h, m, 0);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "TransformaInData");
            }

            return dt;
        }

        private static DateTime? TransformaInData(DateTime? zi, int min)
        {
            DateTime? dt = null;
            try
            {
                int h = min / 60;
                int m = min % 60;
                if (zi != null)
                    dt = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, h, m, 0);
                else
                    dt = new DateTime(1900, 1, 1, h, m, 0);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "TransformaInData");
            }

            return dt;
        }

        private static DateTime? TransformaInData(int min, int an, int luna, int zi)
        {
            DateTime? dt = null;
            try
            {
                dt = new DateTime(an, luna, zi, 0, 0, 0);
                dt = dt.Value.AddMinutes(min);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "TransformaInData");
            }

            return dt;
        }

        private static DateTime? TransformaInData(DateTime? zi, int ora, int minute, int secunde = 0)
        {
            DateTime? dt = zi;
            try
            {
                if (zi != null) dt = new DateTime(zi.Value.Year, zi.Value.Month, zi.Value.Day, ora, minute, secunde);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "TransformaInData");
            }

            return dt;

        }

        private static void GolimCampurile(DataRow ent)
        {
            try
            {
                //Florin 2019.07.29
                //se golesc doar primele 30 F-uri
                for (int i = 0; i <= 30; i++)
                {
                    if (i != 0) ent["F" + i.ToString()] = DBNull.Value;
                }
                string golesteVal = Dami.ValoareParam("ProceseCeasuri_GolesteVal", "");
                if (golesteVal != "")
                {
                    string[] param = golesteVal.Split(',');
                    for (int i = 0; i < param.Length; i++)
                        ent["Val" + param[i]] = DBNull.Value;
                }

                ent["ValStr"] = DBNull.Value;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul Ceasuri", "GolimCampurile");
            }
        }

        private static int TrunchiereLa10Minute(int minute)
        {
            int ras = 0;

            try
            {
                int ora = minute / 60;
                int min = minute % 60;

                int rest = min % 10;
                int nr = min / 10;

                if ((min % 10) >= 5)
                    min = ((min / 10) + 1) * 10;
                else
                    min = min / 10 * 10;

                if (min == 60)
                {
                    ora += 1;
                    min = 0;
                }
                ras = (ora * 60) + min;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul", "TrunchiereLa10Minute");
            }
            return ras;
        }

        private static int TrunchiereLa5Minute(int minute)
        {
            int ras = 0;
            try
            {
                int ora = minute / 60;
                int min = minute % 60;

                int rest = min % 10;
                int nr = min / 10;

                if ((min % 10) >= 5)
                    min = ((min / 10) * 10) + 5;
                else
                    min = min / 10 * 10;

                if (min == 60)
                {
                    ora += 1;
                    min = 0;
                }

                ras = (ora * 60) + min;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul", "TrunchiereLa5Minute");
            }

            return ras;
        }

        private static int CalculIntervale(DataRow ent, DateTime intInc, DateTime intSf)
        {
            int rez = 0;

            try
            {
                if (intInc == null || intSf == null) return rez;

                int total = 0;

                for (int i = 1; i <= 20; i++)
                {
                    if (ent["In" + i] != DBNull.Value && ent["Out" + i] != DBNull.Value)
                    {
                        try
                        {
                            //intersectam fiecare interval In-Out cu intervalul trimis ca parametru
                            if (Convert.ToDateTime(ent["In" + i]) <= Convert.ToDateTime(intSf) && Convert.ToDateTime(intInc) <= Convert.ToDateTime(ent["Out" + i]))
                            {
                                //luam max de date intrare
                                DateTime gridIn = Convert.ToDateTime(ent["In" + i]) > Convert.ToDateTime(intInc) ? Convert.ToDateTime(ent["In" + i]) : Convert.ToDateTime(intInc);
                                //luam min de date iesire
                                DateTime gridOut = Convert.ToDateTime(ent["Out" + i]) < Convert.ToDateTime(intSf) ? Convert.ToDateTime(ent["Out" + i]) : Convert.ToDateTime(intSf);
                                //facem diferenta
                                int difTmp = Convert.ToInt32((Convert.ToDateTime(gridOut) - Convert.ToDateTime(gridIn)).TotalMinutes);
                                if (difTmp > 0) total += difTmp;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                if (total > 0) rez = total;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Calcul", "CalculIntervale");
            }

            return rez;
        }

        private static string VerificareCereriH5(int f10003, DateTime ziua, int ziSapt, int ziLibera, int ziLiberaLegala)
        {
            string cmp = "";
            string strSql =
                $@"SELECT ""DenumireScurta"", ""IdTipOre"", ""OreInVal"", SUM(""NrOre"") AS ""NrOre""
                FROM ""Ptj_Cereri"" A
                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                INNER JOIN ""F100Contracte"" C ON C.F10003={f10003} AND CAST(C.""DataInceput"" AS DATE) <= {General.ToDataUniv(ziua)} AND {General.ToDataUniv(ziua)} <= CAST(C.""DataSfarsit"" AS DATE)
                INNER JOIN ""Ptj_ContracteAbsente"" D ON A.""IdAbsenta"" = D.""IdAbsenta"" AND C.""IdContract"" = D.""IdContract""
                WHERE A.F10003 = {f10003} AND A.""IdStare"" = 3 
                AND CAST(A.""DataInceput"" AS DATE) <= {General.ToDataUniv(ziua)} AND {General.ToDataUniv(ziua)} <= CAST(A.""DataSfarsit"" AS DATE) AND (""NuTrimiteInPontaj"" IS NULL OR ""NuTrimiteInPontaj"" = 0)
                AND 
                (((CASE WHEN({ziSapt} < 6 AND {ziLibera} = 0) THEN 1 ELSE 0 END) = COALESCE(D.ZL,0) AND COALESCE(D.ZL,0) <> 0) OR
                ((CASE WHEN {ziSapt} = 6 THEN 1 ELSE 0 END) = COALESCE(D.S,0) AND COALESCE(D.S,0) <> 0) OR
                ((CASE WHEN {ziSapt} = 7 THEN 1 ELSE 0 END) = COALESCE(D.D,0) AND COALESCE(D.D,0) <> 0) OR
                (COALESCE({ziLiberaLegala},0) = COALESCE(D.SL,0) AND COALESCE(D.SL,0) <> 0)) 
                GROUP BY ""DenumireScurta"", ""IdTipOre"", ""OreInVal"" ";

            DataTable entCer = General.IncarcaDT(strSql);

            if (entCer != null && entCer.Rows.Count > 0)
            {
                for (int i = 0; i < entCer.Rows.Count; i++)
                {
                    if (General.Nz(entCer.Rows[i]["IdTipOre"], "").ToString() == "1")
                    {
                        if (General.Nz(entCer.Rows[i]["DenumireScurta"], "").ToString().Trim() != "")
                            cmp += $@",""ValStr""='{entCer.Rows[i]["DenumireScurta"].ToString().Trim()}',""Val0""=null,""Val1""=null,""Val2""=null,""Val3""=null,""Val4""=null,""Val5""=null,""Val6""=null,""Val7""=null,""Val8""=null,""Val9""=null,""Val10""=null,""Val11""=null,""Val12""=null,""Val13""=null,""Val14""=null,""Val15""=null,""Val16""=null,""Val17""=null,""Val18""=null,""Val19""=null,""Val20""=null ";
                    }
                    else
                    {
                        if (General.Nz(entCer.Rows[i]["OreInVal"], "").ToString().Trim() != "")
                            cmp += ",\"" + entCer.Rows[i]["OreInVal"].ToString() + "\"=" + Convert.ToInt32(Math.Round(Convert.ToDecimal(General.Nz(entCer.Rows[i]["NrOre"], 0)) * 60));
                    }
                }
            }

            return cmp;
        }

        private static string CreeazaSintaxaUpdate(DataRow dr)
        {
            string sqlUpd = "";
            string sintaxa = "";

            try
            {
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    if (dr.Table.Columns[i].ColumnName.ToLower() == "idauto") continue;
                    if (dr[i] == DBNull.Value)
                    {
                        sintaxa += ", \"" + dr.Table.Columns[i].ColumnName + "\" = NULL";
                        continue;
                    }

                    switch (dr.Table.Columns[i].DataType.ToString())
                    {
                        case "System.String":
                            sintaxa += ", \"" + dr.Table.Columns[i].ColumnName + "\" = '" + General.Nz(dr[i], "") + "'";
                            break;
                        case "System.DateTime":
                            DateTime dt = Convert.ToDateTime(General.Nz(dr[i], new DateTime(1900, 1, 1)));
                            sintaxa += ", \"" + dr.Table.Columns[i].ColumnName + "\" = " + General.ToDataUniv(dt, true);
                            break;
                        case "System.Double":
                        case "System.Decimal":
                            sintaxa += ", \"" + dr.Table.Columns[i].ColumnName + "\" = " + General.Nz(dr[i], "null").ToString().Replace(",", ".");
                            break;
                        default:
                            sintaxa += ", \"" + dr.Table.Columns[i].ColumnName + "\" = " + General.Nz(dr[i], "null");
                            break;
                    }
                }

                if (sintaxa != "")
                {
                    sqlUpd = $@"UPDATE ""Ptj_Intrari"" SET {sintaxa.Substring(1)} WHERE F10003={dr["F10003"]} AND ""Ziua""={General.ToDataUniv(Convert.ToDateTime(dr["Ziua"]))}";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlUpd;
        }


    }
}
