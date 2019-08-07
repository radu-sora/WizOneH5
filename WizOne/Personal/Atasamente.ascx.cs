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
    public partial class Atasamente : System.Web.UI.UserControl
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
                grDateAtasamente.DataBind();

                foreach (dynamic c in grDateAtasamente.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                grDateAtasamente.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateAtasamente.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateAtasamente.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateAtasamente.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateAtasamente.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAtasamente_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Atasamente"))
                {
                    dt = ds.Tables["Atasamente"];
                }
                else
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Atasamente"" WHERE ""IdEmpl"" = @1", new object[] { Session["Marca"] });
                    dt.TableName = "Atasamente";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                grDateAtasamente.KeyFieldName = "IdAuto";
                grDateAtasamente.DataSource = dt;

                string sql = @"SELECT * FROM ""CategoriiAtasamente"" ";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("CategoriiAtasamente", "IdCategory");
                DataTable dtCategAt = General.IncarcaDT(sql, null);
                GridViewDataComboBoxColumn colCategAt = (grDateAtasamente.Columns["IdCategory"] as GridViewDataComboBoxColumn);
                colCategAt.PropertiesComboBox.DataSource = dtCategAt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAtasamente_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Atasamente"];
                //if (dt.Columns["IdAuto"] != null)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        //int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                //        int max = Dami.NextId("Atasamente");
                //        e.NewValues["IdAuto"] = max;
                //    }
                //    else
                //        e.NewValues["IdAuto"] = 1;
                //}
                e.NewValues["IdEmpl"] = Session["Marca"];

                if (Constante.tipBD == 1)
                    e.NewValues["IdAuto"] = Convert.ToInt32(General.Nz(ds.Tables["Atasamente"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    e.NewValues["IdAuto"] = Dami.NextId("Atasamente");         
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAtasamente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataTable dt = ds.Tables["Atasamente"];
                DataRow dr = dt.NewRow();

                ASPxMemo txtDesc = grDateAtasamente.FindEditFormTemplateControl("txtDesc") as ASPxMemo;
                ASPxDateEdit txtDataDoc = grDateAtasamente.FindEditFormTemplateControl("txtDataDoc") as ASPxDateEdit;
                ASPxComboBox cmbCateg = grDateAtasamente.FindEditFormTemplateControl("cmbCateg") as ASPxComboBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(ds.Tables["Atasamente"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Atasamente");
                dr["IdEmpl"] = Session["Marca"];
                dr["IdCategory"] = cmbCateg.Value ?? DBNull.Value;
                dr["DateAttach"] = txtDataDoc.Value ?? DBNull.Value;
                dr["DescrAttach"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Atasamente"] as metaUploadFile;
                if (itm != null)
                {
                    dr["Attach"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;
                }
                //if (itm != null)
                //{
                //    General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Atasamente", Convert.ToInt32(dr["IdAuto"].ToString()) + (Constante.tipBD == 1 ? 0 : 1));
                //    if (Constante.tipBD == 2)
                //        dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1;
                //    //dr["FisierNume"] = itm.UploadedFileName;
                //    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                //}

                ds.Tables["Atasamente"].Rows.Add(dr);

                Session["DocUpload_MP_Atasamente"] = null;

                e.Cancel = true;
                grDateAtasamente.CancelEdit();
                grDateAtasamente.KeyFieldName = "IdAuto";
                grDateAtasamente.DataSource = ds.Tables["Atasamente"];                
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAtasamente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Atasamente"].Rows.Find(idAuto);

                ASPxMemo txtDesc = grDateAtasamente.FindEditFormTemplateControl("txtDesc") as ASPxMemo;
                ASPxDateEdit txtDataDoc = grDateAtasamente.FindEditFormTemplateControl("txtDataDoc") as ASPxDateEdit;
                ASPxComboBox cmbCateg = grDateAtasamente.FindEditFormTemplateControl("cmbCateg") as ASPxComboBox;

                dr["IdCategory"] = cmbCateg.Value ?? DBNull.Value;
                dr["DateAttach"] = txtDataDoc.Value ?? DBNull.Value;
                dr["DescrAttach"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Atasamente"] as metaUploadFile;               
                if (itm != null)
                {
                    dr["Attach"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Atasamente", dr["IdAuto"]);
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                }

                Session["DocUpload_MP_Atasamente"] = null;

                e.Cancel = true;
                grDateAtasamente.CancelEdit();
                grDateAtasamente.KeyFieldName = "IdAuto";
                grDateAtasamente.DataSource = ds.Tables["Atasamente"];                
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateAtasamente_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Atasamente"].Rows.Find(keys);

                row.Delete();

                Session["DocUpload_MP_Atasamente"] = null;

                e.Cancel = true;
                grDateAtasamente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateAtasamente.DataSource = ds.Tables["Atasamente"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void btnDocUploadAtas_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_MP_Atasamente"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAtasamente_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbCateg = grDateAtasamente.FindEditFormTemplateControl("cmbCateg") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    string sql = @"SELECT * FROM ""CategoriiAtasamente"" ";
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("CategoriiAtasamente", "IdCategory");
                    DataTable dtCateg = General.IncarcaDT(sql, null);
                    cmbCateg.DataSource = dtCateg;
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