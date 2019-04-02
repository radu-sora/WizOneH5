using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class EvalDetaliu : System.Web.UI.Page
    {
        private string _idClient
        {
            get { return (Session["IdClient"] as int? ?? 1).ToString(); }
        }

        private int _idQuiz
        {
            get { return (_idQuiz as int? ?? 1); }
        }

        private int _f10003
        {
            get { return (_f10003 as int? ?? 1); }
        }

        private int _trebuieSaIaLaCunostinta
        {
            get { return (Session["CompletareChestionar_TrebuieSaIaLaCunostinta"] as int? ?? 0); }
        }

        private int _aLuatLaCunostinta
        {
            get { return (Session["CompletareChestionar_ALuatLaCunostinta"] as int? ?? 0); }
        }

        private int _pozitieQuiz
        {
            get { return (Session["CompletareChestionar_Pozitie"] as int? ?? 1); }
        }

        private int _finalizat
        {
            get { return (Session["CompletareChestionar_Finalizat"] as int? ?? 0); }
        }

        private int _modifica
        {
            get { return (Session["CompletareChestionar_Modifica"] as int? ?? 1); }
        }

        private string _numeAngajat
        {
            get { return (Session["CompletareChestionar_Nume"] as string ?? "").ToString(); }
        }

        private int _pozitieUserLogat
        {
            get { return (Session["Eval_PozitieUserLogat"] as int? ?? 1); }
        }

        #region Clase
        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
            public int ParentId { get; set; }
        }
        #endregion

        #region Variabile
        List<Eval_Raspuns> lstEval_Raspuns = new List<Eval_Raspuns>();
        List<Eval_RaspunsLinii> lstEval_RaspunsLinii = new List<Eval_RaspunsLinii>();
        List<Eval_QuizIntrebari> lstEval_QuizIntrebari = new List<Eval_QuizIntrebari>();
        List<Eval_tblTipValoriLinii> lstEval_tblTipValoriLinii = new List<Eval_tblTipValoriLinii>();

        #region Obiective

        List<Eval_ConfigObTemplate> lstEval_ConfigObTemplate = new List<Eval_ConfigObTemplate>();
        List<Eval_ConfigObTemplateDetail> lstEval_ConfigObTemplateDetail = new List<Eval_ConfigObTemplateDetail>();
        List<Eval_ConfigObiective> lstEval_ConfigObiective = new List<Eval_ConfigObiective>();
        List<Eval_SetCalificativDet> lstEval_SetCalificativDet = new List<Eval_SetCalificativDet>();
        List<Eval_ObiIndividualeTemp> lstEval_ObiIndividualeTemp = new List<Eval_ObiIndividualeTemp>();

        List<metaDate> lstObiective = new List<metaDate>();
        List<metaDate> lstActivitati = new List<metaDate>();
        #endregion

        #region Competente
        List<Eval_ConfigCompTemplate> lstEval_ConfigCompTemplate = new List<Eval_ConfigCompTemplate>();
        List<Eval_ConfigCompTemplateDetail> lstEval_ConfigCompTemplateDetail = new List<Eval_ConfigCompTemplateDetail>();
        List<Eval_ConfigCompetente> lstEval_ConfigCompetente = new List<Eval_ConfigCompetente>();
        List<Eval_SetCalificativDet> lstEval_CompSetCalificativDet = new List<Eval_SetCalificativDet>();
        List<Eval_CompetenteAngajatTemp> lstEval_CompetenteAngajatTemp = new List<Eval_CompetenteAngajatTemp>();

        List<metaDate> lstCompetente = new List<metaDate>();
        #endregion

        int totalSec = 0;
        List<int> lstSec = new List<int>();
        int indexSec = -99;
        int font = 16;
        //List<HtmlTableCell> lstSectiuni = new List<HtmlTableCell>();
        HtmlTable grIntrebari = new HtmlTable();
        enum tipSectiune
        {
            SageataAlbaIntermediara = 1,
            SageataAlbastraUltima = 2,
            SageataAlbaPrima = 3
        }
        #endregion

        string idCateg = "0";

        protected void Page_Init(object sender, EventArgs e)
        {

            try
            {
                idCateg = General.Nz(General.ExecutaScalar(@"SELECT ""CategorieQuiz"" FROM ""Eval_Quiz"" WHERE ""Id""=@1", new object[] { _idQuiz }), "0").ToString();

                lblEvaluat.InnerText = Dami.TraduCuvant("Evaluat") + ":" + _numeAngajat;

                if (!IsPostBack)
                {
                    //Begin Florin 2019.01.21
                    DataTable tableAAA = General.IncarcaDT(@"SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2", new object[] { _idQuiz, _f10003 });
                    tableAAA.TableName = "Eval_Raspuns";

                    DataTable tableBBB = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2", new object[] { _idQuiz, _f10003 });
                    tableBBB.TableName = "Eval_RaspunsLinii";

                    DataTable tableCCC = General.IncarcaDT(@"SELECT * FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1", new object[] { _idQuiz });
                    tableCCC.TableName = "Eval_QuizIntrebari";

                    DataSet ds = new DataSet();
                    ds.Tables.Add(tableAAA);
                    tableBBB.PrimaryKey = new DataColumn[] { tableBBB.Columns["Id"] };
                    ds.Tables.Add(tableBBB);
                    tableCCC.PrimaryKey = new DataColumn[] { tableCCC.Columns["Id"] };
                    ds.Tables.Add(tableCCC);

                    Session["InformatiaCurentaCompletareChestionar"] = ds;
                    //End Florin 2019.01.21

                    DataTable table = new DataTable();
                    DataTable tableIntrebari = new DataTable();
                    //DataSet ds = Session["InformatiaCurentaCompletareChestionar"] as DataSet;

                    table = ds.Tables[0];
                    lstEval_Raspuns = new List<Eval_Raspuns>();
                    foreach (DataRow rwEval_Raspuns in table.Rows)
                    {
                        Eval_Raspuns clsEval_Raspuns = new Eval_Raspuns(rwEval_Raspuns);
                        lstEval_Raspuns.Add(clsEval_Raspuns);
                    }
                    Session["lstEval_Raspuns"] = lstEval_Raspuns;
                    //log += "Eval_Raspuns - " + lstEval_Raspuns.Count() + Environment.NewLine;

                    tableIntrebari = ds.Tables[1];
                    lstEval_RaspunsLinii = new List<Eval_RaspunsLinii>();
                    foreach (DataRow rwEval_RaspunsLinii in tableIntrebari.Rows)
                    {
                        Eval_RaspunsLinii clsEval_RaspunsLinii = new Eval_RaspunsLinii(rwEval_RaspunsLinii);
                        lstEval_RaspunsLinii.Add(clsEval_RaspunsLinii);
                    }
                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;
                    //log += "Eval_RaspunsLinii - " + lstEval_RaspunsLinii.Count() + Environment.NewLine;

                    DataTable table2 = ds.Tables[2];
                    lstEval_QuizIntrebari = new List<Eval_QuizIntrebari>();
                    foreach (DataRow rwEval_QuizIntrebari in table2.Rows)
                    {
                        Eval_QuizIntrebari clsEval_QuizIntrebari = new Eval_QuizIntrebari(rwEval_QuizIntrebari);
                        lstEval_QuizIntrebari.Add(clsEval_QuizIntrebari);
                    }
                    Session["lstEval_QuizIntrebari"] = lstEval_QuizIntrebari;
                    //log += "Eval_QuizIntrebari - " + lstEval_QuizIntrebari.Count() + Environment.NewLine;

                    DataTable table3 = General.IncarcaDT(@"select * from ""Eval_tblTipValoriLinii""", null);
                    lstEval_tblTipValoriLinii = new List<Eval_tblTipValoriLinii>();
                    foreach (DataRow dr3 in table3.Rows)
                    {
                        Eval_tblTipValoriLinii clsNew = new Eval_tblTipValoriLinii(dr3);
                        lstEval_tblTipValoriLinii.Add(clsNew);
                    }
                    Session["lstEval_tblTipValoriLinii"] = lstEval_tblTipValoriLinii;

                    //Florin 2018.12.20
                    Session["Aprobat360"] = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""IdUser""=@3", new object[] { _idQuiz, _f10003, Session["UserId"] }), 1));

                    CreazaMeniu();
                    //Session["lstSectiuni"] = lstSectiuni;

                    Session["indexSec"] = indexSec;
                    Session["totalSec"] = totalSec;
                    txtNrSectiune.Text = (indexSec + 1).ToString();
                    txtNrTotalSectiune.Text = totalSec.ToString();

                    //Radu 17.07.2018
                    //int IdQuiz = Convert.ToInt32(_idQuiz ?? -99);
                    //int F10003 = Convert.ToInt32(_f10003 ?? -99);


                    #region Luat la cunostinta

                    //Florin 2019.01.29
                    if (_f10003 == Convert.ToInt32(General.Nz(Session["User_Marca"], -98)) && _finalizat == 1 && _trebuieSaIaLaCunostinta == 1 && _aLuatLaCunostinta == 0) btnLuatCunostinta.Visible = true;

                    

                    ////string sql = "SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {0} and F10003 = {1} AND ""Pozitie"" = {2}";
                    ////sql = string.Format(sql, IdQuiz, F10003, (Convert.ToInt32(_idClient) == 21 ? "3" : "1"));
                    //DataTable dt = General.IncarcaDT(@"SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3", new object[] { IdQuiz, F10003, (Convert.ToInt32(_idClient) == 21 ? "3" : "1") });

                    ////sql = "SELECT ""LuatLaCunostinta"", ""NrZileLuatLaCunostinta"" FROM ""Eval_Quiz"" WHERE ""Id"" = {0}" ;
                    ////sql = string.Format(sql, IdQuiz);
                    //DataTable dtQuiz = General.IncarcaDT(@"SELECT ""LuatLaCunostinta"", ""NrZileLuatLaCunostinta"" FROM ""Eval_Quiz"" WHERE ""Id"" = @1", new object[] { IdQuiz });
                    //if (dtQuiz.Rows[0]["LuatLaCunostinta"] != null && dtQuiz.Rows[0]["LuatLaCunostinta"].ToString().Length > 0 && dtQuiz.Rows[0]["LuatLaCunostinta"].ToString() != "0")
                    //{
                    //    int nrZile = -1;
                    //    if (dtQuiz.Rows[0]["NrZileLuatLaCunostinta"] != null && dtQuiz.Rows[0]["NrZileLuatLaCunostinta"].ToString().Length > 0)                        
                    //        nrZile = Convert.ToInt32(dtQuiz.Rows[0]["NrZileLuatLaCunostinta"].ToString());

                    //    //string data = (Constante.tipBD == 1 ? "CONVERT(VARCHAR, a.""DataAprobare"", 103)" : "TO_CHAR(a.""DataAprobare"", 'dd/mm/yyyy')");
                    //    //sql = "SELECT {2} AS ""DataAprobare"" FROM ""Eval_RaspunsIstoric"" a JOIN ""Eval_Raspuns"" b on a.""IdQuiz"" = b.""IdQuiz"" AND a.F10003 = b.F10003 WHERE a.""IdQuiz"" = {0} AND a.F10003 =  {1} AND a.""Pozitie"" = b.""TotalCircuit"" {3} ";
                    //    //sql = string.Format(sql, IdQuiz, F10003, data, (Convert.ToInt32(_idClient) == 21 ? " - 1" : ""));
                    //    //DataTable dtRaspIst = General.IncarcaDT(
                    //    //    @"SELECT {2} AS ""DataAprobare"" FROM ""Eval_RaspunsIstoric"" A 
                    //    //    JOIN ""Eval_Raspuns"" b on a.""IdQuiz"" = b.""IdQuiz"" AND a.F10003 = b.F10003 
                    //    //    WHERE a.""IdQuiz"" = {0} AND a.F10003 =  {1} AND a.""Pozitie"" = b.""TotalCircuit"" { 3} ", null);

                    //    DataTable dtRaspIst = General.IncarcaDT(
                    //        @"SELECT ""DataAprobare"" FROM ""Eval_RaspunsIstoric"" A 
                    //        INNER JOIN ""Eval_Raspuns"" B ON A.""IdQuiz"" = B.""IdQuiz"" AND A.F10003 = B.F10003 
                    //        WHERE A.""IdQuiz"" = @1 AND A.F10003 =  @2 AND A.""Pozitie"" = B.""TotalCircuit""" + (Convert.ToInt32(_idClient) == 21 ? " - 1" : ""), new object[] { IdQuiz, F10003 });
                    //    if (dtRaspIst != null && dtRaspIst.Rows.Count > 0 && General.Nz(dtRaspIst.Rows[0]["DataAprobare"],"").ToString().Length > 0)
                    //    {
                    //        //DateTime dtAprob = new DateTime(Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(6, 4)), Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(3, 2)), Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(0, 2)));
                    //        DateTime dtAprob = Convert.ToDateTime(dtRaspIst.Rows[0]["DataAprobare"]);
                    //        if (nrZile <= 0 || (nrZile > 0 && (DateTime.Now - dtAprob).Days <= nrZile))
                    //            if (((lstEval_Raspuns.FirstOrDefault().Finalizat == 1 && Convert.ToInt32(_idClient) != 21) || (Convert.ToInt32(_idClient) == 21 && lstEval_Raspuns.FirstOrDefault().Finalizat != 1 && lstEval_Raspuns.FirstOrDefault().Pozitie == 3)) 
                    //                && lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta != 1 && lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta != 2
                    //                && Convert.ToInt32(Session["UserId"].ToString()) == Convert.ToInt32(dt.Rows[0][0].ToString()))
                    //                btnLuatCunostinta.Visible = true;
                    //    }
                    //}


                    #endregion

                    #region CreeazaTab
                    int consRU = Convert.ToInt32(Session["consRU"] ?? 0);
                    int Pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"] ?? -99);

                    //Florin 2018.12.18
                    List<metaEvalDenumireSuper> lstEvalDenumireSuper = Evaluare.EvalDenumireSuper(_idQuiz, _pozitieUserLogat, consRU);


                    Session["lstEvalDenumireSuper"] = lstEvalDenumireSuper;
                    CreazaTab(lstEvalDenumireSuper);

                    //CreeazaSectiune("Super" + Pozitie);


                    //Florin 2019.01.28
                    Session["Eval_ActiveTab"] = tabSuper.ActiveTab.Name.Replace("tab", "");
                    //Session["Eval_ActiveTab"] = 0;


                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"]);

                    #endregion
                }
                else
                {
                    lstEval_Raspuns = Session["lstEval_Raspuns"] as List<Eval_Raspuns>;
                    lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;
                    lstEval_QuizIntrebari = Session["lstEval_QuizIntrebari"] as List<Eval_QuizIntrebari>;
                    List<metaEvalDenumireSuper> lstEvalDenumireSuper = Session["lstEvalDenumireSuper"] as List<metaEvalDenumireSuper>;
                    indexSec = Convert.ToInt32(Session["indexSec"].ToString());
                    totalSec = Convert.ToInt32(Session["totalSec"].ToString());
                    //lstSectiuni = Session["lstSectiuni"] as List<HtmlTableCell>;
                    int Pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"] ?? -99);

                    ////Radu 17.07.2018
                    //int IdQuiz = Convert.ToInt32(_idQuiz ?? -99);
                    //int F10003 = Convert.ToInt32(_f10003 ?? -99);


                    #region Luat la cunostinta

                    //Florin 2019.01.29
                    if (_f10003 == Convert.ToInt32(General.Nz(Session["User_Marca"], -98)) && _finalizat == 1 && _trebuieSaIaLaCunostinta == 1 && _aLuatLaCunostinta == 0) btnLuatCunostinta.Visible = true;



                    ////string sql = "SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {0} and F10003 = {1} AND ""Pozitie"" = {2}";
                    ////sql = string.Format(sql, IdQuiz, F10003, (Convert.ToInt32(_idClient) == 21 ? "3" : "1"));
                    //DataTable dt = General.IncarcaDT(@"SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3", new object[] { IdQuiz, F10003, (Convert.ToInt32(_idClient) == 21 ? "3" : "1") });

                    ////sql = "SELECT ""LuatLaCunostinta"", ""NrZileLuatLaCunostinta"" FROM ""Eval_Quiz"" WHERE ""Id"" = {0}";
                    ////sql = string.Format(sql, IdQuiz);
                    //DataTable dtQuiz = General.IncarcaDT(@"SELECT ""LuatLaCunostinta"", ""NrZileLuatLaCunostinta"" FROM ""Eval_Quiz"" WHERE ""Id"" = @1", new object[] { IdQuiz });
                    //if (dtQuiz.Rows[0]["LuatLaCunostinta"] != null && dtQuiz.Rows[0]["LuatLaCunostinta"].ToString().Length > 0 && dtQuiz.Rows[0]["LuatLaCunostinta"].ToString() != "0")
                    //{
                    //    int nrZile = -1;
                    //    if (dtQuiz.Rows[0]["NrZileLuatLaCunostinta"] != null && dtQuiz.Rows[0]["NrZileLuatLaCunostinta"].ToString().Length > 0)
                    //        nrZile = Convert.ToInt32(dtQuiz.Rows[0]["NrZileLuatLaCunostinta"].ToString());

                    //    //string data = (Constante.tipBD == 1 ? "CONVERT(VARCHAR, a.""DataAprobare"", 103)" : "TO_CHAR(a.""DataAprobare"", 'dd/mm/yyyy')");
                    //    //sql = "SELECT {2} AS ""DataAprobare"" FROM ""Eval_RaspunsIstoric"" a JOIN ""Eval_Raspuns"" b on a.""IdQuiz"" = b.""IdQuiz"" AND a.F10003 = b.F10003 WHERE a.""IdQuiz"" = {0} AND a.F10003 =  {1} AND a.""Pozitie"" = b.""TotalCircuit"" {3} ";
                    //    //sql = string.Format(sql, IdQuiz, F10003, data, (Convert.ToInt32(_idClient) == 21 ? " - 1" : ""));
                    //    DataTable dtRaspIst = General.IncarcaDT(
                    //        @"SELECT ""DataAprobare"" FROM ""Eval_RaspunsIstoric"" A 
                    //        INNER JOIN ""Eval_Raspuns"" B ON A.""IdQuiz"" = B.""IdQuiz"" AND A.F10003 = B.F10003 
                    //        WHERE A.""IdQuiz"" = @1 AND A.F10003 =  @2 AND A.""Pozitie"" = B.""TotalCircuit""" + (Convert.ToInt32(_idClient) == 21 ? " - 1" : ""), new object[] { IdQuiz, F10003 });
                    //    if (dtRaspIst != null && dtRaspIst.Rows.Count > 0 && General.Nz(dtRaspIst.Rows[0]["DataAprobare"], "").ToString().Length > 0)
                    //    {
                    //        //DateTime dtAprob = new DateTime(Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(6, 4)), Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(3, 2)), Convert.ToInt32(dtRaspIst.Rows[0]["DataAprobare"].ToString().Substring(0, 2)));
                    //        DateTime dtAprob = Convert.ToDateTime(dtRaspIst.Rows[0]["DataAprobare"]);
                    //        if (nrZile <= 0 || (nrZile > 0 && (DateTime.Now - dtAprob).Days <= nrZile))
                    //            if (((lstEval_Raspuns.FirstOrDefault().Finalizat == 1 && Convert.ToInt32(_idClient) != 21) || (Convert.ToInt32(_idClient) == 21 && lstEval_Raspuns.FirstOrDefault().Finalizat != 1 && lstEval_Raspuns.FirstOrDefault().Pozitie == 3))
                    //                && lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta != 1 && lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta != 2
                    //                && Convert.ToInt32(Session["UserId"].ToString()) == Convert.ToInt32(dt.Rows[0][0].ToString()))
                    //                btnLuatCunostinta.Visible = true;
                    //    }
                    //}

                    #endregion  

                    CreazaMeniu();
                    CreazaTab(lstEvalDenumireSuper);

                    //Florin 2019.01.28

                    ////Radu 20.07.2018
                    ////Session["Eval_ActiveTab"] = tabSuper.ActiveTabIndex;
                    ////CreeazaSectiune("Super" + (tabSuper.ActiveTabIndex + 1).ToString());
                    //CreeazaSectiune("Super" + (Convert.ToInt32(Session["Eval_ActiveTab"] ?? 0) + 1).ToString());
                    ////CreeazaSectiune("Super" + Session["CompletareChestionar_Pozitie"]);
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());
                }

                //General.MemoreazaEroarea(log);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset session data                    
            Session.Remove("CompletareChestionar_IdQuiz");
            Session.Remove("CompletareChestionar_F10003");
            Session.Remove("CompletareChestionar_TrebuieSaIaLaCunostinta");
            Session.Remove("CompletareChestionar_ALuatLaCunostinta");
            Session.Remove("CompletareChestionar_Pozitie");
            Session.Remove("CompletareChestionar_Finalizat");
            Session.Remove("CompletareChestionar_Modifica");
            Session.Remove("CompletareChestionar_Nume");
            Session.Remove("Eval_PozitieUserLogat");
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlCommandDelete = string.Empty;
                string sqlCommandDeleteTemp = string.Empty;
                string sqlCommandInsert = string.Empty;
                string sqlCommandInsertTemp = string.Empty;

                #region Set Scripts Upload DB
                sqlCommandDelete = @"delete from ""Eval_RaspunsLinii"" where ""IdQuiz"" = @1 and ""F10003"" = @2 and ""Id"" = @3";
                sqlCommandInsert = @"insert into ""Eval_RaspunsLinii""(""IdQuiz"", ""F10003"", ""Id"", ""Linia"", 
                                                ""Super1"",""Super2"",""Super3"",""Super4"",""Super5"",
                                                ""Super6"",""Super7"",""Super8"",""Super9"",""Super10"",
                                                ""Super11"",""Super12"",""Super13"",""Super14"",""Super15"",
                                                ""Super16"",""Super17"",""Super18"",""Super19"",""Super20"",
                                                ""USER_NO"",""TIME"",""Descriere"",""TipData"",""TipValoare"",
                                                ""Sublinia"",""Tinta"",""Super1_1"",""Super1_2"",""Super1_3"",
                                                ""Super2_1"",""Super2_2"",""Super2_3"",""Super3_1"",""Super3_2"",
                                                ""Super3_3"",""Super4_1"",""Super4_2"",""Super4_3"",""Super5_1"",
                                                ""Super5_2"",""Super5_3"",""Super6_1"",""Super6_2"",""Super6_3"",
                                                ""Super7_1"",""Super7_2"",""Super7_3"",""Super8_1"",""Super8_2"",
                                                ""Super8_3"",""Super9_1"",""Super9_2"",""Super9_3"",""Super10_1"",
                                                ""Super10_2"",""Super10_3"",""IdGrup"",""PondereRatingGlobal"",""NumeGrup"",
                                                ""Super11_1"",""Super11_2"",""Super11_3"",""Super12_1"",""Super12_2"",
                                                ""Super12_3"",""Super13_1"",""Super13_2"",""Super13_3"",""Super14_1"",
                                                ""Super14_2"",""Super14_3"",""Super15_1"",""Super15_2"",""Super15_3"",
                                                ""Super16_1"",""Super16_2"",""Super16_3"",""Super17_1"",""Super17_2"",
                                                ""Super17_3"",""Super18_1"",""Super18_2"",""Super18_3"",""Super19_1"",
                                                ""Super19_2"",""Super19_3"",""Super20_1"",""Super20_2"",""Super20_3"",
                                                ""Super1_4"",""Super1_5"",""Super1_6"",""Super2_4"",""Super2_5"",
                                                ""Super2_6"",""Super3_4"",""Super3_5"",""Super3_6"",""Super4_4"",
                                                ""Super4_5"",""Super4_6"",""Super5_4"",""Super5_5"",""Super5_6"",
                                                ""Super6_4"",""Super6_5"",""Super6_6"",""Super7_4"",""Super7_5"",
                                                ""Super7_6"",""Super8_4"",""Super8_5"",""Super8_6"",""Super9_4"",
                                                ""Super9_5"",""Super9_6"",""Super10_4"",""Super10_5"",""Super10_6"",
                                                ""Super11_4"",""Super11_5"",""Super11_6"",""Super12_4"",""Super12_5"",
                                                ""Super12_6"",""Super13_4"",""Super13_5"",""Super13_6"",""Super14_4"",
                                                ""Super14_5"",""Super14_6"",""Super15_4"",""Super15_5"",""Super15_6"",
                                                ""Super16_4"",""Super16_5"",""Super16_6"",""Super17_4"",""Super17_5"",
                                                ""Super17_6"",""Super18_4"",""Super18_5"",""Super18_6"",""Super19_4"",
                                                ""Super19_5"",""Super19_6"",""Super20_4"",""Super20_5"",""Super20_6"",
                                                ""DescriereInRatingGlobal"")
                                              values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,
                                                @20,@21,@22,@23,@24,@25,@26,@27,@28,@29,@30,@31,@32,@33,@34,@35,@36,@37,@38,@39,
                                                @40,@41,@42,@43,@44,@45,@46,@47,@48,@49,@50,@51,@52,@53,@54,@55,@56,@57,@58,@59,
                                                @60,@61,@62,@63,@64,@65,@66,@67,@68,@69,@70,@71,@72,@73,@74,@75,@76,@77,@78,@79,
                                                @80,@81,@82,@83,@84,@85,@86,@87,@88,@89,@90,@91,@92,@93,@94,@95,@96,@97,@98,@99,
                                                @100,@101,@102,@103,@104,@105,@106,@107,@108,@109,@110,@111,@112,@113,@114,@115,@116,@117,@118,@119,
                                                @120,@121,@122,@123,@124,@125,@126,@127,@128,@129,@130,@131,@132,@133,@134,@135,@136,@137,@138,@139,
                                                @140,@141,@142,@143,@144,@145,@146,@147,@148,@149,@150,@151,@152,@153,@154,@155);";
                #endregion

                lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;
                foreach (Eval_RaspunsLinii entRaspLinie in lstEval_RaspunsLinii)
                {
                    try
                    {
                        sqlCommandDeleteTemp = sqlCommandDelete;
                        General.ExecutaNonQuery(sqlCommandDeleteTemp, new object[] { entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id });

                        sqlCommandInsertTemp = sqlCommandInsert;
                        General.ExecutaNonQuery(sqlCommandInsertTemp, new object[] {  entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id,
                                            entRaspLinie.Linia, entRaspLinie.Super1, entRaspLinie.Super2, entRaspLinie.Super3, entRaspLinie.Super4,
                                            entRaspLinie.Super5, entRaspLinie.Super6, entRaspLinie.Super7, entRaspLinie.Super8, entRaspLinie.Super9,
                                            entRaspLinie.Super10, entRaspLinie.Super11, entRaspLinie.Super12, entRaspLinie.Super13, entRaspLinie.Super14,
                                            entRaspLinie.Super15, entRaspLinie.Super16, entRaspLinie.Super17, entRaspLinie.Super18, entRaspLinie.Super19,
                                            entRaspLinie.Super20, entRaspLinie.USER_NO, DateTime.Now.ToString("yyyyMMdd HH:mm"), entRaspLinie.Descriere, entRaspLinie.TipData,
                                            entRaspLinie.TipValoare, entRaspLinie.Sublinia, entRaspLinie.Tinta, entRaspLinie.Super1_1, entRaspLinie.Super1_2,
                                            entRaspLinie.Super1_3, entRaspLinie.Super2_1, entRaspLinie.Super2_2, entRaspLinie.Super2_3, entRaspLinie.Super3_1,
                                            entRaspLinie.Super3_2, entRaspLinie.Super3_3, entRaspLinie.Super4_1, entRaspLinie.Super4_2, entRaspLinie.Super4_3,
                                            entRaspLinie.Super5_1, entRaspLinie.Super5_2, entRaspLinie.Super5_3, entRaspLinie.Super6_1, entRaspLinie.Super6_2,
                                            entRaspLinie.Super6_3, entRaspLinie.Super7_1, entRaspLinie.Super7_2, entRaspLinie.Super7_3, entRaspLinie.Super8_1,
                                            entRaspLinie.Super8_2, entRaspLinie.Super8_3, entRaspLinie.Super9_1, entRaspLinie.Super9_2, entRaspLinie.Super9_3,
                                            entRaspLinie.Super10_1, entRaspLinie.Super10_2, entRaspLinie.Super10_3, entRaspLinie.IdGrup, entRaspLinie.PondereRatingGlobal,
                                            entRaspLinie.NumeGrup, entRaspLinie.Super11_1, entRaspLinie.Super11_2, entRaspLinie.Super11_3, entRaspLinie.Super12_1,
                                            entRaspLinie.Super12_2, entRaspLinie.Super12_3, entRaspLinie.Super13_1, entRaspLinie.Super13_2, entRaspLinie.Super13_3,
                                            entRaspLinie.Super14_1, entRaspLinie.Super14_2, entRaspLinie.Super14_3, entRaspLinie.Super15_1, entRaspLinie.Super15_2,
                                            entRaspLinie.Super15_3, entRaspLinie.Super16_1, entRaspLinie.Super16_2, entRaspLinie.Super16_3, entRaspLinie.Super17_1,
                                            entRaspLinie.Super17_2, entRaspLinie.Super17_3, entRaspLinie.Super18_1, entRaspLinie.Super18_2, entRaspLinie.Super18_3,
                                            entRaspLinie.Super19_1, entRaspLinie.Super19_2, entRaspLinie.Super19_3, entRaspLinie.Super20_1, entRaspLinie.Super20_2,
                                            entRaspLinie.Super20_3, entRaspLinie.Super1_4, entRaspLinie.Super1_5, entRaspLinie.Super1_6, entRaspLinie.Super2_4,
                                            entRaspLinie.Super2_5, entRaspLinie.Super2_6, entRaspLinie.Super3_4, entRaspLinie.Super3_5, entRaspLinie.Super3_6,
                                            entRaspLinie.Super4_4, entRaspLinie.Super4_5, entRaspLinie.Super4_6, entRaspLinie.Super5_4, entRaspLinie.Super5_5,
                                            entRaspLinie.Super5_6, entRaspLinie.Super6_4, entRaspLinie.Super6_5, entRaspLinie.Super6_6, entRaspLinie.Super7_4,
                                            entRaspLinie.Super7_5, entRaspLinie.Super7_6, entRaspLinie.Super8_4, entRaspLinie.Super8_5, entRaspLinie.Super8_6,
                                            entRaspLinie.Super9_4, entRaspLinie.Super9_5, entRaspLinie.Super9_6, entRaspLinie.Super10_4, entRaspLinie.Super10_5,
                                            entRaspLinie.Super10_6, entRaspLinie.Super11_4, entRaspLinie.Super11_5, entRaspLinie.Super11_6, entRaspLinie.Super12_4,
                                            entRaspLinie.Super12_5, entRaspLinie.Super12_6, entRaspLinie.Super13_4, entRaspLinie.Super13_5, entRaspLinie.Super13_6,
                                            entRaspLinie.Super14_4, entRaspLinie.Super14_5, entRaspLinie.Super14_6, entRaspLinie.Super15_4, entRaspLinie.Super15_5,
                                            entRaspLinie.Super15_6, entRaspLinie.Super16_4, entRaspLinie.Super16_5, entRaspLinie.Super16_6, entRaspLinie.Super17_4,
                                            entRaspLinie.Super17_5, entRaspLinie.Super17_6, entRaspLinie.Super18_4, entRaspLinie.Super18_5, entRaspLinie.Super18_6,
                                            entRaspLinie.Super19_4, entRaspLinie.Super19_5, entRaspLinie.Super19_6, entRaspLinie.Super20_4, entRaspLinie.Super20_5,
                                            entRaspLinie.Super20_6, entRaspLinie.DescriereInRatingGlobal});
                    }
                    catch (Exception ex)
                    {
                        General.MemoreazaEroarea("1 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }

                //Florin 2019.01.29
                //int pozitie = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                int pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);
                
                //int idQuiz = Convert.ToInt32(_idQuiz);
                //int f10003 = Convert.ToInt32(_f10003);


                #region set scripts Obiective Individuale

                List<Eval_ObiIndividualeTemp> arrObi = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                if (arrObi != null && arrObi.Count != 0)
                {
                    List<Eval_ObiIndividualeTemp> lstObiIndividuale = arrObi.Where(p => p.IdQuiz == _idQuiz && p.F10003 == _f10003 && p.Pozitie == pozitie).ToList();

                    if (lstObiIndividuale != null && lstObiIndividuale.Count != 0)
                    {
                        string sqlDeleteObiIndividuale = @"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdAuto"" = @19;";
                        string sqlInsertObiIndividuale = $@"insert into ""Eval_ObiIndividualeTemp""
                                (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", 
                                ""Pondere"", ""Descriere"", ""Target"", ""Realizat"", ""IdCalificativ"",
                                ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", ""F10003"", ""Pozitie"",
                                ""Id"", ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""IdUnic"", USER_NO, TIME)
                                values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@idUnic,@20,@21);";

                        string tgv = "";

                        foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
                        {
                            try
                            {
                                string sqlDel = sqlDeleteObiIndividuale;
                                string sqlIns = sqlInsertObiIndividuale;
                                string sqlObi =
                                    "BEGIN " + Environment.NewLine +
                                        sqlDel + Environment.NewLine +
                                        sqlIns + Environment.NewLine +
                                    "END;";
                                
                                sqlObi = sqlObi.Replace("@idUnic", clsObiIndividuale.IdUnic <= 0 ? "NEXT VALUE FOR ObiIndividuale_SEQ" : clsObiIndividuale.IdUnic.ToString());
                                tgv += sqlObi + Environment.NewLine;
                                General.ExecutaNonQuery(sqlObi, new object[] { clsObiIndividuale.IdObiectiv, clsObiIndividuale.Obiectiv, clsObiIndividuale.IdActivitate, General.Nz(clsObiIndividuale.Activitate, "").ToString().Replace(",", "."),
                                                                            General.Nz(clsObiIndividuale.Pondere, "0").ToString().Replace(",", "."), clsObiIndividuale.Descriere, General.Nz(clsObiIndividuale.Target, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.Realizat, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.IdCalificativ, "0").ToString().Replace(",", "."),
                                                                            clsObiIndividuale.Calificativ, clsObiIndividuale.ExplicatiiCalificativ, clsObiIndividuale.IdQuiz, clsObiIndividuale.F10003, clsObiIndividuale.Pozitie,
                                                                            clsObiIndividuale.Id, clsObiIndividuale.IdLinieQuiz, clsObiIndividuale.ColoanaSuplimentara1, clsObiIndividuale.ColoanaSuplimentara2, clsObiIndividuale.IdAuto, General.Nz(clsObiIndividuale.USER_NO, Session["UserId"]), General.Nz(clsObiIndividuale.TIME, DateTime.Now) });
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea("3 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                        }
                    }
                }
                #endregion


                #region set scripts Competente Angajat

                List<Eval_CompetenteAngajatTemp> arrComp = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                if (arrComp != null && arrComp.Count != 0)
                {
                    List<Eval_CompetenteAngajatTemp> lstCompAngajat = arrComp.Where(p => p.IdQuiz == _idQuiz && p.F10003 == _f10003 && p.Pozitie == pozitie).ToList();
                    if (lstCompAngajat != null && lstCompAngajat.Count != 0)
                    {
                        string sqlDeleteCompAngajat = @"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdAuto""=@15;";
                        string sqlInsertCompAngajat = @"insert into ""Eval_CompetenteAngajatTemp""(""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"",
                                                                                      ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", 
                                                                                      ""Explicatii"", ""IdQuiz"", ""F10003"", ""Pozitie"", ""Id"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME)
                                                                                        values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@idUnic,@16,@17)";

                        string tgv = "";

                        foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
                        {
                            try
                            {
                                string sqlDel = sqlDeleteCompAngajat;
                                string sqlIns = sqlInsertCompAngajat;
                                string sqlCmp =
                                    "BEGIN " + Environment.NewLine +
                                        sqlDel + Environment.NewLine +
                                        sqlIns + Environment.NewLine +
                                    "END;";

                                sqlCmp = sqlCmp.Replace("@idUnic", clsCompetenta.IdUnic <= 0 ? "NEXT VALUE FOR CompetenteAng_SEQ" : clsCompetenta.IdUnic.ToString());
                                tgv += sqlCmp + Environment.NewLine;
                                General.ExecutaNonQuery(sqlCmp, new object[] { clsCompetenta.IdCategCompetenta, clsCompetenta.CategCompetenta, clsCompetenta.IdCompetenta, clsCompetenta.Competenta,
                                                                        General.Nz(clsCompetenta.Pondere, "0").ToString().Replace(",", "."), clsCompetenta.IdCalificativ, clsCompetenta.Calificativ, clsCompetenta.ExplicatiiCalificativ,
                                                                        clsCompetenta.Explicatii, clsCompetenta.IdQuiz, clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.Id, clsCompetenta.IdLinieQuiz, clsCompetenta.IdAuto, General.Nz(clsCompetenta.USER_NO, Session["UserId"]), General.Nz(clsCompetenta.TIME, DateTime.Now) });
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea("4 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                        }
                    }
                }
                #endregion

                //#region set scripts Competente Angajat

                //List<Eval_CompetenteAngajatTemp> arrComp = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                //if (arrComp != null && arrComp.Count != 0)
                //{
                //    List<Eval_CompetenteAngajatTemp> lstCompAngajat = arrComp.Where(p => p.IdQuiz == idQuiz && p.F10003 == f10003 && p.Pozitie == pozitie).ToList();
                //    if (lstCompAngajat != null && lstCompAngajat.Count != 0)
                //    {
                //        string sqlDeleteCompAngajat, sqlDeleteCompAngajatTemp = string.Empty;
                //        string sqlInsertCompAngajat, sqlInsertCompAngajatTemp = string.Empty;

                //        sqlDeleteCompAngajat = @"delete from ""Eval_CompetenteAngajatTemp"" where ""F10003"" = @1 and ""Pozitie"" = @2 and ""IdQuiz"" = @3 and ""IdLinieQuiz"" = @4";
                //        sqlInsertCompAngajat = @"insert into ""Eval_CompetenteAngajatTemp""(""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"",
                //                                                                      ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", 
                //                                                                      ""Explicatii"", ""IdQuiz"", ""F10003"", ""Pozitie"", ""Id"", ""IdAuto"", ""IdLinieQuiz"")
                //                                                                        values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15)";

                //        foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
                //        {
                //            try
                //            {
                //                sqlDeleteCompAngajatTemp = sqlDeleteCompAngajat;
                //                General.ExecutaNonQuery(sqlDeleteCompAngajatTemp, new object[] { clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.IdQuiz, clsCompetenta.IdLinieQuiz });
                //            }
                //            catch (Exception ex)
                //            {
                //                General.MemoreazaEroarea("4 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                //            }
                //        }
                //        foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
                //        {
                //            try
                //            {
                //                sqlInsertCompAngajatTemp = sqlInsertCompAngajat;
                //                General.ExecutaNonQuery(sqlInsertCompAngajatTemp, new object[] { clsCompetenta.IdCategCompetenta, clsCompetenta.CategCompetenta, clsCompetenta.IdCompetenta, clsCompetenta.Competenta,
                //                                                        General.Nz(clsCompetenta.Pondere, "0").ToString().Replace(",", "."), clsCompetenta.IdCalificativ, clsCompetenta.Calificativ, clsCompetenta.ExplicatiiCalificativ,
                //                                                        clsCompetenta.Explicatii, clsCompetenta.IdQuiz, clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.Id, clsCompetenta.IdAuto, clsCompetenta.IdLinieQuiz });
                //            }
                //            catch (Exception ex)
                //            {
                //                General.MemoreazaEroarea("5 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                //            }
                //        }
                //    }
                //}
                //#endregion


                if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1" && idCateg == "0")
                    PreluareDateAutomat(_idQuiz, _f10003, pozitie);

                MessageBox.Show("Proces realizat cu succes!", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //General.MemoreazaEroarea("Intrat");
        //        string sqlCommandDelete = string.Empty;
        //        string sqlCommandDeleteTemp = string.Empty;
        //        string sqlCommandInsert = string.Empty;
        //        string sqlCommandInsertTemp = string.Empty;

        //        #region Set Scripts Upload DB
        //        sqlCommandDelete = @"delete from ""Eval_RaspunsLinii"" where ""IdQuiz"" = {0} and ""F10003"" = {1} and ""Id"" = {2}";
        //        sqlCommandInsert = @"insert into ""Eval_RaspunsLinii""(""IdQuiz"", ""F10003"", ""Id"", ""Linia"", 
        //                                        ""Super1"",""Super2"",""Super3"",""Super4"",""Super5"",
        //                                        ""Super6"",""Super7"",""Super8"",""Super9"",""Super10"",
        //                                        ""Super11"",""Super12"",""Super13"",""Super14"",""Super15"",
        //                                        ""Super16"",""Super17"",""Super18"",""Super19"",""Super20"",
        //                                        ""USER_NO"",""TIME"",""Descriere"",""TipData"",""TipValoare"",
        //                                        ""Sublinia"",""Tinta"",""Super1_1"",""Super1_2"",""Super1_3"",
        //                                        ""Super2_1"",""Super2_2"",""Super2_3"",""Super3_1"",""Super3_2"",
        //                                        ""Super3_3"",""Super4_1"",""Super4_2"",""Super4_3"",""Super5_1"",
        //                                        ""Super5_2"",""Super5_3"",""Super6_1"",""Super6_2"",""Super6_3"",
        //                                        ""Super7_1"",""Super7_2"",""Super7_3"",""Super8_1"",""Super8_2"",
        //                                        ""Super8_3"",""Super9_1"",""Super9_2"",""Super9_3"",""Super10_1"",
        //                                        ""Super10_2"",""Super10_3"",""IdGrup"",""PondereRatingGlobal"",""NumeGrup"",
        //                                        ""Super11_1"",""Super11_2"",""Super11_3"",""Super12_1"",""Super12_2"",
        //                                        ""Super12_3"",""Super13_1"",""Super13_2"",""Super13_3"",""Super14_1"",
        //                                        ""Super14_2"",""Super14_3"",""Super15_1"",""Super15_2"",""Super15_3"",
        //                                        ""Super16_1"",""Super16_2"",""Super16_3"",""Super17_1"",""Super17_2"",
        //                                        ""Super17_3"",""Super18_1"",""Super18_2"",""Super18_3"",""Super19_1"",
        //                                        ""Super19_2"",""Super19_3"",""Super20_1"",""Super20_2"",""Super20_3"",
        //                                        ""Super1_4"",""Super1_5"",""Super1_6"",""Super2_4"",""Super2_5"",
        //                                        ""Super2_6"",""Super3_4"",""Super3_5"",""Super3_6"",""Super4_4"",
        //                                        ""Super4_5"",""Super4_6"",""Super5_4"",""Super5_5"",""Super5_6"",
        //                                        ""Super6_4"",""Super6_5"",""Super6_6"",""Super7_4"",""Super7_5"",
        //                                        ""Super7_6"",""Super8_4"",""Super8_5"",""Super8_6"",""Super9_4"",
        //                                        ""Super9_5"",""Super9_6"",""Super10_4"",""Super10_5"",""Super10_6"",
        //                                        ""Super11_4"",""Super11_5"",""Super11_6"",""Super12_4"",""Super12_5"",
        //                                        ""Super12_6"",""Super13_4"",""Super13_5"",""Super13_6"",""Super14_4"",
        //                                        ""Super14_5"",""Super14_6"",""Super15_4"",""Super15_5"",""Super15_6"",
        //                                        ""Super16_4"",""Super16_5"",""Super16_6"",""Super17_4"",""Super17_5"",
        //                                        ""Super17_6"",""Super18_4"",""Super18_5"",""Super18_6"",""Super19_4"",
        //                                        ""Super19_5"",""Super19_6"",""Super20_4"",""Super20_5"",""Super20_6"",
        //                                        ""DescriereInRatingGlobal"")
        //                          values({0}, {1}, {2}, {3}, '{4}', '{5}', '{6}',
        //                                 '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}',
        //                                 '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}',
        //                                 '{21}', '{22}', '{23}', {24}, '{25}', '{26}', {27},
        //                                 {28}, {29}, '{30}', '{31}', '{32}', '{33}', '{34}',
        //                                 '{35}', '{36}', '{37}', '{38}', '{39}', '{40}', '{41}',
        //                                 '{42}', '{43}', '{44}', '{45}', '{46}', '{47}', '{48}',
        //                                 '{49}', '{50}', '{51}', '{52}', '{53}', '{54}', '{55}',
        //                                 '{56}', '{57}', '{58}', '{59}', '{60}', {61}, {62},
        //                                 '{63}', '{64}', '{65}', '{66}', '{67}', '{68}', '{69}',
        //                                 '{70}', '{71}', '{72}', '{73}', '{74}', '{75}', '{76}',
        //                                 '{77}', '{78}', '{79}', '{80}', '{81}', '{82}', '{83}',
        //                                 '{84}', '{85}', '{86}', '{87}', '{88}', '{89}', '{90}',
        //                                 '{91}', '{92}', '{93}', '{94}', '{95}', '{96}', '{97}',
        //                                 '{98}', '{99}', '{100}', '{101}', '{102}', '{103}', '{104}',
        //                                 '{105}', '{106}', '{107}', '{108}', '{109}', '{110}', '{111}',
        //                                 '{112}', '{113}', '{114}', '{115}', '{116}', '{117}', '{118}',
        //                                 '{119}', '{120}', '{121}', '{122}', '{123}', '{124}', '{125}',
        //                                 '{126}', '{127}', '{128}', '{129}', '{130}', '{131}', '{132}',
        //                                 '{133}', '{134}', '{135}', '{136}', '{137}', '{138}', '{139}',
        //                                 '{140}', '{141}', '{142}', '{143}', '{144}', '{145}', '{146}',
        //                                 '{147}', '{148}', '{149}', '{150}', '{151}', '{152}', '{153}',
        //                                 '{154}');";
        //        #endregion

        //        lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;
        //        foreach (Eval_RaspunsLinii entRaspLinie in lstEval_RaspunsLinii)
        //        {
        //            try
        //            {
        //                sqlCommandDeleteTemp = sqlCommandDelete;
        //                sqlCommandDeleteTemp = string.Format(sqlCommandDeleteTemp, entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id);
        //                General.ExecutaNonQuery(sqlCommandDeleteTemp, null);

        //                sqlCommandInsertTemp = sqlCommandInsert;
        //                sqlCommandInsertTemp = string.Format(sqlCommandInsertTemp, entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id,
        //                                    entRaspLinie.Linia, entRaspLinie.Super1, entRaspLinie.Super2, entRaspLinie.Super3, entRaspLinie.Super4,
        //                                    entRaspLinie.Super5, entRaspLinie.Super6, entRaspLinie.Super7, entRaspLinie.Super8, entRaspLinie.Super9,
        //                                    entRaspLinie.Super10, entRaspLinie.Super11, entRaspLinie.Super12, entRaspLinie.Super13, entRaspLinie.Super14,
        //                                    entRaspLinie.Super15, entRaspLinie.Super16, entRaspLinie.Super17, entRaspLinie.Super18, entRaspLinie.Super19,
        //                                    entRaspLinie.Super20, entRaspLinie.USER_NO, DateTime.Now.ToString("yyyyMMdd HH:mm"), entRaspLinie.Descriere, entRaspLinie.TipData,
        //                                    entRaspLinie.TipValoare, entRaspLinie.Sublinia, entRaspLinie.Tinta, entRaspLinie.Super1_1, entRaspLinie.Super1_2,
        //                                    entRaspLinie.Super1_3, entRaspLinie.Super2_1, entRaspLinie.Super2_2, entRaspLinie.Super2_3, entRaspLinie.Super3_1,
        //                                    entRaspLinie.Super3_2, entRaspLinie.Super3_3, entRaspLinie.Super4_1, entRaspLinie.Super4_2, entRaspLinie.Super4_3,
        //                                    entRaspLinie.Super5_1, entRaspLinie.Super5_2, entRaspLinie.Super5_3, entRaspLinie.Super6_1, entRaspLinie.Super6_2,
        //                                    entRaspLinie.Super6_3, entRaspLinie.Super7_1, entRaspLinie.Super7_2, entRaspLinie.Super7_3, entRaspLinie.Super8_1,
        //                                    entRaspLinie.Super8_2, entRaspLinie.Super8_3, entRaspLinie.Super9_1, entRaspLinie.Super9_2, entRaspLinie.Super9_3,
        //                                    entRaspLinie.Super10_1, entRaspLinie.Super10_2, entRaspLinie.Super10_3, entRaspLinie.IdGrup, entRaspLinie.PondereRatingGlobal,
        //                                    entRaspLinie.NumeGrup, entRaspLinie.Super11_1, entRaspLinie.Super11_2, entRaspLinie.Super11_3, entRaspLinie.Super12_1,
        //                                    entRaspLinie.Super12_2, entRaspLinie.Super12_3, entRaspLinie.Super13_1, entRaspLinie.Super13_2, entRaspLinie.Super13_3,
        //                                    entRaspLinie.Super14_1, entRaspLinie.Super14_2, entRaspLinie.Super14_3, entRaspLinie.Super15_1, entRaspLinie.Super15_2,
        //                                    entRaspLinie.Super15_3, entRaspLinie.Super16_1, entRaspLinie.Super16_2, entRaspLinie.Super16_3, entRaspLinie.Super17_1,
        //                                    entRaspLinie.Super17_2, entRaspLinie.Super17_3, entRaspLinie.Super18_1, entRaspLinie.Super18_2, entRaspLinie.Super18_3,
        //                                    entRaspLinie.Super19_1, entRaspLinie.Super19_2, entRaspLinie.Super19_3, entRaspLinie.Super20_1, entRaspLinie.Super20_2,
        //                                    entRaspLinie.Super20_3, entRaspLinie.Super1_4, entRaspLinie.Super1_5, entRaspLinie.Super1_6, entRaspLinie.Super2_4,
        //                                    entRaspLinie.Super2_5, entRaspLinie.Super2_6, entRaspLinie.Super3_4, entRaspLinie.Super3_5, entRaspLinie.Super3_6,
        //                                    entRaspLinie.Super4_4, entRaspLinie.Super4_5, entRaspLinie.Super4_6, entRaspLinie.Super5_4, entRaspLinie.Super5_5,
        //                                    entRaspLinie.Super5_6, entRaspLinie.Super6_4, entRaspLinie.Super6_5, entRaspLinie.Super6_6, entRaspLinie.Super7_4,
        //                                    entRaspLinie.Super7_5, entRaspLinie.Super7_6, entRaspLinie.Super8_4, entRaspLinie.Super8_5, entRaspLinie.Super8_6,
        //                                    entRaspLinie.Super9_4, entRaspLinie.Super9_5, entRaspLinie.Super9_6, entRaspLinie.Super10_4, entRaspLinie.Super10_5,
        //                                    entRaspLinie.Super10_6, entRaspLinie.Super11_4, entRaspLinie.Super11_5, entRaspLinie.Super11_6, entRaspLinie.Super12_4,
        //                                    entRaspLinie.Super12_5, entRaspLinie.Super12_6, entRaspLinie.Super13_4, entRaspLinie.Super13_5, entRaspLinie.Super13_6,
        //                                    entRaspLinie.Super14_4, entRaspLinie.Super14_5, entRaspLinie.Super14_6, entRaspLinie.Super15_4, entRaspLinie.Super15_5,
        //                                    entRaspLinie.Super15_6, entRaspLinie.Super16_4, entRaspLinie.Super16_5, entRaspLinie.Super16_6, entRaspLinie.Super17_4,
        //                                    entRaspLinie.Super17_5, entRaspLinie.Super17_6, entRaspLinie.Super18_4, entRaspLinie.Super18_5, entRaspLinie.Super18_6,
        //                                    entRaspLinie.Super19_4, entRaspLinie.Super19_5, entRaspLinie.Super19_6, entRaspLinie.Super20_4, entRaspLinie.Super20_5,
        //                                    entRaspLinie.Super20_6, entRaspLinie.DescriereInRatingGlobal);
        //                General.ExecutaNonQuery(sqlCommandInsertTemp, null);
        //            }
        //            catch (Exception ex)
        //            {
        //                General.MemoreazaEroarea("1 - " +  ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //            }
        //        }


        //        int pozitie = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
        //        int idQuiz = Convert.ToInt32(_idQuiz);
        //        int f10003 = Convert.ToInt32(_f10003);


        //        #region set scripts Obiective Individuale

        //        //Florin 2019.01.09
        //        List<Eval_ObiIndividualeTemp> arrObi = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
        //        if (arrObi != null && arrObi.Count != 0)
        //        {
        //            List<Eval_ObiIndividualeTemp> lstObiIndividuale = arrObi.Where(p => p.IdQuiz == idQuiz && p.F10003 == f10003 && p.Pozitie == pozitie).ToList();

        //            if (lstObiIndividuale != null && lstObiIndividuale.Count != 0)
        //            {
        //                string sqlDeleteObiIndividuale, sqlDeleteObiIndividualeTemp = string.Empty;
        //                string sqlInsertObiIndividuale, sqlInsertObiIndividualeTemp = string.Empty;

        //                sqlDeleteObiIndividuale = @"delete from ""Eval_ObiIndividualeTemp"" where ""F10003"" = {0} and ""Pozitie"" = {1} and ""IdQuiz"" = {2} and ""IdLinieQuiz"" = {3}";
        //                //sqlInsertObiIndividuale = @"insert into ""Eval_ObiIndividualeTemp""
        //                //        (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", 
        //                //        ""Pondere"", ""Descriere"", ""Target"", ""Realizat"", ""IdCalificativ"",
        //                //        ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", ""F10003"", ""Pozitie"",
        //                //        ""Id"", ""IdAuto"", ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"" )
        //                //                        values({0}, N'{1}', {2}, N'{3}',
        //                //                               {4}, N'{5}', {6}, {7}, {8},
        //                //                               N'{9}', N'{10}', {11}, {12}, {13},
        //                //                               {14}, {15}, {16}, N'{17}', N'{18}')";

        //                sqlInsertObiIndividuale = @"insert into ""Eval_ObiIndividualeTemp""
        //                        (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", 
        //                        ""Pondere"", ""Descriere"", ""Target"", ""Realizat"", ""IdCalificativ"",
        //                        ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", ""F10003"", ""Pozitie"",
        //                        ""Id"", ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"" )
        //                                        values({0}, N'{1}', {2}, N'{3}',
        //                                               {4}, N'{5}', {6}, {7}, {8},
        //                                               N'{9}', N'{10}', {11}, {12}, {13},
        //                                               {14}, {16}, N'{17}', N'{18}')";


        //                foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
        //                {
        //                    try
        //                    {
        //                        sqlDeleteObiIndividualeTemp = sqlDeleteObiIndividuale;
        //                        sqlDeleteObiIndividualeTemp = string.Format(sqlDeleteObiIndividualeTemp, clsObiIndividuale.F10003, clsObiIndividuale.Pozitie, clsObiIndividuale.IdQuiz, clsObiIndividuale.IdLinieQuiz);
        //                        General.ExecutaNonQuery(sqlDeleteObiIndividualeTemp, null);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        General.MemoreazaEroarea("2 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //                    }
        //                }
        //                foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
        //                {
        //                    try
        //                    {
        //                        sqlInsertObiIndividualeTemp = sqlInsertObiIndividuale;
        //                        sqlInsertObiIndividualeTemp = string.Format(sqlInsertObiIndividualeTemp, clsObiIndividuale.IdObiectiv, clsObiIndividuale.Obiectiv, clsObiIndividuale.IdActivitate, General.Nz(clsObiIndividuale.Activitate, "").ToString().Replace(",", "."),
        //                                                                    General.Nz(clsObiIndividuale.Pondere, "0").ToString().Replace(",", "."), clsObiIndividuale.Descriere, General.Nz(clsObiIndividuale.Target, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.Realizat, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.IdCalificativ, "0").ToString().Replace(",", "."),
        //                                                                    clsObiIndividuale.Calificativ, clsObiIndividuale.ExplicatiiCalificativ, clsObiIndividuale.IdQuiz, clsObiIndividuale.F10003, clsObiIndividuale.Pozitie,
        //                                                                    clsObiIndividuale.Id, clsObiIndividuale.IdAuto, clsObiIndividuale.IdLinieQuiz, clsObiIndividuale.ColoanaSuplimentara1, clsObiIndividuale.ColoanaSuplimentara2);
        //                        General.ExecutaNonQuery(sqlInsertObiIndividualeTemp, null);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        General.MemoreazaEroarea("3 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //                    }
        //                }
        //            }
        //        }
        //        #endregion

        //        #region set scripts Competente Angajat

        //        //Florin 2019.01.09
        //        List<Eval_CompetenteAngajatTemp> arrComp = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
        //        if (arrComp != null && arrComp.Count != 0)
        //        {
        //            List<Eval_CompetenteAngajatTemp> lstCompAngajat = arrComp.Where(p => p.IdQuiz == idQuiz && p.F10003 == f10003 && p.Pozitie == pozitie).ToList();
        //            //List<Eval_CompetenteAngajatTemp> lstCompAngajat = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
        //            if (lstCompAngajat != null && lstCompAngajat.Count != 0)
        //            {
        //                string sqlDeleteCompAngajat, sqlDeleteCompAngajatTemp = string.Empty;
        //                string sqlInsertCompAngajat, sqlInsertCompAngajatTemp = string.Empty;

        //                sqlDeleteCompAngajat = @"delete from ""Eval_CompetenteAngajatTemp"" where ""F10003"" = {0} and ""Pozitie"" = {1} and ""IdQuiz"" = {2} and ""IdLinieQuiz"" = {3}";
        //                sqlInsertCompAngajat = @"insert into ""Eval_CompetenteAngajatTemp""(""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"",
        //                                                                              ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", 
        //                                                                              ""Explicatii"", ""IdQuiz"", ""F10003"", ""Pozitie"", ""Id"", ""IdAuto"", ""IdLinieQuiz"")
        //                                    values({0}, '{1}', {2}, '{3}',
        //                                           {4}, {5}, '{6}', '{7}',
        //                                           '{8}', {9}, {10}, {11}, {12}, {13}, {14})";
        //                //Eval_CompetenteAngajatTemp clsFirst = lstCompAngajat.FirstOrDefault();


        //                foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
        //                {
        //                    try
        //                    {
        //                        sqlDeleteCompAngajatTemp = sqlDeleteCompAngajat;
        //                        sqlDeleteCompAngajatTemp = string.Format(sqlDeleteCompAngajatTemp, clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.IdQuiz, clsCompetenta.IdLinieQuiz);
        //                        General.ExecutaNonQuery(sqlDeleteCompAngajatTemp, null);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        General.MemoreazaEroarea("4 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //                    }
        //                }
        //                foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
        //                {
        //                    try
        //                    { 
        //                        sqlInsertCompAngajatTemp = sqlInsertCompAngajat;
        //                        sqlInsertCompAngajatTemp = string.Format(sqlInsertCompAngajatTemp, clsCompetenta.IdCategCompetenta, clsCompetenta.CategCompetenta, clsCompetenta.IdCompetenta, clsCompetenta.Competenta,
        //                                                                General.Nz(clsCompetenta.Pondere, "0").ToString().Replace(",", "."), clsCompetenta.IdCalificativ, clsCompetenta.Calificativ, clsCompetenta.ExplicatiiCalificativ,
        //                                                                clsCompetenta.Explicatii, clsCompetenta.IdQuiz, clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.Id, clsCompetenta.IdAuto, clsCompetenta.IdLinieQuiz);
        //                        General.ExecutaNonQuery(sqlInsertCompAngajatTemp, null);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        General.MemoreazaEroarea("5 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //                    }
        //                }
        //            }
        //        }
        //        #endregion


        //        //Florin 2019.01.08
        //        if (Convert.ToInt32(_idClient) == 20 && pozitie == 1 && idCateg == "0")
        //            ActualizareComentarii(idQuiz, f10003, pozitie);


        //        ////Radu 03.07.2017 - se doreste ca, la salvarea datelor, comentariile sa se salveze pe paginile evaluatului si evaluatorului (doar la PeliFilip)
        //        //int pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"]);
        //        //if (Convert.ToInt32(_idClient) == 20 && pozitie == 1)
        //        //{
        //        //    int idQuiz = Convert.ToInt32(_idQuiz);
        //        //    int f10003 = Convert.ToInt32(_f10003);
        //        //    string sql = "SELECT ""Pozitie"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {0} and F10003 = {1} AND ""IdUser"" = {2}";
        //        //    sql = string.Format(sql, idQuiz, f10003, Session["UserId"].ToString());
        //        //    DataTable dt = General.IncarcaDT(sql, null);
        //        //    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
        //        //    {
        //        //        if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
        //        //                ActualizareComentarii(idQuiz, f10003, 1);
        //        //        if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 2)
        //        //            ActualizareComentarii(idQuiz, f10003, 2);
        //        //    }
        //        //}

        //        MessageBox.Show("Proces realizat cu succes!", MessageBox.icoSuccess);
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
        //    }
        //}

        #region Sectiuni Creare + Focus


        private void CreazaMeniu()
        {
            try
            {
                int i = 0;
                int idRoot = -99;
                var entRoot = lstEval_QuizIntrebari.Where(p => p.Parinte == 0 && p.Descriere == "Root").FirstOrDefault();
                if (entRoot != null) idRoot = entRoot.Id;

                var entSec = lstEval_QuizIntrebari.Where(p => p.Parinte == idRoot && p.EsteSectiune == 1).OrderBy(p => p.Id);

                foreach (var ent in entSec)
                {

                    try
                    {
                        i += 1;
                        if (!IsPostBack)
                            totalSec += 1;

                        HtmlGenericControl li = new HtmlGenericControl("li");

                        if (hf.Contains("IdSec") && hf["IdSec"].ToString() != "" && General.IsNumeric(hf["IdSec"]))
                        {
                            if (Convert.ToInt32(hf["IdSec"]) == i)
                            {
                                //li.Attributes["class"] = "active";
                                li.Attributes.Add("class", "active");
                            }
                        }

                        ASPxLabel txt = new ASPxLabel();
                        lstSec.Add(ent.Id);
                        txt.ID = "Sect" + i;
                        txt.Text = ent.Descriere;
                        txt.ClientSideEvents.Click = "function(s, e){ OnClickLabelSectiune(s); }";

                        li.Controls.Add(txt);
                        ulCtn.Controls.Add(li);

                        //lstSectiuni.Add(cell1);
                    }
                    catch (Exception ex)
                    {
                        General.MemoreazaEroarea("1 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }

                if (totalSec >= 0 && !IsPostBack)
                    indexSec = 0;

                txtNrSectiune.Text = (indexSec + 1).ToString();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //private void CreazaMeniu()
        //{
        //    try
        //    {
        //        int i = 0;
        //        int idRoot = -99;
        //        var entRoot = lstEval_QuizIntrebari.Where(p => p.Parinte == 0 && p.Descriere == "Root").FirstOrDefault();
        //        if (entRoot != null) idRoot = entRoot.Id;

        //        var entSec = lstEval_QuizIntrebari.Where(p => p.Parinte == idRoot && p.EsteSectiune == 1).OrderBy(p => p.Id);

        //        foreach (var ent in entSec)
        //        {
        //            i += 1;
        //            if (!IsPostBack)
        //                totalSec += 1;

        //            HtmlGenericControl div = new HtmlGenericControl();
        //            div.ID = "Div" + i;
        //            div.Style["text-align"] = "center";
        //            div.Style["vertical-align"] = "middle";

        //            ASPxLabel txt = new ASPxLabel();
        //            lstSec.Add(ent.Id);
        //            txt.ID = "Sect" + i;
        //            txt.Wrap = DevExpress.Utils.DefaultBoolean.True;
        //            txt.Text = ent.Descriere;
        //            txt.ForeColor = System.Drawing.Color.White;
        //            txt.ClientSideEvents.Click = "function(s, e){ OnClickLabelSectiune(s); }";

        //            HtmlTableCell cell1 = new HtmlTableCell();
        //            if ((i == 1 && !IsPostBack) || (IsPostBack && i == indexSec + 1))
        //            {
        //                txt.ForeColor = System.Drawing.Color.White;
        //                txt.Font.Bold = true;
        //                txt.Font.Underline = true;

        //                poligonPriLabels.Cells.Add(CreeazaForma(tipSectiune.SageataAlbaPrima, txt, div));
        //            }
        //            else
        //            {
        //                if (i != entSec.Count())
        //                    poligonPriLabels.Cells.Add(CreeazaForma(tipSectiune.SageataAlbaIntermediara, txt, div));
        //                else
        //                    poligonPriLabels.Cells.Add(CreeazaForma(tipSectiune.SageataAlbastraUltima, txt, div));
        //            }
        //            //lstSectiuni.Add(cell1);


        //            HtmlGenericControl divIntre = new HtmlGenericControl();
        //            divIntre.ID = "DivIntre" + i;
        //            divIntre.Style["text-align"] = "center";
        //            divIntre.Style["vertical-align"] = "middle";

        //            ASPxLabel lblIntre = new ASPxLabel();
        //            lblIntre.ID = "lblIntreSect" + i;
        //            lblIntre.Text = "";
        //            HtmlTableCell cellNew = new HtmlTableCell();
        //            cellNew.Align = "right";
        //            cellNew.RowSpan = 1;
        //            cellNew.ColSpan = 1;
        //            cellNew.Width = "20";
        //            cellNew.Height = "20";
        //            divIntre.Controls.Add(lblIntre);
        //            cellNew.Controls.Add(divIntre);
        //            poligonPriLabels.Cells.Add(cellNew);
        //        }

        //        if (totalSec >= 0 && !IsPostBack)
        //            indexSec = 0;

        //        txtNrSectiune.Text = (indexSec + 1).ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        private HtmlTableCell CreeazaForma(tipSectiune entSectiune, ASPxLabel lbl, HtmlGenericControl div)
        {
            HtmlTableCell cell = new HtmlTableCell();
            cell.Align = "right";
            cell.RowSpan = 1;
            cell.ColSpan = 1;
            cell.Width = "150";
            cell.Height = "11";
            try
            {
                switch (entSectiune)
                {
                    case tipSectiune.SageataAlbaIntermediara:
                        cell.Style["border-style"] = "solid";
                        cell.Style["border-color"] = "lightseagreen";
                        cell.Style["background-color"] = "lightseagreen";
                        break;
                    case tipSectiune.SageataAlbaPrima:
                        cell.Style["text-emphasis-color"] = "aqua";
                        cell.Style["border-style"] = "solid";
                        cell.Style["border-color"] = "lightseagreen";
                        cell.Style["background-color"] = "lightseagreen";
                        break;
                    case tipSectiune.SageataAlbastraUltima:
                        cell.Style["border-style"] = "solid";
                        cell.Style["border-color"] = "lightseagreen";
                        cell.Style["background-color"] = "lightseagreen";
                        break;
                }
                div.Controls.Add(lbl);
                cell.Controls.Add(div);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return cell;
        }

        private void CreazaTab(List<metaEvalDenumireSuper> lstSuper)
        {
            try
            {
                foreach (var ent in lstSuper)
                {
                    Tab tabPage = new Tab();
                    tabPage.Name = "tab" + ent.Pozitie.ToString();
                    tabPage.Text = ent.Denumire;
                    tabPage.Visible = true;
                    tabSuper.Tabs.Add(tabPage);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlSectiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                if (e.Parameter.Contains(";"))
                {
                    string[] param = e.Parameter.Split(';');
                    string nameControl = param[0];
                    string valueControl = param[1].Replace("'"," ").Replace("\""," ").Replace("\\"," ").Replace("/"," ");

                    //if (valueControl.ToUpper() == "TRUE" && nameControl.ToUpper().Contains("CHK"))
                    //    valueControl = "1";
                    //if (valueControl.ToUpper() == "FALSE" && nameControl.ToUpper().Contains("CHK"))
                    //    valueControl = "0";

                    //Radu 17.07.2018
                    if (nameControl == "btnLuatCunostinta")
                    {
                        lstEval_Raspuns = Session["lstEval_Raspuns"] as List<Eval_Raspuns>;
                        if (valueControl == "1")
                            lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta = 1;
                        else
                            lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta = 2;
                        Session["lstEval_Raspuns"] = lstEval_Raspuns;

                        //string sql = "UPDATE ""Eval_Raspuns"" SET ""LuatLaCunostinta"" = " + lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta + " WHERE ""IdQuiz""=" 
                        //    + lstEval_Raspuns.FirstOrDefault().IdQuiz + "AND F10003 = " + lstEval_Raspuns.FirstOrDefault().F10003;
                        General.ExecutaNonQuery(@"UPDATE ""Eval_Raspuns"" SET ""LuatLaCunostinta"" = @1 WHERE ""IdQuiz""=@2 AND F10003 = @3", new object[] { lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta, lstEval_Raspuns.FirstOrDefault().IdQuiz, lstEval_Raspuns.FirstOrDefault().F10003 });
                        pnlSectiune.JSProperties["cpAlertMessage"] = "Proces realizat cu succes!";
                        return;
                    }

                    nameControl = nameControl.Substring(nameControl.IndexOf("_WXY_"));
                    nameControl = nameControl.Replace("_WXY_", "");

                    if (lstEval_RaspunsLinii.Count == 0)
                        lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;

                    string super = "Super" + Session["CompletareChestionar_Pozitie"];
                    

                    Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == Convert.ToInt32(nameControl)).FirstOrDefault();
                    if (raspLinie != null)
                    {
                        if (Convert.ToInt32(_idClient) == 20)
                        {
                            if (raspLinie.TipData == 3 && raspLinie.TipValoare == 8)
                            {
                                //FLORIN 2018.12.03  - am rotunjit superior daca reziltatul este 2,75 => 3
                                //am adaugat si pozitia in filtru, pt a calcula doar pe obiectivele utilizatorului conectat
                                //int pozitie = Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"],-99));
                                //int idQuiz = Convert.ToInt32(General.Nz(_idQuiz,-99));
                                //int f10003 = Convert.ToInt32(_f10003);

                                //string sql = "select convert(INT,round(CONVERT(DECIMAL(18,2),sum(coalesce(""IdCalificativ"", 0))) / count(*),0)) from ""Eval_ObiIndividualeTemp"" where ""IdQuiz"" = {0} and F10003 = {1} AND ""Pozitie""={2} and ""IdLinieQuiz"" in  "
                                //            + "(select ""Id"" from ""Eval_QuizIntrebari"" where ""IdQuiz"" = {0} and ""TipData"" = 23)";
                                //sql = string.Format(sql, idQuiz, f10003, pozitie);
                                DataTable dt = General.IncarcaDT(
                                    @"select convert(INT,round(CONVERT(DECIMAL(18,2),sum(coalesce(""IdCalificativ"", 0))) / count(*),0)) 
                                    from ""Eval_ObiIndividualeTemp"" 
                                    where ""IdQuiz"" = @1 and F10003 = @2 AND ""Pozitie""=@3 
                                    and ""IdLinieQuiz"" in (select ""Id"" from ""Eval_QuizIntrebari"" where ""IdQuiz"" = @1 and ""TipData"" = 23)", new object[] { _idQuiz, _f10003, _pozitieQuiz });

                                //sql = "select * from ""Eval_tblTipValoriLinii"" where ""Id"" = 8 ORDER BY ""IdAuto""";
                                DataTable dtVal = General.IncarcaDT(@"select * from ""Eval_tblTipValoriLinii"" where ""Id"" = 8 ORDER BY ""IdAuto"" ", null);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    int i = 0;
                                    for (i = 0; i < dtVal.Rows.Count; i++)
                                    {
                                        if (dtVal.Rows[i]["Valoare"].ToString().Trim() == valueControl.Trim())
                                            break;
                                    }
                                
                                    if (i + 1 > Convert.ToInt32(dt.Rows[0][0].ToString()))
                                    {
                                        pnlSectiune.JSProperties["cpAlertMessage"] = "Calificativul ales este mai mare decat media calificativelor Obiectivelor Operationale. \n Va rugam sa alegeti alta optiune.";                                      
                                        return;
                                    }
                                }


                                //pt clientul Pelifilip - daca este calificativ final diferenta intre nota fiecarui obiectiv si calificativ final sa fie mai mica de 2 puncte, in caz contrar apare mesaj de atentionare, neblocant
                                //Sub asteptari
                                //In linie cu asteptarile
                                //Peste asteptari
                                if (_pozitieQuiz == 2)
                                {
                                    int valCtl = 1;
                                    if (valueControl.Trim().ToLower() == "sub asteptari") valCtl = 1;
                                    if (valueControl.Trim().ToLower() == "in linie cu asteptarile") valCtl = 2;
                                    if (valueControl.Trim().ToLower() == "peste asteptari") valCtl = 3;


                                    List<Eval_ObiIndividualeTemp> arrObi = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                                    List<Eval_ObiIndividualeTemp> lstObiIndividuale = arrObi.Where(p => p.IdQuiz == _idQuiz && p.F10003 == _f10003 && p.Pozitie == _pozitieQuiz).ToList();
                                    if (lstObiIndividuale != null && lstObiIndividuale.Count != 0)
                                    {
                                        foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
                                        {
                                            int dif = Convert.ToInt32(General.Nz(clsObiIndividuale.IdCalificativ, 1)) - valCtl;
                                            if (Math.Abs(dif) >= 2)
                                            {
                                                pnlSectiune.JSProperties["cpAlertMessage"] = "Exista o diferenta fata de calificativele acordate per obiectiv !";
                                                break;
                                            }
                                        }
                                    }

                                    //DataTable dtObi = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""=@3", new object[] { _idQuiz, _f10003, pozitie });
                                    //for(int j=0; j < dtObi.Rows.Count; j++)
                                    //{
                                    //    int dif = Convert.ToInt32(General.Nz(dtObi.Rows[j]["IdCalificativ"], 1)) - valCtl;
                                    //    if (Math.Abs(dif) >= 2)
                                    //    {
                                    //        pnlSectiune.JSProperties["cpAlertMessage"] = "Exista o diferenta fata de calificativele acordate per obiectiv !";
                                    //        break;
                                    //    }
                                    //}
                                }
                            }
                        }


                        
                        //Florin 2018.01.29
                        PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        ////Florin 2018.12.10
                        //PropertyInfo piValue = raspLinie.GetType().GetProperty(super);
                        //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                        //    piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());                            

                        if (piValue != null)
                        {
                            piValue.SetValue(raspLinie, valueControl, null);
                            Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;
                        }
                    }

                    if (param[0].Substring(0, 3) == "cmb")
                    {
                        ASPxComboBox cmb = pnlSectiune.FindControl(param[0]) as ASPxComboBox;
                        cmb.Value = (valueControl == "null" ? null : valueControl);
                    }

                    return;
                    
                }

                if (e.Parameter == "tabSuper")
                {
                    //Florin 2019.01.28
                    //Session["Eval_ActiveTab"] = tabSuper.ActiveTabIndex;
                    //CreeazaSectiune("Super" + (tabSuper.ActiveTabIndex + 1).ToString());    
                    Session["Eval_ActiveTab"] = tabSuper.ActiveTab.Name.Replace("tab","");
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"]);

                }

                if (e.Parameter == "btnFinalizare")
                    btnFinalizare_ItemClick(sender, e);


                if (e.Parameter == "btnInapoi" || e.Parameter == "btnInainte")
                {
                    totalSec = Convert.ToInt32(Session["totalSec"].ToString());
                    indexSec = Convert.ToInt32(hf["IdSec"]) - 1;
                    bool foundSecvParcurs = false;
                    bool foundSectiuneCurentaParcurs = false;
                    bool executaNextSectiune = false;
                    switch (e.Parameter)
                    {
                        case "btnInapoi":
                            if (indexSec > 0)
                                executaNextSectiune = true;

                            indexSec = indexSec - 1;

                            break;
                        case "btnInainte":
                            if (indexSec != totalSec - 1)
                                executaNextSectiune = true;

                            indexSec = indexSec + 1;
                            break;
                    }

                    if (!executaNextSectiune)
                        return;


                    //Florin 2018.03.08
                    //for (int i = 0; i <= totalSec - 1; i++)
                    //{
                    //    foundSecvParcurs = false;
                    //    foreach (HtmlTableCell cell in poligonPriLabels.Cells)
                    //    {
                    //        foreach (HtmlGenericControl div in cell.Controls)
                    //        {
                    //            if (!foundSectiuneCurentaParcurs)
                    //            {
                    //                ASPxLabel lblSectiuneCurent = div.FindControl("Sect" + (indexSec + 1).ToString()) as ASPxLabel;
                    //                if (lblSectiuneCurent != null)
                    //                {
                    //                    lblSectiuneCurent.ForeColor = System.Drawing.Color.White;
                    //                    lblSectiuneCurent.Font.Bold = true;
                    //                    lblSectiuneCurent.Font.Underline = true;
                    //                    foundSectiuneCurentaParcurs = true;
                    //                }
                    //            }
                    //            if (indexSec != i)
                    //            {
                    //                ASPxLabel lblSectiune = div.FindControl("Sect" + (i + 1).ToString()) as ASPxLabel;
                    //                if (lblSectiune != null)
                    //                {
                    //                    lblSectiune.ForeColor = System.Drawing.Color.White;
                    //                    lblSectiune.Font.Bold = false;
                    //                    lblSectiune.Font.Underline = false;
                    //                    foundSecvParcurs = true;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //        if (foundSecvParcurs)
                    //            break;
                    //    }
                    //}

                    txtNrSectiune.Text = (indexSec + 1).ToString();
                    Session["indexSec"] = indexSec;

                    //Florin 2019.01.28
                    ////CreeazaSectiune("Super" + Session["CompletareChestionar_Pozitie"]);
                    //CreeazaSectiune("Super" + (Convert.ToInt32(Session["Eval_ActiveTab"] ?? 0) + 1).ToString());
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());

                    return;
                }


                string sectiune = e.Parameter;
                if (sectiune.IndexOf("Sect") >= 0 && General.IsNumeric(sectiune.Replace("Sect", "")))
                    indexSec = Convert.ToInt32(sectiune.Replace("Sect", "")) - 1;


                //Florin 2018.03.08
                //bool found = false;
                //bool foundSectiuneCurenta = false;
                //for (int i = 0; i <= totalSec - 1; i++)
                //{
                //    found = false;
                //    foreach (HtmlTableCell cell in poligonPriLabels.Cells)
                //    {
                //        foreach (HtmlGenericControl div in cell.Controls)
                //        {
                //            if (!foundSectiuneCurenta)
                //            {
                //                ASPxLabel lblSectiuneCurent = div.FindControl("Sect" + (indexSec + 1).ToString()) as ASPxLabel;
                //                if (lblSectiuneCurent != null)
                //                {
                //                    lblSectiuneCurent.ForeColor = System.Drawing.Color.White;
                //                    lblSectiuneCurent.Font.Bold = true;
                //                    lblSectiuneCurent.Font.Underline = true;
                //                    foundSectiuneCurenta = true;
                //                }
                //            }
                //            if (indexSec != i)
                //            {
                //                ASPxLabel lblSectiune = div.FindControl("Sect" + (i + 1).ToString()) as ASPxLabel;
                //                if (lblSectiune != null)
                //                {
                //                    lblSectiune.ForeColor = System.Drawing.Color.White;
                //                    lblSectiune.Font.Bold = false;
                //                    lblSectiune.Font.Underline = false;
                //                    found = true;
                //                    break;
                //                }
                //            }
                //        }
                //        if (found)
                //            break;
                //    }

                //}
                txtNrSectiune.Text = (indexSec + 1).ToString();
                hf["IdSec"] = (indexSec + 1).ToString();

                Session["indexSec"] = indexSec;

                //Florin 2019.01.28
                ////CreeazaSectiune("Super" + Session["CompletareChestionar_Pozitie"]);
                //CreeazaSectiune("Super" + (Convert.ToInt32(Session["Eval_ActiveTab"] ?? 0) + 1).ToString());
                CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

        private void CreeazaSectiune(string super)
        {
            try
            {
                divIntrebari.Controls.Clear();
                grIntrebari = new HtmlTable();
                grIntrebari.CellSpacing = 12;
                grIntrebari.ID = "grIntrebari" + super;
                grIntrebari.Width = "100%";
                int idParinte = lstSec.ElementAt(indexSec);

                var arrIntre = lstEval_QuizIntrebari.Where(p => p.Ordine != null && p.Ordine.Contains("-" + idParinte + "-")).OrderBy(p => p.Id);
                if (arrIntre.Any())
                {
                    foreach (var ent in arrIntre)
                    {
                        try
                        {
                            CreeazaControl(ent, super);
                        }
                        catch (Exception ex)
                        {
                            General.MemoreazaEroarea("6 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                        }
                    }
                }

                divIntrebari.Controls.Add(grIntrebari);

                //int pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"] ?? -99);
                //int idQuiz = Convert.ToInt32(_idQuiz ?? -99);
                int blocat = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Blocat"",0)) FROM ""Eval_DrepturiTab"" WHERE ""IdQuiz"" = @1 AND ""Pozitie"" = @2 AND ""TabIndex"" = @3 ", new object[] { _idQuiz, super.Replace("Super",""), indexSec+1 }), 0));
                divIntrebari.Enabled = blocat == 1 ? false : true;
                
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void CreeazaControl(Eval_QuizIntrebari ent, string super)
        {
            try
            {
                //Florin 2019.01.29
                //Control ctl = null;
                dynamic ctl = null;

                if (ent.EsteSectiune == 1)
                {
                    ASPxLabel lbl = CreeazaTitlu(ent.Descriere, ent.Ordine);
                    if (lbl != null)
                    {
                        HtmlTableRow rwNew = new HtmlTableRow();
                        HtmlTableCell cellNew = new HtmlTableCell();
                        cellNew.ColSpan = 2;
                        cellNew.Controls.Add(lbl);
                        rwNew.Controls.Add(cellNew);
                        grIntrebari.Controls.Add(rwNew);
                    }
                }
                else
                {
                    switch (ent.TipData)
                    {
                        //TextSimplu
                        case 1:
                            ctl = CreeazaTextEdit(ent.Id, 1, super);
                            break;
                        case 2: //Lista derulanta
                            ctl = CreeazaCombo(ent.Id, (int)ent.TipValoare, super);
                            break;
                        case 3: //Butoane radio
                            ctl = CreeazaButoaneRadio(ent.Id, (int)ent.TipValoare, super);
                            break;
                        case 4: //CheckBox
                            ctl = CreeazaCheckBox(ent.Id, super);
                            break;
                        case 8: // TextMemo
                            ctl = CreeazaMemo(ent.Id, super);
                            break;
                        case 9: //Eticheta
                            ctl = CreeazaEticheta(ent.Descriere, ent.Id);
                            break;
                        case 10: //Data
                            ctl = CreeazaDateEdit(ent.Id, super);
                            break;
                        case 11: // Numeric
                            ctl = CreeazaTextEdit(ent.Id, 4, super);
                            break;
                        case 13: //NumeComplet
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 14: //Structura Organizatorica
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 15: // Post
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 17: //Tabel
                            ctl = CreeazaTabel(ent.Id, super);
                            break;
                        case 18: //Nume Manager
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 19: //Post Manager
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 20: //Nume Manager N + 2
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 21: // Post Manager N + 2
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 23: //Obiective individuale
                            ctl = CreeazaObiectiveIndividuale(ent.Id, 1, super);
                            break;
                        case 5: //Competente angajat
                            ctl = CreeazaCompetenteAngajat(ent.Id, 1, super);
                            break;
                        case 53: //tabel din view
                            ctl = CreazaTabel(ent.Id, ent.Parinte, "viewEvaluare360");
                            break;
                        case 54: //tabel din view
                            ctl = CreazaTabel(ent.Id, ent.Parinte, "viewEvaluareProiect");
                            break;
                        case 55: //tabel din view Others
                            ctl = CreazaTabelOthers();
                            break;
                    }

                    if (ctl != null)
                    {

                        if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) || 
                            Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                            ctl.Enabled = false;
                        //ctl.ReadOnly = true;
                        //ctl.Enabled = false;

                        if (ent.TipData == 9 || ent.TipData == 16 || ent.TipData == 24) //este eticheta sau rating global
                        {
                            HtmlTableRow row = new HtmlTableRow();
                            HtmlTableCell cell = new HtmlTableCell();
                            cell.ColSpan = 2;
                            cell.Controls.Add(ctl);
                            row.Cells.Add(cell);
                            grIntrebari.Rows.Add(row);                               
                        }
                        else
                        {
                            ASPxLabel lbl = CreeazaEticheta(ent.Descriere, ent.Id);
                            if (ent.Orientare == 1) //orientare orizontala
                            {
                                HtmlTableRow row = new HtmlTableRow();
                                HtmlTableCell cell1 = new HtmlTableCell();
                                cell1.Controls.Add(lbl);

                                HtmlTableCell cell2 = new HtmlTableCell();
                                cell2.Controls.Add(ctl);

                                row.Cells.Add(cell1);
                                row.Cells.Add(cell2);
                                grIntrebari.Rows.Add(row);
                            }
                            else
                            {
                                HtmlTableRow row1 = new HtmlTableRow();
                                HtmlTableRow row2 = new HtmlTableRow();

                                HtmlTableCell cell1 = new HtmlTableCell();
                                cell1.ColSpan = 2;
                                HtmlTableCell cell2 = new HtmlTableCell();
                                cell2.ColSpan = 2;

                                cell1.Controls.Add(lbl);
                                cell2.Controls.Add(ctl);

                                row1.Cells.Add(cell1);
                                row2.Cells.Add(cell2);

                                grIntrebari.Rows.Add(row1);
                                grIntrebari.Rows.Add(row2);                                                                
                            }
                        }

                        

                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #region Controls
        private ASPxLabel CreeazaTitlu(string desc, string ordine)
        {
            ASPxLabel lbl = new ASPxLabel();
            try
            {
                lbl.Text = desc;
                lbl.Wrap = DevExpress.Utils.DefaultBoolean.True;
                lbl.Font.Bold = true;

                char c = Convert.ToChar("-");
                int nr = ordine.ToCharArray().Where(p => p == c).Count();

                lbl.Font.Size = font - nr;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return lbl;
        }

        private ASPxGridView CreeazaTabel(int id, string super)
        {
            ASPxGridView gr = new ASPxGridView();
            try
            {
                #region GridProperties
                gr.Width = 1500;
                gr.ID = "gr" + "_WXY_" + id.ToString();
                gr.SettingsBehavior.AllowFocusedRow = true;
                gr.SettingsBehavior.EnableCustomizationWindow = true;
                gr.SettingsBehavior.AllowSelectByRowClick = true;
                gr.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
                gr.Settings.ShowFilterRow = true;
                gr.Settings.ShowGroupPanel = true;
                gr.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                gr.SettingsSearchPanel.Visible = true;
                gr.AutoGenerateColumns = false;

                gr.ClientSideEvents.ContextMenu = "ctx";
                gr.SettingsEditing.Mode = GridViewEditingMode.Inline;

                
                gr.CellEditorInitialize += Gr_CellEditorInitialize; ;
                gr.RowUpdating += Gr_RowUpdating; 
                gr.CustomErrorText += Gr_CustomErrorText; 
                #endregion


                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowEditButton = true;
                colCommand.VisibleIndex = 0;
                colCommand.Caption = " ";
                colCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                gr.Columns.Add(colCommand);

                 string[] arr = { "IdQuiz", "F10003", "Id", "Linia", "TipData", "Descriere", super, "USER_NO", "TIME" };

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                    col.FieldName = arr[i];
                    col.Name = Dami.TraduCuvant(arr[i]);
                    col.Caption = Dami.TraduCuvant(arr[i]);

                    if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 7 || i == 8)
                    {
                        col.Visible = false;
                    }

                    if (i == 5) col.ReadOnly = true;
                    if (i == 6)
                    {
                        col.Name = Dami.TraduCuvant("Raspuns");
                        col.Caption = Dami.TraduCuvant("Raspuns");
                    }
                    if (i == 6)                    
                        col.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                
                    gr.Columns.Add(col);
                }                
              

                #region Grid Command Buttons
                gr.SettingsCommandButton.EditButton.Image.Url = "../Fisiere/Imagini/Icoane/edit.png";
                gr.SettingsCommandButton.EditButton.Image.AlternateText = "Edit";
                gr.SettingsCommandButton.EditButton.Image.ToolTip = "Edit";

                gr.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                gr.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                gr.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                gr.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                gr.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                gr.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";
                #endregion
                
                gr.DataSource = lstEval_RaspunsLinii.Where(p => p.Id == id);
                gr.KeyFieldName = "IdQuiz; F10003; Id; Linia";
                gr.DataBind();

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        gr.Enabled = false;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        gr.Enabled = false;
                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    gr.Enabled = false;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        gr.Enabled = false;
                //}

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return gr;
        }

        private void Gr_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
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


        private void Gr_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                ASPxGridView grid = sender as ASPxGridView;                
                GridViewDataTextColumn col = grid.Columns[7] as GridViewDataTextColumn;

                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
                
                if (raspLinie != null)
                {
                    PropertyInfo piValue = raspLinie.GetType().GetProperty(col.FieldName);
                    if (piValue != null)
                    {
                        piValue.SetValue(raspLinie, Convert.ChangeType(e.NewValues[col.FieldName] ?? DBNull.Value, piValue.PropertyType), null);
                    }
                }

                e.Cancel = true;

                grid.CancelEdit();
                grid.DataSource = lstEval_RaspunsLinii.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void Gr_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                //ASPxGridView grid = sender as ASPxGridView;
                //if (grid.Columns.Count > 8)
                //{
                //    for (int i = 8; i < grid.Columns.Count; i++)
                //        grid.Columns[i].Visible = false; 
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        private ASPxTextBox CreeazaTextEdit(int id, int tip, string super, int tipData = 0)
        {
            //tip
            //1 - text simplu
            //2 - text memo
            //3 - data
            //4 - numeric

            //tipData
            //13 - NumeComplet
            //14 - Structura org.
            //15 - Post

            ASPxTextBox txt = new ASPxTextBox();            
            try
            {
                txt.Height = 21;
                txt.Width = 400;
                txt.HorizontalAlign = HorizontalAlign.Left;
                txt.ID = "txt" + "_WXY_" + id.ToString();
                txt.ClientSideEvents.TextChanged = "function(s, e){ OnTextChangedHandlerCtr(s); }";
                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        txt.Text = piValue.GetValue(raspLinie, null).ToString();
                    }
                }

                switch (tip)
                {
                    case 1:
                        //NOP
                        break;
                    case 2:
                        txt.Height = 150;
                        txt.Width = 900;                                                
                        break;
                    case 3:
                        txt.Width = 100;
                        txt.DisplayFormatString = "dd/MM/yyyy";
                        break;
                    case 4:
                        txt.Width = 100;
                        //txt.MaskSettings.Mask = "0..999g";
                        txt.DisplayFormatString = "N0";
                        break;
                }


                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        txt.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        txt.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    txt.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        txt.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return txt;
        }

        private ASPxMemo CreeazaMemo(int id, string super)
        {
            ASPxMemo txt = new ASPxMemo();
            try
            {
                txt.Height = 150;
                txt.Width = 900;
                txt.HorizontalAlign = HorizontalAlign.Left;
                txt.ID = "txt" + "_WXY_" + id.ToString();
                txt.ClientSideEvents.TextChanged = "function(s, e){ OnTextChangedHandlerCtr(s); }";
                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        txt.Text = piValue.GetValue(raspLinie, null).ToString();
                    }
                }

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        txt.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        txt.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    txt.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        txt.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return txt;
        }

        private ASPxComboBox CreeazaCombo(int id, int idGrupValori, string super)
        {
            ASPxComboBox cmb = new ASPxComboBox();
            try
            {
                cmb.Width = 250;
                cmb.HorizontalAlign = HorizontalAlign.Left;
                cmb.TextField = "Valoare";
                cmb.ValueField = "Valoare";

                lstEval_tblTipValoriLinii = Session["lstEval_tblTipValoriLinii"] as List<Eval_tblTipValoriLinii>;

                var lst = lstEval_tblTipValoriLinii.Where(p => p.Id == idGrupValori);
                if (lst.Count() > 0 && lst.FirstOrDefault() != null && General.Nz(lst.FirstOrDefault().Nota, "").ToString() != "") cmb.ValueField = "Nota";
                cmb.DataSource = lstEval_tblTipValoriLinii.Where(p => p.Id == idGrupValori);
                cmb.DataBind();
                cmb.ID = "cmb" + "_WXY_" + id.ToString();
                cmb.ClientSideEvents.ValueChanged = "function(s, e) { OnCMBTipChanged(s); }";
                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        cmb.Value = piValue.GetValue(raspLinie, null).ToString();
                    }
                }

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        cmb.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        cmb.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    cmb.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        cmb.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return cmb;
        }


        //Florin 2018.12.08
        //am recreat functia RadioButtons
        private ASPxRadioButtonList CreeazaButoaneRadio(int id, int idGrupValori, string super)
        {
            ASPxRadioButtonList radio = new ASPxRadioButtonList();
            try
            {
                radio.ID = "lstBox" + "_WXY_" + id.ToString();
                radio.ClientSideEvents.ValueChanged = "function(s, e) { OnCHKChanged(s); }";

                lstEval_tblTipValoriLinii = Session["lstEval_tblTipValoriLinii"] as List<Eval_tblTipValoriLinii>;
                var lst = lstEval_tblTipValoriLinii.Where(p => p.Id == idGrupValori);
                if (lst.Count() > 0 && lst.FirstOrDefault() != null && General.Nz(lst.FirstOrDefault().Nota, "").ToString() != "") radio.ValueField = "Nota";

                foreach (var l in lst)
                {
                    if (lst.Where(p => p.Nota.Equals(null) || p.Nota.Equals("")).Count() == 0)
                        radio.Items.Add(new ListEditItem { Text = l.Valoare, Value = l.Nota });
                    else
                        radio.Items.Add(new ListEditItem { Text = l.Valoare, Value = l.Valoare });
                }

                //Temporar - HardCodat
                //daca este calificativ final si este angajat nu are acces
                //675,677,681,685,687,691,695,699,703,707
                if (super.ToLower() == "super1" && "675,677,681,685,687,691,695,699,703,707,".IndexOf(id.ToString() + ",") >= 0)
                    radio.ReadOnly = true;

                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();

                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        radio.Value = piValue.GetValue(raspLinie, null).ToString();
                    }
                }

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        radio.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        radio.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    radio.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        radio.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return radio;
        }

        //private ASPxRadioButtonList CreeazaButoaneRadio(int id, int idGrupValori, string super)
        //{
        //    ASPxRadioButtonList radio = new ASPxRadioButtonList();
        //    try
        //    {
        //        radio.ValueField = "Valoare";
        //        radio.TextField = "Valoare";

        //        lstEval_tblTipValoriLinii = Session["lstEval_tblTipValoriLinii"] as List<Eval_tblTipValoriLinii>;

        //        var lst = lstEval_tblTipValoriLinii.Where(p => p.Id == idGrupValori);
        //        if (lst.Count() > 0 && lst.FirstOrDefault() != null && General.Nz(lst.FirstOrDefault().Nota, "").ToString() != "") radio.ValueField = "Nota";

        //        radio.DataSource = lstEval_tblTipValoriLinii.Where(p => p.Id == idGrupValori);
        //        radio.DataBind();

        //        radio.ID = "lstBox" + "_WXY_" + id.ToString();
        //        radio.ClientSideEvents.ValueChanged = "function(s, e) { OnCHKChanged(s); }";

        //        Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();

        //        if (raspLinie != null)
        //        {
        //            PropertyInfo piValue = raspLinie.GetType().GetProperty(super);
        //            if (piValue != null)
        //            {
        //                radio.Value = piValue.GetValue(raspLinie, null).ToString();
        //            }
        //        }

        //        int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

        //        if (idCateg == "0")
        //        {
        //            if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1 != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
        //                radio.ReadOnly = true;
        //        }
        //        else
        //        {
        //            if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
        //                radio.ReadOnly = true;
        //        }

        //        if (Convert.ToInt32(_idClient) == 21)
        //        {//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
        //            if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
        //                radio.ReadOnly = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //    return radio;
        //}


        private ASPxCheckBox CreeazaCheckBox(int id, string super)
        {
            ASPxCheckBox chk = new ASPxCheckBox();
            try
            {
                chk.ID = "chk" + "_WXY_" + id.ToString();
                chk.ClientSideEvents.CheckedChanged = "function(s, e) { OnCHKChanged(s); }";

                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        chk.Checked = (piValue.GetValue(raspLinie, null).ToString() == "1" || piValue.GetValue(raspLinie, null).ToString().ToUpper() == "TRUE" ? true : false);
                    }
                }

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        chk.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        chk.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    chk.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        chk.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return chk;
        }

        private ASPxLabel CreeazaEticheta(string valoare, int idCtl)
        {
            ASPxLabel lbl = new ASPxLabel();
            try
            {
                lbl.Text = valoare;
                lbl.Wrap = DevExpress.Utils.DefaultBoolean.True;
                lbl.ForeColor = Evaluare.CuloareBrush("#FF000099");
                //Florin 2018.06.25  cerere de la Pelifilip
                lbl.Font.Size = 12;
                lbl.ID = "lbl" + valoare.Replace(" ", "") + "_" + idCtl;
                lbl.CssClass = "lbl_eval_desc";

                //Radu 28.06.2018 - pot fi mai multe etichete fara text (cu valoare = "")
                if (lbl.ID == "lbl")
                {
                    if (Session["Eval_IdEticheta"] != null)
                    {
                        int id = Convert.ToInt32(Session["Eval_IdEticheta"].ToString());
                        id++;
                        lbl.ID = "lbl" + id;
                        Session["Eval_IdEticheta"] = id.ToString();
                    }
                    else
                    {
                        lbl.ID = "lbl1";
                        Session["Eval_IdEticheta"] = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return lbl;
        }

        private ASPxDateEdit CreeazaDateEdit(int id, string super)
        {
            ASPxDateEdit dt = new ASPxDateEdit();
            try
            {
                dt.Height = 21;
                dt.Width = 120;
                dt.HorizontalAlign = HorizontalAlign.Left;
                dt.DisplayFormatString = "dd/MM/yyyy";
                dt.UseMaskBehavior = true;
                dt.ID = "dt" + "_WXY_" + id.ToString();
                dt.ClientSideEvents.ValueChanged = "function(s, e) { OnDateChangedHandlerCtr(s); }";

                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    //Florin 2019.01.29
                    //PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_PozitieUserLogat"].ToString());
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        if (General.Nz(piValue.GetValue(raspLinie, null),"").ToString() != "")
                        {
                            try
                            {
                                dt.Value = Convert.ToDateTime(piValue.GetValue(raspLinie, null));
                            }catch (Exception){}
                        }
                    }
                }

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        dt.ReadOnly = true;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        dt.ReadOnly = true;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    dt.ReadOnly = true;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        dt.ReadOnly = true;
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return dt;
        }

        #endregion

        #region ObiectiveIndividuale

        private ASPxGridView CreeazaObiectiveIndividuale(int id, int idGrupValori, string super)
        {
            //int IdQuiz = Convert.ToInt32(_idQuiz);
            Session["CompletareChestionarObiectiv_LinieQuiz"] = id;
            //int F10003 = Convert.ToInt32(_f10003);
            //int idPozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
            ASPxGridView grDateObiective = new ASPxGridView();
            if (Session["lstEval_ObiIndividualeTemp"] == null)
            {
                //string sqlEval_ObiIndividuale = $@"select * from ""Eval_ObiIndividualeTemp"" WHERE F10003={F10003} ORDER BY ""Obiectiv"", ""Activitate"" ";
                DataTable dtObiIndividuale = General.IncarcaDT(@"select * from ""Eval_ObiIndividualeTemp"" WHERE F10003=@1 ORDER BY ""Obiectiv"", ""Activitate"" ", new object[] { _f10003 });
                foreach (DataRow rwObiIndividuale in dtObiIndividuale.Rows)
                {
                    Eval_ObiIndividualeTemp clsObiIndividuale = new Eval_ObiIndividualeTemp(rwObiIndividuale);
                    lstEval_ObiIndividualeTemp.Add(clsObiIndividuale);
                }
                Session["lstEval_ObiIndividualeTemp"] = lstEval_ObiIndividualeTemp;
            }
            else
                lstEval_ObiIndividualeTemp = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
            try
            {
                int TemplateIdObiectiv = -99;
                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    Eval_QuizIntrebari clsQuizIntrebare = lstEval_QuizIntrebari.Where(p => p.Id == id).FirstOrDefault();
                    TemplateIdObiectiv = clsQuizIntrebare.TemplateIdObiectiv;
                }

                if (Session["lstEval_ConfigObTemplate"] == null)
                {
                    //string sqlConfigObTemplate = @"select * from ""Eval_ConfigObTemplate"" /* where ""TemplateId"" = {0} */";
                    //sqlConfigObTemplate = string.Format(sqlConfigObTemplate, TemplateIdObiectiv);
                    DataTable dtEval_ConfigObTemplate = General.IncarcaDT(@"select * from ""Eval_ConfigObTemplate"" /* where ""TemplateId"" = @1 */", new object[] { TemplateIdObiectiv });
                    foreach (DataRow rwEval_ConfigObTemplate in dtEval_ConfigObTemplate.Rows)
                    {
                        Eval_ConfigObTemplate clsConfigObTemplate = new Eval_ConfigObTemplate(rwEval_ConfigObTemplate);
                        lstEval_ConfigObTemplate.Add(clsConfigObTemplate);
                    }
                    Session["lstEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                }
                else
                {
                    lstEval_ConfigObTemplate = Session["lstEval_ConfigObTemplate"] as List<Eval_ConfigObTemplate>;
                }

                if (Session["lstEval_ConfigObTemplateDetail"] == null)
                {
                    //string sqlConfigObTemplateDetail = @"select * from ""Eval_ConfigObTemplateDetail"" /* where ""TemplateId"" = {0} */";
                    //sqlConfigObTemplateDetail = string.Format(sqlConfigObTemplateDetail, TemplateIdObiectiv);
                    DataTable dtEval_ConfigObTemplateDetail = General.IncarcaDT(@"select * from ""Eval_ConfigObTemplateDetail"" /* where ""TemplateId"" = @1 */", new object[] { TemplateIdObiectiv  });
                    foreach (DataRow rwEval_ConfigObTemplateDetail in dtEval_ConfigObTemplateDetail.Rows)
                    {
                        Eval_ConfigObTemplateDetail clsConfigObTemplateDetail = new Eval_ConfigObTemplateDetail(rwEval_ConfigObTemplateDetail);
                        lstEval_ConfigObTemplateDetail.Add(clsConfigObTemplateDetail);
                    }
                    Session["lstEval_ConfigObTemplateDetail"] = lstEval_ConfigObTemplateDetail;
                }
                else
                {
                    lstEval_ConfigObTemplateDetail = Session["lstEval_ConfigObTemplateDetail"] as List<Eval_ConfigObTemplateDetail>;
                }

                if (Session["lstEval_ConfigObiective"] == null)
                {
                    string sqlConfigObiective = @"select * from ""Eval_ConfigObiective""";
                    DataTable dtConfigObiective = General.IncarcaDT(sqlConfigObiective, null);
                    foreach (DataRow rwConfigObiective in dtConfigObiective.Rows)
                    {
                        Eval_ConfigObiective clsConfigObiective = new Eval_ConfigObiective(rwConfigObiective);
                        lstEval_ConfigObiective.Add(clsConfigObiective);
                    }

                    Session["lstEval_ConfigObiective"] = lstEval_ConfigObiective;
                }
                else
                {
                    lstEval_ConfigObiective = Session["lstEval_ConfigObiective"] as List<Eval_ConfigObiective>;
                }

                Eval_ConfigObTemplate clsEval_ConfigObTemplate = lstEval_ConfigObTemplate.Where(p => p.TemplateId == TemplateIdObiectiv).FirstOrDefault();
                if (clsEval_ConfigObTemplate == null)
                    return null;

                #region Grid Properties
                grDateObiective.Width = new Unit(100, UnitType.Percentage);
                grDateObiective.ID = "grDateObiective" + "_WXY_" + id.ToString();
                grDateObiective.SettingsBehavior.AllowFocusedRow = true;
                grDateObiective.SettingsBehavior.EnableCustomizationWindow = true;
                grDateObiective.SettingsBehavior.AllowSelectByRowClick = true;
                grDateObiective.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
                //grDateObiective.Settings.ShowFilterRow = true;
                grDateObiective.Settings.ShowGroupPanel = false;
                grDateObiective.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                grDateObiective.SettingsSearchPanel.Visible = false;
                grDateObiective.AutoGenerateColumns = false;
                
                grDateObiective.ClientSideEvents.ContextMenu = "ctx";
                grDateObiective.SettingsEditing.Mode = GridViewEditingMode.Inline;

                grDateObiective.SettingsBehavior.ConfirmDelete = true;
                grDateObiective.SettingsText.ConfirmDelete = "Confirmati operatia de stergere?";

                grDateObiective.RowDeleting += GrDateObiective_RowDeleting;
                grDateObiective.AutoFilterCellEditorInitialize += GrDateObiective_AutoFilterCellEditorInitialize;
                grDateObiective.CellEditorInitialize += GrDateObiective_CellEditorInitialize;
                grDateObiective.RowInserting += GrDateObiective_RowInserting;
                grDateObiective.RowUpdating += GrDateObiective_RowUpdating;
                grDateObiective.InitNewRow += GrDateObiective_InitNewRow;
                grDateObiective.CustomErrorText += GrDateObiective_CustomErrorText;                                
                #endregion

                #region Grid Default Columns
                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowDeleteButton = clsEval_ConfigObTemplate.PoateSterge == 1 ? true : false;
                colCommand.ShowNewButtonInHeader = clsEval_ConfigObTemplate.PoateAdauga == 1 ? true : false;
                colCommand.ShowEditButton = clsEval_ConfigObTemplate.PoateModifica == 1 ? true : false;
                colCommand.VisibleIndex = 0;
                colCommand.Caption = " ";
                colCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                grDateObiective.Columns.Add(colCommand);

                GridViewDataTextColumn colIdAuto = new GridViewDataTextColumn();
                colIdAuto.FieldName = "IdAuto";
                colIdAuto.Name = "IdAuto";
                colIdAuto.Caption = "IdAuto";
                colIdAuto.Visible = false;
                grDateObiective.Columns.Add(colIdAuto);

                GridViewDataTextColumn colId = new GridViewDataTextColumn();
                colId.FieldName = "Id";
                colId.Name = "Id";
                colId.Caption = "Id";
                colId.Visible = false;
                grDateObiective.Columns.Add(colId);

                GridViewDataTextColumn colIdQuiz = new GridViewDataTextColumn();
                colIdQuiz.FieldName = "IdQuiz";
                colIdQuiz.Name = "IdQuiz";
                colIdQuiz.Caption = "IdQuiz";
                colIdQuiz.Visible = false;
                grDateObiective.Columns.Add(colIdQuiz);

                GridViewDataTextColumn colF10003 = new GridViewDataTextColumn();
                colF10003.FieldName = "F10003";
                colF10003.Name = "F10003";
                colF10003.Caption = "F10003";
                colF10003.Visible = false;
                grDateObiective.Columns.Add(colF10003);

                GridViewDataTextColumn colPozitie = new GridViewDataTextColumn();
                colPozitie.FieldName = "Pozitie";
                colPozitie.Name = "Pozitie";
                colPozitie.Caption = "Pozitie";
                colPozitie.Visible = false;
                grDateObiective.Columns.Add(colPozitie);
                #endregion


                #region AddColumns

                //Florin 2018.07.09
                //string idCateg = General.Nz(General.ExecutaScalar(@"SELECT ""CategorieQuiz"" FROM ""Eval_Quiz"" WHERE ""Id""=" + _idQuiz, null), "0").ToString();
                int y = 34;
                foreach (Eval_ConfigObTemplateDetail clsConfigDetail in lstEval_ConfigObTemplateDetail.Where(p=>p.TemplateId==TemplateIdObiectiv))
                {
                    y++;
                    if (clsConfigDetail.Vizibil == true)
                    {
                        #region colObiectiv
                        if (clsConfigDetail.ColumnName == "Obiectiv")
                        {
                            if (clsConfigDetail.TipValoare == 1)
                            {
                                #region getDS values
                                if (Session["feedEval_Obiectiv"] == null)
                                {
                                    string strSQLObiectiv = @"select ob.""IdObiectiv"" as ""Id"", ob.""Obiectiv"" as ""Denumire""
                                                            from ""Eval_ListaObiectivDet"" det
                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                            where det.""IdLista"" = @1
                                                            and setAng.""Id"" = @2
                                                            group by ob.""IdObiectiv"", ob.""Obiectiv"" ";
                                    //strSQLObiectiv = string.Format(strSQLObiectiv, clsConfigDetail.IdNomenclator, _f10003);
                                    DataTable dtObiectiv = General.IncarcaDT(strSQLObiectiv, new object[] { clsConfigDetail.IdNomenclator, _f10003 });
                                    foreach (DataRow rwObiectiv in dtObiectiv.Rows)
                                    {
                                        metaDate clsObiectiv = new metaDate();
                                        clsObiectiv.Id = Convert.ToInt32(rwObiectiv["Id"]);
                                        clsObiectiv.Denumire = rwObiectiv["Denumire"].ToString();
                                        lstObiective.Add(clsObiectiv);
                                    }
                                }
                                else
                                    lstObiective = Session["feedEval_Obiectiv"] as List<metaDate>;

                                if (Session["feedEval_ObiectivActivitate"] == null)
                                {
                                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", obAct.""Activitate"" as ""Denumire""
                                                                        from ""Eval_ListaObiectivDet"" det
                                                                        join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                        join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                        join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                        where det.""IdLista"" = @1
                                                                        and setAng.""Id"" = @2
                                                                        group by ob.""IdObiectiv"", obAct.""IdActivitate"", obAct.""Activitate"" ";
                                    //strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, clsConfigDetail.IdNomenclator, _f10003);
                                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { clsConfigDetail.IdNomenclator, _f10003 });
                                    lstActivitati.Clear();
                                    foreach (DataRow rwObiectivActivitate in dtObiectivActivitate.Rows)
                                    {
                                        metaDate clsObiectivActivitate = new metaDate();
                                        clsObiectivActivitate.Id = Convert.ToInt32(rwObiectivActivitate["Id"]);
                                        clsObiectivActivitate.Denumire = rwObiectivActivitate["Denumire"].ToString();
                                        clsObiectivActivitate.ParentId = Convert.ToInt32(rwObiectivActivitate["Parinte"].ToString());
                                        lstActivitati.Add(clsObiectivActivitate);
                                    }
                                }
                                else
                                    lstActivitati = Session["feedEval_ObiectivActivitate"] as List<metaDate>;

                                #endregion

                                GridViewDataComboBoxColumn colObiectiv = new GridViewDataComboBoxColumn();
                                colObiectiv.FieldName = "IdObiectiv";
                                colObiectiv.Name = "IdObiectiv";
                                colObiectiv.Caption = Dami.TraduCuvant("Obiectiv");
                                colObiectiv.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colObiectiv.ReadOnly = !clsConfigDetail.Editare;
                                colObiectiv.Visible = clsConfigDetail.Vizibil;

                                colObiectiv.PropertiesComboBox.TextField = "Denumire";
                                colObiectiv.PropertiesComboBox.ValueField = "Id";
                                //colObiectiv.PropertiesComboBox.ValueType = "System.Int32"; 
                                colObiectiv.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDown;
                                colObiectiv.PropertiesComboBox.EnableCallbackMode = true;
                                colObiectiv.PropertiesComboBox.ValidationSettings.RequiredField.IsRequired = true;
                                colObiectiv.PropertiesComboBox.ValidationSettings.Display = Display.None;
                                colObiectiv.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "cmbObiectiv_SelectedIndexChanged";
                                colObiectiv.PropertiesComboBox.ClientInstanceName = "ObjectiveEditor";

                                //Florin 2018.12.11
                                colObiectiv.PropertiesComboBox.DataSource = lstObiective;
                                //colObiectiv.PropertiesComboBox.DataSource = General.IncarcaDT($@"SELECT ""IdObiectiv"" AS ""Id"", ""Obiectiv"" AS ""Denumire"" FROM ""Eval_Obiectiv"" ", null);

                                grDateObiective.Columns.Add(colObiectiv);
                                continue;
                            }
                            else
                            {
                                GridViewDataMemoColumn colObiectiv = new GridViewDataMemoColumn();
                                colObiectiv.FieldName = "Obiectiv";
                                colObiectiv.PropertiesMemoEdit.Rows = 5;
                                colObiectiv.PropertiesMemoEdit.Height = Unit.Percentage(100);
                                colObiectiv.Name = "Obiectiv";
                                colObiectiv.Caption = Dami.TraduCuvant("Obiectiv");
                                colObiectiv.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colObiectiv.ReadOnly = !clsConfigDetail.Editare;
                                colObiectiv.Visible = clsConfigDetail.Vizibil;

                                grDateObiective.Columns.Add(colObiectiv);
                            }
                            continue;
                        }
                        #endregion

                        #region colActivitate
                        if (clsConfigDetail.ColumnName == "Activitate")
                        {
                            //if (lstEval_ConfigObTemplateDetail.Where(p => p.ColumnName == "Obiectiv").FirstOrDefault().TipValoare == 1)
                            if (clsConfigDetail.TipValoare == 1)
                            {
                                #region getDS values
                                if (Session["feedEval_Obiectiv"] == null)
                                {
                                    string strSQLObiectiv = @"select ob.""IdObiectiv"" as ""Id"", ob.""Obiectiv"" as ""Denumire""
                                                            from ""Eval_ListaObiectivDet"" det
                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                            where det.""IdLista"" = @1
                                                            and setAng.""Id"" = @2
                                                            group by ob.""IdObiectiv"", ob.""Obiectiv"" ";
                                    //strSQLObiectiv = string.Format(strSQLObiectiv, clsConfigDetail.IdNomenclator, _f10003);
                                    DataTable dtObiectiv = General.IncarcaDT(strSQLObiectiv, new object[] { clsConfigDetail.IdNomenclator, _f10003 });
                                    foreach (DataRow rwObiectiv in dtObiectiv.Rows)
                                    {
                                        metaDate clsObiectiv = new metaDate();
                                        clsObiectiv.Id = Convert.ToInt32(rwObiectiv["Id"]);
                                        clsObiectiv.Denumire = rwObiectiv["Denumire"].ToString();
                                        lstObiective.Add(clsObiectiv);
                                    }
                                }
                                else
                                    lstObiective = Session["feedEval_Obiectiv"] as List<metaDate>;

                                if (Session["feedEval_ObiectivActivitate"] == null)
                                {
                                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", obAct.""Activitate"" as ""Denumire""
                                                                        from ""Eval_ListaObiectivDet"" det
                                                                        join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                        join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                        join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                        where det.""IdLista"" = @1
                                                                        and setAng.""Id"" = @2
                                                                        group by ob.""IdObiectiv"", obAct.""IdActivitate"", obAct.""Activitate"" ";
                                    //strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, clsConfigDetail.IdNomenclator, _f10003);
                                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { clsConfigDetail.IdNomenclator, _f10003 });
                                    lstActivitati.Clear();
                                    foreach (DataRow rwObiectivActivitate in dtObiectivActivitate.Rows)
                                    {
                                        metaDate clsObiectivActivitate = new metaDate();
                                        clsObiectivActivitate.Id = Convert.ToInt32(rwObiectivActivitate["Id"]);
                                        clsObiectivActivitate.Denumire = rwObiectivActivitate["Denumire"].ToString();
                                        clsObiectivActivitate.ParentId = Convert.ToInt32(rwObiectivActivitate["Parinte"].ToString());
                                        lstActivitati.Add(clsObiectivActivitate);
                                    }
                                }
                                else
                                    lstActivitati = Session["feedEval_ObiectivActivitate"] as List<metaDate>;

                                #endregion

                                GridViewDataComboBoxColumn colActivitate = new GridViewDataComboBoxColumn();
                                colActivitate.FieldName = "IdActivitate";
                                colActivitate.Name = "IdActivitate";
                                colActivitate.Caption = Dami.TraduCuvant("Activitate") + " - " + y;
                                colActivitate.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colActivitate.ReadOnly = !clsConfigDetail.Editare;
                                colActivitate.Visible = clsConfigDetail.Vizibil;

                                colActivitate.PropertiesComboBox.TextField = "Denumire";
                                colActivitate.PropertiesComboBox.ValueField = "Id";
                                colActivitate.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDown;
                                colActivitate.PropertiesComboBox.EnableCallbackMode = true;
                                colActivitate.PropertiesComboBox.ItemRequestedByValue += Activitate_RequestedByValue;
                                colActivitate.PropertiesComboBox.ItemsRequestedByFilterCondition += Activitate_ItemsRequestedByFilterCondition;
                                colActivitate.PropertiesComboBox.DataSource = lstActivitati;


                                colActivitate.PropertiesComboBox.ClientInstanceName = "ActivityEditor";
                                string endCallBackFunction = @"function cmbActivity_EndCallBack(s, e) {
                                                                                                    if (isCustomCascadingCallback)
                                                                                                    {
                                                                                                       if (s.GetItemCount() > 0)
                                                                                                                " + grDateObiective.ID + @".BatchEditApi.SetCellValue(currentEditingIndex, ""IdActivitate"", s.GetItem(0).value); 
                                                                                                        isCustomCascadingCallback = false;
                                                                                                     }
                                                                                                   }";
                                colActivitate.PropertiesComboBox.ClientSideEvents.EndCallback = endCallBackFunction;


                                grDateObiective.Columns.Add(colActivitate);
                            }
                            else
                            {
                                //GridViewDataTextColumn colActivitate = new GridViewDataTextColumn();
                                GridViewDataMemoColumn colActivitate = new GridViewDataMemoColumn();
                                colActivitate.FieldName = "Activitate";
                                colActivitate.PropertiesMemoEdit.Rows = 5;
                                colActivitate.PropertiesMemoEdit.Height = Unit.Percentage(100);
                                colActivitate.Name = "Activitate";
                                colActivitate.Caption = Dami.TraduCuvant("Activitate");
                                colActivitate.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colActivitate.ReadOnly = !clsConfigDetail.Editare;
                                colActivitate.Visible = clsConfigDetail.Vizibil;

                                grDateObiective.Columns.Add(colActivitate);
                            }

                            continue;
                        }
                        #endregion

                        #region colCalificativ
                        if (clsConfigDetail.ColumnName == "Calificativ")
                        {
                            //Radu 03.07.2018
                            int pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                            //Florin 2019.01.29
                            //int tab = Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1;
                            int tab = Convert.ToInt32(Session["Eval_ActiveTab"]);

                            if (clsConfigDetail.TipValoare == 1)
                            {
                                #region getDS
                                if (Session["feedEval_Calificativ"] == null)
                                {
                                    string sqlCalificativ = @"select det.""IdSet"", det.""IdCalificativ"", det.""Denumire"", det.""Nota"", det.""RatingMin"", det.""RatingMax"", det.""Ordine"",
                                                                    det.""Explicatii""
                                                               from ""Eval_SetCalificativDet"" det
                                                                where det.""IdSet"" = @1 ";
                                    //sqlCalificativ = string.Format(sqlCalificativ, clsConfigDetail.IdNomenclator);
                                    DataTable dtCalificativ = General.IncarcaDT(sqlCalificativ, new object[] { clsConfigDetail.IdNomenclator });
                                    foreach (DataRow rwCalificativ in dtCalificativ.Rows)
                                    {
                                        Eval_SetCalificativDet clsCalificativ = new Eval_SetCalificativDet(rwCalificativ);
                                        lstEval_SetCalificativDet.Add(clsCalificativ);
                                    }
                                    Session["feedEval_Calificativ"] = lstEval_SetCalificativDet;
                                }
                                else
                                    lstEval_SetCalificativDet = Session["feedEval_Calificativ"] as List<Eval_SetCalificativDet>;
                                #endregion

                                GridViewDataComboBoxColumn colCalificativ = new GridViewDataComboBoxColumn();
                                colCalificativ.FieldName = "IdCalificativ";
                                colCalificativ.Name = "IdCalificativ";
                                colCalificativ.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colCalificativ.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colCalificativ.ReadOnly = !clsConfigDetail.Editare;
                                colCalificativ.Visible = clsConfigDetail.Vizibil;

                                colCalificativ.PropertiesComboBox.TextField = "Denumire";
                                colCalificativ.PropertiesComboBox.ValueField = "IdCalificativ";
                                colCalificativ.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDown;
                                colCalificativ.PropertiesComboBox.DataSource = lstEval_SetCalificativDet;
                                colCalificativ.Visible = false;
                                if (Convert.ToInt32(_idClient) != 20 || pozitie >= 2 || tab >= 2)   //Radu 03.07.2018 - calificativul nu trebuie sa fie afisat pe tab-ul angajatului decat dupa ce acesta finalizeaza
                                    colCalificativ.Visible = true;
                                grDateObiective.Columns.Add(colCalificativ);
                                continue;
                            }
                            else
                            {
                                GridViewDataTextColumn colCalificativ = new GridViewDataTextColumn();
                                colCalificativ.FieldName = "Calificativ";
                                colCalificativ.Name = "Calificativ";
                                colCalificativ.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colCalificativ.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colCalificativ.ReadOnly = !clsConfigDetail.Editare;
                                colCalificativ.Visible = clsConfigDetail.Vizibil;
                                colCalificativ.Visible = false;
                                if (Convert.ToInt32(_idClient) != 20 || pozitie >= 2 || tab >= 2)   //Radu 03.07.2018 - calificativul nu trebuie sa fie afisat pe tab-ul angajatului decat dupa ce acesta finalizeaza
                                    colCalificativ.Visible = true;
                                grDateObiective.Columns.Add(colCalificativ);
                            }
                            continue;
                        }
                        #endregion

                        Eval_ConfigObiective clsConfigObiective = lstEval_ConfigObiective.Where(p => p.ColumnName == clsConfigDetail.ColumnName).FirstOrDefault();
                        if (clsConfigObiective == null)
                            continue;

                        switch (clsConfigObiective.ColumnType)
                        {
                            case "System.String":
                                //Florin 2019.01.09 vor ca coloana Comentarii Status Evaluat sa fie de tip memo la fel ca Descriere si Indicator Performanta
                                GridViewDataMemoColumn colString = new GridViewDataMemoColumn();
                                //GridViewDataTextColumn colString = new GridViewDataTextColumn();
                                colString.FieldName = clsConfigDetail.ColumnName;
                                colString.Name = clsConfigDetail.ColumnName;
                                colString.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colString.PropertiesMemoEdit.Rows = 5;
                                colString.PropertiesMemoEdit.Height = Unit.Percentage(100);
                                colString.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colString.ReadOnly = !clsConfigDetail.Editare;
                                colString.Visible = clsConfigDetail.Vizibil;

                                grDateObiective.Columns.Add(colString);
                                break;
                            case "System.Decimal":
                                GridViewDataTextColumn colDecimal = new GridViewDataTextColumn();
                                colDecimal.FieldName = clsConfigDetail.ColumnName;
                                colDecimal.Name = clsConfigDetail.ColumnName;
                                colDecimal.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colDecimal.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colDecimal.ReadOnly = !clsConfigDetail.Editare;
                                colDecimal.Visible = clsConfigDetail.Vizibil;

                                colDecimal.PropertiesTextEdit.DisplayFormatString = "n2";
                                colDecimal.PropertiesTextEdit.MaskSettings.Mask = "<0..999g>.<00..99>";

                                grDateObiective.Columns.Add(colDecimal);
                                break;
                            case "System.Int32":
                                GridViewDataTextColumn colInt = new GridViewDataTextColumn();
                                colInt.FieldName = clsConfigDetail.ColumnName;
                                colInt.Name = clsConfigDetail.ColumnName;
                                colInt.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colInt.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colInt.ReadOnly = !clsConfigDetail.Editare;
                                colInt.Visible = clsConfigDetail.Vizibil;

                                colInt.PropertiesTextEdit.DisplayFormatString = "n0";
                                colInt.PropertiesTextEdit.MaskSettings.Mask = "<0..999g>";

                                grDateObiective.Columns.Add(colInt);
                                break;
                        }

                    }
                }
                #endregion


                //Florin 2018.07.09
                if (grDateObiective.Columns["Obiectiv"] != null && Convert.ToInt32(_idClient) == 20 && (idCateg == "1" || idCateg == "2"))
                    grDateObiective.Columns["Obiectiv"].Caption = Dami.TraduCuvant("Puncte Forte");
                if (grDateObiective.Columns["Activitate"] != null && Convert.ToInt32(_idClient) == 20 && (idCateg == "1" || idCateg == "2"))
                    grDateObiective.Columns["Activitate"].Caption = Dami.TraduCuvant("Zone de dezvoltare");

				//Radu 13.07.2018
				//int poz = 0;
                //if (Session["CompletareChestionar_Pozitie"] != null)
                //    poz = (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1).ToString();				
			
                //if (grDateObiective.Columns["Obiectiv"] != null && Convert.ToInt32(_idClient) == 20 && (idCateg == "1" || idCateg == "2" || (poz == 2 && grDateObiective.Columns.Count == 2)))
                //    grDateObiective.Columns["Obiectiv"].Caption = Dami.TraduCuvant("Puncte Forte");
                //if (grDateObiective.Columns["Activitate"] != null && Convert.ToInt32(_idClient) == 20 && (idCateg == "1" || idCateg == "2" || (poz == 2 && grDateObiective.Columns.Count == 2)))
                //    grDateObiective.Columns["Activitate"].Caption = Dami.TraduCuvant("Zone de dezvoltare");					


                #region Grid Command Buttons
                //grDateObiective.SettingsCommandButton.EditButton.Image.Url = "../Fisiere/Imagini/Icoane/edit.png";
                //grDateObiective.SettingsCommandButton.EditButton.Image.AlternateText = "Edit";
                //grDateObiective.SettingsCommandButton.EditButton.Image.ToolTip = "Edit";

                grDateObiective.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                grDateObiective.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                grDateObiective.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                grDateObiective.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                grDateObiective.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                grDateObiective.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";
                
                if (clsEval_ConfigObTemplate.PoateAdauga == 1)
                {
                    grDateObiective.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                    grDateObiective.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";
                }

                if (clsEval_ConfigObTemplate.PoateSterge == 1)
                {
                    grDateObiective.SettingsCommandButton.DeleteButton.Image.ToolTip = "Sterge";
                    grDateObiective.SettingsCommandButton.DeleteButton.Image.Url = "~/Fisiere/Imagini/Icoane/sterge.png";
                }

                if (clsEval_ConfigObTemplate.PoateModifica == 1)
                {
                    grDateObiective.SettingsCommandButton.EditButton.Image.ToolTip = "Modifica";
                    grDateObiective.SettingsCommandButton.EditButton.Image.Url = "~/Fisiere/Imagini/Icoane/edit.png";
                }

                #endregion

                //string log = "Inainte - " + lstEval_ObiIndividualeTemp.Count() + Environment.NewLine;
                //log += "Concret - " + lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == F10003 && p.Pozitie == Convert.ToInt32(Session["Eval_PozitieUserLogat"]) && p.IdQuiz == IdQuiz).ToList().Count();
                

                var erf = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == _f10003 && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"]) && p.IdQuiz == _idQuiz).ToList();
                //Florin 2019.01.29
                grDateObiective.DataSource = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == _f10003 && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"]) && p.IdQuiz == _idQuiz).ToList();
                ////Florin 2019.01.08
                //grDateObiective.DataSource = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == F10003 && p.Pozitie == Convert.ToInt32(Session["Eval_PozitieUserLogat"]) && p.IdQuiz == IdQuiz).ToList();
                var ert = Convert.ToInt32(Session["Eval_ActiveTab"]);

                //List<Eval_ObiIndividualeTemp> ert = grDateObiective.DataSource as List<Eval_ObiIndividualeTemp>;
                //log += "Dupa - " + ert.Count() + Environment.NewLine;

                // General.MemoreazaEroarea(log);

                ////Florin 2018.12.10
                //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                //    grDateObiective.DataSource = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == F10003 && p.Pozitie == Convert.ToInt32(Session["Eval_PozitieUserLogat"]) && p.IdQuiz == IdQuiz).ToList();
                //else
                //    grDateObiective.DataSource = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == F10003 && p.Pozitie == Convert.ToInt32(super.Substring(5, 1)) && p.IdQuiz == IdQuiz).ToList();
                grDateObiective.KeyFieldName = "IdAuto";
                grDateObiective.DataBind();


                if (Convert.ToInt32(_idClient) == 20)
                {                    
                    for (int i = 0; i < grDateObiective.VisibleRowCount; i++)
                    {
                        Eval_ObiIndividualeTemp clsObiIndividual = grDateObiective.GetRow(i) as Eval_ObiIndividualeTemp;
                        try
                        {
                            clsObiIndividual.Target = (Convert.ToDecimal(clsObiIndividual.Pondere) / Convert.ToDecimal(clsObiIndividual.Activitate)) * 100;
                            if (clsObiIndividual.Target < 80) clsObiIndividual.IdCalificativ = 1;
                            if (80 <= clsObiIndividual.Target && clsObiIndividual.Target < 95) clsObiIndividual.IdCalificativ = 2;
                            if (95 <= clsObiIndividual.Target && clsObiIndividual.Target < 105) clsObiIndividual.IdCalificativ = 3;
                            if (105 <= clsObiIndividual.Target && clsObiIndividual.Target < 130) clsObiIndividual.IdCalificativ = 4;
                            if (clsObiIndividual.Target >= 130) clsObiIndividual.IdCalificativ = 5;
                        }
                        catch (Exception) { }
                    }                    
                }

                int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        grDateObiective.Enabled = false;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        grDateObiective.Enabled = false;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    grDateObiective.Enabled = false;
                //}

                if (Convert.ToInt32(_idClient) == 20)
                {//daca formularul este pe pozitia 1, evaluatorul sa poata completa comentariile pe tab-ul sau
                    if (idCateg == "0")
                    {
                        //Florin 2019.01.29
                        //if (poz == 1 && Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1 == 1)
                        if (poz == 1 && Session["Eval_ActiveTab"].ToString() == "1")
                        {
                            //string sql = "SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = {0} and F10003 = {1} AND ""Pozitie"" = 2";
                            //sql = string.Format(sql, IdQuiz, F10003);
                            DataTable dt = General.IncarcaDT(@"SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 and F10003 = @2 AND ""Pozitie"" = 2", new object[] { _idQuiz, _f10003 });
                            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                            {
                                if (dt.Rows[0][0].ToString() == Session["UserId"].ToString())
                                    grDateObiective.Enabled = true;

                            }
                        }
                    }
                }

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        grDateObiective.Enabled = false;
                //}

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return grDateObiective;
        }

        private void GrDateObiective_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
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

        private void GrDateObiective_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                List<Eval_ObiIndividualeTemp> dt = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                e.NewValues["IdAuto"] = dt.Count + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateObiective_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                ASPxGridView grid = sender as ASPxGridView;

                GridViewDataComboBoxColumn colObiectiv = (grid.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);


                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;

                Eval_ObiIndividualeTemp clsObiIndividual = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                if (e.NewValues.Contains("IdObiectiv"))
                {
                    clsObiIndividual.IdObiectiv = e.NewValues["IdObiectiv"] == null ? -99 : Convert.ToInt32(e.NewValues["IdObiectiv"]);
                }

                //Radu 09.01.2019
                if (e.NewValues.Contains("Obiectiv"))
                {
                    clsObiIndividual.Obiectiv = (colObiectiv != null ? colObiectiv.PropertiesComboBox.Items.FindByValue(clsObiIndividual.IdObiectiv).Text : (e.NewValues["Obiectiv"] ?? "").ToString()).Replace("'", "");
                }

                if (e.NewValues.Contains("IdActivitate"))
                    clsObiIndividual.IdActivitate = e.NewValues["IdActivitate"] == null ? -99 : Convert.ToInt32(e.NewValues["IdActivitate"]);

                if (Session["feedEval_ObiectivActivitate"] == null)
                {
                    //string strSQLObiectivActivitate = @"select ""IdObiectiv"" as ""Parinte"", ""IdActivitate"" as ""Id"", ""Activitate"" as ""Denumire""
                    //                                                    from  ""Eval_ObiectivXActivitate"" WHERE  ""IdActivitate"" = {0} ";
                    //strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, clsObiIndividual.IdActivitate);
                    DataTable dtObiectivActivitate = General.IncarcaDT(
                        @"select ""IdObiectiv"" as ""Parinte"", ""IdActivitate"" as ""Id"", ""Activitate"" as ""Denumire""
                        from  ""Eval_ObiectivXActivitate"" WHERE  ""IdActivitate"" = @1 ", new object[] { clsObiIndividual.IdActivitate });
                    lstActivitati.Clear();
                    foreach (DataRow rwObiectivActivitate in dtObiectivActivitate.Rows)
                    {
                        metaDate clsObiectivActivitate = new metaDate();
                        clsObiectivActivitate.Id = Convert.ToInt32(rwObiectivActivitate["Id"]);
                        clsObiectivActivitate.Denumire = rwObiectivActivitate["Denumire"].ToString();
                        clsObiectivActivitate.ParentId = Convert.ToInt32(rwObiectivActivitate["Parinte"].ToString());
                        lstActivitati.Add(clsObiectivActivitate);
                    }
                }
                else
                    lstActivitati = Session["feedEval_ObiectivActivitate"] as List<metaDate>;


                if (e.NewValues.Contains("Activitate"))
                    clsObiIndividual.Activitate = (lstActivitati != null && lstActivitati.Count > 0 ? (lstActivitati.Find(p => p.Id == clsObiIndividual.IdActivitate) == null ? "" : lstActivitati.Find(p => p.Id == clsObiIndividual.IdActivitate).Denumire) : (e.NewValues["Activitate"] ?? "").ToString()).Replace("'", "");
                if (e.NewValues.Contains("Pondere"))
                    clsObiIndividual.Pondere = e.NewValues["Pondere"] == null ? 0 : Convert.ToDecimal(e.NewValues["Pondere"].ToString());
                if (e.NewValues.Contains("Descriere"))
                    clsObiIndividual.Descriere = e.NewValues["Descriere"] == null ? "" : e.NewValues["Descriere"].ToString().Replace("'", "");
                if (e.NewValues.Contains("Target"))
                    clsObiIndividual.Target = e.NewValues["Target"] == null ? 0 : Convert.ToDecimal(e.NewValues["Target"].ToString());
                if (e.NewValues.Contains("Realizat"))
                    clsObiIndividual.Realizat = e.NewValues["Realizat"] == null ? 0 : Convert.ToInt32(e.NewValues["Realizat"].ToString());
                if (e.NewValues.Contains("IdCalificativ"))
                    clsObiIndividual.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                if (e.NewValues.Contains("Calificativ"))
                    clsObiIndividual.Calificativ = e.NewValues["Calificativ"] == null ? "" : e.NewValues["Calificativ"].ToString().Replace("'", "");
                if (e.NewValues.Contains("ExplicatiiCalificativ"))
                    clsObiIndividual.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString().Replace("'", "");
                clsObiIndividual.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);

                //Radu 23.07.2018
                int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //Florin 2019.01.29
                //int pozTab = Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1;
                int pozTab = Convert.ToInt32(Session["Eval_ActiveTab"]);
                if (Convert.ToInt32(_idClient) == 20 && poz == 1 && pozTab == 2 && e.NewValues.Contains("ExplicatiiCalificativ"))
                {//evaluatorul poate completa o coloana care sa fie vizibila si pe tab-ul angajat
                    Eval_ObiIndividualeTemp clsObiIndividualAngajat = lst.Where(p => p.IdQuiz == clsObiIndividual.IdQuiz && p.F10003 == clsObiIndividual.F10003 && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz
                                                                                    && p.IdObiectiv == clsObiIndividual.IdObiectiv && p.IdActivitate == clsObiIndividual.IdActivitate && p.Pozitie == 1).FirstOrDefault();
                    clsObiIndividualAngajat.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString().Replace("'", "");
                }

                if (Convert.ToInt32(_idClient) == 20 && poz == 2 && pozTab == 2 && e.NewValues.Contains("IdCalificativ"))
                {
                    Eval_ObiIndividualeTemp clsObiIndividualAngajat = lst.Where(p => p.IdQuiz == clsObiIndividual.IdQuiz && p.F10003 == clsObiIndividual.F10003 && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz
                                                                                    && p.IdObiectiv == clsObiIndividual.IdObiectiv && p.IdActivitate == clsObiIndividual.IdActivitate && p.Pozitie == 1).FirstOrDefault();
                    clsObiIndividualAngajat.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                }


                //Florin 2019.01.29
                poz = Convert.ToInt32(Session["Eval_ActiveTab"]);
                //////Florin 2018.12.10
                ////if (Convert.ToInt32(_idClient) == 20)
                ////{
                ////    //Florin 2019.01.08
                //    poz = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                ////    //if (idCateg == "0")
                ////    //    poz = pozTab;
                ////    //else
                ////    //    poz = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                ////}


                clsObiIndividual.F10003 = _f10003;
                clsObiIndividual.Pozitie = poz;
                clsObiIndividual.IdQuiz = _idQuiz;

                //Radu 16.07.2018
                if (e.NewValues.Contains("ColoanaSuplimentara1"))
                    clsObiIndividual.ColoanaSuplimentara1 = e.NewValues["ColoanaSuplimentara1"] == null ? "" : e.NewValues["ColoanaSuplimentara1"].ToString().Replace("'", "");
                if (e.NewValues.Contains("ColoanaSuplimentara2"))
                    clsObiIndividual.ColoanaSuplimentara2 = e.NewValues["ColoanaSuplimentara2"] == null ? "" : e.NewValues["ColoanaSuplimentara2"].ToString().Replace("'", "");

                clsObiIndividual.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                clsObiIndividual.TIME = DateTime.Now;


                if (Convert.ToInt32(_idClient) == 20)
                {
                    try
                    {
                        int pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                        if (pozitie >= 2)   //se doreste ca rating-ul sa fie afisat numai dupa ce angajatul finalizeaza
                        {
                            decimal activitate = 0;
                            if (decimal.TryParse(clsObiIndividual.Activitate, out activitate))
                            {
                                clsObiIndividual.Target = (Convert.ToDecimal(clsObiIndividual.Pondere) / Convert.ToDecimal(clsObiIndividual.Activitate)) * 100;
                                if (clsObiIndividual.Target < 80) clsObiIndividual.IdCalificativ = 1;
                                if (80 <= clsObiIndividual.Target && clsObiIndividual.Target < 95) clsObiIndividual.IdCalificativ = 2;
                                if (95 <= clsObiIndividual.Target && clsObiIndividual.Target < 105) clsObiIndividual.IdCalificativ = 3;
                                if (105 <= clsObiIndividual.Target && clsObiIndividual.Target < 130) clsObiIndividual.IdCalificativ = 4;
                                if (clsObiIndividual.Target >= 130) clsObiIndividual.IdCalificativ = 5;
                            }
                            //if (clsObiIndividual.Target < 80) clsObiIndividual.Calificativ = "mult sub asteptari";
                            //if (80 <= clsObiIndividual.Target && clsObiIndividual.Target < 95) clsObiIndividual.Calificativ = "sub asteptari";
                            //if (95 <= clsObiIndividual.Target && clsObiIndividual.Target < 105) clsObiIndividual.Calificativ = "in linie cu asteptarile";
                            //if (105 <= clsObiIndividual.Target && clsObiIndividual.Target < 130) clsObiIndividual.Calificativ = "peste asteptari";
                            //if (clsObiIndividual.Target >= 130) clsObiIndividual.Calificativ = "mult peste asteptari";
                        }
                    }
                    catch (Exception){}
                }


                e.Cancel = true;

                grid.CancelEdit();
                Session["lstEval_ObiIndividualeTemp"] = lst;

                //Radu 23.07.2018
                if (Convert.ToInt32(_idClient) == 20 && poz == 1 && pozTab == 2)
                    grid.DataSource = lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.Pozitie == 2 && p.IdQuiz == clsObiIndividual.IdQuiz && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz).ToList();
                else
                {
                    //Florin 2019.01.14
                    //grid.DataSource = lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.Pozitie == clsObiIndividual.Pozitie && p.IdQuiz == clsObiIndividual.IdQuiz && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz).ToList();
                    grid.DataSource = lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz && p.Pozitie == poz && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateObiective_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                ASPxGridView grid = sender as ASPxGridView;

                GridViewDataComboBoxColumn colObiectiv = (grid.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);


                int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                //if (Convert.ToInt32(_idClient) == 20)
                //    poz = Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1;

                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                Eval_ObiIndividualeTemp clsNew = new Eval_ObiIndividualeTemp();
                //clsNew.IdAuto = lst.Count + 1;

                //Florin 2019.01.14
                clsNew.IdAuto = -1 * (Convert.ToInt32(General.Nz(lst.Max(p => (int?)Math.Abs(p.IdAuto)),0)) + 1);
                //clsNew.IdAuto = DamiIdObi();

                clsNew.F10003 = Convert.ToInt32(_f10003.ToString());

                //Florin 2019.01.29
                poz = Convert.ToInt32(Session["Eval_ActiveTab"]);
                //////Florin 2018.12.10
                ////if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                ////{
                //    poz = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                ////}

                clsNew.Pozitie = poz;
                clsNew.IdQuiz = _idQuiz;
                clsNew.IdObiectiv = e.NewValues["IdObiectiv"] == null ? -99 : Convert.ToInt32(e.NewValues["IdObiectiv"]);
                clsNew.Obiectiv = (colObiectiv != null ? colObiectiv.PropertiesComboBox.Items.FindByValue(clsNew.IdObiectiv).Text : (e.NewValues["Obiectiv"] ?? "").ToString()).Replace("'", "");
                clsNew.IdActivitate = e.NewValues["IdActivitate"] == null ? -99 : Convert.ToInt32(e.NewValues["IdActivitate"]);

                if (Session["feedEval_ObiectivActivitate"] == null)
                {
                    //string strSQLObiectivActivitate = @"select ""IdObiectiv"" as ""Parinte"", ""IdActivitate"" as ""Id"", ""Activitate"" as ""Denumire""
                    //                                                    from  ""Eval_ObiectivXActivitate"" WHERE  ""IdActivitate"" = {0} ";
                    //strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, clsNew.IdActivitate);
                    DataTable dtObiectivActivitate = General.IncarcaDT(
                        @"select ""IdObiectiv"" as ""Parinte"", ""IdActivitate"" as ""Id"", ""Activitate"" as ""Denumire""
                        from  ""Eval_ObiectivXActivitate"" WHERE  ""IdActivitate"" = @1 ", new object[] { clsNew.IdActivitate });
                    lstActivitati.Clear();
                    foreach (DataRow rwObiectivActivitate in dtObiectivActivitate.Rows)
                    {
                        metaDate clsObiectivActivitate = new metaDate();
                        clsObiectivActivitate.Id = Convert.ToInt32(rwObiectivActivitate["Id"]);
                        clsObiectivActivitate.Denumire = rwObiectivActivitate["Denumire"].ToString();
                        clsObiectivActivitate.ParentId = Convert.ToInt32(rwObiectivActivitate["Parinte"].ToString());
                        lstActivitati.Add(clsObiectivActivitate);
                    }
                }
                else
                    lstActivitati = Session["feedEval_ObiectivActivitate"] as List<metaDate>;

                clsNew.Activitate = (lstActivitati != null && lstActivitati.Count > 0 ? (lstActivitati.Find(p => p.Id == clsNew.IdActivitate) == null ? "" : lstActivitati.Find(p => p.Id == clsNew.IdActivitate).Denumire) : (e.NewValues["Activitate"] ?? "").ToString()).Replace("'", "");
                clsNew.Pondere = e.NewValues["Pondere"] == null ? 0 : Convert.ToDecimal(e.NewValues["Pondere"].ToString());
                clsNew.Descriere = e.NewValues["Descriere"] == null ? "" : e.NewValues["Descriere"].ToString().Replace("'", "");
                clsNew.Target = e.NewValues["Target"] == null ? 0 : Convert.ToDecimal(e.NewValues["Target"].ToString());
                clsNew.Realizat = e.NewValues["Realizat"] == null ? 0 : Convert.ToInt32(e.NewValues["Realizat"].ToString());
                clsNew.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                clsNew.Calificativ = e.NewValues["Calificativ"] == null ? "" : e.NewValues["Calificativ"].ToString().Replace("'", "");
                clsNew.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString().Replace("'", "");
                //clsNew.IdLinieQuiz = Convert.ToInt32(Session["CompletareChestionarObiectiv_LinieQuiz"]);
                clsNew.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);

                //Radu 16.07.2018
                clsNew.ColoanaSuplimentara1 = e.NewValues["ColoanaSuplimentara1"] == null ? "" : e.NewValues["ColoanaSuplimentara1"].ToString().Replace("'", "");
                clsNew.ColoanaSuplimentara2 = e.NewValues["ColoanaSuplimentara2"] == null ? "" : e.NewValues["ColoanaSuplimentara2"].ToString().Replace("'", "");

                clsNew.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"],-99));
                clsNew.TIME = DateTime.Now;

                lst.Add(clsNew);
                Session["lstEval_ObiIndividualeTemp"] = lst;

                e.Cancel = true;

                grid.CancelEdit();

                //Florin 2019.01.14
                ///grid.DataSource = lst.Where(p => p.F10003 == clsNew.F10003 && p.Pozitie == clsNew.Pozitie && p.IdQuiz == clsNew.IdQuiz && p.IdLinieQuiz == clsNew.IdLinieQuiz).ToList();
                grid.DataSource = lst.Where(p => p.F10003 == clsNew.F10003 && p.IdQuiz == clsNew.IdQuiz && p.Pozitie == poz && p.IdLinieQuiz == clsNew.IdLinieQuiz).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateObiective_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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

        private void GrDateObiective_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateObiective_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {

                int IdLinieQuiz = Convert.ToInt32(Session["CompletareChestionarObiectiv_LinieQuiz"]);

                //int F10003 = Convert.ToInt32(_f10003.ToString());
                //int Pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                //int IdQuiz = Convert.ToInt32(_idQuiz.ToString());


                //Florin 2018.01.29
                int Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);

                ////Florin 2018.12.10
                //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                //{
                //    Pozitie = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                //}

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                ASPxGridView grid = sender as ASPxGridView;

                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;

                Eval_ObiIndividualeTemp clsObiIndividual = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                lst.Remove(clsObiIndividual);
                Session["lstEval_ObiIndividualeTemp"] = lst;
                e.Cancel = true;
                grid.DataSource = lst.Where(p => p.IdQuiz == _idQuiz && p.F10003 == _f10003 && p.IdLinieQuiz == IdLinieQuiz && p.Pozitie == Pozitie).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private int GetCurrentObjective()
        {
            object id = null;
            if (hfObiectiv.TryGet("CurrentObjective", out id))
                return Convert.ToInt32(id);
            return -1;
        }

        private void Activitate_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                GetActivitati();

                ASPxComboBox editor = source as ASPxComboBox;
                List<metaDate> query;
                var take = e.EndIndex - e.BeginIndex + 1;
                var skip = e.BeginIndex;
                int countryValue = GetCurrentObjective();
                if (countryValue > -1)
                    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter) && p.ParentId == countryValue).OrderBy(p => p.Id).ToList<metaDate>();
                else
                    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter)).OrderBy(p => p.Id).ToList<metaDate>();
                editor.DataSource = query;
                editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Activitate_RequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
        {
            try
            {
                GetActivitati();

                int id;
                if (e.Value == null || !int.TryParse(e.Value.ToString(), out id))
                    return;
                ASPxComboBox editor = source as ASPxComboBox;
                var query = lstActivitati.Where(p => p.Id == id);
                editor.DataSource = query.ToList();
                editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        #region CompetenteAngajat
        private ASPxGridView CreeazaCompetenteAngajat(int id, int idGrupValori, string super)
        {
            int idPozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
            Session["CompletareChestionarCompetente_LinieQuiz"] = id;
            //int IdQuiz = Convert.ToInt32(_idQuiz ?? -99);
            //int F10003 = Convert.ToInt32(_f10003 ?? -99);

            ASPxGridView grDateCompetente = new ASPxGridView();
            if (Session["lstEval_CompetenteAngajatTemp"] == null)
            {
                //Florin 2018.09.25
                ////string sqlEval_CompetenteAngajat = @"select * from ""Eval_CompetenteAngajatTemp"" ORDER BY ""Competenta"" ";
                //string sqlEval_CompetenteAngajat = $@"select * from ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""={IdQuiz} AND F10003={F10003} ORDER BY ""Competenta"" ";
                DataTable dtCompetenteAngajat = General.IncarcaDT(@"select * from ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 ORDER BY ""Competenta"" ", new object[] { _idQuiz, _f10003 });
                foreach (DataRow rwCompetenteAngajat in dtCompetenteAngajat.Rows)
                {
                    Eval_CompetenteAngajatTemp clsCompetenteAngajat = new Eval_CompetenteAngajatTemp(rwCompetenteAngajat);
                    lstEval_CompetenteAngajatTemp.Add(clsCompetenteAngajat);
                }
                Session["lstEval_CompetenteAngajatTemp"] = lstEval_CompetenteAngajatTemp;
            }
            else
                lstEval_CompetenteAngajatTemp = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;

            try
            {
                int TemplateIdCompetenta = -99;
                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();
                if (raspLinie != null)
                {
                    Eval_QuizIntrebari clsQuizIntrebare = lstEval_QuizIntrebari.Where(p => p.Id == id).FirstOrDefault();
                    TemplateIdCompetenta = clsQuizIntrebare.TemplateIdCompetenta;
                }

                if (Session["lstEval_ConfigCompTemplate"] == null)
                {
                    //string sqlConfigCompTemplate = @"select * from ""Eval_ConfigCompTemplate"" /*where ""TemplateId"" = {0} */";
                    //sqlConfigCompTemplate = string.Format(sqlConfigCompTemplate, TemplateIdCompetenta);
                    DataTable dtEval_ConfigCompTemplate = General.IncarcaDT(@"select * from ""Eval_ConfigCompTemplate"" /*where ""TemplateId"" = @1 */", new object[] { TemplateIdCompetenta });
                    foreach (DataRow rwEval_ConfigCompTemplate in dtEval_ConfigCompTemplate.Rows)
                    {
                        Eval_ConfigCompTemplate clsConfigCompTemplate = new Eval_ConfigCompTemplate(rwEval_ConfigCompTemplate);
                        lstEval_ConfigCompTemplate.Add(clsConfigCompTemplate);
                    }
                    Session["lstEval_ConfigCompTemplate"] = lstEval_ConfigCompTemplate;
                }
                else
                {
                    lstEval_ConfigCompTemplate = Session["lstEval_ConfigCompTemplate"] as List<Eval_ConfigCompTemplate>;
                }

                if (Session["lstEval_ConfigCompTemplateDetail"] == null)
                {
                    //string sqlConfigCompTemplateDetail = @"select * from ""Eval_ConfigCompTemplateDetail"" /*where ""TemplateId"" = {0} */";
                    //sqlConfigCompTemplateDetail = string.Format(sqlConfigCompTemplateDetail, TemplateIdCompetenta);
                    DataTable dtEval_ConfigCompTemplateDetail = General.IncarcaDT(@"select * from ""Eval_ConfigCompTemplateDetail"" /*where ""TemplateId"" = @1 */", new object[] { TemplateIdCompetenta });
                    foreach (DataRow rwEval_ConfigCompTemplateDetail in dtEval_ConfigCompTemplateDetail.Rows)
                    {
                        Eval_ConfigCompTemplateDetail clsConfigCompTemplateDetail = new Eval_ConfigCompTemplateDetail(rwEval_ConfigCompTemplateDetail);
                        lstEval_ConfigCompTemplateDetail.Add(clsConfigCompTemplateDetail);
                    }
                    Session["lstEval_ConfigCompTemplateDetail"] = lstEval_ConfigCompTemplateDetail;
                }
                else
                {
                    lstEval_ConfigCompTemplateDetail = Session["lstEval_ConfigCompTemplateDetail"] as List<Eval_ConfigCompTemplateDetail>;
                }

                if (Session["lstEval_ConfigCompetente"] == null)
                {
                    //string sqlConfigCompetente = @"select * from ""Eval_ConfigCompetente"" ";
                    DataTable dtConfigCompetente = General.IncarcaDT(@"select * from ""Eval_ConfigCompetente"" ", null);
                    foreach (DataRow rwConfigCompetente in dtConfigCompetente.Rows)
                    {
                        Eval_ConfigCompetente clsConfigCompetente = new Eval_ConfigCompetente(rwConfigCompetente);
                        lstEval_ConfigCompetente.Add(clsConfigCompetente);
                    }
                    Session["lstEval_ConfigCompetente"] = lstEval_ConfigCompetente;
                }
                else
                {
                    lstEval_ConfigCompetente = Session["lstEval_ConfigCompetente"] as List<Eval_ConfigCompetente>;
                }

                Eval_ConfigCompTemplate clsEval_ConfigCompTemplate = lstEval_ConfigCompTemplate.Where(p => p.TemplateId == TemplateIdCompetenta).FirstOrDefault();
                if (clsEval_ConfigCompTemplate == null)
                    return null;

                #region GridProperties
                grDateCompetente.Width = 1500;
                grDateCompetente.ID = "grDateCompetente" + "_WXY_" + id.ToString();
                grDateCompetente.SettingsBehavior.AllowFocusedRow = true;
                grDateCompetente.SettingsBehavior.EnableCustomizationWindow = true;
                grDateCompetente.SettingsBehavior.AllowSelectByRowClick = true;
                grDateCompetente.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
                //grDateCompetente.Settings.ShowFilterRow = true;
                grDateCompetente.Settings.ShowGroupPanel = false;
                grDateCompetente.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                grDateCompetente.SettingsSearchPanel.Visible = false;

                grDateCompetente.ClientSideEvents.ContextMenu = "ctx";
                grDateCompetente.SettingsEditing.Mode = GridViewEditingMode.Inline;

                grDateCompetente.RowDeleting += GrDateCompetente_RowDeleting; ;
                grDateCompetente.AutoFilterCellEditorInitialize += GrDateCompetente_AutoFilterCellEditorInitialize; ;
                grDateCompetente.CellEditorInitialize += GrDateCompetente_CellEditorInitialize; ;
                grDateCompetente.RowInserting += GrDateCompetente_RowInserting; ;
                grDateCompetente.RowUpdating += GrDateCompetente_RowUpdating; ;
                grDateCompetente.InitNewRow += GrDateCompetente_InitNewRow; ;
                grDateCompetente.CustomErrorText += GrDateCompetente_CustomErrorText; ;
                #endregion


                #region Grid Default Columns
                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowDeleteButton = true;
                colCommand.ShowNewButtonInHeader = true;
                colCommand.ShowEditButton = true;
                colCommand.VisibleIndex = 0;
                colCommand.Caption = " ";
                colCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                grDateCompetente.Columns.Add(colCommand);

                GridViewDataTextColumn colIdAuto = new GridViewDataTextColumn();
                colIdAuto.FieldName = "IdAuto";
                colIdAuto.Name = "IdAuto";
                colIdAuto.Caption = "IdAuto";
                colIdAuto.Visible = false;
                grDateCompetente.Columns.Add(colIdAuto);

                GridViewDataTextColumn colId = new GridViewDataTextColumn();
                colId.FieldName = "Id";
                colId.Name = "Id";
                colId.Caption = "Id";
                colId.Visible = false;
                grDateCompetente.Columns.Add(colId);

                GridViewDataTextColumn colIdQuiz = new GridViewDataTextColumn();
                colIdQuiz.FieldName = "IdQuiz";
                colIdQuiz.Name = "IdQuiz";
                colIdQuiz.Caption = "IdQuiz";
                colIdQuiz.Visible = false;
                grDateCompetente.Columns.Add(colIdQuiz);

                GridViewDataTextColumn colF10003 = new GridViewDataTextColumn();
                colF10003.FieldName = "F10003";
                colF10003.Name = "F10003";
                colF10003.Caption = "F10003";
                colF10003.Visible = false;
                grDateCompetente.Columns.Add(colF10003);

                GridViewDataTextColumn colPozitie = new GridViewDataTextColumn();
                colPozitie.FieldName = "Pozitie";
                colPozitie.Name = "Pozitie";
                colPozitie.Caption = "Pozitie";
                colPozitie.Visible = false;
                grDateCompetente.Columns.Add(colPozitie);
                #endregion

                #region AddColumns

                foreach (Eval_ConfigCompTemplateDetail clsConfigDetail in lstEval_ConfigCompTemplateDetail.Where(p=>p.TemplateId==TemplateIdCompetenta))
                {
                    if (clsConfigDetail.Vizibil == true)
                    {
                        #region colCompetenta
                        if (clsConfigDetail.ColumnName == "Competenta")
                        {
                            #region getDS
                            if (Session["feedEval_Competenta"] == null)
                            {
                                string strSQLCompetenta = @"
                                                            select categDet.""IdCompetenta"" as ""Id"", categDet.""DenCompetenta"" as ""Denumire""
                                                            from ""Eval_CategCompetente"" categ
                                                            join ""Eval_CategCompetenteDet"" categDet on categ.""IdCategorie"" = categDet.""IdCategorie""
                                                            join ""Eval_CompXSetAng"" setAng on categ.""IdCategorie"" = setAng.""IdCategorie""
                                                            join ""Eval_SetAngajatiDetail"" setAngDetail on setAng.""IdSetAng"" = setAngDetail.""IdSetAng""
                                                            where categ.""IdCategorie"" = @1
                                                            and setAngDetail.""Id"" = @2
                                                            group by categDet.""IdCompetenta"", categDet.""DenCompetenta""";
                                //strSQLCompetenta = string.Format(strSQLCompetenta, clsConfigDetail.IdNomenclator, _f10003);
                                DataTable dtCompetenta = General.IncarcaDT(strSQLCompetenta, new object[] { clsConfigDetail.IdNomenclator, _f10003 });
                                foreach (DataRow rwCompetenta in dtCompetenta.Rows)
                                {
                                    metaDate clsCompetenta = new metaDate();
                                    clsCompetenta.Id = Convert.ToInt32(rwCompetenta["Id"].ToString());
                                    clsCompetenta.Denumire = rwCompetenta["Denumire"].ToString();
                                    lstCompetente.Add(clsCompetenta);
                                }
                            }
                            else
                                lstCompetente = Session["feedEval_Competenta"] as List<metaDate>;

                            GridViewDataComboBoxColumn colCompetenta = new GridViewDataComboBoxColumn();
                            colCompetenta.FieldName = "IdCompetenta";
                            colCompetenta.Name = "IdCompetenta";
                            colCompetenta.Caption = Dami.TraduCuvant("Competenta");
                            colCompetenta.Width = clsConfigDetail.Width;

                            colCompetenta.PropertiesComboBox.TextField = "Denumire";
                            colCompetenta.PropertiesComboBox.ValueField = "Id";
                            //colObiectiv.PropertiesComboBox.ValueType = "System.Int32"; 
                            colCompetenta.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDown;
                            //colCompetenta.PropertiesComboBox.EnableCallbackMode = true;
                            colCompetenta.PropertiesComboBox.ValidationSettings.RequiredField.IsRequired = true;
                            colCompetenta.PropertiesComboBox.ValidationSettings.Display = Display.None;
                            //colCompetenta.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "cmbObiectiv_SelectedIndexChanged";
                            //colCompetenta.PropertiesComboBox.ClientInstanceName = "ObjectiveEditor";
                            colCompetenta.PropertiesComboBox.DataSource = lstCompetente;

                            grDateCompetente.Columns.Add(colCompetenta);
                            continue;

                            #endregion
                        }
                        #endregion

                        #region colCalificativ
                        if (clsConfigDetail.ColumnName == "Calificativ")
                        {

                            #region getDS
                            if (Session["feedEval_CompCalificativ"] == null || (Session["feedEval_CompCalificativ"] as List<Eval_SetCalificativDet>).Count <= 0)
                            {
                                string sqlCalificativ = @"select det.""IdSet"", det.""IdCalificativ"", det.""Denumire"", det.""Nota"", det.""RatingMin"", det.""RatingMax"", det.""Ordine"",
                                                                    det.""Explicatii""
                                                               from ""Eval_SetCalificativDet"" det
                                                                where det.""IdSet"" = @1 ";
                                //sqlCalificativ = string.Format(sqlCalificativ, clsConfigDetail.IdNomenclator);
                                DataTable dtCalificativ = General.IncarcaDT(sqlCalificativ, new object[] { clsConfigDetail.IdNomenclator });
                                foreach (DataRow rwCalificativ in dtCalificativ.Rows)
                                {
                                    Eval_SetCalificativDet clsCalificativ = new Eval_SetCalificativDet(rwCalificativ);
                                    lstEval_CompSetCalificativDet.Add(clsCalificativ);
                                }
                                Session["feedEval_CompCalificativ"] = lstEval_CompSetCalificativDet;
                            }
                            else
                                lstEval_CompSetCalificativDet = Session["feedEval_CompCalificativ"] as List<Eval_SetCalificativDet>;
                            #endregion

                            GridViewDataComboBoxColumn colCalificativ = new GridViewDataComboBoxColumn();
                            colCalificativ.FieldName = "IdCalificativ";
                            colCalificativ.Name = "IdCalificativ";
                            colCalificativ.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                            colCalificativ.Width = clsConfigDetail.Width;

                            colCalificativ.PropertiesComboBox.TextField = "Denumire";
                            colCalificativ.PropertiesComboBox.ValueField = "IdCalificativ";
                            colCalificativ.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDown;
                            colCalificativ.PropertiesComboBox.DataSource = lstEval_CompSetCalificativDet;

                            grDateCompetente.Columns.Add(colCalificativ);
                            continue;
                        }

                        #endregion

                        Eval_ConfigCompetente clsConfigCompetente = lstEval_ConfigCompetente.Where(p => p.ColumnName == clsConfigDetail.ColumnName).FirstOrDefault();
                        if (clsConfigCompetente == null)
                            continue;

                        switch (clsConfigCompetente.ColumnType)
                        {
                            case "System.String":
                                GridViewDataTextColumn colString = new GridViewDataTextColumn();
                                colString.FieldName = clsConfigDetail.ColumnName;
                                colString.Name = clsConfigDetail.ColumnName;
                                colString.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colString.Width = clsConfigDetail.Width;

                                grDateCompetente.Columns.Add(colString);
                                break;
                            case "System.Decimal":
                                GridViewDataTextColumn colDecimal = new GridViewDataTextColumn();
                                colDecimal.FieldName = clsConfigDetail.ColumnName;
                                colDecimal.Name = clsConfigDetail.ColumnName;
                                colDecimal.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colDecimal.Width = clsConfigDetail.Width;

                                colDecimal.PropertiesTextEdit.DisplayFormatString = "n2";
                                colDecimal.PropertiesTextEdit.MaskSettings.Mask = "<0..999g>.<0..999g>";

                                grDateCompetente.Columns.Add(colDecimal);
                                break;
                            case "System.Int32":
                                GridViewDataTextColumn colInt = new GridViewDataTextColumn();
                                colInt.FieldName = clsConfigDetail.ColumnName;
                                colInt.Name = clsConfigDetail.ColumnName;
                                colInt.Caption = Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colInt.Width = clsConfigDetail.Width;

                                colInt.PropertiesTextEdit.DisplayFormatString = "n0";
                                colInt.PropertiesTextEdit.MaskSettings.Mask = "<0..999g>";

                                grDateCompetente.Columns.Add(colInt);
                                break;
                        }
                    }
                }

                #endregion

                #region Grid Command Buttons
                grDateCompetente.SettingsCommandButton.EditButton.Image.Url = "../Fisiere/Imagini/Icoane/edit.png";
                grDateCompetente.SettingsCommandButton.EditButton.Image.AlternateText = "Edit";
                grDateCompetente.SettingsCommandButton.EditButton.Image.ToolTip = "Edit";

                grDateCompetente.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                grDateCompetente.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                grDateCompetente.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                grDateCompetente.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                grDateCompetente.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                grDateCompetente.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";


                grDateCompetente.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                grDateCompetente.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";

                #endregion


                //Florin 2019.01.29
                grDateCompetente.DataSource = lstEval_CompetenteAngajatTemp.Where(p => p.F10003 == _f10003 && p.IdLinieQuiz == id && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"])).ToList();
                ////Florin 2018.12.10
                //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                //    grDateCompetente.DataSource = lstEval_CompetenteAngajatTemp.Where(p => p.F10003 == F10003 && p.IdLinieQuiz == id && p.Pozitie == Convert.ToInt32(Session["Eval_PozitieUserLogat"])).ToList();
                //else
                //    grDateCompetente.DataSource = lstEval_CompetenteAngajatTemp.Where(p => p.F10003 == F10003 && p.IdLinieQuiz == id && p.Pozitie == Convert.ToInt32(super.Substring(5, 1))).ToList();
                grDateCompetente.KeyFieldName = "IdAuto";
                grDateCompetente.DataBind();

                //int poz = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());
                //if (idCateg == "0")
                //{
                //    //Florin 2019.01.29
                //    //if (Convert.ToInt32(Session["Eval_PozitieUserLogat"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //    if (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) != poz || Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1 || Convert.ToInt32(Session["CompletareChestionar_Modifica"].ToString()) == 0)
                //        grDateCompetente.Enabled = false;
                //}
                //else
                //{
                //    if (Convert.ToInt32(Session["Aprobat360"].ToString()) == 1)
                //        grDateCompetente.Enabled = false;

                //    //if (Convert.ToInt32(Session["CompletareChestionar_Finalizat"].ToString()) == 1)
                //    //    grDateCompetente.Enabled = false;
                //}

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        grDateCompetente.Enabled = false;
                //}


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return grDateCompetente;
        }

        private void GrDateCompetente_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
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

        private void GrDateCompetente_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                List<Eval_CompetenteAngajatTemp> dt = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                e.NewValues["IdAuto"] = dt.Count + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateCompetente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                ASPxGridView grid = sender as ASPxGridView;

                GridViewDataComboBoxColumn colCompetenta = (grid.Columns["IdCompetenta"] as GridViewDataComboBoxColumn);



                List<Eval_CompetenteAngajatTemp> lst = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;

                Eval_CompetenteAngajatTemp clsCompetenteAngajat = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                clsCompetenteAngajat.IdCompetenta = e.NewValues["IdCompetenta"] == null ? -99 : Convert.ToInt32(e.NewValues["IdCompetenta"]);
                clsCompetenteAngajat.Competenta = (colCompetenta != null ? colCompetenta.PropertiesComboBox.Items.FindByValue(clsCompetenteAngajat.IdCompetenta).Text : "");
                clsCompetenteAngajat.Pondere = e.NewValues["Pondere"] == null ? 0 : Convert.ToDecimal(e.NewValues["Pondere"].ToString());
                clsCompetenteAngajat.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                clsCompetenteAngajat.Calificativ = e.NewValues["Calificativ"] == null ? "" : e.NewValues["Calificativ"].ToString();
                clsCompetenteAngajat.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString();
                clsCompetenteAngajat.Explicatii = e.NewValues["Explicatii"] == null ? "" : e.NewValues["Explicatii"].ToString();

                //clsCompetenteAngajat.IdLinieQuiz = Convert.ToInt32(Session["CompletareChestionarCompetente_LinieQuiz"]);
                clsCompetenteAngajat.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                clsCompetenteAngajat.Pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());

                //Florin 2019.01.29
                clsCompetenteAngajat.Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);
                ////Florin 2018.12.10
                //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                //{
                //    clsCompetenteAngajat.Pozitie = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                //}

                clsCompetenteAngajat.IdQuiz = _idQuiz;
                clsCompetenteAngajat.F10003 = _f10003;

                e.Cancel = true;

                grid.CancelEdit();
                Session["lstEval_CompetenteAngajatTemp"] = lst;
                grid.DataSource = lst.Where(p => p.F10003 == clsCompetenteAngajat.F10003 && p.IdLinieQuiz == clsCompetenteAngajat.IdLinieQuiz && p.Pozitie == clsCompetenteAngajat.Pozitie && p.IdQuiz == clsCompetenteAngajat.IdQuiz).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateCompetente_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                ASPxGridView grid = sender as ASPxGridView;

                GridViewDataComboBoxColumn colCompetenta = (grid.Columns["IdCompetenta"] as GridViewDataComboBoxColumn);

                List<Eval_CompetenteAngajatTemp> lst = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                Eval_CompetenteAngajatTemp clsNew = new Eval_CompetenteAngajatTemp();
                //clsNew.IdAuto = lst.Count + 1;
                clsNew.IdAuto = Convert.ToInt32(General.Nz(lst.Max(p => (int?)p.IdAuto),0)) + 1;
                clsNew.F10003 = Convert.ToInt32(_f10003.ToString());
                clsNew.Pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString());


                //Florin 2019.01.29
                clsNew.Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);
                ////Florin 2018.12.10
                //if (Convert.ToInt32(_idClient) == 20 && idCateg != "0")
                //{
                //    clsNew.Pozitie = Convert.ToInt32(Session["Eval_PozitieUserLogat"]);
                //}

                clsNew.IdQuiz = _idQuiz;
                //clsNew.IdLinieQuiz = Convert.ToInt32(Session["CompletareChestionarCompetente_LinieQuiz"]);
                clsNew.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                clsNew.IdCompetenta = e.NewValues["IdCompetenta"] == null ? -99 : Convert.ToInt32(e.NewValues["IdCompetenta"]);
                clsNew.Competenta = (colCompetenta != null ? colCompetenta.PropertiesComboBox.Items.FindByValue(clsNew.IdCompetenta).Text : "").Replace("'","");
                clsNew.Pondere = e.NewValues["Pondere"] == null ? 0 : Convert.ToDecimal(e.NewValues["Pondere"].ToString());
                clsNew.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                clsNew.Calificativ = e.NewValues["Calificativ"] == null ? "" : e.NewValues["Calificativ"].ToString().Replace("'", "");
                clsNew.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString().Replace("'", "");
                clsNew.Explicatii = e.NewValues["Explicatii"] == null ? "" : e.NewValues["Explicatii"].ToString().Replace("'", "");

                lst.Add(clsNew);
                Session["lstEval_CompetenteAngajatTemp"] = lst;

                e.Cancel = true;

                grid.CancelEdit();
                grid.DataSource = lst.Where(p => p.F10003 == clsNew.F10003 && p.IdLinieQuiz == clsNew.IdLinieQuiz && p.Pozitie == clsNew.Pozitie && p.IdQuiz == clsNew.IdQuiz).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateCompetente_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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

        private void GrDateCompetente_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GrDateCompetente_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                ASPxGridView grid = sender as ASPxGridView;

                List<Eval_CompetenteAngajatTemp> lst = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;

                Eval_CompetenteAngajatTemp clsCompetenteAngajat = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                lst.Remove(clsCompetenteAngajat);
                Session["lstEval_CompetenteAngajatTemp"] = lst;
                e.Cancel = true;
                grid.DataSource = lst;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

        //Radu 22.01.2018
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(_idClient) == 20)
                    Session["PrintDocument"] = "EvaluarePeliFilip";
                else
                    Session["PrintDocument"] = "Evaluare";
                //string idQuiz = "-99", F10003 = "-99", poz = "1";
                //if (_idQuiz != null)
                //    idQuiz = _idQuiz.ToString();
                //if (_f10003 != null)
                //    F10003 = _f10003.ToString();

                //Florin 2019.01.29
                //if (Session["CompletareChestionar_Pozitie"] != null)
                //    //poz = Session["CompletareChestionar_Pozitie"].ToString();
                //    poz = (Convert.ToInt32(Session["Eval_ActiveTab"].ToString()) + 1).ToString();
                //if (Session["Eval_ActiveTab"] != null)
                //    poz = Session["Eval_ActiveTab"].ToString();


                Session["PrintParametrii"] = _idQuiz+ ";" + _f10003 + ";" + Session["UserId"] + ";Super" + (Session["Eval_ActiveTab"] ?? "1").ToString() + ";0";
                Session["PaginaWeb"] = "Eval/EvalDetaliu.aspx";
                Response.Redirect("~/Reports/Imprima.aspx", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void btnFinalizare_ItemClick(object sender, EventArgs e)
        {
            try
            {

                //string ras = ValidareDateObligatorii();
                string ras = "";
                if (ras != "")
                {                  
                    //MessageBox.Show(Dami.TraduCuvant(ras), MessageBox.icoError, "Atentie !");
                    pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }

                //int idQuiz = Convert.ToInt32(_idQuiz);
                //int F10003 = Convert.ToInt32(_f10003);
                //int pozitie = Convert.ToInt32(Session["CompletareChestionar_Pozitie"]);



                //Florin 2019.01.21
                //nu se mai doreste aceasta verificare


                #region Verificare minim 5 evaluari 360

                ////Florin 2018.12.03   se verifica daca exista minim 5 evaluari 360 (PeliFilip)

                //////Radu 28.08.2018 - se verifica daca exista minim 5 evaluari 360 (PeliFilip) atunci cand angajatul finalizeaza chestionarul anual
                ////if (Convert.ToInt32(_idClient) == 20 && (idCateg == "1" || idCateg == "2"))
                ////{
                ////    string sql = @"select distinct ""IdUser"" from ""Eval_Invitatie360"" where F10003 = {0} and ""IdStare"" = 3 and ""IdQuiz"" in 
                ////                (select ""Id"" from ""Eval_Quiz"" where ""CategorieQuiz"" = {2} and ""Anul"" = (select ""Anul"" from ""Eval_Quiz"" where ""Id"" = {1}))";
                ////    sql = string.Format(sql, F10003, idQuiz, idCateg);
                ////    DataTable dt = General.IncarcaDT(sql, null);
                ////    if (dt == null || dt.Rows.Count < 5)
                ////    {
                ////        //MessageBox.Show(Dami.TraduCuvant("Trebuie sa aveti minim 5 evaluari 360 pentru a putea finaliza!"), MessageBox.icoError, "Atentie !");
                ////        pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Trebuie sa aveti minim 5 evaluari 360 pentru a putea finaliza!");
                ////        return;
                ////    }
                ////}

                //if (Convert.ToInt32(_idClient) == 20 && idCateg == "0" && pozitie == 1)
                //{
                //    ////cautam chestionarul de feedback
                //    //int idQuiz_feedback = -99;
                //    //DataTable dtQuiz = General.IncarcaDT(
                //    //    @"SELECT TOP 1 A.Id
                //    //    FROM Eval_Quiz A
                //    //    INNER JOIN Eval_relGrupAngajatQuiz B ON A.Id=B.IdQuiz
                //    //    INNER JOIN Eval_SetAngajatiDetail C ON B.IdGrup=C.IdSetAng
                //    //    WHERE A.CategorieQuiz IN (1,2) AND C.Id = @1", new object[] { F10003 });

                //    //if (dtQuiz != null && dtQuiz.Rows.Count > 0) idQuiz_feedback = Convert.ToInt32(General.Nz(dtQuiz.Rows[0]["Id"], -99));

                //    //DataTable dt = General.IncarcaDT(@"SELECT DISTINCT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND COALESCE(""Aprobat"",0)=1", new object[] { idQuiz_feedback, F10003 });

                //    DataTable dt = General.IncarcaDT(
                //        @"SELECT DISTINCT IdUser FROM Eval_RaspunsIstoric 
                //        WHERE F10003 = @1 AND COALESCE(Aprobat,0)=1
                //        AND IdQuiz IN 
                //        (SELECT DISTINCT A.Id
                //        FROM Eval_Quiz A
                //        INNER JOIN Eval_relGrupAngajatQuiz B ON A.Id=B.IdQuiz
                //        INNER JOIN Eval_SetAngajatiDetail C ON B.IdGrup=C.IdSetAng
                //        WHERE A.CategorieQuiz IN (1,2) AND C.Id = @1)", new object[] { F10003 });
                //    if (dt == null || dt.Rows.Count < 5)
                //    {
                //        pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Trebuie sa aveti minim 5 evaluari 360 pentru a putea finaliza!");
                //        return;
                //    }
                //}

                #endregion  


                //se doreste ca, inainte de finalizare, sa se apeleze salvarea, deoarece sunt utilizatori care uita de salvare, iar la finalizare se pierd date
                //btnSave_Click(sender, e);



                string msg = QuizAproba(_idQuiz, _f10003, Convert.ToInt32(Session["UserId"].ToString()));
 
                if (msg.Length > 0)
                {
                    //MessageBox.Show(Dami.TraduCuvant(msg), MessageBox.icoError, "Atentie !");
                    pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                    return;                                    
                }
				
				msg = Notif.TrimiteNotificare("Eval.EvalLista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"" FROM ""Eval_Raspuns"" WHERE ""IdQuiz""=" + _idQuiz + @"AND F10003 = " + _f10003, "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
				if (msg.Length > 0)                    
					General.CreazaLog(msg);   
				
                string url = "~/Eval/EvalLista.aspx";
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        //private string ValidareDateObligatorii()
        //{
        //    string ras = "";
        //    string super = "Super" + poz;

        //    try
        //    {
        //        foreach (var l in lstIntr.Where(p => p.Obligatoriu == 1))
        //        {
        //            switch (l.TipData)
        //            {
        //                case 5:                 //competente
        //                case 45:                 //competente Zitec
        //                    {
        //                        var lst = lstRasp.Where(p => p.Id == l.Id);
        //                        foreach (var li in lst)
        //                        {
        //                            string val = "";
        //                            PropertyInfo pi = li.GetType().GetProperty(super);
        //                            if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                            if (val == "")
        //                            {
        //                                ras += ", competente";
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case 6:                 //obiective anterioare
        //                case 49:                //obiective anterioare OMN 2
        //                    {
        //                        switch (Constante.Id_Client)
        //                        {
        //                            case (int)IdClienti.Clienti.Omniasig:
        //                                {
        //                                    int nr = lstRasp.Where(p => p.Id == l.Id).Count();
        //                                    if (nr < 2 || nr > 4) ras += ", minim 2 obiective si maxim 4";

        //                                    int pro = 0;
        //                                    bool are = false;

        //                                    //verif. daca sunt completate argumente si calificative
        //                                    var lst = lstRasp.Where(p => p.Id == l.Id);
        //                                    foreach (var li in lst)
        //                                    {
        //                                        if (!are)
        //                                        {
        //                                            //argumentul
        //                                            string val = "";
        //                                            PropertyInfo pi = li.GetType().GetProperty(super + "_1");
        //                                            if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                            if (val.Trim() == "")
        //                                            {
        //                                                ras += ", aveti obiectiv pentru care nu s-a completat argumentul";
        //                                                are = true;
        //                                            }

        //                                            //calificativul
        //                                            string cal = "";
        //                                            PropertyInfo piCal = li.GetType().GetProperty(super);
        //                                            if (piCal != null) cal = (piCal.GetValue(li, null) ?? "").ToString();
        //                                            if (cal.Trim() == "")
        //                                            {
        //                                                ras += ", aveti obiectiv pentru care nu s-a completat calificativul";
        //                                                are = true;
        //                                            }

        //                                        }

        //                                        if ((li.Tinta ?? "").Trim() != "")
        //                                        {
        //                                            try
        //                                            {
        //                                                pro += Convert.ToInt32(li.Tinta);
        //                                            }
        //                                            catch (Exception) { }
        //                                        }
        //                                    }

        //                                    //verif. suma procentelor

        //                                    if (idQuiz == 3)
        //                                    {
        //                                        string categ = "Management L1";
        //                                        var entCateg = lstRasp.Where(p => p.TipData == 38).FirstOrDefault();
        //                                        if (entCateg != null && entCateg.Super1 != null) categ = entCateg.Super1;

        //                                        if (categ.ToUpper().IndexOf("MANAGEMENT L1") >= 0 && pro != 60) ras += ", suma procentelor obi. individuale trebuie sa fie 60";
        //                                        if (categ.ToUpper().IndexOf("MANAGEMENT L2") >= 0 && pro != 70) ras += ", suma procentelor obi. individuale trebuie sa fie 70";
        //                                        if ((categ.ToUpper().IndexOf("EXPERT") >= 0 || categ.ToUpper().IndexOf("EXECUTIE") >= 0) && pro != 80) ras += ", suma procentelor obi. individuale trebuie sa fie 80";
        //                                    }
        //                                    else
        //                                        if (pro != 100) ras += ", suma ponderilor in obiective nu este 100%";
        //                                }
        //                                break;
        //                            case (int)IdClienti.Clienti.Zitec:
        //                                {
        //                                    var lst = lstRasp.Where(p => p.Id == l.Id);
        //                                    foreach (var li in lst)
        //                                    {
        //                                        string val = "";
        //                                        PropertyInfo pi = li.GetType().GetProperty(super);
        //                                        if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                        if (val == "")
        //                                        {
        //                                            ras += ", obiective anterioare";
        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                break;
        //                            default:
        //                                {
        //                                    int suma = 0;
        //                                    var lst = lstRasp.Where(p => p.Id == l.Id);
        //                                    foreach (var li in lst)
        //                                    {
        //                                        try
        //                                        {
        //                                            suma += Convert.ToInt32(li.Tinta ?? "0");
        //                                        }
        //                                        catch (Exception) { }
        //                                    }
        //                                    if (suma != 100) ras += ", suma ponderilor in obiective nu este 100%";

        //                                    if (l.TipData == 12)
        //                                    {
        //                                        int nr = lstRasp.Where(p => p.Id == l.Id).Count();
        //                                        if (nr < 3) ras += ", minim 3 obiective";
        //                                    }
        //                                }
        //                                break;
        //                        }

        //                    }
        //                    break;
        //                case 12:                //obiective viitoare
        //                    {
        //                        switch (Constante.Id_Client)
        //                        {
        //                            case (int)IdClienti.Clienti.Omniasig:
        //                                {
        //                                    int nr = lstRasp.Where(p => p.Id == l.Id).Count();
        //                                    if (nr < 2 || nr > 4) ras += ", minim 2 obiective si maxim 4";

        //                                    int pro = 0;
        //                                    bool are = false;

        //                                    //verif. daca sunt completate argumente
        //                                    var lst = lstRasp.Where(p => p.Id == l.Id);
        //                                    foreach (var li in lst)
        //                                    {
        //                                        if (!are)
        //                                        {
        //                                            string val = "";
        //                                            PropertyInfo pi = li.GetType().GetProperty(super);
        //                                            if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                            if (val.Trim() == "")
        //                                            {
        //                                                ras += ", aveti obiectiv pentru care nu s-a completat argumentul";
        //                                                are = true;
        //                                            }
        //                                        }

        //                                        //Radu 03.08.2017
        //                                        for (int i = 1; i <= 4; i++)
        //                                        {
        //                                            if (!are)
        //                                            {
        //                                                string val = "";
        //                                                PropertyInfo pi = li.GetType().GetProperty(super + "_" + i.ToString());
        //                                                if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                                if (val.Trim() == "")
        //                                                {
        //                                                    ras += ", aveti obiectiv pentru care nu s-a completat gradul de realizare " + i.ToString();
        //                                                    are = true;
        //                                                }
        //                                            }
        //                                        }

        //                                        if ((li.Tinta ?? "").Trim() != "")
        //                                        {
        //                                            try
        //                                            {
        //                                                pro += Convert.ToInt32(li.Tinta);
        //                                            }
        //                                            catch (Exception) { }
        //                                        }
        //                                    }

        //                                    //verif. suma procentelor

        //                                    if (idQuiz == 3)
        //                                    {
        //                                        string categ = "Management L1";
        //                                        var entCateg = lstRasp.Where(p => p.TipData == 38).FirstOrDefault();
        //                                        if (entCateg != null && entCateg.Super1 != null) categ = entCateg.Super1;

        //                                        if (categ.ToUpper().IndexOf("MANAGEMENT L1") >= 0 && pro != 60) ras += ", suma procentelor obi. individuale trebuie sa fie 60";
        //                                        if (categ.ToUpper().IndexOf("MANAGEMENT L2") >= 0 && pro != 70) ras += ", suma procentelor obi. individuale trebuie sa fie 70";
        //                                        if ((categ.ToUpper().IndexOf("EXPERT") >= 0 || categ.ToUpper().IndexOf("EXECUTIE") >= 0) && pro != 80) ras += ", suma procentelor obi. individuale trebuie sa fie 80";
        //                                    }
        //                                    else
        //                                        if (pro != 100) ras += ", suma ponderilor in obiective nu este 100%";
        //                                }
        //                                break;
        //                            default:
        //                                {
        //                                    int suma = 0;
        //                                    var lst = lstRasp.Where(p => p.Id == l.Id);
        //                                    foreach (var li in lst)
        //                                    {
        //                                        try
        //                                        {
        //                                            suma += Convert.ToInt32(li.Tinta ?? "0");
        //                                        }
        //                                        catch (Exception) { }
        //                                    }
        //                                    if (suma != 100) ras += ", suma ponderilor in obiective nu este 100%";

        //                                    if (l.TipData == 12)
        //                                    {
        //                                        int nr = lstRasp.Where(p => p.Id == l.Id).Count();
        //                                        if (nr < 3) ras += ", minim 3 obiective";
        //                                    }
        //                                }
        //                                break;
        //                        }
        //                    }
        //                    break;
        //                case 22:            //Plan de dezvoltare
        //                    {
        //                        int nr = lstRasp.Where(p => p.Id == l.Id).Count();
        //                        if (nr < 1) ras += ", minim 1 obiectiv de dezvoltare";
        //                    }
        //                    break;
        //                case 23:            //Obiective Angajat
        //                    {
        //                        if (Constante.Id_Client == Convert.ToInt32(Module.IdClienti.Clienti.Omniasig))
        //                        {
        //                            var lst = lstRasp.Where(p => p.Id == l.Id);
        //                            foreach (var li in lst)
        //                            {
        //                                string val = "";
        //                                PropertyInfo pi = li.GetType().GetProperty(super + "_1");
        //                                if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                if (val.Trim() == "")
        //                                {
        //                                    ras += ", aveti obiectiv pentru care nu s-a completat argumentul";
        //                                    break;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            int nrTotal = 0;
        //                            int nrZero = 0;
        //                            var lst = lstRasp.Where(p => p.Id == l.Id);
        //                            foreach (var li in lst)
        //                            {
        //                                string val = "";
        //                                PropertyInfo pi = li.GetType().GetProperty(super);
        //                                if (pi != null) val = (pi.GetValue(li, null) ?? "").ToString();
        //                                if (val == "")
        //                                {
        //                                    ras += ", aveti obiective pentru care nu a fost acordata nota";
        //                                    break;
        //                                }
        //                                if (val == "0")
        //                                {
        //                                    nrZero += 1;
        //                                }
        //                                nrTotal += 1;
        //                            }

        //                            if (nrZero != 0 && nrTotal != nrZero) ras += ", exista obiectiv cu nota 0; va rugam revizuiti nota obiectiv";
        //                        }
        //                    }
        //                    break;
        //                case 32:                //competente pt OMN
        //                    {
        //                        bool are = false;
        //                        bool stop = false;
        //                        bool una = false;
        //                        decimal rez = 0;

        //                        var lst = lstRasp.Where(p => p.Id == l.Id);
        //                        foreach (var li in lst)
        //                        {
        //                            string arg = "";
        //                            PropertyInfo piArg = li.GetType().GetProperty(super + "_1");
        //                            if (piArg != null) arg = (piArg.GetValue(li, null) ?? "").ToString();

        //                            if (arg == "" && una == false)
        //                            {
        //                                ras += ", exista argumente necompletate";
        //                                una = true;
        //                            }

        //                            if (poz == 2)
        //                            {
        //                                try
        //                                {
        //                                    rez += Convert.ToDecimal(li.Tinta) * Convert.ToDecimal(li.Super2);
        //                                }
        //                                catch (Exception) { }

        //                                string rat = "";
        //                                PropertyInfo pi = li.GetType().GetProperty(super);
        //                                if (pi != null) rat = (pi.GetValue(li, null) ?? "").ToString();

        //                                string obi = "";
        //                                PropertyInfo piObi = li.GetType().GetProperty(super + "_2");
        //                                if (piObi != null) obi = (piObi.GetValue(li, null) ?? "").ToString();

        //                                string dt = "";
        //                                PropertyInfo piDt = li.GetType().GetProperty(super + "_3");
        //                                if (piDt != null) dt = (piDt.GetValue(li, null) ?? "").ToString();

        //                                if (!stop && (rat == "1" || rat == "2" || rat == "3") && (obi == "" || dt == ""))
        //                                {
        //                                    ras += ", obiectiv de dezvoltare si/sau termen finalizare";
        //                                    //break;
        //                                    stop = true;
        //                                }

        //                                if (obi != "") are = true;
        //                            }
        //                        }

        //                        if (poz == 2)
        //                        {
        //                            bool verif = false;
        //                            //decimal rez = CalculeazaTotalOMN2();
        //                            rez = Math.Round(rez / 6, 2);
        //                            var ent = lstRasp.Where(p => p.IdQuiz == idQuiz && p.F10003 == f10003 && p.TipData == 38).FirstOrDefault();
        //                            string categ = "management";
        //                            if (ent != null && ent.Super1 != null) categ = (ent.Super1 ?? "").ToString();

        //                            if (categ.IndexOf("management") >= 0)
        //                            {
        //                                if (rez <= 80) verif = true;
        //                            }
        //                            else
        //                            {
        //                                if (rez <= 70) verif = true;
        //                            }

        //                            if (verif)
        //                            {
        //                                if (!are)
        //                                {
        //                                    ras += ", trebuie completat cel putin un obiectiv de dezvoltare deoarece procentajul de excelenta este sub limita";
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;
        //                default:
        //                    {
        //                        string val = "";
        //                        var ent = lstRasp.Where(p => p.Id == l.Id).FirstOrDefault();
        //                        if (ent != null)
        //                        {
        //                            PropertyInfo pi = ent.GetType().GetProperty(super);
        //                            if (pi != null) val = (pi.GetValue(ent, null) ?? "").ToString();
        //                        }
        //                        if (val == "" && l.Descriere.Trim() != "") ras += ", " + l.Descriere;
        //                    }
        //                    break;
        //            }
        //        }

        //        if (ras != "") ras = Dami.TraduCuvant("Lipsesc date:") + " " + ras.Substring(2);
        //    }
        //    catch (Exception)
        //    {
        //    }

        //    return ras;
        //}

        //public string QuizAproba(int idQuiz, int f10003, int idUser)
        public string QuizAproba()
        {
            string msg = "";

            try
            {
                //Florin 2018.09.25
                //verificam daca este deja aprobat

                string strSql = @"SELECT COUNT(*) FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND COALESCE(""Aprobat"",0) = 1 AND ""IdUser"" = @3";
                int este = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, new object[] { _idQuiz, _f10003, Session["UserId"] } ),0));
                if (este == 1)
                    return "Formularul este deja aprobat";

                //string sql = "";                
                //sql = "SELECT * FROM ""Eval_Quiz"" WHERE ""Id"" = " +idQuiz;
                DataTable entQz = General.IncarcaDT(@"SELECT * FROM ""Eval_Quiz"" WHERE ""Id"" = @1", new object[] { _idQuiz });

                //sql = "SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz"" = " + idQuiz + " AND F10003 = " + f10003;
                DataTable ent = General.IncarcaDT(@"SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz"" = @1 AND F10003 = @2", new object[] { _idQuiz, _f10003 });
                //sql = "SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = " + idQuiz + " AND F10003 = " + f10003;
                DataTable lstIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2", new object[] { _idQuiz, _f10003 });
                //sql = "SELECT * FROM ""Eval_RaspunsIstoric"" a WHERE ""IdQuiz"" = " + idQuiz + " AND F10003 = " + f10003 + " AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0) "  
                //    + " AND ((""IdSuper"" >= 0 AND ""IdUser"" = " + idUser + ") OR (a.""IdSuper"" < 0 AND " + idUser + " in (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = " 
                //    + f10003 + " AND b.""IdSuper"" = (-1) * a.""IdSuper""))) ";
                DataTable entIst = General.IncarcaDT(
                    @"SELECT * FROM ""Eval_RaspunsIstoric"" a 
                    WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0)
                    AND ((""IdSuper"" >= 0 AND ""IdUser"" = @3) OR (a.""IdSuper"" < 0 AND @3 in (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 =
                    @2 AND b.""IdSuper"" = (-1) * a.""IdSuper""))) ORDER BY ""Pozitie"" ", new object[] { _idQuiz, _f10003, Session["UserId"] });       

                if (ent == null || ent.Rows.Count <= 0 || entIst == null || entIst.Rows.Count <= 0) return "Date incomplete in istoric.";

                int respectaOrdinea = 0;
                //sql = "SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = " + idQuiz;
                DataTable entCir = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = @1", new object[] { _idQuiz });
                if (entCir != null && entCir.Rows.Count > 0) respectaOrdinea = Convert.ToInt32(entCir.Rows[0]["RespectaOrdinea"] != DBNull.Value ? entCir.Rows[0]["RespectaOrdinea"].ToString() : "0"); 

                //verificam daca se respecta ordinea din circuit
                if (respectaOrdinea == 1 && Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") != Convert.ToInt32(ent.Rows[0]["Pozitie"] != DBNull.Value ? ent.Rows[0]["Pozitie"].ToString() : "-99")) return "Nu puteti aproba in acest moment.";
                
                //cautam urmatoarea pozitie din istoric care nu a aprobat
                int poz = Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "0") + 1;
                //sql = "SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = " + idQuiz + " AND F10003 = " + f10003 + " AND ""Aprobat"" IS NULL AND ""Pozitie"" = " + (entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") + " ORDER BY ""Pozitie""";
                DataTable entPoz = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Aprobat"" IS NULL AND ""Pozitie"" = @3 ORDER BY ""Pozitie"" ", new object[] { _idQuiz, _f10003, (entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") });
                if (entPoz != null && entPoz.Rows.Count > 0) poz = Convert.ToInt32(entPoz.Rows[0]["Pozitie"] != DBNull.Value ? entPoz.Rows[0]["Pozitie"].ToString() : "0") + 1;
                string culoare = DamiEvalCuloare(poz);

                if (poz > Convert.ToInt32(ent.Rows[0]["TotalCircuit"].ToString()))
                {
                    poz--;
                    ent.Rows[0]["Finalizat"] = 1;
                    Session["CompletareChestionar_Finalizat"] = 1;
                }

                ent.Rows[0]["Pozitie"] = poz;
                ent.Rows[0]["Culoare"] = culoare;

                #region Finalizeaza

                bool fin = true;
                for (int i = 0; i < lstIst.Rows.Count; i++)
                {
                    if (Convert.ToInt32(lstIst.Rows[i]["Aprobat"] != DBNull.Value ? lstIst.Rows[i]["Aprobat"].ToString() : "-99") != 1)
                    {
                        fin = false;
                        break;
                    }
                }

                if (fin && ent != null && ent.Rows.Count > 0)
                {
                    ent.Rows[0]["Finalizat"] = 1;
                    Session["CompletareChestionar_Finalizat"] = 1;
                    ent.Rows[0]["Culoare"] = "#ffc8ffc8";
                }

                #endregion

                //daca in chestionar exista tipul de camp 36 - Data Finalizare
                //sql = "SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = " + idQuiz + " AND F10003 = " + f10003 + " AND ""TipData"" = 36";
                DataTable entDtFin = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""TipData"" = 36", new object[] { _idQuiz, _f10003 });
                if (entDtFin != null && entDtFin.Rows.Count > 0)
                {
                    string sql = @"UPDATE ""Eval_RaspunsLinii"" SET ";
                    for (int i = 1; i <= 20; i++)
                    {                        
                        sql += " \"Super" + i + "\" = " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                        if (i < 20)
                            sql += ", ";
                    }
                    sql += "WHERE \"IdQuiz\" = @1 AND F10003 = @2 AND \"TipData\" = 36";
                    General.IncarcaDT(sql, new object[] { _idQuiz, _f10003 });
                }

                //sql = "UPDATE ""Eval_Raspuns"" SET ""Pozitie"" = {0}, ""Culoare"" = '{1}', ""Finalizat"" = {2} WHERE ""IdQuiz"" = {3} AND F10003 = {4}";
                //sql = string.Format(sql, ent.Rows[0]["Pozitie"].ToString(), ent.Rows[0]["Culoare"].ToString(), (ent.Rows[0]["Finalizat"] != DBNull.Value ? ent.Rows[0]["Finalizat"].ToString() : "0").ToString(), idQuiz, f10003);
                General.IncarcaDT(@"UPDATE ""Eval_Raspuns"" SET ""Pozitie"" = @1, ""Culoare"" = @2, ""Finalizat"" = @3 WHERE ""IdQuiz"" = @4 AND F10003 = @5", new object[] { ent.Rows[0]["Pozitie"].ToString(), ent.Rows[0]["Culoare"].ToString(), (ent.Rows[0]["Finalizat"] != DBNull.Value ? ent.Rows[0]["Finalizat"].ToString() : "0").ToString(), _idQuiz, _f10003 });

                //sql = "UPDATE ""Eval_RaspunsIstoric"" SET ""DataAprobare"" = {0}, ""Aprobat"" = 1, ""Culoare"" = '{1}', USER_NO = {2}, TIME = {3} WHERE ""IdQuiz"" = {4} AND F10003 = {5} AND ((""IdSuper"" >= 0 AND ""IdUser"" = {6}) OR " 
                //    + "(""IdSuper"" < 0 AND {6} IN (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = {5} AND b.""IdSuper"" = (-1) * ""Eval_RaspunsIstoric"".""IdSuper"" ))) AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0)";
                //sql = string.Format(sql, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE") , culoare, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idQuiz, f10003, idUser);
                General.IncarcaDT(
                    $@"UPDATE ""Eval_RaspunsIstoric"" SET ""DataAprobare"" = {General.CurrentDate()}, ""Aprobat"" = 1, ""Culoare"" = @2, USER_NO = @3, TIME = {General.CurrentDate()} WHERE ""IdQuiz"" = @5 AND F10003 = @6 AND ((""IdSuper"" >= 0 AND ""IdUser"" = @7) OR
                    (""IdSuper"" < 0 AND @7 IN (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = @6 AND b.""IdSuper"" = (-1) * ""Eval_RaspunsIstoric"".""IdSuper"" ))) AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0)", new object[] { (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), culoare, Session["UserId"], (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), _idQuiz, _f10003, Session["UserId"] });


                //Radu 05.09.2018 - la CLAIM, trebuie suprascris IdUser de pe pozitia 2 (HR) cu cel al utilizatorului care finalizeaza efectiv
                if (Convert.ToInt32(_idClient) == 21 && Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1 == 2 && !fin)
                {
                    //sql = "UPDATE ""Eval_RaspunsIstoric"" SET ""IdUser"" = {0} WHERE ""IdQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = 2 ";
                    //sql = string.Format(sql, idUser, idQuiz, f10003);
                    General.IncarcaDT(@"UPDATE ""Eval_RaspunsIstoric"" SET ""IdUser"" = @1 WHERE ""IdQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" = 2 ", new object[] { Session["UserId"], _idQuiz, _f10003 });
                }


                ////obiective individuale
                //if (ent.Rows[0]["Finalizat"].ToString() != "1")
                //{
                //    //sql = "SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = " + idQuiz + " AND ""TipData"" = 23";
                //    DataTable dtObiective = General.IncarcaDT(@"SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""TipData"" = 23", new object[] { idQuiz });

                //    if (dtObiective != null && dtObiective.Rows.Count > 0)
                //    {
                //        for (int i = 0; i < dtObiective.Rows.Count; i++)
                //        {
                //            //sql = "DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} ";
                //            //sql = string.Format(sql, dtObiective.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), idQuiz);
                //            General.IncarcaDT(@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 ", new object[] { dtObiective.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), idQuiz });

                //            Session["lstEval_ObiIndividualeTemp"] = null;

                //            int idAuto = DamiIdObi();
                //            //sql = "INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"") "
                //            //    + " SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"" "
                //            //    + " FROM ""Eval_ObiIndividualeTemp"" WHERE "
                //            //    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} ";
                //            ////sql = "INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""IdAuto"") "
                //            ////    + " SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"", "
                //            ////    + " {5} "
                //            ////    + " FROM ""Eval_ObiIndividualeTemp"" WHERE "
                //            ////    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} ";
                //            //sql = string.Format(sql, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), dtObiective.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1, idQuiz, idAuto);
                //            General.IncarcaDT(
                //                $@"INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"")
                //                SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Descriere"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString())}, ""IdLinieQuiz""
                //                FROM ""Eval_ObiIndividualeTemp"" WHERE
                //                ""IdLinieQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" = @4 AND ""IdQuiz"" = @5 ", new object[] { Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), dtObiective.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1, idQuiz });
                //        }
                //    }
                //}


                //competenta
                if (ent.Rows[0]["Finalizat"].ToString() != "1")
                {
                    //sql = "SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = " + idQuiz + " AND ""TipData"" = 5";
                    DataTable dtCompetente = General.IncarcaDT(@"SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""TipData"" = 5", new object[] { _idQuiz });

                    if (dtCompetente != null && dtCompetente.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtCompetente.Rows.Count; i++)
                        {
                            //sql = "DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} ";
                            //sql = string.Format(sql, dtCompetente.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), idQuiz);
                            General.IncarcaDT(@"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 ", new object[] { dtCompetente.Rows[i][0].ToString(), _f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), _idQuiz });

                            Session["lstEval_CompetenteAngajatTemp"] = null;

                            //int idAuto = DamiIdObi();

                            string sql = $@"INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"")
                                SELECT ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString())}, ""IdLinieQuiz""
                                FROM ""Eval_CompetenteAngajatTemp"" WHERE
                                ""IdLinieQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" = @4 AND ""IdQuiz"" = @5 ";

                            //sql = "INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""IdAuto"") "
                            //    + " SELECT ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"", "
                            //    + " {5} "
                            //    + " FROM ""Eval_CompetenteAngajatTemp"" WHERE "
                            //    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} ";
                            //sql = string.Format(sql, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), dtCompetente.Rows[i][0].ToString(), f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1, idQuiz, idAuto);
                            General.IncarcaDT(sql, new object[] { Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), dtCompetente.Rows[i][0].ToString(), _f10003, Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1, _idQuiz });
                        }
                    }
                }

                if (entQz != null && entQz.Rows.Count > 0 && Convert.ToInt32(entQz.Rows[0]["Preluare"] != DBNull.Value ? entQz.Rows[0]["Preluare"].ToString() : "0") == 1 && Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") < Convert.ToInt32(ent.Rows[0]["TotalCircuit"].ToString()))
                    QuizPreluareDate(_idQuiz, _f10003, Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99"));

                //msg = "Proces finalizat cu succes.";

                //if (entQz != null && entQz.Rows.Count > 0 && Convert.ToInt32(entQz.Rows[0]["TipQuiz"].ToString()) == 3)
                //{
                //    //Notif.TrimiteNotificare("Eval.EvalLista360", "grDate", ent, idUser, f10003);
                //}
                //else
                //{
                //    //Notif.TrimiteNotificare("Eval.EvalLista", "grDate", ent, idUser, f10003);
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return msg;

        }

        private string DamiEvalCuloare(int poz)
        {
            string culoare = "#FFFFFFFF";

            try
            {
                //string sql = "SELECT * FROM ""Eval_CircuitCulori"" WHERE ""Pozitie"" = " + poz;
                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_CircuitCulori"" WHERE ""Pozitie"" = @1", new object[] { poz });
                if (dt != null && dt.Rows.Count > 0)
                {
                    culoare = (dt.Rows[0]["Culoare"] != DBNull.Value ? dt.Rows[0]["Culoare"].ToString() : "").ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return culoare;
        }

        private string DamiDataString(DateTime? dt)
        {
            string rez = "";

            try
            {
                if (dt != null) rez = dt.Value.Day.ToString().PadLeft(2, '0') + "/" + dt.Value.Month.ToString().PadLeft(2, '0') + "/" + dt.Value.Year;
            }
            catch (Exception)
            {
            }

            return rez;
        }


        private void QuizPreluareDate(int poz)
        {
            try
            {
                //string strSql = "UPDATE ""Eval_RaspunsLinii"" SET ""Super" + (poz + 1).ToString() + """=""Super" + poz + """, ""Super" + (poz + 1).ToString() + "_1""=""Super" + poz + "_1"", ""Super" + (poz + 1).ToString() + "_2""=""Super" + poz + "_2"", ""Super" + (poz + 1).ToString() + "_3""=""Super" + poz + "_3"", ""Super" + (poz + 1).ToString() + "_4""=""Super" + poz + "_4""   WHERE ""IdQuiz""=" + idQuiz + " AND F10003=" + f10003 + " AND ""Super" + (poz + 1).ToString() + """ is null AND ""Super" + poz + """ is not null";
                General.IncarcaDT(
                    $@"UPDATE ""Eval_RaspunsLinii"" 
                    SET 
                    ""Super{poz+1}""=""Super{poz}"", 
                    ""Super{poz+1}_1""=""Super{poz}_1"", 
                    ""Super{poz+1}_2""=""Super{poz}_2"", 
                    ""Super{poz+1}_3""=""Super{poz}_3"", 
                    ""Super{poz+1}_4""=""Super{poz}_4""   
                    WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Super{poz+1}"" is null AND ""Super{poz}"" is not null", new object[] { _idQuiz, _f10003 });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void GetActivitati()
        {
            try
            {
                if (Session["feedEval_ObiectivActivitate"] == null)
                {
                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", obAct.""Activitate"" as ""Denumire""
                                                                            from ""Eval_ListaObiectivDet"" det
                                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                            join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                    and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                            where setAng.""Id"" = @1
                                                                            group by ob.""IdObiectiv"", obAct.""IdActivitate"", obAct.""Activitate"" ";
                    strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, _f10003);
                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { _f10003 });
                    lstActivitati.Clear();
                    foreach (DataRow rwObiectivActivitate in dtObiectivActivitate.Rows)
                    {
                        metaDate clsObiectivActivitate = new metaDate();
                        clsObiectivActivitate.Id = Convert.ToInt32(rwObiectivActivitate["Id"]);
                        clsObiectivActivitate.Denumire = rwObiectivActivitate["Denumire"].ToString();
                        clsObiectivActivitate.ParentId = Convert.ToInt32(rwObiectivActivitate["Parinte"].ToString());
                        lstActivitati.Add(clsObiectivActivitate);
                    }
                }
                else
                    lstActivitati = Session["feedEval_ObiectivActivitate"] as List<metaDate>;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private ASPxGridView CreazaTabel(int id, int parinte, string numeView)
        {
            ASPxGridView grDate = new ASPxGridView();

            try
            {
                if (numeView.Trim() == "") return grDate;

                //Radu 19.07.2018
                //DataTable dt = General.IncarcaDT($@"SELECT * FROM {numeView} WHERE F10003=@1", new object[] { _f10003 });
                //string sql = @"SELECT ""Id"", ""Descriere"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = {0} AND ""Parinte"" = {1} AND (""Descriere"" like '%Vitality%' OR ""Descriere"" like '%Effectiveness%' OR ""Descriere"" like '%Common Sense%' OR ""Descriere"" like '%Creativity%' OR ""Descriere"" like '%Relationships%')";
                //sql = string.Format(sql, _idQuiz.ToString(), parinte);
                DataTable dtIntreb = General.IncarcaDT(@"SELECT ""Id"", ""Descriere"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""Parinte"" = @2 AND (""Descriere"" like '%Vitality%' OR ""Descriere"" like '%Effectiveness%' OR ""Descriere"" like '%Common Sense%' OR ""Descriere"" like '%Creativity%' OR ""Descriere"" like '%Relationships%' OR ""Descriere"" like '%Others%')", new object[] { _idQuiz, parinte });
                string descriere = "";
                for (int i = 0; i < dtIntreb.Rows.Count - 1; i++)
                    if (Convert.ToInt32(dtIntreb.Rows[i]["Id"].ToString()) < id && id < Convert.ToInt32(dtIntreb.Rows[i + 1]["Id"].ToString()))
                    {
                        descriere = dtIntreb.Rows[i]["Descriere"].ToString();
                        break;
                    }
                if (descriere.Length <= 0)
                    descriere = dtIntreb.Rows[dtIntreb.Rows.Count - 1]["Descriere"].ToString();

                string op = "+";
                if (Constante.tipBD == 2) op = "||";
                //sql = "SELECT * FROM ""{0}"" WHERE F10003 = {1} AND '{2}' LIKE '%' {3} ""Descriere"" {3} '%' ";
                //sql = string.Format(sql, numeView, _f10003.ToString(), descriere, op);
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""{numeView}"" WHERE F10003 = @1 AND '{descriere}' LIKE '%' {Dami.Operator()} ""Descriere"" {Dami.Operator()} '%' ", new object[] { _f10003.ToString() });

                grDate.AutoGenerateColumns = true;
                grDate.DataSource = dt;
                grDate.Width = Unit.Percentage(100);
                grDate.ID = "grDate_DinView_" + id;
                //grDate.KeyFieldName = "IdAuto";
                grDate.DataBind();

                if (grDate.Columns["IdQuiz"] != null)
                {
                    grDate.Columns["IdQuiz"].Visible = false;
                    grDate.Columns["IdQuiz"].ShowInCustomizationForm = false;

                }
                if (grDate.Columns["F10003"] != null)
                {
                    grDate.Columns["F10003"].Visible = false;
                    grDate.Columns["F10003"].ShowInCustomizationForm = false;
                }
                if (grDate.Columns["Descriere"] != null)
                {
                    grDate.Columns["Descriere"].Visible = false;
                    grDate.Columns["Descriere"].ShowInCustomizationForm = false;
                }

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        grDate.Enabled = false;
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return grDate;
        }

        private ASPxGridView CreazaTabelOthers()
        {
            ASPxGridView grDate = new ASPxGridView();

            try
            {
                DataTable dt = General.IncarcaDT($@"SELECT ""PuncteForte"" FROM ""viewEvaluare360"" WHERE F10003 = @1 AND ""Descriere"" LIKE '%Others%' ", new object[] { _f10003.ToString() });

                grDate.AutoGenerateColumns = true;
                grDate.DataSource = dt;
                grDate.Width = Unit.Percentage(100);
                grDate.ID = "grDate_DinView_Others";
                grDate.DataBind();

                //if (Convert.ToInt32(_idClient) == 21)
                //{//Radu 07.09.2018 - pt. CLAIM, angajatul, care este pe ultima pozitie, sa nu poata modifica nimic
                //    if (Convert.ToInt32(Session["CompletareChestionar_Pozitie"].ToString()) == 3)
                //        grDate.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return grDate;
        }

        private void PreluareDateAutomat(int pozitie)
        {
            try
            {
                string sqlUpd = "";
                string sablon =
                    @"UPDATE ""Eval_RaspunsLinii"" SET Super@pozIst=Super@poz
                    WHERE ""IdQuiz""=@1 AND F10003=@2 AND Super@poz IS NOT NULL AND Super@poz<>'' AND Super@pozIst<>Super@poz;";

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>@3", new object[] { _idQuiz, _f10003, pozitie });
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    if (General.Nz(dt.Rows[i]["Pozitie"], "").ToString() != "")
                        sqlUpd += sablon.Replace("@pozIst", General.Nz(dt.Rows[i]["Pozitie"], 1).ToString()).Replace("@poz", pozitie.ToString()) + Environment.NewLine;
                }

                string strSql =
                    $@"
                    BEGIN

                    {sqlUpd}

                    UPDATE B
                    SET B.Obiectiv=A.Obiectiv, B.Activitate=A.Activitate, B.Descriere=A.Descriere, B.ColoanaSuplimentara1=A.ColoanaSuplimentara1, B.ColoanaSuplimentara2=A.ColoanaSuplimentara2
                    FROM Eval_ObiIndividualeTemp A
                    INNER JOIN Eval_ObiIndividualeTemp B ON A.IdUnic=B.IdUnic AND A.Pozitie<>B.Pozitie
                    WHERE A.IdQuiz = @1 AND A.F10003 = @2 AND A.Pozitie=@3;
                    
                    INSERT INTO ""Eval_ObiIndividualeTemp""(""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""Id"",  ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""IdUnic"", USER_NO, TIME)
                    SELECT ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", A.""IdQuiz"", A.F10003,  B.""Pozitie"", ""Id"",  ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""IdUnic"", A.USER_NO, A.TIME
                    FROM Eval_ObiIndividualeTemp A
                    INNER JOIN Eval_RaspunsIstoric B ON A.IdQuiz=B.IdQuiz AND A.F10003=B.F10003 AND B.Pozitie<>@3
                    WHERE A.IdQuiz = @1 AND A.F10003 = @2 AND A.Pozitie=@3 AND
                    (B.Pozitie * 100000000 + A.IdUnic) NOT IN (SELECT (Pozitie * 100000000 + IdUnic)
                    FROM Eval_ObiIndividualeTemp A
                    WHERE A.IdQuiz = @1 AND A.F10003 = @2 AND A.Pozitie<>@3);

                    DELETE
                    FROM Eval_ObiIndividualeTemp
                    WHERE IdQuiz = @1 AND F10003 = @2 AND Pozitie<>@3 AND
                    IdUnic NOT IN (SELECT IdUnic
                    FROM Eval_ObiIndividualeTemp
                    WHERE IdQuiz = @1 AND F10003 = @2 AND Pozitie=@3);

                    END;";

                General.ExecutaNonQuery(strSql, new object[] { idQuiz, f10003, pozitie });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //private void ActualizareComentarii(int idQuiz, int f10003, int poz)
        //{
        //    //string sql = "SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = " + idQuiz + " AND ""TipData"" = 23";
        //    DataTable dtObiective = General.IncarcaDT(@"SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""TipData"" = 23", new object[] { idQuiz });

        //    if (dtObiective != null && dtObiective.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < dtObiective.Rows.Count; i++)
        //        {
        //            if (poz == 1)
        //            { 
        //                //sql = "SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} ";
        //                //sql = string.Format(sql, dtObiective.Rows[i][0].ToString(), f10003, (poz).ToString(), idQuiz);
        //                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 ", new object[] { dtObiective.Rows[i][0].ToString(), f10003, (poz).ToString(), idQuiz });
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    for (int j = 0; j < dt.Rows.Count; j++)
        //                    {
        //                        //string strCnt = @"SELECT COUNT(*) FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {4} AND ""IdQuiz"" = {3} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND ""Obiectiv"" = '{7}' AND ""Activitate"" = '{8}' ";
        //                        //strCnt = string.Format(strCnt, dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz + 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString());
        //                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @5 AND ""IdQuiz"" = @4 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND ""Obiectiv"" = @8 AND ""Activitate"" = @9 ", new object[] { dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz + 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString() }),0));

        //                        if (cnt == 0)
        //                        {
        //                            //sql = "INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""Descriere"") "
        //                            //    + " SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"" "
        //                            //    + ", ""Descriere"" FROM ""Eval_ObiIndividualeTemp"" WHERE "
        //                            //    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND ""Obiectiv"" = '{7}' AND ""Activitate"" = '{8}'  ";
        //                            //sql = string.Format(sql, poz + 1, dtObiective.Rows[i][0].ToString(), f10003, poz, idQuiz, dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString());
        //                            General.IncarcaDT(
        //                                $@"INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""Descriere"")
        //                                SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {poz + 1}, ""IdLinieQuiz"",
        //                                ""Descriere"" FROM ""Eval_ObiIndividualeTemp"" WHERE
        //                                ""IdLinieQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" = @4 AND ""IdQuiz"" = @5 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND ""Obiectiv"" = @8 AND ""Activitate"" = @9  ", new object[] { poz + 1, dtObiective.Rows[i][0].ToString(), f10003, poz, idQuiz, dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString() });
        //                            Session["lstEval_ObiIndividualeTemp"] = null;
        //                        }
        //                        else
        //                        {
        //                            //sql = "UPDATE ""Eval_ObiIndividualeTemp"" SET ""Descriere"" = (SELECT ""Descriere"" FROM ""Eval_ObiIndividualeTemp""  WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND ""Obiectiv""='{7}' AND ""Activitate""='{8}')  WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {4} AND ""IdQuiz"" = {3} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND Obiectiv='{7}' AND Activitate='{8}'";
        //                            //sql = string.Format(sql, dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz + 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString());
        //                            General.IncarcaDT(@"UPDATE ""Eval_ObiIndividualeTemp"" SET ""Descriere"" = (SELECT ""Descriere"" FROM ""Eval_ObiIndividualeTemp""  WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND ""Obiectiv""=@8 AND ""Activitate""=@9)  WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @5 AND ""IdQuiz"" = @4 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND Obiectiv=@8 AND Activitate=@9", new object[] { dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz + 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString(), dt.Rows[j]["Obiectiv"].ToString(), dt.Rows[j]["Activitate"].ToString() });
        //                        }
        //                    }
        //                    Session["lstEval_ObiIndividualeTemp"] = null;
        //                }
        //                else
        //                {
        //                    int idAuto = DamiIdObi();
        //                    //sql = "INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""Descriere"") "
        //                    //    + " SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"" "
        //                    //    + ", ""Descriere"" FROM ""Eval_ObiIndividualeTemp"" WHERE "
        //                    //    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} ";
        //                    ////sql = "INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""IdAuto"", ""Descriere"") "
        //                    ////    + " SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {0}, ""IdLinieQuiz"", "
        //                    ////    + " {5} "
        //                    ////    + ", ""Descriere"" FROM ""Eval_ObiIndividualeTemp"" WHERE "
        //                    ////    + " ""IdLinieQuiz"" = {1} AND F10003 = {2} AND ""Pozitie"" = {3} AND ""IdQuiz"" = {4} ";
        //                    //sql = string.Format(sql, poz + 1, dtObiective.Rows[i][0].ToString(), f10003, poz, idQuiz, idAuto);
        //                    General.IncarcaDT(
        //                        $@"INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""Descriere"")
        //                        SELECT  ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {poz + 1}, ""IdLinieQuiz"",
        //                        ""Descriere"" FROM ""Eval_ObiIndividualeTemp"" WHERE
        //                        ""IdLinieQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" =@4 AND ""IdQuiz"" = @5 ", new object[] { poz + 1, dtObiective.Rows[i][0].ToString(), f10003, poz, idQuiz, idAuto });
        //                    Session["lstEval_ObiIndividualeTemp"] = null;
        //                }
        //            }

        //            if (poz == 2)
        //            {
        //                //sql = "SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} ";
        //                //sql = string.Format(sql, dtObiective.Rows[i][0].ToString(), f10003, (poz - 1).ToString(), idQuiz);
        //                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 ", new object[] { dtObiective.Rows[i][0].ToString(), f10003, (poz - 1).ToString(), idQuiz });
        //                if (dt != null && dt.Rows.Count > 0)
        //                {
        //                    for (int j = 0; j < dt.Rows.Count; j++)
        //                    {
        //                        //sql = "UPDATE ""Eval_ObiIndividualeTemp"" SET ""ExplicatiiCalificativ"" = (SELECT ""ExplicatiiCalificativ"" FROM ""Eval_ObiIndividualeTemp""  WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {2} AND ""IdQuiz"" = {3} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND ""Obiectiv""='{7}' AND ""Activitate""='{8}')  WHERE ""IdLinieQuiz"" = {0} AND F10003 = {1} AND ""Pozitie"" = {4} AND ""IdQuiz"" = {3} AND ""IdObiectiv"" = {5} AND ""IdActivitate"" = {6} AND ""Obiectiv""='{7}' AND ""Activitate""='{8}'";
        //                        //sql = string.Format(sql, dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz - 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString());
        //                        General.IncarcaDT(@"UPDATE ""Eval_ObiIndividualeTemp"" SET ""ExplicatiiCalificativ"" = (SELECT ""ExplicatiiCalificativ"" FROM ""Eval_ObiIndividualeTemp""  WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND ""Obiectiv""=@8 AND ""Activitate""=@9)  WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @5 AND ""IdQuiz"" = @4 AND ""IdObiectiv"" = @6 AND ""IdActivitate"" = @7 AND ""Obiectiv""=@8 AND ""Activitate""=@9", new object[] { dtObiective.Rows[i][0].ToString(), f10003, poz.ToString(), idQuiz, (poz - 1).ToString(), dt.Rows[j]["IdObiectiv"].ToString(), dt.Rows[j]["IdActivitate"].ToString() });
        //                    }
        //                    Session["lstEval_ObiIndividualeTemp"] = null;
        //                }
        //            }

        //        }
        //    }
        //}


        //private int DamiIdObi()
        //{
        //    int id = 1;

        //    try
        //    {
        //        int exista = 0;

        //        do
        //        {
        //            id = Convert.ToInt32(General.ExecutaScalar($@"SELECT NEXT VALUE FOR ObiIndividuale_SEQ", null));
        //            exista = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT COUNT(*) FROM Eval_ObiIndividualeTemp WHERE IdAuto=@1", new object[] { id }), 1));
        //        }
        //        while (exista == 1);
        //    }
        //    catch (Exception ex)
        //    {
        //        // srvGeneral.MemoreazaEroarea(ex.ToString(), "srvGeneral", new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return id;
        //}


    }
}
 