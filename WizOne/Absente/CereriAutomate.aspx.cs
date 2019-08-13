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

namespace WizOne.Absente
{
    public partial class CereriAutomate : System.Web.UI.Page
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
                    

                    IncarcaAngajati();
                    DataTable dt = null;
                    if (sir.Length > 0)
                        dt = General.IncarcaDT("SELECT a.*, c.\"IdAuto\" as \"IdCircuit\" FROM \"Ptj_tblAbsente\" a LEFT JOIN \"Ptj_Circuit\" C ON a.\"IdGrupAbsenta\" = c.\"IdGrupAbsente\" WHERE a.\"Id\" IN (" + sir + ")", null);
                    
                    cmbAbs.DataSource = dt;
                    cmbAbs.DataBind();
                    Session["CereriAut_Absente"] = dt;
                    AfisareCtl("cmbAbs;0");
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


                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    lstMarci.Add(Convert.ToInt32(General.Nz(lst[i], -99)));
                }

                grDate.Selection.UnselectAll();


                if (lstMarci.Count > 0)
                {
                    string msg = "";

                    msg = GenerareCereri(lstMarci);

                    MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoSuccess);

                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string GenerareCereri(List<int> lstMarci)
        {
            string msg = "";
            string err = "";
            if (rbPrel1.Checked)
            {
                lstMarci.Clear();
                DateTime data = Convert.ToDateTime(dtDataInc.Value);
                int dept = Convert.ToInt32(cmbDept.Value);
                int idAbsenta = Convert.ToInt32(cmbAbs.Value);
                string dt = " CONVERT(DATETIME, '" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 103) ";
                if (Constante.tipBD == 2) dt = " TO_DATE('" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 'dd/MM/yyyy') ";

                string sql = "SELECT * FROM GENERARE_AUTOMATA_CERERI a WHERE a.F10007 =  " + dept + " AND a.\"IdAbsenta\" =  " + idAbsenta + " AND \"Data\" = " + dt + " AND "
                           + " F10003 IN (select b.F10003 from \"F100Supervizori\" b where b.\"IdSuper\" = (select (-1) * \"UserIntrod\" from \"Ptj_Circuit\" where \"IdAuto\" = a.\"IdCircuit\") and b.\"IdUser\" = " + Session["UserId"].ToString() + ") ";

                DataTable dtPrel = General.IncarcaDT(sql, null);
                for (int i = 0; i < dtPrel.Rows.Count; i++)
                    lstMarci.Add(Convert.ToInt32(dtPrel.Rows[i]["F10003"].ToString()));
            }
            int x = 0;
            foreach (int marca in lstMarci)
            {
               
                #region Salvare in baza

                #region Construim istoricul
                DataTable dtAbs = General.IncarcaDT(General.SelectAbsente(marca.ToString()), null);
                DataRow[] lst = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                if (lst.Count() == 0)
                {
                    if (dtAbs.Rows.Count <= 0)
                    {
                        err += "Angajatul cu marca " + marca + " nu are definit contract!\n";
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

                int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={marca} AND F10022 <= {General.ToDataUniv(Convert.ToDateTime(dtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime(dtDataSf.Value))} <= F10023", null), 0));
                if (esteActiv == 0)
                {
                    err += "In perioada solicitata, angajatul cu marca " + marca + " este inactiv\n";         
                    continue;
                }

                if (Convert.ToInt32(General.Nz(marca, -98)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -97)) && Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { marca })) == 0)
                {
                    err += "Angajatul cu marca " + marca + " nu are nici un supervizor\n";             
                    continue;
                }

                int intersec = Convert.ToInt32(General.ExecutaScalar($@"
                                SELECT COUNT(*) 
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.F10003 = {marca} AND A.""DataInceput"" <= {General.ToDataUniv(dtDataSf.Date)} AND {General.ToDataUniv(dtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND A.""IdStare"" IN (1,2,3,4) AND B.""GrupOre"" IN({General.Nz(drAbs["GrupOreDeVerificat"], -99)})", null));

                if (intersec > 0)
                {
                    err += "Intervalul pentru angajatul cu marca " + marca + " se intersecteaza cu altul deja existent\n";            
                    continue;
                }


                General.SelectCereriIstoric(marca, -1, Convert.ToInt32(drAbs["IdCircuit"]), 0, out sqlIst, out trimiteLaInlocuitor);

                #endregion

                string sqlCer = "";
                string sqlPre = "";
                string strGen = "";

                if (Constante.tipBD == 1)
                {
                    sqlCer = CreazaSelectCuValori(marca);

                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") 
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
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") ";

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
                        General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), idCer, 5, trimiteLaInlocuitor, Convert.ToInt32(txtNr.Value ?? 0));

                        //Se va face cand vom migra GAM
                        //TrimiteCerereInF300(Session["UserId"], idCer);
                    }
                }
                x++;
                #endregion
                

                #endregion

            }

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
         
                IncarcaGrid();
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
                string sql = "";
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
            DataTable dt = Session["CereriAut_Absente"] as DataTable;
            if (dt != null)
            {
                DataRow[] dr = dt.Select("Id = " + param.Split(';')[1]);
                if (dr != null && dr.Count() > 0)
                {
                    if (Convert.ToInt32(dr[0]["IdTipOre"].ToString()) == 1)
                    {//tip zi
                        lblDataInc.InnerText = "Data inceput";
                        lblDataSf.Visible = true;
                        dtDataSf.Visible = true;
                        lblNr.InnerText = "Nr. zile";
                        rbPrel.Visible = false;
                        rbPrel1.Visible = false;
                        txtNr.ClientEnabled = false;
                        if (Session["CereriAut_NrZile"] != null)                        
                            txtNr.Text = Session["CereriAut_NrZile"].ToString();                   
                        
                    }
                    else
                    {//tip ora
                        lblDataInc.InnerText = "Data";
                        lblDataSf.Visible = false;
                        dtDataSf.Visible = false;
                        lblNr.InnerText = "Nr. ore";
                        rbPrel.Visible = true;
                        rbPrel1.Visible = true;
                        rbPrel.Checked = true;
                        DataTable dtTemp = General.IncarcaDT((Constante.tipBD == 1 ? "SELECT COUNT(*) FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'GENERARE_AUTOMATA_CERERI'" :
                            "SELECT COUNT(*) FROM user_views where view_name = 'GENERARE_AUTOMATA_CERERI'"), null);
                        if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtTemp.Rows[0][0].ToString()) > 0)
                            rbPrel1.Enabled = true;
                        else
                            rbPrel1.Enabled = false;
                        txtNr.ClientEnabled = true;
                        Session["CereriAut_NrZile"] = null;
                    }
                }
                else
                {
                    lblDataInc.InnerText = "Data inceput";
                    lblDataSf.Visible = true;
                    dtDataSf.Visible = true;
                    lblNr.InnerText = "Nr. zile";
                    rbPrel.Visible = false;
                    rbPrel1.Visible = false;
                }
            }
            else
            {
                lblDataInc.InnerText = "Data inceput";
                lblDataSf.Visible = true;
                dtDataSf.Visible = true;
                lblNr.InnerText = "Nr. zile";
                rbPrel.Visible = false;
                rbPrel1.Visible = false;
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

        public string CreazaSelectCuValori(int marca, int tip = 1)
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

                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };
                

                string valExtra = "";
                for (int i = 0; i < lstExtra.Count(); i++)
                {
                    if (tip == 1)
                        valExtra += "," + lstExtra[i] + "  AS \"CampExtra" + (i + 1).ToString() + "\" ";
                    else
                        valExtra += "," + lstExtra[i];
                }

                //string dual = "";
                string strTop = "";
                if (Constante.tipBD == 1) strTop = "TOP 1";

                string nrZile = "1";
                if (dtDataSf.Visible != false)
                    nrZile = txtNr.Text;
                else
                    dtDataSf.Value = dtDataInc.Value;

                string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                string sqlInloc =  "NULL" ;
                string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                string sqlIdStare = $@"(SELECT {strTop} ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlPozitie = $@"(SELECT {strTop} ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlCuloare = $@"(SELECT {strTop} ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlNrOre = nrZile == "" ? "NULL" : nrZile;

                if (Constante.tipBD == 2)
                {
                    sqlIdStare = $@"(SELECT * FROM ({sqlIdStare}) WHERE ROWNUM=1)";
                    sqlPozitie = $@"(SELECT * FROM ({sqlPozitie}) WHERE ROWNUM=1)";
                    sqlCuloare = $@"(SELECT * FROM ({sqlCuloare}) WHERE ROWNUM=1)";
                    //dual = " FROM DUAL";
                }

                sqlCer = @"SELECT " +
                                sqlIdCerere + " AS \"Id\", " +
                                marca + " AS \"F10003\", " +
                                cmbAbs.Value + " AS \"IdAbsenta\", " +
                                General.ToDataUniv(dtDataInc.Date) + " AS \"DataInceput\", " +
                                General.ToDataUniv(dtDataSf.Date) + " AS \"DataSfarsit\", " +
                                (nrZile == null ? "NULL" : nrZile) + " AS \"NrZile\", " +
                                 "NULL"  + " AS \"NrZileViitor\", " +
                                (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString() + "'") + " AS \"Observatii\", " +
                                (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + " AS \"IdStare\", " +
                                (idCircuit) + " AS \"IdCircuit\", " +
                                Session["UserId"] + " AS \"UserIntrod\", " +
                                (sqlCuloare == null ? "NULL" : sqlCuloare) + " AS \"Culoare\", " +
                                (sqlInloc == null ? "NULL" : sqlInloc) + " AS \"Inlocuitor\", " +
                                (sqlTotal == null ? "NULL" : sqlTotal) + " AS \"TotalSuperCircuit\", " +
                                (sqlPozitie == null ? "NULL" : sqlPozitie) + " AS \"Pozitie\", " +
                                //trimiteLaInlocuitor + " AS \"TrimiteLa\", " +
                                " NULL AS \"TrimiteLa\", " +
                                (sqlNrOre == null ? "NULL" : sqlNrOre) + " AS \"NrOre\", " +
                                " 0 AS \"AreAtas\"" +
                                valExtra;
                if (tip == 2)
                    sqlCer = @"VALUES(" +
                    sqlIdCerere + ", " +
                    marca + ", " +
                    cmbAbs.Value + ", " +
                    General.ToDataUniv(dtDataInc.Date) + ", " +
                    General.ToDataUniv(dtDataSf.Date) + ", " +
                    (nrZile == null ? "NULL" : nrZile) + ", " +
                    "NULL" + ", " +
                    (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString() + "'") + ", " +
                    (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + ", " +
                    (idCircuit) + ", " +
                    Session["UserId"] + ", " +
                    (sqlCuloare == null ? "NULL" : sqlCuloare) + ", " +
                    (sqlInloc == null ? "NULL" : sqlInloc) + ", " +
                    (sqlTotal == null ? "NULL" : sqlTotal) + ", " +
                    (sqlPozitie == null ? "NULL" : sqlPozitie) + ", " +
                    " NULL, " +
                    (sqlNrOre == null ? "NULL" : sqlNrOre) + ", " +
                     " 0 " +
                    valExtra + ")";
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

    }
}
 
 
 
 
 
 
 
 
 
 
 
 