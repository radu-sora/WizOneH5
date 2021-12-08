using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Web;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WizOne.Module;
using Wizrom.Reports.Code;
using Wizrom.Reports.Models;
using DevExpress.Pdf;
using DevExpress.XtraRichEdit;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;

namespace WizOne.Pagini
{
    public partial class TrimitereFluturasi : System.Web.UI.Page
    {
        private static readonly HttpClient client = new HttpClient();

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ","10"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);             

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");


                btnTrimitere.Text = Dami.TraduCuvant("btnTrimitere", "Trimitere");              
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

            
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblSub.InnerText = Dami.TraduCuvant("Subcompanie");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Dept");
                lblSubDept.InnerText = Dami.TraduCuvant("SubDept");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");
                lblMail.InnerText = Dami.TraduCuvant("Are e-mail");
                lblParola.InnerText = Dami.TraduCuvant("Are parola");


                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                //Radu 13.12.2019
                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();
                Session["PaginaWeb"] = "Pagini.TrimitereFluturasi";

                if (!IsPostBack)
                {
                    txtAnLuna.Value = DateTime.Now;
                    Session["TrimitereFluturasi_Angajati"] = null;
                    Session["InformatiaCurenta_TF"] = null;
                    IncarcaAngajati();
                    DataTable dtParam = General.IncarcaDT("SELECT \"Nume\", \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" IN ('TrimitereFluturas_Subiect', 'TrimitereFluturas_Continut')", null);
                    if (dtParam != null && dtParam.Rows.Count > 0)
                        for (int i = 0; i < dtParam.Rows.Count; i++)
                        {
                            if (dtParam.Rows[i][0].ToString() == "TrimitereFluturas_Subiect")
                                txtSubiect.Text = General.Nz(dtParam.Rows[i][1], "").ToString();
                            if (dtParam.Rows[i][0].ToString() == "TrimitereFluturas_Continut")
                                txtContinut.Html = General.Nz(dtParam.Rows[i][1], "").ToString();
                        }
                }
                else
                {
             
                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();
                    cmbAng.DataSource = Session["TrimitereFluturasi_Angajati"];
                    cmbAng.DataBind();

                    DataTable dtert = Session["InformatiaCurenta_TF"] as DataTable;

                    if (Session["InformatiaCurenta_TF"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta_TF"];
                        grDate.DataBind();
                    }          
                    
                       
                }

                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                    cmbDept.DataBind();
                }
                else
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                    cmbDept.DataBind();
                }
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT("SELECT F00809, F00810 FROM F008", null);
                cmbBirou.DataBind();

           
                cmbCateg.DataSource = General.IncarcaDT(@"SELECT ""Denumire"" AS ""Id"", ""Denumire"" FROM ""viewCategoriePontaj"" GROUP BY ""Denumire"" ", null);
                cmbCateg.DataBind();

                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Denumire", typeof(string));

                dt.Rows.Add(0, "Nu");
                dt.Rows.Add(1, "Da");

                cmbMail.DataSource = dt;
                cmbMail.DataBind();

                cmbParola.DataSource = dt;
                cmbParola.DataBind();

                //#1014
                string lstIds = Dami.ValoareParam("IdRaportFluturasMail", "-99");
                DataTable dtDoc = General.IncarcaDT("select DynReportId as Id, Name as Denumire from DynReports where DynReportId in (" + lstIds + ")", null);
                dtDoc.Rows.Add(1000000 + 0, "Adev. Sănătate 2019");
                dtDoc.Rows.Add(1000000 + 1, "Adev. Sănătate");
                dtDoc.Rows.Add(1000000 + 2, "Adev. Venituri anuale");
                dtDoc.Rows.Add(1000000 + 3, "Adev. CIC");
                dtDoc.Rows.Add(1000000 + 4, "Adev. Șomaj");
                dtDoc.Rows.Add(1000000 + 6, "Adev. Stagiu");
                dtDoc.Rows.Add(1000000 + 7, "Adev. Vechime");
                dtDoc.Rows.Add(1000000 + 11, "Adev. Deplasare");
                dtDoc.Rows.Add(1000000 + 12, "Adev. Sănătate 2020");
                dtDoc.Rows.Add(1000000 + 13, "Adev. Șomaj tehnic 2020");
                crView.DataSource = dtDoc;
                crView.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnTrimitere_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> lstMarci = new Dictionary<int, string>();
                //string[] sablon = new string[11];

                txtLog.Text = "";

                if (txtAnLuna.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat luna si anul!"), MessageBox.icoError);
                    return;
                }

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "Email", "F10016", "NumeComplet" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                //#1014
                var selectedValues = crView.GetSelectedFieldValues(new string[] { "Id" });
                if (selectedValues.Count <= 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun document!"), MessageBox.icoError);
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    lstMarci.Add(Convert.ToInt32(General.Nz(arr[0], -99)), General.Nz(arr[1], "").ToString() + "_#_$_&_" + General.Nz(arr[2], "").ToString() + "_#_$_&_" + General.Nz(arr[3], "").ToString());
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {

                    string msg = TrimitereFluturasiMail(lstMarci, (int)selectedValues[0]);
                  
                    //if (msg.Length <= 0)                        
                    //    MessageBox.Show(Dami.TraduCuvant("Trimitere reusita!"), MessageBox.icoSuccess);

                    if (msg.Length > 0)
                        txtLog.Text = "S-au intalnit urmatoarele erori:\n" + msg;
                    else
                        txtLog.Text = "";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnWA_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> lstMarci = new Dictionary<int, string>();
                //string[] sablon = new string[11];

                txtLog.Text = "";

                if (txtAnLuna.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat luna si anul!"), MessageBox.icoError);
                    return;
                }

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "Email", "F10016", "NumeComplet" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                //#1014
                var selectedValues = crView.GetSelectedFieldValues(new string[] { "Id" });
                if (selectedValues.Count <= 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun document!"), MessageBox.icoError);
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    lstMarci.Add(Convert.ToInt32(General.Nz(arr[0], -99)), General.Nz(arr[1], "").ToString() + "_#_$_&_" + General.Nz(arr[2], "").ToString() + "_#_$_&_" + General.Nz(arr[3], "").ToString());
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {

                    //string msg = TrimitereFluturasiMail(lstMarci, (int)selectedValues[0]);

                    ////if (msg.Length <= 0)                        
                    ////    MessageBox.Show(Dami.TraduCuvant("Trimitere reusita!"), MessageBox.icoSuccess);

                    //if (msg.Length > 0)
                    //    txtLog.Text = "S-au intalnit urmatoarele erori:\n" + msg;
                    //else
                    //    txtLog.Text = "";


                    //HttpClient client = new SystemNetHttpClient();
                    //Request request = new Request(HttpMethod.Get, "https://api.twilio.com:8443");
                    //Response response = client.MakeRequest(request);
                    //Console.Write(response.Content);


                    //TwilioClient.Init(
                    //    Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"),
                    //    Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN"));

                    //var message = MessageResource.Create(
                    //    from: new PhoneNumber("whatsapp:+14155238886"),
                    //    to: new PhoneNumber("whatsapp:+40723899574"),
                    //    body: "Test");

                    //string msg = message.Sid;

                    //string status = "";
                    //WhatsApp wa = new WhatsApp("40723899574", "", "Radu", false, false);
                    //wa.OnConnectSuccess += () =>
                    //{
                    //    wa.OnLoginSuccess += (phoneNumber, data) =>
                    //    {
                    //        wa.SendMessage("40723899574", "Test");
                    //    };
                    //    wa.OnLoginFailed += (data) =>
                    //    {
                    //        status = data;
                    //    };
                    //    wa.Login();
                    //};
                    //wa.OnConnectFailed += (ex) =>
                    //{
                    //    status = ex.StackTrace;
                    //};
                    //wa.Connect();

                    TrimitereWA();

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void TrimitereWA()
        {
            try
            {

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();                    
                    data["url"] = "https://99deb2cc6165910debbfaa5ccafa4d21.m.pipedream.net";

                    var antet = new NameValueCollection();
                    antet["D360-API-KEY"] = "tMhush_sandbox";

                    wb.Headers.Add(antet);

                    var response = wb.UploadValues("https://waba-sandbox.360dialog.io/v1/configs/webhook", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }





                //var client = new RestClient("https://waba-sandbox.360dialog.io/v1/configs/webhook");
                //// client.Authenticator = new HttpBasicAuthenticator(username, password);
                //var request = new RestRequest("resource/{id}");
                //request.AddParameter("D360-API-KEY", "1234567_sandbox");
                //request.AddParameter("url", "https://enjgn0xpx1j6afb.m.pipedream.net");
                //request.AddHeader("header", "value");
                ////request.AddFile("file", path);
                //var response = client.Post(request);
                //var content = response.Content; // Raw content as string     

                //var values = new Dictionary<string, string>
                //    {
                //        { "D360-API-KEY", "1234567_sandbox" },
                //        { "url", "https://pipedream.com/@radusora/p_ljCnnbB/edit?e=21xLhi0udZMfUn5qYXRbSn8BMN8" }
                //    };

                //var content = new FormUrlEncodedContent(values);

                //var response = await client.PostAsync("https://waba-sandbox.360dialog.io/v1/configs/webhook", content);

                //var responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string TrimitereFluturasiMail(Dictionary<int, string> lstMarci, int reportId)
        {
            string msg = "";

            try
            {
                string interval = Dami.ValoareParam("IntervalTrimitereEmailuri", "15");
                foreach (int key in lstMarci.Keys)
                {
                    List<Notif.metaAdreseMail> lstOne = new List<Notif.metaAdreseMail>();
                    if (lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[0].Length <= 0)
                    {
                        msg += "Angajatul cu marca " + key + " nu are completat e-mail-ul!\n";
                        continue;
                    }

                    if (lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1].Length <= 0)
                    {
                        msg += "Angajatul cu marca " + key + " nu are completata parola pentru PDF!\n";
                        continue;
                    }

                    lstOne.Add(new Notif.metaAdreseMail { Mail = lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[0], Destinatie = "TO", IncludeLinkAprobare = 0 });
                    //int reportId = Convert.ToInt32(Dami.ValoareParam("IdRaportFluturasMail", "-99"));
                    int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                    int an = Convert.ToDateTime(txtAnLuna.Value).Year;

                    if (reportId < 1000000)
                    {
                        using (var entities = new ReportsEntities())
                        using (var xtraReport = new XtraReport())
                        {
                            var report = entities.Reports.Find(reportId);

                            using (var memStream = new MemoryStream(report.LayoutData))
                                xtraReport.LoadLayoutFromXml(memStream);

                            var values = new
                            {
                                Implicit = new { UserId = Session?["UserId"] },
                                Explicit = new { Angajat = key.ToString(), Luna = luna, An = an }
                            };
                            var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                            var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                            var parameters = xtraReport.ObjectStorage.OfType<DevExpress.DataAccess.Sql.SqlDataSource>().
                                SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                                Where(p => p.Type != typeof(Expression));

                            foreach (var param in parameters)
                            {
                                var name = param.Name.TrimStart('@');
                                var value = explicitValues?.SingleOrDefault(p => p.Name == name)?.GetValue(values.Explicit) ??
                                    implicitValues.SingleOrDefault(p => p.Name == name)?.GetValue(values.Implicit);

                                if (value != null)
                                    param.Value = Convert.ChangeType(value, param.Type);
                            }

                            xtraReport.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService());
                            PdfExportOptions pdfOptions = xtraReport.ExportOptions.Pdf;
                            pdfOptions.PasswordSecurityOptions.OpenPassword = lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1];                

                            MemoryStream mem = new MemoryStream();
                            xtraReport.ExportToPdf(mem, pdfOptions);
                            mem.Seek(0, System.IO.SeekOrigin.Begin);

                            string numeFis = "Fluturaș_" + key + ".pdf";
                            if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.Elanor)
                            {
                                string dataInc = an.ToString() + luna.ToString().PadLeft(2, '0') + "01";
                                string dataSf = an.ToString() + luna.ToString().PadLeft(2, '0') + DateTime.DaysInMonth(an, luna).ToString();

                                numeFis = "P_SLP_02344_" + dataInc + "_" + dataSf + "_00_V2_0000_00000_FILE_" + key + "_" + lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[2].Replace(' ', '_') + ".pdf";
                            }

                            Notif.TrimiteMail(lstOne, txtSubiect.Text, (txtContinut.Html ?? "").ToString(), 0, numeFis, "", 0, "", "", Convert.ToInt32(Session["IdClient"]), mem);
                            mem.Close();
                            mem.Flush();

                            System.Threading.Thread.Sleep(Convert.ToInt32(interval) * 1000);
                        }
                    }
                    else
                    {//#1014 - Adeverinta
                        List<int> lst = new List<int>();
                        lst.Add(key);
                        string numeFisier = "";
                        byte[] fisier = GenerareAdeverinta(lst, reportId - 1000000, Convert.ToDateTime(txtAnLuna.Value).Year, lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1],out numeFisier);
                        MemoryStream mem = new MemoryStream(fisier);
                        Notif.TrimiteMail(lstOne, txtSubiect.Text, (txtContinut.Html ?? "").ToString(), 0, numeFisier.Split('.')[0] + ".pdf", "", 0, "", "", Convert.ToInt32(Session["IdClient"]), mem);
                        mem.Close();
                        mem.Flush();
                        System.Threading.Thread.Sleep(Convert.ToInt32(interval) * 1000);
                    }

                }
                if (msg.Length <= 0)
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);
                else
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes, dar cu unele erori! Verificati log-ul!"), MessageBox.icoWarning);
            }
            catch (Exception ex)
            {
                msg += "Eroare la trimitere!\n";
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSalvare_Click(object sender, EventArgs e)
        {
            try
            {
                General.ExecutaNonQuery("UPDATE \"tblParametrii\" SET \"Valoare\" = '" + txtSubiect.Text + "' WHERE \"Nume\" = 'TrimitereFluturas_Subiect'", null);
                General.ExecutaNonQuery("UPDATE \"tblParametrii\" SET \"Valoare\" = '" + txtContinut.Html + "' WHERE \"Nume\" = 'TrimitereFluturas_Continut'", null);
                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltruSterge_Click()
        {
            try
            {
                cmbAng.Value = null;
                cmbSub.Value = null;
                cmbSec.Value = null;
                cmbFil.Value = null;
                cmbDept.Value = null;
                cmbSubDept.Value = null;
                cmbBirou.Value = null;                
                cmbCateg.Value = null;
                cmbMail.Value = null;
                cmbParola.Value = null;

                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                cmbDept.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                bool esteStruc = true;      
                switch (e.Parameter)
                {
                    case "cmbSub":
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbFil":
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbSec":
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbDept":
                        cmbSubDept.Value = null;
                        break;
                    case "EmptyFields":
                        btnFiltruSterge_Click();
                        return;
                    case "cmbRol":
                        Session["TrimitereFluturasi_Angajati"] = null;
                        IncarcaAngajati();
                        break;
                }

                if (esteStruc)
                {
                    cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                    cmbSec.DataBind();
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                        cmbDept.DataBind();
                    }
                    else
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                        cmbDept.DataBind();
                    }
                    cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                    cmbSubDept.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void IncarcaGrid()
        {
            try
            {
                grDate.KeyFieldName = "F10003";

                DataTable dt = GetF100NumeCompletTrimitereFluturasi(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99),
                    Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbMail.Value ?? -99), Convert.ToInt32(cmbParola.Value ?? -99), General.Nz(cmbCateg.Value,"").ToString());

                grDate.DataSource = dt;
                Session["InformatiaCurenta_TF"] = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetF100NumeCompletTrimitereFluturasi(int idUser, int idSubcomp = -99, int idFiliala = -99, int idSectie = -99, int idDept = -99, int idSubdept = -99, int idBirou = -99, int idAngajat = -9, int areEmail = -99, int areParola = -99, string idCateg = "")
        {
            DataTable dt = new DataTable();

            try
            {
                string op = "+", func = "LEN";
                if (Constante.tipBD == 2)
                {
                    op = "||";
                    func = "LENGTH";
                }


                string strSql = @"SELECT Y.* FROM(
                                SELECT DISTINCT CAST(A.F10003 AS int) AS F10003,  A.F10008 {0} ' ' {0} A.F10009 AS ""NumeComplet"",                                  
                                A.F10002, A.F10004, A.F10005, A.F10006, A.F10007, X.F100958, X. F100959, A.F10025, A.F100894, USERS.""Mail"", F10016,
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"",
                                CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) THEN ""Mail"" ELSE F100894 END AS ""Email"",
                                CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) AND (""Mail"" IS NULL OR {6}(""Mail"") = 0) THEN 0 ELSE 1 END AS ""AreMail"",
                                CASE WHEN F10016 IS NULL OR {6}(F10016) = 0 THEN 0 ELSE 1 END AS ""AreParola""
                                {3}

                                FROM ""relGrupAngajat"" B                                
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""                                
                                INNER JOIN F100 A ON b.F10003 = a.F10003   
                                INNER JOIN F1001 X ON A.F10003 = X.F10003                            
                                LEFT JOIN F718 D ON A.F10071 = D.F71802                                
                                LEFT JOIN F002 E ON A.F10002 = E.F00202                                
                                LEFT JOIN F003 F ON A.F10004 = F.F00304                                
                                LEFT JOIN F004 G ON A.F10005 = G.F00405                                
                                LEFT JOIN F005 H ON A.F10006 = H.F00506                                
                                LEFT JOIN F006 I ON A.F10007 = I.F00607                                
                                LEFT JOIN F007 K ON X.F100958 = K.F00708  
                                LEFT JOIN F008 L ON X.F100959 = L.F00809   
                                LEFT JOIN USERS ON A.F10003 = USERS.F10003
                                {5}
                                WHERE C.""IdSuper"" = {1} {4}

                                UNION

                                SELECT DISTINCT CAST(A.F10003 AS int) AS F10003,  A.F10008 {0} ' ' {0} A.F10009 AS ""NumeComplet"",                                  
                                A.F10002, A.F10004, A.F10005, A.F10006, A.F10007, X.F100958, X. F100959, A.F10025  , A.F100894, USERS.""Mail"", F10016,
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"",
                                CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) THEN ""Mail"" ELSE F100894 END AS ""Email"",
                                CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) AND (""Mail"" IS NULL OR {6}(""Mail"") = 0) THEN 0 ELSE 1 END AS ""AreMail"",
                                CASE WHEN F10016 IS NULL OR {6}(F10016) = 0 THEN 0 ELSE 1 END AS ""AreParola""
                                {3}

                                FROM ""relGrupAngajat"" B                                
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""                                
                                INNER JOIN F100 A ON b.F10003 = a.F10003 
                                INNER JOIN F1001 X ON A.F10003 = X.F10003                               
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")                                
                                LEFT JOIN F718 D ON A.F10071 = D.F71802                                
                                LEFT JOIN F002 E ON A.F10002 = E.F00202                                
                                LEFT JOIN F003 F ON A.F10004 = F.F00304                                
                                LEFT JOIN F004 G ON A.F10005 = G.F00405                                
                                LEFT JOIN F005 H ON A.F10006 = H.F00506                                
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN F007 K ON X.F100958 = K.F00708  
                                LEFT JOIN F008 L ON X.F100959 = L.F00809 
                                LEFT JOIN USERS ON A.F10003 = USERS.F10003
                                {5}
                                WHERE J.""IdUser"" = {1} {4}
  
                                                           
                                ) Y 
                                {2}    ";


                string tmp = "", cond = "", condCtr= "";

                if (idSubcomp != -99)
                {
                    tmp = string.Format("  Y.F10004 = {0} ", idSubcomp);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idFiliala != -99)
                {
                    tmp = string.Format("  Y.F10005 = {0} ", idFiliala);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idSectie != -99)
                {
                    tmp = string.Format("  Y.F10006 = {0} ", idSectie);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idDept != -99)
                {
                    tmp = string.Format("  Y.F10007 = {0} ", idDept);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idSubdept != -99)
                {
                    tmp = string.Format("  Y.F100958 = {0} ", idSubdept);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idBirou != -99)
                {
                    tmp = string.Format("  Y.F100959 = {0} ", idBirou);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idAngajat != -99)
                {
                    tmp = string.Format("  Y.F10003 = {0} ", idAngajat);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (areEmail  != -99)
                {
                    if (cond.Length <= 0)
                        cond = " WHERE (CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) AND (\"Mail\" IS NULL OR {6}(\"Mail\") = 0) THEN 0 ELSE 1 END) = " + areEmail;
                    else
                        cond += " AND (CASE WHEN (F100894 IS NULL OR {6}(F100894) = 0) AND (\"Mail\" IS NULL OR {6}(\"Mail\") = 0) THEN 0 ELSE 1 END) = " + areEmail;
                }

                if (areParola != -99)
                {
                    if (cond.Length <= 0)
                        cond = " WHERE (CASE WHEN F10016 IS NULL OR {6}(F10016) = 0 THEN 0 ELSE 1 END) = " + areParola;
                    else
                        cond += " AND (CASE WHEN F10016 IS NULL OR {6}(F10016) = 0 THEN 0 ELSE 1 END) = " + areParola;
                }

                string cmpCateg = @" ,NULL AS ""Categorie"" ";
                string strLeg = "";
                string filtruPlus = "";
                string sqlCateg = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewCategoriePontaj'";
                if (Constante.tipBD == 2)
                    sqlCateg = "SELECT COUNT(*) FROM user_views where view_name = 'viewCategoriePontaj'";
                int este = Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlCateg), 0));
                if (este == 1)
                {
                    cmpCateg = @" ,CTG.""Denumire"" AS ""Categorie"" ";
                    if (idCateg != "")
                        filtruPlus += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                    strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                }

                if (cond.Length <= 0)
                    cond = " WHERE (Y.F10025 = 0 OR Y.F10025 = 999) ";
                else
                    cond += " AND (Y.F10025 = 0 OR Y.F10025 = 999) ";

                strSql += cond;

                strSql = string.Format(strSql, op, idUser, condCtr, cmpCateg, filtruPlus, strLeg, func);

                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        private void IncarcaAngajati()
        {
            try
            {                

                if (Session["TrimitereFluturasi_Angajati"] == null)
                {
                    DataTable dt = General.IncarcaDT(SelectAngajati(), null);

                    DataView view = new DataView(dt);
                    DataTable dtRol = view.ToTable(true, "Rol", "RolDenumire");

                    string lstIdSuper = Dami.ValoareParam("Adev_IdSuper", "");

                    DataTable dtSuper = new DataTable();
                    dtSuper.Columns.Add("Rol", typeof(int));
                    dtSuper.Columns.Add("RolDenumire", typeof(string));

                    if (lstIdSuper.Length > 0)
                        dtSuper = dtRol.Select("Rol IN (" + lstIdSuper + ")").CopyToDataTable();
                    else
                        dtSuper = dtRol;

                    cmbRol.DataSource = dtSuper;
                    cmbRol.DataBind();

                    cmbRol.SelectedIndex = 0;

                    DataTable dtAngFiltrati = General.IncarcaDT(SelectAngajati("WHERE Rol = " + cmbRol.Value.ToString()), null);
                    cmbAng.DataSource = dtAngFiltrati;
                    cmbAng.DataBind();

                    Session["TrimitereFluturasi_Angajati"] = dtAngFiltrati;
                }

                DataTable dtAng = Session["TrimitereFluturasi_Angajati"] as DataTable;
                cmbAng.DataSource = null;
                cmbAng.DataSource = dtAng;               
                cmbAng.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" , ""Rol"", ""RolDenumire""
                        FROM (
                        SELECT A.F10003, 0 AS ""Rol"",  COALESCE((SELECT COALESCE(""Alias"", ""Denumire"") FROM ""tblSupervizori"" WHERE ""Id""=0),'Angajat') AS ""RolDenumire""
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003, B.""IdSuper"" AS ""Rol"", CASE WHEN D.""Alias"" IS NOT NULL AND D.""Alias"" <> '' THEN D.""Alias"" ELSE D.""Denumire"" END AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        LEFT JOIN ""tblSupervizori"" D ON D.""Id"" = B.""IdSuper""
                        WHERE B.""IdUser""= {Session["UserId"]}) B                        
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607 {filtru}";

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {

                string str = e.Parameters;
                if (str != "")
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnGenerare_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> lstMarci = new Dictionary<int, string>();

                txtLog.Text = "";

                if (txtAnLuna.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat luna si anul!"), MessageBox.icoError);
                    return;
                }

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "Email", "F10016", "NumeComplet" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                //#1014
                var selectedValues = crView.GetSelectedFieldValues(new string[] { "Id" });
                if (selectedValues.Count <= 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun document!"), MessageBox.icoError);
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    lstMarci.Add(Convert.ToInt32(General.Nz(arr[0], -99)), General.Nz(arr[1], "").ToString() + "_#_$_&_" + General.Nz(arr[2], "").ToString() + "_#_$_&_" + General.Nz(arr[3], "").ToString());
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {

                    string msg = SalvareFluturasi(lstMarci, (int)selectedValues[0]);

                    if (msg.Length > 0)
                        txtLog.Text = "S-au intalnit urmatoarele erori:\n" + msg;
                    else
                        txtLog.Text = "";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string SalvareFluturasi(Dictionary<int, string> lstMarci, int reportId)
        {
            string msg = "";

            try
            {
                string cale = Dami.ValoareParam("TrimitereFluturas_Locatie", "");
                string paramParola = Dami.ValoareParam("TrimitereFluturas_LipsaParola", "0");
                if (cale.Length <= 0)
                {
                    msg += "Nu ati precizat locatia in care sa fie salvati fluturasii!\n";
                    return msg;
                }

                string numeFis = Dami.ValoareParam("TrimitereFluturas_Denumire", "");
                if (numeFis.Length <= 0)
                {
                    msg += "Nu ati precizat denumirea sub care sa fie salvati fluturasii!\n";
                    return msg;
                }

                foreach (int key in lstMarci.Keys)
                {      
                    //int reportId = Convert.ToInt32(Dami.ValoareParam("IdRaportFluturasMail", "-99"));
                    int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                    int an = Convert.ToDateTime(txtAnLuna.Value).Year;

                    if (lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1].Length <= 0)
                    {
                        msg += "Angajatul cu marca " + key + " nu are completata parola pentru PDF!\n";
                        continue;
                    }

                    if (reportId < 1000000)
                    {
                        using (var entities = new ReportsEntities())
                        using (var xtraReport = new XtraReport())
                        {
                            var report = entities.Reports.Find(reportId);

                            using (var memStream = new MemoryStream(report.LayoutData))
                                xtraReport.LoadLayoutFromXml(memStream);

                            var values = new
                            {
                                Implicit = new { UserId = Session?["UserId"] },
                                Explicit = new { Angajat = key.ToString(), Luna = luna, An = an }
                            };
                            var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                            var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                            var parameters = xtraReport.ObjectStorage.OfType<DevExpress.DataAccess.Sql.SqlDataSource>().
                                SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                                Where(p => p.Type != typeof(Expression));

                            foreach (var param in parameters)
                            {
                                var name = param.Name.TrimStart('@');
                                var value = explicitValues?.SingleOrDefault(p => p.Name == name)?.GetValue(values.Explicit) ??
                                    implicitValues.SingleOrDefault(p => p.Name == name)?.GetValue(values.Implicit);

                                if (value != null)
                                    param.Value = Convert.ChangeType(value, param.Type);
                            }

                            xtraReport.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService());
                            PdfExportOptions pdfOptions = xtraReport.ExportOptions.Pdf;
                            if (paramParola != "1")
                            {
                                pdfOptions.PasswordSecurityOptions.OpenPassword = lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1];

                                if (lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1].Length <= 0)
                                {
                                    msg += "Angajatul cu marca " + key + " nu are completata parola pentru PDF!\n";
                                    continue;
                                }
                            }
                            MemoryStream mem = new MemoryStream();
                            xtraReport.ExportToPdf(mem, pdfOptions);
                            mem.Seek(0, System.IO.SeekOrigin.Begin);
                            string numeFisTmp = numeFis;
                            string dataInc = an.ToString() + luna.ToString().PadLeft(2, '0') + "01";
                            string dataSf = an.ToString() + luna.ToString().PadLeft(2, '0') + DateTime.DaysInMonth(an, luna).ToString();
                            numeFisTmp = numeFisTmp.Replace("<Anul>", an.ToString()).Replace("<Luna>", luna.ToString()).Replace("<Marca>", key.ToString()) + ".pdf";

                            using (FileStream file = new FileStream(cale + numeFisTmp, FileMode.Create, System.IO.FileAccess.Write))
                            {
                                byte[] bytes = new byte[mem.Length];
                                mem.Read(bytes, 0, (int)mem.Length);
                                file.Write(bytes, 0, bytes.Length);
                            }

                            mem.Close();
                            mem.Flush();
                        }
                    }
                    else
                    {//#1014 - Adeverinta
                        List<int> lst = new List<int>();
                        lst.Add(key);
                        string numeFisier = "";
                        byte[] fisier = GenerareAdeverinta(lst, reportId - 1000000, Convert.ToDateTime(txtAnLuna.Value).Year, lstMarci[key].Split(new string[] { "_#_$_&_" }, StringSplitOptions.None)[1], out numeFisier);
                        MemoryStream mem = new MemoryStream(fisier);
                        using (FileStream file = new FileStream(cale + numeFisier.Split('.')[0] + ".pdf", FileMode.Create, System.IO.FileAccess.Write))
                        {
                            byte[] bytes = new byte[mem.Length];
                            mem.Read(bytes, 0, (int)mem.Length);
                            file.Write(bytes, 0, bytes.Length);
                        }
                        mem.Close();
                        mem.Flush();                
                    }
                }
                if (msg.Length <= 0)
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);
                else
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes, dar cu unele erori! Verificati log-ul!"), MessageBox.icoWarning);
            }
            catch (Exception ex)
            {
                msg += "Eroare la salvare!\n";
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }

        private byte[] GenerareAdeverinta(List<int> lstMarci, int adev, int anul, string parola, out string fisier)
        {
            fisier = "";
            try
            {
                string msg = "";                
                Dictionary<String, String> lista = new Dictionary<string, string>();
                Adev.Adeverinta pagAdev = new Adev.Adeverinta();
                lista = pagAdev.LoadParameters();           

                string cnApp = Constante.cnnWeb;
                string tmp = cnApp.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                tmp = cnApp.ToUpper().Split(new[] { "DATA SOURCE=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = cnApp.ToUpper().Split(new[] { "USER ID=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                string DB = "";
                if (Constante.tipBD == 1)
                {
                    tmp = cnApp.ToUpper().Split(new[] { "INITIAL CATALOG=" }, StringSplitOptions.None)[1];
                    DB = tmp.Split(';')[0];
                }
                else
                    DB = user;

                string cale = HostingEnvironment.MapPath("~/Adeverinta");

                Hashtable Config = new Hashtable();

                Config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
                Config.Add("ORAUSER", DB);
                Config.Add("ORAPWD", pwd);
                Config.Add("ORALOGIN", user);

                string host = "", port = "";

                if (Constante.tipBD == 2)
                {
                    host = conn.Split('/')[0];
                    conn = conn.Split('/')[1];
                }
                Config.Add("ORACONN", conn);
                Config.Add("HOST_ADEV", host);
                Config.Add("PORT_ADEV", port);

                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE"));
                if (!folder.Exists)
                    folder.Create();

                string sql = "SELECT * FROM F100 WHERE F10003 = " + lstMarci[0];
                DataTable dtAng = General.IncarcaDT(sql, null);

                string listaM = "";
                foreach (int elem in lstMarci)
                    listaM += ";" + elem;

                listaM = listaM.Substring(1);

                int tipGen = 1;
                string data = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');


                String FileName = "";
                switch (adev)
                {
                    case 0:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_sanatate_2019_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_sanatate_2019_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 0, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 1:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_sanatate_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_sanatate_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 1, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 2:
                        if (lstMarci.Count() == 1)
                            fisier = dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + (lista["MAR"] == "1" ? lstMarci[0].ToString() : dtAng.Rows[0]["F10017"].ToString().Trim()) + ".xml";
                        else
                            fisier = "Adev_venituri_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/VENITURI_" + anul + "/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 2, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 3:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_CIC_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_CIC_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 3, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 4:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_SOMAJ_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_SOMAJ_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 4, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 6:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_Stagiu_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_Stagiu_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 6, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 7:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_Vechime_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_Vechime_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 7, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 11:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_Deplasare_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_Deplasare_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 11, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 12:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_sanatate_2020_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_sanatate_2020_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 12, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                    case 13:
                        if (lstMarci.Count() == 1)
                            fisier = "Adev_SOMAJ_TEHNIC_" + dtAng.Rows[0]["F10008"].ToString().Trim().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Trim().Replace(' ', '_') + "_" + lstMarci[0] + ".xml";
                        else
                            fisier = "Adev_SOMAJ_TEHNIC_" + data + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, 13, Config, HostingEnvironment.MapPath("~/Adeverinta/"), listaM.Split(';'), tipGen);
                        break;
                }

                XDocument doc;
                doc = XDocument.Load(FileName);
                Adev.Adeverinta.FlatToOpc(doc, FileName.Split('.')[0] + ".docx");
                File.Delete(FileName);

                using (RichEditDocumentServer wordProcessor = new RichEditDocumentServer())
                {
                    // Load a DOCX document.
                    wordProcessor.LoadDocument(FileName.Split('.')[0] + ".docx", DevExpress.XtraRichEdit.DocumentFormat.OpenXml);

                    // Specify export options.
                    PdfExportOptions options = new PdfExportOptions();
                    options.Compressed = false;
                    options.ImageQuality = PdfJpegImageQuality.Highest;

                    // Export the document to a stream.
                    using (FileStream pdfFileStream = new FileStream(FileName.Split('.')[0] + ".pdf", FileMode.Create))
                    {
                        wordProcessor.ExportToPdf(pdfFileStream, options);
                    }
                }
                File.Delete(FileName.Split('.')[0] + ".docx");  
                using (PdfDocumentProcessor pdfDocumentProcessor = new PdfDocumentProcessor())
                {
                    // Load a PDF document.
                    pdfDocumentProcessor.LoadDocument(FileName.Split('.')[0] + ".pdf");                    
                    // Specify printing, data extraction, modification, and interactivity permissions. 
                    PdfEncryptionOptions encryptionOptions = new PdfEncryptionOptions();
                    encryptionOptions.PrintingPermissions = PdfDocumentPrintingPermissions.Allowed;
                    encryptionOptions.DataExtractionPermissions = PdfDocumentDataExtractionPermissions.NotAllowed;
                    encryptionOptions.ModificationPermissions = PdfDocumentModificationPermissions.DocumentAssembling;
                    encryptionOptions.InteractivityPermissions = PdfDocumentInteractivityPermissions.Allowed;

                    // Specify the owner and user passwords for the document.  
                    encryptionOptions.OwnerPasswordString = parola;
                    encryptionOptions.UserPasswordString = parola;

                    // Specify the 256-bit AES encryption algorithm.
                    encryptionOptions.Algorithm = PdfEncryptionAlgorithm.AES256;

                    // Save the protected document with encryption settings.  
                    pdfDocumentProcessor.SaveDocument(FileName.Split('.')[0] + "_protected.pdf", new PdfSaveOptions() { EncryptionOptions = encryptionOptions });
                }

                byte[] fisierGen = File.ReadAllBytes(FileName.Split('.')[0] + "_protected.pdf");
                File.Delete(FileName.Split('.')[0] + ".pdf");
                File.Delete(FileName.Split('.')[0] + "_protected.pdf");
                return fisierGen;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }
    }
}
 
 
 
 
 
 
 
 
 
 
 
 