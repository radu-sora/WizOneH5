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
    public partial class ImportDate : System.Web.UI.Page
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
                    //if (Session["ImportDate_Sablon"] != null)
                    //{
                    //    cmbSablon.Value = Convert.ToInt32(Session["ImportDate_Sablon"].ToString());
                    //    Session["ImportDate_Reload"] = "0";
                    //}

                    if (Session["ImportDate_ColFisier"] != null)
                    {
                        DataTable table = Session["ImportDate_ColFisier"] as DataTable;
                        GridViewDataComboBoxColumn colFileCol = (grDateNomen.Columns["ColoanaFisier"] as GridViewDataComboBoxColumn);
                        colFileCol.PropertiesComboBox.DataSource = table;

                    }

                    if (Session["ImportDate_ColBD"] != null)
                    {
                        DataTable table = Session["ImportDate_ColBD"] as DataTable;
                        GridViewDataComboBoxColumn colBDCol = (grDateNomen.Columns["ColoanaBD"] as GridViewDataComboBoxColumn);
                        colBDCol.PropertiesComboBox.DataSource = table;

                    }
     

                    DataTable dt = new DataTable();
                    if (Session["ImportDateSablon_Grid"] == null)
                    {
                        dt = General.IncarcaDT("SELECT * FROM \"Template\" ", null);
                    }
                    else
                    {
                        dt = Session["ImportDateSablon_Grid"] as DataTable;
                    }
                                       
                    grDate.DataSource = dt;
                    grDate.KeyFieldName = "Id";
                    grDate.DataBind();
                    Session["ImportDateSablon_Grid"] = dt;
                    

                    if (Session["ImportDateNomen_Grid"] == null)
                    {
                        if (cmbSablon.Value != null)
                            dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = " + Convert.ToInt32(cmbSablon.Value), null);
                        else
                            dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = -1 ", null);
                    }
                    else
                    {
                        dt = Session["ImportDateNomen_Grid"] as DataTable;
                    }

                    grDateNomen.DataSource = dt;
                    grDateNomen.KeyFieldName = "Id;IdAuto";
                    grDateNomen.DataBind();
                    Session["ImportDateNomen_Grid"] = dt;

                    if (Session["ImportDate_Previz"] != null)
                    {
                        DataTable dtViz = Session["ImportDate_Previz"] as DataTable;
                        grDateViz.DataSource = dtViz;
                        grDateViz.DataBind();
                    }
                    else
                    {
                        grDateViz.DataSource = null;
                        grDateViz.DataBind();
                    }
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt = General.IncarcaDT("SELECT * FROM \"Template\" ", null);               

                    grDate.DataSource = dt;
                    grDate.KeyFieldName = "Id";
                    grDate.DataBind();
                    Session["ImportDateSablon_Grid"] = dt;

                    //dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" ", null);
                    //grDateNomen.DataSource = dt;
                    //grDateNomen.KeyFieldName = "Id;IdAuto";
                    //grDateNomen.DataBind();
                    //Session["ImportDateNomen_Grid"] = dt;   

                    //Session["ImportDate_Reload"] = "1";

                    Session["ImportDate_Marca"] = null;

                }

                //if (Constante.tipBD == 1)
                //{
                //    SqlDataSource ds = new SqlDataSource();
                //    ds.EnableCaching = false;
                //    ds.ConnectionString = Constante.cnnWeb;
                //    ds.SelectCommand = "SELECT \"Id\",  \"NumeSablon\" as \"Denumire\", coalesce(DESCRIERE, \"NumeTabela\") as \"Tabela\" FROM \"Template\" left join \"ALIASTAB\" on NUME = \"NumeTabela\" ORDER BY \"Denumire\"";
                //    cmbSablon.DataSource = ds;
                //    cmbSablon.DataBind();
                //}

                DataTable dtSablon = General.IncarcaDT("SELECT \"Id\",  \"NumeSablon\" as \"Denumire\", coalesce(DESCRIERE, \"NumeTabela\") as \"Tabela\" FROM \"Template\" left join \"ALIASTAB\" on NUME = \"NumeTabela\" ORDER BY \"Denumire\"", null);
                cmbSablon.DataSource = dtSablon;
                cmbSablon.DataBind();

                grDateViz.SettingsPager.PageSize = 20;
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
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/ImportDate"));
                if (!folder.Exists)
                    folder.Create();

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }

                string path = folder.FullName + "\\Temp." + btnDocUpload.UploadedFiles[0].FileName.Split('.')[btnDocUpload.UploadedFiles[0].FileName.Split('.').Length - 1];
                Session["ImportDate_Extensie"] = btnDocUpload.UploadedFiles[0].FileName.Split('.')[btnDocUpload.UploadedFiles[0].FileName.Split('.').Length - 1];
                File.WriteAllBytes(path, btnDocUpload.UploadedFiles[0].FileBytes);  

                if (cmbSablon.Text.Length > 0)
                    IncarcaGrid();

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void IncarcaGrid()
        {
            try 
            {
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/ImportDate"));
                if (!folder.Exists)
                    folder.Create();

                if (Session["ImportDate_Extensie"] == null)
                    return;

                if (File.Exists(folder.FullName + "\\Temp." + Session["ImportDate_Extensie"].ToString()))
                {
                    DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                    workbook.LoadDocument(folder.FullName + "\\Temp." + Session["ImportDate_Extensie"].ToString(), 
                        (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSX" ? DevExpress.Spreadsheet.DocumentFormat.Xlsx : (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSM" ? DevExpress.Spreadsheet.DocumentFormat.Xlsm : DevExpress.Spreadsheet.DocumentFormat.Xls)));

             
                    DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets[0];

                    DataTable table = new DataTable();
                    table.Columns.Add("Id", typeof(string));
                    table.Columns.Add("Denumire", typeof(string));

                    int i = 0;
                    while (!ws2.Cells[0, i].Value.IsEmpty && ws2.Cells[0, i].Value.ToString().Length > 0)
                    {
                        table.Rows.Add(ws2.Cells[0, i].Value, ws2.Cells[0, i].Value);
                        i++;
                    }

                    GridViewDataComboBoxColumn colFileCol = (grDateNomen.Columns["ColoanaFisier"] as GridViewDataComboBoxColumn);
                    colFileCol.PropertiesComboBox.DataSource = table;
                    Session["ImportDate_ColFisier"] = table;

                    table = new DataTable();
                    table.Columns.Add("Id", typeof(string));
                    table.Columns.Add("Denumire", typeof(string));

                    int id = -1;
                    if (cmbSablon.Value != null)
                        id = Convert.ToInt32(cmbSablon.Value);
                    else
                    {
                        object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "NumeSablon", "NumeTabela" }) as object[];
                        if (obj != null && obj.Count() > 0)
                            id = Convert.ToInt32(obj[0]);
                    }

                    string sql = "SELECT * FROM \"Template\" WHERE \"Id\" =  " + id;
                    DataTable dtSablon = General.IncarcaDT(sql, null);
                    if (dtSablon != null && dtSablon.Rows.Count > 0)
                    {
                        //sql = "SELECT * FROM \"" + dtSablon.Rows[0]["NumeTabela"].ToString().Trim() + "\"";
                        if (Constante.tipBD == 1)
                        {
                            sql = " SELECT COLUMN_NAME, coalesce(DESCRIERE, COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS left join ALIASCMP ON TABELA = TABLE_NAME AND COLUMN_NAME = CAMP WHERE  TABLE_NAME = '" + dtSablon.Rows[0]["NumeTabela"].ToString().Trim() + "'";
                            if (dtSablon.Rows[0]["NumeTabela"].ToString().Trim() == "F100")                            
                                sql += " UNION SELECT COLUMN_NAME, coalesce(DESCRIERE, COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS left join ALIASCMP ON TABELA = TABLE_NAME AND COLUMN_NAME = CAMP WHERE  TABLE_NAME = 'F1001'";                            
                        }
                        else
                        {
                            sql = "SELECT COLUMN_NAME, COALESCE(DESCRIERE, COLUMN_NAME) FROM user_tab_columns left join ALIASCMP ON TABELA = TABLE_NAME AND CAMP = COLUMN_NAME WHERE  TABLE_NAME = '" + dtSablon.Rows[0]["NumeTabela"].ToString().Trim() + "'";
                            if (dtSablon.Rows[0]["NumeTabela"].ToString().Trim() == "F100")                            
                                sql += " UNION SELECT COLUMN_NAME, COALESCE(DESCRIERE, COLUMN_NAME) FROM user_tab_columns left join ALIASCMP ON TABELA = TABLE_NAME AND CAMP = COLUMN_NAME WHERE  TABLE_NAME = 'F1001'";                            
                        }

                        DataTable dt = General.IncarcaDT(sql, null);
                        //for (int k = 0; k < dt.Columns.Count; k++)
                        //    table.Rows.Add(dt.Columns[k].ColumnName, dt.Columns[k].ColumnName);
                        for (int k = 0; k < dt.Rows.Count; k++)
                            table.Rows.Add(dt.Rows[k][0].ToString(), dt.Rows[k][1].ToString());

                        GridViewDataComboBoxColumn colDBCol = (grDateNomen.Columns["ColoanaBD"] as GridViewDataComboBoxColumn);
                        colDBCol.PropertiesComboBox.DataSource = table;
                        Session["ImportDate_ColBD"] = table;

                        grDateNomen.DataBind();
                    }
                }      
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["ImportDateSablon_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName)
                        {
                            case "Id":
                                row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("Id")), 0)) + 1;
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
                grDate.KeyFieldName = "Id";
                Session["ImportDateSablon_Grid"] = dt;
                General.SalveazaDate(dt, "Template");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["ImportDateSablon_Grid"] as DataTable;

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
                Session["ImportDateSablon_Grid"] = dt;
                grDate.DataSource = dt;
                General.SalveazaDate(dt, "Template");
                //grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["ImportDateSablon_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["ImportDateSablon_Grid"] = dt;
                grDate.DataSource = dt;
                General.SalveazaDate(dt, "Template");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateViz_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                if (e.Parameters.Equals("clear"))
                {
                    grDateViz.DataSource = null;
                    grDateViz.DataBind();
                    grDateViz.Columns.Clear();
                    Session["ImportDate_Previz"] = null;
                    return;
                }

                if (e.Parameters.Equals("1"))
                {
                    btnImport_Click();
                    return;
                }


                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/ImportDate"));
                if (folder.GetFiles().Count() <= 0)
                {
                    return;
                }

                //if (Session["ImportDate_Reload"].ToString() == "0")
                //{
                //    Session["ImportDate_Reload"] = "1";
                //    return;
                //}

                DataTable dt = new DataTable();
                for (int i = 0; i < grDateNomen.VisibleRowCount; i++)
                {
                    DataRowView obj = grDateNomen.GetRow(i) as DataRowView;
                    if (obj["ColoanaFisier"] != null && obj["ColoanaFisier"].ToString().Length > 0)
                        dt.Columns.Add(obj["ColoanaFisier"].ToString(), typeof(string));
                }

                if (Session["ImportDate_Extensie"] == null)
                    return;

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp." + Session["ImportDate_Extensie"].ToString(),
                    (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSX" ? DevExpress.Spreadsheet.DocumentFormat.Xlsx : (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSM" ? DevExpress.Spreadsheet.DocumentFormat.Xlsm : DevExpress.Spreadsheet.DocumentFormat.Xls)));

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets[0];
                Dictionary<int, int> lstIndex = new Dictionary<int, int>();

                int k = 0, nrCol = 0;                
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    k = 0;
                    while (!ws2.Cells[0, k].Value.IsEmpty && ws2.Cells[0, k].Value.ToString().Length > 0)
                    {
                        if (ws2.Cells[0, k].Value.ToString().Trim() == dt.Columns[i].ColumnName)
                        {
                            lstIndex.Add(k, i);
                            break;
                        }
                        k++;
                    }
                    if (k > nrCol)
                        nrCol = k;
                }

                Session["ImportDate_NrColoane"] = nrCol;

                string[] sir = new string[dt.Columns.Count];
                for (int i = 0; i < sir.Length; i++)
                    sir[i] = "";

                int j = 1;
                k = 0;
                while (!ws2.Cells[j, k].Value.IsEmpty && ws2.Cells[j, k].Value.ToString().Length > 0)
                {
                    //if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                    //{
                    //    j++;
                    //    k = 0;
                    //    continue;
                    //}

                    dt.Rows.Add(sir);
                    //while (!ws2.Cells[j, k].Value.IsEmpty)
                    for (int x = 0; x <= nrCol; x++)
                    {
                        //if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                        //{
                        //    k++;
                        //    continue;
                        //}    

                        if (lstIndex.ContainsKey(k))
                            dt.Rows[j - 1][lstIndex[k]] = ws2.Cells[j, k].Value;
                        k++;
                    }
                    j++;
                    k = 0;
                }
                
                dt.Columns.Add("Actiune", typeof(string));
                dt.Columns.Add("MesajEroare", typeof(string));

                grDateViz.DataSource = dt;
                grDateViz.DataBind();
                Session["ImportDate_Previz"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            { 
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
                DataTable dt = Session["ImportDateSablon_Grid"] as DataTable;
                if (dt.Columns["Id"] != null)
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("Id")), 0)) + 1;
                        e.NewValues["Id"] = max;
                    }
                    else
                        e.NewValues["Id"] = 1;
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


        protected void grDateNomen_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["ImportDateNomen_Grid"] as DataTable;

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
                            case "Id":
                                row[x] = Convert.ToInt32(Session["ImportDate_Sablon"].ToString());
                                break;
                            case "Tabela":
                                string sql = "SELECT * FROM \"Template\" WHERE \"Id\" =  " + Convert.ToInt32(Session["ImportDate_Sablon"].ToString());
                                DataTable dtSablon = General.IncarcaDT(sql, null);
                                if (dtSablon != null && dtSablon.Rows.Count > 0)
                                {
                                    if (dtSablon.Rows[0]["NumeTabela"].ToString().Trim() == "F100" && e.NewValues["ColoanaBD"] != null)
                                    {
                                        if (Constante.tipBD == 1)                                        
                                            sql = " SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS  WHERE  (TABLE_NAME = 'F100' OR  TABLE_NAME = 'F1001') AND COLUMN_NAME = '" + e.NewValues["ColoanaBD"].ToString() + "' ORDER BY TABLE_NAME";                                        
                                        else                                                                                  
                                            sql = "SELECT TABLE_NAME FROM user_tab_columns  WHERE  (TABLE_NAME = 'F100' OR  TABLE_NAME = 'F1001') AND COLUMN_NAME = '" + e.NewValues["ColoanaBD"].ToString() + "' ORDER BY TABLE_NAME";                       

                                        DataTable dtTab = General.IncarcaDT(sql, null);
                                        row[x] = dtTab.Rows[0][0].ToString();
                                    }
                                }
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }
                    x++;
                }

                //bool err = false;
                //if (e.NewValues["ColoanaBD"] == null || e.NewValues["ColoanaBD"].ToString().Length <= 0)
                //    err = true;

                //if ((e.NewValues["ColoanaFisier"] == null || e.NewValues["ColoanaFisier"].ToString().Length <= 0) && (e.NewValues["ValoareImplicita"] == null || e.NewValues["ValoareImplicita"].ToString().Length <= 0))
                //    err = true;

                //if (!err)
                    dt.Rows.Add(row);
                e.Cancel = true;
                grDateNomen.CancelEdit();
                grDateNomen.DataSource = dt;
                //grDate.DataBind();
                grDateNomen.KeyFieldName = "IdAuto;Id";
                Session["ImportDateNomen_Grid"] = dt;

                General.SalveazaDate(dt, "TemplateCampuri");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateNomen_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["ImportDateNomen_Grid"] as DataTable;


                //bool err = false;
                //if (e.NewValues["ColoanaBD"] == null || e.NewValues["ColoanaBD"].ToString().Length <= 0)
                //    err = true;

                //if ((e.NewValues["ColoanaFisier"] == null || e.NewValues["ColoanaFisier"].ToString().Length <= 0) && (e.NewValues["ValoareImplicita"] == null || e.NewValues["ValoareImplicita"].ToString().Length <= 0))
                //    err = true;


                DataRow row = dt.Rows.Find(keys);

                //if (!err)
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateNomen.Columns[col.ColumnName] != null && grDateNomen.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;

                    }
                    if (col.ColumnName == "Tabela")
                    {
                        string sql = "SELECT * FROM \"Template\" WHERE \"Id\" =  " + Convert.ToInt32(Session["ImportDate_Sablon"].ToString());
                        DataTable dtSablon = General.IncarcaDT(sql, null);
                        if (dtSablon != null && dtSablon.Rows.Count > 0)
                        {
                            if (dtSablon.Rows[0]["NumeTabela"].ToString().Trim() == "F100" && e.NewValues["ColoanaBD"] != null)
                            {
                                if (Constante.tipBD == 1)
                                    sql = " SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS  WHERE  (TABLE_NAME = 'F100' OR  TABLE_NAME = 'F1001') AND COLUMN_NAME = '" + e.NewValues["ColoanaBD"].ToString() + "' ORDER BY TABLE_NAME";
                                else
                                    sql = "SELECT TABLE_NAME FROM user_tab_columns  WHERE  (TABLE_NAME = 'F100' OR  TABLE_NAME = 'F1001') AND COLUMN_NAME = '" + e.NewValues["ColoanaBD"].ToString() + "' ORDER BY TABLE_NAME";

                                DataTable dtTab = General.IncarcaDT(sql, null);
                                row[col.ColumnName] = dtTab.Rows[0][0].ToString();
                            }
                        }
                    }

                }

                e.Cancel = true;
                grDateNomen.CancelEdit();
                Session["ImportDateNomen_Grid"] = dt;
                grDateNomen.DataSource = dt;
                //grDate.DataBind();

                General.SalveazaDate(dt, "TemplateCampuri");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateNomen_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["ImportDateNomen_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                //dt.AcceptChanges();

                e.Cancel = true;
                grDateNomen.CancelEdit();
                Session["ImportDateNomen_Grid"] = dt;
                grDateNomen.DataSource = dt;

                General.SalveazaDate(dt, "TemplateCampuri");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDateNomen_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                //if (Session["ImportDate_Reload"].ToString() == "0")                
                //    return;                

                if (e.Parameters.Equals("1"))
                {
                    btnViz_Click();
                    return;
                }

                int id = -1;

                if (cmbSablon.Value != null)
                    id = Convert.ToInt32(cmbSablon.Value);

                if (e.Parameters.Equals("2"))
                {                    
                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "NumeSablon", "NumeTabela" }) as object[];
                    if (obj != null && obj.Count() > 0)
                        id = Convert.ToInt32(obj[0]);
                }

                Session["ImportDate_Sablon"] = id;
                DataTable dt = new DataTable();
                //if (cmbSablon.Value != null)      
                //    dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = " + Convert.ToInt32(cmbSablon.Value), null);
                //else
                //    dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" =  -1", null);

                dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = " + id, null);

                grDateNomen.DataSource = dt;
                grDateNomen.KeyFieldName = "Id;IdAuto";
                grDateNomen.DataBind();
                Session["ImportDateNomen_Grid"] = dt;
                Session["ImportDate_ColBD"] = null;
                Session["ImportDate_ColFisier"] = null;
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateNomen_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = Session["ImportDateNomen_Grid"] as DataTable;
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

        protected void grDateNomen_DataBinding(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }

        //protected void btnImport_Click(object sender, EventArgs e)

        protected void btnImport_Click()
        {
            try
            {   
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Temp/ImportDate"));

                if (folder.GetFiles().Count() <= 0)
                {
                    MessageBox.Show("Nu ati incarcat niciun fisier!", MessageBox.icoError, "");
                    return;
                }  
                
                if (cmbSablon.Text.Length <= 0)
                {
                    MessageBox.Show("Nu ati selectat sablonul!", MessageBox.icoError, "");
                    return;
                }

                if (Session["ImportDate_Extensie"] == null)
                    return;

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp." + Session["ImportDate_Extensie"].ToString(),
                    (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSX" ? DevExpress.Spreadsheet.DocumentFormat.Xlsx : (Session["ImportDate_Extensie"].ToString().ToUpper() == "XLSM" ? DevExpress.Spreadsheet.DocumentFormat.Xlsm : DevExpress.Spreadsheet.DocumentFormat.Xls)));

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets[0];

                //DataTable dtSablon = General.IncarcaDT("SELECT * FROM \"ImportDateSablon\" WHERE \"NumeTabela\" = '" + cmbTabela.Text + "'", null);
                //DataTable dtNomen = General.IncarcaDT("SELECT * FROM \"ImportDateNomen\" WHERE \"NumeTabela\" = '" + cmbTabela.Text + "' AND \"Obligatoriu\" = 1", null);
                //string err = "";
                //for (int i = 0; i < dtNomen.Rows.Count; i++)
                //{
                //    bool gasit = false;
                //    for (int l = 0; l < dtSablon.Rows.Count; l++)
                //    {
                //        if (dtNomen.Rows[i]["NumeColoana"].ToString() == dtSablon.Rows[l]["ColoanaBD"].ToString())
                //        {
                //            gasit = true;
                //            break;
                //        }
                //    }
                //    if (!gasit)
                //        err += ", " + dtNomen.Rows[i]["NumeColoana"].ToString();
                //}

                //if (err.Length > 0)
                //{
                //    MessageBox.Show("Pentru tabela " + cmbTabela.Text + ", nu ati precizat corespondenta pentru urmatoarele coloane obligatorii: " + err.Substring(1) + "!", MessageBox.icoError, "");
                //    return;
                //}

                string numeTabela = "";
                string sql = "SELECT * FROM \"Template\" WHERE \"Id\" =  " + Convert.ToInt32(cmbSablon.Value);
                DataTable dtSablon = General.IncarcaDT(sql, null);
                if (dtSablon != null && dtSablon.Rows.Count > 0)
                    numeTabela = dtSablon.Rows[0]["NumeTabela"].ToString().Trim();
                if (numeTabela.Length <= 0)
                    return;


                int k = 0;
                //sql = "select max(ColoanaFisier) as ColoanaFisier,  NumeColoana,  max(Obligatoriu) as Obligatoriu, max(ValoareImplicita) as ValoareImplicita, null as PozitieFisier, max(Tip) as Tip from "
                //            + "(select ColoanaFisier, ColoanaBD as NumeColoana, 0 as Obligatoriu, '' as ValoareImplicita, null AS PozitieFisier, "
                //            + "(SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = ColoanaBD) as Tip FROM ImportDateSablon    WHERE NumeTabela = '" + numeTabela + "' "
                //            + "union "
                //            + "select '' as ColoanaFisier, NumeColoana, coalesce(Obligatoriu, 0),  ValoareImplicita, NULL AS PozitieFisier, "
                //            + "(SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = NumeColoana ) as Tip FROM ImportDateNomen    WHERE NumeTabela = '" + numeTabela + "') a "
                //            + "group by numecoloana, PozitieFisier ";
                //if (Constante.tipBD == 2)
                //    sql = "select max(\"ColoanaFisier\") as \"ColoanaFisier\",  \"NumeColoana\",  max(\"Obligatoriu\") as \"Obligatoriu\", max(\"ValoareImplicita\") as \"ValoareImplicita\", null as \"PozitieFisier\", max(\"Tip\") as \"Tip\" from "
                //            + "(select \"ColoanaFisier\", \"ColoanaBD\" as \"NumeColoana\", 0 as \"Obligatoriu\", '' as \"ValoareImplicita\", null AS \"PozitieFisier\", "
                //            + "(SELECT DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = \"ColoanaBD\") as \"Tip\" FROM \"ImportDateSablon\"    WHERE \"NumeTabela\" = '" + numeTabela + "' "
                //            + "union "
                //            + "select '' as \"ColoanaFisier\", \"NumeColoana\", coalesce(\"Obligatoriu\", 0),  \"ValoareImplicita\", NULL AS \"PozitieFisier\", "
                //            + "(SELECT DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = \"NumeColoana\" ) as \"Tip\" FROM \"ImportDateNomen\"    WHERE \"NumeTabela\" = '" + numeTabela + "') a "
                //            + "group by \"NumeColoana\", \"PozitieFisier\"";

                sql = " select ColoanaFisier, ColoanaBD as NumeColoana, Obligatoriu, OmiteLaActualizare, ValoareImplicita, null as PozitieFisier, "
                      + " (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = ColoanaBD " 
                      + " UNION SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = 'F1001' and COLUMN_NAME = ColoanaBD) as Tip, Tabela "
                      + "  from TemplateCampuri a " 
                      + "  left join Template b on a.id = b.Id where b.Id = '" + Convert.ToInt32(cmbSablon.Value).ToString()  + "' ";
                if (Constante.tipBD == 2)
                    sql = " select \"ColoanaFisier\", \"ColoanaBD\" as \"NumeColoana\", \"Obligatoriu\", \"OmiteLaActualizare\", \"ValoareImplicita\", null as \"PozitieFisier\", "
                      + " (SELECT DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = \"ColoanaBD\" " 
                      + "UNION SELECT DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = 'F1001' and COLUMN_NAME = \"ColoanaBD\") as \"Tip\", \"Tabela\" "
                      + "  from \"TemplateCampuri\" a "
                      + "  left join \"Template\" b on a.\"Id\" = b.\"Id\" where b.\"Id\" = '" + Convert.ToInt32(cmbSablon.Value).ToString() + "' ";
                               
                DataTable dtCombinat = General.IncarcaDT(sql, null);
                foreach (DataColumn col in dtCombinat.Columns)
                    col.ReadOnly = false;

                for (int i = 0; i < dtCombinat.Rows.Count; i++)
                {
                    k = 0;
                    while (!ws2.Cells[0, k].Value.IsEmpty && ws2.Cells[0, k].Value.ToString().Length > 0)
                    {
                        if (ws2.Cells[0, k].Value.ToString().Trim() == dtCombinat.Rows[i]["ColoanaFisier"].ToString())
                        {
                            dtCombinat.Rows[i]["PozitieFisier"] = k;
                            break;
                        }
                        k++;
                    }
                }

                bool utilizator = false, timp = false;                
                sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = 'USER_NO'";
                if (Constante.tipBD == 2)
                    sql = "SELECT COUNT(*) FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = 'USER_NO'";
                DataTable dtVer = General.IncarcaDT(sql, null);
                if (dtVer != null && dtVer.Rows.Count > 0 && dtVer.Rows[0][0] != null && Convert.ToInt32(dtVer.Rows[0][0].ToString()) > 0)
                    utilizator = true;
                sql = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = 'TIME'";
                if (Constante.tipBD == 2)
                    sql = "SELECT COUNT(*) FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = 'TIME'";
                dtVer = General.IncarcaDT(sql, null);
                if (dtVer != null && dtVer.Rows.Count > 0 && dtVer.Rows[0][0] != null && Convert.ToInt32(dtVer.Rows[0][0].ToString()) > 0)
                    timp = true;

                int j = 1;
                k = 0;
                string msgErr = "";
                int idSablon = -99;
                string marcaInit = "";
                string idCerere = "";
                DataTable dtViz = Session["ImportDate_Previz"] as DataTable;
                int nrCol = Convert.ToInt32(Session["ImportDate_NrColoane"].ToString());
                while (!ws2.Cells[j, k].Value.IsEmpty && ws2.Cells[j, k].Value.ToString().Length > 0)
                {
                    if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                    {
                        k = 0;
                        j++;
                        continue;
                    }

                    string campOblig = "";
                    string campNonOblig = "";
                    string campNonObligAct = "", campNonObligAct2 = "";
                    //while (!ws2.Cells[j, k].Value.IsEmpty)
                    for (int x = 0; x <= nrCol; x++)
                    {

                        //if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                        //{
                        //    k++;
                        //    continue;
                        //}

                        DataRow[] dr = dtCombinat.Select("PozitieFisier=" + k);
                        if (dr != null && dr.Count() > 0)
                        {
                            string val = "NULL";
                            switch (dr[0]["Tip"].ToString().ToLower())
                            {
                                case "int":
                                case "numeric":
                                case "number":
                                case "decimal":
                                    if (ws2.Cells[j, k].Value != null && ws2.Cells[j, k].Value.ToString().Length > 0)
                                        val = ws2.Cells[j, k].Value.ToString(new CultureInfo("en-US"));
                                    break;
                                case "nvarchar":
                                case "varchar":
                                case "varchar2":
                                case "nvarchar2":
                                    if (ws2.Cells[j, k].Value != null && ws2.Cells[j, k].Value.ToString().Length > 0)
                                        val = "'" + ws2.Cells[j, k].Value.ToString().Replace("'","''") + "'";
                                    break;
                                case "date":
                                case "datetime":
                                    if (ws2.Cells[j, k].Value != null && ws2.Cells[j, k].Value.ToString().Length > 0)
                                    {
                                        if (Constante.tipBD == 1)
                                            val = "CONVERT(DATETIME#&* '" + ws2.Cells[j, k].Value.ToString() + "'#&* 103)";
                                        else
                                            val = "TO_DATE('" + ws2.Cells[j, k].Value.ToString() + "'#&* 'dd/mm/yyyy HH24:mi:ss')";
                                    }
                                    break;
                            }


                            val = val.Replace(",", "#&*");

                            if (dr[0]["Obligatoriu"].ToString() == "1")
                            {
                                campOblig += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
                                if (dr[0]["NumeColoana"].ToString() == "F10096" && val != "NULL")
                                    idSablon = Convert.ToInt32(val);
                                if (dr[0]["NumeColoana"].ToString() == "F10003")
                                {
                                    Session["ImportDate_Marca"] = val;
                                    marcaInit = val;
                                }
                                if ((numeTabela == "Avs_Cereri" || numeTabela == "Ptj_Cereri") && dr[0]["NumeColoana"].ToString() == "Id")
                                {
                                    idCerere = val;
                                }
                                if (val == "NULL")
                                    msgErr = "Lipsa valoare camp " + dr[0]["NumeColoana"].ToString();
                            }
                            else
                            {
                                campNonOblig += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
                                if (dr[0]["OmiteLaActualizare"] == null || dr[0]["OmiteLaActualizare"].ToString().Length <= 0 || Convert.ToInt32(dr[0]["OmiteLaActualizare"].ToString()) == 0)
                                {
                                    if (dr[0]["Tabela"] != null && dr[0]["Tabela"].ToString().Length > 0 && dr[0]["Tabela"].ToString() == "F1001")
                                        campNonObligAct2 += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
                                    else
                                        campNonObligAct += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
                                }
                                if (dr[0]["NumeColoana"].ToString() == "F10096" && val != "NULL")
                                    idSablon = Convert.ToInt32(val);                    
                            }
                        }
                        k++;
                    }

                    if (msgErr.Length > 0)
                    {
                        dtViz.Rows[j - 1]["MesajEroare"] = msgErr;
                        msgErr = "";
                        k = 0;
                        j++;
                        continue;
                    }           

                    string[] lstCampuri = (campOblig + campNonOblig).Substring(1).Split(',');
                    string camp = "", valoare = "";
                    for (int x = 0; x < lstCampuri.Length; x++)
                    {
                        camp += "," + lstCampuri[x].Split('=')[0];
                        valoare += "," + lstCampuri[x].Split('=')[1];
                    }

                    DataRow[] drAltele = dtCombinat.Select("PozitieFisier IS NULL");
                    if (drAltele != null && drAltele.Count() > 0)
                    {
                        string valAltele = "";
                        for (int z = 0; z < drAltele.Count(); z++)
                        {
                            if (drAltele[z]["ValoareImplicita"] != null && drAltele[z]["ValoareImplicita"].ToString().Contains("ent."))
                            {
                                valAltele = drAltele[z]["ValoareImplicita"].ToString().Replace(",", "#&*");
                                for (int y = 0; y < camp.Substring(1).Split(',').Length; y++)
                                    valAltele = valAltele.Replace("ent." + camp.Substring(1).Split(',')[y].Replace("\"", "").Trim(), valoare.Substring(1).Split(',')[y]);
                            }
                            else
                            {
                                valAltele = (drAltele[z]["ValoareImplicita"] ?? "NULL").ToString();
                                if (drAltele[z]["Tip"].ToString().ToLower().Contains("varchar") && drAltele[z]["ValoareImplicita"] != null)
                                    valAltele = "'" + drAltele[z]["ValoareImplicita"].ToString() + "'";
                            }
                            if (valAltele.Length <= 0)
                                valAltele = "NULL";                        

                            if (drAltele[z]["NumeColoana"].ToString().ToString() == "F10096" && valAltele != "NULL")
                                idSablon = Convert.ToInt32(valAltele);
                            if (numeTabela == "F100" && drAltele[z]["NumeColoana"].ToString() == "F10003")
                            {
                                if (Session["ImportDate_Marca"] != null)
                                    valAltele = Session["ImportDate_Marca"].ToString();
                                else
                                {
                                    string dual = "";
                                    if (Constante.tipBD == 2) dual = " FROM DUAL";
                                    DataTable dtMarca = General.IncarcaDT("SELECT " + valAltele + dual, null);
                                    valAltele = dtMarca.Rows[0][0].ToString();
                                }
                                marcaInit = valAltele;
                                Session["ImportDate_Marca"] = marcaInit;
                            }
                            if ((numeTabela == "Avs_Cereri" || numeTabela == "Ptj_Cereri") && drAltele[z]["NumeColoana"].ToString() == "Id")
                            {
                                string dual = "";
                                if (Constante.tipBD == 2) dual = " FROM DUAL";
                                DataTable dtId = General.IncarcaDT("SELECT " + valAltele + dual, null);
                                valAltele = dtId.Rows[0][0].ToString();
                                idCerere = valAltele;
                            }
                            campNonOblig += ", \"" + drAltele[z]["NumeColoana"].ToString() + "\" = " + valAltele;
                            if (drAltele[z]["OmiteLaActualizare"] == null || drAltele[z]["OmiteLaActualizare"].ToString().Length <= 0 || Convert.ToInt32(drAltele[z]["OmiteLaActualizare"].ToString()) == 0)
                            {
                                if (drAltele[z]["Tabela"] != null && drAltele[z]["Tabela"].ToString().Length > 0 && drAltele[z]["Tabela"].ToString() == "F1001")
                                    campNonObligAct2 += ", \"" + drAltele[z]["NumeColoana"].ToString() + "\" = " + valAltele;
                                else
                                    campNonObligAct += ", \"" + drAltele[z]["NumeColoana"].ToString() + "\" = " + valAltele;
                            }                                
                        }
                    }

                    //campNonOblig = campNonOblig.Replace("#&*", ",");

                    lstCampuri = (campOblig + campNonOblig).Substring(1).Split(',');
                    camp = ""; valoare = "";
                    for (int x = 0; x < lstCampuri.Length; x++)
                    {
                        camp += "," + lstCampuri[x].Split('=')[0];
                        valoare += "," + lstCampuri[x].Split('=')[1].Replace("#&*", ",");
                    }

                    if (utilizator)
                    {
                        if (!campNonObligAct.Contains("\"USER_NO\""))
                            campNonObligAct += ", USER_NO = " + Session["UserId"].ToString();
                        if (!campNonObligAct2.Contains("\"USER_NO\""))
                            campNonObligAct2 += ", USER_NO = " + Session["UserId"].ToString();
                        if (!camp.Contains("\"USER_NO\""))
                        {
                            camp += ", USER_NO ";
                            valoare += ", " + Session["UserId"].ToString();
                        }
                    }
                    if (timp)
                    {
                        if (!campNonObligAct.Contains("\"TIME\""))
                            campNonObligAct += ", TIME = " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                        if (!campNonObligAct2.Contains("\"TIME\""))
                            campNonObligAct2 += ", TIME = " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                        if (!camp.Contains("\"TIME\""))
                        {
                            camp += ", TIME ";
                            valoare += ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                        }
                    }

                    campNonObligAct = campNonObligAct.Replace("GLOBAL.IDUSER", Session["UserId"].ToString());
                    campNonObligAct2 = campNonObligAct2.Replace("GLOBAL.IDUSER", Session["UserId"].ToString());
                    valoare = valoare.Replace("GLOBAL.IDUSER", Session["UserId"].ToString());

                    //if (numeTabela == "F100")
                    //    if (campOblig.Length <= 0 || !campOblig.Contains("F10003"))
                    //        campOblig += ", F10003 = " + marcaInit;

                    DataTable dtTest = General.IncarcaDT("SELECT COUNT(*) FROM \"" + numeTabela + "\" WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ","), null);
                    sql = "";
                    string sql1001 = "";
                    if (dtTest != null && dtTest.Rows.Count > 0 && dtTest.Rows[0][0] != null && Convert.ToInt32(dtTest.Rows[0][0].ToString()) > 0)
                    {
                        sql = "UPDATE \"" + numeTabela + "\" SET " + campNonObligAct.Substring(1).Replace("#&*", ",") + " WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");                      
                        if (numeTabela == "F100" && campNonObligAct2.Length > 0)
                            sql1001 = "UPDATE F1001 SET " + campNonObligAct2.Substring(1).Replace("#&*", ",") + " WHERE F10003 IN (SELECT F10003 FROM F100 WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",") + ")";
                        dtViz.Rows[j - 1]["Actiune"] = "UPDATE";                       
                    }
                    else
                    {
                        if (numeTabela == "F100" && idSablon > 0)
                        {
                            Personal.DateAngajat pag = new Personal.DateAngajat();
                            DataSet ds = new DataSet();
                            Session["IdSablon"] = idSablon;
                            Session["Marca"] = marcaInit;
                            pag.Initializare(ref ds);
                            Session["InformatiaCurentaPersonal"] = null;
                            Session["IdSablon"] = null;

                            General.SalveazaDate(ds.Tables[1], "F100");
                            General.SalveazaDate(ds.Tables[2], "F1001");

                            sql = "UPDATE \"" + numeTabela + "\" SET " + campOblig.Substring(1).Replace("#&*", ",") + ", " + campNonObligAct.Substring(1).Replace("#&*", ",") + " WHERE F10003 = " + marcaInit;
                            if (numeTabela == "F100" && campNonObligAct2.Length > 0)
                                sql1001 = "UPDATE F1001 SET " + campNonObligAct2.Substring(1).Replace("#&*", ",") + " WHERE F10003 = " + marcaInit;
                        }
                        else
                        {
                            sql = "INSERT INTO \"" + numeTabela + "\" (" + camp.Substring(1) + ")  VALUES (" + valoare.Substring(1) + ")";                    
                        }
                        dtViz.Rows[j - 1]["Actiune"] = "INSERT";
                    }
                    bool result = true, result1 = true;
                    result = General.ExecutaNonQuery(sql, null);
                    if (sql1001.Length > 0)
                        result1 = General.ExecutaNonQuery(sql1001, null);

                    if (!result || !result1)
                    {
                        dtViz.Rows[j - 1]["Actiune"] = "";
                        dtViz.Rows[j - 1]["MesajEroare"] = "Eroare";
                    }
                    k = 0;
                    j++;

                    Session["ImportDate_Marca"] = null;

                    if (!result)
                        continue;

                    if (numeTabela == "F100")
                    {
                        int an = DateTime.Now.Year;
                        DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + marcaInit, null);
                        if (dtAng != null && dtAng.Rows.Count > 0 && dtAng.Rows[0]["F10022"] != DBNull.Value) an = Convert.ToDateTime(dtAng.Rows[0]["F10022"]).Year;
                        General.CalculCO(an, Convert.ToInt32(marcaInit));
                    }

                    switch (numeTabela)
                    {
                        case "Avs_Cereri":
                            //circuit
                            string idAtribut = "";
                            int idCircuit = -99;
                            DataTable dtCir = new DataTable();
                            for (int x = 0; x < lstCampuri.Length; x++)
                            {
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "IdAtribut")
                                    idAtribut = lstCampuri[x].Split('=')[1].Trim();
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "IdCircuit")
                                    idCircuit = Convert.ToInt32(lstCampuri[x].Split('=')[1].Trim());
                            }
                            sql = "SELECT COUNT(*) FROM \"Avs_Circuit\" WHERE \"IdAtribut\" = " + idAtribut;                            
                            DataTable dtAtr = General.IncarcaDT(sql, null);
                            int nr = Convert.ToInt32(dtAtr.Rows[0][0].ToString());
                            if (nr > 0)
                            {
                                //prima data determinam circuitul in functie de userul logat;
                                //in cazul in care nu exista, luam primul circuit pe care il gasim

                                if (idCircuit == -99)
                                {
                                    string cond1 = "", cond2 = "";
                                    if (Constante.tipBD == 1)
                                    {
                                        cond1 = " TOP 1 ";
                                        cond2 = "";
                                    }
                                    else
                                    {
                                        cond1 = "";
                                        cond2 = " AND ROWNUM = 1 ";
                                    }

                                    sql = "SELECT " + cond1 + " c.*, b.\"IdUser\" FROM \"F100Supervizori\" b, \"Avs_Circuit\" c WHERE b.F10003 = " + marcaInit + " AND c.\"IdAtribut\" = " + idAtribut
                                        + " AND (c.\"Super1\" = 0 OR c.\"Super1\" = -1 * b.\"IdSuper\" OR c.\"Super2\" = 0 OR c.\"Super2\" = -1 * b.\"IdSuper\" OR c.\"Super3\" = 0 OR c.\"Super3\" = -1 * b.\"IdSuper\" OR c.\"Super4\" = 0 OR c.\"Super4\" = -1 * b.\"IdSuper\" OR c.\"Super5\" = 0 OR c.\"Super5\" = -1 * b.\"IdSuper\" OR c.\"Super6\" = 0 OR c.\"Super6\" = -1 * b.\"IdSuper\" OR c.\"Super7\" = 0 OR c.\"Super7\" = -1 * b.\"IdSuper\" OR c.\"Super8\" = 0 OR c.\"Super8\" = -1 * b.\"IdSuper\" OR c.\"Super9\" = 0 OR c.\"Super9\" = -1 * b.\"IdSuper\" OR c.\"Super10\" = 0 OR c.\"Super10\" = -1 * b.\"IdSuper\" OR c.\"Super11\" = 0 OR c.\"Super11\" = -1 * b.\"IdSuper\" OR c.\"Super12\" = 0 OR c.\"Super12\" = -1 * b.\"IdSuper\" OR c.\"Super13\" = 0 OR c.\"Super13\" = -1 * b.\"IdSuper\" OR c.\"Super14\" = 0 OR c.\"Super14\" = -1 * b.\"IdSuper\" OR c.\"Super15\" = 0 OR c.\"Super15\" = -1 * b.\"IdSuper\" OR c.\"Super16\" = 0 OR c.\"Super16\" = -1 * b.\"IdSuper\" OR c.\"Super17\" = 0 OR c.\"Super17\" = -1 * b.\"IdSuper\" OR c.\"Super18\" = 0 OR c.\"Super18\" = -1 * b.\"IdSuper\" OR c.\"Super19\" = 0 OR c.\"Super19\" = -1 * b.\"IdSuper\" OR c.\"Super20\" = 0 OR c.\"Super20\" = -1 * b.\"IdSuper\") "
                                        + " AND b.\"IdUser\" = " + Session["UserId"] + cond2;
                                    dtCir = General.IncarcaDT(sql, null);

                                    if (dtCir != null && dtCir.Rows.Count > 0)
                                        idCircuit = Convert.ToInt32(dtCir.Rows[0]["Id"].ToString());
                                    else
                                    {
                                        sql = "SELECT " + cond1 + " c.*, b.\"IdUser\" FROM \"F100Supervizori\" b, \"Avs_Circuit\" c WHERE b.F10003 = " + marcaInit + " AND c.\"IdAtribut\" = " + idAtribut
                                            + " AND (c.\"Super1\" = 0 OR c.\"Super1\" = -1 * b.\"IdSuper\" OR c.\"Super2\" = 0 OR c.\"Super2\" = -1 * b.\"IdSuper\" OR c.\"Super3\" = 0 OR c.\"Super3\" = -1 * b.\"IdSuper\" OR c.\"Super4\" = 0 OR c.\"Super4\" = -1 * b.\"IdSuper\" OR c.\"Super5\" = 0 OR c.\"Super5\" = -1 * b.\"IdSuper\" OR c.\"Super6\" = 0 OR c.\"Super6\" = -1 * b.\"IdSuper\" OR c.\"Super7\" = 0 OR c.\"Super7\" = -1 * b.\"IdSuper\" OR c.\"Super8\" = 0 OR c.\"Super8\" = -1 * b.\"IdSuper\" OR c.\"Super9\" = 0 OR c.\"Super9\" = -1 * b.\"IdSuper\" OR c.\"Super10\" = 0 OR c.\"Super10\" = -1 * b.\"IdSuper\" OR c.\"Super11\" = 0 OR c.\"Super11\" = -1 * b.\"IdSuper\" OR c.\"Super12\" = 0 OR c.\"Super12\" = -1 * b.\"IdSuper\" OR c.\"Super13\" = 0 OR c.\"Super13\" = -1 * b.\"IdSuper\" OR c.\"Super14\" = 0 OR c.\"Super14\" = -1 * b.\"IdSuper\" OR c.\"Super15\" = 0 OR c.\"Super15\" = -1 * b.\"IdSuper\" OR c.\"Super16\" = 0 OR c.\"Super16\" = -1 * b.\"IdSuper\" OR c.\"Super17\" = 0 OR c.\"Super17\" = -1 * b.\"IdSuper\" OR c.\"Super18\" = 0 OR c.\"Super18\" = -1 * b.\"IdSuper\" OR c.\"Super19\" = 0 OR c.\"Super19\" = -1 * b.\"IdSuper\" OR c.\"Super20\" = 0 OR c.\"Super20\" = -1 * b.\"IdSuper\") "
                                            + cond2;
                                        dtCir = General.IncarcaDT(sql, null);

                                        if (dtCir != null && dtCir.Rows.Count > 0)
                                            idCircuit = Convert.ToInt32(dtCir.Rows[0]["Id"].ToString());
                                    }
                                }
                                else
                                    dtCir = General.IncarcaDT("SELECT  * FROM \"Avs_Circuit\" WHERE \"Id\" = " + idCircuit, null);

                                if (idCircuit == -99 || idCircuit == 0)                                                                   
                                    dtViz.Rows[j - 1]["MesajEroare"] += (dtViz.Rows[j - 1]["MesajEroare"] == null || dtViz.Rows[j - 1]["MesajEroare"].ToString().Length <= 0 ? "" : " / ") + "Angajatul nu are supervizor pe circuit!";
                                else
                                {
                                    int total = 0;
                                    int idSt = 2;
                                    int pozUser = 1;
                                    string strSql = "";
                                    DataTable dtTemp = new DataTable();
                                    //aflam totalul de utilizatori din circuit
                                    for (int i = 1; i <= 20; i++)
                                    {
                                        if (dtCir != null && dtCir.Rows.Count > 0 && dtCir.Rows[0]["Super" + i] != null && dtCir.Rows[0]["Super" + i].ToString().Length > 0)
                                        {
                                            string idSuper = dtCir.Rows[0]["Super" + i].ToString();
                                            if (idSuper != null && Convert.ToInt32(idSuper) != -99)
                                            {
                                                //ne asiguram ca exista user pentru supervizorul din circuit
                                                if (Convert.ToInt32(idSuper) < 0)
                                                {
                                                    int idSpr = Convert.ToInt32(idSuper);
                                                    strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + marcaInit + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                                                    dtTemp = General.IncarcaDT(strSql, null);
                                                    if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                                                    {
                                                        continue;
                                                    }
                                                }
                                                total++;
                                            }
                                        }
                                    }

                                    sql = "SELECT \"Id\" FROM  \"Avs_Cereri\" WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");
                                    dtTemp = General.IncarcaDT(sql, null);

                                    int idUrm = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
                                    //int idUrm = Convert.ToInt32(idCerere);         

                                    //adaugam istoricul
                                    int poz = 0;
                                    int idUserCalc = -99;

                                    General.ExecutaNonQuery("DELETE FROM Avs_CereriIstoric WHERE Id = " + idUrm, null);

                                    for (int i = 1; i <= 20; i++)
                                    {
                                        if (dtCir != null && dtCir.Rows.Count > 0 && dtCir.Rows[0]["Super" + i] != null && dtCir.Rows[0]["Super" + i].ToString().Length > 0)
                                        {
                                            string valId = dtCir.Rows[0]["Super" + i].ToString();
                                            if (valId != null && Convert.ToInt32(valId) != -99)
                                            {
                                                //poz++;
                                                int usr = Convert.ToInt32(valId);

                                                //IdUser
                                                if (Convert.ToInt32(valId) == 0)
                                                {
                                                    //idUserCalc = idUser;
                                                    strSql = "SELECT * FROM USERS WHERE F10003 = " + marcaInit;
                                                    dtTemp = General.IncarcaDT(strSql, null);
                                                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["F70102"] != null)
                                                    {
                                                        idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["F70102"].ToString());
                                                    }
                                                }
                                                if (Convert.ToInt32(valId) > 0) idUserCalc = Convert.ToInt32(valId);
                                                if (Convert.ToInt32(valId) < 0)
                                                {
                                                    int idSpr = Convert.ToInt32(valId);
                                                    //verif. daca nu cumva user-ul logat este deja un superviozr pt acest angajat;
                                                    //astfel se rezolva problema cand, de exemplu, un angajat are mai multi AdminRu
                                                    strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + marcaInit + " AND \"IdSuper\" = " + (-1 * idSpr).ToString() + " AND \"IdUser\" = " + Session["UserId"].ToString();
                                                    dtTemp = General.IncarcaDT(strSql, null);
                                                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0]["IdUser"] != null && dtTemp.Rows[0]["IdUser"].ToString().Length > 0)
                                                    {
                                                        idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["IdUser"].ToString());
                                                    }
                                                    else
                                                    {
                                                        //ne asiguram ca exista user pentru supervizorul din circuit
                                                        strSql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = " + marcaInit + " AND \"IdSuper\" = " + (-1 * idSpr).ToString();
                                                        dtTemp = General.IncarcaDT(strSql, null);
                                                        if (dtTemp == null || dtTemp.Rows.Count == 0 || dtTemp.Rows[0]["IdUser"] == null || dtTemp.Rows[0]["IdUser"].ToString().Length <= 0)
                                                        {
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            idUserCalc = Convert.ToInt32(dtTemp.Rows[0]["IdUser"].ToString());
                                                        }
                                                    }
                                                }

                                                poz += 1;


                                                //starea
                                                string camp3 = ", \"IdStare\", \"Culoare\"";
                                                string camp4 = ",null, null";
                                                if (idUserCalc == Convert.ToInt32(Session["UserId"].ToString()))
                                                {
                                                    pozUser = poz;
                                                    if (poz == 1) idSt = 1;
                                                    if (poz == total) idSt = 3;
                                                    camp3 = ", \"Aprobat\", \"DataAprobare\", \"IdStare\", \"Culoare\"";
                                                    camp4 = ", 1, " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", " + idSt + ", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idSt.ToString() + ")";
                                                }        

                                                strSql = "INSERT INTO \"Avs_CereriIstoric\" (\"Id\", \"IdCircuit\", \"IdSuper\", \"Pozitie\", USER_NO, TIME, \"Inlocuitor\", \"IdUser\" {0}) "
                                                    + "VALUES (" + idUrm + ", " + idCircuit + ", " + Convert.ToInt32(valId) + ", " + poz + ", " + Session["UserId"].ToString() + ", "
                                                    + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", 0, " + idUserCalc + " {1})";
                                                strSql = string.Format(strSql, camp3, camp4);
                                                General.ExecutaNonQuery(strSql, null);
                                            }
                                        }
                                    }

                                    sql = "UPDATE \"Avs_Cereri\" SET \"IdStare\" =  " + idSt.ToString() + ", \"Culoare\" = (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idSt.ToString() + "), \"TotalCircuit\" = " + total.ToString() + ", \"Pozitie\" = " + pozUser.ToString() + " WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");
                                    General.ExecutaNonQuery(sql, null);
                                }                                
                            }
                            else
                                dtViz.Rows[j - 1]["MesajEroare"] += (dtViz.Rows[j - 1]["MesajEroare"] == null || dtViz.Rows[j - 1]["MesajEroare"].ToString().Length <= 0 ? "" : " / ") + "Atributul nu are circuit alocat!";
                            break;
                        case "Ptj_Cumulat":
                            string filtru = "";
                            for (int x = 0; x < lstCampuri.Length; x++)
                            {
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "F10003" || lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "An" || lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "Luna")
                                    filtru += " AND " + lstCampuri[x].Replace("#&*", ",");
                                filtru = filtru.Substring(5);
                            }
                            General.CalculFormuleCumulat(filtru);
                            break;
                        case "Ptj_Intrari":
                        case "Ptj_Cereri":
                            string idStare = "", marca = "", ziua = "", dataInceput = "", dataSfarsit = "";
                            for (int x = 0; x < lstCampuri.Length; x++)
                            {
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "IdStare")
                                    idStare = lstCampuri[x].Split('=')[1].Trim();
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "F10003")
                                    marca = lstCampuri[x].Split('=')[1].Trim();
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "Ziua")
                                    ziua = lstCampuri[x].Split('=')[1].Replace("#&*", ",").Trim();
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "DataInceput")
                                    dataInceput = lstCampuri[x].Split('=')[1].Replace("#&*", ",").Trim();
                                if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "DataSfarsit")
                                    dataSfarsit = lstCampuri[x].Split('=')[1].Replace("#&*", ",").Trim();
                            }

                            filtru = "";
                            if (ziua.Length > 0)
                                filtru = " AND \"Ziua\" = " + ziua;
                            if (dataInceput.Length > 0)
                                filtru = " AND \"Ziua\" >= " + dataInceput;
                            if (dataSfarsit.Length > 0)
                                filtru += " AND \"Ziua\" <= " + dataSfarsit;

                            if (numeTabela == "Ptj_Intrari")
                                idStare = "3";

                            if (idStare == "3")
                            {
                                DataTable dtModif = General.IncarcaDT("SELECT * FROM \"Ptj_Intrari\" WHERE F10003 = " + marca + filtru, null);

                                if (dtModif != null && dtModif.Rows.Count > 0)
                                {
                                    for (int x = 0; x < dtModif.Rows.Count; x++)
                                    {
                                        Calcul.AlocaContract(Convert.ToInt32(dtModif.Rows[x]["F10003"]), Convert.ToDateTime(dtModif.Rows[x]["Ziua"]));
                                        Calcul.CalculInOut(dtModif.Rows[x], true, true);

                                        General.CalculFormule(dtModif.Rows[x]["F10003"], null, Convert.ToDateTime(dtModif.Rows[x]["Ziua"]), null);
                                        General.ExecValStr(Convert.ToInt32(dtModif.Rows[x]["F10003"]), Convert.ToDateTime(dtModif.Rows[x]["Ziua"]));
                                    }
                                }
                            }
                            if (numeTabela == "Ptj_Cereri")
                            {//circuit
                                string sqlIst;
                                int trimiteLaInlocuitor;
                                int inloc = -1, idCir = -1;
                                DateTime dataInc = DateTime.Now.Date;
                                for (int x = 0; x < lstCampuri.Length; x++)
                                {
                                    if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "DataInceput")
                                    {
                                        string data = lstCampuri[x].Split('=')[1].Trim();
                                        if (Constante.tipBD == 1)
                                            data = data.Replace("CONVERT(DATETIME#&* '", "").Replace("'#&* 103)", "");
                                        else
                                            data = data.Replace("TO_DATE('", "").Replace("'#&* 'dd/mm/yyyy HH24:mi:ss')", "");

                                        dataInc = Convert.ToDateTime(data);
                                    }
                                    if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "Inlocuitor" && lstCampuri[x].Split('=')[1].Trim() != "NULL")
                                        inloc = Convert.ToInt32(lstCampuri[x].Split('=')[1].Trim());
                                    if (lstCampuri[x].Split('=')[0].Replace("\"", "").Trim() == "IdCircuit" && lstCampuri[x].Split('=')[1].Trim() != "NULL")
                                        idCir = Convert.ToInt32(lstCampuri[x].Split('=')[1].Trim());
                                }
                                //string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) FROM ""Ptj_Cereri"") ";
                                //DataTable dtId = General.IncarcaDT(sqlIdCerere, null);

                                int estePlanif = 0, idCircuitAbs = -1;                                

                                DataTable dtAbs = General.IncarcaDT(General.SelectAbsente(marcaInit, dataInc), null);

                                if (dtAbs != null && dtAbs.Rows.Count > 0)
                                {
                                    estePlanif = Convert.ToInt32(General.Nz(dtAbs.Rows[0]["EstePlanificare"], 0));
                                    idCircuitAbs = Convert.ToInt32(dtAbs.Rows[0]["IdCircuit"].ToString());
                                }

                                if (idCir < 0 && idCircuitAbs < 0)
                                {
                                    General.ExecutaNonQuery("DELETE FROM \"Ptj_CereriIstoric\" WHERE \"IdCerere\" = " + idCerere, null);
                                    General.ExecutaNonQuery("DELETE FROM \"Ptj_Cereri\" WHERE \"Id\" = " + idCerere, null);
                                    dtViz.Rows[j - 2]["Actiune"] = "";
                                    dtViz.Rows[j - 2]["MesajEroare"] = "Nu a fost gasit circuit!";
                                    continue;
                                }


                                sql = "SELECT \"Id\" FROM  \"Ptj_Cereri\" WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");
                                DataTable dtTemp = General.IncarcaDT(sql, null);

                                int idUrm = Convert.ToInt32(dtTemp.Rows[0][0].ToString());
                                General.ExecutaNonQuery("DELETE FROM \"Ptj_CereriIstoric\" WHERE \"IdCerere\" = " + idUrm, null);

                                General.SelectCereriIstoric(Convert.ToInt32(marcaInit), inloc, idCir < 0 ? idCircuitAbs : idCir, estePlanif, out sqlIst, out trimiteLaInlocuitor, Convert.ToInt32(idCerere), dataInc);
                                General.ExecutaNonQuery(sqlIst, null);

                                string strTop = "";
                                if (Constante.tipBD == 1) strTop = "TOP 1";   
                                string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + idCerere + ")";
                                string sqlIdStare = $@"(SELECT {strTop} ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={idCerere} ORDER BY ""Pozitie"" DESC)";
                                string sqlPozitie = $@"(SELECT {strTop} ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={idCerere} ORDER BY ""Pozitie"" DESC)";
                                string sqlCuloare = $@"(SELECT {strTop} ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={idCerere} ORDER BY ""Pozitie"" DESC)";

                                if (Constante.tipBD == 2)
                                {
                                    sqlIdStare = $@"(SELECT * FROM ({sqlIdStare}) WHERE ROWNUM=1)";
                                    sqlPozitie = $@"(SELECT * FROM ({sqlPozitie}) WHERE ROWNUM=1)";
                                    sqlCuloare = $@"(SELECT * FROM ({sqlCuloare}) WHERE ROWNUM=1)";
                                }

                                DataTable dtVerif = General.IncarcaDT(sqlPozitie.Substring(1, sqlPozitie.Length - 2), null);
                                if (dtVerif == null || dtVerif.Rows.Count <= 0 || dtVerif.Rows[0][0] == null || Convert.ToInt32(dtVerif.Rows[0][0].ToString()) < 1)
                                {
                                    General.ExecutaNonQuery("DELETE FROM \"Ptj_CereriIstoric\" WHERE \"IdCerere\" = " + idCerere, null);
                                    General.ExecutaNonQuery("DELETE FROM \"Ptj_Cereri\" WHERE \"Id\" = " + idCerere, null);
                                    dtViz.Rows[j - 2]["Actiune"] = "";
                                    dtViz.Rows[j - 2]["MesajEroare"] = "Circuitul nu este valid!";
                                }
                                else
                                {
                                    sql = "UPDATE \"Ptj_Cereri\" SET \"IdStare\" =  " + sqlIdStare + ", \"Culoare\" = " + sqlCuloare + ", \"TotalSuperCircuit\" = " + sqlTotal + ", \"Pozitie\" = " + sqlPozitie + " WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");
                                    General.ExecutaNonQuery(sql, null);
                                }

                            }
                            break;
                    }
                
                }

                grDateViz.DataSource = dtViz;
                grDateViz.DataBind();

                MessageBox.Show("Import terminat cu succes!", MessageBox.icoSuccess);

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }

                //scriere in log
                StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~/Temp/") + "ImportDateLog.csv", false);   
                for (int i = 0; i < dtViz.Columns.Count; i++)                
                    sw.Write(dtViz.Columns[i].ColumnName + "\t");                
                sw.Write("\r\n");
                for (int p = 0; p < dtViz.Rows.Count; p++)
                {
                    for (int q = 0; q < dtViz.Columns.Count; q++)
                        sw.Write(dtViz.Rows[p][q] + "\t");
                    sw.Write("\r\n");
                } 
                sw.Close();
                sw.Dispose();

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        
        protected void cmbSablon_Callback(object sender, CallbackEventArgsBase e)
        {      
            //cmbTabela.DataSource = null;
            //cmbTabela.DataBind();
            //string id = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) ";
            //if (Constante.tipBD == 2)
            //    id = "ROWNUM";
            //SqlDataSource ds = new SqlDataSource();
            //ds.EnableCaching = false;
            //ds.ConnectionString = Constante.cnnWeb;
            //ds.SelectCommand = "SELECT " + id + "\"Id\", a.* from (select distinct \"NumeTabela\" as \"Denumire\" FROM \"ImportDateNomen\") a ORDER BY \"Denumire\"";
            //cmbTabela.DataSource = ds;
            //cmbTabela.DataBind();
                        
        }

        //protected void btnViz_Click(object sender, EventArgs e)

        protected void btnViz_Click()
        {
            try
            {     
                int id = -99;
                if (cmbSablon.Value != null)
                    id = Convert.ToInt32(cmbSablon.Value);
                else
                {
                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "NumeSablon", "NumeTabela" }) as object[];
                    if (obj != null && obj.Count() > 0)
                        id = Convert.ToInt32(obj[0]);
                }
                if (id < 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista sablon selectat");
                    return;
                }

                DataTable dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = " + id, null);

    

                grDateNomen.KeyFieldName = "Id;IdAuto";
                grDateNomen.DataSource = dt;
                grDateNomen.DataBind();
                Session["ImportDateNomen_Grid"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

   
    }
}