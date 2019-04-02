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