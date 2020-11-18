using DevExpress.Web;
using DevExpress.Web.ASPxTreeList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class QuizCreate : System.Web.UI.UserControl
    {
        //string cmp = "USER_NO,TIME,IDAUTO,";
        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
            public int Parinte { get;set;}
        }

        public class metaEval_tblTipCamp
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
            public int Parinte { get; set; }
            public int Activ { get; set; }
        }

        public class metaCmb
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }
        List<metaDate> lstEval_tblCategTipCamp = new List<metaDate>();
        List<metaEval_tblTipCamp> lstEval_tblTipCamp = new List<metaEval_tblTipCamp>();
        List<metaDate> lstEval_tblTipValori = new List<metaDate>();
        List<metaDate> lstEval_tblIntrebari = new List<metaDate>();
        List<metaDate> lstEval_Perioada = new List<metaDate>();

        List<Eval_DictionaryItem> lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
        List<vwEval_ConfigObiectivCol> lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();
        List<Eval_ConfigObiective> lstEval_ConfigObiective = new List<Eval_ConfigObiective>();
        List<Eval_ConfigObTemplate> lstEval_ConfigObTemplate = new List<Eval_ConfigObTemplate>();
        List<Eval_ConfigObTemplateDetail> lstEval_ConfigObTemplateDetail = new List<Eval_ConfigObTemplateDetail>();
        List<Eval_ConfigObTemplateDetail> lstDefaultEval_ConfigObTemplateDetail = new List<Eval_ConfigObTemplateDetail>();

        List<vwEval_ConfigCompetenteCol> lstVwEval_ConfigCompetenteCol = new List<vwEval_ConfigCompetenteCol>();
        List<Eval_ConfigCompetente> lstEval_ConfigCompetente = new List<Eval_ConfigCompetente>();
        List<Eval_ConfigCompTemplate> lstEval_ConfigCompTemplate = new List<Eval_ConfigCompTemplate>();
        List<Eval_ConfigCompTemplateDetail> lstEval_ConfigCompTemplateDetail = new List<Eval_ConfigCompTemplateDetail>();



        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                DataTable table = new DataTable();
                DataTable tableIntrebari = new DataTable();
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                table = ds.Tables[0];

                dtList1.DataSource = table;
                dtList1.DataBind();
                if (ds.Tables.Contains("Eval_QuizIntrebari"))
                {
                    tableIntrebari = ds.Tables["Eval_QuizIntrebari"];
                }
                else
                {
                    string strSQL = " select * from \"Eval_QuizIntrebari\" where \"IdQuiz\" ={0}";
                    strSQL = string.Format(strSQL, Session["IdEvalQuiz"].ToString());
                    tableIntrebari = General.IncarcaDT(strSQL, null);
                }
                tableIntrebari.PrimaryKey = new DataColumn[] { tableIntrebari.Columns["Id"] };

                grDateIntrebari.DataSource = tableIntrebari;
                grDateIntrebari.DataBind();

                #region Nomenclatoare
                if (Session["nomenEval_tblCategTipCamp"] == null)
                {
                    string strSQL = " select * from \"Eval_tblCategTipCamp\" ";
                    DataTable dtEval_tblCategTipCamp = General.IncarcaDT(strSQL, null);
                    lstEval_tblCategTipCamp = new List<metaDate>();
                    foreach (DataRow rwEval_tblCategTipCamp in dtEval_tblCategTipCamp.Rows)
                    {
                        metaDate clsMeta = new metaDate();
                        clsMeta.Id = Convert.ToInt32(rwEval_tblCategTipCamp["IdCategorie"].ToString());
                        clsMeta.Denumire = rwEval_tblCategTipCamp["DenCategorie"].ToString();
                        lstEval_tblCategTipCamp.Add(clsMeta);
                    }
                    Session["nomenEval_tblCategTipCamp"] = lstEval_tblCategTipCamp;
                    cmbTip.DataSource = lstEval_tblCategTipCamp;
                    cmbTip.DataBind();
                }
                else
                {
                    cmbTip.DataSource = Session["nomenEval_tblCategTipCamp"];
                    cmbTip.DataBind();
                }

                if (Session["nomenEval_tblTipCamp"] == null)
                {
                    string strSQL = "select * from \"Eval_tblTipCamp\" ";
                    DataTable dtEval_tblTipCamp = General.IncarcaDT(strSQL, null);
                    lstEval_tblTipCamp = new List<metaEval_tblTipCamp>();
                    foreach (DataRow rwEval_tblTipCamp in dtEval_tblTipCamp.Rows)
                    {
                        metaEval_tblTipCamp clsMeta = new metaEval_tblTipCamp();
                        clsMeta.Id = Convert.ToInt32(rwEval_tblTipCamp["Id"].ToString());
                        clsMeta.Denumire = rwEval_tblTipCamp["Denumire"].ToString();
                        clsMeta.Parinte = Convert.ToInt32(rwEval_tblTipCamp["IdCategorie"].ToString());
                        clsMeta.Activ = Convert.ToInt32(General.Nz(rwEval_tblTipCamp["Activ"],0));
                        lstEval_tblTipCamp.Add(clsMeta);
                    }

                    Session["nomenEval_tblTipCamp"] = lstEval_tblTipCamp;
                }
                else
                {
                    lstEval_tblTipCamp = new List<metaEval_tblTipCamp>();
                    lstEval_tblTipCamp = Session["nomenEval_tblTipCamp"] as List<metaEval_tblTipCamp>;
                }

                if (Session["nomenEval_tblTipValori"] == null)
                {
                    string strSQL = "select * from \"Eval_tblTipValori\" ";
                    DataTable dtEval_tblTipValori = General.IncarcaDT(strSQL, null);
                    lstEval_tblTipValori = new List<metaDate>();
                    foreach (DataRow rwEval_tblTipValori in dtEval_tblTipValori.Rows)
                    {
                        metaDate clsEval_tblTipValori = new metaDate();
                        clsEval_tblTipValori.Id = Convert.ToInt32(rwEval_tblTipValori["Id"].ToString());
                        clsEval_tblTipValori.Denumire = rwEval_tblTipValori["Denumire"].ToString();
                        lstEval_tblTipValori.Add(clsEval_tblTipValori);
                    }

                    Session["nomenEval_tblTipValori"] = lstEval_tblTipValori;
                    cmbSursaDate.DataSource = lstEval_tblTipValori;
                    cmbSursaDate.DataBind();
                }
                else
                {
                    lstEval_tblTipValori = Session["nomenEval_tblTipValori"] as List<metaDate>;
                    cmbSursaDate.DataSource = lstEval_tblTipValori;
                    cmbSursaDate.DataBind();
                }

                if (Session["nomenEval_tblIntrebari"] == null)
                {
                    string strSQL = "select * from \"Eval_tblIntrebari\" ";
                    DataTable dtEval_tblIntrebari = General.IncarcaDT(strSQL, null);
                    lstEval_tblIntrebari = new List<metaDate>();
                    foreach (DataRow rwEval_tblIntrebari in dtEval_tblIntrebari.Rows)
                    {
                        metaDate clsEval_tblIntrebari = new metaDate();
                        clsEval_tblIntrebari.Id = Convert.ToInt32(rwEval_tblIntrebari["Id"].ToString());
                        clsEval_tblIntrebari.Denumire = rwEval_tblIntrebari["Descriere"].ToString();
                        lstEval_tblIntrebari.Add(clsEval_tblIntrebari);
                    }
                    Session["nomenEval_tblIntrebari"] = lstEval_tblIntrebari;
                    //cmbDinNomenclator.DataSource = lstEval_tblIntrebari;
                    //cmbDinNomenclator.DataBind();
                }
                else
                {
                    lstEval_tblIntrebari = Session["nomenEval_tblIntrebari"] as List<metaDate>;
                    //cmbDinNomenclator.DataSource = lstEval_tblIntrebari;
                    //cmbDinNomenclator.DataBind();
                }


                if (Session["nomenEval_Perioada"] == null)
                {
                    string strSQL = "select \"IdPerioada\" AS \"Id\", \"DenPerioada\" AS \"Descriere\" from \"Eval_Perioada\" ";
                    DataTable dtEval_Perioada = General.IncarcaDT(strSQL, null);
                    lstEval_Perioada = new List<metaDate>();
                    foreach (DataRow rwEval_Perioada in dtEval_Perioada.Rows)
                    {
                        metaDate clsEval_Perioada = new metaDate();
                        clsEval_Perioada.Id = Convert.ToInt32(rwEval_Perioada["Id"].ToString());
                        clsEval_Perioada.Denumire = rwEval_Perioada["Descriere"].ToString();
                        lstEval_Perioada.Add(clsEval_Perioada);
                    }
                    Session["nomenEval_Perioada"] = lstEval_Perioada;
                    cmbPerioadaObi.DataSource = lstEval_Perioada;
                    cmbPerioadaObi.DataBind();

                    cmbPerioadaComp.DataSource = lstEval_Perioada;
                    cmbPerioadaComp.DataBind();
                }
                else
                {
                    lstEval_Perioada = Session["nomenEval_Perioada"] as List<metaDate>;
                    cmbPerioadaObi.DataSource = lstEval_Perioada;
                    cmbPerioadaObi.DataBind();

                    cmbPerioadaComp.DataSource = lstEval_Perioada;
                    cmbPerioadaComp.DataBind();
                }
                #endregion


                #region configurare Obiective
                if (Session["nomenEval_DictionaryItem"] == null)
                {
                    string strSQLEval_DictionaryItem = @"select * from ""Eval_DictionaryItem"" where ""DictionaryId"" = 1 ";
                    DataTable dtEval_DictionaryItem = General.IncarcaDT(strSQLEval_DictionaryItem, null);
                    lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
                    foreach (DataRow rwEval_DictionaryItem in dtEval_DictionaryItem.Rows)
                    {
                        Eval_DictionaryItem clsDictItem = new Eval_DictionaryItem(rwEval_DictionaryItem);
                        lstEval_DictionaryItem.Add(clsDictItem);
                    }

                    Session["nomenEval_DictionaryItem"] = lstEval_DictionaryItem;
                }
                else
                {
                    lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
                    lstEval_DictionaryItem = Session["nomenEval_DictionaryItem"] as List<Eval_DictionaryItem>;
                }

                if (Session["nomenVwEval_ConfigObiectivCol"] == null)
                {
                    string strSQLEval_ConfigObiectivCol = @"select * from ""vwEval_ConfigObiectivCol"" ";
                    DataTable dtEval_ConfigObiectivCol = General.IncarcaDT(strSQLEval_ConfigObiectivCol, null);
                    lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();
                    foreach (DataRow rwEval_ConfigObiectivCol in dtEval_ConfigObiectivCol.Rows)
                    {
                        vwEval_ConfigObiectivCol clsObiectivCol = new vwEval_ConfigObiectivCol(rwEval_ConfigObiectivCol);
                        lstVwEval_ConfigObiectivCol.Add(clsObiectivCol);
                    }
                    Session["nomenVwEval_ConfigObiectivCol"] = lstVwEval_ConfigObiectivCol;
                }
                else
                {
                    lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();
                    lstVwEval_ConfigObiectivCol = Session["nomenVwEval_ConfigObiectivCol"] as List<vwEval_ConfigObiectivCol>;
                }

                if (Session["createEval_ConfigObTemplate"] == null)
                {
                    string strSQLEval_ConfigObTemplate = @"select * from ""Eval_ConfigObTemplate""";
                    DataTable dtEval_ConfigObTemplate = General.IncarcaDT(strSQLEval_ConfigObTemplate, null);
                    lstEval_ConfigObTemplate = new List<Eval_ConfigObTemplate>();
                    foreach (DataRow rwEval_ConfigObTemplate in dtEval_ConfigObTemplate.Rows)
                    {
                        Eval_ConfigObTemplate clsEval_ConfigObTemplate = new Eval_ConfigObTemplate(rwEval_ConfigObTemplate);
                        lstEval_ConfigObTemplate.Add(clsEval_ConfigObTemplate);
                    }
                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                }
                else
                {
                    lstEval_ConfigObTemplate = new List<Eval_ConfigObTemplate>();
                    lstEval_ConfigObTemplate = Session["createEval_ConfigObTemplate"] as List<Eval_ConfigObTemplate>;
                }
                if (Session["createEval_ConfigObTemplateDetail"] == null)
                {
                    string strSQLEval_ConfigObTemplateDetail = @"select * from ""Eval_ConfigObTemplateDetail"" ";
                    DataTable dtEval_ConfigObTemplateDetail = General.IncarcaDT(strSQLEval_ConfigObTemplateDetail, null);
                    lstEval_ConfigObTemplateDetail = new List<Eval_ConfigObTemplateDetail>();
                    foreach (DataRow rwEvalConfigObTemplateDetail in dtEval_ConfigObTemplateDetail.Rows)
                    {
                        Eval_ConfigObTemplateDetail clsEval_ConfigObTemplateDetail = new Eval_ConfigObTemplateDetail(rwEvalConfigObTemplateDetail);
                        lstEval_ConfigObTemplateDetail.Add(clsEval_ConfigObTemplateDetail);
                    }
                    Session["createEval_ConfigObTemplateDetail"] = lstEval_ConfigObTemplateDetail;
                }
                else
                {
                    lstEval_ConfigObTemplateDetail = new List<Eval_ConfigObTemplateDetail>();
                    lstEval_ConfigObTemplateDetail = Session["createEval_ConfigObTemplateDetail"] as List<Eval_ConfigObTemplateDetail>;
                }
                if (Session["createEval_ConfigObiective"] == null)
                {
                    string strSQLEval_ConfigObiective = @"select * from ""Eval_ConfigObiective"" ORDER BY ""Ordine"" ";
                    DataTable dtEval_ConfigObiective = General.IncarcaDT(strSQLEval_ConfigObiective, null);
                    lstEval_ConfigObiective = new List<Eval_ConfigObiective>();
                    foreach (DataRow rwEval_ConfigObiective in dtEval_ConfigObiective.Rows)
                    {
                        Eval_ConfigObiective clsEval_ConfigObiective = new Eval_ConfigObiective(rwEval_ConfigObiective);
                        lstEval_ConfigObiective.Add(clsEval_ConfigObiective);
                    }
                    Session["createEval_ConfigObiective"] = lstEval_ConfigObiective;
                }
                else
                {
                    lstEval_ConfigObiective = new List<Eval_ConfigObiective>();
                    lstEval_ConfigObiective = Session["createEval_ConfigObiective"] as List<Eval_ConfigObiective>;
                }

                grDateObiective.KeyFieldName = "Id";
                grDateObiective.DataSource = lstEval_ConfigObTemplateDetail.Where(p => p.TemplateId == Convert.ToInt32(Session["editingObiectivTemplate"] ?? -99));
                grDateObiective.DataBind();
                #endregion

                #region configurare Competente

                if (Session["nomenVwEval_ConfigCompetenteCol"] == null)
                {
                    string strSQLEval_ConfigCompetenteCol = @"select * from ""vwEval_ConfigCompetenteCol"" ";
                    DataTable dtEval_ConfigCompetenteCol = General.IncarcaDT(strSQLEval_ConfigCompetenteCol, null);
                    lstVwEval_ConfigCompetenteCol = new List<vwEval_ConfigCompetenteCol>();
                    foreach (DataRow rwEval_ConfigCompetenteCol in dtEval_ConfigCompetenteCol.Rows)
                    {
                        vwEval_ConfigCompetenteCol clsCompetenteCol = new vwEval_ConfigCompetenteCol(rwEval_ConfigCompetenteCol);
                        lstVwEval_ConfigCompetenteCol.Add(clsCompetenteCol);
                    }
                    Session["nomenVwEval_ConfigCompetenteCol"] = lstVwEval_ConfigCompetenteCol;
                }
                else
                {
                    lstVwEval_ConfigCompetenteCol = new List<vwEval_ConfigCompetenteCol>();
                    lstVwEval_ConfigCompetenteCol = Session["nomenVwEval_ConfigCompetenteCol"] as List<vwEval_ConfigCompetenteCol>;
                }

                if (Session["createEval_ConfigCompTemplate"] == null)
                {
                    string strSQLEval_ConfigCompTemplate = @"select * from ""Eval_ConfigCompTemplate""";
                    DataTable dtEval_ConfigCompTemplate = General.IncarcaDT(strSQLEval_ConfigCompTemplate, null);
                    lstEval_ConfigCompTemplate = new List<Eval_ConfigCompTemplate>();
                    foreach (DataRow rwEval_ConfigCompTemplate in dtEval_ConfigCompTemplate.Rows)
                    {
                        Eval_ConfigCompTemplate clsEval_ConfigCompTemplate = new Eval_ConfigCompTemplate(rwEval_ConfigCompTemplate);
                        lstEval_ConfigCompTemplate.Add(clsEval_ConfigCompTemplate);
                    }
                    Session["createEval_ConfigCompTemplate"] = lstEval_ConfigCompTemplate;
                }
                else
                {
                    lstEval_ConfigCompTemplate = new List<Eval_ConfigCompTemplate>();
                    lstEval_ConfigCompTemplate = Session["createEval_ConfigCompTemplate"] as List<Eval_ConfigCompTemplate>;
                }
                if (Session["createEval_ConfigCompTemplateDetail"] == null)
                {
                    string strSQLEval_ConfigCompTemplateDetail = @"select * from ""Eval_ConfigCompTemplateDetail"" ";
                    DataTable dtEval_ConfigCompTemplateDetail = General.IncarcaDT(strSQLEval_ConfigCompTemplateDetail, null);
                    lstEval_ConfigCompTemplateDetail = new List<Eval_ConfigCompTemplateDetail>();
                    foreach (DataRow rwEvalConfigCompTemplateDetail in dtEval_ConfigCompTemplateDetail.Rows)
                    {
                        Eval_ConfigCompTemplateDetail clsEval_ConfigCompTemplateDetail = new Eval_ConfigCompTemplateDetail(rwEvalConfigCompTemplateDetail);
                        lstEval_ConfigCompTemplateDetail.Add(clsEval_ConfigCompTemplateDetail);
                    }
                    Session["createEval_ConfigCompTemplateDetail"] = lstEval_ConfigCompTemplateDetail;
                }
                else
                {
                    lstEval_ConfigCompTemplateDetail = new List<Eval_ConfigCompTemplateDetail>();
                    lstEval_ConfigCompTemplateDetail = Session["createEval_ConfigCompTemplateDetail"] as List<Eval_ConfigCompTemplateDetail>;
                }
                if (Session["createEval_ConfigCompetente"] == null)
                {
                    string strSQLEval_ConfigCompetente = @"select * from ""Eval_ConfigCompetente"" ";
                    DataTable dtEval_ConfigCompetente = General.IncarcaDT(strSQLEval_ConfigCompetente, null);
                    lstEval_ConfigCompetente = new List<Eval_ConfigCompetente>();
                    foreach (DataRow rwEval_ConfigCompetente in dtEval_ConfigCompetente.Rows)
                    {
                        Eval_ConfigCompetente clsEval_ConfigCompetente = new Eval_ConfigCompetente(rwEval_ConfigCompetente);
                        lstEval_ConfigCompetente.Add(clsEval_ConfigCompetente);
                    }
                    Session["createEval_ConfigCompetente"] = lstEval_ConfigCompetente;
                }
                else
                {
                    lstEval_ConfigCompetente = new List<Eval_ConfigCompetente>();
                    lstEval_ConfigCompetente = Session["createEval_ConfigCompetente"] as List<Eval_ConfigCompetente>;
                }

                grDateCompetente.KeyFieldName = "Id";
                grDateCompetente.DataSource = lstEval_ConfigCompTemplateDetail.Where(p => p.TemplateId == Convert.ToInt32(Session["editingCompetenteTemplate"] ?? -99));
                grDateCompetente.DataBind();
                #endregion


                lblGrup.Visible = false;
                cmbGrup.Visible = false;

                if (Session["isEditingObiectivTemplate"] == null || Session["isEditingObiectivTemplate"].ToString() == "0")
                {
                    partGridObiective.Visible = false;
                    lblPrelObi.Visible = false;
                    chkNomenclator.Visible = false;
                    chkObiective.Visible = false;
                    lblPerioadaObi.Visible = false;
                    cmbPerioadaObi.Visible = false;
                }
                else
                {
                    //Florin 2020.11.06 - s-a mutat in panel2_callback
                    //cmbTemplateObiective.DataSource = lstEval_ConfigObTemplate;
                    //cmbTemplateObiective.DataBind();

                    //int TemplateIdObiectiv = Convert.ToInt32(Session["editingObiectivTemplate"].ToString());
                    //cmbTemplateObiective.Value = TemplateIdObiectiv;
                }

                if (Session["isEditingCompetenteTemplate"] == null || Session["isEditingCompetenteTemplate"].ToString() == "0")
                {
                    partGridCompetente.Visible = false;
                    lblPrelComp.Visible = false;
                    chkNomenclatorComp.Visible = false;
                    chkCompetente.Visible = false;
                    lblPerioadaComp.Visible = false;
                    cmbPerioadaComp.Visible = false;
                }
                else
                {
                    //Florin 2020.11.06 - s-a mutat in panel2_callback
                    //cmbTemplateCompetente.DataSource = lstEval_ConfigCompTemplate;
                    //cmbTemplateCompetente.DataBind();

                    //int TemplateIdCompetente = Convert.ToInt32(Session["editingCompetenteTemplate"].ToString());
                    //cmbTemplateCompetente.Value = TemplateIdCompetente;
                }


                if (Session["Eval_QuizIntrebari_IdCategorieData"] != null)
                {
                    int idCategorieTipCamp = Convert.ToInt32(Session["Eval_QuizIntrebari_IdCategorieData"].ToString());
                    cmbTipObiect.DataSource = lstEval_tblTipCamp.Where(p => p.Parinte == idCategorieTipCamp && p.Activ == 1);
                    cmbTipObiect.DataBind();
                }

                if (!IsPostBack)
                {
                    //Florin 2020.11.12 - am scos + 1
                    //Session["TemplateIdObiectiv"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""TemplateId"",0)) FROM ""Eval_ConfigObTemplate"" "), 0)) + 1;
                    //Session["TemplateIdCompetenta"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""TemplateId"",0)) FROM ""Eval_ConfigCompTemplate"" "), 0)) + 1;
                    Session["TemplateIdObiectiv"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""TemplateId"",0)) FROM ""Eval_ConfigObTemplate"" "), 0));
                    Session["TemplateIdCompetenta"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""TemplateId"",0)) FROM ""Eval_ConfigCompTemplate"" "), 0));

                    Session["QuizIntrebari_Id"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""Id"",0)) FROM ""Eval_QuizIntrebari"" "), 0)) + 1;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if(!IsPostBack)
                {
                    GridViewDataComboBoxColumn colTipValoare = (grDateObiective.Columns["TipValoare"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colIdNomenclator = (grDateObiective.Columns["IdNomenclator"] as GridViewDataComboBoxColumn);
                    colTipValoare.PropertiesComboBox.DataSource = lstEval_DictionaryItem;
                    colIdNomenclator.PropertiesComboBox.DataSource = lstVwEval_ConfigObiectivCol;

                    //Florin 2020.01.03
                    DataTable dt = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Eval_tblCategorieObiective"" ", null);
                    cmbCategObi.DataSource = dt;
                    cmbCategObi.DataBind();
                    Session["Eval_tblCategorieObiective"] = dt;

                    //Florin 2020.11.13
                    DataTable dtTbl = General.IncarcaDT(@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE ""IdQuiz""=@1 ", new object[] { Session["IdEvalQuiz"] });
                    Session["Eval_ConfigTipTabela"] = dtTbl;
                }
                else
                {
                    lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
                    lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();
                    lstVwEval_ConfigCompetenteCol = new List<vwEval_ConfigCompetenteCol>();

                    lstEval_DictionaryItem = Session["nomenEval_DictionaryItem"] as List<Eval_DictionaryItem>;
                    lstVwEval_ConfigObiectivCol = Session["nomenVwEval_ConfigObiectivCol"] as List<vwEval_ConfigObiectivCol>;
                    lstVwEval_ConfigCompetenteCol = Session["nomenVwEval_ConfigCompetenteCol"] as List<vwEval_ConfigCompetenteCol>;

                    GridViewDataComboBoxColumn colTipValoare = (grDateObiective.Columns["TipValoare"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colIdNomenclator = (grDateObiective.Columns["IdNomenclator"] as GridViewDataComboBoxColumn);
                    colTipValoare.PropertiesComboBox.DataSource = lstEval_DictionaryItem;
                    colIdNomenclator.PropertiesComboBox.DataSource = lstVwEval_ConfigObiectivCol;
                    
                    //Florin 2020.01.03
                    DataTable dt = Session["Eval_tblCategorieObiective"] as DataTable;
                    cmbCategObi.DataSource = dt;
                    cmbCategObi.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtlQuiz_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            try
            {
                string[] param = e.Parameter.Split(';');
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;
                switch(param[0])
                {
                    case "txtDenumireQuiz":
                        ds.Tables[0].Rows[0]["Denumire"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "chkQuizActiv":
                        ds.Tables[0].Rows[0]["Activ"] = (param[1] == "true" ? 1 : 0);
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "dtDataInceput":
                        string[] data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["DataInceput"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "dtDataSfarsit":
                        string[] dataSf = param[1].Split('.');
                        ds.Tables[0].Rows[0]["DataSfarsit"] = new DateTime(Convert.ToInt32(dataSf[2]), Convert.ToInt32(dataSf[1]), Convert.ToInt32(dataSf[0]));
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "cmbPerioada":
                        ds.Tables[0].Rows[0]["Anul"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "chkLuatLaCunostinta":
                        ds.Tables[0].Rows[0]["LuatLaCunostinta"] = (param[1] == "true" ? 1 : 0);
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "txtNrZileLuatLaCunostinta":
                        ds.Tables[0].Rows[0]["NrZileLuatLaCunostinta"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "chkPreluareAutomataRaspunsuri":
                        ds.Tables[0].Rows[0]["Preluare"] = (param[1] == "true" ? 1 : 0);
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "txtTitlu":
                        ds.Tables[0].Rows[0]["TitluRTF"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "txtConditii":
                        ds.Tables[0].Rows[0]["ConditiiRTF"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "cmbIdRaport":
                        ds.Tables[0].Rows[0]["IdRaport"] = param[1];
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                    case "chkSinc":
                        ds.Tables[0].Rows[0]["Sincronizare"] = (param[1] == "true" ? 1 : 0);
                        Session["InformatiaCurentaEvalQuiz"] = ds;
                        break;
                }
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void panel2_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaEvalQuiz"] as DataSet;

                //Florin 2018.02.26 adaugat " si DeleteSectiune"
                #region DeleteIntrebare si DeleteSectiune
                if (hf1["Id"].ToString() == "-1")
                {
                    int IdQuizIntrebare = Convert.ToInt32(hf["Id"]);
                    if (!ds.Tables.Contains("Eval_QuizIntrebari"))
                        return;

                    DataTable dtIntrebari = ds.Tables["Eval_QuizIntrebari"] as DataTable;
                    //Florin 2018.02.26
                    DataRow rw = dtIntrebari.Rows.Find(IdQuizIntrebare);
                    //DataRow rw = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == IdQuizIntrebare).FirstOrDefault();
                    if (rw == null)
                        return;

                    //Florin 2018.02.26 Folosim aceeasi procedura pentru a sterge si o sectiune
                    //if (rw["EsteSectiune"].ToString() != "0")
                    //    return;

                    rw.Delete();

                    cmbTip.Value = null;
                    //cmbTipData.Value = null;
                    chkOrizontal.Checked = false;
                    chkVertical.Checked = false;
                    cmbSursaDate.Value = null;
                    chkObligatoriu.Checked = false;
                    txtDescIntrebare.Text = string.Empty;
                    //txtDexRatGlobal.Text = string.Empty;
                    //cmbPreluareDate.Value = null;
                    cmbTipObiect.Value = null;
                    //cmbDinNomenclator.Value = null;
                    partGridObiective.Visible = false;
                    partGridCompetente.Visible = false;
                    pnlConfigTipTabela.Visible = false;

                    lblPrelObi.Visible = false;
                    chkNomenclator.Visible = false;
                    chkObiective.Visible = false;
                    chkNomenclator.Checked = false;
                    chkObiective.Checked = false;
                    lblPerioadaObi.Visible = false;
                    cmbPerioadaObi.Visible = false;
                    cmbPerioadaObi.Value = null;

                    lblPrelComp.Visible = false;
                    chkNomenclatorComp.Visible = false;
                    chkCompetente.Visible = false;
                    chkNomenclatorComp.Checked = false;
                    chkCompetente.Checked = false;
                    lblPerioadaComp.Visible = false;
                    cmbPerioadaComp.Visible = false;
                    cmbPerioadaComp.Value = null;
                    cmbCategObi.Value = null;


                    grDateIntrebari.DataSource = ds.Tables["Eval_QuizIntrebari"];
                    grDateIntrebari.DataBind();

                    return;
                }
                #endregion

                #region AdaugaIntrebare
                if (hf1["Id"].ToString() == "1")
                //if (!string.IsNullOrEmpty(hf3.Value.ToString()) && string.IsNullOrEmpty(hf2.Value.ToString()) && string.IsNullOrEmpty(hf1.Value.ToString()) && string.IsNullOrEmpty(hf.Value.ToString()))
                {
                    if (!ds.Tables.Contains("Eval_QuizIntrebari"))
                        return;


                    //Florin 2018.03.06
                    //daca nivelul este 2 inseamna ca este selectata o sectiune si deci se adauga o noua intrebare
                    //daca este nivelul 3 inseamna ca este selectata o intrebare care se modifica


                    //int idSectie = Convert.ToInt32(hf["Id"].ToString());
                    //DataTable dtIntrebari = ds.Tables["Eval_QuizIntrebari"] as DataTable;

                    //DataRow rwDataSectie = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == idSectie).FirstOrDefault();
                    //if (rwDataSectie == null)
                    //    return;

                    //if (rwDataSectie["Parinte"].ToString() == "0")
                    //    return;

                    //DataRow rwRoot = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == Convert.ToInt32(rwDataSectie["Parinte"].ToString())).FirstOrDefault();
                    //if (rwRoot == null)
                    //    return;

                    //if (rwRoot["Parinte"].ToString() != "0")
                    //    return;

                    if (grDateIntrebari.FocusedNode == null || grDateIntrebari.FocusedNode.Level == 1) return;

                    int idSelectat = Convert.ToInt32(grDateIntrebari.FocusedNode.Key);
                    DataTable dtIntrebari = ds.Tables["Eval_QuizIntrebari"] as DataTable;

                    DataRow rowSelected = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == idSelectat).FirstOrDefault();
                    if (rowSelected == null)
                        return;

                    //se modifica descrierea intrebarii
                    if (grDateIntrebari.FocusedNode.Level == 3)
                    {
                        rowSelected["Descriere"] = txtDescIntrebare.Text;
                        Session["InformatiaCurentaEvalQuiz"] = ds;

                        if (cmbCategObi.Value != null)
                            rowSelected["IdCategObiective"] = Convert.ToInt32(cmbCategObi.Value);
                        else
                            rowSelected["IdCategObiective"] = DBNull.Value;

                        Session["InformatiaCurentaEvalQuiz"] = ds;

                        grDateIntrebari.DataSource = ds.Tables["Eval_QuizIntrebari"];
                        grDateIntrebari.DataBind();

                        GolesteCtl();
                        return;
                    }

                    int OrdineIntrebare = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Parinte") == Convert.ToInt32(rowSelected["Parinte"].ToString())).AsEnumerable().Select(row => row.Field<int>("OrdineInt")).Distinct().ToList().Max();

                    Session["QuizIntrebari_Id"] = Convert.ToInt32(Session["QuizIntrebari_Id"]) + 1;
                    int idNewIntrebare = Convert.ToInt32(Session["QuizIntrebari_Id"]);
                    DataRow rwNewSectiune = dtIntrebari.NewRow();
                    rwNewSectiune["Id"] = idNewIntrebare;
                    rwNewSectiune["Parinte"] = idSelectat;
                    rwNewSectiune["OrdineInt"] = OrdineIntrebare + 1;
                    rwNewSectiune["TIME"] = DateTime.Now;
                    rwNewSectiune["USER_NO"] = Session["UserId"];
                    rwNewSectiune["EsteSectiune"] = 0;
                    rwNewSectiune["IdQuiz"] = rowSelected["IdQuiz"] ?? DBNull.Value;
                    rwNewSectiune["Descriere"] = txtDescIntrebare.Text;
                    rwNewSectiune["TipData"] = cmbTipObiect.Value ?? DBNull.Value;
                    //rwNewSectiune["IdIntrebare"] = cmbDinNomenclator.Value ?? DBNull.Value;
                    rwNewSectiune["TipValoare"] = cmbSursaDate.Value ?? DBNull.Value;
                    rwNewSectiune["Orientare"] = chkOrizontal.Checked == true ? 1 : (chkVertical.Checked == true ? 2 : 0);
                    rwNewSectiune["Obligatoriu"] = chkObligatoriu.Checked == true ? 1 : 0;
                    //rwNewSectiune["DescriereInRatingGlobal"] = txtDexRatGlobal.Text;
                    rwNewSectiune["TemplateIdObiectiv"] = DBNull.Value;
                    rwNewSectiune["TemplateIdCompetenta"] = DBNull.Value;


                    //FLorin 2020.11.12 Begin
                    if (Convert.ToInt32(cmbTipObiect.Value.ToString()) == 23)
                    {
                        rwNewSectiune["TemplateIdObiectiv"] = Convert.ToInt32(General.Nz(Session["TemplateIdObiectiv"],1)) + 1;
                        Session["TemplateIdObiectiv"] = rwNewSectiune["TemplateIdObiectiv"];
                    }

                    if (Convert.ToInt32(cmbTipObiect.Value.ToString()) == 5)
                    {
                        rwNewSectiune["TemplateIdCompetenta"] = Convert.ToInt32(General.Nz(Session["TemplateIdCompetenta"], 1)) + 1;
                        Session["TemplateIdCompetenta"] = rwNewSectiune["TemplateIdCompetenta"];
                    }
                    //FLorin 2020.11.12 End


                    if (Session["isEditingObiectivTemplate"] != null && Session["isEditingObiectivTemplate"].ToString() == "1")
                    {
                        rwNewSectiune["IdPeriod"] = cmbPerioadaObi.Value ?? DBNull.Value;
                        rwNewSectiune["PreluareObiective"] = chkNomenclator.Checked == true ? 0 : (chkObiective.Checked == true ? 1 : 0);
                    }

                    if (Session["isEditingCompetenteTemplate"] != null && Session["isEditingCompetenteTemplate"].ToString() == "1")
                    {
                        rwNewSectiune["IdPeriodComp"] = cmbPerioadaComp.Value ?? DBNull.Value;
                        rwNewSectiune["PreluareCompetente"] = chkNomenclatorComp.Checked == true ? 0 : (chkCompetente.Checked == true ? 1 : 0);
                    }

                    //rwNewSectiune["TemplateId"] = -99;
                    ds.Tables["Eval_QuizIntrebari"].Rows.Add(rwNewSectiune);
                    Session["InformatiaCurentaEvalQuiz"] = ds;

                    grDateIntrebari.DataSource = ds.Tables["Eval_QuizIntrebari"];
                    grDateIntrebari.DataBind();

                    TreeListNode node = grDateIntrebari.FindNodeByKeyValue(idNewIntrebare.ToString());
                    if (node != null)
                        node.Focus();

                    GolesteCtl();
                    return;
                }
                #endregion

                #region Adauga Sectiune
                if (hf1["Id"].ToString() == "2")
                //if (!string.IsNullOrEmpty(hf2.Value)&&string.IsNullOrEmpty(hf.Value) && string.IsNullOrEmpty(hf1.Value) && string.IsNullOrEmpty(hf3.Value))
                {
                    if (!ds.Tables.Contains("Eval_QuizIntrebari"))
                        return;

                    int idIntrebare = Convert.ToInt32(hf["Id"].ToString());

                    //Florin 2018.02.22
                    //inseamna ca a selectata o sectiune existenta
                    if (grDateIntrebari.Nodes[0].Key != idIntrebare.ToString())
                    {
                        DataRow[] rw = ds.Tables["Eval_QuizIntrebari"].Select("Id =" + idIntrebare);
                        if (rw.Length <= 0) return;

                        DataRow rwIntrebare = rw[0];
                        rwIntrebare["Descriere"] = txtDenSectiune.Text;
                        Session["InformatiaCurentaEvalQuiz"] = ds;

                        grDateIntrebari.DataSource = ds.Tables["Eval_QuizIntrebari"];
                        grDateIntrebari.DataBind();

                        GolesteCtl();
                        return;
                    }


                    DataTable dtIntrebari = ds.Tables["Eval_QuizIntrebari"] as DataTable;
                    DataRow rwDataCurrent = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == idIntrebare).FirstOrDefault();
                    if (rwDataCurrent == null)
                        return;

                    if (rwDataCurrent["Parinte"].ToString() != "0")
                    {
                        MessageBox.Show("Nodul selectat nu reprezinta radacina chestionarului!", MessageBox.icoInfo);
                        return;
                    }
                    int OrdineIntrebare;
                    if (dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Parinte") == idIntrebare).AsEnumerable().Select(row => row.Field<int>("OrdineInt")).Count() == 0)
                        OrdineIntrebare = 1;
                    else
                        OrdineIntrebare = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Parinte") == idIntrebare).AsEnumerable().Select(row => row.Field<int>("OrdineInt")).Distinct().ToList().Max();

                    Session["QuizIntrebari_Id"] = Convert.ToInt32(Session["QuizIntrebari_Id"]) + 1;
                    int idNewIntrebare = Convert.ToInt32(Session["QuizIntrebari_Id"]);
                    DataRow rwNewSectiune = dtIntrebari.NewRow();
                    rwNewSectiune["Id"] = idNewIntrebare;
                    rwNewSectiune["Parinte"] = idIntrebare;
                    rwNewSectiune["OrdineInt"] = OrdineIntrebare + 1;
                    rwNewSectiune["TIME"] = DateTime.Now;
                    rwNewSectiune["USER_NO"] = Session["UserId"];
                    rwNewSectiune["EsteSectiune"] = 1;
                    rwNewSectiune["IdQuiz"] = rwDataCurrent["IdQuiz"];

                    //Florin 2018.02.22
                    rwNewSectiune["Descriere"] = txtDenSectiune.Value;

                    ds.Tables["Eval_QuizIntrebari"].Rows.Add(rwNewSectiune);
                    Session["InformatiaCurentaEvalQuiz"] = ds;

                    grDateIntrebari.DataSource = ds.Tables["Eval_QuizIntrebari"];
                    grDateIntrebari.DataBind();

                    TreeListNode node = grDateIntrebari.FindNodeByKeyValue(idNewIntrebare.ToString());
                    if (node != null)
                        node.Focus();

                    hf["Id"] = idNewIntrebare.ToString();
                    GolesteCtl();

                    return;
                }
                #endregion

                #region Asignare Date

                if (General.Nz(hf["Id"], "").ToString() != "" && String.IsNullOrEmpty(e.Parameter))
                {
                    int IdQuizIntrebare = Convert.ToInt32(hf["Id"]);
                    if (!ds.Tables.Contains("Eval_QuizIntrebari"))
                        return;

                    DataTable dtIntrebari = ds.Tables["Eval_QuizIntrebari"] as DataTable;
                    DataRow rwDataCurrent = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == IdQuizIntrebare).FirstOrDefault();
                    if (rwDataCurrent == null)
                        return;

                    GolesteCtl();

                    if (rwDataCurrent["EsteSectiune"].ToString() == "1")
                    {
                        //sectiune

                        //Florin 2018.02.22
                        if (rwDataCurrent["Descriere"].ToString().ToLower() != "root")
                        {
                            txtDenSectiune.Text = rwDataCurrent["Descriere"].ToString();
                            txtDenSectiune.DataBind();
                        }
                        partSectiune.Visible = true;
                        partIntrebari.Visible = true;
                        return;
                    }

                    //intrebare
                    txtDescIntrebare.Text = rwDataCurrent["Descriere"].ToString();
                    txtDescIntrebare.DataBind();
                    //txtDexRatGlobal.Text = rwDataCurrent["DescriereInRatingGlobal"].ToString();
                    //txtDexRatGlobal.DataBind();
                    partSectiune.Visible = true;
                    partIntrebari.Visible = true;

                    //Categorie Tip Camp & Tip Camp
                    if (!string.IsNullOrEmpty(rwDataCurrent["TipData"].ToString()))
                    {
                        lstEval_tblTipCamp = Session["nomenEval_tblTipCamp"] as List<metaEval_tblTipCamp>;
                        if (lstEval_tblTipCamp.Where(p => p.Id == Convert.ToInt32(rwDataCurrent["TipData"].ToString())).Count() != 0)
                        {
                            int IdCategorieTipCamp = lstEval_tblTipCamp.Where(p => p.Id == Convert.ToInt32(rwDataCurrent["TipData"].ToString())).FirstOrDefault().Parinte;
                            cmbTip.Value = IdCategorieTipCamp;
                            cmbTipObiect.DataSource = lstEval_tblTipCamp.Where(p => p.Parinte == IdCategorieTipCamp && p.Activ == 1);
                            cmbTipObiect.DataBind();
                            cmbTipObiect.Value = rwDataCurrent["TipData"];

                            #region configObiective
                            if (cmbTipObiect.Value !=null && Convert.ToInt32(cmbTipObiect.Value.ToString()) == 23)
                            {
                                partGridObiective.Visible = true;
                                Session["isEditingObiectivTemplate"] = 1;
                                Session["isEditingCompetenteTemplate"] = null;

                                int TemplateId = Convert.ToInt32(General.Nz(rwDataCurrent["TemplateIdObiectiv"], Session["TemplateIdObiectiv"]));
                                
                                rwDataCurrent["TemplateIdObiectiv"] = TemplateId;
                                ShowTemplateObiectiv(TemplateId);

                                lblPrelObi.Visible = true;
                                chkNomenclator.Visible = true;
                                chkObiective.Visible = true;
                                lblPerioadaObi.Visible = true; // chkObiective.Checked == true ? true : false;
                                cmbPerioadaObi.Visible = true; // chkObiective.Checked == true ? true : false;

                                //Florin 2020.11.06
                                cmbTemplateObiective.DataSource = lstEval_ConfigObTemplate;
                                cmbTemplateObiective.DataBind();
                                int TemplateIdObiectiv = Convert.ToInt32(Session["editingObiectivTemplate"].ToString());
                                cmbTemplateObiective.Value = TemplateIdObiectiv;
                            }
                            #endregion

                            #region configCompetente
                            if (cmbTipObiect.Value!=null && Convert.ToInt32(cmbTipObiect.Value.ToString()) == 5)
                            {
                                partGridCompetente.Visible = true;
                                Session["isEditingObiectivTemplate"] = null;
                                Session["isEditingCompetenteTemplate"] = 1;

                                int TemplateId = Convert.ToInt32(General.Nz(rwDataCurrent["TemplateIdCompetenta"], Session["TemplateIdCompetenta"]));
                                rwDataCurrent["TemplateIdCompetenta"] = TemplateId;
                                ShowTemplateCompetente(TemplateId);

                                lblPrelComp.Visible = true;
                                chkNomenclatorComp.Visible = true;
                                chkCompetente.Visible = true;
                                lblPerioadaComp.Visible = true; //chkCompetente.Checked == true ? true : false;
                                cmbPerioadaComp.Visible = true; //chkCompetente.Checked == true ? true : false;

                                //Florin 2020.11.06
                                cmbTemplateCompetente.DataSource = lstEval_ConfigCompTemplate;
                                cmbTemplateCompetente.DataBind();

                                int TemplateIdCompetente = Convert.ToInt32(Session["editingCompetenteTemplate"].ToString());
                                cmbTemplateCompetente.Value = TemplateIdCompetente;
                            }
                            #endregion

                            //FLorin 2020.11.12
                            if (cmbTipObiect.Value != null && Convert.ToInt32(cmbTipObiect.Value.ToString()) == 17)
                            {
                                pnlConfigTipTabela.Visible = true;
                                DataTable dtTbl = Session["Eval_ConfigTipTabela"] as DataTable;
                                DataTable dtFiltru = dtTbl.Select("IdQuiz = " + Convert.ToInt32(General.Nz(Session["IdEvalQuiz"], -99)) + " AND IdLInie = " + IdQuizIntrebare).CopyToDataTable();
                                grDateTabela.DataSource = dtFiltru;
                                //grDateTabela.DataSource = General.IncarcaDT($@"SELECT * FROM ""Eval_ConfigTipTabela"" WHERE ""IdQuiz"" = @1 AND ""IdLinie""=@2", new object[] { Session["IdEvalQuiz"], IdQuizIntrebare });
                                grDateTabela.DataBind();
                            }

                            Session["Eval_QuizIntrebari_IdCategorieData"] = IdCategorieTipCamp;

                            //Florin 2020.01.03 - daca tipul de control ales este obiectiv si exista inregistrari in tabela de categori de obiective, atunci afisam combobox-ul de categ obi
                            DataTable dtCtgObi = Session["Eval_tblCategorieObiective"] as DataTable;
                            if (dtCtgObi != null && dtCtgObi.Rows.Count > 0 && Convert.ToInt32(General.Nz(cmbTip.Value, -1)) == 4)
                            {
                                lblCategObi.ClientVisible = true;
                                cmbCategObi.ClientVisible = true;
                                if (Convert.ToInt32(General.Nz(rwDataCurrent["IdCategObiective"], -99)) != -99)
                                    cmbCategObi.Value = Convert.ToInt32(General.Nz(rwDataCurrent["IdCategObiective"], -1));
                            }
                            else
                            {
                                lblCategObi.ClientVisible = false;
                                cmbCategObi.ClientVisible = false;
                            }
                        }
                    }
                    else
                    {
                        cmbTip.Value = null;
                        cmbTipObiect.Value = null;
                    }

                    //Orientare
                    if (!string.IsNullOrEmpty(rwDataCurrent["Orientare"].ToString()))
                    {
                        int idOrientare = Convert.ToInt32(rwDataCurrent["Orientare"].ToString());
                        switch (idOrientare)
                        {
                            case 1:
                                chkOrizontal.Checked = true;
                                chkVertical.Checked = false;
                                break;
                            case 2:
                                chkVertical.Checked = true;
                                chkOrizontal.Checked = false;
                                break;
                        }
                    }
                    else
                    {
                        chkOrizontal.Checked = false;
                        chkVertical.Checked = false;
                    }

                    //Florin 2020.11.06
                    List<metaDate> tmpListaTblTipValori = Session["nomenEval_tblTipValori"] as List<metaDate>;
                    if (tmpListaTblTipValori.Count > 0 && Convert.ToInt32(General.Nz(cmbTip.Value, -1)) == 1 && Convert.ToInt32(General.Nz(cmbTipObiect.Value, -1)) == 2)
                    {
                        lblSursaDate.ClientVisible = true;
                        cmbSursaDate.ClientVisible = true;
                    }
                    else
                    {
                        lblSursaDate.ClientVisible = false;
                        cmbSursaDate.ClientVisible = false;
                    }

                    //Sursa date
                    if (!string.IsNullOrEmpty(rwDataCurrent["TipValoare"].ToString()))
                        cmbSursaDate.Value = Convert.ToInt32(rwDataCurrent["TipValoare"].ToString());
                    else
                        cmbSursaDate.Value = null;

                    //Obligatoriu
                    if (!string.IsNullOrEmpty(rwDataCurrent["Obligatoriu"].ToString()))
                        chkObligatoriu.Checked = Convert.ToInt32(rwDataCurrent["Obligatoriu"].ToString()) == 0 ? false : true;
                    else
                        chkObligatoriu.Checked = false;

                    //intrebare nomenclator
                    //if (!string.IsNullOrEmpty(rwDataCurrent["IdIntrebare"].ToString()))
                    //    cmbDinNomenclator.Value = Convert.ToInt32(rwDataCurrent["IdIntrebare"].ToString());
                    //else
                    //    cmbDinNomenclator.Value = null;

                    //afisare detalii sectie
                    if (rwDataCurrent["Parinte"].ToString() == "0")
                        return;


                    if (Session["isEditingObiectivTemplate"] != null && Session["isEditingObiectivTemplate"].ToString() == "1")
                    {
                        //Preluare obiective
                        if (!string.IsNullOrEmpty(rwDataCurrent["PreluareObiective"].ToString()))
                        {
                            int idPrelObi = Convert.ToInt32(rwDataCurrent["PreluareObiective"].ToString());
                            switch (idPrelObi)
                            {
                                case 0:
                                    chkNomenclator.Checked = true;
                                    chkObiective.Checked = false;
                                    break;
                                case 1:
                                    chkObiective.Checked = true;
                                    chkNomenclator.Checked = false;
                                    break;
                            }
                        }
                        else
                        {
                            chkNomenclator.Checked = false;
                            chkObiective.Checked = false;
                        }

                        //Perioada obiective
                        if (!string.IsNullOrEmpty(rwDataCurrent["IdPeriod"].ToString()))
                            cmbPerioadaObi.Value = Convert.ToInt32(rwDataCurrent["IdPeriod"].ToString());
                        else
                            cmbPerioadaObi.Value = null;
                    }

                    if (Session["isEditingCompetenteTemplate"] != null && Session["isEditingCompetenteTemplate"].ToString() == "1")
                    {
                        //Preluare competente
                        if (!string.IsNullOrEmpty(rwDataCurrent["PreluareCompetente"].ToString()))
                        {
                            int idPrelComp = Convert.ToInt32(rwDataCurrent["PreluareCompetente"].ToString());
                            switch (idPrelComp)
                            {
                                case 0:
                                    chkNomenclatorComp.Checked = true;
                                    chkCompetente.Checked = false;
                                    break;
                                case 1:
                                    chkCompetente.Checked = true;
                                    chkNomenclatorComp.Checked = false;
                                    break;
                            }
                        }
                        else
                        {
                            chkNomenclatorComp.Checked = false;
                            chkCompetente.Checked = false;
                        }

                        //Perioada Competente
                        if (!string.IsNullOrEmpty(rwDataCurrent["IdPeriodComp"].ToString()))
                            cmbPerioadaComp.Value = Convert.ToInt32(rwDataCurrent["IdPeriodComp"].ToString());
                        else
                            cmbPerioadaComp.Value = null;
                    }


                    DataRow rwDataSectiune = dtIntrebari.AsEnumerable().Where(row => row.Field<Int32>("Id") == Convert.ToInt32(rwDataCurrent["Parinte"])).FirstOrDefault();
                    if (rwDataSectiune == null)
                        return;

                    txtDenSectiune.Text = rwDataSectiune["Descriere"].ToString();
                    int TipValoare = Convert.ToInt32((rwDataCurrent["TipData"].ToString() == string.Empty ? "-99" : rwDataCurrent["TipData"]) ?? -99);
                    metaEval_tblTipCamp cls = lstEval_tblTipCamp.Where(p => p.Id == TipValoare).FirstOrDefault();
                    if (cls != null)
                    {
                        cmbTip.Value = cls.Parinte;
                    }
                    return;
                }

                //Florin 2020.11.06 - este pusa de 2 ori (este si mai sus din cauza return-ului de deasupra)
                //Florin 2020.11.06- daca tipul de control ales este obiectiv si exista inregistrari in tabela de categori de obiective, atunci afisam combobox-ul de categ obi
                DataTable dtCtgObi2 = Session["Eval_tblCategorieObiective"] as DataTable;
                if (dtCtgObi2 != null && dtCtgObi2.Rows.Count > 0 && Convert.ToInt32(General.Nz(cmbTip.Value, -1)) == 4)
                {
                    lblCategObi.ClientVisible = true;
                    cmbCategObi.ClientVisible = true;
                }
                else
                {
                    lblCategObi.ClientVisible = false;
                    cmbCategObi.ClientVisible = false;
                }

                //Florin 2020.11.06 - este pusa de 2 ori (este si mai sus din cauza return-ului de deasupra)
                List<metaDate> tmpListaTblTipValori2 = Session["nomenEval_tblTipValori"] as List<metaDate>;
                if (tmpListaTblTipValori2.Count > 0 && Convert.ToInt32(General.Nz(cmbTip.Value, -1)) == 1 && Convert.ToInt32(General.Nz(cmbTipObiect.Value, -1)) == 2)
                {
                    lblSursaDate.ClientVisible = true;
                    cmbSursaDate.ClientVisible = true;
                }
                else
                {
                    lblSursaDate.ClientVisible = false;
                    cmbSursaDate.ClientVisible = false;
                }

                #endregion

                #region Salvare Date

                int idQuizIntrebare;

                string str = e.Parameter;
                if (string.IsNullOrEmpty(str))
                    return;

                string[] arr = e.Parameter.Split(';');
                if (arr.Length == 3 || arr[0] != "" || arr[1] != "" || arr[2] != "")
                {
                    idQuizIntrebare = Convert.ToInt32(arr[2]);
                    if (!ds.Tables.Contains("Eval_QuizIntrebari"))
                        return;

                    DataRow[] rw = ds.Tables["Eval_QuizIntrebari"].Select("Id =" + idQuizIntrebare);
                    if (rw.Length <= 0)
                        return;

                    DataRow rwIntrebare = rw[0];
                    if (rwIntrebare["Id"].ToString() != idQuizIntrebare.ToString())
                        return;

                    #region EditTemplateObiective

                    string controlName = arr[0].Substring(arr[0].LastIndexOf('_') + 1);
                    if (controlName == "cmbTemplateObiective" || controlName == "txtMinObiective" || controlName == "txtMaxObiective" || controlName == "chkCanAddObiective"
                        || controlName == "chkCanDeleteObiective" || controlName == "chkCanEditObiective")
                    {
                        Eval_ConfigObTemplate clsTemplateObiective = lstEval_ConfigObTemplate.Where(p => p.TemplateId ==
                                                                            Convert.ToInt32(Session["editingObiectivTemplate"].ToString())).FirstOrDefault();
                        switch (controlName)
                        {
                            case "cmbTemplateObiective":
                                int TemplateId = Convert.ToInt32(arr[1]);
                                rwIntrebare["TemplateIdObiectiv"] = TemplateId;
                                ShowTemplateObiectiv(TemplateId);
                                break;
                            case "txtMinObiective":
                                if (clsTemplateObiective != null)
                                {
                                    clsTemplateObiective.NrMinObiective = Convert.ToInt32(arr[1].ToString() == string.Empty ? "0" : txtMinObiective.Text);
                                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                                }
                                break;
                            case "txtMaxObiective":
                                if (clsTemplateObiective != null)
                                {
                                    clsTemplateObiective.NrMaxObiective = Convert.ToInt32(arr[1].ToString() == string.Empty ? "0" : txtMaxObiective.Text);
                                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                                }
                                break;
                            case "chkCanAddObiective":
                                if (clsTemplateObiective != null)
                                {
                                    clsTemplateObiective.PoateAdauga = arr[1].ToString() == "true" ? 1 : 0;
                                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                                }
                                break;
                            case "chkCanDeleteObiective":
                                if (clsTemplateObiective != null)
                                {
                                    clsTemplateObiective.PoateSterge = arr[1].ToString() == "true" ? 1 : 0;
                                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                                }
                                break;
                            case "chkCanEditObiective":
                                if (clsTemplateObiective != null)
                                {
                                    clsTemplateObiective.PoateModifica = arr[1].ToString() == "true" ? 1 : 0;
                                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                                }
                                break;
                        }
                        Session["isEditingObiectivTemplate"] = 1;
                        Session["isEditingCompetenteTemplate"] = null;
                        partGridObiective.Visible = true;
                        lblPrelObi.Visible = true;
                        chkNomenclator.Visible = true;
                        chkObiective.Visible = true;
                        lblPerioadaObi.Visible = true; //chkObiective.Checked == true ? true : false;
                        cmbPerioadaObi.Visible = true; // chkObiective.Checked == true ? true : false;
                        return;
                    }
                    #endregion

                    #region EditTemplateCompetente

                    string controlNameCompetente = arr[0].Substring(arr[0].LastIndexOf('_') + 1);
                    if (controlNameCompetente == "cmbTemplateCompetente")
                    {
                        Eval_ConfigCompTemplate clsTemplateCompetente = lstEval_ConfigCompTemplate.Where(p => p.TemplateId ==
                                                                            Convert.ToInt32(Session["editingCompetenteTemplate"].ToString())).FirstOrDefault();
                        switch (controlName)
                        {
                            case "cmbTemplateCompetente":
                                int TemplateId = Convert.ToInt32(arr[1]);
                                rwIntrebare["TemplateIdCompetenta"] = TemplateId;
                                ShowTemplateCompetente(TemplateId);
                                break;
                        }
                        Session["isEditingObiectivTemplate"] = null;
                        Session["isEditingCompetenteTemplate"] = 1;
                        partGridCompetente.Visible = true;
                        lblPrelComp.Visible = true;
                        lblPrelComp.Text = "Preluare competente:";
                        chkNomenclatorComp.Visible = true;
                        chkCompetente.Visible = true;
                        chkCompetente.Text = "Competente";
                        lblPerioadaComp.Visible = true; // chkCompetente.Checked == true ? true : false;
                        cmbPerioadaComp.Visible = true; // chkCompetente.Checked == true ? true : false;
                        return;
                    }
                    #endregion

                    if (Session["isEditingObiectivTemplate"] == null || Session["isEditingObiectivTemplate"].ToString() == "0")
                    {
                        partGridObiective.Visible = false;
                        lblPrelObi.Visible = false;
                        chkNomenclator.Visible = false;
                        chkObiective.Visible = false;
                        lblPerioadaObi.Visible = false;
                        cmbPerioadaObi.Visible = false;
                    }

                    if (Session["isEditingCompetenteTemplate"] == null || Session["isEditingCompetenteTemplate"].ToString() == "0")
                    {
                        partGridCompetente.Visible = false;
                        lblPrelComp.Visible = false;
                        chkNomenclatorComp.Visible = false;
                        chkCompetente.Visible = false;
                        lblPerioadaComp.Visible = false;
                        cmbPerioadaComp.Visible = false;
                    }

                    switch (arr[0].Substring(arr[0].LastIndexOf('_') + 1))
                    {
                        case "cmbTip":
                            int idCategorieTipCamp = Convert.ToInt32(arr[1]);
                            lstEval_tblTipCamp = Session["nomenEval_tblTipCamp"] as List<metaEval_tblTipCamp>;
                            cmbTipObiect.DataSource = lstEval_tblTipCamp.Where(p => p.Parinte == idCategorieTipCamp && p.Activ == 1);
                            cmbTipObiect.DataBind();

                            Session["Eval_QuizIntrebari_IdCategorieData"] = idCategorieTipCamp;

                            if (lstEval_tblTipCamp.Count == 0)
                                return;
                            cmbTipObiect.SelectedIndex = -1;
                            
                            #region obiective
                            if (idCategorieTipCamp != 4)
                            {
                                partGridObiective.Visible = false;
                                lblPrelObi.Visible = false;
                                chkNomenclator.Visible = false;
                                chkObiective.Visible = false;
                                lblPerioadaObi.Visible = false;
                                cmbPerioadaObi.Visible =  false;
                                Session["isEditingObiectivTemplate"] = 0;
                                rwIntrebare["TemplateIdObiectiv"] = DBNull.Value;
                            }
                            int IdTipObiect = -99;
                            if (Int32.TryParse(cmbTipObiect.Value.ToString(), out IdTipObiect))
                            {
                                partGridObiective.Visible = IdTipObiect == 23 ? true : false;
                                Session["isEditingObiectivTemplate"] = IdTipObiect == 23 ? 1 : 0;

                                lblPrelObi.Visible = IdTipObiect == 23 ? true : false;
                                chkNomenclator.Visible = IdTipObiect == 23 ? true : false;
                                chkObiective.Visible = IdTipObiect == 23 ? true : false;
                                lblPerioadaObi.Visible = IdTipObiect == 23 ? true : false;// chkObiective.Checked == true ? true : false;
                                cmbPerioadaObi.Visible = IdTipObiect == 23 ? true : false; //chkObiective.Checked == true ? true : false;

                                int TemplateId = Convert.ToInt32(General.Nz(rwIntrebare["TemplateIdObiectiv"], Session["TemplateIdObiectiv"]));
                                rwIntrebare["TemplateIdObiectiv"] = IdTipObiect == 23 ? TemplateId : -99;
                                if (IdTipObiect == 23)
                                {
                                    ShowTemplateObiectiv(TemplateId);
                                    cmbTemplateObiective.DataSource = lstEval_ConfigObTemplate;
                                    cmbTemplateObiective.DataBind();
                                    cmbTemplateObiective.Value = TemplateId;
                                }
                            }
                            #endregion

                            #region competente
                            if (idCategorieTipCamp != 3)
                            {
                                partGridCompetente.Visible = false;
                                lblPrelComp.Visible = false;
                                chkNomenclatorComp.Visible = false;
                                chkCompetente.Visible = false;
                                lblPerioadaComp.Visible = false;
                                cmbPerioadaComp.Visible = false;
                                Session["isEditingCompetenteTemplate"] = 0;
                                rwIntrebare["TemplateIdCompetenta"] = DBNull.Value;
                            }
                            int IdTipObiectCompetente = -99;
                            if (Int32.TryParse(cmbTipObiect.Value.ToString(), out IdTipObiectCompetente))
                            {
                                partGridCompetente.Visible = IdTipObiectCompetente == 5 ? true : false;
                                Session["isEditingCompetenteTemplate"] = IdTipObiectCompetente == 5 ? 1 : 0;
                                chkNomenclatorComp.Visible = IdTipObiectCompetente == 5 ? true : false;
                                chkCompetente.Visible = IdTipObiectCompetente == 5 ? true : false;
                                lblPerioadaComp.Visible = IdTipObiectCompetente == 5 ? true : false; //chkCompetente.Checked == true ? true : false;
                                cmbPerioadaComp.Visible = IdTipObiectCompetente == 5 ? true : false; //chkCompetente.Checked == true ? true : false;

                                int TemplateId = Convert.ToInt32(General.Nz(rwIntrebare["TemplateIdCompetenta"], Session["TemplateIdCompetenta"]));
                                rwIntrebare["TemplateIdCompetenta"] = IdTipObiectCompetente == 5 ? TemplateId : -99;
                                ShowTemplateCompetente(TemplateId);
                                cmbTemplateCompetente.DataSource = lstEval_ConfigCompTemplate;
                                cmbTemplateCompetente.DataBind();
                                cmbTemplateCompetente.Value = TemplateId;
                            }
                            #endregion

                            break;
                        case "cmbTipObiect":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            int TipValoare = Convert.ToInt32(arr[1]);
                            rwIntrebare["TipData"] = TipValoare;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            #region obiective
                            if (TipValoare == 23)
                            {
                                partGridObiective.Visible = true;
                                lblPrelObi.Visible = true;
                                chkNomenclator.Visible = true;
                                chkObiective.Visible = true;
                                lblPerioadaObi.Visible = true; // chkObiective.Checked == true ? true : false;
                                cmbPerioadaObi.Visible = true; // chkObiective.Checked == true ? true : false;
                                Session["isEditingObiectivTemplate"] = 1;
                                Session["isEditingCompetenteTemplate"] = null;

                                int TemplateId = Convert.ToInt32(General.Nz(rwIntrebare["TemplateIdObiectiv"], Session["TemplateIdObiectiv"]));
                                rwIntrebare["TemplateIdObiectiv"] = TemplateId;
                                ShowTemplateObiectiv(TemplateId);
                                cmbTemplateObiective.DataSource = lstEval_ConfigObTemplate;
                                cmbTemplateObiective.DataBind();
                                cmbTemplateObiective.Value = TemplateId;
                            }
                            #endregion

                            #region competente
                            if (TipValoare == 5)
                            {
                                partGridCompetente.Visible = true;
                                lblPrelComp.Visible = true;
                                lblPrelComp.Text = "Preluare competente:";
                                chkNomenclatorComp.Visible = true;
                                chkCompetente.Visible = true;
                                chkObiective.Text = "Competente";
                                lblPerioadaComp.Visible = true; // chkCompetente.Checked == true ? true : false;
                                cmbPerioadaComp.Visible = true; // chkCompetente.Checked == true ? true : false;
                                Session["isEditingObiectivTemplate"] = null;
                                Session["isEditingCompetenteTemplate"] = 1;

                                int TemplateId = Convert.ToInt32(General.Nz(rwIntrebare["TemplateIdCompetenta"], Session["TemplateIdCompetenta"]));
                                rwIntrebare["TemplateIdCompetenta"] = TemplateId;
                                ShowTemplateCompetente(TemplateId);
                                cmbTemplateCompetente.DataSource = lstEval_ConfigCompTemplate;
                                cmbTemplateCompetente.DataBind();
                                cmbTemplateCompetente.Value = TemplateId;
                            }
                            #endregion
                            break;
                        case "cmbDinNomenclator":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            int IdIntrebareNomen = Convert.ToInt32(arr[1]);
                            rwIntrebare["IdIntrebare"] = IdIntrebareNomen;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "cmbSursaDate":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            int idSursaDate = Convert.ToInt32(arr[1]);
                            rwIntrebare["TipValoare"] = idSursaDate;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "chkObligatoriu":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["Obligatoriu"] = arr[1] == "true" ? 1 : 0;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "chkOrizontal":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["Orientare"] = arr[1] == "true" ? 1 : 2;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "chkVertical":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["Orientare"] = arr[1] == "true" ? 2 : 1;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "txtDescIntrebare":
                            //Florin 2018.03.06
                            //acest proces se realizeaza in salvare intrebare

                            //if (rwIntrebare["EsteSectiune"].ToString() == "1")
                            //    return;
                            //rwIntrebare["Descriere"] = arr[1].ToString();
                            //Session["InformatiaCurentaEvalQuiz"] = ds;

                            break;
                        case "txtDexRatGlobal":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["Descriere"] = arr[1].ToString();
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "txtDenSectiune":
                            {
                                //Florin 2018.03.06
                                //acest proces se realizeaza in salvare sectiune

                                //if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                //{
                                //    //Florin 2018.02.23
                                //    if (grDateIntrebari.Nodes[0].Key != idQuizIntrebare.ToString())
                                //    {
                                //        rwIntrebare["Descriere"] = arr[1].ToString();
                                //        Session["InformatiaCurentaEvalQuiz"] = ds;
                                //    }
                                //    return;
                                //}

                                //DataRow rwDataSectiune = ds.Tables["Eval_QuizIntrebari"].AsEnumerable().Where(row => row.Field<Int32>("Id") == Convert.ToInt32(rwIntrebare["Parinte"])).FirstOrDefault();
                                //if (rwDataSectiune == null)
                                //    return;
                                //rwDataSectiune["Descriere"] = arr[1].ToString();
                                //Session["InformatiaCurentaEvalQuiz"] = ds;
                            }

                            break;

                        case "chkNomenclator":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["PreluareObiective"] = arr[1] == "true" ? 0 : 1;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            //if (chkObiective.Checked == true)
                            //{
                            //    lblPerioadaObi.Visible = true;
                            //    cmbPerioadaObi.Visible = true;
                            //    cmbPerioadaObi.Value = (rwIntrebare["IdPeriod"] == null || rwIntrebare["IdPeriod"].ToString().Length <= 0) ? -1 : Convert.ToInt32(rwIntrebare["IdPeriod"].ToString());
                            //}
                            //else
                            //{
                            //    lblPerioadaObi.Visible = false;
                            //    cmbPerioadaObi.Visible = false;
                            //}
                            break;
                        case "chkNomenclatorComp":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["PreluareCompetente"] = arr[1] == "true" ? 0 : 1;

                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            //if (chkCompetente.Checked == true)
                            //{
                            //    lblPerioadaComp.Visible = true;
                            //    cmbPerioadaComp.Visible = true;
                            //    cmbPerioadaComp.Value = (rwIntrebare["IdPeriodComp"] == null || rwIntrebare["IdPeriodComp"].ToString().Length <= 0) ? -1 : Convert.ToInt32(rwIntrebare["IdPeriodComp"].ToString());
                            //}
                            //else
                            //{
                            //    lblPerioadaComp.Visible = false;
                            //    cmbPerioadaComp.Visible = false;
                            //}
                            break;
                        case "chkObiective":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["PreluareObiective"] = arr[1] == "true" ? 1 : 0;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            //if (arr[1] == "true")
                            //{
                            //    lblPerioadaObi.Visible = true;
                            //    cmbPerioadaObi.Visible = true;
                            //    cmbPerioadaObi.Value = rwIntrebare["IdPeriod"] == null ? -1 : Convert.ToInt32(rwIntrebare["IdPeriod"].ToString());
                            //}
                            //else
                            //{
                            //    lblPerioadaObi.Visible = false;
                            //    cmbPerioadaObi.Visible = false;
                            //}
                            break;
                        case "chkCompetente":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            rwIntrebare["PreluareCompetente"] = arr[1] == "true" ? 1 : 0;

                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            //if (arr[1] == "true")
                            //{
                            //    lblPerioadaComp.Visible = true;
                            //    cmbPerioadaComp.Visible = true;
                            //    cmbPerioadaComp.Value = (rwIntrebare["IdPeriodComp"] == null || rwIntrebare["IdPeriodComp"].ToString().Length <= 0) ? -1 : Convert.ToInt32(rwIntrebare["IdPeriodComp"].ToString());
                            //}
                            //else
                            //{
                            //    lblPerioadaComp.Visible = false;
                            //    cmbPerioadaComp.Visible = false;
                            //}
                            break;
                        case "cmbPerioadaObi":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            int idPeriod = Convert.ToInt32(arr[1]);
                            rwIntrebare["IdPeriod"] = idPeriod;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                        case "cmbPerioadaComp":
                            if (rwIntrebare["EsteSectiune"].ToString() == "1")
                                return;
                            int idPeriodComp = Convert.ToInt32(arr[1]);
                            rwIntrebare["IdPeriodComp"] = idPeriodComp;
                            Session["InformatiaCurentaEvalQuiz"] = ds;
                            break;
                    }

                    return;

                }
                #endregion

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void GolesteCtl()
        {
            try
            {
                txtDenSectiune.Text = string.Empty;
                cmbTip.Value = null;
                //cmbTipData.Value = null;
                chkOrizontal.Checked = false;
                chkVertical.Checked = false;
                cmbSursaDate.Value = null;
                chkObligatoriu.Checked = false;
                txtDescIntrebare.Text = string.Empty;
                //txtDexRatGlobal.Text = string.Empty;
                //cmbPreluareDate.Value = null;
                cmbTipObiect.Value = null;
                //cmbDinNomenclator.Value = null;
                partGridObiective.Visible = false;
                partGridCompetente.Visible = false;
                pnlConfigTipTabela.Visible = false;

                lblPrelObi.Visible = false;
                chkNomenclator.Visible = false;
                chkObiective.Visible = false;
                chkNomenclator.Checked = false;
                chkObiective.Checked = false;
                lblPerioadaObi.Visible = false;
                cmbPerioadaObi.Visible = false;
                cmbPerioadaObi.Value = null;

                lblPrelComp.Visible = false;
                chkNomenclatorComp.Visible = false;
                chkCompetente.Visible = false;
                chkNomenclatorComp.Checked = false;
                chkCompetente.Checked = false;
                lblPerioadaComp.Visible = false;
                cmbPerioadaComp.Visible = false;
                cmbPerioadaComp.Value = null;

                //Florin 2020.01.03
                cmbCategObi.Value = null;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        #region Obiective
        protected void grDateObiective_AutoFilterCellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
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

        protected void grDateObiective_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TipValoare")
                    e.Editor.ClientInstanceName = "TipValoareEditor";
                if (e.Column.FieldName != "IdNomenclator")
                    return;
                var editor = (ASPxComboBox)e.Editor;
                editor.ClientInstanceName = "IdNomenclatorEditor";
                editor.ClientSideEvents.EndCallback = "cmbNomenclator_EndCallBack";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        
        protected void grDateObiective_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                int Id = Convert.ToInt32(e.Keys[0].ToString());
                lstEval_ConfigObTemplateDetail = Session["createEval_ConfigObTemplateDetail"] as List<Eval_ConfigObTemplateDetail>;
                int TemplateId = Convert.ToInt32(Session["editingObiectivTemplate"]);
                foreach(Eval_ConfigObTemplateDetail row in lstEval_ConfigObTemplateDetail.Where(p=>p.Id == Id && p.TemplateId == TemplateId))
                {
                    row.Citire = Convert.ToBoolean(e.NewValues["Citire"] ?? DBNull.Value);
                    row.Obligatoriu = Convert.ToBoolean(e.NewValues["Obligatoriu"] ?? DBNull.Value);
                    row.Vizibil = Convert.ToBoolean(e.NewValues["Vizibil"] ?? DBNull.Value);
                    row.IdNomenclator = Convert.ToInt32(e.NewValues["IdNomenclator"] ?? -99);
                    row.TipValoare = Convert.ToInt32(e.NewValues["TipValoare"] ?? -99);
                    row.Editare = Convert.ToBoolean(e.NewValues["Editare"] ?? DBNull.Value);
                    row.Width = Convert.ToInt32(e.NewValues["Width"] ?? DBNull.Value);
                    if (e.NewValues["Ordine"] != null)
                        row.Ordine = Convert.ToInt32(e.NewValues["Ordine"]);
                    else
                        row.Ordine = null;
                    row.FormulaSql = e.NewValues["FormulaSql"] != null ? e.NewValues["FormulaSql"].ToString() : null;
                    row.Alias = e.NewValues["Alias"] != null ? e.NewValues["Alias"].ToString() : null;
                    if (e.NewValues["TotalColoana"] != null)
                        row.TotalColoana = Convert.ToInt32(e.NewValues["TotalColoana"]);
                    else
                        row.TotalColoana = null;
                }

                Session["createEval_ConfigObTemplateDetail"] = lstEval_ConfigObTemplateDetail;
                grDateObiective.DataSource = lstEval_ConfigObTemplateDetail.Where(p => p.TemplateId == TemplateId);
                e.Cancel = true;
                grDateObiective.CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateObiective_CustomErrorText(object sender, DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs e)
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

        protected void Unnamed_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
        {
            try
            {
                int id;
                if (e.Value == null || !int.TryParse(e.Value.ToString(), out id))
                    return;
                ASPxComboBox editor = source as ASPxComboBox;
                //var query = lstActivitati.Where(p => p.ParentId == id);
                //editor.DataSource = query.ToList();
                //editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Unnamed_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                ASPxComboBox editor = source as ASPxComboBox;
                List<vwEval_ConfigObiectivCol> query;
                var take = e.EndIndex - e.BeginIndex + 1;
                var skip = e.BeginIndex;
                int tipValoare = GetCurrentTipValoare();
                string colName = GetCurrentColNameObiectiv();
                if (tipValoare > -1)
                    query = lstVwEval_ConfigObiectivCol.Where(p => p.TipValoare.Contains(e.Filter) && p.IdTipValoare == tipValoare && p.ColumnName == colName).OrderBy(p => p.CodNomenclator).ToList<vwEval_ConfigObiectivCol>();
                else
                    query = lstVwEval_ConfigObiectivCol.Where(p => p.TipValoare.Contains(e.Filter) && p.ColumnName==colName).OrderBy(p => p.CodNomenclator).ToList<vwEval_ConfigObiectivCol>();

                editor.DataSource = query;
                editor.TextField = "DenNomenclator";
                editor.ValueField = "IdNomenclator";
                editor.ValueType = typeof(Int32);
                editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private int GetCurrentTipValoare()
        {
            object id = null;
            if (hfObiectiv.TryGet("CurrentTipLista", out id))
                return Convert.ToInt32(id);
            return -1;
            //object val = grDateObiective.GetRowValues(0, "Id;ColumnName");
        }

        private string GetCurrentColNameObiectiv()
        {
            object colName = null;
            if (hfObiectivColName.TryGet("CurrentColumnNameObiectiv", out colName))
                return colName.ToString();
            return "";
        }


        private void ShowTemplateObiectiv(int TemplateIdObiectiv)
        {
            try
            {
                Session["isEditingObiectivTemplate"] = 1;
                Session["isEditingcompetenteTemplate"] = null;
                Session["editingObiectivTemplate"] = TemplateIdObiectiv;
                bool newTemplate = false;
                Eval_ConfigObTemplate clsObTemplate = lstEval_ConfigObTemplate.Where(p => p.TemplateId == TemplateIdObiectiv).FirstOrDefault();
                if (clsObTemplate == null)
                    newTemplate = true;

                bool newTemplateDetail = false;
                Eval_ConfigObTemplateDetail clsObTemplateDetail = lstEval_ConfigObTemplateDetail.Where(p => p.TemplateId == TemplateIdObiectiv).FirstOrDefault();
                if (clsObTemplateDetail == null)
                    newTemplateDetail = true;

                if (newTemplate)
                {
                    Eval_ConfigObTemplate clsNewTemplateObiectiv = new Eval_ConfigObTemplate();
                    clsNewTemplateObiectiv.TemplateId = TemplateIdObiectiv;
                    clsNewTemplateObiectiv.TemplateName = "Template " + TemplateIdObiectiv.ToString();
                    clsNewTemplateObiectiv.NrMaxObiective = 900;
                    clsNewTemplateObiectiv.NrMinObiective = 0;
                    clsNewTemplateObiectiv.PoateAdauga = 0;
                    clsNewTemplateObiectiv.PoateSterge = 0;
                    clsNewTemplateObiectiv.PoateModifica = 0;
                    lstEval_ConfigObTemplate.Add(clsNewTemplateObiectiv);
                    Session["createEval_ConfigObTemplate"] = lstEval_ConfigObTemplate;
                }
                if(newTemplateDetail||newTemplate)
                { 
                    foreach(Eval_ConfigObiective rwObiectiv in lstEval_ConfigObiective)
                    {
                        Eval_ConfigObTemplateDetail clsTemplateDetail = new Eval_ConfigObTemplateDetail();
                        //clsTemplateDetail.Id = rwObiectiv.Id;
                        clsTemplateDetail.Id = rwObiectiv.Ordine;   //Radu 10.05.2019
                        clsTemplateDetail.TemplateId = TemplateIdObiectiv;
                        clsTemplateDetail.ColumnName = rwObiectiv.ColumnName;
                        clsTemplateDetail.Width = 100;
                        clsTemplateDetail.Obligatoriu = false;
                        clsTemplateDetail.Citire = false;
                        clsTemplateDetail.Editare = false;
                        clsTemplateDetail.Vizibil = false;
                        clsTemplateDetail.TipValoare = -99;
                        clsTemplateDetail.IdNomenclator = -99;
                        lstEval_ConfigObTemplateDetail.Add(clsTemplateDetail);
                    }
                    Session["createEval_ConfigObTemplateDetail"] = lstEval_ConfigObTemplateDetail;
                }

                foreach (Eval_ConfigObTemplate clsTemplate in lstEval_ConfigObTemplate.Where(p => p.TemplateId == TemplateIdObiectiv))
                {
                    txtMaxObiective.Text = clsTemplate.NrMaxObiective.ToString();
                    txtMinObiective.Text = clsTemplate.NrMinObiective.ToString();
                    chkCanAddObiective.Checked = clsTemplate.PoateAdauga == 0 ? false : true;
                    chkCanDeleteObiective.Checked = clsTemplate.PoateSterge == 0 ? false : true;
                    chkCanEditObiective.Checked = clsTemplate.PoateModifica == 0 ? false : true;
                }

                grDateObiective.KeyFieldName = "Id";
                grDateObiective.DataSource = lstEval_ConfigObTemplateDetail.Where(p => p.TemplateId == TemplateIdObiectiv);
                grDateObiective.DataBind();

                lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
                lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();

                lstEval_DictionaryItem = Session["nomenEval_DictionaryItem"] as List<Eval_DictionaryItem>;
                lstVwEval_ConfigObiectivCol = Session["nomenVwEval_ConfigObiectivCol"] as List<vwEval_ConfigObiectivCol>;

                GridViewDataComboBoxColumn colTipValoare = (grDateObiective.Columns["TipValoare"] as GridViewDataComboBoxColumn);
                GridViewDataComboBoxColumn colIdNomenclator = (grDateObiective.Columns["IdNomenclator"] as GridViewDataComboBoxColumn);
                colTipValoare.PropertiesComboBox.DataSource = lstEval_DictionaryItem;
                colIdNomenclator.PropertiesComboBox.DataSource = lstVwEval_ConfigObiectivCol;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        #endregion

        #region Competente
        protected void grDateCompetente_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
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

        protected void grDateCompetente_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                //if (e.Column.FieldName == "TipValoare")
                //    e.Editor.ClientInstanceName = "TipValoareEditor";
                if (e.Column.FieldName != "IdNomenclator")
                    return;
                var editor = (ASPxComboBox)e.Editor;
                editor.ClientInstanceName = "IdNomenclatorCompetenteEditor";
                
                editor.ClientSideEvents.EndCallback = "cmbNomenclatorCompetente_EndCallBack";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCompetente_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                int Id = Convert.ToInt32(e.Keys[0].ToString());
                lstEval_ConfigCompTemplateDetail = Session["createEval_ConfigCompTemplateDetail"] as List<Eval_ConfigCompTemplateDetail>;
                int TemplateId = Convert.ToInt32(Session["editingCompetenteTemplate"]);
                foreach (Eval_ConfigCompTemplateDetail row in lstEval_ConfigCompTemplateDetail.Where(p => p.Id == Id && p.TemplateId == TemplateId))
                {
                    row.Citire = Convert.ToBoolean(e.NewValues["Citire"] ?? DBNull.Value);
                    row.Obligatoriu = Convert.ToBoolean(e.NewValues["Obligatoriu"] ?? DBNull.Value);
                    row.Vizibil = Convert.ToBoolean(e.NewValues["Vizibil"] ?? DBNull.Value);
                    row.Editare = Convert.ToBoolean(e.NewValues["Editare"] ?? DBNull.Value);
                    row.IdNomenclator = Convert.ToInt32(e.NewValues["IdNomenclator"] ?? -99);
                    row.Width = Convert.ToInt32(e.NewValues["Width"] ?? DBNull.Value);
                    if (e.NewValues["Ordine"] != null)
                        row.Ordine = Convert.ToInt32(e.NewValues["Ordine"]);
                    else
                        row.Ordine = null;
                    row.FormulaSql = e.NewValues["FormulaSql"] != null ? e.NewValues["FormulaSql"].ToString() : null;
                    row.Alias = e.NewValues["Alias"] != null ? e.NewValues["Alias"].ToString() : null;
                    if (e.NewValues["TotalColoana"] != null)
                        row.TotalColoana = Convert.ToInt32(e.NewValues["TotalColoana"]);
                    else
                        row.TotalColoana = null;
                }

                Session["createEval_ConfigCompTemplateDetail"] = lstEval_ConfigCompTemplateDetail;
                grDateCompetente.DataSource = lstEval_ConfigCompTemplateDetail.Where(p => p.TemplateId == TemplateId);
                e.Cancel = true;
                grDateCompetente.CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateCompetente_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
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

        protected void Unnamed_ItemRequestedByValue1(object source, ListEditItemRequestedByValueEventArgs e)
        {
            try
            {
                int id;
                if (e.Value == null || !int.TryParse(e.Value.ToString(), out id))
                    return;
                ASPxComboBox editor = source as ASPxComboBox;
                //var query = lstActivitati.Where(p => p.ParentId == id);
                //editor.DataSource = query.ToList();
                editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Unnamed_ItemsRequestedByFilterCondition1(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                ASPxComboBox editor = source as ASPxComboBox;
                List<vwEval_ConfigCompetenteCol> query;
                var take = e.EndIndex - e.BeginIndex + 1;
                var skip = e.BeginIndex;
                string colName = GetCurrentColNameCompetente();
                query = lstVwEval_ConfigCompetenteCol.Where(p => p.ColumnName == colName).OrderBy(p => p.CodNomenclator).ToList<vwEval_ConfigCompetenteCol>();
                editor.DataSource = query;
                editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private string GetCurrentColNameCompetente()
        {
            object colName = null;
            if (hfCompetenteColName.TryGet("CurrentColumnNameCompetente", out colName))
                return colName.ToString();
            return "";
        }

        private void ShowTemplateCompetente(int TemplateIdCompetente)
        {
            try
            {
                Session["isEditingObiectivTemplate"] = null;
                Session["isEditingcompetenteTemplate"] = 1;
                Session["editingCompetenteTemplate"] = TemplateIdCompetente;
                bool newTemplate = false;
                Eval_ConfigCompTemplate clsCompTemplate = lstEval_ConfigCompTemplate.Where(p => p.TemplateId == TemplateIdCompetente).FirstOrDefault();
                if (clsCompTemplate == null)
                    newTemplate = true;

                if (newTemplate)
                {
                    Eval_ConfigCompTemplate clsNewTemplateCompetente = new Eval_ConfigCompTemplate();
                    clsNewTemplateCompetente.TemplateId = TemplateIdCompetente;
                    clsNewTemplateCompetente.TemplateName = "Template " + TemplateIdCompetente.ToString();
                    lstEval_ConfigCompTemplate.Add(clsNewTemplateCompetente);
                    Session["createEval_ConfigCompTemplate"] = lstEval_ConfigCompTemplate;

                    foreach (Eval_ConfigCompetente rwCompetente in lstEval_ConfigCompetente)
                    {
                        Eval_ConfigCompTemplateDetail clsTemplateDetail = new Eval_ConfigCompTemplateDetail();
                        clsTemplateDetail.Id = rwCompetente.Id;
                        clsTemplateDetail.TemplateId = TemplateIdCompetente;
                        clsTemplateDetail.ColumnName = rwCompetente.ColumnName;
                        clsTemplateDetail.Width = 100;
                        clsTemplateDetail.Obligatoriu = false;
                        clsTemplateDetail.Citire = false;
                        clsTemplateDetail.Editare = false;
                        clsTemplateDetail.Vizibil = false;
                        clsTemplateDetail.IdNomenclator = -99;
                        lstEval_ConfigCompTemplateDetail.Add(clsTemplateDetail);
                    }
                    Session["createEval_ConfigCompTemplateDetail"] = lstEval_ConfigCompTemplateDetail;
                }

                foreach (Eval_ConfigCompTemplate clsTemplate in lstEval_ConfigCompTemplate.Where(p => p.TemplateId == TemplateIdCompetente))
                {
                    
                }

                grDateCompetente.KeyFieldName = "Id";
                grDateCompetente.DataSource = lstEval_ConfigCompTemplateDetail.Where(p => p.TemplateId == TemplateIdCompetente);
                grDateCompetente.DataBind();

                //lstEval_DictionaryItem = new List<Eval_DictionaryItem>();
                //lstVwEval_ConfigObiectivCol = new List<vwEval_ConfigObiectivCol>();

                //lstEval_DictionaryItem = Session["nomenEval_DictionaryItem"] as List<Eval_DictionaryItem>;
                //lstVwEval_ConfigObiectivCol = Session["nomenVwEval_ConfigObiectivCol"] as List<vwEval_ConfigObiectivCol>;

                //GridViewDataComboBoxColumn colTipValoare = (grDateObiective.Columns["TipValoare"] as GridViewDataComboBoxColumn);
                //GridViewDataComboBoxColumn colIdNomenclator = (grDateObiective.Columns["IdNomenclator"] as GridViewDataComboBoxColumn);
                //colTipValoare.PropertiesComboBox.DataSource = lstEval_DictionaryItem;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #endregion

        protected void grDateTabela_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow dr = dt.NewRow();

                dr["IdQuiz"] = Convert.ToInt32(General.Nz(Session["IdEvalQuiz"], -99));
                dr["IdLinie"] = Convert.ToInt32(hf["Id"]);
                dr["Coloana"] = e.NewValues["Coloana"];
                dr["Lungime"] = e.NewValues["Lungime"];
                dr["Alias"] = e.NewValues["Alias"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
                e.Cancel = true;
                grDateTabela.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                dr["Coloana"] = e.NewValues["Coloana"];
                dr["Lungime"] = e.NewValues["Lungime"];
                dr["Alias"] = e.NewValues["Alias"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                e.Cancel = true;
                grDateTabela.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["Eval_ConfigTipTabela"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDateTabela.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateTabela_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["IdLinie"] = Convert.ToInt32(General.Nz(hf["Id"], 1));
                e.NewValues["IdQuiz"] = Convert.ToInt32(General.Nz(Session["IdEvalQuiz"], -99));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}
