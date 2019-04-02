using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.IO;
using System.Data;
using System.Web.Resources;
using System.Threading;
using System.Globalization;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;


namespace WizOne.Personal
{
    public partial class Sablon : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                string sql = "SELECT CAST(F09903 AS INT) AS \"Id\", F09908 " + op + " ' ' " + op + " F09909 AS \"Denumire\" FROM F099";
                DataTable dt = General.IncarcaDT(sql, null);
                cmbSablon.DataSource = dt;
                cmbSablon.DataBind();
                Session["InformatiaCurentaPersonal"] = null;
                Session["Marca"] = null;

                btnOK.Text = Dami.TraduCuvant(btnOK.Text);
                btnExit.Text = Dami.TraduCuvant(btnExit.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void cmbSablon_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["IdSablon"] = cmbSablon.SelectedItem.Value;
        }




    }
}