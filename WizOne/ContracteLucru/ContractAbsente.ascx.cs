using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ContracteLucru
{
    public partial class ContractAbsente : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {       
            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();

            //Florin 2019.09.06
            if (table != null && table.Rows.Count > 0)
            {
                if (General.Nz(table.Rows[0]["Afisare"], "").ToString() != "")
                {
                    ASPxComboBox cmbAfis = DataList1.Items[0].FindControl("cmbAfis") as ASPxComboBox;
                    cmbAfis.Value = Convert.ToInt32(table.Rows[0]["Afisare"]);
                }

                if (General.Nz(table.Rows[0]["TipRaportareOreNoapte"], "").ToString() != "")
                {
                    ASPxComboBox cmbRap = DataList1.Items[0].FindControl("cmbRap") as ASPxComboBox;
                    cmbRap.Value = Convert.ToInt32(table.Rows[0]["TipRaportareOreNoapte"]);
                }

                //  Value='<%#Eval("Afisare") %>'
                //  Value='<%#Eval("TipRaportareOreNoapte") %>'
            }

            grDateCtrAbs.DataBind();

        }

        protected void grDateCtrAbs_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGrid()
        {
            string sqlFinal = "SELECT * FROM \"Ptj_ContracteAbsente\" WHERE \"IdContract\" = " + Session["IdContract"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            if (ds.Tables.Contains("Ptj_ContracteAbsente"))
            {
                dt = ds.Tables["Ptj_ContracteAbsente"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ContracteAbsente";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCtrAbs.KeyFieldName = "IdAuto";
            grDateCtrAbs.DataSource = dt;

            DataTable dtAbs = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsente"" ", null);
            GridViewDataComboBoxColumn colAbs = (grDateCtrAbs.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
            colAbs.PropertiesComboBox.DataSource = dtAbs;

            Session["InformatiaCurentaContracte"] = ds;
        }

        protected void pnlCtlContractAbsente_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;            
            switch (param[0])
            {
                case "txtOraIn":                    
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OraInSchimbare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0); 
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "txtOraOut":                   
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OraOutSchimbare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "chkOreSup":
                    ds.Tables[0].Rows[0]["OreSup"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "cmbAfis":
                    ds.Tables[0].Rows[0]["Afisare"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "chkScadeVal0":
                    ds.Tables[0].Rows[0]["ScadeVal0"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "chkScadeFara":
                    ds.Tables[0].Rows[0]["ScadeFaraDepasireNorma"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "cmbRap":
                    ds.Tables[0].Rows[0]["TipRaportareOreNoapte"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "chkPontareAuto":
                    ds.Tables[0].Rows[0]["PontareAutomata"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "txtInInit":
                    {
                        string[] oraIni = param[1].Split(':');
                        ds.Tables[0].Rows[0]["OraInInitializare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIni[0]), Convert.ToInt32(oraIni[1]), 0);
                        Session["InformatiaCurentaContracte"] = ds;
                    }
                    break;
                case "txtOutInit":
                    {
                        string[] oraIni = param[1].Split(':');
                        ds.Tables[0].Rows[0]["OraOutInitializare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIni[0]), Convert.ToInt32(oraIni[1]), 0);
                        Session["InformatiaCurentaContracte"] = ds;
                    }
                    break;
            }
        }

        protected void grDateCtrAbs_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteAbsente"];
                if (dt.Columns["IdAuto"] != null)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                        e.NewValues["IdAuto"] = max;
                    }
                    else
                        e.NewValues["IdAuto"] = 1;
                }

                e.NewValues["ZL"] = 0;
                e.NewValues["S"] = 0;
                e.NewValues["D"] = 0;
                e.NewValues["SL"] = 0;
                e.NewValues["InPontajAnual"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrAbs_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteAbsente"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteAbsente"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "IDAUTO":
                                //Florin 2019.11.22
                                //row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteAbsente"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteAbsente"].Compute("max([IdAuto])", string.Empty), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "IDABSENTA":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste tipul de absenta. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                if (valid)
                {
                    ds.Tables["Ptj_ContracteAbsente"].Rows.Add(row);
                    e.Cancel = true;
                    grDateCtrAbs.CancelEdit();
                    grDateCtrAbs.DataSource = ds.Tables["Ptj_ContracteAbsente"];
                    grDateCtrAbs.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrAbs_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteAbsente"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteAbsente"].Columns)
                {
                    if (!col.AutoIncrement && grDateCtrAbs.Columns[col.ColumnName] != null && grDateCtrAbs.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDABSENTA":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste tipul de absenta. ";
                            }
                            break;

                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDateCtrAbs.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    grDateCtrAbs.DataSource = ds.Tables["Ptj_ContracteAbsente"];
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrAbs_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteAbsente"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCtrAbs.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                grDateCtrAbs.DataSource = ds.Tables["Ptj_ContracteAbsente"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void ArataMesaj(string mesaj)
        {
            pnlCtlContractAbsente.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            pnlCtlContractAbsente.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }

    }
}