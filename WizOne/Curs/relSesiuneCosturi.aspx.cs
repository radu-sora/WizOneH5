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

namespace WizOne.Curs
{
    public partial class relSesiuneCosturi : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);
       
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");


                string qwe = General.Nz(Request["qwe"], "-99").ToString();
                int tip = Convert.ToInt32(General.Nz(Request["tip"], -99));

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCosturi"" ", null);
                GridViewDataComboBoxColumn col = (grDate.Columns["IdCost"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblFurnizori"" ", null);
                col = (grDate.Columns["IdFurnizor"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                dt = General.IncarcaDT(@"SELECT * FROM ""Curs_tblCentreCost"" ", null);
                col = (grDate.Columns["IdCentruCost"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = dt;

                int alocareCursCosturiXCentruCost = Convert.ToInt32(Dami.ValoareParam("alocareCursCosturiXCentruCost", "0"));

                if (!IsPostBack)
                    Session["relSesiuneCosturi_Grid"] = null;

                switch (tip)
                {
                    case 0:
                        lblTitlu.Text = Dami.TraduCuvant("Cost estimat");
                        grDate.Columns["IdCentruCost"].Visible = false;
                        break;
                    case 1:
                        lblTitlu.Text = Dami.TraduCuvant("Cost efectiv");
                        grDate.Columns["IdCentruCost"].Visible = (alocareCursCosturiXCentruCost == 1) ? true : false;
                        break;                    
                }
                Session["relSesiuneCosturi_Tip"] = tip;
                IncarcaGrid(tip, qwe);

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                    }
                    catch (Exception) { }
                }
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDate.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

      

        private void IncarcaGrid(int tip, string id)
        {
            try
            {
                DataTable dt = new DataTable();

                string strSql = " SELECT * FROM \"Curs_relSesiuneCosturi\" WHERE \"IdCurs\" = " + id.Split(',')[0] + " AND \"IdSesiune\" = " + id.Split(',')[1] + " AND \"IdTipCost\" = " + tip;

                if (Session["relSesiuneCosturi_Grid"] == null)
                {
                    dt = General.IncarcaDT(strSql, null);
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["relSesiuneCosturi_Grid"] = dt;
                }
                else
                {
                    dt = Session["relSesiuneCosturi_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable dt = Session["relSesiuneCosturi_Grid"] as DataTable;

                General.SalveazaDate(dt, "Curs_relSesiuneCosturi");

                MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["relSesiuneCosturi_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["IdCost"] == null)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste costul!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["relSesiuneCosturi_Grid"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["relSesiuneCosturi_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;


                string iduri = General.Nz(Request["qwe"], -99).ToString();
                int tip = Convert.ToInt32(General.Nz(Request["tip"], -99));
                e.NewValues["IdCurs"] = Convert.ToInt32(iduri.Split(',')[0]);
                e.NewValues["IdSesiune"] = Convert.ToInt32(iduri.Split(',')[1]);
                e.NewValues["IdTipCost"] = tip;
                e.NewValues["NumeTipCost"] = tip == 0 ? "Cost Estimat" : "Cost Efectiv";

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
                {
                    e.NewValues["IdAuto"] = Dami.NextId("Curs_relSesiuneCosturi");

                }

                if (e.NewValues["IdCost"] == null)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste costul!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }  

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["relSesiuneCosturi_Grid"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["relSesiuneCosturi_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["relSesiuneCosturi_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {

            try
            {
      
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        

    }
}