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
            grDateTarife.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateTarife.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
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
            DataSet ds = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
            if (ds != null && ds.Tables.Contains("Tarife"))
            {
                dt = ds.Tables["Tarife"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("F01104", typeof(int));
                dt.Columns.Add("F01105", typeof(int));
                dt.TableName = "Tarife";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F01104"], dt.Columns["F01105"] };

                if (ds == null)
                    ds = new DataSet();

                ds.Tables.Add(dt);
            }
            grDateTarife.KeyFieldName = "F01104; F01105";
            grDateTarife.DataSource = dt;

            string sql = @"SELECT * FROM F011 WHERE F01105 = 1 ORDER BY F01104";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F011", "F01104") + "ORDER BY F01104 ";
          
            //DataTable dtGrup = General.IncarcaDT(sql, null);
            //GridViewDataComboBoxColumn colCateg = (grDateTarife.Columns["F01104"] as GridViewDataComboBoxColumn);
            //colCateg.PropertiesComboBox.DataSource = dtGrup;


            Session["InformatiaCurentaPersonalCalcul"] = ds;

        }


        protected void grDateTarife_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {            

                //DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                //DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;
              
                //object[] rowComp = new object[dsCalcul.Tables["Componente"].Columns.Count];
                //int x = 0;
                //foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
                //{                   
                //    switch (col.ColumnName.ToUpper())
                //    {
                //        case "SUMA":
                //            rowComp[x] = e.NewValues[col.ColumnName];
                //            ds.Tables[1].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
                //            break;
                //        default:
                //            rowComp[x] = e.NewValues[col.ColumnName];
                //            break;
                //    } 
                //    x++;
                //}

                //dsCalcul.Tables["Componente"].Rows.Add(rowComp);
                //e.Cancel = true;
                //grDateTarife.CancelEdit();
                //grDateTarife.DataSource = dsCalcul.Tables["Componente"];
                //grDateTarife.KeyFieldName = "F02104";               

                //Session["InformatiaCurentaPersonal"] = ds;
                //Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
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
                
                //object[] keys = new object[e.Keys.Count];
                //for (int i = 0; i < e.Keys.Count; i++)
                //{ keys[i] = e.Keys[i]; }

                //DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                //DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                //DataRow rowComp = dsCalcul.Tables["Componente"].Rows.Find(keys);

                //foreach (DataColumn col in dsCalcul.Tables["Componente"].Columns)
                //{
                //    if (col.ColumnName.ToUpper() == "SUMA")
                //    {
                //        col.ReadOnly = false;
                //        var edc = e.NewValues[col.ColumnName];
                //        rowComp[col.ColumnName] = e.NewValues[col.ColumnName] ?? 0;
                //        ds.Tables[1].Rows[0]["F10069" + (Convert.ToInt32(e.NewValues["F02104"].ToString().Substring(2)) - 1).ToString()] = e.NewValues[col.ColumnName];
                //    }

                //}

                //e.Cancel = true;
                //grDateTarife.CancelEdit();
                //Session["InformatiaCurentaPersonal"] = ds;
                //Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                //grDateTarife.DataSource = dsCalcul.Tables["Componente"];
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateTarife_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            //if (!grDateTarife.IsEditing || e.Column.FieldName != "F01105") return;
            //if (e.KeyValue == DBNull.Value || e.KeyValue == null) return;
            //object val = grDateTarife.GetRowValuesByKeyValue(e.KeyValue, "F01104");
            //if (val == DBNull.Value) return;
            //int tarif = Convert.ToInt32(val);

            //ASPxComboBox combo = e.Editor as ASPxComboBox;
            //FillTarifCombo(combo, tarif);

            //combo.Callback += new CallbackEventHandlerBase(cmbTarif_OnCallback);
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

            cmbParent.ClientSideEvents.SelectedIndexChanged = String.Format("function(s, e) {{ OnSelectedIndexChanged(s, e, {0}); }}", templateContainer.VisibleIndex);
                       
            //ControlParameter idCateg = new ControlParameter();
            //idCateg.ControlID = cmbParent.UniqueID;
            //idCateg.PropertyName = "Value";
            //idCateg.Name = "categ";
            //idCateg.Type = TypeCode.Int32;
            //asdChild.SelectParameters.Add(idCateg);
            
        }

        protected void cmbChild_Init(object sender, EventArgs e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            GridViewDataItemTemplateContainer templateContainer = cmbChild.NamingContainer as GridViewDataItemTemplateContainer;

            cmbChild.ClientInstanceName = String.Format("cmbChild_{0}", templateContainer.VisibleIndex);

            if (templateContainer.Grid.IsNewRowEditing)
                cmbChild.ClientInstanceName = String.Format("cmbChild_new", templateContainer.VisibleIndex);
     
    
            cmbChild.Callback += new CallbackEventHandlerBase(cmbChild_Callback);
    
        }

        protected void cmbChild_Callback(object sender, CallbackEventArgsBase e)
        {
            ASPxComboBox cmbChild = sender as ASPxComboBox;

            ObjectDataSource cmbChildAccessDataSource = cmbChild.NamingContainer.FindControl("asdChild") as ObjectDataSource;

            cmbChildAccessDataSource.SelectParameters.Clear();
            cmbChildAccessDataSource.SelectParameters.Add("categ", e.Parameter);
            cmbChild.DataBindItems();
        }





    }
}