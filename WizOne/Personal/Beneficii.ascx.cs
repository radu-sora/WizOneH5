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
using System.Web.UI.HtmlControls;

namespace WizOne.Personal
{
    public partial class Beneficii : System.Web.UI.UserControl
    {
        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grDateBeneficii.DataBind();

            foreach (dynamic c in grDateBeneficii.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateBeneficii.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateBeneficii.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateBeneficii.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateBeneficii.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateBeneficii.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (!IsPostBack)
                Session["DocUpload_MP_Beneficii"] = null;

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateBeneficii);
        }

        protected void grDateBeneficii_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Beneficii\" WHERE \"Marca\" = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Beneficii"))
            {
                dt = ds.Tables["Admin_Beneficii"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Beneficii";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateBeneficii.KeyFieldName = "IdAuto";
            grDateBeneficii.DataSource = dt;
            
            DataTable dtBen = General.GetObiecteDinArie("ArieTabBeneficiiDinPersonal");
            GridViewDataComboBoxColumn colBen = (grDateBeneficii.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colBen.PropertiesComboBox.DataSource = dtBen;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;

        }

        protected void grDateBeneficii_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Beneficii"];
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
                    else
                        e.NewValues["IdAuto"] = 1;
                }
                else
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Beneficii");
                e.NewValues["Marca"] = Session["Marca"];
                e.NewValues["DataPrimire"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDateBeneficii_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Beneficii"];
                DataRow dr = dt.NewRow();

                ASPxDateEdit txtDataPrim = grDateBeneficii.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateBeneficii.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateBeneficii.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateBeneficii.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Beneficii");
                if (Convert.ToInt32(dr["IdAuto"].ToString()) < 1000000)
                    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1000000;
                               
                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                dr["DataPrimire"] = txtDataPrim.Value ?? DBNull.Value;
                dr["DataExpirare"] = txtDataExp.Value ?? DBNull.Value;     
                dr["Caracteristica"] = txtCaract.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Beneficii"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Medicina", Convert.ToInt32(dr["IdAuto"].ToString()) + (Constante.tipBD == 1 ? 0 : 1));
                    //if (Constante.tipBD == 2)
                    //    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1;
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Beneficii"] = lstFiles;
                }

                ds.Tables["Admin_Beneficii"].Rows.Add(dr);
                Session["DocUpload_MP_Beneficii"] = null;

                e.Cancel = true;
                grDateBeneficii.CancelEdit();
                grDateBeneficii.DataSource = ds.Tables["Admin_Beneficii"];
                grDateBeneficii.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateBeneficii_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Beneficii"].Rows.Find(idAuto);

                ASPxDateEdit txtDataPrim = grDateBeneficii.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateBeneficii.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateBeneficii.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateBeneficii.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;

                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                dr["DataPrimire"] = txtDataPrim.Value ?? DBNull.Value;
                dr["DataExpirare"] = txtDataExp.Value ?? DBNull.Value;
                dr["Caracteristica"] = txtCaract.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Beneficii"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Medicina", dr["IdAuto"]);
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    if (lstFiles.ContainsKey(Convert.ToInt32(idAuto.ToString())))
                        lstFiles[Convert.ToInt32(idAuto.ToString())] = itm;
                    else
                        lstFiles.Add(Convert.ToInt32(idAuto.ToString()), itm);
                    Session["List_DocUpload_MP_Beneficii"] = lstFiles;
                }
                Session["DocUpload_MP_Beneficii"] = null;

                e.Cancel = true;
                grDateBeneficii.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateBeneficii.DataSource = ds.Tables["Admin_Beneficii"];
                grDateBeneficii.KeyFieldName = "IdAuto";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateBeneficii_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Beneficii"].Rows.Find(keys);

                Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, metaUploadFile>;
                if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                    lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                Session["List_DocUpload_MP_Beneficii"] = lstFiles;

                Session["DocUpload_MP_MBeneficii"] = null;

                row.Delete();

                Session["FisiereDeSters"] = General.Nz(Session["FisiereDeSters"], "").ToString() + ";" + General.Nz(General.ExecutaScalar($@"SELECT '{Constante.fisiereApp}/Beneficii/' {Dami.Operator()} ""FisierNume"" FROM ""tblFisiere"" WHERE ""Tabela""='Admin_Medicina' AND ""Id""={keys[0]}"), "").ToString();

                e.Cancel = true;
                grDateBeneficii.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateBeneficii.DataSource = ds.Tables["Admin_Beneficii"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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

                Session["DocUpload_MP_Beneficii"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateBeneficii_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbCateg = grDateBeneficii.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtBeneficii = General.GetObiecteDinArie("ArieTabBeneficiiDinPersonal");
                    cmbCateg.DataSource = dtBeneficii;
                    cmbCateg.DataBindItems();
                }

                HtmlTableCell lblNume = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblNume");
                lblNume.InnerText = Dami.TraduCuvant("Nume beneficiu");
                HtmlTableCell lblDataPrimire = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblDataPrimire");
                lblDataPrimire.InnerText = Dami.TraduCuvant("Data primire");
                HtmlTableCell lblDataExp = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblDataExp");
                lblDataExp.InnerText = Dami.TraduCuvant("Data expirare");
                HtmlTableCell lblCaract = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblCaract");
                lblCaract.InnerText = Dami.TraduCuvant("Caracteristica echipament");

                ASPxUploadControl btnDocUploadBen = (ASPxUploadControl)grDateBeneficii.FindEditFormTemplateControl("btnDocUploadBen");
                btnDocUploadBen.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUploadBen.ToolTip = Dami.TraduCuvant("Incarca Document");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}