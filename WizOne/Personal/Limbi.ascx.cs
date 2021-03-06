using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.IO;
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class Limbi : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateLimbi.DataBind();
            //grDateLimbi.AddNewRow();
            foreach (dynamic c in grDateLimbi.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateLimbi.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateLimbi.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateLimbi.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateLimbi.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateLimbi.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateLimbi);
        }

        protected void grDateLimbi_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Limbi\" WHERE \"Marca\" = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Limbi"))
            {
                dt = ds.Tables["Admin_Limbi"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Limbi";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateLimbi.KeyFieldName = "IdAuto";
            grDateLimbi.DataSource = dt;

            string sql = @"SELECT * FROM ""tblLimbi"" ORDER BY ""Denumire"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("tblLimbi", "IdAuto") + @" ORDER BY ""Denumire""";
            DataTable dtLimba = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colLimba = (grDateLimbi.Columns["IdLimba"] as GridViewDataComboBoxColumn);
            colLimba.PropertiesComboBox.DataSource = dtLimba;
            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;
        }

        protected void grDateLimbi_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Limbi"];
                if (Constante.tipBD == 1)
                {
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
                else
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Limbi");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDateLimbi_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["Admin_Limbi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Limbi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "MARCA":
                                row[x] = Session["Marca"];
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Limbi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("Admin_Limbi");
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                ds.Tables["Admin_Limbi"].Rows.Add(row);
                e.Cancel = true;
                grDateLimbi.CancelEdit();
                grDateLimbi.DataSource = ds.Tables["Admin_Limbi"];
                grDateLimbi.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateLimbi_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Limbi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Limbi"].Columns)
                {
                    if (!col.AutoIncrement && grDateLimbi.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateLimbi.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateLimbi.DataSource = ds.Tables["Admin_Limbi"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateLimbi_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Limbi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateLimbi.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateLimbi.DataSource = ds.Tables["Admin_Limbi"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}