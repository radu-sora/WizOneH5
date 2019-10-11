using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Eval;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class MainPage : System.Web.UI.Page
    {


        public class metaBadge
        {
            public string StringSelect { get; set; }
            public string Pagina { get; set; }
            public string Eticheta { get; set; }
            public string RutaImg { get; set; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                //2017.05.25 - Nu preia corect valoarea in variabila de sesiune - intotdeauna este MainPage si profilele din Cadru.Master nu se mai incarca
                //Session["PaginaWeb"] = "Pagini.MainPage";

                //Radu 09.01.2018
                Session["Avs_MarcaFiltru"] = null;
                Session["Avs_AtributFiltru"] = null;

                if (General.Nz(Session["PrimaIntrare"],"0").ToString() == "1")
                {
                    //pnlMsgWelcome.Visible = true;
                    pnlMsgWelcome.Style["display"] = "inline-block";
                    General.ExecutaNonQuery("UPDATE USERS SET F70105=0 WHERE F70102=@1", new object[] { Session["UserId"] });
                }

                #region ATENTIE !!!

                //Bucata de cod aproape identica cu cea din IntroDetaliu -> Page_Load 

                //adaugam campurile din view
                System.Data.DataTable dt = General.IncarcaDT(@"SELECT * FROM ""IntroView"" WHERE ""IdUser""=@1", new string[] { Session["UserId"].ToString() });

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        Label lbl = new Label();
                        lbl.Text = Dami.TraduCuvant((dt.Rows[0][i] ?? "").ToString());

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgView" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = Dami.TraduCuvant(dt.Columns[i].ColumnName);
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = false;
                        pnl.AllowResize = false;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowCloseButton = false;
                        pnl.Controls.Add(lbl);

                        divPanel.Controls.Add(pnl);

                        //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = dt.Columns[i].ColumnName, RutaImg = "pnlWidget1" });
                    }
                }

                //adaugam stirile
                string strSql = @"SELECT A.""Continut""
                                FROM ""Stiri"" A
                                INNER JOIN ""relGrupStire"" B ON A.""Id"" = B.""IdStire""
                                INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                WHERE A.""Activ"" = 1 AND A.""DataInceput"" <= {1} AND {1} <= A.""DataSfarsit"" AND C.""IdUser"" = {0} AND ""IdLimba""='{2}'
                                GROUP BY A.""Continut"" ";
                if (Constante.tipBD == 2)
                    strSql = @"SELECT TO_CHAR(A.""Continut"") AS ""Continut""
                                FROM ""Stiri"" A
                                INNER JOIN ""relGrupStire"" B ON A.""Id"" = B.""IdStire""
                                INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                WHERE A.""Activ"" = 1 AND A.""DataInceput"" <= {1} AND {1} <= A.""DataSfarsit"" AND C.""IdUser"" = {0} AND ""IdLimba""='{2}'
                                GROUP BY TO_CHAR(A.""Continut"") ";								

                if (Constante.tipBD == 1)
                    strSql = string.Format(strSql, Session["UserId"], "GetDate()", General.Nz(Session["IdLimba"], "RO"));
                else
                    strSql = string.Format(strSql, Session["UserId"], "SYSDATE", General.Nz(Session["IdLimba"], "RO"));

                DataTable dtSt = General.IncarcaDT(strSql, null);

                HtmlGenericControl divStiri = new HtmlGenericControl("div");

                foreach (DataRow dr in dtSt.Rows)
                {
                    divStiri.InnerHtml += dr["Continut"].ToString();
                }

                {
                    ASPxDockPanel pnl = new ASPxDockPanel();
                    string nme = "wdgStiri0";
                    pnl.ID = nme;
                    pnl.PanelUID = nme;
                    pnl.ClientInstanceName = nme;
                    pnl.HeaderText = " ";
                    pnl.DragElement = DragElement.Window;
                    pnl.OwnerZoneUID = "LeftZone";
                    pnl.CssClass = "cssDockPanel";
                    pnl.Styles.Header.CssClass = "cssHeader";
                    pnl.AllowDragging = false;
                    pnl.AllowResize = false;
                    pnl.ShowOnPageLoad = false;
                    pnl.ShowCloseButton = false;
                    pnl.ShowHeader = false;
                    pnl.ScrollBars = ScrollBars.Vertical;
                    pnl.Controls.Add(divStiri);

                    divPanel.Controls.Add(pnl);

                    //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = "Stiri", RutaImg = "pnlWidget2" });
                }

                string strFiltru = "";
                //strFiltru = @"AND A.""Rol"" IN (SELECT TOP 1 A.""IdSuper"" FROM ""F100Supervizori"" A WHERE A.""IdUser"" = 2 GROUP BY A.""IdSuper"")";
                //if (Constante.tipBD == 2) strFiltru = @"AND A.""Rol"" IN (SELECT A.""IdSuper"" FROM ""F100Supervizori"" A WHERE A.""IdUser"" = 2 AND ROWNUM <= 1 GROUP BY A.""IdSuper"")";

                //adaugam badge-urile
                List <metaBadge> lstBadges = new List<metaBadge>();
                //pontajul echipei
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajEchipa.aspx", Eticheta = "Pontaj Echipa", RutaImg = "bdgPtj.jpg" });
                //cereri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCereri() + $@" AND A.""Actiune"" = 1 AND A.""IdStare"" IN (1,2) " + strFiltru, Pagina = "../Absente/Lista.aspx", Eticheta = "Cereri", RutaImg = "bdgCer.jpg" });
                //evaluare
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx", Eticheta = "Evaluari", RutaImg = "bdgEvl.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=2", Eticheta = "Pontaj pe zi", RutaImg = "bdgPtj.jpg" });
                //pontaj pe angajat
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=1", Eticheta = "Pontaj pe angajat", RutaImg = "bdgPtj.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajOne.aspx", Eticheta = "Pontajul meu", RutaImg = "bdgPtj.jpg" });

                //Invitatie 360
                Invitatie pagInv = new Invitatie();
                lstBadges.Add(new metaBadge { StringSelect = pagInv.CreazaSelect(Convert.ToInt32(General.Nz(Session["User_Marca"], -99)), Convert.ToInt32(General.Nz(Session["UserId"], -99))), Pagina = "../Eval/Invitatie.aspx", Eticheta = "Feedback", RutaImg = "bdgEvl.jpg" });

                ////pontaj
                ////lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajPeAng.aspx?tip=1", Eticheta = "Pontaj pe angajat", RutaImg = "bdgPtj.jpg" });
                //lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=1", Eticheta = "Pontaj pe angajat", RutaImg = "bdgPtj.jpg" });
                ////cereri
                //lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCereri() + $@" AND A.""Actiune"" = 1 AND A.""IdStare"" IN (1,2) " + strFiltru, Pagina = "../Absente/Lista.aspx?pp=1", Eticheta = "Cereri", RutaImg = "bdgCer.jpg" });
                ////evaluare
                //lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx", Eticheta = "Evaluari", RutaImg = "bdgEvl.jpg" });
                ////decont
                //lstBadges.Add(new metaBadge { StringSelect = Dami.SelectDecont(), Pagina = "WebForm1.aspx", Eticheta = "Decont", RutaImg = "bdgDec.jpg" });
                ////Cursuri
                //lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCurs(), Pagina = "WebForm1.aspx", Eticheta = "Cursuri", RutaImg = "bdgCrs.jpg" });

                int j = 0;

                foreach (var ele in lstBadges)
                {
                    string str = @"<div onclick=""window.location.href='{1}'"" >
                                        <img src = ""../Fisiere/Imagini/{3}"" alt = """" />
                                        <span class='badge'>{0}</span>
                                    </div>
                                    <h3>{2}</h3>";
                    object nr = General.ExecutaScalar("SELECT COUNT(*) FROM (" + ele.StringSelect + ") X", null);
                    str = string.Format(str, General.Nz(nr,"0").ToString(), ele.Pagina, Dami.TraduCuvant(ele.Eticheta), ele.RutaImg);

                    HtmlGenericControl divBadge = new HtmlGenericControl("div");
                    divBadge.Attributes["class"] = "badgeContainer";
                    divBadge.InnerHtml = str;

                    ASPxDockPanel pnl = new ASPxDockPanel();
                    string nme = "wdgBadges" + j;
                    pnl.ID = nme;
                    pnl.PanelUID = nme;
                    pnl.ClientInstanceName = nme;
                    pnl.HeaderText = " ";
                    pnl.DragElement = DragElement.Window;
                    pnl.OwnerZoneUID = "LeftZone";
                    pnl.CssClass = "cssDockPanel";
                    pnl.Styles.Header.CssClass = "cssHeader";
                    pnl.AllowDragging = false;
                    pnl.AllowResize = false;
                    pnl.ShowOnPageLoad = false;
                    pnl.ShowCloseButton = false;
                    pnl.ShowHeader = false;
                    pnl.Controls.Add(divBadge);

                    divPanel.Controls.Add(pnl);

                    j++;
                    //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = ele.Eticheta, RutaImg = "pnlWidget3" });

                }


                //adaugam rapoartele
                //DataTable dtRap = General.IncarcaDT(@"SELECT * FROM ""Rap_Rapoarte"" WHERE COALESCE(""Activ"",0)=1", null);
                DataTable dtRap = General.IncarcaDT(@"SELECT * FROM ""DynReports""", null);

                if (dtRap.Rows.Count > 0)
                {
                    //Radu 09.10.2019
                    ASPxCallbackPanel pnlMain = new ASPxCallbackPanel();
                    pnlMain.Callback += new CallbackEventHandlerBase(pnlMain_OnCallback);                    
                    pnlMain.ClientInstanceName = "pnlMain";

                    for (int i = 0; i < dtRap.Rows.Count; i++)
                    {
                        ASPxButton btn = new ASPxButton();
                        btn.Text = Dami.TraduCuvant((dtRap.Rows[i]["Name"] ?? "").ToString());
                        //btn.PostBackUrl = "RapDetaliu.aspx?id=" + dtRap.Rows[i]["DynReportId"];
                        //Radu 09.10.2019 - Id-ul raportului nu poate fi stocat in Session["ReportId"] in acest moment (deoarece ramane ultimul din iteratie); 
                        //                  la apasarea butonului se va apela o functie care se seta corect Session["ReportId"]
                        //Session["ReportId"] = dtRap.Rows[i]["DynReportId"];
                        btn.AutoPostBack = true;
                        //btn.PostBackUrl = "../Generatoare/Reports/Pages/ReportView.aspx";
                        btn.Click += new EventHandler(btnRap_Click); 
                        //btn.ClientSideEvents.Click = string.Format("function(s,e) {{ pnlMain.PerformCallback('{0}');  window.open(getAbsoluteUrl + 'Generatoare/Reports/Pages/ReportView.aspx', '_blank '); }}", dtRap.Rows[i]["DynReportId"].ToString());
                        btn.ClientSideEvents.Click = string.Format("function(s,e) {{ pnlMain.PerformCallback('{0}'); e.processOnServer = true; }}", dtRap.Rows[i]["DynReportId"].ToString());

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgRap" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = "";
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel_noBorder";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = false;
                        pnl.AllowResize = false;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowShadow = false;
                        pnl.ShowHeader = false;
                        pnl.Styles.Content.Paddings.Padding = 0;
                        pnl.Controls.Add(btn);

                        //divPanel.Controls.Add(pnl);
                        pnlMain.Controls.Add(pnl);

                        //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = btn.Text, RutaImg = "pnlWidget4" });
                    }
                    divPanel.Controls.Add(pnlMain);
                }


                //adaugam meniuri
                DataTable dtMnu = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" WHERE COALESCE(""Stare"",0)=1", null);

                if (dtMnu.Rows.Count > 0)
                {
                    for (int i = 0; i < dtMnu.Rows.Count; i++)
                    {
                        //Florin 2019.09.09
                        //am schimbat linkurile in butoane

                        //ASPxHyperLink lnk = new ASPxHyperLink();
                        //lnk.Text = Dami.TraduMeniu((dtMnu.Rows[i]["Nume"] ?? "").ToString());
                        //lnk.Font.Underline = true;

                        ASPxButton btn = new ASPxButton();
                        btn.Text = Dami.TraduCuvant((dtMnu.Rows[i]["Nume"] ?? "").ToString());
                        btn.CssClass = "btnMeniuDash";

                        string strUrl = dtMnu.Rows[i]["Pagina"].ToString();
                        string pag = strUrl;

                        if (strUrl.IndexOf("[") >= 0)
                            pag = strUrl.Substring(0, strUrl.IndexOf("["));

                        //lnk.NavigateUrl = "~/" + pag + ".aspx";
                        btn.PostBackUrl = "~/" + pag + ".aspx";

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgMnu" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = "";
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel_noBorder";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = false;
                        pnl.AllowResize = false;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowShadow = false;
                        pnl.ShowHeader = false;
                        pnl.Styles.Content.Paddings.Padding = 0;
                        //pnl.Controls.Add(lnk);
                        pnl.Controls.Add(btn);

                        divPanel.Controls.Add(pnl);

                        //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = lnk.Text, RutaImg = "pnlWidget5" });
                    }
                }


                //adaugam link-uri custom
                DataTable dtLnk = General.IncarcaDT(@"SELECT * FROM ""IntroLink""  ", null);

                if (dtLnk.Rows.Count > 0)
                {
                    for (int i = 0; i < dtLnk.Rows.Count; i++)
                    {
                        ASPxHyperLink lnk = new ASPxHyperLink();
                        lnk.Text = Dami.TraduMeniu((dtLnk.Rows[i]["Denumire"] ?? "").ToString());
                        lnk.Font.Underline = true;
                        lnk.NavigateUrl = (dtLnk.Rows[i]["Link"] ?? "").ToString();
                        lnk.Target = "_blank";
                        lnk.Wrap = DevExpress.Utils.DefaultBoolean.True;

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgLnk" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = "";
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel_noBorder";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = false;
                        pnl.AllowResize = false;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowShadow = false;
                        pnl.ShowHeader = false;
                        pnl.Styles.Content.Paddings.Padding = 0;
                        pnl.Controls.Add(lnk);

                        divPanel.Controls.Add(pnl);

                        //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = lnk.Text, RutaImg = "pnlWidget5" });
                    }
                }



                //var src = widgetNames.Where(p => p.Nume.Contains("wdgBadges"));
                //lst.DataSource = src;
                //lst.DataBind();



                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 09.10.2019
        void pnlMain_OnCallback(object source, CallbackEventArgsBase e)
        {
            Session["ReportId"] = Convert.ToInt32(e.Parameter.ToString());       
        }
        void btnRap_Click(object sender, EventArgs e)
        {        
            Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx", false);
        }



        protected void dockManager_ClientLayout(object sender, DevExpress.Web.ASPxClientLayoutArgs e)
        {
            try
            {
                if (e.LayoutMode == ClientLayoutMode.Loading)
                {
                    string idUser = General.Nz(Session["UserId"], -99).ToString() == "" ? "-99" : General.Nz(Session["UserId"], -99).ToString();

                    string strSql = $@"SELECT TOP 1 A.""Continut"" FROM ""Intro"" A
                                    INNER JOIN ""relGrupIntro"" B ON A.""Id""=B.""IdIntro""
                                    INNER JOIN ""relGrupUser"" C ON B.""IdGrup""=C.""IdGrup""
                                    INNER JOIN ""tblGrupUsers"" D ON C.""IdGrup""=D.""Id""
                                    WHERE ""Activ""=1 AND C.""IdUser""={idUser}
                                    GROUP BY A.""Continut"", A.TIME, D.""Prioritate""
                                    ORDER BY D.""Prioritate"" DESC, A.TIME DESC";
                    if (Constante.tipBD == 2)
                        strSql = $@"SELECT * FROM (
                                    SELECT A.""Continut"" FROM ""Intro"" A
                                    INNER JOIN ""relGrupIntro"" B ON A.""Id"" = B.""IdIntro""
                                    INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                    INNER JOIN ""tblGrupUsers"" D ON C.""IdGrup"" = D.""Id""
                                    WHERE ""Activ"" = 1 AND C.""IdUser"" = {idUser}
                                    ORDER BY D.""Prioritate"" DESC, A.TIME DESC) X
                                    WHERE ROWNUM <= 1";

                    //strSql = string.Format(strSql, General.Nz(Session["UserId"],-99));

                    DataTable dt = General.IncarcaDT(strSql, null);
                    if (dt.Rows.Count > 0 && dt.Rows[0]["Continut"] != null) e.LayoutData = dt.Rows[0]["Continut"].ToString();
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