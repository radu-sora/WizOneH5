using DevExpress.Web;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    class metaOrdine
    {
        public string Ordine { get; set; }
        public int Id { get; set; }

        public metaOrdine() { }

        public metaOrdine(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;

            Ordine = columns.Contains("Ordine") == true ? dr["Ordine"].ToString() : "";
            Id = columns.Contains("Id") == true ? Convert.ToInt32(dr["Id"].ToString() == string.Empty ? "-99" : dr["Id"].ToString()) : -99;
        }
    }

    public partial class QuizDetaliu : System.Web.UI.Page
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                #endregion

                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                if (ds==null && General.Nz(Session["IdEvalQuiz"],"").ToString() == "")
                {
                    Session["IdEvalQuiz"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Id"",0)) FROM ""Eval_Quiz"" "), 0)) + 1;
                    Session["esteQuizNou"] = "true";
                    Initializare(ds);
                }

                DataTable dt = General.IncarcaDT("select \"Denumire\", \"Pagina\" from \"Eval_QuizTaburi\" order by \"Ordine\" ", null);

                int nrTaburi = dt.Rows.Count;
                bool multirand = false;
                if (nrTaburi > 10)
                    multirand = true;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Name = dt.Rows[i]["Pagina"].ToString();
                    tabPage.Text = dt.Rows[i]["Denumire"].ToString();

                    if (multirand && i >= nrTaburi / 2)
                    {
                        tabPage.NewLine = true;
                        multirand = false;
                    }

                    Control ctrl = new Control();
                    if (File.Exists(HostingEnvironment.MapPath("~/Eval/" + dt.Rows[i]["Pagina"].ToString() + ".ascx")))
                    {
                        ctrl = this.LoadControl(dt.Rows[i]["Pagina"].ToString() + ".ascx");                        
                        tabPage.Controls.Add(ctrl);
                    }                  

                    this.ASPxPageControl2.TabPages.Add(tabPage);                  
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommandBuilder cb = new SqlCommandBuilder();

                OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                OracleCommandBuilder cbOle = new OracleCommandBuilder();

                //Florin 2018.02.22
                {
                    if (ds != null && ds.ToString() != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        Control ctl = ASPxPageControl2.TabPages[0].Controls[0];
                        if (ctl != null)
                        {
                            ASPxHiddenField lst = ctl.FindControl("lstValori") as ASPxHiddenField;
                            if (lst != null && lst.Count > 0)
                            {
                                if (lst.Contains("Denumire")) ds.Tables[0].Rows[0]["Denumire"] = lst["Denumire"];
                                if (lst.Contains("Activ")) ds.Tables[0].Rows[0]["Activ"] = (lst["Activ"].ToString().ToLower() == "true" ? 1 : 0);
                                if (lst.Contains("DataInceput")) ds.Tables[0].Rows[0]["DataInceput"] = Convert.ToDateTime(lst["DataInceput"]);
                                if (lst.Contains("DataSfarsit")) ds.Tables[0].Rows[0]["DataSfarsit"] = Convert.ToDateTime(lst["DataSfarsit"]);
                                if (lst.Contains("Anul")) ds.Tables[0].Rows[0]["Anul"] = lst["Anul"];
                                if (lst.Contains("Preluare")) ds.Tables[0].Rows[0]["Preluare"] = (lst["Preluare"].ToString().ToLower() == "true" ? 1 : 0);
                                if (lst.Contains("Titlu")) ds.Tables[0].Rows[0]["Titlu"] = lst["Titlu"];
                                if (lst.Contains("CategorieQuiz")) ds.Tables[0].Rows[0]["CategorieQuiz"] = lst["CategorieQuiz"];
                                if (lst.Contains("LuatLaCunostinta")) ds.Tables[0].Rows[0]["LuatLaCunostinta"] = lst["LuatLaCunostinta"];
                                if (lst.Contains("NrZileLuatLaCunostinta")) ds.Tables[0].Rows[0]["NrZileLuatLaCunostinta"] = lst["NrZileLuatLaCunostinta"];
                                if (lst.Contains("IdRaport")) ds.Tables[0].Rows[0]["IdRaport"] = lst["IdRaport"];
                            }
                        }
                    }
                }


                if (Session["esteQuizNou"] != null && Session["esteQuizNou"].ToString().Length > 0 && Session["esteQuizNou"].ToString() == "true")
                {
                    InserareQuiz(Session["IdEvalQuiz"].ToString(), ds.Tables[0], ds.Tables[1], ds.Tables[2], ds.Tables[3]);
                    Session["esteQuizNou"] = "false";
                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                }

                #region salvare Template Obiective
                //salvare Eval_ConfigObTemplate
                try
                {                    
                    string sqlEval_ConfigObTemplate = string.Empty;
                    List<Eval_ConfigObTemplate> lstConfigObTemplate = Session["createEval_ConfigObTemplate"] as List<Eval_ConfigObTemplate>;
                    if(lstConfigObTemplate.Count!=0)
                    {
                        sqlEval_ConfigObTemplate = "delete from \"Eval_ConfigObTemplate\" ";
                        General.ExecutaNonQuery(sqlEval_ConfigObTemplate, null);
                    }
                    General.SalveazaDate(Evaluare.ToDataTable<Eval_ConfigObTemplate>(lstConfigObTemplate), "Eval_ConfigObTemplate");
                }
                catch { }


                //salvare Eval_ConfigObTemplateDetail
                try
                {                    
                    List<Eval_ConfigObTemplateDetail> lstConfigObTemplateDetail = Session["createEval_ConfigObTemplateDetail"] as List<Eval_ConfigObTemplateDetail>;
                    string sqlEval_ConfigObTemplateDetail = string.Empty;
                    if(lstConfigObTemplateDetail.Count!=0)
                    {
                        sqlEval_ConfigObTemplateDetail = "delete from \"Eval_ConfigObTemplateDetail\" ";
                        General.ExecutaNonQuery(sqlEval_ConfigObTemplateDetail, null);
                    }
                    General.SalveazaDate(Evaluare.ToDataTable<Eval_ConfigObTemplateDetail>(lstConfigObTemplateDetail), "Eval_ConfigObTemplateDetail");
                }
                catch { }
                #endregion

                #region salvare Template Competente
                //salvare Eval_ConfigCompTemplate
                try
                {                    
                    string sqlEval_ConfigCompTemplate = string.Empty;
                    List<Eval_ConfigCompTemplate> lstConfigCompTemplate = Session["createEval_ConfigCompTemplate"] as List<Eval_ConfigCompTemplate>;
                    if (lstConfigCompTemplate.Count != 0)
                    {
                        sqlEval_ConfigCompTemplate = "delete from \"Eval_ConfigCompTemplate\" ";
                        General.ExecutaNonQuery(sqlEval_ConfigCompTemplate, null);
                    }
                    General.SalveazaDate(Evaluare.ToDataTable<Eval_ConfigCompTemplate>(lstConfigCompTemplate), "Eval_ConfigCompTemplate");
                }
                catch { }

                //salvare Eval_ConfigCompTemplateDetail
                try
                {                   
                    List<Eval_ConfigCompTemplateDetail> lstConfigCompTemplateDetail = Session["createEval_ConfigCompTemplateDetail"] as List<Eval_ConfigCompTemplateDetail>;
                    string sqlEval_ConfigCompTemplateDetail = string.Empty;
                    if (lstConfigCompTemplateDetail.Count != 0)
                    {
                        sqlEval_ConfigCompTemplateDetail = "delete from \"Eval_ConfigCompTemplateDetail\" ";
                        General.ExecutaNonQuery(sqlEval_ConfigCompTemplateDetail, null);
                    }
                    General.SalveazaDate(Evaluare.ToDataTable<Eval_ConfigCompTemplateDetail>(lstConfigCompTemplateDetail), "Eval_ConfigCompTemplateDetail");
                }
                catch { }
                #endregion

                #region Salvare ordine corecta Eval_QuizIntrebari
                int idQuiz = Convert.ToInt32(Session["IdEvalQuiz"].ToString());
                string tip = "nvarchar(10)";
                if (Constante.tipBD == 2)
                    tip = "varchar2(10)";
                
                string sqlOrdine = $@"
                            select '-' {Dami.Operator()} cast(rot.""Id"" as {tip}) {Dami.Operator()} '-' {Dami.Operator()} cast(sec.""Id"" as {tip}) {Dami.Operator()} '-' {Dami.Operator()} cast(intre.""Id"" as {tip}) {Dami.Operator()} '-' as ""Ordine"", intre.""Id""
                            from ""Eval_QuizIntrebari"" intre
                            join ""Eval_QuizIntrebari"" sec on intre.""Parinte"" = sec.""Id""
                            join ""Eval_QuizIntrebari"" rot on sec.""Parinte"" = rot.""Id""
                            left join ""Eval_QuizIntrebari"" cop on intre.""Id"" = cop.""Parinte""
                            where cop.""Id"" is null
                            and intre.""IdQuiz"" = {idQuiz}
                            union all
                            select '-' {Dami.Operator()} cast(rot.""Id"" as {tip}) {Dami.Operator()} '-' {Dami.Operator()} cast(sec.""Id"" as {tip}) {Dami.Operator()} '-' as ""Ordine"", sec.""Id""
                            from ""Eval_QuizIntrebari"" rot
                            join ""Eval_QuizIntrebari"" sec on rot.""Id"" = sec.""Parinte""
                            where rot.""IdQuiz"" = {idQuiz}
                            and rot.""Descriere"" ='Root'
                            union all
                            select '-' {Dami.Operator()} CAST(rot.""Id"" as {tip}) {Dami.Operator()} '-' as ""Ordine"", rot.""Id""
                            from ""Eval_QuizIntrebari"" rot
                            where rot.""IdQuiz"" = {idQuiz}
                            and rot.""Descriere"" = 'Root'";

                DataTable dtOrdine = General.IncarcaDT(sqlOrdine, null);
                if (dtOrdine != null && dtOrdine.Rows.Count != 0)
                {
                    foreach (DataRow dr in dtOrdine.Rows)
                    {
                        metaOrdine clsOrdine = new metaOrdine(dr);
                        General.ExecutaNonQuery(@"UPDATE ""Eval_QuizIntrebari"" SET ""Ordine"" = @1 WHERE ""Id"" = @2", new object[] { clsOrdine.Ordine, clsOrdine.Id });
                    }
                }

                #endregion

                //MessageBox.Show("Proces realizat cu succes!", MessageBox.icoSuccess);
                ASPxPageControl2.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces finalizat cu succes");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        #region Methods
        protected void Initializare(DataSet ds)
        {
            try
            {
                ds = new DataSet();

                #region Eval_Quiz
                DataTable dtEval_Quiz = General.IncarcaDT("select * from \"Eval_Quiz\"  where \"Id\" = (select min(\"Id\") from \"Eval_Quiz\")", null);
                dtEval_Quiz.TableName = "Eval_Quiz";
                object[] rowEval_Quiz = new object[dtEval_Quiz.Columns.Count];

                int x = 0;
                foreach (DataColumn col in dtEval_Quiz.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ID":
                            rowEval_Quiz[x] = Convert.ToInt32(Session["IdEvalQuiz"].ToString());
                            break;
                        case "USER_NO":
                            rowEval_Quiz[x] = Session["UserId"];
                            break;
                        case "TIME":
                            rowEval_Quiz[x] = DateTime.Now;
                            break;
                        case "CATEGORIEQUIZ":
                            rowEval_Quiz[x] = Convert.ToInt32(Session["Quiz360"].ToString());
                            break;
                    }
                    x++;
                }

                if (dtEval_Quiz.Rows.Count != 0)
                    dtEval_Quiz.Rows.RemoveAt(0);
                dtEval_Quiz.Rows.Add(rowEval_Quiz);
                dtEval_Quiz.PrimaryKey = new DataColumn[] { dtEval_Quiz.Columns["IdQuiz"] };

                #endregion

                #region Eval_QuizIntrebari
                DataTable dtEval_QuizIntrebari = General.IncarcaDT("select * from \"Eval_QuizIntrebari\" where \"Id\" = (select min(\"Id\") from \"Eval_QuizIntrebari\")", null);
                dtEval_QuizIntrebari.TableName = "Eval_QuizIntrebari";
                object[] rowEval_QuizIntrebari = new object[dtEval_QuizIntrebari.Columns.Count];

                x = 0;
                foreach (DataColumn col in dtEval_QuizIntrebari.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDQUIZ":
                            rowEval_QuizIntrebari[x] = Convert.ToInt32(Session["IdEvalQuiz"].ToString());
                            break;
                        case "USER_NO":
                            rowEval_QuizIntrebari[x] = Session["UserId"];
                            break;
                        case "TIME":
                            rowEval_QuizIntrebari[x] = DateTime.Now;
                            break;
                        case "ID":
                            rowEval_QuizIntrebari[x] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Id"",0)) FROM ""Eval_QuizIntrebari"" "), 0)) + 1;
                            break;
                        case "DESCRIERE":
                            rowEval_QuizIntrebari[x] = "Root";
                            break;
                        case "PARINTE":
                            rowEval_QuizIntrebari[x] = 0;
                            break;
                        case "ESTESECTIUNE":
                            rowEval_QuizIntrebari[x] = 1;
                            break;
                    }
                    x++;
                }
                if (dtEval_QuizIntrebari.Rows.Count != 0)
                    dtEval_QuizIntrebari.Rows.RemoveAt(0);
                dtEval_QuizIntrebari.Rows.Add(rowEval_QuizIntrebari);
                dtEval_QuizIntrebari.PrimaryKey = new DataColumn[] { dtEval_QuizIntrebari.Columns["Id"] };
                #endregion

                #region Eval_Circuit
                DataTable dtEval_Circuit = General.IncarcaDT("select * from \"Eval_Circuit\" where \"IdAuto\" = (select min(\"IdAuto\") from \"Eval_Circuit\")", null);
                dtEval_Circuit.TableName = "Eval_Circuit";
                object[] rowEval_Circuit = new object[dtEval_Circuit.Columns.Count];

                x = 0;
                foreach (DataColumn col in dtEval_Circuit.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDQUIZ":
                            rowEval_Circuit[x] = Convert.ToInt32(Session["IdEvalQuiz"].ToString());
                            break;
                        case "USER_NO":
                            rowEval_Circuit[x] = Session["UserId"];
                            break;
                        case "TIME":
                            rowEval_Circuit[x] = DateTime.Now;
                            break;
                    }
                    x++;
                }
                if (dtEval_Circuit.Rows.Count != 0)
                    dtEval_Circuit.Rows.RemoveAt(0);
                dtEval_Circuit.Rows.Add(rowEval_Circuit);
                dtEval_Circuit.PrimaryKey = new DataColumn[] { dtEval_Circuit.Columns["IdAuto"] };
                #endregion

                #region Eval_Drepturi
                DataTable dtEval_Drepturi = General.IncarcaDT("select * from \"Eval_Drepturi\" where \"IdAuto\" = (select min(\"IdAuto\") from \"Eval_Drepturi\")", null);
                dtEval_Drepturi.TableName = "Eval_Drepturi";
                object[] rowEval_Drepturi = new object[dtEval_Drepturi.Columns.Count];

                x = 0;
                foreach (DataColumn col in dtEval_Drepturi.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "IDQUIZ":
                            rowEval_Drepturi[x] = Convert.ToInt32(Session["IdEvalQuiz"].ToString());
                            break;
                        case "USER_NO":
                            rowEval_Drepturi[x] = Session["UserId"];
                            break;
                        case "TIME":
                            rowEval_Drepturi[x] = DateTime.Now;
                            break;
                        case "POZITIE":
                            rowEval_Drepturi[x] = 1;
                            break;
                        case "POZITIEVIZIBILA":
                            rowEval_Drepturi[x] = 1;
                            break;

                    }
                    x++;
                }
                if (dtEval_Drepturi.Rows.Count != 0)
                    dtEval_Drepturi.Rows.RemoveAt(0);
                dtEval_Drepturi.Rows.Add(rowEval_Drepturi);
                dtEval_Drepturi.PrimaryKey = new DataColumn[] { dtEval_Drepturi.Columns["IdAuto"] };
                #endregion

                ds.Tables.Add(dtEval_Quiz);
                ds.Tables.Add(dtEval_QuizIntrebari);
                ds.Tables.Add(dtEval_Circuit);
                ds.Tables.Add(dtEval_Drepturi);
                Session["InformatiaCurentaEvalQuiz"] = ds;
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void InserareQuiz(string IdQuiz, DataTable ctQuiz, DataTable ctQuizIntrebari, DataTable ctCircuit, DataTable ctDrepturi)
        {
            try
            {
                General.SalveazaDate(ctQuiz, "Eval_Quiz");
                General.SalveazaDate(ctQuizIntrebari, "Eval_QuizIntrebari");
                General.SalveazaDate(ctCircuit, "Eval_Circuit");
                General.SalveazaDate(ctDrepturi, "Eval_Drepturi");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        protected void ASPxPageControl2_Callback(object sender, CallbackEventArgsBase e)
        {
            btnSave_Click(null, null);
        }
    }
}