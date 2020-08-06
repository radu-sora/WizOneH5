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
    public partial class Cursuri : System.Web.UI.UserControl
    {
        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grDateCursuri.DataBind();
            //grDateCursuri.AddNewRow();
            foreach (dynamic c in grDateCursuri.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateCursuri.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateCursuri.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateCursuri.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateCursuri.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateCursuri.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateCursuri);

            if (!IsPostBack)
                Session["DocUpload_MP_Cursuri"] = null;

        }

        protected void grDateCursuri_DataBinding(object sender, EventArgs e)
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
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"Admin_Cursuri_VIEW\" a WHERE \"Marca\" = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Cursuri"))
            {
                dt = ds.Tables["Admin_Cursuri"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Cursuri";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateCursuri.KeyFieldName = "IdAuto";
            grDateCursuri.DataSource = dt;

            string sql = @"SELECT * FROM ""Admin_TipCurs"" ORDER BY ""TipCurs""";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Admin_TipCurs", "IdAuto") + " ORDER BY \"TipCurs\"";
            DataTable dtTipAutorizatie = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colTipAutorizatie = (grDateCursuri.Columns["IdTipCurs"] as GridViewDataComboBoxColumn);
            colTipAutorizatie.PropertiesComboBox.DataSource = dtTipAutorizatie;

            sql = @"SELECT * FROM ""Admin_DescrCurs"" ORDER BY  ""DescriereCurs""";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Admin_DescrCurs", "IdAuto") + " ORDER BY \"DescriereCurs\"";
            DataTable dtDescriereAutorizatie = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colDescriereAutorizatie = (grDateCursuri.Columns["IdDescriereCurs"] as GridViewDataComboBoxColumn);
            colDescriereAutorizatie.PropertiesComboBox.DataSource = dtDescriereAutorizatie;

            //Radu 11.02.2020
            sql = @"SELECT * FROM ""tblMonede"" ORDER BY ""Abreviere""";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("tblMonede", "Id") + " ORDER BY \"Abreviere\"";
            DataTable dtMonede = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colMonede = (grDateCursuri.Columns["IdMoneda"] as GridViewDataComboBoxColumn);
            colMonede.PropertiesComboBox.DataSource = dtMonede;


            sql = @"SELECT * FROM ""tblOperatorMedMuncii"" ORDER BY ""Denumire""";
            DataTable dtOperator = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colOperator = (grDateCursuri.Columns["Operator"] as GridViewDataComboBoxColumn);
            colOperator.PropertiesComboBox.DataSource = dtOperator;

        }

        protected void grDateCursuri_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Cursuri"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Cursuri");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void grDateCursuri_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Cursuri"];
                DataRow dr = dt.NewRow();

                ASPxDateEdit deDataInceput = grDateCursuri.FindEditFormTemplateControl("deDataInceput") as ASPxDateEdit;
                ASPxDateEdit deDataSfarsit = grDateCursuri.FindEditFormTemplateControl("deDataSfarsit") as ASPxDateEdit;
                ASPxDateEdit deDataCurs = grDateCursuri.FindEditFormTemplateControl("deDataCurs") as ASPxDateEdit;
                ASPxDateEdit deDataExpAut = grDateCursuri.FindEditFormTemplateControl("deDataExpAut") as ASPxDateEdit;
                ASPxComboBox cmbTipCurs = grDateCursuri.FindEditFormTemplateControl("cmbTipCurs") as ASPxComboBox;
                ASPxComboBox cmbDescriere = grDateCursuri.FindEditFormTemplateControl("cmbDescriere") as ASPxComboBox;
                ASPxComboBox cmbOperator = grDateCursuri.FindEditFormTemplateControl("cmbOperator") as ASPxComboBox;
                ASPxComboBox cmbMoneda = grDateCursuri.FindEditFormTemplateControl("cmbMoneda") as ASPxComboBox;
                ASPxTextBox txtNumeComplet = grDateCursuri.FindEditFormTemplateControl("txtNumeComplet") as ASPxTextBox;
                ASPxTextBox txtInfo = grDateCursuri.FindEditFormTemplateControl("txtInfo") as ASPxTextBox;
                ASPxTextBox txtNrZile = grDateCursuri.FindEditFormTemplateControl("txtNrZile") as ASPxTextBox;
                ASPxTextBox txtNrOre = grDateCursuri.FindEditFormTemplateControl("txtNrOre") as ASPxTextBox;
                ASPxTextBox txtNumeFurnizor = grDateCursuri.FindEditFormTemplateControl("txtNumeFurnizor") as ASPxTextBox;
                ASPxTextBox txtTema = grDateCursuri.FindEditFormTemplateControl("txtTema") as ASPxTextBox;
                ASPxTextBox txtPerAmortiz = grDateCursuri.FindEditFormTemplateControl("txtPerAmortiz") as ASPxTextBox;
                ASPxTextBox txtBuget = grDateCursuri.FindEditFormTemplateControl("txtBuget") as ASPxTextBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Cursuri");
                if (Convert.ToInt32(dr["IdAuto"].ToString()) < 1000000)
                    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1000000;

                dr["Marca"] = Session["Marca"];

                dr["IdTipCurs"] = cmbTipCurs.Value ?? DBNull.Value;
                dr["NumeComplet"] = txtNumeComplet.Value ?? DBNull.Value;
                dr["Info"] = txtInfo.Value ?? DBNull.Value;
                dr["DataInceput"] = deDataInceput.Value ?? DBNull.Value;
                dr["DataSfarsit"] = deDataSfarsit.Value ?? DBNull.Value;
                dr["NrZile"] = txtNrZile.Value ?? DBNull.Value;
                dr["NrOre"] = txtNrOre.Value ?? DBNull.Value;
                dr["IdDescriereCurs"] = cmbDescriere.Value ?? DBNull.Value;
                dr["NumeFurnizor"] = txtNumeFurnizor.Value ?? DBNull.Value;
                dr["TemaCurs"] = txtTema.Value ?? DBNull.Value;
                dr["DataCurs"] = deDataCurs.Value ?? DBNull.Value;
                dr["Buget"] = txtBuget.Value ?? DBNull.Value;
                dr["IdMoneda"] = cmbMoneda.Value ?? DBNull.Value;
                dr["Operator"] = cmbOperator.Value ?? DBNull.Value;
                dr["PerioadaAmortizare"] = txtPerAmortiz.Value ?? DBNull.Value;
                dr["AutorizareExpirare"] = deDataExpAut.Value ?? DBNull.Value;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                metaUploadFile itm = Session["DocUpload_MP_Cursuri"] as metaUploadFile;
                if (itm != null)
                {          
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Cursuri"] = lstFiles;
                }
                Session["DocUpload_MP_Cursuri"] = null;

                ds.Tables["Admin_Cursuri"].Rows.Add(dr);
                e.Cancel = true;
                grDateCursuri.CancelEdit();
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];
                grDateCursuri.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCursuri_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Cursuri"].Rows.Find(idAuto);

                ASPxDateEdit deDataInceput = grDateCursuri.FindEditFormTemplateControl("deDataInceput") as ASPxDateEdit;
                ASPxDateEdit deDataSfarsit = grDateCursuri.FindEditFormTemplateControl("deDataSfarsit") as ASPxDateEdit;
                ASPxDateEdit deDataCurs = grDateCursuri.FindEditFormTemplateControl("deDataCurs") as ASPxDateEdit;
                ASPxDateEdit deDataExpAut = grDateCursuri.FindEditFormTemplateControl("deDataExpAut") as ASPxDateEdit;
                ASPxComboBox cmbTipCurs = grDateCursuri.FindEditFormTemplateControl("cmbTipCurs") as ASPxComboBox;
                ASPxComboBox cmbDescriere = grDateCursuri.FindEditFormTemplateControl("cmbDescriere") as ASPxComboBox;
                ASPxComboBox cmbOperator = grDateCursuri.FindEditFormTemplateControl("cmbOperator") as ASPxComboBox;
                ASPxComboBox cmbMoneda = grDateCursuri.FindEditFormTemplateControl("cmbMoneda") as ASPxComboBox;
                ASPxTextBox txtNumeComplet = grDateCursuri.FindEditFormTemplateControl("txtNumeComplet") as ASPxTextBox;
                ASPxTextBox txtInfo = grDateCursuri.FindEditFormTemplateControl("txtInfo") as ASPxTextBox;
                ASPxTextBox txtNrZile = grDateCursuri.FindEditFormTemplateControl("txtNrZile") as ASPxTextBox;
                ASPxTextBox txtNrOre = grDateCursuri.FindEditFormTemplateControl("txtNrOre") as ASPxTextBox;
                ASPxTextBox txtNumeFurnizor = grDateCursuri.FindEditFormTemplateControl("txtNumeFurnizor") as ASPxTextBox;
                ASPxTextBox txtTema = grDateCursuri.FindEditFormTemplateControl("txtTema") as ASPxTextBox;
                ASPxTextBox txtPerAmortiz = grDateCursuri.FindEditFormTemplateControl("txtPerAmortiz") as ASPxTextBox;
                ASPxTextBox txtBuget = grDateCursuri.FindEditFormTemplateControl("txtBuget") as ASPxTextBox;

                dr["Marca"] = Session["Marca"];

                dr["IdTipCurs"] = cmbTipCurs.Value ?? DBNull.Value;
                dr["NumeComplet"] = txtNumeComplet.Value ?? DBNull.Value;
                dr["Info"] = txtInfo.Value ?? DBNull.Value;
                dr["DataInceput"] = deDataInceput.Value ?? DBNull.Value;
                dr["DataSfarsit"] = deDataSfarsit.Value ?? DBNull.Value;
                dr["NrZile"] = txtNrZile.Value ?? DBNull.Value;
                dr["NrOre"] = txtNrOre.Value ?? DBNull.Value;
                dr["IdDescriereCurs"] = cmbDescriere.Value ?? DBNull.Value;
                dr["NumeFurnizor"] = txtNumeFurnizor.Value ?? DBNull.Value;
                dr["TemaCurs"] = txtTema.Value ?? DBNull.Value;
                dr["DataCurs"] = deDataCurs.Value ?? DBNull.Value;
                dr["Buget"] = txtBuget.Value ?? DBNull.Value;
                dr["IdMoneda"] = cmbMoneda.Value ?? DBNull.Value;
                dr["Operator"] = cmbOperator.Value ?? DBNull.Value;
                dr["PerioadaAmortizare"] = txtPerAmortiz.Value ?? DBNull.Value;
                dr["AutorizareExpirare"] = deDataExpAut.Value ?? DBNull.Value;

                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;


                metaUploadFile itm = Session["DocUpload_MP_Cursuri"] as metaUploadFile;
                if (itm != null)
                {
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    if (lstFiles.ContainsKey(Convert.ToInt32(dr["IdAuto"].ToString())))
                        lstFiles[Convert.ToInt32(dr["IdAuto"].ToString())] = itm;
                    else
                        lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Cursuri"] = lstFiles;
                }
                Session["DocUpload_MP_Cursuri"] = null;

                e.Cancel = true;
                grDateCursuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateCursuri_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateCursuri.GetRow(e.VisibleIndex) as DataRowView;

                if (values != null)
                {
                    string modif = values.Row["Modificabil"].ToString();

                    if (modif == "0")
                    {
                        if (e.ButtonType == ColumnCommandButtonType.Edit || e.ButtonType == ColumnCommandButtonType.Delete)

                            e.Visible = false;

                    }
                }
            }
        }

        protected void grDateCursuri_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Cursuri"].Rows.Find(keys);

                Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, metaUploadFile>;
                if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                    lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                Session["List_DocUpload_MP_Cursuri"] = lstFiles;

                Session["DocUpload_MP_Cursuri"] = null;

                row.Delete();

                e.Cancel = true;
                grDateCursuri.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateCursuri.DataSource = ds.Tables["Admin_Cursuri"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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

                Session["DocUpload_MP_Cursuri"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDateCursuri_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbTipCurs = grDateCursuri.FindEditFormTemplateControl("cmbTipCurs") as ASPxComboBox;
                if (cmbTipCurs != null)
                {
                    string sql = @"SELECT * FROM ""Admin_TipCurs"" ORDER BY ""TipCurs""";
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("Admin_TipCurs", "IdAuto") + " ORDER BY \"TipCurs\"";
                    DataTable dtTipAutorizatie = General.IncarcaDT(sql, null);
                    cmbTipCurs.DataSource = dtTipAutorizatie;
                    cmbTipCurs.DataBindItems();
                }

                ASPxComboBox cmbDescriere = grDateCursuri.FindEditFormTemplateControl("cmbDescriere") as ASPxComboBox;
                if (cmbDescriere != null)
                {
                    string sql = @"SELECT * FROM ""Admin_DescrCurs"" ORDER BY  ""DescriereCurs""";
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("Admin_DescrCurs", "IdAuto") + " ORDER BY \"DescriereCurs\"";
                    DataTable dtDescriereAutorizatie = General.IncarcaDT(sql, null);
                    cmbDescriere.DataSource = dtDescriereAutorizatie;
                    cmbDescriere.DataBindItems();
                }

                ASPxComboBox cmbMoneda = grDateCursuri.FindEditFormTemplateControl("cmbMoneda") as ASPxComboBox;
                if (cmbMoneda != null)
                {
                    string sql = @"SELECT * FROM ""tblMonede"" ORDER BY ""Abreviere""";
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("tblMonede", "Id") + " ORDER BY \"Abreviere\"";
                    DataTable dtMonede = General.IncarcaDT(sql, null);
                    cmbMoneda.DataSource = dtMonede;
                    cmbMoneda.DataBindItems();
                }

                ASPxComboBox cmbOperator = grDateCursuri.FindEditFormTemplateControl("cmbOperator") as ASPxComboBox;
                if (cmbOperator != null)
                {
                    string sql = @"SELECT * FROM ""tblOperatorMedMuncii"" ORDER BY ""Denumire""";
                    DataTable dtOperator = General.IncarcaDT(sql, null);
                    cmbOperator.DataSource = dtOperator;
                    cmbOperator.DataBindItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}