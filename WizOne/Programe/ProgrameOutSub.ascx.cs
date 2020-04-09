using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ProgrameLucru
{
    public partial class ProgrameOutSub : System.Web.UI.UserControl
    {
        DataTable dtProgTrepte = new DataTable();

        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();

            grDateOutSub.DataBind();
        }

        protected void grDateOutSub_DataBinding(object sender, EventArgs e)
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
            string sqlFinal = "SELECT * FROM \"Ptj_ProgrameTrepte\" WHERE \"IdProgram\" = " + Session["IdProgram"].ToString() + " AND \"TipInOut\" = 'OUTSub'";
            DataTable dt = new DataTable();

            dtProgTrepte = General.IncarcaDT("SELECT * FROM \"Ptj_ProgrameTrepte\"", null);
            dtProgTrepte.Clear();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            grDateOutSub.KeyFieldName = "IdAuto";
            if (ds.Tables.Contains("Ptj_ProgrameTrepte"))
            {
                DataRow[] source = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut = 'OUTSub'");
                for (int i = 0; i < source.Count(); i++)
                    dtProgTrepte.ImportRow(source[i]);
                grDateOutSub.DataSource = dtProgTrepte;
            }
            else
            {
                dt = General.IncarcaDT("SELECT * FROM \"Ptj_ProgrameTrepte\" WHERE \"IdProgram\" = " + Session["IdProgram"].ToString(), null);
                dt.TableName = "Ptj_ProgrameTrepte";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
                DataRow[] source = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut = 'OUTSub'");
                for (int i = 0; i < source.Count(); i++)
                    dtProgTrepte.ImportRow(source[i]);
                grDateOutSub.DataSource = dtProgTrepte;
                Session["InformatiaCurentaPrograme"] = ds;
            }  
            
        }

        protected void pnlCtlOutSub_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "deDifRapOutSub":
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OUTSubDiferentaRaportare"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deDifPlataOutSub":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OUTSubDiferentaPlata"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOUTSubCampPlatit":
                    ds.Tables[0].Rows[0]["OUTSubCampPlatit"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "cmbOUTSubCampNeplatit":
                    ds.Tables[0].Rows[0]["OUTSubCampNeplatit"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
            }
        }


        protected void grDateOutSub_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ProgrameTrepte"];
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void grDateOutSub_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                string errMsg = "";
                bool valid = true;
                object[] row = new object[ds.Tables["Ptj_ProgrameTrepte"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ProgrameTrepte"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDPROGRAM":
                                row[x] = Session["IdProgram"];
                                break;
                            case "TIPINOUT":
                                row[x] = "OUTSub";
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ProgrameTrepte"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "ORAINCEPUT":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste ora inceput. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "ORASFARSIT":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste ora sfarsit. ";
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
                    ds.Tables["Ptj_ProgrameTrepte"].Rows.Add(row);
                    e.Cancel = true;
                    grDateOutSub.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut = 'OUTSub'");
                    dtProgTrepte.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtProgTrepte.ImportRow(source[i]);
                    grDateOutSub.DataSource = dtProgTrepte;
                    grDateOutSub.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaPrograme"] = ds;
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateOutSub_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameTrepte"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ProgrameTrepte"].Columns)
                {
                    if (!col.AutoIncrement && grDateOutSub.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste ora inceput. ";
                            }
                            break;
                        case "ORASFARSIT":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste ora sfarsit. ";
                            }
                            break;
                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDateOutSub.CancelEdit();
                    Session["InformatiaCurentaPrograme"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut = 'OUTSub'");
                    dtProgTrepte.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtProgTrepte.ImportRow(source[i]);
                    grDateOutSub.DataSource = dtProgTrepte;
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateOutSub_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameTrepte"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateOutSub.CancelEdit();
                Session["InformatiaCurentaPrograme"] = ds;
                DataRow[] source = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut = 'OUTSub'");
                dtProgTrepte.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtProgTrepte.ImportRow(source[i]);
                grDateOutSub.DataSource = dtProgTrepte;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

    }
}