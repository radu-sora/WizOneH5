using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;

namespace WizOne.Personal
{
    public partial class Lista : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                //grDate.SettingsPager.PageSize = 20;

                #endregion

                string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'AngajatImplicitCandidat'";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                {
                    chkCandidat.Checked = true;
                    chkCandidat.ReadOnly = true;
                }

                txtTitlu.Text = Dami.TraduCuvant((Session["Titlu"] ?? "").ToString());
                //cmbTip.DataSource = General.ListaStariAngajat();
                //cmbTip.DataBind();
                if (!IsPostBack)
                {
                    //cmbTip.SelectedIndex = 0;
                    if (Session["FiltruListaMP"] != null)
                        checkComboBoxStare.Value = Session["FiltruListaMP"].ToString();
                    else
                    {
                        checkComboBoxStare.Value = "Activ;Angajat in avans";
                        Session["FiltruListaMP"] = "Activ;Angajat in avans";
                    }
                    IncarcaGrid();
                    //btnNew.Attributes.Add("onclick", "window.open('Sablon.aspx', null,'height=300,width=500,left='+(window.outerWidth / 2 + window.screenX - 150)+', top=' + (window.outerHeight / 2 + window.screenY - 100));");
                    if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
                }
                else
                {
                    GridViewDataComboBoxColumn colSec = (grDate.Columns["Sectie"] as GridViewDataComboBoxColumn);
                    colSec.PropertiesComboBox.DataSource = General.GetF005();

                    GridViewDataComboBoxColumn colDept = (grDate.Columns["Departament"] as GridViewDataComboBoxColumn);
                    colDept.PropertiesComboBox.DataSource = General.GetF006(); 

                    grDate.DataSource = Session["InformatiaCurenta"];
                    //grDate.FilterExpression = "";
                    grDate.KeyFieldName = "Marca";
                    grDate.DataBind();

                    //if (Session["FiltruListaMP"] != null)
                    //    checkComboBoxStare.Value = Session["FiltruListaMP"].ToString();
                    //else
                    //    checkComboBoxStare.Value = null;

                }

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.Caption);
                    }
                    catch (Exception) { }
                }


            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }



        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {

                DataTable dtTmp = General.IncarcaDT($@"DECLARE
                    ret_x1 number;
                    BEGIN
                    insert into ""Admin_Limbi""(""Marca"", ""IdLimba"", ""Nivel"", ""NrAniVorbit"") VALUES(460, 2, 5, 20) returning ""IdAuto"" into ret_x1;
                    commit;
                END; ", null);

                DataTable dtAAA = General.IncarcaDT($@"
                    insert into ""Admin_Limbi""(""Marca"", ""IdLimba"", ""Nivel"", ""NrAniVorbit"") VALUES(460, 2, 5, 20) returning ""IdAuto"" into @out_1", new object[] { "string" });

                DataTable dt = General.GetPersonalRestrans(Convert.ToInt32(Session["UserId"].ToString()), checkComboBoxStare.Text, 1);
                grDate.DataSource = dt;
                grDate.FilterExpression = "";
                grDate.DataBind();
                Session["InformatiaCurenta"] = dt;
                Session["FiltruListaMP"] = checkComboBoxStare.Value.ToString();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void IncarcaGrid(bool AdaugaColoanele = true)
        {
            try
            {
                DataTable dt = new DataTable();

                //string adresa = "CASE WHEN F10084 IS NOT NULL THEN 'str. '||F10083|| ' nr. '||F10084||' bl. '||F10085|| ' ap. '||F10086||' scara '||F100892||' et. '||F100893 ELSE '' END "
                //                        + " || CASE WHEN F10082 IS NOT NULL THEN ' sector '||F1008) ELSE '' END "
                //                        + " || CASE WHEN Localitati.DENLOC IS NOT NULL THEN ' localitatea ' ||  Localitati.DENLOC ELSE '' END "
                //                        + " || CASE WHEN Judete.DENJUD IS NOT NULL THEN ' judetul ' || Judete.DENJUD ELSE '' END AS ADRESA";

                //string cond = " , Localitati, Judete, Localitati Loc_Sup WHERE F100897=Localitati.SIRUTA(+) AND Localitati.JUD=Judete.JUDETID(+) AND Localitati.SIRSUP = Loc_Sup.SIRUTA  ORDER BY F10008 || ' ' || F10009 ";

                //if (Constante.tipBD == 1)
                //{
                //    adresa = "CASE WHEN F10084 IS NOT NULL THEN 'str. '+F10083+ ' nr. '+F10084+' bl. '+F10085+ ' ap. '+LTRIM(STR(F10086))+' scara '+F100892+' et. '+F100893 ELSE '' END "
                //                        + " + CASE WHEN F10082 IS NOT NULL THEN ' sector '+F10082 ELSE '' END "
                //                        + " + CASE WHEN Localitati.DENLOC IS NOT NULL THEN ' localitatea ' +  Localitati.DENLOC ELSE '' END "
                //                        + " + CASE WHEN Judete.DENJUD IS NOT NULL THEN ' judetul ' + Judete.DENJUD ELSE '' END AS ADRESA";

                //    cond = " LEFT JOIN Localitati ON F100897 = Localitati.SIRUTA LEFT JOIN Judete ON Localitati.JUD = Judete.JUDETID "
                //                           + " LEFT JOIN Localitati Loc_Sup ON Localitati.SIRSUP = Loc_Sup.SIRUTA ORDER BY F10008 + ' ' + F10009 ";
                //}

                //if (Constante.tipBD == 1)
                //    dt = General.IncarcaDT("SELECT F10003 AS MARCA, F10017 AS CNP, F10008 + ' ' + F10009 AS NUME, CONVERT(VARCHAR, F10022, 103) AS \"DATA ANGAJARII\", " + adresa + " FROM F100 " + cond, null);
                //else
                //    dt = General.IncarcaDT("SELECT F10003 AS MARCA, F10017 AS CNP, F10008 || ' ' || F10009 AS NUME, TO_CHAR(F10022, 'dd/MM/yyyy') AS \"DATA ANGAJARII\", " + adresa + " FROM F100 " + cond, null);

                dt = General.GetPersonalRestrans(Convert.ToInt32(Session["UserId"].ToString()), checkComboBoxStare.Text, 1);


                grDate.DataSource = dt;
                grDate.KeyFieldName = "Marca";
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();                

                if (AdaugaColoanele)
                {
                    //adugam coloanele
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName == "Sectie" || col.ColumnName == "Departament")
                        {
                            GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                            c.Name = col.ColumnName;
                            c.FieldName = col.ColumnName;
                            c.Caption = Dami.TraduCuvant(col.ColumnName);
                            if (col.ColumnName == "Sectie")
                            {
                                c.PropertiesComboBox.TextField = "F00507";
                                c.PropertiesComboBox.ValueField = "F00506";
                                c.PropertiesComboBox.DataSource = General.GetF005();
                            }
                            else
                            {
                                c.PropertiesComboBox.TextField = "F00608";
                                c.PropertiesComboBox.ValueField = "F00607";
                                c.PropertiesComboBox.DataSource = General.GetF006();
                            }
                            c.ReadOnly = true;
                            grDate.Columns.Add(c);
                        }
                        else
                        {
                            GridViewDataColumn c = new GridViewDataColumn();
                            c.Name = col.ColumnName;
                            c.FieldName = col.ColumnName;
                            c.Caption = Dami.TraduCuvant(col.ColumnName);
                            c.ReadOnly = true;
                            if (col.ColumnName == "Culoare")
                                c.Visible = false;
                            grDate.Columns.Add(c);
                        }
                    }

                    DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NrRanduriPePaginaMP'", null);
                    if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                        grDate.SettingsPager.PageSize = Convert.ToInt32(dtParam.Rows[0][0].ToString());

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        //grDate.GetRowValues(e.visibleIndex, 'Marca', GoToEditMode);

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
                        //MessageBox.Show(this, "Insuficienti parametrii", MessageBox.icoError, "Eroare !");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                string url = "~/Personal/DateAngajat.aspx";                                
                                if (url != "")
                                {
                                    Session["Marca"] = arr[1];
                                    string sql = "SELECT * FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + arr[1];
                                    if (Constante.tipBD == 2)
                                        sql = General.SelectOraclePersonal(arr[1]);
                                    DataTable table = General.IncarcaDT(sql, null);
                                    DataTable table0 = General.IncarcaDT("SELECT * FROM F100 WHERE F100.F10003 = " + arr[1], null);
                                    DataTable table1 = General.IncarcaDT("SELECT * FROM F1001 WHERE F1001.F10003 = " + arr[1], null);
                                    DataSet ds = new DataSet();
                                    ds.Tables.Add(table);
                                    table0.TableName = "F100";
                                    table0.PrimaryKey = new DataColumn[] { table0.Columns["F10003"] };
                                    ds.Tables.Add(table0);
                                    table1.TableName = "F1001";
                                    table1.PrimaryKey = new DataColumn[] { table1.Columns["F10003"] };
                                    ds.Tables.Add(table1);
                                    Session["InformatiaCurentaPersonal"] = ds;



                                    //Session["Sablon_TipActiune"] = arr[0].Replace("btn", "");

                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                }
                            }
                            break;
                        case "btnSterge":
                            //Florin 2019.03.26
                            int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE F10003=@1 AND COALESCE(""Revisal"",0)=1", new object[] { arr[1] }),0));
                            if (cnt == 1)
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti sterge angajatul deoarece acesta a fost trimis in Revisal!");
                            else
                                StergeAngajat(arr[1]);

                            btnFiltru_Click(sender, e);
                            break;
                        case "btnTransf":
                            TransformaCandidatInAngajat(Convert.ToInt32(arr[1]));
                            btnFiltru_Click(sender, e);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "Stare")
                {
                    object row = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                    string culoare = row.ToString();

                    e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(culoare);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void StergeAngajat(string id)
        {
            try
            {
                //daca angajatul a fost salvat in arhiva, nu poate fi sters
                DataTable dtIstoric = General.IncarcaDT("SELECT COUNT(*) FROM F910 WHERE F91003 = " + id, null);
                if (dtIstoric != null && dtIstoric.Rows.Count > 0 && Convert.ToInt32(dtIstoric.Rows[0][0].ToString()) > 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti sterge angajatul deoarece acesta a fost salvat in arhiva!");
                    return;
                }
				
                General.ExecutaNonQuery("DELETE FROM \"relGrupAngajat2\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100Supervizori2\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = 'F100' AND \"Id\" = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100Cartele\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100Adrese\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100Contacte\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100CCTarife\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100Contracte2\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM \"F100CentreCost2\" WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM F110 WHERE F11003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM F1001 WHERE F10003 = " + id, null);

                General.ExecutaNonQuery("DELETE FROM F100 WHERE F10003 = " + id, null);

                Session["InformatiaCurenta"] = General.GetPersonalRestrans(Convert.ToInt32(Session["UserId"].ToString()), checkComboBoxStare.Text, 1);

            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.Message.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void TransformaCandidatInAngajat(int f10003)
        {
            string sql = "", data = "";

            try
            {
                if (Constante.tipBD == 1)
                    data = " CONVERT(DATETIME, '01/01/2100', 103) ";
                else
                    data = " TO_DATE('01/01/2100', 'dd/mm/yyyy') ";

                sql = "UPDATE F100 SET F10025 = 999, F100910 = {0}, F10023 = {0},  F100906 = {0}, FX1 = {0}, F100986 = {0}, F100994 = {0}, F10015 = {0}, F10051 = 1, F10027 = 1, F10046 = 999, F10024 = {0}, F10048 = 1, F10076 = {0}, ";
                sql += " F10077 = {0}, F10067 = '0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000', F1003800 = 1, F1003801 = 2, F1003802 = 3, F1003803 = 4, F1003804 = 5, F1003805 = 6, ";
                sql += " F1003806 = 7, F1003807 = 8, F1003809 = 10, F1003810 = 11, F1003811 = 12, F1003812 = 13, F1003813 = 14 WHERE F10003 = {1} ";
                sql = string.Format(sql, data, f10003);
                General.ExecutaNonQuery(sql, null);

                int idPost = -99;
                if (Constante.tipBD == 1)
                    sql = "SELECT IdPost, CONVERT(VARCHAR, DataInceput, 103) AS DataInceput FROM Org_Date WHERE F10003Candidat = {0} ";
                else
                    sql = "SELECT \"IdPost\", TO_CHAR(\"DataInceput\", 'dd/mm/yyyy') AS \"DataInceput\"  FROM \"Org_Date\" WHERE \"F10003Candidat\" = {0} ";
                sql = string.Format(sql, f10003);
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0)
                    idPost = Convert.ToInt32(dt.Rows[0]["IdPost"].ToString());

                if (idPost != -99)
                {
                    sql = "INSERT INTO \"Org_relPostAngajat\" (F10003, \"IdPost\", \"DataInceput\", \"DataSfarsit\", \"DataReferinta\", \"Stare\", TIME, USER_NO) VALUES ";
                    sql += " ({0}, {1}, {2}, {3}, {2}, 2, {4}, {5}) ";
                    sql = string.Format(sql, f10003, idPost, (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dt.Rows[0]["DataInceput"].ToString() + "', 103)" : "TO_DATE('" + dt.Rows[0]["DataInceput"].ToString() + "', 'dd/mm/yyyy')"),
                                            data, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), Session["UserId"].ToString());
                    General.ExecutaNonQuery(sql, null);
                }

            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.Message.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }


        }




        protected void popUpSablon_WindowCallback(object source, PopupWindowCallbackArgs e)
        {
            try
            {
                string param = e.Parameter;
                switch (param)
                {
                    case "Sablon":
                        Session["IdSablon"] = cmbSablon.SelectedItem.Value;
                        Session["InformatiaCurentaPersonal"] = null;
                        Session["Marca"] = null;
                        break;
                    case "Candidat":
                        Session["MP_Candidat"] = chkCandidat.Checked ? 1 : 0;
                        break;
                    case "Salvare":
                        if (Session["IdSablon"] == null)
                        {
                            popUpSablon.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat sablonul!");
                            return;
                        }
                        Session["MP_Candidat"] = chkCandidat.Checked ? 1 : 0;
                        ASPxWebControl.RedirectOnCallback("~/Personal/DateAngajat.aspx");
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

    }
}