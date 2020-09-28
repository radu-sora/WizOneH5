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

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Traducere
            foreach (dynamic c in grDateDosar.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
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
            {
                IncarcaGrid();
            }
            else
            {
                DataTable dtAll = Session["Tmp_Admin_Dosar"] as DataTable;
                grDateDosar.DataSource = dtAll;
                grDateDosar.DataBind();
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string strSql =
                    $@"SELECT A.F10003, A.""IdObiect"", A.""Fisier"", A.""FisierNume"", A.""FisierExtensie"", A.""Descriere"", A.""AreFisier"", A.USER_NO, A.TIME, 0 AS ""Dinamic""
                FROM ""Admin_Dosar"" A
                INNER JOIN ""Org_relPostAngajat"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
                LEFT JOIN ""Org_PosturiDosar"" C ON B.""IdPost"" = C.""IdPost"" AND A.""IdObiect""=C.""IdObiect""
                WHERE A.F10003=@1";

                DataTable dt = new DataTable();
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Dosar"))
                {
                    dt = ds.Tables["Admin_Dosar"];
                }
                else
                {
                    dt = General.IncarcaDT(strSql, new object[] { Session["Marca"] });
                    dt.TableName = "Admin_Dosar";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdObiect"] };

                    ds.Tables.Add(dt);
                }

                string sqlAll = $@"SELECT {Session["Marca"]} AS F10003, A.""IdObiect"", null AS ""Fisier"", null AS ""FisierNume"", null AS ""FisierExtensie"", null AS ""Descriere"", 0 AS ""AreFisier"", {Session["UserId"]}, {General.CurrentDate()}, 1 AS Dinamic
                FROM ""Org_PosturiDosar"" A
                INNER JOIN ""Org_relPostAngajat"" B ON A.""IdPost""=B.""IdPost"" AND B.""DataInceput"" <= GetDate() AND GetDate() <= B.""DataSfarsit""
                LEFT JOIN ""Admin_Dosar"" C ON B.F10003=C.F10003 AND A.""IdObiect""=C.""IdObiect""
                WHERE B.F10003=@1 AND C.""IdObiect"" IS NULL";

                DataTable dtAll = General.IncarcaDT(strSql, new object[] { Session["Marca"] });
                dtAll.PrimaryKey = new DataColumn[] { dtAll.Columns["IdObiect"] };
                DataTable dtTmp = General.IncarcaDT(sqlAll, new object[] { Session["Marca"] });
                foreach (DataRow dr in dtTmp.Rows)
                {
                    dtAll.ImportRow(dr);
                }
                Session["Tmp_Admin_Dosar"] = dtAll;

                grDateDosar.KeyFieldName = "IdObiect";
                grDateDosar.DataSource = dtAll;
                grDateDosar.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (ds.Tables.Contains("Admin_Dosar"))
                    dt = ds.Tables["Admin_Dosar"];
                else
                    return;

                DataTable dtAll = Session["Tmp_Admin_Dosar"] as DataTable;

                grDateDosar.CancelEdit();

                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
                    DataRow dr = dt.NewRow();
                    LoadData(ref dr, upd.NewValues["IdObiect"] ?? DBNull.Value, upd.NewValues["Descriere"] ?? DBNull.Value);
                    dt.Rows.Add(dr);

                    DataRow drAll = dtAll.NewRow();
                    LoadData(ref drAll, upd.NewValues["IdObiect"] ?? DBNull.Value, upd.NewValues["Descriere"] ?? DBNull.Value);
                    dtAll.Rows.Add(drAll);
                }


                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow drAll = dtAll.Rows.Find(keys);
                    if (drAll != null)
                    {
                        LoadData(ref drAll, upd.NewValues["IdObiect"] ?? DBNull.Value, upd.NewValues["Descriere"] ?? DBNull.Value);
                        DataRow dr = dt.Rows.Find(keys);
                        if (dr != null)
                        {
                            LoadData(ref dr, upd.NewValues["IdObiect"] ?? DBNull.Value, upd.NewValues["Descriere"] ?? DBNull.Value);
                        }
                        else
                        {
                            dr = dt.NewRow();
                            LoadData(ref dr, upd.NewValues["IdObiect"] ?? DBNull.Value, upd.NewValues["Descriere"] ?? DBNull.Value);
                            dt.Rows.Add(dr);
                        }
                    }
                }


                //daca avem linii sterse
                for (int i = 0; i < e.DeleteValues.Count; i++)
                {
                    ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr != null)
                        dt.Rows.Remove(dr);

                    DataRow drAll = dtAll.Rows.Find(keys);
                    if (drAll != null)
                        dtAll.Rows.Remove(drAll);
                }

                e.Handled = true;

                Session["InformatiaCurentaPersonal"] = ds;
                Session["Tmp_Admin_Dosar"] = dtAll;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
        //            //dr["Fisier"] = upd.NewValues["Fisier"] ?? DBNull.Value;
        //            //dr["FisierNume"] = upd.NewValues["FisierNume"] ?? DBNull.Value;
        //            //dr["FisierExtensie"] = upd.NewValues["FisierExtensie"] ?? DBNull.Value;
        //            //if (dr["Fisier"] == DBNull.Value)
        //            //    dr["AreFisier"] = 0;
        //            //else
        //            //    dr["AreFisier"] = 1;
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

        //            DataRow dr = dt.Rows.Find(keys);
        //            if (dr != null)
        //            {
        //                dr["F10003"] = Session["Marca"];
        //                dr["IdObiect"] = upd.NewValues["IdObiect"] ?? DBNull.Value;
        //                dr["Descriere"] = upd.NewValues["Descriere"] ?? DBNull.Value;
        //                //dr["Fisier"] = upd.NewValues["Fisier"] ?? DBNull.Value;
        //                //dr["FisierNume"] = upd.NewValues["FisierNume"] ?? DBNull.Value;
        //                //dr["FisierExtensie"] = upd.NewValues["FisierExtensie"] ?? DBNull.Value;
        //                //if (dr["Fisier"] == DBNull.Value)
        //                //    dr["AreFisier"] = 0;
        //                //else
        //                //    dr["AreFisier"] = 1;
        //                dr["USER_NO"] = Session["UserId"];
        //                dr["TIME"] = DateTime.Now;
        //            }
        //            else
        //            {
        //                DataTable dtAfisat = Session["Tmp_Admin_Dosar"] as DataTable;
        //                DataRow drTmp = dtAfisat.Rows.Find(keys);
        //                if (drTmp != null && General.Nz(drTmp["Dinamic"],0).ToString() == "1")
        //                {
        //                    DataRow drNew = dt.NewRow();

        //                    drNew["F10003"] = Session["Marca"];
        //                    drNew["IdObiect"] = upd.NewValues["IdObiect"] ?? DBNull.Value;
        //                    drNew["Descriere"] = upd.NewValues["Descriere"] ?? DBNull.Value;
        //                    //drNew["Fisier"] = upd.NewValues["Fisier"] ?? DBNull.Value;
        //                    //drNew["FisierNume"] = upd.NewValues["FisierNume"] ?? DBNull.Value;
        //                    //drNew["FisierExtensie"] = upd.NewValues["FisierExtensie"] ?? DBNull.Value;
        //                    //if (dr["Fisier"] == DBNull.Value)
        //                    //    dr["AreFisier"] = 0;
        //                    //else
        //                    //    dr["AreFisier"] = 1;
        //                    drNew["USER_NO"] = Session["UserId"];
        //                    drNew["TIME"] = DateTime.Now;

        //                    dt.Rows.Add(drNew);
        //                }
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
        //            if (dr == null) continue;

        //            dt.Rows.Remove(dr);
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

        protected void grDateDosar_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["AreFisier"] = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void grDateDosar_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (e.VisibleIndex >= 0)
        //        {
        //            if (General.Nz(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "Dinamic"), 0).ToString() == "1" && e.ButtonType == ColumnCommandButtonType.Delete)
        //                e.Visible = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void grDateDosar_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
        {
            try
            {
                if (e.ButtonID == "btnSterge")
                {
                    if (General.Nz(((ASPxGridView)sender).GetRowValues(e.VisibleIndex, "Dinamic"),0).ToString() == "1")
                        e.Visible = DevExpress.Utils.DefaultBoolean.False;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                object[] obj = grDateDosar.GetRowValues(grDateDosar.FocusedRowIndex, new string[] { "F10003", "IdObiect" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

                {
                    DataTable dt = new DataTable();
                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                    if (ds.Tables.Contains("Admin_Dosar"))
                    {
                        dt = ds.Tables["Admin_Dosar"];

                        DataRow dr = dt.Rows.Find(obj[1]);

                        if (dr == null)
                        {
                            dr = dt.NewRow();
                            dr["F10003"] = Session["Marca"];
                            dr["IdObiect"] = obj[1];
                            dt.Rows.Add(dr);
                        }

                        //string numeFis = Path.GetFileNameWithoutExtension(e.UploadedFile.FileName);
                        string extensie = Path.GetExtension(e.UploadedFile.FileName);
                        dr["Fisier"] = e.UploadedFile.FileBytes;
                        dr["FisierNume"] = e.UploadedFile.FileName;
                        dr["FisierExtensie"] = extensie;
                        dr["AreFisier"] = 1;

                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                }

                {
                    DataTable dtAll = Session["Tmp_Admin_Dosar"] as DataTable;
                    DataRow dr = dtAll.Rows.Find(obj[1]);
                    if (dr != null)
                    {
                        //string numeFis = Path.GetFileNameWithoutExtension(e.UploadedFile.FileName);
                        string extensie = Path.GetExtension(e.UploadedFile.FileName);
                        dr["Fisier"] = e.UploadedFile.FileBytes;
                        dr["FisierNume"] = e.UploadedFile.FileName;
                        dr["FisierExtensie"] = extensie;
                        dr["AreFisier"] = 1;

                        Session["Tmp_Admin_Dosar"] = dtAll;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    //if (arr.Length != 2 || arr[0] == "")
                    //{
                    //    grDateDosar.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                    //    return;
                    //}

                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                    DataTable dtAll = Session["Tmp_Admin_Dosar"] as DataTable;
                    object[] obj = grDateDosar.GetRowValues(grDateDosar.FocusedRowIndex, new string[] { "F10003", "IdObiect" }) as object[];

                    grDateDosar.CancelEdit();
                    
                    switch (arr[0])
                    {
                        case "btnDocUpload":
                            {
                                //NOP
                            }
                            break;
                        case "btnSterge":
                            {
                                {
                                    DataTable dt = new DataTable();
                                    if (ds.Tables.Contains("Admin_Dosar"))
                                    {
                                        //object[] arrKey = arr[1].Split('|');
                                        dt = ds.Tables["Admin_Dosar"];

                                        DataRow dr = dt.Rows.Find(obj[1]);
                                        dr.Delete();

                                        Session["InformatiaCurentaPersonal"] = ds;
                                    }
                                }

                                {
                                    //object[] arrKey = arr[1].Split('|');
                                    DataRow dr = dtAll.Rows.Find(obj[1]);
                                    if (dr != null)
                                    {
                                        dr["Descriere"] = null;
                                        dr["Fisier"] = null;
                                        dr["FisierNume"] = null;
                                        dr["FisierExtensie"] = null;
                                        dr["AreFisier"] = 0;

                                        Session["Tmp_Admin_Dosar"] = dtAll;
                                    }
                                    //dr.Delete();

                                    //Session["Tmp_Admin_Dosar"] = dtAll;
                                }
                            }
                            break;
                        case "btnStergeFisier":
                            {
                                

                                if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

                                {
                                    DataTable dt = new DataTable();
                                    if (ds.Tables.Contains("Admin_Dosar"))
                                    {
                                        dt = ds.Tables["Admin_Dosar"];
                                        DataRow dr = dt.Rows.Find(obj[1]);
                                        if (dr != null)
                                        {
                                            dr["Fisier"] = null;
                                            dr["FisierNume"] = null;
                                            dr["FisierExtensie"] = null;
                                            dr["AreFisier"] = 0;

                                            Session["InformatiaCurentaPersonal"] = ds;
                                        }
                                    }
                                }

                                {
                                    DataRow dr = dtAll.Rows.Find(obj[1]);
                                    if (dr != null)
                                    {
                                        dr["Fisier"] = null;
                                        dr["FisierNume"] = null;
                                        dr["FisierExtensie"] = null;
                                        dr["AreFisier"] = 0;

                                        Session["Tmp_Admin_Dosar"] = dtAll;
                                    }
                                }
                            }
                            break;
                    }

                    grDateDosar.KeyFieldName = "IdObiect";
                    grDateDosar.DataSource = dtAll;
                    grDateDosar.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDosar_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            try
            {
                if (e.RowType == GridViewRowType.Data)
                {
                    ASPxGridView grDateDosar = sender as ASPxGridView;

                    //GridViewDataColumn colUpload = grDateDosar.Columns["colAtas"].Columns["colUpload"] as GridViewDataColumn;
                    //ASPxUploadControl btnUpload = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colUpload, "btnDocUpload") as ASPxUploadControl;
                    //if (btnUpload != null)
                    //{
                    //    btnUpload.ClientSideEvents.FileUploadComplete = string.Format("function(s,e) {{ grDateDosar.PerformCallback('btnDocUpload;{0}'); }}", e.GetValue("IdObiect"));
                    //}

                    GridViewDataColumn colSterge = grDateDosar.Columns["colAtas"].Columns["colSterge"] as GridViewDataColumn;
                    ASPxButton btnSterge = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colSterge, "btnStergeFisier") as ASPxButton;
                    if (btnSterge != null)
                        btnSterge.ClientSideEvents.Click = "function(s,e) { if (" + General.Nz(e.GetValue("AreFisier"), 0).ToString() + " == 1) grDateDosar.PerformCallback('btnStergeFisier;" + General.Nz(e.GetValue("IdObiect"),-99).ToString() + "'); else { swal({ title: '', text: 'Nu exista fisier de sters', type: 'info' }); } }";


                    //btnSterge.ClientSideEvents.Click = string.Format("function(s,e) {{ If (" + General.Nz(e.GetValue("AreFisier"),0).ToString() + " == 1) grDateDosar.PerformCallback('btnStergeFisier;{0}'); else { swal({ title: '', text: s.cpAlertMessage, type: 'info' }); } }}", e.GetValue("IdObiect"));

                    GridViewDataColumn colArata = grDateDosar.Columns["colAtas"].Columns["colArata"] as GridViewDataColumn;
                    ASPxButton btnArata = grDateDosar.FindRowCellTemplateControl(e.VisibleIndex, colArata, "btnArata") as ASPxButton;
                    if (btnArata != null)
                        btnArata.ClientSideEvents.Click = "function(s,e) { if (" + General.Nz(e.GetValue("AreFisier"), 0).ToString() + " == 1) GoToAtasMode(" + General.Nz(e.GetValue("IdObiect"), -99).ToString() + "); else { swal({ title: '', text: 'Nu exista fisier de vizualizat', type: 'info' }); } }";


                    //btnArata.ClientSideEvents.Click = string.Format("function(s,e) {{ GoToAtasMode({0}); }}", e.GetValue("IdObiect"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected string GetImagePath(object valoare)
        {
            string rez = "~/Fisiere/Imagini/Icoane/cript.png";

            try
            {
                if (Convert.ToInt32(General.Nz(valoare, 0)) == 1)
                    rez = "~/Fisiere/Imagini/Icoane/decript.png";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }


        private void LoadData(ref DataRow dr, object idObiect, object descriere)
        {
            try
            {
                dr["F10003"] = Session["Marca"];
                dr["IdObiect"] = idObiect;
                dr["Descriere"] = descriere;
                //dr["Fisier"] = upd.NewValues["Fisier"] ?? DBNull.Value;
                //dr["FisierNume"] = upd.NewValues["FisierNume"] ?? DBNull.Value;
                //dr["FisierExtensie"] = upd.NewValues["FisierExtensie"] ?? DBNull.Value;
                //if (dr["Fisier"] == DBNull.Value)
                //    dr["AreFisier"] = 0;
                //else
                //    dr["AreFisier"] = 1;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}