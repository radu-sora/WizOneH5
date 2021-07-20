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
    public partial class CursuriView : System.Web.UI.Page
    {

 


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");  
                                
                lblCurs.InnerText = Dami.TraduCuvant("De la");
                lblCurs2.InnerText = Dami.TraduCuvant("Pana la");


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


                DataTable dt1 = General.IncarcaDT(@"SELECT * FROM ""Curs_tblLocatii"" ", null);
                GridViewDataComboBoxColumn col1 = (grDate.Columns["Locatie"] as GridViewDataComboBoxColumn);
                col1.PropertiesComboBox.DataSource = dt1;
    

                grDate.KeyFieldName = "IdAuto";
                DataTable dt = Session["CursuriView_Grid"] as DataTable;
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
                txtDataInc.Value = null;
                txtDataSf.Value = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public DataTable GetCursView(int idUser, int f10003, DateTime dtInc, DateTime dtSf, int tipFunctii)
        {
            DataTable q = null;

            try
            {
                //LeonardM 23.05.2015
                //folosim clauza with de mai jos pentru a afisa trainer-ul de pe sesiune in functie de parametrul 'hasNomenclatorTrainer'
                //daca nu exista, afisam trainer-ul din tabela Curs_tblCursSesiune,
                //in caz contrar, afisam trainer-ii din tabela Curs_relSesiuneTrainer (ultimul trainer pt moment)
                string strSQLWith = @"with cteNomenclatorTraineri (""Valoare"")
                                            as (select {4}a.""Valoare"", 0) as ""Valoare"" 
                                              from 
                                              {5} 
                                              left join ""tblParametrii"" a on 1 = 1
                                                                      and a.""Nume"" = 'hasNomenclatorTraineri'),
                                    cteTraineri (""IdSesiune"", ""IdCurs"", ""TrainerId"", ""TrainerNume"", ""ValoareNomenclatorTrainer"")
                                    as (select ""Id"" as ""IdSesiune"", ""IdCurs"", ""TrainerId"", (SELECT Denumire from Curs_tblTraineri x where x.IdUser = TrainerId) as ""TrainerNume"", 0 as ""ValoareNomenclatorTrainer""
                                    from ""Curs_tblCursSesiune""
                                    union all
                                    select a.""IdSesiune"" as ""IdSesiune"",  a.""IdCurs"", a.""IdTrainer"" as ""TrainerId"", (SELECT Denumire from Curs_tblTraineri x where x.IdUser = a.IdTrainer) as ""TrainerNume"", 1 as ""ValoareNomenclatorTrainer""
                                    from ""Curs_relSesiuneTrainer"" a
                                    inner join (select ""IdCurs"", ""IdSesiune"", max(""IdTrainer"") as ""IdTrainer""
                                                from ""Curs_relSesiuneTrainer"" b
                                                group by b.""IdCurs"", b.""IdSesiune"") b on a.""IdCurs"" = b.""IdCurs""
                                                                                      and a.""IdSesiune"" = b.""IdSesiune""
                                                                                      and a.""IdTrainer"" = b.""IdTrainer"")";
                string strSql = strSQLWith +
                                @"SELECT B.""Id"" as ""IdSesiune"", B.""IdCurs"" as ""IdCurs"",
                                B.""IdAuto"",A.""Denumire"" AS ""Curs"",B.""Denumire"" AS ""Sesiune"",B.""DataInceput"",B.""DataSfarsit"",B.""OraInceput"",B.""OraSfarsit"",B.""TematicaNume"" AS ""Tematica"",
                                case when (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'afisareOrganizatorCursuri') = '0' THEN A.""OrganizatorNume"" ELSE B.""OrganizatorNume"" END AS ""Organizator"",  /*Radu 07.11.2016 - numele organizatorului trebuie luat in functie de parametru*/
                                /*LeonardM 01.09.2015 afisare modalitate finalizare curs + observatii*/
                                A.""FinalizareCurs"" as ""FinalizareCurs"", A.""Observatii"" as ""Observatii"", 
                                /*end LeonardM 01.09.2015*/
                                /*LeonardM 23.05.2015 afisare triner conform nomenclator trainer */
                                /*B.""TrainerNume"" AS ""Trainer"",*/
                                cteTrainer.""TrainerNume"" as ""Trainer"",
                                B.""Locatie"",D.""Denumire"" AS ""Categ_Niv1Nume"",E.""Denumire"" AS ""Categ_Niv2Nume"",F.""Denumire"" AS ""Categ_Niv3Nume"", {4}C.""Denumire"",'') AS ""StareSesiune""
                                FROM ({1}) X
                                INNER JOIN ""Curs_tblCurs"" A ON X.""Id""=A.""Id""
                                INNER JOIN ""Curs_tblCursSesiune"" B ON  A.""Id"" = B.""IdCurs""
                                /*LeonardM 23.05.2015
                                pentru a prelua trainerii in functie de parametrul 'hasNomenclatorTrainer' */
                                inner join cteNomenclatorTraineri cteNomenTrainer on 1 = 1
                                /*LeonardM 11.06.2015
                                am modificat legatura in left join, deoarece pe unele sesiuni e posibil sa fi exclus trainer pe sesiune/curs*/
                                left join cteTraineri cteTrainer on cteNomenTrainer.""Valoare"" = cteTrainer.""ValoareNomenclatorTrainer""
                                                                  and  B.""IdCurs"" = cteTrainer.""IdCurs""
                                                                  and B.""Id"" = cteTrainer.""IdSesiune""
                                LEFT JOIN ""Curs_tblStariSesiune"" C ON B.""IdStare"" = C.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv1"" D ON A.""Categ_Niv1Id"" = D.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv2"" E ON A.""Categ_Niv2Id"" = E.""Id""
                                LEFT JOIN ""Curs_tblCateg_Niv3"" F ON A.""Categ_Niv3Id"" = F.""Id""
                                WHERE {4}A.""Activ"",0)=1 AND B.""IdStare""=2 AND 
                                B.""DataInceput"" <= {2} AND {3} <= B.""DataSfarsit""
                                ORDER BY B.""DataInceput"" ";

                CursuriInregistrare pagCI = new CursuriInregistrare();

                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, f10003, pagCI.CursuriDisponibile(f10003, tipFunctii), General.ToDataUniv(dtSf), General.ToDataUniv(dtInc), "isnull(", "(select 1 as Column1) as dual");
                else
                    strSql = string.Format(strSql, f10003, pagCI.CursuriDisponibile(f10003, tipFunctii), General.ToDataUniv(dtSf), General.ToDataUniv(dtInc), "nvl(", "dual");

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }



        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                DateTime dtInc = Convert.ToDateTime(txtDataInc.Value ?? new DateTime(1900, 1, 1));
                DateTime dtSf = Convert.ToDateTime(txtDataSf.Value ?? new DateTime(2200, 12, 31));

                int paramTipFunctii = Convert.ToInt32(Dami.ValoareParam("TipFunctiiCurs", "0"));

                grDate.KeyFieldName = "IdAuto";
                dt = GetCursView(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(Session["User_Marca"].ToString()), dtInc, dtSf, paramTipFunctii);

                grDate.DataSource = dt;
                grDate.DataBind();
                Session["CursuriView_Grid"] = dt;               
        
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


        


    }
}