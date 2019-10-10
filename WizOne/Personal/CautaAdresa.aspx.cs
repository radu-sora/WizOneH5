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
    public partial class CautaAdresa : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                lblLocalitate.Text = Dami.TraduCuvant(lblLocalitate.Text);
                lblArtera.Text = Dami.TraduCuvant(lblArtera.Text);
                btnOK.Text = Dami.TraduCuvant(btnOK.Text);
                btnExit.Text = Dami.TraduCuvant(btnExit.Text);

                foreach (dynamic c in grDateCautaAdresa.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
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

               // List<object> lst = grDateCautaAdresa.GetSelectedFieldValues("LocSatSect", "Judet");

                if (txtArtera.Text.Length == 0 && txtLocalitate.Text.Length == 0)
                {
                    //MessageBox.Show("Nu ati introdus nici artera, nici localitatea!", MessageBox.icoWarning, "");
                    ArataMesaj("Nu ati introdus nici artera, nici localitatea!");
                    return;
                }
                

                if (txtArtera.Text.Length > 0 && txtArtera.Text.Length < 3)
                {
                    //MessageBox.Show("Introduceti minim 3 caractere la numele arterei!", MessageBox.icoWarning, "");
                    ArataMesaj("Introduceti minim 3 caractere la numele arterei!");
                    return;
                }

                if (txtLocalitate.Text.Length > 0 && txtLocalitate.Text.Length < 3)
                {
                    //MessageBox.Show("Introduceti minim 3 caractere la numele localitatii!", MessageBox.icoWarning, "");
                    ArataMesaj("Introduceti minim 3 caractere la numele localitatii!");
                    return;
                } 
                               
                grDateCautaAdresa.DataSource = GetAdresa(txtArtera.Text, txtLocalitate.Text);
                grDateCautaAdresa.KeyFieldName = "IdAuto";
                grDateCautaAdresa.DataBind();

                //ScriptManager.RegisterStartupScript(this, this.GetType(), "windowScript", "CompleteazaAdresa(" + grDateCautaAdresa.GetSelectedFieldValues("LocSatSect", "SirutaSat", "MunOraCom", "SirutaOras", "Judet", "SirutaJudet") + ")", true);



            }
            catch (Exception ex)
            {
                 MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                // General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public DataTable GetAdresa(string artera, string localitate)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            int nr = 0;

            string sql = "";
            if (Constante.tipBD == 1)
                sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('CODURIPOSTALE')";
            else
                sql = "SELECT COUNT(*) FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('CODURIPOSTALE')";

            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                if (dt.Rows[0][0].ToString().Length > 0)
                {
                    nr = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (nr > 0)
                    {
                        nr = 0;
                        sql = "SELECT COUNT(*) FROM CODURIPOSTALE";
                        dt1 = General.IncarcaDT(sql, null);
                        if (dt1 != null && dt1.Rows.Count > 0)
                            if (dt1.Rows[0][0].ToString().Length > 0)
                                nr = Convert.ToInt32(dt1.Rows[0][0].ToString());
                    }
                }

            if (artera == "" || nr == 0)
            {
                string strSql = "SELECT ROW_NUMBER() over(order by a.DENLOC, b.DENLOC, c.DENLOC) AS IdAuto, " +
                         " ' ' AS Artera, a.DENLOC AS LocSatSect, MAX(a.SIRUTA) AS SirutaSat, " +
                         " b.DENLOC AS MunOraCom, MAX(b.SIRUTA) AS SirutaOras, c.DENLOC AS Judet, MAX(c.SIRUTA) AS SirutaJudet from LOCALITATI a, LOCALITATI b, LOCALITATI c " +
                         " WHERE UPPER(a.DENLOC) LIKE UPPER('%" + localitate + "%') AND a.NIV = 3 AND a.SIRSUP = b.SIRUTA AND " +
                         " b.NIV = 2 AND b.SIRSUP = c.SIRUTA AND c.NIV = 1 GROUP BY a.DENLOC, b.DENLOC, " +
                         " c.DENLOC ORDER BY LocSatSect, MunOraCom, Judet";

                dt = General.IncarcaDT(strSql, null);
            }
            else
            {
                string cityName = "";
                if (localitate != "")
                    cityName = " AND UPPER(CITY_NAME) LIKE UPPER('%" + localitate + "%') ";

                string op = " + ";
                if (Constante.tipBD == 2) op = " || ";


                string strSql = "SELECT MAX(\"IdAuto\") AS IdAuto, STREET_I_T AS Artera, a.DENLOC AS LocSatSect, MAX(a.SIRUTA) AS SirutaSat, b.DENLOC AS MunOraCom, " +
                                "MAX(b.SIRUTA) AS SirutaOras, c.DENLOC AS Judet, MAX(c.SIRUTA) AS SirutaJudet from CODURIPOSTALE, LOCALITATI a, LOCALITATI b, LOCALITATI c WHERE UPPER(STREET_I_T) LIKE UPPER('%" + artera + "%') AND a.NIV = 3 " +
                                "AND a.SIRSUP = b.SIRUTA AND b.NIV = 2 AND b.SIRSUP = c.SIRUTA AND c.NIV = 1 AND CITY_NAME = " +
                                "a.DENLOC " + cityName + " AND (c.DENLOC = 'JUDETUL ' " + op + " REGION_NAM OR c.DENLOC = 'MUN ' " + op + " REGION_NAM OR c.DENLOC = 'MUNICIPIUL ' " + op + " REGION_NAM) " +
                                "GROUP BY  STREET_I_T, a.DENLOC, b.DENLOC, c.DENLOC " +
                                "ORDER BY Artera, LocSatSect, MunOraCom, Judet";

                dt = General.IncarcaDT(strSql, null);
            }

            return dt;
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                object lst = grDateCautaAdresa.GetSelectedFieldValues("LocSatSect", "SirutaSat", "MunOraCom", "SirutaOras", "Judet", "SirutaJudet", "Artera");
                Session["AdresaSelectata"] = lst;
               // ArataMesaj("Datele vor aparea in tabel dupa apasarea butonului Actualizeaza!");
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