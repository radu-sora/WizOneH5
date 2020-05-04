using DevExpress.Web;
using System;
using System.Collections;
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
    public partial class QuizLista : System.Web.UI.Page
    {
        //string cmp = "USER_NO,TIME,IDAUTO,";

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                foreach(GridViewColumn c in grDate.Columns)
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

                txtTitlu.Text = (Session["Titlu"] ?? "").ToString();
                if(!IsPostBack)
                {

                    Session["Eval_QuizListaPerioada"] = General.IncarcaDT(@"select ""IdPerioada"" as ""Id"", ""DenPerioada"" as ""Denumire"" from ""Eval_Perioada"" ", null);
                    cmbPerioada.DataSource = Session["Eval_QuizListaPerioada"];
                    cmbPerioada.DataBind();

                    IncarcaGrid();
                    btnNew.Attributes.Add("onclik", "window.open('QuizDetaliu.aspx', null, '', null);");

                    #region Reset DataSet
                    Session["createEval_ConfigObTemplate"] = null;
                    Session["createEval_ConfigObTemplateDetail"] = null;
                    Session["createEval_ConfigCompTemplate"] = null;
                    Session["createEval_ConfigCompTemplateDetail"] = null;

                    Session["createEval_ConfigObiective"] = null;
                    Session["editingObiectivTemplate"] = null;
                    Session["isEditingObiectivTemplate"] = null;
                    Session["nomenEval_DictionaryItem"] = null;
                    Session["nomenVwEval_ConfigObiectivCol"] = null;

                    Session["createEval_ConfigCompetente"] = null;
                    Session["editingCompetenteTemplate"] = null;
                    Session["isEditingcompetenteTemplate"] = null;
                    Session["nomenVwEval_ConfigCompetenteCol"] = null;
                    #endregion
                }
                else
                {
                    cmbPerioada.DataSource = Session["Eval_QuizListaPerioada"];
                    cmbPerioada.DataBind();
                    grDate.DataSource = Session["InformatiaCurentaQuiz"];
                    grDate.KeyFieldName = "Id";
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string str = e.Parameters;
                if(str!="")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "" || arr[1] == "")
                        return;

                    switch(arr[0])
                    {
                        case "btnEdit":
                            {
                                //Florin 2020.01.30
                                Session["Eval_QuizSetAngajati"] = null;

                                string url = "~/Eval/QuizDetaliu.aspx";
                                if(url!="")
                                {
                                    Session["IdEvalQuiz"] = arr[1];
                                    DataTable table = General.IncarcaDT(@"select * from ""Eval_Quiz"" where ""Id"" = " + arr[1], null);
                                    DataTable table0 = General.IncarcaDT(@"select * from ""Eval_QuizIntrebari"" where ""IdQuiz"" = " + arr[1], null);
                                    DataTable table1 = General.IncarcaDT(@"select * from ""Eval_Circuit"" where ""IdQuiz"" = " + arr[1], null);
                                    DataTable table2 = General.IncarcaDT(@"select * from ""Eval_Drepturi"" where ""IdQuiz"" = " + arr[1], null);

                                    DataSet ds = new DataSet();
                                    table.TableName = "Eval_Quiz";
                                    table.PrimaryKey = new DataColumn[] { table.Columns["Id"] };
                                    ds.Tables.Add(table);
                                    table0.TableName = "Eval_QuizIntrebari";
                                    table0.PrimaryKey = new DataColumn[] { table0.Columns["Id"] };
                                    ds.Tables.Add(table0);
                                    table1.TableName = "Eval_Circuit";
                                    ds.Tables.Add(table1);
                                    table1.PrimaryKey = new DataColumn[] { table1.Columns["IdAuto"] };
                                    table2.TableName = "Eval_Drepturi";
                                    table2.PrimaryKey = new DataColumn[] { table2.Columns["IdAuto"] };
                                    ds.Tables.Add(table2);

                                    Session["InformatiaCurentaEvalQuiz"] = ds;
                                    if (Page.IsCallback)
                                        ASPxWebControl.RedirectOnCallback(url);
                                    else
                                        Response.Redirect(url, false);

                                }
                            }
                            break;
                        case "btnSterge":
                            StergeChestionar(arr[1]);
                            btnFiltru_Click(sender, e);
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if(e.DataColumn.FieldName=="Stare")
                {
                    object row = grDate.GetRowValues(e.VisibleIndex, "Culoare");
                    string culoare = row.ToString();

                    e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(culoare);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie!");
            }
        }
        #endregion

        #region Methods 
        private void IncarcaGrid()
        {
            try
            {
                DateTime dtStart = dtInceput.Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dtInceput.Value);
                DateTime dtEnd = dtSfarsit.Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dtSfarsit.Value);
                DataTable dt = Evaluare.GetEval_Quiz(Convert.ToInt32(cmbPerioada.Value ?? -99), dtStart, dtEnd);
                
                grDate.DataSource = dt;
                grDate.KeyFieldName = "Id";
                Session["InformatiaCurentaQuiz"] = dt;
                grDate.DataBind();
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void StergeChestionar(string id)
        {
            try
            {
                General.ExecutaNonQuery("DELETE FROM \"Eval_Circuit\" where \"IdQuiz\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_Drepturi\" where \"IdQuiz\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_QuizIntrebari\" where \"IdQuiz\" = " + id, null);
                General.ExecutaNonQuery("DELETE FROM \"Eval_Quiz\" where \"Id\" = " + id, null);

                DateTime dtStart = dtInceput.Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dtInceput.Value);
                DateTime dtEnd = dtSfarsit.Value == null ? new DateTime(1900, 1, 1) : Convert.ToDateTime(dtSfarsit.Value);
                
                Session["InformatiaCurentaQuiz"] = Evaluare.GetEval_Quiz(Convert.ToInt32(cmbPerioada.Value ?? -99), dtStart, dtEnd);
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //int IdQuiz = Dami.NextId("Eval_Quiz");
                Session["IdEvalQuiz"] = null;
                Session["InformatiaCurentaEvalQuiz"] = null;
                Session["Quiz360"] = 0;

                //Florin 2020.01.30
                Session["Eval_QuizSetAngajati"] = null;

                /*
                DataTable table = General.IncarcaDT(@"select * from ""Eval_Quiz"" where ""Id"" = " + IdQuiz, null);
                DataTable table0 = General.IncarcaDT(@"select * from ""Eval_QuizIntrebari"" where ""Id"" = " + IdQuiz, null);
                DataTable table1 = General.IncarcaDT(@"select * from ""Eval_Circuit"" where ""IdQuiz"" = " + IdQuiz, null);
                DataTable table2 = General.IncarcaDT(@"select * from ""Eval_Drepturi"" where ""IdQuiz"" = " + IdQuiz, null);

                DataSet ds = new DataSet();
                table.TableName = "Eval_Quiz";
                ds.Tables.Add(table);
                table0.TableName = "Eval_QuizIntrebari";
                ds.Tables.Add(table0);
                table1.TableName = "Eval_Circuit";
                ds.Tables.Add(table1);
                table2.TableName = "Eval_Drepturi";
                ds.Tables.Add(table2);

                Session["InformatiaCurentaEvalQuiz"] = ds;
                */
                string url = "";
                url = "~/Eval/QuizDetaliu.aspx";
                Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        protected void btnNew360_Click(object sender, EventArgs e)
        {
            try
            {
                Session["IdEvalQuiz"] = null;
                Session["InformatiaCurentaEvalQuiz"] = null;
                Session["Quiz360"] = 1;

                string url = "";
                url = "~/Eval/QuizDetaliu.aspx";
                Response.Redirect(url, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        protected void btnDuplicare_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id" }) ?? -99);
                if (id != -99)
                {
                    QuizDuplica(id, Convert.ToInt32(Session["UserId"].ToString()));
                    IncarcaGrid();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void QuizDuplica(int id, int idUser)
        {
            try
            {
                DataTable dtOri = General.IncarcaDT("SELECT * FROM \"Eval_Quiz\" WHERE \"Id\" = " + id, null);
                DataTable dtOriLinii = General.IncarcaDT("SELECT * FROM \"Eval_QuizIntrebari\" WHERE \"IdQuiz\" = " + id, null);
                DataTable dtOriCircuit = General.IncarcaDT("SELECT * FROM \"Eval_Circuit\" WHERE \"IdQuiz\" = " + id, null);
                DataTable dtOriDrepturi = General.IncarcaDT("SELECT * FROM \"Eval_Drepturi\" WHERE \"IdQuiz\" = " + id, null);
                DataTable dtOriGrup = General.IncarcaDT("SELECT * FROM \"Eval_relGrupAngajatQuiz\" WHERE \"IdQuiz\" = " + id, null);

                int idUrm = 2;
                idUrm = Dami.NextId("Eval_Quiz");

                string sqlQuiz = "";
                if (dtOri != null && dtOri.Rows.Count > 0)
                {
                    sqlQuiz = "INSERT INTO \"Eval_Quiz\" (";
                    for (int i = 0; i < dtOri.Columns.Count; i++)
                    {
                        sqlQuiz += "\"" + dtOri.Columns[i].ColumnName + "\"";
                        if (i < dtOri.Columns.Count - 1)
                            sqlQuiz += ", ";
                    }
                    sqlQuiz += ") VALUES (";
                    for (int i = 0; i < dtOri.Columns.Count; i++)
                    {
                        switch (dtOri.Columns[i].ColumnName)
                        {
                            case "Id":
                                sqlQuiz += idUrm;
                                break;
                            case "Denumire":
                                sqlQuiz += "'" + dtOri.Rows[0]["Denumire"].ToString() + " - Copie'";
                                break;
                            case "USER_NO":
                                sqlQuiz += idUser;
                                break;
                            case "TIME":
                                sqlQuiz += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                break;
                            default:
                                if (dtOri.Rows[0][dtOri.Columns[i].ColumnName] == null || dtOri.Rows[0][dtOri.Columns[i].ColumnName].ToString().Length <= 0)
                                    sqlQuiz += "NULL";
                                else
                                {
                                    switch (dtOri.Columns[i].DataType.ToString())
                                    {
                                        case "System.String":
                                            sqlQuiz += "'" + dtOri.Rows[0][dtOri.Columns[i].ColumnName].ToString() + "'";
                                            break;
                                        case "System.DateTime":
                                            DateTime dt = Convert.ToDateTime(General.Nz(dtOri.Rows[0][dtOri.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                            sqlQuiz += General.ToDataUniv(dt);
                                            break;
                                        default:
                                            sqlQuiz += dtOri.Rows[0][dtOri.Columns[i].ColumnName].ToString();
                                            break;
                                    }
                                }
                                break;
                        }
                        if (i < dtOri.Columns.Count - 1)
                            sqlQuiz += ", ";
                    }

                    sqlQuiz += ")";
                }

                General.IncarcaDT(sqlQuiz, null);



                //Florin 2019.10.02
                #region OLD QuizIntrebari

                //int idRoot = 0, idSec = 0, idParinte = 0;
                //Hashtable rel = new Hashtable();

                ////Florin 2019.01.07
                //int idUrmLinii = 10000;
                ////idUrmLinii = Dami.NextId("Eval_QuizIntrebari", dtOriLinii.Columns.Count);
                ////idUrmLinii = idUrmLinii - dtOriLinii.Columns.Count + 1;
                //idUrmLinii = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(""Id"") FROM ""Eval_QuizIntrebari"" ", null), 0)) + 1;



                //string sqlQuizLinii = "";
                //if (dtOriLinii != null && dtOriLinii.Rows.Count > 0)
                //{
                //    for (int j = 0; j < dtOriLinii.Rows.Count; j++)
                //    {
                //        sqlQuizLinii = "INSERT INTO \"Eval_QuizIntrebari\" (";
                //        for (int i = 0; i < dtOriLinii.Columns.Count; i++)
                //        {
                //            sqlQuizLinii += "\"" + dtOriLinii.Columns[i].ColumnName + "\"";
                //            if (i < dtOriLinii.Columns.Count - 1)
                //                sqlQuizLinii += ", ";
                //        }
                //        sqlQuizLinii += ") VALUES (";
                //        for (int i = 0; i < dtOriLinii.Columns.Count; i++)
                //        {
                //            switch (dtOriLinii.Columns[i].ColumnName)
                //            {
                //                case "Ordine":
                //                    string[] param = dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName].ToString().Split('-');
                //                    if (param.Length == 3)
                //                    {
                //                        if (rel[param[1]] != null)
                //                            idRoot = Convert.ToInt32(rel[param[1]]);
                //                        else
                //                        {
                //                            idRoot = idUrmLinii;
                //                            rel[param[1]] = idRoot;
                //                        }
                //                    }
                //                    if (param.Length == 4)
                //                    {
                //                        if (rel[param[2]] != null)
                //                            idSec = Convert.ToInt32(rel[param[2]]);
                //                        else
                //                        {
                //                            idSec = idUrmLinii;
                //                            rel[param[2]] = idSec;
                //                        }
                //                        idParinte = idRoot;
                //                    }
                //                    if (param.Length == 5)
                //                    {
                //                        if (rel[param[2]] != null)
                //                            idSec = Convert.ToInt32(rel[param[2]]);
                //                        else
                //                        {
                //                            idSec = idUrmLinii;
                //                            rel[param[2]] = idSec;
                //                        }
                //                        idParinte = idSec;
                //                    }
                //                    sqlQuizLinii += "'";
                //                    for (int k = 1; k < param.Length - 1; k++)
                //                    {
                //                        if (k == 1)
                //                            sqlQuizLinii += "-" + idRoot;
                //                        if (k == 2)
                //                            sqlQuizLinii += "-" + idSec;
                //                        if (k == 3)
                //                            sqlQuizLinii += "-" + idUrmLinii;
                //                        if (k == param.Length - 2)
                //                            sqlQuizLinii += "-";
                //                    }
                //                    sqlQuizLinii += "'";
                //                    break;
                //                case "Parinte":
                //                    sqlQuizLinii += idParinte;
                //                    break;
                //                case "Id":
                //                    sqlQuizLinii += idUrmLinii;
                //                    break;
                //                case "IdQuiz":
                //                    sqlQuizLinii += idUrm;
                //                    break;
                //                case "TemplateIdObiectiv":
                //                case "TemplateIdCompetenta":
                //                    sqlQuizLinii += "NULL";
                //                    break;
                //                case "USER_NO":
                //                    sqlQuizLinii += idUser;
                //                    break;
                //                case "TIME":
                //                    sqlQuizLinii += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                //                    break;
                //                default:
                //                    if (dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName] == null || dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName].ToString().Length <= 0)
                //                        sqlQuizLinii += "NULL";
                //                    else
                //                    {
                //                        switch (dtOriLinii.Columns[i].DataType.ToString())
                //                        {
                //                            case "System.String":
                //                                sqlQuizLinii += "'" + dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName].ToString() + "'";
                //                                break;
                //                            case "System.DateTime":
                //                                DateTime dt = Convert.ToDateTime(General.Nz(dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                //                                sqlQuizLinii += General.ToDataUniv(dt);
                //                                break;
                //                            default:
                //                                sqlQuizLinii += dtOriLinii.Rows[j][dtOriLinii.Columns[i].ColumnName].ToString();
                //                                break;
                //                        }
                //                    }
                //                    break;
                //            }
                //            if (i < dtOriLinii.Columns.Count - 1)
                //                sqlQuizLinii += ", ";
                //        }
                //        sqlQuizLinii += ")";
                //        General.IncarcaDT(sqlQuizLinii, null);
                //        idUrmLinii++;

                //    }
                //}

                #endregion

                int idUrmLinii = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(""Id"") FROM ""Eval_QuizIntrebari"" ", null), 0)) + 1;
                General.ExecutaNonQuery($@"
                INSERT INTO ""Eval_QuizIntrebari""(""Id"",                ""Descriere"", ""TipValoare"", ""Ordine"", ""IdIntrebare"", ""TipData"", ""IdQuiz"", ""Orientare"", ""Obligatoriu"", ""Parinte"",                ""EsteSectiune"", ""DescriereInRatingGlobal"", ""TemplateIdObiectiv"", ""TemplateIdCompetenta"", ""OrdineInt"", ""PreluareObiective"", ""IdPeriod"", ""PreluareCompetente"", ""IdPeriodComp"", ""TIME"", ""USER_NO"")
                                            SELECT ""Id"" + {idUrmLinii}, ""Descriere"", ""TipValoare"", ""Ordine"", ""IdIntrebare"", ""TipData"", {idUrm},    ""Orientare"", ""Obligatoriu"", CASE WHEN ""Descriere"" ='Root' THEN 0 ELSE ""Parinte"" + {idUrmLinii} END AS ""Parinte"", ""EsteSectiune"", ""DescriereInRatingGlobal"", ""TemplateIdObiectiv"", ""TemplateIdCompetenta"", ""OrdineInt"", ""PreluareObiective"", ""IdPeriod"", ""PreluareCompetente"", ""IdPeriodComp"", {General.CurrentDate()}, {Session["UserId"]} FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = {id}", null);

                //End Florin 2019.10.02

                //Florin 2020.05.04
                string sqlTemp = "";
                DataTable dtTemp = General.IncarcaDT($@"SELECT * FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz""={idUrm} AND (COALESCE(""TemplateIdObiectiv"",0)>0 OR COALESCE(""TemplateIdCompetenta"",0)>0)");
                for(int i=0; i<dtTemp.Rows.Count; i++)
                {
                    if (Convert.ToInt32(General.Nz(dtTemp.Rows[i]["TemplateIdObiectiv"],"-99")) > 0)
                        sqlTemp += $@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdObiectiv""=(SELECT MIN(""TemplateId"") FROM ""Eval_ConfigObTemplate"" WHERE ""TemplateId"" NOT IN (SELECT DISTINCT ""TemplateIdObiectiv"" FROM ""Eval_QuizIntrebari"" WHERE ""TemplateIdObiectiv"" IS NOT NULL)) WHERE ""Id""={dtTemp.Rows[i]["Id"]};" + Environment.NewLine;
                    if (Convert.ToInt32(General.Nz(dtTemp.Rows[i]["TemplateIdCompetenta"], "-99")) > 0)
                        sqlTemp += $@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdCompetenta""=(SELECT MIN(""TemplateId"") FROM ""Eval_ConfigCompTemplate"" WHERE ""TemplateId"" NOT IN (SELECT DISTINCT ""TemplateIdCompetenta"" FROM ""Eval_QuizIntrebari"" WHERE ""TemplateIdCompetenta"" IS NOT NULL)) WHERE ""Id""={dtTemp.Rows[i]["Id"]};" + Environment.NewLine;
                }
                if (sqlTemp != "")
                    General.ExecutaNonQuery("BEGIN" + Environment.NewLine +
                        sqlTemp + Environment.NewLine +
                        "END;");


                int idUrmCirc = 2;
                idUrmCirc = Dami.NextId("Eval_Circuit");

                string sqlCircuit = "";
                if (dtOriCircuit != null && dtOriCircuit.Rows.Count > 0)
                {
                    sqlCircuit = "INSERT INTO \"Eval_Circuit\" (";
                    for (int i = 0; i < dtOriCircuit.Columns.Count; i++)
                    {
                        if (dtOriCircuit.Columns[i].ColumnName != "IdAuto")
                        {
                            sqlCircuit += "\"" + dtOriCircuit.Columns[i].ColumnName + "\"";
                            if (i < dtOriCircuit.Columns.Count - 1)
                                sqlCircuit += ", ";
                        }
                        else
                            if (sqlCircuit.Length > 2 && i == dtOriCircuit.Columns.Count - 1)
                                sqlCircuit = sqlCircuit.Substring(0, sqlCircuit.Length - 2);
                    }
                    sqlCircuit += ") VALUES (";
                    for (int i = 0; i < dtOriCircuit.Columns.Count; i++)
                    {
                        if (dtOriCircuit.Columns[i].ColumnName != "IdAuto")
                        {
                            switch (dtOriCircuit.Columns[i].ColumnName)
                            {
                                case "IdQuiz":
                                    sqlCircuit += idUrm;
                                    break;
                                case "USER_NO":
                                    sqlCircuit += idUser;
                                    break;
                                case "TIME":
                                    sqlCircuit += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                    break;
                                default:
                                    if (dtOriCircuit.Rows[0][dtOriCircuit.Columns[i].ColumnName] == null || dtOriCircuit.Rows[0][dtOriCircuit.Columns[i].ColumnName].ToString().Length <= 0)
                                        sqlCircuit += "NULL";
                                    else
                                    {
                                        switch (dtOriCircuit.Columns[i].DataType.ToString())
                                        {
                                            case "System.String":
                                                sqlCircuit += "'" + dtOriCircuit.Rows[0][dtOriCircuit.Columns[i].ColumnName].ToString() + "'";
                                                break;
                                            case "System.DateTime":
                                                DateTime dt = Convert.ToDateTime(General.Nz(dtOriCircuit.Rows[0][dtOriCircuit.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                                sqlCircuit += General.ToDataUniv(dt);
                                                break;
                                            default:
                                                sqlCircuit += dtOriCircuit.Rows[0][dtOriCircuit.Columns[i].ColumnName].ToString();
                                                break;
                                        }
                                    }
                                    break;
                            }
                            if (i < dtOriCircuit.Columns.Count - 1)
                                sqlCircuit += ", ";
                        }
                        else
                            if (sqlCircuit.Length > 2 && i == dtOriCircuit.Columns.Count - 1)
                                sqlCircuit = sqlCircuit.Substring(0, sqlCircuit.Length - 2);
                    }

                    sqlCircuit += ")";
                }
                General.IncarcaDT(sqlCircuit, null);

                int idUrmDrepturi = 10000;
                idUrmDrepturi = Dami.NextId("Eval_Drepturi", dtOriDrepturi.Columns.Count);
                idUrmDrepturi = idUrmDrepturi - dtOriDrepturi.Columns.Count + 1;

                string sqlDrepturi = "";
                if (dtOriDrepturi != null && dtOriDrepturi.Rows.Count > 0)
                {
                    for (int j = 0; j < dtOriDrepturi.Rows.Count; j++)
                    {
                        sqlDrepturi = "INSERT INTO \"Eval_Drepturi\" (";
                        for (int i = 0; i < dtOriDrepturi.Columns.Count; i++)
                        {
                            if (dtOriDrepturi.Columns[i].ColumnName != "IdAuto")
                            {
                                sqlDrepturi += "\"" + dtOriDrepturi.Columns[i].ColumnName + "\"";
                                if (i < dtOriDrepturi.Columns.Count - 1)
                                    sqlDrepturi += ", ";
                            }
                            else
                                if (sqlDrepturi.Length > 2 && i == dtOriDrepturi.Columns.Count - 1)
                                sqlDrepturi = sqlDrepturi.Substring(0, sqlDrepturi.Length - 2);
                        }
                        sqlDrepturi += ") VALUES (";
                        for (int i = 0; i < dtOriDrepturi.Columns.Count; i++)
                        {
                            if (dtOriDrepturi.Columns[i].ColumnName != "IdAuto")
                            {
                                switch (dtOriDrepturi.Columns[i].ColumnName)
                                {
                                    case "IdQuiz":
                                        sqlDrepturi += idUrm;
                                        break;
                                    case "USER_NO":
                                        sqlDrepturi += idUser;
                                        break;
                                    case "TIME":
                                        sqlDrepturi += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                        break;
                                    default:
                                        if (dtOriDrepturi.Rows[j][dtOriDrepturi.Columns[i].ColumnName] == null || dtOriDrepturi.Rows[j][dtOriDrepturi.Columns[i].ColumnName].ToString().Length <= 0)
                                            sqlDrepturi += "NULL";
                                        else
                                        {
                                            switch (dtOriDrepturi.Columns[i].DataType.ToString())
                                            {
                                                case "System.String":
                                                    sqlDrepturi += "'" + dtOriDrepturi.Rows[j][dtOriDrepturi.Columns[i].ColumnName].ToString() + "'";
                                                    break;
                                                case "System.DateTime":
                                                    DateTime dt = Convert.ToDateTime(General.Nz(dtOriDrepturi.Rows[j][dtOriDrepturi.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                                    sqlDrepturi += General.ToDataUniv(dt);
                                                    break;
                                                default:
                                                    sqlDrepturi += dtOriDrepturi.Rows[j][dtOriDrepturi.Columns[i].ColumnName].ToString();
                                                    break;
                                            }
                                        }
                                        break;
                                }
                                if (i < dtOriDrepturi.Columns.Count - 1)
                                    sqlDrepturi += ", ";
                            }
                            else
                                if (sqlDrepturi.Length > 2 && i == dtOriDrepturi.Columns.Count - 1)
                                sqlDrepturi = sqlDrepturi.Substring(0, sqlDrepturi.Length - 2);
                        }
                        sqlDrepturi += ")";
                        General.IncarcaDT(sqlDrepturi, null);
                        idUrmDrepturi++;

                    }
                }


                //int idAuto = 10000000;
                int idUrmGrup = 10000;
                idUrmGrup = Dami.NextId("Eval_relGrupAngajatQuiz", dtOriGrup.Columns.Count);
                idUrmGrup = idUrmGrup - dtOriGrup.Columns.Count + 1;

                string sqlGrup = "";
                if (dtOriGrup != null && dtOriGrup.Rows.Count > 0)
                {
                    for (int j = 0; j < dtOriGrup.Rows.Count; j++)
                    {
                        sqlGrup = "INSERT INTO \"Eval_relGrupAngajatQuiz\" (";
                        for (int i = 0; i < dtOriGrup.Columns.Count; i++)
                        {
                            if (dtOriGrup.Columns[i].ColumnName != "IdAuto")
                            {
                                sqlGrup += "\"" + dtOriGrup.Columns[i].ColumnName + "\"";
                                if (i < dtOriGrup.Columns.Count - 1)
                                    sqlGrup += ", ";
                            }
                            else
                                if (sqlGrup.Length > 2 && i == dtOriGrup.Columns.Count - 1)
                                sqlGrup = sqlGrup.Substring(0, sqlGrup.Length - 2);
                        }
                        sqlGrup += ") VALUES (";
                        for (int i = 0; i < dtOriGrup.Columns.Count; i++)
                        {
                            if (dtOriGrup.Columns[i].ColumnName != "IdAuto")
                            {
                                switch (dtOriGrup.Columns[i].ColumnName)
                                {
                                    case "IdQuiz":
                                        sqlGrup += idUrm;
                                        break;
                                    case "USER_NO":
                                        sqlGrup += idUser;
                                        break;
                                    case "TIME":
                                        sqlGrup += (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                                        break;
                                    default:
                                        if (dtOriGrup.Rows[j][dtOriGrup.Columns[i].ColumnName] == null || dtOriGrup.Rows[j][dtOriGrup.Columns[i].ColumnName].ToString().Length <= 0)
                                            sqlGrup += "NULL";
                                        else
                                        {
                                            switch (dtOriGrup.Columns[i].DataType.ToString())
                                            {
                                                case "System.String":
                                                    sqlGrup += "'" + dtOriGrup.Rows[j][dtOriGrup.Columns[i].ColumnName].ToString() + "'";
                                                    break;
                                                case "System.DateTime":
                                                    DateTime dt = Convert.ToDateTime(General.Nz(dtOriGrup.Rows[j][dtOriGrup.Columns[i].ColumnName], new DateTime(2100, 1, 1)));
                                                    sqlGrup += General.ToDataUniv(dt);
                                                    break;
                                                default:
                                                    sqlGrup += dtOriGrup.Rows[j][dtOriGrup.Columns[i].ColumnName].ToString();
                                                    break;
                                            }
                                        }
                                        break;
                                }
                                if (i < dtOriGrup.Columns.Count - 1)
                                    sqlGrup += ", ";
                            }
                            else
                                if (sqlGrup.Length > 2 && i == dtOriGrup.Columns.Count - 1)
                                sqlGrup = sqlGrup.Substring(0, sqlGrup.Length - 2);
                        }
                        sqlGrup += ")";
                        General.IncarcaDT(sqlGrup, null);
                        idUrmGrup++;

                    }
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