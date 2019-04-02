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
    public partial class ProgrameDateGenerale : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();
 
            cmbTipPontare_SelectedIndexChanged(ds.Tables[0].Rows[0]["TipPontare"].ToString() ?? "0");
            chkFlexibil_EditValueChanged(Convert.ToInt32(ds.Tables[0].Rows[0]["Flexibil"].ToString()));
            
        }

        protected void pnlCtlDateGen_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "deDataInc":
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["DataInceput"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deDataSf":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["DataSfarsit"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtNorma":
                    ds.Tables[0].Rows[0]["Norma"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbTipPont":
                    ds.Tables[0].Rows[0]["TipPontare"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    cmbTipPontare_SelectedIndexChanged(param[1] ?? "0");
                    break;
                case "dePauza":
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["PauzaMin"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "chkNoapte":
                    ds.Tables[0].Rows[0]["DeNoapte"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "chkFlex":
                    ds.Tables[0].Rows[0]["Flexibil"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    chkFlexibil_EditValueChanged(param[1] == "true" ? 1 : 0);
                    break;
                case "deOraIn":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OraIntrare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deOraOut":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OraIesire"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;

            }
        }

        private void cmbTipPontare_SelectedIndexChanged(string param)
        {
            try
            {
                ASPxDateEdit dePauza = DataList1.Items[0].FindControl("dePauza") as ASPxDateEdit;
                if (Convert.ToInt32(param) == 5)
                {
                    dePauza.Enabled = true;
                }
                else
                {
                    dePauza.Value = null;
                    dePauza.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void chkFlexibil_EditValueChanged(int param)
        {
            try
            {               
                ASPxLabel lblOraIn = DataList1.Items[0].FindControl("lblOraIn") as ASPxLabel;
                ASPxLabel lblOraOut = DataList1.Items[0].FindControl("lblOraOut") as ASPxLabel;
                if (param == 1)
                {
                    lblOraIn.Text = "Intrare de la";
                    lblOraOut.Text = "Pana la";
                }
                else
                {
                    lblOraIn.Text = "Ora Intrare";
                    lblOraOut.Text = "Ora Iesire";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


    }
}