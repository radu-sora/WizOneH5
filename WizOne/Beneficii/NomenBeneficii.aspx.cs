using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using DevExpress.Web.ASPxHtmlEditor;

namespace WizOne.Beneficii
{
    public partial class NomenBeneficii : System.Web.UI.Page
    {
        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDate.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDate.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                #endregion

                txtTitlu.Text = Dami.TraduCuvant("Nomenclator beneficii");

                if (IsPostBack)
                {
                    DataTable dt = Session["NomenBen_Grid"] as DataTable;
                    grDate.KeyFieldName = "Id";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                }
                else
                    IncarcaGrid();
                 
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
                string strSql = @"select a.""IdCategorie"", a.""Id"", a.""Denumire"", a.""Descriere"", a.USER_NO, a.TIME, a.ValoareEstimata
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') ORDER BY a.""Denumire""";

                DataTable dt = General.IncarcaDT(strSql, null);
                grDate.KeyFieldName = "Id";
                grDate.DataSource = dt;              
                grDate.DataBind();
                Session["NomenBen_Grid"] = dt;

                if (!IsPostBack)
                {
                    DataTable dtCateg = General.IncarcaDT(@"SELECT  a.""Id"" FROM ""Admin_Categorii"" a
                                where a.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal')", null);
                    if (dtCateg != null && dtCateg.Rows.Count > 0)
                        Session["NomenBen_IdCateg"] = Convert.ToInt32(dtCateg.Rows[0][0].ToString());
                    else
                        Session["NomenBen_IdCateg"] = 1;
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }       

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
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

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["NomenBen_Grid"] as DataTable;
                DataRow dr = dt.NewRow();


                ASPxTextBox txtDen = grDate.FindEditFormTemplateControl("txtDen") as ASPxTextBox;
                ASPxMemo txtDesc = grDate.FindEditFormTemplateControl("txtDesc") as ASPxMemo;

                dr["IdCategorie"] = Convert.ToInt32(Session["NomenBen_IdCateg"].ToString());
                dr["Denumire"] = txtDen.Text;

                int id = 1;
                DataTable dtId = General.IncarcaDT("SELECT MAX(Id) + 1 FROM Admin_Obiecte", null);
                if (dtId != null && dtId.Rows.Count > 0)
                    id = Convert.ToInt32(dtId.Rows[0][0].ToString());
                dr["Id"] =id;
                dr["Descriere"] = txtDesc.Text;

                metaUploadFile itm = Session["DocUpload_NomenBen"] as metaUploadFile;
                if (itm != null)                
                    General.LoadFile(itm.UploadedFileName.ToString(), itm.UploadedFile, "Beneficii", Convert.ToInt32(dr["Id"].ToString())); 

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "Id";
                Session["NomenBen_Grid"] = dt;
                General.SalveazaDate(dt, "Admin_Obiecte");
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

                DataTable dt = Session["NomenBen_Grid"] as DataTable;

                DataRow dr = dt.Rows.Find(keys);


                ASPxTextBox txtDen = grDate.FindEditFormTemplateControl("txtDen") as ASPxTextBox;
                ASPxMemo txtDesc = grDate.FindEditFormTemplateControl("txtDesc") as ASPxMemo;

                dr["Denumire"] = txtDen.Text;

                dr["Descriere"] = txtDesc.Text;

                metaUploadFile itm = Session["DocUpload_NomenBen"] as metaUploadFile;
                if (itm != null)
                    General.LoadFile(itm.UploadedFileName.ToString(), itm.UploadedFile, "Beneficii", Convert.ToInt32(dr["Id"].ToString()));

                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "Id";
                Session["NomenBen_Grid"] = dt;
                General.SalveazaDate(dt, "Admin_Obiecte");
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

                DataTable dt = Session["NomenBen_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);               

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["NomenBen_Grid"] = dt;
                grDate.DataSource = dt;
                General.SalveazaDate(dt, "Admin_Obiecte");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUploadBen_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_NomenBen"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                HtmlTableCell lblDen = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDen");
                lblDen.InnerText = Dami.TraduCuvant("Denumire");
                HtmlTableCell lblDeLa = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDeLa");
                lblDeLa.InnerText = Dami.TraduCuvant("De la");
                HtmlTableCell lblLa = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblLa");
                lblLa.InnerText = Dami.TraduCuvant("La");
                HtmlTableCell lblDesc = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDesc");
                lblDesc.InnerText = Dami.TraduCuvant("Descriere");    

                ASPxUploadControl btnDocUploadBen = (ASPxUploadControl)grDate.FindEditFormTemplateControl("btnDocUploadBen");
                btnDocUploadBen.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUploadBen.ToolTip = Dami.TraduCuvant("Incarca Document");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }

}