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

        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        DataTable dtAtr = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Avs_tblAtribute"" ", null);
        //        GridViewDataComboBoxColumn colAtr = (grDate.Columns["IdAtribut"] as GridViewDataComboBoxColumn);
        //        colAtr.PropertiesComboBox.DataSource = dtAtr;

        //        DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
        //        GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
        //        colStari.PropertiesComboBox.DataSource = dtStari;

        //        //Page.ClientScript.RegisterStartupScript(this.GetType(), "ANY_KEY13", "CloseDeferedWindow();", true);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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


                DataTable dtAtr = General.IncarcaDT(SelectAtribute(), null);
                GridViewDataComboBoxColumn colAtr = (grDate.Columns["IdAtribut"] as GridViewDataComboBoxColumn);
                colAtr.PropertiesComboBox.DataSource = dtAtr;

                cmbAtributeFiltru.DataSource = dtAtr;
                cmbAtributeFiltru.DataBind();

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                DataTable dtRol = General.IncarcaDT(SelectRoluri(), null);
                cmbRol.DataSource = dtRol;
                cmbRol.DataBind();

                if (!IsPostBack && dtRol != null && dtRol.Rows.Count > 0) cmbRol.SelectedIndex = 0;

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

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
                checkComboBoxStare.Value = null;

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
        //        //MessageBox.Show(msg, MessageBox.icoWarning, "");
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
        //        //MessageBox.Show(msg, MessageBox.icoWarning, "");
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
 
                //int f10003 = Convert.ToInt32(cmbAngFiltru.Value ?? -99);
                //int idAtr = Convert.ToInt32(cmbAtributeFiltru.Value ?? -99);

                grDate.KeyFieldName = "Id";

                //Florin 2019.05.20

                ////Florin 2019.03.04
                ////if (Session["Avs_Grid"] == null)
                //    dt = GetCereriAprobare(f10003, idAtr, FiltruTipStari(ref checkComboBoxStare), 0, Convert.ToInt32(Session["UserId"].ToString()));
                ////else
                ////    dt = Session["Avs_Grid"] as DataTable;


                //Florin 2019.05.27
                //dt = GetCereriAprobare(0, Convert.ToInt32(Session["UserId"].ToString()));
                dt = SelectGrid();


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
                                //{f10003}, {dtCer.Rows[0]["MotivSuspId"].ToString()},{data11}, {data12}, {data13},
                                MetodeCereri(3,1);
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

                if (e.VisibleIndex >= 0)
                {
                    DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;
                    if (values != null)
                    {
                        int idAtr = Convert.ToInt32(values.Row["IdAtribut"].ToString());
                        if (e.ButtonID == "btnDetalii")
                        {
                            if (idAtr != (int)Constante.Atribute.Sporuri && idAtr != (int)Constante.Atribute.SporTranzactii
                                && idAtr != (int)Constante.Atribute.Componente && idAtr != (int)Constante.Atribute.Tarife)
                            {
                                e.Visible = DefaultBoolean.False;

                            }
                        }

                        if (e.ButtonID == "btnArata")
                        {
                            if (idAtr == (int)Constante.Atribute.BancaSalariu || idAtr == (int)Constante.Atribute.BancaGarantii
                                || idAtr == (int)Constante.Atribute.DocId || idAtr == (int)Constante.Atribute.PermisAuto)
                            {
                                e.Visible = DefaultBoolean.True;

                            }
                            else
                                e.Visible = DefaultBoolean.False;
                        }

                    }
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

        private bool VerificareSalariu(int salariu, int timpPartial)
        {
            int salMin = Convert.ToInt32(Dami.ValoareParam("SAL_MIN", "0"));
            if (salMin * timpPartial / 8 > salariu)
                return false;
            else
                return true;
        }


        private void MetodeCereri(int tipActiune, int tipMsg = 0)
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

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdAtribut", "NumeAngajat", "DataModif", "F10003", "Revisal", "Actualizat", "PoateModifica", "ValoareNoua", "Semnat" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    if (tipMsg == 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
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
                            if ((tipActiune == 2 || tipActiune == 3) && EsteActualizata(arr[5].ToString(), arr[2].ToString(), data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString()))
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

                    //Florin 2020.08.04
                    //Nu se poate respinge sau anula o cerere pt program de lucru daca este trimisa in status semnat
                    if ((tipActiune == 2 || tipActiune == 3) && (Convert.ToInt32(General.Nz(arr[2], 0)) == 34 || Convert.ToInt32(General.Nz(arr[2], 0)) == 35) && Convert.ToInt32(General.Nz(arr[10], 0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Cererea este trimisa la semnat") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2019.05.23
                    //Nu se poate respinge sau anula o cerere daca este actualizata in F100
                    if ((tipActiune == 2 || tipActiune == 3) && Convert.ToInt32(General.Nz(arr[7], 0)) == 1)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Datele au fost trimise in personal") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2019.05.28
                    //daca este aprobare sau respingere si nu are dreptul de a modifica, il blocam; aici, implicit, se respecta ordinea
                    //algoritmul de modificare se calculeaza in selectul cu care se populeaza grodul
                    //daca este HR are voie, daca 'Fara Rol' si userul logat este la pozitie care trebuie sa aprobe poate modifica, daca rolul selectat di combo box si userul logat este acelasi cu idSuper si user-ul de la pozitia care trebuie sa aprobe poate modifica
                    if ((tipActiune == 1 || tipActiune == 2) && Convert.ToInt32(General.Nz(arr[8], 0)) == 0)
                    {
                        msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Nu este randul dvs.") + System.Environment.NewLine;
                        continue;
                    }

                    //Florin 2020.05.13
                    if (tipActiune == 3 && (Convert.ToInt32(General.Nz(arr[2],0)) == Convert.ToInt32(Constante.Atribute.Suspendare) || Convert.ToInt32(General.Nz(arr[2], 0)) == Convert.ToInt32(Constante.Atribute.RevenireSuspendare)))
                    {
                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(
                            @"SELECT COUNT(*)
                            FROM ""Avs_Cereri"" B
                            INNER JOIN F111 A ON A.F11103 = B.F10003 AND A.F11104 = B.""IdAtribut"" AND CAST(A.F11105 AS DATE) = CAST(B.""DataInceputSusp"" AS DATE)
                            AND CAST(A.F11106 AS DATE) = CAST(B.""DataSfEstimSusp"" AS DATE) AND CAST(A.F11107 AS DATE) = CAST(B.""DataIncetareSusp"" AS DATE)
                            WHERE B.""Id""=706", new object[] { arr[0] }),0));
                        if (cnt == 1)
                        {
                            msg += "Cererea pt " + arr[3] + "-" + data.Value.Day.ToString().PadLeft(2, '0') + "/" + data.Value.Month.ToString().PadLeft(2, '0') + "/" + data.Value.Year.ToString() + " - " + Dami.TraduCuvant("Datele au fost actualizate in personal") + System.Environment.NewLine;
                            continue;
                        }
                    }

                    //Radu 14.11.2019 - verificare norma si salariu
                    if ((Convert.ToInt32(General.Nz(arr[2], 0)) == (int)Constante.Atribute.Norma || Convert.ToInt32(General.Nz(arr[2], 0)) == (int)Constante.Atribute.Salariul) && (tipActiune == 2 || tipActiune == 3))
                    {
                        string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");

                        string sql = "select f100.f10003 as Marca, case when f100991 is null or f100991 = convert(datetime, '01/01/2100', 103) then  "
                        + "  convert(datetime, '01/' + convert(varchar, f01012) + '/' + convert(varchar, f01011), 103) "
                        + " else f100991 end as Data, COALESCE(" + salariu + ", 0) as Valoare from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                        + " union "
                        + " select f70403 as Marca, f70406 as Data, COALESCE(F70407, 0) as Valoare from f704 where f70404 = 1 and f70420 = 0 "
                        + " union "
                        + " select f10003 as Marca, datamodif as Data, COALESCE(SalariulBrut, 0) as Valoare from Avs_Cereri where IdAtribut = 1 and IdStare in (1, 2, 3)";
                        if (Constante.tipBD == 2)
                            sql = "select f100.f10003 as \"Marca\", case when f100991 is null or f100991 = TO_DATE('01/01/2100', 'dd/mm/yyyy') then  "
                                + "  TO_DATE('01/' ||  f01012 || '/' || F01011, 'dd/mm/yyyy') "
                                + " else f100991 end as \"Data\", COALESCE(" + salariu + ", 0) as \"Valoare\" from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                                + " union "
                                + " select f70403 as \"Marca\", f70406 as \"Data\", COALESCE(F70407, 0) as \"Valoare\" from f704 where f70404 = 1 and f70420 = 0 "
                                + " union "
                                + " select f10003 as \"Marca\", \"DataModif\" as \"Data\", COALESCE(\"SalariulBrut\", 0) as \"Valoare\" from \"Avs_Cereri\" where \"IdAtribut\" = 1 and \"IdStare\" in (1, 2, 3)";
                        DataTable dtSal = General.IncarcaDT(sql, null);
                        string dtModif = General.ToDataUniv(Convert.ToDateTime(data).Year, Convert.ToDateTime(data).Month, Convert.ToDateTime(data).Day);                  

                        sql = "select f100.f10003 as Marca, case when f100955 is null or f100955 = convert(datetime, '01/01/2100', 103) then  "
                        + "  convert(datetime, '01/' + convert(varchar, f01012) + '/' + convert(varchar, f01011), 103) "
                        + " else f100955 end as Data, COALESCE(f10043, 0) as Valoare from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                        + " union "
                        + " select f70403 as Marca, f70406 as Data, COALESCE(f70422, 0) as Valoare from f704 where f70404 = 6 and f70420 = 0 "
                        + " union "
                        + " select f10003 as Marca, datamodif as Data, COALESCE(TimpPartial, 0) as Valoare from Avs_Cereri where IdAtribut = 6 and IdStare in (1, 2, 3)";
                        if (Constante.tipBD == 2)
                            sql = "select f100.f10003 as \"Marca\", case when f100955 is null or f100955 = TO_DATE('01/01/2100', 'dd/mm/yyyy') then  "
                                + "  TO_DATE('01/' ||  f01012 || '/' || F01011, 'dd/mm/yyyy') "
                                + " else f100955 end as \"Data\", COALESCE(f10043, 0) as \"Valoare\" from f100 left join f1001 on f100.f10003 = f1001.f10003 left join f010 on 1 = 1 "
                                + " union "
                                + " select f70403 as \"Marca\", f70406 as \"Data\", COALESCE(f70422, 0) as \"Valoare\" from f704 where f70404 = 6 and f70420 = 0 "
                                + " union "
                                + " select f10003 as \"Marca\", \"DataModif\" as \"Data\", COALESCE(\"TimpPartial\", 0) as \"Valoare\" from \"Avs_Cereri\" where \"IdAtribut\" = 6 and \"IdStare\" in (1, 2, 3)";
                        DataTable dtNorma = General.IncarcaDT(sql, null);                          
                        
                        if (Convert.ToInt32(General.Nz(arr[2], 0)) == (int)Constante.Atribute.Norma)
                        {
                            DataRow[] drSal = dtSal.Select("Marca = " + arr[5] + " AND Data <= " + dtModif, "Data DESC");
                            DataRow[] drNorma = dtNorma.Select("Marca = " + arr[5] + " AND Data < " + dtModif, "Data DESC");
                            if (!VerificareSalariu(Convert.ToInt32(Convert.ToDouble(drSal[0]["Valoare"].ToString())), Convert.ToInt32(drNorma[0]["Valoare"].ToString())))
                            {
                                msg += Dami.TraduCuvant("Salariul angajatului este mai mic decat cel minim (raportat la timp partial)!\nVa rugam sa anulati mai intai cererea de modificare salariu in concordanta cu acest Timp partial!\n");
                                continue;
                            }
                        }
                        if (Convert.ToInt32(General.Nz(arr[2], 0)) == (int)Constante.Atribute.Salariul)
                        {
                            DataRow[] drSal = dtSal.Select("Marca = " + arr[5] + " AND Data < " + dtModif, "Data DESC");
                            DataRow[] drNorma = dtNorma.Select("Marca = " + arr[5] + " AND Data <= " + dtModif, "Data DESC");
                            if (!VerificareSalariu(Convert.ToInt32(Convert.ToDouble(drSal[0]["Valoare"].ToString())), Convert.ToInt32(drNorma[0]["Valoare"].ToString())))
                            {
                                msg += Dami.TraduCuvant("Salariul angajatului este mai mic decat cel minim (raportat la timp partial)!\nVa rugam sa anulati si cererea de modificare norma contract in concordanta cu acest salariu!\n");                             
                            }
                        }              
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
                    {
                        if (tipMsg == 0)
                            MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                        else
                            grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";

                    }
                    else
                    {
                        if (tipMsg == 0)
                            MessageBox.Show(msg, MessageBox.icoWarning, "");
                        else
                            grDate.JSProperties["cpAlertMessage"] = msg;
                    }
                    return;
                }

                msg = msg + AprobaCerere(Convert.ToInt32(Session["UserId"].ToString()), ids, idsAtr, lstDataModif, lstMarci, nrSel, tipActiune, General.ListaCuloareValoare()[5], false, comentarii, Convert.ToInt32(Session["User_Marca"].ToString()));
                //grDate.JSProperties["cpAlertMessage"] = msg;
                //Session["Avs_Grid"] = null;
                if (tipMsg == 0)
                    MessageBox.Show(msg, MessageBox.icoWarning, "");
                else
                    grDate.JSProperties["cpAlertMessage"] = msg;

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

                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                if (("," + idHR + ",").IndexOf("," + General.Nz(cmbRol.Value, -99).ToString() + ",") >= 0)
                    HR = true;

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
                            sql = "SELECT * FROM \"Avs_CereriIstoric\" WHERE \"Id\" = " + id.ToString() + (!HR ? " AND \"IdUser\" = " + idUser.ToString() : "") 
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

                            //Florin 2019.05.28
                            //if (HR)
                            //    idStare = 3;
                            //else
                            //    if (idStare == 2 && dtCer.Rows[0]["TotalCircuit"].ToString() == dtCerIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

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
                                //Florin 2019.05.28
                                //sql = "UPDATE \"Avs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                //    + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                //    + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                //    + " WHERE \"Id\" = " + id.ToString() + " AND \"IdUSer\"=" + Session["UserId"];
                                sql = "UPDATE \"Avs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                + " WHERE \"Id\" = " + id.ToString() + " AND \"Pozitie\"=" + dtCerIst.Rows[0]["Pozitie"].ToString();
                            }
                            else
                            {
                                //Florin 2019.06.03
                                //daca anuleaza, introducem o linie noua cu anulat
                                sql = $@"INSERT INTO ""Avs_CereriIstoric"" (""Id"", ""IdCircuit"", ""IdUser"", ""IdStare"", ""Pozitie"", ""Culoare"", ""Aprobat"", ""DataAprobare"", ""Inlocuitor"", ""IdSuper"")
                                        VALUES ({id}, {dtCerIst.Rows[0]["IdCircuit"]}, {Session["UserId"]}, -1, 22, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1), 1, {General.CurrentDate()}, null, {-1 * Convert.ToInt32(General.Nz(cmbRol.Value,0))})";
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
                            //Florin 2019.07.29
                            //s-a adaugat si parametrul cu id-uri excluse
                            string idExcluse = "," + Dami.ValoareParam("IdExcluseCircuitDoc") + ",";
                            if (idStare == 3 && (Dami.ValoareParam("FinalizareCuActeAditionale") == "0" || (Dami.ValoareParam("FinalizareCuActeAditionale") == "1" && idExcluse.IndexOf("," + id + ",") >= 0)))
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


        ////public DataTable GetCereriAprobare(int f10003, int idAtr, string filtruStari, int tip, int idUser)
        //public DataTable GetCereriAprobare(int tip, int idUser)
        //{
        //    //tip
        //    //0  -  Toate
        //    //1  -  Ale mele

        //    DataTable q = null;

        //    try
        //    {
        //        string strSql = "";
        //        string filtru = "";

        //        if (Convert.ToInt32(cmbAngFiltru.Value ?? -99) != -99) filtru += " AND a.F10003 = " + Convert.ToInt32(cmbAngFiltru.Value ?? -99);
        //        if (Convert.ToInt32(cmbAtributeFiltru.Value ?? -99) != -99) filtru += " AND a.\"IdAtribut\" = " + Convert.ToInt32(cmbAtributeFiltru.Value ?? -99);

        //        //Florin 2019.05.20
        //        //if (filtruStari != "") filtru += " AND a.\"IdStare\" IN (" + filtruStari.Replace(";", ",").Substring(0, filtruStari.Length - 1) + ")";
        //        if (checkComboBoxStare.Value != null) filtru += @" AND A.""IdStare"" IN (" + DamiStari() + ")";

        //        if (tip == 1)
        //            filtru += " AND \"IdUser\"=" + idUser + " AND (a.\"Pozitie\" + 1) = f.\"Pozitie\" ";
        //        else
        //            filtru += " AND \"IdUser\"=" + idUser + " AND (a.\"Pozitie\" <= f.\"Pozitie\" or a.\"IdStare\" = 3 or a.\"IdStare\" = 0) ";


        //        //Florin 2019.03.26
        //        //S-a adaugat coloana Revisal
        //        if (Constante.tipBD == 1)
        //        {
        //            strSql = "select DISTINCT a.Id, a.F10003, b.F10008 + ' ' + b.F10009 AS NumeAngajat, a.IdAtribut, c.Denumire AS NumeAtribut , a.DataModif, a.Explicatii, a.Motiv, a.Actualizat, a.IdStare, a.Culoare,  " +
        //                    " case a.IdAtribut  " +
        //                    " when 1 then convert(nvarchar(20),a.SalariulBrut)  " +
        //                    " when 2 then a.FunctieNume  " +
        //                    " when 3 then a.CORNume  " +
        //                    " when 4 then a.MotivNume  " +
        //                    " when 5 then ''  " +
        //                    " when 6 then convert(nvarchar(20),a.Norma)  " +
        //                    " when 8 then convert(nvarchar(20),a.NrIntern) + ' / ' + convert(nvarchar(20),a.DataIntern,103)  " +
        //                    " when 9 then convert(nvarchar(20),a.NrITM) + ' / ' + convert(nvarchar(20),a.DataITM,103)  " +
        //                    " when 10 then convert(nvarchar(20),a.DataAngajarii,103)  " +
        //                    " when 11 then ''  " +
        //                    " when 12 then a.TitlulAcademicNume  " +
        //                    " when 13 then a.MesajPersonalNume  " +
        //                    " when 14 then a.CentruCostNume  " +
        //                    " when 15 then ''  " +
        //                    " when 16 then a.PunctLucruNume  " +
        //                    " when 22 then a.MeserieNume  " +
        //                    " when 23 then a.SectieNume  " +
        //                    " when 24 then a.DeptNume  " +
        //                    " when 25 then convert(nvarchar(20),a.DataInceputCIM,103) + ' - ' + convert(nvarchar(20),a.DataSfarsitCIM,103)  " +
        //                    " when 26 then convert(nvarchar(20),a.DataInceputCIM,103) + ' - ' + convert(nvarchar(20),a.DataSfarsitCIM,103)  " +
        //                    " end AS ValoareNoua,  " +
        //                    " a.SalariulNet, a.ScutitImpozit,  " +
        //                    " COALESCE((SELECT COALESCE(NR.Revisal,0) FROM Admin_NrActAd NR WHERE NR.IdAuto=COALESCE(A.IdActAd,-99)),0) AS Revisal, " +
        //                    " COALESCE((SELECT COALESCE(X.F70420,0) FROM F704 X WHERE X.F70403=A.F10003 AND X.F70404=A.IdAtribut AND X.F70406=A.DataModif),0) AS ActualizatF704 " +
        //                    " from Avs_Cereri a  " +
        //                    " inner join F100 b on a.F10003=b.F10003  " +
        //                    " inner join Avs_tblAtribute c on a.IdAtribut=c.Id  " +
        //                    " inner join Avs_CereriIstoric f on a.Id = f.Id  " +
        //                    " where COALESCE(B.F10025,0) <> 900 " + filtru +
        //                    " order by a.DataModif";
        //        }
        //        else
        //        {
        //            strSql = "select DISTINCT a.\"Id\", a.F10003, b.F10008 || ' ' || b.F10009 AS \"NumeAngajat\", a.\"IdAtribut\", c.\"Denumire\" AS \"NumeAtribut\" , a.\"DataModif\", a.\"Explicatii\", a.\"Motiv\", a.\"Actualizat\", a.\"IdStare\", a.\"Culoare\", " +
        //                    " case a.\"IdAtribut\" " +
        //                    " when 1 then to_char(a.\"SalariulBrut\") " +
        //                    " when 2 then a.\"FunctieNume\" " +
        //                    " when 3 then a.\"CORNume\" " +
        //                    " when 4 then a.\"MotivNume\" " +
        //                    " when 5 then '' " +
        //                    " when 6 then to_char(a.\"Norma\") " +
        //                    " when 8 then to_char(a.\"NrIntern\") || ' / ' || to_char(a.\"DataIntern\",'DD/MM/YYYY') " +
        //                    " when 9 then to_char(a.\"NrITM\") || ' / ' || to_char(a.\"DataITM\",'DD/MM/YYYY') " +
        //                    " when 10 then to_char(a.\"DataAngajarii\",'DD/MM/YYYY') " +
        //                    " when 11 then '' " +
        //                    " when 12 then a.\"TitlulAcademicNume\" " +
        //                    " when 13 then a.\"MesajPersonalNume\" " +
        //                    " when 14 then a.\"CentruCostNume\" " +
        //                    " when 15 then '' " +
        //                    " when 16 then a.\"PunctLucruNume\" " +
        //                    " when 22 then a.\"MeserieNume\" " +
        //                    " when 23 then a.\"SectieNume\" " +
        //                    " when 24 then a.\"DeptNume\" " +
        //                    " when 25 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
        //                    " when 26 then to_char(a.\"DataInceputCIM\",'DD/MM/YYYY') || ' - ' || to_char(a.\"DataSfarsitCIM\",'DD/MM/YYYY') " +
        //                    " end AS \"ValoareNoua\", " +
        //                    " a.\"SalariulNet\", a.\"ScutitImpozit\", " +
        //                    " COALESCE((SELECT COALESCE(NR.\"Revisal\",0) FROM \"Admin_NrActAd\" NR WHERE NR.\"IdAuto\"=COALESCE(A.\"IdActAd\",-99)),0) AS \"Revisal\", " +
        //                    " COALESCE((SELECT COALESCE(X.F70420,0) FROM F704 X WHERE X.F70403=A.F10003 AND X.F70404=A.\"IdAtribut\" AND X.F70406=A.\"DataModif\"),0) AS \"ActualizatF704\" " +
        //                    " from \"Avs_Cereri\" a " +
        //                    " inner join F100 b on a.F10003=b.F10003 " +
        //                    " inner join \"Avs_tblAtribute\" c on a.\"IdAtribut\"=c.\"Id\" " +
        //                    " inner join \"Avs_CereriIstoric\" f on a.\"Id\" = f.\"Id\" " +
        //                    " where COALESCE(B.F10025,0) <> 900 " + filtru +
        //                    " order by a.\"DataModif\"";
        //        }

        //        q = General.IncarcaDT(strSql, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return q;
        //}


        //public static string FiltruTipStari(ref ASPxDropDownEdit cmbTip)
        //{
        //    string val = "";

        //    try
        //    {
        //        List<object> lst = cmbTip.Value as List<object>;
        //        foreach (var ele in lst)
        //        {
        //            switch (ele.ToString().ToLower())
        //            {
        //                case "solicitat":
        //                    val += "1;";
        //                    break;
        //                case "in curs":
        //                    val += "2;";
        //                    break;
        //                case "aprobat":
        //                    val += "3;";
        //                    break;
        //                case "respins":
        //                    val += "0;";
        //                    break;
        //                case "anulat":
        //                    val += "-1;";
        //                    break;
        //                case "planificat":
        //                    val += "4;";
        //                    break;
        //                case "prezent":
        //                    val += "5;";
        //                    break;
        //                case "absent":
        //                    val += "6;";
        //                    break;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return val;
        //}


        //private string SelectAngajati()
        //{
        //    string strSql = "";

        //    try
        //    {
        //        //Florin 2019.04.25
        //        //s-a adaugat conditia F10025 <> 900;  sa nu apara candidatii

        //        string op = "+";
        //        if (Constante.tipBD == 2) op = "||";

        //        strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
        //                X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
        //                FROM (
        //                SELECT A.F10003
        //                FROM F100 A
        //                WHERE COALESCE(A.F10025,0) <> 900 AND A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
        //                UNION
        //                SELECT A.F10003
        //                FROM F100 A
        //                INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
        //                WHERE COALESCE(A.F10025,0) <> 900 AND B.""IdUser""= {Session["UserId"]}) B
        //                INNER JOIN F100 A ON A.F10003=B.F10003
        //                LEFT JOIN F718 X ON A.F10071=X.F71802
        //                LEFT JOIN F003 F ON A.F10004 = F.F00304
        //                LEFT JOIN F004 G ON A.F10005 = G.F00405
        //                LEFT JOIN F005 H ON A.F10006 = H.F00506
        //                LEFT JOIN F006 I ON A.F10007 = I.F00607";

        //    }
        //    catch (Exception ex)
        //    {
        //        //ArataMesaj("");
        //        //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return strSql;
        //}

        private string SelectAngajati()
        {
            string strSql = "";

            try
            {
                //Florin 2019.05.27
                //strSql = $@"SELECT DISTINCT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS ""NumeComplet"", 
                //        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                //        FROM ""Avs_CereriIstoric"" B
                //        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id""
                //        INNER JOIN F100 A ON A.F10003=C.F10003
                //        LEFT JOIN F718 X ON A.F10071=X.F71802
                //        LEFT JOIN F003 F ON A.F10004 = F.F00304
                //        LEFT JOIN F004 G ON A.F10005 = G.F00405
                //        LEFT JOIN F005 H ON A.F10006 = H.F00506
                //        LEFT JOIN F006 I ON A.F10007 = I.F00607
                //        LEFT JOIN ""F100Supervizori"" FF on C.F10003 = FF.F10003 AND (-1 * B.""IdSuper"") = FF.""IdSuper""
                //        LEFT JOIN ""F100Supervizori"" GG ON C.F10003 = GG.F10003 AND CHARINDEX(',' + CONVERT(nvarchar(20),GG.""IdSuper"") + ','  ,  ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0   
                //        WHERE (B.""IdSuper"" >= 0 AND B.""IdUser""={Session["UserId"]}) OR (B.""IdSuper"" < 0 AND FF.""IdUser""={Session["UserId"]})
                //        OR gg.""IdUser"" = {Session["UserId"]} ";

                string filtru = "";
                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                if (("," + idHR + ",").IndexOf("," + General.Nz(cmbRol.Value, -99).ToString() + ",") < 0)
                {
                    if (Convert.ToInt32(cmbRol.Value ?? -99) != -99)
                    {
                        filtru += @" AND  b.idsuper = " + Convert.ToInt32(cmbRol.Value ?? -99);
                    }
                }

                strSql = $@"select F10003, ""NumeComplet"", ""Functia"", ""Subcompanie"", ""Filiala"", ""Sectie"", ""Departament""
                        from
                        (SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"",
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id""
                        INNER JOIN F100 A ON A.F10003 = C.F10003
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" FF on C.f10003 = FF.f10003 and(-1 * B.""IdSuper"") = FF.IdSuper
                        WHERE case when B.""IdSuper"" >= 0 then  B.""IdUser"" else FF.IdUser end = {Session["UserId"]}
                        OR c.F10003 in (select b.f10003 from f100supervizori b where b.iduser = {Session["UserId"]} {filtru})
                        union
                        SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament""
                        FROM ""Avs_Cereri"" C
                        INNER JOIN F100 A ON A.F10003 = C.F10003
                        LEFT JOIN F718 X ON A.F10071 = X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        left join ""F100Supervizori"" GG on C.f10003 = GG.f10003 and CHARINDEX(',' + CONVERT(nvarchar(20),GG.""IdSuper"") + ','  ,  ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0                     
                        WHERE gg.iduser = {Session["UserId"]} ) T order by T.""NumeComplet"" ";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string SelectRoluri()
        {
            string strSql = "";

            try
            {
                //Florin 2019.05.27
                strSql = $@"SELECT DISTINCT 
                        CASE WHEN B.""IdSuper"" > 0 THEN 76 ELSE (-1 * B.""IdSuper"") END AS ""Rol"",  
                        CASE WHEN B.""IdSuper"" > 0 THEN 'Fara Rol' ELSE COALESCE(A.""Alias"", A.""Denumire"") END AS ""RolDenumire""
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id"" 
                        LEFT JOIN ""tblSupervizori"" A ON A.""Id""=(-1 * B.""IdSuper"")
                        LEFT JOIN ""F100Supervizori"" F on C.f10003 = F.f10003 AND (-1 * B.""IdSuper"") = F.""IdSuper""
                        WHERE (B.""IdSuper"" >= 0 AND B.""IdUser""={Session["UserId"]}) OR (B.""IdSuper"" < 0 AND F.""IdUser""={Session["UserId"]})";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string SelectAtribute()
        {
            string strSql = "";

            try
            {
                //Florin 2019.05.27
                //strSql = $@"SELECT DISTINCT C.""Id"", C.""Denumire""
                //        FROM ""Avs_CereriIstoric"" B
                //        INNER JOIN ""Avs_Cereri"" A ON A.Id=B.Id
                //        INNER JOIN ""Avs_tblAtribute"" C ON C.""Id""=A.""IdAtribut""
                //        WHERE B.""IdUser"" = {Session["UserId"]}
                //        ORDER BY C.""Denumire""";


                strSql = $@"SELECT ""Id"", ""Denumire"" FROM (					 
                        SELECT DISTINCT C.""Id"", C.""Denumire""
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" A ON A.Id = B.Id
                        INNER JOIN ""Avs_tblAtribute"" C ON C.""Id"" = A.""IdAtribut""
                        LEFT JOIN ""F100Supervizori"" FF on A.f10003 = FF.f10003 AND (-1 * B.""IdSuper"") = FF.IdSuper
                        WHERE case when B.""IdSuper"" >= 0 then B.""IdUser"" else FF.IdUser end = {Session["UserId"]}
                        union
                        SELECT DISTINCT C.""Id"", C.""Denumire""
                        FROM ""Avs_Cereri"" A 
                        INNER JOIN ""Avs_tblAtribute"" C ON C.""Id"" = A.""IdAtribut""
                        left join ""F100Supervizori"" GG on A.f10003 = GG.f10003 and CHARINDEX(',' + CONVERT(nvarchar(20),GG.""IdSuper"") + ','  ,  ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0                 
                        WHERE gg.iduser = {Session["UserId"]} ) T
                        ORDER BY T.""Denumire"" ";


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string DamiStari()
        {
            string val = "";

            try
            {
                if (General.Nz(checkComboBoxStare.Value, "").ToString() != "")
                {
                    val = checkComboBoxStare.Value.ToString().ToLower().Replace("solicitat", "1").Replace("in curs", "2").Replace("aprobat", "3").Replace("respins", "0").Replace("anulat", "-1").Replace("planificat", "4");
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        public DataTable SelectGrid()
        {

            DataTable q = null;

            try
            {
                string strSql = "";
                string filtru = "";


                if (Convert.ToInt32(cmbAngFiltru.Value ?? -99) != -99) filtru += " AND a.F10003 = " + Convert.ToInt32(cmbAngFiltru.Value ?? -99);
                if (Convert.ToInt32(cmbAtributeFiltru.Value ?? -99) != -99) filtru += " AND a.\"IdAtribut\" = " + Convert.ToInt32(cmbAtributeFiltru.Value ?? -99);
                if (checkComboBoxStare.Value != null) filtru += @" AND A.""IdStare"" IN (" + DamiStari() + ")";

                //daca este rol de hr aratam toate cererile
                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                if (("," + idHR + ",").IndexOf("," + General.Nz(cmbRol.Value, -99).ToString() + ",") < 0)
                {
                    if (Convert.ToInt32(cmbRol.Value ?? -99) != -99)
                    {
                        filtru += @" AND ((G.""IdUser"" = " + Session["UserId"] + @" AND (CASE WHEN G.""IdSuper"" > 0 THEN 76 ELSE (-1 * G.""IdSuper"") END) = " + Convert.ToInt32(cmbRol.Value ?? -99)
                            + @")  OR a.F10003 in (select b.f10003 from F100Supervizori b where b.iduser= " + Session["UserId"] + @" and b.idsuper = " + Convert.ToInt32(cmbRol.Value ?? -99) + @")) ";
                    }
                    else
                    {
                        filtru += @" AND (G.""IdUser"" = " + Session["UserId"] + @" OR a.F10003 IN (select b.f10003 from F100Supervizori b where b.iduser= " + Session["UserId"] + @"))";
                    }
                }
                else
                {
                    //aratam toate cererile
                }


                if (Constante.tipBD == 1)
                {
                    strSql = "select DISTINCT a.Id, a.F10003, b.F10008 + ' ' + b.F10009 AS NumeAngajat, a.IdAtribut, c.Denumire AS NumeAtribut , a.DataModif, a.Explicatii, a.Motiv, a.Actualizat, a.IdStare, a.Culoare,  " +
                            " case a.IdAtribut  " +
                            " when 1 then convert(nvarchar(20),a.SalariulBrut)  " +
                            " when 2 then a.FunctieNume  " +
                            " when 3 then a.CORNume  " +
                            " when 4 then a.MotivNume  " +
                            " when 5 then a.SubcompanieNume + ' / ' + a.FilialaNume + ' / ' + a.SectieNume + ' / ' +  a.DeptNume + ' / ' + a.SubdeptNume + ' / ' + a.BirouNume " +
                            " when 6 then convert(nvarchar(20),a.TimpPartial)  " +
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
                            " when 34 then (select \"Ptj_Contracte\".\"Denumire\" from \"Ptj_Contracte\" where \"Ptj_Contracte\".\"Id\" = a.\"ProgramLucru\") " +
                            " when 35 then a.\"TipContractNume\"" +
                            " when 36 then a.\"DurataContractNume\"" +
                            " end AS ValoareNoua,  " +
                            " a.SalariulNet, a.ScutitImpozit,  " +
                            " COALESCE((SELECT COALESCE(NR.Revisal,0) FROM Admin_NrActAd NR WHERE NR.IdAuto=COALESCE(A.IdActAd,-99)),0) AS Revisal, " +
                            " COALESCE((SELECT COALESCE(NR.Semnat,0) FROM Admin_NrActAd NR WHERE NR.IdAuto=COALESCE(A.IdActAd,-99)),0) AS Semnat, " +
                            " COALESCE((SELECT COALESCE(X.F70420,0) FROM F704 X WHERE X.F70403=A.F10003 AND X.F70404=A.IdAtribut AND X.F70406=A.DataModif),0) AS ActualizatF704, " +
                            " CASE WHEN CHARINDEX('," + General.Nz(cmbRol.Value,-99).ToString() + ",', ',' + (SELECT Valoare FROM tblParametrii WHERE Nume='Avans_IDuriRoluriHR') + ',') > 0 THEN 1 ELSE  " +
                            " CASE WHEN " + General.Nz(cmbRol.Value, -99).ToString() + " = 76 AND F.IdUser = " + Session["UserId"] + " THEN 1 ELSE " +
                            " CASE WHEN -1 * " + General.Nz(cmbRol.Value, -99).ToString() + " = F.IdSuper AND F.IdUser= " + Session["UserId"] + " THEN 1 ELSE 0 END END END AS PoateModifica " +
                            " from Avs_Cereri a  " +
                            " inner join F100 b on a.F10003=b.F10003  " +
                            " inner join Avs_tblAtribute c on a.IdAtribut=c.Id  " +
                            " LEFT join Avs_CereriIstoric f on a.Id = f.Id AND (A.Pozitie+1)=F.Pozitie " +
                            " INNER JOIN Avs_CereriIstoric G ON A.Id = G.Id " + 
                            " where COALESCE(B.F10025,0) <> 900 " + filtru +
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
                            " when 6 then to_char(a.\"TimpPartial\") " +
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
                            " when 34 then (select \"Ptj_Contracte\".\"Denumire\" from \"Ptj_Contracte\" where \"Ptj_Contracte\".\"Id\" = a.\"ProgramLucru\") " +
                            " when 35 then a.\"TipContractNume\"" +
                            " when 36 then a.\"DurataContractNume\"" +
                            " end AS \"ValoareNoua\", " +
                            " a.\"SalariulNet\", a.\"ScutitImpozit\", " +
                            " COALESCE((SELECT COALESCE(NR.\"Revisal\",0) FROM \"Admin_NrActAd\" NR WHERE NR.\"IdAuto\"=COALESCE(A.\"IdActAd\",-99)),0) AS \"Revisal\", " +
                            " COALESCE((SELECT COALESCE(NR.\"Semnat\",0) FROM \"Admin_NrActAd\" NR WHERE NR.\"IdAuto\"=COALESCE(A.\"IdActAd\",-99)),0) AS \"Semnat\", " +
                            " COALESCE((SELECT COALESCE(X.F70420,0) FROM F704 X WHERE X.F70403=A.F10003 AND X.F70404=A.\"IdAtribut\" AND X.F70406=A.\"DataModif\"),0) AS \"ActualizatF704\", " +
                            " CASE WHEN INSTR(',' + (SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\"='Avans_IDuriRoluriHR') + ',', '," + General.Nz(cmbRol.Value, -99).ToString() + ",') > 0 THEN 1 ELSE  " +
                            " CASE WHEN " + General.Nz(cmbRol.Value, -99).ToString() + " = 76 AND F.\"IdUser\" = " + Session["UserId"] + " THEN 1 ELSE " +
                            " CASE WHEN -1 * " + General.Nz(cmbRol.Value, -99).ToString() + " = F.\"IdSuper\" AND F.\"IdUser\"= " + Session["UserId"] + " THEN 1 ELSE 0 END END END AS \"PoateModifica\" " +
                            " from \"Avs_Cereri\" a " +
                            " inner join F100 b on a.F10003=b.F10003 " +
                            " inner join \"Avs_tblAtribute\" c on a.\"IdAtribut\"=c.\"Id\" " +
                            " LEFT join \"Avs_CereriIstoric\" f on a.\"Id\" = f.\"Id\" AND (A.\"Pozitie\"+1)=F.\"Pozitie\" " +
                            " where COALESCE(B.F10025,0) <> 900 " + filtru +
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




    }
}