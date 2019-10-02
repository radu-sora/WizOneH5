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

                //btnClose.Text = Dami.TraduCuvant("btnClose", "Inchidere luna");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salvare luna");

                #endregion





            }
            catch (Exception ex)
            {
                ArataMesaj(ex.ToString());
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
                ArataMesaj(ex.ToString());
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
            ArataMesaj(Dami.TraduCuvant(mesaj));

            General.InitSessionVariables();
            
        }

        public string InchideLuna()
        {
            string mesaj = "", sql = "";
            string AdunaComp = "", szSOMA = "", szSOMB = "";

            try
            {
                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'AdunaComp'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    AdunaComp = dtParam.Rows[0][0].ToString();

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMA'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMA = dtParam.Rows[0][0].ToString();

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'SOMB'";
                dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    szSOMB = dtParam.Rows[0][0].ToString();

                if (szSOMA.Length <= 0 || szSOMB.Length <= 0)
                {
                    mesaj = "Nu ati precizat etichetele SOMA si SOMB in tblParametrii!";
                    return mesaj;
                }

                CriptDecript prc = new CriptDecript();
                Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);

                string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                //tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                //string DB = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                string DB = "";
                if (Constante.tipBD == 1)
                {
                    tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                    DB = tmp.Split(';')[0];
                }
                else
                    DB = user;        
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
            }
            catch (Exception ex)
            {
                ArataMesaj(ex.ToString());
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
            ArataMesaj(Dami.TraduCuvant(mesaj));

        }

        public string SalvareLuna()
        {
            string mesaj = "", sql = "", sql_tmp = "";

            try
            {
                CriptDecript prc = new CriptDecript();
                Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);
                                
                string tmp = Constante.cnnWeb.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                string DB = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                tmp = Constante.cnnWeb.ToUpper().Split(new[] { "PASSWORD=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                string cale = HostingEnvironment.MapPath("~/SALVARI");

                sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CaleBackUp'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    cale = dtParam.Rows[0][0].ToString();

                sql = "SELECT F01012, F01011 FROM F010";
                DataTable dt010 = General.IncarcaDT(sql, null);
                int anLucru = DateTime.Now.Year;
                int lunaLucru = DateTime.Now.Month;
                if (dt010 != null && dt010.Rows.Count > 0)
                {
                    lunaLucru = Convert.ToInt32(dt010.Rows[0][0].ToString());
                    anLucru = Convert.ToInt32(dt010.Rows[0][1].ToString());
                }


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
                    Process process = new Process();
                    string fisierExp = "{0}\\exp.exe";
                    string caleExp = "";

                    sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CaleExpOracle'";
                    dtParam = General.IncarcaDT(sql, null);
                    if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                        caleExp = dtParam.Rows[0][0].ToString();

                    if (caleExp.Length <= 0)
                    {
                        mesaj = "Nu ati precizat calea executabilului exp.exe in tblParametrii!";
                        return mesaj;
                    }
                    fisierExp = string.Format(fisierExp, caleExp);
                    process.StartInfo.FileName = fisierExp;
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
                ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        private void ArataMesaj(string mesaj)
        {
            pnlCtl.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            pnlCtl.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }



    }
}