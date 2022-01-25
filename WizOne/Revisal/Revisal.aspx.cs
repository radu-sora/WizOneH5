using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO.Packaging;
using System.Xml;
using WizOne.Module;
using DevExpress.XtraReports.UI;

namespace WizOne.Revisal
{
    public partial class Revisal : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                btnSalvare.Text = Dami.TraduCuvant("btnSalvare", "Salvare parametri");
                btnPreg.Text = Dami.TraduCuvant("btnPreg", "Pregatire date");
                btnGenerare.Text = Dami.TraduCuvant("btnGenerare", "Genereaza XML");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                lgConfigAng.InnerText = Dami.TraduCuvant("Configurare angajati inactivi");
                lgConfigPer.InnerText = Dami.TraduCuvant("Configurare perioada");
                lgConfigData.InnerText = Dami.TraduCuvant("Configurare Data sf. contract per. determ./Data incetarerbDataSf1");
                lgConfigSpor.InnerText = Dami.TraduCuvant("Configurare Sporuri");
                lgConfigModAv.InnerText = Dami.TraduCuvant("Configurare Modificari in avans");

                chkInactivi.Text = Dami.TraduCuvant("Inclusiv inactivi");
                lblInactivi.Text = Dami.TraduCuvant("Inactivi din data:");
                lblDeLa.Text = Dami.TraduCuvant("De la");
                lblLa.Text = Dami.TraduCuvant("La");
                chkCalc.Text = Dami.TraduCuvant("Calculul e final");
                lblDataSpor.Text = Dami.TraduCuvant("Data modif. sporului");
                rbDataSf1.Text = Dami.TraduCuvant("Data sfarsit contract = Data incetare");
                rbDataSf2.Text = Dami.TraduCuvant("Data sfarsit contract = Data incetare - 1 zi");
                rbSporuri2.Text = Dami.TraduCuvant("Sporuri cuvenite");
                rbSporuri1.Text = Dami.TraduCuvant("Sporuri calculate");
                lblSpor.Text = Dami.TraduCuvant("Angajati noi");
                rbSporuriAngNoi1.Text = Dami.TraduCuvant("Sporuri cuvenite");
                rbSporuriAngNoi2.Text = Dami.TraduCuvant("Spor nedeclarat 0");
                rbModif1.Text = Dami.TraduCuvant("Modificarile in intervalul [DeLa ; La]");
                rbModif2.Text = Dami.TraduCuvant("Modificarile in intervalul [DeLa ; DeLa + 19 zile]");

                #endregion


                Dictionary<String, String> lista = new Dictionary<string, string>();
                if (!IsPostBack)
                {
                    lista = LoadParameters();
                    chkInactivi.Checked = lista.ContainsKey("REVISAL_BIFAINACTIVI") && lista["REVISAL_BIFAINACTIVI"] == "1" ? true : false;
                    deDataInactivi.Value = lista.ContainsKey("REVISAL_DATAINACTIVI") ? Convert.ToDateTime(lista["REVISAL_DATAINACTIVI"]) : new DateTime(2100, 1, 1);
                    if (lista.ContainsKey("REVISAL_DATA_SF_INCET"))
                    {
                        if (lista["REVISAL_DATA_SF_INCET"] == "0")
                        {
                            rbDataSf1.Checked = true;
                            rbDataSf2.Checked = false;
                        }
                        if (lista["REVISAL_DATA_SF_INCET"] == "1")
                        {
                            rbDataSf1.Checked = false;
                            rbDataSf2.Checked = true;
                        }
                    }
                    if (lista.ContainsKey("REVISAL_SPORURI_CALCULATE_CUVENITE"))
                    {
                        if (lista["REVISAL_SPORURI_CALCULATE_CUVENITE"] == "0")
                        {
                            rbSporuri1.Checked = true;
                            rbSporuri2.Checked = false;
                        }
                        if (lista["REVISAL_SPORURI_CALCULATE_CUVENITE"] == "1")
                        {
                            rbSporuri1.Checked = false;
                            rbSporuri2.Checked = true;
                        }
                    }
                    if (lista.ContainsKey("REVISAL_INTERVAL_MODIFICARI"))
                    {
                        if (lista["REVISAL_INTERVAL_MODIFICARI"] == "0")
                        {
                            rbModif1.Checked = true;
                            rbModif2.Checked = false;
                        }
                        if (lista["REVISAL_INTERVAL_MODIFICARI"] == "1")
                        {
                            rbModif1.Checked = false;
                            rbModif2.Checked = true;
                        }
                    }
                    if (lista.ContainsKey("REVISAL_SPORURI_ANGAJATI_NOI"))
                    {
                        if (lista["REVISAL_SPORURI_ANGAJATI_NOI"] == "0")
                        {
                            rbSporuriAngNoi1.Checked = true;
                            rbSporuriAngNoi2.Checked = false;
                        }
                        if (lista["REVISAL_SPORURI_ANGAJATI_NOI"] == "1")
                        {
                            rbSporuriAngNoi1.Checked = false;
                            rbSporuriAngNoi2.Checked = true;
                        }
                    }
                    deDeLa.Value = DateTime.Now;
                    deLa.Value = DateTime.Now;
                    deDataSpor.Value = DateTime.Now;
                    Session["ListaParamRevisal"] = lista;
                }
                else
                    lista = Session["ListaParamRevisal"] as Dictionary<string, string>;


                rbModif1.ClientEnabled = false;
                rbModif2.ClientEnabled = false;


                if (rbSporuri2.Checked)
                {
                    rbSporuriAngNoi1.ClientEnabled = false;
                    rbSporuriAngNoi2.ClientEnabled = false;
                }
                else
                {
                    rbSporuriAngNoi1.ClientEnabled = true;
                    rbSporuriAngNoi2.ClientEnabled = true;
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public Dictionary<String, String> LoadParameters()
        {
            Dictionary<String, String> lista = new Dictionary<string, string>();

            string sql = "SELECT \"Nume\" , \"Valoare\"  FROM \"tblParametrii\" WHERE \"Nume\" IN ('REVISAL_SAL', 'REVISAL_BIFAINACTIVI', 'REVISAL_DATAINACTIVI', 'REVISAL_DATA_SF_INCET', 'REVISAL_SPORURI_CALCULATE_CUVENITE', 'REVISAL_INTERVAL_MODIFICARI', 'REVISAL_SPORURI_ANGAJATI_NOI')";
            DataTable dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    if (!lista.ContainsKey(dtParam.Rows[i]["Nume"].ToString()))
                        lista.Add(dtParam.Rows[i]["Nume"].ToString(), dtParam.Rows[i]["Valoare"].ToString());
            return lista;
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string[] param = e.Parameter.Split(';');
                Dictionary<String, String> lista = Session["ListaParamRevisal"] as Dictionary<String, String>;
                switch (param[0])
                {
                    case "chkInactivi":
                        if (!lista.ContainsKey("REVISAL_BIFAINACTIVI"))
                            lista.Add("REVISAL_BIFAINACTIVI", "");
                        lista["REVISAL_BIFAINACTIVI"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "deDataInactivi":
                        string[] data = param[1].Split('.');                       
                        if (!lista.ContainsKey("REVISAL_DATAINACTIVI"))
                            lista.Add("REVISAL_DATAINACTIVI", "");
                        lista["REVISAL_DATAINACTIVI"] = data[0] + "/" + data[1] + "/" + data[2];
                        break;
                    case "rbDataSf1":
                        if (!lista.ContainsKey("REVISAL_DATA_SF_INCET"))
                            lista.Add("REVISAL_DATA_SF_INCET", "");
                        lista["REVISAL_DATA_SF_INCET"] = param[1] == "true" ? "0" : "1";
                        break;
                    case "rbDataSf2":
                        if (!lista.ContainsKey("REVISAL_DATA_SF_INCET"))
                            lista.Add("REVISAL_DATA_SF_INCET", "");
                        lista["REVISAL_DATA_SF_INCET"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "rbSporuri1":
                        if (!lista.ContainsKey("REVISAL_SPORURI_CALCULATE_CUVENITE"))
                            lista.Add("REVISAL_SPORURI_CALCULATE_CUVENITE", "");
                        lista["REVISAL_SPORURI_CALCULATE_CUVENITE"] = param[1] == "true" ? "0" : "1";
                        if (rbSporuri2.Checked)
                        {
                            rbSporuriAngNoi1.ClientEnabled = false;
                            rbSporuriAngNoi2.ClientEnabled = false;
                        }
                        else
                        {
                            rbSporuriAngNoi1.ClientEnabled = true;
                            rbSporuriAngNoi2.ClientEnabled = true;
                        }
                        break;
                    case "rbSporuri2":
                        if (!lista.ContainsKey("REVISAL_SPORURI_CALCULATE_CUVENITE"))
                            lista.Add("REVISAL_SPORURI_CALCULATE_CUVENITE", "");
                        lista["REVISAL_SPORURI_CALCULATE_CUVENITE"] = param[1] == "true" ? "1" : "0";
                        if (rbSporuri2.Checked)
                        {
                            rbSporuriAngNoi1.ClientEnabled = false;
                            rbSporuriAngNoi2.ClientEnabled = false;
                        }
                        else
                        {
                            rbSporuriAngNoi1.ClientEnabled = true;
                            rbSporuriAngNoi2.ClientEnabled = true;
                        }
                        break;
                    case "rbModif1":
                        if (!lista.ContainsKey("REVISAL_INTERVAL_MODIFICARI"))
                            lista.Add("REVISAL_INTERVAL_MODIFICARI", "");
                        lista["REVISAL_INTERVAL_MODIFICARI"] = param[1] == "true" ? "0" : "1";
                        break;
                    case "rbModif2":
                        if (!lista.ContainsKey("REVISAL_INTERVAL_MODIFICARI"))
                            lista.Add("REVISAL_INTERVAL_MODIFICARI", "");
                        lista["REVISAL_INTERVAL_MODIFICARI"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "rbSporuriAngNoi1":
                        if (!lista.ContainsKey("REVISAL_SPORURI_ANGAJATI_NOI"))
                            lista.Add("REVISAL_SPORURI_ANGAJATI_NOI", "");
                        lista["REVISAL_SPORURI_ANGAJATI_NOI"] = param[1] == "true" ? "0" : "1";
                        break;
                    case "rbSporuriAngNoi2":
                        if (!lista.ContainsKey("REVISAL_SPORURI_ANGAJATI_NOI"))
                            lista.Add("REVISAL_SPORURI_ANGAJATI_NOI", "");
                        lista["REVISAL_SPORURI_ANGAJATI_NOI"] = param[1] == "true" ? "1" : "0";
                        break;      
                }    

                Session["ListaParamRevisal"] = lista;


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void btnPreg_Click(object sender, EventArgs e)
        {
            try
            {
                string cnApp = Constante.cnnWeb;
                string tmp = cnApp.Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = cnApp.Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                //tmp = cnApp.Split(new[] { "Initial catalog=" }, StringSplitOptions.None)[1];
                //string DB = tmp.Split(';')[0];

                string DB = "";
                if (Constante.tipBD == 1)
                {
                    tmp = cnApp.Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                    DB = tmp.Split(';')[0];
                }
                else
                    DB = user;
       
                tmp = cnApp.Split(new[] { "PASSWORD=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                string sql = "SELECT \"Valoare\", \"Criptat\"  FROM \"tblParametrii\" WHERE \"Nume\" = 'ORAPWD'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0)
                {
                    if (dtParam.Rows[0][1] == null || dtParam.Rows[0][1].ToString().Length <= 0 || dtParam.Rows[0][1].ToString() == "0")
                        pwd = dtParam.Rows[0][0].ToString();
                    else
                    {
                        CriptDecript prc = new CriptDecript();
                        pwd = prc.EncryptString("WizOne2016", (dtParam.Rows[0][0] as string ?? "").ToString(), 2);
                    }
                }

                string msg = "";
                long userNumber = Convert.ToInt64(Session["UserId"].ToString());
                Dictionary<String, String> lista = Session["ListaParamRevisal"] as Dictionary<String, String>;
                Hashtable config = new Hashtable();
                foreach (string key in lista.Keys)
                    config.Add(key, lista[key]);

                config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
                config.Add("ORACONN", conn);
                config.Add("ORAUSER", Constante.tipBD == 1 ? DB : user);
                config.Add("ORAPWD", pwd);
                config.Add("ORALOGIN", user);              

                msg = RevisalDLL.Revisal.createTables(config);
                if (msg.Length > 0)
                {
                    MessageBox.Show(msg);
                    return;
                }

                DateTime dataInactivi = Convert.ToDateTime(deDataInactivi.Value);
                DateTime dataDeLa = Convert.ToDateTime(deDeLa.Value);
                DateTime dataLa = Convert.ToDateTime(deLa.Value);              

                msg = "";
                msg = RevisalDLL.Revisal.LoadSalariati(userNumber, false, dataInactivi, dataDeLa, dataLa, config, 1);
                if (msg.Length > 0)
                {
                    MessageBox.Show(msg);
                    return;
                }
                msg = "";
                msg = RevisalDLL.Revisal.LoadContracte(userNumber, false, dataInactivi, dataDeLa, dataLa, config, 1);
                if (msg.Length > 0)
                {
                    MessageBox.Show(msg);
                    return;
                }
                msg = "";
                msg = RevisalDLL.Revisal.LoadSporuri(chkCalc.Checked, dataLa, config);
                if (msg.Length > 0)                
                    MessageBox.Show(msg);                   
                else
                    MessageBox.Show("Pregatirea datelor a avut loc cu succes!");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void btnGenerare_Click()
        {
            try
            {
                string cnApp = Constante.cnnWeb;
                string tmp = cnApp.Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = cnApp.Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                string DB = tmp.Split(';')[0];
                tmp = cnApp.Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                tmp = cnApp.Split(new[] { "PASSWORD=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                string sql = "SELECT \"Valoare\", \"Criptat\"  FROM \"tblParametrii\" WHERE \"Nume\" = 'ORAPWD'";
                DataTable dtParam = General.IncarcaDT(sql, null);
                if (dtParam != null && dtParam.Rows.Count > 0)
                {
                    if (dtParam.Rows[0][1] == null || dtParam.Rows[0][1].ToString().Length <= 0 || dtParam.Rows[0][1].ToString() == "0")
                        pwd = dtParam.Rows[0][0].ToString();
                    else
                    {
                        CriptDecript prc = new CriptDecript();
                        pwd = prc.EncryptString("WizOne2016", (dtParam.Rows[0][0] as string ?? "").ToString(), 2);
                    }
                }

                Dictionary<String, String> lista = Session["ListaParamRevisal"] as Dictionary<String, String>;
                Hashtable config = new Hashtable();
                foreach (string key in lista.Keys)
                    config.Add(key, lista[key]);

                config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
                config.Add("ORACONN", conn);
                config.Add("ORAUSER", Constante.tipBD == 1 ? DB : user);
                config.Add("ORAPWD", pwd);
                config.Add("ORALOGIN", user);

                DateTime dataLa = Convert.ToDateTime(deLa.Value);
                DateTime dataSpor = Convert.ToDateTime(deDataSpor.Value);
                string cale = HostingEnvironment.MapPath("~/FisiereRevisal/");
                string nume = "XmlReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                string fileName = HostingEnvironment.MapPath("~/FisiereRevisal/") + nume; 
                var f = File.Create(fileName);
                f.Close();
                string msg = RevisalDLL.Revisal.GenerareFisier(dataLa, dataSpor, chkCalc.Checked, config, fileName, cale);
                MessageBox.Show(msg);

                byte[] fisier = File.ReadAllBytes(fileName);
                if (fisier != null)
                {
                    MemoryStream stream = new MemoryStream(fisier);
                    Response.Clear();
                    MemoryStream ms = stream;
                    Response.ContentType = "text/xml";
                    Response.AddHeader("Content-Disposition", "attachment;filename=" + nume);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                }
                File.Delete(fileName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void SalvareParam()
        {
            try
            {
                Dictionary<String, String> lista = Session["ListaParamRevisal"] as Dictionary<String, String>;
                string sql = "DELETE FROM \"tblParametrii\" WHERE \"Nume\" IN ('REVISAL_SAL', 'REVISAL_BIFAINACTIVI', 'REVISAL_DATAINACTIVI', 'REVISAL_DATA_SF_INCET', 'REVISAL_SPORURI_CALCULATE_CUVENITE', 'REVISAL_INTERVAL_MODIFICARI', 'REVISAL_SPORURI_ANGAJATI_NOI')";
                General.ExecutaNonQuery(sql, null);

                foreach (string key in lista.Keys)
                {
                    string templ = "INSERT INTO \"tblParametrii\" (\"Nume\", \"Valoare\") VALUES ('{0}', '{1}')";
                    sql = string.Format(templ, key, lista[key]);
                    General.ExecutaNonQuery(sql, null);
            
                }
            }
            catch (Exception ex)
            {    

                General.MemoreazaEroarea(ex, "Revisal", new StackTrace().GetFrame(0).GetMethod().Name);
            }    
        }
        public void btnSalvare_Click(object sender, EventArgs e)
        {
            SalvareParam();
        }


        public void btnGen_Click(object sender, EventArgs e)
        {
            btnGenerare_Click();
        }

    }


}