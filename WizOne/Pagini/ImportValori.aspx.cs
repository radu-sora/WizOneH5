using DevExpress.Web;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class ImportValori : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion

                if (IsPostBack)
                {
                    if (Session["Import_ColFisier"] != null)
                    {
                        DataTable table = Session["Import_ColFisier"] as DataTable;
                        GridViewDataComboBoxColumn colFileCol = (grDate.Columns["ColoanaFisier"] as GridViewDataComboBoxColumn);
                        colFileCol.PropertiesComboBox.DataSource = table;

                    }

                    if (Session["Import_ColBD"] != null)
                    {
                        DataTable table = Session["Import_ColBD"] as DataTable;
                        GridViewDataComboBoxColumn colBDCol = (grDate.Columns["ColoanaBD"] as GridViewDataComboBoxColumn);
                        colBDCol.PropertiesComboBox.DataSource = table;

                    }

                    DataTable tbl = new DataTable();
                    tbl.Columns.Add("Id", typeof(string));
                    tbl.Columns.Add("Denumire", typeof(string));
                    tbl.Rows.Add("Numeric", "Numeric");
                    tbl.Rows.Add("String", "String");
                    GridViewDataComboBoxColumn colTip = (grDate.Columns["Tip"] as GridViewDataComboBoxColumn);
                    colTip.PropertiesComboBox.DataSource = tbl;

                    DataTable dt = new DataTable();
                    if (Session["Import_Grid"] == null)
                    {
                        dt = General.IncarcaDT("SELECT * FROM \"Eval_ImportSablon\" ", null);
                    }
                    else
                    {
                        dt = Session["Import_Grid"] as DataTable;
                    }
                    grDate.DataSource = dt;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
                    Session["Import_Grid"] = dt;


                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = General.IncarcaDT("SELECT * FROM \"Eval_ImportSablon\" ", null);
                    grDate.DataSource = dt;                 
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
                    Session["Import_Grid"] = dt; 

                    rbTip1.Checked = true;
                }

                cmbObiectiv.DataSource = General.IncarcaDT("SELECT \"IdObiectiv\", \"Obiectiv\" FROM \"Eval_Obiectiv\"", null);
                cmbObiectiv.DataBind();

                cmbComp.DataSource = General.IncarcaDT("SELECT a.\"IdCompetenta\", b.\"DenCategorie\" " + Dami.Operator() + " ' - ' " + Dami.Operator() + "  a.\"DenCompetenta\" as \"DenCompetenta\" FROM \"Eval_CategCompetenteDet\" a left join \"Eval_CategCompetente\" b on a.\"IdCategorie\" = b.\"IdCategorie\" ", null);
                cmbComp.DataBind();

                cmbPerioada.DataSource = General.IncarcaDT("SELECT \"IdPerioada\", \"DenPerioada\" FROM \"Eval_Perioada\"", null);
                cmbPerioada.DataBind();



            }
            catch (Exception ex)
            {               
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Eval_Import"));
                if (!folder.Exists)
                    folder.Create();

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }

                string path = folder.FullName + "\\Temp.xlsx";
                File.WriteAllBytes(path, btnDocUpload.UploadedFiles[0].FileBytes);

    
                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets["Sheet1"];

                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(string));
                table.Columns.Add("Denumire", typeof(string));

                int i = 0;
                while (!ws2.Cells[0, i].Value.IsEmpty)
                {
                    table.Rows.Add(ws2.Cells[0, i].Value, ws2.Cells[0, i].Value);
                    i++;
                }

                GridViewDataComboBoxColumn colFileCol = (grDate.Columns["ColoanaFisier"] as GridViewDataComboBoxColumn);
                colFileCol.PropertiesComboBox.DataSource = table;
                Session["Import_ColFisier"] = table;

                table = new DataTable();
                table.Columns.Add("Id", typeof(string));
                table.Columns.Add("Denumire", typeof(string));
                string sql = "SELECT * FROM \"Eval_ObiIndividualeTemp\"";
                if (rbTip2.Checked)
                    sql = "SELECT * FROM \"Eval_CompetenteAngajatTemp\"";
                DataTable dt = General.IncarcaDT(sql, null);
                for (int k = 0; k < dt.Columns.Count; k++)                
                    table.Rows.Add(dt.Columns[k].ColumnName, dt.Columns[k].ColumnName == "F10003" ? "Marca" : dt.Columns[k].ColumnName);                

                GridViewDataComboBoxColumn colDBCol = (grDate.Columns["ColoanaBD"] as GridViewDataComboBoxColumn);
                colDBCol.PropertiesComboBox.DataSource = table;
                Session["Import_ColBD"] = table;

                DataTable tbl = new DataTable();
                tbl.Columns.Add("Id", typeof(string));
                tbl.Columns.Add("Denumire", typeof(string));
                tbl.Rows.Add("Numeric", "Numeric");
                tbl.Rows.Add("String", "String");
                GridViewDataComboBoxColumn colTip = (grDate.Columns["Tip"] as GridViewDataComboBoxColumn);
                colTip.PropertiesComboBox.DataSource = tbl;

                grDate.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Import_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName)
                        {
                            case "IdAuto":
                                row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }
                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                //grDate.DataBind();
                grDate.KeyFieldName = "IdAuto";
                Session["Import_Grid"] = dt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Import_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;

                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Import_Grid"] = dt;
                grDate.DataSource = dt;
                //grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Import_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Import_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                //dt = General.IncarcaDT("SELECT * FROM \"tblTichete_WebService\" WHERE \"Companie\" = '" + cmbCompanie.Text + "'", null);
                grDate.DataSource = dt;
                grDate.DataBind();
                grDate.KeyFieldName = "IdAuto";
                Session["Import_Grid"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = Session["Import_Grid"] as DataTable;
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
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {   
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Eval_Import"));

                if (folder.GetFiles().Count() <= 0)
                {
                    MessageBox.Show("Nu ati incarcat niciun fisier!", MessageBox.icoError, "");
                    return;
                }

                if (cmbPerioada.Value == null)
                {
                    MessageBox.Show("Nu ati selectat perioada!", MessageBox.icoError, "");
                    return;
                }

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets["Sheet1"];
                int x = 0;
                for (int i = 0; i < grDate.VisibleRowCount; i++)
                {
                    DataRowView obj = grDate.GetRow(i) as DataRowView;
                    if (obj["ColoanaBD"].ToString() == "F10003")
                    {                       
                        while (!ws2.Cells[0, x].Value.IsEmpty)
                        {                            
                            if (ws2.Cells[0, x].Value.ToString() == obj["ColoanaFisier"].ToString())
                                break;
                            x++;
                        }
                    }
                }

                //Radu 29.10.2016 -  daca doar una dintre coloanele IdCalificativ si Calificativ este prezenta in sablon, cea absenta se va completa in functie de cea prezenta
                int y = 0, z = 0;
                bool calif = false, idCalif = false, necesitaCompletare = false;
                string campAbsent = "", campPrezent = "";
                for (int i = 0; i < grDate.VisibleRowCount; i++)
                {
                    DataRowView obj = grDate.GetRow(i) as DataRowView;
                    if (obj["ColoanaBD"].ToString() == "IdCalificativ")
                    {
                        while (!ws2.Cells[0, y].Value.IsEmpty)
                        {
                            if (ws2.Cells[0, y].Value.ToString() == obj["ColoanaFisier"].ToString())
                            {
                                idCalif = true;
                                break;                               
                            }
                            y++;
                        }
                    }
                    if (obj["ColoanaBD"].ToString() == "Calificativ")
                    {
                        while (!ws2.Cells[0, z].Value.IsEmpty)
                        {
                            if (ws2.Cells[0, z].Value.ToString() == obj["ColoanaFisier"].ToString())
                            {
                                calif = true;
                                break;                               
                            }
                            z++;
                        }
                    }
                }    
                if ((!idCalif && calif) || (idCalif && !calif))
                {
                    necesitaCompletare = true;
                    if (!idCalif)
                    {
                        campAbsent = ", \"IdCalificativ\" = (SELECT \"IdCalificativ\" FROM \"Eval_SetCalificativDet\" WHERE UPPER(\"Denumire\") = '{0}')";
                        campPrezent = "Calificativ";
                    }
                    if (!calif)
                    {
                        campAbsent = ", \"Calificativ\" = (SELECT \"Denumire\" FROM \"Eval_SetCalificativDet\" WHERE \"IdCalificativ\" = {0})";
                        campPrezent = "IdCalificativ";
                    }
                }
                

                //if (x >= )
                //{
                //    MessageBox.Show("Nu ati specificat coloana pentru marca!", MessageBox.icoError, "");
                //    return;
                //}

                for (int i = 0; i < grDate.VisibleRowCount; i++)
                {
                    DataRowView obj = grDate.GetRow(i) as DataRowView;
                    if (obj["ColoanaBD"].ToString() != "F10003")
                    {

                        int k = 0;               
                        while (!ws2.Cells[0, k].Value.IsEmpty)
                        {
                            if (ws2.Cells[0, k].Value.ToString() == obj["ColoanaFisier"].ToString())
                                break;
                            k++;
                        }
                                               
                        int j = 1;
                        while (!ws2.Cells[j, k].Value.IsEmpty)
                        {
                            string cmp = "";
                            if (necesitaCompletare && obj["ColoanaBD"].ToString() == campPrezent)                            
                                cmp = string.Format(campAbsent, ws2.Cells[j, k].Value.ToString());                            

                            string sql = "";
                            if (rbTip1.Checked)
                            {
                                if (obj["Tip"].ToString() == "String")
                                    sql = "UPDATE \"Eval_ObiIndividualeTemp\" SET \"" + obj["ColoanaBD"].ToString() + "\" = '" + ws2.Cells[j, k].Value.ToString() + "' " + cmp + " WHERE F10003 = " + ws2.Cells[j, x].Value.ToString() + (cmbObiectiv.Value == null ? "" : " AND \"IdObiectiv\" = " + cmbObiectiv.Value.ToString()) 
                                        + " AND COALESCE(\"IdPeriod\", 1) =  " + Convert.ToInt32(cmbPerioada.Value);
                                else
                                    sql = "UPDATE \"Eval_ObiIndividualeTemp\" SET \"" + obj["ColoanaBD"].ToString() + "\" = " + ws2.Cells[j, k].Value.ToString(new CultureInfo("en-US")) + cmp + " WHERE F10003 = " + ws2.Cells[j, x].Value.ToString() + (cmbObiectiv.Value == null ? "" : " AND \"IdObiectiv\" = " + cmbObiectiv.Value.ToString())
                                        + " AND COALESCE(\"IdPeriod\", 1) =  " + Convert.ToInt32(cmbPerioada.Value);
                            }
                            else
                            {
                                if (obj["Tip"].ToString() == "String")
                                    sql = "UPDATE \"Eval_CompetenteAngajatTemp\" SET \"" + obj["ColoanaBD"].ToString() + "\" = '" + ws2.Cells[j, k].Value.ToString() + "' " + cmp + " WHERE F10003 = " + ws2.Cells[j, x].Value.ToString() + (cmbComp.Value == null ? "" : " AND \"IdCompetenta\" = " + cmbComp.Value.ToString())
                                          + " AND COALESCE(\"IdPeriod\", 1) =  " + Convert.ToInt32(cmbPerioada.Value);
                                else
                                    sql = "UPDATE \"Eval_CompetenteAngajatTemp\" SET \"" + obj["ColoanaBD"].ToString() + "\" = " + ws2.Cells[j, k].Value.ToString(new CultureInfo("en-US")) + cmp + " WHERE F10003 = " + ws2.Cells[j, x].Value.ToString() + (cmbComp.Value == null ? "" : " AND \"IdCompetenta\" = " + cmbComp.Value.ToString())
                                          + " AND COALESCE(\"IdPeriod\", 1) =  " + Convert.ToInt32(cmbPerioada.Value);

                            }
                            General.ExecutaNonQuery(sql, null);
                            j++;
                        }                           
                    }
                }

                MessageBox.Show("Import terminat cu succes!", MessageBox.icoSuccess);

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtGrid = Session["Import_Grid"] as DataTable;
                //SqlDataAdapter da = new SqlDataAdapter();
                //SqlCommandBuilder cb = new SqlCommandBuilder();

                //da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"Eval_ImportSablon\"", null);
                //cb = new SqlCommandBuilder(da);
                //da.Update(dtGrid);
                //da.Dispose();
                //da = null;
                General.SalveazaDate(dtGrid, "Eval_ImportSablon");

                MessageBox.Show("Salvare reusita!", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void btnNotif_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = Notif.TrimiteNotificare("Pagini.ImportValori", (int)Constante.TipNotificare.Notificare, "SELECT 1 " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "")
                    MessageBox.Show(msg, MessageBox.icoError, "");
                else
                    MessageBox.Show("Notificare trimisa cu succes!", MessageBox.icoSuccess);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

    }
}