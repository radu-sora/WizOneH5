using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PlanificareSchimburi : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Pontaj.PlanificareSchimburi";

                if (!IsPostBack)
                    Session["Ptj_NrRanduri"] = 10;

                int nrRanduri = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ", "10"));
                grDate.SettingsPager.PageSize = nrRanduri;
                Session["Ptj_NrRanduri"] = nrRanduri;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txtTitlu.Text = General.VarSession("Titlu").ToString();

                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnExport.Text = Dami.TraduCuvant("btnExport", "Export");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblDtInc.InnerText = Dami.TraduCuvant("Data Inceput");
                lblDtSf.InnerText = Dami.TraduCuvant("Data Sfarsit");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblCtr.InnerText = Dami.TraduCuvant("Contract");
                lblSub.InnerText = Dami.TraduCuvant("Subcompanie");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Dept");
                lblSubDept.InnerText = Dami.TraduCuvant("SubDept");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");
                lblCateg.InnerText = Dami.TraduCuvant("Categorie");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    txtDtInc.Value = DateTime.Now;
                    txtDtSf.Value = DateTime.Now;

                    IncarcaAngajati();
                }
                else
                {
                    DataTable dtCmb = Session["SurseCombo"] as DataTable;
                    GridViewDataComboBoxColumn colAbs = (grDate.Columns["ValAbs"] as GridViewDataComboBoxColumn);
                    if (colAbs != null) colAbs.PropertiesComboBox.DataSource = dtCmb;

                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();
                    cmbAng.DataSource = Session["Pontaj_Angajati"];
                    cmbAng.DataBind();

                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }
                }

                CreeazaGrid();

                cmbSub.DataSource = General.IncarcaDT($@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003 WHERE F00310 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00311", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT($@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404= {General.Nz(cmbSub.Value, -99)} AND F00411 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00412", null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT($@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505= {General.Nz(cmbFil.Value, -99)} AND F00513 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00514", null);
                cmbSec.DataBind();

                ASPxListBox lstDept = cmbDept.FindControl("listBox") as ASPxListBox;
                if (lstDept != null)
                {
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        lstDept.DataSource = General.IncarcaDT(
                            $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                            UNION
                            SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00622 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00623", null);
                        lstDept.DataBind();
                    }
                    else
                    {
                        lstDept.DataSource = General.IncarcaDT(
                            $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                            UNION
                            SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606={General.Nz(cmbSec.Value, -99)} AND F00622 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00623", null);
                        lstDept.DataBind();
                    }
                }

                cmbSubDept.DataSource = General.IncarcaDT(
                    $@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 
                    INNER JOIN F006 ON F007.F00707=F006.F00607 WHERE F00608 IN ('{General.Nz(cmbDept.Value, -99).ToString().Replace(",","','")}') AND F00714 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00715", null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT($"SELECT F00809, F00810 FROM F008 WHERE F00814 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00815", null);
                cmbBirou.DataBind();

                cmbCateg.DataSource = General.IncarcaDT(@"SELECT ""Denumire"" AS ""Id"", ""Denumire"" FROM ""viewCategoriePontaj"" GROUP BY ""Denumire"" ", null);
                cmbCateg.DataBind();

                ASPxListBox lstCtr = cmbCtr.FindControl("listBox") as ASPxListBox;
                if (lstCtr != null)
                {
                    lstCtr.DataSource = General.IncarcaDT(
                        $@"SELECT -5 AS ""Id"", 'Select All' AS ""Denumire"" {General.FromDual()}
                        UNION
                        SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte""
                        ORDER BY ""Id"" ", null);
                    lstCtr.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                bool esteStruc = true;
                string dataRef = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                switch (e.Parameter)
                {
                    case "cmbSub":
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbFil":
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbSec":
                        cmbDept.Value = null;
                        cmbSubDept.Value = null;
                        break;
                    case "cmbDept":
                        cmbSubDept.Value = null;
                        break;
                    case "EmptyFields":
                        cmbDept.DataSource = General.IncarcaDT(
                            $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                            UNION
                            SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00622 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00623", null);
                        cmbDept.DataBind();
                        return;
                    case "cmbRol":
                    case "txtAnLuna":
                        IncarcaAngajati();
                        esteStruc = false;
                        break;
                }

                if (esteStruc)
                {
                    cmbFil.DataSource = General.IncarcaDT($@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404={General.Nz(cmbSub.Value, -99)} AND F00411 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00412", null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT($@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505={General.Nz(cmbFil.Value, -99)} AND F00513 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00514", null);
                    cmbSec.DataBind();
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        cmbDept.DataSource = General.IncarcaDT(
                            $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                            UNION
                            SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00622 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00623", null);
                        cmbDept.DataBind();
                    }
                    else
                    {
                        cmbDept.DataSource = General.IncarcaDT(
                            $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                            UNION 
                            SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606={General.Nz(cmbSec.Value, -99)} AND F00622 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00623", null);
                        cmbDept.DataBind();
                    }
                    cmbSubDept.DataSource = General.IncarcaDT($@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 INNER JOIN F006 ON F007.F00707=F006.F00607 WHERE F00608 IN ('{General.Nz(cmbDept.Value, -99).ToString().Replace(",", "','")}') AND F00714 <= {General.ToDataUniv(DateTime.Now)} AND {General.ToDataUniv(DateTime.Now)} <= F00715", null);
                    cmbSubDept.DataBind();       

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                grDate.KeyFieldName = "F10003";

                string strSql = DamiSelect();
                DataTable dt = General.IncarcaDT(strSql, null);

                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaAngajati()
        {
            try
            {
                DataTable dt = General.IncarcaDT(SelectAngajati(), null);

                cmbAng.DataSource = null;
                cmbAng.DataSource = dt;
                Session["Pontaj_Angajati"] = dt;
                cmbAng.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2) cmp = "ROWNUM";

                strSql = $@"SELECT {cmp} AS ""IdAuto"", X.* 
                            FROM ({SelectComun()}) X 
                            WHERE X.F10022 <= {General.ToDataUniv(txtDtSf.Date)} AND {General.ToDataUniv(txtDtInc.Date)} <= X.F10023 
                            {filtru}
                            ORDER BY X.""NumeComplet"" ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string SelectComun()
        {
            string strSql = "";
            try
            {
                strSql = $@"SELECT B.F10003 AS F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                            A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                            G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                            CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Denumire"", '') AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                            FROM ""relGrupAngajat"" B
                            INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                            INNER JOIN F100 A ON b.F10003 = a.F10003
                            LEFT JOIN F718 D ON A.F10071 = D.F71802
                            LEFT JOIN F002 E ON A.F10002 = E.F00202
                            LEFT JOIN F003 F ON A.F10004 = F.F00304
                            LEFT JOIN F004 G ON A.F10005 = G.F00405
                            LEFT JOIN F005 H ON A.F10006 = H.F00506
                            LEFT JOIN F006 I ON A.F10007 = I.F00607
                            LEFT JOIN ""Ptj_tblRoluri"" S ON C.""IdRol""=S.""Id""
                            WHERE C.""IdSuper"" = {Session["UserId"]}
                            UNION
                            SELECT B.F10003 AS F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                            A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                            G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                            CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Denumire"", '') AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                            FROM ""relGrupAngajat"" B
                            INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                            INNER JOIN F100 A ON b.F10003 = a.F10003
                            INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                            LEFT JOIN F718 D ON A.F10071 = D.F71802
                            LEFT JOIN F002 E ON A.F10002 = E.F00202
                            LEFT JOIN F003 F ON A.F10004 = F.F00304
                            LEFT JOIN F004 G ON A.F10005 = G.F00405
                            LEFT JOIN F005 H ON A.F10006 = H.F00506
                            LEFT JOIN F006 I ON A.F10007 = I.F00607
                            LEFT JOIN ""Ptj_tblRoluri"" S ON C.""IdRol""=S.""Id""
                            WHERE J.""IdUser"" = {Session["UserId"]}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName.Length >= 4 && e.DataColumn.FieldName.ToLower().Substring(0, 4) == "ziua")
                {
                    string val = General.Nz(grDate.GetRowValuesByKeyValue(e.KeyValue, "ZileGri"), "").ToString();
                    if (val != "" && (val + ",").IndexOf("," + e.DataColumn.FieldName + ",") >= 0)
                    {
                        e.Cell.BackColor = Color.DarkGray;
                        e.Cell.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
                    if (arr.Length == 0 || arr[0] == "")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    switch (arr[0].ToString())
                    {
                        case "btnFiltru":
                            IncarcaGrid();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string DamiSelect(bool ptDnata = false)
        {
            string strSql = "";

            try
            {
                int cnt = 0;
                string zile = "";
                string zileAs = "";
                string zileVal = "";
                string strFiltru = "";
                string strLeg = "";

                string dtInc = General.ToDataUniv(txtDtInc.Date);
                string dtSf = General.ToDataUniv(txtDtSf.Date);
                
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (General.Nz(cmbDept.Value,"").ToString() != "") strFiltru += @" AND S6.F00608 IN ('" + cmbDept.Value.ToString().Replace(",","','") + "')";
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99) strFiltru += " AND B.F100958 = " + cmbSubDept.Value;
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99) strFiltru += " AND B.F100959 = " + cmbBirou.Value;
                if (General.Nz(cmbCtr.Value, "").ToString() != "") strFiltru += " AND C.\"Denumire\" IN ('" + cmbCtr.Value.ToString().Replace(",","','") + "')";

                string sqlCateg = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewCategoriePontaj'";
                if (Constante.tipBD == 2)
                    sqlCateg = "SELECT COUNT(*) FROM user_views where view_name = 'viewCategoriePontaj'";
                int exista = Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlCateg, null),0));
                if (exista != 0)
                {
                    if (General.Nz(cmbCateg.Value, "").ToString() != "")
                    {
                        strFiltru += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                        strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    }
                }

                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                    strFiltru += $@" AND A.F10003 IN (SELECT F10003 FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]})";
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;

                for (DateTime zi = txtDtInc.Date; zi <= txtDtSf.Date; zi=zi.AddDays(1))
                {
                    cnt++;
                    string strZi = $"[{zi.Year}-{zi.Month.ToString().PadLeft(2, Convert.ToChar("0"))}-{zi.Day.ToString().PadLeft(2, Convert.ToChar("0"))}]";
                    if (Constante.tipBD == 2) strZi = $"'{zi.Day.ToString().PadLeft(2, Convert.ToChar("0"))}-{zi.Month.ToString().PadLeft(2, Convert.ToChar("0"))}-{zi.Year}'";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + cnt.ToString() + "\"";
                    zileVal += $@",""Ziua{cnt}""";
                }

                //COALESCE((-1 * B.""Id""), (A.""IdContractP"" * 1000 + A.""IdProgramP"")) AS ""Id""

                if (Constante.tipBD == 1)
                    strSql = $@"SELECT A.*,
                        (SELECT ',Ziua' + CASE WHEN Y.Zi < CONVERT(date,X.F10022) OR CONVERT(date,X.F10023) < Y.Zi THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                        FROM F100 X
                        INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                        WHERE X.F10003=A.F10003
                        FOR XML PATH ('')) AS ZileGri
                        FROM (
                        SELECT TOP 100 PERCENT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS ""AngajatNume"", C.""Denumire"" AS ""Contract"" {zileVal}
                        FROM
                        (SELECT F10003 {zileAs} FROM 
                        (SELECT  A.""Ziua"", A.F10003, 
                        COALESCE((-1 * B.""Id""), A.""IdProgramP"") AS ""Id""
                        FROM ""Ptj_Intrari"" A
                        LEFT JOIN ""Ptj_tblAbsente"" B ON A.""ValStr""=B.""DenumireScurta"" AND B.""DenumireScurta"" <> ''
                        LEFT JOIN ""Ptj_Contracte"" C ON A.""IdContractP"" = C.""Id""
                        LEFT JOIN ""Ptj_Programe"" D ON A.""IdProgramP""=D.""Id""
                        WHERE {dtInc} <= CAST(A.""Ziua"" AS date) AND CAST(A.""Ziua"" AS date) <= {dtSf}) source  
                        PIVOT (MAX(""Id"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                        ) pvt
                        LEFT JOIN F100 A ON A.F10003=pvt.F10003 
                        LEFT JOIN F1001 B ON A.F10003=B.F10003 
                        LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari"" R WHERE {dtInc} <= CAST(R.""Ziua"" AS date) AND CAST(R.""Ziua"" AS date) <= {dtSf} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                        LEFT JOIN ""Ptj_Intrari"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                        LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                        LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                        {strLeg}
                        LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
                        LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
                        LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405
                        LEFT JOIN F005 S5 ON Y.F10006 = S5.F00506
                        LEFT JOIN F006 S6 ON Y.F10007 = S6.F00607
                        LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                        LEFT JOIN F008 S8 ON B.F100959 = S8.F00809
                        WHERE 1=1 {strFiltru}
                        ORDER BY ""AngajatNume"") A";
                else
                    strSql = $@"SELECT A.*,
                        (SELECT LISTAGG(',Ziua' || CASE WHEN Y.""Zi"" < TRUNC(F10022) OR TRUNC(X.F10023) < Y.""Zi"" THEN TO_CHAR(EXTRACT(DAY FROM Y.""Zi"")) END) WITHIN GROUP (ORDER BY X.F10003)
                        FROM F100 X
                        INNER JOIN ""tblZile"" Y ON {dtInc} <= Y.""Zi"" AND Y.""Zi"" <= {dtSf}
                        WHERE X.F10003 = A.F10003
                        ) AS ""ZileGri""
                        FROM (
                        SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS ""AngajatNume"", C.""Denumire"" AS ""Contract"" {zileVal}
                        FROM
                        (SELECT * FROM 
                        (SELECT  A.""Ziua"", A.F10003, 
                        COALESCE((-1 * B.""Id""), A.""IdProgramP"") AS ""Id""
                        FROM ""Ptj_Intrari"" A
                        LEFT JOIN ""Ptj_tblAbsente"" B ON A.""ValStr""=B.""DenumireScurta"" AND B.""DenumireScurta"" <> ''
                        LEFT JOIN ""Ptj_Contracte"" C ON A.""IdContractP"" = C.""Id""
                        LEFT JOIN ""Ptj_Programe"" D ON A.""IdProgramP""=D.""Id""
                        WHERE {dtInc} <= CAST(A.""Ziua"" AS date) AND CAST(A.""Ziua"" AS date) <= {dtSf}) source  
                        PIVOT (MAX(""Id"") FOR ""Ziua"" IN ( {zileAs.Substring(1)} )) pvt
                        ) pvt
                        LEFT JOIN F100 A ON A.F10003=pvt.F10003 
                        LEFT JOIN F1001 B ON A.F10003=B.F10003 
                        LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari"" R WHERE {dtInc} <= CAST(R.""Ziua"" AS date) AND CAST(R.""Ziua"" AS date) <= {dtSf} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                        LEFT JOIN ""Ptj_Intrari"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                        LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                        LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                        {strLeg}
                        LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
                        LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
                        LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405
                        LEFT JOIN F005 S5 ON Y.F10006 = S5.F00506
                        LEFT JOIN F006 S6 ON Y.F10007 = S6.F00607
                        LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                        LEFT JOIN F008 S8 ON B.F100959 = S8.F00809
                        WHERE 1=1 {strFiltru}
                        ORDER BY ""AngajatNume"") A";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private void CreeazaGrid()
        {
            try
            {
                #region OLD
                //DataTable dt = General.IncarcaDT(
                //    $@"SELECT ""ContractId"" * 1000 + ""ProgramId"" AS ""IdAuto"", ""ProgDenScurta"" AS ""Denumire"", X.* FROM
                //    (
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 1 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb1"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program1"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb1"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program1"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 2 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb2"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program2"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb2"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program2"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 3 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb3"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program3"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb3"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program3"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 4 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb4"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program4"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb4"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program4"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 5 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb5"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program5"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb5"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program5"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 6 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb6"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program6"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb6"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program6"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 7 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb7"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program7"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb7"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program7"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    UNION
                //    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 8 AS ""ZiSapt"", 
                //    CASE WHEN COALESCE(A.""TipSchimb8"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program8"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                //    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                //    FROM ""Ptj_Contracte"" A
                //    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract""
                //    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb8"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program8"", A.""Program0"") ELSE B.""IdProgram"" END)
                //    ) X
                //    UNION 
                //    SELECT -1 * ""Id"", COALESCE(""DenumireScurta"",""Denumire""), -99, '', -99, -99, '', '' FROM ""Ptj_tblAbsente""", null);
                #endregion

                DataTable dt = General.IncarcaDT(
                    $@"SELECT DISTINCT ""ProgramId"" AS ""IdAuto"", ""ProgDenScurta"" AS ""Denumire"", ""ZiSapt"" FROM
                    (
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 1 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb1"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program1"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,1))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb1"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program1"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 2 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb2"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program2"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,2))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb2"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program2"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 3 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb3"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program3"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,3))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb3"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program3"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 4 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb4"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program4"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,4))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb4"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program4"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 5 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb5"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program5"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,5))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb5"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program5"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 6 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb6"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program6"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,6))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb6"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program6"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 7 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb7"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program7"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,7))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb7"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program7"", A.""Program0"") ELSE B.""IdProgram"" END)
                    UNION
                    SELECT A.""Id"" AS ""ContractId"", A.""Denumire"" AS ""ContractDen"", 8 AS ""ZiSapt"", 
                    CASE WHEN COALESCE(A.""TipSchimb8"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program8"", A.""Program0"") ELSE B.""IdProgram"" END AS ""ProgramId"",
                    C.""Denumire"" AS ""ProgramDen"", COALESCE(C.""DenumireScurta"", C.""Denumire"") AS ""ProgDenScurta""
                    FROM ""Ptj_Contracte"" A
                    LEFT JOIN ""Ptj_ContracteSchimburi"" B ON A.""Id""=B.""IdContract"" AND B.""TipSchimb"" IN (SELECT MAX(X.""TipSchimb"") FROM ""Ptj_ContracteSchimburi"" X WHERE X.""IdContract""=A.""Id"" AND X.""TipSchimb"" IN (0,8))
                    LEFT JOIN ""Ptj_Programe"" C ON C.""Id"" = (CASE WHEN COALESCE(A.""TipSchimb8"", A.""TipSchimb0"") = 1 THEN COALESCE(A.""Program8"", A.""Program0"") ELSE B.""IdProgram"" END)
                    ) X
                    UNION 
                    SELECT -1 * ""Id"", COALESCE(""DenumireScurta"",""Denumire""), -99 FROM ""Ptj_tblAbsente""", null);


                if (dt != null && dt.Rows.Count > 0)
                {
                    string jsonPrg = "";
                    for (int g = 0; g < dt.Rows.Count; g++)
                    {
                        //jsonPrg += ",{ IdAuto: " + dt.Rows[g]["IdAuto"] + ", Denumire: \"" + General.Nz(dt.Rows[g]["Denumire"], "").ToString().Trim().Replace("\n", "").Replace("\r", "") + "\", ZiSapt: " + dt.Rows[g]["ZiSapt"] + ", Contract: '" + dt.Rows[g]["ContractDen"] + "' }";
                        jsonPrg += ",{ IdAuto: " + dt.Rows[g]["IdAuto"] + ", Denumire: \"" + General.Nz(dt.Rows[g]["Denumire"], "").ToString().Trim().Replace("\n", "").Replace("\r", "") + "\", ZiSapt: " + dt.Rows[g]["ZiSapt"] + " }";
                    }
                    if (jsonPrg.Length > 0)
                        Session["Json_Programe"] = "[" + jsonPrg.Substring(1) + "]";
                }

                DataTable dtZi = General.IncarcaDT(
                    $@"SELECT ""Zi"", CASE WHEN ""ZiLiberaLegala"" <> 0 THEN 8 ELSE ""ZiSapt"" END AS ""ZiSapt"",
                    CASE WHEN ""ZiLiberaLegala"" <> 0 OR ""ZiSapt"" = 6 OR ""ZiSapt"" = 7 THEN 1 ELSE 0 END AS ""Libera""
                    FROM ""tblZile"" WHERE {General.ToDataUniv(txtDtInc.Date)} <= ""Zi"" AND ""Zi"" <= {General.ToDataUniv(txtDtSf.Date)} ", null);
                var lstZiSapt = new Dictionary<int, object>();

                //stergem coloanele Ziua
                for (int i = 0; i < grDate.Columns.Count; i++)
                {
                    if (grDate.Columns[i].Name.Length >= 4 && grDate.Columns[i].Name.Substring(0, 4).ToLower() == "ziua")
                        grDate.Columns.Remove(grDate.Columns[i]);
                }

                //adaugam coloanele din noul interval
                int cnt = 0;
                for (int i = 0; i < dtZi.Rows.Count; i++)
                {
                    cnt++;
                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                    c.Name = "Ziua" + cnt;
                    c.FieldName = "Ziua" + cnt;
                    c.Caption = Convert.ToDateTime(dtZi.Rows[i]["Zi"]).Day.ToString().PadLeft(2, '0') + "." + Convert.ToDateTime(dtZi.Rows[i]["Zi"]).Month.ToString().PadLeft(2, '0');
                    c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                    c.PropertiesComboBox.DataSource = dt;
                    c.PropertiesComboBox.AllowNull = true;
                    c.PropertiesComboBox.ValueField = "IdAuto";
                    c.PropertiesComboBox.TextField = "Denumire";

                    c.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True;
                    c.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False;
                    c.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText;
                    c.SettingsHeaderFilter.Mode = GridHeaderFilterMode.CheckedList;

                    if (General.Nz(dtZi.Rows[i]["Libera"],0).ToString() == "1")
                        c.CellStyle.BackColor = Color.Aquamarine;

                    grDate.Columns.Add(c);

                    lstZiSapt.Add(i, dtZi.Rows[i]["ZiSapt"]);
                }

                grDate.JSProperties["cp_ZiSapt"] = lstZiSapt;
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
                if (e.Column.FieldName.IndexOf("Ziua") >= 0)
                    e.Editor.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                grDate.CancelEdit();

                string strSql = "";
                int i = 0;
                var lstZile = new Dictionary<int, DateTime>();
                for (DateTime zi = txtDtInc.Date; zi <= txtDtSf.Date; zi = zi.AddDays(1))
                {
                    lstZile.Add(i, zi);
                    i++;
                }

                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                    string f10003 = General.Nz(upd.NewValues["F10003"], -99).ToString();

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        string numeCol = de.Key.ToString();
                        dynamic oldValue = upd.OldValues[numeCol];
                        dynamic newValue = upd.NewValues[numeCol];
                        if (oldValue != null && upd.OldValues[numeCol].GetType() == typeof(System.DBNull))
                            oldValue = null;

                        if (numeCol.ToLower().IndexOf("ziua") >= 0 && oldValue != newValue)
                        {
                            int idx = Convert.ToInt32(numeCol.ToLower().Replace("ziua", "")) - 1;
                            string idCtr = "NULL";
                            string idPrg = "NULL";
                            int val = Convert.ToInt32(General.Nz(upd.NewValues[numeCol], 0));
                            if (val > 0)
                            {
                                idCtr = Convert.ToInt32(val / 1000).ToString();
                                idPrg = Convert.ToInt32(val % 1000).ToString();
                            }

                            strSql += $@"UPDATE ""Ptj_Intrari"" SET ""IdContractP""={idCtr}, ""IdProgramP""={idPrg}, USER_NO={Session["UserId"]}, TIME={General.CurrentDate()} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(lstZile[idx])};" + Environment.NewLine;
                        }
                    }
                }

                if (strSql != "")
                {
                    General.ExecutaNonQuery(strSql, null);
                    IncarcaGrid();
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
                }

                e.Handled = true;
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
                grDateExport.WriteXlsxToResponse("PlanificareSchimburi-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
            }
            catch (Exception)
            {
            }
        }

    }
}
 
 
 
 
 
 
 
 
 
 
 
 