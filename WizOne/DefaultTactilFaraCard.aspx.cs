using DevExpress.Web;
using ProceseSec;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne
{
    public partial class DefaultTactilFaraCard : System.Web.UI.Page
    {
        //static string arrIncercari = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                

                if (!IsPostBack)
                {
                    Session.Clear();
                    General.InitSessionVariables();
                    Session["Tactil_PIN"] = null;

                }

                string tip = Dami.ValoareParam("TipInfoChiosc", "0");

                Session["TipInfoChiosc"] = tip;

                if (tip == "2")
                {
                    btn0.ClientVisible = false;
                    btn1.ClientVisible = false;
                    btn2.ClientVisible = false;
                    btn3.ClientVisible = false;
                    btn4.ClientVisible = false;
                    btn5.ClientVisible = false;
                    btn6.ClientVisible = false;
                    btn7.ClientVisible = false;
                    btn8.ClientVisible = false;
                    btn9.ClientVisible = false;
                }

                Session["IdLimba"] = Dami.ValoareParam("LimbaStart");
                if (General.VarSession("IdLimba").ToString() == "") Session["IdLimba"] = "RO";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public string PermisiuneConectare()
        {
            string rez = "";

            try
            {
                string txt = "In aceasta zi conectarea nu este permisa. Va rugam sa luati legatura cu administratorul de sistem.";
                string msg = Dami.ValoareParam("MesajConectareNepermisa", txt);

                string interval = "," + Dami.ValoareParam("IntervalConectareNepermisa", "") + ",";
                if (interval != ",," && interval.IndexOf("," + DateTime.Now.Day + ",") >= 0)
                    return msg;

                string blocat = General.Nz(General.ExecutaScalar("SELECT F002541 FROM F002", null),"").ToString();
                if (blocat.ToUpper() == "BLOCAT")
                    return msg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }



        private void VerificareCartela()
        {
            //codul de mai jos este aproximativ la fel cu cel din default.aspx -> functia Verifica cu deosebirea
            //ca s-a adaugat F100Cartele si filtrarea se face dupa cartela

            try
            {

                string interval = Dami.ValoareParam("IntervalConectareInfoChiosc", "5");
                string terminal = Constante.idTerminal, cartela = "";
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");

                DataTable dtInfo = General.IncarcaDT("SELECT * FROM \"tblInfoChiosc\" WHERE \"IdTerminal\" = '" + terminal + "' AND TIME >= " + (Constante.tipBD == 1 ? "(select dateadd(ss,-" + interval + ",getdate()))" : "(select sysdate  - interval '" + interval + "' second  from dual)") , null);
                if (dtInfo != null && dtInfo.Rows.Count > 0)
                {
                    cartela = dtInfo.Rows[0]["Cartela"].ToString();
                }
                else
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu a fost gasita nicio cartela valida!\n Va rugam apropiati din nou cartela de cititor " + (tip == "1" ? ", tastati codul PIN" : "") + " si apasati butonul Logare!");
                    return;
                }            


                //Radu 20.03.2019
                string valMax = Dami.ValoareParam("LungimeMaximaCodCartela","10");
                int max = 10;
                if (valMax.Length > 0)
                    max = Convert.ToInt32(valMax);            

                cartela = cartela.PadLeft(max, '0');

                int lung = Convert.ToInt32(General.Nz(Dami.ValoareParam("LungimeCartela","0"),"0"));
                if (lung > 0 && cartela.Length >= lung) cartela = cartela.Substring(cartela.Length - lung);

                string sir = "";
                for (int i = 1; i <= max; i++)
                    sir += "0";

                string camp = "RIGHT(convert(varchar, '" + sir + "')+convert(varchar, ISNULL(D.Cartela,''))," + max +")";
                if (Constante.tipBD == 2)
                    camp = " LPAD(D.\"Cartela\", " + cartela.Length + ", '0')";

                string exp = $@"CASE WHEN DATEADD(d,COALESCE((SELECT CASE WHEN COALESCE(""Valoare"",99999) = 0 THEN 99999 ELSE COALESCE(""Valoare"",99999) END FROM ""tblParametrii"" WHERE ""Nume""='Parola_VechimeMaxima'),30),(SELECT TOP 1 ""Data"" FROM ""ParoleUtilizatorIstoric"" WHERE ""IdUser""=A.F70102 ORDER BY ""Data"" Desc)) < GetDate() THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";
                if (Constante.tipBD == 2)
                    exp = $@"CASE WHEN (COALESCE((SELECT CASE WHEN COALESCE(""Valoare"",30) = 0 THEN 99999 ELSE COALESCE(""Valoare"",99999) END FROM ""tblParametrii"" WHERE ""Nume""='Parola_VechimeMaxima'),30) + (SELECT TOP 1 ""Data"" FROM ""ParoleUtilizatorIstoric"" WHERE ""IdUser""=A.F70102 ORDER BY ""Data"" Desc)) < SYSDATE THEN 1 ELSE 0 END AS ""ParolaExpirata"" ";

                string strSql = $@"SELECT a.F70102 AS ""UtilizatorId"", a.F70104 AS ""Utilizator"", COALESCE(a.""F70113"",0) AS ""ResetareParola"", F70112 AS ""ParolaComplexa"",
                                    CRP.F10003 AS ""Marca"", CRP.F10007 AS ""IdDept"",C.F00608 AS ""Dept"",CRP.F10017 AS CNP,
                                    CRP.F10008 As ""Nume"", CRP.F10009 AS ""Prenume"", CRP.F10022 AS F10022,
                                    (SELECT MAX(""Tema"") FROM ""tblConfigUsers"" WHERE F70102=A.F70102) AS ""Tema"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=0)=0 THEN 0 ELSE 1 END AS ""EsteAdmin"",
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=99)=0 THEN 0 ELSE 1 END AS ""EsteInGrup99"",
                                    ""PINInfoChiosc""
                                    FROM USERS A
                                    LEFT JOIN F100 CRP ON A.F10003=CRP.F10003
                                    LEFT JOIN F006 C ON CRP.F10007=C.F00607
                                    INNER JOIN ""F100Cartele"" D ON A.F10003=D.F10003
                                    WHERE {camp}='{cartela}' AND D.""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= D.""DataSfarsit"" ";



                DataRow drUsr = General.IncarcaDR(strSql, null);

                if (drUsr == null)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Utilizator inexistent!");
                    return;
                }

                if (tip == "1")
                {//verificare PIN
                    string pin = Session["Tactil_PIN"].ToString();
                    Session["Tactil_PIN"] = null;
                    CriptDecript prc = new CriptDecript();
                    string pinBD = prc.EncryptString(Constante.cheieCriptare, General.Nz(drUsr["PINInfoChiosc"], "").ToString(), 2);
                    if (pinBD != pin)
                    {
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("PIN incorect!");
                        return;
                    }
                }

                //Radu 14.01.2020
                //1 - blocare utilizatori inactivi la logare
                //2- blocare utilizatori inactivi si suspendati la logare

                string dezactiv = Dami.ValoareParam("DezactivareUtilizatori");
                if (dezactiv.Length <= 0) dezactiv = "0";
                bool err = false;
                switch (dezactiv)
                {
                    case "1":
                        string inactiv = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN  {General.TruncateDate("F10023")} < {General.CurrentDate(true)} OR {General.TruncateDate("F10022")} > {General.CurrentDate(true)} THEN 1 ELSE 0 END INACTIV FROM F100 WHERE F10003 = @1", new object[] { Convert.ToInt32(General.Nz(drUsr["Marca"], -99)) }), "").ToString();

                        if (inactiv == "1")
                        {
                            General.InregistreazaLogarea(0, "", "Angajatul asociat acestui utilizator este inactiv!");
                            pnlCtl.JSProperties["cpAlertMessage"] = "Angajatul asociat acestui utilizator este inactiv! Contactati administratorul de sistem!";
                           
                            err = true;
                        }
                        break;
                    case "2":
                        string suspendatinactiv = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN ({General.TruncateDate("F10023")} < {General.CurrentDate(true)} OR {General.TruncateDate("F10022")} > {General.CurrentDate(true)} ) OR ( {General.TruncateDate("F100922")} <= {General.CurrentDate(true)} AND NOT ( {General.TruncateDate("F100924")} <= {General.CurrentDate(true)} ) ) THEN 1 ELSE 0 END SI FROM F100 WHERE F10003 = @1", new object[] { Convert.ToInt32(General.Nz(drUsr["Marca"], -99)) }), "").ToString();

                        if (suspendatinactiv == "1")
                        {
                            General.InregistreazaLogarea(0, "", "Angajatul asociat acestui utilizator este suspendat!");
                            pnlCtl.JSProperties["cpAlertMessage"] = "Angajatul asociat acestui utilizator este suspendat! Contactati administratorul de sistem!";
                           
                            err = true;
                        }
                        break;
                }


                if (!err)
                {
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

                        General.SetTheme();
                        General.InregistreazaLogarea(1, "");
                        Session["SecApp"] = "OK_Tactil";

                        //Florin 2022.01.06     #1065
                        Session["TimeOutSecundePrint"] = Dami.TimeOutSecundePrint();

                        Response.RedirectLocation = System.Web.VirtualPathUtility.ToAbsolute("~/Tactil/Main");
                    }
                    else
                    {
                        General.InregistreazaLogarea(0, "", "Utilizator inexistent in aplicatie");
                        MessageBox.Show("Utilizator inexistent in aplicatie! Contactati administratorul de sistem!", MessageBox.icoWarning);
                        
                    }
                }

               

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string param = e.Parameter;
                if (param == "btnLog")
                {
                    string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                    if (Session["Tactil_PIN"] != null || tip == "2")                                     
                        VerificareCartela();  
                    else
                    {   
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati introdus PIN-ul!");
                        return;                                                               
                    }    
                }
                else
                {
                    if (Session["Tactil_PIN"] != null)
                    {
                        string pin = Session["Tactil_PIN"].ToString();
                        Session["Tactil_PIN"] = pin + param;
                    }
                    else
                        Session["Tactil_PIN"] = param;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}