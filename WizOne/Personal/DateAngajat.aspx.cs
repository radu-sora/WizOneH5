using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;
using DevExpress.Data;
using System.Web.Hosting;
using System.Drawing;
using System.Diagnostics;

namespace WizOne.Personal
{
    public partial class DateAngajat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                btnSave.Text = Dami.TraduCuvant(btnSave.Text);
                btnExit.Text = Dami.TraduCuvant(btnExit.Text);

                #endregion

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string marca = Session["Marca"] as string;
                if (ds == null && marca == null)
                {
                    marca = NextMarca().ToString();
                    Session["Marca"] = marca;

                    Session["esteNou"] = "true";

                    Initializare(ref ds);
                }

                if (Session["esteNou"] == null || Session["esteNou"].ToString().Length <= 0 || Session["esteNou"].ToString() == "false")
                {
                    DataTable dtDept = General.IncarcaDT("SELECT F00608 FROM F006 WHERE F00607 = " + ds.Tables[0].Rows[0]["F10007"].ToString(), null);
                    lblDateAngajat.Text = ds.Tables[0].Rows[0]["F10008"].ToString() + " " + ds.Tables[0].Rows[0]["F10009"].ToString() + ", " + Dami.TraduCuvant("Marca") + ": " + ds.Tables[0].Rows[0]["F10003"].ToString()
                                        + ", " + Dami.TraduCuvant("Departament") + ": " + (dtDept != null && dtDept.Rows.Count > 0 ? dtDept.Rows[0]["F00608"].ToString() : "");
       
                }


                string sql = "SELECT DISTINCT B.\"Denumire\", B.\"Pagina\", B.\"Ordine\" FROM \"MP_tblTaburi\" B ";

                if (General.VarSession("EsteAdmin").ToString() == "0")
                    sql += "JOIN \"relGrupMeniu2\" C ON (-1) * B.\"IdAuto\" = C.\"IdMeniu\" AND  C.\"IdGrup\" IN (SELECT \"IdGrup\" FROM \"relGrupUser\" WHERE \"IdUser\" = " + Session["UserId"].ToString() + ") ";

                sql += "ORDER BY B.\"Ordine\"";

                DataTable dt = General.IncarcaDT(sql, null);

                int nrTaburi = dt.Rows.Count;
                bool multirand = false;
                if (nrTaburi > 10)
                    multirand = true;

                List<string> lista = ListaSecuritate();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Name = dt.Rows[i]["Pagina"].ToString();
                    tabPage.Text = Dami.TraduCuvant(dt.Rows[i]["Denumire"].ToString());

                    if (multirand && i >= nrTaburi / 2)
                    {
                        tabPage.NewLine = true;
                        multirand = false;
                    }

                    Control ctrl = new Control();
                    if (File.Exists(HostingEnvironment.MapPath("~/Personal/" + dt.Rows[i]["Pagina"].ToString() + ".ascx")))
                    {
                        ctrl = this.LoadControl(dt.Rows[i]["Pagina"].ToString() + ".ascx");
                        bool vizibil = true, blocat = false;
                        Securitate(dt.Rows[i]["Denumire"].ToString().Replace(" ", ""), out vizibil, out blocat);

                        if (blocat)
                        {
                            for (int j = 0; j < ctrl.Controls.Count; j++)
                            {
                                if (ctrl.Controls[j].GetType() == typeof(DevExpress.Web.ASPxGridView))
                                {
                                    ASPxGridView gr = ctrl.Controls[j] as ASPxGridView;
                                    gr.Enabled = false;
                                }
                                if (ctrl.Controls[j].GetType() == typeof(DevExpress.Web.ASPxCallbackPanel))
                                {
                                    ASPxCallbackPanel cb = ctrl.Controls[j] as ASPxCallbackPanel;
                                    cb.Enabled = false;
                                }
                                if (ctrl.Controls[j].GetType() == typeof(DevExpress.Web.ASPxButton))
                                {
                                    ASPxButton btn = ctrl.Controls[j] as ASPxButton;
                                    btn.Enabled = false;
                                }
                            }
                        }

                        if (lista != null && lista.Count > 0)
                        {
                            foreach (string elem in lista)
                            {
                                bool vizibilElem = true, blocatElem = false;
                                Securitate(elem, out vizibilElem, out blocatElem);
                                string[] param = elem.Split('_');
                                WebControl ctl = ctrl.FindControl(param[0]) as WebControl;
                                if (ctl != null)
                                {
                                    ctl.Visible = vizibilElem;
                                    ctl.Enabled = !blocatElem;
                                }
                            }
                        }

                        tabPage.Controls.Add(ctrl);
                        tabPage.Visible = vizibil;
                    }

                    if (Dami.ValoareParam("ValidariPersonal") == "1")
                    {
                        string[] lst = new string[5] { "DateIdentificare", "Contract", "Structura", "Adresa", "Documente" };
                        if (lst.Contains(dt.Rows[i]["Pagina"].ToString()))
                            tabPage.TabStyle.BackColor = Color.FromArgb(255, 255, 179, 128);
                    }

                    this.ASPxPageControl2.TabPages.Add(tabPage);
                }

                this.ASPxPageControl2.Attributes.Add("oncontextmenu", "ctx(this,event); return false;");

                if (Session["MP_Avans_Tab"] != null)
                {
                    for (int i = 0; i < this.ASPxPageControl2.TabPages.Count; i++)
                        if (this.ASPxPageControl2.TabPages[i].Name == Session["MP_Avans_Tab"].ToString())
                        {
                            this.ASPxPageControl2.ActiveTabIndex = i;
                            break;
                        }
                    Session["MP_Avans_Tab"] = null;
                }

                if (!IsPostBack)
                {
                    Session["AdresaCompusa"] = null;
                    Session["AdresaSelectata"] = null;
                }

                Session["MP_NuPermiteCNPInvalid"] = General.Nz(General.ExecutaScalar(@"SELECT COALESCE(""Valoare"",'1') FROM ""tblParametrii"" WHERE ""Nume"" = 'NuPermiteCNPInvalid' ", null),1);
                Session["MP_AreContract"] = General.Nz(General.ExecutaScalar(@"SELECT COUNT(*) FROM F095 WHERE F09503=@1 ", new object[] { Session["Marca"]  }), 1);
                string sql36 = @"SELECT CONVERT(nvarchar(20),DATEADD(m,36,MIN(F09505)),103) FROM F095 WHERE F09503=@1 AND F09504=@2 AND COALESCE(F09511,'Determinat')='Determinat' ";
                if (Constante.tipBD == 2)
                    sql36 = "SELECT ADD_MONTHS(F09505, 36) FROM F095 WHERE F09503=@1 AND F09504=@2 AND COALESCE(F09511,'Determinat')='Determinat' ";
                Session["MP_DataSfarsit36"] = General.Nz(General.ExecutaScalar(sql36, new object[] { Session["Marca"], ds.Tables[1].Rows[0]["F100985"] }), "01/01/2100").ToString();
                var ert = Session["MP_DataSfarsit36"];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                AdaugaValorile();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                if (Dami.ValoareParam("ValidariPersonal") == "1")
                {
                    string mesaj = "", mesajDI = "", mesajDA = "", mesajStr = "", mesajAdr = "", mesajDoc = "";

                    if (ds.Tables[0].Rows[0]["F10017"] == null || ds.Tables[0].Rows[0]["F10017"].ToString().Length <= 0)
                        mesaj += " - CNP/CUI" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10008"] == null || ds.Tables[0].Rows[0]["F10008"].ToString().Length <= 0)
                        mesaj += " - nume" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10009"] == null || ds.Tables[0].Rows[0]["F10009"].ToString().Length <= 0)
                        mesaj += " - prenume" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10003"] == null || ds.Tables[0].Rows[0]["F10003"].ToString().Length <= 0)
                        mesaj += " - marca" + Environment.NewLine;
                    if (mesaj.Length > 0)
                    {
                        mesajDI = "Date identificare: " + Environment.NewLine + mesaj + Environment.NewLine;
                        mesaj = "";
                    }

                    if (ds.Tables[0].Rows[0]["F100985"] == null || ds.Tables[0].Rows[0]["F100985"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100985"].ToString() == "0")
                        mesaj += " - nr. ctr. intern" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100986"] == null || ds.Tables[0].Rows[0]["F100986"].ToString().Length <= 0
                        || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100986"].ToString()) == new DateTime(2100, 1, 1) || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100986"].ToString()) == new DateTime(1900, 1, 1))
                        mesaj += " - data ctr. intern" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100984"] == null || ds.Tables[0].Rows[0]["F100984"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100984"].ToString() == "0")
                        mesaj += " - tip contract munca" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F1009741"] == null || ds.Tables[0].Rows[0]["F1009741"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F1009741"].ToString() == "0")
                        mesaj += " - durata contract" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100699"] == null || ds.Tables[0].Rows[0]["F100699"].ToString().Length <= 0 || Convert.ToDouble(ds.Tables[0].Rows[0]["F100699"].ToString()) == 0.0)
                        mesaj += " - salariu" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10010"] == null || ds.Tables[0].Rows[0]["F10010"].ToString().Length <= 0)
                        mesaj += " - tip angajat" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10043"] == null || ds.Tables[0].Rows[0]["F10043"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10043"].ToString() == "0")
                        mesaj += " - timp partial" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100926"] == null || ds.Tables[0].Rows[0]["F100926"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100926"].ToString() == "0")
                        mesaj += " - tip norma" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100973"] == null || ds.Tables[0].Rows[0]["F100973"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100973"].ToString() == "0")
                        mesaj += " - norma" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100927"] == null || ds.Tables[0].Rows[0]["F100927"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100927"].ToString() == "0")
                        mesaj += " - durata timp munca" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100928"] == null || ds.Tables[0].Rows[0]["F100928"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100928"].ToString() == "0")
                        mesaj += " - repartizare timp munca" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10010"] != null && ds.Tables[0].Rows[0]["F10010"].ToString() != "0")
                    {
                        if (ds.Tables[0].Rows[0]["F100939"] == null || ds.Tables[0].Rows[0]["F100939"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100939"].ToString() == "0")
                            mesaj += " - interval repartizare timp munca" + Environment.NewLine;
                        if (ds.Tables[0].Rows[0]["F100939"] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) > 1)
                        {
                            if (ds.Tables[0].Rows[0]["F100964"] == null || ds.Tables[0].Rows[0]["F100964"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100964"].ToString() == "0")
                                mesaj += " - nr ore pe luna/saptamana" + Environment.NewLine;
                        }
                    }
                    if (ds.Tables[0].Rows[0]["F10098"] == null || ds.Tables[0].Rows[0]["F10098"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10098"].ToString() == "0")
                        mesaj += " - COR" + Environment.NewLine;
                    //if (ds.Tables[0].Rows[0]["F10071"] == null || ds.Tables[0].Rows[0]["F10071"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10071"].ToString() == "0")
                    //    mesaj += " - functie" + Environment.NewLine;
                    if ((ds.Tables[0].Rows[0]["F1001063"] == null || ds.Tables[0].Rows[0]["F1001063"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F1001063"].ToString() == "0") 
                        && (ds.Tables[0].Rows[0]["F100975"] == null || ds.Tables[0].Rows[0]["F100975"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100975"].ToString() == "0"))
                        mesaj += " - perioada de proba (zile lucratoare sau zile calendaristice)" + Environment.NewLine; 
                    if (ds.Tables[0].Rows[0]["F1009742"] == null || ds.Tables[0].Rows[0]["F1009742"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F1009742"].ToString() == "0")
                        mesaj += " - nr zile preaviz demisie" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100931"] == null || ds.Tables[0].Rows[0]["F100931"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100931"].ToString() == "0")
                        mesaj += " - nr zile preaviz concediere" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100642"] == null || ds.Tables[0].Rows[0]["F100642"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100642"].ToString() == "0")
                        mesaj += " - zile CO cuvenite cf. grila" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10022"] == null || ds.Tables[0].Rows[0]["F10022"].ToString().Length <= 0
                        || Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()) == new DateTime(2100, 1, 1) || Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()) == new DateTime(1900, 1, 1))
                        mesaj += " - data angajarii" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100933"] == null || ds.Tables[0].Rows[0]["F100933"].ToString().Length <= 0)
                        mesaj += " - de la data" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100934"] == null || ds.Tables[0].Rows[0]["F100934"].ToString().Length <= 0)
                        mesaj += " - la data" + Environment.NewLine;
                    
                    if (mesaj.Length > 0)
                    {
                        mesajDA = "Date angajare: " + Environment.NewLine + mesaj + Environment.NewLine;
                        mesaj = "";
                    }

                    

                    //if (ds.Tables[0].Rows[0]["F10079"] == null || ds.Tables[0].Rows[0]["F10079"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10079"].ToString() == "0")
                    //    mesaj += " - punct de lucru" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10007"] == null || ds.Tables[0].Rows[0]["F10007"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10007"].ToString() == "0")
                        mesaj += " - structura" + Environment.NewLine;
                    if (mesaj.Length > 0)
                    {
                        mesajStr = "Structura: " + Environment.NewLine + mesaj + Environment.NewLine;
                        mesaj = "";
                    }

                    if (ds.Tables["F100Adrese"] == null || ds.Tables["F100Adrese"].Rows.Count <= 0)
                    {
                        mesajAdr = "Adresa" + Environment.NewLine + Environment.NewLine;
                        mesaj = "";
                    }

                    if (ds.Tables[0].Rows[0]["F100983"] == null || ds.Tables[0].Rows[0]["F100983"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100983"].ToString() == "0")
                        mesaj += " - tip document (BI/CI)" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10052"] == null || ds.Tables[0].Rows[0]["F10052"].ToString().Length <= 0)
                        mesaj += " - serie si numar" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100521"] == null || ds.Tables[0].Rows[0]["F100521"].ToString().Length <= 0)
                        mesaj += " - emis de" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100522"] == null || ds.Tables[0].Rows[0]["F100522"].ToString().Length <= 0
                        || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100522"].ToString()) == new DateTime(2100, 1, 1) || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100522"].ToString()) == new DateTime(1900, 1, 1))
                        mesaj += " - data eliberarii" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100963"] == null || ds.Tables[0].Rows[0]["F100963"].ToString().Length <= 0)
                        mesaj += " - data expirarii BI/CI" + Environment.NewLine;
                    if (mesaj.Length > 0)
                    {
                        mesajDoc = "Documente: " + Environment.NewLine + mesaj + Environment.NewLine;
                        mesaj = "";
                    }

                    if (mesajDI.Length > 0 || mesajDA.Length > 0 || mesajStr.Length > 0 || mesajAdr.Length > 0 || mesajDoc.Length > 0)
                    {
                        MessageBox.Show("Nu ati completat: " + Environment.NewLine + Environment.NewLine + mesajDI + mesajDA + mesajStr + mesajAdr + mesajDoc, MessageBox.icoError, "Atentie!");
                        return;
                    }         
                }

                //verificarea se face pe partea de client
                //string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NuPermiteCNPInvalid'";
                //DataTable dt = General.IncarcaDT(sql, null);
                //if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                //    if (!General.VerificaCNP(ds.Tables[1].Rows[0]["F10017"].ToString()))
                //    {
                //        MessageBox.Show("CNP invalid!", MessageBox.icoError);
                //        return;
                //    }

                //DateIdentificare dateId = new DateIdentificare();
                //int varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                //if (varsta < 16)
                //{
                //    MessageBox.Show(Dami.TraduCuvant("Nu puteti angaja o persoana cu varsta mai mica de 16 ani!"), MessageBox.icoError);
                //    return;
                //}

                int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"]));
                if (varsta >= 16 && varsta < 18 && Convert.ToInt32(ds.Tables[0].Rows[0]["F10043"].ToString()) > 6)
                {
                    MessageBox.Show(Dami.TraduCuvant("Timp partial invalid (max 6 pentru minori peste 16 ani)!"));
                    return;
                }

                if (varsta >= 16 && varsta < 18 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100964"].ToString()) > 30)
                {
                    MessageBox.Show(Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 30 pentru minori peste 16 ani)!"));
                    return;
                }


                //Florin 2018-10-30
                //calculam CO daca se insereaza un angajat
                bool calcCO = false;
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    int val = 1;
                    string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TermenDepasireRevisal'";
                    DataTable dt = General.IncarcaDT(sql, null);
                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 )
                        val = Convert.ToInt32(dt.Rows[0][0].ToString());
                    if (val == 1)
                    {
                        Contract ctr = new Contract();
                        DateTime dataRevisal = ctr.SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
                        if (dataRevisal.Date < DateTime.Now.Date)
                        {
                            MessageBox.Show("Termen depunere Revisal depasit!", MessageBox.icoError);
                            //return;
                        }
                    }

                    //Florin 2018.11.23
                    //daca este nou verificam ca data angajarii sa fie mai mare decat luna de lucru
                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["F10022"] != DBNull.Value)
                    {
                        try
                        {
                            var ert = Convert.ToInt32(General.Nz(ds.Tables[1].Rows[0]["F10025"], 0));
                            if (Convert.ToInt32(General.Nz(ds.Tables[1].Rows[0]["F10025"],0)) == 999  && Convert.ToDateTime(ds.Tables[1].Rows[0]["F10022"]) < General.DamiDataLucru().AddMonths(1))
                            {
                                MessageBox.Show("Data angajarii este mai mica decat luna de salarizare", MessageBox.icoError, "Atentie !");
                                return;
                            }
                        }
                        catch (Exception) { }
                    }

                    if (Session["MP_Candidat"] != null && Session["MP_Candidat"].ToString() == "1")
                    {
                        ds.Tables[1].Rows[0]["F10025"] = 900;
                    }
                    else
                    {
                        if (Convert.ToDateTime(ds.Tables[1].Rows[0]["F10022"]) >= General.DamiDataLucru().AddMonths(1))
                            ds.Tables[1].Rows[0]["F10025"] = 999;
                        else
                            ds.Tables[1].Rows[0]["F10025"] = 0;
                    }


                    //Florin 2019.06.24
                    //Mihnea 2019.06.13
                    int tip_pass = 0;
                    tip_pass = Convert.ToInt32(Dami.ValoareParam("Parola_creare_user", "0"));

                    string pass = General.Nz(ds.Tables[1].Rows[0]["F10017"],"").ToString();
                    ProceseSec.CriptDecript cls = new ProceseSec.CriptDecript();

                    switch (tip_pass)
                    {
                        case 0:
                            //parola este cnp-ul
                            break;
                        case 1:
                            if (ds.Tables[1].Rows[0]["F10017"].ToString().Length >= 4)
                                pass = ds.Tables[1].Rows[0]["F10017"].ToString().Substring(ds.Tables[1].Rows[0]["F10017"].ToString().Length - 4);
                            break;
                        default:
                            //parola este cnp-ul
                            break;
                    }

                    if (pass == "") pass = "0";
                    string userNume = "";
                    if (General.Nz(ds.Tables[1].Rows[0]["F10008"], "").ToString() != "" || General.Nz(ds.Tables[1].Rows[0]["F10009"], "").ToString() != "")
                        userNume = General.Nz(ds.Tables[1].Rows[0]["F10009"], "").ToString().Replace("-","").Replace(" ","") + "." + General.Nz(ds.Tables[1].Rows[0]["F10008"], "").ToString().Replace("-", "").Replace(" ", "");

                    //daca numele de utilizator exista, adaugam un 2 in coada
                    if (Convert.ToInt32(General.ExecutaScalar("SELECT COUNT(*) FROM USERS WHERE F70104=@1", new object[] { userNume })) != 0)
                        userNume += "2";


                    General.ExecutaNonQuery($@"
                        BEGIN
                            INSERT INTO USERS (F70101, F70102, F70103, F70104, F10003, USER_NO, TIME) VALUES(701, (SELECT MAX(COALESCE(F70102,0)) + 1 FROM USERS), @1, @2, @3, @4, {General.CurrentDate()})
                            INSERT INTO relGrupUser(IdGrup, IdUser) VALUES(1, (SELECT MAX(COALESCE(F70102,1)) FROM USERS));
                        END;", new object[] { cls.EncryptString(Constante.cheieCriptare, pass, Constante.ENCRYPT), userNume, Session["Marca"], Session["UserId"] });



                    #region OLD

                    ////Mihnea 2019.06.13
                    //if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    //{
                    //    int tip_pass = -1;
                    //    sql = "SELECT COALESCE(\"Valoare\", '-1') FROM \"tblParametrii\" WHERE \"Nume\" = 'Parola_creare_user'";
                    //    dt = General.IncarcaDT(sql, null);
                    //    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                    //    {
                    //        try
                    //        {
                    //            tip_pass = Convert.ToInt32(dt.Rows[0][0].ToString());
                    //        }
                    //        catch
                    //        {
                    //            tip_pass = -1;
                    //        }

                    //    }
                    //    if (val != -1)
                    //    {
                    //        string pass = string.Empty;
                    //        ProceseSec.CriptDecript cls = new ProceseSec.CriptDecript();

                    //        switch (tip_pass)
                    //        {
                    //            case 1:
                    //                if (ds.Tables[1].Rows[0]["F10017"].ToString().Length >= 4)
                    //                {
                    //                    pass = ds.Tables[1].Rows[0]["F10017"].ToString().Substring(ds.Tables[1].Rows[0]["F10017"].ToString().Length - 4);
                    //                }
                    //                else
                    //                {
                    //                    pass = ds.Tables[1].Rows[0]["F10017"].ToString();
                    //                }

                    //                General.ExecutaNonQuery($@"INSERT INTO USERS (F70101, F70102, F70103, F70104, F10003) VALUES (701, (SELECT MAX(F70102) + 1 FROM USERS),@1, @2, @3)", new object[] { cls.EncryptString("WizOne2016", pass, 1), Session["Marca"].ToString(), Session["Marca"].ToString() });

                    //                break;
                    //            case 0:

                    //                General.ExecutaNonQuery($@"INSERT INTO USERS (F70101, F70102, F70103, F70104, F10003) VALUES (701, (SELECT MAX(F70102) + 1 FROM USERS),@1, @2, @3)", new object[] { cls.EncryptString("WizOne2016", "0", 1), Session["Marca"].ToString(), Session["Marca"].ToString() });

                    //                break;
                    //            default:
                    //                // am parametrul completat cu o valoare non null ,dar nu am tratat-o

                    //                break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //nu exista parametrul in tblparametrii sau e null
                    //    }

                    //}

                    #endregion


                    InserareAngajat(Session["Marca"].ToString(), ds.Tables[1], ds.Tables[2]);
                    Session["esteNou"] = "false";

                    try
                    {
                        int an = DateTime.Now.Year;
                        if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["F10022"] != DBNull.Value) an = Convert.ToDateTime(ds.Tables[1].Rows[0]["F10022"]).Year;
                        General.CalculCO(an, Convert.ToInt32(General.Nz(Session["Marca"], -99)));

                        //Asignam angajatul pe post
                        General.ModificaFunctieAngajat(Convert.ToInt32(General.Nz(Session["Marca"], -99)), Convert.ToInt32(General.Nz(ds.Tables[1].Rows[0]["F10071"], -99)), Convert.ToDateTime(ds.Tables[1].Rows[0]["F10022"]), Convert.ToDateTime(ds.Tables[1].Rows[0]["F10023"]));

                    }
                    catch (Exception) { }
                }
                else
                {
                    DateTime dataModif = Convert.ToDateTime(General.ExecutaScalar("SELECT F10023 FROM F100 WHERE F10003=" + General.Nz(Session["Marca"], -99), null));
                    if (dataModif != Convert.ToDateTime(ds.Tables[1].Rows[0]["F10023"]))
                        calcCO = true;
                }


                for (int i = 1; i < ds.Tables.Count; i++)
                {//Radu 10.06.2019
                    if (ds.Tables[i].TableName == "Admin_Medicina" || ds.Tables[i].TableName == "Admin_Sanctiuni")
                        SalvareSpeciala(ds.Tables[i].TableName);
                    else
                        General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                }


                //Florin 2018-10-30
                //calculam CO daca se modifica data plecare
                if (calcCO)
                {
                    try
                    {
                        int an = DateTime.Now.Year;
                        if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["F10023"] != DBNull.Value) an = Convert.ToDateTime(ds.Tables[1].Rows[0]["F10023"]).Year;
                        General.CalculCO(an, Convert.ToInt32(General.Nz(Session["Marca"], -99)));
                    }
                    catch (Exception) { }
                }


                //Florin 2018.11.22
                //trimitem la lista de angajati
                Response.Redirect("~/Personal/Lista.aspx", false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void SalvareSpeciala(string tabela)
        {
            string sql = "SELECT * FROM \"" + tabela + "\"";
            //DataTable dtGen = new DataTable();
            //dtGen = General.IncarcaDT(sql, null);
            //dtGen.TableName = tabela;
            //dtGen.PrimaryKey = new DataColumn[] { dtGen.Columns["IdAuto"] };

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable dt = ds.Tables[tabela] as DataTable;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i].RowState == DataRowState.Deleted)
                {
                    sql = "DELETE FROM \"" + tabela + "\" WHERE \"IdAuto\" = " + dt.Rows[i]["IdAuto", DataRowVersion.Original].ToString();
                    General.ExecutaNonQuery(sql, null);
                    sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto", DataRowVersion.Original].ToString();
                    General.ExecutaNonQuery(sql, null);
                    continue;
                }

                if (Convert.ToInt32(dt.Rows[i]["IdAuto"].ToString()) < 1000000)
                {//UPDATE 
                    DataRow dr = dt.Select("IdAuto = " + dt.Rows[i]["IdAuto"].ToString()).FirstOrDefault();
                    string sir = "";
                    sql = "UPDATE \"" + tabela + "\" SET ";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (!dt.Columns[k].AutoIncrement)
                        {
                            string val = "";
                            if (dr[k].GetType() == typeof(int) || dr[k].GetType() == typeof(decimal))
                                val = dr[k] == null ? "null" : dr[k].ToString();
                            if (dr[k].GetType() == typeof(string))
                                val = dr[k] == null ? "null" : "'" + dr[k].ToString() + "'";
                            if (dr[k].GetType() == typeof(DateTime))
                                val = dr[k] == null ? "null" : General.ToDataUniv((DateTime)dr[k]);
                            if (val.Length <= 0)
                                val = "''";
                            sir += ",\"" + dt.Columns[k].ColumnName + "\" = " + val;
                        }
                    }
                    sql += sir.Substring(1) + " WHERE \"IdAuto\" = " + dt.Rows[i]["IdAuto"].ToString();
                    General.ExecutaNonQuery(sql, null);
           
                    int idAuto = Convert.ToInt32(dt.Rows[i]["IdAuto"].ToString());
                    if (tabela == "Admin_Medicina")
                    {
                        Dictionary<int, Personal.Medicina.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, Personal.Medicina.metaUploadFile>;
                        if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                        {
                            sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                            General.ExecutaNonQuery(sql, null);
                            General.IncarcaFisier(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                        }
                    }
                    if (tabela == "Admin_Sanctiuni")
                    {
                        Dictionary<int, Personal.Sanctiuni.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, Personal.Sanctiuni.metaUploadFile>;
                        if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                        {
                            sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                            General.ExecutaNonQuery(sql, null);
                            General.IncarcaFisier(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                        }
                    }

;
                }
                else
                {//INSERT
                    DataRow dr = dt.Select("IdAuto = " + dt.Rows[i]["IdAuto"].ToString()).FirstOrDefault();
                    sql = "INSERT INTO \"" + tabela + "\" (";
                    string sir = "";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (!dt.Columns[k].AutoIncrement)
                            sir += ",\"" + dt.Columns[k].ColumnName + "\"";
                    }

                    string rez1 = " OUTPUT Inserted.IdAuto ";
                    if (Constante.tipBD == 2) rez1 = "";

                    sql += sir.Substring(1) + ") " + rez1 + " VALUES (";

                    sir = "";
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        if (!dt.Columns[k].AutoIncrement)
                        {
                            string val = "";
                            if (dr[k].GetType() == typeof(int) || dr[k].GetType() == typeof(decimal))
                                val = dr[k] == null ? "null" : dr[k].ToString();
                            if (dr[k].GetType() == typeof(string))
                                val = dr[k] == null ? "null" : "'" + dr[k].ToString() + "'";
                            if (dr[k].GetType() == typeof(DateTime))
                                val = dr[k] == null ? "null" : General.ToDataUniv((DateTime)dr[k]);
                            if (val.Length <= 0)
                                val = "''";
                            sir += "," + val;
                        }
                    }

                    string rez2 = "";
                    if (Constante.tipBD == 2) rez2 = "  RETURNING \"IdAuto\" INTO @out_1; ";

                    sql += sir.Substring(1) + ") " + rez2;

                    DataTable dtRez = new DataTable();
                    int idAuto = -99;
                    if (Constante.tipBD == 1)
                    {
                        dtRez = General.IncarcaDT(sql, null);
                        if (dtRez.Rows.Count > 0)
                            idAuto = Convert.ToInt32(dtRez.Rows[0]["IdAuto"]);
                    }
                    else
                    {
                        List<string> lstOut = General.DamiOracleScalar(sql, null);
                        if (lstOut.Count == 1)
                            idAuto = Convert.ToInt32(lstOut[0]);
                    }
                    int idAuto1 = Convert.ToInt32(dt.Rows[i]["IdAuto"].ToString());
                    if (tabela == "Admin_Medicina")
                    {
                        Dictionary<int, Personal.Medicina.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, Personal.Medicina.metaUploadFile>;
                        if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                            General.IncarcaFisier(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);                    
                    }
                    if (tabela == "Admin_Sanctiuni")
                    {
                        Dictionary<int, Personal.Sanctiuni.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, Personal.Sanctiuni.metaUploadFile>;
                        if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                            General.IncarcaFisier(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);                   
                    }


                }
            }
            if (tabela == "Admin_Medicina")
                Session["List_DocUpload_MP_Medicina"] = null;
            
            if (tabela == "Admin_Sanctiuni")
                Session["List_DocUpload_MP_Sanctiuni"] = null;
            

        }


        protected void Initializare(ref DataSet ds)
        {
            try
            {
                ds = new DataSet();


                string idSablon = Session["IdSablon"].ToString();
                string sql = "SELECT * FROM F099 WHERE F09903 = " + idSablon;
                DataTable dt = General.IncarcaDT(sql, null);

                sql = "SELECT * FROM F0991 WHERE F09903 = " + idSablon;
                DataTable dt1 = General.IncarcaDT(sql, null);

                DataTable dtComb = General.IncarcaDT("SELECT * FROM F100, F1001 WHERE F100.F10003 = F1001.F10003 AND F100.F10003 = (SELECT MIN(F10003) FROM F100)", null);
                object[] rowComb = new object[dtComb.Columns.Count];

                DataTable dt100 = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = (SELECT MIN(F10003) FROM F100)", null);
                object[] row100 = new object[dt100.Columns.Count];

                int x = 0;
                foreach (DataColumn col in dt100.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "F10003":
                        case "F100985":
                            row100[x] = Convert.ToInt32(Session["Marca"].ToString());
                            rowComb[x] = Convert.ToInt32(Session["Marca"].ToString());
                            break;
                        case "USER_NO":
                            row100[x] = Session["UserId"];
                            rowComb[x] = Session["UserId"];
                            break;
                        case "F100986":
                        case "F10022":
                        case "TIME":
                            row100[x] = DateTime.Now;
                            rowComb[x] = DateTime.Now;
                            break;                  
                    }
                    x++;
                }
                if (dt100.Rows.Count > 0)
                    dt100.Rows.RemoveAt(0);
                dt100.Rows.Add(row100);
                dt100.PrimaryKey = new DataColumn[] { dt100.Columns["F10003"] };

                DataTable dt1001 = General.IncarcaDT("SELECT * FROM F1001 WHERE F10003 = (SELECT MIN(F10003) FROM F1001)", null);
                dt1001.TableName = "F1001";
                object[] row1001 = new object[dt1001.Columns.Count];

                x = 0;
                foreach (DataColumn col in dt1001.Columns)
                {
                    switch (col.ColumnName.ToUpper())
                    {
                        case "F10003":
                            row1001[x] = Convert.ToInt32(Session["Marca"].ToString());
                            rowComb[x + dt100.Columns.Count] = Convert.ToInt32(Session["Marca"].ToString());
                            break;
                        case "USER_NO":
                            row1001[x] = Session["UserId"];
                            rowComb[x + dt100.Columns.Count] = Session["UserId"];
                            break;
                        case "TIME":
                            row1001[x] = DateTime.Now;
                            rowComb[x + dt100.Columns.Count] = DateTime.Now;
                            break;
                    }
                    x++;
                }
                if (dt1001.Rows.Count > 0)
                    dt1001.Rows.RemoveAt(0);
                dt1001.Rows.Add(row1001);
                dt1001.PrimaryKey = new DataColumn[] { dt1001.Columns["F10003"] };

                if (dtComb.Rows.Count > 0)
                    dtComb.Rows.RemoveAt(0);
                dtComb.Rows.Add(rowComb);
                dtComb.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };

                for (int i = 0; i < dt.Columns.Count; i++)
                    if (dt.Columns[i].ColumnName != "F09903" && dt.Columns[i].ColumnName != "F099985" && dt.Columns[i].ColumnName != "F099986" && dt.Columns[i].ColumnName != "F09922" && dt.Columns[i].ColumnName != "F09908" && dt.Columns[i].ColumnName != "F09909" && dt.Columns[i].ColumnName != "USER_NO" && dt.Columns[i].ColumnName != "TIME")
                    {
                        dt100.Rows[0][dt.Columns[i].ColumnName.Replace("F099", "F100")] = dt.Rows[0][dt.Columns[i].ColumnName];
                        dtComb.Rows[0][dt.Columns[i].ColumnName.Replace("F099", "F100")] = dt.Rows[0][dt.Columns[i].ColumnName];
                    }

                for (int i = 0; i < dt1.Columns.Count; i++)
                    if (dt1.Columns[i].ColumnName != "F09903" && dt1.Columns[i].ColumnName != "USER_NO" && dt1.Columns[i].ColumnName != "TIME")
                    {
                        dt1001.Rows[0][dt1.Columns[i].ColumnName.Replace("F099", "F100")] = dt1.Rows[0][dt1.Columns[i].ColumnName];
                        dtComb.Rows[0][dt1.Columns[i].ColumnName.Replace("F099", "F100")] = dt1.Rows[0][dt1.Columns[i].ColumnName];
                    }

                ds.Tables.Add(dtComb);
                dt100.TableName = "F100";
                ds.Tables.Add(dt100);
                dt1001.TableName = "F1001";
                ds.Tables.Add(dt1001);

                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void InserareAngajat(string marca, DataTable dt100, DataTable dt1001)
        {
            try
            {
                General.SalveazaDate(dt100, "F100");
                General.SalveazaDate(dt1001, "F1001");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public int NextMarca()
        {
            int id = 1;

            try
            {

                if (Constante.tipBD == 1)                   //SQL
                {
                    string vers = "2008";
                    string sql = "SELECT Valoare FROM tblParametrii WHERE Nume = 'VersiuneSQL'";
                    DataTable dt = General.IncarcaDT(sql, null);
                    if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                        vers = dt.Rows[0][0].ToString();

                    if (vers == "2012")
                    {
                        //secventa  SQL
                        sql = "SELECT NEXT VALUE FOR F100MARCA;";
                        dt = General.IncarcaDT(sql, null);

                        id = Convert.ToInt32(dt.Rows[0][0].ToString());
                    }
                    else
                    {
                        sql = "SELECT * FROM tblConfig WHERE Id = 1";
                        dt = General.IncarcaDT(sql, null);
                        if (dt.Rows[0]["NextMarca"] != null && dt.Rows[0]["NextMarca"].ToString().Length > 0)
                        {
                            id = Convert.ToInt32(dt.Rows[0]["NextMarca"].ToString()) + 1;
                        }
                        dt.Rows[0]["NextMarca"] = id;
                        SqlDataAdapter da = new SqlDataAdapter();
                        SqlCommandBuilder cb = new SqlCommandBuilder();
                        da = new SqlDataAdapter();
                        da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM tblConfig", null);
                        cb = new SqlCommandBuilder(da);
                        da.Update(dt);
                        da.Dispose();
                        da = null;

                    }
                }
                else                                   //Oracle
                {
                    try
                    {
                        string sql = "select F100MARCA.nextval from dual";
                        DataTable dt = General.IncarcaDT(sql, null);
                        id = Convert.ToInt32(dt.Rows[0][0].ToString());
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return id;
        }

        public void Securitate(string numeTab, out bool vizibil, out bool blocat)
        {
            vizibil = true;
            blocat = false;
            try
            {

                string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = {1} AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" = '{0}'
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" = '{0}') X
                                GROUP BY X.""IdControl"", X.""IdColoana""";
                strSql = string.Format(strSql, numeTab, Session["UserId"].ToString());

                DataTable dt = General.IncarcaDT(strSql, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    vizibil = (Convert.ToInt32(dt.Rows[0]["Vizibil"].ToString()) == 1 ? true : false);
                    blocat = (Convert.ToInt32(dt.Rows[0]["Blocat"].ToString()) == 1 ? true : false);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public List<string> ListaSecuritate()
        {
            List<string> lista = new List<string>();
            try
            {

                string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = {0} AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl"" like '%_I%'
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = 'Personal.Lista' AND ""IdControl""  like '%_I%') X
                                GROUP BY X.""IdControl"", X.""IdColoana""";
                strSql = string.Format(strSql, Session["UserId"].ToString());

                DataTable dt = General.IncarcaDT(strSql, null);

                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                        lista.Add(dt.Rows[i]["IdControl"].ToString());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            return lista;
        }

        protected void ASPxPageControl2_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                ASPxPageControl ctl = sender as ASPxPageControl;
                if (ctl == null) return;
                TabPage tab = ctl.ActiveTabPage;
                
                if (Session["PreluareDate"] != null && Session["PreluareDate"].ToString() == "1")
                {
                    Session["PreluareDate"] = 0;
                    for (int j = 0; j < tab.Controls[0].Controls.Count; j++)
                    {
                        if (tab.Controls[0].Controls[j].GetType() == typeof(DevExpress.Web.ASPxCallbackPanel))
                        {
                            ASPxCallbackPanel cb = tab.Controls[0].Controls[j] as ASPxCallbackPanel;
                            for (int k = 0; k < cb.Controls.Count; k++)
                            {
                                if (cb.Controls[k].GetType() == typeof(DataList))
                                {
                                    DataList dl = cb.Controls[k] as DataList;
                                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                                    DataTable table = ds.Tables[0];
                                    dl.DataSource = table;
                                    dl.DataBind();
                                    break;
                                }
                            }
                            break;
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

        private void AdaugaValorile()
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string[] tabs = { "DateIdentificare", "Contract", "DateGenerale", "Detasari", "Diverse", "Documente", "Studii", "Banca" };

                //DateIdentificare
                #region DateIdentificare
                Dictionary<String, String> lstDI = new Dictionary<string, string>();
                lstDI.Add("txtMarcaDI", "F10003");
                lstDI.Add("txtCNPDI", "F10017");
                lstDI.Add("txtMarcaUnica", "F1001036");
                lstDI.Add("txtEIDDI", "F100901");
                lstDI.Add("deDataNasterii", "F10021");
                lstDI.Add("txtNume", "F10008");
                lstDI.Add("txtPrenume", "F10009");
                lstDI.Add("txtNumeAnt", "F100905");
                lstDI.Add("deDataModifNume", "F100906");
                lstDI.Add("cmbStareCivila", "F10046");
                lstDI.Add("rbSex", "F10047");
                #endregion

                //Contract
                #region Contract
                Dictionary<String, String> lstCtr = new Dictionary<string, string>();
                lstCtr.Add("txtNrCtrInt", "F100985");
                lstCtr.Add("deDataCtrInt", "F100986");
                lstCtr.Add("deDataAng", "F10022");
                //lstCtr.Add("deTermenRevisal", "F100986");
                lstCtr.Add("cmbTipCtrMunca", "F100984");
                lstCtr.Add("cmbDurCtr", "F1009741");
                lstCtr.Add("deDeLaData", "F100933");
                lstCtr.Add("deLaData", "F100934");
                lstCtr.Add("cmbPrel", "F100938");
                lstCtr.Add("cmbExcIncet", "F100929");
                lstCtr.Add("cmbCASSAngajat", "F1003900");
                lstCtr.Add("cmbCASSAngajator", "F1003907");
                lstCtr.Add("txtSalariu", "F100699");
                lstCtr.Add("deDataModifSal", "F100991");
                lstCtr.Add("cmbCategAng1", "F10061");
                lstCtr.Add("cmbCategAng2", "F10062");
                lstCtr.Add("txtLocAnt", "F10078");
                lstCtr.Add("cmbLocatieInt", "F100966");
                lstCtr.Add("cmbTipAng", "F10010");
                lstCtr.Add("cmbTimpPartial", "F10043");
                lstCtr.Add("cmbNorma", "F100973");
                lstCtr.Add("deDataModifNorma", "F100955");
                lstCtr.Add("cmbTipNorma", "F100926");
                lstCtr.Add("cmbDurTimpMunca", "F100927");
                lstCtr.Add("cmbRepTimpMunca", "F100928");
                lstCtr.Add("cmbIntRepTimpMunca", "F100939");
                lstCtr.Add("txtNrOre", "F100964");
                lstCtr.Add("cmbCOR", "F10098");
                lstCtr.Add("deDataModifCOR", "F100956");
                lstCtr.Add("cmbFunctie", "F10071");
                lstCtr.Add("deDataModifFunctie", "F100992");
                lstCtr.Add("cmbMeserie", "F10029");
                lstCtr.Add("chkFunctieBaza", "F10032");
                lstCtr.Add("chkCalcDed", "F10048");
                lstCtr.Add("txtPerProbaZL", "F1001063");
                lstCtr.Add("txtPerProbaZC", "F100975");
                lstCtr.Add("txtNrZilePreavizDemisie", "F1009742");
                lstCtr.Add("txtNrZilePreavizConc", "F100931");
                lstCtr.Add("deUltimaZiLucr", "F10023");
                //lstCtr.Add("deUltimaZiLucr", "F100993");
                lstCtr.Add("cmbMotivPlecare", "F10025");
                lstCtr.Add("deDataPlecarii", "F100993");
                lstCtr.Add("deDataReintegr", "F100930");
                lstCtr.Add("cmbGradInvalid", "F10027");
                lstCtr.Add("deDataValabInvalid", "F100271");
                lstCtr.Add("chkScutitImp", "F10026");
                lstCtr.Add("cmbMotivScutit", "F1001098");          
                lstCtr.Add("cmbMotivScutitCAS", "F1001096");
                lstCtr.Add("chkSalMin", "F1001117");
                lstCtr.Add("chkConstr", "F1001118");
                lstCtr.Add("chkBifaPensionar", "F10037");
                lstCtr.Add("chkCotaForfetara", "F1001069");
                lstCtr.Add("chkBifaDetasat", "F100954");
                lstCtr.Add("rbCtrRadiat", "F1001077");

                //lstCtr.Add("txtVechCompAni", "F100986");
                //lstCtr.Add("txtVechCompLuni", "F100986");
                //lstCtr.Add("txtVechCarteMuncaAni", "F100986");
                //lstCtr.Add("txtVechCarteMuncaLuni", "F100986");

                lstCtr.Add("txtGrila", "F10072");
                lstCtr.Add("txtZileCOFidel", "F100640");
                //lstCtr.Add("txtZileCOAnAnt", "F100641");
                lstCtr.Add("txtZileCOAnAnt", "F100996");
                lstCtr.Add("txtZileCOCuvAnCrt", "F100642");
                lstCtr.Add("deDataPrimeiAng", "F1001049");
                lstCtr.Add("txtVechimeCompanie", "F100643");
                lstCtr.Add("txtVechimeCarte", "F100644");
                lstCtr.Add("txtZileCOAnCrt", "F100995");

                lstCtr.Add("hfNrLuni", "F100935");
                lstCtr.Add("hfNrAni", "F100936");

                #endregion

                //DateGenerale
                #region DateGenerale
                Dictionary<String, String> lstDG = new Dictionary<string, string>();
                lstDG.Add("txtCNP", "F10017");
                lstDG.Add("txtCNPVechi", "F100171");
                lstDG.Add("txtEID", "F100901");
                lstDG.Add("txtNrCtr", "F100985");
                lstDG.Add("txtNumeDG", "F10008");
                lstDG.Add("txtPrenumeDG", "F10009");
                lstDG.Add("txtNumeAntDG", "F100905");
                lstDG.Add("dePanaLa", "F100906");
                lstDG.Add("deDataNasteriiDG", "F10021");
                lstDG.Add("cmbCompanie", "F10002");
                lstDG.Add("cmbSubcompanie", "F10004");
                lstDG.Add("cmbFiliala", "F10005");
                lstDG.Add("cmbSectie", "F10006");
                lstDG.Add("cmbDepartament", "F10007");
                lstDG.Add("cmbSubdept", "F100958");
                lstDG.Add("cmbBirouEchipa", "F100959");
                lstDG.Add("deDataAngDG", "F10022");
                lstDG.Add("deUltimaZiLucrDG", "F10023");
                lstDG.Add("cmbMotivPlecareDG", "F10025");
                lstDG.Add("cmbTimpPartialDG", "F10043");
                lstDG.Add("cmbNormaDG", "F100973");
                lstDG.Add("cmbCategAng", "F10061");
                #endregion

                //Detasari
                #region Detasari
                Dictionary<String, String> lstDS = new Dictionary<string, string>();
                lstDS.Add("txtNumeAngajator", "F100918");
                lstDS.Add("txtCUI", "F100919");
                lstDS.Add("cmbNationalitate", "F100920");
                lstDS.Add("deDataInceputDet", "F100915");
                lstDS.Add("deDataSfarsitDet", "F100916");
                lstDS.Add("deDataIncetareDet", "F100917");
                #endregion

                //Diverse
                #region Diverse
                Dictionary<String, String> lstDV = new Dictionary<string, string>();
                lstDV.Add("txtNr", "F10011");
                lstDV.Add("txtNorma", "F10043");
                lstDV.Add("deData", "FX1");
                lstDV.Add("txtLocNastere", "F100980");
                lstDV.Add("cmbStudiiDiv", "F10050");
                lstDV.Add("txtStudiiDet", "F100902");
                lstDV.Add("cmbFunctieDiv", "F10071");
                lstDV.Add("cmbNivel", "F10029");
                lstDV.Add("txtZileCOFidel", "F100640");
                //lstDV.Add("txtZileCOAnAnt", "F100641");
                lstDV.Add("txtZileCOAnAnt", "F100996");
                lstDV.Add("txtZileCOCuvAnC", "F100642");
                lstDV.Add("txtZileCOAnCrt", "F100995");

                lstDV.Add("txtVechimeComp", "F100643");
                lstDV.Add("txtVechimeCarteMunca", "F100644");
                #endregion

                //Documente
                #region Documente
                Dictionary<String, String> lstDO = new Dictionary<string, string>();
                lstDO.Add("cmbTara", "F100987");
                lstDO.Add("cmbCetatenie", "F100981");
                lstDO.Add("cmbTipAutMunca", "F100911");
                lstDO.Add("deDataInc", "F100912");
                lstDO.Add("deDataSf", "F100913");
                lstDO.Add("txtNumeMama", "F100988");
                lstDO.Add("txtNumeTata", "F100989");
                lstDO.Add("cmbTipDoc", "F100983");
                lstDO.Add("txtSerieNr", "F10052");
                lstDO.Add("txtEmisDe", "F100521");
                lstDO.Add("txtLocNastere", "F100980");
                lstDO.Add("deDataElib", "F100522");
                lstDO.Add("deDataExp", "F100963");
                lstDO.Add("txtNrPermisMunca", "F100982");
                lstDO.Add("deDataPermisMunca", "F100994");
                lstDO.Add("txtNrCtrIntVechi", "F100940");
                lstDO.Add("txtDataCtrIntVechi", "F100941");
                lstDO.Add("txtDetaliiCtrAngajat", "F100942");
                lstDO.Add("cmbCateg", "F10028");
                lstDO.Add("deDataEmitere", "F10024");
                lstDO.Add("deDataExpirare", "F1001000");
                lstDO.Add("txtNr", "F1001001");
                lstDO.Add("txtPermisEmisDe", "F1001002");
                lstDO.Add("cmbStudiiDoc", "F10050");
                lstDO.Add("txtCalif1", "F100903");
                lstDO.Add("txtCalif2", "F100904");
                lstDO.Add("cmbTitluAcademic", "F10051");
                lstDO.Add("cmbDedSomaj", "F10073");
                lstDO.Add("txtNrCarteMunca", "F10012");
                lstDO.Add("txtSerieCarteMunca", "F10013");
                lstDO.Add("deDataCarteMunca", "FX1");
                lstDO.Add("txtLivret", "F100571");
                lstDO.Add("txtElibDe", "F100572");
                lstDO.Add("deDeLaDataLivMil", "F100573");
                lstDO.Add("deLaDataLivMil", "F100574");
                lstDO.Add("txtGrad", "F100575");
                lstDO.Add("txtOrdin", "F100576");
                #endregion

                //Studii
                #region Studii
                Dictionary<String, String> lstSU = new Dictionary<string, string>();
                lstSU.Add("cmbStudiiSt", "F10050");
                lstSU.Add("txtInstit", "F1001085");
                lstSU.Add("txtSpec", "F1001086");
                lstSU.Add("deDataInceputSt", "F1001087");
                lstSU.Add("deDataSfarsitSt", "F1001088");
                lstSU.Add("deDataDiploma", "F1001089");
                lstSU.Add("txtObs", "F1001090");
                #endregion

                //Structura
                #region Structura
                Dictionary<String, String> lstST = new Dictionary<string, string>();
                lstST.Add("xxxxxxxxxxxxx", "F10003");

                #endregion

                //Banca
                #region Banca
                Dictionary<String, String> lstBC = new Dictionary<string, string>();
                lstBC.Add("txtIBANSal", "F10020");
                lstBC.Add("txtNrCard", "F10055");
                lstBC.Add("cmbBancaSal", "F10018");
                lstBC.Add("cmbSucSal", "F10019");
                lstBC.Add("deDataModifSal", "F1001040");
                lstBC.Add("txtIBANGar", "F1001028");
                lstBC.Add("cmbBancaGar", "F1001026");
                lstBC.Add("cmbSucGar", "F1001027");
                lstBC.Add("deDataModifGar", "F1001041");
                lstBC.Add("txtIBANTichete", "F1001121");
                lstBC.Add("deDataIncTichete", "F1001122");
                lstBC.Add("deDataSfTichete", "F1001123");
                #endregion

                DataColumnCollection cols1 = ds.Tables[1].Columns;
                DataColumnCollection cols2 = ds.Tables[2].Columns;
                DataColumnCollection cols3 = ds.Tables[0].Columns;

                for (int i = 0; i < ASPxPageControl2.TabPages.Count; i++)
                {
                    if (tabs.Contains(ASPxPageControl2.TabPages[i].Name))
                    {
                        string numeTab = ASPxPageControl2.TabPages[i].Name;
                        Dictionary<String, String> lst = new Dictionary<string, string>();
                        switch (numeTab)
                        {
                            case "DateIdentificare":
                                lst = lstDI;
                                break;
                            case "Contract":
                                lst = lstCtr;
                                break;
                            case "DateGenerale":
                                lst = lstDG;
                                break;
                            case "Detasari":
                                lst = lstDS;
                                break;
                            case "Diverse":
                                lst = lstDV;
                                break;
                            case "Documente":
                                lst = lstDO;
                                break;
                            case "Studii":
                                lst = lstSU;
                                break;
                            case "Structura":
                                lst = lstST;
                                break;
                            case "Banca":
                                lst = lstBC;
                                break;
                        }
                        foreach (string idCtl in lst.Keys)
                        {
                            try
                            {
                                string colName = lst[idCtl];
                                dynamic ctl = ((dynamic)ASPxPageControl2.TabPages[i].Controls[0].FindControl(numeTab + "_pnlCtl").FindControl(numeTab + "_DataList")).Items[0].FindControl(idCtl);

                                DataTable dt = new DataTable();
                                if (cols1.Contains(colName)) dt = ds.Tables[1];
                                if (cols2.Contains(colName)) dt = ds.Tables[2];
                                if (ctl != null && General.Nz(dt.Rows[0][colName], "").ToString() != General.Nz(ctl.Value, "").ToString()) dt.Rows[0][colName] = ctl.Value;

                                DataTable dt2 = new DataTable();
                                if (cols3.Contains(colName)) dt2 = ds.Tables[0];
                                if (ctl != null && General.Nz(dt2.Rows[0][colName], "").ToString() != General.Nz(ctl.Value, "").ToString()) dt2.Rows[0][colName] = ctl.Value;
                            }
                            catch (Exception ex)
                            {
                                var ert = ex.Message;
                            }
                        }
                    }
                    //dynamic ctl1 = ((dynamic)tab.Controls[0].FindControl("pnlCtlDateIdent").FindControl("DateIdentListView")).Items[0].FindControl("txtNume");
                    //if (ctl1 != null) ds.Tables[1].Rows[0]["F10008"] = ctl1.Value;
                    //dynamic ctl2 = ((dynamic)tab.Controls[0].FindControl("pnlCtlDateIdent").FindControl("DateIdentListView")).Items[0].FindControl("txtPrenume");
                    //if (ctl2 != null) ds.Tables[1].Rows[0]["F10009"] = ctl2.Value;
                    //dynamic ctl3 = ((dynamic)tab.Controls[0].FindControl("pnlCtlDateIdent").FindControl("DateIdentListView")).Items[0].FindControl("txtCNPDI");
                    //if (ctl3 != null) ds.Tables[0].Rows[0]["F10017"] = ctl3.Value;
                    //dynamic ctl4 = ((dynamic)tab.Controls[0].FindControl("pnlCtlDateIdent").FindControl("DateIdentListView")).Items[0].FindControl("deDataModifNume");
                    //if (ctl4 != null) ds.Tables[0].Rows[0]["F100906"] = ctl4.Value;
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