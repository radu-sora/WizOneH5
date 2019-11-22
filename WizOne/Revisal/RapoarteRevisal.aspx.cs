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

namespace WizOne.Revisal
{
    public partial class RapoarteRevisal : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

      
                #endregion





            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }





        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
 
            }
            catch (Exception ex)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnRegSal_Click(object sender, EventArgs e)
        {

        }

        protected void btnContrSal_Click(object sender, EventArgs e)
        {

        }

        protected void btnRapSal_Click(object sender, EventArgs e)
        {

        }

        protected void btnCont_Click(object sender, EventArgs e)
        {

        }

        protected void btnRen_Click(object sender, EventArgs e)
        {

        }
         



    }
}