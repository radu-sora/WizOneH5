using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne
{
    public partial class DefaultTactil : System.Web.UI.Page
    {
        static string arrIncercari = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txtPan1.Focus();

                if (!IsPostBack)
                {
                    Session.Clear();
                    General.InitSessionVariables();
                }

                Session["IdLimba"] = Dami.ValoareParam("LimbaStart");
                if (General.VarSession("IdLimba").ToString() == "") Session["IdLimba"] = "RO";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex,MessageBox.icoError, "Atentie !");
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

        protected void txtPan1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //Radu 20.03.2019
                string valMax = Dami.ValoareParam("LungimeMaximaCodCartela");
                int max = 10;
                if (valMax.Length > 0)
                    max = Convert.ToInt32(valMax);

                if (txtPan1.Value.Trim().Length >= max)
                {
                    if (txtPan1.Value.Trim().Length > max)
                    {
                        MessageBox.Show("Cod invalid! Va rugam apropiati din nou cardul de cititor", MessageBox.icoWarning, "Atentie !");
                        txtPan1.Value = null;
                        txtPan1.Focus();
                    }
                    string msg = PermisiuneConectare();
                    if (msg != "")
                    {
                        MessageBox.Show(msg, MessageBox.icoWarning, "Atentie !");
                        txtPan1.Focus();
                    }
                    else
                        VerificaCartela();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void VerificaCartela()
        {
            //codul de mai jos este aproximativ la fel cu cel din default.aspx -> functia Verifica cu deosebirea
            //ca s-a adaugat F100Cartele si filtrarea se face dupa cartela

            try
            {
                //Radu 20.03.2019
                string valMax = Dami.ValoareParam("LungimeMaximaCodCartela");
                int max = 10;
                if (valMax.Length > 0)
                    max = Convert.ToInt32(valMax);

                string cartela = txtPan1.Value;
                //int lung = Convert.ToInt32(General.Nz(Dami.ValoareParam("LungimeCartela","0"),"0"));
                //if (lung > 0 && cartela.Length >= lung) cartela = cartela.Substring(cartela.Length - lung);
                string camp = "RIGHT(convert(varchar, '0000000000')+convert(varchar, ISNULL(D.Cartela,''))," + max +")";
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
                                    CASE WHEN (SELECT COUNT(*) FROM ""relGrupUser"" WHERE ""IdUser""=A.F70102 AND ""IdGrup""=99)=0 THEN 0 ELSE 1 END AS ""EsteInGrup99""
                                    FROM USERS A
                                    LEFT JOIN F100 CRP ON A.F10003=CRP.F10003
                                    LEFT JOIN F006 C ON CRP.F10007=C.F00607
                                    INNER JOIN F100Cartele D ON A.F10003=D.F10003
                                    WHERE {camp}='{cartela}' AND D.DataInceput <= {General.CurrentDate()} AND {General.CurrentDate()} <= D.DataSfarsit";



                DataRow drUsr = General.IncarcaDR(strSql, null);
                //DataRow drUsr = General.IncarcaDR(strSql, new object[] { cartela });
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
                    General.InregistreazaLogarea(1, txtPan1.Value);
                    Session["SecApp"] = "OK_Tactil";

                    DataTable dt = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TimeoutSecunde'", null);
                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                        Session["TimeOutSecunde"] = Convert.ToInt32(dt.Rows[0][0].ToString());
                    Response.Redirect("~/Tactil/Main.aspx", false);
                }
                else
                {
                    General.InregistreazaLogarea(0, txtPan1.Value, "Utilizator inexistent in aplicatie");
                    MessageBox.Show("Utilizator inexistent in aplicatie! Contactati administratorul de sistem!", MessageBox.icoWarning);
                    txtPan1.Focus();
                }

                txtPan1.Value = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}