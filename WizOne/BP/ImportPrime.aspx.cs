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

namespace WizOne.BP
{
    public partial class ImportPrime : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnImport.Text = Dami.TraduCuvant("btnImport", "Import");
                btnExport.Text = Dami.TraduCuvant("btnExport", "Exporta document");
                btnDocUpload.ToolTip = Dami.TraduCuvant("Incarca document");

                lgImport.InnerText = Dami.TraduCuvant("Import prime");
                lblAn.Text = Dami.TraduCuvant("An");
                lblLuna.Text = Dami.TraduCuvant("Luna");
                lblPrima.Text = Dami.TraduCuvant("Selectati tipul primei");
                lblAvs.Text = Dami.TraduCuvant("Avans/Lichidare");        

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }
                #endregion

                if (IsPostBack)
                {
                    DataTable dt = new DataTable();
                    if (Session["ImportPrime_Grid"] == null)
                    {
                        dt.Columns.Add("Marca", typeof(int));
                        dt.Columns.Add("Nume", typeof(string));
                        dt.Columns.Add("Suma", typeof(int));
                        dt.Columns.Add("Explicatie", typeof(string));
                        dt.Columns.Add("IdAuto", typeof(int));
                    }
                    else
                    {
                        dt = Session["ImportPrime_Grid"] as DataTable;
                    }
                    grDate.DataSource = dt;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
                    Session["ImportPrime_Grid"] = dt;          

                }
                else
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Marca", typeof(int));
                    dt.Columns.Add("Nume", typeof(string));
                    dt.Columns.Add("Suma", typeof(int));
                    dt.Columns.Add("Explicatie", typeof(string));
                    dt.Columns.Add("IdAuto", typeof(int));   
                    
                    grDate.DataSource = dt;                 
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataBind();
                    Session["ImportPrime_Grid"] = dt;

                    DataTable dtAng = General.IncarcaDT("SELECT * FROM F100", null);
                    Session["ImportPrime_Ang"] = dtAng;

                    cmbAn.DataSource = General.ListaNumere(2015, DateTime.Now.Year + 5);
                    cmbAn.DataBind();
                    cmbLuna.DataSource = General.ListaLuniDesc();
                    cmbLuna.DataBind();

                    DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                    try
                    {
                        cmbLuna.Value = Convert.ToInt32(dt010.Rows[0][1].ToString());
                        cmbAn.Value = Convert.ToInt32(dt010.Rows[0][0].ToString());
                    }
                    catch (Exception) { }
                }

                cmbAvs.DataSource = General.ListaAvansLich();
                cmbAvs.DataBind();


                cmbPrima.DataSource = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"BP_tblPrime\"", null);
                cmbPrima.DataBind();





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
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/BP_Import"));
                if (!folder.Exists)
                    folder.Create();

                foreach (FileInfo file in folder.GetFiles())
                {
                    file.Delete();
                }

                string path = folder.FullName + "\\ImportPrime." + btnDocUpload.UploadedFiles[0].FileName.Split('.')[1];
                File.WriteAllBytes(path, btnDocUpload.UploadedFiles[0].FileBytes);          


                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\ImportPrime." + btnDocUpload.UploadedFiles[0].FileName.Split('.')[1], 
                    ("\\ImportPrime." + btnDocUpload.UploadedFiles[0].FileName.Split('.')[1].ToUpper() == "XLS" ? DevExpress.Spreadsheet.DocumentFormat.Xls : DevExpress.Spreadsheet.DocumentFormat.Xlsx));

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets["Sheet1"];

                DataTable table = new DataTable();
                table.Columns.Add("Marca", typeof(int));
                table.Columns.Add("Nume", typeof(string));
                table.Columns.Add("Suma", typeof(int));
                table.Columns.Add("Explicatie", typeof(string));
                table.Columns.Add("IdAuto", typeof(int));

                DataTable dtAng =  Session["ImportPrime_Ang"] as DataTable;

                int i = 1;
                while (!ws2.Cells[i, 0].Value.IsEmpty)
                {
                    if (ws2.Cells[i, 2].Value != null && ws2.Cells[i, 2].Value.ToString().Length > 0)
                    {
                        DataRow[] dr = dtAng.Select("F10003=" + ws2.Cells[i, 0].Value);
                        if (dr != null && dr.Count() > 0)
                        {
                            DataTable dt = General.IncarcaDT("SELECT A.* FROM \"F100Supervizori\" A JOIN F100 B ON A.F10003 = B.F10003 WHERE A.F10003 = " + ws2.Cells[i, 0].Value + " AND \"IdUser\" = " + Session["UserId"].ToString() + " AND \"IdSuper\" <> 0 AND F10025 IN (0, 999) ORDER BY \"NumeComplet\" ", null);
                            if (dt != null && dt.Rows.Count > 0)
                                table.Rows.Add(Convert.ToInt32(ws2.Cells[i, 0].Value.ToString()), dr.ElementAt(0)["F10008"].ToString() + " " + dr.ElementAt(0)["F10009"].ToString(), ws2.Cells[i, 2].Value.ToString(), ws2.Cells[i, 3].Value.ToString(), i);
                        }
                    }
                    i++;
                }

                grDate.DataSource = table;
                grDate.KeyFieldName = "IdAuto";
                grDate.DataBind();
                Session["ImportPrime_Grid"] = table;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                DataTable dt = Session["ImportPrime_Grid"] as DataTable;
                grDate.DataSource = dt;
                grDate.DataBind();
                grDate.KeyFieldName = "IdAuto";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {   
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/BP_Import"));

                if (folder.GetFiles().Count() <= 0)
                {
                    MessageBox.Show("Nu ati incarcat niciun fisier!", MessageBox.icoError, "");
                    return;
                }

                if (cmbPrima.Value == null)
                {
                    MessageBox.Show("Nu ati selectat niciun tip de prima!", MessageBox.icoError, "");
                    return;
                }

                if (cmbAn.Value == null || cmbLuna.Value == null)
                {
                    MessageBox.Show("Nu ati selectat luna/anul!", MessageBox.icoError, "");
                    return;
                }

                DataTable dt010 = General.IncarcaDT($@"SELECT F01011, F01012 FROM F010 ", null);

                if (Convert.ToInt32(cmbAn.Value) < Convert.ToInt32(dt010.Rows[0][0].ToString())
                    || (Convert.ToInt32(cmbAn.Value) == Convert.ToInt32(dt010.Rows[0][0].ToString()) && Convert.ToInt32(cmbLuna.Value) < Convert.ToInt32(dt010.Rows[0][1].ToString())))
                {
                    MessageBox.Show("Nu se pot introduce bonusuri pentru o luna anterioara lunii de salarizare curente!", MessageBox.icoError, "");
                    return;
                }

                DevExpress.Spreadsheet.Workbook workbook = new DevExpress.Spreadsheet.Workbook();
                workbook.LoadDocument(folder.FullName + "\\ImportPrime.xlsx", DevExpress.Spreadsheet.DocumentFormat.Xlsx);

                DevExpress.Spreadsheet.Worksheet ws2 = workbook.Worksheets["Sheet1"];           

                for (int i = 0; i < grDate.VisibleRowCount; i++)
                {
                    DataRowView obj = grDate.GetRow(i) as DataRowView;
                    if (obj["Nume"] != null && obj["Nume"].ToString().Length > 0 && obj["Suma"] != null && obj["Suma"].ToString().Length > 0)
                    {
                        //string sql = "INSERT INTO \"BP_Prime\" (F10003, \"SumaNeta\", \"TotalNet\", \"IdTip\", \"Recurenta\", \"IdMoneda\", \"Curs\", \"An\", \"Luna\", \"UserIntrod\", USER_NO, TIME, \"Culoare\")";
                        //sql += " VALUES ({0}, {1}, {1}, {2}, 0, 0, 1, {3}, {4}, {5}, {5}, {6}, '#FFFFA500')";

                        //sql = string.Format(sql, obj["Marca"].ToString(), obj["Suma"].ToString().ToString(new CultureInfo("en-US")), cmbPrima.Value.ToString(), DateTime.Now.Year, DateTime.Now.Month, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));

                        //General.ExecutaNonQuery(sql, null);

                        string msg = General.AdaugaCerere(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(obj["Marca"].ToString()), Convert.ToInt32(cmbAn.Value), Convert.ToInt32(cmbLuna.Value), Convert.ToInt32(cmbPrima.Value ?? -99),
                            Convert.ToDecimal(obj["Suma"].ToString()), 0, 1, Convert.ToDecimal(obj["Suma"].ToString()), Convert.ToInt32(cmbAvs.Value ?? 2), (obj["Explicatie"] ?? "").ToString());

                        if (msg != "")
                        {
                            MessageBox.Show(msg, MessageBox.icoError);
                            return;
                        }

                    }
                }

                MessageBox.Show("Import terminat cu succes!", MessageBox.icoSuccess);

                DataTable dt = new DataTable();

                dt.Columns.Add("Marca", typeof(int));
                dt.Columns.Add("Nume", typeof(string));
                dt.Columns.Add("Suma", typeof(int));
                dt.Columns.Add("Explicatie", typeof(string));
                dt.Columns.Add("IdAuto", typeof(int));

                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                grDate.DataBind();
                Session["ImportPrime_Grid"] = dt;

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

        protected void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";
                DataTable dtAng = General.IncarcaDT("SELECT A.F10003, A.F10008 " + op + " ' ' " + op + " A.F10009 AS NUME FROM F100 A JOIN \"F100Supervizori\" B ON A.F10003 = B.F10003 AND B.\"IdUser\" = " 
                    + Session["UserId"].ToString() + " AND \"IdSuper\" <> 0 AND F10025 IN (0, 999) GROUP BY A.F10003, A.F10008 " + op + " ' ' " + op + " A.F10009 ORDER BY NUME ", null);
                DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
                DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];


                ws2.Cells[0, 0].Value = "Marca";
                ws2.Cells[0, 1].Value = "Nume si prenume";
                ws2.Cells[0, 2].Value = "Suma";
                ws2.Cells[0, 3].Value = "Explicatie";

                for (int i = 0; i < dtAng.Rows.Count; i++)
                {
                    ws2.Cells[i + 1, 0].Value = dtAng.Rows[i]["F10003"].ToString();
                    ws2.Cells[i + 1, 1].Value = dtAng.Rows[i]["NUME"].ToString();
                }

                ws2.Columns.AutoFit(0, 5000);

                byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xlsx);

                DateTime ora = DateTime.Now;
                string numeXLS = "ImportPrime_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xlsx";


                MemoryStream stream = new MemoryStream(byteArray);
                Response.Clear();
                MemoryStream ms = stream;
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + numeXLS);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}