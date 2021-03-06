using DevExpress.Web;
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

namespace WizOne.Eval
{
    public partial class QuizInit : System.Web.UI.Page
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
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LagSelectorPopup") >= 0)
                    Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnInit.Text = Dami.TraduCuvant("btnInit", "Initiaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

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
                grDate.DataBind();

                DataTable dtQuiz = General.IncarcaDT("SELECT Id, Denumire FROM Eval_Quiz WHERE COALESCE(Activ,0) = 1 ORDER BY Denumire");
                DataTable dtUsers = General.IncarcaDT("SELECT F70104, F70102 FROM USERS ORDER BY NumeComplet");

                cmbQuiz.DataSource = dtQuiz;
                cmbQuiz.DataBind();

                string strSqL = $@"select fnume.F10003, fnume.F10008 {Dami.Operator()} ' ' {Dami.Operator()} fnume.F10009 as ""NumeComplet"",
		                            fil.F00406 as ""Filiala"", sec.F00507 as ""Sectie"", dep.F00608 as ""Departament""
                            from F100 fnume
                            left join F002 comp on fnume.F10002 = comp.F00202
                            left join F003 subComp on fnume.F10004 = subComp.F00304
                            left join F004 fil on fnume.F10005 = fil.F00405
                            left join F005 sec on fnume.F10006 = sec.F00506
                            left join F006 dep on fnume.F10007 = dep.F00607 WHERE fnume.F10025 IN (0, 999)";

                DataTable dtAngajat = General.IncarcaDT(strSqL, null);
                cmbAngajat.DataSource = dtAngajat;
                cmbAngajat.DataBind();

                cmbQuizModif.DataSource = dtQuiz;
                cmbQuizModif.DataBind();

                cmbUserOld.DataSource = dtUsers;
                cmbUserOld.DataBind();

                cmbUserNew.DataSource = dtUsers;
                cmbUserNew.DataBind();
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
                string strSql = @"select q.""Id"" * 100000 + fnume.""F10003"" as ""IdAuto"",
		                                fnume.F10003 as F10003,
		                                fnume.F10008 {0} ' ' {0} fnume.F10009 as ""NumeComplet"",
		                                q.""Id"" as ""IdQuiz"",
		                                q.""Denumire"" as ""Quiz"",
		                                q.""DataInceput"", q.""DataSfarsit"",
		                                case when {1}(rasp.""IdQuiz"", -99) = -99 then 'Neinitiat' else 'Initiat' end as ""Stare"",
		                                case when {1}(rasp.""IdQuiz"", -99) = -99 then '#ffffc8c8' else '#ffc8ffc8' end as ""Culoare"",
		                                case when {1}(rasp.""Finalizat"", -99) = 1 then 1 else 0 end as ""Finalizat""
                                from ""Eval_Quiz"" q
                                join ""Eval_relGrupAngajatQuiz"" relGrup on q.""Id"" = relGrup.""IdQuiz""
                                join ""Eval_SetAngajatiDetail"" angDet on relGrup.""IdGrup"" = angDet.""IdSetAng""
                                join F100 fnume on angDet.""Id"" = fnume.F10003
                                left join ""Eval_Raspuns"" rasp on q.""Id"" = rasp.""IdQuiz""
                                                       and fnume.F10003 = rasp.F10003
                                where q.""Id"" = {2}
                                and angDet.""Id"" = {3}  and fnume.F10025 IN (0, 999)";
                if (Constante.tipBD == 1) //SQL
                    strSql = string.Format(strSql, "+", "isnull",
                            (cmbQuiz.Value == null ? @"q.""Id""" : cmbQuiz.Value.ToString()),
                            (cmbAngajat.Value == null ? @"angDet.""Id""" : cmbAngajat.Value.ToString()));
                else
                    strSql = string.Format(strSql, "||", "nvl",
                        (cmbQuiz.Value == null ? @"q.""Id""" : cmbQuiz.Value.ToString()),
                        (cmbAngajat.Value == null ? @"angDet.""Id""" : cmbAngajat.Value.ToString()));

                dt = General.IncarcaDT(strSql, null);
                grDate.KeyFieldName = "IdAuto; F10003; IdQuiz";
                grDate.DataSource = dt;
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
        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                grDate.DataBind();
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
                cmbQuiz.Value = null;
                cmbAngajat.Value = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, DevExpress.Web.ASPxGridViewCustomCallbackEventArgs e)
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
                        case "btnInit":
                            btnInit_Click(null, null);
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
        protected void btnInit_Click(object sender, EventArgs e)
        {
            try
            {
                List<metaQuizAngajat> ids = new List<metaQuizAngajat>();
                string msg = "";
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "IdQuiz", "F10003", "NumeComplet", "Quiz" });
                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                for (int i = 0; i < lst.Count(); i++)
                {
                    object[] arr = lst[i] as object[];
                    ids.Add(new metaQuizAngajat { IdQuiz = Convert.ToInt32(General.Nz(arr[0], 0)), F10003 = Convert.ToInt32(General.Nz(arr[1], 0)) });
                }

                if (ids.Count != 0) msg += Evaluare.InitializareChestionar(ids, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                                
                grDate.JSProperties["cpAlertMessage"] = msg;
                grDate.DataBind();
                grDate.Selection.UnselectAll();                
            }
            catch(Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "Stare")
                {
                    string idStare = e.GetValue("Stare").ToString();
                    string culoare = e.GetValue("Culoare").ToString();
                    if(!string.IsNullOrEmpty(culoare))
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(culoare);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnModif_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbQuizModif.Value == null || cmbUserOld.Value == null || cmbUserNew.Value == null)
                    MessageBox.Show("Lipsesc date", MessageBox.icoError);
                else
                {
                    int nr = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"
                        BEGIN
                            UPDATE Eval_RaspunsIstoric SET IdUser = @3 WHERE IdQuiz = @1 AND IdUser = @2;
                            SELECT @@ROWCOUNT;
                        END", new object[] { cmbQuizModif.Value, cmbUserOld.Value, cmbUserNew.Value }),0));

                    if (nr == 0)
                        MessageBox.Show("Nu a fost actualizata nici o inregistrare", MessageBox.icoError);
                    else
                        MessageBox.Show("Au fost actualizate " + nr + " inregistrari", MessageBox.icoSuccess);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}