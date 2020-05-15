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
    public partial class PersIntretinere : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            grDatePersIntr.DataBind();
            //grDatePersIntr.AddNewRow();
            foreach (dynamic c in grDatePersIntr.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDatePersIntr.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDatePersIntr.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDatePersIntr.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDatePersIntr.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDatePersIntr.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDatePersIntr);
        }

        protected void grDatePersIntr_DataBinding(object sender, EventArgs e)
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
            string sql = @"SELECT * FROM F711 ORDER BY F71104";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F711", "F71102") + " ORDER BY F71104";
            DataTable dtRel = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colRel = (grDatePersIntr.Columns["F11004"] as GridViewDataComboBoxColumn);
            colRel.PropertiesComboBox.DataSource = dtRel;

            sql = @"SELECT * FROM F715 ORDER BY F71504";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F715", "F71502") + " ORDER BY F71504";
            DataTable dtInv = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colInv = (grDatePersIntr.Columns["F11014"] as GridViewDataComboBoxColumn);
            colInv.PropertiesComboBox.DataSource = dtInv;

            string sqlFinal = "SELECT * FROM F110 WHERE F11003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F110"))
            {
                dt = ds.Tables["F110"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F110";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F11007"] };
                ds.Tables.Add(dt);
            }
            grDatePersIntr.KeyFieldName = "F11007";
            grDatePersIntr.DataSource = dt;

        }


        protected void grDatePersIntr_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F110"];
                if (Constante.tipBD == 1)
                {
                    if (dt.Columns["F11007"] != null)
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int max = -99;
                            try
                            {
                                max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("F11007")), 0)) + 1;
                            }
                            catch (Exception)
                            {
                            }
                            e.NewValues["F11007"] = max;
                        }
                        else
                            e.NewValues["F11007"] = 1;
                    }
                }
                else
                    e.NewValues["F11007"] = Dami.NextId("F110");

                e.NewValues["F11017"] = 1;
                e.NewValues["F11014"] = 1;           
               
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDatePersIntr_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                bool err = false;
                object[] row = new object[ds.Tables["F110"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F110"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F11001":
                                row[x] = 110;
                                break;
                            case "F11002":
                                row[x] = Convert.ToInt32(ds.Tables[1].Rows[0]["F10002"].ToString());
                                break;
                            case "F11003":
                                row[x] = Session["Marca"];
                                break;
                            case "F11007":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F110"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("F11007")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F110");
                                break;
                            case "F11012":
                                //Florin 2019.09.05 - nu se mai doreste verificare de cnp; este de ajuns doar mesajul de averitizare de pe partea de client, nu se doreste sa fie blocanta aceasta verificare (venita de la Z pe mail 2019.09.05)


                                //if (e.NewValues["F11012"] == null || e.NewValues["F11012"].ToString().Length <= 0 || !General.VerificaCNP(e.NewValues["F11012"].ToString()))
                                //{
                                //    grDatePersIntr.JSProperties["cpAlertMessage"] = "CNP invalid!";
                                //    err = true;
                                //}
                                //else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            //case "F11006":
                            //    if (e.NewValues["F11012"] != null && e.NewValues["F11012"].ToString().Length > 0 && General.VerificaCNP(e.NewValues["F11012"].ToString()))                                
                            //        row[x] = General.getDataNasterii(e.NewValues["F11012"].ToString());                                
                            //    break;
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
                e.Cancel = true;
                grDatePersIntr.CancelEdit();
                if (!err)
                {
                    ds.Tables["F110"].Rows.Add(row);  
                    grDatePersIntr.DataSource = ds.Tables["F110"];
                    grDatePersIntr.KeyFieldName = "F11007";                  
                    Session["InformatiaCurentaPersonal"] = ds;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDatePersIntr_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                bool err = false;
                DataRow row = ds.Tables["F110"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F110"].Columns)
                {
                    if (!col.AutoIncrement && grDatePersIntr.Columns[col.ColumnName] != null && grDatePersIntr.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    //Florin 2019.09.05 - nu se mai doreste verificare de cnp; este de ajuns doar mesajul de avertizare de pe partea de client, nu se doreste sa fie blocanta aceasta verificare (venita de la Z pe mail 2019.09.05)

                    //if (col.ColumnName.ToUpper() == "F11012")
                    //    if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0 || !General.VerificaCNP(e.NewValues[col.ColumnName].ToString()))
                    //    {
                    //        grDatePersIntr.JSProperties["cpAlertMessage"] = "CNP invalid!";
                    //        err = true;
                    //    }

                    if (col.ColumnName.ToUpper() == "F11001")
                        row[col.ColumnName] = 110;

                    if (col.ColumnName.ToUpper() == "F11002")
                        row[col.ColumnName] = Convert.ToInt32(ds.Tables[1].Rows[0]["F10002"].ToString());

                    //if (col.ColumnName.ToUpper() == "F11006")
                    //    if (e.NewValues["F11012"] != null && e.NewValues["F11012"].ToString().Length > 0 && General.VerificaCNP(e.NewValues["F11012"].ToString()))
                    //        row[col.ColumnName] = General.getDataNasterii(e.NewValues["F11012"].ToString());
                }

                e.Cancel = true;
                grDatePersIntr.CancelEdit();
                if (!err)
                {
                    Session["InformatiaCurentaPersonal"] = ds;
                    grDatePersIntr.DataSource = ds.Tables["F110"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePersIntr_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F110"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDatePersIntr.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDatePersIntr.DataSource = ds.Tables["F110"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePersIntr_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            var grid = sender as ASPxGridView;
            e.Editor.ReadOnly = false;
            if (e.Column.FieldName == "F11012")
            {
                var tb = e.Editor as ASPxTextBox;
                tb.ClientSideEvents.TextChanged = "OnTextChangedPI";
            }
        }

        protected void grDatePersIntr_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (!General.VerificaCNP(e.Parameters))
            {
                grDatePersIntr.JSProperties["cpAlertMessage"] = "CNP invalid!";
            }
        }

    }
}