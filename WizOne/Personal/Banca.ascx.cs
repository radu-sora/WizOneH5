using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Banca : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            Banca_DataList.DataSource = table;
            Banca_DataList.DataBind();

            ASPxComboBox cmbBancaSal = Banca_DataList.Items[0].FindControl("cmbBancaSal") as ASPxComboBox;
            ASPxComboBox cmbSucSal = Banca_DataList.Items[0].FindControl("cmbSucSal") as ASPxComboBox;
            ASPxComboBox cmbBancaGar = Banca_DataList.Items[0].FindControl("cmbBancaGar") as ASPxComboBox;
            ASPxComboBox cmbSucGar = Banca_DataList.Items[0].FindControl("cmbSucGar") as ASPxComboBox;

            int sal = 0, gar = 0;
            if (ds.Tables[0].Rows[0]["F10018"] != DBNull.Value)
                sal = Convert.ToInt32(ds.Tables[0].Rows[0]["F10018"].ToString());
            if (ds.Tables[0].Rows[0]["F1001026"] != DBNull.Value)
                gar = Convert.ToInt32(ds.Tables[0].Rows[0]["F1001026"].ToString());

            cmbSucSal.DataSource = General.GetSucursale(Convert.ToInt32(cmbBancaSal.SelectedItem == null ? sal : cmbBancaSal.SelectedItem.Value));
            cmbSucSal.DataBindItems();

            cmbSucGar.DataSource = General.GetSucursale(Convert.ToInt32(cmbBancaGar.SelectedItem == null ? gar : cmbBancaGar.SelectedItem.Value));
            cmbSucGar.DataBindItems();

            string[] etichete = new string[12] { "lblIBANSal", "lblNrCard", "lblBancaSal", "lblSucursalaSal", "lblDataModifSal", "lblIBANGar", "lblBancaGar", "lblSucursalaGar", "lblDataModifGar",
                                                "lblIBANTichete", "lblDataIncTichete", "lblDataSfTichete"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Banca_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] butoane = new string[4] { "btnContSal", "btnContSalIst", "btnContGar", "btnContGarIst"};
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = Banca_DataList.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }
            General.SecuritatePersonal(Banca_DataList, Convert.ToInt32(Session["UserId"].ToString()));

        }

        protected void pnlCtlBanca_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "cmbBancaSal":
                    cmbBancaSal_SelectedIndexChanged(param[1]);
                    break;
                case "cmbBancaGar":
                    cmbBancaGar_SelectedIndexChanged(param[1]);
                    break;
                case "btnContSal":
                    ModifAvans((int)Constante.Atribute.BancaSalariu);
                    break;
                case "btnContGar":
                    ModifAvans((int)Constante.Atribute.BancaGarantii);
                    break;
            }
        }

        private void cmbBancaSal_SelectedIndexChanged(string val)
        {
            ASPxComboBox cmbSucSal = Banca_DataList.Items[0].FindControl("cmbSucSal") as ASPxComboBox;
            cmbSucSal.DataSource = General.GetSucursale(Convert.ToInt32(val));
            cmbSucSal.DataBindItems();
        }

        private void cmbBancaGar_SelectedIndexChanged(string val)
        {
            ASPxComboBox cmbSucGar = Banca_DataList.Items[0].FindControl("cmbSucGar") as ASPxComboBox;
            cmbSucGar.DataSource = General.GetSucursale(Convert.ToInt32(val));
            cmbSucGar.DataBindItems();
        }

        private void ModifAvans(int atribut)
        {
            string strRol = Avs.Cereri.DamiRol(Convert.ToInt32(General.Nz(Session["Marca"], -99)), atribut);
            if (strRol == "")
            {
                if (Page.IsCallback)
                {
                    //DateIdentificare_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie");
                }
                else
                    MessageBox.Show("Nu aveti drepturi pentru aceasta operatie", MessageBox.icoWarning, "Atentie");
            }
            else
            {
                string url = "~/Avs/Cereri.aspx";
                Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString() + ";" + strRol;
                Session["MP_Avans"] = "true";
                Session["MP_Avans_Tab"] = "Banca";
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
        }
    }
}