using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public partial class EvalLista : System.Web.UI.Page
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
                Session["PaginaWeb"] = "Eval.EvalLista";

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LagSelectorPopup") >= 0)
                    Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                
                foreach(GridViewColumn c in grDate.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataTextColumn) || c.GetType() == typeof(GridViewDataDateColumn) || c.GetType() == typeof(GridViewDataComboBoxColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception ex) { }
                }
                #endregion

                if (Request["pp"] != null)
                    txtTitlu.Text = "Prima Pagina - Evaluare";
                else
                    txtTitlu.Text = General.VarSession("Titlu").ToString();
                grDate.DataBind();

                #region ChestionarBind

                string strSQL = "select * from \"Eval_Quiz\" ";
                DataTable dtQuiz = General.IncarcaDT(strSQL, null);
                if(dtQuiz!=null)
                {
                    cmbQuiz.DataSource = dtQuiz;
                    cmbQuiz.DataBind();
                }

                #endregion

                #region AngajatBind
                strSQL = $@"select fnume.F10003, fnume.F10008 {Dami.Operator()} ' ' {Dami.Operator()} fnume.F10009 as ""NumeComplet"",
		                            fil.F00406 as ""Filiala"", sec.F00507 as ""Sectie"", dep.F00608 as ""Departament""
                            from F100 fnume
                            left join F002 comp on fnume.F10002 = comp.F00202
                            left join F003 subComp on fnume.F10004 = subComp.F00304
                            left join F004 fil on fnume.F10005 = fil.F00405
                            left join F005 sec on fnume.F10006 = sec.F00506
                            left join F006 dep on fnume.F10007 = dep.F00607";
                //if (Constante.tipBD == 1) //SQL
                //    strSQL = string.Format(strSQL, "+");
                //else
                //    strSQL = string.Format(strSQL, "||");
                DataTable dtAngajat = General.IncarcaDT(strSQL, null);
                cmbAngajat.DataSource = dtAngajat;
                cmbAngajat.DataBind();
                #endregion

                #region NivelulMeuBind
                List<metaDate> lstNivelulMeu = new List<metaDate>();

                metaDate clsAleMele = new metaDate() { Id = 1, Denumire = Dami.TraduCuvant("Ale mele") };
                metaDate clsAleAngajatilor = new metaDate() { Id = 2, Denumire = Dami.TraduCuvant("Ale angajatilor") };
                metaDate clsToate = new metaDate() { Id = 3, Denumire = Dami.TraduCuvant("Toate") };
                lstNivelulMeu.Add(clsAleMele);
                lstNivelulMeu.Add(clsAleAngajatilor);
                lstNivelulMeu.Add(clsToate);

                cmbNivel.DataSource = lstNivelulMeu;
                cmbNivel.DataBind();
                #endregion

                #region RolulMeuBind
                DataTable dtRol = Evaluare.GetSupervizoriEval(Convert.ToInt32(Session["UserId"].ToString()));
                cmbRoluri.DataSource = dtRol;
                cmbRoluri.DataBind();
                #endregion

                //string idHR = Dami.ValoareParam("Eval_IDuriRoluriHR", "-99");
                //string sqlHr = $@"SELECT COUNT(""IdUser"") FROM ""F100Supervizori"" WHERE ""IdUser""={HttpContext.Current.Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                //if (Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlHr, null), 0)) != 0) btnModif.Visible = true;

                //Florin 2019.10.23
                //aplicam filtrele
                if (!IsPostBack)
                {
                    string q = General.Nz(Request.QueryString["q"], "").ToString();
                    switch(q)
                    {
                        case "12":
                            //Florin 2019.12.11
                            //grDate.FilterExpression = "[Stare] NOT LIKE '%finalizat%' AND [CategorieQuiz] = 0";
                            grDate.FilterExpression = "[Stare] NOT LIKE '%finalizat%'";
                            break;
                        case "34":
                            grDate.FilterExpression = "[Quiz360Completat] = 0 AND [CategorieQuiz] <> 0";
                            break;
                        case "56":
                            {
                                if (General.Nz(Session["EvalLista_FiltrulGrid"], "").ToString() != "")
                                    grDate.FilterExpression = General.Nz(Session["EvalLista_FiltrulGrid"],"").ToString();

                                if (General.Nz(Session["EvalLista_FiltrulCmb"], "").ToString() != "")
                                {
                                    NameValueCollection lst = HttpUtility.ParseQueryString((Session["EvalLista_FiltrulCmb"] ?? "").ToString());
                                    if (lst.Count > 0)
                                    {
                                        if (General.Nz(lst["Quiz"], "").ToString() != "") cmbQuiz.Value = Convert.ToInt32(lst["Quiz"]);
                                        if (General.Nz(lst["Angajat"], "").ToString() != "") cmbAngajat.Value = Convert.ToInt32(lst["Angajat"]);
                                        if (General.Nz(lst["Nivel"], "").ToString() != "") cmbNivel.Value = Convert.ToInt32(lst["Nivel"]);
                                        if (General.Nz(lst["Roluri"], "").ToString() != "") cmbRoluri.Value = Convert.ToInt32(lst["Roluri"]);

                                        if (General.Nz(lst["DataInc"], "").ToString() != "") dtDataInceput.Value = Convert.ToDateTime(lst["DataInc"]);
                                        if (General.Nz(lst["DataSf"], "").ToString() != "") dtDataSfarsit.Value = Convert.ToDateTime(lst["DataSf"]);
                                    }

                                    btnFiltru_Click(null, null);
                                }
                            }
                            break;
                    }

                    if (General.Nz(Session["EvalLista_FiltrulRowIndex"],"").ToString() != "" && General.IsNumeric(Session["EvalLista_FiltrulRowIndex"]))
                        grDate.FocusedRowIndex = Convert.ToInt32(Session["EvalLista_FiltrulRowIndex"]);

                    Session["EvalLista_FiltrulCmb"] = "";
                    Session["EvalLista_FiltrulGrid"] = "";
                    Session["EvalLista_FiltrulRowIndex"] = "";
                }
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
                cmbNivel.Value = null;
                cmbRoluri.Value = null;
                dtDataInceput.Value = null;
                dtDataSfarsit.Value = null;
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
                    //if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                    //    return;
                    if (arr.Length > 1 && arr[0] == "")
                        return;

                    switch (arr[0])
                    {
                        case "btnEdit":
                            {
                                //object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdQuiz", "F10003", "PozitiePeCircuit", "Finalizat", "PoateModifica", "Utilizator", "TrebuieSaIaLaCunostinta", "ALuatLaCunostinta" }) as object[];
                                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAuto", "IdQuiz", "F10003", "PozitiePeCircuit", "Finalizat", "PoateModifica", "Utilizator", "TrebuieSaIaLaCunostinta", "ALuatLaCunostinta", "Aprobat" }) as object[];
                                if (obj == null || obj.Count() == 0)
                                {
                                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata");
                                    return;
                                }

                                Session["CompletareChestionar_IdQuiz"] = -99;
                                Session["CompletareChestionar_F10003"] = -99;
                                Session["CompletareChestionar_Pozitie"] = -99;
                                Session["CompletareChestionar_Finalizat"] = -99;
                                Session["CompletareChestionar_Modifica"] = -99;
                                Session["CompletareChestionar_Nume"] = "";
                                Session["CompletareChestionar_TrebuieSaIaLaCunostinta"] = "0";
                                Session["CompletareChestionar_ALuatLaCunostinta"] = "0";

                                //Florin 2019.01.31  Cerinta de la Claim
                                //daca utilizatorul logat este HR si avem rolul de HR pe circuit, actualizam in istoric cu userul logat
                                try
                                {
                                    string idHR = Dami.ValoareParam("Eval_IDuriRoluriHR", "-99");
                                    if (idHR.Trim() != "")
                                        General.ExecutaNonQuery(
                                            $@"UPDATE ""Eval_RaspunsIstoric"" 
                                            SET ""IdUser""=@3
                                            WHERE ""IdQuiz""=@1 AND F10003=@2 AND 
                                            (SELECT COUNT(""IdUser"") FROM ""F100Supervizori"" WHERE ""IdUser""=@3 AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"") <> 0 AND
                                            (-1 * ""IdSuper"") IN ({idHR})", new object[] { Convert.ToInt32(General.Nz(obj[1], 1)), Convert.ToInt32(General.Nz(obj[2], 1)), Session["UserId"] });
                                }
                                catch (Exception) { }

                                //Florin 2020.01.23 - Begin - am pus toate cumpurile care ne intereseaza in grid, si luam valorile de acolo

                                #region OLD

                                //string strSql = Evaluare.GetEvalLista(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbQuiz.Value ?? -99), Convert.ToInt32(cmbAngajat.Value ?? -99),
                                //    Convert.ToDateTime(dtDataInceput.Value ?? new DateTime(1900, 1, 1)), Convert.ToDateTime(dtDataSfarsit.Value ?? new DateTime(1900, 1, 1)),
                                //    Convert.ToInt32(cmbNivel.Value ?? -99), Convert.ToInt32(cmbRoluri.Value ?? -99), 1, Convert.ToInt32(General.Nz(obj[0],-99)));
                                //DataTable dt = General.IncarcaDT(strSql, null);

                                //if (dt != null && dt.Rows.Count > 0)
                                //{
                                //    DataRow dr = dt.Rows[0];
                                //    Session["CompletareChestionar_IdQuiz"] = Convert.ToInt32(General.Nz(dr["IdQuiz"],-99));
                                //    Session["CompletareChestionar_F10003"] = Convert.ToInt32(General.Nz(dr["F10003"],-99));
                                //    Session["CompletareChestionar_Pozitie"] = Convert.ToInt32(General.Nz(dr["PozitiePeCircuit"], -99));
                                //    Session["CompletareChestionar_Finalizat"] = Convert.ToInt32(General.Nz(dr["Finalizat"],-99));
                                //    Session["CompletareChestionar_Modifica"] = Convert.ToInt32(General.Nz(dr["PoateModifica"],-99));
                                //    Session["CompletareChestionar_Nume"] = General.Nz(dr["Utilizator"],"");
                                //    Session["CompletareChestionar_TrebuieSaIaLaCunostinta"] = General.Nz(dr["TrebuieSaIaLaCunostinta"],"0");
                                //    Session["CompletareChestionar_ALuatLaCunostinta"] = General.Nz(dr["ALuatLaCunostinta"],"0");

                                //    //Session["lstEval_ObiIndividualeTemp"] = null;
                                //    //Session["lstEval_CompetenteAngajatTemp"] = null;
                                //    //Session["CompletareChestionar_IdQuiz"] = Convert.ToInt32(obj[0] ?? -99);
                                //    //Session["CompletareChestionar_F10003"] = Convert.ToInt32(obj[1] ?? -99);
                                //    //Session["Eval_PozitieUserLogat"] = General.Nz(General.ExecutaScalar($@"SELECT ""Pozitie"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""={ Convert.ToInt32(obj[0] ?? -99)} AND F10003={ Convert.ToInt32(obj[1] ?? -99)} AND ""IdUser""={Session["UserId"]}", null), 1);
                                //    //Session["IdClient"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='IdClient'", null), 1));
                                //    //Session["CompletareChestionar_Pozitie"] = Convert.ToInt32(obj[2] ?? -99);
                                //    //Session["CompletareChestionar_Finalizat"] = Convert.ToInt32(obj[3] ?? -99);
                                //    //Session["CompletareChestionar_Modifica"] = Convert.ToInt32(obj[4] ?? -99);
                                //    //Session["CompletareChestionar_Nume"] = obj[5] ?? "";
                                //    //Session["CompletareChestionar_TrebuieSaIaLaCunostinta"] = obj[6] ?? "0";
                                //    //Session["CompletareChestionar_ALuatLaCunostinta"] = obj[7] ?? "0";
                                //}

                                #endregion

                                Session["CompletareChestionar_IdQuiz"] = Convert.ToInt32(General.Nz(obj[1], -99));
                                Session["CompletareChestionar_F10003"] = Convert.ToInt32(General.Nz(obj[2], -99));
                                Session["CompletareChestionar_Pozitie"] = Convert.ToInt32(General.Nz(obj[3], -99));
                                Session["CompletareChestionar_Finalizat"] = Convert.ToInt32(General.Nz(obj[4], -99));
                                Session["CompletareChestionar_Modifica"] = Convert.ToInt32(General.Nz(obj[5], -99));
                                Session["CompletareChestionar_Nume"] = General.Nz(obj[6], "");
                                Session["CompletareChestionar_TrebuieSaIaLaCunostinta"] = General.Nz(obj[7], "0");
                                Session["CompletareChestionar_ALuatLaCunostinta"] = General.Nz(obj[8], "0");
                                Session["CompletareChestionar_Aprobat"] = General.Nz(obj[9], "0");

                                //Florin 2020.01.23 - End

                                Session["lstEval_ObiIndividualeTemp"] = null;
                                Session["lstEval_CompetenteAngajatTemp"] = null;
                                Session["Eval_PozitieUserLogat"] = General.Nz(General.ExecutaScalar($@"SELECT ""Pozitie"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""={ Convert.ToInt32(obj[1] ?? -99)} AND F10003={ Convert.ToInt32(obj[2] ?? -99)} AND ""IdUser""={Session["UserId"]}", null), 1);
                                Session["IdClient"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='IdClient'", null), 1));


                                //Florin 2019.06.27
                                Session["lstEval_ObiIndividualeTemp_Sterse"] = null;
                                Session["lstEval_CompetenteAngajatTemp_Sterse"] = null;

                                //Florin 2019.10.23 - retinem filtrul
                                #region Salvam Filtrul

                                string req = "";
                                if (cmbQuiz.Value != null) req += "&Quiz=" + cmbQuiz.Value;
                                if (cmbAngajat.Value != null) req += "&Angajat=" + cmbAngajat.Value;
                                if (cmbNivel.Value != null) req += "&Nivel=" + cmbNivel.Value;
                                if (cmbRoluri.Value != null) req += "&Roluri=" + cmbRoluri.Value;
                                if (dtDataInceput.Value != null) req += "&DataInc=" + dtDataInceput.Value;
                                if (dtDataSfarsit.Value != null) req += "&DataSf=" + dtDataSfarsit.Value;

                                Session["EvalLista_FiltrulCmb"] = req;
                                Session["EvalLista_FiltrulGrid"] = grDate.FilterExpression;
                                Session["EvalLista_FiltrulRowIndex"] = grDate.FocusedRowIndex;

                                #endregion

                                if (Page.IsCallback)
                                    ASPxWebControl.RedirectOnCallback("~/Eval/EvalDetaliu.aspx");
                                else
                                    Response.Redirect("~/Eval/EvalDetaliu.aspx", false);

                                #region OLD
                                //string url = "~/Eval/EvalDetaliu.aspx";
                                ////string url = "~/Eval/EvalTemp.aspx";
                                //if (url != "")
                                //{
                                //    Session["IdEvalRaspuns"] = arr[1];
                                //    DataTable table = General.IncarcaDT(@"select * from ""Eval_Raspuns"" where ""IdAuto"" = " + arr[1], null);
                                //    table.TableName = "Eval_Raspuns";
                                //    if (table != null && table.Rows.Count != 0)
                                //    {
                                //        int IdQuiz = -99;
                                //        int F10003 = -99;
                                //        IdQuiz = Convert.ToInt32(table.Rows[0]["IdQuiz"].ToString());
                                //        F10003 = Convert.ToInt32(table.Rows[0]["F10003"].ToString());


                                //        //Florin 2019.01.21
                                //        //s-a mutat in EvalDetaliu

                                //        //DataTable table0 = General.IncarcaDT(@"select * from ""Eval_RaspunsLinii"" where ""IdQuiz"" = " + IdQuiz + @" and ""F10003"" = " + F10003, null);
                                //        //table0.TableName = "Eval_RaspunsLinii";

                                //        //DataTable table1 = General.IncarcaDT(@"select * from ""Eval_QuizIntrebari"" where ""IdQuiz"" = " + IdQuiz, null);
                                //        //table1.TableName = "Eval_QuizIntrebari";

                                //        //DataSet ds = new DataSet();
                                //        //ds.Tables.Add(table);
                                //        //table0.PrimaryKey = new DataColumn[] { table0.Columns["Id"] };
                                //        //ds.Tables.Add(table0);
                                //        //table1.PrimaryKey = new DataColumn[] { table1.Columns["Id"] };
                                //        //ds.Tables.Add(table1);

                                //        //Session["InformatiaCurentaCompletareChestionar"] = ds;



                                //        Session["lstEval_ObiIndividualeTemp"] = null;
                                //        Session["CompletareChestionar_IdQuiz"] = IdQuiz;
                                //        Session["CompletareChestionar_F10003"] = F10003;
                                //        //Session["Eval_ActiveTab"] = 0;

                                //        //Florin 2019.01.14
                                //        Session["IdClient"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT ""Valoare"" FROM ""tblParametrii"" WHERE ""Nume""='IdClient'", null), 1));

                                //        //Florin 2018.12.10
                                //        Session["Eval_PozitieUserLogat"] = General.Nz(General.ExecutaScalar($@"SELECT ""Pozitie"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""={IdQuiz} AND F10003={F10003} AND ""IdUser""={Session["UserId"]}", null),1);
                                //        var ert = Session["Eval_PozitieUserLogat"];
                                //        //try
                                //        //{
                                //        //    Session["Eval_ActiveTab"] = Convert.ToInt32(Session["Eval_PozitieUserLogat"]) - 1;
                                //        //}catch (Exception){}


                                //        var entRow = grDate.GetRowValues(GetCurrentIndex(), "PozitiePeCircuit");
                                //        if (entRow != null)
                                //            Session["CompletareChestionar_Pozitie"] = Convert.ToInt32(entRow.ToString() == string.Empty ? "1" : entRow.ToString());
                                //        var entRowFin = grDate.GetRowValues(GetCurrentIndex(), "Finalizat");
                                //        if (entRowFin != null)
                                //            Session["CompletareChestionar_Finalizat"] = Convert.ToInt32(entRowFin.ToString() == string.Empty ? "0" : entRowFin.ToString());
                                //        var entRowModif = grDate.GetRowValues(GetCurrentIndex(), "PoateModifica");
                                //        if (entRowModif != null)
                                //            Session["CompletareChestionar_Modifica"] = Convert.ToInt32(entRowModif.ToString() == string.Empty ? "0" : entRowModif.ToString());
                                //        var entRowNume = grDate.GetRowValues(GetCurrentIndex(), "Utilizator");
                                //        if (entRowNume != null)
                                //            Session["CompletareChestionar_Nume"] = entRowNume.ToString();
                                //        if (Page.IsCallback)
                                //            ASPxWebControl.RedirectOnCallback(url);
                                //        else
                                //            Response.Redirect(url, false);
                                //    }
                                //}
                                #endregion
                            }
                            break;
                        case "btnModif":
                            btnModif_Click(null, null);
                            break;                       
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_DataBinding(object sender, EventArgs e)
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

        protected void grDate_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "Stare")
                {
                    string idStare = e.GetValue("Stare").ToString();
                    string culoare = e.GetValue("Culoare").ToString();
                    if (idStare.ToLower() == "evaluare finalizata" || idStare.ToLower() == "finalizat")
                        culoare = "#FF96fa96";
                    if (!string.IsNullOrEmpty(culoare))
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(culoare);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void txtObs_Init(object sender, EventArgs e)
        {
            try
            {
                ASPxMemo txt = sender as ASPxMemo;
                GridViewDataColumn col = grDate.Columns["Observatii"] as GridViewDataColumn;
                txt.Enabled = !col.ReadOnly;
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
                //Florin 2018.12.17
                //dt = Evaluare.GetEvalLista(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbQuiz.Value ?? -99), Convert.ToInt32(cmbAngajat.Value ?? -99),
                //                            Convert.ToDateTime(dtDataInceput.Value ?? new DateTime(1900, 1, 1)), Convert.ToDateTime(dtDataSfarsit.Value ?? new DateTime(1900, 1, 1)),
                //                            Convert.ToInt32(cmbNivel.Value ?? -99), Convert.ToInt32(cmbRoluri.Value ?? -99));

                string strSql = Evaluare.GetEvalLista(Convert.ToInt32(Session["UserId"].ToString()), Convert.ToInt32(cmbQuiz.Value ?? -99), Convert.ToInt32(cmbAngajat.Value ?? -99),
                            Convert.ToDateTime(dtDataInceput.Value ?? new DateTime(1900, 1, 1)), Convert.ToDateTime(dtDataSfarsit.Value ?? new DateTime(1900, 1, 1)),
                            Convert.ToInt32(cmbNivel.Value ?? -99), Convert.ToInt32(cmbRoluri.Value ?? -99));

                dt = General.IncarcaDT(strSql, null);
                grDate.KeyFieldName = "IdAuto; IdQuiz; F10003";
                grDate.DataSource = dt;
            }
            catch(Exception ex)
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

        private int GetCurrentIndex()
        {
            int idx = -1;

            try
            {
                if (hfVisibleIndex.Contains("Index")) idx = Convert.ToInt32(General.Nz(hfVisibleIndex["Index"],-1));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return idx;

            //object idx = null;
            //if (hfVisibleIndex.TryGet("CurrentObjective", out idx))
            //    return Convert.ToInt32(idx);
            //return -1;
        }

        protected void btnModif_Click(object sender, EventArgs e)
        {
            try
            {
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdQuiz", "F10003", "PozitiePeCircuit", "Finalizat", "CategorieQuiz" }) as object[];
                if (obj == null || obj.Count() == 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu exista linie selectata1");
                    return;
                }

                if (obj[4] != null && obj[4].ToString() != "0")
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica starea acestui tip de chestionar!");
                    return;
                }

                if (obj[3] != null && obj[3].ToString() == "1")
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica starea deoarece chestionarul este finalizat!");
                    return;
                }

                if (obj[2] == null || obj[2].ToString().Length <= 0 || Convert.ToInt32(obj[2].ToString()) < 2)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica starea deoarece chestionarul este pe prima pozitie!");
                    return;
                }

                bool poateModifica = false;

                string idHR = Dami.ValoareParam("Eval_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT COUNT(""IdUser"") FROM ""F100Supervizori"" WHERE ""IdUser""={HttpContext.Current.Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                if (Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlHr, null), 0)) != 0) poateModifica = true;

                if (!poateModifica)
                {//cautare utilizator conectat in istoric
                    string sql = "select  case when b.\"IdSuper\" > 0 and {0} = b.\"IdUser\" then 1 "
                            + "  when b.\"IdSuper\" < 0 and {0} in (select a.\"IdUser\" from \"F100Supervizori\" a where a.\"IdSuper\" = -1 * b.\"IdSuper\" and a.f10003 = b.f10003) then 1 "
                            + " else 0 end as drept from \"Eval_RaspunsIstoric\" b where \"IdQuiz = {1} and f10003 = {2} and \"Pozitie\" = 2";
                    sql = string.Format(sql, Session["UserId"].ToString(), obj[0], obj[1]);
                    DataTable dt = General.IncarcaDT(sql, null);
                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                        poateModifica = true;
                }

                if (!poateModifica)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica starea deoarece nu aveti drepturi pe acest chestionar!");
                    return;
                }

                string strSql = 
                    $@"BEGIN

                    UPDATE ""Eval_Raspuns"" SET ""LuatLaCunostinta""=null, ""Pozitie""=""Pozitie""-1 
                    WHERE ""IdQuiz""=@1 AND F10003=@2 ;
                    
                    UPDATE ""Eval_RaspunsIstoric"" SET ""Aprobat""=null, ""DataAprobare""=null
                    WHERE ""IdQuiz""=@1 AND F10003=@2 AND 
                    COALESCE((SELECT COALESCE(""Pozitie"",0) FROM ""Eval_Raspuns"" WHERE ""IdQuiz""=@1 AND F10003=@2),0) <= ""Pozitie"";
                    
                    END;";

                General.ExecutaNonQuery(strSql, new object[] { obj[0], obj[1] });

                grDate.DataBind();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



    }
}