using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                grDateMedicina.DataBind();

                foreach (dynamic c in grDateMedicina.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }
                grDateMedicina.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
                grDateMedicina.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
                grDateMedicina.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
                grDateMedicina.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
                grDateMedicina.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

                if (!IsPostBack)                
                    Session["DocUpload_MP_Medicina"] = null;                

                if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateMedicina);

                GridViewDataDateColumn colData = (grDateMedicina.Columns["DataElibControlMed"] as GridViewDataDateColumn);
                colData.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colData = (grDateMedicina.Columns["DataUrmControl"] as GridViewDataDateColumn);
                colData.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataTextColumn colText = (grDateMedicina.Columns["PerioadaValab"] as GridViewDataTextColumn);
                colText.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataCheckColumn colChk = (grDateMedicina.Columns["Risc1"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMedicina.Columns["Risc2"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMedicina.Columns["Risc3"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMedicina.Columns["Risc4"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                colChk = (grDateMedicina.Columns["Risc5"] as GridViewDataCheckColumn);
                colChk.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                GridViewDataComboBoxColumn colCmb = (grDateMedicina.Columns["Manager"] as GridViewDataComboBoxColumn);
                colCmb.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMedicina_DataBinding(object sender, EventArgs e)
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
                if (ds.Tables.Contains("Admin_Medicina"))
                {
                    dt = ds.Tables["Admin_Medicina"];
                }
                else
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Admin_Medicina"" WHERE ""Marca""=@1", new object[] { Session["Marca"] });
                    dt.TableName = "Admin_Medicina";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    ds.Tables.Add(dt);

                    //DataTable dtGen = General.IncarcaDT(@"SELECT * FROM ""Admin_Medicina"" ", null);
                    //Session["Admin_Medicina_General"] = dtGen;
                }
                grDateMedicina.KeyFieldName = "IdAuto";
                grDateMedicina.DataSource = dt;

                DataTable dtMedicina = General.GetObiecteDinArie("ArieTabMedicinaDinPersonal");
                GridViewDataComboBoxColumn colMedicina = (grDateMedicina.Columns["IdObiect"] as GridViewDataComboBoxColumn);
                colMedicina.PropertiesComboBox.DataSource = dtMedicina;

                DataTable dtLocatie = General.IncarcaDT("SELECT * FROM \"tblLocatieMedMuncii\" ORDER BY\"Denumire\"", null);
                GridViewDataComboBoxColumn colLocatie = (grDateMedicina.Columns["IdLocatie"] as GridViewDataComboBoxColumn);
                colLocatie.PropertiesComboBox.DataSource = dtLocatie;

                DataTable dtManager = General.IncarcaDT("SELECT * FROM \"tblManagerMedMuncii\" ORDER BY\"Denumire\"", null);
                GridViewDataComboBoxColumn colManager = (grDateMedicina.Columns["Manager"] as GridViewDataComboBoxColumn);
                colManager.PropertiesComboBox.DataSource = dtManager;

                DataTable dtSectAlim = General.GetPrelungireContract();
                GridViewDataComboBoxColumn colSectAlim = (grDateMedicina.Columns["SectorAlim"] as GridViewDataComboBoxColumn);
                colSectAlim.PropertiesComboBox.DataSource = dtSectAlim;

                DataTable dtRez = General.ListaRezultatExamen();
                GridViewDataComboBoxColumn colRez = (grDateMedicina.Columns["RezultatExamen"] as GridViewDataComboBoxColumn);
                colRez.PropertiesComboBox.DataSource = dtRez;

                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMedicina_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
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

        protected void grDateMedicina_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataTable dt = ds.Tables["Admin_Medicina"];
                //DataTable dtGen = Session["Admin_Medicina_General"] as DataTable;
                DataRow dr = dt.NewRow();

                ASPxMemo txtObs = grDateMedicina.FindEditFormTemplateControl("txtObs") as ASPxMemo;
                ASPxDateEdit txtDataElib = grDateMedicina.FindEditFormTemplateControl("txtDataElib") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateMedicina.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateMedicina.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtSerie =  grDateMedicina.FindEditFormTemplateControl("txtSerie") as ASPxTextBox;
                ASPxTextBox txtEmi = grDateMedicina.FindEditFormTemplateControl("txtEmi") as ASPxTextBox;

                ASPxDateEdit txtDataElibCtrlMed = grDateMedicina.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                ASPxTextBox txtValab = grDateMedicina.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                ASPxDateEdit txtDataUrmCtrl = grDateMedicina.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
                ASPxComboBox cmbLocatie = grDateMedicina.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                ASPxComboBox cmbManager = grDateMedicina.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                ASPxComboBox cmbSectAlim = grDateMedicina.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                //ASPxComboBox cmbRiscuri = grDateMedicina.FindEditFormTemplateControl("cmbRiscuri") as ASPxComboBox;
                ASPxTextBox txtAlteRiscuri = grDateMedicina.FindEditFormTemplateControl("txtAlteRiscuri") as ASPxTextBox;
                ASPxComboBox cmbRez = grDateMedicina.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                ASPxCheckBox chk1 = grDateMedicina.FindEditFormTemplateControl("chk1") as ASPxCheckBox;
                ASPxCheckBox chk2 = grDateMedicina.FindEditFormTemplateControl("chk2") as ASPxCheckBox;
                ASPxCheckBox chk3 = grDateMedicina.FindEditFormTemplateControl("chk3") as ASPxCheckBox;
                ASPxCheckBox chk4 = grDateMedicina.FindEditFormTemplateControl("chk4") as ASPxCheckBox;
                ASPxCheckBox chk5 = grDateMedicina.FindEditFormTemplateControl("chk5") as ASPxCheckBox;

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
                grDateMedicina.CancelEdit();
                grDateMedicina.DataSource = ds.Tables["Admin_Medicina"];
                grDateMedicina.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMedicina_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Medicina"].Rows.Find(idAuto);

                ASPxMemo txtObs = grDateMedicina.FindEditFormTemplateControl("txtObs") as ASPxMemo;
                ASPxDateEdit txtDataElib = grDateMedicina.FindEditFormTemplateControl("txtDataElib") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateMedicina.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbObi = grDateMedicina.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                ASPxTextBox txtSerie = grDateMedicina.FindEditFormTemplateControl("txtSerie") as ASPxTextBox;
                ASPxTextBox txtEmi = grDateMedicina.FindEditFormTemplateControl("txtEmi") as ASPxTextBox;

                ASPxDateEdit txtDataElibCtrlMed = grDateMedicina.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                ASPxTextBox txtValab = grDateMedicina.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                ASPxDateEdit txtDataUrmCtrl = grDateMedicina.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
                ASPxComboBox cmbLocatie = grDateMedicina.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                ASPxComboBox cmbManager = grDateMedicina.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                ASPxComboBox cmbSectAlim = grDateMedicina.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                //ASPxComboBox cmbRiscuri = grDateMedicina.FindEditFormTemplateControl("cmbRiscuri") as ASPxComboBox;
                ASPxTextBox txtAlteRiscuri = grDateMedicina.FindEditFormTemplateControl("txtAlteRiscuri") as ASPxTextBox;
                ASPxComboBox cmbRez = grDateMedicina.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                ASPxCheckBox chk1 = grDateMedicina.FindEditFormTemplateControl("chk1") as ASPxCheckBox;
                ASPxCheckBox chk2 = grDateMedicina.FindEditFormTemplateControl("chk2") as ASPxCheckBox;
                ASPxCheckBox chk3 = grDateMedicina.FindEditFormTemplateControl("chk3") as ASPxCheckBox;
                ASPxCheckBox chk4 = grDateMedicina.FindEditFormTemplateControl("chk4") as ASPxCheckBox;
                ASPxCheckBox chk5 = grDateMedicina.FindEditFormTemplateControl("chk5") as ASPxCheckBox;

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
                grDateMedicina.CancelEdit();
                grDateMedicina.DataSource = ds.Tables["Admin_Medicina"];
                grDateMedicina.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateMedicina_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
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

                e.Cancel = true;
                grDateMedicina.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateMedicina.DataSource = ds.Tables["Admin_Medicina"];
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

        protected void grDateMedicina_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbCateg = grDateMedicina.FindEditFormTemplateControl("cmbObi") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    DataTable dtMedicina = General.GetObiecteDinArie("ArieTabMedicinaDinPersonal");
                    cmbCateg.DataSource = dtMedicina;
                    cmbCateg.DataBindItems();
                }

                ASPxComboBox cmbLocatie = grDateMedicina.FindEditFormTemplateControl("cmbLocatie") as ASPxComboBox;
                if (cmbLocatie != null)
                {
                    DataTable dtLocatie = General.IncarcaDT("SELECT * FROM \"tblLocatieMedMuncii\" ORDER BY\"Denumire\"", null);
                    cmbLocatie.DataSource = dtLocatie;
                    cmbLocatie.DataBindItems();
                }

                ASPxComboBox cmbManager = grDateMedicina.FindEditFormTemplateControl("cmbManager") as ASPxComboBox;
                if (cmbManager != null)
                {
                    DataTable dtManager = General.IncarcaDT("SELECT * FROM \"tblManagerMedMuncii\" ORDER BY\"Denumire\"", null);
                    cmbManager.DataSource = dtManager;
                    cmbManager.DataBindItems();
                }

                ASPxComboBox cmbSectAlim = grDateMedicina.FindEditFormTemplateControl("cmbSectAlim") as ASPxComboBox;
                if (cmbSectAlim != null)
                {
                    DataTable dtSectAlim = General.GetPrelungireContract();
                    cmbSectAlim.DataSource = dtSectAlim;
                    cmbSectAlim.DataBindItems();
                }

                ASPxComboBox cmbRez = grDateMedicina.FindEditFormTemplateControl("cmbRez") as ASPxComboBox;
                if (cmbRez != null)
                {
                    DataTable dtRez = General.ListaRezultatExamen();
                    cmbRez.DataSource = dtRez;
                    cmbRez.DataBindItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateMedicina_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {

        }

        protected void grDateMedicina_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;

                switch (str)
                {
                    case "valab":
                        ASPxDateEdit txtDataElibCtrlMed = grDateMedicina.FindEditFormTemplateControl("txtDataElibCtrlMed") as ASPxDateEdit;
                        ASPxTextBox txtValab = grDateMedicina.FindEditFormTemplateControl("txtValab") as ASPxTextBox;
                        ASPxDateEdit txtDataUrmCtrl = grDateMedicina.FindEditFormTemplateControl("txtDataUrmCtrl") as ASPxDateEdit;
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