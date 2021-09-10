using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Globalization;
using System.Web.Hosting;
using DevExpress.Skins;
using DevExpress.LookAndFeel;

namespace WizOne.Tactil
{
    public partial class CereriTactil : System.Web.UI.Page
    {

        public class metaCereriDate
        {
            public object Angajat { get; set; }
            public object Absenta { get; set; }
            public object DataInceput { get; set; }
            public object DataSfarsit { get; set; }
            public object Inlocuitor { get; set; }
            public object NrZile { get; set; }
            public object NrOre { get; set; }
            public object OreCalc { get; set; }
            public object OreSursa { get; set; }
            public object Data1 { get; set; }
            public object Data2 { get; set; }
            public object Bifa1 { get; set; }
            public object BIfa2 { get; set; }
            public object CmpText { get; set; }
            public object NrZileViitor { get; set; }
            public object Obs { get; set; }
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Constante.esteTactil)
                    Dami.AccesTactil();
                else
                    Dami.AccesApp(this.Page);


                lnkSave.Attributes.Add("onClick", "return false;");

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                #endregion

                
                lblRol.Visible = false;
                cmbRol.Visible = false;
                cmbRol.Value = 0;

                lblTip.Visible = false;
                cmbAbs.Visible = false;

                string denumire = "";
                if (!IsPostBack)
                {                    
                    switch (Session["CereriTactil"].ToString())
                    {
                        case "AbsenteOra":
                        case "BiletVoie":
                            if (Session["CereriTactil"].ToString() == "BiletVoie")
                                denumire = "BV%";
                            else
                                denumire = "%%";
                            //lblZile.InnerText = "Nr. ore";
                            lblNrOre.Visible = true;
                            tdNrOre.Visible = true;
                            txtNrOre.ClientVisible = true;
                            txtNrOre.MinValue = 1;
                            txtNrOre.MaxValue = 8;
                            tdNrOre.Align = "left";    

                            txtNrZile.ClientVisible = false;
                            lblZile.Visible = false;
                            tdNrZile.Visible = false;

                            lblDataSf.Visible = false;
                            txtDataSf.ClientVisible = false;

                            //tdNrZile.Align = "left";
                            tdDataSf.Visible = false;
                            tdNrOre.Width = "550";
                            if (Session["CereriTactil"].ToString() == "BiletVoie")
                            {
                                rbMotiv1.ClientVisible = true;
                                rbMotiv2.ClientVisible = true;
                            }

                            lblZileRamase.Visible = false;
                            txtNrZileRamase.ClientVisible = false;
                            tdNrZileRamase.Visible = false;
                            break;
                        case "PlanificareCO":
                            denumire = "COP";
                            tdDataSf.Width = "1200";
                            break;
                        case "CerereCO":
                            denumire = "CO";
                            tdDataSf.Width = "1200";
                            break;
                    }

                    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23 || Session["CereriTactil"].ToString() == "AbsenteOra")
                    {
                        tdSelAbs.Visible = true;
                        cmbSelAbs.Visible = true;
                    }
                    else
                    {
                        tdSelAbs.Visible = false;
                        cmbSelAbs.Visible = false;
                    }
                }

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

                DataTable dtAbs = General.IncarcaDT("SELECT * FROM \"Ptj_tblAbsente\" WHERE \"DenumireScurta\" LIKE '" + denumire + "'", null);               


                cmbAbs.DataSource = dtAbs;
                if (dtAbs != null && dtAbs.Rows.Count > 0 && !IsPostBack)
                    cmbAbs.Value = Convert.ToInt32(dtAbs.Rows[0]["Id"].ToString());
             
                DataTable dtZile = General.IncarcaDT("SELECT * FROM \"SituatieZileAbsente\" WHERE F10003 = " + General.Nz(General.VarSession("User_Marca"), -99).ToString() + " AND \"An\" = " + DateTime.Now.Year, null);

                if (dtZile != null && dtZile.Rows.Count > 0)
                    txtNrZileRamase.Text = dtZile.Rows[0]["Ramase"].ToString();

                if (!IsPostBack)
                {
                    //txtTitlu.Text = General.VarSession("Titlu").ToString();
                    Session["TactilNrZile"] = null;
                    txtDataInc.Date = DateTime.Now;
                    txtDataSf.Date = DateTime.Now;
                    rbMotiv1.Checked = true;

                    //DataTable dt = General.IncarcaDT($@"SELECT ""Rol"" AS ""Id"", ""RolDenumire"" AS ""Denumire"" FROM ({Dami.SelectCereri()}) X WHERE ""Rol"" >= 0 GROUP BY ""Rol"", ""RolDenumire""  ", null);



                    //Incarcam Angajatii in functie de rol

                    //if ((dtRol.Rows.Count == 0 || dtRol.Rows.Count == 1) && General.VarSession("User_Marca").ToString() != "-99") cmbAng.Value = General.VarSession("User_Marca");



                    if (General.VarSession("IstoricExtins_VineDin").ToString() == "2-OK" && Session["Absente_Cereri_Date"] != null)
                    {
                        metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                        txtDataInc.Value = itm.DataInceput;
                        txtDataSf.Value = itm.DataSfarsit;
                    }


                    dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(General.VarSession("User_Marca"), -99).ToString(), Convert.ToDateTime(txtDataInc.Value ?? DateTime.Now.Date)), null);




                    //Incarcam Inlocuitorii


                    //Populam campurile
                    if (General.VarSession("IstoricExtins_VineDin").ToString() == "2-OK" && Session["Absente_Cereri_Date"] != null)
                    {
                        metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                        txtNrZile.Value = itm.NrZile;
                        //txtNrOre.Value = itm.NrOre;

                        //txtNrZileViitor.Value = itm.NrZileViitor;

                        //lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();



                        //este necesar pt cand se intoarce din IstoricExtins
                        AfiseazaCtl();

                    }
                    //AfiseazaCtl();

                    DataRow[] dtRowAbs = null;
                    if (Session["CereriTactil"].ToString() == "AbsenteOra")
                        dtRowAbs = dtAbs.Select("IdTipOre = 0", "Id");
                    else
                        dtRowAbs = dtAbs.Select("IdTipOre = 1", "Id");
                    DataTable dtAbsSpn = new DataTable();
                    if (dtRowAbs != null && dtRowAbs.Count() > 0)
                    {
                        dtAbsSpn = dtRowAbs.CopyToDataTable();

                        if (dtAbsSpn != null && dtAbsSpn.Rows.Count > 0 && Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                        {
                            cmbSelAbs.DataSource = dtAbsSpn;
                            cmbSelAbs.DataBind();
                            cmbSelAbs.SelectedIndex = 0;
                            Session["Absente_Tactil"] = dtAbsSpn;

                            DataRow[] dtRow = dtAbsSpn.Select("Id=" + Convert.ToInt32(cmbSelAbs.Value));
                            //if (dtRow.ElementAt(0)["DenumireScurta"] != null && 
                            //    ((dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 2 &&  dtRow.ElementAt(0)["DenumireScurta"].ToString().Substring(0, 2) == "CO") 
                            //        || (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 &&  dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP")))
                            if ((dtRow.ElementAt(0)["DenumireScurta"] != null && (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 && dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP"))
                                || Convert.ToInt32(dtRow.ElementAt(0)["Id"].ToString()) == Convert.ToInt32(Dami.ValoareParam("IdAbsentaCO", "1")))
                            {
                                //lblZileRamase.Visible = true;
                                //tdNrZileRamase.Visible = true;
                                //txtNrZileRamase.Visible = true;
                            }
                            else
                            {
                                lblZileRamase.Visible = false;
                                //tdNrZileRamase.Visible = false;
                                txtNrZileRamase.ClientVisible = false;
                            }
                        }

                        //Incarcam Absentele
                        Session["Cereri_Absente_Absente"] = dtAbsSpn; //dtAbs.Select("DenumireScurta LIKE '" + denumire + "'");

                        DataRow[] arr = dtAbsSpn.Select("Id=" + (cmbSelAbs.Visible == true ? General.Nz(cmbSelAbs.Value, -99) : General.Nz(cmbAbs.Value, -99)));
                        //DataRow[] arr = dtAbs.Select("Id=" + General.Nz(General.VarSession("User_Marca"), -99));
                        if (arr.Count() > 0)
                        {
                            //Afisam explicatiile
                            //calculam nr de zile luate
                            int nr = 0;
                            //int nrViitor = 0;
                            //string adunaZL = General.Nz(arr[0]["AdunaZileLibere"], "0").ToString();
                            //General.CalcZile(txtDataInc.Date, txtDataSf.Date, adunaZL, out nr, out nrViitor);
                            nr = General.CalcZile(Convert.ToInt32(General.Nz(General.VarSession("User_Marca"), -99)), Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), (cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value ?? 0) : Convert.ToInt32(cmbAbs.Value ?? 0)));;
                            txtNrZile.Value = nr;
                            Session["TactilNrZile"] = nr;
                            //txtNrZileViitor.Value = nrViitor;


                        }
                    }
                    AfiseazaCtl();

                }
                else
                {
                    AfiseazaCtl();
                    if (IsCallback)
                    {

                        if (Session["Absente_Cereri_Date_Aditionale"] != null) AfiseazaCtl();

                    }
                    if (Session["TactilNrZile"] != null)
                        txtNrZile.Value = Convert.ToInt32(Session["TactilNrZile"].ToString());


                }

                txtNrZile.ClientVisible = true;

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
                Response.Redirect("../Tactil/Main", false);       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0")
                    Response.Redirect("../DefaultTactil", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard", false);
                else if (tip == "3")
                    Response.Redirect("../DefaultTactilExtra", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkSave_Click(object sender, EventArgs e)
        {
            try
            {
                SalveazaDate(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private string SelectAngajati(int idRol = -44)
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
                        SELECT A.F10003, 0 AS ""Rol"", 'Angajat' AS ""RolDenumire""
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
                        SELECT A.F10003, 76 AS ""Rol"", 'Fara rol' AS ""RolDenumire""
                        FROM F100 A
                        INNER JOIN ""Ptj_Circuit"" C ON C.""UserIntrod""={General.VarSession("UserId")} {inn2}) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607
                        WHERE 1=1 ";

                if (idRol != -44) strSql += " AND Rol=" + idRol;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter;
                DataTable dtAbs = new DataTable(), dtAbsente = new DataTable();

                DataRow[] dtRowAbs = null;

                if (tip == "3" && txtDataInc.Value != null)
                {
                    txtDataSf.Value = txtDataInc.Value;                
                    DataTable dtZile = General.IncarcaDT("SELECT * FROM \"SituatieZileAbsente\" WHERE F10003 = " + General.Nz(General.VarSession("User_Marca"), -99).ToString() + " AND \"An\" = " + Convert.ToDateTime(txtDataInc.Value).Year, null);

                    if (dtZile != null && dtZile.Rows.Count > 0)
                        txtNrZileRamase.Text = dtZile.Rows[0]["Ramase"].ToString();
                    else
                        txtNrZileRamase.Text = "";
                }

                //if (Session["Cereri_Absente_Absente"] != null) dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                //if (Session["Cereri_Absente_Absente"] != null) dtRowAbs = Session["Cereri_Absente_Absente"] as DataRow[];
                if (Session["Cereri_Absente_Absente"] != null) dtAbsente = Session["Cereri_Absente_Absente"] as DataTable;

                dtRowAbs = dtAbsente.Select();

                if (dtRowAbs != null)
                    dtAbs = dtRowAbs.CopyToDataTable();

                if (dtAbs.Rows.Count > 0)
                {
                    object id = General.Nz(cmbSelAbs.Visible == true ? cmbSelAbs.Value : cmbAbs.Value, -99);
                    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                        id = General.Nz(cmbSelAbs.Value, -99);

                    DataRow[] arr = dtAbs.Select("Id=" + id);
                    //DataRow[] arr = dtAbs.Select("Id=" + General.Nz(General.VarSession("User_Marca"), -99));
                    if (arr.Count() > 0)
                    {
                        //Afisam explicatiile


                        //calculam nr de zile luate
                        int nr = 0;
                        //int nrViitor = 0;
                        //string adunaZL = General.Nz(arr[0]["AdunaZileLibere"], "0").ToString();
                        //General.CalcZile(txtDataInc.Date, txtDataSf.Date, adunaZL, out nr, out nrViitor);
                        nr = General.CalcZile(Convert.ToInt32(General.Nz(General.VarSession("User_Marca"), -99)), Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), (cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value ?? 0) : Convert.ToInt32(cmbAbs.Value ?? 0))); ;
                        txtNrZile.Value = nr;
                        Session["TactilNrZile"] = nr;
                        //txtNrZileViitor.Value = nrViitor;
                    }
                }



                switch (tip)
                {
                    case "1":               //cmbAng
                        {

                            Session["Cereri_Absente_Absente"] = dtAbs;
                            AfiseazaCtl();
                        }
                        break;
                    case "2":               //cmbAbs
                        AfiseazaCtl();
                        break;
                    case "3":               //DataInceput
                        {
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                            //lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                        }
                        break;
                    case "4":               //btnSave
                        {
                            SalveazaDate(2);
                            AfiseazaCtl();
                        }
                        break;
                    case "5":               //btnDocSterge
                        {
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                            itm.UploadedFile = null;
                            itm.UploadedFileName = null;
                            itm.UploadedFileExtension = null;

                            Session["Absente_Cereri_Date"] = itm;
                            //lblDoc.InnerHtml = "&nbsp;";
                        }
                        break;
                    case "6":                   //DataSfarsit
                        {
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                            //lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                        }
                        break;
                    case "7":                   //cmbRol



                        //acelasi cod ca la case "1"

                        Session["Cereri_Absente_Absente"] = dtAbs;
                        AfiseazaCtl();

                        break;
                    case "8":
                        DataTable dtAbsSpn = Session["Absente_Tactil"] as DataTable;
                        if (dtAbsSpn != null && dtAbsSpn.Rows.Count > 0)
                        {
                            DataRow[] dtRow = dtAbsSpn.Select("Id=" + (cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value) : Convert.ToInt32(cmbAbs.Value)));
                            //if (dtRow.ElementAt(0)["DenumireScurta"] != null &&
                            //                      ((dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 2 && dtRow.ElementAt(0)["DenumireScurta"].ToString().Substring(0, 2) == "CO")
                            //                          || (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 && dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP")))
                            if ((dtRow.ElementAt(0)["DenumireScurta"] != null && (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 && dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP"))
                                || Convert.ToInt32(dtRow.ElementAt(0)["Id"].ToString()) == Convert.ToInt32(Dami.ValoareParam("IdAbsentaCO", "1")))
                            {
                                //lblZileRamase.Visible = true;
                                //tdNrZileRamase.Visible = true;
                                //txtNrZileRamase.Visible = true;
                            }
                            else
                            {
                                lblZileRamase.Visible = false;
                                //tdNrZileRamase.Visible = false;
                                txtNrZileRamase.ClientVisible = false;
                            }
                            //if (dtRow.ElementAt(0)["AdunaZileLibere"] != null && dtRow.ElementAt(0)["AdunaZileLibere"].ToString() == "1")                            
                            //    lblZile.InnerText = "Nr. zile calendaristice";                            
                            //else
                            //    lblZile.InnerText = "Nr. zile ";


                        }
                        break;
                }

                #region OLD

                //if (tip == "1")
                //{
                //    dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString()), null);
                //    cmbAbs.DataSource = dtAbs;
                //    cmbAbs.DataBind();
                //    cmbAbs.SelectedIndex = -1;
                //    Session["Cereri_Absente_Absente"] = dtAbs;
                //    AfiseazaCtl();
                //}

                //if (tip == "2")
                //{
                //    AfiseazaCtl();

                //    #region OLD

                //    //DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE IdAbsenta=@1", new object[] { cmbAbs.Value });
                //    //for (int i = 0; i < dt.Rows.Count; i++)
                //    //{
                //    //    DataRow dr = dt.Rows[i];

                //    //    HtmlGenericControl ctlDiv = new HtmlGenericControl("div");
                //    //    ctlDiv.Attributes["class"] = "Absente_Cereri_CampuriSup";

                //    //    Label lbl = new Label();
                //    //    lbl.ID = "lblDinamic" + i;
                //    //    lbl.Text = dr["Denumire"].ToString();
                //    //    lbl.Style.Add("margin", "10px 0px !important");
                //    //    ctlDiv.Controls.Add(lbl);

                //    //    switch (General.Nz(dr["TipCamp"], "").ToString())
                //    //    {
                //    //        case "0":                   //text
                //    //            ASPxTextBox txt = new ASPxTextBox();
                //    //            txt.ID = "ctlDinamic" + i;
                //    //            txt.ClientIDMode = ClientIDMode.Static;
                //    //            txt.ClientInstanceName = "ctlDinamic" + i;
                //    //            txt.Width = Unit.Pixel(70);
                //    //            txt.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                //    //            if (General.Nz(dr["Sursa"], "").ToString() != "")
                //    //            {
                //    //                string sel = dr["Sursa"].ToString().Replace("ent.F10003", General.Nz(cmbAng.Value, "").ToString()).Replace("ent.DataInceput", General.ToDataUniv((DateTime)General.Nz(txtDataInc.Value, DateTime.Now)));
                //    //                object val = General.Nz(General.ExecutaScalar(sel, null), "");
                //    //                txt.Value = val.ToString();
                //    //            }
                //    //            ctlDiv.Controls.Add(txt);
                //    //            break;
                //    //        case "1":                   //checkBox
                //    //            ASPxCheckBox chk = new ASPxCheckBox();
                //    //            chk.ID = "ctlDinamic" + i;
                //    //            chk.ClientIDMode = ClientIDMode.Static;
                //    //            chk.ClientInstanceName = "ctlDinamic" + i;
                //    //            chk.Checked = false;
                //    //            chk.AllowGrayed = false;
                //    //            chk.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                //    //            if (General.Nz(dr["Sursa"], "").ToString() != "")
                //    //            {
                //    //                string sel = dr["Sursa"].ToString().Replace("ent.F10003", General.Nz(cmbAng.Value, "").ToString()).Replace("ent.DataInceput", General.ToDataUniv((DateTime)General.Nz(txtDataInc.Value, DateTime.Now)));
                //    //                object val = General.Nz(General.ExecutaScalar(sel, null), "");
                //    //                chk.Value = val.ToString();
                //    //            }
                //    //            ctlDiv.Controls.Add(chk);
                //    //            break;
                //    //        case "3":                   //dateTime
                //    //            ASPxDateEdit dte = new ASPxDateEdit();
                //    //            dte.ID = "ctlDinamic" + i;
                //    //            dte.ClientIDMode = ClientIDMode.Static;
                //    //            dte.ClientInstanceName = "ctlDinamic" + i;
                //    //            dte.Width = Unit.Pixel(100);
                //    //            dte.DisplayFormatString = "dd/MM/yyyy";
                //    //            dte.EditFormat = EditFormat.Custom;
                //    //            dte.EditFormatString = "dd/MM/yyyy";
                //    //            dte.ReadOnly = General.Nz(dr["ReadOnly"], "0").ToString() == "0" ? false : true;
                //    //            if (General.Nz(dr["Sursa"], "").ToString() != "")
                //    //            {
                //    //                string sel = dr["Sursa"].ToString().Replace("ent.F10003", General.Nz(cmbAng.Value, "").ToString()).Replace("ent.DataInceput", General.ToDataUniv((DateTime)General.Nz(txtDataInc.Value, DateTime.Now)));
                //    //                object val = General.Nz(General.ExecutaScalar(sel, null), "");
                //    //                dte.Value = val.ToString();
                //    //            }
                //    //            ctlDiv.Controls.Add(dte);
                //    //            break;
                //    //    }

                //    //    divDateSup.Controls.Add(ctlDiv);
                //    //}

                //    #endregion
                //}

                //if (tip == "3") if (txtDataInc.Value != null) txtDataSf.Value = txtDataInc.Value;

                ////OreCalculate();

                //dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                //if (dtAbs.Rows.Count > 0)
                //{
                //    DataRow[] arr = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                //    if (arr.Count() > 0)
                //    {
                //        //Afisam explicatiile
                //        txtAbsDesc.InnerText = General.Nz(arr[0]["Explicatii"], "").ToString();

                //        //calculam nr de zile luate
                //        int nr = 0;
                //        int nrViitor = 0;
                //        string adunaZL = General.Nz(arr[0]["AdunaZileLibere"], "0").ToString();
                //        General.CalcZile(txtDataInc.Date, txtDataSf.Date, adunaZL, out nr, out nrViitor);
                //        txtNrZile.Value = nr;
                //        txtNrZileViitor.Value = nrViitor;

                //        //AfiseazaCtl();
                //    }
                //}

                //if (tip == "4")
                //{
                //    SalveazaDate(2);

                //    metaCereriDate itm = new metaCereriDate();
                //    if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                //    lblDoc.InnerText = General.Nz(itm.UploadedFileName,"").ToString();
                //}

                //if (tip == "5")
                //{
                //    metaCereriDate itm = new metaCereriDate();
                //    if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                //    itm.UploadedFile = null;
                //    itm.UploadedFileName = null;
                //    itm.UploadedFileExtension = null;

                //    Session["Absente_Cereri_Date"] = itm;

                //    //Session["Absente_Cereri_UploadedFile"] = null;
                //    //Session["Absente_Cereri_UploadedFileName"] = null;
                //    //Session["Absente_Cereri_UploadedFileExtension"] = null;

                //    lblDoc.InnerHtml = "&nbsp;";
                //}
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        //Florin 2020.01.22

        //public string VerificareDepasireNorma(int f10003, DateTime dtInc, int? nrOre, int tip)
        //{
        //    //tip
        //    //tip - 1  vine din cererei - unde trebuie sa luam in caclul si valorile care deja exista in pontaj
        //    //tip - 2  vine din pontaj  - valorile sunt deja in pontaj


        //    string msg = "";

        //    try
        //    {
        //        //calculam norma
        //        string strSql = "SELECT CAST(rez as int) FROM DamiNorma(" + f10003 + "," + General.ToDataUniv(dtInc) + ")";
        //        if (Constante.tipBD == 2) strSql = "SELECT \"DamiNorma\"(" + f10003 + ", " + General.ToDataUniv(dtInc) + ") FROM DUAL";
        //        int norma = Convert.ToInt32(General.ExecutaScalar(strSql, null));

        //        int sumaPtj = 0;
        //        if (tip == 1)
        //        {
        //            //absentele din pontaj care intra in suma de ore
        //            string sqlOre = @"SELECT ' + COALESCE(' + OreInVal + ',0)'  FROM Ptj_tblAbsente WHERE COALESCE(VerificareNrMaxOre,0) = 1 FOR XML PATH ('')";
        //            if (Constante.tipBD == 2) sqlOre = @"SELECT LISTAGG('COALESCE(' || ""OreInVal"" || ')', ' + ') WITHIN GROUP (ORDER BY ""OreInVal"") FROM ""Ptj_tblAbsente"" WHERE COALESCE(VerificareNrMaxOre,0) = 1";
        //            string strVal = (General.ExecutaScalar(sqlOre, null) ?? "").ToString();
        //            if (Constante.tipBD == 1) strVal = strVal.Substring(3);
        //            if (strVal != "") sumaPtj = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM({strVal}), 0) FROM ""Ptj_Intrari"" WHERE F10003={f10003} AND ""Ziua""={General.ToDataUniv(dtInc.Date)}", null));
        //        }

        //        //suma de ore din Cereri
        //        int sumaCere = Convert.ToInt32(General.ExecutaScalar($@"SELECT COALESCE(SUM(COALESCE(""NrOre"",0)),0) FROM ""Ptj_Cereri"" WHERE F10003={f10003} AND ""DataInceput"" = {General.ToDataUniv(dtInc.Date)} AND ""IdStare"" IN (1,2)", null));
        //        if (((sumaCere * 60) + sumaPtj + (nrOre * 60)) > (norma * 60))
        //        {
        //            msg = "Totalul de ore depaseste norma pe aceasta zi";
        //            return msg;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return msg;
        //}

        private void SalveazaDate(int tip = 1)
        {
            try
            {
                string strErr = "";

                if (txtDataInc.Text == "") strErr += ", " + Dami.TraduCuvant("data inceput");
                if (txtDataSf.Text == "") strErr += ", " + Dami.TraduCuvant("data sfarsit");


                //ASPxPanel.RedirectOnCallback("../Tactil/Main.aspx");



                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

                    return;
                }
                DataRow drAbs = null;
                DataTable dtAbs = new DataTable(), dtAbsente = new DataTable();
                DataRow[] dtRowAbs = null;
                //if (Session["Cereri_Absente_Absente"] != null) dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                //if (Session["Cereri_Absente_Absente"] != null) dtRowAbs = Session["Cereri_Absente_Absente"] as DataRow[];
                //if (Session["Cereri_Absente_Absente"] != null) dtRowAbs = Session["Cereri_Absente_Absente"] as DataRow[];
                if (Session["Cereri_Absente_Absente"] != null) dtAbsente = Session["Cereri_Absente_Absente"] as DataTable;


                if (dtAbsente != null && dtAbsente.Rows.Count > 0)
                {
                    dtRowAbs = dtAbsente.Select();                    

                    if (dtRowAbs != null && dtRowAbs.Count() > 0)
                    {
                        dtAbs = dtRowAbs.CopyToDataTable();

                        if (Session["CereriTactil"].ToString() == "BiletVoie")
                        {
                            if (rbMotiv1.Checked)
                                cmbAbs.Value = Convert.ToInt32(dtAbs.Rows[1]["Id"].ToString());
                            else
                                cmbAbs.Value = Convert.ToInt32(dtAbs.Rows[0]["Id"].ToString());
                        }

                        object id = General.Nz(cmbSelAbs.Visible == true ? cmbSelAbs.Value : cmbAbs.Value, -99);
                        if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                            id = General.Nz(cmbSelAbs.Value, -99);

                        DataRow[] lst = dtAbs.Select("Id=" + id);
                        if (lst.Count() == 0) return;

                        drAbs = lst[0];
                    }
                    else
                        return;
                }
                else
                    return;



                metaCereriDate itmAta = new metaCereriDate();
                if (Session["Absente_Cereri_Date"] != null) itmAta = Session["Absente_Cereri_Date"] as metaCereriDate;

                //if (Convert.ToInt32(General.Nz(drAbs["ArataAtasament"], 0)) == (int)Constante.ArataControl.Obligatoriu && itmAta.UploadedFile == null) strErr += ", atasamentul";
                //if (Convert.ToInt32(General.Nz(drAbs["ArataInlocuitor"], 0)) == (int)Constante.ArataControl.Obligatoriu && cmbInloc.Value == null) strErr += ", " + Dami.TraduCuvant("inlocuitorul");
                //if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && txtNrOre.Value == null) strErr += ", " + Dami.TraduCuvant("nr. ore");

                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

                    return;
                }


                #region Verificare validitate angajat

                try
                {
                    int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={General.VarSession("User_Marca")} AND F10022 < {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value))} < F10023", null), 0));
                    if (esteActiv == 0)
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("In perioada solicitata, angajatul este inactiv"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In perioada solicitata, angajatul este inactiv");

                        return;
                    }
                }
                catch (Exception) { }

                #endregion  



                #region Verificare Campuri Extra

                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                DataTable dtEx = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1 AND ""AfisareInfoChiosc"" = 1", new object[] { General.Nz((cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value) : Convert.ToInt32(cmbAbs.Value)), "-99") });
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    DataRow dr = dtEx.Rows[i];
                    ASPxEdit ctl = divDateExtra.FindControl("ctlDinamic" + i) as ASPxEdit;

                    if (Session["CereriTactil"].ToString() == "AbsenteOra" || Session["CereriTactil"].ToString() == "BiletVoie")                    
                        ctl = divDateExtraOre.FindControl("ctlDinamic" + i) as ASPxEdit;   
                    
                    if (Convert.ToInt32(General.Nz(dr["Obligatoriu"], 0)) == 1)
                    {
                        if (ctl == null || ctl.Value == null || ctl.Value.ToString() == "") strErr += ", " + General.Nz(dr["Denumire"], "date extra").ToString();
                    }

                }


                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

                    return;
                }


                #endregion


                #region verif client


                if (txtDataSf.Date < txtDataInc.Date && Session["CereriTactil"].ToString() != "BiletVoie" && Session["CereriTactil"].ToString() != "AbsenteOra") strErr += " " + Dami.TraduCuvant("Data sfarsit este mai mica decat data inceput");

                if (txtDataSf.Date < txtDataInc.Date && (Session["CereriTactil"].ToString() == "BiletVoie" || Session["CereriTactil"].ToString() == "AbsenteOra"))
                    txtDataSf.Date = txtDataInc.Date;

                if (txtDataSf.ClientVisible == false)
                    txtDataSf.Date = txtDataInc.Date;

                //daca abs este de tip ore dtinc si datasf trebuie sa fie aceeasi
                //if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && txtDataInc.Date != txtDataSf.Date) strErr += " " + Dami.TraduCuvant("Data inceput si data sfarsit trebuie sa fie aceeasi in cazul acestui tip de absenta");


                #region OLD

                //#############################################################################################
                //S-au modificat campurile; aceasta verificare se face mai jos cu campurile NrMax si NrMaxAn
                //#############################################################################################

                ////nr de ore cerute sa nu depaseasca nr max de ore permis
                //if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && Convert.ToInt32(General.Nz(drAbs["NrMaxOre"], 999)) < Convert.ToInt32(txtNrOre.Value)) strErr += " " + Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMaxOre"], 999)) + " ore");

                #endregion


                if (Session["CereriTactil"].ToString() == "BiletVoie" || Session["CereriTactil"].ToString() == "AbsenteOra")
                {
                    int nrMinute = 0;
                    if (hfNrMinute.Contains("NrMinute"))
                    {
                        nrMinute = Convert.ToInt32(General.Nz(hfNrMinute["NrMinute"], -1));
                        txtNrOreInMinute.Value = nrMinute;
                        txtNrOreInMinute.Text = nrMinute.ToString();
                    }
                    if (Convert.ToInt32(General.Nz(txtNrOre.Value, -99)) <= 0 && Convert.ToInt32(General.Nz(txtNrOreInMinute.Value, -99)) <= 0) strErr += " " + Dami.TraduCuvant("Cerere cu numar de ore 0");
                }
                else
                {
                    if (Convert.ToInt32(General.Nz(txtNrZile.Value, -99)) <= 0) strErr += " " + Dami.TraduCuvant("Cerere cu numar de zile 0");
                }

                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(strErr, MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = strErr;

                    return;
                }

                var ert = General.Nz(General.VarSession("User_Marca"), -98);
                var edc = General.Nz(Session["User_Marca"], -97);
                var esx = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { General.VarSession("User_Marca") }));

                if (Convert.ToInt32(General.Nz(General.VarSession("User_Marca"), -98)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -97)) && Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { General.VarSession("User_Marca") })) == 0)
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Angajatul nu are nici un supervizor"), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul nu are nici un supervizor");

                    return;
                }

                #endregion

                #region  verif server

                //verificam daca se intersecteaza cu alte intervale
                //modificare facute pt Honeywell; pt a putea face mai multe cereri cu x ore in aceeasi zi
                int intersec = Convert.ToInt32(General.ExecutaScalar($@"
                                SELECT COUNT(*) 
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.F10003 = {General.VarSession("User_Marca")} AND A.""DataInceput"" <= {General.ToDataUniv(txtDataSf.Date)} AND {General.ToDataUniv(txtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND A.""IdStare"" IN (1,2,3,4) AND B.""GrupOre"" IN({General.Nz(drAbs["GrupOreDeVerificat"], -99)})", null));

                if (intersec > 0)
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Intervalul se intersecteaza cu altul deja existent"), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Intervalul se intersecteaza cu altul deja existent");

                    return;
                }


                //Aici nu se tine cont de stare
                int valoare = -99;
                int nrZile = 0;

                string sqlDrp = $@"SELECT  
                                (SELECT TOP 1 Valoare FROM Ptj_CereriDrepturi DR WHERE (DR.IdAbs={Convert.ToInt32(cmbAbs.Value)} OR DR.IdAbs = -13) AND (DR.IdRol={Convert.ToInt32(cmbRol.Value ?? -99)} OR DR.IdRol = -13) AND (DR.IdActiune=4 OR DR.IdActiune = -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Valoare,
                                (SELECT TOP 1 NrZile FROM Ptj_CereriDrepturi DR WHERE (DR.IdAbs={Convert.ToInt32(cmbAbs.Value)} OR DR.IdAbs = -13) AND (DR.IdRol={Convert.ToInt32(cmbRol.Value ?? -99)} OR DR.IdRol = -13) AND (DR.IdActiune=4 OR DR.IdActiune = -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS NrZile";
                if (Constante.tipBD == 2)
                    sqlDrp = $@"   SELECT  
                                (SELECT  ""Valoare"" FROM ""Ptj_CereriDrepturi"" DR WHERE (DR.""IdAbs""={Convert.ToInt32(cmbAbs.Value)} OR DR.""IdAbs"" = -13) AND (DR.""IdRol"" = {Convert.ToInt32(cmbRol.Value ?? -99)} OR DR.""IdRol"" = -13) AND (DR.""IdActiune"" = 4 OR DR.""IdActiune"" = -13) and rownum=1 ) AS ""Valoare"",
                                (SELECT  ""NrZile"" FROM ""Ptj_CereriDrepturi"" DR WHERE (DR.""IdAbs"" = {Convert.ToInt32(cmbAbs.Value)} OR DR.""IdAbs"" = -13) AND(  DR.""IdRol"" = {Convert.ToInt32(cmbRol.Value ?? -99)}  OR DR.""IdRol"" = -13) AND(DR.""IdActiune"" = 4 OR DR.""IdActiune"" = -13) and rownum = 1) AS ""NrZile"" FROM dual ";

                DataTable dtDrp = General.IncarcaDT(sqlDrp, null);
                if (dtDrp.Rows.Count > 0)
                {
                    valoare = Convert.ToInt32(General.Nz(dtDrp.Rows[0]["Valoare"], -99));
                    nrZile = Convert.ToInt32(General.Nz(dtDrp.Rows[0]["NrZile"], 0));
                }

                if (valoare == -99)
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Nu aveti drepturi sa creati acest tip de absenta"), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi sa creati acest tip de absenta");

                    return;
                } 

                DateTime ziDrp = Dami.DataDrepturi(valoare, nrZile, Convert.ToDateTime(txtDataInc.Value), Convert.ToInt32(General.VarSession("User_Marca") ?? -99));
                if (txtDataInc.Date < ziDrp.Date)
                {
                    if (valoare == 13)
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Pontajul a fost aprobat pentru aceasta perioada"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pontajul a fost aprobat pentru aceasta perioada");
                    }
                    else
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat " + ziDrp.Date.ToShortDateString()), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat " + ziDrp.Date.ToShortDateString());
                    }

                    return;
                }


                #endregion

                #region verificam starea pontajului



                #endregion

                #region Verificam sa nu depaseasca norma


                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0)
                {
                    //ce are voie sa introduca
                    DataRow dtCtr = General.IncarcaDR($@"
                            SELECT A.*
                            FROM ""Ptj_ContracteAbsente"" A
                            INNER JOIN ""F100Contracte"" B ON A.""IdContract"" = B.""IdContract""
                            WHERE A.""IdAbsenta"" = {Convert.ToInt32(General.VarSession("User_Marca"))} AND B.F10003 = {Convert.ToInt32(General.VarSession("User_Marca"))} 
                            AND B.""DataInceput"" <= {General.ToDataUniv(txtDataInc.Date)} AND {General.ToDataUniv(txtDataInc.Date)} <= B.""DataSfarsit""", null);

                    if (dtCtr != null)
                    {
                        int esteSL = 0;
                        int esteS = 0;
                        int esteD = 0;
                        int esteZL = 1;

                        esteSL = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM HOLIDAYS WHERE DAY =@1", new object[] { txtDataInc.Date }) ?? 0);
                        if (txtDataInc.Date.DayOfWeek == DayOfWeek.Saturday) esteS = 1;
                        if (txtDataInc.Date.DayOfWeek == DayOfWeek.Sunday) esteD = 1;
                        if (esteSL == 1 || esteS == 1 || esteD == 1) esteZL = 0;

                        if ((esteSL != 0 && Convert.ToInt32(General.Nz(dtCtr["SL"], 0)) == 1) || (esteS != 0 && Convert.ToInt32(General.Nz(dtCtr["S"], 0)) == 1) || (esteD != 0 && Convert.ToInt32(General.Nz(dtCtr["D"], 0)) == 1) || (esteZL != 0 && Convert.ToInt32(General.Nz(dtCtr["ZL"], 0)) == 1))
                        {
                            //are voie sa ponteze aceasta absenta
                        }
                        else
                        {
                            if (tip == 1)
                                MessageBox.Show(Dami.TraduCuvant("Contractul nu permite aceasta absenta"), MessageBox.icoWarning);
                            else
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Contractul nu permite aceasta absenta");

                            return;
                        }

                        if (Convert.ToInt32(General.Nz(drAbs["VerificareNrMaxOre"], 0)) == 1)
                        {
                            //string msgNr = VerificareDepasireNorma(Convert.ToInt32(General.VarSession("User_Marca")), txtDataInc.Date, Convert.ToInt32(txtNrOre.Value ?? 0), 1);
                            //if (msgNr != "")
                            //{
                            //    if (tip == 1)
                            //        MessageBox.Show(Dami.TraduCuvant(msgNr), MessageBox.icoWarning);
                            //    else
                            //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msgNr);

                            //    return;
                            //}
                        }

                    }
                }

                #endregion

                #region Validare Max Ore

                int cv = Convert.ToInt32(Dami.ValoareParam("ZileCuveniteInAvans", "0") ?? "0");




                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0)
                {
                    //daca este absenta de tip ora atunci se verifica doar campul NrMax; campul NrMaxAn nu are sens
                    //if (Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) < Convert.ToInt32(txtNrOre.Value))
                    //{
                    //    //strErr += " " + Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " ore");
                    //    if (tip == 1)
                    //        MessageBox.Show(Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " ore"), MessageBox.icoWarning);
                    //    else
                    //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " ore");

                    //    return;
                    //}
                }
                else
                {
                    //daca este absenta de tip zi

                    //verificam nr max pe cerere
                    //folosim campul txtNrZile pt ca este deja calculat cu nr de zile in functie de parametrul AdunaZileLibere
                    if (txtNrZile.Value != null && drAbs["NrMax"] != null && Convert.ToInt32(txtNrZile.Value) > Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)))
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite");

                        return;
                    }


                    //verificam nr max pe an
                    string sqlAn = $@"SELECT COALESCE(SUM(COALESCE(""NrZile"",0)),0) AS ""ZileAn"" FROM ""Ptj_Cereri"" WHERE F10003=@1 AND {General.FunctiiData("\"DataInceput\"", "A")}=@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                    DataRow drAn = General.IncarcaDR(sqlAn, new object[] { Convert.ToInt32(General.VarSession("User_Marca")), txtDataInc.Date.Year, Convert.ToInt32(General.VarSession("User_Marca")) });

                    if (drAn != null && drAbs[0] != null && drAbs["NrMaxAn"] != DBNull.Value && Convert.ToInt32(drAbs["NrMaxAn"]) > (int)drAbs[0])
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an");

                        return;
                    }

                    #region OLD
                    //De sters - procesul de mai jos se face prin procedura de validare
                    ////Radu 18.01.2018 - verificare daca NrZile este <= NrZileRamase
                    //DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1 AND UPPER(""Denumire"") like '%RAMASE%' ", new object[] { General.Nz(cmbAbs.Value, "-99") });
                    //if (dt != null && dt.Rows.Count > 0)
                    //{
                    //    if (General.Nz(dt.Rows[0]["Sursa"], "").ToString() != "")
                    //    {
                    //        string sel = InlocuiesteCampuri(dt.Rows[0]["Sursa"].ToString());
                    //        if (sel != "")
                    //        {
                    //            object val = General.Nz(General.ExecutaScalar(sel, null), "");
                    //            if (val != null && val.ToString().Length > 0)
                    //            {
                    //                decimal nrZileRamase = Convert.ToDecimal(val.ToString());
                    //                if (Convert.ToInt32(dt.Rows[0]["IdAbsenta"].ToString()) == 1)
                    //                {
                    //                    if (Convert.ToInt32(txtNrZile.Value) > nrZileRamase)
                    //                    {
                    //                        if (tip == 1)
                    //                            MessageBox.Show(Dami.TraduCuvant("Nr de zile solicitate depaseste nr de zile ramase!"), MessageBox.icoWarning);
                    //                        else
                    //                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile solicitate depaseste nr de zile ramase!");
                    //                        return;
                    //                    }
                    //                }
                    //                if (Convert.ToInt32(dt.Rows[0]["IdAbsenta"].ToString()) == 2)
                    //                {
                    //                    if (Convert.ToDecimal(Convert.ToInt32(txtNrZile.Value) * 0.5) > nrZileRamase)
                    //                    {
                    //                        if (tip == 1)
                    //                            MessageBox.Show(Dami.TraduCuvant("Nr de zile solicitate depaseste nr de zile ramase!"), MessageBox.icoWarning);
                    //                        else
                    //                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile solicitate depaseste nr de zile ramase!");
                    //                        return;
                    //                    }
                    //                }

                    //            }
                    //        }
                    //    }
                    //}
                    #endregion
                }




                #region OLD

                //#############################################################################################
                //S-au modificat campurile; aceasta verificare se face mai sus cu campurile NrMax si NrMaxAn
                //#############################################################################################

                ////verificam sa nu depaseasca nr max de zile
                //int nrZileCal = Convert.ToInt32((txtDataSf.Date - txtDataInc.Date).TotalDays) + 1;

                //if (nrZileCal > Convert.ToInt32(General.Nz(drAbs["NrMaxZileCalendaristice"], 999)))
                //{
                //    if (tip == 1)
                //        MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite"), MessageBox.icoWarning);
                //    else
                //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite");

                //    return;
                //}
                //if (Convert.ToInt32(txtNrZile.Value) > Convert.ToInt32(General.Nz(drAbs["NrMaxZileLucratoare"], 999)))
                //{
                //    if (tip == 1)
                //        MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite"), MessageBox.icoWarning);
                //    else
                //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite");

                //    return;
                //}

                ////Cod dezactivat. Nu se poate face verificarea in pontaj pt ca poti sa ai mai multe absente cu aceeasi denumire scurta asa ca se face in cereri
                //// - casatorie    EV
                //// - zi nastere   EV
                //// - deces        EV
                ////int zileLucrAn = ctx.Ptj_Intrari.Where(p => p.F10003 == f10003 && p.Ziua.Year == dtInc.Year && p.ValStr == entAbs.DenumireScurta).Count();
                ////ZEMY + unificare cu planificare
                //string sqlAn = @"SELECT COUNT(*) AS 'ZileLucrateAn', COALESCE(SUM(DATEDIFF(d,DataInceput, DataSfarsit)),0) AS 'ZileCalendAn' FROM ""Ptj_Cereri"" WHERE F10003=@1 AND YEAR(""DataInceput"") =@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                //if (Constante.tipBD == 2) sqlAn = @"SELECT COUNT(*) AS ""ZileLucrateAn"", COALESCE(SUM(""DataSfarsit"" - ""DataInceput""),0) AS ""ZileCalendAn"" FROM ""Ptj_Cereri"" WHERE F10003=@1 AND TO_NUMBER(TO_CHAR(""DataInceput"",'YYYY')) =@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                //DataRow drAn = General.IncarcaDR(sqlAn, new object[] { Convert.ToInt32(cmbAng.Value), txtDataInc.Date.Year, Convert.ToInt32(cmbAbs.Value) });

                //if (drAn != null)
                //{
                //    if (Convert.ToInt32(General.Nz(drAbs["NrMaxZileCalendaristiceAn"], -99)) > 0 && ((nrZileCal + Convert.ToInt32(General.Nz(drAbs["ZileCalendAn"], 0))) > Convert.ToInt32(General.Nz(drAbs["NrMaxZileCalendaristiceAn"], -99))))
                //    {
                //        if (tip == 1)
                //            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an"), MessageBox.icoWarning);
                //        else
                //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an");

                //        return;
                //    }

                //    if (Convert.ToInt32(General.Nz(drAbs["NrMaxZileLucratoareAn"], -99)) > 0 && ((Convert.ToInt32(txtNrZile.Value) + Convert.ToInt32(General.Nz(drAbs["ZileLucrateAn"], 0)))) > Convert.ToInt32(General.Nz(drAbs["NrMaxZileLucratoareAn"], 0)))
                //    {
                //        if (tip == 1)
                //            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an"), MessageBox.icoWarning);
                //        else
                //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an");

                //        return;
                //    }
                //}

                #endregion

                try
                {

                }
                catch (Exception)
                {
                }

                #endregion

                #region Salvare in baza

                #region Construim istoricul

                //Florin 2021.03.17 - citire idCerere din secventa
                int idCerere = Dami.NextId("Ptj_Cereri");

                string sqlIst;
                int trimiteLaInlocuitor;
                General.SelectCereriIstoric(Convert.ToInt32(General.VarSession("User_Marca")), -1, Convert.ToInt32(drAbs["IdCircuit"]), Convert.ToInt32(General.Nz(drAbs["EstePlanificare"], 0)), out sqlIst, out trimiteLaInlocuitor, idCerere, txtDataInc.Date);

                #endregion

                //Florin 2019.10.17 - am mutat sqlCer in afara if-ului pt ca trebuie in modulul de notificari indiferent daca este sql sau oracle
                string sqlCer = CreazaSelectCuValori(1, idCerere);
                string sqlPre = "";
                string strGen = "";

                if (Constante.tipBD == 1)
                {
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"",  ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") 
                                OUTPUT Inserted.Id, Inserted.IdStare ";

                    strGen = "BEGIN TRAN " +
                            sqlIst + "; " +
                            sqlPre +
                            sqlCer + "; " +
                            "COMMIT TRAN";
                }
                else
                {
                    string sqlCerOrcl = CreazaSelectCuValori(2);
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") ";

                    strGen = "BEGIN " +
                                sqlIst + "; " + Environment.NewLine +
                                sqlPre +
                                sqlCerOrcl + " RETURNING \"Id\", \"IdStare\" INTO @out_1, @out_2; " +
                                @"
                                EXCEPTION
                                    WHEN DUP_VAL_ON_INDEX THEN
                                        ROLLBACK;
                            END;";
                }

                if (Dami.ValoareParam("LogCereri", "0") == "1") General.CreazaLogCereri(strGen, General.VarSession("User_Marca").ToString(), txtDataInc.Value.ToString());

                string msg = Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " + General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    if (tip == 1)
                        MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);

                    return;
                }
                else
                {
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
                        return;
                    }


                    string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        NotifAsync.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + idCer, "Ptj_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), arrParam);
                    });

                    //Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" WHERE ""Id""=" + idCer, "Ptj_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                    #region  Pontaj start

                    //completeaza soldul de ZL; Este numai pt clientul Groupama
                    General.SituatieZLOperatii(Convert.ToInt32(General.VarSession("User_Marca") ?? -99), txtDataInc.Date, 2, Convert.ToInt32(txtNrZile.Value));

                    //trimite in pontaj daca este finalizat
                    if (idStare == 3)
                    {
                        if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                        {
                            //General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), idCer, 5, trimiteLaInlocuitor, Convert.ToInt32(txtNrOre.Value ?? 0));

                            //Se va face cand vom migra GAM
                            //TrimiteCerereInF300(Session["UserId"], idCer);
                        }
                    }

                    #endregion
                }

                #endregion

                Session["Absente_Cereri_Date_Aditionale"] = null;
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");

                // ASPxPanel.RedirectOnCallback("../Tactil/Main");
                //Response.Redirect("../Tactil/Main", false);
                Response.RedirectLocation = System.Web.VirtualPathUtility.ToAbsolute("~/Tactil/Main");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AfiseazaCtl()
        {
            try
            {
                DataTable dtAbs = Session["Cereri_Absente_Absente"] as DataTable;

                string arataAtas = "1";
                string arataInloc = "1";
                string idOre = "1";

                if (dtAbs != null && dtAbs.Rows.Count > 0)
                {
                    //DataRow[] arr = dtAbs.Select("Id=" + General.Nz(General.VarSession("User_Marca"), "-99"));
                    DataRow[] arr = dtAbs.Select("Id=" + General.Nz(cmbSelAbs.Value, "-99"));
                    if (arr.Count() > 0)
                    {
                        DataRow dr = arr[0];

                        arataInloc = General.Nz(dr["ArataInlocuitor"], "1").ToString();
                        arataAtas = General.Nz(dr["ArataAtasament"], "1").ToString();
                        idOre = General.Nz(dr["IdTipOre"], "1").ToString();

                        int folosesteInterval = 0;
                        int perioada = 0;
                        idOre = General.Nz(dr["IdTipOre"], "1").ToString();
                        folosesteInterval = Convert.ToInt32(General.Nz(dr["AbsentaTipOraFolosesteInterval"], 0));
                        perioada = Convert.ToInt32(General.Nz(dr["AbsentaTipOraPerioada"], 0));
                        if (perioada <= 0) perioada = 60;
                        if (perioada > 60) perioada = 60;

                        if (idOre == "0")
                        {
                            if (folosesteInterval == 1)
                            {
                                List<Module.Dami.metaGeneral2> lst = ListaInterval(perioada);

                                tdNrOreInMinute.Visible = true;
                                //txtNrOreInMinute.Visible = true;
                                //txtNrOreInMinute.ClientEnabled = false;
                                txtNrOreTime.ClientVisible = true;
                                txtNrOreTime.ClientEnabled = false;
                                tdNrOre.Visible = false;
                                txtNrOre.ClientVisible = false;
                                lblOraInc.Visible = true;
                                lblOraSf.Visible = true;
                                tdOraInc.Visible = true;
                                cmbOraInc.ClientVisible = true;
                                cmbOraInc.DataSource = lst;
                                cmbOraInc.DataBind();
                                tdOraSf.Visible = true;
                                cmbOraSf.ClientVisible = true;
                                cmbOraSf.DataSource = lst;
                                cmbOraSf.DataBind();
                                tdNrOre.Width = "300";
                                tdNrOreInMinute.Width = "550";
                                H3.InnerText = "Nr. ore";
                            }
                            else
                                tdNrOre.Width = "1200";
                        }
                    }
                }

                DataTable dtAbsSpn = Session["Absente_Tactil"] as DataTable;
                if (dtAbsSpn != null && dtAbsSpn.Rows.Count > 0)
                {
                    DataRow[] dtRow = dtAbsSpn.Select("Id=" + (cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value) : Convert.ToInt32(cmbAbs.Value)));
                    //if (dtRow.ElementAt(0)["DenumireScurta"] != null &&
                    //                      ((dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 2 && dtRow.ElementAt(0)["DenumireScurta"].ToString().Substring(0, 2) == "CO")
                    //                          || (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 && dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP")))
                    if ((dtRow.ElementAt(0)["DenumireScurta"] != null && (dtRow.ElementAt(0)["DenumireScurta"].ToString().Length >= 3 && dtRow.ElementAt(0)["DenumireScurta"].ToString() == "ZLP"))
                        || Convert.ToInt32(dtRow.ElementAt(0)["Id"].ToString()) == Convert.ToInt32(Dami.ValoareParam("IdAbsentaCO", "1")))
                    {
                        //lblZileRamase.Visible = true;
                        //tdNrZileRamase.Visible = true;
                        //txtNrZileRamase.Visible = true;
                    }
                    else
                    {
                        lblZileRamase.Visible = false;
                        //tdNrZileRamase.Visible = false;
                        txtNrZileRamase.ClientVisible = false;
                    }
                    //if (dtRow.ElementAt(0)["AdunaZileLibere"] != null && dtRow.ElementAt(0)["AdunaZileLibere"].ToString() == "1")                            
                    //    lblZile.InnerText = "Nr. zile calendaristice";                            
                    //else
                    //    lblZile.InnerText = "Nr. zile ";


                }

                if (idOre == "0")
                {
                    //lblNrOre.Style["display"] = "inline-block";
                    //txtNrOre.Visible = true;
                }
                else
                {
                    //lblNrOre.Style["display"] = "none";
                    //txtNrOre.Visible = false;
                    //txtNrOre.Value = null;
                }

                AfiseazaCtlExtra();

                #region old

                //if (!string.IsNullOrEmpty(dr["CampData1"].ToString()))
                //{
                //    lblData1.InnerText = Dami.TraduCuvant(dr["CampData1"].ToString());
                //    lblData1.Style["display"] = "inline-block";
                //    txtData1.Visible = true;
                //}
                //else
                //{
                //    lblData1.InnerText = "";
                //    lblData1.Style["display"] = "none";
                //    txtData1.Visible = false;
                //    txtData1.Value = null;
                //}

                //if (!string.IsNullOrEmpty(dr["CampData2"].ToString()))
                //{
                //    lblData2.InnerText = Dami.TraduCuvant(dr["CampData2"].ToString());
                //    lblData2.Style["display"] = "inline-block";
                //    txtData2.Visible = true;
                //}
                //else
                //{
                //    lblData2.InnerText = "";
                //    lblData2.Style["display"] = "none";
                //    txtData2.Visible = false;
                //    txtData2.Value = null;
                //}

                //if (!string.IsNullOrEmpty(dr["CampBifa1"].ToString()))
                //{
                //    lblBifa1.InnerText = Dami.TraduCuvant(dr["CampBifa1"].ToString());
                //    lblBifa1.Style["display"] = "inline-block";
                //    txtBifa1.Visible = true;
                //}
                //else
                //{
                //    lblBifa1.InnerText = "";
                //    lblBifa1.Style["display"] = "none";
                //    txtBifa1.Visible = false;
                //    txtBifa1.Value = null;
                //}

                //if (!string.IsNullOrEmpty(dr["CampBifa2"].ToString()))
                //{
                //    lblBifa2.InnerText = Dami.TraduCuvant(dr["CampBifa2"].ToString());
                //    lblBifa2.Style["display"] = "inline-block";
                //    txtBifa2.Visible = true;
                //}
                //else
                //{
                //    lblBifa2.InnerText = "";
                //    lblBifa2.Style["display"] = "none";
                //    txtBifa2.Visible = false;
                //    txtBifa2.Value = null;
                //}


                //if (!string.IsNullOrEmpty(dr["CampText"].ToString()))
                //{
                //    lblText.InnerText = Dami.TraduCuvant(dr["CampText"].ToString());
                //    lblText.Style["display"] = "inline-block";
                //    txtText.Visible = true;
                //}
                //else
                //{
                //    lblText.InnerText = "";
                //    lblText.Style["display"] = "none";
                //    txtText.Visible = false;
                //    txtText.Value = null;
                //}

                //if (!string.IsNullOrEmpty(dr["OreCalculateEticheta"].ToString()))
                //{
                //    lblOreCalc.InnerText = Dami.TraduCuvant(dr["OreCalculateEticheta"].ToString());
                //    lblOreCalc.Style["display"] = "inline-block";
                //    txtOreCalc.Visible = true;
                //}
                //else
                //{
                //    lblOreCalc.InnerText = "";
                //    lblOreCalc.Style["display"] = "none";
                //    txtOreCalc.Visible = false;
                //}

                //if (!string.IsNullOrEmpty(dr["OreSursaEticheta"].ToString()))
                //{
                //    lblOreSursa.InnerText = Dami.TraduCuvant(dr["OreSursaEticheta"].ToString());
                //    lblOreSursa.Style["display"] = "inline-block";
                //    txtOreSursa.Visible = true;
                //}
                //else
                //{
                //    lblOreSursa.InnerText = "";
                //    lblOreSursa.Style["display"] = "none";
                //    txtOreSursa.Visible = false;
                //}


                //if (!string.IsNullOrEmpty(dr["IdTipOre"].ToString()) && dr["IdTipOre"].ToString() == "0")
                //{
                //    lblNrOre.Style["display"] = "inline-block";
                //    txtNrOre.Visible = true;
                //}
                //else
                //{
                //    lblOreSursa.Style["display"] = "none";
                //    txtOreSursa.Visible = false;
                //    txtNrOre.Value = null;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void AfiseazaCtlExtra()
        {
            try
            {
                divDateExtra.Controls.Clear();
                divDateExtraOre.Controls.Clear();

                string ids = "";

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1 AND ""AfisareInfoChiosc"" = 1", new object[] { General.Nz((cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value) : Convert.ToInt32(cmbAbs.Value)), "-99") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    HtmlGenericControl ctlDiv = new HtmlGenericControl("div");
                    ctlDiv.Attributes["class"] = "Absente_Cereri_CampuriSup";

                    Label lbl = new Label();
                    //lbl.ID = "lblDinamic" + i;
                    lbl.Text = dr["Denumire"].ToString();
                    lbl.Style.Add("margin", "10px 0px !important");
                    lbl.Font.Size = 16;       
                    ctlDiv.Controls.Add(lbl);

                    string ctlId = "ctlDinamic" + i;
                    switch (General.Nz(dr["TipCamp"], "").ToString())
                    {
                        case "0":                   //text
                            lbl.Width = Unit.Pixel(250);
                            ASPxTextBox txt = new ASPxTextBox();
                            txt.ID = ctlId;
                            txt.ClientIDMode = ClientIDMode.Static;
                            txt.ClientInstanceName = "ctlDinamic" + i;
                            txt.Width = Unit.Pixel(200);
                            txt.Height = Unit.Pixel(75);
                            txt.Font.Size = 30;
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
                            chk.Width = Unit.Pixel(200);
                            chk.Height = Unit.Pixel(75);
                            chk.CheckedImage.Url = "../Fisiere/Imagini/Checked.png";                        
                            chk.CheckedImage.Height = Unit.Pixel(50);
                            chk.CheckedImage.Width = Unit.Pixel(50);
                            chk.UncheckedImage.Url = "../Fisiere/Imagini/Unchecked.png";
                            chk.UncheckedImage.Height = Unit.Pixel(50);
                            chk.UncheckedImage.Width = Unit.Pixel(50);
                            //chk.GrayedImage.Height = Unit.Pixel(50);
                            //chk.GrayedImage.Width = Unit.Pixel(50);                            
                            //SkinElement element = SkinManager.GetSkinElement(SkinProductId.Editors, DevExpress.LookAndFeel.UserLookAndFeel.Default, EditorsSkins.SkinCheckBox);
                            //element.Size.MinSize = new System.Drawing.Size(20, 20);
                            ////element.Image.Stretch = SkinImageStretch.Stretch;
                            //LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
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
                        case "3":                   //dateTime
                            ASPxDateEdit dte = new ASPxDateEdit();
                            dte.ID = ctlId;
                            dte.ClientIDMode = ClientIDMode.Static;
                            dte.ClientInstanceName = "ctlDinamic" + i;                         
                            dte.DisplayFormatString = "dd/MM/yyyy";
                            dte.EditFormat = EditFormat.Custom;
                            dte.EditFormatString = "dd/MM/yyyy";
                            dte.Width = Unit.Pixel(250);
                            dte.Height = Unit.Pixel(75);
                            dte.ButtonStyle.Width = 75;
                            dte.Font.Size = 24;
                            dte.CalendarProperties.DayStyle.Font.Size = 30;
                            dte.CalendarProperties.DayStyle.Paddings.Padding = Unit.Pixel(30);
                            dte.CalendarProperties.MonthGridPaddings.Padding = Unit.Pixel(30);
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
                      

                    if (Session["CereriTactil"].ToString() == "AbsenteOra" || Session["CereriTactil"].ToString() == "BiletVoie")
                        divDateExtraOre.Controls.Add(ctlDiv);
                    else
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
                string dual = "";
                if (Constante.tipBD == 2) dual = " FROM DUAL";
                DataTable dt = General.IncarcaDT(CreazaSelectCuValori() + dual, null);
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var ert = dt.Columns[i];
                        var rfv = dt.Rows[0][i];

                        if (dt.Rows[0][i].GetType() == typeof(DateTime))
                            str = str.Replace("ent." + dt.Columns[i], (dt.Rows[0][i] == null ? "null" : General.ToDataUniv((DateTime)dt.Rows[0][i])).ToString());
                        else
                            str = str.Replace("ent." + dt.Columns[i], General.Nz(dt.Rows[0][i], "null").ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return str;
        }

        private string CreazaSelectCuValori(int tip = 1, int idCerere = -99)
        {
            //tip = 1 intoarce un select
            //tip = 2 intoarce ca values; necesar pt Oracle

            string sqlCer = "";

            try
            {
                string idCircuit = "-99";
                //DataTable dtAbs = Session["Cereri_Absente_Absente"] as DataTable;


                DataTable dtAbs = new DataTable(), dtAbsente = new DataTable();
                DataRow[] dtRowAbs = null;
                //if (Session["Cereri_Absente_Absente"] != null) dtRowAbs = Session["Cereri_Absente_Absente"] as DataRow[];
                if (Session["Cereri_Absente_Absente"] != null) dtAbsente = Session["Cereri_Absente_Absente"] as DataTable;

                dtRowAbs = dtAbsente.Select();

                if (dtRowAbs != null)
                    dtAbs = dtRowAbs.CopyToDataTable();

                object id = General.Nz(cmbSelAbs.Visible == true ? cmbSelAbs.Value : cmbAbs.Value, -99);
                if (dtRowAbs != null && dtRowAbs.Count() > 0)
                {
                    dtAbs = dtRowAbs.CopyToDataTable();                    
                    if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                        id = General.Nz(cmbSelAbs.Value, -99);

                    DataRow[] lstAbs = dtAbs.Select("Id=" + id);
                    if (lstAbs.Count() == 0) return "";


                }
                else
                    return "";

                id = General.Nz(cmbSelAbs.Visible == true ? cmbSelAbs.Value : cmbAbs.Value, -99);
                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                    id = General.Nz(cmbSelAbs.Value, -99);

                DataRow[] lst = dtAbs.Select("Id=" + id);
                if (lst.Count() != 0)
                {
                    idCircuit = (lst[0]["IdCircuit"] == null ? "NULL" : lst[0]["IdCircuit"].ToString());
                }



                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                DataTable dtEx = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1 AND ""AfisareInfoChiosc"" = 1", new object[] { General.Nz((cmbSelAbs.Visible == true ? Convert.ToInt32(cmbSelAbs.Value) : Convert.ToInt32(cmbAbs.Value)), "-99") });
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    DataRow dr = dtEx.Rows[i];
                    ASPxEdit ctl = divDateExtra.FindControl("ctlDinamic" + i) as ASPxEdit;
                    if (Session["CereriTactil"].ToString() == "AbsenteOra" || Session["CereriTactil"].ToString() == "BiletVoie")
                        ctl = divDateExtraOre.FindControl("ctlDinamic" + i) as ASPxEdit;

                    if (General.Nz(dr["IdCampExtra"], "").ToString() != "")
                    {
                        if (ctl != null && ctl.Value != null && ctl.Value.ToString() != "")
                        {
                            switch (General.Nz(dr["TipCamp"], "").ToString())
                            {
                                case "0":
                                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString().Replace("'", "''") + "'";
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
                    }
                }

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

                //Radu 01.02.2021 - citire idCerere din secventa
                //int idCerere = Dami.NextId("Ptj_Cereri");
                string sqlIdCerere = idCerere.ToString();
                if (idCerere == -99) sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"")";

                string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                string sqlIdStare = $@"(SELECT {strTop} ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere})";
                string sqlPozitie = $@"(SELECT {strTop} ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere})";
                string sqlCuloare = $@"(SELECT {strTop} ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere})";
                //string sqlNrOre = txtNrOre.Text == "" ? "NULL" : txtNrOre.Text;

                string sqlOraInc = "NULL";
                string sqlOraSf = "NULL";

                if (General.Nz(cmbOraInc.Value, "").ToString() != "")
                    sqlOraInc = "'" + txtDataInc.Date.Year + "-" + txtDataInc.Date.Month + "-" + txtDataInc.Date.Day + " " + General.Nz(cmbOraInc.Value, "").ToString() + ":00'";

                if (General.Nz(cmbOraSf.Value, "").ToString() != "")
                    sqlOraSf = "'" + txtDataSf.Date.Year + "-" + txtDataSf.Date.Month + "-" + txtDataSf.Date.Day + " " + General.Nz(cmbOraSf.Value, "").ToString() + ":00'";

                if (Constante.tipBD == 2)
                {
                    sqlIdStare = $@"(SELECT * FROM ({sqlIdStare}) WHERE ROWNUM=1)";
                    sqlPozitie = $@"(SELECT * FROM ({sqlPozitie}) WHERE ROWNUM=1)";
                    sqlCuloare = $@"(SELECT * FROM ({sqlCuloare}) WHERE ROWNUM=1)";

                    if (General.Nz(cmbOraInc.Value, "").ToString() != "")
                        sqlOraInc = "TO_DATE('" + txtDataInc.Date.Day + "-" + txtDataInc.Date.Month + "-" + txtDataInc.Date.Year + " " + General.Nz(cmbOraInc.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";

                    if (General.Nz(cmbOraSf.Value, "").ToString() != "")
                        sqlOraSf = "TO_DATE('" + txtDataSf.Date.Day + "-" + txtDataSf.Date.Month + "-" + txtDataSf.Date.Year + " " + General.Nz(cmbOraSf.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";

                }

                id = General.Nz(cmbSelAbs.Visible == true ? cmbSelAbs.Value : cmbAbs.Value, -99);
                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 23)
                    id = General.Nz(cmbSelAbs.Value, -99);

                sqlCer = @"SELECT " +
                                sqlIdCerere + " AS \"Id\", " +
                                General.VarSession("User_Marca") + " AS \"F10003\", " +
                                id + " AS \"IdAbsenta\", " +
                                General.ToDataUniv(txtDataInc.Date) + " AS \"DataInceput\", " +
                                (Session["CereriTactil"].ToString() == "BiletVoie" ? General.ToDataUniv(txtDataInc.Date) : General.ToDataUniv(txtDataSf.Date)) + " AS \"DataSfarsit\", " +
                                (txtNrZile.Text.Length > 0 ? txtNrZile.Text : "NULL") + " AS \"NrZile\", " +
                                "NULL" + " AS \"NrZileViitor\", " +
                                "NULL" + " AS \"Observatii\", " +
                                (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + " AS \"IdStare\", " +
                                (idCircuit) + " AS \"IdCircuit\", " +
                                Session["UserId"] + " AS \"UserIntrod\", " +
                                (sqlCuloare == null ? "NULL" : sqlCuloare) + " AS \"Culoare\", " +
                                "NULL" + " AS \"Inlocuitor\", " +
                                (sqlTotal == null ? "NULL" : sqlTotal) + " AS \"TotalSuperCircuit\", " +
                                (sqlPozitie == null ? "NULL" : sqlPozitie) + " AS \"Pozitie\", " +
                                //trimiteLaInlocuitor + " AS \"TrimiteLa\", " +
                                " NULL AS \"TrimiteLa\", " +
                                (txtNrOre.Text.Length > 0 ? txtNrOre.Text : (txtNrOreInMinute.Text.Length > 0 ? Convert.ToDecimal(Convert.ToDecimal(txtNrOreInMinute.Text) / 60).ToString(new CultureInfo("en-US")) : "NULL")) + " AS \"NrOre\", " +
                                sqlOraInc + " AS \"OraInceput\", " +
                                sqlOraSf + " AS \"OraSfarsit\", " +
                                "NULL AS \"AreAtas\"" +
                                valExtra;
                if (tip == 2)
                    sqlCer = @"VALUES(" +
                    sqlIdCerere + ", " +
                    General.VarSession("User_Marca") + ", " +
                    id + ", " +
                    General.ToDataUniv(txtDataInc.Date) + ", " +
                    (Session["CereriTactil"].ToString() == "BiletVoie" ? General.ToDataUniv(txtDataInc.Date) : General.ToDataUniv(txtDataSf.Date)) + ", " +
                    (txtNrZile.Text.Length > 0 ? txtNrZile.Text : "NULL") + ", " +
                    "NULL" + ", " +
                    "NULL" + ", " +
                    (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + ", " +
                    (idCircuit) + ", " +
                    Session["UserId"] + ", " +
                    (sqlCuloare == null ? "NULL" : sqlCuloare) + ", " +
                    "NULL" + ", " +
                    (sqlTotal == null ? "NULL" : sqlTotal) + ", " +
                    (sqlPozitie == null ? "NULL" : sqlPozitie) + ", " +
                    " NULL, " +
                    (txtNrOre.Text.Length > 0 ? txtNrOre.Text : (txtNrOreInMinute.Text.Length > 0 ? Convert.ToDecimal(Convert.ToDecimal(txtNrOreInMinute.Text) / 60).ToString(new CultureInfo("en-US")) : "NULL")) + ", " +
                    sqlOraInc + ", " +
                    sqlOraSf + ", " +
                    "NULL" +
                    valExtra + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlCer;
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



    }
}