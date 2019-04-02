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
    public partial class ProgrameOutPeste : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();            
        }

        protected void pnlCtlOutPeste_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "deDifRapOutPeste":
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OUTPesteDiferentaRaportare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deMinPlataOutPeste":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OUTPesteMinPlata"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deMaxPlatitOutPeste":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OUTPesteMaxPlata"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOUTPesteCampPlatit":
                    ds.Tables[0].Rows[0]["OUTPesteCampPlatit"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOUTPesteCampNeplatit":
                    ds.Tables[0].Rows[0]["OUTPesteCampNeplatit"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;


            }
        }
     


    }
}