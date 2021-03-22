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
using System.Threading;
using DevExpress.Pdf.Native;

namespace WizOne.Absente
{
    public partial class Cereri : Page
    {

        internal class metaCereriDate
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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnIstoricExtins.Text = Dami.TraduCuvant("btnIstoricExtins", "Istoric Extins");

                lblRol.InnerText = Dami.TraduCuvant("Rol");
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblTip.InnerText = Dami.TraduCuvant("Tip Cerere");         
                lblDataInc.InnerText = Dami.TraduCuvant("Data Inceput");
                lblDataSf.InnerText = Dami.TraduCuvant("Data Sfarsit");
                lblInl.InnerText = Dami.TraduCuvant("Inlocuitor");
                lblNrZile.InnerText = Dami.TraduCuvant("Nr. zile");
                lblNrOre.InnerText = Dami.TraduCuvant("Nr. ore");
                lblObs.InnerText = Dami.TraduCuvant("Observatii");

                btnDocUpload.ToolTip = Dami.TraduCuvant("incarca document");
                btnDocSterge.ToolTip = Dami.TraduCuvant("sterge document");

                txtAbsDesc.InnerText = Dami.TraduCuvant("Fara descriere");

                //Radu 09.12.2019
                foreach (ListBoxColumn col in cmbAng.Columns)
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                #endregion

                if (!IsPostBack)
                {
                    if (Request["pp"] != null)
                        txtTitlu.Text = Dami.TraduCuvant("Prima Pagina - Cereri");
                    else
                        txtTitlu.Text = General.VarSession("Titlu").ToString();

                    txtDataInc.Date = DateTime.Now;
                    txtDataSf.Date = DateTime.Now;

                    DataTable dtAng = General.IncarcaDT(SelectAngajati(-44), null, "F10003;Rol");

                    DataView view = new DataView(dtAng);
                    DataTable dtRol = view.ToTable(true, "Rol", "RolDenumire");

                    switch (dtRol.Rows.Count)
                    {
                        case 0:
                            divRol.Visible = false;
                            cmbRol.Value = 0;
                            break;
                        case 1:
                            divRol.Visible = false;
                            cmbRol.DataSource = dtRol;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;

                            if (Convert.ToInt32(General.Nz(cmbRol.Value,0)) != 0)
                            {
                                cmbAng.Buttons.Add(new EditButton { Text = Dami.TraduCuvant("Activi") });
                                cmbAng.Buttons.Add(new EditButton { Text = Dami.TraduCuvant("Toti") });
                            }
                            break;
                        default:
                            divRol.Visible = true;
                            cmbRol.DataSource = dtRol;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;
                            cmbAng.SelectedIndex = 0;

                            cmbAng.Buttons.Add(new EditButton { Text = Dami.TraduCuvant("Activi") });
                            cmbAng.Buttons.Add(new EditButton { Text = Dami.TraduCuvant("Toti") });

                            break;
                    }

                    //Incarcam Angajatii in functie de rol
                    DataTable dtAngFiltrati = dtAng;
                    if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != -44 && dtAng != null && dtAng.Rows.Count > 0) dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();

                    DataTable dtAngActivi = new DataTable();
                    if (dtAngFiltrati != null && dtAngFiltrati.Rows.Count > 0 && dtAngFiltrati.Select("AngajatActiv=1").Count() > 0) dtAngActivi = dtAngFiltrati.Select("AngajatActiv=1").CopyToDataTable();
                    cmbAng.DataSource = dtAngActivi;
                    Session["Cereri_Absente_Angajati"] = dtAngActivi;
                    Session["Cereri_Absente_AngajatiToti"] = dtAngFiltrati;
                    cmbAng.DataBind();
                    if ((dtRol.Rows.Count == 0 || dtRol.Rows.Count == 1) && General.VarSession("User_Marca").ToString() != "-99") cmbAng.Value = General.VarSession("User_Marca");

                    if (General.VarSession("IstoricExtins_VineDin").ToString() == "2-OK" && Session["Absente_Cereri_Date"] != null)
                    {
                        metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                        cmbAng.Value = itm.Angajat;
                        txtDataInc.Value = itm.DataInceput;
                        txtDataSf.Value = itm.DataSfarsit;
                    }

                    //Incarcam Absentele
                    DataTable dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value,-99).ToString(), Convert.ToDateTime(txtDataInc.Value ?? DateTime.Now.Date),-99, Convert.ToInt32(cmbRol.Value ?? -99)), null);
                    cmbAbs.DataSource = dtAbs;
                    cmbAbs.DataBind();
                    Session["Cereri_Absente_Absente"] = dtAbs;

                    //Incarcam Inlocuitorii
                    DataTable dtInl = General.IncarcaDT(General.SelectInlocuitori(Convert.ToInt32(General.Nz(cmbAng.Value,-99)), txtDataInc.Date, txtDataSf.Date), null);
                    cmbInloc.DataSource = dtInl;
                    cmbInloc.DataBind();

                    //Populam campurile
                    if (General.VarSession("IstoricExtins_VineDin").ToString() == "2-OK" && Session["Absente_Cereri_Date"] != null)
                    {
                        metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                        cmbAbs.Value = itm.Absenta;
                        cmbInloc.Value = itm.Inlocuitor;
                        txtNrZile.Value = itm.NrZile;
                        txtNrOre.Value = itm.NrOre;
                        txtNrZileViitor.Value = itm.NrZileViitor;
                        txtObs.Value = itm.Obs;
                        lblDoc.InnerText = General.Nz(itm.UploadedFileName,"").ToString();

                        //este necesar pt cand se intoarce din IstoricExtins
                        AfiseazaCtl();
                    }
                }
                else
                {
                    if (IsCallback)
                    {
                        cmbAng.DataSource = null;
                        cmbAng.Items.Clear();
                        cmbAng.DataSource = Session["Cereri_Absente_Angajati"];
                        cmbAng.DataBind();

                        DataTable dtInl = General.IncarcaDT(General.SelectInlocuitori(Convert.ToInt32(General.Nz(cmbAng.Value, "-99")), txtDataInc.Date, txtDataSf.Date), null);
                        cmbInloc.DataSource = dtInl;
                        cmbInloc.DataBind();

                        if (Session["Absente_Cereri_Date_Aditionale"] != null) AfiseazaCtl();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnIstoricExtins_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAng.Value == null)
                {
                    Session["IstoricExtins_Angajat_Marca"] = Session["User_Marca"];
                    Session["IstoricExtins_Angajat_Nume"] = Session["User_NumeComplet"];
                }
                else
                {
                    Session["IstoricExtins_Angajat_Marca"] = cmbAng.Value;
                    Session["IstoricExtins_Angajat_Nume"] = cmbAng.Text;
                }

                Session["IstoricExtins_VineDin"] = 2;
                Response.Redirect("~/Absente/IstoricExtins.aspx", false);
                
                metaCereriDate itm = new metaCereriDate();
                if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                itm.Angajat = cmbAng.Value;
                itm.Absenta = cmbAbs.Value;
                itm.DataInceput = txtDataInc.Value;
                itm.DataSfarsit = txtDataSf.Value;
                itm.Inlocuitor = cmbInloc.Value;
                itm.NrZile = txtNrZile.Value;
                itm.NrOre = txtNrOre.Value;
                itm.NrZileViitor = txtNrZileViitor.Value;
                itm.Obs = txtObs.Value;
                Session["Absente_Cereri_Date"] = itm;
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

                //Florin 2020-05-27 - am adaugat filtrul la F100Supervizori
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
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003 AND B.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= B.""DataSfarsit""
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

        protected void cmbAng_ItemsRequestedByFilterCondition(object source, DevExpress.Web.ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = (ASPxComboBox)source;
                DataTable dt = General.IncarcaDT(SelectAngajati(Convert.ToInt32(cmbRol.Value ?? -99)) + " WHERE (A.F10008 + ' ' + A.F10009) LIKE @1",new object[] {  e.Filter + "%" } );
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
                DataTable dt = General.IncarcaDT(SelectAngajati(Convert.ToInt32(cmbRol.Value ?? -99)) + " WHERE A.F10003 = @1", new object[] { e.Value });
                cmb.DataSource = dt;
                cmb.DataBind();
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
                string tip = e.Parameter;
                DataTable dtAbs = new DataTable();

                if (tip == "3" && txtDataInc.Value != null) txtDataSf.Value = txtDataInc.Value;

                if (Session["Cereri_Absente_Absente"] != null) dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                if (dtAbs.Rows.Count > 0)
                {
                    DataRow[] arr = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                    if (arr.Count() > 0)
                    {
                        //Afisam explicatiile
                        txtAbsDesc.InnerText = General.Nz(arr[0]["Explicatii"], "").ToString();
                        txtNrZile.Value = General.CalcZile(Convert.ToInt32(cmbAng.Value ?? 0), Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), Convert.ToInt32(cmbRol.Value ?? 0), Convert.ToInt32(cmbAbs.Value ?? 0));
                    }
                }

                cmbAbs.DataSource = dtAbs;
                cmbAbs.DataBind();

                switch (tip)
                {
                    case "1":               //cmbAng
                        {
                            dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString(), Convert.ToDateTime(txtDataInc.Value ?? DateTime.Now.Date), -99, Convert.ToInt32(cmbRol.Value ?? -99)), null);
                            cmbAbs.DataSource = dtAbs;
                            cmbAbs.DataBind();
                            cmbAbs.SelectedIndex = -1;
                            Session["Cereri_Absente_Absente"] = dtAbs;
                            AfiseazaCtl();
                        }
                        break;
                    case "2":               //cmbAbs
                        AfiseazaCtl();
                        break;
                    case "3":               //DataInceput
                        {
                            dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString(), Convert.ToDateTime(txtDataInc.Value ?? DateTime.Now.Date), -99, Convert.ToInt32(cmbRol.Value ?? -99)), null);
                            cmbAbs.DataSource = dtAbs;
                            cmbAbs.DataBind();
                            bool gasit = false;
                            if (cmbAbs.Value != null)
                            {
                                for (int k = 0; k < dtAbs.Rows.Count; k++)
                                    if (Convert.ToInt32(cmbAbs.Value) == Convert.ToInt32(dtAbs.Rows[k]["Id"].ToString()))
                                    {
                                        gasit = true;
                                        break;
                                    }
                            }
                            if (!gasit)
                                cmbAbs.SelectedIndex = -1;
                            Session["Cereri_Absente_Absente"] = dtAbs;


                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                            lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                            //Florin 2019.01.11
                            AfiseazaCtl();
                        }
                        break;
                    case "4":               //btnSave
                        {
                            SalveazaDate();
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
                            lblDoc.InnerHtml = "&nbsp;";
                        }
                        break;
                    case "6":                   //DataSfarsit
                        {
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                            lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                            //Florin 2019.01.11
                            AfiseazaCtl();
                        }
                        break;
                    case "7":                   //cmbRol
                        DataTable dtAng = General.IncarcaDT(SelectAngajati(Convert.ToInt32(cmbRol.Value ?? -99)), null);

                        DataTable dtAngFiltrati = dtAng;
                        if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != -44 && dtAng != null && dtAng.Rows.Count > 0) dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();

                        DataTable dtAngActivi = new DataTable();
                        if (dtAngFiltrati != null && dtAngFiltrati.Rows.Count > 0 && dtAngFiltrati.Select("AngajatActiv=1").Count() > 0) dtAngActivi = dtAngFiltrati.Select("AngajatActiv=1").CopyToDataTable();
                        cmbAng.DataSource = dtAngActivi;
                        Session["Cereri_Absente_Angajati"] = dtAngActivi;
                        Session["Cereri_Absente_AngajatiToti"] = dtAngFiltrati;
                        //cmbAng.DataSource = dtAng;
                        //Session["Cereri_Absente_Angajati"] = dtAng;
                        //Session["Cereri_Absente_AngajatiToti"] = dtAng;
                        cmbAng.DataBind();
                        cmbAng.SelectedIndex = 0;


                        //acelasi cod ca la case "1"
                        dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString(), Convert.ToDateTime(txtDataInc.Value ?? DateTime.Now.Date), -99, Convert.ToInt32(cmbRol.Value ?? -99)), null);
                        cmbAbs.DataSource = dtAbs;
                        cmbAbs.DataBind();
                        cmbAbs.SelectedIndex = -1;
                        Session["Cereri_Absente_Absente"] = dtAbs;
                        AfiseazaCtl();

                        break;
                    case "8":
                        AfiseazaCtl();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                metaCereriDate itm = new metaCereriDate();
                if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["Absente_Cereri_Date"] = itm;
                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocSterge_Click(object sender, EventArgs e)
        {
            try
            {
                metaCereriDate itm = new metaCereriDate();
                if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;

                itm.UploadedFile = null;
                itm.UploadedFileName = null;
                itm.UploadedFileExtension = null;
                Session["Absente_Cereri_Date"] = itm;
                lblDoc.InnerHtml = "&nbsp;";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SalveazaDate()
        {
            try
            {
                string strErr = "";

                if (cmbAng.Value == null) strErr += ", " + Dami.TraduCuvant("angajat");
                if (cmbAbs.Value == null) strErr += ", " + Dami.TraduCuvant("tip absenta");
                if (txtDataInc.Text == "") strErr += ", " + Dami.TraduCuvant("data inceput");
                if (txtDataSf.Text == "") strErr += ", " + Dami.TraduCuvant("data sfarsit");

                //Florin 2019.09.25
                if (cmbOraInc.Visible == true && cmbOraInc.Text == "") strErr += ", " + Dami.TraduCuvant("ora inceput");
                if (cmbOraSf.Visible == true && cmbOraSf.Text == "") strErr += ", " + Dami.TraduCuvant("ora sfarsit");

                if (strErr != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);
                    return;
                }

                DataTable dtAbs = new DataTable();
                if (Session["Cereri_Absente_Absente"] != null) dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                DataRow[] lst = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                if (lst.Count() == 0) return;

                DataRow drAbs = lst[0];

                metaCereriDate itmAta = new metaCereriDate();
                if (Session["Absente_Cereri_Date"] != null) itmAta = Session["Absente_Cereri_Date"] as metaCereriDate;

                if (Convert.ToInt32(General.Nz(drAbs["ArataAtasament"], 0)) == (int)Constante.ArataControl.Obligatoriu && itmAta.UploadedFile == null) strErr += ", " + Dami.TraduCuvant("atasamentul");
                if (Convert.ToInt32(General.Nz(drAbs["ArataInlocuitor"], 0)) == (int)Constante.ArataControl.Obligatoriu && cmbInloc.Value == null) strErr += ", " + Dami.TraduCuvant("inlocuitorul");
                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && txtNrOre.Value == null) strErr += ", " + Dami.TraduCuvant("nr. ore");

                if (strErr != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);
                    return;
                }


                #region Verificare validitate angajat

                try
                {
                    int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={cmbAng.Value} AND F10022 <= {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value))} <= F10023", null), 0));
                    if (esteActiv == 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In perioada solicitata, angajatul este inactiv");
                        return;
                    }
                }
                catch (Exception) { }

                #endregion

                #region Verificare validitate inlocuitor
                //Radu 21.05.2019
                try
                {
                    if (cmbInloc.Value != null)
                    {
                        int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={cmbInloc.Value} AND F10022 <= {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value))} <= F10023", null), 0));
                        if (esteActiv == 0)
                        {
                            string campData = "CONVERT(VARCHAR, F10023, 103)";
                            if (Constante.tipBD == 2)
                                campData = "TO_CHAR(F10023, 'dd/mm/yyyy')";
                            string data = General.Nz(General.ExecutaScalar($@"SELECT {campData} AS F10023 FROM F100 WHERE F10003={cmbInloc.Value}", null), "").ToString();
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In perioada solicitata, inlocuitorul este inactiv (ultima zi lucrata: " + data + ")");
                            return;
                        }
                    }
                }
                catch (Exception) { }

                #endregion  



                #region Verificare Campuri Extra

                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                DataTable dtEx = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1", new object[] { cmbAbs.Value });
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    DataRow dr = dtEx.Rows[i];
                    ASPxEdit ctl = divDateExtra.FindControl("ctlDinamic" + i) as ASPxEdit;
                    if (Convert.ToInt32(General.Nz(dr["Obligatoriu"], 0)) == 1)
                    {
                        if (ctl == null || ctl.Value == null || ctl.Value.ToString() == "") strErr += ", " + General.Nz(dr["Denumire"], "date extra").ToString();
                    }
                }

                if (strErr != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);
                    return;
                }


                #endregion


                #region verif client

                if (txtDataSf.Date < txtDataInc.Date) strErr += " " + Dami.TraduCuvant("Data sfarsit este mai mica decat data inceput");


                //daca abs este de tip ore dtinc si datasf trebuie sa fie aceeasi

                //Florin 2019.09.13
                //s-a adaugat filtrul cu prezenta
                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && Convert.ToInt32(General.Nz(drAbs["Prezenta"], 1)) == 0 && txtDataInc.Date != txtDataSf.Date)
                    strErr += " " + Dami.TraduCuvant("Data inceput si data sfarsit trebuie sa fie aceeasi in cazul acestui tip de absenta");

                if (Convert.ToInt32(General.Nz(txtNrZile.Value, -99)) <= 0) strErr += " " + Dami.TraduCuvant("Cerere cu numar de zile 0");

                if (strErr != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = strErr;
                    return;
                }

                if (Convert.ToInt32(General.Nz(cmbAng.Value, -98)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -97)) && Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { cmbAng.Value })) == 0)
                {
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
                                WHERE A.F10003 = {cmbAng.Value} AND A.""DataInceput"" <= {General.ToDataUniv(txtDataSf.Date)} AND {General.ToDataUniv(txtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND A.""IdStare"" IN (1,2,3,4) AND B.""GrupOre"" IN({General.Nz(drAbs["GrupOreDeVerificat"], -99)})", null));

                if (intersec > 0)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Intervalul se intersecteaza cu altul deja existent");
                    return;
                }

                //verificam ca inlocuitorul sa nu aiba o cerere in aceeasi perioada, at. cand inlocuitorul nu este ascuns, este ori obligatoriu ori optional
                if (Convert.ToInt32(General.Nz(cmbInloc.Value, -99)) != -99 && Convert.ToInt32(General.Nz(drAbs["ArataInlocuitor"], 1)) != (int)Constante.ArataControl.Ascuns && Convert.ToInt32(General.Nz(drAbs["VerificaCereriInlocuitor"], 0)) == 1)
                {
                    int inlocIts = Convert.ToInt32(General.ExecutaScalar($@"
                                SELECT COUNT(*)
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_tblAbsente"" B ON A.""IdAbsenta"" = B.""Id""
                                WHERE A.F10003 = {General.Nz(cmbInloc.Value, -99)} AND A.""DataInceput"" <= {General.ToDataUniv(txtDataSf.Date)} 
                                AND {General.ToDataUniv(txtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND a.""IdStare"" IN (1,2,3) AND b.""Prezenta"" = 0", null));

                    if (inlocIts > 0)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Inlocuitorul are cerere in aceasta perioada");
                        return;
                    }
                }


                //verificam ca angajatul sa nu fie desemnat inlocuitor pt alte persoane daca isi face cerere pt absenta de tip "absenta"
                if (Convert.ToInt32(General.Nz(drAbs["Prezenta"], 0)) == 0 && Convert.ToInt32(Dami.ValoareParam("PermisiuneCerereInlocuitor", "1") ?? "1") == 0)
                {
                    string inlocOre = General.Nz((General.ExecutaScalar($@"
                                SELECT B.F10008 + ' ' + B.F10009
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN F100 B ON A.F10003 = B.F10003
                                WHERE A.""Inlocuitor"" = {cmbAng.Value} AND A.""DataInceput"" <= {General.ToDataUniv(txtDataSf.Date)} 
                                AND {General.ToDataUniv(txtDataInc.Date)} <= A.""DataSfarsit"" AND a.""IdStare"" IN (1,2,3)", null)), "").ToString();

                    if (inlocOre != "")
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul este desemnat inlocuitor pentru " + inlocOre);
                        return;
                    }
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
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi sa creati acest tip de absenta");
                    return;
                }

                DateTime ziDrp = Dami.DataDrepturi(valoare, nrZile, Convert.ToDateTime(txtDataInc.Value), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbRol.Value ?? -99));
                if (txtDataInc.Date < ziDrp.Date)
                {
                    if (valoare == 13)
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pontajul a fost aprobat pentru aceasta perioada");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat " + ziDrp.Date.ToShortDateString());

                    return;
                }

                //Radu 13.03.2020 - verificare ca idAbsenta sa fie valabil si la DataSfarsit
                DataTable dtAbsVerif = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString(), Convert.ToDateTime(txtDataSf.Value ?? DateTime.Now.Date), -99, Convert.ToInt32(cmbRol.Value ?? -99)), null);
                bool eroare = true;
                if (dtAbsVerif != null && dtAbsVerif.Rows.Count > 0)
                    for (int k = 0; k < dtAbsVerif.Rows.Count; k++)
                        if (Convert.ToInt32(dtAbsVerif.Rows[k]["Id"].ToString()) == Convert.ToInt32(cmbAbs.Value))
                        {
                            eroare = false;
                            break;
                        }
                if (eroare)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Acest tip de cerere nu este disponibil pe intreaga durata a solicitarii!");
                    return;
                }

                #endregion


                #region Verificam sa nu depaseasca norma


                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0)
                {
                    //ce are voie sa introduca
                    DataRow dtCtr = General.IncarcaDR($@"
                            SELECT A.*
                            FROM ""Ptj_ContracteAbsente"" A
                            INNER JOIN ""F100Contracte"" B ON A.""IdContract"" = B.""IdContract""
                            WHERE A.""IdAbsenta"" = {Convert.ToInt32(cmbAbs.Value)} AND B.F10003 = {Convert.ToInt32(cmbAng.Value)} 
                            AND B.""DataInceput"" <= {General.ToDataUniv(txtDataInc.Date)} AND {General.ToDataUniv(txtDataInc.Date)} <= B.""DataSfarsit""", null);

                    if (dtCtr != null)
                    {
                        int esteSL = 0;
                        int esteS = 0;
                        int esteD = 0;
                        int esteZL = 1;

                        esteSL = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = " + General.ToDataUniv(txtDataInc.Date)) ?? 0);
                        if (txtDataInc.Date.DayOfWeek == DayOfWeek.Saturday) esteS = 1;
                        if (txtDataInc.Date.DayOfWeek == DayOfWeek.Sunday) esteD = 1;
                        if (esteSL == 1 || esteS == 1 || esteD == 1) esteZL = 0;

                        if ((esteSL != 0 && Convert.ToInt32(General.Nz(dtCtr["SL"], 0)) == 1) || (esteS != 0 && Convert.ToInt32(General.Nz(dtCtr["S"], 0)) == 1) || (esteD != 0 && Convert.ToInt32(General.Nz(dtCtr["D"], 0)) == 1) || (esteZL != 0 && Convert.ToInt32(General.Nz(dtCtr["ZL"], 0)) == 1))
                        {
                            //are voie sa ponteze aceasta absenta
                        }
                        else
                        {
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Contractul nu permite aceasta absenta");
                            return;
                        }

                        if (Convert.ToInt32(General.Nz(drAbs["VerificareNrMaxOre"], 0)) == 1)
                        {
                            string msgNr = General.VerificareDepasireNorma(Convert.ToInt32(cmbAng.Value), txtDataInc.Date, Convert.ToInt32(txtNrOre.Value ?? 0) * 60, 1);
                            if (msgNr != "")
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msgNr);
                                return;
                            }
                        }

                    }
                }

                #endregion

                #region Validare Max Ore

                int cv = Convert.ToInt32(Dami.ValoareParam("ZileCuveniteInAvans", "0") ?? "0");

                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0)
                {
                    //daca este absenta de tip ora 

                    //verificam NrMax pe cerere
                    if (Convert.ToDecimal(General.Nz(drAbs["NrMax"], 999)) < Convert.ToDecimal(txtNrOre.Value))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de" + " " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " " + Dami.TraduCuvant("ore"));
                        return;
                    }

                    //Florin 2020.05.27
                    //verificam nr max pe an
                    string sqlAn = $@"SELECT COALESCE(SUM(COALESCE(""NrOre"",0)),0) AS ""OreAn"" FROM ""Ptj_Cereri"" WHERE F10003=@1 AND {General.FunctiiData("\"DataInceput\"", "A")}=@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                    DataRow drAn = General.IncarcaDR(sqlAn, new object[] { Convert.ToInt32(cmbAng.Value), txtDataInc.Date.Year, Convert.ToInt32(cmbAbs.Value) });

                    if (drAn != null && drAbs[0] != null && drAbs["NrMaxAn"] != DBNull.Value && (Convert.ToDecimal(General.Nz(drAn[0], 0)) + Convert.ToDecimal(txtNrOre.Value)) > Convert.ToDecimal(drAbs["NrMaxAn"]))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr total de ore depaseste nr maxim de ore cuvenite in an");
                        return;
                    }
                }
                else
                {
                    //daca este absenta de tip zi

                    //verificam nr max pe cerere
                    //folosim campul txtNrZile pt ca este deja calculat cu nr de zile in functie de parametrul AdunaZileLibere
                    if (txtNrZile.Value != null && drAbs["NrMax"] != null && Convert.ToInt32(txtNrZile.Value) > Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite");
                        return;
                    }


                    //verificam nr max pe an
                    string sqlAn = $@"SELECT COALESCE(SUM(COALESCE(""NrZile"",0)),0) AS ""ZileAn"" FROM ""Ptj_Cereri"" WHERE F10003=@1 AND {General.FunctiiData("\"DataInceput\"", "A")}=@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                    DataRow drAn = General.IncarcaDR(sqlAn, new object[] { Convert.ToInt32(cmbAng.Value), txtDataInc.Date.Year, Convert.ToInt32(cmbAbs.Value) });

                    if (drAn != null && drAbs[0] != null && drAbs["NrMaxAn"] != DBNull.Value && (Convert.ToInt32(General.Nz(drAn[0], 0)) + Convert.ToInt32(txtNrZile.Value)) > Convert.ToInt32(drAbs["NrMaxAn"]))
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an");
                        return;
                    }
                }

                #endregion

                #region Salvare in baza

                #region Construim istoricul

                //Radu 01.02.2021 - citire idCerere din secventa
                int idCerere = Dami.NextId("Ptj_Cereri");

                string sqlIst;
                int trimiteLaInlocuitor;
                General.SelectCereriIstoric(Convert.ToInt32(cmbAng.Value), Convert.ToInt32(General.Nz(cmbInloc.Value, -1)), Convert.ToInt32(drAbs["IdCircuit"]), Convert.ToInt32(General.Nz(drAbs["EstePlanificare"], 0)), out sqlIst, out trimiteLaInlocuitor, idCerere, txtDataInc.Date);

                #endregion

                //Florin 2019.10.17 - am mutat sqlCer in afara if-ului pt ca trebuie in modulul de notificari indiferent daca este sql sau oracle
                string sqlCer = CreazaSelectCuValori(1, idCerere);
                string sqlPre = "";
                string strGen = "";

                if (Constante.tipBD == 1)
                {
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"", USER_NO, TIME, ""IdCerereDivizata"", ""Comentarii"", ""CampBifa"") 
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
                    sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""OraInceput"", ""OraSfarsit"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"", USER_NO, TIME, ""IdCerereDivizata"", ""Comentarii"", ""CampBifa"") ";

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

                if (Dami.ValoareParam("LogCereri", "0") == "1") General.CreazaLogCereri(strGen, cmbAng.Value.ToString(), txtDataInc.Value.ToString());

                string msg = Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", 1 AS \"Actiune\", 1 AS \"IdStareViitoare\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
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
                            if(lstOut.Count == 2)
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


                    #region Adaugare atasament

                    if (Session["Absente_Cereri_Date"] != null)
                    {
                        metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                        if (itm.UploadedFile != null)
                        {
                            string sqlFis = $@"INSERT INTO ""tblFisiere""(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                            General.ExecutaNonQuery(sqlFis, new object[] { "Ptj_Cereri", idCer, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                        }
                    }

                    #endregion
                    

                    string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        NotifAsync.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" Z WHERE ""Id""=" + idCer, "Ptj_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), arrParam);
                    });


                    #region  Pontaj start

                    //completeaza soldul de ZL; Este numai pt clientul Groupama
                    General.SituatieZLOperatii(Convert.ToInt32(cmbAng.Value ?? -99), txtDataInc.Date, 2, Convert.ToInt32(txtNrZile.Value));

                    //trimite in pontaj daca este finalizat
                    if (idStare == 3)
                    {
                        if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                            General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), idCer, 5, trimiteLaInlocuitor, Convert.ToDecimal(txtNrOre.Value ?? 0));

                        if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 && Dami.ValoareParam("PontajCCStergeDacaAbsentaDeTipZi") == "1")
                            General.ExecutaNonQuery($@"DELETE FROM ""Ptj_CC"" WHERE F10003={Convert.ToInt32(cmbAng.Value)} AND {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Text))} <= ""Ziua"" AND ""Ziua"" <= {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Text))} ", null);

                        General.CalculFormule(cmbAng.Value, null, txtDataInc.Date, txtDataSf.Date);
                        //General.ExecValStr($@"F10003={cmbAng.Value} AND {General.ToDataUniv(txtDataInc.Date)} <= ""Ziua"" AND ""Ziua"" <= {General.ToDataUniv(txtDataSf.Date)}");
                    }

                    #endregion
                }

                #endregion


                Session["Absente_Cereri_Date_Aditionale"] = null;

                if (msg != "" && msg.Substring(0, 1) != "2")
                {
                    pnlCtl.JSProperties["cp_InfoMessage"] = Dami.TraduCuvant("Proces realizat cu succes, dar cu urmatorul avertisment") + ": " + Dami.TraduCuvant(msg);
                }
                else
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
                    if (Dami.ValoareParam("IesireDinCereri") == "1")
                        ASPxPanel.RedirectOnCallback("~/Absente/Lista.aspx");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                
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
                int folosesteInterval = 0;
                int perioada = 0;

                if (dtAbs != null && dtAbs.Rows.Count > 0)
                {
                    DataRow[] arr = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, "-99"));
                    if (arr.Count() > 0)
                    {
                        DataRow dr = arr[0];

                        arataInloc = General.Nz(dr["ArataInlocuitor"], "1").ToString();
                        arataAtas = General.Nz(dr["ArataAtasament"], "1").ToString();
                        idOre = General.Nz(dr["IdTipOre"], "1").ToString();
                        folosesteInterval = Convert.ToInt32(General.Nz(dr["AbsentaTipOraFolosesteInterval"], 0));
                        perioada = Convert.ToInt32(General.Nz(dr["AbsentaTipOraPerioada"], 0));
                        if (perioada <= 0) perioada = 60;
                        if (perioada > 60) perioada = 60;
                    }
                }

                if (arataInloc == "2" || arataInloc == "3")
                {
                    lblInl.Style["display"] = "inline-block";
                    cmbInloc.Visible = true;
                }
                else
                {
                    lblInl.Style["display"] = "none";
                    cmbInloc.Visible = false;
                }

                if (arataAtas == "2" || arataAtas == "3")
                {
                    btnDocUpload.Visible = true;
                    btnDocSterge.Visible = true;
                }
                else
                {
                    btnDocUpload.Visible = false;
                    btnDocSterge.Visible = false;
                }

                if (idOre == "0")
                {
                    lblNrOre.Style["display"] = "inline-block";
                    txtNrOre.ClientVisible = true;
                    txtNrOre.DecimalPlaces = 0;
                    txtNrOre.NumberType = SpinEditNumberType.Integer;

                    if (folosesteInterval == 1)
                    {
                        List<Module.Dami.metaGeneral2> lst = ListaInterval(perioada);

                        lblOraInc.Style["display"] = "inline-block";
                        cmbOraInc.Visible = true;
                        cmbOraInc.DataSource = lst;
                        cmbOraInc.DataBind();

                        lblOraSf.Style["display"] = "inline-block";
                        cmbOraSf.Visible = true;
                        cmbOraSf.DataSource = lst;
                        cmbOraSf.DataBind();

                        txtNrOre.ClientEnabled = false;
                        txtNrOre.DecimalPlaces = 4;
                        txtNrOre.NumberType = SpinEditNumberType.Float;
                        txtNrOre.ClientVisible = false;

                        txtNrOreTime.ClientVisible = true;

                        lblNrOre.InnerText = Dami.TraduCuvant("Nr. ore");
                    }
                }
                else
                {
                    lblNrOre.Style["display"] = "none";
                    txtNrOre.ClientVisible = false;
                    txtNrOre.Value = null;

                    lblOraInc.Style["display"] = "none";
                    cmbOraInc.Visible = false;
                    cmbOraInc.Value = null;

                    lblOraSf.Style["display"] = "none";
                    cmbOraSf.Visible = false;
                    cmbOraSf.Value = null;
                }

                AfiseazaCtlExtra();
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

                string ids = "";

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1", new object[] { General.Nz(cmbAbs.Value,"-99") });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dt.Rows[i];

                    HtmlGenericControl ctlDiv = new HtmlGenericControl("div");
                    ctlDiv.Attributes["class"] = "Absente_Cereri_CampuriSup";

                    Label lbl = new Label();
                    lbl.Text = Dami.TraduCuvant(dr["Denumire"].ToString());
                    lbl.Style.Add("margin", "10px 0px !important");
                    ctlDiv.Controls.Add(lbl);

                    string ctlId = "ctlDinamic" + i;
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
                            cmb.Width = Unit.Pixel(200);
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

        public string CreazaSelectCuValori(int tip = 1, int idCerere = -99)
        {
            //tip = 1 intoarce un select
            //tip = 2 intoarce ca values; necesar pt Oracle

            string sqlCer = "";

            try
            {
                string idCircuit = "-99";
                DataTable dtAbs = Session["Cereri_Absente_Absente"] as DataTable;
                DataRow[] lst = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value, -99));
                if (lst.Count() != 0)
                {
                    idCircuit = (lst[0]["IdCircuit"] == null ? "NULL" : lst[0]["IdCircuit"].ToString());
                }

                string[] lstExtra = new string[20] { "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };

                DataTable dtEx = General.IncarcaDT(@"SELECT * FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=@1", new object[] { cmbAbs.Value });
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    DataRow dr = dtEx.Rows[i];
                    ASPxEdit ctl = divDateExtra.FindControl("ctlDinamic" + i) as ASPxEdit;
                    if (General.Nz(dr["IdCampExtra"], "").ToString() != "")
                    {
                        if (ctl != null && ctl.Value != null && ctl.Value.ToString() != "")
                        {
                            switch (General.Nz(dr["TipCamp"], "").ToString())
                            {
                                case "0":
                                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString().Replace("'","''") + "'";
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


                //verificam daca are atasament
                int areAtas = 0;
                if (Session["Absente_Cereri_Date"] != null)
                {
                    metaCereriDate itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                    if (itm.UploadedFile != null) areAtas = 1;
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

                //string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                //Radu 01.02.2021
                string sqlIdCerere = idCerere.ToString();
                if (idCerere == -99) sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"")";

                string sqlInloc = cmbInloc.Value == null ? "NULL" : cmbInloc.Value.ToString();
                string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                string sqlIdStare = $@"(SELECT {strTop} ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlPozitie = $@"(SELECT {strTop} ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlCuloare = $@"(SELECT {strTop} ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
                string sqlNrOre = txtNrOre.Text == "" ? "NULL" : txtNrOre.Text.Replace(",",".");

                //Florin 2019.09.13
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
                    //dual = " FROM DUAL";


                    //Florin 2019.09.13
                    if (General.Nz(cmbOraInc.Value, "").ToString() != "")
                        sqlOraInc = "TO_DATE('" +  txtDataInc.Date.Day + "-" + txtDataInc.Date.Month + "-" + txtDataInc.Date.Year + " " + General.Nz(cmbOraInc.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";

                    if (General.Nz(cmbOraSf.Value, "").ToString() != "")
                        sqlOraSf = "TO_DATE('" + txtDataSf.Date.Day + "-" + txtDataSf.Date.Month + "-" + txtDataSf.Date.Year + " " + General.Nz(cmbOraSf.Value, "").ToString() + ":00','DD-MM-YYYY HH24:MI:SS')";
                }

                sqlCer = @"SELECT " +
                                sqlIdCerere + " AS \"Id\", " +
                                cmbAng.Value + " AS \"F10003\", " +
                                cmbAbs.Value + " AS \"IdAbsenta\", " +
                                General.ToDataUniv(txtDataInc.Date) + " AS \"DataInceput\", " +
                                General.ToDataUniv(txtDataSf.Date) + " AS \"DataSfarsit\", " +
                                (txtNrZile.Value == null ? "NULL" : txtNrZile.Value.ToString()) + " AS \"NrZile\", " +
                                (txtNrZileViitor.Value == null ? "NULL" : txtNrZileViitor.Value.ToString()) + " AS \"NrZileViitor\", " +
                                (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString().Replace("'","''") + "'") + " AS \"Observatii\", " +
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
                                sqlOraInc + " AS \"OraInceput\", " +
                                sqlOraSf + " AS \"OraSfarsit\", " +
                                areAtas + " AS \"AreAtas\"" +
                                valExtra + ", " + Session["UserId"] + " AS USER_NO, " + General.CurrentDate() + " AS TIME, null AS \"IdCerereDivizata\", null AS \"Comentarii\", 0 AS \"CampBifa\"";
                if (tip == 2)
                    sqlCer = @"VALUES(" +
                    sqlIdCerere + ", " +
                    cmbAng.Value + ", " +
                    cmbAbs.Value + ", " +
                    General.ToDataUniv(txtDataInc.Date) + ", " +
                    General.ToDataUniv(txtDataSf.Date) + ", " +
                    (txtNrZile.Value == null ? "NULL" : txtNrZile.Value.ToString()) + ", " +
                    (txtNrZileViitor.Value == null ? "NULL" : txtNrZileViitor.Value.ToString()) + ", " +
                    (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString().Replace("'","''") + "'") + ", " +
                    (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + ", " +
                    (idCircuit) + ", " +
                    Session["UserId"] + ", " +
                    (sqlCuloare == null ? "NULL" : sqlCuloare) + ", " +
                    (sqlInloc == null ? "NULL" : sqlInloc) + ", " +
                    (sqlTotal == null ? "NULL" : sqlTotal) + ", " +
                    (sqlPozitie == null ? "NULL" : sqlPozitie) + ", " +
                    " NULL, " +
                    (sqlNrOre == null ? "NULL" : sqlNrOre) + ", " +
                    sqlOraInc + ", " +
                    sqlOraSf + ", " +
                    areAtas + "" +
                    valExtra + ", " + Session["UserId"] + ", " + General.CurrentDate() + ", null, null, 0)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlCer;
        }

        protected void cmbAng_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter == "Activi")
                {
                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();

                    DataTable dtAng = Session["Cereri_Absente_AngajatiToti"] as DataTable;
                    DataTable dtAngFiltrati = dtAng.Select("AngajatActiv=1").CopyToDataTable();

                    cmbAng.DataSource = dtAngFiltrati;
                    Session["Cereri_Absente_Angajati"] = dtAngFiltrati;
                    cmbAng.DataBind();
                }
                if (e.Parameter == "Toti")
                {
                    cmbAng.DataSource = null;
                    cmbAng.Items.Clear();

                    DataTable dtAng = Session["Cereri_Absente_AngajatiToti"] as DataTable;

                    cmbAng.DataSource = dtAng;
                    Session["Cereri_Absente_Angajati"] = dtAng;
                    cmbAng.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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


    }
}