using DevExpress.XtraReports.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

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

                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                {
                    divPontaj.Visible = true;
                    divCereriDiv.Visible = true;
                    divBiletVoie.Visible = false;
                    divCereriCO.Visible = false;
                    divPlanifCO.Visible = false;
                    divIstCereri.Visible = false;
                }
                else
                {
                    divPontaj.Visible = false;
                    divCereriDiv.Visible = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkFlut_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/MainTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkBiletVoie_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CereriTactil"] = "BiletVoie";
                Response.Redirect("~/Tactil/CereriTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkAdev_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/Adeverinte.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkPlanif_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CereriTactil"] = "PlanificareCO";
                Response.Redirect("~/Tactil/CereriTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkCereri_Click(object sender, EventArgs e)
        {
            try
            {
                Session["CereriTactil"] = "CerereCO";
                Response.Redirect("~/Tactil/CereriTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkIst_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/ListaTactil.aspx", false);
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
                Response.Redirect("../DefaultTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void lnkPontaj_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/PontajDetaliatTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkCereriDiv_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Tactil/CereriDiverseTactil.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkRap_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lnk = sender as LinkButton;
                int id = Convert.ToInt32(lnk.CommandArgument);
                Session["ReportId"] = id;
                Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {

        }
    }
}