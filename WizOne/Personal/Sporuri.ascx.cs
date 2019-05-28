﻿using System;
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
        }

        protected void grDateSporuri1_DataBinding(object sender, EventArgs e)
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

        protected void grDateSporuri2_DataBinding(object sender, EventArgs e)
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

            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
            if (dsCalcul != null && dsCalcul.Tables.Contains("Tarife"))
            {
                dt = dsCalcul.Tables["Tarife"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("DenCateg", typeof(string));
                dt.Columns.Add("DenTarif", typeof(string));
                dt.Columns.Add("F01104", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F01104"], dt.Columns["F01105"] };

                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sqlFinal = "";
                string sql = "SELECT F01104, F01105, (SELECT TOP 1 b.F01107 FROM F011 b WHERE b.F01104 = a.F01104) AS \"DenCateg\", " 
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ", cond = "";

                if (Constante.tipBD == 2)
                    sql = "SELECT F01104, F01105, (SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 WHERE ROWNUM = 1) AS \"DenCateg\", "
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ";

                for (int i = 0; i < sir.Length; i++)
                    if (sir[i] != '0')
                    {
                        cond = "WHERE (a.F01104 = " + (i + 1).ToString() + " AND a.F01105 = " + sir[i] + ") ";
                        sqlFinal += (sqlFinal.Length <= 0 ? "" : " UNION ") + sql + cond;
                    }

                if (sqlFinal.Length > 0)
                    dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Tarife";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateSporuri1.KeyFieldName = "F01104; F01105";
            grDateSporuri1.DataSource = dt;   

            Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;

        }


        protected void grDateSporuri1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                int index = ((ASPxGridView)sender).EditingRowVisibleIndex;
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["DenCateg"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster");
                e.NewValues["F01104"] = cb1.Value;
                e.NewValues["DenCateg"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["DenTarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["DenTarif"] = cb2.Text;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["Tarife"].Rows.Find(keys);
                int poz = 0, val = 0;
                foreach (DataColumn col in dsCalcul.Tables["Tarife"].Columns)
                {                  
                    col.ReadOnly = false;
                    var edc = e.NewValues[col.ColumnName];
                    row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                    if (col.ColumnName == "F01104")
                        poz = Convert.ToInt32(e.NewValues[col.ColumnName]);
                    if (col.ColumnName == "F01105")
                        val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                }

                e.Cancel = true;
                grDateSporuri1.CancelEdit();

                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sirNou = "";
                for (int i = 0; i < sir.Length; i++)
                    if (i == poz - 1)
                        sirNou += val.ToString();
                    else
                        sirNou += sir[i];

                ds.Tables[0].Rows[0]["F10067"] = sirNou;
                ds.Tables[1].Rows[0]["F10067"] = sirNou;


                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateSporuri1.DataSource = dsCalcul.Tables["Tarife"];
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
                GridViewDataColumn col1 = ((ASPxGridView)sender).Columns["DenCateg"] as GridViewDataColumn;
                ASPxComboBox cb1 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col1, "cmbMaster");
                e.NewValues["F01104"] = cb1.Value;
                e.NewValues["DenCateg"] = cb1.Text;
                GridViewDataColumn col2 = ((ASPxGridView)sender).Columns["DenTarif"] as GridViewDataColumn;
                ASPxComboBox cb2 = (ASPxComboBox)((ASPxGridView)sender).FindEditRowCellTemplateControl(col2, "cmbChild");
                e.NewValues["F01105"] = cb2.Value;
                e.NewValues["DenTarif"] = cb2.Text;

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["Tarife"].Rows.Find(keys);
                int poz = 0, val = 0;
                foreach (DataColumn col in dsCalcul.Tables["Tarife"].Columns)
                {
                    col.ReadOnly = false;
                    var edc = e.NewValues[col.ColumnName];
                    row[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                    if (col.ColumnName == "F01104")
                        poz = Convert.ToInt32(e.NewValues[col.ColumnName]);
                    if (col.ColumnName == "F01105")
                        val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                }

                e.Cancel = true;
                grDateSporuri2.CancelEdit();

                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sirNou = "";
                for (int i = 0; i < sir.Length; i++)
                    if (i == poz - 1)
                        sirNou += val.ToString();
                    else
                        sirNou += sir[i];

                ds.Tables[0].Rows[0]["F10067"] = sirNou;
                ds.Tables[1].Rows[0]["F10067"] = sirNou;


                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateSporuri2.DataSource = dsCalcul.Tables["Tarife"];
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
                      
                
        }

        protected void cmbChild1_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild1_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild1_new", templateContainer.VisibleIndex);
     
    
            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild1_Callback);
    
        }

        protected void cmbChild1_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildAccessDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

            cmbChildAccessDataSource.SelectParameters.Clear();
            cmbChildAccessDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChild.DataBindItems();
        }



        protected void cmbMaster2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged2(s, e, {0}); }}", templateContainer.VisibleIndex);

   
        }

        protected void cmbChild2_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild2_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild2_new", templateContainer.VisibleIndex);


            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild2_Callback);

        }

        protected void cmbChild2_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildAccessDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

            cmbChildAccessDataSource.SelectParameters.Clear();
            cmbChildAccessDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChild.DataBindItems();
        }


    }
}