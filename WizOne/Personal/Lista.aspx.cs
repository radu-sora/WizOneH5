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
using Oracle.ManagedDataAccess.Client;

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

                //Radu 09.12.2019             
                ASPxListBox nestedListBox = checkComboBoxStare.FindControl("listBox") as ASPxListBox;
                foreach (ListEditItem item in nestedListBox.Items)
                    item.Text = Dami.TraduCuvant(item.Text);
                ASPxButton btnInchide = checkComboBoxStare.FindControl("btnInchide") as ASPxButton;
                if (btnInchide != null)
                    btnInchide.Text = Dami.TraduCuvant("btnInchide", "Inchide");
                btnSterge.Image.ToolTip = Dami.TraduCuvant("btnSterge", "Sterge");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                btnTransf.Image.ToolTip = Dami.TraduCuvant("btnTransf", "Transforma candidat in angajat");
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
                        checkComboBoxStare.Value = Dami.TraduCuvant("Activ") + ";" + Dami.TraduCuvant("Angajat in avans");
                        Session["FiltruListaMP"] = Dami.TraduCuvant("Activ") + ";" + Dami.TraduCuvant("Angajat in avans");
                    }
                    IncarcaGrid();
                    //btnNew.Attributes.Add("onclick", "window.open('Sablon.aspx', null,'height=300,width=500,left='+(window.outerWidth / 2 + window.screenX - 150)+', top=' + (window.outerHeight / 2 + window.screenY - 100));");
                    if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);

                    if (Session["MP_Mesaj"] != null)
                    {
                        MessageBox.Show(Session["MP_Mesaj"].ToString(), MessageBox.icoInfo, "Atentie !");
                        Session["MP_Mesaj"] = null;
                    }
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

                if (Dami.ValoareParam("ValidariPersonal") == "1")
                {
                    lblText.Text = "Campurile insemnate cu gri sunt obligatoriu de completat";                    
                }


                }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }



        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                ////string strSql = "";
                //OracleConnection conn = new OracleConnection(Constante.cnnWeb);
                //conn.Open();

                ////OracleCommand cmd = new OracleCommand(strSql, conn);
                //using (OracleCommand cmd = conn.CreateCommand())
                //{
                //    //cmd.Connection = conn;
                //    //cmd.CommandText = "insert into foo values('foo','bar') returning id into :myOutputParameter";
                //    cmd.CommandText = @"insert into ""Admin_Limbi""(""Marca"", ""IdLimba"", ""Nivel"", ""NrAniVorbit"") 
                //            VALUES(460, 2, 5, 20) returning ""IdAuto"" into :out_1";
                //    //cmd.Parameters.Add("out_1", OracleDbType.Int32, ParameterDirection.ReturnValue);
                //    cmd.Parameters.Add(new OracleParameter("out_1", OracleDbType.Int32, ParameterDirection.ReturnValue));
                //    //cmd.Parameters.Add(new OracleParameter("myOutputParameter", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                //    //cmd.Parameters.Add(new OracleParameter("out_" + x.ToString(), OracleDbType.Varchar2, ParameterDirection.ReturnValue));
                //    cmd.ExecuteNonQuery(); // an INSERT is always a Non Query
                //    var ert = cmd.Parameters["out_1"].Value;
                //}

                //conn.Close();

                //DataTable dtTmp = General.IncarcaDT($@"DECLARE
                //    ret_x1 number;
                //    BEGIN
                //    insert into ""Admin_Limbi""(""Marca"", ""IdLimba"", ""Nivel"", ""NrAniVorbit"") VALUES(460, 2, 5, 20) returning ""IdAuto"" into ret_x1;
                //    commit;
                //END; ", null);

                //var dtAAA = General.DamiOracleScalar($@"
                //    insert into ""Admin_Limbi""(""Marca"", ""IdLimba"", ""Nivel"", ""NrAniVorbit"") VALUES(460, 2, 5, 20) returning ""IdAuto"" into @out_1", new object[] { "int" });

                DataTable dt = General.GetPersonalRestrans(Convert.ToInt32(Session["UserId"].ToString()), checkComboBoxStare.Text, 1);
                grDate.DataSource = dt;
                grDate.FilterExpression = "";
                grDate.DataBind();
                Session["InformatiaCurenta"] = dt;
                Session["FiltruListaMP"] = checkComboBoxStare.Value.ToString();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
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
                        dynamic c = new GridViewDataColumn();

                        if (col.ColumnName == "Sectie" || col.ColumnName == "Departament")
                        {
                            //GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                            c = new GridViewDataComboBoxColumn();
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
                            //GridViewDataColumn c = new GridViewDataColumn();
                            c = new GridViewDataColumn();
                            c.Name = col.ColumnName;
                            c.FieldName = col.ColumnName;
                            c.Caption = Dami.TraduCuvant(col.ColumnName);
                            c.ReadOnly = true;
                            if (col.ColumnName == "Culoare")
                                c.Visible = false;
                            grDate.Columns.Add(c);
                        }

                        switch(col.ColumnName.ToLower())
                        {
                            case "marca":
                                c.Width = Unit.Pixel(100);
                                break;
                            case "cnp":
                                c.Width = Unit.Pixel(130);
                                break;
                            case "numecomplet":
                                c.Width = Unit.Pixel(200);
                                break;
                            case "companie":
                                c.Width = Unit.Pixel(150);
                                break;
                            case "subcompanie":
                                c.Width = Unit.Pixel(150);
                                break;
                            case "filiala":
                                c.Width = Unit.Pixel(150);
                                break;
                            case "sectie":
                                c.Width = Unit.Pixel(150);
                                break;
                            case "departament":
                                c.Width = Unit.Pixel(350);
                                break;
                            case "dataangajarii":
                                c.Width = Unit.Pixel(100);
                                break;
                            case "stare":
                                c.Width = Unit.Pixel(100);
                                break;
                            case "adresacompleta":
                                c.Width = Unit.Pixel(800);
                                break;
                        }
                    }

                    DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NrRanduriPePaginaMP'", null);
                    if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                        grDate.SettingsPager.PageSize = Convert.ToInt32(dtParam.Rows[0][0].ToString());

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
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
                                    Session["InformatiaCurentaPersonalCalcul"] = null;
                                    Session["esteNou"] = "false";



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
                            int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE F10003=@1 AND COALESCE(""Semnat"",0)=1", new object[] { arr[1] }),0));
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
                        case "colHide":
                            grDate.Columns[arr[1]].Visible = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
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
                DataTable dtIstoric = General.IncarcaDT("SELECT COUNT(*) FROM F910 WHERE F91003 = " + id + " AND F91025 <> 900", null);
                if (dtIstoric != null && dtIstoric.Rows.Count > 0 && Convert.ToInt32(dtIstoric.Rows[0][0].ToString()) > 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu puteti sterge angajatul deoarece acesta a fost salvat in arhiva!");
                    return;
                }

                General.ExecutaNonQuery("DELETE FROM \"Avs_CereriIstoric\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Avs_Cereri\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"BP_Istoric\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"BP_Prime\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_CereriIstoric\" WHERE \"IdCerere\" IN (SELECT \"Id\" FROM \"Curs_Inregistrare\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"MP_CereriIstoric\" WHERE \"IdCerere\" IN (SELECT \"Id\" FROM \"MP_Cereri\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"MP_DirectorRUIstoric\" WHERE \"IdCerere\" IN (SELECT \"Id\" FROM \"MP_DirectorRUCereri\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateIstoric\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm1\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm2\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm3\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm4\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm5\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm6\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm7\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm8\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm9\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateForm10\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateGenerale\" WHERE \"Id\" IN (SELECT \"Id\" FROM \"Org_Date\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_CereriIstoric\" WHERE \"IdCerere\" IN (SELECT \"Id\" FROM \"Ptj_Cereri\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tmpCeasuri\" WHERE \"Cartela\" IN (SELECT \"Cartela\" FROM \"F100Cartele\" WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblInfoChiosc\" WHERE \"Cartela\" IN (SELECT \"Cartela\" FROM \"F100Cartele\" WHERE F10003 = " + id + ")", null);

                General.ExecutaNonQuery("DELETE FROM \"Admin_Activitati\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_AngajatGrup\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Atestate\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Beneficii\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Cursuri\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Documente\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Echipamente\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Evaluare\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Evolutie\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Experienta\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Limbi\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Medicina\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_NrActAd\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_NrActeAd\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Admin_Sanctiuni\" WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Atasamente\" WHERE \"IdEmpl\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Avs_Cereri\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Avs_IstoricNota\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"AvsXDec_Document\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ben_Beneficii\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ben_Facturi\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ben_tblAprobari\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM BENEFICII WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"BP_Prime\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"BP_relGrupAngajat\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM CO_HISTORY WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM CONTRACTE WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_Anterior\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_Inregistrare\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_relAngajatSkills\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_relAngajatSuper\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_tblGrupuri\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_tblGrupuriAngajati\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM D205_ANGAJATI_CORECTIE WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM D205_ANGAJATI_WS WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM DAILYCHANGES WHERE \"Marca\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"DamiCC_Table\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"DamiDataPlecare_Table\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"DamiDept_Table\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"DamiNorma_Table\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_CompetenteAngajat\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_CompetenteAngajatTemp\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_Invitatie360\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_ObiIndividuale\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_ObiIndividualeTemp\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_QuizIstoric360\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_Raspuns\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_RaspunsIstoric\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_RaspunsLinii\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_relAngajatEvaluator\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F100 WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F1001 WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F1002 WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Adrese\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Cartele2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100CCTarife\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100CentreCost2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Contacte\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Contracte2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Info\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Rating\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"F100Supervizori2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F110 WHERE F11003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F111 WHERE F11103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F112 WHERE F11203 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F113 WHERE F11303 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F200 WHERE F20003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F201 WHERE F20103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F203 WHERE F20303 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F210 WHERE F21003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F300 WHERE F30003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F300_CM_AVANS WHERE F30003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F400 WHERE F40003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F401 WHERE F40103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F501 WHERE F50103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F502 WHERE F50203 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F700 WHERE F70003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F704 WHERE F70403 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F705 WHERE F70503 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F706 WHERE F70603 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F9_D112ASIGURAT WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F9_ISTORICVENITURI WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F910 WHERE F91003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F911 WHERE F91103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F9101 WHERE F91003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F9102 WHERE F91003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F920 WHERE F92003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F921 WHERE F92103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F930 WHERE F93003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F940 WHERE F94003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM F941 WHERE F94103 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"MP_Cereri\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"MP_FluxSalarii\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_Date\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_Date\" WHERE \"F10003Candidat\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_DateGenerale\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_relPostAngajat\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_relPostAngajatMarcaAuto\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"ParoleFluturasIstoric\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM PENSIE_F WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM POZE WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_CC\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Cereri\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Cereri_import\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Cumulat\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_CumulatIstoric\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Intrari\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Intrari_import\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Intrari2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_IstoricVal\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Planificare\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_Pontaj\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_relAngajatCC\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_relAngajatProiect\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_relAngajatSuper\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tblInlocuitori\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tblZileCO\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tblZLP\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tmpCeasuri\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_tmpCeasuriPeLinie\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM REGISTRU_VECHIME WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"relAngajatInlocuitor\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"relAngajatInlocuitor\" WHERE \"F10003Inlocuitor\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"relAngajatObiective\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"relAngajatProiect\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"relGrupAngajat2\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM SPORURI WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = 'F100' AND \"Id\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"tblFluturasLog\" WHERE F10003 = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"tblTipAdresa_WebService\" WHERE \"PersContact\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM TMP_ADEVERINTA WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM TRANSPRJ WHERE MARCA = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"ZileHand\" WHERE MARCA = " + id, null);

                General.ExecutaNonQuery("UPDATE \"Avs_CereriIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Avs_CereriIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"BP_Istoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"BP_Istoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"BP_relRolUser\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"BP_tblIstoric\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Curs_CereriIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_tblFormatori\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Curs_tblTraineri\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"DynReportsUsers\" WHERE \"DynReportUserId\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Eval_QuizIstoric360\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Eval_RaspunsIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Eval_RaspunsIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Eval_RaspunsIstoric\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"GDPR_2FA\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_Cereri\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("UPDATE \"MP_Cereri\" SET \"UserIntrod\" = NULL WHERE \"UserIntrod\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_CereriIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_CereriIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_CereriIstoric\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("UPDATE \"MP_DirectorRUCereri\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("UPDATE \"MP_DirectorRUCereri\" SET \"UserIntrod\" = NULL WHERE \"UserIntrod\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_DirectorRUIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_DirectorRUIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"MP_DirectorRUIstoric\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ntf_tblDrepturiUsers\" WHERE F70102 IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Org_Date\" SET \"UserIntrod\" = NULL WHERE \"UserIntrod\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Org_Date\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("UPDATE \"Org_DateIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Org_DateIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Org_DateIstoric\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Org_relPostRol\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_relRolGrupAngajat\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Org_tblRoluri\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"ParoleUtilizatorIstoric\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"Proiecte\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Ptj_Cereri\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("UPDATE \"Ptj_Cereri\" SET \"UserIntrod\" = NULL WHERE \"UserIntrod\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Ptj_CereriIstoric\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Ptj_CereriIstoric\" SET \"IdUserInlocuitor\" = NULL WHERE \"IdUserInlocuitor\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Ptj_CereriIstoric\" SET \"Inlocuitor\" = NULL WHERE \"Inlocuitor\"  = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Ptj_IstoricBlocare\" WHERE F70102 IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("UPDATE \"Ptj_IstoricVal\" SET \"IdUser\" = NULL WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"relGrupUser2\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"relUserCentruCost\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblConfigUsers\" WHERE F70102 IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblDelegari\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblDelegari\" WHERE \"IdDelegat\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblLogSelect\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);
                General.ExecutaNonQuery("DELETE FROM \"tblProfileLinii\" WHERE \"IdUser\" IN (SELECT F70102 FROM USERS WHERE F10003 = " + id + ")", null);

                General.ExecutaNonQuery("DELETE FROM USERS WHERE F10003 = " + id, null);

                //General.ExecutaNonQuery("UPDATE USERS SET F70114 = 1 WHERE F10003 = " + id, null);
                Session["InformatiaCurenta"] = General.GetPersonalRestrans(Convert.ToInt32(Session["UserId"].ToString()), checkComboBoxStare.Text, 1);

                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes!");

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
                        Session["MP_CreareUtilizator"] = chkUser.Checked ? 1 : 0;
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