using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Drawing;
using System.Data.SqlClient;

namespace WizOne.Pontaj
{
    public partial class PlanificareSchimburi : System.Web.UI.Page
    {
        private string arrZL = "";

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

                //CreeazaGrid();

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

                DateTime ziua = Convert.ToDateTime(txtDtInc.Value);
                if (Constante.tipBD == 1)
                    arrZL = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN A.ZiSapt=6 OR A.ZiSapt=7 OR B.DAY IS NOT NULL THEN 'Ziua' + CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2)) + ';' ELSE '' END 
                                        FROM tblzile A
                                        LEFT JOIN HOLIDAYS B ON A.Zi=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month,1)} <= A.Zi AND A.Zi <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}
                                        FOR XML PATH ('')", null), "").ToString();
                else
                    arrZL = General.Nz(General.ExecutaScalar($@"SELECT LISTAGG(CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR B.DAY IS NOT NULL THEN 'Ziua' || CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2))  ELSE '' END,  ';') WITHIN GROUP (ORDER BY A.""Zi"") || ';'
                                        FROM ""tblZile"" A
                                        LEFT JOIN HOLIDAYS B ON A.""Zi""=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month, 1)} <= A.""Zi"" AND A.""Zi"" <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}", null), "").ToString();

                DataTable dtZ = General.IncarcaDT(
    $@"SELECT ContractId * 1000 + ProgramId AS IdAuto, ContractDen + ' - ' + ProgramDen AS Denumire FROM
                    (
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 1 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb1, A.TipSchimb0) = 1 THEN COALESCE(A.Program1, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb1, A.TipSchimb0) = 1 THEN COALESCE(A.Program1, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 2 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb2, A.TipSchimb0) = 1 THEN COALESCE(A.Program2, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb2, A.TipSchimb0) = 1 THEN COALESCE(A.Program2, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 3 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb3, A.TipSchimb0) = 1 THEN COALESCE(A.Program3, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb3, A.TipSchimb0) = 1 THEN COALESCE(A.Program3, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 4 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb4, A.TipSchimb0) = 1 THEN COALESCE(A.Program4, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb4, A.TipSchimb0) = 1 THEN COALESCE(A.Program4, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 5 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb5, A.TipSchimb0) = 1 THEN COALESCE(A.Program5, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb5, A.TipSchimb0) = 1 THEN COALESCE(A.Program5, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 6 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb6, A.TipSchimb0) = 1 THEN COALESCE(A.Program6, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb6, A.TipSchimb0) = 1 THEN COALESCE(A.Program6, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 7 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb7, A.TipSchimb0) = 1 THEN COALESCE(A.Program7, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb7, A.TipSchimb0) = 1 THEN COALESCE(A.Program7, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 8 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb8, A.TipSchimb0) = 1 THEN COALESCE(A.Program8, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb8, A.TipSchimb0) = 1 THEN COALESCE(A.Program8, Program0) ELSE B.IdProgram END)
                    ) X
                    UNION 
                    SELECT -1 * Id AS IdAuto, Denumire FROM Ptj_tblAbsente", null);

                for (int i = 1; i <= 3; i++)
                {
                    GridViewDataComboBoxColumn cmb = grDate.Columns["Ziua" + i] as GridViewDataComboBoxColumn;
                    cmb.PropertiesComboBox.DataSource = dtZ;
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

                DataTable dt = PontajAfiseaza();
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

        public DataTable PontajAfiseaza()
        {
            string strSql = "";
            DataTable dt = new DataTable();

            try
            {
                strSql = DamiSelect();
                dt = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
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
                    if (arrZL.Length > 0 && arrZL.IndexOf(e.DataColumn.FieldName + ";") >= 0)
                        e.Cell.BackColor = System.Drawing.Color.Aquamarine;
                    
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
                        //case "divDtSf":
                        //    CreeazaGrid();
                        //    break;
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

        protected void btnExp_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    if (!chkTotaluri.Checked && !chkOre.Checked && !chkPauza.Checked)
            //    {
            //        MessageBox.Show(Dami.TraduCuvant("Lipsesc date"), MessageBox.icoInfo, "Export");
            //    }
            //    else
            //    {
            //        DataTable dt = General.IncarcaDT(DamiSelectExport(), null);

            //        DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
            //        DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];

            //        int an = Convert.ToDateTime(txtAnLuna.Value).Year;
            //        int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
            //        string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + an;
            //        if (Constante.tipBD == 2)
            //            strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + an;
            //        DataTable dtHolidays = General.IncarcaDT(strSql, null);

            //        DataTable dtCol = General.IncarcaDT("SELECT * FROM \"Ptj_tblPrint\" WHERE \"Activ\" = 1 ORDER BY \"Ordine\"", null);
            //        Dictionary<string, string> lista = new Dictionary<string, string>();
            //        Dictionary<string, string> listaLung = new Dictionary<string, string>();
            //        Dictionary<string, int> listaId = new Dictionary<string, int>();
            //        if (dtCol != null && dtCol.Rows.Count > 0)
            //            for (int j = 0; j < dtCol.Rows.Count; j++)
            //            {
            //                lista.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["TextAfisare"].ToString());
            //                listaLung.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["Lungime"].ToString());
            //                listaId.Add(dtCol.Rows[j]["Camp"].ToString(), j + 1);
            //            }

            //        DataTable dtAbs = General.IncarcaDT("SELECT DISTINCT \"DenumireScurta\", max(\"Culoare\") AS \"Culoare\" FROM \"Ptj_tblAbsente\" WHERE \"IdTipOre\" = 1 GROUP BY \"DenumireScurta\"", null);
            //        Dictionary<string, string> listaAbs = new Dictionary<string, string>();
            //        if (dtAbs != null && dtAbs.Rows.Count > 0)
            //            for (int j = 0; j < dtAbs.Rows.Count; j++)
            //                listaAbs.Add(dtAbs.Rows[j]["DenumireScurta"].ToString(), dtAbs.Rows[j]["Culoare"].ToString());

            //        //Radu 28.02.2020 - securitate
            //        List<string> listaSec = new List<string>();
            //        strSql = "SELECT X.\"IdControl\", X.\"IdColoana\", MAX(X.\"Vizibil\") AS \"Vizibil\", MIN(X.\"Blocat\") AS \"Blocat\" FROM( "
            //                + "SELECT A.\"IdControl\", A.\"IdColoana\", A.\"Vizibil\", A.\"Blocat\" "
            //                + "FROM \"Securitate\" A "
            //                + "INNER JOIN \"relGrupUser\" B ON A.\"IdGrup\" = B.\"IdGrup\" "
            //                + "WHERE B.\"IdUser\" = {0} AND LOWER(A.\"IdForm\") = 'pontaj.pontajechipa' "
            //                + "UNION "
            //                + "SELECT A.\"IdControl\", A.\"IdColoana\", A.\"Vizibil\", A.\"Blocat\" "
            //                + "FROM \"Securitate\" A "
            //                + "WHERE A.\"IdGrup\" = -1 AND LOWER(A.\"IdForm\") = 'pontaj.pontajechipa') X "
            //                + "GROUP BY X.\"IdControl\", X.\"IdColoana\"";
            //        strSql = string.Format(strSql, Session["UserId"].ToString());
            //        if (General.VarSession("EsteAdmin").ToString() == "0")
            //        {
            //            DataTable dtSec = General.IncarcaDT(strSql, null);
            //            if (dtSec != null && dtSec.Rows.Count > 0)
            //                for (int k = 0; k < dtSec.Rows.Count; k++)
            //                    if (dtSec.Rows[k]["Vizibil"] != null && Convert.ToInt32(dtSec.Rows[k]["Vizibil"].ToString()) == 0)
            //                        listaSec.Add(dtSec.Rows[k]["IdColoana"].ToString());
            //        }


            //        if (chkLinie.Checked)
            //        {
            //            int nrCol = 0;              
            //            int idZile = 0, colZile = 0;
            //            for (int i = 0; i < dt.Columns.Count; i++)
            //            {
            //                if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
            //                {
            //                    if (idZile > 0 && colZile > 0)
            //                    {
            //                        ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = lista[dt.Columns[i].ColumnName];
            //                        ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
            //                    }
            //                    else
            //                    {
            //                        ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
            //                        ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
            //                    }

            //                }
            //                else if (dt.Columns[i].ColumnName.Contains("Ziua"))
            //                {
            //                    ws2.Cells[1, nrCol].Value = dt.Columns[i].ColumnName.Replace("Ziua", "");
            //                    DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
            //                    bool ziLibera = false;
            //                    for (int z = 0; z < dtHolidays.Rows.Count; z++)
            //                        if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
            //                        {
            //                            ziLibera = true;
            //                            break;
            //                        }
            //                    if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[1, nrCol].FillColor = Color.FromArgb(217, 243, 253);
            //                    ws2.Cells[1, nrCol].ColumnWidth = Convert.ToInt32(listaLung["Zilele 1-31"]);
            //                    nrCol++;                          
            //                    idZile = listaId["Zilele 1-31"];
            //                    colZile = nrCol;
            //                }
            //            }

                        
            //            for (int row = 0; row < dt.Rows.Count; row++)
            //            {
            //                nrCol = 0;
            //                idZile = 0; colZile = 0;
            //                for (int i = 0; i < dt.Columns.Count; i++)
            //                {
            //                    if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
            //                    {
            //                        if (idZile > 0 && colZile > 0)                                    
            //                            ws2.Cells[row + 3, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = dt.Rows[row][i].ToString();
            //                        else                                    
            //                            ws2.Cells[row + 3, nrCol++].Value = dt.Rows[row][i].ToString();
            //                    }

            //                    if (dt.Columns[i].ColumnName.Contains("Ziua"))
            //                    {
            //                        ws2.Cells[row + 3, nrCol].Value = dt.Rows[row][i].ToString();
            //                        DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
            //                        bool ziLibera = false;
            //                        for (int z = 0; z < dtHolidays.Rows.Count; z++)
            //                            if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
            //                            {
            //                                ziLibera = true;
            //                                break;
            //                            }
            //                        if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[row + 3, nrCol].FillColor = Color.FromArgb(217, 243, 253);
            //                        if (listaAbs.ContainsKey(dt.Rows[row][i].ToString()))
            //                            ws2.Cells[row + 3, nrCol].FillColor = General.Culoare(listaAbs[dt.Rows[row][i].ToString()]);
            //                        if (dt.Rows[row][i] != null && dt.Rows[row]["CuloareValoare" + zi.Day].ToString().ToUpper() == Constante.CuloareModificatManual.ToUpper())
            //                            ws2.Cells[row + 3, nrCol].FillColor = General.Culoare(Constante.CuloareModificatManual);
            //                        nrCol++;
            //                        idZile = listaId["Zilele 1-31"];
            //                        colZile = nrCol;                                    

            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            int nrCol = 0;
            //            int rand = 0;
            //            int idZile = 0, colZile = 0;
            //            for (int i = 0; i < dt.Columns.Count; i++)
            //            {
            //                if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
            //                {
            //                    if (idZile > 0 && colZile > 0)
            //                    {
            //                        ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = lista[dt.Columns[i].ColumnName];
            //                        ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
            //                    }
            //                    else
            //                    {
            //                        ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
            //                        ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
            //                    }
            //                }
            //                else if (dt.Columns[i].ColumnName.Contains("Ziua") && !dt.Columns[i].ColumnName.Contains("I") && !dt.Columns[i].ColumnName.Contains("O") && !dt.Columns[i].ColumnName.Contains("P"))
            //                {
            //                    ws2.Cells[1, nrCol].Value = dt.Columns[i].ColumnName.Replace("Ziua", "");
            //                    ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung["Zilele 1-31"]);
            //                    idZile = listaId["Zilele 1-31"];
            //                    colZile = nrCol - 1;
            //                }
            //            }

            //            for (int row = 0; row < dt.Rows.Count; row++)
            //            {
            //                nrCol = -1;
            //                idZile = 0; colZile = 0;
            //                for (int i = 0; i < dt.Columns.Count; i++)
            //                {
            //                    if ((lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName)) || (dt.Columns[i].ColumnName.Contains("Ziua")))
            //                    {
            //                        if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("I"))
            //                            rand = 1;
            //                        else if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("O"))
            //                            rand = 2;
            //                        else if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("P"))
            //                            rand = 3;
            //                        else
            //                        {
            //                            rand = 0;
            //                            nrCol++;
            //                        }
            //                        if (idZile > 0 && colZile > 0 && lista.ContainsKey(dt.Columns[i].ColumnName))
            //                            ws2.Cells[4 * row + 3 + rand - 1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = dt.Rows[row][i].ToString();
            //                        else
            //                            ws2.Cells[4 * row + 3 + rand - 1, nrCol].Value = dt.Rows[row][i].ToString();

            //                        if (dt.Columns[i].ColumnName.Contains("Ziua"))
            //                        {
            //                            DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
            //                            bool ziLibera = false;
            //                            for (int z = 0; z < dtHolidays.Rows.Count; z++)
            //                                if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
            //                                {
            //                                    ziLibera = true;
            //                                    break;
            //                                }
            //                            if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = Color.FromArgb(217, 243, 253);
            //                            if (listaAbs.ContainsKey(dt.Rows[row][i].ToString()))
            //                                ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = General.Culoare(listaAbs[dt.Rows[row][i].ToString()]);
            //                            if (dt.Rows[row][i] != null && dt.Rows[row]["CuloareValoare" + zi.Day].ToString().ToUpper() == Constante.CuloareModificatManual.ToUpper())
            //                                ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = General.Culoare(Constante.CuloareModificatManual);
            //                            idZile = listaId["Zilele 1-31"];
            //                            colZile = nrCol;
            //                        }
            //                    }

            //                }
            //                idZile = listaId["Zilele 1-31"];
            //                colZile = nrCol - 1;
            //            }

            //        }

            //        //ws2.Columns.AutoFit(1, 500);

            //        byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xls);


            //        DateTime ora = DateTime.Now;
            //        string numeXLS = "ExportPontaj_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";


            //        MemoryStream stream = new MemoryStream(byteArray);
            //        Response.Clear();
            //        MemoryStream ms = stream;
            //        Response.ContentType = "application/vnd.ms-excel";
            //        Response.AddHeader("content-disposition", "attachment;filename=" + numeXLS);
            //        Response.Buffer = true;
            //        ms.WriteTo(Response.OutputStream);
            //        //Response.End();

            //        btnFiltru_Click(sender, null);
            //        MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Export");
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            //    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            //}
        }

        //protected void grDate_DataBound(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var lstZile = new Dictionary<int, object>();

        //        var grid = sender as ASPxGridView;
        //        for (int i = 0; i < grid.VisibleRowCount; i++)
        //        {
        //            var rowValues = grid.GetRowValues(i, new string[] { "F10003", "ZileLucrate" }) as object[];
        //            lstZile.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
        //        }

        //        grid.JSProperties["cp_ZileLucrate"] = lstZile;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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
                    strFiltru += $" AND A.F10003 IN (SELECT F10003 FROM F100Supervizori WHERE IdUser={Session["UserId"]})";
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

                if (Constante.tipBD == 1)
                {
                    strSql = $@"SELECT *,
                        (SELECT ',Ziua' + CASE WHEN Y.Zi < CONVERT(date,X.F10022) OR CONVERT(date,X.F10023) < Y.Zi THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                        FROM F100 X
                        INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                        WHERE X.F10003=A.F10003
                        FOR XML PATH ('')) AS ZileGri
                        FROM (
                        SELECT TOP 100 PERCENT A.F10003, A.F10008 + ' ' + A.F10009 AS AngajatNume, C.Denumire AS Contract {zileVal}
                        FROM
                        (SELECT F10003 {zileAs} FROM 
                        (SELECT  A.Ziua, A.F10003, 
                        COALESCE((-1 * B.Id), (A.IdContractP * 1000 + A.IdProgramP)) AS Id
                        FROM Ptj_Intrari A
                        LEFT JOIN Ptj_tblAbsente B ON A.ValStr=B.DenumireScurta AND B.DenumireScurta <> ''
                        LEFT JOIN Ptj_Contracte C ON A.IdContractP = C.Id
                        LEFT JOIN Ptj_Programe D ON A.IdProgramP=D.Id
                        WHERE {dtInc} <= CAST(A.Ziua AS date) AND CAST(A.Ziua AS date) <= {dtSf}) AS source  
                        PIVOT (MAX(Id) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                        ) pvt
                        LEFT JOIN F100 A ON A.F10003=pvt.F10003 
                        LEFT JOIN F1001 B ON A.F10003=B.F10003 
                        LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari R WHERE {dtInc} <= CAST(R.Ziua AS date) AND CAST(R.Ziua AS date) <= {dtSf} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                        LEFT JOIN Ptj_Intrari Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                        LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                        LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                        {strLeg}
                        LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
                        LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
                        LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405
                        LEFT JOIN F005 S5 ON Y.F10006 = S5.F00506
                        LEFT JOIN F006 S6 ON Y.F10007 = S6.F00607
                        LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                        LEFT JOIN F008 S8 ON B.F100959 = S8.F00809
                        {strFiltru}
                        ORDER BY AngajatNume) A";

                    //SELECT F10003, (IdContractP * 1000 + IdProgramP) AS Prog, Ziua From Ptj_Intrari WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}




                    //         strSql = $@"with ptj_intrari_2 as (select A.* from Ptj_Intrari A 
                    //                     LEFT JOIN Ptj_Contracte C ON A.IdContract=C.Id
                    //                     LEFT JOIN F006 I ON A.F10007 = I.F00607
                    //                     {strLeg}  
                    //                     WHERE 1=1 AND {dtInc} <= A.Ziua AND A.Ziua <= {dtSf} {strFiltruSpecial})
                    //                     SELECT *,
                    //                     (SELECT ',Ziua' + CASE WHEN Y.Zi <= X.F10023 THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                    //                     FROM F100 X
                    //                     INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                    //                     WHERE X.F10003=A.F10003
                    //                     FOR XML PATH ('')) AS ZileLucrate,
                    //                     (SELECT ',Ziua' + CASE WHEN Y.Zi < CONVERT(date,X.F10022) OR CONVERT(date,X.F10023) < Y.Zi THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                    //                     FROM F100 X
                    //                     INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                    //                     WHERE X.F10003=A.F10003
                    //                     FOR XML PATH ('')) AS ZileGri
                    //                     FROM (
                    //                     SELECT TOP 100 PERCENT COALESCE({idRol},1) AS IdRol, st.Denumire AS StarePontaj, isnull(zabs.Ramase, 0) as ZileCONeefectuate, isnull(zlp.Ramase, 0) as ZLPNeefectuate, A.F100901, CAST({dtInc} AS datetime) AS ZiuaInc, 
                    //                     CONVERT(VARCHAR, A.F10022, 103) AS DataInceput, CONVERT(VARCHAR, ddp.DataPlecare, 103) AS DataSfarsit,  A.F10008 + ' ' + A.F10009 AS AngajatNume, C.Id AS IdContract, 
                    //                     Y.Norma, Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, 
                    //                     C.Denumire AS DescContract, ISNULL(C.OreSup,0) AS OreSup, ISNULL(C.Afisare,1) AS Afisare, 
                    //                     B.F100958, B.F100959,
                    //                     H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, {cmpCateg}
                    //                     ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, K.Denumire AS StareDenumire,
                    //                     A.F10078 AS Angajator, DR.F08903 AS TipContract, 
                    //                     (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                    //                     dn.Norma AS AvansNorma, 
                    //                     CASE WHEN Y.Norma <> dn.Norma THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)={an} AND MONTH(F70406)={luna}) ELSE {General.ToDataUniv(2100, 1, 1)} END AS AvansData,
                    //                     L.F06205, Fct.F71804 AS Functie,
                    //                     X.* {zileVal} {zileF}
                    //                     FROM Ptj_Cumulat X 
                    //               LEFT JOIN Ptj_tblStari st on st.Id = x.IdStare
                    //               left join SituatieZileAbsente zabs on zabs.F10003 = x.F10003 and zabs.An = x.An and zabs.IdAbsenta = (select Id from Ptj_tblAbsente where DenumireScurta = 'CO')
                    //               left join SituatieZLP zlp on zlp.F10003 = x.F10003 and zlp.An = x.An
                    //                     INNER JOIN (SELECT F10003 {zileAs} FROM 
                    //                     (SELECT F10003, {cmpValStr}, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                    //                     PIVOT  (MAX(ValStr) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                    //                     ) pvt ON X.F10003=pvt.F10003
                    //                     LEFT JOIN F100 A ON A.F10003=X.F10003 
                    //                     LEFT JOIN F1001 B ON A.F10003=B.F10003 
                    //                     LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari_2 R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                    //                     LEFT JOIN Ptj_Intrari_2 Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                    //                     LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                    //                     LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                    //                     LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                    //                     {strLeg}
                    //                     {strInner}

                    //LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
                    //LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
                    //LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

                    //LEFT JOIN F005 H ON Y.F10006 = H.F00506
                    //LEFT JOIN F006 I ON Y.F10007 = I.F00607

                    //LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                    //                     LEFT JOIN F008 S8 ON B.F100959 = S8.F00809

                    //                     LEFT JOIN F062 L ON Y.F06204Default=L.F06204

                    //                     LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                    //                     WHERE X.An = {an} AND X.Luna = {luna} {filtruPlus}
                    //                     ORDER BY AngajatNume) A
                    //                     WHERE 1=1 {strFiltru}";
                }
                else
                    strSql = $@"";
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
                DataTable dt = General.IncarcaDT(
                    $@"SELECT ContractId * 1000 + ProgramId AS IdAuto, ContractDen + ' - ' + ProgramDen AS Denumire, * FROM
                    (
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 1 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb1, A.TipSchimb0) = 1 THEN COALESCE(A.Program1, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb1, A.TipSchimb0) = 1 THEN COALESCE(A.Program1, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 2 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb2, A.TipSchimb0) = 1 THEN COALESCE(A.Program2, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb2, A.TipSchimb0) = 1 THEN COALESCE(A.Program2, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 3 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb3, A.TipSchimb0) = 1 THEN COALESCE(A.Program3, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb3, A.TipSchimb0) = 1 THEN COALESCE(A.Program3, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 4 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb4, A.TipSchimb0) = 1 THEN COALESCE(A.Program4, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb4, A.TipSchimb0) = 1 THEN COALESCE(A.Program4, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 5 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb5, A.TipSchimb0) = 1 THEN COALESCE(A.Program5, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb5, A.TipSchimb0) = 1 THEN COALESCE(A.Program5, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 6 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb6, A.TipSchimb0) = 1 THEN COALESCE(A.Program6, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb6, A.TipSchimb0) = 1 THEN COALESCE(A.Program6, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 7 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb7, A.TipSchimb0) = 1 THEN COALESCE(A.Program7, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb7, A.TipSchimb0) = 1 THEN COALESCE(A.Program7, Program0) ELSE B.IdProgram END)
                    UNION
                    SELECT A.Id AS ContractId, A.Denumire AS ContractDen, 8 AS ZiSapt, 
                    CASE WHEN COALESCE(A.TipSchimb8, A.TipSchimb0) = 1 THEN COALESCE(A.Program8, Program0) ELSE B.IdProgram END AS ProgramId,
                    C.Denumire AS ProgramDen
                    FROM Ptj_Contracte A
                    LEFT JOIN Ptj_ContracteSchimburi B ON A.Id=B.IdContract
                    LEFT JOIN Ptj_Programe C ON C.Id = (CASE WHEN COALESCE(A.TipSchimb8, A.TipSchimb0) = 1 THEN COALESCE(A.Program8, Program0) ELSE B.IdProgram END)
                    ) X
                    UNION 
                    SELECT -1 * Id, Denumire, -99, '', -99, -99, '' FROM Ptj_tblAbsente", null);


                if (dt != null && dt.Rows.Count > 0)
                {
                    string jsonPrg = "";
                    for (int g = 0; g < dt.Rows.Count; g++)
                    {
                        jsonPrg += ",{ IdAuto: " + dt.Rows[g]["IdAuto"] + ", Denumire: \"" + General.Nz(dt.Rows[g]["Denumire"], "").ToString().Trim().Replace("\n", "").Replace("\r", "") + "\", ZiSapt: " + dt.Rows[g]["ZiSapt"] + ", Contract: '" + dt.Rows[g]["ContractDen"] + "' }";
                    }
                    if (jsonPrg.Length > 0)
                        Session["Json_Programe"] = "[" + jsonPrg.Substring(1) + "]";
                }

                DataTable dtZi = General.IncarcaDT($@"SELECT CASE WHEN ZiLiberaLegala <> 0 THEN 8 ELSE ZiSapt END AS ZiSapt FROM tblZile WHERE {General.ToDataUniv(txtDtInc.Date)} <= Zi AND Zi <= {General.ToDataUniv(txtDtSf.Date)} ", null);
                var lstZiSapt = new Dictionary<int, object>();

                for (int i = 0; i < dtZi.Rows.Count; i++)
                {
                    lstZiSapt.Add(i, dtZi.Rows[i]["ZiSapt"]);
                }

                grDate.JSProperties["cp_ZiSapt"] = lstZiSapt;



                //stergem coloanele Ziua
                for (int i = 0; i < grDate.Columns.Count; i++)
                {
                    if (grDate.Columns[i].Name.Length >= 4 && grDate.Columns[i].Name.Substring(0, 4).ToLower() == "ziua")
                        grDate.Columns.Remove(grDate.Columns[i]);
                }

                //adaugam colonele din noul interval
                int cnt = 0;
                for (DateTime zi = txtDtInc.Date; zi <= txtDtSf.Date; zi = zi.AddDays(1))
                {
                    cnt++;
                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                    c.Name = "Ziua" + cnt;
                    c.FieldName = "Ziua" + cnt;
                    c.Caption = zi.Day.ToString().PadLeft(2, '0') + "." + zi.Month.ToString().PadLeft(2, '0');
                    c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                    c.PropertiesComboBox.DataSource = dt;
                    c.PropertiesComboBox.AllowNull = true;
                    c.PropertiesComboBox.ValueField = "IdAuto";
                    c.PropertiesComboBox.ValueType = typeof(int);
                    //c.PropertiesComboBox.TextFormatString = "{0}";
                    c.PropertiesComboBox.TextField = "Denumire";
                    grDate.Columns.Add(c);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void grDate_DataBound(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var lstZiSapt = new Dictionary<int, object>();

        //        var grid = sender as ASPxGridView;
        //        for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
        //        {
        //            var rowValues = grid.GetRowValues(i, new string[] { "F10003",  "ZiSapt" }) as object[];
        //            lstZiSapt.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[7] ?? "");
        //        }

        //        grid.JSProperties["cp_ZiSapt"] = lstZiSapt;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}
    }
}
 
 
 
 
 
 
 
 
 
 
 
 