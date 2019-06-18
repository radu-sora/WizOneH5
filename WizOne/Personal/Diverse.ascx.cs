using DevExpress.Web;
using System;
using System.Data;
using System.Linq;
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
            Diverse_DataList.DataSource = table;
            Diverse_DataList.DataBind();

            string[] etichete = new string[13] { "lblData", "lblNr", "lblNorma", "lblLocNastere", "lblStudii", "lblStudiiDet", "lblFunctie", "lblNivel", "lblZileCOFidel", "lblZileCOAnAnt", "lblZileCOCuvAnC", "lblVechimeComp", "lblVechimeCarteMunca" };
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Diverse_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            General.SecuritatePersonal(Diverse_DataList, Convert.ToInt32(Session["UserId"].ToString()));
        }

    }
}