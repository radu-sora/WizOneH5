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
    public partial class CentreCost : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            grDateCentreCost.DataBind();
            //grDateContracte.AddNewRow();
            foreach (dynamic c in grDateCentreCost.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateCentreCost.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateCentreCost.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateCentreCost.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateCentreCost.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateCentreCost.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateCentreCost);
        }

        protected void grDateCentreCost_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100CentreCost\" a WHERE F10003 = " + Session["Marca"].ToString();
            if (Constante.tipBD == 2)
                sqlFinal = "SELECT " + General.SelectListaCampuriOracle("F100CentreCost2", "IdCentruCost") + ", CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100CentreCost\" a WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100CentreCost2"))
            {
                dt = ds.Tables["F100CentreCost2"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100CentreCost2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCentreCost.KeyFieldName = "IdAuto";
            grDateCentreCost.DataSource = dt;

            string sql = @"SELECT * FROM F062 ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F062", "F06204");
            DataTable dtCC = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colCC = (grDateCentreCost.Columns["IdCentruCost"] as GridViewDataComboBoxColumn);
            colCC.PropertiesComboBox.DataSource = dtCC;

        }

        protected void grDateCentreCost_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100CentreCost2"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100CentreCost2");
                e.NewValues["Modificabil"] = 1;
                e.NewValues["DataInceput"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDateCentreCost_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
          
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;


                if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                {
                    try
                    {
                        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                        {
                            grDateCentreCost.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                            e.Cancel = true;
                            grDateCentreCost.CancelEdit();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                for (int i = 0; i <= grDateCentreCost.VisibleRowCount - 1; i++)
                {
                    object[] obj = grDateCentreCost.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                    DateTime dtInc = Convert.ToDateTime(obj[0]);
                    DateTime dtSf = Convert.ToDateTime(obj[1]);

                    if (dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                    {
                        try
                        {
                            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                            {
                                grDateCentreCost.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                                e.Cancel = true;
                                grDateCentreCost.CancelEdit();
                                return;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                object[] row = new object[ds.Tables["F100CentreCost2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100CentreCost2"].Columns)
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
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100CentreCost2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100CentreCost2");
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

                ds.Tables["F100CentreCost2"].Rows.Add(row);
                e.Cancel = true;
                grDateCentreCost.CancelEdit();
                grDateCentreCost.DataSource = ds.Tables["F100CentreCost2"];
                grDateCentreCost.KeyFieldName = "IdAuto";             
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCentreCost_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {        

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                {
                    try
                    {
                        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                        {
                            grDateCentreCost.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                            e.Cancel = true;
                            grDateCentreCost.CancelEdit();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                for (int i = 0; i <= grDateCentreCost.VisibleRowCount - 1; i++)
                {
                    object[] obj = grDateCentreCost.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                    DateTime dtInc = Convert.ToDateTime(obj[0]);
                    DateTime dtSf = Convert.ToDateTime(obj[1]);

                    if (grDateCentreCost.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                    {
                        try
                        {
                            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                            {
                                grDateCentreCost.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                                e.Cancel = true;
                                grDateCentreCost.CancelEdit();
                                return;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100CentreCost2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100CentreCost2"].Columns)
                {
                    if (!col.AutoIncrement && grDateCentreCost.Columns[col.ColumnName] != null && grDateCentreCost.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateCentreCost.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCentreCost.DataSource = ds.Tables["F100CentreCost2"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCentreCost_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
      
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100CentreCost2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateCentreCost.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCentreCost.DataSource = ds.Tables["F100CentreCost2"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateCentreCost_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateCentreCost.GetRow(e.VisibleIndex) as DataRowView;

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