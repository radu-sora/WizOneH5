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
    public partial class Document : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
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
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");              
                btnNou.Text = Dami.TraduCuvant("btnNou", "Adauga");
                btnDuplicare.Text = Dami.TraduCuvant("btnDuplicare", "Duplicare"); 
				
                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");               
                
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

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();	
				

                DataTable dtStari = General.IncarcaDT(@"SELECT b.""DictionaryItemId"" as ""Id"", b.""DictionaryItemName""  as ""Denumire"", b.""Culoare"" FROM ""vwAvsXDec_Nomen_StariDoc"" a LEFT JOIN ""AvsXDec_DictionaryItem"" b ON  a.DictionaryItemId = b.DictionaryItemId ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;	
				
				
				cmbDocumentType.DataSource = General.IncarcaDT("SELECT * FROM AvsXDec_DocumentType WHERE DocumentTypeId IN (1001, 1002, 2001, 2002) ", null);
				cmbDocumentType.DataBind();


				if (!IsPostBack)
				{
					Session["AvansXDecont_Grid"] = null;
					Session["AvsXDec_DataDoc"] = null;
				}

				grDate.SettingsDataSecurity.AllowReadUnlistedFieldsFromClientApi = DefaultBoolean.True;
				IncarcaGrid();


            }
            catch (Exception ex)
            {
				General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                //IncarcaGrid();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }	
		

		
		protected void btnDuplicare_Click(object sender, EventArgs e)
        {
            try
            {				
				List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "IdStare", "USER_NO", "DocumentTypeId" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {                        
                    MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");                
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
					object[] arr = lst[i] as object[];
					
                    if (Convert.ToInt32(arr[2] ?? -99) != Convert.ToInt32(Session["UserId"].ToString()))
                    {
						MessageBox.Show(Dami.TraduCuvant("Documentul a fost realizat de un alt utilizator!"), MessageBox.icoWarning, "");                
						return;
                    }
                    if (Convert.ToInt32(arr[1] ?? -99) != 0)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Nu puteti duplica decat un document ce a fost respins!"), MessageBox.icoWarning, "");
                        return;
                    }

                    string result = VerificaDocumentAdaugat(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(arr[3] ?? -99));
                   
					
					if (result.Length > 0)
					{
						MessageBox.Show(Dami.TraduCuvant(result), MessageBox.icoWarning, "");
						return;
					}
					
					DuplicaDocument(Convert.ToInt32(arr[0] ?? -99));	
					Session["AvansXDecont_Grid"] = null;
					IncarcaGrid();					
                    
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(1, 0);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRespinge_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(2, 0);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
 
                grDate.KeyFieldName = "DocumentId";   

                if (Session["AvansXDecont_Grid"] == null)
                    dt = SelectGrid();
                else
                    dt = Session["AvansXDecont_Grid"] as DataTable;

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["AvansXDecont_Grid"] = dt;
            }
            catch (Exception ex)
            {
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
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Insuficienti parametri");
                        return;
                    }

                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
								List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "IdStare", "USER_NO"  });
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

									
									string msg = AnuleazaDocument(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(arr1[0] ?? "-99"), Convert.ToInt32(Session["User_Marca"].ToString()), "");
									
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
                            DataTable dt = Session["AvansXDecont_Grid"] as DataTable;
                            DataRow[] dr = dt.Select("DocumentId = " + arr[1]);   

							int DocumentId = Convert.ToInt32(dr[0]["DocumentId"].ToString());
							int DocumentTypeId = Convert.ToInt32(dr[0]["DocumentTypeId"].ToString());
							int f10003 = Convert.ToInt32((dr[0]["F10003"] == DBNull.Value ? -99 : dr[0]["F10003"]).ToString());
							/*
							int idRol = ent.IdRol ?? -99;
							 * */
							int pozitie = Convert.ToInt32((dr[0]["Pozitie"] == DBNull.Value ? -99 : dr[0]["Pozitie"]).ToString());
							int idStare = Convert.ToInt32(dr[0]["IdStare"] == DBNull.Value ? "0" : dr[0]["IdStare"].ToString());
							int poateModif = (idStare != 0 && f10003 == Convert.ToInt32(Session["User_Marca"].ToString()) ? 1 : 0);      
							
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
							int userId  =  Convert.ToInt32((dr[0]["USER_NO"] == DBNull.Value ? -99 : dr[0]["USER_NO"]).ToString());

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
									Session["AvsXDec_Pozitie"] = pozitie;
									Session["AvsXDec_PoateModif"] = poateModif;
									Session["AvsXDec_PoateAprobaXRefuzaDoc"] = poateAprobaXRefuzaDoc ? 1 : 0;
									Session["AvsXDec_EsteNou"] = 0;
									//Session["AvsXDec_DocumentTypeId"]launchedPagedFrom = DocumentAvans.LansatDin.Formulare;
									//Session["AvsXDec_DocumentTypeId"]titlu = (barTitlu.Content ?? "").ToString();
									Session["AvsXDec_DocCanBeRefused"] = Convert.ToInt32((dr[0]["CanBeRefused"] == DBNull.Value ? 0 : dr[0]["CanBeRefused"]).ToString());

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
									Session["AvsXDec_PoateAprobaXRefuzaDoc"] = poateAprobaXRefuzaDoc ? 1 : 0;
									Session["AvsXDec_EsteNou"] = 0;
									Session["AvsXDec_SrcDocId"] = SrcDocId;
									Session["AvsXDec_UserId"] = userId;
									Session["AvsXDec_IsBudgetOwnerEdited"] = IsBudgetOwnerForDocument;
									//Session["AvsXDec_DocumentTypeId"]launchedPagedFrom = DocumentAvans.LansatDin.Formulare;
									//Session["AvsXDec_DocumentTypeId"]titlu = (barTitlu.Content ?? "").ToString();
									Session["AvsXDec_DocCanBeRefused"] = Convert.ToInt32((dr[0]["CanBeRefused"] == DBNull.Value ? 0 : dr[0]["CanBeRefused"]).ToString());
									
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
							case "btnRespinge":
								MetodeCereri(2, 1, arr[1].Trim());
								IncarcaGrid();
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
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void MetodeCereri(int tipActiune, int tipMsg, string motivRef = "")
        {
            try
            {
                int nrSel = 0;
                string ids = "";
                /*variabila care determina daca vreau sa anulez lista de avansuri administrative
                 * si nu am completat motivul refuzului*/
                bool rejectListaAvansAdministrativ = false;
                int nrSelAvansAdministrativ = 0;

                //grDate.View.CloseEditor();
                //grDate.ShowLoadingPanel = true;
				
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "DocumentId", "IdStare",  "F10003"});
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    if (tipMsg == 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }				

                for (int i = 0; i < lst.Count(); i++)
                {
					object[] arr = lst[i] as object[];                
					if (((arr[1] ?? "0").ToString() == "1" || (arr[1] ?? "0").ToString() == "2") && Convert.ToInt32((arr[2] ?? "-99").ToString()) != Convert.ToInt32(Session["User_Marca"].ToString()))          
					{                            
						ids += (arr[0] ?? "-99").ToString() + ",";   
						nrSel += 1;                            
					}                    
                }

                if (nrSel == 0)
                {
					MessageBox.Show(Dami.TraduCuvant("Nu exista cereri pentru aceasta actiune!"), MessageBox.icoWarning, "");             
                    return;
                }


                #region completare motiv refuz
                if (tipActiune == 2)
                {        
					string msg = AprobaDocumentAvansXDecont(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, tipActiune, Convert.ToInt32(Session["User_Marca"].ToString()), motivRef);
									
					if (msg != "")
					{
						MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoWarning, "");
					}	                  
              
                }
                #endregion
                else
                #region aprobare document
                {

                    string msg = AprobaDocumentAvansXDecont(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, tipActiune, Convert.ToInt32(Session["User_Marca"].ToString()), "");
                                       
					if (msg != "")
					{
						MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoWarning, "");
					}
					Session["AvansXDecont_Grid"] = null;
					IncarcaGrid();
				}
                #endregion
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string AprobaDocumentAvansXDecont(int idUser, string ids, int total, int actiune, int f10003, string RefuseReason = "")
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere


            string msg = "";
            string msgValidBudgetOwner = string.Empty;
            string msgValid = "";

            try
            {
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
							string sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + DocumentId.ToString();
                            DataTable dtDoc = General.IncarcaDT(sql, null);
							
                            /*LeonardM 26.07.2016
                             * daca documentul a trecut deja din starea aprobat => nu se mai poate anula
                             * */
                            if (Convert.ToInt32(dtDoc.Rows[0]["DocumentStateId"].ToString()) > 3 && actiune == 2)
                            {
                                continue;
                            }
                            /*end LeonardM 26.07.2016*/

							sql = "SELECT * FROM AvsXDec_DocumentStateHistory WHERE DocumentId = " + DocumentId.ToString() + " AND USER_NO = " + idUser + " AND Aprobat IS NULL";
                            DataTable dtIst = General.IncarcaDT(sql, null);
							string lstId = "-99";
							//aflam daca cel care aproba este inlocuitorul 
							if (dtIst == null || dtIst.Rows.Count == 0)
                            {
                                int f10003Inloc = -99;
                                DataTable dtUsr = General.IncarcaDT("SELECT * FROM USERS WHERE F70102 = " + idUser, null); 
                                if (dtUsr != null && dtUsr.Rows.Count > 0) f10003Inloc = Convert.ToInt32(dtUsr.Rows[0]["F10003"] == DBNull.Value ? "-99" : dtUsr.Rows[0]["F10003"].ToString());								
								
                                //obtinem id-urile de utilizator ale persoanelor pe care le inlocuim
                                DataTable dtInloc = General.IncarcaDT("SELECT b.F70102 FROM \"Ptj_Cereri\" a JOIN USERS b ON a.F10003 = b.F10003 WHERE a.\"Inlocuitor\" = " + f10003Inloc 
                                    + " AND a.\"DataInceput\" <= " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + " AND " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + " <= a.\"DataSfarsit\"", null);
                                List<int> lstIdInloc = new List<int>();
                                for (int j = 0; j < dtInloc.Rows.Count; j++)
                                    lstIdInloc.Add(Convert.ToInt32(dtInloc.Rows[j][0] == DBNull.Value ? "-99" : dtInloc.Rows[j][0].ToString()));								
								
                                lstId = "(";
                                for (int j = 0; j < lstIdInloc.Count; j++)
                                {
                                    lstId += lstIdInloc[j];
                                    if (j < lstIdInloc.Count - 1)
                                        lstId += ",";
                                }
                                lstId += ")";
                                if (lstIdInloc.Count > 0)
                                {
									dtIst = General.IncarcaDT("SELECT * FROM \"AvsXDec_DocumentStateHistory\" WHERE \"DocumentId\" = " + DocumentId + " AND USER_NO IN " + lstId, null);                    
                                }							
					
                            }

                            if (dtIst == null || dtIst.Rows.Count == 0) continue;

                            //verificam daca sunt eu cel care trebuie sa aprobe
                            if ((dtIst.Rows[0]["Pozitie"] == DBNull.Value ? -99 : Convert.ToInt32(dtIst.Rows[0]["Pozitie"].ToString())) 
                                != (dtDoc.Rows[0]["Pozitie"] == DBNull.Value ? -99 : Convert.ToInt32(dtDoc.Rows[0]["Pozitie"].ToString())) + 1)
                            {
                                continue;
                            } 							
                  

                            int idStare = 2;
							
                            if (idStare == 2 && dtDoc.Rows[0]["TotalCircuit"].ToString() == dtIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

                            if (actiune == 2)
                                idStare = 0;  		

                            string culoare = "";
                            sql = "SELECT * FROM \"AvsXDec_DictionaryItem\" WHERE \"DictionaryItemId\" = " + idStare.ToString() + " AND DictionaryId = 1";
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";							
							
							
                            sql = "UPDATE \"AvsXDec_Document\" SET \"Pozitie\" = " + dtIst.Rows[0]["Pozitie"].ToString() + ", \"DocumentStateId\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare 
                                + "' WHERE \"DocumentId\" = " + DocumentId.ToString();
                            General.IncarcaDT(sql, null);							
							
							sql = "UPDATE \"AvsXDec_DocumentStateHistory\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"DocumentStateId\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
							+ "', TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
							+ (dtIst.Rows[0]["USER_NO"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
							+ " WHERE \"DocumentId\" = " + DocumentId.ToString() 
							+ " AND Aprobat IS NULL "
							+ " AND USER_NO IN " + (lstId == "-99" ? "(" + idUser.ToString() + ")" : lstId);     
							//+ " AND \"Pozitie\"=" + dtCerIst.Rows[0]["Pozitie"].ToString();                     
							General.IncarcaDT(sql, null);
                   

                            #region verificare completare budgetOwner
                            /*verificam completarea campului de linie de buget doar pentru deconturile avans spre deplasare (2001)
                                * si deconturi cheltuieli (2002)  si doar pentru operatii de aprobare*/
							sql = "SELECT * FROM \"AvsXDec_Document\" WHERE \"DocumentId\" = " + DocumentId.ToString() + " AND DocumentTypeId IN (2001, 2002, 2003)";
                            DataTable dtDocumentValid = General.IncarcaDT(sql, null);								
								
                           /*variabila care e verifica daca documentul este valid pentru a memora starile de pe istoric mai departe*/
                            bool IsValidBudgetOwner = true;
                            if (dtDocumentValid != null && dtDocumentValid.Rows.Count > 0 && actiune == 1)
                            {
                                DataTable q = null;

								sql = "SELECT a.DocumentId, a.DocumentTypeId, a.F10003, a.DocumentDate, a.DocumentStateId, a.Culoare, a.TIME, a.CircuitId, a.USER_NO, CASE WHEN (CASE WHEN bugOwner.IdSuper IS NULL THEN -99 ELSE bugOwner.IdSuper END) = -99 THEN 0 ELSE 1 END AS IsBudgetOwnerForDocument "
								 + " FROM AvsXDec_Document a "
								 + " JOIN AvsXDec_DocumentStateHistory docState ON a.DocumentId = docState.DocumentId "
								 + " LEFT JOIN vwAvsXDec_BudgetOwner bugOwner ON a.DocumentTypeId = (CASE WHEN bugOwner.DocumentTypeId IS NULL THEN -99 ELSE bugOwner.DocumentTypeId END) " 
								 + " AND (CASE WHEN docState.IdSuper IS NULL THEN -99 ELSE docState.IdSuper END) = (CASE WHEN bugOwner.IdSuper IS NULL THEN -99 ELSE bugOwner.IdSuper END) "
								 + " WHERE a.DocumentId = " + DocumentId + " AND docState.Pozitie = " + dtIst.Rows[0]["Pozitie"].ToString();

								q = General.IncarcaDT(sql, null);	
                                if (q != null && q.Rows.Count != 0)
                                {
                                    if (Convert.ToInt32(q.Rows[0]["IsBudgetOwnerForDocument"].ToString()) == 1)
                                    {
										sql = "SELECT * FROM vwAvsXDec_DecDet_DocDecontare WHERE DocumentId = " + DocumentId;
										DataTable dtDec = General.IncarcaDT(sql, null);
										
										if (dtDec != null)
											for (int k = 0; k < dtDec.Rows.Count; k++)
											{
												if (dtDec.Rows[k]["BugetLine"] == null || dtDec.Rows[k]["BugetLine"].ToString().Length <= 0)
													IsValidBudgetOwner = false;
											}
                                    }
                                }
                                if (!IsValidBudgetOwner)
                                    msgValidBudgetOwner = ", " + DocumentId.ToString();
                            }
                            if (!IsValidBudgetOwner)
                                continue;
                            #endregion


                            #region Validare start

                            string corpMesaj = "";
                            bool stop;

                            //ctxNtf.ValidareRegula("AvansXDecont.Document", "grDate", entCer, idUser, f10003, out corpMesaj, out stop);

                            if (corpMesaj != "")
                            {
                                //msgValid += corpMesaj + "\r\n";
                                //if (stop)
                                //{
                                //    continue;
                                //}
                            }

                            #endregion

                            #region schimbare status document in functie de legaturi cu alte documente
                            switch (Convert.ToInt32(dtDoc.Rows[0]["DocumentTypeId"].ToString()))
                            {
                                case 2002:/*Decont avans spre decontare*/
                                case 2001: /*Decont cheltuieli deplasare*/
                                case 2003: /*Decont administrativ*/
                                    /*daca aprobam un decont, trebuie sa punem statusul la avans inchis
                                     * LeonardM 23.08.2016
                                     * cand se aporba un decont administrativ
                                     * avansul legat nu se inchide, deoarece acela se inchide doar automat prin job*/
									sql = "SELECT * FROM AvsXDec_BusinessTransaction WHERE DestDocId = " + dtDoc.Rows[0]["DocumentId"].ToString();
									DataTable dtBT = General.IncarcaDT(sql, null);
                                    if (dtBT != null && dtBT.Rows.Count > 0 && Convert.ToInt32(dtDoc.Rows[0]["DocumentTypeId"].ToString()) != 2003)
                                    {
                                        /* de revizuit LeonardM 22.05.2016
                                         * daca documentul este in stare aprobat si sumele pica fix pe fix,
                                         * atunci documentul de avans trebuie schimbat in status inchis*/
										sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + dtBT.Rows[0]["SrcDocId"].ToString();
										DataTable dtAvans = General.IncarcaDT(sql, null);
										
                                        if (dtAvans != null && dtAvans.Rows.Count > 0)
                                        {
                                            if (idStare == 3 && dtBT.Rows[0]["SrcDocAmount"].ToString() == dtBT.Rows[0]["DestDocAmount"].ToString())
                                            {
                                                #region inchidere document initial
												sql = "SELECT * AvsXDec_DictionaryItem WHERE DictionaryItemId = 8";
												DataTable dtStr = General.IncarcaDT(sql, null);
												
                                                string culAvans = "";
												if (dtStr != null && dtStr.Rows.Count > 0 && dtStr.Rows[0]["Culoare"] != DBNull.Value && dtStr.Rows[0]["Culoare"].ToString().Length > 0)
													culAvans = dtStr.Rows[0]["Culoare"].ToString();
												else
													culAvans = "#FFFFFFFF";										
												
                                                //schimbam statusul
												General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId = 8, Culoare = '" + culAvans + "' WHERE DocumentId = " + dtBT.Rows[0]["SrcDocId"].ToString(), null);
												
                                                //introducem o linie de anulare in AvsXDec_DocumentStateHistory
												int idUrmStateHistory = Dami.NextId("AvsXDec_DocumentStateHistory", 1);
												sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
													+ " VALUES (" + idUrmStateHistory + ", " + dtAvans.Rows[0]["DocumentId"].ToString() + ", " + dtAvans.Rows[0]["CircuitId"].ToString() + ", 8, 22, '" + culAvans + "', 1, GETDATE(), " + idUser + ", GETDATE(), 0)";
												General.ExecutaNonQuery(sql, null);											
                

                                                #region  Notificare strat
                                                //ctxNtf.TrimiteNotificare("Absente.Aprobare", "grDate", entIstAvans, idUser, f10003);
                                                #endregion

                                                #endregion
                                            }
                                        }
                                    }
									
									sql = "SELECT * AvsXDec_Decont WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString();
									DataTable dtDecont2001Update = General.IncarcaDT(sql, null);
									
                                    if (actiune == 2 && dtDecont2001Update != null && dtDecont2001Update.Rows.Count > 0)
                                    {
										General.ExecutaNonQuery("UPDATE AvsXDec_Decont SET RefuseReason = '" + RefuseReason + "' WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString(), null);                                  
                                    }

                                    #region decont Administrativ
                                    if (Convert.ToInt32(dtDoc.Rows[0]["DocumentTypeId"].ToString()) == 2003)
                                    {
                                        #region aporbare decont administrativ
                                        /*in momentul in care se aproba decontul, atunci se trece automat in starea acordat*/
                                        if (Convert.ToInt32(dtDoc.Rows[0]["DocumentStateId"].ToString()) == 3)
                                        {
                                            if (dtIst != null && dtIst.Rows.Count > 0)
                                            {
                                                int decAdmStateIdAcordat = 5;
												string culoareDecAdmAcordat = "";
												sql = "SELECT * FROM AvsXDec_DictionaryItem WHERE DictionaryItemId = " + decAdmStateIdAcordat;
												DataTable dtCulDec = General.IncarcaDT(sql, null);
												if (dtCulDec != null && dtCulDec.Rows.Count > 0 && dtCulDec.Rows[0]["Culoare"] != DBNull.Value && dtCulDec.Rows[0]["Culoare"].ToString().Length > 0)
													culoareDecAdmAcordat = dtCulDec.Rows[0]["Culoare"].ToString();
												else
													culoareDecAdmAcordat = "#FFFFFFFF";													
												
                                                int documentStateIdHistId = Dami.NextId("AvsXDec_DocumentStateHistory", 1);

                                                #region salvare decont
												sql = "SELECT * FROM AvsXDec_Decont WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString();
												DataTable dtAdm = General.IncarcaDT(sql, null);
                                                if (dtAdm != null && dtAdm.Rows.Count > 0)
                                                {
													sql = "UPDATE AvsXDec_Decont SET UnconfRestAmount = " + dtAdm.Rows[0]["TotalAmount"].ToString() + ", ComissionValueFinance = 0, "
														+ " PaymentValueFinance = " + dtAdm.Rows[0]["TotalAmount"].ToString() + " WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString();
													General.ExecutaNonQuery(sql, null);													
                                                }
												General.ExecutaNonQuery("UPDATE AvsXDec_Document SET DocumentStateId = " + decAdmStateIdAcordat + " WHERE DocumentId = " + DocumentId.ToString(), null);													
                                                #endregion

                                                #region salvare istoric
												sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, IdSuper, Pozitie, USER_NO, TIME, Inlocuitor, IdUserInlocuitor, Aprobat, DataAprobare, DocumentStateId, Culoare)"
													+ " VALUES (" + documentStateIdHistId + ", " + dtDoc.Rows[0]["DocumentId"].ToString() + ", " + dtDoc.Rows[0]["CircuitId"].ToString() + ", 22, 22, " + dtIst.Rows[0]["USER_NO"].ToString() + ", " 
													+ " GETDATE(), " + dtIst.Rows[0]["Inlocuitor"].ToString() + ", " + dtIst.Rows[0]["IdUserInlocuitor"].ToString() + ", 1, GETDATE(), " + decAdmStateIdAcordat + ", '" + culoareDecAdmAcordat + "')";
												General.ExecutaNonQuery(sql, null);	
                                                #endregion
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    break;
                                case 1004:/*Lista Avansuri Administrative*/
                                    #region generare avansuri individuale
                                    /*generam avansuri administrative individuale in cazul in care lista
                                     * de avansuri administrative a fost aprobata*/
									 
									 
                                    //AvsXDec_Avans entListAvAdministrativ = ctx.AvsXDec_Avans.Where(p => p.DocumentId == entCer.DocumentId).FirstOrDefault();
                                    //if (entCer.DocumentStateId == 3 && entListAvAdministrativ != null)
                                    //{
                                    //    foreach (AvsXDec_AvansDetail entAvAdm in ctx.AvsXDec_AvansDetail.Where(p => p.DocumentId == entCer.DocumentId))
                                    //    {
                                    //        int documentId = GetNextId("AvsXDec_Document", 1);
                                    //        int documentStateId = 3;
                                    //        string culoareAvAdm = ctx.AvsXDec_DictionaryItem.Where(p => p.DictionaryItemId == documentStateId).FirstOrDefault().Culoare ?? "#FFFFFFFF";

                                    //        int idUserDocument = 0;
                                    //        var entUser = ctx.USERS.Where(p => p.F10003 == entAvAdm.F10003).FirstOrDefault();
                                    //        if (entUser != null)
                                    //            idUserDocument = entUser.F70102;

                                            #region salvare AvsXDec_Document
                                            /*salvare header*/
                                    //        AvsXDec_Document entAvans = new AvsXDec_Document();
                                    //        entAvans.DocumentId = documentId;
                                    //        entAvans.DocumentTypeId = 1003;
                                    //        entAvans.DocumentStateId = documentStateId;
                                    //        entAvans.Culoare = culoare;
                                    //        entAvans.F10003 = Convert.ToInt32(entAvAdm.F10003 ?? -99);
                                    //        entAvans.DocumentDate = DateTime.Now;
                                    //        entAvans.CircuitId = entCer.CircuitId;
                                    //        entAvans.TotalCircuit = entCer.TotalCircuit;
                                    //        entAvans.Pozitie = entCer.Pozitie;
                                    //        entAvans.USER_NO = idUserDocument;
                                    //        entAvans.TIME = DateTime.Now;
                                    //        ctx.AvsXDec_Document.AddObject(entAvans);

                                            #endregion

                                            #region salvare AvsXDec_DocumentStateHistory
                                            /*salvare istoric*/
                                    //        foreach (AvsXDec_DocumentStateHistory entDocHist in ctx.AvsXDec_DocumentStateHistory.Where(p => p.DocumentId == entCer.DocumentId))
                                    //        {
                                    //            int documentIdIstoric = GetNextId("AvsXDec_DocumentStateHistory", 1);
                                    //            AvsXDec_DocumentStateHistory entDocStateHist = new AvsXDec_DocumentStateHistory();
                                    //            entDocStateHist.Id = documentIdIstoric;
                                    //            entDocStateHist.DocumentId = documentId;
                                    //            entDocStateHist.CircuitId = entDocHist.CircuitId;
                                    //            entDocStateHist.IdSuper = entDocHist.IdSuper;
                                    //            entDocStateHist.Pozitie = entDocHist.Pozitie;
                                    //            entDocStateHist.USER_NO = entDocHist.Pozitie == 1 ? idUserDocument : entDocHist.USER_NO;
                                    //            entDocStateHist.TIME = DateTime.Now;
                                    //            entDocStateHist.Inlocuitor = null;
                                    //            entDocStateHist.IdUserInlocuitor = null;
                                    //            entDocStateHist.Aprobat = entDocHist.Aprobat;
                                    //            entDocStateHist.DataAprobare = entDocHist.DataAprobare;
                                    //            entDocStateHist.DocumentStateId = entDocHist.DocumentStateId;
                                    //            entDocStateHist.Culoare = entDocHist.Culoare;
                                    //            ctx.AvsXDec_DocumentStateHistory.AddObject(entDocStateHist);
                                    //        }

                                            #endregion

                                            #region salvare AvsXDec_Avans
                                            /*salvare date in avans*/
                                            #region determinare modalitate plata
                                            /*LeonardM 22.08.2016
                                             * modalitatea de plata se preia de la nivel de configurari din view-ul vwAvsXDec_ConfigCurrencyXPay
                                             * unde se poate configura ce tip de modalitate plata exista pentru fiecare utilizator in parte
                                             * per tip de document si per tip de moneda*/
                                    //        int IdModalitatePlata = -99;
                                    //        IEnumerable<metaAvsXDec_ConfigCurrencyXPay> lstConfigxCurrency = from a in this.ObjectContext.vwAvsXDec_ConfigCurrencyXPay
                                    //                                                                         where a.F10003 == entAvAdm.F10003 && a.DocumentTypeId == 1003 && a.CurrencyId == entAvAdm.CurrencyId
                                    //                                                                         select new metaAvsXDec_ConfigCurrencyXPay
                                    //                                                                         {
                                    //                                                                             DocumentTypeId = a.DocumentTypeId ?? -99,
                                    //                                                                             CurrencyId = a.CurrencyId ?? -99,


                                    //                                                                             F10003 = a.F10003,

                                    //                                                                             PaymentTypeId = a.PaymentTypeId ?? -99
                                    //                                                                         };
                                    //        if (lstConfigxCurrency.Count() != 0)
                                    //        {
                                    //            IdModalitatePlata = lstConfigxCurrency.FirstOrDefault().PaymentTypeId;
                                    //        }
                                    //        else
                                    //        {
                                    //            AvsXDec_DictionaryItem entPaymentType = new AvsXDec_DictionaryItem();
                                    //            var lstPaymentType = ctx.AvsXDec_DictionaryItem.Where(p => p.DictionaryItemName.ToUpper() == "TRANSFER BANCAR");
                                    //            if (lstPaymentType.Count() != 0)
                                    //            {
                                    //                entPaymentType = lstPaymentType.FirstOrDefault();
                                    //                IdModalitatePlata = entPaymentType.DictionaryItemId;
                                    //            }
                                    //        }
                                            #endregion
                                    //        AvsXDec_AccountingPeriod entAccPeriod = ctx.AvsXDec_AccountingPeriod.Where(p => p.PeriodOfAccountId == entListAvAdministrativ.PeriodOfAccountId).FirstOrDefault();
                                    //        AvsXDec_Avans entAvansAdm = new AvsXDec_Avans();
                                    //        entAvansAdm.DocumentId = documentId;
                                    //        entAvansAdm.CurrencyId = entAvAdm.CurrencyId;
                                    //        entAvansAdm.TotalPayment = 0;
                                    //        entAvansAdm.StartDate = ((entAccPeriod != null) ? entAccPeriod.StartDate : new DateTime());
                                    //        entAvansAdm.EndDate = ((entAccPeriod != null) ? entAccPeriod.EndDate : new DateTime());
                                    //        entAvansAdm.PaymentTypeId = IdModalitatePlata;
                                    //        entAvansAdm.TotalAmount = entAvAdm.Amount - entAvAdm.SoldAmount;
                                    //        entAvansAdm.USER_NO = idUserDocument;
                                    //        entAvansAdm.TIME = DateTime.Now;
                                    //        entAvansAdm.EstimatedAmount = entAvAdm.Amount - entAvAdm.SoldAmount;
                                    //        entAvansAdm.UnconfRestAmount = entAvAdm.Amount - entAvAdm.SoldAmount;
                                    //        entAvansAdm.DueDate = DateTime.Now;
                                    //        entAvansAdm.AccPeriodId = entListAvAdministrativ.AccPeriodId;
                                    //        entAvansAdm.PeriodOfAccountId = entListAvAdministrativ.PeriodOfAccountId;
                                    //        ctx.AvsXDec_Avans.AddObject(entAvansAdm);
                                            #endregion

                                            #region salvare legatura intre avansuri
                                    //        AvsXDec_BusinessTransaction entBusinessTransaction = new AvsXDec_BusinessTransaction();
                                    //        entBusinessTransaction.TransactionId = GetNextId("AvsXDec_BusinessTransaction", 1);
                                    //        entBusinessTransaction.SrcDocId = entListAvAdministrativ.DocumentId;
                                    //        entBusinessTransaction.SrcDocAmount = entAvAdm.Amount - entAvAdm.SoldAmount;
                                    //        entBusinessTransaction.DestDocId = documentId;
                                    //        entBusinessTransaction.DestDocAmount = entAvAdm.Amount - entAvAdm.SoldAmount;
                                    //        entBusinessTransaction.USER_NO = idUserDocument;
                                    //        entBusinessTransaction.TIME = DateTime.Now;
                                    //        ctx.AvsXDec_BusinessTransaction.AddObject(entBusinessTransaction);
                                            #endregion
                                    //    }
                                    //}
                                    //if (actiune == 2 && entListAvAdministrativ != null)
                                    //{
                                    //    entListAvAdministrativ.RefuseReason = RefuseReason;
                                    //}
                                    #endregion
                                    break;
                                case 1001:/*Avans spre deplasare*/
                                case 1002:/*Avans spre decontare*/
									sql = "SELECT  * FROM AvsXDec_Avans WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString();
									DataTable dtAvansUpdate = General.IncarcaDT(sql, null);
									
                                    if (actiune == 2 && dtAvansUpdate != null && dtAvansUpdate.Rows.Count > 0)
                                    {
										General.ExecutaNonQuery("UPDATE AvsXDec_Avans SET RefuseReason = '" + RefuseReason + "' WHERE DocumentId = " + dtDoc.Rows[0]["DocumentId"].ToString(), null);	                           
                                    }
                                    break;
                            }

                            #endregion
                            

                            #region  Notificare strat

                            //ctxNtf.TrimiteNotificare("AvansXDecont.Document", "grDate", entCer, idUser, f10003);

                            #endregion

                            nr++;

                        }
                    }
                }

                if (nr > 0)
                {
                    if (actiune == 1)
                        msg = "S-au aprobat " + nr.ToString() + " cereri din " + total + " !";
                    else
                        msg = "S-au respins " + nr.ToString() + " cereri din " + total + " !";

                    if (msgValid != "") msg = msg + System.Environment.NewLine + msgValid;
                }
                else
                {
                    if (msgValid != "")
                        msg = msgValid;
                    else
                        msg = "Nu exista cereri pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            if (!string.IsNullOrEmpty(msgValidBudgetOwner))
                msg = msg + System.Environment.NewLine + " Trebuie completata linia de buget pentru documentele: " + msgValidBudgetOwner.Substring(2) + "!";

            return msg;

        }
		
        public string VerificaDocumentAdaugat(int idUser, int DocumentTypeId)
        {
            /*metoda ce verifica daca mai exista un document de acelasi tip deschis
             * filtre:
             * sa fie acelasi tip de document
             * sa fie generat de acelasi utilizator
             * sa nu fie anulat/respins
             * sa nu fie in starea inchis
             * */
            string result = "";
            try
            {            
				string sql = "SELECT a.* FROM AvsXDec_Document a WHERE a.DocumentTypeId = " + DocumentTypeId + " AND a.USER_NO = " + idUser + " AND a.DocumentStateId >= 1 AND a.DocumentStateId != 8";		
				DataTable q = General.IncarcaDT(sql, null);	

                if (q != null && q.Rows.Count != 0)
                {
                    result = "Mai exista un document de acelasi tip neinchis!";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                result = "";
            }
            return result;
        }		
		
        public int AdaugaDocument(int idUser, int DocumentTypeId)
        {
            //daca returneaza -1 atunci nu se poate adauga, deoarece a fost initiata deja o cerere pentru acest tip de post.

            int idUrm = -99;
            int idUrmIstoric = -99;
            int F10003 = -99;

            try
            {
				string sql = "SELECT * FROM USERS WHERE F70102 = " + idUser;
				DataTable dtUsr = General.IncarcaDT(sql, null);
				if (dtUsr != null & dtUsr.Rows.Count > 0)
					F10003 = Convert.ToInt32((dtUsr.Rows[0]["F10003"] == DBNull.Value ? -99 : dtUsr.Rows[0]["F10003"]).ToString());
                idUrm = Dami.NextId("AvsXDec_Document", 1);

				sql = "SELECT * FROM AvsXDec_Circuit WHERE DocumentTypeId = " +  DocumentTypeId;
				DataTable dtCir = General.IncarcaDT(sql, null);
				
                if (dtCir == null || dtCir.Rows.Count == 0 || dtCir.Rows[0]["CircuitId"] == null) return -123;
                int idCircuit = Convert.ToInt32(dtCir.Rows[0]["CircuitId"]);
				
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

                //aflam totalul de users din circuit
                for (int i = 1; i <= 20; i++)
                {
                    if (dtCir.Rows[0]["Super" + i] != DBNull.Value)
                    {
                        int idSuper = Convert.ToInt32(dtCir.Rows[0]["Super" + i].ToString());
                        if (Convert.ToInt32(idSuper) != -99)
                        {
                            //ne asiguram ca exista user pentru supervizorul din circuit
                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                int idSpr = Convert.ToInt32(idSuper);
                                sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + F10003;
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
				#endregion

				//adaugam headerul
				#region salvare header
				sql = "INSERT INTO AvsXDec_Document (DocumentId, DocumentTypeId, DocumentStateId, Culoare, F10003, DocumentDate, CircuitId, TotalCircuit, Pozitie, USER_NO, TIME) "
					+ " VALUES (" + idUrm + ", " + DocumentTypeId + ", " + idStare + ", '" + culoare + "', " + F10003 + ", GETDATE(), " + idCircuit + ", " + total + ", 1, " + idUser + ", GETDATE())";
				General.ExecutaNonQuery(sql, null);
                #endregion   
             

                //adaugam datele document avans
                #region salvare avans
                switch (DocumentTypeId)
                {
                    case 1001:
                    case 1002:
						sql = "INSERT INTO AvsXDec_Avans (DocumentId, TotalAmount) VALUES (" + idUrm + ", 0)";
						General.ExecutaNonQuery(sql, null);  
                        break;
                    case 2001:
                    case 2002:
						sql = "INSERT INTO AvsXDec_Decont (DocumentId) VALUES (" + idUrm + ")";
						General.ExecutaNonQuery(sql, null); 
                        break;
                    case 2003:					
                        //AvsXDec_Decont entDocDecontAdministrativ = new AvsXDec_Decont();
                        //entDocDecontAdministrativ.DocumentId = idUrm;
                        //ctx.AvsXDec_Decont.AddObject(entDocDecontAdministrativ);
                        break;
                }
                #endregion
    
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return idUrm;
        }	

		public void DuplicaDocument(int documentId)
        {
            try
            {
				string sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + documentId;
				DataTable dtDocSursa = General.IncarcaDT(sql, null);					

				sql = "SELECT * FROM AvsXDec_BusinessTransaction WHERE DestDocId = " + documentId;
				DataTable dtBT = General.IncarcaDT(sql, null);				

                if (dtDocSursa == null || dtDocSursa.Rows.Count == 0)
                    return;

                int newDocumentId = Dami.NextId("AvsXDec_Document", 1);

                #region salvare document
				string culoare = "";
				sql = "SELECT * FROM \"AvsXDec_DictionaryItem\" WHERE \"DictionaryItemId\" = 1";
				DataTable dtCul = General.IncarcaDT(sql, null);
				if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
					culoare = dtCul.Rows[0]["Culoare"].ToString();
				else
					culoare = "#FFFFFFFF";					
				
				sql = "INSERT INTO AvsXDec_Document (DocumentId, DocumentTypeId, DocumentStateId, F10003, DocumentDate, CircuitId, TotalCircuit, Culoare, Pozitie, USER_NO, TIME) "
					+ " VALUES (" + newDocumentId + ", " + dtDocSursa.Rows[0]["DocumentTypeId"].ToString() + ", 1, " + dtDocSursa.Rows[0]["F10003"].ToString() + ", " 
					+ dtDocSursa.Rows[0]["DocumentDate"].ToString() + ", " + dtDocSursa.Rows[0]["CircuitId"].ToString() + ", " + dtDocSursa.Rows[0]["TotalCircuit"].ToString() + ", '" 
					+ culoare + "',1, " + dtDocSursa.Rows[0]["USER_NO"].ToString() + ", GETDATE())";
				General.ExecutaNonQuery(sql, null);	 
                #endregion

                switch (Convert.ToInt32(dtDocSursa.Rows[0]["DocumentTypeId"].ToString()))
                {
                    case 1001:/*Avans spre deplasare*/
                    case 1002:/*Avans spre decontare*/
                        #region salvare avans
						sql = "SELECT * FROM AvsXDec_Avans WHERE DocumentId = " + documentId;
						DataTable dtAvSursa = General.IncarcaDT(sql, null);
						
                        if (dtAvSursa != null && dtAvSursa.Rows.Count > 0)
                        {	
							string sqlQuiz = "";
							if (dtAvSursa != null && dtAvSursa.Rows.Count > 0)
							{
								sqlQuiz = "INSERT INTO \"AvsXDec_Avans\" (";
								for (int i = 0; i < dtAvSursa.Columns.Count; i++)
								{
									if (!dtAvSursa.Columns[i].AutoIncrement)
									{
										sqlQuiz += "\"" + dtAvSursa.Columns[i].ColumnName + "\"";
										if (i < dtAvSursa.Columns.Count - 1)
											sqlQuiz += ", ";
									}
								}
								sqlQuiz += ") VALUES (";
								for (int i = 0; i < dtAvSursa.Columns.Count; i++)
								{
									if (!dtAvSursa.Columns[i].AutoIncrement)
									{
										switch (dtAvSursa.Columns[i].ColumnName)
										{
											case "DocumentId":
												sqlQuiz += newDocumentId;
												break;
											case "RefuseReason":
												sqlQuiz += "NULL";
												break;									
											case "TIME":
												sqlQuiz += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
												break;
											default:
												if (dtAvSursa.Rows[0][dtAvSursa.Columns[i].ColumnName] == null || dtAvSursa.Rows[0][dtAvSursa.Columns[i].ColumnName].ToString().Length <= 0)
													sqlQuiz += "NULL";
												else
												{
													switch (dtAvSursa.Columns[i].DataType.ToString())
													{
														case "System.String":
															sqlQuiz += "'" + dtAvSursa.Rows[0][dtAvSursa.Columns[i].ColumnName].ToString() + "'";
															break;
														case "System.DateTime":
															DateTime dt = Convert.ToDateTime(General.Nz(dtAvSursa.Rows[0][dtAvSursa.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
															sqlQuiz += General.ToDataUniv(dt);
															break;
														default:
															sqlQuiz += dtAvSursa.Rows[0][dtAvSursa.Columns[i].ColumnName].ToString();
															break;
													}
												}
												break;
										}
										if (i < dtAvSursa.Columns.Count - 1)
											sqlQuiz += ", ";
									}
								}

								sqlQuiz += ")";
							}	
							General.ExecutaNonQuery(sqlQuiz, null);		
                        }
                        #endregion

                        #region salvare detaliu avans
						sql = "SELECT * FROM AvsXDec_AvansDetail WHERE DocumentId = " + documentId;
						DataTable dtAvSursaDetail = General.IncarcaDT(sql, null);
						
                        for (int k = 0; k < dtAvSursaDetail.Rows.Count; k++)
                        {
							string sqlQuiz = "";
							int documentDetailId = -99;
							if (dtAvSursaDetail != null && dtAvSursaDetail.Rows.Count > 0)
							{
								sqlQuiz = "INSERT INTO \"AvsXDec_AvansDetail\" (";
								for (int i = 0; i < dtAvSursaDetail.Columns.Count; i++)
								{
									if (!dtAvSursaDetail.Columns[i].AutoIncrement)
									{
										sqlQuiz += "\"" + dtAvSursaDetail.Columns[i].ColumnName + "\"";
										if (i < dtAvSursaDetail.Columns.Count - 1)
											sqlQuiz += ", ";
									}
								}
								sqlQuiz += ") VALUES (";
								for (int i = 0; i < dtAvSursaDetail.Columns.Count; i++)
								{
									if (!dtAvSursaDetail.Columns[i].AutoIncrement)
									{
										switch (dtAvSursaDetail.Columns[i].ColumnName)
										{
											case "DocumentId":
												sqlQuiz += newDocumentId;
												break;																	
											case "DocumentDetailId":
												documentDetailId = Dami.NextId("AvsXDec_AvansDetail", 1);
												sqlQuiz += documentDetailId;
												break;
											default:
												if (dtAvSursaDetail.Rows[k][dtAvSursaDetail.Columns[i].ColumnName] == null || dtAvSursaDetail.Rows[k][dtAvSursaDetail.Columns[i].ColumnName].ToString().Length <= 0)
													sqlQuiz += "NULL";
												else
												{
													switch (dtAvSursaDetail.Columns[i].DataType.ToString())
													{
														case "System.String":
															sqlQuiz += "'" + dtAvSursaDetail.Rows[k][dtAvSursaDetail.Columns[i].ColumnName].ToString() + "'";
															break;
														case "System.DateTime":
															DateTime dt = Convert.ToDateTime(General.Nz(dtAvSursaDetail.Rows[k][dtAvSursaDetail.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
															sqlQuiz += General.ToDataUniv(dt);
															break;
														default:
															sqlQuiz += dtAvSursaDetail.Rows[k][dtAvSursaDetail.Columns[i].ColumnName].ToString();
															break;
													}
												}
												break;
										}
										if (i < dtAvSursaDetail.Columns.Count - 1)
											sqlQuiz += ", ";
									}
								}

								sqlQuiz += ")";
							}							
							General.ExecutaNonQuery(sqlQuiz, null);			
 

                            #region salvare atasamente
							sql = "SELECT * FROM AvsXDec_relUploadDocumente WHERE DocumentId = " + documentId + " AND DocumentDetailId = " + dtAvSursaDetail.Rows[k]["DocumentDetailId"].ToString();
							DataTable dtAvAtas = General.IncarcaDT(sql, null);
							
                            for (int j = 0; j < dtAvAtas.Rows.Count; j++)
                            {
								
								string sqlAtas = "";	
								int idDoc = -99;								
								if (dtAvAtas != null && dtAvAtas.Rows.Count > 0)
								{
									sqlAtas = "INSERT INTO \"AvsXDec_relUploadDocumente\" (";
									for (int i = 0; i < dtAvAtas.Columns.Count; i++)
									{
										if (!dtAvAtas.Columns[i].AutoIncrement)
										{
											sqlAtas += "\"" + dtAvAtas.Columns[i].ColumnName + "\"";
											if (i < dtAvAtas.Columns.Count - 1)
												sqlAtas += ", ";
										}
									}
									sqlAtas += ") VALUES (";
									for (int i = 0; i < dtAvAtas.Columns.Count; i++)
									{
										if (!dtAvAtas.Columns[i].AutoIncrement)
										{
											switch (dtAvAtas.Columns[i].ColumnName)
											{
												case "IdDocument":
													idDoc = Dami.NextId("AvsXDec_relUploadDocumente", 1);
													sqlAtas += idDoc;
													break;
												case "DocumentId":
													sqlAtas += newDocumentId;
													break;																	
												case "DocumentDetailId":													
													sqlAtas += documentDetailId;
													break;
												default:
													if (dtAvAtas.Rows[j][dtAvAtas.Columns[i].ColumnName] == null || dtAvAtas.Rows[j][dtAvAtas.Columns[i].ColumnName].ToString().Length <= 0)
														sqlAtas += "NULL";
													else
													{
														switch (dtAvAtas.Columns[i].DataType.ToString())
														{
															case "System.String":
																sqlAtas += "'" + dtAvAtas.Rows[j][dtAvAtas.Columns[i].ColumnName].ToString() + "'";
																break;
															case "System.DateTime":
																DateTime dt = Convert.ToDateTime(General.Nz(dtAvAtas.Rows[j][dtAvAtas.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
																sqlAtas += General.ToDataUniv(dt);
																break;
															default:
																sqlAtas += dtAvAtas.Rows[j][dtAvAtas.Columns[i].ColumnName].ToString();
																break;
														}
													}
													break;
											}
											if (i < dtAvAtas.Columns.Count - 1)
												sqlAtas += ", ";
										}
									}

									sqlAtas += ")";
								}							
								General.ExecutaNonQuery(sqlAtas, null);									
                       
								sql = "SELECT * FROM tblFisiere WHERE Tabela = 'AvsXDec_relUploadDocumente' AND Id = " + dtAvAtas.Rows[j]["IdDocument"].ToString();
								DataTable dtFisiere = General.IncarcaDT(sql, null);					   

                                for (int l = 0; l < dtFisiere.Rows.Count; l++)
                                {									
									string sqlFis = "";								
									if (dtFisiere != null && dtFisiere.Rows.Count > 0)
									{
										sqlFis = "INSERT INTO \"tblFisiere\" (";
										for (int i = 0; i < dtFisiere.Columns.Count; i++)
										{
											if (!dtFisiere.Columns[i].AutoIncrement)
											{
												sqlFis += "\"" + dtFisiere.Columns[i].ColumnName + "\"";
												if (i < dtFisiere.Columns.Count - 1)
													sqlFis += ", ";
											}
										}
										sqlFis += ") VALUES (";
										for (int i = 0; i < dtFisiere.Columns.Count; i++)
										{
											if (!dtFisiere.Columns[i].AutoIncrement)
											{
												switch (dtFisiere.Columns[i].ColumnName)
												{
													case "Id":
														sqlFis += idDoc;
														break;
													default:
														if (dtFisiere.Rows[l][dtFisiere.Columns[i].ColumnName] == null || dtFisiere.Rows[l][dtFisiere.Columns[i].ColumnName].ToString().Length <= 0)
															sqlFis += "NULL";
														else
														{
															switch (dtFisiere.Columns[i].DataType.ToString())
															{
																case "System.String":
																	sqlFis += "'" + dtFisiere.Rows[l][dtFisiere.Columns[i].ColumnName].ToString() + "'";
																	break;
																case "System.DateTime":
																	DateTime dt = Convert.ToDateTime(General.Nz(dtFisiere.Rows[l][dtFisiere.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
																	sqlFis += General.ToDataUniv(dt);
																	break;
																default:
																	sqlFis += dtFisiere.Rows[l][dtFisiere.Columns[i].ColumnName].ToString();
																	break;
															}
														}
														break;
												}
												if (i < dtFisiere.Columns.Count - 1)
													sqlFis += ", ";
											}
										}

										sqlFis += ")";
									}							
									General.ExecutaNonQuery(sqlFis, null);	   
                                }
                            }
                            #endregion                           
                        }
                        #endregion

                        break;
                    case 2001:/*Decont cheltuieli deplasare*/
                    case 2002:/*Decont cheltuieli*/
                    case 2003:/*Decont administrativ*/
                        #region salvare decont
						sql = "SELECT * FROM AvsXDec_Decont WHERE DocumentId = " + documentId;
						DataTable dtDecSursa = General.IncarcaDT(sql, null);
						
                        if (dtDecSursa != null && dtDecSursa.Rows.Count > 0)
                        {	
							string sqlDec = "";
							if (dtDecSursa != null && dtDecSursa.Rows.Count > 0)
							{
								sqlDec = "INSERT INTO \"AvsXDec_Decont\" (";
								for (int i = 0; i < dtDecSursa.Columns.Count; i++)
								{
									if (!dtDecSursa.Columns[i].AutoIncrement)
									{
										sqlDec += "\"" + dtDecSursa.Columns[i].ColumnName + "\"";
										if (i < dtDecSursa.Columns.Count - 1)
											sqlDec += ", ";
									}
								}
								sqlDec += ") VALUES (";
								for (int i = 0; i < dtDecSursa.Columns.Count; i++)
								{
									if (!dtDecSursa.Columns[i].AutoIncrement)
									{
										switch (dtDecSursa.Columns[i].ColumnName)
										{
											case "DocumentId":
												sqlDec += newDocumentId;
												break;
											case "RefuseReason":
												sqlDec += "NULL";
												break;									
											case "TIME":
												sqlDec += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
												break;
											default:
												if (dtDecSursa.Rows[0][dtDecSursa.Columns[i].ColumnName] == null || dtDecSursa.Rows[0][dtDecSursa.Columns[i].ColumnName].ToString().Length <= 0)
													sqlDec += "NULL";
												else
												{
													switch (dtDecSursa.Columns[i].DataType.ToString())
													{
														case "System.String":
															sqlDec += "'" + dtDecSursa.Rows[0][dtDecSursa.Columns[i].ColumnName].ToString() + "'";
															break;
														case "System.DateTime":
															DateTime dt = Convert.ToDateTime(General.Nz(dtDecSursa.Rows[0][dtDecSursa.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
															sqlDec += General.ToDataUniv(dt);
															break;
														default:
															sqlDec += dtDecSursa.Rows[0][dtDecSursa.Columns[i].ColumnName].ToString();
															break;
													}
												}
												break;
										}
										if (i < dtDecSursa.Columns.Count - 1)
											sqlDec += ", ";
									}
								}

								sqlDec += ")";
							}	
							General.ExecutaNonQuery(sqlDec, null);		
                        }	
                        #endregion

                        #region salvare detaliu decont
						sql = "SELECT * FROM AvsXDec_DecontDetail WHERE DocumentId = " + documentId;
						DataTable dtDecSursaDetail = General.IncarcaDT(sql, null);
						
                        for (int k = 0; k < dtDecSursaDetail.Rows.Count; k++)
                        {
							string sqlDecDet = "";
							int documentDetailId = -99;
							if (dtDecSursaDetail != null && dtDecSursaDetail.Rows.Count > 0)
							{
								sqlDecDet = "INSERT INTO \"AvsXDec_DecontDetail\" (";
								for (int i = 0; i < dtDecSursaDetail.Columns.Count; i++)
								{
									if (!dtDecSursaDetail.Columns[i].AutoIncrement)
									{
										sqlDecDet += "\"" + dtDecSursaDetail.Columns[i].ColumnName + "\"";
										if (i < dtDecSursaDetail.Columns.Count - 1)
											sqlDecDet += ", ";
									}
								}
								sqlDecDet += ") VALUES (";
								for (int i = 0; i < dtDecSursaDetail.Columns.Count; i++)
								{
									if (!dtDecSursaDetail.Columns[i].AutoIncrement)
									{
										switch (dtDecSursaDetail.Columns[i].ColumnName)
										{
											case "DocumentId":
												sqlDecDet += newDocumentId;
												break;																	
											case "DocumentDetailId":
												documentDetailId = Dami.NextId("AvsXDec_DecontDetail", 1);
												sqlDecDet += documentDetailId;
												break;
											default:
												if (dtDecSursaDetail.Rows[k][dtDecSursaDetail.Columns[i].ColumnName] == null || dtDecSursaDetail.Rows[k][dtDecSursaDetail.Columns[i].ColumnName].ToString().Length <= 0)
													sqlDecDet += "NULL";
												else
												{
													switch (dtDecSursaDetail.Columns[i].DataType.ToString())
													{
														case "System.String":
															sqlDecDet += "'" + dtDecSursaDetail.Rows[k][dtDecSursaDetail.Columns[i].ColumnName].ToString() + "'";
															break;
														case "System.DateTime":
															DateTime dt = Convert.ToDateTime(General.Nz(dtDecSursaDetail.Rows[k][dtDecSursaDetail.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
															sqlDecDet += General.ToDataUniv(dt);
															break;
														default:
															sqlDecDet += dtDecSursaDetail.Rows[k][dtDecSursaDetail.Columns[i].ColumnName].ToString();
															break;
													}
												}
												break;
										}
										if (i < dtDecSursaDetail.Columns.Count - 1)
											sqlDecDet += ", ";
									}
								}

								sqlDecDet += ")";
							}							
							General.ExecutaNonQuery(sqlDecDet, null);			
 

                            #region salvare atasamente
							sql = "SELECT * FROM AvsXDec_relUploadDocumente WHERE DocumentId = " + documentId + " AND DocumentDetailId = " + dtDecSursaDetail.Rows[k]["DocumentDetailId"].ToString();
							DataTable dtDecAtas = General.IncarcaDT(sql, null);
							
                            for (int j = 0; j < dtDecAtas.Rows.Count; j++)
                            {
								
								string sqlDecAtas = "";	
								int idDoc = -99;								
								if (dtDecAtas != null && dtDecAtas.Rows.Count > 0)
								{
									sqlDecAtas = "INSERT INTO \"AvsXDec_relUploadDocumente\" (";
									for (int i = 0; i < dtDecAtas.Columns.Count; i++)
									{
										if (!dtDecAtas.Columns[i].AutoIncrement)
										{
											sqlDecAtas += "\"" + dtDecAtas.Columns[i].ColumnName + "\"";
											if (i < dtDecAtas.Columns.Count - 1)
												sqlDecAtas += ", ";
										}
									}
									sqlDecAtas += ") VALUES (";
									for (int i = 0; i < dtDecAtas.Columns.Count; i++)
									{
										if (!dtDecAtas.Columns[i].AutoIncrement)
										{
											switch (dtDecAtas.Columns[i].ColumnName)
											{
												case "IdDocument":
													idDoc = Dami.NextId("AvsXDec_relUploadDocumente", 1);
													sqlDecAtas += idDoc;
													break;
												case "DocumentId":
													sqlDecAtas += newDocumentId;
													break;																	
												case "DocumentDetailId":													
													sqlDecAtas += documentDetailId;
													break;
												default:
													if (dtDecAtas.Rows[j][dtDecAtas.Columns[i].ColumnName] == null || dtDecAtas.Rows[j][dtDecAtas.Columns[i].ColumnName].ToString().Length <= 0)
														sqlDecAtas += "NULL";
													else
													{
														switch (dtDecAtas.Columns[i].DataType.ToString())
														{
															case "System.String":
																sqlDecAtas += "'" + dtDecAtas.Rows[j][dtDecAtas.Columns[i].ColumnName].ToString() + "'";
																break;
															case "System.DateTime":
																DateTime dt = Convert.ToDateTime(General.Nz(dtDecAtas.Rows[j][dtDecAtas.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
																sqlDecAtas += General.ToDataUniv(dt);
																break;
															default:
																sqlDecAtas += dtDecAtas.Rows[j][dtDecAtas.Columns[i].ColumnName].ToString();
																break;
														}
													}
													break;
											}
											if (i < dtDecAtas.Columns.Count - 1)
												sqlDecAtas += ", ";
										}
									}

									sqlDecAtas += ")";
								}							
								General.ExecutaNonQuery(sqlDecAtas, null);									
                       
								sql = "SELECT * FROM tblFisiere WHERE Tabela = 'AvsXDec_relUploadDocumente' AND Id = " + dtDecAtas.Rows[j]["IdDocument"].ToString();
								DataTable dtFisiereDec = General.IncarcaDT(sql, null);					   

                                for (int l = 0; l < dtFisiereDec.Rows.Count; l++)
                                {									
									string sqlFisDec = "";								
									if (dtFisiereDec != null && dtFisiereDec.Rows.Count > 0)
									{
										sqlFisDec = "INSERT INTO \"tblFisiere\" (";
										for (int i = 0; i < dtFisiereDec.Columns.Count; i++)
										{
											if (!dtFisiereDec.Columns[i].AutoIncrement)
											{
												sqlFisDec += "\"" + dtFisiereDec.Columns[i].ColumnName + "\"";
												if (i < dtFisiereDec.Columns.Count - 1)
													sqlFisDec += ", ";
											}
										}
										sqlFisDec += ") VALUES (";
										for (int i = 0; i < dtFisiereDec.Columns.Count; i++)
										{
											if (!dtFisiereDec.Columns[i].AutoIncrement)
											{
												switch (dtFisiereDec.Columns[i].ColumnName)
												{
													case "Id":
														sqlFisDec += idDoc;
														break;
													default:
														if (dtFisiereDec.Rows[l][dtFisiereDec.Columns[i].ColumnName] == null || dtFisiereDec.Rows[l][dtFisiereDec.Columns[i].ColumnName].ToString().Length <= 0)
															sqlFisDec += "NULL";
														else
														{
															switch (dtFisiereDec.Columns[i].DataType.ToString())
															{
																case "System.String":
																	sqlFisDec += "'" + dtFisiereDec.Rows[l][dtFisiereDec.Columns[i].ColumnName].ToString() + "'";
																	break;
																case "System.DateTime":
																	DateTime dt = Convert.ToDateTime(General.Nz(dtFisiereDec.Rows[l][dtFisiereDec.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
																	sqlFisDec += General.ToDataUniv(dt);
																	break;
																default:
																	sqlFisDec += dtFisiereDec.Rows[l][dtFisiereDec.Columns[i].ColumnName].ToString();
																	break;
															}
														}
														break;
												}
												if (i < dtFisiereDec.Columns.Count - 1)
													sqlFisDec += ", ";
											}
										}

										sqlFisDec += ")";
									}							
									General.ExecutaNonQuery(sqlFisDec, null);	   
                                }
                            }
                            #endregion                           
                        }
                        #endregion
                        break;
                }
                #region salvare istoric


                /*salvare BusinessTransaction - Radu 01.09.2016*/
				sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + newDocumentId;
				DataTable dtNewDoc = General.IncarcaDT(sql, null);				
				
                #region salvare Business Transaction
                if (dtBT != null)
                {
					DataTable ent = GetmetaVwAvsXDec_Decont(newDocumentId, Convert.ToInt32((dtBT.Rows[0]["SrcDocId"] ?? -99).ToString()));

					sql = "SELECT * FROM AvsXDec_BusinessTransaction WHERE DestDocId = " + ent.Rows[0]["DocumentId"].ToString();
					DataTable dtBusTran = General.IncarcaDT(sql, null);
					
                    if (dtBusTran == null)
                    {
                        /*LeonardM 22.09.2016
                         * pentru decont administrativ, se copie fix suma de pe decontul anteriori*/
                        //if (entDocumentSursa.DocumentTypeId == 2003)
                        //{
                        //    AvsXDec_BusinessTransaction btDocSursa = ctx.AvsXDec_BusinessTransaction.Where(p => p.DestDocId == entDocumentSursa.DocumentId).FirstOrDefault();
                        //    if (btDocSursa != null)
                        //    {
                        //        AvsXDec_BusinessTransaction btNew2003 = new AvsXDec_BusinessTransaction();
                        //        btNew2003.TransactionId = GetNextId("AvsXDec_BusinessTransaction", 1);
                        //        btNew2003.SrcDocId = ent.AvansDocumentId;
                        //        btNew2003.DestDocId = ent.DocumentId;
                        //        btNew2003.SrcDocAmount = btDocSursa.SrcDocAmount;
                        //        btNew2003.DestDocAmount = btDocSursa.DestDocAmount;
                        //        btNew2003.USER_NO = entDocument.USER_NO;
                        //        btNew2003.TIME = DateTime.Now;

                        //        if ((btNew2003.EntityState != EntityState.Detached))
                        //        {
                        //            this.ObjectContext.ObjectStateManager.ChangeObjectState(btNew2003, EntityState.Added);
                        //        }
                        //        else
                        //        {
                        //            //this.ObjectContext.AvsXDec_BusinessTransaction.AddObject(btNew);
                        //            ctx.AvsXDec_BusinessTransaction.AddObject(btNew2003);
                        //        }
                        //    }

                        //}
                        //else
                        {
							sql = "SELECT * FROM AvsXDec_BusinessTransaction WHERE DestDocId = " + dtDocSursa.Rows[0]["DocumentId"].ToString();
							DataTable dtBTDocSursa = General.IncarcaDT(sql, null);

                            /*nu exista legatura intre avans si decont, trebuie inserat un rand nou*/
							sql = "INSERT INTO AvsXDec_BusinessTransaction (TransactionId, SrcDocId, DestDocId, SrcDocAmount, DestDocAmount, USER_NO, TIME) "
								+ " VALUES (" + Dami.NextId("AvsXDec_BusinessTransaction", 1) + ", " + ent.Rows[0]["AvansDocumentId"].ToString() + ", " 
								+ ent.Rows[0]["DocumentId"].ToString() + ", " + ent.Rows[0]["TotalAmountAvans"].ToString() + ", " 
								+ (dtBTDocSursa == null || dtBTDocSursa.Rows.Count == 0 ? ent.Rows[0]["EstimatedAmount"].ToString() : dtBTDocSursa.Rows[0]["DestDocAmount"].ToString()) 
								+ ", " + dtNewDoc.Rows[0]["USER_NO"].ToString() + ", GETDATE())";
								General.ExecutaNonQuery(sql, null);							

                        }
                    }
                    else
                    {
						sql = "UPDATE AvsXDec_BusinessTransaction SET SrcDocId = " + ent.Rows[0]["AvansDocumentId"].ToString() + ", DestDocId = " + ent.Rows[0]["DocumentId"].ToString() 
						+ ", SrcDocAmount = " + ent.Rows[0]["TotalAmountAvans"].ToString() + ", DestDocAmount = " + ent.Rows[0]["EstimatedAmount"].ToString() 
						+ ", USER_NO = " + ent.Rows[0]["USER_NO"].ToString() + ", TIME = GETDATE() WHERE DestDocId = " + ent.Rows[0]["DocumentId"].ToString();
						General.ExecutaNonQuery(sql, null);	
                    }
                }
                #endregion
                //end Radu



                int pozitieIstoric = 1;
				string val = "";
                /*variabila care memoreaza ca pe circuit a fost aprobat documentul pana la utilizator*/
                bool aprobatUserDocument = false;
				
				sql = "SELECT * FROM AvsXDec_DocumentStateHistory WHERE DocumentId = " + documentId;
				DataTable dtDocumentStateHistory = General.IncarcaDT(sql, null);
				
				if (dtDocumentStateHistory != null && dtDocumentStateHistory.Rows.Count > 0)
				{	
					for (int k = 0; k < dtDocumentStateHistory.Rows.Count; k++)
					{
						string sqlDoc = "";
						if (dtDocumentStateHistory != null && dtDocumentStateHistory.Rows.Count > 0)
						{
							sqlDoc = "INSERT INTO \"AvsXDec_Avans\" (";
							for (int i = 0; i < dtDocumentStateHistory.Columns.Count; i++)
							{
								if (!dtDocumentStateHistory.Columns[i].AutoIncrement)
								{
									sqlDoc += "\"" + dtDocumentStateHistory.Columns[i].ColumnName + "\"";
									if (i < dtDocumentStateHistory.Columns.Count - 1)
										sqlDoc += ", ";
								}
							}
							sqlDoc += ") VALUES (";
							for (int i = 0; i < dtDocumentStateHistory.Columns.Count; i++)
							{
								if (!dtDocumentStateHistory.Columns[i].AutoIncrement)
								{
									switch (dtDocumentStateHistory.Columns[i].ColumnName)
									{
										case "DocumentId":
											sqlDoc += newDocumentId;
											break;
										case "Id":
											sqlDoc += Dami.NextId("AvsXDec_DocumentStateHistory", 1);
											break;									
										case "TIME":
											sqlDoc += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
											break;
										case "Aprobat":
											val = "NULL";
											if (aprobatUserDocument)
												val = "NULL";
											if ((!aprobatUserDocument && pozitieIstoric <= Convert.ToInt32(dtDocSursa.Rows[0]["TotalCircuit"].ToString()))) 
												val = "1";
											if ((Convert.ToInt32(dtDocumentStateHistory.Rows[k]["USER_NO"].ToString()) == Convert.ToInt32(dtDocSursa.Rows[0]["USER_NO"].ToString())))
											{
												aprobatUserDocument = true;
												val = "1";
											}
											sql += val;
											break;	
										case "DocumentStateId":
											if (aprobatUserDocument)
												sqlDoc += "NULL";
											break;
										case "DataAprobare":
											if (aprobatUserDocument)
												sqlDoc += "NULL";
											break;
										case "Culoare":
											if (aprobatUserDocument)
												sqlDoc += "NULL";
											break;											
										default:
											if (dtDocumentStateHistory.Rows[k][dtDocumentStateHistory.Columns[i].ColumnName] == null || dtDocumentStateHistory.Rows[k][dtDocumentStateHistory.Columns[i].ColumnName].ToString().Length <= 0)
												sqlDoc += "NULL";
											else
											{
												switch (dtDocumentStateHistory.Columns[i].DataType.ToString())
												{
													case "System.String":
														sqlDoc += "'" + dtDocumentStateHistory.Rows[k][dtDocumentStateHistory.Columns[i].ColumnName].ToString() + "'";
														break;
													case "System.DateTime":
														DateTime dt = Convert.ToDateTime(General.Nz(dtDocumentStateHistory.Rows[k][dtDocumentStateHistory.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
														sqlDoc += General.ToDataUniv(dt);
														break;
													default:
														sqlDoc += dtDocumentStateHistory.Rows[k][dtDocumentStateHistory.Columns[i].ColumnName].ToString();
														break;
												}
											}
											break;
									}
									if (i < dtDocumentStateHistory.Columns.Count - 1)
										sqlDoc += ", ";
								}
							}

							sqlDoc += ")";
						}	
						pozitieIstoric++;
						General.ExecutaNonQuery(sqlDoc, null);	
					}					
				}	
                #endregion              
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

		
		
        public string AnuleazaDocument(int idUser, int DocumentId, int f10003, string refuseReason = "")
        {
            string msg = "";

            try
            {
                if (DocumentId == -99) return msg;

				string sql = "SELECT * FROM AvsXDec_Document WHERE DocumentId = " + DocumentId;
				DataTable ent = General.IncarcaDT(sql, null);

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
					string culoare = "";
					sql = "SELECT * FROM \"AvsXDec_DictionaryItem\" WHERE \"DictionaryItemId\" = -1";
					DataTable dtCul = General.IncarcaDT(sql, null);
					if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
						culoare = dtCul.Rows[0]["Culoare"].ToString();
					else
						culoare = "#FFFFFFFF";				
					
                    //schimbam statusul
					sql = "UPDATE AvsXDec_Document SET DocumentStateId = -1, Culoare = '" + culoare + "' WHERE DocumentId = " + DocumentId;
					General.ExecutaNonQuery(sql, null);						

                    //introducem o linie de anulare in AvsXDec_DocumentStateHistory
					int idUrmStateHistory = Dami.NextId("AvsXDec_DocumentStateHistory", 1);
					sql = "INSERT INTO AvsXDec_DocumentStateHistory (Id, DocumentId, CircuitId, DocumentStateId, Pozitie, Culoare, Aprobat, DataAprobare, USER_NO, TIME, Inlocuitor) "
						+ " VALUES(" + idUrmStateHistory + ", " + ent.Rows[0]["DocumentId"].ToString() + ", " + ent.Rows[0]["CircuitId"].ToString() + ", -1, 22, '" + culoare + "', 1, GETDATE(), " 
						+ idUser + ", GETDATE(), 0)";
					General.ExecutaNonQuery(sql, null);

                    #region salvare motiv refuz
                    switch (Convert.ToInt32(ent.Rows[0]["DocumentTypeId"].ToString()))
                    {
                        case 1001: /* Avans spre deplasare*/
                        case 1002: /* Avans spre decontare*/
                        case 1003: /*Avans administrativ*/
							sql = "UPDATE AvsXDec_Avans SET RefuseReason = '" + refuseReason + "' WHERE DocumentId = " + ent.Rows[0]["DocumentId"].ToString();
							General.ExecutaNonQuery(sql, null);
                            break;
                        case 2001: /* Decont spre deplasare*/
                        case 2002: /*Decont cheltuieli*/
                        case 2003: /*Decont administrativ*/
							sql = "UPDATE AvsXDec_Decont SET RefuseReason = '" + refuseReason + "' WHERE DocumentId = " + ent.Rows[0]["DocumentId"].ToString();
							General.ExecutaNonQuery(sql, null);		
                            break;
                    }
                    #endregion


           

                    #region  Notificare strat

                    //ctxNtf.TrimiteNotificare("AvansXDecont.Document", "grDate", ent, idUser, f10003);

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

        public DataTable SelectGrid(bool loadAllList = false)
        {

            DataTable q = null;

            try
            {		
				string sql = "SELECT a.DocumentId, b.DocumentTypeName, a.DocumentTypeId, a.F10003, a.DocumentDate, c.F10008 + ' ' + c.F10009 AS NumeComplet, "
						+ " d.DictionaryItemName AS Stare, a.DocumentStateId AS IdStare, a.Culoare, a.TIME, a.CircuitId AS IdCircuit, a.USER_NO, srcDoc.SrcDocId, "
						+ " (CASE WHEN av.RefuseReason IS NULL OR LEN(av.RefuseReason) <= 0 THEN dec.RefuseReason ELSE av.RefuseReason END) AS RefuseReason, "
						+ " (CASE WHEN av.TotalAmount IS NULL THEN dec.TotalAmount ELSE av.TotalAmount END) AS TotalAmountDocument, "
						+ " (CASE WHEN av.CurrencyId IS NULL THEN dec.CurrencyId ELSE av.CurrencyId END) AS CurrencyIdDocument, "
						+ " (CASE WHEN avMoneda.DictionaryItemName IS NULL THEN decMoneda.DictionaryItemName ELSE avMoneda.DictionaryItemName END) AS CurrencyCodeDocument, "
						+ " (CASE WHEN (CASE WHEN bugOwner.IdSuper IS NULL THEN -99 ELSE bugOwner.IdSuper END) = -99 THEN 0 ELSE 1 END) AS IsBudgetOwnerForDocument, "
						+ " (CASE WHEN (a.DocumentTypeId IN (1001, 1002, 1003, 1004) AND a.DocumentStateId < 4 AND a.USER_NO != docState.USER_NO) OR " 
						+ " (a.DocumentTypeId IN (2001, 2002, 2003) AND a.DocumentStateId < 7 AND a.USER_NO != docState.USER_NO)  THEN 1 ELSE 0 END) AS canBeRefused, a.Pozitie "
						+ " FROM AvsXDec_Document a "
						+ " JOIN AvsXDec_DocumentStateHistory docState ON a.DocumentId = docState.DocumentId "
						+ " JOIN AvsXDec_DocumentType b on a.DocumentTypeId = b.DocumentTypeId "
						+ " JOIN F100 c on a.F10003 = c.F10003 "
						+ " JOIN vwAvsXDec_Nomen_StariDoc d on a.DocumentStateId = d.DictionaryItemId "
						+ " LEFT JOIN AvsXDec_BusinessTransaction srcDoc on a.DocumentId = srcDoc.DestDocId " 
						/*LeonardM 13.06.2016
						 * prleluare avans sau decont in functie de tipul documentului pentru a prelua si suma ce trebuie aprobata*/
						+ " LEFT JOIN AvsXDec_Avans av on a.DocumentId = av.DocumentId "		
						+ " LEFT JOIN vwAvsXDec_Nomen_TipMoneda avMoneda on av.CurrencyId = avMoneda.DictionaryItemId "	
						+ " LEFT JOIN AvsXDec_Decont dec on a.DocumentId = dec.DocumentId "		
						+ " LEFT JOIN vwAvsXDec_Nomen_TipMoneda decMoneda on dec.CurrencyId = decMoneda.DictionaryItemId "
						/*end LeonardM 13.06.2016*/
						+ " LEFT JOIN vwAvsXDec_BudgetOwner bugOwner on  a.DocumentTypeId = (CASE WHEN bugOwner.DocumentTypeId IS NULL THEN -99 ELSE bugOwner.DocumentTypeId END ) " 
						+ " AND (CASE WHEN docState.IdSuper IS NULL THEN -99 ELSE docState.IdSuper END) =  (CASE WHEN bugOwner.IdSuper IS NULL THEN -99 ELSE bugOwner.IdSuper END) "
                        /*LeonardM 15.08.2016
                         * filtrare sa fie aduse doar documentele apartinand utilizatorului logat
                         * sau care trebuie sa le aprobe*/						
						+ " WHERE (docState.USER_NO = " + (loadAllList ? "docState.USER_NO" : Session["UserId"].ToString()) 
						+ "	OR docState.IdUserInlocuitor = " + (loadAllList ? "docState.IdUserInlocuitor" : Session["UserId"].ToString()) + ") "
						+ " group by a.DocumentId, b.DocumentTypeName, a.DocumentTypeId, a.F10003, a.DocumentDate, c.F10008 + ' ' + c.F10009,  d.DictionaryItemName, a.DocumentStateId, a.Culoare, a.TIME, "
						+ " a.CircuitId, a.USER_NO, srcDoc.SrcDocId, av.RefuseReason, dec.RefuseReason,  "
						+ "	av.TotalAmount, dec.TotalAmount ,  av.CurrencyId, dec.CurrencyId ,  "
						+ "	avMoneda.DictionaryItemName , decMoneda.DictionaryItemName ,   bugOwner.IdSuper , docState.USER_NO,  a.Pozitie";
					

                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }
		
		
        protected void pnlCtl_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {

                int DocumentTypeId = Convert.ToInt32(cmbDocumentType.Value ?? -99);

                switch (DocumentTypeId)
                {
                    case 1001:/*avans spre deplasare*/
                    case 1002:/*avans spre decontare*/
                        deDataDoc.Value = DateTime.Now;
                        deDataDoc.ClientEnabled = false;
                        lblDataDocument.InnerText = "Data solicitare avans:";
                        break;
                    case 2001:/*decont avans spre deplasare*/
                    case 2002:/*decont avans spre decontare*/
                    case 2003: /*decont administrativ*/
                        deDataDoc.Value = DateTime.Now;
                        deDataDoc.ClientEnabled = false;
                        lblDataDocument.InnerText = "Data solicitare decont:";
                        break;
                    case -99:
						MessageBox.Show(Dami.TraduCuvant("Trebuie selectat tipul de document mai intai!"), MessageBox.icoWarning, "");
                        deDataDoc.Value = null;
                        break;
                    default:
                        break;
                }

				if (deDataDoc.Value != null)
					Session["AvsXDec_DataDoc"] = Convert.ToDateTime(deDataDoc.Value);
				else
					Session["AvsXDec_DataDoc"] = null;


			}
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }	

        protected void btnSalvare_Click(object sender, EventArgs e)
        {
            try 
            {

				if (Session["AvsXDec_DataDoc"] == null || cmbDocumentType.Value == null)
				{
					MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoWarning, "");  
					return;
				}
				else
				{
					int DocumentTypeId = Convert.ToInt32(cmbDocumentType.Value);
					switch (Convert.ToInt32(cmbDocumentType.Value))
					{
						/*Avans spre deplasare*/
						case 1001:
						/*Avans spre decontare*/
						case 1002:
							#region adaugare avans
							string result = VerificaDocumentAdaugat(Convert.ToInt32(Session["UserId"].ToString()), DocumentTypeId);						
								
							if (result.Length > 0)
							{
								MessageBox.Show(Dami.TraduCuvant(result), MessageBox.icoWarning, "");
								return;
							}
							int idDocument = -99;
							idDocument = AdaugaDocument(Convert.ToInt32(Session["UserId"].ToString()), DocumentTypeId);					

							switch (idDocument)
							{
								case -123:
									MessageBox.Show(Dami.TraduCuvant("Nu exista circuit definit pentru tipul de document ales!"), MessageBox.icoWarning, "");
									return;
									break;
							}

							switch (DocumentTypeId)
							{
								/*Avans spre deplasare*/
								case 1001:
								/*Avans spre decontare*/
								case 1002:
									Session["AvsXDec_IdDocument"] = idDocument;
									Session["AvsXDec_DocumentTypeId"] = DocumentTypeId;								
									Session["AvsXDec_Marca"] = Convert.ToInt32(Session["User_Marca"].ToString());									
									Session["AvsXDec_PoateModif"] = 1;
									Session["AvsXDec_EsteNou"] = 1;

									string url = "~/AvansXDecont/DocumentAvans";
									if (Page.IsCallback)
										ASPxWebControl.RedirectOnCallback(url);
									else
										Response.Redirect(url, false);	
									break;
							}
							#endregion
							break;
						case 2001: /*Decont cheltuieli*/
						case 2003: /*Decont cheltuieli administrative*/
						case 2002:
							#region adaugare decont
							idDocument = -99;
							idDocument = AdaugaDocument(Convert.ToInt32(Session["UserId"].ToString()), DocumentTypeId);	

							switch (idDocument)
							{
								case -123:
									MessageBox.Show(Dami.TraduCuvant("Nu exista circuit definit pentru tipul de document ales!"), MessageBox.icoWarning, "");								
									return;
									break;
							}

							switch (DocumentTypeId)
							{
								case 2001: /*Decont avans spre deplasare*/
								case 2003: /*Decont cheltuieli administrative*/
								case 2002: /*Decont cheltuieli*/
									Session["AvsXDec_IdDocument"] = idDocument;
									Session["AvsXDec_DocumentTypeId"] = DocumentTypeId;								
									Session["AvsXDec_Marca"] = Convert.ToInt32(Session["User_Marca"].ToString());									
									Session["AvsXDec_PoateModif"] = 1;
									Session["AvsXDec_EsteNou"] = 1;
									
									string url = "~/AvansXDecont/DocumentDecont";
									if (Page.IsCallback)
										ASPxWebControl.RedirectOnCallback(url);
									else
										Response.Redirect(url, false);
									break;
							}								
							#endregion
							break;
					}
				}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
		
        public DataTable GetmetaVwAvsXDec_Decont(int documentId, int documentIdAvans = -99)
        {
            DataTable q = null;

            #region preluare suma utilizata din avans
            DataTable amountConsumedXDoc = null;
            decimal consumedAmountXDoc = 0;
            string sqlQuery = "";
            try
            {
                sqlQuery = @"select bt.""SrcDocId"", cast(sum(bt.""DestDocAmount"") as decimal(18, 4)) as ""DestDocAmount""
                            from ""AvsXDec_BusinessTransaction"" bt
                            join ""AvsXDec_Document"" doc on bt.""DestDocId"" = doc.""DocumentId""
                                                        and doc.""DocumentStateId"" >=1
                            where bt.""SrcDocId"" = {0}
                            group by ""SrcDocId""";
                sqlQuery = string.Format(sqlQuery, documentIdAvans);
				
                amountConsumedXDoc = General.IncarcaDT(sqlQuery, null);
                if (amountConsumedXDoc.Rows.Count != 0)
                    consumedAmountXDoc = Convert.ToInt32((amountConsumedXDoc.Rows[0]["DestDocAmount"] == DBNull.Value ? 0 : amountConsumedXDoc.Rows[0]["DestDocAmount"]).ToString());
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
				string sql = "SELECT * FROM AvsXDec_BusinessTransaction WHERE DestDocId = " + documentId;
				DataTable bt = General.IncarcaDT(sql, null);
                if (bt != null && bt.Rows.Count > 0)
                {
                    #region legatura existenta intre avans si decont
					sql = "SELECT a.DocumentId, COALESCE(b.SrcDocId, -99) AS AvansDocumentId, COALESCE(a.DocumentTypeId, -99) AS DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName, "
						+ " COALESCE(a.DocumentStateId, -99) AS DocumentStateId, a.DocumentState, COALESCE(a.F10003, -99) AS F10003, a.NumeComplet, a.IdDepartament, a.Departament, "
						+ " a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) AS CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, a.ActionTypeId, a.ActionTypeName, a.ActionPlace, a.ActionReason, "
						+ " a.StartDate, a.EndDate, a.StartHour, a.EndHour, a.chkDiurna, a.TotalAmount, a.EstimatedAmount, b.SrcDocAmount AS TotalAmountAvans, a.CurrencyId, a.CurrencyCode, "
						+ " a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, a.PaymentTypeId, a.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie, a.UnconfRestAmount,"
						+ " a.OriginalDoc, a.TransportTypeId, a.TransportTypeName "
						+ " FROM vwAvsXDec_Decont a "
						+ " JOIN AvsXDec_BusinessTransaction b on a.DocumentId = b.DestDocId "
						+ " WHERE a.DocumentId =" + documentId;
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
						sql = "SELECT a.DocumentId, NULL AS AvansDocumentId, COALESCE(a.DocumentTypeId, -99) AS DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName, "
							+ " COALESCE(a.DocumentStateId, -99) AS DocumentStateId, a.DocumentState, COALESCE(a.F10003, -99) AS F10003, a.NumeComplet, a.IdDepartament, a.Departament, "
							+ " a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) AS CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, a.ActionTypeId, a.ActionTypeName, a.ActionPlace, a.ActionReason, "
							+ " a.StartDate, a.EndDate, a.StartHour, a.EndHour, a.chkDiurna, a.TotalAmount, a.EstimatedAmount, 0 AS TotalAmountAvans, a.CurrencyId, a.CurrencyCode, "
							+ " a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, a.PaymentTypeId, a.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie, a.UnconfRestAmount,"
							+ " a.OriginalDoc, a.TransportTypeId, a.TransportTypeName "
							+ " FROM vwAvsXDec_Decont a "
							+ " WHERE a.DocumentId =" + documentId;
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
                                    /*TotalAmount - pentru documente de tip decont administrativ, scadem suma consumata deja din avans
                                     * pentru a aduce doar soldul*/	
                                    /*TotalAmountAvans - pentru documente de tip decont administrativ, scadem suma consumata deja din avans
                                     * pentru a aduce doar soldul*/									 
							sql = "SELECT a.DocumentId, av.DocumentId AS AvansDocumentId, COALESCE(a.DocumentTypeId, -99) AS DocumentTypeId, a.DocumentTypeCode, a.DocumentTypeName, "
								+ " COALESCE(a.DocumentStateId, -99) AS DocumentStateId, a.DocumentState, COALESCE(a.F10003, -99) AS F10003, a.NumeComplet, a.IdDepartament, a.Departament, "
								+ " a.LocMunca, a.DocumentDate, COALESCE(a.CircuitId, -99) AS CircuitId, a.TotalCircuit, a.Culoare, a.Pozitie, av.ActionTypeId, av.ActionTypeName, av.ActionPlace, av.ActionReason, "
								+ " av.StartDate, av.EndDate, av.StartHour, av.EndHour, av.chkDiurna, (av.TotalAmount - (CASE WHEN a.DocumentTypeId = 2003 THEN " + consumedAmountXDoc + " ELSE 0 END)) AS TotalAmount, " 
								+ " a.EstimatedAmount, (av.TotalAmount - (CASE WHEN a.DocumentTypeId = 2003 THEN consumedAmountXDoc ELSE 0 END)) AS TotalAmountAvans, av.CurrencyId, av.CurrencyCode, "
								+ " a.ContBancar, a.BankId, a.BankName, a.SucursalaId, a.SucursalaName, av.PaymentTypeId, av.PaymentTypeName, a.PaymentDate, a.IdFunctie, a.Functie, a.UnconfRestAmount,"
								+ " a.OriginalDoc, av.TransportTypeId, av.TransportTypeName "
								+ " FROM vwAvsXDec_Decont a "
								+ " JOIN vwAvsXDec_Avans av on 1 = 1 "
								+ " WHERE a.DocumentId =" + documentId + " AND av.DocumentId = " + documentIdAvans;
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
		

    }
}