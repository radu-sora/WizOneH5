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
    public partial class Activitati : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateActivitati.DataBind();
            //grDateBeneficii.AddNewRow();

            foreach (dynamic c in grDateActivitati.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateActivitati.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateActivitati.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateActivitati.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateActivitati.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateActivitati.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateActivitati);
        }

        protected void grDateActivitati_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Activitati\" WHERE \"Marca\" = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Activitati"))
            {
                dt = ds.Tables["Admin_Activitati"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Activitati";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateActivitati.KeyFieldName = "IdAuto";
            grDateActivitati.DataSource = dt;
            
            DataTable dtBen = General.GetObiecteDinArie("ArieTabActivitatiDinPersonal");
            GridViewDataComboBoxColumn colBen = (grDateActivitati.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colBen.PropertiesComboBox.DataSource = dtBen;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;

        }

        protected void grDateActivitati_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Activitati"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Activitati");

                e.NewValues["DataPrimire"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDateActivitati_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["Admin_Activitati"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Activitati"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Activitati"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("Admin_Activitati");
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

                ds.Tables["Admin_Activitati"].Rows.Add(row);
                e.Cancel = true;
                grDateActivitati.CancelEdit();
                grDateActivitati.DataSource = ds.Tables["Admin_Activitati"];
                grDateActivitati.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateActivitati_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Activitati"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Activitati"].Columns)
                {
                    if (!col.AutoIncrement && grDateActivitati.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateActivitati.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateActivitati.DataSource = ds.Tables["Admin_Activitati"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateActivitati_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Activitati"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateActivitati.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateActivitati.DataSource = ds.Tables["Admin_Activitati"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}