using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class RelNomenDoc : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();


                string sqlNomen1 = @"SELECT ""Id"", ""Denumire"" FROM ""tblNomenDoc"" WHERE ""Tip"" = 1";
                DataTable dtNomen1 = General.IncarcaDT(sqlNomen1, null);
                GridViewDataComboBoxColumn colNomen1 = (grDate.Columns["IdNomen1"] as GridViewDataComboBoxColumn);
                colNomen1.PropertiesComboBox.DataSource = dtNomen1;

                string sqlNomen2 = @"SELECT ""Id"", ""Denumire"" FROM ""tblNomenDoc""  WHERE ""Tip"" = 2";
                DataTable dtNomen2 = General.IncarcaDT(sqlNomen2, null);
                GridViewDataComboBoxColumn colNomen2 = (grDate.Columns["IdNomen2"] as GridViewDataComboBoxColumn);
                colNomen2.PropertiesComboBox.DataSource = dtNomen2;

                grDate.DataBind();
               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
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
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.NewRow();      

                dr["IdNomen1"] = e.NewValues["IdNomen1"];
                dr["Val1"] = e.NewValues["Val1"];
                dr["IdNomen2"] = e.NewValues["IdNomen2"];
                dr["Val2"] = e.NewValues["Val2"];

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("tblRelNomenDoc");

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                ASPxHtmlEditor htmlEditor = grDate.FindEditFormTemplateControl("txtContinut") as ASPxHtmlEditor;
                dr["Document"] = (htmlEditor.Html ?? "").ToString();

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
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

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                dr["IdNomen1"] = e.NewValues["IdNomen1"];
                dr["Val1"] = e.NewValues["Val1"];
                dr["IdNomen2"] = e.NewValues["IdNomen2"];
                dr["Val2"] = e.NewValues["Val2"];      

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                ASPxHtmlEditor htmlEditor = grDate.FindEditFormTemplateControl("txtContinut") as ASPxHtmlEditor;
                dr["Document"] = (htmlEditor.Html ?? "").ToString();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;

                General.SalveazaDate(dt, "tblRelNomenDoc");

                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRenunta_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                dt.RejectChanges();

                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void txtContinut_Init(object sender, EventArgs e)
        {
            ASPxHtmlEditor myHtmlEditor = sender as ASPxHtmlEditor;
            GridViewEditItemTemplateContainer container = myHtmlEditor.NamingContainer as GridViewEditItemTemplateContainer;

            if (!container.Grid.IsNewRowEditing)
                myHtmlEditor.Html = DataBinder.Eval(container.DataItem, "Document").ToString();
           
        }

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            ASPxComboBox cmbNomen1 = grDate.FindEditFormTemplateControl("cmbNomen1") as ASPxComboBox;
            if (cmbNomen1 != null)
            {
                string sqlNomen1 = @"SELECT ""Id"", ""Denumire"" FROM ""tblNomenDoc"" WHERE ""Tip"" = 1";
                DataTable dtNomen1 = General.IncarcaDT(sqlNomen1, null);    
                cmbNomen1.DataSource = dtNomen1;
                cmbNomen1.DataBindItems();
            }
            ASPxComboBox cmbNomen2 = grDate.FindEditFormTemplateControl("cmbNomen2") as ASPxComboBox;
            if (cmbNomen2 != null)
            {
                string sqlNomen2 = @"SELECT ""Id"", ""Denumire"" FROM ""tblNomenDoc"" WHERE ""Tip"" = 2";
                DataTable dtNomen2 = General.IncarcaDT(sqlNomen2, null);
                cmbNomen2.DataSource = dtNomen2;
                cmbNomen2.DataBindItems();
            }
            
        }

        private void IncarcaGrid()
        {
            if (!IsPostBack)
            {
                string sqlTbl = @"SELECT * FROM ""tblRelNomenDoc"" ";
                DataTable dt = General.IncarcaDT(sqlTbl, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                Session["InformatiaCurenta"] = dt;

                grDate.DataSource = Session["InformatiaCurenta"];
                grDate.KeyFieldName = "IdAuto";
            
            }
            else
            {
                foreach (var c in grDate.Columns)
                {
                    try
                    {
                        GridViewDataColumn col = (GridViewDataColumn)c;
                        col.Caption = Dami.TraduCuvant(col.FieldName);
                    }
                    catch (Exception) { }
                }

                grDate.DataSource = Session["InformatiaCurenta"];
              
            }
        }

      
        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            IncarcaGrid();
        }
    }
}