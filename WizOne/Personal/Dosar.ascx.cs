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
using DevExpress.Web.Data;
using System.Collections;

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

            grDateDosar.DataBind();

            if (!IsPostBack)
                Session["DocUpload_MP_Dosar"] = null;

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
                General.AdaugaDosar(ref ds, Session["Marca"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["Admin_Dosar"];
                DataRow dr = dt.NewRow();

                ASPxDateEdit txtDataPrim = grDateDosar.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateDosar.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateDosar.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateDosar.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;

                if (cmbNumeBen.Value == null || txtDataPrim.Value == null || txtDataExp.Value == null)
                {
                    e.Cancel = true;
                    grDateDosar.CancelEdit();
                    grDateDosar.JSProperties["cpAlertMessage"] = "Lipsesc date";
                    return;
                }

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("Admin_Dosar");
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
                    Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, metaUploadFile>;
                    if (lstFiles == null)
                        lstFiles = new Dictionary<int, metaUploadFile>();
                    lstFiles.Add(Convert.ToInt32(dr["IdAuto"].ToString()), itm);
                    Session["List_DocUpload_MP_Beneficii"] = lstFiles;
                }

                ds.Tables["Admin_Dosar"].Rows.Add(dr);
                Session["DocUpload_MP_Beneficii"] = null;

                e.Cancel = true;
                grDateDosar.CancelEdit();
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                grDateDosar.KeyFieldName = "IdAuto";
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateDosar_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                var idAuto = e.Keys["IdAuto"];

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow dr = ds.Tables["Admin_Dosar"].Rows.Find(idAuto);

                ASPxDateEdit txtDataPrim = grDateDosar.FindEditFormTemplateControl("txtDataPrim") as ASPxDateEdit;
                ASPxDateEdit txtDataExp = grDateDosar.FindEditFormTemplateControl("txtDataExp") as ASPxDateEdit;
                ASPxComboBox cmbNumeBen = grDateDosar.FindEditFormTemplateControl("cmbNumeBen") as ASPxComboBox;
                ASPxTextBox txtCaract = grDateDosar.FindEditFormTemplateControl("txtCaract") as ASPxTextBox;

                if (cmbNumeBen.Value == null || txtDataPrim.Value == null || txtDataExp.Value == null)
                {
                    e.Cancel = true;
                    grDateDosar.CancelEdit();
                    grDateDosar.JSProperties["cpAlertMessage"] = "Lipsesc date";
                    return;
                }

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
                grDateDosar.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
                grDateDosar.KeyFieldName = "IdAuto";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateDosar_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["Admin_Dosar"].Rows.Find(keys);

                Dictionary<int, metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, metaUploadFile>;
                if (lstFiles != null && lstFiles.ContainsKey(Convert.ToInt32(keys[0].ToString())))
                    lstFiles.Remove(Convert.ToInt32(keys[0].ToString()));
                Session["List_DocUpload_MP_Beneficii"] = lstFiles;

                Session["DocUpload_MP_MBeneficii"] = null;

                row.Delete();

                Session["FisiereDeSters"] = General.Nz(Session["FisiereDeSters"], "").ToString() + ";" + General.Nz(General.ExecutaScalar($@"SELECT '{Constante.fisiereApp}/Beneficii/' {Dami.Operator()} ""FisierNume"" FROM ""tblFisiere"" WHERE ""Tabela""='Admin_Medicina' AND ""Id""={keys[0]}"), "").ToString();

                e.Cancel = true;
                grDateDosar.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateDosar.DataSource = ds.Tables["Admin_Dosar"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }


        //protected void grDateDosar_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();

        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (ds.Tables.Contains("Admin_Dosar"))
        //            dt = ds.Tables["Admin_Dosar"];
        //        else
        //            return;

        //        DataTable dtAll = Session["Tmp_Admin_Dosar"] as DataTable;

        //        grDateDosar.CancelEdit();

        //        //daca avem linii noi
        //        for (int i = 0; i < e.InsertValues.Count; i++)
        //        {
        //            ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
        //            DataRow dr = dt.NewRow();
        //            dr["F10003"] = Session["Marca"];
        //            dr["IdObiect"] = upd.NewValues["IdObiect"] ?? DBNull.Value;
        //            dr["Descriere"] = upd.NewValues["Descriere"] ?? DBNull.Value;
        //            dr["USER_NO"] = Session["UserId"];
        //            dr["TIME"] = DateTime.Now;
        //            dt.Rows.Add(dr);
        //        }


        //        //daca avem linii modificate
        //        for (int i = 0; i < e.UpdateValues.Count; i++)
        //        {
        //            ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

        //            object[] keys = new object[upd.Keys.Count];
        //            for (int x = 0; x < upd.Keys.Count; x++)
        //            { keys[x] = upd.Keys[x]; }

        //            DataRow drAll = dtAll.Rows.Find(keys);
        //            if (drAll != null)
        //            {
        //                DataRow dr = dt.Rows.Find(keys);
        //                dr["F10003"] = Session["Marca"];
        //                dr["IdObiect"] = upd.NewValues["IdObiect"] ?? DBNull.Value;
        //                dr["Descriere"] = upd.NewValues["Descriere"] ?? DBNull.Value;
        //                dr["USER_NO"] = Session["UserId"];
        //                dr["TIME"] = DateTime.Now;
        //            }
        //        }

        //        //daca avem linii sterse
        //        for (int i = 0; i < e.DeleteValues.Count; i++)
        //        {
        //            ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

        //            object[] keys = new object[upd.Keys.Count];
        //            for (int x = 0; x < upd.Keys.Count; x++)
        //            { keys[x] = upd.Keys[x]; }

        //            DataRow dr = dt.Rows.Find(keys);
        //            if (dr != null)
        //                dt.Rows.Remove(dr);
        //        }

        //        e.Handled = true;

        //        Session["InformatiaCurentaPersonal"] = ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grDateDosar_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        //{
        //    try
        //    {
        //        e.NewValues["AreFisier"] = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        ////protected void grDateDosar_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        ////{
        ////    try
        ////    {
        ////        if (e.VisibleIndex >= 0)
        ////        {
        ////            if (General.Nz(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "Dinamic"), 0).ToString() == "1" && e.ButtonType == ColumnCommandButtonType.Delete)
        ////                e.Visible = false;
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        ////        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        ////    }
        ////}

        //protected void grDateDosar_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ButtonID == "btnSterge")
        //        {
        //            //if (General.Nz(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "Dinamic"),0).ToString() == "1")
        //            if (General.Nz(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "Obligatoriu"), 0).ToString() == "1")
        //                e.Visible = DevExpress.Utils.DefaultBoolean.False;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                //if (!e.IsValid) return;
                //ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                //metaUploadFile itm = new metaUploadFile();
                //itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                //itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                //itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                //Session["DocUpload_MP_Beneficii"] = itm;

                //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;


                object[] obj = grDateDosar.GetRowValues(grDateDosar.FocusedRowIndex, new string[] { "F10003", "IdObiect" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Dosar"))
                {
                    dt = ds.Tables["Admin_Dosar"];

                    DataRow dr = dt.Rows.Find(new object[] { obj[0], obj[1] });

                    if (dr == null)
                    {
                        dr = dt.NewRow();
                        dr["F10003"] = Session["Marca"];
                        dr["IdObiect"] = obj[1];
                        dt.Rows.Add(dr);
                    }

                    string extensie = Path.GetExtension(e.UploadedFile.FileName);
                    dr["Fisier"] = e.UploadedFile.FileBytes;
                    dr["FisierNume"] = e.UploadedFile.FileName;
                    dr["FisierExtensie"] = extensie;
                    dr["AreFisier"] = 1;

                    Session["InformatiaCurentaPersonal"] = ds;
                }
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        //protected void grDateDosar_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        //{
        //    try
        //    {
        //        string str = e.Parameters;
        //        if (str != "")
        //        {
        //            string[] arr = e.Parameters.Split(';');
        //            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //            DataTable dtAll = Session["Admin_Dosar"] as DataTable;
        //            object[] obj = grDateDosar.GetRowValues(grDateDosar.FocusedRowIndex, new string[] { "F10003", "IdObiect" }) as object[];

        //            grDateDosar.CancelEdit();

        //            switch (arr[0])
        //            {
        //                case "btnDocUpload":
        //                    {
        //                        string msg = Dami.TraduCuvant("proces realizat cu succes");
        //                        grDateDosar.JSProperties["cpAlertMessage"] = msg;
        //                    }
        //                    break;
        //                case "btnSterge":
        //                    {
        //                        DataTable dt = new DataTable();
        //                        if (ds.Tables.Contains("Admin_Dosar"))
        //                        {
        //                            dt = ds.Tables["Admin_Dosar"];

        //                            DataRow dr = dt.Rows.Find(obj[1]);
        //                            dr.Delete();

        //                            Session["InformatiaCurentaPersonal"] = ds;
        //                        }
        //                    }
        //                    break;
        //                case "btnStergeFisier":
        //                    {
        //                        if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

        //                        DataTable dt = new DataTable();
        //                        if (ds.Tables.Contains("Admin_Dosar"))
        //                        {
        //                            dt = ds.Tables["Admin_Dosar"];
        //                            DataRow dr = dt.Rows.Find(obj[1]);
        //                            if (dr != null)
        //                            {
        //                                dr["Fisier"] = null;
        //                                dr["FisierNume"] = null;
        //                                dr["FisierExtensie"] = null;
        //                                dr["AreFisier"] = 0;

        //                                Session["InformatiaCurentaPersonal"] = ds;
        //                            }
        //                        }
        //                    }
        //                    break;
        //            }

        //            grDateDosar.KeyFieldName = "IdObiect";
        //            grDateDosar.DataSource = dtAll;
        //            grDateDosar.DataBind();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grDateDosar_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.RowType == GridViewRowType.Data)
        //        {
        //            ASPxGridView grDateDosar = sender as ASPxGridView;

        //            //GridViewDataColumn colUpload = grDateDosar.Columns["colAtas"].Columns["colUpload"] as GridViewDataColumn;
        //            //ASPxUploadControl btnUpload = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colUpload, "btnDocUpload") as ASPxUploadControl;
        //            //if (btnUpload != null)
        //            //{
        //            //    btnUpload.ClientSideEvents.FileUploadComplete = string.Format("function(s,e) {{ grDateDosar.PerformCallback('btnDocUpload;{0}'); }}", e.GetValue("IdObiect"));
        //            //}

        //            GridViewDataColumn colSterge = grDateDosar.Columns["colAtas"].Columns["colSterge"] as GridViewDataColumn;
        //            ASPxButton btnSterge = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colSterge, "btnStergeFisier") as ASPxButton;
        //            if (btnSterge != null)
        //                btnSterge.ClientSideEvents.Click = "function(s,e) { if (" + General.Nz(e.GetValue("AreFisier"), 0).ToString() + " == 1) grDateDosar.PerformCallback('btnStergeFisier;" + General.Nz(e.GetValue("IdObiect"),-99).ToString() + "'); else { swal({ title: '', text: 'Nu exista fisier de sters', type: 'info' }); } }";


        //            //btnSterge.ClientSideEvents.Click = string.Format("function(s,e) {{ If (" + General.Nz(e.GetValue("AreFisier"),0).ToString() + " == 1) grDateDosar.PerformCallback('btnStergeFisier;{0}'); else { swal({ title: '', text: s.cpAlertMessage, type: 'info' }); } }}", e.GetValue("IdObiect"));

        //            GridViewDataColumn colArata = grDateDosar.Columns["colAtas"].Columns["colArata"] as GridViewDataColumn;
        //            ASPxButton btnArata = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colArata, "btnArata") as ASPxButton;
        //            if (btnArata != null)
        //                btnArata.ClientSideEvents.Click = "function(s,e) { if (" + General.Nz(e.GetValue("AreFisier"), 0).ToString() + " == 1) GoToAtasModeDosar('" + General.Nz(e.GetValue("F10003"), -99).ToString() + "; " + General.Nz(e.GetValue("IdObiect"), -99).ToString() + "'); else { swal({ title: '', text: 'Nu exista fisier de vizualizat', type: 'info' }); } }";


        //            //btnArata.ClientSideEvents.Click = string.Format("function(s,e) {{ GoToAtasMode({0}); }}", e.GetValue("IdObiect"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected string GetImagePath(object valoare)
        //{
        //    string rez = "~/Fisiere/Imagini/Icoane/cript.png";

        //    try
        //    {
        //        if (Convert.ToInt32(General.Nz(valoare, 0)) == 1)
        //            rez = "~/Fisiere/Imagini/Icoane/decript.png";
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return rez;
        //}

        ////private void LoadData(ref DataRow dr, object idObiect, object descriere)
        ////{
        ////    try
        ////    {
        ////        dr["F10003"] = Session["Marca"];
        ////        dr["IdObiect"] = idObiect;
        ////        dr["Descriere"] = descriere;
        ////        //dr["Fisier"] = upd.NewValues["Fisier"] ?? DBNull.Value;
        ////        //dr["FisierNume"] = upd.NewValues["FisierNume"] ?? DBNull.Value;
        ////        //dr["FisierExtensie"] = upd.NewValues["FisierExtensie"] ?? DBNull.Value;
        ////        //if (dr["Fisier"] == DBNull.Value)
        ////        //    dr["AreFisier"] = 0;
        ////        //else
        ////        //    dr["AreFisier"] = 1;
        ////        dr["USER_NO"] = Session["UserId"];
        ////        dr["TIME"] = DateTime.Now;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        ////        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        ////    }
        ////}

    }
}