using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;

namespace WizOne.Personal
{
    public partial class Atestate : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateAtestate.DataBind();
            //grDateAtestate.AddNewRow();
            foreach (dynamic c in grDateAtestate.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateAtestate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateAtestate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateAtestate.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateAtestate.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateAtestate.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

        }

        protected void grDateAtestate_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Atestate\" WHERE \"Marca\" = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Atestate"))
            {
                dt = ds.Tables["Admin_Atestate"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Atestate";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateAtestate.KeyFieldName = "IdAuto";
            grDateAtestate.DataSource = dt;

            string sql = @"SELECT * FROM ""Admin_TipAutorizatie"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Admin_TipAutorizatie", "IdAuto");
            DataTable dtTipAutorizatie = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colTipAutorizatie = (grDateAtestate.Columns["IdTipAutorizatie"] as GridViewDataComboBoxColumn);
            colTipAutorizatie.PropertiesComboBox.DataSource = dtTipAutorizatie;

        }

        protected void grDateAtestate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Atestate"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Atestate");
            }
            catch (Exception ex)
            {

            }
        }



        protected void grDateAtestate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string result = "", valoare = "";
                object[] row = new object[ds.Tables["Admin_Atestate"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Atestate"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Atestate"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x]= Dami.NextId("Admin_Atestate");
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


                ds.Tables["Admin_Atestate"].Rows.Add(row);
                e.Cancel = true;
                grDateAtestate.CancelEdit();
                grDateAtestate.DataSource = ds.Tables["Admin_Atestate"];
                grDateAtestate.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAtestate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Atestate"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Atestate"].Columns)
                {
                    if (!col.AutoIncrement && grDateAtestate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateAtestate.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateAtestate.DataSource = ds.Tables["Admin_Atestate"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAtestate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Atestate"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateAtestate.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateAtestate.DataSource = ds.Tables["Admin_Atestate"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}