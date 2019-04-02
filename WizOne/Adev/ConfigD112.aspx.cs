using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace WizOne.Adev
{
    public partial class ConfigD112 : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                if (!IsPostBack)
                {
                    //DataTable table = new DataTable();
                    //table.Columns.Add("Id", typeof(int));
                    //table.Columns.Add("Denumire", typeof(string));

                    //for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 10; i--)
                    //    table.Rows.Add(i, i.ToString());
                    //GridViewDataComboBoxColumn colAn = (grDate.Columns["AN"] as GridViewDataComboBoxColumn);
                    //colAn.PropertiesComboBox.DataSource = table;

                    //table.Clear();

                    //for (int i = 1; i <= 12; i++)
                    //    table.Rows.Add(i, i.ToString());
                    //GridViewDataComboBoxColumn colLuna = (grDate.Columns["LUNA"] as GridViewDataComboBoxColumn);
                    //colLuna.PropertiesComboBox.DataSource = table;

                    GridViewDataTextColumn colDecl = (grDate.Columns["DECLARATIE"] as GridViewDataTextColumn);
                    colDecl.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    IncarcaGrid();

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }



        private void IncarcaGrid()
        {
            try
            {

                DataTable dtIst = General.IncarcaDT("SELECT * FROM ADEVERINTE_SOM_D112", null);
                Session["InformatiaCurentaConfigD112"] = dtIst;
                grDate.KeyFieldName = "AN;LUNA";
                grDate.DataSource = dtIst;                
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }



        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }




        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurentaConfigD112"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    row[x] = e.NewValues[col.ColumnName];
                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.KeyFieldName = "AN;LUNA";
                grDate.DataSource = dt;
   
                Session["InformatiaCurentaConfigD112"] = dt;
                SalvareDate();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurentaConfigD112"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaConfigD112"] = dt;
                grDate.DataSource = dt;
                SalvareDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurentaConfigD112"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaConfigD112"] = dt;
                grDate.DataSource = dt;
                SalvareDate();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SalvareDate()
        {
            DataTable dt = Session["InformatiaCurentaConfigD112"] as DataTable;
            //if (Constante.tipBD == 1)
            //{
            //    SqlDataAdapter da = new SqlDataAdapter();
            //    SqlCommandBuilder cb = new SqlCommandBuilder();
            //    da = new SqlDataAdapter();
            //    da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM ADEVERINTE_SOM_D112", null);
            //    cb = new SqlCommandBuilder(da);
            //    da.Update(dt);
            //    da.Dispose();
            //    da = null;
            //}
            //else
            //{
            //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
            //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM ADEVERINTE_SOM_D112 WHERE ROWNUM = 0", null);
            //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
            //    oledbAdapter.Update(dt);
            //    oledbAdapter.Dispose();
            //    oledbAdapter = null;
            //}
            General.SalveazaDate(dt, "ADEVERINTE_SOM_D112");
        }




                            //<dx:GridViewDataComboBoxColumn FieldName = "AN" Name="AN" Caption="Anul"  Width="75px" >
                            //    <PropertiesComboBox TextField = "Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            //</dx:GridViewDataComboBoxColumn>
                            //<dx:GridViewDataComboBoxColumn FieldName = "LUNA" Name="LUNA" Caption="Luna"  Width="75px" >
                            //    <PropertiesComboBox TextField = "Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            //</dx:GridViewDataComboBoxColumn>  


    }
}