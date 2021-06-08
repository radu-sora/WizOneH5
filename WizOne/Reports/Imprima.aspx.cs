using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class Imprima : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dami.AccesApp(this.Page);

            #region Traducere
            string ctlPost = Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

            btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");

            #endregion

            txtTitlu.Text = Dami.TraduCuvant("Imprima document");

            rvGeneral.ReportTypeName = "WizOne.Reports." + Session["PrintDocument"];

            if (Constante.esteTactil)
                rvGeneral.ToolbarMode = DevExpress.XtraReports.Web.DocumentViewer.DocumentViewerToolbarMode.None;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["User_Marca_Tmp"] != null)
                {
                    Session["User_Marca"] = Session["User_Marca_Tmp"];
                    Session["User_Marca_Tmp"] = null;
                }

                string tip = General.Nz(Request["tip"], "").ToString();
                if (General.Nz(Session["PaginaWeb"], "").ToString() != "") Response.Redirect("~/" + General.Nz(Session["PaginaWeb"],"").ToString().Replace(".","/") + (tip == "" ? "" : "?tip=" + tip), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}