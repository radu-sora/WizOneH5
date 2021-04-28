using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;
using System.Drawing;

namespace WizOne.Pagini
{
    public partial class IntroDetaliu : System.Web.UI.Page
    {

        const string LayoutSessionKey = "1e38ba85-292e-494e-8f3e-5c8654a9dfef";
        bool saveLayout = false;
        List<metaWidget> widgetNames = new List<metaWidget>();

        public class metaWidget
        {
            public string Nume { get; set; }
            public string Eticheta { get; set; }
            public string RutaImg { get; set; }
        }

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
                Dami.AccesApp();
                

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                lblId.InnerText = Dami.TraduCuvant("lblId", "Id");
                lblDenumire.InnerText = Dami.TraduCuvant("lblDenumire", "Denumire");
                lblActiv.InnerText = Dami.TraduCuvant("lblActiv", "Activ");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    cmbTip.Items.Add(Dami.TraduCuvant("Badges & Stiri"));
                    cmbTip.Items.Add(Dami.TraduCuvant("Date din view"));
                    cmbTip.Items.Add(Dami.TraduCuvant("Rapoarte"));
                    cmbTip.Items.Add(Dami.TraduCuvant("Meniuri"));
                    cmbTip.Items.Add(Dami.TraduCuvant("Linkuri"));
                    cmbTip.SelectedIndex = 0;
                }

                //adaugam campurile din view
                System.Data.DataTable dt = General.IncarcaDT(@"SELECT * FROM ""IntroView"" WHERE ""IdUser""=@1", new string[] { Session["UserId"].ToString() });

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        Label lbl = new Label();
                        lbl.Text = dt.Rows[0][i].ToString();

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgView" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = dt.Columns[i].ColumnName;
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = true;
                        pnl.AllowResize = true;
                        pnl.ShowOnPageLoad = false;
                        pnl.Controls.Add(lbl);

                        divPanel.Controls.Add(pnl);

                        widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(dt.Columns[i].ColumnName ?? ""), RutaImg = "icoWidget1" });
                    }
                }

                //adaugam stirile
                DataTable dtSt = General.IncarcaDT($@"SELECT ""Continut"" FROM ""Stiri"" WHERE ""Activ""=1 AND ""DataInceput""<= {General.CurrentDate()} AND {General.CurrentDate()} <= ""DataSfarsit"" ", null);

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
                    pnl.AllowDragging = true;
                    pnl.AllowResize = true;
                    pnl.ShowOnPageLoad = false;
                    pnl.ShowHeader = false;
                    pnl.ScrollBars = ScrollBars.Vertical;
                    pnl.Controls.Add(divStiri);

                    divPanel.Controls.Add(pnl);

                    widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant("Stiri"), RutaImg = "icoWidget3" });
                }


                //adaugam badge-urile
                List<metaBadge> lstBadges = new List<metaBadge>();
                //pontajul echipei
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajEchipa.aspx", Eticheta = "Pontaj Echipa", RutaImg = "bdgPtj.jpg" });
                //cereri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCereri(), Pagina = "../Absente/Cereri.aspx", Eticheta = "Cereri", RutaImg = "bdgCer.jpg" });
                //evaluare
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx", Eticheta = "Evaluari", RutaImg = "bdgEvl.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=2", Eticheta = "Pontaj pe zi", RutaImg = "bdgPtj.jpg" });
                //pontaj pe angajat
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajDetaliat.aspx?tip=1", Eticheta = "Pontaj pe angajat", RutaImg = "bdgPtj.jpg" });
                //pontaj pe zi
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectPontaj(), Pagina = "../Pontaj/PontajOne.aspx", Eticheta = "Pontajul meu", RutaImg = "bdgPtj.jpg" });

                //Invitatie 360
                lstBadges.Add(new metaBadge { StringSelect = "SELECT * FROM USERS WHERE 1=2", Pagina = "../Eval/Invitatie.aspx", Eticheta = "Feedback", RutaImg = "bdgEvl.jpg" });

                //Florin 2019.10.15
                //Solicitari absente
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../Absente/Cereri.aspx", Eticheta = Dami.TraduCuvant("Solicitari absente"), RutaImg = "bdgCer.jpg" });
                //Solicitari diverse
                lstBadges.Add(new metaBadge { StringSelect = @"SELECT * FROM ""Ptj_Cereri"" WHERE 1=2", Pagina = "../CereriDiverse/Cereri.aspx", Eticheta = Dami.TraduCuvant("Solicitari diverse"), RutaImg = "bdgCer.jpg" });

                //Florin 2019.10.23
                //evaluare - filtrare questionare 360 sau proiect si care trebuie completate
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectEvaluare(), Pagina = "../Eval/EvalLista.aspx?q=34", Eticheta = Dami.TraduCuvant("Evaluari 360"), RutaImg = "bdgEvl.jpg" });

                //Radu 31.03.2021 - Referate
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectReferate(), Pagina = "../Posturi/FormLista.aspx?pp=1", Eticheta = Dami.TraduCuvant("Referate"), RutaImg = "bdgPtj.jpg" });

                //Radu 08.04.2021 - Cursuri
                lstBadges.Add(new metaBadge { StringSelect = Dami.SelectCursuri(), Pagina = "../Curs/CursuriInregistrare.aspx?pp=1", Eticheta = Dami.TraduCuvant("Cursuri"), RutaImg = "bdgCrs.jpg" });


                int j = 0;

                foreach (var ele in lstBadges)
                {
                    string str = @"<div onclick=""window.location.href='{1}'"" >
                                        <img src = ""../Fisiere/Imagini/{3}"" alt = """" />
                                        <span class='badge'>{0}</span>
                                    </div>
                                    <h3>{2}</h3>";

                    //Florin 2019.09.30 - am transformat nr din object in int
                    int nr = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT COUNT(*) FROM (" + ele.StringSelect + ") X", null),0));
                    str = string.Format(str, nr, ele.Pagina, ele.Eticheta, ele.RutaImg);

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
                    pnl.AllowDragging = true;
                    pnl.AllowResize = true;
                    pnl.ShowOnPageLoad = false;
                    pnl.ShowHeader = false;
                    pnl.Controls.Add(divBadge);

                    divPanel.Controls.Add(pnl);

                    j++;
                    widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(ele.Eticheta), RutaImg = "icoWidget4" });

                }


                //adaugam rapoartele                
                Wizrom.Reports.Pages.Manage.GetReports().ForEach(report =>
                {
                    ASPxButton btn = new ASPxButton();
                    btn.Text = Dami.TraduCuvant(report.Name);

                    // New report access interface
                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(report.Id);

                    btn.PostBackUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(report.Id, reportSettings.ToolbarType, reportSettings.ExportOptions);

                    ASPxDockPanel pnl = new ASPxDockPanel();
                    string nme = "wdgRap" + report.Id;
                    pnl.ID = nme;
                    pnl.PanelUID = nme;
                    pnl.ClientInstanceName = nme;
                    pnl.HeaderText = "";
                    pnl.DragElement = DragElement.Window;
                    pnl.OwnerZoneUID = "LeftZone";
                    pnl.CssClass = "cssDockPanel";
                    pnl.Styles.Header.CssClass = "cssHeader";
                    pnl.AllowDragging = true;
                    pnl.AllowResize = true;
                    pnl.ShowOnPageLoad = false;
                    pnl.ShowShadow = false;
                    pnl.ShowHeader = false;
                    pnl.Styles.Content.Paddings.Padding = 0;
                    pnl.Controls.Add(btn);

                    divPanel.Controls.Add(pnl);

                    widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(btn.Text), RutaImg = "icoWidget0" });
                });                


                //adaugam meniuri
                DataTable dtMnu = General.IncarcaDT(@"SELECT * FROM ""tblMeniuri"" WHERE COALESCE(""Stare"",0)=1", null);

                if (dtMnu.Rows.Count > 0)
                {
                    for (int i = 0; i < dtMnu.Rows.Count; i++)
                    {
                        //Florin 2019.09.09
                        //am schimbat linkurile in butoane

                        //ASPxHyperLink lnk = new ASPxHyperLink();
                        //lnk.Text = (dtMnu.Rows[i]["Nume"] ?? "").ToString();
                        //lnk.Font.Underline = true;

                        ASPxButton btn = new ASPxButton();
                        btn.Text = Dami.TraduCuvant((dtMnu.Rows[i]["Nume"] ?? "").ToString());
                        btn.CssClass = "btnMeniuDash";

                        string strUrl = dtMnu.Rows[i]["Pagina"].ToString();
                        string pag = strUrl;

                        if (strUrl.IndexOf("[") >= 0)
                            pag = strUrl.Substring(0, strUrl.IndexOf("["));
                        
                        //lnk.NavigateUrl = pag + ".aspx";
                        btn.PostBackUrl = pag + ".aspx";

                        ASPxDockPanel pnl = new ASPxDockPanel();
                        string nme = "wdgMnu" + i;
                        pnl.ID = nme;
                        pnl.PanelUID = nme;
                        pnl.ClientInstanceName = nme;
                        pnl.HeaderText = "";
                        pnl.DragElement = DragElement.Window;
                        pnl.OwnerZoneUID = "LeftZone";
                        pnl.CssClass = "cssDockPanel";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = true;
                        pnl.AllowResize = true;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowShadow = false;
                        pnl.ShowHeader = false;
                        pnl.Styles.Content.Paddings.Padding = 0;
                        //pnl.Controls.Add(lnk);
                        pnl.Controls.Add(btn);

                        divPanel.Controls.Add(pnl);

                        //widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(lnk.Text), RutaImg = "icoWidget2" });
                        widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(btn.Text), RutaImg = "icoWidget2" });

                    }
                }



                //adaugam link-uri custom
                DataTable dtLnk = General.IncarcaDT(@"SELECT * FROM ""IntroLink""  ", null);

                if (dtLnk.Rows.Count > 0)
                {
                    for (int i = 0; i < dtLnk.Rows.Count; i++)
                    {
                        ASPxHyperLink lnk = new ASPxHyperLink();
                        lnk.Text = (dtLnk.Rows[i]["Denumire"] ?? "").ToString();
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
                        pnl.CssClass = "cssDockPanel";
                        pnl.Styles.Header.CssClass = "cssHeader";
                        pnl.AllowDragging = true;
                        pnl.AllowResize = true;
                        pnl.ShowOnPageLoad = false;
                        pnl.ShowShadow = false;
                        pnl.ShowHeader = false;
                        pnl.Styles.Content.Paddings.Padding = 0;
                        pnl.Controls.Add(lnk);

                        divPanel.Controls.Add(pnl);

                        widgetNames.Add(new metaWidget { Nume = nme, Eticheta = Dami.TraduCuvant(lnk.Text), RutaImg = "icoWidget2" });
                    }
                }



                var src = widgetNames.Where(p => p.Nume.Contains("wdgBadges") || p.Nume.Contains("wdgStiri"));
                lst.DataSource = src;
                lst.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected string GetClientButtonClickHandler(DataListItem container)
        {
            return string.Format("function(s, e) {{ ShowWidgetPanel('{0}') }}", DataBinder.Eval(container.DataItem, "Nume"));
        }

        protected void dockManager_ClientLayout(object sender, DevExpress.Web.ASPxClientLayoutArgs e)
        {
            try
            {
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);

                //salvare
                if (e.LayoutMode == ClientLayoutMode.Saving && this.saveLayout)
                {
                    string strErr = "";
                    if (txtDenumire.Text.Trim() == "") strErr = ", denumire";
                    if (e.LayoutData.Trim() == "") strErr = ", continut";

                    if (strErr != "")
                    {
                        MessageBox.Show("Lipsesc date:" + strErr.Substring(1), MessageBox.icoWarning, "");
                        return;
                    }

                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                        case "Clone":
                            {
                                //Florin 2019.09.18
                                //string dt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                                //if (Constante.tipBD == 2) dt = DateTime.Now.ToString("yyyy-MMM-dd hh:mm:ss");
                                //id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(""Id"") FROM ""Intro"" ", null), 0)) + 1;
                                //General.ExecutaNonQuery(@"INSERT INTO ""Intro""(""Id"", ""Denumire"", ""Continut"", ""Activ"", USER_NO, TIME) VALUES(@1, @2, @3, @4, @5, @6)", new string[] { id.ToString(), txtDenumire.Text, e.LayoutData, "1", Session["UserId"].ToString(), dt });
                                General.ExecutaNonQuery(
                                    $@"INSERT INTO ""Intro""(""Id"", ""Denumire"", ""Continut"", ""Activ"", USER_NO, TIME) 
                                    VALUES((SELECT COALESCE(MAX(""Id""),0) FROM ""Intro"") + 1, @2, @3, @4, @5, {General.CurrentDate()})",
                                    new string[] { id.ToString(), txtDenumire.Text, e.LayoutData, "1", Session["UserId"].ToString() });
                            }
                            break;
                        case "Edit":
                            General.ExecutaNonQuery(@"UPDATE ""Intro"" SET ""Denumire"" =@2, ""Continut""=@3, ""Activ""=@4 WHERE ""Id""=@1", new string[] { id.ToString(), txtDenumire.Text, e.LayoutData, (chkActiv.Checked == true ? 1 : 0).ToString() });
                            break;
                    }

                    HttpContext.Current.Session["Sablon_Tabela"] = "Intro";
                    Response.Redirect("~/Pagini/SablonLista", false);

                }

                //load
                if (e.LayoutMode == ClientLayoutMode.Loading)
                {
                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Intro"" WHERE ""Id""=" + id, null);
                                if (dt.Rows.Count > 0)
                                {
                                    if (Session["Sablon_TipActiune"].ToString() == "Edit") txtId.Text = dt.Rows[0]["Id"].ToString();
                                    txtDenumire.Text = dt.Rows[0]["Denumire"].ToString();
                                    chkActiv.Checked = Convert.ToBoolean(dt.Rows[0]["Activ"]);
                                    e.LayoutData = dt.Rows[0]["Continut"].ToString();
                                }
                            }
                            break;

                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.saveLayout = true;
        }

        protected void cmbTip_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch(cmbTip.SelectedIndex)
                {
                    case 0:
                        lst.DataSource = widgetNames.Where(p => p.Nume.Contains("wdgBadges") || p.Nume.Contains("wdgStiri"));
                        break;
                    case 1:
                        lst.DataSource = widgetNames.Where(p => p.Nume.Contains("wdgView"));
                        break;
                    case 2:
                        lst.DataSource = widgetNames.Where(p => p.Nume.Contains("wdgRap"));
                        break;
                    case 3:
                        lst.DataSource = widgetNames.Where(p => p.Nume.Contains("wdgMnu"));
                        break;
                    case 4:
                        lst.DataSource = widgetNames.Where(p => p.Nume.Contains("wdgLnk"));
                        break;
                }

                lst.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "Intro";
                Response.Redirect("~/Pagini/SablonLista", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}
