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
    public partial class ProgramePauza : System.Web.UI.UserControl
    {



        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            table = ds.Tables[0];
            DataList1.DataSource = table;
            DataList1.DataBind();

            grDatePauza.DataBind();

        }

        protected void grDatePauza_DataBinding(object sender, EventArgs e)
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
            string sqlFinal = "SELECT * FROM \"Ptj_ProgramePauza\" WHERE \"IdProgram\" = " + Session["IdProgram"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            if (ds.Tables.Contains("Ptj_ProgramePauza"))
            {
                dt = ds.Tables["Ptj_ProgramePauza"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ProgramePauza";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDatePauza.KeyFieldName = "IdAuto";
            grDatePauza.DataSource = dt;

            Session["InformatiaCurentaPrograme"] = ds;
        }

        protected void pnlCtlPtjPauza_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "deTimpPauza":
                    string[] ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["PauzaTimp"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "deOreMinLucr":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["OreLucrateMin"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "chkPauzaDedusa":
                    ds.Tables[0].Rows[0]["PauzaDedusa"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "dePauzaScutita":
                    ora = param[1].Split(':');
                    ds.Tables[0].Rows[0]["PauzaScutita"] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(ora[0]), Convert.ToInt32(ora[1]), 0);
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
            }
        }


        protected void grDatePauza_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ProgramePauza"];
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



        protected void grDatePauza_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                string errMsg = "";
                bool valid = true;
                object[] row = new object[ds.Tables["Ptj_ProgramePauza"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ProgramePauza"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDPROGRAM":
                                row[x] = Session["IdProgram"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ProgramePauza"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
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
                    ds.Tables["Ptj_ProgramePauza"].Rows.Add(row);
                    e.Cancel = true;
                    grDatePauza.CancelEdit();
                    grDatePauza.DataSource = ds.Tables["Ptj_ProgramePauza"];
                    grDatePauza.KeyFieldName = "IdAuto";
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

        protected void grDatePauza_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgramePauza"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ProgramePauza"].Columns)
                {
                    if (!col.AutoIncrement && grDatePauza.Columns[col.ColumnName].Visible)
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
                    grDatePauza.CancelEdit();
                    Session["InformatiaCurentaPrograme"] = ds;
                    grDatePauza.DataSource = ds.Tables["Ptj_ProgramePauza"];
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDatePauza_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgramePauza"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDatePauza.CancelEdit();
                Session["InformatiaCurentaPrograme"] = ds;
                grDatePauza.DataSource = ds.Tables["Ptj_ProgramePauza"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



    }
}