using DevExpress.Web;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Tactil
{
    public partial class ListaDiverseTactil : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Tactil.ListaDiverseTactil";
                Session["Absente_Cereri_Date"] = null;
        

                DataTable dtStari = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;
       

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

 

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                
                    }
                    catch (Exception) { }
                }

                #endregion

      

                if (!IsPostBack)
                {
                  
                    grDate.DataBind();
                }
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
                IncarcaGrid();
                if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
                
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
                string filtru = "";             

                string sqlFinal = Dami.SelectCereriDiverse(Convert.ToInt32(-99)) + $@" {filtru} ORDER BY A.TIME DESC";
                dt = General.IncarcaDT(sqlFinal, null);
                grDate.KeyFieldName = "Id; Rol";
                grDate.DataSource = dt;
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
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnAtasament":
                            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='MP_Cereri' AND ""Id""={arr[1]} AND ""EsteCerere""=0", null);
                            File.WriteAllBytes(HostingEnvironment.MapPath("~/TEMP/") + dt.Rows[0]["FisierNume"].ToString(), (byte[])dt.Rows[0]["Fisier"]);

                            Session["PrintImage"] = HostingEnvironment.MapPath("~/TEMP/") + dt.Rows[0]["FisierNume"].ToString();
                            Reports.PrintImage dlreport = new Reports.PrintImage();
                            dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                            dlreport.Margins.Top = 10;
                            dlreport.Margins.Bottom = 10;
                            dlreport.Margins.Left = 50;
                            dlreport.Margins.Right = 50;
                            dlreport.PrintingSystem.ShowMarginsWarning = false;
                            dlreport.ShowPrintMarginsWarning = false;

                            //Florin 2020.05.22
                            string numeImprimanta = Dami.ValoareParam("TactilImprimantaFluturas").Trim();
                            if (numeImprimanta == "")
                                numeImprimanta = Dami.ValoareParam("TactilImprimanta").Trim();
                            if (numeImprimanta != "")
                                dlreport.PrinterName = numeImprimanta;

                            dlreport.CreateDocument();
                            ReportPrintTool pt = new ReportPrintTool(dlreport);
                            pt.Print();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
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

        protected void grDate_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                if (e.ButtonType == ColumnCommandButtonType.Edit)
                {
                    object row = ((ASPxGridView)sender).GetRow(e.VisibleIndex);
                    if (row == null) return;
                    int id = Convert.ToInt32(((DataRowView)row)["IdStare"]);
                    if (id == 1 || id == 2 || id == 4)
                        e.Enabled = true;
                    else
                        e.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void oRaspunsMemo_Init(object sender, EventArgs e)
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


        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["PrintImage"] != null)
                {
                    File.Delete(HostingEnvironment.MapPath("~/TEMP/") + Session["PrintImage"].ToString());
                    Session["PrintImage"] = null;
                }
                Response.Redirect("../Tactil/Main", false);       

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["PrintImage"] != null)
                {
                    File.Delete(HostingEnvironment.MapPath("~/TEMP/") + Session["PrintImage"].ToString());
                    Session["PrintImage"] = null;
                }
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0")
                    Response.Redirect("../DefaultTactil", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard", false);
                else if (tip == "3")
                    Response.Redirect("../DefaultTactilExtra", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}