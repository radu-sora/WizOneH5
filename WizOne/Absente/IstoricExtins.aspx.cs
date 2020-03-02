using DevExpress.Web;
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
using System.Drawing;

namespace WizOne.Absente
{
    public partial class IstoricExtins : System.Web.UI.Page
    {

        Color culoare = Color.FromArgb(96, 230, 230, 230);
        Color culoareBordura = Color.FromArgb(255, 0, 0, 0);
        Color culoareCapTabel = Color.FromArgb(255, 180, 210, 240);
        Color culoareSeparator = Color.FromArgb(255, 255, 255, 255);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnLoad.Text = Dami.TraduCuvant("btnLoad", "Incarca");
                btnExport.Text = Dami.TraduCuvant("btnExport", "Export");

                lblAn.InnerText = Dami.TraduCuvant("Anul");
                lblLuna.InnerText = Dami.TraduCuvant("Luna");
                lblFil.InnerText = Dami.TraduCuvant("Filtru");

                grLeg.Caption = Dami.TraduCuvant("Legenda");

                txtTitlu.Text = General.VarSession("Titlu").ToString() + " - " + Dami.TraduCuvant("Istoric Extins");

                #endregion


                string sqlView = "select count(*) FROM sys.views where name = 'viewIstoricExtins'";
                if (Constante.tipBD == 2) sqlView = "SELECT count(*) FROM user_views WHERE view_name = 'viewIstoricExtins'";

                int existaView = Convert.ToInt32(General.ExecutaScalar(sqlView, null));

                if (!IsPostBack)
                {                    

                    lblAngajat.Text = Dami.TraduCuvant("Situatie Angajat") + " " + General.Nz(Session["IstoricExtins_Angajat_Nume"],"").ToString();
                    lblViz.InnerText = Dami.TraduCuvant("Vizualizare");
                    txtLuna.Date = DateTime.Now;

                    if (existaView == 1)
                    {
                        cmbViz.Items.Add(Dami.TraduCuvant("concedii de odihna"), 1);
                        cmbViz.Items.Add(Dami.TraduCuvant("generala concedii"), 2);
                        cmbViz.Items.Add(Dami.TraduCuvant("anuala"), 3);
                        cmbViz.Items.Add(Dami.TraduCuvant("lunara"), 4);
                        cmbViz.SelectedIndex = 0;
                    }
                    else
                    {
                        cmbViz.Items.Add(Dami.TraduCuvant("concedii de odihna"), 1);
                        cmbViz.Items.Add(Dami.TraduCuvant("anuala"), 3);
                        cmbViz.Items.Add(Dami.TraduCuvant("lunara"), 4);
                        cmbViz.SelectedIndex = 0;
                    }

                    cmbFil.Items.Add(Dami.TraduCuvant("acelasi departament"), 3);
                    cmbFil.Items.Add(Dami.TraduCuvant("aceeasi sectie"), 2);
                    cmbFil.Items.Add(Dami.TraduCuvant("aceeasi filiala"), 1);
                    cmbFil.Items.Add(Dami.TraduCuvant("toate inregistrarile"), 0);
                    cmbFil.SelectedIndex = 0;

                    cmbAn.DataSource = Dami.ListaAni(2015, 2030);
                    cmbAn.DataBind();
                    cmbAn.Value = DateTime.Now.Year;

                    //Adaugam ultimele zile din luna
                    SetColoane();
                    IncarcaGrid(5);
                    IncarcaGrid(1);
                    if (existaView == 1) IncarcaGrid(2);
                    IncarcaGrid(3);
                }
                else
                {
                    IncarcaGrid(1);
                    if (existaView == 1) IncarcaGrid(2);
                    IncarcaGrid(3);
                    IncarcaGrid(5);

                    if (Convert.ToInt32(cmbViz.Value) == 4) grLunar.DataSource = Session["IstoricExtins_DataSource"];
                }

                TraduGrid(grDate);
                TraduGrid(grLunar);
                TraduGrid(grView);
                TraduGrid(grLeg);

                grLunar.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaIstoricExtins", "10"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(Convert.ToInt32(cmbViz.Value));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                CreazaExcelIstoricExtins(Convert.ToInt32(Session["UserId"] ?? 99), Convert.ToInt32(Session["User_Marca"] ?? 99), txtLuna.Date.Year, txtLuna.Date.Month, (int)cmbFil.Value, Convert.ToInt32(Session["IstoricExtins_Angajat_Marca"] ?? -99));

                //if (grLunar.Selection.Count == 0)
                //{
                //    ExportGrid.ExportedRowType = GridViewExportedRowType.All;
                //}
                //ExportGrid.WriteXlsxToResponse();
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
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                if (Session["IstoricExtins_VineDin"].ToString() == "1")
                    Response.Redirect("~/Absente/Lista.aspx", false);
                else
                {
                    Response.Redirect("~/Absente/Cereri.aspx", false);
                    Session["IstoricExtins_VineDin"] += "-OK";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private DataTable GetIstoricExtinsLunar(int idUser, int f10003, int an, int luna, int structura, int f10003Selectat, bool gradient = true)
        {
            //structura
            //0 - toate inregristrarile
            //1 - aceeasi filiala
            //2 - aceeasi sectie
            //3 - acelasi departament

            DataTable dt = new DataTable();

            try
            {
                string strSql = "";
                string strZile = "";
                string strZilePivot = "";
                string strZi = "";
                string strZileAs = "";
                string culoareGradient = "c.culoare";

                string ziInc = General.ToDataUniv(an, luna, 1);
                string ziSf = General.ToDataUniv(an, luna, 99);

                string filtru = "";
                switch (structura)
                {
                    case 0:
                        filtru = "";
                        break;
                    case 1:
                        filtru = " AND x.F10005 = (SELECT F10005 FROM F100 WHERE F10003=" + f10003Selectat + ")";
                        break;
                    case 2:
                        filtru = " AND x.F10006 = (SELECT F10006 FROM F100 WHERE F10003=" + f10003Selectat + ")";
                        break;
                    case 3:
                        filtru = " AND x.F10007 = (SELECT F10007 FROM F100 WHERE F10003=" + f10003Selectat + ")";
                        break;
                }

                if (gradient) culoareGradient = "'repeating-linear-gradient(45deg, #000000, #000000 1px,' + c.Culoare + ' 1px, ' + c.culoare + ' 5px)'";
                //Adaugam ultimele zile din luna (trebuie pus in load si aici pt eventualitatea in care alege alta luna)
                SetColoane();
                //for (int i = 29; i <= 31; i++)
                //{
                //    if (i <= DateTime.DaysInMonth(txtLuna.Date.Year, txtLuna.Date.Month))
                //    {
                //        GridViewDataColorEditColumn col = grLunar.Columns[i.ToString()] as GridViewDataColorEditColumn;
                //        col.FieldName = i.ToString();
                //        //grLunar.Columns[i.ToString()].field
                //        //if (grLunar.Columns[i.ToString()] == null) AdaugaColoana(i);
                //    }
                //    else
                //    {
                //        GridViewDataColorEditColumn col = grLunar.Columns[i.ToString()] as GridViewDataColorEditColumn;
                //        col.FieldName = "";
                //        //if (grLunar.Columns[i.ToString()] != null) grLunar.Columns.Remove(grLunar.Columns[i.ToString()]);
                //    }
                //}



                if (Constante.tipBD == 1)
                {
                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                    {
                        strZi = an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0"));
                        strZile += "UNION SELECT '" + strZi + "' AS Ziua ";
                        strZilePivot += ", [" + strZi + "]";
                        //strZileAs += ", [" + strZi + "] AS Ziua" + i.ToString();
                        strZileAs += ", [" + strZi + "] AS \"" + i.ToString() + "\"";
                    }

                    strSql = "SELECT F10003 AS Marca, NumeComplet AS Angajat " + strZileAs +
                            " FROM " +
                            " ( " +
                            " SELECT x.F10003, x.F10008 + ' ' + x.F10009 as NumeComplet, y.Zi, y.Culoare FROM F100 x LEFT JOIN ( " +
                            " SELECT y.Ziua as Zi, x.F10003, x.F10008 + ' ' + x.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs,  " +
                            " CASE WHEN c.Culoare IS NOT NULL THEN c.Culoare ELSE CASE WHEN (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY=y.Ziua) = 1 THEN (CASE WHEN COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareZileLibereLegale'),'#FA8282') = '' THEN '#FA8282' ELSE COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareZileLibereLegale'),'#FA8282') END) ELSE CASE WHEN datepart(dw,y.Ziua) in (1,7) THEN (CASE WHEN COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareSambataSiDuminica'),'#FF0000') = '' THEN '#FF0000' ELSE COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareSambataSiDuminica'),'#FF0000') END) END END END AS Culoare " +
                            " FROM F100 x " +
                            " INNER JOIN (" + strZile.Substring(6) + ") y  on 1=1 " +
                            " LEFT JOIN Ptj_Intrari a on a.F10003 = x.F10003 AND a.Ziua = y.Ziua " +
                            " LEFT JOIN Ptj_tblAbsente c on a.ValStr = c.DenumireScurta " +
                            " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                            " UNION " +
                            " SELECT x.Ziua as Zi, a.F10003, b.F10008 + ' ' + b.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs, (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\"=4) AS Culoare " +
                            " FROM (" + strZile.Substring(6) + ") x " +
                            " INNER JOIN Ptj_Cereri a on a.DataInceput <= x.Ziua and x.ziua <= a.DataSfarsit " +
                            " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                            " INNER JOIN Ptj_tblAbsente c on a.IdAbsenta = c.Id " +
                            " WHERE a.IdStare=4 AND ((MONTH(a.DataInceput)=" + luna + " and YEAR(a.DataInceput)=" + an + ") OR (MONTH(a.DataSfarsit)=" + luna + " and YEAR(a.DataSfarsit)=" + an + ")) " +
                            " AND a.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                            " UNION " +
                            " SELECT x.Ziua as Zi, a.F10003, b.F10008 + ' ' + b.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs, " + culoareGradient + " AS Culoare " +
                            " FROM (" + strZile.Substring(6) + ") x " +
                            " INNER JOIN Ptj_Cereri a on a.DataInceput <= x.Ziua and x.ziua <= a.DataSfarsit " +
                            " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                            " INNER JOIN Ptj_tblAbsente c on a.IdAbsenta = c.Id " +
                            " WHERE a.IdStare IN (1,2) AND ((MONTH(a.DataInceput)=" + luna + " and YEAR(a.DataInceput)=" + an + ") OR (MONTH(a.DataSfarsit)=" + luna + " and YEAR(a.DataSfarsit)=" + an + ")) " +
                            " AND a.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                            " ) y on x.F10003=y.F10003 " +
                            " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                            " AND X.F10022 <= " + ziSf + " AND " + ziInc + " <= X.F10023 " +
                            filtru +
                            " ) AS source " +
                            " PIVOT " +
                            " ( " +
                            "     MAX(Culoare) " +
                            "     FOR Zi IN ( " + strZilePivot.Substring(2) + ") " +
                            " ) AS pvt " +
                            " ORDER BY NumeComplet";


                    //strSql = "SELECT F10003 AS Marca, NumeComplet AS Angajat, InCurs " + strZileAs +
                    //        " FROM " +
                    //        " ( " +
                    //        " SELECT x.F10003, x.F10008 + ' ' + x.F10009 as NumeComplet, y.Zi, y.Culoare, y.InCurs FROM F100 x LEFT JOIN ( " +
                    //        " SELECT y.Ziua as Zi, x.F10003, x.F10008 + ' ' + x.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs,  " +
                    //        " CASE WHEN c.Culoare IS NOT NULL THEN c.Culoare ELSE CASE WHEN (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY=y.Ziua) = 1 THEN (CASE WHEN COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareZileLibereLegale'),'#FA8282') = '' THEN '#FA8282' ELSE COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareZileLibereLegale'),'#FA8282') END) ELSE CASE WHEN datepart(dw,y.Ziua) in (1,7) THEN (CASE WHEN COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareSambataSiDuminica'),'#FF0000') = '' THEN '#FF0000' ELSE COALESCE((SELECT Valoare FROM tblParametrii WHERE Nume='CuloareSambataSiDuminica'),'#FF0000') END) END END END AS Culoare, 0 AS \"InCurs\" " +    
                    //        " FROM F100 x " +
                    //        " INNER JOIN (" + strZile.Substring(6) + ") y  on 1=1 " +
                    //        " LEFT JOIN Ptj_Intrari a on a.F10003 = x.F10003 AND a.Ziua = y.Ziua " +
                    //        " LEFT JOIN Ptj_tblAbsente c on a.ValStr = c.DenumireScurta " +
                    //        " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                    //        " UNION " +
                    //        " SELECT x.Ziua as Zi, a.F10003, b.F10008 + ' ' + b.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs, (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\"=4) AS Culoare, 0 AS \"InCurs\" " +
                    //        " FROM (" + strZile.Substring(6) + ") x " +
                    //        " INNER JOIN Ptj_Cereri a on a.DataInceput <= x.Ziua and x.ziua <= a.DataSfarsit " +
                    //        " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                    //        " INNER JOIN Ptj_tblAbsente c on a.IdAbsenta = c.Id " +
                    //        " WHERE a.IdStare=4 AND ((MONTH(a.DataInceput)=" + luna + " and YEAR(a.DataInceput)=" + an + ") OR (MONTH(a.DataSfarsit)=" + luna + " and YEAR(a.DataSfarsit)=" + an + ")) " +
                    //        " AND a.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                    //        " UNION " +
                    //        " SELECT x.Ziua as Zi, a.F10003, b.F10008 + ' ' + b.F10009 as NumeComplet, c.Id as IdAbs, c.Denumire as DenAbs, COALESCE(c.Culoare,'#FFFFFF') AS Culoare, 1 AS \"InCurs\" " +
                    //        " FROM (" + strZile.Substring(6) + ") x " +
                    //        " INNER JOIN Ptj_Cereri a on a.DataInceput <= x.Ziua and x.ziua <= a.DataSfarsit " +
                    //        " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                    //        " INNER JOIN Ptj_tblAbsente c on a.IdAbsenta = c.Id " +
                    //        " WHERE a.IdStare IN (1,2) AND ((MONTH(a.DataInceput)=" + luna + " and YEAR(a.DataInceput)=" + an + ") OR (MONTH(a.DataSfarsit)=" + luna + " and YEAR(a.DataSfarsit)=" + an + ")) " +
                    //        " AND a.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                    //        " ) y on x.F10003=y.F10003 " +
                    //        " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM F100Supervizori WHERE IdUser=" + idUser + " UNION SELECT " + f10003 + ") " +
                    //        " AND X.F10022 <= " + ziSf + " AND " + ziInc + " <= X.F10023 " +
                    //        filtru +
                    //        " ) AS source " +
                    //        " PIVOT " +
                    //        " ( " +
                    //        "     MAX(Culoare) " +
                    //        "     FOR Zi IN ( " + strZilePivot.Substring(2) + ") " +
                    //        " ) AS pvt " +
                    //        " ORDER BY NumeComplet";
                }
                else
                {
                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                    {
                        strZi = General.ToDataUniv(an, luna, i);
                        strZile += "UNION SELECT " + strZi + " AS \"Ziua\" FROM Dual ";
                        //strZileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";
                        strZileAs += ", " + strZi + " AS \"" + i.ToString() + "\"";
                    }

                    strSql = "SELECT * " +
                            " FROM " +
                            " ( " +
                            " SELECT x.F10003 AS \"Marca\", x.F10008 || ' ' || x.F10009 as \"NumeComplet\", y.\"Zi\", y.\"Culoare\" FROM F100 x LEFT JOIN ( " +
                            " SELECT a.\"Ziua\" as \"Zi\", x.F10003, x.F10008 || ' ' || x.F10009 as \"NumeComplet\", c.\"Id\" as \"IdAbs\", c.\"Denumire\" as \"DenAbs\", " +
                            " CASE WHEN c.\"Culoare\" IS NOT NULL THEN c.\"Culoare\" ELSE CASE WHEN (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY=y.\"Ziua\") = 1 THEN (CASE WHEN COALESCE((SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\"='CuloareZileLibereLegale'),'#FA8282') = '' THEN '#FA8282' ELSE COALESCE((SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\"='CuloareZileLibereLegale'),'#FA8282') END) ELSE CASE WHEN (1 + TRUNC (Y.\"Ziua\") - TRUNC (Y.\"Ziua\", 'IW')) in (6,7) THEN (CASE WHEN COALESCE((SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\"='CuloareSambataSiDuminica'),'#FF0000') = '' THEN '#FF0000' ELSE COALESCE((SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\"='CuloareSambataSiDuminica'),'#FF0000') END) END END END AS \"Culoare\" " +   
                            " FROM F100 x " +
                            " INNER JOIN (" + strZile.Substring(6) + ") Y ON 1=1 " +
                            " LEFT JOIN \"Ptj_Intrari\" a on x.F10003 = a.F10003 AND a.\"Ziua\" = y.\"Ziua\" " +
                            " LEFT JOIN \"Ptj_tblAbsente\" c on a.\"ValStr\" = c.\"DenumireScurta\" " +
                            " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM \"F100Supervizori\" WHERE \"IdUser\"=" + idUser + " UNION SELECT " + f10003 + " FROM Dual) " +
                            " UNION " +
                            " SELECT x.\"Ziua\" as \"Zi\", a.F10003, b.F10008 || ' ' || b.F10009 as \"NumeComplet\", c.\"Id\" as \"IdAbs\", c.\"Denumire\" as \"DenAbs\", (SELECT \"Culoare\" FROM \"Ptj_tblStari\" WHERE \"Id\"=4) AS \"Culoare\" " +
                            " FROM (" + strZile.Substring(6) + ") x " +
                            " INNER JOIN \"Ptj_Cereri\" a on a.\"DataInceput\" <= x.\"Ziua\" and x.\"Ziua\" <= a.\"DataSfarsit\" " +
                            " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                            " INNER JOIN \"Ptj_tblAbsente\" c on a.\"IdAbsenta\" = c.\"Id\" " +
                            " WHERE a.\"IdStare\"=4 AND ((to_char(a.\"DataInceput\",'mm')=" + luna + " and to_char(a.\"DataInceput\",'yyyy')=" + an + ") OR (to_char(a.\"DataSfarsit\",'mm')=" + luna + " and to_char(a.\"DataSfarsit\",'yyyy')=" + an + ")) " +
                            " AND a.F10003 IN (SELECT DISTINCT F10003 FROM \"F100Supervizori\" WHERE \"IdUser\"=" + idUser + " UNION SELECT " + f10003 + " FROM Dual) " +
                            " UNION " +
                            " SELECT x.\"Ziua\" as \"Zi\", a.F10003, b.F10008 || ' ' || b.F10009 as \"NumeComplet\", c.\"Id\" as \"IdAbs\", c.\"Denumire\" as \"DenAbs\", COALESCE(c.\"Culoare\",'#FFFFFF') AS \"Culoare\" " +
                            " FROM (" + strZile.Substring(6) + ") x " +
                            " INNER JOIN \"Ptj_Cereri\" a on a.\"DataInceput\" <= x.\"Ziua\" and x.\"Ziua\" <= a.\"DataSfarsit\" " +
                            " INNER JOIN F100 b on a.F10003 = b.F10003 " +
                            " INNER JOIN \"Ptj_tblAbsente\" c on a.\"IdAbsenta\" = c.\"Id\" " +
                            " WHERE a.\"IdStare\" IN (1,2) AND ((to_char(a.\"DataInceput\",'mm')=" + luna + " and to_char(a.\"DataInceput\",'yyyy')=" + an + ") OR (to_char(a.\"DataSfarsit\",'mm')=" + luna + " and to_char(a.\"DataSfarsit\",'yyyy')=" + an + ")) " +
                            " AND a.F10003 IN (SELECT DISTINCT F10003 FROM \"F100Supervizori\" WHERE \"IdUser\"=" + idUser + " UNION SELECT " + f10003 + " FROM Dual) " +
                            " ) y on x.F10003=y.F10003 " +
                            " WHERE x.F10003 IN (SELECT DISTINCT F10003 FROM \"F100Supervizori\" WHERE \"IdUser\"=" + idUser + " UNION SELECT " + f10003 + " FROM Dual) " +
                            " AND X.F10022 <= " + ziSf + " AND " + ziInc + " <= X.F10023 " +
                            filtru +
                            " ) " +
                            " PIVOT " +
                            " ( " +
                            "     MAX(\"Culoare\") " +
                            "     FOR \"Zi\" IN ( " + strZileAs.Substring(2) + ") " +
                            " ) " +
                            " ORDER BY \"NumeComplet\"";
                }

                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;

        }

        //protected void cmbViz_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        switch((int)General.Nz(cmbViz.Value,1))
        //        {
        //            case 1:
        //                grDate.Visible = true;
        //                grView.Visible = false;
        //                grAnual.Style["Display"] = "none";
        //                grLunar.Visible = false;
        //                break;
        //            case 2:
        //                grDate.Visible = false;
        //                grView.Visible = true;
        //                grAnual.Style["Display"] = "none";
        //                grLunar.Visible = false;
        //                break;
        //            case 3:
        //                grDate.Visible = false;
        //                grView.Visible = false;
        //                grAnual.Style["Display"] = "inline";
        //                grLunar.Visible = false;
        //                break;
        //            case 4:
        //                grDate.Visible = false;
        //                grView.Visible = false;
        //                grAnual.Style["Display"] = "none";
        //                grLunar.Visible = true;
        //                break;
        //        }

        //        DataTable dt = new DataTable();
        //        grDate.DataSource = dt;
        //        grDate.DataBind();

        //        grView.DataSource = dt;
        //        grView.DataBind();

        //        grAnual.Rows.Clear();

        //        grLunar.DataSource = dt;
        //        grLunar.DataBind();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        private void CreazaGridAnual(int an, bool primaData = true)
        {
            try
            {
                //ASPxGridView grLeg = (ASPxGridView)pnlLeg.FindControl("grLegenda");
                List<object> lst = grLeg.GetSelectedFieldValues("Id");

                //DataTable lstZlb = GetZileLibere(Convert.ToInt32(Session["UserId"] ?? 99), new DateTime(DateTime.Now.Year, 1, 1), new DateTime(DateTime.Now.Year, 12, 31), Convert.ToInt32(Session["IstoricExtins_Angajat_Marca"] ?? -99));
                DataTable lstZlb = GetZileLibere(Convert.ToInt32(Session["UserId"] ?? 99), new DateTime(an, 1, 1), new DateTime(an, 12, 31), Convert.ToInt32(Session["IstoricExtins_Angajat_Marca"] ?? -99));


                grAnual.Rows.Clear();
                //int an = DateTime.Now.Year;

                //prima linie (cea a zilelor saptamanii) este fixa
                grAnual.Rows.Add(new HtmlTableRow());
                //prima coloana (cea a lunilor) este fixa
                grAnual.Rows[0].Cells.Add(new HtmlTableCell() { });

                for (int i = 1; i <= 12; i++)
                {
                    grAnual.Rows.Add(new HtmlTableRow());
                    grAnual.Rows[i].Cells.Add(new HtmlTableCell() { });

                    HtmlGenericControl lbl = new HtmlGenericControl("div");
                    lbl.InnerText = Dami.TraduCuvant(Dami.NumeLuna(i));
                    //lbl.Attributes["class"] = "IstEx_Luna";
                    lbl.Style.Add("background-color", "#b4d2f0");
                    lbl.Style.Add("color", "#000000");
                    lbl.Style.Add("width", "100px");
                    lbl.Style.Add("height", "22px");
                    lbl.Style.Add("vertical-align", "middle");
                    lbl.Style.Add("line-height", "22px");
                    lbl.Style.Add("padding-left", "8px");
                    grAnual.Rows[i].Cells[0].Controls.Add(lbl);

                    int nrZileLuna = DateTime.DaysInMonth(an, i);
                    DateTime dt = new DateTime(an, i, 1);
                    int nrZileSup = ziSapNume(dt.DayOfWeek.ToString());

                    for (int j = 1; j <= (nrZileLuna + nrZileSup - 1); j++)
                    {
                        grAnual.Rows[i].Cells.Add(new HtmlTableCell() { });

                        if (grAnual.Rows[0].Cells.Count <= j)
                        {
                            grAnual.Rows[0].Cells.Add(new HtmlTableCell() { });

                            HtmlGenericControl lblZi = new HtmlGenericControl("div");
                            lblZi.InnerText = Dami.NumeZi(((j % 7) == 0 ? 7 : (j % 7)), 2, General.Nz(Session["IdLimba"], "RO").ToString());
                            //lblZi.Attributes["class"] = "IstEx_NumeZi";
                            lblZi.Style.Add("background-color", "#b4d2f0");
                            lblZi.Style.Add("color", "#000000");
                            lblZi.Style.Add("width", "24px");
                            lblZi.Style.Add("height", "22px");
                            lblZi.Style.Add("vertical-align", "middle");
                            lblZi.Style.Add("line-height", "22px");
                            lblZi.Style.Add("text-align", "center");
                            grAnual.Rows[0].Cells[j].Controls.Add(lblZi);
                        }

                        if (nrZileSup <= j)
                        {
                            HtmlGenericControl zi = new HtmlGenericControl("div");
                            zi.InnerText = (j - nrZileSup + 1).ToString();
                            //zi.Attributes["class"] = "IstEx_Zi";
                            zi.Style.Add("color", "#000000");
                            zi.Style.Add("width", "22px");
                            zi.Style.Add("height", "22px");
                            zi.Style.Add("vertical-align", "middle");
                            zi.Style.Add("line-height", "22px");
                            zi.Style.Add("text-align", "center");
                            zi.Style.Add("border", "solid 1px #000000");
                            zi.Style.Add("border-radius", "4px");

                            if (lstZlb.Rows.Count > 0)
                            {
                                //string filtru = "Zi = " + General.ToDataUniv(an, i, j - nrZileSup + 1);
                                string filtru = "Zi = #" + an + "-" + i + "-" + (j - nrZileSup + 1).ToString() + "#";
                                DataRow[] ent = lstZlb.Select(filtru);
                                if (ent != null && ent.Count() > 0 && ent.FirstOrDefault() != null)
                                {
                                    object idAbs = ent[0]["AbsId"];
                                    var este = lst.Find(p => p.Equals(idAbs));
                                    if (este != null)
                                    {
                                        if (ent[0]["InCurs"].ToString() == "1")
                                        {
                                            zi.Style.Add("background", "repeating-linear-gradient(45deg, #000000, #000000 1px, " + ent[0]["Culoare"].ToString() + " 1px, " + ent[0]["Culoare"].ToString() + " 5px)");
                                            zi.Attributes["class"] = "crs_" + idAbs.ToString();
                                        }
                                        else
                                        {
                                            zi.Style.Add("background-color", ent[0]["Culoare"].ToString());
                                            zi.Attributes["class"] = "tag_" + idAbs.ToString();
                                        }
                                    }
                                }
                            }

                            grAnual.Rows[i].Cells[j].Controls.Add(zi);
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


        private int ziSapNume(string ziNume)
        {
            try
            {
                int nrZi = 1;

                switch (ziNume.Trim().ToLower())
                {
                    case "monday":
                        nrZi = 1;
                        break;
                    case "tuesday":
                        nrZi = 2;
                        break;
                    case "wednesday":
                        nrZi = 3;
                        break;
                    case "thursday":
                        nrZi = 4;
                        break;
                    case "friday":
                        nrZi = 5;
                        break;
                    case "saturday":
                        nrZi = 6;
                        break;
                    case "sunday":
                        nrZi = 7;
                        break;
                    default:
                        nrZi = 1;
                        break;
                }

                return nrZi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

                return 1;
            }
        }


        private DataTable GetZileLibere(int idUser, DateTime dtInc, DateTime dtSf, int f10003)
        {
            DataTable dt = new DataTable();

            try
            {
                string strSQl = "";

                if (f10003 == -1)                   //toti
                {
                    #region aaa
                    //var ids = from a in GetAngajati(idUser) select a.F10003;

                    //var cereri = from a in this.ObjectContext.Ptj_Cereri
                    //             join b in this.ObjectContext.Ptj_tblAbsente on a.IdAbsenta equals b.Id
                    //             where a.IdStare == 4 && EntityFunctions.DiffDays(dtInc, a.DataSfarsit) >= 0 && EntityFunctions.DiffDays(a.DataInceput, dtSf) >= 0 && ids.Contains((decimal)a.F10003)
                    //             select new
                    //             {
                    //                 DataInceput = a.DataInceput < dtInc ? dtInc : a.DataInceput,
                    //                 DataSfarsit = a.DataSfarsit <= dtSf ? a.DataSfarsit : dtSf,
                    //                 Culoare = culoare,
                    //                 denAbs = b.DenumireIstoricExtins == null ? "" : b.DenumireIstoricExtins + " Planificat",
                    //                 idAbs = a.IdAbsenta
                    //             };

                    //ptj = from a in this.ObjectContext.Ptj_Intrari
                    //      join b in this.ObjectContext.Ptj_tblAbsente on a.ValStr equals b.DenumireScurta
                    //      join c in this.ObjectContext.F100 on a.F10003 equals c.F10003
                    //      where EntityFunctions.DiffDays(dtInc, a.Ziua) >= 0 && EntityFunctions.DiffDays(a.Ziua, dtSf) >= 0 && ids.Contains((decimal)a.F10003) && a.ValStr != null
                    //      select new metaZileLibereIstoricExtins
                    //      {
                    //          F10003 = a.F10003,
                    //          NumeComplet = c.F10008 + " " + c.F10009,
                    //          Zi = EntityFunctions.TruncateTime((DateTime)a.Ziua),
                    //          Culoare = b.Culoare,
                    //          denAbs = b.DenumireIstoricExtins == null ? "" : b.DenumireIstoricExtins,
                    //          idAbs = (int)b.Id
                    //      };


                    //q = (from a in cereri.AsEnumerable()
                    //     from d in (Enumerable.Range(0, 1 + ((DateTime)a.DataSfarsit).Subtract((DateTime)a.DataInceput).Days).Select(p => ((DateTime)a.DataInceput).AddDays(p)))
                    //     select new metaZileLibereIstoricExtins { Zi = d, Culoare = a.Culoare == null ? "#FFFFFFFF" : a.Culoare, denAbs = a.denAbs, idAbs = (int)a.idAbs })
                    //    .Union
                    //    (from d in (Enumerable.Range(0, dtSf.Subtract(dtInc).Days).Select(p => dtInc.AddDays(p)))
                    //     where d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday
                    //     select new metaZileLibereIstoricExtins { F10003 = -1, NumeComplet = "", Zi = d, Culoare = "#FFFF0000", denAbs = "Sambata sau Duminica", idAbs = -1 })
                    //    .Union
                    //    (from a in this.ObjectContext.HOLIDAYS
                    //     where EntityFunctions.DiffDays(dtInc, a.DAY) >= 0 && EntityFunctions.DiffDays(a.DAY, dtSf) >= 0
                    //     select new metaZileLibereIstoricExtins { F10003 = -1, NumeComplet = "", Zi = EntityFunctions.TruncateTime((DateTime)a.DAY), Culoare = "#FFFA8282", denAbs = "Zi libera legala", idAbs = -2 })
                    //     .Union
                    //     (from a in ptj.AsEnumerable() select a);
#endregion

                }
                else
                {
                    strSQl = $@"SELECT COALESCE(X.""Zi"",{General.ToDataUniv(new DateTime(1900, 1, 1))}) AS ""Zi"", CASE WHEN COALESCE((SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = 4),'#EE8D3D') = '' THEN '#EE8D3D' ELSE COALESCE((SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = 4),'#EE8D3D') END AS ""Culoare"", '{Dami.TraduCuvant("CO Planificat")}' AS ""AbsDen"", -3 AS ""AbsId"", 0 AS ""InCurs""
                                FROM ""tblZile"" X
                                INNER JOIN ""Ptj_Cereri"" A ON A.""DataInceput"" <= X.""Zi"" AND X.""Zi"" <= A.""DataSfarsit""
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.""IdStare"" = 4 AND {General.ToDataUniv(dtInc)} <= A.""DataSfarsit"" AND A.""DataInceput"" <= {General.ToDataUniv(dtSf)}
                                AND {General.ToDataUniv(dtInc)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(dtSf)} AND A.F10003 = {f10003}
                                UNION
                                SELECT X.""Zi"", CASE WHEN COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareSambataSiDuminica'),'#FF0000') = '' THEN '#FF0000' ELSE COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareSambataSiDuminica'),'#FF0000') END, '{Dami.TraduCuvant("Sambata sau Duminica")}', -1, 0 AS ""InCurs""
                                FROM ""tblZile"" X
                                WHERE {General.ToDataUniv(dtInc)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(dtSf)} AND ""ZiSapt"" IN (6, 7)
                                UNION
                                SELECT X.DAY, CASE WHEN COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareZileLibereLegale'),'#FA8282') = '' THEN '#FA8282' ELSE COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareZileLibereLegale'),'#FA8282') END, '{Dami.TraduCuvant("Zi libera legala")}', -2, 0 AS ""InCurs""
                                FROM HOLIDAYS X
                                WHERE {General.ToDataUniv(dtInc)} <= X.DAY AND X.DAY <= {General.ToDataUniv(dtSf)}
                                UNION
                                SELECT COALESCE(A.""Ziua"", {General.ToDataUniv(new DateTime(1900, 1, 1))}) AS ""Zi"", COALESCE(B.""Culoare"",'#FFFFFF') AS ""Culoare"", COALESCE(B.""DenumireIstoricExtins"", '') AS ""AbsDen"", COALESCE(B.""Id"", -99) AS ""AbsId"", 0 AS ""InCurs""
                                FROM ""Ptj_Intrari"" A
                                LEFT JOIN ""Ptj_tblAbsente"" B ON A.""ValStr"" = B.""DenumireScurta""
                                WHERE A.F10003 = {f10003} AND {General.ToDataUniv(dtInc)} <= A.""Ziua"" AND A.""Ziua"" <= {General.ToDataUniv(dtSf)} AND A.""ValStr"" IS NOT NULL
                                UNION                                
                                SELECT COALESCE(X.""Zi"",{General.ToDataUniv(new DateTime(1900, 1, 1))}) AS ""Zi"", COALESCE(B.""Culoare"",'#FFFFFF') AS ""Culoare"", COALESCE(B.""DenumireIstoricExtins"", '') AS ""AbsDen"", COALESCE(B.""Id"", -99) AS ""AbsId"", 1 AS ""InCurs""
                                FROM ""tblZile"" X
                                INNER JOIN ""Ptj_Cereri"" A ON A.""DataInceput"" <= X.""Zi"" AND X.""Zi"" <= A.""DataSfarsit""
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.""IdStare"" IN (1,2) AND {General.ToDataUniv(dtInc)} <= A.""DataSfarsit"" AND A.""DataInceput"" <= {General.ToDataUniv(dtSf)}
                                AND {General.ToDataUniv(dtInc)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(dtSf)} AND A.F10003 = {f10003} AND X.""ZiSapt"" NOT IN (6,7)";

                    #region bbb

                    //var cereri = from a in this.ObjectContext.Ptj_Cereri
                    //             join b in this.ObjectContext.Ptj_tblAbsente on a.IdAbsenta equals b.Id
                    //             where a.IdStare == 4 && EntityFunctions.DiffDays(dtInc, a.DataSfarsit) >= 0 && EntityFunctions.DiffDays(a.DataInceput, dtSf) >= 0 && a.F10003 == F10003
                    //             select new
                    //             {
                    //                 DataInceput = a.DataInceput < dtInc ? dtInc : a.DataInceput,
                    //                 DataSfarsit = a.DataSfarsit <= dtSf ? a.DataSfarsit : dtSf,
                    //                 Culoare = culoare,
                    //                 denAbs = b.DenumireIstoricExtins == null ? "" : b.DenumireIstoricExtins + " Planificat",
                    //                 idAbs = a.IdAbsenta
                    //             };

                    //ptj = from a in this.ObjectContext.Ptj_Intrari
                    //      join b in this.ObjectContext.Ptj_tblAbsente on a.ValStr equals b.DenumireScurta
                    //      where EntityFunctions.DiffDays(dtInc, a.Ziua) >= 0 && EntityFunctions.DiffDays(a.Ziua, dtSf) >= 0 && a.F10003 == F10003 && a.ValStr != null
                    //      select new metaZileLibereIstoricExtins
                    //      {
                    //          Zi = EntityFunctions.TruncateTime((DateTime)a.Ziua),
                    //          Culoare = b.Culoare,
                    //          denAbs = b.DenumireIstoricExtins == null ? "" : b.DenumireIstoricExtins,
                    //          idAbs = (int)b.Id
                    //      };


                    //q = (from a in cereri.AsEnumerable()
                    //     from d in (Enumerable.Range(0, 1 + ((DateTime)a.DataSfarsit).Subtract((DateTime)a.DataInceput).Days).Select(p => ((DateTime)a.DataInceput).AddDays(p)))
                    //     select new metaZileLibereIstoricExtins { Zi = d, Culoare = a.Culoare == null ? "#FFFFFFFF" : a.Culoare, denAbs = a.denAbs, idAbs = (int)a.idAbs })
                    //    .Union
                    //    (from d in (Enumerable.Range(0, dtSf.Subtract(dtInc).Days).Select(p => dtInc.AddDays(p)))
                    //     where d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday
                    //     select new metaZileLibereIstoricExtins { Zi = d, Culoare = "#FFFF0000", denAbs = "Sambata sau Duminica", idAbs = -1 })
                    //    .Union
                    //    (from a in this.ObjectContext.HOLIDAYS
                    //     where EntityFunctions.DiffDays(dtInc, a.DAY) >= 0 && EntityFunctions.DiffDays(a.DAY, dtSf) >= 0
                    //     select new metaZileLibereIstoricExtins { Zi = EntityFunctions.TruncateTime((DateTime)a.DAY), Culoare = "#FFFA8282", denAbs = "Zi libera legala", idAbs = -2 })
                    //     .Union
                    //     (from a in ptj.AsEnumerable() select a);
                    #endregion
                }

                dt = General.IncarcaDT(strSQl, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;

        }


        //public string CreeazaExcelSituatieConcedii(int userId, int angId, int an, int luna, int structura, int f10003)
        //{
        //    string rez = "";

        //    try
        //    {
        //        DataTable dt = GetIstoricExtinsLunar(userId, angId, an, luna, structura, f10003);

        //        DateTime ora = DateTime.Now;
        //        string numeXLS = "SituatieConcedii_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";


        //        MemoryStream stream = new MemoryStream(CreazaExcelIstoricExtins(dt));
        //        Response.Clear();
        //        MemoryStream ms = stream;
        //        Response.ContentType = "application/vnd.ms-excel";
        //        Response.AddHeader("content-disposition", "attachment;filename=" + numeXLS);
        //        Response.Buffer = true;
        //        ms.WriteTo(Response.OutputStream);
        //        Response.End();

        //        //MemoryStream stream = new MemoryStream(General.CreazaExcel(dt, 1));
        //        //mm.Attachments.Add(new Attachment(stream, numeXLS, "application/vnd.ms-excel"));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return rez;

        //}



        private void CreazaExcelIstoricExtins(int userId, int angId, int an, int luna, int structura, int f10003)
        {
            //byte[] ras = null;

            try
            {
                
                DataTable dt = GetIstoricExtinsLunar(userId, angId, an, luna, structura, f10003, false);

                DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
                DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws2.Cells[1, i].Value = dt.Columns[i].ColumnName;
                }

                int row = 3;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string val = General.Nz(dt.Rows[i][j], "").ToString();
                        if (val != "" && val.Substring(0, 1) == "#")
                            ws2.Cells[row, j].FillColor = General.Culoare(dt.Rows[i][j].ToString());
                        else
                            ws2.Cells[row, j].Value = dt.Rows[i][j].ToString();
                    }
                    row++;
                }


                row = 3;
                int col = dt.Columns.Count + 2;
                ws2.Cells[1, col].Value = "Legenda";


                //DataTable dtLeg = Session["LegendaAbsente"] as DataTable;
                //for (int i = 0; i < dtLeg.Rows.Count; i++)
                //{
                //    ws2.Cells[row, col].Value = dtLeg.Rows[i]["Denumire"].ToString();
                //    ws2.Cells[row, col + 1].FillColor = General.Culoare(dtLeg.Rows[i]["Culoare"].ToString());
                //    row++;
                //}


                for (int i = 0; i < grLeg.VisibleRowCount; i++)
                {
                    DataRowView obj = grLeg.GetRow(i) as DataRowView;
                    ws2.Cells[row, col].Value = obj["Denumire"].ToString();
                    ws2.Cells[row, col + 1].FillColor = General.Culoare(obj["Culoare"].ToString());
                    row++;
                }


                ws2.Columns.AutoFit(1, 100);

                byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xls);
                //ras = byteArray;


                DateTime ora = DateTime.Now;
                string numeXLS = "SituatieConcedii_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";


                MemoryStream stream = new MemoryStream(byteArray);
                Response.Clear();
                MemoryStream ms = stream;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", "attachment;filename=" + numeXLS);
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid(int tip)
        {
            try
            {
                switch (tip)
                {
                    case 1:
                        {
                            DataTable dt = General.GetSituatieZileAbsente((int)Session["IstoricExtins_Angajat_Marca"]);
                            grDate.DataSource = dt;
                            grDate.DataBind();
                        }
                        break;
                    case 2:
                        {
                            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""viewIstoricExtins"" WHERE F10003=@1", new object[] { Session["IstoricExtins_Angajat_Marca"] });
                            grView.DataSource = dt;
                            grView.DataBind();
                            grView.Columns["F10003"].Visible = false;
                            grView.Columns["F10003"].ShowInCustomizationForm = false;
                        }
                        break;
                    case 3:
                        CreazaGridAnual(Convert.ToInt32(cmbAn.Value ?? DateTime.Now.Year));
                        break;
                    case 4:
                        {
                            DataTable dt = GetIstoricExtinsLunar(Convert.ToInt32(Session["UserId"] ?? 99), Convert.ToInt32(Session["User_Marca"] ?? 99), txtLuna.Date.Year, txtLuna.Date.Month, (int)cmbFil.Value, Convert.ToInt32(Session["IstoricExtins_Angajat_Marca"] ?? -99));
                            Session["IstoricExtins_DataSource"] = dt;
                            grLunar.DataSource = dt;
                            grLunar.DataBind();
                        }
                        break;
                    case 5:
                        {
                            string cmp = "";
                            if (Constante.tipBD == 2)
                                cmp = "FROM DUAL";
                            string sqlLeg = $@"SELECT * FROM (
                                    SELECT ""Id"", ""Culoare"", ""Denumire"" FROM ""Ptj_tblAbsente"" 
                                    WHERE ""Culoare"" IS NOT NULL AND ""Culoare"" <> '#FFFFFF' AND ""Culoare"" <> '#000000'
                                    UNION
                                    SELECT - 2, CASE WHEN COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareZileLibereLegale'),'#FA8282') = '' THEN '#FA8282' ELSE COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareZileLibereLegale'),'#FA8282') END, '{Dami.TraduCuvant("Zi libera legala")}' {cmp}
                                    UNION
                                    SELECT - 1, CASE WHEN COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareSambataSiDuminica'),'#FF0000') = '' THEN '#FF0000' ELSE COALESCE((SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='CuloareSambataSiDuminica'),'#FF0000') END, '{Dami.TraduCuvant("Sambata sau Duminica")}' {cmp}
                                    UNION
                                    SELECT - 3, CASE WHEN COALESCE((SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = 4),'#EE8D3D') = '' THEN '#EE8D3D' ELSE COALESCE((SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = 4),'#EE8D3D') END, '{Dami.TraduCuvant("CO Planificat")}' {cmp}  WHERE (SELECT COUNT(*) FROM ""Ptj_tblAbsente"" WHERE COALESCE(""Planificare"",0)=1) > 0
                                    ) X ORDER BY ""Id"" ";

                            DataTable dt = General.IncarcaDT(sqlLeg, null);
                            grLeg.DataSource = dt;
                            grLeg.KeyFieldName = "Id";
                            grLeg.DataBind();
                            grLeg.Selection.SelectAll();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void SetColoane()
        {
            try
            {
                for (int i = 29; i <= 31; i++)
                {
                    if (i <= DateTime.DaysInMonth(txtLuna.Date.Year, txtLuna.Date.Month))
                    {
                        GridViewDataColorEditColumn col = grLunar.Columns[i.ToString()] as GridViewDataColorEditColumn;
                        col.FieldName = i.ToString();
                        col.Visible = true;
                    }
                    else
                    {
                        GridViewDataColorEditColumn col = grLunar.Columns[i.ToString()] as GridViewDataColorEditColumn;
                        col.FieldName = "";
                        col.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void TraduGrid(ASPxGridView grDate)
        {
            foreach (dynamic c in grDate.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                }catch (Exception) { }
            }


            //foreach (GridViewColumn c in grDate.Columns)
            //{
            //    try
            //    {
            //        dynamic col = null;

            //        //switch(c.GetType().ToString())
            //        //{
            //        //    case "DevExpress.Web.GridViewDataColumn":
            //        //            col = c as GridViewDataColumn;
            //        //        break;
            //        //    case "DevExpress.Web.GridViewDataTextColumn":
            //        //        col = c as GridViewDataTextColumn;
            //        //        break;
            //        //}

            //        //col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

            //        if (c.GetType() == typeof(GridViewDataColumn) || c.GetType() == typeof(GridViewDataTextColumn))
            //        {
            //            col = c as GridViewDataColumn;
            //            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
            //        }
            //    }
            //    catch (Exception) { }
            //}
        }



    }
}