using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using DevExpress.Web.ASPxHtmlEditor;

namespace WizOne.Pagini
{
    public partial class AtribuireDocument : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                #endregion
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");



                if (IsPostBack)
                {
                    DataTable dt = Session["AtribuireDoc_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                }
                else
                    IncarcaGrid();
                 
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void IncarcaGrid()
        {
            try
            {

                DataTable dt = General.IncarcaDT("SELECT * FROM \"tblAtribuireDoc\"", null);
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;              
                grDate.DataBind();
                Session["AtribuireDoc_Grid"] = dt;   

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
                if (e.Parameters.Length > 0)
                {
                    ASPxComboBox cmbInreg = grDate.FindEditFormTemplateControl("cmbInreg") as ASPxComboBox;
                    if (cmbInreg != null)
                    {
                        string sql = "";
                        switch (Convert.ToInt32(e.Parameters))
                        {
                            case 1:
                                sql = "SELECT F00304 AS \"Id\", F00305 AS \"Denumire\" FROM F003";
                                break;
                            case 2:
                                sql = "SELECT F00405 AS \"Id\", F00406 AS \"Denumire\" FROM F004";
                                break;
                            case 3:
                                sql = "SELECT F00506 AS \"Id\", F00507 AS \"Denumire\" FROM F005";
                                break;
                            case 4:
                                sql = "SELECT F00607 AS \"Id\", F00608 AS \"Denumire\" FROM F006";
                                break;
                            case 5:
                                sql = "SELECT F71802 AS \"Id\", F71804 AS \"Denumire\" FROM F718";
                                break;
                        }
                        DataTable dtInreg = General.IncarcaDT(sql, null);
                        cmbInreg.DataSource = dtInreg;
                        cmbInreg.DataBindItems();
                    }
                }
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
                DataTable dt = Session["AtribuireDoc_Grid"] as DataTable;
                DataRow dr = dt.NewRow();

                ASPxComboBox cmbEntitate = grDate.FindEditFormTemplateControl("cmbEntitate") as ASPxComboBox;
                ASPxComboBox cmbInreg = grDate.FindEditFormTemplateControl("cmbInreg") as ASPxComboBox;
                ASPxHtmlEditor txtDoc = grDate.FindEditFormTemplateControl("txtDoc") as ASPxHtmlEditor;

                if (Constante.tipBD == 1)
                    dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                else
                    dr["IdAuto"] = Dami.NextId("tblAtribuireDoc");

                dr["Entitate"] = cmbEntitate.Text;
                dr["IdEntitate"] = cmbEntitate.Value ?? DBNull.Value;
                dr["Inregistrare"] = cmbInreg.Text;
                dr["IdInregistrare"] = cmbInreg.Value ?? DBNull.Value;
                dr["Document"] = txtDoc.Html;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["AtribuireDoc_Grid"] = dt;
                General.SalveazaDate(dt, "tblAtribuireDoc");
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
                var idAuto = e.Keys["IdAuto"];

                DataTable dt = Session["AtribuireDoc_Grid"] as DataTable;

                DataRow dr = dt.Rows.Find(idAuto);

                ASPxComboBox cmbEntitate = grDate.FindEditFormTemplateControl("cmbEntitate") as ASPxComboBox;
                ASPxComboBox cmbInreg = grDate.FindEditFormTemplateControl("cmbInreg") as ASPxComboBox;
                ASPxHtmlEditor txtDoc = grDate.FindEditFormTemplateControl("txtDoc") as ASPxHtmlEditor;

                dr["Entitate"] = cmbEntitate.Text;
                dr["IdEntitate"] = cmbEntitate.Value ?? DBNull.Value;
                dr["Inregistrare"] = cmbInreg.Text;
                dr["IdInregistrare"] = cmbInreg.Value ?? DBNull.Value;
                dr["Document"] = txtDoc.Html;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["AtribuireDoc_Grid"] = dt;
                General.SalveazaDate(dt, "tblAtribuireDoc");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AtribuireDoc_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);               

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["AtribuireDoc_Grid"] = dt;
                grDate.DataSource = dt;
                General.SalveazaDate(dt, "tblAtribuireDoc");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxGridView grid = (ASPxGridView)sender;
                int index = grid.EditingRowVisibleIndex;

                ASPxComboBox cmbEntitate = grDate.FindEditFormTemplateControl("cmbEntitate") as ASPxComboBox;
                if (cmbEntitate != null)
                {
                    DataTable dtEnt = new DataTable();
                    dtEnt.Columns.Add("Id", typeof(int));
                    dtEnt.Columns.Add("Denumire", typeof(string));

                    dtEnt.Rows.Add(1, "Subcompanie");
                    dtEnt.Rows.Add(2, "Filiala");
                    dtEnt.Rows.Add(3, "Sectie");
                    dtEnt.Rows.Add(4, "Departament");
                    dtEnt.Rows.Add(5, "Functie");

                    cmbEntitate.DataSource = dtEnt;
                    cmbEntitate.DataBindItems();
                }

                ASPxComboBox cmbInreg = grDate.FindEditFormTemplateControl("cmbInreg") as ASPxComboBox;
                if (cmbInreg != null)
                {
                    string sql = "";

                    object id = grid.GetRowValues(index, "IdEntitate");

                    switch (Convert.ToInt32(id))
                    {
                        case 1:
                            sql = "SELECT F00304 AS \"Id\", F00305 AS \"Denumire\" FROM F003";
                            break;
                        case 2:
                            sql = "SELECT F00405 AS \"Id\", F00406 AS \"Denumire\" FROM F004";
                            break;
                        case 3:
                            sql = "SELECT F00506 AS \"Id\", F00507 AS \"Denumire\" FROM F005";
                            break;
                        case 4:
                            sql = "SELECT F00607 AS \"Id\", F00608 AS \"Denumire\" FROM F006";
                            break;
                        case 5:
                            sql = "SELECT F71802 AS \"Id\", F71804 AS \"Denumire\" FROM F718";
                            break;
                    }
                    DataTable dtInreg = General.IncarcaDT(sql, null);
                    cmbInreg.DataSource = dtInreg;
                    cmbInreg.DataBindItems();
                }

                object text = grid.GetRowValues(index, "Document");
                if (text != null)
                {
                    ASPxHtmlEditor txtDoc = grDate.FindEditFormTemplateControl("txtDoc") as ASPxHtmlEditor;
                    txtDoc.Html = text.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }


//                                case "12":
//                                //Radu 23.07.2020 
//                                DataTable dtAtr = General.IncarcaDT("SELECT * FROM \"tblAtribuireDoc\"", null);
//    DataRow drAtr = dtAtr.Select("IdAuto = " + id).FirstOrDefault();
//                                if (drAtr != null)
//                                {
//                                    string numeFis = (drAtr["Inregistrare"] == DBNull.Value ? "" : drAtr["Inregistrare"].ToString() + ".doc");
//    string ext = ".doc";
//    byte[] sir = Encoding.ASCII.GetBytes(drAtr["Document"] == DBNull.Value ? "" : drAtr["Document"].ToString());
//    scrieDoc(ext, sir, numeFis);
//}
//                                else
//                                    Response.Write("Nu exista date de afisat !");
//                                break;


}