using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.IO;
using System.Data;
using System.Web.Resources;
using System.Threading;
using System.Globalization;
using System.DirectoryServices.AccountManagement;
using System.Diagnostics;
using ProceseSec;
using System.Text.RegularExpressions;

namespace WizOne
{
    public partial class Default : System.Web.UI.Page
    {
        //static int nrIncercari = 0;
        //int paramNrIncercari = 3;
        static string arrIncercari = "";

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
                
                string tipVerif = General.Nz(Dami.ValoareParam("TipVerificareAccesApp"), "1").ToString();
                if (tipVerif == "3" || tipVerif == "4")
                {
                    string usrTMP = System.Web.HttpContext.Current.User.Identity.Name.ToString();
                    int poz = usrTMP.IndexOf(@"\");
                    if (poz > 0) usrTMP = usrTMP.Remove(0, poz + 1);

                    Verifica(usrTMP, "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                Verifica(General.Strip(txtPan1.Text), txtPan2.Text);
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

        private bool Blocheaza(string utilizator)
        {
            bool rasp = false;

            try
            {
                arrIncercari += utilizator + ";";
                //if (nrIncercari >= Convert.ToInt32(Dami.ValoareParam("NrIncercari", "3")))
                if (Regex.Matches(arrIncercari, utilizator + ";").Count >= Convert.ToInt32(Dami.ValoareParam("Parola_NrIncercSuccNer", "3")))
                {
                    bool esteBlocat = BlocheazaUser(utilizator);
                    if (esteBlocat)
                    {
                        MessageBox.Show("Contul este blocat ! Contactati administratorul de sistem!", MessageBox.icoWarning);
                    }
                    else
                    {
                        MessageBox.Show("Utilizator/Parola gresita ! Contactati administratorul de sistem!", MessageBox.icoWarning);
                    }
                }
                else
                {
                    MessageBox.Show("Utilizator/Parola gresita ! Contactati administratorul de sistem!", MessageBox.icoWarning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rasp;
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
                    General.ExecutaNonQuery("UPDATE USERS SET F70114=1 WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
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

        protected void Verifica(string utilizator, string parola)
        {
            try
            {
                string rasp = VerificaUser(utilizator, parola);
                if (rasp.Length > 1)
                {
                    Session["IdLimba"] = rasp.Substring(rasp.Length-2, 2);                    
                    //rasp = rasp.Substring(0, 1);
                    rasp = rasp.ToUpper().Replace(General.Nz(Session["IdLimba"],"RO").ToString(), "");
                }

                switch (Convert.ToInt32(rasp))
                {
                    case -1:                     //nume Domeniu nu este configurat
                        General.InregistreazaLogarea(0, txtPan1.Text, "Numele de domeniu nu este configurat");
                        MessageBox.Show("Numele de domeniu nu este configurat !", MessageBox.icoWarning);
                        break;
                    case 0:                     //user inexistent
                        General.InregistreazaLogarea(0, txtPan1.Text, "Utilizator inexistent");
                        MessageBox.Show("Utilizator inexistent! Contactati administratorul de sistem!", MessageBox.icoWarning);
                        break;
                    case 1:                     //user existent parola eronata
                        General.InregistreazaLogarea(0, txtPan1.Text, "Introducerea unei parole gresite");
                        //nrIncercari++;
                        Blocheaza(utilizator);
                        break;
                    case 2:                     //valid si blocat
                        General.InregistreazaLogarea(0, txtPan1.Text, "Cont blocat");
                        MessageBox.Show("Contul este blocat ! Contactati administratorul de sistem", MessageBox.icoWarning);
                        break;
                    case 3:                     //valid si activ
                        string op = "+";
                        //string exp = @"CASE WHEN (F70111=1 AND DATEADD(d,f70121,f70122) < GETDATE()) THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                        string exp = $@"CASE WHEN DATEADD(d,COALESCE((SELECT CASE WHEN COALESCE(""Valoare"",99999) = 0 THEN 99999 ELSE COALESCE(""Valoare"",99999) END FROM ""tblParametrii"" WHERE ""Nume""='Parola_VechimeMaxima'),30),(SELECT TOP 1 ""Data"" FROM ""ParoleUtilizatorIstoric"" WHERE ""IdUser""=A.F70102 ORDER BY ""Data"" Desc)) < GetDate() THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                        if (Constante.tipBD == 2)
                        {
                            op = "||";
                            //exp = @"CASE WHEN (F70111=1 AND (f70121+f70122) < SYSDATE) THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
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


                        //CASE WHEN COALESCE(b.F10008,'') = '' THEN a.""NumeComplet"" ELSE (b.F10008 {0} ' ' {0} b.F10009) END AS ""NumeComplet"",
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

                        //DataRow drUsr = General.IncarcaDR(strSql, new string[] { utilizator });
                        DataRow drUsr = General.IncarcaDR(strSql, null);
                        if (drUsr != null)
                        {
                            var ert = General.Nz(drUsr["Nume"], "").ToString() + " " + General.Nz(drUsr["Prenume"], "").ToString();

                            Session["UserId"] = Convert.ToInt32(drUsr["UtilizatorId"]);
                            Session["User"] = drUsr["Utilizator"].ToString();
                            Session["User_Marca"] = Convert.ToInt32(General.Nz(drUsr["Marca"], -99));
                            Session["User_IdDept"] = Convert.ToInt32(General.Nz(drUsr["IdDept"], -99));
                            Session["User_Dept"] = drUsr["Dept"].ToString();
                            Session["User_NumeComplet"] = General.Nz(drUsr["Nume"],"").ToString() + " " + General.Nz(drUsr["Prenume"],"").ToString();
                            Session["User_CNP"] = drUsr["CNP"].ToString();
                            Session["EsteAdmin"] = Convert.ToInt32(General.Nz(drUsr["EsteAdmin"], 0));
                            Session["EsteInGrup99"]= Convert.ToInt32(General.Nz(drUsr["EsteInGrup99"], 0));
                            Session["ParolaComplexa"] = Convert.ToInt32(General.Nz(drUsr["ParolaComplexa"], 0));
                            Session["PrimaIntrare"] = General.Nz(drUsr["F70105"],0);

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
                                    MessageBox.Show("Parola a expirat", MessageBox.icoWarning,"","Pagini/SchimbaParola.aspx");
                                }
                                else
                                {
                                    General.InregistreazaLogarea(1, txtPan1.Text);
                                    Session["SecApp"] = "OK";
                                    //Response.Redirect(GetRouteUrl("MainPage", null), false);
                                    if (Dami.ValoareParam("2FA", "") == "1")
                                    {
                                        string ras = General.CreazaCod2FA();
                                        if (ras != "")
                                            MessageBox.Show(ras, MessageBox.icoError, "");
                                        else
                                            Response.Redirect("~/Pagini/CodConfirmare.aspx", false);
                                    }
                                    else
                                        Response.Redirect("~/Pagini/MainPage.aspx", false);
                                }
                            }
                        }
                        else
                        {
                            General.InregistreazaLogarea(0, txtPan1.Text, "Utilizator inexistent in aplicatie");
                            MessageBox.Show("Utilizator inexistent in aplicatie! Contactati administratorul de sistem!", MessageBox.icoWarning);
                        }
                        break;
                    case 4:                     //este inactivat in AD
                        General.InregistreazaLogarea(0, txtPan1.Text, "Cont inactiv (inactivat in AD)");
                        MessageBox.Show("Contul este inactivat ! Contactati administratorul de sistem.", MessageBox.icoWarning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private string VerificaUser(string utilizator, string parola)
        {

            //stare = -1            domeniul nu este configurat
            //stare = 0             user inexistent
            //stare = 1             user existent parola eronata
            //stare = 2             valid si blocat
            //stare = 3             valid si activ
            //stare = 4             disable in AD


            //optimizare
            //adugam si limba utilizatorului in cazul in care este unul valid dar nu indeplineste restul de conditii pt a putea afisa measjele in limba lui

            string stare = "0";
            string idLimba = General.Nz(Session["IdLimba"],"RO").ToString();

            try
            {
                string tipVerif = Dami.ValoareParam("TipVerificareAccesApp");
                if (tipVerif == "") tipVerif = "1";

                switch (tipVerif)
                {
                    case "1":
                    case "3":
                        {
                            DataRow dr = General.IncarcaDR(@"SELECT F70103, F70114, ""Mail"", ""IdLimba"" FROM USERS WHERE UPPER(F70104)=@1", new string[] { utilizator.ToUpper() });
                            //DataRow dr = General.IncarcaDR(@"SELECT F70103, F70114, ""Mail"", ""IdLimba"" FROM USERS WHERE UPPER(F70104)='" + utilizator.ToUpper() + "'", null);
                            if (dr == null)
                                stare = "0" + idLimba;
                            else
                            {
                                if (dr != null && dr["IdLimba"] != null && dr["IdLimba"].ToString() != "") idLimba = dr["IdLimba"].ToString();
                                if (dr["F70114"].ToString() == "1")
                                    stare = "2" + idLimba;
                                else
                                {
                                    //ProceseSec.CriptDecript sec = new ProceseSec.CriptDecript();
                                    //if (sec.EncryptString("WizOne-2015",entUsr.FirstOrDefault().F70103.ToString(),2) != parola) return 1;
                                    if (tipVerif == "3")
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
                                    DataRow dr = General.IncarcaDR("SELECT \"IdLimba\" FROM USERS WHERE UPPER(F70104)=@1", new string[] { General.Strip(utilizator.ToUpper()) });
                                    if (dr != null && dr["IdLimba"] != null && dr["IdLimba"].ToString() != "") idLimba = dr["IdLimba"].ToString();

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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return stare;
        }

        //private void SetTheme()
        //{
        //    try
        //    {
        //        string tema = (General.ExecutaScalar(@"SELECT ""Tema"" FROM ""tblConfigUsers"" WHERE F70102=@1", new string[] { Session["UserId"].ToString() }) ?? Constante.DefaultTheme).ToString();

        //        if (tema.ToString() == "") tema = Constante.DefaultTheme;

        //        HttpCookie cookie = Request.Cookies[Constante.CurrentThemeCookieKey];
        //        if (cookie == null)
        //        {
        //            cookie = new HttpCookie(Constante.CurrentThemeCookieKey);
        //        }

        //        cookie.Value = tema.ToString();
        //        Response.Cookies.Add(cookie);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void InregistreazaLogarea(int succes, string motiv="")
        //{
        //    try
        //    {
        //        if (Dami.ValoareParam("SecAuditAuth", "0") == "1")
        //        {
        //            string computerName = "";
        //            try
        //            {
        //                string[] computer_name = System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
        //                String ecn = System.Environment.MachineName;
        //                computerName = computer_name[0].ToString();
        //            }
        //            catch (Exception) { }

        //            DataTable dt = General.IncarcaDT(@"SELECT TOP 0 * FROM ""WT_USERS"" ", null);
        //            DataRow dr = dt.NewRow();
        //            dr["USER_WIN"] = "";
        //            dr["COMPUTER_NAME"] = computerName;
        //            dr["USER_WS"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;
        //            dr["DATA"] = DateTime.Now;
        //            dr["TABELA"] = "USERS";
        //            dr["COD_OP"] = "S";
        //            dr["NUME_CAMP"] = "F70102";
        //            dr["COL_ID1"] = "F70102";
        //            dr["VAL_ID1"] = HttpContext.Current.Session["UserId"] ?? DBNull.Value;
        //            dr["VAL_OLD"] = txtPan1.Text;
        //            //dr["VAL_NEW"] = "";
        //            dr["LOGARE_REUSITA"] = succes;
        //            dr["MOTIV"] = motiv;
        //            dt.Rows.Add(dr);
        //            General.SalveazaDate(dt, "WT_USERS");
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


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

                Session["PontajulAreCC"] = "";

                //nu se initializeaza; este pusa aici pentru documentare
                //Session["NrIncercari"] = 0;

                Session["SecAuditSelect"] = "0";
                Session["SecCriptare"] = "0";



                string ti = "nvarchar";
                if (Constante.tipBD == 2) ti = "varchar2";

                string strSql = @"SELECT ""Nume"", ""Valoare"", ""Explicatie"", ""IdModul"", ""Criptat"" FROM ""tblParametrii""
                                UNION
                                SELECT 'AnLucru', CAST(F01011 AS {0}(10)), '', 1, 0 FROM F010
                                UNION
                                SELECT 'LunaLucru', CAST(F01012 AS {0}(10)), '', 1, 0 FROM F010";
                strSql = string.Format(strSql, ti);

                Session["tblParam"] = General.IncarcaDT(strSql, null);
                Session["IdClient"] = Convert.ToInt32(Dami.ValoareParam("IdClient", "1"));
                Session["PontajulAreCC"] = Dami.ValoareParam("PontajulAreCC");


                //Florin 2018.11.13
                if (HttpContext.Current != null && Session["tblParam"] != null)
                {
                    DataTable dt = Session["tblParam"] as DataTable;
                    if (dt != null)
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


    }
}