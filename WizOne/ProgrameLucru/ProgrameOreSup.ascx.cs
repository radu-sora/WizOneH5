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
    public partial class ProgrameOreSup : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();            
        }

        protected void pnlCtlOreSup_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "cmbOSRotunjire":
                    ds.Tables[0].Rows[0]["OSRotunjire"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;      
                    break;
                case "deValMin":
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OSValMin"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deValMax":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OSValMax"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOSCamp":
                    ds.Tables[0].Rows[0]["OSCamp"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOSCampSub":
                    ds.Tables[0].Rows[0]["OSCampSub"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOSCampPeste":
                    ds.Tables[0].Rows[0]["OSCampPeste"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;

            }
        }
     


    }
}