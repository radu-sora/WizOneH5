using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Avs
{
    public partial class Aprobare : System.Web.UI.Page
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

                DataTable dtAtr = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Avs_tblAtribute"" ", null);
                GridViewDataComboBoxColumn colAtr = (grDate.Columns["IdAtribut"] as GridViewDataComboBoxColumn);
                colAtr.PropertiesComboBox.DataSource = dtAtr;

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY13", "CloseDeferedWindow();", true);
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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");              

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");               
                
                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                //if (!IsPostBack)
                //{

                //    //grDate.DataBind();


                //}

                IncarcaGrid();

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                DataTable dtAtr = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"Avs_tblAtribute\" ORDER BY \"Id\"", null);
                cmbAtributeFiltru.DataSource = dtAtr;
                cmbAtributeFiltru.DataBind();
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
                cmbAngFiltru.Value = null;
                cmbAtributeFiltru.Value = null;

                IncarcaGrid();
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
                DataTable dt = Session["Avs_Grid"] as DataTable;

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                        General.ExecutaNonQuery("UPDATE \"Avs_Cereri\" SET \"Explicatii\" = '" + dt.Rows[i]["Explicatii"].ToString() + "' WHERE \"Id\" = " + dt.Rows[i]["Id"].ToString(), null);
                    IncarcaGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnAproba_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
        //        string msg = "";
        //        //Florin 2019.03.04
        //        //Am modificat al treileae camp din Actiune in IdStare
        //        List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdStare", "NumeAngajat", "DataInceput", "Rol" });
        //        if (lst == null || lst.Count() == 0 || lst[0] == null) return;

        //        for (int i = 0; i < lst.Count(); i++)
        //        {
        //            object[] arr = lst[i] as object[];
        //            //switch (Convert.ToInt32(General.Nz(arr[1], 0)))
        //            //{
        //            //    case -1:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 0:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 3:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 4:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
        //            //        continue;
        //            //}

        //            ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
        //        }
        //        //General.MemoreazaEroarea("Vine din Avs Aprobare");
        //        if (ids.Count != 0) MetodeCereri(1);
        //        //msg += General.MetodeCereri(1, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
        //        //MessageBox.Show(msg, MessageBox.icoWarning, "Atentie !");
        //        //grDate.DataBind();
        //        IncarcaGrid();
        //        grDate.Selection.UnselectAll();


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void btnRespinge_Click(object sender, EventArgs e)
        //{
        //    try
        //    {                
        //        List<Module.General.metaCereriRol> ids = new List<Module.General.metaCereriRol>();
        //        string msg = "";
        //        //Florin 2019.03.04
        //        //Am modificat al treileae camp din Actiune in IdStare
        //        List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdStare", "NumeAngajat", "DataInceput", "Rol" });
        //        if (lst == null || lst.Count() == 0 || lst[0] == null) return;

        //        for (int i = 0; i < lst.Count(); i++)
        //        {
        //            object[] arr = lst[i] as object[];
        //            //switch (Convert.ToInt32(General.Nz(arr[1], 0)))
        //            //{
        //            //    case -1:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 0:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 3:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("este aprobata") + System.Environment.NewLine;
        //            //        continue;
        //            //    case 4:
        //            //        msg += "Cererea pt " + arr[3] + "-" + Convert.ToDateTime(arr[4]).ToShortDateString() + " - " + Dami.TraduCuvant("Nu puteti respinge o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
        //            //        continue;
        //            //}

        //            ids.Add(new Module.General.metaCereriRol { Id = Convert.ToInt32(General.Nz(arr[0], 0)), Rol = Convert.ToInt32(General.Nz(arr[5], 0)) });
        //        }
        //        //General.MemoreazaEroarea("Vine din Avs Aprobare");
        //        if (ids.Count != 0) MetodeCereri(2);
        //        //msg += General.MetodeCereri(2, ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
        //        //MessageBox.Show(msg, MessageBox.icoWarning, "Atentie !");
        //        //grDate.DataBind();
        //        IncarcaGrid();

        //        grDate.Selection.UnselectAll();


        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                //Florin 2019.03.04
                MetodeCereri(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRespinge_Click(object sender, EventArgs e)
        {
            try
            {
                //Florin 2019.03.04
                MetodeCereri(2);
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
 
                int f10003 = Convert.ToInt32(cmbAngFiltru.Value ?? -99);
                int idAtr = Convert.ToInt32(cmbAtributeFiltru.Value ?? -99);

                grDate.KeyFieldName = "Id";

                //Florin 2019.03.04
                //if (Session["Avs_Grid"] == null)
                    dt = GetCereriAprobare(f10003, idAtr, FiltruTipStari(ref checkComboBoxStare), 0, Convert.ToInt32(Session["UserId"].ToString()));
                //else
                //    dt = Session["Avs_Grid"] as DataTable;
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["Avs_Grid"] = dt;
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametrii");
                        //MessageBox.Show("Insuficienti parametrii", MessageBox.icoError, "Eroare !");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                MetodeCereri(3);
                                IncarcaGrid();
                            }
                            break;                       
                    }
                }
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
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Avs_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && !col.ReadOnly && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Avs_Grid"] = dt;
                grDate.DataSource = dt;
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
                if (e.VisibleIndex == -1) return;

                if (e.ButtonID == "btnDelete")
                {

                }
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

        private bool EsteActualizata(string marca, string idAtribut, string dataModif)
        {
            string sql = "SELECT F70420 FROM F704 WHERE F70403 = " + marca + " AND F70404 = " + idAtribut + " AND F70406 = " 
                + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dataModif + "', 103) " : "TO_DATE('"+ dataModif + "', 'dd/mm/yyyy')") ;
            DataTable dt = General.IncarcaDT(sql, null);
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                return true;
            else
                return false;
        }


        private void MetodeCereri(int tipActiune)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 

            try
            {
                int nrSel = 0;
                string ids = "", idsAtr = "", lstDataModif = "", lstMarci = "";
                string comentarii = "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdAtribut", "NumeAngajat", "DataModif", "F10003", "Revisal" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[4] as DateTime?;
                    switch (Convert.ToInt32(General.Nz(arr[1], 0)))
                    {
                        case -1:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este anulata") + System.Environment.NewLine;
                            continue;
                        case 0:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este respinsa") + System.Environment.NewLine;
                            continue;
                        case 3:
                            if (tipActiune == 3 && EsteActualizata(arr[5].ToString(), arr[2].ToString(), data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString()))
                            {
                                msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este deja actualizata in F100") + System.Environment.NewLine;
                                continue;
                            }
                            if (tipActiune == 1 || tipActiune == 2)
                            {
                                msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("este deja aprobata") + System.Environment.NewLine;
                                continue;
                            }
                            break;
                        case 4:
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Nu puteti aproba o cerere planificata. Trebuie trecuta in starea solicitat.") + System.Environment.NewLine;
                            continue;
                    }

                    //Florin 2019.03.26
                    //Nu se poate anula o cerere daca este trimisa in revisal
                    if (tipActiune == 3 && Convert.ToInt32(General.Nz(arr[6],0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Cererea este trimisa in Revisal") + System.Environment.NewLine;
                        continue;
                    }


                    if (Convert.ToInt32(General.Nz(arr[1], 0)) == 1 || Convert.ToInt32(General.Nz(arr[1], 0)) == 2 
                        || (Convert.ToInt32(General.Nz(arr[1], 0)) == 3 && !EsteActualizata(arr[5].ToString(), arr[2].ToString(), data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString())))
                    {
                        ids += Convert.ToInt32(General.Nz(arr[0], 0)) + ";";
                        idsAtr += Convert.ToInt32(General.Nz(arr[2], 0)) + ";";
                        lstDataModif += data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + ";";
                        lstMarci += Convert.ToInt32(General.Nz(arr[5], 0)) + ";";
                    }
                    nrSel++;

                }


                //Florin 2019.03.04

                //grDate.JSProperties["cpAlertMessage"] = msg;
                //grDate.DataBind();

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    else
                        grDate.JSProperties["cpAlertMessage"] = msg;

                    return;
                }

                msg = msg + AprobaCerere(Convert.ToInt32(Session["UserId"].ToString()), ids, idsAtr, lstDataModif, lstMarci, nrSel, tipActiune, General.ListaCuloareValoare()[5], false, comentarii, Convert.ToInt32(Session["User_Marca"].ToString()));
                grDate.JSProperties["cpAlertMessage"] = msg;
                //Session["Avs_Grid"] = null;
                IncarcaGrid();

                grDate.Selection.UnselectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string AprobaCerere(int idUser, string ids, string idsAtr, string lstDataModif, string lstMarci, int total, int actiune, string culoareValoare, bool HR, string comentarii, int f10003)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 


            string msg = "";
            string msgValid = "";

            try
            {
                if (ids == "") return "Nu exista cereri pentru aceasta actiune !";

                int nr = 0;
                string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrAtr = idsAtr.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrDataModif = lstDataModif.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrMarci = lstMarci.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {
                            string sql = "SELECT * FROM \"Avs_Cereri\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCer = General.IncarcaDT(sql, null);
                            sql = "SELECT * FROM \"Avs_CereriIstoric\" WHERE \"Id\" = " + id.ToString() + " AND \"IdUser\" = " + idUser.ToString() 
                                + (actiune == 1 || actiune == 2 ? " AND \"Aprobat\" IS NULL" : " AND (\"Aprobat\" IS NULL  OR (\"Aprobat\" = 1 AND \"Pozitie\" = " + dtCer.Rows[0]["TotalCircuit"].ToString() + "))");
                            DataTable dtCerIst = General.IncarcaDT(sql, null);

                            if (dtCerIst == null || dtCerIst.Rows.Count == 0) continue;


                            sql = "SELECT * FROM \"Avs_Circuit\" WHERE \"IdAtribut\" = " + arrAtr[i];
                            DataTable dtCir = General.IncarcaDT(sql, null);


                            //Florin 2019.03.25
                            //Nu exista campul RespectaOrdinea

                            //if (dtCir != null && dtCir.Rows.Count > 0)
                            //{
                            //    if (!HR && dtCir.Rows[0]["RespectaOrdinea"] != null && Convert.ToInt32(dtCir.Rows[0]["RespectaOrdinea"].ToString()) == 1)
                            //        if (Convert.ToInt32(General.Nz(dtCerIst.Rows[0]["Pozitie"], 0)) != (Convert.ToInt32(General.Nz(dtCer.Rows[0]["Pozitie"], 0)) + 1))
                            //            continue;
                            //}
                            //else
                            //    continue;

                            int idStare = 2;

                            if (HR)
                                idStare = 3;
                            else
                                if (idStare == 2 && dtCer.Rows[0]["TotalCircuit"].ToString() == dtCerIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

                            if (actiune == 2)
                                idStare = 0;

                            if (actiune == 3)
                            {
                                idStare = -1;
                                sql = "DELETE FROM F704 WHERE F70403 = " + arrMarci[i] + " AND F70404 = " + arrAtr[i] + " AND F70406 = "
                                          + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + arrDataModif[i] + "', 103) " : "TO_DATE('" + arrDataModif[i] + "', 'dd/mm/yyyy')"); 
                                General.ExecutaNonQuery(sql, null);
                            }

                            string culoare = "";
                            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString();
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != null && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";

                            sql = "UPDATE \"Avs_Cereri\" SET \"Pozitie\" = " + dtCerIst.Rows[0]["Pozitie"].ToString() + ", \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare 
                                + "' WHERE \"Id\" = " + id.ToString();
                            General.IncarcaDT(sql, null);

                            if (actiune == 1 || actiune == 2)
                            {
                                sql = "UPDATE \"Avs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                    + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                    + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                    + " WHERE \"Id\" = " + id.ToString();
                            }
                            else
                            {
                                //sql = "INSERT INTO \"Avs_CereriIstoric\" (\"Id\", \"IdCircuit\", \"IdUser\", \"IdStare\", \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", \"Inlocuitor\", \"IdSuper\") "
                                //    + " VALUES ({0}, {1}, {2}, -1, {3}, '{4}', 1, {5}, {6}, {7})";
                                //sql = string.Format(sql, id.ToString(), dtCerIst.Rows[0]["IdCircuit"].ToString(), dtCerIst.Rows[0]["IdCircuit"].ToString(), dtCerIst.Rows[0]["IdCircuit"].ToString(),
                                //    dtCerIst.Rows[0]["IdCircuit"].ToString(), )
                            }
                            General.IncarcaDT(sql, null);


                            #region Validare start

                            //string corpMesaj = "";
                            //bool stop;

                            //srvNotif ctxNtf = new srvNotif();
                            //ctxNtf.ValidareRegula("Avs.Aprobare", "grDate", entCer, idUser, f10003, out corpMesaj, out stop);

                            //if (corpMesaj != "")
                            //{
                            //    msgValid += corpMesaj + "\r\n";
                            //    if (stop)
                            //    {
                            //        continue;
                            //    }
                            //}

                            #endregion

                            //ctx.SaveChanges();

                            //Florin 2019.03.01
                            //s-a adaugat conditia cu parametrul
                            if (idStare == 3 && Dami.ValoareParam("FinalizareCuActeAditionale") == "0")
                            {
                                Cereri pag = new Cereri();
                                pag.TrimiteInF704(id);
                                if (Convert.ToInt32(General.Nz(dtCer.Rows[0]["IdAtribut"],-99)) == 2)
                                    General.ModificaFunctieAngajat(Convert.ToInt32(dtCer.Rows[0]["F10003"]), Convert.ToInt32(General.Nz(dtCer.Rows[0]["FunctieId"], -99)), Convert.ToDateTime(dtCer.Rows[0]["DataModif"]), new DateTime(2100, 1, 1));
                            }

                            if (actiune == 3)
                            {
                                try
                                {
                                    General.ExecutaNonQuery(
                                        $@"BEGIN
                                        DELETE FROM ""Admin_NrActAd"" WHERE ""IdAuto""=(SELECT ""IdActAd"" FROM ""Avs_Cereri"" WHERE ""ID""=@1);
                                        UPDATE ""Avs_Cereri"" SET ""IdActAd"" = null WHERE ""Id""=@1;
                                        END", new object[] { id });


                                    //UPDATE ""Admin_NrActAd"" SET ""DocNr""=null, ""DocData""=null, ""Tiparit""=0, ""Semnat""=0, ""Revisal""=0 WHERE ""IdAuto""=(SELECT ""IdActAd"" FROM ""Avs_Cereri"" WHERE ""ID""=@1);
                                }
                                catch (Exception){}
                            }

                            #region  Notificare start

                            //ctxNtf.TrimiteNotificare("Avs.Aprobare", "grDate", entCer, idUser, f10003);

                            #endregion

                            nr++;
                            
                        }
                    }
                }

                if (nr > 0)
                {
                    if (actiune == 1)
                        msg = "S-au aprobat " + nr.ToString() + " cereri din " + total + " !";
                    else
                        msg = "S-au respins " + nr.ToString() + " cereri din " + total + " !";

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista cereri pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }


        public DataTable GetCereriAprobare(int f10003, int idAtr, string filtruStari, int tip, int idUser)
        {
            //tip
            //0  -  Toate
            //1  -  Ale mele

            DataTable q = null;

            try
            {
                string strSql = "";
                string filtru = "";

                if (f10003 != -99) filtru += " AND a.F10003 = " + f10003;
                if (idAtr != -99) filtru += " AND a.\"IdAtribut\" = " + idAtr;
                if (filtruStari != "") filtru += " AND a.\"IdStare\" IN (" + filtruStari.Replace(";", ",").Substring(0, filtruStari.Length - 1) + ")";

                if (tip == 1)
                    filtru += " AND \"IdUser\"=" + idUser + " AND (a.\"Pozitie\" + 1) = f.\"Pozitie\" ";
                else
                    filtru += " AND \"IdUser\"=" + idUser + " AND (a.\"Pozitie\" <= f.\"Pozitie\" or a.\"IdStare\" = 3 or a.\"IdStare\" = 0) ";


                //Florin 2019.03.26
                //S-a adaugat coloana Revisal
                if (Constante.tipBD == 1)
                {
                    strSql = "select DISTINCT a.Id, a.F10003, b.F10008 + ' ' + b.F10009 AS NumeAngajat, a.IdAtribut, c.Denumire AS NumeAtribut , a.DataModif, a.Explicatii, a.Motiv, a.Actualizat, a.IdStare, a.Culoare,  " +
                            " case a.IdAtribut  " +
                            " when 1 then convert(nvarchar(20),a.SalariulBrut)  " +
                            " when 2 then a.FunctieNume  " +
                            " when 3 then a.CORNume  " +
                            " when 4 then a.MotivNume  " +
                            " when 5 then ''  " +
                            " when 6 then convert(nvarchar(20),a.Norma)  " +
                            " when 8 then convert(nvarchar(20),a.NrIntern) + ' / ' + convert(nvarchar(20),a.DataIntern,103)  " +
                            " when 9 then convert(nvarchar(20),a.NrITM) + ' / ' + convert(nvarchar(20),a.DataITM,103)  " +
                            " when 10 then convert(nvarchar(20),a.DataAngajarii,103)  " +
                            " when 11 then ''  " +
                            " when 12 then a.TitlulAcademicNume  " +
                            " when 13 then a.MesajPersonalNume  " +
                            " when 14 then a.CentruCostNume  " +
                            " when 15 then ''  " +
                            " when 16 then a.PunctLucruNume  " +
                            " when 22 then a.MeserieNume  " +
                            " when 23 then a.SectieNume  " +
                            " when 24 then a.DeptNume  " +
                            " when 25 then convert(nvarchar(20),a.DataInceputCIM,103) + ' - ' + convert(nvarchar(20),a.DataSfarsitCIM,103)  " +
                            " when 26 then convert(nvarchar(20),a.DataInceputCIM,103) + ' - ' + convert(nvarchar(20),a.DataSfarsitCIM,103)  " +
                            " end AS ValoareNoua,  " +
                            " a.SalariulNet, a.ScutitImpozit,  " +
                            " COALESCE((SELECT COALESCE(NR.Revisal,0) FROM Admin_NrActAd NR WHERE NR.IdAuto=COALESCE(A.IdActAd,-99)),0) AS Revisal " +
                            " from Avs_Cereri a  " +
                            " inner join F100 b on a.F10003=b.F10003  " +
                            " inner join Avs_tblAtribute c on a.IdAtribut=c.Id  " +
                            " inner join Avs_CereriIstoric f on a.Id = f.Id  " +
                            " where 1=1 " + filtru +
                            " order by a.DataModif";
                }
                else
                {
                    strSql = "select DISTINCT a.\"Id\", a.F10003, b.F10008 || ' ' || b.F10009 AS \"NumeAngajat\", a.\"IdAtribut\", c.\"Denumire\" AS \"NumeAtribut\" , a.\"DataModif\", a.\"Explicatii\", a.\"Motiv\", a.\"Actualizat\", a.\"IdStare\", a.\"Culoare\", " +
                            " case a.\"IdAtribut\" " +
                            " when 1 then to_char(a.\"SalariulBrut\") " +
                            " when 2 then a.\"FunctieNume\" " +
                            " when 3 then a.\"CORNume\" " +
                            " when 4 then a.\"MotivNume\" " +
                            " when 5 then '' " +
                            " when 6 then to_char(a.\"Norma\") " +
                            " when 8 then to_char(a.\"NrIntern\") || ' / ' || to_char(a.\"DataIntern\",'DD/MM/YYYY') " +
                            " when 9 then to_char(a.\"NrITM\") || ' / ' || to_char(a.\"DataITM\",'DD/MM/YYYY') " +
                            " when 10 then to_char(a.\"DataAngajarii\",'DD/MM/YYYY') " +
                            " when 11 then '' " +
                            " when 12 then a.\"TitlulAcademicNume\" " +
                            " when 13 then a.\"MesajPersonalNume\" " +
                            " when 14 then a.\"CentruCostNume\" " +
                            " when 15 then '' " +
                            " when 16 then a.\"PunctLucruNume\" " +
                            " when 22 then a.\"MeserieNume\" " +
                            " when 23 then a.\"SectieNume\" " +
                            " when 24 then a.\"DeptNume\" " +
                            " when 25 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
                            " when 26 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
                            " end AS \"ValoareNoua\", " +
                            " a.\"SalariulNet\", a.\"ScutitImpozit\", " +
                            " COALESCE((SELECT COALESCE(NR.\"Revisal\",0) FROM \"Admin_NrActAd\" NR WHERE NR.\"IdAuto\"=COALESCE(A.\"IdActAd\",-99)),0) AS \"Revisal\" " +
                            " from \"Avs_Cereri\" a " +
                            " inner join F100 b on a.F10003=b.F10003 " +
                            " inner join \"Avs_tblAtribute\" c on a.\"IdAtribut\"=c.\"Id\" " +
                            " inner join \"Avs_CereriIstoric\" f on a.\"Id\" = f.\"Id\" " +
                            " where 1=1 " + filtru +
                            " order by a.\"DataModif\"";
                }

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public static string FiltruTipStari(ref ASPxDropDownEdit cmbTip)
        {
            string val = "";

            try
            {
                List<object> lst = cmbTip.Value as List<object>;
                foreach (var ele in lst)
                {
                    switch (ele.ToString().ToLower())
                    {
                        case "solicitat":
                            val += "1;";
                            break;
                        case "in curs":
                            val += "2;";
                            break;
                        case "aprobat":
                            val += "3;";
                            break;
                        case "respins":
                            val += "0;";
                            break;
                        case "anulat":
                            val += "-1;";
                            break;
                        case "planificat":
                            val += "4;";
                            break;
                        case "prezent":
                            val += "5;";
                            break;
                        case "absent":
                            val += "6;";
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return val;
        }


        private string SelectAngajati()
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        WHERE B.""IdUser""= {Session["UserId"]}) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";

            }
            catch (Exception ex)
            {
                //ArataMesaj("Atentie !");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


    }
}