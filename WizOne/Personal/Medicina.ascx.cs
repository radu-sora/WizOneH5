using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Medicina : System.Web.UI.UserControl
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
                grDateMed.DataBind();

                foreach (dynamic c in grDateMed.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                grDateMed.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateMed.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateMed.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateMed.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateMed.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

                if (!IsPostBack)                
                    Session["DocUpload_MP_Medicina"] = null;                

                if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateMed);

                GridViewDataDateColumn colData = (grDateMed.Columns["DataElibControlMed"] as GridViewDataDateColumn);
                colData.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colData = (grDateMed.Columns["DataUrmControl"] as GridViewDataDateColumn);
                colData.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataTextColumn colText = (grDateMed.Columns["PerioadaValab"] as GridViewDataTextColumn);
                colText.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataCheckColumn colChk = (grDateMed.Columns["Risc1"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMed.Columns["Risc2"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMed.Columns["Risc3"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMed.Columns["Risc4"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMed.Columns["Risc5"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataComboBoxColumn colCmb = (grDateMed.Columns["Manager"] as GridViewDataComboBoxColumn);
                colCmb.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMed_DataBinding(object sender, EventArgs e)
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
                DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Medicina"))
                {
                    dt = ds.Tables["Admin_Medicina"];
                }
                else
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Admin_Medicina"" WHERE ""Marca""=@1", new object[] { HttpContext.Current.Session["Marca"] });
                    dt.TableName = "Admin_Medicina";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);
                }
                grDateMed.KeyFieldName = "IdAuto";
                grDateMed.DataSource = dt;

                DataTable dtMedicina = General.GetObiecteDinArie("ArieTabMedicinaDinPersonal");
                GridViewDataComboBoxColumn colMedicina = (grDateMed.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colMedicina.PropertiesComboBox.DataSource = dtMedicina;

                DataTable dtLocatie = General.IncarcaDT("SELECT * FROM \"tblLocatieMedMuncii\" ORDER BY\"Denumire\"", null);
                GridViewDataComboBoxColumn colLocatie = (grDateMed.Columns["IdLocatie"] as GridViewDataComboBoxColumn);
                colLocatie.PropertiesComboBox.DataSource = dtLocatie;

                DataTable dtManager = General.IncarcaDT("SELECT * FROM \"tblManagerMedMuncii\" ORDER BY\"Denumire\"", null);
                GridViewDataComboBoxColumn colManager = (grDateMed.Columns["Manager"] as GridViewDataComboBoxColumn);
                colManager.PropertiesComboBox.DataSource = dtManager;

                DataTable dtSectAlim = General.GetPrelungireContract();
                GridViewDataComboBoxColumn colSectAlim = (grDateMed.Columns["SectorAlim"] as GridViewDataComboBoxColumn);
                colSectAlim.PropertiesComboBox.DataSource = dtSectAlim;

                DataTable dtRez = General.ListaRezultatExamen();
                GridViewDataComboBoxColumn colRez = (grDateMed.Columns["RezultatExamen"] as GridViewDataComboBoxColumn);
                colRez.PropertiesComboBox.DataSource = dtRez;

                HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMed_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Medicina"];
                if (Constante.tipBD == 1)
                {
                    if (dt != null && dt.Columns["IdAuto"] != null)
                    {
                        if (dt.Rows.Count > 0)
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Medicina");
                e.NewValues["Marca"] = Session["Marca"];
                e.NewValues["DataElib"] = DateTime.Now;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMed_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataTable dt = ds.Tables["Admin_Medicina"];
                //DataTable dtGen = Session["Admin_Medicina_General"] as DataTable;
                DataRow dr = dt.NewRow();

                ASPxMemo txtObs = grDateMed.FindEditFormTemplateControl("txtObs") as ASPxMemo;
                ASPxDateEdit txtDataElib = grDateMed.FindEditFormTemplateControl("txtDataElib") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateMed.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateMed.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtSerie =  grDateMed.FindEditFormTemplateControl("txtSerie") as ASPxTextBox;
                ASPxTextBox txtEmi = grDateMed.FindEditFormTemplateControl("txtEmi") as ASPxTextBox;

                ASPxDateEdit txtDataElibCtrlMed = grDateMed.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                ASPxTextBox txtValab = grDateMed.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                ASPxDateEdit txtDataUrmCtrl = grDateMed.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
                ASPxComboBox cmbLocatie = grDateMed.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                ASPxComboBox cmbManager = grDateMed.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                ASPxComboBox cmbSectAlim = grDateMed.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                //ASPxComboBox cmbRiscuri = grDateMed.FindEditFormTemplateControl("cmbRiscuri") as ASPxComboBox;
                ASPxTextBox txtAlteRiscuri = grDateMed.FindEditFormTemplateControl("txtAlteRiscuri") as ASPxTextBox;
                ASPxComboBox cmbRez = grDateMed.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                ASPxCheckBox chk1 = grDateMed.FindEditFormTemplateControl("chk1") as ASPxCheckBox;
                ASPxCheckBox chk2 = grDateMed.FindEditFormTemplateControl("chk2") as ASPxCheckBox;
                ASPxCheckBox chk3 = grDateMed.FindEditFormTemplateControl("chk3") as ASPxCheckBox;
                ASPxCheckBox chk4 = grDateMed.FindEditFormTemplateControl("chk4") as ASPxCheckBox;
                ASPxCheckBox chk5 = grDateMed.FindEditFormTemplateControl("chk5") as ASPxCheckBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Medicina");
                if (Convert.ToInt32(dr["IdAuto"].ToString()) < 1000000)
                    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1000000;

                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataElib"] = txtDataElib.Value ?? DBNull.Value;
                dr["DataExp"] = txtDataExp.Value ?? DBNull.Value;
                dr["Emitent"] = txtEmi.Value ?? DBNull.Value;
                dr["SerieNrDoc"] = txtSerie.Value ?? DBNull.Value;
                dr["Observatii"] = txtObs.Value ?? DBNull.Value;

                dr["DataElibControlMed"] = txtDataElibCtrlMed.Value ?? DBNull.Value;
                dr["PerioadaValab"] = txtValab.Value ?? DBNull.Value;
                dr["DataUrmControl"] = txtDataUrmCtrl.Value ?? DBNull.Value;
                dr["IdLocatie"] = cmbLocatie.Value ?? DBNull.Value;
                dr["Manager"] = cmbManager.Value ?? DBNull.Value;
                dr["SectorAlim"] = cmbSectAlim.Value ?? DBNull.Value;
                //dr["Riscuri"] = FiltruRiscuri(checkComboBoxRiscuri.Value.ToString().Replace(";", ",")).Replace(";", ",").Substring(0, FiltruRiscuri(checkComboBoxRiscuri.Value.ToString()).Length - 1);
                dr["AlteRiscuri"] = txtAlteRiscuri.Value ?? DBNull.Value;
                dr["RezultatExamen"] = cmbRez.Value ?? DBNull.Value;
                dr["Risc1"] = chk1.Checked ? 1 : 0;
                dr["Risc2"] = chk2.Checked ? 1 : 0;
                dr["Risc3"] = chk3.Checked ? 1 : 0;
                dr["Risc4"] = chk4.Checked ? 1 : 0;
                dr["Risc5"] = chk5.Checked ? 1 : 0;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Medicina"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Medicina", Convert.ToInt32(dr["IdAuto"].ToString()) + (Constante.tipBD == 1 ? 0 : 1));
                    //if (Constante.tipBD == 2)
                    //    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1;
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Medicina"] = lstFiles;
                }

                ds.Tables["Admin_Medicina"].Rows.Add(dr);
                Session["DocUpload_MP_Medicina"] = null;

                e.Cancel = true;
                grDateMed.CancelEdit();
                grDateMed.DataSource = ds.Tables["Admin_Medicina"];
                grDateMed.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMed_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Medicina"].Rows.Find(idAuto);

                ASPxMemo txtObs = grDateMed.FindEditFormTemplateControl("txtObs") as ASPxMemo;
                ASPxDateEdit txtDataElib = grDateMed.FindEditFormTemplateControl("txtDataElib") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateMed.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateMed.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtSerie = grDateMed.FindEditFormTemplateControl("txtSerie") as ASPxTextBox;
                ASPxTextBox txtEmi = grDateMed.FindEditFormTemplateControl("txtEmi") as ASPxTextBox;

                ASPxDateEdit txtDataElibCtrlMed = grDateMed.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                ASPxTextBox txtValab = grDateMed.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                ASPxDateEdit txtDataUrmCtrl = grDateMed.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
                ASPxComboBox cmbLocatie = grDateMed.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                ASPxComboBox cmbManager = grDateMed.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                ASPxComboBox cmbSectAlim = grDateMed.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                //ASPxComboBox cmbRiscuri = grDateMed.FindEditFormTemplateControl("cmbRiscuri") as ASPxComboBox;
                ASPxTextBox txtAlteRiscuri = grDateMed.FindEditFormTemplateControl("txtAlteRiscuri") as ASPxTextBox;
                ASPxComboBox cmbRez = grDateMed.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                ASPxCheckBox chk1 = grDateMed.FindEditFormTemplateControl("chk1") as ASPxCheckBox;
                ASPxCheckBox chk2 = grDateMed.FindEditFormTemplateControl("chk2") as ASPxCheckBox;
                ASPxCheckBox chk3 = grDateMed.FindEditFormTemplateControl("chk3") as ASPxCheckBox;
                ASPxCheckBox chk4 = grDateMed.FindEditFormTemplateControl("chk4") as ASPxCheckBox;
                ASPxCheckBox chk5 = grDateMed.FindEditFormTemplateControl("chk5") as ASPxCheckBox;

                dr["IdObiect"] = cmbObi.Value ?? DBNull.Value;
                dr["DataElib"] = txtDataElib.Value ?? DBNull.Value;
                dr["DataExp"] = txtDataExp.Value ?? DBNull.Value;
                dr["Emitent"] = txtEmi.Value ?? DBNull.Value;
                dr["SerieNrDoc"] = txtSerie.Value ?? DBNull.Value;
                dr["Observatii"] = txtObs.Value ?? DBNull.Value;

                dr["DataElibControlMed"] = txtDataElibCtrlMed.Value ?? DBNull.Value;
                dr["PerioadaValab"] = txtValab.Value ?? DBNull.Value;
                dr["DataUrmControl"] = txtDataUrmCtrl.Value ?? DBNull.Value;
                dr["IdLocatie"] = cmbLocatie.Value ?? DBNull.Value;
                dr["Manager"] = cmbManager.Value ?? DBNull.Value;
                dr["SectorAlim"] = cmbSectAlim.Value ?? DBNull.Value;                
                dr["AlteRiscuri"] = txtAlteRiscuri.Value ?? DBNull.Value;
                dr["RezultatExamen"] = cmbRez.Value ?? DBNull.Value;
                dr["Risc1"] = chk1.Checked ? 1 : 0;
                dr["Risc2"] = chk2.Checked ? 1 : 0;
                dr["Risc3"] = chk3.Checked ? 1 : 0;
                dr["Risc4"] = chk4.Checked ? 1 : 0;
                dr["Risc5"] = chk5.Checked ? 1 : 0;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Medicina"] as metaUploadFile;
                if (itm != null)
                {
                    //General.IncarcaFisier(itm.UploadedFileName.ToString(), itm.UploadedFile, "Admin_Medicina", dr["IdAuto"]);
                    //dr["Fisier"] = itm.UploadedFile;
                    //dr["FisierNume"] = itm.UploadedFileName;
                    //dr["FisierExtensie"] = itm.UploadedFileExtension;
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    if (lstFiles.ContainsKey(Convert.ToInt32(idAuto.ToString())))
                        lstFiles[Convert.ToInt32(idAuto.ToString())] = itm;
                    else
                        lstFiles.Add(Convert.ToInt32(idAuto.ToString()), itm);
                    Session["List_DocUpload_MP_Medicina"] = lstFiles;
                }
                Session["DocUpload_MP_Medicina"] = null;

                e.Cancel = true;
                grDateMed.CancelEdit();
                grDateMed.DataSource = ds.Tables["Admin_Medicina"];
                grDateMed.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMed_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Medicina"].Rows.Find(keys);    

                Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, metaUploadFile>;
                if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                    lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                Session["List_DocUpload_MP_Medicina"] = lstFiles;

                Session["DocUpload_MP_Medicina"] = null;

                row.Delete();

                Session["FisiereDeSters"] = General.Nz(Session["FisiereDeSters"],"").ToString() + ";" + General.Nz(General.ExecutaScalar($@"SELECT '{Constante.fisiereApp}/Medicina/' {Dami.Operator()} ""FisierNume"" FROM ""tblFisiere"" WHERE ""Tabela""='Admin_Medicina' AND ""Id""={keys[0]}"),"").ToString();

                e.Cancel = true;
                grDateMed.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateMed.DataSource = ds.Tables["Admin_Medicina"];
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

                Session["DocUpload_MP_Medicina"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMed_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                General.SecuritatePersonal(grDateMed, "Medicina", Convert.ToInt32(Session["UserId"].ToString()), true);

                ASPxComboBox cmbCateg = grDateMed.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtMedicina = General.GetObiecteDinArie("ArieTabMedicinaDinPersonal");
                    cmbCateg.DataSource = dtMedicina;
                    cmbCateg.DataBindItems();
                }

                ASPxComboBox cmbLocatie = grDateMed.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                if (cmbLocatie != null)
                {
                    DataTable dtLocatie = General.IncarcaDT("SELECT * FROM \"tblLocatieMedMuncii\" ORDER BY\"Denumire\"", null);
                    cmbLocatie.DataSource = dtLocatie;
                    cmbLocatie.DataBindItems();
                }

                ASPxComboBox cmbManager = grDateMed.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                if (cmbManager != null)
                {
                    DataTable dtManager = General.IncarcaDT("SELECT * FROM \"tblManagerMedMuncii\" ORDER BY\"Denumire\"", null);
                    cmbManager.DataSource = dtManager;
                    cmbManager.DataBindItems();
                }

                ASPxComboBox cmbSectAlim = grDateMed.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                if (cmbSectAlim != null)
                {
                    DataTable dtSectAlim = General.GetPrelungireContract();
                    cmbSectAlim.DataSource = dtSectAlim;
                    cmbSectAlim.DataBindItems();
                }

                ASPxComboBox cmbRez = grDateMed.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                if (cmbRez != null)
                {
                    DataTable dtRez = General.ListaRezultatExamen();
                    cmbRez.DataSource = dtRez;
                    cmbRez.DataBindItems();
                }

                HtmlTableCell lblObs = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblObs");
                lblObs.InnerText = Dami.TraduCuvant("Observatii");
                HtmlTableCell lblMedMunc = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblMedMunc");
                lblMedMunc.InnerText = Dami.TraduCuvant("Medicina muncii/PSI");
                HtmlTableCell lblDataElib = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblDataElib");
                lblDataElib.InnerText = Dami.TraduCuvant("Data eliberarii");
                HtmlTableCell lblDataExp = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblDataExp");
                lblDataExp.InnerText = Dami.TraduCuvant("Data expirarii");
                HtmlTableCell lblSerieNr = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblSerieNr");
                lblSerieNr.InnerText = Dami.TraduCuvant("Serie si nr. doc.");
                HtmlTableCell lblEmitent = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblEmitent");
                lblEmitent.InnerText = Dami.TraduCuvant("Emitent");
                HtmlTableCell lblDataElibCtrlMed = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblDataElibCtrlMed");
                lblDataElibCtrlMed.InnerText = Dami.TraduCuvant("Data eliberare control medical");
                HtmlTableCell lblPerValab = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblPerValab");
                lblPerValab.InnerText = Dami.TraduCuvant("Perioada valabilitate");
                HtmlTableCell lblDataUrmCtrl = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblDataUrmCtrl");
                lblDataUrmCtrl.InnerText = Dami.TraduCuvant("Data urmatorului control");
                HtmlTableCell lblLocatie = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblLocatie");
                lblLocatie.InnerText = Dami.TraduCuvant("Locatie");
                HtmlTableCell lblManagerDir = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblManagerDir");
                lblManagerDir.InnerText = Dami.TraduCuvant("Manager direct");
                HtmlTableCell lblSectAlim = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblSectAlim");
                lblSectAlim.InnerText = Dami.TraduCuvant("Sector alimentar");
                HtmlTableCell lblRezExamen = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblRezExamen");
                lblRezExamen.InnerText = Dami.TraduCuvant("Rezultat examen");
                HtmlTableCell lblAlteRisc = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblAlteRisc");
                lblAlteRisc.InnerText = Dami.TraduCuvant("Alte riscuri");
                HtmlTableCell lblRiscuri = (HtmlTableCell)grDateMed.FindEditFormTemplateControl("lblRiscuri");
                lblRiscuri.InnerText = Dami.TraduCuvant("Riscuri");

                ASPxUploadControl btnDocUploadAtas = (ASPxUploadControl)grDateMed.FindEditFormTemplateControl("btnDocUploadAtas");
                btnDocUploadAtas.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUploadAtas.ToolTip = Dami.TraduCuvant("Incarca Document");

                ASPxCheckBox chk1 = grDateMed.FindEditFormTemplateControl("chk1") as ASPxCheckBox;
                chk1.Text = Dami.TraduCuvant("Auto/ Categoria ...");
                ASPxCheckBox chk2 = grDateMed.FindEditFormTemplateControl("chk2") as ASPxCheckBox;
                chk2.Text = Dami.TraduCuvant("Lucrul la inaltime");
                ASPxCheckBox chk3 = grDateMed.FindEditFormTemplateControl("chk3") as ASPxCheckBox;
                chk3.Text = Dami.TraduCuvant("Lucrul in ture de noapte");
                ASPxCheckBox chk4 = grDateMed.FindEditFormTemplateControl("chk4") as ASPxCheckBox;
                chk4.Text = Dami.TraduCuvant("Lucrul la casca");
                ASPxCheckBox chk5 = grDateMed.FindEditFormTemplateControl("chk5") as ASPxCheckBox;
                chk5.Text = Dami.TraduCuvant("Zgomot");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMed_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {

        }

        protected void grDateMed_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;

                switch (str)
                {
                    case "valab":
                        ASPxDateEdit txtDataElibCtrlMed = grDateMed.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                        ASPxTextBox txtValab = grDateMed.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                        ASPxDateEdit txtDataUrmCtrl = grDateMed.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
                        int rez = 0;
                        if (txtDataElibCtrlMed.Value != null && txtValab.Text.Length > 0 && int.TryParse(txtValab.Text, out rez))
                            txtDataUrmCtrl.Value = Convert.ToDateTime(txtDataElibCtrlMed.Value).AddDays(rez);
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}