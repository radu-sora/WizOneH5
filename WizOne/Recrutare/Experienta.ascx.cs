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

namespace WizOne.Recrutare
{
    public partial class Experienta : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (dynamic c in grDate.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDate.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDate.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDate.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            IncarcaGrid();
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                if (!ds.Tables.Contains("Rec_Experienta"))
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Rec_Experienta"" WHERE ""Id""=@1", new object[] { Session["Sablon_CheiePrimara"] });
                    dt.TableName = "Rec_Experienta";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                else
                    dt = ds.Tables["Rec_Experienta"];

                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;

                DataTable dtObi = General.GetObiecteDinArie("ArieTabExperientaDinPersonal");
                GridViewDataComboBoxColumn colObi = (grDate.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colObi.PropertiesComboBox.DataSource = dtObi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["Id"] = Session["Sablon_CheiePrimara"];

                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Rec_Experienta"];
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurenta"] as DataSet;

                object[] row = new object[ds.Tables["Rec_Experienta"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Rec_Experienta"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "ID":
                                row[x] = Session["Sablon_CheiePrimara"];
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Rec_Experienta"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
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

                ds.Tables["Rec_Experienta"].Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = ds.Tables["Rec_Experienta"];
                grDate.KeyFieldName = "IdAuto";
                Session["InformatiaCurenta"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurenta"] as DataSet;

                DataRow row = ds.Tables["Rec_Experienta"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Rec_Experienta"].Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = ds;
                grDate.DataSource = ds.Tables["Rec_Experienta"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurenta"] as DataSet;

                DataRow row = ds.Tables["Rec_Experienta"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = ds;
                grDate.DataSource = ds.Tables["Rec_Experienta"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //NOP
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}