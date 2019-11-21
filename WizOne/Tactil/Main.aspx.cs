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
                DataTable dt = General.IncarcaDT("SELECT * FROM \"tblConfigTactil\" WHERE COALESCE(\"Vizibil\",0)=1 AND (\"Pagina\" IS NULL OR \"Pagina\" = '')", null);
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
                    default:
                        {
                            if (nume.Substring(0, 6) == "raport")
                            {//Radu 08.03.2019
                                string[] lstParam = lnk.CommandArgument.Split('_');
                                int id = Convert.ToInt32(lstParam[0]);
                                int tip = -1;
                                if (lstParam.Length > 1)
                                    tip = Convert.ToInt32(lstParam[1]);
                                Session["ReportId"] = id;
                                string param = "";
                                HtmlImage img = lnk.Controls[0].Controls[0] as HtmlImage;
                                if (img.Alt.Contains("Fluturas"))
                                    param = "&An=" + Dami.ValoareParam("AnLucru") + "&Luna=" + Dami.ValoareParam("LunaLucru");
                                if (img.Alt.Contains("Adeverinta") && tip > 0)
                                    param = "&TipAdeverinta=" + tip;

                                //Florin 2019.10.17
                                //Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?Angajat=" + Session["User_Marca"].ToString() + param, false);
                                if (nume.ToLower().Contains("print"))
                                    Session["PrintareAutomata"] = 1;
                                Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?q=" + General.URLEncode("Angajat=" + Session["User_Marca"].ToString() + param), false);
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

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("../DefaultTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }



        #region OLD

        //protected void lnkFlut_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("~/Tactil/MainTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkBiletVoie_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Session["CereriTactil"] = "BiletVoie";
        //        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkAdev_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("~/Tactil/Adeverinte.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkPlanif_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Session["CereriTactil"] = "PlanificareCO";
        //        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkCereri_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Session["CereriTactil"] = "CerereCO";
        //        Response.Redirect("~/Tactil/CereriTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkIst_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("~/Tactil/ListaTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkPontaj_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("~/Tactil/PontajDetaliatTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkCereriDiv_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Response.Redirect("~/Tactil/CereriDiverseTactil.aspx", false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void lnkRap_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LinkButton lnk = sender as LinkButton;
        //        int id = Convert.ToInt32(lnk.CommandArgument);
        //        Session["ReportId"] = id;
        //        Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        #endregion

        protected void lnkGigi_Click(object sender, EventArgs e)
        {
            string ert = "";
        }
    }
}