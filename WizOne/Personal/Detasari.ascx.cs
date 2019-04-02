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

            DataList1.DataSource = table;
            DataList1.DataBind();

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


            string[] etichete = new string[6] { "lblNumeAngajator", "lblCUI", "lblNationalitate", "lblDataInceputDet", "lblDataSfarsitDet", "lblDataIncetareDet" };
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }
            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));

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
            string sqlFinal = "SELECT * FROM F112 WHERE F11203 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sqlFinal, null);
            grDateDetasari.KeyFieldName = "IdAuto";
            grDateDetasari.DataSource = dt;

            string sql = @"SELECT * FROM F733 ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F733", "F73302");
            DataTable dtDet = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colDet = (grDateDetasari.Columns["F11206"] as GridViewDataComboBoxColumn);
            colDet.PropertiesComboBox.DataSource = dtDet;

        }

        protected void btnSalveaza_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        protected void pnlCtlDet_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "txtNumeAngajator":
                    ds.Tables[0].Rows[0]["F100918"] = param[1];
                    ds.Tables[1].Rows[0]["F100918"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtCUI":
                    ds.Tables[0].Rows[0]["F100919"] = param[1];
                    ds.Tables[1].Rows[0]["F100919"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbNationalitate":
                    ds.Tables[0].Rows[0]["F100920"] = param[1];
                    ds.Tables[1].Rows[0]["F100920"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataInceputDet":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100915"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100915"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataSfarsitDet":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100916"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100916"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataIncetareDet":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100917"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100917"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
            }
        }


    }
}