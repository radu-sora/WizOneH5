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
    public partial class CursuriIstoric : System.Web.UI.Page
    {

 


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");  
                
                lblAng.InnerText = Dami.TraduCuvant("Angajat");
                lblCurs.InnerText = Dami.TraduCuvant("Curs");
                lblSesiune.InnerText = Dami.TraduCuvant("Sesiune");


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
        

                int hasNomenclatorTraineri = Convert.ToInt32(Dami.ValoareParam("hasNomenclatorTraineri", "0"));

                grDate.Columns["colNomenclatorTrainer"].Visible = (hasNomenclatorTraineri == 1) ? true : false;
                grDate.Columns["Trainer"].Visible = (hasNomenclatorTraineri == 0) ? true : false;         

                Aprobare pagApr = new Aprobare();
                DataTable dtAng = pagApr.GetCursAngajati(General.VarSession("EsteAdmin").ToString() == "1" ? 0 : 3, Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()));
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();

                DataTable dtCurs = General.IncarcaDT("SELECT * FROM \"Curs_tblCurs\"", null);
                cmbCurs.DataSource = dtCurs;
                cmbCurs.DataBind();

                DataTable dtDep = General.IncarcaDT("SELECT * FROM F006", null);
                cmbDepartament.DataSource = dtDep;
                cmbDepartament.DataBind();

                DataTable dtNiv1 = General.IncarcaDT("SELECT * FROM Curs_tblCateg_Niv1", null);
                cmbCateg_Niv1.DataSource = dtNiv1;
                cmbCateg_Niv1.DataBind();


                DataTable dtNiv2 = General.IncarcaDT("SELECT * FROM Curs_tblCateg_Niv2", null);
                cmbCateg_Niv2.DataSource = dtNiv2;
                cmbCateg_Niv2.DataBind();

                grDate.KeyFieldName = "IdAuto";
                DataTable dt = Session["CursuriIstoric_Grid"] as DataTable;
                grDate.DataSource = dt;
                grDate.DataBind();
         

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
                cmbCurs.Value = null;
                cmbSesiune.Value = null;
                cmbCateg_Niv1.Value = null;
                cmbCateg_Niv2.Value = null;
                cmbDepartament.Value = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
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
                else
                {
                    cmbSesiune.DataSource = null;
                    cmbSesiune.DataBind();
                }
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
                grDate.KeyFieldName = "IdAuto";
                dt = GetCursIstoric(Convert.ToInt32(cmbAng.Value ?? -99), Convert.ToInt32(cmbCurs.Value ?? -99), Convert.ToInt32(cmbSesiune.Value ?? -99), Convert.ToString(cmbCateg_Niv1.Text ?? ""),
                    Convert.ToString(cmbCateg_Niv2.Text ?? ""), "", Convert.ToString(cmbDepartament.Text ?? ""), Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()));

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["CursuriIstoric_Grid"] = dt;               
        
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


        public DataTable GetCursIstoric(int f10003, int idCurs, int idSesiune, string niv1, string niv2, string divizie, string dept, int idUser, int marcaMea)
        {
            DataTable q = null;

            try
            {
                string strFiltru = "";
                string strSql = @"SELECT a.""IdAuto"",a.F10003,b.F10008 || ' ' || b.F10009 AS ""NumeAngajat"",a.""IdCurs"",a.""IdStare"",c.""Denumire"" AS ""Curs"",a.""IdSesiune"",d.""Denumire"" AS ""Sesiune"",D.""OrganizatorNume"" AS ""Organizator"",D.""TrainerNume"" AS ""Trainer"",
                                C.""CertificariNume"" AS ""Certificat"",E.""Denumire"" AS ""Stare"",N1.""Denumire"" AS ""Categ_Niv1"",N2.""Denumire"" AS ""Categ_Niv2"",F.F00507 AS ""Divizie"",G.F00608 AS ""Departament"",a.""CostRONcuTVA"" AS ""Planificare_CostRONcuTVA"",
                                a.""Denumire"" AS ""Planificare_Denumire"",a.""Organizator"" AS ""Planificare_Organizator"",a.""Culoare"", note.""DenumireValoare"" as ""Nota""
                                FROM ""Curs_Inregistrare"" A
                                INNER JOIN F100 B ON A.F10003=B.F10003
                                LEFT JOIN ""Curs_tblCurs"" C ON a.""IdCurs"" = c.""Id""
                                LEFT JOIN ""Curs_tblCursSesiune"" D ON a.""IdCurs""=D.""IdCurs"" AND a.""IdSesiune"" = D.""Id""
                                LEFT JOIN ""Curs_tblStari"" E ON a.""IdStare"" = e.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv1"" N1 ON C.""Categ_Niv1Id"" = N1.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv2"" N2 ON C.""Categ_Niv2Id"" = N2.""Id""
                                LEFT JOIN ""Curs_tblCategNoteValori"" note on A.""IdCategValoareNota"" = note.""IdCategValoare"" and C.""IdCategorieNota"" = note.""IdCategorie""
                                LEFT JOIN F005 F ON B.F10006 = F.F00506
                                LEFT JOIN F006 G ON B.F10007 = G.F00607
                                UNION
                                SELECT a.""IdAuto"",c.F10003,c.F10008 || ' ' || c.F10009 AS ""NumeAngajat"",-99 AS ""IdCurs"",a.""IdStare"",nvl(A.""DenCurs"", A.""CursNume"") AS ""Curs"",-99 AS ""IdSesiune"",A.""SesiuneNume"" AS ""Sesiune"",A.""Organizator"",'' AS ""Trainer"",
                                '' AS ""Certificat"",B.""Denumire"" AS ""Stare"",'' AS ""Categ_Niv1"",'' AS ""Categ_Niv2"",D.F00507 AS ""Divizie"",E.F00608 AS ""Departament"",-99 AS ""Planificare_CostRONcuTVA"",
                                '' AS ""Planificare_Denumire"",'' AS ""Planificare_Organizator"",a.""CuloareStare"" AS ""Culoare"", '0' as ""Nota""
                                FROM ""Curs_Anterior"" A
                                INNER JOIN ""Curs_tblStariAnterior"" B ON a.""IdStare"" = b.""Id""
                                INNER JOIN F100 C ON a.F10003 = c.F10003
                                LEFT JOIN F005 D ON c.F10006 = d.F00506
                                LEFT JOIN F006 E ON c.F10007 = e.F00607";

                if (Constante.tipBD == 1)
                    strSql = @"SELECT a.""IdAuto"",a.F10003,b.F10008 + ' ' + b.F10009 AS ""NumeAngajat"",a.""IdCurs"",a.""IdStare"",c.""Denumire"" AS ""Curs"",a.""IdSesiune"",d.""Denumire"" AS ""Sesiune"",D.""OrganizatorNume"" AS ""Organizator"",D.""TrainerNume"" AS ""Trainer"",
                                C.""CertificariNume"" AS ""Certificat"",E.""Denumire"" AS ""Stare"",N1.""Denumire"" AS ""Categ_Niv1"",N2.""Denumire"" AS ""Categ_Niv2"",F.F00507 AS ""Divizie"",G.F00608 AS ""Departament"",a.""CostRONcuTVA"" AS ""Planificare_CostRONcuTVA"",
                                a.""Denumire"" AS ""Planificare_Denumire"",a.""Organizator"" AS ""Planificare_Organizator"",a.""Culoare"", note.""DenumireValoare"" as ""Nota""
                                FROM ""Curs_Inregistrare"" A
                                INNER JOIN F100 B ON A.F10003=B.F10003
                                LEFT JOIN ""Curs_tblCurs"" C ON a.""IdCurs"" = c.""Id""
                                LEFT JOIN ""Curs_tblCursSesiune"" D ON a.""IdCurs""=D.""IdCurs"" AND a.""IdSesiune"" = D.""Id""
                                LEFT JOIN ""Curs_tblStari"" E ON a.""IdStare"" = e.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv1"" N1 ON C.""Categ_Niv1Id"" = N1.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv2"" N2 ON C.""Categ_Niv2Id"" = N2.""Id""
                                LEFT JOIN ""Curs_tblCategNoteValori"" note on A.""IdCategValoareNota"" = note.""IdCategValoare"" and C.""IdCategorieNota"" = note.""IdCategorie""
                                LEFT JOIN F005 F ON B.F10006 = F.F00506
                                LEFT JOIN F006 G ON B.F10007 = G.F00607
                                UNION
                                SELECT a.""IdAuto"",c.F10003,c.F10008 + ' ' + c.F10009 AS ""NumeAngajat"",-99 AS ""IdCurs"",a.""IdStare"",isnull(A.""DenCurs"", A.""CursNume"") AS ""Curs"",-99 AS ""IdSesiune"",A.""SesiuneNume"" AS ""Sesiune"",A.""Organizator"",'' AS ""Trainer"",
                                '' AS ""Certificat"",B.""Denumire"" AS ""Stare"",'' AS ""Categ_Niv1"",'' AS ""Categ_Niv2"",D.F00507 AS ""Divizie"",E.F00608 AS ""Departament"",-99 AS ""Planificare_CostRONcuTVA"",
                                '' AS ""Planificare_Denumire"",'' AS ""Planificare_Organizator"",a.""CuloareStare"" AS ""Culoare"", '0' as ""Nota""
                                FROM ""Curs_Anterior"" A
                                INNER JOIN ""Curs_tblStariAnterior"" B ON a.""IdStare"" = b.""Id""
                                INNER JOIN F100 C ON a.F10003 = c.F10003
                                LEFT JOIN F005 D ON c.F10006 = d.F00506
                                LEFT JOIN F006 E ON c.F10007 = e.F00607";

                if (f10003 != -99)
                {
                    //filtru dupa angajatul pe care l-am selectat
                    strFiltru += " AND X.F10003 = " + f10003;
                }
                else
                {
                    //daca nu selectez nici un angajat, atunci arat toate cursurile angajatilor la care eu sunt manager + cursurile mele + toate cursurile daca eu sunt trainer
                    strFiltru += "AND (X.F10003 = " + marcaMea + " OR X.F10003 IN (SELECT F10003 FROM \"F100Supervizori\" WHERE \"IdUser\"=" + idUser + " AND \"IdSuper\"=1) OR (SELECT COUNT(*) FROM Curs_tblCursSesiune a LEFT JOIN Curs_tblTraineri b on  a.TrainerId=b.id where b.iduser =" + idUser + ")>0)";
                }
                if (idCurs != -99) strFiltru += " AND X.\"IdCurs\" = " + idCurs;
                if (idSesiune != -99) strFiltru += " AND X.\"IdSesiune\" = " + idSesiune;
                if (niv1 != "") strFiltru += " AND X.\"Categ_Niv1\" = '" + niv1 + "'";
                if (niv2 != "") strFiltru += " AND X.\"Categ_Niv2\" = '" + niv2 + "'";
                if (divizie != "") strFiltru += " AND X.\"Divizie\" = '" + divizie + "'";
                if (dept != "") strFiltru += " AND X.\"Departament\" = '" + dept + "'";

                strSql = "SELECT X.* FROM (" + strSql + ") X WHERE 1=1 " + strFiltru;

                q = General.IncarcaDT(strSql, null);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        


    }
}