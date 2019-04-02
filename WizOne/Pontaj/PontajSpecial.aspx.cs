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

namespace WizOne.Pontaj
{
    public partial class PontajSpecial : System.Web.UI.Page
    {

        private string arrZL = "";

        public class metaActiuni
        {
            public int F10003 { get; set; }
            public int IdStare { get; set; }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
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

                //Radu 31.01.2019 - pentru ca ASPxSpinEdit din popUpModif sa afiseze '.' si nu ',' ca separator zecimal 
                CultureInfo newCulture = (CultureInfo)CultureInfo.CurrentCulture.Clone();         
                newCulture.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;
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


                btnSave.Text = Dami.TraduCuvant("btnSave", "Initializare");              
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                btnFiltruSterge.Text = Dami.TraduCuvant("btnFiltruSterge", "Sterge Filtru");

            
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
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

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta"] = null;
                    
                    IncarcaAngajati();             

  
                }
                else
                {
                    DataTable dtCmb = Session["SurseCombo"] as DataTable;
                    GridViewDataComboBoxColumn colAbs = (grDate.Columns["ValAbs"] as GridViewDataComboBoxColumn);
                    if (colAbs != null) colAbs.PropertiesComboBox.DataSource = dtCmb;

                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();
                    cmbAng.DataSource = Session["PontajSpecial_Angajati"];
                    cmbAng.DataBind();

                    DataTable dtert = Session["InformatiaCurenta"] as DataTable;

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

    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> lstMarci = new List<int>();
                string[] sablon = new string[11];

                if (dtDataStart.Value == null || dtDataSfarsit.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipseste data start/data sfarsit!"), MessageBox.icoError);
                    return;
                }

                if (Convert.ToDateTime(dtDataSfarsit.Value) < Convert.ToDateTime(dtDataStart.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Data de sfarsit este anterioara datei de start!"), MessageBox.icoError);
                    return;
                }

                if (cmbNrZileSablon.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu a fost specificat numarul de zile din sablon!"), MessageBox.icoError);
                    return;
                }

                for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                {
                    ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                    if (tx != null && tx.Text.Length > 0) sablon[i] = tx.Text;
                    else sablon[i] = "";
                }


                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    //object[] arr = lst[i] as object[];
                    lstMarci.Add(Convert.ToInt32(General.Nz(lst[i], -99)));
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {
                    string msg = InitializarePontajSpecial(Convert.ToDateTime(dtDataStart.Value), Convert.ToDateTime(dtDataSfarsit.Value), Convert.ToInt32(cmbNrZileSablon.Value), sablon, lstMarci);
                  
                    if (msg.Length > 0)
                        MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError);
                    else
                        MessageBox.Show(Dami.TraduCuvant("Initializare reusita!"), MessageBox.icoSuccess);

                    //else
                    //    MessageBox.Show("Initializarea pontajului se va finaliza in aproximativ 1-2 minute.", "", MessageBoxButton.OK);
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string InitializarePontajSpecial(DateTime dataStart, DateTime dataSf, int nrZile, string[] sablon, List<int> lstMarci)
        {
            string msg = "";

            int x = 1;
            List<int> lstMarciSDSL = new List<int>();
            string dtStart = General.ToDataUniv(dataStart.Date.Year, dataStart.Date.Month, dataStart.Date.Day);
            string dtSf = General.ToDataUniv(dataSf.Date.Year, dataSf.Date.Month, dataSf.Date.Day);
            DataTable dtHolidays = General.IncarcaDT("SELECT * FROM HOLIDAYS", null);

            DataTable dtAbs = General.IncarcaDT("SELECT * FROM \"Ptj_tblAbsente\" WHERE \"DenumireScurta\" = 'COP'", null);            
            int idAbsenta = -1;
            if (dtAbs != null && dtAbs.Rows.Count > 0)
                idAbsenta = Convert.ToInt32(dtAbs.Rows[0]["Id"].ToString());

            DataTable dtCereri = General.IncarcaDT("SELECT * FROM \"Ptj_Cereri\" WHERE \"DataInceput\" <= " + dtSf + " AND " + dtStart + " <= \"DataSfarsit\" AND \"IdStare\" = 3 AND \"IdAbsenta\" <> " + idAbsenta, null);

            try
            {
                string lista = "", sqlFin = "", lista1 = "";
                for (int j = 0; j < lstMarci.Count; j++)
                {
                    lista += lstMarci[j].ToString();
                    if (j != lstMarci.Count - 1)
                        lista += ",";
                }

   
                string sqlSDSL = "";
                string cond = "";
                for (int i = 1; i <= nrZile; i++)
                {
                    string data = "";
                    sqlSDSL = "";

                    for (DateTime zi = dataStart.AddDays(i - 1); zi <= dataSf; zi = zi.AddDays(nrZile))
                    {
                        string dtStr = General.ToDataUniv(zi.Date.Year, zi.Date.Month, zi.Date.Day);                    

                        int nr = dtHolidays.Select("DAY = " + dtStr).Count();

                        if (nr > 0 || zi.DayOfWeek == DayOfWeek.Saturday || zi.DayOfWeek == DayOfWeek.Sunday)
                        {
                            lstMarciSDSL.Clear();
                            lista1 = "";
                            foreach (var marca in lstMarci)
                            {
                                int numar = dtCereri.Select("F10003 = " + marca + " AND DataInceput <= " + dtStr + " AND " + dtStr + " <= DataSfarsit").Count();
                                if (numar <= 0)
                                    lstMarciSDSL.Add(marca);
                            }

                            if (lstMarciSDSL.Count > 0)
                            {
                                for (int k = 0; k < lstMarciSDSL.Count; k++)
                                {
                                    lista1 += lstMarciSDSL[k].ToString();
                                    if (k != lstMarciSDSL.Count - 1)
                                        lista1 += ",";
                                }

                                string data1 = "";
                                if (Constante.tipBD == 1)
                                    data1 += " CONVERT(DATETIME, '" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "', 103) ";
                                else
                                    data1 += " TO_DATE('" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "', 'dd/mm/yyyy') ";


                                sqlSDSL += "OR (\"Ziua\" = " + data1 + " AND F10003 IN (" + lista1 + ")) ";
                            }

                        }
                        else
                        {
                            data += ",";
                            if (Constante.tipBD == 1)
                                data += " CONVERT(DATETIME, '" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "', 103) ";
                            else
                                data += " TO_DATE('" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "', 'dd/mm/yyyy') ";
                        }
                    }

                    data = data.Substring(1);

                    if (Constante.tipBD == 1)
                        cond = " ISNUMERIC(\"ValStr\") = 1 ";
                    else
                        cond = " TRIM(TRANSLATE(\"ValStr\",'0123456789', ' ')) is null ";

                    string sql = "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = '" + sablon[i] + "' WHERE \"Ziua\" IN (" + data + ") AND F10003 IN (" + lista + ") AND (\"ValStr\" IS NULL OR \"ValStr\" = '' OR \"ValStr\" = ' ' OR " + cond + " OR \"ValStr\" LIKE '%/%')";

                    if (sqlSDSL.Length > 0)
                    {
                        sqlSDSL = sqlSDSL.Substring(2);
                        sqlFin += sql + ";" + "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = '" + sablon[i] + "' WHERE (" + sqlSDSL + ") AND (\"ValStr\" IS NULL OR \"ValStr\" = ''  OR \"ValStr\" = ' ' OR " + cond + " OR \"ValStr\" LIKE '%/%');";
                    }
                    else
                        sqlFin += sql + ";";
                 
                }       

                //ctx.CommandTimeout = 600;
                General.ExecutaNonQuery("BEGIN " + sqlFin + " END;", null);

                //msg = "Initializare reusita!";        

            }
            catch (Exception ex)
            {
                msg = "Eroare la initializare!";
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

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
           

                cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                cmbDept.DataBind();
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
                    case "cmbNrZileSablon":
                        for (int i = 1; i <= 10; i++)
                        {
                            ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                            if (tx != null)
                            {
                                tx.Visible = false;
                                tx.Text = "";
                            }
                        }
                        if (cmbNrZileSablon.Value != null)
                            for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                            {
                                ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                                if (tx != null) tx.Visible = true;
                            }
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

        public static Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            return root.Controls.Cast<Control>()
               .Select(c => FindControlRecursive(c, id))
               .FirstOrDefault(c => c != null);
        }

        private void IncarcaGrid()
        {
            try
            {
                grDate.KeyFieldName = "F10003";

                DataTable dt = GetF100NumeCompletPontajSpecial(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99),
                    Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99));
                //dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
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

        public DataTable GetF100NumeCompletPontajSpecial(int idUser, int idSubcomp = -99, int idFiliala = -99, int idSectie = -99, int idDept = -99, int idAngajat = -99)
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
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"" 

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
                                F00204 AS ""Companie"", F00305 AS ""Subcompanie"", F00406 AS ""Filiala"", F00507 AS ""Sectie"", F00608 AS ""Dept"", F00709 AS ""Subdept"",  F00810 AS ""Birou"" 

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
  
                                                           
                                ) Y ";


                string tmp = "", cond = "";

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

                if (idAngajat != -99)
                {
                    tmp = string.Format("  Y.F10003 = {0} ", idAngajat);
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

                strSql = string.Format(strSql, op, idUser);

                dt = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
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
                Session["PontajSpecial_Angajati"] = dt;
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
                string semn = "+";
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2)
                {
                    semn = "||";
                    cmp = "ROWNUM";
                }

                strSql = @"SELECT {2} AS ""IdAuto"", X.* FROM (
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
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
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
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
                                WHERE J.""IdUser"" = {0} ) X WHERE F10025 IN (0, 999) ORDER BY X.""NumeComplet"" ";

                strSql = string.Format(strSql, Session["UserId"].ToString(), semn, cmp);    

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

                //if (e.DataColumn.FieldName == "StareDenumire")
                //{
                //    object col = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                //    if (col != null) e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(col.ToString());
                //}

                //if (e.DataColumn.FieldName.Length >= 4 && e.DataColumn.FieldName.ToLower().Substring(0, 4) == "ziua")
                //{
                //    if (arrZL.Length > 0 && arrZL.IndexOf(e.DataColumn.FieldName + ";") >= 0)
                //        e.Cell.BackColor = System.Drawing.Color.Aquamarine;
                //}


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
                    //string[] arr = e.Parameters.Split(';');
                    //if (arr.Length == 0 || arr[0] == "")
                    //{
                    //    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                    //    return;
                    //}

                    
                    //switch(arr[0].ToString())
                    //{
                    //    case "btnPeAng":
                    //        {
                    //            RetineFiltru("1");
                    //            ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=10&f10003=" + txtCol["f10003"] + "&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                    //        }                            
                    //        break;
                    //    case "btnPeZi":
                    //        {
                    //            string ziua = txtCol["coloana"].ToString().Replace("Ziua", "");
                    //            RetineFiltru(ziua);
                    //            ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=20&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                    //        }
                    //        break;
                    //    case "grDate":
                    //        {
                    //            RetineFiltru("1");
                    //            ASPxWebControl.RedirectOnCallback("PontajDetaliat.aspx?tip=10&f10003=" + txtCol["f10003"] + "&Ziua=" + txtCol["coloana"] + "&idxPag=" + grDate.PageIndex + "&idxRow=" + grDate.FocusedRowIndex);
                    //        }
                    //        break;
                    //    case "colHide":
                    //        grDate.Columns[arr[1]].Visible = false;
                    //        break;
                    //}
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
                //var lstZile = new Dictionary<int, object>();

                //var grid = sender as ASPxGridView;
                ////for (int i = grid.VisibleStartIndex; i < grid.VisibleStartIndex + grid.SettingsPager.PageSize; i++)
                //for (int i = 0; i < grid.VisibleRowCount; i++)
                //{
                //    var rowValues = grid.GetRowValues(i, new string[] { "F10003", "ZileLucrate" }) as object[];
                //    lstZile.Add(Convert.ToInt32(rowValues[0] ?? (-1 * i)), rowValues[1] ?? "");
                //}

                //grid.JSProperties["cp_ZileLucrate"] = lstZile;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




    }
}
 
 
 
 
 
 
 
 
 
 
 
 