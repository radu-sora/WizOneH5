using DevExpress.Web;
using ProceseSec;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne
{
    //Test Florin 2019.05.29
    public partial class Cadru : System.Web.UI.MasterPage
    {
        protected override void FrameworkInitialize()
        {            
            var targetId = Request["__EVENTTARGET"];

            if (targetId != null && targetId.StartsWith("ctl00$pnlHeader$a"))
                Session["IdLimba"] = targetId.Substring(targetId.Length - 2).ToUpper();

            General.SetLimba();

            base.FrameworkInitialize();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var ert1 = HttpContext.Current.Session["tblParam"];
                DataTable dtGigi1 = HttpContext.Current.Session["tblParam"] as DataTable;
                if (dtGigi1 != null)
                {
                    var edc = dtGigi1.Rows.Count;
                }


                if (Constante.esteTactil)
                    Dami.AccesTactil();
                else
                    Dami.AccesApp();

                //if (!HttpContext.Current.User.Identity.IsAuthenticated)
                //    Response.Redirect("../Default.aspx", false);

                if (General.Nz(Session["UserId"],"").ToString() == "" || !General.IsNumeric(Session["UserId"]))
                    Response.Redirect("../Default.aspx", false);


                //Florin 2019.08.19
                try
                {
                    if (Session["tblParam"] == null || ((DataTable)Session["tblParam"]).Rows.Count == 0)
                        Response.Redirect("../Default.aspx", false);
                }
                catch (Exception)
                {
                    Response.Redirect("../Default.aspx", false);
                }


                if (!IsPostBack)
                {
                    //Session["InformatiaCurenta"] = null;

                    object tipGrid = ContentPlaceHolder1.FindControl("grDate");
                    if (tipGrid != null && tipGrid.GetType() == typeof(ASPxGridView))
                    {
                        ASPxGridView grDate = (ASPxGridView)ContentPlaceHolder1.FindControl("grDate");
                        Session["Profil_Original"] = grDate.SaveClientLayout();

                        string sFont = Dami.ValoareParam("GridFontSize", "0");
                        if (sFont != "0" && General.IsNumeric(sFont)) grDate.Font.Size = FontUnit.Point(Convert.ToInt32(sFont));
                    }
                }


                #region Traducere

                etiLimbi.InnerText = Dami.TraduCuvant("Limba");
                etiTeme.InnerText = Dami.TraduCuvant("Teme");
                etiProfile.InnerText = Dami.TraduCuvant("Alege profil");
                etiCont.InnerText = Dami.TraduCuvant("ContulMeu");
                etiLog.InnerText = Dami.TraduCuvant("LogOut");

                lblNume.Text = Dami.TraduCuvant("Utilizator");
                lblAng.Text = Dami.TraduCuvant("Angajat");
                lblLimbi.Text = Dami.TraduCuvant("Limba");
                lblParola.Text = Dami.TraduCuvant("Parola");
                lblConfirma.Text = Dami.TraduCuvant("ConfirmaParola");


                #endregion

                if (General.VarSession("EsteAdmin").ToString() != "1")
                {
                    mnuCtx.Items[0].ClientVisible = false;
                    mnuCtx.Items[1].ClientVisible = false;
                    mnuCtx.Items[2].ClientVisible = false;
                    mnuCtx.Items[3].ClientVisible = false;
                    mnuCtx.Items[4].ClientVisible = false;
                }

                if (!IsPostBack)
                {

                    //incarcam lista de profile disponibile
                    string sqlPro = @"SELECT A.""Id"", A.""Denumire"", A.""Continut"", A.""Implicit"", A.""Activ"" 
                                FROM ""tblProfile"" A
                                INNER JOIN ""tblProfileLinii"" B ON  A.""Id"" = B.""Id""
                                INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                WHERE A.""Pagina"" = @1 AND C.""IdUser"" = @2 AND COALESCE(A.""Activ"" ,0) = 1
                                GROUP BY A.""Id"", A.""Denumire"", A.""Continut"", A.""Implicit"", A.""Activ"" ";
                    if (Constante.tipBD == 2)
                        sqlPro = @"SELECT A.""Id"", A.""Denumire"", TO_CHAR(A.""Continut"") AS ""Continut"", A.""Implicit"", A.""Activ"" 
                                FROM ""tblProfile"" A
                                INNER JOIN ""tblProfileLinii"" B ON  A.""Id"" = B.""Id""
                                INNER JOIN ""relGrupUser"" C ON B.""IdGrup"" = C.""IdGrup""
                                WHERE A.""Pagina"" = @1 AND C.""IdUser"" = @2 AND COALESCE(A.""Activ"" ,0) = 1
                                GROUP BY A.""Id"", A.""Denumire"", TO_CHAR(A.""Continut""), A.""Implicit"", A.""Activ"" ";								

                    DataTable dtPro = new DataTable();
                    if (General.Nz(Session["PaginaWeb"],"").ToString() != "") dtPro = General.IncarcaDT(sqlPro, new string[] { General.Nz(Session["PaginaWeb"], "").ToString().Replace("\\", "."), Session["UserId"].ToString() });
                    if (dtPro.Rows.Count == 0)
                        divCmbProfile.Visible = false;
                    else
                    {
                        ASPxGridView grDate = (ASPxGridView)ContentPlaceHolder1.FindControl("grDate");
                        if (grDate != null)
                        {
                            string prof = "";
                            DataRow[] lst = dtPro.Select("Implicit=1");
                            if (lst.Count() > 0)
                            {
                                prof = (lst[0]["Continut"] ?? "").ToString();
                                cmbProfile.Value = lst[0]["Id"];
                            }
                            else
                            {
                                prof = (dtPro.Rows[0]["Continut"] as string ?? "").ToString();
                                cmbProfile.Value = dtPro.Rows[0]["Id"];
                            }

                            //adugam profilul original al gridului

                            DataRow dr = dtPro.NewRow();
                            dr["Id"] = -1;
                            dr["Denumire"] = "Original";
                            dr["Continut"] = "";
                            dr["Implicit"] = 0;
                            dr["Activ"] = 1;
                            dtPro.Rows.Add(dr);

                            //incarcam profilul implicit
                            grDate.LoadClientLayout(prof);
                        }

                        cmbProfile.DataSource = dtPro;
                        cmbProfile.DataBindItems();
                    }
                }

                if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(this.ContentPlaceHolder1);

                //Florin 2018.09.10
                txtVers.InnerText = Constante.versiune;
                txtVers1.Text = Constante.versiune;
                //txtVers.InnerText = Dami.TraduCuvant("Versiune") + ": " + Dami.ValoareParam("VersiuneApp","3.3.0");

                var ert = HttpContext.Current.Session["tblParam"];
                DataTable dtGigi = HttpContext.Current.Session["tblParam"] as DataTable;
                if (dtGigi != null)
                {
                    var edc = dtGigi.Rows.Count;
                }
                if (Dami.ValoareParam("ArataLunaCurentaInSubsolApp", "0") == "1") txtLunaLucru.InnerText = Dami.TraduCuvant("Luna de lucru") + ": " + Dami.ValoareParam("LunaLucru") + " " + Dami.ValoareParam("AnLucru");

                //Radu 11.07.2018 - la PeliFilip, pentru anumiti utilizatori, sa nu se mai afiseze cuvantul Angajat si nici data angajarii
                int idDept = (Session["User_IdDept"] != null && Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 20 ? Convert.ToInt32(Session["User_IdDept"].ToString()) : -1);
                
                txtUsr.InnerText = Dami.TraduCuvant("Utilizator") + " " + Session["User"] + ", " + (idDept != 8 ? Dami.TraduCuvant("Angajat") : "") + " " + Session["User_NumeComplet"];
                SetLang();
                if (!Constante.esteTactil)
                    CreazaMeniu();
                SetAccount();
                if (Constante.esteTactil)
                {
                    pnlHeader.Visible = false;
                    pnlMeniu.Visible = false;
                }

                if (Constante.esteTactil)
                {
                    txtLunaLucru.Visible = false;
                    txtUsr.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSaveTheme_Click(object sender, EventArgs e)
        {
            try
            {
                if (__TEME.Value != null && __TEME.Value != "")
                {
                    string strSql = @"BEGIN;
                                      INSERT INTO tblConfigUsers(F70102, Tema, USER_NO, TIME) SELECT @1, @2, @1, GetDate() WHERE (SELECT COUNT(*) FROM tblConfigUsers WHERE F70102=@1)=0;
                                      UPDATE tblConfigUsers SET Tema=@2 WHERE F70102=@1;
                                      END;";

                    General.ExecutaNonQuery(strSql,  new string[] { Session["UserId"].ToString(), __TEME.Value });

                    MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSaveLang_Click(object sender, EventArgs e)
        {
            try
            {
                string strSql = @"UPDATE USERS SET ""IdLimba""=@1 WHERE F70102=@2";
                General.ExecutaNonQuery(strSql, new string[] { Session["IdLimba"].ToString(), Session["UserId"].ToString() });
                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
                SetLang();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSaveAcco_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtParola.Text == "" || txtConfirma.Text == "")
                {
                    MessageBox.Show("Lipsesc date !", MessageBox.icoWarning, "");
                }
                else
                {
                    if (txtParola.Text != txtConfirma.Text)
                    {
                        MessageBox.Show("Parola nu coincide !", MessageBox.icoWarning, "");
                    }
                    else
                    {
                        if (General.VarSession("ParolaComplexa").ToString() == "1")
                        {
                            var ras = General.VerificaComplexitateParola(txtParola.Text);
                            if (ras != "")
                            {
                                MessageBox.Show(ras, MessageBox.icoWarning, "Parola invalida");
                                return;
                            }
                        }

                        CriptDecript prc = new CriptDecript();
                        string parola = prc.EncryptString(Constante.cheieCriptare, txtParola.Text, 1);

                        General.AddUserIstoric();

                        string strSql = @"UPDATE USERS SET F70103=@1 WHERE F70102=@2";
                        General.ExecutaNonQuery(strSql, new string[] { parola, Session["UserId"].ToString() });

                        MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLang_Click(object sender, EventArgs e)
        {
            SetLang();
        }

        private void SetLang()
        {
            try
            {
                aRo.CssClass = "";
                aEn.CssClass = "";
                aEs.CssClass = "";
                aFr.CssClass = "";
                aDe.CssClass = "";
                aIt.CssClass = "";
                aBg.CssClass = "";
                
                switch (General.VarSession("IdLimba").ToString().ToUpper())
                {
                    case "RO":
                        aRo.CssClass += "selected";
                        break;
                    case "EN":
                        aEn.CssClass += "selected";
                        break;
                    case "ES":
                        aEs.CssClass += "selected";
                        break;
                    case "FR":
                        aFr.CssClass += "selected";
                        break;
                    case "IT":
                        aIt.CssClass += "selected";
                        break;
                    case "DE":
                        aDe.CssClass += "selected";
                        break;
                    case "BG":
                        aBg.CssClass += "selected";
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void CreazaMeniu()
        {
            try
            {
                string strSql = @"SELECT B.""IdMeniu"", B.""Parinte"", B.""Nume"", B.""Imagine"", B.""Ordine"", E.""Pagina"", B.""Descriere"" 
                                FROM ""MeniuLinii"" B
                                INNER JOIN ""relGrupMeniu2"" C ON B.""IdMeniu"" = C.""IdMeniu""
                                INNER JOIN ""relGrupUser"" D ON C.""IdGrup"" = D.""IdGrup""
                                LEFT JOIN ""tblMeniuri"" E ON E.""Id"" = B.""IdNomen""
                                WHERE D.""IdUser"" = @1 AND B.""Stare"" = 1
                                GROUP BY B.""Parinte"", B.""IdMeniu"", B.""Nume"", B.""Imagine"", B.""Ordine"", E.""Pagina"", B.""Descriere""
                                ORDER BY B.""Parinte"", B.""Ordine"" ";

                strSql = string.Format(strSql, Session["UserId"].ToString());

                DataTable dt = General.IncarcaDT(strSql, new string[] { Session["UserId"].ToString() });
                //DataTable dt = General.IncarcaDT(strSql, null);
                Session["tmpMeniu"] = dt;

                DataRow[] arrDr = { };
                if (dt.Rows.Count > 0) arrDr = dt.Select("Parinte IS NULL OR Parinte = 0", "Ordine ASC");


                foreach (DataRow dr in arrDr)
                {
                    NavBarGroup modul = new NavBarGroup();
                    modul.Name = "meniul" + dr["IdMeniu"].ToString();
                    modul.Text = Dami.TraduMeniu((dr["Nume"] as string ?? "").ToString());
                    modul.Expanded = false;
                    modul.HeaderImage.Url = "Fisiere/Imagini/Icoane/" + dr["Imagine"];

                    GroupTemplate temp = new GroupTemplate();
                    modul.ContentTemplate = temp;
                    
                    mnuSide.Groups.Add(modul);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);                
            }
        }

        private void SetAccount()
        {
            try
            {
                txtNume.Text = General.VarSession("User").ToString();
                txtAng.Text = General.VarSession("User_NumeComplet").ToString();
                txtLimba.Text = General.VarSession("IdLimba").ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ASPxGridView grDate = ContentPlaceHolder1.FindControl("grDate") as ASPxGridView;
                if (grDate != null)
                {
                    string ert = (Session["Profil_Original"] ?? "").ToString();
                    if ((int)cmbProfile.SelectedItem.Value == -1)
                        grDate.LoadClientLayout((Session["Profil_Original"] ?? "").ToString());
                    else
                        grDate.LoadClientLayout(Dami.Profil((int)cmbProfile.SelectedItem.Value));

                    if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(this.ContentPlaceHolder1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void callBackProfile_Callback(object sender, CallbackEventArgs e)
        {
            try
            {
                ASPxGridView grDate = (ASPxGridView)ContentPlaceHolder1.FindControl("grDate");
                if (grDate != null)
                {
                    Session["Profil_DataGrid"] = grDate.SaveClientLayout();
                    var ert = Session["Profil_DataGrid"];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkOut_Click(object sender, EventArgs e)
        {
            try
            {
                string tipVerif = General.Nz(Dami.ValoareParam("TipVerificareAccesApp"), "1").ToString();
                if (tipVerif == "5")
                    General.SignOut();
                else
                {
                    //Radu 20.07.2018 - am inlocuit ../ cu ~/
                    if (Constante.esteTactil)
                        //Response.Redirect("~/DefaultTactil.aspx", false);
                        Response.Redirect("~/Tactil/MainTactil.aspx", false);
                    else
                        Response.Redirect("~/Default.aspx", false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //Radu 25.03.2019        
        protected void OnImageClick(object sender, EventArgs e)
        {
            string url = string.Format("{0}://{1}{2}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                    (HttpContext.Current.Request.ApplicationPath.Equals("/")) ? string.Empty : HttpContext.Current.Request.ApplicationPath
                    );
            
            Response.Redirect(url + "/Pagini/MainPage.aspx");
        }
    }






    public class GroupTemplate : System.Web.UI.UserControl, ITemplate
    {
        public void InstantiateIn(Control container)
        {
            try
            {
                NavBarGroupTemplateContainer ct = container as NavBarGroupTemplateContainer;

                NavBarGroup modul = ct.DataItem as NavBarGroup;
                if (modul.Name == null || modul.Name == "") return;
                string id = modul.Name.Replace("meniul", "");

                DataTable dt = HttpContext.Current.Session["tmpMeniu"] as DataTable;

                ASPxNavBar mnu = new ASPxNavBar();
                mnu.AutoCollapse = true;
                mnu.EnableAnimation = true;
                mnu.Width = Unit.Percentage(100);
                mnu.Border.BorderWidth = 0;
                mnu.Paddings.Padding = 10;
                mnu.ItemClick += new NavBarItemEventHandler(mnu_ItemClick);

                DataRow[] arrGr = dt.Select("Parinte=" + id, "Ordine ASC");
                foreach (DataRow drGr in arrGr)
                {
                    NavBarGroup gr = new NavBarGroup();
                    gr.Text = Dami.TraduMeniu((drGr["Nume"] as string ?? "").ToString());
                    gr.Expanded = false;
                    gr.HeaderImage.Url = "Fisiere/Imagini/Icoane/" + drGr["Imagine"];
                    gr.HeaderImage.Width = Unit.Pixel(16);
                    gr.HeaderImage.Height = Unit.Pixel(16);
                    gr.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
                    gr.ItemStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
                    gr.ItemStyle.VerticalAlign = VerticalAlign.Top;

                    DataRow[] arrIt = dt.Select("Parinte=" + drGr["IdMeniu"].ToString(), "Ordine ASC");
                    foreach (DataRow drIt in arrIt)
                    {
                        NavBarItem it = new NavBarItem();
                        it.Text = Dami.TraduMeniu((drIt["Nume"] as string ?? "").ToString());
                        it.Image.Width = Unit.Pixel(16);
                        it.Image.Height = Unit.Pixel(16);
                        it.Image.Url = "Fisiere/Imagini/Icoane/" + drIt["Imagine"];
                        it.ToolTip = drIt["Pagina"].ToString();
                        gr.Items.Add(it);
                    }

                    mnu.Groups.Add(gr);
                }

                container.Controls.Add(mnu);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void mnu_ItemClick(object source, NavBarItemEventArgs e)
        {
            try
            {
                NavBarItem it = e.Item as NavBarItem;
                NavBarGroupTemplateContainer gr = e.Item.NavBar.Parent as NavBarGroupTemplateContainer;
                HttpContext.Current.Session["Titlu"] = gr.Group.Text + " - " + it.Group.Text + " - " + it.Text;
                
                string tt = it.ToolTip;
                string pag = tt;

                if (tt.IndexOf("[") >= 0)
                {
                    pag = tt.Substring(0, tt.IndexOf("["));

                    HttpContext.Current.Session.Remove("Sablon_Tabela");
                    HttpContext.Current.Session.Remove("Sablon_CheiePrimara");
                    HttpContext.Current.Session.Remove("Sablon_TipActiune");

                    HttpContext.Current.Session.Remove("InformatiaCurenta");
                    HttpContext.Current.Session.Remove("SecuritateCamp");

                    HttpContext.Current.Session.Remove("PaginaWeb");

                    switch (pag)
                    {
                        case "Pagini\\SablonNomenRec":
                        case "Pagini\\SablonNomen":
                            //HttpContext.Current.Session.Remove("NomenTableName");
                            //HttpContext.Current.Session["NomenTableName"] = tt.Replace(pag + "[", "").Replace("]", "");
                            //break;
                        case "Pagini\\SablonLista":
                            HttpContext.Current.Session["Sablon_Tabela"] = tt.Replace(pag + "[", "").Replace("]", "");
                            break;
                    }
                }

                string param = "";
                if (tt.IndexOf("?") >= 0)
                {
                    pag = tt.Substring(0, tt.IndexOf("?"));
                    param = tt.Substring(tt.IndexOf("?"));
                }

                HttpContext.Current.Session["PaginaWeb"] = pag;
                HttpContext.Current.Response.Redirect("~/" + pag + ".aspx" + param, false);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(this.Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }






}

