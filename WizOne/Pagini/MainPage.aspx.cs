using DevExpress.Web;
using Newtonsoft.Json.Linq;
using ProceseSec;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Eval;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class MainPage : Page
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

                Session["PaginaWeb"] = "Pagini.MainPage";

                if (!IsPostBack || (ctlPost?.Contains("pnlHeader") ?? false))   //Radu 29.01.2020
                {
                    if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0)
                        Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                    if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.Pelifilip && General.Nz(Session["PrimaIntrare"], "0").ToString() == "1")
                    {
                        pnlMsgWelcome.Style["display"] = "inline-block";
                        General.ExecutaNonQuery("UPDATE USERS SET F70105=0 WHERE F70102=@1", new object[] { Session["UserId"] });
                    }

                    string layoutSql = $@"SELECT TOP 1 A.""Continut"" FROM ""Intro"" A
                                    INNER JOIN ""relGrupIntro"" B ON A.""Id""=B.""IdIntro""
                                    INNER JOIN ""relGrupUser"" C ON B.""IdGrup""=C.""IdGrup""
                                    INNER JOIN ""tblGrupUsers"" D ON C.""IdGrup""=D.""Id""
                                    WHERE ""Activ""=1 AND C.""IdUser""={Session["UserId"]}
                                    GROUP BY A.""Continut"", A.TIME, D.""Prioritate""
                                    ORDER BY D.""Prioritate"" DESC, A.TIME DESC";

                    JObject layoutData = JObject.Parse(General.IncarcaDT(layoutSql, null).Rows.OfType<DataRow>().FirstOrDefault()?["Continut"] as string ?? "{}");

                    #region View

                    //adaugam campurile din view
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""IntroView"" WHERE ""IdUser""=@1", new string[] { Session["UserId"].ToString() });
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string nme = "wdgView" + i;
                        var visible = (bool?)layoutData[nme]?[0] ?? false;
                        if (visible)
                        {
                            Label lbl = new Label();
                            lbl.Text = Dami.TraduCuvant((dt.Rows[0][i] ?? "").ToString());
                            AdaugaControl(lbl, nme, Dami.TraduCuvant(dt.Columns[i].ColumnName));
                        }
                    }

                    #endregion

                    #region Stiri

                    //adaugam stirile
                    DataTable dtSt = General.IncarcaDT($@"SELECT A.""Continut""
                                FROM ""Stiri"" A
                                INNER JOIN ""relGrupStire"" B ON A.""Id"" = B.""IdStire""
                                INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                WHERE COALESCE(A.""Activ"",0) = 1 AND A.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= A.""DataSfarsit"" AND C.""IdUser"" = {Session["UserId"]} AND ""IdLimba""='{General.Nz(Session["IdLimba"], "RO")}'
                                GROUP BY A.""Continut"" ", null);

                    HtmlGenericControl divStiri = new HtmlGenericControl("div");
                    foreach (DataRow dr in dtSt.Rows)
                    {
                        divStiri.InnerHtml += dr["Continut"].ToString();
                    }

                    AdaugaControl(divStiri, "wdgStiri0");

                    #endregion

                    #region Badges

                    int j = 0;
                    List<metaBadge> lstBadges = IncarcaBadges(); 
                    foreach (var ele in lstBadges)
                    {
                        string nme = "wdgBadges" + j;
                        var visible = (bool?)layoutData[nme]?[0] ?? false;
                        if (visible)
                        {
                            string str = @"<div onclick=""window.location.href='{1}'"" >
                                        <img src = ""../Fisiere/Imagini/{3}"" alt = """" />
                                        <span class='badge'>{0}</span>
                                    </div>
                                    <h3>{2}</h3>";
                            object nr = General.ExecutaScalar("SELECT COUNT(*) FROM (" + ele.StringSelect + ") X", null);
                            str = string.Format(str, General.Nz(nr, "0").ToString(), ele.Pagina, Dami.TraduCuvant(ele.Eticheta), ele.RutaImg);

                            HtmlGenericControl divBadge = new HtmlGenericControl("div");
                            divBadge.Attributes["class"] = "badgeContainer";
                            divBadge.InnerHtml = str;

                            AdaugaControl(divBadge, nme);

                            Session["tmpMeniu3"] += ";" + ele.Pagina;
                        }

                        j++;
                    }

                    #endregion

                    #region Rapoarte

                    //adaugam rapoartele
                    Wizrom.Reports.Pages.Manage.GetReports().ForEach(report => // Do not use the DynReports table directly. Use Manage[GetReports, GetReportSettings] instead.
                    {                                                          // To modify reports list from code use (make static) the AddReport, SetReport and DelReport methods from Manage class.
                        var widgetName = "wdgRap" + report.Id;

                        if ((bool?)layoutData[widgetName]?[0] ?? false) // FIX05 - Is this shit working?   YES - It is!
                        {
                            ASPxButton btn = new ASPxButton();
                            btn.ID = "btn_Rap" + report.Id;
                            btn.Text = Dami.TraduCuvant(report.Name);
                            btn.UseSubmitBehavior = false;

                            // New report access interface                     
                            // Also, do not open reports pages i.e. View, Design or Print directly. Use ReportProxy[View, Design, Print] or [GetViewUrl, GetDesignUrl, GetPrintUrl] instead.
                            var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(report.Id);
                            var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(report.Id, reportSettings.ToolbarType, reportSettings.ExportOptions, new { Angajat = General.Nz(Session["User_Marca"], -99).ToString() });

                            if (report.Restricted)
                            {
                                btn.ClientSideEvents.Click = $"function(s, e) {{ onRapButtonClick('{ResolveClientUrl(reportUrl)}'); }}";
                                btn.AutoPostBack = false;
                            }
                            else
                                btn.PostBackUrl = reportUrl;

                            AdaugaControl(btn, widgetName);
                        }
                    });

                    #endregion

                    #region Meniuri

                    //adaugam meniuri
                    DataTable dtMnu = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" WHERE COALESCE(""Stare"",0)=1", null);

                    for (int i = 0; i < dtMnu.Rows.Count; i++)
                    {
                        string nme = "wdgMnu" + i;
                        var visible = (bool?)layoutData[nme]?[0] ?? false;

                        if (visible)
                        {
                            ASPxButton btn = new ASPxButton();
                            btn.Text = Dami.TraduCuvant((dtMnu.Rows[i]["Nume"] ?? "").ToString());
                            btn.CssClass = "btnMeniuDash";

                            string strUrl = dtMnu.Rows[i]["Pagina"].ToString();
                            string pag = strUrl;

                            if (strUrl.IndexOf("[") >= 0)
                                pag = strUrl.Substring(0, strUrl.IndexOf("["));

                            btn.PostBackUrl = "~/" + pag + ".aspx?pp=1";

                            AdaugaControl(btn, nme);

                            int xxx = 0;
                            if (pag.Contains("AvsXDec"))
                                xxx++;

                            Session["tmpMeniu3"] += ";" + pag;
                        }
                    }

                    #endregion

                    #region Linkuri

                    //adaugam link-uri custom
                    DataTable dtLnk = General.IncarcaDT(@"SELECT * FROM ""IntroLink""  ", null);

                    for (int i = 0; i < dtLnk.Rows.Count; i++)
                    {
                        ASPxHyperLink lnk = new ASPxHyperLink();
                        lnk.Text = Dami.TraduMeniu((dtLnk.Rows[i]["Denumire"] ?? "").ToString());
                        lnk.Font.Underline = true;
                        lnk.NavigateUrl = (dtLnk.Rows[i]["Link"] ?? "").ToString();
                        lnk.Target = "_blank";
                        lnk.Wrap = DevExpress.Utils.DefaultBoolean.True;

                        string nme = "wdgLnk" + i;

                        AdaugaControl(lnk, nme);
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void dockManager_ClientLayout(object sender, ASPxClientLayoutArgs e)
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

                    if (dt.Rows.Count > 0 && dt.Rows[0]["Continut"] != null)
                        e.LayoutData = dt.Rows[0]["Continut"].ToString();
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
                CriptDecript prc = new CriptDecript();
                string parola = General.Nz(General.ExecutaScalar(@"SELECT ""Parola"" FROM USERS WHERE F70102=@1", new object[] { Session["UserId"] }), "").ToString();

                parola = prc.EncryptString(Constante.cheieCriptare, parola, Constante.DECRYPT);

                if (parola != txtRapPass.Text)
                    popUpPass.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parola nu este corecta");
                
                txtRapPass.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private List<metaBadge> IncarcaBadges()
        {
            List<metaBadge> lstBadges = new List<metaBadge>();

            try
            {                
                //pontajul echipei
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajEchipa.aspx?pp=1", Eticheta = Dami.TraduCuvant("Pontaj Echipa"), RutaImg = "bdgPtj.jpg" });
                
                //cereri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCereri() + $@" AND A.""Actiune"" = 1 AND A.""IdStare"" IN (1,2) ", Pagina = "../Absente/Lista.aspx?pp=1", Eticheta = Dami.TraduCuvant("Cereri"), RutaImg = "bdgCer.jpg" });

                //evaluare - filtrare chestionare nefinalizate si care nu sunt 360 sau proiect
                lstBadges.Add(new metaBadge { StringSelect = "SELECT * FROM (" + Dami.SelectEvaluare() + ") X WHERE Stare NOT LIKE '%finalizat%'", Pagina = "../Eval/EvalLista.aspx?q=12&pp=1", Eticheta = Dami.TraduCuvant("Evaluari"), RutaImg = "bdgEvl.jpg" });

                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=2&pp=1", Eticheta = Dami.TraduCuvant("Pontaj pe zi"), RutaImg = "bdgPtj.jpg" });
                
                //pontaj pe angajat
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=1&pp=1", Eticheta = Dami.TraduCuvant("Pontaj pe angajat"), RutaImg = "bdgPtj.jpg" });
                
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajOne.aspx?pp=1", Eticheta = Dami.TraduCuvant("Pontajul meu"), RutaImg = "bdgPtj.jpg" });

                //Invitatie 360
                Invitatie pagInv = new Invitatie();
                lstBadges.Add(new metaBadge { StringSelect = pagInv.CreazaSelect(Convert.ToInt32(General.Nz(Session["User_Marca"], -99)), Convert.ToInt32(General.Nz(Session["UserId"], -99))), Pagina = "../Eval/Invitatie.aspx?pp=1", Eticheta = Dami.TraduCuvant("Feedback"), RutaImg = "bdgEvl.jpg" });

                //Solicitari absente
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../Absente/Cereri.aspx?pp=1", Eticheta = Dami.TraduCuvant("Solicitari absente"), RutaImg = "bdgCer.jpg" });
                
                //Solicitari diverse
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../CereriDiverse/Cereri.aspx?pp=1", Eticheta = Dami.TraduCuvant("Solicitari diverse"), RutaImg = "bdgCer.jpg" });

                //Evaluare - filtrare questionare 360 sau proiect si care trebuie completate
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx?q=34&pp=1", Eticheta = Dami.TraduCuvant("Evaluari 360"), RutaImg = "bdgEvl.jpg" });

                //Referate
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectReferate(), Pagina = "../Posturi/FormLista.aspx?pp=1", Eticheta = Dami.TraduCuvant("Referate"), RutaImg = "bdgPtj.jpg" });

                //Cursuri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCursuri(), Pagina = "../Curs/CursuriInregistrare.aspx?pp=1", Eticheta = Dami.TraduCuvant("Cursuri"), RutaImg = "bdgCrs.jpg" });

                //Cursuri aprobare
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectAprobareCursuri(), Pagina = "../Curs/Aprobare.aspx?pp=1", Eticheta = Dami.TraduCuvant("Aprobare cursuri"), RutaImg = "bdgCrs.jpg" });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return lstBadges;
        }

        private void AdaugaControl(Control ctl, string id, string headerText = "")
        {
            try
            {
                ASPxDockPanel pnl = new ASPxDockPanel();
                pnl.ID = id;
                pnl.PanelUID = id;
                pnl.ClientInstanceName = id;
                pnl.HeaderText = headerText;
                pnl.DragElement = DragElement.Window;
                pnl.OwnerZoneUID = "LeftZone";
                pnl.CssClass = "cssDockPanel";
                pnl.Styles.Header.CssClass = "cssHeader";
                pnl.AllowDragging = false;
                pnl.AllowResize = false;
                pnl.ShowOnPageLoad = false;
                pnl.ShowCloseButton = false;

                if (headerText.Trim() == "")
                    pnl.ShowHeader = false;
                if (id.IndexOf("wdgStiri") >= 0)
                    pnl.ScrollBars = ScrollBars.Vertical;

                if (id.IndexOf("wdgRap") >= 0 || id.IndexOf("wdgMnu") >= 0 || id.IndexOf("wdgLnk") >= 0)
                {
                    pnl.CssClass = "cssDockPanel_noBorder";
                    pnl.Styles.Content.Paddings.Padding = 0;
                    pnl.ShowShadow = false;
                }

                pnl.Controls.Add(ctl);

                divPanel.Controls.Add(pnl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}