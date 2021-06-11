﻿using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ConcediiMedicale
{
    public partial class Aprobare : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

       

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                txtTitlu.Text = Dami.TraduCuvant("Aprobare concediu medical");

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnTransfera.Text = Dami.TraduCuvant("btnTransfera", "Transfera");
                btnAdauga.Text = Dami.TraduCuvant("btnAdauga", "Adauga CM");

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
                CriptDecript prc = new CriptDecript();
                //string param = General.Nz(Request["tip"], prc.EncryptString(Constante.cheieCriptare, "Introducere", 1)).ToString();
                //param = prc.EncryptString(Constante.cheieCriptare, param, 2);
                //int tip = param == "Aprobare" ? 2 : 1;
                int tip = Convert.ToInt32(General.Nz(Request["tip"], "1").ToString());

                if (tip == 1)
                {
                    txtTitlu.Text = Dami.TraduCuvant("Introducere concediu medical");             
                    btnAproba.ClientVisible = false;
                    btnTransfera.ClientVisible = false; 
                    btnRapCM.ClientVisible = false;
                }
                else
                    btnAdauga.ClientVisible = false;

                //txtTitlu.Text = General.VarSession("Titlu").ToString();    

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""CM_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;  
 
                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                if (!IsPostBack)
                {
                    Session["CM_Grid"] = null;
                    Session["CM_Aprobare"] = null;
                    Session["CM_Id"] = null;
                    Session["CM_Stare"] = null;
                }

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
                Session["CM_Grid"] = null;
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                string ids = "" ;
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");         
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[4] as DateTime?;

                    if (Convert.ToInt32(General.Nz(arr[1], 0)) == 0 || Convert.ToInt32(General.Nz(arr[2], 0)) == 0 || Convert.ToInt32(General.Nz(arr[3], 0)) == 0 || Convert.ToInt32(General.Nz(arr[4], 0)) == 0)
                    {
                        msg += "CM pt marca " + arr[5] + " - " + Dami.TraduCuvant("Date lipsa") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) > 1)
                    {
                        msg += "CM pt marca " + arr[5] + " - " + Dami.TraduCuvant("deja aprobat") + System.Environment.NewLine;
                        continue;
                    }

                    ids += "," + arr[0];
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)                    
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");  
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }
                string[] lstIds = ids.Substring(1).Split(',');
                for (int i = 0; i < lstIds.Length; i++)
                {
                    string sql = "UPDATE CM_CereriIstoric SET Aprobat = 1, DataAprobare = GETDATE(), CULOARE = (SELECT Culoare FROM CM_tblStari WHERE Id = 3), IdUser = " + Session["UserId"].ToString() + " WHERE IdCerere = " + lstIds[i] + " AND Pozitie = 2";
                    General.ExecutaNonQuery(sql, null);

                    sql = "UPDATE CM_Cereri SET IdStare = 3 WHERE Id = " + lstIds[i];
                    General.ExecutaNonQuery(sql, null);

                }

                Session["CM_Grid"] = null;
                IncarcaGrid();

                MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + (nrSel == 1 ? "S-a aprobat un concediu medical!" : "S-au aprobat " + nrSel + " concedii medicale!"), MessageBox.icoInfo, "");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAdauga_Click(object sender, EventArgs e)
        {
            try
            {
                string url = "~/ConcediiMedicale/Introducere.aspx";
                Session["CM_Id"] = null;
                //Session["FormDetaliu_IdFormular"] = Convert.ToInt32(cmbForm.Items[cmbForm.SelectedIndex].Value.ToString());
                //Session["FormDetaliu_IdStare"] = 0;
                //Session["FormDetaliu_PoateModifica"] = 1;
                //Session["FormDetaliu_EsteNou"] = 1;
                //Session["FormDetaliu_Pozitie"] = 1;

                //Session["FormDetaliu_NumeFormular"] = cmbForm.Text;
                //Session["FormDetaliu_DataVigoare"] = DateTime.Now;

                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void btnTransfera_Click(object sender, EventArgs e)
        {
            try
            {
                int nrSel = 0;
                string ids = "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "BazaCalculCM", "ZileBazaCalculCM", "MedieZileBazaCalcul", "MedieZilnicaCM", "F10003", "IdStare" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    return;
                }
                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[4] as DateTime?;

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) < 3)
                    {
                        msg += "CM pt marca " + arr[5] + " - " + Dami.TraduCuvant("nu este aprobat") + System.Environment.NewLine;
                        continue;
                    }

                    if (Convert.ToInt32(General.Nz(arr[6], 0)) == 4)
                    {
                        msg += "CM pt marca " + arr[5] + " - " + Dami.TraduCuvant("deja transferat") + System.Environment.NewLine;
                        continue;
                    }

                    ids += "," + arr[0];
                    nrSel++;
                }

                if (nrSel == 0)
                {
                    if (msg.Length <= 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
                        MessageBox.Show(msg, MessageBox.icoWarning, "");
                    return;
                }
                string[] lstIds = ids.Substring(1).Split(',');
                for (int i = 0; i < lstIds.Length; i++)
                {
                    DataTable dt = General.IncarcaDT("SELECT * FROM CM_Cereri WHERE Id = " + lstIds[i], null);

                    string sql = "INSERT INTO F300 (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30015, F30035, F30036, F30037, F30038, F30050,  USER_NO, TIME,"
                             + " F30012, F30014,  F30021, F30022, F30023, F30039, F30040, F30041, F30044, F30045, F30046, F30051, F30053, F300612, F300613, F300614, F30054, F30042, F300620, F300601, F300602, F300607, F300609, F300610, F300619, F300603, F300606, F300608, F300611, F300615, F300616, F300617, F300618, F30013)"
                             + " SELECT 300, F10002, F10003, F10004, F10005, F10006, F10007, {0},  1, {1}, {2}, {2}, "
                             + " {3}, {4}, CASE WHEN F10053 IS NULL OR F10053=0 THEN F00615 ELSE F10053 END, "
                             + " {5}, {6}, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {8}, {9}, {10}, 0, 'Transfer', {11}, '{12}', '{13}', {14}, {15}, {16}, {17}, {2}, '{18}', '{19}', {20}, '{21}', '{22}', '{23}', {24}, {25}"
                             + " FROM F100, F006 WHERE F10003 = {7} AND F10007=F00607";
                    sql = string.Format(sql, dt.Rows[0]["Cod"].ToString(), dt.Rows[0]["Suma"].ToString().ToString(new CultureInfo("en-US")).Replace(',', '.'),
                        "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataCM"].ToString()).Year + "', 103)",
                        "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).Year + "', 103)",
                        "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).Year + "', 103)",
                        
                        Session["UserId"].ToString(),
                         (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), dt.Rows[0]["F10003"].ToString(),
                         dt.Rows[0]["BazaCalculCM"].ToString().ToString(new CultureInfo("en-US")), dt.Rows[0]["ZileBazaCalculCM"].ToString(), dt.Rows[0]["MedieZilnicaCM"].ToString().ToString(new CultureInfo("en-US")), dt.Rows[0]["MedieZileBazaCalcul"].ToString().ToString(new CultureInfo("en-US")),
                          dt.Rows[0]["SerieCM"].ToString(), dt.Rows[0]["NumarCM"].ToString(), dt.Rows[0]["CodIndemnizatie"].ToString(), dt.Rows[0]["CodUrgenta"].ToString(), dt.Rows[0]["CodInfectoContag"].ToString(), dt.Rows[0]["CodDiagnostic"].ToString(),
                          dt.Rows[0]["SerieCMInitial"].ToString(), dt.Rows[0]["NumarCMInitial"].ToString(), dt.Rows[0]["Prescris"].ToString(), dt.Rows[0]["NrAvizMedicExpert"].ToString(), dt.Rows[0]["MedicCurant"].ToString(), dt.Rows[0]["CNPCopil"].ToString(),
                          "CONVERT(DATETIME, '" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(dt.Rows[0]["DataAvizDSP"].ToString()).Year + "', 103)",
                           dt.Rows[0]["NrZile"].ToString());
                    General.ExecutaNonQuery(sql, null);

                    sql = "UPDATE CM_Cereri SET IdStare = 4 WHERE Id = " + lstIds[i];
                    General.ExecutaNonQuery(sql, null);

                }

                Session["CM_Grid"] = null;
                IncarcaGrid();

                MessageBox.Show(msg + (msg.Length > 0 ? System.Environment.NewLine : "") + (nrSel == 1 ? "S-a transferat un concediu medical!" : "S-au transferat " + nrSel + " concedii medicale!"), MessageBox.icoInfo, "");



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

                if (Session["CM_Grid"] == null)
                    dt = SelectGrid();
                else
                    dt = Session["CM_Grid"] as DataTable;

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["CM_Grid"] = dt;
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
                    string[] arr = e.Parameters.Replace(',', ';').Split(';');        

                    switch (arr[0])
                    {      
                        case "btnEdit":
                            Session["CM_Id"] = arr[1];
                            Session["CM_Stare"] = arr[2];
                            CriptDecript prc = new CriptDecript();
                            //string param = General.Nz(Request["tip"], prc.EncryptString(Constante.cheieCriptare, "Introducere", 1)).ToString();
                            //param = prc.EncryptString(Constante.cheieCriptare, param, 2);
                            //int tip = param == "Aprobare" ? 2 : 1;
                            int tip = Convert.ToInt32(General.Nz(Request["tip"], "1").ToString());
                            if (tip == 2)
                                Session["CM_Aprobare"] = 1;
                            string url = "~/ConcediiMedicale/Introducere.aspx";
                            if (Page.IsCallback)
                                ASPxWebControl.RedirectOnCallback(url);
                            else
                                Response.Redirect(url, false);
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
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["CM_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

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
                Session["CM_Grid"] = dt;
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

                if (e.VisibleIndex >= 0)
                {
                    DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;
                    DateTime dtLucru = General.DamiDataLucru();
                    if (values != null)
                    {
                        DateTime dtInceput = Convert.ToDateTime(values.Row["DataInceput"].ToString());
                        if (e.ButtonID == "btnDelete")
                        {
                            if (dtLucru.Year > dtInceput.Year || (dtLucru.Year == dtInceput.Year && dtLucru.Month > dtInceput.Month)) 
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
                DateTime dtLucru = General.DamiDataLucru();

                if (e.VisibleIndex >= 0)
                {
                    DataRowView values = grDate.GetRow(e.VisibleIndex) as DataRowView;

                    if (values != null)
                    {
                        DateTime dtInceput = Convert.ToDateTime(values.Row["DataInceput"].ToString());
                        int idStare = Convert.ToInt32(values.Row["IdStare"].ToString());

                        if ((dtLucru.Year != dtInceput.Year && dtLucru.Month !=  dtInceput.Month) || (idStare == 3 || idStare == 4))
                        {
                            if (e.ButtonType == ColumnCommandButtonType.Edit)

                                e.Visible = false;
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

        private string SelectAngajati()
        {
            string strSql = "";

            try
            {


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
                        union
                        SELECT DISTINCT A.F10003, A.F10008 + ' ' + A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament""
                        FROM ""Avs_CereriIstoric"" B
                        INNER JOIN ""Avs_Cereri"" C ON B.""Id"" = C.""Id""
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



        public DataTable SelectGrid()
        {

            DataTable q = null;

            try
            {
                string strSql = "";
                string filtru = "";


                //if (Convert.ToInt32(cmbAngFiltru.Value ?? -99) != -99) filtru += " AND a.F10003 = " + Convert.ToInt32(cmbAngFiltru.Value ?? -99);

                //daca este rol de hr aratam toate cererile
                //string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");

                CriptDecript prc = new CriptDecript();
                //string param = General.Nz(Request["tip"], prc.EncryptString(Constante.cheieCriptare, "Introducere", 1)).ToString();
                //param = prc.EncryptString(Constante.cheieCriptare, param, 2);
                //int tip = param == "Aprobare" ? 2 : 1;
                int tip = Convert.ToInt32(General.Nz(Request["tip"], "1").ToString());
                if (tip == 2)
                    strSql = "SELECT * FROM CM_Cereri";
                else
                    strSql = "SELECT * FROM CM_Cereri WHERE F10003 IN (SELECT F10003 FROM F100Supervizori WHERE IdUser = " + Session["UserId"].ToString() + ")";

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