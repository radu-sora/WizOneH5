using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajEchipa : System.Web.UI.Page
    {


        //Radu
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

        //end Radu


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));
                    grDate.Attributes.Add("onclick", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));


                    //Adaugam f-urile
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""Vizibil"",0) = 1 ORDER BY ""Ordine"" ", null);
                    for(int i =0; i< dt.Rows.Count; i++)
                    {
                        //GridViewDataColumn c = new GridViewDataColumn();
                        GridViewDataTextColumn c = new GridViewDataTextColumn();
                        c.Name = dt.Rows[i]["Coloana"].ToString();
                        c.FieldName = dt.Rows[i]["Coloana"].ToString();
                        c.Caption = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Alias"],dt.Rows[i]["Coloana"]).ToString());
                        c.ReadOnly = true;
                        c.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dt.Rows[i]["Latime"], 100)));
                        c.VisibleIndex = 100 + i;

                        //c.PropertiesEdit.DisplayFormatString = "N0";
                        c.PropertiesTextEdit.DisplayFormatString = "N0";

                        grDate.Columns.Add(c);
                    }
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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Print");
                btnInit.Text = Dami.TraduCuvant("btnInit", "Init");
                btnTransfera.Text = Dami.TraduCuvant("btnTransfera", "Transfera");
                btnPeAng.Text = Dami.TraduCuvant("btnPeAng", "Pontaj pe Angajat");
                btnPeZi.Text = Dami.TraduCuvant("btnPeZi", "Pontaj pe Zi");


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

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;

                    txtAnLuna.Value = DateTime.Now;
                    
                    IncarcaRoluri();
                    IncarcaAngajati();

                    SetColoane();

                    grDate.JSProperties["cpAngajatiPlecatiCuPontari"] = AngajatiPlecatiCuPontari(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month);

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
                            if (General.Nz(lst["IdCtr"], "").ToString() != "") cmbCtr.Value = Convert.ToInt32(lst["IdCtr"]);

                            if (General.Nz(lst["IdSub"], "").ToString() != "") cmbSub.Value = Convert.ToInt32(lst["IdSub"]);
                            if (General.Nz(lst["IdFil"], "").ToString() != "") cmbFil.Value = Convert.ToInt32(lst["IdFil"]);
                            if (General.Nz(lst["IdSec"], "").ToString() != "") cmbSec.Value = Convert.ToInt32(lst["IdSec"]);
                            if (General.Nz(lst["IdDpt"], "").ToString() != "") cmbDept.Value = Convert.ToInt32(lst["IdDpt"]);
                            if (General.Nz(lst["IdSubDpt"], "").ToString() != "") cmbSubDept.Value = Convert.ToInt32(lst["IdSubDpt"]);
                            if (General.Nz(lst["IdBirou"], "").ToString() != "") cmbBirou.Value = Convert.ToInt32(lst["IdBirou"]);

                            btnFiltru_Click(null, null);
                        }
                    }
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

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();

                }

                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                cmbDept.DataBind();
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT("SELECT F00809, F00810 FROM F008", null);
                cmbBirou.DataBind();

                cmbStare.DataSource = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblStariPontaj"" ", null);
                cmbStare.DataBind();

                cmbCtr.DataSource = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte"" ", null);
                cmbCtr.DataBind();



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string struc = "";

                string req = Convert.ToDateTime(txtAnLuna.Value).Month + ";" + Convert.ToDateTime(txtAnLuna.Value).Year + ";";

                if (cmbSub.Value != null) struc = "Subcompanie - " + cmbSub.Text;
                if (cmbFil.Value != null) struc = "Filiala - " + cmbFil.Text;
                if (cmbSec.Value != null) struc = "Sectie - " + cmbSec.Text;
                if (cmbDept.Value != null) struc = "Departament - " + cmbDept.Text;
                if (cmbSubDept.Value != null) struc = "Subdepartament - " + cmbSubDept.Text;
                if (cmbBirou.Value != null) struc = "Birou - " + cmbBirou.Text;

                Session["PrintDocument"] = "PontajDinamic";
                Session["PrintParametrii"] = req + struc;
                Response.Redirect("~/Reports/Imprima.aspx", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPeAng_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCol.Count > 0 && txtCol["f10003"] != null)
                {
                    string req = "";
                    var f10003 = txtCol["f10003"];

                    req += "&Ziua=" + 1;
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

                    Session["Filtru_PontajulEchipei"] = req;

                    //if (req != "") req = "?" + req.Substring(1);
                    //Response.Redirect("PontajDetaliat.aspx" + req);

                    Response.Redirect("PontajDetaliat.aspx?tip=10&f10003=" + f10003, false);
                }
                else
                {
                    MessageBox.Show("Nu s-a selectat nici un angajat", MessageBox.icoInfo, "Atentie !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnPeZi_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCol.Count > 0 && txtCol["coloana"] != null && txtCol["coloana"].ToString().Length >4 && txtCol["coloana"].ToString().Substring(0,4) == "Ziua")
                {
                    string req = "";
                    string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");

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

                    Session["Filtru_PontajulEchipei"] = req;

                    //if (req != "") req = "?" + req.Substring(1);
                    //Response.Redirect("PontajDetaliat.aspx" + req);

                    Response.Redirect("PontajDetaliat.aspx?tip=20", false);
                }
                else
                {
                    MessageBox.Show("Nu s-a selectat nici o zi", MessageBox.icoInfo, "Atentie !");
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
                SetColoane();
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
                cmbAng.Value = null;
                cmbSub.Value = null;
                cmbSec.Value = null;
                cmbFil.Value = null;
                cmbDept.Value = null;
                cmbSubDept.Value = null;
                cmbBirou.Value = null;
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
                Response.Redirect("~/Absente/Lista.aspx", false);
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
                }

                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                cmbDept.DataBind();
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
            }
            catch (Exception)
            {

                throw;
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

                grDate.JSProperties["cpAngajatiPlecatiCuPontari"] = AngajatiPlecatiCuPontari(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month);
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
                int idUser = Convert.ToInt32(Session["UserId"]);
                int f10003 = Convert.ToInt32(Session["User_Marca"]);

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                int idRol = Convert.ToInt32(cmbRol.Value ?? -99);

                //int idSub = Convert.ToInt32(cmbSub.Value ?? -99);
                //int idFil = Convert.ToInt32(cmbFil.Value ?? -99);
                //int idSec = Convert.ToInt32(cmbSec.Value ?? -99);
                //int idDpt = Convert.ToInt32(cmbDept.Value ?? -99);
                //int idSubDpt = Convert.ToInt32(cmbSubDept.Value ?? -99);
                //int idBirou = Convert.ToInt32(cmbBirou.Value ?? -99);

                //int idCtr = Convert.ToInt32(cmbCtr.Value ?? -99);
                //int idStr = Convert.ToInt32(cmbStare.Value ?? -99);
                //int idAng = Convert.ToInt32(cmbAng.Value ?? -99);



                string dtInc = General.ToDataUniv(an,luna,1);
                string dtSf = General.ToDataUniv(an, luna, 99);

                string zile = "";
                string zileAs = "";
                string zileVal = "";

                string strFiltru = "";
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (Convert.ToInt32(cmbDept.Value ?? -99) != -99) strFiltru += " AND A.F10007 = " + cmbDept.Value;
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99) strFiltru += " AND A.F100958 = " + cmbSubDept.Value;
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99) strFiltru += " AND A.F100959 = " + cmbBirou.Value;

                if (Convert.ToInt32(cmbCtr.Value ?? -99) != -99) strFiltru += " AND A.IdContract = " + cmbCtr.Value;
                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += " AND COALESCE(A.IdStare,1) = " + cmbStare.Value;
                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                    strFiltru += General.GetF10003Roluri(idUser, an, luna, 0, f10003, idRol, 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;


                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                {
                    string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                    if (Constante.tipBD == 2) strZi = "'" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + Dami.NumeLuna(luna, 1, "EN") + "-" + an.ToString().Substring(2) + "'";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";
                    zileVal += ",\"Ziua" + i.ToString() + "\"";
                }

                strSql = $@"SELECT * FROM (
                            SELECT TOP 100 PERCENT COALESCE({idRol},1) AS IdRol, A.F100901, CAST({dtInc} AS datetime) AS ZiuaInc, 
                            A.F10022 AS DataInceput, ddp.rez AS DataSfarsit,  A.F10008 + ' ' + A.F10009 AS AngajatNume, C.Id AS IdContract, 
                            Y.Norma, Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, 
                            C.Denumire AS DescContract, ISNULL(C.OreSup,0) AS OreSup, ISNULL(C.Afisare,1) AS Afisare, 
                            B.F100958, B.F100959,
                            H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"",
                            ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, K.Denumire AS StareDenumire, 
                            A.F10078 AS Angajator, DR.F08903 AS TipContract, 
                            (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                            CA.F72404 AS CategAngajat, 
                            dn.rez AS AvansNorma, 
                            CASE WHEN Norma <> dn.rez THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)={an} AND MONTH(F70406)={luna}) ELSE {General.ToDataUniv(2100,1,1)} END AS AvansData,
                            X.* {zileVal}
                            FROM Ptj_Cumulat X 
                            INNER JOIN (SELECT F10003 {zileAs} FROM 
                            (SELECT F10003, ValStr, Ziua From Ptj_Intrari WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                            PIVOT  (MAX(ValStr) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                            ) pvt ON X.F10003=pvt.F10003
                            LEFT JOIN F100 A ON A.F10003=X.F10003 
                            LEFT JOIN F1001 B ON A.F10003=B.F10003 
                            LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                            LEFT JOIN Ptj_Intrari Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                            LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                            LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                            LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                            LEFT JOIN F724 CA ON A.F10061 = CA.F72402 
                            OUTER APPLY dbo.DamiNorma(X.F10003, {dtSf}) dn 
                            OUTER APPLY dbo.DamiDataPlecare(X.F10003, {dtSf}) ddp 
							LEFT JOIN F005 H ON Y.F10006 = H.F00506
							LEFT JOIN F006 I ON Y.F10007 = I.F00607
                            WHERE X.An = {an} AND X.Luna = {luna}
                            ORDER BY AngajatNume) A
                            WHERE 1=1 {strFiltru}";

                dt = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }



        ////public DataTable PontajAfiseaza(int idUser, int an, int luna, int alMeu, int F10003, int idRol, int idDept, int idAngajat, int idSubcompanie, int idFiliala, int idSectie, int idStare, int idContract, int idSubdept, int idBirou)
        //public DataTable PontajAfiseaza()
        //{

        //    string strSql = "";
        //    DataTable dt = new DataTable();

        //    try
        //    {

        //        int idUser = Convert.ToInt32(Session["UserId"]);
        //        int an = Convert.ToDateTime(txtAnLuna.Value).Year;
        //        int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
        //        int alMeu = 0;
        //        int f10003 = Convert.ToInt32(Session["User_Marca"]);
        //        int idRol = Convert.ToInt32(cmbRol.Value ?? -99);
        //        int idAng = Convert.ToInt32(cmbAng.Value ?? -99);
        //        int idCtr = Convert.ToInt32(cmbCtr.Value ?? -99);
        //        int idStr = Convert.ToInt32(cmbStare.Value ?? -99);

        //        int idSub = Convert.ToInt32(cmbSub.Value ?? -99);
        //        int idFil = Convert.ToInt32(cmbFil.Value ?? -99);
        //        int idSec = Convert.ToInt32(cmbSec.Value ?? -99);
        //        int idDpt = Convert.ToInt32(cmbDept.Value ?? -99);
        //        int idSubDpt = Convert.ToInt32(cmbSubDept.Value ?? -99);
        //        int idBirou = Convert.ToInt32(cmbBirou.Value ?? -99);



        //        if (Constante.tipBD == 1)
        //        {

        //            //string cmpF = "";
        //            //string cmpZi = "";

        //            //for (int i = 1; i <= 60; i++)
        //            //{
        //            //    if (i <= 31) cmpZi += ",X.Ziua" + i;
        //            //    cmpF += ",X.F" + i;
        //            //}
        //            //strSql = $@"SELECT X.F10003, A.F10008 + ' ' + A.F10009 AS NumeComplet, 
        //            //                F.F00305 AS Subcompanie, G.F00406 AS Filiala, H.F00507 AS Sectie, I.F00608 AS Departament,
        //            //                ISNULL(X.IdStare, 1) AS IdStare, ISNULL(D.Culoare, '#FFFFFFFF') AS Culoare
        //            //                {cmpF} {cmpZi}
        //            //                FROM Ptj_Cumulat X
        //            //                LEFT JOIN Ptj_tblStariPontaj D ON D.Id = ISNULL(X.IdStare, 1)
        //            //                LEFT JOIN F100 A ON A.F10003 = X.F10003
        //            //                LEFT JOIN F003 F ON A.F10004 = F.F00304
        //            //                LEFT JOIN F004 G ON A.F10005 = G.F00405
        //            //                LEFT JOIN F005 H ON A.F10006 = H.F00506
        //            //                LEFT JOIN F006 I ON A.F10007 = I.F00607
        //            //                WHERE X.An = {an} AND X.Luna = {luna}";


        //            string zile = "";
        //            string ZileAs = "";

        //            string ziInc = an.ToString() + "-" + luna.ToString().PadLeft(2, '0') + "-01";
        //            string ziSf = General.ToDataUniv(an, luna, 99);

        //            string ziCuloare = "";
        //            string ziUnion = "";

        //            //2015-12-11   s-a modificat din X. in Y. 
        //            string strFiltru = "";
        //            if (idCtr != -99) strFiltru += " AND Y.IdContract = " + idCtr.ToString();
        //            if (idSub != -99) strFiltru += " AND Y.F10004 = " + idSub.ToString();
        //            if (idFil != -99) strFiltru += " AND Y.F10005 = " + idFil.ToString();
        //            if (idSec != -99) strFiltru += " AND Y.F10006 = " + idSec.ToString();
        //            if (idDpt != -99) strFiltru += " AND Y.F10007 = " + idDpt.ToString();
        //            if (idStr != -99) strFiltru += " AND COALESCE(J.IdStare,1) = " + idStr.ToString();
        //            if (idSubDpt != -99) strFiltru += " AND B.F100958 = " + idSubDpt.ToString();
        //            if (idBirou != -99) strFiltru += " AND B.F100959 = " + idBirou.ToString();
        //            if (idAng == -99)
        //                strFiltru += General.GetF10003Roluri(idUser, an, luna, alMeu, f10003, idRol, 0, -99, idAng).Replace("AND A.F10003 IN", "AND X.F10003 IN");
        //            else
        //                strFiltru += " AND X.F10003=" + idAng;


        //            string cmpF = "";
        //            string cmpF_Fara = "";
        //            for(int i=1; i<=10; i++)
        //            {
        //                cmpF += ",J.F" + i;
        //                cmpF_Fara += ",F" + i;
        //            }

        //            for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
        //            {
        //                string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
        //                zile += ", " + strZi;
        //                ZileAs += ", " + strZi + " AS Val" + i.ToString();
        //                ziCuloare += "+ ISNULL(" + strZi + ",'#00FFFFFF') + ';'";
        //                ziUnion += "union select '" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "' as ziua ";
        //            }

        //            if (ziCuloare.Length > 1) ziCuloare = ziCuloare.Substring(1);
        //            if (ziUnion.Length > 6) ziUnion = ziUnion.Substring(6);

        //            string selectCuloare = " SELECT (CASE WHEN (W.CuloareValoare IS NULL OR W.CuloareValoare = '') THEN '#00FFFFFF' ELSE W.CuloareValoare END) + ';' from ( " +
        //                    " SELECT M.Ziua, N.Linia,  " +
        //                    " CASE WHEN (datepart(dw,M.Ziua)=1 OR datepart(dw,M.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.Ziua)<>0) and (isnull(N.CuloareValoare,'#00FFFFFF') = '#00FFFFFF' or isnull(N.CuloareValoare,'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE isnull(N.CuloareValoare,'#00FFFFFF') END AS CuloareValoare " +
        //                    " FROM (" + ziUnion + ") M " +
        //                    " LEFT JOIN Ptj_Intrari N ON M.Ziua=N.Ziua AND N.F10003=pvt.F10003 AND N.Linia = pvt.Linia AND '" + ziInc + "' <= N.Ziua and N.Ziua <= " + ziSf + " " +
        //                    " ) W  " +
        //                    " FOR XML PATH('')";



        //            strSql = "SELECT 0 as NrAng, COALESCE(" + idRol + ",1) AS IdRol, " + luna + " AS Luna, " + an + " AS An, " +
        //                    " CAST((pvt.F10003 * CAST(10000 AS DECIMAL)) + (CASE WHEN COALESCE(F06204,-1) = -1 THEN 0 ELSE F06204 END) AS DECIMAL) AS IdAuto, " +
        //                    " CAST ((CASE WHEN COALESCE(F06204,-1) =-1 THEN 0 ELSE (pvt.F10003 * CAST(10000 AS DECIMAL)) END) AS DECIMAL) AS Parinte,  " +
        //                    " F100901, pvt.F10003, F06204, ZiuaInc, pvt.Linia, Norma, DataInceput, DataSfarsit, NumeComplet, CCDefault, IdContract, DescContract, OreSup, Afisare, F10002, F10004, F10005, F10006, F10007, F100958, F100959, USER_NO, IdStare, Culoare, " +
        //                    " Angajator, TipContract, EID, CategAngajat, " +
        //                    " (" + selectCuloare + ") AS ZiCuloare, " +
        //                    " CAST(AvansNorma AS int) AS AvansNorma, " +
        //                    " CASE WHEN Norma <> AvansNorma THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)=" + an + " AND MONTH(F70406)=" + luna + ") ELSE '2100-01-01' END AS AvansData " +
        //                    ZileAs + cmpF_Fara +
        //                    " FROM " +
        //                    " ( " +
        //                    " SELECT A.F100901, X.F10003, X.ValStr, X.Ziua, ISNULL(X.Linia,0) AS Linia, ISNULL(X.F06204,-1) AS F06204, " +
        //                    " CONVERT(datetime,'" + ziInc + "') AS ZiuaInc, " +
        //                    " A.F10022 AS DataInceput, ddp.rez AS DataSfarsit,  A.F10008 + ' ' + A.F10009 AS NumeComplet, C.Id AS IdContract, " +
        //                    " Y.Norma, Y.F06204Default AS CCDefault, Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, " +
        //                    " C.Denumire AS DescContract, ISNULL(C.OreSup,0) AS OreSup, ISNULL(C.Afisare,1) AS Afisare, " +
        //                    " B.F100958, B.F100959, " + idUser + " AS USER_NO, ISNULL(J.IdStare,1) AS IdStare " + cmpF + ", " +
        //                    " ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, " +
        //                    " A.F10078 AS Angajator, DR.F08903 AS TipContract, US.F70104 AS EID, CA.F72404 AS CategAngajat, " +
        //                    " dn.rez AS AvansNorma " +
        //                    " FROM Ptj_Intrari X " +
        //                    " LEFT JOIN F100 A ON A.F10003=X.F10003 " +
        //                    " LEFT JOIN F1001 B ON A.F10003=B.F10003 " +
        //                    " LEFT JOIN (SELECT R.F10003, R.Linia, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari R WHERE YEAR(R.Ziua)=" + an + " AND MONTH(R.Ziua)= " + luna + " GROUP BY R.F10003, R.Linia) Q ON Q.F10003=A.F10003 AND Q.Linia=X.Linia " +
        //                    " LEFT JOIN Ptj_Intrari Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin AND Y.Linia=Q.Linia " +
        //                    " LEFT JOIN Ptj_Cumulat J ON J.F10003=A.F10003 AND J.An=" + an + " AND J.Luna=" + luna +
        //                    " LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(J.IdStare,1) " +
        //                    " LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract " +
        //                    " LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 " +
        //                    //" LEFT JOIN USERS US ON A.F10003 = US.F70102 " +
        //                    " LEFT JOIN USERS US ON A.F10003 = US.F10003 " +        //Radu 13.03.2017
        //                    " LEFT JOIN F724 CA ON A.F10061 = CA.F72402 " +
        //                    " OUTER APPLY dbo.DamiNorma(X.F10003, " + ziSf + ") dn " +
        //                    " OUTER APPLY dbo.DamiDataPlecare(X.F10003, " + ziSf + ") ddp " +
        //                    " WHERE YEAR(X.Ziua)=" + an + " AND MONTH(X.Ziua)= " + luna + strFiltru +
        //                    " ) AS source " +
        //                    " PIVOT " +
        //                    " ( " +
        //                    "     MAX(ValStr) " +
        //                    "     FOR Ziua IN ( " + zile.Substring(2) + ") " +
        //                    " ) AS pvt " +
        //                    " ORDER BY NumeComplet, F06204";
        //        }
        //        else
        //        {
        //            string zile = "";
        //            string ZileAs = "";

        //            string ziInc = General.ToDataUniv(an, luna, 1);
        //            string ziSf = General.ToDataUniv(an, luna, 99);

        //            string ziCuloare = "";
        //            string ziUnion = "";

        //            string strFiltru = "";
        //            if (an != -99) strFiltru += " AND to_char(X.\"Ziua\",'yyyy') = " + an.ToString();
        //            if (luna != -99) strFiltru += " AND to_char(X.\"Ziua\",'mm') = " + luna.ToString();
        //            if (idCtr != -99) strFiltru += @" AND (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= " + ziSf + " AND " + ziInc + " <= TRUNC(B.\"DataSfarsit\")) = " + idCtr.ToString();

        //            if (idStr != -99) strFiltru += " AND COALESCE(J.\"IdStare\",1) = " + idStr.ToString();
        //            if (idSub != -99) strFiltru += " AND A.F10004 = " + idSub.ToString();
        //            if (idFil != -99) strFiltru += " AND A.F10005 = " + idFil.ToString();
        //            if (idSec != -99) strFiltru += " AND A.F10006 = " + idSec.ToString();
        //            if (idDpt != -99) strFiltru += " AND A.F10007 = " + idDpt.ToString();
        //            if (idSubDpt != -99) strFiltru += " AND B.F100958 = " + idSubDpt.ToString();
        //            if (idBirou != -99) strFiltru += " AND B.F100959 = " + idBirou.ToString();
        //            if (idAng == -99)
        //                strFiltru += General.GetF10003Roluri(idUser, an, luna, alMeu, f10003, idRol, 0, idDpt, idAng);
        //            else
        //                strFiltru += " AND A.F10003=" + idAng;


        //            for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
        //            {
        //                string dtOra = i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + Dami.NumeLuna(luna, 1, "EN") + "-" + an.ToString().Substring(2);
        //                zile += ", '" + dtOra + "'";
        //                ZileAs += ", '" + dtOra + "' AS \"Val" + i.ToString() + "\"";
        //                ziCuloare += "|| COALESCE(\"Val" + i.ToString() + "\",'#00FFFFFF') || ';'";
        //                ziUnion += "UNION SELECT " + General.ToDataUniv(an, luna, i) + " AS \"Ziua\" FROM Dual ";
        //            }


        //            if (ziCuloare.Length > 1) ziCuloare = ziCuloare.Substring(2);
        //            if (ziUnion.Length > 6) ziUnion = ziUnion.Substring(6);

        //            //string selectCuloare = " SELECT F10003, \"Linia\", LISTAGG((CASE WHEN (W.\"CuloareValoare\" IS NULL OR W.\"CuloareValoare\" = '') THEN '#00FFFFFF' ELSE W.\"CuloareValoare\" END), ';')  " +
        //            //        " WITHIN GROUP (ORDER BY \"Ziua\") AS \"CuloareValoare\"  " +
        //            //        " from (  " +
        //            //        " SELECT N.F10003, M.\"Ziua\", N.\"Linia\", " +
        //            //        " CASE WHEN ((1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=6 OR (1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.\"Ziua\")<>0)  " +
        //            //        " and (COALESCE(N.\"CuloareValoare\",'#00FFFFFF') = '#00FFFFFF' or COALESCE(N.\"CuloareValoare\",'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE COALESCE(N.\"CuloareValoare\",'#00FFFFFF') END AS \"CuloareValoare\" " +
        //            //        " FROM (" + ziUnion + ") M  " +
        //            //        " LEFT JOIN \"Ptj_Intrari\" N ON M.\"Ziua\"=N.\"Ziua\" AND " + ziInc + " <= N.\"Ziua\" and N.\"Ziua\" <= " + ziSf + "  " +
        //            //        " ) W  GROUP BY F10003, \"Linia\" ";

        //            //Radu 05.05.2016
        //            string selectCuloare = " SELECT F10003, \"Linia\", LISTAGG((CASE WHEN (W.\"CuloareValoare\" IS NULL OR W.\"CuloareValoare\" = '') THEN '#00FFFFFF' ELSE W.\"CuloareValoare\" END), ';')  " +
        //                    " WITHIN GROUP (ORDER BY \"Ziua\") AS \"CuloareValoare\"  " +
        //                    " from (  " +
        //                    " SELECT F100.F10003, M.\"Ziua\", nvl(N.\"Linia\", 0) AS \"Linia\", " +
        //                    " CASE WHEN ((1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=6 OR (1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.\"Ziua\")<>0)  " +
        //                    " and (COALESCE(N.\"CuloareValoare\",'#00FFFFFF') = '#00FFFFFF' or COALESCE(N.\"CuloareValoare\",'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE COALESCE(N.\"CuloareValoare\",'#00FFFFFF') END AS \"CuloareValoare\" " +
        //                    " FROM F100 LEFT JOIN (" + ziUnion + ") M  ON 1 = 1 " +
        //                    " LEFT JOIN \"Ptj_Intrari\" N ON M.\"Ziua\"=N.\"Ziua\" AND " + ziInc + " <= N.\"Ziua\" and N.\"Ziua\" <= " + ziSf + "  " +
        //                    " AND F100.F10003 = N.F10003) W  GROUP BY F10003, \"Linia\" ";

        //            strSql = "SELECT 0 as \"NrAng\", COALESCE(" + idRol + ",1) AS \"IdRol\", " + luna + " AS \"Luna\", " + an + " AS \"An\", " +
        //                    " CAST((pvt.F10003 * CAST(10000 AS DECIMAL)) + (CASE WHEN COALESCE(pvt.F06204,-1) = -1 THEN 0 ELSE F06204 END) AS NUMBER(22)) AS IdAuto, " +
        //                    " CAST ((CASE WHEN COALESCE(pvt.F06204,-1) =-1 THEN 0 ELSE (pvt.F10003 * CAST(10000 AS DECIMAL)) END) AS NUMBER(22)) AS Parinte,  " +
        //                    " CASE WHEN \"Norma\" <> \"AvansNorma\" THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND to_number(to_char(F70406,'YYYY'))=" + an + " AND to_number(to_char(F70406,'MM'))=" + luna + ") ELSE to_date('01-JAN-2100','DD-MON-YYYY') END AS AvansData, " +
        //                    " pvt.* " +
        //                    " FROM " +
        //                    " ( " +
        //                    " SELECT A.F100901, X.F10003, X.\"ValStr\", X.\"Ziua\", coalesce(X.\"Linia\",0) AS \"Linia\", coalesce(X.F06204,-1) AS F06204, " +
        //                    " " + ziInc + " AS \"ZiuaInc\", " +
        //                    " A.F10022 AS \"DataInceput\",  TRUNC(\"DamiDataPlecare\"(X.F10003, " + ziSf + ")) AS \"DataSfarsit\",  A.F10008 || ' ' || A.F10009 AS \"NumeComplet\", C.\"Id\" AS \"IdContract\", " +
        //                    " Y.\"Norma\", Y.\"F06204Default\" AS \"CCDefault\", Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, " +
        //                    " C.\"Denumire\" AS \"DescContract\", COALESCE(C.\"OreSup\",0) AS \"OreSup\", COALESCE(C.\"Afisare\",1) AS \"Afisare\", " +
        //                    " B.F100958, B.F100959, " + idUser + " AS USER_NO, coalesce(J.\"IdStare\",1) AS \"IdStare\",  " +
        //                    " J.F31 AS G31,J.F32 AS G32,J.F33 AS G33,J.F34 AS G34,J.F35 AS G35,J.F36 AS G36,J.F37 AS G37,J.F38 AS G38,J.F39 AS G39,J.F40 AS G40, " +
        //                    " coalesce(K.\"Culoare\",'#FFFFFFFF') AS \"Culoare\", " +
        //                    " A.F10078 AS \"Angajator\", DR.F08903 AS \"TipContract\", US.F70104 AS EID, CA.F72404 AS \"CategAngajat\", " +
        //                    " \"DamiNorma\"(X.F10003, " + ziSf + ") AS \"AvansNorma\", V.\"CuloareValoare\" as ZiCuloare " +
        //                    " FROM \"Ptj_Intrari\" X " +
        //                    " LEFT JOIN F100 A ON A.F10003=X.F10003 " +
        //                    " LEFT JOIN F1001 B ON A.F10003=B.F10003 " +
        //                    " LEFT JOIN (SELECT R.F10003, MIN(R.\"Ziua\") AS ZiuaMin FROM \"Ptj_Intrari\" R WHERE to_number(to_char(R.\"Ziua\",'YYYY'))=" + an + " AND to_number(to_char(R.\"Ziua\",'MM'))= " + luna + " GROUP BY R.F10003) Q ON Q.F10003=A.F10003 " +
        //                    " LEFT JOIN \"Ptj_Intrari\" Y ON A.F10003=Y.F10003 AND Y.\"Ziua\"=Q.ZiuaMin " +
        //                    " LEFT JOIN \"Ptj_Cumulat\" J ON J.F10003=A.F10003 AND J.\"An\"=" + an + " AND J.\"Luna\"=" + luna +
        //                    " LEFT JOIN \"Ptj_tblStariPontaj\" K ON K.\"Id\" = coalesce(J.\"IdStare\",1) " +
        //                    " LEFT JOIN \"Ptj_Contracte\" C on C.\"Id\" = Y.\"IdContract\" " +
        //                    " LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 " +
        //                    " LEFT JOIN USERS US ON A.F10003 = US.F70102 " +
        //                    " LEFT JOIN F724 CA ON A.F10061 = CA.F72402 " +
        //                    " LEFT JOIN (" + selectCuloare + ") V ON V.F10003=X.F10003 AND V.\"Linia\" = X.\"Linia\"  " +
        //                    " WHERE to_number(to_char(X.\"Ziua\",'YYYY'))=" + an + " AND to_number(to_char(X.\"Ziua\",'MM'))= " + luna + strFiltru +
        //                    " ) " +
        //                    " PIVOT " +
        //                    " ( " +
        //                    "     MAX(\"ValStr\") " +
        //                    "     FOR \"Ziua\" IN ( " + ZileAs.Substring(2) + ") " +
        //                    " ) pvt " +
        //                    " ORDER BY \"NumeComplet\", F06204";
        //        }

        //        dt = General.IncarcaDT(strSql, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return dt;

        //}

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

                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);

                strSql = @"SELECT {0} AS ""IdAuto"", X.* FROM ({4}) X 
                            WHERE X.""IdRol"" = {1} AND X.F10022 <= {2} AND {3} <= X.F10023 {5}
                            ORDER BY X.""NumeComplet"" ";

                strSql = string.Format(strSql, cmp, Convert.ToInt32(cmbRol.Value ?? -99), General.ToDataUniv(dtData.Year, dtData.Month, 99), General.ToDataUniv(dtData.Year, dtData.Month), SelectComun(), filtru);
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
                                CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Alias"", S.""Denumire"") AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN ""tblSupervizori"" S ON C.""IdRol""=S.""Id""
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Alias"", S.""Denumire"") AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
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
                                LEFT JOIN ""tblSupervizori"" S ON C.""IdRol""=S.""Id""
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

                string strSql = $@"SELECT X.""IdRol"", X.""RolDenumire"" FROM ({SelectComun()}) X 
                                WHERE X.F10022 <= {General.ToDataUniv(dtData.Year, dtData.Month, 99)} AND {General.ToDataUniv(dtData.Year, dtData.Month)} <= X.F10023
                                GROUP BY X.""IdRol"", X.""RolDenumire""
                                ORDER BY X.""RolDenumire"" ";

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
                    }
                    else
                    {
                        GridViewDataTextColumn col = grDate.Columns[i.ToString()] as GridViewDataTextColumn;
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

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "StareDenumire")
                {
                    object col = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                    if (col != null) e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(col.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        #region Transfer F300


        protected void btnTransfera_Click(object sender, EventArgs e)
        {
            int rez = 1;
            int repartProcent = 0;

            DataTable entS = Session["InformatiaCurenta"] as DataTable;

            try
            {
                if (entS == null || entS.Rows.Count == 0)
                {
                    rez = 0;
                }
                else
                {
                    DataTable entFor = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""Vizibil"",0) = 1 AND CampSelect IS NOT NULL AND COALESCE(CampSelect,'') <> '' ORDER BY ""Ordine"" ", null);
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
                            General.IncarcaDT(string.Format("delete from f300 where f30042 in ('WizOnePontaj', 'WizOnePontaj_CC') and f30003 = {0} ", entS.Rows[i]["F10003"].ToString()), null);

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

                                //DataTable entParam = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormule\" WHERE \"Pagina\" = 'Pontaj.Pontaj' AND \"Control\" = 'TransferCC'", null);
                                //if (entParam != null) repartProcent = 1;

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
                            var dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1);
                            ent.F30035 = dtLucru;
                            ent.F30036 = dtLucru;
                            ent.F30037 = dtLucru;
                            ent.F30038 = dtLucru;

                            for (int j = 0; j < entFor.Rows.Count; j++)
                            {
                                try
                                {
                                    decimal val = 0;
                                    try
                                    {
                                        val = Convert.ToDecimal(entS.Rows[i][entFor.Rows[j]["Coloana"].ToString()]);
                                    }catch (Exception){}

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
                                            strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString(), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
                                                    ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                    General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
                                        else
                                            strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString(), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, General.DamiNextId("F300"),
                                                    ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                    General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);


                                        General.IncarcaDT(strTmp, null);
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                rez = 0;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            if (rez == 1)
            {
                MessageBox.Show(Dami.TraduCuvant("Date transferate cu succes"));
            }
            else
            {
                MessageBox.Show(Dami.TraduCuvant("Eroare la transfer"));
            }

        }







        //protected void btnTransfera_Click(object sender, EventArgs e)
        //{
        //    int rez = 1;
        //    int repartProcent = 0;
        //    Reports.PontajDinamic ptjDin = new Reports.PontajDinamic();
        //    DataTable dtTmp = null;


        //    decimal F10003 = Convert.ToDecimal(cmbAng.Value);
        //    int an = Convert.ToDateTime(txtAnLuna.Value).Year;
        //    int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
        //    int idUser = Convert.ToInt32(Session["UserId"].ToString());
        //    int idAngajat = -99;
        //    int idSubcompanie = -99;
        //    int idFiliala = -99;
        //    int idSectie = -99;
        //    int idDept = -99;
        //    int idSubdept = -99;
        //    int idStare = -99;
        //    int idContract = -99;
        //    int alMeu = 0;
        //    int idRol = Convert.ToInt32(cmbRol.Value);

        //    DataTable entS = ptjDin.rapListaPontaj(idUser, an, luna, alMeu, F10003, idRol, idDept, idSubdept, idAngajat, idSubcompanie, idFiliala, idSectie, idStare, idContract, false);

        //    try
        //    {
        //        if (entS == null || entS.Rows.Count == 0)
        //        {
        //            rez = 0;
        //        }
        //        else
        //        {
        //            dtTmp = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormule\" WHERE \"Pagina\" = 'Pontaj.Pontaj' AND \"Control\" = 'grDate' AND \"Coloana\" IS NOT NULL AND \"CodF300\" IS NOT NULL", null);
        //            if (dtTmp == null || dtTmp.Rows.Count == 0)
        //            {
        //                rez = 0;
        //            }
        //            else
        //            {
        //                string strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30052,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
        //                                        VALUES(300, {19}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})";
        //                if (Constante.tipBD == 1)
        //                    strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
        //                                        VALUES(300, {18}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})";


        //                for (int i = 0; i < entS.Rows.Count; i++)
        //                {
        //                    General.IncarcaDT(string.Format("delete from f300 where f30042 in ('WizOnePontaj', 'WizOnePontaj_CC') and f30003 = {0} ", entS.Rows[i]["F10003"].ToString()), null);

        //                    DataTable entFor = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormule\" WHERE \"Pagina\" = 'Pontaj.Pontaj' AND \"Control\" = 'grDate' AND \"Coloana\" IS NOT NULL AND \"CodF300\" IS NOT NULL", null);

        //                    metaF300 ent = new metaF300();
        //                    ent.F30001 = 300;
        //                    ent.F30002 = 1;
        //                    ent.F30003 = Convert.ToInt32(entS.Rows[i]["F10003"].ToString());


        //                    ent.F30012 = 0;
        //                    ent.F30013 = 0;
        //                    ent.F30014 = 0;
        //                    ent.F30015 = 0;
        //                    ent.F30021 = 0;
        //                    ent.F30022 = 0;
        //                    ent.F30023 = 0;
        //                    ent.F30039 = 0;
        //                    ent.F30040 = 0;
        //                    ent.F30041 = 0;
        //                    ent.F30043 = 0;
        //                    ent.F30044 = 0;
        //                    ent.F30045 = 0;
        //                    ent.F30046 = 0;
        //                    ent.F30053 = 0;
        //                    ent.F300612 = 0;
        //                    ent.F300613 = 0;
        //                    ent.F300614 = 0;
        //                    ent.F30054 = 0;


        //                    DataTable entF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + entS.Rows[i]["F10003"].ToString(), null);
        //                    if (entF100 != null)
        //                    {
        //                        ent.F30002 = Convert.ToInt32(entF100.Rows[0]["F10002"].ToString());
        //                        ent.F30004 = Convert.ToInt32(entF100.Rows[0]["F10004"].ToString());
        //                        ent.F30005 = Convert.ToInt32(entF100.Rows[0]["F10005"].ToString());
        //                        ent.F30006 = Convert.ToInt32(entF100.Rows[0]["F10006"].ToString());
        //                        ent.F30007 = Convert.ToInt32(entF100.Rows[0]["F10007"].ToString());

        //                        DataTable entParam = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormule\" WHERE \"Pagina\" = 'Pontaj.Pontaj' AND \"Control\" = 'TransferCC'", null);
        //                        if (entParam != null) repartProcent = 1;

        //                        if (repartProcent == 1)
        //                            ent.F30050 = 9999;
        //                        else
        //                        {
        //                            //in cazul in care F10053 nu este completat, centrul de cost se preia din tabela F006, campul F00615
        //                            if (entF100.Rows[0]["F10053"].ToString().Length > 0)
        //                            {
        //                                DataTable entF006 = General.IncarcaDT("SELECT * FROM F006 WHERE F00607 = " + entF100.Rows[0]["F10007"].ToString(), null);
        //                                if (entF006 != null)
        //                                    ent.F30050 = Convert.ToInt32(entF006.Rows[0]["F00615"].ToString());
        //                            }
        //                            else
        //                                ent.F30050 = Convert.ToInt32(entF100.Rows[0]["F10053"].ToString());
        //                        }
        //                    }

        //                    ent.F30011 = 1;
        //                    ent.F30042 = "WizOnePontaj";
        //                    ent.F30051 = 0;
        //                    var dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1);
        //                    ent.F30035 = dtLucru;
        //                    ent.F30036 = dtLucru;
        //                    ent.F30037 = dtLucru;
        //                    ent.F30038 = dtLucru;

        //                    for (int j = 0; j < entFor.Rows.Count; j++)
        //                    {
        //                        try
        //                        {
        //                            decimal val = 0;
        //                            PropertyInfo pi = entS.Rows[i].GetType().GetProperty(entFor.Rows[j]["Coloana"].ToString());
        //                            if (pi != null) val = Convert.ToDecimal((pi.GetValue(entS.Rows[i], null) ?? 0));

        //                            if (val != 0)
        //                            {
        //                                ent.F30013 = 0;
        //                                ent.F30014 = 0;
        //                                ent.F30015 = 0;

        //                                ent.F30010 = (short?)Convert.ToInt32(entFor.Rows[j]["CodF300"].ToString());
        //                                switch (Convert.ToInt32((entFor.Rows[j]["SursaF300"] ?? 1).ToString()))
        //                                {
        //                                    case 1:
        //                                        ent.F30013 = val;
        //                                        break;
        //                                    case 2:
        //                                        ent.F30014 = val * 100;
        //                                        ent.F30013 = 1;
        //                                        break;
        //                                    case 3:
        //                                        ent.F30015 = val;
        //                                        break;
        //                                }

        //                                string strTmp = "";
        //                                if (Constante.tipBD == 1)
        //                                    strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString(), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
        //                                            ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
        //                                            General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
        //                                else
        //                                    strTmp = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString(), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, General.DamiNextId("F300"),
        //                                            ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
        //                                            General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);


        //                                General.IncarcaDT(strTmp, null);
        //                            }
        //                        }
        //                        catch (Exception) { }
        //                    }
        //                }
        //            }
        //        }


        //        //transferam si formulele de tip "Transfer"
        //        TrimitePontajInF300_Altele(idUser, an, luna, alMeu, F10003, idRol, idDept, idAngajat, idSubcompanie, idFiliala, idSectie, idStare, idContract);
        //    }
        //    catch (Exception ex)
        //    {
        //        rez = 0;
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    if (rez == 1)
        //    {
        //        MessageBox.Show(Dami.TraduCuvant("Date transferate cu succes"));
        //    }
        //    else
        //    {
        //        MessageBox.Show(Dami.TraduCuvant("Eroare la transfer"));
        //    }

        //}

        //private void TrimitePontajInF300_Altele(int idUser, int an, int luna, int alMeu, decimal F10003, int idRol, int idDept, int idAngajat, int idSubcompanie, int idFiliala, int idSectie, int idStare, int idContract)
        //{
        //    try
        //    {
        //        DataTable lstFor = General.IncarcaDT("SELECT * FROM \"Ptj_tblFormule\" WHERE \"Pagina\" = 'Pontaj.Pontaj' AND \"Control\" = 'TransferCC' AND \"CodF300\" IS NOT NULL", null);
        //        if (lstFor == null || lstFor.Rows.Count == 0) return;

        //        string areCC = "0";
        //        DataTable entParam = General.IncarcaDT("SELECT * FROM \"tblParametrii\" WHERE \"Nume\" = 'PontajulAreCC'", null);
        //        if (entParam != null && entParam.Rows.Count > 0 && entParam.Rows[0]["Valoare"].ToString().Length > 0) areCC = entParam.Rows[0]["Valoare"].ToString();

        //        string cmp = "F06204Default";
        //        string strFiltru = " AND F06204 = -1";
        //        string faraPrin = " AND X.F06204 = -1";
        //        if (areCC == "1")
        //        {
        //            cmp = "F06204";
        //            strFiltru = " AND F06204 <> -1";
        //            faraPrin = " AND X.F06204 <> -1";
        //        }

        //        string strTrf = "";

        //        if (Constante.tipBD == 1)
        //            strTrf = @"SELECT A.F10003, A.""{3}"" AS ""CentruCost"", CASE WHEN COALESCE(B.""Total"",0)<>0 THEN ROUND(CONVERT(decimal,SUM({2}))/CONVERT(decimal,B.""Total""),4) ELSE CONVERT(decimal,1) END AS ""Procentaj""
        //                        FROM ""Ptj_Intrari"" A
        //                        LEFT JOIN (SELECT X.F10003, SUM({2}) AS ""Total"" FROM ""Ptj_Intrari"" X WHERE YEAR(X.Ziua)={0} AND MONTH(X.Ziua)={1} {5} GROUP BY X.F10003) B ON A.F10003 = B.F10003
        //                        INNER JOIN F100 C ON A.F10003 = C.F10003
        //                        LEFT JOIN ""Ptj_Cumulat"" J ON J.F10003=C.F10003 AND J.""An""={0} AND J.""Luna""={1}
        //                        WHERE YEAR(A.Ziua)={0} AND MONTH(A.Ziua)={1} AND A.""{3}"" IS NOT NULL {4}
        //                        GROUP BY A.F10003, A.""{3}"", B.""Total""  ";
        //        else
        //            strTrf = @"SELECT A.F10003, A.""{3}"" AS ""CentruCost"", CASE WHEN COALESCE(B.""Total"",0)<>0 THEN ROUND(CONVERT(decimal,SUM({2}))/CONVERT(decimal,B.""Total""),4) ELSE CONVERT(decimal,1) END AS ""Procentaj""
        //                        FROM ""Ptj_Intrari"" A
        //                        LEFT JOIN (SELECT X.F10003, SUM({2}) AS ""Total"" FROM ""Ptj_Intrari"" X WHERE to_number(to_char(X.""Ziua"",'YYYY'))={0} AND to_number(to_char(X.""Ziua"",'MM'))={1} {5} GROUP BY X.F10003) B ON A.F10003 = B.F10003
        //                        INNER JOIN F100 C ON A.F10003 = C.F10003
        //                        LEFT JOIN ""Ptj_Cumulat"" J ON J.F10003=C.F10003 AND J.""An""={0} AND J.""Luna""={1}
        //                        WHERE to_number(to_char(A.""Ziua"",'YYYY'))={0} AND to_number(to_char(A.""Ziua"",'MM'))={1} AND A.""{3}"" IS NOT NULL {4}
        //                        GROUP BY A.F10003, A.""{3}"", B.""Total""  ";



        //        if (an != -99) strFiltru += " AND YEAR(A.Ziua) = " + an.ToString();
        //        if (luna != -99) strFiltru += " AND MONTH(A.Ziua) = " + luna.ToString();
        //        if (idContract != -99) strFiltru += " AND (SELECT TOP 1 IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CONVERT(date,B.DataInceput) <= '" + General.ToDataUniv(an, luna, 99) + "' AND '" + General.ToDataUniv(an, luna) + "' <= CONVERT(date,B.DataSfarsit)) = " + idContract.ToString();

        //        if (idStare != -99) strFiltru += " AND COALESCE(J.IdStare,1) = " + idStare.ToString();

        //        if (idSubcompanie != -99) strFiltru += " AND C.F10004 = " + idSubcompanie.ToString();
        //        if (idFiliala != -99) strFiltru += " AND C.F10005 = " + idFiliala.ToString();
        //        if (idSectie != -99) strFiltru += " AND C.F10006 = " + idSectie.ToString();
        //        if (idDept != -99) strFiltru += " AND C.F10007 = " + idDept.ToString();
        //        if (idAngajat == -99)
        //            strFiltru += General.GetF10003Roluri(idUser, an, luna, alMeu, F10003, idRol, 0, idDept, idAngajat);
        //        else
        //            strFiltru += " AND C.F10003=" + idAngajat;


        //        string strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30052,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
        //                                VALUES(300, {19}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18})";
        //        if (Constante.tipBD == 1)
        //            strSql = @"INSERT INTO F300(f30001,f30002,f30003,f30010,f30013,f30014,f30015,f30042,f30051,f30004,f30005,f30006, f30007,f30050,f30035,f30036,f30037,f30038,USER_NO,TIME,f30011) 
        //                                VALUES(300, {18}, {0}, {1}, {2}, {3}, {4}, '{5}', 0, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17})";

        //        for (int i = 0; i < lstFor.Rows.Count; i++)
        //        {
        //            if (lstFor.Rows[i]["FormulaSql"].ToString().Length > 0)
        //            {
        //                strTrf = string.Format(strTrf, an, luna, lstFor.Rows[i]["FormulaSql"].ToString(), cmp, strFiltru, faraPrin);

        //                DataTable lst = General.IncarcaDT(strTrf, null);

        //                for (int j = 0; j < lst.Rows.Count; j++)
        //                {
        //                    if (Convert.ToInt32(lst.Rows[j]["Procentaj"].ToString()) != 0)
        //                    {
        //                        General.IncarcaDT(string.Format("delete from f300 where f30042 = 'WizOnePontaj_CC' and f30003 = {0} AND F30050 = {1}", lst.Rows[j]["F10003"].ToString(), lst.Rows[j]["CentruCost"].ToString()), null);

        //                        metaF300 ent = new metaF300();
        //                        ent.F30001 = 300;
        //                        ent.F30002 = 1;
        //                        ent.F30003 = Convert.ToInt32(lst.Rows[j]["F10003"].ToString());


        //                        ent.F30012 = 0;
        //                        ent.F30013 = 0;
        //                        ent.F30014 = 0;
        //                        ent.F30015 = 0;
        //                        ent.F30021 = 0;
        //                        ent.F30022 = 0;
        //                        ent.F30023 = 0;
        //                        ent.F30039 = 0;
        //                        ent.F30040 = 0;
        //                        ent.F30041 = 0;
        //                        ent.F30043 = 0;
        //                        ent.F30044 = 0;
        //                        ent.F30045 = 0;
        //                        ent.F30046 = 0;
        //                        ent.F30053 = 0;
        //                        ent.F300612 = 0;
        //                        ent.F300613 = 0;
        //                        ent.F300614 = 0;
        //                        ent.F30054 = 0;


        //                        DataTable entF100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + lst.Rows[j]["F10003"].ToString(), null);
        //                        if (entF100 != null)
        //                        {
        //                            ent.F30002 = Convert.ToInt32(lst.Rows[j]["F10002"].ToString());
        //                            ent.F30004 = Convert.ToInt32(lst.Rows[j]["F10004"].ToString());
        //                            ent.F30005 = Convert.ToInt32(lst.Rows[j]["F10005"].ToString());
        //                            ent.F30006 = Convert.ToInt32(lst.Rows[j]["F10006"].ToString());
        //                            ent.F30007 = Convert.ToInt32(lst.Rows[j]["F10007"].ToString());

        //                            ent.F30050 = Convert.ToInt32(lst.Rows[j]["CentruCost"].ToString());
        //                        }

        //                        ent.F30011 = 1;
        //                        ent.F30042 = "WizOnePontaj_CC";
        //                        ent.F30051 = 0;
        //                        var dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1); ;
        //                        ent.F30035 = dtLucru;
        //                        ent.F30036 = dtLucru;
        //                        ent.F30037 = dtLucru;
        //                        ent.F30038 = dtLucru;


        //                        ent.F30013 = null;
        //                        ent.F30014 = null;
        //                        ent.F30015 = null;

        //                        try
        //                        {
        //                            ent.F30010 = (short?)Convert.ToInt32(lstFor.Rows[i]["CodF300"].ToString());
        //                            switch (Convert.ToInt32((lstFor.Rows[i]["SursaF300"] ?? 1).ToString()))
        //                            {
        //                                case 1:                     //cantitate
        //                                    ent.F30013 = Convert.ToDecimal(lst.Rows[j]["Procentaj"].ToString()) * 100;
        //                                    break;
        //                                case 2:                     //procent
        //                                    ent.F30014 = Convert.ToDecimal(lst.Rows[j]["Procentaj"].ToString()) * 100;
        //                                    ent.F30013 = 1;
        //                                    break;
        //                                case 3:                     //suma
        //                                    ent.F30015 = Convert.ToDecimal(lst.Rows[j]["Procentaj"].ToString()) * 100;
        //                                    break;
        //                            }

        //                            string strTmp = "";
        //                            if (Constante.tipBD == 1)
        //                                strTmp = string.Format(strSql, ent.F30003, ent.F30010,
        //                                        ent.F30013 == null ? "null" : ent.F30013.ToString().Replace(",", "."),
        //                                        ent.F30014 == null ? "null" : ent.F30014.ToString().Replace(",", "."),
        //                                        ent.F30015 == null ? "null" : ent.F30015.ToString().Replace(",", "."), ent.F30042,
        //                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
        //                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
        //                            else
        //                                strTmp = string.Format(strSql, ent.F30003, ent.F30010,
        //                                        ent.F30013 == null ? "null" : ent.F30013.ToString().Replace(",", "."),
        //                                        ent.F30014 == null ? "null" : ent.F30014.ToString().Replace(",", "."),
        //                                        ent.F30015 == null ? "null" : ent.F30015.ToString().Replace(",", "."), ent.F30042, General.DamiNextId("F300"),
        //                                        ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
        //                                        General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);


        //                            General.IncarcaDT(strTmp, null);
        //                        }
        //                        catch (Exception) { }
        //                    }
        //                }
        //            }
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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

                    switch (arr[0])
                    {
                        case "btnInitParam":
                            {
                                if (arr.Length != 4 || arr[0] == "" || arr[1] == "" || arr[2] == "" || arr[3] == "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                                    return;
                                }

                                DateTime dt = Convert.ToDateTime(txtAnLuna.Value);
                                //PontajInit(Session["UserId"], dt.Year, dt.Month, Convert.ToInt32(cmbRol.Value ?? -99), Convert.ToBoolean(arr[1]), false, Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), Convert.ToBoolean(arr[2]), Convert.ToBoolean(arr[3]), false, stergePontariAngPlecati);

                                grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes. Va rugam filtrati din nou.");

                            }
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


        #region Pontaj Init

        public void PontajInit(int idUser, int an, int luna, int idRol, bool cuNormaZL = false, bool cuCCCu = false, int idDept = -99, int idAng = -99, int idSubcompanie = -99, int idFiliala = -99, int idSectie = -99, int idContract = -99, bool cuNormaSD = false, bool cuNormaSL = false, bool cuCCFara = false, int stergePontariAngPlecati = 0)
        {
            try
            {
                if (cuNormaZL == false && cuNormaSD == false && cuNormaSL == false) return;

                string strZile = "";
                string usr = "";
                if (idAng == -99)
                    usr = General.GetF10003Roluri(idUser, an, luna, 0, -99, idRol, 0, idDept, idAng);
                else
                    usr = " AND A.F10003=" + idAng;


                if (Constante.tipBD == 1)
                {
                    #region SQL
                    string strFIN = "";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS Ziua";
                    }

                    if (strZile.Length > 6) strZile = strZile.Substring(6);

                    string ziInc = General.ToDataUniv(an, luna, 1);
                    string ziSf = General.ToDataUniv(an, luna, 99);


                    if (stergePontariAngPlecati == 1)
                    {
                        string strDel = @"DELETE A
                                    FROM Ptj_Intrari A
                                    INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE CONVERT(date,f10023) >= {0} AND CONVERT(date,F10023) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua>= B.F10023;";

                        strDel = string.Format(strDel, ziInc);
                        strFIN += strDel;
                    }

                    string strFiltruZile = "";
                    if (cuNormaZL) strFiltruZile += @"OR (CASE WHEN datepart(dw,X.Ziua) in (2,3,4,5,6) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruZile += @"OR (CASE WHEN datepart(dw,X.Ziua) in (1,7) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruZile += @"OR B.DAY is not null";


                    if (strFiltruZile != "") strFiltruZile = "AND (" + strFiltruZile.Substring(2) + ")";

                    string strFiltru = "";
                    if (idContract != -99) strFiltru += " AND (SELECT MAX(IdContract) FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CONVERT(date,B.DataInceput) <= " + ziSf + " AND " + ziInc + " <= CONVERT(date,B.DataSfarsit)) = " + idContract.ToString();
                    if (idSubcompanie != -99) strFiltru += " AND A.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND A.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND A.F10006 = " + idSectie.ToString();
                    if (idDept != -99) strFiltru += " AND A.F10007 = " + idDept.ToString();

                    string strIns = @"insert into ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", F10002, F10004, F10005, F10006, F10007, ""CuloareValoare"", ""Norma"", ""IdContract"", USER_NO, TIME, ""ZiLiberaLegala"", ""F06204Default"", ""ValStr"", ""Val0"")
                                 {0} {1} {2} {3} ";

                    strIns = string.Format(strIns, DamiSelectPontajInit(idUser, an, luna, 1), strFiltru, strFiltruZile, usr);

                    strFIN += strIns + ";";


                    //actualizam inregistrarile unde norma = null doar pt linia mama, nu si pt centrii de cost
                    //2015-12-07  s-a adaugat inner join f100 pt a elimina zilele in care angajatii s-au angajat sau au plecat in luna
                    string strUp = @"UPDATE A SET 
                                        A.""ValStr"" = dn.rez , 
                                        A.""Val0"" = dn.rez * 60
                                        FROM ""Ptj_Intrari"" A
                                        INNER JOIN F100 C ON A.F10003=C.F10003 AND CONVERT(date, C.F10022) <= CONVERT(date, A.Ziua) AND CONVERT(date, A.Ziua) <= CONVERT(date, C.F10023)
                                        LEFT JOIN HOLIDAYS B ON A.Ziua=B.DAY
                                        OUTER APPLY dbo.DamiNorma(A.F10003, A.Ziua) dn
                                        OUTER APPLY dbo.DamiDataPlecare(A.F10003, A.Ziua) ddp
                                        WHERE YEAR(A.""Ziua"")={0} AND MONTH(A.""Ziua"")={1} AND (A.""ValStr"" IS NULL OR RTRIM(A.""ValStr"") = '') AND F06204=-1 
                                        AND CONVERT(date, A.Ziua) <= CONVERT(date, ddp.rez)
                                        {2} {3} {4}";

                    strUp = string.Format(strUp, an, luna, usr, strFiltru, strFiltruZile.Replace("X.", "A."));
                    //strUp = strUp.Replace("X.", "A.");

                    strFIN += strUp + ";";

                    //initializam Ptj_Cumulat
                    string strCum = @"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"")
                                SELECT F10003, {0}, {1}, 1, F10008 + ' ' + F10009 FROM F100 A WHERE F10022 <= {3} AND {2} <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={0} AND ""Luna"" = {1}) {4}";
                    strCum = string.Format(strCum, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), strFiltru);
                    strFIN += strCum + ";";


                    //initializam Ptj_CumulatIstoric
                    string strIst = @"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {0}, {1}, 1, {4}, {5}, {4}, {5} FROM F100 A WHERE F10022 <= {3} AND {2} <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={0} AND ""Luna"" = {1} GROUP BY F10003) {6}";
                    strIst = string.Format(strIst, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), idUser, "GetDate()", strFiltru);
                    strFIN += strIst + ";";

                    General.ExecutaNonQuery("BEGIN " + strFIN + " END;", null);

                    #endregion
                }
                else
                {

                    #region ORCL
                    string strFIN = "";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " as Ziua from dual";
                    }

                    if (strZile.Length > 6) strZile = strZile.Substring(6);

                    string ziInc = General.ToDataUniv(an, luna, 1);
                    string ziSf = General.ToDataUniv(an, luna, 99);


                    if (stergePontariAngPlecati == 1)
                    {
                        string strDel = @"DELETE FROM ""Ptj_Intrari"" 
                                        WHERE ""IdAuto"" IN 
                                        (SELECT A.""IdAuto""
                                        FROM ""Ptj_Intrari"" A
                                        INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE TRUNC(f10023) >= {0} AND TRUNC(F10023) <> TO_DATE('01-JAN-2100','DD-MON-YYYY')) B ON A.F10003=B.F10003 AND A.""Ziua"" >= B.F10023);";

                        strDel = string.Format(strDel, ziInc);
                        strFIN += strDel;
                    }

                    string strFiltruZile = "";
                    if (cuNormaZL) strFiltruZile += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (1,2,3,4,5) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruZile += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (6,7) AND B.DAY is null THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruZile += @"OR B.DAY is not null";


                    if (strFiltruZile != "") strFiltruZile = "AND (" + strFiltruZile.Substring(2) + ")";

                    string strFiltruUpdate = "";
                    if (cuNormaZL) strFiltruUpdate += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (1,2,3,4,5) AND (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)=0 THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSD) strFiltruUpdate += @"OR (CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW')) in (6,7) AND (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)=0 THEN 1 ELSE 0 END) = 1";
                    if (cuNormaSL) strFiltruUpdate += @"OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0";
                    if (strFiltruUpdate != "") strFiltruUpdate = "AND (" + strFiltruUpdate.Substring(2) + ")";

                    string strFiltru = "";
                    if (idContract != -99) strFiltru += @" AND (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= " + ziSf + " AND " + ziInc + " <= TRUNC(B.\"DataSfarsit\")) = " + idContract.ToString();
                    if (idSubcompanie != -99) strFiltru += " AND A.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND A.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND A.F10006 = " + idSectie.ToString();
                    if (idDept != -99) strFiltru += " AND A.F10007 = " + idDept.ToString();

                    string strIns = @"insert into ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""Parinte"", ""Linia"", F06204, F10002, F10004, F10005, F10006, F10007, ""CuloareValoare"", ""Norma"", ""IdContract"", USER_NO, TIME, ""ZiLiberaLegala"", ""F06204Default"", ""ValStr"", ""Val0"")
                                 {0} {1} {2} {3} ";

                    strIns = string.Format(strIns, DamiSelectPontajInit(idUser, an, luna, 1), strFiltru, strFiltruZile, usr);

                    strFIN += strIns + ";";

                    //actualizam inregistrarile unde norma = null
                    //2015-12-07  s-a adaugat inner join f100 pt a elimina zilele in care angajatii s-au angajat sau au plecat in luna
                    string strUp = @"UPDATE ""Ptj_Intrari"" A SET 
                                        A.""ValStr"" = CASE WHEN ((SELECT COUNT(*) FROM F100 Z WHERE Z.F10003=A.F10003 AND TRUNC(F10022) <= TRUNC(""Ziua"") AND TRUNC(""Ziua"") <= TRUNC(F10023) ) = 1 AND TRUNC(A.""Ziua"") <= TRUNC(""DamiDataPlecare""(A.F10003, A.""Ziua""))) THEN CAST(nvl((""DamiNorma""(A.F10003, A.""Ziua"")),(select f10043 from f100 where f10003 = A.F10003)) as int) ELSE null END , 
                                        A.""Val0"" =   CASE WHEN ((SELECT COUNT(*) FROM F100 Z WHERE Z.F10003=A.F10003 AND TRUNC(F10022) <= TRUNC(""Ziua"") AND TRUNC(""Ziua"") <= TRUNC(F10023) ) = 1 AND TRUNC(A.""Ziua"") <= TRUNC(""DamiDataPlecare""(A.F10003, A.""Ziua""))) THEN CAST(nvl((""DamiNorma""(A.F10003, A.""Ziua"")),(select f10043 from f100 where f10003 = A.F10003)) * 60 as int) ELSE null END 
                                        WHERE TO_CHAR(A.""Ziua"",'yyyy')={0} AND TO_CHAR(A.""Ziua"",'mm')={1} AND (A.""ValStr"" IS NULL OR TRIM(A.""ValStr"") = '') AND A.F06204=-1 
                                        {2} {3} {4}";

                    strUp = string.Format(strUp, an, luna, usr, strFiltru, strFiltruUpdate.Replace("X.Ziua", "A.\"Ziua\""));
                    //strUp = strUp.Replace("X.ZIUA", "A.\"Ziua\"");

                    strFIN += strUp + ";";

                    //initializam Ptj_Cumulat
                    string strCum = @"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"")
                                SELECT F10003, {0}, {1}, 1, F10008 || ' ' || F10009 FROM F100 A WHERE F10022 <= TRUNC({3}) AND TRUNC({2}) <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={0} AND ""Luna"" = {1}) {4}";

                    strCum = string.Format(strCum, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), strFiltru);
                    strFIN += strCum + ";";

                    //initializam Ptj_CumulatIstoric
                    string strIst = @"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {0}, {1}, 1, {4}, {5}, {4}, {5} FROM F100 A WHERE F10022 <= TRUNC({3}) AND TRUNC({2}) <= F10023 AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={0} AND ""Luna"" = {1} GROUP BY F10003) {6}";
                    strIst = string.Format(strIst, an, luna, General.ToDataUniv(an, luna, 1), General.ToDataUniv(an, luna, 99), idUser, "SYSDATE", strFiltru);
                    strFIN += strIst + ";";

                    General.ExecutaNonQuery("BEGIN " + strFIN + " END;", null);

                    #endregion

                }
            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.Message.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private string DamiSelectPontajInit(int idUser, int an, int luna, int cuNorma = 0)
        {
            //cuNorma
            //0 - PontajInitGlobal
            //1 - PontajInit


            string strSql = "";

            try
            {

                string strZile = "";
                string nrm = "";

                if (Constante.tipBD == 1)
                {
                    if (cuNorma == 1)
                        nrm = @" ,CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR CONVERT(date, X.Ziua) > CONVERT(date, ddp.rez) THEN CONVERT(int,NULL) ELSE dn.rez END AS ValStr
                                 ,CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR CONVERT(date, X.Ziua) > CONVERT(date, ddp.rez) THEN CONVERT(int,NULL) ELSE dn.rez * 60 END AS Val0";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS Ziua";
                    }

                    //Radu 04.04.2017 - am modificat F06204Default
                    strSql = @"SELECT A.F10003, X.Ziua, CASE WHEN datepart(dw,X.Ziua) - 1 = 0 THEN 7 ELSE datepart(dw,X.Ziua) - 1 END AS ZiSapt,
                                CASE WHEN datepart(dw,X.Ziua)=1 OR datepart(dw,X.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 THEN 1 ELSE 0 END AS ZiLibera, 
                                0 as Parinte, 0 as Linia, -1 as F06204, 
                                G.F00603 AS F10002, G.F00604 AS F10004, G.F00605 AS F10005, G.F00606 AS F10006, G.F00607 as F10007,
                                '#00FFFFFF' as CuloareValoare, 
                                dn.rez AS Norma, 
                                (SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.ZIUA AND X.ZIUA <= B.""DataSfarsit"") AS IdContract, 
                                {0} as USER_NO, getdate() as TIME,
                                CASE WHEN B.DAY is not null THEN 1 ELSE 0 END AS ZiLiberaLegala,

                                CASE WHEN NOT EXISTS(SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.ZIUA AND X.ZIUA <= C.""DataSfarsit"") THEN 
                                CASE WHEN COALESCE(dc.rez, 9999) <> 9999 THEN dc.rez ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = dc.rez) END 
                                ELSE (SELECT MAX(""IdCentruCost"") FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.ZIUA AND X.ZIUA <= C.""DataSfarsit"") END AS ""F06204Default""

                                {1}
                                FROM ({2}) x
                                inner join F100 A on 1=1 AND CONVERT(date, A.F10022) <= CONVERT(date, X.ZIUA) AND CONVERT(date, X.ZIUA) <= CONVERT(date, A.F10023)
                                left join HOLIDAYS B on X.Ziua=B.DAY
                                left join (select F10003, ""Ziua"", count(*) as CNT from ""Ptj_Intrari"" where YEAR(Ziua)={3} AND MONTH(Ziua)={4} AND F06204=-1 GROUP BY F10003, ""Ziua"") D on D.F10003=A.F10003 AND D.""Ziua"" = x.ZIUA
								OUTER APPLY dbo.DamiNorma(A.F10003, X.Ziua) dn
								OUTER APPLY dbo.DamiCC(A.F10003, X.Ziua) dc
								OUTER APPLY dbo.DamiDept(A.F10003, X.Ziua) dd
                                OUTER APPLY dbo.DamiDataPlecare(A.F10003, X.Ziua) ddp
                                LEFT JOIN F006 G ON G.F00607 = dd.rez
                                where isnull(D.CNT,0) = 0";

                }
                else
                {
                    if (cuNorma == 1)
                        nrm = @" ,CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW'))=6 OR (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR TRUNC(X.Ziua) > TRUNC(""DamiDataPlecare""(A.F10003, X.Ziua)) THEN NULL ELSE ""DamiNorma""(A.F10003, X.Ziua) END AS ValStr
                                 ,CASE WHEN (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW'))=6 OR (1 + TRUNC (X.Ziua) - TRUNC (X.Ziua, 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 OR TRUNC(X.Ziua) > TRUNC(""DamiDataPlecare""(A.F10003, X.Ziua)) THEN NULL ELSE ""DamiNorma""(A.F10003, X.Ziua) * 60 END AS Val0";

                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)               //pt fiecare zi din luna
                    {
                        strZile += " union select " + General.ToDataUniv(an, luna, i) + " AS Ziua FROM Dual";
                    }

                    //Radu 04.04.2017 - am modificat F06204Default
                    strSql = @"SELECT A.F10003, X.Ziua, (1 + TRUNC(X.Ziua) - TRUNC(X.Ziua, 'IW')) AS ""ZiSapt"",
                                CASE WHEN (1 + TRUNC(X.Ziua) - TRUNC(X.Ziua, 'IW'))=6 OR (1 + TRUNC(X.Ziua) - TRUNC(X.Ziua, 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = X.Ziua)<>0 THEN 1 ELSE 0 END AS ""ZiLibera"",
                                0 as ""Parinte"", 0 as ""Linia"", -1 as F06204, 
                                (SELECT C.F00603 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.Ziua)) AS F10002,
                                (SELECT C.F00604 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.Ziua)) AS F10004,
                                (SELECT C.F00605 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.Ziua)) AS F10005,
                                (SELECT C.F00606 FROM F006 C WHERE C.F00607=""DamiDept""(A.F10003, X.Ziua)) AS F10006,
                                ""DamiDept""(A.F10003, X.Ziua) AS F10007,                                
                                '#00FFFFFF' as ""CuloareValoare"", 
                                ""DamiNorma""(A.F10003, X.Ziua) as ""Norma"", 
                                (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE B.F10003 = A.F10003 AND B.""DataInceput"" <= X.Ziua AND X.Ziua <= B.""DataSfarsit"" and ROWNUM <= 1) AS ""IdContract"", 
                                {0} as USER_NO, SYSDATE as TIME,
                                CASE WHEN B.DAY is not null THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",

                                CASE WHEN NOT EXISTS(SELECT ""IdCentruCost"" FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.Ziua AND X.Ziua <= C.""DataSfarsit"" and ROWNUM <= 1) THEN
                                CASE WHEN COALESCE(""DamiCC""(A.F10003, X.Ziua), 9999) <> 9999 THEN ""DamiCC""(A.F10003, X.Ziua) ELSE (SELECT C.F00615 FROM F006 C WHERE C.F00607 = ""DamiDept""(A.F10003, X.Ziua)) END 
                                ELSE (SELECT ""IdCentruCost"" FROM ""F100CentreCost"" C WHERE C.F10003 = A.F10003 AND C.""DataInceput"" <= X.Ziua AND X.Ziua <= C.""DataSfarsit"" and ROWNUM <= 1) END AS ""F06204Default""

                                {1}
                                FROM ({2}) x
                                inner join F100 A on 1=1 AND TRUNC(A.F10022) <= TRUNC(X.Ziua) AND TRUNC(X.Ziua) <= TRUNC(A.F10023)
                                left join HOLIDAYS B on X.Ziua=B.DAY
                                left join (select F10003, ""Ziua"", count(*) as CNT from ""Ptj_Intrari"" WHERE TO_NUMBER(TO_CHAR(""Ziua"",'YYYY'))={3} AND TO_NUMBER(TO_CHAR(""Ziua"",'MM'))={4} AND F06204=-1 GROUP BY F10003, ""Ziua"") D on D.F10003=A.F10003 AND D.""Ziua"" = X.Ziua
                                where COALESCE(D.CNT,0) = 0";

                }

                if (strZile.Length > 6) strZile = strZile.Substring(6);

                strSql = string.Format(strSql, idUser, nrm, strZile, an, luna);

            }
            catch (Exception ex)
            {
                //srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }



        public void PontajInitGeneral(int idUser, int an, int luna)
        {
            try
            {
                //initializam Ptj_Intrari
                string strInt = $@"INSERT INTO ""Ptj_Intrari""(F10003, ""Ziua"", ""ZiSapt"", ""ZiLibera"", ""ZiLiberaLegala"", ""Norma"", ""IdContract"", F10002, F10004, F10005, F10006, F10007, USER_NO, TIME)
                                SELECT B.F10003, A.""Zi"", A.""ZiSapt"", 
                                CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLibera"", 
                                CASE WHEN C.DAY IS NOT NULL THEN 1 ELSE 0 END AS ""ZiLiberaLegala"",
                                dn.rez AS ""Norma"", 
                                (SELECT MAX(""IdContract"") FROM ""F100Contracte"" B WHERE B.F10003 = B.F10003 AND {General.TruncateDateAsString("B.\"DataInceput\"")} <= {General.TruncateDateAsString("A.\"Zi\"")}  AND {General.TruncateDateAsString("A.\"Zi\"")}  <= {General.TruncateDateAsString("B.\"DataSfarsit\"")} ) AS ""IdContract"", 
                                G.F00603 AS F10002, G.F00604 AS F10004, G.F00605 AS F10005, G.F00606 AS F10006, G.F00607 as F10007,
                                {Session["UserId"]} AS USER_NO, {General.CurrentDate()} AS TIME
                                FROM ""tblZile"" A
                                INNER JOIN F100 B ON 1=1 AND {General.TruncateDateAsString("B.\"F10022\"")}  <= {General.TruncateDateAsString("A.\"Zi\"")}  AND {General.TruncateDateAsString("A.\"Zi\"")}  <= {General.TruncateDateAsString("B.\"F10023\"")} 
                                LEFT JOIN HOLIDAYS C on A.""Zi""=C.DAY
                                LEFT JOIN (SELECT X.F10003, X.""Ziua"", COUNT(*) AS CNT FROM ""Ptj_Intrari"" X WHERE {General.FunctiiData("X.\"Ziua\"", "A")}={an} AND {General.FunctiiData("X.\"Ziua\"", "L")}={luna} GROUP BY X.F10003, X.""Ziua"") D ON D.F10003=B.F10003 AND D.""Ziua"" = A.""Zi""
                                OUTER APPLY dbo.DamiNorma(B.F10003, A.""Zi"") dn
                                OUTER APPLY dbo.DamiDept(B.F10003, A.""Zi"") dd
                                LEFT JOIN F006 G ON G.F00607 = dd.rez
                                WHERE {General.FunctiiData("X.\"Ziua\"", "A")}={an} AND {General.FunctiiData("X.\"Ziua\"", "L")}={luna} AND COALESCE(D.CNT,0) = 0;";

                //initializam Ptj_Cumulat
                string strCum = $@"INSERT INTO ""Ptj_Cumulat""(F10003, ""An"",""Luna"",""IdStare"", ""NumeComplet"", USER_NO, TIME)
                                SELECT F10003, {an}, {luna}, 1, F10008 {Dami.Operator()} ' ' {Dami.Operator()} F10009, {Session["UserId"]}, {General.CurrentDate()} FROM F100 
                                WHERE F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= F10023 
                                AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_Cumulat"" WHERE ""An""={an} AND ""Luna"" = {luna});";

                //initializam Ptj_CumulatIstoric
                string strIst = $@"INSERT INTO ""Ptj_CumulatIstoric""(F10003, ""An"",""Luna"",""IdStare"", ""IdUser"", ""DataAprobare"", USER_NO, TIME)
                                SELECT F10003, {an}, {luna}, 1, {Session["UserId"]}, {General.CurrentDate()}, {Session["UserId"]}, {General.CurrentDate()} FROM F100 
                                WHERE F10022 <= {General.ToDataUniv(an, luna, 99)} AND {General.ToDataUniv(an, luna, 1)} <= F10023 
                                AND F10003 NOT IN (SELECT F10003 FROM ""Ptj_CumulatIstoric"" WHERE ""An""={an} AND ""Luna"" = {luna} GROUP BY F10003);";

                General.ExecutaNonQuery("BEGIN " + strInt + "\n\r" + strCum + "\n\r" + strIst + " END;", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        public string AngajatiPlecatiCuPontari(int an, int luna)
        {
            string rez = "";

            try
            {
                string strSql = "";
                
                if (Constante.tipBD == 1)
                    strSql = @"SELECT DISTINCT CONVERT(nvarchar(10),A.F10003) + ', '
                                FROM Ptj_Intrari A
                                INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE CONVERT(date,f10023) >= {0} AND CONVERT(date,F10023) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua>= B.F10023
                                FOR XML PATH('')";
                else
                    strSql = @"SELECT LISTAGG(A.F10003, '; ') WITHIN GROUP (ORDER BY A.F10003) 
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE TRUNC(f10023) >= {0} AND TRUNC(F10023) <> TO_DATE('01-JAN-2100','DD-MON-YYYY')) B ON A.F10003=B.F10003 AND A.""Ziua"" >= B.F10023";

                strSql = string.Format(strSql, General.ToDataUniv(an, luna));

                rez = (General.ExecutaScalar(strSql, null) ?? "").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        protected void btnInitParam_Click(object sender, EventArgs e)
        {
            if (txtCol.Count > 0 && txtCol["mod"] != null)
            {
                string ert = "";
                var e1 = chkNormaSD.Checked;
                var e2 = chkNormaSL.Checked;
                var e3 = chkNormaZL.Checked;
                var e4 = txtCol["mod"];
                var edc = "";


            }
        }
    }
}