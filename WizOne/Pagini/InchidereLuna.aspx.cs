using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class InchidereLuna : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");
                btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                lblInchidere.InnerText = Dami.TraduCuvant("Inchidere luna");
                #endregion
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                switch(e.Parameter)
                {
                    case "btnSave":
                        Salvare();
                        break;
                    case "btnClose":
                        Inchidere();
                        break;
                }
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Inchidere()
        {
            btnClose.Enabled = false;
            btnSave.Enabled = false;        
            string mesaj = InchideLuna();

            btnClose.Enabled = true;
            btnSave.Enabled = true;
            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj);

            General.InitSessionVariables();
        }

        public string InchideLuna()
        {
            string mesaj = "", sql = "";
            string AdunaComp = "", szSOMA = "", szSOMB = "";

            try
            {
                //Florin 2020.02.20 - nu lasam sa inchida luna daca nu are salvare de baza
                string raspuns = SalvareLuna();
                if (raspuns != "Salvarea s-a realizat cu succes!")
                    return raspuns;
                
                //Radu 19.02.2020 - verificare campuri F910 vs F100
                VerificareCampuri();

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'AdunaComp'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    AdunaComp = dtParam.Rows[0][0].ToString();

                sql = "SELECT F80003 FROM F800 WHERE F80002 = 'SOMA'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMA = dtParam.Rows[0][0].ToString();

                sql = "SELECT F80003 FROM F800 WHERE F80002 = 'SOMB'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMB = dtParam.Rows[0][0].ToString();

                if (szSOMA.Length <= 0 || szSOMB.Length <= 0)
                {
                    mesaj = "Nu ati precizat etichetele SOMA si SOMB in F800!";
                    return mesaj;
                }

                string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                string DB = Constante.BD;    
                tmp = Constante.cnnWeb.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                bool rez = MonthClosing.MthClsgFunc.MthCls(Constante.tipBD, AdunaComp, szSOMA, szSOMB, conn, DB, user, pwd, out mesaj);
                if (rez)
                    mesaj = "Proces realizat cu succes";

                if (rez)
                {
                    DataTable dt = General.IncarcaDT(
                        @"SELECT A.""Id"", A.""DataModif"", A.""DataInceputCIM"", A.""DataSfarsitCIM"", A.""DurataContract"", B.F10003, B.F10017, B.F100929, B.F10011 
                        FROM ""Avs_Cereri"" A 
                        INNER JOIN F100 B ON A.F10003=B.F10003
                        WHERE COALESCE(A.""IdAtribut"",-99) IN (25,26) AND COALESCE(A.""Actualizat"",0)=0", null);
                    for(int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow dr = dt.Rows[i];
                        DateTime dtLucru = General.DamiDataLucru();

                        if (Convert.ToDateTime(dr["DataModif"]).Year == dtLucru.Year && Convert.ToDateTime(dr["DataModif"]).Month == dtLucru.Month && Convert.ToInt32(General.Nz(dr["F10003"],-99)) != -99)
                        {
                            DateTime dtInc = Convert.ToDateTime(dr["DataInceputCIM"]);
                            DateTime dtSf = Convert.ToDateTime(dr["DataSfarsitCIM"]);

                            int nrLuni = 0, nrZile = 0;
                            string tmpSql = "";
                            DateTime dtTmp = new DateTime();
                            DateTime dtf = new DateTime(2100, 1, 1, 0, 0, 0);
                            if (dtSf != dtf)
                                dtTmp = dtSf.AddDays(-1);
                            else
                                dtTmp = dtSf;

                            Personal.Contract ctr = new Personal.Contract();
                            ctr.CalculLuniSiZile(Convert.ToDateTime(dtInc.Date), Convert.ToDateTime(dtSf.Date), out nrLuni, out nrZile);

                            tmpSql += "UPDATE F100 SET F100933 = " + General.ToDataUniv(Convert.ToDateTime(dr["DataInceputCIM"])) + ", F100934 = " + General.ToDataUniv(Convert.ToDateTime(dr["DataSfarsitCIM"])) + ", F100936 = " + nrZile.ToString() + ", F100935 = "
                                + nrLuni.ToString() + ", F100938 = 1, F100993 = " + General.ToDataUniv(Convert.ToDateTime(dr["DataSfarsitCIM"]))
                                + ", F10023 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 103)"
                                : "TO_DATE('" + dtTmp.Day.ToString().PadLeft(2, '0') + "/" + dtTmp.Month.ToString().PadLeft(2, '0') + "/" + dtTmp.Year.ToString() + "', 'dd/mm/yyyy')") + ", F1009741 = " + General.Nz(dr["DurataContract"],1) + "  WHERE F10003 = " + dr["F10003"] + ";";

                            tmpSql += "INSERT INTO F095 (F09501, F09502, F09503, F09504, F09505, F09506, F09507, F09508, F09509, F09510, F09511, USER_NO, TIME) "
                                + " VALUES (95, '" + dr["F10017"] + "', " + dr["F10003"] + ", '" + dr["F10011"] + "', " + General.ToDataUniv(Convert.ToDateTime(dr["DataInceputCIM"])) + ", " + General.ToDataUniv(Convert.ToDateTime(dr["DataSfarsitCIM"]))
                                + ", " + nrLuni.ToString() + ", " + nrZile.ToString() + ", " + dr["F100929"] + ", 1, " + (General.Nz(dr["DurataContract"], 1).ToString() == "1" ? "'Nedeterminat'" : "'Determinat'") + ", -9, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ");";

                            tmpSql += $@"UPDATE ""Avs_Cereri"" SET ""Actualizat""=1 WHERE ""Id""={dr["Id"]};";

                            General.IncarcaDT("BEGIN " + tmpSql + "END;", null);
                        }
                    }
                }

                if (rez)
                {
                    DataTable dt = General.IncarcaDT($@" SELECT F70403, F70404, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id""=3) AS ""Culoare"", F70406, F704660,F704661,F704662,F704663,F704664,F704665,F704666,F704667,F704668,F704669
                        FROM F704 A
                        LEFT JOIN ""Avs_Cereri"" B ON A.F70403=B.F10003 AND A.F70404=B.""IdAtribut"" AND A.F70406=B.""DataModif""
                        WHERE A.F70404=11 AND F70410='Automat - grila' AND B.""IdAtribut"" IS NULL", null);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int id = Dami.NextId("Avs_Cereri");
                        DataRow dr = dt.Rows[i];

                        string strSql =
                            $@"BEGIN
                            INSERT INTO ""Avs_CereriIstoric""(""Id"", ""IdCircuit"", ""IdUser"", ""IdStare"", ""Pozitie"", ""Culoare"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""IdSuper"")
                            VALUES( {id}, 1, -89, 3, 1, '{dr["Culoare"] ?? null}', 1, {General.CurrentDate()}, -89, {General.CurrentDate()}, -89);

                            INSERT INTO ""Avs_Cereri""(""Id"", F10003, ""IdAtribut"", ""IdCircuit"", ""Pozitie"", ""TotalCircuit"", ""Culoare"", ""IdStare"", ""Explicatii"", ""DataModif"", ""Actualizat"", USER_NO, TIME, 
                            ""SporTran10"",""SporTran11"",""SporTran12"",""SporTran13"",""SporTran14"",""SporTran15"",""SporTran16"",""SporTran17"",""SporTran18"",""SporTran19"")
                            VALUES ({id}, {dr["F70403"]}, {dr["F70404"]}, 1, 1, 1, '{dr["Culoare"] ?? null}', 3, 'Adaugata automat', {General.ToDataUniv(Convert.ToDateTime(dr["F70406"]))}, 1, -89, {General.CurrentDate()},
                            {dr["F704660"] ?? null},{dr["F704661"] ?? null},{dr["F704662"] ?? null},{dr["F704663"] ?? null},{dr["F704664"] ?? null},{dr["F704665"] ?? null},{dr["F704666"] ?? null},{dr["F704667"] ?? null},{dr["F704668"]},{dr["F704669"] ?? null});
                            END;";
                        General.ExecutaNonQuery(strSql, null);
                    }                  
                }
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        private void Salvare()
        {
            btnClose.Enabled = false;
            btnSave.Enabled = false;
            string mesaj = SalvareLuna();

            btnClose.Enabled = true;
            btnSave.Enabled = true;
            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj);
        }


        public string SalvareLuna()
        {
            string mesaj = "Salvarea s-a realizat cu succes!";

            try
            {
                string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                string DB = Constante.BD;
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "PASSWORD=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                string cale = Dami.ValoareParam("CaleBackUp").Trim();
                if (cale == "")
                {
                    //Florin 2020.02.03 - necesar deoarece apare mesajul prea repede si nu mai are timp sa fie afisat dupa mesajul de interogare cu da sau nu
                    System.Threading.Thread.Sleep(1000);
                    return "Nu este setata calea pentru salvarea bazei de date in parametri (CaleBackUp)";
                }

                string anLucru = Dami.ValoareParam("AnLucru");
                string lunaLucru = Dami.ValoareParam("LunaLucru");
                string numeFisier = anLucru.ToString() + "_" + lunaLucru.ToString().PadLeft(2, '0') + "_" + Dami.TimeStamp();

                if (Constante.tipBD == 1)   //SQL
                {
                    bool raspuns = General.ExecutaNonQuery(
                        $@"Declare @vFileExists int
                        EXEC master.dbo.xp_fileexist '{cale}', @vFileExists OUTPUT
                        IF (@vFileExists = 0)
	                    EXEC Master.sys.xp_create_subdir '{cale}'", null);
                    if (raspuns)
                    {
                        string strSql =
                            $@"ROLLBACK; ALTER DATABASE {DB} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            BACKUP DATABASE {DB} TO  DISK = '{cale}\\{numeFisier}.bak'
                            WITH NOFORMAT, INIT, NAME = '{DB}FullBackup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;
                            ALTER DATABASE {DB} SET MULTI_USER WITH ROLLBACK IMMEDIATE; ";
                        General.ExecutaNonQuery(strSql, null);
                    }
                    else
                        mesaj = "Calea pentru salvarea bazei de date nu este valida." + Environment.NewLine + "Trebuie sa fie relativa la serverul de baza de date.";
                }
                else
                {
                    string caleExp = Dami.ValoareParam("CaleExpOracle");

                    if (caleExp == "")
                    {
                        //Florin 2020.02.03 - necesar deoarece apare mesajul prea repede si nu mai are timp sa fie afisat dupa mesajul de interogare cu da sau nu
                        System.Threading.Thread.Sleep(1000);
                        return "Nu este setata calea executabilului exp.exe in parametri (CaleExpOracle)";
                    }

                    General.ExecutaNonQuery("DROP DIRECTORY DirectorBackupWO", null);
                    General.ExecutaNonQuery($"CREATE DIRECTORY DirectorBackupWO AS '{cale}'", null);
                    string caleValida = General.Nz(General.ExecutaScalar($@"SELECT DBMS_LOB.FILEEXISTS(BFILENAME('DIRECTORBACKUPWO','.')) FROM DUAL", null),"0").ToString();

                    if (caleValida == "0")
                    {
                        System.Threading.Thread.Sleep(1000);
                        mesaj = "Calea pentru salvarea bazei de date nu este valida." + Environment.NewLine + "Trebuie sa fie relativa la serverul de baza de date.";
                    }
                    else
                    {
                        try
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = caleExp + "\\exp.exe";
                            string arg = "{0}/{1}@{2} file={3}.dmp log={3}.log owner={4} grants=Y rows=Y compress=Y";
                            arg = string.Format(arg, user, pwd, conn, cale + "\\" + numeFisier, user);
                            process.StartInfo.Arguments = arg;
                            //process.StartInfo.ErrorDialog = true;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.UseShellExecute = false;
                            process.Start();
                            process.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "The system cannot find the file specified")
                                mesaj = "Calea executabilului 'exp.exe' nu este corecta";
                            else
                            {
                                mesaj = ex.ToString();
                                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mesaj = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        public void VerificareCampuri()
        {
            string sql = "", sqlMod = "";
            if (Constante.tipBD == 1) //SQL
            {
                sql = "select 'F910' + a.camp AS coloana, a.character_maximum_length as lungime_F910, b.character_maximum_length as lungime_F100, "
                    + " a.numeric_precision as lung_numeric_F910, b.numeric_precision as lung_numeric_F100 "
                    + " from "
                    + " (select substring(column_name, 5, 5) as camp, character_maximum_length, numeric_precision from INFORMATION_SCHEMA.COLUMNS where table_name = 'F910' AND column_name <> 'USER_NO' AND  column_name <> 'F91003') a, "
                    + " (select substring(column_name, 5, 5) as camp, character_maximum_length, numeric_precision from INFORMATION_SCHEMA.COLUMNS where table_name = 'F100' AND UPPER(DATA_TYPE) = 'INT') b "
                    + " where a.camp = b.camp and len(a.camp) >= 2 "
                    + " and(a.character_maximum_length < b.character_maximum_length or a.numeric_precision < b.numeric_precision)";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1] != null && dt.Rows[i][2] != null && dt.Rows[i][1].ToString().Length > 0 && dt.Rows[i][2].ToString().Length > 0)
                        {//sir de caractere
                            sqlMod = "ALTER TABLE F910 ALTER COLUMN " + dt.Rows[i][0] + " NVARCHAR(" + dt.Rows[i][2].ToString() + ")";
                            General.ExecutaNonQuery(sqlMod, null);
                        }
                        if (dt.Rows[i][3] != null && dt.Rows[i][4] != null && dt.Rows[i][3].ToString().Length > 0 && dt.Rows[i][4].ToString().Length > 0)
                        {//numeric
                            sqlMod = "ALTER TABLE F910 ALTER COLUMN " + dt.Rows[i][0] + (Convert.ToInt32(dt.Rows[i][4].ToString()) == 10 ? " INT" : "NUMERIC(" + dt.Rows[i][4].ToString() + ")");
                            General.ExecutaNonQuery(sqlMod, null);
                        }
                    }
                }
            }
            else   //Oracle
            {
                sql = "select 'F910' || a.camp AS coloana, a.data_length as lungime_F910, b.data_length as lungime_F100, "
                    + "a.data_precision as lung_numeric_F910, b.data_precision as lung_numeric_F100 "
                    + " from "
                    + "(select substr(column_name, 5, 5) as camp,  case when data_precision is not null then null else data_length end as data_length, data_precision from cols where table_name = 'F910' AND column_name<> 'USER_NO' AND column_name<> 'F91003') a, "
                    + " (select substr(column_name, 5, 5) as camp, case when data_precision is not null then null else data_length end as data_length, data_precision from cols where table_name = 'F100' AND UPPER(DATA_TYPE) = 'NUMBER' ) b "
                    + " where a.camp = b.camp and length(a.camp) >= 2 "
                    + " and(a.data_length < b.data_length or a.data_precision < b.data_precision)";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1] != null && dt.Rows[i][2] != null && dt.Rows[i][1].ToString().Length > 0 && dt.Rows[i][2].ToString().Length > 0)
                        {//sir de caractere
                            sqlMod = "ALTER TABLE F910 MODIFY " + dt.Rows[i][0] + " VARCHAR2(" + dt.Rows[i][2].ToString() + ")";
                            General.ExecutaNonQuery(sqlMod, null);
                        }
                        if (dt.Rows[i][3] != null && dt.Rows[i][4] != null && dt.Rows[i][3].ToString().Length > 0 && dt.Rows[i][4].ToString().Length > 0)
                        {//numeric
                            sqlMod = "ALTER TABLE F910 MODIFY " + dt.Rows[i][0] + "NUMBER(" + dt.Rows[i][4].ToString() + ")";
                            General.ExecutaNonQuery(sqlMod, null);
                        }
                    }
                }
            }
        }
    }
}