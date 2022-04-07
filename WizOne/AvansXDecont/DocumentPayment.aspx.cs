using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.AvansXDecont
{
    public partial class DocumentPayment : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        public partial class metaAvsXDec_FisierBanca
        {
            public DateTime DataDocument { get; set; }
            public decimal SumaTotalaPlata { get; set; }
            public int NrTotalPlati { get; set; }
            public string CodBancaPlatitoare { get; set; }
            public string NumeCompanie { get; set; }
            public string IBANPlatitor { get; set; }
            public string NumePlatitor { get; set; }
            public string CodFiscalPlatitor { get; set; }
            public string StradaPlatitor { get; set; }
            public string OrasPlatitor { get; set; }
            public string CodUtilizator { get; set; }
            public string NumeFisier { get; set; }
            public string ReferintaUnicaPlata { get; set; }
            public DateTime executionDate { get; set; }
            public string NumeAngajat { get; set; }
            public decimal SumaPlata { get; set; }
            public string CurrencyCode { get; set; }
            public string CodBancaBeneficiara { get; set; }
            public string NumeBancaBeneficiara { get; set; }
            public string NumeSucursalaBancaBeneficiara { get; set; }
            public string IBANBeneficiar { get; set; }
            public string EmailBeneficiar { get; set; }
            public string CNPBeneficiar { get; set; }
            public string DetaliiPlata1 { get; set; }
            public string DetaliiPlata2 { get; set; }

        }

        /// pagina folosita de financiar si conta
        /// in functie de rolurile utilizatorului logat, acest ecran se incarca
        /// cu posibilitatea de :
        /// 1 -> genera fisiere de banca (pentru documentele care au modalitate plata transfer bancar)
        /// 2 -> plata documente (pentru documentele care au modalitate plata transfer bancar si cash)
        /// 3 -> generare note contabile (toate documente)
        /// 4 -> inchidere documente (pentru deconturi)

        private enum StareDocumentForUpdate
        {
            TrimisLaBanca = 4,
            Acordat = 5,
            Restituit = 6,
            Contat = 7,
            Inchis = 8
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                foreach (GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }
                lblStatusDoc.InnerText = Dami.TraduCuvant("Status documente");
                lblActiune.InnerText = Dami.TraduCuvant("Actiune");
                lblData.InnerText = Dami.TraduCuvant("Data platii");
                lblModPlata.InnerText = Dami.TraduCuvant("Modalitate plata");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                DataTable dtStari = General.IncarcaDT(@"SELECT b.""DictionaryItemId"" as ""Id"", b.""DictionaryItemName""  as ""Denumire"", b.""Culoare"" FROM ""vwAvsXDec_Nomen_StariDoc"" a LEFT JOIN ""AvsXDec_DictionaryItem"" b ON  a.DictionaryItemId = b.DictionaryItemId ", null);
                Session["AvsXDec_Stari"] = dtStari;

                DataTable dt = GetAvailableDocStateXUserRol(true, true /*Constante.AreRolFinanciar, Constante.AreRolContabilitate*/);
                cmbDocState.DataSource = dt;
                cmbDocState.DataBind();               

                dt = General.IncarcaDT("SELECT * FROM vwAvsXDec_OperationSign", null);
                cmbOperationSign.DataSource = dt;
                cmbOperationSign.DataBind();

                dt = GetAvsXDec_DictionaryItemModalitatePlata();
                cmbPaymentMethod.DataSource = dt;
                cmbPaymentMethod.DataBind();

                if (!IsPostBack)
                {
                    Session["AvsXDec_PaymentGrid"] = null;
                    Session["AvsXDec_DocStateUpdate"] = null;
                    cmbOperationSign.SelectedIndex = 0;
                    cmbDocState.SelectedIndex = 0;
                    cmbPaymentMethod.SelectedIndex = 0;
                    cmbDocState_SelectedIndexChanged(null, null);                   
                    dt.CaseSensitive = false;
                    DataRow entModalitatePlataCash = dt.Select("DictionaryItemName = 'PLATA CASH'").FirstOrDefault();
                    if (entModalitatePlataCash != null)
                    {
                        Session["IdModalitatePlataCash"] = Convert.ToInt32(entModalitatePlataCash["DictionaryItemId"].ToString());
                    }
                }

                IncarcaGrid();
                SetButtonContent();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void SetButtonContent()
        {
            StareDocumentForUpdate docStateUpdate = (StareDocumentForUpdate)Session["AvsXDec_DocStateUpdate"];
            switch (docStateUpdate)
            {
                case StareDocumentForUpdate.Acordat:
                    lblActiune.Visible = false;
                    cmbOperationSign.ClientVisible = false;
                    lblData.Visible = true;
                    txtPaymentDate.ClientVisible = true;                
                    btnSave.ClientEnabled = true;

                    if (Convert.ToInt32(cmbPaymentMethod.Value ?? -99) == Convert.ToInt32(General.Nz(Session["IdModalitatePlataCash"], -99)))
                    {
                        btnSave.Text = "Platit cash";
                        lblModPlata.Visible = true;
                        cmbPaymentMethod.ClientVisible = true;
                    }
                    else
                    {
                        btnSave.Text = "Plata documente";
                        lblModPlata.Visible = false;
                        cmbPaymentMethod.ClientVisible = false;
                    }
                    grDate.Columns["BankNamePlatitor"].Visible = false;
                    break;
                case StareDocumentForUpdate.Restituit:
                case StareDocumentForUpdate.TrimisLaBanca:
                    lblActiune.Visible = true;
                    cmbOperationSign.ClientVisible = true;
                    lblData.Visible = false;
                    txtPaymentDate.ClientVisible = false;
                    switch (docStateUpdate)
                    {
                        case StareDocumentForUpdate.Restituit:
                            btnSave.Text = "Restituire";
                            btnSave.ClientEnabled = true;
                            grDate.Columns["BankNamePlatitor"].Visible = false;
                            break;
                        case StareDocumentForUpdate.TrimisLaBanca:
                            grDate.Columns["BankNamePlatitor"].Visible = false;
                            btnSave.ClientEnabled = true;
                            btnSave.Text = "Generare fisiere banca";
                            break;
                    }
                    lblModPlata.Visible = true;
                    cmbPaymentMethod.ClientVisible = true;
                    break;
                case StareDocumentForUpdate.Contat:
                    cmbDocState.ClientVisible = true;
                    lblActiune.Visible = false;
                    cmbOperationSign.ClientVisible = false;
                    lblData.Visible = false;
                    txtPaymentDate.ClientVisible = false;
                    btnSave.ClientEnabled = true;
                    btnSave.Text = "Generare note contabile";                   
                    grDate.Columns["BankNamePlatitor"].Visible = true;
                    lblModPlata.Visible = false;
                    cmbPaymentMethod.ClientVisible = false;
                    if (!IsPostBack)
                        cmbPaymentMethod.Value = null;
                    break;
                case StareDocumentForUpdate.Inchis:
                    btnSave.Text = "Inchidere documente";                   
                    grDate.Columns["BankNamePlatitor"].Visible = false;
                    btnSave.ClientEnabled = false;
                    lblModPlata.Visible = false;
                    cmbPaymentMethod.ClientVisible = false;
                    if (!IsPostBack)
                        cmbPaymentMethod.Value = null;
                    break;
            }
        }

        public DataTable GetAvailableDocStateXUserRol(bool AreRolFinanciar, bool AreRolContabilitate)
        {
            /*LeonardM 21.06.2016
             * metoda ce returneaza statusul documentelor valabile
             * in functie de rolul utilizatorului
             * si cei de la financiar, cat si cei de la contabilitate acceseaza acelasi ecran
             * pentru cei de la financiar ajung documentele in starea
             * aprobat => pentru a fi trimise catre banca( daca clientul are de dat bani catre angajati)
             *         => sa fie marcate ca restituite (daca angajatul a restituit banii catre companie)
             * trimis la banca => pentru a marca sumele datorate de companie ca fiind acordate
             * pentru cei de la contabilityate ajung documentele in starea
             * acordat (ajung documentele pentru care au fost platite sumele datorate)
             * restituit (ajung documentele pentru care s-au restituit banii de la angajat catre companie
             * */

            DataTable q = null;
            string sql = "";

            try
            {
                sql = "SELECT b.DictionaryId, b.DictionaryItemId, b.Culoare, b.DictionaryItemName, COALESCE(b.Ordine, -99) AS Ordine "
                    + " FROM vwAvsXDec_Nomen_StariDoc a "
                    + " LEFT JOIN AvsXDec_DictionaryItem b on a.DictionaryItemId = b.DictionaryItemId "
                     //+ " WHERE a.DictionaryItemId IN (3, 4, 5, 6, 7)";    //momentan nu folosim starea Contat (7)
                     + " WHERE a.DictionaryItemId IN (3, 4, 5, 6)";

                q = General.IncarcaDT(sql, null);
                if (q != null && q.Rows.Count > 0)
                {
                    if (AreRolContabilitate == true && AreRolFinanciar != true)
                        q = q.Select("DictionaryItemId IN (5, 6 ,7)").CopyToDataTable();
                    else
                        if (AreRolFinanciar == true && AreRolContabilitate != true)
                        q = q.Select("DictionaryItemId IN (3, 4)").CopyToDataTable();
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetAvsXDec_DictionaryItemModalitatePlata()
        {
            DataTable q = null;
            string sql = "";
            try
            {
                sql = "SELECT a.DictionaryItemId, a.DictionaryId,  a.DictionaryItemName, COALESCE(b.Ordine, -99) AS Ordine "
                    + " FROM vwAvsXDec_Nomen_TipPlata a "
                    + " JOIN AvsXDec_DictionaryItem b  on a.DictionaryItemId = b.DictionaryItemId " 
                    + " ORDER BY Ordine ";
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
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


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                Session["AvsXDec_PaymentGrid"] = null;
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                StareDocumentForUpdate docStateUpdate = (StareDocumentForUpdate)Session["AvsXDec_DocStateUpdate"];

                string ras = ValidareDateObligatorii();
                if (ras != "")
                {
                    MessageBox.Show(Dami.TraduCuvant(ras), MessageBox.icoWarning, "");
                    return;
                }

                DataTable lstDocumentePlata = Session["AvsXDec_PaymentGrid"] as DataTable;
                List<object> lst = null;
                switch (docStateUpdate)
                {
                    case StareDocumentForUpdate.TrimisLaBanca:
                        #region trimite fisiere la banca                

                        string val = GenerateBankFile();                    
                         
                        try
                        {
                            if (!string.IsNullOrEmpty(val))
                            {
                                string[] roiFiles = val.Split(';');
                                foreach (string roi in roiFiles)
                                {
                                    //System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(Dami.AbsoluteUrl("FisiereBanca/Temp/" + roi), UriKind.Absolute), "blank");
                                    var fisBanca = HostingEnvironment.MapPath("~/FisiereBanca/Temp/" + roi);
                                    byte[] byteArray = File.ReadAllBytes(fisBanca);
                                    MemoryStream stream = new MemoryStream(byteArray);
                                    Response.Clear();
                                    MemoryStream ms = stream;
                                    Response.ContentType = "text/plain";
                                    Response.AddHeader("content-disposition", "attachment;filename=" + roi);
                                    Response.Buffer = true;
                                    ms.WriteTo(Response.OutputStream);
                                }

                                #region actualizare stare conform cerinte
                                lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "PaymentDate" });
                                if (lst != null && lst.Count() > 0 && lst[0] != null)
                                {
                                    for (int i = 0; i < lst.Count(); i++)
                                    {
                                        object[] arr = lst[i] as object[];
                                        DataRow dr = lstDocumentePlata.Select("DocumentId = " + arr[0].ToString()).FirstOrDefault();
                                        //if (dr != null)
                                        //    dr["DocumentStateIdUpdate"] = Convert.ToInt32(docStateUpdate);
                                      }
                                }
                                Session["AvsXDec_PaymentGrid"] = lstDocumentePlata;
                                if (lst != null && lst.Count() > 0 && lst[0] != null)
                                {
                                    for (int i = 0; i < lst.Count(); i++)
                                    {
                                        object[] arr = lst[i] as object[];
                                        SalvareGrid(arr[0].ToString());
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                MessageBox.Show("Nu au fost generate fisiere banca -> lista contine angajati fara banca alocata!", MessageBox.icoError, "Atentie !");
                            }

                        }
                        catch (Exception ex)
                        {
                            General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                        }
                            
                       
                        #endregion
                        break;
                    case StareDocumentForUpdate.Contat:
                        #region generare note contabile
                        //srvAvansXDecont ctx = new srvAvansXDecont();
                        //InvokeOperation opContat = ctx.GenerateAccountancyBill(lstDocumentePlata.Where(p => p.colSel == true).ToList<metaVwAvsXDec_DocumenteRestPlataMetadata>());
                        //opContat.Completed += (s2, args2) =>
                        //{
                        //    try
                        //    {
                        //        grDate.ShowLoadingPanel = false;
                        //        if (!string.IsNullOrEmpty(opContat.Value.ToString()))
                        //        {
                        //            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(Dami.AbsoluteUrl("NoteContabile/Temp/" + opContat.Value.ToString()), UriKind.Absolute), "blank");

                        //            #region actualizare stare conform cerinte
                        //            foreach (metaVwAvsXDec_DocumenteRestPlataMetadata entSalvare in lstDocumentePlata.Where(p => p.colSel == true))
                        //            {
                        //                entSalvare.DocumentStateIdUpdate = Convert.ToInt32(docStateUpdate);
                        //            }
                        //            #endregion
                        //            dds.SubmitChanges();
                        //        }
                        //        else
                        //        {
                        //            Message.InfoMessage("Nu au fost generate notele contabile!");
                        //            wiIndicator.DeferedVisibility = false;
                        //            grDate.ShowLoadingPanel = false;
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
                        //    }
                        //};
                        #endregion
                        break;
                    case StareDocumentForUpdate.Acordat:
                    case StareDocumentForUpdate.Restituit:
                        #region actualizare stare conform cerinte
                        lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "PaymentDate" });
                        if (lst != null && lst.Count() > 0 && lst[0] != null)
                        {
                            for (int i = 0; i < lst.Count(); i++)
                            {
                                object[] arr = lst[i] as object[];
                                DataRow dr = lstDocumentePlata.Select("DocumentId = " + arr[0].ToString()).FirstOrDefault();
                               // if (dr != null)
                               //     dr["DocumentStateIdUpdate"] = Convert.ToInt32(docStateUpdate);
                            }
                        }
                        Session["AvsXDec_PaymentGrid"] = lstDocumentePlata;
                        if (lst != null && lst.Count() > 0 && lst[0] != null)
                        {
                            for (int i = 0; i < lst.Count(); i++)
                            {
                                object[] arr = lst[i] as object[];
                                SalvareGrid(arr[0].ToString());
                            }
                        }
                        #endregion
                        break;
                }


                /*dupa ce am salvat platile setam statusul de plata document, ca fiind 
                * acordat, pentru avansuri*/
                int nrSel = 0;
                string ids = "";

                if (lst != null && lst.Count() > 0 && lst[0] != null)
                    for (int i = 0; i < lst.Count(); i++)
                    {
                        object[] arr = lst[i] as object[];
                        DataRow dr = lstDocumentePlata.Select("DocumentId = " + arr[0].ToString()).FirstOrDefault();
                        if (dr != null)
                        {
                            ids += dr["DocumentId"].ToString() + ",";
                            nrSel += 1;
                        }
                    }

                string message = SchimbaStatusDocument(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, Convert.ToInt32(Session["User_Marca"].ToString()), Convert.ToInt32(docStateUpdate));

                MessageBox.Show(Dami.TraduCuvant(message), MessageBox.icoInfo, "");
                Session["AvsXDec_PaymentGrid"] = null;
                IncarcaGrid();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string SchimbaStatusDocument(int idUser, string ids, int total, int f10003, int documentStateId)
        {
            string msg = "";

            try
            {
                string msgValid = "";
                string sql = "";
                if (ids == "") return "Nu exista inregistrari pentru aceasta actiune !";

                int nr = 0;
                string[] arr = ids.Split(Convert.ToChar(","));

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int DocumentId = -99;

                        try
                        {
                            DocumentId = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (DocumentId != -99)
                        {                           

                            DataTable ent = General.IncarcaDT("SELECT * FROM AvsXDec_Document Where DocumentId = " + DocumentId, null);

                            if (ent != null && ent.Rows.Count > 0)
                            {
                                DataTable entStr = General.IncarcaDT("SELECT * FROM AvsXDec_DictionaryItem Where DictionaryItemId = " + documentStateId, null);
                                string culoare = "FFFFFFFF";
                                if (entStr != null && entStr.Rows.Count > 0 && entStr.Rows[0]["Culoare"] != null) culoare = entStr.Rows[0]["Culoare"].ToString();

                                //schimbam statusul
                                General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId= " + documentStateId + ", Culoare = '" + culoare + "' Where DocumentId = " + DocumentId, null);


                                //introducem o linie de anulare in AvsXDec_DocumentStateHistory
                                sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
                                    + " VALUES ({0}, {1}, {2}, {3}, 22, '{4}', 1, GETDATE(), {5}, GETDATE(), 0)";
                                sql = string.Format(sql, Dami.NextId("AvsXDec_DocumentStateHistory", 1), ent.Rows[0]["DocumentId"].ToString(), ent.Rows[0]["CircuitId"].ToString(), documentStateId, culoare, idUser);
                                General.ExecutaNonQuery(sql, null);


                                #region prelucrare documente
                                switch (Convert.ToInt32(ent.Rows[0]["DocumentTypeId"].ToString()))
                                {
                                    case 1001:/*Avans spre deplasare*/
                                    case 1002:/*Avans spre decontare*/
                                        break;
                                    case 2001:/*Decont avans spre deplasare*/
                                    case 2002:/*Decont fonduri proprii*/
                                        #region inchidere decont
                                        /*LeonardM 11.08.2016
                                         * in cazul deconturilorla momentul contarii documentelor,
                                         * se verifica daca documentul are pusa bifa deja de documente originale
                                         * caz in care, documentul se trece automat si in starea inchis,
                                         * daca decontul nu are pusa bifa de documente originale, urmeaza ca dupa contare,
                                         * utilizatorul sa poata vedea documentele care au fost contate si sa le puna bifa de documente originale,
                                         * moment in care se inchide decontul*/
                                        if (documentStateId == 7)/*Contat*/
                                        {
                                            DataTable entDecont = General.IncarcaDT("SELECT * FROM AvsXDec_Decont Where DocumentId = " + ent.Rows[0]["DocumentId"].ToString());
                                            if (entDecont != null && entDecont.Rows.Count > 0)
                                            {
                                                if (Convert.ToInt32(General.Nz(entDecont.Rows[0]["OriginalDoc"], 0)) == 1)
                                                {
                                                    DataTable entStrInchis = General.IncarcaDT("SELECT * FROM AvsXDec_DictionaryItem Where DictionaryItemId = 8", null);
                                                    culoare = "FFFFFFFF";
                                                    if (entStrInchis != null && entStrInchis.Rows.Count > 0 && entStrInchis.Rows[0]["Culoare"] != null) culoare = entStrInchis.Rows[0]["Culoare"].ToString();

                                                    General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId= 8, Culoare = '" + culoare + "' Where DocumentId = " + DocumentId, null);

                                                    sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
                                                         + " VALUES ({0}, {1}, {2}, 8, 22, '{3}', 1, GETDATE(), {5}, GETDATE(), 0)";
                                                    sql = string.Format(sql, Dami.NextId("AvsXDec_DocumentStateHistory", 1), ent.Rows[0]["DocumentId"].ToString(), ent.Rows[0]["CircuitId"].ToString(), culoare, idUser);
                                                    General.ExecutaNonQuery(sql, null);
                                                }
                                            }
                                        }
                                        #endregion
                                        #region inchidere avans
                                        /*LeonardM 16.08.2016
                                         * in momentul in care un decont se trece in starea trimis la banca sau restituit,
                                         * avansul pe care este legat se inchide automat
                                         * avansul se mai poate inchide automat in momentul in care se aporba un decont in
                                         * care suma decont = suma avans*/
                                        if (documentStateId == 4 || documentStateId == 6) /*trimis la banca/retsituit*/
                                        {
                                            DataTable entDecontNou = General.IncarcaDT("SELECT * FROM AvsXDec_Decont Where DocumentId = " + ent.Rows[0]["DocumentId"].ToString());
                                            if (entDecontNou != null && entDecontNou.Rows.Count > 0)
                                            {
                                                DataTable bt = General.IncarcaDT("SELECT * FROM AvsXDec_BusinessTransaction Where DestDocId = " + entDecontNou.Rows[0]["DocumentId"].ToString());
                                                if (bt != null && bt.Rows.Count > 0)
                                                {
                                                    DataTable entAvans = General.IncarcaDT("SELECT * FROM AvsXDec_Document where DocumentId = " + bt.Rows[0]["SrcDocId"].ToString());
                                                    if (entAvans != null && entAvans.Rows.Count > 0)
                                                    {
                                                        /*verificam ca avansul sa nu fie inchis deja*/
                                                        if (Convert.ToInt32(entAvans.Rows[0]["DocumentStateId"].ToString()) != 8)
                                                        {
                                                            DataTable entStrInchis = General.IncarcaDT("SELECT * FROM AvsXDec_DictionaryItem Where DictionaryItemId = 8", null);
                                                            culoare = "FFFFFFFF";
                                                            if (entStrInchis != null && entStrInchis.Rows.Count > 0 && entStrInchis.Rows[0]["Culoare"] != null) culoare = entStrInchis.Rows[0]["Culoare"].ToString();

                                                            General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId= 8, Culoare = '" + culoare + "' Where DocumentId = " + entAvans.Rows[0]["DocumentId"].ToString(), null);

                                                            sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
                                                                 + " VALUES ({0}, {1}, {2}, 8, 22, '{3}', 1, GETDATE(), {5}, GETDATE(), 0)";
                                                            sql = string.Format(sql, Dami.NextId("AvsXDec_DocumentStateHistory", 1), entAvans.Rows[0]["DocumentId"].ToString(), entAvans.Rows[0]["CircuitId"].ToString(), culoare, idUser);
                                                            General.ExecutaNonQuery(sql, null);                                                        
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        #endregion
                                        break;
                                }
                                #endregion  

                                #region  Notificare strat

                                //srvNotif ctxNtf = new srvNotif();
                                //ctxNtf.TrimiteNotificare("Absente.Aprobare", "grDate", ent, idUser, f10003);

                                #endregion


                                msg = "Proces finalizat cu succes";

                                #region  Notificare strat

                                //ctxNtf.TrimiteNotificare("AvansXDecont.Document", "grDate", ent, idUser, f10003);

                                #endregion

                                nr++;
                            }
                        }
                    }
                }

                if (nr > 0)
                {
                    string stare = "";
                    switch (documentStateId)
                    {
                        case 4:
                            stare = "Trimis la banca";
                            break;
                        case 5:
                            stare = "Acordat";
                            break;
                        case 6:
                            stare = "Restituit";
                            break;
                        case 7:
                            stare = "Contat";
                            break;
                        case 8:
                            stare = "Inchis";
                            break;
                    }

                    msg = "S-a schimbat statusul pentru " + nr.ToString() + " documente din " + total + " in  " + stare + " !";
                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista documente pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
        }

        public void SalvareGrid(string documentId)
        {
            string sql = "";
            DataTable ent = Session["AvsXDec_PaymentGrid"] as DataTable;
            DataRow dr = ent.Select("DocumentId = " + documentId).FirstOrDefault();
            try
            {
                switch (Convert.ToInt32(dr["DocumentTypeId"].ToString()))
                {
                    case 1001: /*Avans spre deplasare*/
                    case 1002: /*Avans spre decontare*/
                        List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "PaymentDate" });
                        if (lst != null && lst.Count() > 0 && lst[0] != null)
                        {
                            //for (int i = 0; i < lst.Count(); i++)
                            //{
                                //object[] arr = lst[i] as object[];
                                //DataRow dr = ent.Select("DocumentId = " + arr[0].ToString()).FirstOrDefault();
                                if (dr != null)
                                {
                                    DateTime data = Convert.ToDateTime(dr["PaymentDate"].ToString());
                                    sql = "UPDATE AvsXDec_Avans SET PaymentDate = CONVERT(DATETIME, '" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 103), TotalPayment = " 
                                        + (Convert.ToDecimal(dr["UnconfRestAmount"].ToString()) - Convert.ToDecimal(dr["TotalComissionPayment"].ToString())).ToString().Replace(',', '.')
                                        /*LeonardM 10.08.2016
                                        * se sterge si comisionul bancare ca altfel ramane diferenta*/
                                        + ", UnconfRestAmount = UnconfRestAmount - " + (Convert.ToDecimal(dr["UnconfRestAmount"].ToString()) - Convert.ToDecimal(dr["TotalComissionPayment"].ToString())).ToString().Replace(',', '.') + " - " + Convert.ToDecimal(dr["TotalComissionPayment"].ToString()).ToString().Replace(',', '.')
                                        + ", TotalComissionPayment = " + Convert.ToDecimal(dr["TotalComissionPayment"].ToString()).ToString().Replace(',', '.') + ", PaymentCurrencyId = " + dr["PaymentCurrencyId"].ToString() 
                                        //+ ", DocumentStateIdUpdate = " + dr["DocumentStateIdUpdate"].ToString() 
                                        + " WHERE DocumentId = " + dr["DocumentId"].ToString();
                                    General.ExecutaNonQuery(sql, null);
                                }
                            //}
                        }
                        break;
                    case 2001: /*Decont cheltuieli deplasare*/
                    case 2002: /*Decont cheltuieli*/
                        List<object> lst1 = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "PaymentDate" });
                        if (lst1 != null && lst1.Count() > 0 && lst1[0] != null)
                        {
                            //for (int i = 0; i < lst1.Count(); i++)
                            //{
                                //object[] arr = lst1[i] as object[];
                                //DataRow dr = ent.Select("DocumentId = " + arr[0].ToString()).FirstOrDefault();
                                if (dr != null)
                                {
                                    DateTime data = Convert.ToDateTime(dr["PaymentDate"].ToString());
                                    sql = "UPDATE AvsXDec_Decont SET PaymentDate = CONVERT(DATETIME, '" + data.Day.ToString().PadLeft(2, '0') + "/" + data.Month.ToString().PadLeft(2, '0') + "/" + data.Year.ToString() + "', 103), PaymentValueFinance = "
                                        + (Convert.ToDecimal(dr["UnconfRestAmount"].ToString()) - Convert.ToDecimal(dr["TotalComissionPayment"].ToString())).ToString().Replace(',', '.')
                                        + ", UnconfRestAmount = " + Convert.ToDecimal(dr["UnconfRestAmount"].ToString()).ToString().Replace(',', '.') + " - "  + (Convert.ToDecimal(dr["UnconfRestAmount"].ToString()) - Convert.ToDecimal(dr["TotalComissionPayment"].ToString())).ToString().Replace(',', '.') 
                                        + " - " + Convert.ToDecimal(dr["TotalComissionPayment"].ToString()).ToString().Replace(',', '.')
                                        + ", ComissionValueFinance = " + Convert.ToDecimal(dr["TotalComissionPayment"].ToString()).ToString().Replace(',', '.') + ", PaymentCurrencyId = " + dr["PaymentCurrencyId"].ToString() 
                                        //+ ", DocumentStateIdUpdate = " + dr["DocumentStateIdUpdate"].ToString() 
                                        + "  WHERE DocumentId = " + dr["DocumentId"].ToString();
                                    General.ExecutaNonQuery(sql, null);
                                }
                            //}
                        }
           
                        break;
                }                    
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private string ValidareDateObligatorii()
        {
            string resultMethod = string.Empty;
            string verificareDate = string.Empty;
            bool errValidare = false;
            StareDocumentForUpdate docStateUpdate = (StareDocumentForUpdate)Session["AvsXDec_DocStateUpdate"];
            try
            {
                int docValid = 0;
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "PaymentDate" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    return "Nu ati selectat niciun document pentru a fi schimbat pentru status " + docStateUpdate.ToString() + "!";
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DateTime? data = arr[1] as DateTime?;
                    switch (docStateUpdate)
                    {
                        case StareDocumentForUpdate.Acordat:
                            if (data == null)
                            {
                                verificareDate += ", data platii";
                                errValidare = true;
                            }
                            break;
                        case StareDocumentForUpdate.Contat:
                            break;
                        case StareDocumentForUpdate.Restituit:
                            break;
                        case StareDocumentForUpdate.TrimisLaBanca:
                            break;
                    }
                    if (errValidare)
                        break;
                    docValid++;
                    
                }
                if (!string.IsNullOrEmpty(verificareDate))
                    resultMethod = "Lipseste " + verificareDate.Substring(2) + "!";

                if (string.IsNullOrEmpty(verificareDate) && docValid == 0)
                    resultMethod = "Nu ati selectat niciun document pentru a fi schimbat pentru status " + docStateUpdate.ToString() + "!";
            }
            catch (Exception)
            {
            }

            return Dami.TraduCuvant(resultMethod);
        }

        public string GenerateBankFile()
        {
            string roiFiles = string.Empty;
            try
            {

                #region preluare cale fisier banca
                string caleFisierBanca = string.Empty;
                /*LeonardM 22.06.2016
                 * nu mai descarcam fisierele de banca intr-un folder marcat prin parametrii,
                 * ci le punem in folderul FisiereBanca/Temp, urmand ca utilizatorul sa le descarce unde doreste
                 * 
                var entCaleFisierBanca = ctx.tblParametrii.Where(p => p.Nume == "AvsXDec_CaleFisiereBanca").FirstOrDefault();
                if (entCaleFisierBanca != null)
                    caleFisierBanca = entCaleFisierBanca.Valoare;
                else
                    caleFisierBanca = "/Temp/";
                 * */
                caleFisierBanca = "/FisiereBanca/Temp/";
                #endregion

                string codVerificareBCR = "RNCB";
                string codVerificareBRD = "BRDE";
                string codVerificareBT = "BTRL";

                string codBancaBCR = string.Empty, numeBancaBCR = string.Empty, codIbanBCR = string.Empty;
                string codBancaBRD = string.Empty, numeBancaBRD = string.Empty, codIbanBRD = string.Empty;
                string codBancaBT = string.Empty, numeBancaBT = string.Empty, codIbanBT = string.Empty;
                List<metaAvsXDec_FisierBanca> lstBCR = new List<metaAvsXDec_FisierBanca>();
                List<metaAvsXDec_FisierBanca> lstBRD = new List<metaAvsXDec_FisierBanca>();
                List<metaAvsXDec_FisierBanca> lstBT = new List<metaAvsXDec_FisierBanca>();

                DataTable ent = General.IncarcaDT(" SELECT * FROM F002", null);

                #region determinare banca 1
                switch (ent.Rows[0]["F002513"].ToString().Substring(4, 4))
                {
                    case "BRDE":
                        codBancaBRD = codVerificareBRD;
                        DataTable entBRD = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002511"].ToString() + "'");
                        if (entBRD != null && entBRD.Rows.Count > 0)
                        {
                            numeBancaBRD = ent.Rows[0]["F002511"].ToString();
                            codIbanBRD = ent.Rows[0]["F002513"].ToString();
                        }
                        break;
                    case "BTRL":
                        codBancaBT = codVerificareBT;
                        codBancaBRD = codVerificareBRD;
                        DataTable entBT = General.IncarcaDT("SELECT * FROM F075 Where F07509 =  '" + ent.Rows[0]["F002511"].ToString() + "'");
                        if (entBT != null && entBT.Rows.Count > 0)
                        {
                            numeBancaBT = ent.Rows[0]["F002511"].ToString();
                            codIbanBT = ent.Rows[0]["F002513"].ToString();
                        }
                        break;
                    default: /* BCR */
                        codBancaBCR = codVerificareBCR;
                        DataTable entBCR = General.IncarcaDT("SELECT * FROM F075 Where F07509 =  '" + ent.Rows[0]["F002511"].ToString() + "'");
                        if (entBCR != null && entBCR.Rows.Count > 0)
                        {
                            numeBancaBCR = ent.Rows[0]["F002511"].ToString();
                            codIbanBCR = ent.Rows[0]["F002513"].ToString();
                        }
                        break;
                }
                #endregion

                #region determinare banca 2
                if (ent.Rows[0]["F002523"] != null && ent.Rows[0]["F002523"].ToString().Length >= 8)
                    switch (ent.Rows[0]["F002523"].ToString().Substring(4, 4))
                    {
                        case "BRDE":
                            codBancaBRD = codVerificareBRD;
                            DataTable entBRD = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002521"].ToString() + "'");
                            if (entBRD != null && entBRD.Rows.Count > 0)
                            {
                                numeBancaBRD = ent.Rows[0]["F002521"].ToString();
                                codIbanBRD = ent.Rows[0]["F002523"].ToString();
                            }
                            break;
                        case "BTRL":
                            codBancaBT = codVerificareBT;
                            codBancaBRD = codVerificareBRD;
                            DataTable entBT = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002521"].ToString() + "'");
                            if (entBT != null && entBT.Rows.Count > 0)
                            {
                                numeBancaBT = ent.Rows[0]["F002521"].ToString();
                                codIbanBT = ent.Rows[0]["F002523"].ToString();
                            }
                            break;
                        default:/*bcr*/
                            codBancaBCR = codVerificareBCR;
                            DataTable entBCR = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002521"].ToString() + "'");
                            if (entBCR != null && entBCR.Rows.Count > 0)
                            {
                                numeBancaBCR = ent.Rows[0]["F002521"].ToString();
                                codIbanBCR = ent.Rows[0]["F002523"].ToString();
                            }
                            break;
                    }
                #endregion

                #region determinare banca 3
                if (ent.Rows[0]["F002533"] != null && ent.Rows[0]["F002533"].ToString().Length >= 8)
                    switch (ent.Rows[0]["F002533"].ToString().Substring(4, 4))
                    {
                        case "BRDE":
                            codBancaBRD = codVerificareBRD;
                            DataTable entBRD = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002531"].ToString() + "'");
                            if (entBRD != null && entBRD.Rows.Count > 0)
                            {
                                numeBancaBRD = ent.Rows[0]["F002531"].ToString();
                                codIbanBRD = ent.Rows[0]["F002533"].ToString();
                            }
                            break;
                        case "BTRL":
                            codBancaBT = codVerificareBT;
                            codBancaBRD = codVerificareBRD;
                            DataTable entBT = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002531"].ToString() + "'");
                            if (entBT != null && entBT.Rows.Count > 0)
                            {
                                numeBancaBT = ent.Rows[0]["F002531"].ToString();
                                codIbanBT = ent.Rows[0]["F002533"].ToString();
                            }
                            break;
                        default:/*bcr*/
                            codBancaBCR = codVerificareBCR;
                            DataTable entBCR = General.IncarcaDT("SELECT * FROM F075 Where F07509 = '" + ent.Rows[0]["F002531"].ToString() + "'");
                            if (entBCR != null && entBCR.Rows.Count > 0)
                            {
                                numeBancaBCR = ent.Rows[0]["F002531"].ToString();
                                codIbanBCR = ent.Rows[0]["F002533"].ToString();
                            }
                            break;
                    }
                #endregion

                #region construire liste pentru banca
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "F10003", "UnconfRestAmount", "CurrencyCode" });
                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    DataTable doc = General.IncarcaDT("SELECT * FROM AvsXDec_Document Where DocumentId = " + arr[0].ToString());
                    if (doc == null || doc.Rows.Count <= 0)
                        continue;

                    DataTable angajat = General.IncarcaDT("SELECT * FROM F100 Where F10003 = " + arr[1].ToString());
                    if (angajat == null || angajat.Rows.Count <= 0)
                        continue;

                    DataTable bancaProprie = General.IncarcaDT("SELECT * FROM F075 Where F07503 = " + angajat.Rows[0]["F10018"].ToString() + " AND F07504 = " + angajat.Rows[0]["F10019"].ToString());
                    if (bancaProprie == null || bancaProprie.Rows.Count <= 0)
                        continue;

                    bool docAllocatedOnDefaultBank = false;

                    DataTable entComp = null;
                    //verificam daca, pe document, a fost salvata alta companie decat cea pe care o are angajatul in F100
                    if (doc.Rows[0]["IdCompanie"] != DBNull.Value && Convert.ToInt32(doc.Rows[0]["IdCompanie"].ToString()) > 0 && Convert.ToInt32(doc.Rows[0]["IdCompanie"].ToString()) != Convert.ToInt32(angajat.Rows[0]["F10002"].ToString()))
                    {
                        entComp = General.IncarcaDT("SELECT * FROM AvsXDec_Companie WHERE Id = " + doc.Rows[0]["IdCompanie"].ToString(), null);
                    }


                    if (angajat.Rows[0]["F10020"].ToString().Substring(4, 4) == codVerificareBRD)
                    {
                        #region lista BRD
                        metaAvsXDec_FisierBanca entBCR = new metaAvsXDec_FisierBanca();
                        entBCR.DataDocument = DateTime.Now;
                        entBCR.SumaTotalaPlata = 0;
                        entBCR.NrTotalPlati = 0;
                        entBCR.CodBancaPlatitoare = codBancaBRD;
                        entBCR.NumePlatitor = numeBancaBRD;
                        entBCR.IBANPlatitor = codIbanBRD;
                        entBCR.CodFiscalPlatitor = entComp != null ? entComp.Rows[0]["CodFiscal"].ToString() : ent.Rows[0]["F00207"].ToString();
                        entBCR.StradaPlatitor = entComp != null ? entComp.Rows[0]["Artera"].ToString() + " NR " + entComp.Rows[0]["Numar"].ToString() : ent.Rows[0]["F00233"].ToString() + " NR " + ent.Rows[0]["F00234"].ToString();
                        entBCR.OrasPlatitor = entComp != null ? entComp.Rows[0]["Localitate"].ToString().ToUpper() : ent.Rows[0]["F00231"].ToString().ToUpper();
                        entBCR.CodUtilizator = string.Empty;
                        entBCR.NumeFisier = string.Empty;
                        entBCR.ReferintaUnicaPlata = (lstBCR.Count + 1).ToString();
                        entBCR.executionDate = DateTime.Now;
                        entBCR.NumeAngajat = angajat.Rows[0]["F10008"].ToString() + " " + angajat.Rows[0]["F10009"].ToString();
                        entBCR.SumaPlata = Convert.ToDecimal(arr[2].ToString());
                        entBCR.CurrencyCode = arr[3].ToString();
                        entBCR.CodBancaBeneficiara = angajat.Rows[0]["F10020"].ToString().Substring(4, 4);
                        entBCR.NumeBancaBeneficiara = bancaProprie.Rows[0]["F07509"].ToString();
                        entBCR.NumeCompanie = entComp != null ? entComp.Rows[0]["Denumire"].ToString() : ent.Rows[0]["F00204"].ToString();
                        entBCR.NumeSucursalaBancaBeneficiara = bancaProprie.Rows[0]["F07505"].ToString();
                        entBCR.IBANBeneficiar = angajat.Rows[0]["F10020"].ToString();
                        entBCR.EmailBeneficiar = angajat.Rows[0]["F100894"].ToString();
                        entBCR.CNPBeneficiar = angajat.Rows[0]["F10017"].ToString();
                        entBCR.DetaliiPlata1 = "pl {0} nr" + doc.Rows[0]["DocumentId"].ToString();
                        entBCR.DetaliiPlata2 = "din data " + Convert.ToDateTime(doc.Rows[0]["DocumentDate"].ToString());
                        switch (Convert.ToInt32(doc.Rows[0]["DocumentTypeId"].ToString()))
                        {
                            case 1001: /* avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre deplasare");
                                break;
                            case 1002: /*avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans decontare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre decontare");
                                break;
                            case 1003: /*avans administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans administrativ");
                                break;
                            case 2001: /* decont avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "ordin deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre deplasare");
                                break;
                            case 2002: /*decont avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre decontare");
                                break;
                            case 2003: /*decont administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont administrativ");
                                break;
                        }
                        #endregion
                        lstBRD.Add(entBCR);
                        docAllocatedOnDefaultBank = true;
                    }

                    if (angajat.Rows[0]["F10020"].ToString().Substring(4, 4) == codVerificareBT)
                    {
                        #region lista BT
                        metaAvsXDec_FisierBanca entBCR = new metaAvsXDec_FisierBanca();
                        entBCR.DataDocument = DateTime.Now;
                        entBCR.SumaTotalaPlata = 0;
                        entBCR.NrTotalPlati = 0;
                        entBCR.CodBancaPlatitoare = codBancaBT;
                        entBCR.NumePlatitor = numeBancaBT;
                        entBCR.IBANPlatitor = codIbanBT;
                        entBCR.CodFiscalPlatitor = entComp != null ? entComp.Rows[0]["CodFiscal"].ToString() : ent.Rows[0]["F00207"].ToString();
                        entBCR.StradaPlatitor = entComp != null ? entComp.Rows[0]["Artera"].ToString() + " NR " + entComp.Rows[0]["Numar"].ToString() : ent.Rows[0]["F00233"].ToString() + " NR " + ent.Rows[0]["F00234"].ToString();
                        entBCR.OrasPlatitor = entComp != null ? entComp.Rows[0]["Localitate"].ToString().ToUpper() : ent.Rows[0]["F00231"].ToString().ToUpper();
                        entBCR.CodUtilizator = string.Empty;
                        entBCR.NumeFisier = string.Empty;
                        entBCR.ReferintaUnicaPlata = (lstBCR.Count + 1).ToString();
                        entBCR.executionDate = DateTime.Now;
                        entBCR.NumeAngajat = angajat.Rows[0]["F10008"].ToString() + " " + angajat.Rows[0]["F10009"].ToString();   
                        entBCR.SumaPlata = Convert.ToDecimal(arr[2].ToString());
                        entBCR.CurrencyCode = arr[3].ToString();
                        entBCR.CodBancaBeneficiara = angajat.Rows[0]["F10020"].ToString().Substring(4, 4);
                        entBCR.NumeBancaBeneficiara = bancaProprie.Rows[0]["F07509"].ToString();
                        entBCR.NumeCompanie = entComp != null ? entComp.Rows[0]["Denumire"].ToString() : ent.Rows[0]["F00204"].ToString();
                        entBCR.NumeSucursalaBancaBeneficiara = bancaProprie.Rows[0]["F07505"].ToString();
                        entBCR.IBANBeneficiar = angajat.Rows[0]["F10020"].ToString();
                        entBCR.EmailBeneficiar = angajat.Rows[0]["F100894"].ToString();
                        entBCR.CNPBeneficiar = angajat.Rows[0]["F10017"].ToString();
                        entBCR.DetaliiPlata1 = "pl {0} nr" + doc.Rows[0]["DocumentId"].ToString();
                        entBCR.DetaliiPlata2 = "din data " + Convert.ToDateTime(doc.Rows[0]["DocumentDate"].ToString());
                        switch (Convert.ToInt32(doc.Rows[0]["DocumentTypeId"].ToString()))
                        {
                            case 1001: /* avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre deplasare");
                                break;
                            case 1002: /*avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans decontare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre decontare");
                                break;
                            case 1003: /*avans administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans administrativ");
                                break;
                            case 2001: /* decont avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "ordin deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre deplasare");
                                break;
                            case 2002: /*decont avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre decontare");
                                break;
                            case 2003: /*decont administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont administrativ");
                                break;
                        }
                        #endregion
                        lstBT.Add(entBCR);
                        docAllocatedOnDefaultBank = true;
                    }

                    if ((!docAllocatedOnDefaultBank && angajat.Rows[0]["F10020"].ToString().Substring(4, 4) != codVerificareBCR) || (angajat.Rows[0]["F10020"].ToString().Substring(4, 4) == codVerificareBCR))
                    {
                        #region lista bcr
                        metaAvsXDec_FisierBanca entBCR = new metaAvsXDec_FisierBanca();  
                        entBCR.DataDocument = DateTime.Now;
                        entBCR.SumaTotalaPlata = 0;
                        entBCR.NrTotalPlati = 0;
                        entBCR.CodBancaPlatitoare = codBancaBCR;
                        entBCR.NumePlatitor = numeBancaBCR;
                        entBCR.IBANPlatitor = codIbanBCR;
                        entBCR.CodFiscalPlatitor = entComp != null ? entComp.Rows[0]["CodFiscal"].ToString() : ent.Rows[0]["F00207"].ToString();
                        entBCR.StradaPlatitor = entComp != null ? entComp.Rows[0]["Artera"].ToString() + " NR " + entComp.Rows[0]["Numar"].ToString() : ent.Rows[0]["F00233"].ToString() + " NR " + ent.Rows[0]["F00234"].ToString();
                        entBCR.OrasPlatitor = entComp != null ? entComp.Rows[0]["Localitate"].ToString().ToUpper() : ent.Rows[0]["F00231"].ToString().ToUpper();
                        entBCR.CodUtilizator = string.Empty;
                        entBCR.NumeFisier = string.Empty;
                        entBCR.ReferintaUnicaPlata = (lstBCR.Count + 1).ToString();
                        entBCR.executionDate = DateTime.Now;
                        entBCR.NumeAngajat = angajat.Rows[0]["F10008"].ToString() + " " + angajat.Rows[0]["F10009"].ToString();
                        entBCR.SumaPlata = Convert.ToDecimal(arr[2].ToString());
                        entBCR.CurrencyCode = arr[3].ToString();
                        entBCR.CodBancaBeneficiara = angajat.Rows[0]["F10020"].ToString().Substring(4, 4);
                        entBCR.NumeBancaBeneficiara = bancaProprie.Rows[0]["F07509"].ToString();
                        entBCR.NumeCompanie = entComp != null ? entComp.Rows[0]["Denumire"].ToString() : ent.Rows[0]["F00204"].ToString();
                        entBCR.NumeSucursalaBancaBeneficiara = bancaProprie.Rows[0]["F07505"].ToString();
                        entBCR.IBANBeneficiar = angajat.Rows[0]["F10020"].ToString();
                        entBCR.EmailBeneficiar = angajat.Rows[0]["F100894"].ToString();
                        entBCR.CNPBeneficiar = angajat.Rows[0]["F10017"].ToString();
                        entBCR.DetaliiPlata1 = "pl {0} nr" + doc.Rows[0]["DocumentId"].ToString();
                        entBCR.DetaliiPlata2 = "din data " + Convert.ToDateTime(doc.Rows[0]["DocumentDate"].ToString());

                        switch (Convert.ToInt32(doc.Rows[0]["DocumentTypeId"].ToString()))
                        {
                            case 1001: /* avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre deplasare");
                                break;
                            case 1002: /*avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans decontare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans spre decontare");
                                break;
                            case 1003: /*avans administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "avans administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "avans administrativ");
                                break;
                            case 2001: /* decont avans spre deplasare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "ordin deplasare");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre deplasare");
                                break;
                            case 2002: /*decont avans spre decontare*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont avans spre decontare");
                                break;
                            case 2003: /*decont administrativ*/
                                entBCR.DetaliiPlata1 = string.Format(entBCR.DetaliiPlata1, "decont administrativ");
                                //entBCR.DetaliiPlata2 = string.Format(entBCR.DetaliiPlata2, "decont administrativ");
                                break;
                        }
                        #endregion
                        lstBCR.Add(entBCR);
                        docAllocatedOnDefaultBank = true;
                    }
                }
                #endregion

                var folder = new DirectoryInfo(HostingEnvironment.MapPath("~" + caleFisierBanca));
                if (!folder.Exists)
                    folder.Create();

                #region creare fisier BCR
                if (lstBCR.Count() != 0)
                {
                    int NumarFisierBanca = 1;
                    NumarFisierBanca = Dami.NextId("AvsXDec_fisierBCR", 1);
                    decimal sumaTotalPlata = lstBCR.Sum(p => p.SumaPlata);
                    int numarPlati = lstBCR.Count();
                    /*LeonardM 26.07.2016
                     * Eugen doreste sa se salveze cu TimeStamp
                     * old
                    string numeFisier = "BCR_" + NumarFisierBanca.ToString() + ".roi";
                     * */
                    string numeFisier = "BCR_" + DateTime.Now.ToString("yyyyMMdd HH_mm_ss") + ".roi";
                    /*end LeonardM 26.07.2016*/
                    roiFiles += numeFisier + ";";
                    string mesaj = string.Empty;
                    StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~" + caleFisierBanca) + numeFisier, true);
                    mesaj = ":01:" + lstBCR.FirstOrDefault().DataDocument.ToString("yyMMdd") + "01";
                    mesaj += "\r\n";
                    mesaj += ":02:" + string.Format("{0:0.00}", Convert.ToDouble(sumaTotalPlata)).Replace('.', ',');
                    mesaj += "\r\n";
                    mesaj += ":03:" + numarPlati.ToString();
                    mesaj += "\r\n";
                    /*LeonardM 15.10.2016
                     * pentru banca bcr trebuie sa fie default RNCB, deoarece prin BCR se fac si platile
                    * catre alte banci*/
                    mesaj += ":04:" + "RNCB";
                    mesaj += "\r\n";
                    mesaj += ":05:" + lstBCR.FirstOrDefault().NumeCompanie;
                    mesaj += "\r\n";
                    mesaj += lstBCR.FirstOrDefault().CodFiscalPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBCR.FirstOrDefault().StradaPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBCR.FirstOrDefault().OrasPlatitor;
                    mesaj += "\r\n";
                    mesaj += ":06:" + lstBCR.FirstOrDefault().CodUtilizator;
                    mesaj += "\r\n";
                    mesaj += ":07:" + numeFisier;
                    mesaj += "\r\n"; mesaj += "\r\n";

                    int nrCrt = 1;
                    foreach (metaAvsXDec_FisierBanca entBCR in lstBCR)
                    {
                        mesaj += ":20:" + nrCrt.ToString();
                        mesaj += "\r\n";
                        mesaj += ":32A:" + entBCR.executionDate.ToString("yyMMdd") + entBCR.CurrencyCode + string.Format("{0:0.00}", Convert.ToDouble(entBCR.SumaPlata)).Replace('.', ',');
                        mesaj += "\r\n";
                        mesaj += ":50:" + entBCR.NumeCompanie;
                        mesaj += "\r\n";
                        mesaj += entBCR.CodFiscalPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBCR.StradaPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBCR.OrasPlatitor;
                        mesaj += "\r\n";
                        mesaj += ":52A:/D/" + entBCR.IBANPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBCR.CodBancaPlatitoare;
                        mesaj += "\r\n";
                        mesaj += ":57A:" + entBCR.CodBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":57D:" + entBCR.NumeBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += entBCR.NumeSucursalaBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":59:/" + entBCR.IBANBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBCR.NumeAngajat;
                        mesaj += "\r\n";
                        mesaj += entBCR.EmailBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBCR.CNPBeneficiar;
                        mesaj += "\r\n";
                        mesaj += ":70:" + entBCR.DetaliiPlata1;
                        mesaj += "\r\n";
                        mesaj += entBCR.DetaliiPlata2;
                        mesaj += "\r\n"; mesaj += "\r\n";
                        /*mesaj += ":72:";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                         * */
                        nrCrt++;
                    }
                    //
                    sw.Write(mesaj);
                    sw.Close();
                    sw.Dispose();

                }
                #endregion

                #region creare fisier BRD
                if (lstBRD.Count() != 0)
                {
                    int NumarFisierBanca = 1;
                    NumarFisierBanca = Dami.NextId("AvsXDec_fisierBRD", 1);
                    decimal sumaTotalPlata = lstBRD.Sum(p => p.SumaPlata);
                    int numarPlati = lstBRD.Count();
                    /*LeonardM 26.07.2016
                     * Eugen doreste sa se salveze cu TimeStamp
                     * old
                    string numeFisier = "BRD_" + NumarFisierBanca.ToString() + ".roi";
                     * */
                    string numeFisier = "BRD_" + DateTime.Now.ToString("yyyyMMdd HH_mm_ss") + ".roi";
                    /*end LeonardM 26.07.2016*/
                    roiFiles += numeFisier + ";";
                    string mesaj = string.Empty;
                    StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~" + caleFisierBanca) + numeFisier, true);
                    mesaj = ":01:" + lstBRD.FirstOrDefault().DataDocument.ToString("yyMMdd") + "01";
                    mesaj += "\r\n";
                    mesaj += ":02:" + string.Format("{0:0.00}", Convert.ToDouble(sumaTotalPlata)).Replace('.', ',');
                    mesaj += "\r\n";
                    mesaj += ":03:" + numarPlati.ToString();
                    mesaj += "\r\n";
                    mesaj += ":04:" + lstBRD.FirstOrDefault().CodBancaBeneficiara;
                    mesaj += "\r\n";
                    mesaj += ":05:" + lstBRD.FirstOrDefault().NumeCompanie;
                    mesaj += "\r\n";
                    mesaj += lstBRD.FirstOrDefault().CodFiscalPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBRD.FirstOrDefault().StradaPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBRD.FirstOrDefault().OrasPlatitor;
                    mesaj += "\r\n";
                    mesaj += ":06:" + lstBRD.FirstOrDefault().CodUtilizator;
                    mesaj += "\r\n";
                    mesaj += ":07:" + numeFisier;
                    mesaj += "\r\n"; mesaj += "\r\n";

                    int nrCrt = 1;
                    foreach (metaAvsXDec_FisierBanca entBRD in lstBRD)
                    {
                        mesaj += ":20:" + nrCrt.ToString();
                        mesaj += "\r\n";
                        mesaj += ":32A:" + entBRD.executionDate.ToString("yyMMdd") + entBRD.CurrencyCode + string.Format("{0:0.00}", Convert.ToDouble(entBRD.SumaPlata)).Replace('.', ',');
                        mesaj += "\r\n";
                        mesaj += ":50:" + entBRD.NumeCompanie;
                        mesaj += "\r\n";
                        mesaj += entBRD.CodFiscalPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBRD.StradaPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBRD.OrasPlatitor;
                        mesaj += "\r\n";
                        mesaj += ":52A:/D/" + entBRD.IBANPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBRD.CodBancaPlatitoare;
                        mesaj += "\r\n";
                        mesaj += ":57A:" + entBRD.CodBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":57D:" + entBRD.NumeBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += entBRD.NumeSucursalaBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":59:/" + entBRD.IBANBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBRD.NumeAngajat;
                        mesaj += "\r\n";
                        mesaj += entBRD.EmailBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBRD.CNPBeneficiar;
                        mesaj += "\r\n";
                        mesaj += ":70:" + entBRD.DetaliiPlata1;
                        mesaj += "\r\n";
                        mesaj += entBRD.DetaliiPlata2;
                        mesaj += "\r\n"; mesaj += "\r\n";
                        /*mesaj += ":72:";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                         * */
                        nrCrt++;
                    }
                    //
                    sw.Write(mesaj);
                    sw.Close();
                    sw.Dispose();

                }
                #endregion

                #region creare fisier BT
                if (lstBT.Count() != 0)
                {
                    int NumarFisierBanca = 1;
                    NumarFisierBanca = Dami.NextId("AvsXDec_fisierBT", 1);
                    decimal sumaTotalPlata = lstBT.Sum(p => p.SumaPlata);
                    int numarPlati = lstBT.Count();
                    /*LeonardM 26.07.2016
                     * Eugen doreste sa se salveze cu TimeStamp
                     * old
                    string numeFisier = "BT_" + NumarFisierBanca.ToString() + ".roi";
                     * */
                    string numeFisier = "BT_" + DateTime.Now.ToString("yyyyMMdd HH_mm_ss") + ".roi";
                    /*end LeonardM 26.07.2016*/
                    roiFiles += numeFisier + ";";
                    string mesaj = string.Empty;
                    StreamWriter sw = new StreamWriter(HostingEnvironment.MapPath("~" + caleFisierBanca) + numeFisier, true);
                    mesaj = ":01:" + lstBT.FirstOrDefault().DataDocument.ToString("yyMMdd") + "01";
                    mesaj += "\r\n";
                    mesaj += ":02:" + string.Format("{0:0.00}", Convert.ToDouble(sumaTotalPlata)).Replace('.', ',');
                    mesaj += "\r\n";
                    mesaj += ":03:" + numarPlati.ToString();
                    mesaj += "\r\n";
                    mesaj += ":04:" + lstBT.FirstOrDefault().CodBancaBeneficiara;
                    mesaj += "\r\n";
                    mesaj += ":05:" + lstBT.FirstOrDefault().NumeCompanie;
                    mesaj += "\r\n";
                    mesaj += lstBT.FirstOrDefault().CodFiscalPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBT.FirstOrDefault().StradaPlatitor;
                    mesaj += "\r\n";
                    mesaj += lstBT.FirstOrDefault().OrasPlatitor;
                    mesaj += "\r\n";
                    mesaj += ":06:" + lstBT.FirstOrDefault().CodUtilizator;
                    mesaj += "\r\n";
                    mesaj += ":07:" + numeFisier;
                    mesaj += "\r\n"; mesaj += "\r\n";

                    int nrCrt = 1;
                    foreach (metaAvsXDec_FisierBanca entBT in lstBT)
                    {
                        mesaj += ":20:" + nrCrt.ToString();
                        mesaj += "\r\n";
                        mesaj += ":32A:" + entBT.executionDate.ToString("yyMMdd") + entBT.CurrencyCode + string.Format("{0:0.00}", Convert.ToDouble(entBT.SumaPlata)).Replace('.', ',');
                        mesaj += "\r\n";
                        mesaj += ":50:" + entBT.NumeCompanie;
                        mesaj += "\r\n";
                        mesaj += entBT.CodFiscalPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBT.StradaPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBT.OrasPlatitor;
                        mesaj += "\r\n";
                        mesaj += ":52A:/D/" + entBT.IBANPlatitor;
                        mesaj += "\r\n";
                        mesaj += entBT.CodBancaPlatitoare;
                        mesaj += "\r\n";
                        mesaj += ":57A:" + entBT.CodBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":57D:" + entBT.NumeBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += entBT.NumeSucursalaBancaBeneficiara;
                        mesaj += "\r\n";
                        mesaj += ":59:/" + entBT.IBANBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBT.NumeAngajat;
                        mesaj += "\r\n";
                        mesaj += entBT.EmailBeneficiar;
                        mesaj += "\r\n";
                        mesaj += entBT.CNPBeneficiar;
                        mesaj += "\r\n";
                        mesaj += ":70:" + entBT.DetaliiPlata1;
                        mesaj += "\r\n";
                        mesaj += entBT.DetaliiPlata2;
                        mesaj += "\r\n"; mesaj += "\r\n";
                        /*mesaj += ":72:";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                        mesaj += "\r\n";
                         * */
                        nrCrt++;
                    }
                    //
                    sw.Write(mesaj);
                    sw.Close();
                    sw.Dispose();

                }
                #endregion

                /*parcurgem lista si in functie de ce se afla punem pe 3 liste*/
                if (roiFiles.Length != 0)
                {
                    roiFiles = roiFiles.Substring(0, roiFiles.Length - 1);
                }

                #region Salvare atasament
                bool esteNou = false;
                string[] roiFilesGen = roiFiles.ToString().Split(';');
                foreach (string roi in roiFilesGen)
                {
                    esteNou = false;
                    DataTable entAtas = General.IncarcaDT("SELECT * FROM tblAtasamente Where Tabela = 'AvsXDec_FisiereBanca' AND FisierNume = '" +  roi + "'");
                    if (entAtas == null || entAtas.Rows.Count == 0)
                    {                       
                        esteNou = true;
                    }
                    string sql = "";
                    byte[] fisier;
                    MemoryStream ms = new MemoryStream();
                    File.OpenRead(HostingEnvironment.MapPath("~" + caleFisierBanca) + roi).CopyTo(ms);
                    fisier = ms.ToArray();
                    ms.Dispose();
                    ms = null;

                    if (esteNou)
                        sql = "INSERT INTO tblAtasamente (Tabela, Id, Fisier, FisierNume, FisierExtensie, USER_NO, TIME) VALUES ('AvsXDec_FisiereBanca', " + Dami.NextId("AvsXDec_fisierBT", 1) + ", '" + fisier 
                            + "', '" + roi + "', '.roi', " + General.Nz(Session["IdClient"], 1).ToString() + ", GETDATE())";
                    else
                        sql = "UPDATE tblAtasamente SET Fisier = '" + fisier + "', FisierNume = '" + roi + "', FisierExtensie = '.roi', USER_NO = " + General.Nz(Session["IdClient"], 1).ToString() 
                            + ", TIME = GETDATE() WHERE Tabela = 'AvsXDec_FisiereBanca' AND FisierNume = '" + roi + "'";

                    General.ExecutaNonQuery(sql, null);

                    fisier = null;
                    entAtas = null;
                }
                #endregion

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                roiFiles = string.Empty;
            }
            return roiFiles;
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {

                grDate.KeyFieldName = "DocumentId";
                        
                if (Session["AvsXDec_PaymentGrid"] == null)
                    dt = GetmetaVwAvsXDec_DocumenteRestPlataMetadata(Convert.ToInt32(cmbDocState.Value ?? -99), (cmbOperationSign.ClientVisible ? Convert.ToInt32(cmbOperationSign.Value ?? -99) : -99), Convert.ToInt32(txtPaymentDate.Value == null ? 1900 : Convert.ToDateTime(txtPaymentDate.Value).Year),
                        Convert.ToInt32(txtPaymentDate.Value == null ? 1 : Convert.ToDateTime(txtPaymentDate.Value).Month), Convert.ToInt32(txtPaymentDate.Value == null ? 1 : Convert.ToDateTime(txtPaymentDate.Value).Day), (cmbPaymentMethod.ClientVisible ? Convert.ToInt32(cmbPaymentMethod.Value ?? -99) : -99));
                else
                    dt = Session["AvsXDec_PaymentGrid"] as DataTable;

                dt.PrimaryKey = new DataColumn[] { dt.Columns["DocumentId"] };
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["AvsXDec_PaymentGrid"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametrii");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "IdStare", "USER_NO" });
                                if (lst == null || lst.Count() == 0)
                                {

                                    grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                                    return;
                                }

                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    object[] arr1 = lst[i] as object[];
                                    if (Convert.ToInt32(arr1[1] ?? "-99") == -1) return;          //nu anulezi o cerere deja anulata

                                    if (Convert.ToInt32(arr1[1] ?? "-99") == 0)
                                    {
                                        MessageBox.Show(Dami.TraduCuvant("Nu puteti anula un document respins!"), MessageBox.icoWarning, "");
                                        return;
                                    }

                                    if (Convert.ToInt32(Session["UserId"].ToString()) != Convert.ToInt32(arr1[2] ?? "-99"))
                                    {
                                        MessageBox.Show(Dami.TraduCuvant("Nu puteti anula un document care nu va apartine !"), MessageBox.icoWarning, "");
                                        return;
                                    }

                                    Document pag = new Document();
                                    string msg = pag.AnuleazaDocument(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(arr1[0] ?? "-99"), Convert.ToInt32(Session["User_Marca"].ToString()), arr[1]);

                                    if (msg.Length > 0 && msg == "OK")
                                    {
                                        MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes."), MessageBox.icoSuccess, "");
                                    }
                                }
                                Session["AvansXDecont_Grid"] = null;
                                IncarcaGrid();
                            }
                            break;
                        case "btnEdit":
                            DataTable dt = Session["AvsXDec_PaymentGrid"] as DataTable;
                            DataRow[] dr = dt.Select("DocumentId = " + arr[1]);

                            int DocumentId = Convert.ToInt32(dr[0]["DocumentId"].ToString());
                            int DocumentTypeId = Convert.ToInt32(dr[0]["DocumentTypeId"].ToString());
                            int f10003 = Convert.ToInt32((dr[0]["F10003"] == DBNull.Value ? -99 : dr[0]["F10003"]).ToString());
                            int userId = Convert.ToInt32(dr[0]["USER_NO"].ToString());
                            int idStare = Convert.ToInt32(dr[0]["DocumentStateId"] == DBNull.Value ? "0" : dr[0]["DocumentStateId"].ToString());
                            int poateModif = 0;

                            bool IsBudgetOwnerForDocument = Convert.ToInt32((dr[0]["IsBudgetOwnerForDocument"] == DBNull.Value ? 0 : dr[0]["IsBudgetOwnerForDocument"]).ToString()) == 0 ? false : true;
                            /*
							string descFormular = ent.DescFormular;
							string descPost = ent.DescPost;
							*/
                            bool poateAprobaXRefuzaDoc = false;
                            if ((idStare == 1 || idStare == 2) && f10003 != Convert.ToInt32(Session["User_Marca"].ToString()))
                                poateAprobaXRefuzaDoc = true;
                            string documentTypeName = dr[0]["DocumentTypeName"] == DBNull.Value ? "" : dr[0]["DocumentTypeName"].ToString();
                            DateTime documentDate = Convert.ToDateTime(dr[0]["DocumentDate"] == DBNull.Value ? "01/01/2100" : dr[0]["DocumentDate"].ToString());
                            /*pentru a aduce documentul sursa din care a fost generat
							 * folosit mai mult pentru deconturi care au ca sursa avans*/
                            int SrcDocId = Convert.ToInt32((dr[0]["SrcDocId"] == DBNull.Value ? -99 : dr[0]["SrcDocId"]).ToString());

                            switch (DocumentTypeId)
                            {
                                case 1001: /*Avans spre deplasare*/
                                case 1002: /*Avans spre decontare*/
                                case 1003: /*Avans Administrativ*/
                                    Session["AvsXDec_IdDocument"] = DocumentId;
                                    Session["AvsXDec_DocumentTypeId"] = DocumentTypeId;
                                    Session["AvsXDec_IdStare"] = idStare;
                                    Session["AvsXDec_Marca"] = f10003;
                                    Session["AvsXDec_DataVigoare"] = documentDate;
                                    //pag1.idRol = idRol;
                                    Session["AvsXDec_PoateModif"] = poateModif;
                                    if (idStare == 3)
                                        Session["AvsXDec_PoateAprobaXRefuzaDoc"] = 1;
                                    Session["AvsXDec_EsteNou"] = 0;
                                    //Session["AvsXDec_DocumentTypeId"]launchedPagedFrom = DocumentAvans.LansatDin.Formulare;
                                    //Session["AvsXDec_DocumentTypeId"]titlu = (barTitlu.Content ?? "").ToString();
                                    Session["AvsXDec_DocCanBeRefused"] = Convert.ToInt32((dr[0]["canBeRefused"] == DBNull.Value ? 0 : dr[0]["canBeRefused"]).ToString());

                                    string url = "~/AvansXDecont/DocumentAvans";
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                    break;
                                /*Decont cheltuieli deplasare*/
                                case 2001:
                                /*Decont cheltuieli*/
                                case 2002:
                                /*Decont administrativ*/
                                case 2003:
                                    //AvansXDecont.DocumentDecont pag2 = new AvansXDecont.DocumentDecont();

                                    Session["AvsXDec_IdDocument"] = DocumentId;
                                    Session["AvsXDec_DocumentTypeId"] = DocumentTypeId;
                                    Session["AvsXDec_IdStare"] = idStare;
                                    Session["AvsXDec_Marca"] = f10003;
                                    //pag1.idRol = idRol;
                                    Session["AvsXDec_PoateModif"] = poateModif;                                   
                                    Session["AvsXDec_EsteNou"] = 0;
                                    Session["AvsXDec_SrcDocId"] = SrcDocId;
                                    Session["AvsXDec_UserId"] = userId;
                                    Session["AvsXDec_IsBudgetOwnerEdited"] = IsBudgetOwnerForDocument;
                                    //Session["AvsXDec_DocumentTypeId"]launchedPagedFrom = DocumentAvans.LansatDin.Formulare;
                                    //Session["AvsXDec_DocumentTypeId"]titlu = (barTitlu.Content ?? "").ToString();
                                    /*cand avansul, decontul este in starea aprobat, atunci se mai poate respinge*/
                                    if (idStare == 3)
                                    {
                                        Session["AvsXDec_PoateAprobaXRefuzaDoc"] = 1;
                                        Session["AvsXDec_DocCanBeRefused"] = 1;
                                    }
                                    else
                                        Session["AvsXDec_DocCanBeRefused"] = Convert.ToInt32((dr[0]["canBeRefused"] == DBNull.Value ? 0 : dr[0]["canBeRefused"]).ToString());

                                    url = "~/AvansXDecont/DocumentDecont";
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);
                                    break;
                                case 1004: /*Lista Avansuri Administrative*/
                                    //AvansXDecont.DocumentAvansAdministrativ pag3 = new DocumentAvansAdministrativ();
                                    //pag3.idDocument = DocumentId;
                                    //pag3.documentTypeId = DocumentTypeId;
                                    //pag3.esteNou = false;
                                    //pag3.idStare = idStare;
                                    //if (Constante.UserId == userId && idStare == 0)
                                    //	pag3.poateModif = 1;
                                    //else
                                    //	pag3.poateModif = poateModif;
                                    //pag3.launchedPagedFrom = DocumentAvansAdministrativ.LansatDin.Formulare;
                                    //pag3.titlu = (barTitlu.Content ?? "").ToString();
                                    //pnl.Content = pag3;
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "DocumentState")
                {
                    DataTable dt = Session["AvsXDec_Stari"] as DataTable;

                    string stare = e.GetValue("DocumentState").ToString();
                    DataRow[] lst = dt.Select("Denumire='" + stare + "'");
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_PaymentGrid"] as DataTable;
                DataRow row = dt.Rows.Find(keys);
                int tip = -1, id = -1;
                string data = "";
                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName == "PaymentDate")
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                        data = e.NewValues[col.ColumnName].ToString();
                    }
                    if (col.ColumnName == "DocumentTypeId")
                        tip = Convert.ToInt32(row[col.ColumnName].ToString());
                    if (col.ColumnName == "DocumentId")
                        id = Convert.ToInt32(e.NewValues[col.ColumnName].ToString());
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["AvsXDec_PaymentGrid"] = dt;
                grDate.DataSource = dt;

                if (data.Length > 0)
                    switch (tip)
                    {
                        case 1001:
                        case 1002:
                            {
                                General.ExecutaNonQuery("UPDATE AvsXDec_Avans SET PaymentDate = CONVERT(DATETIME, '" + data + "', 103) WHERE DocumentId = " + id, null);
                                break;
                            }
                        case 2001:
                        case 2002:
                            {
                                General.ExecutaNonQuery("UPDATE AvsXDec_Decont SET PaymentDate =  CONVERT(DATETIME, '" + data + "', 103) WHERE DocumentId = " + id, null);
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_CustomButtonInitialize(object sender, ASPxGridViewCustomButtonEventArgs e)
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

        protected void grDate_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                StareDocumentForUpdate docStateUpdate = (StareDocumentForUpdate)Session["AvsXDec_DocStateUpdate"];
                if (e.VisibleIndex >= 0)
                {
                    object[] obj = grDate.GetRowValues(e.VisibleIndex, new string[] { "DocumentId", "PaymentDate" }) as object[];               

                    if (obj != null)
                    {
                        switch (docStateUpdate)
                        {
                            case StareDocumentForUpdate.Acordat:
                            case StareDocumentForUpdate.Restituit:
                            case StareDocumentForUpdate.TrimisLaBanca:
                            case StareDocumentForUpdate.Contat:
                            case StareDocumentForUpdate.Inchis:
                                if (e.ButtonType == ColumnCommandButtonType.Delete)
                                    e.Visible = false;
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



        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
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



        public DataTable GetmetaVwAvsXDec_DocumenteRestPlataMetadata(int documentStateId = -99, int operationSignId = -99, int yearPaymentDate = 1900, int monthPaymentDate = 1, int dayPaymentDate = 1, int paymentTypeId = -99)
        {
            DataTable q = null;
            string sql = "";
            string condData = new DateTime(yearPaymentDate, monthPaymentDate, dayPaymentDate) != new DateTime(1900, 1, 1) ? "CONVERT(DATETIME, '" + dayPaymentDate + "/" + monthPaymentDate + "/" + yearPaymentDate + "', 103)" : "a.PaymentDate";

            try 
            {
                /*LeonardM 11.09.2016
                 * in momentul in care utilizatorul logat este diferit de cel care a initiat documentul => doc poate fi respins
                 * daca documentul este avans si starea este mai mica de 4 => doc poate fi respins
                 * daca documentul este decont si starea este mai mica de 7 => doc poate fi respins
                 * */               

                sql = "SELECT  a.DocumentId, fNume.F10008 + ' ' + fNume.F10009 as NumeComplet, a.DocumentStateId,  a.DocumentState, a.DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName, a.DocumentDate, a.TotalAmount, "
                    + " a.CurrencyId, a.CurrencyCode, (CASE WHEN a.UnconfRestAmount is null or a.UnconfRestAmount = 0 THEN a.TotalAmount + a.TotalComissionPayment ELSE a.UnconfRestAmount + a.TotalComissionPayment END) as UnconfRestAmount, "
                    + " a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, a.TotalComissionPayment, a.PaymentCurrencyId, a.PaymentCurrencyCode, " + condData + " as PaymentDate, srcDoc.SrcDocId, COALESCE(srcDoc.SrcDocAmount, -99) AS SrcDocAmount, "
                    + " COALESCE(srcDoc.DestDocAmount, -999) AS DestDocAmount, doc.F10003, doc.USER_NO, COALESCE(a.PaymentTypeId, -99) AS PaymentTypeId,  a.PaymentTypeName, a.BankIdPlatitor,  a.BankCodePlatitor, a.BankNamePlatitor, "
                    + " a.IBANPlatitor, CASE WHEN ((a.DocumentTypeId IN (1001, 1002, 1003) AND a.DocumentStateId < 4) OR (a.DocumentTypeId IN (2001, 2002, 2003) AND a.DocumentStateId < 7)) THEN 1 ELSE 0 END as canBeRefused "
                    + " FROM vwAvsXDec_DocumenteRestPlata a "
                    + " JOIN AvsXDec_Document doc  on a.DocumentId = doc.DocumentId "
                    + " JOIN F100 fNume on doc.F10003 = fNume.F10003 "
                    + " LEFT JOIN AvsXDec_BusinessTransaction srcDoc on a.DocumentId = srcDoc.DestDocId ";     
                
                q = General.IncarcaDT(sql, null);

                if (q != null && q.Rows.Count > 0)
                {
                    if (documentStateId != -99)
                    {
                        /*LeonardM 10.08.2016
                     * in cazul in care filtreaza financiarul (documente in starea 3 => aprobat si 4 => trimis la banca
                     * pentru deconturile care s-au inchis exact cu suma de pe avansul sursa, acestea se exclud din filtrare
                         LeonardM 23.08.2016
                         avansurile administrative trebuie aduse in lista, chiar daca sunt generate cu aceeasi suma din lista
                         de avansuri administrative*/
                        if (documentStateId == 3 || documentStateId == 4)
                        {
                            if (q.Select("DocumentStateId = " + documentStateId + " AND ((SrcDocAmount <> DestDocAmount AND DocumentTypeId <> 1003) OR (SrcDocAmount = DestDocAmount AND DocumentTypeId = 1003))") != null &&
                                q.Select("DocumentStateId = " + documentStateId + " AND ((SrcDocAmount <> DestDocAmount AND DocumentTypeId <> 1003) OR (SrcDocAmount = DestDocAmount AND DocumentTypeId = 1003))").Length > 0)
                                q = q.Select("DocumentStateId = " + documentStateId + " AND ((SrcDocAmount <> DestDocAmount AND DocumentTypeId <> 1003) OR (SrcDocAmount = DestDocAmount AND DocumentTypeId = 1003))").CopyToDataTable();
                            else
                                q.Rows.Clear();
                        }
                        else
                        {
                            if (q.Select("(DocumentStateId = " + documentStateId + ") OR (DocumentStateId = 3 AND SrcDocAmount = DestDocAmount AND " + documentStateId + " = 5)") != null &&
                                q.Select("(DocumentStateId = " + documentStateId + ") OR (DocumentStateId = 3 AND SrcDocAmount = DestDocAmount AND " + documentStateId + " = 5)").Length > 0)
                                q = q.Select("(DocumentStateId = " + documentStateId + ") OR (DocumentStateId = 3 AND SrcDocAmount = DestDocAmount AND " + documentStateId + " = 5)").CopyToDataTable();
                            else
                                q.Rows.Clear();
                        }

                        /*LeonardM 17.08.2016
                         * in cazul in care se doresc a fi aduse documente in starea inchis atunci, se aduc doar
                         * documentele de tip decont care trebuie inchise prin bifa de documente originale*/
                        if (q.Rows.Count > 0 && documentStateId == 7)
                        {
                            if (q.Select("DocumentTypeId IN (2001, 2002, 2003") != null && q.Select("DocumentTypeId IN (2001, 2002, 2003").Length > 0)
                                q = q.Select("DocumentTypeId IN (2001, 2002, 2003").CopyToDataTable();
                            else
                                q.Rows.Clear();
                        }
                        /*end LeonardM 10.08.2016*/
                    }
                    if (q.Rows.Count > 0)
                    {
                        switch (operationSignId)
                        {
                            /*LeonardM 16.08.2016
                             * semnul se ia in considerare doar pentru stare diferit pentru contare*/
                            case -1:
                                if (documentStateId != 5 && documentStateId != 6 && documentStateId != 7)
                                {
                                    if (q.Select("UnconfRestAmount < 0") != null && q.Select("UnconfRestAmount < 0").Length > 0)
                                        q = q.Select("UnconfRestAmount < 0").CopyToDataTable();
                                    else
                                        q.Rows.Clear();
                                }
                                break;
                            case +1:
                                if (documentStateId != 5 && documentStateId != 6 && documentStateId != 7)
                                {
                                    if (q.Select("UnconfRestAmount >= 0") != null && q.Select("UnconfRestAmount >= 0").Length > 0)
                                        q = q.Select("UnconfRestAmount >= 0").CopyToDataTable();
                                    else
                                        q.Rows.Clear();
                                }
                                break;
                        }
                    }

                    if (q.Rows.Count > 0 && paymentTypeId != -99)
                    {
                        if (q.Select("PaymentTypeId = " + paymentTypeId) != null && q.Select("PaymentTypeId = " + paymentTypeId).Length > 0)
                            q = q.Select("PaymentTypeId = " + paymentTypeId).CopyToDataTable();
                        else
                            q.Rows.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        protected void cmbDocState_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStare();
        }

        protected void cmbOperationSign_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStare();
        }

        protected void cmbPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetStare();
        }

        protected void SetStare()
        {
            StareDocumentForUpdate docStateUpdate = new StareDocumentForUpdate();
            switch (Convert.ToInt32(cmbDocState.Value))
            {
                //momentan nu se foloseste starea Contat
                case 3:/*aprobat*/
                    if (Convert.ToInt32(cmbOperationSign.Value ?? -99) == -1)
                        docStateUpdate = StareDocumentForUpdate.Restituit;
                    else if (Convert.ToInt32(cmbPaymentMethod.Value ?? -99) != Convert.ToInt32(General.Nz(Session["IdModalitatePlataCash"], -99)))
                        docStateUpdate = StareDocumentForUpdate.TrimisLaBanca;
                    else
                        docStateUpdate = StareDocumentForUpdate.Acordat;
                    break;
                case 4:/*Trimis la banca*/
                    docStateUpdate = StareDocumentForUpdate.Acordat;
                    break;
                case 6: /*restituit*/
                case 5:/*acordat*/
                    /* a fost completat avansul si pus sumele din extras, trebuie generate notele contabile*/
                    //docStateUpdate = StareDocumentForUpdate.Contat;
                    docStateUpdate = StareDocumentForUpdate.Inchis;
                    break;
                case 7:/*contat*/
                    /*a fost contate documentele si se doresc a fi inchise,
                     * Acest caz este valabil doar pentru documentele de tip decont, unde se pun bifele de documente originale*/
                    docStateUpdate = StareDocumentForUpdate.Inchis;
                    break;
            }
            Session["AvsXDec_DocStateUpdate"] = docStateUpdate;
            SetButtonContent();
            Session["AvsXDec_PaymentGrid"] = null;
            IncarcaGrid();
        }

        protected void grDate_DataBound(object sender, EventArgs e)
        {
            for (int i = 0; i < grDate.VisibleRowCount; i++)
            {
                DataRowView values = grDate.GetRow(i) as DataRowView;
                int documentStateId = Convert.ToInt32(values.Row["DocumentStateId"].ToString());
                /*cand starea documentului este acordat/restituit, atunci se genereaza note contabile*/
                if (documentStateId == 5 || documentStateId == 6)
                {
                    grDate.Selection.SetSelection(i, true);
                }
            }
        }
    }
}