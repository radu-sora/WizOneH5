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
    public partial class ExportSAP : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnExportSAP.Text = Dami.TraduCuvant("btnExportSAP", "Export SAP");
                lblExportSAP.InnerText = Dami.TraduCuvant("Export SAP");
                chk1.Text = Dami.TraduCuvant("CheckBox1", "Zile CO");
                chk2.Text = Dami.TraduCuvant("CheckBox2", "Pontaj estimat");
                chk3.Text = Dami.TraduCuvant("CheckBox3", "Pontaj Lichidare");
                chk4.Text = Dami.TraduCuvant("CheckBox4", "view_suplimentar");
                chk5.Text = Dami.TraduCuvant("CheckBox5", "view_suplimentar");
                #endregion

                int nr = Convert.ToInt32(Dami.ValoareParam("NrOptiuniExportSAP", "3"));

                if (nr == 4)
                    chk4.ClientVisible = true;

                if (nr == 5)
                {
                    chk4.ClientVisible = true;
                    chk5.ClientVisible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void Export()
        {
            btnExportSAP.Enabled = false;
            string mesaj = ExportDateSAP();

            btnExportSAP.Enabled = true;
            MessageBox.Show(Dami.TraduCuvant(mesaj));
        }

        public string ExportDateSAP()
        {
            string mesaj = "", fileName = "";

            try
            {
                string cale = Dami.ValoareParam("CaleConfigPeopleSoft", "");       
                if (cale.Length <= 0)
                {
                    mesaj = Dami.TraduCuvant("Nu ati precizat calea fisierului Config in tblParametrii") + "!";
                    return mesaj;
                }

                string DB = Constante.BD;
                //string DB = "WIZONE_HGR_H5";
                string param = (chk1.Checked ? "1" : "0") + (chk2.Checked ? "1" : "0") + (chk3.Checked ? "1" : "0") + (chk4.ClientVisible ? (chk4.Checked ? "1" : "0") : "") + (chk5.ClientVisible ? (chk5.Checked ? "1" : "0") : "");

                wizone_exchange.Exchange.Export_Data(Session["UserId"].ToString(), DB, cale, param, out mesaj, out fileName);

                string output_path = (fileName.Length > 0 ? System.IO.Path.GetDirectoryName(cale) + @"\upld" + @"\" + fileName : "");
                if (output_path.Length > 0)
                {
                    var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/FisiereSAP"));
                    if (!folder.Exists)
                        folder.Create();
                    File.Copy(output_path, HostingEnvironment.MapPath("~/FisiereSAP/" + fileName), true);

                    byte[] fisierGen = File.ReadAllBytes(HostingEnvironment.MapPath("~/FisiereSAP/" + fileName));

                    if (fisierGen != null)
                    {
                        MemoryStream stream = new MemoryStream(fisierGen);
                        Response.Clear();
                        MemoryStream ms = stream;
                        Response.ContentType = "text/plain";
                        Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                        Response.Buffer = true;
                        ms.WriteTo(Response.OutputStream);

                        HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                        HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                        HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event

                        File.Delete(HostingEnvironment.MapPath("~/FisiereSAP/" + fileName));
                        File.Delete(System.IO.Path.GetDirectoryName(cale) + @"\upld" + @"\" + fileName);
                    }
                    else
                    {
                        mesaj = Dami.TraduCuvant("Fisierul nu a fost generat") + "!";
                    }
                }

            }
            catch (Exception ex)
            {
                mesaj = Dami.TraduCuvant("Eroare la exportul de date");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }


            return mesaj;
        }


        protected void btnExportSAP_Click(object sender, EventArgs e)
        {
            try
            {       
                if (!chk1.Checked && !chk2.Checked && !chk3.Checked && !chk4.Checked && !chk5.Checked)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati bifat nicio optiune") + "!");
                    return;
                }

                if (chk2.Checked && chk3.Checked)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu puteti bifa ambele tipuri de Pontaj (Estimat si Lichidare) in acelasi timp") + "!");
                    return;
                }
                Export();                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}