
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

namespace WizOne.Personal
{
    public partial class Componente : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            grDateComponente.DataBind();           
            foreach (dynamic c in grDateComponente.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateComponente.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateComponente.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");           
            grDateComponente.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateComponente);
        }

        protected void grDateComponente_DataBinding(object sender, EventArgs e)
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
            string tabela = "F100", id = Session["Marca"].ToString();
            if ((Session["esteNou"] ?? "false").ToString() == "true")
            {
                tabela = "F099";
                id = Session["IdSablon"].ToString();
            }

            string sql = " select F02104, " + tabela + "690 as \"Suma\" from f021 join " + tabela + " on f02104 = 4001 and " + tabela + "690 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "691 as \"Suma\" from f021 join " + tabela + " on f02104 = 4002 and " + tabela + "691 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "692 as \"Suma\" from f021 join " + tabela + " on f02104 = 4003 and " + tabela + "692 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "693 as \"Suma\" from f021 join " + tabela + " on f02104 = 4004 and " + tabela + "693 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "694 as \"Suma\" from f021 join " + tabela + " on f02104 = 4005 and " + tabela + "694 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "695 as \"Suma\" from f021 join " + tabela + " on f02104 = 4006 and " + tabela + "695 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "696 as \"Suma\" from f021 join " + tabela + " on f02104 = 4007 and " + tabela + "696 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "697 as \"Suma\" from f021 join " + tabela + " on f02104 = 4008 and " + tabela + "697 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "698 as \"Suma\" from f021 join " + tabela + " on f02104 = 4009 and " + tabela + "698 > 0 and " + tabela + "03 = " + id
                        + "union "
                        + "select F02104, " + tabela + "699 as \"Suma\" from f021 join " + tabela + " on f02104 = 4010 and " + tabela + "699 > 0 and " + tabela + "03 = " + id + " ORDER BY F02104";
            DataTable dt = new DataTable();
           
            DataSet ds = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
            if (ds != null && ds.Tables.Contains("Componente"))
            {
                dt = ds.Tables["Componente"];
            }
            else
            {
                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Componente";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F02104"] };

                if (ds == null)
                    ds = new DataSet();

                ds.Tables.Add(dt);
            }
            grDateComponente.KeyFieldName = "F02104";
            grDateComponente.DataSource = dt;

            sql = @"SELECT F02104 AS Id, CONVERT(VARCHAR, F02104) + ' - ' + F02105 as Denumire FROM F021 WHERE F02104 BETWEEN 4001 AND 4010 ORDER BY Denumire";
            if (Constante.tipBD == 2)
                sql = @"SELECT F02104 AS ""Id"", F02104 || ' - ' || F02105 as ""Denumire"" FROM F021 WHERE F02104 BETWEEN 4001 AND 4010 ORDER BY ""Denumire""";
            DataTable dtGrup = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colComp = (grDateComponente.Columns["F02104"] as GridViewDataComboBoxColumn);
            colComp.PropertiesComboBox.DataSource = dtGrup;

            Session["InformatiaCurentaPersonalCalcul"] = ds;

        }


        protected void grDateComponente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {            

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
              
                object[] rowComp = new object[dsCalcul.Tables["Componente"].Columns.Count];
                int x = 0;

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Componente"].Rows.Count; i++)
                {
                    if (dsCalcul.Tables["Componente"].Rows[i]["F02104"].ToString() == e.NewValues["F02104"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateComponente.JSProperties["cpAlertMessage"] = "Codul a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "SUMA":
                                rowComp[x] = e.NewValues[col.ColumnName];
                                ds.Tables[1].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                rowComp[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                        x++;
                    }
                    dsCalcul.Tables["Componente"].Rows.Add(rowComp);
                }
            
                e.Cancel = true;
                grDateComponente.CancelEdit();
                grDateComponente.DataSource = dsCalcul.Tables["Componente"];
                grDateComponente.KeyFieldName = "F02104";               

                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateComponente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {    
                
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                bool dublura = false;

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                DataRow rowComp = dsCalcul.Tables["Componente"].Rows.Find(keys);

                for (int i = 0; i < dsCalcul.Tables["Componente"].Rows.Count; i++)
                {
                    if (grDateComponente.EditingRowVisibleIndex != i &&  dsCalcul.Tables["Componente"].Rows[i]["F02104"].ToString() == e.NewValues["F02104"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }


                if (dublura)
                {
                    grDateComponente.JSProperties["cpAlertMessage"] = "Codul a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
                    {
                        if (col.ColumnName.ToUpper() == "SUMA")
                        {
                            col.ReadOnly = false;
                            var edc = e.NewValues[col.ColumnName];
                            rowComp[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                            ds.Tables[1].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
                        }

                    }
                }

                e.Cancel = true;
                grDateComponente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateComponente.DataSource = dsCalcul.Tables["Componente"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateComponente_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            var grid = sender as ASPxGridView;
            e.Editor.ReadOnly = false;
            //if (e.Column.FieldName == "F02104")
            //{              
            //    e.Editor.ReadOnly = !grid.IsNewRowEditing;
            //    if (!e.Editor.ReadOnly)
            //    {
            //        var cb = e.Editor as ASPxComboBox;
            //        cb.ClientSideEvents.ValueChanged = "OnValueChangedComp";
            //    }
            //}

            if (e.Column.FieldName == "Suma")
            {
                var tb = e.Editor as ASPxTextBox;
                tb.ClientSideEvents.TextChanged = "OnTextChangedComp";         
            }
        }



    }
}