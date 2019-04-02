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
    public partial class Studii : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            //bindingSource1.DataSource = table;
            DataList1.DataSource = table;
            DataList1.DataBind();


            //ASPxComboBox cmbStudii = DataList1.Items[0].FindControl("cmbStudiiSt") as ASPxComboBox;

            //DataTable dtSt = General.IncarcaDT("SELECT F71202, F71204 FROM F712", null);
            //cmbStudii.DataSource = dtSt;
            //cmbStudii.DataBind();
            //cmbStudii.SelectedIndex = cmbStudii.Items.FindByValue(table.Rows[0]["F10050"].ToString()).Index;

            string[] etichete = new string[7] { "lblStudii", "lblInstit", "lblSpec", "lblDataInceputSt", "lblDataSfarsitSt", "lblDataDiploma", "lblObs"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] butoane = new string[2] { "btnStudii", "btnStudiiIst" };
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = DataList1.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }

            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));
        }



        protected void pnlCtlStudii_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "cmbStudiiSt":
                    ds.Tables[0].Rows[0]["F10050"] = param[1];
                    ds.Tables[1].Rows[0]["F10050"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtInstit":
                    ds.Tables[0].Rows[0]["F1001085"] = param[1];
                    ds.Tables[2].Rows[0]["F1001085"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtSpec":
                    ds.Tables[0].Rows[0]["F1001086"] = param[1];
                    ds.Tables[2].Rows[0]["F1001086"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataInceputSt":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F1001087"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F1001087"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataSfarsitSt":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F1001088"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F1001088"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataDiploma":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F1001089"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F1001089"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtObs":
                    ds.Tables[0].Rows[0]["F1001090"] = param[1];
                    ds.Tables[2].Rows[0]["F1001090"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "btnStudii":
                    ModifAvans((int)Constante.Atribute.Studii);
                    break;

            }
        }

        private void ModifAvans(int atribut)
        {
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "Studii";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
        }

    }
}