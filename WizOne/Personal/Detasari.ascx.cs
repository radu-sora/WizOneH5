using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;

namespace WizOne.Personal
{
    public partial class Detasari : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            Detasari_DataList.DataSource = table;
            Detasari_DataList.DataBind();

            Mutare_DataList.DataSource = table;
            Mutare_DataList.DataBind();

            grDateDetasari.DataBind();
            foreach (dynamic c in grDateDetasari.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateDetasari.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateDetasari.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateDetasari.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateDetasari.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateDetasari.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (!IsPostBack)
            {//Radu 17.12.2020
                if (table.Rows[0]["F100920"] != DBNull.Value && Convert.ToInt32(table.Rows[0]["F100920"].ToString()) == 1)
                {
                    for (int i= 2; i <= 5; i++)
                    {
                        ASPxCheckBox chk = Detasari_DataList.Items[0].FindControl("chk" + i) as ASPxCheckBox;
                        chk.ClientEnabled = false;
                    }
                }
            }

            string[] etichete = new string[6] { "lblNumeAngajator", "lblCUI", "lblNationalitate", "lblDataInceputDet", "lblDataSfarsitDet", "lblDataIncetareDet"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Detasari_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }
            General.SecuritatePersonal(Detasari_DataList, Convert.ToInt32(Session["UserId"].ToString()));


            etichete = new string[9] {  "lblNumeAngExp", "lblCUIExp", "lblMutareExp", "lblTemeiLegal", "lblDataMutare",
                                                "lblNumeAngPrel", "lblCUIPrel", "lblMutarePrel", "lblDataPrel"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Mutare_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }
            General.SecuritatePersonal(Mutare_DataList, Convert.ToInt32(Session["UserId"].ToString()));
        }

        protected void grDateDetasari_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGrid()
        {
            string sqlFinal = "SELECT * FROM F112 WHERE F11203 = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sqlFinal, null);
            grDateDetasari.KeyFieldName = "IdAuto";
            grDateDetasari.DataSource = dt;

            string sql = @"SELECT * FROM F733 ORDER BY F73304";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F733", "F73302") + " ORDER BY F73304";
            DataTable dtDet = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colDet = (grDateDetasari.Columns["F11206"] as GridViewDataComboBoxColumn);
            colDet.PropertiesComboBox.DataSource = dtDet;

        }

        protected void grDateDetasari_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {

        }

        protected void grDateDetasari_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {

        }
    }

    //Checked='<%#Convert.ToInt32(DataBinder.GetPropertyValue(Container.DataItem,"F1001125"))==1%>'
}