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

namespace WizOne.Posturi
{
    public partial class FormLista : System.Web.UI.Page
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
                btnAprobareHR.Text = Dami.TraduCuvant("btnAprobareHR", "Aprobare HR");
                btnNou.Text = Dami.TraduCuvant("btnNou", "Adauga");
                btnDuplica.Text = Dami.TraduCuvant("btnDuplica", "Duplica");

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


                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Denumire", typeof(string));
                dt.Rows.Add(1, "Nivelul meu");
                dt.Rows.Add(2, "Toate");

                if (!IsPostBack)
                    cmbNivel.Value = 1;
                cmbNivel.DataSource = dt;
                cmbNivel.DataBind();

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                IncarcaGrid();

                DataTable dtForm = GetFormulareUser(Convert.ToInt32(Session["UserId"].ToString()));
                cmbForm.DataSource = dtForm;
                cmbForm.DataBind();

                cmbFormNou.DataSource = dtForm;
                cmbFormNou.DataBind();
                

                cmbRecrut.DataSource = GetCereriRecrutare();
                cmbRecrut.DataBind();


                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                string sql = "SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdUser\" = {0} AND \"IdSuper\" IN ({1})";
                sql = string.Format(sql, Session["UserId"].ToString(), idHR);
                DataTable dtHR = General.IncarcaDT(sql, null);               
                if (dtHR != null && dtHR.Rows.Count > 0 && dtHR.Rows[0][0] != null && dtHR.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtHR.Rows[0][0].ToString()) > 0)
                {
                    btnAprobareHR.ClientVisible = true;
                }             


                if (!IsPostBack)
                {
                    lblDataVig.Visible = false;
                    deDataVig.ClientVisible = false;
                    lblRecrut.Visible = false;
                    cmbRecrut.ClientVisible = false;
                    lblAng.Visible = false;
                    cmbAng.ClientVisible = false;
                    cmbFormNou.SelectedIndex = 0;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(dtDataSfarsit.Value) < Convert.ToDateTime(dtDataInceput.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Data sfarsit este mai mica decat data inceput !"), MessageBox.icoError, "Atentie!");
                    return;
                }

                IncarcaGrid();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltruSterge_Click(object sender, EventArgs e)
        {
            try
            {
                cmbForm.Value = null;
                dtDataInceput.Value = null;
                dtDataSfarsit.Value = null;
                cmbNivel.Value = 1;
                checkComboBoxStare.Value = null;

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


       

        protected void btnAproba_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRespinge_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnDuplica_Click(object sender, EventArgs e)
        {
            try
            {

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdFormular", "NumeComplet", "DataInceput", "F10003", "UserIntrod" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {                        
                    MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");                
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];

                    if (Convert.ToInt32(arr[1] ?? 0) != 0)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Se pot duplica numai cererile respinse!"), MessageBox.icoWarning, "");                     
                        return;
                    }

                    if (Convert.ToInt32(arr[6] ?? -99) != Convert.ToInt32(Session["UserId"].ToString()))
                    {
                        MessageBox.Show(Dami.TraduCuvant("Documentul a fost realizat de un alt utilizator!"), MessageBox.icoWarning, "");                      
                        return;
                    }

                    DuplicaDocument(Convert.ToInt32(Convert.ToInt32(arr[0] ?? -99)));                   
                    btnFiltru_Click(sender, e);   
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
            DataTable dt = new DataTable();

            try
            {
 

                grDate.KeyFieldName = "Id";

                string strSql = "";
                dt = SelectGrid(out strSql);


                grDate.DataSource = dt;
                grDate.DataBind();
                Session["FormLista_Grid"] = dt;
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
                    switch (arr[0])
                    {
                        case "btnDelete":
                            {
                                MetodeCereri(3, 1);
                                IncarcaGrid();
                            }
                            break;
                        case "btnEdit":
                            DataTable dt = Session["FormLista_Grid"] as DataTable;
                            DataRow[] dr = dt.Select("IdAuto = " + arr[1]);                            

                            int id = Convert.ToInt32(dr[0]["Id"].ToString());
                            int idFormular = Convert.ToInt32(dr[0]["IdFormular"].ToString());
                            int idPost = Convert.ToInt32(dr[0]["IdPost"] == DBNull.Value ? "-99" : dr[0]["IdPost"].ToString());
                            int f10003 = Convert.ToInt32(dr[0]["F10003"] == DBNull.Value ? "-99" : dr[0]["F10003"].ToString());
                            int idRol = Convert.ToInt32(dr[0]["IdRol"] == DBNull.Value ? "-99" : dr[0]["IdRol"].ToString());
                            int pozitie = Convert.ToInt32(dr[0]["Pozitie"] == DBNull.Value ? "-99" : dr[0]["Pozitie"].ToString());
                            int poateModif = Convert.ToInt32(dr[0]["PoateModifica"] == DBNull.Value ? "0" : dr[0]["PoateModifica"].ToString());
                            int idStare = Convert.ToInt32(dr[0]["IdStare"] == DBNull.Value ? "0" : dr[0]["IdStare"].ToString());

                            //daca formularul este inca in stadiul Solicitat, cel care l-a initiat sa-l poata modifica
                            if (idStare == 1 && Convert.ToInt32(Session["UserId"].ToString()) == Convert.ToInt32(dr[0]["UserIntrod"] == DBNull.Value ? "-99" : dr[0]["UserIntrod"].ToString()))
                                Session["FormDetaliu_PoateModifica"] = 1;

                            string descFormular = (dr[0]["DescFormular"] == DBNull.Value ? "" : dr[0]["DescFormular"].ToString());
                            string descPost = (dr[0]["DescPost"] == DBNull.Value ? "" : dr[0]["DescPost"].ToString());
                            DateTime dtVigoare = dr[0]["DataInceput"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr[0]["DataInceput"].ToString());

                            string url = "~/Posturi/FormDetaliu";
                            Session["FormDetaliu_Id"] = id;
                            Session["FormDetaliu_IdFormular"] = idFormular;                            
                            Session["FormDetaliu_IdStare"] = idStare;                   
                            Session["FormDetaliu_PoateModifica"] = poateModif;
                            Session["FormDetaliu_EsteNou"] = null;
                            Session["FormDetaliu_NumeFormular"] = descFormular;
                            Session["FormDetaliu_DataVigoare"] = dtVigoare;
                            Session["FormDetaliu_Pozitie"] = pozitie;
                            Session["FormDetaliu_IdRol"] = idRol;


                            if (Page.IsCallback)
                                ASPxWebControl.RedirectOnCallback(url);
                            else
                                Response.Redirect(url, false);

                            //switch (idFormular)
                            //{
                            //   case 20:
                            //        string url = "~/Posturi/FormDetaliu.aspx";
                            //        Session["FormDetaliu_Id"] = id;
                            //        Session["FormDetaliu_IdFormular"] = idFormular;
                            //        //pag20.idPost = idPost;
                            //        Session["FormDetaliu_IdStare"] = idStare;
                            //        //pag20.f10003 = f10003;
                            //        //pag20.dtVigoare = dtVigoare;
                            //        //pag20.idRol = idRol;
                            //        //pag20.pozitie = pozitie;
                            //        Session["FormDetaliu_PoateModifica"] = poateModif;

                            //        Session["FormDetaliu_EsteNou"] = null;
                            //        //Session["FormDetaliu_Titlu"] = (barTitlu.Content ?? "").ToString();

                            //        //pag20.txtData.EditValue = dtVigoare;
                            //        //pag20.txtPost.Text = descPost;
                            //        //pag20.txtForm.EditValue = descFormular;
                            //        if (Page.IsCallback)
                            //            ASPxWebControl.RedirectOnCallback(url);
                            //        else
                            //            Response.Redirect(url, false);


                            //        break;
                            //    case 21:
                            //        Session["FormDetaliu_Id"] = id;
                            //        Session["FormDetaliu_IdFormular"] = idFormular;
                            //        //pag21.idPost = idPost;
                            //        Session["FormDetaliu_IdStare"] = idStare;
                            //        //pag21.f10003 = f10003;
                            //        //pag21.dtVigoare = dtVigoare;
                            //        //pag21.idRol = idRol;
                            //        //pag21.pozitie = pozitie;
                            //        Session["FormDetaliu_PoateModifica"] = poateModif;

                            //        Session["FormDetaliu_EsteNou"] = null;
                            //        //pag21.titlu = (barTitlu.Content ?? "").ToString();                                 
                            //        break;
                            //}
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
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



        private void MetodeCereri(int tipActiune, int tipMsg = 0)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 
            //actiune  4  - aprobare HR

            try
            {
                int nrSel = 0;
                string ids = "", idsAtr = "", lstDataModif = "", lstMarci = "";
                string comentarii = "";
                string msg = "";

                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "Id", "IdStare", "IdFormular", "NumeComplet", "DataInceput", "F10003", "UserIntrod" });
                if (lst == null || lst.Count() == 0)
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
                    if (tipActiune == 3 && Convert.ToInt32(Session["UserId"].ToString()) != Convert.ToInt32(arr[6] ?? "-99"))
                    {
                        if (tipMsg == 0)
                            MessageBox.Show("Nu puteti anula o cerere care nu va apartine!", MessageBox.icoWarning, "");
                        else
                            grDate.JSProperties["cpAlertMessage"] = "Nu puteti anula o cerere care nu va apartine!";
                        return;
                    }


                    if ((arr[1] ?? "0").ToString() == "1" || (arr[1] ?? "0").ToString() == "2")
                    {
                        ids += lst[0].ToString() + ";";
                        nrSel++;
                    }
                }

                if (tipActiune == 1 || tipActiune == 2)
                    msg = msg + AprobaFormular(Convert.ToInt32(Session["UserId"].ToString()), ids, idsAtr, lstDataModif, lstMarci, nrSel, tipActiune, General.ListaCuloareValoare()[5], false, comentarii, Convert.ToInt32(Session["User_Marca"].ToString()));
                else if (tipActiune == 3)
                    msg = msg + AnuleazaFormular(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, Convert.ToInt32(Session["User_Marca"].ToString()));
                else
                    msg = msg + AprobaFormularRU(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, Convert.ToInt32(Session["User_Marca"].ToString()));

                if (tipMsg == 0)
                    MessageBox.Show(msg, MessageBox.icoWarning, "");
                else
                    grDate.JSProperties["cpAlertMessage"] = msg;

                IncarcaGrid();

                grDate.Selection.UnselectAll();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public string AprobaFormular(int idUser, string ids, string idsAtr, string lstDataModif, string lstMarci, int total, int actiune, string culoareValoare, bool HR, string comentarii, int f10003)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            //actiune  3  - anulare 


            string msg = "";
            string msgValid = "";

            try
            {
                if (ids == "") return "Nu exista cereri pentru aceasta actiune !";

                //string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                //if (("," + idHR + ",").IndexOf("," + General.Nz(cmbRol.Value, -99).ToString() + ",") >= 0)
                //    HR = true;

                int nr = 0;
                string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.None);

                string[] arrAtr = idsAtr.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrDataModif = lstDataModif.Split(new string[] { ";" }, StringSplitOptions.None);
                string[] arrMarci = lstMarci.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {
                            string sql = "SELECT * FROM \"Org_Date\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCer = General.IncarcaDT(sql, null);
                            sql = "SELECT * FROM \"Org_DateIstoric\" WHERE \"Id\" = " + id.ToString()
                            //+ (!HR ? " AND \"IdUser\" = " + idUser.ToString() : "") 
                            +  " AND \"IdUser\" = " + idUser.ToString()
                             //+ (actiune == 1 || actiune == 2 ? " AND \"Aprobat\" IS NULL" : " AND (\"Aprobat\" IS NULL  OR (\"Aprobat\" = 1 AND \"Pozitie\" = " + dtCer.Rows[0]["TotalCircuit"].ToString() + "))");
                             +  " AND \"Aprobat\" IS NULL" ;
                            DataTable dtCerIst = General.IncarcaDT(sql, null);

                            //aflam daca cel care aproba este inlocuitorul 
                            if (dtCerIst == null || dtCerIst.Rows.Count == 0)
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

                                string lstId = "(";
                                for (int j = 0; j < lstIdInloc.Count; j++)
                                {
                                    lstId += lstIdInloc[j];
                                    if (j < lstIdInloc.Count - 1)
                                        lstId += ",";
                                }
                                lstId += ")";
                                if (lstIdInloc.Count > 0)
                                {
                                    dtCerIst = General.IncarcaDT("SELECT * FROM \"Org_DateIstoric\" WHERE \"Id\" = " + id + " AND \"IdUser\" IN " + lstId, null);

                     
                                }
                            }


                            if (dtCerIst == null || dtCerIst.Rows.Count == 0) continue;

                            //verificam daca sunt eu cel care trebuie sa aprobe
                            if ((dtCerIst.Rows[0]["Pozitie"] == DBNull.Value ? -99 : Convert.ToInt32(dtCerIst.Rows[0]["Pozitie"].ToString())) 
                                != (dtCer.Rows[0]["Pozitie"] == DBNull.Value ? -99 : Convert.ToInt32(dtCer.Rows[0]["Pozitie"].ToString())) + 1)
                            {
                                continue;
                            }                                  

                            int idStare = 2;
     
                            if (idStare == 2 && dtCer.Rows[0]["TotalCircuit"].ToString() == dtCerIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

                            if (actiune == 2)
                                idStare = 0;  

                            string culoare = "";
                            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString();
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";

                            sql = "UPDATE \"Org_Date\" SET \"Pozitie\" = " + dtCerIst.Rows[0]["Pozitie"].ToString() + ", \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare 
                                + "' WHERE \"Id\" = " + id.ToString();
                            General.IncarcaDT(sql, null);

                            if (actiune == 1 || actiune == 2)
                            {
     
                                sql = "UPDATE \"Org_DateIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                + (dtCerIst.Rows[0]["IdUser"].ToString() != idUser.ToString() ? ", \"IdUserInlocuitor\" = " + idUser.ToString() : "")
                                + " WHERE \"Id\" = " + id.ToString() + " AND \"Pozitie\"=" + dtCerIst.Rows[0]["Pozitie"].ToString();
                            }
                            else
                            {
                                sql = $@"INSERT INTO ""Org_DateIstoric"" (""Id"", ""IdCircuit"", ""IdUser"", ""IdStare"", ""Pozitie"", ""Culoare"", ""Aprobat"", ""DataAprobare"", ""Inlocuitor"", USER_NO, TIME)
                                        VALUES ({id}, {dtCerIst.Rows[0]["IdCircuit"]}, {Session["UserId"]}, -1, 22, (SELECT ""Culoare"" FROM ""Ptj_tblStari"" WHERE ""Id"" = -1), 1, {General.CurrentDate()}, 0, {Session["UserId"]}, {General.CurrentDate()})";


                                //stergem din relatie Post Angajat, si actualizam data de sfarsit a penultimei linii (procesul in oglinda din functia ActualizeazaInRel
                                //var entRel = ctx.Org_relPostAngajat.Where(p => p.F10003 == ent.F10003 && p.IdPost == ent.IdPost && p.DataReferinta == ent.DataInceput).FirstOrDefault();
                                //if (entRel != null)
                                //{
                                //    var entOld = ctx.Org_relPostAngajat.Where(p => p.F10003 == ent.F10003 && p.IdPost == ent.IdPost && p.DataSfarsit == ent.DataInceput.Value.AddDays(-1)).FirstOrDefault();
                                //    if (entOld != null)
                                //    {
                                //        entOld.DataSfarsit = new DateTime(2100, 1, 1);
                                //    }
                                //    ctx.Org_relPostAngajat.DeleteObject(entRel);
                                //}

                            }
                            General.IncarcaDT(sql, null);


                            #region Validare start

                            //string corpMesaj = "";
                            //bool stop;

                            //srvNotif ctxNtf = new srvNotif();
                            //ctxNtf.ValidareRegula("Avs.Aprobare", "grDate", entCer, idUser, f10003, out corpMesaj, out stop);

                            //if (corpMesaj != "")
                            //{
                            //    msgValid += corpMesaj + "\r\n";
                            //    if (stop)
                            //    {
                            //        continue;
                            //    }
                            //}

                            #endregion


                            if (idStare == 3)
                            {
                                //ActualizeazaPlanHC(id);
                                //ActualizeazaInRel(idUser, id);
                                //CreazaAngajatDeTipCandidat(id, Convert.ToInt32(entCer.IdFormular), Convert.ToInt32(entCer.IdPost), Convert.ToDateTime(entCer.DataInceput).Date);
                            }

                            #region  Notificare start
                            string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                            int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);

                            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                            {
                                NotifAsync.TrimiteNotificare("Posturi.FormLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM Org_Date Z WHERE Id=" + id.ToString(), "Org_Date", id, idUser, marcaUser, arrParam);
                            });

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

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
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
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }

        public string AprobaFormularRU(int idUser, string ids, int total, int f10003)
        {
            //actiune  1  - aprobare

            string msg = "";
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
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {
                            string sql = "SELECT * FROM \"Org_Date\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCer = General.IncarcaDT(sql, null);
                            sql = "SELECT * FROM \"Org_DateIstoric\" WHERE \"Id\" = " + id.ToString()
                             + " AND \"Aprobat\" IS NULL ORDER BY Pozitie";
                            DataTable dtCerIst = General.IncarcaDT(sql, null);                  

                            int idStare = 2;
                            if (idStare == 2 && Convert.ToInt32(dtCer.Rows[0]["TotalCircuit"].ToString()) == Convert.ToInt32(dtCerIst.Rows[0]["Pozitie"].ToString())) idStare = 3;

                            string culoare = "";
                            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString();
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";

                            sql = "UPDATE \"Org_Date\" SET \"Pozitie\" = " + dtCerIst.Rows[0]["Pozitie"].ToString() + ", \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                 + "' WHERE \"Id\" = " + id.ToString();
                            General.IncarcaDT(sql, null);

                            sql = "UPDATE \"Org_DateIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                                 + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate")
                                 +  ", \"IdUserInlocuitor\" = " + idUser.ToString() 
                                 + " WHERE \"Id\" = " + id.ToString() + " AND \"Pozitie\"=" + dtCerIst.Rows[0]["Pozitie"].ToString();
                            General.IncarcaDT(sql, null);                    

                            #region Validare start

                            //string corpMesaj = "";
                            //bool stop;

                            //srvNotif ctxNtf = new srvNotif();
                            //ctxNtf.ValidareRegula("Posturi.FormLista", "grDate", entCer, idUser, f10003, out corpMesaj, out stop);

                            //if (corpMesaj != "")
                            //{
                            //    msgValid += corpMesaj + "\r\n";
                            //    if (stop)
                            //    {
                            //        continue;
                            //    }
                            //}

                            #endregion

                      
                            if (idStare == 3)
                            {
                                //ActualizeazaPlanHC(id);
                                //ActualizeazaInRel(idUser, id);
                                //CreazaAngajatDeTipCandidat(id, Convert.ToInt32(entCer.IdFormular), Convert.ToInt32(entCer.IdPost), Convert.ToDateTime(entCer.DataInceput).Date);
                            }

                            #region  Notificare strat

                            string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                            int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);

                            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                            {
                                NotifAsync.TrimiteNotificare("Posturi.FormLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM Org_Date Z WHERE Id=" + id.ToString(), "Org_Date", id, idUser, marcaUser, arrParam);
                            });

                            #endregion

                            nr++;

                        }
                    }
                }

                if (nr > 0)
                {
                   
                    msg = "S-au aprobat " + nr.ToString() + " cereri din " + total + " !";
                   

                    if (msgValid != "") msg = msg + "/n/r" + msgValid;
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

            return msg;

        }



        private string DamiStari()
        {
            string val = "";

            try
            {
                if (General.Nz(checkComboBoxStare.Value, "").ToString() != "")
                {
                    val = checkComboBoxStare.Value.ToString().ToLower().Replace("solicitat", "1").Replace("in curs", "2").Replace("aprobat", "3").Replace("respins", "0").Replace("anulat", "-1").Replace("planificat", "4");
                }
            }
            catch (Exception)
            {
            }

            return val;
        }

        public DataTable SelectGrid(out string strSql, bool param = true)
        {

            DataTable q = null;
            strSql = "";
            try
            {
                string strSub = "";
                string filtru = "";
                if (param && checkComboBoxStare.Value != null) filtru += @" AND A.""IdStare"" IN (" + DamiStari() + ")";
                int idFormular = param ? Convert.ToInt32(cmbForm.Value ?? -99) : -99;
                int nivel = param ? Convert.ToInt32(cmbNivel.Value ?? 1) : 1;
                DateTime dtInc = param ? Convert.ToDateTime(dtDataInceput.Value ?? new DateTime(1900, 1, 1)).Date : new DateTime(1900, 1, 1);
                DateTime dtSf = param ? Convert.ToDateTime(dtDataSfarsit.Value ?? new DateTime(2200, 12, 31)).Date : new DateTime(2200, 12, 31);
                int idUser = Convert.ToInt32(Session["UserId"].ToString());

                string dt = (Constante.tipBD == 1 ? "getdate()" : "sysdate");

                if (nivel == 1)                                         //nivelul meu
                    strSub = @"select ""Id"" from (        
                                        select a.* from ""Org_Date"" a
                                        inner join ""Org_DateIstoric"" b on a.""Id"" = b.""Id""
                                        where (a.""Pozitie"" + 1) = b.""Pozitie"" and b.""IdUser"" = {0}
                                        UNION
                                        select a.* from ""Org_Date"" a
                                        inner join ""Org_DateIstoric"" b on a.""Id"" = b.""Id""
                                        where (a.""Pozitie"" + 1) = b.""Pozitie"" and b.""IdUser"" in 
                                        (select b.F70102 from ""Ptj_Cereri"" a
                                        inner join USERS b on a.F10003 = b.F10003
                                        inner join USERS c on c.F10003 = a.""Inlocuitor""
                                        where a.""IdStare"" = 3 and c.F70102={0} and a.""DataInceput"" <= {1} and {1} <= a.""DataSfarsit"")
                                        ) X WHERE 1=1 ";
                else                                                    //toate nivelurile
                    strSub = @"select ""Id"" from (        
                                        select a.* from ""Org_Date"" a
                                        inner join ""Org_DateIstoric"" b on a.""Id"" = b.""Id""
                                        where b.""IdUser"" = {0}
                                        UNION
                                        select a.* from ""Org_Date"" a
                                        inner join ""Org_DateIstoric"" b on a.""Id"" = b.""Id""
                                        where b.""IdUser"" in 
                                        (select b.F70102 from ""Ptj_Cereri"" a
                                        inner join USERS b on a.F10003 = b.F10003
                                        inner join USERS c on c.F10003 = a.""Inlocuitor""
                                        where a.""IdStare"" = 3 and c.F70102={0} and a.""DataInceput"" <= {1} and {1} <= a.""DataSfarsit"")
                                        ) X WHERE 1=1 ";

                if (strSub != "")
                {
                    if (idFormular != -99) strSub += " AND \"IdFormular\" = " + idFormular;
                    strSub += " AND " + General.ToDataUniv(dtInc) + (Constante.tipBD == 1 ? " <= CONVERT(DATE, \"DataInceput\") AND CONVERT(DATE, \"DataInceput\") <= " : " <= TRUNC(\"DataInceput\") AND TRUNC(\"DataInceput\") <= ") + General.ToDataUniv(dtSf);
                    if (filtru != "")
                        strSub += filtru;
                        //strSub += " AND \"IdStare\" IN (" + filtru.Replace(";", ",").Substring(0, filtru.Length - 1) + ")";
                }

                strSub = string.Format(strSub, idUser, dt);

                if (strSub != "") filtru = " AND a.\"Id\" in (" + strSub + ") ";

                string idAuto = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS IdAuto";
                if (Constante.tipBD == 2) idAuto = "ROWNUM AS \"IdAuto\" ";

                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                string nvl = "ISNULL";
                if (Constante.tipBD == 2) nvl = "nvl";

                strSql = "select " + idAuto + ", x.* from ( " +
                                "select a.\"Id\", a.\"IdFormular\", a.\"IdPost\", a.\"IdCircuit\", a.F10003, a.\"Culoare\", a.\"DataInceput\", a.\"IdStare\", " +
                                " b.\"Denumire\" as \"DescFormular\", org1.\"PostDenumire\" as \"DescPost\", " +
                                " org1.\"NumeComplet\", " +
                                " a.\"Pozitie\", a.\"TotalCircuit\", a.TIME, a.USER_NO, a.\"UserIntrod\", " +
                                nvl + "(g.\"IdRol\",-99) as \"IdRol\", CASE WHEN COALESCE(h.\"Alias\",'') <> '' THEN COALESCE(h.\"Alias\",'') ELSE COALESCE(h.\"Denumire\",'') END AS \"DescRol\", " +
                                " case when (a.\"Pozitie\" + 1) = g.\"Pozitie\" then 1 else 0 end as \"PoateModifica\", " +
                                " x1.F00204 as \"PostCompanie\", x2.F00305 as \"PostSubcompanie\", x3.F00406 as \"PostFiliala\", x4.F00507 as \"PostSectie\", x5.F00608 as \"PostDept\", " +
                                " case a.\"IdFormular\" " +
                                " when 1 then org1.\"Locatie\" " +
                                " when 2 then org1.\"Locatie\" " +
                                " when 3 then org1.\"Locatie\" " +
                                " else ' ' end as \"Locatie\" " +
                                " from \"Org_Date\" a " +
                                " inner join \"Org_tblFormulare\" b on a.\"IdFormular\" = b.\"Id\" " +
                                " left join \"Org_Posturi\" c on a.\"IdPost\" = c.\"Id\"  and c.\"DataInceput\" <= a.\"DataInceput\" and a.\"DataInceput\" <= c.\"DataSfarsit\" " +
                                " left join F100 d on a.F10003 = d.F10003  " +
                                " left join \"Org_DateGenerale\" org1 on a.\"Id\" = org1.\"Id\" " +
                                " left join \"Org_DateIstoric\" g on  a.\"Id\" = g.\"Id\" and g.\"IdUser\"=" + idUser + " " +
                                " left join \"tblSupervizori\" h on (-1 * g.\"IdRol\") = h.\"Id\" " +
                                " LEFT JOIN F002 x1 ON c.F10002 = x1.F00202 " +
                                " LEFT JOIN F003 x2 ON c.F10004 = x2.F00304 " +
                                " LEFT JOIN F004 x3 ON c.F10005 = x3.F00405 " +
                                " LEFT JOIN F005 x4 ON c.F10006 = x4.F00506 " +
                                " LEFT JOIN F006 x5 ON c.F10007 = x5.F00607 " +
                                " where 1 = 1 " + filtru +
                                " group by a.\"Id\", a.\"IdFormular\", a.\"IdPost\", a.\"IdCircuit\", a.F10003, a.\"Culoare\", a.\"DataInceput\", a.\"IdStare\", " +
                                " b.\"Denumire\", c.\"Denumire\", d.F10008, d.F10009, " +
                                " a.\"Pozitie\", g.\"Pozitie\", a.\"TotalCircuit\", a.TIME, a.USER_NO, a.\"UserIntrod\", g.\"IdRol\", h.\"Denumire\", h.\"Alias\", " +
                                " x1.F00204, x2.F00305, x3.F00406, x4.F00507, x5.F00608, org1.\"Locatie\", org1.\"NumeComplet\", org1.\"PostDenumire\" ) x ";

                q = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public DataTable GetFormulareUser(int idUser)
        {
            DataTable q = null;

            try
            {
                string sql = "SELECT a.\"Id\", a.\"Denumire\" FROM \"Org_tblFormulare\" a JOIN \"Org_relFormularGrupUser\" b ON a.\"Id\" = b.\"IdFormular\" JOIN \"relGrupUser\" c on c.\"IdGrup\" = b.\"IdGrup\" WHERE c.\"IdUser\" = " 
                        + idUser + " GROUP BY  a.\"Id\", a.\"Denumire\" ORDER BY a.\"Id\"";

                q = General.IncarcaDT(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public void DuplicaDocument(int documentId)
        {
            try
            {
                string sql = "SELECT * FROM \"Org_Date\" WHERE \"Id\" = " + documentId.ToString();
                DataTable dtFor = General.IncarcaDT(sql, null);

                if (dtFor == null || dtFor.Rows.Count <= 0)
                    return;

                int newDocumentId = 0;
                sql = "SELECT * FROM \"Org_Date\"  ORDER BY \"Id\" DESC";
                DataTable dtMax = General.IncarcaDT(sql, null);
                if (dtMax != null && dtMax.Rows.Count > 0)
                    newDocumentId = Convert.ToInt32(dtMax.Rows[0]["Id"].ToString());
                newDocumentId++;

                sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = 1";
                DataTable dtCul = General.IncarcaDT(sql, null);
                string culoare = "#FFFFFFFF";
                if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value)
                    culoare = dtCul.Rows[0]["Culoare"].ToString();

                DateTime dtInc = Convert.ToDateTime(dtFor.Rows[0]["DataInceput"].ToString());
                string dataInceput = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtInc.Day.ToString().PadLeft(2, '0') + "/" + dtInc.Month.ToString().PadLeft(2, '0') + "/" + dtInc.Year.ToString() + "', 103)" :
                    "TO_DATE('" + dtInc.Day.ToString().PadLeft(2, '0') + "/" + dtInc.Month.ToString().PadLeft(2, '0') + "/" + dtInc.Year.ToString() + "', 'dd/mm/yyyy')");
                string strSql = "INSERT INTO \"Org_Date\"(\"Id\", \"IdFormular\", \"IdStare\", \"IdPost\", \"UserIntrod\", \"Inlocuitor\", \"IdCerereRecrutare\", \"IdCircuit\", \"TotalCircuit\", \"Culoare\", \"Pozitie\", \"DataInceput\", F10003, \"Observatii\", \"F10003Candidat\", USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11}, {12}, '{13}', {14}, {15}, {16})";
                strSql = string.Format(strSql, newDocumentId, dtFor.Rows[0]["IdFormular"].ToString(), 1, dtFor.Rows[0]["IdPost"].ToString(), dtFor.Rows[0]["UserIntrod"].ToString(), (dtFor.Rows[0]["Inlocuitor"] == DBNull.Value ? "NULL" : dtFor.Rows[0]["Inlocuitor"].ToString()),
                    dtFor.Rows[0]["IdCerereRecrutare"].ToString(), dtFor.Rows[0]["IdCircuit"].ToString(), dtFor.Rows[0]["TotalCircuit"].ToString(), culoare, 1, dataInceput, dtFor.Rows[0]["F10003"].ToString(), (dtFor.Rows[0]["Observatii"] == DBNull.Value ? "NULL" : dtFor.Rows[0]["Observatii"].ToString()),
                    (dtFor.Rows[0]["F10003Candidat"] == DBNull.Value ? "NULL" : dtFor.Rows[0]["F10003Candidat"].ToString()), Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));

                General.ExecutaNonQuery(strSql, null);


                sql = "SELECT * FROM \"Org_DateIstoric\" WHERE \"Id\" = " + documentId.ToString();
                DataTable dtForIst = General.IncarcaDT(sql, null);

                for (int i = 0; i < dtForIst.Rows.Count; i++)
                {
                    int poz = Convert.ToInt32(dtForIst.Rows[i]["Pozitie"].ToString());
                    string strSqlIst = "INSERT INTO \"Org_DateIstoric\"(\"Id\", \"IdCircuit\", \"IdPost\", \"IdUser\", \"IdStare\",  \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", \"Inlocuitor\", \"IdUserInlocuitor\", \"IdRol\",  USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13})";
                    strSqlIst = string.Format(strSqlIst, newDocumentId, dtForIst.Rows[i]["IdCircuit"].ToString(), dtForIst.Rows[i]["IdPost"].ToString(), dtForIst.Rows[i]["IdUser"].ToString(), (poz == 1 ? "1" : "NULL"), poz, (poz == 1 ? culoare : "NULL"),
                        (poz == 1 ? "1" : "NULL"), (poz == 1 ? (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") : "NULL"), (dtForIst.Rows[i]["Inlocuitor"] == DBNull.Value ? "NULL" : dtForIst.Rows[i]["Inlocuitor"].ToString()),
                        (dtForIst.Rows[i]["IdUserInlocuitor"] == DBNull.Value ? "NULL" : dtForIst.Rows[i]["IdUserInlocuitor"].ToString()), dtForIst.Rows[i]["IdRol"].ToString(), Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                    General.ExecutaNonQuery(strSqlIst, null);
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public DataTable GetCereriRecrutare()
        {
            DataTable q = null;

            try
            {
                string strSql = " ";
                string op = "+";
                if (Constante.tipBD == 2) op = "||";           
                
                strSql = @"SELECT s.""Id"", j.""PostId"", j.""PostDenumire"", s.""IdStare"",    
                        b.F00204 AS Companie, c.F00305 as Subcompanie, d.F00406 as Filiala, e.F00507 as Sectie, f.F00608 as Departament, 
                        CASE WHEN i.F10008 IS NOT NULL THEN i.F10008 {0} ' ' {0} i.F10009 ELSE h.F70104 END AS Solicitant 
                        FROM ""Org_Date"" s 
                        LEFT JOIN ""Org_Posturi"" a on s.""IdPost""=a.""Id"" 
                        LEFT JOIN ""Org_DateGenerale"" j on s.""Id""=j.""Id"" 
                        LEFT JOIN F002 b on j.""Companie""=b.F00202 
                        LEFT JOIN F003 c on j.""Subcompanie""=c.F00304 
                        LEFT JOIN F004 d on j.""Filiala""=d.F00405 
                        LEFT JOIN F005 e on j.""Sectie""=e.F00506 
                        LEFT JOIN F006 f on j.""Dept""=f.F00607 
                        LEFT JOIN ""Org_DateIstoric"" g on s.""Id""=g.""Id"" and g.""Pozitie""=1 
                        LEFT JOIN USERS h on g.""IdUser""=h.F70102 
                        LEFT JOIN F100 i on h.F10003=i.F10003 
                        WHERE s.""IdFormular""=20 AND s.""IdStare"" = 3";   

                strSql = string.Format(strSql, op);
                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }





        public DataTable GetF100NumeCompletPost(int idFormular, int idUser, DateTime dtVigoare)
        {
            DataTable q = null;

            try
            {
                string sql = "", condStare = "", condDifLuni = "", dtVig = "";
                string op = "+";

                dtVig = "CONVERT(DATETIME, '" + dtVigoare.Day.ToString().PadLeft(2, '0') + "/" + dtVigoare.Month.ToString().PadLeft(2, '0') + "/" + dtVigoare.Year.ToString() + "', 103)";
                if (Constante.tipBD == 2)
                    dtVig = "TO_DATE('" + dtVigoare.Day.ToString().PadLeft(2, '0') + "/" + dtVigoare.Month.ToString().PadLeft(2, '0') + "/" + dtVigoare.Year.ToString() + "', 'dd/mm/yyyy')";


                condStare = "CASE WHEN DATEDIFF(day, a.F10022, GETDATE()) >= 0 AND DATEDIFF(day, GETDATE(), a.F10023) >= 0 THEN 1 ELSE 0 END ";
                if (Constante.tipBD == 2)
                    condStare = "CASE WHEN trunc(sysdate) - trunc(a.F10022) >= 0 AND trunc(a.F10023) - trunc(sysdate) >= 0 THEN 1 ELSE 0 END";

                condDifLuni = " DATEDIFF(month, a.F10022, GETDATE()) >= 0 AND DATEDIFF(month, GETDATE(), a.F10023) >= 0 ";
                if (Constante.tipBD == 2)
                    condDifLuni = " MONTHS_BETWEEN(sysdate, a.F10022 ) >= 0 AND MONTHS_BETWEEN(a.F10023, sysdate ) >= 0 ";

                if (Constante.tipBD == 2) op = "||";
                bool esteConsRU = false;
                DataTable dtCons = General.IncarcaDT("SELECT COUNT(*) FROM \"F100Supervizori\" WHERE \"IdSuper\" = 12 AND \"IdUser\" = " + idUser, null);
                if (dtCons != null && dtCons.Rows.Count > 0 && dtCons.Rows[0][0] != DBNull.Value && Convert.ToInt32(dtCons.Rows[0][0].ToString()) > 0)
                    esteConsRU = true;

                //if (esteConsRU)                      //cand este consultant RU aratam toti angajatii
                //    sql = "SELECT   a.F10003, a.F10008 " + op + " ' ' " + op + " F10009 AS \"NumeComplet\", a.F10017 AS CNP, b.F00204 AS \"Companie\", c.F00305 AS \"Subcompanie\", d.F00406 AS \" Filiala\", "
                //        + " e.F00507 AS  \"Sectie\", f.F00608 AS \"Departament\", F10022 as \"DataAngajarii\", F10023 AS \"DataPlecarii\", F10011 AS \"NrContract\", " + condStare + " AS \"Stare\", "
                //        + " a.F100699 AS \"SalariuBrut\", MIN(F70102) AS \"IdUser\", x.F71804 AS \"Functia\", A.F100992 AS \"DataFunctie\", a.F10025, h.F70407 AS \"SalariulModifInAvans\"  FROM  F100 a "
                //        + " LEFT JOIN F002 b on a.F10002 = b.F00202 "
                //        + " LEFT JOIN F003 c on a.F10004 = c.F00304 "
                //        + " LEFT JOIN F004 d on a.F10005 = d.F00405 "
                //        + " LEFT JOIN F005 e on a.F10006 = e.F00506 "
                //        + " LEFT JOIN F006 f on a.F10007 = f.F00607 "
                //        + " LEFT JOIN USERS g on a.F10003 = g.F10003 "
                //        + " LEFT JOIN F718 x on a.F10071 = x.F71802 "
                //        + " LEFT JOIN F704 h on a.F10003 = h.F70403 "
                //        + " WHERE F70404 = 1 and F70420 = 0 AND " + dtVig + " >= F70406 "
                //        + " AND " + condDifLuni 
                //        + " GROUP BY a.F10003, a.F10008 " + op + " ' ' " + op + " F10009, a.F10017, b.F00204, c.F00305, d.F00406, e.F00507, f.F00608, a.F10022, a.F10023, a.F10011, a.F100699, A.F100992, a.F10025, x.F71804, h.F70407"
                //        + " ORDER BY \"NumeComplet\"";
                //else
                //    sql = "SELECT   a.F10003, a.F10008 " + op + " ' ' " + op + " F10009 AS \"NumeComplet\", a.F10017 AS CNP, b.F00204 AS \"Companie\", c.F00305 AS \"Subcompanie\", d.F00406 AS \" Filiala\", "
                //        + " e.F00507 AS  \"Sectie\", f.F00608 AS \"Departament\", F10022 as \"DataAngajarii\", F10023 AS \"DataPlecarii\", F10011 AS \"NrContract\", " + condStare + " AS \"Stare\", "
                //        + " a.F100699 AS \"SalariuBrut\", MIN(F70102) AS \"IdUser\", x.F71804 AS \"Functia\", A.F100992 AS \"DataFunctie\", a.F10025, h.F70407 AS \"SalariulModifInAvans\"  "
                //        + " FROM \"F100Supervizori\" z "
                //        + " JOIN \"Org_Circuit\" y on z.\"IdSuper\" = -1 * y.\"Super2\" "
                //        + " JOIN  F100 a ON z.F10003 = a.F10003"
                //        + " LEFT JOIN F002 b on a.F10002 = b.F00202 "
                //        + " LEFT JOIN F003 c on a.F10004 = c.F00304 "
                //        + " LEFT JOIN F004 d on a.F10005 = d.F00405 "
                //        + " LEFT JOIN F005 e on a.F10006 = e.F00506 "
                //        + " LEFT JOIN F006 f on a.F10007 = f.F00607 "
                //        + " LEFT JOIN USERS g on a.F10003 = g.F10003 "
                //        + " LEFT JOIN F718 x on a.F10071 = x.F71802 "
                //        + " LEFT JOIN F704 h on a.F10003 = h.F70403 "
                //        + " WHERE y.\"Id\" = (SELECT \"IdCircuit\" FROM \"Org_relFormularCircuit\" WHERE \"IdFormular\" = " + idFormular + ") AND z.\"IdUser\" = " + idUser + " AND F70404 = 1 and F70420 = 0 AND " + dtVig + " >= F70406 "
                //        + " AND " + condDifLuni
                //        + " GROUP BY a.F10003, a.F10008 " + op + " ' ' " + op + " F10009, a.F10017, b.F00204, c.F00305, d.F00406, e.F00507, f.F00608, a.F10022, a.F10023, a.F10011, a.F100699, A.F100992, a.F10025, x.F71804, h.F70407"
                //        + " ORDER BY \"NumeComplet\"";

                if (esteConsRU)                      //cand este consultant RU aratam toti angajatii
                    sql = "SELECT   a.F10003, a.F10008 " + op + " ' ' " + op + " F10009 AS \"NumeComplet\", a.F10017 AS CNP, b.F00204 AS \"Companie\", c.F00305 AS \"Subcompanie\", d.F00406 AS \" Filiala\", "
                        + " e.F00507 AS  \"Sectie\", f.F00608 AS \"Departament\", F10022 as \"DataAngajarii\", F10023 AS \"DataPlecarii\", F10011 AS \"NrContract\", " + condStare + " AS \"Stare\", "
                        + " a.F100699 AS \"SalariuBrut\", MIN(F70102) AS \"IdUser\", x.F71804 AS \"Functia\", A.F100992 AS \"DataFunctie\", a.F10025, null AS \"SalariulModifInAvans\"  FROM  F100 a "
                        + " LEFT JOIN F002 b on a.F10002 = b.F00202 "
                        + " LEFT JOIN F003 c on a.F10004 = c.F00304 "
                        + " LEFT JOIN F004 d on a.F10005 = d.F00405 "
                        + " LEFT JOIN F005 e on a.F10006 = e.F00506 "
                        + " LEFT JOIN F006 f on a.F10007 = f.F00607 "
                        + " LEFT JOIN USERS g on a.F10003 = g.F10003 "
                        + " LEFT JOIN F718 x on a.F10071 = x.F71802 "
                        //+ " LEFT JOIN F704 h on a.F10003 = h.F70403 "
                        //+ " WHERE F70404 = 1 and F70420 = 0 AND " + dtVig + " >= F70406 "
                        + " WHERE 1=1 AND " + condDifLuni
                        + " GROUP BY a.F10003, a.F10008 " + op + " ' ' " + op + " F10009, a.F10017, b.F00204, c.F00305, d.F00406, e.F00507, f.F00608, a.F10022, a.F10023, a.F10011, a.F100699, A.F100992, a.F10025, x.F71804"
                        + " ORDER BY \"NumeComplet\"";
                else
                    sql = "SELECT   a.F10003, a.F10008 " + op + " ' ' " + op + " F10009 AS \"NumeComplet\", a.F10017 AS CNP, b.F00204 AS \"Companie\", c.F00305 AS \"Subcompanie\", d.F00406 AS \" Filiala\", "
                        + " e.F00507 AS  \"Sectie\", f.F00608 AS \"Departament\", F10022 as \"DataAngajarii\", F10023 AS \"DataPlecarii\", F10011 AS \"NrContract\", " + condStare + " AS \"Stare\", "
                        + " a.F100699 AS \"SalariuBrut\", MIN(F70102) AS \"IdUser\", x.F71804 AS \"Functia\", A.F100992 AS \"DataFunctie\", a.F10025, null AS \"SalariulModifInAvans\"  "
                        + " FROM \"F100Supervizori\" z "
                        + " JOIN \"Org_Circuit\" y on z.\"IdSuper\" = -1 * y.\"Super2\" "
                        + " JOIN  F100 a ON z.F10003 = a.F10003"
                        + " LEFT JOIN F002 b on a.F10002 = b.F00202 "
                        + " LEFT JOIN F003 c on a.F10004 = c.F00304 "
                        + " LEFT JOIN F004 d on a.F10005 = d.F00405 "
                        + " LEFT JOIN F005 e on a.F10006 = e.F00506 "
                        + " LEFT JOIN F006 f on a.F10007 = f.F00607 "
                        + " LEFT JOIN USERS g on a.F10003 = g.F10003 "
                        + " LEFT JOIN F718 x on a.F10071 = x.F71802 "
                        //+ " LEFT JOIN F704 h on a.F10003 = h.F70403 "
                        + " WHERE y.\"Id\" = (SELECT \"IdCircuit\" FROM \"Org_relFormularCircuit\" WHERE \"IdFormular\" = " + idFormular + ") AND z.\"IdUser\" = " + idUser 
                        //+ " AND F70404 = 1 and F70420 = 0 AND " + dtVig + " >= F70406 "
                        + " AND " + condDifLuni
                        + " GROUP BY a.F10003, a.F10008 " + op + " ' ' " + op + " F10009, a.F10017, b.F00204, c.F00305, d.F00406, e.F00507, f.F00608, a.F10022, a.F10023, a.F10011, a.F100699, A.F100992, a.F10025, x.F71804"
                        + " ORDER BY \"NumeComplet\"";

                q = General.IncarcaDT(sql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;

        }

        protected void btnSalvare_Click(object sender, EventArgs e)
        {
            try 
            {
                if (cmbFormNou.Value == null || (Convert.ToInt32(cmbFormNou.Value) == 22 && cmbAng.Value == null))
                {
                    MessageBox.Show("Lipsesc date!", MessageBox.icoWarning, "");
                    return;
                }          


                int idFrm = Convert.ToInt32(cmbFormNou.Value ?? -99);
                int idRec = Convert.ToInt32(cmbRecrut.Value ?? -99);
                int f10003 = Convert.ToInt32(cmbAng.Value ?? -99);
                string numeAng = cmbAng.Text;
                int idPost = -99;
                DateTime dtVigoare = Convert.ToDateTime(deDataVig.Value ?? DateTime.Now);

                if (idFrm != -99)
                {
                    if (idFrm == 20 || idFrm == 21) f10003 = Convert.ToInt32(Session["User_Marca"] == DBNull.Value ? "-99" : Session["User_Marca"].ToString());
              
                    int id = AdaugaFormular(Convert.ToInt32(Session["UserId"].ToString()), idFrm, idPost, idRec, dtVigoare, f10003, numeAng, Convert.ToInt32(Session["User_Marca"] == DBNull.Value ? "-99" : Session["User_Marca"].ToString()));
                    
                    if (id == -1 && id == -99)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Eroare la creare referat"), MessageBox.icoError, "Atentie !");
                        return;
                    }            

                    string url = "~/Posturi/FormDetaliu";
                    Session["FormDetaliu_Id"] = id;
                    Session["FormDetaliu_IdFormular"] = idFrm;
                    Session["FormDetaliu_IdStare"] = 0;
                    Session["FormDetaliu_PoateModifica"] = 1;
                    Session["FormDetaliu_EsteNou"] = 1;
                    Session["FormDetaliu_Pozitie"] = 0;
                    Session["FormDetaliu_IdRol"] = 0;

                    Session["FormDetaliu_NumeFormular"] = cmbFormNou.Text;
                    Session["FormDetaliu_DataVigoare"] = dtVigoare;

                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);

                    //switch (idFrm)
                    //{
                    //    case 20:
                    //        Posturi.FormDate20 pag20 = new Posturi.FormDate20();

                    //        pag20.id = id;
                    //        pag20.idFormular = idFrm;
                    //        pag20.idPost = idPost;
                    //        pag20.dtVigoare = dtVigoare;
                    //        pag20.txtData.EditValue = dtVigoare;
                    //        pag20.esteNou = true;
                    //        pag20.titlu = (barTitlu.Content ?? "").ToString();

                    //        pag20.txtForm.EditValue = General.IsNull(dlg.cmbFrm.Text, "");

                    //        pag20.lblData.Visibility = System.Windows.Visibility.Collapsed;
                    //        pag20.txtData.Visibility = System.Windows.Visibility.Collapsed;

                    //        pag20.txtPost.IsEnabled = true;

                    //        pag20.lblAng.Visibility = System.Windows.Visibility.Collapsed;
                    //        pag20.cmbAng.Visibility = System.Windows.Visibility.Collapsed;

                    //        pag20.dds.QueryParameters.Clear();
                    //        pag20.dds.QueryParameters.Add(new Parameter { ParameterName = "id", Value = id });

                    //        pnl.Content = pag20;
                    //        break;
                    //    case 21:
                    //        Posturi.FormDate21 pag21 = new Posturi.FormDate21();

                    //        pag21.id = id;
                    //        pag21.idFormular = idFrm;
                    //        pag21.idPost = idPost;
                    //        pag21.dtVigoare = dtVigoare;
                    //        pag21.esteNou = true;
                    //        pag21.titlu = (barTitlu.Content ?? "").ToString();

                    //        pag21.txtForm.EditValue = General.IsNull(dlg.cmbFrm.Text, "");

                    //        pag21.txtPost.IsEnabled = true;

                    //        pag21.lblAng.Visibility = System.Windows.Visibility.Collapsed;
                    //        pag21.txtAng.Visibility = System.Windows.Visibility.Collapsed;

                    //        pag21.dds.QueryParameters.Clear();
                    //        pag21.dds.QueryParameters.Add(new Parameter { ParameterName = "id", Value = id });

                    //        pnl.Content = pag21;
                    //        break;
                    //}


                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public int AdaugaFormular(int idUser, int idFormular, int? idPost, int? idRec, DateTime dtVigoare, int f10003_Ang, string numeAng, int f10003)
        {
            int idUrm = 1;        

            try
            {
                string sql = "SELECT MAX(\"Id\") + 1 FROM \"Org_Date\" ";
                DataTable dtFor = General.IncarcaDT(sql, null);

                if (dtFor != null && dtFor.Rows.Count > 0 && dtFor.Rows[0][0] != DBNull.Value)
                    idUrm = Convert.ToInt32(dtFor.Rows[0][0].ToString());


                sql = "SELECT * FROM \"Org_relFormularCircuit\" WHERE \"IdFormular\" =  " + idFormular;
                DataTable dtFormCirc = General.IncarcaDT(sql, null);

                if (dtFormCirc == null || dtFormCirc.Rows.Count <= 0 || dtFormCirc.Rows[0]["IdCircuit"] == DBNull.Value)
                    return -123;
           
                int idCircuit = Convert.ToInt32(dtFormCirc.Rows[0]["IdCircuit"].ToString());
                sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = 1";
                DataTable dtCul = General.IncarcaDT(sql, null);
                string culoare = "#FFFFFFFF";
                if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value)
                    culoare = dtCul.Rows[0]["Culoare"].ToString();


                #region IstoricCircuit

                //procesul este asemanator cu cel de la Absente

                //cazul Generali
                //pentru a nu face modificari codului, in cazul in care formularul este 1 sau 2, f10003 este chiar utilizatorul logat, adica managerul


                //obtinem campurile care depind de circuit
                sql = "SELECT * FROM \"Org_Circuit\" WHERE \"Id\" =  " + idCircuit;
                DataTable dtCirc = General.IncarcaDT(sql, null);

                int total = 0;
                int idStare = -99;
                int pozUser = 1;

                //aflam totalul de users din circuit
                for (int i = 1; i <= 20; i++)
                {
                    if (dtCirc.Rows[0]["Super" + i] != DBNull.Value)
                    {
                        int idSuper = Convert.ToInt32(dtCirc.Rows[0]["Super" + i].ToString());
                        if (Convert.ToInt32(idSuper) != -99)
                        {
                            //ne asiguram ca exista user pentru supervizorul din circuit
                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                int idSpr = Convert.ToInt32(idSuper);
                                sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + f10003;
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

                //adaugam in tabela Ptj_CereriIstoric
                int poz = 0;
                int idUserPrece = -99;
                int idUserCalc = -99;
                bool adaugat = false;

                List<int> lst = new List<int>();

                for (int i = 1; i <= 20; i++)
                {
                    string aprobat = "NULL", dataAprobare = "NULL";
                    int idSuper = -99;
                    idStare = -99;
                    if (dtCirc.Rows[0]["Super" + i] != DBNull.Value)
                    {
                        idSuper = Convert.ToInt32(dtCirc.Rows[0]["Super" + i].ToString());
                        if (Convert.ToInt32(idSuper) != -99)
                        {
                            //IdUser
                            if (Convert.ToInt32(idSuper) == 0)
                            {
                                //idUserCalc = idUser;
                                sql = "SELECT * FROM USERS WHERE F10003 = " + f10003;
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser != null && dtUser.Rows.Count > 0 && dtUser.Rows[0]["F70102"] != DBNull.Value)
                                {
                                    idUserCalc = Convert.ToInt32(dtUser.Rows[0]["F70102"].ToString());
                                }
                            }
                            if (Convert.ToInt32(idSuper) > 0) idUserCalc = Convert.ToInt32(idSuper);
                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                int idSpr = Convert.ToInt32(idSuper);
                                //verif. daca nu cumva user-ul logat este deja un superviozr pt acest angajat;
                                //astfel se rezolva problema cand, de exemplu, un angajat are mai multi AdminRu
                                sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + f10003 + " AND \"IdUser\" = " + idUser;
                                DataTable dtUserLogat = General.IncarcaDT(sql, null);
                                if (dtUserLogat != null && dtUserLogat.Rows.Count > 0 && dtUserLogat.Rows[0]["IdUser"] != DBNull.Value)
                                {
                                    idUserCalc = Convert.ToInt32(dtUserLogat.Rows[0]["IdUser"].ToString());
                                }
                                else
                                {
                                    //ne asiguram ca exista user pentru supervizorul din circuit
                                    sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + f10003;
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
                            }

                            if (Convert.ToInt32(idSuper) < 0)
                            {
                                int idSpr = Convert.ToInt32(idSuper);

                                sql = "SELECT * FROM \"F100Supervizori\" WHERE \"IdSuper\" =  -1 * " + idSpr + " AND F10003 = " + f10003;
                                DataTable dtUser = General.IncarcaDT(sql, null);
                                if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == DBNull.Value)
                                {
                                    continue;
                                }
                            }

                            int paramCumul = 0;                            
                            paramCumul = Convert.ToInt32(Dami.ValoareParam("CumulareAcelasiSupervizor", "0"));   

                            switch (paramCumul)
                            {
                                case 0:                                     //se pun toti supervizorii chiar daca se repeta; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  3;   8;   3;   9;
                                    {
                                        poz += 1;     
                                        if (idUserCalc == idUser && !adaugat)
                                        {
                                            pozUser = poz;
                                            if (poz == 1) idStare = 1;
                                            if (poz == total) idStare = 3;

                                            aprobat = "1";
                                            dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                            adaugat = true;
                                        }
                                        idUserPrece = idUserCalc;
                                    }
                                    break;
                                case 1:                                     //daca uramtorul in circuit este acelasi user, se salveaza doar o singura data; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;   3;   9;
                                    {
                                        if (idUserCalc != idUserPrece)
                                        {
                                            poz += 1;
                                            if (idUserCalc == idUser && !adaugat)
                                            {
                                                pozUser = poz;
                                                if (poz == 1) idStare = 1;
                                                if (poz == total) idStare = 3;

                                                aprobat = "1";
                                                dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                                adaugat = true;
                                            }
                                            idUserPrece = idUserCalc;
                                        }
                                        else
                                        {
                                            total--;
                                            if (idUserCalc == idUser && poz == total) idStare = 3;
                                        }
                                    }
                                    break;
                                case 2:                                     //user-ul se salveaza doar o singura data indiferent de cate ori este pe circuit sau pe ce pozitie este;  ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;  9;
                                    {
                                        poz += 1;

                                        bool esteMUlti = false;
                                        int entMulti = lst.Where(p => p == idUserCalc).Count();
                                        if (entMulti > 0) esteMUlti = true;
                                        if (idUserCalc == idUser && esteMUlti == false)
                                        {
                                            pozUser = poz;
                                            if (poz == 1) idStare = 1;
                                            if (poz == total) idStare = 3;

                                            aprobat = "1";
                                            dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                     
                                        }
                                        idUserPrece = idUserCalc;

                                        lst.Add(idUserCalc);
                                    }
                                    break;
                            }
                        }
                        string strSqlIst = "INSERT INTO \"Org_DateIstoric\"(\"Id\", \"IdCircuit\", \"IdPost\", \"IdUser\", \"IdStare\",  \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", \"Inlocuitor\", \"IdUserInlocuitor\", \"IdRol\",  USER_NO, TIME) "
                                + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13})";
                        strSqlIst = string.Format(strSqlIst, idUrm, idCircuit, idPost == null ? "NULL" : idPost.ToString(), idUserCalc, (idStare == -99 ? "NULL" : idStare.ToString()), poz, ("(SELECT CASE WHEN \"Culoare\" IS NULL THEN '#FFFFFFFF' ELSE \"Culoare\" END FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare + ")"),
                            aprobat, dataAprobare, "0", "NULL", idSuper, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                        General.ExecutaNonQuery(strSqlIst, null);
                    } 
                }
                #endregion

                string dataInceput = (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtVigoare.Day.ToString().PadLeft(2, '0') + "/" + dtVigoare.Month.ToString().PadLeft(2, '0') + "/" + dtVigoare.Year.ToString() + "', 103)" :
                    "TO_DATE('" + dtVigoare.Day.ToString().PadLeft(2, '0') + "/" + dtVigoare.Month.ToString().PadLeft(2, '0') + "/" + dtVigoare.Year.ToString() + "', 'dd/mm/yyyy')");
                string strSql = "INSERT INTO \"Org_Date\"(\"Id\", \"IdFormular\", \"IdStare\", \"IdPost\", \"UserIntrod\", \"Inlocuitor\", \"IdCerereRecrutare\", \"IdCircuit\", \"TotalCircuit\", \"Culoare\", \"Pozitie\", \"DataInceput\", F10003, \"Observatii\", \"F10003Candidat\", USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', {10}, {11}, {12}, '{13}', {14}, {15}, {16})";
                strSql = string.Format(strSql, idUrm, idFormular, "1", idPost == null ? "NULL" : idPost.ToString(), idUser, "NULL",
                    idRec == null ? "NULL" : idRec.ToString(), idCircuit, total, culoare, pozUser, dataInceput, f10003, "NULL", "NULL", Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));

                General.ExecutaNonQuery(strSql, null);

                //adaugam datele in formular
                switch (idFormular)
                {
                    case 20:
                        {
                            string sqlGen = "INSERT INTO \"Org_DateGenerale\" (\"Id\", USER_NO, TIME) VALUES (" + idUrm + ", " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") +")";
                            General.ExecutaNonQuery(sqlGen, null);
                        }
                        break;
                    case 21:
                        {
                            if (idRec != -99)
                            {
                                sql = "SELECT * FROM \"Org_DateGenerale\" WHERE \"Id\" = " +idRec;
                                DataTable dtRec = General.IncarcaDT(sql, null);
                                if (dtRec != null && dtRec.Rows.Count > 0)
                                {
                                    string sqlGen = "INSERT INTO \"Org_DateGenerale\" (\"Id\", \"PostDenumire\", \"FunctieCOR\", \"ContractDurata\", \"TipContract\", \"NrOre\", \"Locatie\", \"Salariul\", \"CentruCost\", " 
                                        + " \"Companie\", \"Subcompanie\", \"Filiala\", \"Sectie\", \"Dept\", \"Masina\", \"Calculator\", \"Telefon\", \"CertificatVPN\", USER_NO, TIME) "
                                        + " SELECT " + idUrm + ", \"PostDenumire\", \"FunctieCOR\", \"ContractDurata\", \"TipContract\", \"NrOre\", \"Locatie\", \"Salariul\", \"CentruCost\", "
                                        + " \"Companie\", \"Subcompanie\", \"Filiala\", \"Sectie\", \"Dept\", 0, 1, 1, 0, " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + " FROM \"Org_DateGenerale\" WHERE \"Id\" = " + idRec;
                                    General.ExecutaNonQuery(sqlGen, null);
                                }
                                else
                                {
                                    string sqlGen = "INSERT INTO \"Org_DateGenerale\" (\"Id\", \"Masina\", \"Calculator\", \"Telefon\", \"CertificatVPN\", USER_NO, TIME) VALUES (" + idUrm + ", 0, 1, 1, 0, " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ")";
                                    General.ExecutaNonQuery(sqlGen, null);
                                }
                            }
                            else
                            {
                                string sqlGen = "INSERT INTO \"Org_DateGenerale\" (\"Id\", \"Masina\", \"Calculator\", \"Telefon\", \"CertificatVPN\", USER_NO, TIME) VALUES (" + idUrm + ", 0, 1, 1, 0, " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ")";
                                General.ExecutaNonQuery(sqlGen, null);
                            }                         
                        }
                        break;
                    case 22:
                        {
                            string sqlGen = "INSERT INTO \"Org_DateGenerale\" (\"Id\", \"DataVigoare\", \"DataInceput\", F10003, \"NumeComplet\", USER_NO, TIME) VALUES (" 
                                + idUrm + ", " + dataInceput + ", " + dataInceput + ", " + f10003_Ang + ", '" + numeAng + "', " + Session["UserId"].ToString() + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ")";
                            General.ExecutaNonQuery(sqlGen, null);
                        }
                        break;
                }           
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return idUrm;
        }



        protected void pnlCtl_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {

                DataTable dtAng = GetF100NumeCompletPost(Convert.ToInt32(cmbFormNou.Value ?? -99), Convert.ToInt32(Session["UserId"].ToString()), Convert.ToDateTime(deDataVig.Value ?? DateTime.Now));
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                int idFrm = Convert.ToInt32(e.Parameter);
                switch (idFrm)
                {
                    case 20:
                        lblDataVig.Visible = false;
                        deDataVig.ClientVisible = false;
                        deDataVig.Value = null;
                        lblRecrut.Visible = false;
                        cmbRecrut.ClientVisible = false;
                        cmbRecrut.Value = null;
                        lblAng.Visible = false;
                        cmbAng.ClientVisible = false;
                        cmbAng.Value = null;
                        break;
                    case 21:
                        lblDataVig.Visible = false;
                        deDataVig.ClientVisible = false;
                        deDataVig.Value = null;
                        lblRecrut.Visible = false; //true
                        cmbRecrut.ClientVisible = false; //true
                        cmbRecrut.Value = null;
                        lblAng.Visible = false;
                        cmbAng.ClientVisible = false;
                        cmbAng.Value = null;
                        break;
                    case 22:
                        lblDataVig.Visible = true;
                        deDataVig.ClientVisible = true;
                        deDataVig.Value = null;
                        lblRecrut.Visible = false;
                        cmbRecrut.ClientVisible = false;
                        cmbRecrut.Value = null;
                        lblAng.Visible = true;
                        cmbAng.ClientVisible = true;
                        cmbAng.Value = null;
                        break;
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        public string AnuleazaFormular(int idUser, string ids, int total, int f10003)
        {
            string msg = "";

            try
            {
                if (ids == "") return "Nu exista cereri pentru aceasta actiune !";            

                int nr = 0;
                string[] arr = ids.Split(new string[] { ";" }, StringSplitOptions.None);

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    if (arr[i] != "")
                    {
                        int id = -99;

                        try
                        {
                            id = Convert.ToInt32(arr[i]);
                        }
                        catch (Exception)
                        {
                        }

                        if (id != -99)
                        {

                            string sql = "SELECT * FROM \"Org_Date\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCer = General.IncarcaDT(sql, null);

                            if (dtCer != null && dtCer.Rows.Count > 0)
                            {
                                sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = -1";
                                DataTable dtCul = General.IncarcaDT(sql, null);
                                string culoare = "#FFFFFFFF";
                                if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value)
                                    culoare = dtCul.Rows[0]["Culoare"].ToString();

                                //schimbam statusul
                                General.ExecutaNonQuery("UPDATE \"Org_Date\" SET \"IdStare\" = -1, \"Culoare\" = '" + culoare + "' WHERE \"Id\" = " + id, null);

                                //introducem o linie de anulare in Ptj_CereriIstoric
                                sql = "INSERT INTO \"Org_DateIstoric\" (\"Id\", \"IdCircuit\", \"IdUser\", \"IdStare\", \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", USER_NO, TIME, \"Inlocuitor\") "
                                    + " VALUES (" + id + ", " + dtCer.Rows[0]["IdCircuit"].ToString() + ", " + idUser + ", -1, 22, '" + culoare + "', 1, " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE")
                                    + ", " + idUser + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ", 0) ";
                                General.ExecutaNonQuery(sql, null);


                                //stergem din relatie Post Angajat, si actualizam data de sfarsit a penultimei linii (procesul in oglinda din functia ActualizeazaInRel
                                //var entRel = ctx.Org_relPostAngajat.Where(p => p.F10003 == ent.F10003 && p.IdPost == ent.IdPost && p.DataReferinta == ent.DataInceput).FirstOrDefault();
                                //if (entRel != null)
                                //{
                                //    var entOld = ctx.Org_relPostAngajat.Where(p => p.F10003 == ent.F10003 && p.IdPost == ent.IdPost && p.DataSfarsit == ent.DataInceput.Value.AddDays(-1)).FirstOrDefault();
                                //    if (entOld != null)
                                //    {
                                //        entOld.DataSfarsit = new DateTime(2100, 1, 1);
                                //    }
                                //    ctx.Org_relPostAngajat.DeleteObject(entRel);
                                //}  


                                string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                                int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);

                                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                                {
                                    NotifAsync.TrimiteNotificare("Posturi.FormLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", -1 AS ""IdStareViitoare"" FROM Org_Date Z WHERE Id=" + id.ToString(), "Org_Date", id, idUser, marcaUser, arrParam);
                                });

                                //msg = "Proces finalizat cu succes";
                                nr++;
                            }
                           
                        }
                    }
                }

                if (nr > 0)
                {
                    msg = "S-au anulat " + nr.ToString() + " cereri din " + total + " !";
            
                }
                else
                {
                    msg = "Nu exista cereri pentru aceasta actiune !";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name); 
            }

            return msg;
        }

        protected void btnAprobareHR_Click(object sender, EventArgs e)
        {
            MetodeCereri(4);
        }
    }
}