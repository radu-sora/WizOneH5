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
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class Sanctiuni : System.Web.UI.UserControl
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
                grDateSanctiuni.DataBind();

                foreach (dynamic c in grDateSanctiuni.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                grDateSanctiuni.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateSanctiuni.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateSanctiuni.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateSanctiuni.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateSanctiuni.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_DataBinding(object sender, EventArgs e)
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
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Sanctiuni"))
                {
                    dt = ds.Tables["Admin_Sanctiuni"];
                }
                else
                {
                    dt = General.IncarcaDT($@"SELECT * FROM ""Admin_Sanctiuni"" WHERE ""Marca""=@1", new object[] { Session["Marca"] });
                    dt.TableName = "Admin_Sanctiuni";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                grDateSanctiuni.KeyFieldName = "IdAuto";
                grDateSanctiuni.DataSource = dt;

                DataTable dtSanc = General.GetObiecteDinArie("ArieTabSanctiuniDinPersonal");
                GridViewDataComboBoxColumn colSanc = (grDateSanctiuni.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colSanc.PropertiesComboBox.DataSource = dtSanc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Sanctiuni"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Sanctiuni");
                e.NewValues["Marca"] = Session["Marca"];
       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataTable dt = ds.Tables["Admin_Sanctiuni"];
                DataRow dr = dt.NewRow();

                ASPxMemo txtDesc = grDateSanctiuni.FindEditFormTemplateControl("txtDesc") as ASPxMemo;
                ASPxDateEdit txtDataInc = grDateSanctiuni.FindEditFormTemplateControl("txtDataInc") as ASPxDateEdit;
                ASPxDateEdit txtDataSf = grDateSanctiuni.FindEditFormTemplateControl("txtDataSf") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateSanctiuni.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtVal = grDateSanctiuni.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                ASPxTextBox txtProc = grDateSanctiuni.FindEditFormTemplateControl("txtProc") as ASPxTextBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(ds.Tables["Admin_Sanctiuni"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Sanctiuni");
                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataInceput"] = txtDataInc.Value ?? DBNull.Value;
                dr["DataSfarsit"] = txtDataSf.Value ?? DBNull.Value;
                dr["ValoareAbsoluta"] = txtVal.Value ?? DBNull.Value;
                dr["ValoareProcent"] = txtProc.Value ?? DBNull.Value;
                dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Sanctiuni"] as metaUploadFile;
                if (itm != null)
                {
                    General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Sanctiuni", Convert.ToInt32(dr["IdAuto"].ToString()) + (Constante.tipBD == 1 ? 0 : 1));
                    if (Constante.tipBD == 2)
                        dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1;
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                }

                ds.Tables["Admin_Sanctiuni"].Rows.Add(dr);

                Session["DocUpload_MP_Sanctiuni"] = null;

                e.Cancel = true;
                grDateSanctiuni.CancelEdit();
                grDateSanctiuni.DataSource = ds.Tables["Admin_Sanctiuni"];
                grDateSanctiuni.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Sanctiuni"].Rows.Find(idAuto);

                ASPxMemo txtDesc = grDateSanctiuni.FindEditFormTemplateControl("txtDesc") as ASPxMemo;
                ASPxDateEdit txtDataInc = grDateSanctiuni.FindEditFormTemplateControl("txtDataInc") as ASPxDateEdit;
                ASPxDateEdit txtDataSf = grDateSanctiuni.FindEditFormTemplateControl("txtDataSf") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateSanctiuni.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtVal = grDateSanctiuni.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                ASPxTextBox txtProc = grDateSanctiuni.FindEditFormTemplateControl("txtProc") as ASPxTextBox;

                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataInceput"] = txtDataInc.Value ?? DBNull.Value;
                dr["DataSfarsit"] = txtDataSf.Value ?? DBNull.Value;
                dr["ValoareAbsoluta"] = txtVal.Value ?? DBNull.Value;
                dr["ValoareProcent"] = txtProc.Value ?? DBNull.Value;
                dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Sanctiuni"] as metaUploadFile;
                if (itm != null)
                {
                    General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Sanctiuni", dr["IdAuto"]);
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                }

                Session["DocUpload_MP_Sanctiuni"] = null;

                e.Cancel = true;
                grDateSanctiuni.CancelEdit();
                grDateSanctiuni.DataSource = ds.Tables["Admin_Sanctiuni"];
                grDateSanctiuni.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Sanctiuni"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateSanctiuni.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateSanctiuni.DataSource = ds.Tables["Admin_Sanctiuni"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUploadSanc_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_MP_Sanctiuni"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSanctiuni_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbCateg = grDateSanctiuni.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtSanc = General.GetObiecteDinArie("ArieTabSanctiuniDinPersonal");
                    cmbCateg.DataSource = dtSanc;
                    cmbCateg.DataBindItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}