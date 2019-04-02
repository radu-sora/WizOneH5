using DevExpress.Web;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }

        }





    }
}