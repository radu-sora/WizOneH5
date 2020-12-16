using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Dosar : System.Web.UI.UserControl
    {
        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Traducere
            foreach (var col in grDateDosar.Columns.OfType<GridViewDataColumn>())
                col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

            grDateDosar.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateDosar.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateDosar.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateDosar.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateDosar.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
            #endregion

            DataTable dtEchip = General.GetObiecteDinArie("ArieTabDosarPersonalDinPersonal");
            GridViewDataComboBoxColumn colEchip = (grDateDosar.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colEchip.PropertiesComboBox.DataSource = dtEchip;

            if (!IsPostBack)
                grDateDosar.DataBind();

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateDosar);
        }

        protected void grDateDosar_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

            try
            {
                DataTable dt = ds.Tables["Admin_Dosar"];
                DataRow dr = dt.NewRow();

                ASPxComboBox cmbNumeBen = grDateDosar.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtDesc = grDateDosar.FindEditFormTemplateControl("txtDesc") as ASPxTextBox;

                if (cmbNumeBen.Value == null)
                {
                    e.Cancel = true;
                    grDateDosar.CancelEdit();
                    grDateDosar.JSProperties["cpAlertMessage"] = "Lipsesc date";
                    return;
                }

                dr["F10003"] = Session["Marca"];
                dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Dosar"] as metaUploadFile;
                if (itm != null)
                {
                    dr["Fisier"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;
                }
                Session["DocUpload_MP_Dosar"] = null;

                ds.Tables["Admin_Dosar"].Rows.Add(dr);

                e.Cancel = true;
                grDateDosar.CancelEdit();
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("is constrained to be unique") >= 0)
                    grDateDosar.JSProperties["cpAlertMessage"] = "Date duplicate";
                else
                    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
            finally
            {
                e.Cancel = true;
                grDateDosar.CancelEdit();
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                Session["InformatiaCurentaPersonal"] = ds;
            }
        }

        protected void grDateDosar_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

            try
            {
                DataRow dr = ds.Tables["Admin_Dosar"].Rows.Find(new object[] { e.Keys["F10003"], e.Keys["IdObiect"] });

                ASPxComboBox cmbNumeBen = grDateDosar.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtDesc = grDateDosar.FindEditFormTemplateControl("txtDesc") as ASPxTextBox;

                if (cmbNumeBen.Value != null)
                {
                    dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                    dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    metaUploadFile itm = Session["DocUpload_MP_Dosar"] as metaUploadFile;
                    if (itm != null)
                    {
                        dr["Fisier"] = itm.UploadedFile;
                        dr["FisierNume"] = itm.UploadedFileName;
                        dr["FisierExtensie"] = itm.UploadedFileExtension;
                    }
                    Session["DocUpload_MP_Dosar"] = null;
                }
                else
                    grDateDosar.JSProperties["cpAlertMessage"] = "Lipsesc date";
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("is constrained to be unique") >= 0)
                    grDateDosar.JSProperties["cpAlertMessage"] = "Date duplicate";
                else
                    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
            finally
            {
                e.Cancel = true;
                grDateDosar.CancelEdit();
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                Session["InformatiaCurentaPersonal"] = ds;
            }
        }

        protected void grDateDosar_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataRow dr = ds.Tables["Admin_Dosar"].Rows.Find(new object[] { e.Keys["F10003"], e.Keys["IdObiect"] });

                if (General.Nz(e.Values["Obligatoriu"], 0).ToString() == "0")
                {
                    dr.Delete();
                    Session["DocUpload_MP_Dosar"] = null;
                }
                else
                    grDateDosar.JSProperties["cpAlertMessage"] = "Acest tip de document vine din fisa postului, este obligatoriu si nu se poate sterge";

                e.Cancel = true;
                grDateDosar.CancelEdit();
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateDosar_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["AreFisier"] = 0;
                e.NewValues["Marca"] = Session["Marca"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUploadDosar_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = e.UploadedFile.FileBytes;
                itm.UploadedFileName = e.UploadedFile.FileName;
                itm.UploadedFileExtension = Path.GetExtension(e.UploadedFile.FileName);

                Session["DocUpload_MP_Dosar"] = itm;

                ((ASPxUploadControl)sender).JSProperties["cpDocUploadName"] = e.UploadedFile.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbCateg = grDateDosar.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtBeneficii = General.GetObiecteDinArie("ArieTabDosarPersonalDinPersonal");
                    cmbCateg.DataSource = dtBeneficii;
                    cmbCateg.DataBindItems();

                    ASPxGridView grDateDosar = (ASPxGridView)sender;
                    string obligatoriu = grDateDosar.GetRowValues(grDateDosar.EditingRowVisibleIndex, new string[] { "Obligatoriu" }).ToString();
                    if (obligatoriu == "1")
                    {
                        cmbCateg.ReadOnly = true;
                        cmbCateg.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
    }
}