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
        string cmp = "USER_NO,TIME,IDAUTO,";

        public class metaAbsTipZi
        {
            public int F10003 { get; set; }
            public DateTime Ziua { get; set; }
        }


        public class metaIds
        {
            public Nullable<int> F10003 { get; set; }
            public Nullable<int> Zi { get; set; }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                if (!IsPostBack)
                {
                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));

                    if (tip == 1 || tip == 10)
                    {
                        divPeAng.Style["display"] = "inline-block";
                        divPeZi.Style["display"] = "none";
                        grDate.Columns["NumeComplet"].Visible = false;

                        btnFiltruSterge.Visible = false;

                        grDate.Columns["Cheia"].Caption = "Ziua";
                    }
                    else
                    {
                        divPeAng.Style["display"] = "none";
                        divPeZi.Style["display"] = "inline-block";
                        grDate.Columns["NumeComplet"].Visible = true;

                        btnAproba.Visible = false;
                        btnRespins.Visible = false;
                        btnInit.Visible = false;
                        btnDelete.Visible = false;
                        btnFiltruSterge.Visible = true;

                        grDate.Columns["Cheia"].Caption = "Marca";
                    }

                    CreazaGrid();

                    DataTable dtVal = General.IncarcaDT(Constante.tipBD == 1 ? @"SELECT TOP 0 * FROM ""Ptj_IstoricVal"" " : @"SELECT * FROM ""Ptj_IstoricVal"" WHERE ROWNUM = 0 ", null);
                    Session["Ptj_IstoricVal"] = dtVal;
                }

                if (Dami.ValoareParam("PontajulAreCC") == "1" && (tip == 1 || tip == 10))
                {
                    //grCC.Visible = true;
                    grDate.Columns[0].Visible = true;
                    //lblZiuaCC.Visible = true;
                    //btnSaveCC.Visible = true;
                    tblCC.Attributes["class"] = "vizibil";

                    if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "1")
                        grCC.Columns["IdStare"].Visible = true;

                    DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                    GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    colStari.PropertiesComboBox.DataSource = dtStari;


                    if (!IsPostBack)
                    {
                        DataTable dtAd = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAdminCC"" ", null);
                        Session["Ptj_tblAdminCC"] = dtAd;
                        for (int i = 0; i < dtAd.Rows.Count; i++)
                        {
                            grCC.Columns[dtAd.Rows[i]["Camp"].ToString()].Visible = Convert.ToBoolean(dtAd.Rows[i]["Vizibil"]);
                            grCC.Columns[dtAd.Rows[i]["Camp"].ToString()].ToolTip = General.Nz(dtAd.Rows[i]["AliasToolTip"],"").ToString();
                            grCC.Columns[dtAd.Rows[i]["Camp"].ToString()].Caption = General.Nz(dtAd.Rows[i]["Alias"], dtAd.Rows[i]["Camp"]).ToString();
                        }
                    }
                }

                if (tip == 2 || tip == 20)
                {
                    //Florin 2018.09.25
                    grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ", "10"));
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
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnPtjEchipa.Text = Dami.TraduCuvant("btnPtjEchipa", "Pontajul Echipei");
                //btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblAnLuna.InnerText = Dami.TraduCuvant("Luna/An");
                lblRolAng.InnerText = Dami.TraduCuvant("Roluri");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblStare.InnerText = Dami.TraduCuvant("Stare");

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

                #endregion

                if (tip == 1 || tip == 10)
                    txtTitlu.Text = Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj pe angajat");
                else
                    txtTitlu.Text = Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj") + " - " + Dami.TraduCuvant("Pontaj pe zi");

                //tip = Convert.ToInt32(General.Nz(Request["tip"], 1));

                if (!IsPostBack)
                {
                    txtAnLuna.Value = DateTime.Now;
                    txtZiua.Value = DateTime.Now;

                    Session["InformatiaCurenta"] = null;

                    IncarcaRoluri();

                    if (tip == 1) IncarcaAngajati();

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

                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }

                }


                if (tip == 1 || tip == 10)
                {
                    grDate.SettingsPager.PageSize = 31;
                }
                else
                {
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
                    cmbBirou.DataSource = General.IncarcaDT("select F00809, F00810 from F008", null);
                    cmbBirou.DataBind();

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
                        DataTable dt = General.IncarcaDT("SELECT * FROM F062", null);
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
                        string ziua = General.Nz(Request.QueryString["ziua"], "").ToString().Replace("Ziua","");
                        string f10003 = General.Nz(Request.QueryString["f10003"], "").ToString();
                        if (ziua != "" && f10003 != "")
                        {
                            //IncarcaGrid(ziua);

                            DateTime ziTmp = Convert.ToDateTime(txtAnLuna.Value);

                            DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_CC"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziTmp.Year, ziTmp.Month, Convert.ToInt32(ziua))}", null);
                            Session["PtjCC"] = dt;
                            grCC.KeyFieldName = "F10003;Ziua;F06204";
                            dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };

                            //grCC.KeyFieldName = "IdAuto";
                            //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                            grCC.DataSource = dt;
                            grCC.DataBind();
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

        protected void btnRespins_Click(object sender, EventArgs e)
        {
            try
            {
                Actiuni(0);
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
                Session["PrintParametrii"] = cmbAng.Value + ";" + Convert.ToDateTime(txtAnLuna.Value).Year + ";" + Convert.ToDateTime(txtAnLuna.Value).Month;
                Response.Redirect("~/Reports/Imprima.aspx", true);
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
                //DataTable dt = PontajCuInOut(Convert.ToInt32(cmbAng.Value ?? -99), dtData, Convert.ToInt32(Session["UserId"] ?? -99), idRol, Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), tip);
                DataTable dt = PontajCuInOut();
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Cheia"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta"] = dt;
                grDate.DataBind();

                if (dt.Rows.Count > 0)
                {
                    txtStare.InnerText = General.Nz(dt.Rows[0]["NumeStare"],"Initiat").ToString();
                    string cul = General.Nz(dt.Rows[0]["CuloareStare"], "#FFFFFF").ToString();
                    if (cul.Length == 9) cul = "#" + cul.Substring(3);
                    txtStare.Style.Add("background", cul);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        //public DataTable PontajCuInOut(int F10003, DateTime ziua, int idUser, int idRol, int subComp, int filiala, int sectie, int dept, int idSubdept, int idBirou, int tip = 1)
        public DataTable PontajCuInOut()
        {
            //tip  =  1    pontaj pe angajat
            //tip  =  2    pontaj pe zi

            DataTable dt = new DataTable();

            try
            {
                string filtru = "";
                string cheia = "";
                int tipInreg = 1;

                DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                int idRol = Convert.ToInt32(cmbRolAng.Value);

                if (General.Nz(Request.QueryString["Tip"], "").ToString() == "1" || General.Nz(Request.QueryString["Tip"], "").ToString() == "10")
                {
                    filtru = $@" AND {General.ToDataUniv(ziua.Year, ziua.Month)} <= {General.TruncateDateAsString("P.\"Ziua\"")} AND {General.TruncateDateAsString("P.\"Ziua\"")} <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)} AND A.F10003=" + Convert.ToInt32(cmbAng.Value ?? -99);
                    cheia = General.FunctiiData("P.\"Ziua\"", "Z");

                    //2018.02.09 Imbunatatire
                    if (General.Nz(Request.QueryString["Ziua"], "").ToString() != "")
                        filtru += @" AND P.""Ziua""=" + General.ToDataUniv(ziua.Year, ziua.Month, Convert.ToInt32(Request.QueryString["Ziua"].Replace("Ziua", "")));

                    tipInreg = Convert.ToInt32(General.Nz(cmbPtjAng.Value,1));
                }
                else
                {
                    ziua = Convert.ToDateTime(txtZiua.Value);
                    idRol = Convert.ToInt32(cmbRolZi.Value);

                    cheia = "P.F10003";
                    filtru = $@" AND {General.TruncateDateAsString("P.\"Ziua\"")} = {General.ToDataUniv(ziua.Year, ziua.Month, ziua.Day)}";

                    if (General.Nz(cmbSub.Value,"").ToString() != "") filtru += " AND P.F10004=" + cmbSub.Value;
                    if (General.Nz(cmbFil.Value, "").ToString() != "") filtru += " AND P.F10005=" + cmbFil.Value;
                    if (General.Nz(cmbSec.Value, "").ToString() != "") filtru += " AND P.F10006=" + cmbSec.Value;
                    if (General.Nz(cmbDept.Value, "").ToString() != "") filtru += " AND P.F10007=" + cmbDept.Value;
                    if (General.Nz(cmbSubDept.Value, "").ToString() != "") filtru += " AND C.F100958 = " + cmbSubDept.Value;
                    if (General.Nz(cmbBirou.Value, "").ToString() != "") filtru += " AND C.F100959 = " + cmbBirou.Value;

                    if (General.Nz(cmbCtr.Value, "").ToString() != "") filtru += " AND P.\"IdContract\"=" + cmbCtr.Value;
                    if (General.Nz(cmbStare.Value, "").ToString() != "") filtru += " AND J.\"IdStare\"=" + cmbStare.Value;
                    if (General.Nz(cmbAngZi.Value, "").ToString() != "")
                        filtru += " AND P.F10004=" + cmbAngZi.Value;
                    else
                        filtru += General.GetF10003Roluri(Convert.ToInt32(Session["UserId"]), ziua.Year, ziua.Month, 0, -99, idRol, ziua.Day);

                    tipInreg = Convert.ToInt32(General.Nz(cmbPtjZi.Value, 1));
                }


                //Florin 2018.08.27
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
                }




                string valTmp = "";
                string[] arrVal = Constante.lstValuri.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < arrVal.Length - 1; i++)
                {
                    valTmp += $@",CONVERT(datetime,DATEADD(minute, P.""{arrVal[i]}"", '')) AS ""ValTmp{arrVal[i].Replace("Val","")}"" ";
                }

                //Schimbam IdAuto in Cheia si modificam valoarea cu ziua cand este pontaj pe zi si cu Marca cand este pontaj pe angajat pentru a putea dezactiva valurile pe care nu avem voie sa pontam

                string op = "+";
                if (Constante.tipBD == 2) op = "||";


                //Florin 2018-07-25 am adugat filtrul CONVERT(date,P.""Ziua"") <= A.F10023

                string strSql = $@"SELECT P.*, {General.FunctiiData("P.\"Ziua\"", "Z")} AS ""Zi"", P.""Ziua"", A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"" {valTmp} ,
                            {cheia} AS ""Cheia"", 
                            E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Dept"",
                            L.""Denumire"" AS ""DescContract"", M.""Denumire"" AS DescProgram, COALESCE(L.""OreSup"",1) AS ""OreSup"", COALESCE(L.""Afisare"",1) AS ""Afisare"",
                            CASE WHEN {General.TruncateDateAsString("A.F10022")} <= {General.TruncateDateAsString("P.\"Ziua\"")} AND {General.TruncateDateAsString("P.\"Ziua\"")} <= {General.TruncateDateAsString("A.F10023")} THEN 1 ELSE 0 END AS ""Activ"",  
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
                            WHERE A.""OreInVal"" IS NOT NULL AND RTRIM(LTRIM(A.""OreInVal"")) <> '' AND A.""InPontaj""=1 AND B.""IdContract""=P.""IdContract"" AND C.""IdRol""={idRol} AND 
                            (((CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0)) OR
                            ((CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0)) OR
                            ((CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0)) OR
                            P.""ZiLibera"" = COALESCE(B.SL,0)) 
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
                            WHERE B.""IdUser"" = {Session["UserId"]} AND A.""IdForm"" = 'pontaj.pontajone' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
                            UNION
                            SELECT REPLACE(A.""IdColoana"", 'Tmp', '')
                            FROM ""Securitate"" A
                            WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'pontaj.pontajone' AND SUBSTRING(A.""IdColoana"", 1, 6) = 'ValTmp' AND COALESCE(A.""Blocat"",0)=1
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
                            UNION
                            select a.""Id"", b.""IdContract"", c.""IdRol"", a.""Id"" as ""IdAbsenta"" , b.ZL as ""ZileSapt"", b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"", 1 AS ""Tip""
                            from ""Ptj_tblAbsente"" a
                            inner join ""Ptj_ContracteAbsente"" b on a.""Id"" = b.""IdAbsenta""
                            inner join ""Ptj_relRolAbsenta"" c on a.""Id"" = c.""IdAbsenta""
                            WHERE A.""OreInVal"" IS NOT NULL AND A.""InPontaj"" = 1
                            group by b.""IdContract"", c.""IdRol"", a.""Id"", b.ZL, b.S, b.D, b.SL, a.""Denumire"", a.""DenumireScurta"", c.""IdAbsentePermise"", A.""OreInVal"") x
                            WHERE COALESCE(X.DenumireScurta,'') <> '' AND X.""IdContract"" = P.""IdContract"" and X.""IdRol"" = {idRol} AND
                            (COALESCE(X.""ZileSapt"",0)=(CASE WHEN P.""ZiSapt""<6 AND P.""ZiLibera""=0 THEN 1 ELSE 0 END) 
                            AND COALESCE(X.S,0) = (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) 
                            AND COALESCE(X.D,0) = (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END)
							AND COALESCE(X.SL,0) = (CASE WHEN P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 1 THEN 1 ELSE 0 END))
                            GROUP BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            ORDER BY X.""Id"", X.""DenumireScurta"", X.""Denumire""
                            FOR XML PATH ('')) AS ""ValAbsente"",


                            CASE WHEN {idRol} = 3 THEN 1 ELSE 
                            CASE WHEN ({idRol} = 2 AND ((COALESCE(J.""IdStare"",1)=1 OR COALESCE(J.""IdStare"",1) = 2 OR COALESCE(J.""IdStare"",1) = 4 OR COALESCE(J.""IdStare"",1) = 6))) THEN 1 ELSE 
                            CASE WHEN ({idRol} = 1 AND(COALESCE(J.""IdStare"", 1) = 1 OR COALESCE(J.""IdStare"", 1) = 4)) THEN 1 ELSE 0
                            END END END AS ""DrepturiModif""
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
                            WHERE CONVERT(date,P.""Ziua"") <= A.F10023
                            {filtru}
                            ORDER BY A.F10003, {General.TruncateDateAsString("P.\"Ziua\"")}";


                //LEFT JOIN ""Ptj_Cereri"" M ON A.F10003=M.F10003 AND M.""DataInceput"" <= P.""Ziua"" AND P.""Ziua"" <= M.""DataSfarsit"" AND M.""IdAbsenta"" IN (SELECT ""Id"" FROM ""Ptj_tblAbsente"" WHERE ""IdTipOre""=1)

                //(select ',' + '""' + CONVERT(nvarchar(10),COALESCE(X.Id,'')) + ' = ' + X.DenumireScurta + '""' from ( 

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

                //string strSql = $@"SELECT X.""IdRol"", X.""RolDenumire"" FROM ({SelectComun()}) X 
                //                WHERE X.F10022 <= {General.ToDataUniv(dtData.Year, dtData.Month, 99)} AND {General.ToDataUniv(dtData.Year, dtData.Month)} <= X.F10023
                //                GROUP BY X.""IdRol"", X.""RolDenumire""
                //                ORDER BY X.""RolDenumire"" ";


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
                                 group by a.""Id"", a.""Denumire"", a.""PoateInitializa"",a.""PoateSterge"", a.""TipMesaj"" ";

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
                //   SqlDataSource1.SelectCommand = @"SELECT OID, [From], Sent, Subject 
                //FROM (select OID, [From], Sent, Subject, row_number() over(order by t.Sent) as [rn] 
                //from dbo.ServerSideGridTest as t WHERE ((t.Subject LIKE @filter) OR (t.[From] 
                // LIKE @filter))) as st where st.[rn] between @startIndex and @endIndex";
                //   SqlDataSource1.SelectParameters.Clear();
                //   SqlDataSource1.SelectParameters.Add("filter", TypeCode.String, string.Format("%{0}%", e.Filter));
                //   SqlDataSource1.SelectParameters.Add("startIndex", TypeCode.Int64, (e.BeginIndex + 1).ToString());
                //   SqlDataSource1.SelectParameters.Add("endIndex", TypeCode.Int64, (e.EndIndex + 1).ToString());
                //   cmb.DataSource = SqlDataSource1;
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

                //SqlDataSource1.SelectCommand = @"SELECT ID, LastName, [Phone], FirstName FROM Persons WHERE (ID = @ID) ORDER BY FirstName";

                //SqlDataSource1.SelectParameters.Clear();
                //SqlDataSource1.SelectParameters.Add("ID", TypeCode.Int64, e.Value.ToString());
                //cmb.DataSource = SqlDataSource1;
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
                                WHERE X.""IdRol""={(cmbRolAng.Value ?? -99)} AND X.F10022 <= {General.ToDataUniv(zi.Year, zi.Month, 99)} AND {General.ToDataUniv(zi.Year, zi.Month, 1)} <= X.F10023
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
                                        row["Val0"] =  Convert.ToInt32(General.Nz(row["Norma"],0)) * 60;
                                        //este = true;
                                    }
                                }
                            }
                            break;
                        case 2:
                            row["ValStr"] = DBNull.Value;
                            row["Val0"] = DBNull.Value;
                            row["Val1"] = DBNull.Value;
                            row["Val2"] = DBNull.Value;
                            row["Val3"] = DBNull.Value;
                            row["Val4"] = DBNull.Value;
                            row["Val5"] = DBNull.Value;
                            row["Val6"] = DBNull.Value;
                            row["Val7"] = DBNull.Value;
                            row["Val8"] = DBNull.Value;
                            row["Val9"] = DBNull.Value;
                            row["Val10"] = DBNull.Value;
                            row["Val11"] = DBNull.Value;
                            row["Val12"] = DBNull.Value;
                            row["Val13"] = DBNull.Value;
                            row["Val14"] = DBNull.Value;
                            row["Val15"] = DBNull.Value;
                            row["Val16"] = DBNull.Value;
                            row["Val17"] = DBNull.Value;
                            row["Val18"] = DBNull.Value;
                            row["Val19"] = DBNull.Value;
                            row["Val20"] = DBNull.Value;
                            ////este = true;
                            break;
                    }
                }

                General.SalveazaDate(dt, "Ptj_Intrari");

                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
                grDate.DataBind();
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
                                        if (General.CuloarePontaj((int)obj) != null) e.Cell.BackColor = (System.Drawing.Color)General.CuloarePontaj((int)obj);
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


        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                grDate.CancelEdit();

                string ids = "";

                List<metaAbsTipZi> lst = new List<metaAbsTipZi>();

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataTable dtVal = Session["Ptj_IstoricVal"] as DataTable;

                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                    object[] keys = new object[] { upd.Keys[0] };

                    DataRow row = dt.Rows.Find(keys);
                    if (row == null) continue;

                    if (upd.OldValues["ValStr"] != upd.NewValues["ValStr"])
                    {
                        if (tip == 1 || tip == 10)
                            ids = (cmbAng.Value ?? -99) + ";";
                        else
                            ids += row["F10003"] + ";";
                    }

                    row["ValStr"] = upd.NewValues["ValStr"] ?? DBNull.Value;
                    row["USER_NO"] = Session["UserId"];
                    row["TIME"] = DateTime.Now;

                    if (upd.NewValues["Observatii"] != null) row["Observatii"] = upd.NewValues["Observatii"];
                    if (upd.NewValues["Observatii2"] != null) row["Observatii2"] = upd.NewValues["Observatii2"];
                    if (upd.NewValues["F06204Default"] != null) row["F06204Default"] = upd.NewValues["F06204Default"];

                    DateTime zi = Convert.ToDateTime(row["Ziua"]);
                    for (int i = 0; i <= 60; i++)
                    {
                        //salvam In Out -urile
                        if (i >= 1 && i <= 20)
                        {
                            if (upd.NewValues["In" + i] != null)
                            {
                                DateTime inOut = Convert.ToDateTime(upd.NewValues["In" + i]);
                                //Florin 2018.08.29
                                row["In" + i] = new DateTime(zi.Year, zi.Month, zi.Day, inOut.Hour, inOut.Minute, inOut.Second);
                                try
                                {
                                    if (row["Out" + (i - 1)] != null && Convert.ToDateTime(row["Out" + (i - 1)]) > Convert.ToDateTime(row["In" + i]))
                                        row["In" + i] = Convert.ToDateTime(row["In" + i]).AddDays(1);
                                }
                                catch (Exception){}
                            }
                            else
                            {
                                row["In" + i] = DBNull.Value;
                            }


                            if (upd.NewValues["Out" + i] != null)
                            {
                                DateTime inOut = Convert.ToDateTime(upd.NewValues["Out" + i]);
                                //Florin 2018.08.29
                                row["Out" + i] = new DateTime(zi.Year, zi.Month, zi.Day, inOut.Hour, inOut.Minute, inOut.Second);
                                try
                                {
                                    if (row["In" + i] != null && Convert.ToDateTime(row["In" + i]) > Convert.ToDateTime(row["Out" + i]))
                                        row["Out" + i] = Convert.ToDateTime(row["Out" + i]).AddDays(1);
                                }
                                catch (Exception){}
                            }
                            else
                            {
                                row["Out" + i] = DBNull.Value;
                            }
                        }

                        //salvam Val-urile
                        if (i >= 0 && i <= 20)
                        {
                            if (upd.NewValues["ValTmp" + i] != null)
                                row["Val" + i] = Convert.ToDateTime(upd.NewValues["ValTmp" + i]).Minute + (Convert.ToDateTime(upd.NewValues["ValTmp" + i]).Hour * 60);
                            else
                                row["Val" + i] = DBNull.Value;

                            //salvam ValModif -urile
                            if (Convert.ToDateTime(General.Nz(upd.NewValues["ValTmp" + i], DateTime.Now)) != Convert.ToDateTime(General.Nz(upd.OldValues["ValTmp" + i], DateTime.Now))) row["ValModif" + i] = Constante.TipModificarePontaj.ModificatManual;

                            //Radu 31.10.2017
                            //Florin 2018.09.03 rescris codul de mai jos

                            //var ert = upd.NewValues["ValTmp" + i];
                            //var ert1 = upd.OldValues["ValTmp" + i];
                            //if (ert != ert1 && ((ert != null && ert.ToString().Length > 0) || (ert1 != null && ert1.ToString().Length > 0)))
                            //    row["CuloareValoare"] = Constante.CuloareModificatManual;
                            try
                            {
                                var tmpNew = Convert.ToDateTime(General.Nz(upd.NewValues["ValTmp" + i], new DateTime(1900,1,1)));
                                var tmpOld = Convert.ToDateTime(General.Nz(upd.OldValues["ValTmp" + i], new DateTime(1900, 1, 1)));
                                if (tmpNew != tmpOld)
                                    row["CuloareValoare"] = Constante.CuloareModificatManual;
                            }
                            catch (Exception){}
                        }

                        //salvam F-urile
                        if (i > 0) row["F" + i] = upd.NewValues["F" + i] ?? DBNull.Value;
                    }


                    //adaugam istoricul modificarilor de val-uri
                    DataRow drVal = dtVal.NewRow();
                    drVal["F10003"] = row["F10003"];
                    drVal["Ziua"] = row["Ziua"];
                    drVal["ValStr"] = upd.NewValues["ValStr"];
                    drVal["ValStrOld"] = upd.OldValues["ValStr"];
                    drVal["IdUser"] = Session["UserId"];
                    drVal["DataModif"] = DateTime.Now;
                    drVal["USER_NO"] = Session["UserId"];
                    drVal["TIME"] = DateTime.Now;
                    drVal["Observatii"] = "Pontajul Meu";
                    dtVal.Rows.Add(drVal);

                    if (General.Nz(upd.NewValues["ValAbs"], "").ToString() != "")
                    {
                        lst.Add(new metaAbsTipZi { F10003 = Convert.ToInt32(row["F10003"]), Ziua = Convert.ToDateTime(row["Ziua"]) });
                    }
                }

                if (dt.GetChanges() != null && ((DataTable)dt.GetChanges()).Rows.Count > 0)
                {
                    DataTable dtModif = ((DataTable)dt.GetChanges());

                    General.SalveazaDate(dt, "Ptj_Intrari");
                    General.SalveazaDate(dtVal, "Ptj_IstoricVal");

                    for (int i = 0; i < dtModif.Rows.Count; i++)
                    {
                        if (dtModif.Rows[i]["CuloareValoare"].ToString() != "#e6c8fa")
                        {
                            //Florin 2018.05.15
                            //daca este absenta de tip zi nu mai recalculam
                            if (lst.Where(p => p.F10003 == Convert.ToInt32(dtModif.Rows[i]["F10003"]) && p.Ziua == Convert.ToDateTime(dtModif.Rows[i]["Ziua"])).Count() == 0)
                            {
                                string golesteVal = Dami.ValoareParam("GolesteVal");
                                FunctiiCeasuri.Calcul.cnApp = Module.Constante.cnnWeb;
                                FunctiiCeasuri.Calcul.tipBD = Constante.tipBD;
                                FunctiiCeasuri.Calcul.golesteVal = golesteVal;
                                FunctiiCeasuri.Calcul.AlocaContract(Convert.ToInt32(dtModif.Rows[i]["F10003"].ToString()), FunctiiCeasuri.Calcul.nzData(dtModif.Rows[i]["Ziua"]));
                                FunctiiCeasuri.Calcul.CalculInOut(dtModif.Rows[i], true, true);
                            }
                        }
                    }

                    for (int i = 0; i < dtModif.Rows.Count; i++)
                    {
                        if (dtModif.Rows[i]["CuloareValoare"].ToString() != Constante.CuloareModificatManual)
                        {
                            for (int j = 0; j < dt.Rows.Count; j++)
                                if (dt.Rows[j]["F10003"].ToString() == dtModif.Rows[i]["F10003"].ToString() && dt.Rows[j]["Ziua"].ToString() == dtModif.Rows[i]["Ziua"].ToString())
                                {
                                    dt.Rows[j]["ValStr"] = dtModif.Rows[i]["ValStr"];
                                    break;
                                }
                            for (int k = 0; k < dt.Rows.Count; k++)
                                if (dtVal.Rows[k]["F10003"].ToString() == dtModif.Rows[i]["F10003"].ToString() && dtVal.Rows[k]["Ziua"].ToString() == dtModif.Rows[i]["Ziua"].ToString())
                                {
                                    dtVal.Rows[k]["ValStr"] = dtModif.Rows[i]["ValStr"];
                                    break;
                                }
                        }
                    }

                    //Session["InformatiaCurenta"] = dt;
                    //grDate.DataSource = dt;
                    //grDate.DataBind();

                    ExecCalcul(ids);

                    IncarcaGrid();

                    MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
                }
                else
                    MessageBox.Show("Nu exista modificari", MessageBox.icoInfo);

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        //{
        //    try
        //    {
        //        grDate.CancelEdit();

        //        string ids = "";

        //        List<metaAbsTipZi> lst = new List<metaAbsTipZi>();

        //        DataTable dt = Session["InformatiaCurenta"] as DataTable;
        //        DataTable dtVal = Session["Ptj_IstoricVal"] as DataTable;

        //        for (int x = 0; x < e.UpdateValues.Count; x++)
        //        {
        //            ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
        //            object[] keys = new object[] { upd.Keys[0] };

        //            DataRow row = dt.Rows.Find(keys);
        //            if (row == null) continue;

        //            if (upd.OldValues["ValStr"] != upd.NewValues["ValStr"])
        //            {
        //                if (tip == 1 || tip == 10)
        //                    ids = (cmbAng.Value ?? -99) + ";";
        //                else
        //                    ids += row["F10003"] + ";";
        //            }

        //            row["ValStr"] = upd.NewValues["ValStr"] ?? DBNull.Value;
        //            row["USER_NO"] = Session["UserId"];
        //            row["TIME"] = DateTime.Now;

        //            DateTime zi = Convert.ToDateTime(row["Ziua"]);
        //            for (int i = 0; i <= 60; i++)
        //            {

        //                //salvam In Out -urile
        //                if (i >= 1 && i <= 20)
        //                {
        //                    if (upd.NewValues["In" + i] != null)
        //                    {
        //                        DateTime inOut = Convert.ToDateTime(upd.NewValues["In" + i]);
        //                        if (inOut.Year > 2000)
        //                        {
        //                            row["In" + i] = inOut;
        //                        }
        //                        else
        //                        {
        //                            row["In" + i] = new DateTime(zi.Year, zi.Month, zi.Day, inOut.Hour, inOut.Minute, inOut.Second);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        row["In" + i] = DBNull.Value;
        //                    }


        //                    if (upd.NewValues["Out" + i] != null)
        //                    {
        //                        DateTime inOut = Convert.ToDateTime(upd.NewValues["Out" + i]);
        //                        if (inOut.Year > 2000)
        //                        {
        //                            row["Out" + i] = inOut;
        //                        }
        //                        else
        //                        {
        //                            row["Out" + i] = new DateTime(zi.Year, zi.Month, zi.Day, inOut.Hour, inOut.Minute, inOut.Second);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        row["Out" + i] = DBNull.Value;
        //                    }
        //                }

        //                //salvam Val-urile
        //                if (i >= 0 && i <= 20)
        //                {
        //                    var ert = upd.NewValues["ValTmp" + i];
        //                    var ert1 = upd.OldValues["ValTmp" + i];

        //                    if (upd.NewValues["ValTmp" + i] != null)
        //                        row["Val" + i] = Convert.ToDateTime(upd.NewValues["ValTmp" + i]).Minute + (Convert.ToDateTime(upd.NewValues["ValTmp" + i]).Hour * 60);
        //                    else
        //                        row["Val" + i] = DBNull.Value;

        //                    //salvam ValModif -urile
        //                    if (Convert.ToDateTime(General.Nz(upd.NewValues["ValTmp" + i], DateTime.Now)) != Convert.ToDateTime(General.Nz(upd.OldValues["ValTmp" + i], DateTime.Now))) row["ValModif" + i] = Constante.TipModificarePontaj.ModificatManual;

        //                    //Radu 31.10.2017
        //                    if (ert != ert1 && ((ert != null && ert.ToString().Length > 0) || (ert1 != null && ert1.ToString().Length > 0)))
        //                        row["CuloareValoare"] = Constante.CuloareModificatManual;
        //                }

        //                //salvam F-urile
        //                if (i > 0) row["F" + i] = upd.NewValues["F" + i] ?? DBNull.Value;
        //            }


        //            //adaugam istoricul modificarilor de val-uri
        //            DataRow drVal = dtVal.NewRow();
        //            drVal["F10003"] = row["F10003"];
        //            drVal["Ziua"] = row["Ziua"];
        //            drVal["ValStr"] = upd.NewValues["ValStr"];
        //            drVal["IdUser"] = Session["UserId"];
        //            drVal["DataModif"] = DateTime.Now;
        //            drVal["USER_NO"] = Session["UserId"];
        //            drVal["TIME"] = DateTime.Now;
        //            dtVal.Rows.Add(drVal);

        //            if (General.Nz(upd.NewValues["ValAbs"], "").ToString() != "")
        //            {
        //                lst.Add(new metaAbsTipZi { F10003 = Convert.ToInt32(row["F10003"]), Ziua = Convert.ToDateTime(row["Ziua"]) });
        //            }
        //        }

        //        if (dt.GetChanges() != null && ((DataTable)dt.GetChanges()).Rows.Count > 0)
        //        {
        //            DataTable dtModif = ((DataTable)dt.GetChanges());

        //            General.SalveazaDate(dt, "Ptj_Intrari");
        //            General.SalveazaDate(dtVal, "Ptj_IstoricVal");

        //            for (int i = 0; i < dtModif.Rows.Count; i++)
        //            {
        //                if (dtModif.Rows[i]["CuloareValoare"].ToString() != "#e6c8fa")
        //                {
        //                    //Florin 2018.05.15
        //                    //daca este absenta de tip zi nu mai recalculam
        //                    if (lst.Where(p => p.F10003 == Convert.ToInt32(dtModif.Rows[i]["F10003"]) && p.Ziua == Convert.ToDateTime(dtModif.Rows[i]["Ziua"])).Count() == 0)
        //                    {
        //                        string golesteVal = Dami.ValoareParam("GolesteVal");
        //                        FunctiiCeasuri.Calcul.cnApp = Module.Constante.cnnWeb;
        //                        FunctiiCeasuri.Calcul.tipBD = Constante.tipBD;
        //                        FunctiiCeasuri.Calcul.golesteVal = golesteVal;
        //                        FunctiiCeasuri.Calcul.AlocaContract(Convert.ToInt32(dtModif.Rows[i]["F10003"].ToString()), FunctiiCeasuri.Calcul.nzData(dtModif.Rows[i]["Ziua"]));
        //                        FunctiiCeasuri.Calcul.CalculInOut(dtModif.Rows[i], true, true);
        //                    }
        //                }
        //            }

        //            for (int i = 0; i < dtModif.Rows.Count; i++)
        //            {
        //                if (dtModif.Rows[i]["CuloareValoare"].ToString() != Constante.CuloareModificatManual)
        //                {
        //                    for (int j = 0; j < dt.Rows.Count; j++)
        //                        if (dt.Rows[j]["F10003"].ToString() == dtModif.Rows[i]["F10003"].ToString() && dt.Rows[j]["Ziua"].ToString() == dtModif.Rows[i]["Ziua"].ToString())
        //                        {
        //                            dt.Rows[j]["ValStr"] = dtModif.Rows[i]["ValStr"];
        //                            break;
        //                        }
        //                    for (int k = 0; k < dt.Rows.Count; k++)
        //                        if (dtVal.Rows[k]["F10003"].ToString() == dtModif.Rows[i]["F10003"].ToString() && dtVal.Rows[k]["Ziua"].ToString() == dtModif.Rows[i]["Ziua"].ToString())
        //                        {
        //                            dtVal.Rows[k]["ValStr"] = dtModif.Rows[i]["ValStr"];
        //                            break;
        //                        }
        //                }
        //            }

        //            Session["InformatiaCurenta"] = dt;
        //            grDate.DataSource = dt;
        //            grDate.DataBind();

        //            ExecCalcul(ids);

        //            MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
        //        }
        //        else
        //            MessageBox.Show("Nu exista modificari", MessageBox.icoInfo);

        //        e.Handled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }            
        //}

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

                                string strSql = @"SELECT A.* FROM ""Ptj_Intrari"" A
                                                LEFT JOIN ""F100Contracte"" B ON A.F10003=B.F10003
                                                WHERE @1 <= A.F10003 AND A.F10003 <= @2 AND @3 <= A.""Ziua"" AND A.""Ziua"" <= @4";

                                FunctiiCeasuri.Calcul.cnApp = Module.Constante.cnnWeb;
                                FunctiiCeasuri.Calcul.tipBD = Constante.tipBD;

                                DateTime dtInc = new DateTime(Convert.ToInt32(arr[1].Split('/')[2]), Convert.ToInt32(arr[1].Split('/')[1]), Convert.ToInt32(arr[1].Split('/')[0]));
                                DateTime dtSf = new DateTime(Convert.ToInt32(arr[2].Split('/')[2]), Convert.ToInt32(arr[2].Split('/')[1]), Convert.ToInt32(arr[2].Split('/')[0]));

                                //DataTable dt = General.IncarcaDT(strSql, new object[] { arr[3], arr[4], Convert.ToDateTime(arr[1]), Convert.ToDateTime(arr[2]) });
                                DataTable dt = General.IncarcaDT(strSql, new object[] { arr[3], arr[4], dtInc, dtSf });

                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (dt.Rows[i]["CuloareValoare"].ToString() != Constante.CuloareModificatManual)
                                    {
                                        string golesteVal = Dami.ValoareParam("GolesteVal");
                                        FunctiiCeasuri.Calcul.cnApp = Module.Constante.cnnWeb;
                                        FunctiiCeasuri.Calcul.tipBD = Constante.tipBD;
                                        FunctiiCeasuri.Calcul.golesteVal = golesteVal;
                                        FunctiiCeasuri.Calcul.AlocaContract(Convert.ToInt32(dt.Rows[i]["F10003"].ToString()), FunctiiCeasuri.Calcul.nzData(dt.Rows[i]["Ziua"]));
                                        FunctiiCeasuri.Calcul.CalculInOut(dt.Rows[i], true, true);
                                    }
                                }
                                Session["InformatiaCurenta"] = dt;
                                grDate.DataBind();
                            }
                            break;
                        case "btnInit":
                            MetodeInitializare(1);
                            break;
                        case "btnDelete":
                            MetodeInitializare(2);
                            break;
                        case "dayPlus":
                            {
                                if (arr.Length > 1 && arr[1] != null && arr[1] != "")
                                {
                                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Cheia", "Ziua", arr[1] }) as object[];
                                    DataTable dt = Session["InformatiaCurenta"] as DataTable;

                                    DateTime zi = Convert.ToDateTime(obj[1]);
                                    DateTime inOut = Convert.ToDateTime(obj[2]);

                                    if (zi.Date == inOut.Date || zi.Date == inOut.AddDays(1).Date)
                                    {
                                        DataRow dr = dt.Rows.Find(obj[0]);
                                        dr[arr[1]] = Convert.ToDateTime(dr[arr[1]]).AddDays(1);
                                        Session["InformatiaCurenta"] = dt;
                                        grDate.DataBind();
                                    }
                                }
                            }
                            break;
                        case "dayMinus":
                            {
                                if (arr.Length > 1 && arr[1] != null && arr[1] != "")
                                {
                                    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Cheia", "Ziua", arr[1] }) as object[];
                                    DataTable dt = Session["InformatiaCurenta"] as DataTable;

                                    DateTime zi = Convert.ToDateTime(obj[1]);
                                    DateTime inOut = Convert.ToDateTime(obj[2]);

                                    if (zi.Date == inOut.Date || zi.Date == inOut.AddDays(-1).Date)
                                    {
                                        DataRow dr = dt.Rows.Find(obj[0]);
                                        dr[arr[1]] = Convert.ToDateTime(dr[arr[1]]).AddDays(-1);
                                        Session["InformatiaCurenta"] = dt;
                                        grDate.DataBind();
                                    }
                                }
                            }
                            break;
                        case "cellPlus":
                            if (arr.Length > 1 && arr[1] != null && arr[1] != "")
                            {
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Cheia", "Ziua", arr[1] }) as object[];
                                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                                DataRow dr = dt.Rows.Find(obj[0]);
                                
                                string[] arrIO = new string[] { "In1", "Out1", "In2", "Out2", "In3", "Out3", "In4", "Out4", "In5", "Out5", "In6", "Out6", "In7", "Out7", "In8", "Out8", "In9", "Out9", "In10", "Out10", "In11", "Out11", "In12", "Out12", "In13", "Out13", "In14", "Out14", "In15", "Out15", "In16", "Out16", "In17", "Out17", "In18", "Out18", "In19", "Out19", "In20", "Out20" };
                                
                                int idx = arrIO.ToList().FindIndex(pnlCtl => pnlCtl == arr[1]);

                                for (int i = arrIO.Length - 1; i > idx; i--)
                                {
                                    dr[arrIO[i]] = dr[arrIO[i - 1]];
                                }
                                //stergem pozitia curenta pt ca am mutat-o pe urmataorea poztie
                                dr[arr[1]] = DBNull.Value;

                                Session["InformatiaCurenta"] = dt;
                                grDate.DataBind();
                            }
                            break;
                        case "cellMinus":
                            if (arr.Length > 1 && arr[1] != null && arr[1] != "")
                            {
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Cheia", "Ziua", arr[1] }) as object[];
                                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                                DataRow dr = dt.Rows.Find(obj[0]);

                                string[] arrIO = new string[] { "In1", "Out1", "In2", "Out2", "In3", "Out3", "In4", "Out4", "In5", "Out5", "In6", "Out6", "In7", "Out7", "In8", "Out8", "In9", "Out9", "In10", "Out10", "In11", "Out11", "In12", "Out12", "In13", "Out13", "In14", "Out14", "In15", "Out15", "In16", "Out16", "In17", "Out17", "In18", "Out18", "In19", "Out19", "In20", "Out20" };
                                
                                int idx = arrIO.ToList().FindIndex(pnlCtl => pnlCtl == arr[1]);

                                //daca celula pe care sunt are valoare, nu fac nimic, celula trebuie sa fie goala ca sa pot sa mut tot ce este la stanga la dreapta
                                if (dr[arr[1]] != DBNull.Value) return;

                                for (int i = idx; i < arrIO.Length - 1; i++)
                                {
                                    dr[arrIO[i]] = dr[arrIO[i + 1]];
                                }
                                //stergem ultima pozitie pt ca am mutat valoarea pe campul precedent
                                dr[arrIO[arrIO.Length-1]] = DBNull.Value;

                                Session["InformatiaCurenta"] = dt;
                                grDate.DataBind();
                            }
                            break;
                        case "PtAbs":
                            {
                                //object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Cheia", "Ziua" }) as object[];

                                //int idAbs = 2;
                                //int ziua = Convert.ToInt32(obj[0]);
                                //if (ziua == 1) idAbs = 5;
                                //if (ziua == 2) idAbs = 6;
                                //if (ziua == 3) idAbs = 16;


                                ////DataTable dt = General.GetAbsentePeContract(Convert.ToInt32(obj[0]));
                                //DataTable dt = General.GetAbsentePeContract(idAbs);
                                //GridViewDataComboBoxColumn cmb = grDate.Columns["ValAbs"] as GridViewDataComboBoxColumn;

                                //cmb.PropertiesComboBox.DataSource = dt;
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

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (e.Column.FieldName.Length >= 6 && e.Column.FieldName.Substring(0, 6) == "ValTmp")
                {
                    e.Editor.ReadOnly = false;
                }

                //if (e.Column.FieldName == "In1")
                //{
                //    object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Ziua", "F10003" }) as object[];
                //    var ert = e.KeyValue;
                //    var edc = obj[0];
                //    (e.Editor as ASPxDateEdit).Date = Convert.ToDateTime(obj[0]);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        void combo_Callback(object sender, CallbackEventArgsBase e)
        {
            
            ASPxComboBox combo = sender as ASPxComboBox;
            var ert = e.Parameter;

            //for (int i = 0; i < 10; i++)
            //{
            //    combo.Items.Add(string.Format("Row_{0} Item_{1}", e.Parameter, i));
            //}
        }

        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {
                var clientData = new Dictionary<int, object>();
                var lstDrepturi = new Dictionary<int, int>();
                var lstValAbsente = new Dictionary<int, object>();
                var lstValSec = new Dictionary<int, object>();

                var grid = sender as ASPxGridView;
                for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
                {
                    var rowValues = grid.GetRowValues(i, new string[] { "Cheia", "ValActive", "DrepturiModif", "ValAbsente", "ValSecuritate" }) as object[];
                    clientData.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
                    lstDrepturi.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), Convert.ToInt32(rowValues[2] ?? 0));
                    lstValAbsente.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[3] ?? "");
                    lstValSec.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[4] ?? "");
                }

                grid.JSProperties["cp_cellsToDisable"] = clientData;
                grid.JSProperties["cp_cellsDrepturi"] = lstDrepturi;
                grid.JSProperties["cp_ValAbsente"] = lstValAbsente;
                grid.JSProperties["cp_ValSec"] = lstValSec;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


#endregion 


        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                Actiuni(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Actiuni(int actiune)
        {
            try
            {
                int idRol = Convert.ToInt32(cmbRolAng.Value ?? 1);
                if (tip == 2) idRol = Convert.ToInt32(cmbRolZi.Value ?? 1);

                if (!General.DrepturiAprobare(actiune, idRol))
                    MessageBox.Show(Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie."), MessageBox.icoError, "Eroare !");
                else
                {
                    int f10003 = Convert.ToInt32(cmbAng.Value ?? -99);
                    int idStare = 1;
                    DataRowView entCu = grDate.GetRow(0) as DataRowView;
                    if (entCu != null) idStare = Convert.ToInt32(entCu["IdStare"] ?? 1);

                    string mesaj = General.ActiuniExec(actiune, f10003, idRol, idStare, Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, "Pontaj.PontajDetaliat", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["User_Marca"]));
                    if (actiune == 1)
                        MessageBox.Show(Dami.TraduCuvant(mesaj), MessageBox.icoInfo, Dami.TraduCuvant("Aprobare") + " !");
                    else
                        MessageBox.Show(Dami.TraduCuvant(mesaj), MessageBox.icoInfo, Dami.TraduCuvant("Respingere") + " !");
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
                    case "cmbRolAng":
                    case "cmbRolZi":
                    case "txtAnLuna":
                        IncarcaAngajati();
                        esteStruc = false;
                        break;
                }

                if (esteStruc)
                {
                    cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                    cmbSec.DataBind();
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                    cmbDept.DataBind();
                    cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                    cmbSubDept.DataBind();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void CreazaGrid()
        {
            try
            {
                string cmp = "SUBSTRING";
                if (Constante.tipBD == 2)
                    cmp = "SUBSTR";
                DataTable dtCol = General.IncarcaDT($@"SELECT A.*, 
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""DenumireScurta"",'') <> '' THEN REPLACE(B.""DenumireScurta"",' ','') ELSE A.""Coloana"" END AS ""ColDen"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""DenumireScurta"",'') <> '' THEN B.""DenumireScurta"" ELSE A.""Alias"" END AS ""ColAlias"",
                                CASE WHEN {cmp}(A.""Coloana"",1,3)='Val' AND COALESCE(B.""Denumire"",'') <> '' THEN B.""Denumire"" ELSE (CASE WHEN COALESCE(A.""AliasToolTip"",'') <> '' THEN A.""AliasToolTip"" ELSE A.""Coloana"" END) END AS ""ColTT"",
                                COALESCE(B.""DenumireScurta"",'') AS ""ColScurta""
                                FROM ""Ptj_tblAdmin"" A
                                LEFT JOIN ""Ptj_tblAbsente"" B ON A.""Coloana""=B.""OreInVal""
                                ORDER BY A.""Ordine"" ", null);

                if (dtCol != null)
                {
                    DataSet ds = new DataSet();

                    for (int i = 0; i < dtCol.Rows.Count; i++)
                    {
                        DataRow dr = dtCol.Rows[i];
                        string colField = General.Nz(dr["Coloana"], "col" + i).ToString();
                        string colName = General.Nz(dr["ColDen"], "col" + i).ToString() + "_" + General.Nz(dr["ColScurta"], "").ToString();
                        //string colName = General.Nz(dr["ColDen"], "col" + i).ToString();
                        string alias = General.Nz(dr["ColAlias"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool vizibil = Convert.ToBoolean(General.Nz(dr["Vizibil"], false));
                        bool blocat = Convert.ToBoolean(General.Nz(dr["Blocat"], false));
                        int latime = Convert.ToInt32(General.Nz(dr["Latime"], 80));
                        int tipCol = Convert.ToInt32(General.Nz(dr["TipColoana"],1));
                        string tt = General.Nz(dr["ColTT"], General.Nz(dr["Coloana"], "col" + i).ToString()).ToString();
                        bool unb = false;

                        if (Constante.lstValuri.IndexOf(colField + ";") >= 0)
                        {
                            unb = true;
                            colField = "ValTmp" + colField.Replace("Val","");
                        }

                        switch (tipCol)
                        {
                            case 0:                             //General
                                {
                                    GridViewDataColumn c = new GridViewDataColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    //Settings-AutoFilterCondition="Contains"
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 1:                             //CheckBox
                                {
                                    GridViewDataCheckColumn c = new GridViewDataCheckColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 2:                             //ComboBox
                                {
                                    
                                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (colName.Length >= 6 && colName.Substring(0, 6) == "ValAbs")
                                    {
                                        c.UnboundType = DevExpress.Data.UnboundColumnType.String;
                                    }

                                    c.PropertiesComboBox.AllowNull = true;

                                    if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                    {
                                        string sursa = (dr["SursaCombo"] ?? "").ToString().Trim();
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

                                        for (int f = 0; f < dtCmb.Columns.Count; f++)
                                        {
                                            ListBoxColumn lstCol = new ListBoxColumn();
                                            lstCol.FieldName = dtCmb.Columns[f].ColumnName;
                                            lstCol.Caption = dtCmb.Columns[f].ColumnName;
                                            if (f == 0)
                                                lstCol.Width = 60;
                                            else
                                                lstCol.Width = 250;
                                            c.PropertiesComboBox.Columns.Add(lstCol);
                                        }

                                    }

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 3:                             //Date
                                {
                                    GridViewDataDateColumn c = new GridViewDataDateColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 4:                             //Memo
                                {
                                    GridViewDataMemoColumn c = new GridViewDataMemoColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 5:                             //Color
                                {
                                    GridViewDataColorEditColumn c = new GridViewDataColorEditColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 6:                             //Text
                                {
                                    GridViewDataTextColumn c = new GridViewDataTextColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    c.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.String;

                                    //if (col.MaxLength != -1) c.PropertiesTextEdit.MaxLength = col.MaxLength;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 7:                             //Numeric
                                {
                                    GridViewDataSpinEditColumn c = new GridViewDataSpinEditColumn();
                                    //c.Name = colName;
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 8:                             //Time
                                {
                                    GridViewDataTimeEditColumn c = new GridViewDataTimeEditColumn();
                                    c.Name = colName;
                                    c.FieldName = colField;
                                    c.Caption = Dami.TraduCuvant(alias);
                                    c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(latime);
                                    c.VisibleIndex = i + 4;
                                    c.ToolTip = tt;
                                    //c.PropertiesTimeEdit.ClientSideEvents.KeyDown = "function(s, e) { TestGigi(s,e) }";

                                    if (unb) c.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;

                                    c.PropertiesTimeEdit.DisplayFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.DisplayFormatInEditMode = true;
                                    c.PropertiesTimeEdit.EditFormatString = "HH:mm";
                                    c.PropertiesTimeEdit.EditFormat = EditFormat.DateTime;

                                    if (c.FieldName.Length > 2 && c.FieldName.Substring(0, 3) == "Val" && c.FieldName != "ValStr" && c.FieldName != "ValAbs")
                                        c.BatchEditModifiedCellStyle.BackColor = General.Culoare(Constante.CuloareModificatManual);

                                    grDate.Columns.Add(c);
                                }
                                break;
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

        protected void cmbAng_ButtonClick(object source, ButtonEditClickEventArgs e)
        {
            try
            {
                switch(e.ButtonIndex)
                {
                    case 0:
                        if (cmbAng.SelectedIndex > 0) cmbAng.SelectedIndex -= 1;
                        break;
                    case 1:
                        if (cmbAng.SelectedIndex < cmbAng.Items.Count) cmbAng.SelectedIndex += 1;
                        break;
                }

                btnFiltru_Click(source, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void txtZiua_ButtonClick(object source, ButtonEditClickEventArgs e)
        {
            try
            {
                switch (e.ButtonIndex)
                {
                    case 0:
                        txtZiua.Date = txtZiua.Date.AddDays(-1);
                        break;
                    case 1:
                        txtZiua.Date = txtZiua.Date.AddDays(1);
                        break;
                }

                btnFiltru_Click(source, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void ExecCalcul(string ids)
        {
            try
            {
                DateTime ziua = DateTime.Now;
                //int f10003 = -99;
                string[] arr = ids.Split(new string[] { ";"}, StringSplitOptions.RemoveEmptyEntries);

                if (tip == 1 || tip == 10)
                {
                    ziua = Convert.ToDateTime(txtAnLuna.Value ?? DateTime.Now);
                    //f10003 = Convert.ToInt32(cmbAng.Value ?? -99);
                }
                else
                {
                    ziua = Convert.ToDateTime(txtZiua.Value ?? DateTime.Now);
                    //f10003 = Convert.ToInt32(cmbAngZi.Value ?? -99);
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    General.CalculFormuleCumulat(Convert.ToInt32(arr[i]), ziua.Year, ziua.Month);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void grCC_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        //{
        //    try
        //    {
        //        if (e.NewValues["F06204"] == null) return;

        //        DataTable dt = Session["PtjCC"] as DataTable;
        //        object[] row = new object[dt.Columns.Count];
        //        int x = 0;

        //        List<object> lst = ValoriChei();

        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (!col.AutoIncrement)
        //            {
        //                switch (col.ColumnName.ToLower())
        //                {
        //                    case "f10003":
        //                        row[x] = lst[0];
        //                        break;
        //                    case "ziua":
        //                        row[x] = Convert.ToDateTime(lst[1]);
        //                        break;
        //                    case "f06204":
        //                        row[x] = e.NewValues[col.ColumnName] ?? 999;
        //                        break;
        //                    case "idproiect":
        //                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "iddept":
        //                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "de":
        //                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "la":
        //                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "nrore1":
        //                    case "nrore2":
        //                    case "nrore3":
        //                    case "nrore4":
        //                    case "nrore5":
        //                    case "nrore6":
        //                    case "nrore7":
        //                    case "nrore8":
        //                    case "nrore9":
        //                    case "nrore10":
        //                        row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "idstare":
        //                        if (Dami.ValoareParam("PontajCCcuAprobare","0") == "0")
        //                            row[x] = 3;
        //                        else
        //                            row[x] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //                        break;
        //                    case "user_no":
        //                        row[x] = Session["UserId"];
        //                        break;
        //                    case "time":
        //                        row[x] = DateTime.Now;
        //                        break;
        //                }
        //            }

        //            x++;
        //        }

        //        dt.Rows.Add(row);
        //        e.Cancel = true;
        //        grCC.CancelEdit();
        //        Session["PtjCC"] = dt;
        //        grCC.DataSource = dt;

        //        btnSaveCC_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        //msgError = ex.Message;
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grCC_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        //{
        //    try
        //    {
        //        object[] keys = new object[e.Keys.Count];
        //        for (int i = 0; i < e.Keys.Count; i++)
        //        { keys[i] = e.Keys[i]; }

        //        DataTable dt = Session["PtjCC"] as DataTable;
        //        DataRow row = dt.Rows.Find(keys);

        //        foreach (DictionaryEntry col in e.NewValues)
        //        {
        //            if (cmp.IndexOf(col.Key.ToString().ToUpper() + ",") < 0)
        //            {
        //                    row[col.Key.ToString()] = col.Value ?? DBNull.Value;
        //            }
        //        }

        //        e.Cancel = true;
        //        grCC.CancelEdit();
        //        Session["PtjCC"] = dt;
        //        grCC.DataSource = dt;

        //        btnSaveCC_Click(null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grCC_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        //{
        //    try
        //    {
        //        List<object> lst = ValoriChei();

        //        e.NewValues["F10003"] = lst[0];
        //        e.NewValues["Ziua"] = lst[1];

        //        e.NewValues["De"] = lst[1];
        //        e.NewValues["La"] = lst[1];

        //        if (Dami.ValoareParam("PontajCCcuAprobare", "0") == "0")
        //            e.NewValues["IdStare"] = 3;
        //        else
        //            e.NewValues["IdStare"] = 1;

        //        e.NewValues["TIME"] = DateTime.Now;
        //        e.NewValues["USER_NO"] = Session["UserId"];
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grCC_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        //{
        //    try
        //    {
        //        object[] keys = new object[e.Keys.Count];
        //        for (int i = 0; i < e.Keys.Count; i++)
        //        { keys[i] = e.Keys[i]; }

        //        DataTable dt = Session["PtjCC"] as DataTable;
        //        DataRow found = dt.Rows.Find(keys);
        //        found.Delete();

        //        e.Cancel = true;
        //        grCC.DataSource = dt;

        //        //btnSaveCC_Click(null, null);
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
                if (tip == 1)
                {
                    DateTime dtTmp = Convert.ToDateTime(txtAnLuna.Value);
                    lst[1] = new DateTime(dtTmp.Year, dtTmp.Month, Convert.ToInt32(ccValori["cheia"]), 0, 0, 0);
                }
                else
                {
                    lst[0] = Convert.ToInt32(ccValori["cheia"]);
                }
                

                //var cmp = grDate.GetSelectedFieldValues(new string[] { "Cheia" });

                //if (cmp != null && cmp.Count > 0)
                //{
                //    var ert = cmp[0];

                //    if (tip == 1 || tip == 10)
                //    {
                //        lst[0] = cmbAng.Value;
                //        lst[1] = new DateTime(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, Convert.ToInt32(cmp[0]));
                //    }
                //    else
                //    {
                //        lst[0] = cmp[0];
                //        lst[1] = txtZiua.Value;
                //    }
                //}
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
                            cmp += ", " + dr["Destinatie"] + "=(SELECT SUM(" + dr["Camp"] + ") * 60 FROM \"Ptj_CC\" WHERE F10003=@1 AND Ziua=@2)";
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

                        //recalcul f-uri la nivel de luna
                        DateTime zi = Convert.ToDateTime(lst[1]);
                        General.CalculFormuleCumulat(Convert.ToInt32(lst[0]), zi.Year, zi.Month);
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnCC":
                            {
                                lblZiuaCC.Text = "Centrii de cost - Ziua " + Convert.ToDateTime(lst[1]).Day;
                                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_CC"" WHERE F10003={lst[0]} AND ""Ziua""={General.ToDataUniv(Convert.ToDateTime(lst[1]))}", null);
                                Session["PtjCC"] = dt;
                                //grCC.KeyFieldName = "IdAuto";
                                //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                                grCC.KeyFieldName = "F10003;Ziua;F06204";
                                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };

                                grCC.DataSource = dt;
                                grCC.DataBind();

                                //GridViewDataComboBoxColumn colCC = (grCC.Columns["F06204"] as GridViewDataComboBoxColumn);
                                //if (colCC != null)
                                //{
                                //    DataTable dtCC = General.IncarcaDT("SELECT * FROM F062", null);
                                //    colCC.PropertiesComboBox.DataSource = dtCC;
                                //}
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
                                grCC.KeyFieldName = "F10003;Ziua;F06204";
                                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };
                                //grCC.KeyFieldName = "IdAuto";
                                //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
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
                                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT D.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

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
                                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT E.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={arr[1]} AND C.""IdSubproiect""={arr[2]} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

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

        //protected void grCC_RowValidating(object sender, ASPxDataValidationEventArgs e)
        //{
        //    try
        //    {
        //        string msg = "";
        //        DataTable dt = Session["Ptj_tblAdminCC"] as DataTable;

        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            DataRow dr = dt.Rows[i];
        //            //if (Convert.ToBoolean(dr["Vizibil"]) && Convert.ToBoolean(dr["Obligatoriu"]) && e.NewValues[dr["Camp"]] == null) msg += ", " + Dami.TraduCuvant(dr["Alias"].ToString().Trim());
        //            if (Convert.ToBoolean(General.Nz(dr["Obligatoriu"],false)) && e.NewValues[dr["Camp"]] == null) msg += ", " + Dami.TraduCuvant(dr["Alias"].ToString().Trim());
        //        }

        //        //if (e.NewValues["NrOre"] == null || Convert.ToInt32(e.NewValues["NrOre"]) == 0) msg += ", " + Dami.TraduCuvant("NrOre");

        //        if (msg != "")
        //            e.RowError = Dami.TraduCuvant("Lipsesc date") + ": " + msg.Substring(1);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grCC_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        //{
        //    try
        //    {
        //        switch (e.Column.FieldName)
        //        {
        //            case "IdStare":
        //                e.Editor.ReadOnly = true;
        //                break;
        //            case "F06204":
        //                {
        //                    if (grCC.IsEditing && e.KeyValue != DBNull.Value && e.KeyValue != null)
        //                    {
        //                        ASPxComboBox combo = e.Editor as ASPxComboBox;
        //                        LoadCC(combo);
        //                    }
        //                }
        //                break;
        //            case "IdProiect":
        //                {
        //                    if (grCC.IsEditing && e.KeyValue != DBNull.Value && e.KeyValue != null)
        //                    {
        //                        ASPxComboBox combo = e.Editor as ASPxComboBox;
        //                        LoadPro(combo);
        //                    }
        //                }
        //                break;
        //            case "IdSubproiect":
        //                {
        //                    if (grCC.IsEditing && e.KeyValue != DBNull.Value && e.KeyValue != null)
        //                    {
        //                        object valPro = grCC.GetRowValuesByKeyValue(e.KeyValue, "IdProiect");
        //                        if (valPro == DBNull.Value) return;
        //                        Int32 idPro = (Int32)valPro;

        //                        ASPxComboBox combo = e.Editor as ASPxComboBox;
        //                        LoadSubPro(combo, idPro);

        //                        combo.Callback += new CallbackEventHandlerBase(cmbSubPro_OnCallback);
        //                    }
        //                }
        //                break;
        //            case "IdActivitate":
        //                {
        //                    if (grCC.IsEditing && e.KeyValue != DBNull.Value && e.KeyValue != null)
        //                    {
        //                        object valPro = grCC.GetRowValuesByKeyValue(e.KeyValue, "IdProiect");
        //                        object valSub = grCC.GetRowValuesByKeyValue(e.KeyValue, "IdSubproiect");
        //                        if (valPro == DBNull.Value || valSub == DBNull.Value) return;
        //                        Int32 idPro = (Int32)valPro;
        //                        Int32 idSub = (Int32)valSub;

        //                        ASPxComboBox combo = e.Editor as ASPxComboBox;
        //                        LoadAct(combo, idPro, idSub);

        //                        combo.Callback += new CallbackEventHandlerBase(cmbAct_OnCallback);
        //                    }
        //                }
        //                break;
        //            //case "IdDept":
        //            //    {
        //            //        GridViewDataComboBoxColumn colDpt = (grCC.Columns["IdDept"] as GridViewDataComboBoxColumn);
        //            //        if (colDpt != null)
        //            //        {
        //            //            DataTable dtDpt = General.IncarcaDT(General.SelectDepartamente(), null);
        //            //            colDpt.PropertiesComboBox.DataSource = dtDpt;
        //            //        }
        //            //    }
        //            //    break;
        //            //case "IdProiect":
        //            //    var dert = grCC.Columns["IdProiect"];

        //            //    GridViewDataComboBoxColumn colPro = (grCC.Columns["IdProiect"] as GridViewDataComboBoxColumn);
        //            //    if (colPro != null) colPro.PropertiesComboBox.DataSource = Session["PtjCC_Proiecte"];
        //            //    break;
        //            //case "":
        //            //    GridViewDataComboBoxColumn colCC = (grCC.Columns["F06204"] as GridViewDataComboBoxColumn);
        //            //    if (colCC != null) colCC.PropertiesComboBox.DataSource = Session["PtjCC_CentruCost"];
        //            //    break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void grCC_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grCC.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
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

        //protected void grCC_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        //{
        //    try
        //    {
        //        if (e.ButtonType == ColumnCommandButtonType.Edit && Dami.ValoareParam("PontajCCcuAprobare", "0") == "1")
        //        {
        //            object val = grCC.GetRowValues(e.VisibleIndex, new string[] { "IdStare" });
        //            if (val.ToString() == "3") e.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void cmbPro_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ASPxComboBox cmbSubPro = ((ASPxComboBox)grCC.FindEditRowCellTemplateControl(grCC.Columns["IdSubproiect"] as GridViewDataComboBoxColumn, "cmbSubPro"));
        //        if (cmbSubPro == null) return;
        //        List<object> lst = ValoriChei();
        //        DataTable dtPro = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM Ptj_relAngajatProiect A
        //                                                INNER JOIN tblProiecte B ON A.IdProiect = B.Id
        //                                                INNER JOIN relProSubAct C ON B.Id = C.IdProiect
        //                                                INNER JOIN tblSubProiecte D ON C.IdSubproiect=D.Id
        //                                                WHERE A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
        //                                                SELECT * FROM tblSubProiecte
        //                                                ELSE
        //                                                SELECT D.* FROM Ptj_relAngajatProiect A
        //                                                INNER JOIN tblProiecte B ON A.IdProiect = B.Id
        //                                                INNER JOIN relProSubAct C ON B.Id = C.IdProiect
        //                                                INNER JOIN tblSubProiecte D ON C.IdSubproiect=D.Id
        //                                                WHERE A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

        //        cmbSubPro.DataSource = dtPro;
        //        cmbSubPro.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void cmbSubPro_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string ert = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void cmbAct_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string ert = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void cmbPro_Load(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //List<object> lst = ValoriChei();
        //        //DataTable dtPro = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" WHERE F10003={lst[0]} AND {General.TruncateDateAsString("\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("\"DataSfarsit\"")}) = 0)
        //        //                                        SELECT * FROM ""tblProiecte""
        //        //                                        ELSE
        //        //                                        SELECT B.* FROM ""Ptj_relAngajatProiect"" A 
        //        //                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
        //        //                                        WHERE A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

        //        //ASPxComboBox cmb = sender as ASPxComboBox;
        //        //if (cmb != null)
        //        //{
        //        //    cmb.DataSource = dtPro;
        //        //    //cmb.SelectedItem = 
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        private void LoadPro(ASPxComboBox cmb)
        {
            try
            {
                List<object> lst = ValoriChei();

                DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatProiect"" WHERE F10003={lst[0]} AND {General.TruncateDateAsString("\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("\"DataSfarsit\"")}) = 0)
                                                    SELECT * FROM ""tblProiecte""
                                                    ELSE
                                                    SELECT B.* FROM ""Ptj_relAngajatProiect"" A 
                                                    INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                    WHERE A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

                cmb.DataSource = dt;
                cmb.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void LoadCC(ASPxComboBox cmb)
        {
            try
            {
                List<object> lst = ValoriChei();

                DataTable dt = General.IncarcaDT($@"IF((SELECT COUNT(*) FROM ""Ptj_relAngajatCC"" WHERE F10003={lst[0]} AND {General.TruncateDateAsString("\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("\"DataSfarsit\"")}) = 0)
                                                    SELECT * FROM F062
                                                    ELSE
                                                    SELECT B.* FROM ""Ptj_relAngajatCC"" A 
                                                    INNER JOIN F062 B ON A.F06204 = B.F06204
                                                    WHERE A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

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
                                                        WHERE A.""IdProiect""={idPro} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT D.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);

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
                                                        WHERE A.""IdProiect""={idPro} AND C.""IdSubproiect""={idSub} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ) = 0)
                                                        SELECT * FROM ""tblSubProiecte""
                                                        ELSE
                                                        SELECT DISTINCT E.* FROM ""Ptj_relAngajatProiect"" A
                                                        INNER JOIN ""tblProiecte"" B ON A.""IdProiect"" = B.""Id""
                                                        INNER JOIN ""relProSubAct"" C ON B.""Id"" = C.""IdProiect""
                                                        INNER JOIN ""tblSubProiecte"" D ON C.""IdSubproiect""=D.""Id""
                                                        INNER JOIN ""tblActivitati"" E ON C.""IdActivitate""=E.""Id""
                                                        WHERE A.""IdProiect""={idPro} AND C.""IdSubproiect""={idSub} AND A.F10003 = {lst[0]} AND {General.TruncateDateAsString("A.\"DataInceput\"")} <= {General.ToDataUniv(Convert.ToDateTime(lst[1]))} AND {General.ToDataUniv(Convert.ToDateTime(lst[1]))} <= {General.TruncateDateAsString("A.\"DataSfarsit\"")} ", null);
                
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
                    bool ras = General.PontajInit(Convert.ToInt32(Session["UserId"]), dt.Year, dt.Month, -99, chkNormaZL.Checked, chkCCCu.Checked, Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), chkNormaSD.Checked, chkNormaSL.Checked, false, 0);
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

        protected void grDate_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e)
        {
            e.Properties["cpIsEditing"] = grDate.IsEditing;
        }

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

                    dr["F10003"] = lst[0];
                    dr["Ziua"] = Convert.ToDateTime(lst[1]);
                    dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                    dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                    dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                    dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                    dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                    dr["De"] = upd.NewValues["De"] ?? DBNull.Value;
                    dr["La"] = upd.NewValues["La"] ?? DBNull.Value;
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

                    if (upd.NewValues["F06204"] != null) dr["F06204"] = upd.NewValues["F06204"] ?? 9999;
                    if (upd.NewValues["IdProiect"] != null) dr["IdProiect"] = upd.NewValues["IdProiect"] ?? DBNull.Value;
                    if (upd.NewValues["IdSubproiect"] != null) dr["IdSubproiect"] = upd.NewValues["IdSubproiect"] ?? DBNull.Value;
                    if (upd.NewValues["IdActivitate"] != null) dr["IdActivitate"] = upd.NewValues["IdActivitate"] ?? DBNull.Value;
                    if (upd.NewValues["IdDept"] != null) dr["IdDept"] = upd.NewValues["IdDept"] ?? DBNull.Value;
                    if (upd.NewValues["De"] != null) dr["De"] = upd.NewValues["De"] ?? DBNull.Value;
                    if (upd.NewValues["La"] != null) dr["La"] = upd.NewValues["La"] ?? DBNull.Value;
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
                    btnSaveCC_Click(null, null);


                //btnSaveCC_Click(null, null);
                //General.SalveazaDate(dt, "Ptj_CC");

                e.Handled = true;

                DataTable dtCC = General.IncarcaDT($@"SELECT * FROM ""Ptj_CC"" WHERE F10003={lst[0]} AND ""Ziua""={General.ToDataUniv(Convert.ToDateTime(lst[1]))}", null);
                Session["PtjCC"] = dtCC;
                grCC.KeyFieldName = "F10003;Ziua;F06204";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"], dt.Columns["Ziua"], dt.Columns["F06204"] };
                //grCC.KeyFieldName = "IdAuto";
                //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                grCC.DataSource = dtCC;
                grCC.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}