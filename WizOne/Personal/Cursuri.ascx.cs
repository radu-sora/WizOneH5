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
    public partial class Cursuri : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDateCursuri.DataBind();
            //grDateCursuri.AddNewRow();
            foreach (dynamic c in grDateCursuri.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateCursuri.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateCursuri.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateCursuri.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateCursuri.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateCursuri.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

        }

        protected void grDateCursuri_DataBinding(object sender, EventArgs e)
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
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"Admin_Cursuri_VIEW\" a WHERE Marca = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Cursuri"))
            {
                dt = ds.Tables["Admin_Cursuri"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Cursuri";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCursuri.KeyFieldName = "IdAuto";
            grDateCursuri.DataSource = dt;

            string sql = @"SELECT * FROM ""Admin_TipCurs"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Admin_TipCurs", "IdAuto");
            DataTable dtTipAutorizatie = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colTipAutorizatie = (grDateCursuri.Columns["IdTipCurs"] as GridViewDataComboBoxColumn);
            colTipAutorizatie.PropertiesComboBox.DataSource = dtTipAutorizatie;

            sql = @"SELECT * FROM ""Admin_DescrCurs"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Admin_DescrCurs", "IdAuto");
            DataTable dtDescriereAutorizatie = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colDescriereAutorizatie = (grDateCursuri.Columns["IdDescriereCurs"] as GridViewDataComboBoxColumn);
            colDescriereAutorizatie.PropertiesComboBox.DataSource = dtDescriereAutorizatie;


        }

        protected void grDateCursuri_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Cursuri"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Cursuri");
            }
            catch (Exception ex)
            {

            }
        }



        protected void grDateCursuri_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["Admin_Cursuri"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Admin_Cursuri"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Cursuri"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x]= Dami.NextId("Admin_Cursuri");
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


                ds.Tables["Admin_Cursuri"].Rows.Add(row);
                e.Cancel = true;
                grDateCursuri.CancelEdit();
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];
                grDateCursuri.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCursuri_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Cursuri"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Admin_Cursuri"].Columns)
                {
                    if (!col.AutoIncrement && grDateCursuri.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateCursuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateCursuri_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateCursuri.GetRow(e.VisibleIndex) as DataRowView;

                if (values != null)
                {
                    string modif = values.Row["Modificabil"].ToString();

                    if (modif == "0")
                    {
                        if (e.ButtonType == ColumnCommandButtonType.Edit || e.ButtonType == ColumnCommandButtonType.Delete)

                            e.Visible = false;

                    }
                }
            }
        }

        protected void grDateCursuri_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Cursuri"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCursuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}