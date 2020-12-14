using DevExpress.PivotGrid.OLAP.Mdx;
using DevExpress.Web;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class EvalDetaliu : System.Web.UI.Page
    {

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
        HtmlTable grIntrebari = new HtmlTable();
        enum tipSectiune
        {
            SageataAlbaIntermediara = 1,
            SageataAlbastraUltima = 2,
            SageataAlbaPrima = 3
        }
        #endregion

        string idCateg = "0";
        int idPerioada = 0;

        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                //Florin 23.01.2020
                DataTable dtQ = General.IncarcaDT(@"SELECT ""CategorieQuiz"", ""Anul"" AS ""IdPerioada"" FROM ""Eval_Quiz"" WHERE ""Id""=@1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                if (dtQ != null && dtQ.Rows.Count > 0)
                {
                    idCateg = General.Nz(dtQ.Rows[0]["CategorieQuiz"], 0).ToString();
                    idPerioada = Convert.ToInt32(General.Nz(dtQ.Rows[0]["IdPerioada"], 0));
                }
                //idCateg = General.Nz(General.ExecutaScalar(@"SELECT ""CategorieQuiz"" FROM ""Eval_Quiz"" WHERE ""Id""=@1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) }), "0").ToString();

                lblEvaluat.InnerText = Dami.TraduCuvant("Evaluat") + ":" + General.Nz(Session["CompletareChestionar_Nume"],"");

                if (!IsPostBack)
                {
                    DataTable tableAAA = General.IncarcaDT(@"SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                    tableAAA.TableName = "Eval_Raspuns";

                    DataTable tableBBB = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                    tableBBB.TableName = "Eval_RaspunsLinii";

                    DataTable tableCCC = General.IncarcaDT(@"SELECT * FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                    tableCCC.TableName = "Eval_QuizIntrebari";

                    DataSet ds = new DataSet();
                    ds.Tables.Add(tableAAA);
                    tableBBB.PrimaryKey = new DataColumn[] { tableBBB.Columns["IdQuiz"], tableBBB.Columns["F10003"], tableBBB.Columns["Id"], tableBBB.Columns["Linia"] };
                    ds.Tables.Add(tableBBB);
                    tableCCC.PrimaryKey = new DataColumn[] { tableCCC.Columns["Id"] };
                    ds.Tables.Add(tableCCC);

                    Session["InformatiaCurentaCompletareChestionar"] = ds;

                    //Florin 2020.11.13
                    //Session["Eval_RaspunsLinii_Tabel"] = tableBBB;

                    DataTable table = new DataTable();
                    DataTable tableIntrebari = new DataTable();

                    table = ds.Tables[0];
                    lstEval_Raspuns = new List<Eval_Raspuns>();
                    foreach (DataRow rwEval_Raspuns in table.Rows)
                    {
                        Eval_Raspuns clsEval_Raspuns = new Eval_Raspuns(rwEval_Raspuns);
                        lstEval_Raspuns.Add(clsEval_Raspuns);
                    }
                    Session["lstEval_Raspuns"] = lstEval_Raspuns;


                    tableIntrebari = ds.Tables[1];
                    lstEval_RaspunsLinii = new List<Eval_RaspunsLinii>();
                    foreach (DataRow rwEval_RaspunsLinii in tableIntrebari.Rows)
                    {
                        Eval_RaspunsLinii clsEval_RaspunsLinii = new Eval_RaspunsLinii(rwEval_RaspunsLinii);
                        lstEval_RaspunsLinii.Add(clsEval_RaspunsLinii);
                    }
                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;


                    DataTable table2 = ds.Tables[2];
                    lstEval_QuizIntrebari = new List<Eval_QuizIntrebari>();
                    foreach (DataRow rwEval_QuizIntrebari in table2.Rows)
                    {
                        Eval_QuizIntrebari clsEval_QuizIntrebari = new Eval_QuizIntrebari(rwEval_QuizIntrebari);
                        lstEval_QuizIntrebari.Add(clsEval_QuizIntrebari);
                    }
                    Session["lstEval_QuizIntrebari"] = lstEval_QuizIntrebari;


                    DataTable table3 = General.IncarcaDT(@"select * from ""Eval_tblTipValoriLinii""", null);
                    lstEval_tblTipValoriLinii = new List<Eval_tblTipValoriLinii>();
                    foreach (DataRow dr3 in table3.Rows)
                    {
                        Eval_tblTipValoriLinii clsNew = new Eval_tblTipValoriLinii(dr3);
                        lstEval_tblTipValoriLinii.Add(clsNew);
                    }
                    Session["lstEval_tblTipValoriLinii"] = lstEval_tblTipValoriLinii;

                    Session["Aprobat360"] = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COALESCE(""Aprobat"",0) FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""IdUser""=@3", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Session["UserId"] }), 1));

                    CreazaMeniu();

                    Session["indexSec"] = indexSec;
                    Session["totalSec"] = totalSec;
                    txtNrSectiune.Text = (indexSec + 1).ToString();
                    txtNrTotalSectiune.Text = totalSec.ToString();


                    if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -98)) && Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_TrebuieSaIaLaCunostinta"], 1)) == 1 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_ALuatLaCunostinta"], 1)) == 0) btnLuatCunostinta.Visible = true;


                    #region CreeazaTab

                    int consRU = Convert.ToInt32(Session["consRU"] ?? 0);

                    List<metaEvalDenumireSuper> lstEvalDenumireSuper = Evaluare.EvalDenumireSuper(Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["Eval_PozitieUserLogat"], 1)), consRU);

                    Session["lstEvalDenumireSuper"] = lstEvalDenumireSuper;
                    CreazaTab(lstEvalDenumireSuper);

                    Session["Eval_ActiveTab"] = "1";
                    if (tabSuper != null && tabSuper.ActiveTab != null)
                        Session["Eval_ActiveTab"] = tabSuper.ActiveTab.Name.Replace("tab", "");
                    else
                    {//Radu 20.02.2019
                        Session["Eval_ActiveTab"] = Convert.ToInt32(General.Nz(Session["Eval_PozitieUserLogat"], 1));
                    }

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

                    if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) == Convert.ToInt32(General.Nz(Session["User_Marca"], -98)) && Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_TrebuieSaIaLaCunostinta"], 1)) == 1 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_ALuatLaCunostinta"], 1)) == 0) btnLuatCunostinta.Visible = true;

                    CreazaMeniu();
                    //CreazaTab(lstEvalDenumireSuper);
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());
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
                //Florin 2019.12.10 Begin - sa poata sa modifice chestionarul simultan, oricare actor de pe circuit

                ////Florin 2019.02.27
                //if ((Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))) || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1)) == 0)
                //{
                //    //MessageBox.Show("Nu aveti drepturi pentru aceasta operatie!", MessageBox.icoSuccess);
                //    pnlSectiune.JSProperties["cpAlertMessage"] = "Nu aveti drepturi pentru aceasta operatie!";
                //    return;
                //}

                int respectaOrdinea = 0;
                DataTable entCir = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                if (entCir != null && entCir.Rows.Count > 0) respectaOrdinea = Convert.ToInt32(entCir.Rows[0]["RespectaOrdinea"] != DBNull.Value ? entCir.Rows[0]["RespectaOrdinea"].ToString() : "0");

                if (respectaOrdinea == 1)
                {
                    if ((Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))) || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1)) == 0)
                    {
                        pnlSectiune.JSProperties["cpAlertMessage"] = "Nu aveti drepturi pentru aceasta operatie!";
                        return;
                    }
                }
                else
                {
                    //NOP
                }

                //Florin 2019.12.10 End


                //Florin 2019.06.27
                SalveazaGridurile();

                string sqlCommandDelete = string.Empty;
                string sqlCommandDeleteTemp = string.Empty;
                string sqlCommandInsert = string.Empty;
                string sqlCommandInsertTemp = string.Empty;

                #region Set Scripts Upload DB
                //Florin 2020.12.04 - am adaugat filtrul cu linia
                sqlCommandDelete = @"delete from ""Eval_RaspunsLinii"" where ""IdQuiz"" = @1 and ""F10003"" = @2 and ""Id"" = @3 AND ""Linia"" = @4";
                //sqlCommandInsert = $@"insert into ""Eval_RaspunsLinii""(""IdQuiz"", ""F10003"", ""Id"", ""Linia"", 
                //                                ""Super1"",""Super2"",""Super3"",""Super4"",""Super5"",
                //                                ""Super6"",""Super7"",""Super8"",""Super9"",""Super10"",
                //                                ""Super11"",""Super12"",""Super13"",""Super14"",""Super15"",
                //                                ""Super16"",""Super17"",""Super18"",""Super19"",""Super20"",
                //                                ""USER_NO"",""TIME"",""Descriere"",""TipData"",""TipValoare"",
                //                                ""Sublinia"",""Tinta"",""Super1_1"",""Super1_2"",""Super1_3"",
                //                                ""Super2_1"",""Super2_2"",""Super2_3"",""Super3_1"",""Super3_2"",
                //                                ""Super3_3"",""Super4_1"",""Super4_2"",""Super4_3"",""Super5_1"",
                //                                ""Super5_2"",""Super5_3"",""Super6_1"",""Super6_2"",""Super6_3"",
                //                                ""Super7_1"",""Super7_2"",""Super7_3"",""Super8_1"",""Super8_2"",
                //                                ""Super8_3"",""Super9_1"",""Super9_2"",""Super9_3"",""Super10_1"",
                //                                ""Super10_2"",""Super10_3"",""IdGrup"",""PondereRatingGlobal"",""NumeGrup"",
                //                                ""Super11_1"",""Super11_2"",""Super11_3"",""Super12_1"",""Super12_2"",
                //                                ""Super12_3"",""Super13_1"",""Super13_2"",""Super13_3"",""Super14_1"",
                //                                ""Super14_2"",""Super14_3"",""Super15_1"",""Super15_2"",""Super15_3"",
                //                                ""Super16_1"",""Super16_2"",""Super16_3"",""Super17_1"",""Super17_2"",
                //                                ""Super17_3"",""Super18_1"",""Super18_2"",""Super18_3"",""Super19_1"",
                //                                ""Super19_2"",""Super19_3"",""Super20_1"",""Super20_2"",""Super20_3"",
                //                                ""Super1_4"",""Super1_5"",""Super1_6"",""Super2_4"",""Super2_5"",
                //                                ""Super2_6"",""Super3_4"",""Super3_5"",""Super3_6"",""Super4_4"",
                //                                ""Super4_5"",""Super4_6"",""Super5_4"",""Super5_5"",""Super5_6"",
                //                                ""Super6_4"",""Super6_5"",""Super6_6"",""Super7_4"",""Super7_5"",
                //                                ""Super7_6"",""Super8_4"",""Super8_5"",""Super8_6"",""Super9_4"",
                //                                ""Super9_5"",""Super9_6"",""Super10_4"",""Super10_5"",""Super10_6"",
                //                                ""Super11_4"",""Super11_5"",""Super11_6"",""Super12_4"",""Super12_5"",
                //                                ""Super12_6"",""Super13_4"",""Super13_5"",""Super13_6"",""Super14_4"",
                //                                ""Super14_5"",""Super14_6"",""Super15_4"",""Super15_5"",""Super15_6"",
                //                                ""Super16_4"",""Super16_5"",""Super16_6"",""Super17_4"",""Super17_5"",
                //                                ""Super17_6"",""Super18_4"",""Super18_5"",""Super18_6"",""Super19_4"",
                //                                ""Super19_5"",""Super19_6"",""Super20_4"",""Super20_5"",""Super20_6"",
                //                                ""DescriereInRatingGlobal"")
                //                              values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,
                //                                @20,@21,@22,@23,@24,{General.CurrentDate()},@26,@27,@28,@29,@30,@31,@32,@33,@34,@35,@36,@37,@38,@39,
                //                                @40,@41,@42,@43,@44,@45,@46,@47,@48,@49,@50,@51,@52,@53,@54,@55,@56,@57,@58,@59,
                //                                @60,@61,@62,@63,@64,@65,@66,@67,@68,@69,@70,@71,@72,@73,@74,@75,@76,@77,@78,@79,
                //                                @80,@81,@82,@83,@84,@85,@86,@87,@88,@89,@90,@91,@92,@93,@94,@95,@96,@97,@98,@99,
                //                                @100,@101,@102,@103,@104,@105,@106,@107,@108,@109,@110,@111,@112,@113,@114,@115,@116,@117,@118,@119,
                //                                @120,@121,@122,@123,@124,@125,@126,@127,@128,@129,@130,@131,@132,@133,@134,@135,@136,@137,@138,@139,
                //                                @140,@141,@142,@143,@144,@145,@146,@147,@148,@149,@150,@151,@152,@153,@154,@155);";
                sqlCommandInsert = $@"insert into ""Eval_RaspunsLinii""(""IdQuiz"", ""F10003"", ""Id"", ""Linia"", 
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
                                                @20,@21,@22,@23,@24,@25,{General.CurrentDate()},@26,@27,@28,@29,@30,@31,@32,@33,@34,@35,@36,@37,@38,@39,
                                                @40,@41,@42,@43,@44,@45,@46,@47,@48,@49,@50,@51,@52,@53,@54,@55,@56,@57,@58,@59,
                                                @60,@61,@62,@63,@64,@65,@66,@67,@68,@69,@70,@71,@72,@73,@74,@75,@76,@77,@78,@79,
                                                @80,@81,@82,@83,@84,@85,@86,@87,@88,@89,@90,@91,@92,@93,@94,@95,@96,@97,@98,@99,
                                                @100,@101,@102,@103,@104,@105,@106,@107,@108,@109,@110,@111,@112,@113,@114,@115,@116,@117,@118,@119,
                                                @120,@121,@122,@123,@124,@125,@126,@127,@128,@129,@130,@131,@132,@133,@134,@135,@136,@137,@138,@139,
                                                @140,@141,@142,@143,@144,@145,@146,@147,@148,@149,@150,@151,@152,@153,@154)";

                #endregion

                lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;
                foreach (Eval_RaspunsLinii entRaspLinie in lstEval_RaspunsLinii)
                {
                    try
                    {
                        sqlCommandDeleteTemp = sqlCommandDelete;
                        //Florin 2020.12.04 - am adaugat filtrul cu linia
                        General.ExecutaNonQuery(sqlCommandDeleteTemp, new object[] { entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id, entRaspLinie.Linia });

                        sqlCommandInsertTemp = sqlCommandInsert;
                        General.ExecutaNonQuery(sqlCommandInsertTemp, new object[] {  entRaspLinie.IdQuiz, entRaspLinie.F10003, entRaspLinie.Id,
                                            entRaspLinie.Linia, entRaspLinie.Super1, entRaspLinie.Super2, entRaspLinie.Super3, entRaspLinie.Super4,
                                            entRaspLinie.Super5, entRaspLinie.Super6, entRaspLinie.Super7, entRaspLinie.Super8, entRaspLinie.Super9,
                                            entRaspLinie.Super10, entRaspLinie.Super11, entRaspLinie.Super12, entRaspLinie.Super13, entRaspLinie.Super14,
                                            entRaspLinie.Super15, entRaspLinie.Super16, entRaspLinie.Super17, entRaspLinie.Super18, entRaspLinie.Super19,
                                            entRaspLinie.Super20, entRaspLinie.USER_NO, entRaspLinie.Descriere, entRaspLinie.TipData,
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
                
                #region set scripts Obiective Individuale

                List<Eval_ObiIndividualeTemp> arrObi = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                if (arrObi != null && arrObi.Count != 0)
                {
                    List<Eval_ObiIndividualeTemp> lstObiIndividuale = arrObi.Where(p => p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == pozitie).ToList();

                    if (lstObiIndividuale != null && lstObiIndividuale.Count != 0)
                    {
                        string sqlDeleteObiIndividuale = @"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdAuto"" = @1;";
                        string sqlInsertObiIndividuale = 
                            $@"INSERT INTO ""Eval_ObiIndividualeTemp""
                            (""IdUnic"", ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", 
                            ""Pondere"", ""Descriere"", ""Target"", ""Realizat"", ""IdCalificativ"",
                            ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", ""F10003"", ""Pozitie"",
                            ""Id"", ""IdLinieQuiz"", 
                            ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"", 
                            USER_NO, TIME, ""IdPeriod"", ""IdCategObiective"", ""Total1"", ""Total2"", ""Termen"")
                            VALUES(@idUnic,@2,@3,@4,@5,
                            @6,@7,@8,@9,@10,
                            @11,@12,@13,@14,@15,
                            @16,@17,
                            @18,@19,@20,@21,
                            @22,@23,@24,@25,formulaSql1,formulaSql2,@31);";

                        string tgv = "";

                        foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
                        {
                            try
                            {
                                string sqlDel = sqlDeleteObiIndividuale;
                                string sqlIns = sqlInsertObiIndividuale;

                                string formulaSql1 = "@26";
                                string formulaSql2 = "@27";
                                if (General.Nz(clsObiIndividuale.FormulaSql1, "").ToString().Trim() != "") formulaSql1 = General.Nz(clsObiIndividuale.FormulaSql1, "").ToString().Trim();
                                if (General.Nz(clsObiIndividuale.FormulaSql2, "").ToString().Trim() != "") formulaSql2 = General.Nz(clsObiIndividuale.FormulaSql2, "").ToString().Trim();
                                formulaSql1 = formulaSql1.Replace("Obiectiv", "@3").Replace("Activitate", "@5").Replace("Pondere", "@28").Replace("Calificativ", "@11").Replace("Descriere", "@7").Replace("ExplicatiiCalificativ", "@12").Replace("Target", "@29").Replace("Realizat", "@30").Replace("ColoanaSuplimentara1", "@18").Replace("ColoanaSuplimentara2", "@19").Replace("ColoanaSuplimentara3", "@20").Replace("ColoanaSuplimentara4", "@21");
                                formulaSql1 = formulaSql1.Replace("Obiectiv", "@3").Replace("Activitate", "@5").Replace("Pondere", "@28").Replace("Calificativ", "@11").Replace("Descriere", "@7").Replace("ExplicatiiCalificativ", "@12").Replace("Target", "@29").Replace("Realizat", "@30").Replace("ColoanaSuplimentara1", "@18").Replace("ColoanaSuplimentara2", "@19").Replace("ColoanaSuplimentara3", "@20").Replace("ColoanaSuplimentara4", "@21");
                                sqlIns = sqlIns.Replace("formulaSql1", formulaSql1).Replace("formulaSql2", formulaSql2);

                                string sqlObi =
                                    "BEGIN " + Environment.NewLine +
                                        sqlDel + Environment.NewLine +
                                        sqlIns + Environment.NewLine +
                                    "END;";

                                //Florin 2020.02.03 - am scos .Replace(",", ".") de la activitate
                                string seq = "NEXT VALUE FOR ObiIndividuale_SEQ";
                                if (Constante.tipBD == 2) seq = @" ""ObiIndividuale_SEQ"".NEXTVAL";
                                sqlObi = sqlObi.Replace("@idUnic", clsObiIndividuale.IdUnic <= 0 ? seq : clsObiIndividuale.IdUnic.ToString());
                                tgv += sqlObi + Environment.NewLine;
                                General.ExecutaNonQuery(sqlObi, new object[] {
                                    clsObiIndividuale.IdAuto, clsObiIndividuale.IdObiectiv, clsObiIndividuale.Obiectiv, clsObiIndividuale.IdActivitate, General.Nz(clsObiIndividuale.Activitate, "").ToString(),
                                    General.Nz(clsObiIndividuale.Pondere, "0").ToString().Replace(",", "."), clsObiIndividuale.Descriere, General.Nz(clsObiIndividuale.Target, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.Realizat, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.IdCalificativ, "0").ToString().Replace(",", "."),
                                    clsObiIndividuale.Calificativ, clsObiIndividuale.ExplicatiiCalificativ, clsObiIndividuale.IdQuiz, clsObiIndividuale.F10003, clsObiIndividuale.Pozitie,
                                    clsObiIndividuale.Id, clsObiIndividuale.IdLinieQuiz,
                                    clsObiIndividuale.ColoanaSuplimentara1, clsObiIndividuale.ColoanaSuplimentara2, clsObiIndividuale.ColoanaSuplimentara3, clsObiIndividuale.ColoanaSuplimentara4,
                                    General.Nz(clsObiIndividuale.USER_NO, Session["UserId"]), General.Nz(clsObiIndividuale.TIME, DateTime.Now), clsObiIndividuale.IdPeriod, clsObiIndividuale.IdCategObiective,
                                    General.Nz(clsObiIndividuale.Total1, "0").ToString().Replace(",", "."), General.Nz(clsObiIndividuale.Total2, "0").ToString().Replace(",", "."), clsObiIndividuale.Pondere, clsObiIndividuale.Target, clsObiIndividuale.Realizat, clsObiIndividuale.Termen });
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea("3 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                        }

                        //Radu 07.02.2019 - deoarece liniile vechi au fost sterse, trebuie reinitializata lista obiectivelor, deoarece liniile au alt IdAuto in tabela
                        lstEval_ObiIndividualeTemp = new List<Eval_ObiIndividualeTemp>();
                        DataTable dtObiIndividuale = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE F10003=@1 ORDER BY CAST(""Obiectiv"" AS varchar(4000)), CAST(""Activitate"" AS varchar(4000)) ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                        foreach (DataRow rwObiIndividuale in dtObiIndividuale.Rows)
                        {
                            Eval_ObiIndividualeTemp clsObiIndividuale = new Eval_ObiIndividualeTemp(rwObiIndividuale);
                            lstEval_ObiIndividualeTemp.Add(clsObiIndividuale);
                        }
                        Session["lstEval_ObiIndividualeTemp"] = lstEval_ObiIndividualeTemp;
                    }
                }



                //Florin 2019.06.27
                //stergem liniile din baza
                {
                    List<Eval_ObiIndividualeTemp> lstSterse = Session["lstEval_ObiIndividualeTemp_Sterse"] as List<Eval_ObiIndividualeTemp>;
                    if (lstSterse != null && lstSterse.Count != 0)
                    {
                        string idauto = "";

                        foreach (Eval_ObiIndividualeTemp l in lstSterse)
                        {
                            idauto += "," + l.IdAuto;
                        }

                        if (idauto != "")
                            General.ExecutaNonQuery($@"DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdAuto"" IN ({idauto.Substring(1)})", null);

                        Session["lstEval_ObiIndividualeTemp_Sterse"] = null;
                    }
                }


                #endregion


                #region set scripts Competente Angajat

                List<Eval_CompetenteAngajatTemp> arrComp = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                if (arrComp != null && arrComp.Count != 0)
                {
                    List<Eval_CompetenteAngajatTemp> lstCompAngajat = arrComp.Where(p => p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == pozitie).ToList();
                    if (lstCompAngajat != null && lstCompAngajat.Count != 0)
                    {
                        string sqlDeleteCompAngajat = @"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdAuto""=@15;";
                        string sqlInsertCompAngajat = @"insert into ""Eval_CompetenteAngajatTemp""(""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"",
                                                                                      ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", 
                                                                                      ""Explicatii"", ""IdQuiz"", ""F10003"", ""Pozitie"", ""Id"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME, ""IdPeriod"", ""Total1"", ""Total2"")
                                                                                        values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@idUnic,@16,@17,@18,formulaSql1,formulaSql2)";

                        string tgv = "";

                        foreach (Eval_CompetenteAngajatTemp clsCompetenta in lstCompAngajat)
                        {
                            try
                            {
                                string sqlDel = sqlDeleteCompAngajat;
                                string sqlIns = sqlInsertCompAngajat;

                                string formulaSql1 = "@20";
                                string formulaSql2 = "@21";
                                if (General.Nz(clsCompetenta.FormulaSql1, "").ToString().Trim() != "") formulaSql1 = General.Nz(clsCompetenta.FormulaSql1, "").ToString().Trim();
                                if (General.Nz(clsCompetenta.FormulaSql2, "").ToString().Trim() != "") formulaSql2 = General.Nz(clsCompetenta.FormulaSql2, "").ToString().Trim();
                                formulaSql1 = formulaSql1.Replace("Competenta", "@3").Replace("Pondere", "@19").Replace("Calificativ", "@7").Replace("Explicatii", "@9").Replace("ExplicatiiCalificativ", "@8");
                                formulaSql1 = formulaSql1.Replace("Competenta", "@3").Replace("Pondere", "@19").Replace("Calificativ", "@7").Replace("Explicatii", "@9").Replace("ExplicatiiCalificativ", "@8");
                                sqlIns = sqlIns.Replace("formulaSql1", formulaSql1).Replace("formulaSql2", formulaSql2);

                                string sqlCmp =
                                    "BEGIN " + Environment.NewLine +
                                        sqlDel + Environment.NewLine +
                                        sqlIns + Environment.NewLine +
                                    "END;";

                                sqlCmp = sqlCmp.Replace("@idUnic", clsCompetenta.IdUnic <= 0 ? "NEXT VALUE FOR CompetenteAng_SEQ" : clsCompetenta.IdUnic.ToString());
                                tgv += sqlCmp + Environment.NewLine;
                                General.ExecutaNonQuery(sqlCmp, new object[] { clsCompetenta.IdCategCompetenta, clsCompetenta.CategCompetenta, clsCompetenta.IdCompetenta, clsCompetenta.Competenta,
                                                                        General.Nz(clsCompetenta.Pondere, "0").ToString().Replace(",", "."), clsCompetenta.IdCalificativ, clsCompetenta.Calificativ, clsCompetenta.ExplicatiiCalificativ,
                                                                        clsCompetenta.Explicatii, clsCompetenta.IdQuiz, clsCompetenta.F10003, clsCompetenta.Pozitie, clsCompetenta.Id, clsCompetenta.IdLinieQuiz, clsCompetenta.IdAuto, General.Nz(clsCompetenta.USER_NO, Session["UserId"]), General.Nz(clsCompetenta.TIME, DateTime.Now), 
                                                                        clsCompetenta.IdPeriod, clsCompetenta.Pondere, General.Nz(clsCompetenta.Total1, "0").ToString().Replace(",", "."), General.Nz(clsCompetenta.Total2, "0").ToString().Replace(",", ".")});
                            }
                            catch (Exception ex)
                            {
                                General.MemoreazaEroarea("4 - " + ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                            }
                        }

                        //Radu 07.02.2019 - deoarece liniile vechi au fost sterse, trebuie reinitializata lista competentelor, deoarece liniile au alt IdAuto in tabela
                        lstEval_CompetenteAngajatTemp = new List<Eval_CompetenteAngajatTemp>();
                        DataTable dtCompetenteAngajat = General.IncarcaDT(@"select * from ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 ORDER BY ""Competenta"" ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                        foreach (DataRow rwCompetenteAngajat in dtCompetenteAngajat.Rows)
                        {
                            Eval_CompetenteAngajatTemp clsCompetenteAngajat = new Eval_CompetenteAngajatTemp(rwCompetenteAngajat);
                            lstEval_CompetenteAngajatTemp.Add(clsCompetenteAngajat);
                        }
                        Session["lstEval_CompetenteAngajatTemp"] = lstEval_CompetenteAngajatTemp;
                    }
                }


                //Florin 2019.06.27
                //stergem liniile din baza
                {
                    List<Eval_CompetenteAngajatTemp> lstSterse = Session["lstEval_CompetenteAngajatTemp_Sterse"] as List<Eval_CompetenteAngajatTemp>;
                    if (lstSterse != null && lstSterse.Count != 0)
                    {
                        string idauto = "";

                        foreach (Eval_CompetenteAngajatTemp l in lstSterse)
                        {
                            idauto += "," + l.IdAuto;
                        }

                        if (idauto != "")
                            General.ExecutaNonQuery($@"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdAuto"" IN ({idauto.Substring(1)})", null);

                        Session["lstEval_CompetenteAngajatTemp_Sterse"] = null;
                    }
                }


                #endregion


                //Florin 2020.11.13
                DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                if (dtTbl != null)
                    General.SalveazaDate(dtTbl, "Eval_RaspunsLinii");


                //Florin 2020.11.06
                if (General.Nz(Session["CompletareChestionar_Sincronizare"],0).ToString() == "1" && idCateg == "0")
                    PreluareDateAutomat(pozitie);

                //Florin 2019.10.16
                if (General.Nz(Session["NumeGriduri"], "").ToString() != "")
                {
                    string[] arr = Session["NumeGriduri"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ASPxGridView grDate = grIntrebari.FindControl(arr[i]) as ASPxGridView;
                        if (grDate != null)
                        {
                            int idLinieQuiz = Convert.ToInt32(grDate.ID.Split('_')[grDate.ID.Split('_').Count() - 1]);

                            if (grDate.ID.IndexOf("grDateObiective") >= 0)
                            {
                                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                                grDate.DataSource = lst.Where(p => p.IdLinieQuiz == idLinieQuiz && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"]) && p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1))).ToList();
                            }

                            if (grDate.ID.IndexOf("grDateCompetente") >= 0)
                            {
                                List<Eval_CompetenteAngajatTemp> lst = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;
                                grDate.DataSource = lst.Where(p => p.IdLinieQuiz == idLinieQuiz && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"])).ToList();
                            }

                            //Florin 2020.11.19
                            if (grDate.ID.IndexOf("grTabela") >= 0)
                            {
                                //DataTable dt = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                                //DataRow[] arrDr = dtTbl.Select("Id=" + idLinieQuiz);
                                //if (arrDr.Count() > 0)
                                //    grDate.DataSource = arrDr.CopyToDataTable();
                                //else
                                //    grDate.DataSource = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE 1=2");

                                //grDate.DataSource = Session["Eval_RaspunsLinii_Tabel"];
                                grDate.DataSource = dtTbl;
                            }

                            grDate.KeyFieldName = "IdAuto";
                            grDate.DataBind();
                        }
                    }
                }

                //MessageBox.Show("Proces realizat cu succes!", MessageBox.icoSuccess);
                pnlSectiune.JSProperties["cpAlertMessage"] = "Proces realizat cu succes!";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        #region Sectiuni Creare + Focus

        private void CreazaMeniu()
        {
            try
            {
                int i = 0;
                int idRoot = -99;
                var entRoot = lstEval_QuizIntrebari.Where(p => p.Parinte == 0 && p.Descriere == "Root").FirstOrDefault();
                if (entRoot != null) idRoot = entRoot.Id;

                var entSec = lstEval_QuizIntrebari.Where(p => p.Parinte == idRoot && p.EsteSectiune == 1).OrderBy(p => p.OrdineAfisare);

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
                bool creat = false;

                if (e.Parameter == "btnSave")
                {
                    btnSave_Click(null, null);
                    return;
                }

                //Radu 19.04.2019
                if (e.Parameter == "CreeazaSectiune")
                {
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());
                    return;
                }

                if (e.Parameter.Contains(";"))
                {
                    string[] param = e.Parameter.Split(';');
                    string nameControl = param[0];
                    string valueControl = param[1].Replace("'"," ").Replace("\""," ").Replace("\\"," ").Replace("/"," ");

                    if (nameControl == "btnLuatCunostinta")
                    {
                        if (valueControl != "1") valueControl = "2";
                        //lstEval_Raspuns = Session["lstEval_Raspuns"] as List<Eval_Raspuns>;
                        //if (valueControl == "1")
                        //    lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta = 1;
                        //else
                        //    lstEval_Raspuns.FirstOrDefault().LuatLaCunostinta = 2;
                        //Session["lstEval_Raspuns"] = lstEval_Raspuns;

                        General.ExecutaNonQuery($@"UPDATE ""Eval_Raspuns"" SET ""LuatLaCunostinta"" = @1, ""LuatData""={General.CurrentDate()}, ""LuatUser""={Session["UserId"]}, USER_NO={Session["UserId"]}, TIME={General.CurrentDate()} WHERE ""IdQuiz""=@2 AND F10003 = @3", new object[] { valueControl, Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"],1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                        pnlSectiune.JSProperties["cpAlertMessage"] = "Proces realizat cu succes!";
                        return;
                    }

                    nameControl = nameControl.Substring(nameControl.IndexOf("_WXY_"));
                    nameControl = nameControl.Replace("_WXY_", "");

                    //if (lstEval_RaspunsLinii.Count == 0)
                        lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;

                    string super = "Super" + Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1));
                    

                    Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == Convert.ToInt32(nameControl)).FirstOrDefault();
                    if (raspLinie != null)
                    {
                        if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20)
                        {
                            if (raspLinie.TipData == 3 && raspLinie.TipValoare == 8)
                            {
                                string mesaj = "";


                                //Florin 2019.12.10 - Begin - in DataTable-ul de mia jos am inlocuit Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) cu pozComp
                                int pozComp = Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1));
                                int respectaOrdinea = 0;
                                DataTable entCir = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                                if (entCir != null && entCir.Rows.Count > 0) respectaOrdinea = Convert.ToInt32(entCir.Rows[0]["RespectaOrdinea"] != DBNull.Value ? entCir.Rows[0]["RespectaOrdinea"].ToString() : "0");

                                if (respectaOrdinea == 0)
                                    pozComp = Convert.ToInt32(General.Nz(Session["Eval_PozitieUserLogat"], 1));
                                //Florin 2019.12.10 - End

                                DataTable dt = General.IncarcaDT(
                                    @"select convert(INT,round(CONVERT(DECIMAL(18,2),sum(coalesce(""IdCalificativ"", 0))) / count(*),0)) 
                                    from ""Eval_ObiIndividualeTemp"" 
                                    where ""IdQuiz"" = @1 and F10003 = @2 AND ""Pozitie""=@3 
                                    and ""IdLinieQuiz"" in (select ""Id"" from ""Eval_QuizIntrebari"" where ""IdQuiz"" = @1 and ""TipData"" = 23)", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), pozComp });

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
                                        mesaj += "Calificativul ales este mai mare decat media calificativelor Obiectivelor Operationale. \n Va rugam sa alegeti alta optiune.";

                                        //Florin 2019.02.20
                                        //pnlSectiune.JSProperties["cpAlertMessage"] = "Calificativul ales este mai mare decat media calificativelor Obiectivelor Operationale. \n Va rugam sa alegeti alta optiune.";
                                        //return;
                                    }
                                }


                                //pt clientul Pelifilip - daca este calificativ final diferenta intre nota fiecarui obiectiv si calificativ final sa fie mai mica de 2 puncte, in caz contrar apare mesaj de atentionare, neblocant
                                //Sub asteptari
                                //In linie cu asteptarile
                                //Peste asteptari
                                if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) == 2)
                                {
                                    int valCtl = 1;
                                    if (valueControl.Trim().ToLower() == "sub asteptari") valCtl = 1;
                                    if (valueControl.Trim().ToLower() == "in linie cu asteptarile") valCtl = 2;
                                    if (valueControl.Trim().ToLower() == "peste asteptari") valCtl = 3;


                                    List<Eval_ObiIndividualeTemp> arrObi = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                                    List<Eval_ObiIndividualeTemp> lstObiIndividuale = arrObi.Where(p => p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))).ToList();
                                    if (lstObiIndividuale != null && lstObiIndividuale.Count != 0)
                                    {
                                        foreach (Eval_ObiIndividualeTemp clsObiIndividuale in lstObiIndividuale)
                                        {
                                            List<Eval_SetCalificativDet> lstEval_SetCalificativDet = Session["feedEval_Calificativ"] as List<Eval_SetCalificativDet>;
                                            int nota = lstEval_SetCalificativDet.Where(p => p.IdCalificativ == clsObiIndividuale.IdCalificativ).FirstOrDefault().Nota;
                                            //int dif = Convert.ToInt32(General.Nz(clsObiIndividuale.IdCalificativ, 1)) - valCtl;
                                            int dif = nota - valCtl;
                                            if (Math.Abs(dif) >= 2)
                                            {
                                                //Florin 2019.02.20
                                                //pnlSectiune.JSProperties["cpAlertMessage"] = "Exista o diferenta fata de calificativele acordate per obiectiv !";
                                                mesaj += "Exista o diferenta fata de calificativele acordate per obiectiv !";
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (mesaj != "")
                                    pnlSectiune.JSProperties["cpAlertMessage"] = mesaj;
                            }
                        }

                        if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 24)
                        {//Cristim
                            if (raspLinie.TipData == 3 && raspLinie.TipValoare == 2)
                            {//butoane radio
                                Eval_QuizIntrebari elem = lstEval_QuizIntrebari.Where(p => p.Id == raspLinie.Id).FirstOrDefault();
                                Eval_QuizIntrebari parinte = lstEval_QuizIntrebari.Where(p => p.Id == elem.Parinte).FirstOrDefault();

                                if (parinte != null && parinte.Descriere.ToUpper().Contains("EVALUARE CALITATIVA"))
                                {
                                    List<Eval_QuizIntrebari> lstIntrebari = lstEval_QuizIntrebari.Where(p => p.Parinte == parinte.Id && p.TipData == 3 && p.TipValoare == 2).OrderBy(p => p.Id).ToList();
                                    List<int> lst = new List<int>();
                                    for (int i = 0; i < lstIntrebari.Count; i++)
                                        lst.Add(lstIntrebari[i].Id);
                                    List<Eval_RaspunsLinii> lstRasp = lstEval_RaspunsLinii.Where(p => p.F10003 == raspLinie.F10003 && lst.Contains(p.Id)).ToList();
                                    int suma = 0, nr = 0;
                                    foreach (Eval_RaspunsLinii linie in lstRasp)
                                    {
                                        nr++;
                                        if (linie.Id == raspLinie.Id)
                                            suma += Convert.ToInt32(param[1]);
                                        else
                                        {
                                            PropertyInfo val = linie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                            if (val != null)
                                            {
                                                string s = val.GetValue(linie, null).ToString();
                                                if (s.Length > 0)
                                                {
                                                    int rez = 0;
                                                    int.TryParse(s, out rez);
                                                    suma += rez;
                                                }
                                            }
                                        }
                                    }
                                    double calif = 100 + 100 * ((Convert.ToDouble(suma) / Convert.ToDouble(nr) - 3.5) * 0.1) / 0.5;
                                    calif *= 0.3;
                                    Eval_QuizIntrebari txtCalif = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CALITATIVA") && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                    if (txtCalif != null)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalif.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            val.SetValue(linieCalif, calif.ToString(), null);
                                            Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;
                                        }
                                    }


                                    Eval_QuizIntrebari txtCalifCalit = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CANTITATIVA") && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                    if (txtCalifCalit != null)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifCalit.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            string s = val.GetValue(linieCalif, null).ToString();
                                            if (s.Length > 0)
                                            {
                                                double rez = 0;
                                                double.TryParse(s, out rez);
                                                calif += rez;
                                            }
                                        }
                                    }

                                    Eval_QuizIntrebari txtCalifFinal = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV FINAL EVALUARE PERFORMANTA") && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                    if (txtCalifFinal != null)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifFinal.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                        if (val != null)
                                            val.SetValue(linieCalif, calif.ToString(), null);
                                    }
                                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                                }
                            }
                        }

                        
                        PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (piValue != null)
                        {
                            piValue.SetValue(raspLinie, valueControl, null);
                            Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                            if (General.Nz(Session["IdClient"], 1).ToString() == "27")
                            {//Euroins
                                int nrSec = Convert.ToInt32(Session["indexSec"].ToString());
                                if (nrSec == 3 && raspLinie.TipData == 3)
                                {
                                    double nota = 0;
                                    int cnt = 0;
                                    List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.TipData == 3 && p.IdQuiz == raspLinie.IdQuiz).ToList();
                                    if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                    {
                                        foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                        {
                                            Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                            PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                            if (val != null)
                                            {
                                                string s = val.GetValue(linieCalif, null).ToString();
                                                if (s.Length > 0)
                                                {
                                                    double rez = 0;
                                                    double.TryParse(s, out rez);
                                                    nota += rez;
                                                    cnt++;
                                                }
                                            }
                                        }
                                    }

                                    nota = nota / cnt;
                                    double calif = nota;

                                    Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("TOTAL INTERMEDIAR 3A") && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                    if (notaFinala != null)
                                    {
                                        Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                        if (val != null)
                                            val.SetValue(linieNota, calif.ToString("0.##"), null);
                                    }

                                    double notaF = 0;
                                    int nr = 5;
                                    lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("TOTAL INTERMEDIAR") && p.IdQuiz == raspLinie.IdQuiz).ToList();
                                    if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                    {
                                        foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                        {
                                            Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                            PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                            if (val != null)
                                            {
                                                string s = val.GetValue(linieCalif, null).ToString();
                                                if (s.Length > 0)
                                                {
                                                    double rez = 0;
                                                    double.TryParse(s, out rez);
                                                    notaF += rez;
                                                }
                                                if (linie.Descriere.ToUpper() == "TOTAL INTERMEDIAR 3B" && s.Length <= 0)
                                                    nr = 4;
                                            }
                                        }
                                    }

                                    notaF /= nr;
                                    double califF = notaF;

                                    Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA") && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                    if (notaFinalaEvaluare != null)
                                    {
                                        Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == raspLinie.F10003 && p.IdQuiz == raspLinie.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                        if (val != null)
                                            val.SetValue(linieNotaFinala, califF.ToString("0.##"), null);
                                    }

                                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                                }

                            }
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
                    Session["Eval_ActiveTab"] = tabSuper.ActiveTab.Name.Replace("tab","");
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"]);
                    creat = true;
                }

                if (e.Parameter == "btnFinalizare")
                    btnFinalizare_ItemClick(sender, e);


                if (e.Parameter == "btnInapoi" || e.Parameter == "btnInainte")
                {
                    totalSec = Convert.ToInt32(Session["totalSec"].ToString());
                    indexSec = Convert.ToInt32(hf["IdSec"]) - 1;
                    //bool foundSecvParcurs = false;
                    //bool foundSectiuneCurentaParcurs = false;
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

                    txtNrSectiune.Text = (indexSec + 1).ToString();
                    Session["indexSec"] = indexSec;
                    CreeazaSectiune("Super" + Session["Eval_ActiveTab"].ToString());

                    return;
                }

                
                string sectiune = e.Parameter;
                if (sectiune.IndexOf("Sect") >= 0 && General.IsNumeric(sectiune.Replace("Sect", "")))
                    indexSec = Convert.ToInt32(sectiune.Replace("Sect", "")) - 1;

                txtNrSectiune.Text = (indexSec + 1).ToString();
                hf["IdSec"] = (indexSec + 1).ToString();

                Session["indexSec"] = indexSec;
                if (!creat)
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
                //Florin 2019.06.27
                SalveazaGridurile();
                Session["NumeGriduri"] = "";


                divIntrebari.Controls.Clear();
                grIntrebari = new HtmlTable();
                grIntrebari.CellSpacing = 12;
                grIntrebari.ID = "grIntrebari" + super;
                grIntrebari.Width = "100%";
                int idParinte = lstSec.ElementAt(indexSec);

                var arrIntre = lstEval_QuizIntrebari.Where(p => p.Ordine != null && p.Ordine.Contains("-" + idParinte + "-")).OrderBy(p => p.OrdineAfisare);
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

                int blocat = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Blocat"",0)) FROM ""Eval_DrepturiTab"" WHERE ""IdQuiz"" = @1 AND ""Pozitie"" = @2 AND ""TabIndex"" = @3 ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), super.Replace("Super",""), indexSec+1 }), 0));
                divIntrebari.ClientEnabled = blocat == 1 ? false : true;


                //Florin 2020.11.11
                //Work Around - pt cazul cand in gridul de competente se adauga coloana goale dupa callback
                if (General.Nz(Session["NumeGriduri"], "").ToString() != "")
                {
                    string[] arr = Session["NumeGriduri"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ASPxGridView grDate = grIntrebari.FindControl(arr[i]) as ASPxGridView;
                        if (grDate != null)
                        {
                            List<GridViewDataColumn> lst = new List<GridViewDataColumn>();
                            foreach (GridViewDataColumn col in grDate.Columns.OfType<GridViewDataColumn>())
                            {
                                if (col.FieldName == "" && col.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                                    lst.Add(col);
                            }

                            foreach (GridViewDataColumn col in lst)
                            {
                                grDate.Columns.Remove(col);
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

        private void CreeazaControl(Eval_QuizIntrebari ent, string super)
        {
            try
            {
                dynamic ctl = null;

                //Florin 2020.01.05
                string denCateg = "";

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
                        case 1: //TextSimplu
                            ctl = CreeazaTextEdit(ent.Id, 1, super);
                            break;
                        case 2: //Lista derulanta
                            ctl = CreeazaCombo(ent.Id, (int)ent.TipValoare, super);
                            break;
                        case 3: //Butoane radio
                            ctl = CreeazaButoaneRadio(ent.Id, (int)ent.TipValoare, super, ent.Orientare);
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
                        case 19: //Post Manager
                        case 20: //Nume Manager N + 2  
                        case 21: // Post Manager N + 2
                        case 60: //Supervizor3 Nume
                        case 61: //Supervizor3 Post
                        case 62: //Supervizor4 Nume
                        case 63: //Supervizor4 Post
                        case 64: //Supervizor5 Nume
                        case 65: //Supervizor5 Post
                        case 66: //Supervizor6 Nume
                        case 67: //Supervizor6 Post
                            //Radu 30.10.2020 - am grupat toate optiunile de mai sus
                            ctl = CreeazaTextEdit(ent.Id, 1, super, Convert.ToInt32(ent.TipData));
                            break;
                        case 23: //Obiective individuale
                            ctl = CreeazaObiectiveIndividuale(ent.Id, 1, super);

                            //Florin 2020.01.05
                            int idCateg = lstEval_QuizIntrebari.Where(p => p.Id == ent.Id).FirstOrDefault().IdCategObiective ?? -99;
                            denCateg = General.Nz(General.ExecutaScalar($@"SELECT ""Denumire"" FROM ""Eval_tblCategorieObiective"" WHERE ""Id""=@1", new object[] { idCateg }), "").ToString();
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
                        case 68: //tabel din view Others
                            ctl = CreeazaLink(ent.Descriere, ent.Id);
                            break;
                    }

                    if (ctl != null)
                    {
                        var ert = Convert.ToInt32(Session["Eval_ActiveTab"]);
                        ert = Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1));
                        ert = Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1));
                        ert = Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1));


                        //Florin 2020.01.23 - daca utilizatorul conectat este cel de pe circuit si a terminat evaluarea
                        if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Aprobat"], 1)) == 1)
                            ctl.Enabled = false;

                        //Florin 2020.01.23 - daca utilizatorul conectat intra pe alt tab decat cel care nu este al lui
                        if (Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)))
                            ctl.Enabled = false;

                        ////Radu 11.02.2019 - am adaugat conditia idCateg = 0
                        //if ((Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))) || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1)) == 0)
                        //    ctl.Enabled = false;
                        ////ctl.ReadOnly = true;
                        ////ctl.Enabled = false;

                        //Florin 2019.10.23 - s-a rescris functia de mai sus pentru a tine cont si de respecta ordinea
                        if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1)
                            ctl.Enabled = false;

                        int respectaOrdinea = 0;
                        DataTable entCir = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                        if (entCir != null && entCir.Rows.Count > 0) respectaOrdinea = Convert.ToInt32(entCir.Rows[0]["RespectaOrdinea"] != DBNull.Value ? entCir.Rows[0]["RespectaOrdinea"].ToString() : "0");

                        if (respectaOrdinea == 1)
                        {
                            if ((Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))) || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1)) == 0)
                                ctl.Enabled = false;
                        }
                        else
                        {
                            //NOP
                        }
                        //Florin 2019.10.23  END


                        if (ent.TipData == 9 || ent.TipData == 16 || ent.TipData == 24 || ent.TipData == 68) //este eticheta sau rating global
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
                            //Florin 2020.01.05 - am adaugat si categorie obiectiv
                            ASPxLabel lbl = CreeazaEticheta(ent.Descriere + "  " + denCateg, ent.Id);
                            if (ent.Orientare == 1 && ent.TipData != 3) //orientare orizontala      Radu 19.04.2019 - pentru butoane radio s-a facut o exceptie
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
                gr.Width = new Unit(100, UnitType.Percentage);
                gr.ID = "grTabela" + "_WXY_" + id.ToString();
                gr.ClientInstanceName = "grTabela" + "_WXY_" + id.ToString();
                gr.ClientIDMode = ClientIDMode.Static;
                gr.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
                gr.SettingsText.ConfirmDelete = "";    

                gr.SettingsBehavior.AllowFocusedRow = true;
                gr.SettingsBehavior.EnableCustomizationWindow = true;
                gr.SettingsBehavior.AllowSelectByRowClick = true;
                gr.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn;
                gr.Settings.ShowFilterRow = false;
                gr.Settings.ShowGroupPanel = false;
                gr.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                gr.SettingsSearchPanel.Visible = false;
                gr.AutoGenerateColumns = false;
                gr.ClientSideEvents.ContextMenu = "ctx";
               
                gr.SettingsBehavior.ConfirmDelete = true;
                gr.SettingsText.ConfirmDelete = "Confirmati operatia de stergere?";

                gr.SettingsEditing.Mode = GridViewEditingMode.Inline;
                //gr.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                //gr.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;
                //gr.SettingsEditing.BatchEditSettings.ShowConfirmOnLosingChanges = false;

                //gr.BatchUpdate += gr_BatchUpdate;
                //gr.InitNewRow += gr_InitNewRow;
                gr.RowInserting += gr_RowInserting;
                gr.RowUpdating += gr_RowUpdating;
                gr.RowDeleting += gr_RowDeleting;
                Session["NumeGriduri"] += ";" + gr.ID;

                gr.KeyFieldName = "IdQuiz;F10003;Id;Linia";

                #endregion


                #region Grid Command Buttons
                gr.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                gr.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";

                gr.SettingsCommandButton.DeleteButton.Image.ToolTip = "Sterge";
                gr.SettingsCommandButton.DeleteButton.Image.Url = "~/Fisiere/Imagini/Icoane/sterge.png";

                gr.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                gr.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                gr.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                gr.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                gr.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                gr.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";

                gr.SettingsCommandButton.EditButton.Image.ToolTip = "Modifica";
                gr.SettingsCommandButton.EditButton.Image.Url = "~/Fisiere/Imagini/Icoane/edit.png";

                #endregion


                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowDeleteButton = true;
                colCommand.ShowEditButton = true;
                colCommand.ShowNewButtonInHeader = true;
                colCommand.VisibleIndex = 0;
                colCommand.Caption = " ";
                colCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image;
                gr.Columns.Add(colCommand);

                //string[] arr = { "IdQuiz", "F10003", "Id", "Linia", "TipData", "Descriere", super, "USER_NO", "TIME" };
                string[] arr = { "IdQuiz", "F10003", "Id", "Linia", "TipData", "USER_NO", "TIME" };

                for (int i = 0; i <= arr.Length - 1; i++)
                {
                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                    col.FieldName = arr[i];
                    col.Name = Dami.TraduCuvant(arr[i]);
                    col.Caption = Dami.TraduCuvant(arr[i]);
                    col.Visible = false;
                    col.ShowInCustomizationForm = false;
                    gr.Columns.Add(col);
                }

                DataTable dtConfig = General.IncarcaDT(@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE ""IdQuiz""=@1 AND ""IdLinie""=@2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), id });
                for (int i = 0; i < dtConfig.Rows.Count; i++)
                {
                    DataRow dr = dtConfig.Rows[i];
                    string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + dr["Coloana"];

                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                    col.FieldName = camp;
                    col.Name = camp;
                    col.Caption = Dami.TraduCuvant(General.Nz(dr["Alias"], "Coloana " + dr["Coloana"]).ToString());
                    col.Width = Convert.ToInt32(General.Nz(dr["Lungime"],250));
                    col.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
                    gr.Columns.Add(col);
                }

                ////gr.DataSource = lstEval_RaspunsLinii.Where(p => p.Id == id);
                //DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                //DataRow[] arrDr = dtTbl.Select("Id=" + id);

                //if (arrDr.Count() > 0)
                //{
                //    DataTable dtTmp = arrDr.CopyToDataTable();
                //    dtTmp.PrimaryKey = new DataColumn[] { dtTmp.Columns["IdQuiz"], dtTmp.Columns["F10003"], dtTmp.Columns["Id"], dtTmp.Columns["Linia"] };
                //    gr.DataSource = dtTmp;
                //} 
                //else
                //    gr.DataSource = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE 1=2");
                ////gr.DataSource = dtTbl;
                ////gr.KeyFieldName = "IdQuiz; F10003; Id; Linia";
                //gr.KeyFieldName = "IdQuiz;F10003;Id;Linia";
                //gr.DataBind();
                ////tableBBB.Columns["IdQuiz"], tableBBB.Columns["F10003"], tableBBB.Columns["Id"], tableBBB.Columns["Linia"]
                ///

                DataTable dt = new DataTable();
                if (Session["Eval_RaspunsLinii_Tabel"] == null)
                {
                    dt = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND ""F10003"" = @2 AND ""Id""=@3", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), id });
                    Session["Eval_RaspunsLinii_Tabel"] = dt;
                }
                else
                    dt = Session["Eval_RaspunsLinii_Tabel"] as DataTable;

                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdQuiz"], dt.Columns["F10003"], dt.Columns["Id"], dt.Columns["Linia"] };

                gr.DataSource = dt;
                gr.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return gr;
        }

        private void gr_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                ASPxGridView grDate = sender as ASPxGridView;
                DataTable dt = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                if (dt.Rows.Count == 1)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        found[camp] = DBNull.Value;
                    }
                }
                else
                    found.Delete();

                e.Cancel = true;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void gr_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                ASPxGridView grDate = sender as ASPxGridView;
                DataTable dt = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                dr["TIME"] = DateTime.Now;

                for (int i = 1; i <= 6; i++)
                {
                    string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                    dr[camp] = e.NewValues[camp];
                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["Eval_RaspunsLinii_Tabel"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void gr_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            try
            {
                ASPxGridView grDate = sender as ASPxGridView;
                DataTable dt = Session["Eval_RaspunsLinii_Tabel"] as DataTable;
                DataRow dr = dt.NewRow();

                dr["F10003"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                dr["IdQuiz"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                dr["Id"] = Convert.ToInt32(grDate.ID.Split('_')[grDate.ID.Split('_').Count() - 1]);
                dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                dr["TIME"] = DateTime.Now;

                int max = Convert.ToInt32(General.Nz(dt.Compute("MAX(Linia)", "Id=" + dr["Id"]), 0));
                dr["Linia"] = max + 1;

                //foreach (KeyValuePair<string, object> item in e.NewValues)
                //{
                //    dr[item.Key] = item.Value ?? DBNull.Value;
                //}

                for (int i = 1; i <= 6; i++)
                {
                    string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                    dr[camp] = e.NewValues[camp];
                }

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["Eval_RaspunsLinii_Tabel"] = dt;
                grDate.DataSource = dt;
                //DataRow[] arrDr = dt.Select("Id=" + dr["Id"]);
                //grDate.DataSource = arrDr.CopyToDataTable();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //private void gr_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e)
        //{
        //    try
        //    {
        //        ASPxGridView grid = sender as ASPxGridView;
        //        e.NewValues["IdQuiz"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
        //        e.NewValues["F10003"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
        //        e.NewValues["Id"] = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);

        //        DataTable dt = grid.DataSource as DataTable;
        //        int cnt = 0;
        //        if (dt != null)
        //            cnt = dt.Rows.Count;
        //        e.NewValues["Linia"] = cnt + 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void gr_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        //{
        //    try
        //    {
        //        ASPxGridView grid = sender as ASPxGridView;
        //        grid.CancelEdit();

        //        List<Eval_RaspunsLinii> lst = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;

        //        for (int x = 0; x < e.InsertValues.Count; x++)
        //        {
        //            ASPxDataInsertValues vals = e.InsertValues[x] as ASPxDataInsertValues;
        //            Eval_RaspunsLinii cls = new Eval_RaspunsLinii();
        //            cls.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
        //            cls.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
        //            cls.Id = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
        //            cls.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
        //            cls.TIME = DateTime.Now;

        //            int max = lst.Where(p => p.IdQuiz == cls.IdQuiz && p.F10003 == cls.F10003 && p.Id == cls.Id).Max(p => p.Linia);
        //            cls.Linia = max + 1;

        //            for (int i = 1; i <= 6; i++)
        //            {
        //                string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
        //                PropertyInfo val = cls.GetType().GetProperty(camp);
        //                if (val != null)
        //                {
        //                    val.SetValue(cls, vals.NewValues[camp], null);
        //                }
        //            }

        //            lst.Add(cls);
        //        }

        //        for (int x = 0; x < e.UpdateValues.Count; x++)
        //        {
        //            ASPxDataUpdateValues vals = e.UpdateValues[x] as ASPxDataUpdateValues;
        //            object[] keys = new object[] { vals.Keys[0] };

        //            Eval_RaspunsLinii cls = lst.Where(p => p.IdQuiz == Convert.ToInt32(vals.Keys[0]) && p.F10003 == Convert.ToInt32(vals.Keys[1]) && p.Id == Convert.ToInt32(vals.Keys[2]) && p.Linia == Convert.ToInt32(vals.Keys[3])).FirstOrDefault();
        //            if (cls == null) return;

        //            cls.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
        //            cls.TIME = DateTime.Now;

        //            for (int i = 1; i <= 6; i++)
        //            {
        //                string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
        //                PropertyInfo val = cls.GetType().GetProperty(camp);
        //                if (val != null)
        //                {
        //                    val.SetValue(cls, vals.NewValues[camp], null);
        //                }
        //            }
        //        }

        //        for (int x = 0; x < e.DeleteValues.Count; x++)
        //        {
        //            ASPxDataDeleteValues vals = e.DeleteValues[x] as ASPxDataDeleteValues;
        //            object[] keys = new object[] { vals.Keys[0] };

        //            Eval_RaspunsLinii cls = lst.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
        //            if (cls == null) return;
        //            lst.Remove(cls);

        //            //List<Eval_CompetenteAngajatTemp> lstSterse = Session["lstEval_CompetenteAngajatTemp_Sterse"] as List<Eval_CompetenteAngajatTemp>;
        //            //if (lstSterse == null) lstSterse = new List<Eval_CompetenteAngajatTemp>();
        //            //lstSterse.Add(cls);
        //            //Session["lstEval_CompetenteAngajatTemp_Sterse"] = lstSterse;
        //        }

        //        Session["lstEval_RaspunsLinii"] = lst;

        //        e.Handled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }


        //}

        //private void gr_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        //{
        //    try
        //    {
        //        ASPxGridView grid = sender as ASPxGridView;
        //        grid.CancelEdit();

        //        DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;

        //        for (int x = 0; x < e.InsertValues.Count; x++)
        //        {
        //            ASPxDataInsertValues vals = e.InsertValues[x] as ASPxDataInsertValues;
        //            DataRow dr = dtTbl.NewRow();
        //            dr["F10003"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
        //            dr["IdQuiz"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
        //            dr["Id"] = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
        //            dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
        //            dr["TIME"] = DateTime.Now;

        //            //int max = lst.Where(p => p.IdQuiz == cls.IdQuiz && p.F10003 == cls.F10003 && p.Id == cls.Id).Max(p => p.Linia);
        //            //cls.Linia = max + 1;
        //            //DataTable dtFiltru = dtTbl.Select("Id=" + dr["Id"]).CopyToDataTable();
        //            int max = Convert.ToInt32(dtTbl.Compute("MAX(Linia)", "Id=" + dr["Id"]));
        //            dr["Linia"] = max + 1;

        //            for (int i = 1; i <= 6; i++)
        //            {
        //                string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
        //                dr[camp] = vals.NewValues[camp];
        //                //PropertyInfo val = cls.GetType().GetProperty(camp);
        //                //if (val != null)
        //                //{
        //                //    val.SetValue(cls, vals.NewValues[camp], null);
        //                //}
        //            }

        //            dtTbl.Rows.Add(dr);
        //        }

        //        for (int x = 0; x < e.UpdateValues.Count; x++)
        //        {
        //            //ASPxDataUpdateValues vals = e.UpdateValues[x] as ASPxDataUpdateValues;
        //            //object[] keys = new object[] { vals.Keys[0] };

        //            //Eval_RaspunsLinii cls = lst.Where(p => p.IdQuiz == Convert.ToInt32(vals.Keys[0]) && p.F10003 == Convert.ToInt32(vals.Keys[1]) && p.Id == Convert.ToInt32(vals.Keys[2]) && p.Linia == Convert.ToInt32(vals.Keys[3])).FirstOrDefault();
        //            //if (cls == null) return;

        //            //cls.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
        //            //cls.TIME = DateTime.Now;

        //            //for (int i = 1; i <= 6; i++)
        //            //{
        //            //    string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
        //            //    PropertyInfo val = cls.GetType().GetProperty(camp);
        //            //    if (val != null)
        //            //    {
        //            //        val.SetValue(cls, vals.NewValues[camp], null);
        //            //    }
        //            //}
        //        }

        //        for (int x = 0; x < e.DeleteValues.Count; x++)
        //        {
        //            ASPxDataDeleteValues vals = e.DeleteValues[x] as ASPxDataDeleteValues;
        //            object[] keys = new object[] { vals.Keys[0] };

        //            //Eval_RaspunsLinii cls = lst.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
        //            //if (cls == null) return;
        //            //lst.Remove(cls);

        //            //List<Eval_CompetenteAngajatTemp> lstSterse = Session["lstEval_CompetenteAngajatTemp_Sterse"] as List<Eval_CompetenteAngajatTemp>;
        //            //if (lstSterse == null) lstSterse = new List<Eval_CompetenteAngajatTemp>();
        //            //lstSterse.Add(cls);
        //            //Session["lstEval_CompetenteAngajatTemp_Sterse"] = lstSterse;
        //        }

        //        Session["Eval_RaspunsLinii_Tabel"] = dtTbl;

        //        e.Handled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }


        //}


        private void gr_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grid = sender as ASPxGridView;
                grid.CancelEdit();

                DataTable dtTbl = Session["Eval_RaspunsLinii_Tabel"] as DataTable;

                for (int x = 0; x < e.InsertValues.Count; x++)
                {
                    ASPxDataInsertValues vals = e.InsertValues[x] as ASPxDataInsertValues;
                    DataRow dr = dtTbl.NewRow();
                    dr["F10003"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                    dr["IdQuiz"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                    dr["Id"] = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    int max = Convert.ToInt32(dtTbl.Compute("MAX(Linia)", "Id=" + dr["Id"]));
                    dr["Linia"] = max + 1;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = vals.NewValues[camp];
                    }

                    dtTbl.Rows.Add(dr);
                }

                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues vals = e.UpdateValues[x] as ASPxDataUpdateValues;

                    object[] keys = new object[vals.Keys.Count];
                    for (int y = 0; y < vals.Keys.Count; y++)
                    { keys[y] = vals.Keys[y]; }

                    DataRow dr = dtTbl.Rows.Find(keys);

                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = vals.NewValues[camp];
                    }
                }

                for (int x = 0; x < e.DeleteValues.Count; x++)
                {
                    ASPxDataDeleteValues vals = e.DeleteValues[x] as ASPxDataDeleteValues;

                    object[] keys = new object[vals.Keys.Count];
                    for (int y = 0; y < vals.Keys.Count; y++)
                    { keys[y] = vals.Keys[y]; }

                    DataRow dr = dtTbl.Rows.Find(keys);
                    dr["USER_NO"] = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    dr["TIME"] = DateTime.Now;

                    for (int i = 1; i <= 6; i++)
                    {
                        string camp = "Super" + Session["Eval_ActiveTab"].ToString() + "_" + i;
                        dr[camp] = DBNull.Value;
                    }
                }

                Session["Eval_RaspunsLinii_Tabel"] = dtTbl;

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }


        }


        //private void Gr_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        //{
        //    try
        //    {
        //        object[] keys = new object[e.Keys.Count];
        //        for (int i = 0; i < e.Keys.Count; i++)
        //        { keys[i] = e.Keys[i]; }
        //        ASPxGridView grid = sender as ASPxGridView;                
        //        GridViewDataTextColumn col = grid.Columns[7] as GridViewDataTextColumn;

        //        Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
                
        //        if (raspLinie != null)
        //        {
        //            PropertyInfo piValue = raspLinie.GetType().GetProperty(col.FieldName);
        //            if (piValue != null)
        //            {
        //                piValue.SetValue(raspLinie, Convert.ChangeType(e.NewValues[col.FieldName] ?? DBNull.Value, piValue.PropertyType), null);
        //            }
        //        }

        //        e.Cancel = true;

        //        grid.CancelEdit();
        //        grid.DataSource = lstEval_RaspunsLinii.Where(p => p.IdQuiz == Convert.ToInt32(keys[0]) && p.F10003 == Convert.ToInt32(keys[1]) && p.Id == Convert.ToInt32(keys[2]) && p.Linia == Convert.ToInt32(keys[3])).FirstOrDefault();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        txt.Text = piValue.GetValue(raspLinie, null).ToString();
                    }
                }
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
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        cmb.Value = piValue.GetValue(raspLinie, null).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return cmb;
        }

        private ASPxRadioButtonList CreeazaButoaneRadio(int id, int idGrupValori, string super, int orientare)
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

                radio.RepeatDirection = (RepeatDirection)Enum.Parse(typeof(RepeatDirection), orientare == 1 ? "Horizontal" : "Vertical");  //Radu 19.04.2019

                //Temporar - HardCodat
                //daca este calificativ final si este angajat nu are acces
                //675,677,681,685,687,691,695,699,703,707
                if (super.ToLower() == "super1" && "675,677,681,685,687,691,695,699,703,707,".IndexOf(id.ToString() + ",") >= 0)
                    radio.ReadOnly = true;

                Eval_RaspunsLinii raspLinie = lstEval_RaspunsLinii.Where(p => p.Id == id).FirstOrDefault();

                if (raspLinie != null)
                {
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        radio.Value = piValue.GetValue(raspLinie, null).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return radio;
        }

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
                    PropertyInfo piValue = raspLinie.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                    if (piValue != null)
                    {
                        chk.Checked = (piValue.GetValue(raspLinie, null).ToString() == "1" || piValue.GetValue(raspLinie, null).ToString().ToUpper() == "TRUE" ? true : false);
                    }
                }
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
            Session["CompletareChestionarObiectiv_LinieQuiz"] = id;
            ASPxGridView grDateObiective = new ASPxGridView();
            if (Session["lstEval_ObiIndividualeTemp"] == null)
            {
                DataTable dtObiIndividuale = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE F10003=@1 ORDER BY CAST(""Obiectiv"" AS varchar(4000)), CAST(""Activitate"" AS varchar(4000)) ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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

                //Florin 2019.06.26
                grDateObiective.ClientInstanceName = "grDateObiective" + "_WXY_" + id.ToString();
                grDateObiective.ClientIDMode = ClientIDMode.Static;
                grDateObiective.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;


                grDateObiective.SettingsBehavior.AllowFocusedRow = true;
                grDateObiective.SettingsBehavior.EnableCustomizationWindow = true;
                grDateObiective.SettingsBehavior.AllowSelectByRowClick = true;
                grDateObiective.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn;
                grDateObiective.Settings.ShowGroupPanel = false;
                grDateObiective.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                grDateObiective.SettingsSearchPanel.Visible = false;
                grDateObiective.AutoGenerateColumns = false;
                
                grDateObiective.ClientSideEvents.ContextMenu = "ctx";
                grDateObiective.SettingsEditing.Mode = GridViewEditingMode.Inline;

                grDateObiective.SettingsBehavior.ConfirmDelete = true;
                grDateObiective.SettingsText.ConfirmDelete = "Confirmati operatia de stergere?";

                //grDateObiective.RowDeleting += GrDateObiective_RowDeleting;
                //grDateObiective.AutoFilterCellEditorInitialize += GrDateObiective_AutoFilterCellEditorInitialize;
                //grDateObiective.RowInserting += GrDateObiective_RowInserting;
                //grDateObiective.RowUpdating += GrDateObiective_RowUpdating;
                grDateObiective.InitNewRow += GrDateObiective_InitNewRow;

                grDateObiective.SettingsEditing.Mode = GridViewEditingMode.Batch;
                grDateObiective.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                grDateObiective.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;
                grDateObiective.SettingsEditing.BatchEditSettings.ShowConfirmOnLosingChanges = false;


                //Florin 2019.06.26
                grDateObiective.BatchUpdate += GrDateObiective_BatchUpdate;
                Session["NumeGriduri"] += ";" + grDateObiective.ID;

                grDateObiective.CustomSummaryCalculate += grDate_CustomSummaryCalculate;

                //Radu 19.04.2019
                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 24 || Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 25)
                {
                    string endCallBackFunctionGrDate = @"function " + grDateObiective.ID + @"_EndCallBack(s, e) { pnlSectiune.PerformCallback('CreeazaSectiune');  }";
                    grDateObiective.ClientSideEvents.EndCallback = endCallBackFunctionGrDate;
                }
                #endregion

                #region Grid Default Columns
                GridViewCommandColumn colCommand = new GridViewCommandColumn();
                colCommand.Width = 80;
                colCommand.ShowDeleteButton = clsEval_ConfigObTemplate.PoateSterge == 1 ? true : false;
                colCommand.ShowNewButtonInHeader = clsEval_ConfigObTemplate.PoateAdauga == 1 ? true : false;
                //colCommand.ShowEditButton = clsEval_ConfigObTemplate.PoateModifica == 1 ? true : false;
                grDateObiective.Enabled = clsEval_ConfigObTemplate.PoateModifica == 1 ? true : false;
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

                GridViewDataTextColumn colCateg = new GridViewDataTextColumn();
                colCateg.FieldName = "IdCategObiective";
                colCateg.Name = "IdCategObiective";
                colCateg.Caption = "IdCategObiective";
                colCateg.Visible = false;
                grDateObiective.Columns.Add(colCateg);
                #endregion


                #region AddColumns

                string formulaSql1 = "";
                string formulaSql2 = "";
                int y = 34;
                foreach (Eval_ConfigObTemplateDetail clsConfigDetail in lstEval_ConfigObTemplateDetail.Where(p=>p.TemplateId==TemplateIdObiectiv).OrderBy(p => p.Ordine ?? 99))
                {
                    //Florin 2020.09.14 Begin
                    if (clsConfigDetail.ColumnName == "Total1" || clsConfigDetail.ColumnName == "Total2")
                    {
                        GridViewDataTextColumn colFormula = new GridViewDataTextColumn();
                        string colName = clsConfigDetail.ColumnName.Replace("Total", "FormulaSql");
                        colFormula.FieldName = colName;
                        colFormula.Name = colName;
                        colFormula.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
                        colFormula.UnboundExpression = clsConfigDetail.FormulaSql;
                        colFormula.Visible = false;

                        grDateObiective.Columns.Add(colFormula);

                        if (clsConfigDetail.ColumnName == "Total1") formulaSql1 = clsConfigDetail.FormulaSql;
                        if (clsConfigDetail.ColumnName == "Total2") formulaSql2 = clsConfigDetail.FormulaSql;
                    }
                    //Florin 2020.09.14 End

                    //Florin 2020.12.09 Begin
                    if (Convert.ToString(clsConfigDetail.TotalColoana) != "")
                    {
                        grDateObiective.Settings.ShowFooter = true;
                        grDateObiective.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
                        ASPxSummaryItem s = new ASPxSummaryItem();
                        string camp = clsConfigDetail.ColumnName;
                        if (clsConfigDetail.TipValoare == 1)
                        {
                            switch (clsConfigDetail.ColumnName)
                            {
                                case "Obiectiv":
                                    camp = "IdObiectiv";
                                    break;
                                case "Activitate":
                                    camp = "IdActivitate";
                                    break;
                                case "Calificativ":
                                    camp = "IdCalificativ";
                                    break;
                            }
                        }
                        s.FieldName = camp;
                        s.ShowInColumn = camp;
                        switch (clsConfigDetail.TotalColoana)
                        {
                            case 1:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Suma {0:N0}";
                                break;
                            case 2:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Suma {0:N2}";
                                break;
                            case 3:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Media {0:N0}";
                                break;
                            case 4:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Media {0:N2}";
                                break;
                            case 5:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Val. min. {0}";
                                break;
                            case 6:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Val. max. {0}";
                                break;
                        }
                        grDateObiective.TotalSummary.Add(s);
                    }
                    //Florin 2020.12.09 End

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
                                    string strSQLObiectiv = @"select ob.""IdObiectiv"" as ""Id"", CAST(ob.""Obiectiv"" AS varchar(4000)) as ""Denumire""
                                                            from ""Eval_ListaObiectivDet"" det
                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                            where det.""IdLista"" = @1
                                                            and setAng.""Id"" = @2
                                                            group by ob.""IdObiectiv"", CAST(ob.""Obiectiv"" AS varchar(4000)) ";

                                    if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1)
                                        strSQLObiectiv = @"SELECT ""IdObiectiv"" AS ""Id"", CAST(""Obiectiv"" AS varchar(4000)) AS ""Denumire"" FROM ""Eval_Obiectiv""  ";

                                    DataTable dtObiectiv = General.IncarcaDT(strSQLObiectiv, new object[] { clsConfigDetail.IdNomenclator, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", CAST(obAct.""Activitate"" AS varchar(4000)) as ""Denumire""
                                                                        from ""Eval_ListaObiectivDet"" det
                                                                        join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                        join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                        join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                        where det.""IdLista"" = @1
                                                                        and setAng.""Id"" = @2
                                                                        group by ob.""IdObiectiv"", obAct.""IdActivitate"", CAST(obAct.""Activitate"" AS varchar(4000)) ";

                                    if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1)
                                        strSQLObiectivActivitate = @"SELECT ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", CAST(obAct.""Activitate"" AS varchar(4000)) as ""Denumire""
                                                                    FROM ""Eval_Obiectiv"" ob
                                                                    INNER JOIN ""Eval_ObiectivXActivitate"" obAct on ob.""IdObiectiv"" = obAct.""IdObiectiv"" ";

                                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { clsConfigDetail.IdNomenclator, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                                colObiectiv.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant("Obiectiv");
                                colObiectiv.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colObiectiv.ReadOnly = !clsConfigDetail.Editare;
                                colObiectiv.Visible = clsConfigDetail.Vizibil;

                                colObiectiv.PropertiesComboBox.TextField = "Denumire";
                                colObiectiv.PropertiesComboBox.ValueField = "Id";
                                colObiectiv.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDownList;
                                //Florin 2020.01.29 - comentat deoarece apare eroarea - Unable to set property 'collectCallbackIDs' of undefined or null reference
                                //colObiectiv.PropertiesComboBox.EnableCallbackMode = true;
                                colObiectiv.PropertiesComboBox.ValidationSettings.RequiredField.IsRequired = true;
                                colObiectiv.PropertiesComboBox.ValidationSettings.Display = Display.None;
                                colObiectiv.PropertiesComboBox.ClientSideEvents.SelectedIndexChanged = "cmbObiectiv_SelectedIndexChanged";
                                colObiectiv.PropertiesComboBox.ClientInstanceName = "ObjectiveEditor";

                                colObiectiv.PropertiesComboBox.DataSource = lstObiective;

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
                                colObiectiv.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant("Obiectiv");
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
                            if (clsConfigDetail.TipValoare == 1)
                            {
                                #region getDS values
                                if (Session["feedEval_Obiectiv"] == null)
                                {
                                    string strSQLObiectiv = @"select ob.""IdObiectiv"" as ""Id"", CAST(ob.""Obiectiv"" AS varchar(4000)) as ""Denumire""
                                                            from ""Eval_ListaObiectivDet"" det
                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                            where det.""IdLista"" = @1
                                                            and setAng.""Id"" = @2
                                                            group by ob.""IdObiectiv"", CAST(ob.""Obiectiv"" AS varchar(4000)) ";
                                    
                                    DataTable dtObiectiv = General.IncarcaDT(strSQLObiectiv, new object[] { clsConfigDetail.IdNomenclator, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", CAST(obAct.""Activitate"" AS varchar(4000)) as ""Denumire""
                                                                        from ""Eval_ListaObiectivDet"" det
                                                                        join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                        join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                        join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                        where det.""IdLista"" = @1
                                                                        and setAng.""Id"" = @2
                                                                        group by ob.""IdObiectiv"", obAct.""IdActivitate"", CAST(obAct.""Activitate"" AS varchar(4000)) ";
                                    
                                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { clsConfigDetail.IdNomenclator, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                                colActivitate.Caption = (clsConfigDetail.Alias ?? Dami.TraduCuvant("Activitate")) + " - " + y;
                                colActivitate.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colActivitate.ReadOnly = !clsConfigDetail.Editare;
                                colActivitate.Visible = clsConfigDetail.Vizibil;

                                colActivitate.PropertiesComboBox.TextField = "Denumire";
                                colActivitate.PropertiesComboBox.ValueField = "Id";
                                colActivitate.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDownList;
                                //Florin 2020.01.29 - comentat deoarece apare eroarea - Unable to set property 'collectCallbackIDs' of undefined or null reference
                                //colActivitate.PropertiesComboBox.EnableCallbackMode = true;
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
                                GridViewDataMemoColumn colActivitate = new GridViewDataMemoColumn();
                                colActivitate.FieldName = "Activitate";
                                colActivitate.PropertiesMemoEdit.Rows = 5;
                                colActivitate.PropertiesMemoEdit.Height = Unit.Percentage(100);
                                colActivitate.Name = "Activitate";
                                colActivitate.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant("Activitate");
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
                                colCalificativ.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colCalificativ.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colCalificativ.ReadOnly = !clsConfigDetail.Editare;
                                colCalificativ.Visible = clsConfigDetail.Vizibil;

                                colCalificativ.PropertiesComboBox.TextField = "Denumire";
                                colCalificativ.PropertiesComboBox.ValueField = "IdCalificativ";
                                colCalificativ.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDownList;
                                colCalificativ.PropertiesComboBox.DataSource = lstEval_SetCalificativDet;
                                colCalificativ.Visible = false;
                                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) != 20 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) >= 2 || tab >= 2)   //Radu 03.07.2018 - calificativul nu trebuie sa fie afisat pe tab-ul angajatului decat dupa ce acesta finalizeaza
                                    colCalificativ.Visible = true;
                                grDateObiective.Columns.Add(colCalificativ);
                                continue;
                            }
                            else
                            {
                                GridViewDataTextColumn colCalificativ = new GridViewDataTextColumn();
                                colCalificativ.FieldName = "Calificativ";
                                colCalificativ.Name = "Calificativ";
                                colCalificativ.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colCalificativ.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colCalificativ.ReadOnly = !clsConfigDetail.Editare;
                                colCalificativ.Visible = clsConfigDetail.Vizibil;
                                colCalificativ.Visible = false;
                                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) != 20 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) >= 2 || tab >= 2)   //Radu 03.07.2018 - calificativul nu trebuie sa fie afisat pe tab-ul angajatului decat dupa ce acesta finalizeaza
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
                                {
                                    GridViewDataMemoColumn col = new GridViewDataMemoColumn();
                                    col.FieldName = clsConfigDetail.ColumnName;
                                    col.Name = clsConfigDetail.ColumnName;
                                    col.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                    col.PropertiesMemoEdit.Rows = 5;
                                    col.PropertiesMemoEdit.Height = Unit.Percentage(100);
                                    col.Width = clsConfigDetail.Width;
                                    if (idCateg == "0")
                                        col.ReadOnly = !clsConfigDetail.Editare;
                                    col.Visible = clsConfigDetail.Vizibil;

                                    grDateObiective.Columns.Add(col);
                                }
                                break;
                            case "System.Decimal":
                                GridViewDataTextColumn colDecimal = new GridViewDataTextColumn();
                                colDecimal.FieldName = clsConfigDetail.ColumnName;
                                colDecimal.Name = clsConfigDetail.ColumnName;
                                colDecimal.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colDecimal.Width = clsConfigDetail.Width;
                                if (idCateg == "0")
                                    colDecimal.ReadOnly = !clsConfigDetail.Editare;
                                colDecimal.Visible = clsConfigDetail.Vizibil;

                                colDecimal.PropertiesTextEdit.DisplayFormatString = "n2";
                                colDecimal.PropertiesTextEdit.MaskSettings.Mask = "<0..99999>.<00..99g>";

                                grDateObiective.Columns.Add(colDecimal);
                                break;
                            case "System.Int32":
                                {
                                    GridViewDataTextColumn col = new GridViewDataTextColumn();
                                    col.FieldName = clsConfigDetail.ColumnName;
                                    col.Name = clsConfigDetail.ColumnName;
                                    col.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                    col.Width = clsConfigDetail.Width;
                                    if (idCateg == "0")
                                        col.ReadOnly = !clsConfigDetail.Editare;
                                    col.Visible = clsConfigDetail.Vizibil;

                                    col.PropertiesTextEdit.DisplayFormatString = "n0";
                                    col.PropertiesTextEdit.MaskSettings.Mask = "<0..99999>";

                                    grDateObiective.Columns.Add(col);
                                }
                                break;
                            case "System.DateTime":
                                {
                                    GridViewDataDateColumn col = new GridViewDataDateColumn();
                                    col.FieldName = clsConfigDetail.ColumnName;
                                    col.Name = clsConfigDetail.ColumnName;
                                    col.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                    col.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy";
                                    col.PropertiesDateEdit.EditFormatString = "dd/MM/yyyy";
                                    col.PropertiesDateEdit.EditFormat = EditFormat.Custom;
                                    col.Width = clsConfigDetail.Width;
                                    if (idCateg == "0")
                                        col.ReadOnly = !clsConfigDetail.Editare;
                                    col.Visible = clsConfigDetail.Vizibil;

                                    grDateObiective.Columns.Add(col);
                                }
                                break;
                        }

                    }
                }
                #endregion

                if (grDateObiective.Columns["Obiectiv"] != null && Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && (idCateg == "1" || idCateg == "2"))
                    grDateObiective.Columns["Obiectiv"].Caption = Dami.TraduCuvant("Puncte Forte");
                if (grDateObiective.Columns["Activitate"] != null && Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && (idCateg == "1" || idCateg == "2"))
                    grDateObiective.Columns["Activitate"].Caption = Dami.TraduCuvant("Zone de dezvoltare");

                #region Grid Command Buttons

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

                List<Eval_ObiIndividualeTemp> lst = lstEval_ObiIndividualeTemp.Where(p => p.IdLinieQuiz == id && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"]) && p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1))).ToList();
                if (formulaSql1 != "" || formulaSql2 != "")
                {
                    foreach (Eval_ObiIndividualeTemp l in lst)
                    {
                        if (formulaSql1 != "")
                            l.FormulaSql1 = formulaSql1;
                        if (formulaSql2 != "")
                            l.FormulaSql2 = formulaSql2;
                    }
                }
                grDateObiective.DataSource = lst;
                grDateObiective.KeyFieldName = "IdAuto";
                grDateObiective.DataBind();


                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20)
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

                //if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20)
                //{//daca formularul este pe pozitia 1, evaluatorul sa poata completa comentariile pe tab-ul sau
                //    if (idCateg == "0")
                //    {
                //        if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) == 1 && Session["Eval_ActiveTab"].ToString() == "1")
                //        {
                //            DataTable dt = General.IncarcaDT(@"SELECT ""IdUser"" FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 and F10003 = @2 AND ""Pozitie"" = 2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                //            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                //            {
                //                if (dt.Rows[0][0].ToString() == Session["UserId"].ToString())
                //                    grDateObiective.Enabled = true;

                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return grDateObiective;
        }

        private void grDate_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            try
            {
                ASPxSummaryItem itm = e.Item as ASPxSummaryItem;
                switch(itm.DisplayFormat)
                {
                    case "Suma {0:N0}":
                    case "Suma {0:N2}":
                        {
                            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                                e.TotalValue = 0;
                            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
                            {
                                decimal val = 0;
                                if (General.Nz(e.FieldValue, "").ToString() != "" && General.IsNumeric(e.FieldValue))
                                    val = Convert.ToDecimal(e.FieldValue);
                                e.TotalValue = Convert.ToDecimal(e.TotalValue) + val;
                            }
                        }
                        break;
                    case "Media {0:N0}":
                    case "Media {0:N2}":
                        {
                            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                                e.TotalValue = 0;
                            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
                            {
                                decimal val = 0;
                                if (General.Nz(e.FieldValue, "").ToString() != "" && General.IsNumeric(e.FieldValue))
                                    val = Convert.ToDecimal(e.FieldValue);
                                e.TotalValue = Convert.ToDecimal(e.TotalValue) + val;
                            }
                            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
                                e.TotalValue = Convert.ToDecimal(e.TotalValue) / (e.RowHandle + 1);
                        }
                        break;
                    case "Val. min. {0}":
                        {
                            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                                e.TotalValue = 999999;
                            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
                            {
                                if (General.Nz(e.FieldValue, "").ToString() != "" && General.IsNumeric(e.FieldValue))
                                {
                                    if (Convert.ToDecimal(e.FieldValue) < Convert.ToDecimal(e.TotalValue))
                                        e.TotalValue = Convert.ToDecimal(e.FieldValue);
                                }
                            }
                        }
                        break;
                    case "Val. max. {0}":
                        {
                            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
                                e.TotalValue = 0;
                            else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
                            {
                                if (General.Nz(e.FieldValue, "").ToString() != "" && General.IsNumeric(e.FieldValue))
                                {
                                    if (Convert.ToDecimal(e.FieldValue) > Convert.ToDecimal(e.TotalValue))
                                        e.TotalValue = Convert.ToDecimal(e.FieldValue);
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //Florin 2019.06.27
        private void GrDateObiective_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grid = sender as ASPxGridView;
                grid.CancelEdit();
                GridViewDataComboBoxColumn colObiectiv = (grid.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);

                //Radu 07.05.2020
                GridViewDataComboBoxColumn colCalificativ = (grid.Columns["IdCalificativ"] as GridViewDataComboBoxColumn);

                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;

                for (int x = 0; x < e.InsertValues.Count; x++)
                {
                    ASPxDataInsertValues ins = e.InsertValues[x] as ASPxDataInsertValues;
                    Eval_ObiIndividualeTemp clsNew = new Eval_ObiIndividualeTemp();
                    clsNew.IdAuto = -1 * (Convert.ToInt32(General.Nz(lst.Max(p => (int?)Math.Abs(p.IdAuto)), 0)) + 1);
                    clsNew.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                    int poz = Convert.ToInt32(Session["Eval_ActiveTab"]);

                    clsNew.Pozitie = poz;
                    clsNew.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                    clsNew.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);

                    //Florin 2020.01.27
                    ////Radu 24.10.2019
                    //clsNew.IdPeriod = lstEval_QuizIntrebari.Where(p => p.Id == clsNew.IdLinieQuiz).FirstOrDefault().IdPeriod;
                    clsNew.IdPeriod = idPerioada;

                    //Florin 2020.01.03
                    clsNew.IdCategObiective = lstEval_QuizIntrebari.Where(p => p.Id == clsNew.IdLinieQuiz).FirstOrDefault().IdCategObiective;

                    clsNew.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    clsNew.TIME = DateTime.Now;

                    //Florin 2020.01.17 - martor care ne indica daca este inregistrare goala
                    bool areValori = false;

                    foreach (DictionaryEntry de in ins.NewValues)
                    {
                        //Florin 2020.01.17 
                        if (ins.NewValues[de.Key.ToString()] != null) areValori = true;

                        switch (de.Key.ToString())
                        {
                            case "IdObiectiv":
                                clsNew.IdObiectiv = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colObiectiv != null && colObiectiv.PropertiesComboBox.Items.Count > 0)
                                    clsNew.Obiectiv = colObiectiv.PropertiesComboBox.Items.FindByValue(clsNew.IdObiectiv).Text;
                                break;
                            case "Obiectiv":
                                if (colObiectiv == null)
                                    clsNew.Obiectiv = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "IdActivitate":
                                clsNew.IdActivitate = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (lstActivitati != null && lstActivitati.Count > 0)
                                    clsNew.Activitate = General.Nz(lstActivitati.Find(p => p.Id == clsNew.IdActivitate),"").ToString();


                                if (Session["feedEval_ObiectivActivitate"] == null)
                                {
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


                                break;
                            case "Activitate":
                                clsNew.Activitate = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Pondere":
                                clsNew.Pondere = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "Descriere":
                                clsNew.Descriere = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Target":
                                clsNew.Target = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "Realizat":
                                clsNew.Realizat = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "IdCalificativ":
                                clsNew.IdCalificativ = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCalificativ != null)
                                    clsNew.Calificativ = colCalificativ.PropertiesComboBox.Items.FindByValue(clsNew.IdCalificativ).Text;
                                break;
                            case "Calificativ":
                                clsNew.Calificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ExplicatiiCalificativ":
                                clsNew.ExplicatiiCalificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara1":
                                clsNew.ColoanaSuplimentara1 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara2":
                                clsNew.ColoanaSuplimentara2 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara3":
                                clsNew.ColoanaSuplimentara3 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara4":
                                clsNew.ColoanaSuplimentara4 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Termen":
                                clsNew.Termen = (DateTime?)(ins.NewValues[de.Key.ToString()]);
                                break;
                        }

                        if (grid.Columns["FormulaSql1"] != null)
                            clsNew.FormulaSql1 = ((GridViewDataTextColumn)grid.Columns["FormulaSql1"]).UnboundExpression;
                        if (grid.Columns["FormulaSql2"] != null)
                            clsNew.FormulaSql2 = ((GridViewDataTextColumn)grid.Columns["FormulaSql2"]).UnboundExpression;

                    }

                    //Florin 2020.01.17 
                    if (areValori)
                        lst.Add(clsNew);
                }
                decimal sumaClaim = 0;
                int marca = -99;
                int idQuiz = -99;
           
                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                    object[] keys = new object[] { upd.Keys[0] };

                    ASPxDataUpdateValues ins = e.UpdateValues[x] as ASPxDataUpdateValues;
                    Eval_ObiIndividualeTemp clsUpd = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                    if (clsUpd == null) return;

                    clsUpd.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    clsUpd.TIME = DateTime.Now;

                    marca = clsUpd.F10003;
                    idQuiz = clsUpd.IdQuiz;              

                    foreach (DictionaryEntry de in ins.NewValues)
                    {
                        switch (de.Key.ToString())
                        {
                            case "IdObiectiv":
                                clsUpd.IdObiectiv = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colObiectiv != null && colObiectiv.PropertiesComboBox.Items.Count > 0)
                                    clsUpd.Obiectiv = colObiectiv.PropertiesComboBox.Items.FindByValue(clsUpd.IdObiectiv).Text;
                                break;
                            case "Obiectiv":
                                if (colObiectiv == null)
                                    clsUpd.Obiectiv = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "IdActivitate":
                                clsUpd.IdActivitate = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (lstActivitati != null && lstActivitati.Count > 0)
                                    clsUpd.Activitate = General.Nz(lstActivitati.Find(p => p.Id == clsUpd.IdActivitate), "").ToString();


                                if (Session["feedEval_ObiectivActivitate"] == null)
                                {
                                    DataTable dtObiectivActivitate = General.IncarcaDT(
                                        @"select ""IdObiectiv"" as ""Parinte"", ""IdActivitate"" as ""Id"", ""Activitate"" as ""Denumire""
                                        from  ""Eval_ObiectivXActivitate"" WHERE  ""IdActivitate"" = @1 ", new object[] { clsUpd.IdActivitate });
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

                                break;
                            case "Activitate":
                                clsUpd.Activitate = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Pondere":
                                clsUpd.Pondere = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "Descriere":
                                clsUpd.Descriere = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Target":
                                clsUpd.Target = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "Realizat":
                                clsUpd.Realizat = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "IdCalificativ":
                                clsUpd.IdCalificativ = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCalificativ != null)
                                    clsUpd.Calificativ = colCalificativ.PropertiesComboBox.Items.FindByValue(clsUpd.IdCalificativ).Text;
                                break;
                            case "Calificativ":
                                clsUpd.Calificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ExplicatiiCalificativ":
                                clsUpd.ExplicatiiCalificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara1":
                                clsUpd.ColoanaSuplimentara1 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara2":
                                clsUpd.ColoanaSuplimentara2 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara3":
                                clsUpd.ColoanaSuplimentara3 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ColoanaSuplimentara4":
                                clsUpd.ColoanaSuplimentara4 = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Termen":
                                clsUpd.Termen = (DateTime?)(ins.NewValues[de.Key.ToString()]);
                                break;
                        }

                        if (grid.Columns["FormulaSql1"] != null)
                            clsUpd.FormulaSql1 = ((GridViewDataTextColumn)grid.Columns["FormulaSql1"]).UnboundExpression;
                        if (grid.Columns["FormulaSql2"] != null)
                            clsUpd.FormulaSql2 = ((GridViewDataTextColumn)grid.Columns["FormulaSql2"]).UnboundExpression;
                    }

                    switch(General.Nz(Session["IdClient"], 1).ToString())
                    {
                        case "20":
                            {
                                try
                                {
                                    if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) >= 2)   //se doreste ca rating-ul sa fie afisat numai dupa ce angajatul finalizeaza
                                    {
                                        decimal activitate = 0;
                                        if (decimal.TryParse(clsUpd.Activitate, out activitate))
                                        {
                                            clsUpd.Target = (Convert.ToDecimal(clsUpd.Pondere) / Convert.ToDecimal(clsUpd.Activitate)) * 100;
                                            if (clsUpd.Target < 80) clsUpd.IdCalificativ = 1;
                                            if (80 <= clsUpd.Target && clsUpd.Target < 95) clsUpd.IdCalificativ = 2;
                                            if (95 <= clsUpd.Target && clsUpd.Target < 105) clsUpd.IdCalificativ = 3;
                                            if (105 <= clsUpd.Target && clsUpd.Target < 130) clsUpd.IdCalificativ = 4;
                                            if (clsUpd.Target >= 130) clsUpd.IdCalificativ = 5;
                                        }
                                        //if (clsObiIndividual.Target < 80) clsObiIndividual.Calificativ = "mult sub asteptari";
                                        //if (80 <= clsObiIndividual.Target && clsObiIndividual.Target < 95) clsObiIndividual.Calificativ = "sub asteptari";
                                        //if (95 <= clsObiIndividual.Target && clsObiIndividual.Target < 105) clsObiIndividual.Calificativ = "in linie cu asteptarile";
                                        //if (105 <= clsObiIndividual.Target && clsObiIndividual.Target < 130) clsObiIndividual.Calificativ = "peste asteptari";
                                        //if (clsObiIndividual.Target >= 130) clsObiIndividual.Calificativ = "mult peste asteptari";
                                    }
                                }
                                catch (Exception) { }
                            }
                            break;
                        case "24":                          //Cristim
                            {
                                clsUpd.Target = (int)(Convert.ToDouble(General.Nz(clsUpd.Descriere, "0")) * Convert.ToDouble(General.Nz(clsUpd.Pondere, 0)) / Convert.ToDouble(General.Nz(clsUpd.Realizat, "0")));
                                int suma = 0;
                                foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))
                                    suma += Convert.ToInt32(General.Nz(linie.Target, 0));

                                Eval_QuizIntrebari txtCalifCantit = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CANTITATIVA") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (txtCalifCantit != null)
                                {
                                    Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifCantit.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                    if (val != null)
                                        val.SetValue(linieCalif, suma.ToString(), null);
                                }

                                double califTotal = suma;

                                Eval_QuizIntrebari txtCalifCalit = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CALITATIVA") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (txtCalifCalit != null)
                                {
                                    Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifCalit.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                    if (val != null)
                                    {
                                        string s = val.GetValue(linieCalif, null).ToString();
                                        if (s.Length > 0)
                                        {
                                            double rez = 0;
                                            double.TryParse(s, out rez);
                                            califTotal += rez;
                                        }
                                    }
                                }

                                Eval_QuizIntrebari txtCalifFinal = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV FINAL EVALUARE PERFORMANTA") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (txtCalifFinal != null)
                                {
                                    Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifFinal.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieCalif, califTotal.ToString(), null);
                                }
                                Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;


                            }
                            break;
                        case "25":                          //Franke
                            {
                                clsUpd.Calificativ = (10 * Convert.ToDouble(General.Nz(clsUpd.Realizat, 0)) / Convert.ToDouble(General.Nz(clsUpd.Target, 0))).ToString();
                                double total = 0;
                                int cnt = 0;
                                foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))
                                {
                                    total += Convert.ToDouble(General.Nz(linie.Calificativ, "0"));
                                    cnt++;
                                }
                                double nota = (total / cnt) * 0.4;

                                Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Id == clsUpd.IdLinieQuiz + 1 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinala != null)
                                {
                                    Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNota, nota.ToString("0.##"), null);
                                }

                                double notaF = 0;
                                List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA OBIECTIV") && p.IdQuiz == clsUpd.IdQuiz).ToList();
                                if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                {
                                    foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            string s = val.GetValue(linieCalif, null).ToString();
                                            if (s.Length > 0)
                                            {
                                                double rez = 0;
                                                double.TryParse(s, out rez);
                                                notaF += rez;
                                            }
                                        }
                                    }
                                }

                                Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA EVALUARE") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinalaEvaluare != null)
                                {
                                    Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNotaFinala, notaF.ToString("0.##"), null);
                                }

                                Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                            }
                            break;
                        case "21":      //CLAIM                          
                            if (clsUpd.IdQuiz != 8 && clsUpd.IdQuiz != 11 && clsUpd.IdQuiz != 24)
                            {
                                //foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                //                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))
                                //sumaClaim += Convert.ToInt32(General.Nz(clsUpd.Calificativ, 0)) * Convert.ToInt32(General.Nz(clsUpd.Pondere, 0));
                                //Radu 07.05.2020
                                decimal val = decimal.Parse(General.Nz(clsUpd.Calificativ, 0).ToString(), CultureInfo.InvariantCulture);
                                sumaClaim += val * (decimal)clsUpd.Pondere;
                            }                            
                            break;
                        case "27":                          //Euroins
                            {
                                double total = 0;
                                int cnt = 0;
                                foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))
                                {
                                    total += Convert.ToDouble(General.Nz(linie.IdCalificativ, 0));
                                    cnt++;
                                }
                                double nota = total / cnt;
                                double calif = nota;

                                int nrSec = Convert.ToInt32(Session["indexSec"].ToString());

                                string numeTxt = "";
                                switch (nrSec)
                                {
                                    case 1:
                                        numeTxt = "TOTAL INTERMEDIAR 1";
                                        break;
                                    case 2:
                                        numeTxt = "TOTAL INTERMEDIAR 2";
                                        break;
                                    case 3:
                                        numeTxt = "TOTAL INTERMEDIAR 3C";
                                        break;
                                }

                                Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains(numeTxt) && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinala != null)
                                {
                                    Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNota, calif.ToString("0.##"), null);
                                }

                                double notaF = 0;
                                int nr = 5;
                                List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("TOTAL INTERMEDIAR") && p.IdQuiz == clsUpd.IdQuiz).ToList();
                                if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                {
                                    foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            string s = val.GetValue(linieCalif, null).ToString();
                                            if (s.Length > 0)
                                            {
                                                double rez = 0;
                                                double.TryParse(s, out rez);
                                                notaF += rez;
                                            }
                                            if (linie.Descriere.ToUpper() == "TOTAL INTERMEDIAR 3B" && s.Length <= 0)
                                                nr = 4;
                                        }
                                    }
                                }

                                notaF /= nr;

                                double califF = notaF;

                                Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinalaEvaluare != null)
                                {
                                    Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNotaFinala, califF.ToString("0.##"), null);
                                }

                                Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                            }
                            break;
                    }
                }

                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 21)
                {
                    Eval_QuizIntrebari txtSumaOb = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTE FOR INDIVIDUAL OBJECTIVES") && p.IdQuiz == idQuiz).FirstOrDefault();
                    if (txtSumaOb != null)
                    {
                        Eval_RaspunsLinii linieSumaOb = lstEval_RaspunsLinii.Where(p => p.Id == txtSumaOb.Id && p.F10003 == marca && p.IdQuiz == idQuiz).FirstOrDefault();
                        PropertyInfo val = linieSumaOb.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)
                            val.SetValue(linieSumaOb, sumaClaim.ToString(), null);
                    }

                    decimal notaFinala = sumaClaim;
                    Eval_QuizIntrebari txtNotaFinala = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("FINAL NOTE") && p.IdQuiz == idQuiz).FirstOrDefault();
                    if (txtNotaFinala != null)
                    {
                        Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == txtNotaFinala.Id && p.F10003 == marca && p.IdQuiz == idQuiz).FirstOrDefault();
                        PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)
                        {
                            string s = val.GetValue(linieNotaFinala, null).ToString();
                            if (s.Length > 0)
                            {
                                decimal rez = 0;
                                decimal.TryParse(s, out rez);
                                notaFinala += rez;
                            }
                            val.SetValue(linieNotaFinala, notaFinala.ToString(), null);
                        }
                    }
                }

                for (int x = 0; x < e.DeleteValues.Count; x++)
                {
                    ASPxDataDeleteValues del = e.DeleteValues[x] as ASPxDataDeleteValues;
                    object[] keys = new object[] { del.Keys[0] };

                    Eval_ObiIndividualeTemp clsDel = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                    if (clsDel == null) return;
                    lst.Remove(clsDel);

                    List<Eval_ObiIndividualeTemp> lstSterse = Session["lstEval_ObiIndividualeTemp_Sterse"] as List<Eval_ObiIndividualeTemp>;
                    if (lstSterse == null) lstSterse = new List<Eval_ObiIndividualeTemp>();
                    lstSterse.Add(clsDel);
                    Session["lstEval_ObiIndividualeTemp_Sterse"] = lstSterse;
                }

                Session["lstEval_ObiIndividualeTemp"] = lst;

                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Florin 2019.06.28
        private void GrDateCompetente_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grid = sender as ASPxGridView;
                grid.CancelEdit();
                GridViewDataComboBoxColumn colCompetenta = (grid.Columns["IdCompetenta"] as GridViewDataComboBoxColumn);
                //Florin 2020.04.29
                GridViewDataComboBoxColumn colCalificativ = (grid.Columns["IdCalificativ"] as GridViewDataComboBoxColumn);

                List<Eval_CompetenteAngajatTemp> lst = Session["lstEval_CompetenteAngajatTemp"] as List<Eval_CompetenteAngajatTemp>;

                for (int x = 0; x < e.InsertValues.Count; x++)
                {
                    ASPxDataInsertValues ins = e.InsertValues[x] as ASPxDataInsertValues;
                    Eval_CompetenteAngajatTemp clsNew = new Eval_CompetenteAngajatTemp();
                    clsNew.IdAuto = Convert.ToInt32(General.Nz(lst.Max(p => (int?)p.IdAuto), 0)) + 1;
                    clsNew.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                    clsNew.Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]); ;
                    clsNew.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                    clsNew.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);

                    clsNew.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    clsNew.TIME = DateTime.Now;

                    //Florin 2020.01.27
                    ////Radu 24.10.2019
                    //clsNew.IdPeriod = lstEval_QuizIntrebari.Where(p => p.Id == clsNew.IdLinieQuiz).FirstOrDefault().IdPeriodComp;
                    clsNew.IdPeriod = idPerioada;

                    foreach (DictionaryEntry de in ins.NewValues)
                    {
                        switch (de.Key.ToString())
                        {
                            case "IdCompetenta":
                                clsNew.IdCompetenta = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCompetenta != null)
                                    clsNew.Competenta = colCompetenta.PropertiesComboBox.Items.FindByValue(clsNew.IdCompetenta).Text;
                                break;
                            case "Competenta":
                                if (colCompetenta == null)
                                    clsNew.Competenta = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Pondere":
                                clsNew.Pondere = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "IdCalificativ":
                                clsNew.IdCalificativ = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCalificativ != null && colCalificativ.PropertiesComboBox.Items.FindByValue(clsNew.IdCalificativ) != null)
                                    clsNew.Calificativ = colCalificativ.PropertiesComboBox.Items.FindByValue(clsNew.IdCalificativ).Text;
                                break;
                            case "Calificativ":
                                clsNew.Calificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ExplicatiiCalificativ":
                                clsNew.ExplicatiiCalificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Explicatii":
                                clsNew.Explicatii = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                        }
                    }

                    if (grid.Columns["FormulaSql1"] != null)
                        clsNew.FormulaSql1 = ((GridViewDataTextColumn)grid.Columns["FormulaSql1"]).UnboundExpression;
                    if (grid.Columns["FormulaSql2"] != null)
                        clsNew.FormulaSql2 = ((GridViewDataTextColumn)grid.Columns["FormulaSql2"]).UnboundExpression;

                    lst.Add(clsNew);
                }

                decimal sumaClaim = 0;
                int marca = -99;
                int idQuiz = -99;
                string categComp = "";
                for (int x = 0; x < e.UpdateValues.Count; x++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[x] as ASPxDataUpdateValues;
                    object[] keys = new object[] { upd.Keys[0] };

                    ASPxDataUpdateValues ins = e.UpdateValues[x] as ASPxDataUpdateValues;
                    Eval_CompetenteAngajatTemp clsUpd = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                    if (clsUpd == null) return;

                    clsUpd.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                    clsUpd.TIME = DateTime.Now;

                    marca = clsUpd.F10003;
                    idQuiz = clsUpd.IdQuiz;
                    categComp = clsUpd.CategCompetenta;

                    foreach (DictionaryEntry de in ins.NewValues)
                    {
                        switch (de.Key.ToString())
                        {
                            case "IdCompetenta":
                                clsUpd.IdCompetenta = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCompetenta != null)
                                    clsUpd.Competenta = colCompetenta.PropertiesComboBox.Items.FindByValue(clsUpd.IdCompetenta).Text;
                                break;
                            case "Competenta":
                                if (clsUpd == null)
                                    clsUpd.Competenta = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Pondere":
                                clsUpd.Pondere = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToDecimal(ins.NewValues[de.Key.ToString()]);
                                break;
                            case "IdCalificativ":
                                clsUpd.IdCalificativ = ins.NewValues[de.Key.ToString()] == null ? -99 : Convert.ToInt32(ins.NewValues[de.Key.ToString()]);
                                if (colCalificativ != null && colCalificativ.PropertiesComboBox.Items.FindByValue(clsUpd.IdCalificativ) != null)
                                    clsUpd.Calificativ = colCalificativ.PropertiesComboBox.Items.FindByValue(clsUpd.IdCalificativ).Text;
                                break;
                            case "Calificativ":
                                clsUpd.Calificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "ExplicatiiCalificativ":
                                clsUpd.ExplicatiiCalificativ = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                            case "Explicatii":
                                clsUpd.Explicatii = ins.NewValues[de.Key.ToString()] == null ? "" : ins.NewValues[de.Key.ToString()].ToString().Replace("'", "");
                                break;
                        }
                    }

                    if (grid.Columns["FormulaSql1"] != null)
                        clsUpd.FormulaSql1 = ((GridViewDataTextColumn)grid.Columns["FormulaSql1"]).UnboundExpression;
                    if (grid.Columns["FormulaSql2"] != null)
                        clsUpd.FormulaSql2 = ((GridViewDataTextColumn)grid.Columns["FormulaSql2"]).UnboundExpression;

                    switch (General.Nz(Session["IdClient"], 1).ToString())
                    {
                        case "25":                          //Franke
                            {
                                int suma = 0, cnt = 0;
                                double medie = 0;

                                lstEval_CompSetCalificativDet = Session["feedEval_CompCalificativ"] as List<Eval_SetCalificativDet>;

                                foreach (Eval_CompetenteAngajatTemp linie in lst)
                                {
                                    Eval_SetCalificativDet nota = lstEval_CompSetCalificativDet.Where(p => p.IdCalificativ == linie.IdCalificativ).FirstOrDefault();
                                    suma += (nota == null ? 0 : nota.Nota);
                                    cnt++;
                                }
                                medie = (Convert.ToDouble(suma) / Convert.ToDouble(cnt)) * 0.2;


                                Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Id == clsUpd.IdLinieQuiz + 1 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinala != null)
                                {
                                    Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNota, medie.ToString("0.##"), null);
                                }

                                double notaF = 0;
                                List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA OBIECTIV") && p.IdQuiz == clsUpd.IdQuiz).ToList();
                                if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                {
                                    foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            string s = val.GetValue(linieCalif, null).ToString();
                                            if (s.Length > 0)
                                            {
                                                double rez = 0;
                                                double.TryParse(s, out rez);
                                                notaF += rez;
                                            }
                                        }
                                    }
                                }

                                Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA EVALUARE") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinalaEvaluare != null)
                                {
                                    Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNotaFinala, notaF.ToString("0.##"), null);
                                }

                                Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                            }
                            break;
                        case "21":  //CLAIM
                            if (clsUpd.IdQuiz != 8 && clsUpd.IdQuiz != 11 && clsUpd.IdQuiz != 24)
                            {
                                //foreach (Eval_CompetenteAngajatTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                //                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))

                                //Florin 2020.05.06
                                decimal val = decimal.Parse(General.Nz(clsUpd.Calificativ, 0).ToString(), CultureInfo.InvariantCulture);
                                sumaClaim += val * clsUpd.Pondere;

                            }
                            break;
                        case "27":                          //Euroins
                            {
                                double total = 0;
                                int cnt = 0;
                                foreach (Eval_CompetenteAngajatTemp linie in lst.Where(p => p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz
                                                                                    && p.IdLinieQuiz == clsUpd.IdLinieQuiz && p.Pozitie == clsUpd.Pozitie))
                                {
                                    total += Convert.ToDouble(General.Nz(linie.IdCalificativ, 0));
                                    cnt++;
                                }
                                double nota = total / cnt;
                                double calif = nota;


                                Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("TOTAL INTERMEDIAR 3B") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinala != null)
                                {
                                    Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNota, calif.ToString("0.##"), null);
                                }

                                double notaF = 0;
                                int nr = 5;
                                List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("TOTAL INTERMEDIAR") && p.IdQuiz == clsUpd.IdQuiz).ToList();
                                if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                                {
                                    foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                                    {
                                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                                        if (val != null)
                                        {
                                            string s = val.GetValue(linieCalif, null).ToString();
                                            if (s.Length > 0)
                                            {
                                                double rez = 0;
                                                double.TryParse(s, out rez);
                                                notaF += rez;
                                            }
                                            if (linie.Descriere.ToUpper() == "TOTAL INTERMEDIAR 3B" && s.Length <= 0)
                                                nr = 4;
                                        }
                                    }
                                }

                                notaF /= nr;

                                double califF = notaF;

                                Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA") && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                if (notaFinalaEvaluare != null)
                                {
                                    Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsUpd.F10003 && p.IdQuiz == clsUpd.IdQuiz).FirstOrDefault();
                                    PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                                    if (val != null)
                                        val.SetValue(linieNotaFinala, califF.ToString("0.##"), null);
                                }

                                Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;

                            }
                            break;
                    }
                }

                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 21)
                {
                    string tipComp = "";
                    if (categComp.ToUpper().Contains("PROFESSIONAL") || categComp.ToUpper().Contains("PROFESIONALE"))
                        tipComp = "NOTE FOR PROFESSIONAL COMPETENCES";
                    if (categComp.ToUpper().Contains("PERSONAL"))
                        tipComp = "NOTE FOR PERSONAL COMPETENCES";

                    Eval_QuizIntrebari txtSumaComp = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains(tipComp) && p.IdQuiz == idQuiz).FirstOrDefault();
                    if (txtSumaComp != null)
                    {
                        Eval_RaspunsLinii linieSumaComp = lstEval_RaspunsLinii.Where(p => p.Id == txtSumaComp.Id && p.F10003 == marca && p.IdQuiz == idQuiz).FirstOrDefault();
                        PropertyInfo val = linieSumaComp.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)
                            val.SetValue(linieSumaComp, sumaClaim.ToString(), null);
                    }


                    decimal notaFinala = sumaClaim;
                    Eval_QuizIntrebari txtNotaFinala = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("FINAL NOTE") && p.IdQuiz == idQuiz).FirstOrDefault();
                    if (txtNotaFinala != null)
                    {
                        Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == txtNotaFinala.Id && p.F10003 == marca && p.IdQuiz == idQuiz).FirstOrDefault();
                        PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)
                        {
                            string s = val.GetValue(linieNotaFinala, null).ToString();
                            if (s.Length > 0)
                            {
                                decimal rez = 0;
                                decimal.TryParse(s, out rez);
                                notaFinala += rez;
                            }
                            val.SetValue(linieNotaFinala, notaFinala.ToString(), null);
                        }
                    }
                }

                for (int x = 0; x < e.DeleteValues.Count; x++)
                {
                    ASPxDataDeleteValues del = e.DeleteValues[x] as ASPxDataDeleteValues;
                    object[] keys = new object[] { del.Keys[0] };

                    Eval_CompetenteAngajatTemp clsDel = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                    if (clsDel == null) return;
                    lst.Remove(clsDel);

                    List<Eval_CompetenteAngajatTemp> lstSterse = Session["lstEval_CompetenteAngajatTemp_Sterse"] as List<Eval_CompetenteAngajatTemp>;
                    if (lstSterse == null) lstSterse = new List<Eval_CompetenteAngajatTemp>();
                    lstSterse.Add(clsDel);
                    Session["lstEval_CompetenteAngajatTemp_Sterse"] = lstSterse;
                }

                Session["lstEval_CompetenteAngajatTemp"] = lst;

                e.Handled = true;
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

                lstEval_RaspunsLinii = Session["lstEval_RaspunsLinii"] as List<Eval_RaspunsLinii>;

                GridViewDataComboBoxColumn colObiectiv = (grid.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);
                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                Eval_ObiIndividualeTemp clsObiIndividual = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                if (e.NewValues.Contains("IdObiectiv"))
                {
                    clsObiIndividual.IdObiectiv = e.NewValues["IdObiectiv"] == null ? -99 : Convert.ToInt32(e.NewValues["IdObiectiv"]);
                }

                if (e.NewValues.Contains("Obiectiv"))
                {
                    clsObiIndividual.Obiectiv = ((colObiectiv != null && colObiectiv.PropertiesComboBox.Items.Count > 0) ? colObiectiv.PropertiesComboBox.Items.FindByValue(clsObiIndividual.IdObiectiv).Text : (e.NewValues["Obiectiv"] ?? "").ToString()).Replace("'", "");
                }

                if (e.NewValues.Contains("IdActivitate"))
                    clsObiIndividual.IdActivitate = e.NewValues["IdActivitate"] == null ? -99 : Convert.ToInt32(e.NewValues["IdActivitate"]);

                if (Session["feedEval_ObiectivActivitate"] == null)
                {
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

                //int pozTab = Convert.ToInt32(Session["Eval_ActiveTab"]);
                //if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) == 1 && pozTab == 2 && e.NewValues.Contains("ExplicatiiCalificativ"))
                //{//evaluatorul poate completa o coloana care sa fie vizibila si pe tab-ul angajat
                //    Eval_ObiIndividualeTemp clsObiIndividualAngajat = lst.Where(p => p.IdQuiz == clsObiIndividual.IdQuiz && p.F10003 == clsObiIndividual.F10003 && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz
                //                                                                    && p.IdObiectiv == clsObiIndividual.IdObiectiv && p.IdActivitate == clsObiIndividual.IdActivitate && p.Pozitie == 1).FirstOrDefault();
                //    clsObiIndividualAngajat.ExplicatiiCalificativ = e.NewValues["ExplicatiiCalificativ"] == null ? "" : e.NewValues["ExplicatiiCalificativ"].ToString().Replace("'", "");
                //}

                //if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) == 2 && pozTab == 2 && e.NewValues.Contains("IdCalificativ"))
                //{
                //    Eval_ObiIndividualeTemp clsObiIndividualAngajat = lst.Where(p => p.IdQuiz == clsObiIndividual.IdQuiz && p.F10003 == clsObiIndividual.F10003 && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz
                //                                                                    && p.IdObiectiv == clsObiIndividual.IdObiectiv && p.IdActivitate == clsObiIndividual.IdActivitate && p.Pozitie == 1).FirstOrDefault();
                //    clsObiIndividualAngajat.IdCalificativ = e.NewValues["IdCalificativ"] == null ? 0 : Convert.ToInt32(e.NewValues["IdCalificativ"].ToString());
                //}

                int poz = Convert.ToInt32(Session["Eval_ActiveTab"]);
                clsObiIndividual.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                clsObiIndividual.Pozitie = poz;
                clsObiIndividual.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));

                if (e.NewValues.Contains("ColoanaSuplimentara1"))
                    clsObiIndividual.ColoanaSuplimentara1 = e.NewValues["ColoanaSuplimentara1"] == null ? "" : e.NewValues["ColoanaSuplimentara1"].ToString().Replace("'", "");
                if (e.NewValues.Contains("ColoanaSuplimentara2"))
                    clsObiIndividual.ColoanaSuplimentara2 = e.NewValues["ColoanaSuplimentara2"] == null ? "" : e.NewValues["ColoanaSuplimentara2"].ToString().Replace("'", "");
                if (e.NewValues.Contains("ColoanaSuplimentara3"))
                    clsObiIndividual.ColoanaSuplimentara3 = e.NewValues["ColoanaSuplimentara3"] == null ? "" : e.NewValues["ColoanaSuplimentara3"].ToString().Replace("'", "");
                if (e.NewValues.Contains("ColoanaSuplimentara4"))
                    clsObiIndividual.ColoanaSuplimentara4 = e.NewValues["ColoanaSuplimentara4"] == null ? "" : e.NewValues["ColoanaSuplimentara4"].ToString().Replace("'", "");

                clsObiIndividual.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"], -99));
                clsObiIndividual.TIME = DateTime.Now;


                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20)
                {
                    try
                    {
                        if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) >= 2)   //se doreste ca rating-ul sa fie afisat numai dupa ce angajatul finalizeaza
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

                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 24)
                {//Cristim
                     //clsObiIndividual.Realizat = (int)(Convert.ToDouble(General.Nz(clsObiIndividual.Descriere, "0")) * Convert.ToDouble(General.Nz(clsObiIndividual.Target, 0)) / Convert.ToDouble(General.Nz(clsObiIndividual.Pondere, 0)));
                     //int suma = 0;
                     //foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz 
                     //                                                    && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz && p.Pozitie == clsObiIndividual.Pozitie))
                     //    suma += Convert.ToInt32(General.Nz(linie.Realizat, 0));

                    clsObiIndividual.Target = (int)(Convert.ToDouble(General.Nz(clsObiIndividual.Descriere, "0")) * Convert.ToDouble(General.Nz(clsObiIndividual.Pondere, 0)) / Convert.ToDouble(General.Nz(clsObiIndividual.Realizat, "0")));
                    int suma = 0;
                    foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz
                                                                        && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz && p.Pozitie == clsObiIndividual.Pozitie))
                        suma += Convert.ToInt32(General.Nz(linie.Target, 0));

                    Eval_QuizIntrebari txtCalifCantit = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CANTITATIVA") && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                    if (txtCalifCantit != null)
                    {
                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifCantit.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)                        
                            val.SetValue(linieCalif, suma.ToString(), null);                
                    }

                    double califTotal = suma;

                    Eval_QuizIntrebari txtCalifCalit = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV EVALUARE CALITATIVA") && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                    if (txtCalifCalit != null)
                    {
                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifCalit.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                        if (val != null)
                        {
                            string s = val.GetValue(linieCalif, null).ToString();
                            if (s.Length > 0)
                            {
                                double rez = 0;
                                double.TryParse(s, out rez);
                                califTotal += rez;
                            }
                        }
                    }

                    Eval_QuizIntrebari txtCalifFinal = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("CALIFICATIV FINAL EVALUARE PERFORMANTA") && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                    if (txtCalifFinal != null)
                    {
                        Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == txtCalifFinal.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                        if (val != null)
                            val.SetValue(linieCalif, califTotal.ToString(), null);
                    }
                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;                   

                }

                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 25)
                {//Franke
                    clsObiIndividual.Calificativ = (10 * Convert.ToDouble(General.Nz(clsObiIndividual.Realizat, 0)) / Convert.ToDouble(General.Nz(clsObiIndividual.Target, 0))).ToString();
                    double total = 0;
                    int cnt = 0;
                    foreach (Eval_ObiIndividualeTemp linie in lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz
                                                                        && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz && p.Pozitie == clsObiIndividual.Pozitie))
                    {
                        total += Convert.ToDouble(General.Nz(linie.Calificativ, "0"));
                        cnt++;
                    }
                    double nota = (total / cnt) * 0.4;

                    Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Id == clsObiIndividual.IdLinieQuiz + 1 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                    if (notaFinala != null)
                    {
                        Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                        if (val != null)
                            val.SetValue(linieNota, nota.ToString("0.##"), null);
                    }

                    double notaF = 0;
                    List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA OBIECTIV") && p.IdQuiz == clsObiIndividual.IdQuiz).ToList();
                    if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                    {
                        foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                        {
                            Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                            PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                            if (val != null)
                            {
                                string s = val.GetValue(linieCalif, null).ToString();
                                if (s.Length > 0)
                                {
                                    double rez = 0;
                                    double.TryParse(s, out rez);
                                    notaF += rez;
                                }
                            }
                        }
                    }

                    Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA EVALUARE") && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                    if (notaFinalaEvaluare != null)
                    {
                        Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                        if (val != null)
                            val.SetValue(linieNotaFinala, notaF.ToString("0.##"), null);
                    }

                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;
                }                


                e.Cancel = true;

                grid.CancelEdit();
                Session["lstEval_ObiIndividualeTemp"] = lst;

                //if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && poz == 1 && pozTab == 2)
                //    grid.DataSource = lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.Pozitie == 2 && p.IdQuiz == clsObiIndividual.IdQuiz && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz).ToList();
                //else
                //{
                    grid.DataSource = lst.Where(p => p.F10003 == clsObiIndividual.F10003 && p.IdQuiz == clsObiIndividual.IdQuiz && p.Pozitie == poz && p.IdLinieQuiz == clsObiIndividual.IdLinieQuiz).ToList();
                //}              
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

                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;
                Eval_ObiIndividualeTemp clsNew = new Eval_ObiIndividualeTemp();
                clsNew.IdAuto = -1 * (Convert.ToInt32(General.Nz(lst.Max(p => (int?)Math.Abs(p.IdAuto)),0)) + 1);
                clsNew.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                int poz = Convert.ToInt32(Session["Eval_ActiveTab"]);

                clsNew.Pozitie = poz;
                clsNew.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                clsNew.IdObiectiv = e.NewValues["IdObiectiv"] == null ? -99 : Convert.ToInt32(e.NewValues["IdObiectiv"]);
                clsNew.Obiectiv = ((colObiectiv != null && colObiectiv.PropertiesComboBox.Items.Count > 0) ? colObiectiv.PropertiesComboBox.Items.FindByValue(clsNew.IdObiectiv).Text : (e.NewValues["Obiectiv"] ?? "").ToString()).Replace("'", "");
                clsNew.IdActivitate = e.NewValues["IdActivitate"] == null ? -99 : Convert.ToInt32(e.NewValues["IdActivitate"]);

                if (Session["feedEval_ObiectivActivitate"] == null)
                {
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
                clsNew.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                clsNew.ColoanaSuplimentara1 = e.NewValues["ColoanaSuplimentara1"] == null ? "" : e.NewValues["ColoanaSuplimentara1"].ToString().Replace("'", "");
                clsNew.ColoanaSuplimentara2 = e.NewValues["ColoanaSuplimentara2"] == null ? "" : e.NewValues["ColoanaSuplimentara2"].ToString().Replace("'", "");
                clsNew.ColoanaSuplimentara3 = e.NewValues["ColoanaSuplimentara3"] == null ? "" : e.NewValues["ColoanaSuplimentara3"].ToString().Replace("'", "");
                clsNew.ColoanaSuplimentara4 = e.NewValues["ColoanaSuplimentara4"] == null ? "" : e.NewValues["ColoanaSuplimentara4"].ToString().Replace("'", "");

                clsNew.USER_NO = Convert.ToInt32(General.Nz(Session["UserId"],-99));
                clsNew.TIME = DateTime.Now;

                lst.Add(clsNew);
                Session["lstEval_ObiIndividualeTemp"] = lst;

                e.Cancel = true;

                grid.CancelEdit();

                grid.DataSource = lst.Where(p => p.F10003 == clsNew.F10003 && p.IdQuiz == clsNew.IdQuiz && p.Pozitie == poz && p.IdLinieQuiz == clsNew.IdLinieQuiz).ToList();
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
                int Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                ASPxGridView grid = sender as ASPxGridView;

                List<Eval_ObiIndividualeTemp> lst = Session["lstEval_ObiIndividualeTemp"] as List<Eval_ObiIndividualeTemp>;

                Eval_ObiIndividualeTemp clsObiIndividual = lst.Where(p => p.IdAuto == Convert.ToInt32(keys[0])).FirstOrDefault();
                lst.Remove(clsObiIndividual);
                Session["lstEval_ObiIndividualeTemp"] = lst;
                e.Cancel = true;
                grid.DataSource = lst.Where(p => p.IdQuiz == Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) && p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.IdLinieQuiz == IdLinieQuiz && p.Pozitie == Pozitie).ToList();
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
            Session["CompletareChestionarCompetente_LinieQuiz"] = id;
            ASPxGridView grDateCompetente = new ASPxGridView();
            if (Session["lstEval_CompetenteAngajatTemp"] == null)
            {
                DataTable dtCompetenteAngajat = General.IncarcaDT(@"select * from ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz""=@1 AND F10003=@2 ORDER BY ""Competenta"" ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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

                //Florin 2019.06.26
                grDateCompetente.ClientInstanceName = "grDateCompetente" + "_WXY_" + id.ToString();
                grDateCompetente.ClientIDMode = ClientIDMode.Static;
                grDateCompetente.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;

                grDateCompetente.SettingsBehavior.AllowFocusedRow = true;
                grDateCompetente.SettingsBehavior.EnableCustomizationWindow = true;
                grDateCompetente.SettingsBehavior.AllowSelectByRowClick = true;
                grDateCompetente.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn;
                grDateCompetente.Settings.ShowGroupPanel = false;
                grDateCompetente.Settings.HorizontalScrollBarMode = ScrollBarMode.Auto;
                grDateCompetente.SettingsSearchPanel.Visible = false;
                grDateCompetente.AutoGenerateColumns = false;

                grDateCompetente.ClientSideEvents.ContextMenu = "ctx";
                grDateCompetente.SettingsEditing.Mode = GridViewEditingMode.Inline;

                grDateCompetente.SettingsBehavior.ConfirmDelete = true;
                grDateCompetente.SettingsText.ConfirmDelete = "Confirmati operatia de stergere?";
              

                //grDateCompetente.RowDeleting += GrDateCompetente_RowDeleting;
                //grDateCompetente.AutoFilterCellEditorInitialize += GrDateCompetente_AutoFilterCellEditorInitialize;
                //grDateCompetente.RowInserting += GrDateCompetente_RowInserting;
                //grDateCompetente.RowUpdating += GrDateCompetente_RowUpdating;
                grDateCompetente.InitNewRow += GrDateCompetente_InitNewRow;

                grDateCompetente.SettingsEditing.Mode = GridViewEditingMode.Batch;
                grDateCompetente.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
                grDateCompetente.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;
                grDateCompetente.SettingsEditing.BatchEditSettings.ShowConfirmOnLosingChanges = false;

                //Florin 2019.06.26
                grDateCompetente.BatchUpdate += GrDateCompetente_BatchUpdate;
                Session["NumeGriduri"] += ";" + grDateCompetente.ID;

                grDateCompetente.CustomSummaryCalculate += grDate_CustomSummaryCalculate;

                grDateCompetente.EnableRowsCache = false;



                //Radu 13.05.2019
                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 25)
                {
                    string endCallBackFunctionGrDate = @"function " + grDateCompetente.ID + @"_EndCallBack(s, e) { pnlSectiune.PerformCallback('CreeazaSectiune');  }";
                    grDateCompetente.ClientSideEvents.EndCallback = endCallBackFunctionGrDate;
                }
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

                string formulaSql1 = "";
                string formulaSql2 = "";
                foreach (Eval_ConfigCompTemplateDetail clsConfigDetail in lstEval_ConfigCompTemplateDetail.Where(p=>p.TemplateId==TemplateIdCompetenta).OrderBy(p => p.Ordine ?? 99))
                {
                    //Florin 2020.09.14 Begin
                    if ((clsConfigDetail.ColumnName == "Total1" || clsConfigDetail.ColumnName == "Total2") && clsConfigDetail.FormulaSql != "")
                    {
                        GridViewDataTextColumn colFormula = new GridViewDataTextColumn();
                        string colName = clsConfigDetail.ColumnName.Replace("Total", "FormulaSql");
                        colFormula.FieldName = colName;
                        colFormula.Name = colName;
                        colFormula.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
                        colFormula.UnboundExpression = clsConfigDetail.FormulaSql;
                        colFormula.Visible = false;

                        grDateCompetente.Columns.Add(colFormula);

                        if (clsConfigDetail.ColumnName == "Total1") formulaSql1 = clsConfigDetail.FormulaSql;
                        if (clsConfigDetail.ColumnName == "Total2") formulaSql2 = clsConfigDetail.FormulaSql;
                    }
                    //Florin 2020.09.14 End

                    //Florin 2020.12.09 Begin
                    if (Convert.ToString(clsConfigDetail.TotalColoana) != "")
                    {
                        grDateCompetente.Settings.ShowFooter = true;
                        grDateCompetente.Settings.ShowStatusBar = GridViewStatusBarMode.Hidden;
                        ASPxSummaryItem s = new ASPxSummaryItem();
                        string camp = clsConfigDetail.ColumnName;
                        switch (clsConfigDetail.ColumnName)
                        {
                            case "Competenta":
                                camp = "IdCompetenta";
                                break;
                            case "Calificativ":
                                camp = "IdCalificativ";
                                break;
                        }
                        s.FieldName = camp;
                        s.ShowInColumn = camp;
                        switch (clsConfigDetail.TotalColoana)
                        {
                            case 1:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Suma {0:N0}";
                                break;
                            case 2:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Suma {0:N2}";
                                break;
                            case 3:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Media {0:N0}";
                                break;
                            case 4:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Media {0:N2}";
                                break;
                            case 5:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Val. min. {0}";
                                break;
                            case 6:
                                s.SummaryType = DevExpress.Data.SummaryItemType.Custom;
                                s.DisplayFormat = "Val. max. {0}";
                                break;
                        }
                        grDateCompetente.TotalSummary.Add(s);
                    }
                    //Florin 2020.12.09 End



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

                                if (Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1)
                                    strSQLCompetenta = @"select categDet.""IdCompetenta"" as ""Id"", categDet.""DenCompetenta"" as ""Denumire""
                                                            from ""Eval_CategCompetente"" categ
                                                            join ""Eval_CategCompetenteDet"" categDet on categ.""IdCategorie"" = categDet.""IdCategorie""";

                                DataTable dtCompetenta = General.IncarcaDT(strSQLCompetenta, new object[] { clsConfigDetail.IdNomenclator, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                            colCompetenta.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                            colCompetenta.Width = clsConfigDetail.Width;
                            colCompetenta.PropertiesComboBox.TextField = "Denumire";
                            colCompetenta.PropertiesComboBox.ValueField = "Id";
                            colCompetenta.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDownList;
                            colCompetenta.PropertiesComboBox.ValidationSettings.RequiredField.IsRequired = true;
                            colCompetenta.PropertiesComboBox.ValidationSettings.Display = Display.None;
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
                            colCalificativ.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                            colCalificativ.Width = clsConfigDetail.Width;

                            colCalificativ.PropertiesComboBox.TextField = "Denumire";
                            colCalificativ.PropertiesComboBox.ValueField = "IdCalificativ";
                            colCalificativ.PropertiesComboBox.DropDownStyle = DropDownStyle.DropDownList;
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
                                colString.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colString.Width = clsConfigDetail.Width;

                                grDateCompetente.Columns.Add(colString);
                                break;
                            case "System.Decimal":
                                GridViewDataTextColumn colDecimal = new GridViewDataTextColumn();
                                colDecimal.FieldName = clsConfigDetail.ColumnName;
                                colDecimal.Name = clsConfigDetail.ColumnName;
                                colDecimal.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colDecimal.Width = clsConfigDetail.Width;

                                colDecimal.PropertiesTextEdit.DisplayFormatString = "n3";
                                colDecimal.PropertiesTextEdit.MaskSettings.Mask = "<0..99999>.<0..999g>";

                                grDateCompetente.Columns.Add(colDecimal);
                                break;
                            case "System.Int32":
                                GridViewDataTextColumn colInt = new GridViewDataTextColumn();
                                colInt.FieldName = clsConfigDetail.ColumnName;
                                colInt.Name = clsConfigDetail.ColumnName;
                                colInt.Caption = clsConfigDetail.Alias ?? Dami.TraduCuvant(clsConfigDetail.ColumnName);
                                colInt.Width = clsConfigDetail.Width;

                                colInt.PropertiesTextEdit.DisplayFormatString = "n0";
                                colInt.PropertiesTextEdit.MaskSettings.Mask = "<0..99999>";

                                grDateCompetente.Columns.Add(colInt);
                                break;
                        }
                    }
                }

                #endregion

                #region Grid Command Buttons

                //grDateCompetente.SettingsCommandButton.EditButton.Image.Url = "../Fisiere/Imagini/Icoane/edit.png";
                //grDateCompetente.SettingsCommandButton.EditButton.Image.AlternateText = "Edit";
                //grDateCompetente.SettingsCommandButton.EditButton.Image.ToolTip = "Edit";

                //grDateCompetente.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                //grDateCompetente.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                //grDateCompetente.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                //grDateCompetente.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                //grDateCompetente.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                //grDateCompetente.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";


                //grDateCompetente.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                //grDateCompetente.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";

                grDateCompetente.SettingsCommandButton.UpdateButton.Image.Url = "../Fisiere/Imagini/Icoane/salveaza.png";
                grDateCompetente.SettingsCommandButton.UpdateButton.Image.AlternateText = "Save";
                grDateCompetente.SettingsCommandButton.UpdateButton.Image.ToolTip = "Actualizeaza";

                grDateCompetente.SettingsCommandButton.CancelButton.Image.Url = "../Fisiere/Imagini/Icoane/renunta.png";
                grDateCompetente.SettingsCommandButton.CancelButton.Image.AlternateText = "Renunta";
                grDateCompetente.SettingsCommandButton.CancelButton.Image.ToolTip = "Renunta";

                //if (clsEval_ConfigCompTemplate.PoateAdauga == 1)
                //{
                //    grDateCompetente.SettingsCommandButton.NewButton.Image.ToolTip = "Rand nou";
                //    grDateCompetente.SettingsCommandButton.NewButton.Image.Url = "~/Fisiere/Imagini/Icoane/New.png";
                //}

                //if (clsEval_ConfigCompTemplate.PoateSterge == 1)
                //{
                //    grDateCompetente.SettingsCommandButton.DeleteButton.Image.ToolTip = "Sterge";
                //    grDateCompetente.SettingsCommandButton.DeleteButton.Image.Url = "~/Fisiere/Imagini/Icoane/sterge.png";
                //}

                //if (clsEval_ConfigCompTemplate.PoateModifica == 1)
                //{
                //    grDateCompetente.SettingsCommandButton.EditButton.Image.ToolTip = "Modifica";
                //    grDateCompetente.SettingsCommandButton.EditButton.Image.Url = "~/Fisiere/Imagini/Icoane/edit.png";
                //}

                #endregion

                List<Eval_CompetenteAngajatTemp> lst = lstEval_CompetenteAngajatTemp.Where(p => p.F10003 == Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) && p.IdLinieQuiz == id && p.Pozitie == Convert.ToInt32(Session["Eval_ActiveTab"])).ToList();
                if (formulaSql1 != "" || formulaSql2 != "")
                {
                    foreach (Eval_CompetenteAngajatTemp l in lst)
                    {
                        if (formulaSql1 != "")
                            l.FormulaSql1 = formulaSql1;
                        if (formulaSql2 != "")
                            l.FormulaSql2 = formulaSql2;
                    }
                }
                grDateCompetente.DataSource = lst;
                grDateCompetente.KeyFieldName = "IdAuto";
                grDateCompetente.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return grDateCompetente;
        }

        //private void grDateCompetente_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        //{
        //    try
        //    {
        //        ASPxSummaryItem itm = e.Item as ASPxSummaryItem;
        //        switch (itm.DisplayFormat)
        //        {
        //            case "Suma {0:N0}":
        //            case "Suma {0:N2}":
        //                {
        //                    if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
        //                        e.TotalValue = 0;
        //                    else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        //                        e.TotalValue = Convert.ToDecimal(e.TotalValue) + Convert.ToDecimal(e.FieldValue);
        //                }
        //                break;
        //            case "Media {0:N0}":
        //            case "Media {0:N2}":
        //                {
        //                    if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
        //                        e.TotalValue = 0;
        //                    else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        //                        e.TotalValue = Convert.ToDecimal(e.TotalValue) + Convert.ToDecimal(e.FieldValue);
        //                    else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
        //                        e.TotalValue = Convert.ToDecimal(e.TotalValue) / (e.RowHandle + 1);
        //                }
        //                break;
        //            case "Val. min. {0}":
        //                {
        //                    if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
        //                        e.TotalValue = 999999;
        //                    else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        //                    {
        //                        if (Convert.ToDecimal(e.FieldValue) < Convert.ToDecimal(e.TotalValue))
        //                            e.TotalValue = Convert.ToDecimal(e.FieldValue);
        //                    }
        //                }
        //                break;
        //            case "Val. max. {0}":
        //                {
        //                    if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
        //                        e.TotalValue = 0;
        //                    else if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
        //                    {
        //                        if (Convert.ToDecimal(e.FieldValue) > Convert.ToDecimal(e.TotalValue))
        //                            e.TotalValue = Convert.ToDecimal(e.FieldValue);
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //}

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
                clsCompetenteAngajat.IdLinieQuiz = Convert.ToInt32(grid.ID.Split('_')[grid.ID.Split('_').Count() - 1]);
                clsCompetenteAngajat.Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);
                clsCompetenteAngajat.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
                clsCompetenteAngajat.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));


                if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 25)
                {//Franke

                    int suma = 0, cnt = 0;
                    double medie = 0;

                    lstEval_CompSetCalificativDet = Session["feedEval_CompCalificativ"] as List<Eval_SetCalificativDet>;

                    foreach (Eval_CompetenteAngajatTemp linie in lst)
                    {
                        Eval_SetCalificativDet nota = lstEval_CompSetCalificativDet.Where(p => p.IdCalificativ == linie.IdCalificativ).FirstOrDefault();
                        suma += (nota == null ? 0 : nota.Nota);
                        cnt++;
                    }
                    medie = (Convert.ToDouble(suma) / Convert.ToDouble(cnt)) * 0.2;
                

                    Eval_QuizIntrebari notaFinala = lstEval_QuizIntrebari.Where(p => p.Id == clsCompetenteAngajat.IdLinieQuiz + 1 && p.IdQuiz == clsCompetenteAngajat.IdQuiz).FirstOrDefault();
                    if (notaFinala != null)
                    {
                        Eval_RaspunsLinii linieNota = lstEval_RaspunsLinii.Where(p => p.Id == notaFinala.Id && p.F10003 == clsCompetenteAngajat.F10003 && p.IdQuiz == clsCompetenteAngajat.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieNota.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                        if (val != null)
                            val.SetValue(linieNota, medie.ToString("0.##"), null);
                    }

                    double notaF = 0;
                    List<Eval_QuizIntrebari> lstNoteFinale = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA OBIECTIV") && p.IdQuiz == clsCompetenteAngajat.IdQuiz).ToList();
                    if (lstNoteFinale != null && lstNoteFinale.Count > 0)
                    {
                        foreach (Eval_QuizIntrebari linie in lstNoteFinale)
                        {
                            Eval_RaspunsLinii linieCalif = lstEval_RaspunsLinii.Where(p => p.Id == linie.Id && p.F10003 == clsCompetenteAngajat.F10003 && p.IdQuiz == clsCompetenteAngajat.IdQuiz).FirstOrDefault();
                            PropertyInfo val = linieCalif.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());
                            if (val != null)
                            {
                                string s = val.GetValue(linieCalif, null).ToString();
                                if (s.Length > 0)
                                {
                                    double rez = 0;
                                    double.TryParse(s, out rez);
                                    notaF += rez;
                                }
                            }
                        }
                    }

                    Eval_QuizIntrebari notaFinalaEvaluare = lstEval_QuizIntrebari.Where(p => p.Descriere.ToUpper().Contains("NOTA FINALA EVALUARE") && p.IdQuiz == clsCompetenteAngajat.IdQuiz).FirstOrDefault();
                    if (notaFinalaEvaluare != null)
                    {
                        Eval_RaspunsLinii linieNotaFinala = lstEval_RaspunsLinii.Where(p => p.Id == notaFinalaEvaluare.Id && p.F10003 == clsCompetenteAngajat.F10003 && p.IdQuiz == clsCompetenteAngajat.IdQuiz).FirstOrDefault();
                        PropertyInfo val = linieNotaFinala.GetType().GetProperty("Super" + Session["Eval_ActiveTab"].ToString());

                        if (val != null)
                            val.SetValue(linieNotaFinala, notaF.ToString("0.##"), null);
                    }

                    Session["lstEval_RaspunsLinii"] = lstEval_RaspunsLinii;
                }        


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

                clsNew.IdAuto = Convert.ToInt32(General.Nz(lst.Max(p => (int?)p.IdAuto),0)) + 1;
                clsNew.F10003 = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1));
                clsNew.Pozitie = Convert.ToInt32(Session["Eval_ActiveTab"]);
                clsNew.IdQuiz = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1));
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

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                int idRap = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdRaport"], -99));

                if (idRap != -99)
                {
                    var reportParams = new
                    {
                        Angajat = Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], -99)),
                        Perioada = idPerioada
                    };

                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
                    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);

                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Eval_Print", "window.location.href = \"" + ResolveClientUrl(reportUrl) + "\"", true);
                }
                else
                {
                    Session["PrintDocument"] = "Evaluare";
                    if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20) Session["PrintDocument"] = "EvaluareFilip";
                    if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 24) Session["PrintDocument"] = "EvaluareCristim";

                    Session["PrintParametrii"] = Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) + ";" + Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) + ";" + Session["UserId"] + ";Super" + (Session["Eval_ActiveTab"] ?? "1").ToString() + ";0";
                    Session["PaginaWeb"] = "Eval/EvalDetaliu.aspx";
                    Response.Redirect("~/Reports/Imprima.aspx", true);
                }
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
                //Florin 2019.02.27
                if ((Convert.ToInt32(General.Nz(idCateg, 0)) == 0 && Convert.ToInt32(Session["Eval_ActiveTab"]) != Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1))) || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Finalizat"], 1)) == 1 || Convert.ToInt32(General.Nz(Session["CompletareChestionar_Modifica"], 1)) == 0)
                {
                    //MessageBox.Show("Nu aveti drepturi pentru aceasta operatie!", MessageBox.icoSuccess);
                    pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie!");
                    return;
                }

                string ras = "";
                if (ras != "")
                {                  
                    pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ras);
                    return;
                }

                //Florin 2020.01.27
                btnSave_Click(sender,e);

                #region Verificare minim 5 evaluari 360

                //if (Convert.ToInt32(Convert.ToInt32(General.Nz(Session["IdClient"], 1))) == 20 && idCateg == "0" && Convert.ToInt32(General.Nz(Session["CompletareChestionar_Pozitie"], 1)) == 1)
                //{
                //    DataTable dt = General.IncarcaDT(
                //        @"SELECT DISTINCT IdUser FROM Eval_RaspunsIstoric 
                //        WHERE F10003 = @1 AND COALESCE(Aprobat,0)=1
                //        AND IdQuiz IN 
                //        (SELECT DISTINCT A.Id
                //        FROM Eval_Quiz A
                //        INNER JOIN Eval_relGrupAngajatQuiz B ON A.Id=B.IdQuiz
                //        INNER JOIN Eval_SetAngajatiDetail C ON B.IdGrup=C.IdSetAng
                //        WHERE A.CategorieQuiz IN (1,2) AND C.Id = @1)", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                //    if (dt == null || dt.Rows.Count < 5)
                //    {
                //        pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Trebuie sa aveti minim 5 evaluari 360 pentru a putea finaliza!");
                //        return;
                //    }
                //}

                #endregion  

                string msg = QuizAproba();
 
                if (msg.Length > 0)
                {
                    pnlSectiune.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(msg);
                    return;                                    
                }
				
				msg = Notif.TrimiteNotificare("Eval.EvalLista", (int)Constante.TipNotificare.Notificare, @"SELECT Z.*, 1 AS ""Actiune"" FROM ""Eval_Raspuns"" Z WHERE ""IdQuiz""=" + Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) + @"AND F10003 = " + Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
				if (msg.Length > 0)                    
					General.CreazaLog(msg);   
				
                string url = "~/Eval/EvalLista.aspx?q=56";
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

        public string QuizAproba()
        {
            string msg = "";

            try
            {
                string strSql = @"SELECT COUNT(*) FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND COALESCE(""Aprobat"",0) = 1 AND ""IdUser"" = @3";
                int este = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Session["UserId"] } ),0));
                if (este == 1)
                    return "Formularul este deja aprobat";

                DataTable entQz = General.IncarcaDT(@"SELECT * FROM ""Eval_Quiz"" WHERE ""Id"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                DataTable ent = General.IncarcaDT(@"SELECT * FROM ""Eval_Raspuns"" WHERE ""IdQuiz"" = @1 AND F10003 = @2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                DataTable lstIst = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                DataTable entIst = General.IncarcaDT(
                    @"SELECT * FROM ""Eval_RaspunsIstoric"" a 
                    WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0)
                    AND ((""IdSuper"" >= 0 AND ""IdUser"" = @3) OR (a.""IdSuper"" < 0 AND @3 in (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = @2 
                    AND b.""IdSuper"" = (-1) * a.""IdSuper""))) ORDER BY ""Pozitie"" ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Session["UserId"] });       

                if (ent == null || ent.Rows.Count <= 0 || entIst == null || entIst.Rows.Count <= 0) return "Date incomplete in istoric.";

                int respectaOrdinea = 0;
                DataTable entCir = General.IncarcaDT(@"SELECT * FROM ""Eval_Circuit"" WHERE ""IdQuiz"" = @1", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                if (entCir != null && entCir.Rows.Count > 0) respectaOrdinea = Convert.ToInt32(entCir.Rows[0]["RespectaOrdinea"] != DBNull.Value ? entCir.Rows[0]["RespectaOrdinea"].ToString() : "0"); 

                //verificam daca se respecta ordinea din circuit
                if (respectaOrdinea == 1 && Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") != Convert.ToInt32(ent.Rows[0]["Pozitie"] != DBNull.Value ? ent.Rows[0]["Pozitie"].ToString() : "-99")) return "Nu puteti aproba in acest moment.";
                
                //cautam urmatoarea pozitie din istoric care nu a aprobat
                int poz = Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "0") + 1;
                DataTable entPoz = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Aprobat"" IS NULL AND ""Pozitie"" = @3 ORDER BY ""Pozitie"" ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), (entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") });
                if (entPoz != null && entPoz.Rows.Count > 0) poz = Convert.ToInt32(entPoz.Rows[0]["Pozitie"] != DBNull.Value ? entPoz.Rows[0]["Pozitie"].ToString() : "0") + 1;
                string culoare = DamiEvalCuloare(poz);

                if (poz > Convert.ToInt32(ent.Rows[0]["TotalCircuit"].ToString()))
                {
                    poz--;
                    ent.Rows[0]["Finalizat"] = 1;
                    //Session["CompletareChestionar_Finalizat"] = 1;
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
                    //Session["CompletareChestionar_Finalizat"] = 1;
                    ent.Rows[0]["Culoare"] = "#ffc8ffc8";
                }

                #endregion

                //daca in chestionar exista tipul de camp 36 - Data Finalizare
                //DataTable entDtFin = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsLinii"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""TipData"" = 36", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                //if (entDtFin != null && entDtFin.Rows.Count > 0)
                //{
                //    string sql = @"UPDATE ""Eval_RaspunsLinii"" SET ";
                //    for (int i = 1; i <= 20; i++)
                //    {                        
                //        sql += " \"Super" + i + "\" = " + (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
                //        if (i < 20)
                //            sql += ", ";
                //    }
                //    sql += "WHERE \"IdQuiz\" = @1 AND F10003 = @2 AND \"TipData\" = 36";
                //    General.IncarcaDT(sql, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                //}


                //General.IncarcaDT(
                //    $@"UPDATE ""Eval_RaspunsLinii""
                //    SET ""Super1""={General.CurrentDate()},""Super2""={General.CurrentDate()},""Super3""={General.CurrentDate()},""Super4""={General.CurrentDate()},
                //    ""Super5""={General.CurrentDate()},""Super6""={General.CurrentDate()},""Super7""={General.CurrentDate()},""Super8""={General.CurrentDate()},
                //    ""Super9""={General.CurrentDate()},""Super10""={General.CurrentDate()},""Super11""={General.CurrentDate()},""Super12""={General.CurrentDate()},
                //    ""Super13""={General.CurrentDate()},""Super14""={General.CurrentDate()},""Super15""={General.CurrentDate()},""Super16""={General.CurrentDate()},
                //    ""Super17""={General.CurrentDate()},""Super18""={General.CurrentDate()},""Super19""={General.CurrentDate()},""Super20""={General.CurrentDate()}, 
                //    WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""TipData""=36", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });

                //General.IncarcaDT(@"UPDATE ""Eval_Raspuns"" SET ""Pozitie"" = @1, ""Culoare"" = @2, ""Finalizat"" = @3 WHERE ""IdQuiz"" = @4 AND F10003 = @5", new object[] { ent.Rows[0]["Pozitie"].ToString(), ent.Rows[0]["Culoare"].ToString(), (ent.Rows[0]["Finalizat"] != DBNull.Value ? ent.Rows[0]["Finalizat"].ToString() : "0").ToString(), Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                //General.IncarcaDT(
                //    $@"UPDATE ""Eval_RaspunsIstoric"" SET ""DataAprobare"" = {General.CurrentDate()}, ""Aprobat"" = 1, ""Culoare"" = @2, USER_NO = @3, TIME = {General.CurrentDate()} WHERE ""IdQuiz"" = @5 AND F10003 = @6 AND ((""IdSuper"" >= 0 AND ""IdUser"" = @7) OR
                //    (""IdSuper"" < 0 AND @7 IN (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = @6 AND b.""IdSuper"" = (-1) * ""Eval_RaspunsIstoric"".""IdSuper"" ))) AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0)", new object[] { (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), culoare, Session["UserId"], (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Session["UserId"] });


                string tmpSql = $@"
                    BEGIN
                    UPDATE ""Eval_RaspunsLinii""
                    SET ""Super1""=CAST({General.CurrentDate()} AS varchar(30)),""Super2""=CAST({General.CurrentDate()} AS varchar(30)),""Super3""=CAST({General.CurrentDate()} AS varchar(30)),""Super4""=CAST({General.CurrentDate()} AS varchar(30)),
                    ""Super5""=CAST({General.CurrentDate()} AS varchar(30)),""Super6""=CAST({General.CurrentDate()} AS varchar(30)),""Super7""=CAST({General.CurrentDate()} AS varchar(30)),""Super8""=CAST({General.CurrentDate()} AS varchar(30)),
                    ""Super9""=CAST({General.CurrentDate()} AS varchar(30)),""Super10""=CAST({General.CurrentDate()} AS varchar(30)),""Super11""=CAST({General.CurrentDate()} AS varchar(30)),""Super12""=CAST({General.CurrentDate()} AS varchar(30)),
                    ""Super13""=CAST({General.CurrentDate()} AS varchar(30)),""Super14""=CAST({General.CurrentDate()} AS varchar(30)),""Super15""=CAST({General.CurrentDate()} AS varchar(30)),""Super16""=CAST({General.CurrentDate()} AS varchar(30)),
                    ""Super17""=CAST({General.CurrentDate()} AS varchar(30)),""Super18""=CAST({General.CurrentDate()} AS varchar(30)),""Super19""=CAST({General.CurrentDate()} AS varchar(30)),""Super20""=CAST({General.CurrentDate()} AS varchar(30))
                    WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""TipData""=36;
                    
                    UPDATE ""Eval_Raspuns"" SET ""Pozitie"" = @3, ""Culoare"" = @4, ""Finalizat"" = @5 WHERE ""IdQuiz"" = @1 AND F10003 = @2;

                    UPDATE ""Eval_RaspunsIstoric"" SET ""DataAprobare"" = {General.CurrentDate()}, ""Aprobat"" = 1, ""Culoare"" = @4, USER_NO = {Session["UserId"]}, TIME = {General.CurrentDate()} WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ((""IdSuper"" >= 0 AND ""IdUser"" = {Session["UserId"]}) OR
                    (""IdSuper"" < 0 AND {Session["UserId"]} IN (SELECT ""IdUser"" FROM ""F100Supervizori"" b where F10003 = @2 AND b.""IdSuper"" = (-1) * ""Eval_RaspunsIstoric"".""IdSuper"" ))) AND (""Aprobat"" IS NULL OR ""Aprobat"" = 0);
                    END;";

                General.ExecutaNonQuery(tmpSql, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), General.Nz(ent.Rows[0]["Pozitie"],1), General.Nz(ent.Rows[0]["Culoare"], "#FFFFFF00"), General.Nz(ent.Rows[0]["Finalizat"],0) });

              
                if (ent.Rows[0]["Finalizat"].ToString() != "1")
                {
                    if (Dami.ValoareParam("PreluareDateAutomat", "0") == "1")
                    {
                        //NOP
                        //functia de sincronizare date se ocupa de preluarea datelor de la o pozitie la alta
                    }
                    else
                    {
                        string sqlOC = 
                            $@"BEGIN
                            DELETE FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1))};

                            INSERT INTO ""Eval_ObiIndividualeTemp"" (""IdPeriod"", ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME, ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Id"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"", ""IdCategObiective"")
                            SELECT ""IdPeriod"", ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""IdQuiz"", F10003, {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1))}, ""IdLinieQuiz"", ""IdUnic"", {Session["UserId"]}, {General.CurrentDate()}, ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Id"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"", ""IdCategObiective""
                            FROM ""Eval_ObiIndividualeTemp"" 
                            WHERE ""IdQuiz"" =@1 AND F10003 =@2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1)) - 1};

                            DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"],1))};

                            INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdPeriod"", ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"", ""IdUnic"", USER_NO, TIME, ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Explicatii"", ""Id"")
                            SELECT ""IdPeriod"", ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""IdQuiz"", F10003, {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"],1))}, ""IdLinieQuiz"", ""IdUnic"", {Session["UserId"]}, {General.CurrentDate()}, ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Explicatii"", ""Id""
                            FROM ""Eval_CompetenteAngajatTemp"" 
                            WHERE  ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1)) - 1};

                            END; ";

                        General.ExecutaNonQuery(sqlOC, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
                    }

                    //DataTable dtCompetente = General.IncarcaDT(@"SELECT ""Id"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""TipData"" = 5", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });

                    //if (dtCompetente != null && dtCompetente.Rows.Count > 0)
                    //{
                    //    for (int i = 0; i < dtCompetente.Rows.Count; i++)
                    //    {
                    //        General.IncarcaDT(@"DELETE FROM ""Eval_CompetenteAngajatTemp"" WHERE ""IdLinieQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3 AND ""IdQuiz"" = @4 ", new object[] { dtCompetente.Rows[i][0].ToString(), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });

                    //        Session["lstEval_CompetenteAngajatTemp"] = null;

                    //        string sql = $@"INSERT INTO ""Eval_CompetenteAngajatTemp"" (""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""IdLinieQuiz"")
                    //            SELECT ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003, {Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString())}, ""IdLinieQuiz""
                    //            FROM ""Eval_CompetenteAngajatTemp"" WHERE
                    //            ""IdLinieQuiz"" = @2 AND F10003 = @3 AND ""Pozitie"" = @4 AND ""IdQuiz"" = @5 ";
                    //        General.IncarcaDT(sql, new object[] { Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()), dtCompetente.Rows[i][0].ToString(), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), Convert.ToInt32(ent.Rows[0]["Pozitie"].ToString()) - 1, Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)) });
                    //    }
                    //}
                }
                else
                {
                    //Florin 2020.01.23 - am scos -1 de la filtrul pozitie Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1) - 1)
                    //Florin 2020.01.27 - am adaugat si script de stergere
                    //Radu 23.10.2019 - se transfera in EvalObiIndividuale si Eval_CompetenteAngajat
                    string sqlOC =
                            $@"BEGIN

                            DELETE FROM ""Eval_ObiIndividuale"" WHERE F10003 =@2 AND ""IdPeriod"" = {idPerioada};

                            INSERT INTO ""Eval_ObiIndividuale"" (""IdPeriod"", F10003, ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"")
                            SELECT ""IdPeriod"", F10003, ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4""
                            FROM ""Eval_ObiIndividualeTemp"" 
                            WHERE ""IdQuiz"" =@1 AND F10003 =@2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1))};

                            DELETE FROM ""Eval_CompetenteAngajat"" WHERE F10003 =@2 AND ""IdPeriod"" = {idPerioada};

                            INSERT INTO ""Eval_CompetenteAngajat"" (""IdPeriod"", F10003, ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"",  ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Explicatii"")
                            SELECT ""IdPeriod"",  F10003, ""IdCategCompetenta"", ""CategCompetenta"", ""IdCompetenta"", ""Competenta"", ""Pondere"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""Explicatii""
                            FROM ""Eval_CompetenteAngajatTemp"" 
                            WHERE  ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = {Convert.ToInt32(General.Nz(ent.Rows[0]["Pozitie"], 1))};

                            END; ";
                    General.ExecutaNonQuery(sqlOC, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });

                }

                if (entQz != null && entQz.Rows.Count > 0 && Convert.ToInt32(entQz.Rows[0]["Preluare"] != DBNull.Value ? entQz.Rows[0]["Preluare"].ToString() : "0") == 1 && Convert.ToInt32(entIst.Rows[0]["Pozitie"] != DBNull.Value ? entIst.Rows[0]["Pozitie"].ToString() : "-99") < Convert.ToInt32(ent.Rows[0]["TotalCircuit"].ToString()))
                    QuizPreluareDate(Convert.ToInt32(General.Nz(entIst.Rows[0]["Pozitie"],-99)));
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
                General.IncarcaDT(
                    $@"
                    BEGIN

                    UPDATE ""Eval_RaspunsLinii"" 
                    SET 
                    ""Super{poz + 1}""=""Super{poz}"", 
                    ""Super{poz + 1}_1""=""Super{poz}_1"", 
                    ""Super{poz + 1}_2""=""Super{poz}_2"", 
                    ""Super{poz + 1}_3""=""Super{poz}_3"", 
                    ""Super{poz + 1}_4""=""Super{poz}_4""   
                    WHERE ""IdQuiz""=@1 AND F10003=@2 AND (""Super{poz + 1}"" is null OR RTRIM(LTRIM(""Super{poz + 1}""))='') AND ""Super{poz}"" is not null;

                    UPDATE B
                    SET  B.""IdPeriod"" = A.""IdPeriod"", B.""Pondere"" = A.""Pondere"", B.""IdCalificativ"" = A.""IdCalificativ"", B.""Calificativ"" = A.""Calificativ"", 
                    B.""ExplicatiiCalificativ"" = A.""ExplicatiiCalificativ"", B.""Explicatii"" = A.""Explicatii"", B.""Id"" = A.""Id"", TIME = {General.CurrentDate()}, USER_NO = {Session["UserId"]}
                    FROM ""Eval_CompetenteAngajatTemp"" B
                    LEFT JOIN ""Eval_CompetenteAngajatTemp"" A ON A.""IdUnic"" = B.""IdUnic"" AND A.""Pozitie"" = {poz}
                    WHERE B.""IdQuiz"" = @1 AND B.F10003 = @2 AND B.""Pozitie"" = {poz + 1};

                    UPDATE B
                    SET B.""IdPeriod"" = A.""IdPeriod"", B.""Pondere""=A.""Pondere"", B.""Descriere""=A.""Descriere"", B.""Target""=A.""Target"", B.""Termen""=A.""Termen"", B.""Realizat""=A.""Realizat"",
                    B.""IdCalificativ""=A.""IdCalificativ"", B.""Calificativ""=A.""Calificativ"", B.""ExplicatiiCalificativ""=A.""ExplicatiiCalificativ"", 
                    B.""ColoanaSuplimentara1""=A.""ColoanaSuplimentara1"", B.""ColoanaSuplimentara2""=A.""ColoanaSuplimentara2"", B.""ColoanaSuplimentara3""=A.""ColoanaSuplimentara3"", B.""ColoanaSuplimentara4""=A.""ColoanaSuplimentara4"", B.""Id""=A.""Id"", TIME = {General.CurrentDate()}, USER_NO = {Session["UserId"]}
                    FROM ""Eval_ObiIndividualeTemp"" B
                    LEFT JOIN ""Eval_ObiIndividualeTemp"" A ON A.""IdUnic"" = B.""IdUnic"" AND A.""Pozitie"" = {poz}
                    WHERE B.""IdQuiz"" = @1 AND B.F10003 = @2 AND B.""Pozitie"" = {poz + 1};

                    END;", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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
                    string strSQLObiectivActivitate = @"select ob.""IdObiectiv"" as ""Parinte"", obAct.""IdActivitate"" as ""Id"", CAST(obAct.""Activitate"" AS varchar(4000)) as ""Denumire""
                                                                            from ""Eval_ListaObiectivDet"" det
                                                                            join ""Eval_SetAngajatiDetail"" setAng on det.""IdSetAngajat"" = setAng.""IdSetAng""
                                                                            join ""Eval_Obiectiv"" ob on det.""IdObiectiv"" = ob.""IdObiectiv""
                                                                            join ""Eval_ObiectivXActivitate"" obAct on det.""IdObiectiv"" = obAct.""IdObiectiv""
                                                                                                                    and det.""IdActivitate"" = obAct.""IdActivitate""
                                                                            where setAng.""Id"" = @1
                                                                            group by ob.""IdObiectiv"", obAct.""IdActivitate"", CAST(obAct.""Activitate"" AS varchar(4000)) ";
                    strSQLObiectivActivitate = string.Format(strSQLObiectivActivitate, Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)));
                    DataTable dtObiectivActivitate = General.IncarcaDT(strSQLObiectivActivitate, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)) });
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

                DataTable dtIntreb = General.IncarcaDT(@"SELECT ""Id"", ""Descriere"" FROM ""Eval_QuizIntrebari"" WHERE ""IdQuiz"" = @1 AND ""Parinte"" = @2 AND (""Descriere"" like '%Vitality%' OR ""Descriere"" like '%Effectiveness%' OR ""Descriere"" like '%Common Sense%' OR ""Descriere"" like '%Creativity%' OR ""Descriere"" like '%Relationships%' OR ""Descriere"" like '%Others%')", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), parinte });
                string descriere = "";
                for (int i = 0; i < dtIntreb.Rows.Count - 1; i++)
                    if (Convert.ToInt32(dtIntreb.Rows[i]["Id"].ToString()) < id && id < Convert.ToInt32(dtIntreb.Rows[i + 1]["Id"].ToString()))
                    {
                        descriere = dtIntreb.Rows[i]["Descriere"].ToString();
                        break;
                    }
                if (descriere.Length <= 0)
                    descriere = dtIntreb.Rows[dtIntreb.Rows.Count - 1]["Descriere"].ToString();

                //string op = "+";
                //if (Constante.tipBD == 2) op = "||";
                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""{numeView}"" WHERE F10003 = @1 AND ""Perioada""=(SELECT ""Anul"" FROM ""Eval_Quiz"" WHERE ""Id""=@2) AND '{descriere}' LIKE '%' {Dami.Operator()} ""Descriere"" {Dami.Operator()} '%' ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)).ToString(), Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)).ToString() });

                grDate.AutoGenerateColumns = true;
                grDate.DataSource = dt;
                grDate.Width = Unit.Percentage(100);
                grDate.ID = "grDate_DinView_" + id;
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
                if (grDate.Columns["Perioada"] != null)
                {
                    grDate.Columns["Perioada"].Visible = false;
                    grDate.Columns["Perioada"].ShowInCustomizationForm = false;

                }
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
                DataTable dt = General.IncarcaDT($@"SELECT ""PuncteForte"" FROM ""viewEvaluare360"" WHERE F10003 = @1 AND ""Descriere"" LIKE '%Others%' ", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)).ToString() });

                grDate.AutoGenerateColumns = true;
                grDate.DataSource = dt;
                grDate.Width = Unit.Percentage(100);
                grDate.ID = "grDate_DinView_Others";
                grDate.DataBind();
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
                //string sablon =
                //    @"UPDATE ""Eval_RaspunsLinii"" SET ""Super@pozIst""=""Super@poz""
                //    WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Super@poz"" IS NOT NULL AND ""Super@poz""<>'' AND ""Super@pozIst""<>""Super@poz"";";

                string sablon =
                    $@"UPDATE ""Eval_RaspunsLinii"" SET ""Super@pozIst""=""Super@poz""
                    WHERE ""IdQuiz""=@1 AND F10003=@2 {General.FiltrulCuNull("Super@poz")} AND ""Super@pozIst""<>""Super@poz"";";
                if (Constante.tipBD == 2)
                    sablon =
                        $@"UPDATE ""Eval_RaspunsLinii"" SET ""Super@pozIst""=""Super@poz""
                        WHERE ""IdQuiz""=@1 AND F10003=@2 {General.FiltrulCuNull("Super@poz")} AND TO_CHAR(""Super@pozIst"")<>TO_CHAR(""Super@poz"");";

                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_RaspunsIstoric"" WHERE ""IdQuiz""=@1 AND F10003=@2 AND ""Pozitie""<>@3", new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), pozitie });
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    if (General.Nz(dt.Rows[i]["Pozitie"], "").ToString() != "")
                        sqlUpd += sablon.Replace("@pozIst", General.Nz(dt.Rows[i]["Pozitie"], 1).ToString()).Replace("@poz", pozitie.ToString()) + Environment.NewLine;
                }

                //string sqlSinc = $@"                    
                //    UPDATE B
                //    SET B.""IdPeriod"" = A.""IdPeriod"", B.""Obiectiv""=A.""Obiectiv"", B.""Activitate""=A.""Activitate"", B.""Descriere""=A.""Descriere"", B.""Pondere""=A.""Pondere"", B.""Target""=A.""Target"", B.""Termen""=A.""Termen"", B.""Realizat""=A.""Realizat"", B.""IdCalificativ""=A.""IdCalificativ"",B.""Calificativ""=A.""Calificativ"",  B.""ExplicatiiCalificativ""=A.""ExplicatiiCalificativ"", B.""ColoanaSuplimentara1""=A.""ColoanaSuplimentara1"", B.""ColoanaSuplimentara2""=A.""ColoanaSuplimentara2"", B.""ColoanaSuplimentara3""=A.""ColoanaSuplimentara3"", B.""ColoanaSuplimentara4""=A.""ColoanaSuplimentara4"", B.""IdCategObiective""=A.""IdCategObiective""
                //    FROM ""Eval_ObiIndividualeTemp"" A
                //    INNER JOIN ""Eval_ObiIndividualeTemp"" B ON A.""IdUnic""=B.""IdUnic"" AND A.""Pozitie""<>B.""Pozitie""
                //    WHERE A.""IdQuiz"" = @1 AND A.F10003 = @2 AND A.""Pozitie""=@3;";

                //if (Constante.tipBD == 2)
                string sqlSinc = $@"MERGE INTO ""Eval_ObiIndividualeTemp"" B
                            USING (SELECT * FROM ""Eval_ObiIndividualeTemp"" WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie"" = @3) A
                            ON(A.""IdUnic"" = B.""IdUnic"" AND A.""Pozitie"" <> B.""Pozitie"")
                            WHEN MATCHED THEN
                            UPDATE SET B.""IdPeriod"" = A.""IdPeriod"", B.""Obiectiv""=A.""Obiectiv"", B.""Activitate""=A.""Activitate"", B.""Descriere""=A.""Descriere"", B.""Pondere""=A.""Pondere"", B.""Target""=A.""Target"", B.""Termen""=A.""Termen"", B.""Realizat""=A.""Realizat"", B.""IdCalificativ""=A.""IdCalificativ"",B.""Calificativ""=A.""Calificativ"",  B.""ExplicatiiCalificativ""=A.""ExplicatiiCalificativ"", B.""ColoanaSuplimentara1""=A.""ColoanaSuplimentara1"", B.""ColoanaSuplimentara2""=A.""ColoanaSuplimentara2"", B.""ColoanaSuplimentara3""=A.""ColoanaSuplimentara3"", B.""ColoanaSuplimentara4""=A.""ColoanaSuplimentara4"", B.""IdCategObiective""=A.""IdCategObiective"";";


                string strSql =
                    $@"
                    BEGIN

                    {sqlUpd}

                    {sqlSinc}
                    
                    INSERT INTO ""Eval_ObiIndividualeTemp""(""IdPeriod"", ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", ""IdQuiz"", F10003,  ""Pozitie"", ""Id"",  ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"", ""IdUnic"", USER_NO, TIME, ""IdCategObiective"")
                    SELECT ""IdPeriod"", ""IdObiectiv"", ""Obiectiv"", ""IdActivitate"", ""Activitate"", ""Pondere"", ""Descriere"", ""Target"", ""Termen"", ""Realizat"", ""IdCalificativ"", ""Calificativ"", ""ExplicatiiCalificativ"", A.""IdQuiz"", A.F10003,  B.""Pozitie"", ""Id"",  ""IdLinieQuiz"", ""ColoanaSuplimentara1"", ""ColoanaSuplimentara2"", ""ColoanaSuplimentara3"", ""ColoanaSuplimentara4"", ""IdUnic"", A.USER_NO, A.TIME, ""IdCategObiective""
                    FROM ""Eval_ObiIndividualeTemp"" A
                    INNER JOIN ""Eval_RaspunsIstoric"" B ON A.""IdQuiz""=B.""IdQuiz"" AND A.F10003=B.F10003 AND B.""Pozitie""<>@3
                    WHERE A.""IdQuiz"" = @1 AND A.F10003 = @2 AND A.""Pozitie""=@3 AND
                    (B.""Pozitie"" * 100000000 + A.""IdUnic"") NOT IN (SELECT (""Pozitie"" * 100000000 + ""IdUnic"")
                    FROM ""Eval_ObiIndividualeTemp"" A
                    WHERE A.""IdQuiz"" = @1 AND A.F10003 = @2 AND A.""Pozitie""<>@3);

                    DELETE
                    FROM ""Eval_ObiIndividualeTemp""
                    WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie""<>@3 AND
                    ""IdUnic"" NOT IN (SELECT ""IdUnic""
                    FROM ""Eval_ObiIndividualeTemp""
                    WHERE ""IdQuiz"" = @1 AND F10003 = @2 AND ""Pozitie""=@3);

                    END;";

                General.ExecutaNonQuery(strSql, new object[] { Convert.ToInt32(General.Nz(Session["CompletareChestionar_IdQuiz"], 1)), Convert.ToInt32(General.Nz(Session["CompletareChestionar_F10003"], 1)), pozitie });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        //Florin 2019.06.27
        public void SalveazaGridurile()
        {
            try
            {
                if (General.Nz(Session["NumeGriduri"], "").ToString() != "")
                {
                    string[] arr = Session["NumeGriduri"].ToString().Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < arr.Length; i++)
                    {
                        ASPxGridView grDate = grIntrebari.FindControl(arr[i]) as ASPxGridView;
                        if (grDate != null)
                            grDate.UpdateEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private ASPxHyperLink CreeazaLink(string valoare, int idCtl)
        {
            ASPxHyperLink lnk = new ASPxHyperLink();

            try
            {
                lnk.Text = valoare;
                lnk.Font.Underline = true;
                lnk.NavigateUrl = valoare;
                lnk.Target = "_blank";
                lnk.ID = "lnk_" + idCtl;
                lnk.Wrap = DevExpress.Utils.DefaultBoolean.True;
                lnk.ForeColor = Evaluare.CuloareBrush("#FF000099");
                lnk.Font.Size = 12;
                lnk.Style.Add("margin-bottom", "10px");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lnk;
        }

    }
}
 