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
    public partial class SporuriTranzactii : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            grDateSporTran.DataBind();
            //grDateGrupuri.AddNewRow();
            foreach (dynamic c in grDateSporTran.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateSporTran.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateSporTran.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
     
        }

        protected void grDateSporTran_DataBinding(object sender, EventArgs e)
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
            string sql = "";
            if (dsCalcul != null && dsCalcul.Tables.Contains("SporTran"))
            {
                dt = dsCalcul.Tables["SporTran"];
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Cod", typeof(int));
                dt.Columns.Add("Spor", typeof(int));

                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };

            
                sql = "select 1 as id, f1009580 as spor, f1009580 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 2 as id, f1009581 as spor, f1009581 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 3 as id, f1009582 as spor, f1009582 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 4 as id, f1009583 as spor, f1009583 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 5 as id, f1009584 as spor, f1009584 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 6 as id, f1009585 as spor, f1009585 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 7 as id, f1009586 as spor, f1009586 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 8 as id, f1009587 as spor, f1009587 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 9 as id, f1009588 as spor, f1009588 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 10 as id, f1009589 as spor, f1009589 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 11 as id, f1009590 as spor, f1009590 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 12 as id, f1009591 as spor, f1009591 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 13 as id, f1009592 as spor, f1009592 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 14 as id, f1009593 as spor, f1009593 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 15 as id, f1009594 as spor, f1009594 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 16 as id, f1009595 as spor, f1009595 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union " 
                            + " select 17 as id, f1009596 as spor, f1009596 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 18 as id, f1009597 as spor, f1009597 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 19 as id, f1009598 as spor, f1009598 as cod from f1001 where f10003 = " + Session["Marca"].ToString()
                            + " union "
                            + " select 20 as id, f1009599 as spor, f1009599 as cod from f1001 where f10003 = " + Session["Marca"].ToString();

             
                dt = General.IncarcaDT(sql, null);
                dt.TableName = "SporTran";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateSporTran.KeyFieldName = "Id";
            grDateSporTran.DataSource = dt;


            sql = @"SELECT F02104, F02105 FROM F021 WHERE F02162 IS NOT NULL AND F02162 <> 0";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("F021", "F02104") + " WHERE F02162 IS NOT NULL AND F02162 <> 0 ";
            DataTable dtSpor = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colSpor = (grDateSporTran.Columns["Spor"] as GridViewDataComboBoxColumn);
            colSpor.PropertiesComboBox.DataSource = dtSpor;

            Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;

        }

      

        protected void grDateSporTran_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataSet dsCalcul = Session["InformatiaCurentaPersonalCalcul"] as DataSet;

                DataRow row = dsCalcul.Tables["SporTran"].Rows.Find(keys);              
                foreach (DataColumn col in dsCalcul.Tables["SporTran"].Columns)
                {
                    col.ReadOnly = false;
                    var edc = e.NewValues[col.ColumnName];   
                }

                e.Cancel = true;
                grDateSporTran.CancelEdit();

                //ds.Tables[0].Rows[0]["F10067"] = sirNou;
                //ds.Tables[1].Rows[0]["F10067"] = sirNou;


                Session["InformatiaCurentaPersonal"] = ds;
                Session["InformatiaCurentaPersonalCalcul"] = dsCalcul;
                grDateSporTran.DataSource = dsCalcul.Tables["SporTran"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




    }
}