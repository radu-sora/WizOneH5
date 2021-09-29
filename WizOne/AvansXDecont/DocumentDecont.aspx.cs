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
using DevExpress.Web.Data;

namespace WizOne.AvansXDecont
{
    public partial class DocumentDecont : System.Web.UI.Page
    {

        public class metaCereriDate
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

				btnPrint.Text = Dami.TraduCuvant("btnPrint", "Imprima");
                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aprobare");
                btnRespins.Text = Dami.TraduCuvant("btnRespins", "Respinge");				
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnDocOrig.Text = Dami.TraduCuvant("btnDocOrig", "Documente originale");

                lblNrOrdin.InnerText = Dami.TraduCuvant("Nr. decont");
				lblNume.InnerText = Dami.TraduCuvant("Nume");
				lblDept.InnerText = Dami.TraduCuvant("Departament");
				lblLocMunca.InnerText = Dami.TraduCuvant("Loc munca");
				lblIBAN.InnerText = Dami.TraduCuvant("IBAN");
                lblAvs.InnerText = Dami.TraduCuvant("Avansuri");
                lblDocAvs.InnerText = Dami.TraduCuvant("Avans");
                lblLocDepl.InnerText = Dami.TraduCuvant("Loc");
				lblTipDepl.InnerText = Dami.TraduCuvant("Tip deplasare");
				lblTipTrans.InnerText = Dami.TraduCuvant("Tip transport");
				lblMotiv.InnerText = Dami.TraduCuvant("Motiv deplasare");
				lblDtPlec.InnerText = Dami.TraduCuvant("Data plecare");
				lblOraPlec.InnerText = Dami.TraduCuvant("Ora plecare");
				lblDtSos.InnerText = Dami.TraduCuvant("Data sosire");
				lblDtSf.InnerText = Dami.TraduCuvant("Ora sosire");
				lblMoneda.InnerText = Dami.TraduCuvant("Moneda");
				lblModPlata.InnerText = Dami.TraduCuvant("Modalitate plata");
				lblDiurna.InnerText = Dami.TraduCuvant("Diurna");
                lblValDec.InnerText = Dami.TraduCuvant("Val. decontata");
                lblValAvs.InnerText = Dami.TraduCuvant("Val. avans");
                lblPlRec.InnerText = Dami.TraduCuvant("Val. plata/recuperat");

                pnlDateGen.HeaderText = Dami.TraduCuvant("Date generale");		
				pnlDateDepl.HeaderText = Dami.TraduCuvant("Date deplasare");	
				pnlDatePlata.HeaderText = Dami.TraduCuvant("Date plata");
                pnlDocJust.HeaderText = Dami.TraduCuvant("Documente justificative");
                pnlEstChelt.HeaderText = Dami.TraduCuvant("Estimare cheltuieli");
                pnlDateDec.HeaderText = Dami.TraduCuvant("Date decont");
                pnlPlataBanca.HeaderText = Dami.TraduCuvant("Restituire avans neutilizat");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString() + " / Document Decont"; ;

                if (!IsPostBack)
                {
                    Session["AvsXDec_SursaDateDec"] = null;
                    Session["AvsXDec_SursaDateCheltuieli"] = null;

                    DataTable lstConfigCurrencyXPay_Currency = new DataTable();
                    lstConfigCurrencyXPay_Currency.Columns.Add("DictionaryItemId", typeof(int));
                    lstConfigCurrencyXPay_Currency.Columns.Add("DictionaryItemName", typeof(string));
                    lstConfigCurrencyXPay_Currency.Columns.Add("Culoare", typeof(string));
                    lstConfigCurrencyXPay_Currency.Columns.Add("DictionaryId", typeof(int));
                    Session["ConfigCurrencyXPay_Currency"] = lstConfigCurrencyXPay_Currency;

                    DataTable lstConfigCurrencyXPay_Pay = new DataTable();
                    lstConfigCurrencyXPay_Pay.Columns.Add("DictionaryItemId", typeof(int));
                    lstConfigCurrencyXPay_Pay.Columns.Add("DictionaryItemName", typeof(string));
                    lstConfigCurrencyXPay_Pay.Columns.Add("Culoare", typeof(string));
                    lstConfigCurrencyXPay_Pay.Columns.Add("DictionaryId", typeof(int));
                    Session["ConfigCurrencyXPay_Pay"] = lstConfigCurrencyXPay_Pay;

                    DataTable lstConfigCurrencyXPay_PayCopy = new DataTable();
                    lstConfigCurrencyXPay_PayCopy.Columns.Add("DictionaryItemId", typeof(int));
                    lstConfigCurrencyXPay_PayCopy.Columns.Add("DictionaryItemName", typeof(string));
                    lstConfigCurrencyXPay_PayCopy.Columns.Add("Culoare", typeof(string));
                    lstConfigCurrencyXPay_PayCopy.Columns.Add("DictionaryId", typeof(int));
                    Session["ConfigCurrencyXPay_PayCopy"] = lstConfigCurrencyXPay_PayCopy;
                }
                IncarcaDate();             


                DataTable lstConfigCurrencyXPay = GetAvsXDec_ConfigCurrencyXPay(Convert.ToInt32(Session["AvsXDec_Marca"].ToString()), Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                Session["ConfigCurrencyXPay"] = lstConfigCurrencyXPay;
                DataTable lpTipDeplasare = GetAvsXDec_DictionaryItemTipDeplasare();
				DataTable lpTipMoneda = GetAvsXDec_DictionaryItemValute();
				DataTable lpModPlata = GetAvsXDec_DictionaryItemModalitatePlata();
               
                DataTable lstAvsXDec_DocTypeXUser = GetAvsXDec_DocTypeXUser(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()), Convert.ToInt32((Session["AvsXDec_Marca"] ?? -99).ToString()));


                if (lpTipDeplasare != null && lpTipDeplasare.Rows.Count > 0)
				{
				   
					if (lpTipMoneda != null && lpTipMoneda.Rows.Count > 0)
					{
                        if (lstAvsXDec_DocTypeXUser.Rows.Count != 0)
                        {
                            string avansFolosit = "Nr. {0}; Stare {1}; Valoare: {2} {3}";
                            avansFolosit = string.Format(avansFolosit, lstAvsXDec_DocTypeXUser.Rows[0]["DocumentId"].ToString(),
                                                    lstAvsXDec_DocTypeXUser.Rows[0]["DocumentState"].ToString(),
                                                    lstAvsXDec_DocTypeXUser.Rows[0]["TotalDocument"].ToString(),
                                                    lstAvsXDec_DocTypeXUser.Rows[0]["CurrencyCode"].ToString());
                            txtAvansFolosit.Text = avansFolosit;
                        }
                        else
                        {
                            txtAvansFolosit.Text = "Nu exista avansuri initiate!";
                        }

                        if (lpModPlata != null && lpModPlata.Rows.Count > 0)
						{

							#region ModPlata
							cmbModPlata.DataSource = lpModPlata;
                            cmbModPlata.DataBind();

                            //if (lpModPlata.Select("LOWER(DictionaryItemName) LIKE (%bancar%)").Count() != 0)
                            //	IdModPlata_TransferBancar = lpModPlata.Select("LOWER(DictionaryItemName) LIKE (%bancar%)")[0]["DictionaryItemId"].ToString();

                            //if (lpModPlata.Select("LOWER(DictionaryItemName) LIKE (%cash%)").Count() != 0)
                            //	IdModPlata_PlataCash = lpModPlata.Select("LOWER(DictionaryItemName) LIKE (%cash%)")[0]["DictionaryItemId"].ToString();

                            #region aranjare modalitati plata default
                            /*LeonardM 13.06.2016
					        * la nivel de setari de nomenclator, se completeaza si ordinea in care se doresc a
					        * fi selectate valorile si default vine completat cea mai mica valoare ca ordine
					        * */


                            int minimumOrderModPlata = Convert.ToInt32(lpModPlata.AsEnumerable().Min(row => row["Ordine"]));
							if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1)
							{
								if (Convert.ToInt32(General.Nz(Session["IdModPlataDefault"], -99)) == -99)
								{
                                    Session["IdModPlataDefault"] = lpModPlata.Select("Ordine = " + minimumOrderModPlata)[0]["DictionaryItemId"].ToString();
									cmbModPlata.Value = Convert.ToInt32(Session["IdModPlataDefault"].ToString());
								}
								else
								{
									cmbModPlata.Value = Convert.ToInt32(Session["IdModPlataDefault"].ToString());
                                }                                                                    
							}
							#endregion
							
							#endregion

							#region valute
							cmbMonedaAvans.DataSource = lpTipMoneda;
                            cmbMonedaAvans.DataBind();
                            if (lpTipMoneda != null && lpTipMoneda.Rows.Count != 0)
						    {
                                lpTipMoneda.CaseSensitive = false;
                                if (lpTipMoneda.Select("DictionaryItemName LIKE ('%ron%')").Count() != 0)
                                    Session["IdMonedaRON"] = lpTipMoneda.Select("DictionaryItemName LIKE ('%ron%')")[0]["DictionaryItemId"].ToString();
						    }
							#region aranjare tip moneda default
							/*LeonardM 13.06.2016
						    * la nivel de setari de nomenclator, se completeaza si ordinea in care se doresc a
						    * fi selectate valorile si default vine completat cea mai mica valoare ca ordine
						    * */
							int minimumOrderMoneda = Convert.ToInt32(lpTipMoneda.AsEnumerable().Min(row => row["Ordine"]));
                            if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1)
							{
                                if (Convert.ToInt32(General.Nz(Session["IdMonedaDefault"], -99)) == -99) 
								{
                                    Session["IdMonedaDefault"] = lpTipMoneda.Select("Ordine = " + minimumOrderMoneda)[0]["DictionaryItemId"].ToString();
									cmbMonedaAvans.Value = Convert.ToInt32(Session["IdMonedaDefault"].ToString()); 
								}
								else
								{
									cmbMonedaAvans.Value = Convert.ToInt32(Session["IdMonedaDefault"].ToString());
                                }
							}
							#endregion
							
							#endregion

							#region tip deplasare
							cmbActionType.DataSource = lpTipDeplasare;
                            cmbActionType.DataBind();


                            if (lpTipDeplasare != null && lpTipDeplasare.Rows.Count != 0)
							{
                                lpTipDeplasare.CaseSensitive = false;
                                if (lpTipDeplasare.Select("DictionaryItemName LIKE ('%interna%')").Count() != 0)
                                    Session["IdDeplasareInterna"] = lpTipDeplasare.Select("DictionaryItemName LIKE ('%interna%')")[0]["DictionaryItemId"].ToString();
                                lpTipDeplasare.CaseSensitive = false;
                                if (lpTipDeplasare.Select("DictionaryItemName LIKE ('%externa%')").Count() != 0)
                                    Session["IdDeplasareExterna"] = lpTipDeplasare.Select("DictionaryItemName LIKE ('%externa%')")[0]["DictionaryItemId"].ToString();

								#region aranjare tip deplasare default
								/*LeonardM 13.06.2016
						         * la nivel de setari de nomenclator, se completeaza si ordinea in care se doresc a
						         * fi selectate valorile si default vine completat cea mai mica valoare ca ordine
						         * */
								int minimumOrderTipDeplasare = Convert.ToInt32(lpTipDeplasare.AsEnumerable().Min(row => row["Ordine"]));
                                if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1)
								{
                                    if (Convert.ToInt32(General.Nz(Session["IdDeplasareDefault"], -99)) == -99) 
									{
                                        Session["IdDeplasareDefault"] = lpTipDeplasare.Select("Ordine = " + minimumOrderTipDeplasare)[0]["DictionaryItemId"].ToString();
										cmbActionType.Value = Convert.ToInt32(Session["IdDeplasareDefault"].ToString()); 
									}
									else
									{
										cmbActionType.Value = Convert.ToInt32(Session["IdDeplasareDefault"].ToString());
                                    }
								}
								#endregion
							}
							#endregion

							#region incarcare date



							#endregion
						}							
					}
				}

                DataTable dtTransp = General.IncarcaDT("SELECT a.* FROM vwAvsXDec_Nomen_TipTransport a JOIN AvsXDec_DictionaryItem b on a.DictionaryItemId = b.DictionaryItemId ORDER BY COALESCE(b.Ordine, -99)", null);
                cmbTransportType.DataSource = dtTransp;
                cmbTransportType.DataBind();
 

                if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0 && (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 || (Convert.ToInt32(Session["AvsXDec_IdStare"].ToString()) != 1)))          //are doar drepturi de vizualizare
                {
                    //ctlGeneral.IsEnabled = false;
                    txtLocatie.ClientEnabled = false;
                    cmbActionType.ClientEnabled = false;
					cmbTransportType.ClientEnabled = false;
                    txtActionReason.ClientEnabled = false;
                    txtStartDate.ClientEnabled = false;
                    txtOraPlecare.ClientEnabled = false;
                    txtEndDate.ClientEnabled = false;
                    txtOraSosire.ClientEnabled = false;
                    cmbMonedaAvans.ClientEnabled = false;
                    cmbDocAvans.ClientEnabled = false;
                    cmbModPlata.ClientEnabled = false;
                    chkIsDiurna.ClientEnabled = false;
                    grDateEstChelt.Enabled = false;

                    btnSave.ClientEnabled = false;
                    //dreptUserAccesFormular = tipAccesPagina.formularSalvat;
                }
                //else if (!esteNou)
                //    dreptUserAccesFormular = tipAccesPagina.formularSalvatEditUser;
                //else
                //    dreptUserAccesFormular = tipAccesPagina.formularNou;

                #region drept aprobare/refuz document

                btnAproba.ClientEnabled = Convert.ToInt32(General.Nz(Session["AvsXDec_PoateAprobaXRefuzaDoc"], 0).ToString()) == 1 &&
                   Convert.ToInt32(General.Nz(Session["AvsXDec_IsBudgetOwnerEdited"], 0).ToString()) == 1 ? false : (Convert.ToInt32(General.Nz(Session["AvsXDec_PoateAprobaXRefuzaDoc"], 0).ToString()) == 1 ? true : false);
                btnRespins.ClientEnabled = Convert.ToInt32(General.Nz(Session["AvsXDec_DocCanBeRefused"], 0).ToString()) == 1 ? true : false;

                #endregion

                #region bifa documente originale
                //chkDecontDocPlataBancaoriginale.Visibility = System.Windows.Visibility.Collapsed;
                btnDocOrig.ClientEnabled = Constante.AreRolContabilitate;
                if (btnDocOrig.Content == statusDocOriginale)
                    btnDocOrig.ClientEnabled = false;
                /*LeonardM 15.08.2016
                 * avem nevoie de acest buton, deoarece supervizorul trebuie sa poata salva linia de buget*/
                if (Constante.AreRolContabilitate || IsBudgetOwnerEdited)
                    btnSave.ClientEnabled = true;
                /*end LeonardM 15.08.2016*/
                #endregion

                PageXDocumentType();

                string AvsXDec_TipDocument_Diurna = Dami.ValoareParam("AvsXDec_TipDocument_Diurna", "");
                string AvsXDec_BudgetLine_Diurna2001 =Dami.ValoareParam("AvsXDec_BudgetLine_Diurna2001", "");


                DataTable dtCh = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                GridViewDataComboBoxColumn colCh = (grDate.Columns["DictionaryItemId"] as GridViewDataComboBoxColumn);
                colCh.PropertiesComboBox.DataSource = dtCh;

                DataTable dtCheltuieli = new DataTable();
                if (!IsPostBack)
                {
                    dtCheltuieli = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                    Session["AvsXDec_SursaDateCheltuieli"] = dtCheltuieli;
                }
                else
                {
                    dtCheltuieli = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;                    
                }
                grDate.KeyFieldName = "DocumentDetailId;DocumentId";
                grDate.DataSource = dtCheltuieli;
                grDate.DataBind();


                IncarcaCheltuieli();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void DrepturiCtl()
        {
            try
            {
                /*06.06.2016
                 * gridul de cheltuieli nu este activ, deoarece acestea vin din avans*/
                grDateEstChelt.Enabled = false;
            }
            catch (Exception)
            {
            }
        }

        public DataTable GetAvsXDec_DocTypeXUser(int documentTypeId, int F10003)
        {
            try
            {
                DataTable q = null;
                int docTypeId = -99;

                switch (documentTypeId)
                {
                    case 2001: /*Avans spre deplasare*/
                        docTypeId = 1001;
                        break;
                    case 2002: /* Avans spre decontare*/
                        docTypeId = 1002;
                        break;
                }
                string docAcordat = "SELECT COALESCE(a.SrcDocId, -99) AS DocumentId FROM AvsXDec_BusinessTransaction a "
                            + " JOIN vwAvsXDec_Decont b on a.DestDocId = b.DocumentId "
                            + " WHERE b.DocumentStateId >= 1 ";               

                string sql = "SELECT a.DocumentId, a.DocumentStateId, a.DocumentState, COALESCE(a.TotalAmount, 0) AS TotalDocument, a.CurrencyCode "
                    + " FROM vwAvsXDec_Avans a "
                    + " LEFT JOIN AvsXDec_BusinessTransaction b on a.DocumentId = b.SrcDocId "
                    + " WHERE a.DocumentTypeId = " + docTypeId + " AND a.DocumentStateId >= 1 AND a.DocumentStateId < 8 AND a.F10003 = " + F10003 + " AND COALESCE(b.SrcDocId, -99) = -99 "
                    + " UNION "
                    + "SELECT a.DocumentId, a.DocumentStateId, a.DocumentState, COALESCE(a.TotalAmount, 0) AS TotalDocument, a.CurrencyCode "
                    + " FROM vwAvsXDec_Avans a "
                    + " JOIN AvsXDec_BusinessTransaction b on a.DocumentId = b.SrcDocId "
                    + " JOIN vwAvsXDec_Decont c on b.DestDocId = c.DocumentId "
                    + " LEFT JOIN (" + docAcordat + ") d on a.DocumentId = d.DocumentId "
                    + " where c.DocumentStateId < 1 AND a.DocumentTypeId == 1001 AND a.DocumentStateId >= 1 AND a.DocumentStateId < 8 AND a.F10003 = " + F10003 + " AND d.DocumentId IS NULL";
                q = General.IncarcaDT(sql, null);

                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


        private void PageXDocumentType()
        {
            switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
            {
                case 2001:/*Decont cheltuieli deplasare*/
                case 2002:/*Decont cheltuieli*/
                case 2003: /*Decont Administrativ*/
                    grDateEstChelt.Enabled = false;
                    /*LeonardM 15.08.2016
                     * coloana de currency de pe docuemnte justificative nu se doreste a fi editabila, 
                     * aceasta intializandu-se cu moneda default aleasa inainte pe avans*/

                    //grDateDocJust.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;

                    /*end LeonardM 15.08.2016*/

                    /*LeonardM 12.08.2016
                     * utilizatorul care creeaza documentul nu are drept de editare pe coloana linie buget,
                     * ci doar cei de pe circuit (solicitare GroupamA)*/

                    //if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                    //{
                    //    #region documente justificative

                    //    if (IsBudgetOwnerEdited && idStare != 0)    //Radu 31.08.2016
                    //        grDateDocJust.Columns["BugetLine"].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    else
                    //        grDateDocJust.Columns["BugetLine"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colSterge*/
                    //    grDateDocJust.Columns[1].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colSel*/
                    //    grDateDocJust.Columns[0].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["DocumentId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["DocumentDetailId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["Furnizor"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["DictionaryItemId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["DocNumberDecont"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["DocDateDecont"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["TotalPayment"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["ExpenseTypeId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["ExpenseTypeAdmId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    //grDateDocJustifDecont.Columns["colDocumente"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["IdDocument"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDateDocJust.Columns["FreeTxt"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colDocumente*/
                    //    grDateDocJust.Columns[13].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    #endregion

                    //    #region documente plata banca
                    //    grDatePlataBanca.Enabled = true;
                    //    /*colSelPlataBanca*/
                    //    grDatePlataBanca.Columns[0].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colStergePlataBanca*/
                    //    grDatePlataBanca.Columns[1].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["DocumentId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["DocumentDetailId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["DictionaryItemId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["DocNumberDecont"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["DocDateDecont"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    grDatePlataBanca.Columns["TotalPayment"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colDocumentePlataBanca*/
                    //    grDatePlataBanca.Columns[9].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    grDatePlataBanca.Columns["IdDocument"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    #endregion
                    //}
                    //else
                    //{
                    //    #region Documente justificative
                    //    /*LeonardM 15.08.2016
                    //     * coloana de currency de pe docuemnte justificative nu se doreste a fi editabila, 
                    //     * aceasta intializandu-se cu moneda default aleasa inainte pe avans*/
                    //    grDateDocJust.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    /*end LeonardM 15.08.2016*/
                    //    grDateDocJust.Columns["BugetLine"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    //    /*colDocumente*/
                    //    grDateDocJust.Columns[13].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    #endregion

                    //    #region documente plata banca
                    //    /*colDocumentePlataBanca*/
                    //    grDatePlataBanca.Columns[9].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    //    #endregion
                    //}

                    /*end LeonardM 12.08.2016*/
                    /*LeonardM 15.08.2016
                     * in momentul in care am selectat ceva ca avans, se afiseaza gridul aferent cheltuielilor estimate de 
                     * pe avans; caz contrar, nu se afiseaza*/
                    if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                        pnlEstChelt.Visible = true;
                    else
                        pnlEstChelt.Visible = false;

                    /*detaliu estimare cheltuieli*/
                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2002)
                    {
                        grDateEstChelt.Columns["BugetLine"].Visible = false;
                        grDateEstChelt.Columns["FreeTxt"].Visible = true;
                    }
                    else
                    {
                        grDateEstChelt.Columns["BugetLine"].Visible = false;
                        grDateEstChelt.Columns["FreeTxt"].Visible = true;
                    }

                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2002)
                    {
                        lblDiurna.Visible = false;
                        chkIsDiurna.ClientVisible = false;
                        pnlDateDepl.Visible = false;
                    }

                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2001 || Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2002)
                        grDateDocJust.Columns["ExpenseTypeAdmId"].Visible = false;
                    else /*Decont administrativ*/
                        grDateDocJust.Columns["ExpenseTypeId"].Visible = false;

                    //if (documentTypeId == 2003)/*decont administrativ*/
                    //{
                    //    lcDataDeplasare.Visibility = Visibility.Collapsed;
                    //    liModPlata.Visibility = Visibility.Collapsed;
                    //    liIsDiurna.Visibility = Visibility.Collapsed;
                    //}
                    /*11.08.2016 AdrianDumitrache a zis sa decomentez aceasta parte
                     * Groupama
                    lcGridCheltuieli.Visibility = System.Windows.Visibility.Collapsed;
                     * */
                    break;
            }  
        }

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) != 1)
                {
                    #region documente justificative 
                    /*colDocumente*/
                    //grDate.Columns[8].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                    if (e.Column.FieldName == "DocumentId" || e.Column.FieldName == "DocumentDetailId" || e.Column.FieldName == "DictionaryItemId" || e.Column.FieldName == "Amount" || e.Column.FieldName == "FreeTxt")
                        e.Editor.ReadOnly = true;
                    #endregion

                    Session["docPoateFiModificat"] = 0;
                }
                else
                    Session["docPoateFiModificat"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public DataTable GetAvsXDec_ConfigCurrencyXPay(int F10003, int documentTypeId)
        {
            DataTable q = null;
            try
            {
				string sql = "SELECT a.IdAuto, a.F10003, a.NumeComplet, COALESCE(a.CurrencyId, -99) AS CurrencyId, a.CurrencyCode, COALESCE(a.PaymentTypeId, -99) AS PaymentTypeId, "
							+ " a.PaymentTypeName, COALESCE(a.DocumentTypeId, -99) AS DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName "
							+ " FROM vwAvsXDec_ConfigCurrencyXPay a "
							+ " WHERE a.F10003 = " + F10003 + " AND a.DocumentTypeId = " + documentTypeId;
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }		
		
        public DataTable GetAvsXDec_DictionaryItemTipDeplasare()
        {
            DataTable q = null;
            try
            {
				string sql = "SELECT a.DictionaryItemId, a.DictionaryId, a.DictionaryItemName, COALESCE(b.Ordine, -99) AS Ordine "
							+ " FROM vwAvsXDec_Nomen_TipDeplasare a "
							+ " JOIN AvsXDec_DictionaryItem b ON a.DictionaryItemId = b.DictionaryItemId "
							+ " ORDER BY Ordine";
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetAvsXDec_DictionaryItemValute()
        {
            DataTable q = null;
            try
            {
				string sql = "SELECT a.DictionaryItemId, a.DictionaryId, a.DictionaryItemName, COALESCE(b.Ordine, -99) AS Ordine "
							+ " FROM vwAvsXDec_Nomen_TipMoneda a "
							+ " JOIN AvsXDec_DictionaryItem b ON a.DictionaryItemId = b.DictionaryItemId "
							+ " ORDER BY Ordine";
				
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetAvsXDec_DictionaryItemModalitatePlata()
        {
            DataTable q = null;
            try
            {
				string sql = "SELECT a.DictionaryItemId, a.DictionaryId, a.DictionaryItemName, COALESCE(b.Ordine, -99) AS Ordine "
							+ " FROM vwAvsXDec_Nomen_TipPlata a "
							+ " JOIN AvsXDec_DictionaryItem b ON a.DictionaryItemId = b.DictionaryItemId "
							+ " ORDER BY Ordine";
				
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }		

        protected void btnAproba_Click(object sender, EventArgs e)
        {
            MetodeCereri(1);
        }
        protected void btnRespins_Click(object sender, EventArgs e)
        {
            MetodeCereri(2);
        }

        private void MetodeCereri(int tipActiune, string motivRefuz = "")
        {
            try
            {
                DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
                int nrSel = 1;
                string ids = ent.Rows[0]["DocumentId"].ToString() + ",";


                #region completare motiv refuz
                if (tipActiune == 2)
                {
                    //switch (launchedPagedFrom)
                    //{
                    //case LansatDin.Formulare:
                    #region document editat din formulare


                            Document pagDoc = new Document();
                            string msg = pagDoc.AprobaDocumentAvansXDecont(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, tipActiune, Convert.ToInt32(Session["User_Marca"].ToString()), motivRefuz);

                            if (msg != "")
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                                System.Threading.Thread.Sleep(5000);
                                btnBack_Click(null, null);
                            }
                                    
                                
                          
                            #endregion
                            //break;
                        //case LansatDin.PlataFinanciar:
                            #region document editat din financiar
                            //CustomMessage confirmMessage = new CustomMessage(Dami.TraduCuvant("Sigur doriti continuarea procesului de respingere ?"), CustomMessage.MessageType.Confirm, null, Constante.IdLimba);
                            //confirmMessage.OKButton.Click += (obj, args) =>
                            //{
                            //    RejectDocument dlgFinanciar = new RejectDocument();
                            //    dlgFinanciar.Closed += (s, eargs) =>
                            //    {
                            //        if (dlgFinanciar.DialogResult == true)
                            //        {
                            //            if (string.IsNullOrEmpty(dlgFinanciar.txtRefuseReason.Text))
                            //            {
                            //                Message.InfoMessage("Nu ati completat motivul refuzului pentru respingere documente!");
                            //                wiIndicator.DeferedVisibility = false;
                            //                return;
                            //            }
                            //            wiIndicator.DeferedVisibility = true;

                            //            srvAvansXDecont ctxAvansXDecont = new srvAvansXDecont();
                            //            InvokeOperation opApr = ctxAvansXDecont.RefuzaDocument(Constante.UserId, (int)ent.DocumentId, Constante.User_AngajatId, dlgFinanciar.txtRefuseReason.Text);
                            //            opApr.Completed += (s3, args3) =>
                            //            {
                            //                if (!opApr.HasError)
                            //                {
                            //                    if (opApr.Value.ToString() != "" && opApr.Value.ToString() == "OK")
                            //                    {
                            //                        wiIndicator.DeferedVisibility = false;
                            //                        Message.InfoMessage(Dami.TraduCuvant("Proces finalizat cu succes."));
                            //                        apasat = true;
                            //                        wiIndicator.DeferedVisibility = false;
                            //                        btnInapoi_ItemClick(null, null);
                            //                    }
                            //                }
                            //            };
                            //        }
                            //    };
                            //    dlgFinanciar.Show();
                            //};
                            //confirmMessage.Show();
                            #endregion
                            //break;
                    //}
                }
                #endregion
                else
                #region aprobare document
                {
                    Document pagDoc = new Document();
                    string msg = pagDoc.AprobaDocumentAvansXDecont(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, tipActiune, Convert.ToInt32(Session["User_Marca"].ToString()), "");  
                 
                    if (msg != "")
                    {
                        MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError, "Atentie !");
                        System.Threading.Thread.Sleep(5000);
                        btnBack_Click(null, null);
                    }        
                   
                }
                #endregion
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                GolireVariabile();
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback("~/AvansXDecont/Document.aspx");
                else
                    Response.Redirect("~/AvansXDecont/Document", false);
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
                DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;

                if (txtLocatie.Text.Length > 0) ent.Rows[0]["ActionPlace"] = txtLocatie.Text;
                if (txtActionReason.Text.Length > 0) ent.Rows[0]["ActionReason"] = txtActionReason.Text;
                if (txtStartDate.Value != null) ent.Rows[0]["StartDate"] = Convert.ToDateTime(txtStartDate.Value);
                if (txtEndDate.Value != null) ent.Rows[0]["EndDate"] = Convert.ToDateTime(txtEndDate.Value);
                if (txtOraPlecare.Value != null) ent.Rows[0]["StartHour"] = General.ChangeToCurrentYear(txtOraPlecare.DateTime);
                if (txtOraSosire.Value != null) ent.Rows[0]["EndHour"] = General.ChangeToCurrentYear(txtOraSosire.DateTime);
                if (cmbModPlata.Value != null) ent.Rows[0]["PaymentTypeId"] = Convert.ToInt32(cmbModPlata.Value);
                if (cmbMonedaAvans.Value != null) ent.Rows[0]["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value);
                if (cmbActionType.Value != null) ent.Rows[0]["ActionTypeId"] = Convert.ToInt32(cmbActionType.Value);
                if (dtDueDate.Value != null) ent.Rows[0]["DueDate"] = Convert.ToDateTime(dtDueDate.Value);

                Session["AvsXDec_SursaDate"] = ent;

                string ras = ValidareDateObligatorii();
                if (ras != "")
                {
                    MessageBox.Show(Dami.TraduCuvant(ras), MessageBox.icoError, "Atentie !");
                    return;
                }
                ras = ValidareLimiteCheltuieli();
                if (ras != "")
                {
                    MessageBox.Show(Dami.TraduCuvant(ras), MessageBox.icoError, "Atentie !");
                    return;
                }

                ras = ValidareTipCheltuieli();
                if (ras != "")
                {
                    MessageBox.Show(Dami.TraduCuvant(ras), MessageBox.icoError, "Atentie !");
                    return;
                }

                /*LeonardM 10.08.2016
                 * ne focusam pe un alt control pentru a putea salva in regula*/
                grDate.CancelEdit();

                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 1001: /*Avans spre deplasare*/
                        #region salvare rezervari
                        string str = "";
                        List<object> lst = cmbTip.Value as List<object>;
                        if (lst != null) str = string.Join(";", lst);
                        #endregion

                        /*salvam rezervarile mai intai, dupa care cheltuielile si de abia la final
                         * detaliile pentru document, deoarece trebuie actualizat circuitul in functie de
                         * dimensiunile alese*/
                        string msg = SalvareRezervari(str, Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));                      
						if (msg != "Succes!")
						{
                            MessageBox.Show(Dami.TraduCuvant("Eroare la salvarea rezervarilor: " + System.Environment.NewLine + msg), MessageBox.icoError, "Atentie !");
						}
						else
						{
                            SalvareGrid();
						}
                        
                        break;
                    case 1002: /*Avans spre decontare*/
                        SalvareGrid();
                        break;
                }

                SalvareDate();


                int rez = SetCircuitSettingsDocument(Convert.ToInt32(ent.Rows[0]["DocumentId"].ToString()));
                switch (rez)
                {
                    case -123:
                        MessageBox.Show(Dami.TraduCuvant("Nu exista circuit definit pentru proprietatile alese!"), MessageBox.icoError, "Atentie !");
                        break;
                    case 0:
                        MessageBox.Show(Dami.TraduCuvant("Date salvate cu succes."), MessageBox.icoSuccess, "Succes!");
                        //General.NotificariDocumente(idDocument);                      
                        btnBack_Click(null, null);
                        break;
                }


                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void SalvareDate()
        {
            try
            {
                string sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString();
                DataTable entDocument = General.IncarcaDT(sql, null);

               
                DateTime dtSt = Convert.ToDateTime(txtStartDate.Value ?? "01/01/2100");
                string dataStart = dtSt.Day.ToString().PadLeft(2, '0') + "/" + dtSt.Month.ToString().PadLeft(2, '0') + "/" + dtSt.Year.ToString();
                DateTime dtEnd = Convert.ToDateTime(txtEndDate.Value ?? "01/01/2100");
                string dataEnd = dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString();
                string oraSt = "NULL";
                if (txtOraPlecare.Text.Length > 0)
                    oraSt = "CONVERT(DATETIME, '" + DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + txtOraPlecare.Text + ":00', 120)";

                string oraEnd = "NULL";
                if (txtOraSosire.Text.Length > 0)
                    oraEnd = "CONVERT(DATETIME, '" + DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0') + " " + txtOraSosire.Text + ":00', 120)";
                                
                DateTime dtD = Convert.ToDateTime(dtDueDate.Value ?? "01/01/2100");
                string dataDue = dtD.Day.ToString().PadLeft(2, '0') + "/" + dtD.Month.ToString().PadLeft(2, '0') + "/" + dtD.Year.ToString();

                //if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                {
                    sql = @"UPDATE AvsXDec_Avans SET CurrencyId = {0}, 
                    RefCurrencyId = null,
                    RefCurrencyValue = null,
                    TotalPayment = null,
                    RefTotalPayment = null,
                    StartDate = {1},
                    EndDate = {2},
                    UnconfRestAmount = null,
                    TotalAmount = {3},
                    USER_NO = {4},
                    TIME = GETDATE(),
                    StartHour = {5},
                    EndHour = {6},
                    EstimatedAmount = {7},
                    DueDate = {8},
                    PaymentTypeId = {9},
                    ActionTypeId = {10},
                    ActionPlace = '{11}',
                    ActionReason ='{12}',
                    chkDiurna = {13},
                    RefuseReason = '{14}',
                    AccPeriodId = null,
                    PeriodOfAccountId = null,
                    TransportTypeId = {15}
                    WHERE DocumentId =  {16}";
                   sql = string.Format(sql, Convert.ToInt32(cmbMonedaAvans.Value ?? 0), "CONVERT(DATETIME, '" + dataStart + "', 103)", "CONVERT(DATETIME, '" + dataEnd + "', 103)", (txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text.Replace(',', '.')), entDocument.Rows[0]["USER_NO"].ToString(), oraSt, oraEnd, (txtValEstimata.Text.Length <= 0 ? "0" : txtValEstimata.Text.Replace(',', '.')), "CONVERT(DATETIME, '" + dataDue + "', 103)",
                        Convert.ToInt32(cmbModPlata.Value ?? 0), Convert.ToInt32(cmbActionType.Value ?? 0), txtLocatie.Text, txtActionReason.Text, (chkIsDiurna.Checked ? "1" : "0"), "", Convert.ToInt32(cmbTransportType.Value ?? 0), Session["AvsXDec_IdDocument"].ToString());
                    General.ExecutaNonQuery(sql, null); 
                }
                //else
                //{
                //    sql = @"INSERT INTO AvsXDec_Avans (CurrencyId, RefCurrencyId, RefCurrencyValue, TotalPayment, RefTotalPayment, StartDate, EndDate, UnconfRestAmount, TotalAmount, USER_NO, TIME, StartHour,
                //    EndHour, EstimatedAmount, DueDate, PaymentTypeId, ActionTypeId, ActionPlace, ActionReason, chkDiurna, RefuseReason, AccPeriodId,PeriodOfAccountId, TransportTypeId)
                //    VALUES ({0}, null, null, null, null, {1}, {2}, null, {3}, {4}, GETDATE(), {5}, {6}, {7}, {8}, {9}, {10}, '{11}', '{12}', {13}, '{14}', null, null, {15}) ";
                //    sql = string.Format(sql, Convert.ToInt32(cmbMonedaAvans.Value ?? 0), "CONVERT(DATETIME, '" + dataStart + "', 103)", "CONVERT(DATETIME, '" + dataEnd + "', 103)", (txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text), entDocument.Rows[0]["USER_NO"].ToString(), oraSt, oraEnd, (txtValAvans.Text.Length <= 0 ? "0" : txtValEstimata.Text), "CONVERT(DATETIME, '" + dataDue + "', 103)",
                //         Convert.ToInt32(cmbModPlata.Value ?? 0), Convert.ToInt32(cmbActionType.Value ?? 0), txtLocatie.Text, txtActionReason.Text, (chkIsDiurna.Checked ? "1" : "0"), "", Convert.ToInt32(cmbTransportType.Value ?? 0), Session["AvsXDec_IdDocument"].ToString());
                //    General.ExecutaNonQuery(sql, null);
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void SalvareGrid()
		{
            DataTable ent = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;

            DataTable dt = new DataTable();

            dt.Columns.Add("DocumentId", typeof(int));
            dt.Columns.Add("DocumentDetailId", typeof(int));
            dt.Columns.Add("DictionaryItemId", typeof(int));
            dt.Columns.Add("Amount", typeof(decimal));
            dt.Columns.Add("F10003", typeof(int));
            dt.Columns.Add("CurrencyId", typeof(int));
            dt.Columns.Add("SoldAmount", typeof(decimal));
            dt.Columns.Add("SoldCurrencyId", typeof(int));
            dt.Columns.Add("FreeTxt", typeof(string));    
        
            for (int i = 0; i < ent.Rows.Count; i++)
            {
                DataRow row = dt.NewRow();
                foreach (DataColumn col in ent.Columns)
                {
                    switch (col.ColumnName)
                    {
                        case "DictionaryItemId":
                            row["DictionaryItemId"] = ent.Rows[i]["DictionaryItemId"];
                            break;
                        case "DocumentDetailId":
                            row["DocumentDetailId"] = ent.Rows[i]["DocumentDetailId"];
                            break;
                        case "DocumentId":
                            row["DocumentId"] = ent.Rows[i]["DocumentId"];
                            break;
                        case "Amount":
                            row["Amount"] = ent.Rows[i]["Amount"];
                            break;
                        case "FreeTxt":
                            row["FreeTxt"] = ent.Rows[i]["FreeTxt"];
                            break;
                    }                
                }
                row["F10003"] = Convert.ToInt32((Session["AvsXDec_Marca"] ?? -99).ToString());
                dt.Rows.Add(row);
            }

            General.SalveazaDate(dt, "AvsXDec_AvansDetail");
        }


        public string SalvareRezervari(string filtruRezervari, int documentId)
        {
            try
            {
                /*stergem toate rezervarile de la nivel de document*/
				string sql = "SELECT * FROM vwAvsXDec_AvDet_Rezervari WHERE DocumentId = " + documentId;
				DataTable entRezervare = General.IncarcaDT(sql, null);
                for (int i = 0; i < entRezervare.Rows.Count; i++)
                {
					sql = "DELETE FROM AvsXDec_AvansDetail WHERE DocumentId = " + entRezervare.Rows[i]["DocumentId"].ToString() + " AND DocumentDetailId =  " + entRezervare.Rows[i]["DocumentDetailId"].ToString();
					General.ExecutaNonQuery(sql, null);
                }

                /*adaugam rezervarile la nivel de document*/
                string[] arr = filtruRezervari.Split(Convert.ToChar(";"));
                List<int> lst = new List<int>();
                if (arr.Count() > 0)
                {
                    for (int i = 0; i <= arr.Length - 1; i++)
                    {
                        if (arr[i] != "") lst.Add(Convert.ToInt32(arr[i]));
                    }
                }
                foreach (int IdRezervare in lst)
                {
					sql = "INSERT INTO AvsXDec_AvansDetail (DocumentDetailId, DocumentId, DictionaryItemId) VALUES (" 
					+ Dami.NextId("AvsXDec_AvansDetail", 1) + ", " + documentId + ", " + IdRezervare + ")";
					General.ExecutaNonQuery(sql, null);				
          
                }
                return "Succes!";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return ex.Message;
            }
        }		

		
        public DataTable GetmetaAvsXDec_AvansDetailCheltuieli(int documentId)
        {
            DataTable q = null;

            try
            {
				string sql = "SELECT a.DictionaryItemId, a.DocumentDetailId, a.DocumentId, a.Amount, a.FreeTxt, (CASE WHEN c.DocumentDetailId = 0 THEN 0 ELSE 1 END) as areFisier "
							+ " FROM vwAvsXDec_AvDet_Cheltuieli a "
							+ " LEFT JOIN AvsXDec_relUploadDocumente c ON a.DocumentId = c.DocumentId AND a.DocumentDetailId = c.DocumentDetailId "
							+ " where a.DocumentId = " + documentId;
				
                q = General.IncarcaDT(sql, null);
                return q;		

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }		
		
        public DataTable GetAvsXDec_DictionaryItemCheltuiala(int documentTypeId)
        {
            DataTable q = null;
			string sql = "";
            try
            {
                switch (documentTypeId)
                {
                    case 1001: /* Avans spre deplasare*/
					case 2001: /*Decontare avans*/
						sql = "SELECT a.DictionaryItemId, a.DictionaryId, a.DictionaryItemName, b.Ordine "
									+ " FROM vwAvsXDec_Nomen_TipCheltuieli a "
									+ " JOIN AvsXDec_DictionaryItem b ON a.DictionaryItemId = b.DictionaryItemId "
									+ " ORDER BY Ordine";	
                        break;
                    case 1002: /*Avans spre decontare*/
					case 2002: /*Decont cheltuieli simplu*/
						sql = "SELECT a.DictionaryItemId, a.DictionaryId, a.DictionaryItemName, b.Ordine "
									+ " FROM vwAvsXDec_Nomen_ChAvDecontare a "
									+ " JOIN AvsXDec_DictionaryItem b ON a.DictionaryItemId = b.DictionaryItemId "
									+ " ORDER BY Ordine";	
                        break;
                    case 2003: /*Decont Administrativ*/
                        //q = from a in this.ObjectContext.vwAvsXDec_Nomen_TipChDecAdmin
                        //    join b in this.ObjectContext.AvsXDec_DictionaryItem on a.DictionaryItemId equals b.DictionaryItemId
                        //    select new metaAvsXDec_DictionaryItem
                        //    {
                        //        DictionaryItemId = a.DictionaryItemId,
                        //        DictionaryId = a.DictionaryId,
                        //        DictionaryItemName = a.DictionaryItemName,
                        //        Ordine = b.Ordine
                        //    };
                        break;
                }
                q = General.IncarcaDT(sql, null);
                return q;	
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }		
		
        private string ValidareLimiteCheltuieli()
        {    
            string ras = "";
            DataTable dtCheltuieli = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;// GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
			DataTable dtNomenCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));

            dtNomenCheltuieli.CaseSensitive = false;
            DataRow[] itmDiurna = dtNomenCheltuieli.Select("DictionaryItemName = 'diurna'");
            for (int i = 0; i < dtCheltuieli.Rows.Count; i++)
            {
                decimal minValue = 0, maxValue =0, valExpenseCorrected = 0;
                if (!SumIsValidForExpense(Convert.ToInt32(dtCheltuieli.Rows[i]["DictionaryItemId"].ToString()), Convert.ToDecimal((dtCheltuieli.Rows[i]["Amount"] == DBNull.Value ? 0 : dtCheltuieli.Rows[i]["Amount"]).ToString()), out valExpenseCorrected, out minValue, out maxValue))
                {
                    DataRow[] itmCheltuiala = dtNomenCheltuieli.Select("DictionaryItemId = " + dtCheltuieli.Rows[i]["DictionaryItemId"].ToString());
                    if (itmDiurna != null && itmDiurna.Length > 0)
                    {
                        if (itmCheltuiala != null && itmCheltuiala.Length > 0)
                        {
                            if(itmCheltuiala[0]["DictionaryItemId"].ToString() != itmDiurna[0]["DictionaryItemId"].ToString())
                                ras += ", pentru cheltuiala " + itmCheltuiala[0]["DictionaryItemName"].ToString() + " -> valorile trebuie sa fie intre " + minValue.ToString() + " si " + maxValue.ToString() + "\n";
                        }
                    }
                    else
                    {
                        if (itmCheltuiala != null && itmCheltuiala.Length > 0)
                            ras += ", pentru cheltuiala " + itmCheltuiala[0]["DictionaryItemName"].ToString() + " -> valorile trebuie sa fie intre " + minValue.ToString() + " si " + maxValue.ToString() + "\n";
                    }
                }
                #region verificare limite diurna
                /*LeonardM 11.09.2016
                             * se seteaza implicit diurna cu valoarea conform calculelor, dar poate fi editata
                             * cerinta Groupama: sa poata fi editata in minus */

                
                if (itmDiurna != null && itmDiurna.Length > 0)
                {
                    if (Convert.ToInt32(dtCheltuieli.Rows[i]["DictionaryItemId"].ToString()) == Convert.ToInt32(itmDiurna[0]["DictionaryItemId"].ToString()))
                    {
                        if (Convert.ToDecimal((dtCheltuieli.Rows[i]["Amount"] == DBNull.Value ? 0 : dtCheltuieli.Rows[i]["Amount"]).ToString()) > Convert.ToDecimal((Session["maxValueDiurna"] ?? 0).ToString()))
                        {
                            ras +=", diurna nu poate fi mai mare decat " + (Session["maxValueDiurna"] ?? 0).ToString() + " conform configurarilor stabilite! ";
                        }
                    }
                }
                #endregion

            }
            if (ras != "") ras = "Trebuie verificate sumele introduse " + ras.Substring(2) + "!";
            return ras;
        }

        private bool SumIsValidForExpense(int ExpenseId, decimal initialValExpense, out decimal valExpense, out decimal minValue, out decimal maxValue)
        {
            //DataTable ent = General.IncarcaDT("SELECT * FROM vwAvsXDec_Avans WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString(), null); 
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            int KeyField1 = -99, KeyField2 = -99, KeyField3 = -99;
            AvsXDec_Settings_SetExpenseDIMValue(1, ExpenseId, out KeyField1);
            AvsXDec_Settings_SetExpenseDIMValue(2, ExpenseId, out KeyField2);
            AvsXDec_Settings_SetExpenseDIMValue(3, ExpenseId, out KeyField3);
            DataRow[] entSettingsExpense;

            DataTable lstVwAvsXDec_Settings = General.IncarcaDT("SELECT * FROM vwAvsXDec_Settings", null);

            if (lstVwAvsXDec_Settings == null || lstVwAvsXDec_Settings.Rows.Count <= 0)
            {
                valExpense = initialValExpense;
                minValue = 0;
                maxValue = 0;
                return true;
            }

            DateTime dtStartHour;
            int noDays = 1;
            switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
            {
                case 1001: /* Avans spre deplasare
                              se calculeaza limitele de cheltuieli per zi*/
                    if (!DateTime.TryParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStartHour))
                    {
                        dtStartHour = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                    }
                    DateTime dtEndHour;
                    if (!DateTime.TryParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEndHour))
                    {
                        dtEndHour = Convert.ToDateTime(ent.Rows[0]["EndHour"].ToString()); 

                    }

                    DateTime dtStart = new DateTime(Convert.ToDateTime(txtStartDate.Value).Year, Convert.ToDateTime(txtStartDate.Value).Month, Convert.ToDateTime(txtStartDate.Value).Day, dtStartHour.Hour, dtStartHour.Minute, 0);
                    DateTime dtEnd = new DateTime(Convert.ToDateTime(txtEndDate.Value).Year, Convert.ToDateTime(txtEndDate.Value).Month, Convert.ToDateTime(txtEndDate.Value).Day, dtEndHour.Hour, dtEndHour.Minute, 0);
                    //DateTime dtEnd = new DateTime(ent.EndDate.Value.Year, ent.EndDate.Value.Month, ent.EndDate.Value.Day, ent.EndHour.Value.Hour, ent.EndHour.Value.Minute, 0);
                    double diffHours = (dtEnd - dtStart).TotalHours;
                    noDays = Convert.ToInt32(diffHours / 24);
                    noDays = noDays == 0 ? 1 : noDays;

                    break;
            }

            initialValExpense = initialValExpense / noDays;

            entSettingsExpense = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1  + " AND KeyField2 = " + KeyField2  + " AND KeyField3 = " + KeyField3 + " AND F71802 = " + General.Nz(Session["IdFunctieAngajat"], "-99").ToString() + " AND CurrencyId = " + ent.Rows[0]["CurrencyId"].ToString());
            /*nu am gasit o setare conform celor de mai sus si functiei, incercam
             * sa gasim o configurare conform celor de mai sus si idfunctie = -99*/
            if (entSettingsExpense == null)
            {
                entSettingsExpense = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1 + " AND KeyField2 = " + KeyField2 + " AND KeyField3 =  " + KeyField3 + " AND F71802 = -99 AND CurrencyId = " + ent.Rows[0]["CurrencyId"].ToString());
                /*daca tot nu gasim o configurare, atunci in mod default diurna = 0*/
                if (entSettingsExpense == null)
                {
                    valExpense = initialValExpense;
                    minValue = 0;
                    maxValue = 0;
                    return true;
                }
            }

            #region validare cheltuiala conform configurari
            if (Convert.ToDecimal((entSettingsExpense[0]["MinimumPay"] == DBNull.Value ? 0 : entSettingsExpense[0]["MinimumPay"]).ToString()) <= initialValExpense && Convert.ToDecimal((entSettingsExpense[0]["MaximumPay"] == DBNull.Value ? 0 : entSettingsExpense[0]["MaximumPay"]).ToString()) >= initialValExpense)
            {
                valExpense = initialValExpense;
                minValue = 0;
                maxValue = 0;
                return true;
            }
            else
            {
                valExpense = Convert.ToDecimal((entSettingsExpense[0]["MinimumPay"] == DBNull.Value ? 0 : entSettingsExpense[0]["MinimumPay"]).ToString());
                minValue = Convert.ToDecimal((entSettingsExpense[0]["MinimumPay"] == DBNull.Value ? 0 : entSettingsExpense[0]["MinimumPay"]).ToString());
                maxValue = Convert.ToDecimal((entSettingsExpense[0]["MaximumPay"] == DBNull.Value ? 0 : entSettingsExpense[0]["MaximumPay"]).ToString());
                return false;
            }
            #endregion

        }
        private void AvsXDec_Settings_SetExpenseDIMValue(int dim, int ExpenseId, out int value)
        {
            value = -99;
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;

            DataTable lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
            string DIMSetting = string.Empty;
            DataTable lstVwAvsXDec_Settings_Config = General.IncarcaDT(@"select * from ""vwAvsXDec_Settings_Config""", null);

            if (lstVwAvsXDec_Settings_Config != null && lstVwAvsXDec_Settings_Config.Rows.Count > 0 && lstCheltuieli != null && lstCheltuieli.Rows.Count > 0)
            {
                /*descoperim ce dimensiune verificam*/
                switch (dim)
                {
                    case 1:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField1"].ToString();
                        break;
                    case 2:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField2"].ToString();
                        break;
                    case 3:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField3"].ToString();
                        break;
                    default:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField1"].ToString();
                        break;
                }
                /*asignare valori conform setarilor pe dimensiune*/
                switch (DIMSetting)
                {
                    case "ActionTypeId":
                        value = Convert.ToInt32(ent.Rows[0]["ActionTypeId"].ToString());
                        break;
                    case "PaymentTypeId":
                        value = Convert.ToInt32(ent.Rows[0]["PaymentTypeId"].ToString());
                        break;
                    case "ExpenseId":
                        DataRow[] itmExpense = lstCheltuieli.Select("DictionaryItemId = " + ExpenseId);
                        if (itmExpense != null && itmExpense.Length > 0)
                            value = Convert.ToInt32(itmExpense[0]["DictionaryItemId"].ToString());
                        break;
                    default:
                        value = -99;
                        break;
                }
            }
        }

        private string ValidareDateObligatorii()
        {
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable; 
            try
            {
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 1001: /*Avans spre deplasare*/
                        if (ent.Rows[0]["ActionPlace"] == DBNull.Value) ras += ", locatie";
                        if (ent.Rows[0]["ActionReason"] == DBNull.Value) ras += ", motiv deplasare";
                        if (ent.Rows[0]["StartDate"] == DBNull.Value) ras += ", data inceput";
                        if (ent.Rows[0]["EndDate"] == DBNull.Value) ras += ", data sfarsit";
                        if (string.IsNullOrEmpty(txtOraPlecare.Text) && ent.Rows[0]["StartHour"] == DBNull.Value)
                            ras += ", ora inceput";
                        else
                        {
                            if (ent.Rows[0]["StartHour"] == DBNull.Value)
                                ras += ", ora inceput";
                        }
                        if (string.IsNullOrEmpty(txtOraSosire.Text) && ent.Rows[0]["EndHour"] == DBNull.Value)
                            ras += ", ora sfarsit";
                        else
                        {
                            if (ent.Rows[0]["EndHour"] == DBNull.Value)
                                ras += ", ora sfarsit";                            
                        }
                        if (ent.Rows[0]["PaymentTypeId"] == DBNull.Value) ras += ", modalitate plata";
                        if (ent.Rows[0]["CurrencyId"] == DBNull.Value) ras += ", moneda plata";
                        if (ent.Rows[0]["ActionTypeId"] == DBNull.Value) ras += ", tip deplasare";
                        if (txtValEstimata.Value == null || Convert.ToDecimal(txtValEstimata.Value) == 0) ras += ", cheltuieli pentru avans";
                        break;
                    case 1002: /*Avans spre decontare*/
                        if (ent.Rows[0]["CurrencyId"] == DBNull.Value) ras += ", moneda plata";
                        if (ent.Rows[0]["PaymentTypeId"] == DBNull.Value) ras += ", modalitate plata";
                        if (ent.Rows[0]["DueDate"] == DBNull.Value) ras += ", data scadenta";
                        if (txtValEstimata.Value == null || Convert.ToDecimal(txtValEstimata.Value) == 0) ras += ", cheltuieli pentru avans";
                        break;


                }
				DataTable dtCheltuieli = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;// GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieli != null && dtCheltuieli.Rows.Count != 0 && dtCheltuieli.Select("DictionaryItemId IS NULL").Count() != 0)
                    ras += ", cheltuiala completata pe unele randuri";
                if (ras != "") ras = "Date lipsa: " + ras.Substring(2) + " !";
            }
            catch (Exception)
            {
            }

            return ras;
        }
        private string ValidareTipCheltuieli()
        {
            /*LeonardM 10.08.2016
             * pentru avansul spre decontare trebuie specificat un singur tip de cheltuiala
             * in afara de diurna*/
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            try
            {
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 1001: /*Avans spre deplasare*/
                        break;
                    case 1002: /*Avans spre decontare*/
						DataTable dtCheltuieliInserate = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;
						DataTable dtCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                        int idTipCheltuiala = -99;
                        for (int i = 0; i < dtCheltuieliInserate.Rows.Count; i++)
                        {
                            if (idTipCheltuiala == -99)
                            {
                                idTipCheltuiala = Convert.ToInt32((dtCheltuieli.Rows[i]["DictionaryItemId"] == DBNull.Value ? -99 : dtCheltuieli.Rows[i]["DictionaryItemId"]).ToString());
                                continue;
                            }
                            if (Convert.ToInt32((dtCheltuieli.Rows[i]["DictionaryItemId"] == DBNull.Value ? -99 : dtCheltuieli.Rows[i]["DictionaryItemId"]).ToString()) != idTipCheltuiala)
                            {
                                ras = ", se completeaza doar un singur tip de cheltuiala pentru avansul spre decontare!";
                                break;
                            }
                        }
                        break;
                }
                if (ras != "") ras = "Atentie: " + ras.Substring(2) + " !";
            }
            catch (Exception)
            {
            }

            return ras;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //int idRap = Convert.ToInt32(Dami.ValoareParam("IdRaportFisaPost", "-99"));
                //if (idRap != -99 && Convert.ToInt32(General.Nz(Session["IdAuto"], "-99")) != -99)
                //{
                //    var reportParams = new
                //    {
                //        IdAutoPost = Convert.ToInt32(General.Nz(Session["IdAuto"], "-99"))
                //    };

                //    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
                //    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);

                //    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Org_FisaPost", "window.location.href = \"" + ResolveClientUrl(reportUrl) + "\"", true);
                //}
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
                //metaCereriDate itm = new metaCereriDate();
                //if (Session["Posturi_Upload"] != null) itm = Session["Posturi_Upload"] as metaCereriDate;

                //itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                //itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                //itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                //Session["Posturi_Upload"] = itm;

                //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
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
                DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
                switch (e.Parameter.Split(';')[0])
                {
                    case "cmbMonedaAvans":
                        ent.Rows[0]["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value ?? -99);
                        Session["AvsXDec_SursaDate"] = ent;
                        cmbMonedaAvans_SelectedIndexChanged();
                        break;
                    case "cmbActionType":
                        ent.Rows[0]["ActionTypeId"] = Convert.ToInt32(cmbActionType.Value ?? -99);
                        Session["AvsXDec_SursaDate"] = ent;
                        cmbActionType_EditValueChanged();
                        break;
                    case "btnRespinge":
                        MetodeCereri(2, e.Parameter.Split(';')[1]);
                        break;
                    case "txtStartDate":
                        ent.Rows[0]["StartDate"] = Convert.ToDateTime(txtStartDate.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDate"] = ent;
                        txtStartDate_EditValueChanged();
                        break;
                    case "txtEndDate":
                        ent.Rows[0]["EndDate"] = Convert.ToDateTime(txtEndDate.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDate"] = ent;
                        txtEndDate_EditValueChanged();
                        break;
                    case "txtOraPlecare":
                        ent.Rows[0]["StartHour"] = Convert.ToDateTime(txtOraPlecare.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDate"] = ent;
                        txtOraPlecare_EditValueChanged();
                        break;
                    case "txtOraSosire":
                        ent.Rows[0]["EndHour"] = Convert.ToDateTime(txtOraSosire.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDate"] = ent;
                        txtOraSosire_EditValueChanged();
                        break;
                    case "txtValEstimata":
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(txtValEstimata.Value ?? -99);
                        Session["AvsXDec_SursaDate"] = ent;
                        txtValEstimata_EditValueChanged();
                        break;
                    case "txtValAvans":
                        ent.Rows[0]["TotalAmount"] = Convert.ToInt32(txtValAvans.Value ?? -99);
                        Session["AvsXDec_SursaDate"] = ent;
                        txtValAvans_EditValueChanged();
                        break;
                }
   
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

		
		
        private void IncarcaDate()
        {
            try
            {
                DataTable ent = null;

                if (Session["AvsXDec_SursaDate"] == null)
                {
                    ent = General.IncarcaDT("SELECT * FROM vwAvsXDec_Avans WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString(), null);
                    Session["AvsXDec_SursaDate"] = ent;
                }
                else
                    ent = Session["AvsXDec_SursaDate"] as DataTable;


                string sql = "";
                if (ent != null && ent.Rows.Count > 0)
                {
                    txtNrOrdinDeplasare.Text = ent.Rows[0]["DocumentId"].ToString();
                    txtNumeComplet.Text = ent.Rows[0]["NumeComplet"].ToString();
                    txtDepartament.Text = ent.Rows[0]["Departament"].ToString();
                    txtLocMunca.Text = ent.Rows[0]["LocMunca"].ToString();
                    txtContIban.Text = ent.Rows[0]["ContBancar"].ToString();
                    Session["IdFunctieAngajat"] = Convert.ToInt32((ent.Rows[0]["IdFunctie"] == DBNull.Value ? -99 : ent.Rows[0]["IdFunctie"]).ToString());

                    if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1)
                    {
                        if (Convert.ToInt32(General.Nz(Session["IdDeplasareDefault"], -99)) != -99) 
                        {
                            ent.Rows[0]["ActionTypeId"] = Convert.ToInt32(Session["IdDeplasareDefault"].ToString());
                        }
                        if (Convert.ToInt32(General.Nz(Session["IdMonedaDefault"], -99)) != -99)
                        {
                            ent.Rows[0]["CurrencyId"] = Convert.ToInt32(Session["IdMonedaDefault"].ToString());
                        }
                        if (Convert.ToInt32(General.Nz(Session["IdModPlataDefault"], -99)) != -99)
                        {
                            ent.Rows[0]["PaymentTypeId"] = Convert.ToInt32(Session["IdModPlataDefault"].ToString());
                        }
                        /*conform specificatiilor, pentru tipul de document avans spre decontyare,
                        * data scadenta se popate edita, iar initial se completeaza cu data curenta*/
                        if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1 && Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 1002)
                        {
                            ent.Rows[0]["DueDate"] = DateTime.Now;
                            dtDueDate.Value = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (!IsPostBack)
                        {
                            txtLocatie.Text = ent.Rows[0]["ActionPlace"].ToString();
                            cmbActionType.Value = Convert.ToInt32(ent.Rows[0]["ActionTypeId"].ToString());
                            cmbTransportType.Value = Convert.ToInt32(ent.Rows[0]["TransportTypeId"].ToString());
                            txtActionReason.Text = ent.Rows[0]["ActionReason"].ToString();
                            txtStartDate.Value = Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString());
                            txtOraPlecare.Value = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                            txtEndDate.Value = Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString());
                            txtOraSosire.Value = Convert.ToDateTime(ent.Rows[0]["EndHour"].ToString());
                            cmbMonedaAvans.Value = Convert.ToInt32(ent.Rows[0]["CurrencyId"].ToString());
                            cmbModPlata.Value = Convert.ToInt32(ent.Rows[0]["PaymentTypeId"].ToString());
                            chkIsDiurna.Checked = Convert.ToInt32(General.Nz(ent.Rows[0]["chkDiurna"], 0).ToString()) == 1 ? true : false;
                            txtValEstimata.Text = ent.Rows[0]["EstimatedAmount"].ToString();
                            txtValAvans.Text = ent.Rows[0]["TotalAmount"].ToString();
                            dtDueDate.Value = Convert.ToDateTime(ent.Rows[0]["DueDate"].ToString());
                            //cmbTip.Value = Convert.ToInt32(ent.Rows[0]["???"].ToString());
                        }
                    }

                    #region configurari moneda x modalitate plata
                    DataTable lstConfigCurrencyXPay = Session["ConfigCurrencyXPay"] as DataTable;
                    DataTable lstConfigCurrencyXPay_Currency = Session["ConfigCurrencyXPay_Currency"] as DataTable;
                    DataTable lstConfigCurrencyXPay_Pay = Session["ConfigCurrencyXPay_Pay"] as DataTable;
                    DataTable lstConfigCurrencyXPay_PayCopy = Session["ConfigCurrencyXPay_PayCopy"] as DataTable;
                    //switch (dreptUserAccesFormular)
                    {
                        //case tipAccesPagina.formularNou:
                        //case tipAccesPagina.formularSalvatEditUser:
                            if (lstConfigCurrencyXPay != null && lstConfigCurrencyXPay.Rows.Count != 0)
                            {
                                for (int i = 0; i < lstConfigCurrencyXPay.Rows.Count;i ++)
                                {
                                    if (lstConfigCurrencyXPay_Currency != null)
                                    {
                                        if (lstConfigCurrencyXPay_Currency.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["CurrencyId"].ToString()).Count() == 0)
                                        {
                                            lstConfigCurrencyXPay_Currency.Rows.Add(lstConfigCurrencyXPay.Rows[i]["CurrencyId"].ToString(), lstConfigCurrencyXPay.Rows[i]["CurrencyCode"].ToString(), null, 1);
                                            Session["ConfigCurrencyXPay_Currency"] = lstConfigCurrencyXPay_Currency;
                                        }
                                    }
                                    if (lstConfigCurrencyXPay_Pay != null)
                                    {
                                        if (lstConfigCurrencyXPay_Pay.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString()).Count() == 0)
                                        {
                                            lstConfigCurrencyXPay_Pay.Rows.Add(lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString(), lstConfigCurrencyXPay.Rows[i]["PaymentTypeName"].ToString(), null, 2);
                                            Session["ConfigCurrencyXPay_Pay"] = lstConfigCurrencyXPay_Pay;
                                        }
                                    }
                                }
                                cmbModPlata.DataSource = lstConfigCurrencyXPay_Pay;
                                cmbModPlata.DataBind();
                                cmbMonedaAvans.DataSource = lstConfigCurrencyXPay_Currency;
                                cmbMonedaAvans.DataBind();

                                ent.Rows[0]["PaymentTypeId"] = null;
                                lstConfigCurrencyXPay_PayCopy.Clear();
								DataRow[] entConfig = lstConfigCurrencyXPay.Select("CurrencyId = " + Convert.ToInt32(cmbMonedaAvans.Value ?? -99));
                                for (int i = 0; i < entConfig.Length; i++)
                                {
                                    if (lstConfigCurrencyXPay_PayCopy != null)
                                    {
                                        if (lstConfigCurrencyXPay_PayCopy.Select("DictionaryItemId = " + entConfig[i]["PaymentTypeId"].ToString()).Count() == 0)
                                        {
                                            lstConfigCurrencyXPay_PayCopy.Rows.Add(lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString(), lstConfigCurrencyXPay.Rows[i]["PaymentTypeName"].ToString(), null, 2);
                                            Session["ConfigCurrencyXPay_PayCopy"] = lstConfigCurrencyXPay_PayCopy;
                                        }
                                    }
                                }
                                if (lstConfigCurrencyXPay_PayCopy.Rows.Count == 1)
                                {
                                    ent.Rows[0]["PaymentTypeId"] = lstConfigCurrencyXPay_PayCopy.Rows[0]["DictionaryItemId"];
                                    cmbModPlata.ClientEnabled = false;
                                    cmbModPlata.DataBind();
                                }
                                else
                                {
                                    ent.Rows[0]["PaymentTypeId"] = lstConfigCurrencyXPay_PayCopy.Rows[0]["DictionaryItemId"];
                                    cmbModPlata.ClientEnabled = true;
                                    cmbModPlata.DataBind();
                                }
                            }
                            //break;
                        //case tipAccesPagina.formularSalvat:
                            //break;
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaNomenclatorCheltuieli()
        {
			DataTable dtCheltuieli = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));	

            #region verificare limite diurna
            /*LeonardM 11.09.2016
             * se seteaza implicit diurna cu valoarea conform calculelor, dar poate fi editata
             * cerinta Groupama: sa poata fi editata in minus */
            if (dtCheltuieli.Rows.Count == 0)
                return;
            dtCheltuieli.CaseSensitive = false;
            DataRow[] itmDiurna = dtCheltuieli.Select("DictionaryItemName = 'diurna'");
            if (itmDiurna != null && itmDiurna.Length > 0)
            {
                DataTable dtCheltuieliAvans = Session["AvsXDec_SursaDateCheltuieli"] as DataTable; //GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieliAvans.Rows.Count != 0)
                {
                    DataRow[] entCheltuialaDiurna = dtCheltuieliAvans.Select("DictionaryItemId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                    if (entCheltuialaDiurna != null && entCheltuialaDiurna.Length > 0)
                        Session["maxValueDiurna"] = Convert.ToDecimal((entCheltuialaDiurna[0]["Amount"] == DBNull.Value ? 0 : entCheltuialaDiurna[0]["Amount"]).ToString());
                }
            }
            #endregion


            #region cheltuiala limita cazare
            DataRow[] itmCazare = dtCheltuieli.Select("DictionaryItemName = 'cazare'");
            if (itmCazare != null && itmCazare.Length > 0)
            {
                DataTable dtCheltuieliAvans = Session["AvsXDec_SursaDateCheltuieli"] as DataTable; //GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                DataRow[] entCheltuialaCazare = dtCheltuieliAvans.Select("DictionaryItemId = " + itmCazare[0]["DictionaryItemId"].ToString());
                if (entCheltuialaCazare != null && entCheltuialaCazare.Length > 0)
                    LoadAvansReservations(true);
            }
            #endregion
        }

        private void IncarcaCheltuieli()
        {

            #region verificare limite diurna
            /*LeonardM 11.09.2016
             * se seteaza implicit diurna cu valoarea conform calculelor, dar poate fi editata
             * cerinta Groupama: sa poata fi editata in minus */
            DataTable dtCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
            if (dtCheltuieli.Rows.Count == 0)
                return;
            dtCheltuieli.CaseSensitive = false;
            DataRow[] itmDiurna = dtCheltuieli.Select("DictionaryItemName = 'diurna'");
            if (itmDiurna != null && itmDiurna.Length > 0)
            {
                DataTable dtCheltuieliAvans = Session["AvsXDec_SursaDateCheltuieli"] as DataTable; //GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieliAvans.Rows.Count != 0)
                {
                    DataRow[] entCheltuialaDiurna = dtCheltuieliAvans.Select("DictionaryItemId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                    if (entCheltuialaDiurna != null && entCheltuialaDiurna.Length > 0)
                        Session["maxValueDiurna"] = Convert.ToDecimal((entCheltuialaDiurna[0]["Amount"] == DBNull.Value ? 0 : entCheltuialaDiurna[0]["Amount"]).ToString());
                }
            }
            #endregion

            #region cheltuiala cazare
            DataRow[] itmCazare = dtCheltuieli.Select("DictionaryItemName = 'cazare'");
            if (itmCazare != null && itmCazare.Length > 0)
            {
                DataTable dtCheltuieliAvans = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;  //GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                DataRow[] entCheltuialaCazare = dtCheltuieliAvans.Select("DictionaryItemId = " + itmCazare[0]["DictionaryItemId"].ToString());
                if (entCheltuialaCazare != null && entCheltuialaCazare.Length > 0)
                    LoadAvansReservations(true);
            }
            #endregion

        }		
		
		private void cmbMonedaAvans_SelectedIndexChanged()
        {
            try
            {
                #region incarcare moneda si tip de plata conform configurari din  baza de date
                DataTable lstConfigCurrencyXPay = Session["ConfigCurrencyXPay"] as DataTable;
                DataTable lstConfigCurrencyXPay_PayCopy = Session["ConfigCurrencyXPay_PayCopy"] as DataTable;
                //switch (dreptUserAccesFormular)
                {
                    //case tipAccesPagina.formularNou:
                    //case tipAccesPagina.formularSalvatEditUser:
                        if (lstConfigCurrencyXPay != null)
                        {
                            cmbModPlata.Value = null;
                            lstConfigCurrencyXPay_PayCopy.Clear();
							DataRow[] entConfig = lstConfigCurrencyXPay.Select("CurrencyId = " + Convert.ToInt32(cmbMonedaAvans.Value ?? -99));
                            for (int i = 0; i < entConfig.Length; i++)
                            {
                                if (lstConfigCurrencyXPay_PayCopy != null)
                                {
                                    if (lstConfigCurrencyXPay_PayCopy.Select("DictionaryItemId = " + entConfig[i]["PaymentTypeId"].ToString()).Count() == 0)
                                    {
                                        lstConfigCurrencyXPay_PayCopy.Rows.Add(entConfig[i]["PaymentTypeId"].ToString(), entConfig[i]["PaymentTypeName"].ToString(), null, 2);
                                        Session["ConfigCurrencyXPay_PayCopy"] = lstConfigCurrencyXPay_PayCopy;
                                    }
                                }
                            }
                            if (lstConfigCurrencyXPay_PayCopy.Rows.Count == 1)
                            {
                                cmbModPlata.Value = Convert.ToInt32(lstConfigCurrencyXPay_PayCopy.Rows[0]["DictionaryItemId"].ToString());
                                cmbModPlata.ClientEnabled = false;
                            }
                            else
                            {                                
                                cmbModPlata.Value = Convert.ToInt32(lstConfigCurrencyXPay_PayCopy.Rows[0]["DictionaryItemId"].ToString());
                                cmbModPlata.ClientEnabled = true;
                            }
                        }
                        //break;
                    //case tipAccesPagina.formularSalvat:
                        //break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
		
		
        private void LoadAvansReservations(bool excludeCheltuialaCazare)
        {
            /*LeonardM 15.08.2016
             * se doreste ca in cazul in care utilizatorul a selectat cheltuiala pentru cazare,
             * sa nu mai poata face rezervare pentru cazare*/
            try
            {
               DataTable lstRezervari =null;       
                //cmbTip.StyleSettings = new CheckedComboBoxStyleSettings();
				DataTable dtRez = General.IncarcaDT("SELECT * FROM vwAvsXDec_Nomen_TipRezervari WHERE LOWER(DictionaryItemName) = 'cazare'", null);
				
             
				if (excludeCheltuialaCazare && dtRez != null && dtRez.Rows.Count > 0)
					lstRezervari = dtRez.Select("DictionaryItemId != " + dtRez.Rows[0]["DictionaryItemId"].ToString()).CopyToDataTable();
				else
					lstRezervari = dtRez;

				DataTable loRez = GetmetaAvsXDec_AvansDetailRezervari(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));			
		
				cmbTip.DataSource = lstRezervari;
                cmbTip.DataBind();

                List<object> lst = new List<object>();
                if (loRez != null)
				    for (int i = 0; i < loRez.Rows.Count; i++)
				    {
					    lst.Add(Convert.ToInt32(loRez.Rows[i]["DictionaryItemId"].ToString()));
				    }
				//cmbTip.Value = lst;
			
               
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetmetaAvsXDec_AvansDetailRezervari(int documentId)
        {
            DataTable q = null;

            try
            {
                string sql = "SELECT a.DictionaryItemId, a.DictionaryItemName, a.DocumentDetailId, a.DocumentId, 0 As Amount,  a.FreeTxt FROM vwAvsXDec_AvDet_Rezervari a WHERE a.DocumentId = " + documentId;
                General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }
        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;

                DataRow row = dt.Rows.Find(keys);                

                #region actualizare valoare avans
                /*LeonardM 25.08.2016
                 * actualizarea sumelor o realizam mai jos cu diurna,
                 * deoarece daca dezactivam bifa de diurna se scade de doua ori valoarea diurnei
                 * si nu e ok*/
                DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
                #endregion

                #region stergere diurna
                /*verificam daca exista cheltuiala de diurna si o stergem*/
                DataTable dtNomenCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                dtNomenCheltuieli.CaseSensitive = false;
                DataRow[] itmDiurna = dtNomenCheltuieli.Select("DictionaryItemName= 'diurna'");
                if (itmDiurna != null)
                {
                    if (Convert.ToInt32(row["DictionaryItemId"].ToString()) == Convert.ToInt32(itmDiurna[0]["DictionaryItemId"]))
                    {
                        chkIsDiurna.Checked = false;
                        Session["maxValueDiurna"] = 0;
                    }
                    else
                    {
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToInt32(General.Nz(row["Amount"], 0));
                        row.Delete();
                    }
                }
                else
                {
                    ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToInt32(General.Nz(row["Amount"], 0));
                    row.Delete();
                }
                #endregion


                #region cheltuiala Cazare
                /*LeonardM 15.08.2016
                * solicitare ca in momentul in care utilizatorul selecteaza cheltuiala cu cazare, sa nu mai poata
                * face rezervare pentru cazare */
                DataRow[] itmCazare = dtNomenCheltuieli.Select("DictionaryItemName = 'cazare'");
                if (itmCazare != null)
                {
                    if (Convert.ToInt32(row["DictionaryItemId"].ToString()) == Convert.ToInt32(itmCazare[0]["DictionaryItemId"].ToString()))
                    {
                        LoadAvansReservations(false);
                    }
                }
                #endregion
                Session["AvsXDec_SursaDate"] = ent;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["AvsXDec_SursaDateCheltuieli"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	
      
		

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;
     
                /*comentam aceasta parte, deoarece in momentul in care adaugam diurna
                 * dorim valoarea sa fie cea asignata de mine prin calcul
                 * ent.Amount = 0;
                 * */

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {                    
                            case "IDAUTO":
                                 row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1; 
                                break;              
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DOCUMENTID":
                                row[x] = Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()); 
                                break;
                            case "DOCUMENTDETAILID":
                                row[x] = Dami.NextId("AvsXDec_AvansDetail", 1);
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.DataSource = dt;
                grDate.KeyFieldName = "IdAuto";
                Session["AvsXDec_SursaDateCheltuieli"] = dt;
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

                DataTable dt = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }
                e.Cancel = true;
                grDate.CancelEdit();
                Session["AvsXDec_SursaDateCheltuieli"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
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
		
		
        //private void tvDateCheltuieli_ValidateRow(object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row == null) return;

        //        e.ErrorContent += Dami.AreNume(ref grDateCheltuieli, e.RowHandle, "DocumentDetailId", "Lipseste detaliu document. ");
        //        e.ErrorContent += Dami.AreNume(ref grDateCheltuieli, e.RowHandle, "DocumentId", "Lipseste document. ");

        //        #region verificare completare diurna
        //        /*verificam daca exista acelasi tip de cheltuiala anteriori*/
        //        List<metaAvsXDec_AvansDetailCheltuieli> lstCheltuieliInserate = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().ToList();
        //        List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
        //        metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
        //        if ((e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId == itmDiurna.DictionaryItemId && chkIsDiurna.IsChecked == false)
        //        {
        //            (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId = null;
        //            e.ErrorContent += "Diurna se adauga automat prin bifarea optiunii de calcul diurna!";
        //        }
        //        if ((e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId != null)
        //        {
        //            if (lstCheltuieliInserate.Where(p => p.DictionaryItemId == (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId && p.DocumentDetailId != (e.Value as metaAvsXDec_AvansDetailCheltuieli).DocumentDetailId).Count() != 0)
        //            {
        //                (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId = null;
        //                e.ErrorContent += "Acest tip de cheltuiala a mai fost adaugat!";
        //            }
        //        }
        //        #endregion

        //        if ((string)(e.ErrorContent) != "")
        //            e.IsValid = false;
        //        //else
        //        //    dds.SubmitChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}	
        //private void tvDateCheltuieli_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        //{
        //    try
        //    {
        //        var ent = dds.DataView.Cast<metaVwAvsXDec_Avans>().FirstOrDefault();
        //        metaAvsXDec_AvansDetailCheltuieli tmpRowChanged = grDateCheltuieli.GetFocusedRow() as metaAvsXDec_AvansDetailCheltuieli;
        //        switch (e.Column.FieldName)
        //        {
        //            case "colSel":
        //                ((TableView)sender).PostEditor();
        //                break;
        //            case "Amount":
        //                decimal valoareCheltuieli = 0;
        //                valoareCheltuieli = Convert.ToDecimal(ent.EstimatedAmount ?? 0);
        //                if (e.Value != null)
        //                {
        //                    #region verificare limite diurna
        //                    /*LeonardM 11.09.2016
        //                     * se seteaza implicit diurna cu valoarea conform calculelor, dar poate fi editata
        //                     * cerinta Groupama: sa poata fi editata in minus */
        //                    List<metaAvsXDec_DictionaryItem> lstCheltuieli_1 = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
        //                    metaAvsXDec_DictionaryItem itmDiurna_1 = lstCheltuieli_1.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
        //                    if (itmDiurna_1 != null)
        //                    {
        //                        if (Convert.ToInt32(tmpRowChanged.DictionaryItemId) == itmDiurna_1.DictionaryItemId)
        //                        {
        //                            if (Convert.ToDecimal(e.Value) > maxValueDiurna)
        //                            {
        //                                tmpRowChanged.Amount = maxValueDiurna;
        //                                Message.InfoMessage("Diurna nu poate fi mai mare decat " + maxValueDiurna.ToString() + " conform configurarilor stabilite! ");
        //                            }
        //                        }
        //                    }
        //                    #endregion

        //                    decimal valueExpenseCorrected = 0, minValue = 0, maxValue = 0;
        //                    if (tmpRowChanged.DictionaryItemId != null)
        //                        if (!SumIsValidForExpense((int)tmpRowChanged.DictionaryItemId, Convert.ToDecimal(e.Value.ToString()), out valueExpenseCorrected, out minValue, out maxValue))
        //                        {
        //                            bool depasesteLimitaSetata = false;

        //                            if (itmDiurna_1 != null)
        //                            {
        //                                if (itmDiurna_1.DictionaryItemId != tmpRowChanged.DictionaryItemId)
        //                                    depasesteLimitaSetata = true;
        //                            }
        //                            else
        //                            {
        //                                #region verificare suma intre limite
        //                                depasesteLimitaSetata = true;
        //                                #endregion
        //                            }
                                 
        //                            if (depasesteLimitaSetata)
        //                            {
        //                                Message.InfoMessage("Suma introdusa nu se incadreaza intre limitele setate in aplicatie: " + minValue.ToString() + " - " + maxValue.ToString() + "!");
        //                            }
        //                        }                           

        //                    if (e.OldValue == null)
        //                    {
        //                        /*rand nou*/
        //                        valoareCheltuieli = valoareCheltuieli + Convert.ToDecimal(e.Value.ToString());
        //                    }
        //                    else
        //                    {
        //                        /*rand vechi pentru care se actualizeaza suma*/
        //                        valoareCheltuieli = valoareCheltuieli - Convert.ToDecimal(e.OldValue.ToString()) + Convert.ToDecimal(e.Value.ToString());
        //                    }
        //                }
        //                ent.EstimatedAmount = valoareCheltuieli;
        //                break;
        //            case "DictionaryItemId":
        //                /*verificam daca exista acelasi tip de cheltuiala anteriori*/
        //                List<metaAvsXDec_AvansDetailCheltuieli> lstCheltuieliInserate = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().ToList();
        //                List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
        //                metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
        //                if (itmDiurna != null)
        //                {
        //                    if (Convert.ToInt32(e.Value) == itmDiurna.DictionaryItemId && chkIsDiurna.IsChecked == false)
        //                    {
        //                        grDateCheltuieli.SetCellValue(tvDateCheltuieli.FocusedRowHandle, "DictionaryItemId", null);
        //                        Message.InfoMessage("Diurna se adauga automat prin bifarea optiunii de calcul diurna!");
        //                        return;
        //                    }
        //                }

        //                /*LeonardM 10.08.2016
        //                * cerinta de la Groupama ca pentru avansul spre decontare sa se poata adauga doar un tip de cheltuiala
        //                * in afara de diurna, deoarece pentru acest tip de document exista circuit definit pentru un singur tip de 
        //                * cheltuiala*/
        //                switch (documentTypeId)
        //                {
        //                    case 1001: /*Avans spre deplasare*/
        //                        if (lstCheltuieliInserate.Where(p => p.DictionaryItemId == Convert.ToInt32(e.Value) && p.DocumentDetailId != tmpRowChanged.DocumentDetailId).Count() != 0)
        //                        {

        //                            Message.InfoMessage("Acest tip de cheltuiala a mai fost adaugat!");

        //                            if (!grDateCheltuieli.View.AllowEditing)
        //                                return;

        //                            var rand = grDateCheltuieli.GetFocusedRow();// tvDateCheltuieli.Grid.GetRow(rowHandle);

        //                            #region actualizare valoare avans
        //                            /*LeonardM 25.08.2016
        //                                 * actualizarea sumelor o realizam mai jos cu diurna,
        //                                 * deoarece daca dezactivam bifa de diurna se scade de doua ori valoarea diurnei
        //                                 * si nu e ok*/
        //                            #endregion

        //                            #region stergere diurna
        //                            /*verificam daca exista cheltuiala de diurna si o stergem*/
        //                            if (itmDiurna != null)
        //                            {
        //                                if (tmpRowChanged.DictionaryItemId == itmDiurna.DictionaryItemId)
        //                                {
        //                                    chkIsDiurna.IsChecked = false;
        //                                }
        //                                else
        //                                {
        //                                    ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmpRowChanged.Amount ?? 0);
        //                                    tvDateCheltuieli.CommitEditing();
        //                                    ddsCheltuieli.DataView.Remove(tmpRowChanged);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmpRowChanged.Amount ?? 0);
        //                                tvDateCheltuieli.CommitEditing();
        //                                ddsCheltuieli.DataView.Remove(tmpRowChanged);
        //                            }
        //                            #endregion
        //                        }
        //                        else
        //                        {
        //                            #region cheltuiala Cazare
        //                            /*LeonardM 15.08.2016
        //                             * solicitare ca in momentul in care utilizatorul selecteaza cheltuiala cu cazare, sa nu mai poata
        //                             * face rezervare pentru cazare */
        //                            metaAvsXDec_DictionaryItem itmCazare = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "cazare").FirstOrDefault();
        //                            if (itmCazare != null)
        //                            {
        //                                if (Convert.ToInt32(e.Value) == itmCazare.DictionaryItemId)
        //                                {
        //                                    LoadAvansReservations(true);
        //                                }
        //                            }
        //                            #endregion
        //                        }
        //                        break;
        //                    case 1002:/*Avans spre decontare*/
        //                        if (lstCheltuieliInserate.Where(p => p.DictionaryItemId != Convert.ToInt32(e.Value) && p.DocumentDetailId != tmpRowChanged.DocumentDetailId).Count() != 0)
        //                        {
        //                            Message.InfoMessage("Trebuie selectat doar un tip de cheltuiala pentru avansul spre decontare!");
        //                            tmpRowChanged.DictionaryItemId = null;
        //                            return;
        //                        }
        //                        break;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        public int SetCircuitSettingsDocument(int IdDocument)
        {
            try
            {
                string sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + IdDocument;
                DataTable entDocument = General.IncarcaDT(sql, null);
                List<int> DIM1Value = new List<int>(), DIM2Value = new List<int>();
                DataTable q = General.IncarcaDT("SELECT * FROM vwAvsXDec_CircuitSettings WHERE DocumentTypeId = " + entDocument.Rows[0]["DocumentTypeId"].ToString(), null); 
                SetCircuitSettingsDocumentXDim(1, out DIM1Value, q, entDocument);
                SetCircuitSettingsDocumentXDim(2, out DIM2Value, q, entDocument);
                DataTable entCircuit;
				
                string lstDIM1 = "";
                for (int j = 0; j < DIM1Value.Count; j++)
                {
                    lstDIM1 += DIM1Value[j].ToString();
                    if (j < DIM1Value.Count - 1)
                        lstDIM1 += ",";
                }
                if (lstDIM1.Length <= 0)
                    lstDIM1 = "-98";	
                string lstDIM2 = "";
                for (int j = 0; j < DIM2Value.Count; j++)
                {
                    lstDIM2 += DIM2Value[j].ToString();
                    if (j < DIM2Value.Count - 1)
                        lstDIM2 += ",";
                }
                if (lstDIM2.Length <= 0)
                    lstDIM2 = "-98";				
				
                DataTable lstCircuitXDocumentType = General.IncarcaDT("SELECT * FROM AvsXDec_Circuit Where DocumentTypeId = " + entDocument.Rows[0]["DocumentTypeId"].ToString());
                if (lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")") == null
                    || lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")").Count() <= 0)
                {
                    /*nu am valori care sa coincida DIM1 si DIM2, incerc sa vad daca am ceva pe DIM1 care sa se potriveasca*/
                    if (lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) = -99") != null &&
                        lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) = -99").Count() > 0)
                        entCircuit = lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) = -99").CopyToDataTable();
                    else /*nu am valori asociate pe DIM1, incerc sa gasesc valori asociate pentru DIM2*/
                        if (lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) = -99 AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")") != null &&
                        lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) = -99 AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")").Count() > 0)
                            entCircuit = lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) = -99 AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")").CopyToDataTable();
                        else /* nu am valori asociate pe DIM1 si DIM2, incerc sa gasesc valori asociate pentru DIM1 = -99, DIM2 = -99 */
                            entCircuit = lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) = -99 AND ISNULL(DIM2, -99) = -99").CopyToDataTable();
                }
                else
                    entCircuit = lstCircuitXDocumentType.Select("ISNULL(DIM1, -99) IN (" + lstDIM1 + ") AND ISNULL(DIM2, -99) IN (" + lstDIM2 + ")").CopyToDataTable();

                if (entCircuit == null || entCircuit.Rows.Count <= 0 || entCircuit.Rows[0]["CircuitId"] == null) return -123;
                int idCircuit = Convert.ToInt32(entCircuit.Rows[0]["CircuitId"].ToString());

				string culoare = "";
				sql = "SELECT * FROM \"AvsXDec_DictionaryItem\" WHERE \"DictionaryItemId\" = 1";
				DataTable dtCul = General.IncarcaDT(sql, null);
				if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
					culoare = dtCul.Rows[0]["Culoare"].ToString();
				else
					culoare = "#FFFFFFFF";

                //adaugam in tabela AvsXDec_DocumentStateHistory
                #region salvare istoric

                int total = 0;
                int idStare = 1;
                int pozUser = 1;
                int idSpr = -99;

                //aflam totalul de users din circuit
                for (int i = 1; i <= 20; i++)
                {
                    if (entCircuit.Rows[0]["Super" + i] != DBNull.Value)
                    {
                        int idSuper = Convert.ToInt32(entCircuit.Rows[0]["Super" + i].ToString());
                        if (Convert.ToInt32(idSuper) != -99)
                        {
                            //ne asiguram ca exista user pentru supervizorul din circuit						
                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                idSpr = Convert.ToInt32(idSuper);
								sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + entDocument.Rows[0]["F10003"].ToString();
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == DBNull.Value)
                                {
                                    continue;
                                }						
                            }
                            total++;
                        }
                    }
                }
						

                #region stergem istoric

                string sqlQuery = @"delete from ""AvsXDec_DocumentStateHistory"" where ""DocumentId"" = {0}";

                sqlQuery = string.Format(sqlQuery, entDocument.Rows[0]["DocumentId"].ToString());
                General.ExecutaNonQuery(sqlQuery, null);             
                #endregion

                int poz = 0;
                int idUserPrece = -99;
                int idUserCalc = -99;                
                for (int i = 1; i <= 20; i++)
                {
                    string aprobat = "NULL", dataAprobare = "NULL", inloc ="NULL", idUserInloc = "NULL";
                    int idSuper = -99;
                    idStare = -99;										
                    if (entCircuit.Rows[0]["Super" + i] != DBNull.Value)
                    {
                        idSuper = Convert.ToInt32(entCircuit.Rows[0]["Super" + i].ToString());
                        if (Convert.ToInt32(idSuper) != -99)
                        {
                            //IdUser
                            if (Convert.ToInt32(idSuper) == 0)
                            {							
                                //idUserCalc = idUser;
                                sql = "SELECT * FROM USERS WHERE F10003 = " + entDocument.Rows[0]["F10003"].ToString();
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser != null && dtUser.Rows.Count > 0 && dtUser.Rows[0]["F70102"] != DBNull.Value)
                                {
                                    idUserCalc = Convert.ToInt32(dtUser.Rows[0]["F70102"].ToString());
                                }
                            }					
							if (Convert.ToInt32(idSuper) > 0) idUserCalc = Convert.ToInt32(idSuper);
                            if (Convert.ToInt32(idSuper) < 0)
                            { 
								//ne asiguram ca exista user pentru supervizorul din circuit
								sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + entDocument.Rows[0]["F10003"].ToString();
								DataTable dtUser = General.IncarcaDT(sql, null);
								if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == DBNull.Value)
								{
									continue;
								}
								else
								{
									idUserCalc = Convert.ToInt32(dtUser.Rows[0]["IdUser"].ToString());
								}                                
                            }
							
		                    if (Convert.ToInt32(idSuper) < 0)
                            {
                                idSpr = Convert.ToInt32(idSuper);

                                sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + entDocument.Rows[0]["F10003"].ToString();
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == DBNull.Value)
                                {
                                    continue;
                                }
                            }	

                            //daca urmatorul in circuit este acelasi user, se salveaza doar o singura data
                            int idUrmIstoric;
                            if (idUserCalc != idUserPrece)
                            {
                                poz += 1;

                                /*LeonardM 19.01.2017
                                 * adaugare inlocuitor*/
								sql = "SELECT cer.Inlocuitor FROM USERS a "
									+ " JOIN Ptj_Cereri cer on a.F10003 = cer.F10003 "
									+ " WHERE a.F70102 = " + idUserCalc 
									+ " AND CONVERT(DATE, cer.DataInceput) <= CONVERT(DATE, GETDATE()) "
									+ " CONVERT(DATE, GETDATE()) <= CONVERT(DATE, cer.DataSfarsit) and cer.Inlocuitor IS NOT NULL " ;
								DataTable dtInloc = General.IncarcaDT(sql, null);      

								if (dtInloc != null && dtInloc.Rows.Count != 0)
								{
									inloc = dtInloc.Rows[0][0].ToString();
									idUserInloc = dtInloc.Rows[0][0].ToString();
								}		

								string culoareIst = "#FFFFFFFF";	
                                if (idUserCalc == Convert.ToInt32(entDocument.Rows[0]["USER_NO"].ToString()))
                                {
                                    pozUser = poz;
                                    if (poz == 1) idStare = 1;
                                    if (poz == total) idStare = 3;

                                    aprobat = "1";
                                    dataAprobare = "GETDATE()";

									sql = "SELECT * FROM \"AvsXDec_DictionaryItem\" WHERE DictionaryId = 1 AND \"DictionaryItemId\" = " + idStare;
									dtCul = General.IncarcaDT(sql, null);
									if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
										culoareIst = dtCul.Rows[0]["Culoare"].ToString();								
                                }								

                                idUrmIstoric = Dami.NextId("AvsXDec_DocumentStateHistory", 1);
								sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, IdSuper, Pozitie, USER_NO, TIME, Inlocuitor, IdUserInlocuitor, " 
								+ "Aprobat, DataAprobare, DocumentStateId, Culoare) VALUES (" + idUrmIstoric + ", " + entDocument.Rows[0]["DocumentId"].ToString() 
								+ ", " + idCircuit + ", " + idSuper + ", " + poz + ", " + idUserCalc + ", GETDATE(), " + inloc + ", " + idUserInloc 
								+ ", " + aprobat + ", " + dataAprobare + ", " + idStare + ", '" + culoareIst + "')";
								General.ExecutaNonQuery(sql, null);
								
                                idUserPrece = idUserCalc;
                            }
                            else
                            {
                                total--;
                            }
                        }
                    }
                }
                #endregion

                //adaugam headerul
                #region salvare header
				sql = "UPDATE AvsXDec_Document SET CircuitId = " + idCircuit + ", TotalCircuit = " + total + ", DocumentStateId = 1, Culoare = '" + culoare + "', Pozitie = 1 WHERE  DocumentId = " + IdDocument;
				General.ExecutaNonQuery(sql, null);             
                #endregion

                string corpMesaj = "";
                bool stop = false;

                try
                {
                    //srvNotif ctxNtf = new srvNotif();
                    //ctxNtf.TrimiteNotificare("AvansXDecont.Document", "grDate", entDocument, (int)entDocument.USER_NO, entDocument.F10003);
                }
                catch (Exception) { }

                #region actualizare data scadenta

                {
                    /*actualizare data scadenta*/

                    //Radu 09.08.2017
					sqlQuery = "\n" + @" exec procAvsXDec_dtPlata1001 @ndocumentId={0}";  
                    sqlQuery = string.Format(sqlQuery, IdDocument);
                    General.ExecutaNonQuery(sqlQuery, null);
                }
                #endregion
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return 0;
        }
		
        private void SetCircuitSettingsDocumentXDim(int dim, out List<int> lstValue, DataTable q, DataTable entDocument)
        {
            lstValue = new List<int>();
            string dimensiuneString = string.Empty;
			string sql = "";
            DataTable dtDocDec = null;
            if (q.Rows.Count != 0)
            {
                /*determinare dimensiune*/
                switch (dim)
                {
                    case 1:
                        dimensiuneString = q.Rows[0]["DIM1"].ToString();
                        break;
                    case 2:
                        dimensiuneString = q.Rows[0]["DIM2"].ToString();
                        break;
                }
            }
            /*asignare valori lista pentru dimensiune*/
            switch (dimensiuneString)
            {
                case "ActionTypeId":
                    if (Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1001 
					|| Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1002 
					|| Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1003)
					{
						sql = "SELECT * FROM AvsXDec_Avans WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
						DataTable dtDoc = General.IncarcaDT(sql, null);						
                        lstValue.Add(Convert.ToInt32(dtDoc.Rows[0]["ActionTypeId"].ToString()));
					}
                    else
					{
						sql = "SELECT * FROM AvsXDec_Decont WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
						DataTable dtDoc = General.IncarcaDT(sql, null);						
                        lstValue.Add(Convert.ToInt32(dtDoc.Rows[0]["ActionTypeId"].ToString()));						
					}
                    break;
                case "ReservationId":
					sql = "SELECT * FROM vwAvsXDec_AvDet_Rezervari WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
					DataTable dtRez = General.IncarcaDT(sql, null);					
                    for (int i = 0; i < dtRez.Rows.Count; i++)
                    {
                        lstValue.Add(Convert.ToInt32(dtRez.Rows[i]["DictionaryItemId"].ToString()));
                    }
                    break;
                case "ExpenseAvDecontareId":
                    if (Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1001
                        || Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1002 
						|| Convert.ToInt32(entDocument.Rows[0]["DocumentTypeId"].ToString()) == 1003)
						{
							sql = "SELECT * FROM vwAvsXDec_AvDet_Cheltuieli WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
							DataTable dtExp = General.IncarcaDT(sql, null);								
							for (int i = 0; i < dtExp.Rows.Count; i++)
							{
								lstValue.Add(Convert.ToInt32(dtExp.Rows[i]["DictionaryItemId"].ToString()));
							}
						}
                    else
					{
						sql = "SELECT * FROM vwAvsXDec_DecDet_DocDecontare WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
						dtDocDec = General.IncarcaDT(sql, null);								
						for (int i = 0; i < dtDocDec.Rows.Count; i++)						
						{
                            lstValue.Add(Convert.ToInt32(dtDocDec.Rows[i]["ExpenseTypeId"].ToString()));
                        }
					}
                    break;
                case "DocumentTypeDecontId":
					sql = "SELECT * FROM vwAvsXDec_DecDet_DocDecontare WHERE DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString();
					dtDocDec = General.IncarcaDT(sql, null);								
					for (int i = 0; i < dtDocDec.Rows.Count; i++)				
                    {
                        lstValue.Add(Convert.ToInt32(dtDocDec.Rows[i]["IdTipDocument"].ToString()));
                    }
                    break;
                case "-99":
                    /*nu se completeaza cu nimic lista*/
                    break;
            }
        }

        private void cmbActionType_EditValueChanged()
        {
            /*LeonardM 13.06.2016
             * in cazul in care se selecteaza id deplasare interna, default sa vina completat
             * moneda RON*/
            if (Convert.ToInt32(General.Nz(Session["IdDeplasareInterna"], -99)) != -99 && Convert.ToInt32(General.Nz(Session["IdMonedaRON"], -99)) != -99)
            {
                if (Convert.ToInt32(cmbActionType.Value ?? -99) == Convert.ToInt32(General.Nz(Session["IdDeplasareInterna"], -99)))
                    cmbMonedaAvans.Value = Convert.ToInt32(General.Nz(Session["IdMonedaRON"], -99));
            }
            /*end LeonardM 13.06.2016*/
            SetDiurnaProperty();
            UpdateCheltuialaDiurna();
        }

        public void SetDiurnaProperty()
        {
            /*daca sunt mai putin de 12 ore pentru deplasare, butonul de diurna nu trebuie sa fie vizibil*/
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            if (ent != null)
            {
                if (txtStartDate.Value != null && txtEndDate.Value != null && txtOraPlecare.Value != null && txtOraSosire.Value != null)
                {
                    DateTime dtStartDay = string.IsNullOrEmpty(txtStartDate.Value.ToString()) ? Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString()) : Convert.ToDateTime(txtStartDate.Value);
                    DateTime dtEndDay = string.IsNullOrEmpty(txtEndDate.Value.ToString()) ? Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString()) : Convert.ToDateTime(txtEndDate.Value);
                    DateTime dtStartHour;
                    if (!DateTime.TryParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStartHour))
                    {
                        dtStartHour = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                    }
                    DateTime dtEndHour;
                    if (!DateTime.TryParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEndHour))
                    {
                        dtEndHour = Convert.ToDateTime(ent.Rows[0]["EndHour"].ToString());
                    }
                    DateTime dtStart = new DateTime(dtStartDay.Year, dtStartDay.Month, dtStartDay.Day, dtStartHour.Hour, dtStartHour.Minute, 0);
                    DateTime dtEnd = new DateTime(dtEndDay.Year, dtEndDay.Month, dtEndDay.Day, dtEndHour.Hour, dtEndHour.Minute, 0);

                    double diffHours = (dtEnd - dtStart).TotalHours;
                    if (diffHours < 12)
                    {
                        chkIsDiurna.ClientEnabled = false;
                        chkIsDiurna.Checked = false;
                    }
                    else
                    {
                        if (!(Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0 && (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 || (Convert.ToInt32(Session["AvsXDec_IdStare"].ToString()) != 1))))
                            chkIsDiurna.ClientEnabled = true;
                    }
                }
                else
                {
                    chkIsDiurna.ClientEnabled = false;
                }
            }
            else
            {
                chkIsDiurna.ClientEnabled = false;
            }
        }

        private void UpdateCheltuialaDiurna()
        {
            if (chkIsDiurna.Checked)
            {
                /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;
                DataTable lstCheltuieli = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;// GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                lstCheltuieli.CaseSensitive = false;
                DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                DataRow[] chDiurna = lstCheltuieliInserate.Select("DictionaryItemId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                if (chDiurna != null)
                {
                    decimal valDiurna = 0;
                    CalculDiurna(out valDiurna);
                    DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
                    /*LeonardM 31.10.2016
                           * in mommentul in care diurna este modificat in jos,
                           * se recalculeaza la incarcare si atunci se modifica valoare*/
                    if (Convert.ToDecimal(General.Nz(chDiurna[0]["Amount"], 0)) > valDiurna)
                    {
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToDecimal(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToDecimal(General.Nz(chDiurna[0]["Amount"], 0)) + valDiurna;
                        Session["maxValueDiurna"] = valDiurna;

                        chDiurna[0]["Amount"] = valDiurna;
                        Session["AvsXDec_SursaDateCheltuieli"] = lstCheltuieliInserate;
                    }
                    else
                        Session["maxValueDiurna"] = valDiurna;
                    /*end LeonardM 31.10.2016*/
                }
            }
        }

        private void CalculDiurna(out decimal valDiurna)
        {
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            int KeyField1 = -99, KeyField2 = -99, KeyField3 = -99;
            AvsXDec_Settings_SetDIMValue(1, out KeyField1);
            AvsXDec_Settings_SetDIMValue(2, out KeyField2);
            AvsXDec_Settings_SetDIMValue(3, out KeyField3);
            DataRow[] entSettingsDiurna;
            int IdTipDeplasareAles = Convert.ToInt32(General.Nz(ent.Rows[0]["ActionTypeId"], -99));

            DataTable lstVwAvsXDec_Settings = General.IncarcaDT("SELECT * FROM vwAvsXDec_Settings", null);
            if (lstVwAvsXDec_Settings == null && lstVwAvsXDec_Settings.Rows.Count <= 0)
            {
                valDiurna = 0;
                return;
            }

            entSettingsDiurna = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1 + " AND KeyField2 = " + KeyField2 + " AND KeyField3 = " + KeyField3 + " AND F71802 = " + General.Nz(Session["IdFunctieAngajat"], "-99").ToString());
            /*nu am gasit o setare conform celor de mai sus si functiei, incercam
             * sa gasim o configurare conform celor de mai sus si idfunctie = -99*/
            if (entSettingsDiurna == null)
                //entSettingsDiurna = lstVwAvsXDec_Settings.Where(p => p.KeyField1 == KeyField1 && p.KeyField2 == KeyField2 && p.KeyField3 == KeyField3 && (p.F71802 ?? -99) == -99).FirstOrDefault();
                entSettingsDiurna = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1 + " AND KeyField2 = " + KeyField2 + " AND KeyField3 = " + KeyField3 + " AND F71802 = 0");
            /*daca tot nu gasim o configurare, atunci in mod default diurna = 0*/
            if (entSettingsDiurna == null)
                valDiurna = 0;
            else
            {
                #region calcul diurna conform configurarilor
                DateTime dtStartHour;
                if (!DateTime.TryParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStartHour))
                {
                    dtStartHour = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                }
                DateTime dtEndHour;
                if (!DateTime.TryParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtEndHour))
                {
                    dtEndHour = Convert.ToDateTime(ent.Rows[0]["EndHour"].ToString()); 
                }

                DateTime dtStart = new DateTime(Convert.ToDateTime(txtStartDate.Value).Year, Convert.ToDateTime(txtStartDate.Value).Month, Convert.ToDateTime(txtStartDate.Value).Day, dtStartHour.Hour, dtStartHour.Minute, 0);
                DateTime dtEnd = new DateTime(Convert.ToDateTime(txtEndDate.Value).Year, Convert.ToDateTime(txtEndDate.Value).Month, Convert.ToDateTime(txtEndDate.Value).Day, dtEndHour.Hour, dtEndHour.Minute, 0);
                //DateTime dtEnd = new DateTime(ent.EndDate.Value.Year, ent.EndDate.Value.Month, ent.EndDate.Value.Day, ent.EndHour.Value.Hour, ent.EndHour.Value.Minute, 0);
                double diffHours = (dtEnd - dtStart).TotalHours;

                if (IdTipDeplasareAles == Convert.ToInt32(General.Nz(Session["IdDeplasareExterna"], "-99").ToString()))
                {
                    /*la deplasarea externa, trebuie sa vedem cat din numarul total de ore se imparte la 6
                     * astfel incat daca am 11 ore => primesc jumatate de diurna, 
                     * daca am 12 ore primesc diurna intreaga
                     * daca am 19 ore primesc diurna + 1/2*/
                    decimal noHoursPer24 = Convert.ToDecimal(diffHours / 24);
                    decimal decimalNo = noHoursPer24 - Decimal.Truncate(noHoursPer24);
                    if (decimalNo > 0 && decimalNo < Convert.ToDecimal(0.5))
                        valDiurna = Decimal.Truncate(noHoursPer24) * Convert.ToDecimal(General.Nz(entSettingsDiurna[0]["MinimumPay"], 0).ToString()) + Convert.ToDecimal(General.Nz(entSettingsDiurna[0]["MinimumPay"], 0).ToString()) / 2;
                    else
                        valDiurna = (Decimal.Truncate(noHoursPer24) + Decimal.Ceiling(decimalNo)) * Convert.ToDecimal(General.Nz(entSettingsDiurna[0]["MinimumPay"], 0).ToString());
                }
                else
                {
                    /*la deplasarea interna, trebuie sa vedem cat din numarul total de ore se imparte la 12
                     * astfel incat daca am 12 ore => primesc 0
                     * daca 12 ore primesc diurna pe o zi
                     * daca am 19 ore primesc diurna pe o zi*/
                    decimal noHoursPer24 = Convert.ToDecimal(diffHours / 24);
                    decimal decimalNo = noHoursPer24 - Decimal.Truncate(noHoursPer24);
                    if (decimalNo >= 0 && decimalNo < Convert.ToDecimal(0.5))
                        valDiurna = Decimal.Truncate(noHoursPer24) * Convert.ToDecimal(General.Nz(entSettingsDiurna[0]["MinimumPay"], 0).ToString());
                    else
                        valDiurna = (Decimal.Truncate(noHoursPer24) + 1) * Convert.ToDecimal(General.Nz(entSettingsDiurna[0]["MinimumPay"], 0).ToString());
                }
                #endregion
            }

        }

        private void AvsXDec_Settings_SetDIMValue(int dim, out int value)
        {
            value = -99;
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            DataTable lstCheltuieli = Session["AvsXDec_SursaDateCheltuieli"] as DataTable; //GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
            string DIMSetting = string.Empty;
            DataTable lstVwAvsXDec_Settings_Config = General.IncarcaDT(@"select * from ""vwAvsXDec_Settings_Config""", null);
            if (lstVwAvsXDec_Settings_Config != null && lstVwAvsXDec_Settings_Config.Rows.Count > 0 && lstCheltuieli != null && lstCheltuieli.Rows.Count > 0)
            {
                /*descoperim ce dimensiune verificam*/
                switch (dim)
                {
                    case 1:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField1"].ToString();
                        break;
                    case 2:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField2"].ToString();
                        break;
                    case 3:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField3"].ToString();
                        break;
                    default:
                        DIMSetting = lstVwAvsXDec_Settings_Config.Rows[0]["KeyField1"].ToString();
                        break;
                }
                /*asignare valori conform setarilor pe dimensiune*/
                switch (DIMSetting)
                {
                    case "ActionTypeId":
                        value = Convert.ToInt32(ent.Rows[0]["ActionTypeId"].ToString());
                        break;
                    case "PaymentTypeId":
                        value = Convert.ToInt32(ent.Rows[0]["PaymentTypeId"].ToString());
                        break;
                    case "ExpenseId":
                        lstCheltuieli.CaseSensitive = false;
                        DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                        if (itmDiurna != null)
                            value = Convert.ToInt32(itmDiurna[0]["DictionaryItemId"].ToString());
                        break;
                    default:
                        value = -99;
                        break;
                }
            }
        }

        protected void grDate_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                //General.SecuritatePersonal(grDateBeneficii, "Beneficii", Convert.ToInt32(Session["UserId"].ToString()), true);

                ASPxComboBox cmbChelt = grDate.FindEditFormTemplateControl("cmbChelt") as ASPxComboBox;
                if (cmbChelt != null)
                {
                    DataTable dtChelt = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                    cmbChelt.DataSource = dtChelt;
                    cmbChelt.DataBindItems();
                }
                //lblDet
                //txtDet
                HtmlTableCell lblDet = (HtmlTableCell)grDate.FindEditFormTemplateControl("lblDet");
                ASPxTextBox txtDet = grDate.FindEditFormTemplateControl("txtDet") as ASPxTextBox;
                if (lblDet != null && txtDet != null)
                    switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                    {
                        /*Avans spre deplasare*/
                        case 1001:
                            /*conform specificatiilor pentru tipul de document avans spre deplasare,
                             * data scadenta nu se poate edita, ci doar din baza de date se actualizeaza prin procedura
                             * procAvsXDec_dtPlata1001*/
                            lblDet.Visible = false;
                            txtDet.ClientVisible = false;
                            break;
                        /*Avans spre decontare*/
                        case 1002:
                            lblDet.Visible = true;
                            txtDet.ClientVisible = true;
                            break;
                        /*Avans Administrativ*/
                        case 1003:
                            //grDate.Columns["FreeTxt"].Visible = false;
          
                            break;
                    }

                //HtmlTableCell lblNume = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblNume");
                //lblNume.InnerText = Dami.TraduCuvant("Nume beneficiu");
                //HtmlTableCell lblDataPrimire = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblDataPrimire");
                //lblDataPrimire.InnerText = Dami.TraduCuvant("Data primire");
                //HtmlTableCell lblDataExp = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblDataExp");
                //lblDataExp.InnerText = Dami.TraduCuvant("Data expirare");
                //HtmlTableCell lblCaract = (HtmlTableCell)grDateBeneficii.FindEditFormTemplateControl("lblCaract");
                //lblCaract.InnerText = Dami.TraduCuvant("Caracteristica echipament");

                //ASPxUploadControl btnDocUploadBen = (ASPxUploadControl)grDateBeneficii.FindEditFormTemplateControl("btnDocUploadBen");
                //btnDocUploadBen.BrowseButton.Text = Dami.TraduCuvant("Incarca Document");
                //btnDocUploadBen.ToolTip = Dami.TraduCuvant("Incarca Document");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void GolireVariabile()
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

        private void txtStartDate_EditValueChanged()
        {
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 1001)
                return;
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 1001) /*avans spre deplasare*/
            {
                if (GetDayDateTime(Convert.ToDateTime(txtStartDate.Value)) < GetDayDateTime((ent.Rows[0]["DocumentDate"] == DBNull.Value ? new DateTime(1900, 1, 1) : Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString()))) /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data plecarii in delegatie trebuie sa fie ulterioara datei intocmirii avansului!");
                    txtStartDate.Value = ent.Rows[0]["DocumentDate"];
                    txtEndDate.Value = txtStartDate.Value;
                    return;
                }
            }

            if (GetDayDateTime(Convert.ToDateTime(txtEndDate.Value ?? Convert.ToDateTime(txtStartDate.Value))) < GetDayDateTime(Convert.ToDateTime(txtStartDate.Value)) /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data sosirii din delegatie trebuie sa fie ulterioara datei plecarii in delegatie!");
                txtEndDate.Value = txtStartDate.Value;
                return;
            }

            #region verificare data si ora pentru Avans spre deplasare
            if (txtOraPlecare.Value != null && Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 1001)
            {
                double TotalHours = 0;
                DateTime dataInceput = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());

                DateTime dtStartDay = string.IsNullOrEmpty(txtStartDate.Value.ToString()) ? Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString()) : Convert.ToDateTime(txtStartDate.Value);
                DateTime dtStartHour;
                if (!DateTime.TryParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStartHour))
                {
                    dtStartHour = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                }
                DateTime dataSfarsit = new DateTime(dtStartDay.Year, dtStartDay.Month, dtStartDay.Day, dtStartHour.Hour, dtStartHour.Minute, 0);

                int days = dataSfarsit.Subtract(dataInceput).Days + 1;
                TimeSpan ts;
                ts = dataSfarsit.Subtract(dataInceput);
                TotalHours = (ts.Hours + (ts.Minutes / 60.0)) * days;
                if (TotalHours < 0 /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data si ora plecarii in delegatie trebuie sa fie ulterioara datei intocmirii avansului!");
                    txtStartDate.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    txtEndDate.Value = txtStartDate.Value;
                    txtOraPlecare.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    txtOraSosire.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    return;
                }
            }
            #endregion

            SetDiurnaProperty();
            UpdateCheltuialaDiurna();
        }
        private void txtEndDate_EditValueChanged()
        {
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 1001)
                return;
            if (ent != null)
            {
                if (GetDayDateTime(Convert.ToDateTime(txtEndDate.Value ?? Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString()))) < GetDayDateTime(Convert.ToDateTime(ent.Rows[0]["DocumentDate"])) /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data sosirii din delegatii nu poate fi mai mica decat data intocmirii avansului!");
                    txtEndDate.Value = ent.Rows[0]["DocumentDate"];
                    return;
                }
            }

            if (GetDayDateTime(Convert.ToDateTime(txtEndDate.Value)) < GetDayDateTime(Convert.ToDateTime(txtStartDate.Value)) /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
            {
                pnlCtl.JSProperties["cpAlertMessage"] =  Dami.TraduCuvant("Data sosirii din delegatie trebuie sa fie ulterioara datei plecarii in delegatie!");
                txtEndDate.Value = txtStartDate.Value;
                return;
            }
            SetDiurnaProperty();
            UpdateCheltuialaDiurna();
        }
        private void txtOraPlecare_EditValueChanged()
        {
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 1001)
                return;

            #region verificare data si ora pentru Avans spre deplasare
            if (txtOraPlecare.Value != null && Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 1001 && txtStartDate.Value != null)
            {
                double TotalHours = 0;
                DateTime dataInceput = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());

                DateTime dtStartDay = string.IsNullOrEmpty(txtStartDate.Value.ToString()) ? Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString()) : Convert.ToDateTime(txtStartDate.Value);
                DateTime dtStartHour;
                if (!DateTime.TryParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStartHour))
                {
                    dtStartHour = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                }
                DateTime dataSfarsit = new DateTime(dtStartDay.Year, dtStartDay.Month, dtStartDay.Day, dtStartHour.Hour, dtStartHour.Minute, 0);

                int days = dataSfarsit.Subtract(dataInceput).Days + 1;
                TimeSpan ts;
                ts = dataSfarsit.Subtract(dataInceput);
                TotalHours = (ts.Hours + (ts.Minutes / 60.0)) * days;
                if (TotalHours < 0 /*&& dreptUserAccesFormular != tipAccesPagina.formularSalvat*/)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] =  Dami.TraduCuvant("Data si ora plecarii in delegatie trebuie sa fie ulterioara datei intocmirii avansului!");
                    txtStartDate.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    txtEndDate.Value = txtStartDate.Value;
                    txtOraPlecare.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    txtOraSosire.Value = Convert.ToDateTime(ent.Rows[0]["DocumentDate"].ToString());
                    return;
                }
            }
            #endregion
            SetDiurnaProperty();
            UpdateCheltuialaDiurna();
        }
        private void txtOraSosire_EditValueChanged()
        {
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 1001)
                return;

            SetDiurnaProperty();
            UpdateCheltuialaDiurna();
        }

        private void chkIsDiurna_EditValueChanged()
        {
            //DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            //if (Convert.ToBoolean(chkIsDiurna.Value.ToString()) == true)
            //{
            //    /*verificam sa fie completata modalitatea de deplasare, moneda timpul de start si final, numarul de ore, 
            //     * modalitatea plata si moneda*/
            //    #region verifDateComplete
            //    string campuriNecompletate = string.Empty;
            //    if (ent.Rows[0]["StartDate"] == DBNull.Value)
            //        campuriNecompletate += "data start;";
            //    if (string.IsNullOrEmpty(txtOraPlecare.Text) && ent.Rows[0]["StartHour"] == DBNull.Value)
            //        campuriNecompletate += ", ora inceput";
            //    else
            //    {
            //        if (ent.Rows[0]["StartHour"] == DBNull.Value)
            //        {
            //            ent.StartHour = DateTime.ParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            //            if (ent.Rows[0]["StartHour"] == DBNull.Value)
            //                campuriNecompletate += ", ora inceput";
            //        }
            //    }
            //    if (string.IsNullOrEmpty(txtOraSosire.Text) && ent.EndHour == null)
            //        campuriNecompletate += ", ora sfarsit";
            //    else
            //    {
            //        if (ent.Rows[0]["EndHour"] == DBNull.Value)
            //        {
            //            ent.EndHour = DateTime.ParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            //            if (ent.Rows[0]["EndHour"] == DBNull.Value)
            //                campuriNecompletate += ", ora sfarsit";
            //        }
            //    }
            //    if (ent.Rows[0]["EndHour"] == DBNull.Value)
            //        campuriNecompletate += "data final;";
            //    if (ent.PaymentTypeId == null)
            //        campuriNecompletate += "modalitate plata;";
            //    if (ent.CurrencyId == null)
            //        campuriNecompletate += "moneda plata";
            //    if (ent.ActionTypeId == null)
            //        campuriNecompletate += "tip deplasare;";
            //    if (!string.IsNullOrEmpty(campuriNecompletate))
            //    {
            //        ent.chkDiurna = false;
            //        ent.chkDiurnaInt = 0;
            //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat " + campuriNecompletate.Substring(0, campuriNecompletate.Length - 1) + " pentru a efectua calcul de diurna ok!");
            //        return;
            //    }
            //    else
            //    {
            //        decimal valDiurna;
            //        List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();

            //        /*verificam mai intai in lista daca exista deja diurna adaugata, pentru a nu o adauga aiurea
            //         * acest lucru ma ajuta si pentru cazurile in care am diurna completata pe avans si aleg un 
            //         * avans de acest gen. ca practic nu pun diurna de 2 ori pe avans*/
            //        metaAvsXDec_DictionaryItem itmDiurna_1 = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
            //        if (itmDiurna_1 == null)
            //            return;

            //        /*asteptam pana cand se incarca datele cu cheltuieli aferente,
            //         * pentru a sti sigur daca am sau nu de adaugat diurna*/
            //        if (ddsCheltuieli.IsLoadingData)
            //            return;

            //        var entCheltuialaDiurna = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().Where(p => p.DictionaryItemId == itmDiurna_1.DictionaryItemId).FirstOrDefault();
            //        if (entCheltuialaDiurna != null || lstCheltuieli == null || lstCheltuieli.Count() == 0)
            //            return;

            //        CalculDiurna(out valDiurna);
            //        tvDateCheltuieli.AddNewRow();
            //        try
            //        {
            //            InvokeOperation opApr = Constante.ctxAvansXDecont.GetNextId("AvsXDec_AvansDetail", 1);
            //            opApr.Completed += (s3, args3) =>
            //            {
            //                if (!opApr.HasError)
            //                {
            //                    if (opApr.Value.ToString() != "")
            //                    {
            //                        metaAvsXDec_AvansDetailCheltuieli entDiurna = grDateCheltuieli.GetRow(GridControl.NewItemRowHandle) as metaAvsXDec_AvansDetailCheltuieli;
            //                        entDiurna.DocumentId = idDocument;
            //                        entDiurna.DocumentDetailId = Convert.ToInt32(opApr.Value.ToString());
            //                        metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
            //                        entDiurna.DictionaryItemId = itmDiurna.DictionaryItemId;
            //                        entDiurna.Amount = valDiurna;
            //                        ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) + valDiurna;
            //                        maxValueDiurna = valDiurna;
            //                        tvDateCheltuieli.CommitEditing();
            //                    }
            //                }
            //                grDateCheltuieli.ShowLoadingPanel = false;
            //            };
            //        }
            //        catch (Exception ex)
            //        {
            //            Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            //        }
            //    }
            //    #endregion
            //}
            //else
            //{
            //    /*verificam daca exista cheltuiala de diurna si o stergem*/
            //    List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
            //    metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
            //    if (itmDiurna == null)
            //        return;
            //    var entCheltuialaDiurna = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().Where(p => p.DictionaryItemId == itmDiurna.DictionaryItemId).FirstOrDefault();
            //    if (entCheltuialaDiurna != null)
            //    {
            //        ent.EstimatedAmount -= entCheltuialaDiurna.Amount;
            //        ddsCheltuieli.DataView.Remove(entCheltuialaDiurna);
            //    }
            //}
        }

        private void ValidateAvansAmount()
        {
            DataTable ent = Session["AvsXDec_SursaDate"] as DataTable;
            double TotalAmount = Convert.ToDouble(ent.Rows[0]["TotalAmount"].ToString());
            double TotalAmountAvailable = 0;
            double procentCheltuieliTotaleAvansDeplasare = Convert.ToDouble(Dami.ValoareParam("AvsXDec_Modul_ProcentAvansDeplasareCheltuieliEstimate", "0"));
            double procentCheltuieliTotaleAvansDecontare = Convert.ToDouble(Dami.ValoareParam("AvsXDec_Modul_ProcentAvansDecontareCheltuieliEstimate", "0"));
            switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
            {
                case 1001:/*Avans spre deplasare*/
                    /*se verifica ca cheltuielile totale sa nu fie cheltuielile estimate + un procent 
                     * setat la nivel de parametrii*/
                    TotalAmountAvailable = Convert.ToDouble(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) + Convert.ToDouble(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) * procentCheltuieliTotaleAvansDeplasare;
                    if (TotalAmountAvailable < TotalAmount)
                    {
                        txtValAvans.Value = 0;
                        ent.Rows[0]["TotalAmount"] = 0;

                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti voie sa depasiti cheltuielile estimate cu mai mult de " + string.Format("{0:0}", Convert.ToDouble(procentCheltuieliTotaleAvansDeplasare * 100)) + "%! \n Valoare maxima: " + TotalAmountAvailable.ToString() + " !");
                    }
                    break;
                case 1002:/*Avans spre decontare*/
                    /*se verifica ca cheltuielile totale sa nu fie cheltuielile estimate + un procent 
                     * setat la nivel de parametrii*/
                    TotalAmountAvailable = Convert.ToDouble(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) + Convert.ToDouble(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) * procentCheltuieliTotaleAvansDecontare;
                    if (TotalAmountAvailable < TotalAmount)
                    {
                        txtValAvans.Value = 0;
                        ent.Rows[0]["TotalAmount"] = 0;

                        pnlCtl.JSProperties["cpAlertMessage"] =  Dami.TraduCuvant("Nu aveti voie sa depasiti cheltuielile estimate cu mai mult de " + string.Format("{0:0}", Convert.ToDouble(procentCheltuieliTotaleAvansDecontare * 100)) + "%! \n Valoare maxima: " + TotalAmountAvailable.ToString() + " !");
                    }
                    break;
            }
            Session["AvsXDec_SursaDate"] = ent;
        }
        private void txtValAvans_EditValueChanged()
        {
            ValidateAvansAmount();
        }
        private void txtValEstimata_EditValueChanged()
        {
            ValidateAvansAmount();
        }

        public static int GetDayDateTime(DateTime dtVerif)
        {
            return dtVerif.Year * 10000 + dtVerif.Month * 100 + dtVerif.Day;
        }
    }
}