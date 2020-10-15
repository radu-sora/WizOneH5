using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using WizOne.Module;
using WizOne.Module.Saml;

namespace WizOne
{
    public partial class LogOut : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txtTitlu.InnerText = Dami.TraduCuvant("Ati fost delogat cu succes !");
                lnkBack.InnerText = Dami.TraduCuvant("Inapoi in site");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}