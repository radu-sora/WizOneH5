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


        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                    

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
                Dami.AccesApp();             

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");


                btnInit.Text = Dami.TraduCuvant("btnInit", "Initializare");              
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

                //Radu 13.12.2019
                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    Session["InformatiaCurenta_PS"] = null;
                    Session["PtjSpecial_Id"] = "-1";
                    IncarcaAngajati();

                    //General.ExecutaNonQuery("DELETE FROM \"PtjSpecial_Sabloane\" WHERE \"Denumire\" IS NULL", null);                
                    string sql = "SELECT 0 AS \"Id\", '---' AS \"Denumire\", NULL AS \"IdProgram1\",  NULL AS \"IdProgram2\", NULL AS \"IdProgram3\", NULL AS \"IdProgram4\", NULL AS \"IdProgram5\", NULL AS \"IdProgram6\", NULL AS \"IdProgram7\", NULL AS \"IdProgram8\", NULL AS \"IdProgram9\", NULL AS \"IdProgram10\","
                        + " NULL AS \"IdProgram11\",  NULL AS \"IdProgram12\", NULL AS \"IdProgram13\", NULL AS \"IdProgram14\", NULL AS \"IdProgram15\", NULL AS \"IdProgram16\", NULL AS \"IdProgram17\", NULL AS \"IdProgram18\", NULL AS \"IdProgram19\", NULL AS \"IdProgram20\","
                        + " NULL AS \"IdProgram21\",  NULL AS \"IdProgram22\", NULL AS \"IdProgram23\", NULL AS \"IdProgram24\", NULL AS \"IdProgram25\", NULL AS \"IdProgram26\", NULL AS \"IdProgram27\", NULL AS \"IdProgram28\", NULL AS \"IdProgram29\", NULL AS \"IdProgram30\", NULL AS \"IdProgram31\","
                        + " NULL as \"NrZile\", NULL as \"Ziua1\", NULL as \"Ziua2\",  NULL as \"Ziua3\", NULL as \"Ziua4\", NULL as \"Ziua5\", NULL as \"Ziua6\", NULL as \"Ziua7\", NULL as \"Ziua8\", NULL as \"Ziua9\", NULL as \"Ziua10\", "
                        + " NULL as \"Ziua11\", NULL as \"Ziua12\",  NULL as \"Ziua13\", NULL as \"Ziua14\", NULL as \"Ziua15\", NULL as \"Ziua16\", NULL as \"Ziua17\", NULL as \"Ziua18\", NULL as \"Ziua19\", NULL as \"Ziua20\", "
                        + " NULL as \"Ziua21\", NULL as \"Ziua22\",  NULL as \"Ziua23\", NULL as \"Ziua24\", NULL as \"Ziua25\", NULL as \"Ziua26\", NULL as \"Ziua27\", NULL as \"Ziua28\", NULL as \"Ziua29\", NULL as \"Ziua30\", NULL as \"Ziua31\","
                        + " NULL as \"ValZiua1\", NULL as \"ValZiua2\",  NULL as \"ValZiua3\", NULL as \"ValZiua4\", NULL as \"ValZiua5\", NULL as \"ValZiua6\", NULL as \"ValZiua7\", NULL as \"ValZiua8\", NULL as \"ValZiua9\", NULL as \"ValZiua10\", "
                        + " NULL as \"ValZiua11\", NULL as \"ValZiua12\",  NULL as \"ValZiua13\", NULL as \"ValZiua14\", NULL as \"ValZiua15\", NULL as \"ValZiua16\", NULL as \"ValZiua17\", NULL as \"ValZiua18\", NULL as \"ValZiua19\", NULL as \"ValZiua20\", "
                        + " NULL as \"ValZiua21\", NULL as \"ValZiua22\",  NULL as \"ValZiua23\", NULL as \"ValZiua24\", NULL as \"ValZiua25\", NULL as \"ValZiua26\", NULL as \"ValZiua27\", NULL as \"ValZiua28\", NULL as \"ValZiua29\", NULL as \"ValZiua30\", NULL as \"ValZiua31\", 0 AS S, 0 AS D, 0 AS SL, 0 AS \"Decalare\" "
                        + (Constante.tipBD == 2 ? " FROM DUAL " : "") + " UNION "
                        + "SELECT  \"Id\",  \"Denumire\", \"IdProgram1\", \"IdProgram2\", \"IdProgram3\",  \"IdProgram4\",  \"IdProgram5\",  \"IdProgram6\",  \"IdProgram7\",  \"IdProgram8\",  \"IdProgram9\",  \"IdProgram10\","
                        + "  \"IdProgram11\",   \"IdProgram12\",  \"IdProgram13\",  \"IdProgram14\",  \"IdProgram15\",  \"IdProgram16\",  \"IdProgram17\",  \"IdProgram18\",  \"IdProgram19\",  \"IdProgram20\","
                        + "  \"IdProgram21\",   \"IdProgram22\",  \"IdProgram23\",  \"IdProgram24\",  \"IdProgram25\",  \"IdProgram26\",  \"IdProgram27\",  \"IdProgram28\",  \"IdProgram29\",  \"IdProgram30\",  \"IdProgram31\", \"NrZile\", \"Ziua1\", \"Ziua2\",  \"Ziua3\", \"Ziua4\", \"Ziua5\", \"Ziua6\", \"Ziua7\", \"Ziua8\", \"Ziua9\", \"Ziua10\", "
                        + " \"Ziua11\", \"Ziua12\",  \"Ziua13\", \"Ziua14\", \"Ziua15\", \"Ziua16\", \"Ziua17\", \"Ziua18\", \"Ziua19\", \"Ziua20\", "
                        + " \"Ziua21\", \"Ziua22\",  \"Ziua23\", \"Ziua24\", \"Ziua25\", \"Ziua26\", \"Ziua27\", \"Ziua28\", \"Ziua29\", \"Ziua30\", \"Ziua31\","
                        + " \"ValZiua1\", \"ValZiua2\",  \"ValZiua3\", \"ValZiua4\", \"ValZiua5\", \"ValZiua6\", \"ValZiua7\", \"ValZiua8\", \"ValZiua9\", \"ValZiua10\", "
                        + " \"ValZiua11\", \"ValZiua12\",  \"ValZiua13\", \"ValZiua14\", \"ValZiua15\", \"ValZiua16\", \"ValZiua17\", \"ValZiua18\", \"ValZiua19\", \"ValZiua20\", "
                        + " \"ValZiua21\", \"ValZiua22\",  \"ValZiua23\", \"ValZiua24\", \"ValZiua25\", \"ValZiua26\", \"ValZiua27\", \"ValZiua28\", \"ValZiua29\", \"ValZiua30\", \"ValZiua31\", S, D, SL, \"Decalare\" FROM \"PtjSpecial_Sabloane\" ";
                    DataTable dt = General.IncarcaDT(sql, null);
                    cmbSablon.DataSource = dt;
                    cmbSablon.DataBind();
                    Session["PtjSpecial_Sabloane"] = dt;

                    DataTable dtProgr = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Programe""", null);
                    Session["PtjSpecial_Programe"] = dtProgr;
                    string progr = "";
                    for (int i = 0; i < dtProgr.Rows.Count; i++)
                    {
                        progr += dtProgr.Rows[i]["Id"].ToString() + "," + dtProgr.Rows[i]["Denumire"].ToString();
                        if (i < dtProgr.Rows.Count - 1)
                            progr += ";";
                    }
                    Session["PtjSpecial_ProgrameJS"] = progr;
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

                    DataTable dtert = Session["InformatiaCurenta_PS"] as DataTable;

                    if (Session["InformatiaCurenta_PS"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta_PS"];
                        grDate.DataBind();
                    }
                    DataTable dt = Session["PtjSpecial_Sabloane"] as DataTable;
                    cmbSablon.DataSource = dt;
                    cmbSablon.DataBind();
                    
                    if (cmbNrZileSablon.Value != null)
                    {
                        for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                        {
                            ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                            ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                            if (tx != null)                            
                                tx.Visible = true;
                            if (lx != null)
                                lx.Visible = true;
                        }
                        for (int i = Convert.ToInt32(cmbNrZileSablon.Value) + 1; i <= 31; i++)
                        {
                            ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                            ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                            if (tx != null)
                                tx.Visible = false;
                            if (lx != null)
                                lx.Visible = false;
                        }
                    }             
                }

                DataTable table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Denumire", typeof(string));
                for (int i = 1; i <= 31; i++)
                    table.Rows.Add(i, i.ToString());
                cmbNrZileSablon.DataSource = table;
                cmbNrZileSablon.DataBind();

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

                IncarcaPopUp();

    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> lstMarci = new List<int>();
                //string[] sablon = new string[11];

                if (cmbSablon.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu a fost specificat sablonul!"), MessageBox.icoError);
                    return;
                }

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

                if (!chkPontare.Checked && !chkPlanif.Checked)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati specificat pentru pontare si/sau planificare!"), MessageBox.icoError);
                    return;
                }

                //for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                //{
                //    ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                //    if (tx != null && tx.Text.Length > 0) sablon[i] = tx.Text;
                //    else sablon[i] = "";
                //}



                DataTable table = Session["PtjSpecial_Sabloane"] as DataTable;
                DataRow sablon = table.Select("Id=" + cmbSablon.Value)[0];


                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    lstMarci.Add(Convert.ToInt32(General.Nz(lst[i], -99)));
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {
                    DateTime dt = Convert.ToDateTime(dtDataStart.Value);
                    while (dt.Year < Convert.ToDateTime(dtDataSfarsit.Value).Year || (dt.Year == Convert.ToDateTime(dtDataSfarsit.Value).Year && dt.Month <= Convert.ToDateTime(dtDataSfarsit.Value).Month))
                    {
                        General.PontajInitGeneral(Convert.ToInt32(Session["UserId"]), dt.Year, dt.Month);             
                        dt = dt.AddMonths(1);
                    }
                    string msg = InitializarePontajSpecial(Convert.ToDateTime(dtDataStart.Value), Convert.ToDateTime(dtDataSfarsit.Value), Convert.ToInt32(cmbNrZileSablon.Value), sablon, lstMarci);
                  
                    if (msg.Length > 0)
                        MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError);
                    else
                        MessageBox.Show(Dami.TraduCuvant("Initializare reusita!"), MessageBox.icoSuccess);

                }                
                 

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string InitializarePontajSpecial(DateTime dataStart, DateTime dataSf, int nrZile, DataRow sablon, List<int> lstMarci)
        {
            string msg = "";

            //int x = 1;
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
                string sqlZileGolite = "";
                for (int j = 0; j < lstMarci.Count; j++)
                {
                    lista += lstMarci[j].ToString();
                    if (j != lstMarci.Count - 1)
                        lista += ",";
                }


                string sqlSDSL = "";
                string cond = "";
                List<DateTime> lstInit = new List<DateTime>();

                for (int i = 1; i <= nrZile; i++)
                {
                    string data = "";
                    sqlSDSL = "";
                    List<DateTime> lstZileGolite = new List<DateTime>();

                    DateTime ziuaPrec = dataStart;

                    for (DateTime zi = dataStart.AddDays(i - 1); zi <= dataSf; zi = zi.AddDays(nrZile))
                    {
                        string dtStr = General.ToDataUniv(zi.Date.Year, zi.Date.Month, zi.Date.Day);

                        int nr = dtHolidays.Select("DAY = " + dtStr).Count();
                        int nrZileNel = 0;
                        if (zi.Date != ziuaPrec.Date)
                        {//calculam nr de zile nelucratoare intre cele 2 date
                            for (DateTime k = ziuaPrec; k <= zi; k = k.AddDays(1))
                            {
                                string dtK = General.ToDataUniv(k.Date.Year, k.Date.Month, k.Date.Day);

                                int nrK = dtHolidays.Select("DAY = " + dtK).Count();

                                if (!chkS.Checked && k.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    if (!lstZileGolite.Contains(k))
                                        lstZileGolite.Add(k);
                                    nrZileNel++;
                                }
                                if (!chkD.Checked && k.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    if (!lstZileGolite.Contains(k))
                                        lstZileGolite.Add(k);
                                    nrZileNel++;
                                }
                                if (!chkSL.Checked && nrK > 0)
                                {
                                    if (!lstZileGolite.Contains(k))
                                        lstZileGolite.Add(k);
                                    nrZileNel++;
                                }
                            }
                        }

                        int m = 0;
                        DateTime dtTemp = zi.AddDays(nrZileNel);
                        string dtT = General.ToDataUniv(dtTemp.Date.Year, dtTemp.Date.Month, dtTemp.Date.Day);
                        int nrT = dtHolidays.Select("DAY = " + dtT).Count();
                        while ((!chkS.Checked && dtTemp.DayOfWeek == DayOfWeek.Saturday) || (!chkD.Checked && dtTemp.DayOfWeek == DayOfWeek.Sunday) || (!chkSL.Checked && nrT > 0))
                        {
                            m++;                       
                            dtTemp = dtTemp.AddDays(1);
                            dtT = General.ToDataUniv(dtTemp.Date.Year, dtTemp.Date.Month, dtTemp.Date.Day);
                            nrT = dtHolidays.Select("DAY = " + dtT).Count();
                        }

                        if (chkDecalare.Checked)
                        {
                            zi = zi.AddDays(nrZileNel + m);
                            while (lstInit.Contains(zi))
                            {
                                zi = zi.AddDays(1);
                                while ((!chkS.Checked && zi.DayOfWeek == DayOfWeek.Saturday) || (!chkD.Checked && zi.DayOfWeek == DayOfWeek.Sunday)
                                    || (!chkSL.Checked && dtHolidays.Select("DAY = " + General.ToDataUniv(zi.Date.Year, zi.Date.Month, zi.Date.Day)).Count() > 0))
                                    zi = zi.AddDays(1);
                            }
                            if (zi > dataSf)
                                break;
                        }

                        if (nr > 0 || zi.DayOfWeek == DayOfWeek.Saturday || zi.DayOfWeek == DayOfWeek.Sunday)
                        {
                            lstMarciSDSL.Clear();
                            lista1 = "";
                            foreach (var marca in lstMarci)
                            {
                                int numar = dtCereri.Select("F10003 = " + marca + " AND DataInceput <= " + dtStr + " AND " + dtStr + " <= DataSfarsit").Count();
                                if (numar <= 0)
                                {
                                    if (!chkDecalare.Checked)
                                    {
                                        if (chkS.Checked && zi.DayOfWeek == DayOfWeek.Saturday)
                                            lstMarciSDSL.Add(marca);
                                        if (chkD.Checked && zi.DayOfWeek == DayOfWeek.Sunday)
                                            lstMarciSDSL.Add(marca);
                                        if (chkSL.Checked && nr > 0)
                                            lstMarciSDSL.Add(marca);
                                    }
                                    else
                                        lstMarciSDSL.Add(marca);
                                }
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

                                lstInit.Add(zi);
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
                            lstInit.Add(zi);  
                        }

                        ziuaPrec = zi;
                    }


                    if (Constante.tipBD == 1)
                        cond = " ISNUMERIC(\"ValStr\") = 1 ";
                    else
                        cond = " TRIM(TRANSLATE(\"ValStr\",'0123456789', ' ')) is null ";

                    string sql = "";
                    string oraIn = "", oraOut = "", firstIn = "", lastOut = "";

                    if (sablon["IdProgram" + i] != null && sablon["IdProgram" + i].ToString().Length > 0)
                    {
                        if (Constante.tipBD == 1)
                        {
                            oraIn = ", \"In1\" =convert(datetime, convert(varchar, Ziua, 111) + ' ' + convert(varchar, (select oraintrare from ptj_programe where id = " + sablon["IdProgram" + i].ToString() + "), 108), 120)";
                            oraOut = ", \"Out1\" =convert(datetime, convert(varchar, Ziua, 111) + ' ' + convert(varchar, (select oraiesire from ptj_programe where id = " + sablon["IdProgram" + i].ToString() + "), 108), 120)";
                            firstIn = ", \"FirstInPaid\" =convert(datetime, convert(varchar, Ziua, 111) + ' ' + convert(varchar, (select oraintrare from ptj_programe where id = " + sablon["IdProgram" + i].ToString() + "), 108), 120)";
                            lastOut = ", \"LastOutPaid\" =convert(datetime, convert(varchar, Ziua, 111) + ' ' + convert(varchar, (select oraiesire from ptj_programe where id = " + sablon["IdProgram" + i].ToString() + "), 108), 120)";

                        }
                        else
                        {
                            oraIn = ", \"In1\" =to_date(to_char(\"Ziua\", 'dd/mm/yyyy') || ' ' || to_char((select \"OraIntrare\" from \"Ptj_Programe\" WHERE \"Id\"=" + sablon["IdProgram" + i].ToString() + "), 'hh24:mi:ss'), 'dd/mm/yyyy hh24:mi:ss') ";
                            oraOut = ", \"Out1\" =to_date(to_char(\"Ziua\", 'dd/mm/yyyy') || ' ' || to_char((select \"OraIesire\" from \"Ptj_Programe\" WHERE \"Id\"=" + sablon["IdProgram" + i].ToString() + "), 'hh24:mi:ss'), 'dd/mm/yyyy hh24:mi:ss') ";
                            firstIn = ", \"FirstInPaid\" =to_date(to_char(\"Ziua\", 'dd/mm/yyyy') || ' ' || to_char((select \"OraIntrare\" from \"Ptj_Programe\" WHERE \"Id\"=" + sablon["IdProgram" + i].ToString() + "), 'hh24:mi:ss'), 'dd/mm/yyyy hh24:mi:ss') ";
                            lastOut = ", \"LastOutPaid\" =to_date(to_char(\"Ziua\", 'dd/mm/yyyy') || ' ' || to_char((select \"OraIesire\" from \"Ptj_Programe\" WHERE \"Id\"=" + sablon["IdProgram" + i].ToString() + "), 'hh24:mi:ss'), 'dd/mm/yyyy hh24:mi:ss') ";

                        }
                    }
                    else
                    {
                        oraIn = ", \"In1\" = NULL ";
                        oraOut = ", \"Out1\" = NULL ";
                        firstIn = ", \"FirstInPaid\" = NULL ";
                        lastOut = ", \"LastOutPaid\" = NULL ";
                    }

                    string sirVal = "";
                    List<string> listaVal = new List<string>();
                    if (sablon["ValZiua" + i] != null && sablon["ValZiua" + i].ToString().Length > 0)
                    {
                        string[] param = sablon["ValZiua" + i].ToString().Split(';');
                        for (int k = 0; k < param.Length; k++)
                        {
                            sirVal += ", \"" + param[k].Split('=')[0] + "\" = " + param[k].Split('=')[1] + " * 60";
                            listaVal.Add(param[k].Split('=')[0]);
                        }
                    }


                    string sirValZileGolite = "";
                    for (int k = 0; k <= 20; k++)
                    {
                        string val = "Val" + k;
                        if (!listaVal.Contains(val))
                        {
                            sirVal += ", \"" + val + "\" = NULL";
                        }
                        sirValZileGolite += ", \"" + val + "\" = NULL";
                    }

                    if (!chkPontare.Checked)
                    {
                        oraIn = "";
                        oraOut = "";
                        firstIn = "";
                        lastOut = "";
                        sirVal = "";
                    }

                    string planif = "";
                    if (chkPlanif.Checked)
                    {
                        planif = ", \"IdContractP\" =  \"IdContract\" , \"IdProgramP\" = " + (sablon["IdProgram" + i] != null && sablon["IdProgram" + i].ToString().Length > 0 ? sablon["IdProgram" + i].ToString() : "NULL");
                    }

                    if (data.Length > 0)
                    {
                        data = data.Substring(1);

                        sql = "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = " + (chkPontare.Checked ? "'" + (sablon["Ziua" + i] != null ? sablon["Ziua" + i].ToString() : "") + "' " : "NULL") + oraIn + oraOut + firstIn + lastOut + sirVal + planif + " WHERE \"Ziua\" IN (" + data + ") AND F10003 IN (" + lista + ") AND (\"ValStr\" IS NULL OR \"ValStr\" = '' OR \"ValStr\" = ' ' OR " + cond + " OR \"ValStr\" LIKE '%/%')";
                    }
                    if (lstZileGolite != null && lstZileGolite.Count > 0)
                    {
                        string dataZileGolite = "";
                        foreach (DateTime ziGolita in lstZileGolite)
                        {
                            if (Constante.tipBD == 1)
                                dataZileGolite += ", CONVERT(DATETIME, '" + ziGolita.Day.ToString().PadLeft(2, '0') + "/" + ziGolita.Month.ToString().PadLeft(2, '0') + "/" + ziGolita.Year.ToString() + "', 103) ";
                            else
                                dataZileGolite += ", TO_DATE('" + ziGolita.Day.ToString().PadLeft(2, '0') + "/" + ziGolita.Month.ToString().PadLeft(2, '0') + "/" + ziGolita.Year.ToString() + "', 'dd/mm/yyyy') ";
                        }
                        sqlZileGolite = "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = NULL, \"In1\" = NULL, \"Out1\" = NULL, \"FirstInPaid\" = NULL, \"LastOutPaid\" = NULL " + sirValZileGolite + " WHERE \"Ziua\" IN (" + dataZileGolite.Substring(1) + ") AND F10003 IN (" + lista + ") AND (\"ValStr\" IS NULL OR \"ValStr\" = '' OR \"ValStr\" = ' ' OR " + cond + " OR \"ValStr\" LIKE '%/%')";
                    }
                    if (sqlSDSL.Length > 0)
                    {
                        sqlSDSL = sqlSDSL.Substring(2);
                        sqlFin += sql + ";" + "UPDATE \"Ptj_Intrari\" SET \"ValStr\" = " + (chkPontare.Checked ? "'" + (sablon["Ziua" + i] != null ? sablon["Ziua" + i].ToString() : "") + "'" : "NULL") + oraIn + oraOut + firstIn + lastOut + sirVal + planif + " WHERE (" + sqlSDSL + ") AND (\"ValStr\" IS NULL OR \"ValStr\" = ''  OR \"ValStr\" = ' ' OR " + cond + " OR \"ValStr\" LIKE '%/%');";
                    }
                    else
                        sqlFin += sql + ";";

                }

                General.ExecutaNonQuery("BEGIN " + sqlFin + sqlZileGolite + ";" + " END;", null);

                //DateTime dt = dataStart;
                //while (dt.Year < dataSf.Year || (dt.Year == dataSf.Year && dt.Month <= dataSf.Month))
                //{
                //    General.CalculFormuleCumulatToti(dt.Year, dt.Month, $@" F10003 IN ({lista})");
                //    dt = dt.AddMonths(1);
                //}
                General.CalculFormuleCumulat($@"ent.F10003 IN ({lista}) AND {dataStart.Year * 100 + dataStart.Month} <= (ent.""An"" * 100 + ent.""Luna"") AND (ent.""An"" * 100 + ent.""Luna"") <= {dataSf.Year * 100 + dataSf.Month}");
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

        protected void btnFiltruSterge_Click()
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
                cmbCtr.Value = null;
                cmbCateg.Value = null;

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
                string sql = "";
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
                        //cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                        //cmbDept.DataBind();
                        btnFiltruSterge_Click();
                        return;
                    case "cmbNrZileSablon":
                        AscundeCtl();
                        for (int i = 1; i <= 10; i++)
                        {
                            ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                            ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                            if (tx != null)
                            {
                                tx.Visible = false;
                                tx.Text = "";
                            }
                            if (lx != null)
                            {
                                lx.Visible = false;
                                lx.Text = "";
                                lx.ToolTip = "";
                            }
                        }
                        if (cmbNrZileSablon.Value != null)
                            for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                            {
                                ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                                ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                                if (tx != null) tx.Visible = true;
                                if (lx != null) lx.Visible = true;
                            }
                        break;
                    case "cmbSablon":
                        AscundeCtl();                    
                        if (cmbSablon.Value != null)
                        {
                            if (Convert.ToInt32(cmbSablon.Value) > 0)
                            {//sablon existent
                                DataTable dt = Session["PtjSpecial_Sabloane"] as DataTable;
                                DataRow[] dr = dt.Select("Id = " + Convert.ToInt32(cmbSablon.Value));
                                DataTable dtProgr = Session["PtjSpecial_Programe"]as DataTable;
                                if (dr != null && dr.Count() > 0)
                                {
                                    Session["PtjSpecial_Id"] = dr[0]["Id"].ToString();                                   
                                    txtNumeSablon.Text = dr[0]["Denumire"].ToString();
                                    cmbNrZileSablon.Value = Convert.ToInt32(dr[0]["NrZile"].ToString());
                                    if (cmbNrZileSablon.Value != null)
                                        for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                                        {
                                            ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                                            ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                                            if (tx != null)
                                            {
                                                tx.Visible = true;
                                                tx.Text = (dr[0]["Ziua" + i.ToString()] as string ?? "").ToString();
                                            }
                                            if (lx != null)
                                            {
                                                lx.Visible = true;
                                                int idProgr = dr[0]["IdProgram" + i.ToString()] as int? ?? -1;
                                                lx.Text = (idProgr > 0 ? idProgr.ToString() : "-");
                                                if (dtProgr != null && dtProgr.Rows.Count > 0 && lx.Text != "-")
                                                    lx.ToolTip = dtProgr.Select("Id=" + lx.Text)[0]["Denumire"] as string ?? "";                                              
                                            }
                                        }
                                    chkS.Checked = Convert.ToInt32(dr[0]["S"] == null ? "0" : dr[0]["S"].ToString()) == 1 ? true : false;
                                    chkD.Checked = Convert.ToInt32(dr[0]["D"] == null ? "0" : dr[0]["D"].ToString()) == 1 ? true : false;
                                    chkSL.Checked = Convert.ToInt32(dr[0]["SL"] == null ? "0" : dr[0]["SL"].ToString()) == 1 ? true : false;
                                    chkDecalare.Checked = Convert.ToInt32(dr[0]["Decalare"] == null ? "0" : dr[0]["Decalare"].ToString()) == 1 ? true : false;
                                }
                            }
                            else
                            {//sablon nou
                                if (Convert.ToInt32(cmbSablon.Value) == 0)
                                {
                                    string cmp = "ISNULL";
                                    if (Constante.tipBD == 2) cmp = "NVL";
                                    General.ExecutaNonQuery("INSERT INTO \"PtjSpecial_Sabloane\" (\"Id\") SELECT " + cmp + "(MAX(\"Id\"), 0) + 1 FROM \"PtjSpecial_Sabloane\"", null);
                                    sql = "SELECT 0 AS \"Id\", '---' AS \"Denumire\", NULL AS \"IdProgram1\",  NULL AS \"IdProgram2\", NULL AS \"IdProgram3\", NULL AS \"IdProgram4\", NULL AS \"IdProgram5\", NULL AS \"IdProgram6\", NULL AS \"IdProgram7\", NULL AS \"IdProgram8\", NULL AS \"IdProgram9\", NULL AS \"IdProgram10\","
                        + " NULL AS \"IdProgram11\",  NULL AS \"IdProgram12\", NULL AS \"IdProgram13\", NULL AS \"IdProgram14\", NULL AS \"IdProgram15\", NULL AS \"IdProgram16\", NULL AS \"IdProgram17\", NULL AS \"IdProgram18\", NULL AS \"IdProgram19\", NULL AS \"IdProgram20\","
                        + " NULL AS \"IdProgram21\",  NULL AS \"IdProgram22\", NULL AS \"IdProgram23\", NULL AS \"IdProgram24\", NULL AS \"IdProgram25\", NULL AS \"IdProgram26\", NULL AS \"IdProgram27\", NULL AS \"IdProgram28\", NULL AS \"IdProgram29\", NULL AS \"IdProgram30\", NULL AS \"IdProgram31\", NULL as \"NrZile\", NULL as \"Ziua1\", NULL as \"Ziua2\",  NULL as \"Ziua3\", NULL as \"Ziua4\", NULL as \"Ziua5\", NULL as \"Ziua6\", NULL as \"Ziua7\", NULL as \"Ziua8\", NULL as \"Ziua9\", NULL as \"Ziua10\", "
                                      + " NULL as \"Ziua11\", NULL as \"Ziua12\",  NULL as \"Ziua13\", NULL as \"Ziua14\", NULL as \"Ziua15\", NULL as \"Ziua16\", NULL as \"Ziua17\", NULL as \"Ziua18\", NULL as \"Ziua19\", NULL as \"Ziua20\", "
                                      + " NULL as \"Ziua21\", NULL as \"Ziua22\",  NULL as \"Ziua23\", NULL as \"Ziua24\", NULL as \"Ziua25\", NULL as \"Ziua26\", NULL as \"Ziua27\", NULL as \"Ziua28\", NULL as \"Ziua29\", NULL as \"Ziua30\", NULL as \"Ziua31\","
                                      + " NULL as \"ValZiua1\", NULL as \"ValZiua2\",  NULL as \"ValZiua3\", NULL as \"ValZiua4\", NULL as \"ValZiua5\", NULL as \"ValZiua6\", NULL as \"ValZiua7\", NULL as \"ValZiua8\", NULL as \"ValZiua9\", NULL as \"ValZiua10\", "
                                      + " NULL as \"ValZiua11\", NULL as \"ValZiua12\",  NULL as \"ValZiua13\", NULL as \"ValZiua14\", NULL as \"ValZiua15\", NULL as \"ValZiua16\", NULL as \"ValZiua17\", NULL as \"ValZiua18\", NULL as \"ValZiua19\", NULL as \"ValZiua20\", "
                                      + " NULL as \"ValZiua21\", NULL as \"ValZiua22\",  NULL as \"ValZiua23\", NULL as \"ValZiua24\", NULL as \"ValZiua25\", NULL as \"ValZiua26\", NULL as \"ValZiua27\", NULL as \"ValZiua28\", NULL as \"ValZiua29\", NULL as \"ValZiua30\", NULL as \"ValZiua31\", 0 AS S, 0 AS D, 0 AS SL, 0 AS \"Decalare\" "
                                      + (Constante.tipBD == 2 ? " FROM DUAL " : "") + " UNION "
                                      + "SELECT  \"Id\",  \"Denumire\", \"IdProgram1\", \"IdProgram2\", \"IdProgram3\",  \"IdProgram4\",  \"IdProgram5\",  \"IdProgram6\",  \"IdProgram7\",  \"IdProgram8\",  \"IdProgram9\",  \"IdProgram10\","
                        + "  \"IdProgram11\",   \"IdProgram12\",  \"IdProgram13\",  \"IdProgram14\",  \"IdProgram15\",  \"IdProgram16\",  \"IdProgram17\",  \"IdProgram18\",  \"IdProgram19\",  \"IdProgram20\","
                        + "  \"IdProgram21\",   \"IdProgram22\",  \"IdProgram23\",  \"IdProgram24\",  \"IdProgram25\",  \"IdProgram26\",  \"IdProgram27\",  \"IdProgram28\",  \"IdProgram29\",  \"IdProgram30\",  \"IdProgram31\", \"NrZile\", \"Ziua1\", \"Ziua2\",  \"Ziua3\", \"Ziua4\", \"Ziua5\", \"Ziua6\", \"Ziua7\", \"Ziua8\", \"Ziua9\", \"Ziua10\", "
                                      + " \"Ziua11\", \"Ziua12\",  \"Ziua13\", \"Ziua14\", \"Ziua15\", \"Ziua16\", \"Ziua17\", \"Ziua18\", \"Ziua19\", \"Ziua20\", "
                                      + " \"Ziua21\", \"Ziua22\",  \"Ziua23\", \"Ziua24\", \"Ziua25\", \"Ziua26\", \"Ziua27\", \"Ziua28\", \"Ziua29\", \"Ziua30\", \"Ziua31\","
                                      + " \"ValZiua1\", \"ValZiua2\",  \"ValZiua3\", \"ValZiua4\", \"ValZiua5\", \"ValZiua6\", \"ValZiua7\", \"ValZiua8\", \"ValZiua9\", \"ValZiua10\", "
                                      + " \"ValZiua11\", \"ValZiua12\",  \"ValZiua13\", \"ValZiua14\", \"ValZiua15\", \"ValZiua16\", \"ValZiua17\", \"ValZiua18\", \"ValZiua19\", \"ValZiua20\", "
                                      + " \"ValZiua21\", \"ValZiua22\",  \"ValZiua23\", \"ValZiua24\", \"ValZiua25\", \"ValZiua26\", \"ValZiua27\", \"ValZiua28\", \"ValZiua29\", \"ValZiua30\", \"ValZiua31\", S, D, SL, \"Decalare\" FROM \"PtjSpecial_Sabloane\" ";
                                    DataTable tabela = General.IncarcaDT(sql, null);
                                    //cmbSablon.DataSource = tabela;
                                    //cmbSablon.DataBind();
                                    txtNumeSablon.Text = "";
                                    cmbNrZileSablon.SelectedIndex = -1;
                                    Session["PtjSpecial_Sabloane"] = tabela;
                                    Session["PtjSpecial_Id"] = tabela.Select("Denumire IS NULL OR Denumire = ''", "Id DESC")[0]["Id"].ToString(); 
                                }

                            }
                        }
                        break;
                    case "btnSablon":
                        DataTable tbl = Session["PtjSpecial_Sabloane"] as DataTable;
                        foreach (DataColumn col in tbl.Columns)
                            col.ReadOnly = false;
                        DataRow[] linii = tbl.Select("Id = " + Convert.ToInt32(Session["PtjSpecial_Id"].ToString())); 
                        linii[0]["Denumire"] = txtNumeSablon.Text;
                        if (cmbNrZileSablon.Value != null)
                        {
                            linii[0]["NrZile"] = Convert.ToInt32(cmbNrZileSablon.Value);
                            //linii[0]["IdProgram"] = cmbProgr.Value ?? DBNull.Value;
                            linii[0]["S"] = chkS.Checked ? 1 : 0;
                            linii[0]["D"] = chkD.Checked ? 1 : 0;
                            linii[0]["SL"] = chkSL.Checked ? 1 : 0;
                            linii[0]["Decalare"] = chkDecalare.Checked ? 1 : 0;
                            for (int i = 1; i <= Convert.ToInt32(cmbNrZileSablon.Value); i++)
                            {
                                ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                                ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                                if (tx != null)
                                {
                                    tx.Visible = true;
                                    //if (tx.Text.Length > 0)
                                    //{
                                        linii[0]["Ziua" + i.ToString()] = tx.Text;
                                       
                                        foreach (var item in txtValuri)
                                        {                                          
                                            if (item.Key == "txtZiua" + i.ToString())
                                                linii[0]["ValZiua" + i.ToString()] = item.Value;
                                            if (item.Key == "IdProgram_" + "txtZiua" + i.ToString())
                                                linii[0]["IdProgram" + i.ToString()] = item.Value ?? DBNull.Value;
                                        }                         
                                      
                                    //}
                                }
                                if (lx != null)
                                    lx.Visible = true;
                            }
                        }
                        General.SalveazaDate(tbl, "PtjSpecial_Sabloane");

                        cmbSablon.DataSource = tbl;
                        cmbSablon.DataBind();

                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Sablon salvat cu succes!");
                        break;
                    case "btnSterge":
                        General.ExecutaNonQuery("DELETE FROM \"PtjSpecial_Sabloane\" WHERE \"Id\" = " + Convert.ToInt32(cmbSablon.Value), null);
                        sql = "SELECT 0 AS \"Id\", '---' AS \"Denumire\", NULL AS \"IdProgram1\",  NULL AS \"IdProgram2\", NULL AS \"IdProgram3\", NULL AS \"IdProgram4\", NULL AS \"IdProgram5\", NULL AS \"IdProgram6\", NULL AS \"IdProgram7\", NULL AS \"IdProgram8\", NULL AS \"IdProgram9\", NULL AS \"IdProgram10\","
                        + " NULL AS \"IdProgram11\",  NULL AS \"IdProgram12\", NULL AS \"IdProgram13\", NULL AS \"IdProgram14\", NULL AS \"IdProgram15\", NULL AS \"IdProgram16\", NULL AS \"IdProgram17\", NULL AS \"IdProgram18\", NULL AS \"IdProgram19\", NULL AS \"IdProgram20\","
                        + " NULL AS \"IdProgram21\",  NULL AS \"IdProgram22\", NULL AS \"IdProgram23\", NULL AS \"IdProgram24\", NULL AS \"IdProgram25\", NULL AS \"IdProgram26\", NULL AS \"IdProgram27\", NULL AS \"IdProgram28\", NULL AS \"IdProgram29\", NULL AS \"IdProgram30\", NULL AS \"IdProgram31\", NULL as \"NrZile\", NULL as \"Ziua1\", NULL as \"Ziua2\",  NULL as \"Ziua3\", NULL as \"Ziua4\", NULL as \"Ziua5\", NULL as \"Ziua6\", NULL as \"Ziua7\", NULL as \"Ziua8\", NULL as \"Ziua9\", NULL as \"Ziua10\", "
                          + " NULL as \"Ziua11\", NULL as \"Ziua12\",  NULL as \"Ziua13\", NULL as \"Ziua14\", NULL as \"Ziua15\", NULL as \"Ziua16\", NULL as \"Ziua17\", NULL as \"Ziua18\", NULL as \"Ziua19\", NULL as \"Ziua20\", "
                          + " NULL as \"Ziua21\", NULL as \"Ziua22\",  NULL as \"Ziua23\", NULL as \"Ziua24\", NULL as \"Ziua25\", NULL as \"Ziua26\", NULL as \"Ziua27\", NULL as \"Ziua28\", NULL as \"Ziua29\", NULL as \"Ziua30\", NULL as \"Ziua31\","
                          + " NULL as \"ValZiua1\", NULL as \"ValZiua2\",  NULL as \"ValZiua3\", NULL as \"ValZiua4\", NULL as \"ValZiua5\", NULL as \"ValZiua6\", NULL as \"ValZiua7\", NULL as \"ValZiua8\", NULL as \"ValZiua9\", NULL as \"ValZiua10\", "
                          + " NULL as \"ValZiua11\", NULL as \"ValZiua12\",  NULL as \"ValZiua13\", NULL as \"ValZiua14\", NULL as \"ValZiua15\", NULL as \"ValZiua16\", NULL as \"ValZiua17\", NULL as \"ValZiua18\", NULL as \"ValZiua19\", NULL as \"ValZiua20\", "
                          + " NULL as \"ValZiua21\", NULL as \"ValZiua22\",  NULL as \"ValZiua23\", NULL as \"ValZiua24\", NULL as \"ValZiua25\", NULL as \"ValZiua26\", NULL as \"ValZiua27\", NULL as \"ValZiua28\", NULL as \"ValZiua29\", NULL as \"ValZiua30\", NULL as \"ValZiua31\", 0 AS S, 0 AS D, 0 AS SL, 0 AS \"Decalare\" "
                          + (Constante.tipBD == 2 ? " FROM DUAL " : "") + " UNION "
                          + "SELECT  \"Id\",  \"Denumire\", \"IdProgram1\", \"IdProgram2\", \"IdProgram3\",  \"IdProgram4\",  \"IdProgram5\",  \"IdProgram6\",  \"IdProgram7\",  \"IdProgram8\",  \"IdProgram9\",  \"IdProgram10\","
                        + "  \"IdProgram11\",   \"IdProgram12\",  \"IdProgram13\",  \"IdProgram14\",  \"IdProgram15\",  \"IdProgram16\",  \"IdProgram17\",  \"IdProgram18\",  \"IdProgram19\",  \"IdProgram20\","
                        + "  \"IdProgram21\",   \"IdProgram22\",  \"IdProgram23\",  \"IdProgram24\",  \"IdProgram25\",  \"IdProgram26\",  \"IdProgram27\",  \"IdProgram28\",  \"IdProgram29\",  \"IdProgram30\",  \"IdProgram31\", \"NrZile\", \"Ziua1\", \"Ziua2\",  \"Ziua3\", \"Ziua4\", \"Ziua5\", \"Ziua6\", \"Ziua7\", \"Ziua8\", \"Ziua9\", \"Ziua10\", "
                          + " \"Ziua11\", \"Ziua12\",  \"Ziua13\", \"Ziua14\", \"Ziua15\", \"Ziua16\", \"Ziua17\", \"Ziua18\", \"Ziua19\", \"Ziua20\", "
                          + " \"Ziua21\", \"Ziua22\",  \"Ziua23\", \"Ziua24\", \"Ziua25\", \"Ziua26\", \"Ziua27\", \"Ziua28\", \"Ziua29\", \"Ziua30\", \"Ziua31\","
                          + " \"ValZiua1\", \"ValZiua2\",  \"ValZiua3\", \"ValZiua4\", \"ValZiua5\", \"ValZiua6\", \"ValZiua7\", \"ValZiua8\", \"ValZiua9\", \"ValZiua10\", "
                          + " \"ValZiua11\", \"ValZiua12\",  \"ValZiua13\", \"ValZiua14\", \"ValZiua15\", \"ValZiua16\", \"ValZiua17\", \"ValZiua18\", \"ValZiua19\", \"ValZiua20\", "
                          + " \"ValZiua21\", \"ValZiua22\",  \"ValZiua23\", \"ValZiua24\", \"ValZiua25\", \"ValZiua26\", \"ValZiua27\", \"ValZiua28\", \"ValZiua29\", \"ValZiua30\", \"ValZiua31\", S, D, SL, \"Decalare\" FROM \"PtjSpecial_Sabloane\" ";
                        DataTable table = General.IncarcaDT(sql, null);
                        cmbSablon.DataSource = table;
                        cmbSablon.DataBind();
                        Session["PtjSpecial_Sabloane"] = table;
                        Session["PtjSpecial_Id"] = "-1";
                        txtNumeSablon.Text = "";
                        cmbNrZileSablon.SelectedIndex = -1;
                        cmbSablon.SelectedIndex = -1;
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
                    Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), Convert.ToInt32(cmbCateg.Value ?? -99));
                //dt.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };
                grDate.DataSource = dt;
                Session["InformatiaCurenta_PS"] = dt;
                grDate.DataBind();
                grDate.SettingsPager.PageSize = 25;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetF100NumeCompletPontajSpecial(int idUser, int idSubcomp = -99, int idFiliala = -99, int idSectie = -99, int idDept = -99, int idSubdept = -99, int idBirou = -99, int idAngajat = -9, int idCtr = -99, int idCateg = -99)
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


                string tmp = "", cond = "", condCtr= "";

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void AscundeCtl()
        {
            for (int i = 1; i <= 31; i++)
            {
                ASPxTextBox tx = FindControlRecursive(this, "txtZiua" + i.ToString()) as ASPxTextBox;
                ASPxTextBox lx = FindControlRecursive(this, "lblZiua" + i.ToString()) as ASPxTextBox;
                if (tx != null)
                {
                    tx.Visible = false;
                    tx.Text = "";
                }
                if (lx != null)
                {
                    lx.Visible = false;
                    lx.Text = "";
                    lx.ToolTip = "";
                }
            }
        }
            


        protected void grDate_DataBound(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaPopUp()
        {
            try
            {
                DataTable dtAbs = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""DenumireScurta"" FROM ""Ptj_tblAbsente"" WHERE COALESCE(""IdTipOre"",1)=1", null);
                cmbTipAbs.DataSource = dtAbs;
                cmbTipAbs.DataBind();

                DataTable dtProgr = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_Programe""", null);
                cmbProgr.DataSource = dtProgr;
                cmbProgr.DataBind();

                DataTable dtVal = General.IncarcaDT($@"SELECT COALESCE(A.""OreInVal"",'') AS ""ValAbs"", A.""DenumireScurta"", A.""Denumire"", A.""Id"", COALESCE(A.""NrMax"", 23) AS ""NrMax"" 
                        FROM ""Ptj_tblAbsente"" A WHERE  COALESCE(A.""IdTipOre"",1)=0 AND A.""OreInVal"" IS NOT NULL AND RTRIM(LTRIM(A.""OreInVal"")) <> '' ORDER BY A.""OreInVal"" ", null);

                string pre = "flo1";

                for (int i = 0; i < dtVal.Rows.Count; i++)
                {
                    DataRow dr = dtVal.Rows[i];
                    string id = dr["ValAbs"] + "_" + General.Nz(dr["DenumireScurta"], "").ToString().Trim().Replace(" ", "");
                    //string id = General.Nz(dr["DenumireScurta"], "").ToString().Trim().Replace(" ", "");
                    if (id == "")
                    {
                        id = pre;
                        pre += "flo1";
                    }

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
                    txt.DecimalPlaces = 0;
                    txt.NumberType = SpinEditNumberType.Integer;
                    //txt.Attributes["data-val"] = General.Nz(dr["DenumireScurta"], "").ToString().Trim().Replace(" ", "");

                    //if (General.Nz(dr["ValAbs"], "").ToString() != "")
                    //{
                    //    try
                    //    {
                    //        int min = Convert.ToInt32(General.Nz(dr[dr["ValAbs"].ToString()], "0"));
                    //        txt.Text = (min / 60).ToString();
                    //    }
                    //    catch (Exception) { }
                    //}

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


    }
}
 
 
 
 
 
 
 
 
 
 
 
 