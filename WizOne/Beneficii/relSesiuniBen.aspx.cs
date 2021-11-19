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

namespace WizOne.Beneficii
{
    public partial class relSesiuniBen : System.Web.UI.Page
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

                if (!IsPostBack)
                    Session["relSesiune_Grid"] = null;


                switch (tip)
                {
                    case 1:
                        lblTitlu.Text = Dami.TraduCuvant("Grupuri angajati");
                        dynamic c = new GridViewDataColumn();
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "IdGrup";
                        c.FieldName = "IdGrup";
                        c.Caption = Dami.TraduCuvant("Grup angajati");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        DataTable dt= GetGrupuri();
                        GridViewDataComboBoxColumn col = (grDate.Columns["IdGrup"] as GridViewDataComboBoxColumn);
                        col.PropertiesComboBox.DataSource = dt;
                        break;
                    case 2:
                        lblTitlu.Text = Dami.TraduCuvant("Grup beneficii");
                        c = new GridViewDataComboBoxColumn();
                        c.Name = "IdGrup";
                        c.FieldName = "IdGrup";
                        c.Caption = Dami.TraduCuvant("Grup beneficii");
                        c.PropertiesComboBox.TextField = "Denumire";
                        c.PropertiesComboBox.ValueField = "Id";
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        c = new GridViewDataDateColumn();
                        c.Name = "DataInceput";
                        c.FieldName = "DataInceput";
                        c.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy";
                        c.Caption = Dami.TraduCuvant("Data inceput");
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        c = new GridViewDataDateColumn();
                        c.Name = "DataSfarsit";
                        c.FieldName = "DataSfarsit";
                        c.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy";
                        c.Caption = Dami.TraduCuvant("Data sfarsit");
                        if (!IsPostBack)
                            grDate.Columns.Add(c);

                        dt = GetBeneficii();
                        col = (grDate.Columns["IdGrup"] as GridViewDataComboBoxColumn);
                        col.PropertiesComboBox.DataSource = dt;
                        break;
                   
                }
                
                Session["relSesiune_Tip"] = tip;
                IncarcaGrid(tip, qwe);

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetGrupuri()
        {
            try
            {
                string sql = "SELECT DenSet AS \"Denumire\", IdSetAng AS \"Id\" FROM "
                    + " \"Ben_SetAngajati\"  "
                    + "  ORDER BY \"Denumire\" ";      

                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetBeneficii()
        {
            try
            {
                //string sql =  @"select  CAST (a.""Id"" AS INT) as ""Id"", a.""Denumire""
                //                from ""Admin_Obiecte"" a
                //                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                //                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') ORDER BY a.""Denumire""";

                string sql = @"SELECT * FROM ""Ben_tblGrupBeneficii"" ORDER BY ""Id""";

                return General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

       

        private void IncarcaGrid(int tip, string id)
        {
            try
            {
                DataTable dt = new DataTable();

                string strSql = "";

                switch (tip)
                {
                    case 1:
                        strSql = " SELECT * FROM \"Ben_relSesGrupAng\" WHERE \"IdSesiune\" = " + id;
                        break;
                    case 2:
                        strSql = " SELECT * FROM \"Ben_relSesGrupBen\" WHERE \"IdSesiune\" = " + id;
                        break;                   
                }

                if (Session["relSesiune_Grid"] == null)
                {
                    dt = General.IncarcaDT(strSql, null);
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["relSesiune_Grid"] = dt;
                }
                else
                {
                    dt = Session["relSesiune_Grid"] as DataTable;
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

                DataTable dt = Session["relSesiune_Grid"] as DataTable;

                switch (Convert.ToInt32(Session["relSesiune_Tip"].ToString()))
                {
                    case 1:
                        General.SalveazaDate(dt, "Ben_relSesGrupAng");
                        break;
                    case 2:
                        General.SalveazaDate(dt, "Ben_relSesGrupBen");
                        break;                   
                }               

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

                DataTable dt = Session["relSesiune_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                switch (Convert.ToInt32(Session["relSesiune_Tip"].ToString()))
                {
                    case 1:
                        if (e.NewValues["IdGrup"] == null)
                        { 
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste grupul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 2:
                        if (e.NewValues["IdGrup"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste grupul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;                   
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
                Session["relSesiune_Grid"] = dt;
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
                DataTable dt = Session["relSesiune_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
       
                string id = General.Nz(Request["qwe"], -99).ToString();
                e.NewValues["IdSesiune"] = Convert.ToInt32(id);

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
                    switch (Convert.ToInt32(Session["relSesiune_Tip"].ToString()))
                    {
                        case 1:
                            e.NewValues["IdAuto"] = Dami.NextId("Ben_relSesGrupAng");
                            break;
                        case 2:
                            e.NewValues["IdAuto"] = Dami.NextId("Ben_relSesGrupBen");
                            break;                        
                    }
                }


                switch (Convert.ToInt32(Session["relSesiune_Tip"].ToString()))
                {
                    case 1:
                        if (e.NewValues["IdGrup"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste grupul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;
                    case 3:
                        if (e.NewValues["IdGrup"] == null)
                        {
                            grDate.JSProperties["cpAlertMessage"] = "Lipseste grupul!";
                            e.Cancel = true;
                            grDate.CancelEdit();
                            return;
                        }
                        break;                    
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
                Session["relSesiune_Grid"] = dt;
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

                DataTable dt = Session["relSesiune_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["relSesiune_Grid"] = dt;
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