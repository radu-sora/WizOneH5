using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Diverse : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;               
            DataList1.DataBind();

            string[] etichete = new string[13] { "lblData", "lblNr", "lblNorma", "lblLocNastere", "lblStudii", "lblStudiiDet", "lblFunctie", "lblNivel", "lblZileCOFidel", "lblZileCOAnAnt", "lblZileCOCuvAnC", "lblVechimeComp", "lblVechimeCarteMunca" };
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));

        }

        protected void pnlCtlDiverse_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "deData":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["FX1"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["FX1"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNr":
                    ds.Tables[0].Rows[0]["F10011"] = param[1];
                    ds.Tables[1].Rows[0]["F10011"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNorma":
                    ds.Tables[0].Rows[0]["F10043"] = param[1];
                    ds.Tables[1].Rows[0]["F10043"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtLocNastere":
                    ds.Tables[0].Rows[0]["F100980"] = param[1];
                    ds.Tables[1].Rows[0]["F100980"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbStudiiDiv":
                    ds.Tables[0].Rows[0]["F10050"] = param[1];
                    ds.Tables[1].Rows[0]["F10050"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtStudiiDet":
                    ds.Tables[0].Rows[0]["F100902"] = param[1];
                    ds.Tables[1].Rows[0]["F100902"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbFunctieDiv":
                    ds.Tables[0].Rows[0]["F10071"] = param[1];
                    ds.Tables[1].Rows[0]["F10071"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbNivel":
                    ds.Tables[0].Rows[0]["F10029"] = param[1];
                    ds.Tables[1].Rows[0]["F10029"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtZileCOFidel":
                    ds.Tables[0].Rows[0]["F100640"] = param[1];
                    ds.Tables[1].Rows[0]["F100640"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtZileCOAnAnt":
                    ds.Tables[0].Rows[0]["F100641"] = param[1];
                    ds.Tables[1].Rows[0]["F100641"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtZileCOCuvAnC":
                    ds.Tables[0].Rows[0]["F100642"] = param[1];
                    ds.Tables[1].Rows[0]["F100642"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtVechimeComp":
                    ds.Tables[0].Rows[0]["F100643"] = param[1];
                    ds.Tables[1].Rows[0]["F100643"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtVechimeCarteMunca":
                    ds.Tables[0].Rows[0]["F100644"] = param[1];
                    ds.Tables[1].Rows[0]["F100644"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
            }
        }
    }
}