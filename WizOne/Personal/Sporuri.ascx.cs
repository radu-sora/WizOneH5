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
    public partial class Sporuri : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            grDateSporuri1.DataBind();           
            foreach (dynamic c in grDateSporuri1.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateSporuri1.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateSporuri1.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateSporuri1);

            grDateSporuri2.DataBind();
            foreach (dynamic c in grDateSporuri2.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateSporuri2.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateSporuri2.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateSporuri2);
        }

        protected void grDateSporuri1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid1();
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void grDateSporuri2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid2();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void IncarcaGrid1()
        {

            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Sporuri1"))
            {
                dt = dsCalcul.Tables["Sporuri1"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Spor", typeof(string));
                dt.Columns.Add("Tarif", typeof(string));
                dt.Columns.Add("F02504", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                dt.Columns.Add("Id", typeof(int));
                               
                string sql = "";
                string cmp = "ISNULL";
                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                if (Constante.tipBD == 2)
                    cmp = "NVL";     

                for (int i = 0; i <= 9; i++)
                {
                    string val = "0";
                    DataTable dtTemp = General.IncarcaDT("select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + ds.Tables[0].Rows[0]["F10065" + i].ToString(), null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        val = dtTemp.Rows[0][0].ToString();

                    string tabela = "F100";
                    if ((Session["esteNou"] ?? "false").ToString() == "true")                  
                        tabela = "F099";                    

                    //Florin 2019.06.20
                    //am inlocuit TOP 1 cu ROWNUM
                    if (Constante.tipBD == 1)
                        sql += "select " + (i + 1).ToString() + " as \"Id\", " + tabela + "65" + i + " as F02504, CASE WHEN " + tabela + "65" + i + " = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT TOP 1 F02505 FROM F025 WHERE F02504 = " + tabela + "65" + i + ") END as \"Spor\", "
                            + " case when " + tabela + "65" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when " + tabela + "65" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select top 1 f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = " + tabela + "65" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + "), '---')  END as \"Tarif\" "                            
                            + ((Session["esteNou"] ?? "false").ToString() == "true" ? " from f099 where f09903 = " + Session["IdSablon"].ToString() : " from f100 where f10003 = " + Session["Marca"].ToString());
                    else
                        sql += "select " + (i + 1).ToString() + " as \"Id\", " + tabela + "65" + i + " as F02504, CASE WHEN " + tabela + "65" + i + " = 0 THEN 'Spor " + (i + 1).ToString() + "' ELSE (SELECT F02505 FROM F025 WHERE F02504 = " + tabela + "65" + i + " AND ROWNUM <= 1) END as \"Spor\", "
                            + " case when " + tabela + "65" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when " + tabela + "65" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = " + tabela + "65" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " AND ROWNUM <= 1), '---')  END as \"Tarif\" "
                            + ((Session["esteNou"] ?? "false").ToString() == "true" ? " from f099 where f09903 = " + Session["IdSablon"].ToString() : " from f100 where f10003 = " + Session["Marca"].ToString());


                    if (i < 9)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Sporuri1";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dsCalcul.Tables.Add(dt);
            }
            grDateSporuri1.KeyFieldName = "Id";
            grDateSporuri1.DataSource = dt;   

            Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;

        }

        private void IncarcaGrid2()
        {
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Sporuri2"))
            {
                dt = dsCalcul.Tables["Sporuri2"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Spor", typeof(string));
                dt.Columns.Add("Tarif", typeof(string));
                dt.Columns.Add("F02504", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                dt.Columns.Add("Id", typeof(int));

                string sql = "";
                string cmp = "ISNULL";
                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                if (Constante.tipBD == 2)
                    cmp = "NVL";
                for (int i = 0; i <= 9; i++)
                {
                    string val = "0";
                    DataTable dtTemp = General.IncarcaDT("select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + ds.Tables[0].Rows[0]["F10066" + i].ToString(), null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        val = dtTemp.Rows[0][0].ToString();

                    string tabela = "F100";
                    if ((Session["esteNou"] ?? "false").ToString() == "true")
                        tabela = "F099";

                    //Florin 2019.06.20
                    //am inlocuit TOP 1 cu ROWNUM
                    if (Constante.tipBD == 1)
                        sql += "select " + (i + 11).ToString() + " as \"Id\", " + tabela + "66" + i + " as F02504, CASE WHEN " + tabela + "66" + i + " = 0 THEN 'Spor " + (i + 11).ToString() + "' ELSE (SELECT TOP 1 F02505 FROM F025 WHERE F02504 = " + tabela + "66" + i + ") END as \"Spor\", "
                            + " case when " + tabela + "66" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when " + tabela + "66" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select top 1 f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = " + tabela + "66" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + "), '---')  END as \"Tarif\" "
                            + ((Session["esteNou"] ?? "false").ToString() == "true" ? " from f099 where f09903 = " + Session["IdSablon"].ToString() : " from f100 where f10003 = " + Session["Marca"].ToString());
                    else
                        sql += "select " + (i + 11).ToString() + " as \"Id\", " + tabela + "66" + i + " as F02504, CASE WHEN " + tabela + "66" + i + " = 0 THEN 'Spor " + (i + 11).ToString() + "' ELSE (SELECT F02505 FROM F025 WHERE F02504 = " + tabela + "66" + i + " AND ROWNUM <= 1) END as \"Spor\", "
                            + " case when " + tabela + "66" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end as F01105, "
                            + " CASE WHEN(case when " + tabela + "66" + i + " = 0 then 0 else " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " end ) = 0 THEN '---' ELSE "
                            + cmp + "((select f01107 from f025 "
                            + " left join f021 on f02510 = f02104 "
                            + " left join f011 on f02106 = f01104 "
                            + " where f02504 = " + tabela + "66" + i + " and f01105 = " + (val == "0" ? "0" : sir[Convert.ToInt32(val) - 1].ToString()) + " AND ROWNUM <= 1), '---')  END as \"Tarif\" "
                            + ((Session["esteNou"] ?? "false").ToString() == "true" ? " from f099 where f09903 = " + Session["IdSablon"].ToString() : " from f100 where f10003 = " + Session["Marca"].ToString());


                    if (i < 9)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.TableName = "Sporuri2";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dsCalcul.Tables.Add(dt);
            }
            grDateSporuri2.KeyFieldName = "Id";
            grDateSporuri2.DataSource = dt;

            Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
        }

        protected void grDateSporuri1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["Spor"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster1");
                e.NewValues["F02504"] = cb1.Value;
                e.NewValues["Spor"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["Tarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild1");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["Tarif"] = cb2.Text;

                if (e.NewValues["F02504"].ToString() == "0")
                {
                    e.NewValues["Spor"] = "Spor " + (grDateSporuri1.EditingRowVisibleIndex + 1).ToString();
                    e.NewValues["Tarif"] = "---";
                }

                if (e.NewValues["Spor"] == null || e.NewValues["Spor"].ToString().Length < 0)
                    return;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["Sporuri1"].Rows.Find(keys);
                int poz = 0, val = 0;

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Sporuri1"].Rows.Count; i++)
                {
                    if (grDateSporuri1.EditingRowVisibleIndex != i && e.NewValues["F02504"].ToString() != "0" && dsCalcul.Tables["Sporuri1"].Rows[i]["F02504"].ToString() == e.NewValues["F02504"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateSporuri1.JSProperties["cpAlertMessage"] = "Acest spor a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Sporuri1"].Columns)
                    {
                        if (col.ColumnName != "Id")
                        {
                            col.ReadOnly = false;
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                            if (col.ColumnName == "F02504")
                            {
                                DataTable dt = General.IncarcaDT("  select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + e.NewValues[col.ColumnName], null);
                                poz = Convert.ToInt32(dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == DBNull.Value ? "0" : dt.Rows[0][0].ToString());
                            }
                            if (col.ColumnName == "F01105")
                                val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        }
                    }
                }

                e.Cancel = true;
                grDateSporuri1.CancelEdit();

                if (!dublura)
                {
                    ds.Tables[0].Rows[0]["F10065" + (Convert.ToInt32(keys[0]) - 1).ToString()] = e.NewValues["F02504"];
                    ds.Tables[1].Rows[0]["F10065" + (Convert.ToInt32(keys[0]) - 1).ToString()] = e.NewValues["F02504"];


                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;
                    ds.Tables[1].Rows[0]["F10067"] = sirNou;
                }

                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateSporuri1.DataSource = dsCalcul.Tables["Sporuri1"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateSporuri2_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {


                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["Spor"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster2");
                e.NewValues["F02504"] = cb1.Value;
                e.NewValues["Spor"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["Tarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild2");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["Tarif"] = cb2.Text;

                if (e.NewValues["F02504"].ToString() == "0")
                {
                    e.NewValues["Spor"] = "Spor " + (grDateSporuri2.EditingRowVisibleIndex + 11).ToString();
                    e.NewValues["Tarif"] = "---";
                }

                if (e.NewValues["Spor"] == null || e.NewValues["Spor"].ToString().Length < 0)
                    return;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
                int poz = 0, val = 0;
                DataRow row = dsCalcul.Tables["Sporuri2"].Rows.Find(keys);

                bool dublura = false;
                for (int i = 0; i < dsCalcul.Tables["Sporuri2"].Rows.Count; i++)
                {
                    if (grDateSporuri2.EditingRowVisibleIndex != i && e.NewValues["F02504"].ToString() != "0" && dsCalcul.Tables["Sporuri2"].Rows[i]["F02504"].ToString() == e.NewValues["F02504"].ToString())
                    {
                        dublura = true;
                        break;
                    }
                }

                if (dublura)
                {
                    grDateSporuri2.JSProperties["cpAlertMessage"] = "Acest spor a mai fost deja atribuit acestui angajat!";
                }
                else
                {
                    foreach (DataColumn col in dsCalcul.Tables["Sporuri2"].Columns)
                    {
                        if (col.ColumnName != "Id")
                        {
                            col.ReadOnly = false;
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                            if (col.ColumnName == "F02504")
                            {
                                DataTable dt = General.IncarcaDT("  select distinct f01104 from f025 left join f021 on f02510 = f02104 left join f011 on f02106 = f01104 where  f02504 = " + e.NewValues[col.ColumnName], null);
                                poz = Convert.ToInt32(dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == DBNull.Value ? "0" : dt.Rows[0][0].ToString());
                            }
                            if (col.ColumnName == "F01105")
                                val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                        }
                    }
                }
                e.Cancel = true;
                grDateSporuri2.CancelEdit();

                if (!dublura)
                {
                    ds.Tables[0].Rows[0]["F10066" + (Convert.ToInt32(keys[0]) - 11).ToString()] = e.NewValues["F02504"];
                    ds.Tables[1].Rows[0]["F10066" + (Convert.ToInt32(keys[0]) - 11).ToString()] = e.NewValues["F02504"];


                    string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                    string sirNou = "";
                    for (int i = 0; i < sir.Length; i++)
                        if (i == poz - 1)
                            sirNou += val.ToString();
                        else
                            sirNou += sir[i];

                    ds.Tables[0].Rows[0]["F10067"] = sirNou;
                    ds.Tables[1].Rows[0]["F10067"] = sirNou;
                }

                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateSporuri2.DataSource = dsCalcul.Tables["Sporuri2"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void cmbMaster1_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged1(s, e, {0}); }}", templateContainer.VisibleIndex);

            ObjectDataSource cmbMasterDataSource = cmbParent.NamingContainer.FindControl("adsMaster1") as ObjectDataSource;

            cmbMasterDataSource.SelectParameters.Clear();
            cmbMasterDataSource.SelectParameters.Add("param", "1");
            cmbMasterDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbParent.DataBindItems();

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                object[] obj = grDateSporuri1.GetRowValues(grDateSporuri1.FocusedRowIndex, new string[] { "Spor", "Tarif", "F02504" }) as object[];
                cmbParent.Value = Convert.ToInt32(obj[2].ToString());
                Session["Sporuri_cmbMaster1"] = Convert.ToInt32(obj[2].ToString());
            }


        }

        protected void cmbChild1_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild1_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild1_new", templateContainer.VisibleIndex);
            else
            {
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild1") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Sporuri_cmbMaster1"].ToString());
                cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }

            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild1_Callback);
    
        }

        protected void cmbChild1_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild1") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbChild.DataBindItems();
        }



        protected void cmbMaster2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged2(s, e, {0}); }}", templateContainer.VisibleIndex);

            ObjectDataSource cmbMasterDataSource = cmbParent.NamingContainer.FindControl("adsMaster2") as ObjectDataSource;

            cmbMasterDataSource.SelectParameters.Clear();
            cmbMasterDataSource.SelectParameters.Add("param", "0");
            cmbMasterDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbParent.DataBindItems();

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                object[] obj = grDateSporuri2.GetRowValues(grDateSporuri2.FocusedRowIndex, new string[] { "Spor", "Tarif", "F02504" }) as object[];
                cmbParent.Value = Convert.ToInt32(obj[2].ToString());
                Session["Sporuri_cmbMaster2"] = Convert.ToInt32(obj[2].ToString());
            }
        }

        protected void cmbChild2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild2_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild2_new", templateContainer.VisibleIndex);
            else
            {
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild2") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Sporuri_cmbMaster2"].ToString());
                cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }

            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild2_Callback);

        }

        protected void cmbChild2_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("adsChild2") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChildDataSource.SelectParameters.Add("data", DateTime.Now.ToShortDateString());
            cmbChild.DataBindItems();
        }

 
    }
}