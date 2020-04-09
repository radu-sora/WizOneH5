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
    public partial class ProgrameAlteOre : System.Web.UI.UserControl
    {



        protected void Page_Init(object sender, EventArgs e)
        {

            grDateAlteOre.DataBind();

        }

        protected void grDateAlteOre_DataBinding(object sender, EventArgs e)
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
            string sqlFinal = "SELECT * FROM \"Ptj_ProgrameAlteOre\" WHERE \"IdProgram\" = " + Session["IdProgram"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            if (ds.Tables.Contains("Ptj_ProgrameAlteOre"))
            {
                dt = ds.Tables["Ptj_ProgrameAlteOre"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ProgrameAlteOre";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateAlteOre.KeyFieldName = "IdAuto";
            grDateAlteOre.DataSource = dt;

            DataTable dtRot = General.ListaRotunjirePrgLucru();
            GridViewDataComboBoxColumn colRot = (grDateAlteOre.Columns["Rotunjire"] as GridViewDataComboBoxColumn);
            colRot.PropertiesComboBox.DataSource = dtRot;


            DataTable dtAlias = General.GetPtj_AliasFOrdonat();
            GridViewDataComboBoxColumn colAlias = (grDateAlteOre.Columns["Camp"] as GridViewDataComboBoxColumn);
            colAlias.PropertiesComboBox.DataSource = dtAlias;

            Session["InformatiaCurentaPrograme"] = ds;
        }


        protected void grDateAlteOre_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ProgrameAlteOre"];
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



        protected void grDateAlteOre_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                string errMsg = "";
                bool valid = true;
                object[] row = new object[ds.Tables["Ptj_ProgrameAlteOre"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ProgrameAlteOre"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDPROGRAM":
                                row[x] = Session["IdProgram"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ProgrameAlteOre"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "ROTUNJIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste tipul de rotunjire. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
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
                    ds.Tables["Ptj_ProgrameAlteOre"].Rows.Add(row);
                    e.Cancel = true;
                    grDateAlteOre.CancelEdit();
                    grDateAlteOre.DataSource = ds.Tables["Ptj_ProgrameAlteOre"];
                    grDateAlteOre.KeyFieldName = "IdAuto";
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

        protected void grDateAlteOre_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameAlteOre"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ProgrameAlteOre"].Columns)
                {
                    if (!col.AutoIncrement && grDateAlteOre.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "ROTUNJIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste tipul de rotunjire. ";
                            }
                            break;
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
                    grDateAlteOre.CancelEdit();
                    Session["InformatiaCurentaPrograme"] = ds;
                    grDateAlteOre.DataSource = ds.Tables["Ptj_ProgrameAlteOre"];
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAlteOre_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameAlteOre"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateAlteOre.CancelEdit();
                Session["InformatiaCurentaPrograme"] = ds;
                grDateAlteOre.DataSource = ds.Tables["Ptj_ProgrameAlteOre"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



    }
}