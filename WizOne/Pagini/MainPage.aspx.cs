using DevExpress.Web;
using ProceseSec;
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

                //Radu 09.01.2018
                Session["Avs_MarcaFiltru"] = null;
                Session["Avs_AtributFiltru"] = null;

                if (General.Nz(Session["PrimaIntrare"],"0").ToString() == "1")
                {
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
                }

                string strFiltru = "";

                //adaugam badge-urile
                List <metaBadge> lstBadges = new List<metaBadge>();
                //pontajul echipei
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajEchipa.aspx", Eticheta = Dami.TraduCuvant("Pontaj Echipa"), RutaImg = "bdgPtj.jpg" });
                //cereri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCereri() + $@" AND A.""Actiune"" = 1 AND A.""IdStare"" IN (1,2) " + strFiltru, Pagina = "../Absente/Lista.aspx", Eticheta = Dami.TraduCuvant("Cereri"), RutaImg = "bdgCer.jpg" });
                //Florin 2019.11.01
                //evaluare - filtrare chestionare nefinalizate si care nu sunt 360 sau proiect
                lstBadges.Add(new metaBadge { StringSelect = "SELECT * FROM (" + Dami.SelectEvaluare() + ") X WHERE Stare NOT LIKE '%finalizat%' AND CategorieQuiz = 0", Pagina = "../Eval/EvalLista.aspx?q=12", Eticheta = Dami.TraduCuvant("Evaluari"), RutaImg = "bdgEvl.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=2", Eticheta = Dami.TraduCuvant("Pontaj pe zi"), RutaImg = "bdgPtj.jpg" });
                //pontaj pe angajat
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=1", Eticheta = Dami.TraduCuvant("Pontaj pe angajat"), RutaImg = "bdgPtj.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajOne.aspx", Eticheta = Dami.TraduCuvant("Pontajul meu"), RutaImg = "bdgPtj.jpg" });

                //Invitatie 360
                Invitatie pagInv = new Invitatie();
                lstBadges.Add(new metaBadge { StringSelect = pagInv.CreazaSelect(Convert.ToInt32(General.Nz(Session["User_Marca"], -99)), Convert.ToInt32(General.Nz(Session["UserId"], -99))), Pagina = "../Eval/Invitatie.aspx", Eticheta = Dami.TraduCuvant("Feedback"), RutaImg = "bdgEvl.jpg" });

                //Florin 2019.10.15
                //Solicitari absente
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../Absente/Cereri.aspx", Eticheta = Dami.TraduCuvant("Solicitari absente"), RutaImg = "bdgCer.jpg" });
                //Solicitari diverse
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../CereriDiverse/Cereri.aspx", Eticheta = Dami.TraduCuvant("Solicitari diverse"), RutaImg = "bdgCer.jpg" });

                //Florin 2019.10.23
                //evaluare - filtrare questionare 360 sau proiect si care trebuie completate
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx?q=34", Eticheta = Dami.TraduCuvant("Evaluari 360"), RutaImg = "bdgEvl.jpg" });


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
                }


                //adaugam rapoartele
                DataTable dtRap = General.IncarcaDT(@"SELECT * FROM ""DynReports""", null);

                if (dtRap.Rows.Count > 0)
                {
                    for (int i = 0; i < dtRap.Rows.Count; i++)
                    {
                        ASPxButton btn = new ASPxButton();
                        btn.ID = "btn_Rap_" + dtRap.Rows[i]["DynReportId"];
                        btn.Text = Dami.TraduCuvant(General.Nz(dtRap.Rows[i]["Name"],"").ToString());
                        btn.UseSubmitBehavior = false;
                        if (Convert.ToInt32(General.Nz(dtRap.Rows[i]["HasPassword"], 0)) == 1)
                        {
                            btn.ClientSideEvents.Click = "function(s, e){ OnClickRapButton(s); }";
                            btn.AutoPostBack = false;
                        }
                        else
                            btn.PostBackUrl = "../Generatoare/Reports/Pages/ReportView.aspx?q=" + General.URLEncode("IdRaportDyn=" + dtRap.Rows[i]["DynReportId"] + "&Angajat=" + General.Nz(Session["User_Marca"], -99).ToString());

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

                        divPanel.Controls.Add(pnl);
                    }
                }


                //adaugam meniuri
                DataTable dtMnu = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" WHERE COALESCE(""Stare"",0)=1", null);

                if (dtMnu.Rows.Count > 0)
                {
                    for (int i = 0; i < dtMnu.Rows.Count; i++)
                    {
                        ASPxButton btn = new ASPxButton();
                        btn.Text = Dami.TraduCuvant((dtMnu.Rows[i]["Nume"] ?? "").ToString());
                        btn.CssClass = "btnMeniuDash";

                        string strUrl = dtMnu.Rows[i]["Pagina"].ToString();
                        string pag = strUrl;

                        if (strUrl.IndexOf("[") >= 0)
                            pag = strUrl.Substring(0, strUrl.IndexOf("["));

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

                        pnl.Controls.Add(btn);

                        divPanel.Controls.Add(pnl);
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
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
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

        protected void popUpPass_WindowCallback(object source, PopupWindowCallbackArgs e)
        {
            try
            {
                if (txtRapPass.Text.Trim() != "")
                {
                    string numeRap = "";
                    if (hfRap.Contains("NumeRap"))
                        numeRap = General.Nz(hfRap["NumeRap"], "").ToString();

                    if (numeRap != "")
                    {
                        string idRap = numeRap.Substring(numeRap.LastIndexOf("_") + 1);
                        if (General.IsNumeric(idRap))
                        {
                            string parola = General.Nz(General.ExecutaScalar(@"SELECT ""Parola"" FROM USERS WHERE F70102=@1", new object[] { Session["UserId"] }), "").ToString();
                            CriptDecript prc = new CriptDecript();
                            parola = prc.EncryptString(Constante.cheieCriptare, parola, Constante.DECRYPT);
                            if (parola == txtRapPass.Text)
                            {
                                string url = "../Generatoare/Reports/Pages/ReportView.aspx?q=" + General.URLEncode("IdRaportDyn=" + idRap + "&Angajat=" + General.Nz(Session["User_Marca"], -99).ToString());

                                //Response.Redirect(url);
                                ASPxPopupControl.RedirectOnCallback(url);
                            }
                            else
                            {
                                //MessageBox.Show("Parola nu este corecta", MessageBox.icoWarning, "Atentie !");
                                popUpPass.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parola nu este corecta");
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Lipsesc date", MessageBox.icoWarning, "Atentie !");
                }

                txtRapPass.Text = "";
                hfRap.Remove("NumeRap");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}