using DevExpress.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Web.Hosting;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using WizOne.Module;

namespace WizOne.Adev
{
    public partial class Adeverinta : System.Web.UI.Page
    {
        string fisier = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                if (!IsPostBack)
                {
                    Session["MarcaConfigCIC"] = null;
                    config.Visible = false;
                    Session["AdevConfig"] = "false";
                }

                if (Session["AdevConfig"] != null)
                    config.Visible = Session["AdevConfig"].ToString() == "true" ? true : false;


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnGenerare.Text = Dami.TraduCuvant("btnGenerare", "Genereaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblAngBulk.InnerText = Dami.TraduCuvant("Angajat");
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

                /////TEST
                bulk1.Visible = false;
                bulk2.Visible = false;
                btnFiltru.Visible = false;
                btnFiltruSterge.Visible = false;
                grDate.Visible = false;
                ///////////
                


                //cmbAng.SelectedIndex = -1;

                Dictionary<String, String> lista = new Dictionary<string, string>();
                if (!IsPostBack)
                    lista = LoadParameters();
                else
                    lista = Session["AdevListaParam"] as Dictionary<string, string>;

                DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                cmbAngBulk.DataSource = dtAng;
                cmbAngBulk.DataBind();

                if (!IsPostBack)
                {
                    UpdateControls(lista);
                    if (!lista.ContainsKey("XML"))
                        lista.Add("XML", "1");
                    else
                        lista["XML"] = "1";
                    SalvareParam(lista);

                    Session["InformatiaCurenta_Adev"] = null;
                }
                else
                {
                    DataTable dtert = Session["InformatiaCurenta_Adev"] as DataTable;

                    if (Session["InformatiaCurenta_Adev"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta_Adev"];
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

                cmbCateg.DataSource = General.IncarcaDT("SELECT F72402, F72404 FROM F724", null);
                cmbCateg.DataBind();

                cmbCtr.DataSource = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Contracte"" ", null);
                cmbCtr.DataBind();

            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void UpdateControls(Dictionary<string, string> lista)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Denumire", typeof(string));

            table.Rows.Add(0, "Sănătate 2019");
            table.Rows.Add(1, "Sănătate");
            table.Rows.Add(2, "Venituri anuale");
            table.Rows.Add(3, "CIC");
            table.Rows.Add(4, "Șomaj");
            table.Rows.Add(6, "Stagiu");
            table.Rows.Add(7, "Vechime");
     
            cmbAdev.DataSource = table;
            cmbAdev.DataBind();
            //cmbAdev.SelectedIndex = 0;

            DataTable dtAng = General.IncarcaDT(SelectAngajati(), null);
            cmbAng.DataSource = dtAng;
            cmbAng.DataBind();

            foreach (string sufix in lista.Keys)
                switch (sufix)
                {
                    case "NR1": txtNumeRL1.Text = lista[sufix]; break;
                    case "FR1": txtFunctieRL1.Text = lista[sufix]; break;
                    case "NR2": txtNumeRL2.Text = lista[sufix]; break;
                    case "FR2": txtFunctieRL2.Text = lista[sufix]; break;
                    case "INT":
                        int interval = 12;
                        int.TryParse(lista[sufix], out interval);
                        if (interval == 12)
                        {
                            rbInterval1.Checked = true;
                            rbInterval2.Checked = false;
                        }
                        else
                        {
                            rbInterval1.Checked = false;
                            rbInterval2.Checked = true;
                        }
                        break;
                    case "EMI":
                        string emitent = lista[sufix];
                        if (emitent.ToUpper() == "ANGAJATOR")
                        {
                            rbEmitent1.Checked = true;
                            rbEmitent2.Checked = false;
                        }
                        else
                        {
                            rbEmitent1.Checked = false;
                            rbEmitent2.Checked = true;
                        }
                        break;
                    case "NET":
                        int salariuNet = 0;
                        int.TryParse(lista[sufix], out salariuNet);
                        if (salariuNet == 0)
                            chkSalNet.Checked = false;
                        else
                            chkSalNet.Checked = true;
                        break;
                    case "ANU":
                        int anul = DateTime.Now.Year;
                        int.TryParse(lista[sufix], out anul);

                        List<int> listAn = GetYears();
                        table.Clear();
                        for (int i = 0; i < listAn.Count(); i++)
                            table.Rows.Add(listAn[i], listAn[i].ToString());
                        cmbAnul.DataSource = table;
                        cmbAnul.DataBind();
                        chkVenit.Text = "Angajați cu venituri în " + anul;
                        cmbAnul.Text = anul.ToString();
                        break;
                    case "MAR":
                        int cotract_marca = 0;
                        int.TryParse(lista[sufix], out cotract_marca);
                        if (cotract_marca == 0)
                        {
                            rbSumeContract1.Checked = false;
                            rbSumeContract2.Checked = true;
                        }
                        else
                        {
                            rbSumeContract1.Checked = true;
                            rbSumeContract2.Checked = false;
                        }
                        break;
                    case "CIC": txtVarstaCopil.Text = (lista[sufix] == "0" ? "2" : lista[sufix]); break;
                    case "ZLU": txtZileLucrate.Text = (lista[sufix].Length <= 0 ? "F20004030/F10043" : lista[sufix]); break;
                    case "ZCO": txtZileCO.Text = (lista[sufix].Length <= 0 ? "F20004034" : lista[sufix]); break;
                    case "ZAB":
                        if (lista[sufix].Length == 0)
                        {
                            string tranzactii = "";
                            if (lista.ContainsKey("SUSP_CO"))
                            {
                                if (lista["SUSP_CO"].ToString().Length >= 4)
                                {
                                    tranzactii += lista["SUSP_CO"].ToString();
                                }
                            }

                            int nr = 0;
                            while (tranzactii.Length > 1 && int.TryParse(tranzactii.Substring(tranzactii.Length - 1, 1), out nr) == false)
                                tranzactii = tranzactii.Substring(0, tranzactii.Length - 1);
                            tranzactii = tranzactii.Replace("+", ",");
                            tranzactii = tranzactii.Replace("-", ",");
                            tranzactii = tranzactii.Replace(".", ",");
                            tranzactii = tranzactii.Replace(";", ",");

                            if (lista[sufix].Length == 0 && tranzactii.Length >= 4)
                            {
                                Dictionary<int, int> listaAbs = GetAbsente(tranzactii);
                                tranzactii = "";
                                if (listaAbs != null && listaAbs.Count > 0)
                                {
                                    int index = 0;
                                    int i = 0;
                                    foreach (int keyAbs in listaAbs.Keys)
                                    {
                                        if (i > 0)
                                            tranzactii += "+";
                                        index = keyAbs;
                                        index = index + listaAbs[keyAbs];
                                        if (index > 99)
                                            tranzactii += ("F20004" + index);
                                        else
                                            tranzactii += ("F200040" + index);
                                        i++;
                                    }
                                }
                                txtZileAbsente.Text = tranzactii;
                            }
                        }
                        else
                            txtZileAbsente.Text = lista[sufix];
                        break;
                    case "ZCM":
                        if (lista[sufix].Length <= 0)
                        {
                            string tranzactii = "";
                            if (lista.ContainsKey("PP"))
                            {
                                if (lista["PP"].ToString().Length >= 4)
                                {
                                    tranzactii += lista["PP"].ToString();
                                    tranzactii += "+";
                                }
                            }
                            if (lista.ContainsKey("ZAMBP"))
                            {
                                if (lista["ZAMBP"].ToString().Length >= 4)
                                {
                                    tranzactii += lista["ZAMBP"].ToString();
                                    tranzactii += "+";
                                }
                            }
                            if (lista.ContainsKey("ZAMBP2"))
                            {
                                if (lista["ZAMBP2"].ToString().Length >= 4)
                                {
                                    tranzactii += lista["ZAMBP2"].ToString();
                                    tranzactii += "+";
                                }
                            }
                            int nr = 0;
                            while (tranzactii.Length > 1 && int.TryParse(tranzactii.Substring(tranzactii.Length - 1, 1), out nr) == false)
                                tranzactii = tranzactii.Substring(0, tranzactii.Length - 1);
                            tranzactii = tranzactii.Replace(",", "+");
                            tranzactii = tranzactii.Replace("-", "+");
                            tranzactii = tranzactii.Replace(".", "+");
                            tranzactii = tranzactii.Replace(";", "+");

                            if (tranzactii.Length >= 9)
                            {
                                txtZileCM.Text = tranzactii;
                            }
                        }
                        else
                            txtZileCM.Text = lista[sufix];
                        break;
                    case "VEN": txtVenit.Text = (lista[sufix].Length <= 0 ? "F20004042" : lista[sufix]); break;
                    case "TIT": txtTitlu.Text = lista[sufix]; break;
                    case "CMP": txtCompartiment.Text = lista[sufix]; break;
                    case "SUS":
                        if (lista[sufix].Length == 0)
                        {
                            string tranzactii = "";
                            if (lista.ContainsKey("SUSP_CO"))
                            {
                                if (lista["SUSP_CO"].ToString().Length >= 4)
                                {
                                    tranzactii += lista["SUSP_CO"].ToString();
                                }
                            }

                            int nr = 0;
                            while (tranzactii.Length > 1 && int.TryParse(tranzactii.Substring(tranzactii.Length - 1, 1), out nr) == false)
                                tranzactii = tranzactii.Substring(0, tranzactii.Length - 1);
                            tranzactii = tranzactii.Replace("+", ",");
                            tranzactii = tranzactii.Replace("-", ",");
                            tranzactii = tranzactii.Replace(".", ",");
                            tranzactii = tranzactii.Replace(";", ",");

                            if (lista[sufix].Length == 0)
                                txtZileSusp.Text = tranzactii.Replace(",", "+");
                        }
                        else
                            txtZileSusp.Text = lista[sufix];
                        break;
                    case "CEX":
                        if (lista[sufix].Length > 0)
                            txtCoduri.Text = lista[sufix];
                        else
                            txtCoduri.Text = "05,06";
                        break;
                    case "FIN":
                        int f = 0;
                        int.TryParse(lista[sufix], out f);
                        if (f == 0)
                        {
                            rbFunc1.Checked = true;
                            rbFunc2.Checked = false;
                        }
                        else
                        {
                            rbFunc1.Checked = false;
                            rbFunc2.Checked = true;
                        }
                        break;
                    case "NEM":
                        if (lista[sufix].Length > 0)
                            txtNEM.Text = lista[sufix];
                        else
                            txtNEM.Text = "F20004038/F10043";
                        break;
                    case "CFP":
                        if (lista[sufix].Length > 0)
                            txtCFP.Text = lista[sufix];
                        else
                            txtCFP.Text = "F20004037";
                        break;
                    case "INS":
                        int stagiu = 6;
                        int.TryParse(lista[sufix], out stagiu);
                        if (stagiu == 6)
                        {
                            rbIntervalStagiu1.Checked = true;
                            rbIntervalStagiu2.Checked = false;
                        }
                        else
                        {
                            rbIntervalStagiu1.Checked = false;
                            rbIntervalStagiu2.Checked = true;
                        }
                        break;
                    case "BCM":
                        if (lista[sufix].Length > 0)
                            txtBCM.Text = lista[sufix];
                        else
                            txtBCM.Text = "";
                        break;
                }
            if (txtNumeRL1.Text.Length > 0 || txtFunctieRL1.Text.Length > 0)
                chkRep1.Checked = true;
            if (txtNumeRL2.Text.Length > 0 || txtFunctieRL2.Text.Length > 0)
                chkRep2.Checked = true;
                       
            Session["AdevListaParam"] = lista;
          
        }

        public Dictionary<String, String> LoadParameters()
        {
            Dictionary<String, String> lista = new Dictionary<string, string>();

            string sql = "SELECT NRCRT, ETICHETA, VALOARE FROM F800_ADEVERINTE_CONFIG WHERE NRCRT > 0 ORDER BY NRCRT";
            DataTable dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    lista.Add(dtParam.Rows[i]["ETICHETA"].ToString(), dtParam.Rows[i]["VALOARE"].ToString());

            sql = "SELECT -1 AS NRCRT, F80002 AS ETICHETA, F80003 AS VALOARE FROM F800";
            dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                {
                    if (!lista.ContainsKey(dtParam.Rows[i]["ETICHETA"].ToString()))
                        lista.Add(dtParam.Rows[i]["ETICHETA"].ToString(), dtParam.Rows[i]["VALOARE"].ToString());
                    else
                        lista[dtParam.Rows[i]["ETICHETA"].ToString()] = dtParam.Rows[i]["VALOARE"].ToString();                   
                }

            //sql = "SELECT -2 AS NRCRT, \"Nume\" AS ETICHETA, \"Valoare\" AS VALOARE FROM \"tblParametrii\" WHERE \"Nume\" IN ('CT_SIND_ST', 'CT_SIND_ORG', 'BAZA_FF', 'BAZAC_IMP_PROD', 'IMPOZIT_IMP_PROD',  'SOMB', 'SOMA', 'NN', 'PP', 'ZAMBP', 'ZAMBP2', 'SUSP_CO', 'REP_CODZILEABNEM', 'REP_CODZILECFP', 'AdevDimX', 'AdevDimY')";
            sql = "SELECT -2 AS NRCRT, \"Nume\" AS ETICHETA, \"Valoare\" AS VALOARE FROM \"tblParametrii\" WHERE \"Nume\" IN ('AdevDimX', 'AdevDimY')";
            dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    if (!lista.ContainsKey(dtParam.Rows[i]["ETICHETA"].ToString()))
                    {
                        if (!lista.ContainsKey(dtParam.Rows[i]["ETICHETA"].ToString()))
                            lista.Add(dtParam.Rows[i]["ETICHETA"].ToString(), dtParam.Rows[i]["VALOARE"].ToString());
                        else
                            lista[dtParam.Rows[i]["ETICHETA"].ToString()] = dtParam.Rows[i]["VALOARE"].ToString();
                    }
            if (!lista.ContainsKey("AdevDimX"))
                lista.Add("AdevDimX", "1314450");
            if (!lista.ContainsKey("AdevDimY"))
                lista.Add("AdevDimY", "790575");
            return lista;
        }

        public List<int> GetYears()
        {
            List<int> lista = new List<int>();

            string sql = "SELECT F01011 AS YEAR FROM F010 UNION SELECT DISTINCT YEAR FROM F920 ORDER BY YEAR";
            DataTable dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    lista.Add(Convert.ToInt32(dtParam.Rows[i]["YEAR"].ToString()));

            return lista;
        }

        public Dictionary<int, int> GetAbsente(string tranzactii)
        {
            Dictionary<int, int> lista = new Dictionary<int, int>();

            string sql = "";
            if (Constante.tipBD == 2)
                sql = "SELECT DISTINCT CASE WHEN F02113 = 0 THEN INSTR(F02118, '6') ELSE F02113 END AS INDEXS, CASE WHEN F02113 = 0 THEN 59 ELSE 29 END AS PLUS FROM F021 where F02104 IN (" + tranzactii + ")";
            else
                sql = "SELECT DISTINCT CASE WHEN F02113 = 0 THEN CHARINDEX('6', F02118) ELSE F02113 END AS INDEXS, CASE WHEN F02113 = 0 THEN 59 ELSE 29 END AS PLUS FROM F021 where F02104 IN (" + tranzactii + ")";

            DataTable dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    lista.Add(Convert.ToInt32(dtParam.Rows[i]["INDEXS"].ToString()), Convert.ToInt32(dtParam.Rows[i]["PLUS"].ToString()));
            return lista;
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

        private void IncarcaGrid()
        {
            try
            {
                grDate.KeyFieldName = "F10003";

                DataTable dt = GetF100NumeComplet(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99),
                    Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), Convert.ToInt32(cmbCateg.Value ?? -99));

                grDate.DataSource = dt;
                Session["InformatiaCurenta_Adev"] = dt;
                grDate.DataBind();
                grDate.SettingsPager.PageSize = 25;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetF100NumeComplet(int idUser, int idSubcomp = -99, int idFiliala = -99, int idSectie = -99, int idDept = -99, int idSubdept = -99, int idBirou = -99, int idAngajat = -9, int idCtr = -99, int idCateg = -99)
        {
            DataTable dt = new DataTable();

            try
            {
                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";


                string strSql = @"SELECT Y.* FROM(
                                SELECT DISTINCT CAST(A.F10003 AS int) AS F10003,  A.F10008 {0} ' ' {0} A.F10009 AS ""NumeComplet"",                                  
                                A.F10002, A.F10004, A.F10005, A.F10006, A.F10007, X.F100958, X. F100959, A.F10025,
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"",
                                A.F10061, A.F10062

                                FROM ""relGrupAngajat"" B                                
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""                                
                                INNER JOIN F100 A ON b.F10003 = a.F10003   
                                INNER JOIN F1001 X ON A.F10003 = X.F10003                            
                                LEFT JOIN F718 D ON A.F10071 = D.F71802                                
                                LEFT JOIN F002 E ON A.F10002 = E.F00202                                
                                LEFT JOIN F003 F ON A.F10004 = F.F00304                                
                                LEFT JOIN F004 G ON A.F10005 = G.F00405                                
                                LEFT JOIN F005 H ON A.F10006 = H.F00506                                
                                LEFT JOIN F006 I ON A.F10007 = I.F00607                                
                                LEFT JOIN F007 K ON X.F100958 = K.F00708  
                                LEFT JOIN F008 L ON X.F100959 = L.F00809                                 
                                WHERE C.""IdSuper"" = {1}

                                UNION

                                SELECT DISTINCT CAST(A.F10003 AS int) AS F10003,  A.F10008 {0} ' ' {0} A.F10009 AS ""NumeComplet"",                                  
                                A.F10002, A.F10004, A.F10005, A.F10006, A.F10007, X.F100958, X. F100959, A.F10025  ,
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"",
                                A.F10061, A.F10062

                                FROM ""relGrupAngajat"" B                                
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""                                
                                INNER JOIN F100 A ON b.F10003 = a.F10003 
                                INNER JOIN F1001 X ON A.F10003 = X.F10003                               
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")                                
                                LEFT JOIN F718 D ON A.F10071 = D.F71802                                
                                LEFT JOIN F002 E ON A.F10002 = E.F00202                                
                                LEFT JOIN F003 F ON A.F10004 = F.F00304                                
                                LEFT JOIN F004 G ON A.F10005 = G.F00405                                
                                LEFT JOIN F005 H ON A.F10006 = H.F00506                                
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN F007 K ON X.F100958 = K.F00708  
                                LEFT JOIN F008 L ON X.F100959 = L.F00809                                  
                                WHERE J.""IdUser"" = {1}
  
                                                           
                                ) Y 
                                {2}    ";


                string tmp = "", cond = "", condCtr = "";

                if (idSubcomp != -99)
                {
                    tmp = string.Format("  Y.F10004 = {0} ", idSubcomp);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idFiliala != -99)
                {
                    tmp = string.Format("  Y.F10005 = {0} ", idFiliala);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idSectie != -99)
                {
                    tmp = string.Format("  Y.F10006 = {0} ", idSectie);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idDept != -99)
                {
                    tmp = string.Format("  Y.F10007 = {0} ", idDept);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idSubdept != -99)
                {
                    tmp = string.Format("  Y.F100958 = {0} ", idSubdept);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idBirou != -99)
                {
                    tmp = string.Format("  Y.F100959 = {0} ", idBirou);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idAngajat != -99)
                {
                    tmp = string.Format("  Y.F10003 = {0} ", idAngajat);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idCtr != -99)
                {
                    condCtr = " LEFT JOIN \"F100Contracte\" Ctr ON Ctr.F10003 = Y.F10003  ";
                    tmp = string.Format("  \"DataInceput\" <= {0} AND {0} <= \"DataSfarsit\" AND \"IdContract\" = {1}", (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idCtr);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (idCateg != -99)
                {
                    tmp = string.Format("  (Y.F10061 = {0} OR Y.F10062 = {0})", idCateg);
                    if (cond.Length <= 0)
                        cond = " WHERE " + tmp;
                    else
                        cond += " AND " + tmp;
                }

                if (cond.Length <= 0)
                    cond = " WHERE (Y.F10025 = 0 OR Y.F10025 = 999) ";
                else
                    cond += " AND (Y.F10025 = 0 OR Y.F10025 = 999) ";

                strSql += cond;

                strSql = string.Format(strSql, op, idUser, condCtr);

                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string[] param = e.Parameter.Split(';');
                Dictionary<String, String> lista = Session["AdevListaParam"] as Dictionary<String, String>;
                switch(param[0])
                {
                    case "txtNumeRL1":
                        if (!lista.ContainsKey("NR1"))
                            lista.Add("NR1", "");
                        lista["NR1"] = param[1];
                        break;
                    case "txtFunctieRL1":
                        if (!lista.ContainsKey("FR1"))
                            lista.Add("FR1", "");
                        lista["FR1"] = param[1];
                        break;
                    case "txtNumeRL2":
                        if (!lista.ContainsKey("NR2"))
                            lista.Add("NR2", "");
                        lista["NR2"] = param[1];
                        break;
                    case "txtFunctieRL2":
                        if (!lista.ContainsKey("FR2"))
                            lista.Add("FR2", "");
                        lista["FR2"] = param[1];
                        break;
                    case "rbInterval1":
                        if (!lista.ContainsKey("INT"))
                            lista.Add("INT", "");
                        lista["INT"] = param[1] == "true" ? "12" : "24";
                        break;
                    case "rbEmitent1":
                        if (!lista.ContainsKey("EMI"))
                            lista.Add("EMI", "");
                        lista["EMI"] = param[1] == "true" ? "Angajator" : "Casa de sanatate";
                        break;
                    case "txtCoduri":
                        if (!lista.ContainsKey("CEX"))
                            lista.Add("CEX", "");
                        lista["CEX"] = param[1]; ;
                        break;
                    case "chkSalNet":
                        if (!lista.ContainsKey("NET"))
                            lista.Add("NET", "");
                        lista["NET"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "cmbAnul":
                        if (!lista.ContainsKey("ANU"))
                            lista.Add("ANU", "");
                        lista["ANU"] = param[1];
                        break;
                    case "rbSumeContract1":
                        if (!lista.ContainsKey("MAR"))
                            lista.Add("MAR", "");
                        lista["MAR"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "txtVarstaCopil":
                        if (!lista.ContainsKey("CIC"))
                            lista.Add("CIC", "");
                        lista["CIC"] = param[1];
                        break;
                    case "txtZileLucrate":
                        if (!lista.ContainsKey("ZLU"))
                            lista.Add("ZLU", "");
                        lista["ZLU"] = param[1];
                        break;
                    case "txtZileCM":
                        if (!lista.ContainsKey("ZCM"))
                            lista.Add("ZCM", "");
                        lista["ZCM"] = param[1];
                        break;
                    case "txtZileAbsente":
                        if (!lista.ContainsKey("ZAB"))
                            lista.Add("ZAB", "");
                        lista["ZAB"] = param[1];
                        break;
                    case "txtZileCO":
                        if (!lista.ContainsKey("ZCO"))
                            lista.Add("ZCO", "");
                        lista["ZCO"] = param[1];
                        break;
                    case "txtVenit":
                        if (!lista.ContainsKey("VEN"))
                            lista.Add("VEN", "");
                        lista["VEN"] = param[1];
                        break;
                    case "txtTitlu":
                        if (!lista.ContainsKey("TIT"))
                            lista.Add("TIT", "");
                        lista["TIT"] = param[1];
                        break;
                    case "txtCompartiment":
                        if (!lista.ContainsKey("CMP"))
                            lista.Add("CMP", "");
                        lista["CMP"] = param[1];
                        break;
                    case "txtZileSusp":
                        if (!lista.ContainsKey("SUS"))
                            lista.Add("SUS", "");
                        lista["SUS"] = param[1];
                        break;
                    case "rbFunc1":
                        if (!lista.ContainsKey("FIN"))
                            lista.Add("FIN", "");
                        lista["FIN"] = param[1] == "true" ? "0" : "1";
                        break;
                    case "rbFunc2":
                        if (!lista.ContainsKey("FIN"))
                            lista.Add("FIN", "");
                        lista["FIN"] = param[1] == "true" ? "1" : "0";
                        break;
                    case "txtNEM":
                        if (!lista.ContainsKey("NEM"))
                            lista.Add("NEM", "");
                        lista["NEM"] = param[1];
                        break;
                    case "txtCFP":
                        if (!lista.ContainsKey("CFP"))
                            lista.Add("CFP", "");
                        lista["CFP"] = param[1];
                        break;
                    case "rbIntervalStagiu1":
                        if (!lista.ContainsKey("INS"))
                            lista.Add("INS", "");
                        lista["INS"] = param[1] == "true" ? "6" : "12";
                        break;
                    case "rbIntervalStagiu2":
                        if (!lista.ContainsKey("INS"))
                            lista.Add("INS", "");
                        lista["INS"] = param[1] == "true" ? "12" : "6";
                        break;
                    case "txtBCM":
                        if (!lista.ContainsKey("BCM"))
                            lista.Add("BCM", "");
                        lista["BCM"] = param[1];
                        break;
                    case "EmptyFields":
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                        cmbDept.DataBind();
                        return;
                }
                UpdateControls(lista);

                switch (param[0])
                {
                    case "chkActivi":
                        //DataTable dtAng = General.IncarcaDT(SelectAngajati(param[1] == "true" ? " WHERE A.F10025 = 0 " : ""), null);
                        //cmbAng.DataSource = dtAng;
                        //cmbAng.DataBind();
                        //cmbAng.SelectedIndex = -1;
                        ReloadCombo();
                        break;
                    case "chkVenit":
                        ReloadCombo();
                        break;
                    case "chkCIC":
                        ReloadCombo();
                        break;
                    case "cmbAng":
                        Session["MarcaConfigCIC"] = param[1];
                        break;
                    case "btnSalvare":
                        btnSalvare_Click();
                        break;
                    case "btnAnulare":
                        btnAnulare_Click();
                        break;
                    case "btnGenerare":
                        btnGenerare_Click();
                        break;
                    case "btnConfig":
                        btnConfigurare_Click();
                        break;
                }


                Session["AdevListaParam"] = lista;
           
                   
            }
            catch (Exception ex)
            {
                //ArataMesaj(ex.ToString());
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        WHERE B.""IdUser""= {Session["UserId"]}) B                        
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607 {filtru}";

            }
            catch (Exception ex)
            {
                //ArataMesaj("");
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        private void btnSalvare_Click()
        {
            if (chkRep1.Checked == true)
                if (txtNumeRL1.Text.Length <= 0 || txtFunctieRL1.Text.Length <= 0)
                {
                    //ArataMesaj(Dami.TraduCuvant("Date insuficiente Reprezentant Legal 1!"));
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Date insuficiente Reprezentant Legal 1!");
                    return;
                }

            if (chkRep2.Checked == true)
                if (txtNumeRL2.Text.Length <= 0 || txtFunctieRL2.Text.Length <= 0)
                {
                    //ArataMesaj(Dami.TraduCuvant("Date insuficiente Reprezentant Legal 2!"));
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Date insuficiente Reprezentant Legal 2!");
                    return;
                }

            if (cmbAnul.Text.Length <= 0)
            {
                //ArataMesaj(Dami.TraduCuvant("Nu ati selectat anul!"));
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat anul!");
                return;
            }

            Dictionary<String, String> lista = Session["AdevListaParam"] as Dictionary<String, String>;
            //lista.Add("NR1", txtNumeRL1.Text);
            //lista.Add("FR1", txtFunctieRL1.Text);
            //lista.Add("NR2", txtNumeRL2.Text);
            //lista.Add("FR2", txtFunctieRL2.Text);
            //lista.Add("INT", (bool)rbInterval1.Checked ? "12" : "24");
            //lista.Add("EMI", (bool)rbEmitent1.Checked ? "Angajator" : "Casa de sanatate");
            //lista.Add("NET", (bool)chkSalNet.Checked ? "1" : "0");
            //lista.Add("ANU", cmbAnul.Text.Length > 0 ? cmbAnul.Text : DateTime.Now.Year.ToString());
            //lista.Add("MAR", (bool)rbSumeContract1.Checked ? "1" : "0");
            //lista.Add("CIC", txtVarstaCopil.Text);
            //lista.Add("ZLU", txtZileLucrate.Text);
            //lista.Add("ZCM", txtZileCM.Text);
            //lista.Add("ZAB", txtZileAbsente.Text);
            //lista.Add("ZCO", txtZileCO.Text);
            //lista.Add("VEN", txtVenit.Text);
            //lista.Add("TIT", txtTitlu.Text);
            //lista.Add("CMP", txtCompartiment.Text);
            //lista.Add("SUS", txtZileSusp.Text);
            //lista.Add("FIN", (bool)rbFunc1.Checked ? "0" : "1");

            SalvareParam(lista);

            chkVenit.Text = "Angajați cu venituri în " + cmbAnul.Text;
            config.Visible = false;
            Session["AdevConfig"] = "false";
        }

        private void btnGenerare_Click()
        {
            try
            {
                if (cmbAng.Value == null)
                {
                    //ArataMesaj(Dami.TraduCuvant("Campul angajat nu este completat!"));
                    MessageBox.Show(Dami.TraduCuvant("Campul angajat nu este completat!"));
                    return;
                }

                if (cmbAdev.Value == null)
                {
                    //ArataMesaj(Dami.TraduCuvant("Nu ati selectat tipul de adeverinta!"));
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat tipul de adeverinta!"));
                    return;
                }




                //if (Convert.ToInt32(cmbAdev.Value) == 2)
                //{

                //    PregAdevCIC(Convert.ToInt32(cmbAng.Value));

                //    //ConfigCIC dlg = new ConfigCIC();
                //    //dlg.Closed += (s, eargs) =>
                //    //{
                //    //    if (dlg.DialogResult == true)
                //    //    {
                //    //        GenerareFisier(Convert.ToInt32(cmbAngajat.EditValue), Convert.ToInt32(cmbAdev.EditValue), Convert.ToInt32(cmbAnul.EditValue ?? DateTime.Now.Year));
                //    //    }
                //    //};
                //    //dlg.Show();


                //}
                //else
                //{
                //    GenerareFisier(Convert.ToInt32(cmbAng.Value), Convert.ToInt32(cmbAdev.Value), Convert.ToInt32(cmbAnul.Value ?? DateTime.Now.Year));
                //}


               
               GenerareFisier(Convert.ToInt32(cmbAng.Value), Convert.ToInt32(cmbAdev.Value), Convert.ToInt32(cmbAnul.Value ?? DateTime.Now.Year));
 

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void SalvareParam(Dictionary<String, String> lista)
        {
            try
            {
                string sql = "UPDATE F800_ADEVERINTE_CONFIG SET NRCRT = (-1 * NRCRT)";
                General.ExecutaNonQuery(sql, null);

                int index = 1;
                foreach (string key in lista.Keys)
                {
                    string templ = "INSERT INTO F800_ADEVERINTE_CONFIG (NRCRT, ETICHETA, VALOARE) VALUES ({0}, '{1}', '{2}')";
                    sql = string.Format(templ, index, key, lista[key]);
                    General.ExecutaNonQuery(sql, null);
                    index++;
                }

                sql = "DELETE FROM F800_ADEVERINTE_CONFIG WHERE NRCRT < 0";
                General.ExecutaNonQuery(sql, null);
            }
            catch (Exception ex)
            {
                string sql = "DELETE FROM F800_ADEVERINTE_CONFIG WHERE NRCRT > 0";
                General.ExecutaNonQuery(sql, null);

                sql = "UPDATE F800_ADEVERINTE_CONFIG SET NRCRT = (-1 * NRCRT)";
                General.ExecutaNonQuery(sql, null);

                General.MemoreazaEroarea(ex, "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                string sql = "DELETE FROM F800_ADEVERINTE_CONFIG WHERE NRCRT < 0";
                General.ExecutaNonQuery(sql, null);
            }
        }



        private void GenerareFisier(int marca, int adev, int anul)
        {
           
       
            byte[] fisierGen = GenerareAdeverinta(marca, adev, anul);
            
            if (fisierGen != null)
            {
                //ArataMesaj(Dami.TraduCuvant("Fisierul a fost generat cu success!"));

                MemoryStream stream = new MemoryStream(fisierGen);
                Response.Clear();
                MemoryStream ms = stream;
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + fisier.Split('.')[0] + ".docx");
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                Response.End();


            }
            else
            {
                //ArataMesaj(Dami.TraduCuvant("Fisierul nu a fost generat!"));
                MessageBox.Show(Dami.TraduCuvant("Fisierul nu a fost generat!"));
            }

        
            
        }

        public byte[] GenerareAdeverinta(int marca, int adev, int anul)
        {

            try
            {
                //byte[] bytes = null;

                string cnApp = Constante.cnnWeb;
                string tmp = cnApp.Split(new[] { "Data source=" }, StringSplitOptions.None)[1];
                string conn = tmp.Split(';')[0];
                tmp = cnApp.Split(new[] { "User Id=" }, StringSplitOptions.None)[1];
                string user = tmp.Split(';')[0];
                string DB = "";
                if (Constante.tipBD == 1)
                {
                    tmp = cnApp.Split(new[] { "Initial catalog=" }, StringSplitOptions.None)[1];
                    DB = tmp.Split(';')[0];
                }
                else
                    DB = user;
                tmp = cnApp.Split(new[] { "Password=" }, StringSplitOptions.None)[1];
                string pwd = tmp.Split(';')[0];

                string cale = HostingEnvironment.MapPath("~/Adeverinta");

                //Dictionary<String, String> lista = new Dictionary<string, string>();
                //string sql = "SELECT -1 AS NRCRT, \"Nume\" AS ETICHETA, \"Valoare\" AS VALOARE FROM \"tblParametrii\" WHERE \"Nume\" IN ('CT_SIND_ST', 'CT_SIND_ORG', 'BAZA_FF', 'BAZAC_IMP_PROD', 'IMPOZIT_IMP_PROD',  'SOMB', 'SOMA', 'PP', 'ZAMBP', 'ZAMBP2', 'SUSP_CO')";
                //DataTable dtParam = General.IncarcaDT(sql, null);
                //if (dtParam != null && dtParam.Rows.Count > 0)
                //    for (int i = 0; i < dtParam.Rows.Count; i++)
                //        lista.Add(dtParam.Rows[i]["ETICHETA"].ToString(), dtParam.Rows[i]["VALOARE"].ToString());

                Hashtable Config = new Hashtable();
                //foreach (string key in lista.Keys)
                //    Config.Add(key, lista[key]);

                Config.Add("DATABASE", (Constante.tipBD == 2 ? "ORACLE" : "SQLSVR"));
                Config.Add("ORACONN", conn);
                Config.Add("ORAUSER", DB);
                Config.Add("ORAPWD", pwd);
                Config.Add("ORALOGIN", user);


                string sql = "SELECT * FROM F100 WHERE F10003 = " + marca;
                DataTable dtAng = General.IncarcaDT(sql, null);

                //var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/"));
                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE"));
                if (!folder.Exists)
                    folder.Create();

                //String msg = Adeverinte.Print_Adeverinte.Print_Adeverinte_Main(1, adev, Config, folder.ToString(), marca);


                String FileName = "";
                switch (adev)
                {
                    case 0:
                        fisier = "Adev_sanatate_2019_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + marca + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        break;
                    case 1:
                        fisier = "Adev_sanatate_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + marca + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        AdeverintaSanatate(marca, FileName);
                        break;                        
                    case 2:
                        fisier = dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + marca + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/VENITURI_" + anul + "/") + fisier;
                        AdeverintaVenituriAnuale(marca, FileName);
                        break;
                    case 3:
                        fisier = "Adev_CIC_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10017"].ToString() + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        AdeverintaCIC(marca, FileName);
                        break;
                    case 4:
                        fisier = "Adev_SOMAJ_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10017"].ToString() + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        AdeverintaSomaj(marca, FileName);
                        break;
                    case 6:
                        fisier = "Adev_Stagiu_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        AdeverintaStagiu(marca, FileName);
                        break;
                    case 7:
                        fisier = "Adev_Vechime_" + dtAng.Rows[0]["F10008"].ToString().Replace(' ', '_') + "_" + dtAng.Rows[0]["F10009"].ToString().Replace(' ', '_') + ".xml";
                        FileName = HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/") + fisier;
                        AdeverintaVechime(marca, FileName);
                        break;
                }

                //if (msg.Length > 0)
                //    MessageBox.Show(msg, MessageBox.icoError);

                XDocument doc;
                doc = XDocument.Load(FileName);
                FlatToOpc(doc, FileName.Split('.')[0] + ".docx");

                File.Delete(FileName);


                return File.ReadAllBytes(FileName.Split('.')[0] + ".docx");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        private void AdeverintaSanatate(int marca, string FileName)
        {
            string sql = "";
            string luna_istoric_F940 = "";
            DataTable dtData = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
            int luna = 0; int.TryParse(dtData.Rows[0][0].ToString(), out luna);
            int an = 0; int.TryParse(dtData.Rows[0][1].ToString(), out an);

            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;


            luna_istoric_F940 = "01/" + (int.Parse(dtData.Rows[0][0].ToString()) < 10 ? "0" + dtData.Rows[0][0].ToString() : dtData.Rows[0][0].ToString()) + "/" + dtData.Rows[0][1].ToString();

            if (Constante.tipBD == 2)
            {
                luna_istoric_F940 = "TO_DATE('" + luna_istoric_F940 + "', 'DD/MM/YYYY') ";
                luna_istoric_F940 = "TO_DATE('01/'||TO_CHAR(F940.MONTH)||'/'||TO_CHAR(F940.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F940;
            }
            else
            {
                luna_istoric_F940 = "CONVERT(DATETIME, '" + luna_istoric_F940 + "', 103)";
                luna_istoric_F940 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F940.MONTH)+'/'+CONVERT(VARCHAR,F940.YEAR), 103) < " + luna_istoric_F940;
            }

            DataTable dtCompany = new DataTable();
            DataTable dtEmpl = new DataTable();
            DataTable dtCM = new DataTable();
            DataTable dtLC = new DataTable();
            DataTable dtCoasig = new DataTable();

            double interval = 0;
            if (lista["INT"] == "24")
                interval = 23.9;
            else
                interval = 11.9;

            DataTable dtCoAdress = new DataTable();
            string coaddress = "SELECT F00204, F00233, F00234, F00238, F00232, F00281, F00282, F00283, F00205, F00207 FROM F002, F100 WHERE F10002 = F00202 AND F10003 = " + marca;
            dtCoAdress = General.IncarcaDT(coaddress, null);
            coaddress = "";
            coaddress = dtCoAdress.Rows[0]["F00232"].ToString() +
                (dtCoAdress.Rows[0]["F00233"].ToString().Length == 0 ? "" : ", Str. " + dtCoAdress.Rows[0]["F00233"].ToString())
                + ((dtCoAdress.Rows[0]["F00234"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00234"].ToString() == "0") ? "" : ", Nr. " + dtCoAdress.Rows[0]["F00234"].ToString())
                + ((dtCoAdress.Rows[0]["F00238"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00238"].ToString() == "0") ? "" : ", Judet " + dtCoAdress.Rows[0]["F00238"].ToString())
                ;

            string filtru_coduri = "";
            if (lista.ContainsKey("CEX") && lista["CEX"].Length > 0)
            {
                string coduri = lista["CEX"].Trim(' ');
                string[] separators = { ",", ".", "_", "-", ";", ":", "+" };
                string[] words = coduri.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    int n = 0;
                    if (int.TryParse(word, out n))
                    {
                        if (Constante.tipBD == 2)
                        {
                            filtru_coduri += " AND TO_NUMBER(F940607) <> ";
                        }
                        else
                        {
                            filtru_coduri += " AND CONVERT(INT, F940607) <> ";
                        }

                        filtru_coduri += n;
                    }
                }
            }

            if (Constante.tipBD == 2)
            {
                dtCompany = General.IncarcaDT("SELECT TRIM(f00204) AS NUME,  TRIM(f06304) AS CAS_ANG from f002, f063 where f06302 = f00284", null);

                dtEmpl = General.IncarcaDT("SELECT F10003 AS MARCA, trim(F10008)||' '||trim(F10009) AS NUME, "
                    + "TRIM(F10017) AS CNP, "
                    + "F10047 AS SEX, "
                    + "TO_CHAR(F10022, 'dd/MM/yyyy') AS EMPL_DATANG, "
                    + "TO_CHAR(F100993, 'dd/MM/yyyy') AS EMPL_DATASF, "
                    + "F10025, "
                    + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                    + "TRIM(F10052) AS SN_AI, "
                    + "TRIM(F100521) AS ELIB_AI, "
                    + "TO_CHAR(F100522, 'dd/mm/yyyy') AS DATA_AI, "
                    + "case when F10081 is null then "
                    + "case when F100907 is null then '' else "
                    + "'Comuna ' || TRIM(F100907) || ', Sat ' || TRIM(F100908) end else TRIM(F10081) end AS DOMICILIU, "
                    + "case when trim(F10083) is null or trim(F10083) = '0' or LENGTH(TRIM(F10083)) = 0 then '-' else  trim(F10083) end  AS STR, "
                    + "case when trim(F10084) is null or trim(F10084) = '0' or LENGTH(TRIM(F10084)) = 0 then '-' else  trim(F10084) end  AS NR, "
                    + "case when trim(F10085) is null or  trim(F10085) = '0' or LENGTH(TRIM(F10085)) = 0 then '-' else  trim(F10085) end  AS BL, "
                    + "case when trim(F10086) is null or trim(F10086) = '0' or LENGTH(TRIM(F10086)) = 0 then '-' else  trim(F10086) end  AS AP, "
                    + "trim(F100891) AS JUDET, "
                    + "trim(F10082) AS SECTOR "
                    + " FROM F100 WHERE F10003=" + marca, null);

                //sql = "select distinct diff, f940607, f300607, DATA from ("
                //        + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                //        + "(select sum(diff) as diff, f940607, NULL AS DATA "
                //        + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                //        + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))<" + interval.ToString(new CultureInfo("en-US"))
                //        + " AND TO_NUMBER(f940607) <> 5 AND TO_NUMBER(f940607) <> 6 "
                //        + " AND " + luna_istoric_F940
                //        + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                //        + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                //        + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449 AND TO_NUMBER(f300607) <> 5 AND TO_NUMBER(f300607) <> 6 GROUP BY F300.F300601, F300.F300602, "
                //        + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b  "
                //        + "where f940607 = f300607(+) "
                //        + "union all "
                //        + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                //        + "(select sum(diff) as diff, f940607, NULL AS DATA "
                //        + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                //        + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))< " + interval.ToString(new CultureInfo("en-US"))
                //        + " AND TO_NUMBER(f940607) <> 5 AND TO_NUMBER(f940607) <> 6 "
                //        + " AND " + luna_istoric_F940
                //        + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                //        + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                //        + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449 AND TO_NUMBER(f300607) <> 5 AND TO_NUMBER(f300607) <> 6 GROUP BY F300.F300601, F300.F300602, "
                //        + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b "
                //        + "where f940607(+) = f300607) c";

                sql = "select distinct diff, f940607, f300607, DATA from ("
                    + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                    + "(select sum(diff) as diff, f940607, NULL AS DATA "
                    + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                    + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))<" + interval.ToString(new CultureInfo("en-US"))
                    + filtru_coduri
                    + " AND " + luna_istoric_F940
                    + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                    + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                    + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449"
                    + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, "
                    + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b  "
                    + "where f940607 = f300607(+) "
                    + "union all "
                    + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                    + "(select sum(diff) as diff, f940607, NULL AS DATA "
                    + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                    + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))< " + interval.ToString(new CultureInfo("en-US"))
                    + filtru_coduri
                    + " AND " + luna_istoric_F940
                    + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                    + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                    + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449"
                    + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, "
                    + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b "
                    + "where f940607(+) = f300607) c";

                string sqlCoasig = "SELECT TRIM(F11010)||' '||TRIM(F11005) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + marca;


                dtCM = General.IncarcaDT(sql, null);

                dtLC = General.IncarcaDT("SELECT '01/'||LPAD(F01012, 2, '0')||'/'||F01011 AS LC from F010", null);

                dtCoasig = General.IncarcaDT(sqlCoasig, null);

            }
            else
            {
                dtCompany = General.IncarcaDT("SELECT LTRIM(RTRIM(f00204)) AS NUME,  LTRIM(RTRIM(f06304)) AS CAS_ANG from f002, f063 where f06302 = f00284", null);

                dtEmpl = General.IncarcaDT("SELECT F10003 AS MARCA, CAST(LTRIM(RTRIM(F10008)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F10009)) AS VARCHAR(256)) AS NUME, "
                    + "LTRIM(RTRIM(F10017)) AS CNP, "
                    + "F10047 AS SEX, "
                    + "CONVERT(VARCHAR, F10022, 103) AS EMPL_DATANG, "
                    + "CONVERT(VARCHAR,F100993, 103) AS EMPL_DATASF, "
                    + "F10025, "
                    + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                    + "LTRIM(RTRIM(F10052)) AS SN_AI, "
                    + "LTRIM(RTRIM(F100521)) AS ELIB_AI, "
                    + "CONVERT(VARCHAR, F100522, 103) AS DATA_AI, "
                    + "case when len(LTRIM(RTRIM(F10081))) = 0 then "
                    + "case when len(LTRIM(RTRIM(F100907))) = 0 then '' else "
                    + "'Comuna ' + LTRIM(RTRIM(F100907)) + ', Sat ' + LTRIM(RTRIM(F100908)) end else LTRIM(RTRIM(F10081)) end AS DOMICILIU, "

                    + "case when LTRIM(RTRIM(F10083)) is null or LTRIM(RTRIM(F10083)) = '0' or LEN(LTRIM(RTRIM(F10083))) = 0 then '-' else  LTRIM(RTRIM(F10083)) end  AS STR, "
                    + "case when LTRIM(RTRIM(F10084)) is null or LTRIM(RTRIM(F10084)) = '0' or LEN(LTRIM(RTRIM(F10084))) = 0 then '-' else  LTRIM(RTRIM(F10084)) end  AS NR, "
                    + "case when LTRIM(RTRIM(F10085)) is null or LTRIM(RTRIM(F10085)) = '0' or LEN(LTRIM(RTRIM(F10085))) = 0 then '-' else  LTRIM(RTRIM(F10085)) end  AS BL, "
                    + "case when LTRIM(RTRIM(F10086)) is null or LTRIM(RTRIM(F10086)) = '0' or LEN(LTRIM(RTRIM(F10086))) = 0 then '-' else  LTRIM(RTRIM(F10086)) end  AS AP, "

                    + "LTRIM(RTRIM(F100891)) AS JUDET, "
                    + "LTRIM(RTRIM(F10082)) AS SECTOR "
                    + "FROM F100 WHERE F10003=" + marca, null);


                //sql = "select (ISNULL(a.diff, 0) + ISNULL(b. diff, 0)) as diff, f940607, f300607, b.DATA from (select sum(diff) as diff, f940607, NULL AS DATA from (SELECT ISNULL(DATEDIFF(DAY, Max(f94037), Min(F94038))+1, 0) "
                //    + "as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 AND F940.F94010<4449 AND "
                //    + "(DATEDIFF(DAY, CONVERT(DATETIME, '01/'+CONVERT(VARCHAR, MONTH)+'/'+CONVERT(VARCHAR, YEAR), 103), "
                //    + "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F01012)+'/'+CONVERT(VARCHAR,F01011), 103) )/30.436875E) "
                //    + "<" + interval.ToString(new CultureInfo("en-US"))
                //    + " AND convert(int, F940607) <> 5 AND convert(int, F940607) <> 6 "
                //    + " AND " + luna_istoric_F940
                //    + " GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) t1 "
                //    + "group by f940607 ) a "
                //    + "full outer join	"
                //    + "( select sum(diff) as diff, f300607, CONVERT(VARCHAR, MAX(F30038), 103) AS DATA from  (SELECT ISNULL(DATEDIFF(DAY, Max(F30037), Min(F30038))+1, 0) AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca
                //    + " AND F300.F30010>=4401 "
                //    + "AND F300.F30010<4449 AND convert(int, F300607) <> 5 AND convert(int, F300607) <> 6 GROUP BY F300.F300601, F300.F300602, F30036, F30037, F30038, F300607) t2 group by F300607 ) b  on f940607 = f300607 order by f300607";

                sql = "select (ISNULL(a.diff, 0) + ISNULL(b. diff, 0)) as diff, f940607, f300607, b.DATA from (select sum(diff) as diff, f940607, NULL AS DATA from (SELECT ISNULL(DATEDIFF(DAY, Max(f94037), Min(F94038))+1, 0) "
                    + "as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 AND F940.F94010<4449 AND "
                    + "(DATEDIFF(DAY, CONVERT(DATETIME, '01/'+CONVERT(VARCHAR, MONTH)+'/'+CONVERT(VARCHAR, YEAR), 103), "
                    + "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F01012)+'/'+CONVERT(VARCHAR,F01011), 103) )/30.436875E) "
                    + "<" + interval.ToString(new CultureInfo("en-US"))
                    + filtru_coduri
                    + " AND " + luna_istoric_F940
                    + " GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) t1 "
                    + "group by f940607 ) a "
                    + "full outer join	"
                    + "( select sum(diff) as diff, f300607, CONVERT(VARCHAR, MAX(F30038), 103) AS DATA from  (SELECT ISNULL(DATEDIFF(DAY, Max(F30037), Min(F30038))+1, 0) AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca
                    + " AND F300.F30010>=4401 "
                    + "AND F300.F30010<4449"
                    + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, F30036, F30037, F30038, F300607) t2 group by F300607 ) b  on f940607 = f300607 order by f300607";


                string sqlCoasig = "SELECT CAST(LTRIM(RTRIM(F11010)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F11005)) AS VARCHAR(256)) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + marca;

                dtCoasig = General.IncarcaDT(sqlCoasig, null);

                dtCM = General.IncarcaDT(sql, null);

                dtLC = General.IncarcaDT("SELECT '01/'+CONVERT(VARCHAR, RIGHT(REPLICATE('0',2)+CAST(f01012 AS VARCHAR(2)),2))+'/'+CONVERT(VARCHAR, F01011) AS LC from F010", null);
            }

            string XMLFile = "";
            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_sanatate_sablon.xml")))
            {
                String line = sr.ReadLine();

                while (line != null)
                {
                    XMLFile += line;
                    line = sr.ReadLine();
                }
            }

            XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCoAdress.Rows[0]["F00204"].ToString());
            XMLFile = XMLFile.Replace("[COD_FISCAL]", dtCoAdress.Rows[0]["F00207"].ToString());
            XMLFile = XMLFile.Replace("[COD_CAEN]", dtCoAdress.Rows[0]["F00205"].ToString());
            XMLFile = XMLFile.Replace("[ADRESA_ANGAJATOR]", coaddress);
            XMLFile = XMLFile.Replace("[TELEFON]", dtCoAdress.Rows[0]["F00281"].ToString());
            XMLFile = XMLFile.Replace("[FAX]", dtCoAdress.Rows[0]["F00282"].ToString());
            XMLFile = XMLFile.Replace("[E_MAIL_INTERNET]", dtCoAdress.Rows[0]["F00283"].ToString());

            //XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", (lista["EMI"] == "ANGAJATOR" ? dtCompany.Rows[0]["NUME"].ToString() : dtCompany.Rows[0]["CAS_ANG"].ToString()));
            XMLFile = XMLFile.Replace("[SEX]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "doamna" : "domnul");
            XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["NUME"].ToString());
            XMLFile = XMLFile.Replace("[MARCA]", dtEmpl.Rows[0]["MARCA"].ToString());
            XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["CNP"].ToString());
            XMLFile = XMLFile.Replace("[AI]", dtEmpl.Rows[0]["AI"].ToString());
            XMLFile = XMLFile.Replace("[AI_SERIA_NR]", dtEmpl.Rows[0]["SN_AI"].ToString());
            XMLFile = XMLFile.Replace("[CI_ELIB_DE]", dtEmpl.Rows[0]["ELIB_AI"].ToString());
            XMLFile = XMLFile.Replace("[CI_DATA_ELIB]", dtEmpl.Rows[0]["DATA_AI"].ToString());
            XMLFile = XMLFile.Replace("[LOCALIT_ANGAJAT]", dtEmpl.Rows[0]["DOMICILIU"].ToString());
            XMLFile = XMLFile.Replace("[ARTERA_ANGAJAT]", dtEmpl.Rows[0]["STR"].ToString());
            XMLFile = XMLFile.Replace("[NUMAR_ANGAJAT]", dtEmpl.Rows[0]["NR"].ToString());
            XMLFile = XMLFile.Replace("[BLOC_ANGAJAT]", dtEmpl.Rows[0]["BL"].ToString());
            XMLFile = XMLFile.Replace("[APART_ANGAJAT]", dtEmpl.Rows[0]["AP"].ToString());
            XMLFile = XMLFile.Replace("[SECTOR_JUDET_ANGAJAT]", (dtEmpl.Rows[0]["JUDET"].ToString().ToUpper().Contains("BUCURESTI") ? dtEmpl.Rows[0]["SECTOR"].ToString() : dtEmpl.Rows[0]["JUDET"].ToString()));
            XMLFile = XMLFile.Replace("[SEX_ANGAJARE]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? ", angajată" : ", angajat");
            XMLFile = XMLFile.Replace("[DATA_ANGAJARII]", dtEmpl.Rows[0]["EMPL_DATANG"].ToString());

            DateTime dataAng = new DateTime(Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(6, 4)),
             Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(0, 2)));
            DateTime dataLC = new DateTime(Convert.ToInt32(dtLC.Rows[0]["LC"].ToString().Substring(6, 4)),
                             Convert.ToInt32(dtLC.Rows[0]["LC"].ToString().Substring(3, 2)), Convert.ToInt32(dtLC.Rows[0]["LC"].ToString().Substring(0, 2)));
            TimeSpan ts = dataLC - dataAng;
            int interv = 24;
            if (lista["INT"] == "12")
                interv = 12;
            XMLFile = XMLFile.Replace("[START_CM]", ts.Days / 30.436875 < interv ? "din data de " : "în ultimele ");
            XMLFile = XMLFile.Replace("[INTERVAL_CM]", ts.Days / 30.436875 < interv ? dtEmpl.Rows[0]["EMPL_DATANG"].ToString() : interv.ToString() + " luni");

            DateTime dataUCM = new DateTime(1900, 1, 1);
            if (dtCM != null && dtCM.Rows.Count > 0 && dtCM.Rows[0][0].ToString() != String.Empty)
            {
                int total = 0;
                for (int x = 0; x < dtCM.Rows.Count; x++)
                {
                    total += int.Parse(dtCM.Rows[x]["diff"].ToString());
                    DateTime tmpCM = new DateTime(1900, 1, 1);
                    if (dtCM.Rows[x]["Data"].ToString().Length >= 10)
                        tmpCM = new DateTime(Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(6, 4)),
                                                Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(3, 2)),
                                                Convert.ToInt32(dtCM.Rows[x]["Data"].ToString().Substring(0, 2)));
                    if (tmpCM > dataUCM)
                        dataUCM = tmpCM;
                }
                XMLFile = XMLFile.Replace("[NR_ZILE_CM]", total.ToString());
            }
            else
                XMLFile = XMLFile.Replace("[NR_ZILE_CM]", "0");


            XMLFile = XMLFile.Replace("[LIMITA_CM]", dataUCM > new DateTime(1900, 1, 1) ? dataUCM.Day.ToString().PadLeft(2, '0') + "/" + dataUCM.Month.ToString().PadLeft(2, '0') + "/" + dataUCM.Year.ToString()
                                            : dtLC.Rows[0]["LC"].ToString());

            string tableRow = "<w:tr w:rsidR=\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidTr=\"00CE091E\"><w:tc><w:tcPr><w:tcW w:w=\"2400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/>"
                        + "</w:tcPr><w:p w:rsidR =\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:rPr>"
                        + "<w:rFonts w:ascii =\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                        + "</w:rPr><w:t>[COL1]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w =\"5000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>"
                        + "<w:p w:rsidR =\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                        + "</w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii =\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL2]</w:t></w:r></w:p></w:tc></w:tr>";
            string table = "";
            string val1 = "————", val2 = "————";


            if (dtCM != null && dtCM.Rows.Count > 0 && dtCM.Rows[0][0].ToString() != String.Empty)
            {
                for (int x = 0; x < dtCM.Rows.Count; x++)
                {
                    table += tableRow;
                    if (dtCM.Rows[x]["f940607"].ToString() != String.Empty)
                    {
                        val1 = dtCM.Rows[x]["f940607"].ToString();
                        val2 = dtCM.Rows[x]["diff"].ToString();
                    }
                    if (dtCM.Rows[x]["f300607"].ToString() != String.Empty)
                    {
                        val1 = dtCM.Rows[x]["f300607"].ToString();
                        val2 = dtCM.Rows[x]["diff"].ToString();
                    }
                    table = table.Replace("[COL1]", val1);
                    table = table.Replace("[COL2]", val2);
                }
            }
            else
            {
                table = tableRow;
                table = table.Replace("[COL1]", val1);
                table = table.Replace("[COL2]", val2);
            }


            XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);


            string tableCoasig = "<w:p w:rsidR=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:jc w:val=\"both\"/><w:rPr>" 
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"24\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"24\"/></w:rPr><w:tab/><w:t>Listă persoane coasigurate:</w:t></w:r></w:p>"
                + "<w:tbl><w:tblPr><w:tblW w:w=\"0\" w:type=\"auto\"/><w:tblBorders><w:top w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/><w:left w:val=\"single\"" 
                + " w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/><w:bottom w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/><w:right w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/>" 
                + "<w:insideH w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/><w:insideV w:val=\"single\" w:sz=\"4\" w:space=\"0\" w:color=\"auto\"/></w:tblBorders><w:tblLayout w:type=\"fixed\"/>" 
                + "<w:tblLook w:val=\"0000\" w:firstRow=\"0\" w:lastRow=\"0\" w:firstColumn=\"0\" w:lastColumn=\"0\" w:noHBand=\"0\" w:noVBand=\"0\"/></w:tblPr><w:tblGrid><w:gridCol w:w=\"2400\"/>" 
                + "<w:gridCol w:w=\"5000\"/></w:tblGrid><w:tr w:rsidR=\"00CE091E\" w:rsidTr=\"00CE091E\"><w:tc><w:tcPr><w:tcW w:w=\"2400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/>" 
                + "</w:tcPr><w:p w:rsidR=\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:jc w:val=\"center\"/><w:rPr>" 
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr><w:r w:rsidRPr=\"00CE091E\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>Nume și prenume</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"5000\" w:type=\"dxa\"/>" 
                + "<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/>" 
                + "<w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr><w:r w:rsidRPr=\"00CE091E\"><w:rPr>" 
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>CNP</w:t></w:r></w:p></w:tc></w:tr>[TABLE_COASIG_ROWS]</w:tbl>";

            string tblTemp = "";

            string tableCoasigRow = "<w:tr w:rsidR=\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidTr=\"00CE091E\"><w:tc><w:tcPr><w:tcW w:w=\"2400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/>"
                + "</w:tcPr><w:p w:rsidR =\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:rPr>"
                + "<w:rFonts w:ascii =\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                + "</w:rPr><w:t>[COL_COASIG1]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w =\"5000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>"
                + "<w:p w:rsidR =\"00CE091E\" w:rsidRPr=\"00CE091E\" w:rsidRDefault=\"00CE091E\" w:rsidP=\"00CE091E\"><w:pPr><w:spacing w:before=\"160\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                + "</w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii =\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL_COASIG2]</w:t></w:r></w:p></w:tc></w:tr>";

            if (dtCoasig != null && dtCoasig.Rows.Count > 0)
            {
                for (int x = 0; x < dtCoasig.Rows.Count; x++)
                {
                    tblTemp += tableCoasigRow;
                    tblTemp = tblTemp.Replace("[COL_COASIG1]", dtCoasig.Rows[x]["NUME_COASIG"].ToString());
                    tblTemp = tblTemp.Replace("[COL_COASIG2]", dtCoasig.Rows[x]["CNP_COASIG"].ToString());

                }
                tableCoasig = tableCoasig.Replace("[TABLE_COASIG_ROWS]", tblTemp);
                XMLFile = XMLFile.Replace("[TABLE_COASIG]", tableCoasig);
            }
            else
                XMLFile = XMLFile.Replace("[TABLE_COASIG]", "");


            XMLFile = XMLFile.Replace("[FUNCTIE_RL1]", lista["FR1"]);
            XMLFile = XMLFile.Replace("[FUNCTIE_RL2]", lista["FR2"]);
            XMLFile = XMLFile.Replace("[NUME_RL1]", lista["NR1"]);
            XMLFile = XMLFile.Replace("[NUME_RL2]", lista["NR2"]);

            XMLFile = XMLFile.Replace("[DIMX]", lista["AdevDimX"]);
            XMLFile = XMLFile.Replace("[DIMY]", lista["AdevDimY"]);


            DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/"));
            FileInfo[] listfiles = root.GetFiles("Logo.*");
            string logo = "";
            if (listfiles.Length > 0)
            {
                byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                logo = Convert.ToBase64String(fileBytes);
            }
            XMLFile = XMLFile.Replace("[LOGO]", logo);

            
            listfiles = root.GetFiles("Subsol.*");
            string subsol = "";
            if (listfiles.Length > 0)
            {
                byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                subsol = Convert.ToBase64String(fileBytes);
            }
            XMLFile = XMLFile.Replace("[SUBSOL]", subsol);

            StreamWriter sw = new StreamWriter(FileName, false);

            sw.Write(XMLFile);
            sw.Close();
            sw.Dispose();
        }



        private void AdeverintaVenituriAnuale(int marca, string FileName)
        {


            DataTable dtCoAdress = new DataTable();
            string coaddress = "";
            DataTable dtEmpl = new DataTable();
            string sql = "";
            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;
            string cnp_contract = "";
            string tip_ctr = "97";
            if (lista["MAR"] == "0")
            {
                DataTable dtCNP = General.IncarcaDT("SELECT F10017 FROM F100 WHERE F10003 = " + marca, null);
                cnp_contract = dtCNP.Rows[0][0].ToString();
                dtCoAdress = General.IncarcaDT("SELECT F100984 AS TIPCTR from F100 WHERE F10017 = '" + cnp_contract + "'", null);
                for (int i = 0; i < dtCoAdress.Rows.Count; i++)
                {
                    if (dtCoAdress.Rows[i]["TIPCTR"].ToString() != "97" && dtCoAdress.Rows[i]["TIPCTR"].ToString() != "99")
                        tip_ctr = dtCoAdress.Rows[i]["TIPCTR"].ToString();
                }
                dtCoAdress = new DataTable();

            }

            coaddress = "SELECT F00204, F00233, F00234, F00235, F00239, F00236, F00231, F00232, F00238, F00237, F00281, F00282, F00241, F00242, F00243, F00207 FROM F002, F100 WHERE F10002 = F00202 AND F10003 = " + marca;

            if (Constante.tipBD == 2)
            {
                sql = "SELECT F10008, F10009, F10052, F100521, TO_CHAR(F100522, 'dd/mm/yyyy') AS F100522, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, F10047 AS SEX, F100984 AS TIPCTR FROM F100 WHERE F10003=" + marca;
            }
            else
            {
                sql = "SELECT F10008, F10009, F10052, F100521, CONVERT(VARCHAR, F100522, 103) AS F100522, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, F10047 AS SEX, F100984 AS TIPCTR FROM F100 WHERE F10003=" + marca;
            }
            dtCoAdress = General.IncarcaDT(coaddress, null);
            dtEmpl = General.IncarcaDT(sql, null);

            coaddress = "";
            coaddress = (dtCoAdress.Rows[0]["F00233"].ToString().Length == 0 ? "" : "Str. " + dtCoAdress.Rows[0]["F00233"].ToString())
                + ((dtCoAdress.Rows[0]["F00234"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00234"].ToString() == "0") ? "" : ", Nr. " + dtCoAdress.Rows[0]["F00234"].ToString())
                + ((dtCoAdress.Rows[0]["F00235"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00235"].ToString() == "0") ? "" : ", Bl. " + dtCoAdress.Rows[0]["F00235"].ToString())
                + ((dtCoAdress.Rows[0]["F00239"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00239"].ToString() == "0") ? "" : ", Sc. " + dtCoAdress.Rows[0]["F00239"].ToString())
                + ((dtCoAdress.Rows[0]["F00236"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00236"].ToString() == "0") ? "" : ", Ap. " + dtCoAdress.Rows[0]["F00236"].ToString())
                ;


            string XMLFile = "";
            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_venituri_sablon.xml")))
            {
                String line = sr.ReadLine();

                while (line != null)
                {
                    XMLFile += line;
                    line = sr.ReadLine();
                }
            }

            XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCoAdress.Rows[0]["F00204"].ToString());
            XMLFile = XMLFile.Replace("[ADRESA_ANGAJATOR]", coaddress);
            XMLFile = XMLFile.Replace("[LOCALITATE_ANGAJATOR]", dtCoAdress.Rows[0]["F00231"].ToString());
            XMLFile = XMLFile.Replace("[JUDET/SECTOR]", (dtCoAdress.Rows[0]["F00238"].ToString().ToUpper().Contains("BUCURESTI") ? "SECTOR " : dtCoAdress.Rows[0]["F00238"].ToString() + " ")
                + (dtCoAdress.Rows[0]["F00232"].ToString().Length == 0 ? (dtCoAdress.Rows[0]["F00238"].ToString().ToUpper().Contains("BUCURESTI") ? "-" : "") : dtCoAdress.Rows[0]["F00232"].ToString()));
            XMLFile = XMLFile.Replace("[COD_POSTAL]", dtCoAdress.Rows[0]["F00237"].ToString());
            XMLFile = XMLFile.Replace("[TELEFON]", dtCoAdress.Rows[0]["F00281"].ToString());
            XMLFile = XMLFile.Replace("[FAX]", dtCoAdress.Rows[0]["F00282"].ToString());
            XMLFile = XMLFile.Replace("[NR_INREG]", dtCoAdress.Rows[0]["F00241"].ToString() + "/" + dtCoAdress.Rows[0]["F00242"].ToString() + "/" + dtCoAdress.Rows[0]["F00243"].ToString());
            XMLFile = XMLFile.Replace("[COD_UNIC]", dtCoAdress.Rows[0]["F00207"].ToString());

            XMLFile = XMLFile.Replace("[ANUL]", lista["ANU"]);

            XMLFile = XMLFile.Replace("[SEX_ANGAJAT]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "doamna" : "domnul");
            XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["F10008"].ToString() + " " + dtEmpl.Rows[0]["F10009"].ToString());

            string actIdentitate = "";
            if (dtEmpl.Rows[0]["F10052"].ToString().Length > 0)
                if (dtEmpl.Rows[0]["SEX"].ToString() == "2")
                {
                    actIdentitate += ", legitimată cu BI/CI seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString();
                }
                else
                {
                    actIdentitate += ", legitimat cu seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString();
                }
            if (dtEmpl.Rows[0]["F100521"].ToString().Length > 0)
                actIdentitate += ", eliberat de " + dtEmpl.Rows[0]["F100521"].ToString();
            if (dtEmpl.Rows[0]["F100522"].ToString().Length > 0 && dtEmpl.Rows[0]["F100522"].ToString() != "1/1/1900" && dtEmpl.Rows[0]["F100522"].ToString() != "01/01/1900")
                actIdentitate += ", la data de " + dtEmpl.Rows[0]["F100522"].ToString();

            XMLFile = XMLFile.Replace("[ACT_IDENTITATE]", actIdentitate);

            string CNP = "         ";
            if (dtEmpl.Rows[0]["F10017"].ToString().Length > 0)
                CNP = dtEmpl.Rows[0]["F10017"].ToString();

            XMLFile = XMLFile.Replace("[CNP]", CNP);

            string text2 = "", text3 = "";
            string sector = dtEmpl.Rows[0]["F10082"].ToString().Length > 0 ? dtEmpl.Rows[0]["F10082"].ToString() : "";

            text2 = ((dtEmpl.Rows[0]["F10083"].ToString().Length == 0 || dtEmpl.Rows[0]["F10083"].ToString() == "0") ? "" : " str. " + dtEmpl.Rows[0]["F10083"].ToString());
            text2 += ((dtEmpl.Rows[0]["F10084"].ToString().Length == 0 || dtEmpl.Rows[0]["F10084"].ToString() == "0") ? "" : ", nr. " + dtEmpl.Rows[0]["F10084"].ToString());
            text2 += ((dtEmpl.Rows[0]["F10085"].ToString().Length == 0 || dtEmpl.Rows[0]["F10085"].ToString() == "0") ? "" : ", bl. " + dtEmpl.Rows[0]["F10085"].ToString());
            text2 += ((dtEmpl.Rows[0]["F100892"].ToString().Length == 0 || dtEmpl.Rows[0]["F100892"].ToString() == "0") ? "" : ", sc. " + dtEmpl.Rows[0]["F100892"].ToString());
            text2 += ((dtEmpl.Rows[0]["F100893"].ToString().Length == 0 || dtEmpl.Rows[0]["F100893"].ToString() == "0") ? "" : ", et. " + dtEmpl.Rows[0]["F100893"].ToString());
            text2 += ((dtEmpl.Rows[0]["F10086"].ToString().Length == 0 || dtEmpl.Rows[0]["F10086"].ToString() == "0") ? "" : ", ap. " + dtEmpl.Rows[0]["F10086"].ToString());
            text3 = ((dtEmpl.Rows[0]["F10087"].ToString().Length == 0 || dtEmpl.Rows[0]["F10087"].ToString() == "0") ? "" : ", cod poștal " + dtEmpl.Rows[0]["F10087"].ToString());
            //text3 += ", a realizat în anul " + cmbAnul.Text + " următoarele venituri: ";
            string loc_angajat = dtEmpl.Rows[0]["F10081"].ToString().Length == 0 ? "" : dtEmpl.Rows[0]["F10081"].ToString();
            string jud_angajat = dtEmpl.Rows[0]["F100891"].ToString().Length == 0 ? "" : dtEmpl.Rows[0]["F100891"].ToString();

            sql = "SELECT DENLOC FROM LOCALITATI, F100 WHERE F100921 = SIRUTA AND NIV = 1 AND F10003=" + marca;
            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sql, null);

            string judet_sector = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["DENLOC"].ToString().Length > 0)
                    judet_sector = ", județ/sector " + dt.Rows[0]["DENLOC"].ToString().Trim() + " " + sector;// + text3;
                else
                    if (jud_angajat.Length > 0 || sector.Length > 0)
                    judet_sector = ", județ/sector " + jud_angajat + " " + sector;// +text3;
            }
            else
                if (jud_angajat.Length > 0 || sector.Length > 0)
                judet_sector = ", județ/sector " + jud_angajat + " " + sector;// +text3;

            sql = "SELECT DENLOC FROM LOCALITATI, F100 WHERE F100897 = SIRUTA AND NIV = 3 AND F10003=" + marca;
            dt = new DataTable();
            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["DENLOC"].ToString().Length > 0)
                    text2 = " localitatea " + dt.Rows[0]["DENLOC"].ToString().Trim() + "," + text2;
                else
                    if (loc_angajat.Length > 0)
                    text2 = " localitatea " + loc_angajat + "," + text2;
            }
            else
                if (loc_angajat.Length > 0)
                text2 = " localitatea " + loc_angajat + "," + text2;

            if (!text2.ToUpper().Contains("SECTOR"))
                text3 = judet_sector + text3;

            XMLFile = XMLFile.Replace("[ADRESA_ANGAJAT]", text2 + text3);


            string nr_col = "<w:gridCol w:w=\"1400\"/>";
            string baseTable = "<w:tr w:rsidR=\"007B07D7\" w:rsidTr=\"007B07D7\"><w:trPr><w:trHeight w:val=\"400\"/></w:trPr><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\""
                + " w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr>"
                + "<w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>Luna</w:t></w:r></w:p></w:tc><w:tc><w:tcPr>"
                + "<w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\">"
                + "<w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr><w:proofErr "
                + "w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>Venit</w:t></w:r>"
                + "<w:proofErr w:type=\"spellEnd\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t xml:space=\"preserve\"> "
                + "brut</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" "
                + "w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                + "<w:b/></w:rPr></w:pPr><w:proofErr w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr>"
                + "<w:t>[COL3_ANTET]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/>	</w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>"
                + "<w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" "
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr><w:proofErr w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" "
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>[COL4_ANTET]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/></w:p></w:tc>[COLOANA5_ANTET][COLOANA6_ANTET][COLOANA7_ANTET]</w:tr> ";

            string col5 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\""
                + " w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr>"
                + "</w:pPr><w:proofErr w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr>"
                + "<w:t>[COL5_ANTET]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/></w:p></w:tc>";
            string col6 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\""
                + " w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr>"
                + "<w:proofErr w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr>"
                + "<w:t>[COL6_ANTET]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/></w:p></w:tc>";
            string col7 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\""
                + " w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr>"
                + "<w:proofErr w:type=\"spellStart\"/><w:r w:rsidRPr=\"007B07D7\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/>"
                + "</w:rPr><w:t>[COL7_ANTET]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/></w:p></w:tc>";


            //Tabelul cu modificarile intervenite
            int numar_coloane = 4;
            if (lista["NET"] == "1")
                numar_coloane += 1;
            if (lista["MAR"] == "1")
            {
                if (dtEmpl.Rows[0]["TIPCTR"].ToString() != "97" && dtEmpl.Rows[0]["TIPCTR"].ToString() != "99")
                    numar_coloane += 2; // CIM si nu Agenti
            }
            else
            {
                if (tip_ctr != "97" && tip_ctr != "99")
                    numar_coloane += 2; // CIM si nu Agenti
            }

            if (numar_coloane >= 5)
            {
                baseTable = baseTable.Replace("[COLOANA5_ANTET]", col5);
                if (numar_coloane >= 6)
                {
                    baseTable = baseTable.Replace("[COLOANA6_ANTET]", col6);
                    if (numar_coloane == 7)
                        baseTable = baseTable.Replace("[COLOANA7_ANTET]", col7);
                    else
                        baseTable = baseTable.Replace("[COLOANA7_ANTET]", "");
                }
                else
                {
                    baseTable = baseTable.Replace("[COLOANA6_ANTET]", "");
                    baseTable = baseTable.Replace("[COLOANA7_ANTET]", "");
                }
            }
            else
            {
                baseTable = baseTable.Replace("[COLOANA5_ANTET]", "");
                baseTable = baseTable.Replace("[COLOANA6_ANTET]", "");
                baseTable = baseTable.Replace("[COLOANA7_ANTET]", "");
            }
            string col = "";
            for (int i = 1; i <= numar_coloane; i++)
                col += nr_col;
            //END ADD TABLE

            //SET HEADER                        
            if (numar_coloane > 5)
            {
                baseTable = baseTable.Replace("[COL3_ANTET]", "Deduceri personale");
                baseTable = baseTable.Replace("[COL4_ANTET]", "Alte deduceri");
                baseTable = baseTable.Replace("[COL5_ANTET]", "Venit baza de calcul");
                baseTable = baseTable.Replace("[COL6_ANTET]", "Impozit lunar");
                if (numar_coloane > 6)
                    baseTable = baseTable.Replace("[COL7_ANTET]", "Salariu net");
            }
            else
            {
                baseTable = baseTable.Replace("[COL3_ANTET]", "Venit baza de calcul");
                baseTable = baseTable.Replace("[COL4_ANTET]", "Impozit lunar");
                if (numar_coloane > 4)
                    baseTable = baseTable.Replace("[COL5_ANTET]", "Salariu net");
            }
            //END SET HEADER


            XMLFile = XMLFile.Replace("[NR_COL]", col);
            XMLFile = XMLFile.Replace("[ANTET_TABEL]", baseTable);




            string alteded = "0", baza = "0";
            string baza_imp_prod = "0", impozit_imp_prod = "0"; // producatori agricoli
            string CT_SIND_ST = "", CT_SIND_ORG = "";

            CT_SIND_ST = (lista.ContainsKey("CT_SIND_ST") && lista["CT_SIND_ST"] != null ? lista["CT_SIND_ST"].ToString() : "");
            CT_SIND_ORG = (lista.ContainsKey("CT_SIND_ORG") && lista["CT_SIND_ORG"] != null ? lista["CT_SIND_ORG"].ToString() : "");

            if (CT_SIND_ST == CT_SIND_ORG)
                alteded = (CT_SIND_ST.Length == 0 ? "0" : CT_SIND_ST);
            else
                alteded = (CT_SIND_ST.Length == 0 ? "0" : CT_SIND_ST) + "+" + (CT_SIND_ORG.Length == 0 ? "0" : CT_SIND_ORG);

            if (lista.ContainsKey("BAZA_FF") && lista["BAZA_FF"] != null)
                if (lista["BAZA_FF"].ToString().Length < 8)
                    baza = "F92004040";
                else
                    baza = lista["BAZA_FF"].ToString();
            if (lista.ContainsKey("BAZAC_IMP_PROD") && lista["BAZAC_IMP_PROD"] != null)
                if (lista["BAZAC_IMP_PROD"].ToString().Length < 8)
                    baza_imp_prod = "0";
                else
                    baza_imp_prod = "(" + lista["BAZAC_IMP_PROD"].ToString() + ")";
            if (lista.ContainsKey("IMPOZIT_IMP_PROD") && lista["IMPOZIT_IMP_PROD"] != null)
                if (lista["IMPOZIT_IMP_PROD"].ToString().Length < 8)
                    impozit_imp_prod = "0";
                else
                    impozit_imp_prod = "(" + lista["IMPOZIT_IMP_PROD"].ToString() + ")";

            //Florin 2018.11.23
            //am schimbat F920.F92004055 cu F920.F9204040 

            if (lista["MAR"] == "1")
            {
                sql = "SELECT F910.MONTH, " + baza + ", F920.F92004055, " + alteded + ", F920.F92004052 + " + baza_imp_prod + ", F920.F92004009 + " + impozit_imp_prod + ", F920.F92004042";
                sql += " FROM F910, F920 WHERE F910.F91003=F920.F92003 AND F910.YEAR = F920.YEAR AND F910.MONTH=F920.MONTH AND F910.YEAR = " + lista["ANU"];
                sql += " AND F910.F91025=0 AND F910.F91003 = " + marca + " ORDER BY F910.MONTH";
            }
            else
            {
                sql = "SELECT F910.MONTH, SUM(" + baza + " + " + baza_imp_prod + "), SUM(F920.F92004055), SUM(" + alteded + "), SUM(F920.F92004052), SUM(F920.F92004009 + " + impozit_imp_prod + "), SUM(F920.F92004042)";
                sql += " FROM F910, F920 WHERE F910.F91003=F920.F92003 AND F910.YEAR = F920.YEAR AND F910.MONTH=F920.MONTH AND F910.YEAR = " + lista["ANU"];
                sql += " AND F910.F91025=0 AND F910.F91017 = '" + cnp_contract + "' GROUP BY F910.MONTH ORDER BY F910.MONTH";
            }

            //Row newRow = tbl.Rows.Add(ref nullobj);
            string[] lunile_anului = new string[] { "", "Ianuarie", "Februarie", "Martie", "Aprilie", "Mai", "Iunie", "Iulie", "August", "Septembrie", "Octombrie", "Noiembrie", "Decembrie" };

            dt = new DataTable();
            dt = General.IncarcaDT(sql, null);


            string coloana5 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" "
                + "w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL5]</w:t></w:r></w:p></w:tc>";
            string coloana6 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" "
                + "w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL6]</w:t></w:r></w:p></w:tc>";
            string coloana7 = "<w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" "
                + "w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL7]</w:t></w:r></w:p></w:tc>";

            string tableRow = "<w:tr w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidTr=\"007B07D7\"><w:trPr><w:trHeight w:val=\"400\"/></w:trPr><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/>"
                + "<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/>"
                + "<w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:b/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                + "w:cs=\"Times New Roman\"/><w:b/></w:rPr><w:t>[LUNA]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>"
                + "<w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" "
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[VENIT_BRUT]</w:t>"
                + "</w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1400\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" "
                + "w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                + "</w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL3]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1400\" "
                + "w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"007B07D7\" w:rsidRPr=\"007B07D7\" w:rsidRDefault=\"007B07D7\" w:rsidP=\"007B07D7\"><w:pPr><w:spacing "
                + "w:after=\"0\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" "
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[COL4]</w:t></w:r></w:p></w:tc>[COLOANA5][COLOANA6][COLOANA7]</w:tr>";

            string table = "";

            if (dt != null)
            {
                double total_vnet = 0.00, total_dedpers = 0.00, total_alteded = 0.00, total_bcimp = 0.00, total_impozit = 0.00, total_salariu = 0.00;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    table += tableRow;

                    table = table.Replace("[LUNA]", lunile_anului[int.Parse(dt.Rows[i][0].ToString())]);

                    string value = "";
                    try
                    {
                        if (dt.Rows[i][1].ToString().Length > 0)
                        {
                            value = dt.Rows[i][1].ToString();
                            if (dt.Rows[i][1].ToString().Contains("."))
                                if (dt.Rows[i][1].ToString().Substring(dt.Rows[i][1].ToString().IndexOf("."), 3) == ".00")
                                    value = dt.Rows[i][1].ToString().Substring(0, dt.Rows[i][1].ToString().IndexOf("."));
                            total_vnet += double.Parse(value);
                        }
                        else
                            value = "0";
                    }
                    catch (Exception)
                    {
                        double n = 0.00;
                        if (double.TryParse(dt.Rows[i][1].ToString(), out n))
                        {
                            value = dt.Rows[i][1].ToString();
                            total_vnet += n;
                        }
                        else
                            value = "0";
                    }

                    table = table.Replace("[VENIT_BRUT]", value);

                    if (numar_coloane > 5)
                    {
                        value = "";
                        try
                        {
                            if (dt.Rows[i][2].ToString().Length > 0)
                            {
                                value = dt.Rows[i][2].ToString();
                                if (dt.Rows[i][2].ToString().Contains("."))
                                    if (dt.Rows[i][2].ToString().Substring(dt.Rows[i][2].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][2].ToString().Substring(0, dt.Rows[i][2].ToString().IndexOf("."));
                                total_dedpers += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][2].ToString(), out n))
                            {
                                value = dt.Rows[i][2].ToString();
                                total_dedpers += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COL3]", value);

                        value = "";
                        try
                        {
                            if (dt.Rows[i][3].ToString().Length > 0)
                            {
                                value = dt.Rows[i][3].ToString();
                                if (dt.Rows[i][3].ToString().Contains("."))
                                    if (dt.Rows[i][3].ToString().Substring(dt.Rows[i][3].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][3].ToString().Substring(0, dt.Rows[i][3].ToString().IndexOf("."));
                                total_alteded += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][3].ToString(), out n))
                            {
                                value = dt.Rows[i][3].ToString();
                                total_alteded += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COL4]", value);

                        value = "";
                        try
                        {
                            if (dt.Rows[i][4].ToString().Length > 0)
                            {
                                value = dt.Rows[i][4].ToString();
                                if (dt.Rows[i][4].ToString().Contains("."))
                                    if (dt.Rows[i][4].ToString().Substring(dt.Rows[i][4].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][4].ToString().Substring(0, dt.Rows[i][4].ToString().IndexOf("."));
                                total_bcimp += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][4].ToString(), out n))
                            {
                                value = dt.Rows[i][4].ToString();
                                total_bcimp += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COLOANA5]", coloana5);
                        table = table.Replace("[COL5]", value);

                        value = "";
                        try
                        {
                            if (dt.Rows[i][5].ToString().Length > 0)
                            {
                                value = dt.Rows[i][5].ToString();
                                if (dt.Rows[i][5].ToString().Contains("."))
                                    if (dt.Rows[i][5].ToString().Substring(dt.Rows[i][5].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][5].ToString().Substring(0, dt.Rows[i][5].ToString().IndexOf("."));
                                total_impozit += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][5].ToString(), out n))
                            {
                                value = dt.Rows[i][5].ToString();
                                total_impozit += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COLOANA6]", coloana6);
                        table = table.Replace("[COL6]", value);


                        if (numar_coloane > 6)
                        {
                            value = "";
                            try
                            {
                                if (dt.Rows[i][6].ToString().Length > 0)
                                {
                                    value = dt.Rows[i][6].ToString();
                                    if (dt.Rows[i][6].ToString().Contains("."))
                                        if (dt.Rows[i][6].ToString().Substring(dt.Rows[i][6].ToString().IndexOf("."), 3) == ".00")
                                            value = dt.Rows[i][6].ToString().Substring(0, dt.Rows[i][6].ToString().IndexOf("."));
                                    total_salariu += double.Parse(value);
                                }
                                else
                                    value = "0";
                            }
                            catch (Exception)
                            {
                                double n = 0.00;
                                if (double.TryParse(dt.Rows[i][6].ToString(), out n))
                                {
                                    value = dt.Rows[i][6].ToString();
                                    total_salariu += n;
                                }
                                else
                                    value = "0";
                            }
                            table = table.Replace("[COLOANA7]", coloana7);
                            table = table.Replace("[COL7]", value);
                        }
                        else
                        {
                            table = table.Replace("[COLOANA7]", "");
                        }
                    }
                    else
                    {
                        value = "";
                        try
                        {
                            if (dt.Rows[i][4].ToString().Length > 0)
                            {
                                value = dt.Rows[i][4].ToString();
                                if (dt.Rows[i][4].ToString().Contains("."))
                                    if (dt.Rows[i][4].ToString().Substring(dt.Rows[i][4].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][4].ToString().Substring(0, dt.Rows[i][4].ToString().IndexOf("."));
                                total_bcimp += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][4].ToString(), out n))
                            {
                                value = dt.Rows[i][4].ToString();
                                total_bcimp += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COL3]", value);

                        value = "";
                        try
                        {
                            if (dt.Rows[i][5].ToString().Length > 0)
                            {
                                value = dt.Rows[i][5].ToString();
                                if (dt.Rows[i][5].ToString().Contains("."))
                                    if (dt.Rows[i][5].ToString().Substring(dt.Rows[i][5].ToString().IndexOf("."), 3) == ".00")
                                        value = dt.Rows[i][5].ToString().Substring(0, dt.Rows[i][5].ToString().IndexOf("."));
                                total_impozit += double.Parse(value);
                            }
                            else
                                value = "0";
                        }
                        catch (Exception)
                        {
                            double n = 0.00;
                            if (double.TryParse(dt.Rows[i][5].ToString(), out n))
                            {
                                value = dt.Rows[i][5].ToString();
                                total_impozit += n;
                            }
                            else
                                value = "0";
                        }

                        table = table.Replace("[COL4]", value);

                        if (numar_coloane > 4)
                        {
                            value = "";
                            try
                            {
                                if (dt.Rows[i][6].ToString().Length > 0)
                                {
                                    value = dt.Rows[i][6].ToString();
                                    if (dt.Rows[i][6].ToString().Contains("."))
                                        if (dt.Rows[i][6].ToString().Substring(dt.Rows[i][6].ToString().IndexOf("."), 3) == ".00")
                                            value = dt.Rows[i][6].ToString().Substring(0, dt.Rows[i][6].ToString().IndexOf("."));
                                    total_salariu += double.Parse(value);
                                }
                                else
                                    value = "0";
                            }
                            catch (Exception)
                            {
                                double n = 0.00;
                                if (double.TryParse(dt.Rows[i][6].ToString(), out n))
                                {
                                    value = dt.Rows[i][6].ToString();
                                    total_salariu += n;
                                }
                                else
                                    value = "0";
                            }

                            table = table.Replace("[COLOANA5]", coloana5);
                            table = table.Replace("[COL5]", value);

                            table = table.Replace("[COLOANA6]", "");
                            table = table.Replace("[COLOANA7]", "");
                        }
                        else
                        {
                            table = table.Replace("[COLOANA5]", "");
                            table = table.Replace("[COLOANA6]", "");
                            table = table.Replace("[COLOANA7]", "");
                        }
                    }
                }
                // linia de TOTAL
                {
                    table += tableRow;

                    table = table.Replace("[LUNA]", "TOTAL");



                    if (Math.Floor(Math.Abs(total_vnet)) < total_vnet)
                        table = table.Replace("[VENIT_BRUT]", total_vnet.ToString());
                    else
                        table = table.Replace("[VENIT_BRUT]", ((int)total_vnet).ToString());

                    if (numar_coloane > 5)
                    {
                        if (Math.Floor(Math.Abs(total_dedpers)) < total_dedpers)
                            table = table.Replace("[COL3]", total_dedpers.ToString());
                        else
                            table = table.Replace("[COL3]", ((int)total_dedpers).ToString());

                        if (Math.Floor(Math.Abs(total_alteded)) < total_alteded)
                            table = table.Replace("[COL4]", total_alteded.ToString());
                        else
                            table = table.Replace("[COL4]", ((int)total_alteded).ToString());

                        table = table.Replace("[COLOANA5]", coloana5);
                        if (Math.Floor(Math.Abs(total_bcimp)) < total_bcimp)
                            table = table.Replace("[COL5]", total_bcimp.ToString());
                        else
                            table = table.Replace("[COL5]", ((int)total_bcimp).ToString());

                        table = table.Replace("[COLOANA6]", coloana6);
                        if (Math.Floor(Math.Abs(total_impozit)) < total_impozit)
                            table = table.Replace("[COL6]", total_impozit.ToString());
                        else
                            table = table.Replace("[COL6]", ((int)total_impozit).ToString());


                        if (numar_coloane > 6)
                        {
                            table = table.Replace("[COLOANA7]", coloana7);
                            if (Math.Floor(Math.Abs(total_salariu)) < total_salariu)
                                table = table.Replace("[COL7]", total_salariu.ToString());
                            else
                                table = table.Replace("[COL7]", ((int)total_salariu).ToString());
                        }
                        else
                            table = table.Replace("[COLOANA7]", "");
                    }
                    else
                    {
                        if (Math.Floor(Math.Abs(total_bcimp)) < total_bcimp)
                            table = table.Replace("[COL3]", total_bcimp.ToString());
                        else
                            table = table.Replace("[COL3]", ((int)total_bcimp).ToString());

                        if (Math.Floor(Math.Abs(total_impozit)) < total_impozit)
                            table = table.Replace("[COL4]", total_impozit.ToString());
                        else
                            table = table.Replace("[COL4]", ((int)total_impozit).ToString());

                        if (numar_coloane > 4)
                        {
                            table = table.Replace("[COLOANA5]", coloana5);
                            if (Math.Floor(Math.Abs(total_salariu)) < total_salariu)
                                table = table.Replace("[COL5]", total_salariu.ToString());
                            else
                                table = table.Replace("[COL5]", ((int)total_salariu).ToString());

                            table = table.Replace("[COLOANA6]", "");
                            table = table.Replace("[COLOANA7]", "");
                        }
                        else
                        {
                            table = table.Replace("[COLOANA5]", "");
                            table = table.Replace("[COLOANA6]", "");
                            table = table.Replace("[COLOANA7]", "");
                        }
                    }
                }
            }

            XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);

            XMLFile = XMLFile.Replace("[FUNCTIE_RL1]", lista["FR1"]);
            XMLFile = XMLFile.Replace("[FUNCTIE_RL2]", lista["FR2"]);
            XMLFile = XMLFile.Replace("[NUME_RL1]", lista["NR1"]);
            XMLFile = XMLFile.Replace("[NUME_RL2]", lista["NR2"]);

            XMLFile = XMLFile.Replace("[DIMX]", lista["AdevDimX"]);
            XMLFile = XMLFile.Replace("[DIMY]", lista["AdevDimY"]);

            DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/"));
            FileInfo[] listfiles = root.GetFiles("Logo.*");
            string logo = "";
            if (listfiles.Length > 0)
            {
                byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                logo = Convert.ToBase64String(fileBytes);
            }
            XMLFile = XMLFile.Replace("[LOGO]", logo);

            var folder = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/ADEVERINTE/VENITURI_" + lista["ANU"]));
            if (!folder.Exists)
                folder.Create();

            StreamWriter sw = new StreamWriter(FileName, false);

            sw.Write(XMLFile);
            sw.Close();
            sw.Dispose();

        }
        private void AdeverintaCIC(int marca, string FileName)
        {

            DataTable dtCoAdress = new DataTable();
            string coaddress = "";
            DataTable dtEmpl = new DataTable();
            string sql = "";

            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;

            string cnp_contract = "";
            cnp_contract = marca.ToString();

            coaddress = "SELECT F00204, F00233, F00234, F00235, F00239, F00236, F00231, F00232, F00238, F00237, F00281, F00282, F00241, F00242, F00243, F00207 FROM F002, F100 WHERE F10002 = F00202 AND F10003 = " + marca;
            dtCoAdress = General.IncarcaDT(coaddress, null);
            if (Constante.tipBD == 2)
            {

                sql = "SELECT F10008, F10009, TO_CHAR(F10022, 'dd/mm/yyyy') AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, "
                    + "F10047 AS SEX, F1009741, F100905, TO_CHAR(DATA_NASTERE + 42, 'dd.mm.yyyy') AS COLOANA3, TO_CHAR(MATERNITATE_DELA, 'dd.mm.yyyy') AS COLOANA4, TO_CHAR(MATERNITATE_PANALA, 'dd.mm.yyyy') AS COLOANA5, "
                    + "TO_CHAR(INDEMNIZATIE_DELA, 'dd.mm.yyyy') AS COLOANA6, TO_CHAR(INDEMNIZATIE_PANALA, 'dd.mm.yyyy') AS COLOANA7, TO_CHAR(DATA_APROBARE, 'dd.mm.yyyy') AS COLOANA8, "
                    + "TO_CHAR(DATA_NASTERE, 'dd/mm/yyyy') AS DATA_NASTERE FROM F100, ADEVERINTE_CIC_DATE WHERE F10003 = MARCA AND EMITERE = 1 AND F10003=" + marca;
            }
            else 
            {
                sql = "SELECT F10008, F10009, CONVERT(VARCHAR, F10022, 103) AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, "
                    + "F10047 AS SEX, F1009741, F100905, CONVERT(VARCHAR, DATEADD(day,42,DATA_NASTERE), 104) AS COLOANA3, CONVERT(VARCHAR, MATERNITATE_DELA, 104) AS COLOANA4, CONVERT(VARCHAR, MATERNITATE_PANALA, 104) AS COLOANA5, "
                    + "CONVERT(VARCHAR, INDEMNIZATIE_DELA, 104) AS COLOANA6, CONVERT(VARCHAR, INDEMNIZATIE_PANALA, 104) AS COLOANA7, CONVERT(VARCHAR, DATA_APROBARE, 104) AS COLOANA8, "
                    + "CONVERT(VARCHAR, DATA_NASTERE, 103) AS DATA_NASTERE FROM F100, ADEVERINTE_CIC_DATE WHERE F10003 = MARCA AND EMITERE = 1 AND F10003=" + marca;                
            }
            dtEmpl = General.IncarcaDT(sql, null);

            if (dtEmpl == null || dtEmpl.Rows.Count <= 0)
            {
                MessageBox.Show(Dami.TraduCuvant("Nu sunt date!"));
                return;
            }

            coaddress = "";
            coaddress = dtCoAdress.Rows[0]["F00232"].ToString() +
                (dtCoAdress.Rows[0]["F00233"].ToString().Length == 0 ? "" : ", Str. " + dtCoAdress.Rows[0]["F00233"].ToString())
                + ((dtCoAdress.Rows[0]["F00234"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00234"].ToString() == "0") ? "" : ", Nr. " + dtCoAdress.Rows[0]["F00234"].ToString())
                + ((dtCoAdress.Rows[0]["F00238"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00238"].ToString() == "0") ? "" : ", Judet " + dtCoAdress.Rows[0]["F00238"].ToString())
                ;


            string XMLFile = "";
            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_CIC_sablon.xml")))
            {
                String line = sr.ReadLine();

                while (line != null)
                {
                    XMLFile += line;
                    line = sr.ReadLine();
                }
            }

            XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCoAdress.Rows[0]["F00204"].ToString());
            XMLFile = XMLFile.Replace("[ADRESA_ANGAJATOR]", coaddress);
            XMLFile = XMLFile.Replace("[NR_ORC]", dtCoAdress.Rows[0]["F00241"].ToString() + "/" + dtCoAdress.Rows[0]["F00242"].ToString() + "/" + dtCoAdress.Rows[0]["F00243"].ToString());
            XMLFile = XMLFile.Replace("[COD_CUI]", dtCoAdress.Rows[0]["F00207"].ToString());
            XMLFile = XMLFile.Replace("[TELEFON]", dtCoAdress.Rows[0]["F00281"].ToString());
            XMLFile = XMLFile.Replace("[FAX]", dtCoAdress.Rows[0]["F00282"].ToString());
            XMLFile = XMLFile.Replace("[SEX_ANGAJAT]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "doamna" : "domnul");
            XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["F10008"].ToString());
            XMLFile = XMLFile.Replace("[NUME_ANTERIOR]", dtEmpl.Rows[0]["F100905"].ToString().Length > 0 ? dtEmpl.Rows[0]["F100905"].ToString() : "————");
            XMLFile = XMLFile.Replace("[PRENUME_ANGAJAT]", dtEmpl.Rows[0]["F10009"].ToString());
            XMLFile = XMLFile.Replace("[STRADA_ANGAJAT]", dtEmpl.Rows[0]["F10083"].ToString());
            XMLFile = XMLFile.Replace("[NR_ANGAJAT]", dtEmpl.Rows[0]["F10084"].ToString().Length > 0 ? dtEmpl.Rows[0]["F10084"].ToString() : "————");
            XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["F10017"].ToString());
            XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["F10017"].ToString());

            string text2 = dtEmpl.Rows[0]["F10084"].ToString().Length > 0 ? dtEmpl.Rows[0]["F10084"].ToString() : "————";
            text2 += ((dtEmpl.Rows[0]["F10085"].ToString().Length == 0 || dtEmpl.Rows[0]["F10085"].ToString() == "0") ? "" : ", Bl. " + dtEmpl.Rows[0]["F10085"].ToString());
            text2 += ((dtEmpl.Rows[0]["F100892"].ToString().Length == 0 || dtEmpl.Rows[0]["F100892"].ToString() == "0") ? "" : ", Sc. " + dtEmpl.Rows[0]["F100892"].ToString());
            text2 += ((dtEmpl.Rows[0]["F10086"].ToString().Length == 0 || dtEmpl.Rows[0]["F10086"].ToString() == "0") ? "" : ", Apart. " + dtEmpl.Rows[0]["F10086"].ToString());
            text2 += dtEmpl.Rows[0]["F10082"].ToString().Length > 0 ? ", Sector " + dtEmpl.Rows[0]["F10082"].ToString() : "";

            XMLFile = XMLFile.Replace("[CNP]", text2);
            XMLFile = XMLFile.Replace("[LOCALITATE]", dtEmpl.Rows[0]["F10081"].ToString().Length > 0 ? dtEmpl.Rows[0]["F10081"].ToString() : "————");
            XMLFile = XMLFile.Replace("[JUDET]", dtEmpl.Rows[0]["F100891"].ToString().Length > 0 ? dtEmpl.Rows[0]["F100891"].ToString() : "————");
            XMLFile = XMLFile.Replace("[PERIOADA]", (dtEmpl.Rows[0]["F1009741"].ToString().Trim() == "1" ? "nedeterminată" : "determinată"));
            string dataang = "";
            dataang += dtEmpl.Rows[0]["F10022"].ToString().Substring(0, 2);  
            dataang += ".";
            dataang += dtEmpl.Rows[0]["F10022"].ToString().Substring(3, 2);
            dataang += ".";
            dataang += dtEmpl.Rows[0]["F10022"].ToString().Substring(6, 4);
            XMLFile = XMLFile.Replace("[DATA_ANGAJARII]", dataang);

            XMLFile = XMLFile.Replace("[INDEMNIZATIE]", dtEmpl.Rows[0]["COLOANA4"].ToString() + (dtEmpl.Rows[0]["COLOANA5"].ToString().Length > 0 ? " - " + dtEmpl.Rows[0]["COLOANA5"].ToString() + " " : " "));
            XMLFile = XMLFile.Replace("[LAUZIE]", dtEmpl.Rows[0]["COLOANA5"].ToString().Length > 0 ? dtEmpl.Rows[0]["COLOANA3"].ToString() + " " : " ");
            XMLFile = XMLFile.Replace("[CONCEDIU]", dtEmpl.Rows[0]["COLOANA6"].ToString() + (dtEmpl.Rows[0]["COLOANA7"].ToString().Length > 0 ? " - " + dtEmpl.Rows[0]["COLOANA7"].ToString() + " " : " "));
            XMLFile = XMLFile.Replace("[DATA_APROBARE]", dtEmpl.Rows[0]["COLOANA8"].ToString() + " ");


            DataTable dtdata = new DataTable();
            dtdata = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);

            string luna_istoric_F920 = ""; string luna_istoric_F940 = "";
            int luna = 0; int.TryParse(dtdata.Rows[0][0].ToString(), out luna);
            int an = 0; int.TryParse(dtdata.Rows[0][1].ToString(), out an);

            luna_istoric_F920 = "01/" + (int.Parse(dtdata.Rows[0][0].ToString()) < 10 ? "0" + dtdata.Rows[0][0].ToString() : dtdata.Rows[0][0].ToString())
                + "/" + dtdata.Rows[0][1].ToString();
            if (Constante.tipBD == 2)
            {
                luna_istoric_F920 = "TO_DATE('" + luna_istoric_F920 + "', 'DD/MM/YYYY') ";
                luna_istoric_F940 = luna_istoric_F920;
                luna_istoric_F920 = "TO_DATE('01/'||TO_CHAR(F920.MONTH)||'/'||TO_CHAR(F920.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F920;
                luna_istoric_F940 = "TO_DATE('01/'||TO_CHAR(F940.MONTH)||'/'||TO_CHAR(F940.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F940;
            }
            else
            {
                luna_istoric_F920 = "CONVERT(DATETIME, '" + luna_istoric_F920 + "', 103)";
                luna_istoric_F940 = luna_istoric_F920;
                luna_istoric_F920 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F920.MONTH)+'/'+CONVERT(VARCHAR,F920.YEAR), 103) < " + luna_istoric_F920;
                luna_istoric_F940 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F940.MONTH)+'/'+CONVERT(VARCHAR,F940.YEAR), 103) < " + luna_istoric_F940;
            }

            int nrluni = 25;
            string data_luna = "";
            data_luna = "01/" + (int.Parse(dtdata.Rows[0][0].ToString()) < 10 ? "0" + dtdata.Rows[0][0].ToString() : dtdata.Rows[0][0].ToString()) + "/" + dtdata.Rows[0][1].ToString();

            string data_nastere = dtEmpl.Rows[0]["DATA_NASTERE"].ToString();

            string zile_lucrate = lista["ZLU"].ToUpper();
            string zile_cm = lista["ZCM"].ToUpper();
            string zile_co = lista["ZCO"].ToUpper();
            string zile_absente = lista["ZAB"].ToUpper();
            string venit_realizat = lista["VEN"].ToUpper();

            if (data_luna.Substring(3) == data_nastere.Substring(3))
            {
                zile_lucrate = zile_lucrate.Replace("F920", "F200"); zile_lucrate = zile_lucrate.Replace("F910", "F100");
                zile_cm = zile_cm.Replace("F920", "F200"); zile_cm = zile_cm.Replace("F910", "F100");
                zile_absente = zile_absente.Replace("F920", "F200"); zile_absente = zile_absente.Replace("F910", "F100");
                zile_co = zile_co.Replace("F920", "F200"); zile_co = zile_co.Replace("F910", "F100");
                venit_realizat = venit_realizat.Replace("F920", "F200"); venit_realizat = venit_realizat.Replace("F910", "F100");

                sql += "SELECT 1 AS NO, F01012 AS LUNA, F01011 AS ANUL, SUM(" + zile_lucrate + ") AS SUMA1, SUM(" + zile_cm + ") AS SUMA2, SUM(" + zile_co
                    + ") AS SUMA3, SUM(" + zile_absente + ") AS SUMA4, SUM(" + venit_realizat + ") AS SUMA5 FROM F010, F100, F200 WHERE F01002 = F10002 AND F10003 = F20003 "
                    + "AND F10003 = " + marca + " GROUP BY F01012, F01011 UNION ";

                nrluni = 24;
            }
            zile_lucrate = zile_lucrate.Replace("F200", "F920"); zile_lucrate = zile_lucrate.Replace("F100", "F910");
            zile_cm = zile_cm.Replace("F200", "F920"); zile_cm = zile_cm.Replace("F100", "F910");
            zile_absente = zile_absente.Replace("F200", "F920"); zile_absente = zile_absente.Replace("F100", "F910");
            zile_co = zile_co.Replace("F200", "F920"); zile_co = zile_co.Replace("F100", "F910");
            venit_realizat = venit_realizat.Replace("F200", "F920"); venit_realizat = venit_realizat.Replace("F100", "F910");

            sql = "SELECT NO, LUNA, ANUL, SUMA1, SUMA2, SUMA3, SUMA4, SUMA5 FROM "
                    + "(SELECT ROW_NUMBER() OVER (ORDER BY F920.YEAR DESC, F920.MONTH DESC) AS NO, F920.MONTH AS LUNA, F920.YEAR AS ANUL, SUM(" + zile_lucrate
                    + ") AS SUMA1, SUM(" + zile_cm + ") AS SUMA2, SUM(" + zile_co + ") AS SUMA3, SUM(" + zile_absente + ") AS SUMA4, SUM(" + venit_realizat
                    + ") AS SUMA5 FROM F920, F910 WHERE F920.MONTH = F910.MONTH AND F920.YEAR = F910.YEAR AND F92003 = F91003 AND F91003 = " + marca
                    + " AND ((F920.MONTH <= " + int.Parse(data_nastere.Substring(3, 2)) + " AND F920.YEAR = " + data_nastere.Substring(6, 4) + ") OR (F920.YEAR < " + data_nastere.Substring(6, 4) + "))"
                    + " AND " + luna_istoric_F920      
                    + " GROUP BY F920.MONTH, F920.YEAR) tab WHERE NO <= " + nrluni
                    + " ORDER BY ANUL DESC, LUNA DESC";

            DataTable dt = new DataTable();
            dt = General.IncarcaDT(sql, null);




            string tableRow = "<w:tr w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidTr=\"004B2D3C\"><w:trPr><w:trHeight w:val=\"400\"/></w:trPr><w:tc><w:tcPr><w:tcW w:w=\"600\" " 
                + "w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" " 
                + "w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>" 
                + "</w:rPr><w:t>[NR_CRT]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1800\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/>" 
                + "</w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/>" 
                + "<w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:proofErr w:type=\"spellStart\"/><w:r>" 
                + "<w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[NR_LUNA]</w:t></w:r><w:proofErr " 
                + "w:type=\"spellEnd\"/>	</w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"800\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>" 
                + "<w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr>" 
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[LUNA]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"800\" w:type=\"dxa\"/>" 
                + "<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" " 
                + "w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[ANUL]</w:t></w:r></w:p></w:tc><w:tc>" 
                + "<w:tcPr><w:tcW w:w=\"1300\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" " 
                + "w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr>" 
                + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[NR_ZILE_LUCRATE]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1300\" w:type=\"dxa\"/>" 
                + "<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" " 
                + "w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                + "w:cs=\"Times New Roman\"/></w:rPr><w:t>[NR_ZILE_CM]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1300\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>" 
                + "<w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[NR_ZILE_CO]</w:t>" 
                + "</w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1500\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" " 
                + "w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>" 
                + "</w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[NR_ZILE_CFP]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1300\" " 
                + "w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"004B2D3C\" w:rsidRPr=\"004B2D3C\" w:rsidRDefault=\"004B2D3C\" w:rsidP=\"004B2D3C\"><w:pPr><w:spacing " 
                + "w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                + "w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[VENIT_NET]</w:t></w:r></w:p></w:tc></w:tr>";

            string table = "";
            //string val1 = "————", val2 = "————";

            if (dt != null)
            {
                int i = 0;
                int j = 0;
                for (i = 0; j < dt.Rows.Count && i < 25; i++)
                {
                    DateTime data_linie = new DateTime(int.Parse(dt.Rows[j][2].ToString()), int.Parse(dt.Rows[j][1].ToString()), 1);
                    DateTime data_for = new DateTime(int.Parse(data_nastere.Substring(6, 4)), int.Parse(data_nastere.Substring(3, 2)), 1);
                    data_for = i == 0 ? data_for : data_for.AddMonths(-1 * i);
                    string nr_luna = "";
                    if (data_linie != data_for)
                    {

                        table += tableRow;

                        table = table.Replace("[NR_CRT]", (i + 1).ToString() + ".");

                       
                        if (i == 0)
                            nr_luna = "luna nașterii copilului";
                        else if (i == 1)
                            nr_luna = "luna anterioară lunii nașterii copilului";
                        else
                            nr_luna = "luna a " + i + "-a anterioară lunii nașterii copilului";

                        table = table.Replace("[NR_LUNA]", nr_luna);

                        table = table.Replace("[LUNA]", "——");
                        table = table.Replace("[ANUL]", "——");
                        table = table.Replace("[NR_ZILE_LUCRATE]", "————");
                        table = table.Replace("[NR_ZILE_CM]", "————");
                        table = table.Replace("[NR_ZILE_CO]", "————");
                        table = table.Replace("[NR_ZILE_CFP]", "————");
                        table = table.Replace("[VENIT_NET]", "————");

                        continue;
                    }

                    table += tableRow;
                    table = table.Replace("[NR_CRT]", (i + 1).ToString() + ".");


                    if (i == 0)
                        nr_luna = "luna nașterii copilului";
                    else  if (i == 1)
                        nr_luna = "luna anterioară lunii nașterii copilului";
                    else
                        nr_luna = "luna a " + i + "-a anterioară lunii nașterii copilului";

                    table = table.Replace("[NR_LUNA]", nr_luna);

                    table = table.Replace("[LUNA]", dt.Rows[j][1].ToString());
                    table = table.Replace("[ANUL]", dt.Rows[j][2].ToString());

        
                    string value = "";
                    double nn = 0.00;
                    if (double.TryParse(dt.Rows[j][3].ToString(), out nn))
                    {
                        value = string.Format("{0}", Math.Round(nn));
                    }
                    else
                        value = "0";
                    table = table.Replace("[NR_ZILE_LUCRATE]", value);
                    value = "";
                    nn = 0.00;
                    if (double.TryParse(dt.Rows[j][4].ToString(), out nn))
                    {
                        value = string.Format("{0}", Math.Round(nn));
                    }
                    else
                        value = "0";
                    table = table.Replace("[NR_ZILE_CM]", value);
                    value = "";
                    nn = 0.00;
                    if (double.TryParse(dt.Rows[j][5].ToString(), out nn))
                    {
                        value = string.Format("{0}", Math.Round(nn));
                    }
                    else
                        value = "0";
                    table = table.Replace("[NR_ZILE_CO]", value);
                    value = "";
                    nn = 0.00;
                    if (double.TryParse(dt.Rows[j][6].ToString(), out nn))
                    {
                        value = string.Format("{0}", Math.Round(nn));
                    }
                    else
                        value = "0";
                    table = table.Replace("[NR_ZILE_CFP]", value);
                    value = "";
                    nn = 0.00;
                    if (double.TryParse(dt.Rows[j][7].ToString(), out nn))
                    {
                        value = string.Format("{0}", Math.Round(nn));
                    }
                    else
                        value = "0";
                    table = table.Replace("[VENIT_NET]", value);
                    j++;
                }

                for (i = 0; i < 25; i++)
                {
                    string nr_luna = "";
                    table += tableRow;
                    table = table.Replace("[NR_CRT]", (i + 1).ToString() + ".");

                    if (i == 0)
                        nr_luna = "luna nașterii copilului";
                    else if (i == 1)
                        nr_luna = "luna anterioară lunii nașterii copilului";
                    else
                        nr_luna = "luna a " + i + "-a anterioară lunii nașterii copilului";

                    table = table.Replace("[NR_LUNA]", nr_luna);

                    table = table.Replace("[LUNA]", "——");
                    table = table.Replace("[ANUL]", "——");
                    table = table.Replace("[NR_ZILE_LUCRATE]", "————");
                    table = table.Replace("[NR_ZILE_CM]", "————");
                    table = table.Replace("[NR_ZILE_CO]", "————");
                    table = table.Replace("[NR_ZILE_CFP]", "————");
                    table = table.Replace("[VENIT_NET]", "————");
                }
            }

            XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);

            General.ExecutaNonQuery("UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 0 WHERE MARCA = " + marca, null);


            StreamWriter sw = new StreamWriter(FileName, false);

            sw.Write(XMLFile);
            sw.Close();
            sw.Dispose();  

        }



        private void AdeverintaSomaj(int marca, string FileName)
        {
            try
            {
                
                DataTable dtdata = new DataTable();
                dtdata = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
                string data_luna = "";
                data_luna = "01/" + (int.Parse(dtdata.Rows[0][0].ToString()) < 10 ? "0" + dtdata.Rows[0][0].ToString() : dtdata.Rows[0][0].ToString())
                    + "/" + dtdata.Rows[0][1].ToString();

                DataTable dtCoAdress = new DataTable();
                string coaddress = "";
                DataTable dtEmpl = new DataTable();
                string sql = "";

                Dictionary<String, String> lista = new Dictionary<string, string>();
                if (Session["AdevListaParam"] == null)
                    lista = LoadParameters();
                else
                    lista = Session["AdevListaParam"] as Dictionary<string, string>;

                string luna_istoric_F920 = ""; string luna_istoric_F940 = "";
                int luna = 0; int.TryParse(dtdata.Rows[0][0].ToString(), out luna);
                int an = 0; int.TryParse(dtdata.Rows[0][1].ToString(), out an);

                luna_istoric_F920 = "01/" + (int.Parse(dtdata.Rows[0][0].ToString()) < 10 ? "0" + dtdata.Rows[0][0].ToString() : dtdata.Rows[0][0].ToString())
                    + "/" + dtdata.Rows[0][1].ToString();
                if (Constante.tipBD == 2)
                {
                    luna_istoric_F920 = "TO_DATE('" + luna_istoric_F920 + "', 'DD/MM/YYYY') ";
                    luna_istoric_F940 = luna_istoric_F920;
                    luna_istoric_F920 = "TO_DATE('01/'||TO_CHAR(F920.MONTH)||'/'||TO_CHAR(F920.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F920;
                    luna_istoric_F940 = "TO_DATE('01/'||TO_CHAR(F940.MONTH)||'/'||TO_CHAR(F940.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F940;
                }
                else 
                {
                    luna_istoric_F920 = "CONVERT(DATETIME, '" + luna_istoric_F920 + "', 103)";
                    luna_istoric_F940 = luna_istoric_F920;
                    luna_istoric_F920 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F920.MONTH)+'/'+CONVERT(VARCHAR,F920.YEAR), 103) < " + luna_istoric_F920;
                    luna_istoric_F940 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F940.MONTH)+'/'+CONVERT(VARCHAR,F940.YEAR), 103) < " + luna_istoric_F940;
                }


                string cnp_contract = "";
                cnp_contract = marca.ToString();

                coaddress = "SELECT F00204, F00233, F00234, F00238, F00232, F00281, F00282, F00283, F00205, F00207 FROM F002, F100 WHERE F10002 = F00202 AND F10003 = " + marca;

                if (Constante.tipBD == 2)
                {
                    dtCoAdress = General.IncarcaDT(coaddress, null);

                    sql = "SELECT F10008, F10009, TO_CHAR(F10022, 'dd.mm.yyyy') AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, "
                        + "F10047 AS SEX, F1009741, F100905, F10052, F100985, TO_CHAR(F100986, 'dd.mm.yyyy') AS F100986 FROM F100 WHERE F10003=" + marca;
                    dtEmpl = General.IncarcaDT(sql, null);
                }
                else
                {
                    dtCoAdress = General.IncarcaDT(coaddress, null);

                    sql = "SELECT F10008, F10009, CONVERT(VARCHAR, F10022, 104) AS F10022, F10017, F10083, F10084, F10085, F10086, F10082, F10087, F100892, F100893, F10081, F100891, "
                        + "F10047 AS SEX, F1009741, F100905, F10052, F100985, CONVERT(VARCHAR, F100986, 104) AS F100986 FROM F100 WHERE F10003=" + marca;
                    dtEmpl = General.IncarcaDT(sql, null);
                }

                DataTable dtIncetare = new DataTable();
                if (Constante.tipBD == 2)
                {
                    sql = "SELECT TO_CHAR(F70406, 'dd.mm.yyyy') AS F70406, F09805, TO_CHAR(F70406, 'dd/mm/yyyy') AS DI FROM F704, F098, F721 WHERE F70404=4 AND F70407 = F72102 AND F72106 = F09802 "
                        + "AND F70403 = " + marca + " ORDER BY F70406 DESC";
                    dtIncetare = General.IncarcaDT(sql, null);
                }
                else 
                {
                    sql = "SELECT CONVERT(VARCHAR, F70406, 104) AS F70406, F09805, CONVERT(VARCHAR, F70406, 103) AS DI FROM F704, F098, F721 WHERE F70404=4 AND F70407 = F72102 AND F72106 = F09802 "
                        + "AND F70403 = " + marca + " ORDER BY F70406 DESC";
                    dtIncetare = General.IncarcaDT(sql, null);
                }

                string data_incetare = "";
                int nrluni = 12;

                if (dtIncetare.Rows.Count > 0)
                {
                    data_incetare = dtIncetare.Rows[0]["DI"].ToString();
                    nrluni = 12;
                }
                else
                    data_incetare = data_luna;

                coaddress = "";
                coaddress = dtCoAdress.Rows[0]["F00232"].ToString() +
                    (dtCoAdress.Rows[0]["F00233"].ToString().Length == 0 ? "" : ", Str. " + dtCoAdress.Rows[0]["F00233"].ToString())
                    + ((dtCoAdress.Rows[0]["F00234"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00234"].ToString() == "0") ? "" : ", Nr. " + dtCoAdress.Rows[0]["F00234"].ToString())
                    + ((dtCoAdress.Rows[0]["F00238"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00238"].ToString() == "0") ? "" : ", Judet " + dtCoAdress.Rows[0]["F00238"].ToString())
                    ;


                string XMLFile = "";
                using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_somaj_sablon.xml")))
                {
                    String line = sr.ReadLine();

                    while (line != null)
                    {
                        XMLFile += line;
                        line = sr.ReadLine();
                    }
                }

                XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCoAdress.Rows[0]["F00204"].ToString());
                XMLFile = XMLFile.Replace("[COD_FISCAL]", dtCoAdress.Rows[0]["F00207"].ToString());
                XMLFile = XMLFile.Replace("[COD_CAEN]", dtCoAdress.Rows[0]["F00205"].ToString());
                XMLFile = XMLFile.Replace("[ADRESA_ANGAJATOR]", coaddress);
                XMLFile = XMLFile.Replace("[TELEFON]", dtCoAdress.Rows[0]["F00281"].ToString());
                XMLFile = XMLFile.Replace("[FAX]", dtCoAdress.Rows[0]["F00282"].ToString());
                XMLFile = XMLFile.Replace("[E_MAIL_INTERNET]", dtCoAdress.Rows[0]["F00283"].ToString());
                XMLFile = XMLFile.Replace("[SEX_ANGAJAT]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "doamna" : "domnul");
                XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["F10008"].ToString() + " " + dtEmpl.Rows[0]["F10009"].ToString());
                XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["F10017"].ToString());
                XMLFile = XMLFile.Replace("[ANGAJAT_AI]", dtEmpl.Rows[0]["F10052"].ToString().Length > 0 ? ", actul de identitate BI/CI seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString() : "");

                

                string bc_2017 = "0", ci_2017 = "0", bc_2018 = "0", ci_2018 = "0", szCAM = "4", szCpF100CAM = "F1003803";
                if (int.Parse(data_luna.Substring(6, 4)) < 2018)
                    sql = "SELECT F01311, F01361 FROM F013 WHERE F01304 = " + (lista.ContainsKey("SOMB") ? lista["SOMB"].ToString() : "0");
                else
                    sql = "SELECT F901311, F901361 FROM F9013 WHERE F901304 = " + (lista.ContainsKey("SOMB") ? lista["SOMB"].ToString() : "0")
                        + " AND MONTH = (SELECT MAX(MONTH) FROM F9013 WHERE YEAR = (SELECT MAX(YEAR) FROM F9013 WHERE YEAR <= 2018)) AND YEAR = (SELECT MAX(YEAR) FROM F9013 WHERE YEAR <= 2018)";
                DataTable dt = new DataTable();
                dt = General.IncarcaDT(sql, null);
                int counter = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString().Length > 0 && dt.Rows[0][0].ToString() != "0")
                    {
                        counter = 0;
                        if (int.TryParse(dt.Rows[0][0].ToString(), out counter))
                            ci_2017 = string.Format("F92004{0:000}", (counter + 59));
                    }
                }

                sql = "SELECT F01311, F01361, F01304 FROM F013 WHERE F01304 = (SELECT F80003 FROM F800 WHERE F80002 = 'CASIGM')";

                dt = new DataTable();
                dt = General.IncarcaDT(sql, null);
                counter = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString().Length > 0 && dt.Rows[0][0].ToString() != "0")
                    {
                        counter = 0;
                        if (int.TryParse(dt.Rows[0][0].ToString(), out counter))
                        {
                            bc_2018 = string.Format("F92004{0:000}", (counter + 59));
                            ci_2018 = string.Format("F92004{0:000}", (counter + 59));
                        }
                        counter = 0;
                        if (int.TryParse(dt.Rows[0][2].ToString(), out counter))
                        {
                            szCAM = counter.ToString();
                            szCpF100CAM = string.Format("F91038{0:00}", (counter - 1));
                        }
                    }
                }

                sql = "";

                bc_2017 = bc_2017.ToUpper(); ci_2017 = ci_2017.ToUpper();
                bc_2018 = bc_2018.ToUpper(); ci_2018 = ci_2018.ToUpper();


                if (data_incetare.Substring(3) == data_luna.Substring(3))
                {
                    sql += "SELECT 1 AS NO, F01012 AS LUNA, F01011 AS ANUL, CASE WHEN F01011 < 2018 THEN " + bc_2017.Replace("F920", "F200") + " ELSE " + bc_2018.Replace("F920", "F200") + " END AS BC, "
                        + "CASE WHEN F01011 < 2018 THEN " + ci_2017.Replace("F920", "F200") + " ELSE (CASE WHEN F10037 = 0 AND " + szCpF100CAM.Replace("F910", "F100") + " = " + szCAM + " THEN " + ci_2018.Replace("F920", "F200") + " ELSE 0 END) END AS CI, "
                        + "DECLARATIE FROM F100, F200, F010 LEFT JOIN ADEVERINTE_SOM_D112 ON F01011 = AN AND F01012 = LUNA "
                        + " WHERE F10003 = F20003 AND F20002 = F01002 AND F20003 = " + marca + " AND NVL("
                        + "(CASE WHEN F01011 < 2018 THEN " + ci_2017.Replace("F920", "F200") + " ELSE (CASE WHEN F10037 = 0 AND " + szCpF100CAM.Replace("F910", "F100") + " = " + szCAM + " THEN " + ci_2018.Replace("F920", "F200") + " ELSE 0 END) END)" + ", 0) <> 0";

                    if (Constante.tipBD == 1)
                        sql = sql.Replace("NVL", "ISNULL");

                    dt = new DataTable();
                    dt = General.IncarcaDT(sql, null);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        nrluni = 11;
                        sql += " UNION ";
                    }
                    else
                    {
                        nrluni = 12;
                        sql = "";
                    }

                }
                if (Constante.tipBD == 2)
                {

                    sql += "SELECT NO, LUNA, ANUL, BC, CI, DECLARATIE FROM "
                        + "(SELECT ROW_NUMBER() OVER (ORDER BY F920.YEAR DESC, F920.MONTH DESC) AS NO, F920.MONTH AS LUNA, F920.YEAR AS ANUL, "    
                        + "CASE WHEN F920.YEAR < 2018 THEN " + bc_2017 + " ELSE " + bc_2018 + " END AS BC, "
                        + "CASE WHEN F920.YEAR < 2018 THEN " + ci_2017 + " ELSE (CASE WHEN F91037 = 0 AND " + szCpF100CAM + " = " + szCAM + " THEN " + ci_2018 + " ELSE 0 END) END AS CI, "
                        + "DECLARATIE FROM F910, F920 LEFT JOIN ADEVERINTE_SOM_D112 ON F920.YEAR = AN AND F920.MONTH = LUNA "
                        + "WHERE F91003 = F92003 AND F910.MONTH = F920.MONTH AND F910.YEAR = F920.YEAR AND F92003 = " + marca
                        + " AND ((F920.MONTH <= " + int.Parse(data_incetare.Substring(3, 2)) + " AND F920.YEAR = " + data_incetare.Substring(6, 4) + ") OR (F920.YEAR < " + data_incetare.Substring(6, 4) + "))"
                        + " AND " + luna_istoric_F920   // mihad 02.03.2018
                        + " AND NVL((CASE WHEN F920.YEAR < 2018 THEN " + ci_2017 + " ELSE (CASE WHEN F91037 = 0 AND " + szCpF100CAM + " = " + szCAM + " THEN " + ci_2018 + " ELSE 0 END) END), 0) <> 0" + ") tab WHERE NO <= " + nrluni + " ORDER BY ANUL DESC, LUNA DESC";
                }
                else
                {
                    sql = sql.Replace("NVL", "ISNULL");

                    sql += "SELECT NO, LUNA, ANUL, BC, CI, DECLARATIE FROM "
                        + "(SELECT ROW_NUMBER() OVER (ORDER BY F920.YEAR DESC, F920.MONTH DESC) AS NO, F920.MONTH AS LUNA, F920.YEAR AS ANUL, "     
                        + "CASE WHEN F920.YEAR < 2018 THEN " + bc_2017 + " ELSE " + bc_2018 + " END AS BC, "
                        + "CASE WHEN F920.YEAR < 2018 THEN " + ci_2017 + " ELSE (CASE WHEN F91037 = 0 AND " + szCpF100CAM + " = " + szCAM + " THEN " + ci_2018 + " ELSE 0 END) END AS CI, "
                        + "DECLARATIE FROM F910, F920 LEFT JOIN ADEVERINTE_SOM_D112 ON F920.YEAR = AN AND F920.MONTH = LUNA "
                        + "WHERE F91003 = F92003 AND F910.MONTH = F920.MONTH AND F910.YEAR = F920.YEAR AND F92003 = " + marca
                        + " AND ((F920.MONTH <= " + int.Parse(data_incetare.Substring(3, 2)) + " AND F920.YEAR = " + data_incetare.Substring(6, 4) + ") OR (F920.YEAR < " + data_incetare.Substring(6, 4) + "))"
                        + " AND " + luna_istoric_F920   // mihad 02.03.2018
                        + " AND ISNULL((CASE WHEN F920.YEAR < 2018 THEN " + ci_2017 + " ELSE (CASE WHEN F91037 = 0 AND " + szCpF100CAM + " = " + szCAM + " THEN " + ci_2018 + " ELSE 0 END) END), 0) <> 0" + ") tab WHERE NO <= " + nrluni + " ORDER BY ANUL DESC, LUNA DESC";
                }

                string tableRow = "<w:tr w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidTr=\"00F2239B\"><w:trPr><w:trHeight w:val=\"400\"/></w:trPr><w:tc><w:tcPr><w:tcW w:w=\"600\" w:type=\"dxa\"/><w:shd w:val=\"clear\" " 
                    + " w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:jc" 
                    + " w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:t>[NR_CRT]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1500\" w:type=\"dxa\"/><w:shd w:val=\"clear\"" 
                    + " w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr>" 
                    + "<w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"" 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:t>[LUNA_AN]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2820\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p" 
                    + " w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz" 
                    + " w:val=\"20\"/></w:rPr><w:t>[BC_CONTRIB]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2820\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00F2239B\"" 
                    + " w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz" 
                    + " w:val=\"20\"/></w:rPr><w:t>[BC_INDEMN]</w:t></w:r></w:p></w:tc>		<w:tc><w:tcPr><w:tcW w:w=\"2820\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00F2239B\"" 
                    + " w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:jc w:val=\"right\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"" 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:t>[NR_INREG]</w:t></w:r>" 
                    + "</w:p></w:tc>	<w:tc><w:tcPr><w:tcW w:w=\"2820\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\"" 
                    + " w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:proofErr" 
                    + " w:type=\"spellStart\"/><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:t>[NATURA_VENIT]</w:t></w:r><w:proofErr w:type=\"spellEnd\"/></w:p></w:tc></w:tr>";
                string table = "";
                string val = " ";

                dt = new DataTable();
                dt = General.IncarcaDT(sql, null);
                if (dt != null)
                {
                    int i = 0;
                    for (i = 0; i < dt.Rows.Count; i++)
                    {
                        table += tableRow;

                        table = table.Replace("[NR_CRT]", (i + 1).ToString());
                        table = table.Replace("[LUNA_AN]", dt.Rows[i]["LUNA"].ToString() + "." + dt.Rows[i]["ANUL"].ToString());

                        string value = "";
                        double nn = 0.00;
                        if (double.TryParse(dt.Rows[i][3].ToString(), out nn))
                        {
                            value = string.Format("{0}", Math.Round(nn));
                        }
                        else
                            value = "0";
                        table = table.Replace("[BC_CONTRIB]", value);

                        value = "";
                        nn = 0.00;
                        if (double.TryParse(dt.Rows[i][4].ToString(), out nn))
                        {
                            value = string.Format("{0}", Math.Round(nn));
                        }
                        else
                            value = "0";
                        
                        table = table.Replace("[BC_INDEMN]", value);
                        table = table.Replace("[NR_INREG]", dt.Rows[i]["DECLARATIE"].ToString());
                        table = table.Replace("[NATURA_VENIT]", "Salariu");
                    }
                }
                else
                {
                    table = tableRow;
                    table = table.Replace("[NR_CRT]", val);
                    table = table.Replace("[LUNA_AN]", val);
                    table = table.Replace("[BC_CONTRIB]", val);
                    table = table.Replace("[BC_INDEMN]", val);
                    table = table.Replace("[NR_INREG]", val);
                    table = table.Replace("[NATURA_VENIT]", val);
                }

                XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);

                XMLFile = XMLFile.Replace("[NR_DATA_ACT]", dtEmpl.Rows[0]["F100985"].ToString() + " / " + dtEmpl.Rows[0]["F100986"].ToString() + " .");
                XMLFile = XMLFile.Replace("[DATA_ANGAJARII]", dtEmpl.Rows[0]["F10022"].ToString());
                XMLFile = XMLFile.Replace("[DATA_INCETARE]", dtIncetare.Rows.Count > 0 ? dtIncetare.Rows[0]["F70406"].ToString() + " ." : " .");
                XMLFile = XMLFile.Replace("[TEMEI_INCETARE]", dtIncetare.Rows.Count > 0 ? dtIncetare.Rows[0]["F09805"].ToString() + " ." : " .");
                XMLFile = XMLFile.Replace("[ANGAJAT_AI]", dtEmpl.Rows[0]["F10052"].ToString().Length > 0 ? ", actul de identitate BI/CI seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString() : "");
                XMLFile = XMLFile.Replace("[ANGAJAT_AI]", dtEmpl.Rows[0]["F10052"].ToString().Length > 0 ? ", actul de identitate BI/CI seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString() : "");
                XMLFile = XMLFile.Replace("[ANGAJAT_AI]", dtEmpl.Rows[0]["F10052"].ToString().Length > 0 ? ", actul de identitate BI/CI seria și numărul " + dtEmpl.Rows[0]["F10052"].ToString() : "");



                string zile_suspendare = lista["SUS"];
                zile_suspendare = zile_suspendare.Replace("+", ","); zile_suspendare = zile_suspendare.Replace("-", ",");
                zile_suspendare = zile_suspendare.Replace(".", ","); zile_suspendare = zile_suspendare.Replace(";", ",");

                DataTable dtSuspendari = new DataTable();
                sql = "";
                if (data_incetare.Substring(3) == data_luna.Substring(3))
                {
                    if (Constante.tipBD == 2)
                    {
                        sql += "SELECT DISTINCT TO_CHAR(F40037, 'DD.MM.YYYY') AS F40037, TO_CHAR(F40038, 'DD.MM.YYYY') AS F40038, (F40038 - F40037 + 1) AS MOTIV, F01012 AS LUNA, F01011 AS ANUL, "
                            + "CASE WHEN (F40010 < 4449 AND F40010 > 4400) THEN 'Zile concediu medical' ELSE '' END AS EXPLICATIE "
                            + "FROM F400, F010 WHERE F40002 = F01002 AND (F40010 IN (" + (zile_suspendare.Length > 0 ? zile_suspendare : "-9") + ") OR (F40010 < 4449 AND F40010 > 4400)) AND F40003 = " + marca
                            + " UNION ";
                    }
                    else
                    {
                        sql += "SELECT DISTINCT CONVERT(VARCHAR, F40037, 104) AS F40037, CONVERT(VARCHAR, F40038, 104) AS F40038, DATEDIFF(day,F40037,F40038) + 1 AS MOTIV, F01012 AS LUNA, F01011 AS ANUL, "
                            + "CASE WHEN (F40010 < 4449 AND F40010 > 4400) THEN 'Zile concediu medical' ELSE '' END AS EXPLICATIE "
                            + "FROM F400, F010 WHERE F40002 = F01002 AND (F40010 IN (" + (zile_suspendare.Length > 0 ? zile_suspendare : "-9") + ") OR (F40010 < 4449 AND F40010 > 4400)) AND F40003 = " + marca
                            + " UNION ";
                    }
                }
                if (Constante.tipBD == 2)
                {
                    sql += "SELECT DISTINCT F40037, F40038, MOTIV, LUNA, ANUL, EXPLICATIE FROM "
                        + "(SELECT ROW_NUMBER() OVER (ORDER BY F940.YEAR DESC, F940.MONTH DESC) AS NO, TO_CHAR(F94037, 'DD.MM.YYYY') AS F40037, TO_CHAR(F94038, 'DD.MM.YYYY') AS F40038, "
                        + "(F94038 - F94037 + 1) AS MOTIV, F940.MONTH AS LUNA, F940.YEAR AS ANUL, "
                        + "CASE WHEN (F94010 < 4449 AND F94010 > 4400) THEN 'Zile concediu medical' ELSE '' END AS EXPLICATIE "
                        + "FROM F940 WHERE F94003 = " + marca + " AND (F94010 IN (" + (zile_suspendare.Length > 0 ? zile_suspendare : "-9") + ") OR (F94010 < 4449 AND F94010 > 4400))"
                        + " AND ((F940.MONTH <= " + int.Parse(data_incetare.Substring(3, 2)) + " AND F940.YEAR = " + data_incetare.Substring(6, 4) + ") OR (F940.YEAR < " + data_incetare.Substring(6, 4) + "))"
                        + " AND " + luna_istoric_F940   // mihad 02.03.2018
                        + ") tab ORDER BY ANUL DESC, LUNA DESC, F40037 DESC";   
                }
                else
                {
                    sql += "SELECT DISTINCT F40037, F40038, MOTIV, LUNA, ANUL, EXPLICATIE FROM "
                        + "(SELECT ROW_NUMBER() OVER (ORDER BY F940.YEAR DESC, F940.MONTH DESC) AS NO, CONVERT(VARCHAR, F94037, 104) AS F40037, CONVERT(VARCHAR, F94038, 104) AS F40038, "
                        + "DATEDIFF(day,F94037,F94038) + 1 AS MOTIV, F940.MONTH AS LUNA, F940.YEAR AS ANUL, "
                        + "CASE WHEN (F94010 < 4449 AND F94010 > 4400) THEN 'Zile concediu medical' ELSE '' END AS EXPLICATIE "
                        + "FROM F940 WHERE F94003 = " + marca + " AND (F94010 IN (" + (zile_suspendare.Length > 0 ? zile_suspendare : "-9") + ") OR (F94010 < 4449 AND F94010 > 4400))"
                        + " AND ((F940.MONTH <= " + int.Parse(data_incetare.Substring(3, 2)) + " AND F940.YEAR = " + data_incetare.Substring(6, 4) + ") OR (F940.YEAR < " + data_incetare.Substring(6, 4) + "))"
                        + " AND " + luna_istoric_F940   // mihad 02.03.2018
                        + ") tab ORDER BY ANUL DESC, LUNA DESC, F40037 DESC";        
                }

                string suspRow = "<w:p w:rsidR=\"00F2239B\" w:rsidRPr=\"00F2239B\" w:rsidRDefault=\"00F2239B\" w:rsidP=\"00F2239B\"><w:pPr><w:spacing w:before=\"20\" w:after=\"20\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\"" 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr></w:pPr><w:r w:rsidRPr=\"00F2239B\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\"" 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"20\"/></w:rPr><w:tab/></w:r><w:r w:rsidRPr=\"00F2239B\"><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz" 
                    + " w:val=\"20\"/></w:rPr><w:tab/><w:t xml:space=\"preserve\">[TEXT_SUSPENDARE]</w:t></w:r>	</w:p>";
                string susp = "";
                string valSusp = "Data de suspendare ————  data de încetare a suspendării ———— motivul suspendării**) ";

                dtSuspendari = General.IncarcaDT(sql, null);
                for (int j = 0; j < dtSuspendari.Rows.Count; j++)
                {
                    susp += suspRow;
                    valSusp = "Data de suspendare ";
                    valSusp += dtSuspendari.Rows[j]["F40037"].ToString();
                    valSusp += " data de încetare a suspendării ";
                    valSusp += dtSuspendari.Rows[j]["F40038"].ToString();
                    valSusp += " motivul suspendării**) ";
                    if (dtSuspendari.Rows[j]["EXPLICATIE"].ToString().Length > 5)
                    {
                        valSusp += dtSuspendari.Rows[j]["EXPLICATIE"].ToString() + ": " + dtSuspendari.Rows[j]["MOTIV"].ToString();
                    }
                    susp = susp.Replace("[TEXT_SUSPENDARE]", valSusp);

                }
                if (dtSuspendari.Rows.Count == 0)
                {
                    susp = suspRow;
                    susp = susp.Replace("[TEXT_SUSPENDARE]", valSusp);
                }
                XMLFile = XMLFile.Replace("[RAND_SUSPENDARE]", susp);

                string titlu = "Administrator/Director/Reprezentant legal";
                if (lista["TIT"].Length > 0)
                    titlu = lista["TIT"];

                string compartiment = "_________________________";
                if (lista["CMP"].Length > 0)
                    compartiment = lista["CMP"];

                XMLFile = XMLFile.Replace("[REPR_TITLU]", titlu);
                XMLFile = XMLFile.Replace("[REPR_COMPARTIMENT]", compartiment);

                string NRL1 = "Nume şi prenume", FRL1 = "funcţia  (în clar)", NRL2 = "Nume şi prenume", FRL2 = "funcţia  (în clar)";
                if (lista["FR1"].Length > 0)
                    FRL1 = lista["FR1"];
                if (lista["FR2"].Length > 0)
                    FRL2 = lista["FR2"];
                if (lista["NR1"].Length > 0)
                    NRL1 = lista["NR1"];
                if (lista["NR2"].Length > 0)
                    NRL2 = lista["NR2"];


                XMLFile = XMLFile.Replace("[FUNCTIE_RL1]", FRL1);
                XMLFile = XMLFile.Replace("[FUNCTIE_RL2]", FRL2);
                XMLFile = XMLFile.Replace("[NUME_RL1]", NRL1);
                XMLFile = XMLFile.Replace("[NUME_RL2]", NRL2);


                StreamWriter sw = new StreamWriter(FileName, false);

                sw.Write(XMLFile);
                sw.Close();
                sw.Dispose();

            }
            catch
            {

            }
        }

        private void AdeverintaVechime(int marca, string FileName)
        {
            DataTable dtCoAdress = new DataTable();
            DataTable dtCompany = new DataTable();
            string coaddress = "";
            DataTable dtEmpl = new DataTable();
            DataTable dtEmplFunction = new DataTable();
            DataTable dtEmplFunction704 = new DataTable();
            DataTable dtEmplMutations = new DataTable();
            DataTable dtEmplSum = new DataTable();

            string dataStart = "";
            string dataSf = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;

            if (Constante.tipBD == 2)
            {

                dtCompany = General.IncarcaDT("SELECT TRIM(f00204) || CASE WHEN(f00220=1) THEN (' ' || TRIM(f00221)) ELSE '' END, "
                                        + "F00207, F00241 || '/' || F00242 || '/' || F00243 FROM F002", null);

                coaddress = "SELECT F00231, F00233, F00234, "
                    + "CASE WHEN F00235 IS NULL OR F00235 = '0' THEN '' ELSE F00235 END, "
                    + "CASE WHEN F00239 IS NULL OR F00239 = '0' THEN '' ELSE F00239 END, "
                    + "CASE WHEN F002310 IS NULL OR F002310 = '0' THEN '' ELSE F002310 END, "
                    + "CASE WHEN F00236 = 0 THEN '' ELSE TO_CHAR(F00236) END, "
                    + "CASE WHEN F00232 IS NULL THEN '' ELSE F00232 END, F00237, "
                    + "F00207 AS CUI, "
                    + "TRIM(f00204) || CASE WHEN(f00220=1) THEN (' ' || TRIM(f00221)) ELSE '' END AS CONAME, "
                    + "F00241 || '/' || F00242 || '/' || F00243 AS COFISC, F00261||' '||F00262 AS DIRECTOR, F00281 AS TELEFON FROM F002";
                dtCoAdress = General.IncarcaDT(coaddress, null);

                coaddress = "";
                if (dtCoAdress.Rows[0][0].ToString().Length > 0)
                    coaddress += dtCoAdress.Rows[0][0].ToString() + ", ";
                if (dtCoAdress.Rows[0][1].ToString().Length > 0)
                    coaddress += "Strada " + dtCoAdress.Rows[0][1].ToString() + ", ";
                if (dtCoAdress.Rows[0][2].ToString().Length > 0)
                    coaddress += "Nr " + dtCoAdress.Rows[0][2].ToString() + ", ";
                if (dtCoAdress.Rows[0][3].ToString().Length > 0)
                    coaddress += "Bloc " + dtCoAdress.Rows[0][3].ToString() + ", ";
                if (dtCoAdress.Rows[0][4].ToString().Length > 0)
                    coaddress += "Scara " + dtCoAdress.Rows[0][4].ToString() + ", ";
                if (dtCoAdress.Rows[0][5].ToString().Length > 0)
                    coaddress += "Etaj " + dtCoAdress.Rows[0][5].ToString() + ", ";
                if (dtCoAdress.Rows[0][6].ToString().Length > 0)
                    coaddress += "Ap " + dtCoAdress.Rows[0][6].ToString() + ", ";
                if (dtCoAdress.Rows[0][7].ToString().Length > 0)
                    coaddress += "Sector " + dtCoAdress.Rows[0][7].ToString() + ", ";
                if (dtCoAdress.Rows[0][8].ToString().Length > 0)
                    coaddress += "Cod postal " + dtCoAdress.Rows[0][8].ToString() + ", ";

                if (coaddress.Substring(coaddress.Length - 2, 2) == ", ")
                    coaddress = coaddress.Substring(0, coaddress.Length - 2);

                dtEmpl = General.IncarcaDT("SELECT trim(F10008)||' '||trim(F10009) AS NUME, "
                    + "TRIM(F10017) AS CNP, "
                    + "F10047 AS SEX, "
                    + "TO_CHAR(F10022, 'dd/MM/yyyy') AS EMPL_DATANG, "
                    + "TO_CHAR(F100993, 'dd/MM/yyyy') AS EMPL_DATASF, "
                    + "F10025, "
                    + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                    + "TRIM(F10052) AS SN_AI, "
                    + "trim(F10081) AS DOMICILIU, "
                    + "trim(F10083) AS STR, "
                    + "trim(F10084) AS NR, "
                    + "trim(F10085) AS BL, "
                    + "trim(F10086) AS AP, "
                    + "F10043 AS NORMA, "
                    + "CASE WHEN F1009741=1 THEN 'nedeterminata' ELSE 'determinata' END AS DURATA, "
                    + "F100985 || ' din data de ' ||TO_CHAR(F100986, 'dd.MM.yyyy') AS NRINSPECTORAT, "
                    + "trim(F100891) AS JUDET, "
                    + "trim(F10082) AS SECTOR "        
                    + " FROM F100 WHERE F10003=" + marca, null);

                string temp = "";
                if (rbFunc2.Checked)
                {
                    temp = "SELECT F71804 FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                }
                else
                {
                    temp = "SELECT F72204 FROM F100, F722, F010 WHERE F10098=F72202(+) AND "
                             + "(F72206 = case when f01011 < 2012 then 5 else case when f01011 < 2016 or (f01011 = 2016 and f01012 < 4) then 6 "
                             + "else case when f01011 > 2016 or (f01011 = 2016 and f01012 > 4) then 7 else "
                             + "case when f10022 < TO_DATE('01/01/2012', 'dd/mm/yyyy') then 5 else case when F10022 < TO_DATE('17/04/2016', 'dd/mm/yyyy') then 6 else 7 end end end end end OR F72206 is null) AND F10003=" + marca;
                }
                dtEmplFunction = General.IncarcaDT(temp, null);

                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                string salariu_i = salariu.Replace("F100", "F910");

                temp =
                     ("SELECT CASE WHEN (TO_CHAR(MAX(F910992), 'dd/MM/yyyy') <> '01/01/1900' AND EXTRACT(YEAR FROM MAX(F910992)) = Max(F910.YEAR) AND EXTRACT(MONTH FROM MAX(F910992)) = Min(F910.MONTH)) THEN TO_CHAR(MAX(F910992), 'yyyy-MM-dd') "
                     + "ELSE CASE WHEN (TO_DATE('01/'||MIN (F910.MONTH)||'/'||MAX (F910.YEAR), 'dd/MM/yyyy')- MIN(F910991)<31) THEN TO_CHAR(MIN(F910991), 'yyyy-MM-dd') "

                     + "ELSE CASE WHEN (ROUND(F910." + salariu_i + ") = 0 AND TO_CHAR(MAX(F91076), 'dd/MM/yyyy') <> '01/01/2100' ) THEN TO_CHAR(MAX(F91076), 'yyyy-MM-dd') "

                     + "ELSE TO_CHAR( MIN(TO_DATE(YEAR||'-'||LPAD(TO_CHAR(F910.MONTH), 2, '0')||'-01', 'yyyy-MM-dd')), 'yyyy-MM-dd') END END END AS DATA,"
                     + "TO_CHAR(Max(F10022), 'dd/MM/yyyy') AS DATANG, ROUND(F910." + salariu_i + ") AS SALARIU, "
                     + "CASE WHEN (TO_DATE(MIN(F100993), 'dd/mm/yyyy') < TO_DATE('01/01/2100', 'dd/mm/yyyy')) THEN TO_CHAR(MIN(F100993), 'dd/MM/yyyy') ELSE NULL END AS DATASF,"
                     + (rbFunc2.Checked ? "NVL(MIN(F718_I.F71804), MIN(F718.F71804))" : "MIN(F722.F72204)") + " AS FUNCTIA, MAX(F098.F09804) AS MOTIVINTR FROM F910, F717, F006, F721, " + (rbFunc2.Checked ? "F718 F718_I, F718" : "F722") + ", F100, F098 "
                     + "WHERE F910.F91029 = F717.F71702(+) "
                     + "AND F910.F91007 = F006.F00607(+) AND F10025 = F721.F72102 AND F910.F91003 = F100.F10003 AND "
                     + (rbFunc2.Checked ? "F910.F91071 = F718_I.F71802(+) AND F100.F10071 = F718.F71802(+) " : "F910.F91098 = F722.F72202(+) AND F72206 = case when f91022 < TO_DATE('01/01/2012', 'dd/mm/yyyy') then 5 else case when F91022 < TO_DATE('17/04/2016', 'dd/mm/yyyy') then 6 else 7 end end ") 
                                                                                                                                                                                                                                                                                                                      
                     + "AND TO_DATE('01/'||LPAD(TO_CHAR(F910.MONTH), 2, '0')||'/'||TO_CHAR(F910.YEAR), 'dd/MM/yyyy') BETWEEN "
                     
                     + "CASE WHEN F10022 < TO_DATE('01/01/2011', 'dd/mm/yyyy') THEN TO_DATE('01/01/2011', 'dd/mm/yyyy') ELSE F10022 END AND SYSDATE "

                     + "AND F72106 = F09802 "
                     + "GROUP BY F910.F91003, F910." + salariu_i + ", F910.F91071, F91076, F91077 "
                     + "HAVING F910.F91003=" + marca 
                     + " ORDER BY DATA, SALARIU, Max(F100.F10022)");
                dtEmplMutations = General.IncarcaDT(temp, null);

                //if (lista.ContainsKey("REP_CODZILEABNEM") && lista["REP_CODZILEABNEM"].ToString() != String.Empty
                //    && lista.ContainsKey("REP_CODZILECFP") && lista["REP_CODZILECFP"].ToString() != String.Empty)
                //{
                //    string sql = "SELECT CASE WHEN SUM(" + lista["REP_CODZILEABNEM"].ToString().Replace("F200", "F920") + ") IS NOT NULL THEN "
                //               + "SUM(" + lista["REP_CODZILEABNEM"].ToString().Replace("F200", "F920") + ") ELSE 0 END AS CODZILEABNEM, "
                //               + "CASE WHEN SUM(" + lista["REP_CODZILECFP"].ToString().Replace("F200", "F920") + ") IS NOT NULL THEN "
                //               + "SUM(" + lista["REP_CODZILECFP"].ToString().Replace("F200", "F920") + ") ELSE 0 END AS CODZILECFP  FROM F920, F100 "
                //               + "WHERE F92003 = " + marca + " AND TO_DATE('01/'||LPAD(TO_CHAR(F920.MONTH), 2, '0')||'/'||TO_CHAR(F920.YEAR), 'dd/MM/yyyy') "                               
                //               + "BETWEEN CASE WHEN F10022 < TO_DATE('01/01/2011', 'dd/mm/yyyy') THEN TO_DATE('01/01/2011', 'dd/mm/yyyy') ELSE F10022 END AND SYSDATE AND F10003 = F92003 "
                //               + " UNION "
                //               + "SELECT CASE WHEN SUM(" + lista["REP_CODZILEABNEM"].ToString() + ") IS NOT NULL THEN "
                //               + "SUM(" + lista["REP_CODZILEABNEM"].ToString() + ") ELSE 0 END AS CODZILEABNEM, CASE WHEN "
                //               + "SUM(" + lista["REP_CODZILECFP"].ToString() + ") IS NOT NULL THEN SUM(" + lista["REP_CODZILECFP"].ToString()
                //               + ") ELSE 0 END AS CODZILECFP  FROM F200 WHERE F20003 = " + marca;

                //    dtEmplSum = General.IncarcaDT(sql, null);
                //}

                string sql = "SELECT cast(COALESCE( " +
                      "  (SELECT SUM(NEM) FROM ( " +
                      "      select " + lista["NEM"].ToString() + " AS NEM FROM F200 " +
                      "      INNER JOIN F100 ON F200.F20003=F100.F10003 " +
                      "      where F20003 = " + marca +
                      "      union ALL " +
                      "      select " + lista["NEM"].ToString().Replace("F200", "F920") + " as NEM FROM F920  " +
                      "      INNER JOIN F100 on F920.F92003=F100.F10003 " +
                      "      WHERE F92003 = " + marca + ") A) , 0) AS INT ) CODZILEABNEM," +
                      "  CAST(COALESCE(" +
                      "  (SELECT SUM(CFP) FROM" +
                      "      (select " + lista["CFP"].ToString() + " AS CFP FROM F200 " +
                      "      INNER JOIN F100 ON F200.F20003=F100.F10003   " +
                      "      where F20003 = " + marca +
                      "      union ALL " +
                      "      select " + lista["CFP"].ToString().Replace("F200", "F920") + " AS CFP FROM F920  " +
                      "      INNER JOIN F100 on F920.F92003=F100.F10003 " +
                      "      WHERE F92003= " + marca + ") A), 0) AS INT) CODZILECFP " +
                      "FROM DUAL";
                dtEmplSum = General.IncarcaDT(sql, null);
            }
            else            
            {
                dtCompany = General.IncarcaDT("SELECT LTRIM(RTRIM(f00204)) + CASE WHEN(f00220=1) THEN (' ' + LTRIM(RTRIM(f00221))) ELSE '' END, "
                                        + "F00207, F00241 + '/' + CONVERT(VARCHAR,F00242) + '/' + CONVERT(VARCHAR,F00243) FROM F002", null);

                coaddress = " SELECT CAST(F00231 AS VARCHAR(256)), CAST(F00233 AS VARCHAR(256)), CAST(F00234 AS VARCHAR(256)), CAST(CASE WHEN F00235 IS NULL OR F00235 = '0' THEN '' ELSE F00235 END AS VARCHAR(256)), "
                    + "CAST(CASE WHEN F00239 IS NULL OR F00239 = '0' THEN '' ELSE F00239 END AS VARCHAR(256)), "
                    + "CAST(CASE WHEN F002310 IS NULL OR F002310 = '0' THEN '' ELSE F002310 END AS VARCHAR(256)), "
                    + "CAST(CASE WHEN F00236 = 0 THEN '' ELSE CONVERT(VARCHAR,F00236) END AS VARCHAR(256)), "
                    + "CAST(CASE WHEN F00232 IS NULL THEN '' ELSE F00232 END AS VARCHAR(256)), "
                    + "CAST(F00237 AS VARCHAR(256)), F00207 AS CUI, "
                    + "(CAST(LTRIM(RTRIM(f00204)) AS VARCHAR(256)) + CAST(CASE WHEN(f00220=1) THEN (' ' + LTRIM(RTRIM(f00221))) ELSE '' END AS VARCHAR(256))) AS CONAME, "
                    + "(CAST (F00241 AS VARCHAR(256)) + CAST('/' AS VARCHAR(256)) + CAST(F00242 AS VARCHAR(256)) + CAST('/' AS VARCHAR(256)) + CAST(F00243 AS VARCHAR(256))) AS COFISC, "
                    + "(CAST(F00261 AS VARCHAR(256))+CAST(' ' AS VARCHAR(256))+CAST(F00262 AS VARCHAR(256))) AS DIRECTOR, F00281 AS TELEFON FROM F002";
                dtCoAdress = General.IncarcaDT(coaddress, null);
                coaddress = "";
                if (dtCoAdress.Rows[0][0].ToString().Length > 0)
                    coaddress += dtCoAdress.Rows[0][0].ToString() + ", ";
                if (dtCoAdress.Rows[0][1].ToString().Length > 0)
                    coaddress += "Strada " + dtCoAdress.Rows[0][1].ToString() + ", ";
                if (dtCoAdress.Rows[0][2].ToString().Length > 0)
                    coaddress += "Nr " + dtCoAdress.Rows[0][2].ToString() + ", ";              
                if (dtCoAdress.Rows[0][3].ToString().Length > 0)
                    coaddress += "Bloc " + dtCoAdress.Rows[0][3].ToString() + ", ";
                if (dtCoAdress.Rows[0][4].ToString().Length > 0)
                    coaddress += "Scara " + dtCoAdress.Rows[0][4].ToString() + ", ";
                if (dtCoAdress.Rows[0][5].ToString().Length > 0)
                    coaddress += "Etaj " + dtCoAdress.Rows[0][5].ToString() + ", ";
                if (dtCoAdress.Rows[0][6].ToString().Length > 0)
                    coaddress += "Ap " + dtCoAdress.Rows[0][6].ToString() + ", ";
                if (dtCoAdress.Rows[0][7].ToString().Length > 0)
                    coaddress += "Sector " + dtCoAdress.Rows[0][7].ToString() + ", ";
                if (dtCoAdress.Rows[0][8].ToString().Length > 0)
                    coaddress += "Cod postal " + dtCoAdress.Rows[0][8].ToString() + ", ";

                if (coaddress.Substring(coaddress.Length - 2, 2) == ", ")
                    coaddress = coaddress.Substring(0, coaddress.Length - 2);


                dtEmpl = General.IncarcaDT("SELECT CAST(LTRIM(RTRIM(F10008)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F10009)) AS VARCHAR(256)) AS NUME, "
                            + "LTRIM(RTRIM(F10017)) AS CNP, "
                            + "F10047 AS SEX, "
                            + "CONVERT(VARCHAR, F10022, 103) AS EMPL_DATANG, "
                            + "CONVERT(VARCHAR,F100993, 103) AS EMPL_DATASF, "
                            + "F10025, "
                            + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                            + "LTRIM(RTRIM(F10052)) AS SN_AI, "
                            + "LTRIM(RTRIM(F10081)) AS DOMICILIU, "
                            + "LTRIM(RTRIM(F10083)) AS STR, "
                            + "LTRIM(RTRIM(F10084)) AS NR, "
                            + "LTRIM(RTRIM(F10085)) AS BL, "
                            + "LTRIM(RTRIM(F10086)) AS AP, "
                            + "F10043 AS NORMA, "
                            + "CASE WHEN F1009741=1 THEN 'nedeterminata' ELSE 'determinata' END AS DURATA, "
                            + "CAST(F100985 AS VARCHAR(256)) + CAST(' din data de ' AS VARCHAR(256)) +CAST(CONVERT(VARCHAR,F100986, 103) AS VARCHAR(256)) AS NRINSPECTORAT, "
                            + "LTRIM(RTRIM(F100891)) AS JUDET, "
                            + "LTRIM(RTRIM(F10082)) AS SECTOR "    
                            + "FROM F100 WHERE F10003=" + marca, null);

                string temp = "";
                if (rbFunc2.Checked)
                {
                    temp = "SELECT F71804 FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                }
                else
                {
                    temp = "SELECT F72204 FROM F100 LEFT JOIN F722 on F10098=F72202 AND "
                        + "F72206 = case when (select f01011 from f010) < 2012 then 5 else CASE WHEN (select f01011 from f010) < 2016 or ((select f01011 from f010) = 2016 and (select f01012 from f010) < 4) then 6 "
                        + "else case when (select f01011 from f010) > 2016 or ((select f01011 from f010) = 2016 and (select f01012 from f010) > 4) then 7 else "
                        + "case when F10022 < CONVERT(datetime, '01/01/2012', 103) then 5 else CASE WHEN F10022 < CONVERT(datetime, '17/04/2016', 103) then 6 else 7 end end end end end WHERE F10003=" + marca;
                }
                dtEmplFunction = General.IncarcaDT(temp, null);

                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                string salariu_i = salariu.Replace("F100", "F910");

                temp =
                ("SELECT CASE WHEN(CONVERT(VARCHAR,MAX(F910992), 103) <> '01/01/1900' AND YEAR(MAX(F910992)) = Max(F910.YEAR) "
                + "AND MONTH(MAX(F910992)) = Min(F910.MONTH)) THEN CONVERT(VARCHAR, MAX(F910992), 23) ELSE	CASE WHEN "
                + "(CONVERT(DATETIME, '01/' + CONVERT(VARCHAR,MIN(F910.MONTH)) + '/' + CONVERT(VARCHAR,MAX(F910.YEAR)), 103) - MIN(F910991)<31) "
                + "THEN CONVERT(VARCHAR, MIN(F910991), 23)	ELSE CONVERT(VARCHAR, MIN(CONVERT(DATETIME, CONVERT(VARCHAR,YEAR) + '-' + right(replicate('0',2) "
                + "+ CONVERT(VARCHAR,F910.MONTH),2) + '-01', 103)), 23) END END AS DATA, "
                + "CONVERT(VARCHAR, Max(F10022), 103) AS DATANG, ROUND(F910." + salariu_i + ",0) AS SALARIU, "
                + "CASE WHEN (CONVERT(DATETIME, MIN(F100993), 103) < CONVERT(DATETIME, '01/01/2100', 103)) THEN CONVERT(VARCHAR, MIN(F100993), 103) ELSE NULL END AS DATASF, "
                + (rbFunc2.Checked ? "ISNULL(MIN(F718_I.F71804), MIN(F718.F71804))" : "MIN(F722.F72204)") + " AS FUNCTIA, MAX(F098.F09804) AS MOTIVINTR FROM F721, F098, F100"
                + (rbFunc2.Checked ? " LEFT JOIN F718 ON F100.F10071 = F718.F71802 " : "")
                + ", F910 "
                + "LEFT JOIN F717 ON F910.F91029 = F717.F71702 "
                + "LEFT JOIN F006 ON F910.F91007 = F006.F00607 "
                + (rbFunc2.Checked ? "LEFT JOIN F718 F718_I ON F910.F91071 = F718_I.F71802 "
                    : "LEFT JOIN F722 ON F910.F91098 = F722.F72202 AND F72206 = case when F91022 < CONVERT(datetime, '01/01/2012', 103) then 5 else CASE WHEN F91022 < CONVERT(datetime, '17/04/2016', 103) then 6 else 7 end end ") 
                + "WHERE F10025 = F721.F72102 AND F910.F91003 = F100.F10003 "
                + "AND (CONVERT(DATETIME, '01/' + right(replicate('0',2) + CONVERT(VARCHAR,F910.MONTH),2) + '/' +CONVERT(VARCHAR,F910.YEAR), 103)) BETWEEN "
                + "CASE WHEN F10022 < CONVERT(DATETIME, '01/01/2011', 103) THEN CONVERT(DATETIME, '01/01/2011', 103) ELSE F10022 END AND getdate() "
                + "AND F72106 = F09802 "
                + "GROUP BY F910.F91003, F910." + salariu_i + ", F910.F91071, F91076, F91077 "
                + "HAVING F910.F91003=" + marca
                + " ORDER BY DATA, SALARIU, Max(F100.F10022) ");

                dtEmplMutations = General.IncarcaDT(temp, null);

                //if (lista.ContainsKey("REP_CODZILEABNEM") && lista["REP_CODZILEABNEM"].ToString() != String.Empty
                //    && lista.ContainsKey("REP_CODZILECFP") && lista["REP_CODZILECFP"].ToString() != String.Empty)
                //{
                //    string sql = "SELECT CASE WHEN SUM(" + lista["REP_CODZILEABNEM"].ToString().Replace("F200", "F920") + ") IS NOT NULL THEN "
                //               + "CONVERT(INT, SUM(" + lista["REP_CODZILEABNEM"].ToString().Replace("F200", "F920") + ")) ELSE 0 END AS CODZILEABNEM, "
                //               + "CASE WHEN SUM(" + lista["REP_CODZILECFP"].ToString().Replace("F200", "F920") + ") IS NOT NULL THEN "
                //               + "CONVERT(INT, SUM(" + lista["REP_CODZILECFP"].ToString().Replace("F200", "F920") + ")) ELSE 0 END AS CODZILECFP  FROM F920, F100 "
                //               + "WHERE F92003 = " + marca + " AND CONVERT(DATETIME, '01/' + right(replicate('0',2) + CONVERT(VARCHAR,F920.MONTH),2) + '/' + CONVERT(VARCHAR, F920.YEAR), 103) "                              
                //               + "BETWEEN CASE WHEN F10022 < CONVERT(DATETIME, '01/01/2011', 103) THEN CONVERT(DATETIME, '01/01/2011', 103) ELSE F10022 END AND getdate() AND F10003 = F92003 "
                //               + " UNION "
                //               + "SELECT CASE WHEN SUM(" + lista["REP_CODZILEABNEM"].ToString() + ") IS NOT NULL THEN "
                //               + "CONVERT(INT, SUM(" + lista["REP_CODZILEABNEM"].ToString() + ")) ELSE 0 END AS CODZILEABNEM, CASE WHEN "
                //               + "SUM(" + lista["REP_CODZILECFP"].ToString() + ") IS NOT NULL THEN CONVERT(INT, SUM(" + lista["REP_CODZILECFP"].ToString()
                //               + ")) ELSE 0 END AS CODZILECFP  FROM F200 WHERE F20003 = " + marca;

                //    dtEmplSum = General.IncarcaDT(sql, null);
                //}

                string sql = "SELECT cast(COALESCE( " +
                      "  (SELECT SUM(NEM) FROM ( " +
                      "      select " + lista["NEM"].ToString() + " AS NEM FROM F200 " +
                      "      INNER JOIN F100 ON F200.F20003=F100.F10003 " +
                      "      where F20003 = " + marca +
                      "      union ALL " +
                      "      select " + lista["NEM"].ToString().Replace("F200", "F920") + " as NEM FROM F920  " +
                      "      INNER JOIN F100 on F920.F92003=F100.F10003 " +
                      "      WHERE F92003 = " + marca + ") A) , 0) AS INT ) CODZILEABNEM," +
                      "  CAST(COALESCE(" +
                      "  (SELECT SUM(CFP) FROM" +
                      "      (select " + lista["CFP"].ToString() + " AS CFP FROM F200 " +
                      "      INNER JOIN F100 ON F200.F20003=F100.F10003   " +
                      "      where F20003 = " + marca +
                      "      union ALL " +
                      "      select " + lista["CFP"].ToString().Replace("F200", "F920") + " AS CFP FROM F920  " +
                      "      INNER JOIN F100 on F920.F92003=F100.F10003 " +
                      "      WHERE F92003= " + marca + ") A), 0) AS INT) CODZILECFP";
                dtEmplSum = General.IncarcaDT(sql, null);
            }

            string XMLFile = "";
            using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_vechime_sablon.xml")))
            {
                String line = sr.ReadLine();

                while (line != null)
                {
                    XMLFile += line;
                    line = sr.ReadLine();
                }
            }

            XMLFile = XMLFile.Replace("[CO_NAME]", dtCoAdress.Rows[0]["CONAME"].ToString());
            XMLFile = XMLFile.Replace("[CO_ADRESA]", coaddress);
            XMLFile = XMLFile.Replace("[CO_CODFISCAL]", dtCompany.Rows[0][1].ToString());
            XMLFile = XMLFile.Replace("[CO_FISC]", dtCoAdress.Rows[0]["COFISC"].ToString());
            XMLFile = XMLFile.Replace("[CO_TELEFON]", dtCoAdress.Rows[0]["TELEFON"].ToString());


            if (dtEmpl != null && dtEmpl.Rows.Count > 0)
            {
                DataTable currentMonth = null;
                currentMonth = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);                              

                string month = currentMonth.Rows[0]["F01012"].ToString();
                string year = currentMonth.Rows[0]["F01011"].ToString();

                month = (Convert.ToInt32(month) + 1).ToString();
                if (month == "13")
                {
                    month = "01";
                    year = (Convert.ToInt32(year) + 1).ToString();
                }
                string sql = "";
                if (Constante.tipBD == 2)
                {
                    if (rbFunc2.Checked)
                    {
                        sql = "SELECT F71804 FROM F718, F704 WHERE F71802 = F70407 AND F70404 = 2 AND F70406 <= "
                        + "SYSDATE "
                        + "AND F70406 >= TO_DATE('01/'||LPAD(TO_CHAR(" + month
                           + "), 2, '0')||'/'||TO_CHAR(" + year + "), 'dd/MM/yyyy') AND F70403 = " + marca + "";
                    }
                    else
                    {
                        sql = "SELECT F72204 FROM F722, F704 WHERE F72206 = case when F70406 < TO_DATE('01/01/2012', 'dd/mm/yyyy') then 5 else case when F70406 < TO_DATE('17/04/2016', 'dd/mm/yyyy') THEN 6 ELSE 7 END END AND F72202 = F70407 AND F70404 = 3 AND F70406 <= "
                        + "SYSDATE "
                        + "AND F70406 >= TO_DATE('01/'||LPAD(TO_CHAR(" + month
                           + "), 2, '0')||'/'||TO_CHAR(" + year + "), 'dd/MM/yyyy') AND F70403 = " + marca + "";
                    }                    
                }
                else 
                {
                    
                    if (rbFunc2.Checked)
                    {
                        sql = "SELECT F71804 FROM F718, F704 WHERE F71802 = F70407 AND F70404 = 2 AND F70406 <= "
                            + "getdate() "
                            + "AND F70406 >= CONVERT(DATETIME,'01/' + RIGHT(REPLICATE('0',2) + CONVERT(VARCHAR, " + month
                               + "), 2) + '/'  + CONVERT(VARCHAR, " + year + "), 103) AND F70403 = " + marca + "";
                    }
                    else
                    {
                        sql = "SELECT F72204 FROM F722, F704 WHERE F72206 = case when F70406 < CONVERT(DATETIME, '01/01/2012', 103) then 5 else case when F70406 < CONVERT(DATETIME, '17/04/2016', 103) THEN 6 ELSE 7 END END AND F72202 = F70407 AND F70404 = 3 AND F70406 <= "
                            + "getdate() "
                            + "AND F70406 >= CONVERT(DATETIME,'01/' + RIGHT(REPLICATE('0',2) + CONVERT(VARCHAR, " + month
                               + "), 2) + '/'  + CONVERT(VARCHAR, " + year + "), 103) AND F70403 = " + marca + "";
                    }                    
                }
                dtEmplFunction704 = General.IncarcaDT(sql, null);

                XMLFile = XMLFile.Replace("[SEX]", (dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "d-na " : "dl " ));
                XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["NUME"].ToString());
                XMLFile = XMLFile.Replace("[SEX_DOMICILIU]", (dtEmpl.Rows[0]["SEX"].ToString() == "2" ? " domiciliată în " : " domiciliat în "));

                string ang_adresa = "";
                if (dtEmpl.Rows[0]["DOMICILIU"].ToString().Length > 0 && dtEmpl.Rows[0]["DOMICILIU"].ToString() != "0")                
                    ang_adresa += dtEmpl.Rows[0]["DOMICILIU"].ToString() + ", ";                
                if (dtEmpl.Rows[0]["STR"].ToString().Length > 0 && dtEmpl.Rows[0]["STR"].ToString() != "0")                
                    ang_adresa += "str. " + dtEmpl.Rows[0]["STR"].ToString() + ", ";                
                if (dtEmpl.Rows[0]["NR"].ToString().Length > 0 && dtEmpl.Rows[0]["NR"].ToString() != "0")                
                    ang_adresa += "nr. " + dtEmpl.Rows[0]["NR"].ToString() + ", ";
                if (dtEmpl.Rows[0]["BL"].ToString().Length > 0 && dtEmpl.Rows[0]["BL"].ToString() != "0")                
                    ang_adresa += "bl. " + dtEmpl.Rows[0]["BL"].ToString() + ", ";
                if (dtEmpl.Rows[0]["AP"].ToString().Length > 0 && dtEmpl.Rows[0]["AP"].ToString() != "0")                
                    ang_adresa += "ap. " + dtEmpl.Rows[0]["AP"].ToString() + ", ";
                if (dtEmpl.Rows[0]["JUDET"].ToString().Length > 0 && dtEmpl.Rows[0]["JUDET"].ToString() != "0")
                {
                    if (dtEmpl.Rows[0]["JUDET"].ToString().ToUpper().Contains("BUCURESTI"))                    
                        ang_adresa += "sector " + dtEmpl.Rows[0]["SECTOR"].ToString() + ", ";
                    else                    
                        ang_adresa += "judeţul " + dtEmpl.Rows[0]["JUDET"].ToString() + ", ";
                }

                XMLFile = XMLFile.Replace("[ADRESA_ANGAJAT]", ang_adresa);
                XMLFile = XMLFile.Replace("[SEX_AI]", (dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "posesoare a " : "posesor al "));
                XMLFile = XMLFile.Replace("[AI]", dtEmpl.Rows[0]["AI"].ToString());
                XMLFile = XMLFile.Replace("[AI_SN]", dtEmpl.Rows[0]["SN_AI"].ToString());
                XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["CNP"].ToString());                        


                XMLFile = XMLFile.Replace("[ANGAJAT_PREZ_TRECUT]", (dtEmpl.Rows[0]["F10025"].ToString() == "0" ? " este " : " a fost "));

                string angajare = "";
                if (dtCoAdress != null && dtCoAdress.Rows.Count > 0)
                {
                    angajare = dtEmpl.Rows[0]["SEX"].ToString() == "2" ? " angajata " : " angajatul ";
                    angajare += "societății " + dtCoAdress.Rows[0]["CONAME"].ToString() + ", CUI ";
                    angajare += dtCoAdress.Rows[0]["CUI"].ToString();
                    angajare += ", cu sediul social în ";
                    angajare += coaddress;
                }
                XMLFile = XMLFile.Replace("[ANGAJARE]", angajare);
                XMLFile = XMLFile.Replace("[NORMA_ANGAJAT_1]", (dtEmpl.Rows[0]["NORMA"].ToString() == "8" ? "cu normă întreagă de " : "cu fracțiune de normă de "));
                XMLFile = XMLFile.Replace("[NORMA_ANGAJAT_2]", dtEmpl.Rows[0]["NORMA"].ToString());
                XMLFile = XMLFile.Replace("[DURATA_ANGAJAT]", dtEmpl.Rows[0]["DURATA"].ToString());
                XMLFile = XMLFile.Replace("[NRINSPECTORAT]", dtEmpl.Rows[0]["NRINSPECTORAT"].ToString());

                string functie = "";
                if (dtEmplFunction704 != null && dtEmplFunction704.Rows.Count > 0)
                    functie = dtEmplFunction704.Rows[0][0].ToString();
                else
                    if (dtEmplFunction != null && dtEmplFunction.Rows.Count > 0)
                        functie = dtEmplFunction.Rows[0][0].ToString();

                XMLFile = XMLFile.Replace("[FUNCTIE_ANGAJAT]", functie);

                DateTime dtA = new DateTime(Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmpl.Rows[0]["EMPL_DATANG"].ToString().Substring(0, 2)));
                //dtA = DateTime.Parse(dtEmpl.Rows[0]["EMPL_DATANG"].ToString());

                string start_contract = "";
                if (dtA >= new DateTime(2011, 1, 1))
                {
                    start_contract = "Începând cu data de ";
                    start_contract += dtA.Day.ToString().PadLeft(2, '0') + "." + dtA.Month.ToString().PadLeft(2, '0') + "." + dtA.Year.ToString();
                    start_contract += " pe durata executării contractului individual de muncă";
                }
                else
                    start_contract ="De la data de 01.01.2011";

                XMLFile = XMLFile.Replace("[START_CONTRACT]", start_contract);


                string tableRow = "<w:tr w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidTr=\"00AF1CA4\"><w:tc><w:tcPr><w:tcW w:w=\"800\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" " 
                    + " w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts " 
                    + " w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t>[NR_CRT]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2000\" " 
                    + " w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" " 
                    + " w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:proofErr " 
                    + " w:type=\"spellStart\"/><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t>[MUTATIA]</w:t></w:r><w:proofErr " 
                    + " w:type=\"spellEnd\"/></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1100\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" " 
                    + " w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t>[DATA_MODIF]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" " 
                    + " w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts " 
                    + " w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t>[MESERIE]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1700\" w:type=\"dxa\"/><w:shd w:val=\"clear\" " 
                    + " w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts " 
                    + " w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t>[SALARIU]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"1700\" w:type=\"dxa\"/><w:shd w:val=\"clear\" " 
                    + " w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AF1CA4\" w:rsidRPr=\"00AF1CA4\" w:rsidRDefault=\"00AF1CA4\" w:rsidP=\"00AF1CA4\"><w:pPr><w:spacing w:after=\"0\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" " 
                    + " w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" " 
                    + " w:cs=\"Times New Roman\"/><w:sz w:val=\"18\"/></w:rPr><w:t xml:space=\"preserve\">[NR_DATA_ACT]</w:t></w:r>  </w:p></w:tc></w:tr>";

                string nrCrt = " ", mutatia = " ", dataModif = " ", meserie = " ", salariu = " ", nrDataAct = " ";
                string table = "";

                string[] motivIntr = new string[2];
                if (dtEmplMutations != null && dtEmplMutations.Rows.Count > 0)
                {
                    motivIntr = dtEmplMutations.Rows[0]["MOTIVINTR"].ToString().Split(':');
                    if (motivIntr[0] == "---")
                    {
                        dtEmplMutations = new DataTable();
                        sql = "";
                        sql = "SELECT MAX(F098.F09804) AS MOTIVINTR FROM F098, F721, F704 WHERE F70403 = " + marca
                            + " AND F70404 = 4 AND F70407 = F72102 AND F72106 = F09802";
                        dtEmplMutations = General.IncarcaDT(sql, null);      
                        if (dtEmplMutations != null && dtEmplMutations.Rows.Count > 0)
                        {
                            motivIntr = dtEmplMutations.Rows[0]["MOTIVINTR"].ToString().Split(':');
                        }
                    }
                }

                dtEmplMutations = RapoarteRevisal(marca.ToString(), rbFunc2.Checked, lista);

                if (dtEmplMutations != null && dtEmplMutations.Rows.Count > 0)
                {
                    string dataANG = dtEmplMutations.Rows[0]["DATANG"].ToString();
                    DateTime dtAng = new DateTime(Convert.ToInt32(dataANG.Substring(6, 4)), Convert.ToInt32(dataANG.Substring(3, 2)), Convert.ToInt32(dataANG.Substring(0, 2)));


                    if (dtAng < new DateTime(2011, 1, 1))
                        dataStart = "01/01/2011";
                    else
                        dataStart = dataANG;  

                    nrCrt = "1";

                    if (dtAng >= new DateTime(2011, 1, 1))
                    {
                        mutatia = "Angajat pe perioadă " + dtEmplMutations.Rows[0]["Tip durata"].ToString() + "ă";
                        dataModif = (dataANG != String.Empty && dataANG.Length == 10 ?
                            dataANG.Substring(0, 2) + "/" + dataANG.Substring(3, 2) + "/" + dataANG.Substring(6, 4) : dataANG);
                    }
                    else
                    {
                        if (dtAng < new DateTime(2011, 1, 1))
                        {
                            mutatia = "Continuă activitatea în baza aceluiași contract individual de muncă";
                            dataModif = "01/01/2011";
                        } 
                    }

                    meserie = dtEmplMutations.Rows[0]["FUNCTIA"].ToString();
                    salariu = dtEmplMutations.Rows[0]["SALARIU"].ToString();
                    nrDataAct = "Contract individual de muncă nr. " + dtEmplMutations.Rows[0]["Nr./Data contract"].ToString();

                    table += tableRow;
                    table = table.Replace("[NR_CRT]", nrCrt);
                    table = table.Replace("[MUTATIA]", mutatia);
                    table = table.Replace("[DATA_MODIF]", dataModif);
                    table = table.Replace("[MESERIE]", meserie);
                    table = table.Replace("[SALARIU]", salariu);
                    table = table.Replace("[NR_DATA_ACT]", nrDataAct);

                    string sal = dtEmplMutations.Rows[0]["SALARIU"].ToString();
                    string func = dtEmplMutations.Rows[0]["FUNCTIA"].ToString();
                    string norm = dtEmplMutations.Rows[0]["NORMA"].ToString();

                    for (int i = 1; i < dtEmplMutations.Rows.Count; i++)
                    {
                        DateTime dtData = new DateTime(Convert.ToInt32(dtEmplMutations.Rows[i]["DATA"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[i]["DATA"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[i]["DATA"].ToString().Substring(0, 2)));
                        DateTime dtStart = new DateTime(Convert.ToInt32(dataStart.Substring(6, 4)), Convert.ToInt32(dataStart.Substring(3, 2)), Convert.ToInt32(dataStart.Substring(0, 2)));
                        DateTime dtSfarsit = new DateTime(Convert.ToInt32(dataSf.Substring(6, 4)), Convert.ToInt32(dataSf.Substring(3, 2)), Convert.ToInt32(dataSf.Substring(0, 2)));

                        if (dtData < dtStart || dtData > dtSfarsit)
                            continue;

                        table += tableRow;

                       nrCrt = (i + 1).ToString();

                        if (dtEmplMutations.Rows[i]["SALARIU"].ToString() == "0")
                        {
                            mutatia = "Suspendat CIM conform art.51, lit.a";
                        }
                        else
                        {
                            if (dtEmplMutations.Rows[i - 1]["SALARIU"].ToString() == "0")
                            {
                                mutatia = "Încetat suspendarea și reluat activitatea în baza aceluiași CIM";
                            }
                            else
                            {
                                string text = "Prelungire contract";
                                if (sal != dtEmplMutations.Rows[i]["SALARIU"].ToString())
                                    text = "Modificare de salariu";
                                if (func != dtEmplMutations.Rows[i]["FUNCTIA"].ToString())
                                    text = "Modificare de funcție";
                                if (norm != dtEmplMutations.Rows[i]["NORMA"].ToString())
                                    text = "Modificare de normă";
                                mutatia = text;
                            }
                        }

                        dataModif = dtEmplMutations.Rows[i]["DATA"].ToString();
                        meserie = dtEmplMutations.Rows[i]["FUNCTIA"].ToString();
                        salariu = dtEmplMutations.Rows[i]["SALARIU"].ToString();
                        nrDataAct = dtEmplMutations.Rows[i]["Nr./Data contract"].ToString();

                        sal = dtEmplMutations.Rows[i]["SALARIU"].ToString();
                        func = dtEmplMutations.Rows[i]["FUNCTIA"].ToString();
                        norm = dtEmplMutations.Rows[i]["NORMA"].ToString();

                        table = table.Replace("[NR_CRT]", nrCrt);
                        table = table.Replace("[MUTATIA]", mutatia);
                        table = table.Replace("[DATA_MODIF]", dataModif);
                        table = table.Replace("[MESERIE]", meserie);
                        table = table.Replace("[SALARIU]", salariu);
                        table = table.Replace("[NR_DATA_ACT]", nrDataAct);

                    }

                    if (dtEmplMutations.Rows[0]["DATASF"].ToString() != String.Empty
                        && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2))) 
                        >= new DateTime(Convert.ToInt32(dataStart.Substring(6, 4)), Convert.ToInt32(dataStart.Substring(3, 2)), Convert.ToInt32(dataStart.Substring(0, 2))) 
                        && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2)))
                        <= new DateTime(Convert.ToInt32(dataSf.Substring(6, 4)), Convert.ToInt32(dataSf.Substring(3, 2)), Convert.ToInt32(dataSf.Substring(0, 2))))
                    {
                        if (motivIntr[0] == null)
                        {
                            DataTable dt = new DataTable();
                            sql = "";
                            sql = "SELECT MAX(F098.F09804) AS MOTIVINTR FROM F098, F721, F704 WHERE F70403 = " + marca
                                + " AND F70404 = 4 AND F70407 = F72102 AND F72106 = F09802";
                            dt = General.IncarcaDT(sql, null);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                motivIntr = dt.Rows[0]["MOTIVINTR"].ToString().Split(':');
                            }
                        }

                        table += tableRow;

                        nrCrt = (dtEmplMutations.Rows.Count + 1).ToString();
                        mutatia = "Încetat CIM cf. " + (motivIntr[0] != "" ? motivIntr[0] : "Art. 55 lit. b") + " din Codul Muncii";

                        string dataSF = dtEmplMutations.Rows[dtEmplMutations.Rows.Count - 1]["DATASF"].ToString();
                        dataModif = (dataSF != String.Empty && dataSF.Length == 10 ?
                                                      dataSF.Substring(0, 2) + "/" + dataSF.Substring(3, 2) + "/" + dataSF.Substring(6, 4) : dataSF);

                        meserie = dtEmplMutations.Rows[dtEmplMutations.Rows.Count - 1]["FUNCTIA"].ToString();
                        salariu = dtEmplMutations.Rows[dtEmplMutations.Rows.Count - 1]["SALARIU"].ToString();
                        nrDataAct = "Decizia din data " + (dataSF != String.Empty && dataSF.Length == 10 ?
                                                        dataSF.Substring(0, 2) + "/" + dataSF.Substring(3, 2) + "/" + dataSF.Substring(6, 4) : dataSF);

                        table = table.Replace("[NR_CRT]", nrCrt);
                        table = table.Replace("[MUTATIA]", mutatia);
                        table = table.Replace("[DATA_MODIF]", dataModif);
                        table = table.Replace("[MESERIE]", meserie);
                        table = table.Replace("[SALARIU]", salariu);
                        table = table.Replace("[NR_DATA_ACT]", nrDataAct);
                    }
                }

                XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);

                string incetareCtr = "";

                if (dtEmplMutations.Rows[0]["DATASF"].ToString() != String.Empty
                    && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2)))
                    >= new DateTime(Convert.ToInt32(dataStart.Substring(6, 4)), Convert.ToInt32(dataStart.Substring(3, 2)), Convert.ToInt32(dataStart.Substring(0, 2)))
                    && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2)))
                    <= new DateTime(Convert.ToInt32(dataSf.Substring(6, 4)), Convert.ToInt32(dataSf.Substring(3, 2)), Convert.ToInt32(dataSf.Substring(0, 2))))
                {                    
                    incetareCtr = "Începând cu data de " + dtEmplMutations.Rows[0]["DATASF"].ToString() + " contractul individual de muncă  al ";
                    if (dtEmpl.Rows[0]["SEX"].ToString() == "2")                    
                        incetareCtr += "doamnei ";                    
                    else                    
                        incetareCtr += "domnului ";                    
                    string dataSF = dtEmplMutations.Rows[dtEmplMutations.Rows.Count - 1]["DATASF"].ToString();

                    incetareCtr += dtEmpl.Rows[0]["NUME"].ToString();
                    incetareCtr += " a încetat în baza prevederilor " + (motivIntr[0] != "" ? motivIntr[0] : "art. 55 lit. b") + " din Legea nr.53/2003-Codul Muncii, modificată și completată, ";
                    incetareCtr += "astfel cum rezultă din decizia din data " + (dataSF != String.Empty && dataSF.Length == 10 ? dataSF.Substring(0, 2) + "/" + dataSF.Substring(3, 2) + "/" + dataSF.Substring(6, 4) : dataSF) + " .";                                       
                }
                XMLFile = XMLFile.Replace("[INCETARE_CONTRACT]", incetareCtr);

                if (dtEmplSum != null && dtEmplSum.Rows.Count > 0)
                {
                    int nrZileAbNem = Convert.ToInt32(dtEmplSum.Rows[0]["CODZILEABNEM"].ToString());
                    int nrZileCFP = Convert.ToInt32(dtEmplSum.Rows[0]["CODZILECFP"].ToString());
                    if (dtEmplSum.Rows.Count > 1)
                    {
                        nrZileAbNem += Convert.ToInt32(dtEmplSum.Rows[1]["CODZILEABNEM"].ToString());
                        nrZileCFP += Convert.ToInt32(dtEmplSum.Rows[1]["CODZILECFP"].ToString());
                    }

                    XMLFile = XMLFile.Replace("[ZILE_ABS]", nrZileAbNem.ToString());
                    XMLFile = XMLFile.Replace("[ZILE_CFP]", nrZileCFP.ToString());
                }
                else
                {
                    XMLFile = XMLFile.Replace("[ZILE_ABS]", "0");
                    XMLFile = XMLFile.Replace("[ZILE_CFP]", "0");
                }

                string textIncetare = "";
                if (dtEmplMutations.Rows[0]["DATASF"].ToString() != String.Empty
                    && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2)))
                    >= new DateTime(Convert.ToInt32(dataStart.Substring(6, 4)), Convert.ToInt32(dataStart.Substring(3, 2)), Convert.ToInt32(dataStart.Substring(0, 2)))
                    && new DateTime(Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(6, 4)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(3, 2)), Convert.ToInt32(dtEmplMutations.Rows[0]["DATASF"].ToString().Substring(0, 2)))
                    <= new DateTime(Convert.ToInt32(dataSf.Substring(6, 4)), Convert.ToInt32(dataSf.Substring(3, 2)), Convert.ToInt32(dataSf.Substring(0, 2))))
                {
                    textIncetare = "În perioada de la .............. până la .............. a lucrat în grupa (I sau II de muncă), poziția nr. .......... "
                        + "din anexa la Ordinul nr. .......... din .............. al ministrului .............................., în total ..... ani ..... luni ..... zile "
                        + "( ...............................).";
                }
                XMLFile = XMLFile.Replace("[TEXT_INCETARE]", textIncetare);

                if (lista["FR1"].Length > 0)
                    XMLFile = XMLFile.Replace("[FUNC_REPR1]", lista["FR1"]);
                else
                    XMLFile = XMLFile.Replace("[FUNC_REPR1]", "");

                if (lista["FR2"].Length > 0)
                    XMLFile = XMLFile.Replace("[FUNC_REPR2]", lista["FR2"]);
                else
                    XMLFile = XMLFile.Replace("[FUNC_REPR2]", "");

                if (lista["NR1"].Length > 0)
                    XMLFile = XMLFile.Replace("[NUME_REPR1]", lista["NR1"]);
                else
                    XMLFile = XMLFile.Replace("[NUME_REPR1]", "");

                if (lista["NR2"].Length > 0)
                    XMLFile = XMLFile.Replace("[NUME_REPR2]", lista["NR2"]);
                else
                    XMLFile = XMLFile.Replace("[NUME_REPR2]", "");

                XMLFile = XMLFile.Replace("[DIMX]", lista["AdevDimX"]);
                XMLFile = XMLFile.Replace("[DIMY]", lista["AdevDimY"]);


                DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/"));
                FileInfo[] listfiles = root.GetFiles("Logo.*");
                string logo = "";
                if (listfiles.Length > 0)
                {
                    byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                    logo = Convert.ToBase64String(fileBytes);
                }
                XMLFile = XMLFile.Replace("[LOGO]", logo);
                
                StreamWriter sw = new StreamWriter(FileName, false);

                sw.Write(XMLFile);
                sw.Close();
                sw.Dispose();

            }
            else
                return;
        }


        private bool AdeverintaStagiu(int marca, string FileName)
        {

            bool err = true;

            DataTable dtCoasig = new DataTable();
            DataTable dtCompany = new DataTable();
          
            DataTable dtEmpl = new DataTable();
            DataTable dtCM = new DataTable();
            DataTable dtLC = new DataTable();
            DataTable dtVer = new DataTable();
            DataTable dtEmplSum = new DataTable();
            string luna_istoric_F940 = "";
            string dataStart = "";
            string dataSf = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;

            try
            {
                DataTable currentMonth = null;
                currentMonth = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
                double interval = 0;
                if (lista["INT"] == "24")
                    interval = 23.9;
                else
                    interval = 11.9;


                luna_istoric_F940 = "01/" + (int.Parse(currentMonth.Rows[0][0].ToString()) < 10 ? "0" + currentMonth.Rows[0][0].ToString() : currentMonth.Rows[0][0].ToString()) + "/" + currentMonth.Rows[0][1].ToString();

                if (Constante.tipBD == 2)
                {
                    luna_istoric_F940 = "TO_DATE('" + luna_istoric_F940 + "', 'DD/MM/YYYY') ";
                    luna_istoric_F940 = "TO_DATE('01/'||TO_CHAR(F940.MONTH)||'/'||TO_CHAR(F940.YEAR), 'dd/MM/yyyy') < " + luna_istoric_F940;
                }
                else
                {
                    luna_istoric_F940 = "CONVERT(DATETIME, '" + luna_istoric_F940 + "', 103)";
                    luna_istoric_F940 = "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F940.MONTH)+'/'+CONVERT(VARCHAR,F940.YEAR), 103) < " + luna_istoric_F940;
                }

                string month = currentMonth.Rows[0]["F01012"].ToString();
                string year = currentMonth.Rows[0]["F01011"].ToString();

                string data_luna = "";
                data_luna = "01/" + (int.Parse(month) < 10 ? "0" + month : month) + "/" + year;

                err = true;

                string sql = "";

                string filtru_coduri = "";
                if (lista.ContainsKey("CEX") && lista["CEX"].Length > 0)
                {
                    string coduri = lista["CEX"].Trim(' ');
                    string[] separators = { ",", ".", "_", "-", ";", ":", "+" };
                    string[] words = coduri.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        int n = 0;
                        if (int.TryParse(word, out n))
                        {
                            if (Constante.tipBD == 2)
                            {
                                filtru_coduri += " AND TO_NUMBER(F940607) <> ";
                            }
                            else
                            {
                                filtru_coduri += " AND CONVERT(INT, F940607) <> ";
                            }

                            filtru_coduri += n;
                        }
                    }
                }
                if (Constante.tipBD == 2)
                {
                    dtCompany = General.IncarcaDT("SELECT TRIM(f00204) AS NUME,  TRIM(f06304) AS CAS_ANG from f002, f063 where f06302 = f00284", null);
                }

                if (Constante.tipBD == 1)
                {
                    dtCompany = General.IncarcaDT("SELECT LTRIM(RTRIM(f00204)) AS NUME,  LTRIM(RTRIM(f06304)) AS CAS_ANG from f002, f063 where f06302 = f00284", null);
                }
                if (Constante.tipBD == 2)
                {
                    dtCompany = General.IncarcaDT("SELECT TRIM(f00204) AS NUME,  TRIM(f06304) AS CAS_ANG from f002, f063 where f06302 = f00284", null);

                    dtEmpl = General.IncarcaDT("SELECT F10003 AS MARCA, trim(F10008)||' '||trim(F10009) AS NUME, "
                        + "TRIM(F10017) AS CNP, "
                        + "F10047 AS SEX, "
                        + "TO_CHAR(F10022, 'dd/MM/yyyy') AS EMPL_DATANG, "
                        + "TO_CHAR(F100993, 'dd/MM/yyyy') AS EMPL_DATASF, "
                        + "F10025, "
                        + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                        + "TRIM(F10052) AS SN_AI, "
                        + "TRIM(F100521) AS ELIB_AI, "
                        + "TO_CHAR(F100522, 'dd/mm/yyyy') AS DATA_AI, "
                        //+ "trim(F10081) AS DOMICILIU, "
                        + "case when F10081 is null then "
                        + "case when F100907 is null then '' else "
                        + "'Comuna ' || TRIM(F100907) || ', Sat ' || TRIM(F100908) end else TRIM(F10081) end AS DOMICILIU, "
                        + "case when trim(F10083) is null or trim(F10083) = '0' or LENGTH(TRIM(F10083)) = 0 then '-' else  trim(F10083) end  AS STR, "
                        + "case when trim(F10084) is null or trim(F10084) = '0' or LENGTH(TRIM(F10084)) = 0 then '-' else  trim(F10084) end  AS NR, "
                        + "case when trim(F10085) is null or  trim(F10085) = '0' or LENGTH(TRIM(F10085)) = 0 then '-' else  trim(F10085) end  AS BL, "
                        + "case when trim(F10086) is null or trim(F10086) = '0' or LENGTH(TRIM(F10086)) = 0 then '-' else  trim(F10086) end  AS AP, "
                        + "trim(F100891) AS JUDET, "
                        + "trim(F10082) AS SECTOR , F1009741 "
                        + " FROM F100 WHERE F10003=" + marca, null);



                    sql = "select distinct diff, f940607, f300607, DATA from ("
                            + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                            + "(select sum(diff) as diff, f940607, NULL AS DATA "
                            + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                            + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))<" + interval.ToString(new CultureInfo("en-US"))
                            + filtru_coduri
                            + " AND " + luna_istoric_F940   // mihad 02.03.2018
                            + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                            + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                            + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449"
                            + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, "
                            + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b  "
                            + "where f940607 = f300607(+) "
                            + "union all "
                            + "select (nvl(a.diff, 0) + nvl(b. diff, 0)) as diff, f940607, f300607, b.DATA from "
                            + "(select sum(diff) as diff, f940607, NULL AS DATA "
                            + "from (SELECT nvl(Min(F94038)-Max(f94037)+1, 0) as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 "
                            + "AND F940.F94010<4449 AND months_between(TO_DATE('01/'||F01012||'/'||F01011, 'dd/MM/yyyy'), TO_DATE('01/'||MONTH||'/'||YEAR, 'dd/MM/yyyy'))< " + interval.ToString(new CultureInfo("en-US"))
                            + filtru_coduri
                            + " AND " + luna_istoric_F940   // mihad 02.03.2018
                            + "GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) group by f940607 order by f940607) a, "
                            + "( select sum(diff) as diff, f300607, TO_CHAR(MAX(F30038), 'dd/mm/yyyy')  AS DATA from  (SELECT nvl(Min(F30038)-Max(F30037)+1, 0) "
                            + "AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca + " AND F300.F30010>=4401 AND F300.F30010<4449"
                            + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, "
                            + "F30036, F30037, F30038, F300607) group by F300607 order by f300607) b "
                            + "where f940607(+) = f300607) c";

                    string sqlCoasig = "SELECT TRIM(F11010)||' '||TRIM(F11005) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + marca;

                    dtCM = General.IncarcaDT(sql, null);

                    dtLC = General.IncarcaDT("SELECT '01/'||LPAD(F01012, 2, '0')||'/'||F01011 AS LC from F010", null);

                    dtCoasig = General.IncarcaDT(sqlCoasig, null);

                }
                if (Constante.tipBD == 1)
                {
                    dtCompany = General.IncarcaDT("SELECT LTRIM(RTRIM(f00204)) AS NUME,  LTRIM(RTRIM(f06304)) AS CAS_ANG from f002, f063 where f06302 = f00284", null);

                    dtEmpl = General.IncarcaDT("SELECT F10003 AS MARCA, CAST(LTRIM(RTRIM(F10008)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F10009)) AS VARCHAR(256)) AS NUME, "
                        + "LTRIM(RTRIM(F10017)) AS CNP, "
                        + "F10047 AS SEX, "
                        + "CONVERT(VARCHAR, F10022, 103) AS EMPL_DATANG, "
                        + "CONVERT(VARCHAR,F100993, 103) AS EMPL_DATASF, "
                        + "F10025, "
                        + "CASE WHEN(F100983=1) THEN 'CI' ELSE 'BI' END AS AI, "
                        + "LTRIM(RTRIM(F10052)) AS SN_AI, "
                        + "LTRIM(RTRIM(F100521)) AS ELIB_AI, "
                        + "CONVERT(VARCHAR, F100522, 103) AS DATA_AI, "
                        //+ "LTRIM(RTRIM(F10081)) AS DOMICILIU, "
                        + "case when len(LTRIM(RTRIM(F10081))) = 0 then "
                        + "case when len(LTRIM(RTRIM(F100907))) = 0 then '' else "
                        + "'Comuna ' + LTRIM(RTRIM(F100907)) + ', Sat ' + LTRIM(RTRIM(F100908)) end else LTRIM(RTRIM(F10081)) end AS DOMICILIU, "

                        + "case when LTRIM(RTRIM(F10083)) is null or LTRIM(RTRIM(F10083)) = '0' or LEN(LTRIM(RTRIM(F10083))) = 0 then '-' else  LTRIM(RTRIM(F10083)) end  AS STR, "
                        + "case when LTRIM(RTRIM(F10084)) is null or LTRIM(RTRIM(F10084)) = '0' or LEN(LTRIM(RTRIM(F10084))) = 0 then '-' else  LTRIM(RTRIM(F10084)) end  AS NR, "
                        + "case when LTRIM(RTRIM(F10085)) is null or LTRIM(RTRIM(F10085)) = '0' or LEN(LTRIM(RTRIM(F10085))) = 0 then '-' else  LTRIM(RTRIM(F10085)) end  AS BL, "
                        + "case when LTRIM(RTRIM(F10086)) is null or LTRIM(RTRIM(F10086)) = '0' or LEN(LTRIM(RTRIM(F10086))) = 0 then '-' else  LTRIM(RTRIM(F10086)) end  AS AP, "

                        + "LTRIM(RTRIM(F100891)) AS JUDET, "
                        + "LTRIM(RTRIM(F10082)) AS SECTOR, F1009741 "
                        + "FROM F100 WHERE F10003=" + marca, null);


                    sql = "select (ISNULL(a.diff, 0) + ISNULL(b. diff, 0)) as diff, f940607, f300607, b.DATA from (select sum(diff) as diff, f940607, NULL AS DATA from (SELECT ISNULL(DATEDIFF(DAY, Max(f94037), Min(F94038))+1, 0) "
                        + "as diff, F940607 FROM F940, F010 WHERE F94003 = " + marca + " AND F940.F94010>=4401 AND F940.F94010<4449 AND "
                        + "(DATEDIFF(DAY, CONVERT(DATETIME, '01/'+CONVERT(VARCHAR, MONTH)+'/'+CONVERT(VARCHAR, YEAR), 103), "
                        + "CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,F01012)+'/'+CONVERT(VARCHAR,F01011), 103) )/30.436875E) "
                        + "<" + interval.ToString(new CultureInfo("en-US"))
                        + filtru_coduri
                        + " AND " + luna_istoric_F940   // mihad 02.03.2018
                        + " GROUP BY F940.YEAR, F940.MONTH, F940.F940601, F940.F940602, F94036, F94037, F940607 ) t1 "
                        + "group by f940607 ) a "
                        + "full outer join	"
                        + "( select sum(diff) as diff, f300607, CONVERT(VARCHAR, MAX(F30038), 103) AS DATA from  (SELECT ISNULL(DATEDIFF(DAY, Max(F30037), Min(F30038))+1, 0) AS diff, F300607, F30038 FROM F300 WHERE F30003 = " + marca
                        + " AND F300.F30010>=4401 "
                        + "AND F300.F30010<4449"
                        + filtru_coduri.Replace("F940", "F300") + " GROUP BY F300.F300601, F300.F300602, F30036, F30037, F30038, F300607) t2 group by F300607 ) b  on f940607 = f300607 order by f300607";

                    string sqlCoasig = "SELECT CAST(LTRIM(RTRIM(F11010)) AS VARCHAR(256)) +CAST(' ' AS VARCHAR(256))+CAST(LTRIM(RTRIM(F11005)) AS VARCHAR(256)) AS NUME_COASIG, F11012 AS CNP_COASIG FROM F110 WHERE F11017 = 0 AND F11003=" + marca;

                    dtCoasig = General.IncarcaDT(sqlCoasig, null);

                    dtCM = General.IncarcaDT(sql, null);

                    dtLC = General.IncarcaDT("SELECT '01/'+CONVERT(VARCHAR, RIGHT(REPLICATE('0',2)+CAST(f01012 AS VARCHAR(2)),2))+'/'+CONVERT(VARCHAR, F01011) AS LC from F010", null);
                }

                DataTable dtCoAdress = new DataTable();
                string coaddress = "SELECT F00204, F00233, F00234, F00238, F00232, F00281, F00282, F00283, F00205, F00207 FROM F002, F100 WHERE F10002 = F00202 AND F10003 = " + marca;
                dtCoAdress = General.IncarcaDT(coaddress, null);
                coaddress = "";
                coaddress = dtCoAdress.Rows[0]["F00232"].ToString() +
                    (dtCoAdress.Rows[0]["F00233"].ToString().Length == 0 ? "" : ", Str. " + dtCoAdress.Rows[0]["F00233"].ToString())
                    + ((dtCoAdress.Rows[0]["F00234"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00234"].ToString() == "0") ? "" : ", Nr. " + dtCoAdress.Rows[0]["F00234"].ToString())
                    + ((dtCoAdress.Rows[0]["F00238"].ToString().Length == 0 || dtCoAdress.Rows[0]["F00238"].ToString() == "0") ? "" : ", Judet " + dtCoAdress.Rows[0]["F00238"].ToString())
                    ;

                DataTable dtIncetare = new DataTable();
                if (Constante.tipBD == 2)
                {

                    sql = "SELECT TO_CHAR(F70406, 'dd.mm.yyyy') AS F70406, F09805, TO_CHAR(F70406, 'dd/mm/yyyy') AS DI, F70406 AS ORDINE FROM F704, F098, F721 WHERE F70404=4 AND F70407 = F72102 AND F72106 = F09802 "

                        + "AND F70403 = " + marca + " ORDER BY ORDINE DESC";

                    dtIncetare = General.IncarcaDT(sql, null);

                }

                else if (Constante.tipBD == 1)
                {

                    sql = "SELECT CONVERT(VARCHAR, F70406, 104) AS F70406, F09805, CONVERT(VARCHAR, F70406, 103) AS DI, F70406 AS ORDINE FROM F704, F098, F721 WHERE F70404=4 AND F70407 = F72102 AND F72106 = F09802 "

                        + "AND F70403 = " + marca + " ORDER BY ORDINE DESC";

                    dtIncetare = General.IncarcaDT(sql, null);

                }

                string data_incetare = "";
                int nrluni = 12;

                if (dtIncetare != null && dtIncetare.Rows.Count > 0
                    && dtIncetare.Rows[0][0].ToString() != String.Empty)
                {
                    data_incetare = dtIncetare.Rows[0]["DI"].ToString();
                    nrluni = 12;
                }
                else
                    data_incetare = "";



                string XMLFile = "";
                using (StreamReader sr = new StreamReader(HostingEnvironment.MapPath("~/Adeverinta/Adev_stagiu_sablon.xml")))
                {
                    String line = sr.ReadLine();

                    while (line != null)
                    {
                        XMLFile += line;
                        line = sr.ReadLine();
                    }
                }


                XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCoAdress.Rows[0]["F00204"].ToString());
                XMLFile = XMLFile.Replace("[COD_FISCAL]", dtCoAdress.Rows[0]["F00207"].ToString());
                XMLFile = XMLFile.Replace("[COD_CAEN]", dtCoAdress.Rows[0]["F00205"].ToString());
                XMLFile = XMLFile.Replace("[ADRESA_ANGAJATOR]", coaddress);
                XMLFile = XMLFile.Replace("[TELEFON]", dtCoAdress.Rows[0]["F00281"].ToString());
                XMLFile = XMLFile.Replace("[FAX]", dtCoAdress.Rows[0]["F00282"].ToString());
                XMLFile = XMLFile.Replace("[E_MAIL_INTERNET]", dtCoAdress.Rows[0]["F00283"].ToString());

                // XMLFile = XMLFile.Replace("[NUME_ANGAJATOR]", dtCompany.Rows[0]["NUME"].ToString());
                XMLFile = XMLFile.Replace("[SEX_ANGAJAT]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "dna" : "dl");
                XMLFile = XMLFile.Replace("[NUME_ANGAJAT]", dtEmpl.Rows[0]["NUME"].ToString());
                XMLFile = XMLFile.Replace("[CNP]", dtEmpl.Rows[0]["CNP"].ToString());
                XMLFile = XMLFile.Replace("[DOMICILIU_ANGAJAT]", dtEmpl.Rows[0]["DOMICILIU"].ToString());
                XMLFile = XMLFile.Replace("[STRADA]", dtEmpl.Rows[0]["STR"].ToString());
                XMLFile = XMLFile.Replace("[NUMAR]", dtEmpl.Rows[0]["NR"].ToString());
                XMLFile = XMLFile.Replace("[BLOC]", dtEmpl.Rows[0]["BL"].ToString());
                XMLFile = XMLFile.Replace("[APARTAMENT]", dtEmpl.Rows[0]["AP"].ToString());
                XMLFile = XMLFile.Replace("[SECTOR_JUDET]", dtEmpl.Rows[0]["JUDET"].ToString().ToUpper().Contains("BUCURESTI") ? ", sector " : ", judeţul ");
                XMLFile = XMLFile.Replace("[SEC_JUD]", dtEmpl.Rows[0]["JUDET"].ToString().ToUpper().Contains("BUCURESTI") ? dtEmpl.Rows[0]["SECTOR"].ToString() : dtEmpl.Rows[0]["JUDET"].ToString());
                XMLFile = XMLFile.Replace("[SEX_ANGAJAT_2]", dtEmpl.Rows[0]["SEX"].ToString() == "2" ? "angajata" : "angajatul");
                XMLFile = XMLFile.Replace("[DURATA_CONTRACT]", dtEmpl.Rows[0]["F1009741"].ToString().Trim() == "1" ? "nedeterminată" : "determinată");
                XMLFile = XMLFile.Replace("[PERIOADA_CONTRACT]", dtEmpl.Rows[0]["EMPL_DATANG"].ToString() + " - " + data_incetare);


                string f1_uri_zile_stagiu = "";

                if (lista.ContainsKey("PP") && lista["PP"].Length > 0) f1_uri_zile_stagiu = lista["PP"].Trim(' ').ToUpper();
                if (lista.ContainsKey("NN") && lista["NN"].Length > 0) f1_uri_zile_stagiu += " + " + lista["NN"].Trim(' ').ToUpper();
                if (lista.ContainsKey("ZAMBP") && lista["ZAMBP"].Length > 0) f1_uri_zile_stagiu += " + " + lista["ZAMBP"].Trim(' ').ToUpper();
                if (lista.ContainsKey("ZAMBP2") && lista["ZAMBP2"].Length > 0) f1_uri_zile_stagiu += " + " + lista["ZAMBP2"].Trim(' ').ToUpper();

                string f1_uri_zile_CO = "";

                if (lista.ContainsKey("PP") && lista["PP"].Length > 0) f1_uri_zile_CO = lista["PP"].Trim(' ').ToUpper();
                if (lista.ContainsKey("ZAMBP") && lista["ZAMBP"].Length > 0) f1_uri_zile_CO += " + " + lista["ZAMBP"].Trim(' ').ToUpper();
                if (lista.ContainsKey("ZAMBP2") && lista["ZAMBP2"].Length > 0) f1_uri_zile_CO += " + " + lista["ZAMBP2"].Trim(' ').ToUpper();

                string f5_uri_suma_stagiu = "";
                if (lista.ContainsKey("BCM") && lista["BCM"].Trim(' ').ToUpper().Length > 0) f5_uri_suma_stagiu = lista["BCM"].Trim(' ').ToUpper();

                string f5_uri_suma_stagiu_2018 = f5_uri_suma_stagiu.Replace("F20004231", "F20004309");

                String campuri = "";
                string[] sep = { ",", ".", "_", "-", ";", ":", "+" };
                string[] w = f1_uri_zile_stagiu.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                int cn = 0;
                campuri = "";

                foreach (var word in w)
                {
                    if (cn == 0)
                        campuri = "COALESCE(" + word + ",0)";
                    else
                        campuri += "+" + "COALESCE(" + word + ",0)";
                    cn++;
                }

                f1_uri_zile_stagiu = campuri;

                w = f1_uri_zile_CO.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                cn = 0;
                campuri = "";

                foreach (var word in w)
                {
                    if (cn == 0)
                        campuri = "COALESCE(" + word + ",0)";
                    else
                        campuri += "+" + "COALESCE(" + word + ",0)";
                    cn++;
                }

                f1_uri_zile_CO = campuri;

                DataTable dsStagiu6_10 = new DataTable();
                if (Constante.tipBD == 2)
                {
                    sql = "Select round(sum(zile),0) as zile,"
                        + "     substr('IANUARIE    FEBRUARIE   MARTIE      APRILIE     MAI         IUNIE       IULIE       AUGUST      SEPTEMBRIE  OCTOMBRIE   NOIEMBRIE   DECEMBRIE    ', (max(luna)*12)-11,11) luna, "
                        + "     max(anul) anul, round(sum(suma),0) as suma, round(sum(zco),0) as zco, max(luna) as luna1 "
                        + "from"
                        + "	(";
                }
                else
                {

                    sql = "Select cast(sum(zile) as decimal(6,0)) as zile,"
                            + "     substring('IANUARIE    FEBRUARIE   MARTIE      APRILIE     MAI         IUNIE       IULIE       AUGUST      SEPTEMBRIE  OCTOMBRIE   NOIEMBRIE   DECEMBRIE    ', (max(luna)*12)-11,11) luna, "
                            + "     max(anul) anul, cast(sum(suma) as decimal(9,0)) as suma, cast(sum(zco) as decimal(3,0)) as zco, max(luna) as luna1 "
                            + "from"
                            + "	(";
                }
                if (String.Compare(data_incetare, "", true) == 0 || data_incetare.Substring(3).CompareTo(data_luna.Substring(3)) >= 0)
                {
                    if (Constante.tipBD == 2)
                    {
                        sql += "SELECT F01012 AS LUNA, F01011 AS ANUL, case when F01011 < 2018 then " + f5_uri_suma_stagiu
                            + " else " + f5_uri_suma_stagiu_2018 + " end AS SUMA, "
                            + f1_uri_zile_stagiu + " AS ZILE, "
                            + f1_uri_zile_CO + " AS ZCO "
                            + "	 FROM F010, f200"
                            + "	 WHERE F20002 = F01002 AND F20003 = " + marca
                            + "	 UNION";
                    }
                    else
                    {
                        sql += "SELECT F01012 AS LUNA, F01011 AS ANUL, case when F01011 < 2018 then " + f5_uri_suma_stagiu
                            + " else " + f5_uri_suma_stagiu_2018 + " end AS SUMA, "
                            + f1_uri_zile_stagiu + " AS ZILE, "
                            + f1_uri_zile_CO + " AS ZCO "
                            + "	 FROM F010, f200"
                            + "	 WHERE F20002 = F01002 AND F20003 = " + marca
                            + "	 UNION";
                    }
                }
                if (Constante.tipBD == 2)
                {
                    sql += "	 SELECT DISTINCT LUNA, ANUL, round(SUMA,0) as SUMA, ZILE, round(ZCO,0) as zco"
                    + "	 FROM"
                    + "		(SELECT ROW_NUMBER() OVER (ORDER BY F920.YEAR DESC, F920.MONTH DESC) AS NO,  "
                    + "			F920.MONTH AS LUNA, F920.YEAR AS ANUL, case when year < 2018 then " + f5_uri_suma_stagiu.Replace("F200", "F920")
                    + " else " + f5_uri_suma_stagiu_2018.Replace("F200", "F920") + " end AS SUMA, "
                    + f1_uri_zile_stagiu.Replace("F200", "F920") + " AS ZILE, "
                    + f1_uri_zile_CO.Replace("F200", "F920") + " AS ZCO "
                    + "		 FROM F920"
                    + "		 WHERE F92003 = " + marca
                    + "		 )tab)"
                    + "tab1"
                    //+ " where TO_DATE('01/'||TO_CHAR(luna)||'/'||TO_CHAR(anul), 'dd/MM/yyyy')   <= " + luna_ist
                    + " group by anul, luna order by anul, luna1 ";
                    dsStagiu6_10 = General.IncarcaDT(sql, null);
                }
                else
                {
                    sql += "	 SELECT DISTINCT LUNA, ANUL, cast(SUMA as decimal(9,0)) as SUMA, ZILE, cast(ZCO as decimal(3,0)) as zco"
                    + "	 FROM"
                    + "		(SELECT ROW_NUMBER() OVER (ORDER BY F920.YEAR DESC, F920.MONTH DESC) AS NO,  "
                    + "			F920.MONTH AS LUNA, F920.YEAR AS ANUL, case when year < 2018 then " + f5_uri_suma_stagiu.Replace("F200", "F920")
                    + " else " + f5_uri_suma_stagiu_2018.Replace("F200", "F920") + " end AS SUMA, "
                    + f1_uri_zile_stagiu.Replace("F200", "F920") + " AS ZILE, "
                    + f1_uri_zile_CO.Replace("F200", "F920") + " AS ZCO "
                    + "		 FROM F920"
                    + "		 WHERE F92003 = " + marca
                    + "		 )tab)"
                    + "tab1"
                    //+ " where DATEDIFF(day,DATEADD(month,1,DATEADD(year,-1," + luna_ist + ")),CONVERT(DATETIME, '01/'+CONVERT(VARCHAR,luna)+'/'+CONVERT(VARCHAR,anul), 103))>=0 "
                    + " group by anul, luna order by anul, max(luna) ";

                    dsStagiu6_10 = General.IncarcaDT(sql, null);
                }

                int intervalStagiu = 6;
                if (lista.ContainsKey("INS") && lista["INS"].Length > 0)
                    intervalStagiu = Convert.ToInt32(lista["INS"]);

                XMLFile = XMLFile.Replace("[NR_LUNI]", Math.Min(dsStagiu6_10.Rows.Count, intervalStagiu == 6 ? 6 : 12).ToString());


                string tableRow = "<w:tr w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" w:rsidTr=\"00AD1CED\"><w:tc><w:tcPr><w:tcW w:w=\"1600\" w:type=\"dxa\"/><w:shd w:val=\"clear\" "
                    + "w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" w:rsidRDefault=\"00AD1CED\" w:rsidP=\"00AD1CED\"><w:pPr><w:spacing w:after=\"0\" "
                    + "w:line=\"240\" w:lineRule=\"auto\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr>"
                    + "<w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[TABEL_AN]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW "
                    + "w:w=\"2000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" w:rsidRDefault=\"00AD1CED\" "
                    + "w:rsidP=\"00AD1CED\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                    + "w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr>"
                    + "<w:t xml:space=\"preserve\">[TABEL_LUNA]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr>"
                    + "<w:p w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" w:rsidRDefault=\"00AD1CED\" w:rsidP=\"00AD1CED\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/>"
                    + "<w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts "
                    + "w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[TABEL_ZILE]</w:t></w:r></w:p></w:tc><w:tc><w:tcPr><w:tcW w:w=\"2000\" w:type=\"dxa\"/>"
                    + "<w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" w:rsidRDefault=\"00AD1CED\" w:rsidP=\"00AD1CED\"><w:pPr>"
                    + "<w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/>"
                    + "</w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr><w:t>[TABEL_ZILE_CM]</w:t></w:r></w:p></w:tc><w:tc>"
                    + "<w:tcPr><w:tcW w:w=\"2000\" w:type=\"dxa\"/><w:shd w:val=\"clear\" w:color=\"auto\" w:fill=\"auto\"/></w:tcPr><w:p w:rsidR=\"00AD1CED\" w:rsidRPr=\"00AD1CED\" "
                    + "w:rsidRDefault=\"00AD1CED\" w:rsidP=\"00AD1CED\"><w:pPr><w:spacing w:after=\"0\" w:line=\"240\" w:lineRule=\"auto\"/><w:jc w:val=\"center\"/><w:rPr><w:rFonts "
                    + "w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" w:cs=\"Times New Roman\"/></w:rPr></w:pPr><w:r><w:rPr><w:rFonts w:ascii=\"Times New Roman\" w:hAnsi=\"Times New Roman\" "
                    + "w:cs=\"Times New Roman\"/></w:rPr><w:t>[TABEL_BAZA]</w:t></w:r></w:p></w:tc></w:tr>";

                string table = "";



                if (dsStagiu6_10 != null && dsStagiu6_10.Rows.Count > 0 && dsStagiu6_10.Rows[0][0].ToString() != String.Empty)
                {
                    for (int x = dsStagiu6_10.Rows.Count - 1; x >= dsStagiu6_10.Rows.Count - 1 - (Math.Min(intervalStagiu, dsStagiu6_10.Rows.Count) - 1); x--)
                    {
                        table += tableRow;
                        table = table.Replace("[TABEL_AN]", dsStagiu6_10.Rows[x]["anul"].ToString());
                        table = table.Replace("[TABEL_LUNA]", dsStagiu6_10.Rows[x]["luna"].ToString());
                        table = table.Replace("[TABEL_ZILE]", dsStagiu6_10.Rows[x]["zile"].ToString());
                        table = table.Replace("[TABEL_ZILE_CM]", dsStagiu6_10.Rows[x]["zco"].ToString());
                        table = table.Replace("[TABEL_BAZA]", dsStagiu6_10.Rows[x]["suma"].ToString());
                    }
                }
                else
                {
                    table += tableRow;
                    table = table.Replace("[TABEL_AN]", "————");
                    table = table.Replace("[TABEL_LUNA]", "");
                    table = table.Replace("[TABEL_ZILE]", "————");
                    table = table.Replace("[TABEL_ZILE_CM]", "");
                    table = table.Replace("[TABEL_BAZA]", "");
                }
                XMLFile = XMLFile.Replace("[TABLE_ROWS]", table);


                if (lista["FR1"].Length > 0)
                    XMLFile = XMLFile.Replace("[FUNC_REPR1]", lista["FR1"]);
                else
                    XMLFile = XMLFile.Replace("[FUNC_REPR1]", "");

                if (lista["FR2"].Length > 0)
                    XMLFile = XMLFile.Replace("[FUNC_REPR2]", lista["FR2"]);
                else
                    XMLFile = XMLFile.Replace("[FUNC_REPR2]", "");

                if (lista["NR1"].Length > 0)
                    XMLFile = XMLFile.Replace("[NUME_REPR1]", lista["NR1"]);
                else
                    XMLFile = XMLFile.Replace("[NUME_REPR1]", "");

                if (lista["NR2"].Length > 0)
                    XMLFile = XMLFile.Replace("[NUME_REPR2]", lista["NR2"]);
                else
                    XMLFile = XMLFile.Replace("[NUME_REPR2]", "");

                XMLFile = XMLFile.Replace("[DIMX]", lista["AdevDimX"]);
                XMLFile = XMLFile.Replace("[DIMY]", lista["AdevDimY"]);


                DirectoryInfo root = new DirectoryInfo(HostingEnvironment.MapPath("~/Adeverinta/"));
                FileInfo[] listfiles = root.GetFiles("Logo.*");
                string logo = "";
                if (listfiles.Length > 0)
                {
                    byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                    logo = Convert.ToBase64String(fileBytes);
                }
                XMLFile = XMLFile.Replace("[LOGO]", logo);


                listfiles = root.GetFiles("Subsol.*");
                string subsol = "";
                if (listfiles.Length > 0)
                {
                    byte[] fileBytes = File.ReadAllBytes(HostingEnvironment.MapPath("~/Adeverinta/" + listfiles[0].Name));
                    subsol = Convert.ToBase64String(fileBytes);
                }
                XMLFile = XMLFile.Replace("[SUBSOL]", subsol);

                StreamWriter sw = new StreamWriter(FileName, false);

                sw.Write(XMLFile);
                sw.Close();
                sw.Dispose();

            }
            catch (Exception ex)
            {

                return false;
            }

            return true;

        }


        private DataTable RapoarteRevisal(string marca, bool f, Dictionary<String, String> lista)
        {
            string salariu = "F100699";
            if (lista.ContainsKey("REVISAL_SAL") && lista["REVISAL_SAL"] != null && lista["REVISAL_SAL"].ToString() != string.Empty && lista["REVISAL_SAL"].ToString().Substring(0, 4) == "F100")
            {
                salariu = lista["REVISAL_SAL"].ToString().Replace(" ", "");
            }
            string salariu_i = salariu.Replace("F100", "F910");
            string luna = string.Empty;
            string an = string.Empty;

            string sqlEmpls = string.Empty;
            DataTable dtRezultat = new DataTable();

            string sqlCo = string.Empty;
            DataTable dtCo = new DataTable();

            string sqlEmpls2 = string.Empty;
            DataTable dtRezultat2 = new DataTable();

            string sqlEmpls3 = string.Empty;
            DataTable dtRezultat3 = new DataTable();

            string sqlEmpls4 = string.Empty;
            DataTable dtRezultat4 = new DataTable();

            string sqlContracte = string.Empty;
            DataTable dtContracte = new DataTable();

            string dataAng = "";
            DateTime dtAng = new DateTime(), dt2100 = new DateTime(2100, 1, 1), dtRevisal = new DateTime(2016, 4, 17);


            DataTable dtTmp = null;
            string sqltmp = "";

            if (Constante.tipBD == 2)            
                sqltmp = "SELECT COUNT(*) CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('TMP_ADEVERINTA')";            
            else            
                sqltmp = "SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('TMP_ADEVERINTA')";
            
            dtTmp = General.IncarcaDT(sqltmp, null);

            if (dtTmp.Rows[0]["CNT"].ToString() != "0")
            {
                sqltmp = "DROP TABLE TMP_ADEVERINTA";
                General.ExecutaNonQuery(sqltmp, null);
            }

            if (Constante.tipBD == 2)
            {
                sqltmp = "CREATE TABLE TMP_ADEVERINTA(LINIE NUMBER(3), MARCA NUMBER(9), CNP VARCHAR2(13), DATA_ANGAJARII DATE, NR_CONTRACT VARCHAR2(80), DATA_CONTRACT DATE, "
                    + "TIP_MODIFICARE VARCHAR2(50), VALOARE_MODIFICARE VARCHAR2(30), DATA_MODIFICARE DATE, INCEPUT_INTERVAL VARCHAR2(10), SFARSIT_INTERVAL VARCHAR2(10), FUNCTIE VARCHAR2(80), "
                    + "TIP_DURATA VARCHAR2(40), SALARIU VARCHAR2(10), NORMA VARCHAR2(2))";              
            }
            else 
            {
                sqltmp = "CREATE TABLE TMP_ADEVERINTA(LINIE NUMERIC(3), MARCA NUMERIC(9), CNP VARCHAR(13), DATA_ANGAJARII DATETIME, NR_CONTRACT VARCHAR(80), DATA_CONTRACT DATETIME, "
                    + "TIP_MODIFICARE VARCHAR(50), VALOARE_MODIFICARE VARCHAR(30), DATA_MODIFICARE DATETIME, INCEPUT_INTERVAL VARCHAR(10), SFARSIT_INTERVAL VARCHAR(10), FUNCTIE VARCHAR(80), "
                    + "TIP_DURATA VARCHAR(40), SALARIU VARCHAR(10), NORMA VARCHAR(2))";                
            }
            General.ExecutaNonQuery(sqltmp, null);

            string szsql = "DELETE FROM TMP_ADEVERINTA WHERE MARCA = '" + marca + "'";
            General.ExecutaNonQuery(szsql, null);

            DataTable dtFunctie = new DataTable();
            DataTable dtSpor = new DataTable();
            DataTable dtDoua_ctr = new DataTable();
            string func = string.Empty;
            string functie = string.Empty;
            string tip_ctr = string.Empty;
            string data = string.Empty;
            string data_a = string.Empty;
            string data_ctr = string.Empty;
            string sal = string.Empty;
            string norma = string.Empty;
            string tmodif = "Adaugare";
            string data_plecarii = "01/01/2100";
            string contract = string.Empty;
            string CNP = string.Empty;
            int linie = 0;
            int k_ctr = 0;

            DataTable dtDoua = new DataTable();

            string inregistru = string.Empty;
            inregistru = "INSERT INTO TMP_ADEVERINTA (LINIE, MARCA, CNP, DATA_ANGAJARII, NR_CONTRACT, DATA_CONTRACT, "
                + "TIP_MODIFICARE, VALOARE_MODIFICARE, DATA_MODIFICARE, INCEPUT_INTERVAL, SFARSIT_INTERVAL, FUNCTIE, TIP_DURATA, SALARIU, NORMA) ";

            sqlEmpls = "SELECT F910985, YEAR, MONTH FROM F910 WHERE F910985 IS NOT NULL AND ((YEAR >= 2011 AND MONTH >= 8) OR YEAR >= 2012) AND F91003 = " + marca + "  AND F91025 = 0 ORDER BY YEAR, MONTH";       // mihad 21.11.2017 - am adaugat  AND F91025 = 0 , pt cazul in care nu a fost niciodata angajat activ in istoric
            DataTable dtContract = null;
            dtContract = General.IncarcaDT(sqlEmpls, null);

            if (dtContract != null)
                if (dtContract.Rows.Count > 0)
                {
                    string an1 = string.Empty;
                    string luna1 = string.Empty;
                    string an_1 = string.Empty;
                    string luna_1 = string.Empty;
                    int an_ctr = 0;
                    int luna_ctr = 0;
                    int iteratii_j = 0;

                    for (int j = 0; j < dtContract.Rows.Count; j++)
                    {
                        while ((j + 1) < dtContract.Rows.Count)
                            if (dtContract.Rows[j]["F910985"].ToString().CompareTo(dtContract.Rows[j + 1]["F910985"].ToString()) == 0)
                                j++;
                            else
                                break;
                        contract = dtContract.Rows[j]["F910985"].ToString();

                        // inchid contractul precedent daca nu a fost inchis
                        iteratii_j++;
                        if (iteratii_j > 1)
                        {
                            if (Convert.ToInt32(luna1) < 12) luna1 = Convert.ToString(Convert.ToInt32(luna1) + 1);
                            else { an1 = Convert.ToString(Convert.ToInt32(an1) + 1); luna1 = "1"; }
                            if (Convert.ToInt32(luna1) < 10)
                                data = "01/0";
                            else
                                data = "01/";
                            data = data + luna1 + "/" + an1;
                            szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                                + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca;
                            General.ExecutaNonQuery(szsql, null);
                        }

                        if (Constante.tipBD == 2)
                            dataAng = "TO_CHAR(F91022, 'dd/mm/yyyy')";
                        else 
                            dataAng = "CONVERT(VARCHAR, F91022, 103)";


                        szsql = "SELECT MONTH AS LUNA, YEAR AS AN, " + dataAng + " AS DA, " + (f == true ? "F91071" : "F91098") + " AS FUNCTIE, " + (Constante.tipBD == 2 ? "TO_CHAR(" : "CONVERT(VARCHAR, " ) + salariu_i + ") AS SAL, F91043 AS NORMA FROM F910 WHERE F91003 = " + marca
                            + " AND ((YEAR >= 2011 AND MONTH >= 8) OR YEAR >= 2012) AND F910985 = '" + contract + "' AND F91025 = 0 ORDER BY YEAR, MONTH";

                        DataTable dtPrima = new DataTable();
                        dtPrima = General.IncarcaDT(szsql, null);
                        an = "2011"; luna = "8";
                        if (dtPrima != null)
                            if (dtPrima.Rows.Count > 0)
                            {
                                luna = dtPrima.Rows[0]["LUNA"].ToString();
                                an = dtPrima.Rows[0]["AN"].ToString();
                                sal = TryToParse(dtPrima.Rows[0]["SAL"].ToString());
                                functie = dtPrima.Rows[0]["FUNCTIE"].ToString();
                                norma = dtPrima.Rows[0]["NORMA"].ToString();

                                if (f == true)
                                {
                                    if (functie == "0")
                                        szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                                    else
                                        szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718 WHERE F71802 = " + functie;
                                }
                                else
                                {
                                    if (Convert.ToInt32(an) < 2012)
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 5 AND F72202 = " + functie;

                                    if (Convert.ToInt32(an) >= 2012 && (Convert.ToInt32(an) < 2016 || (Convert.ToInt32(an) == 2016 && Convert.ToInt32(luna) < 4)))
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 6 AND F72202 = " + functie;

                                    if (Convert.ToInt32(an) > 2016 || (Convert.ToInt32(an) == 2016 && Convert.ToInt32(luna) > 4))
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 7 AND F72202 = " + functie;

                                    if (Convert.ToInt32(an) == 2016 && Convert.ToInt32(luna) == 4)
                                    {
                                        dtAng = new DateTime(Convert.ToInt32(dtPrima.Rows[0]["DA"].ToString().Substring(6, 4)), Convert.ToInt32(dtPrima.Rows[0]["DA"].ToString().Substring(3, 2)), Convert.ToInt32(dtPrima.Rows[0]["DA"].ToString().Substring(0, 2)));
                                        if (dtAng < dtRevisal)
                                            szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 6 AND F72202 = " + functie;
                                        else
                                            szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 7 AND F72202 = " + functie;
                                    }
                                    //end Radu
                                }

                                dtFunctie = new DataTable();
                                dtFunctie = General.IncarcaDT(szsql, null);
                                functie = "";
                                if (dtFunctie != null)
                                    if (dtFunctie.Rows.Count > 0)
                                        functie = dtFunctie.Rows[0]["A"].ToString() + " " + dtFunctie.Rows[0]["B"].ToString();  

                                if (Constante.tipBD == 2)
                                {
                                    szsql = "SELECT (CASE WHEN MIN(F09506)<TO_DATE('01/01/2100', 'dd/MM/yyyy') THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP FROM F095 "
                                        + " WHERE F09503 = " + marca + " AND F09504 = '" + contract + "'";                                    
                                }
                                else 
                                {
                                    szsql = "SELECT (CASE WHEN MIN(F09506)<CONVERT(DATETIME, '01/01/2100', 103) THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP FROM F095 "
                                        + " WHERE F09503 = " + marca + " AND F09504 = '" + contract + "'";
                                }
                                dtFunctie = new DataTable();
                                dtFunctie = General.IncarcaDT(szsql, null);
                                tip_ctr = "Nedeterminata";
                                if (dtFunctie != null)                                    
                                    if (dtFunctie.Rows.Count > 0)
                                        tip_ctr = dtFunctie.Rows[0]["TIP"].ToString();

                                linie++;
                                string explicatii = "'' AS F100985, '' AS F100986,";
                                if (tmodif.CompareTo("Adaugare") == 0)
                                {
                                    if (Constante.tipBD == 2)
                                        data = "(CASE WHEN F91022<TO_DATE('01/08/2011', 'dd/MM/yyyy') THEN '01/08/2011' ELSE TO_CHAR(F91022, 'dd/MM/yyyy') END)";
                                    else 
                                        data = "(CASE WHEN F91022<CONVERT(DATETIME, '01/08/2011', 103) THEN '01/08/2011' ELSE CONVERT(VARCHAR, F91022, 103) END)";
                                    explicatii = "F910985, F910986,";
                                }
                                else
                                    data = "'" + data + "'";

                                if (Constante.tipBD == 2)
                                {
                                    szsql = inregistru + " (SELECT " + Convert.ToString(linie) + ", F91003, '" + CNP + "', F91022, " + explicatii + "'contract', '" + tmodif + "',"
                                        + " (CASE WHEN F91022<TO_DATE('01/08/2011', 'dd/MM/yyyy') THEN TO_DATE('01/08/2011', 'dd/MM/yyyy') ELSE F91022 END), " + data + ", 'prezent' "
                                        + ", '" + functie + "', '" + tip_ctr + "', '" + sal
                                        + "', '" + norma + "' FROM F910 WHERE F91003 = " + marca + " AND F910985 = '" + contract + "' AND MONTH = " + luna + " AND YEAR = " + an
                                        + " GROUP BY F91003, F91022, F910985, F910986,F91022," + salariu_i + ")";
                                }
                                else 
                                {
                                    szsql = inregistru + " (SELECT " + Convert.ToString(linie) + ", F91003, '" + CNP + "', F91022, " + explicatii + "'contract', '" + tmodif + "',"
                                        + " (CASE WHEN F91022<CONVERT(DATETIME, '01/08/2011', 103) THEN CONVERT(DATETIME, '01/08/2011', 103) ELSE F91022 END), " + data + ", 'prezent' "
                                        + ", '" + functie + "', '" + tip_ctr + "', '" + sal
                                        + "', '" + norma + "' FROM F910 WHERE F91003 = " + marca + " AND F910985 = '" + contract + "' AND MONTH = " + luna + " AND YEAR = " + an
                                        + " GROUP BY F91003, F91022, F910985, F910986,F91022," + salariu_i + ")";
                                }
                                General.ExecutaNonQuery(szsql, null);

                                tmodif = "Modificare";
                            }

                        if (Constante.tipBD == 2)
                        {
                            szsql = "SELECT TO_CHAR(F09505, 'dd/MM/yyyy') AS DI, TO_CHAR(F09506, 'dd/MM/yyyy') AS DSF, (CASE WHEN F09506<TO_DATE('01/01/2100', 'dd/MM/yyyy') THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP "
                                + " FROM F095 WHERE F09503 = " + marca + " AND F09504 = '" + contract + "' ORDER BY F09506";
                        }
                        else 
                        {
                            szsql = "SELECT CONVERT(VARCHAR, F09505, 103) AS DI, CONVERT(VARCHAR, F09506, 103) AS DSF, (CASE WHEN F09506<CONVERT(DATETIME, '01/01/2100', 103) THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP "
                                + " FROM F095 WHERE F09503 = " + marca + " AND F09504 = '" + contract + "' ORDER BY F09506";
                        }
                        dtDoua_ctr = General.IncarcaDT(szsql, null);
                        k_ctr = 1;
                        if (Constante.tipBD == 2)
                        {
                            szsql = "SELECT TO_CHAR(F910.F91022, 'dd/MM/yyyy') AS DA, TO_CHAR(F910.F91023, 'dd/MM/yyyy') AS DP, F910.F91025 AS ACTIV, TO_CHAR(F910.F910986, 'dd/MM/yyyy') AS DC, " + (f == true ? "F910.F91071" : "F910.F91098") + " AS FUNC, TO_CHAR(F910."
                                + salariu_i + ") AS SAL, F91043 AS NORMA, F910.MONTH AS MONTH, F910.YEAR AS YEAR, TO_CHAR(F910.F910991, 'dd/MM/yyyy') AS DMS " + "FROM F910 WHERE F910.F91003 = "
                                + marca + " AND ((F910.YEAR >= 2011 AND F910.MONTH >= 8) OR F910.YEAR >= 2012) AND F910.F910985 = '" + contract + "'  ORDER BY F910.YEAR, F910.MONTH";    
                        }
                        else 
                        {
                            szsql = "SELECT CONVERT(VARCHAR, F910.F91022, 103) AS DA, CONVERT(VARCHAR, F910.F91023, 103) AS DP, F910.F91025 AS ACTIV, CONVERT(VARCHAR, F910.F910986, 103) AS DC, " + (f == true ? "F910.F91071" : "F910.F91098") + " AS FUNC, CONVERT(VARCHAR, F910."
                                + salariu_i + ") AS SAL, F91043 AS NORMA, F910.MONTH AS MONTH, F910.YEAR AS YEAR, CONVERT(VARCHAR, F910.F910991, 103) AS DMS " + "FROM F910 WHERE F910.F91003 = "
                                + marca + " AND ((F910.YEAR >= 2011 AND F910.MONTH >= 8) OR F910.YEAR >= 2012) AND F910.F910985 = '" + contract + "' ORDER BY F910.YEAR, F910.MONTH"; 
                        }
                        dtDoua = new DataTable();
                        dtDoua = General.IncarcaDT(szsql, null);
                        int zi_ang = 0, luna_ang = 0, an_ang = 0;
                        //int luna_ant = 0, an_ant = 0;
                        if (dtDoua != null)                            
                            if (dtDoua.Rows.Count > 0)
                                for (int k = 0; k < dtDoua.Rows.Count; k++)
                                {
                                    luna1 = dtDoua.Rows[k]["MONTH"].ToString();
                                    an1 = dtDoua.Rows[k]["YEAR"].ToString();
                                    data_ctr = dtDoua.Rows[k]["DC"].ToString();
                                    if (k == 0)
                                    {
                                        data_a = dtDoua.Rows[k]["DA"].ToString();
                                        sal = TryToParse(dtDoua.Rows[k]["SAL"].ToString());
                                        func = dtDoua.Rows[k]["FUNC"].ToString();
                                        norma = dtDoua.Rows[k]["NORMA"].ToString();
                                        zi_ang = Convert.ToInt32(data_a.Substring(0, 2));
                                        luna_ang = Convert.ToInt32(data_a.Substring(3, 2));
                                        an_ang = Convert.ToInt32(data_a.Substring(6, 4));

                                        //luna_ant = Convert.ToInt32(dtDoua.Rows[k]["MONTH"].ToString());
                                        //an_ant = Convert.ToInt32(dtDoua.Rows[k]["YEAR"].ToString());
                                    }                             
                                    //else
                                    //{
                                    //    if ((Convert.ToInt32(an1) == an_ant && Convert.ToInt32(luna1) == luna_ant + 1) || (Convert.ToInt32(an1) == an_ant + 1 && Convert.ToInt32(luna1) == 1 && luna_ant == 12))
                                    //    {
                                    //        luna_ant = Convert.ToInt32(dtDoua.Rows[k]["MONTH"].ToString());
                                    //        an_ant = Convert.ToInt32(dtDoua.Rows[k]["YEAR"].ToString());
                                    //    }
                                    //    else
                                    //        continue;
                                    //}

                                    // parcurg F095, daca gasesc modificari, data sfarsit < data(luna1, an1)
                                    int zi_ctr = 0;
                                    bool data_inc = true;
                                    if (dtDoua_ctr != null)                                        
                                        if (dtDoua_ctr.Rows.Count > k_ctr)
                                        {
                                            //Radu 04.07.2013
                                            zi_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(0, 2));
                                            luna_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(3, 2));
                                            an_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(6, 4));

                                            if (an_ctr < 2011 || (an_ctr == 2011 && luna_ctr < 8) || (zi_ctr == zi_ang && luna_ctr == luna_ang && an_ctr == an_ang))
                                            {
                                                zi_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2));
                                                luna_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2));
                                                an_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4));
                                                data_inc = false;
                                            }

                                            if (luna_ctr == Convert.ToInt32(luna1) && an_ctr == Convert.ToInt32(an1))
                                            {
                                                tip_ctr = dtDoua_ctr.Rows[k_ctr]["TIP"].ToString();
                                                if (data_inc)
                                                    szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(0, 2))).AddDays(-1).ToShortDateString()
                                                        + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                                        + contract + "'"; 
                                                else
                                                    szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString()
                                                        + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                                        + contract + "'";
                                                General.ExecutaNonQuery(szsql, null);
                                                linie++;
                                                if (data_inc)
                                                {
                                                    if (Constante.tipBD == 2)
                                                    {
                                                        szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                            + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                            + " TO_DATE('" + dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString() + "', 'dd/MM/yyyy'), '" +dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString()
                                                            + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                                    }
                                                    else 
                                                    {
                                                        szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                            + data_a + "', 103), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                            + " CONVERT(DATETIME,'" + dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString() + "', 103), '" + dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString()
                                                            + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Constante.tipBD == 2)
                                                    {
                                                        szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                            + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                            + " TO_DATE('" + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString() + "', 'dd/MM/yyyy'), '" 
                                                            + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString()
                                                            + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                                    }
                                                    else 
                                                    {
                                                        szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME, '"
                                                            + data_a + "', 103), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                            + " CONVERT(DATETIME,'" + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString() + "', 103), '" 
                                                            + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString()
                                                            + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                                    }
                                                }
                                                General.ExecutaNonQuery(szsql, null);
                                                k_ctr++;
                                            }
                                            
                                        }                                    
                                    if (func != dtDoua.Rows[k]["FUNC"].ToString())
                                    {
                                        //Radu 11.05.2016
                                        string dtMod = "";
                                        if (Constante.tipBD == 2)
                                            dtMod = "TO_CHAR(F910956, 'dd/mm/yyyy')";
                                        else 
                                            dtMod = "CONVERT(VARCHAR, F910956, 103)";

                                        szsql = "SELECT " + dtMod + " AS DMC FROM F9101 WHERE F91003 = " + marca + " AND YEAR = " + an1 + " AND MONTH = " + luna1;
                                        dtSpor = new DataTable();
                                        dtSpor = General.IncarcaDT(szsql, null);

                                        func = dtDoua.Rows[k]["FUNC"].ToString();
                                        sal = TryToParse(dtDoua.Rows[k]["SAL"].ToString());
                                        data = "01/01/1900";
                                        if (dtSpor != null)
                                            if (dtSpor.Rows.Count > 0)                                                                
                                                data = dtSpor.Rows[0]["DMC"].ToString();   

                                        if (Convert.ToInt32(data.Substring(6, 4)) < 1951 || Convert.ToInt32(data.Substring(6, 4)) > 2098)
                                        {
                                            if (Convert.ToInt32(luna1) < 10)
                                                data = "01/0";
                                            else
                                                data = "01/";
                                            data = data + luna1 + "/" + an1;
                                        }

                                        //Radu 11.05.2016
                                        DateTime dateTmp = new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2)));
                                        // mihad 07.12.2016
                                        if (f == true)
                                        {
                                            if (func == "0")
                                                szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                                            else
                                                szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718 WHERE F71802 = " + func;
                                        }
                                        else
                                        {
                                            if (dateTmp < new DateTime(2012, 1, 1))
                                                szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 5 AND F72202 = " + func;
                                            if (dateTmp >= new DateTime(2012, 1, 1) && dateTmp < dtRevisal)
                                                szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 6 AND F72202 = " + func;
                                            if (dateTmp >= dtRevisal)
                                                szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 7 AND F72202 = " + func;
                                            //end Radu
                                        }
                                        dtFunctie = new DataTable();
                                        dtFunctie = General.IncarcaDT(szsql, null);
                                        functie = "";
                                        if (dtFunctie != null)
                                            if (dtFunctie.Rows.Count > 0)
                                                functie = dtFunctie.Rows[0]["A"].ToString() + " " + dtFunctie.Rows[0]["B"].ToString();  
                                        szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                            + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                            + contract + "'";
                                        General.ExecutaNonQuery(szsql, null);
                                        linie++;
                                        if (Constante.tipBD == 2)
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'COR/SALARIU', 'Modificare', "
                                                + " TO_DATE('" + data + "', 'dd/MM/yyyy'), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        else 
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                + data_a + "', 103), NULL, NULL, " + " 'COR/SALARIU', 'Modificare', "
                                                + " CONVERT(DATETIME,'" + data + "', 103), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        General.ExecutaNonQuery(szsql, null);
                                    }

                                    // modificare salariu
                                    if (sal != TryToParse(dtDoua.Rows[k]["SAL"].ToString()))
                                    {
                                        sal = TryToParse(dtDoua.Rows[k]["SAL"].ToString());
                                        data = dtDoua.Rows[k]["DMS"].ToString();
                                        if (Convert.ToInt32(data.Substring(6, 4)) < 1951 || Convert.ToInt32(data.Substring(6, 4)) > 2098)
                                        {
                                            if (Convert.ToInt32(luna1) < 10)
                                                data = "01/0";
                                            else
                                                data = "01/";
                                            data = data + luna1 + "/" + an1;
                                        }
                                        szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                            + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                            + contract + "'";
                                        General.ExecutaNonQuery(szsql, null);
                                        linie++;
                                        if (Constante.tipBD == 2)
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'SALARIU', 'Modificare', "
                                                + " TO_DATE('" + data + "', 'dd/MM/yyyy'), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        else 
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                + data_a + "', 103), NULL, NULL, " + " 'SALARIU', 'Modificare', "
                                                + " CONVERT(DATETIMe,'" + data + "', 103), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        General.ExecutaNonQuery(szsql, null);
                                    }

                                    //modificare norma
                                    if (norma != dtDoua.Rows[k]["NORMA"].ToString())
                                    {
                                        szsql = "SELECT F910955 AS DMN FROM F9101 WHERE F91003 = " + marca + " AND YEAR = " + an1 + " AND MONTH = " + luna1;
                                        dtSpor = new DataTable();
                                        dtSpor = General.IncarcaDT(szsql, null);
                                        norma = dtDoua.Rows[k]["NORMA"].ToString();
                                        data = "01/01/1900";
                                        if (dtSpor != null)
                                            if (dtSpor.Rows.Count > 0)
                                                data = dtSpor.Rows[0]["DMN"].ToString();
                                        if (Convert.ToInt32(data.Substring(6, 4)) < 1951 || Convert.ToInt32(data.Substring(6, 4)) > 2098)
                                        {
                                            if (Convert.ToInt32(luna1) < 10)
                                                data = "01/0";
                                            else
                                                data = "01/";
                                            data = data + luna1 + "/" + an1;
                                        }

                                        szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                            + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                            + contract + "'";
                                        General.ExecutaNonQuery(szsql, null);
                                        linie++;
                                        if (Constante.tipBD == 2)
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'SALARIU', 'Modificare', "
                                                + " TO_DATE('" + data + "', 'dd/MM/yyyy'), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        else 
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                + data_a + "', 103), NULL, NULL, " + " 'SALARIU', 'Modificare', "
                                                + " CONVERT(DATETIME,'" + data + "', 103), '" + data + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '"
                                                + sal + "', '" + norma + "')";
                                        }
                                        General.ExecutaNonQuery(szsql, null);
                                    }

                                    an_1 = an1;
                                    luna_1 = luna1;

                                    if (dtDoua_ctr.Rows.Count > k_ctr)
                                    {
                                        zi_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(0, 2));
                                        luna_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(3, 2));
                                        an_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(6, 4));
                                        while (zi_ctr != 1 && dtDoua_ctr.Rows.Count > k_ctr &&
                                            luna_ctr == Convert.ToInt32(luna1) && an_ctr == Convert.ToInt32(an1))    
                                        {
                                            tip_ctr = dtDoua_ctr.Rows[k_ctr]["TIP"].ToString();
                                            szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString(). Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(0, 2))).AddDays(-1).ToShortDateString()
                                                + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                                + contract + "'";
                                            General.ExecutaNonQuery(szsql, null);
                                            linie++;
                                            if (Constante.tipBD == 2)
                                            {
                                                szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                    + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                    + " TO_DATE('" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 'dd/MM/yyyy'), '" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString()
                                                    + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                            }
                                            else 
                                            {
                                                szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                    + data_a + "', 103), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                    + " CONVERT(DATETIME,'" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 103), '" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString()
                                                    + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                            }
                                            General.ExecutaNonQuery(szsql, null);
                                            k_ctr++;
                                            if (dtDoua_ctr.Rows.Count > k_ctr)
                                            {
                                                zi_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(0, 2));
                                                luna_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(3, 2));
                                                an_ctr = Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(6, 4));
                                            }
                                        }
                                    }                                    
                                }

                    }   // sf contract
                    // citesc din F100
                    if (Constante.tipBD == 2)
                    {
                        szsql = "SELECT TO_CHAR(F100.F10022, 'dd/MM/yyyy') AS DA,  TO_CHAR(F100.F10023, 'dd/MM/yyyy') AS DP, F100.F100985 AS NC, TO_CHAR(F100.F100986, 'dd/MM/yyyy') AS DC, "
                            + (f == true ? "F100.F10071" : "F100.F10098") + " AS FUNC, TO_CHAR(F100.F100991, 'dd/mm/yyyy') AS DMS, TO_CHAR(F1001.F100956. 'dd/mm/yyyy') AS DMC, TO_CHAR(F1001.F100955, 'dd/mm/yyyy') AS DMN, TO_CHAR(F100."
                        + salariu + ") AS SAL, F10043 AS NORMA FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + marca;
                    }
                    else 
                    {
                        szsql = "SELECT CONVERT(VARCHAR,F100.F10022, 103) AS DA,  CONVERT(VARCHAR,F100.F10023, 103) AS DP, F100.F100985 AS NC, CONVERT(VARCHAR, F100.F100986, 103) AS DC, "
                        + (f == true ? "F100.F10071" : "F100.F10098") + " AS FUNC, CONVERT(VARCHAR, F100.F100991, 103) AS DMS, CONVERT(VARCHAR, F1001.F100956, 103) AS DMC, CONVERT(VARCHAR, F1001.F100955, 103) AS DMN, CONVERT(VARCHAR, F100."
                        + salariu + ") AS SAL, F10043 AS NORMA FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = " + marca;
                    }
                    dtDoua = General.IncarcaDT(szsql, null);
                    if (dtDoua != null)                        
                        if (dtDoua.Rows.Count > 0)
                        {
                            data_a = dtDoua.Rows[0]["DA"].ToString();
                            if (Convert.ToInt32(luna1) == 12)
                            {
                                luna1 = "1";
                                an1 = Convert.ToString(Convert.ToInt32(an1) + 1);
                            }
                            else
                                luna1 = Convert.ToString(Convert.ToInt32(luna1) + 1);
                            data_ctr = dtDoua.Rows[0]["DC"].ToString();

                            data_plecarii = dtDoua.Rows[0]["DP"].ToString();

                            // daca mai am in F095 cu data mai mica decat F010
                            if (dtDoua_ctr.Rows.Count > k_ctr - 1)
                            {
                                int luna_dsf = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).Month;
                                int an_dsf = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).Year;
                                if ((Convert.ToInt32(an1) >= an_dsf && Convert.ToInt32(luna1) >= luna_dsf) || Convert.ToInt32(an1) > an_dsf)
                                    // iau ce a ramas in F095
                                    while (dtDoua_ctr.Rows.Count > k_ctr)
                                    {
                                        tip_ctr = dtDoua_ctr.Rows[k_ctr]["TIP"].ToString();
                                        string data_temp = string.Empty; string data_temp_i = string.Empty;
                                        if (Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(6, 4)) == 2100)
                                        {
                                            data_temp = dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString();
                                            data_temp_i = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString();
                                        }
                                        else
                                        {
                                            data_temp = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(0, 2))).AddDays(-1).ToShortDateString();
                                            data_temp_i = dtDoua_ctr.Rows[k_ctr]["DI"].ToString();
                                        }
                                        szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + data_temp
                                            + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                            + contract + "'";
                                        General.ExecutaNonQuery(szsql, null);
                                        linie++;
                                        if (Constante.tipBD == 2)
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                                + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                + " TO_DATE('" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 'dd/MM/yyyy'), '" + data_temp_i
                                                + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                        }
                                        else 
                                        {
                                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                                + data_a + "', 103), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                                + " CONVERT(DATETIME, '" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 103), '" + data_temp_i
                                                + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                        }
                                        General.ExecutaNonQuery(szsql, null);
                                        k_ctr++;

                                        luna_dsf = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).Month;
                                        an_dsf = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).Year;
                                        if (!((Convert.ToInt32(an1) >= an_dsf && Convert.ToInt32(luna1) >= luna_dsf) || Convert.ToInt32(an1) > an_dsf))
                                            break;
                                    }
                            }

                            if (func != dtDoua.Rows[0]["FUNC"].ToString() || sal != TryToParse(dtDoua.Rows[0]["SAL"].ToString())
                                || norma != dtDoua.Rows[0]["NORMA"].ToString())
                            {
                                if (func != dtDoua.Rows[0]["FUNC"].ToString())
                                {
                                    data = dtDoua.Rows[0]["DMC"].ToString();
                                    func = dtDoua.Rows[0]["FUNC"].ToString();
                                }
                                if (sal != TryToParse(dtDoua.Rows[0]["SAL"].ToString()))
                                {
                                    data = dtDoua.Rows[0]["DMS"].ToString();
                                    sal = TryToParse(dtDoua.Rows[0]["SAL"].ToString());
                                }
                                if (norma != dtDoua.Rows[0]["NORMA"].ToString())
                                {
                                    data = dtDoua.Rows[0]["DMN"].ToString();
                                    norma = dtDoua.Rows[0]["NORMA"].ToString();
                                }

                                if (Convert.ToInt32(data.Substring(6, 4)) < 1951 || Convert.ToInt32(data.Substring(6, 4)) > 2098)
                                {
                                    if (Convert.ToInt32(luna1) < 10)
                                        data = "01/0";
                                    else
                                        data = "01/";
                                    data = data + luna1 + "/" + an1;
                                }

                                //Radu 11.05.2016
                                DateTime dataTmp = new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2)));
                                // mihad 07.12.2016
                                if (f == true)
                                {
                                    if (func == "0")
                                        szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                                    else
                                        szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718 WHERE F71802 = " + func;
                                }
                                else
                                {
                                    if (dataTmp < new DateTime(2012, 1, 1))
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 5 AND F72202 = " + func;
                                    if (dataTmp >= new DateTime(2012, 1, 1) && dataTmp < dtRevisal)
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 6 AND F72202 = " + func;
                                    if (dataTmp >= dtRevisal)
                                        szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 7 AND F72202 = " + func;
                                    //end Radu
                                }

                                dtFunctie = new DataTable();
                                dtFunctie = General.IncarcaDT(szsql, null);
                                functie = "";
                                if (dtFunctie != null)                                    
                                    if (dtFunctie.Rows.Count > 0)
                                        functie = dtFunctie.Rows[0]["A"].ToString() + " " + dtFunctie.Rows[0]["B"].ToString();                                             
                                szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                    + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                    + contract + "'";
                                General.ExecutaNonQuery(szsql, null);
                                linie++;
                                if (Constante.tipBD == 2)
                                {
                                    szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                        + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'COR/SALARIU', 'Modificare', "
                                        + " TO_DATE('" + data + "', 'dd/MM/yyyy'), '" + data + "', 'prezent', '" + functie
                                        + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                }
                                else 
                                {
                                    szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                        + data_a + "', 103), NULL, NULL, " + " 'COR/SALARIU', 'Modificare', "
                                        + " CONVERT(DATETIME,'" + data + "', 103), '" + data + "', 'prezent', '" + functie
                                        + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                                }
                                General.ExecutaNonQuery(szsql, null);
                            }
                        }
                    // iau ce a ramas in F095
                    while (dtDoua_ctr.Rows.Count > k_ctr)
                    {
                        tip_ctr = dtDoua_ctr.Rows[k_ctr]["TIP"].ToString();
                        string data_temp = string.Empty; string data_temp_i = string.Empty;
                        if (Convert.ToInt32(dtDoua_ctr.Rows[k_ctr]["DI"].ToString().Substring(6, 4)) == 2100)
                        {
                            data_temp = dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString();
                            data_temp_i = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DSF"].ToString().Substring(0, 2))).AddDays(1).ToShortDateString();
                        }
                        else
                        {
                            data_temp = new DateTime(Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua_ctr.Rows[k_ctr - 1]["DI"].ToString().Substring(0, 2))).AddDays(-1).ToShortDateString();
                            data_temp_i = dtDoua_ctr.Rows[k_ctr]["DI"].ToString();
                        }
                        szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + data_temp
                            + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                            + contract + "'";
                        General.ExecutaNonQuery(szsql, null);
                        linie++;
                        if (Constante.tipBD == 2)
                        {
                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                + data_a + "', 'dd/MM/yyyy'), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                + " TO_DATE('" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 'dd/MM/yyyy'), '" + data_temp_i
                                + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                        }
                        else
                        {
                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                + data_a + "', 103), NULL, NULL, " + " 'CONTRACT', 'Modificare', "
                                + " CONVERT(DATETIME, '" + dtDoua_ctr.Rows[k_ctr]["DI"].ToString() + "', 103), '" + data_temp_i
                                + "', 'prezent', '" + functie + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                        }
                        General.ExecutaNonQuery(szsql, null);
                        k_ctr++;
                    }

                }
                else
                {
                    //Radu 11.05.2016
                    if (Constante.tipBD == 2)
                        szsql = "SELECT F10022, F100985, 'TO_DATE(''' || TO_CHAR(F100986, 'DD/MM/YYYY') || ''', ''dd/mm/yyyy'')' AS F100986, " + salariu + " AS S, "
                                + (f == true ? "MAX(F71804) AS A, MAX('') AS B, MAX('') AS C" : "MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C")
                                + ", MAX(F10043) AS NORMA FROM F100, "
                                + (f == true ? "F718 WHERE F10071 = F71802(+) " : "F722 where F10098 = F72202(+) and (F72206 = CASE WHEN F10022 < TO_DATE('01/01/2012', 'dd/mm/yyyy') then 5 else CASE WHEN F10022 < TO_DATE('17/04/2016', 'dd/mm/yyyy') then 6 else 7 end end or F72206 is null) ")
                                + "AND F10003 = " + marca
                                + " group by  F10022, F100985, 'TO_DATE(''' || TO_CHAR(F100986, 'DD/MM/YYYY') || ''', ''dd/mm/yyyy'')',  " + salariu;
                    else 
                        szsql = "SELECT F10022, F100985, 'CONVERT(DATETIME,''' + CONVERT(VARCHAR, F100986, 103) + ''', 103)' AS F100986, " + salariu + " AS S, "
                                + (f == true ? "MAX(F71804) AS A, MAX('') AS B, MAX('') AS C" : "MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C")
                                + ", MAX(F10043) AS NORMA FROM F100 "
                                + (f == true ? "LEFT JOIN F718 ON F10071 = F71802 " : "LEFT JOIN F722 on F72202 = F10098 and F72206 = CASE WHEN F10022 < CONVERT(Datetime, '01/01/2012', 103) then 5 else CASE WHEN F10022 < CONVERT(Datetime, '17/04/2016', 103) then 6 else 7 end end ")
                                + "WHERE F10003 = " + marca
                                + " group by  F10022, F100985, 'CONVERT(DATETIME,''' + CONVERT(VARCHAR, F100986, 103) + ''', 103)',  " + salariu;
                    dtFunctie = new DataTable();
                    dtFunctie = General.IncarcaDT(szsql, null);
                    contract = ""; data_a = ""; data_ctr = ""; sal = ""; norma = ""; functie = "";
                    if (dtFunctie != null)                       
                        if (dtFunctie.Rows.Count > 0)
                        {
                            contract = dtFunctie.Rows[0]["F100985"].ToString();
                            data_a = dtFunctie.Rows[0]["F10022"].ToString();
                            data_ctr = dtFunctie.Rows[0]["F100986"].ToString();
                            sal = TryToParse(dtFunctie.Rows[0]["S"].ToString());
                            norma = dtFunctie.Rows[0]["NORMA"].ToString();
                            functie = dtFunctie.Rows[0]["A"].ToString() + " " + dtFunctie.Rows[0]["B"].ToString();  
                        }
                    if (Constante.tipBD == 2)
                    {
                        szsql = "SELECT (CASE WHEN MIN(F09506)<TO_DATE('01/01/2100', 'dd/MM/yyyy') THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP "
                            + " FROM F095 WHERE F09503=" + marca + " AND F09504='" + contract + "'";
                    }
                    else 
                    {
                        szsql = "SELECT (CASE WHEN MIN(F09506)<CONVERT(DATETIME, '01/01/2100', 103) THEN 'Determinat' ELSE 'Nedeterminat' END) AS TIP "
                            + " FROM F095 WHERE F09503=" + marca + " AND F09504='" + contract + "'";
                    }
                    dtFunctie = new DataTable();
                    dtFunctie = General.IncarcaDT(szsql, null);
                    tip_ctr = "Nedeterminata";
                    if (dtFunctie != null)                        
                        if (dtFunctie.Rows.Count > 0)
                            tip_ctr = dtFunctie.Rows[0]["TIP"].ToString();
                    dtSpor = new DataTable();
                    if (Constante.tipBD == 2)
                    {
                        szsql = inregistru + " (SELECT 1, F10003, F10017, F10022, '" + contract + "' AS F100985, " + data_ctr + " AS F100986, 'contract', 'Adaugare', F10022, TO_CHAR(10022, 'dd/MM/yyyy'), 'prezent' "
                            + ", '" + functie + "', '" + tip_ctr + "', '"
                            + sal + "', TO_CHAR('" + norma + "') FROM F100, F095 WHERE F100.F10003 = F095.F09503 AND F100.F100985 = F095.F09504 AND F100.F10003 = " + marca
                            + " GROUP BY F10003, F10017, F10022, F100985, F100986,F10022," + salariu + ")";
                    }
                    else 
                    {
                        szsql = inregistru + " (SELECT 1, F10003, F10017, F10022, '" + contract + "' AS F100985, " + data_ctr + " AS F100986, 'contract', 'Adaugare', F10022, CONVERT(VARCHAR, 10022, 103), 'prezent' "
                            + ", '" + functie + "', '" + tip_ctr + "', '"
                            + sal + "', CONVERT(VARCHAR,'" + norma + "') FROM F100, F095 WHERE F100.F10003 = F095.F09503 AND F100.F100985 = F095.F09504 AND F100.F10003 = " + marca
                            + " GROUP BY F10003, F10017, F10022, F100985, F100986,F10022," + salariu +")";
                    }
                    General.ExecutaNonQuery(szsql, null);
                    linie++;
                }
            // citesc din F704
            if (Constante.tipBD == 2)
            {
                szsql = "SELECT F70404, F70407, TO_CHAR(F70406, 'dd/MM/yyyy') AS DM, F70410 "
                    + " FROM F704 WHERE F70420 = 0 AND (F70404 = 1 OR F70404 = " + (f == true ? 2 : 3) + " OR F70404 = 6) AND F70403 = " + marca
                        + " ORDER BY F70406";
            }
            else 
            {
                szsql = "SELECT F70404, F70407, CONVERT(VARCHAR,F70406, 103) AS DM, F70410 "
                        + " FROM F704 WHERE F70420 = 0 AND (F70404 = 1 OR F70404 = " + (f == true ? 2 : 3) + " OR F70404 = 6) AND F70403 = " + marca
                        + " ORDER BY F70406";
            }
            dtDoua = new DataTable();
            dtDoua = General.IncarcaDT(szsql, null);

            if (dtDoua != null)
                if (dtDoua.Rows.Count > 0)
                {
                    for (int k = 0; k < dtDoua.Rows.Count; k++)
                    {
                        string tip_modif = dtDoua.Rows[k]["F70404"].ToString();
                        if (Convert.ToInt32(tip_modif) == 3 || Convert.ToInt32(tip_modif) == 2)
                        {
                            func = dtDoua.Rows[k]["F70407"].ToString();
                            data_ctr = string.Empty;

                            //Radu 11.05.2016
                            DateTime dataTmp = new DateTime(Convert.ToInt32(dtDoua.Rows[k]["DM"].ToString().Substring(6, 4)), Convert.ToInt32(dtDoua.Rows[k]["DM"].ToString().Substring(3, 2)), Convert.ToInt32(dtDoua.Rows[k]["DM"].ToString().Substring(0, 2)));
                            // mihad 07.12.2016
                            if (f == true)
                            {
                                if (func == "0")
                                    szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718, F100 WHERE F71802 = F10071 AND F10003 = " + marca;
                                else
                                    szsql = "SELECT F71804 AS A, '' AS B, '' AS C FROM F718 WHERE F71802 = " + func;
                            }
                            else
                            {
                                if (dataTmp < new DateTime(2012, 1, 1))
                                    szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 5 AND F72202 = " + func;
                                if (dataTmp >= new DateTime(2012, 1, 1) && dataTmp < dtRevisal)
                                    szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 6 AND F72202 = " + func;
                                if (dataTmp >= dtRevisal)
                                    szsql = "SELECT MAX(F72202) AS A, MAX(F72204) AS B, MAX(F72206) AS C FROM F722 WHERE F72206 = 7 AND F72202 = " + func;
                            }
                            //end Radu


                            dtFunctie = new DataTable();
                            dtFunctie = General.IncarcaDT(szsql, null);
                            functie = "";
                            if (dtFunctie != null)
                                if (dtFunctie.Rows.Count > 0)
                                    functie = dtFunctie.Rows[0]["A"].ToString() + " " + dtFunctie.Rows[0]["B"].ToString();  
                        }
                        if (Convert.ToInt32(tip_modif) == 1)
                            sal = TryToParse(dtDoua.Rows[k]["F70407"].ToString());
                        if (Convert.ToInt32(tip_modif) == 6)
                            norma = dtDoua.Rows[k]["F70407"].ToString();

                        contract = dtDoua.Rows[k]["F70410"].ToString();
                        data = dtDoua.Rows[k]["DM"].ToString();
                        if (Constante.tipBD == 2)
                        {
                            szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                + contract + "' AND TO_DATE(INCEPUT_INTERVAL, 'dd/MM/yyyy') <= TO_DATE('" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString() + "', 'dd/MM/yyyy')";
                        }
                        else 
                        {
                            szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString()
                                + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca + " AND NR_CONTRACT = '"
                                + contract + "' AND CONVERT(DATETIME,INCEPUT_INTERVAL, 103) <= CONVERT(DATETIME,'" + new DateTime(Convert.ToInt32(data.Substring(6, 4)), Convert.ToInt32(data.Substring(3, 2)), Convert.ToInt32(data.Substring(0, 2))).AddDays(-1).ToShortDateString() + "', 103)";
                        }
                        General.ExecutaNonQuery(szsql, null);
                        linie++;
                        if (Constante.tipBD == 2)
                        {
                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', TO_DATE('"
                                + data_a + "', 'dd/MM/yyyy'), '" + contract + "', NULL, " + (f == true ? " 'FUNCTIE/SALARIU', 'Modificare', " : " 'COR/SALARIU', 'Modificare', ")
                                + " TO_DATE('" + data + "', 'dd/MM/yyyy'), '" + data + "', 'prezent', '" + functie
                                + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                        }
                        else 
                        {
                            szsql = inregistru + " VALUES (" + Convert.ToString(linie) + ", " + marca + ", '" + CNP + "', CONVERT(DATETIME,'"
                                + data_a + "', 103), '" + contract + "', NULL, " + (f == true ? " 'FUNCTIE/SALARIU', 'Modificare', " : " 'COR/SALARIU', 'Modificare', ")
                                + " CONVERT(DATETIME,'" + data + "', 103), '" + data + "', 'prezent', '" + functie
                                + "', '" + tip_ctr + "', '" + sal + "', '" + norma + "')";
                        }
                        General.ExecutaNonQuery(szsql, null);
                    }
                }

            //citire luna curenta
            sqltmp = "SELECT F01011, F01012 FROM F010";
            DataTable dtTemp = new DataTable();
            dtTemp = General.IncarcaDT(sqltmp, null);
            string lunaC = "", anC = "", dataC = "";
            if (dtTemp != null)
                if (dtTemp.Rows.Count > 0)
                {
                    lunaC = dtTemp.Rows[0]["F01012"].ToString().PadLeft(2, '0');
                    anC = dtTemp.Rows[0]["F01011"].ToString();
                    dataC = "01/" + lunaC + "/" + anC;
                }

            // daca angajatul are data plecarii in luna curenta
            if (new DateTime(Convert.ToInt32(data_plecarii.Substring(6, 4)), Convert.ToInt32(data_plecarii.Substring(3, 2)), Convert.ToInt32(data_plecarii.Substring(0, 2))) 
                <= new DateTime(Convert.ToInt32(dataC.Substring(6, 4)), Convert.ToInt32(dataC.Substring(3, 2)), Convert.ToInt32(dataC.Substring(0, 2))))
            {
                szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = '" + data_plecarii
                    + "' WHERE SFARSIT_INTERVAL = 'prezent' AND MARCA = " + marca;
                General.ExecutaNonQuery(szsql, null);
            }
            // daca au avut loc mai multe modificari in aceeasi zi
            if (Constante.tipBD == 2)
            {
                szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = INCEPUT_INTERVAL "
                    + " WHERE TO_CHAR((TO_DATE(INCEPUT_INTERVAL, 'dd/MM/yyyy') - 1)) = SFARSIT_INTERVAL";
            }
            else 
            {
                szsql = "UPDATE TMP_ADEVERINTA SET SFARSIT_INTERVAL = INCEPUT_INTERVAL "
                    + " WHERE CONVERT(VARCHAR, (CONVERT(DATETIME, INCEPUT_INTERVAL, 103) - 1)) = SFARSIT_INTERVAL";
            }
            General.ExecutaNonQuery(szsql, null);

            string sqlModifications = "";
            DataTable dtModifications = new DataTable();
            if (Constante.tipBD == 2)
            {
                sqlModifications = "SELECT CASE WHEN DATA_CONTRACT IS NOT NULL THEN (NR_CONTRACT||'/ '||TO_CHAR(DATA_CONTRACT, 'dd.MM.yyyy')) ELSE NR_CONTRACT END AS \"Nr./Data contract\","
                    + " FUNCTIE AS FUNCTIA,"
                    + " TIP_DURATA AS \"Tip durata\","
                    + " INCEPUT_INTERVAL AS DATA, "
                    + " CASE WHEN (TO_DATE(F100993, 'dd/MM/yyyy') < TO_DATE('01/01/2016', 'dd/mm/yyyy')) THEN TO_CHAR(F100993, 'dd/MM/yyyy') ELSE NULL END AS DATASF, "
                    + " SALARIU, "
                    + " NORMA, "
                    + " VALOARE_MODIFICARE AS MOTIVINTR, "
                    + " TO_CHAR(DATA_ANGAJARII, 'dd/MM/yyyy') AS DATANG "
                    + " FROM TMP_ADEVERINTA, F100 WHERE MARCA = '" + marca + "' AND TO_CHAR(F10003) = MARCA"
                    + " ORDER BY LINIE";
            }
            else 
            {
                sqlModifications = "SELECT CASE WHEN DATA_CONTRACT IS NOT NULL THEN (NR_CONTRACT + '/ ' + CONVERT(VARCHAR, DATA_CONTRACT, 104)) ELSE NR_CONTRACT END AS \"Nr./Data contract\","
                    + " FUNCTIE AS FUNCTIA,"
                    + " TIP_DURATA AS \"Tip durata\","
                    + " INCEPUT_INTERVAL AS DATA, "
                    + " CASE WHEN (CONVERT(DATETIME, F100993, 103) < CONVERT(DATETIME, '01/01/2100', 103)) THEN CONVERT(VARCHAR, F100993, 103) ELSE NULL END AS DATASF, "
                    + " SALARIU, "
                    + " NORMA, "
                    + " VALOARE_MODIFICARE AS MOTIVINTR, "
                    + " CONVERT(VARCHAR, DATA_ANGAJARII, 103) AS DATANG "
                    + " FROM TMP_ADEVERINTA, F100 WHERE MARCA = '" + marca + "' AND CONVERT(VARCHAR, F10003) = MARCA"
                    + " ORDER BY LINIE";

            }
            dtModifications = General.IncarcaDT(sqlModifications, null);

            sqltmp = "DROP TABLE TMP_ADEVERINTA";
            General.ExecutaNonQuery(sqltmp, null);

            return dtModifications;

        }

        private string TryToParse(string value)
        {
            double number = 0.00; int num = 0;
            //bool result = Double.TryParse(value, out number);
            value = value.Replace(',', '.');
            bool result = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
            if (result)
            {
                num = Convert.ToInt32(number);
                return Convert.ToString(num);
            }
            else
            {
                result = Int32.TryParse(value, out num);
                return Convert.ToString(num);
            }
        }

        private void btnConfigurare_Click()
        {
            config.Visible = true;
            Session["AdevConfig"] = "true";
        }

        private void btnAnulare_Click()
        {
            config.Visible = false;
            Session["AdevConfig"] = "false";
        }

        public void btnGen_Click(object sender, EventArgs e)
        {
            btnGenerare_Click();
        }



        public void ReloadCombo()
        {
            string sql = "";
            Dictionary<String, String> lista = new Dictionary<string, string>();
            if (Session["AdevListaParam"] == null)
                lista = LoadParameters();
            else
                lista = Session["AdevListaParam"] as Dictionary<string, string>;
            if (Constante.tipBD == 2)
            {
                sql = (chkActivi.Checked == true ? "WHERE F10025 = 0" : String.Empty);

 
                if (chkVenit.Checked == true)
                {
                    if (sql.Contains("WHERE") == true)
                        sql += " AND A.F10003 IN (SELECT DISTINCT F92003 FROM F920 WHERE YEAR = " + Convert.ToInt32(lista["ANU"] ?? DateTime.Now.Year.ToString()) + ") ";
                    else
                        sql += " WHERE A.F10003 IN (SELECT DISTINCT F92003 FROM F920 WHERE YEAR = " + Convert.ToInt32(lista["ANU"] ?? DateTime.Now.Year.ToString()) + ") ";
                }

                if (chkCIC.Checked == true)
                {
                    if (sql.Contains("WHERE") == true)
                        sql += " AND A.F10003 IN (SELECT F10003 FROM F100, F110, f010 WHERE F10002 = F01002 AND F10003 = F11003 AND (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"])
                            + ") <=  extract(year from F11006) AND CASE WHEN (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"]) + ") =  extract(year from F11006) THEN F01012 ELSE 0 END <= extract(month from F11006)) ";
                    else
                        sql += " WHERE A.F10003 IN (SELECT F10003 FROM F100, F110, f010 WHERE F10002 = F01002 AND F10003 = F11003 AND (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"])
                            + ") <=  extract(year from F11006) AND CASE WHEN (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"]) + ") =  extract(year from F11006) THEN F01012 ELSE 0 END <= extract(month from F11006)) ";
                }

                //sql += " ORDER BY F10008||' '||F10009";
                
            }
            else if (Constante.tipBD == 1)
            {
                sql = (this.chkActivi.Checked == true ? "WHERE F10025 = 0" : String.Empty);
         
                if (this.chkVenit.Checked == true)
                {
                    if (sql.Contains("WHERE") == true)
                        sql += " AND A.F10003 IN (SELECT DISTINCT F92003 FROM F920 WHERE YEAR = " + Convert.ToInt32(lista["ANU"] ?? DateTime.Now.Year.ToString()) + ") ";
                    else
                        sql += " WHERE A.F10003 IN (SELECT DISTINCT F92003 FROM F920 WHERE YEAR = " + Convert.ToInt32(lista["ANU"] ?? DateTime.Now.Year.ToString()) + ") ";
                }

                if (this.chkCIC.Checked == true)
                {
                    if (sql.Contains("WHERE") == true)
                        sql += " AND A.F10003 IN (SELECT F10003 FROM F100, F110, f010 WHERE F10002 = F01002 AND F10003 = F11003 AND (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"])
                            + ") <=  YEAR(F11006) AND CASE WHEN (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"]) + ") =  YEAR(F11006) THEN F01012 ELSE 0 END <= MONTH(F11006)) ";
                    else
                        sql += " WHERE A.F10003 IN (SELECT F10003 FROM F100, F110, f010 WHERE F10002 = F01002 AND F10003 = F11003 AND (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"])
                            + ") <=  YEAR(F11006) AND CASE WHEN (F01011 - " + (lista["CIC"].Length <= 0 ? "2" : lista["CIC"]) + ") =  YEAR(F11006) THEN F01012 ELSE 0 END <= MONTH(F11006)) ";
                }

                //sql += " ORDER BY F10008 + ' ' + F10009";
                
            }

            DataTable dtAng = General.IncarcaDT(SelectAngajati(sql), null);
            cmbAng.DataSource = null;
            cmbAng.DataSource = dtAng;
            cmbAng.DataBind();
            cmbAng.SelectedIndex = -1;

        }


        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtl.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtl.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        //}



        static void FlatToOpc(XDocument doc, string docxPath)
        {
            XNamespace pkg =
                "http://schemas.microsoft.com/office/2006/xmlPackage";
            XNamespace rel =
                "http://schemas.openxmlformats.org/package/2006/relationships";
            using (Package package = Package.Open(docxPath, FileMode.Create))
            {
                // add all parts (but not relationships)
                foreach (var xmlPart in doc.Root
                    .Elements()
                    .Where(p =>
                        (string)p.Attribute(pkg + "contentType") !=
                        "application/vnd.openxmlformats-package.relationships+xml"))
                {
                    string name = (string)xmlPart.Attribute(pkg + "name");
                    string contentType = (string)xmlPart.Attribute(pkg + "contentType");
                    if (contentType.EndsWith("xml"))
                    {
                        Uri u = new Uri(name, UriKind.Relative);
                        PackagePart part = package.CreatePart(u, contentType,
                            CompressionOption.SuperFast);
                        using (Stream str = part.GetStream(FileMode.Create))
                        using (XmlWriter xmlWriter = XmlWriter.Create(str))
                            xmlPart.Element(pkg + "xmlData")
                                .Elements()
                                .First()
                                .WriteTo(xmlWriter);
                    }
                    else
                    {
                        Uri u = new Uri(name, UriKind.Relative);
                        PackagePart part = package.CreatePart(u, contentType,
                            CompressionOption.SuperFast);
                        using (Stream str = part.GetStream(FileMode.Create))
                        using (BinaryWriter binaryWriter = new BinaryWriter(str))
                        {
                            string base64StringInChunks =
                                (string)xmlPart.Element(pkg + "binaryData");
                            char[] base64CharArray = base64StringInChunks
                                .Where(c => c != '\r' && c != '\n').ToArray();
                            byte[] byteArray =
                                System.Convert.FromBase64CharArray(base64CharArray,
                                0, base64CharArray.Length);
                            binaryWriter.Write(byteArray);
                        }
                    }
                }
                foreach (var xmlPart in doc.Root.Elements())
                {
                    string name = (string)xmlPart.Attribute(pkg + "name");
                    string contentType = (string)xmlPart.Attribute(pkg + "contentType");
                    if (contentType ==
                        "application/vnd.openxmlformats-package.relationships+xml")
                    {
                        // add the package level relationships
                        if (name == "/_rels/.rels")
                        {
                            foreach (XElement xmlRel in
                                xmlPart.Descendants(rel + "Relationship"))
                            {
                                string id = (string)xmlRel.Attribute("Id");
                                string type = (string)xmlRel.Attribute("Type");
                                string target = (string)xmlRel.Attribute("Target");
                                string targetMode =
                                    (string)xmlRel.Attribute("TargetMode");
                                if (targetMode == "External")
                                    package.CreateRelationship(
                                        new Uri(target, UriKind.Absolute),
                                        TargetMode.External, type, id);
                                else
                                    package.CreateRelationship(
                                        new Uri(target, UriKind.Relative),
                                        TargetMode.Internal, type, id);
                            }
                        }
                        else
                        // add part level relationships
                        {
                            string directory = name.Substring(0, name.IndexOf("/_rels"));
                            string relsFilename = name.Substring(name.LastIndexOf('/'));
                            string filename =
                                relsFilename.Substring(0, relsFilename.IndexOf(".rels"));
                            PackagePart fromPart = package.GetPart(
                                new Uri(directory + filename, UriKind.Relative));
                            foreach (XElement xmlRel in
                                xmlPart.Descendants(rel + "Relationship"))
                            {
                                string id = (string)xmlRel.Attribute("Id");
                                string type = (string)xmlRel.Attribute("Type");
                                string target = (string)xmlRel.Attribute("Target");
                                string targetMode =
                                    (string)xmlRel.Attribute("TargetMode");
                                if (targetMode == "External")
                                    fromPart.CreateRelationship(
                                        new Uri(target, UriKind.Absolute),
                                        TargetMode.External, type, id);
                                else
                                    fromPart.CreateRelationship(
                                        new Uri(target, UriKind.Relative),
                                        TargetMode.Internal, type, id);
                            }
                        }
                    }
                }
            }
        }


        // <fieldset border = "0" >
        //  < legend class="legend-border"></legend>            
        //  <table width = "10%" >
        //      < tr >
        //          < td align="center">
        //              <dx:ASPxButton ID = "btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" Text="Generare" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)" >                                
        //                  <ClientSideEvents Click = "function(s,e){ OnGenerare(s); }" />
        //                  < Paddings Padding="0px" />
        //              </dx:ASPxButton>
        //          </td>
        //      </tr>

        //  </table>
        //</fieldset >


    }
}