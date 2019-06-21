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
    public partial class Tarife : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            grDateTarife.DataBind();           
            foreach (dynamic c in grDateTarife.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateTarife.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateTarife.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateTarife.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");



        }

        protected void grDateTarife_DataBinding(object sender, EventArgs e)
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
                
             

                string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                string sqlFinal = "";
                string sql = "SELECT F01104, F01105, (SELECT TOP 1 b.F01107 FROM F011 b WHERE b.F01104 = a.F01104) AS \"DenCateg\", " 
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ", cond = "";

                if (Constante.tipBD == 2)
                    sql = "SELECT F01104, F01105, (SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND ROWNUM = 1) AS \"DenCateg\", "
                        + "(SELECT b.F01107 FROM F011 b WHERE b.F01104 = a.F01104 AND b.F01105 = a.F01105) AS \"DenTarif\" FROM F011 a ";

                for (int i = 0; i < sir.Length; i++)
                    if (sir[i] != '0')
                    {
                        cond = "WHERE (a.F01104 = " + (i + 1).ToString() + " AND a.F01105 = " + sir[i] + ") ";
                        sqlFinal += (sqlFinal.Length <= 0 ? "" : " UNION ") + sql + cond;
                    }

                if (sqlFinal.Length > 0)
                    dt = General.IncarcaDT(sqlFinal, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F01104"] };
                dt.TableName = "Tarife";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateTarife.KeyFieldName = "F01104";
            grDateTarife.DataSource = dt;   

            Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;

        }


        protected void grDateTarife_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
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

                if (e.NewValues["DenCateg"] == null || e.NewValues["DenCateg"].ToString().Length < 0)
                    return;

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                object[] row = new object[dsCalcul.Tables["Tarife"].Columns.Count];
                int x = 0, poz = 0, val = 0;
                foreach (DataColumn col in dsCalcul.Tables["Tarife"].Columns)
                {
                    row[x] = e.NewValues[col.ColumnName];
                    if (col.ColumnName == "F01104")
                        poz = Convert.ToInt32(e.NewValues[col.ColumnName]);
                    if (col.ColumnName == "F01105")
                        val = Convert.ToInt32(e.NewValues[col.ColumnName]);
                    x++;
                }

                dsCalcul.Tables["Tarife"].Rows.Add(row);
                e.Cancel = true;
                grDateTarife.CancelEdit();
                grDateTarife.DataSource = dsCalcul.Tables["Tarife"];
                grDateTarife.KeyFieldName = "F01104";

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
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateTarife_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
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

                if (e.NewValues["DenCateg"] == null || e.NewValues["DenCateg"].ToString().Length < 0)
                    return;

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
                grDateTarife.CancelEdit();

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
                grDateTarife.DataSource = dsCalcul.Tables["Tarife"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateTarife_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            //if (e.Column.FieldName == "DenCateg")
            //{
            //    var tb = e.Editor as ASPxTextBox;
            //    tb.ClientSideEvents.TextChanged = "OnTextChangedTarife";
            //}
        }

        //protected void FillTarifCombo(ASPxComboBox cmb, int tarif)
        //{
        //    //if (string.IsNullOrEmpty(tarif)) return;

        //    DataTable dt = GetTarife(tarif);
        //    cmb.Items.Clear();
        //    cmb.DataSource = dt;

        //}
        //DataTable GetTarife(int tarif)
        //{
        //    string sql = @"SELECT * FROM F011 WHERE F01104 = " + tarif;
        //    if (Constante.tipBD == 2)
        //        sql = General.SelectOracle("F011", "F01105") + " WHERE F01104 = " + tarif;
        //    return General.IncarcaDT(sql, null);        

        //}

        //void cmbTarif_OnCallback(object source, CallbackEventArgsBase e)
        //{
        //    FillTarifCombo(source as ASPxComboBox, Convert.ToInt32(e.Parameter));
        //}

        //protected void grDateTarife_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        //{ 
        //    var grid = sender as ASPxGridView;
        //    string sql = @"SELECT * FROM F011 WHERE F01104 = " + e.Parameters;
        //    if (Constante.tipBD == 2)
        //        sql = General.SelectOracle("F011", "F01105") + " WHERE F01104 = " + e.Parameters;

        //    DataTable dtGrup = General.IncarcaDT(sql, null);
        //    GridViewDataComboBoxColumn colCateg = (grDateTarife.Columns["F01105"] as GridViewDataComboBoxColumn);
        //    colCateg.PropertiesComboBox.DataSource = dtGrup;
        //}


        protected void cmbMaster_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbParent = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbParent.NamingContainer as GridViewDataItemTemplateContainer;

            string[] param = templateContainer.ClientID.Split('_');
            if (param[1] != "editnew")
            {
                cmbParent.Value = Convert.ToInt32(templateContainer.KeyValue);
                Session["Tarife_cmbMaster"] = templateContainer.KeyValue;
            }

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged(s, e, {0}); }}", templateContainer.VisibleIndex);               
        }

        protected void cmbChild_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            string[] param = templateContainer.ClientID.Split('_');    

            cmbChild.ClientInstanceName = String.Format("cmbChild_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild_new", templateContainer.VisibleIndex);
            else
            {           
                ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

                cmbChildDataSource.SelectParameters.Clear();
                cmbChildDataSource.SelectParameters.Add("categ", Session["Tarife_cmbMaster"].ToString());
                cmbChild.DataBindItems();
                //cmbChild.Value = Convert.ToInt32(param[2]);
            }
     
    
            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild_Callback);
    
        }

        protected void cmbChild_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

            cmbChildDataSource.SelectParameters.Clear();
            cmbChildDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChild.DataBindItems();
        }

  



    }
}