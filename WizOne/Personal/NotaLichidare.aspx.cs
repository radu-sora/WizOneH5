﻿using DevExpress.Web;
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
using DevExpress.Utils.DPI;
using DevExpress.Web.Data;

namespace WizOne.Personal
{
    public partial class NotaLichidare : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                DataTable table = General.IncarcaDT("SELECT * FROM \"MP_NotaLichidare_Stari\"", null);
                GridViewDataComboBoxColumn col = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = table;

                table = General.IncarcaDT(SelectAngajati(), null);
                col = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                col.PropertiesComboBox.DataSource = table;

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                #endregion

                DataTable dt = General.IncarcaDT("SELECT * FROM \"MP_NotaLichidare_Stari\"", null);
                if (dt != null && dt.Rows.Count > 0)
                {   
                    ASPxListBox nestedListBox = checkComboBoxStare.FindControl("listBox") as ASPxListBox;
                    nestedListBox.Items.Clear();
                    nestedListBox.Items.Add("(Selectie toate)", 0);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        nestedListBox.Items.Add(dt.Rows[i]["Denumire"].ToString(), Convert.ToInt32(dt.Rows[i]["Id"].ToString()));
                        if (!IsPostBack)
                            if (Convert.ToInt32(dt.Rows[i]["Id"].ToString()) == 1)
                                nestedListBox.Items[nestedListBox.Items.Count - 1].Value = 1;
                    }
                }

                DataTable dtDat = new DataTable();
                dtDat.Columns.Add("Id", typeof(int));
                dtDat.Columns.Add("Denumire", typeof(string));

                dtDat.Rows.Add(1, "Are datorii");
                dtDat.Rows.Add(2, "Nu are datorii");
                cmbStareDatorii.DataSource = dtDat;
                cmbStareDatorii.DataBind();

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                if (!IsPostBack)
                {
                    string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                    string sql = "SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdUser\" = {0} AND \"IdSuper\" IN ({1})";
                    sql = string.Format(sql, Session["UserId"].ToString(), idHR);
                    DataTable dtHR = General.IncarcaDT(sql, null);
                    if (dtHR != null && dtHR.Rows.Count > 0 && dtHR.Rows[0][0] != null && dtHR.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)
                    {
                        Session["NL_HR"] = "1";
                        ASPxListBox nestedListBox = checkComboBoxStare.FindControl("listBox") as ASPxListBox;
                        for (int i = 0; i < nestedListBox.Items.Count; i++)
                        {
                            if (Convert.ToInt32(nestedListBox.Items[i].Value) == 1)
                                nestedListBox.Items[i].Selected = true;
                        }
                        grDateDet.ClientVisible = false;
                        lblStareDatorii.ClientVisible = false;
                        cmbStareDatorii.ClientVisible = false;
                    }
                    else
                    {
                        btnRap.ClientVisible = false;
                        grDate.ClientVisible = false;
                    }

                    DataTable dtStare = General.IncarcaDT("SELECT \"IdStare\", \"IdAuto\" FROM \"MP_NotaLichidare\"", null);
                    string sir = "";
                    for (int i = 0; i < dtStare.Rows.Count; i++)
                    {
                        sir += dtStare.Rows[i]["IdAuto"].ToString() + "," + dtStare.Rows[i]["IdStare"].ToString();
                        if (i < dtStare.Rows.Count - 1)
                            sir += ";";
                    }
                    Session["NL_Stare"] = sir;
                    cmbStareDatorii.SelectedIndex = 1;
                    IncarcaGrid();
                    IncarcaGridDet();

                }
                else
                {
                    if (Session["NL_HR"] != null && Session["NL_HR"].ToString() == "1")
                    {                        
                        grDateDet.ClientVisible = false;
                        lblStareDatorii.ClientVisible = false;
                        cmbStareDatorii.ClientVisible = false;
                    }
                    else
                    {
                        btnRap.ClientVisible = false;
                        grDate.ClientVisible = false;
                    }


                    DataTable dt1 = Session["NL_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt1;
                    grDate.DataBind();

                    DataTable dt2 = Session["NL_GridDet"] as DataTable;
                    grDateDet.KeyFieldName = "IdAuto";
                    grDateDet.DataSource = dt2;
                    grDateDet.DataBind();

                }



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
                string filtru = "";
                if (cmbAng.Value != null)
                    filtru += " AND F10003 = " + Convert.ToInt32(cmbAng.Value);
                else
                    filtru += " AND F10003 IN (SELECT F10003 FROM (" + SelectAngajati() + ") a)";

                if (checkComboBoxStare.Value != null)
                    filtru += " AND \"IdStare\" IN (" + FiltruTipStari(checkComboBoxStare.Value.ToString().Replace(";", ",")).Replace(";", ",").Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1) + ")";


                string sql = "SELECT * FROM \"MP_NotaLichidare\"  WHERE 1=1 " + filtru;
                DataTable dt =  General.IncarcaDT(sql, null);  
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;
                //if (!IsPostBack)
                    grDate.DataBind();
                Session["NL_Grid"] = dt;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void IncarcaGridDet()
        {
            try
            {
                string filtru = "";
                if (cmbAng.Value != null)
                    filtru += " AND F10003 = " + Convert.ToInt32(cmbAng.Value);
                else
                    filtru += " AND F10003 IN (SELECT F10003 FROM (" + SelectAngajati() + ") a)";

                if (cmbStareDatorii.Value != null)
                    filtru += " AND (\"Datorii\" " + (Convert.ToInt32(cmbStareDatorii.Value) == 1 ? " <> 0 AND \"Datorii\" IS NOT NULL)" : " = 0 OR \"Datorii\" IS NULL)");

                if (checkComboBoxStare.Value != null) 
                    filtru += " AND \"IdStare\" IN (" + FiltruTipStari(checkComboBoxStare.Value.ToString().Replace(";", ",")).Replace(";", ",").Substring(0, FiltruTipStari(checkComboBoxStare.Value.ToString()).Length - 1) + ")";


                string sql = "SELECT a.* FROM \"MP_NotaLichidare_Detalii\" a LEFT JOIN \"MP_NotaLichidare\" b on a.\"IdNotaLichidare\" = b.\"IdAuto\"  WHERE 1=1 " + filtru;
                //string sql = "SELECT a.* FROM \"MP_NotaLichidare_Detalii\" a  ";
                DataTable dt =  General.IncarcaDT(sql, null);               

                grDateDet.KeyFieldName = "IdAuto";
                grDateDet.DataSource = dt;
                //if (!IsPostBack)
                    grDateDet.DataBind();
                Session["NL_GridDet"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string FiltruTipStari(string stari)
        {
            string val = "";
            DataTable dt = General.IncarcaDT("SELECT * FROM \"MP_NotaLichidare_Stari\"", null);
            try
            {
                string[] param = stari.Split(',');
                foreach (string elem in param)
                {
                    if (dt != null && dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            if (dt.Rows[i]["Denumire"].ToString().ToLower() == elem.ToLower())
                            {
                                val += dt.Rows[i]["Id"].ToString().ToLower() + ";";
                                break;
                            }
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        protected void btnRap_Click(object sender, EventArgs e)
        {
            try
            {
                int idRap = Convert.ToInt32(Dami.ValoareParam("IdRaportNotaLichidare", "-99"));
                int marca = -99;

                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "F10003", "IdAuto" }) as object[];
                if (obj != null && obj.Count() > 0)
                    marca = Convert.ToInt32(obj[0]);
                else
                {
                    MessageBox.Show("Nu ati selectat nicio linie!", MessageBox.icoError, "Atentie !");
                    return;
                }

                var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
                var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, new { Angajat = marca.ToString() });

                Response.Redirect(reportUrl, false);
            }
            catch (Exception ex)
            {
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

                DataTable dt = Session["NL_Grid"] as DataTable;


                DataRow row = dt.Rows.Find(keys);

              
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;                                               

                    }

                    if (col.ColumnName == "USER_NO")
                        row[col.ColumnName] = Session["UserId"].ToString();
                    if (col.ColumnName == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["NL_Grid"] = dt;
                grDate.DataSource = dt;               

                General.SalveazaDate(dt, "MP_NotaLichidare");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDet_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["NL_GridDet"] as DataTable;


                DataRow row = dt.Rows.Find(keys);


                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateDet.Columns[col.ColumnName] != null && grDateDet.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;

                    }
                    if (col.ColumnName == "USER_NO")
                        row[col.ColumnName] = Session["UserId"].ToString();
                    if (col.ColumnName == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                e.Cancel = true;
                grDateDet.CancelEdit();
                Session["NL_GridDet"] = dt;
                grDateDet.DataSource = dt;

                General.SalveazaDate(dt, "MP_NotaLichidare_Detalii");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string ctrl = e.Parameter.Split(';')[0];
                switch (ctrl)
                { 
                    case "btnFiltru": 
                        IncarcaGrid();
                        IncarcaGridDet();
                        break;
                    case "btnFiltruSterge":
                        StergeFiltre();
                        break;             
                } 
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                grDate.CancelEdit();

                DataTable dt = Session["NL_Grid"] as DataTable;

                ASPxDataUpdateValues upd = e.UpdateValues[0] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) return;


                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = upd.NewValues[col.ColumnName] ?? DBNull.Value;

                    }

                    if (col.ColumnName == "USER_NO")
                        row[col.ColumnName] = Session["UserId"].ToString();
                    if (col.ColumnName == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                Session["NL_Grid"] = dt;
                grDate.DataSource = dt;
                General.SalveazaDate(dt, "MP_NotaLichidare");

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDet_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                grDateDet.CancelEdit();

                DataTable dt = Session["NL_GridDet"] as DataTable;

                ASPxDataUpdateValues upd = e.UpdateValues[0] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) return;


                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateDet.Columns[col.ColumnName] != null && grDateDet.Columns[col.ColumnName].Visible)
                    {
                        row[col.ColumnName] = upd.NewValues[col.ColumnName] ?? DBNull.Value;

                    }

                    if (col.ColumnName == "USER_NO")
                        row[col.ColumnName] = Session["UserId"].ToString();
                    if (col.ColumnName == "TIME")
                        row[col.ColumnName] = DateTime.Now;

                }

                Session["NL_GridDet"] = dt;
                grDateDet.DataSource = dt;
                General.SalveazaDate(dt, "MP_NotaLichidare_Detalii");

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void grDateDet_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        //{
        //    if (e.VisibleIndex >= 0)
        //    {
        //        DataRowView values = grDateDet.GetRow(e.VisibleIndex) as DataRowView;

        //        if (values != null)
        //        {
        //            int idAuto = Convert.ToInt32(values.Row["IdNotaLichidare"].ToString());

        //            DataTable dtStare = Session["NL_Stare"] as DataTable;

        //            if (dtStare != null && dtStare.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dtStare.Rows.Count; i++)
        //                {
        //                    if (idAuto == Convert.ToInt32(dtStare.Rows[i]["IdAuto"].ToString()))
        //                    {
        //                        if ((Convert.ToInt32(dtStare.Rows[i]["IdStare"].ToString()) == -1 || Convert.ToInt32(dtStare.Rows[i]["IdStare"].ToString()) == 3) && e.ButtonType == ColumnCommandButtonType.Edit)
        //                            e.Visible = false;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}



        protected void StergeFiltre(int param = 0)
        {      
            cmbAng.Value = null;
            cmbAng.SelectedIndex = -1;
            cmbStareDatorii.Value = null;
            cmbStareDatorii.SelectedIndex = -1;
        }


        private string SelectAngajati(int idRol = -44)
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT  A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"" 
                          FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {Session["UserId"]} AND (SELECT COUNT(*) FROM ""MP_NotaLichidare_Circuit"" WHERE ""Supervizor""=0) > 0
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        INNER JOIN ""MP_NotaLichidare_Circuit"" C ON B.""IdSuper"" = -1 * c.""Supervizor""
                        LEFT JOIN ""tblSupervizori"" D ON D.""Id"" = B.""IdSuper""
                        WHERE B.""IdUser""={Session["UserId"]}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""MP_NotaLichidare_Circuit"" C ON C.""Supervizor""={Session["UserId"]}
                        ) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F002 C ON A.F10002 = C.F00202
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
						WHERE A.F10025 IN (0, 999) ";

                //if (idRol != -44) strSql += @" AND ""Rol""=" + idRol;

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }






    }


}