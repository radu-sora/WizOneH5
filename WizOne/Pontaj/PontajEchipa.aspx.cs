﻿using DevExpress.Web;
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

namespace WizOne.Pontaj
{
    public partial class PontajEchipa : System.Web.UI.Page
    {

        private string arrZL = "";

        public class metaActiuni
        {
            public int F10003 { get; set; }
            public int IdStare { get; set; }
        }

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
                Session["PaginaWeb"] = "Pontaj.PontajEchipa";

                //Radu 31.01.2019 - pentru ca ASPxSpinEdit din popUpModif sa afiseze '.' si nu ',' ca separator zecimal 
                CultureInfo newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                newCulture.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

                if (!IsPostBack)
                {
                    grDate.Attributes.Add("onkeypress", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));
                    grDate.Attributes.Add("onclick", String.Format("eventKeyPress(event, {0});", grDate.ClientInstanceName));


                    //Adaugam f-urile
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblFormuleCumulat"" WHERE COALESCE(""Vizibil"",0) = 1 ORDER BY COALESCE(""OrdineAfisare"",999999) ", null);
                    for(int i =0; i< dt.Rows.Count; i++)
                    {
                        //GridViewDataColumn c = new GridViewDataColumn();
                        GridViewDataTextColumn c = new GridViewDataTextColumn();
                        c.Name = dt.Rows[i]["Coloana"].ToString();
                        c.FieldName = dt.Rows[i]["Coloana"].ToString();
                        c.Caption = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Alias"],dt.Rows[i]["Coloana"]).ToString());
                        c.ToolTip = Dami.TraduCuvant(General.Nz(dt.Rows[i]["AliasToolTip"], Dami.TraduCuvant(General.Nz(dt.Rows[i]["Alias"], dt.Rows[i]["Coloana"]).ToString())).ToString());
                        c.ReadOnly = true;
                        c.Width = Unit.Pixel(Convert.ToInt32(General.Nz(dt.Rows[i]["Latime"], 100)));
                        c.VisibleIndex = 100 + i;

                        //c.PropertiesEdit.DisplayFormatString = "N0";
                        c.PropertiesTextEdit.DisplayFormatString = "N0";

                        grDate.Columns.Add(c);
                    }
                }


                //Florin 2018.09.25
                //DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NrRanduriPePaginaPTJ'", null);
                //if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                //    grDate.SettingsPager.PageSize = Convert.ToInt32(dtParam.Rows[0][0].ToString());

                grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaPTJ","10"));
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
                //test();

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


                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
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
                            if (General.Nz(lst["IdCateg"], "").ToString() != "") cmbCateg.Value = lst["IdCateg"];

                            btnFiltru_Click(null, null);

                            if (General.Nz(lst["IndexPag"], "").ToString() != "" && General.Nz(lst["IndexRow"], "").ToString() != "")
                            {
                                grDate.PageIndex = Convert.ToInt32(General.Nz(lst["IndexPag"], "1"));
                                grDate.FocusedRowIndex = Convert.ToInt32(General.Nz(lst["IndexRow"], "1"));
                            }
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

                    //DataTable dtert = Session["InformatiaCurenta"] as DataTable;

                    if (Session["InformatiaCurenta"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta"];
                        grDate.DataBind();
                    }
                }

                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                    cmbDept.DataBind();
                }
                else
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                    cmbDept.DataBind();
                }
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT("SELECT F00809, F00810 FROM F008", null);
                cmbBirou.DataBind();


                //Florin 2019.09.23
                cmbCateg.DataSource = General.IncarcaDT(@"SELECT ""Denumire"" AS ""Id"", ""Denumire"" FROM ""viewCategoriePontaj"" GROUP BY ""Denumire"" ", null);
                //cmbCateg.DataSource = General.IncarcaDT("SELECT \"Id\", \"Denumire\" FROM \"viewCategoriePontaj\"", null);
                cmbCateg.DataBind();

                cmbStare.DataSource = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblStariPontaj"" ", null);
                cmbStare.DataBind();

                cmbCtr.DataSource = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte"" ", null);
                cmbCtr.DataBind();

                DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                if (Constante.tipBD == 1)
                    arrZL = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN A.ZiSapt=6 OR A.ZiSapt=7 OR B.DAY IS NOT NULL THEN 'Ziua' + CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2)) + ';' ELSE '' END 
                                        FROM tblzile A
                                        LEFT JOIN HOLIDAYS B ON A.Zi=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month,1)} <= A.Zi AND A.Zi <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}
                                        FOR XML PATH ('')", null), "").ToString();
                else
                    arrZL = General.Nz(General.ExecutaScalar($@"SELECT LISTAGG(CASE WHEN A.""ZiSapt""=6 OR A.""ZiSapt""=7 OR B.DAY IS NOT NULL THEN 'Ziua' || CAST({General.FunctiiData("A.\"Zi\"", "Z")} AS varchar(2))  ELSE '' END,  ';') WITHIN GROUP (ORDER BY A.""Zi"") 
                                        FROM ""tblZile"" A
                                        LEFT JOIN HOLIDAYS B ON A.""Zi""=B.DAY
                                        WHERE {General.ToDataUniv(ziua.Year, ziua.Month, 1)} <= A.""Zi"" AND A.""Zi"" <= {General.ToDataUniv(ziua.Year, ziua.Month, 99)}", null), "").ToString();

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

        protected void btnPeAng_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCol.Count > 0 && txtCol["f10003"] != null)
                {

                    RetineFiltru("1");
                    //string req = "";
                    //var f10003 = txtCol["f10003"];

                    //req += "&Ziua=" + 1;
                    //req += "&Luna=" + Convert.ToDateTime(txtAnLuna.Value).Month;
                    //req += "&An=" + Convert.ToDateTime(txtAnLuna.Value).Year;

                    //if (cmbRol.Value != null) req += "&Rol=" + cmbRol.Value;
                    //if (cmbCtr.Value != null) req += "&IdCtr=" + cmbCtr.Value;
                    //if (cmbStare.Value != null) req += "&IdStr=" + cmbStare.Value;
                    //if (cmbAng.Value != null) req += "&IdAng=" + cmbAng.Value;

                    //if (cmbSub.Value != null) req += "&IdSub=" + cmbSub.Value;
                    //if (cmbFil.Value != null) req += "&IdFil=" + cmbFil.Value;
                    //if (cmbSec.Value != null) req += "&IdSec=" + cmbSec.Value;
                    //if (cmbDept.Value != null) req += "&IdDpt=" + cmbDept.Value;
                    //if (cmbSubDept.Value != null) req += "&IdSubDpt=" + cmbSubDept.Value;
                    //if (cmbBirou.Value != null) req += "&IdBirou=" + cmbBirou.Value;

                    //Session["Filtru_PontajulEchipei"] = req;

                    ////if (req != "") req = "?" + req.Substring(1);
                    ////Response.Redirect("PontajDetaliat.aspx" + req);

                    //grDate.PageIndex
                    //grDate.FocusedRowIndex
                    var f10003 = txtCol["f10003"];
                    Response.Redirect("PontajDetaliat.aspx?tip=10&f10003=" + f10003, false);
                }
                else
                {
                    MessageBox.Show("Nu s-a selectat nici un angajat", MessageBox.icoInfo, "");
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
                    string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");
                    RetineFiltru(ziua);


                    //string req = "";
                    //string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");

                    //req += "&Ziua=" + ziua;
                    //req += "&Luna=" + Convert.ToDateTime(txtAnLuna.Value).Month;
                    //req += "&An=" + Convert.ToDateTime(txtAnLuna.Value).Year;

                    //if (cmbRol.Value != null) req += "&Rol=" + cmbRol.Value;
                    //if (cmbCtr.Value != null) req += "&IdCtr=" + cmbCtr.Value;
                    //if (cmbStare.Value != null) req += "&IdStr=" + cmbStare.Value;
                    //if (cmbAng.Value != null) req += "&IdAng=" + cmbAng.Value;

                    //if (cmbSub.Value != null) req += "&IdSub=" + cmbSub.Value;
                    //if (cmbFil.Value != null) req += "&IdFil=" + cmbFil.Value;
                    //if (cmbSec.Value != null) req += "&IdSec=" + cmbSec.Value;
                    //if (cmbDept.Value != null) req += "&IdDpt=" + cmbDept.Value;
                    //if (cmbSubDept.Value != null) req += "&IdSubDpt=" + cmbSubDept.Value;
                    //if (cmbBirou.Value != null) req += "&IdBirou=" + cmbBirou.Value;

                    //Session["Filtru_PontajulEchipei"] = req;

                    ////if (req != "") req = "?" + req.Substring(1);
                    ////Response.Redirect("PontajDetaliat.aspx" + req);

                    Response.Redirect("PontajDetaliat.aspx?tip=20", false);
                }
                else
                {
                    MessageBox.Show("Nu s-a selectat nici o zi", MessageBox.icoInfo, "");
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
                RetineFiltru("1");
                SetColoane();
                IncarcaGrid();


                //Florin 2019.07.19
                //long dataBlocare = (new DateTime(2200, 12, 31)).Ticks / TimeSpan.TicksPerMillisecond;
                string dataBlocare = "22001231";
                string strSql = $@"SELECT COALESCE(Ziua,'2200-12-31') FROM Ptj_tblBlocarePontaj WHERE IdRol=@1";
                if (Constante.tipBD == 2)
                    strSql = @"SELECT COALESCE(""Ziua"",TO_DATE('31-12-2200','DD-MM-YYYY')) FROM ""Ptj_tblBlocarePontaj"" WHERE ""IdRol""=@1";
                DataTable dt = General.IncarcaDT(strSql, new object[] { Convert.ToInt32(cmbRol.Value ?? -99) });
                if (dt != null && dt.Rows.Count > 0 && General.Nz(dt.Rows[0][0],"").ToString() != "" && General.IsDate(dt.Rows[0][0]))
                    dataBlocare = Convert.ToDateTime(dt.Rows[0][0]).Year + Convert.ToDateTime(dt.Rows[0][0]).Month.ToString().PadLeft(2,'0') + Convert.ToDateTime(dt.Rows[0][0]).Day.ToString().PadLeft(2, '0');

                Session["Ptj_DataBlocare"] = dataBlocare.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //protected void btnFiltruSterge_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        cmbAng.Value = null;
        //        cmbSub.Value = null;
        //        cmbSec.Value = null;
        //        cmbFil.Value = null;
        //        cmbDept.Value = null;
        //        cmbSubDept.Value = null;
        //        cmbBirou.Value = null;
        //        cmbCateg.Value = null;

        //        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
        //        cmbDept.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


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
                    case "EmptyFields":
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
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
                    cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                    cmbFil.DataBind();
                    cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                    cmbSec.DataBind();
                    if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                        cmbDept.DataBind();
                    }
                    else
                    {
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                        cmbDept.DataBind();
                    }
                    cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
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
                General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month);

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

                //DataTable dtErt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Cereri"" WHERE ""DataInceput"" >= " + General.ToDataUniv(dtData.Year, dtData.Month, 99), null);

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

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {

                if (e.DataColumn.FieldName == "StareDenumire")
                {
                    object col = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                    if (col != null) e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(col.ToString());
                }

                if (e.DataColumn.FieldName.Length >= 4 && e.DataColumn.FieldName.ToLower().Substring(0, 4) == "ziua")
                {
                    if (arrZL.Length > 0 && arrZL.IndexOf(e.DataColumn.FieldName + ";") >= 0)
                        e.Cell.BackColor = System.Drawing.Color.Aquamarine;
                    

                    string val = General.Nz(grDate.GetRowValuesByKeyValue(e.KeyValue, "ZileGri"), "").ToString();
                    if (val != "" && (val + ",").IndexOf(e.DataColumn.FieldName) >= 0)
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


        protected void btnTransfera_Click(object sender, EventArgs e)
        {
            int rez = 1;
            int repartProcent = 0;


            DateTime ziua = Convert.ToDateTime(General.Nz(txtAnLuna.Value, DateTime.Now));
            DateTime dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1);
            if (ziua.Year != dtLucru.Year || ziua.Month != dtLucru.Month)
            {
                MessageBox.Show(Dami.TraduCuvant("Luna selectata este diferita de luna de lucru"), MessageBox.icoWarning,"Transfer in salarizare");
                return;
            }

            DataTable entS = Session["InformatiaCurenta"] as DataTable;

            try
            {
                if (entS == null || entS.Rows.Count == 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu exista date de transferat"));
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
                            //var dtLucru = new DateTime(Convert.ToInt32(Dami.ValoareParam("AnLucru")), Convert.ToInt32(Dami.ValoareParam("LunaLucru")), 1);
                            ent.F30035 = dtLucru;
                            ent.F30036 = dtLucru;
                            ent.F30037 = dtLucru;
                            ent.F30038 = dtLucru;

                            for (int j = 0; j < entFor.Rows.Count; j++)
                            {
                                try
                                {
                                    if (General.Nz(entFor.Rows[j]["TransferF300Detaliat"], "").ToString() != "")
                                    {
                                        //Florin 2019.07.19

                                        #region NEW

                                        ent.F30013 = 0;
                                        ent.F30014 = 0;
                                        ent.F30015 = 0;
                                        ent.F30010 = (short?)Convert.ToInt32(entFor.Rows[j]["CodF300"].ToString());

                                        string strInt = $@"SELECT Ziua, ValStr, (CASE WHEN (ZiSapt IN (6,7) OR ZiLibera = 1 OR ZiLiberaLegala=1) THEN 1 ELSE 0 END) AS Libera 
                                            FROM Ptj_Intrari WHERE F10003={ent.F30003} AND Year(Ziua)={dtLucru.Year} AND MONTH(Ziua)={dtLucru.Month} 
                                            AND (CASE WHEN (ZiSapt IN (6,7) OR ZiLibera = 1 OR ZiLiberaLegala=1) THEN '{entFor.Rows[j]["TransferF300Detaliat"]}' ELSE ValStr END) = '{entFor.Rows[j]["TransferF300Detaliat"]}'";
                                        if (Constante.tipBD == 2)
                                            strInt = $@"SELECT ""Ziua"", ""ValStr"", (CASE WHEN (""ZiSapt"" IN (6,7) OR ""ZiLibera"" = 1 OR ""ZiLiberaLegala""=1) THEN 1 ELSE 0 END) AS ""Libera"" 
                                            FROM ""Ptj_Intrari"" WHERE F10003={ent.F30003} AND TO_NUMBER(TO_CHAR(Ziua,'YYYY'))={dtLucru.Year} AND TO_NUMBER(TO_CHAR(Ziua,'MM'))={dtLucru.Month} 
                                            AND (CASE WHEN (""ZiSapt"" IN (6,7) OR ""ZiLibera"" = 1 OR ""ZiLiberaLegala"" = 1) THEN '{entFor.Rows[j]["TransferF300Detaliat"]}' ELSE ""ValStr"" END) = '{entFor.Rows[j]["TransferF300Detaliat"]}'";

                                        DataTable dtInt = General.IncarcaDT(strInt, null);

                                        bool sarit = false;
                                        int cnt = 0;
                                        DateTime ziInc = Convert.ToDateTime(dtInt.Rows[0]["Ziua"]);
                                        DateTime ziSf = Convert.ToDateTime(dtInt.Rows[dtInt.Rows.Count - 1]["Ziua"]);
                                        int ziStart = ziInc.Day - 1;

                                        for (int x = 0; x < dtInt.Rows.Count; x++)
                                        {
                                            if (General.Nz(dtInt.Rows[x]["Valstr"], "").ToString() == entFor.Rows[j]["TransferF300Detaliat"].ToString() && Convert.ToDateTime(dtInt.Rows[x]["Ziua"]).Day - ziStart == 1)
                                            {
                                                cnt += 1;
                                                ziSf = Convert.ToDateTime(dtInt.Rows[x]["Ziua"]);
                                            }

                                            if (Convert.ToDateTime(dtInt.Rows[x]["Ziua"]).Day - ziStart > 1)
                                            {
                                                if (!sarit)
                                                {
                                                    ent.F30013 = cnt;
                                                    ent.F30036 = ziInc;
                                                    ent.F30038 = ziSf;

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

                                                if (Convert.ToInt32(dtInt.Rows[x]["Libera"]) == 1)
                                                {
                                                    sarit = true;
                                                    continue;
                                                }

                                                cnt = 1;
                                                ziInc = Convert.ToDateTime(dtInt.Rows[x]["Ziua"]);
                                                ziSf  = Convert.ToDateTime(dtInt.Rows[x]["Ziua"]);
                                                sarit = false;
                                            }

                                            ziStart = Convert.ToDateTime(dtInt.Rows[x]["Ziua"]).Day;
                                        }

                                        //salvam si ultimul interval

                                        ent.F30013 = cnt;
                                        ent.F30036 = ziInc;
                                        ent.F30038 = Convert.ToDateTime(dtInt.Rows[dtInt.Rows.Count - 1]["Ziua"]);

                                        string strTmp2 = "";
                                        if (Constante.tipBD == 1)
                                            strTmp2 = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042,
                                                    ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                    General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);
                                        else
                                            strTmp2 = string.Format(strSql, ent.F30003, ent.F30010, ent.F30013.ToString().Replace(",", "."), ent.F30014.ToString(), ent.F30015.ToString(), ent.F30042, Dami.NextId("F300"),
                                                    ent.F30004, ent.F30005, ent.F30006, ent.F30007, ent.F30050, General.ToDataUnivPontaj(ent.F30035, 2), General.ToDataUnivPontaj(ent.F30036, 2),
                                                    General.ToDataUnivPontaj(ent.F30037, 2), General.ToDataUnivPontaj(ent.F30038, 2), 1, General.ToDataUnivPontaj(DateTime.Now, 2), ent.F30011, ent.F30002);


                                        General.ExecutaNonQuery(strTmp2, null);

                                        #endregion
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


                                            General.IncarcaDT(strTmp, null);
                                        }
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

                    
                    switch(arr[0].ToString())
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

                    DataTable dtCol = General.IncarcaDT("SELECT * FROM \"Ptj_tblPrint\" WHERE \"Activ\" = 1", null);
                    Dictionary<string, string> lista = new Dictionary<string, string>();
                    Dictionary<string, string> listaLung = new Dictionary<string, string>();
                    if (dtCol != null && dtCol.Rows.Count > 0)
                        for (int j = 0; j < dtCol.Rows.Count; j++)
                        {
                            lista.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["TextAfisare"].ToString());
                            listaLung.Add(dtCol.Rows[j]["Camp"].ToString(), dtCol.Rows[j]["Lungime"].ToString());
                        }

                    DataTable dtAbs = General.IncarcaDT("SELECT DISTINCT \"DenumireScurta\", max(\"Culoare\") AS \"Culoare\" FROM \"Ptj_tblAbsente\" WHERE \"IdTipOre\" = 1 GROUP BY \"DenumireScurta\"", null);
                    Dictionary<string, string> listaAbs = new Dictionary<string, string>();
                    if (dtAbs != null && dtAbs.Rows.Count > 0)
                        for (int j = 0; j < dtAbs.Rows.Count; j++)
                            listaAbs.Add(dtAbs.Rows[j]["DenumireScurta"].ToString(), dtAbs.Rows[j]["Culoare"].ToString());

                    if (chkLinie.Checked)
                    {
                        int nrCol = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (lista.ContainsKey(dt.Columns[i].ColumnName))
                            {
                                ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
                                ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);

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
                            }
                        }

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            nrCol = 0;
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                if (lista.ContainsKey(dt.Columns[i].ColumnName))
                                    ws2.Cells[row + 3, nrCol++].Value = dt.Rows[row][i].ToString();

                                if (dt.Columns[i].ColumnName.Contains("Ziua"))
                                {
                                    ws2.Cells[row + 3, nrCol].Value = dt.Rows[row][i].ToString();
                                    DateTime zi = new DateTime(an, luna, Convert.ToInt32(dt.Columns[i].ColumnName.Replace("Ziua", "").Replace("I", "").Replace("O", "").Replace("P", "")));
                                    bool ziLibera = false;
                                    for (int z = 0; z < dtHolidays.Rows.Count; z++)
                                        if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == zi)
                                        {
                                            ziLibera = true;
                                            break;
                                        }
                                    if (zi.DayOfWeek.ToString().ToLower() == "saturday" || zi.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera) ws2.Cells[row + 3, nrCol].FillColor = Color.FromArgb(217, 243, 253);
                                    if (listaAbs.ContainsKey(dt.Rows[row][i].ToString()))
                                        ws2.Cells[row + 3, nrCol].FillColor = General.Culoare(listaAbs[dt.Rows[row][i].ToString()]);
                                    nrCol++;
                                }
                            }
                        }
                    }
                    else
                    {
                        int nrCol = 0;
                        int rand = 0;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (lista.ContainsKey(dt.Columns[i].ColumnName))
                            {
                                ws2.Cells[1, nrCol].Value = lista[dt.Columns[i].ColumnName];
                                ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung[dt.Columns[i].ColumnName]);
                            }
                            else if (dt.Columns[i].ColumnName.Contains("Ziua") && !dt.Columns[i].ColumnName.Contains("I") && !dt.Columns[i].ColumnName.Contains("O") && !dt.Columns[i].ColumnName.Contains("P"))
                            {
                                ws2.Cells[1, nrCol].Value = dt.Columns[i].ColumnName.Replace("Ziua", "");
                                ws2.Cells[1, nrCol++].ColumnWidth = Convert.ToInt32(listaLung["Zilele 1-31"]);
                            }
                        }

                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            nrCol = -1;
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                if (lista.ContainsKey(dt.Columns[i].ColumnName) || (dt.Columns[i].ColumnName.Contains("Ziua")))
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
                                    ws2.Cells[4 * row + 3 + rand - 1, nrCol].Value = dt.Rows[row][i].ToString();

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
                                    }
                                }

                            }
                        }

                    }

                    //ws2.Columns.AutoFit(1, 500);

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
                    //Response.End();

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
                    bool ras = General.PontajInit(Convert.ToInt32(Session["UserId"]), dt.Year, dt.Month, Convert.ToInt32(cmbRol.Value ?? -99), chkNormaZL.Checked, chkCCCu.Checked, Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99), Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), chkNormaSD.Checked, chkNormaSL.Checked, false, 1, Convert.ToInt32(chkInOut.Checked));
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


        protected void btnStergePontari_Click(object sender, EventArgs e)
        {
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
                                INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE TRUNC(f10023) >= {0} AND TRUNC(F10023) <> TO_DATE('01-JAN-2100','DD-MM-YYYY')) B ON A.F10003=B.F10003 AND A.""Ziua"" >= B.F10023";

                strSql = string.Format(strSql, General.ToDataUniv(Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month));

                string rez = (General.ExecutaScalar(strSql, null) ?? "").ToString();

                if (rez != "")
                {
                    DateTime dt = DateTime.Now;
                    if (txtAnLuna.Value != null) dt = Convert.ToDateTime(txtAnLuna.Value);

                    string ziInc = General.ToDataUniv(dt.Year, dt.Month, 1);
                    string strDel = $@"
                        BEGIN
                        DELETE A
                        FROM Ptj_Intrari A
                        INNER JOIN (SELECT F10003, F10023 FROM f100 WHERE CONVERT(date,f10023) >= {ziInc} AND CONVERT(date,F10023) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua > B.F10023;
                        DELETE A
                        FROM Ptj_Intrari A
                        INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE CONVERT(date,f10022) >= {ziInc} AND CONVERT(date,F10022) <> '2100-01-01') B ON A.F10003=B.F10003 AND A.Ziua < B.F10022;
                        END;";
                    if (Constante.tipBD == 2)
                        strDel = $@"
                            BEGIN
                            DELETE FROM ""Ptj_Intrari"" A
                                    WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10023 FROM f100 WHERE TRUNC(f10023) >= {ziInc}  AND TRUNC(F10023) <> TO_DATE('01/01/2100', 'dd/mm/yyyy')) B  WHERE B.F10003 = A.F10003  AND A.""Ziua"" > B.F10023);
                            DELETE FROM ""Ptj_Intrari"" A
                                    WHERE EXISTS (SELECT B.F10003 FROM (SELECT F10003, F10023 FROM f100 WHERE TRUNC(f10022) >= {ziInc}  AND TRUNC(F10022) <> TO_DATE('01/01/2100', 'dd/mm/yyyy')) B  WHERE B.F10003 = A.F10003  AND A.""Ziua"" < B.F10022);
                            END;";
									
									
                    General.ExecutaNonQuery(strDel, null);
                    MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes"), MessageBox.icoInfo, "Angajati plecati");

                }
                else
                    MessageBox.Show(Dami.TraduCuvant("Nu exista postari"), MessageBox.icoInfo, "Angajati plecati");
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
                int x = 0;
                int select = 0;
                //List<int> ids = new List<int>();
                int idRol = -99;


                if (cmbRol.Value != null) idRol = Convert.ToInt32(cmbRol.Value);
                if ((idRol == 1 && actiune == 0) || idRol == 4 || idRol == -99)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie."), MessageBox.icoError, "Eroare !");
                    return;
                }

                List<metaActiuni> ids = new List<metaActiuni>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "IdStare" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    ids.Add(new metaActiuni { F10003 = Convert.ToInt32(General.Nz(arr[0], -99)), IdStare = Convert.ToInt32(General.Nz(arr[1], -99)) });
                }

                grDate.Selection.UnselectAll();

                if (ids.Count > 0)
                {
                    DateTime ziua = Convert.ToDateTime(txtAnLuna.Value);
                    string rez = AprobaPontaj(Convert.ToInt32(General.Nz(Session["UserId"], -99)), ziua.Year, ziua.Month, ids, Convert.ToInt32(cmbRol.Value ?? -99), actiune, "Pontaj.Pontaj");

                    //grDate.ShowLoadingPanel = false;
                    MessageBox.Show(Dami.TraduCuvant(rez), MessageBox.icoWarning, "Pontajul echipei");
                    btnFiltru_Click(null, null);
                }
                else
                {
                    //grDate.ShowLoadingPanel = false;
                    if (select == 0)
                        MessageBox.Show(Dami.TraduCuvant("Nu exista date selectate !"), MessageBox.icoWarning, "");
                    else
                        MessageBox.Show(Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie."), MessageBox.icoWarning, "");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //public static string ActiuniExec(int actiune, int f10003, int idRol, int idStare, int an, int luna, string pagina)
        public string AprobaPontaj(int idUser, int an, int luna, List<metaActiuni> ids, int idRol, int actiune, string pagina)
        {
            //    Actiune
            // 1   -  aprobat
            // 0   -  respins
            // 2   -  finalizat

            string msg = "";

            try
            {
                foreach (var l in ids)
                {
                    msg += General.ActiuniExec(actiune, l.F10003, idRol, l.IdStare, Convert.ToDateTime(txtAnLuna.Value).Year, Convert.ToDateTime(txtAnLuna.Value).Month, "Pontaj.PontajEchipa", Convert.ToInt32(Session["UserId"]), Convert.ToInt32(Session["User_Marca"])) + System.Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
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
                for(int i=0; i<=20; i++)
                {
                    val_uri += ",\"Val" + i + "\"";
                }

                string sqlVal = $@"SELECT COALESCE(A.""OreInVal"",'') AS ""ValAbs"", A.""DenumireScurta"", A.""Denumire"", A.""Id"", 
                        COALESCE(D.""Afisare"",1) AS ""Afisare"", COALESCE(A.""VerificareNrMaxOre"",0) AS ""VerificareNrMaxOre"",
                        COALESCE(A.""NrMax"", 23) AS ""NrMax"" {val_uri}
                        FROM ""Ptj_tblAbsente"" a
                        INNER JOIN ""Ptj_ContracteAbsente"" b ON a.""Id"" = b.""IdAbsenta""
                        INNER JOIN ""Ptj_relRolAbsenta"" c ON a.""Id"" = c.""IdAbsenta""
                        INNER JOIN(SELECT * FROM ""Ptj_Intrari"" Y WHERE {General.ToDataUniv(ziua)} <= CAST(Y.""Ziua"" AS DATE) AND CAST(Y.""Ziua"" AS DATE) <= {General.ToDataUniv(ziua)} AND Y.F10003 = {f10003}) P ON 1 = 1
                        LEFT JOIN ""Ptj_Contracte"" D ON B.""IdContract"" = D.""Id""
                        WHERE A.""OreInVal"" IS NOT NULL {strGol1} AND B.""IdContract"" = P.""IdContract"" AND C.""IdRol"" = {cmbRol.Value} AND
                        (
                        (COALESCE(B.ZL,0)<> 0 AND (CASE WHEN(P.""ZiSapt"" < 6 AND P.""ZiLibera"" = 0) THEN 1 ELSE 0 END) = COALESCE(B.ZL,0)) OR
                        (COALESCE(B.S,0) <> 0 AND (CASE WHEN P.""ZiSapt"" = 6 THEN 1 ELSE 0 END) = COALESCE(B.S,0)) OR
                        (COALESCE(B.D,0)<> 0 AND (CASE WHEN P.""ZiSapt"" = 7 THEN 1 ELSE 0 END) = COALESCE(B.D,0)) OR
                        (COALESCE(B.SL,0)<> 0 AND COALESCE(P.""ZiLiberaLegala"",0) = COALESCE(B.SL,0))
                        ) 
                        GROUP BY A.""OreInVal"", A.""DenumireScurta"", A.""Denumire"", A.""Id"", D.""Afisare"", A.""NrMax"", A.""VerificareNrMaxOre"" {val_uri}
                        ORDER BY A.""OreInVal"" ";

                DataTable dtVal = General.IncarcaDT(sqlVal, null);

                for (int i = 0; i < dtVal.Rows.Count; i++)
                {
                    DataRow dr = dtVal.Rows[i];
                    string id = dr["ValAbs"] + "_" + General.Nz(dr["DenumireScurta"], "").ToString().Trim().Replace(" ","") + "_" + dr["VerificareNrMaxOre"];

                    HtmlGenericControl divCol = new HtmlGenericControl("div");
                    divCol.Attributes["class"] = "col-md-3";
                    divCol.Style["margin-bottom"] = "15px";

                    //HtmlGenericControl lbl = new HtmlGenericControl("label");
                    //lbl.Style["width"] = "100%";
                    //lbl.InnerHtml = General.Nz(dr["DenumireScurta"], "&nbsp;").ToString();
                    ////Label lbl = new Label();
                    ////lbl.Style["width"] = "100%";
                    ////lbl.Text = General.Nz(dr["DenumireScurta"], "&nbsp;").ToString();


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
                    //txt.Attributes["validareMaxOre"] = i.ToString();

                    if (General.Nz(dr["Afisare"], "1").ToString() == "1")
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

                                if (General.Nz(dr["Afisare"], "1").ToString() == "1")
                                {
                                    txt.Text = (min / 60).ToString();
                                }
                                else
                                {
                                    txt.Text = (min / 60).ToString() + "," + (min % 60).ToString();
                                }
                            }
                            catch (Exception) { }
                        }
                    }

                    divCol.Controls.Add(lbl);
                    divCol.Controls.Add(txt);

                    pnlValuri.Controls.Add(divCol);
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



                //string sqlDel = $@"UPDATE ""Ptj_Intrari"" SET Val0=null,Val1=null,Val2=null,Val3=null,Val4=null,Val5=null,Val6=null,Val7=null,Val8=null,Val9=null,Val10=null,
                //                Val11=null,Val12=null,Val13=null,Val14=null,Val15=null,Val16=null,Val17=null,Val18=null,Val19=null,Val20=null,
                //                ValModif0=null,ValModif1=null,ValModif2=null,ValModif3=null,ValModif4=null,ValModif5=null,ValModif6=null,ValModif7=null,ValModif8=null,ValModif9=null,ValModif10=null,
                //                ValModif11=null,ValModif12=null,ValModif13=null,ValModif14=null,ValModif15=null,ValModif16=null,ValModif17=null,ValModif18=null,ValModif19=null,ValModif20=null
                //                WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}";

                string sqlDel = $@"UPDATE ""Ptj_Intrari"" SET ""Val0""=null,""Val1""=null,""Val2""=null,""Val3""=null,""Val4""=null,""Val5""=null,""Val6""=null,""Val7""=null,""Val8""=null,""Val9""=null,""Val10""=null,
                                ""Val11""=null,""Val12""=null,""Val13""=null,""Val14""=null,""Val15""=null,""Val16""=null,""Val17""=null,""Val18""=null,""Val19""=null,""Val20""=null
                                WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}";



                if (General.Nz(cmbTipAbs.Value, "").ToString() != "")
                {
                    General.ExecutaNonQuery(sqlDel, null);
                    General.ExecutaNonQuery($@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" = '{cmbTipAbs.Text}' WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);
                    General.CalculFormuleCumulat(f10003, ziua.Year, ziua.Month);

                    IncarcaGrid();
                }
                else
                {
                    //txtValuri
                    //,Val0__1=1,Val1_OS_0=3,Val2_ERT_1=2
                    if (txtCol.Count > 0 && txtCol["valuri"] != null)
                    {
                        var ert = txtCol["valuri"];
                        
                        DataRow drMd = General.IncarcaDR($@"SELECT * FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);
                        string valStr = "";
                        string cmp = "";
                        string cmpModif = "";
                        int nrMin = 0;

                        var txt = txtCol["valuri"].ToString().Replace("_I=", "=");
                        string[] arrVal = txt.Split(';');
                        for (int i = 0; i < arrVal.Length; i++)
                        {
                            if (arrVal[i] != "")
                            {
                                string[] arrAtr = arrVal[i].Split('=');
                                if (arrAtr[0] != "" && arrAtr[1] != "" && arrAtr[1] != "0" && arrAtr[1] != "0,00")
                                {
                                    string[] str = arrAtr[0].Split('_');
                                    valStr += "/" + arrAtr[1] + str[1];

                                    //salvam val-urile
                                    try
                                    {
                                        cmp += ",\"" + str[0] + "\"=" + Convert.ToInt32((Convert.ToDecimal(arrAtr[1]) * 60)).ToString();
                                        if (str[2] == "1") nrMin += Convert.ToInt32(Convert.ToDecimal(arrAtr[1]) * 60);
                                    }
                                    catch (Exception ex) { var ert55 = ex.Message; }


                                    //marcam ce valuri au fost modificate manual
                                    try
                                    {
                                        if (drMd == null)
                                        {
                                            cmpModif += ",\"" + str[0].Replace("Val", "ValModif") + "\"=4";
                                        }
                                        else
                                        {
                                            if (drMd[str[0]].ToString() != arrAtr[1].ToString())
                                                cmpModif += ",\"" + str[0].Replace("Val", "ValModif") + "\"=4";
                                        }
                                    }
                                    catch (Exception) { }


                                }
                            }
                        }

                        if (valStr != "") valStr = valStr.Substring(1);

                        string sqlIst = "";
                        try
                        {
                            sqlIst = $@"INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", USER_NO, TIME) 
                                           VALUES ({f10003}, {General.ToDataUniv(ziua)}, '{valStr}', '{General.Nz(drMd["ValStr"], "")}', {Session["UserId"]}, {General.ToDataUniv(DateTime.Now, true)}, {Session["UserId"]}, {General.ToDataUniv(DateTime.Now, true)})";
                        }
                        catch (Exception){}

                        if (nrMin != 0)
                        {
                            string msgNr = VerificareDepasireNorma(f10003, ziua.Date, nrMin, 2);
                            if (msgNr != "")
                            {
                                MessageBox.Show(Dami.TraduCuvant(msgNr), MessageBox.icoWarning);
                                return;
                            }
                        }

                        General.ExecutaNonQuery(sqlDel, null);
                        General.ExecutaNonQuery($@"UPDATE ""Ptj_Intrari"" SET ""ValStr"" = '{valStr}' {cmp} {cmpModif} WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(ziua)}", null);
                        General.ExecutaNonQuery(sqlIst, null);
                        General.CalculFormuleCumulat(f10003, ziua.Year, ziua.Month);

                        IncarcaGrid();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string VerificareDepasireNorma(int f10003, DateTime dtInc, int? nrMin, int tip)
        {
            //tip
            //tip - 1  vine din cererei - unde trebuie sa luam in caclul si valorile care deja exista in pontaj
            //tip - 2  vine din pontaj  - valorile sunt deja in pontaj


            string msg = "";

            try
            {
                //calculam norma
                string strSql = "SELECT CAST(rez as int) FROM DamiNorma(" + f10003 + "," + General.ToDataUniv(dtInc) + ")";
                if (Constante.tipBD == 2) strSql = "SELECT \"DamiNorma\"(" + f10003 + ", " + General.ToDataUniv(dtInc) + ") FROM DUAL";
                int norma = Convert.ToInt32(General.ExecutaScalar(strSql, null));

                int sumaPtj = 0;
                if (tip == 1)
                {
                    //absentele din pontaj care intra in suma de ore
                    string sqlOre = @"SELECT ' + COALESCE(' + OreInVal + ',0)'  FROM Ptj_tblAbsente WHERE COALESCE(VerificareNrMaxOre,0) = 1 FOR XML PATH ('')";
                    if (Constante.tipBD == 2) sqlOre = @"SELECT LISTAGG('COALESCE(' || ""OreInVal"" || ')', ' + ') WITHIN GROUP (ORDER BY ""OreInVal"") FROM ""Ptj_tblAbsente"" WHERE COALESCE(VerificareNrMaxOre,0) = 1";
                    string strVal = (General.ExecutaScalar(sqlOre, null) ?? "").ToString();
                    if (Constante.tipBD == 1) strVal = strVal.Substring(3);
                    if (strVal != "") sumaPtj = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM({strVal}), 0) FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(dtInc.Date)}", null));
                }

                //suma de ore din Cereri
                int sumaCere = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM(COALESCE(""NrOre"",0)),0) FROM ""Ptj_Cereri"" WHERE F10003={f10003} AND ""DataInceput"" = {General.ToDataUniv(dtInc.Date)} AND ""IdStare"" IN (1,2)", null));
                if (((sumaCere * 60) + sumaPtj + (nrMin)) > (norma * 60))
                {
                    msg = "Totalul de ore depaseste norma pe aceasta zi";
                    return msg;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
        }

        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {
                var lstZile = new Dictionary<int, object>();

                var grid = sender as ASPxGridView;
                //for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
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

                string strFiltru = "", strLeg = "";
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (Convert.ToInt32(cmbDept.Value ?? -99) != -99) strFiltru += " AND A.F10007 = " + cmbDept.Value;
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99)
                {
                    strFiltru += " AND A.F100958 = " + cmbSubDept.Value;
                    strLeg = " LEFT JOIN (SELECT F10003, F100958, F100959 FROM F1001) B ON A.F10003 = B.F10003 ";
                }
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99)
                {
                    strFiltru += " AND A.F100959 = " + cmbBirou.Value;
                    strLeg = " LEFT JOIN (SELECT F10003, F100958, F100959 FROM F1001) B ON A.F10003 = B.F10003 ";
                }
                //Florin 2019.09.23
                //if (Convert.ToInt32(cmbCateg.Value ?? -99) != -99)
                if (General.Nz(cmbCateg.Value,"").ToString() != "")
                {
                    filtruPlus += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                    strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    //strInner += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    //strFiltru += " AND (A.F10061 = " + cmbCateg.Value + " OR A.F10062 = " + cmbCateg.Value + ")";
                    //strLeg += " LEFT JOIN (SELECT F10003, F10061, F10062 FROM F100) C ON A.F10003 = C.F10003 ";
                }
                if (Convert.ToInt32(cmbCtr.Value ?? -99) != -99) strFiltru += " AND A.\"IdContract\" = " + cmbCtr.Value;

                //Radu 13.03.2019
                string strFiltruSpecial = "";
                if (Dami.ValoareParam("PontajulEchipeiFiltruAplicat") == "1")
                    strFiltruSpecial = strFiltru.Replace("A.F10095", "B.F10095").Replace("A.F1006", "C.F1006");
                else
                    strLeg = "";

                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += " AND COALESCE(A.\"IdStare\",1) = " + cmbStare.Value;


                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                    strFiltru += General.GetF10003Roluri(idUser, an, luna, 0, f10003, idRol, 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;

                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                {
                    string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                    if (Constante.tipBD == 2) strZi = "'" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + an.ToString() + "'";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";
                    zileVal += $@",COALESCE(""Ziua{i}"",'') AS ""Ziua{i}""";
                }

                for (int i = 1; i <= 60; i++)
                {
                    zileF += $@",CAST(COALESCE(X.""F{i}"",0) AS numeric(10)) AS ""F{i}_Tmp""";
                }


                if (Dami.ValoareParam("TipCalculDate") == "2")
                    strInner += $@"LEFT JOIN DamiNorma_Table dn ON dn.F10003=X.F10003 AND dn.dt={dtSf}
								LEFT JOIN DamiDataPlecare_Table ddp ON ddp.F10003=X.F10003 AND ddp.dt={dtSf}";
                else
                    strInner += $@"OUTER APPLY dbo.DamiNorma(X.F10003, {dtSf}) dn 
                                OUTER APPLY dbo.DamiDataPlecare(X.F10003, {dtSf}) ddp ";


                //Florin 2019.09.23 s-a scos de mai jos LEFT JOIN F724 etc.
                //Radu 19.09.2019 - am inlocuit F724 cu viewCategoriePontaj
                //LEFT JOIN F724 CA ON A.F10061 = CA.F72402 
                //LEFT JOIN F724 CB ON A.F10062 = CB.F72402
                if (Constante.tipBD == 1)
                    strSql = $@"with ptj_intrari_2 as (select A.* from Ptj_Intrari A {strLeg}  WHERE 1=1 {strFiltruSpecial})
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
                                B.F100958, B.F100959,
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, CTG.Denumire as Categorie,
                                ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, K.Denumire AS StareDenumire,
                                A.F10078 AS Angajator, DR.F08903 AS TipContract, 
                                (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                                dn.Norma AS AvansNorma, 
                                CASE WHEN Y.Norma <> dn.Norma THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)={an} AND MONTH(F70406)={luna}) ELSE {General.ToDataUniv(2100, 1, 1)} END AS AvansData,
                                L.F06205, Fct.F71804 AS Functie,
                                X.* {zileVal} {zileF}
                                FROM Ptj_Cumulat X 
		                        LEFT JOIN Ptj_tblStari st on st.Id = x.IdStare
		                        left join SituatieZileAbsente zabs on zabs.F10003 = x.F10003 and zabs.An = x.An and zabs.IdAbsenta = (select Id from Ptj_tblAbsente where DenumireScurta = 'CO')
		                        left join SituatieZLP zlp on zlp.F10003 = x.F10003 and zlp.An = x.An
                                INNER JOIN (SELECT F10003 {zileAs} FROM 
                                (SELECT F10003, {cmpValStr}, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(ValStr) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari_2 R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN Ptj_Intrari_2 Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                                LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                                LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                LEFT JOIN viewCategoriePontaj CTG ON A.F10003 = CTG.F10003
                                {strInner}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON B.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.F06204Default=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                                WHERE X.An = {an} AND X.Luna = {luna} {filtruPlus}
                                ORDER BY AngajatNume) A
                                WHERE 1=1 {strFiltru}";
                else
                    strSql = $@"with ""Ptj_Intrari_2"" as (select * from ""Ptj_Intrari"" A WHERE 1=1 {strFiltruSpecial})
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
                                TO_CHAR(A.F10022, 'dd/mm/yyyy') AS ""DataInceput"", TO_CHAR(""DamiDataPlecare""(X.F10003, {dtSf}), 'dd/mm/yyyy') AS ""DataSfarsit"",  A.F10008 || ' ' || A.F10009 AS ""AngajatNume"", C.""Id"" AS ""IdContract"", 
                                Y.""Norma"", Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, 
                                C.""Denumire"" AS ""DescContract"", NVL(C.""OreSup"",0) AS ""OreSup"", NVL(C.""Afisare"",1) AS ""Afisare"", 
                                B.F100958, B.F100959,
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, CTG.""Denumire"" as ""Categorie"",
                                NVL(K.""Culoare"",'#FFFFFFFF') AS ""Culoare"", K.""Denumire"" AS ""StareDenumire"",
                                A.F10078 AS ""Angajator"", DR.F08903 AS ""TipContract"", 
                                (SELECT MAX(US.F70104) FROM USERS US WHERE US.F10003=X.F10003) AS EID,
                                ""DamiNorma""(X.F10003, {dtSf}) AS ""AvansNorma"", 
                                CASE WHEN ""Norma"" <> ""DamiNorma""(X.F10003, {dtSf}) THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND EXTRACT(YEAR FROM F70406)={an} AND EXTRACT(MONTH FROM F70406)={luna}) ELSE {General.ToDataUniv(2100, 1, 1)} END AS ""AvansData"",
                                L.F06205, Fct.F71804 AS ""Functie"",                                
                                X.* {zileVal} {zileF}
                                FROM ""Ptj_Cumulat"" X 
                                LEFT JOIN ""Ptj_tblStari"" st on st.""Id"" = x.""IdStare""
                                left join (SELECT * FROM ""SituatieZileAbsente"" WHERE ""IdAbsenta"" = (select ""Id"" from ""Ptj_tblAbsente"" where ""DenumireScurta"" = 'CO')) zabs on zabs.F10003 = x.F10003 and zabs.""An"" = x.""An""
                                left join ""SituatieZLP"" zlp on zlp.F10003 = x.F10003 and zlp.""An"" = x.""An""
                                INNER JOIN (SELECT * FROM 
                                (SELECT F10003, ""ValStr"", ""Ziua"" From ""Ptj_Intrari_2"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf})  source  
                                PIVOT  (MAX(COALESCE(""ValStr"",'')) FOR ""Ziua"" IN ( {zileAs.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari_2"" R WHERE EXTRACT(YEAR FROM R.""Ziua"")= {an} AND EXTRACT(MONTH FROM R.""Ziua"")= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN ""Ptj_Intrari_2"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                                LEFT JOIN ""Ptj_tblStariPontaj"" K ON K.""Id"" = NVL(X.""IdStare"",1) 
                                LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                {strInner}
							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON B.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.""F06204Default""=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802
                                LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003
                                WHERE X.""An""= {an} AND X.""Luna"" = {luna} {filtruPlus}
                                ORDER BY ""AngajatNume"") A
                                WHERE 1=1 {strFiltru}";
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

                string cmpValStr = "ValStr";

                int idUser = Convert.ToInt32(Session["UserId"]);
                int f10003 = Convert.ToInt32(Session["User_Marca"]);

                int an = Convert.ToDateTime(txtAnLuna.Value).Year;
                int luna = Convert.ToDateTime(txtAnLuna.Value).Month;
                int idRol = Convert.ToInt32(cmbRol.Value ?? -99);

                string dtInc = General.ToDataUniv(an, luna, 1);
                string dtSf = General.ToDataUniv(an, luna, 99);

                string zile = "";
                string zileAs = "", zileAsIn = "", zileAsOut = "", zileAsPauza = "";
                string zileVal = "";
                string zileF = "";
                string strInner = "";
                string filtruPlus = "";

                string pvt = "", pvtIn = "", pvtOut = "", pvtPauza = "";

                string strFiltru = "", strLeg = "";
                if (Convert.ToInt32(cmbSub.Value ?? -99) != -99) strFiltru += " AND A.F10004 = " + cmbSub.Value;
                if (Convert.ToInt32(cmbFil.Value ?? -99) != -99) strFiltru += " AND A.F10005 = " + cmbFil.Value;
                if (Convert.ToInt32(cmbSec.Value ?? -99) != -99) strFiltru += " AND A.F10006 = " + cmbSec.Value;
                if (Convert.ToInt32(cmbDept.Value ?? -99) != -99) strFiltru += " AND A.F10007 = " + cmbDept.Value;
                if (Convert.ToInt32(cmbSubDept.Value ?? -99) != -99)
                {
                    strFiltru += " AND A.F100958 = " + cmbSubDept.Value;
                    strLeg = " LEFT JOIN (SELECT F10003 AS MARCA1, F100958, F100959 FROM F1001) B ON A.F10003 = B.MARCA1 ";
                }
                if (Convert.ToInt32(cmbBirou.Value ?? -99) != -99)
                {
                    strFiltru += " AND A.F100959 = " + cmbBirou.Value;
                    strLeg = " LEFT JOIN (SELECT F10003 AS MARCA2, F100958, F100959 FROM F1001) B ON A.F10003 = B.MARCA2 ";
                }
                //Florin 2019.09.23
                //if (Convert.ToInt32(cmbCateg.Value ?? -99) != -99)
                if (General.Nz(cmbCateg.Value, "").ToString() != "")
                {
                    filtruPlus += @" AND CTG.""Denumire"" = '" + cmbCateg.Value + "'";
                    strLeg += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    //strInner += @" LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 ";
                    //strFiltru += " AND (A.F10061 = " + cmbCateg.Value + " OR A.F10062 = " + cmbCateg.Value + ")";
                    //strLeg += " LEFT JOIN (SELECT F10003, F10061, F10062 FROM F100) C ON A.F10003 = C.F10003 ";
                }
                if (Convert.ToInt32(cmbCtr.Value ?? -99) != -99) strFiltru += " AND A.\"IdContract\" = " + cmbCtr.Value;

                string strFiltruSpecial = "";
                if (Dami.ValoareParam("PontajulEchipeiFiltruAplicat") == "1")
                    strFiltruSpecial = strFiltru.Replace("A.F10095", "B.F10095").Replace("A.F10061", "C.CATEG1").Replace("A.F10062", "C.CATEG2");
                else
                    strLeg = "";
                
                if (Convert.ToInt32(cmbStare.Value ?? -99) != -99) strFiltru += " AND COALESCE(A.\"IdStare\",1) = " + cmbStare.Value;
                if (Convert.ToInt32(cmbAng.Value ?? -99) == -99)
                    strFiltru += General.GetF10003Roluri(idUser, an, luna, 0, f10003, idRol, 0, -99, Convert.ToInt32(cmbAng.Value ?? -99));
                else
                    strFiltru += " AND A.F10003=" + cmbAng.Value;


                for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                {
                    string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                    if (Constante.tipBD == 2) strZi = "'" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + an.ToString() + "'";

                    zile += ", " + strZi;
                    zileAs += ", " + strZi + " AS \"Ziua" + i.ToString() + "\"";

                    if (chkTotaluri.Checked)
                        zileVal += $@",COALESCE(""Ziua{i}"",'') AS ""Ziua{i}""";
                    if (chkOre.Checked)
                        zileVal += $@",COALESCE(CONVERT(VARCHAR(5), ""Ziua{i}I"", 108),'') AS ""Ziua{i}I""" + $@",COALESCE(CONVERT(VARCHAR(5), ""Ziua{i}O"", 108),'') AS ""Ziua{i}O""";
                    if (chkPauza.Checked)
                        zileVal += $@",COALESCE(""Ziua{i}P"",'') AS ""Ziua{i}P""";

                    zileAsIn += ", " + strZi + " AS \"Ziua" + i.ToString() + "I\"";
                    zileAsOut += ", " + strZi + " AS \"Ziua" + i.ToString() + "O\"";
                    zileAsPauza += ", " + strZi + " AS \"Ziua" + i.ToString() + "P\"";
                }

                for (int i = 1; i <= 60; i++)
                {
                    zileF += $@",CAST(COALESCE(X.""F{i}"",0) AS numeric(10)) AS ""F{i}""";
                }

                if (chkTotaluri.Checked)
                    pvt = $@"INNER JOIN (SELECT F10003 {zileAs} FROM 
                                (SELECT F10003, {cmpValStr}, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(ValStr) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003";

                if (chkOre.Checked)
                {
                    pvtIn = $@"INNER JOIN (SELECT F10003 {zileAsIn} FROM 
                                (SELECT F10003, FirstInPaid, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(FirstInPaid) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvtIn ON X.F10003=pvtIn.F10003";

                    pvtOut = $@"INNER JOIN (SELECT F10003 {zileAsOut} FROM 
                                (SELECT F10003, LastOutPaid, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(LastOutPaid) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvtOut ON X.F10003=pvtOut.F10003";
                }
                if (chkPauza.Checked)
                    pvtPauza = $@"INNER JOIN (SELECT F10003 {zileAsPauza} FROM 
                                (SELECT F10003, TimpPauzaReal, Ziua From Ptj_Intrari_2 WHERE {dtInc} <= CAST(Ziua AS date) AND CAST(Ziua AS date) <= {dtSf}) AS source  
                                PIVOT  (MAX(TimpPauzaReal) FOR Ziua IN ( {zile.Substring(1)} )) pvt
                                ) pvtPauza ON X.F10003=pvtPauza.F10003";



                if (Dami.ValoareParam("") == "2")
                    strInner += $@"LEFT JOIN DamiNorma_Table dn ON dn.F10003=X.F10003 AND dn.dt={dtSf}
								LEFT JOIN DamiDataPlecare_Table dd ON ddp.F10003=X.F10003 AND ddp.dt={dtSf}";
                else
                    strInner += $@"OUTER APPLY dbo.DamiNorma(X.F10003, {dtSf}) dn 
                                OUTER APPLY dbo.DamiDataPlecare(X.F10003, {dtSf}) ddp ";

                //Radu 19.09.2019 - am inlocuit F724 cu viewCategoriePontaj
                //LEFT JOIN F724 CA ON A.F10061 = CA.F72402 
                //LEFT JOIN F724 CB ON A.F10062 = CB.F72402
                if (Constante.tipBD == 1)
                    strSql = $@"with ptj_intrari_2 as (select * from Ptj_Intrari A {strLeg}  WHERE 1=1 {strFiltruSpecial})
                                SELECT *
                           
                                FROM (
                                 SELECT TOP 100 PERCENT X.F10003, CONVERT(VARCHAR, A.F10022, 103) AS DataInceput, convert(VARCHAR, ddp.DataPlecare, 103) AS DataSfarsit, A.F10008 + ' ' + A.F10009 AS AngajatNume, st.Denumire AS StarePontaj, isnull(zabs.Ramase, 0) as ZileCONeefectuate, isnull(zlp.Ramase, 0) as ZLPNeefectuate,
                                 H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", F10061, F10062, B.F100958, B.F100959, Y.IdContract, A.F100901 AS EID, CTG.Denumire as Categorie
                                {zileVal}  {zileF}
                                FROM Ptj_Cumulat X 
		                        LEFT JOIN Ptj_tblStari st on st.Id = x.IdStare
		                        left join SituatieZileAbsente zabs on zabs.F10003 = x.F10003 and zabs.An = x.An and zabs.IdAbsenta = (select Id from Ptj_tblAbsente where DenumireScurta = 'CO')
		                        left join SituatieZLP zlp on zlp.F10003 = x.F10003 and zlp.An = x.An
                                {pvt}
                                {pvtIn}
                                {pvtOut}
                                {pvtPauza}
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari_2 R WHERE YEAR(R.Ziua)= {an} AND MONTH(R.Ziua)= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN Ptj_Intrari_2 Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin
                                LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(X.IdStare,1) 
                                LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract 
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                LEFT JOIN viewCategoriePontaj CTG ON A.F10003 = CTG.F10003 
                                {strInner}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON B.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.F06204Default=L.F06204

                                LEFT JOIN F718 Fct ON A.F10071=Fct.F71802

                                WHERE X.An = {an} AND X.Luna = {luna} {filtruPlus}
                                ORDER BY NumeComplet) A
                                WHERE 1=1 {strFiltru}";
                else
                    strSql = $@"with ""Ptj_Intrari_2"" as (select * from ""Ptj_Intrari"" A WHERE 1=1 {strFiltruSpecial})
                                SELECT  *                                
                                FROM (
                                SELECT X.F10003, TO_CHAR(A.F10022, 'dd/mm/yyyy') AS ""DataInceput"", TO_CHAR(""DamiDataPlecare""(X.F10003, {dtSf}), 'dd/mm/yyyy') AS ""DataSfarsit"", A.F10008 || ' ' || A.F10009 AS ""AngajatNume"", st.""Denumire"" AS ""StarePontaj"", nvl(zabs.""Ramase"", 0) as ""ZileCONeefectuate"", isnull(zlp.""Ramase"", 0) as ""ZLPNeefectuate"",
                                H.F00507 AS ""Sectie"",I.F00608 AS ""Dept"", S2.F00204 AS ""Companie"", S3.F00305 AS ""Subcompanie"", S4.F00406 AS ""Filiala"", S7.F00709 AS ""Subdept"", S8.F00810 AS ""Birou"", CA.""Denumire"" AS ""Categorie1"", CB.""Denumire"" AS ""Categorie2"" ,  F10061, F10062,  B.F100958, B.F100959  , Y.""IdContract"" , A.F100901 AS EID , CTG.""Denumire"" as ""Categorie""                   
                                {zileVal} {zileF}
                                FROM ""Ptj_Cumulat"" X 
		                        LEFT JOIN ""Ptj_tblStari"" st on st.""Id"" = x.""IdStare""
		                        left join (SELECT * FROM ""SituatieZileAbsente"" WHERE ""IdAbsenta"" = (select ""Id"" from ""Ptj_tblAbsente"" where ""DenumireScurta"" = 'CO')) zabs on zabs.F10003 = x.F10003 and zabs.""An"" = x.""An""
                                left join ""SituatieZLP"" zlp on zlp.F10003 = x.F10003 and zlp.""An"" = x.""An""
                                INNER JOIN (SELECT F10003 {zileAs} FROM 
                                (SELECT F10003, ""ValStr"", ""Ziua"" From ""Ptj_Intrari"" WHERE {dtInc} <= CAST(""Ziua"" AS date) AND CAST(""Ziua"" AS date) <= {dtSf})  source  
                                PIVOT  (MAX(""ValStr"") FOR ""Ziua"" IN ( {zile.Substring(1)} )) pvt
                                ) pvt ON X.F10003=pvt.F10003
                                LEFT JOIN F100 A ON A.F10003=X.F10003 
                                LEFT JOIN F1001 B ON A.F10003=B.F10003 
                                LEFT JOIN (SELECT R.F10003, MIN(R.""Ziua"") AS ""ZiuaMin"" FROM ""Ptj_Intrari"" R WHERE EXTRACT(YEAR FROM R.""Ziua"")= {an} AND EXTRACT(MONTH FROM R.""Ziua"")= {luna} GROUP BY R.F10003) Q ON Q.F10003=A.F10003
                                LEFT JOIN ""Ptj_Intrari"" Y ON A.F10003=Y.F10003 AND Y.""Ziua""=Q.""ZiuaMin""
                                LEFT JOIN ""Ptj_tblStariPontaj"" K ON K.""Id"" = NVL(X.""IdStare"",1) 
                                LEFT JOIN ""Ptj_Contracte"" C on C.""Id"" = Y.""IdContract""
                                LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 
                                LEFT JOIN ""viewCategoriePontaj"" CTG ON A.F10003 = CTG.F10003 
                                {strInner}

							    LEFT JOIN F002 S2 ON Y.F10002 = S2.F00202
							    LEFT JOIN F003 S3 ON Y.F10004 = S3.F00304
							    LEFT JOIN F004 S4 ON Y.F10005 = S4.F00405

							    LEFT JOIN F005 H ON Y.F10006 = H.F00506
							    LEFT JOIN F006 I ON Y.F10007 = I.F00607

							    LEFT JOIN F007 S7 ON B.F100958 = S7.F00708
                                LEFT JOIN F008 S8 ON B.F100959 = S8.F00809

                                LEFT JOIN F062 L ON Y.F06204Default=L.F06204

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



    }
}
 
 
 
 
 
 
 
 
 
 
 
 