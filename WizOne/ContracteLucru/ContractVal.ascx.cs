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
    public partial class ContractVal : System.Web.UI.UserControl
    {



        protected void Page_Init(object sender, EventArgs e)
        {

            grDateCtrVal.DataBind();

        }

        protected void grDateCtrVal_DataBinding(object sender, EventArgs e)
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
            string sqlFinal = "SELECT * FROM \"Ptj_ContracteVal\" WHERE \"IdContract\" = " + Session["IdContract"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            if (ds.Tables.Contains("Ptj_ContracteVal"))
            {
                dt = ds.Tables["Ptj_ContracteVal"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ContracteVal";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCtrVal.KeyFieldName = "IdAuto";
            grDateCtrVal.DataSource = dt;

            DataTable dtFuri = General.ListaFuri();
            GridViewDataComboBoxColumn colFuri = (grDateCtrVal.Columns["F_uri"] as GridViewDataComboBoxColumn);
            colFuri.PropertiesComboBox.DataSource = dtFuri;


            DataTable dtValuri = General.ListaVal_uri();
            GridViewDataComboBoxColumn colValuri = (grDateCtrVal.Columns["Val_uri"] as GridViewDataComboBoxColumn);
            colValuri.PropertiesComboBox.DataSource = dtValuri;

            Session["InformatiaCurentaContracte"] = ds;
        }


        protected void grDateCtrVal_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteVal"];
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
            }
            catch (Exception ex)
            {

            }
        }



        protected void grDateCtrVal_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                string errMsg = "";
                bool valid = true;
                object[] row = new object[ds.Tables["Ptj_ContracteVal"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteVal"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteVal"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "VAL_URI":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste Val. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "F_URI":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste F. ";
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
                    ds.Tables["Ptj_ContracteVal"].Rows.Add(row);
                    e.Cancel = true;
                    grDateCtrVal.CancelEdit();
                    grDateCtrVal.DataSource = ds.Tables["Ptj_ContracteVal"];
                    grDateCtrVal.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrVal_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteVal"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteVal"].Columns)
                {
                    if (!col.AutoIncrement && grDateCtrVal.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "VAL_URI":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste Val. ";
                            }
                            break;
                        case "F_URI":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste F. ";
                            }
                            break;

                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDateCtrVal.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    grDateCtrVal.DataSource = ds.Tables["Ptj_ContracteVal"];
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCtrVal_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteVal"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCtrVal.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                grDateCtrVal.DataSource = ds.Tables["Ptj_ContracteVal"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



    }
}