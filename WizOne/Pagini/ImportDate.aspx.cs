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
                Dami.AccesApp();


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

                }             
                SqlDataSource ds = new SqlDataSource();
                ds.EnableCaching = false;
                ds.ConnectionString = Constante.cnnWeb;
                ds.SelectCommand = "SELECT \"Id\",  \"NumeSablon\" as \"Denumire\" FROM \"Template\" ORDER BY \"Denumire\"";
                cmbSablon.DataSource = ds;
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

                string path = folder.FullName + "\\Temp.xlsx";
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

                if (File.Exists(folder.FullName + "\\Temp.xlsx"))
                {
                    DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                    workbook.LoadDocument(folder.FullName + "\\Temp.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

             
                    DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets[0];

                    DataTable table = new DataTable();
                    table.Columns.Add("Id", typeof(string));
                    table.Columns.Add("Denumire", typeof(string));

                    int i = 0;
                    while (!ws2.Cells[0, i].Value.IsEmpty)
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

                    string sql = "SELECT * FROM \"Template\" WHERE \"Id\" =  " + Convert.ToInt32(cmbSablon.Value);
                    DataTable dtSablon = General.IncarcaDT(sql, null);
                    if (dtSablon != null && dtSablon.Rows.Count > 0)
                    {
                        sql = "SELECT * FROM \"" + dtSablon.Rows[0]["NumeTabela"].ToString().Trim() + "\"";
                        DataTable dt = General.IncarcaDT(sql, null);
                        for (int k = 0; k < dt.Columns.Count; k++)
                            table.Rows.Add(dt.Columns[k].ColumnName, dt.Columns[k].ColumnName);

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
                    dt.Columns.Add(obj["ColoanaFisier"].ToString(), typeof(string));
                }

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets[0];
                Dictionary<int, int> lstIndex = new Dictionary<int, int>();

                int k = 0;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    k = 0;
                    while (!ws2.Cells[0, k].Value.IsEmpty)
                    {
                        if (ws2.Cells[0, k].Value.ToString().Trim() == dt.Columns[i].ColumnName)
                        {
                            lstIndex.Add(k, i);
                            break;
                        }
                        k++;
                    }
                }

                Session["ImportDate_NrColoane"] = k;

                string[] sir = new string[dt.Columns.Count];
                for (int i = 0; i < sir.Length; i++)
                    sir[i] = "";

                int j = 1;
                k = 0;
                while (!ws2.Cells[j, k].Value.IsEmpty)
                {
                    if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                    {
                        j++;
                        k = 0;
                        continue;
                    }

                    dt.Rows.Add(sir);
                    while (!ws2.Cells[j, k].Value.IsEmpty)
                    {
                        if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                        {
                            k++;
                            continue;
                        }

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
                                row[x] = Convert.ToInt32(cmbSablon.Value);
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }
                    x++;
                }

                bool err = false;
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


                bool err = false;
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

                Session["ImportDate_Sablon"] = cmbSablon.Value;
                DataTable dt = new DataTable();
                if (cmbSablon.Value != null)
                    dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" = " + Convert.ToInt32(cmbSablon.Value), null);
                else
                    dt = General.IncarcaDT("SELECT * FROM \"TemplateCampuri\" WHERE \"Id\" =  -1", null);

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

        protected void btnImport_Click(object sender, EventArgs e)
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

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\Temp.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

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

                sql = " select ColoanaFisier, ColoanaBD as NumeColoana, Obligatoriu, ValoareImplicita, null as PozitieFisier, "
                      + " (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = ColoanaBD) as Tip "
                      + "  from TemplateCampuri a " 
                      + "  left join Template b on a.id = b.Id where b.NumeTabela = '" + numeTabela + "' ";
                if (Constante.tipBD == 2)
                    sql = " select \"ColoanaFisier\", \"ColoanaBD\" as \"NumeColoana\", \"Obligatoriu\", \"ValoareImplicita\", null as \"PozitieFisier\", "
                      + " (SELECT DATA_TYPE FROM user_tab_columns WHERE  TABLE_NAME = '" + numeTabela + "' and COLUMN_NAME = \"ColoanaBD\") as \"Tip\" "
                      + "  from \"TemplateCampuri\" a "
                      + "  left join \"Template\" b on a.\"Id\" = b.\"Id\" where b.\"NumeTabela\" = '" + numeTabela + "' ";

                DataTable dtCombinat = General.IncarcaDT(sql, null);
                foreach (DataColumn col in dtCombinat.Columns)
                    col.ReadOnly = false;

                for (int i = 0; i < dtCombinat.Rows.Count; i++)
                {
                    k = 0;
                    while (!ws2.Cells[0, k].Value.IsEmpty)
                    {
                        if (ws2.Cells[0, k].Value.ToString() == dtCombinat.Rows[i]["ColoanaFisier"].ToString())
                        {
                            dtCombinat.Rows[i]["PozitieFisier"] = k;
                            break;
                        }
                        k++;
                    }
                }


                int j = 1;
                k = 0;
                string msgErr = "";
                DataTable dtViz = Session["ImportDate_Previz"] as DataTable;
                int nrCol = Convert.ToInt32(Session["ImportDate_NrColoane"].ToString());
                while (!ws2.Cells[j, k].Value.IsEmpty)
                {
                    if (ws2.Cells[j, k].Value.ToString().Length <= 0)
                    {
                        k = 0;
                        j++;
                        continue;
                    }

                    string campOblig = "";
                    string campNonOblig = "";
                    //while (!ws2.Cells[j, k].Value.IsEmpty)
                    for (int x = 0; x < nrCol; x++)
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
                                case "varchar2":
                                    if (ws2.Cells[j, k].Value != null && ws2.Cells[j, k].Value.ToString().Length > 0)
                                        val = "'" + ws2.Cells[j, k].Value.ToString() + "'";
                                    break;
                                case "date":
                                case "datetime":
                                    if (ws2.Cells[j, k].Value != null && ws2.Cells[j, k].Value.ToString().Length > 0)
                                    {
                                        if (Constante.tipBD == 1)
                                            val = "CONVERT(DATETIME#&* '" + ws2.Cells[j, k].Value.ToString() + "'#&* 103)";
                                        else
                                            val = "TO_DATE('" + ws2.Cells[j, k].Value.ToString() + "'#&* dd/mm/yyyy)";
                                    }
                                    break;
                            }


                            val = val.Replace(",", "#&*");

                            if (dr[0]["Obligatoriu"].ToString() == "1")
                            {
                                campOblig += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
                                if (val == "NULL")                                
                                    msgErr = "Lipsa valoare camp " + dr[0]["NumeColoana"].ToString();                                
                            }
                            else
                                campNonOblig += ", \"" + dr[0]["NumeColoana"].ToString() + "\" = " + val;
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
                            campNonOblig += ", \"" + drAltele[z]["NumeColoana"].ToString() + "\" = " + valAltele;
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

                    DataTable dtTest = General.IncarcaDT("SELECT COUNT(*) FROM \"" + numeTabela + "\" WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ","), null);
                    sql = "";
                    if (dtTest != null && dtTest.Rows.Count > 0 && dtTest.Rows[0][0] != null && Convert.ToInt32(dtTest.Rows[0][0].ToString()) > 0)
                    {
                        sql = "UPDATE \"" + numeTabela + "\" SET " + campNonOblig.Substring(1).Replace("#&*", ",") + " WHERE " + campOblig.Substring(1).Replace(",", " AND ").Replace("#&*", ",");                      
                        dtViz.Rows[j - 1]["Actiune"] = "UPDATE";                       
                    }
                    else
                    {
                        sql = "INSERT INTO \"" + numeTabela + "\" (" + camp.Substring(1) + ")  VALUES (" + valoare.Substring(1) + ")";                    
                        dtViz.Rows[j - 1]["Actiune"] = "INSERT";                      
                    }
                    bool result = true;
                    result = General.ExecutaNonQuery(sql, null);
                
                    if (!result)
                    {
                        dtViz.Rows[j - 1]["Actiune"] = "";
                        dtViz.Rows[j - 1]["MesajEroare"] = "Eroare";
                    }
                    k = 0;
                    j++;

                    switch (numeTabela)
                    {
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



        protected void btnViz_Click(object sender, EventArgs e)
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