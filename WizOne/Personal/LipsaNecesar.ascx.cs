using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.IO;
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class LipsaNecesar : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateNecesar.DataBind();

            foreach (dynamic c in grDateNecesar.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
        }

        protected void grDateNecesar_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaObiecte();
                IncarcaObiectDiferenta(HttpContext.Current.Session["Marca"].ToString());
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void IncarcaObiecte()
        {
            try
            {
                DataTable dtOb = AduObiecte();
                GridViewDataComboBoxColumn colOb = (grDateNecesar.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colOb.PropertiesComboBox.DataSource = dtOb;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaObiectDiferenta(string marca)
        {
            try
            {
                DataTable dtOb = AduNecesarGrup(marca);
                GridViewDataComboBoxColumn colOb = (grDateNecesar.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colOb.PropertiesComboBox.DataSource = dtOb;

                DataTable dtObGrid = AduLipsaAngajat(marca);

                grDateNecesar.KeyFieldName = "IdAuto";
                grDateNecesar.DataSource = dtObGrid;
                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public DataTable AduObiecte()
        {
            string sql = "SELECT a.\"IdCategorie\", CAST(a.\"Id\" AS INT) AS \"IdObiect\", b.\"Denumire\" {0} ' / ' {0} a.\"Denumire\" AS \"NumeCompus\", a.\"ValoareEstimata\", a.\"Denumire\" AS \"NumeObiect\" "
                        + " FROM \"Admin_Obiecte\" a JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" ";

            if (Constante.tipBD == 1)
                sql = string.Format(sql, "+");
            else
                sql = string.Format(sql, "||");

            return General.IncarcaDT(sql, null);
        }

        public DataTable AduNecesarGrup(string marca)
        {
            //Florin 2019.12.04 - s-a modificat Admin_AngajatGrup in relGrupAngajat
            string sql = "SELECT a.\"IdCategorie\", CAST(a.\"Id\" AS INT) AS \"IdObiect\", b.\"Denumire\" {0} ' / ' {0} a.\"Denumire\" AS \"NumeCompus\", a.\"ValoareEstimata\", a.\"Denumire\" AS \"NumeObiect\""
                        + " FROM\"relGrupAngajat\" c "
                        + " JOIN \"Admin_NecesarGrup\" d ON c.\"IdGrup\" = d.\"IdGrup\" "
                        + " JOIN \"Admin_Obiecte\" a ON d.\"IdObiect\" = a.\"Id\" "
                        + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
                        + " WHERE c.F10003 = {1} "
                        + " UNION "
                        + " SELECT 10000 + a.\"IdCategory\", 10000 + CAST(a.\"IdCategory\" AS INT) AS \"IdObiect\", a.\"NameCategory\" {0} ' / ' {0} 'Document' AS \"NumeCompus\", "
                        + " null as \"ValoareEstimata\", 'Document' AS \"NumeObiect\"   FROM \"CategoriiAtasamente\" a where a.\"Obligatoriu\" = 1";

            if (Constante.tipBD == 1)
                sql = string.Format(sql, "+", marca);
            else
                sql = string.Format(sql, "||", marca);


            return General.IncarcaDT(sql, null);

        }


        public DataTable AduLipsaAngajat(string marca)
        {
            //string totalAng = " SELECT a.\"Id\" FROM \"Atasamente\" c "
            //                + " JOIN \"Admin_Obiecte\" a on c.\"IdAuto\" = a.\"Id\" "
            //                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
            //                + " WHERE c.\"IdEmpl\" = {0} "
            //                + " UNION "
            //                + " SELECT a.\"Id\" FROM \"Admin_Echipamente\" c "
            //                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
            //                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
            //                + " WHERE c.\"Marca\" = {0} "
            //                + " UNION "
            //                + " SELECT a.\"Id\" FROM \"Admin_Beneficii\" c "
            //                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
            //                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
            //                + " WHERE c.\"Marca\" = {0} "
            //                + " UNION "
            //                + " SELECT a.\"Id\" FROM \"Admin_Activitati\" c "
            //                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
            //                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
            //                + " WHERE c.\"Marca\" = {0} ";
            string totalAng = " SELECT a.\"Id\" FROM \"Admin_Echipamente\" c "
                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
                + " WHERE c.\"Marca\" = {0} "
                + " UNION "
                + " SELECT a.\"Id\" FROM \"Admin_Beneficii\" c "
                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
                + " WHERE c.\"Marca\" = {0} "
                + " UNION "
                + " SELECT a.\"Id\" FROM \"Admin_Activitati\" c "
                + " JOIN \"Admin_Obiecte\" a on c.\"IdObiect\" = a.\"Id\" "
                + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
                + " WHERE c.\"Marca\" = {0} ";

            totalAng = string.Format(totalAng, marca);

            //Florin 2019.12.04 - s-a modificat Admin_AngajatGrup in relGrupAngajat
            string sql = "SELECT a.\"IdCategorie\", CAST(a.\"Id\" AS INT) AS \"IdObiect\", b.\"Denumire\" {0} ' / ' {0} a.\"Denumire\" AS \"NumeCompus\", a.\"ValoareEstimata\", a.\"Denumire\" AS \"NumeObiect\""
                        + " FROM\"relGrupAngajat\" c "
                        + " JOIN \"Admin_NecesarGrup\" d ON c.\"IdGrup\" = d.\"IdGrup\" "
                        + " JOIN \"Admin_Obiecte\" a ON d.\"IdObiect\" = a.\"Id\" "
                        + " JOIN \"Admin_Categorii\" b ON a.\"IdCategorie\" = b.\"Id\" "
                        + " WHERE c.F10003 = {1} AND d.\"IdObiect\" NOT IN ({2}) "

                        + " UNION "

                        + " SELECT 10000 + a.\"IdCategory\", 10000 + CAST(a.\"IdCategory\" AS INT) AS \"IdObiect\", a.\"NameCategory\" {0} ' / ' {0} 'Document' AS \"NumeCompus\", null as \"ValoareEstimata\", 'Document' AS \"NumeObiect\" "
                        + " FROM \"CategoriiAtasamente\" a "
                        + " where a.\"Obligatoriu\" = 1 and a.\"IdCategory\" not in ( "
                        + " select b.\"IdCategory\" from \"Atasamente\" b where \"IdEmpl\" = {1})";

            if (Constante.tipBD == 1)
                sql = string.Format(sql, "+", marca, totalAng);
            else
                sql = string.Format(sql, "||", marca, totalAng);


            return General.IncarcaDT(sql, null);

        }


    }
}