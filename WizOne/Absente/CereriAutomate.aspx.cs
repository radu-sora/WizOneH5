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
using System.Threading;

namespace WizOne.Absente
{
    public partial class CereriAutomate : System.Web.UI.Page
    {
        List<int> lstMarciProcesate = new List<int>();

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


                btnGen.Text = Dami.TraduCuvant("btnGen", "Generare");              
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

                txtNr.MinValue = 1;
                txtNr.MaxValue = 1000;

                var ert = txtNrOre.Value;
                var edc = txtNrOreInMinute.Value;

                if (!IsPostBack)
                {
                    string sir = "";
                    Session["InformatiaCurenta_CereriAut"] = null;
                    DataTable dtTemp = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'AbsenteCereriAutomate'", null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        sir = dtTemp.Rows[0][0].ToString();
                   
                    dtTemp = General.IncarcaDT((Constante.tipBD == 1 ? "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'GENERARE_AUTOMATA_CERERI'" :
                        "SELECT COUNT(*) FROM user_views where view_name = 'GENERARE_AUTOMATA_CERERI'"), null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtTemp.Rows[0][0].ToString()) > 0)
                    {
                        dtTemp = General.IncarcaDT("SELECT DISTINCT \"IdAbsenta\" FROM GENERARE_AUTOMATA_CERERI", null);
                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                        {
                            if (sir.Length > 0)
                                sir += ", ";
                            for (int i = 0; i < dtTemp.Rows.Count; i++)
                            {
                                sir += dtTemp.Rows[i][0].ToString();
                                if (i < dtTemp.Rows.Count - 1)
                                    sir += ", ";
                            }
                        }
                    }      

                    DataTable dtAng = General.IncarcaDT(SelectAngajatiRol(-44), null, "F10003;Rol");

                    DataView view = new DataView(dtAng);
                    DataTable dtRol = view.ToTable(true, "Rol", "RolDenumire");                  
                    cmbRol.DataSource = dtRol;
                    cmbRol.DataBind();
                    cmbRol.SelectedIndex = 0;

                    IncarcaAngajati();
                    DataTable dt = null;
                    if (sir.Length > 0)
                        dt = General.IncarcaDT("SELECT a.*, c.\"IdAuto\" as \"IdCircuit\" FROM \"Ptj_tblAbsente\" a LEFT JOIN \"Ptj_Circuit\" C ON a.\"IdGrupAbsenta\" = c.\"IdGrupAbsente\" WHERE a.\"Id\" IN (" + sir + ") AND (c.\"UserIntrod\" = - 1 * " + cmbRol.Value + " OR c.\"UserIntrod\" = " + Session["UserId"].ToString() + ")" , null);

                    cmbAbs.DataSource = dt;
                    cmbAbs.DataBind();
                    Session["CereriAut_Absente"] = dt;
                    Session["CereriAut_Sir"] = sir;
                    AfisareCtl("cmbAbs;0");

                    DataTable dtAngFiltrati = dtAng;
                    if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != -44 && dtAng != null && dtAng.Rows.Count > 0 && dtAng.Select("Rol=" + cmbRol.Value).Count() > 0)
                        dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();

                    DataTable dtAngActivi = new DataTable();
                    if (dtAngFiltrati != null && dtAngFiltrati.Rows.Count > 0 && dtAngFiltrati.Select("AngajatActiv=1").Count() > 0) dtAngActivi = dtAngFiltrati.Select("AngajatActiv=1").CopyToDataTable();
                    cmbAng.DataSource = dtAngActivi;
                    Session["CereriAut_Angajati"] = dtAngActivi;                  
                    Session["CereriAut_AngajatiToti"] = dtAngFiltrati;
                    cmbAng.DataBind();

                }
                else
                {               
                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();
                    cmbAng.DataSource = Session["CereriAut_Angajati"];
                    cmbAng.DataBind();

                    DataTable dtert = Session["InformatiaCurenta_CereriAut"] as DataTable;

                    if (Session["InformatiaCurenta_CereriAut"] != null)
                    {
                        grDate.DataSource = Session["InformatiaCurenta_CereriAut"];
                        grDate.DataBind();
                    }

                    DataTable dt = Session["CereriAut_Absente"] as DataTable;
                    cmbAbs.DataSource = dt;
                    cmbAbs.DataBind();

                    AfisareCtl("cmbAbs;" + (cmbAbs.Value ?? 0));

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


                ProgressUpdatePanel.ContentTemplateContainer.Controls.Add(lblProgres);
                ProgressUpdatePanel.Update();
                lblProgres.Text = "";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnGen_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> lstMarci = new List<int>();
                string[] sablon = new string[11];
                lstMarciProcesate.Clear();

                lblProgres.Text = "";
                ProgressUpdatePanel.ContentTemplateContainer.Controls.Add(lblProgres);
                ProgressUpdatePanel.Update();
                                      
                if (cmbAbs.Value == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat tip absenta!"), MessageBox.icoError);
                    return;
                }

                if (dtDataInc.Value == null || (dtDataSf.Visible == true && dtDataSf.Value == null))
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipseste data start/data sfarsit!"), MessageBox.icoError);
                    return;
                }

                if (dtDataSf.Visible == true && Convert.ToDateTime(dtDataSf.Value) < Convert.ToDateTime(dtDataInc.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Data de sfarsit este anterioara datei de start!"), MessageBox.icoError);
                    return;
                }

                //if (txtNr.Value == null)
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Nu ati specificat numarul de ore!"), MessageBox.icoError);
                //    return;
                //}
                var goigi = txtNrOreInMinute.Text;
                if (!rbPrel1.Checked && dtDataSf.Visible == false && txtNrOre.Text.Length <= 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati specificat numarul de ore!"), MessageBox.icoError);
                    return;
                }

                if (!rbPrel1.Checked && (cmbOraInc.Visible == true && cmbOraInc.Text == ""))
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati specificat ora inceput!"), MessageBox.icoError);
                    return;              
                }
                if (!rbPrel1.Checked && (cmbOraSf.Visible == true && cmbOraSf.Text == ""))
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati specificat ora sfarsit!"), MessageBox.icoError);
                    return;
                }

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati selectat niciun angajat!"), MessageBox.icoError);
                    return;
                }

                //if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && txtNrOre.Value == null) strErr += ", " + Dami.TraduCuvant("nr. ore");


                for (int i = 0; i < lst.Count(); i++)
                {
                    lstMarci.Add(Convert.ToInt32(General.Nz(lst[i], -99)));
                }
        

                grDate.Selection.UnselectAll();
                string msg = "";
                bool cont = true;
                while (cont)
                {
                    try
                    {

                        if (lstMarci.Count > 0)
                        {   
                            msg += GenerareCereri(lstMarci);
                            cont = false;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private string GenerareCereri(List<int> lstMarci)
        {
            //string msg = "";
            string err = "";
            Dictionary<int, int> lstOre = new Dictionary<int, int>();
            if (rbPrel1.Checked)
            {
                //lstMarci.Clear();
                DateTime data = Convert.ToDateTime(dtDataInc.Value);
                //int dept = Convert.ToInt32(cmbDept.Value);
                int idAbsenta = Convert.ToInt32(cmbAbs.Value);
                string dt = " CONVERT(DATETIME, '" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 103) ";
                if (Constante.tipBD == 2) dt = " TO_DATE('" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 'dd/MM/yyyy') ";

                string sql = "SELECT * FROM GENERARE_AUTOMATA_CERERI a WHERE  a.\"IdAbsenta\" =  " + idAbsenta + " AND \"Data\" = " + dt + " AND \"NrOre\" > 0 AND "
                           + " F10003 IN (select b.F10003 from \"F100Supervizori\" b where b.\"IdSuper\" = (select (-1) * \"UserIntrod\" from \"Ptj_Circuit\" where \"IdAuto\" = a.\"IdCircuit\") and b.\"IdUser\" = " + Session["UserId"].ToString() + ") ";

                DataTable dtPrel = General.IncarcaDT(sql, null);               
                for (int i = 0; i < dtPrel.Rows.Count; i++)
                {
                    if (lstMarci.Contains(Convert.ToInt32(dtPrel.Rows[i]["F10003"].ToString())))
                        lstOre.Add(Convert.ToInt32(dtPrel.Rows[i]["F10003"].ToString()), Convert.ToInt32(dtPrel.Rows[i]["NrOre"].ToString()));
                }              
            }
            int x = 0, y = 1;

            ScriptManager.RegisterClientScriptBlock(Page, typeof(string), "bindButton", "bindButton();", true);

            string strF10003 = "";

            foreach (int marca in lstMarci)
            {              
                lblProgres.Text = "Procesat " + y + " din " + lstMarci.Count + " angajati...";
                ProgressUpdatePanel.ContentTemplateContainer.Controls.Add(lblProgres);
                ProgressUpdatePanel.Update();
                y++;
                //System.Threading.Thread.Sleep(1000);

                if (lstMarciProcesate.Contains(marca))
                    continue;

                if (rbPrel1.Checked)
                    if (!lstOre.ContainsKey(marca))
                    {//angajatul selectat in grid nu are ore in view-ul GENERARE_AUTOMATA_CERERI
                        lstMarciProcesate.Add(marca);
                        continue;
                    }

                #region Salvare in baza


                #region Construim istoricul
                DataTable dtAbs = General.IncarcaDT(General.SelectAbsente(marca.ToString(), Convert.ToDateTime(dtDataInc.Value ?? DateTime.Now.Date)), null);
                DataRow[] lst = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                if (lst.Count() == 0)
                {
                    if (dtAbs.Rows.Count <= 0)
                    {
                        err += "Angajatul cu marca " + marca + " nu are definit contract!\n";
                        lstMarciProcesate.Add(marca);
                        continue;
                    }
                    else
                    {
                        return "Nu este definit circuit pentru acest tip de absenta!";                       
                    }
                    
                }
                DataRow drAbs = lst[0];

                string sqlIst;
                int trimiteLaInlocuitor;

                int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={marca} AND F10022 <= {General.ToDataUniv(Convert.ToDateTime(dtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime((dtDataSf.Visible ? dtDataSf.Value : dtDataInc.Value)))} <= F10023", null), 0));
                if (esteActiv == 0)
                {
                    err += "In perioada solicitata, angajatul cu marca " + marca + " este inactiv\n";
                    lstMarciProcesate.Add(marca);
                    continue;
                }

                if (Convert.ToInt32(General.Nz(marca, -98)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -97)) && Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { marca })) == 0)
                {
                    err += "Angajatul cu marca " + marca + " nu are nici un supervizor\n";
                    lstMarciProcesate.Add(marca);
                    continue;
                }

                int intersec = Convert.ToInt32(General.ExecutaScalar($@"
                                SELECT COUNT(*) 
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.F10003 = {marca} AND A.""DataInceput"" <= {General.ToDataUniv((dtDataSf.Visible ? dtDataSf.Date : dtDataInc.Date))} AND {General.ToDataUniv(dtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND A.""IdStare"" IN (1,2,3,4) AND B.""GrupOre"" IN({General.Nz(drAbs["GrupOreDeVerificat"], -99)})", null));

                if (intersec > 0)
                {
                    err += "Intervalul pentru angajatul cu marca " + marca + " se intersecteaza cu altul deja existent\n";
                    lstMarciProcesate.Add(marca);
                    continue;
                }


                General.SelectCereriIstoric(marca, -1, Convert.ToInt32(drAbs["IdCircuit"]), 0, out sqlIst, out trimiteLaInlocuitor);

                #endregion

                string sqlCer = "";
                string sqlPre = "";
                string strGen = "";

                if (Constante.tipBD == 1)
                {
                    sqlCer = CreazaSelectCuValori(marca, 1, lstOre.ContainsKey(marca) ? lstOre[marca] : -99);

                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"", ""AreAtas"", USER_NO, TIME, ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") 
                                OUTPUT Inserted.Id, Inserted.IdStare ";

                    strGen = "BEGIN TRAN " +
                            sqlIst + "; " +
                            sqlPre +
                            sqlCer + "; " +
                            "COMMIT TRAN";
                }
                else
                {
                    sqlCer = CreazaSelectCuValori(marca, 2);
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"", ""AreAtas"", USER_NO, TIME, ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") ";

                    strGen = "BEGIN " +
                                sqlIst + "; " + Environment.NewLine +
                                sqlPre +
                                sqlCer + " RETURNING \"Id\", \"IdStare\" INTO @out_1, @out_2; " +
                                @"
                                EXCEPTION
                                    WHEN DUP_VAL_ON_INDEX THEN
                                        ROLLBACK;
                            END;";
                }


                if (Dami.ValoareParam("LogCereri", "0") == "1") General.CreazaLogCereri(strGen, marca.ToString(), dtDataInc.Value.ToString());


                string msg = Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", 1 AS \"Actiune\", 1 AS \"IdStareViitoare\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {

                    err += "Pentru angajatul cu marca " + marca + ":" + msg.Substring(2) + "\n";
                    lstMarciProcesate.Add(marca);
                    continue;                
                }
                
                int idCer = 1;
                int idStare = 1;
                DataTable dtCer = new DataTable();

                try
                {
                    if (Constante.tipBD == 1)
                    {
                        dtCer = General.IncarcaDT(strGen, null);
                        if (dtCer.Rows.Count > 0)
                        {
                            idCer = Convert.ToInt32(dtCer.Rows[0]["Id"]);
                            idStare = Convert.ToInt32(dtCer.Rows[0]["IdStare"]);
                        }
                    }
                    else
                    {
                        List<string> lstOut = General.DamiOracleScalar(strGen, new object[] { "int", "int" });
                        if (lstOut.Count == 2)
                        {
                            idCer = Convert.ToInt32(lstOut[0]);
                            idStare = Convert.ToInt32(lstOut[1]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    General.ExecutaNonQuery("ROLLBACK TRAN", null);
                    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                    continue;
                }

                #region  Pontaj start
               
                //trimite in pontaj daca este finalizat
                if (idStare == 3)
                {
                    if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                    {
                        decimal nr = Convert.ToDecimal(dtDataSf.Visible == false ? txtNrOre.Value ?? 0: txtNr.Value ?? 0);
                        General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), idCer, 5, trimiteLaInlocuitor, nr);
                    }

                    strF10003 += "," + marca;
                }
                x++;
                lstMarciProcesate.Add(marca);
                #endregion


                #endregion

            }

            //Florin 2020.02.20
            if (strF10003 != "")
            {
                General.CalculFormulePeZi(   $@"ent.F10003 IN ({strF10003.Substring(1)}) AND {General.ToDataUniv(dtDataInc.Date)} <= {General.TruncateDate("Ziua")} AND {General.TruncateDate("Ziua")} <= {General.ToDataUniv(dtDataSf.Date)}");
                General.CalculFormuleCumulat($@"ent.F10003 IN ({strF10003.Substring(1)}) AND {dtDataInc.Date.Year * 100 + dtDataInc.Date.Month} <= (ent.""An"" * 100 + ent.""Luna"") AND (ent.""An"" * 100 + ent.""Luna"") <= {dtDataSf.Date.Year * 100 + dtDataSf.Date.Month}");
                General.ExecValStr($@"F10003 IN ({strF10003.Substring(1)}) AND {General.ToDataUniv(dtDataInc.Date)} <= ""Ziua"" AND ""Ziua"" <= {General.ToDataUniv(dtDataSf.Date)}");
            }

            lblProgres.Text = "Procesare terminata!";
            ProgressUpdatePanel.ContentTemplateContainer.Controls.Add(lblProgres);
            ProgressUpdatePanel.Update();

            if (err.Length > 0)
                txtLog.Text = "S-au intalnit urmatoarele erori:\n" + err;
            else
                txtLog.Text = "";

            return "S-au generat " + x + " cereri!";

        }
                 

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
         
                //IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnFiltruSterge_Click()
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
        //        cmbCtr.Value = null;
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



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                bool esteStruc = true;
                //string sql = "";
                switch (e.Parameter.Split(';')[0])
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
                    case "cmbAbs":
                        AfisareCtl(e.Parameter);
                        break;
                    case "dtDataInc":
                    case "dtDataSf":
                        if (dtDataSf.Visible == false)
                            dtDataSf.Value = dtDataInc.Value;
                        CalcZile();
                        break;
                    case "EmptyFields":
                        cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                        cmbDept.DataBind();
                        return;
                    case "cmbRol":
                        DataTable dtAng = General.IncarcaDT(SelectAngajatiRol(Convert.ToInt32(cmbRol.Value ?? -99)), null);
                        cmbAng.DataSource = dtAng;
                        Session["CereriAut_Angajati"] = dtAng;
                        Session["CereriAut_AngajatiToti"] = dtAng;
                        cmbAng.DataBind();
                        cmbAng.SelectedIndex = -1;

                        DataTable dt = null;
                        string sir = Session["CereriAut_Sir"].ToString();
                        if (sir.Length > 0)
                            dt = General.IncarcaDT("SELECT a.*, c.\"IdAuto\" as \"IdCircuit\" FROM \"Ptj_tblAbsente\" a LEFT JOIN \"Ptj_Circuit\" C ON a.\"IdGrupAbsenta\" = c.\"IdGrupAbsente\" WHERE a.\"Id\" IN (" + sir + ") AND (c.\"UserIntrod\" = - 1 * " + cmbRol.Value + " OR c.\"UserIntrod\" = " + Session["UserId"].ToString() + ")", null);
                        cmbAbs.DataSource = dt;
                        cmbAbs.DataBind();
                        Session["CereriAut_Absente"] = dt;                     
                        AfisareCtl("cmbAbs;0");
                        grDate.DataSource = null;
                        grDate.DataBind();
                        break;
                    case "btnFiltru":
                        IncarcaGrid();
                        break;
                    case "btnGen":
                        btnGen_Click(null, null);
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
                grDate.KeyFieldName = "F10003";

                DataTable dt = GetF100NumeComplet(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbSub.Value ?? -99), Convert.ToInt32(cmbFil.Value ?? -99),
                    Convert.ToInt32(cmbSec.Value ?? -99), Convert.ToInt32(cmbDept.Value ?? -99), Convert.ToInt32(cmbSubDept.Value ?? -99), Convert.ToInt32(cmbBirou.Value ?? -99), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbCtr.Value ?? -99), Convert.ToInt32(cmbCateg.Value ?? -99));
   
                grDate.DataSource = dt;
                Session["InformatiaCurenta_CereriAut"] = dt;
                grDate.DataBind();
                grDate.SettingsPager.PageSize = 25;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AfisareCtl(string param)
        {
            try
            {
                DataTable dt = Session["CereriAut_Absente"] as DataTable;
                int folosesteInterval = 0;
                int perioada = 0;
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] arr = dt.Select("Id=" + General.Nz(cmbAbs.Value, "-99"));

                    if (arr.Count() > 0)
                    {
                        DataRow dr = arr[0];
                        folosesteInterval = Convert.ToInt32(General.Nz(dr["AbsentaTipOraFolosesteInterval"], 0));
                        perioada = Convert.ToInt32(General.Nz(dr["AbsentaTipOraPerioada"], 0));
                        if (Convert.ToInt32(dr["IdTipOre"].ToString()) == 1)
                        {//tip zi
                            lblDataInc.InnerText = "Data inceput";
                            lblDataSf.Visible = true;
                            dtDataSf.Visible = true;
                            lblNr.InnerText = "Nr. zile";
                            lblNr.Visible = false;
                            rbPrel.Visible = false;
                            rbPrel1.Visible = false;
                            txtNr.ClientEnabled = false;
                            if (Session["CereriAut_NrZile"] != null)                        
                                txtNr.Text = Session["CereriAut_NrZile"].ToString();
                            txtNr.Visible = true;

                            lblNrOre.Visible = false;
                            txtNrOre.ClientVisible = false;
                            txtNrOre.Value = null;
                            txtNrOreInMinute.ClientVisible = false;
                            txtNrOreInMinute.Value = null;

                            lblOraInc.Visible = false;
                            cmbOraInc.Visible = false;
                            cmbOraInc.Value = null;

                            lblOraSf.Visible = false;
                            cmbOraSf.Visible = false;
                            cmbOraSf.Value = null;

                        }
                        else
                        {//tip ora
                            //lblNrOre.Style["display"] = "inline-block";
                            txtNrOre.ClientVisible = true;
                            txtNrOre.DecimalPlaces = 0;
                            txtNrOre.NumberType = SpinEditNumberType.Integer;

                            lblNr.InnerText = "Nr. ore";
                            lblNr.Visible = true;

                            if (folosesteInterval == 1)
                            {
                                List<Module.Dami.metaGeneral2> lst = ListaInterval(perioada);

                                //lblOraInc.Style["display"] = "inline-block";
                                lblOraInc.Visible = true;
                                cmbOraInc.Visible = true;
                                cmbOraInc.DataSource = lst;
                                cmbOraInc.DataBind();

                                //lblOraSf.Style["display"] = "inline-block";
                                lblOraSf.Visible = true;
                                cmbOraSf.Visible = true;
                                cmbOraSf.DataSource = lst;
                                cmbOraSf.DataBind();

                                txtNrOre.ClientEnabled = false;
                                txtNrOre.DecimalPlaces = 4;
                                txtNrOre.NumberType = SpinEditNumberType.Float;
                                txtNrOre.ClientVisible = false;

                                txtNrOreInMinute.ClientVisible = true;

                                //lblNrOre.InnerText = Dami.TraduCuvant("Nr. minute");
                                lblNr.InnerText = Dami.TraduCuvant("Nr. minute");
                            }


                            lblDataInc.InnerText = "Data";
                            lblDataSf.Visible = false;
                            dtDataSf.Visible = false;                     
                            txtNr.Visible = false;
                            rbPrel.Visible = true;
                            rbPrel1.Visible = true;
                            rbPrel.Checked = true;
                            DataTable dtTemp = General.IncarcaDT((Constante.tipBD == 1 ? "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'GENERARE_AUTOMATA_CERERI'" :
                                "SELECT COUNT(*) FROM user_views where view_name = 'GENERARE_AUTOMATA_CERERI'"), null);
                            if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtTemp.Rows[0][0].ToString()) > 0)
                                rbPrel1.Enabled = true;
                            else
                                rbPrel1.Enabled = false;
                            //txtNr.ClientEnabled = true;
                            Session["CereriAut_NrZile"] = null;
                        }
                    }
                    else
                    {
                        lblDataInc.InnerText = "Data inceput";
                        lblDataSf.Visible = true;
                        dtDataSf.Visible = true;
                        lblNr.InnerText = "Nr. zile";
                        lblNr.Visible = false;
                        txtNr.Visible = false;
                        rbPrel.Visible = false;
                        rbPrel1.Visible = false;

                        lblNrOre.Visible = false;
                        txtNrOre.ClientVisible = false;
                        txtNrOre.Value = null;
                        txtNrOreInMinute.ClientVisible = false;
                        txtNrOreInMinute.Value = null;

                        lblOraInc.Visible = false;
                        cmbOraInc.Visible = false;
                        cmbOraInc.Value = null;

                        lblOraSf.Visible = false;
                        cmbOraSf.Visible = false;
                        cmbOraSf.Value = null;
                    }
                }
                else
                {
                    lblDataInc.InnerText = "Data inceput";
                    lblDataSf.Visible = true;
                    dtDataSf.Visible = true;
                    lblNr.InnerText = "Nr. zile";
                    lblNr.Visible = false;
                    txtNr.Visible = false;
                    rbPrel.Visible = false;
                    rbPrel1.Visible = false;

                    lblNrOre.Visible = false;
                    txtNrOre.ClientVisible = false;
                    txtNrOre.Value = null;
                    txtNrOreInMinute.ClientVisible = false;
                    txtNrOreInMinute.Value = null;

                    lblOraInc.Visible = false;
                    cmbOraInc.Visible = false;
                    cmbOraInc.Value = null;

                    lblOraSf.Visible = false;
                    cmbOraSf.Visible = false;
                    cmbOraSf.Value = null;
                }

                AfiseazaCtlExtra();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private List<Module.Dami.metaGeneral2> ListaInterval(int perioada)
        {
            try
            {
                List<Module.Dami.metaGeneral2> list = new List<Module.Dami.metaGeneral2>();

                DateTime ziua = new DateTime(2200, 1, 1, 0, 0, 0);
                DateTime ziuaPlus = ziua.AddDays(1);

                do
                {
                    ziua = ziua.AddMinutes(perioada);
                    string str = ziua.Hour.ToString().PadLeft(2, '0') + ":" + ziua.Minute.ToString().PadLeft(2, '0');
                    list.Add(new Module.Dami.metaGeneral2() { Id = str, Denumire = str });
                }
                while (ziua < ziuaPlus);

                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                return null;
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
                Session["CereriAut_Angajati"] = dt;
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

        //Florin 2020-04-30
        public string CreazaSelectCuValori(int marca, int tip = 1, decimal nrOre = -99)
        {
            //tip = 1 intoarce un select
            //tip = 2 intoarce ca values; necesar pt Oracle

            string sqlCer = "";

            try
            {
                string idCircuit = "-99";
                DataTable dtAbs = Session["CereriAut_Absente"] as DataTable;
                DataRow[] lst = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                if (lst.Count() != 0)
                {
                    idCircuit = (lst[0]["IdCircuit"] == null ? "NULL" : lst[0]["IdCircuit"].ToString());
                }

                string strTop = "";
                if (Constante.tipBD == 1) strTop = "TOP 1";

                string nrZile = "1";
                if (dtDataSf.Visible != false)
                    nrZile = txtNr.Text;
                else
                {
                    dtDataSf.Value = dtDataInc.Value;
                    if (nrOre == -99)
                        nrOre = Convert.ToDecimal(txtNrOre.Text);
                }

                string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                string sqlInloc =  "NULL" ;
                string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                string sqlIdStare = $@"(SELECT {strTop} ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlPozitie = $@"(SELECT {strTop} ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlCuloare = $@"(SELECT {strTop} ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlNrOre = nrOre == -99 ? "NULL" : nrOre.ToString(new CultureInfo("en-US"));

                if (Constante.tipBD == 2)
                {
                    sqlIdStare = $@"(SELECT * FROM ({sqlIdStare}) WHERE ROWNUM=1)";
                    sqlPozitie = $@"(SELECT * FROM ({sqlPozitie}) WHERE ROWNUM=1)";
                    sqlCuloare = $@"(SELECT * FROM ({sqlCuloare}) WHERE ROWNUM=1)";
                }

                string sqlOraInc = "NULL";
                string sqlOraSf = "NULL";

                if (General.Nz(cmbOraInc.Value, "").ToString() != "")
                    sqlOraInc = "'" + dtDataInc.Date.Year + "-" + dtDataInc.Date.Month + "-" + dtDataInc.Date.Day + " " + General.Nz(cmbOraInc.Value, "").ToString() + ":00'";

                if (General.Nz(cmbOraSf.Value, "").ToString() != "")
                    sqlOraSf = "'" + dtDataSf.Date.Year + "-" + dtDataSf.Date.Month + "-" + dtDataSf.Date.Day + " " + General.Nz(cmbOraSf.Value, "").ToString() + ":00'";

                if (Constante.tipBD == 2)
                {
                    sqlIdStare = $@"(SELECT * FROM ({sqlIdStare}) WHERE ROWNUM=1)";
                    sqlPozitie = $@"(SELECT * FROM ({sqlPozitie}) WHERE ROWNUM=1)";
                    sqlCuloare = $@"(SELECT * FROM ({sqlCuloare}) WHERE ROWNUM=1)";

             
                    if (General.Nz(cmbOraInc.Value, "").ToString() != "")
                        sqlOraInc = "TO_DATE('" + dtDataInc.Date.Day + "-" + dtDataInc.Date.Month + "-" + dtDataInc.Date.Year + " " + General.Nz(cmbOraInc.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";

                    if (General.Nz(cmbOraSf.Value, "").ToString() != "")
                        sqlOraSf = "TO_DATE('" + dtDataSf.Date.Day + "-" + dtDataSf.Date.Month + "-" + dtDataSf.Date.Year + " " + General.Nz(cmbOraSf.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";
                }

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("id", sqlIdCerere);
                dic.Add("f10003", marca.ToString());
                dic.Add("idabsenta", cmbAbs.Value.ToString());
                dic.Add("datainceput", General.ToDataUniv(dtDataInc.Date));
                dic.Add("datasfarsit", General.ToDataUniv(dtDataSf.Date));
                dic.Add("nrzile", (nrZile == null ? "NULL" : nrZile));
                dic.Add("nrzileviitor", "NULL");
                dic.Add("observatii", (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString() + "'"));
                dic.Add("idstare", (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()));
                dic.Add("idcircuit", idCircuit);
                dic.Add("userintrod", General.Nz(Session["UserId"], "-99").ToString());
                dic.Add("culoare", (sqlCuloare == null ? "NULL" : sqlCuloare));
                dic.Add("inlocuitor", (sqlInloc == null ? "NULL" : sqlInloc));
                dic.Add("totalsupercircuit", (sqlTotal == null ? "NULL" : sqlTotal));
                dic.Add("pozitie", (sqlPozitie == null ? "NULL" : sqlPozitie));
                dic.Add("trimitela", "NULL");
                dic.Add("nrore", (sqlNrOre == null ? "NULL" : sqlNrOre));
                dic.Add("orainceput", sqlOraInc);
                dic.Add("orasfarsit", sqlOraSf);
                dic.Add("areatas", "0");
                dic.Add("user_no", General.Nz(Session["UserId"], "-99").ToString());
                dic.Add("time", General.CurrentDate());

                #region Campuri Extra

                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                DataTable dtEx = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1", new object[] { cmbAbs.Value });
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    DataRow dr = dtEx.Rows[i];
                    ASPxEdit ctl = divDateExtra.FindControl("ctlDinamic" + General.Nz(dr["IdCampExtra"], 0)) as ASPxEdit;
                    if (General.Nz(dr["IdCampExtra"], "").ToString() != "")
                    {
                        if (ctl != null && ctl.Value != null && ctl.Value.ToString() != "")
                        {
                            switch (General.Nz(dr["TipCamp"], "").ToString())
                            {
                                case "0":
                                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString() + "'";
                                    break;
                                case "1":
                                    if (Convert.ToBoolean(ctl.Value) == true)
                                        lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'Da'";
                                    else
                                        lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'Nu'";
                                    break;
                                case "3":
                                    DateTime zi = Convert.ToDateTime(ctl.Value);
                                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "'";
                                    break;
                                default:
                                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString() + "'";
                                    break;
                            }
                        }
                        else
                        {
                            if (General.Nz(dr["Sursa"], "").ToString() != "")
                                lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "(" + General.Nz(dr["Sursa"], "").ToString() + ")";
                        }
                    }
                }

                string valExtra = "";
                for (int i = 0; i < lstExtra.Count(); i++)
                {
                    string val = lstExtra[i];
                    if (val.ToLower().IndexOf("ent.")>=0)
                    {
                        foreach (KeyValuePair<string, string> l in dic)
                        {
                            val = val.ToUpper().Replace("ENT." + l.Key.ToUpper(), l.Value);
                        }
                    }
                    if (tip == 1)
                        valExtra += "," + val + "  AS \"CampExtra" + (i + 1).ToString() + "\" ";
                    else
                        valExtra += "," + val;
                }

                #endregion

                string strSelect = "";
                foreach(KeyValuePair<string,string> l in dic)
                {
                    if (tip == 1)
                        strSelect += "," + l.Value + " AS \"" + l.Key + "\"";
                    else
                        strSelect += "," + l.Value;
                }

                if (tip == 1)
                    sqlCer = "SELECT " + strSelect.Substring(1) + valExtra;
                else
                    sqlCer = "VALUES(" + strSelect.Substring(1) + valExtra + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlCer;
        }

        protected void CalcZile()
        {
            if (dtDataInc.Value == null || dtDataSf.Value == null)
                return;

            DateTime dtStart = Convert.ToDateTime(dtDataInc.Value);
            DateTime dtSfarsit = Convert.ToDateTime(dtDataSf.Value);

            string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dtStart.Year + " UNION SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dtSfarsit.Year ;
            if (Constante.tipBD == 2)
                strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dtStart.Year + " UNION SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY)  = " + dtSfarsit.Year ;
            DataTable dtHolidays = General.IncarcaDT(strSql, null);

            int i = 0;
            for (DateTime zi = dtStart; zi <= dtSfarsit; zi = zi.AddDays(1))
            {
                bool ziLibera = EsteZiLibera(zi, dtHolidays);
                if (zi.DayOfWeek.ToString().ToLower() != "saturday" && zi.DayOfWeek.ToString().ToLower() != "sunday" && !ziLibera)
                    i++;
            }
            txtNr.Text = i.ToString();
            Session["CereriAut_NrZile"] = i;

        }

        private bool EsteZiLibera(DateTime data, DataTable dtHolidays)
        {
            bool ziLibera = false;
            for (int z = 0; z < dtHolidays.Rows.Count; z++)
                if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == data.Date)
                {
                    ziLibera = true;
                    break;
                }
            return ziLibera;
        }


        private string SelectAngajatiRol(int idRol = -44)
        {
            // dreptul de aprobare si ce angajati se incarca sunt preluati din baza de date pe baza parametrului DreptSolicitareAbsenta
            // 0 - se poate face aprobarea pentru toti angajatii asignati supervizorului de pe prima coloana de pe circuit (User Introducere)  
            // 1 - se poate face aprobarea pentru toti angajatii asignati oricarui supervizor de pe circuit

            string strSql = "";

            try
            {
                string cmp = "";
                string inn1 = "";
                string inn2 = "";
                string op = "+";
                if (Constante.tipBD == 2) op = "||";
                if (Dami.ValoareParam("DreptSolicitareAbsenta") == "1")
                {
                    inn1 = @" OR B.""IdSuper"" = -1 * c.""Super1"" OR B.""IdSuper"" = -1 * c.""Super2"" OR B.""IdSuper"" = -1 * c.""Super3"" OR B.""IdSuper"" = -1 * c.""Super4"" OR B.""IdSuper"" = -1 * c.""Super5"" OR B.""IdSuper"" = -1 * c.""Super6""  OR B.""IdSuper"" = -1 * c.""Super7"" OR B.""IdSuper"" = -1 * c.""Super8"" OR B.""IdSuper"" = -1 * c.""Super9"" OR B.""IdSuper"" = -1 * c.""Super10"" OR B.""IdSuper"" = -1 * c.""Super11"" OR B.""IdSuper"" = -1 * c.""Super12"" OR B.""IdSuper"" = -1 * c.""Super13"" OR B.""IdSuper"" = -1 * c.""Super14"" OR B.""IdSuper"" = -1 * c.""Super15"" OR B.""IdSuper"" = -1 * c.""Super16"" OR B.""IdSuper"" = -1 * c.""Super17"" OR B.""IdSuper"" = -1 * c.""Super18"" OR B.""IdSuper"" = -1 * c.""Super19"" OR B.""IdSuper"" = -1 * c.""Super20"" ";
                    inn2 = $@" OR c.""Super1"" = {General.VarSession("UserId")}  OR c.""Super2"" = {General.VarSession("UserId")}  OR c.""Super3"" = {General.VarSession("UserId")}  OR c.""Super4"" = {General.VarSession("UserId")}  OR c.""Super5"" = {General.VarSession("UserId")}  OR c.""Super6"" = {General.VarSession("UserId")}  OR c.""Super7"" = {General.VarSession("UserId")}  OR c.""Super8"" = {General.VarSession("UserId")}  OR c.""Super9"" = {General.VarSession("UserId")}  OR c.""Super10"" = {General.VarSession("UserId")} OR c.""Super11"" = {General.VarSession("UserId")} OR c.""Super12"" = {General.VarSession("UserId")}  OR c.""Super13"" = {General.VarSession("UserId")}  OR c.""Super14"" = {General.VarSession("UserId")}  OR c.""Super15"" = {General.VarSession("UserId")}  OR c.""Super16"" = {General.VarSession("UserId")}  OR c.""Super17"" = {General.VarSession("UserId")}  OR c.""Super18"" = {General.VarSession("UserId")}  OR c.""Super19"" = {General.VarSession("UserId")}  OR c.""Super20"" = {General.VarSession("UserId")} ";
                }

                strSql = $@"SELECT B.""Rol"", B.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", A.F10017 AS CNP, A.F10022 AS ""DataAngajarii"", A.F10023 AS ""DataPlecarii"",
                        A.F10011 AS ""NrContract"", CAST(A.F10043 AS int) AS ""Norma"",A.F100901, CASE WHEN (A.F10025 = 0 OR A.F10025=999) THEN 1 ELSE 0 END AS ""AngajatActiv"",
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"", B.""RolDenumire""  {cmp}
                        FROM (
                        SELECT A.F10003, 0 AS ""Rol"", COALESCE((SELECT COALESCE(""Alias"", ""Denumire"") FROM ""tblSupervizori"" WHERE ""Id""=0),'Angajat') AS ""RolDenumire""
                        FROM F100 A
                        WHERE A.F10003 = {General.VarSession("User_Marca")}
                        UNION
                        SELECT A.F10003, B.""IdSuper"" AS ""Rol"", CASE WHEN D.""Alias"" IS NOT NULL AND D.""Alias"" <> '' THEN D.""Alias"" ELSE D.""Denumire"" END AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        INNER JOIN ""Ptj_Circuit"" C ON B.""IdSuper"" = -1 * c.""UserIntrod"" {inn1}
                        LEFT JOIN ""tblSupervizori"" D ON D.""Id"" = B.""IdSuper""
                        WHERE B.""IdUser""= {General.VarSession("UserId")}
                        UNION
                        SELECT A.F10003, 76 AS ""Rol"", '{Dami.TraduCuvant("Fara rol")}' AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""Ptj_Circuit"" C ON C.""UserIntrod""={General.VarSession("UserId")} {inn2}) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        WHERE 1=1 ";

                if (idRol != -44) strSql += @" AND ""Rol""=" + idRol;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void UpdatePanel_Unload(object sender, EventArgs e)
        {
            RegisterUpdatePanel((UpdatePanel)sender);
        }
        protected void RegisterUpdatePanel(UpdatePanel panel)
        {
            var sType = typeof(ScriptManager);
            var mInfo = sType.GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
            if (mInfo != null)
                mInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
        }

        private void AfiseazaCtlExtra()
        {
            try
            {
                divDateExtra.Controls.Clear();

                string ids = "";

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1 AND COALESCE(""ReadOnly"",0)=0", new object[] { General.Nz(cmbAbs.Value, "-99") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    HtmlGenericControl ctlDiv = new HtmlGenericControl("div");
                    ctlDiv.Style.Add("float", "left");
                    ctlDiv.Style.Add("padding-right", "15px");
                    ctlDiv.Style.Add("padding-bottom", "10px");

                    Label lbl = new Label();
                    lbl.Text = Dami.TraduCuvant(dr["Denumire"].ToString());
                    lbl.Style.Add("display", "inline-block");
                    lbl.Style.Add("float", "left");
                    lbl.Style.Add("padding-right", "15px");
                    lbl.Style.Add("width", "80px");
                    ctlDiv.Controls.Add(lbl);

                    //Florin 2020.04.30
                    //string ctlId = "ctlDinamic" + i;
                    string ctlId = "ctlDinamic" + General.Nz(dr["IdCampExtra"],0);
                    switch (General.Nz(dr["TipCamp"], "").ToString())
                    {
                        case "0":                   //text
                            ASPxTextBox txt = new ASPxTextBox();
                            txt.ID = ctlId;
                            txt.ClientIDMode = ClientIDMode.Static;
                            txt.ClientInstanceName = "ctlDinamic" + i;
                            txt.Width = Unit.Pixel(70);
                            txt.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                            if (General.Nz(dr["Sursa"], "").ToString() != "")
                            {
                                string sel = InlocuiesteCampuri(dr["Sursa"].ToString());
                                if (sel != "")
                                {
                                    object val = General.Nz(General.ExecutaScalar(sel, null), "");
                                    txt.Value = val.ToString();
                                }
                            }
                            ctlDiv.Controls.Add(txt);
                            break;
                        case "1":                   //checkBox
                            ASPxCheckBox chk = new ASPxCheckBox();
                            chk.ID = ctlId;
                            chk.ClientIDMode = ClientIDMode.Static;
                            chk.ClientInstanceName = "ctlDinamic" + i;
                            chk.Checked = false;
                            chk.AllowGrayed = false;
                            chk.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                            if (General.Nz(dr["Sursa"], "").ToString() != "")
                            {
                                string sel = InlocuiesteCampuri(dr["Sursa"].ToString());
                                if (sel != "")
                                {
                                    object val = General.Nz(General.ExecutaScalar(sel, null), "");
                                    chk.Value = val.ToString();
                                }
                            }
                            ctlDiv.Controls.Add(chk);
                            break;
                        case "2":                   //combobox
                            ASPxComboBox cmb = new ASPxComboBox();
                            cmb.ID = ctlId;
                            cmb.ClientIDMode = ClientIDMode.Static;
                            cmb.ClientInstanceName = "ctlDinamic" + i;
                            cmb.Width = Unit.Pixel(150);
                            cmb.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                            try
                            {
                                if (General.Nz(dr["Sursa"], "").ToString() != "")
                                {
                                    string sel = InlocuiesteCampuri(dr["Sursa"].ToString());
                                    if (sel != "")
                                    {
                                        DataTable dtCmb = General.IncarcaDT(sel, null);
                                        cmb.ValueField = dtCmb.Columns[0].ColumnName;
                                        cmb.TextField = dtCmb.Columns[1].ColumnName;
                                        cmb.DataSource = dtCmb;
                                        cmb.DataBind();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), "Incarcare combobox camp extra");
                            }
                            ctlDiv.Controls.Add(cmb);
                            break;
                        case "3":                   //dateTime
                            ASPxDateEdit dte = new ASPxDateEdit();
                            dte.ID = ctlId;
                            dte.ClientIDMode = ClientIDMode.Static;
                            dte.ClientInstanceName = "ctlDinamic" + i;
                            dte.Width = Unit.Pixel(100);
                            dte.DisplayFormatString = "dd/MM/yyyy";
                            dte.EditFormat = EditFormat.Custom;
                            dte.EditFormatString = "dd/MM/yyyy";
                            dte.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                            if (General.Nz(dr["Sursa"], "").ToString() != "")
                            {
                                string sel = InlocuiesteCampuri(dr["Sursa"].ToString());
                                if (sel != "")
                                {
                                    object val = General.Nz(General.ExecutaScalar(sel, null), "");
                                    dte.Value = val.ToString();
                                }
                            }
                            ctlDiv.Controls.Add(dte);
                            break;
                    }

                    divDateExtra.Controls.Add(ctlDiv);

                    ids += ctlId + ";";
                }

                Session["Absente_Cereri_Date_Aditionale"] = ids;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string InlocuiesteCampuri(string strSelect)
        {
            string str = strSelect;

            try
            {
                //string dual = "";
                //if (Constante.tipBD == 2) dual = " FROM DUAL";
                //DataTable dt = General.IncarcaDT(CreazaSelectCuValori() + dual, null);
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    for (int i = 0; i < dt.Columns.Count; i++)
                //    {
                //        var ert = dt.Columns[i];
                //        var rfv = dt.Rows[0][i];

                //        if (dt.Rows[0][i].GetType() == typeof(DateTime))
                //            str = str.Replace("ent." + dt.Columns[i], (dt.Rows[0][i] == null ? "null" : General.ToDataUniv((DateTime)dt.Rows[0][i])).ToString());
                //        else
                //            str = str.Replace("ent." + dt.Columns[i], General.Nz(dt.Rows[0][i], "null").ToString());
                //    }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }


    }
}
 
 
 
 
 
 
 
 
 
 
 
 