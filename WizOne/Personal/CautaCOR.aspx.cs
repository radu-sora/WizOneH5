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
using DevExpress.Web;

namespace WizOne.Personal
{
    public partial class CautaCOR : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblCodCOR.Text = Dami.TraduCuvant(lblCodCOR.Text);
                lblDenumire.Text = Dami.TraduCuvant(lblDenumire.Text);
                btnOK.Text = Dami.TraduCuvant(btnOK.Text);
                btnExit.Text = Dami.TraduCuvant(btnExit.Text);

                foreach (dynamic c in grDateCautaCOR.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                if (IsPostBack)
                {
                    DataTable dtCOR = Session["CautaCOR_Grid"] as DataTable;
                    grDateCautaCOR.DataSource = dtCOR;
                    grDateCautaCOR.KeyFieldName = "F72202";
                    grDateCautaCOR.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCodCOR.Text.Length == 0 && txtDenumire.Text.Length == 0)
                {
                    ArataMesaj("Nu ati introdus nici codul, nici denumirea!");
                    return;
                }
                

                if (txtCodCOR.Text.Length > 0 && txtCodCOR.Text.Length < 3)
                {
                    ArataMesaj("Introduceti minim 3 cifre pentru cod COR!");
                    return;
                }

                if (txtDenumire.Text.Length > 0 && txtDenumire.Text.Length < 3)
                {
                    ArataMesaj("Introduceti minim 3 caractere la denumire!");
                    return;
                }

                DataTable dtCOR = GetCOR(txtCodCOR.Text, txtDenumire.Text);
                grDateCautaCOR.DataSource = dtCOR;
                grDateCautaCOR.KeyFieldName = "F72202";
                grDateCautaCOR.DataBind();
                Session["CautaCOR_Grid"] = dtCOR;

            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }


        public DataTable GetCOR(string codCOR, string denumire)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable table = ds.Tables[0];

            string cond = " AND F72206 = " + (table.Rows[0]["F1001082"] as string ?? "(SELECT MAX(F72206) FROM F722)") + "";  

            string sql = "SELECT F72202, F72204, F72206 FROM F722 WHERE " + (codCOR.Length > 0 ? " F72202 LIKE '%" + codCOR + "%' " + (denumire.Length > 0 ? " AND " : "") : "") + (denumire.Length > 0 ? " UPPER(F72204) LIKE UPPER('%" + denumire + "%') " : "") + cond;
            dt = General.IncarcaDT(sql, null);

            return dt;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                object lst = grDateCautaCOR.GetSelectedFieldValues("F72202", "F72204");
                Session["CodCORSelectat"] = lst;
                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void ArataMesaj(string mesaj)
        {
            form1.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            form1.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }


    }
}