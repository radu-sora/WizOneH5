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
    public partial class Grupuri : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            grDateGrupuri.DataBind();
            //grDateGrupuri.AddNewRow();
            foreach (dynamic c in grDateGrupuri.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateGrupuri.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateGrupuri.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateGrupuri.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateGrupuri.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateGrupuri.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
        }

        protected void grDateGrupuri_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"relGrupAngajat\" a WHERE F10003 = " + Session["Marca"].ToString();
            if (Constante.tipBD == 2)
                sqlFinal = "SELECT " + General.SelectListaCampuriOracle("relGrupAngajat2", "IdGrup") + ", CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"relGrupAngajat\" a WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            //string sqlFinal = "SELECT * FROM \"relGrupAngajat2\" WHERE F10003 = " + Session["Marca"].ToString();
            //DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("relGrupAngajat2"))
            {
                dt = ds.Tables["relGrupAngajat2"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "relGrupAngajat2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateGrupuri.KeyFieldName = "IdAuto";
            grDateGrupuri.DataSource = dt;

            string sql = @"SELECT * FROM ""tblGrupAngajati"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("tblGrupAngajati", "Id");
            DataTable dtGrup = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colGrup = (grDateGrupuri.Columns["IdGrup"] as GridViewDataComboBoxColumn);
            colGrup.PropertiesComboBox.DataSource = dtGrup;

        }

        protected void grDateGrupuri_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["relGrupAngajat2"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("relGrupAngajat2");

                e.NewValues["Modificabil"] = 1;
               
            }
            catch (Exception ex)
            {

            }
        }





        protected void grDateGrupuri_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                //if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                //{
                //    e.Cancel = true;
                //    grDateGrupuri.CancelEdit();
                //    return;
                //}

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                object[] row = new object[ds.Tables["relGrupAngajat2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["relGrupAngajat2"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Session["Marca"];
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["relGrupAngajat2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("relGrupAngajat2"); 
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

                ds.Tables["relGrupAngajat2"].Rows.Add(row);
                e.Cancel = true;
                grDateGrupuri.CancelEdit();
                grDateGrupuri.DataSource = ds.Tables["relGrupAngajat2"];
                grDateGrupuri.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateGrupuri_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    e.Cancel = true;
                    grDateGrupuri.CancelEdit();
                    return;
                }


                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["relGrupAngajat2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["relGrupAngajat2"].Columns)
                {
                    if (!col.AutoIncrement && grDateGrupuri.Columns[col.ColumnName] != null && grDateGrupuri.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateGrupuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateGrupuri.DataSource = ds.Tables["relGrupAngajat2"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateGrupuri_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    e.Cancel = true;
                    grDateGrupuri.CancelEdit();
                    return;
                }

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["relGrupAngajat2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateGrupuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateGrupuri.DataSource = ds.Tables["relGrupAngajat2"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateGrupuri_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                //string[] fields = { "Modificabil" };
                DataRowView values = grDateGrupuri.GetRow(e.VisibleIndex) as DataRowView;

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


    }
}