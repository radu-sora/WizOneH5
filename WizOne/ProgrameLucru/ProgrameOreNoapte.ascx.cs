﻿using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ProgrameLucru
{
    public partial class ProgrameOreNoapte : System.Web.UI.UserControl
    {



        protected void Page_Init(object sender, EventArgs e)
        {

            grDateOreNopate.DataBind();

        }

        protected void grDateOreNopate_DataBinding(object sender, EventArgs e)
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
            string sqlFinal = "SELECT * FROM \"Ptj_ProgrameOreNoapte\" WHERE \"IdProgram\" = " + Session["IdProgram"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            if (ds.Tables.Contains("Ptj_ProgrameOreNoapte"))
            {
                dt = ds.Tables["Ptj_ProgrameOreNoapte"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Ptj_ProgrameOreNoapte";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateOreNopate.KeyFieldName = "IdAuto";
            grDateOreNopate.DataSource = dt;

            DataTable dtRot = General.ListaRotunjirePrgLucru();
            GridViewDataComboBoxColumn colRot = (grDateOreNopate.Columns["Rotunjire"] as GridViewDataComboBoxColumn);
            colRot.PropertiesComboBox.DataSource = dtRot;


            DataTable dtAlias = General.GetPtj_AliasFOrdonat();
            GridViewDataComboBoxColumn colAlias = (grDateOreNopate.Columns["Camp"] as GridViewDataComboBoxColumn);
            colAlias.PropertiesComboBox.DataSource = dtAlias;

            Session["InformatiaCurentaPrograme"] = ds;
        }


        protected void grDateOreNopate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ProgrameOreNoapte"];
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



        protected void grDateOreNopate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                string errMsg = "";
                bool valid = true;
                object[] row = new object[ds.Tables["Ptj_ProgrameOreNoapte"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ProgrameOreNoapte"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDPROGRAM":
                                row[x] = Session["IdProgram"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ProgrameOreNoapte"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
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
                    ds.Tables["Ptj_ProgrameOreNoapte"].Rows.Add(row);
                    e.Cancel = true;
                    grDateOreNopate.CancelEdit();
                    grDateOreNopate.DataSource = ds.Tables["Ptj_ProgrameOreNoapte"];
                    grDateOreNopate.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaPrograme"] = ds;
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "Atentie !");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateOreNopate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameOreNoapte"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ProgrameOreNoapte"].Columns)
                {
                    if (!col.AutoIncrement && grDateOreNopate.Columns[col.ColumnName].Visible)
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
                    grDateOreNopate.CancelEdit();
                    Session["InformatiaCurentaPrograme"] = ds;
                    grDateOreNopate.DataSource = ds.Tables["Ptj_ProgrameOreNoapte"];
                }
                else
                    MessageBox.Show(errMsg, MessageBox.icoError, "Atentie !");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateOreNopate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;

                DataRow row = ds.Tables["Ptj_ProgrameOreNoapte"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateOreNopate.CancelEdit();
                Session["InformatiaCurentaPrograme"] = ds;
                grDateOreNopate.DataSource = ds.Tables["Ptj_ProgrameOreNoapte"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



    }
}