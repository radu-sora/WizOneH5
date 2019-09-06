using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ProgrameLucru
{
    public partial class ProgrameOreNormale : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();

            //Florin 2019.09.06
            if (table != null && table.Rows.Count > 0 && General.Nz(table.Rows[0]["ONRotunjire"], "").ToString() != "")
            {
                ASPxComboBox cmbRotunjire = DataList1.Items[0].FindControl("cmbRotunjire") as ASPxComboBox;
                cmbRotunjire.Value = Convert.ToInt32(table.Rows[0]["ONRotunjire"]);

                //Value='<%#Eval("ONRotunjire") %>'
            }
        }

        protected void pnlCtlOreNormale_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "cmbRotunjire":
                    ds.Tables[0].Rows[0]["ONRotunjire"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbONCamp":
                    ds.Tables[0].Rows[0]["ONCamp"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;

            }
        }
   


    }
}