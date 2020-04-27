using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajDetaliat : System.Web.UI.Page
    {

        //tip = 1       Pontaj pe Angajat
        //tip = 2       Pontaj pe Zi
        public int tip = 1;

        protected string lstInOut
        {
            get; private set;
        }

        protected int tipAfisareCC
        {
            get; private set;
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Pontaj.PontajDetaliat";
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                if (!IsPostBack)
                {
                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));

                    if (tip == 1 || tip == 10)
                    {
                        divPeAng.Style["display"] = "inline-block";
                        divPeZi.Style["display"] = "none";
                        grDate.Columns["NumeComplet"].Visible = false;
                        grDate.Columns["Cheia"].Caption = Dami.TraduCuvant("Ziua");
                    }
                    else
                    {
                        divPeAng.Style["display"] = "none";
                        divPeZi.Style["display"] = "inline-block";
                        grDate.Columns["NumeComplet"].Visible = true;
                        grDate.Columns["NumeComplet"].Caption = Dami.TraduCuvant("NumeComplet");
                        grDate.Columns["Cheia"].Caption = Dami.TraduCuvant("Marca");

                        btnAproba.Visible = false;
                        btnRespins.Visible = false;
                        btnInit.Visible = false;
                        btnDelete.Visible = false;
                    }

                    CreeazaGrid();

                    DataTable dtVal = General.IncarcaDT(Constante.tipBD == 1 ? @"SELECT TOP 0 * FROM ""Ptj_IstoricVal"" " : @"SELECT * FROM ""Ptj_IstoricVal"" WHERE ROWNUM = 0 ", null);
                    Session["Ptj_IstoricVal"] = dtVal;
                }

                if (Dami.ValoareParam("PontajulAreCC") == "1" && (tip == 1 || tip == 10))
                {
                    grDate.Columns[0].Visible = true;
                    tblCC.Attributes["class"] = "visible";

                    if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "1")
                        grCC.Columns["IdStare"].Visible = true;

                    DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                    GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    colStari.PropertiesComboBox.DataSource = dtStari;

                    tipAfisareCC = Convert.ToInt32(Dami.ValoareParam("PontajCCTipAfisareNrOre", "7"));
                    if (!IsPostBack)
                    {
                        DataTable dtAd = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAdminCC"" ", null);
                        Session["Ptj_tblAdminCC"] = dtAd;
                        for (int i = 0; i < dtAd.Rows.Count; i++)
                        {
                            string cmp = dtAd.Rows[i]["Camp"].ToString();

                            grCC.Columns[cmp].Visible = Convert.ToBoolean(dtAd.Rows[i]["Vizibil"]);
                            grCC.Columns[cmp].ToolTip = Dami.TraduCuvant(General.Nz(dtAd.Rows[i]["AliasToolTip"], "").ToString());
                            grCC.Columns[cmp].Caption = Dami.TraduCuvant(General.Nz(dtAd.Rows[i]["Alias"], dtAd.Rows[i]["Camp"]).ToString());

                            if (cmp.ToLower().IndexOf("nrore") >= 0)
                            {
                                GridViewDataTextColumn col = grCC.Columns[cmp] as GridViewDataTextColumn;
                                switch (tipAfisareCC)
                                {
                                    case 1:         //minutes
                                        col.PropertiesTextEdit.MaskSettings.Mask = "<0..999>";
                                        col.PropertiesTextEdit.MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.None;
                                        break;
                                    case 2:         //houres
                                        col.PropertiesTextEdit.MaskSettings.Mask = "<0..99>";
                                        col.PropertiesTextEdit.MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.None;
                                        break;
                                    case 3:         //HH:MM
                                        col.PropertiesTextEdit.MaskSettings.Mask = "<00..23>:<00..59>";
                                        break;
                                }
                            }
                        }
                    }
                }


                if (tip == 2 || tip == 20)
                    grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ", "10"));
                else
                    grDate.SettingsPager.PageSize = 31;

                cmbPtjAng.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Toate inregistrarile"), Value = 1 });
                cmbPtjAng.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Erori"), Value = 2 });
                cmbPtjAng.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Lipsa pontari"), Value = 3 });
                cmbPtjAng.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Erori si lipsa pontari"), Value = 4 });

                cmbPtjZi.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Toate inregistrarile"), Value = 1 });
                cmbPtjZi.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Erori"), Value = 2 });
                cmbPtjZi.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Lipsa pontari"), Value = 3 });
                cmbPtjZi.Items.Add(new ListEditItem { Text = Dami.TraduCuvant("Erori si lipsa pontari"), Value = 4 });

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Filtru"" ", null);
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbPtjAng.Items.Add(new ListEditItem { Text = General.Nz(dt.Rows[i]["Denumire"],"").ToString(), Value = Convert.ToInt32(General.Nz(dt.Rows[i]["Id"],1)) + 5 });
                    cmbPtjZi.Items.Add(new ListEditItem { Text = General.Nz(dt.Rows[i]["Denumire"], "").ToString(), Value = Convert.ToInt32(General.Nz(dt.Rows[i]["Id"], 1)) + 5 });
                }

                if (Convert.ToInt32(General.Nz(Session["IdClient"], "-99")) != Convert.ToInt32(IdClienti.Clienti.Chimpex))
                {
                    divHovercardAng.Visible = false;
                    divHovercardZi.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Print");
                btnRespins.Text = Dami.TraduCuvant("btnRespins", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");
                btnInit.Text = Dami.TraduCuvant("btnInit", "Init");
                btnDelete.Text = Dami.TraduCuvant("btnDelete", "Sterge");
                btnRecalc.Text = Dami.TraduCuvant("btnRecalc", "Recalculeaza");
                btnPtjEchipa.Text = Dami.TraduCuvant("btnPtjEchipa", "Pontajul Echipei");
                //btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");

                btnFiltruAng.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruZi.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblAnLuna.InnerText = Dami.TraduCuvant("Luna/An");
                lblRolAng.InnerText = Dami.TraduCuvant("Roluri");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblPtjAng.InnerText = Dami.TraduCuvant("Tip inregistrare");
                lblPtjZi.InnerText = Dami.TraduCuvant("Tip inregistrare");
                lblCateg.InnerText = Dami.TraduCuvant("Categorie");

                lblZiua.InnerText = Dami.TraduCuvant("Data");
                lblRolZi.InnerText = Dami.TraduCuvant("Roluri");
                lblAngZi.InnerText = Dami.TraduCuvant("Angajat");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblCtr.InnerText = Dami.TraduCuvant("Contract");
                lblSub.InnerText = Dami.TraduCuvant("Subcompanie");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Dept");
                lblSubDept.InnerText = Dami.TraduCuvant("SubDept");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");


                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                //Radu 13.12.2019
                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                foreach (ListBoxColumn col in cmbAngZi.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                pnlFiltrare.HeaderText = Dami.TraduCuvant("Setare filtru de selectie");
                #endregion

                if (tip == 1 || tip == 10)
                    txtTitlu.Text = Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj pe angajat");
                else
                    txtTitlu.Text = Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj pe zi");

                if (!IsPostBack)
                {
                    txtAnLuna.Value = DateTime.Now;
                    txtZiua.Value = DateTime.Now;

                    //Radu 15.01.2020
                    GetDataBlocare(DateTime.Now);

                    Session["InformatiaCurenta"] = null;

                    IncarcaRoluri();

                    if (tip == 1 || tip == 2) IncarcaAngajati();

                    if (tip == 10)
                    {
                        NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_PontajulEchipei"] ?? "").ToString());
                        if (lst.Count > 0)
                        {
                            if (lst["An"] != "" && lst["Luna"] != "" && lst["Ziua"] != "")
                            {
                                try
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(lst["Ziua"]));
                                    if (General.Nz(Request.QueryString["Ziua"], "").ToString() != "") dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(General.Nz(Request.QueryString["Ziua"], "").ToString().Replace("Ziua","")));
                                    txtAnLuna.Value = dt;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (General.Nz(lst["Rol"], "").ToString() != "") cmbRolAng.Value = Convert.ToInt32(lst["Rol"]);
                            if (General.Nz(Request.QueryString["f10003"], "").ToString() != "") cmbAng.Value = Convert.ToInt32(Request.QueryString["f10003"]);

                            btnFiltru_Click(null, null);

                            if (General.Nz(Request.QueryString["idxPag"], "").ToString() != "" && General.Nz(Request.QueryString["idxRow"], "").ToString() != "")
                            {
                                Session["Filtru_PontajulEchipei"] += "&IndexPag=" + General.Nz(Request.QueryString["idxPag"], "").ToString();
                                Session["Filtru_PontajulEchipei"] += "&IndexRow=" + General.Nz(Request.QueryString["idxRow"], "").ToString();
                            }
                        }

                        IncarcaAngajati();
                    }

                    if (tip == 20)
                    {
                        NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_PontajulEchipei"] ?? "").ToString());
                        if (lst.Count > 0)
                        {
                            if (lst["An"] != "" && lst["Luna"] != "" && lst["Ziua"] != "")
                            {
                                try
                                {
                                    DateTime dt = new DateTime(Convert.ToInt32(lst["An"]), Convert.ToInt32(lst["Luna"]), Convert.ToInt32(lst["Ziua"]));
                                    txtZiua.Value = dt;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (General.Nz(lst["Rol"], "").ToString() != "") cmbRolZi.Value = Convert.ToInt32(lst["Rol"]);
                            if (General.Nz(lst["IdAng"], "").ToString() != "") cmbAngZi.Value = Convert.ToInt32(lst["IdAng"]);
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
                    if (General.Nz(Session["SurseCombo"],"").ToString() != "")
                    {
                        DataSet ds = Session["SurseCombo"] as DataSet;
                        if (ds.Tables.Count > 0)
                        {
                            for(int i =0; i < ds.Tables.Count; i++)
                            {
                                GridViewDataComboBoxColumn colAbs = (grDate.Columns[ds.Tables[i].TableName] as GridViewDataComboBoxColumn);
                                if (colAbs != null) colAbs.PropertiesComboBox.DataSource = ds.Tables[i];

                            }
                        }
                    }

                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();
                    cmbAng.DataSource = Session["Pontaj_Angajati"];
                    cmbAng.DataBind();

                    cmbAngZi.DataSource = null;
                    cmbAngZi.Items.Clear();
                    cmbAngZi.DataSource = Session["Pontaj_Angajati"];
                    cmbAngZi.DataBind();

                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }

                }

                //Florin 2020.03.30
                //if (tip == 1 || tip == 10)
                //{
                //    grDate.SettingsPager.PageSize = 31;
                //}
                //else
                if (tip == 2 || tip == 20)
                {
                    string dataRef = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                    cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003 " +
                        (Constante.tipBD == 1 ? " WHERE F00310 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00311" : " WHERE  F00310 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00311"), null);
                    cmbSub.DataBind();
                    cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00411 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00412" : " AND F00411 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00412"), null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00513 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00514" : " AND F00513 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00514"), null);
                    cmbSec.DataBind();
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE " +
                            (Constante.tipBD == 1 ? "F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00623" : "F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00623"), null);
                        cmbDept.DataBind();
                    }
                    else
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99) +
                             (Constante.tipBD == 1 ? " AND F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00623" : " AND F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00623"), null);
                        cmbDept.DataBind();
                    }
                    cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00714 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00715" : " AND F00714 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00715"), null);
                    cmbSubDept.DataBind();
                    cmbBirou.DataSource = General.IncarcaDT("SELECT F00809, F00810 FROM F008 WHERE " +
                        (Constante.tipBD == 1 ? "F00814 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00815" : "F00814 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00815 "), null);
                    cmbBirou.DataBind();

                    cmbCateg.DataSource = General.IncarcaDT(@"SELECT ""Denumire"" AS ""Id"", ""Denumire"" FROM ""viewCategoriePontaj"" GROUP BY ""Denumire"" ", null);
                    cmbCateg.DataBind();

                    cmbStare.DataSource = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblStariPontaj"" ", null);
                    cmbStare.DataBind();

                    cmbCtr.DataSource = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte"" ", null);
                    cmbCtr.DataBind();
                }


                if (Dami.ValoareParam("PontajulAreCC") == "1")
                {
                    GridViewDataComboBoxColumn colDpt = (grCC.Columns["IdDept"] as GridViewDataComboBoxColumn);
                    if (colDpt != null)
                    {
                        DataTable dtDpt = General.IncarcaDT(General.SelectDepartamente(), null);
                        colDpt.PropertiesComboBox.DataSource = dtDpt;
                    }

                    GridViewDataComboBoxColumn colCC = (grCC.Columns["F06204"] as GridViewDataComboBoxColumn);
                    if (colCC != null)
                    {
                        string sqlCC = General.Nz(General.ExecutaScalar(@"SELECT ""SursaCombo"" FROM ""Ptj_tblAdminCC"" WHERE ""Camp""='F06204'", null), "").ToString().Trim();
                        if (sqlCC == "")
                            sqlCC = "SELECT F06204 AS Id, F06205 AS Denumire FROM F062";
                        DataTable dt = General.IncarcaDT(sqlCC, null);
                        colCC.PropertiesComboBox.DataSource = dt;
                    }

                    GridViewDataComboBoxColumn colPro = (grCC.Columns["IdProiect"] as GridViewDataComboBoxColumn);
                    if (colPro != null)
                    {
                        DataTable dt = General.IncarcaDT("SELECT * FROM \"tblProiecte\"", null);
                        colPro.PropertiesComboBox.DataSource = dt;
                    }

                    GridViewDataComboBoxColumn colSub = (grCC.Columns["IdSubproiect"] as GridViewDataComboBoxColumn);
                    if (colSub != null)
                    {
                        DataTable dt = General.IncarcaDT("SELECT * FROM \"tblSubproiecte\"", null);
                        colSub.PropertiesComboBox.DataSource = dt;
                    }

                    GridViewDataComboBoxColumn colAct = (grCC.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                    if (colAct != null)
                    {
                        DataTable dt = General.IncarcaDT("SELECT * FROM \"tblActivitati\"", null);
                        colAct.PropertiesComboBox.DataSource = dt;
                    }

                    if (tip == 10)
                    {
                        string ziua = General.Nz(Request.QueryString["ziua"], "").ToString().Replace("Ziua", "");
                        string f10003 = General.Nz(Request.QueryString["f10003"], "").ToString();
                        if (ziua != "" && f10003 != "")
                        {
                            DateTime ziTmp = Convert.ToDateTime(txtAnLuna.Value);
                            DataTable dt = SursaCC(Convert.ToInt32(f10003), General.ToDataUniv(ziTmp.Year, ziTmp.Month, Convert.ToInt32(ziua)));
                            Session["PtjCC"] = dt;
                            grCC.KeyFieldName = "IdAuto";
                            dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                            grCC.DataSource = dt;
                            grCC.DataBind();
                        }
                    }


                    if (IsPostBack && tip == 1)
                        IncarcaCC();
                }

                if (tip == 1 || tip == 10)
                    CreeazaGridTotaluri();
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

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
                grCC.DataSource = null;
                grCC.DataBind();
                if (Convert.ToInt32(General.Nz(Session["IdClient"], "-99")) == Convert.ToInt32(IdClienti.Clienti.Chimpex))
                {
                    divHovercardAng.Visible = false;
                    divHovercardZi.Visible = false;
                }

                int idRol = Convert.ToInt32(cmbRolAng.Value);
                if (Convert.ToInt32(General.Nz(Request["tip"], 1)) == 2)
                    idRol = Convert.ToInt32(cmbRolZi.Value);

                string dataBlocare = "22001231";
                string strSql = $@"SELECT COALESCE(Ziua,'2200-12-31') FROM Ptj_tblBlocarePontaj WHERE IdRol=@1";
                if (Constante.tipBD == 2)
                    strSql = @"SELECT COALESCE(""Ziua"",TO_DATE('31-12-2200','DD-MM-YYYY')) FROM ""Ptj_tblBlocarePontaj"" WHERE ""IdRol""=@1";
                DataTable dt = General.IncarcaDT(strSql, new object[] { idRol });
                if (dt != null && dt.Rows.Count > 0 && General.Nz(dt.Rows[0][0], "").ToString() != "" && General.IsDate(dt.Rows[0][0]))
                    dataBlocare = Convert.ToDateTime(dt.Rows[0][0]).Year + Convert.ToDateTime(dt.Rows[0][0]).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(dt.Rows[0][0]).Day.ToString().PadLeft(2, '0');

                Session["Ptj_DataBlocare"] = dataBlocare.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Session["PrintDocument"] = "PontajDetaliat";
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                string first = General.Nz(cmbAng.Value,-99).ToString();
                DateTime dt = Convert.ToDateTime(txtAnLuna.Value);
                if (tip == 2 || tip == 20)
                {
                    first = "SELECT X.F10003 FROM (" + SelectPontaj() + ") X";
                    dt = Convert.ToDateTime(txtZiua.Value);
                }

                Session["PrintParametrii"] = first + "#$" + dt.Year + "#$" + dt.Month + "#$" + dt.Day + "#$" + tip;

                Response.Redirect("~/Reports/Imprima.aspx?tip=" + tip, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPtjEchipa_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/Pontaj/PontajEchipa.aspx?tip=1", false);
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

        private void IncarcaGrid()
        {
            try
            {
                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                int idRol = Convert.ToInt32(cmbRolAng.Value);
                if (tip == 2)
                {
                    dtData = Convert.ToDateTime(txtZiua.Value);
                    idRol = Convert.ToInt32(cmbRolZi.Value);
                }

                General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), dtData.Year, dtData.Month);

                grDate.KeyFieldName = "Cheia";
                DataTable dt = PontajCuInOut();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Cheia"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();

                if (dt.Rows.Count > 0)
                {
                    string cul = General.Nz(dt.Rows[0]["CuloareStare"], "#FFFFFF").ToString();
                    if (cul.Length == 9) cul = "#" + cul.Substring(3);

                    if (tip == 1 || tip == 10)
                    {
                        if (grDate.Columns["Stare"] != null)
                        {
                            grDate.Columns["Stare"].Caption = General.Nz(dt.Rows[0]["NumeStare"], "Initiat").ToString();
                            grDate.Columns["Stare"].HeaderStyle.BackColor = General.Culoare(cul);
                            grDate.Columns["Stare"].HeaderStyle.ForeColor = System.Drawing.Color.Black;
                        }
                    }

                    grCC.Enabled = Convert.ToBoolean(General.Nz(dt.Rows[0]["DrepturiModif"], "0"));

                    //if (General.Nz(dt.Rows[0]["DrepturiModif"], "0").ToString() == "0")
                    //    grCC.Enabled = false;
                    //else
                    //    grCC.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
  
        private string SelectPontaj()
        {
            //tip  =  1    pontaj pe angajat
            //tip  =  2    pontaj pe zi

            //Florin 2019.02.19
            //tblRoluri_PoateModifica
            //-55  -  nu este cazul sa facem verificarea - nu este absenta de tip zi
            //-33  -  nu are drepturi pt a sterge - valoarea din coloana PoateModifica este 0
            // 1   -  poate sterge
            // 2   -  poate sterge dar apare o avertizare
            // 3   -  nu poate sterge, apare un mesaj blocant

            string strSql = "";

            try
            {
                string filtru = "";
                string cheia = "";
                int tipInreg = 1;
                string strLeg = "";

                DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                int idRol = Convert.ToInt32(cmbRolAng.Value);

                if (General.Nz(Request.QueryString["Tip"], "").ToString() == "1" || General.Nz(Request.QueryString["Tip"], "").ToString() == "10")
                {
                    filtru = $@" AND {General.ToDataUniv(ziua.Year, ziua.Month)} <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)} AND A.F10003=" + Convert.ToInt32(cmbAng.Value ?? -99);
                    cheia = General.FunctiiData("P.\"Ziua\"", "Z");

                    if (General.Nz(Request.QueryString["Ziua"], "").ToString() != "")
                        filtru += @" AND P.""Ziua""=" + General.ToDataUniv(ziua.Year, ziua.Month, Convert.ToInt32(Request.QueryString["Ziua"].Replace("Ziua", "")));

                    tipInreg = Convert.ToInt32(General.Nz(cmbPtjAng.Value, 1));
                }
                else
                {
                    ziua = Convert.ToDateTime(txtZiua.Value);
                    idRol = Convert.ToInt32(cmbRolZi.Value);

                    cheia = "P.F10003";
                    filtru = $@" AND {General.TruncateDate("P.Ziua")} = {General.ToDataUniv(ziua.Year, ziua.Month, ziua.Day)}";

                    if (General.Nz(cmbSub.Value, "").ToString() != "") filtru += " AND P.F10004=" + cmbSub.Value;
                    if (General.Nz(cmbFil.Value, "").ToString() != "") filtru += " AND P.F10005=" + cmbFil.Value;
                    if (General.Nz(cmbSec.Value, "").ToString() != "") filtru += " AND P.F10006=" + cmbSec.Value;
                    if (General.Nz(cmbDept.Value, "").ToString() != "") filtru += " AND P.F10007=" + cmbDept.Value;
                    if (General.Nz(cmbSubDept.Value, "").ToString() != "") filtru += " AND C.F100958 = " + cmbSubDept.Value;
                    if (General.Nz(cmbBirou.Value, "").ToString() != "") filtru += " AND C.F100959 = " + cmbBirou.Value;

                    if (General.Nz(cmbCtr.Value, "").ToString() != "") filtru += " AND P.\"IdContract\"=" + cmbCtr.Value;
                    if (General.Nz(cmbStare.Value, "").ToString() != "") filtru += " AND J.\"IdStare\"=" + cmbStare.Value;
                    if (General.Nz(cmbAngZi.Value, "").ToString() != "")
                        filtru += " AND P.F10003=" + cmbAngZi.Value;
                    else
                        filtru += General.GetF10003Roluri(Convert.ToInt32(Session["UserId"]), ziua.Year, ziua.Month, 0, -99, idRol, ziua.Day,-99, -99);

                    tipInreg = Convert.ToInt32(General.Nz(cmbPtjZi.Value, 1));

                    if (General.Nz(cmbCateg.Value, "").ToString() != "")
                    {
                        filtru += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                        strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    }
                }

                if (tipInreg != 1)
                {
                    string conditie = " P.\"ValStr\" not like '%[0-9]%' AND P.\"ValStr\" != '' ";
                    if (Constante.tipBD == 2) conditie = " NOT REGEXP_LIKE(P.\"ValStr\", '[[:digit:]]') ";

                    string erori = " AND (((P.\"In1\" IS NULL AND P.\"Out1\" IS NOT NULL) OR (P.\"In1\" IS NOT NULL AND P.\"Out1\" IS NULL)) "
                                    + "OR ((P.\"In2\" IS NULL AND P.\"Out2\" IS NOT NULL) OR (P.\"In2\" IS NOT NULL AND P.\"Out2\" IS NULL)) "
                                    + "OR ((P.\"In3\" IS NULL AND P.\"Out3\" IS NOT NULL) OR (P.\"In3\" IS NOT NULL AND P.\"Out3\" IS NULL)) "
                                    + "OR ((P.\"In4\" IS NULL AND P.\"Out4\" IS NOT NULL) OR (P.\"In4\" IS NOT NULL AND P.\"Out4\" IS NULL)) "
                                    + "OR ((P.\"In5\" IS NULL AND P.\"Out5\" IS NOT NULL) OR (P.\"In5\" IS NOT NULL AND P.\"Out5\" IS NULL)) "
                                    + "OR ((P.\"In6\" IS NULL AND P.\"Out6\" IS NOT NULL) OR (P.\"In6\" IS NOT NULL AND P.\"Out6\" IS NULL)) "
                                    + "OR ((P.\"In7\" IS NULL AND P.\"Out7\" IS NOT NULL) OR (P.\"In7\" IS NOT NULL AND P.\"Out7\" IS NULL)) "
                                    + "OR ((P.\"In8\" IS NULL AND P.\"Out8\" IS NOT NULL) OR (P.\"In8\" IS NOT NULL AND P.\"Out8\" IS NULL)) "
                                    + "OR ((P.\"In9\" IS NULL AND P.\"Out9\" IS NOT NULL) OR (P.\"In9\" IS NOT NULL AND P.\"Out9\" IS NULL)) "
                                    + "OR ((P.\"In10\" IS NULL AND P.\"Out10\" IS NOT NULL) OR (P.\"In10\" IS NOT NULL AND P.\"Out10\" IS NULL)) "
                                    + "OR ((P.\"In11\" IS NULL AND P.\"Out11\" IS NOT NULL) OR (P.\"In11\" IS NOT NULL AND P.\"Out11\" IS NULL)) "
                                    + "OR ((P.\"In12\" IS NULL AND P.\"Out12\" IS NOT NULL) OR (P.\"In12\" IS NOT NULL AND P.\"Out12\" IS NULL)) "
                                    + "OR ((P.\"In13\" IS NULL AND P.\"Out13\" IS NOT NULL) OR (P.\"In13\" IS NOT NULL AND P.\"Out13\" IS NULL)) "
                                    + "OR ((P.\"In14\" IS NULL AND P.\"Out14\" IS NOT NULL) OR (P.\"In14\" IS NOT NULL AND P.\"Out14\" IS NULL)) "
                                    + "OR ((P.\"In15\" IS NULL AND P.\"Out15\" IS NOT NULL) OR (P.\"In15\" IS NOT NULL AND P.\"Out15\" IS NULL)) "
                                    + "OR ((P.\"In16\" IS NULL AND P.\"Out16\" IS NOT NULL) OR (P.\"In16\" IS NOT NULL AND P.\"Out16\" IS NULL)) "
                                    + "OR ((P.\"In17\" IS NULL AND P.\"Out17\" IS NOT NULL) OR (P.\"In17\" IS NOT NULL AND P.\"Out17\" IS NULL)) "
                                    + "OR ((P.\"In18\" IS NULL AND P.\"Out18\" IS NOT NULL) OR (P.\"In18\" IS NOT NULL AND P.\"Out18\" IS NULL)) "
                                    + "OR ((P.\"In19\" IS NULL AND P.\"Out19\" IS NOT NULL) OR (P.\"In19\" IS NOT NULL AND P.\"Out19\" IS NULL)) "
                                    + "OR ((P.\"In20\" IS NULL AND P.\"Out20\" IS NOT NULL) OR (P.\"In20\" IS NOT NULL AND P.\"Out20\" IS NULL)) "

                                    + "OR ((P.\"In1\" is not null OR P.\"In2\" is not null OR P.\"In3\" is not null OR P.\"In4\" is not null OR P.\"In5\" is not null OR "
                                    + "P.\"In6\" is not null OR P.\"In7\" is not null OR P.\"In8\" is not null OR P.\"In9\" is not null OR P.\"In10\" is not null OR "
                                    + "P.\"In11\" is not null OR P.\"In12\" is not null OR P.\"In13\" is not null OR P.\"In14\" is not null OR P.\"In15\" is not null OR "
                                    + "P.\"In16\" is not null OR P.\"In17\" is not null OR P.\"In18\" is not null OR P.\"In19\" is not null OR P.\"In20\" is not null) "
                                    + "AND " + conditie + " AND P.\"ValStr\" IS NOT NULL) ) ";

                    string lipsaPontaj = " AND (P.\"In1\" IS NULL AND P.\"Out1\" IS NULL AND P.\"In2\" IS NULL AND P.\"Out2\" IS NULL AND " +
                                        " P.\"In3\" IS NULL AND P.\"Out3\" IS NULL AND P.\"In4\" IS NULL AND P.\"Out4\" IS NULL AND " +
                                        " P.\"In5\" IS NULL AND P.\"Out5\" IS NULL AND P.\"In6\" IS NULL AND P.\"Out6\" IS NULL AND " +
                                        " P.\"In7\" IS NULL AND P.\"Out7\" IS NULL AND P.\"In8\" IS NULL AND P.\"Out8\" IS NULL AND " +
                                        " P.\"In9\" IS NULL AND P.\"Out9\" IS NULL AND P.\"In10\" IS NULL AND P.\"Out10\" IS NULL AND " +
                                        " P.\"In11\" IS NULL AND P.\"Out11\" IS NULL AND P.\"In12\" IS NULL AND P.\"Out12\" IS NULL AND " +
                                        " P.\"In13\" IS NULL AND P.\"Out13\" IS NULL AND P.\"In14\" IS NULL AND P.\"Out14\" IS NULL AND " +
                                        " P.\"In15\" IS NULL AND P.\"Out15\" IS NULL AND P.\"In16\" IS NULL AND P.\"Out16\" IS NULL AND " +
                                        " P.\"In17\" IS NULL AND P.\"Out17\" IS NULL AND P.\"In18\" IS NULL AND P.\"Out18\" IS NULL AND " +
                                        " P.\"In19\" IS NULL AND P.\"Out19\" IS NULL AND P.\"In20\" IS NULL AND P.\"Out20\" IS NULL AND (\"ValStr\" IS NULL OR \"ValStr\" = '') ) ";

                    if (tipInreg == 2) filtru += erori;
                    if (tipInreg == 3) filtru += lipsaPontaj;
                    if (tipInreg == 4) filtru += " AND (" + erori.Substring(4) + " OR " + lipsaPontaj.Substring(4) + ")";
                    if (tipInreg > 4)
                    {
                        try
                        {
                            DataTable dtSup = General.IncarcaDT(@"SELECT * FROM ""Ptj_Filtru"" WHERE ""Id""=@1", new object[] { tipInreg - 5 });
                            if (dtSup != null && dtSup.Rows.Count > 0)
                            {
                                filtru += " AND P." + dtSup.Rows[0]["Camp"] + dtSup.Rows[0]["Operator"] + dtSup.Rows[0]["Valoare"];
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }

                string valTmp = "";
                string[] arrVal = Constante.lstValuri.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrVal.Length - 1; i++)
                {
                    if (Constante.tipBD == 1)
                        valTmp += $@",CONVERT(datetime,DATEADD(minute, P.""{arrVal[i]}"", '')) AS ""ValTmp{arrVal[i].Replace("Val", "")}"" ";
                    else
                        valTmp += $@",TO_DATE('01-01-1900','DD-MM-YYYY') + P.""{arrVal[i]}""/1440 AS ""ValTmp{arrVal[i].Replace("Val", "")}"" ";
                }

                string furiTmp = "";
                string[] arrFuri = Constante.lstFuri.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrFuri.Length - 1; i++)
                {
                    if (Constante.tipBD == 1)
                        furiTmp += $@",CONVERT(datetime,DATEADD(minute, P.""{arrFuri[i]}"", '')) AS ""FTmp{arrFuri[i].Replace("F", "")}"" ";
                    else
                        furiTmp += $@",TO_DATE('01-01-1900','DD-MM-YYYY') + P.""{arrFuri[i]}""/1440 AS ""FTmp{arrFuri[i].Replace("F", "")}"" ";
                }

                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                if (Constante.tipBD == 1)
                    strSql = $@"SELECT P.*, {General.FunctiiData("P.\"Ziua\"", "Z")} AS ""Zi"", A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"" {valTmp} {furiTmp},
                            {cheia} AS ""Cheia"",
                            E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Dept"",
                            L.""Denumire"" AS ""DescContract"", M.""Denumire"" AS DescProgram, COALESCE(L.""OreSup"",1) AS ""OreSup"", COALESCE(L.""Afisare"",1) AS ""Afisare"",
                            CASE WHEN A.F10022 <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <= A.F10023 AND
                            (SELECT COUNT(*) FROM ""F100Supervizori"" FS WHERE FS.F10003=P.F10003 AND FS.""IdSuper""={idRol} AND FS.""IdUser""={Session["UserId"]} AND {General.TruncateDate("FS.DataInceput")} <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <=  {General.TruncateDate("FS.DataSfarsit")}) = 1
                            THEN 1 ELSE 0 END AS ""Activ"",  
                            COALESCE(J.""IdStare"",1) AS ""IdStare"", K.""Culoare"" AS ""CuloareStare"", K.""Denumire"" AS ""NumeStare"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""Ptj_Cereri"" Z 
                            INNER JOIN ""Ptj_tblAbsente"" Y ON Z.""IdAbsenta"" = Y.""Id""
                            WHERE Z.F10003 = P.F10003 AND Z.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= Z.""DataSfarsit"" AND Z.""IdStare"" = 3
                            AND Z.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre"" = 1) AND COALESCE(Y.""NuTrimiteInPontaj"", 0) = 0) = 0 THEN 0 ELSE 1 END AS ""VineDinCereri"", 
                            A.F10022, A.F10023,
                            (SELECT COALESCE(A.""OreInVal"",'') + ';'
                            FROM ""Ptj_tblAbsente"" a
                            INNER JOIN ""Ptj_ContracteAbsente"" b ON a.""Id"" = b.""IdAbsenta""
                            INNER JOIN ""Ptj_relRolAbsenta"" c ON a.""Id"" = c.""IdAbsenta""
                            WHERE A.""OreInVal"" IS NOT NULL AND RTRIM(LTRIM(A.""OreInVal"")) <> '' AND B.""IdContract""=P.""IdContract"" AND C.""IdRol""={idRol} AND 
                            (((CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0) AND COALESCE(B.ZL,0) <> 0) OR
                            ((CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0) AND COALESCE(B.S,0) <> 0) OR
                            ((CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0) AND COALESCE(B.D,0) <> 0) OR
                            (COALESCE(P.""ZiLiberaLegala"",0) = COALESCE(B.SL,0) AND COALESCE(B.SL,0) <> 0)) 
                            GROUP BY A.""OreInVal""
                            ORDER BY A.""OreInVal""
                            FOR XML PATH ('')) AS ""ValActive"",


							(SELECT COALESCE(A.""Coloana"",'') + ';'
                            FROM(
                            SELECT ""Coloana"" FROM ""Ptj_tblAdmin"" WHERE SUBSTRING(""Coloana"", 1, 3) = 'Val' AND ""Coloana"" NOT IN('ValAbs', 'ValStr') AND COALESCE(""Blocat"", 0) = 1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                            WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'pontaj.pontajdetaliat' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp'
                            GROUP BY REPLACE(A.""IdColoana"", 'Tmp', '')
                            HAVING MIN(COALESCE(A.""Blocat"",0))= 1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'pontaj.pontajdetaliat' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp'
                            GROUP BY REPLACE(A.""IdColoana"", 'Tmp', '')
                            HAVING MIN(COALESCE(A.""Blocat"",0))= 1
                            ) A
                            GROUP BY A.""Coloana""
                            ORDER BY A.""Coloana""
                            FOR XML PATH('')) AS ""ValSecuritate"",


	                        (select ',' + X.""DenumireScurta"" + '=' + X.""Denumire"" from ( 
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 0 AS ""Tip"" 
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""IdTipOre"" = 1 
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal""
                            ) x
                            WHERE COALESCE(X.DenumireScurta,'') <> '' AND X.""IdContract"" = P.""IdContract"" and X.""IdRol"" = {idRol} AND
                            ( (COALESCE(X.""ZileSapt"",0)=(CASE WHEN P.""ZiSapt""<6 AND P.""ZiLibera""=0 THEN 1 ELSE 0 END) AND COALESCE(X.""ZileSapt"",0) <> 0)
                            OR (COALESCE(X.S,0) = (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) AND COALESCE(X.S,0) <> 0)
                            OR (COALESCE(X.D,0) = (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) AND COALESCE(X.D,0) <> 0)
							OR (COALESCE(X.SL,0) = (CASE WHEN P.""ZiSapt"" < 6 AND COALESCE(P.""ZiLiberaLegala"",0) = 1 THEN 1 ELSE 0 END) AND COALESCE(X.SL,0) <> 0))
                            GROUP BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            ORDER BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            FOR XML PATH ('')) AS ""ValAbsente"",

                            CASE WHEN (
                            CASE WHEN {idRol} = 3 THEN 1 ELSE 
                            CASE WHEN ({idRol} = 2 AND ((COALESCE(J.""IdStare"",1)=1 OR COALESCE(J.""IdStare"",1) = 2 OR COALESCE(J.""IdStare"",1) = 4 OR COALESCE(J.""IdStare"",1) = 6))) THEN 1 ELSE 
                            CASE WHEN ({idRol} = 1 AND(COALESCE(J.""IdStare"", 1) = 1 OR COALESCE(J.""IdStare"", 1) = 4)) THEN 1 ELSE 0
                            END END END)=1 AND
                            (SELECT COUNT(*)
                            FROM ""Ptj_relGrupSuper"" BB
                            INNER JOIN ""relGrupAngajat"" CC ON BB.""IdGrup""  = CC.""IdGrup"" 
                            INNER JOIN ""F100Supervizori"" DD ON CC.F10003 = DD.F10003 AND (-1 * BB.""IdSuper"")= DD.""IdSuper""
                            WHERE DD.F10003=P.F10003 AND BB.""IdRol""={idRol} AND DD.""IdUser""={Session["UserId"]}
                            AND {General.TruncateDate("DD.DataInceput")} <= {General.TruncateDate("P.Ziua")} 
                            AND {General.TruncateDate("P.Ziua")} <=  {General.TruncateDate("DD.DataSfarsit")}) >= 1                            
                            THEN 1 ELSE 0 END AS ""DrepturiModif"", 
                            Fct.F71804 AS ""Functie"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", CA.F72404 AS ""Categorie1"", CB.F72404 AS ""Categorie2"",
                            CASE WHEN 
							    (SELECT COUNT(*) FROM ""Ptj_Cereri"" X
                                INNER JOIN ""Ptj_tblAbsente"" Y ON X.""IdAbsenta""=Y.""Id""
                                WHERE X.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= X.""DataSfarsit"" AND Y.""DenumireScurta""=P.""ValStr"" AND
                                X.F10003=P.F10003 AND X.""IdStare""=3 AND Y.""IdTipOre""=1 AND COALESCE(Y.""NuTrimiteInPontaj"",0) != 1) = 0
                            THEN -55 ELSE (SELECT CASE WHEN COALESCE(""PoateSterge"",0) = 0 THEN -33 ELSE COALESCE(""TipMesaj"",1) END FROM ""Ptj_tblRoluri"" WHERE ""Id""={idRol}) END AS ""tblRoluri_PoateModifica"",
                            ABSE.""DenumireScurta"" AS ""ValAbs""                            
                            FROM ""Ptj_Intrari"" P
                            LEFT JOIN F100 A ON A.F10003 = P.F10003
                            LEFT JOIN F1001 C ON A.F10003=C.F10003
                            LEFT JOIN F002 E ON P.F10002 = E.F00202
                            LEFT JOIN F003 F ON P.F10004 = F.F00304
                            LEFT JOIN F004 G ON P.F10005 = G.F00405
                            LEFT JOIN F005 H ON P.F10006 = H.F00506
                            LEFT JOIN F006 I ON P.F10007 = I.F00607
                            LEFT JOIN ""Ptj_Cumulat"" J ON J.F10003=A.F10003 AND J.""An""={General.FunctiiData("P.\"Ziua\"", "A")} AND J.""Luna""={General.FunctiiData("P.\"Ziua\"", "L")}
                            LEFT JOIN ""Ptj_tblStariPontaj"" K ON COALESCE(J.""IdStare"",1) = K.""Id""
                            LEFT JOIN ""Ptj_Contracte"" L ON P.""IdContract""=L.""Id""
                            LEFT JOIN ""Ptj_Programe"" M ON P.""IdProgram""=M.""Id""

                            LEFT JOIN F007 S7 ON C.F100958 = S7.F00708
                            LEFT JOIN F008 S8 ON C.F100959 = S8.F00809
                            LEFT JOIN F718 Fct ON A.F10071=Fct.F71802
                            LEFT JOIN F724 CA ON A.F10061 = CA.F72402
                            LEFT JOIN F724 CB ON A.F10062 = CB.F72402
                            LEFT JOIN ""Ptj_tblAbsente"" ABSE ON P.""ValStr""=ABSE.""DenumireScurta"" AND ABSE.""DenumireScurta""<>''
                            {strLeg}
                            WHERE CAST(P.""Ziua"" AS DATE) <= A.F10023
                            {filtru}";
                else
                    strSql = $@"SELECT P.*, {General.FunctiiData("P.\"Ziua\"", "Z")} AS ""Zi"", A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"" {valTmp} {furiTmp},
                            {cheia} AS ""Cheia"",
                            E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Dept"",
                            L.""Denumire"" AS ""DescContract"", M.""Denumire"" AS ""DescProgram"", COALESCE(L.""OreSup"",1) AS ""OreSup"", COALESCE(L.""Afisare"",1) AS ""Afisare"",
                            CASE WHEN A.F10022 <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <= A.F10023 AND
                            (SELECT COUNT(*) FROM ""F100Supervizori"" FS WHERE FS.F10003=P.F10003 AND FS.""IdSuper""={idRol} AND FS.""IdUser""={Session["UserId"]} AND {General.TruncateDate("FS.DataInceput")} <= {General.TruncateDate("P.Ziua")} AND {General.TruncateDate("P.Ziua")} <=  {General.TruncateDate("FS.DataSfarsit")}) = 1
                            THEN 1 ELSE 0 END AS ""Activ"",  
                            COALESCE(J.""IdStare"",1) AS ""IdStare"", K.""Culoare"" AS ""CuloareStare"", K.""Denumire"" AS ""NumeStare"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""Ptj_Cereri"" Z 
                            INNER JOIN ""Ptj_tblAbsente"" Y ON Z.""IdAbsenta"" = Y.""Id""
                            WHERE Z.F10003 = P.F10003 AND Z.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= Z.""DataSfarsit"" AND Z.""IdStare"" = 3
                            AND Z.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre"" = 1) AND COALESCE(Y.""NuTrimiteInPontaj"", 0) = 0) = 0 THEN 0 ELSE 1 END AS ""VineDinCereri"", 
                            A.F10022, A.F10023,
                            (SELECT LISTAGG(A.""OreInVal"", ';') WITHIN GROUP (ORDER BY A.""OreInVal"") || ';' AS ""OreInVal""
                            FROM(SELECT * FROM ""Ptj_tblAbsente"" ORDER BY ""OreInVal"") a 
                            INNER JOIN ""Ptj_ContracteAbsente"" b ON a.""Id"" = b.""IdAbsenta""
                            INNER JOIN ""Ptj_relRolAbsenta"" c ON a.""Id"" = c.""IdAbsenta""
                            WHERE A.""OreInVal"" IS NOT NULL AND B.""IdContract""=P.""IdContract"" AND C.""IdRol""={idRol} AND 
                            (((CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0) AND COALESCE(B.ZL,0) <> 0) OR
                            ((CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0) AND COALESCE(B.S,0) <> 0) OR
                            ((CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0) AND COALESCE(B.D,0) <> 0) OR
                            (COALESCE(P.""ZiLiberaLegala"",0) = COALESCE(B.SL,0) AND COALESCE(B.SL,0) <> 0)) 
                            ) AS ""ValActive"",


							(SELECT LISTAGG(A.""Coloana"", ';') WITHIN GROUP (ORDER BY A.""Coloana"") || ';'
                            FROM(
                            SELECT ""Coloana"" FROM ""Ptj_tblAdmin"" WHERE SUBSTR(""Coloana"", 1, 3) = 'Val' AND ""Coloana"" NOT IN('ValAbs', 'ValStr') AND COALESCE(""Blocat"", 0) = 1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                            WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'pontaj.pontajdetaliat' AND SUBSTR(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'pontaj.pontajdetaliat' AND SUBSTR(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
                            ) A
                            ) AS ""ValSecuritate"",


	                        (select  ',' || LISTAGG(X.""DenumireScurta"" || '=' || X.""Denumire"", ',') WITHIN GROUP (ORDER BY X.""Id"", X.""DenumireScurta"", X.""Denumire"") from ( 
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 0 AS ""Tip"" 
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""IdTipOre"" = 1
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal""
                            ) x
                            WHERE X.""DenumireScurta"" IS NOT NULL AND X.""IdContract"" = P.""IdContract"" AND X.""IdRol"" = {idRol} AND
                            ( (COALESCE(X.""ZileSapt"",0)=(CASE WHEN P.""ZiSapt""<6 AND P.""ZiLibera""=0 THEN 1 ELSE 0 END) AND COALESCE(X.""ZileSapt"",0) <> 0)
                            OR (COALESCE(X.S,0) = (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) AND COALESCE(X.S,0) <> 0)
                            OR (COALESCE(X.D,0) = (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) AND COALESCE(X.D,0) <> 0)
							OR (COALESCE(X.SL,0) = (CASE WHEN P.""ZiSapt"" < 6 AND COALESCE(P.""ZiLiberaLegala"",0) = 1 THEN 1 ELSE 0 END) AND COALESCE(X.SL,0) <> 0))
                            ) AS ""ValAbsente"",

                            CASE WHEN (
                            CASE WHEN {idRol} = 3 THEN 1 ELSE 
                            CASE WHEN ({idRol} = 2 AND ((COALESCE(J.""IdStare"",1)=1 OR COALESCE(J.""IdStare"",1) = 2 OR COALESCE(J.""IdStare"",1) = 4 OR COALESCE(J.""IdStare"",1) = 6))) THEN 1 ELSE 
                            CASE WHEN ({idRol} = 1 AND(COALESCE(J.""IdStare"", 1) = 1 OR COALESCE(J.""IdStare"", 1) = 4)) THEN 1 ELSE 0
                            END END END)=1 AND
                            (SELECT COUNT(*)
                            FROM ""Ptj_relGrupSuper"" BB
                            INNER JOIN ""relGrupAngajat"" CC ON BB.""IdGrup""  = CC.""IdGrup"" 
                            INNER JOIN ""F100Supervizori"" DD ON CC.F10003 = DD.F10003 AND (-1 * BB.""IdSuper"")= DD.""IdSuper""
                            WHERE DD.F10003=P.F10003 AND BB.""IdRol""={idRol} AND DD.""IdUser""={Session["UserId"]}
                            AND {General.TruncateDate("DD.DataInceput")} <= {General.TruncateDate("P.Ziua")} 
                            AND {General.TruncateDate("P.Ziua")} <=  {General.TruncateDate("DD.DataSfarsit")}) >= 1                            
                            THEN 1 ELSE 0 END AS ""DrepturiModif"", 
                            Fct.F71804 AS ""Functie"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", CA.F72404 AS ""Categorie1"", CB.F72404 AS ""Categorie2"",
                            CASE WHEN 
							    (SELECT COUNT(*) FROM ""Ptj_Cereri"" X
                                INNER JOIN ""Ptj_tblAbsente"" Y ON X.""IdAbsenta""=Y.""Id""
                                WHERE X.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= X.""DataSfarsit"" AND Y.""DenumireScurta""=P.""ValStr"" AND
                                X.F10003=P.F10003 AND X.""IdStare""=3 AND Y.""IdTipOre""=1 AND COALESCE(Y.""NuTrimiteInPontaj"",0) != 1) = 0
                            THEN -55 ELSE (SELECT CASE WHEN COALESCE(""PoateSterge"",0) = 0 THEN -33 ELSE COALESCE(""TipMesaj"",1) END FROM ""Ptj_tblRoluri"" WHERE ""Id""={idRol}) END AS ""tblRoluri_PoateModifica"",
                            ABSE.""DenumireScurta"" AS ""ValAbs""                            
                            FROM ""Ptj_Intrari"" P
                            LEFT JOIN F100 A ON A.F10003 = P.F10003
                            LEFT JOIN F1001 C ON A.F10003=C.F10003
                            LEFT JOIN F002 E ON P.F10002 = E.F00202
                            LEFT JOIN F003 F ON P.F10004 = F.F00304
                            LEFT JOIN F004 G ON P.F10005 = G.F00405
                            LEFT JOIN F005 H ON P.F10006 = H.F00506
                            LEFT JOIN F006 I ON P.F10007 = I.F00607
                            LEFT JOIN ""Ptj_Cumulat"" J ON J.F10003=A.F10003 AND J.""An""={General.FunctiiData("P.\"Ziua\"", "A")} AND J.""Luna""={General.FunctiiData("P.\"Ziua\"", "L")}
                            LEFT JOIN ""Ptj_tblStariPontaj"" K ON COALESCE(J.""IdStare"",1) = K.""Id""
                            LEFT JOIN ""Ptj_Contracte"" L ON P.""IdContract""=L.""Id""
                            LEFT JOIN ""Ptj_Programe"" M ON P.""IdProgram""=M.""Id""

                            LEFT JOIN F007 S7 ON C.F100958 = S7.F00708
                            LEFT JOIN F008 S8 ON C.F100959 = S8.F00809
                            LEFT JOIN F718 Fct ON A.F10071=Fct.F71802
                            LEFT JOIN F724 CA ON A.F10061 = CA.F72402 
                            LEFT JOIN F724 CB ON A.F10062 = CB.F72402 
                            LEFT JOIN ""Ptj_tblAbsente"" ABSE ON P.""ValStr""=ABSE.""DenumireScurta""
                            {strLeg}
                            WHERE CAST(P.""Ziua"" AS DATE) <= A.F10023
                            {filtru}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public DataTable PontajCuInOut()
        {
            DataTable dt = new DataTable();

            try
            {
                string strSql = SelectPontaj() + $@" ORDER BY A.F10003, {General.TruncateDate("P.Ziua")}";
                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        private void IncarcaRoluri()
        {
            try
            {
                DateTime dtData = Convert.ToDateTime(txtAnLuna.Value);
                if (tip == 2) dtData = Convert.ToDateTime(txtZiua.Value);

                //Radu 31.01.2020 - se doreste sa se puna implicit rolul cel mai mare (am adaugat ORDER BY)
                string strSql = @"select a.""Id"", a.""Denumire"", case when a.""PoateInitializa"" = 1 then 1 else 0 end as ""PoateInitializa"", case when a.""PoateSterge"" = 1 then 1 else 0 end as ""PoateSterge"", COALESCE(a.""TipMesaj"",1) AS ""TipMesaj"" 
                                 from ""Ptj_tblRoluri"" a 
                                 inner join ""Ptj_relGrupSuper"" b on a.""Id"" = b.""IdRol"" 
                                 where b.""IdSuper"" =  {0}
                                 group by a.""Id"", a.""Denumire"", a.""PoateInitializa"",a.""PoateSterge"", a.""TipMesaj"" 
                                 union 
                                 select a.""Id"", a.""Denumire"", case when a.""PoateInitializa"" = 1 then 1 else 0 end as ""PoateInitializa"", case when a.""PoateSterge"" = 1 then 1 else 0 end as ""PoateSterge"", COALESCE(a.""TipMesaj"",1) AS ""TipMesaj"" 
                                 from ""Ptj_tblRoluri"" a 
                                 inner join ""Ptj_relGrupSuper"" b on a.""Id"" = b.""IdRol"" 
                                 inner join ""relGrupAngajat"" c on b.""IdGrup""  = c.""IdGrup"" 
                                 inner join ""F100Supervizori"" d on c.F10003 = d.F10003 and (-1 * b.""IdSuper"")= d.""IdSuper"" 
                                 where d.""IdUser"" =  {0}
                                 group by a.""Id"", a.""Denumire"", a.""PoateInitializa"",a.""PoateSterge"", a.""TipMesaj"" ORDER BY ""Id"" DESC";

                strSql = string.Format(strSql, Session["UserId"]);

                DataTable dtRol = General.IncarcaDT(strSql, null);

                cmbRolAng.DataSource = dtRol;
                cmbRolAng.DataBind();
                cmbRolZi.DataSource = dtRol;
                cmbRolZi.DataBind();

                if (dtRol != null && dtRol.Rows.Count > 0)
                {
                    cmbRolAng.SelectedIndex = 0;
                    cmbRolZi.SelectedIndex = 0;
                }

                if (tip == 2)
                {
                    btnInit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void cmbAng_ItemsRequestedByFilterCondition(object source, DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = (ASPxComboBox)source;
                DataTable dt = General.IncarcaDT(SelectAngajati(" AND (A.F10008 + ' ' + A.F10009) LIKE @1"), new object[] { e.Filter + "%" });
                cmb.DataSource = dt;
                cmb.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbAng_ItemRequestedByValue(object source, DevExpress.Web.ListEditItemRequestedByValueEventArgs e)
        {
            try
            {
                int value = 0;
                if (e.Value == null || !Int32.TryParse(e.Value.ToString(), out value)) return;
                ASPxComboBox cmb = (ASPxComboBox)source;
                DataTable dt = General.IncarcaDT(SelectAngajati(" AND A.F10003 = @1"), new object[] { e.Value });
                cmb.DataSource = dt;
                cmb.DataBind();
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
                if (tip == 2) dtData = Convert.ToDateTime(txtZiua.Value);


                strSql = @"SELECT {0} AS ""IdAuto"", X.* FROM ({4}) X 
                            WHERE X.""IdRol"" = {1} AND X.F10022 <= {2} AND {3} <= X.F10023 {5}
                            ORDER BY X.""NumeComplet"" ";

                strSql = string.Format(strSql, cmp, Convert.ToInt32(cmbRolAng.Value ?? -99), General.ToDataUniv(dtData.Year, dtData.Month, 99), General.ToDataUniv(dtData.Year, dtData.Month), SelectComun(), filtru);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private void IncarcaAngajati()
        {
            try
            {
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2) cmp = "ROWNUM";

                DateTime zi = Convert.ToDateTime(txtAnLuna.Value);

                //Florin 2019.10.01
                string dtInc = General.ToDataUniv(zi.Year, zi.Month, 1);
                string dtSf = General.ToDataUniv(zi.Year, zi.Month, 99);
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                if (tip == 2 || tip == 20)
                {
                    dtInc = General.ToDataUniv(txtZiua.Date);
                    dtSf = General.ToDataUniv(txtZiua.Date);
                }

                string strSql = $@"SELECT {cmp} AS ""IdAuto"", X.* FROM (
                                SELECT B.F10003 AS F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                WHERE C.""IdSuper"" = {Session["UserId"]}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(A.F10025,0) AS F10025
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
                                WHERE J.""IdUser"" = {Session["UserId"]} ) X 
                                WHERE X.""IdRol""={(cmbRolAng.Value ?? -99)} AND X.F10022 <= {dtSf} AND {dtInc} <= X.F10023 AND X.F10025 <> 900
                                ORDER BY X.""NumeComplet"" ";

                DataTable dt = General.IncarcaDT(strSql, null);
                
                cmbAng.DataSource = null;
                cmbAng.DataSource = dt;
                cmbAng.DataBind();

                cmbAngZi.DataSource = null;
                cmbAngZi.DataSource = dt;
                cmbAngZi.DataBind();

                Session["Pontaj_Angajati"] = dt;

                if (tip == 1 && txtAnLuna.Value != null && dt != null && dt.Rows.Count > 0) cmbAng.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void MetodeInitializare(int actiune)
        {
            try
            {
                //1 - initializeaza
                //2 - sterge initializarea

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Nu exista inregistrari", MessageBox.icoError, "Eroare !");
                    return;
                }

                int idRol = Convert.ToInt32(cmbRolAng.Value ?? -99);
                if (tip == 2) idRol = Convert.ToInt32(cmbRolZi.Value ?? -99);
                int idStare = 1;

                DataRow dr = dt.Rows[0];
                idStare = Convert.ToInt32(dr["IdStare"] ?? 1);

                if (!VerifDrepturi(idRol, idStare))
                {
                    MessageBox.Show("Nu aveti drepturi pentru aceasta actiune", MessageBox.icoError, "Eroare !");
                    return;
                }

                DateTime dtData = txtAnLuna.Date;
                DateTime dtInc = new DateTime(dtData.Year, dtData.Month, 1);
                DateTime dtSf = new DateTime(dtData.Year, dtData.Month, DateTime.DaysInMonth(dtData.Year, dtData.Month));

                if (!(dtInc <= Convert.ToDateTime(dr["F10023"]) && Convert.ToDateTime(dr["F10022"]) <= dtSf))
                {
                    MessageBox.Show("Acest angajat nu mai este activ", MessageBox.icoError, "Eroare !");
                    return;
                }

                DataTable dtModif = dt.Clone();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];

                    //daca inca este angajat si este zi lucratoare
                    switch (actiune)
                    {
                        case 1:
                            if (Convert.ToInt32(row["Activ"]) == 1 && Convert.ToInt32(row["ZiLibera"]) == 0 && Convert.ToInt32(row["ZiLiberaLegala"]) == 0 && Convert.ToInt32(row["ZiSapt"]) < 6)
                            {
                                if (General.Nz(row["ValStr"], "").ToString() == "" && General.Nz(row["Norma"], "").ToString() != "")
                                {
                                    if (Convert.ToDateTime(dr["F10022"]) <= Convert.ToDateTime(row["Ziua"]) && Convert.ToDateTime(row["Ziua"]) <= Convert.ToDateTime(dr["F10023"]))
                                    {
                                        row["ValStr"] = row["Norma"];
                                        row["Val0"] = Convert.ToInt32(General.Nz(row["Norma"], 0)) * 60;
                                        //este = true;
                                    }
                                }
                            }
                            break;
                        case 2:
                            {
                                //stergem toate valurile - o sa le repopulam dupa caz
                                for (int x = 0; x <= 20; x++)
                                {
                                    row["Val" + x] = DBNull.Value;
                                }

                                //verificam daca nu cumva avem o cerere de absenta aprobata
                                DataTable dtCer = General.IncarcaDT(
                                    $@"SELECT COALESCE(B.""IdTipOre"",1) AS ""IdTipOre"", ""OreInVal"", COALESCE(""NrOre"",0) AS ""NrOre"" FROM ""Ptj_Cereri"" A 
                                    INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta""=B.""Id""
                                    WHERE A.F10003={row["F10003"]} AND A.""IdStare"" = 3 
                                    AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(row["Ziua"]))} 
                                    AND {General.ToDataUniv(Convert.ToDateTime(row["Ziua"]))} <= {General.TruncateDate("A.DataSfarsit")}  ", null);
                                if (dtCer != null && dtCer.Rows.Count > 0)
                                {
                                    //daca este absenta de tip ora mai executam codul
                                    if (Convert.ToInt32(dtCer.Rows[0]["IdTipOre"]) == 0)
                                    {
                                        //golim ValStr
                                        row["ValStr"] = DBNull.Value;
                                        //Adaugam valurile din cerere/cereri
                                        for (int j = 0; j < dtCer.Rows.Count; j++)
                                        {
                                            if (General.Nz(dtCer.Rows[j]["OreInVal"], "").ToString() != "")
                                            {
                                                row[dtCer.Rows[j]["OreInVal"].ToString()] = Math.Round(Convert.ToDecimal(dtCer.Rows[j]["NrOre"]) * 60);
                                            }
                                        }
                                    }

                                    dtModif.ImportRow(row);
                                }
                                else
                                    row["ValStr"] = DBNull.Value;
                            }
                            break;
                    }
                }

                General.SalveazaDate(dt, "Ptj_Intrari");

                for (int i = 0; i < dtModif.Rows.Count; i++)
                {
                    General.CalculFormule(dtModif.Rows[i]["F10003"], null, Convert.ToDateTime(dtModif.Rows[i]["Ziua"]), null);
                    //General.ExecValStr(Convert.ToInt32(dtModif.Rows[i]["F10003"]), Convert.ToDateTime(dtModif.Rows[i]["Ziua"]));
                }

                btnFiltru_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public static bool VerifDrepturi(int idRol, int idStare)
        {
            bool areDrepturi = true;

            try
            {
                switch (idRol)
                {
                    case 0:                             //  angajat
                        if (idStare != 1 && idStare != 4)
                        {
                            areDrepturi = false;
                        }
                        break;
                    case 1:                             //  introducere
                        if (idStare != 1 && idStare != 4)
                        {
                            areDrepturi = false;
                        }
                        break;
                    case 2:                             //  manager
                        if (idStare == 3 || idStare == 5 || idStare == 7)
                        {
                            areDrepturi = false;
                        }
                        break;
                    case 3:                             //  HR
                        //NOP                           are voie sa modifice orice
                        break;
                    case 4:                             //  HR extern - totul este read-only
                        areDrepturi = false;
                        break;
                }

            }
            catch (Exception)
            {
                areDrepturi = false;
            }

            return areDrepturi;
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
                if (tip == 2) dtData = Convert.ToDateTime(txtZiua.Value);


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

#region grDate


        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName.Length >= 3)
                {
                    switch (e.DataColumn.FieldName.ToLower().Substring(0, 3))
                    {
                        case "in1":
                        case "in2":
                        case "in3":
                        case "in4":
                        case "in5":
                        case "in6":
                        case "in7":
                        case "in8":
                        case "in9":
                        case "out":
                            {
                                if (e.CellValue != null && e.CellValue != DBNull.Value)
                                {
                                    e.Cell.ToolTip = e.GetValue(e.DataColumn.FieldName).ToString();
                                    
                                    object obj = grDate.GetRowValues(e.VisibleIndex, "Ziua");
                                    DateTime inOut = Convert.ToDateTime(e.CellValue);
                                    DateTime zi = Convert.ToDateTime(obj);
                                    if (inOut.Date != zi.Date) e.Cell.BackColor = System.Drawing.Color.LightGray;
                                }
                            }
                            break;
                        case "val":
                            {
                                string col = e.DataColumn.FieldName;
                                if (Constante.lstValuri.IndexOf(col.Replace("Tmp", "") + ";") >= 0)
                                {
                                    //setam culoarea pentru weekend si sarbatori legale


                                    //setam culoarea de fundal daca a fost modificata
                                    object obj = grDate.GetRowValues(e.VisibleIndex, "ValModif" + col.Replace("ValTmp", ""));

                                    if (General.Nz(obj, "").ToString() != "")
                                    {
                                        if (General.CuloarePontaj(Convert.ToInt32(obj)) != null) e.Cell.BackColor = (System.Drawing.Color)General.CuloarePontaj(Convert.ToInt32(obj));
                                    }

                                }
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

        protected void grDate_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e)
        {
            try
            {
                int ziSapt = Convert.ToInt32(e.GetValue("ZiSapt"));
                int ziLib = Convert.ToInt32(e.GetValue("ZiLibera"));
                if (ziSapt == 6 || ziSapt == 7 || ziLib == 1)
                {
                    e.Row.BackColor = System.Drawing.Color.Aquamarine;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Florin 2020.02.07 - am adaugat functia de validare; pt acest lucru am adaugat valorile in row si am facut selectul care se trimite ca parametru in validari din acest row;
        //Florin 2019.10.31 - am refacut intreaga functie

        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                grDate.CancelEdit();

                DataTable dt = Session["InformatiaCurenta"] as DataTable;

                ASPxDataUpdateValues upd = e.UpdateValues[0] as ASPxDataUpdateValues;
                object[] keys = new object[] { upd.Keys[0] };

                DataRow row = dt.Rows.Find(keys);
                if (row == null) return;

                bool adaugat = false;
                bool absentaDeTipZi = false;
                int f10003 = Convert.ToInt32(row["F10003"]);
                DateTime ziua = Convert.ToDateTime(row["Ziua"]);
                string cmp = "";
                string strSql = "";
                var dic = upd.NewValues.Cast<DictionaryEntry>().OrderBy(r => r.Key).ToDictionary(c => c.Key, d => d.Value);

                foreach (var l in dic)
                {
                    string numeCol = l.Key.ToString();
                    dynamic oldValue = upd.OldValues[numeCol];
                    dynamic newValue = upd.NewValues[numeCol];
                    if (oldValue != null && upd.OldValues[numeCol].GetType() == typeof(System.DBNull))
                        oldValue = null;
                       
                    if (oldValue != newValue)
                    {
                        //daca sunt valorile temporare ValTmp
                        if (numeCol.Length >= 5 && numeCol.Substring(0,6).ToLower() == "valtmp" && General.IsNumeric(numeCol.Replace("ValTmp", "")))
                        {
                            string i = numeCol.Replace("ValTmp", "");
                            if (!adaugat)
                            {
                                cmp += @", ""CuloareValoare""='" + Constante.CuloareModificatManual + "'";
                                row["CuloareValoare"] = Constante.CuloareModificatManual;
                                adaugat = true;
                            }
                            cmp += $@", ""ValModif{i}""=" + (int)Constante.TipModificarePontaj.ModificatManual;
                            cmp += $@", ""Val{i}""=" + (Convert.ToInt32(Convert.ToDateTime(newValue).Minute) + Convert.ToInt32((Convert.ToDateTime(newValue).Hour * 60))).ToString();

                            row["ValModif" + i] = (int)Constante.TipModificarePontaj.ModificatManual;
                            row["Val" + i] = Convert.ToInt32(Convert.ToDateTime(newValue).Minute) + Convert.ToInt32((Convert.ToDateTime(newValue).Hour * 60));

                            continue;
                        }

                        //daca sunt In-uri
                        if (newValue != null && numeCol.Length >= 2 && numeCol.Substring(0,2).ToLower() == "in" && General.IsNumeric(numeCol.Replace("In", "")))
                        {
                            int i = Convert.ToInt32(numeCol.ToLower().Replace("in", ""));
                            DateTime inOut = Convert.ToDateTime(newValue);
                            if (inOut.Year == 100)
                                inOut = new DateTime(ziua.Year, ziua.Month, ziua.Day, Convert.ToDateTime(newValue).Hour, Convert.ToDateTime(newValue).Minute, Convert.ToDateTime(newValue).Second);
                            try
                            {
                                if (i > 1 && row["Out" + (i - 1)] != DBNull.Value && Convert.ToDateTime(row["Out" + (i - 1)]) > inOut)
                                    row[numeCol] = inOut.AddDays(1);
                                else
                                    row[numeCol] = inOut;

                                cmp += $@", ""{numeCol}""=" + General.ToDataUniv(Convert.ToDateTime(row[numeCol]), true);
                                row[numeCol] = inOut;
                            }
                            catch (Exception ex)
                            {
                                string edc = ex.Message;
                            }

                            continue;
                        }

                        //daca sunt Out-uri
                        if (newValue != null && numeCol.Length >= 3 && numeCol.Substring(0,3).ToLower() == "out" && General.IsNumeric(numeCol.Replace("Out", "")))
                        {
                            int i = Convert.ToInt32(numeCol.ToLower().Replace("out", ""));
                            DateTime inOut = Convert.ToDateTime(newValue);
                            if (inOut.Year == 100)
                                inOut = new DateTime(ziua.Year, ziua.Month, ziua.Day, Convert.ToDateTime(newValue).Hour, Convert.ToDateTime(newValue).Minute, Convert.ToDateTime(newValue).Second);
                            try
                            {
                                var ert = row["In" + i];
                                if (row["In" + i] != DBNull.Value && Convert.ToDateTime(row["In" + i]) > inOut)
                                    row[numeCol] = inOut.AddDays(1);
                                else
                                    row[numeCol] = inOut;

                                cmp += $@", ""{numeCol}""=" + General.ToDataUniv(Convert.ToDateTime(row[numeCol]), true);
                            }
                            catch (Exception ex)
                            {
                                string edc = ex.Message;
                            }
                            continue;
                        }

                        //daca este valstr inseram in istoric
                        if (numeCol.ToLower() == "valstr")
                        {
                            strSql += $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                            VALUES({f10003}, {General.ToDataUniv(ziua)}, '{newValue}', '{oldValue}', {Session["UserID"]}, {General.CurrentDate()}, 'Pontajul Detaliat - modificare pontare', {Session["UserId"]}, {General.CurrentDate()});" + Environment.NewLine;
                        }

                        //daca este ValAbs, stergem pontajul pe centrii de cost
                        if (numeCol.ToLower() == "valabs")
                        {
                            if (newValue != null)
                            {
                                absentaDeTipZi = true;
                                if (Dami.ValoareParam("PontajCCStergeDacaAbsentaDeTipZi") == "1")
                                    strSql += $@"DELETE FROM ""Ptj_CC"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};" + Environment.NewLine;
                            }
                            continue;
                        }


                        //daca sunt F-urile temporare FTmp
                        if (numeCol.IndexOf("FTmp") >= 0)
                        {
                            string i = numeCol.Replace("FTmp", "");
                            cmp += $@", ""F{i}""=" + (Convert.ToInt32(Convert.ToDateTime(newValue).Minute) + Convert.ToInt32((Convert.ToDateTime(newValue).Hour * 60))).ToString();
                            row["F" + i] = Convert.ToInt32(Convert.ToDateTime(newValue).Minute) + Convert.ToInt32((Convert.ToDateTime(newValue).Hour * 60));
                            continue;
                        }

                        //daca sunt restul campurilor
                        if (newValue == null)
                        {
                            cmp += $@", ""{numeCol}"" = NULL";
                            row[numeCol] = DBNull.Value;
                        }
                        else
                        {
                            switch (row.Table.Columns[numeCol].DataType.ToString())
                            {
                                case "System.String":
                                    cmp += $@", ""{numeCol}""='{newValue}'";
                                    break;
                                case "System.DateTime":
                                    cmp += $@", ""{numeCol}""={General.ToDataUniv(newValue)}";
                                    break;
                                default:
                                    cmp += $@", ""{numeCol}""={newValue}";
                                    break;
                            }

                            row[numeCol] = newValue;
                        }
                    }
                }

                if (cmp != "")
                    strSql += $@"UPDATE ""Ptj_Intrari"" SET {cmp.Substring(1)}, USER_NO={Session["UserId"]}, TIME={General.CurrentDate()} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)};" + Environment.NewLine;

                //Florin 2020.02.07 - am adaugat validarile
                string msg = "";
                if (strSql != "")
                {
                    string sqlPtj = General.CreazaSelectFromRow(row);
                    msg = Notif.TrimiteNotificare("Pontaj.PontajDetaliat", (int)Constante.TipNotificare.Validare, sqlPtj, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                    if (msg != "" && msg.Substring(0, 1) == "2")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg.Substring(2));
                        e.Handled = true;
                        IncarcaGrid();
                        return;
                    }
                    else
                        General.ExecutaNonQuery(
                            "BEGIN " + Environment.NewLine +
                            strSql + Environment.NewLine +
                            "END;", null);
                }

                DataRow dr = General.IncarcaDR($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);
                if (dr != null)
                {
                    if (!absentaDeTipZi)
                    {
                        Calcul.AlocaContract(f10003, ziua);
                        Calcul.CalculInOut(dr, true, true);
                    }

                    General.CalculFormule(f10003, null, ziua, null);
                    //General.ExecValStr(f10003, ziua);
                }

                IncarcaGrid();

                //Florin 2020.02.07
                if (msg != "" && msg.Substring(0, 1) != "2")
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes, dar cu urmatorul avertisment: " + msg);
                //else
                //    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");

                e.Handled = true;
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

                    switch (arr[0])
                    {
                        case "btnRecalcParam":
                            {
                                if (arr.Length != 5 || arr[0] == "" || arr[1] == "" || arr[2] == "" || arr[3] == "" || arr[4] == "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                                    return;
                                }

                                string strSql = $@"SELECT A.* FROM ""Ptj_Intrari"" A
                                                LEFT JOIN ""Ptj_tblAbsente"" C ON A.""ValStr""=C.""DenumireScurta""
                                                WHERE @1 <= A.F10003 AND A.F10003 <= @2 AND @3 <= A.""Ziua"" AND A.""Ziua"" <= @4 AND C.""DenumireScurta"" IS NULL";

                                DateTime dtInc = new DateTime(Convert.ToInt32(arr[1].Split('/')[2]), Convert.ToInt32(arr[1].Split('/')[1]), Convert.ToInt32(arr[1].Split('/')[0]));
                                DateTime dtSf = new DateTime(Convert.ToInt32(arr[2].Split('/')[2]), Convert.ToInt32(arr[2].Split('/')[1]), Convert.ToInt32(arr[2].Split('/')[0]));

                                DataTable dt = General.IncarcaDT(strSql, new object[] { arr[3], arr[4], dtInc, dtSf });

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    Calcul.AlocaContract(Convert.ToInt32(dt.Rows[i]["F10003"].ToString()), Convert.ToDateTime(dt.Rows[i]["Ziua"]));
                                    Calcul.CalculInOut(dt.Rows[i], true, true);
                                }

                                General.CalculFormule(arr[3], arr[4], dtInc, dtSf);
                                IncarcaGrid();
                            }
                            break;
                        case "btnDelete":
                            MetodeInitializare(2);
                            break;
                        case "colHide":
                            grDate.Columns[arr[1]].Visible = false;
                            break;
                        case "btnFiltru":
                            if (arr.Length > 1)
                            {
                                switch (General.Nz(arr[1],"").ToString())
                                {
                                    case "0":
                                        if (cmbAng.SelectedIndex > 0) cmbAng.SelectedIndex -= 1;
                                        break;
                                    case "1":
                                        if (cmbAng.SelectedIndex < cmbAng.Items.Count) cmbAng.SelectedIndex += 1;
                                        break;
                                }
                            }

                            btnFiltru_Click(null, null);
                            break;
                        case "btnAproba":
                            Actiuni(1);
                            break;
                        case "btnRespins":
                            Actiuni(0);
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

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                //if (e.Column.FieldName.Length >= 6 && e.Column.FieldName.Substring(0, 6) == "ValTmp")

                if (e.Column.FieldName.IndexOf("ValTmp") >= 0 || e.Column.FieldName.IndexOf("FTmp") >= 0)
                    e.Editor.ReadOnly = false;
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
                var clientData = new Dictionary<int, object>();
                var lstDrepturi = new Dictionary<int, int>();
                var lstValAbsente = new Dictionary<int, object>();
                var lstValSec = new Dictionary<int, object>();
                var lstAfisare = new Dictionary<int, object>();
                var lstPoateModifica = new Dictionary<int, object>();
                var lstZiSapt = new Dictionary<int, object>();

                var grid = sender as ASPxGridView;
                for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
                {
                    var rowValues = grid.GetRowValues(i, new string[] { "Cheia", "ValActive", "DrepturiModif", "ValAbsente", "ValSecuritate", "Afisare", "tblRoluri_PoateModifica", "ZiSapt" }) as object[];
                    clientData.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
                    lstDrepturi.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), Convert.ToInt32(rowValues[2] ?? 0));
                    lstValAbsente.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[3] ?? "");
                    lstValSec.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[4] ?? "");
                    lstAfisare.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[5] ?? "");
                    lstPoateModifica.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[6] ?? "");
                    lstZiSapt.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[7] ?? "");
                }

                grid.JSProperties["cp_cellsToDisable"] = clientData;
                grid.JSProperties["cp_cellsDrepturi"] = lstDrepturi;
                grid.JSProperties["cp_ValAbsente"] = lstValAbsente;
                grid.JSProperties["cp_ValSec"] = lstValSec;
                grid.JSProperties["cp_Afisare"] = lstAfisare;
                grid.JSProperties["cp_PoateModifica"] = lstPoateModifica;
                grid.JSProperties["cp_ZiSapt"] = lstZiSapt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


#endregion 




        private void Actiuni(int actiune)
        {
            try
            {
                int idRol = Convert.ToInt32(cmbRolAng.Value ?? 1);
                if (tip == 2) idRol = Convert.ToInt32(cmbRolZi.Value ?? 1);

                if (!General.DrepturiAprobare(actiune, idRol))
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie");
                else
                {
                    int f10003 = Convert.ToInt32(cmbAng.Value ?? -99);
                    int idStare = 1;
                    DataRowView entCu = grDate.GetRow(0) as DataRowView;
                    if (entCu != null) idStare = Convert.ToInt32(entCu["IdStare"] ?? 1);

                    string mesaj = General.ActiuniExec(actiune, f10003, idRol, idStare, Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, "Pontaj.PontajDetaliat", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["User_Marca"]));
                    if (actiune == 1)
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Aprobare") + Environment.NewLine + Dami.TraduCuvant(mesaj);
                    else
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Respingere") + Environment.NewLine + Dami.TraduCuvant(mesaj);
                    btnFiltru_Click(null, null);
                }
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
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE " +
                            (Constante.tipBD == 1 ? "F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00623" : "F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00623"), null);
                        cmbDept.DataBind();
                        return;
                    case "cmbRolAng":
                    case "cmbRolZi":
                    case "txtAnLuna":
                    case "txtZiua":
                        IncarcaAngajati();
                        esteStruc = false;
                        //Radu 15.01.2020
                        GetDataBlocare(Convert.ToDateTime(txtAnLuna.Value));
                        break;
                }

                if (esteStruc)
                {
                    cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00411 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00412" : " AND F00411 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00412"), null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00513 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00514" : " AND F00513 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00514"), null);
                    cmbSec.DataBind();
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE " +
                            (Constante.tipBD == 1 ? "F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00623" : "F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00623"), null);
                        cmbDept.DataBind();
                    }
                    else
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99) +
                             (Constante.tipBD == 1 ? " AND F00622 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00623" : " AND F00622 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00623"), null);
                        cmbDept.DataBind();
                    }
                    cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99) +
                        (Constante.tipBD == 1 ? " AND F00714 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00715" : " AND F00714 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00715"), null);
                    cmbSubDept.DataBind();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CreeazaGrid()
        {
            try
            {
                string cmp = "SUBSTRING";
                if (Constante.tipBD == 2)
                    cmp = "SUBSTR";
                DataTable dtCol = General.IncarcaDT($@"SELECT A.*, CASE WHEN A.""Blocat"" < COALESCE(C.""Blocat"",0) THEN COALESCE(C.""Blocat"",0) ELSE A.""Blocat"" END AS ""BlocatBis"", 
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("DenumireScurta")} THEN REPLACE(B.""DenumireScurta"",' ','') ELSE A.""Coloana"" END AS ""ColDen"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("DenumireScurta")} THEN B.""DenumireScurta"" ELSE A.""Alias"" END AS ""ColAlias"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("Denumire")} THEN B.""Denumire"" ELSE (CASE WHEN 1=1 {General.FiltrulCuNull("AliasToolTip")} THEN A.""AliasToolTip"" ELSE A.""Coloana"" END) END AS ""ColTT"",
                                COALESCE(B.""DenumireScurta"",'') AS ""ColScurta"",
                                CASE WHEN A.""Coloana"" ='Stare' THEN 0 ELSE 1 END AS ""OrdineSec""
                                FROM ""Ptj_tblAdmin"" A
                                LEFT JOIN (SELECT ""OreInVal"", MAX(""Denumire"") AS ""Denumire"", MAX(""DenumireScurta"") AS ""DenumireScurta"" FROM ""Ptj_tblAbsente"" WHERE 1=1 {General.FiltrulCuNull("OreInVal")} GROUP BY ""OreInVal"") B ON A.""Coloana""=B.""OreInVal""
                                LEFT JOIN (SELECT X.""IdColoana"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                                                FROM ""Securitate"" A
                                                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                                                WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'Pontaj.PontajDetaliat' AND A.""IdControl"" = 'grDate'
                                                                UNION
                                                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                                                FROM ""Securitate"" A
                                                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Pontaj.PontajDetaliat' AND A.""IdControl"" = 'grDate') X
                                                                GROUP BY X.""IdColoana"") C ON A.""Coloana"" = C.""IdColoana""
                                WHERE COALESCE(A.""Vizibil"",0)=1
                                ORDER BY ""OrdineSec"", A.""Ordine"" ", null);

                if (dtCol != null)
                {
                    DataSet ds = new DataSet();

                    for (int i = 0; i < dtCol.Rows.Count; i++)
                    {
                        DataRow dr = dtCol.Rows[i];
                        string colField = General.Nz(dr["Coloana"], "col" + i).ToString();
                        string colName = General.Nz(dr["ColDen"], "col" + i).ToString() + "_" + General.Nz(dr["ColScurta"], "").ToString();
                        string alias = General.Nz(dr["ColAlias"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool vizibil = Convert.ToBoolean(General.Nz(dr["Vizibil"], false));
                        bool blocat = Convert.ToBoolean(General.Nz(dr["BlocatBis"], false));
                        int latime = Convert.ToInt32(General.Nz(dr["Latime"], 80));
                        int tipCol = Convert.ToInt32(General.Nz(dr["TipColoana"], 1));
                        string tt = General.Nz(dr["ColTT"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool unb = false;

                        if (Constante.lstValuri.IndexOf(colField + ";") >= 0)
                        {
                            unb = true;
                            colField = "ValTmp" + colField.Replace("Val", "");
                        }

                        if (colField.ToLower() == "valabs")
                            unb = true;

                        if (colField == "Stare")
                        {
                            if (i == 0 && vizibil && (tip == 1 || tip == 10))
                            {
                                GridViewBandColumn banda = new GridViewBandColumn();
                                banda.Name = colField;
                                grDate.Columns.Add(banda);
                            }
                            continue;
                        }

                        if (blocat)
                            lstInOut += colField + ";";

                        dynamic c = new GridViewDataColumn();

                        switch (tipCol)
                        {
                            case 0:                             //General
                                {
                                    c = new GridViewDataColumn();
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                }
                                break;
                            case 1:                             //CheckBox
                                {
                                    c = new GridViewDataCheckColumn();
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
                                }
                                break;
                            case 2:                             //ComboBox
                                {
                                    c = new GridViewDataComboBoxColumn();
                                    c.PropertiesComboBox.AllowNull = true;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                    {
                                        string sursa = (dr["SursaCombo"] as string ?? "").ToString().Trim();
                                        DataTable dtCmb = General.IncarcaDT(sursa, null);
                                        dtCmb.TableName = colField;
                                        ds.Tables.Add(dtCmb);

                                        Session["SurseCombo"] = ds;

                                        c.PropertiesComboBox.DropDownWidth = 350;
                                        c.PropertiesComboBox.DataSource = dtCmb;
                                        c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
                                        c.PropertiesComboBox.ValueType = dtCmb.Columns[0].DataType;
                                        c.PropertiesComboBox.TextFormatString = "{0}";
                                        switch (dtCmb.Columns.Count)
                                        {
                                            case 1:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
                                                break;
                                            case 2:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
                                                break;
                                        }

                                        if (c.FieldName == "IdContract")
                                        {
                                            c.PropertiesComboBox.ClientInstanceName = "cmbContract";
                                            c.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "function(s,e) { cmbContract_SelectedIndexChanged_Client(s,e); }";
                                        }

                                        if (c.FieldName == "IdProgram")
                                        {
                                            c.PropertiesComboBox.ClientInstanceName = "cmbProgram";
                                            DataTable dtPrg = General.IncarcaDT(
                                                $@"SELECT A.""IdContract"", A.""IdProgram"", B.""Denumire"" AS ""Program"", A.""TipSchimb"" AS ""ZiSapt""
                                                FROM ""Ptj_ContracteSchimburi"" A
                                                INNER JOIN ""Ptj_Programe"" B ON A.""IdProgram"" = B.""Id""
                                                GROUP BY A.""IdContract"", A.""IdProgram"", B.""Denumire"", A.""TipSchimb""
                                                ORDER BY B.""Denumire""", null);
                                            if (dtPrg != null && dtPrg.Rows.Count > 0)
                                            {
                                                string jsonPrg = "";
                                                for (int g = 0; g < dtPrg.Rows.Count; g++)
                                                {
                                                    jsonPrg += ",{ idContract: " + dtPrg.Rows[g]["IdContract"] + ", program: '" + General.Nz(dtPrg.Rows[g]["Program"], "").ToString().Trim().Replace("\n", "").Replace("\r", "") + "', idProgram: " + dtPrg.Rows[g]["IdProgram"] + ", ziSapt: " + dtPrg.Rows[g]["ZiSapt"] + " }";
                                                }
                                                if (jsonPrg.Length > 0)
                                                    Session["Json_Programe"] = "[" + jsonPrg.Substring(1) + "]";
                                            }
                                        }
                                    }
                                }
                                break;
                            case 3:                             //Date
                                {
                                    c = new GridViewDataDateColumn();
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                                }
                                break;
                            case 4:                             //Memo
                                {
                                    c = new GridViewDataMemoColumn();
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                }
                                break;
                            case 5:                             //Color
                                {
                                    c = new GridViewDataColorEditColumn();
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                }
                                break;
                            case 6:                             //Text
                                {
                                    c = new GridViewDataTextColumn();
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                }
                                break;
                            case 7:                             //Numeric
                                {
                                    c = new GridViewDataSpinEditColumn();
                                    c.PropertiesSpinEdit.DecimalPlaces = 0;
                                    c.PropertiesSpinEdit.NumberType = SpinEditNumberType.Integer;
                                    c.PropertiesSpinEdit.MinValue = 0;
                                    c.PropertiesSpinEdit.MaxValue = 2000;
                                    c.PropertiesSpinEdit.DisplayFormatString = "N0";
                                    c.PropertiesSpinEdit.DisplayFormatInEditMode = true;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                                }
                                break;
                            case 8:                             //Time
                            case 9:                             //Time - fara spin buttons
                                {
                                    c = new GridViewDataTimeEditColumn();
                                    c.PropertiesTimeEdit.AllowNull = true;
                                    c.PropertiesTimeEdit.DisplayFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.DisplayFormatInEditMode = true;
                                    c.PropertiesTimeEdit.EditFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.EditFormat = EditFormat.DateTime;
                                    if (tipCol == 9)
                                        c.PropertiesTimeEdit.SpinButtons.ShowIncrementButtons = false;

                                    if (Constante.lstFuri.IndexOf(colField + ";") >= 0)
                                    {
                                        unb = true;
                                        colField = "FTmp" + colField.Replace("F", "");
                                    }
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
                                }
                                break;
                        }

                        c.Name = colName;
                        c.FieldName = colField;
                        c.Caption = Dami.TraduCuvant(alias);
                        c.Visible = vizibil;
                        c.ReadOnly = blocat;
                        c.Width = Unit.Pixel(latime);
                        c.VisibleIndex = i + 4;
                        c.ToolTip = tt;

                        if (c.FieldName.Length >= 6 && c.FieldName.ToLower().Substring(0, 6) == "valtmp")
                            c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                        if (grDate.Columns["Stare"] != null)
                            grDate.Columns["Stare"].Columns.Add(c);
                        else
                            grDate.Columns.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //private void CreazaGrid()
        //{
        //    try
        //    {
        //        string cmp = "SUBSTRING";
        //        if (Constante.tipBD == 2)
        //            cmp = "SUBSTR";
        //        DataTable dtCol = General.IncarcaDT($@"SELECT A.*, CASE WHEN A.""Blocat"" < COALESCE(C.""Blocat"",0) THEN COALESCE(C.""Blocat"",0) ELSE A.""Blocat"" END AS ""BlocatBis"", 
        //                        CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("DenumireScurta")} THEN REPLACE(B.""DenumireScurta"",' ','') ELSE A.""Coloana"" END AS ""ColDen"",
        //                        CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("DenumireScurta")} THEN B.""DenumireScurta"" ELSE A.""Alias"" END AS ""ColAlias"",
        //                        CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' {General.FiltrulCuNull("Denumire")} THEN B.""Denumire"" ELSE (CASE WHEN 1=1 {General.FiltrulCuNull("AliasToolTip")} THEN A.""AliasToolTip"" ELSE A.""Coloana"" END) END AS ""ColTT"",
        //                        COALESCE(B.""DenumireScurta"",'') AS ""ColScurta"",
        //                        CASE WHEN A.""Coloana"" ='Stare' THEN 0 ELSE 1 END AS ""OrdineSec""
        //                        FROM ""Ptj_tblAdmin"" A
        //                        LEFT JOIN (SELECT ""OreInVal"", MAX(""Denumire"") AS ""Denumire"", MAX(""DenumireScurta"") AS ""DenumireScurta"" FROM ""Ptj_tblAbsente"" WHERE ""OreInVal""='Val4' GROUP BY ""OreInVal"") B ON A.""Coloana""=B.""OreInVal""
        //                        LEFT JOIN (SELECT X.""IdColoana"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
        //                                                        SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
        //                                                        FROM ""Securitate"" A
        //                                                        INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
        //                                                        WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'Pontaj.PontajDetaliat' AND A.""IdControl"" = 'grDate'
        //                                                        UNION
        //                                                        SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
        //                                                        FROM ""Securitate"" A
        //                                                        WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Pontaj.PontajDetaliat' AND A.""IdControl"" = 'grDate') X
        //                                                        GROUP BY X.""IdColoana"") C ON A.""Coloana"" = C.""IdColoana""
        //                        WHERE COALESCE(A.""Vizibil"",0)=1
        //                        ORDER BY ""OrdineSec"", A.""Ordine"" ", null);

        //        if (dtCol != null)
        //        {
        //            DataSet ds = new DataSet();

        //            for (int i = 0; i < dtCol.Rows.Count; i++)
        //            {
        //                DataRow dr = dtCol.Rows[i];
        //                string colField = General.Nz(dr["Coloana"], "col" + i).ToString();
        //                string colName = General.Nz(dr["ColDen"], "col" + i).ToString() + "_" + General.Nz(dr["ColScurta"], "").ToString();
        //                string alias = General.Nz(dr["ColAlias"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
        //                bool vizibil = Convert.ToBoolean(General.Nz(dr["Vizibil"], false));
        //                bool blocat = Convert.ToBoolean(General.Nz(dr["BlocatBis"], false));
        //                int latime = Convert.ToInt32(General.Nz(dr["Latime"], 80));
        //                int tipCol = Convert.ToInt32(General.Nz(dr["TipColoana"],1));
        //                string tt = General.Nz(dr["ColTT"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
        //                bool unb = false;

        //                if (Constante.lstValuri.IndexOf(colField + ";") >= 0)
        //                {
        //                    unb = true;
        //                    colField = "ValTmp" + colField.Replace("Val","");
        //                }

        //                if (colField == "Stare")
        //                {
        //                    if (i == 0 && vizibil && (tip == 1 || tip == 10))
        //                    {
        //                        GridViewBandColumn banda = new GridViewBandColumn();
        //                        banda.Name = colField;
        //                        grDate.Columns.Add(banda); 
        //                    }
        //                    continue;
        //                }

        //                if (blocat && colField.Length >= 3 && ((colField.Substring(0,2).ToLower() == "in" && General.IsNumeric(colField.ToLower().Replace("in",""))) || (colField.Substring(0, 3).ToLower() == "out" && General.IsNumeric(colField.ToLower().Replace("out", "")))))
        //                {
        //                    lstInOut += colField + ";";
        //                }

        //                switch (tipCol)
        //                {
        //                    case 0:                             //General
        //                        {
        //                            GridViewDataColumn c = new GridViewDataColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 1:                             //CheckBox
        //                        {
        //                            GridViewDataCheckColumn c = new GridViewDataCheckColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 2:                             //ComboBox
        //                        {

        //                            GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

        //                            if (colName.Length >= 6 && colName.Substring(0, 6) == "ValAbs")
        //                            {
        //                                c.UnboundType = DevExpress.Data.UnboundColumnType.String;
        //                            }

        //                            c.PropertiesComboBox.AllowNull = true;

        //                            if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
        //                            {
        //                                string sursa = (dr["SursaCombo"] as string ?? "").ToString().Trim();
        //                                DataTable dtCmb = General.IncarcaDT(sursa, null);
        //                                dtCmb.TableName = colField;
        //                                ds.Tables.Add(dtCmb);

        //                                Session["SurseCombo"] = ds;

        //                                c.PropertiesComboBox.DropDownWidth = 350;
        //                                c.PropertiesComboBox.DataSource = dtCmb;
        //                                c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
        //                                c.PropertiesComboBox.ValueType = dtCmb.Columns[0].DataType;
        //                                c.PropertiesComboBox.TextFormatString = "{0}";
        //                                switch (dtCmb.Columns.Count)
        //                                {
        //                                    case 1:
        //                                        c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
        //                                        break;
        //                                    case 2:
        //                                        c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
        //                                        break;
        //                                }

        //                                if (c.FieldName == "IdContract")
        //                                {
        //                                    c.PropertiesComboBox.ClientInstanceName = "cmbContract";
        //                                    c.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "function(s,e) { cmbContract_SelectedIndexChanged_Client(s,e); }";
        //                                }

        //                                if (c.FieldName == "IdProgram")
        //                                {
        //                                    c.PropertiesComboBox.ClientInstanceName = "cmbProgram";
        //                                    //DataTable dtPrg = General.IncarcaDT(
        //                                    //    $@"SELECT A.""IdContract"", A.""IdProgram"", B.""Denumire"" AS ""Program""
        //                                    //    FROM ""Ptj_ContracteSchimburi"" A
        //                                    //    INNER JOIN ""Ptj_Programe"" B ON A.""IdProgram""=B.""Id""
        //                                    //    ORDER BY B.""Denumire"" ", null);
        //                                    DataTable dtPrg = General.IncarcaDT(
        //                                        $@"SELECT A.""IdContract"", A.""IdProgram"", B.""Denumire"" AS ""Program"", A.""TipSchimb"" AS ""ZiSapt""
        //                                        FROM ""Ptj_ContracteSchimburi"" A
        //                                        INNER JOIN ""Ptj_Programe"" B ON A.""IdProgram"" = B.""Id""
        //                                        GROUP BY A.""IdContract"", A.""IdProgram"", B.""Denumire"", A.""TipSchimb""
        //                                        ORDER BY B.""Denumire""", null);
        //                                    if (dtPrg != null && dtPrg.Rows.Count > 0)
        //                                    {
        //                                        string jsonPrg = "";
        //                                        for(int g = 0; g < dtPrg.Rows.Count; g++)
        //                                        {
        //                                            jsonPrg += ",{ idContract: " + dtPrg.Rows[g]["IdContract"] + ", program: '" + General.Nz(dtPrg.Rows[g]["Program"],"").ToString().Trim().Replace("\n","").Replace("\r","") + "', idProgram: " + dtPrg.Rows[g]["IdProgram"] + ", ziSapt: " + dtPrg.Rows[g]["ZiSapt"] + " }";
        //                                        }
        //                                        if (jsonPrg.Length > 0)
        //                                            Session["Json_Programe"] = "[" + jsonPrg.Substring(1) + "]";
        //                                    }
        //                                }
        //                            }

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 3:                             //Date
        //                        {
        //                            GridViewDataDateColumn c = new GridViewDataDateColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 4:                             //Memo
        //                        {
        //                            GridViewDataMemoColumn c = new GridViewDataMemoColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 5:                             //Color
        //                        {
        //                            GridViewDataColorEditColumn c = new GridViewDataColorEditColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 6:                             //Text
        //                        {
        //                            GridViewDataTextColumn c = new GridViewDataTextColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 7:                             //Numeric
        //                        {
        //                            GridViewDataSpinEditColumn c = new GridViewDataSpinEditColumn();
        //                            //c.Name = colName;
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
        //                    case 8:                             //Time
        //                    case 9:                             //Time - fara spin buttons
        //                        {
        //                            GridViewDataTimeEditColumn c = new GridViewDataTimeEditColumn();
        //                            c.Name = colName;
        //                            c.FieldName = colField;
        //                            c.Caption = Dami.TraduCuvant(alias);
        //                            c.Visible = vizibil;
        //                            c.ReadOnly = blocat;
        //                            c.Width = Unit.Pixel(latime);
        //                            c.VisibleIndex = i + 4;
        //                            c.ToolTip = tt;
        //                            c.PropertiesTimeEdit.AllowNull = true;

        //                            if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

        //                            c.PropertiesTimeEdit.DisplayFormatString = "HH:mm";
        //                            c.PropertiesTimeEdit.DisplayFormatInEditMode = true;
        //                            c.PropertiesTimeEdit.EditFormatString = "HH:mm";
        //                            c.PropertiesTimeEdit.EditFormat = EditFormat.DateTime;

        //                            if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
        //                                c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

        //                            //Florin 2019.12.11
        //                            if (tipCol == 9)
        //                                c.PropertiesTimeEdit.SpinButtons.ShowIncrementButtons = false;

        //                            if (grDate.Columns["Stare"] != null)
        //                                grDate.Columns["Stare"].Columns.Add(c);
        //                            else
        //                                grDate.Columns.Add(c);
        //                        }
        //                        break;
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

        private List<object> ValoriChei()
        {
            List<object> lst = new List<object>();

            lst.Add(cmbAng.Value);
            lst.Add(txtAnLuna.Value);

            try
            {
                if (!ccValori.Contains("cheia")) return lst;
                
                int tip = Convert.ToInt32(General.Nz(Request["tip"], 1));
                if (tip == 1 || tip == 10)
                {
                    DateTime dtTmp = Convert.ToDateTime(txtAnLuna.Value);
                    lst[1] = new DateTime(dtTmp.Year, dtTmp.Month, Convert.ToInt32(ccValori["cheia"]), 0, 0, 0);
                }
                else
                    lst[0] = Convert.ToInt32(ccValori["cheia"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lst;
        }


        protected void btnSaveCC_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["PtjCC"] as DataTable;
                General.SalveazaDate(dt, "Ptj_CC");

                if (Dami.ValoareParam("PontajCCCalculTotalPeZi", "0") == "1")
                {
                    string cmp = "";
                    DataTable dtAdm = Session["Ptj_tblAdminCC"] as DataTable;

                    for (int i = 0; i < dtAdm.Rows.Count; i++)
                    {
                        DataRow dr = dtAdm.Rows[i];
                        if (General.Nz(dr["Destinatie"], "").ToString() != "" && dr["Camp"].ToString().Substring(0, 5) == "NrOre")
                        {
                            cmp += ", " + dr["Destinatie"] + "=(SELECT SUM(" + dr["Camp"] + ")  FROM \"Ptj_CC\" WHERE F10003=@1 AND Ziua=@2)";
                        }
                    }

                    if (cmp != "")
                    {
                        //transferam suma minutelor din CC in Ptj_Intrari
                        List<object> lst = ValoriChei();
                        string sqlVal = $@"UPDATE ""Ptj_Intrari"" SET {cmp.Substring(1)} WHERE F10003=@1 AND Ziua=@2;";
                        sqlVal = sqlVal.Replace("@1", lst[0].ToString()).Replace("@2", General.ToDataUniv(Convert.ToDateTime(lst[1])));

                        //refacem ValStr
                        string sqlTot = $@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" ={General.CalculValStr(Convert.ToInt32(lst[0]), Convert.ToDateTime(lst[1]), "", "", 0)}
                                        WHERE  F10003={lst[0].ToString()} AND ""Ziua"" ={General.ToDataUniv(Convert.ToDateTime(lst[1]))};";

                        General.ExecutaNonQuery("BEGIN" + "\n\r" +
                                                sqlVal + "\n\r" +
                                                sqlTot + "\n\r" +
                                                "END;"
                                                , null);

                        General.CalculFormule(lst[0], null, Convert.ToDateTime(lst[1]), null);
                    }
                }           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {             
                List<object> lst = ValoriChei();

                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length == 0 || arr[0] == "")
                    {
                        grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnCC":
                            {
                                lblZiuaCC.Text = "Centrii de cost - Ziua " + Convert.ToDateTime(lst[1]).Day;
                                DataTable dt = SursaCC(Convert.ToInt32(lst[0]), General.ToDataUniv(Convert.ToDateTime(lst[1])));
                                Session["PtjCC"] = dt;
                                grCC.KeyFieldName = "IdAuto";
                                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                                grCC.DataSource = dt;
                                grCC.DataBind();
                            }
                            break;
                        case "btnDeleteCC":
                            {
                                DataTable dt = Session["PtjCC"] as DataTable;
                                object[] arrKey = arr[1].Split('|');
                                DataRow found = dt.Rows.Find(arrKey);
                                found.Delete();

                                btnSaveCC_Click(null, null);

                                Session["PtjCC"] = dt;
                                grCC.KeyFieldName = "IdAuto";
                                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                                grCC.DataSource = dt;
                                grCC.DataBind();
                            }
                            break;
                        case "cmbPro":
                            {
                                ASPxComboBox cmbSubPro = ((ASPxComboBox)grCC.FindEditRowCellTemplateControl(grCC.Columns["IdSubproiect"] as GridViewDataComboBoxColumn, "cmbSubPro"));
                                if (cmbSubPro == null) return;
                                cmbSubPro.SelectedIndex = -1;

                                if (arr.Count() > 1 && arr[1] != null && arr[1].ToString() != "")
                                {                                    
                                    DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT D.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ", null);

                                    cmbSubPro.DataSource = dt;
                                    cmbSubPro.DataBind();
                                    Session["PtjCC_SubProiecte"] = dt;
                                }
                                else
                                {
                                    cmbSubPro.DataSource = null;
                                    cmbSubPro.DataBind();
                                    Session["PtjCC_SubProiecte"] = null;
                                }
                            }
                            break;
                        case "cmbSubPro":
                            {
                                ASPxComboBox cmbAct = ((ASPxComboBox)grCC.FindEditRowCellTemplateControl(grCC.Columns["IdActivitate"] as GridViewDataComboBoxColumn, "cmbAct"));
                                if (cmbAct == null) return;
                                cmbAct.SelectedIndex = -1;

                                if (arr.Count() > 2 && arr[1] != null && arr[1].ToString() != "" && arr[2] != null && arr[2].ToString() != "")
                                {
                                    DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT E.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ", null);

                                    cmbAct.DataSource = dt;
                                    cmbAct.DataBind();
                                    Session["PtjCC_Activitati"] = dt;
                                }
                                else
                                {
                                    cmbAct.DataSource = null;
                                    cmbAct.DataBind();
                                    Session["PtjCC_Activitati"] = null;
                                }
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

        protected void grCC_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = General.Nz(e.GetValue("IdStare"),"").ToString();
                    if (idStare != "")
                    {
                        DataRow[] lst = dt.Select("Id=" + idStare);
                        if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                        {
                            e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
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

        private void LoadPro(ASPxComboBox cmb)
        {
            try
            {
                List<object> lst = ValoriChei();

                DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" WHERE F10003={lst[0]} AND {General.TruncateDate("DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("DataSfarsit")}) = 0)
                                                    SELECT * FROM ""tblProiecte""
                                                    ELSE
                                                    SELECT B.* FROM ""Ptj_relAngajatProiect"" A 
                                                    INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                    WHERE A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ", null);

                cmb.DataSource = dt;
                cmb.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void LoadSubPro(ASPxComboBox cmb, int idPro)
        {
            try
            {
                List<object> lst = ValoriChei();

                DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT D.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ", null);

                cmb.DataSource = dt;
                cmb.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void LoadAct(ASPxComboBox cmb, int idPro, int idSub)
        {
            try
            {
                List<object> lst = ValoriChei();

                DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND C.""IdSubproiect""={idSub} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT E.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND C.""IdSubproiect""={idSub} AND A.F10003 = {lst[0]} AND {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDate("A.DataSfarsit")} ", null);
                
                cmb.DataSource = dt;
                cmb.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        void cmbSubPro_OnCallback(object source, CallbackEventArgsBase e)
        {
            try
            {
                LoadSubPro(source as ASPxComboBox, Convert.ToInt32(e.Parameter));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        void cmbAct_OnCallback(object source, CallbackEventArgsBase e)
        {
            try
            {
                var idPro = grCC.GetRowValues(grCC.EditingRowVisibleIndex, (new string[] { "IdProiect" }));
                LoadAct(source as ASPxComboBox,1, Convert.ToInt32(e.Parameter));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #region Pontaj Init
        protected void btnInitParam_Click(object sender, EventArgs e)
        {
            try
            {
                if (!chkNormaSD.Checked && !chkNormaSL.Checked && !chkNormaZL.Checked)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date"), MessageBox.icoInfo, "Initializare");
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(txtAnLuna.Value);
                    bool ras = General.PontajInit(Convert.ToInt32(Session["UserId"]), dt.Year, dt.Month, -99, chkNormaZL.Checked, chkCCCu.Checked, cmbDept.Text, Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), cmbCtr.Text, chkNormaSD.Checked, chkNormaSL.Checked, false, 0, Convert.ToInt32(chkInOut.Checked));

                    if (ras)
                    {
                        btnFiltru_Click(sender, null);
                        MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Initializare");
                    }
                    else
                        MessageBox.Show(Dami.TraduCuvant("Eroare la initializare"), MessageBox.icoInfo, "Initializare");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        protected void grCC_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                bool suntModif = false;
                List<object> lst = ValoriChei();
                grDate.CancelEdit();
                DataTable dt = Session["PtjCC"] as DataTable;
                if (dt == null) return;
                 
                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
                    DataRow dr = dt.NewRow();

                    //Radu 04.12.2019 - campurile "De" si "La" trebuie sa contina si ziua, nu doar ora
                    DateTime oraDeLa = Convert.ToDateTime(upd.NewValues["De"] ?? new DateTime(2100, 1, 1, 0, 0, 0));
                    DateTime oraLa = Convert.ToDateTime(upd.NewValues["La"] ?? new DateTime(2100, 1, 1, 0, 0, 0));

                    dr["F10003"] = lst[0];
                    dr["Ziua"] = Convert.ToDateTime(lst[1]);
                    dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                    dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                    dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                    dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                    dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                    if (upd.NewValues["De"] == null)
                        dr["De"] = DBNull.Value;
                    else
                        dr["De"] = new DateTime(Convert.ToDateTime(lst[1]).Year, Convert.ToDateTime(lst[1]).Month, Convert.ToDateTime(lst[1]).Day, oraDeLa.Hour, oraDeLa.Minute, oraDeLa.Second);
                    if (upd.NewValues["La"] == null)
                        dr["La"] = DBNull.Value;
                    else
                        dr["La"] = new DateTime(Convert.ToDateTime(lst[1]).Year, Convert.ToDateTime(lst[1]).Month, Convert.ToDateTime(lst[1]).Day, oraLa.Hour, oraLa.Minute, oraLa.Second);
                    dr["NrOre1"] = upd.NewValues["NrOre1"] ?? DBNull.Value;
                    dr["NrOre2"] = upd.NewValues["NrOre2"] ?? DBNull.Value;
                    dr["NrOre3"] = upd.NewValues["NrOre3"] ?? DBNull.Value;
                    dr["NrOre4"] = upd.NewValues["NrOre4"] ?? DBNull.Value;
                    dr["NrOre5"] = upd.NewValues["NrOre5"] ?? DBNull.Value;
                    dr["NrOre6"] = upd.NewValues["NrOre6"] ?? DBNull.Value;
                    dr["NrOre7"] = upd.NewValues["NrOre7"] ?? DBNull.Value;
                    dr["NrOre8"] = upd.NewValues["NrOre8"] ?? DBNull.Value;
                    dr["NrOre9"] = upd.NewValues["NrOre9"] ?? DBNull.Value;
                    dr["NrOre10"] = upd.NewValues["NrOre10"] ?? DBNull.Value;
                    if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "0")
                        dr["IdStare"] = 3;
                    else
                        dr["IdStare"] = upd.NewValues["IdStare"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);

                    suntModif = true;
                }


                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    //Radu 04.12.2019 - campurile "De" si "La" trebuie sa contina si ziua, nu doar ora
                    DateTime oraDeLa = Convert.ToDateTime(upd.NewValues["De"] ?? new DateTime(2100, 1, 1, 0, 0, 0));
                    DateTime oraLa = Convert.ToDateTime(upd.NewValues["La"] ?? new DateTime(2100, 1, 1, 0, 0, 0));

                    if (upd.NewValues["F06204"] != null) dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                    if (upd.NewValues["IdProiect"] != null) dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                    if (upd.NewValues["IdSubproiect"] != null) dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                    if (upd.NewValues["IdActivitate"] != null) dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                    if (upd.NewValues["IdDept"] != null) dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                    if (upd.NewValues["De"] != null) dr["De"] = new DateTime(Convert.ToDateTime(lst[1]).Year, Convert.ToDateTime(lst[1]).Month, Convert.ToDateTime(lst[1]).Day, oraDeLa.Hour, oraDeLa.Minute, oraDeLa.Second);
                    if (upd.NewValues["La"] != null) dr["La"] = new DateTime(Convert.ToDateTime(lst[1]).Year, Convert.ToDateTime(lst[1]).Month, Convert.ToDateTime(lst[1]).Day, oraLa.Hour, oraLa.Minute, oraLa.Second);
                    if (upd.NewValues["NrOre1"] != null) dr["NrOre1"] = upd.NewValues["NrOre1"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre2"] != null) dr["NrOre2"] = upd.NewValues["NrOre2"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre3"] != null) dr["NrOre3"] = upd.NewValues["NrOre3"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre4"] != null) dr["NrOre4"] = upd.NewValues["NrOre4"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre5"] != null) dr["NrOre5"] = upd.NewValues["NrOre5"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre6"] != null) dr["NrOre6"] = upd.NewValues["NrOre6"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre7"] != null) dr["NrOre7"] = upd.NewValues["NrOre7"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre8"] != null) dr["NrOre8"] = upd.NewValues["NrOre8"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre9"] != null) dr["NrOre9"] = upd.NewValues["NrOre9"] ?? DBNull.Value;
                    if (upd.NewValues["NrOre10"] != null) dr["NrOre10"] = upd.NewValues["NrOre10"] ?? DBNull.Value;
                    if (upd.NewValues["IdStare"] != null) dr["IdStare"] = upd.NewValues["IdStare"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    suntModif = true;
                }


                //daca avem linii modificate
                for (int i = 0; i < e.DeleteValues.Count; i++)
                {
                    ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dt.Rows.Remove(dr);

                    grCC.DataSource = dt;
                    grCC.DataBind();

                    suntModif = true;
                }

                if (suntModif == true)
                {
                    bool faraErori = true;
                    DataTable dtInt = General.IncarcaDT($@"SELECT COALESCE(B.""Norma"",8) AS ""Norma"", CASE WHEN C.""DenumireScurta"" IS NULL THEN 0 ELSE 1 END AS ""EsteAbsenta"" 
                        FROM ""Ptj_Intrari"" B
                        LEFT JOIN ""Ptj_tblAbsente"" C ON B.""ValStr""=C.""DenumireScurta""
                        WHERE B.F10003={lst[0]} AND B.""Ziua""={General.ToDataUniv(Convert.ToDateTime(lst[1]))}", null);
                    if (dtInt.Rows.Count > 0)
                    {
                        var ert = dt.Compute("Sum(NrOre1)", string.Empty);
                        int total = Convert.ToInt32(General.Nz(dt.Compute("Sum(NrOre1)", string.Empty), 0));
                        //Florin 2019.11.04 - am adaugat parametrul PontajCCVerificareDepasireNorma
                        if (General.Nz(Dami.ValoareParam("PontajCCVerificareDepasireNorma"), 0).ToString() == "1" && total > (Convert.ToInt32(General.Nz(dtInt.Rows[0]["Norma"], 8)) * 60))
                        {
                            grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Suma orelor depaseste norma");
                            faraErori = false;
                        }

                        if (Convert.ToInt32(General.Nz(dtInt.Rows[0]["EsteAbsenta"], 0)) == 1 && Dami.ValoareParam("PontajCCCalculTotalPeZi") == "1")
                        {
                            grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate salva deoarece exista absenta de tip zi");
                            faraErori = false;
                        }
                    }

                    if (faraErori)
                        btnSaveCC_Click(null, null);
                }

                e.Handled = true;

                Session["PtjCC"] = dt;
                grCC.DataSource = dt;
                grCC.DataBind();

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                grCC.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ex.Message);
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }

        private DataTable SursaCC(int f10003, string ziua)
        {
            DataTable dt = new DataTable();

            try
            {
                //string strCmp = "";
                //for (int i = 1; i <= 10; i++)
                //{
                //    if (Constante.tipBD == 1)
                //        strCmp += $@",CONVERT(datetime,DATEADD(minute, NrOre{i}, '')) AS NrOre{i}_Tmp ";
                //    else
                //        strCmp += $@",TO_DATE('01-01-1900','DD-MM-YYYY') + NrOre{i}/1440 AS ""NrOre{i}_Tmp"" ";
                //}

                dt = General.IncarcaDT($@"SELECT *
                        FROM ""Ptj_CC"" A 
                        WHERE A.F10003={f10003} AND A.""Ziua""={ziua}", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        private void IncarcaCC()
        {
            try
            {
                List<object> lst = ValoriChei();

                lblZiuaCC.Text = "Centrii de cost - Ziua " + Convert.ToDateTime(lst[1]).Day;
                DataTable dt = SursaCC(Convert.ToInt32(lst[0]), General.ToDataUniv(Convert.ToDateTime(lst[1])));
                Session["PtjCC"] = dt;
                grCC.KeyFieldName = "IdAuto";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                grCC.DataSource = dt;
                grCC.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grCC_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
        {
            if (e != null && e.Column != null && e.Column.FieldName != null & e.Column.FieldName.ToString().Length >= 5 && e.Column.FieldName.Substring(0, 5) == "NrOre")
            {
                switch (tipAfisareCC)
                {
                    case 1:         //minutes
                        //NOP
                        break;
                    case 2:         //houres
                        {
                            var ts = TimeSpan.FromMinutes(Convert.ToDouble(General.Nz(e.Value, 0)));
                            e.DisplayText = ((int)ts.TotalHours).ToString();
                        }
                        break;
                    case 3:         //HH:MM
                        {
                            var ts = TimeSpan.FromMinutes(Convert.ToDouble(General.Nz(e.Value, 0)));
                            e.DisplayText = string.Format("{0:00}:{1:00}", (int)ts.TotalHours, ts.Minutes);
                        }
                        break;
                }
            }
        }

        protected void CreeazaGridTotaluri()
        {
            try
            {
                DataTable dtCol = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""OrdineAfisarePontajDetaliat"",0) > 0 ORDER BY ""OrdineAfisarePontajDetaliat"" ", null);

                if (dtCol != null && dtCol.Rows.Count > 0)
                {
                    string cmp = "";

                    ASPxGridView grDate = new ASPxGridView();
                    grDate.ID = "grDateTotaluri";
                    grDate.ClientInstanceName = "grDateTotaluri";
                    grDate.Width = Unit.Percentage(100);
                    grDate.AutoGenerateColumns = false;

                    for (int i = 0; i < dtCol.Rows.Count; i++)
                    {
                        DataRow dr = dtCol.Rows[i];
                        if (General.Nz(dr["Coloana"], "").ToString() == "")
                            continue;
                        string colField = General.Nz(dr["Coloana"], "col" + i).ToString();
                        cmp += "," + colField;
                        string alias = General.Nz(dr["Alias"], "").ToString();
                        string toolTip = General.Nz(dr["Explicatii"], "").ToString();

                        GridViewDataColumn c = new GridViewDataColumn();
                        c.Name = colField;
                        c.FieldName = colField;
                        c.Caption = Dami.TraduCuvant(alias);
                        c.ToolTip = toolTip;
                        c.ReadOnly = true;
                        grDate.Columns.Add(c);
                    }

                    if (cmp == "")
                        return;

                    DataTable dt = General.IncarcaDT($@"SELECT {cmp.Substring(1)} FROM ""Ptj_Cumulat"" WHERE F10003=@1 AND ""An""=@2 AND ""Luna""=@3 ", new object[] { General.Nz(cmbAng.Value,-99), txtAnLuna.Date.Year, txtAnLuna.Date.Month } );
                    grDate.DataSource = dt;
                    grDate.DataBind();

                    tdGridTotaluri.Controls.Add(grDate);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 15.01.2020
        private void GetDataBlocare(DateTime dataInc)
        {
            string idRol = "-99";
            string strSql = $@"SELECT X.""IdRol"", X.""RolDenumire"" FROM ({SelectComun(dataInc)}) X 
                                WHERE X.F10022 <= {General.ToDataUniv(dataInc.Year, dataInc.Month, 99)} AND {General.ToDataUniv(dataInc.Year, dataInc.Month)} <= X.F10023
                                GROUP BY X.""IdRol"", X.""RolDenumire""
                                ORDER BY X.""RolDenumire"" ";
            DataTable dtRol = General.IncarcaDT(strSql, null);
            if (dtRol != null && dtRol.Rows.Count > 0)
            {
                idRol = "";
                for (int i = 0; i < dtRol.Rows.Count; i++)
                    idRol += dtRol.Rows[i]["IdRol"].ToString() + ",";
                idRol = idRol.Substring(0, idRol.Length - 1);
            }

            //Radu 09.01.2020
            string dataBlocare = "22001231";
            strSql = $@"SELECT COALESCE(MIN(Ziua),'2200-12-31') FROM Ptj_tblBlocarePontaj WHERE IdRol IN (" + idRol + ")";
            if (Constante.tipBD == 2)
                strSql = @"SELECT COALESCE(MIN(""Ziua""),TO_DATE('31-12-2200','DD-MM-YYYY')) FROM ""Ptj_tblBlocarePontaj"" WHERE ""IdRol"" IN (" + idRol + ")";
            DataTable dt = General.IncarcaDT(strSql, null);
            if (dt != null && dt.Rows.Count > 0 && General.Nz(dt.Rows[0][0], "").ToString() != "" && General.IsDate(dt.Rows[0][0]))
                dataBlocare = Convert.ToDateTime(dt.Rows[0][0]).Year + Convert.ToDateTime(dt.Rows[0][0]).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(dt.Rows[0][0]).Day.ToString().PadLeft(2, '0');
            Session["Ptj_DataBlocare"] = dataBlocare.ToString();
        }

        private string SelectComun(DateTime dtInc)
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

                DateTime dtData = dtInc;

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

        private void CreeazaGridCC()
        {
            try
            {
                DataTable dtCol = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAdminCC"" WHERE ""Camp"" LIKE 'NrOre%' AND COALESCE(""Vizibil"",0)=1 ORDER BY ""Ordine"" ", null);
                Session["Ptj_tblAdminCC"] = dtCol;
                //for (int i = 0; i < dtAd.Rows.Count; i++)
                //{
                //    string cmp = dtAd.Rows[i]["Camp"].ToString();

                //    grCC.Columns[cmp].Visible = Convert.ToBoolean(dtAd.Rows[i]["Vizibil"]);
                //    grCC.Columns[cmp].ToolTip = Dami.TraduCuvant(General.Nz(dtAd.Rows[i]["AliasToolTip"], "").ToString());
                //    grCC.Columns[cmp].Caption = Dami.TraduCuvant(General.Nz(dtAd.Rows[i]["Alias"], dtAd.Rows[i]["Camp"]).ToString());
                //}

                for (int i = 0; i < dtCol.Rows.Count; i++)
                {
                    DataRow dr = dtCol.Rows[i];
                    string colField = General.Nz(dr["Camp"], "col" + i).ToString();
                    string colName = General.Nz(dr["Camp"], "col" + i).ToString();
                    string alias = Dami.TraduCuvant(General.Nz(dtCol.Rows[i]["Alias"], dtCol.Rows[i]["Camp"]).ToString());
                    string tt = Dami.TraduCuvant(General.Nz(dtCol.Rows[i]["AliasToolTip"], dtCol.Rows[i]["Camp"]).ToString());
                    int tipCol = Convert.ToInt32(General.Nz(dr["TipColoana"], 1));

                    dynamic c = new GridViewDataColumn();

                    if (colName.ToLower().IndexOf("nrore") >= 0 && tipCol != 7 && tipCol != 8 && tipCol != 9)
                        tipCol = 7;

                    switch (tipCol)
                    {
                        case 0:                             //General
                            {
                                c = new GridViewDataColumn();
                                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                            }
                            break;
                        case 1:                             //CheckBox
                            {
                                c = new GridViewDataCheckColumn();
                            }
                            break;
                        case 2:                             //ComboBox
                            {
                                c = new GridViewDataComboBoxColumn();
                                c.PropertiesComboBox.AllowNull = true;

                                if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                {
                                    string sursa = (dr["SursaCombo"] as string ?? "").ToString().Trim();
                                    DataTable dtCmb = General.IncarcaDT(sursa, null);
                                    dtCmb.TableName = colField;
                                    //ds.Tables.Add(dtCmb);

                                    //Session["SurseCombo"] = ds;

                                    c.PropertiesComboBox.DropDownWidth = 350;
                                    c.PropertiesComboBox.DataSource = dtCmb;
                                    c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
                                    c.PropertiesComboBox.ValueType = dtCmb.Columns[0].DataType;
                                    c.PropertiesComboBox.TextFormatString = "{0}";
                                    switch (dtCmb.Columns.Count)
                                    {
                                        case 1:
                                            c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
                                            break;
                                        case 2:
                                            c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
                                            break;
                                    }
                                }
                            }
                            break;
                        case 3:                             //Date
                            {
                                c = new GridViewDataDateColumn();
                            }
                            break;
                        case 4:                             //Memo
                            {
                                c = new GridViewDataMemoColumn();
                                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                            }
                            break;
                        case 5:                             //Color
                            {
                                c = new GridViewDataColorEditColumn();
                            }
                            break;
                        case 6:                             //Text
                            {
                                c = new GridViewDataTextColumn();
                                c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                            }
                            break;
                        case 7:                             //Numeric
                            {
                                c = new GridViewDataSpinEditColumn();
                                c.PropertiesSpinEdit.DecimalPlaces = 0;
                                c.PropertiesSpinEdit.NumberType = SpinEditNumberType.Integer;
                                c.PropertiesSpinEdit.MinValue = 0;
                                c.PropertiesSpinEdit.MaxValue = 2000;
                                c.PropertiesSpinEdit.DisplayFormatString = "N0";
                                c.PropertiesSpinEdit.DisplayFormatInEditMode = true;
                                GridViewDataSpinEditColumn cc = new GridViewDataSpinEditColumn();
                            }
                            break;
                        case 8:                             //Time
                        case 9:                             //Time - fara spin buttons
                            {
                                c = new GridViewDataTimeEditColumn();
                                c.PropertiesTimeEdit.AllowNull = true;
                                c.PropertiesTimeEdit.DisplayFormatString = "HH:mm";
                                c.PropertiesTimeEdit.DisplayFormatInEditMode = true;
                                c.PropertiesTimeEdit.EditFormatString = "HH:mm";
                                c.PropertiesTimeEdit.EditFormat = EditFormat.DateTime;
                                if (tipCol == 9)
                                    c.PropertiesTimeEdit.SpinButtons.ShowIncrementButtons = false;
                            }
                            break;
                    }

                    c.Name = colName;
                    c.FieldName = colField;
                    c.Caption = Dami.TraduCuvant(alias);
                    c.VisibleIndex = i + 4;
                    c.ToolTip = tt;
                    c.Width = Unit.Pixel(100);

                    grCC.Columns.Add(c);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}