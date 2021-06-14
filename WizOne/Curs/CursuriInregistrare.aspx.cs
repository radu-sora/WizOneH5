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

namespace WizOne.Curs
{
    public partial class CursuriInregistrare : System.Web.UI.Page
    {

        public class metaUploadFile
        {
            public byte[] UploadedFile { get; set; }
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
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                //btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                btnDelete.Image.ToolTip = Dami.TraduCuvant("btnDelete", "Sterge");
                btnIstoric.Image.ToolTip = Dami.TraduCuvant("btnIstoric", "Istoric");  
                
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblCurs.InnerText = Dami.TraduCuvant("Curs");
                lblSesiune.InnerText = Dami.TraduCuvant("Sesiune");

                lblAngFiltru.InnerText = Dami.TraduCuvant("Angajat");
                lblCursFiltru.InnerText = Dami.TraduCuvant("Curs");
                lblSesiuneFiltru.InnerText = Dami.TraduCuvant("Sesiune");
                lblStare.InnerText = Dami.TraduCuvant("Stare");
                lblObs.InnerText = Dami.TraduCuvant("Observatii");
                btnDoc.ToolTip= Dami.TraduCuvant("Incarca document");
                btnDocSterge.ToolTip = Dami.TraduCuvant("Sterge document");
                btnArataDoc.ToolTip = Dami.TraduCuvant("Arata document");

                chkListaAsteptare.Text = Dami.TraduCuvant("Lista asteptare");

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

                if (!IsPostBack)
                    Session["CursuriInreg_Grid"] = null;

                string verifCursAnterior = Dami.ValoareParam("verifCursAnterior", "0");
                string verifNumarParticipantiSesiune = Dami.ValoareParam("verifNumarParticipantiSesiune", "0");
                if (verifNumarParticipantiSesiune == "0")
                    chkListaAsteptare.ClientVisible = false;
                Session["CursInreg_verifCursAnterior"] = verifCursAnterior;
                Session["CursInreg_verifNumarParticipantiSesiune"] = verifNumarParticipantiSesiune;

                if (!IsPostBack)
                {
                    DataTable dtSes = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\"", null);
                    Session["CursInreg_Sesiuni"] = dtSes;
                }



                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;

                DataTable dtCurs = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCurs"" ", null);
                GridViewDataComboBoxColumn colCurs = (grDate.Columns["IdCurs"] as GridViewDataComboBoxColumn);
                colCurs.PropertiesComboBox.DataSource = dtCurs;
                cmbCursFiltru.DataSource = dtCurs;
                cmbCursFiltru.DataBind();

                DataTable dtSesiune = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Curs_tblCursSesiune"" ", null);
                GridViewDataComboBoxColumn colSes = (grDate.Columns["IdSesiune"] as GridViewDataComboBoxColumn);
                colSes.PropertiesComboBox.DataSource = dtSesiune;

                Aprobare pagApr = new Aprobare();
                DataTable dtAng = pagApr.GetCursAngajati(General.VarSession("EsteAdmin").ToString() == "1" ? 0 : 3, Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()));
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();
                cmbAngFiltru.DataSource = dtAng;
                cmbAngFiltru.DataBind();

                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;

                IncarcaGrid();


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
                Session["CursuriInreg_Grid"] = null;
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
                cmbAngFiltru.Value = null;
                cmbCursFiltru.Value = null;
                cmbSesiuneFiltru.Value = null;

                IncarcaGrid();
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
                int? f10003 = null;
                int? curs = null, sesiune = null;
                if (cmbAngFiltru.Value != null)
                {
                    f10003 = Convert.ToInt32(cmbAngFiltru.Value);
                }
                if (cmbCursFiltru.Value != null)
                {
                    curs = Convert.ToInt32(cmbCursFiltru.Value);
                }
                if (cmbSesiuneFiltru.Value != null)
                {
                    sesiune = Convert.ToInt32(cmbSesiuneFiltru.Value);
                }

                string arr = DamiStari();

                if (Session["CursuriInreg_Grid"] == null)
                {
                    grDate.KeyFieldName = "IdAuto";
                    string strSql = "";
                    dt = GetmetaCurs_InregistrareFiltru(arr, General.VarSession("EsteAdmin").ToString() == "1" ? 0 : 3, Convert.ToInt32(cmbCursFiltru.Value ?? -99), Convert.ToInt32(cmbSesiuneFiltru.Value ?? -99), Convert.ToInt32(Session["UserId"].ToString()), (f10003 ?? -99), out strSql);

                    grDate.DataSource = dt;
                    grDate.DataBind();
                    Session["CursuriInreg_Grid"] = dt;
                }
                else
                {
                    grDate.KeyFieldName = "IdAuto";

                    dt = Session["CursuriInreg_Grid"] as DataTable;

                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                if (dt != null)
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
                                object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "IdCurs", "F10003", "IdSesiune" }) as object[];
                                Aprobare pagApr = new Aprobare();
                                pagApr.AnuleazaCerereCurs(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(lst[0] ?? "-99"), 1);
                                Session["CursuriInreg_Grid"] = null;
                                IncarcaGrid();
                            }
                            break;
                        case "btnDocSterge":
                            Session["DocUpload_CursInreg"] = null;
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

        public DataTable GetSesiuneFiltru(int IdCurs)
        {
            //numai sesiunile deschise pt inscriere
            try
            {
                DataTable q = null;

                q = General.IncarcaDT("SELECT\"Denumire\", \"Id\" FROM \"Curs_tblCursSesiune\" WHERE \"IdCurs\" = " + IdCurs + " and \"IdStare\" = 2 GROUP BY \"Denumire\", \"Id\" ORDER BY \"Denumire\" ", null);
                         
                return q;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        protected void cmbSesiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                if (cmbCurs.Value != null)
                {
                    cmbSesiune.DataSource = GetSesiuneFiltru(Convert.ToInt32(cmbCurs.Value));
                    cmbSesiune.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetCursuriFiltru(int f10003, int tipFunctii)
        {
            DataTable q = null;

            try
            {
                string strSql = "";
                strSql = "SELECT X.\"Denumire\", X.\"Id\", X.\"IdAuto\", X.\"Activ\" FROM (" + CursuriDisponibile(f10003, tipFunctii) + ") X " +
                         " WHERE COALESCE(X.\"Activ\",0)=1 " +
                         " GROUP BY X.\"Denumire\", X.\"Id\", X.\"IdAuto\", X.\"Activ\" ";
                strSql = string.Format(strSql, f10003);

                q = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }


        public string CursuriDisponibile(int f10003, int tipFunctii)
        {
            string strSql = "";

            try
            {
                /*LeonardM 23.05.2015
                 * toate query-urile sunt facute cu iner join si intersect
                 * pentru a lua in considerare toti angajatii care sunt pe anumite departamente si angajati*/

                /*LeonardM 26.02.2016
                 * se face inner join cu union all, pentru a prelua toti angajatii de pe functia
                 * respectiva*/

                /*LeonardM 10.10.2016
                * adaugare parametrii pentru a defini daca am nevoie de vreo dimensiune a cursului
                * angajat ; departamente ; competente ; functii
                * 0 => nu trebuie sa fie completat
                * 1 => trebuie sa fie completata obligatoriu dimensiunea
                * 2 => poate sa fie/nu fie dimensiunea completata
                * 3 => cel putin una din dimensiuni completata*/
                int Curs_CompletareAngajati = 3;
                int Curs_CompletareDepartament = 3;
                int Curs_CompletareCompetente = 3;
                int Curs_CompletareFunctii = 3;
                /*end LeonardM 10.10.2016*/

                #region preluare parametrii pentru completare dimensiuni curs
                Curs_CompletareAngajati = Convert.ToInt32(Dami.ValoareParam("Curs_CompletareAngajati", "0"));
                Curs_CompletareDepartament = Convert.ToInt32(Dami.ValoareParam("Curs_CompletareDepartament", "0"));
                Curs_CompletareCompetente = Convert.ToInt32(Dami.ValoareParam("Curs_CompletareCompetente", "0"));
                Curs_CompletareFunctii = Convert.ToInt32(Dami.ValoareParam("Curs_CompletareFunctii", "0"));      
                #endregion

                //angajati
                string strAng = string.Empty;
                #region dimensiuneAngajati
                switch (Curs_CompletareAngajati)
                {
                    case 0: /* nu trebuie completata dimensiunea*/
                        strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                ";
                        break;
                    case 1: /* trebuie completat obligatoriu*/
                        strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_relCursAngajati"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Angajat"" = C.F10003
                                ";
                        break;
                    case 2: /* poate sau nu poate fi completata dimensiunea*/
                        DataTable dtCurs = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                   + " join \"Curs_relCursAngajati\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                        if (dtCurs != null && dtCurs.Rows.Count > 0)
                        {
                            strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursAngajati"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Angajat"" = C.F10003
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursAngajati"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                        }
                        else
                        {
                            strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                ";
                        }
                        break;
                    case 3: /*trebuie cel putin un camp completat*/
                        strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                /*LEFT JOIN ""Curs_relCursAngajati"" B ON A.""Id"" = B.""Id_Curs""*/
                                INNER JOIN ""Curs_relCursAngajati"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Angajat"" = C.F10003
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursAngajati"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                        break;
                }
                #endregion
                //departament
                #region dimensiuneDepartament
                string strDept = string.Empty;
                switch (Curs_CompletareDepartament)
                {
                    case 0: /* nu trebuie completata dimensiunea*/
                        strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                ";
                        break;
                    case 1: /* trebuie completat obligatoriu*/
                        strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_relCursDepart"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Depart"" = C.F10007
                                ";
                        break;
                    case 2: /* poate sau nu poate fi completata dimensiunea*/
                        DataTable dtCurs = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                 + " join \"Curs_relCursDepart\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                        if (dtCurs != null && dtCurs.Rows.Count > 0)
                        {
                            strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursDepart"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Depart"" = C.F10007
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursDepart"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                        }
                        else
                        {
                            strAng = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                ";
                        }
                        break;
                    case 3: /*trebuie cel putin un camp completat*/
                        strDept = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursDepart"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Depart"" = C.F10007
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursDepart"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                        break;
                }

                #endregion
                //competente
                #region dimensiuneCompetente
                string strComp = string.Empty;
                switch (Curs_CompletareCompetente)
                {
                    case 0:/* nu trebuie completata dimensiunea*/
                        #region nu trebuie completata dimensiunea
                        strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A";
                        break;
                    #endregion
                    case 1: /* trebuie completat obligatoriu*/
                        #region trebuie completata obligatoriu
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/
                                strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursCompetente"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN ""Curs_relCompetenteFunctii"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                JOIN F100 D ON C.""Id_Functie"" = D.F10071";
                                break;
                            case 1:                                     //functii academice
                                strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursCompetenteAcad"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN ""Curs_relCompetenteFunctiiAcad"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                JOIN F100 D ON C.""Id_Functie"" = D.F10051";
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)
                                strComp = "";   //nu doreste competente, acestea sunt doar cu titlu informativ, pt rapoarte
                                break;
                        }
                        break;
                    #endregion
                    case 2: /* poate sau nu poate fi completata dimensiunea*/
                        #region poate/nu fi completata dimensiunea
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/                  
                                DataTable dtCurs = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                           + " join \"Curs_relCursCompetente\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                                if (dtCurs != null && dtCurs.Rows.Count > 0)
                                {
                                    strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                            FROM ""Curs_tblCurs"" A
                                            JOIN ""Curs_relCursCompetente"" B ON A.""Id"" = B.""Id_Curs""
                                            JOIN ""Curs_relCompetenteFunctii"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                            JOIN F100 D ON C.""Id_Functie"" = D.F10071
                                            WHERE ((SELECT COUNT(*) FROM ""Curs_relCursCompetente"" WHERE ""Id_Curs"" = A.""Id"")=0 OR D.F10003 = {0})";
                                }
                                else
                                {
                                    strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                               FROM ""Curs_tblCurs"" A
                                                ";
                                }

                                break;
                            case 1:                                     //functii academice                     
                                DataTable dtCursAcad = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                           + " join \"Curs_relCursCompetenteAcad\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                                if (dtCursAcad != null && dtCursAcad.Rows.Count > 0)
                                {
                                    strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                                JOIN ""Curs_relCursCompetenteAcad"" B ON A.""Id"" = B.""Id_Curs""
                                                JOIN ""Curs_relCompetenteFunctiiAcad"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                                JOIN F100 D ON C.""Id_Functie"" = D.F10051
                                            WHERE ((SELECT COUNT(*) FROM ""Curs_relCursCompetenteAcad"" WHERE ""Id_Curs"" = A.""Id"")=0 OR D.F10003 = {0})";
                                }
                                else
                                {
                                    strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                               FROM ""Curs_tblCurs"" A
                                                ";
                                }
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)
                                strComp = "";   //nu doreste competente, acestea sunt doar cu titlu informativ, pt rapoarte
                                break;
                        }
                        break;
                    #endregion
                    case 3: /*trebuie cel putin un camp completat*/
                        #region trebuie cel putin un camp completat
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/
                                strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursCompetente"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN ""Curs_relCompetenteFunctii"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                JOIN F100 D ON C.""Id_Functie"" = D.F10071
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursCompetente"" WHERE ""Id_Curs"" = A.""Id"")=0 OR D.F10003 = {0})";
                                break;
                            case 1:                                     //functii academice
                                strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                LEFT JOIN ""Curs_relCursCompetenteAcad"" B ON A.""Id"" = B.""Id_Curs""
                                LEFT JOIN ""Curs_relCompetenteFunctiiAcad"" C ON B.""Id_Competenta"" = C.""Id_Competenta""
                                LEFT JOIN F100 D ON C.""Id_Functie"" = D.F10051
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursCompetenteAcad"" WHERE ""Id_Curs"" = A.""Id"")=0 OR D.F10003 = {0})";
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)
                                strComp = "";   //nu doreste competente, acestea sunt doar cu titlu informativ, pt rapoarte
                                break;
                        }
                        break;
                        #endregion
                }
                #endregion

                #region dimensiuneFunctii
                string strFct = string.Empty;
                switch (Curs_CompletareFunctii)
                {
                    case 0:/* nu trebuie completata dimensiunea*/
                        #region nu trebuie completata dimensiunea
                        strComp = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A";
                        break;
                    #endregion
                    case 1: /* trebuie completat obligatoriu*/
                        #region trebuie completata obligatoriu
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Functie"" = C.F10071
                                UNION
                                /*LeonardM 23.05.2015 pentru a prelua si modificarile de functii*/
                                SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_tblCursSesiune"" B1 ON  A.""Id"" = B1.""IdCurs""
                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                /*inner JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""*/
                                JOIN F704 C ON B.""Id_Functie"" = C.F70407
                                                   and 2 = C.F70404
                                                    and {1}B1.""DataInceput""{2} <= {1}C.F70406{2}";

                                break;
                            case 1:                                     //functii academice
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                          JOIN ""Curs_relCursFunctiiAcad"" B ON A.""Id"" = B.""Id_Curs""
                                          JOIN F100 C ON B.""Id_FunctieAcad"" = C.F10051
                                          ";
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN ""Org_relPostAngajat"" C ON B.""Id_Functie"" = C.""IdPost"" AND C.""DataInceput"" <= {3} AND {3} <= C.""DataSfarsit""
                                JOIN F100 D ON C.F10003 = D.F10003
                                ";
                                break;
                        }
                        break;
                    #endregion
                    case 2: /* poate sau nu poate fi completata dimensiunea*/
                        #region poate/nu fi completata dimensiunea
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/                         
                                DataTable dtCurs = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                           + " join \"Curs_relCursFunctii\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                                if (dtCurs != null && dtCurs.Rows.Count > 0)
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                                FROM ""Curs_tblCurs"" A
                                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                                JOIN F100 C ON B.""Id_Functie"" = C.F10071
                                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctii"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})
                                                UNION
                                                /*LeonardM 23.05.2015 pentru a prelua si modificarile de functii*/
                                                SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                                FROM ""Curs_tblCurs"" A
                                                INNER JOIN ""Curs_tblCursSesiune"" B1 ON  A.""Id"" = B1.""IdCurs""
                                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                                /*inner JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""*/
                                                JOIN F704 C ON B.""Id_Functie"" = C.F70407
                                                                   and 2 = C.F70404
                                                                    and {1}B1.""DataInceput""{2} <= {1}C.F70406{2}
                                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctii"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F70403 = {0})";

                                }
                                else
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                               FROM ""Curs_tblCurs"" A
                                                ";
                                }

                                break;
                            case 1:                                     //functii academice                            
                                DataTable dtCursFunctii = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                           + " join \"Curs_relCursFunctiiAcad\" b on a.\"Id\" = b.\"Id_Curs\" ", null);
                                if (dtCursFunctii != null && dtCursFunctii.Rows.Count > 0)
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                                JOIN ""Curs_relCursFunctiiAcad"" B ON A.""Id"" = B.""Id_Curs""
                                                JOIN F100 C ON B.""Id_FunctieAcad"" = C.F10051
                                                ";
                                }
                                else
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                               FROM ""Curs_tblCurs"" A
                                                ";
                                }
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)                
                                DataTable dtCursPosturi = General.IncarcaDT("SELECT a.* from \"Curs_tblCurs\" a "
                                           + " join \"Curs_relCursFunctii\" b on a.\"Id\" = b.\"Id_Curs\" "
                                           + " join \"Org_relPostAngajat\" c on b.\"Id_Functie\" = c.\"IdPost\" "
                                           + " WHERE c.\"DataInceput\" <= " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + " AND "
                                           + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") + " <= c.\"DataSfarsit\" ", null);
                                if (dtCursPosturi != null && dtCursPosturi.Rows.Count > 0)
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                            JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                            JOIN ""Org_relPostAngajat"" C ON B.""Id_Functie"" = C.""IdPost"" AND C.""DataInceput"" <= {3} AND {3} <= C.""DataSfarsit""
                                            JOIN F100 D ON C.F10003 = D.F10003
                                            ";
                                }
                                else
                                {
                                    strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                               FROM ""Curs_tblCurs"" A
                                                ";
                                }
                                break;
                        }
                        break;
                    #endregion
                    case 3: /*trebuie cel putin un camp completat*/
                        #region trebuie cel putin un camp completat
                        switch (tipFunctii)
                        {
                            case 0: /* functii normale*/
                                //functii
                                /*LeonardM 25.02.2016
                                 * nu mai se respecta acest lucru, am vorbit cu Diana
                                 * si preluam functiile din F718*/
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                JOIN F100 C ON B.""Id_Functie"" = C.F10071
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctii"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})
                                UNION
                                /*LeonardM 23.05.2015 pentru a prelua si modificarile de functii*/
                                SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" 
                                FROM ""Curs_tblCurs"" A
                                INNER JOIN ""Curs_tblCursSesiune"" B1 ON  A.""Id"" = B1.""IdCurs""
                                JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                /*inner JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""*/
                                JOIN F704 C ON B.""Id_Functie"" = C.F70407
                                                   and 2 = C.F70404
                                                    and {1}B1.""DataInceput""{2} <= {1}C.F70406{2}
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctii"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F70403 = {0})";

                                /*new LeonardM 25.02.2016*/
                                break;
                            case 1:                                     //functii academice
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                LEFT JOIN ""Curs_relCursFunctiiAcad"" B ON A.""Id"" = B.""Id_Curs""
                                LEFT JOIN F100 C ON B.""Id_FunctieAcad"" = C.F10051
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctiiAcad"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                                break;
                            case 2:                                     //posturi (se foloseste la clientul GAM)
                                strFct = @"SELECT A.""Denumire"", A.""Id"", A.""IdAuto"", A.""Activ"" FROM ""Curs_tblCurs"" A
                                LEFT JOIN ""Curs_relCursFunctii"" B ON A.""Id"" = B.""Id_Curs""
                                LEFT JOIN ""Org_relPostAngajat"" C ON B.""Id_Functie"" = C.""IdPost"" AND C.""DataInceput"" <= {3} AND {3} <= C.""DataSfarsit""
                                LEFT JOIN F100 D ON C.F10003 = D.F10003
                                WHERE ((SELECT COUNT(*) FROM ""Curs_relCursFunctii"" WHERE ""Id_Curs"" = A.""Id"")=0 OR C.F10003 = {0})";
                                break;
                        }
                        break;
                        #endregion
                }
                #endregion

                strSql = strAng;
                /*LeonardM 25.02.2016
                 * nu se mai face intersect, ci union*/
                if (strDept != "") strSql += " UNION " + strDept;
                if (strFct != "") strSql += " UNION " + strFct;
                if (strComp != "") strSql += " UNION " + strComp;

                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, f10003, "CONVERT(VARCHAR(10),", ", 112)", "getdate()");
                else
                    strSql = string.Format(strSql, f10003, "to_number(to_char(", ", 'yyyymm'))", "sysdate");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        protected void btnDoc_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_CursInreg"] = itm;

                btnDoc.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {               


                string strErr = "";

                if (cmbAng.Value == null) strErr += ", angajatul";
                if (cmbCurs.Value == null) strErr += ", cursul";
                if (cmbSesiune.Value == null) strErr += ", sesiunea";

                if (strErr != "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date: " + strErr.Substring(1)), MessageBox.icoError);
                    return;
                }

                //procesul de salvare
                DateTime cmpData1 = new DateTime(1900, 1, 1);
                DateTime cmpData2 = new DateTime(1900, 1, 1);


                /*LeonardM 01.09.2015 verificare curs anterior*/
                int absovitToateCursuriAnterioare = 0;
                DataTable dtCursAnt = GetVerificareAbsolvireCursAnterior(Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbCurs.Value ?? -99));
             
                if (dtCursAnt != null && dtCursAnt.Rows.Count > 0)
                {
                    if (dtCursAnt.Rows[0]["CNT"] != null)
                    {
                        try
                        {
                            absovitToateCursuriAnterioare = Convert.ToInt32(dtCursAnt.Rows[0]["CNT"].ToString());
                        }
                        catch { }
                    }
                }
                metaUploadFile doc = Session["DocUpload_CursInreg"] as metaUploadFile;
                if (doc != null)
                {
                    //General.LoadFile(doc.UploadedFileName.ToString(), doc.UploadedFile, "Curs_Inregistrare", Convert.ToInt32(e.NewValues["IdAuto"].ToString()));
                    Session["DocUpload_CursInreg"] = null;
                }

                int verifNumarParticipantiSesiune = Convert.ToInt32(Session["CursInreg_verifNumarParticipantiSesiune"].ToString());

                int verifCursAnterior = Convert.ToInt32(Session["CursInreg_verifCursAnterior"].ToString());
                if (verifCursAnterior == 1 && absovitToateCursuriAnterioare != 0)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati absolvit toate cursurile anterioare necesare pentru inscrierea la cursul selectat!"), MessageBox.icoError);
                    return;
                }
                else
                {
                    /*LeonardM 01.09.2015
                        * verificare numar maxim participanti*/
                    string msg = AdaugaCerere(Convert.ToInt32(Session["UserId"].ToString()),
                                Convert.ToInt32(cmbAng.Value ?? -99),
                                Convert.ToInt32(cmbCurs.Value ?? -99),
                                Convert.ToInt32(cmbSesiune.Value ?? -99),
                                -99,
                                (txtObs.Value ?? "").ToString(),
                                null,
                                null,
                                null,
                                doc != null ? doc.UploadedFile : null,
                                doc != null ? doc.UploadedFileExtension.ToString() : "",
                                doc != null ? doc.UploadedFileName.ToString(): "",
                                verifNumarParticipantiSesiune,
                                chkListaAsteptare.Checked == true ? 1 : 0
                                );
                    /*end LeonardM 01.09.2015*/


                    MessageBox.Show(Dami.TraduCuvant(msg.Replace("?0?-", "")), (!msg.Contains("succes") ? MessageBox.icoWarning : MessageBox.icoSuccess));

                    if (msg != "" && msg.Substring(0, 4) == "?0?-")
                    {
                         

                        btnFiltru_Click(sender, null);
                        chkListaAsteptare.Checked = false;
                    }                 
                    
                    
                }

                //MessageBox.Show("Proces finalizat cu succes!", MessageBox.icoSuccess);

                cmbAng.Value = null;
                cmbCurs.Value = null;
                cmbSesiune.Value = null;
                txtObs.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        /*LeonardM 01.09.2015 - verificare daca angajatul a terminat cursurile anterioare pentru a participa la cursul prezent*/
        public DataTable GetVerificareAbsolvireCursAnterior(int F10003, int idCurs)
        {
            DataTable result = null;

            try
            {
                string sql = @"select count(*) as CNT
                               from ""Curs_relCursAnterior"" ant
                               join ""Curs_tblCurs"" curs on ant.""IdCursAnterior"" = curs.""Id""
                               join ""Curs_tblCursSesiune"" ses on curs.""Id"" = ses.""IdCurs""          
                               left join ""Curs_Inregistrare"" inreg on curs.""Id"" = inreg.""IdCurs""   
                               and ses.""Id"" = inreg.""IdSesiune""
                               and inreg.""IdStare"" = 5
                               and inreg.""F10003"" = {0}
                               where ant.""IdCurs"" =  {1}
                               and inreg.""Id"" is null
                            ";
                sql = string.Format(sql, F10003, idCurs);

                result = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return result;
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["CursuriInreg_Grid"] as DataTable;

                DataRow row = dt.Rows.Find(keys); 


                if (Session["DocUpload_CursuriInreg_Grid"] != null)
                {
                    metaUploadFile doc = Session["DocUpload_CursuriInreg_Grid"] as metaUploadFile;
                    if (doc != null)
                    {
                        General.LoadFile(doc.UploadedFileName.ToString(), doc.UploadedFile, "Curs_Inregistrare", Convert.ToInt32(row["Id"].ToString()));
                        Session["DocUpload_CursuriInreg_Grid"] = null;
                    }
                }

                row["USER_NO"] = Session["UserId"];
                row["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDate.CancelEdit();
                Session["CursuriInreg_Grid"] = dt;
                grDate.DataSource = dt;

                General.SalveazaDate(dt, "Curs_Inregistrare");
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
                if (!e.IsValid) return;
                ASPxUploadControl btnDocUpload = (ASPxUploadControl)sender;

                metaUploadFile itm = new metaUploadFile();
                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["DocUpload_CursuriInreg_Grid"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void pnlCtlCurs_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "IdCurs", "F10003", "IdSesiune" }) as object[];
                DataTable dtCurs = General.IncarcaDT("SELECT * FROM \"Curs_tblCurs\" WHERE\"Id\" = " + Convert.ToInt32(lst[2] ?? "-99"), null);
                if (dtCurs != null && dtCurs.Rows.Count > 0)
                {
                    txtDenumire.Text = dtCurs.Rows[0]["Denumire"] == DBNull.Value ? "" : dtCurs.Rows[0]["Denumire"].ToString();
                    //txtGrad.Text = dtCurs.Rows[0]["Gradul"] == DBNull.Value ? "" : dtCurs.Rows[0]["Gradul"].ToString();
                    //txtCertif.Text = dtCurs.Rows[0]["CertificariNume"] == DBNull.Value ? "" : dtCurs.Rows[0]["CertificariNume"].ToString();
                    txtComen.Text = dtCurs.Rows[0]["Observatii"] == DBNull.Value ? "" : dtCurs.Rows[0]["Observatii"].ToString();
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void pnlCtlSesiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                object[] lst = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id", "IdStare", "IdCurs", "F10003", "IdSesiune" }) as object[];
                DataTable dtSes = General.IncarcaDT("SELECT * FROM \"Curs_tblCursSesiune\" WHERE\"IdCurs\" = " + Convert.ToInt32(lst[2] ?? "-99") + " AND \"Id\" = " + Convert.ToInt32(lst[4] ?? "-99"), null);
                if (dtSes != null && dtSes.Rows.Count > 0)
                {
                    txtDenumireS.Text = dtSes.Rows[0]["Denumire"] == DBNull.Value ? "" : dtSes.Rows[0]["Denumire"].ToString();
                    txtDataInc.Text = dtSes.Rows[0]["DataInceput"] == DBNull.Value ? "" : Convert.ToDateTime(dtSes.Rows[0]["DataInceput"].ToString()).ToShortDateString();
                    txtDataSf.Text = dtSes.Rows[0]["DataSfarsit"] == DBNull.Value ? "" : Convert.ToDateTime(dtSes.Rows[0]["DataSfarsit"].ToString()).ToShortDateString();
                    txtOraInc.Text = dtSes.Rows[0]["OraInceput"] == DBNull.Value ? "" : Convert.ToDateTime(dtSes.Rows[0]["OraInceput"].ToString()).ToShortTimeString();
                    txtOraSf.Text = dtSes.Rows[0]["OraSfarsit"] == DBNull.Value ? "" : Convert.ToDateTime(dtSes.Rows[0]["OraSfarsit"].ToString()).ToShortTimeString();
                    txtTematica.Text = dtSes.Rows[0]["TematicaNume"] == DBNull.Value ? "" : dtSes.Rows[0]["TematicaNume"].ToString();
                    txtOrganizator.Text = dtSes.Rows[0]["OrganizatorNume"] == DBNull.Value ? "" : dtSes.Rows[0]["OrganizatorNume"].ToString();
                    txtTrainer.Text = dtSes.Rows[0]["TrainerNume"] == DBNull.Value ? "" : dtSes.Rows[0]["TrainerNume"].ToString();
                    txtLocatie.Text = dtSes.Rows[0]["Locatie"] == DBNull.Value ? "" : dtSes.Rows[0]["Locatie"].ToString();
                    txtObservatii.Text = dtSes.Rows[0]["Observatii"] == DBNull.Value ? "" : dtSes.Rows[0]["Observatii"].ToString();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        public DataTable GetmetaCurs_InregistrareFiltru(string filtruStari, int tipUser, int idCurs, int idSesiune, decimal IdUser, decimal? F10003, out string strSql)
        {
            strSql = "";
            try
            {
                string lstStari = filtruStari.Replace(";", ",");

                /*LeonardM 15.03.2016
                 * preluare angajati subordonati conform listei de subordonare*/
                DataTable dtUser = General.IncarcaDT("SELECT * FROM USERS WHERE F70102 = " + IdUser, null);
                int User_AngajatId = Convert.ToInt32(dtUser.Rows[0]["F10003"] == DBNull.Value ? "-99" : dtUser.Rows[0]["F10003"].ToString());
                Aprobare pagApr = new Aprobare();
                DataTable lstF100NumeComplet = pagApr.GetCursAngajati(tipUser, (int)IdUser, User_AngajatId);
                if (F10003 != -99)
                    lstF100NumeComplet = lstF100NumeComplet.Select("F10003 = " + F10003) == null ? null : lstF100NumeComplet.Select("F10003 = " + F10003).CopyToDataTable();
                List<int> lstF10003Available = new List<int>();
                if (lstF100NumeComplet != null)
                    for (int i = 0; i < lstF100NumeComplet.Rows.Count; i++)
                    {
                        lstF10003Available.Add(Convert.ToInt32(lstF100NumeComplet.Rows[i]["F10003"].ToString()));
                    }
                /*end LeonardM 15.03.2016*/

                DataTable q = null;
                DataTable q2 = null;

                string sql = "", cond = "";

                string lstAng = "";
                for (int j = 0; j < lstF10003Available.Count; j++)
                {
                    lstAng += lstF10003Available[j].ToString();
                    if (j < lstF10003Available.Count - 1)
                        lstAng += ",";
                }
                if (lstAng.Length <= 0)
                    lstAng = "-99";

                string lstCampuri = "a.\"Certificat\" , a.\"CostRONcuTVA\", a.\"Culoare\", a.\"Denumire\", a.F10003, a.\"Id\", a.\"IdAuto\", a.\"IdCircuit\", a.\"IdCurs\", a.\"IdSesiune\" , a.\"IdStare\", " 
                    + "  a.\"Observatii\", a.\"Organizator\", a.\"Pozitie\", a.TIME, a.\"TotalSuperCircuit\", a.USER_NO, a.\"UserIntrod\", b.\"IdQuiz\", b.\"IdStare\" as \"IdStareSesiune\", "
                    + " case when a.\"eListaAsteptare\" = 1 then 'Lista Asteptare' else '---' end as \"eListaAsteptare\" ";

                switch (tipUser)
                {
                    case 1:   //este trainer
                        //q = General.IncarcaDT("SELECT " + lstCampuri + " FROM \"Curs_Inregistrare\" a WHERE a.F10003 = " + F10003 + " OR F10003 IS NULL", null);                  
                        sql = "SELECT " + lstCampuri + " FROM \"Curs_Inregistrare\" a  ";
                        cond = " and a.F10003 = " + F10003 + " OR F10003 IS NULL";
                        break;
                    case 2:                     //cand este responsabil de grup
                        //q = General.IncarcaDT("SELECT " + lstCampuri + " FROM \"Curs_Inregistrare\" a WHERE a.F10003 in (" + lstAng + ") "
                        //    + " UNION "
                        //    + " SELECT a.* FROM \"Curs_Inregistrare\" a JOIN \"F100Supervizori\" b ON a.F10003 = b.F10003 WHERE b.\"IdUser\" = " + IdUser, null);
                        sql = " SELECT " + lstCampuri + " from "
                            + "(SELECT a.* FROM \"Curs_Inregistrare\" a WHERE a.F10003 in (" + lstAng + ") "
                            + " UNION "
                            + " SELECT a.* FROM \"Curs_Inregistrare\" a JOIN \"F100Supervizori\" b ON a.F10003 = b.F10003 WHERE b.\"IdUser\" = " + IdUser + ") a ";                     
                        break;
                    default:     //cand este simplu angajat
                        //q = General.IncarcaDT("SELECT " + lstCampuri + " FROM \"Curs_Inregistrare\" a WHERE a.F10003 in (" + lstAng + ") ", null);
                        sql = "SELECT " + lstCampuri + " FROM \"Curs_Inregistrare\" a  ";
                        cond = " and a.F10003 in (" + lstAng + ") ";
                        break;
                }

                string join = " LEFT JOIN \"Curs_tblCursSesiune\" b on a.\"IdSesiune\" = b.\"Id\" WHERE 1=1 ";

               

                if (idCurs != -99)
                    cond += " AND a.\"IdCurs\" = " + idCurs;

                if (lstStari.Length > 0)
                    cond += " AND a.\"IdStare\" IN (" + lstStari + ")";

                if (idSesiune != -99)
                    cond = " AND a.\"IdSesiune\" = " + idSesiune;

                q2 = General.IncarcaDT(sql + join + cond, null);
                strSql = sql + join + cond;
                return q2;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        protected void cmbSesiuneFiltru_Callback(object sender, CallbackEventArgsBase e)
        {
            if (cmbCursFiltru.Value != null)
            {
                DataTable dtSes = Session["CursInreg_Sesiuni"] as DataTable;
                cmbSesiuneFiltru.DataSource = dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCursFiltru.Value)) == null || dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCursFiltru.Value)).Count() <= 0
                    ? null : dtSes.Select("IdCurs = " + Convert.ToInt32(cmbCursFiltru.Value)).CopyToDataTable();
                cmbSesiuneFiltru.DataBind();
                cmbSesiuneFiltru.Value = null;
            }
            else
            {
                cmbSesiuneFiltru.DataSource = null;
                cmbSesiuneFiltru.DataBind();
            }
        }


        public string AdaugaCerere(int idUser,
                              int f10003,
                              int idCurs,
                              int idSesiune,
                              int idCircuit,
                              string strObs,
                              int? CostRON,
                              string Den,
                              string Org,
                              byte[] fis,
                              string fisExt,
                              string fisNume,
                              int verificareNrMaximParticipanti,
                              int chkListaAsteptare
                           )
        {
            string msg = "";

            try
            {

                DataTable dtTmp = General.IncarcaDT("SELECT COUNT(*) FROM \"Curs_Inregistrare\" WHERE F10003 = " + f10003 + " and \"IdCurs\" = " + idCurs + " and \"IdSesiune\" = " + idSesiune + " and \"IdStare\" NOT IN (-1, 0)", null);
                if (dtTmp != null && dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null && dtTmp.Rows[0][0].ToString().Length > 0 &&  Convert.ToInt32(dtTmp.Rows[0][0].ToString()) > 0)
                {
                    msg = "?0?-Angajatul a fost deja inscris la acest curs/sesiune!";
                    return msg;
                }

                dtTmp = General.IncarcaDT("SELECT COUNT(*) FROM \"Curs_tblCursSesiune\" WHERE \"Id\" =" + idSesiune + " AND \"IdStare\" = 4", null);

                if (dtTmp != null && dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null && dtTmp.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtTmp.Rows[0][0].ToString()) > 0)
                {
                    msg = "?0?-Sesiune este deja finalizata!";
                    return msg;
                }

                /*LeonardM 01.09.2015
                 * verificare numar participanti pe sesiune*/
                if (verificareNrMaximParticipanti == 1 && chkListaAsteptare == 0)
                {
                    msg = Curs_VerificaNumarMaximParticipanti(idCurs, idSesiune);
                    if (msg != "")
                    {
                        msg = "?0?-" + msg;
                        return msg;
                    }
                }
                /*end Leonardm 01.09.2015*/

                //Radu 18.10.2016
                DataTable dtSuperv = General.IncarcaDT("SELECT \"IdSuper\" FROM \"F100Supervizori\" WHERE \"IdUser\" = " + idUser + " AND F10003 = " + f10003 + " GROUP BY \"IdSuper\" ", null);
                int idSuperv = 0;
                if (dtSuperv != null && dtSuperv.Rows.Count > 0)
                    idSuperv = Convert.ToInt32(dtSuperv.Rows[0][0].ToString());

                DataTable q = null;


                q = General.IncarcaDT("SELECT a.* FROM \"Curs_Circuit\" a " +
                                      " LEFT JOIN  \"relGrupAngajat\" b on a.\"IdGrupAngajat\" = b.\"IdGrup\" " +
                                      " JOIN \"Curs_tblCursSesiune\" c on a.\"IdInternExtern\" = c.\"InternExtern\" " +
                                      " WHERE F10003 = " + f10003 + " and c.\"Id\" = " + idSesiune +

                                      " AND ((a.Super1 < 0 AND a.Super1 = " + (-1) * idSuperv + ") OR (a.Super1 >= 0 AND a.Super1 = " + (idUser == f10003 ? 0 : idUser) + ")) ORDER BY a.\"IdGrupAngajat\"", null);

        
                if (q == null || q.Rows.Count <= 0)
                {
                    msg = "?0?-Nu se poate adauga, deoarece nu exista circuit definit!";
                    return msg;
                }
                idCircuit = Convert.ToInt32(q.Rows[0]["IdAuto"].ToString());


                //obtinem campurile care depind de circuit
                DataTable dtCirc = General.IncarcaDT("SELECT * FROM \"Curs_Circuit\" WHERE \"IdAuto\" = " + idCircuit, null); 

                int total = 0;
                int idStare = 2;
                int pozUser = 1;
                string sql = "";

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


                int idUrmat = Convert.ToInt32(Dami.NextId("Curs_Inregistrare"));


                //--------------------------  Curs_CereriIstoric start

                //adaugam in tabela Curs_CereriIstoric
                int poz = 0;
                int idUserPrece = -99;
                int idUserCalc = -99;
                int idStareGen = -99;
                List<int> lst = new List<int>();

                for (int i = 1; i <= 20; i++)
                {
                    string aprobat = "NULL", dataAprobare = "NULL";
                    int idSuper = -99;
                    idStare = -99;
                    int lstAstept = 0;
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

                            //daca uramtorul in circuit este acelasi user, se salveaza doar o singura data
                            if (idUserCalc != idUserPrece)
                            {
                                poz += 1;                       

                                //starea
                                if (idUserCalc == idUser)
                                {
                                    pozUser = poz;
                                    if (poz == 1) { idStare = 1; idStareGen = 1; }
                                    if (poz == total && total == 1) { idStare = 3; idStareGen = 3; }   //Radu 18.10.2016

                                    dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                    aprobat = "1";
                                    /*LeonardM 01.09.2015
                                     * verificare lista asteptare*/
                                    lstAstept = chkListaAsteptare;
                                    /*end LeonardM 01.09.2015*/
                                }
                                /*LeonardM 15.03.2016
                                 * atunci cand indeplinesc tot circuitul,
                                 * se trece direct in starea aprobat*/
                                else if (poz == total)
                                {
                                    //idStare = 3;
                                    if (poz == 1) { idStare = 1; idStareGen = 1; }                    //Radu 18.10.2016
                                    if (poz == total && total == 1) { idStare = 3; idStareGen = 3; }   //Radu 18.10.2016

                                    if (idStare == 3)
                                    {
                                        dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                        aprobat = "1"; 
                                    }
                                    else
                                    {
                                        aprobat = "NULL";
                                        dataAprobare = "NULL";
                                    }
                                    /*LeonardM 01.09.2015
                                     * verificare lista asteptare*/
                                    lstAstept = chkListaAsteptare;
                                    /*end LeonardM 01.09.2015*/
                                }
                                /*end LeonardM 15.03.2016*/

                                string strSqlIst = "INSERT INTO \"Curs_CereriIstoric\"(\"IdCerere\", \"IdCircuit\", \"IdSuper\", \"IdUser\", \"IdStare\",  \"Pozitie\", \"Culoare\", \"Aprobat\", \"DataAprobare\",  \"eListaAsteptare\",  USER_NO, TIME) "
                                        + " VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";
                                strSqlIst = string.Format(strSqlIst, idUrmat, idCircuit, idSuper, idUserCalc, (idStare == -99 ? "NULL" : idStare.ToString()), poz, ("(SELECT CASE WHEN \"Culoare\" IS NULL THEN '#FFFFFFFF' ELSE \"Culoare\" END FROM \"Curs_tblStari\" WHERE \"Id\" = " + idStare + ")"),
                                    aprobat, dataAprobare, lstAstept, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                                General.ExecutaNonQuery(strSqlIst, null);

                                idUserPrece = idUserCalc;
                            }
                            else
                            {
                                total--;
                            }


                        }

                    }
                }

                //--------------------------  Curs_Inregistrare start

                //adaugam in tabela Curs_Inregistrare               

               string strSql = "INSERT INTO \"Curs_Inregistrare\"(\"Id\", F10003, \"IdCurs\", \"IdSesiune\", \"IdCircuit\", \"Observatii\", \"UserIntrod\", \"TotalSuperCircuit\", \"Pozitie\", \"IdStare\", \"Culoare\", \"CostRONcuTVA\", \"Denumire\", \"Organizator\", \"eListaAsteptare\", USER_NO, TIME) "
                        + " VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10}, {11}, '{12}', '{13}', {14}, {15}, {16})";
                strSql = string.Format(strSql, idUrmat, f10003, idCurs, idSesiune, idCircuit, strObs, idUser, total, pozUser, idStareGen,
                    ("(SELECT CASE WHEN \"Culoare\" IS NULL THEN '#FFFFFFFF' ELSE \"Culoare\" END FROM \"Curs_tblStari\" WHERE \"Id\" = " + idStareGen + ")"),
                    CostRON == null ? "NULL" : CostRON.ToString(), Den, Org, chkListaAsteptare, Session["UserId"].ToString(), (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));

                General.ExecutaNonQuery(strSql, null);

                //--------------------------  Curs_Inregistrare end

                #region  Atasamente start


                //adauga atasamentul
                if (fis != null)
                {
                    sql = "INSERT INTO \"tblFisiere\" (\"Tabela\", \"Id\", \"Fisier\", \"FisierNume\", \"FisierExtensie\", USER_NO, TIME)  VALUES ('{0}', {1}, '{2}', '{3}', '{4}', {5}, {6})";
                    sql = string.Format(sql, "Curs_Inregistrare", idUrmat, fis, fisNume, fisExt, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"));
                    General.ExecutaNonQuery(sql, null); 
                }

                #endregion


                #region Validare strat

                //bool trimteMesajFinal = true;
                //string corpMesaj = "";
                //bool stop = false;

                //srvNotif ctxNtf = new srvNotif();
                //ctxNtf.ValidareRegula("Curs.CursuriInregistrare", "grDate", ent, idUser, f10003, out corpMesaj, out stop);

                //if (corpMesaj != "")
                //{
                //    trimteMesajFinal = false;
                //    msg = corpMesaj;
                //    if (stop) return msg;
                //}

                #endregion



                #region  Notificare strat

                string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
               
                int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);               

                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    NotifAsync.TrimiteNotificare("Curs.CursuriInregistrare", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM Curs_Inregistrare Z WHERE Id=" + idUrmat.ToString(), "Curs_Inregistrare", idUrmat, idUser, marcaUser, arrParam);
                });

                #endregion


                //particula ?0?-  este folosita pentru optimzare - semnaleaza daca procesul s-a incheiat cu succes; 
                //in caz ca nu s-a incheiat cu succes, pe client nu se mai face refresh la grid si nici nu se mai golesc campurile
                //if (trimteMesajFinal) 
                msg = "?0?-Proces finalizat cu succes !";
            }
            catch (Exception ex)
            {
                msg = "Eroare la salvare !";
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

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

        protected void cmbCurs_Callback(object sender, CallbackEventArgsBase e)
        {
            if (cmbAng.Value != null)
            {
                int paramTipFunctii = Convert.ToInt32(Dami.ValoareParam("TipFunctiiCurs"));

                DataTable dt = GetCursuriFiltru(Convert.ToInt32(cmbAng.Value), paramTipFunctii);
                cmbCurs.DataSource = dt;
                cmbCurs.DataBind();

            }
        }
    }
}