using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Curs
{
    public partial class CursuriAnterioare : System.Web.UI.Page
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
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn coloana = c as GridViewDataColumn;
                            coloana.Caption = Dami.TraduCuvant(coloana.FieldName ?? coloana.Caption, coloana.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                    Session["CursuriAnt_Grid"] = null;

                IncarcaGrid();


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
                //IncarcaGrid();
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
                DataTable dt = Session["CursuriAnt_Grid"] as DataTable;

                General.SalveazaDate(dt, "Curs_Anterior");

                MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

     


        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
 

                grDate.KeyFieldName = "IdAuto";
                if (Session["CursuriAnt_Grid"] == null)
                {
                    dt = General.IncarcaDT("SELECT * FROM \"Curs_Anterior\" WHERE F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99), null);
                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["CursuriAnt_Grid"] = dt;
                }
                else
                {
                    dt = Session["CursuriAnt_Grid"] as DataTable;
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
     
                
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

      

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["CursuriAnt_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["CursNume"] == null || e.NewValues["CursNume"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                if (Session["DocUpload_CursuriAnt"] != null)
                {
                    metaUploadFile doc = Session["DocUpload_CursuriAnt"] as metaUploadFile;
                    if (doc != null)
                    {
                        General.LoadFile(doc.UploadedFileName.ToString(), doc.UploadedFile, "Curs_Anterior", Convert.ToInt32(e.NewValues["IdAuto"].ToString()));
                        Session["DocUpload_CursuriAnt"] = null;
                    }
                }

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["CursuriAnt_Grid"] = dt;
                grDate.DataSource = dt;
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
                DataTable dt = Session["CursuriAnt_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                e.NewValues["F10003"] = Convert.ToInt32(Session["User_Marca"] ?? -99);
                e.NewValues["IdStare"] = 1;
                e.NewValues["areFisier"] = 0;

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
                    e.NewValues["IdAuto"] = Dami.NextId("Curs_Anterior");

                if (e.NewValues["CursNume"] == null || e.NewValues["CursNume"].ToString().Length <= 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Lipseste denumirea!";
                    e.Cancel = true;
                    grDate.CancelEdit();
                    return;
                }

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {  
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                metaUploadFile doc = Session["DocUpload_CursuriAnt"] as metaUploadFile;
                if (doc != null)
                {
                    General.LoadFile(doc.UploadedFileName.ToString(), doc.UploadedFile, "Curs_Anterior", Convert.ToInt32(e.NewValues["IdAuto"].ToString()));
                    Session["DocUpload_CursuriAnt"] = null;
                }
                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["CursuriAnt_Grid"] = dt;
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

                DataTable dt = Session["CursuriAnt_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["CursuriAnt_Grid"] = dt;
                grDate.DataSource = dt;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {

            try
            {
 
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
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_CursuriAnt"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {          

                HtmlTableCell lblObs = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblObs");
                lblObs.InnerText = Dami.TraduCuvant("Observatii");
                HtmlTableCell lblDataInc = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDataInc");
                lblDataInc.InnerText = Dami.TraduCuvant("Data inceput");
                HtmlTableCell lblDataSf = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDataSf");
                lblDataSf.InnerText = Dami.TraduCuvant("Data sfarsit");
                HtmlTableCell lblNumeCurs = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblNumeCurs");
                lblNumeCurs.InnerText = Dami.TraduCuvant("Denumire curs");
                HtmlTableCell lblNumeSesiune = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblNumeSesiune");
                lblNumeSesiune.InnerText = Dami.TraduCuvant("Tip curs");
                HtmlTableCell lblOrg = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblOrg");
                lblOrg.InnerText = Dami.TraduCuvant("Organizator");
                HtmlTableCell lblSpec = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblSpec");
                lblSpec.InnerText = Dami.TraduCuvant("Specializare");

                ASPxUploadControl btnDocUpload = (ASPxUploadControl)grDate.FindEditFormTemplateControl("btnDocUpload");
                btnDocUpload.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                btnDocUpload.ToolTip = Dami.TraduCuvant("Incarca Document");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}