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
    public partial class PontajEchipa : System.Web.UI.Page
    {

        public class metaActiuni
        {
            public int F10003 { get; set; }
            public int IdStare { get; set; }
        }

        public class metaF300
        {
            public Nullable<int> F30001 { get; set; }

            public Nullable<int> F30002 { get; set; }

            public Nullable<int> F30003 { get; set; }

            public Nullable<int> F30004 { get; set; }

            public Nullable<int> F30005 { get; set; }

            public Nullable<int> F30006 { get; set; }

            public Nullable<int> F30007 { get; set; }

            public Nullable<int> F30010 { get; set; }

            public Nullable<int> F30011 { get; set; }

            public Nullable<decimal> F30012 { get; set; }

            public Nullable<decimal> F30013 { get; set; }

            public Nullable<decimal> F30014 { get; set; }

            public Nullable<decimal> F30015 { get; set; }

            public Nullable<decimal> F30021 { get; set; }

            public Nullable<decimal> F30022 { get; set; }

            public Nullable<decimal> F30023 { get; set; }

            public string F30025 { get; set; }

            public Nullable<DateTime> F30035 { get; set; }

            public Nullable<DateTime> F30036 { get; set; }

            public Nullable<DateTime> F30037 { get; set; }

            public Nullable<DateTime> F30038 { get; set; }

            public Nullable<int> F30039 { get; set; }

            public Nullable<int> F30040 { get; set; }

            public Nullable<int> F30041 { get; set; }

            public string F30042 { get; set; }

            public Nullable<int> F30043 { get; set; }

            public Nullable<int> F30044 { get; set; }

            public Nullable<int> F30045 { get; set; }

            public Nullable<int> F30046 { get; set; }

            public Nullable<int> F30047 { get; set; }

            public Nullable<int> F30048 { get; set; }

            public Nullable<int> F30050 { get; set; }

            public Nullable<int> F30051 { get; set; }

            public int F30052 { get; set; }

            public Nullable<int> F30053 { get; set; }

            public Nullable<int> F30054 { get; set; }

            public string F300601 { get; set; }

            public string F300602 { get; set; }

            public Nullable<DateTime> F300603 { get; set; }

            public string F300604 { get; set; }

            public string F300606 { get; set; }

            public string F300607 { get; set; }

            public string F300608 { get; set; }

            public string F300609 { get; set; }

            public string F300610 { get; set; }

            public Nullable<int> F300611 { get; set; }

            public Nullable<decimal> F300612 { get; set; }

            public Nullable<int> F300613 { get; set; }

            public Nullable<decimal> F300614 { get; set; }

            public string F300615 { get; set; }

            public string F300616 { get; set; }

            public string F300617 { get; set; }

            public Nullable<int> LOCKED { get; set; }

            public Nullable<DateTime> TIME { get; set; }

            public Nullable<int> USER_NO { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Pontaj.PontajEchipa";

                //CultureInfo newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                //newCulture.NumberFormat.NumberDecimalSeparator = ".";
                //System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

                if (Convert.ToInt32(General.Nz(Session["IdClient"], "-99")) != Convert.ToInt32(IdClienti.Clienti.Chimpex))
                    divHovercard.Visible = false;

                if (!IsPostBack)
                {
                    grDate.JSProperties["cpParamMotiv"] = Dami.ValoareParam("AfisareMotivLaRespingereCerere", "0");

                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));
                    grDate.Attributes.Add("onclick", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));

                    int nrRanduri = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ", "10"));
                    grDate.SettingsPager.PageSize = nrRanduri;

                    //Adaugam f-urile
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""Vizibil"",0) = 1 ORDER BY COALESCE(""OrdineAfisare"",999999) ", null);
                    for(int i = 0; i < dt.Rows.Count; i++)
                    {
                        GridViewDataTextColumn c = new GridViewDataTextColumn();
                        c.Name = dt.Rows[i]["Coloana"].ToString();
                        c.FieldName = dt.Rows[i]["Coloana"].ToString();
                        c.Caption = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Alias"],dt.Rows[i]["Coloana"]).ToString());
                        c.ToolTip = Dami.TraduCuvant(General.Nz(dt.Rows[i]["AliasToolTip"], Dami.TraduCuvant(General.Nz(dt.Rows[i]["Alias"], dt.Rows[i]["Coloana"]).ToString())).ToString());
                        c.ReadOnly = true;
                        c.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dt.Rows[i]["Latime"], 100)));
                        c.VisibleIndex = 100 + i;

                        c.PropertiesTextEdit.DisplayFormatString = "N" + General.Nz(dt.Rows[i]["NumarZecimale"], 0);

                        grDate.Columns.Add(c);
                    }
                }
                else if(grDate.IsCallback)
                { 
                    DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStariPontaj"" ", null);
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    colStari.PropertiesComboBox.DataSource = dtStari;
                }
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
                if (Request["pp"] != null)
                    txtTitlu.Text = "Prima Pagina - Pontaj";
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Print");
                btnRespins.Text = Dami.TraduCuvant("btnRespins", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnInit.Text = Dami.TraduCuvant("btnInit", "Init");
                btnStergePontari.Text = Dami.TraduCuvant("btnStergePontari", "Sterge Pontari");
                btnTransfera.Text = Dami.TraduCuvant("btnTransfera", "Transfera");
                btnPeAng.Text = Dami.TraduCuvant("btnPeAng", "Pontaj pe Angajat");
                btnPeZi.Text = Dami.TraduCuvant("btnPeZi", "Pontaj pe Zi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnExport.Text = Dami.TraduCuvant("btnExport", "Export");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblAnLuna.InnerText = Dami.TraduCuvant("Luna/An");
                lblRol.InnerText = Dami.TraduCuvant("Roluri");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblCtr.InnerText = Dami.TraduCuvant("Contract");
                lblSub.InnerText = Dami.TraduCuvant("Subcompanie");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Dept");
                lblSubDept.InnerText = Dami.TraduCuvant("SubDept");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");
                lblCateg.InnerText = Dami.TraduCuvant("Categorie");

                //Radu 27.11.2019
                popUpExport.HeaderText = Dami.TraduCuvant("Export pontaj");
                ASPxButton btnExp = popUpExport.FindControl("btnExp") as ASPxButton;
                if (btnExp != null)
                    btnExp.Text = Dami.TraduCuvant("btnExp", "Export");
                ASPxCheckBox chkTotaluri = popUpExport.FindControl("chkTotaluri") as ASPxCheckBox;
                if (chkTotaluri != null)
                    chkTotaluri.Text = Dami.TraduCuvant("chkTotaluri", "totaluri");
                ASPxCheckBox chkOre = popUpExport.FindControl("chkOre") as ASPxCheckBox;
                if (chkOre != null)
                    chkOre.Text = Dami.TraduCuvant("chkOre", "ore intrare si iesire");
                ASPxCheckBox chkPauza = popUpExport.FindControl("chkPauza") as ASPxCheckBox;
                if (chkPauza != null)
                    chkPauza.Text = Dami.TraduCuvant("chkPauza", "pauza");
                ASPxCheckBox chkLinie = popUpExport.FindControl("chkLinie") as ASPxCheckBox;
                if (chkLinie != null)
                    chkLinie.Text = Dami.TraduCuvant("chkLinie", "afisare pe o singura linie");

                foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    txtAnLuna.Value = DateTime.Now;

                    IncarcaRoluri();
                    IncarcaAngajati();

                    SetColoane();
                    SetColoaneCuloare();

                    #region Filtru Retinut

                    if (Request.QueryString["tip"] != null)
                    {
                        NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_PontajulEchipei"] ?? "").ToString());
                        if (lst.Count > 0)
                        {
                            if (lst["An"] != "" && lst["Luna"] != "" && lst["Ziua"] != "")
                            {
                                try
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(lst["Ziua"]));
                                    txtAnLuna.Value = dt;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (General.Nz(lst["Rol"], "").ToString() != "") cmbRol.Value = Convert.ToInt32(lst["Rol"]);
                            if (General.Nz(lst["IdAng"], "").ToString() != "") cmbAng.Value = Convert.ToInt32(lst["IdAng"]);
                            if (General.Nz(lst["IdStr"], "").ToString() != "") cmbStare.Value = Convert.ToInt32(lst["IdStr"]);
                            if (General.Nz(lst["IdCtr"], "").ToString() != "") cmbCtr.Value = lst["IdCtr"];

                            if (General.Nz(lst["IdSub"], "").ToString() != "") cmbSub.Value = Convert.ToInt32(lst["IdSub"]);
                            if (General.Nz(lst["IdFil"], "").ToString() != "") cmbFil.Value = Convert.ToInt32(lst["IdFil"]);
                            if (General.Nz(lst["IdSec"], "").ToString() != "") cmbSec.Value = Convert.ToInt32(lst["IdSec"]);
                            if (General.Nz(lst["IdDpt"], "").ToString() != "") cmbDept.Value = lst["IdDpt"];
                            if (General.Nz(lst["IdSubDpt"], "").ToString() != "") cmbSubDept.Value = Convert.ToInt32(lst["IdSubDpt"]);
                            if (General.Nz(lst["IdBirou"], "").ToString() != "") cmbBirou.Value = Convert.ToInt32(lst["IdBirou"]);
                            if (General.Nz(lst["IdCateg"], "").ToString() != "") cmbCateg.Value = lst["IdCateg"];

                            btnFiltru_Click(null, null);

                            if (General.Nz(lst["IndexPag"], "").ToString() != "" && General.Nz(lst["IndexRow"], "").ToString() != "")
                            {
                                grDate.PageIndex = Convert.ToInt32(General.Nz(lst["IndexPag"], "1"));
                                grDate.FocusedRowIndex = Convert.ToInt32(General.Nz(lst["IndexRow"], "1"));
                            }
                        }
                    }

                    #endregion

                    cmbStare.DataSource = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblStariPontaj"" ", null);
                    cmbStare.DataBind();

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

                    cmbBirou.DataSource = General.IncarcaDT($"SELECT F00809, F00810 FROM F008 WHERE F00814 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00815");
                    cmbBirou.DataBind();

                    IncarcaSubcompanie();
                    IncarcaDept();

                    GetDataBlocare();
                }
                else if (pnlCtl.IsCallback)
                {
                    cmbAng.DataSource = Session["Pontaj_Angajati"];
                    cmbAng.DataBind();
                }
                else if (grDate.IsCallback)
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                RetineFiltru("1");

                string struc = "";

                string req = Convert.ToDateTime(txtAnLuna.Value).Month + ";" + Convert.ToDateTime(txtAnLuna.Value).Year + ";";

                if (cmbSub.Value != null) struc = "Subcompanie - " + cmbSub.Text;
                if (cmbFil.Value != null) struc = "Filiala - " + cmbFil.Text;
                if (cmbSec.Value != null) struc = "Sectie - " + cmbSec.Text;
                if (cmbDept.Value != null) struc = "Departament - " + cmbDept.Text;
                if (cmbSubDept.Value != null) struc = "Subdepartament - " + cmbSubDept.Text;
                if (cmbBirou.Value != null) struc = "Birou - " + cmbBirou.Text;

                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 22) Session["PrintDnata"] = DamiSelect(true);
                Session["PrintDocument"] = "PontajDinamic";
                Session["PrintParametrii"] = req + struc;
                Response.Redirect("~/Reports/Imprima.aspx?tip=30", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnPeAng_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtCol.Count > 0 && txtCol["f10003"] != null)
        //        {
        //            RetineFiltru("1");
        //            var f10003 = txtCol["f10003"];
        //            Response.Redirect("PontajDetaliat.aspx?tip=10&f10003=" + f10003, false);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Nu s-a selectat nici un angajat", MessageBox.icoInfo, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void btnPeZi_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (txtCol.Count > 0 && txtCol["coloana"] != null && txtCol["coloana"].ToString().Length >4 && txtCol["coloana"].ToString().Substring(0,4) == "Ziua")
        //        {
        //            string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");
        //            RetineFiltru(ziua);
        //            Response.Redirect("PontajDetaliat.aspx?tip=20", false);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Nu s-a selectat nici o zi", MessageBox.icoInfo, "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlCnt = $@"
                    SELECT
                    (SELECT COUNT(*) FROM F100 WHERE F10025 IN (0,999)) AS ""NrAng"",
                    (SELECT COUNT(DISTINCT F10003) FROM ""Ptj_Intrari"" WHERE {General.ToDataUniv(txtAnLuna.Date.Year, txtAnLuna.Date.Month)} <= ""Ziua"" AND ""Ziua"" <=  {General.ToDataUniv(txtAnLuna.Date.Year, txtAnLuna.Date.Month, 99)}) AS ""NrPtj"" {General.FromDual()}";

                if (General.Nz(cmbCtr.Value, "").ToString() != "")
                {
                    sqlCnt = $@"
                    SELECT
                    (SELECT COUNT(*) FROM F100 A
                    INNER JOIN ""F100Contracte"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
                    INNER JOIN ""Ptj_Contracte"" C ON B.""IdContract""=C.""Id"" AND C.""Denumire"" IN ('{cmbCtr.Value.ToString().Replace(",", "','")}')
                    WHERE A.F10025 IN (0,999)) AS ""NrAng"",
                    (SELECT COUNT(DISTINCT A.F10003) FROM ""Ptj_Intrari"" A
                    INNER JOIN ""Ptj_Contracte"" C ON A.""IdContract"" = C.""Id"" AND C.""Denumire"" IN ('{cmbCtr.Value.ToString().Replace(",", "', '")}')
                    WHERE {General.ToDataUniv(txtAnLuna.Date.Year, txtAnLuna.Date.Month)} <= A.""Ziua"" AND A.""Ziua"" <=  {General.ToDataUniv(txtAnLuna.Date.Year, txtAnLuna.Date.Month, 99)}) AS ""NrPtj"" {General.FromDual()}";
                }

                DataRow drCnt = General.IncarcaDR(sqlCnt);
                if (drCnt != null)
                {
                    decimal nrAng = Convert.ToDecimal(General.Nz(drCnt["NrAng"], 0));
                    decimal nrPtj = Convert.ToDecimal(General.Nz(drCnt["NrPtj"], 0));

                    if (nrAng != 0)
                    {
                        decimal rez = ((nrAng - nrPtj)/nrAng) * 100;
                        if (rez > 25)
                        {
                            grDate.DataSource = null;
                            grDate.DataBind();
                            MessageBox.Show("Pontajul nu este initializat." + Environment.NewLine + "Va rugam ca mai intai sa efectuati initializarea", MessageBox.icoInfo,"Initializare");
                            return;
                        }
                    }
                }

                RetineFiltru("1");
                SetColoane();
                IncarcaGrid();
                if (Convert.ToInt32(General.Nz(Session["IdClient"], "-99")) == Convert.ToInt32(IdClienti.Clienti.Chimpex))
                {
                    divHovercard.Visible = false;
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
                General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, cmbCtr.Value == null ? "" : cmbCtr.Value.ToString().Replace(",", "', '"));

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
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2) cmp = "ROWNUM";

                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);

                string strSql = $@"SELECT {cmp} AS ""IdAuto"", X.* FROM ({SelectComun()}) X 
                            WHERE X.""IdRol"" = {Convert.ToInt32(cmbRol.Value ?? -99)} AND X.F10022 <= {General.ToDataUniv(dtData.Year, dtData.Month, 99)} AND {General.ToDataUniv(dtData.Year, dtData.Month)} <= X.F10023
                            ORDER BY X.""NumeComplet"" ";

                DataTable dt = General.IncarcaDT(strSql, null);

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

        private string SelectComun()
        {
            string strSql = "";
            try
            {
                string semn = "+";
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2)
                {
                    semn = "||";
                    cmp = "ROWNUM";
                }

                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);

                strSql = @"SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
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
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
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
                                WHERE J.""IdUser"" = {0}";

                strSql = string.Format(strSql, Session["UserId"], semn, cmp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private void IncarcaRoluri()
        {
            try
            {
                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);
                //Radu 31.01.2020 - se doreste sa se puna implicit rolul cel mai mare
                string strSql = $@"SELECT X.""IdRol"", X.""RolDenumire"" FROM ({SelectComun()}) X 
                                WHERE X.F10022 <= {General.ToDataUniv(dtData.Year, dtData.Month, 99)} AND {General.ToDataUniv(dtData.Year, dtData.Month)} <= X.F10023
                                GROUP BY X.""IdRol"", X.""RolDenumire""
                                ORDER BY X.""IdRol"" DESC";

                DataTable dtRol = General.IncarcaDT(strSql, null);

                cmbRol.DataSource = dtRol;
                cmbRol.DataBind();

                if (dtRol.Rows.Count > 0) cmbRol.SelectedIndex = 0;
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
                    if (i <= DateTime.DaysInMonth(txtAnLuna.Date.Year, txtAnLuna.Date.Month))
                    {
                        GridViewDataTextColumn col = grDate.Columns[i.ToString()] as GridViewDataTextColumn;
                        col.FieldName = "Ziua" + i.ToString();
                        col.Visible = true;
                        col.ShowInCustomizationForm = true;
                    }
                    else
                    {
                        GridViewDataTextColumn col = grDate.Columns[i.ToString()] as GridViewDataTextColumn;
                        col.FieldName = "";
                        col.Visible = false;
                        col.ShowInCustomizationForm = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SetColoaneCuloare()
        {
            try
            {
                DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                if (Constante.tipBD == 1)
                    Session["ZileLibere"] = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN A.ZiSapt=6 OR A.ZiSapt=7 OR B.DAY IS NOT NULL THEN 'Ziua' + CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2)) + ';' ELSE '' END 
                                        FROM tblzile A
                                        LEFT JOIN HOLIDAYS B ON A.Zi=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month, 1)} <= A.Zi AND A.Zi <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}
                                        FOR XML PATH ('')", null), "").ToString();
                else
                    Session["ZileLibere"] = General.Nz(General.ExecutaScalar($@"SELECT LISTAGG(CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR B.DAY IS NOT NULL THEN 'Ziua' || CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2))  ELSE '' END,  ';') WITHIN GROUP (ORDER BY A.""Zi"") || ';'
                                        FROM ""tblZile"" A
                                        LEFT JOIN HOLIDAYS B ON A.""Zi""=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month, 1)} <= A.""Zi"" AND A.""Zi"" <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}", null), "").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    object col = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                    if (col != null) e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(col.ToString());
                }

                if (e.DataColumn.FieldName.Length >= 4 && e.DataColumn.FieldName.ToLower().Substring(0, 4) == "ziua")
                {
                    if (Session["ZileLibere"].ToString().Length > 0 && Session["ZileLibere"].ToString().IndexOf(e.DataColumn.FieldName + ";") >= 0)
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

        #region Transfer F300

        protected void btnTransfera_Click()
        {//Radu 13.07.2020 - modificata pentru Callback
            int rez = 1;
            int repartProcent = 0;


            DateTime ziua = Convert.ToDateTime(General.Nz(txtAnLuna.Value, DateTime.Now));
            DateTime dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1);
            if (ziua.Year != dtLucru.Year || ziua.Month != dtLucru.Month)
            {
                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Luna selectata este diferita de luna de lucru");
                return;
            }

            //Radu 16.07.2020 - transfer in F300 pe centri de cost
            string sqlCC = "SELECT F10003, F06204, SUM(NrOre1)/60 AS NrOre1, SUM(NrOre2)/60 AS NrOre2, SUM(NrOre3)/60 AS NrOre3, SUM(NrOre4)/60 AS NrOre4, SUM(NrOre5)/60 AS NrOre5, SUM(NrOre6)/60 AS NrOre6, SUM(NrOre7)/60 AS NrOre7, SUM(NrOre8)/60 AS NrOre8, SUM(NrOre9)/60 AS NrOre9, SUM(NrOre10)/60 AS NrOre10 FROM Ptj_CC WHERE MONTH(Ziua) = (SELECT F01012 FROM F010) AND YEAR(ZIUA) = (SELECT F01011 FROM F010) GROUP BY f10003, f06204, convert(varchar, month(ziua)) + convert(varchar, year(ziua))";
            if (Constante.tipBD == 2)
                sqlCC = "SELECT F10003, F06204, SUM(\"NrOre1\")/60 AS \"NrOre1\", SUM(\"NrOre2\")/60 AS \"NrOre2\", SUM(\"NrOre3\")/60 AS \"NrOre3\", SUM(\"NrOre4\")/60 AS \"NrOre4\", SUM(\"NrOre5\")/60 AS \"NrOre5\", SUM(\"NrOre6\")/60 AS \"NrOre6\", SUM(\"NrOre7\")/60 AS \"NrOre7\", SUM(\"NrOre8\")/60 AS \"NrOre8\", SUM(\"NrOre9\")/60 AS \"NrOre9\", SUM(\"NrOre10\")/60 AS \"NrOre10\" FROM \"Ptj_CC\" WHERE EXTRACT(MONTH FROM \"Ziua\") = (SELECT F01012 FROM F010) AND EXTRACT(YEAR FROM \"ZIUA\") = (SELECT F01011 FROM F010) GROUP BY f10003, f06204, TO_CHAR(EXTRACT(MONTH FROM \"Ziua\")) || TO_CHAR(EXTRACT(YEAR FROM \"ZIUA\"))";
            DataTable dtCC = General.IncarcaDT(sqlCC, null);

            if (dtCC != null && dtCC.Rows.Count > 0)
            {
                DataTable dtFor = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblAdminCC"" WHERE ""CodF300"" IS NOT NULL ORDER BY ""Ordine"" ", null);
                if (dtFor != null && dtFor.Rows.Count > 0)
                {
                    string strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30052,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
                                                VALUES(300, {19}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})";
                    if (Constante.tipBD == 1)
                        strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
                                                VALUES(300, {18}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})";

                    for (int i = 0; i < dtCC.Rows.Count; i++)
                    {
                        General.IncarcaDT(string.Format("delete from f300 where f30042 in ('WizOnePontaj_CC') and f30003 = {0} and F30050 = {1} ", dtCC.Rows[i]["F10003"].ToString(), dtCC.Rows[i]["F06204"].ToString()), null);

                        metaF300 ent = new metaF300();
                        ent.F30001 = 300;
                        ent.F30002 = 1;
                        ent.F30003 = Convert.ToInt32(dtCC.Rows[i]["F10003"].ToString());

                        ent.F30012 = 0;
                        ent.F30013 = 0;
                        ent.F30014 = 0;
                        ent.F30015 = 0;
                        ent.F30021 = 0;
                        ent.F30022 = 0;
                        ent.F30023 = 0;
                        ent.F30039 = 0;
                        ent.F30040 = 0;
                        ent.F30041 = 0;
                        ent.F30043 = 0;
                        ent.F30044 = 0;
                        ent.F30045 = 0;
                        ent.F30046 = 0;
                        ent.F30053 = 0;
                        ent.F300612 = 0;
                        ent.F300613 = 0;
                        ent.F300614 = 0;
                        ent.F30054 = 0;

                        DataTable entF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + dtCC.Rows[i]["F10003"].ToString(), null);
                        if (entF100 != null && entF100.Rows.Count > 0)
                        {
                            ent.F30002 = Convert.ToInt32(entF100.Rows[0]["F10002"].ToString());
                            ent.F30004 = Convert.ToInt32(entF100.Rows[0]["F10004"].ToString());
                            ent.F30005 = Convert.ToInt32(entF100.Rows[0]["F10005"].ToString());
                            ent.F30006 = Convert.ToInt32(entF100.Rows[0]["F10006"].ToString());
                            ent.F30007 = Convert.ToInt32(entF100.Rows[0]["F10007"].ToString());
                        }

                        ent.F30050 = Convert.ToInt32(dtCC.Rows[i]["F06204"].ToString());

                        ent.F30011 = 1;
                        ent.F30042 = "WizOnePontaj_CC";
                        ent.F30051 = 0;
                        ent.F30035 = dtLucru;
                        ent.F30036 = dtLucru;
                        ent.F30037 = dtLucru;
                        ent.F30038 = dtLucru;

                        for (int j = 0; j < dtFor.Rows.Count; j++)
                        {
                            ent.F30013 = 0;
                            ent.F30014 = 0;
                            ent.F30015 = 0;
                            ent.F30036 = dtLucru;
                            ent.F30037 = dtLucru;
                            ent.F30038 = dtLucru;

                            try
                            {                               
                                decimal val = 0;
                                try
                                {
                                    val = Convert.ToDecimal(dtCC.Rows[i][dtFor.Rows[j]["Camp"].ToString()]);
                                }
                                catch (Exception) { }

                                if (val != 0)
                                {
                                    ent.F30013 = 0;
                                    ent.F30014 = 0;
                                    ent.F30015 = 0;

                                    ent.F30010 = (short?)Convert.ToInt32(dtFor.Rows[j]["CodF300"].ToString());
                                    switch (Convert.ToInt32((dtFor.Rows[j]["SursaF300"] ?? 1).ToString()))
                                    {
                                        case 1:
                                            ent.F30013 = val;
                                            break;
                                        case 2:
                                            ent.F30014 = val * 100;
                                            ent.F30013 = 1;
                                            break;
                                        case 3:
                                            ent.F30015 = val;
                                            break;
                                    }

                                    string strTmp = "";
                                    if (Constante.tipBD == 1)
                                        strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
                                                ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
                                    else
                                        strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, Dami.NextId("F300"),
                                                ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);


                                    General.IncarcaDT(strTmp, null);
                                }
                                
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
            //end Radu 16.07.2020

            DataTable entS = Session["InformatiaCurenta"] as DataTable;

            try
            {
                if (entS == null || entS.Rows.Count == 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista date de transferat");
                    return;
                }
                else
                {
                    //Florin 2019.07.19
                    //am adaugat filtru    AND CodF300 IS NOT NULL 
                    //Florin 2019.09.30
                    //am scos conditia cu vizibil
                    DataTable entFor = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE 1=1 {General.FiltrulCuNull("CampSelect")} AND ""CodF300"" IS NOT NULL ORDER BY ""Ordine"" ", null);
                    if (entFor == null || entFor.Rows.Count == 0)
                    {
                        rez = 0;
                    }
                    else
                    {
                        string strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30052,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
                                                VALUES(300, {19}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})";
                        if (Constante.tipBD == 1)
                            strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
                                                VALUES(300, {18}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})";


                        for (int i = 0; i < entS.Rows.Count; i++)
                        {
                            General.IncarcaDT(string.Format("delete from f300 where f30042 in ('WizOnePontaj') and f30003 = {0} ", entS.Rows[i]["F10003"].ToString()), null);

                            metaF300 ent = new metaF300();
                            ent.F30001 = 300;
                            ent.F30002 = 1;
                            ent.F30003 = Convert.ToInt32(entS.Rows[i]["F10003"].ToString());

                            ent.F30012 = 0;
                            ent.F30013 = 0;
                            ent.F30014 = 0;
                            ent.F30015 = 0;
                            ent.F30021 = 0;
                            ent.F30022 = 0;
                            ent.F30023 = 0;
                            ent.F30039 = 0;
                            ent.F30040 = 0;
                            ent.F30041 = 0;
                            ent.F30043 = 0;
                            ent.F30044 = 0;
                            ent.F30045 = 0;
                            ent.F30046 = 0;
                            ent.F30053 = 0;
                            ent.F300612 = 0;
                            ent.F300613 = 0;
                            ent.F300614 = 0;
                            ent.F30054 = 0;

                            DataTable entF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + entS.Rows[i]["F10003"].ToString(), null);
                            if (entF100 != null && entF100.Rows.Count > 0)
                            {
                                ent.F30002 = Convert.ToInt32(entF100.Rows[0]["F10002"].ToString());
                                ent.F30004 = Convert.ToInt32(entF100.Rows[0]["F10004"].ToString());
                                ent.F30005 = Convert.ToInt32(entF100.Rows[0]["F10005"].ToString());
                                ent.F30006 = Convert.ToInt32(entF100.Rows[0]["F10006"].ToString());
                                ent.F30007 = Convert.ToInt32(entF100.Rows[0]["F10007"].ToString());

                                if (repartProcent == 1)
                                    ent.F30050 = 9999;
                                else
                                {
                                    //in cazul in care F10053 nu este completat, centrul de cost se preia din tabela F006, campul F00615
                                    if (General.Nz(entF100.Rows[0]["F10053"], "").ToString() == "")
                                    {
                                        DataTable entF006 = General.IncarcaDT("SELECT * FROM F006 WHERE F00607 = " + entF100.Rows[0]["F10007"].ToString(), null);
                                        if (entF006 != null && entF006.Rows.Count > 0)
                                            ent.F30050 = Convert.ToInt32(entF006.Rows[0]["F00615"].ToString());
                                        else
                                            ent.F30050 = null;
                                    }
                                    else
                                        ent.F30050 = Convert.ToInt32(entF100.Rows[0]["F10053"].ToString());
                                }
                            }

                            ent.F30011 = 1;
                            ent.F30042 = "WizOnePontaj";
                            ent.F30051 = 0;
                            ent.F30035 = dtLucru;
                            ent.F30036 = dtLucru;
                            ent.F30037 = dtLucru;
                            ent.F30038 = dtLucru;

                            for (int j = 0; j < entFor.Rows.Count; j++)
                            {
                                ent.F30013 = 0;
                                ent.F30014 = 0;
                                ent.F30015 = 0;
                                ent.F30036 = dtLucru;
                                ent.F30037 = dtLucru;
                                ent.F30038 = dtLucru;

                                try
                                {
                                    if (General.Nz(entFor.Rows[j]["TransferF300Detaliat"], "").ToString() != "")
                                    {
                                        ent.F30013 = 0;
                                        ent.F30014 = 0;
                                        ent.F30015 = 0;
                                        ent.F30010 = (short?)Convert.ToInt32(entFor.Rows[j]["CodF300"].ToString());

                                        string strInt =
                                            $@"WITH mycte
                                            AS 
                                            (
                                             SELECT A.Ziua, DATEADD(DAY, - ROW_NUMBER() OVER (PARTITION BY A.ValStr ORDER BY A.Ziua), A.Ziua) AS grp,
                                            CASE WHEN (COALESCE(B.""ZiSapt"",-99) NOT IN (6,7) AND C.DAY IS NULL) THEN 1 ELSE {General.Nz(entFor.Rows[j]["AdaugaZileLibere"], 0)} END AS Nr
                                             FROM Ptj_Intrari A
                                                LEFT JOIN ""tblZile"" B ON A.""Ziua""=B.""Zi""
                                                LEFT JOIN HOLIDAYS C ON A.""Ziua""= C.DAY
                                                WHERE A.ValStr = '{entFor.Rows[j]["TransferF300Detaliat"]}' AND A.F10003={ent.F30003} AND Year(A.Ziua)={dtLucru.Year} AND MONTH(A.Ziua)={dtLucru.Month}
                                            )    
                                            SELECT MIN(Ziua) AS DeLa, MAX(Ziua) AS La, DATEDIFF(day,MIN(Ziua),MAX(Ziua)) + 1 AS NrZile2, SUM(Nr) AS NrZile
                                            FROM mycte
                                            GROUP BY grp
                                            ORDER BY DeLa";
                                        if (Constante.tipBD == 2)
                                            strInt =
                                                $@"WITH mycte
                                                AS 
                                                (
                                                 SELECT A.""Ziua"", A.""Ziua"" - ROW_NUMBER() OVER (PARTITION BY A.""ValStr"" ORDER BY A.""Ziua"")  AS grp,
                                                    CASE WHEN (COALESCE(B.""ZiSapt"",-99) NOT IN (6,7) AND C.DAY IS NULL) THEN 1 ELSE {General.Nz(entFor.Rows[j]["AdaugaZileLibere"], 0)} END AS ""Nr""
                                                    FROM ""Ptj_Intrari"" A
                                                    LEFT JOIN ""tblZile"" B ON A.""Ziua""=B.""Zi""
                                                    LEFT JOIN HOLIDAYS C ON A.""Ziua""= C.DAY
                                                    WHERE A.""ValStr"" = '{entFor.Rows[j]["TransferF300Detaliat"]}' AND A.F10003={ent.F30003} AND TO_NUMBER(TO_CHAR(A.""Ziua"",'YYYY'))={dtLucru.Year} AND TO_NUMBER(TO_CHAR(A.""Ziua"",'MM'))={dtLucru.Month}
                                                )    
                                                SELECT MIN(""Ziua"") AS ""DeLa"", MAX(""Ziua"") AS ""La"", MAX(""Ziua"") - MIN(""Ziua"") + 1 AS ""NrZile2"", SUM(""Nr"") AS ""NrZile""
                                                FROM mycte
                                                GROUP BY grp
                                                ORDER BY ""DeLa"" ";

                                        DataTable dtInt = General.IncarcaDT(strInt, null);

                                        for (int x = 0; x < dtInt.Rows.Count; x++)
                                        {
                                            DataRow dr = dtInt.Rows[x];

                                            ent.F30013 = Convert.ToInt32(dr["NrZile"]);
                                            ent.F30036 = Convert.ToDateTime(dr["DeLa"]);
                                            ent.F30037 = Convert.ToDateTime(dr["DeLa"]);
                                            ent.F30038 = Convert.ToDateTime(dr["La"]);

                                            string strTmp = "";
                                            if (Constante.tipBD == 1)
                                                strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
                                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
                                            else
                                                strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, Dami.NextId("F300"),
                                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);

                                            General.ExecutaNonQuery(strTmp, null);
                                        }
                                    }
                                    else
                                    {
                                        decimal val = 0;
                                        try
                                        {
                                            val = Convert.ToDecimal(entS.Rows[i][entFor.Rows[j]["Coloana"].ToString()]);
                                        }
                                        catch (Exception) { }

                                        if (val != 0)
                                        {
                                            ent.F30013 = 0;
                                            ent.F30014 = 0;
                                            ent.F30015 = 0;

                                            ent.F30010 = (short?)Convert.ToInt32(entFor.Rows[j]["CodF300"].ToString());
                                            switch (Convert.ToInt32((entFor.Rows[j]["SursaF300"] ?? 1).ToString()))
                                            {
                                                case 1:
                                                    ent.F30013 = val;
                                                    break;
                                                case 2:
                                                    ent.F30014 = val * 100;
                                                    ent.F30013 = 1;
                                                    break;
                                                case 3:
                                                    ent.F30015 = val;
                                                    break;
                                            }

                                            string strTmp = "";
                                            if (Constante.tipBD == 1)
                                                strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
                                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
                                            else
                                                strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, Dami.NextId("F300"),
                                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);

                                            General.ExecutaNonQuery(strTmp);
                                        }
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                rez = 0;
                grDate.JSProperties["cpAlertMessage"] = ex;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            if (rez == 1)
            {
                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Date transferate cu succes");
            }
            else
            {
                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Eroare la transfer");
            }

        }

        #endregion

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
                        case "btnPeAng":
                            {
                                RetineFiltru("1");
                                ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=10&f10003=" + txtCol["f10003"] + "&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                            }                            
                            break;
                        case "btnPeZi":
                            {
                                string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");
                                RetineFiltru(ziua);
                                ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=20&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                            }
                            break;
                        case "grDate":
                            {
                                RetineFiltru("1");
                                ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=10&f10003=" + txtCol["f10003"] + "&Ziua=" + txtCol["coloana"] + "&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                            }
                            break;
                        case "colHide":
                            grDate.Columns[arr[1]].Visible = false;
                            break;
                        case "btnRespinge":
                            Actiuni(0, arr[1].Trim());                   
                            break;
                        case "btnValidare":
                            {
                                DataTable dt = General.IncarcaDT($"SELECT * FROM ProcesValidare({Session["UserId"]})", null);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    string raspuns = General.Nz(dt.Rows[0]["Raspuns"], "").ToString();
                                    raspuns = Dami.TraduCuvant(raspuns);

                                    switch (General.Nz(dt.Rows[0]["IdRaspuns"],"").ToString())
                                    {
                                        case "0":
                                            grDate.JSProperties["cpAlertMessage"] = raspuns;
                                            break;
                                        case "1":
                                            {
                                                string mesaj = ExecutaProcedura("ProcesAprobare");
                                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj);
                                            }
                                            break;
                                        case "2":
                                            grDate.JSProperties["cp_MesajProces"] = raspuns;
                                            break;
                                    }
                                }
                            }
                            break;
                        case "btnRefuza":
                            {
                                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                                if (lst == null || lst.Count() == 0 || lst[0] == null)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat niciun angajat!");
                                    return;
                                }
                                string ids = "";
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    ids += "," + lst[i];
                                }

                                object[] obj = new object[] { arr[1].Trim(), ids.Substring(1) };
                                string mesaj = ExecutaProcedura("ProcesRespingere", obj);
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj);
                            }
                            break;
                        case "ProcesConfirmare":
                            {
                                string mesaj = ExecutaProcedura("ProcesAprobare");
                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj);
                            }
                            break;
                        case "btnTransfera":
                            //Radu 13.07.2020
                            btnTransfera_Click();
                            break;
                        case "btnFiltru":
                            btnFiltru_Click(sender, e);
                            break;
                        case "btnAproba":
                            Actiuni(1);
                            break;
                        case "btnStergePontari":
                            btnStergePontari_Click(sender, e);
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
            try
            {
                if (!chkTotaluri.Checked && !chkOre.Checked && !chkPauza.Checked)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date"), MessageBox.icoInfo, "Export");
                }
                else
                {
                    DataTable dt = General.IncarcaDT(DamiSelectExport(), null);

                    DevExpress.Spreadsheet.Workbook book = new DevExpress.Spreadsheet.Workbook();
                    DevExpress.Spreadsheet.Worksheet ws2 = book.Worksheets["Sheet1"];

                    int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                    int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                    string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + an;
                    if (Constante.tipBD == 2)
                        strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + an;
                    DataTable dtHolidays = General.IncarcaDT(strSql, null);

                    DataTable dtCol = General.IncarcaDT("SELECT * FROM \"Ptj_tblPrint\" WHERE \"Activ\" = 1 AND \"Id\" NOT IN (2, 3, 4) ORDER BY \"Ordine\"", null);
                    Dictionary<string, string> lista = new Dictionary<string, string>();
                    Dictionary<string, string> listaLung = new Dictionary<string, string>();
                    Dictionary<string, int> listaId = new Dictionary<string, int>();
                    if (dtCol != null && dtCol.Rows.Count > 0)
                        for (int j = 0; j < dtCol.Rows.Count; j++)
                        {
                            lista.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["TextAfisare"].ToString());
                            listaLung.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["Lungime"].ToString());
                            listaId.Add(dtCol.Rows[j]["Camp"].ToString(), j + 1);
                        }

                    DataTable dtAbs = General.IncarcaDT("SELECT DISTINCT \"DenumireScurta\", max(\"Culoare\") AS \"Culoare\" FROM \"Ptj_tblAbsente\" WHERE \"IdTipOre\" = 1 GROUP BY \"DenumireScurta\"", null);
                    Dictionary<string, string> listaAbs = new Dictionary<string, string>();
                    if (dtAbs != null && dtAbs.Rows.Count > 0)
                        for (int j = 0; j < dtAbs.Rows.Count; j++)
                            listaAbs.Add(dtAbs.Rows[j]["DenumireScurta"].ToString(), dtAbs.Rows[j]["Culoare"].ToString());

                    //Radu 05.05.2020
                    DataTable dtZec = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormuleCumulat\"", null);
                    Dictionary<string, int> listaZec = new Dictionary<string, int>();
                    if (dtZec != null && dtZec.Rows.Count > 0)
                        for (int j = 0; j < dtZec.Rows.Count; j++)
                            if (!listaZec.ContainsKey(dtZec.Rows[j]["Coloana"].ToString()))
                                listaZec.Add(dtZec.Rows[j]["Coloana"].ToString(), Convert.ToInt32(General.Nz(dtZec.Rows[j]["NumarZecimale"], "0").ToString()));
                    
                    //Radu 28.02.2020 - securitate
                    List<string> listaSec = new List<string>();
                    strSql = "SELECT X.\"IdControl\", X.\"IdColoana\", MAX(X.\"Vizibil\") AS \"Vizibil\", MIN(X.\"Blocat\") AS \"Blocat\" FROM( "
                            + "SELECT A.\"IdControl\", A.\"IdColoana\", A.\"Vizibil\", A.\"Blocat\" "
                            + "FROM \"Securitate\" A "
                            + "INNER JOIN \"relGrupUser\" B ON A.\"IdGrup\" = B.\"IdGrup\" "
                            + "WHERE B.\"IdUser\" = {0} AND LOWER(A.\"IdForm\") = 'pontaj.pontajechipa' "
                            + "UNION "
                            + "SELECT A.\"IdControl\", A.\"IdColoana\", A.\"Vizibil\", A.\"Blocat\" "
                            + "FROM \"Securitate\" A "
                            + "WHERE A.\"IdGrup\" = -1 AND LOWER(A.\"IdForm\") = 'pontaj.pontajechipa') X "
                            + "GROUP BY X.\"IdControl\", X.\"IdColoana\"";
                    strSql = string.Format(strSql, Session["UserId"].ToString());
                    if (General.VarSession("EsteAdmin").ToString() == "0")
                    {
                        DataTable dtSec = General.IncarcaDT(strSql, null);
                        if (dtSec != null && dtSec.Rows.Count > 0)
                            for (int k = 0; k < dtSec.Rows.Count; k++)
                                if (dtSec.Rows[k]["Vizibil"] != null && Convert.ToInt32(dtSec.Rows[k]["Vizibil"].ToString()) == 0)
                                    listaSec.Add(dtSec.Rows[k]["IdColoana"].ToString());
                    }

                    foreach (DataColumn col in dt.Columns)
                        col.ReadOnly = false;


                    if (chkLinie.Checked)
                    {
                        int nrCol = 0;              
                        int idZile = 0, colZile = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
                            {
                                if (idZile > 0 && colZile > 0)
                                {
                                    ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = lista[dt.Columns[i].ColumnName];
                                    ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                                }
                                else
                                {
                                    //ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
                                    //ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                                    ws2.Cells[1, listaId[dt.Columns[i].ColumnName] - 1].Value = lista[dt.Columns[i].ColumnName];
                                    ws2.Cells[1, listaId[dt.Columns[i].ColumnName] - 1].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                                    if (listaId[dt.Columns[i].ColumnName] > nrCol)
                                        nrCol = listaId[dt.Columns[i].ColumnName];
                                }

                            }
                            else if (dt.Columns[i].ColumnName.Contains("Ziua"))
                            {
                                ws2.Cells[1, nrCol].Value = dt.Columns[i].ColumnName.Replace("Ziua", "");
                                DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
                                bool ziLibera = false;
                                for (int z = 0; z < dtHolidays.Rows.Count; z++)
                                    if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
                                    {
                                        ziLibera = true;
                                        break;
                                    }
                                if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[1, nrCol].FillColor = Color.FromArgb(217, 243, 253);
                                ws2.Cells[1, nrCol].ColumnWidth = Convert.ToInt32(listaLung["Zilele 1-31"]);
                                nrCol++;                          
                                idZile = listaId["Zilele 1-31"];
                                colZile = nrCol;
                            }
                        }

                        
                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            nrCol = 0;
                            idZile = 0; colZile = 0;
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
                                {
                                    int nrZec = 0;
                                    string format = "0.";
                                    if (listaZec.ContainsKey(dt.Columns[i].ColumnName))
                                    {
                                        nrZec = listaZec[dt.Columns[i].ColumnName];
                                        dt.Rows[row][i] = MathExt.Round(Convert.ToDecimal(dt.Rows[row][i].ToString()), nrZec, MidpointRounding.AwayFromZero);
                                    }
                                    if (nrZec > 0)
                                        for (int z = 1; z <= nrZec; z++)
                                            format += "#";
                                    if (idZile > 0 && colZile > 0)
                                    {
                                        if (nrZec > 0)
                                            ws2.Cells[row + 2, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].NumberFormat = format;
                                        ws2.Cells[row + 2, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = dt.Rows[row][i].ToString();
                                    }
                                    else
                                    {
                                        //if (nrZec > 0)
                                        //    ws2.Cells[row + 2, nrCol].NumberFormat = format;
                                        //ws2.Cells[row + 2, nrCol++].Value = dt.Rows[row][i].ToString();
                                        if (nrZec > 0)
                                            ws2.Cells[row + 2, listaId[dt.Columns[i].ColumnName] - 1].NumberFormat = format;
                                        ws2.Cells[row + 2, listaId[dt.Columns[i].ColumnName] - 1].Value = dt.Rows[row][i].ToString();
                                        if (listaId[dt.Columns[i].ColumnName] > nrCol)
                                            nrCol = listaId[dt.Columns[i].ColumnName];
                                    }
                                }

                                if (dt.Columns[i].ColumnName.Contains("Ziua"))
                                {
                                    ws2.Cells[row + 2, nrCol].Value = dt.Rows[row][i].ToString();
                                    DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
                                    bool ziLibera = false;
                                    for (int z = 0; z < dtHolidays.Rows.Count; z++)
                                        if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
                                        {
                                            ziLibera = true;
                                            break;
                                        }
                                    if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[row + 2, nrCol].FillColor = Color.FromArgb(217, 243, 253);
                                    if (listaAbs.ContainsKey(dt.Rows[row][i].ToString()))
                                        ws2.Cells[row + 2, nrCol].FillColor = General.Culoare(listaAbs[dt.Rows[row][i].ToString()]);
                                    if (dt.Rows[row][i] != null && dt.Rows[row]["CuloareValoare" + zi.Day].ToString().ToUpper() == Constante.CuloareModificatManual.ToUpper())
                                        ws2.Cells[row + 2, nrCol].FillColor = General.Culoare(Constante.CuloareModificatManual);
                                    nrCol++;
                                    idZile = listaId["Zilele 1-31"];
                                    colZile = nrCol;                                    

                                }
                            }
                        }
                    }
                    else
                    {
                        int nrCol = 0;
                        int rand = 0;
                        int idZile = 0, colZile = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName))
                            {
                                if (idZile > 0 && colZile > 0)
                                {
                                    ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = lista[dt.Columns[i].ColumnName];
                                    ws2.Cells[1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                                }
                                else
                                {
                                    ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
                                    ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                                }
                            }
                            else if (dt.Columns[i].ColumnName.Contains("Ziua") && !dt.Columns[i].ColumnName.Contains("I") && !dt.Columns[i].ColumnName.Contains("O") && !dt.Columns[i].ColumnName.Contains("P"))
                            {
                                ws2.Cells[1, nrCol].Value = dt.Columns[i].ColumnName.Replace("Ziua", "");
                                ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung["Zilele 1-31"]);
                                idZile = listaId["Zilele 1-31"];
                                colZile = nrCol - 1;
                            }
                        }

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            nrCol = -1;
                            idZile = 0; colZile = 0;
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                if ((lista.ContainsKey(dt.Columns[i].ColumnName) && !listaSec.Contains(dt.Columns[i].ColumnName)) || (dt.Columns[i].ColumnName.Contains("Ziua")))
                                {
                                    if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("I"))
                                        rand = 1;
                                    else if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("O"))
                                        rand = 2;
                                    else if (dt.Columns[i].ColumnName.Contains("Ziua") && dt.Columns[i].ColumnName.Contains("P"))
                                        rand = 3;
                                    else
                                    {
                                        rand = 0;
                                        nrCol++;
                                    }

                                    int nrZec = 0;
                                    string format = "0.";
                                    if (listaZec.ContainsKey(dt.Columns[i].ColumnName))
                                    {
                                        nrZec = listaZec[dt.Columns[i].ColumnName];
                                        dt.Rows[row][i] = MathExt.Round(Convert.ToDecimal(dt.Rows[row][i].ToString()), nrZec, MidpointRounding.AwayFromZero);
                                    }
                                    if (nrZec > 0)
                                        for (int z = 1; z <= nrZec; z++)
                                            format += "#";

                                    if (idZile > 0 && colZile > 0 && lista.ContainsKey(dt.Columns[i].ColumnName))
                                    {
                                        if (nrZec > 0)
                                            ws2.Cells[4 * row + 3 + rand - 1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].NumberFormat = format;
                                        ws2.Cells[4 * row + 3 + rand - 1, colZile + (listaId[dt.Columns[i].ColumnName] - idZile)].Value = dt.Rows[row][i].ToString();
                                    }
                                    else
                                    {
                                        if (nrZec > 0)
                                            ws2.Cells[4 * row + 3 + rand - 1, nrCol].NumberFormat = format;
                                        ws2.Cells[4 * row + 3 + rand - 1, nrCol].Value = dt.Rows[row][i].ToString();
                                    }

                                    if (dt.Columns[i].ColumnName.Contains("Ziua"))
                                    {
                                        DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
                                        bool ziLibera = false;
                                        for (int z = 0; z < dtHolidays.Rows.Count; z++)
                                            if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
                                            {
                                                ziLibera = true;
                                                break;
                                            }
                                        if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = Color.FromArgb(217, 243, 253);
                                        if (listaAbs.ContainsKey(dt.Rows[row][i].ToString()))
                                            ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = General.Culoare(listaAbs[dt.Rows[row][i].ToString()]);
                                        if (dt.Rows[row][i] != null && dt.Rows[row]["CuloareValoare" + zi.Day].ToString().ToUpper() == Constante.CuloareModificatManual.ToUpper())
                                            ws2.Cells[4 * row + 3 + rand - 1, nrCol].FillColor = General.Culoare(Constante.CuloareModificatManual);
                                        idZile = listaId["Zilele 1-31"];
                                        colZile = nrCol;
                                    }
                                }

                            }
                            idZile = listaId["Zilele 1-31"];
                            colZile = nrCol - 1;
                        }

                    }

                    byte[] byteArray = book.SaveDocument(DevExpress.Spreadsheet.DocumentFormat.Xls);

                    DateTime ora = DateTime.Now;
                    string numeXLS = "ExportPontaj_" + ora.Year.ToString() + ora.Month.ToString().PadLeft(2, '0') + ora.Day.ToString().PadLeft(2, '0') + "_" + ora.Hour.ToString().PadLeft(2, '0') + ora.Minute.ToString().PadLeft(2, '0') + ora.Second.ToString().PadLeft(2, '0') + ".xls";

                    MemoryStream stream = new MemoryStream(byteArray);
                    Response.Clear();
                    MemoryStream ms = stream;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment;filename=" + numeXLS);
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);

                    btnFiltru_Click(sender, null);
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Export");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnInitParam_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(txtAnLuna.Value);
                bool ras = General.PontajInit(Convert.ToInt32(Session["UserId"]), dt.Year, dt.Month, Convert.ToInt32(cmbRol.Value ?? -99), chkNormaZL.Checked, chkCCCu.Checked, General.Nz(cmbDept.Value,"").ToString(), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), General.Nz(cmbCtr.Value,"").ToString(), chkNormaSD.Checked, chkNormaSL.Checked, false, 1, Convert.ToInt32(chkInOut.Checked));
                if (ras)
                {
                    btnFiltru_Click(sender, null);
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Initializare");
                }
                else
                    MessageBox.Show(Dami.TraduCuvant("Eroare la initializare"), MessageBox.icoInfo, "Initializare");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnStergePontari_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dt = DateTime.Now;
                if (txtAnLuna.Value != null) dt = Convert.ToDateTime(txtAnLuna.Value);

                string ziInc = General.ToDataUniv(dt.Year, dt.Month, 1);
                string strDel = $@"
                    BEGIN
                        INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                        SELECT F10003, ""Ziua"", NULL, ""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare pontare angajati plecati', {Session["UserId"]}, {General.CurrentDate()}                            
                        FROM Ptj_Intrari A
                            WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10023 FROM f100 WHERE {General.TruncateDate("F10023")} >= {ziInc}  AND {General.TruncateDate("F10023")} <> {General.ToDataUniv(2100, 1, 1)}) B  WHERE B.F10003 = A.F10003  AND A.""Ziua"" > B.F10023);

                        DELETE FROM ""Ptj_Intrari""
                            WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10023 FROM f100 WHERE {General.TruncateDate("F10023")} >= {ziInc}  AND {General.TruncateDate("F10023")} <> {General.ToDataUniv(2100, 1, 1)}) B  WHERE B.F10003 = ""Ptj_Intrari"".F10003  AND ""Ptj_Intrari"".""Ziua"" > B.F10023);
                            
                        INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                        SELECT F10003, ""Ziua"", NULL, ""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare pontare angajati plecati', {Session["UserId"]}, {General.CurrentDate()}                            
                        FROM Ptj_Intrari A
                            WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10022 FROM f100 WHERE {General.TruncateDate("F10022")} >= {ziInc}  AND {General.TruncateDate("F10022")} <> {General.ToDataUniv(2100, 1, 1)}) B  WHERE B.F10003 = A.F10003  AND A.""Ziua"" < B.F10022);

                        DELETE FROM ""Ptj_Intrari""
                            WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10022 FROM f100 WHERE {General.TruncateDate("F10022")} >= {ziInc}  AND {General.TruncateDate("F10022")} <> {General.ToDataUniv(2100, 1, 1)}) B  WHERE B.F10003 = ""Ptj_Intrari"".F10003  AND ""Ptj_Intrari"".""Ziua"" < B.F10022);
                    END;";

                General.ExecutaNonQuery(strDel, null);
                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Angajati plecati");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Actiuni(int actiune, string motiv = "")
        {
            try
            {
                string msg = "";
                int idRol = -99;
                if (cmbRol.Value != null) idRol = Convert.ToInt32(cmbRol.Value);
                if ((idRol == 1 && actiune == 0) || idRol == 4 || idRol == -99)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie.");
                    return;
                }

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "IdStare" });
                if (lst == null) return;

                if (lst.Count() == 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista date selectate !");
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    msg += General.ActiuniExec(actiune, Convert.ToInt32(General.Nz(arr[0], -99)), idRol, Convert.ToInt32(General.Nz(arr[1], -99)), Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, "Pontaj.PontajEchipa", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["User_Marca"]), motiv) + System.Environment.NewLine;
                }

                if (msg != "")
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);

                grDate.Selection.UnselectAll();
                btnFiltru_Click(null, null);
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex;
               General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void RetineFiltru(string ziua)
        {
            try
            {
                string req = "";
                
                req += "&Ziua=" + ziua;
                req += "&Luna=" + Convert.ToDateTime(txtAnLuna.Value).Month;
                req += "&An=" + Convert.ToDateTime(txtAnLuna.Value).Year;

                if (cmbRol.Value != null) req += "&Rol=" + cmbRol.Value;
                if (cmbCtr.Value != null) req += "&IdCtr=" + cmbCtr.Value;
                if (cmbStare.Value != null) req += "&IdStr=" + cmbStare.Value;
                if (cmbAng.Value != null) req += "&IdAng=" + cmbAng.Value;

                if (cmbSub.Value != null) req += "&IdSub=" + cmbSub.Value;
                if (cmbFil.Value != null) req += "&IdFil=" + cmbFil.Value;
                if (cmbSec.Value != null) req += "&IdSec=" + cmbSec.Value;
                if (cmbDept.Value != null) req += "&IdDpt=" + cmbDept.Value;
                if (cmbSubDept.Value != null) req += "&IdSubDpt=" + cmbSubDept.Value;
                if (cmbBirou.Value != null) req += "&IdBirou=" + cmbBirou.Value;
                if (cmbCateg.Value != null) req += "&IdCateg=" + cmbCateg.Value;

                Session["Filtru_PontajulEchipei"] = req;
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void popUpModif_WindowCallback(object source, PopupWindowCallbackArgs e)
        {
            try
            {
                string[] arr = e.Parameter.Split(';');
                switch (arr[0])
                {
                    case "popUpModif":
                        {
                            Session["PontajEchipa_Valuri"] = null;
                            int f10003 = -99;
                            DateTime ziua = DateTime.Now;

                            if (txtCol.Count > 0 && txtCol["f10003"] != null && txtCol["f10003"] != null && General.IsNumeric(txtCol["f10003"])) f10003 = Convert.ToInt32(txtCol["f10003"]);
                            if (txtCol.Count > 0 && txtCol["coloana"] != null && txtCol["coloana"].ToString().Length > 4 && txtCol["coloana"].ToString().Substring(0, 4) == "Ziua")
                            {
                                string zi = txtCol["coloana"].ToString().Replace("Ziua", "");
                                ziua = new DateTime(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, Convert.ToInt32(zi));
                            }

                            cmbTipAbs.SelectedIndex = -1;
                            pnlValuri.Controls.Clear();

                            string strGol = "";
                            string strGol1 = "";
                            if (Constante.tipBD == 1)
                            {
                                strGol = @"AND X.""DenumireScurta"" <> '' ";
                                strGol1 = @"AND A.""OreInVal"" <> '' ";
                            }

                            //incarcam combo absente
                            //ne folosim de datele din pontaj (Contract, ziSapt, ziLibera) pt ca sunt deja determinate
                            string sqlAbs = $@"SELECT X.""Id"", X.""DenumireScurta"", X.""Denumire"", P.""ValStr"", CASE WHEN P.""ValStr""=X.""DenumireScurta"" THEN 1 ELSE 0 END AS ""Exista"" from ( 
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 0 AS ""Tip""
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""IdTipOre"" = 1
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"") x
                            INNER JOIN(SELECT * FROM ""Ptj_Intrari"" Y WHERE {General.ToDataUniv(ziua)} <= CAST(Y.""Ziua"" AS DATE) AND CAST(Y.""Ziua"" AS DATE) <= {General.ToDataUniv(ziua)} AND Y.F10003 = {f10003}) P ON 1 = 1
                            WHERE X.""DenumireScurta"" IS NOT NULL {strGol} AND X.""IdContract"" = P.""IdContract"" and X.""IdRol"" = {cmbRol.Value} AND
                            (
                                (COALESCE(X.""ZileSapt"",0) <> 0 AND COALESCE(X.""ZileSapt"",0) = (CASE WHEN P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0 THEN 1 ELSE 0 END))
                                OR
                                (COALESCE(X.S,0)  <> 0 AND COALESCE(X.S,0) = (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END))
							    OR
                                (COALESCE(X.D,0)  <> 0 AND COALESCE(X.D,0) = (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END))
                                OR
                                (COALESCE(X.SL,0) <> 0 AND COALESCE(X.SL,0) = (CASE WHEN P.""ZiSapt"" < 6 AND COALESCE(P.""ZiLiberaLegala"",0) = 1 THEN 1 ELSE 0 END))
							)
                            GROUP BY X.""Id"", X.""DenumireScurta"", X.""Denumire"", P.""ValStr"" ";

                            DataTable dtAbs = General.IncarcaDT(sqlAbs, null);

                            cmbTipAbs.DataSource = dtAbs;
                            cmbTipAbs.DataBind();

                            if (dtAbs.Rows.Count > 0 && dtAbs.Select("Exista=1").Count() > 0)
                                cmbTipAbs.Value = dtAbs.Rows[0]["ValStr"];

                            //cream textbox valuri
                            string val_uri = "";
                            for (int i = 0; i <= 20; i++)
                            {
                                val_uri += ",\"Val" + i + "\"";
                            }

                            //Florin 2020.02.04 - am inlocuit Ptj_Contracte.Afisare cu tblParametrii.TipAfisareOre
                            string sqlVal = $@"SELECT P.ValStr, COALESCE(A.""OreInVal"",'') AS ""ValAbs"", A.""DenumireScurta"", A.""Denumire"", A.""Id"", 
                                COALESCE(A.""VerificareNrMaxOre"",0) AS ""VerificareNrMaxOre"",
                                COALESCE(A.""NrMax"", 23) AS ""NrMax"" {val_uri}
                                FROM ""Ptj_tblAbsente"" a
                                INNER JOIN ""Ptj_ContracteAbsente"" b ON a.""Id"" = b.""IdAbsenta""
                                INNER JOIN ""Ptj_relRolAbsenta"" c ON a.""Id"" = c.""IdAbsenta""
                                INNER JOIN(SELECT * FROM ""Ptj_Intrari"" Y WHERE {General.ToDataUniv(ziua)} <= CAST(Y.""Ziua"" AS DATE) AND CAST(Y.""Ziua"" AS DATE) <= {General.ToDataUniv(ziua)} AND Y.F10003 = {f10003}) P ON 1 = 1
                                WHERE A.""OreInVal"" IS NOT NULL {strGol1} AND B.""IdContract"" = P.""IdContract"" AND C.""IdRol"" = {cmbRol.Value} AND
                                (
                                (COALESCE(B.ZL,0)<> 0 AND (CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0)) OR
                                (COALESCE(B.S,0) <> 0 AND (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0)) OR
                                (COALESCE(B.D,0)<> 0 AND (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0)) OR
                                (COALESCE(B.SL,0)<> 0 AND COALESCE(P.""ZiLiberaLegala"",0) = COALESCE(B.SL,0))
                                ) 
                                GROUP BY P.ValStr, A.""OreInVal"", A.""DenumireScurta"", A.""Denumire"", A.""Id"", A.""NrMax"", A.""VerificareNrMaxOre"" {val_uri}
                                ORDER BY A.""OreInVal"" ";

                            DataTable dtVal = General.IncarcaDT(sqlVal, null);

                            Session["PontajEchipa_Valuri"] = dtVal;

                            for (int i = 0; i < dtVal.Rows.Count; i++)
                            {
                                DataRow dr = dtVal.Rows[i];
                                string id = "txt" + General.Nz(dr["ValAbs"], "Gigi").ToString();

                                HtmlGenericControl divCol = new HtmlGenericControl("div");
                                divCol.Attributes["class"] = "col-md-3";
                                divCol.Style["margin-bottom"] = "15px";

                                ASPxLabel lbl = new ASPxLabel();
                                lbl.Text = General.Nz(dr["DenumireScurta"], "___").ToString();
                                lbl.ToolTip = General.Nz(dr["Denumire"], "&nbsp;").ToString();
                                lbl.Width = new Unit(100, UnitType.Percentage);

                                ASPxSpinEdit txt = new ASPxSpinEdit();
                                txt.ClientInstanceName = id;
                                txt.ID = id;
                                txt.ClientIDMode = ClientIDMode.Static;
                                txt.Width = 70;
                                txt.MinValue = 0;
                                txt.ClientSideEvents.ValueChanged = "EmptyCmbAbs";
                                txt.MaxValue = Convert.ToInt32(dr["NrMax"]);

                                string tipAfisare = Dami.ValoareParam("TipAfisareOre", "1");
                                if (tipAfisare == "1")
                                {
                                    txt.DecimalPlaces = 0;
                                    txt.NumberType = SpinEditNumberType.Integer;
                                }
                                else
                                {
                                    txt.DecimalPlaces = 2;
                                    txt.DisplayFormatString = "f2";
                                }


                                if (dtAbs.Rows.Count > 0 && dtAbs.Select("Exista=1").Count() > 0)
                                {
                                    //inseamna ca este absenta de tip zi
                                }
                                else
                                {
                                    if (General.Nz(dr["ValAbs"], "").ToString() != "")
                                    {
                                        try
                                        {
                                            int min = Convert.ToInt32(General.Nz(dr[dr["ValAbs"].ToString()], "0"));
                                            string val = "0";

                                            switch (tipAfisare)
                                            {
                                                case "1":
                                                    val = (min / 60).ToString();
                                                    break;
                                                case "2":
                                                    var ert = Math.Round(Convert.ToDecimal(min % 60) / 100, 2);
                                                    var edc = (min % 60) / 100;

                                                    val = ((min / 60) + Math.Round(Convert.ToDecimal(min % 60) / 100, 2)).ToString();
                                                    break;
                                                case "3":
                                                    val = Math.Round((Convert.ToDecimal(min) / 60), 2).ToString();
                                                    break;
                                            }

                                            txt.Text = val;
                                            txt.Value = Convert.ToDecimal(val);
                                        }
                                        catch (Exception) { }
                                    }
                                }

                                divCol.Controls.Add(lbl);
                                divCol.Controls.Add(txt);

                                pnlValuri.Controls.Add(divCol);
                            }
                        }
                        break;
                    case "":
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnModif_Click(object sender, EventArgs e)
        {
            try
            {
                int f10003 = -99;
                DateTime ziua = DateTime.Now;

                if (txtCol.Count > 0 && txtCol["f10003"] != null && txtCol["f10003"] != null && General.IsNumeric(txtCol["f10003"]))
                    f10003 = Convert.ToInt32(txtCol["f10003"]);
                else
                    return;

                if (txtCol.Count > 0 && txtCol["coloana"] != null && txtCol["coloana"].ToString().Length > 4 && txtCol["coloana"].ToString().Substring(0, 4) == "Ziua")
                {
                    string zi = txtCol["coloana"].ToString().Replace("Ziua", "");
                    ziua = new DateTime(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, Convert.ToInt32(zi));
                }
                else
                    return;

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);

                string sqlUpd = "";
                string sqlIst = "";
                string sqlValStr = "";
                string sqlDel = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr""=null,""Val0""=null,""Val1""=null,""Val2""=null,""Val3""=null,""Val4""=null,""Val5""=null,""Val6""=null,""Val7""=null,""Val8""=null,""Val9""=null,""Val10""=null,
                                ""Val11""=null,""Val12""=null,""Val13""=null,""Val14""=null,""Val15""=null,""Val16""=null,""Val17""=null,""Val18""=null,""Val19""=null,""Val20""=null
                                WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};";

                if (General.Nz(cmbTipAbs.Value, "").ToString() != "")
                {
                    sqlUpd = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" = '{cmbTipAbs.Text}', Val0=NULL, USER_NO={Session["UserId"]}, TIME={General.CurrentDate()} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};";
                }
                else
                {
                    string cmp = "";
                    int nrMin = 0;
                    string tipAfisare = Dami.ValoareParam("TipAfisareOre", "1");
                    DataTable dtVal = Session["PontajEchipa_Valuri"] as DataTable;
                    
                    string[] arrVal = null;
                    if (txtCol.Count > 0 && txtCol["valuri"] != null)
                        arrVal = txtCol["valuri"].ToString().Replace("_I=", "=").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrVal == null || arrVal.Count() == 0)
                        return;

                    for (int i =0; i < dtVal.Rows.Count; i++)
                    {
                        DataRow dr = dtVal.Rows[i];
                        string colNume = General.Nz(dr["ValAbs"], "").ToString();
                        if (txtCol["valuri"].ToString().IndexOf(colNume) < 0) continue;

                        string val = "0";
                        for(int j = 0; j < arrVal.Length; j++)
                        {
                            if (arrVal[j].IndexOf(colNume) >= 0)
                            {
                                string[] arrAtr = arrVal[i].Split('=');
                                if (arrAtr.Length == 2) val = General.Nz(arrAtr[1],0).ToString();
                                break;
                            }
                        }

                        int valCalc = 0;

                        switch (tipAfisare)
                        {
                            case "1":
                                valCalc = Convert.ToInt32((Convert.ToDecimal(val) * 60));
                                break;
                            case "2":
                                string[] v = val.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                                if (v.Length > 0) valCalc += Convert.ToInt32(v[0]) * 60;
                                if (v.Length > 1) valCalc += Convert.ToInt32(v[1]);
                                break;
                            case "3":
                                valCalc = Convert.ToInt32((Convert.ToDecimal(val) * 60));
                                break;
                        }

                        if (valCalc == 0)
                        {
                            cmp += ",\"" + colNume + "\"=NULL";
                            dt.Rows[0][colNume] = DBNull.Value;
                        }
                        else
                        {
                            cmp += ",\"" + colNume + "\"=" + valCalc.ToString();
                            dt.Rows[0][colNume] = valCalc;
                        }

                        cmp += ",\"" + colNume.Replace("Val", "ValModif") + "\"=4";
                        if (General.Nz(dr["VerificareNrMaxOre"],0).ToString() == "1") nrMin += valCalc;
                    }

                    if (cmp != "")
                    {
                        //Florin 2020.04.08 - am adaugat Valstr=null
                        sqlUpd = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr""=null {cmp}, USER_NO={Session["UserId"]}, TIME={General.CurrentDate()} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};";
                        sqlValStr = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr""={Dami.ValoareParam("SintaxaValStr")} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};";
                        sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME) 
                                        VALUES ({f10003}, {General.ToDataUniv(ziua)}, (SELECT ""ValStr"" FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}), '{General.Nz(dtVal.Rows[0]["ValStr"], "")}', {Session["UserId"]}, {General.ToDataUniv(DateTime.Now, true)}, 'Pontajul echipei - modificare pontaj', {Session["UserId"]}, {General.ToDataUniv(DateTime.Now, true)});";
                    }
                }

                string sqlPtj = General.CreazaSelectFromRow(dt.Rows[0]);
                string msg = Notif.TrimiteNotificare("Pontaj.PontajDetaliat", (int)Constante.TipNotificare.Validare, sqlPtj, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg.Substring(2));
                }
                else
                {
                    General.ExecutaNonQuery("BEGIN " + Environment.NewLine +
                        sqlDel + Environment.NewLine +
                        sqlUpd + Environment.NewLine +
                        sqlValStr + Environment.NewLine +
                        sqlIst + Environment.NewLine +
                        " END;", null);

                    if (General.Nz(cmbTipAbs.Value, "").ToString().Trim() == "")
                    {
                        DataRow dr = General.IncarcaDR($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);
                        if (dr != null)
                        {
                            Calcul.AlocaContract(f10003, ziua);
                            Calcul.CalculInOut(dr, true, true);
                        }
                    }

                    General.CalculFormule(f10003, null, ziua, null);

                    IncarcaGrid();

                    if (msg != "")
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes, dar cu urmatorul avertisment: " + msg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {
                var lstZile = new Dictionary<int, object>();

                var grid = sender as ASPxGridView;
                for (int i = 0; i < grid.VisibleRowCount; i++)
                {
                    var rowValues = grid.GetRowValues(i, new string[] { "F10003", "ZileLucrate" }) as object[];
                    lstZile.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
                }

                grid.JSProperties["cp_ZileLucrate"] = lstZile;
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
                string cmpValStr = "ValStr";
                if (ptDnata)
                {
                    cmpValStr = @"CASE 
								WHEN ValStr IN ('N') THEN '12N'
								WHEN ValStr IN ('D', 'X') THEN '12'
								WHEN ValStr IN ('Y','B','Z','F','M','G','Q','T') THEN '8'
								ELSE ValStr END AS ValStr";
                }

                int idUser = Convert.ToInt32(Session["UserId"]);
                int f10003 = Convert.ToInt32(Session["User_Marca"]);

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                int idRol = Convert.ToInt32(cmbRol.Value ?? -99);

                string dtInc = General.ToDataUniv(an, luna, 1);
                string dtSf = General.ToDataUniv(an, luna, 99);

                string zile = "";
                string zileAs = "";
                string zileVal = "";
                string zileF = "";
                string strInner = "";
                string filtruPlus = "";
                string cmpCateg = "";

                string strFiltru = "", strLeg = "";
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;

                if (General.Nz(cmbDept.Value,"").ToString() != "") strFiltru += @" AND A.""Dept"" IN ('" + cmbDept.Value.ToString().Replace(",","','") + "')";
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99) strFiltru += @" AND A.F100958=" + cmbSubDept.Value;
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99) strFiltru += @" AND A.F100959=" + cmbBirou.Value;
                if (General.Nz(cmbCtr.Value, "").ToString() != "") strFiltru += " AND A.\"DescContract\" IN ('" + cmbCtr.Value.ToString().Replace(",","','") + "')";
                string strFiltruSpecial = "";
                if (Dami.ValoareParam("PontajulEchipeiFiltruAplicat") == "1")
                    strFiltruSpecial = strFiltru.Replace("A.F1006", "C.F1006").Replace(@"A.""DescContract""",@"C.""Denumire""").Replace(@"A.""Dept""", "I.F00608");
                else
                    strLeg = "";

                string sqlCateg = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewCategoriePontaj'";
                if (Constante.tipBD == 2)
                    sqlCateg = "SELECT COUNT(*) FROM user_views where view_name = 'viewCategoriePontaj'";
                DataTable dtCateg = General.IncarcaDT(sqlCateg, null);

                if (dtCateg != null && dtCateg.Rows.Count > 0 && dtCateg.Rows[0][0] != null && Convert.ToInt32(dtCateg.Rows[0][0].ToString()) == 1)
                {
                    cmpCateg = @" CTG.""Denumire"" AS ""Categorie"", ";
                    if (General.Nz(cmbCateg.Value, "").ToString() != "")
                        filtruPlus += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                    strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                }

                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += " AND COALESCE(A.\"IdStare\",1) = " + cmbStare.Value;

                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                    strFiltru += General.GetF10003Roluri(idUser, an, luna, 0, f10003, idRol, 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;

                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                {
                    string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                    if (Constante.tipBD == 2) strZi = "TO_DATE('" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + an.ToString() + "', 'dd-mm-yyyy')";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";
                    zileVal += $@",COALESCE(""Ziua{i}"",'') AS ""Ziua{i}""";
                }

                for (int i = 1; i <= 100; i++)
                {
                    zileF += $@",CAST(COALESCE(X.""F{i}"",0) AS numeric(10)) AS ""F{i}_Tmp""";
                }


                if (Dami.ValoareParam("TipCalculDate") == "2")
                    strInner += $@"LEFT JOIN DamiNorma_Table dn ON dn.F10003=X.F10003 AND dn.dt={dtSf}
								LEFT JOIN DamiDataPlecare_Table ddp ON ddp.F10003=X.F10003 AND ddp.dt={dtSf}";
                else
                    strInner += $@"OUTER APPLY dbo.DamiNorma(X.F10003, {dtSf}) dn 
                                OUTER APPLY dbo.DamiDataPlecare(X.F10003, {dtSf}) ddp ";

                if (Constante.tipBD == 1)
                {
                    string colCumulat = General.Nz(General.ExecutaScalar(@"SELECT ',X.' + ""Coloana"" FROM ""Ptj_tblFormuleCumulat"" GROUP BY ""Coloana"" FOR XML Path('')"), "").ToString();

                    strSql = $@"with ptj_intrari_2 as (select A.* from Ptj_Intrari A 
                                LEFT JOIN Ptj_Contracte C ON A.IdContract=C.Id
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                {strLeg}  
                                WHERE 1=1 AND {dtInc} <= A.Ziua AND A.Ziua <= {dtSf} {strFiltruSpecial})
                                SELECT *,
                                (SELECT ',Ziua' + CASE WHEN Y.Zi <= X.F10023 THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                                FROM F100 X
                                INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                                WHERE X.F10003=A.F10003
                                FOR XML PATH ('')) AS ZileLucrate,
                                (SELECT ',Ziua' + CASE WHEN Y.Zi < CONVERT(date,X.F10022) OR CONVERT(date,X.F10023) < Y.Zi THEN CONVERT(nvarchar(10), DAY(Y.Zi)) END
                                FROM F100 X
                                INNER JOIN tblZile Y ON {dtInc} <= Y.Zi AND Y.Zi <= {dtSf}
                                WHERE X.F10003=A.F10003
                                FOR XML PATH ('')) AS ZileGri
                                FROM (
                                SELECT TOP 100 PERCENT COALESCE({idRol},1) AS IdRol, st.Denumire AS StarePontaj, isnull(zabs.Ramase, 0) as ZileCONeefectuate, isnull(zlp.Ramase, 0) as ZLPNeefectuate, A.F100901, CAST({dtInc} AS datetime) AS ZiuaInc, 
                                CONVERT(VARCHAR, A.F10022, 103) AS DataInceput, CONVERT(VARCHAR, ddp.DataPlecare, 103) AS DataSfarsit,  A.F10008 + ' ' + A.F10009 AS AngajatNume, C.Id AS IdContract, 
                                Y.Norma, Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, 
                                C.Denumire AS DescContract, ISNULL(C.OreSup,0) AS OreSup, ISNULL(C.Afisare,1) AS Afisare, 
                                Y.F100958, Y.F100959,
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, {cmpCateg}
                                ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, K.Denumire AS StareDenumire,
                                A.F10078 AS Angajator, DR.F08903 AS TipContract, 
                                (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                                dn.Norma AS AvansNorma, 
                                CASE WHEN Y.Norma <> dn.Norma THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)={an} AND MONTH(F70406)={luna}) ELSE {General.ToDataUniv(2100, 1, 1)} END AS AvansData,
                                L.F06205, Fct.F71804 AS Functie, X.F10003, X.IdStare {colCumulat} {zileVal} {zileF}
                                FROM Ptj_Cumulat X 
		                        LEFT JOIN Ptj_tblStari st on st.Id = x.IdStare
		                        left join SituatieZileAbsente zabs on zabs.F10003 = x.F10003 and zabs.An = x.An and zabs.IdAbsenta = (select Id from Ptj_tblAbsente where DenumireScurta = 'CO')
		                        left join SituatieZLP zlp on zlp.F10003 = x.F10003 and zlp.An = x.An
                                INNER JOIN (SELECT F10003 {zileAs} FROM 
                                (SELECT F10003, {cmpValStr}, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(ValStr) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari_2 R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN Ptj_Intrari_2 Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                                LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                                LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                {strLeg}
                                {strInner}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON Y.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON Y.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.F06204Default=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                                WHERE X.An = {an} AND X.Luna = {luna} {filtruPlus}
                                ORDER BY AngajatNume) A
                                WHERE 1=1 {strFiltru}";
                }
                else
                {
                    string colCumulat = General.Nz(General.ExecutaScalar(@"SELECT LISTAGG(',X.' || ""Coloana"") WITHIN GROUP (ORDER BY COALESCE(""Coloana"", '')) FROM (SELECT ""Coloana"" FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""Vizibil"", 0) = 1 GROUP BY ""Coloana"")"), "").ToString();

                    strSql = $@"with ""Ptj_Intrari_2"" as (select A.* from ""Ptj_Intrari"" A 
                                LEFT JOIN ""Ptj_Contracte"" C ON A.""IdContract""=C.""Id""
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                {strLeg}
                                WHERE 1=1 AND {dtInc} <= A.""Ziua"" AND A.""Ziua"" <= {dtSf} {strFiltruSpecial})
                                SELECT A.*,
                                (SELECT LISTAGG(',Ziua' || CASE WHEN Y.""Zi"" <= X.F10023 THEN TO_CHAR(EXTRACT(DAY FROM Y.""Zi"")) END) WITHIN GROUP (ORDER BY X.F10003)
                                FROM F100 X
                                INNER JOIN ""tblZile"" Y ON {dtInc} <= Y.""Zi"" AND Y.""Zi"" <= {dtSf}
                                WHERE X.F10003 = A.F10003
                                ) AS ""ZileLucrate"",
                                (SELECT LISTAGG(',Ziua' || CASE WHEN Y.""Zi"" < TRUNC(F10022) OR TRUNC(X.F10023) < Y.""Zi"" THEN TO_CHAR(EXTRACT(DAY FROM Y.""Zi"")) END) WITHIN GROUP (ORDER BY X.F10003)
                                FROM F100 X
                                INNER JOIN ""tblZile"" Y ON {dtInc} <= Y.""Zi"" AND Y.""Zi"" <= {dtSf}
                                WHERE X.F10003 = A.F10003
                                ) AS ""ZileGri""
                                FROM (
                                SELECT COALESCE({idRol},1) AS ""IdRol"", st.""Denumire"" AS ""StarePontaj"", nvl(zabs.""Ramase"", 0) as ""ZileCONeefectuate"", COALESCE(zlp.""Ramase"", 0) as ""ZLPNeefectuate"", A.F100901, {dtInc}  AS ""ZiuaInc"", 
                                A.F10022 AS ""DataInceput"", ""DamiDataPlecare""(X.F10003, {dtSf}) AS ""DataSfarsit"",  A.F10008 || ' ' || A.F10009 AS ""AngajatNume"", C.""Id"" AS ""IdContract"", 
                                Y.""Norma"", Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, 
                                C.""Denumire"" AS ""DescContract"", NVL(C.""OreSup"",0) AS ""OreSup"", NVL(C.""Afisare"",1) AS ""Afisare"", 
                                Y.F100958, Y.F100959, 
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, {cmpCateg}
                                NVL(K.""Culoare"",'#FFFFFFFF') AS ""Culoare"", K.""Denumire"" AS ""StareDenumire"",
                                A.F10078 AS ""Angajator"", DR.F08903 AS ""TipContract"", 
                                (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                                ""DamiNorma""(X.F10003, {dtSf}) AS ""AvansNorma"", 
                                CASE WHEN ""Norma"" <> ""DamiNorma""(X.F10003, {dtSf}) THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND EXTRACT(YEAR FROM F70406)={an} AND EXTRACT(MONTH FROM F70406)={luna}) ELSE {General.ToDataUniv(2100, 1, 1)} END AS ""AvansData"",
                                L.F06205, Fct.F71804 AS ""Functie"", X.F10003, X.""IdStare"" {colCumulat} {zileVal} {zileF}
                                FROM ""Ptj_Cumulat"" X 
                                LEFT JOIN ""Ptj_tblStari"" st on st.""Id"" = x.""IdStare""
                                left join (SELECT * FROM ""SituatieZileAbsente"" WHERE ""IdAbsenta"" = (select ""Id"" from ""Ptj_tblAbsente"" where ""DenumireScurta"" = 'CO')) zabs on zabs.F10003 = x.F10003 and zabs.""An"" = x.""An""
                                left join ""SituatieZLP"" zlp on zlp.F10003 = x.F10003 and zlp.""An"" = x.""An""
                                INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""ValStr"", TO_CHAR(""Ziua"",'DD-MM-YYYY') AS ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf})  source  
                                PIVOT  (MAX(COALESCE(""ValStr"",'')) FOR ""Ziua"" IN ( {zileAs.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003
                                LEFT JOIN F100 A ON A.F10003=X.F10003
                                LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari_2"" R WHERE EXTRACT(YEAR FROM R.""Ziua"")= {an} AND EXTRACT(MONTH FROM R.""Ziua"")= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN ""Ptj_Intrari_2"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                                LEFT JOIN ""Ptj_tblStariPontaj"" K ON K.""Id"" = NVL(X.""IdStare"",1) 
                                LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                               {strLeg}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON Y.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON Y.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.""F06204Default""=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802
                                
                                WHERE X.""An""= {an} AND X.""Luna"" = {luna} {filtruPlus}
                                ORDER BY ""AngajatNume"") A
                                WHERE 1=1 {strFiltru}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string DamiSelectExport()
        {
            string strSql = "";

            try
            {
                string cmpValStr = @"""ValStr""";

                int idUser = Convert.ToInt32(Session["UserId"]);
                int f10003 = Convert.ToInt32(Session["User_Marca"]);

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                int luna = Convert.ToDateTime(txtAnLuna.Value).Month;

                string dtInc = General.ToDataUniv(an, luna, 1);
                string dtSf = General.ToDataUniv(an, luna, 99);

                string zile = "";
                string zileAs = "", zileAsIn = "", zileAsOut = "", zileAsPauza = "", zileAsCuloare = "";
                string zileVal = "";
                string zileF = "";
                string strInner = "";
                string filtruPlus = "";
                string cmpCateg = "";

                string pvt = "", pvtIn = "", pvtOut = "", pvtPauza = "", pvtCuloare = "";

                string strFiltru = "", strLeg = "";
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (General.Nz(cmbDept.Value, "").ToString() != "") strFiltru += @" AND A.""Dept"" IN ('" + cmbDept.Value.ToString().Replace(",", "','") + "')";

                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99) strFiltru += @" AND A.F100958=" + cmbSubDept.Value;
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99) strFiltru += @" AND A.F100959=" + cmbBirou.Value;
                if (General.Nz(cmbCtr.Value, "").ToString() != "") strFiltru += " AND A.\"DescContract\" IN ('" + cmbCtr.Value.ToString().Replace(",", "','") + "')";

                string strFiltruSpecial = "";
                if (Dami.ValoareParam("PontajulEchipeiFiltruAplicat") == "1")
                {
                    strFiltruSpecial = strFiltru.Replace("A.F10095", "Z.F10095").Replace("A.F1006", "C.F1006").Replace(@"A.""DescContract""", @"C.""Denumire""").Replace(@"A.""Dept""", "I.F00608");
                }
                else
                    strLeg = "";

                string sqlCateg = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'viewCategoriePontaj'";
                if (Constante.tipBD == 2)
                    sqlCateg = "SELECT COUNT(*) FROM user_views where view_name = 'viewCategoriePontaj'";
                DataTable dtCateg = General.IncarcaDT(sqlCateg, null);
                if (dtCateg != null && dtCateg.Rows.Count > 0 && dtCateg.Rows[0][0] != null && Convert.ToInt32(dtCateg.Rows[0][0].ToString()) == 1)
                {
                    cmpCateg = @" CTG.""Denumire"" AS ""Categorie"", ";
                    if (General.Nz(cmbCateg.Value, "").ToString() != "")
                        filtruPlus += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                    strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                }

                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += " AND COALESCE(A.\"IdStare\",1) = " + cmbStare.Value;
                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                {//Radu 04.02.2020
                    if (chkRoluri.Checked)
                    {
                        List<int> lstRoluri = new List<int>();
                        foreach (ListEditItem val in cmbRol.Items)
                            lstRoluri.Add(Convert.ToInt32(val.Value));
                        strFiltru += General.GetF10003RoluriComasate(idUser, an, luna, f10003, lstRoluri, 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                    }
                    else
                        strFiltru += General.GetF10003Roluri(idUser, an, luna, 0, f10003, Convert.ToInt32(cmbRol.Value ?? -99), 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                }
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;


                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                {
                    string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                    if (Constante.tipBD == 2) strZi = "TO_DATE('" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + an.ToString() + "', 'dd-mm-yyyy')";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";

                    if (chkTotaluri.Checked)
                        zileVal += $@",COALESCE(pvt.""Ziua{i}"",'') AS ""Ziua{i}"", pvtCuloare.""CuloareValoare{i}"" AS ""CuloareValoare{i}""";
                    if (chkOre.Checked)
                    {
                        if (Constante.tipBD == 1)
                            zileVal += $@",COALESCE(CONVERT(VARCHAR(5), pvtIn.""Ziua{i}I"", 108),'') AS ""Ziua{i}I""" + $@",COALESCE(CONVERT(VARCHAR(5), pvtOut.""Ziua{i}O"", 108),'') AS ""Ziua{i}O""";
                        else
                            zileVal += $@",TO_CHAR(pvtIn.""Ziua{i}I"", 'HH24:mi') AS ""Ziua{i}I""" + $@",TO_CHAR(pvtOut.""Ziua{i}O"", 'HH24:mi') AS ""Ziua{i}O""";
                    }
                    if (chkPauza.Checked)
                    {
                        if (Constante.tipBD == 1)
                            zileVal += $@",COALESCE(pvtPauza.""Ziua{i}P"",'') AS ""Ziua{i}P""";
                        else
                            zileVal += $@",COALESCE(TO_CHAR(pvtPauza.""Ziua{i}P""),'') AS ""Ziua{i}P""";
                    }


                    zileAsIn += ", " + strZi + " AS \"Ziua" + i.ToString() + "I\"";
                    zileAsOut += ", " + strZi + " AS \"Ziua" + i.ToString() + "O\"";
                    zileAsPauza += ", " + strZi + " AS \"Ziua" + i.ToString() + "P\"";
                    zileAsCuloare += ", " + strZi + " AS \"CuloareValoare" + i.ToString() + "\"";
                }

                for (int i = 1; i <= 100; i++)
                {
                    zileF += $@",COALESCE(X.""F{i}"",0) AS ""F{i}""";
                }

                //Florin 2020.05.21
                string cmpExpIn = "FirstInPaid";
                string cmpExpOut = "LastOutPaid";
                string tipExp = Dami.ValoareParam("InOutInExportPontaj");
                switch(tipExp)
                {
                    case "1":
                        cmpExpIn = "FirstIn";
                        cmpExpOut = "LastOut";
                        break;
                    case "2":
                        cmpExpIn = "FirstInRap";
                        cmpExpOut = "LastOutRap";
                        break;
                    case "4":
                        cmpExpIn = "FirstInPaid";
                        cmpExpOut = "LastOutPaid";
                        break;
                }

                if (Constante.tipBD == 1)
                {
                    if (chkTotaluri.Checked)
                        pvt = $@"INNER JOIN (SELECT F10003 {zileAs} FROM 
                                (SELECT F10003, {cmpValStr}, ""Ziua"" FROM ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT  (MAX(""ValStr"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003";

                    if (chkOre.Checked)
                    {
                        pvtIn = $@"INNER JOIN (SELECT F10003 {zileAsIn} FROM 
                                (SELECT F10003, ""{cmpExpIn}"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""{cmpExpIn}"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvtIn ON X.F10003=pvtIn.F10003";

                        pvtOut = $@"INNER JOIN (SELECT F10003 {zileAsOut} FROM 
                                (SELECT F10003, ""{cmpExpOut}"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""{cmpExpOut}"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvtOut ON X.F10003=pvtOut.F10003";
                    }
                    if (chkPauza.Checked)
                        pvtPauza = $@"INNER JOIN (SELECT F10003 {zileAsPauza} FROM 
                                (SELECT F10003, ""TimpPauzaReal"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""TimpPauzaReal"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvtPauza ON X.F10003=pvtPauza.F10003";

                    pvtCuloare = $@"INNER JOIN (SELECT F10003 {zileAsCuloare} FROM 
                                (SELECT F10003, ""CuloareValoare"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""CuloareValoare"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvtCuloare ON X.F10003=pvtCuloare.F10003";
                }
                else
                {
                    if (chkTotaluri.Checked)
                        pvt = $@"INNER JOIN (SELECT * FROM 
                                (SELECT F10003, {cmpValStr}, ""Ziua"" FROM ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT  (MAX(""ValStr"") FOR ""Ziua"" IN ( {zileAs.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003";

                    if (chkOre.Checked)
                    {
                        pvtIn = $@"INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""{cmpExpIn}"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""{cmpExpIn}"") FOR ""Ziua"" IN ( {zileAsIn.Substring(1)} )) pvt
                                ) pvtIn ON X.F10003=pvtIn.F10003";

                        pvtOut = $@"INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""{cmpExpOut}"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""{cmpExpOut}"") FOR ""Ziua"" IN ( {zileAsOut.Substring(1)} )) pvt
                                ) pvtOut ON X.F10003=pvtOut.F10003";
                    }
                    if (chkPauza.Checked)
                        pvtPauza = $@"INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""TimpPauzaReal"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""TimpPauzaReal"") FOR ""Ziua"" IN ( {zileAsPauza.Substring(1)} )) pvt
                                ) pvtPauza ON X.F10003=pvtPauza.F10003";

                    pvtCuloare = $@"INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""CuloareValoare"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf}) source  
                                PIVOT (MAX(""CuloareValoare"") FOR ""Ziua"" IN ( {zileAsCuloare.Substring(1)} )) pvt
                                ) pvtCuloare ON X.F10003=pvtCuloare.F10003";
                }

                if (Dami.ValoareParam("TipCalculDate") == "2")
                    strInner += $@"LEFT JOIN DamiDataPlecare_Table ddp ON ddp.F10003=X.F10003 AND ddp.dt={dtSf}";
                else
                    strInner += $@"OUTER APPLY dbo.DamiDataPlecare(X.F10003, {dtSf}) ddp ";

                if (Constante.tipBD == 1)
                    strSql = $@"with ptj_intrari_2 as (select A.* from Ptj_Intrari A 
                                LEFT JOIN Ptj_Contracte C ON A.IdContract=C.Id
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                {strLeg}  
                                WHERE 1=1 AND {dtInc} <= A.Ziua AND A.Ziua <= {dtSf} {strFiltruSpecial})
                                SELECT *
                                FROM (
                                 SELECT TOP 100 PERCENT X.F10003, CONVERT(VARCHAR, A.F10022, 103) AS DataInceput, convert(VARCHAR, ddp.DataPlecare, 103) AS DataSfarsit, A.F10008 + ' ' + A.F10009 AS AngajatNume, st.Denumire AS StarePontaj, isnull(zabs.Ramase, 0) as ZileCONeefectuate, isnull(zlp.Ramase, 0) as ZLPNeefectuate,
                                 H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, F06205, {cmpCateg}
                                Y.F100958, Y.F100959, Y.IdContract, A.F100901 AS EID, X.IdStare , Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, C.Denumire AS DescContract, X.Comentarii
                                {zileVal}  {zileF}
                                FROM Ptj_Cumulat X 
		                        LEFT JOIN Ptj_tblStari st on st.Id = x.IdStare
		                        left join SituatieZileAbsente zabs on zabs.F10003 = x.F10003 and zabs.An = x.An and zabs.IdAbsenta = (select Id from Ptj_tblAbsente where DenumireScurta = 'CO')
		                        left join SituatieZLP zlp on zlp.F10003 = x.F10003 and zlp.An = x.An
                                {pvt}
                                {pvtIn}
                                {pvtOut}
                                {pvtPauza}
                                {pvtCuloare}
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari_2 R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN Ptj_Intrari_2 Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                                LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                                LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                {strLeg}
                                {strInner}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON Y.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON Y.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.F06204Default=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                                WHERE X.An = {an} AND X.Luna = {luna} {filtruPlus}
                                ORDER BY NumeComplet) A
                                WHERE 1=1 {strFiltru}";
                else
                    strSql = $@"with ""Ptj_Intrari_2"" as (select A.* from ""Ptj_Intrari"" A 
                                LEFT JOIN ""Ptj_Contracte"" C ON A.""IdContract""=C.""Id""
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                {strLeg}
                                WHERE 1=1 AND {dtInc} <= A.""Ziua"" AND A.""Ziua"" <= {dtSf} {strFiltruSpecial})
                                SELECT  *                                
                                FROM (
                                SELECT X.F10003, TO_CHAR(A.F10022, 'dd/mm/yyyy') AS ""DataInceput"", TO_CHAR(""DamiDataPlecare""(X.F10003, {dtSf}), 'dd/mm/yyyy') AS ""DataSfarsit"", A.F10008 || ' ' || A.F10009 AS ""AngajatNume"", st.""Denumire"" AS ""StarePontaj"", COALESCE(zabs.""Ramase"", 0) as ""ZileCONeefectuate"", COALESCE(zlp.""Ramase"", 0) as ""ZLPNeefectuate"",
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, F06205, {cmpCateg}
                                Y.F100958, Y.F100959, Y.""IdContract"", A.F100901 AS EID, X.""IdStare"" , Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, C.""Denumire"" AS ""DescContract"", X.""Comentarii""
                                {zileVal} {zileF}
                                FROM ""Ptj_Cumulat"" X 
		                        LEFT JOIN ""Ptj_tblStari"" st on st.""Id"" = x.""IdStare""
		                        left join (SELECT * FROM ""SituatieZileAbsente"" WHERE ""IdAbsenta"" = (select ""Id"" from ""Ptj_tblAbsente"" where ""DenumireScurta"" = 'CO')) zabs on zabs.F10003 = x.F10003 and zabs.""An"" = x.""An""
                                left join ""SituatieZLP"" zlp on zlp.F10003 = x.F10003 and zlp.""An"" = x.""An""
                                {pvt}
                                {pvtIn}
                                {pvtOut}
                                {pvtPauza}
                                {pvtCuloare}
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari"" R WHERE EXTRACT(YEAR FROM R.""Ziua"")= {an} AND EXTRACT(MONTH FROM R.""Ziua"")= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN ""Ptj_Intrari"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                                LEFT JOIN ""Ptj_tblStariPontaj"" K ON K.""Id"" = NVL(X.""IdStare"",1) 
                                LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                {strLeg}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON Y.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON Y.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.""F06204Default""=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                                WHERE X.""An""= {an} AND X.""Luna"" = {luna} {filtruPlus}
                                ORDER BY ""NumeComplet"") A
                                WHERE 1=1 {strFiltru}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private string ExecutaProcedura(string numeProcedura, object[] obj = null)
        {
            string mesaj = "Eroare la executia procedurii";
            try
            {
                
                using (SqlConnection conn = new SqlConnection(Constante.cnnWeb))
                using (SqlCommand cmd = new SqlCommand(numeProcedura, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    switch(numeProcedura)
                    {
                        case "ProcesRespingere":
                            if (obj.Length == 2)
                            {
                                cmd.Parameters.AddWithValue("@comentariu", obj[0]);
                                cmd.Parameters.AddWithValue("@ids", obj[1]);
                            }
                            else
                                return mesaj;
                            break;
                    }
                    cmd.Parameters.AddWithValue("@idUser", General.Nz(Session["UserId"],-99));
                    cmd.Parameters.Add("@mesaj", SqlDbType.NVarChar, 500).Direction = ParameterDirection.Output;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    mesaj = General.Nz(cmd.Parameters["@mesaj"].Value,"").ToString();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return mesaj;
        }

        protected void grDateIstoric_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');   

                    switch (arr[0].ToString())
                    {
                        case "btnIstoricAprobare":
                            IncarcaGridIstoric();
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

        //Radu 27.07.2020
        private void IncarcaGridIstoric()
        {
            string strSql = @"SELECT A.""IdAuto"", A.F10003, A.""An"", A.""Luna"", A.""IdSuper"", A.""IdStare"", D.""Culoare"", A.""DataAprobare"", D.""Denumire"" AS ""NumeStare"",
                            CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {3} ' ' {3} C.F10009 END) END as ""Nume""
                            FROM ""Ptj_CumulatIstoric"" A
                            LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
                            LEFT JOIN F100 C ON B.F10003 = C.F10003
                            LEFT JOIN ""Ptj_tblStariPontaj"" D ON A.""IdStare""=D.""Id""
                            WHERE A.""IdStare"" <>1 AND A.F10003 = {0} AND ""An""={1} AND A.""Luna""={2}
                            ORDER BY A.""DataAprobare""";

            string op = "+";
            if (Constante.tipBD == 2) op = "||";

            List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "AngajatNume" });
            if (lst == null || lst.Count() == 0 || lst[0] == null) return;
            object[] arr = lst[0] as object[];

            DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);

            strSql = string.Format(strSql, arr[0].ToString(), ziua.Year, ziua.Month, op);

            grDateIstoric.DataSource = General.IncarcaDT(strSql, null);
            grDateIstoric.DataBind();

        }

        #region callback

        protected void cmbAng_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                IncarcaAngajati();
                if (e.Parameter == "txtAnLuna")
                    SetColoaneCuloare();

                if (e.Parameter == "cmbRol")
                    GetDataBlocare();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbFil_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                IncarcaFiliala();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbSec_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                IncarcaSectie();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbSubDept_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                IncarcaSubdept();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void listBox_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                IncarcaDept();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        #region Incarca Struct. Org.

        private void IncarcaSubcompanie()
        {
            try
            {
                cmbSub.DataSource = General.IncarcaDT($@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003 WHERE F00310 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00311");
                cmbSub.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaFiliala()
        {
            try
            {
                cmbFil.DataSource = General.IncarcaDT($@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404={General.Nz(cmbSub.Value, -99)} AND F00411 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00412");
                cmbFil.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        
        private void IncarcaSectie()
        {
            try
            {
                cmbSec.DataSource = General.IncarcaDT($@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505={General.Nz(cmbFil.Value, -99)} AND F00513 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00514");
                cmbSec.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaDept()
        {
            try
            {
                ASPxListBox lstDept = cmbDept.FindControl("listBox") as ASPxListBox;
                if (lstDept == null) return;

                string filtru = "";
                if (cmbSec.Value != null) filtru = $@" AND F00606={General.Nz(cmbSec.Value, -99)}";

                lstDept.DataSource = General.IncarcaDT(
                    $@"SELECT -5 AS ""IdDept"", 'Select All' AS ""Dept"" {General.FromDual()} 
                    UNION
                    SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00622 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00623 {filtru}");
                lstDept.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaSubdept()
        {
            try
            {
                cmbSubDept.DataSource = General.IncarcaDT(
                    $@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 
                    INNER JOIN F006 ON F007.F00707=F006.F00607 
                    WHERE F00608 IN ('{General.Nz(cmbDept.Value, -99).ToString().Replace(",", "','")}')
                    AND F00714 <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= F00715");
                cmbSubDept.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        #endregion

        private void GetDataBlocare()
        {
            try
            {
                string dataBlocare = "22001231";
                DataTable dt = General.IncarcaDT($@"SELECT ""Ziua"" FROM ""Ptj_tblBlocarePontaj"" WHERE ""IdRol""=@1", new object[] { Convert.ToInt32(cmbRol.Value ?? -99) });
                if (dt != null && dt.Rows.Count > 0 && General.Nz(dt.Rows[0][0], "").ToString() != "" && General.IsDate(dt.Rows[0][0]))
                    dataBlocare = Convert.ToDateTime(dt.Rows[0][0]).Year + Convert.ToDateTime(dt.Rows[0][0]).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(dt.Rows[0][0]).Day.ToString().PadLeft(2, '0');

                Session["Ptj_DataBlocare"] = dataBlocare.ToString();
                grDate.JSProperties["cpDataBlocare"] = dataBlocare.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}