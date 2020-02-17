using Newtonsoft.Json.Linq;
using ProceseSec;
using System;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using WizOne.Module;
using WizOne.Module.Saml;

namespace WizOne
{
    public partial class Default : System.Web.UI.Page
    {
        static string arrIncercari = "";

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                string tipVerif = General.Nz(Dami.ValoareParam("TipVerificareAccesApp"), "1").ToString();
                if (tipVerif == "6")
                {
                    string samlLink = General.Nz(Dami.ValoareParam("SAML_Link"), "").ToString();

                    if (samlLink != "")
                    {
                        if (Request.Form["SAMLResponse"] == null)
                            Response.Redirect(samlLink, false);
                    }
                    else
                    {
                        MessageBox.Show("Lipseste linkul de redirectionare (parametrul SAML_Link)", MessageBox.icoWarning, "Eroare configurare");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                lblPan1.InnerText = Dami.TraduCuvant("Utilizator");
                lblPan2.InnerText = Dami.TraduCuvant("Parola");
                lnkUitat.Text = Dami.TraduCuvant("Am uitat parola");

                #endregion

                //Florin 2018.09.10
                txtVers.Text = Constante.versiune;

                if (Constante.esteTactil)
                {
                    Response.Redirect("DefaultTactil.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    arrIncercari = "";

                    //este necesar sa golim variabilele de sesiune pt cazul cand apasa Log Out
                    Session.Clear();
                    InitSessionVariables();
                }

                Session["IdLimba"] = Dami.ValoareParam("LimbaStart");
                if (General.VarSession("IdLimba").ToString() == "") Session["IdLimba"] = "RO";
                if (Dami.ValoareParam("LinkParolaUitata") != "" && Dami.ValoareParam("LinkParolaUitata") == "0") lnkUitat.Visible = false;


                //Florin 2019.07.23
                string tipVerif = General.Nz(Dami.ValoareParam("TipVerificareAccesApp"), "1").ToString();
                switch (tipVerif)
                {
                    case "3":
                    case "4":
                        {
                            string usrTMP = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                            int poz = usrTMP.IndexOf(@"\");
                            if (poz > 0) usrTMP = usrTMP.Remove(0, poz + 1);

                            Verifica(usrTMP, "", false);
                        }
                        break;
                    case "5":
                        {
                            if (Page.User.Identity.IsAuthenticated)
                            {
                                var claimsPrincipal = Page.User as ClaimsPrincipal;

                                if (claimsPrincipal != null)
                                {
                                    string usrTMP = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Upn).Select(c => c.Value).SingleOrDefault();
                                    int poz = usrTMP.IndexOf("@");
                                    if (poz > 0) usrTMP = usrTMP.Remove(poz);
                                    General.MemoreazaEroarea(usrTMP);
                                    string txtRas = Verifica(usrTMP, "", false, false);

                                    if (General.Nz(Session["SecApp"], "").ToString() != "OK")
                                    {
                                        divRas.Visible = false;
                                        lblRaspuns.Visible = true;
                                        lblRaspuns.InnerText = txtRas;
                                    }
                                }
                            }
                        }
                        break;
                    case "6":
                        {
                            string samlCertificate = General.Nz(Dami.ValoareParam("SAML_Certificat"), "").ToString();
                            if (samlCertificate != "")
                            {
                                Response samlResponse = new Response(samlCertificate);
                                samlResponse.LoadXmlFromBase64(Request.Form["SAMLResponse"]);
                                if (samlResponse.IsValid())
                                {
                                    string usrTMP = samlResponse.GetNameID();
                                    Verifica(usrTMP, "", false);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Lipseste certificatul (parametrul SAML_Certificat)", MessageBox.icoWarning, "Eroare configurare");
                                return;
                            }
                        }
                        break;
                }

                if (Dami.ValoareParam("Captcha") == "1")
                {
                    HtmlGenericControl divCap = new HtmlGenericControl("div");
                    divCap.Attributes["class"] = "g-recaptcha";
                    divCap.Attributes["data-sitekey"] = Dami.ValoareParam("Captcha_Site");

                    divOuter.Controls.Add(divCap);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (Dami.ValoareParam("Captcha") == "1")
                {
                    if (IsReCaptchValid())
                        Verifica(General.Strip(txtPan1.Text), txtPan2.Text, true);
                    else
                        MessageBox.Show("Va rugam verificati codul captcha", MessageBox.icoWarning, "Captcha");
                }
                else
                    Verifica(General.Strip(txtPan1.Text), txtPan2.Text, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }            
        }

        protected void lnkUitat_Click(object sender, EventArgs e)
        {
            try
            {
                TrimiteParola(General.Strip(txtPan1.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void TrimiteParola(string utilizator)
        {
            try
            {

                DataRow dr = General.IncarcaDR("SELECT F70103, \"Mail\" FROM USERS WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                if (dr != null)
                {
                    if (dr["Mail"].ToString() == "")
                    {
                        MessageBox.Show("Adresa de mail nu exista", MessageBox.icoWarning);
                    }
                    else
                    {
                        if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) == 14)
                        {
                            string sqlUp = @"INSERT INTO WIZONE_HIS_DEC.dbo.ParolaUitata(Companie, UserId, username, Parola, Mail, Status, TIME)
                                        SELECT (SELECT NumeCompanie FROM tblConfig)AS Companie, F70102, F70104, F70103, Mail, 0 AS Status, getdate() AS TIME
                                        FROM Users A
                                        WHERE F70104 = @1";

                            General.ExecutaNonQuery(sqlUp,new string[] { utilizator });

                            MessageBox.Show("Parola a fost resetata", MessageBox.icoSuccess);
                        }
                        else
                        {
                            CriptDecript prc = new CriptDecript();
                            string parola = prc.EncryptString(Constante.cheieCriptare, dr["F70103"].ToString(), 2);

                            string msg = General.TrimiteMail(dr["Mail"].ToString(), "", "", "Parola uitata in aplicatie!", "Parola dvs. pentru conectarea la WizOne este: " + parola);

                            if (msg == "") 
                                MessageBox.Show("Parola a fost trimisa la adresa dvs de mail", MessageBox.icoSuccess);
                            else
                                MessageBox.Show(msg, MessageBox.icoInfo);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string Blocheaza(string utilizator)
        {
            string txtRas = "";

            try
            {
                arrIncercari += utilizator + ";";
                if (Regex.Matches(arrIncercari, utilizator + ";").Count >= Convert.ToInt32(Dami.ValoareParam("Parola_NrIncercSuccNer", "3")))
                {
                    bool esteBlocat = BlocheazaUser(utilizator);
                    if (esteBlocat)
                    {
                        txtRas = "Contul este blocat ! Contactati administratorul de sistem!";
                    }
                    else
                    {
                        txtRas = "Utilizator/Parola gresita ! Contactati administratorul de sistem!";
                    }
                }
                else
                {
                    txtRas = "Utilizator/Parola gresita ! Contactati administratorul de sistem!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return txtRas;
        }

        private Boolean BlocheazaUser(string utilizator)
        {
            bool esteBlocat = false;

            try
            {
                string tipVerif = Dami.ValoareParam("TipVerificareAccesApp");
                if (tipVerif == "") tipVerif = "1";

                if (tipVerif == "2")
                {
                    string ADDom = Dami.ValoareParam("ADNumeDomeniu");
                    string ADuser = Dami.ValoareParam("ADUser");
                    string ADpass = Dami.ValoareParam("ADPass");

                    if (ADDom != "" && ADuser != "" && ADpass != "")
                    {
                        PrincipalContext context = new PrincipalContext(ContextType.Domain, ADDom, ADuser, ADpass);
                        UserPrincipal usr = UserPrincipal.FindByIdentity(context, utilizator);
                        if (usr != null)
                        {
                            //blocam decat daca exista in tablea USER si este activ in AD 
                            DataRow dr = General.IncarcaDR("SELECT F70103, \"Mail\" FROM USERS WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                            if (dr != null && usr.Enabled == true)
                            {
                                usr.Enabled = false;
                                usr.Save();

                                esteBlocat = true;
                            }
                        }
                    }
                }
                else
                {
                    if ( Constante.tipBD == 2 )
                        General.ExecutaNonQuery("UPDATE USERS SET F70114=1,BLOCKTIME = SYSDATE  WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                    else
                        General.ExecutaNonQuery("UPDATE USERS SET F70114=1,BLOCKTIME = GETDATE()  WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                    esteBlocat = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return esteBlocat;
        }

        protected string Verifica(string utilizator, string parola, bool dinButon, bool cuMesaj = true)
        {
            string txtRas = "";
            bool schimba = false;

            try
            {
                string rasp = VerificaUser(utilizator, parola, dinButon);
                if (rasp.Length > 1)
                {
                    Session["IdLimba"] = rasp.Substring(rasp.Length - 2, 2);
                    rasp = rasp.ToUpper().Replace(General.Nz(Session["IdLimba"], "RO").ToString(), "");
                }

                switch (Convert.ToInt32(rasp))
                {
                    case -1:                     //nume Domeniu nu este configurat
                        General.InregistreazaLogarea(0, txtPan1.Text, "Numele de domeniu nu este configurat");
                        txtRas = "Numele de domeniu nu este configurat !";
                        break;
                    case 0:                     //user inexistent
                        General.InregistreazaLogarea(0, txtPan1.Text, "Utilizator inexistent");
                        txtRas = "Utilizator inexistent! Contactati administratorul de sistem!";
                        break;
                    case 1:                     //user existent parola eronata
                        General.InregistreazaLogarea(0, txtPan1.Text, "Introducerea unei parole gresite");
                        txtRas = Blocheaza(utilizator);
                        break;
                    case 2:                     //valid si blocat
                        General.InregistreazaLogarea(0, txtPan1.Text, "Cont blocat");
                        txtRas = "Contul este blocat ! Contactati administratorul de sistem";
                        break;
                    case 3:                     //valid si activ
                        string op = "+";
                        string exp = $@"CASE WHEN DATEADD(d,COALESCE((SELECT CASE WHEN COALESCE(""Valoare"",99999) = 0 THEN 99999 ELSE COALESCE(""Valoare"",99999) END FROM ""tblParametrii"" WHERE ""Nume""='Parola_VechimeMaxima'),30),(SELECT TOP 1 ""Data"" FROM ""ParoleUtilizatorIstoric"" WHERE ""IdUser""=A.F70102 ORDER BY ""Data"" Desc)) < GetDate() THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                        if (Constante.tipBD == 2)
                        {
                            op = "||";
                            exp = $@"CASE WHEN (COALESCE((SELECT CASE WHEN COALESCE(TO_NUMBER(""Valoare""),30) = 0 THEN 99999 ELSE COALESCE(TO_NUMBER(""Valoare""),99999) END FROM ""tblParametrii"" WHERE ""Nume""='Parola_VechimeMaxima'),30) + d.""Data"") < SYSDATE THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                        }

                        string sql_G = @"SELECT CONVERT(nvarchar(10), Camp) + ' ;' + CONVERT(nvarchar(10), Camp) + ',;' + CONVERT(nvarchar(10), Camp) + '+;' + CONVERT(nvarchar(10), Camp) + '||;'   FROM tblCampSec WHERE COALESCE(Criptat,0)=1 FOR XML PATH ('')";
                        if (Constante.tipBD == 2)
                            sql_G = @"SELECT LISTAGG(""Camp"" , ' ;' ) WITHIN GROUP (ORDER by ""Camp"")   FROM ""tblCampSec"" WHERE COALESCE(""Criptat"",0)=1 ";
                        Constante.campuriGDPR = (General.ExecutaScalar(sql_G, null) ?? "").ToString();

                        string sql_G_S = @"SELECT CONVERT(nvarchar(10), Camp) + ';'  FROM tblCampSec WHERE COALESCE(Criptat,0)=1 FOR XML PATH ('')";
                        if (Constante.tipBD == 2)
                            sql_G_S = @"SELECT LISTAGG(""Camp"" , ';') WITHIN GROUP (ORDER by ""Camp"") FROM ""tblCampSec"" WHERE COALESCE(""Criptat"",0)=1";
                        Constante.campuriGDPR_Strip = (General.ExecutaScalar(sql_G_S, null) ?? "").ToString();

                        string strSql = @"SELECT a.F70102 AS ""UtilizatorId"", a.F70104 AS ""Utilizator"", COALESCE(a.""F70113"",0) AS ""ResetareParola"", F70112 AS ""ParolaComplexa"",
                                    CRP.F10003 AS ""Marca"", CRP.F10007 AS ""IdDept"",C.F00608 AS ""Dept"",CRP.F10017 AS CNP, A.F70105,
                                    CRP.F10008 As ""Nume"", CRP.F10009 AS ""Prenume"", CRP.F10022 AS F10022,
                                    (SELECT MAX(""Tema"") FROM ""tblConfigUsers"" WHERE F70102=A.F70102) AS ""Tema"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=0)=0 THEN 0 ELSE 1 END AS ""EsteAdmin"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=99)=0 THEN 0 ELSE 1 END AS ""EsteInGrup99"",
                                    {1}
                                    FROM USERS A
                                    LEFT JOIN F100 CRP ON A.F10003=CRP.F10003
                                    LEFT JOIN F006 C ON CRP.F10007=C.F00607
                                    WHERE UPPER(A.F70104)='" + utilizator.ToUpper() + "'";

                        if (Constante.tipBD == 2)
                        {
                            strSql = @"SELECT a.F70102 AS ""UtilizatorId"", a.F70104 AS ""Utilizator"", COALESCE(a.""F70113"",0) AS ""ResetareParola"", F70112 AS ""ParolaComplexa"",
                                    CRP.F10003 AS ""Marca"", CRP.F10007 AS ""IdDept"",C.F00608 AS ""Dept"",CRP.F10017 AS CNP, A.F70105,
                                    CRP.F10008 As ""Nume"", CRP.F10009 AS ""Prenume"", CRP.F10022 AS F10022,
                                    (SELECT MAX(""Tema"") FROM ""tblConfigUsers"" WHERE F70102=A.F70102) AS ""Tema"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=0)=0 THEN 0 ELSE 1 END AS ""EsteAdmin"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=99)=0 THEN 0 ELSE 1 END AS ""EsteInGrup99"",
                                    {1}
                                    FROM USERS A
                                    LEFT JOIN F100 CRP ON A.F10003=CRP.F10003
                                    LEFT JOIN F006 C ON CRP.F10007=C.F00607
                                    left JOIN (SELECT ""IdUser"", ""Data"" FROM ""ParoleUtilizatorIstoric"" WHERE rownum = 1  ORDER BY ""Data"" Desc) d on d.""IdUser""=A.F70102 
                                    WHERE UPPER(A.F70104)='" + utilizator.ToUpper() + "'";
                        }

                        strSql = string.Format(strSql, op, exp);

                        DataRow drUsr = General.IncarcaDR(strSql, null);
                        if (drUsr != null)
                        {
                            var ert = General.Nz(drUsr["Nume"], "").ToString() + " " + General.Nz(drUsr["Prenume"], "").ToString();

                            Session["UserId"] = Convert.ToInt32(drUsr["UtilizatorId"]);
                            Session["User"] = drUsr["Utilizator"].ToString();
                            Session["User_Marca"] = Convert.ToInt32(General.Nz(drUsr["Marca"], -99));
                            Session["User_IdDept"] = Convert.ToInt32(General.Nz(drUsr["IdDept"], -99));
                            Session["User_Dept"] = drUsr["Dept"].ToString();
                            Session["User_NumeComplet"] = General.Nz(drUsr["Nume"], "").ToString() + " " + General.Nz(drUsr["Prenume"], "").ToString();
                            Session["User_CNP"] = drUsr["CNP"].ToString();
                            Session["EsteAdmin"] = Convert.ToInt32(General.Nz(drUsr["EsteAdmin"], 0));
                            Session["EsteInGrup99"] = Convert.ToInt32(General.Nz(drUsr["EsteInGrup99"], 0));
                            Session["ParolaComplexa"] = Convert.ToInt32(General.Nz(drUsr["ParolaComplexa"], 0));
                            Session["PrimaIntrare"] = General.Nz(drUsr["F70105"], 0);

                            General.SetTheme();

                            string tipVerif = Dami.ValoareParam("TipVerificareAccesApp");
                            if (tipVerif == "") tipVerif = "1";

                            //Radu 21.03.2018 - daca tipVerif este 2 sau 4, nu mai trebuie sa verificam validitatea parolei
                            if (Convert.ToInt32(General.Nz(drUsr["ResetareParola"], 0)) == 1 && (tipVerif == "1" || tipVerif == "3"))
                            {
                                General.InregistreazaLogarea(1, txtPan1.Text);
                                Session["SecApp"] = "OK";
                                Response.Redirect("Pagini/SchimbaParola.aspx", false);
                            }
                            else
                            {
                                if (Convert.ToInt32(General.Nz(drUsr["ParolaExpirata"], 0)) == 1 && (tipVerif == "1" || tipVerif == "3"))
                                {
                                    General.InregistreazaLogarea(0, txtPan1.Text, "Parola expirata");
                                    txtRas = "Parola a expirat";
                                    schimba = true; //Radu 06.01.2020
                                }
                                else
                                {
                                    General.InregistreazaLogarea(1, txtPan1.Text);
                                    Session["SecApp"] = "OK";
                                    if (Dami.ValoareParam("2FA", "") == "1")
                                    {
                                        string ras = General.CreazaCod2FA();
                                        if (ras != "")
                                        {
                                            txtRas = ras;
                                        }
                                        else
                                            Response.Redirect("~/Pagini/CodConfirmare.aspx", false);
                                    }
                                    else
                                    {
                                        //Florin 2019.09.19
                                        if (Constante.tipBD == 2)
                                            General.ExecutaNonQuery("alter session set nls_date_format='DD-MM-RRRR'", null);

                                        Response.Redirect("~/Pagini/MainPage.aspx", false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            General.InregistreazaLogarea(0, txtPan1.Text, "Utilizator inexistent in aplicatie");
                            txtRas = "Utilizator inexistent in aplicatie! Contactati administratorul de sistem!";
                        }
                        break;
                    case 4:                     //este inactivat in AD
                        General.InregistreazaLogarea(0, txtPan1.Text, "Cont inactiv (inactivat in AD)");
                        txtRas = "Contul este inactivat ! Contactati administratorul de sistem.";
                        break;
                    case 5:
                        General.InregistreazaLogarea(0, txtPan1.Text, "Angajatul asociat acestui utilizator este inactiv sau suspendat!");
                        txtRas = "Angajatul asociat acestui utilizator este inactiv sau suspendat! Contactati administratorul de sistem.";
                        break;
                    case 6: //Radu 28.01.2020
                        General.InregistreazaLogarea(0, txtPan1.Text, "Acest utilizator are alocati mai multi angajati!");                        
                        txtRas = "Acest utilizator are alocati mai multi angajati! Va rugam contactati administratorul de sistem!";
                        break;
                }

                if (txtRas != "" && cuMesaj)
                {
                    MessageBox.Show(txtRas, MessageBox.icoWarning);

                    //Radu 06.01.2020
                    if (schimba)
                    {
                        Session["SecApp"] = "OK";
                        Response.Redirect("Pagini/SchimbaParola.aspx", false);
                        Session["SchimbaParolaMesaj"] = txtRas;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return txtRas;
        }

        private string VerificaUser(string utilizator, string parola, bool dinButon)
        {

            //stare = -1            domeniul nu este configurat
            //stare = 0             user inexistent
            //stare = 1             user existent parola eronata
            //stare = 2             valid si blocat
            //stare = 3             valid si activ
            //stare = 4             disable in AD
            //stare = 5             user suspendat sau inactiv

            //optimizare
            //adugam si limba utilizatorului in cazul in care este unul valid dar nu indeplineste restul de conditii pt a putea afisa measjele in limba lui

            string stare = "0";
            string idLimba = General.Nz(Session["IdLimba"],"RO").ToString();
            string marca = "-99";

            try
            {
                //Florin 2019.07.23
                //s-a adaugat conditia cu tip 5
                string tipVerif = Dami.ValoareParam("TipVerificareAccesApp");
                if (tipVerif == "") tipVerif = "1";

                //Radu 09.10.2019 - daca VerificaUser este apelata de la butonul de logare, se ignora SSO (cazul 3 devine 1, cazul 4 devine 2)
                if (dinButon)
                {
                    if (tipVerif == "3")
                        tipVerif = "1";
                    if (tipVerif == "4")
                        tipVerif = "2";
                }

                switch (tipVerif)
                {
                    case "1":
                    case "3":
                    case "5":
                        {
                            //Florin 2019.08.14
                            //am adaugat conditia ca deblocarea sa se faca doar pt angajati activi   ->   AND (SELECT COUNT(*) FROM F100 Y WHERE Y.F10003=X.F10003 AND Y.F10025 IN (0,999)) > 0
                            //s-a schimbat diferenta cu comparatie deoarece se adauga un mimut suplimentar
                            // Mihnea - modificare pt implementare deblocare dupa x minute
                            string strsql = @"SELECT F10003,F70103, F70114, ""Mail"", ""IdLimba"" ,
                                                            CASE WHEN 
                                                            COALESCE(CAST((SELECT COALESCE(""Valoare"",'0') FROM ""tblParametrii"" WHERE ""Nume"" = 'NrMinuteDeblocareParola' ) AS INT), 0) >0  AND (SELECT COUNT(*) FROM F100 Y WHERE Y.F10003=X.F10003 AND Y.F10025 IN (0,999)) > 0
                                                            THEN 
                                                            CASE WHEN
                                                            {1} >= 
                                                            COALESCE(CAST((SELECT COALESCE(""Valoare"",'0') FROM ""tblParametrii"" WHERE ""Nume"" = 'NrMinuteDeblocareParola' ) AS INT), 0) 
                                                            THEN 1 ELSE 0 END
                                                            ELSE 0 
                                                            END DEBLOCARE  
                                                            FROM USERS X WHERE UPPER(F70104)='" + utilizator.ToUpper() + "'";
                            string op = "+";
                            //string exp = @"CASE WHEN (F70111=1 AND DATEADD(d,f70121,f70122) < GETDATE()) THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                            string exp = $@" DATEDIFF(MINUTE,COALESCE([BLOCKTIME],convert(datetime,'1980-01-01 00:00:00',120)),GETDATE()) ";
                            if (Constante.tipBD == 2)
                            {
                                op = "||";
                                exp = $@" ( (sysdate - COALESCE(BLOCKTIME,to_date('01/01/1980 00:00:00','dd/MM/rrrr hh24:mi:ss')) )*24*60 ) ";
                            }
                            strsql = string.Format(strsql, op, exp);

                            DataRow dr = General.IncarcaDR(strsql,null);
                            if (dr != null)
                                marca = (dr["F10003"] as int? ?? -99).ToString();

                            if (dr == null)
                                stare = "0" + idLimba;
                            else
                            {
                                if (dr != null && dr["IdLimba"] != null && dr["IdLimba"].ToString() != "") idLimba = dr["IdLimba"].ToString();
                                if (dr["F70114"].ToString() == "1" && dr["DEBLOCARE"].ToString() == "0")
                                    stare = "2" + idLimba;
                                else
                                {
                                    if(dr["F70114"].ToString() == "1" && dr["DEBLOCARE"].ToString() == "1")
                                    {
                                        General.ExecutaNonQuery("UPDATE USERS SET F70114=0 WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                                    }

                                    if (tipVerif == "3" || tipVerif == "5")
                                    {
                                        stare = "3" + idLimba;
                                    }
                                    else
                                    {
                                        CriptDecript prc = new CriptDecript();
                                        string parolaDinBaza = prc.EncryptString(Constante.cheieCriptare, dr["F70103"].ToString(), 2);

                                        if (parolaDinBaza != parola)
                                            stare = "1" + idLimba;
                                        else
                                            stare = "3" + idLimba;
                                    }
                                }
                            }
                        }
                        break;
                    case "2":
                    case "4":
                        {
                            string ADDom = Dami.ValoareParam("ADNumeDomeniu");
                            string ADuser = Dami.ValoareParam("ADUser");
                            string ADpass = Dami.ValoareParam("ADPass");

                            if (ADDom == "" || ADuser == "" || ADpass == "")
                                stare = "-1" + idLimba;
                            else
                            {
                                PrincipalContext context = new PrincipalContext(ContextType.Domain, ADDom, ADuser, ADpass);
                                UserPrincipal usr = UserPrincipal.FindByIdentity(context, utilizator);

                                if (usr == null)
                                    stare = "0" + idLimba;
                                else
                                {
                                    DataRow dr = General.IncarcaDR("SELECT \"IdLimba\", F10003 FROM USERS WHERE UPPER(F70104)=@1", new string[] { General.Strip(utilizator.ToUpper()) });
                                    if (dr != null && dr["IdLimba"] != null && dr["IdLimba"].ToString() != "")
                                    {
                                        idLimba = dr["IdLimba"].ToString();
                                        marca = (dr["F10003"] as int? ?? -99).ToString();
                                    }

                                    if (usr.Enabled == false)
                                        stare = "4" + idLimba;
                                    else
                                    {
                                        if (usr.IsAccountLockedOut() == true)
                                            stare = "2" + idLimba;
                                        else
                                        {
                                            if (tipVerif == "4")
                                            {
                                                stare = "3" + idLimba;
                                            }
                                            else
                                            {
                                                if (!(context.ValidateCredentials(utilizator, parola)))
                                                    stare = "1" + idLimba;
                                                else
                                                    stare = "3" + idLimba;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "6":
                        //intra direct in aplicatie
                        stare = "3" + idLimba;
                        break;
                }

                //Mihnea adaugat blocare pt useri inactivi / suspendati
                //Radu 14.01.2020 - verificarea trebuie facuta pentru toate tipurile de conectare + inlocuire conditie F10025 cu F10022 si F10023 + parametru

                //1 - blocare utilizatori inactivi la logare
                //2- blocare utilizatori inactivi si suspendati la logare
                
                string dezactiv = Dami.ValoareParam("DezactivareUtilizatori");
                if (dezactiv.Length <= 0) dezactiv = "0";
                switch (dezactiv)
                {
                    case "1":
                        string inactiv = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN {General.TruncateDate("F10023")} < {General.CurrentDate(true)} OR {General.TruncateDate("F10022")} > {General.CurrentDate(true)} THEN 1 ELSE 0 END INACTIV FROM F100 WHERE F10003 = @1", new object[] { marca }), "").ToString();

                        if (inactiv == "1")
                        {
                            stare = "5" + idLimba;
                        }
                        break;
                    case "2":
                        string suspendatinactiv = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN ({General.TruncateDate("F10023")} < {General.CurrentDate(true)} OR {General.TruncateDate("F10022")} > {General.CurrentDate(true)} ) OR ( {General.TruncateDate("F100922")} <= {General.CurrentDate(true)} AND NOT ( {General.TruncateDate("F100924")} <= {General.CurrentDate(true)} ) ) THEN 1 ELSE 0 END SI FROM F100 WHERE F10003 = @1", new object[] { marca }), "").ToString();

                        if (suspendatinactiv == "1")
                        {
                            stare = "5" + idLimba;
                        }
                        break;               
                }

                //Radu 28.01.2020 - daca numele utilizatorului apare de mai multe ori in USERS, accesul sa fie blocat
                DataTable dtMultiUser = General.IncarcaDT("SELECT COUNT(*) FROM USERS WHERE UPPER(F70104) = '" + utilizator.ToUpper() + "'", null);
                if (dtMultiUser != null && dtMultiUser.Rows.Count > 0 && dtMultiUser.Rows[0][0] != null && dtMultiUser.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtMultiUser.Rows[0][0].ToString()) > 1)
                {
                    stare = "6" + idLimba;
                }

 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return stare;
        }

        public void InitSessionVariables()
        {
            try
            {
                //Lista cu toate variabilele de sesiune din aplicatie
                Session["SecApp"] = "NO";
                Session["tblParam"] = "";
                Session["tmpMeniu"] = "";
                Session["Titlu"] = "";
                Session["Sablon_Tabela"] = "";
                Session["Sablon_CheiePrimara"] = "";
                Session["Sablon_TipActiune"] = "";
                Session["InformatiaCurenta"] = "";
                Session["SecuritateCamp"] = "";
                Session["SecuritateDate"] = "";
                Session["PaginaWeb"] = "";
                Session["TipNotificare"] = "";
                Session["Date_Profile"] = "";
                Session["ProfilId"] = "";
                Session["Profil_Original"] = "";
                Session["AbsDescValues"] = "";

                Session["Cereri_Absente_Absente"] = "";
                Session["Cereri_Absente_Angajati"] = "";


                Session["formatDataSistem"] = CultureInfo.CurrentCulture;

                Session["IdLimba"] = "RO";
                Session["IdLimba_Veche"] = "RO";

                Session["User"] = "";
                Session["UserId"] = -99;
                Session["User_NumeComplet"] = "";
                Session["User_Marca"] = -99;
                Session["User_IdDept"] = -99;
                Session["User_Dept"] = "";
                Session["User_CNP"] = "";

                Session["EsteAdmin"] = 0;
                Session["SchimbaParola"] = 0;
                Session["EsteInGrup99"] = 0;
                Session["ParolaComplexa"] = 0;

                Session["PrimaIntrare"] = 0;

                Session["IdMaxValue"] = 1;

                Session["Ptj_IstoricVal"] = "";

                Session["Filtru_PontajulEchipei"] = "";

                Session["IdClient"] = "1";

                Session["PontajulAreCC"] = "0";

                //nu se initializeaza; este pusa aici pentru documentare
                //Session["NrIncercari"] = 0;

                Session["SecAuditSelect"] = "0";
                Session["SecCriptare"] = "0";

                //Florin 2019.06.04
                Session["MP_NuPermiteCNPInvalid"] = "1";
                Session["MP_AreContract"] = "0";
                Session["MP_DataSfarsit36"] = "01/01/2100";


                //Florin 2019.06.21
                Session["EsteTactil"] = "0";
                Session["TimeOutSecunde"] = 0;

                //Florin 2019.07.15
                Session["Filtru_ActeAditionale"] = "{}";

                //Florin 2019.07.17
                Session["Filtru_CereriAbs"] = "";

                //Florin 2019.07.19
                Session["Ptj_DataBlocare"] = "22001231";

                //Florin 2019.10.16
                Session["Json_Programe"] = "[]";

                //Florin 2020.01.03
                Session["Eval_tblCategorieObiective"] = null;


                string strSql = @"SELECT ""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", ""Criptat"" FROM ""tblParametrii""
                                UNION
                                SELECT 'AnLucru', CAST(F01011 AS varchar(10)), '', 1, 0 FROM F010
                                UNION
                                SELECT 'LunaLucru', CAST(F01012 AS varchar(10)), '', 1, 0 FROM F010";

                Session["tblParam"] = General.IncarcaDT(strSql, null);
                Session["IdClient"] = Convert.ToInt32(Dami.ValoareParam("IdClient", "1"));
                Session["PontajulAreCC"] = General.Nz(Dami.ValoareParam("PontajulAreCC"),"0");


                //Florin 2018.11.13
                if (HttpContext.Current != null && Session["tblParam"] != null)
                {
                    DataTable dt = Session["tblParam"] as DataTable;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] arr1 = dt.Select("Nume='SecAuditSelect'");
                        if (arr1.Count() > 0)
                            Session["SecAuditSelect"] = arr1[0];

                        DataRow[] arr2 = dt.Select("Nume='SecCriptare'");
                        if (arr2.Count() > 0)
                            Session["SecCriptare"] = arr2[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public bool IsReCaptchValid()
        {
            var result = false;

            try
            {
                var captchaResponse = Request.Form["g-recaptcha-response"];
                var secretKey = Dami.ValoareParam("Captcha_Secret");
                var apiUrl = "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
                var requestUri = string.Format(apiUrl, secretKey, captchaResponse);
                var request = (HttpWebRequest)WebRequest.Create(requestUri);

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        JObject jResponse = JObject.Parse(stream.ReadToEnd());
                        var isSuccess = jResponse.Value<bool>("success");
                        result = (isSuccess) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, "General", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return result;
        }

        protected void lnkOut_Click(object sender, EventArgs e)
        {
            try
            {
                General.SignOut();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}