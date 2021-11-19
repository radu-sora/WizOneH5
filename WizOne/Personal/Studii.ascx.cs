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
        protected void Page_Load(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            Studii_DataList.DataSource = table;
            if (!IsPostBack)
                Studii_DataList.DataBind();

            string[] etichete = new string[7] { "lblStudii", "lblInstit", "lblSpec", "lblDataInceputSt", "lblDataSfarsitSt", "lblDataDiploma", "lblObs"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Studii_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] butoane = new string[2] { "btnStudii", "btnStudiiIst" };
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = Studii_DataList.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }

            General.SecuritatePersonal(Studii_DataList, Convert.ToInt32(Session["UserId"].ToString()));
        }



        protected void pnlCtlStudii_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "btnStudii":
                    ModifAvans((int)Constante.Atribute.Studii);
                    break;
            }
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
                string url = "~/Avs/Cereri";
                Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString() + ";" + strRol;
                Session["MP_Avans"] = "true";
                Session["MP_Avans_Tab"] = "Studii";
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
        }

    }
}