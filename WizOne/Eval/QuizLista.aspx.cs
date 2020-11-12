using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class QuizLista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnEdit.Image.ToolTip = Dami.TraduCuvant("btnEdit", "Modifica");
                foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                #endregion

                txtTitlu.Text = (Session["Titlu"] ?? "").ToString();
                if(!IsPostBack)
                {
                    cmbPerioada.DataSource = General.IncarcaDT(@"SELECT ""IdPerioada"" AS ""Id"", ""DenPerioada"" AS ""Denumire"" FROM ""Eval_Perioada"" ", null);
                    cmbPerioada.DataBind();

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

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "Id" }) ?? -99);

                if (e.Parameters != "btnFiltru" && id == -99)
                    return;

                switch (e.Parameters)
                {
                    case "btnEdit":
                        {
                            //Florin 2020.01.30
                            Session["Eval_QuizSetAngajati"] = null;

                            string url = "~/Eval/QuizDetaliu.aspx";
                            if(url!="")
                            {
                                Session["IdEvalQuiz"] = id;
                                DataTable table = General.IncarcaDT(@"SELECT * FROM ""Eval_Quiz"" WHERE ""Id"" = " + id, null);
                                DataTable table0 = General.IncarcaDT(@"SELECT * FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = " + id, null);
                                DataTable table1 = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = " + id, null);
                                DataTable table2 = General.IncarcaDT(@"SELECT * FROM ""Eval_Drepturi"" WHERE ""IdQuiz"" = " + id, null);

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
                        General.ExecutaNonQuery($@"DELETE FROM ""Eval_relGrupAngajatQuiz"" WHERE ""IdQuiz"" = " + id, null);
                        General.ExecutaNonQuery($@"DELETE FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = " + id, null);
                        General.ExecutaNonQuery($@"DELETE FROM ""Eval_Drepturi"" WHERE ""IdQuiz"" = " + id, null);
                        General.ExecutaNonQuery($@"DELETE FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = " + id, null);
                        General.ExecutaNonQuery($@"DELETE FROM ""Eval_Quiz"" WHERE ""Id"" = " + id, null);

                        IncarcaGrid();
                        break;
                    case "btnDuplicare":
                        QuizDuplica(id, Convert.ToInt32(Session["UserId"].ToString()));
                        IncarcaGrid();
                        break;
                    case "btnFiltru":
                        IncarcaGrid();
                        break;
                }
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string filtru = "";
                if (txtDtSf.Value != null) filtru += @" AND A.""DataInceput"" <= " + General.ToDataUniv(txtDtSf.Date);
                if (txtDtInc.Value != null) filtru += " AND " + General.ToDataUniv(txtDtInc.Date) + @" <= A.""DataSfarsit"" ";
                if (cmbPerioada.Value != null) filtru += @" AND A.""Anul""= " + General.Nz(cmbPerioada.Value, -99);

                string strSql = $@"SELECT A.""Id"", A.""Denumire"", A.""Titlu"", A.""DataInceput"", A.""DataSfarsit"", B.""DenPerioada"" AS ""Perioada""
                    FROM ""Eval_Quiz"" A
                    LEFT JOIN ""Eval_Perioada"" B ON A.""Anul"" = B.""IdPerioada""
                    WHERE 1 = 1 {filtru}";
                
                grDate.DataSource = General.IncarcaDT(strSql);
                grDate.KeyFieldName = "Id";
                grDate.DataBind();
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Session["IdEvalQuiz"] = null;
                Session["InformatiaCurentaEvalQuiz"] = null;
                Session["Quiz360"] = 0;
                Session["Eval_QuizSetAngajati"] = null;

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
                idUrm = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Id"",0)) FROM ""Eval_Quiz"" "), 0)) + 1;

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

                int idUrmLinii = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(""Id"") FROM ""Eval_QuizIntrebari"" ", null), 0)) + 1;
                General.ExecutaNonQuery($@"
                INSERT INTO ""Eval_QuizIntrebari""(""Id"",                ""Descriere"", ""TipValoare"", ""Ordine"", ""IdIntrebare"", ""TipData"", ""IdQuiz"", ""Orientare"", ""Obligatoriu"", ""Parinte"",                ""EsteSectiune"", ""DescriereInRatingGlobal"", ""TemplateIdObiectiv"", ""TemplateIdCompetenta"", ""OrdineInt"", ""PreluareObiective"", ""IdPeriod"", ""PreluareCompetente"", ""IdPeriodComp"", ""TIME"", ""USER_NO"", ""OrdineAfisare"")
                                            SELECT ""Id"" + {idUrmLinii}, ""Descriere"", ""TipValoare"", ""Ordine"", ""IdIntrebare"", ""TipData"", {idUrm},    ""Orientare"", ""Obligatoriu"", CASE WHEN ""Descriere"" ='Root' THEN 0 ELSE ""Parinte"" + {idUrmLinii} END AS ""Parinte"", ""EsteSectiune"", ""DescriereInRatingGlobal"", ""TemplateIdObiectiv"", ""TemplateIdCompetenta"", ""OrdineInt"", ""PreluareObiective"", ""IdPeriod"", ""PreluareCompetente"", ""IdPeriodComp"", {General.CurrentDate()}, {Session["UserId"]}, ""OrdineAfisare"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = {id}", null);

                //End Florin 2019.10.02

                //Florin 2020.05.04
                //Radu 27.08.2020 - se doreste ca aceste sabloane sa fie copiate si reintroduse, nu sa se ia alte sabloane existente
                string sqlTemp = "";
                DataTable dtTemp = General.IncarcaDT($@"SELECT * FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz""={idUrm} AND (COALESCE(""TemplateIdObiectiv"",0)>0 OR COALESCE(""TemplateIdCompetenta"",0)>0)");
                for(int i=0; i<dtTemp.Rows.Count; i++)
                {
                    if (Convert.ToInt32(General.Nz(dtTemp.Rows[i]["TemplateIdObiectiv"], "-99")) > 0)
                    {
                        //sqlTemp += $@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdObiectiv""=(SELECT MIN(""TemplateId"") FROM ""Eval_ConfigObTemplate"" WHERE ""TemplateId"" NOT IN (SELECT DISTINCT ""TemplateIdObiectiv"" FROM ""Eval_QuizIntrebari"" WHERE ""TemplateIdObiectiv"" IS NOT NULL)) WHERE ""Id""={dtTemp.Rows[i]["Id"]};" + Environment.NewLine;
                        string camp = "'Template ' +  CONVERT(VARCHAR, (SELECT max(TemplateId) + 1 FROM  Eval_ConfigObTemplate))";
                        if (Constante.tipBD == 2) camp = "'Template ' || (SELECT max(\"TemplateId\") + 1 FROM  \"Eval_ConfigObTemplate\")";
                        General.ExecutaNonQuery("INSERT INTO \"Eval_ConfigObTemplate\" (\"TemplateId\", \"TemplateName\", \"NrMinObiective\", \"NrMaxObiective\", \"PoateAdauga\", \"PoateSterge\", \"PoateModifica\") " +
                            " SELECT (SELECT max(\"TemplateId\") + 1 FROM  \"Eval_ConfigObTemplate\"), " + camp + ", \"NrMinObiective\", \"NrMaxObiective\", \"PoateAdauga\", \"PoateSterge\", \"PoateModifica\" " +
                            " FROM \"Eval_ConfigObTemplate\" WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdObiectiv"].ToString(), null);

                        DataTable dtOb = General.IncarcaDT("SELECT * FROM \"Eval_ConfigObTemplateDetail\" WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdObiectiv"].ToString(), null);
                        for (int j = 0; j < dtOb.Rows.Count; j++)
                            General.ExecutaNonQuery("INSERT INTO \"Eval_ConfigObTemplateDetail\" (\"Id\", \"TemplateId\", \"ColumnName\", \"Width\", \"Obligatoriu\", \"Citire\", \"Editare\", \"Vizibil\", \"IdNomenclator\") " +
                                " SELECT " + (j + 1).ToString() + ", (SELECT max(\"TemplateId\") FROM  \"Eval_ConfigObTemplate\"), max(\"ColumnName\"), max(\"Width\"), max(\"Obligatoriu\"), max(\"Citire\"), max(\"Editare\"), max(\"Vizibil\"), max(\"IdNomenclator\") FROM \"Eval_ConfigObTemplateDetail\" " +
                                " WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdObiectiv"].ToString() + " AND \"ColumnName\" = '" + dtOb.Rows[j]["ColumnName"].ToString() + "'", null);

                        General.ExecutaNonQuery($@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdObiectiv""=(SELECT MAX(""TemplateId"") FROM ""Eval_ConfigObTemplate"") WHERE ""Id""={dtTemp.Rows[i]["Id"]}", null);
                    }
                    if (Convert.ToInt32(General.Nz(dtTemp.Rows[i]["TemplateIdCompetenta"], "-99")) > 0)
                    {
                        //sqlTemp += $@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdCompetenta""=(SELECT MIN(""TemplateId"") FROM ""Eval_ConfigCompTemplate"" WHERE ""TemplateId"" NOT IN (SELECT DISTINCT ""TemplateIdCompetenta"" FROM ""Eval_QuizIntrebari"" WHERE ""TemplateIdCompetenta"" IS NOT NULL)) WHERE ""Id""={dtTemp.Rows[i]["Id"]};" + Environment.NewLine;
                        string camp = "'Template ' +  CONVERT(VARCHAR, (SELECT max(TemplateId) + 1 FROM  Eval_ConfigCompTemplate))";
                        if (Constante.tipBD == 2) camp = "'Template ' || (SELECT max(\"TemplateId\") + 1 FROM  \"Eval_ConfigCompTemplate\")";
                        General.ExecutaNonQuery("INSERT INTO \"Eval_ConfigCompTemplate\" (\"TemplateId\", \"TemplateName\") " +
                            " SELECT (SELECT max(\"TemplateId\") + 1 FROM  \"Eval_ConfigCompTemplate\"), " + camp + " FROM \"Eval_ConfigCompTemplate\" WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdCompetenta"].ToString(), null);

                        DataTable dtComp = General.IncarcaDT("SELECT * FROM \"Eval_ConfigCompTemplateDetail\" WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdCompetenta"].ToString(), null);
                        for (int j = 0; j < dtComp.Rows.Count; j++)
                            General.ExecutaNonQuery("INSERT INTO \"Eval_ConfigCompTemplateDetail\" (\"Id\", \"TemplateId\", \"ColumnName\", \"Width\", \"Obligatoriu\", \"Citire\", \"Editare\", \"Vizibil\", \"IdNomenclator\") " +
                                " SELECT " + (j + 1).ToString() + ", (SELECT max(\"TemplateId\") FROM  \"Eval_ConfigCompTemplate\"), max(\"ColumnName\"), max(\"Width\"), max(\"Obligatoriu\"), max(\"Citire\"), max(\"Editare\"), max(\"Vizibil\"), max(\"IdNomenclator\") FROM \"Eval_ConfigCompTemplateDetail\" " +
                                " WHERE \"TemplateId\" = " + dtTemp.Rows[i]["TemplateIdCompetenta"].ToString() + " AND \"ColumnName\" = '" + dtComp.Rows[j]["ColumnName"].ToString() + "'", null);
                        General.ExecutaNonQuery($@"UPDATE ""Eval_QuizIntrebari"" SET ""TemplateIdCompetenta""=(SELECT MAX(""TemplateId"") FROM ""Eval_ConfigCompTemplate"") WHERE ""Id""={dtTemp.Rows[i]["Id"]}", null);
                    }
                }
                if (sqlTemp != "")
                    General.ExecutaNonQuery("BEGIN" + Environment.NewLine +
                        sqlTemp + Environment.NewLine +
                        "END;");

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
                    }
                }


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