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
            //ArataMesaj(Dami.TraduCuvant(mesaj));

            General.InitSessionVariables();
            
        }

        public string InchideLuna()
        {
            string mesaj = "", sql = "";
            string AdunaComp = "", szSOMA = "", szSOMB = "";

            try
            {
                //Florin 2020.02.20 - nu lasam sa inchida luna daca nu are salvare de baza
                string cale = Dami.ValoareParam("CaleBackUp").Trim();
                string anLucru = Dami.ValoareParam("AnLucru");
                string lunaLucru = Dami.ValoareParam("LunaLucru");
                string numeFisier = anLucru.ToString() + "_" + lunaLucru.ToString().PadLeft(2, '0');
                string ext = ".bak";
                if (Constante.tipBD == 2) ext = ".dmp";
                if (!File.Exists(cale + "\\" + numeFisier + ext))
                {
                    mesaj = "Inainte de inchidere de luna trebuie sa salvati luna";
                    return mesaj;
                }

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'AdunaComp'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    AdunaComp = dtParam.Rows[0][0].ToString();

                //sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMA'";
                sql = "SELECT F80003 FROM F800 WHERE F80002 = 'SOMA'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMA = dtParam.Rows[0][0].ToString();

                //sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMB'";
                sql = "SELECT F80003 FROM F800 WHERE F80002 = 'SOMB'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMB = dtParam.Rows[0][0].ToString();

                if (szSOMA.Length <= 0 || szSOMB.Length <= 0)
                {
                    mesaj = "Nu ati precizat etichetele SOMA si SOMB in F800!";
                    return mesaj;
                }

                //Florin 2020.02.03
                //CriptDecript prc = new CriptDecript();
                //Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);

                string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                //tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                //string DB = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                string DB = Constante.BD;
                //if (Constante.tipBD == 1)
                //{
                //    tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                //    DB = tmp.Split(';')[0];
                //}
                //else
                //    DB = user;        
                tmp = Constante.cnnWeb.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                bool rez = MonthClosing.MthClsgFunc.MthCls(Constante.tipBD, AdunaComp, szSOMA, szSOMB, conn, DB, user, pwd, out mesaj);
                if (rez)
                    mesaj = "Proces realizat cu succes";
                //else
                //    mesaj = "Eroare la inchidere de luna";


                //Florin 2019.04.09
                //se trimite in F100 modificarea in avans pentru CIM
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


                //Florin 2019.11.19
                //adaugam automat linie in modificari in avans pentru spor vechime
                if (rez)
                {
                    //string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                    //if (Constante.tipBD == 2)
                    //    cmp = "ROWNUM";

                    //string strSql = 
                    //    $@"BEGIN
                    //    INSERT INTO ""Avs_CereriIstoric""(""Id"", ""IdCircuit"", ""IdUser"", ""IdStare"", ""Pozitie"", ""Culoare"", ""Aprobat"", ""DataAprobare"", USER_NO, TIME, ""IdSuper"")
                    //    SELECT (COALESCE((SELECT MAX(COALESCE(""Id"",0)) FROM ""Avs_Cereri""),0)) + {cmp}, 1, -89, 3, 1, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id""=3), 1, {General.CurrentDate()}, -89, {General.CurrentDate()}, -89
                    //    FROM F704 A
                    //    LEFT JOIN ""Avs_Cereri"" B ON A.F70403=B.F10003 AND A.F70404=B.""IdAtribut"" AND A.F70406=B.""DataModif""
                    //    WHERE A.F70404=11 AND F70410='Automat - grila' AND B.""IdAtribut"" IS NULL;

                    //    INSERT INTO ""Avs_Cereri""(""Id"", F10003, ""IdAtribut"", ""IdCircuit"", ""Pozitie"", ""TotalCircuit"", ""Culoare"", ""IdStare"", ""Explicatii"", ""DataModif"", ""Actualizat"", USER_NO, TIME, 
                    //    ""SporTran10"",""SporTran11"",""SporTran12"",""SporTran13"",""SporTran14"",""SporTran15"",""SporTran16"",""SporTran17"",""SporTran18"",""SporTran19"")
                    //    SELECT (COALESCE((SELECT MAX(COALESCE(""Id"",0)) FROM ""Avs_Cereri""),0)) + {cmp}, F70403, F70404, 1, 1, 1, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id""=3), 3, 'Adaugata automat', F70406, 1, -89, {General.CurrentDate()},
                    //    F704660,F704661,F704662,F704663,F704664,F704665,F704666,F704667,F704668,F704669
                    //    FROM F704 A
                    //    LEFT JOIN ""Avs_Cereri"" B ON A.F70403=B.F10003 AND A.F70404=B.""IdAtribut"" AND A.F70406=B.""DataModif""
                    //    WHERE A.F70404=11 AND F70410='Automat - grila' AND B.""IdAtribut"" IS NULL;
                    //    END;";

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


        //Florin 2020.02.03

        public string SalvareLuna()
        {
            string mesaj = "", sql = "", sql_tmp = "";

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

                try
                {
                    if (!Directory.Exists(cale))
                        Directory.CreateDirectory(cale);
                }
                catch (Exception)
                {
                    //Florin 2020.02.03 - necesar deoarece apare mesajul prea repede si nu mai are timp sa fie afisat dupa mesajul de interogare cu da sau nu
                    System.Threading.Thread.Sleep(1000);
                    return "Calea pentru salvarea bazei de date nu este valida";
                }

                string anLucru = Dami.ValoareParam("AnLucru");
                string lunaLucru = Dami.ValoareParam("LunaLucru");

                string numeFisier = anLucru.ToString() + "_" + lunaLucru.ToString().PadLeft(2, '0');

                //cautam se vedem daca mai exista o salvare pe luna curenta
                var folder = new DirectoryInfo(cale);
                if (!folder.Exists)
                    folder.Create();
                FileInfo[] fileList = folder.GetFiles("*.bak");

                bool gasit = false;
                foreach (FileInfo fileDest in fileList)
                {
                    if (fileDest.Name == numeFisier + ".bak")
                    {
                        gasit = true;
                        break;
                    }
                }
                if (gasit)
                {
                    int i = 1;
                    do
                    {
                        if (!File.Exists(folder + @"\" + numeFisier + "_" + i.ToString() + ".bak"))
                        {
                            numeFisier = numeFisier + "_" + i.ToString();
                            break;
                        }
                        i++;
                    } while (true);
                }


                if (Constante.tipBD == 1)   //SQL
                {

                    sql_tmp = "ROLLBACK; ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; "
                        + " BACKUP DATABASE {1} TO  DISK = '{2}\\{3}.bak' "
                        + " WITH NOFORMAT, INIT, NAME = '{4}FullBackup', SKIP, NOREWIND, NOUNLOAD, STATS = 10; "
                        + " ALTER DATABASE {5} SET MULTI_USER WITH ROLLBACK IMMEDIATE; ";
                    sql = string.Format(sql_tmp, DB, DB, cale, numeFisier, DB, DB);

                    General.ExecutaNonQuery(sql, null);
                    mesaj = "Salvarea s-a realizat cu succes!";
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

                    if (!File.Exists(caleExp + "\\exp.exe"))
                    {
                        //Florin 2020.02.03 - necesar deoarece apare mesajul prea repede si nu mai are timp sa fie afisat dupa mesajul de interogare cu da sau nu
                        System.Threading.Thread.Sleep(1000);
                        return "Calea executabilului exp.exe nu este una valida (CaleExpOracle)";
                    }

                    Process process = new Process();
                    process.StartInfo.FileName = caleExp + "\\exp.exe";
                    string arg = "{0}/{1}@{2} file={3}.dmp log={4}.log owner={5} grants=Y rows=Y compress=Y";
                    arg = string.Format(arg, user, pwd, conn, cale + "\\" + numeFisier, numeFisier, user);
                    process.StartInfo.Arguments = arg;
                    process.StartInfo.ErrorDialog = true;
                    process.Start();
                    process.WaitForExit();
                    mesaj = "Salvarea s-a realizat cu succes!";
                }
            }
            catch (Exception ex)
            {
                mesaj = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }


        //public string SalvareLuna()
        //{
        //    string mesaj = "", sql = "", sql_tmp = "";

        //    try
        //    {
        //        //Florin 2020.02.03



        //        //CriptDecript prc = new CriptDecript();
        //        //Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);

        //        string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
        //        string conn = tmp.Split(';')[0];
        //        tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
        //        string DB = tmp.Split(';')[0];
        //        tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
        //        string user = tmp.Split(';')[0];
        //        tmp = Constante.cnnWeb.ToUpper().Split(new[] { "PASSWORD=" }, StringSplitOptions.None)[1];
        //        string pwd = tmp.Split(';')[0];

        //        //string cale = HostingEnvironment.MapPath("~/SALVARI");

        //        //sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CaleBackUp'";
        //        //DataTable dtParam = General.IncarcaDT(sql, null);
        //        //if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
        //        //    cale = dtParam.Rows[0][0].ToString();

        //        string cale = Dami.ValoareParam("CaleBackUp").Trim();
        //        if (cale == "")
        //            return "Nu este setata calea pentru salvarea bazei de date in parametri";

        //        if (!Directory.Exists(cale))
        //            Directory.CreateDirectory(cale);

        //        if (!Directory.Exists(cale))
        //            return "Calea pentru salvarea bazei de date nu este valida";

        //        //sql = "SELECT F01012, F01011 FROM F010";
        //        //DataTable dt010 = General.IncarcaDT(sql, null);
        //        //int anLucru = DateTime.Now.Year;
        //        //int lunaLucru = DateTime.Now.Month;
        //        //if (dt010 != null && dt010.Rows.Count > 0)
        //        //{
        //        //    lunaLucru = Convert.ToInt32(dt010.Rows[0][0].ToString());
        //        //    anLucru = Convert.ToInt32(dt010.Rows[0][1].ToString());
        //        //}

        //        string anLucru = Dami.ValoareParam("AnLucru");
        //        string lunaLucru = Dami.ValoareParam("LunaLucru");

        //        string numeFisier = anLucru.ToString() + "_" + lunaLucru.ToString().PadLeft(2, '0');

        //        //cautam se vedem daca mai exista o salvare pe luna curenta
        //        var folder = new DirectoryInfo(cale);
        //        if (!folder.Exists)
        //            folder.Create();
        //        FileInfo[] fileList = folder.GetFiles("*.bak");

        //        bool gasit = false;
        //        foreach (FileInfo fileDest in fileList)
        //        {
        //            if (fileDest.Name == numeFisier + ".bak")
        //            {
        //                gasit = true;
        //                break;
        //            }
        //        }
        //        if (gasit)
        //        {
        //            int i = 1;
        //            do
        //            {
        //                if (!File.Exists(folder + @"\" + numeFisier + "_" + i.ToString() + ".bak"))
        //                {
        //                    numeFisier = numeFisier + "_" + i.ToString();
        //                    break;
        //                }
        //                i++;
        //            } while (true);
        //        }


        //        if (Constante.tipBD == 1)   //SQL
        //        {

        //            sql_tmp = "ROLLBACK; ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; "
        //                + " BACKUP DATABASE {1} TO  DISK = '{2}\\{3}.bak' "
        //                + " WITH NOFORMAT, INIT, NAME = '{4}FullBackup', SKIP, NOREWIND, NOUNLOAD, STATS = 10; "
        //                + " ALTER DATABASE {5} SET MULTI_USER WITH ROLLBACK IMMEDIATE; ";
        //            sql = string.Format(sql_tmp, DB, DB, cale, numeFisier, DB, DB);

        //            General.ExecutaNonQuery(sql, null);
        //            mesaj = "Salvarea s-a realizat cu succes!";
        //        }
        //        else
        //        {
        //            Process process = new Process();
        //            //string fisierExp = "{0}\\exp.exe";
        //            //string caleExp = "";

        //            //sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CaleExpOracle'";
        //            //dtParam = General.IncarcaDT(sql, null);
        //            //if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
        //            //    caleExp = dtParam.Rows[0][0].ToString();

        //            //if (caleExp.Length <= 0)
        //            //{
        //            //    mesaj = "Nu ati precizat calea executabilului exp.exe in tblParametrii!";
        //            //    return mesaj;
        //            //}
        //            //fisierExp = string.Format(fisierExp, caleExp);


        //            string caleExp = Dami.ValoareParam("CaleExpOracle");

        //            if (cale == "")
        //                return "Nu ati precizat calea executabilului exp.exe in tblParametri!";

        //            //process.StartInfo.FileName = fisierExp;

        //            process.StartInfo.FileName = caleExp + "\\exp.exe";

        //            string arg = "{0}/{1}@{2} file={3}.dmp log={4}.log owner={5} grants=Y rows=Y compress=Y";
        //            arg = string.Format(arg, user, pwd, conn, cale + "\\" + numeFisier, numeFisier, user);
        //            process.StartInfo.Arguments = arg;
        //            process.StartInfo.ErrorDialog = true;
        //            process.Start();
        //            process.WaitForExit();
        //            mesaj = "Salvarea s-a realizat cu succes!";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return mesaj;
        //}





    }
}