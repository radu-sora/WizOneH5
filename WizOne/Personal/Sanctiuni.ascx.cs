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

                if (!IsPostBack)
                    Session["DocUpload_MP_Sanctiuni"] = null;

                if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateSanctiuni);

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
                DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Sanctiuni"))
                {
                    dt = ds.Tables["Admin_Sanctiuni"];
                }
                else
                {
                    dt = General.IncarcaDT($@"SELECT * FROM ""Admin_Sanctiuni"" WHERE ""Marca""=@1", new object[] { HttpContext.Current.Session["Marca"] });
                    dt.TableName = "Admin_Sanctiuni";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);

                    //DataTable dtGen = General.IncarcaDT(@"SELECT * FROM ""Admin_Sanctiuni"" ", null);
                    //Session["Admin_Sanctiuni_General"] = dtGen;
                }
                grDateSanctiuni.KeyFieldName = "IdAuto";
                grDateSanctiuni.DataSource = dt;

                DataTable dtSanc = General.GetObiecteDinArie("ArieTabSanctiuniDinPersonal");
                GridViewDataComboBoxColumn colSanc = (grDateSanctiuni.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colSanc.PropertiesComboBox.DataSource = dtSanc;

                HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;
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
                DataTable dtGen = Session["Admin_Sanctiuni"] as DataTable;
                if (Constante.tipBD == 1)
                {
                    if (dtGen != null && dtGen.Columns["IdAuto"] != null)
                    {
                        if (dtGen.Rows.Count > 0)
                        {
                            int max = Convert.ToInt32(General.Nz(dtGen.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                            e.NewValues["IdAuto"] = max;
                        }
                        else
                            e.NewValues["IdAuto"] = 1;
                    }
                    else
                        e.NewValues["IdAuto"] = 1;
                }
                else
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Sanctiuni");
                e.NewValues["Marca"] = Session["Marca"];
                e.NewValues["DataInceput"] = DateTime.Now;

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
                //DataTable dtGen = Session["Admin_Sanctiuni_General"] as DataTable;
                DataTable dt = ds.Tables["Admin_Sanctiuni"];
                DataRow dr = dt.NewRow();

                ASPxMemo txtDesc = grDateSanctiuni.FindEditFormTemplateControl("txtDesc") as ASPxMemo;
                ASPxDateEdit txtDataInc = grDateSanctiuni.FindEditFormTemplateControl("txtDataInc") as ASPxDateEdit;
                ASPxDateEdit txtDataSf = grDateSanctiuni.FindEditFormTemplateControl("txtDataSf") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateSanctiuni.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxSpinEdit txtVal = grDateSanctiuni.FindEditFormTemplateControl("txtVal") as ASPxSpinEdit;
                ASPxSpinEdit txtProc = grDateSanctiuni.FindEditFormTemplateControl("txtProc") as ASPxSpinEdit;
                ASPxSpinEdit txtNrInregSesizare = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregSesizare") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregSesizare = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregSesizare") as ASPxDateEdit;
                ASPxMemo txtMaterialeDoveditoare = grDateSanctiuni.FindEditFormTemplateControl("txtMaterialeDoveditoare") as ASPxMemo;
                ASPxSpinEdit txtNrInregComisie = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregComisie") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregComisie = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregComisie") as ASPxDateEdit;
                ASPxSpinEdit txtNrInregConvocare = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregConvocare") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregConvocare = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregConvocare") as ASPxDateEdit;
                ASPxDateEdit txtDataCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtDataCercetare") as ASPxDateEdit;
                ASPxSpinEdit txtNrProcesCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtNrProcesCercetare") as ASPxSpinEdit;
                ASPxDateEdit txtDataProcesCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtDataProcesCercetare") as ASPxDateEdit;
                ASPxSpinEdit txtNrDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtNrDecizie") as ASPxSpinEdit;
                ASPxDateEdit txtDataDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtDataDecizie") as ASPxDateEdit;
                ASPxMemo txtComponentaComisie = grDateSanctiuni.FindEditFormTemplateControl("txtComponentaComisie") as ASPxMemo;
                ASPxDateEdit txtDataComunicareDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtDataComunicareDecizie") as ASPxDateEdit;
                ASPxDateEdit txtDataRadiereSanctiune = grDateSanctiuni.FindEditFormTemplateControl("txtDataRadiereSanctiune") as ASPxDateEdit;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Sanctiuni");
                if (Convert.ToInt32(dr["IdAuto"].ToString()) < 1000000)
                    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1000000;
                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataInceput"] = txtDataInc.Value ?? DBNull.Value;
                dr["DataSfarsit"] = txtDataSf.Value ?? DBNull.Value;
                dr["ValoareAbsoluta"] = txtVal.Value ?? DBNull.Value;
                dr["ValoareProcent"] = txtProc.Value ?? DBNull.Value;
                dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                dr["NrInregSesizare"] = txtNrInregSesizare.Value ?? DBNull.Value;
                dr["DataInregSesizare"] = txtDataInregSesizare.Value ?? DBNull.Value;
                dr["MaterialeDoveditoare"] = txtMaterialeDoveditoare.Value ?? DBNull.Value;
                dr["NrInregComisie"] = txtNrInregComisie.Value ?? DBNull.Value;
                dr["DataInregComisie"] = txtDataInregComisie.Value ?? DBNull.Value;
                dr["NrInregConvocare"] = txtNrInregConvocare.Value ?? DBNull.Value;
                dr["DataInregConvocare"] = txtDataInregConvocare.Value ?? DBNull.Value;
                dr["DataCercetare"] = txtDataCercetare.Value ?? DBNull.Value;
                dr["NrProcesCercetare"] = txtNrProcesCercetare.Value ?? DBNull.Value;
                dr["DataProcesCercetare"] = txtDataProcesCercetare.Value ?? DBNull.Value;
                dr["NrDecizie"] = txtNrDecizie.Value ?? DBNull.Value;
                dr["DataDecizie"] = txtDataDecizie.Value ?? DBNull.Value;
                dr["ComponentaComisie"] = txtComponentaComisie.Value ?? DBNull.Value;
                dr["DataComunicareDecizie"] = txtDataComunicareDecizie.Value ?? DBNull.Value;
                dr["DataRadiereSanctiune"] = txtDataRadiereSanctiune.Value ?? DBNull.Value;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Sanctiuni"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Sanctiuni", Convert.ToInt32(dr["IdAuto"].ToString()) + (Constante.tipBD == 1 ? 0 : 1));
                    //if (Constante.tipBD == 2)
                    //    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1;
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Sanctiuni"] = lstFiles;
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
                ASPxSpinEdit txtVal = grDateSanctiuni.FindEditFormTemplateControl("txtVal") as ASPxSpinEdit;
                ASPxSpinEdit txtProc = grDateSanctiuni.FindEditFormTemplateControl("txtProc") as ASPxSpinEdit;
                ASPxSpinEdit txtNrInregSesizare = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregSesizare") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregSesizare = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregSesizare") as ASPxDateEdit;
                ASPxMemo txtMaterialeDoveditoare = grDateSanctiuni.FindEditFormTemplateControl("txtMaterialeDoveditoare") as ASPxMemo;
                ASPxSpinEdit txtNrInregComisie = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregComisie") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregComisie = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregComisie") as ASPxDateEdit;
                ASPxSpinEdit txtNrInregConvocare = grDateSanctiuni.FindEditFormTemplateControl("txtNrInregConvocare") as ASPxSpinEdit;
                ASPxDateEdit txtDataInregConvocare = grDateSanctiuni.FindEditFormTemplateControl("txtDataInregConvocare") as ASPxDateEdit;
                ASPxDateEdit txtDataCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtDataCercetare") as ASPxDateEdit;
                ASPxSpinEdit txtNrProcesCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtNrProcesCercetare") as ASPxSpinEdit;
                ASPxDateEdit txtDataProcesCercetare = grDateSanctiuni.FindEditFormTemplateControl("txtDataProcesCercetare") as ASPxDateEdit;
                ASPxSpinEdit txtNrDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtNrDecizie") as ASPxSpinEdit;
                ASPxDateEdit txtDataDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtDataDecizie") as ASPxDateEdit;
                ASPxMemo txtComponentaComisie = grDateSanctiuni.FindEditFormTemplateControl("txtComponentaComisie") as ASPxMemo;
                ASPxDateEdit txtDataComunicareDecizie = grDateSanctiuni.FindEditFormTemplateControl("txtDataComunicareDecizie") as ASPxDateEdit;
                ASPxDateEdit txtDataRadiereSanctiune = grDateSanctiuni.FindEditFormTemplateControl("txtDataRadiereSanctiune") as ASPxDateEdit;

                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataInceput"] = txtDataInc.Value ?? DBNull.Value;
                dr["DataSfarsit"] = txtDataSf.Value ?? DBNull.Value;
                dr["ValoareAbsoluta"] = txtVal.Value ?? DBNull.Value;
                dr["ValoareProcent"] = txtProc.Value ?? DBNull.Value;
                dr["Descriere"] = txtDesc.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;
                dr["NrInregSesizare"] = txtNrInregSesizare.Value ?? DBNull.Value;
                dr["DataInregSesizare"] = txtDataInregSesizare.Value ?? DBNull.Value;
                dr["MaterialeDoveditoare"] = txtMaterialeDoveditoare.Value ?? DBNull.Value;
                dr["NrInregComisie"] = txtNrInregComisie.Value ?? DBNull.Value;
                dr["DataInregComisie"] = txtDataInregComisie.Value ?? DBNull.Value;
                dr["NrInregConvocare"] = txtNrInregConvocare.Value ?? DBNull.Value;
                dr["DataInregConvocare"] = txtDataInregConvocare.Value ?? DBNull.Value;
                dr["DataCercetare"] = txtDataCercetare.Value ?? DBNull.Value;
                dr["NrProcesCercetare"] = txtNrProcesCercetare.Value ?? DBNull.Value;
                dr["DataProcesCercetare"] = txtDataProcesCercetare.Value ?? DBNull.Value;
                dr["NrDecizie"] = txtNrDecizie.Value ?? DBNull.Value;
                dr["DataDecizie"] = txtDataDecizie.Value ?? DBNull.Value;
                dr["ComponentaComisie"] = txtComponentaComisie.Value ?? DBNull.Value;
                dr["DataComunicareDecizie"] = txtDataComunicareDecizie.Value ?? DBNull.Value;
                dr["DataRadiereSanctiune"] = txtDataRadiereSanctiune.Value ?? DBNull.Value;

                metaUploadFile itm = Session["DocUpload_MP_Sanctiuni"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Sanctiuni", dr["IdAuto"]);
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    if (lstFiles.ContainsKey(Convert.ToInt32(idAuto.ToString())))
                        lstFiles[Convert.ToInt32(idAuto.ToString())] = itm;
                    else
                        lstFiles.Add(Convert.ToInt32(idAuto.ToString()), itm);
                    Session["List_DocUpload_MP_Sanctiuni"] = lstFiles;
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

                Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, metaUploadFile>;
                if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                    lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                Session["List_DocUpload_MP_Sanctiuni"] = lstFiles;

                row.Delete();

                Session["FisiereDeSters"] = General.Nz(Session["FisiereDeSters"], "").ToString() + ";" + General.Nz(General.ExecutaScalar($@"SELECT '{Constante.fisiereApp}/Sanctiuni/' {Dami.Operator()} ""FisierNume"" FROM ""tblFisiere"" WHERE ""Tabela""='Admin_Medicina' AND ""Id""={keys[0]}"), "").ToString();

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
                General.SecuritatePersonal(grDateSanctiuni, "Sanctiuni", Convert.ToInt32(Session["UserId"].ToString()), true);

                ASPxComboBox cmbCateg = grDateSanctiuni.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtSanc = General.GetObiecteDinArie("ArieTabSanctiuniDinPersonal");
                    cmbCateg.DataSource = dtSanc;
                    cmbCateg.DataBindItems();
                }

                HtmlTableCell lblDesc = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDesc");
                lblDesc.InnerText = Dami.TraduCuvant("Descriere");
                HtmlTableCell lblSanc = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblSanc");
                lblSanc.InnerText = Dami.TraduCuvant("Sanctiune aplicata");
                HtmlTableCell lblDataCercet = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataCercet");
                lblDataCercet.InnerText = Dami.TraduCuvant("Data cercetarii");
                HtmlTableCell lblDataInc = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataInc");
                lblDataInc.InnerText = Dami.TraduCuvant("Data inceput");
                HtmlTableCell lblDataSf = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataSf");
                lblDataSf.InnerText = Dami.TraduCuvant("Data sfarsit");
                HtmlTableCell lblVal = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblVal");
                lblVal.InnerText = Dami.TraduCuvant("Valoare sanctiune");
                HtmlTableCell lblValProc = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblValProc");
                lblValProc.InnerText = Dami.TraduCuvant("Valoare%");
                HtmlTableCell lblMatDov = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblMatDov");
                lblMatDov.InnerText = Dami.TraduCuvant("Materiale doveditoare");
                HtmlTableCell lblNrInregSes = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblNrInregSes");
                lblNrInregSes.InnerText = Dami.TraduCuvant("Numar inregistrare sesizare");
                HtmlTableCell lblNrInregConv = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblNrInregConv");
                lblNrInregConv.InnerText = Dami.TraduCuvant("Numar inregistrare convocare");
                HtmlTableCell lblNrDecizie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblNrDecizie");
                lblNrDecizie.InnerText = Dami.TraduCuvant("Numar decizie");
                HtmlTableCell lblDataInregSes = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataInregSes");
                lblDataInregSes.InnerText = Dami.TraduCuvant("Data inregistrare sesizare");
                HtmlTableCell lblDataInregConv = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataInregConv");
                lblDataInregConv.InnerText = Dami.TraduCuvant("Data inregistrare convocare");
                HtmlTableCell lblDataDecizie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataDecizie");
                lblDataDecizie.InnerText = Dami.TraduCuvant("Data decizie");
                HtmlTableCell lblNrInregComisie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblNrInregComisie");
                lblNrInregComisie.InnerText = Dami.TraduCuvant("Numar inregistrare comisie");
                HtmlTableCell lblNrProcVerb = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblNrProcVerb");
                lblNrProcVerb.InnerText = Dami.TraduCuvant("Numar proces verbal");
                HtmlTableCell lblDataComDecizie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataComDecizie");
                lblDataComDecizie.InnerText = Dami.TraduCuvant("Data comunicare decizie");
                HtmlTableCell lblDataInregComisie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataInregComisie");
                lblDataInregComisie.InnerText = Dami.TraduCuvant("Data inregistrare comisie");
                HtmlTableCell lblDataProcVerb = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataProcVerb");
                lblDataProcVerb.InnerText = Dami.TraduCuvant("Data proces verbal");
                HtmlTableCell lblDataRadSanc = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblDataRadSanc");
                lblDataRadSanc.InnerText = Dami.TraduCuvant("Data radiere sanctiune");
                HtmlTableCell lblCompComisie = (HtmlTableCell)grDateSanctiuni.FindEditFormTemplateControl("lblCompComisie");
                lblCompComisie.InnerText = Dami.TraduCuvant("Componenta comisiei");

                ASPxUploadControl btnDocUploadSanc = (ASPxUploadControl)grDateSanctiuni.FindEditFormTemplateControl("btnDocUploadSanc");
                btnDocUploadSanc.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUploadSanc.ToolTip = Dami.TraduCuvant("Incarca Document");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}