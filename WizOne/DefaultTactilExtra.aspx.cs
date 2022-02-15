using DevExpress.Web;
using DevExpress.XtraBars.Docking2010.Views.NativeMdi;
using DevExpress.XtraRichEdit.API.Native;
using ProceseSec;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne
{
    public partial class DefaultTactilExtra : System.Web.UI.Page
    {
        //static string arrIncercari = "";
        int tipInfoChiosc = 0;

        protected void Page_Init(object sender, EventArgs e)
        {
            //SetFocus(txtPan1);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {             
                if (!IsPostBack)
                {
                    //Session.Clear();
                    //General.InitSessionVariables();

                    AscundeButoane();
                }

                //Florin 2021.02.18  #795
                tipInfoChiosc = Convert.ToInt32(Dami.ValoareParam("TipInfoChiosc", "0"));
                Session["TipInfoChiosc"] = tipInfoChiosc;
                if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == Convert.ToInt32(IdClienti.Clienti.TMK))
                {
                    tipInfoChiosc = 3;
                    Session["TipInfoChiosc"] = tipInfoChiosc;
                }
                    
                Session["IdLimba"] = Dami.ValoareParam("LimbaStart");
                if (General.VarSession("IdLimba").ToString() == "") Session["IdLimba"] = "RO";

                //txtPan1.Attributes.Add("autofocus", "autofocus");
                //txtPan1.Focus();

                if (!IsPostBack)
                    txtPan1_TextChanged(null, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //public string PermisiuneConectare()
        //{
        //    string rez = "";

        //    try
        //    {
        //        string txt = "In aceasta zi conectarea nu este permisa. Va rugam sa luati legatura cu administratorul de sistem.";
        //        string msg = Dami.ValoareParam("MesajConectareNepermisa", txt);

        //        string interval = "," + Dami.ValoareParam("IntervalConectareNepermisa", "") + ",";
        //        if (interval != ",," && interval.IndexOf("," + DateTime.Now.Day + ",") >= 0)
        //            return msg;

        //        string blocat = General.Nz(General.ExecutaScalar("SELECT F002541 FROM F002", null),"").ToString();
        //        if (blocat.ToUpper() == "BLOCAT")
        //            return msg;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return rez;
        //}

        protected void txtPan1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //Radu 20.03.2019
                //string valMax = Dami.ValoareParam("LungimeMaximaCodCartela","10");
                //int max = 10;
                //if (valMax.Length > 0)
                //    max = Convert.ToInt32(valMax);

                ////Radu 20.05.2019 - anumite cititoare returneaza un numar variabil de caractere
                ////if (txtPan1.Value.Trim().Length >= max)
                ////{
                //if (txtPan1.Value.Trim().Length > max)            
                //{
                //    MessageBox.Show("Cod invalid! Va rugam apropiati din nou cardul de cititor", MessageBox.icoWarning, "");
                //    txtPan1.Value = null;
                //    txtPan1.Focus();
                //}
                //else
                {
                    //string msg = PermisiuneConectare();
                    //if (msg != "")
                    //{
                    //    MessageBox.Show(msg, MessageBox.icoWarning, "");
                    //    txtPan1.Focus();
                    //}
                    //else
                    {
                        ////Florin 2021.02.18
                        ////Radu 18.05.2020
                        ////if (Session["TipInfoChiosc"] == null)
                        ////{
                        ////    MessageBox.Show("Nu s-a putut realiza conexiunea cu baza de date!", MessageBox.icoError, "");
                        ////    return;
                        ////}

                        ////string tip = Session["TipInfoChiosc"].ToString();
                        ////if (tip == "3")
                        if (tipInfoChiosc == 3)
                        {
                            btn0.ClientVisible = true;
                            btn1.ClientVisible = true;
                            btn2.ClientVisible = true;
                            btn3.ClientVisible = true;
                            btn4.ClientVisible = true;
                            btn5.ClientVisible = true;
                            btn6.ClientVisible = true;
                            btn7.ClientVisible = true;
                            btn8.ClientVisible = true;
                            btn9.ClientVisible = true;
                            btnLog.ClientVisible = true;
                            //panouExt.Visible = false;

                        }
                        else
                            VerificaCartela();
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void VerificaCartela(string pin = "")
        {
            //codul de mai jos este aproximativ la fel cu cel din default.aspx -> functia Verifica cu deosebirea
            //ca s-a adaugat F100Cartele si filtrarea se face dupa cartela

            try
            {
                //Radu 20.03.2019
                string valMax = Dami.ValoareParam("LungimeMaximaCodCartela","10");
                int max = 10;
                if (valMax.Length > 0)
                    max = Convert.ToInt32(valMax);

                string cartela = Session["Tactil_Cartela"].ToString(); //txtPan1.Value; 

                int lung = Convert.ToInt32(General.Nz(Dami.ValoareParam("LungimeCartela","0"),"0"));
                if (lung > 0 && cartela.Length >= lung) cartela = cartela.Substring(cartela.Length - lung);

                cartela = cartela.PadLeft(max, '0');

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
                //DataRow drUsr = General.IncarcaDR(strSql, new object[] { cartela });

                if (drUsr == null || drUsr.ItemArray.Length == 0)
                {
                    MessageBox.Show("Utilizator inexistent!", MessageBox.icoError, "Atentie !");
                    System.Threading.Thread.Sleep(2000);
                    AscundeButoane();                    
                    Response.Redirect("~/DefaultTactil", false);
                    return;
                }

                //Florin 2021.02.18
                //string tip = Session["TipInfoChiosc"].ToString();
                //if (tip == "3")
                if (tipInfoChiosc == 3)
                {//verificare PIN       
                    CriptDecript prc = new CriptDecript();
                    string pinBD = prc.EncryptString(Constante.cheieCriptare, General.Nz(drUsr["PINInfoChiosc"], "").ToString(), 2);
                    if (pinBD != pin)
                    {
                        MessageBox.Show("PIN incorect!", MessageBox.icoError, "Atentie !");
                        System.Threading.Thread.Sleep(2000);
                        AscundeButoane();                        
                        Response.Redirect("~/DefaultTactil", false);
                        return;
                    }
                    AscundeButoane();
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
                            General.InregistreazaLogarea(0, Session["Tactil_Cartela"].ToString() /*txtPan1.Value*/, "Angajatul asociat acestui utilizator este inactiv!");
                            MessageBox.Show("Angajatul asociat acestui utilizator este inactiv! Contactati administratorul de sistem!", MessageBox.icoWarning);
                            System.Threading.Thread.Sleep(2000);
                            //txtPan1.Focus();
                            err = true;

                            Response.Redirect("~/DefaultTactil", false);
                            return;
                        }
                        break;
                    case "2":
                        string suspendatinactiv = General.Nz(General.ExecutaScalar($@"SELECT CASE WHEN ({General.TruncateDate("F10023")} < {General.CurrentDate(true)} OR {General.TruncateDate("F10022")} > {General.CurrentDate(true)} ) OR ( {General.TruncateDate("F100922")} <= {General.CurrentDate(true)} AND NOT ( {General.TruncateDate("F100924")} <= {General.CurrentDate(true)} ) ) THEN 1 ELSE 0 END SI FROM F100 WHERE F10003 = @1", new object[] { Convert.ToInt32(General.Nz(drUsr["Marca"], -99)) }), "").ToString();

                        if (suspendatinactiv == "1")
                        {
                            General.InregistreazaLogarea(0, Session["Tactil_Cartela"].ToString() /*txtPan1.Value*/, "Angajatul asociat acestui utilizator este suspendat!");
                            MessageBox.Show("Angajatul asociat acestui utilizator este suspendat! Contactati administratorul de sistem!", MessageBox.icoWarning);
                            System.Threading.Thread.Sleep(2000);
                            //txtPan1.Focus();
                            err = true;

                            Response.Redirect("~/DefaultTactil", false);
                            return;
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
                        General.InregistreazaLogarea(1, Session["Tactil_Cartela"].ToString() /*txtPan1.Value*/);
                        Session["SecApp"] = "OK_Tactil";

                        //Florin 2022.01.06     #1065
                        Session["TimeOutSecundePrint"] = Dami.TimeOutSecunde("TimeOutSecundePrint");

                        Response.Redirect("~/Tactil/Main", false);
                    }
                    else
                    {
                        General.InregistreazaLogarea(0, Session["Tactil_Cartela"].ToString() /*txtPan1.Value*/, "Utilizator inexistent in aplicatie");
                        MessageBox.Show("Utilizator inexistent in aplicatie! Contactati administratorul de sistem!", MessageBox.icoWarning);
                        System.Threading.Thread.Sleep(2000);
                        AscundeButoane();

                        Response.Redirect("~/DefaultTactil", false);
                    }
                }

                //txtPan1.Value = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnLog_Click(object sender, EventArgs e)
        {
            string PIN = "";
            if (hfPIN.Contains("PIN")) 
                PIN = General.Nz(hfPIN["PIN"], "").ToString();

            if (PIN.Length > 0)
            {
                VerificaCartela(PIN);
                hfPIN.Set("PIN", null);
            }
            else
            {
                MessageBox.Show(Dami.TraduCuvant("Nu ati introdus PIN-ul!"), MessageBox.icoError, "Atentie !");
                System.Threading.Thread.Sleep(2000);
                //return;
                Response.Redirect("~/DefaultTactil", false);
            }
        }

        private void AscundeButoane()
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
            btnLog.ClientVisible = false;
            //panouExt.Visible = true;
            //txtPan1.Value = null;
            //txtPan1.Focus();
        }

        protected void Resetare(object sender, EventArgs e)
        {
            AscundeButoane();
            hfPIN.Set("PIN", null);
        }



        //      <div class="outer">
        //          <div class="inner">
        //              Pentru accesarea aplicatiei va rugam apropiati cardul de cititor
        //          </div>
        //          <input type = "text" id="txtPan1" name="txtPan1" runat="server" autofocus="autofocus" autocomplete="off" class="hide" maxlength="15" onserverchange="txtPan1_TextChanged" onblur="this.focus()"  />
        //</div>



        //<dx:ASPxButton ID = "btn7" ClientInstanceName="btn7" ClientIDMode="Static" ClientVisible="false" TabIndex="7" runat="server" Height="30px" Text="7" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider"  RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn7" >
        //        <ClientSideEvents Click = "function(s, e) { RetinePIN(7); }" />
        //    <Paddings PaddingBottom="10px" PaddingRight="20px" />
        //</dx:ASPxButton>

    }
}