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

                Session["statusDocOriginale"] = "Documente originale primite";
                Session["statusDocNeOriginale"] = "Documente originale neprimite";

                txtTitlu.Text = General.VarSession("Titlu").ToString() + " / Document Decont"; ;

                decimal suma = Convert.ToDecimal(General.Nz(Session["AvsXDec_SumaDecont"], 0));
                txtValDecont.Text = suma.ToString();

                if (!IsPostBack)
                {
                    Session["AvsXDec_SursaDateDec"] = null;
                    Session["AvsXDec_SumaDecont"] = null;
                    Session["AvsXDec_SursaDateCheltuieli"] = null;
                    Session["AvsXDec_Apasat"] = null;
                    Session["AvsXDec_cpAlertMessage"] = null;

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
                            string avansFolosit = "Nr. {0}; Stare {1}; Valoare: {2:0.00} {3}";
                            avansFolosit = string.Format(avansFolosit, lstAvsXDec_DocTypeXUser.Rows[0]["DocumentId"].ToString(),
                                                    lstAvsXDec_DocTypeXUser.Rows[0]["DocumentState"].ToString(),
                                                    Convert.ToDecimal(lstAvsXDec_DocTypeXUser.Rows[0]["TotalDocument"].ToString()),
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

                DataTable dtDocAvans = GetAvsXDec_AvansXDecont(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()), Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                cmbDocAvans.DataSource = dtDocAvans;
                cmbDocAvans.DataBind();


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

                btnAproba.ClientEnabled = Convert.ToInt32(General.Nz(Session["AvsXDec_PoateAprobaXRefuzaDoc"], 0).ToString()) == 1 ? true : false;
                //    /*&& Convert.ToInt32(General.Nz(Session["AvsXDec_IsBudgetOwnerEdited"], 0).ToString()) == 1 ? false : (Convert.ToInt32(General.Nz(Session["AvsXDec_PoateAprobaXRefuzaDoc"], 0).ToString()) == 1 ? true : false*/);
                btnRespins.ClientEnabled = Convert.ToInt32(General.Nz(Session["AvsXDec_DocCanBeRefused"], 0).ToString()) == 1 ? true : false;

                #endregion

                #region bifa documente originale
                //chkDecontDocPlataBancaoriginale.Visibility = System.Windows.Visibility.Collapsed;
                
                //btnDocOrig.ClientEnabled = Constante.AreRolContabilitate;
                
                if (btnDocOrig.Text == Session["statusDocOriginale"].ToString())
                    btnDocOrig.ClientEnabled = false;
                /*LeonardM 15.08.2016
                 * avem nevoie de acest buton, deoarece supervizorul trebuie sa poata salva linia de buget*/
                //if (/*Constante.AreRolContabilitate || Convert.ToInt32(General.Nz(Session["AvsXDec_IsBudgetOwnerEdited"], 0).ToString()) == 1*/)
                    btnSave.ClientEnabled = true;
                /*end LeonardM 15.08.2016*/
                #endregion

                PageXDocumentType();              
                


                DataTable dtDateDocJust = GetAvsXDec_DictionaryItemDocumenteDecont();
                GridViewDataComboBoxColumn col1 = (grDateDocJust.Columns["DictionaryItemId"] as GridViewDataComboBoxColumn);
                col1.PropertiesComboBox.DataSource = dtDateDocJust;

                dtDateDocJust = GetAvsXDec_DictionaryItemValute();
                GridViewDataComboBoxColumn col2 = (grDateDocJust.Columns["CurrencyId"] as GridViewDataComboBoxColumn);
                col2.PropertiesComboBox.DataSource = dtDateDocJust;

                dtDateDocJust = GetDecontExpenseType(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                GridViewDataComboBoxColumn col3 = (grDateDocJust.Columns["ExpenseTypeId"] as GridViewDataComboBoxColumn);
                col3.PropertiesComboBox.DataSource = dtDateDocJust;

                DataTable dtDocJust = new DataTable();
                if (!IsPostBack)
                {
                    dtDocJust = GetmetaAvsXDec_DecontDocumenteJustificative(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                    dtDocJust.PrimaryKey = new DataColumn[] { dtDocJust.Columns["DocumentDetailId"], dtDocJust.Columns["DocumentId"] };
                    Session["AvsXDec_SursaDateDocJust"] = dtDocJust;                   
                }
                else
                {
                    dtDocJust = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                    dtDocJust.PrimaryKey = new DataColumn[] { dtDocJust.Columns["DocumentDetailId"], dtDocJust.Columns["DocumentId"] };
                }
                grDateDocJust.KeyFieldName = "DocumentDetailId;DocumentId";
                grDateDocJust.DataSource = dtDocJust;
                grDateDocJust.DataBind();


                DataTable dtDateCheltEst = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                GridViewDataComboBoxColumn colEC1 = (grDateEstChelt.Columns["DictionaryItemId"] as GridViewDataComboBoxColumn);
                colEC1.PropertiesComboBox.DataSource = dtDateCheltEst;

                dtDateCheltEst = GetAvsXDec_DictionaryItemValute();
                GridViewDataComboBoxColumn colEC2 = (grDateEstChelt.Columns["CurrencyId"] as GridViewDataComboBoxColumn);
                colEC2.PropertiesComboBox.DataSource = dtDateCheltEst;

                DataTable dtCheltEst = new DataTable();
                if (!IsPostBack)
                {
                    dtCheltEst = GetmetaAvsXDec_DecontCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()), Convert.ToInt32(General.Nz(Session["AvsXDec_SrcDocId"], -99).ToString()));
                    dtCheltEst.PrimaryKey = new DataColumn[] { dtCheltEst.Columns["DocumentDetailId"], dtCheltEst.Columns["DocumentId"] };
                    Session["AvsXDec_SursaDateEstChelt"] = dtCheltEst;
                }
                else
                {
                    dtCheltEst = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                    dtCheltEst.PrimaryKey = new DataColumn[] { dtCheltEst.Columns["DocumentDetailId"], dtCheltEst.Columns["DocumentId"] };
                }
                grDateEstChelt.KeyFieldName = "DocumentDetailId;DocumentId";
                grDateEstChelt.DataSource = dtCheltEst;
                grDateEstChelt.DataBind();


                DataTable dtDatePlataBanca = GetAvsXDec_DictionaryItemDocumentePlataDecont();
                GridViewDataComboBoxColumn colPB1 = (grDatePlataBanca.Columns["DictionaryItemId"] as GridViewDataComboBoxColumn);
                colPB1.PropertiesComboBox.DataSource = dtDatePlataBanca;

                dtDatePlataBanca = GetAvsXDec_DictionaryItemValute();
                GridViewDataComboBoxColumn colPB2 = (grDatePlataBanca.Columns["CurrencyId"] as GridViewDataComboBoxColumn);
                colPB2.PropertiesComboBox.DataSource = dtDatePlataBanca;

                DataTable dtPlataBanca = new DataTable();
                if (!IsPostBack)
                {
                    dtPlataBanca = GetmetaAvsXDec_DecontDocumentePlataBanca(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                    dtPlataBanca.PrimaryKey = new DataColumn[] { dtPlataBanca.Columns["DocumentDetailId"], dtPlataBanca.Columns["DocumentId"] };
                    Session["AvsXDec_SursaDatePlataBanca"] = dtPlataBanca;
                }
                else
                {
                    dtPlataBanca = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                    dtPlataBanca.PrimaryKey = new DataColumn[] { dtPlataBanca.Columns["DocumentDetailId"], dtPlataBanca.Columns["DocumentId"] };
                }
                grDatePlataBanca.KeyFieldName = "DocumentDetailId;DocumentId";
                grDatePlataBanca.DataSource = dtPlataBanca;
                grDatePlataBanca.DataBind();

                IncarcaDate();
                ActualizareSumeDecont();

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

        public DataTable GetmetaAvsXDec_DecontDocumentePlataBanca(int documentId)
        {
            DataTable q = null;
            string sql = "";
            try
            {
                sql = "SELECT a.DocumentId, a.DocumentDetailId, a.CurrencyId, a.IdTipDocument as DictionaryItemId, a.DocDateDecont, a.DocNumberDecont, a.Furnizor, b.IdDocument, a.TotalPayment,  (CASE WHEN c.DocumentDetailId = 0 THEN 0 ELSE 1 END) as areFisierPlataBanca "
                    + " FROM vwAvsXDec_DecDet_DocPlataBanca a "
                    + " JOIN AvsXDec_DecontDetail b on a.DocumentDetailId = b.DocumentDetailId "
                    + " LEFT JOIN AvsXDec_relUploadDocumente c on a.DocumentId =  c.DocumentId AND a.DocumentDetailId = c.DocumentDetailId "
                    + "  where a.DocumentId = " + documentId;
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public DataTable GetAvsXDec_DictionaryItemDocumentePlataDecont()
        {
            DataTable q = null;
            string sql = "";
            try
            {
                sql = "SELECT a.DictionaryId, a.DictionaryItemId, a.DictionaryItemName, b.Ordine "
                    + " FROM vwAvsXDec_Nomen_DocPlataBanca a "
                    + " JOIN AvsXDec_DictionaryItem b  on a.DictionaryItemId = b.DictionaryItemId "
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


        public DataTable GetAvsXDec_DictionaryItemDocumenteDecont()
        {
            DataTable q = null;
            try
            {
                string sql = "SELECT a.DictionaryId, a.DictionaryItemId, a.DictionaryItemName FROM vwAvsXDec_Nomen_DocDecont a ";
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
                string sql = "SELECT a.DictionaryId, a.DictionaryItemId, a.DictionaryItemName, COALESCE(b.Ordine,-99) AS Ordine FROM vwAvsXDec_Nomen_TipMoneda a "
                            + " JOIN AvsXDec_DictionaryItem b on a.DictionaryItemId = b.DictionaryItemId ORDER BY Ordine";
                q = General.IncarcaDT(sql, null);
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetDecontExpenseType(int documentTypeId)
        {
            DataTable q = null;
            string sql = "";
            try
            {
                switch (documentTypeId)
                {
                    case 2001: /*Decontare avans*/
                        sql = "SELECT a.DictionaryId, a.DictionaryItemId, a.DictionaryItemName FROM vwAvsXDec_Nomen_TipCheltuieli a ";
                        break;
                    case 2002: /*Decontare avans spre decontare*/
                        sql = "SELECT a.DictionaryId, a.DictionaryItemId, a.DictionaryItemName FROM vwAvsXDec_Nomen_ChDecDecontare a ";
                        break;
                    case 2003: /*Decont Administrativ*/
                        //q = from a in this.ObjectContext.vwAvsXDec_Nomen_TipChDecAdmin
                        //    select new metaAvsXDec_DictionaryItem
                        //    {
                        //        DictionaryItemId = a.DictionaryItemId,
                        //        DictionaryId = a.DictionaryId,
                        //        DictionaryItemName = a.DictionaryItemName
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
                    + " where c.DocumentStateId < 1 AND a.DocumentTypeId = 1001 AND a.DocumentStateId >= 1 AND a.DocumentStateId < 8 AND a.F10003 = " + F10003 + " AND d.DocumentId IS NULL";
                q = General.IncarcaDT(sql, null);

                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public DataTable GetmetaAvsXDec_DecontDocumenteJustificative(int documentId)
        {
            DataTable q = null;
            string sql = "";
            try
            {
                sql = "SELECT a.DocumentId, a.DocumentDetailId, a.CurrencyId, a.IdTipDocument as DictionaryItemId, a.DocDateDecont, a.DocNumberDecont,  a.Furnizor,  b.IdDocument, a.RefCurrencyId, a.RefCurrencyValue, b.RefTotalPayment, "
                    + "  a.TotalPayment, a.ExpenseTypeId,  b.ExpenseTypeAdmId, b.BugetLine, (CASE WHEN c.DocumentDetailId = 0 THEN 0 ELSE 1 END) as areFisier, a.FreeTxt "
                    + " FROM vwAvsXDec_DecDet_DocDecontare a "
                    + " JOIN AvsXDec_DecontDetail b on a.DocumentId = b.DocumentId and a.DocumentDetailId = b.DocumentDetailId "
                    + " LEFT JOIN AvsXDec_relUploadDocumente c on  a.DocumentId = c.DocumentId and a.DocumentDetailId = c.DocumentDetailId "
                    + " WHERE a.DocumentId = " + documentId;
                q = General.IncarcaDT(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetmetaAvsXDec_DecontCheltuieli(int documentId, int documentIdAvans = -99)
        {
            DataTable q = null;
            string sql = "";
            DataTable bt = General.IncarcaDT("SELECT * FROM AvsXDec_BusinessTransaction Where SrcDocId = " + documentIdAvans + " AND DestDocId = " + documentId);
            try
            {
                /*0000 - nu am ales vreun document de tip avans pe decont
                 sau am ales si salvat un document de avans*/
                if (documentIdAvans == -99 || (documentIdAvans != -99 && bt != null && bt.Rows.Count > 0))
                {
                    sql = "SELECT a.DocumentId, a.DocumentDetailId, a.CurrencyId, a.IdTipCheltuiala as DictionaryItemId, a.RefCurrencyId, a.RefCurrencyValue, b.RefTotalPayment,  a.TotalPayment, a.BugetLine, 0 as toBeSaved,  a.FreeTxt "
                        + " FROM vwAvsXDec_DecDet_Cheltuieli a "
                        + " JOIN AvsXDec_DecontDetail b on a.DocumentDetailId = b.DocumentDetailId "
                        + " WHERE a.DocumentId = " + documentId;
                    q = General.IncarcaDT(sql, null);
                }
                /*0001 - se alege un avans din combo la initializare decont deplasare*/
                else
                {
                    sql = "SELECT " + documentId + " AS DocumentId,  b.DocumentDetailId,  c.CurrencyId, a.DictionaryItemId, c.CurrencyId as RefCurrencyId, b.Amount as RefCurrencyValue, b.Amount as RefTotalPayment, b.Amount as TotalPayment, "
                        + " '' as BugetLine, 0 as toBeSaved,  a.FreeTxt "
                        + " FROM vwAvsXDec_AvDet_Cheltuieli a "
                        + " JOIN AvsXDec_AvansDetail b on a.DocumentDetailId = b.DocumentDetailId and a.DocumentId = b.DocumentId "
                        + " JOIN AvsXDec_Avans c on b.DocumentId = c.DocumentId "
                        + " WHERE a.DocumentId = " + documentIdAvans;
                    q = General.IncarcaDT(sql, null);
                }

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

                    

                    /*end LeonardM 15.08.2016*/

                    /*LeonardM 12.08.2016
                     * utilizatorul care creeaza documentul nu are drept de editare pe coloana linie buget,
                     * ci doar cei de pe circuit (solicitare GroupamA)*/

                    if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                    {            

                        #region documente plata banca
                        grDatePlataBanca.Enabled = true;                       
                        #endregion
                    }
                    else
                    {
        
                    }
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
                    {
                        //grDateDocJust.Columns["ExpenseTypeAdmId"].Visible = false;
                    }
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
            MetodeCereri(1, "");
        }
        protected void btnRespins_Click(object sender, EventArgs e)
        {
            MetodeCereri(2, "");
        }

        private void MetodeCereri(int tipActiune, string motivRefuz, int tip = 1)
        {
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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
                                if (tip == 1)
                                    MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError, "Atentie !");
                                else
                                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                                Session["AvsXDec_Apasat"] = 1;
                                System.Threading.Thread.Sleep(5000);
                                btnBack_Click(null, null);
                            }



                    #endregion
                    //break;
                    //case LansatDin.PlataFinanciar:
                    #region document editat din financiar
                    string ras = RefuzaDocument(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(ent.Rows[0]["DocumentId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()), motivRefuz);

                    if (ras != "" && ras == "OK")
                    {
                        Session["AvsXDec_Apasat"] = 1;
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes."), MessageBox.icoError, "Atentie !");
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                        Session["AvsXDec_Apasat"] = 1;
                        System.Threading.Thread.Sleep(5000);
                        btnBack_Click(null, null);
                    }
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
                        if (tip == 1)
                            MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError, "Atentie !");
                        else
                            pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                        Session["AvsXDec_Apasat"] = 1;
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

        public string RefuzaDocument(int idUser, int DocumentId, int f10003, string refuseReason = "")
        {
            string msg = "";

            try
            {
                if (DocumentId == -99) return msg;

                int documentStateRefuzat = 0;

                DataTable ent = General.IncarcaDT("SELECT * FROM AvsXDec_Document WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString(), null);

                if (ent != null && ent.Rows.Count > 0)
                {
                    /*LeonardM 26.07.2016
                     * daca documentul a trecut deja din starea aprobat => nu se mai poate anula
                     * */
                    if (Convert.ToInt32(ent.Rows[0]["DocumentStateId"].ToString()) > 3)
                    {
                        msg = "Nu se poate anula documentul, deoarece a trecut de starea Aprobat!";
                        return msg;
                    }
                    /*end LeonardM 26.07.2016*/
                    DataTable entStr = General.IncarcaDT("SELECT * FROM AvsXDec_DictionaryItem Where DictionaryItemId =  " + documentStateRefuzat, null);
                    string culoare = "FFFFFFFF";
                    if (entStr != null && entStr.Rows.Count > 0 && entStr.Rows[0]["Culoare"] != null && entStr.Rows[0]["Culoare"].ToString().Length > 0) culoare = entStr.Rows[0]["Culoare"].ToString();

                    //schimbam statusul
                    General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId = " + documentStateRefuzat + ", Culoare = '" + culoare + "' WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString(), null);

                    //introducem o linie de anulare in AvsXDec_DocumentStateHistory
                    string sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
                        + " VALUES (" + Dami.NextId("AvsXDec_DocumentStateHistory", 1) + ", " + ent.Rows[0]["DocumentId"].ToString() + ", " + ent.Rows[0]["CircuitId"].ToString() + ", " + documentStateRefuzat
                        + ", 22, '" + culoare + "', 1, GETDATE(), " + idUser + ", GETDATE(), 0)";
                    General.ExecutaNonQuery(sql, null);

                    #region salvare motiv refuz
                    switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                    {
                        case 1001: /* Avans spre deplasare*/
                        case 1002: /* Avans spre decontare*/
                        case 1003: /*Avans administrativ*/
                            General.ExecutaNonQuery("UPDATE AvsXDec_Avans SET RefuseReason = '" + refuseReason + "' WHERE DocumentId = " + ent.Rows[0]["DocumentId"].ToString(), null);
                            break;
                        case 2001: /* Decont spre deplasare*/
                        case 2002: /*Decont cheltuieli*/
                        case 2003: /*Decont administrativ*/
                            General.ExecutaNonQuery("UPDATE AvsXDec_Decont SET RefuseReason = '" + refuseReason + "' WHERE DocumentId = " + ent.Rows[0]["DocumentId"].ToString(), null);
                            break;
                    }
                    #endregion

                    #region  Notificare strat

                    #region  Notificare strat

                    //ctxNtf.TrimiteNotificare("AvansXDecont.Document", "grDate", ent, idUser, f10003);

                    #endregion

                    #endregion


                    msg = "OK";

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                GolireVariabile();
                if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1 && Convert.ToInt32((Session["AvsXDec_Apasat"] ?? 0).ToString()) == 0)
                {
                    StergeDocument(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                }
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

        public void StergeDocument(int DocumentId)
        {
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                int DocumentTypeId = 1;
                if (ent != null && ent.Rows.Count > 0 && ent.Rows[0]["DocumentTypeId"] != null) DocumentTypeId = Convert.ToInt32(ent.Rows[0]["DocumentTypeId"].ToString());

                string strSql = "BEGIN " +
                    "delete from \"AvsXDec_DocumentStateHistory\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_BusinessTransaction\" where \"DestDocId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_AvansDetail\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_DecontDetail\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_Avans\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_Decont\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"AvsXDec_Document\" where \"DocumentId\"=" + DocumentId + "; " +
                    "delete from \"tblFisiere\" where \"Id\" in (select \"IdDocument\" from \"AvsXDec_relUploadDocumente\" where \"DocumentId\" = " + DocumentId + "); " +
                    " delete from \"AvsXDec_relUploadDocumente\" where \"DocumentId\" = " + DocumentId + "; " +
                    "END;";

                General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void SalvareInitiala()
        {
            try 
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;

                if (txtLocatie.Text.Length > 0) ent.Rows[0]["ActionPlace"] = txtLocatie.Text;
                if (txtActionReason.Text.Length > 0) ent.Rows[0]["ActionReason"] = txtActionReason.Text;
                if (txtStartDate.Value != null) ent.Rows[0]["StartDate"] = Convert.ToDateTime(txtStartDate.Value);
                if (txtEndDate.Value != null) ent.Rows[0]["EndDate"] = Convert.ToDateTime(txtEndDate.Value);
                if (txtOraPlecare.Value != null) ent.Rows[0]["StartHour"] = General.ChangeToCurrentYear(txtOraPlecare.DateTime);
                if (txtOraSosire.Value != null) ent.Rows[0]["EndHour"] = General.ChangeToCurrentYear(txtOraSosire.DateTime);
                if (cmbModPlata.Value != null) ent.Rows[0]["PaymentTypeId"] = Convert.ToInt32(cmbModPlata.Value);
                if (cmbMonedaAvans.Value != null) ent.Rows[0]["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value);
                if (cmbActionType.Value != null) ent.Rows[0]["ActionTypeId"] = Convert.ToInt32(cmbActionType.Value);

                Session["AvsXDec_SursaDateDec"] = ent;


                /*LeonardM 10.08.2016
                 * ne focusam pe un alt control pentru a putea salva in regula*/
                grDateDocJust.CancelEdit();
                grDateEstChelt.CancelEdit();
                grDatePlataBanca.CancelEdit();

                string ras = ValidareDateObligatorii();
                if (ras != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }

                /*LeonardM 23.08.2016
                 * pe deconturile de avans deplasare si cel de cheltuieli, ma interseaza sa nu pot salva decontul
                 * pana canu nu e completat si datele pentru restituire.
                 * la decontul administrativ, nu ma intereseaza acest lucru */
                if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 2003)
                {
                    ras = ValidareAvansRestituit();
                    if (ras != "")
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                        return;
                    }
                }

                ras = ValidareDateCompletate();
                if (ras != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }

                ras = ValidareDateDocumentJustificativ();
                if (ras != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }

                ras = ValidareMonedaDecont();
                if (ras != "")
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }
                string msg = "";
                ras = ValidareTipCheltuieli();
                if (ras != "")
                {
                    msg = ras + " \n\r Doriti salvarea decontului?";
                    ras = ValidareAtasDocJustificativ();
                    if (ras != "")
                        msg += "\n\r " + ras + " \n\r Doriti salvarea decontului fara a mai atasa documente justificative?";
                    pnlCtl.JSProperties["cpInfoMessage"] = msg;              
                }
                else
                {
                    ras = ValidareAtasDocJustificativ();
                    if (ras != "")
                    {
                        msg = "\n\r " + ras + " \n\r Doriti salvarea decontului fara a mai atasa documente justificative?";
                        pnlCtl.JSProperties["cpInfoMessage"] = msg;
                        return;
                    }
                    SalvareFinala();     
                }                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void SalvareFinala()
        {
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;

                /*LeonardM 11.09.2016
                 * in cazul in care documentul este editat de BudgetOwner, si se salveaza, tot in acest moment se si aproba documentul
                 * */
                if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0 /*&& IsBudgetOwnerEdited*/)
                {
                    MetodeCereri(1, "", 2);
                    return;
                }

                /*LeonardM 27.06.2016
                 * in cazul in care intra pe decont un user de contabilitate,
                 * acesta trebuie sa aiba posibilitatea de a puna bifa de documente originale, fara a schimba cricuitul*/
                if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                {
                    Session["AvsXDec_Apasat"] = 1;
                    btnBack_Click(null, null);
                    return;
                }
                DataTable entGrid = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                SalvareGrid(entGrid);
                entGrid = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                SalvareGrid(entGrid);
                entGrid = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                SalvareGrid(entGrid);
                
                SalvareDate();


                int rez = SetCircuitSettingsDocument(Convert.ToInt32(ent.Rows[0]["DocumentId"].ToString()));
                switch (rez)
                {
                    case -123:
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista circuit definit pentru proprietatile alese!");
                        break;
                    case 0:
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Date salvate cu succes.");
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
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;

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
                                
                DateTime dtPD = Convert.ToDateTime(General.Nz(ent.Rows[0]["DocumentDate"], "01/01/2100").ToString()); 
                string dataPD = dtPD.Day.ToString().PadLeft(2, '0') + "/" + dtPD.Month.ToString().PadLeft(2, '0') + "/" + dtPD.Year.ToString();

                //if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                {
                    sql = @"UPDATE AvsXDec_Decont SET CurrencyId = {0}, 
                    RefCurrencyId = null,
                    RefCurrencyValue = null,
                    TotalPayment = {17},
                    RefTotalPayment = null,
                    StartDate = {1},
                    EndDate = {2},
                    UnconfRestAmount = {18},
                    TotalAmount = {3},
                    USER_NO = {4},
                    TIME = GETDATE(),
                    StartHour = {5},
                    EndHour = {6},
                    EstimatedAmount = {7},
                    PaymentDate = {8},
                    PaymentTypeId = {9},
                    ActionTypeId = {10},
                    ActionPlace = '{11}',
                    ActionReason ='{12}',
                    chkDiurna = {13},
                    OriginalDoc = {14},
                    TransportTypeId = {15}
                    WHERE DocumentId =  {16}";
                   sql = string.Format(sql, Convert.ToInt32(cmbMonedaAvans.Value ?? 0), "CONVERT(DATETIME, '" + dataStart + "', 103)", "CONVERT(DATETIME, '" + dataEnd + "', 103)", (txtValDecont.Text.Length <= 0 ? "0" : txtValDecont.Text.Replace(',', '.')), entDocument.Rows[0]["USER_NO"].ToString(), oraSt, oraEnd, (txtValDecont.Text.Length <= 0 ? "0" : txtValDecont.Text.Replace(',', '.')), "CONVERT(DATETIME, '" + dataPD + "', 103)",
                        Convert.ToInt32(cmbModPlata.Value ?? 0), Convert.ToInt32(cmbActionType.Value ?? 0), txtLocatie.Text, txtActionReason.Text, (chkIsDiurna.Checked ? "1" : "0"), General.Nz(ent.Rows[0]["OriginalDoc"], 0).ToString(), Convert.ToInt32(cmbTransportType.Value ?? 0), Session["AvsXDec_IdDocument"].ToString(), 
                        (txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text.Replace(',', '.')),
                        /*LeonardM 15.08.2016
                    * nu mai actualizez restul de plata deoarece acesta se actualizeaza
                    * prin financiar/ conta*/
                        ( Convert.ToInt32(ent.Rows[0]["DocumentStateId"].ToString()) <= 4 ? Convert.ToDecimal(txtValDecont.Text.Length <= 0 ? "0" : txtValDecont.Text) - Convert.ToDecimal(txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text) : Convert.ToDecimal(ent.Rows[0]["UnconfRestAmount"].ToString()) ).ToString().Replace(',', '.'));
                    General.ExecutaNonQuery(sql, null); 
                }

                Session["AvsXDec_Apasat"] = 1;
                //else
                //{
                //    sql = @"INSERT INTO AvsXDec_Avans (CurrencyId, RefCurrencyId, RefCurrencyValue, TotalPayment, RefTotalPayment, StartDate, EndDate, UnconfRestAmount, TotalAmount, USER_NO, TIME, StartHour,
                //    EndHour, EstimatedAmount, DueDate, PaymentTypeId, ActionTypeId, ActionPlace, ActionReason, chkDiurna, RefuseReason, AccPeriodId,PeriodOfAccountId, TransportTypeId)
                //    VALUES ({0}, null, null, null, null, {1}, {2}, null, {3}, {4}, GETDATE(), {5}, {6}, {7}, {8}, {9}, {10}, '{11}', '{12}', {13}, '{14}', null, null, {15}) ";
                //    sql = string.Format(sql, Convert.ToInt32(cmbMonedaAvans.Value ?? 0), "CONVERT(DATETIME, '" + dataStart + "', 103)", "CONVERT(DATETIME, '" + dataEnd + "', 103)", (txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text), entDocument.Rows[0]["USER_NO"].ToString(), oraSt, oraEnd, (txtValAvans.Text.Length <= 0 ? "0" : txtValEstimata.Text), "CONVERT(DATETIME, '" + dataDue + "', 103)",
                //         Convert.ToInt32(cmbModPlata.Value ?? 0), Convert.ToInt32(cmbActionType.Value ?? 0), txtLocatie.Text, txtActionReason.Text, (chkIsDiurna.Checked ? "1" : "0"), "", Convert.ToInt32(cmbTransportType.Value ?? 0), Session["AvsXDec_IdDocument"].ToString());
                //    General.ExecutaNonQuery(sql, null);
                //}


                /*salvare BusinessTransaction*/
                DataTable bt = General.IncarcaDT("SELECT * FROM AvsXDec_BusinessTransaction Where DestDocId = " + Session["AvsXDec_IdDocument"].ToString());
                if (bt == null || bt.Rows.Count == 0)
                {
                    /*nu exista legatura intre avans si decont, trebuie inserat un rand nou*/
                    sql = "INSERT INTO AvsXDec_BusinessTransaction (TransactionId, SrcDocId, DestDocId, SrcDocAmount, DestDocAmount, USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, GETDATE())";
                    sql = string.Format(sql, Dami.NextId("AvsXDec_BusinessTransaction", 1), General.Nz(ent.Rows[0]["AvansDocumentId"], -99).ToString(), Session["AvsXDec_IdDocument"].ToString(),
                         General.Nz(ent.Rows[0]["TotalAmountAvans"], 0).ToString().Replace(',', '.'), General.Nz(ent.Rows[0]["EstimatedAmount"], 0).ToString().Replace(',', '.'), entDocument.Rows[0]["USER_NO"].ToString());
                    General.ExecutaNonQuery(sql, null);
                }
                else
                {
                    sql = "UPDATE AvsXDec_BusinessTransaction SET SrcDocId = {0}, DestDocId = {1}, SrcDocAmount = {2}, DestDocAmount = {3}, USER_NO = {4}, TIME = GETDATE() WHERE TransactionId = {5}";
                    sql = string.Format(sql, General.Nz(ent.Rows[0]["AvansDocumentId"], -99).ToString(), Session["AvsXDec_IdDocument"].ToString(),
                            General.Nz(ent.Rows[0]["TotalAmountAvans"], 0).ToString().Replace(',', '.'), General.Nz(ent.Rows[0]["EstimatedAmount"], 0).ToString().Replace(',', '.'), entDocument.Rows[0]["USER_NO"].ToString(),
                            bt.Rows[0]["TransactionId"].ToString());
                    General.ExecutaNonQuery(sql, null);     
                }


                #region prelucrare documente
                switch (Convert.ToInt32(ent.Rows[0]["DocumentTypeId"].ToString()))
                {
                    case 2001:/*Decont avans spre deplasare*/
                    case 2002:/*Decont fonduri proprii*/
                    case 2003:/*Decont administrativ*/
                        /*LeonardM 11.08.2016
                         * in cazul deconturilorla momentul contarii documentelor,
                         * se verifica daca documentul are pusa bifa deja de documente originale
                         * caz in care, documentul se trece automat si in starea inchis,
                         * daca decontul nu are pusa bifa de documente originale, urmeaza ca dupa contare,
                         * utilizatorul sa poata vedea documentele care au fost contate si sa le puna bifa de documente originale,
                         * moment in care se inchide decontul*/
                        if (Convert.ToInt32(ent.Rows[0]["DocumentStateId"].ToString()) == 7)/*Contat*/
                        {
                            if (Convert.ToInt32(General.Nz(ent.Rows[0]["OriginalDoc"], 0).ToString()) == 1)
                            {
                                DataTable entStrInchis = General.IncarcaDT("SELECT * FROM AvsXDec_DictionaryItem Where DictionaryItemId = 8", null);
                                string culoare = "FFFFFFFF";
                                if (entStrInchis != null && entStrInchis.Rows.Count > 0 && entStrInchis.Rows[0]["Culoare"] != null && entStrInchis.Rows[0]["Culoare"].ToString().Length > 0) 
                                    culoare = entStrInchis.Rows[0]["Culoare"].ToString();

                                int UserIdConta = Convert.ToInt32(entDocument.Rows[0]["USER_NO"].ToString());
                                DataTable entDocConta = General.IncarcaDT("SELECT * FROM AvsXDec_DocumentStateHistory Where DocumentId = " + entDocument.Rows[0]["DocumentId"].ToString() + " AND DocumentStateId = 7", null);
                                if (entDocConta != null && entDocConta.Rows.Count > 0 && entDocConta.Rows[0]["USER_NO"] != null) UserIdConta = Convert.ToInt32(entDocConta.Rows[0]["USER_NO"].ToString());

                                sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
                                    + " VALUES ({0}, {1}, {2}, 8, 22, '{3}', 1, GETDATE(), {4}, GETDATE, 0)";
                                sql = string.Format(sql, Dami.NextId("AvsXDec_DocumentStateHistory", 1), Session["AvsXDec_IdDocument"].ToString(), ent.Rows[0]["CircuitId"].ToString(), culoare, UserIdConta);
                                General.ExecutaNonQuery(sql, null);                   

                                General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId = 8 WHERE DocumentId = " + Session["AvsXDec_IdDocument"].ToString(), null);                              
                            }
                        }
                        break;
                }
                #endregion

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void SalvareGrid(DataTable ent)
		{
            DataTable dt = new DataTable();

            dt.Columns.Add("DocumentId", typeof(int));
            dt.Columns.Add("DocumentDetailId", typeof(int));
            dt.Columns.Add("Furnizor", typeof(string));
            dt.Columns.Add("DictionaryItemId", typeof(int));
            dt.Columns.Add("DocNumberDecont", typeof(string));
            dt.Columns.Add("DocDateDecont", typeof(DateTime));
            dt.Columns.Add("CurrencyId", typeof(int));
            dt.Columns.Add("RefCurrencyId", typeof(int));
            dt.Columns.Add("RefCurrencyValue", typeof(decimal));
            dt.Columns.Add("TotalPayment", typeof(decimal));
            dt.Columns.Add("RefTotalPayment", typeof(decimal));
            dt.Columns.Add("IdDocument", typeof(int));
            dt.Columns.Add("BugetLine", typeof(string));
            dt.Columns.Add("ExpenseTypeId", typeof(int));
            dt.Columns.Add("ExpenseTypeAdmId", typeof(int));
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
                        case "Furnizor":
                            row["Furnizor"] = ent.Rows[i]["Furnizor"];
                            break;
                        case "DocNumberDecont":
                            row["DocNumberDecont"] = ent.Rows[i]["DocNumberDecont"];
                            break;
                        case "DocDateDecont":
                            row["DocDateDecont"] = ent.Rows[i]["DocDateDecont"];
                            break;
                        case "CurrencyId":
                            row["CurrencyId"] = ent.Rows[i]["CurrencyId"];
                            break;
                        case "TotalPayment":
                            row["TotalPayment"] = ent.Rows[i]["TotalPayment"];
                            break;
                        case "BugetLine":
                            row["BugetLine"] = ent.Rows[i]["BugetLine"];
                            break;
                        case "ExpenseTypeId":
                            row["ExpenseTypeId"] = ent.Rows[i]["ExpenseTypeId"];
                            break;
                        case "IdDocument":
                            row["IdDocument"] = ent.Rows[i]["IdDocument"];
                            break;   
                        case "FreeTxt":
                            row["FreeTxt"] = ent.Rows[i]["FreeTxt"];
                            break;
                    }                
                }                
                dt.Rows.Add(row);
            }
            General.SalveazaDate(dt, "AvsXDec_DecontDetail");
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


        private string ValidareAvansRestituit()
        {
            /*LeonardM 15.08.2016
             * in momentul in care utilizatorul are de restiuit bani catre Groupama, acesta trebuie sa introduca documente justificatove
             * + suma documentelor de plata banca trebuie sa fie = suma de restituit catre Groupama
             * */
            string ras = "";
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                DataTable lstDocPlataBanca = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                decimal sumDocumentePlataBanca = 0;
                if (Convert.ToInt32(General.Nz(ent.Rows[0]["UnconfRestAmount"], 0)) < 0) /*angajatul trebuie sa restituie bani catre firma*/
                {
                    if (lstDocPlataBanca == null || lstDocPlataBanca.Rows.Count == 0)
                    {
                        sumDocumentePlataBanca = 0;
                        ras = "Nu aveti completate documente care sa justifice restituirea avansului ramas!";
                    }
                    else
                        sumDocumentePlataBanca = Convert.ToDecimal(lstDocPlataBanca.Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"));

                    if (Math.Abs(sumDocumentePlataBanca).ToString("F") != Math.Abs(Convert.ToInt32(General.Nz(ent.Rows[0]["UnconfRestAmount"], 0))).ToString("F") && ras == "")
                        ras = "Suma aferenta documentelor pentru restituire avans ramas este diferita de suma de restituit!";
                }
            }
            catch (Exception ex)
            {
            }
            return ras;
        }
        private string ValidareDateCompletate()
        {
            /*LeonardM 15.08.2016
             * in momentul in care se salveaza documentul, trebuie neapart completate proprietatile de tip document
             * completat
             * */
            string ras = "";
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                DataTable lstDocPlataBanca = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                if (lstDocPlataBanca != null && lstDocPlataBanca.Rows.Count != 0)
                {
                    if (lstDocPlataBanca.Select("ISNULL(DictionaryItemId, -99) = -99").Count() != 0)
                    {
                        ras += ", Documente plata banca: tipul documentului cu care s-a platit restul de plata";
                    }
                }   
                DataTable lstDocJustificative = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                if (lstDocJustificative != null && lstDocJustificative.Rows.Count != 0)
                {
                    if (lstDocJustificative.Select("ISNULL(DictionaryItemId, -99) == -99").Count() != 0)
                    {
                        ras += ", Documente justificative: tipul documentului de decontare";
                    }
                }
                if (ras != "") ras = "Lipseste " + ras.Substring(2) + "!";

            }
            catch (Exception ex)
            {
            }
            return ras;
        }

        private string ValidareDateDocumentJustificativ()
        {
            /*LeonardM 25.08.2016
             pentru avansul spre deplasare nu trebuie sa se completeze date care nu fac parte din intervalul data
             start <-> data final*/
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            try
            {
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 2001: /*Decont avans spre deplasare*/
                        #region verificare date documente justificative conform interval deplasareavans
                        for (int i = 0; i < lstCheltuieliInserate.Rows.Count; i++)                        
                        {
                            DateTime dtDocument = Convert.ToDateTime(lstCheltuieliInserate.Rows[i]["DocDateDecont"].ToString());
                            DateTime dtStartDate = Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString());
                            DateTime dtEndDate = Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString());
                            if (!(GetDayDateTime(dtStartDate) <= GetDayDateTime(dtDocument) && GetDayDateTime(dtEndDate) >= GetDayDateTime(dtDocument)))
                            {
                                ras += ", data de pe documentele justificative trebuie sa fie specifice intervalului de deplasare";
                                break;
                            }
                        }
                        #endregion
                        break;
                }
                if (ras != "") ras = "Atentie: " + ras.Substring(2) + " !";
            }
            catch (Exception)
            {
            }

            return ras;
        }


        private string ValidareMonedaDecont()
        {
            /*LeonardM 11.09.2016
             moneda de pe decont trebuie sa fie unica*/
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            try
            {
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                DataTable lstDocumentePlataBanca = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 2002: /*Decont cheltuieli*/
                    case 2001: /*Decont avans spre deplasare*/
                        if (lstCheltuieliInserate.Select("CurrencyId <> " + ent.Rows[0]["CurrencyId"].ToString()).Count() != 0)
                            ras = ", moneda selectata pentru decont trebuie sa fie aceleasi cu documentele justificative";

                        #region verificare moneda unica documente justificative
                        if (string.IsNullOrEmpty(ras))
                        {
                            int CurrencyId = Convert.ToInt32(lstCheltuieliInserate.Rows[0]["CurrencyId"].ToString());
                            if (lstCheltuieliInserate.Select("CurrencyId <> " + CurrencyId).Count() != 0)
                                ras = ",  moneda selectata pentru documentele justificative trebuie sa fie unica";
                            break;
                        }
                        #endregion

                        break;
                }
                if (ras != "") ras = "Atentie: " + ras.Substring(2) + " !";
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            try
            {
                DataTable lstCheltuieliAvans = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 2001: /*Decont avans spre deplasare*/
                        #region verificare tip cheltuieli conform avans
                        if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                        {
                            for (int i = 0; i < lstCheltuieliInserate.Rows.Count; i++)
                            {
                                if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(lstCheltuieliInserate.Rows[i]["ExpenseTypeId"].ToString())).Count() == 0)
                                {
                                    ras += ", tipurile de cheltuieli alese nu exista pe avans";
                                    break;
                                }
                            }
                        }
                        #endregion
                        break;
                    case 2002: /*Decont avans spre decontare*/
                        //DataTable lstCheltuieli = GetDecontExpenseType(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                        int idTipCheltuiala = -99;
                        for (int i =0; i < lstCheltuieliInserate.Rows.Count; i++)
                        {
                            if (idTipCheltuiala == -99)
                            {
                                idTipCheltuiala = Convert.ToInt32(General.Nz(lstCheltuieliInserate.Rows[i]["ExpenseTypeId"], -99));
                                continue;
                            }
                            if (Convert.ToInt32(General.Nz(lstCheltuieliInserate.Rows[i]["ExpenseTypeId"], -99)) != idTipCheltuiala)
                            {
                                ras += ", se completeaza doar un singur tip de cheltuiala pentru decontul simplu!";
                                break;
                            }
                        }
                        #region verificare tip cheltuieli conform avans
                        if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                        {
                            for (int i = 0; i < lstCheltuieliInserate.Rows.Count; i++)
                            {
                                if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(lstCheltuieliInserate.Rows[i]["ExpenseTypeId"], -99))).Count() == 0)
                                {
                                    ras += ", tipurile de cheltuieli alese nu exista pe avans";
                                    break;
                                }
                            }
                        }
                        #endregion
                        break;
                }
                if (ras != "") ras = "Atentie: " + ras.Substring(2) + " !";
            }
            catch (Exception)
            {
            }

            return ras;
        }

        private string ValidareAtasDocJustificativ()
        {
            /*LeonardM 13.09.2016
             verificare daca exista documente justificative fara atasament*/
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            try
            {
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    case 2002: /*Decont cheltuieli*/
                    case 2001: /*Decont avans spre deplasare*/
                    case 2003: /*Decont administrativ*/
                        #region verificare date documente justificative conform interval deplasareavans
                        if (lstCheltuieliInserate.Select("areFisier = 0").Count() != 0)
                            ras = "Va rugam sa atasati documentele justificative!";
                        #endregion
                        break;
                }
            }
            catch (Exception)
            {
            }

            return ras;
        } 

        private string ValidareDateObligatorii()
        {
            string ras = "";
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable; 
            try
            {
                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                {
                    /*Decont cheltuieli deplasare*/
                    case 2001:
                    /*Decont cheltuieli*/
                    case 2002:
                        /*LeonardM 15.06.2016
                         * cnform specificatiilor nu se mai doreste aceste verificari
                         * deoarece este vorba de un decont, nu de avans deplasare
                        if (txtLocatie.EditValue == null) ras += ", locatie";
                        if (txtStartDate.EditValue == null) ras += ", data inceput";
                        if (txtEndDate.EditValue == null) ras += ", data sfarsit";
                         * */
                        /*LeonardM 25.08.2016
                         * pentru a fi verificate datele doc justificative
                         * trebuie ca datele de deplsare pentru decont avans spre deplasare
                         * sa fie completate*/
                        if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2001)
                        {
                            if (ent.Rows[0]["StartDate"] == DBNull.Value) ras += ", data inceput";
                            if (ent.Rows[0]["EndDate"] == DBNull.Value) ras += ", data sfarsit";
                        }
                        DataTable dtDocJust = Session["AvsXDec_SursaDateDocJust"] as DataTable;// GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                        if (dtDocJust != null && dtDocJust.Rows.Count != 0 && dtDocJust.Select("ExpenseTypeId IS NULL").Count() != 0)
                            ras += ", tipul cheltuielii in gridul de documente justificative";
   
                        break;
                    case 2003:/*Decont Administrativ*/
                        /*List<metaAvsXDec_DecontDocumenteJustificative> lstDocJustificativeDecAdmin = ddsDocumenteJustifDecont.DataView.Cast<metaAvsXDec_DecontDocumenteJustificative>().ToList();
                        if (lstDocJustificativeDecAdmin.Where(p => (p.ExpenseTypeAdmId ?? -99) == -99).Count() != 0)
                            ras += ", tipul cheltuielii in gridul de documente justificative";*/
                        break;
                }	
                if (ras != "") ras = "Date lipsa: " + ras.Substring(2) + " !";
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

        protected void btnDocOrig_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                if (ent == null || ent.Rows.Count == 0)
                    return;

                if (Convert.ToInt32(General.Nz(ent.Rows[0]["OriginalDoc"], 0)) == 1)
                {
                    ent.Rows[0]["OriginalDoc"] = 0;
                    btnDocOrig.Text = Session["statusDocNeOriginale"].ToString();
                }
                else
                {
                    ent.Rows[0]["OriginalDoc"] = 1;
                    btnDocOrig.Text = Session["statusDocOriginale"].ToString();
                    btnDocOrig.ClientEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUploadDJ_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaCereriDate itm = new metaCereriDate();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_AvsXDec_DJ"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUploadPB_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaCereriDate itm = new metaCereriDate();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_AvsXDec_PB"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
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
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                switch (e.Parameter.Split(';')[0])
                {
                    case "cmbMonedaAvans":
                        ent.Rows[0]["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value ?? -99);
                        Session["AvsXDec_SursaDateDec"] = ent;
                        cmbMonedaAvans_SelectedIndexChanged();
                        break;
                    case "cmbActionType":
                        ent.Rows[0]["ActionTypeId"] = Convert.ToInt32(cmbActionType.Value ?? -99);
                        Session["AvsXDec_SursaDateDec"] = ent;
                        cmbActionType_EditValueChanged();
                        break;
                    case "btnRespinge":
                        MetodeCereri(2, e.Parameter.Split(';')[1], 2);
                        break;
                    case "txtStartDate":
                        ent.Rows[0]["StartDate"] = Convert.ToDateTime(txtStartDate.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDateDec"] = ent;
                        txtStartDate_EditValueChanged();
                        break;
                    case "txtEndDate":
                        ent.Rows[0]["EndDate"] = Convert.ToDateTime(txtEndDate.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDateDec"] = ent;
                        txtEndDate_EditValueChanged();
                        break;
                    case "txtOraPlecare":
                        ent.Rows[0]["StartHour"] = Convert.ToDateTime(txtOraPlecare.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDateDec"] = ent;
                        txtOraPlecare_EditValueChanged();
                        break;
                    case "txtOraSosire":
                        ent.Rows[0]["EndHour"] = Convert.ToDateTime(txtOraSosire.Value ?? new DateTime(2100, 1, 1));
                        Session["AvsXDec_SursaDateDec"] = ent;
                        txtOraSosire_EditValueChanged();
                        break;
                    case "txtValDecont":
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(txtValDecont.Value ?? -99);
                        Session["AvsXDec_SursaDateDec"] = ent;
                        ActualizareSumeDecont();
                        break;
                    case "txtValAvans":
                        ent.Rows[0]["TotalAmount"] = Convert.ToInt32(txtValAvans.Value ?? -99);
                        Session["AvsXDec_SursaDateDec"] = ent;
                        ActualizareSumeDecont();
                        break;
                    case "chkIsDiurna":
                        ent.Rows[0]["chkDiurna"] = chkIsDiurna.Checked ? 1 : 0;
                        Session["AvsXDec_SursaDateDec"] = ent;
                        chkIsDiurna_EditValueChanged();
                        break;
                    case "btnSave":
                        SalvareInitiala();
                        break;
                    case "btnSaveConf":
                        SalvareFinala();
                        break;
                    case "cmbDocAvans":
                        foreach (DataColumn col in ent.Columns)
                            col.ReadOnly = false;
                        ent.Rows[0]["AvansDocumentId"] = Convert.ToInt32(cmbDocAvans.Value ?? -99);
                        Session["AvsXDec_SursaDateDec"] = ent;
                        cmbDocAvans_SelectedIndexChanged();
                        break;
                    case "SumaDecont":
                        decimal suma = Convert.ToDecimal(General.Nz(Session["AvsXDec_SumaDecont"], 0));
                        txtValDecont.Text = suma.ToString();
                        ent.Rows[0]["EstimatedAmount"] = suma;
                        Session["AvsXDec_SursaDateDec"] = ent;
                        ActualizareSumeDecont();
                        break;
                }
                if (Session["AvsXDec_cpAlertMessage"] != null)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Session["AvsXDec_cpAlertMessage"];
                    Session["AvsXDec_cpAlertMessage"] = null;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void cmbDocAvans_SelectedIndexChanged()
        {
            try
            {

                int documentIdAvans = Convert.ToInt32(cmbDocAvans.Value ?? -99);

                /*in cazul in care a selectat un avans, atunci se modifica datele
                 * initializam mai intai cheltuielile de decont,
                 * pentru a sti in momentul in care se pune bifa de diurna, daca am deja diurna sau nu*/

                DataTable dtCheltEst = new DataTable();              
                dtCheltEst = GetmetaAvsXDec_DecontCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()), documentIdAvans);
                dtCheltEst.PrimaryKey = new DataColumn[] { dtCheltEst.Columns["DocumentDetailId"], dtCheltEst.Columns["DocumentId"] };
                Session["AvsXDec_SursaDateEstChelt"] = dtCheltEst;        
                grDateEstChelt.KeyFieldName = "DocumentDetailId;DocumentId";
                grDateEstChelt.DataSource = dtCheltEst;
                grDateEstChelt.DataBind();

                Session["AvsXDec_SursaDateDec"] = null;
                IncarcaDate(documentIdAvans);           

                /*LeonardM 15.08.2016
                * in momentul in care am selectat ceva ca avans, se afiseaza gridul aferent cheltuielilor estimate de 
                * pe avans; caz contrar, nu se afiseaza
                 LeonardM 22.08.2016
                 pentru decontul administrativ, nu ma interseaza sa apara estimarea de cheltuieli*/
                if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99 && Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 2003)
                    pnlEstChelt.Visible = true;
                else
                    pnlEstChelt.Visible = false;

                //Radu 01.09.2016 - daca se selecteaza un avans, moneda si modalitatea de plata trebuie blocate
                //if (cmbDocAvans.Value != null)
                //{
                //    cmbMonedaAvans.ClientEnabled = false;
                //    cmbModPlata.ClientEnabled = false;
                //    grDateDocJust.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                //    grDateEstChelt.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                //    grDatePlataBanca.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                //}
                //else
                //{
                //    cmbMonedaAvans.ClientEnabled = true;
                //    cmbModPlata.ClientEnabled = true;
                //    grDateDocJust.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                //    grDateEstChelt.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                //    grDatePlataBanca.Columns["CurrencyId"].AllowEditing = DevExpress.Utils.DefaultBoolean.True;
                //}

       
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaDate(int documentIdAvans = -99)
        {
            try
            {
                DataTable ent = null;

                if (documentIdAvans == -99)
                    documentIdAvans = Convert.ToInt32(General.Nz(Session["AvsXDec_SrcDocId"], -99).ToString());
                bool reload = false;

                if (Session["AvsXDec_SursaDateDec"] == null)
                {
                    ent = GetmetaVwAvsXDec_Decont(documentIdAvans);
                    Session["AvsXDec_SursaDateDec"] = ent;
                    reload = true;
                }
                else
                    ent = Session["AvsXDec_SursaDateDec"] as DataTable;

                string sql = "";
                if (ent != null && ent.Rows.Count > 0)
                {
                    txtDecontNo.Text = ent.Rows[0]["DocumentId"].ToString();
                    txtNumeComplet.Text = ent.Rows[0]["NumeComplet"].ToString();
                    txtDepartament.Text = ent.Rows[0]["Departament"].ToString();
                    txtLocMunca.Text = ent.Rows[0]["LocMunca"].ToString();
                    txtContIban.Text = ent.Rows[0]["ContBancar"].ToString();
                    Session["IdFunctieAngajat"] = Convert.ToInt32((ent.Rows[0]["IdFunctie"] == DBNull.Value ? -99 : ent.Rows[0]["IdFunctie"]).ToString());

                    if (Convert.ToInt32(General.Nz(ent.Rows[0]["OriginalDoc"], 0).ToString()) == 1)
                    {
                        btnDocOrig.Text = Session["statusDocOriginale"].ToString();
                        btnDocOrig.ClientEnabled = false;
                    }
                    else
                        btnDocOrig.Text = Session["statusDocNeOriginale"].ToString();

                    if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 1 && Convert.ToInt32(General.Nz(ent.Rows[0]["AvansDocumentId"], -99).ToString()) == -99)
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
                    }
                    else
                    {
                        if (!IsPostBack || reload)
                        {
                            cmbDocAvans.Value = Convert.ToInt32(General.Nz(ent.Rows[0]["AvansDocumentId"], -1).ToString());
                            txtLocatie.Text = ent.Rows[0]["ActionPlace"].ToString();
                            cmbActionType.Value = Convert.ToInt32(ent.Rows[0]["ActionTypeId"].ToString());
                            cmbTransportType.Value = Convert.ToInt32(ent.Rows[0]["TransportTypeId"].ToString());
                            txtActionReason.Text = ent.Rows[0]["ActionReason"].ToString();
                            txtStartDate.Value = Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString());
                            if (ent.Rows[0]["StartHour"] != DBNull.Value)
                                txtOraPlecare.Value = Convert.ToDateTime(ent.Rows[0]["StartHour"].ToString());
                            txtEndDate.Value = Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString());
                            if (ent.Rows[0]["EndHour"] != DBNull.Value)
                                txtOraSosire.Value = Convert.ToDateTime(ent.Rows[0]["EndHour"].ToString());
                            cmbMonedaAvans.Value = Convert.ToInt32(ent.Rows[0]["CurrencyId"].ToString());
                            cmbModPlata.Value = Convert.ToInt32(ent.Rows[0]["PaymentTypeId"].ToString());
                            chkIsDiurna.Checked = Convert.ToInt32(General.Nz(ent.Rows[0]["chkDiurna"], 0).ToString()) == 1 ? true : false;
                            txtValDecont.Text = General.Nz(ent.Rows[0]["EstimatedAmount"], "").ToString();
                            Session["AvsXDec_SumaDecont"] = Convert.ToDecimal(General.Nz(ent.Rows[0]["EstimatedAmount"], 0).ToString());
                            txtValAvans.Text = General.Nz(ent.Rows[0]["TotalAmountAvans"], "").ToString();
                            txtValPlataBanca.Text = General.Nz(ent.Rows[0]["UnconfRestAmount"], "").ToString();
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
                                        if (lstConfigCurrencyXPay_Currency.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["CurrencyId"].ToString()) == null ||
                                        lstConfigCurrencyXPay_Currency.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["CurrencyId"].ToString()).Count() == 0)
                                        {
                                            lstConfigCurrencyXPay_Currency.Rows.Add(lstConfigCurrencyXPay.Rows[i]["CurrencyId"].ToString(), lstConfigCurrencyXPay.Rows[i]["CurrencyCode"].ToString(), null, 1);
                                            Session["ConfigCurrencyXPay_Currency"] = lstConfigCurrencyXPay_Currency;
                                        }
                                    }
                                    if (lstConfigCurrencyXPay_Pay != null)
                                    {
                                        if (lstConfigCurrencyXPay_Pay.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString()) == null ||
                                        lstConfigCurrencyXPay_Pay.Select("DictionaryItemId = " + lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString()).Count() == 0)
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
                                    if (lstConfigCurrencyXPay_PayCopy != null && lstConfigCurrencyXPay_PayCopy.Rows.Count > 0)
                                    {
                                        if (lstConfigCurrencyXPay_PayCopy.Select("DictionaryItemId = " + entConfig[i]["PaymentTypeId"].ToString()).Count() == 0)
                                        {
                                            lstConfigCurrencyXPay_PayCopy.Rows.Add(lstConfigCurrencyXPay.Rows[i]["PaymentTypeId"].ToString(), lstConfigCurrencyXPay.Rows[i]["PaymentTypeName"].ToString(), null, 2);
                                            Session["ConfigCurrencyXPay_PayCopy"] = lstConfigCurrencyXPay_PayCopy;
                                        }
                                    }
                                }
                                if (lstConfigCurrencyXPay_PayCopy != null && lstConfigCurrencyXPay_PayCopy.Rows.Count == 1)
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

                    DataTable lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                    if (lstCheltuieli != null && lstCheltuieli.Rows.Count != 0)
                    {
                        lstCheltuieli.CaseSensitive = false;
                        DataRow[] itmDiurna_1 = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                        if (itmDiurna_1 != null && itmDiurna_1.Count() > 0)
                        {
                            DataTable lstDocJustificative = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                            if (lstDocJustificative != null && lstDocJustificative.Rows.Count != 0)
                            {
                                if (lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString()) != null &&
                                    lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString()).Count() != 0)
                                {
                                    DataRow[] entDocJustificativDiurna = lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString());
                                    entDocJustificativDiurna[0]["areFisier"] = 1;
                                    Session["AvsXDec_SursaDateDocJust"] = lstDocJustificative;
                                }
                            }
                        }
                    }

                    string AvsXDec_TipDocument_Diurna = Dami.ValoareParam("AvsXDec_TipDocument_Diurna", "");
                    DataTable lstDoc = GetAvsXDec_DictionaryItemDocumenteDecont();
                    if (lstDoc != null && lstDoc.Rows.Count != 0)
                    {
                        DataRow[] entAvsXDec_TipDocument_Diurna = lstDoc.Select("DictionaryItemName = " + AvsXDec_TipDocument_Diurna);
                        if (entAvsXDec_TipDocument_Diurna != null && entAvsXDec_TipDocument_Diurna.Count() > 0)
                            Session["AvsXDec_TipDocument_DiurnaId"] = entAvsXDec_TipDocument_Diurna[0]["DictionaryItemId"];
                    }


                    lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                    if (lstCheltuieli != null && lstCheltuieli.Rows.Count != 0)
                    {
                        lstCheltuieli.CaseSensitive = false;
                        DataRow[] itmDiurna_1 = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                        if (itmDiurna_1 != null && itmDiurna_1.Count() > 0)
                        {
                            DataTable lstDocJustificative = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                            if (lstDocJustificative != null && lstDocJustificative.Rows.Count != 0)
                            {
                                if (lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString()) != null &&
                                    lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString()).Count() != 0)
                                {
                                    DataRow[] entDocJustificativDiurna = lstDocJustificative.Select("ExpenseTypeId = " + itmDiurna_1[0]["DictionaryItemId"].ToString());
                                    entDocJustificativDiurna[0]["areFisier"] = 1;
                                    Session["AvsXDec_SursaDateDocJust"] = lstDocJustificative;
                                }
                            }
                        }
                    }

                    DataTable dtCheltuieliDecont = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                    for (int i = 0; i < dtCheltuieliDecont.Rows.Count; i++)                                            
                        dtCheltuieliDecont.Rows[i]["toBeSaved"] = 1;                    
                    Session["AvsXDec_SursaDateEstChelt"] = dtCheltuieliDecont;

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private DataTable GetmetaVwAvsXDec_Decont(int documentIdAvans)
        {
            DataTable q = null;
            #region preluare suma utilizata din avans
            DataTable amountConsumedXDoc = null;
            decimal consumedAmountXDoc = 0;
            string sql = "";
            string sqlQuery = string.Empty;
            try
            {
                //sqlQuery = @"select bt.""SrcDocId"", cast(sum(bt.""DestDocAmount"") as decimal(18, 4)) as ""DestDocAmount""
                //            from ""AvsXDec_BusinessTransaction"" bt
                //            join ""AvsXDec_Document"" doc on bt.""DestDocId"" = doc.""DocumentId""
                //                                        and doc.""DocumentStateId"" >=1
                //            where bt.""SrcDocId"" = {0}
                //            group by ""SrcDocId""";
                //sqlQuery = string.Format(sqlQuery, Session["AvsXDec_SrcDocId"].ToString());               
                //amountConsumedXDoc = General.IncarcaDT(sqlQuery, null);
                //if (amountConsumedXDoc != null && amountConsumedXDoc.Rows.Count != 0)
                //    consumedAmountXDoc = Convert.ToDecimal(General.Nz(amountConsumedXDoc.Rows[0]["DestDocAmount"], 0));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            #endregion

            try
            {
                /*0000 - verificam daca exista decontul in BusinessTransaction, ceea ce inseamna ca 
                 * decontul a fost salvat si are avans legat la el*/
                DataTable bt = General.IncarcaDT("SELECT * FROM AvsXDec_BusinessTransaction Where DestDocId = " + Session["AvsXDec_IdDocument"].ToString(), null);
                if (bt != null && bt.Rows.Count > 0)
                {
                    #region legatura existenta intre avans si decont
                    sql = "SELECT  a.DocumentId, COALESCE(b.SrcDocId, -99) as AvansDocumentId, COALESCE(a.DocumentTypeId, -99) as DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName,  COALESCE(a.DocumentStateId, -99) as DocumentStateId, "
                        + " a.DocumentState, COALESCE(a.F10003, -99) as F10003, a.NumeComplet, a.IdDepartament, a.Departament,  a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) as CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, "
                        + " a.ActionTypeId, a.ActionTypeName, a.ActionPlace, a.ActionReason, a.StartDate, a.EndDate, a.StartHour, a.EndHour, a.chkDiurna, a.TotalAmount, a.EstimatedAmount, b.SrcDocAmount as TotalAmountAvans, a.CurrencyId, "
                        + " a.CurrencyCode,  a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, a.PaymentTypeId, a.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie,  a.UnconfRestAmount, a.OriginalDoc, a.TransportTypeId, a.TransportTypeName "
                        + " FROM vwAvsXDec_Decont a "
                        + " JOIN AvsXDec_BusinessTransaction b on a.DocumentId = b.DestDocId "
                        + " where a.DocumentId =" + Session["AvsXDec_IdDocument"].ToString();
                    q = General.IncarcaDT(sql, null);                    
                    #endregion
                }
                /*0001- daca nu exista in business transaction nimic, si parametrul trimis pentru avans document este null
                 * inseamna ca initializez un decont si iau proprietatile setate la nivel de decont*/
                else
                {
                    if ((bt == null || bt.Rows.Count == 0) && documentIdAvans == -99)
                    {
                        #region decount nou fara avans
                        sql = "SELECT  a.DocumentId, null as AvansDocumentId, COALESCE(a.DocumentTypeId, -99) as DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName,  COALESCE(a.DocumentStateId, -99) as DocumentStateId, "
                            + " a.DocumentState, COALESCE(a.F10003, -99) as F10003, a.NumeComplet, a.IdDepartament, a.Departament,  a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) as CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, "
                            + " a.ActionTypeId, a.ActionTypeName, a.ActionPlace, a.ActionReason, a.StartDate, a.EndDate, a.StartHour, a.EndHour, a.chkDiurna, a.TotalAmount, a.EstimatedAmount, 0 as TotalAmountAvans, a.CurrencyId, "
                            + " a.CurrencyCode,  a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, a.PaymentTypeId, a.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie,  a.UnconfRestAmount, a.OriginalDoc, a.TransportTypeId, a.TransportTypeName "
                            + " FROM vwAvsXDec_Decont a "
                            + " where a.DocumentId =" + Session["AvsXDec_IdDocument"].ToString();
                        q = General.IncarcaDT(sql, null);                       
                        #endregion
                    }
                    /*0002 - daca nu exista in business transaction nimic, si parametrul trimis pentru avans document nu este null
                     * inseamna ca initializez un decont si iau proprietatile de la nivel de avans trimis ca parametru*/
                    else
                    {
                        if ((bt == null || bt.Rows.Count == 0) && documentIdAvans != -99)
                        {
                            #region decount nou cu avans ales
                            sql = "SELECT  a.DocumentId, av.DocumentId as AvansDocumentId, COALESCE(a.DocumentTypeId, -99) as DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName,  COALESCE(a.DocumentStateId, -99) as DocumentStateId, "
                                + " a.DocumentState, COALESCE(a.F10003, -99) as F10003, a.NumeComplet, a.IdDepartament, a.Departament,  a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) as CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, "
                                + " av.ActionTypeId, av.ActionTypeName, av.ActionPlace, av.ActionReason, av.StartDate, av.EndDate, av.StartHour, av.EndHour, av.chkDiurna, (av.TotalAmount - (CASE WHEN a.DocumentTypeId = 2003 THEN " + consumedAmountXDoc.ToString().Replace(',', '.') + " ELSE 0 END)) as TotalAmount, " 
                                + " a.EstimatedAmount, (av.TotalAmount - (CASE WHEN a.DocumentTypeId = 2003 THEN " + consumedAmountXDoc.ToString().Replace(',', '.') + " ELSE 0 END)) as TotalAmountAvans, av.CurrencyId, "
                                + " av.CurrencyCode,  a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, av.PaymentTypeId, av.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie,  a.UnconfRestAmount, a.OriginalDoc, av.TransportTypeId, av.TransportTypeName "
                                + " FROM vwAvsXDec_Decont a "
                                + " JOIN vwAvsXDec_Avans av on 1 = 1 "
                                + " where a.DocumentId =" + Session["AvsXDec_IdDocument"].ToString() + " AND av.DocumentId = " + documentIdAvans.ToString();
                            q = General.IncarcaDT(sql, null);
                            #endregion
                        }
                    }
                }

                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
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
                        if (lstConfigCurrencyXPay != null && lstConfigCurrencyXPay.Rows.Count > 0)
                        {
                            cmbModPlata.Value = null;
                            lstConfigCurrencyXPay_PayCopy.Clear();
							DataRow[] entConfig = lstConfigCurrencyXPay.Select("CurrencyId = " + Convert.ToInt32(cmbMonedaAvans.Value ?? -99));
                            for (int i = 0; i < entConfig.Length; i++)
                            {
                                if (lstConfigCurrencyXPay_PayCopy != null && lstConfigCurrencyXPay_PayCopy.Rows.Count > 0)
                                {
                                    if (lstConfigCurrencyXPay_PayCopy.Select("DictionaryItemId = " + entConfig[i]["PaymentTypeId"].ToString()).Count() == 0)
                                    {
                                        lstConfigCurrencyXPay_PayCopy.Rows.Add(entConfig[i]["PaymentTypeId"].ToString(), entConfig[i]["PaymentTypeName"].ToString(), null, 2);
                                        Session["ConfigCurrencyXPay_PayCopy"] = lstConfigCurrencyXPay_PayCopy;
                                    }
                                }
                            }
                            if (lstConfigCurrencyXPay_PayCopy != null && lstConfigCurrencyXPay_PayCopy.Rows.Count == 1)
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


        protected void grDateDocJust_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_SursaDateDocJust"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                decimal suma = Convert.ToDecimal(General.Nz(Session["AvsXDec_SumaDecont"], 0));
                suma -= Convert.ToDecimal(General.Nz(row["TotalPayment"], 0));
                Session["AvsXDec_SumaDecont"] = suma;


                #region actualizare valoare avans
                /*LeonardM 25.08.2016
                 * actualizarea sumelor o realizam mai jos cu diurna,
                 * deoarece daca dezactivam bifa de diurna se scade de doua ori valoarea diurnei
                 * si nu e ok*/
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                #endregion

                #region stergere diurna
                /*verificam daca exista cheltuiala de diurna si o stergem*/
                DataTable dtNomenCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                dtNomenCheltuieli.CaseSensitive = false;
                DataRow[] itmDiurna = dtNomenCheltuieli.Select("DictionaryItemName= 'diurna'");
                if (itmDiurna != null && itmDiurna.Count() > 0)
                {
                    if (Convert.ToInt32(row["DictionaryItemId"].ToString()) == Convert.ToInt32(itmDiurna[0]["DictionaryItemId"]))
                    {
                        chkIsDiurna.Checked = false;
                        Session["maxValueDiurna"] = 0;
                    }
                    else
                    {
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToInt32(General.Nz(row["TotalPayment"], 0));
                        row.Delete();
                    }
                }
                else
                {
                    ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToInt32(General.Nz(row["TotalPayment"], 0));
                    row.Delete();
                }
                #endregion
   
                Session["AvsXDec_SursaDateDec"] = ent;

                e.Cancel = true;
                grDateDocJust.CancelEdit();
                Session["AvsXDec_SursaDateDocJust"] = dt;
                grDateDocJust.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	   
        protected void grDateDocJust_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["AvsXDec_SursaDateDocJust"] as DataTable;

                /*LeonardM 15.08.2016
                 * se doreste ca in momentul in care se intializeaza un nou rand sa se ia in considerare 
                 * moneda aleasa de pe avans (fiind considerata moneda implicita
                ent.CurrencyId = IdMonedaDefault;
                 * */
                //ent.CurrencyId = Convert.ToInt32(cmbMonedaAvans.EditValue ?? -99);
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                DataTable lstCheltuieliAvans = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                int detailId = -1;
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
                            case "TOTALPAYMENT":
                                decimal valoareBon = 0;
                                /*
                                    * valoareBon = Convert.ToDecimal(String.IsNullOrEmpty(txtValDecont.Text) ? 0.ToString() : txtValDecont.Text.Replace('.', ','));
                                    * */
                                valoareBon = Convert.ToDecimal(General.Nz(ent.Rows[0]["EstimatedAmount"], 0));
                                if (e.NewValues[col.ColumnName] != null)
                                {                      
                                    /*rand nou*/
                                    valoareBon = valoareBon + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());                                    
                                }
                                /*
                                txtValDecont.Text = Convert.ToDecimal(valoareBon).ToString();
                                */
                                ent.Rows[0]["EstimatedAmount"] = valoareBon;

                                if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                                {
                                    /*avertizare in cazul in care a depasit valorea estimata pe avans*/
                                    decimal sumaAvansCheltuiala = 0;
                                    if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString())) != null &&
                                        lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString())).Count() != 0)
                                        sumaAvansCheltuiala = Convert.ToDecimal(General.Nz(lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString())).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                                    decimal sumaDecontCheltuiala = 0;
                                    if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString())) != null &&
                                        dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString())).Count() != 0)
                                        if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(e.NewValues["DocumentDetailId"].ToString())) != null &&
                                            dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(e.NewValues["DocumentDetailId"].ToString())).Count() != 0)
                                            sumaDecontCheltuiala = Convert.ToDecimal(General.Nz(dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(e.NewValues["DocumentDetailId"].ToString())).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                                    sumaDecontCheltuiala += Convert.ToDecimal(General.Nz(e.NewValues[col.ColumnName], 0));
                                    if (sumaDecontCheltuiala > sumaAvansCheltuiala && Convert.ToInt32(General.Nz(e.NewValues["ExpenseTypeId"], -99).ToString()) != -99)
                                    {
                                        Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Suma pentru acest tip de cheltuiala depaseste suma inregistrata pe avans!");
                                        return;
                                    }
                                }
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "EXPENSETYPEID":
                                /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                                /*daca s-a selectat un avans
                                    * verificam ca cheltuielile de pe avans sa fie identice cu cheltuielile puse pe decont*/
                                if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                                {
                                    if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) == null ||
                                        (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                        lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() == 0))
                                    {
                                        row[x] = null;
                                        Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Tipul de cheltuiala ales nu exista pe avansul ales!");
                                        return;
                                    }

                                    /*avertizare in cazul in care a depasit valorea estimata pe avans*/
                                    decimal sumaAvansCheltuiala = 0;
                                    if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                        lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() != 0)
                                        sumaAvansCheltuiala = Convert.ToDecimal(General.Nz(lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                                    decimal sumaDecontCheltuiala = 0;
                                    if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                        dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() != 0)
                                        sumaDecontCheltuiala = Convert.ToDecimal(General.Nz(dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                                    if (sumaDecontCheltuiala > sumaAvansCheltuiala)
                                    {
                                        Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Suma pentru acest tip de cheltuiala depaseste suma inregistrata pe avans!");
                                        return;
                                    }
                                }

                                #region verificare cheltuieli
                                /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                                DataTable lstCheltuieli = GetDecontExpenseType(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                                lstCheltuieli.CaseSensitive = false;
                                DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                                /*LeonardM 10.08.2016
                                    * cerinta de la Groupama ca pentru avansul spre decontare sa se poata adauga doar un tip de cheltuiala
                                    * in afara de diurna, deoarece pentru acest tip de document exista circuit definit pentru un singur tip de 
                                    * cheltuiala*/
                                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                                {
                                    case 2002:/*Decont simplu*/
                                        if (dt.Select("ExpenseTypeId <> " + Convert.ToInt32(e.NewValues[col.ColumnName]) + " AND DocumentDetailId <>  " + e.NewValues["DocumentDetailId"].ToString()) != null &&
                                            dt.Select("ExpenseTypeId <> " + Convert.ToInt32(e.NewValues[col.ColumnName]) + " AND DocumentDetailId <>  " + e.NewValues["DocumentDetailId"].ToString()).Count() != 0)
                                        {
                                            Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Trebuie selectat doar un tip de cheltuiala pentru decont!");
                                            row[x] = null;
                                            return;
                                        }
                                        break;
                                }
                                #endregion
                                row[x] = e.NewValues[col.ColumnName]; 
                                break;
                            case "DOCDATEDECONT":
                                /*LeonardM 25.08.2016
                                * solicitare de la Groupoama ca pentru avansul spre deplasare sa nu se completeze o data care nu
                                * exista in intervalul de deplasare completat mai sus*/
                                switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                                {
                                    case 2001: /*Decont avans spre deplasare*/
                                        #region verificari avans spre deplasare
                                        if (txtStartDate.Value == null || txtEndDate.Value == null)
                                        {
                                            Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat datele pentru deplasare!");
                                            row[x] = null;
                                            return;
                                        }

                                        DateTime dtDocument = Convert.ToDateTime(e.NewValues[col.ColumnName]);
                                        DateTime dtStartDeplasare = Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString());
                                        DateTime dtEndDeplasare = Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString());
                                        if (!(GetDayDateTime(dtStartDeplasare) <= GetDayDateTime(dtDocument) && GetDayDateTime(dtEndDeplasare) >= GetDayDateTime(dtDocument)))
                                        {
                                            Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Data documentului nu este cuprinsa in intervalul de deplasare completat!");
                                            row[x] = null;
                                            return;
                                        }
                                        #endregion
                                        break;
                                }
                                row[x] = e.NewValues[col.ColumnName]; 
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DOCUMENTID":
                                row[x] = Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()); 
                                break;
                            case "DOCUMENTDETAILID":
                                detailId = Dami.NextId("AvsXDec_DecontDetail", 1);
                                row[x] = detailId;
                                e.NewValues["DocumentDetailId"] = detailId;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);

                metaCereriDate itm = Session["DocUpload_AvsXDec_DJ"] as metaCereriDate;
                if (itm != null)
                {
                    string sql = @"SELECT * FROM ""tblFisiere"" WHERE ""Id"" IN (SELECT ""IdDocument"" FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} and ""DocumentDetailId"" = {1} ) AND ""Tabela"" = 'AvsXDec_relUploadDocumente'";
                    sql = string.Format(sql, Session["AvsXDec_IdDocument"].ToString(), detailId);
                    DataTable dtDoc = General.IncarcaDT(sql, null);
                    if (dtDoc != null && dtDoc.Rows.Count > 0)
                        SalveazaFisier(Convert.ToInt32(dtDoc.Rows[0]["Id"].ToString()), itm);
                    else
                        SalveazaFisier(Dami.NextId("AvsXDec_relUploadDocumente", 1), itm);
                }
                Session["DocUpload_AvsXDec_DJ"] = null;

                e.Cancel = true;
                grDateDocJust.CancelEdit();
                grDateDocJust.DataSource = dt;
                grDateDocJust.KeyFieldName = "DocumentDetailId;DocumentId";
                Session["AvsXDec_SursaDateDocJust"] = dt;

                decimal suma = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                    suma += Convert.ToDecimal(General.Nz(dt.Rows[i]["TotalPayment"], 0));
                Session["AvsXDec_SumaDecont"] = suma;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateDocJust_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {                
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                int detailId = -1;
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                DataTable dt = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateDocJust.Columns[col.ColumnName] != null && grDateDocJust.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    if (col.ColumnName.ToUpper() == "DOCUMENTDETAILID")
                        detailId = Convert.ToInt32(row[col.ColumnName]);

                    DataTable lstCheltuieliAvans = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                    if (col.ColumnName == "TotalPayment")
                    {
                        decimal valoareBon = 0;
                        /*
                            * valoareBon = Convert.ToDecimal(String.IsNullOrEmpty(txtValDecont.Text) ? 0.ToString() : txtValDecont.Text.Replace('.', ','));
                            * */
                        valoareBon = Convert.ToDecimal(General.Nz(ent.Rows[0]["EstimatedAmount"], 0));
                        if (e.NewValues[col.ColumnName] != null)
                        {
                            if (Convert.ToDecimal(General.Nz(e.OldValues[col.ColumnName], 0).ToString()) == 0)
                            {
                                /*rand nou*/
                                valoareBon = valoareBon + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());
                            }
                            else
                            {
                                /*rand vechi pentru care se actualizeaza suma*/
                                valoareBon = valoareBon - Convert.ToDecimal(e.OldValues[col.ColumnName].ToString()) + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());
                            }
                        }
                        /*
                        txtValDecont.Text = Convert.ToDecimal(valoareBon).ToString();
                        */
                        ent.Rows[0]["EstimatedAmount"] = valoareBon;

                        if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                        {
                            /*avertizare in cazul in care a depasit valorea estimata pe avans*/
                            decimal sumaAvansCheltuiala = 0;
                            if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString())) != null &&
                                lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString())).Count() != 0)
                                sumaAvansCheltuiala = Convert.ToDecimal(General.Nz(lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString())).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                            decimal sumaDecontCheltuiala = 0;
                            if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString())) != null &&
                                dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString())).Count() != 0)
                                if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(row["DocumentDetailId"].ToString())) != null &&
                                    dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(row["DocumentDetailId"].ToString())).Count() != 0)
                                    sumaDecontCheltuiala = Convert.ToDecimal(General.Nz(dt.Select("ExpenseTypeId = " + Convert.ToInt32(General.Nz(row["ExpenseTypeId"], 0).ToString()) + " AND DocumentDetailId <> " + Convert.ToInt32(row["DocumentDetailId"].ToString())).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                            sumaDecontCheltuiala += Convert.ToDecimal(General.Nz(e.NewValues[col.ColumnName], 0));
                            if (sumaDecontCheltuiala > sumaAvansCheltuiala && Convert.ToInt32(General.Nz(row["ExpenseTypeId"], -99).ToString()) != -99)
                            {
                                Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Suma pentru acest tip de cheltuiala depaseste suma inregistrata pe avans!");                               
                                return;
                            }
                        }
                    }
                    if (col.ColumnName == "ExpenseTypeId")
                    {
                        /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                        /*daca s-a selectat un avans
                            * verificam ca cheltuielile de pe avans sa fie identice cu cheltuielile puse pe decont*/
                        if (Convert.ToInt32(cmbDocAvans.Value ?? -99) != -99)
                        {
                            if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) == null ||
                                (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() == 0))
                            {
                                row[col.ColumnName] = null;
                                MessageBox.Show(Dami.TraduCuvant("Tipul de cheltuiala ales nu exista pe avansul ales!"), MessageBox.icoError, "Atentie !");
                                return;
                            }

                            /*avertizare in cazul in care a depasit valorea estimata pe avans*/
                            decimal sumaAvansCheltuiala = 0;
                            if (lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() != 0)
                                sumaAvansCheltuiala = Convert.ToDecimal(General.Nz(lstCheltuieliAvans.Select("DictionaryItemId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                            decimal sumaDecontCheltuiala = 0;
                            if (dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])) != null &&
                                dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).Count() != 0)
                                sumaDecontCheltuiala = Convert.ToDecimal(General.Nz(dt.Select("ExpenseTypeId = " + Convert.ToInt32(e.NewValues[col.ColumnName])).CopyToDataTable().Compute("SUM(TotalPayment)", "[TotalPayment] IS NOT NULL"), 0));

                            if (sumaDecontCheltuiala > sumaAvansCheltuiala)
                            {
                                Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Suma pentru acest tip de cheltuiala depaseste suma inregistrata pe avans!");
                                return;
                            }
                        }

                        #region verificare cheltuieli
                        /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                        DataTable lstCheltuieli = GetDecontExpenseType(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                        lstCheltuieli.CaseSensitive = false;
                        DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                        /*LeonardM 10.08.2016
                            * cerinta de la Groupama ca pentru avansul spre decontare sa se poata adauga doar un tip de cheltuiala
                            * in afara de diurna, deoarece pentru acest tip de document exista circuit definit pentru un singur tip de 
                            * cheltuiala*/
                        switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                        {
                            case 2002:/*Decont simplu*/
                                if (dt.Select("ExpenseTypeId <> " + Convert.ToInt32(e.NewValues[col.ColumnName]) + " AND DocumentDetailId <>  " + row["DocumentDetailId"].ToString()) != null &&
                                    dt.Select("ExpenseTypeId <> " + Convert.ToInt32(e.NewValues[col.ColumnName]) + " AND DocumentDetailId <>  " + row["DocumentDetailId"].ToString()).Count() != 0)
                                {
                                    Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Trebuie selectat doar un tip de cheltuiala pentru decont!");
                                    row["DictionaryItemId"] = null;
                                    return;
                                }
                                break;
                        }
                        #endregion
                    }

                    if (col.ColumnName == "DocDateDecont")
                    {
                        /*LeonardM 25.08.2016
                            * solicitare de la Groupoama ca pentru avansul spre deplasare sa nu se completeze o data care nu
                            * exista in intervalul de deplasare completat mai sus*/
                        switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
                        {
                            case 2001: /*Decont avans spre deplasare*/
                                #region verificari avans spre deplasare
                                if (txtStartDate.Value == null || txtEndDate.Value == null)
                                {
                                    Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat datele pentru deplasare!");
                                    row["DocDateDecont"] = null;
                                    return;
                                }

                                DateTime dtDocument = Convert.ToDateTime(e.NewValues[col.ColumnName]);
                                DateTime dtStartDeplasare = Convert.ToDateTime(ent.Rows[0]["StartDate"].ToString());
                                DateTime dtEndDeplasare = Convert.ToDateTime(ent.Rows[0]["EndDate"].ToString());
                                if (!(GetDayDateTime(dtStartDeplasare) <= GetDayDateTime(dtDocument) && GetDayDateTime(dtEndDeplasare) >= GetDayDateTime(dtDocument)))
                                {
                                    Session["AvsXDec_cpAlertMessage"] = Dami.TraduCuvant("Data documentului nu este cuprinsa in intervalul de deplasare completat!");
                                    row["DocDateDecont"] = null;
                                    return;
                                }
                                #endregion
                                break;
                        }
                    }    
                }
                Session["AvsXDec_SursaDateDec"] = ent;

                metaCereriDate itm = Session["DocUpload_AvsXDec_DJ"] as metaCereriDate;
                if (itm != null)
                {
                    string sql = @"SELECT * FROM ""tblFisiere"" WHERE ""Id"" IN (SELECT ""IdDocument"" FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} and ""DocumentDetailId"" = {1} ) AND ""Tabela"" = 'AvsXDec_relUploadDocumente'";
                    sql = string.Format(sql, Session["AvsXDec_IdDocument"].ToString(), detailId);
                    DataTable dtDoc = General.IncarcaDT(sql, null);
                    if (dtDoc != null && dtDoc.Rows.Count > 0)
                        SalveazaFisier(Convert.ToInt32(dtDoc.Rows[0]["Id"].ToString()), itm);
                    else
                        SalveazaFisier(Dami.NextId("AvsXDec_relUploadDocumente", 1), itm);
                }
                Session["DocUpload_AvsXDec_DJ"] = null;

                e.Cancel = true;
                grDateDocJust.CancelEdit();
                Session["AvsXDec_SursaDateDocJust"] = dt;
                grDateDocJust.DataSource = dt;

                decimal suma = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                    suma += Convert.ToDecimal(General.Nz(dt.Rows[i]["TotalPayment"], 0));
                Session["AvsXDec_SumaDecont"] = suma;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDocJust_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value ?? -99);
                e.NewValues["TotalPayment"] = 0;
                e.NewValues["areFisier"] = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateDocJust_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                if (e.VisibleIndex >= 0)
                {
                    if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                        if (e.ButtonType == ColumnCommandButtonType.Delete)
                            e.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void grDateDocJust_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {
                ASPxComboBox cmbDoc = grDateDocJust.FindEditFormTemplateControl("cmbDoc") as ASPxComboBox;
                if (cmbDoc != null)
                {
                    DataTable dtDoc = GetAvsXDec_DictionaryItemDocumenteDecont();
                    cmbDoc.DataSource = dtDoc;
                    cmbDoc.DataBindItems();
                }

                ASPxComboBox cmbMoneda = grDateDocJust.FindEditFormTemplateControl("cmbMoneda") as ASPxComboBox;
                if (cmbMoneda != null)
                {
                    DataTable dtMon = GetAvsXDec_DictionaryItemValute(); 
                    cmbMoneda.DataSource = dtMon;
                    cmbMoneda.DataBindItems();
                }

                ASPxComboBox cmbChelt = grDateDocJust.FindEditFormTemplateControl("cmbChelt") as ASPxComboBox;
                if (cmbChelt != null)
                {
                    DataTable dtChelt = GetDecontExpenseType(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                    cmbChelt.DataSource = dtChelt;
                    cmbChelt.DataBindItems();       
                }

                cmbMoneda.ClientEnabled = false;
                /*end LeonardM 15.08.2016*/
                ASPxTextBox txtLinBug = grDateDocJust.FindEditFormTemplateControl("txtLinBug") as ASPxTextBox;
                ASPxTextBox txtFurn = grDateDocJust.FindEditFormTemplateControl("txtFurn") as ASPxTextBox;
                ASPxTextBox txtNr = grDateDocJust.FindEditFormTemplateControl("txtNr") as ASPxTextBox;
                ASPxDateEdit deData = grDateDocJust.FindEditFormTemplateControl("deData") as ASPxDateEdit;
                ASPxTextBox txtVal = grDateDocJust.FindEditFormTemplateControl("txtVal") as ASPxTextBox;
                ASPxTextBox txtDet = grDateDocJust.FindEditFormTemplateControl("txtDet") as ASPxTextBox;

                /*LeonardM 12.08.2016
                 * utilizatorul care creeaza documentul nu are drept de editare pe coloana linie buget,
                 * ci doar cei de pe circuit (solicitare GroupamA)*/
                if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                {
                    #region documente justificative
                    if (/*IsBudgetOwnerEdited &&*/ Convert.ToInt32(Session["AvsXDec_IdStare"].ToString()) != 0)    //Radu 31.08.2016                        
                        txtLinBug.ClientEnabled = true;
                    else
                        txtLinBug.ClientEnabled = false;

                    /*colSterge*/
                    //grDateDocJustifDecont.Columns[1].AllowEditing = DevExpress.Utils.DefaultBoolean.False;
                    /*colSel*/
                    //grDateDocJustifDecont.Columns[0].AllowEditing = DevExpress.Utils.DefaultBoolean.False;

                    txtFurn.ClientEnabled = false;
                    cmbDoc.ClientEnabled = false;
                    txtNr.ClientEnabled = false;
                    deData.ClientEnabled = false;
                    txtVal.ClientEnabled = false;
                    cmbChelt.ClientEnabled = false;
                    txtDet.ClientEnabled = false;
                    #endregion
   
                }
                else
                {
                    #region Documente justificative
                    /*LeonardM 15.08.2016
                     * coloana de currency de pe docuemnte justificative nu se doreste a fi editabila, 
                     * aceasta intializandu-se cu moneda default aleasa inainte pe avans*/
                    cmbMoneda.ClientEnabled = true;
                    /*end LeonardM 15.08.2016*/
                    txtLinBug.ClientEnabled = false;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



        protected void grDateEstChelt_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_SursaDateEstChelt"] as DataTable;

                DataRow row = dt.Rows.Find(keys);
                row.Delete();

                e.Cancel = true;
                grDateEstChelt.CancelEdit();
                Session["AvsXDec_SursaDateEstChelt"] = dt;
                grDateEstChelt.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEstChelt_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["AvsXDec_SursaDateEstChelt"] as DataTable;      

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
                                row[x] = Dami.NextId("AvsXDec_DecontDetail", 1);
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
                grDateEstChelt.CancelEdit();
                grDateEstChelt.DataSource = dt;
                grDateEstChelt.KeyFieldName = "DocumentDetailId;DocumentId";
                Session["AvsXDec_SursaDateEstChelt"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void grDateEstChelt_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_SursaDateEstChelt"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDateEstChelt.Columns[col.ColumnName] != null && grDateEstChelt.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }
                e.Cancel = true;
                grDateEstChelt.CancelEdit();
                Session["AvsXDec_SursaDateEstChelt"] = dt;
                grDateEstChelt.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateEstChelt_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value ?? -99);
                /*comentam aceasta parte, deoarece in momentul in care adaugam diurna
                 * dorim valoarea sa fie cea asignata de mine prin calcul
                 * e.NewValues["TotalPayment"] = 0;
                 * */

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDatePlataBanca_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;

                DataRow row = dt.Rows.Find(keys);
       
                row.Delete();

                e.Cancel = true;
                grDatePlataBanca.CancelEdit();
                Session["AvsXDec_SursaDatePlataBanca"] = dt;
                grDatePlataBanca.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePlataBanca_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;

                /*comentam aceasta parte, deoarece in momentul in care adaugam diurna
                 * dorim valoarea sa fie cea asignata de mine prin calcul
                 * ent.Amount = 0;
                 * */

                object[] row = new object[dt.Columns.Count];
                int detailId = -1;
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
                                detailId = Dami.NextId("AvsXDec_DecontDetail", 1);
                                row[x] = detailId;
                                break;
                            case "TOTALPAYMENT":
                                decimal valoareBon = 0;
                                valoareBon = Convert.ToDecimal(txtValDecont.Text == "" ? "0" : txtValDecont.Text);
                                if (e.NewValues[col.ColumnName] != null)
                                {                 
                                    /*rand nou*/
                                    valoareBon = valoareBon + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());                                    
                                }
                                txtValDecont.Text = Convert.ToDouble(valoareBon).ToString();
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);

                metaCereriDate itm = Session["DocUpload_AvsXDec_PB"] as metaCereriDate;
                if (itm != null)
                {
                    string sql = @"SELECT * FROM ""tblFisiere"" WHERE ""Id"" IN (SELECT ""IdDocument"" FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} and ""DocumentDetailId"" = {1} ) AND ""Tabela"" = 'AvsXDec_relUploadDocumente'";
                    sql = string.Format(sql, Session["AvsXDec_IdDocument"].ToString(), detailId);
                    DataTable dtDoc = General.IncarcaDT(sql, null);
                    if (dtDoc != null && dtDoc.Rows.Count > 0)
                        SalveazaFisier(Convert.ToInt32(dtDoc.Rows[0]["Id"].ToString()), itm);
                    else
                        SalveazaFisier(Dami.NextId("AvsXDec_relUploadDocumente", 1), itm);
                }
                Session["DocUpload_AvsXDec_PB"] = null;

                e.Cancel = true;
                grDatePlataBanca.CancelEdit();
                grDatePlataBanca.DataSource = dt;
                grDatePlataBanca.KeyFieldName = "DocumentDetailId;DocumentId";
                Session["AvsXDec_SursaDatePlataBanca"] = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePlataBanca_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                int detailId = -1;

                DataTable dt = Session["AvsXDec_SursaDatePlataBanca"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDatePlataBanca.Columns[col.ColumnName] != null && grDatePlataBanca.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    if (col.ColumnName.ToUpper() == "DOCUMENTDETAILID")
                        detailId = Convert.ToInt32(row[col.ColumnName]);

                    if (col.ColumnName == "TotalPayment")
                    {
                        decimal valoareBon = 0;
                        valoareBon = Convert.ToDecimal(txtValDecont.Text == "" ? "0" : txtValDecont.Text);
                        if (e.NewValues[col.ColumnName] != null)
                        {
                            if (Convert.ToDecimal(General.Nz(e.OldValues[col.ColumnName], 0).ToString()) == 0)
                            {
                                /*rand nou*/
                                valoareBon = valoareBon + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());
                            }
                            else
                            {
                                /*rand vechi pentru care se actualizeaza suma*/
                                valoareBon = valoareBon - Convert.ToDecimal(e.OldValues[col.ColumnName].ToString()) + Convert.ToDecimal(e.NewValues[col.ColumnName].ToString());
                            }
                        }
                        txtValDecont.Text = Convert.ToDouble(valoareBon).ToString();
                    }
                }

                metaCereriDate itm = Session["DocUpload_AvsXDec_PB"] as metaCereriDate;
                if (itm != null)
                {
                    string sql = @"SELECT * FROM ""tblFisiere"" WHERE ""Id"" IN (SELECT ""IdDocument"" FROM ""AvsXDec_relUploadDocumente"" WHERE ""DocumentId"" = {0} and ""DocumentDetailId"" = {1} ) AND ""Tabela"" = 'AvsXDec_relUploadDocumente'";
                    sql = string.Format(sql, Session["AvsXDec_IdDocument"].ToString(), detailId);
                    DataTable dtDoc = General.IncarcaDT(sql, null);
                    if (dtDoc != null && dtDoc.Rows.Count > 0)
                        SalveazaFisier(Convert.ToInt32(dtDoc.Rows[0]["Id"].ToString()), itm);
                    else
                        SalveazaFisier(Dami.NextId("AvsXDec_relUploadDocumente", 1), itm);
                }
                Session["DocUpload_AvsXDec_PB"] = null;

                e.Cancel = true;
                grDatePlataBanca.CancelEdit();
                Session["AvsXDec_SursaDatePlataBanca"] = dt;
                grDatePlataBanca.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePlataBanca_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["CurrencyId"] = Convert.ToInt32(cmbMonedaAvans.Value ?? -99);
                e.NewValues["TotalPayment"] = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePlataBanca_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            try
            {
                if (e.VisibleIndex >= 0)
                {
                    if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                        if (e.ButtonType == ColumnCommandButtonType.Delete)
                            e.Visible = false;   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePlataBanca_HtmlEditFormCreated(object sender, ASPxGridViewEditFormEventArgs e)
        {
            try
            {   
                ASPxComboBox cmbDoc = grDatePlataBanca.FindEditFormTemplateControl("cmbDoc") as ASPxComboBox;
                if (cmbDoc != null)
                {
                    DataTable dtDoc = GetAvsXDec_DictionaryItemDocumentePlataDecont();
                    cmbDoc.DataSource = dtDoc;
                    cmbDoc.DataBindItems();
                }

                ASPxComboBox cmbMoneda = grDatePlataBanca.FindEditFormTemplateControl("cmbMoneda") as ASPxComboBox;
                if (cmbMoneda != null)
                {                       
                    DataTable dtMon = GetAvsXDec_DictionaryItemValute();
                    cmbMoneda.DataSource = dtMon;
                    cmbMoneda.DataBindItems();
                }

                cmbMoneda.ClientEnabled = false;
                /*end LeonardM 15.08.2016*/
                ASPxTextBox txtNr = grDateDocJust.FindEditFormTemplateControl("txtNr") as ASPxTextBox;
                ASPxDateEdit deData = grDateDocJust.FindEditFormTemplateControl("deData") as ASPxDateEdit;
                ASPxTextBox txtVal = grDateDocJust.FindEditFormTemplateControl("txtVal") as ASPxTextBox;

                /*LeonardM 12.08.2016
                 * utilizatorul care creeaza documentul nu are drept de editare pe coloana linie buget,
                 * ci doar cei de pe circuit (solicitare GroupamA)*/
                if (Convert.ToInt32(Session["AvsXDec_PoateModif"].ToString()) == 0 && Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
                { 
                    #region documente plata banca
                    grDatePlataBanca.Enabled = true;    

                    cmbDoc.ClientEnabled = false;
                    txtNr.ClientEnabled = false;
                    deData.ClientEnabled = false;
                    txtVal.ClientEnabled = false;
                    #endregion
                }       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }



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
                string idStare = "1";
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
                    idStare = "NULL";

                    if (i == 3 && Convert.ToDecimal(txtValAvans.Text.Length <= 0 ? "0" : txtValAvans.Text) <= 1000)
                    {
                        total--;
                        continue;
                    }

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
								sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSuper + " AND F10003 = " + entDocument.Rows[0]["F10003"].ToString();
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
                                    if (poz == 1) idStare = "1";
                                    if (poz == total) idStare = "3";

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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            if (ent != null && ent.Rows.Count > 0)
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
                DataTable lstCheltuieliInserate = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                DataTable lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                lstCheltuieli.CaseSensitive = false;
                DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                DataRow[] chDiurna = lstCheltuieliInserate.Select("ExpenseTypeId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                if (chDiurna != null && chDiurna.Count() > 0)
                {
                    decimal valDiurna = 0;
                    CalculDiurna(out valDiurna);
                    DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                    /*LeonardM 31.10.2016
                           * in mommentul in care diurna este modificat in jos,
                           * se recalculeaza la incarcare si atunci se modifica valoare*/
                    if (Convert.ToDecimal(General.Nz(chDiurna[0]["TotalPayment"], 0)) > valDiurna)
                    {
                        ent.Rows[0]["EstimatedAmount"] = Convert.ToDecimal(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) - Convert.ToDecimal(General.Nz(chDiurna[0]["TotalPayment"], 0)) + valDiurna;
                        Session["maxValueDiurna"] = valDiurna;

                        chDiurna[0]["TotalPayment"] = valDiurna;
                        Session["AvsXDec_SursaDateDocJust"] = lstCheltuieliInserate;
                    }
                    else
                        Session["maxValueDiurna"] = valDiurna;
                    /*end LeonardM 31.10.2016*/
                }
            }
        }

        private void CalculDiurna(out decimal valDiurna)
        {
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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

            entSettingsDiurna = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1 + " AND KeyField2 = " + KeyField2 + " AND KeyField3 = " + KeyField3 + " AND F71802 = " + General.Nz(Session["IdFunctieAngajat"], "-99").ToString() + " AND CurrencyId =  " + ent.Rows[0]["CurrencyId"].ToString());
            /*nu am gasit o setare conform celor de mai sus si functiei, incercam
             * sa gasim o configurare conform celor de mai sus si idfunctie = -99*/
            if (entSettingsDiurna == null || entSettingsDiurna.Count() == 0)
                //entSettingsDiurna = lstVwAvsXDec_Settings.Where(p => p.KeyField1 == KeyField1 && p.KeyField2 == KeyField2 && p.KeyField3 == KeyField3 && (p.F71802 ?? -99) == -99).FirstOrDefault();
                entSettingsDiurna = lstVwAvsXDec_Settings.Select("KeyField1 = " + KeyField1 + " AND KeyField2 = " + KeyField2 + " AND KeyField3 = " + KeyField3 + " AND F71802 = 0  AND CurrencyId = " + ent.Rows[0]["CurrencyId"].ToString());
            /*daca tot nu gasim o configurare, atunci in mod default diurna = 0*/
            if (entSettingsDiurna == null || entSettingsDiurna.Count() == 0)
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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
                        lstCheltuieli.CaseSensitive = false;
                        DataRow[] itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                        if (itmDiurna != null && itmDiurna.Count() > 0)
                            value = Convert.ToInt32(itmDiurna[0]["DictionaryItemId"].ToString());
                        break;
                    default:
                        value = -99;
                        break;
                }
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 1001)
                return;
            if (ent != null && ent.Rows.Count > 0)
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
            if (Convert.ToBoolean(chkIsDiurna.Value.ToString()) == true)
            {
                /*verificam sa fie completata modalitatea de deplasare, moneda timpul de start si final, numarul de ore, 
                 * modalitatea plata si moneda*/
                #region verifDateComplete
                string campuriNecompletate = string.Empty;
                if (ent.Rows[0]["StartDate"] == DBNull.Value)
                    campuriNecompletate += "data start;";
                if (string.IsNullOrEmpty(txtOraPlecare.Text) && ent.Rows[0]["StartHour"] == DBNull.Value)
                    campuriNecompletate += ", ora inceput";
                else
                {
                    if (ent.Rows[0]["StartHour"] == DBNull.Value)
                    {
                        ent.Rows[0]["StartHour"] = DateTime.ParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (ent.Rows[0]["StartHour"] == DBNull.Value)
                            campuriNecompletate += ", ora inceput";
                    }
                }
                if (string.IsNullOrEmpty(txtOraSosire.Text) && ent.Rows[0]["EndHour"] == DBNull.Value)
                    campuriNecompletate += ", ora sfarsit";
                else
                {
                    if (ent.Rows[0]["EndHour"] == DBNull.Value)
                    {
                        ent.Rows[0]["EndHour"] = DateTime.ParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        if (ent.Rows[0]["EndHour"] == DBNull.Value)
                            campuriNecompletate += ", ora sfarsit";
                    }
                }
                if (ent.Rows[0]["EndHour"] == DBNull.Value)
                    campuriNecompletate += "data final;";
                if (ent.Rows[0]["PaymentTypeId"] == DBNull.Value)
                    campuriNecompletate += "modalitate plata;";
                if (ent.Rows[0]["CurrencyId"] == DBNull.Value)
                    campuriNecompletate += "moneda plata";
                if (ent.Rows[0]["ActionTypeId"] == DBNull.Value)
                    campuriNecompletate += "tip deplasare;";
                if (!string.IsNullOrEmpty(campuriNecompletate))
                {
                    ent.Rows[0]["chkDiurna"] = 0;
                    //ent.chkDiurnaInt = 0;
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat " + campuriNecompletate.Substring(0, campuriNecompletate.Length - 1) + " pentru a efectua calcul de diurna ok!");
                    return;
                }
                else
                {
                    decimal valDiurna;
                    CalculDiurna(out valDiurna);

                    /*verificam mai intai in lista daca exista deja diurna adaugata, pentru a nu o adauga aiurea
                     * acest lucru ma ajuta si pentru cazurile in care am diurna completata pe avans si aleg un 
                     * avans de acest gen. ca practic nu pun diurna de 2 ori pe avans*/
                    DataTable lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                    lstCheltuieli.CaseSensitive = false;
                    DataRow[] itmDiurna_1 = lstCheltuieli.Select("DictionaryItemName = 'diurna'");
                    if (itmDiurna_1 == null || itmDiurna_1.Count() == 0)
                        return;


                    DataTable entCheltuialaDiurnaTot = Session["AvsXDec_SursaDateDocJust"] as DataTable;
                    DataRow[] entCheltuialaDiurna = null;
                    if (entCheltuialaDiurnaTot != null && entCheltuialaDiurnaTot.Rows.Count > 0)
                        entCheltuialaDiurna = entCheltuialaDiurnaTot.Select("DictionaryItemId = " + itmDiurna_1[0]["DictionaryItemId"].ToString());
                    if (entCheltuialaDiurna != null || lstCheltuieli == null || lstCheltuieli.Rows.Count == 0)
                        return;

                    string AvsXDec_BudgetLine_Diurna2001 = Dami.ValoareParam("AvsXDec_BudgetLine_Diurna2001", "");

                    object[] row = new object[entCheltuialaDiurnaTot.Columns.Count];
                    int x = 0;
                    foreach (DataColumn col in entCheltuialaDiurnaTot.Columns)
                    {
                        if (!col.AutoIncrement)
                        {
                            switch (col.ColumnName.ToUpper())
                            {
                                case "IDAUTO":
                                    row[x] = Convert.ToInt32(General.Nz(entCheltuialaDiurnaTot.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
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
                                    row[x] = Dami.NextId("AvsXDec_DecontDetail", 1);
                                    break;
                                case "EXPENSETYPRID":
                                    row[x] = Convert.ToInt32(itmDiurna_1[0]["DictionaryItemId"].ToString());
                                    break;
                                case "TOTALPAYMENT":
                                    row[x] = valDiurna;
                                    break;
                                case "CURRENCYID":
                                    row[x] = Convert.ToInt32(cmbMonedaAvans.Value ?? General.Nz(ent.Rows[0]["CurrencyId"], -99));
                                    break;
                                case "ESTIMATEDAMOUNT":
                                    row[x] = Convert.ToInt32(General.Nz(ent.Rows[0]["EstimatedAmount"], 0)) + valDiurna;
                                    break;
                                case "DICTIONARYITEMID":
                                    row[x] = Convert.ToInt32(Session["AvsXDec_TipDocument_DiurnaId"].ToString());
                                    break;
                                case "BUGETLINE":
                                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2001)
                                        row[x] = AvsXDec_BudgetLine_Diurna2001;
                                    break;
                                case "DOCDATEDECONT":
                                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2001)
                                        row[x] = ent.Rows[0]["StartDate"];
                                    break;
                                case "AREFISIER":
                                    if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2001)
                                        row[x] = 1;
                                    break;
                            }
                        }
                        x++;
                    }

                    entCheltuialaDiurnaTot.Rows.Add(row);
                    grDateDocJust.DataSource = entCheltuialaDiurnaTot;
                    grDateDocJust.KeyFieldName = "DocumentDetailId;DocumentId";
                    grDateDocJust.DataBind();
                    Session["AvsXDec_SursaDateDocJust"] = entCheltuialaDiurnaTot;         
                }
                #endregion
            }
            else
            {
                /*verificam daca exista cheltuiala de diurna si o stergem*/
                DataTable lstCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                lstCheltuieli.CaseSensitive = false;
                DataRow itmDiurna = lstCheltuieli.Select("DictionaryItemName = 'diurna'").FirstOrDefault();
                if (itmDiurna == null)
                    return;
                DataTable entCheltuialaDiurnaTot = Session["AvsXDec_SursaDateCheltuieli"] as DataTable;
                DataRow entCheltuialaDiurna = null;
                if (entCheltuialaDiurnaTot != null && entCheltuialaDiurnaTot.Rows.Count > 0)
                    entCheltuialaDiurna = entCheltuialaDiurnaTot.Select("ExpenseTypeId = " + itmDiurna["DictionaryItemId"].ToString()).FirstOrDefault();
                if (entCheltuialaDiurna != null)
                {
                    ent.Rows[0]["EstimatedAmount"] = Convert.ToInt32(ent.Rows[0]["EstimatedAmount"].ToString()) - Convert.ToInt32(entCheltuialaDiurna["TotalPayment"].ToString());
                    entCheltuialaDiurna.Delete();
                }
                grDateDocJust.DataSource = entCheltuialaDiurnaTot;
                grDateDocJust.KeyFieldName = "DocumentDetailId;DocumentId";
                Session["AvsXDec_SursaDateDocJust"] = entCheltuialaDiurnaTot;
            }
            Session["AvsXDec_SursaDateDec"] = ent;
        }

        private void ValidateAvansAmount()
        {
            DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
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
            Session["AvsXDec_SursaDateDec"] = ent;
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

        private void ActualizareSumeDecont()
        {
            decimal valDecont = 0, valAvans = 0;
            try
            {
                DataTable ent = Session["AvsXDec_SursaDateDec"] as DataTable;
                if (ent != null && ent.Rows.Count > 0)
                {
                    valDecont = Convert.ToDecimal(String.IsNullOrEmpty(txtValDecont.Text) ? 0.ToString() : txtValDecont.Text.Replace('.', ','));
                    valAvans = Convert.ToDecimal(General.Nz(ent.Rows[0]["TotalAmountAvans"], 0));
                }

                if (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) == 2003)
                    ent.Rows[0]["UnconfRestAmount"] = valAvans - valDecont;
                else
                    ent.Rows[0]["UnconfRestAmount"] = valDecont - valAvans;

                ent.Rows[0]["UnconfRestAmount"] = decimal.Round(Convert.ToDecimal(General.Nz(ent.Rows[0]["UnconfRestAmount"], 0)), 2);

                /*LeonardM 15.08.2016
                 * daca utilizatorul are de returnat bani decontul < avans => se afiseaza gridul de documente plata banca
                 * altfel nu
                 LeonardM 23.08.2016
                 pe decontul administrativ nu ma interseaza restituirea banilor*/


                if (Convert.ToDecimal(General.Nz(ent.Rows[0]["UnconfRestAmount"], 0)) < 0 && Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()) != 2003)
                    pnlPlataBanca.Visible = true;
                else
                    pnlPlataBanca.Visible = false;
                txtValPlataBanca.Text = ent.Rows[0]["UnconfRestAmount"].ToString();
                Session["AvsXDec_SursaDateDec"] = ent;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void SalveazaFisier(int id, metaCereriDate itm)
        {
            try
            {
                string sql = "SELECT * FROM \"tblFisiere\"";
                DataTable dt = new DataTable();
                dt = General.IncarcaDT(sql, null);

                DataRow dr = null;
                if (dt == null || dt.Select("Tabela = 'AvsXDec_relUploadDocumente' AND Id = " + id).Count() == 0)
                {
                    dr = dt.NewRow();
                    dr["Tabela"] = "AvsXDec_relUploadDocumente";
                    dr["Id"] = id;       
                    dr["Fisier"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    if (Constante.tipBD == 1)
                        dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                    else
                        dr["IdAuto"] = Dami.NextId("tblFisiere");
                    dr["EsteCerere"] = 0;
                    dt.Rows.Add(dr);
                }
                else
                {
                    dr = dt.Select("Tabela = 'AvsXDec_relUploadDocumente' AND Id = " + id).FirstOrDefault();
                    dr["Fisier"] = itm.UploadedFile;
                    dr["FisierNume"] = itm.UploadedFileName;
                    dr["FisierExtensie"] = itm.UploadedFileExtension;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                }
                General.SalveazaDate(dt, "tblFisiere");               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetAvsXDec_AvansXDecont(int userId, int documentTypeId, int documentId)
        {
            #region preluare suma utilizata din avans
            string sqlQuery = string.Empty;
            string sqlAvansAcordat = "", sql = "", sqlAvansXDecont = "";
            #endregion

            /*documentTypeId = tipul de document de decont
             * pentru a stii ce tipuri de avansuri sa aduc
             * nu pot inchide avans de deplasare cu decont administrativ*/
            DataTable q = null;
            try
            {
                /*identificam avansurile care au fost puse deja pe un decont, pentru a nu le mai aduce in selectie*/
                sqlAvansAcordat = "SELECT avans.DocumentId,  avans.DocumentDate, avans.DocumentTypeId, COALESCE(bt.DestDocId, -99) as DestDocId, dec.DocumentStateId as DecontDocStateId "
                    + " FROM AvsXDec_Document avans "
                    + " JOIN AvsXDec_BusinessTransaction bt on avans.DocumentId = bt.SrcDocId "
                    + " JOIN AvsXDec_Document dec on bt.DestDocId = dec.DocumentId "
                    /*LeonardM 21.09.2016
                 * verificam daca un avans a fost pe un decont in starea initial
                 * old
                 * where dec.DocumentStateId >= 4 && avans.USER_NO == userId
                 * */
                    /*LeonardM 27.09.2016
                     * verificam pentru toate starile pentru decont, deoarece la duplicare,
                     * pe decontul anterior, nu imi mai aduce avansul ca sursa
                    old
                    where dec.DocumentStateId >= 1 && avans.USER_NO == userId */
                    /*LeonardM 23.08.2016
                * nu ma intereseaza avansul administrativ de cine a fost legat si cum*/
                    + " where avans.USER_NO = " + userId + " AND avans.DocumentTypeId != 1003 "
                    + " UNION "
                    + " SELECT avans.DocumentId,  avans.DocumentDate, avans.DocumentTypeId, COALESCE(bt.DestDocId, -99) as DestDocId, dec.DocumentStateId as DecontDocStateId "
                    + " FROM AvsXDec_Document avans "
                    + " JOIN AvsXDec_BusinessTransaction bt on avans.DocumentId = bt.SrcDocId "
                    + " JOIN AvsXDec_Document dec on bt.DestDocId = dec.DocumentId "
                    + " where avans.DocumentStateId = 8 and avans.USER_NO = " + userId  + "  and avans.DocumentTypeId = 1003 ";

                sqlAvansXDecont = "SELECT a.DocumentId, a.DocumentDate, a.DocumentTypeId, MAX(a.DecontDocStateId) as DecontDocStateId FROM (" + sqlAvansAcordat + ") a GROUP BY a.DocumentId, a.DocumentDate, a.DocumentTypeId";
                
                sql = "SELECT a.DocumentId, a.DocumentDate, a.DocumentTypeId, c.DocumentTypeName, b.TotalAmount as PaymentAmount, COALESCE(b.CurrencyId, -99) AS CurrencyId, d.DictionaryItemName as CurrencyCode "
                    + " FROM AvsXDec_Document a "
                    + " JOIN AvsXDec_Avans b on a.DocumentId = b.DocumentId "
                    + " JOIN AvsXDec_DocumentType c on a.DocumentTypeId = c.DocumentTypeId "
                    + " JOIN AvsXDec_DictionaryItem d on b.CurrencyId = d.DictionaryItemId "
                    /*nu luam in considerare avansurile legate deja de un decont*/
                    + " LEFT JOIN (" + sqlAvansAcordat + ") e on a.DocumentId = e.DocumentId "
                    + " LEFT JOIN (" + sqlAvansXDecont + ") f on e.DocumentId = f.DocumentId "
                    /*LeonardM 22.05.2016
                    * aducem doar avansurile care au fost acordate / s-a efectuat plata de catre financiar*/
                    + "  where a.DocumentStateId >= 4 and a.USER_NO = " + userId
                    /*LeonardM 10.08.2016
                     * daca sunt pe un document care are alocat un avans, trebuie sa aduc ca sursa
                     * de date avansul respectiv pentru a-l mapa mai departe*/
                    + " AND (e.DocumentId is null or e.DestDocId = " + documentId + " or (e.DestDocId != " + documentId + " and (f.DecontDocStateId <= 0 or f.DocumentTypeId = 1003))) ";

                q = General.IncarcaDT(sql, null);


                if (q != null && q.Rows.Count > 0)
                {
                    /*Decont cheltuieli deplasare = > Avans spre deplasare*/
                    if (documentTypeId == 2001)
                        q = q.Select("DocumentTypeId = 1001").CopyToDataTable();

                    /*Decont cheltuieli => Avans spre decontare*/
                    if (documentTypeId == 2002)
                        q = q.Select("DocumentTypeId = 1002").CopyToDataTable();

                    /*Decont administrativ => Avans administrativ*/
                    //if (documentTypeId == 2003)
                    //    q = q.Where(p => p.DocumentTypeId == 1003);
                }
                return q;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }


    }
}