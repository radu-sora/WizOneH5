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
    public partial class Echipamente : System.Web.UI.UserControl
    {
        public class metaUploadFile
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grDateEchipamente.DataBind();

            foreach (dynamic c in grDateEchipamente.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateEchipamente.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateEchipamente.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateEchipamente.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateEchipamente.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateEchipamente.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");

            if (!IsPostBack)
                Session["DocUpload_MP_Beneficii"] = null;

            if (General.VarSession("EsteAdmin").ToString() == "0") General.SecuritatePersonal(grDateEchipamente);
        }

        protected void grDateEchipamente_DataBinding(object sender, EventArgs e)
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

            string sqlFinal = "SELECT * FROM \"Admin_Echipamente\" WHERE \"Marca\" = " + HttpContext.Current.Session["Marca"].ToString();
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("Admin_Echipamente"))
            {
                dt = ds.Tables["Admin_Echipamente"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "Admin_Echipamente";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateEchipamente.KeyFieldName = "IdAuto";
            grDateEchipamente.DataSource = dt;

            DataTable dtEchip = General.GetObiecteDinArie("ArieTabEchipamenteDinPersonal");
            GridViewDataComboBoxColumn colEchip = (grDateEchipamente.Columns["IdObiect"] as GridViewDataComboBoxColumn);
            colEchip.PropertiesComboBox.DataSource = dtEchip;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;

        }

        protected void grDateEchipamente_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Echipamente"];
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
                    e.NewValues["IdAuto"] = Dami.NextId("Admin_Echipamente");
                e.NewValues["Marca"] = Session["Marca"];
                e.NewValues["DataPrimire"] = DateTime.Now;
                e.NewValues["DataPrimire"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEchipamente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Echipamente"];
                DataRow dr = dt.NewRow();

                ASPxDateEdit txtDataPrim = grDateEchipamente.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateEchipamente.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxDateEdit txtDataPre = grDateEchipamente.FindEditFormTemplateControl("txtDataPre") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateEchipamente.FindEditFormTemplateControl("cmbNume") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateEchipamente.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;
                ASPxCheckBox chkLaDosar = grDateEchipamente.FindEditFormTemplateControl("chkLaDosar") as ASPxCheckBox;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Echipamente");
                if (Convert.ToInt32(dr["IdAuto"].ToString()) < 1000000)
                    dr["IdAuto"] = Convert.ToInt32(dr["IdAuto"].ToString()) + 1000000;

                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                dr["DataPrimire"] = txtDataPrim.Value ?? DBNull.Value;
                dr["DataExpirare"] = txtDataExp.Value ?? DBNull.Value;
                dr["DataPredare"] = txtDataPre.Value ?? DBNull.Value;
                dr["Caracteristica"] = txtCaract.Value ?? DBNull.Value;
                dr["EsteLaDosar"] = chkLaDosar.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                int nrAni = 0, nrLuni = 0, nrZile = 0;
                CalculVechime(Convert.ToDateTime(e.NewValues["DataPrimire"]).Date, Convert.ToDateTime(e.NewValues["DataExpirare"]).Date, out nrAni, out nrLuni, out nrZile);
                string vechime = " {0} {1} {2} {3} {4} {5} ";
                dr["DurataUtilizare"] = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                 (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                 (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : "")).Trim();

                metaUploadFile itm = Session["DocUpload_MP_Echipamente"] as metaUploadFile;
                if (itm != null)
                {
                    dr["Fisier"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;

                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Echipamente"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Echipamente"] = lstFiles;
                }

                ds.Tables["Admin_Echipamente"].Rows.Add(dr);
                Session["DocUpload_MP_Echipamente"] = null;

                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];
                grDateEchipamente.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateEchipamente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Echipamente"].Rows.Find(idAuto);

                ASPxDateEdit txtDataPrim = grDateEchipamente.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateEchipamente.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxDateEdit txtDataPre = grDateEchipamente.FindEditFormTemplateControl("txtDataPre") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateEchipamente.FindEditFormTemplateControl("cmbNume") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateEchipamente.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;
                ASPxCheckBox chkLaDosar = grDateEchipamente.FindEditFormTemplateControl("chkLaDosar") as ASPxCheckBox;

                dr["Marca"] = Session["Marca"];
                dr["IdObiect"] = cmbNumeBen.Value ?? DBNull.Value;
                dr["DataPrimire"] = txtDataPrim.Value ?? DBNull.Value;
                dr["DataExpirare"] = txtDataExp.Value ?? DBNull.Value;
                dr["DataPredare"] = txtDataPre.Value ?? DBNull.Value;
                dr["Caracteristica"] = txtCaract.Value ?? DBNull.Value;
                dr["EsteLaDosar"] = chkLaDosar.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                int nrAni = 0, nrLuni = 0, nrZile = 0;
                CalculVechime(Convert.ToDateTime(e.NewValues["DataPrimire"]).Date, Convert.ToDateTime(e.NewValues["DataExpirare"]).Date, out nrAni, out nrLuni, out nrZile);
                string vechime = " {0} {1} {2} {3} {4} {5} ";
                dr["DurataUtilizare"] = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                 (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                 (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : "")).Trim();


                metaUploadFile itm = Session["DocUpload_MP_Echipamente"] as metaUploadFile;
                if (itm != null)
                {
                    dr["Fisier"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;

                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Echipamente"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    if (lstFiles.ContainsKey(Convert.ToInt32(idAuto.ToString())))
                        lstFiles[Convert.ToInt32(idAuto.ToString())] = itm;
                    else
                        lstFiles.Add(Convert.ToInt32(idAuto.ToString()), itm);
                    Session["List_DocUpload_MP_Echipamente"] = lstFiles;
                }
                Session["DocUpload_MP_Echipamente"] = null;

                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];
                grDateEchipamente.KeyFieldName = "IdAuto";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateEchipamente_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Echipamente"].Rows.Find(keys);

                if (General.Nz(row["VineDinPosturi"], 0).ToString() == "1")
                {
                    grDateEchipamente.JSProperties["cpAlertMessage"] = "Aceasta linie nu se poate elimina deoarece vine din posturi";
                }
                else
                {
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Echipamente"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                        lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                    Session["List_DocUpload_MP_Echipamente"] = lstFiles;

                    Session["DocUpload_MP_Echipamente"] = null;

                    row.Delete();

                    Session["FisiereDeSters"] = General.Nz(Session["FisiereDeSters"], "").ToString() + ";" + General.Nz(General.ExecutaScalar($@"SELECT '{Constante.fisiereApp}/Echipamente/' {Dami.Operator()} ""FisierNume"" FROM ""tblFisiere"" WHERE ""Tabela""='Admin_Medicina' AND ""Id""={keys[0]}"), "").ToString();
                }

                e.Cancel = true;
                grDateEchipamente.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateEchipamente.DataSource = ds.Tables["Admin_Echipamente"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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

                Session["DocUpload_MP_Echipamente"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateEchipamente_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "DurataUtil")
            {
                int nrAni = 0, nrLuni = 0, nrZile = 0;
                string vechime = " {0} {1} {2} {3} {4} {5} ";
                DateTime dataStart = DateTime.Now, dataSfarsit = DateTime.Now;

                if (e.GetListSourceFieldValue("DataPrimire") != null && e.GetListSourceFieldValue("DataExpirare") != null && e.GetListSourceFieldValue("DataPrimire").ToString().Length > 0 && e.GetListSourceFieldValue("DataExpirare").ToString().Length > 0)
                {
                    DateTime dtStart = Convert.ToDateTime(e.GetListSourceFieldValue("DataPrimire").ToString());
                    DateTime dtSfarsit = Convert.ToDateTime(e.GetListSourceFieldValue("DataExpirare").ToString());

                    dataStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day);
                    dataSfarsit = new DateTime(dtSfarsit.Year, dtSfarsit.Month, dtSfarsit.Day);

                    CalculVechime(dataStart, dataSfarsit, out nrAni, out nrLuni, out nrZile);

                    vechime = string.Format(vechime, (nrAni > 0 ? nrAni.ToString() : ""), (nrAni > 0 ? (nrAni == 1 ? "an" : "ani") : ""),
                                                        (nrLuni > 0 ? nrLuni.ToString() : ""), (nrLuni > 0 ? (nrLuni == 1 ? "luna" : "luni") : ""),
                                                        (nrZile > 0 ? nrZile.ToString() : ""), (nrZile > 0 ? (nrZile == 1 ? "zi" : "zile") : ""));

                    e.Value = vechime;
                    
                }
            }
        }

        private void CalculVechime(DateTime dataStart, DateTime dataSfarsit, out int nrAni, out int nrLuni, out int nrZile)
        {
            DateTime odtT1, odtT2, odtD;
            List<int> arNrZileInLuna = new List<int>();

            // determin nr zile calendaristice in luna:
            odtT1 = new DateTime(dataStart.Year, dataStart.Month, 1, 0, 0, 0);
            odtT2 = new DateTime(dataSfarsit.Year, dataSfarsit.Month, 1, 0, 0, 0);
            for (DateTime odtDt = odtT1; odtDt <= odtT2;)
            {
                odtD = new DateTime(
                    odtDt.Month == 12 ? odtDt.Year + 1 : odtDt.Year,
                    odtDt.Month == 12 ? 1 : odtDt.Month + 1, 1, 0, 0, 0);

                int odtsDf = (int)(odtD - odtDt).TotalDays;
                arNrZileInLuna.Add((int)(odtsDf));
                odtDt = odtD;
            }

            nrLuni = 0;
            nrZile = (int)(dataSfarsit - dataStart).TotalDays + 1;

            for (int nI = 0; nI < arNrZileInLuna.Count && nrZile >= arNrZileInLuna[nI]; nI++)
            {
                nrZile -= arNrZileInLuna[nI];
                nrLuni++;
            }

            nrAni = 0;
            if (nrLuni > 12)
            {
                nrAni = nrLuni / 12;
                nrLuni = nrLuni % 12;
            }
        }

        protected void grDateEchipamente_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                General.SecuritatePersonal(grDateEchipamente, "Echipamente", Convert.ToInt32(Session["UserId"].ToString()), true);

                ASPxComboBox cmbCateg = grDateEchipamente.FindEditFormTemplateControl("cmbNume") as ASPxComboBox;
                if (cmbCateg != null)
                {
                    cmbCateg.DataSource = General.GetObiecteDinArie("ArieTabEchipamenteDinPersonal");
                    cmbCateg.DataBindItems();
                }

                HtmlTableCell lblNume = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblNume");
                lblNume.InnerText = Dami.TraduCuvant("Nume echipament");
                HtmlTableCell lblDataPrimire = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblDataPrimire");
                lblDataPrimire.InnerText = Dami.TraduCuvant("Data primire");
                HtmlTableCell lblDataPredare = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblDataPredare");
                lblDataPredare.InnerText = Dami.TraduCuvant("Data predare");
                HtmlTableCell lblDataExp = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblDataExp");
                lblDataExp.InnerText = Dami.TraduCuvant("Data expirare");
                HtmlTableCell lblCaract = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblCaract");
                lblCaract.InnerText = Dami.TraduCuvant("Caracteristica echipament");
                HtmlTableCell lblLaDosar = (HtmlTableCell)grDateEchipamente.FindEditFormTemplateControl("lblLaDosar");
                lblLaDosar.InnerText = Dami.TraduCuvant("Indosariat");

                ASPxUploadControl btnDocUploadBen = (ASPxUploadControl)grDateEchipamente.FindEditFormTemplateControl("btnDocUploadBen");
                btnDocUploadBen.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUploadBen.ToolTip = Dami.TraduCuvant("Incarca Document");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
    }
}