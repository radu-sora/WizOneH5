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
    public partial class DocumentAvans : System.Web.UI.Page
    {

        int nrPost = 0;

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
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

				btnPrint.Text = Dami.TraduCuvant("btnPrint", "Imprima");
                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aprobare");
                btnRespins.Text = Dami.TraduCuvant("btnRespins", "Respinge");				
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
				
				lblNrOrdin.InnerText = Dami.TraduCuvant("Nr. ordin");
				lblNume.InnerText = Dami.TraduCuvant("Nume");
				lblDept.InnerText = Dami.TraduCuvant("Departament");
				lblLocMunca.InnerText = Dami.TraduCuvant("Loc munca");
				lblIBAN.InnerText = Dami.TraduCuvant("IBAN");
				lblLocDepl.InnerText = Dami.TraduCuvant("Locul deplasarii");
				lblTipDepl.InnerText = Dami.TraduCuvant("Tip deplasare");
				lblTipTrans.InnerText = Dami.TraduCuvant("Tip transport");
				lblMotiv.InnerText = Dami.TraduCuvant("Motiv deplasare");
				lblDtPlec.InnerText = Dami.TraduCuvant("Data plecare");
				lblOraPlec.InnerText = Dami.TraduCuvant("Ora plecare");
				lblDtSos.InnerText = Dami.TraduCuvant("Data sosirii din delegatie");
				lblDtSf.InnerText = Dami.TraduCuvant("Ora sosire");
				lblMoneda.InnerText = Dami.TraduCuvant("Moneda avans/decont");
				lblModPlata.InnerText = Dami.TraduCuvant("Modalitate plata");
				lblDiurna.InnerText = Dami.TraduCuvant("Deplasare cu diurna");
				lblValEst.InnerText = Dami.TraduCuvant("Val. estimata");
				lblValAvsSol.InnerText = Dami.TraduCuvant("Valoare avans solicitat");
				lblDtScad.InnerText = Dami.TraduCuvant("Data scadenta");
				lblRez.InnerText = Dami.TraduCuvant("Rezervari");				
				
				pnlDateGen.HeaderText = Dami.TraduCuvant("Date generale");		
				pnlDateDepl.HeaderText = Dami.TraduCuvant("Date deplasare");	
				pnlDatePlata.HeaderText = Dami.TraduCuvant("Date plata");	
				pnlCheltEst.HeaderText = Dami.TraduCuvant("Cheltuieli estimate");	
				pnlDateAv.HeaderText = Dami.TraduCuvant("Date avans");		

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString() + " / Document Avans"; ;

                if (!IsPostBack)
                {
                    Session["AvsXDec_SursaDate"] = null;
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

                double procentCheltuieliTotaleAvansDeplasare = Convert.ToDouble(Dami.ValoareParam("AvsXDec_Modul_ProcentAvansDeplasareCheltuieliEstimate", "0"));
				double procentCheltuieliTotaleAvansDecontare = Convert.ToDouble(Dami.ValoareParam("AvsXDec_Modul_ProcentAvansDecontareCheltuieliEstimate", "0"));
                   
				DataTable lstConfigCurrencyXPay = GetAvsXDec_ConfigCurrencyXPay(Convert.ToInt32(Session["AvsXDec_Marca"].ToString()), Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
                Session["ConfigCurrencyXPay"] = lstConfigCurrencyXPay;
                DataTable lpTipDeplasare = GetAvsXDec_DictionaryItemTipDeplasare();
				DataTable lpTipMoneda = GetAvsXDec_DictionaryItemValute();
				DataTable lpModPlata = GetAvsXDec_DictionaryItemModalitatePlata();
                        

                 
				if (lpTipDeplasare != null && lpTipDeplasare.Rows.Count > 0)
				{
				   
					if (lpTipMoneda != null && lpTipMoneda.Rows.Count > 0)
					{

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
								if (lpTipMoneda.Select("LOWER(DictionaryItemName) LIKE ('%ron%')").Count() != 0)
                                    Session["IdMonedaRON"] = lpTipMoneda.Select("LOWER(DictionaryItemName) LIKE ('%ron%')")[0]["DictionaryItemId"].ToString();
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
								if (lpTipDeplasare.Select("LOWER(DictionaryItemName) LIKE ('%interna%')").Count() != 0)
                                    Session["IdDeplasareInterna"] = lpTipDeplasare.Select("LOWER(DictionaryItemName) LIKE ('%interna%')")[0]["DictionaryItemId"].ToString();
								if (lpTipDeplasare.Select("LOWER(DictionaryItemName) LIKE ('%externa%')").Count() != 0)
                                    Session["IdDeplasareInterna"] = lpTipDeplasare.Select("LOWER(DictionaryItemName) LIKE ('%externa%')")[0]["DictionaryItemId"].ToString();

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
                    txtNrOrdinDeplasare.ClientEnabled = false;
                    txtNumeComplet.ClientEnabled = false;
                    txtDepartament.ClientEnabled = false;
                    txtLocMunca.ClientEnabled = false;
                    txtContIban.ClientEnabled = false;
                    txtLocatie.ClientEnabled = false;
                    cmbActionType.ClientEnabled = false;
					cmbTransportType.ClientEnabled = false;
                    txtActionReason.ClientEnabled = false;
                    txtStartDate.ClientEnabled = false;
                    txtOraPlecare.ClientEnabled = false;
                    txtEndDate.ClientEnabled = false;
                    txtOraSosire.ClientEnabled = false;
                    cmbMonedaAvans.ClientEnabled = false;
                    
                    cmbModPlata.ClientEnabled = false;
                    chkIsDiurna.ClientEnabled = false;
                    txtValEstimata.ClientEnabled = false;
                    txtValAvans.ClientEnabled = false;
                    dtDueDate.ClientEnabled = false;
                    cmbTip.ClientEnabled = false;

                    btnSave.ClientEnabled = false;
                    //dreptUserAccesFormular = tipAccesPagina.formularSalvat;
                }
                //else if (!esteNou)
                //    dreptUserAccesFormular = tipAccesPagina.formularSalvatEditUser;
                //else
                //    dreptUserAccesFormular = tipAccesPagina.formularNou;

                #region drept aprobare/refuz document

                btnAproba.ClientEnabled = Convert.ToInt32(Session["AvsXDec_PoateAprobaXRefuzaDoc"].ToString()) == 1 ? true : false;
                btnRespins.ClientEnabled = Convert.ToInt32(Session["AvsXDec_DocCanBeRefused"].ToString()) == 1 ? true : false; 

                #endregion

                PageXDocumentType();  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }      


        private void PageXDocumentType()
        {
            switch (Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()))
            {
                /*Avans spre deplasare*/
                case 1001:
                    /*conform specificatiilor pentru tipul de document avans spre deplasare,
                     * data scadenta nu se poate edita, ci doar din baza de date se actualizeaza prin procedura
                     * procAvsXDec_dtPlata1001*/
                    grDate.Columns["FreeTxt"].Visible = false;
					lblDtScad.Visible = true;
					dtDueDate.ClientVisible = true;
                    dtDueDate.ClientEnabled = false;
                    LoadAvansReservations(false);
                    pnlCheltEst.HeaderText = "Cheltuieli estimate";
                    break;
                /*Avans spre decontare*/
                case 1002:
                    grDate.Columns["FreeTxt"].Visible = true;
                    pnlDateDepl.ClientVisible = false;
					lblDiurna.Visible = false;
					chkIsDiurna.ClientVisible = false;
					lblRez.Visible = false;
					cmbTip.ClientVisible = false;
                    pnlCheltEst.HeaderText = "Cheltuieli";
                    break;
                /*Avans Administrativ*/
                case 1003:
                    //grDate.Columns["FreeTxt"].Visible = false;
					//pnlDateDepl.ClientVisible = false;
					//lblDiurna.ClientVisible = false;
					//chkIsDiurna.ClientVisible = false;
                    //liModPlata.Visibility = Visibility.Collapsed;
					//pnlCheltEst.ClientVisible = false;
                    //lcCheltuieli.Visibility = Visibility.Collapsed;
					//lblDtScad.ClientVisible = false;
					//dtDueDate.ClientVisible = false;
					//lblRez.ClientVisible = false;
					//cmbTip.ClientVisible = false;
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

        protected void Page_Unload(object sender, EventArgs e)
        {
            string ert = "";
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
		
		
        private void btnSave_ItemClick(object sender, EventArgs e)
        {
            try
            {
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
                            MessageBox.Show(Dami.TraduCuvant("Eroare la salvarea rezervarilor: \n" + msg), MessageBox.icoError, "Atentie !");
						}
						else
						{
							SalvareDoc();
						}
                        
                        break;
                    case 1002: /*Avans spre decontare*/
                        SalvareDoc();
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

		public void SalvareDoc()
		{
			string sql = "";
			
			if (Convert.ToInt32(Session["AvsXDec_EsteNou"].ToString()) == 0)
			{
				sql = "UPDATE AvsXDec_AvansDetail SET DictionaryItemId = " + ent.DictionaryItemId + ", DocumentDetailId = " + ent.DocumentDetailId 
					+ ", Amount = " + ent.Amount + ", DocumentId = " + ent.DocumentId + ", FreeTxt = '" + ent.FreeTxt 
					+ "' WHERE DocumentDetailId = " + ent.DocumentDetailId + " AND DocumentId = " + ent.DocumentId;
				General.ExecutaNonQuery(sql, null);	
			}
			else
			{
				sql = "INSERT INTO AvsXDec_AvansDetail(DictionaryItemId, DocumentDetailId, DocumentId, Amount, FreeTxt) "
					+ " VALUES (" + ent.DictionaryItemId + ", " + ent.DocumentDetailId + ", " + ent.DocumentId + ", " + ent.Amount + " '" + ent.FreeTxt + "')";
				General.ExecutaNonQuery(sql, null);					
			}
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
							+ " where a.DocumentId = " + documentId 
							+ " ORDER BY Ordine";
				
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
			DataTable dtCheltuieli = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
			DataTable dtNomenCheltuieli = GetAvsXDec_DictionaryItemCheltuiala(Convert.ToInt32(Session["AvsXDec_DocumentTypeId"].ToString()));
			
            DataRow[] itmDiurna = dtNomenCheltuieli.Select("LOWER(DictionaryItemName) = 'diurna'");
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
                        if (Convert.ToDecimal((dtCheltuieli.Rows[i]["Amount"] == DBNull.Value ? 0 : dtCheltuieli.Rows[i]["Amount"]).ToString()) > Convert.ToDecimal(Session["maxValueDiurna"].ToString()))
                        {
                            ras +=", diurna nu poate fi mai mare decat " + Session["maxValueDiurna"].ToString() + " conform configurarilor stabilite! ";
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
                        if (ent.Rows[0]["ActionPlace"] == null) ras += ", locatie";
                        if (ent.Rows[0]["ActionReason"] == null) ras += ", motiv deplasare";
                        if (ent.Rows[0]["StartDate"] == null) ras += ", data inceput";
                        if (ent.Rows[0]["EndDate"] == null) ras += ", data sfarsit";
                        if (string.IsNullOrEmpty(txtOraPlecare.Text) && ent.Rows[0]["StartHour"] == null)
                            ras += ", ora inceput";
                        else
                        {
                            if (ent.Rows[0]["StartHour"] == null)
                            {
                                ent.Rows[0]["StartHour"] = DateTime.ParseExact(txtOraPlecare.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                if (ent.Rows[0]["StartHour"] == null)
                                    ras += ", ora inceput";
                            }
                        }
                        if (string.IsNullOrEmpty(txtOraSosire.Text) && ent.Rows[0]["EndHour"] == null)
                            ras += ", ora sfarsit";
                        else
                        {
                            if (ent.Rows[0]["EndHour"] == null)
                            {
                                ent.Rows[0]["EndHour"] = DateTime.ParseExact(txtOraSosire.Text, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                                if (ent.Rows[0]["EndHour"] == null)
                                    ras += ", ora sfarsit";
                            }
                        }
                        if (ent.Rows[0]["PaymentTypeId"] == null) ras += ", modalitate plata";
                        if (ent.Rows[0]["CurrencyId"] == null) ras += ", moneda plata";
                        if (ent.Rows[0]["ActionTypeId"] == null) ras += ", tip deplasare";
                        if (txtValEstimata.Value == null || Convert.ToDecimal(txtValEstimata.Value) == 0) ras += ", cheltuieli pentru avans";
                        break;
                    case 1002: /*Avans spre decontare*/
                        if (ent.Rows[0]["CurrencyId"] == null) ras += ", moneda plata";
                        if (ent.Rows[0]["PaymentTypeId"] == null) ras += ", modalitate plata";
                        if (ent.Rows[0]["DueDate"] == null) ras += ", data scadenta";
                        if (txtValEstimata.Value == null || Convert.ToDecimal(txtValEstimata.Value) == 0) ras += ", cheltuieli pentru avans";
                        break;


                }
				DataTable dtCheltuieli = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieli != null && dtCheltuieli.Rows.Count != 0 && dtCheltuieli.Select("DictionaryItemId IS NULL").Count() != 0)
                    ras += ", cheltuiala completata pe unele randuri";
                if (ras != "") ras = "Lipseste " + ras.Substring(2) + " !";
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
                int idRap = Convert.ToInt32(Dami.ValoareParam("IdRaportFisaPost", "-99"));
                if (idRap != -99 && Convert.ToInt32(General.Nz(Session["IdAuto"], "-99")) != -99)
                {
                    var reportParams = new
                    {
                        IdAutoPost = Convert.ToInt32(General.Nz(Session["IdAuto"], "-99"))
                    };

                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
                    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);

                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Org_FisaPost", "window.location.href = \"" + ResolveClientUrl(reportUrl) + "\"", true);
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
                switch (e.Parameter)
                {
                    case "cmbCmp":
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
            DataRow[] itmDiurna = dtCheltuieli.Select("LOWER(DictionaryItemName) = 'diurna'");
            if (itmDiurna != null && itmDiurna.Length > 0)
            {
                DataTable dtCheltuieliAvans = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieliAvans.Rows.Count != 0)
                {
                    DataRow[] entCheltuialaDiurna = dtCheltuieliAvans.Select("DictionaryItemId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                    if (entCheltuialaDiurna != null && entCheltuialaDiurna.Length > 0)
                        Session["maxValueDiurna"] = Convert.ToDecimal((entCheltuialaDiurna[0]["Amount"] == DBNull.Value ? 0 : entCheltuialaDiurna[0]["Amount"]).ToString());
                }
            }
            #endregion


            #region cheltuiala limita cazare
            DataRow[] itmCazare = dtCheltuieli.Select("LOWER(DictionaryItemName) = 'cazare'");
            if (itmCazare != null && itmCazare.Length > 0)
            {
                DataTable dtCheltuieliAvans = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
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
            DataRow[] itmDiurna = dtCheltuieli.Select("LOWER(DictionaryItemName = 'diurna'");
            if (itmDiurna != null && itmDiurna.Length > 0)
            {
                DataTable dtCheltuieliAvans = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                if (dtCheltuieliAvans.Rows.Count != 0)
                {
                    DataRow[] entCheltuialaDiurna = dtCheltuieliAvans.Select("DictionaryItemId = " + itmDiurna[0]["DictionaryItemId"].ToString());
                    if (entCheltuialaDiurna != null && entCheltuialaDiurna.Length > 0)
                        Session["maxValueDiurna"] = Convert.ToDecimal((entCheltuialaDiurna[0]["Amount"] == DBNull.Value ? 0 : entCheltuialaDiurna[0]["Amount"]).ToString());
                }
            }
            #endregion

            #region cheltuiala cazare
            DataRow[] itmCazare = dtCheltuieli.Select("LOWER(DictionaryItemName) = 'cazare'");
            if (itmCazare != null && itmCazare.Length > 0)
            {
                DataTable dtCheltuieliAvans = GetmetaAvsXDec_AvansDetailCheltuieli(Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString()));
                DataRow[] entCheltuialaCazare = dtCheltuieliAvans.Select("DictionaryItemId = " + itmCazare[0]["DictionaryItemId"].ToString());
                if (entCheltuialaCazare != null && entCheltuialaCazare.Length > 0)
                    LoadAvansReservations(true);
            }
            #endregion

        }		
		
		private void cmbMonedaAvans_SelectedIndexChanged(object sender, EventArgs e)
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
                        if (lstConfigCurrencyXPay.Rows.Count != 0)
                        {
                            cmbModPlata.Value = null;
                            lstConfigCurrencyXPay_PayCopy.Clear();
							DataRow[] entConfig = dtConfigCurrencyXPay.Select("CurrencyId = " + Convert.ToInt32(cmbMonedaAvans.Value ?? -99));
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
				List<object> lst = new List<object>();
				for (int i = 0; i < loRez.Rows.Count; i++)
				{
					lst.Add(Convert.ToInt32(loRez.Rows[i]["DictionaryItemId"].ToString()));
				}
				cmbTip.Value = lst;
			
               
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
        protected void grDateIstoric_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow found = dt.Rows.Find(e.Keys["IdAuto"]);
                found.Delete();            


                #region actualizare valoare avans
                /*LeonardM 25.08.2016
                 * actualizarea sumelor o realizam mai jos cu diurna,
                 * deoarece daca dezactivam bifa de diurna se scade de doua ori valoarea diurnei
                 * si nu e ok*/
                metaAvsXDec_AvansDetailCheltuieli tmp = tvDateCheltuieli.Grid.GetRow(rowHandle) as metaAvsXDec_AvansDetailCheltuieli;
                var ent = dds.DataView.Cast<metaVwAvsXDec_Avans>().FirstOrDefault();
                #endregion

                #region stergere diurna
                /*verificam daca exista cheltuiala de diurna si o stergem*/
                List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
                metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
                if (itmDiurna != null)
                {
                    if (tmp.DictionaryItemId == itmDiurna.DictionaryItemId)
                    {
                        chkIsDiurna.Checked = false;
                        maxValueDiurna = 0;
                    }
                    else
                    {
                        ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmp.Amount ?? 0);
                        ddsCheltuieli.DataView.Remove(rand);
                    }
                }
                else
                {
                    ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmp.Amount ?? 0);
                    ddsCheltuieli.DataView.Remove(found);
                }
                #endregion


                #region cheltuiala Cazare
                /*LeonardM 15.08.2016
                * solicitare ca in momentul in care utilizatorul selecteaza cheltuiala cu cazare, sa nu mai poata
                * face rezervare pentru cazare */
                metaAvsXDec_DictionaryItem itmCazare = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "cazare").FirstOrDefault();
                if (itmCazare != null)
                {
                    if (tmp.DictionaryItemId == itmCazare.DictionaryItemId)
                    {
                        LoadAvansReservations(false);
                    }
                }
                #endregion


                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	
      
		

        protected void grDateIstoric_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow[] arr = dt.Select("DataInceput <= #" + Convert.ToDateTime(e.NewValues["DataSfarsit"]).ToString("yyyy-MM-dd") + "# AND #" + Convert.ToDateTime(e.NewValues["DataInceput"]).ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                if (arr.Length == 0)
                {
                    DataRow dr = dt.NewRow();

                    dr["IdPost"] = txtId.Value ?? -99;
                    dr["Pozitii"] = e.NewValues["Pozitii"] ?? 0;
                    dr["PozitiiAprobate"] = e.NewValues["PozitiiAprobate"] ?? 0;
                    dr["DataInceput"] = e.NewValues["DataInceput"];
                    dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    dt.Rows.Add(dr);

                    PopuleazaPozitii(dt);
                }
                else
                    grDateIstoric.JSProperties["cpAlertMessage"] = "Acest interval se intersecteaza cu unul deja existent";

                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow[] arr = dt.Select("IdAuto <> " + e.Keys["IdAuto"] + " AND DataInceput <= #" + Convert.ToDateTime(e.NewValues["DataSfarsit"]).ToString("yyyy-MM-dd") + "# AND #" + Convert.ToDateTime(e.NewValues["DataInceput"]).ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                if (arr.Length == 0)
                {
                    DataRow dr = dt.Rows.Find(e.Keys["IdAuto"]);

                    dr["Pozitii"] = e.NewValues["Pozitii"] ?? 0;
                    dr["PozitiiAprobate"] = e.NewValues["PozitiiAprobate"] ?? 0;
                    dr["DataInceput"] = e.NewValues["DataInceput"];
                    dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    PopuleazaPozitii(dt);
                }
                else
                    grDateIstoric.JSProperties["cpAlertMessage"] = "Acest interval se intersecteaza cu unul deja existent";

                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {

                e.NewValues["DocumentId"] = Convert.ToInt32(Session["AvsXDec_IdDocument"].ToString());
                e.NewValues["DocumentDetailId"] = Dami.NextId("AvsXDec_AvansDetail", 1);
                /*comentam aceasta parte, deoarece in momentul in care adaugam diurna
                 * dorim valoarea sa fie cea asignata de mine prin calcul
                 * ent.Amount = 0;
                 * */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }		
		
		
        private void tvDateCheltuieli_ValidateRow(object sender, DevExpress.Xpf.Grid.GridRowValidationEventArgs e)
        {
            try
            {
                if (e.Row == null) return;

                e.ErrorContent += Dami.AreNume(ref grDateCheltuieli, e.RowHandle, "DocumentDetailId", "Lipseste detaliu document. ");
                e.ErrorContent += Dami.AreNume(ref grDateCheltuieli, e.RowHandle, "DocumentId", "Lipseste document. ");

                #region verificare completare diurna
                /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                List<metaAvsXDec_AvansDetailCheltuieli> lstCheltuieliInserate = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().ToList();
                List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
                metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
                if ((e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId == itmDiurna.DictionaryItemId && chkIsDiurna.IsChecked == false)
                {
                    (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId = null;
                    e.ErrorContent += "Diurna se adauga automat prin bifarea optiunii de calcul diurna!";
                }
                if ((e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId != null)
                {
                    if (lstCheltuieliInserate.Where(p => p.DictionaryItemId == (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId && p.DocumentDetailId != (e.Value as metaAvsXDec_AvansDetailCheltuieli).DocumentDetailId).Count() != 0)
                    {
                        (e.Value as metaAvsXDec_AvansDetailCheltuieli).DictionaryItemId = null;
                        e.ErrorContent += "Acest tip de cheltuiala a mai fost adaugat!";
                    }
                }
                #endregion

                if ((string)(e.ErrorContent) != "")
                    e.IsValid = false;
                //else
                //    dds.SubmitChanges();
            }
            catch (Exception ex)
            {
                Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	
        private void tvDateCheltuieli_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            try
            {
                var ent = dds.DataView.Cast<metaVwAvsXDec_Avans>().FirstOrDefault();
                metaAvsXDec_AvansDetailCheltuieli tmpRowChanged = grDateCheltuieli.GetFocusedRow() as metaAvsXDec_AvansDetailCheltuieli;
                switch (e.Column.FieldName)
                {
                    case "colSel":
                        ((TableView)sender).PostEditor();
                        break;
                    case "Amount":
                        decimal valoareCheltuieli = 0;
                        valoareCheltuieli = Convert.ToDecimal(ent.EstimatedAmount ?? 0);
                        if (e.Value != null)
                        {
                            #region verificare limite diurna
                            /*LeonardM 11.09.2016
                             * se seteaza implicit diurna cu valoarea conform calculelor, dar poate fi editata
                             * cerinta Groupama: sa poata fi editata in minus */
                            List<metaAvsXDec_DictionaryItem> lstCheltuieli_1 = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
                            metaAvsXDec_DictionaryItem itmDiurna_1 = lstCheltuieli_1.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
                            if (itmDiurna_1 != null)
                            {
                                if (Convert.ToInt32(tmpRowChanged.DictionaryItemId) == itmDiurna_1.DictionaryItemId)
                                {
                                    if (Convert.ToDecimal(e.Value) > maxValueDiurna)
                                    {
                                        tmpRowChanged.Amount = maxValueDiurna;
                                        Message.InfoMessage("Diurna nu poate fi mai mare decat " + maxValueDiurna.ToString() + " conform configurarilor stabilite! ");
                                    }
                                }
                            }
                            #endregion

                            decimal valueExpenseCorrected = 0, minValue = 0, maxValue = 0;
                            if (tmpRowChanged.DictionaryItemId != null)
                                if (!SumIsValidForExpense((int)tmpRowChanged.DictionaryItemId, Convert.ToDecimal(e.Value.ToString()), out valueExpenseCorrected, out minValue, out maxValue))
                                {
                                    bool depasesteLimitaSetata = false;

                                    if (itmDiurna_1 != null)
                                    {
                                        if (itmDiurna_1.DictionaryItemId != tmpRowChanged.DictionaryItemId)
                                            depasesteLimitaSetata = true;
                                    }
                                    else
                                    {
                                        #region verificare suma intre limite
                                        depasesteLimitaSetata = true;
                                        #endregion
                                    }
                                 
                                    if (depasesteLimitaSetata)
                                    {
                                        Message.InfoMessage("Suma introdusa nu se incadreaza intre limitele setate in aplicatie: " + minValue.ToString() + " - " + maxValue.ToString() + "!");
                                    }
                                }                           

                            if (e.OldValue == null)
                            {
                                /*rand nou*/
                                valoareCheltuieli = valoareCheltuieli + Convert.ToDecimal(e.Value.ToString());
                            }
                            else
                            {
                                /*rand vechi pentru care se actualizeaza suma*/
                                valoareCheltuieli = valoareCheltuieli - Convert.ToDecimal(e.OldValue.ToString()) + Convert.ToDecimal(e.Value.ToString());
                            }
                        }
                        ent.EstimatedAmount = valoareCheltuieli;
                        break;
                    case "DictionaryItemId":
                        /*verificam daca exista acelasi tip de cheltuiala anteriori*/
                        List<metaAvsXDec_AvansDetailCheltuieli> lstCheltuieliInserate = ddsCheltuieli.DataView.Cast<metaAvsXDec_AvansDetailCheltuieli>().ToList();
                        List<metaAvsXDec_DictionaryItem> lstCheltuieli = ddsNomenclatorCheltuieli.DataView.Cast<metaAvsXDec_DictionaryItem>().ToList();
                        metaAvsXDec_DictionaryItem itmDiurna = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "diurna").FirstOrDefault();
                        if (itmDiurna != null)
                        {
                            if (Convert.ToInt32(e.Value) == itmDiurna.DictionaryItemId && chkIsDiurna.IsChecked == false)
                            {
                                grDateCheltuieli.SetCellValue(tvDateCheltuieli.FocusedRowHandle, "DictionaryItemId", null);
                                Message.InfoMessage("Diurna se adauga automat prin bifarea optiunii de calcul diurna!");
                                return;
                            }
                        }

                        /*LeonardM 10.08.2016
                        * cerinta de la Groupama ca pentru avansul spre decontare sa se poata adauga doar un tip de cheltuiala
                        * in afara de diurna, deoarece pentru acest tip de document exista circuit definit pentru un singur tip de 
                        * cheltuiala*/
                        switch (documentTypeId)
                        {
                            case 1001: /*Avans spre deplasare*/
                                if (lstCheltuieliInserate.Where(p => p.DictionaryItemId == Convert.ToInt32(e.Value) && p.DocumentDetailId != tmpRowChanged.DocumentDetailId).Count() != 0)
                                {

                                    Message.InfoMessage("Acest tip de cheltuiala a mai fost adaugat!");

                                    if (!grDateCheltuieli.View.AllowEditing)
                                        return;

                                    var rand = grDateCheltuieli.GetFocusedRow();// tvDateCheltuieli.Grid.GetRow(rowHandle);

                                    #region actualizare valoare avans
                                    /*LeonardM 25.08.2016
                                         * actualizarea sumelor o realizam mai jos cu diurna,
                                         * deoarece daca dezactivam bifa de diurna se scade de doua ori valoarea diurnei
                                         * si nu e ok*/
                                    #endregion

                                    #region stergere diurna
                                    /*verificam daca exista cheltuiala de diurna si o stergem*/
                                    if (itmDiurna != null)
                                    {
                                        if (tmpRowChanged.DictionaryItemId == itmDiurna.DictionaryItemId)
                                        {
                                            chkIsDiurna.IsChecked = false;
                                        }
                                        else
                                        {
                                            ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmpRowChanged.Amount ?? 0);
                                            tvDateCheltuieli.CommitEditing();
                                            ddsCheltuieli.DataView.Remove(tmpRowChanged);
                                        }
                                    }
                                    else
                                    {
                                        ent.EstimatedAmount = (ent.EstimatedAmount ?? 0) - (tmpRowChanged.Amount ?? 0);
                                        tvDateCheltuieli.CommitEditing();
                                        ddsCheltuieli.DataView.Remove(tmpRowChanged);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region cheltuiala Cazare
                                    /*LeonardM 15.08.2016
                                     * solicitare ca in momentul in care utilizatorul selecteaza cheltuiala cu cazare, sa nu mai poata
                                     * face rezervare pentru cazare */
                                    metaAvsXDec_DictionaryItem itmCazare = lstCheltuieli.Where(p => p.DictionaryItemName.ToLower() == "cazare").FirstOrDefault();
                                    if (itmCazare != null)
                                    {
                                        if (Convert.ToInt32(e.Value) == itmCazare.DictionaryItemId)
                                        {
                                            LoadAvansReservations(true);
                                        }
                                    }
                                    #endregion
                                }
                                break;
                            case 1002:/*Avans spre decontare*/
                                if (lstCheltuieliInserate.Where(p => p.DictionaryItemId != Convert.ToInt32(e.Value) && p.DocumentDetailId != tmpRowChanged.DocumentDetailId).Count() != 0)
                                {
                                    Message.InfoMessage("Trebuie selectat doar un tip de cheltuiala pentru avansul spre decontare!");
                                    tmpRowChanged.DictionaryItemId = null;
                                    return;
                                }
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void dds_SubmittedChanges(object sender, SubmittedChangesEventArgs e)
        {
            try
            {       
				int rez = SetCircuitSettingsDocument(idDocument);	
				switch (rez)
				{
					case -123:
						Message.InfoMessage("Nu exista circuit definit pentru proprietatile alese!");
						
						break;
					case 0:
						Message.InfoMessage(Dami.TraduCuvant("Date salvate cu succes."));
						//General.NotificariDocumente(idDocument);
						apasat = true;
						btnInapoi_ItemClick(null, null);
						break;
				}

            }
            catch (Exception ex)
            {
                Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	

        public int SetCircuitSettingsDocument(int IdDocument)
        {
            try
            {
                string sql = "";
                DataTable entDocument = General.IncarcaDT("SELECT * FROM AvsXDec_Document WHERE DocumentId = " + IdDocument, null); 
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
                if (lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) IN (" + lstDIM1 + ") AND COALESCE(DIM2, -99) IN (" + lstDIM2 + ")") == null)
                {
                    /*nu am valori care sa coincida DIM1 si DIM2, incerc sa vad daca am ceva pe DIM1 care sa se potriveasca*/
                    if (lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) IN (" + lstDIM1 + ") AND COALESCE(DIM2, -99) = -99") != null)
                        entCircuit = lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) IN (" + lstDIM1 + ") AND COALESCE(DIM2, -99) = -99").CopyToDataTable();
                    else /*nu am valori asociate pe DIM1, incerc sa gasesc valori asociate pentru DIM2*/
                        if (lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) = -99 AND COALESCE(DIM2, -99) IN (" + lstDIM2 + ")") != null)
                            entCircuit = lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) = -99 AND COALESCE(DIM2, -99) IN (" + lstDIM2 + ")").CopyToDataTable();
                        else /* nu am valori asociate pe DIM1 si DIM2, incerc sa gasesc valori asociate pentru DIM1 = -99, DIM2 = -99 */
                            entCircuit = lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) = -99 AND COALESCE(DIM2, -99) = -99").CopyToDataTable();
                }
                else
                    entCircuit = lstCircuitXDocumentType.Select("COALESCE(DIM1, -99) IN (" + lstDIM1 + ") AND COALESCE(DIM2, -99) IN (" + lstDIM2 + ")").CopyToDataTable();

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

        private void GolireVariabile()
        {
            try
            {
                Session["DataVigoare"] = null;
                Session["IdAuto"] = -97;
                Session["Org_Duplicare"] = "0";
                Session["Posturi_Upload"] = null;
                Session["Org_PosturiPozitii"] = null;
                Session["Org_CampuriExtra"] = null;
                Session.Remove("DataVigoare");
                Session.Remove("IdAuto");
                Session.Remove("Org_Duplicare");
                Session.Remove("Posturi_Upload");
                Session.Remove("Org_PosturiPozitii");
                Session.Remove("Org_CampuriExtra");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}