using DevExpress.XtraReports.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Web.Hosting;

namespace WizOne.Tactil
{
    public partial class Adeverinte : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesTactil();     

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();



                DataTable dt = General.IncarcaDT("SELECT * FROM \"tblConfigTactil\" WHERE COALESCE(\"Vizibil\",0)=1 AND \"Pagina\" = 'Adeverinte' ORDER BY \"Ordine\"", null);
                if (dt == null || dt.Rows.Count <= 0)
                {
                    dt = new DataTable();
                    dt.Columns.Add("Nume", typeof(string));
                    dt.Columns.Add("Denumire", typeof(string));
                    dt.Columns.Add("IdParam", typeof(int));
                    dt.Columns.Add("IdParam2", typeof(int));
             

                    dt.Rows.Add("AdeverintaMedic", "Adeverinta medic/spital", null, null);
                    dt.Rows.Add("AdeverintaAngajat", "Adeverinta de angajat", null, null);
                    if (HttpContext.Current.Session["IdClient"] == null || Convert.ToInt32(HttpContext.Current.Session["IdClient"]) != 23)
                    {
                        dt.Rows.Add("AdeverintaPractica", "Adeverinta de practica", null, null);
                        dt.Rows.Add("AdeverintaCresa", "Adeverinta cresa/gradinita", null, null);
                    }
                }
                dt.Rows.Add("Inapoi", "Inapoi", null, null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string nume = General.Nz(dt.Rows[i]["Denumire"], "").ToString();

                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.Attributes["class"] = "col-sm-4";

                    HtmlGenericControl divInn = new HtmlGenericControl("div");
                    divInn.Attributes["class"] = "badgeTactil";

                    LinkButton lnk = new LinkButton();
                    lnk.ID = "lnk" + General.Nz(dt.Rows[i]["Nume"], "").ToString() + (dt.Rows[i]["IdParam"] != null && dt.Rows[i]["IdParam"].ToString().Length > 0 ? "_" + General.Nz(dt.Rows[i]["IdParam"], "").ToString() : "");
                    lnk.Command += new CommandEventHandler(lnk_Command);
                    lnk.CommandName = "Action";
                    lnk.CommandArgument = General.Nz(dt.Rows[i]["IdParam"], "").ToString() + (dt.Rows[i]["IdParam2"] != null && dt.Rows[i]["IdParam2"].ToString().Length > 0 ? "_" + General.Nz(dt.Rows[i]["IdParam2"], "").ToString() : "");
                    lnk.OnClientClick = "AspLoading()";     


                    HtmlGenericControl divLnk = new HtmlGenericControl("div");

                    HtmlImage img = new HtmlImage();
                    if (General.Nz(dt.Rows[i]["Nume"], "").ToString() != "Inapoi")
                        img.Src = "../Fisiere/Imagini/bdgPtj.jpg";
                    else
                        img.Src = "../Fisiere/Imagini/bdgback.png";
                    img.Alt = nume;

                    HtmlGenericControl h = new HtmlGenericControl("h3");
                    h.InnerText = nume;

                    divLnk.Controls.Add(img);
                    lnk.Controls.Add(divLnk);
                    divInn.Controls.Add(lnk);
                    divInn.Controls.Add(h);
                    div.Controls.Add(divInn);
                    pnlGen.Controls.Add(div);
         
                }
                if (!IsPostBack)
                {
                    //TactilPrintareAdeverinte : 0 - se deschide raport; 1 - se printeaza direct
                    //Radu 09.01.2020 - s-a renuntat la parametru; printarea automata se face daca denumirea butonului contine 'print'
                    //dt = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TactilPrintareAdeverinte'", null);
                    //if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                    //{
                    //    Session["TactilPrintareAdeverinte"] = Convert.ToInt32(dt.Rows[0][0].ToString());
                    //}
                    //else
                    //    Session["TactilPrintareAdeverinte"] = 0;

                }


                //if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                //{
                //    divAdevCresa.Visible = false;
                //    divAdevPractica.Visible = false;
                //}


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void lnk_Command(object sender, CommandEventArgs e)
        {
            try
            {
                LinkButton lnk = sender as LinkButton;
                
                if (lnk.CommandArgument != null && lnk.CommandArgument.Length > 0)
                {
                    string[] lstParam = lnk.CommandArgument.Split('_');
                    int reportId = Convert.ToInt32(lstParam[0]);
                    var reportParams = null as object;
                    int tip = -1;                 

                    if (lstParam.Length > 1)
                        tip = Convert.ToInt32(lstParam[1]);

                    if (tip > 0)
                        reportParams = new
                        {
                            Angajat = Session["User_Marca"].ToString(),
                            TipAdeverinta = tip
                        };
                    else
                        reportParams = new
                        {
                            Angajat = Session["User_Marca"].ToString()
                        };

                    string nume = lnk.ID.Substring(3).ToLower();                  

                    // New report access interface
                    if (!nume.ToLower().Contains("print"))
                    {
                        var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(reportId);

                        Wizrom.Reports.Code.ReportProxy.View(reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);
                    }
                    else
                        Wizrom.Reports.Code.ReportProxy.Print(reportId, "", paramList: reportParams); // TODO: FIX03 - Use async method for non blocking UI.
                }  
                else
                {
                    string nume = lnk.ID.Substring(3).ToLower();
                    if (nume.ToLower().Contains("print"))
                        Session["TactilPrintareAdeverinte"] = 1;
                    else
                        Session["TactilPrintareAdeverinte"] = 0;
                    switch (nume)
                    {
                        case "adeverintamedic":
                        case "adeverintamedicprint":
                            lnkAdevMedic_Click();
                            break;
                        case "adeverintaangajat":
                        case "adeverintaangajatprint":
                            lnkAdevAng_Click();
                            break;
                        case "adeverintapractica":
                        case "adeverintapracticaprint":
                            lnkAdevPractica_Click();
                            break;
                        case "adeverintacresa":
                        case "adeverintacresaprint":
                            lnkAdevGrad_Click(); ;
                            break;
                        case "inapoi":
                            btnBack_Click(sender, e);
                            break;

                    }
                }             
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../Tactil/Main", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0" || tip == "3") //Radu 03.12.2021 - #914
                    Response.Redirect("../DefaultTactil", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard", false);
                //else if (tip == "3")
                //    Response.Redirect("../DefaultTactilExtra", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkAdevMedic_Click()
        {
            try
            {
                Session["TactilAdeverinte"] = "Medic";
                Session["NrInregAdev"] = General.GetnrInreg();
                if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                {                   
                    Session["PrintDocument"] = "AdeverintaMedic";
                    Session["PaginaWeb"] = "Tactil/Adeverinte";
                    Response.Redirect("~/Reports/ImprimaTactil", false);                    
                }
                else
                {
                    Reports.AdeverintaMedic dlreport = new Reports.AdeverintaMedic();
                    dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    dlreport.Margins.Top = 10;
                    dlreport.Margins.Bottom = 10;
                    dlreport.Margins.Left = 50;
                    dlreport.Margins.Right = 50;
                    dlreport.PrintingSystem.ShowMarginsWarning = false;
                    dlreport.ShowPrintMarginsWarning = false;
                    dlreport.CreateDocument();
                    ReportPrintTool pt = new ReportPrintTool(dlreport);
                    pt.Print();
                    General.LogAdeverinta(dlreport);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkAdevGrad_Click()
        {
            try
            {
                Session["TactilAdeverinte"] = "Gradinita";
                Session["NrInregAdev"] = General.GetnrInreg();
                if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                {
            
                    Session["PrintDocument"] = "AdeverintaGenerala";
                    Session["PaginaWeb"] = "Tactil/Adeverinte";
                    Response.Redirect("~/Reports/ImprimaTactil", false);
                    
                }
                else
                {
                    Reports.AdeverintaGenerala dlreport = new Reports.AdeverintaGenerala();
                    dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    dlreport.Margins.Top = 10;
                    dlreport.Margins.Bottom = 10;
                    dlreport.Margins.Left = 50;
                    dlreport.Margins.Right = 50;
                    dlreport.PrintingSystem.ShowMarginsWarning = false;
                    dlreport.ShowPrintMarginsWarning = false;
                    dlreport.CreateDocument();
                    ReportPrintTool pt = new ReportPrintTool(dlreport);
                    pt.Print();
                    General.LogAdeverinta(dlreport);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkAdevPractica_Click()
        {
            try
            {
                Session["TactilAdeverinte"] = "Practica";
                Session["NrInregAdev"] = General.GetnrInreg();
                if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                {
              
                    Session["PrintDocument"] = "AdeverintaGenerala";
                    Session["PaginaWeb"] = "Tactil/Adeverinte";
                    Response.Redirect("~/Reports/ImprimaTactil", false);
                    
                }
                else
                {
                    Reports.AdeverintaGenerala dlreport = new Reports.AdeverintaGenerala();
                    dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    dlreport.Margins.Top = 10;
                    dlreport.Margins.Bottom = 10;
                    dlreport.Margins.Left = 50;
                    dlreport.Margins.Right = 50;
                    dlreport.PrintingSystem.ShowMarginsWarning = false;
                    dlreport.ShowPrintMarginsWarning = false;
                    dlreport.CreateDocument();
                    ReportPrintTool pt = new ReportPrintTool(dlreport);
                    pt.Print();
                    General.LogAdeverinta(dlreport);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void lnkAdevAng_Click()
        {
            try
            {
                Session["TactilAdeverinte"] = "Angajat";
                Session["NrInregAdev"] = General.GetnrInreg();
                if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                {
   
                   
                        Session["PrintDocument"] = "AdeverintaGenerala";
                        Session["PaginaWeb"] = "Tactil/Adeverinte";
                        Response.Redirect("~/Reports/ImprimaTactil", false);
                    
                }
                else
                {
                    Reports.AdeverintaGenerala dlreport = new Reports.AdeverintaGenerala();
                    dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    dlreport.Margins.Top = 10;
                    dlreport.Margins.Bottom = 10;
                    dlreport.Margins.Left = 50;
                    dlreport.Margins.Right = 50;
                    dlreport.PrintingSystem.ShowMarginsWarning = false;
                    dlreport.ShowPrintMarginsWarning = false;
                    dlreport.CreateDocument();
                    ReportPrintTool pt = new ReportPrintTool(dlreport);
                    pt.Print();
                    General.LogAdeverinta(dlreport);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkAdevSanatate_Click()
        {
            try
            {
                Session["TactilAdeverinte"] = "Sanatate";
                Session["NrInregAdev"] = General.GetnrInreg();
                //if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                //{
                //    Session["PrintDocument"] = "AdeverintaMedic";
                //    Session["PaginaWeb"] = "Tactil/Adeverinte";
                //    Response.Redirect("~/Reports/ImprimaTactil", false);
                //}
                //else
                {
                    Adev.Adeverinta pagAdev = new Adev.Adeverinta();
                    List<int> lstMarci = new List<int>();
                    lstMarci.Add(Convert.ToInt32(Session["User_Marca"].ToString()));
                    string sql = "SELECT * FROM F100 WHERE F10003 = " + lstMarci[0];
                    DataTable dtAng = General.IncarcaDT(sql, null);
                    byte[] fisier = pagAdev.GenerareAdeverinta(lstMarci, 12, DateTime.Now.Year, true);
                    string filePath = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + "Adev_sanatate_2020_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + Session["User_Marca"].ToString() + ".docx";
                    using (Stream file = File.OpenWrite(filePath))
                    {
                        file.Write(fisier, 0, fisier.Length);
                    }

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.Verb = "print";
                    info.FileName = filePath;
                    info.CreateNoWindow = true;
                    info.WindowStyle = ProcessWindowStyle.Hidden;

                    Process p = new Process();
                    p.StartInfo = info;
                    p.Start();

                    p.WaitForInputIdle();
                    System.Threading.Thread.Sleep(3000);

                    try
                    {
                        if (false == p.CloseMainWindow())
                            p.Kill();
                    }
                    catch (InvalidOperationException)
                    {

                    }
                    System.Threading.Thread.Sleep(3000);
                    File.Delete(filePath);
                    MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess, "");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}