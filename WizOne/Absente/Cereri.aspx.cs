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

namespace WizOne.Absente
{
    public partial class Cereri : System.Web.UI.Page
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

                #endregion

                if (!IsPostBack)
                {
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
                                cmbAng.Buttons.Add(new EditButton { Text = "Activi" });
                                cmbAng.Buttons.Add(new EditButton { Text = "Toti" });
                            }
                            break;
                        default:
                            divRol.Visible = true;
                            cmbRol.DataSource = dtRol;
                            cmbRol.DataBind();
                            cmbRol.SelectedIndex = 0;
                            cmbAng.SelectedIndex = 0;

                            cmbAng.Buttons.Add(new EditButton { Text = "Activi" });
                            cmbAng.Buttons.Add(new EditButton { Text = "Toti" });

                            break;
                    }


                            //<Buttons>
                            //    <dx:EditButton Text="Activi" />
                            //    <dx:EditButton Text="Toti" />
                            //</Buttons>                            


                    //Incarcam Angajatii in functie de rol
                    DataTable dtAngFiltrati = dtAng;
                    //Florin 2018.08.02
                    //if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != 0 && Convert.ToInt32(cmbRol.Value) != -44) dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();
                    if (cmbRol.Value != null && Convert.ToInt32(cmbRol.Value) != -44 && dtAng != null && dtAng.Rows.Count > 0) dtAngFiltrati = dtAng.Select("Rol=" + cmbRol.Value).CopyToDataTable();


                    //Florin 2019.01.17
                    //prima oara sa apara angajati activi
                    DataTable dtAngActivi = new DataTable();
                    if (dtAngFiltrati != null && dtAngFiltrati.Rows.Count > 0) dtAngActivi = dtAngFiltrati.Select("AngajatActiv=1").CopyToDataTable();
                    cmbAng.DataSource = dtAngActivi;
                    Session["Cereri_Absente_Angajati"] = dtAngActivi;
                    //cmbAng.DataSource = dtAngFiltrati;
                    //Session["Cereri_Absente_Angajati"] = dtAngFiltrati;
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
                    DataTable dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value,-99).ToString()), null);
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
                        //txtOreCalc.Value = itm.OreCalc;
                        //txtOreSursa.Value = itm.OreCalc;
                        //txtData1.Value = itm.Data1;
                        //txtData2.Value = itm.Data2;
                        //txtBifa1.Value = itm.Bifa1;
                        //txtBifa2.Value = itm.BIfa2;
                        //txtText.Value = itm.CmpText;
                        txtNrZileViitor.Value = itm.NrZileViitor;
                        txtObs.Value = itm.Obs;
                        lblDoc.InnerText = General.Nz(itm.UploadedFileName,"").ToString();

                        //if (dtAbs.Rows.Count > 0)
                        //{
                        //    DataRow[] arr = dtAbs.Select("Id=" + cmbAbs.Value);
                        //    if (arr.Count() > 0) AfiseazaCtl(arr[0]);
                        //}


                        //este necesar pt cand se intoarce din IstoricExtins
                        AfiseazaCtl();

                        ////Session["Absente_Cereri_Date"] = null;
                    }

                    //OreCalculate();
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
                    //MessageBox.Show(Dami.TraduCuvant("Nu exista angajat selectat"), MessageBox.icoWarning);
                    //return;
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
                //itm.OreCalc = txtOreCalc.Value;
                //itm.OreSursa = txtOreSursa.Value;
                //itm.Data1 = txtData1.Value;
                //itm.Data2 = txtData2.Value;
                //itm.Bifa1 = txtBifa1.Value;
                //itm.BIfa2 = txtBifa2.Value;
                //itm.CmpText = txtText.Value;
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

                if (idRol != -44) strSql += " AND Rol=" + idRol;
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

                        //calculam nr de zile luate

                        //int nr = 0;
                        //int nrViitor = 0;
                        //string adunaZL = General.Nz(arr[0]["AdunaZileLibere"], "0").ToString();
                        //General.CalcZile(txtDataInc.Date, txtDataSf.Date, adunaZL, out nr, out nrViitor);
                        //txtNrZile.Value = nr;
                        //txtNrZileViitor.Value = nrViitor;

                        txtNrZile.Value = General.CalcZile(Convert.ToInt32(cmbAng.Value ?? 0), Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), Convert.ToInt32(cmbRol.Value ?? 0), Convert.ToInt32(cmbAbs.Value ?? 0));
                    }
                }

                cmbAbs.DataSource = dtAbs;
                cmbAbs.DataBind();

                switch (tip)
                {
                    case "1":               //cmbAng
                        {
                            dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString()), null);
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
                            metaCereriDate itm = new metaCereriDate();
                            if (Session["Absente_Cereri_Date"] != null) itm = Session["Absente_Cereri_Date"] as metaCereriDate;
                            lblDoc.InnerText = General.Nz(itm.UploadedFileName, "").ToString();
                            //Florin 2019.01.11
                            AfiseazaCtl();
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
                        cmbAng.DataSource = dtAng;
                        Session["Cereri_Absente_Angajati"] = dtAng;
                        Session["Cereri_Absente_AngajatiToti"] = dtAng;
                        cmbAng.DataBind();
                        cmbAng.SelectedIndex = 0;


                        //acelasi cod ca la case "1"
                        dtAbs = General.IncarcaDT(General.SelectAbsente(General.Nz(cmbAng.Value, "-99").ToString()), null);
                        cmbAbs.DataSource = dtAbs;
                        cmbAbs.DataBind();
                        cmbAbs.SelectedIndex = -1;
                        Session["Cereri_Absente_Absente"] = dtAbs;
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

                //Session["Absente_Cereri_UploadedFile"] = btnDocUpload.UploadedFiles[0].FileBytes;
                //Session["Absente_Cereri_UploadedFileName"] = btnDocUpload.UploadedFiles[0].FileName;
                //Session["Absente_Cereri_UploadedFileExtension"] = btnDocUpload.UploadedFiles[0].ContentType;

                //lblDoc.InnerText = btnDocUpload.UploadedFiles[0].FileName;
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

                //Session["Absente_Cereri_UploadedFile"] = null;
                //Session["Absente_Cereri_UploadedFileName"] = null;
                //Session["Absente_Cereri_UploadedFileExtension"] = null;

                Session["Absente_Cereri_Date"] = itm;

                lblDoc.InnerHtml = "&nbsp;";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string VerificareDepasireNorma(int f10003, DateTime dtInc, int? nrOre, int tip)
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
                int norma = Convert.ToInt32(General.ExecutaScalar(strSql,null));

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
                if (((sumaCere * 60) + sumaPtj + (nrOre * 60)) > (norma * 60))
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

        private void SalveazaDate(int tip=1)
        {
            string log = DateTime.Now + " - 0";

            try
            {
                string strErr = "";

                if (cmbAng.Value == null) strErr += ", " + Dami.TraduCuvant("angajat");
                if (cmbAbs.Value == null) strErr += ", " + Dami.TraduCuvant("tip absenta");
                if (txtDataInc.Text == "") strErr += ", " + Dami.TraduCuvant("data inceput");
                if (txtDataSf.Text == "") strErr += ", " + Dami.TraduCuvant("data sfarsit");

                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
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
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

                    return;
                }


                #region Verificare validitate angajat

                try
                {
                    int esteActiv = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM F100 WHERE F10003={cmbAng.Value} AND F10022 <= {General.ToDataUniv(Convert.ToDateTime(txtDataInc.Value))} AND {General.ToDataUniv(Convert.ToDateTime(txtDataSf.Value))} <= F10023", null), 0));
                    if (esteActiv == 0)
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("In perioada solicitata, angajatul este inactiv"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In perioada solicitata, angajatul este inactiv");

                        return;
                    }
                }
                catch (Exception){}

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
                    //if (General.Nz(dr["IdCampExtra"], "").ToString() != "")
                    //{
                    //    if (ctl != null && ctl.Value != null && ctl.Value.ToString() != "")
                    //    {
                    //        switch(General.Nz(dr["TipCamp"], "").ToString())
                    //        {
                    //            case "0":
                    //                lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString() + "'";
                    //                break;
                    //            case "1":
                    //                if (Convert.ToBoolean(ctl.Value) == true)
                    //                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'Da'";
                    //                else
                    //                    lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'Nu'";
                    //                break;
                    //            case "3":
                    //                DateTime zi = Convert.ToDateTime(ctl.Value);
                    //                lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + zi.Day.ToString().PadLeft(2, '0') + "/" + zi.Month.ToString().PadLeft(2, '0') + "/" + zi.Year.ToString() + "'";
                    //                break;
                    //            default:
                    //                lstExtra[Convert.ToInt32(dr["IdCampExtra"]) - 1] = "'" + ctl.Value.ToString() + "'";
                    //                break;
                    //        }
                    //    }
                    //}
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

                if (txtDataSf.Date < txtDataInc.Date) strErr += " " + Dami.TraduCuvant("Data sfarsit este mai mica decat data inceput");

                //daca abs este de tip ore dtinc si datasf trebuie sa fie aceeasi
                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && txtDataInc.Date != txtDataSf.Date) strErr += " " + Dami.TraduCuvant("Data inceput si data sfarsit trebuie sa fie aceeasi in cazul acestui tip de absenta");


                #region OLD

                //#############################################################################################
                //S-au modificat campurile; aceasta verificare se face mai jos cu campurile NrMax si NrMaxAn
                //#############################################################################################

                ////nr de ore cerute sa nu depaseasca nr max de ore permis
                //if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0 && Convert.ToInt32(General.Nz(drAbs["NrMaxOre"], 999)) < Convert.ToInt32(txtNrOre.Value)) strErr += " " + Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMaxOre"], 999)) + " ore");

                #endregion


                if (Convert.ToInt32(General.Nz(txtNrZile.Value, -99)) <= 0) strErr += " " + Dami.TraduCuvant("Cerere cu numar de zile 0");

                if (strErr != "")
                {
                    if (tip == 1)
                        MessageBox.Show(strErr, MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = strErr;

                    return;
                }

                var ert = General.Nz(cmbAng.Value, -98);
                var edc = General.Nz(Session["User_Marca"], -97);
                var esx = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1", new object[] { cmbAng.Value }));

                if (Convert.ToInt32(General.Nz(cmbAng.Value,-98)) == Convert.ToInt32(General.Nz(Session["User_Marca"],-97)) && Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""F100Supervizori"" WHERE F10003=@1",new object[] { cmbAng.Value })) == 0)
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
                                WHERE A.F10003 = {cmbAng.Value} AND A.""DataInceput"" <= {General.ToDataUniv(txtDataSf.Date)} AND {General.ToDataUniv(txtDataInc.Date)} <= A.""DataSfarsit"" 
                                AND A.""IdStare"" IN (1,2,3,4) AND B.""GrupOre"" IN({General.Nz(drAbs["GrupOreDeVerificat"], -99)})", null));

                if (intersec > 0)
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Intervalul se intersecteaza cu altul deja existent"), MessageBox.icoWarning);
                    else
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
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Inlocuitorul are cerere in aceasta perioada"), MessageBox.icoWarning);
                        else
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
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Angajatul este desemnat inlocuitor pentru " + inlocOre), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Angajatul este desemnat inlocuitor pentru " + inlocOre);
                        
                        return;
                    }
                }


                //verificam data minima cerere

                //int idStareCumulat = 1;
                //DataRow drCum = General.IncarcaDR(@"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003=@1 AND ""An""=@2 AND ""Luna""=@3", new object[] { cmbAng.Value, txtDataInc.Date.Year, txtDataInc.Date.Month });
                //if (drCum != null) idStareCumulat = Convert.ToInt32(General.Nz(drCum["IdStare"], 1));

                ////verificam data minima cerere
                //try
                //{
                //    DataRow drParam = General.GetParamCereri(2, -99, Convert.ToInt32(General.Nz(drAbs["IdCircuit"], -99)));
                //    if (drParam != null)
                //    {
                //        switch (General.Nz(drParam["IdNomenclator"], 1).ToString())
                //        {
                //            case "7":
                //                if (idStareCumulat == 5 || idStareCumulat == 7)
                //                {
                //                    if (tip == 1)
                //                        MessageBox.Show(Dami.TraduCuvant("Pontajul este aprobat sau finalizat"), MessageBox.icoWarning);
                //                    else
                //                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pontajul este aprobat sau finalizat");

                //                    return;
                //                }
                //                break;
                //            default:
                //                if (txtDataInc.Date < Convert.ToDateTime(drParam["Valoare"]).Date)
                //                {
                //                    if (tip == 1)
                //                        MessageBox.Show(Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat " + Convert.ToDateTime(drParam["Valoare"]).Date.ToShortDateString()), MessageBox.icoWarning);
                //                    else
                //                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput trebuie sa fie mai mare sau egala decat " + Convert.ToDateTime(drParam["Valoare"]).Date.ToShortDateString());

                //                    return;
                //                }
                //                break;
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                //}




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
                    valoare = Convert.ToInt32(General.Nz(dtDrp.Rows[0]["Valoare"],-99));
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

                DateTime ziDrp = Dami.DataDrepturi(valoare, nrZile, Convert.ToDateTime(txtDataInc.Value), Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbRol.Value ?? -99));
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

                ////verificam starea pontajului si daca poate sa trimita in pontaj in functie de rol
                //int idRol = 1;
                //if (General.Nz(cmbAng.Value,-99).ToString() == General.VarSession("User_Marca").ToString())
                //{
                //    idRol = 1;
                //}
                //else
                //{
                //    DataTable dtRol = General.GetRoluriUser(Convert.ToInt32(Session["User_Marca"] ?? -99));
                //    DataRow[] dr = dtRol.Select("Id = 3");
                //    if (dr.Length > 0)
                //    {
                //        idRol = 3;
                //    }
                //    else
                //    {
                //        DataRow[] drTmp = dtRol.Select("Id = 2");
                //        if (drTmp.Length > 0) idRol = 2;
                //    }
                //}

                //bool stInc = true;
                //bool stSf = true;

                ////verificam cu data de inceput
                //{
                //    if (
                //        (idRol == 1 && (idStareCumulat == 1 || idStareCumulat == 4))
                //        ||
                //        (idRol == 2 && (idStareCumulat == 1 || idStareCumulat == 2 || idStareCumulat == 4 || idStareCumulat == 6))
                //        ||
                //        (idRol == 3 && (idStareCumulat == 1 || idStareCumulat == 2 || idStareCumulat == 3 || idStareCumulat == 4 || idStareCumulat == 5 || idStareCumulat == 6)))
                //    {
                //        //NOP
                //        //are voie
                //    }
                //    else
                //    {
                //        stInc = false;
                //    }
                //}

                ////verfiicam si cu data de sfarsit daca este in alta luna decat data de inceput
                //if (txtDataInc.Date.Month != txtDataSf.Date.Month)
                //{
                //    int starePtj = Convert.ToInt32(General.ExecutaScalar(@"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003=@1 AND ""An""=@2 AND ""Luna""=@3", new object[] { cmbAng.Value, txtDataSf.Date.Year, txtDataSf.Date.Month }) ?? 1);
                //    if (
                //        (idRol == 1 && (starePtj == 1 || starePtj == 4))
                //        ||
                //        (idRol == 2 && (starePtj == 1 || starePtj == 2 || starePtj == 4 || starePtj == 6))
                //        ||
                //        (idRol == 3 && (starePtj == 1 || starePtj == 2 || starePtj == 3 || starePtj == 4 || starePtj == 5 || starePtj == 6)))
                //    {
                //        //NOP
                //        //are voie
                //    }
                //    else
                //    {
                //        stSf = false;
                //    }
                //}

                //if (stInc == false || stSf == false)
                //{
                //    if (tip == 1)
                //        MessageBox.Show(Dami.TraduCuvant("Starea pontajului nu permite salvarea"), MessageBox.icoWarning);
                //    else
                //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Starea pontajului nu permite salvarea");
                    
                //    return;
                //}


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
                            string msgNr = VerificareDepasireNorma(Convert.ToInt32(cmbAng.Value), txtDataInc.Date, Convert.ToInt32(txtNrOre.Value ?? 0), 1);
                            if (msgNr != "")
                            {
                                if (tip == 1)
                                    MessageBox.Show(Dami.TraduCuvant(msgNr), MessageBox.icoWarning);
                                else
                                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msgNr);
                                
                                return;
                            }
                        }

                    }
                }

                #endregion

                #region Validare Max Ore

                int cv = Convert.ToInt32(Dami.ValoareParam("ZileCuveniteInAvans", "0") ?? "0");

                ////daca este CO verificam sa nu depaseasca orele cuvenite
                //try
                //{
                //    if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "CO" && General.Nz(drAbs["OreCalculateEticheta"], "").ToString() != "")
                //    {
                //        if (Convert.ToInt32(txtNrZile.Value) > Convert.ToInt32(General.Nz(txtOreCalc.Value, 0)) + cv)
                //        {
                //            if (tip == 1)
                //                MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr de zile cuvenite"), MessageBox.icoWarning);
                //            else
                //                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr de zile cuvenite");

                //            return;
                //        }
                //    }
                //}
                //catch (Exception)
                //{
                //}



                ////daca este ZL verificam sa nu depaseasca sold de zile recuperate
                //try
                //{
                //    if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper() == "ZL" && General.Nz(drAbs["OreCalculateEticheta"], "").ToString() != "")
                //    {
                //        if (Convert.ToInt32(txtNrZile.Value) > Convert.ToInt32(General.Nz(txtOreCalc.Value, 0)))
                //        {
                //            if (tip == 1)
                //                MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr de zile de recuperare"), MessageBox.icoWarning);
                //            else
                //                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr de zile de recuperare");

                //            return;
                //        }
                //    }
                //}
                //catch (Exception)
                //{
                //}


                if (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 1)) == 0)
                {
                    //daca este absenta de tip ora atunci se verifica doar campul NrMax; campul NrMaxAn nu are sens
                    if (Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) < Convert.ToInt32(txtNrOre.Value))
                    {
                        //strErr += " " + Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " ore");
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de") + " " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " " + Dami.TraduCuvant("ore"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Aveti voie sa cereti un numar maxim de" + " " + Convert.ToInt32(General.Nz(drAbs["NrMax"], 999)) + " " + Dami.TraduCuvant("ore"));

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
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite");

                        return;
                    }


                    //verificam nr max pe an
                    string sqlAn = $@"SELECT COALESCE(SUM(COALESCE(""NrZile"",0)),0) AS 'ZileAn' FROM ""Ptj_Cereri"" WHERE F10003=@1 AND {General.FunctiiData("\"DataInceput\"", "A")}=@2 AND ""IdAbsenta"" = @3 AND ""IdStare"" IN (1,2,3)";
                    DataRow drAn = General.IncarcaDR(sqlAn, new object[] { Convert.ToInt32(cmbAng.Value), txtDataInc.Date.Year, Convert.ToInt32(cmbAbs.Value) });

                    if (drAn != null && drAbs[0] != null && drAbs["NrMaxAn"] != DBNull.Value && Convert.ToInt32(drAbs["NrMaxAn"]) > (int)drAbs[0])
                    {
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an"), MessageBox.icoWarning);
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nr de zile depaseste nr maxim de zile cuvenite in an");

                        return;
                    }
                }

                try
                {
                    ////verificam daca cere prima de vacanta
                    //if (General.Nz(drAbs["DenumireScurta"], "").ToString().ToUpper().Trim() == "CO" && lblBifa1.InnerText.ToLower().IndexOf("prima") >= 0 && txtBifa1.Checked == true)
                    //{
                    //    int ani = Convert.ToInt32((txtDataInc.Date - Convert.ToDateTime(drAbs["DataAngajarii"])).TotalDays);
                    //    if (ani < 365)
                    //    {
                    //        if (tip == 1)
                    //            MessageBox.Show(Dami.TraduCuvant("Pentru acordarea de prima este necesar minim un an vechime"), MessageBox.icoWarning);
                    //        else
                    //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Pentru acordarea de prima este necesar minim un an vechime");
                            
                    //        return;
                    //    }

                    //    //daca a cerut prima inseamna ca idAbs este deja codul de absenta pentru CO
                    //    string sqlPrima = @"SELECT COUNT(*) FROM ""Ptj_Cereri"" WHERE F10003=@1 AND YEAR(""DataInceput"")=@2 AND ""IdAbsenta""=@3 AND ""CampBifa1""=1 AND ""IdStare"" IN (1,2,3)";
                    //    if (Constante.tipBD == 2) sqlPrima = @"SELECT COUNT(*) FROM ""Ptj_Cereri"" WHERE F10003=@1 AND TO_NUMBER(TO_CHAR(""DataInceput"",'YYYY'))=@2 AND ""IdAbsenta""=@3 AND ""CampBifa1""=1 AND ""IdStare"" IN (1,2,3)";
                    //    int arePrima = Convert.ToInt32(General.ExecutaScalar(sqlPrima, new object[] { Convert.ToInt32(cmbAng.Value), DateTime.Now.Year, Convert.ToInt32(cmbAbs.Value) }));
                    //    if (arePrima > 0)
                    //    {
                    //        if (tip == 1)
                    //            MessageBox.Show(Dami.TraduCuvant("A mai fost ceruta prima de vacanta pentru acest an"), MessageBox.icoWarning);
                    //        else
                    //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("A mai fost ceruta prima de vacanta pentru acest an");
                            
                    //        return;
                    //    }


                    //    int cmpText = 0;
                    //    DataTable dtSitCO = General.GetSituatieZileAbsente(Convert.ToInt32(cmbAng.Value), Convert.ToInt32(cmbAbs.Value), txtDataInc.Date.Year);
                    //    if (dtSitCO.Rows.Count > 0)
                    //    {
                    //        DataRow drSitCO = dtSitCO.Rows[0];
                    //        if (drSitCO != null)
                    //        {
                    //            int coAn = Convert.ToInt32(General.Nz(drSitCO["Aprobate"], 0)) + Convert.ToInt32(General.Nz(drSitCO["Planificate"], 0));

                    //            if (coAn < Convert.ToInt32(General.Nz(drSitCO["RamaseAnterior"], 0)))
                    //            {
                    //                if ((Convert.ToInt32(General.Nz(drSitCO["RamaseAnterior"], 0)) - coAn) < Convert.ToInt32(txtNrZile.Value))
                    //                    cmpText = (Convert.ToInt32(General.Nz(drSitCO["RamaseAnterior"], 0)) - coAn);
                    //                else
                    //                    cmpText = Convert.ToInt32(txtNrZile.Value);
                    //            }
                    //        }
                    //    }

                    //    if ((Convert.ToInt32(txtNrZile.Value) - cmpText) < 10)
                    //    {
                    //        if (tip == 1)
                    //            MessageBox.Show(Dami.TraduCuvant("Sunt necesare minim 10 zile pentru acordarea de prima"), MessageBox.icoWarning);
                    //        else
                    //            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Sunt necesare minim 10 zile pentru acordarea de prima");
                            
                    //        return;
                    //    }
                    //}
                }
                catch (Exception)
                {
                }

                #endregion

                #region Salvare in baza
                log += DateTime.Now + " - 1" + Environment.NewLine;
                #region Construim istoricul

                string sqlIst;
                int trimiteLaInlocuitor;
                General.SelectCereriIstoric(Convert.ToInt32(cmbAng.Value), Convert.ToInt32(General.Nz(cmbInloc.Value, -1)), Convert.ToInt32(drAbs["IdCircuit"]), Convert.ToInt32(General.Nz(drAbs["EstePlanificare"], 0)), out sqlIst, out trimiteLaInlocuitor);
                log += DateTime.Now + " - 2" + Environment.NewLine;

                #endregion


                string sqlPre = @"INSERT INTO ""Ptj_Cereri""(""Id"", F10003, ""IdAbsenta"", ""DataInceput"", ""DataSfarsit"", ""NrZile"", ""NrZileViitor"", ""Observatii"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalSuperCircuit"", ""Pozitie"", ""TrimiteLa"", ""NrOre"", ""AreAtas"", ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") 
                                OUTPUT Inserted.Id, Inserted.IdStare ";
                log += DateTime.Now + " - 3" + Environment.NewLine;
                string sqlCer = CreazaSelectCuValori();
                log += DateTime.Now + " - 4" + Environment.NewLine;
                string strGen = "BEGIN TRAN " +
                                sqlIst + "; " +
                                sqlPre +
                                sqlCer + (Constante.tipBD == 1 ? "" : " FROM DUAL") + "; " +
                                "COMMIT TRAN";

                if (Dami.ValoareParam("LogCereri", "0") == "1") General.CreazaLogCereri(strGen, cmbAng.Value.ToString(), txtDataInc.Value.ToString());
                log += DateTime.Now + " - 5" + Environment.NewLine;
                string msg = Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " +  General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0,1) == "2")
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
                    DataTable dtCer = new DataTable();
                    log += DateTime.Now + " - 6" + Environment.NewLine;
                    try
                    {
                        dtCer = General.IncarcaDT(strGen, null);
                    }
                    catch (Exception ex)
                    {
                        General.ExecutaNonQuery("ROLLBACK TRAN", null);
                        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                        return;
                    }

                    log += DateTime.Now + " - 7" + Environment.NewLine;
                    if (dtCer.Rows.Count > 0) idCer = Convert.ToInt32(dtCer.Rows[0]["Id"]);


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
                    log += DateTime.Now + " - 8" + Environment.NewLine;

                    string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };

                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        NotifAsync.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" WHERE ""Id""=" + idCer, "Ptj_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99), arrParam);
                        log += DateTime.Now + " - 10" + Environment.NewLine;
                    });

                    log += DateTime.Now + " - 9" + Environment.NewLine;
                    //Notif.TrimiteNotificare("Absente.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""Ptj_Cereri"" WHERE ""Id""=" + idCer, "Ptj_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

                    #region  Pontaj start

                    //completeaza soldul de ZL; Este numai pt clientul Groupama
                    General.SituatieZLOperatii(Convert.ToInt32(cmbAng.Value ?? -99), txtDataInc.Date, 2, Convert.ToInt32(txtNrZile.Value));
                    log += DateTime.Now + " - 11" + Environment.NewLine;
                    //trimite in pontaj daca este finalizat
                    if (Convert.ToInt32(dtCer.Rows[0]["IdStare"]) == 3)
                    {
                        log = DateTime.Now + " - 12" + Environment.NewLine;
                        if ((Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 1 || (Convert.ToInt32(General.Nz(drAbs["IdTipOre"], 0)) == 0 && General.Nz(drAbs["OreInVal"], "").ToString() != "")) && Convert.ToInt32(General.Nz(drAbs["NuTrimiteInPontaj"], 0)) == 0)
                        {
                            log += DateTime.Now + " - 13" + Environment.NewLine;
                            General.TrimiteInPontaj(Convert.ToInt32(Session["UserId"] ?? -99), idCer, 5, trimiteLaInlocuitor, Convert.ToInt32(txtNrOre.Value ?? 0));
                            log += DateTime.Now + " - 14" + Environment.NewLine;
                            //Se va face cand vom migra GAM
                            //TrimiteCerereInF300(Session["UserId"], idCer);
                        }
                    }

                    #endregion
                }

                #endregion

                
                Session["Absente_Cereri_Date_Aditionale"] = null;
                
                log += DateTime.Now + " - 15" + Environment.NewLine;

                if (msg != "" && msg.Substring(0, 1) != "2")
                {
                    if (tip == 1)
                        MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes, dar cu urmatorul avertisment: " + msg), MessageBox.icoWarning);
                    else
                        pnlCtl.JSProperties["cp_InfoMessage"] = Dami.TraduCuvant("Proces realizat cu succes, dar cu urmatorul avertisment: " + msg);
                }
                else
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
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
                General.MemoreazaEroarea(log);
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
                    DataRow[] arr = dtAbs.Select("Id=" + General.Nz(cmbAbs.Value,"-99"));
                    if (arr.Count() > 0)
                    {
                        DataRow dr = arr[0];

                        arataInloc = General.Nz(dr["ArataInlocuitor"], "1").ToString();
                        arataAtas = General.Nz(dr["ArataAtasament"], "1").ToString();
                        idOre = General.Nz(dr["IdTipOre"], "1").ToString();
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
                    txtNrOre.Visible = true;
                }
                else
                {
                    lblNrOre.Style["display"] = "none";
                    txtNrOre.Visible = false;
                    txtNrOre.Value = null;
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

        //private void OreCalculate()
        //{
        //    try
        //    {
        //        DataTable dtAng = Session["Cereri_Absente_Angajati"] as DataTable;
        //        DataRow[] lstAng = dtAng.Select("F10003=" + cmbAng.Value);
        //        if (txtDataInc.Value != null && lstAng.Count() > 0 && cmbAbs.Value != null)
        //        {
        //            DataRow entAng = lstAng[0];
        //            //if (dtAng.Columns["OreCalculate" + Convert.ToDateTime(txtDataInc.Value).Year + General.Nz(cmbAbs.Value, "-99")] != null)
        //            //    txtOreCalc.Value = lstAng[0]["OreCalculate" + Convert.ToDateTime(txtDataInc.Value).Year + General.Nz(cmbAbs.Value,"-99")];
        //            //if (dtAng.Columns["OreSursa" + Convert.ToDateTime(txtDataInc.Value).Year + General.Nz(cmbAbs.Value, "-99")] != null)
        //            //    txtOreSursa.Value = lstAng[0]["OreSursa" + Convert.ToDateTime(txtDataInc.Value).Year + General.Nz(cmbAbs.Value, "-99")];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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
                    //lbl.ID = "lblDinamic" + i;
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
                DataTable dt = General.IncarcaDT(CreazaSelectCuValori(), null);
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

        private string CreazaSelectCuValori()
        {
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
                    valExtra += "," + lstExtra[i] + "  AS \"CampExtra" + (i + 1).ToString() + "\" ";
                }

				if (Constante.tipBD == 1)
				{
					string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
					string sqlInloc = cmbInloc.Value == null ? "NULL" : cmbInloc.Value.ToString();
					string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
					string sqlIdStare = $@"(SELECT TOP 1 ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
					string sqlPozitie = $@"(SELECT TOP 1 ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
					string sqlCuloare = $@"(SELECT TOP 1 ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} ORDER BY ""Pozitie"" DESC) ";
					string sqlNrOre = txtNrOre.Text == "" ? "NULL" : txtNrOre.Text;

					//DateTime.Parse(a[1], Constante.formatDataSistem)

					sqlCer = @"SELECT " +
									sqlIdCerere + " AS \"Id\", " +
									cmbAng.Value + " AS \"F10003\", " +
									cmbAbs.Value + " AS \"IdAbsenta\", " +
									General.ToDataUniv(txtDataInc.Date) + " AS \"DataInceput\", " +
									General.ToDataUniv(txtDataSf.Date) + " AS \"DataSfarsit\", " +
									(txtNrZile.Value == null ? "NULL" : txtNrZile.Value.ToString()) + " AS \"NrZile\", " +
									(txtNrZileViitor.Value == null ? "NULL" : txtNrZileViitor.Value.ToString()) + " AS \"NrZileViitor\", " +
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
									areAtas + " AS \"AreAtas\"" +
									valExtra;
				}
				else
                {
                    string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""Ptj_Cereri"") ";
                    string sqlInloc = cmbInloc.Value == null ? "NULL" : cmbInloc.Value.ToString();
                    string sqlTotal = @"(SELECT COUNT(*) FROM ""Ptj_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
                    string sqlIdStare = $@"(SELECT ""IdStare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} WHERE ROWNUM=1 ORDER BY ""Pozitie"" DESC) ";
                    string sqlPozitie = $@"(SELECT  ""Pozitie"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} WHERE ROWNUM=1  ORDER BY ""Pozitie"" DESC) ";
                    string sqlCuloare = $@"(SELECT ""Culoare"" FROM ""Ptj_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""={sqlIdCerere} WHERE ROWNUM=1 ORDER BY ""Pozitie"" DESC) ";
                    string sqlNrOre = txtNrOre.Text == "" ? "NULL" : txtNrOre.Text;

                    //DateTime.Parse(a[1], Constante.formatDataSistem)

                    sqlCer = @"SELECT " +
                                    sqlIdCerere + " AS \"Id\", " +
                                    cmbAng.Value + " AS \"F10003\", " +
                                    cmbAbs.Value + " AS \"IdAbsenta\", " +
                                    General.ToDataUniv(txtDataInc.Date) + " AS \"DataInceput\", " +
                                    General.ToDataUniv(txtDataSf.Date) + " AS \"DataSfarsit\", " +
                                    (txtNrZile.Value == null ? "NULL" : txtNrZile.Value.ToString()) + " AS \"NrZile\", " +
                                    (txtNrZileViitor.Value == null ? "NULL" : txtNrZileViitor.Value.ToString()) + " AS \"NrZileViitor\", " +
                                    (txtObs.Value == null ? "NULL" : "'" + txtObs.Value.ToString() + "'") + " AS \"Observatii\", " +
                                    " \"IdStare\", " +
                                    (idCircuit) + " AS \"IdCircuit\", " +
                                    Session["UserId"] + " AS \"UserIntrod\", " +
                                    " \"Culoare\", " +
                                    (sqlInloc == null ? "NULL" : sqlInloc) + " AS \"Inlocuitor\", " +
                                    (sqlTotal == null ? "NULL" : sqlTotal) + " AS \"TotalSuperCircuit\", " +
                                    " \"Pozitie\", " +
                                    //trimiteLaInlocuitor + " AS \"TrimiteLa\", " +
                                    " NULL AS \"TrimiteLa\", " +
                                    (sqlNrOre == null ? "NULL" : sqlNrOre) + " AS \"NrOre\", " +
                                    areAtas + " AS \"AreAtas\"" +
                                    valExtra +
                                    " FROM " +
                                    sqlIdStare + ", " +
                                    sqlCuloare + ", " +
                                    sqlPozitie;


                }				
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
    }
}