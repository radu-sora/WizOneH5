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
                dt.Columns.Add("Spor", typeof(int));
                dt.Columns.Add("Cod", typeof(string));
              
              

                string cmp = "CONVERT(VARCHAR, ";
                if (Constante.tipBD == 2)
                    cmp = "TO_CHAR(";
                for (int i = 80; i <= 99; i++)
                {
                    sql += "select " + (i - 79).ToString() + " as \"Id\", f10095" + i + " as \"Spor\", CASE WHEN f10095" + i + " = 0 THEN 'Spor " + (i - 79).ToString() + "' ELSE " + cmp + " f10095" + i + ") END as \"Cod\" from f1001 where f10003 = " + Session["Marca"].ToString();
                    if (i < 99)
                        sql += " UNION ";
                }

                dt = General.IncarcaDT(sql, null);
                dt.Columns["Spor"].ReadOnly = false;
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                dt.TableName = "SporTran";
                if (dsCalcul == null)
                    dsCalcul = new DataSet();

                dsCalcul.Tables.Add(dt);
            }
            grDateSporTran.KeyFieldName = "Id";
            grDateSporTran.DataSource = dt;
            grDateSporTran.SettingsPager.PageSize = 20;


            sql = @"SELECT 0 as F02104, '---' AS F02105 UNION SELECT F02104, F02105 FROM F021 WHERE F02162 IS NOT NULL AND F02162 <> 0";
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
                    if (col.ColumnName != "Id")
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName];
                        if (col.ColumnName == "Cod" && e.NewValues["Spor"].ToString() == "0")
                            row[col.ColumnName] = "Spor " + keys[0];
                        else
                            row[col.ColumnName] = e.NewValues["Spor"];
                    }
                }

                e.Cancel = true;
                grDateSporTran.CancelEdit();

                ds.Tables[0].Rows[0]["F10095" + (79 + Convert.ToInt32(keys[0])).ToString()] = e.NewValues["Spor"];
                ds.Tables[2].Rows[0]["F10095" + (79 + Convert.ToInt32(keys[0])).ToString()] = e.NewValues["Spor"];          

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