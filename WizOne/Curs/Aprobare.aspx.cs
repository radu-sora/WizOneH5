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
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Curs
{
    public partial class Aprobare : System.Web.UI.Page
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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnRespinge.Text = Dami.TraduCuvant("btnRespinge", "Respinge");
                btnAproba.Text = Dami.TraduCuvant("btnAproba", "Aproba");               
                //btnSendWaitingList.Text = Dami.TraduCuvant("btnSendWaitingList", "Trimite Asteptare");
                btnIncludeParticipantsList.Text = Dami.TraduCuvant("btnIncludeParticipantsList", "Include Participanti");

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");  
                
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblRol.InnerText = Dami.TraduCuvant("Rol");
                lblStare.InnerText = Dami.TraduCuvant("Stare");

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
                dt.Rows.Add(0, "Toate (inclusiv nivelurile inferioare)");
                dt.Rows.Add(1, "Doar lista asteptare");
                dt.Rows.Add(2, "Fara lista asteptare");

                if (!IsPostBack)
                    cmbRol.Value = 0;
                cmbRol.DataSource = dt;
                cmbRol.DataBind();

                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;


                DataTable dtCurs = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCurs"" ", null);
                GridViewDataComboBoxColumn colCurs = (grDate.Columns["IdCurs"] as GridViewDataComboBoxColumn);
                colCurs.PropertiesComboBox.DataSource = dtCurs;

                DataTable dtAng = GetCursAngajati(2, Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()));
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                IncarcaGrid();



                string verifNumarParticipantiSesiune = Dami.ValoareParam("verifNumarParticipantiSesiune", "0");
                if (Convert.ToInt32(verifNumarParticipantiSesiune) == 0)
                {
                    btnIncludeParticipantsList.ClientEnabled = false;
                    //btnSendWaitingList.ClientEnabled = false;
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
                cmbAng.Value = null;
                cmbRol.Value = 1;
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


        protected void btnSendWaitingList_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnIncludeParticipantsList_Click(object sender, EventArgs e)
        {
            try
            {
                MetodeCereri(4);
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
                int aleMele = Convert.ToInt32(cmbRol.Value ?? 1);
                int F10003 = Convert.ToInt32(cmbAng.Value ?? -99);     

                grDate.KeyFieldName = "IdAuto";

                dt = GetCurs_CereriAprobare(F10003, Convert.ToInt32(Session["UserId"].ToString()), aleMele, DamiStari());

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["AprobareCurs_Grid"] = dt;
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
                                object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "IdCurs", "F10003", "UserIntrod", "IdSesiune" }) as object[];
                                AnuleazaCerereCurs(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(lst[0] ?? "-99"), 2);
                                IncarcaGrid();
                            }
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

            try
            {
                int nrSel = 0;
                string ids = "", idsAtr = "", lstDataModif = "", lstMarci = "";
                string comentarii = "";
                string msg = "";

                int idCurs = -99;
                int idSesiune = -99;
                string msgErrorValidAprobare = "Inregistrarile ";

                object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "IdCurs", "F10003", "UserIntrod", "IdSesiune" }) as object[];
                if (lst == null || lst.Count() == 0)
                {
                    if (tipMsg == 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }   

                //Angajatul nu poate aproba proria cerere
                if (Convert.ToInt32(Session["User_Marca"].ToString()) != Convert.ToInt32(lst[3].ToString()))
                {
                    if ((lst[1] ?? "0").ToString() == "1" || (lst[1] ?? "0").ToString() == "2" || (lst[1] ?? "0").ToString() == "3") ids += lst[0].ToString() + ";";
                    nrSel += 1;
                }

                if (tipActiune == 1)
                {
                    idCurs = Convert.ToInt32(lst[2].ToString());
                    idSesiune = Convert.ToInt32(lst[5].ToString());

                    DataTable dtSesiune = GetmetaCurs_tblCursSesiune(idSesiune, idCurs);
                    if (dtSesiune != null && dtSesiune.Rows.Count > 0)
                    {
                        if (!(Convert.ToDateTime(dtSesiune.Rows[0]["DataInceputAprobare"].ToString()) <= DateTime.Now && Convert.ToDateTime(dtSesiune.Rows[0]["DataSfarsitAprobare"].ToString()) >= DateTime.Now))
                        {
                            DataTable dtAng = GetF100(lst[3].ToString());
                            if (dtAng != null && dtAng.Rows.Count > 0)
                                msgErrorValidAprobare = msgErrorValidAprobare + " " + dtAng.Rows[0]["NumeComplet"].ToString() + " pt. sesiunea " + dtSesiune.Rows[0]["Denumire"].ToString() + "; ";                  
                        }
                    }
                }

                if (msgErrorValidAprobare != "Inregistrarile " && tipActiune == 1)
                {
                    msgErrorValidAprobare = msgErrorValidAprobare + "nu pot fi aprobate, deoarece data nu se incadreaza in perioada de aprobare a sesiunii!";

                    if (tipMsg == 0)
                        MessageBox.Show(msgErrorValidAprobare, MessageBox.icoWarning, "Validare aprobare conform data sfarsit sesiune");
                    else
                        grDate.JSProperties["cpAlertMessage"] = msgErrorValidAprobare;
             
                    return;
                }

                if (nrSel == 0)
                {
                    if (tipMsg == 0)
                        MessageBox.Show("Nu exista date selectate", MessageBox.icoWarning, "");
                    else
                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }


                //if (tipActiune == 1 || tipActiune == 2)
                    msg = msg + AprobaCurs(Convert.ToInt32(Session["UserId"].ToString()), ids, nrSel, tipActiune, General.ListaCuloareValoare()[5], false);

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


        public string AprobaCurs(int idUser, string ids, int total, int actiune, string culoareValoare, bool HR)
        {
            //actiune  1  - aprobare
            //actiune  2  - respingere
            /* include lista asteptare/participanti*/
            //actiune 3 - include lista asteptare
            //actiune 4 - include lista participanti


            string msg = "";
            string msgValid = "";

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
                            string sql = "SELECT * FROM \"Curs_Inregistrare\" WHERE \"Id\" = " + id.ToString();
                            DataTable dtCur = General.IncarcaDT(sql, null);
                            sql = "SELECT * FROM \"Curs_CereriIstoric\" WHERE \"IdCerere\" = " + id.ToString()
                            +  " AND \"IdUser\" = " + idUser.ToString()
                             +  " AND \"Aprobat\" IS NULL" ;
                            DataTable dtIst = General.IncarcaDT(sql, null);

                            if (dtIst == null || dtIst.Rows.Count == 0) continue;

                            int eListaAsteptare_Curs = Convert.ToInt32(dtCur.Rows[0]["eListaAsteptare"].ToString());
                            int eListaAsteptare_Ist = Convert.ToInt32(dtIst.Rows[0]["eListaAsteptare"].ToString());

                            if (actiune == 3)
                                eListaAsteptare_Curs = 1;
                            else if (actiune == 4)
                            {
                                msgValid = Curs_VerificaNumarMaximParticipanti(Convert.ToInt32(dtCur.Rows[0]["IdCurs"].ToString()), Convert.ToInt32(dtCur.Rows[0]["IdSesiune"].ToString()));
                                if (msgValid != "")
                                    return msgValid;
                                else
                                {
                                    eListaAsteptare_Curs = 0;
                                    eListaAsteptare_Ist = 0;
                                }
                            }

                            int idStare = 2;
     
                            if (idStare == 2 && dtCur.Rows[0]["TotalSuperCircuit"].ToString() == dtIst.Rows[0]["Pozitie"].ToString()) idStare = 3;

                            if (actiune == 2)
                                idStare = 0;  

                            string culoare = "";
                            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare.ToString();
                            DataTable dtCul = General.IncarcaDT(sql, null);
                            if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                                culoare = dtCul.Rows[0]["Culoare"].ToString();
                            else
                                culoare = "#FFFFFFFF";


                            sql = "UPDATE \"Curs_Inregistrare\" SET \"Pozitie\" = " + dtIst.Rows[0]["Pozitie"].ToString() + ", \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare 
                                + "', eListaAsteptare = " + eListaAsteptare_Curs + " WHERE \"Id\" = " + id.ToString();
                            General.IncarcaDT(sql, null);
     
                            sql = "UPDATE \"Curs_CereriIstoric\" SET \"DataAprobare\" = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", \"Aprobat\" = 1, \"IdStare\" = " + idStare.ToString() + ", \"Culoare\" = '" + culoare
                            + "', USER_NO = " + idUser.ToString() + ", TIME = " + (Constante.tipBD == 1 ? "getdate()" : "sysdate") + ", eListaAsteptare =  " + eListaAsteptare_Ist
                            + " WHERE \"IdCerere\" = " + id.ToString()
                            + " AND \"IdUser\" = " + idUser.ToString()
                             + " AND \"Pozitie\"=" + dtIst.Rows[0]["Pozitie"].ToString();                            

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


    
                            #region  Notificare start

                            //ctxNtf.TrimiteNotificare("Avs.Aprobare", "grDate", entCer, idUser, f10003);

                            #endregion

                            nr++;
                            
                        }
                    }
                }

                if (nr > 0)
                {
                    switch (actiune)
                    {
                        case 1:
                            msg = "S-au aprobat " + nr.ToString() + " cursuri din " + total + " !";
                            break;
                        case 2:
                            msg = "S-au respins " + nr.ToString() + " cursuri din " + total + " !";
                            break;
                        case 3:
                            msg = "S-au trimis in lista asteptare  " + nr.ToString() + " participanti din " + total + " !";
                            break;
                        case 4:
                            msg = "S-au trimis in lista de participare " + nr.ToString() + " participanti din " + total + " !";
                            break;
                    }
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

        public DataTable GetCurs_CereriAprobare(int f10003, int idUser, int tip, string filtruStari)
        {
            //tip
            /* modificare tip filtre 
             * 0 - toate
             * 1 - lista asteptare
             * 2 - fara lista asteptare
             * */
            //0  -  Toate
            //1  -  Ale mele

            DataTable q = null;

            try
            {
                string strSql = "";
                string filtru = "";

                if (f10003 != -99) filtru += " AND a.F10003 = " + f10003;
                if (filtruStari != "") filtru += " AND a.\"IdStare\" IN (" + filtruStari.Replace(";", ",").Substring(0, filtruStari.Length) + ")";

                string idHR = Dami.ValoareParam("Avans_IDuriRoluriHR", "-99");
                if (tip == 1 || tip == 2)
                {
                    filtru += " AND ((F.\"IdUser\" = " + idUser + " AND (A.\"Pozitie\" + 1) = F.\"Pozitie\") OR  " + idUser + " IN (select sup.\"IdUser\" FROM \"F100Supervizori\" sup where sup.\"IdSuper\" in (" + idHR + ")) )";
                    filtru += " AND A.\"eListaAsteptare\" = " + (tip == 2 ? 1 : 0);
                }
                else
                    filtru += " AND ((F.\"IdUser\" = " + idUser + " AND (A.\"Pozitie\" <= F.\"Pozitie\" or A.\"IdStare\" = 3 or A.\"IdStare\" = 0)) OR " + idUser + " IN (select sup.\"IdUser\" FROM \"F100Supervizori\" sup where sup.\"IdSuper\" in (" + idHR + ")) ) ";

                string lstCampuri = " a.\"Culoare\", a.F10003, a.\"Id\", a.\"IdAuto\", a.\"IdCircuit\", a.\"IdCurs\", a.\"IdSesiune\" , a.\"IdStare\", "
                    + "   a.TIME, a.USER_NO, a.\"UserIntrod\",  "
                    + " case when a.\"eListaAsteptare\" = 1 then 'Lista Asteptare' else '---' end as \"eListaAsteptare\" ";

                strSql = @"SELECT distinct {1} FROM ""Curs_Inregistrare"" A
                        INNER JOIN ""Curs_CereriIstoric"" F ON A.""Id""=F.""IdCerere""
                        WHERE 1=1 {0}
                        ORDER BY A.TIME";

                strSql = string.Format(strSql, filtru, lstCampuri);
                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public DataTable GetCursAngajati(int Tip, int IdUser, int f10003)
        {
            DataTable q = null;

            try
            {
                string inn1 = "";
                string inn2 = "";
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                string strSql = @"SELECT A.F10003, A.F10008 {4} ' ' {4} A.F10009 AS ""NumeComplet"", A.F10017 AS CNP, A.F10022 AS ""DataAngajarii"",
                                A.F10011 AS ""NrContract"", CAST(A.F10043 AS int) AS ""Norma"",A.F100901, CASE WHEN (A.F10025 = 0 OR A.F10025=999) THEN 1 ELSE 0 END AS ""AngajatActiv"",
                                X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament""
                                FROM (
                                SELECT A.F10003
                                FROM F100 A
                                WHERE A.F10003 = {0}
                                /*LeonardM 08.06.2016
                                filtrare angajati activi*/
                                and A.F10025 in (0, 999)
                                /*end LeonardM 08.06.2016*/
                                UNION
                                SELECT A.F10003
                                FROM F100 A
                                INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                                INNER JOIN ""Curs_Circuit"" C ON {2}
                                INNER JOIN ""relGrupAngajat"" D ON B.F10003=D.F10003 AND C.""IdGrupAngajat""=D.""IdGrup""
                                WHERE B.""IdUser""= {1}
                                /*LeonardM 08.06.2016
                                filtrare angajati activi*/
                                and A.F10025 in (0, 999)
                                /*end LeonardM 08.06.2016*/
                                UNION
                                SELECT A.F10003
                                FROM F100 A
                                INNER JOIN ""Curs_Circuit"" C ON  {3}
                                INNER JOIN ""relGrupAngajat"" D ON A.F10003=D.F10003 AND C.""IdGrupAngajat""=D.""IdGrup"") B
                                INNER JOIN F100 A ON A.F10003=B.F10003
                                LEFT JOIN F718 X ON A.F10071=X.F71802
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                /*LeonardM 08.06.2016
                                filtrare angajati activi*/
                                and A.F10025 in (0, 999)
                                /*end LeonardM 08.06.2016*/";

                inn1 = @" B.""IdSuper"" = -1 * c.""Super1"" OR B.""IdSuper"" = -1 * c.""Super2"" OR B.""IdSuper"" = -1 * c.""Super3"" OR B.""IdSuper"" = -1 * c.""Super4"" OR B.""IdSuper"" = -1 * c.""Super5"" OR B.""IdSuper"" = -1 * c.""Super6""  OR B.""IdSuper"" = -1 * c.""Super7"" OR B.""IdSuper"" = -1 * c.""Super8"" OR B.""IdSuper"" = -1 * c.""Super9"" OR B.""IdSuper"" = -1 * c.""Super10"" OR B.""IdSuper"" = -1 * c.""Super11"" OR B.""IdSuper"" = -1 * c.""Super12"" OR B.""IdSuper"" = -1 * c.""Super13"" OR B.""IdSuper"" = -1 * c.""Super14"" OR B.""IdSuper"" = -1 * c.""Super15"" OR B.""IdSuper"" = -1 * c.""Super16"" OR B.""IdSuper"" = -1 * c.""Super17"" OR B.""IdSuper"" = -1 * c.""Super18"" OR B.""IdSuper"" = -1 * c.""Super19"" OR B.""IdSuper"" = -1 * c.""Super20"" ";
                inn2 = @" c.""Super1"" = {0}  OR c.""Super2"" = {0}  OR c.""Super3"" = {0}  OR c.""Super4"" = {0}  OR c.""Super5"" = {0}  OR c.""Super6"" = {0}  OR c.""Super7"" = {0}  OR c.""Super8"" = {0}  OR c.""Super9"" = {0}  OR c.""Super10"" = {0} OR c.""Super11"" = {0} OR c.""Super12"" = {0}  OR c.""Super13"" = {0}  OR c.""Super14"" = {0}  OR c.""Super15"" = {0}  OR c.""Super16"" = {0}  OR c.""Super17"" = {0}  OR c.""Super18"" = {0}  OR c.""Super19"" = {0}  OR c.""Super20"" = {0} ";
                inn2 = string.Format(inn2, IdUser);

                strSql = string.Format(strSql, f10003, IdUser, inn1, inn2, op);
                q = General.IncarcaDT(strSql, null);

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
                

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        public string AnuleazaFormular(int idUser, int id, int f10003)
        {
            string msg = "";

            try
            {
                if (id == -99) return msg;

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
                    sql = "INSERT INTO \"Org_DateIstoric\" (\"Id\", \"IdCircuit\", \"IdUser\", \"IdStare\", \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", USER_NO, TIME, \"Inlocuitor\", \"IdRol\") "
                        + " VALUES (" + id + ", " + dtCer.Rows[0]["IdCircuit"].ToString() + ", " + idUser + ", -1, 22, '" + culoare + "', 1, "  + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") 
                        + ", " + idUser + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ", 0, " + (dtCer.Rows[0]["IdRol"] == DBNull.Value ? "NULL" : dtCer.Rows[0]["IdRol"].ToString()) + ") ";
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


                    msg = "Proces finalizat cu succes";

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name); 
            }

            return msg;
        }


        public DataTable GetmetaCurs_tblCursSesiune(int idSesiune, int idCurs)
        {
           DataTable q = null;

            try
            {
                string sql = "SELECT a.\"Buget\", a.\"Bugetat\", a.\"CategSesiune\", a.\"CodBuget\", a.\"CostEfectiv\", a.\"CostEstimat\", a.\"DataExpirare\", a.\"DataInceput\", a.\"DataSfarsit\", "
                    + " a.\"Denumire\", a.\"Id\", a.\"IdAuto\", a.\"IdCurs\", a.\"IdMoneda\", a.\"IdQuiz\", a.\"IdStare\", a.\"InternExtern\", a.\"Locatie\", a.\"Materiale\", a.\"NrMax\", a.\"NrMin\", "
                    + " a.\"Observatii\", a.\"OraInceput\", a.\"OraSfarsit\", a.\"OrganizatorId\", a.\"OrganizatorNume\", a.\"TematicaId\", a.\"TematicaNume\", a.TIME, a.\"TipSesiune\", a.\"TotalOre\", "
                    + " a.\"TrainerId\", a.\"TrainerNume\", a.USER_NO, b.\"Categ_Niv1Id\", b.\"Categ_Niv2Id\", a.\"OrePauzaMasa\", a.\"DataInceputAprobare\", a.\"DataSfarsitAprobare\" "
                    + " FROM \"Curs_tblCursSesiune\" a "
                    + " LEFT JOIN \"Curs_tblCurs\" b ON a.\"IdCurs\" = b.\"Id\" "
                    + " WHERE a.\"Id\" = " + idSesiune + " AND a.\"IdCurs\" = " + idCurs;

                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetF100(string f10003)
        {
            DataTable q = null;

            try
            {
                string strSql = @"SELECT F10003, F10008 {0} ' ' {0} F10009 AS ""NumeComplet"",  " +
                                @" F10008 {0} ' ' {0} F10009 {0} ' | ' {0} {2} F10022 {3} as ""Descriere"" " +
                                @" FROM F100 WHERE F10022 <= {1} AND {1} <= F10023 AND F10003 = {4} ORDER BY F10008, F10009";
                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, "+", "GetDate()", "convert(varchar(10), ", ",3)", f10003);
                else
                    strSql = string.Format(strSql, "||", "sysdate", "to_char(", ", 'dd/mm/yyyy')", f10003);

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public string Curs_VerificaNumarMaximParticipanti(int idCurs, int idSesiune)
        {
            string msg, strSQL = string.Empty;
            DataTable result = null;
            msg = "";
            try
            {
                strSQL = @"select count(*) as ""NrParticipanti"", {0} ses.""NrMax"", 0) as ""NrMax""
                         from ""Curs_Inregistrare"" inreg
                         join ""Curs_tblCursSesiune"" ses on inreg.""IdCurs"" = ses.""IdCurs"" and inreg.""IdSesiune"" = ses.""Id""
                         where inreg.""IdCurs"" = {1} and inreg.""IdSesiune"" = {2}
                         and inreg.""eListaAsteptare"" = 0
                         and inreg.""IdStare"" in (1,2,3,4,5)
                         group by {0} ses.""NrMax"", 0)";
                if (Constante.tipBD == 1)
                    strSQL = string.Format(strSQL, "isnull(", idCurs, idSesiune);
                else
                    strSQL = string.Format(strSQL, "nvl(", idCurs, idSesiune);

                result = General.IncarcaDT(strSQL, null);
                if (result != null && result.Rows.Count > 0)
                {
                    if (Convert.ToInt32(result.Rows[0]["NrMax"].ToString()) < Convert.ToInt32(result.Rows[0]["NrParticipanti"].ToString()) + 1)
                        msg = "Numarul maxim de participanti pentru aceasta sesiune este " + result.Rows[0]["NrMax"].ToString() + " si sunt deja " + result.Rows[0]["NrParticipanti"].ToString() + " participanti inscrisi!";
                }
            }
            catch (Exception ex)
            {
                msg = "ERR: -\n" + ex.Message;
            }
            return msg;
        }

        public string AnuleazaCerereCurs(int idUser, int id, int tip)
        {
            //valabil pentru notificari
            //tip = 1  vine din solicitari curs
            //tip = 2  vine din aprobare si aprobare trainer


            string msg = "";

            try
            {
                if (id == -99) return msg;

                string sql = "SELECT * FROM \"Curs_Inregistrare\" WHERE \"Id\" = " + id.ToString();
                DataTable dtCurs = General.IncarcaDT(sql, null);

                if (dtCurs != null && dtCurs.Rows.Count > 0)
                {
                    string culoare = "";
                    sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = -1";
                    DataTable dtCul = General.IncarcaDT(sql, null);
                    if (dtCul != null && dtCul.Rows.Count > 0 && dtCul.Rows[0]["Culoare"] != DBNull.Value && dtCul.Rows[0]["Culoare"].ToString().Length > 0)
                        culoare = dtCul.Rows[0]["Culoare"].ToString();
                    else
                        culoare = "#FFFFFFFF";

                    //schimbam statusul
                    General.ExecutaNonQuery("UPDATE \"Curs_Inregistrare\" SET \"IdStare\" = -1, \"Culoare\" = '" + culoare + "' WHERE \"Id\" = " + id, null);


                    //introducem o linie de anulare in Ptj_CereriIstoric
                    sql = "INSERT INTO \"Curs_CereriIstoric\" (\"IdCerere\", \"IdCircuit\", \"IdSuper\", \"IdUser\",  \"IdStare\", \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\", USER_NO, TIME) "
                    + " VALUES (" + id + ", " + dtCurs.Rows[0]["IdCircuit"].ToString() + ", " + idUser + ",  " + idUser + ", -1, 22, '" + culoare + "', 1, " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE")
                    + ", " + idUser + ", " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + ") ";
                    General.ExecutaNonQuery(sql, null);

                    #region Validare start

                    string corpMesaj = "";
                    bool stop = false;

                    //switch (tip)
                    //{
                    //    case 1:
                    //        ctxNtf.ValidareRegula("Curs.CursuriInregistrare", "grDate", ent, idUser, (ent.F10003 ?? -99), out corpMesaj, out stop);
                    //        break;
                    //    case 2:
                    //        ctxNtf.ValidareRegula("Curs.Aprobare", "grDate", ent, idUser, (ent.F10003 ?? -99), out corpMesaj, out stop);
                    //        break;
                    //    case 3:
                    //        ctxNtf.ValidareRegula("Curs.AprobareHR", "grDate", ent, idUser, (ent.F10003 ?? -99), out corpMesaj, out stop);
                    //        break;
                    //}


                    if (corpMesaj != "")
                    {
                        msg += corpMesaj + "\r\n";
                        if (stop)
                        {
                            return msg;
                        }
                    }

                    #endregion




                    #region  Notificare strat

                    //if (tip == 1)
                    //    ctxNtf.TrimiteNotificare("Curs.CursuriInregistrare", "grDate", ent, idUser, (ent.F10003 ?? -99));
                    //else
                    //    ctxNtf.TrimiteNotificare("Curs.Aprobare", "grDate", ent, idUser, (ent.F10003 ?? -99));

                    #endregion


                    msg = "Proces finalizat cu succes.";
                    

                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;
        }


                //<dx:ASPxButton ID = "btnSendWaitingList"  runat="server" Text="Trimite Asteptare" OnClick="btnSendWaitingList_Click" oncontextMenu="ctx(this,event)" >
                //    <ClientSideEvents Click = "function(s, e) {
                //        pnlLoading.Show();
                //        e.processOnServer = true;
                //    }" />
                //    <Image Url = "~/Fisiere/Imagini/Icoane/adauga.png" ></ Image >
                //</ dx:ASPxButton>


    }
}