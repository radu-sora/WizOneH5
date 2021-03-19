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
    public partial class Main : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesTactil();     

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

                //if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                //{
                //    divPontaj.Visible = true;
                //    divCereriDiv.Visible = true;
                //    divBiletVoie.Visible = false;
                //    divCereriCO.Visible = false;
                //    divPlanifCO.Visible = false;
                //    divIstCereri.Visible = false;
                //}
                //else
                //{
                //    divPontaj.Visible = false;
                //    divCereriDiv.Visible = false;
                //}

                //Florin 2018.11.14
                //adaugam butoanele

                //Radu 08.03.2019 - adaugare coloane Pagina si IdParam2
                DataTable dt = General.IncarcaDT("SELECT * FROM \"tblConfigTactil\" WHERE COALESCE(\"Vizibil\",0)=1 AND (\"Pagina\" IS NULL OR \"Pagina\" = '') ORDER BY \"Ordine\"", null);
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

                    if (nume.ToLower() == "raport")
                    {
                        lnk.CommandArgument = General.Nz(dt.Rows[i]["IdParam"], "").ToString() + (dt.Rows[i]["IdParam2"] != null && dt.Rows[i]["IdParam2"].ToString().Length > 0 ? "_" + General.Nz(dt.Rows[i]["IdParam2"], "").ToString() : "");
                        string rapNume = General.Nz(General.ExecutaScalar("SELECT \"Description\" FROM \"DynReports\" WHERE \"DynReportId\"=@1", new object[] { dt.Rows[i]["IdParam"] }), "").ToString();
                        if (rapNume != "") nume = rapNume;
                    }


                    HtmlGenericControl divLnk = new HtmlGenericControl("div");

                    HtmlImage img = new HtmlImage();
                    img.Src = "../Fisiere/Imagini/bdgPtj.jpg";
                    img.Alt = nume;

                    HtmlGenericControl h = new HtmlGenericControl("h3");
                    h.InnerText = nume;

                    divLnk.Controls.Add(img);
                    lnk.Controls.Add(divLnk);
                    divInn.Controls.Add(lnk);
                    divInn.Controls.Add(h);
                    div.Controls.Add(divInn);
                    pnlGen.Controls.Add(div);

                    //pnlGen.InnerHtml += $@"            
                    //    <div class=""col-sm-4"">
                    //        <div class=""badgeTactil"">
                    //            <asp:LinkButton runat=""server"" ID=""LinkButton1"" CommandArgument=""{dt.Rows[i]["DynReportId"]}"" OnClick=""lnkRap_Click"">
                    //                <div>
                    //                    <img src =""../Fisiere/Imagini/bdgPtj.jpg"" alt=""{dt.Rows[i]["Description"]}"" />
                    //                </div>
                    //            </asp:LinkButton>
                    //            <h3>{dt.Rows[i]["Description"]}</h3>
                    //        </div>
                    //    </div>";
                }
               

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
                string nume = lnk.ID.Substring(3).ToLower();
                switch (nume.Split('_')[0])     //Radu 08.03.2019
                {
                    case "fluturas":
                        string msg = "";
                        if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == (int)IdClienti.Clienti.Harting)
                        {
                            DefaultTactil pag = new DefaultTactil();
                            msg = pag.PermisiuneConectare();
                        }
                        if (msg != "")                        
                            MessageBox.Show(msg, MessageBox.icoWarning, "");
                        else
                            Response.Redirect("~/Tactil/MainTactil.aspx", false);
                        break;
                    case "adeverinte":
                        Response.Redirect("~/Tactil/Adeverinte.aspx", false);
                        break;
                    case "co":
                        Session["CereriTactil"] = "CerereCO";
                        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
                        break;
                    case "biletvoie":
                        Session["CereriTactil"] = "BiletVoie";
                        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
                        break;
                    case "cop":
                        Session["CereriTactil"] = "PlanificareCO";
                        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
                        break;
                    case "absenteora":  //Radu 02.04.2019
                        Session["CereriTactil"] = "AbsenteOra";
                        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
                        break;
                    case "cereriistoric":
                        Response.Redirect("~/Tactil/ListaTactil.aspx", false);
                        break;
                    case "pontaj":
                        Response.Redirect("~/Tactil/PontajDetaliatTactil.aspx", false);
                        break;
                    case "cereridiverse":
                        Response.Redirect("~/Tactil/CereriDiverseTactil.aspx", false);
                        break;
                    case "cereridiverseistoric":
                        Response.Redirect("~/Tactil/ListaDiverseTactil.aspx", false);
                        break;
                    case "adeverintasanatate":           
                        lnkAdevSanatate_Click(); ;
                        break;
                    default:
                        {
                            if (nume.Substring(0, 6) == "raport")
                            { //Radu 08.03.2019
                                string[] lstParam = lnk.CommandArgument.Split('_');
                                int reportId = Convert.ToInt32(lstParam[0]);
                                var reportParams = null as object;
                                HtmlImage img = lnk.Controls[0].Controls[0] as HtmlImage;
                                int tip = -1;                                

                                if (lstParam.Length > 1)
                                    tip = Convert.ToInt32(lstParam[1]);                                                               

                                if (img.Alt.Contains("Fluturas"))
                                    reportParams = new
                                    {
                                        Angajat = Session["User_Marca"].ToString(),
                                        An = Dami.ValoareParam("AnLucru"),
                                        Luna = Dami.ValoareParam("LunaLucru")
                                    };
                                else if (img.Alt.Contains("Adeverinta") && tip > 0)
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

                                // New report access interface
                                if (!nume.ToLower().Contains("print"))
                                {
                                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(reportId);

                                    Wizrom.Reports.Code.ReportProxy.View(reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);
                                }
                                else
                                    Wizrom.Reports.Code.ReportProxy.Print(reportId, paramList: reportParams); // TODO: FIX03                                
                            }
                        }
                        break;
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
                //Session["NrInregAdev"] = General.GetnrInreg();
                //if (Session["TactilPrintareAdeverinte"].ToString() == "0")
                //{
                //    Session["PrintDocument"] = "AdeverintaMedic";
                //    Session["PaginaWeb"] = "Tactil/Adeverinte.aspx";
                //    Response.Redirect("~/Reports/ImprimaTactil.aspx", false);
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

                    HttpContext.Current.Session["AdevSanatate_Cale"] = filePath;  

                    Reports.AdeverintaSanatate dlreport = new Reports.AdeverintaSanatate();
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

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0")
                    Response.Redirect("../DefaultTactil.aspx", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard.aspx", false);
                else if (tip == "3")
                    Response.Redirect("../DefaultTactilExtra.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }
    }
}