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
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Beneficii
{
    public partial class SesiuniBeneficii : System.Web.UI.Page
    {

        public class metaSesiuniBen
        {
            public int Id { get; set; }
            public DateTime DataInceput { get; set; }
            public DateTime DataSfarsit { get; set; }
 
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
                btnInit.Text = Dami.TraduCuvant("btnInit", "Initiaza");


                lblDeLa.InnerText = Dami.TraduCuvant("De la");
                lblLa.InnerText = Dami.TraduCuvant("La");


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
                {
                    Session["SesiuniBen_Grid"] = null;
                    txtDataInc.Value = new DateTime(DateTime.Now.Year, 1, 1);
                    txtDataSf.Value = new DateTime(DateTime.Now.Year, 12, 31);
                    IncarcaGrid();
                }
                else
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


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                Session["SesiuniBen_Grid"] = null;
                IncarcaGrid();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltruSterge_Click(object sender, EventArgs e)
        {
            try
            {
                txtDataInc.Value = null;
                txtDataSf.Value = null;
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
                DateTime dtInc = new DateTime(1900, 1, 1);
                DateTime dtSf = new DateTime(2100, 1, 1);

                if (txtDataInc. Value != null && txtDataSf.Value != null)
                {
                    dtInc = Convert.ToDateTime(txtDataInc.Value);
                    dtSf = Convert.ToDateTime(txtDataSf.Value);
                }

                if (Session["SesiuniBen_Grid"] == null)
                {
                    grDate.KeyFieldName = "Id";

                    dt = General.IncarcaDT("SELECT * FROM \"Ben_tblSesiuni\" WHERE \"DataInceput\" <= " + General.ToDataUniv(dtSf) + " AND \"DataSfarsit\" >= " + General.ToDataUniv(dtInc), null);

                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["SesiuniBen_Grid"] = dt;
                }
                else
                {
                    grDate.KeyFieldName = "Id";

                    dt = Session["SesiuniBen_Grid"] as DataTable;
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

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
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



        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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


        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
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


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }          
               


        protected void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
            //    string msg = "";
            //    DataTable dt = Session["SesiuniBen_Grid"] as DataTable;
            //    List<metaSesiuniBen> ids = new List<metaSesiuniBen>();
            //    string lstIds = "";
            //    List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "DataInceput", "DataSfarsit", "IdStare" });
            //    if (lst == null || lst.Count() == 0 || lst[0] == null) return;

            //    for (int i = 0; i < lst.Count(); i++)
            //    {
            //        object[] arr = lst[i] as object[];    

            //        ids.Add(new metaSesiuniBen { Id = Convert.ToInt32(General.Nz(arr[0], 0)), DataInceput = Convert.ToDateTime(General.Nz(arr[1], 0)), DataSfarsit = Convert.ToDateTime(General.Nz(arr[2], 0)) });
            //        lstIds += ", " + General.Nz(arr[0], 0);
            //}

            //    if (ids.Count != 0)
            //    {
            //        msg += InitializareSesiune(ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
            //        General.ExecutaNonQuery("UPDATE Ben_tblSesiuni SET IdStare = 1 WHERE Id IN (" + lstIds.Substring(1) + ")", null);
            //        Session["SesiuniBen_Grid"] = null;
            //        IncarcaGrid();
            //    }

            //    grDate.JSProperties["cpAlertMessage"] = msg;
            //    grDate.Selection.UnselectAll();  

            //    MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string InitializareSesiune(List<metaSesiuniBen> arr, int idUser, int userMarca)
        {
            string log = string.Empty;
            try
            {
                if (arr.Count == 0) return "Eroare";

                for (int i = 0; i < arr.Count; i++)
                {
                    DataTable dtAng = General.IncarcaDT("SELECT a.* FROM Ben_SetAngajatiDetail a LEFT JOIN Ben_relSesGrupAng b ON a.IdGrup = b.IdGrup WHERE b.IdSesiune = " + arr[i].Id.ToString(), null);
                    if (dtAng != null && dtAng.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtAng.Rows.Count; j++)
                        {
                            General.ExecutaNonQuery("DELETE FROM Ben_Cereri WHERE IdSesiune = " + arr[i].Id.ToString() + " AND F10003 = " + dtAng.Rows[j]["F10003"].ToString(), null);

                            DataTable dtBen = General.IncarcaDT("INSERT INTO Ben_Cereri(IdSesiune, F10003, IdStare, USER_NO, TIME) OUTPUT Inserted.IdAuto VALUES(" + arr[i].Id.ToString() + ", " + dtAng.Rows[j]["F10003"].ToString() + ", 1, " 
                                + idUser + ", GETDATE()) ", null);

                            string[] arrParam = new string[] { General.UrlHost(), General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                            int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);
                            int idAuto = -99;
                            if (dtBen.Rows.Count > 0)
                                idAuto = Convert.ToInt32(dtBen.Rows[0]["IdAuto"]);                            

                            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                            {
                                NotifAsync.TrimiteNotificare("Beneficii.Aprobare", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM Ben_Cereri Z WHERE IdAuto=" + idAuto, "Ben_Cereri", idAuto, idUser, marcaUser, arrParam);
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log = ex.Message;
                General.MemoreazaEroarea(ex, "Beneficii", "InitializareSesiune");
            }

            return log;
        }


        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["SesiuniBen_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
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

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["SesiuniBen_Grid"] = dt;
                grDate.DataSource = dt;
                grDate.DataBind();

                General.SalveazaDate(dt, "Ben_tblSesiuni");
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
                DataTable dt = Session["SesiuniBen_Grid"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;

                
                

                if (Constante.tipBD == 1)
                {
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

                if (e.NewValues["Denumire"] == null || e.NewValues["Denumire"].ToString().Length <= 0)
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
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
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
                grDate.KeyFieldName = "Id";
                Session["SesiuniBen_Grid"] = dt;
                grDate.DataBind();

                General.SalveazaDate(dt, "Ben_tblSesiuni");
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

                DataTable dt = Session["SesiuniBen_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["SesiuniBen_Grid"] = dt;
                grDate.DataSource = dt;

                General.SalveazaDate(dt, "Ben_tblSesiuni");

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
                //e.NewValues["IdStare"] = 0;  

                int idSesMax = 1;
                string sql = "SELECT MAX(\"Id\") + 1 FROM \"Ben_tblSesiuni\"   ";
                DataTable dtTmp = General.IncarcaDT(sql, null);
                if (dtTmp != null & dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null)
                    idSesMax = Convert.ToInt32(dtTmp.Rows[0][0].ToString()) + 1;

                e.NewValues["Id"] = idSesMax;
                //if (e.NewValues["IdStare"] == null)
                //    e.NewValues["IdStare"] = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void ListAng_DataBound(object sender, EventArgs e)
        //{
        //    var listBox = (ASPxListBox)sender;

        //    int editingRowVisibleIndex = grDate.EditingRowVisibleIndex;
        //    string rowValue = grDate.GetRowValues(editingRowVisibleIndex, "SeturiAngajati").ToString();
        //    string[] rowValueItems = rowValue.Split(',');

        //    List<string> rowValueItemsAsList = new List<string>();
        //    rowValueItemsAsList.AddRange(rowValueItems);

        //    foreach (ListEditItem item in listBox.Items)
        //    {
        //        if (rowValueItemsAsList.Contains(item.Value.ToString()))
        //        {
        //            item.Selected = true;
        //        }
        //    }
        //}
    }




}