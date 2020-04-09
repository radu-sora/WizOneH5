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
    public partial class ProgrameValuri : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();
            
        }

        protected void pnlCtlPrgVal_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "txtVal0":
                    ds.Tables[0].Rows[0]["Val0"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtVal1":
                    ds.Tables[0].Rows[0]["Val1"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtVal2":
                    ds.Tables[0].Rows[0]["Val2"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtVal3":
                    ds.Tables[0].Rows[0]["Val3"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtVal4":
                    ds.Tables[0].Rows[0]["Val4"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmb0":
                    ds.Tables[0].Rows[0]["Val0Rot"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmb1":
                    ds.Tables[0].Rows[0]["Val1Rot"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmb2":
                    ds.Tables[0].Rows[0]["Val2Rot"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmb3":
                    ds.Tables[0].Rows[0]["Val3Rot"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmb4":
                    ds.Tables[0].Rows[0]["Val4Rot"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;

            }
        }
   


    }
}