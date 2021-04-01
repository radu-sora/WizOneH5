using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

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

                Response.Cache.SetNoStore();
                Response.AppendHeader("Pragma", "no-cache");
                Response.Expires = 0;

                Session["PaginaWeb"] = "Personal.DateAngajat";

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string marca = Session["Marca"] as string;
                if (ds == null && marca == null)
                {
                    marca = NextMarca().ToString();
                    Session["Marca"] = marca;
                    Session["esteNou"] = "true";

                    Initializare(ref ds);
                }
                else              //Florin 2020.10.28
                {
                    General.AflaIdPost();
                }

                if (!IsPostBack)
                {
                    string culoare = (General.ExecutaScalar(@"SELECT COALESCE(""Valoare"",'') FROM ""tblParametrii"" WHERE ""Nume"" = 'MP_CuloareCampObligatoriu' ", null) ?? "").ToString();
                    if (culoare != null && culoare.Length == 7 && culoare[0] == '#')
                    {
                        int r = int.Parse(culoare[1].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[2].ToString(), System.Globalization.NumberStyles.HexNumber) > 255 ? 255 
                            : int.Parse(culoare[1].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[1].ToString(), System.Globalization.NumberStyles.HexNumber);
                        int g = int.Parse(culoare[3].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[4].ToString(), System.Globalization.NumberStyles.HexNumber) > 255 ? 255
                            : int.Parse(culoare[3].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[4].ToString(), System.Globalization.NumberStyles.HexNumber);
                        int b = int.Parse(culoare[5].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[6].ToString(), System.Globalization.NumberStyles.HexNumber) > 255 ? 255
                            : int.Parse(culoare[5].ToString(), System.Globalization.NumberStyles.HexNumber) * 16 + int.Parse(culoare[6].ToString(), System.Globalization.NumberStyles.HexNumber);
                        List<int> lst = new List<int>();
                        lst.Add(r); lst.Add(g); lst.Add(b);
                        Session["MP_CuloareCampOblig"] = lst;
                    }

                    //Radu 20.02.2020 - citire securitate
                    string sqlSec = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"", X.""IdForm""  FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat"", A.""IdForm""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = {0} AND A.""IdForm"" like 'Personal.%' 
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat"", A.""IdForm""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" like 'Personal.%' ) X
                                GROUP BY X.""IdControl"", X.""IdColoana"", X.""IdForm""";
                    sqlSec = string.Format(sqlSec, Session["UserId"].ToString());
                    DataTable dtSec = General.IncarcaDT(sqlSec, null);
                    Session["SecuritatePersonal"] = dtSec;           
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

                int tipAfisare = Convert.ToInt32(Dami.ValoareParam("AfisareTaburiPersonal", "1"));

                int nrTaburi = dt.Rows.Count;
                bool multirand = false;
                if (nrTaburi > 10 && tipAfisare == 1)
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
                if (tipAfisare == 2)
                    this.ASPxPageControl2.EnableTabScrolling = true;

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
                    string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");

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
                    if (ds.Tables[0].Rows[0]["F1009741"] != null && ds.Tables[0].Rows[0]["F1009741"].ToString().Length > 0 && ds.Tables[0].Rows[0]["F1009741"].ToString() == "2")
                    {
                        if (ds.Tables[0].Rows[0]["F100933"] == null || ds.Tables[0].Rows[0]["F100933"].ToString().Length <= 0 || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()) == new DateTime(1900, 1, 1)
                            || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()) == new DateTime(2100, 1, 1))
                            mesaj += " - de la data" + Environment.NewLine;
                        if ((ds.Tables[0].Rows[0]["F100929"] == null || Convert.ToInt32(ds.Tables[0].Rows[0]["F100929"].ToString()) == 0) && (ds.Tables[0].Rows[0]["F100934"] == null || ds.Tables[0].Rows[0]["F100934"].ToString().Length <= 0 || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100934"].ToString()) == new DateTime(2100, 1, 1)))
                            mesaj += " - la data" + Environment.NewLine;
                    }
                    if (ds.Tables[0].Rows[0][salariu] == null || ds.Tables[0].Rows[0][salariu].ToString().Length <= 0 || Convert.ToDouble(ds.Tables[0].Rows[0][salariu].ToString()) == 0.0)
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
                    //if (ds.Tables[0].Rows[0]["F100642"] == null || ds.Tables[0].Rows[0]["F100642"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100642"].ToString() == "0")
                    //    mesaj += " - zile CO cuvenite cf. grila" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10022"] == null || ds.Tables[0].Rows[0]["F10022"].ToString().Length <= 0
                        || Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()) == new DateTime(2100, 1, 1) || Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()) == new DateTime(1900, 1, 1))
                        mesaj += " - data angajarii" + Environment.NewLine;

                    
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
                else
                {
                    if (ds.Tables[0].Rows[0]["F1009741"] != null && ds.Tables[0].Rows[0]["F1009741"].ToString().Length > 0 && ds.Tables[0].Rows[0]["F1009741"].ToString() == "2")
                    {
                        string mesaj = "Date angajare: " + Environment.NewLine;
                        bool err = false;
                        if (ds.Tables[0].Rows[0]["F100933"] == null || ds.Tables[0].Rows[0]["F100933"].ToString().Length <= 0 || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()) == new DateTime(1900, 1, 1)
                            || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()) == new DateTime(2100, 1, 1))
                        {
                            mesaj += " - de la data" + Environment.NewLine;
                            err = true;
                        }
                        if ((ds.Tables[0].Rows[0]["F100929"] == null || Convert.ToInt32(ds.Tables[0].Rows[0]["F100929"].ToString()) == 0) && (ds.Tables[0].Rows[0]["F100934"] == null || ds.Tables[0].Rows[0]["F100934"].ToString().Length <= 0 || Convert.ToDateTime(ds.Tables[0].Rows[0]["F100934"].ToString()) == new DateTime(2100, 1, 1)))
                        {
                            mesaj += " - la data" + Environment.NewLine;
                            err = true;
                        }
                        if (err)
                        {
                            MessageBox.Show("Nu ati completat: " + Environment.NewLine + Environment.NewLine + mesaj, MessageBox.icoError, "Atentie!");
                            return;
                        }
                    }
                }

                if (Session["esteNou"] == null || Session["esteNou"].ToString().Length <= 0 || Session["esteNou"].ToString() == "false")
                {
                    DataTable dtIncetare = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + ds.Tables[0].Rows[0]["F10003"].ToString(), null);
                    if (Convert.ToDateTime(dtIncetare.Rows[0]["F10023"].ToString()).Date != Convert.ToDateTime(ds.Tables[0].Rows[0]["F10023"].ToString()).Date)
                    {
                        DataTable dtSusp = General.IncarcaDT("SELECT COUNT(*) FROM F111 WHERE F11103 = " + ds.Tables[0].Rows[0]["F10003"].ToString() + " AND (F11107 IS NULL OR F11107 = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '01/01/2100', 103))" : "TO_DATE('01/01/2100', 'dd/mm/yyyy'))"), null);
                        if (dtSusp != null && dtSusp.Rows.Count > 0 && Convert.ToInt32(dtSusp.Rows[0][0].ToString()) > 0)
                        {
                            MessageBox.Show(Dami.TraduCuvant("Nu puteti inceta acest contract deoarece angajatul are cel putin o suspendare activa!"), MessageBox.icoError, "Atentie!");
                            //ds.Tables[0].Rows[0]["F10023"] = dtIncetare.Rows[0]["F10023"];
                            //ds.Tables[0].Rows[0]["F10025"] = dtIncetare.Rows[0]["F10025"];
                            //ds.Tables[0].Rows[0]["F100993"] = dtIncetare.Rows[0]["F100993"];
                            //ds.Tables[1].Rows[0]["F10023"] = dtIncetare.Rows[0]["F10023"];
                            //ds.Tables[1].Rows[0]["F10025"] = dtIncetare.Rows[0]["F10025"];
                            //ds.Tables[1].Rows[0]["F100993"] = dtIncetare.Rows[0]["F100993"];
                            return;
                        }
                    }
                }

                if (ds.Tables[0].Rows[0]["F1009741"] != null && ds.Tables[0].Rows[0]["F1009741"].ToString().Length > 0 && ds.Tables[0].Rows[0]["F1009741"].ToString() == "2")
                {
                    if (Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()).Date > Convert.ToDateTime(ds.Tables[0].Rows[0]["F100934"].ToString()).Date)
                    {
                        MessageBox.Show("Date angajare:" + Environment.NewLine + " - data start contract determinat este mai mare decat data sfarsit!", MessageBox.icoError, "Atentie!");
                        return;
                    }
                    if (Convert.ToDateTime(ds.Tables[0].Rows[0]["F100933"].ToString()).Date < Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()).Date)
                    {
                        MessageBox.Show("Date angajare:" + Environment.NewLine + " - data start contract determinat este mai mica decat data angajarii!", MessageBox.icoError, "Atentie!");
                        return;
                    }
                }

                if (Convert.ToInt32(General.Nz(Session["IdClient"], -99)) == (int)IdClienti.Clienti.Chimpex)
                {
                    if (ds.Tables[0].Rows[0]["F100985"] != null && ds.Tables[0].Rows[0]["F100985"].ToString().Length > 0)
                    {
                        DataTable dtCtrInt = General.IncarcaDT("SELECT COUNT(*) FROM F100 WHERE F100985 = '" + ds.Tables[0].Rows[0]["F100985"].ToString() + "' AND F10003 != " + ds.Tables[0].Rows[0]["F10003"].ToString(), null);
                        if (dtCtrInt != null && dtCtrInt.Rows.Count > 0 && dtCtrInt.Rows[0][0] != null && dtCtrInt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtCtrInt.Rows[0][0].ToString()) > 0)
                        {
                            MessageBox.Show("Date angajare:" + Environment.NewLine + " - nr. ctr. intern este deja alocat altui angajat!", MessageBox.icoError, "Atentie!");
                            return;
                        }
                    }
                }

                //verificarea se face pe partea de client
                //Radu 29.08.2019 - este necesara verificarea si la salvare, pentru ca pot sa ramana valori invalide pe CNP si data nasterii
                string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NuPermiteCNPInvalid'";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                    if (!General.VerificaCNP(ds.Tables[1].Rows[0]["F10017"].ToString()))
                    {
                        MessageBox.Show("CNP invalid!", MessageBox.icoError);
                        return;
                    }


                int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                if (varsta < 16)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu puteti angaja o persoana cu varsta mai mica de 16 ani!"), MessageBox.icoError);
                    return;
                }

                varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"]));
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

                //Radu 15.01.2020

                string valMin = "100000";
                int idCtr = -99;
                DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                    valMin = dtParam.Rows[0][0].ToString();
                string sqlCtr = "SELECT IdContract, DataSfarsit FROM F100Contracte WHERE F10003 = " + ds.Tables[1].Rows[0]["F10003"].ToString() + " AND IdAuto >= " + valMin + " ORDER BY DataSfarsit DESC";
                DataTable dtCtr = General.IncarcaDT(sqlCtr, null);
                DateTime dtSf1 = new DateTime(1900, 1, 1);
                if (dtCtr != null && dtCtr.Rows.Count > 0 && dtCtr.Rows[0]["DataSfarsit"] != DBNull.Value && dtCtr.Rows[0]["DataSfarsit"].ToString() != null)
                    dtSf1 = Convert.ToDateTime(dtCtr.Rows[0]["DataSfarsit"].ToString());
                DateTime dtSf2 = new DateTime(1900, 1, 1);
                DataTable dtCtr2 = null;
                if (ds.Tables.Contains("F100Contracte2") && ds.Tables["F100Contracte2"].Rows.Count > 0)
                {
                    dtCtr2 = ds.Tables["F100Contracte2"].Select("F10003 = " + ds.Tables[1].Rows[0]["F10003"].ToString()) != null ?
                        ds.Tables["F100Contracte2"].Select("F10003 = " + ds.Tables[1].Rows[0]["F10003"].ToString()).OrderByDescending(x => x["DataSfarsit"]).CopyToDataTable() : null;
                    if (dtCtr2 != null)
                        dtSf2 = Convert.ToDateTime(dtCtr2.Rows[0]["DataSfarsit"].ToString());
                }
                if (dtSf1 > dtSf2)
                {
                    idCtr = Convert.ToInt32(dtCtr.Rows[0]["IdContract"].ToString());
                }
                else if (dtCtr2 != null && dtCtr2.Rows.Count > 0)
                {
                    idCtr = Convert.ToInt32(dtCtr2.Rows[0]["IdContract"].ToString());
                }

                string sqlAng = "SELECT " + ds.Tables[1].Rows[0]["F10003"] + " AS F10003, "
                        + (ds.Tables[1].Rows[0]["F100901"] == DBNull.Value ? "NULL" : "'" + ds.Tables[1].Rows[0]["F100901"].ToString() + "'") + " AS F100901, "
                        + (ds.Tables[1].Rows[0]["F10071"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10071"].ToString()) + " AS F10071, "
                        + (ds.Tables[1].Rows[0]["F10050"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10050"].ToString()) + " AS F10050, "
                        + (ds.Tables[1].Rows[0]["F10051"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10051"].ToString()) + " AS F10051, "
                        + (ds.Tables[1].Rows[0]["F10061"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10061"].ToString()) + " AS F10061, "
                        + (ds.Tables[1].Rows[0]["F10062"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10062"].ToString()) + " AS F10062, "
                        + (ds.Tables[1].Rows[0]["F10004"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10004"].ToString()) + " AS F10004, "
                        + (ds.Tables[1].Rows[0]["F1009741"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F1009741"].ToString()) + " AS F1009741, "
                        + (ds.Tables[1].Rows[0]["F100984"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F100984"].ToString()) + " AS F100984, "
                        + (ds.Tables[1].Rows[0]["F100935"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F100935"].ToString()) + " AS F100935, "
                        + (ds.Tables[1].Rows[0]["F100936"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F100936"].ToString()) + " AS F100936, "
                        + (ds.Tables[1].Rows[0]["F100975"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F100975"].ToString()) + " AS F100975, "
                        + (ds.Tables[2].Rows[0]["F1001063"] == DBNull.Value ? "NULL" : ds.Tables[2].Rows[0]["F1001063"].ToString()) + " AS F1001063, "
                        + (ds.Tables[1].Rows[0]["F1009742"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F1009742"].ToString()) + " AS F1009742, "
                        + (ds.Tables[1].Rows[0]["F100931"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F100931"].ToString()) + " AS F100931, "
                        + (ds.Tables[2].Rows[0]["F1001046"] == DBNull.Value ? "NULL" : ds.Tables[2].Rows[0]["F1001046"].ToString()) + " AS F1001046, "
                        + (ds.Tables[1].Rows[0]["F10072"] == DBNull.Value ? "NULL" : ds.Tables[1].Rows[0]["F10072"].ToString()) + " AS F10072, "
                        + (ds.Tables[1].Rows[0]["F100902"] == DBNull.Value ? "NULL" : "'" + ds.Tables[1].Rows[0]["F100902"].ToString() + "'") + " AS F100902, "
                        + (ds.Tables[1].Rows[0]["F100904"] == DBNull.Value ? "NULL" : "'" + ds.Tables[1].Rows[0]["F100904"].ToString() + "'") + " AS F100904, "
                        + (ds.Tables[2].Rows[0]["F100943"] == DBNull.Value ? "NULL" : ds.Tables[2].Rows[0]["F100943"].ToString()) + " AS F100943, "
                        + idCtr + " AS IdContract ";                
                
                //se pot completa in viitor si alte campuri de interes
                string msg = Notif.TrimiteNotificare("Personal.Lista", (int)Constante.TipNotificare.Validare, sqlAng + ", 1 AS \"Actiune\", 1 AS \"IdStareViitoare\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
                if (msg != "" && msg.Substring(0, 1) == "2")
                {
                    MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
                    return;
                }

                //Radu 09.09.2020 - completare data consemnare
                ds.Tables[0].Rows[0]["F1001109"] = General.FindDataConsemnare(Convert.ToInt32(ds.Tables[1].Rows[0]["F10003"].ToString()));
                ds.Tables[2].Rows[0]["F1001109"] = General.FindDataConsemnare(Convert.ToInt32(ds.Tables[1].Rows[0]["F10003"].ToString()));

                //Florin 2018-10-30
                //calculam CO daca se insereaza un angajat
                //bool calcCO = false;
                bool esteNou = false;
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    int val = 1;
                    esteNou = true;
                    sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TermenDepasireRevisal'";
                    dt = General.IncarcaDT(sql, null);
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

                    //Radu 15.11.2019 -  nr si data contract intern se salveaza si pe nr si data contract ITM
                    ds.Tables[1].Rows[0]["F10011"] = ds.Tables[1].Rows[0]["F100985"];
                    ds.Tables[1].Rows[0]["FX1"] = ds.Tables[1].Rows[0]["F100986"];

                    //Radu 15.11.2019 - data angajarii se salveaza si pe data modif. struct. org.
                    ds.Tables[1].Rows[0]["F100910"] = ds.Tables[1].Rows[0]["F10022"];

                    //Florin 2018.11.23
                    //daca este nou verificam ca data angajarii sa fie mai mare decat luna de lucru
                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["F10022"] != DBNull.Value)
                    {
                        try
                        {
                            if (Convert.ToInt32(General.Nz(ds.Tables[1].Rows[0]["F10025"],0)) == 999  && Convert.ToDateTime(ds.Tables[1].Rows[0]["F10022"]) < General.DamiDataLucru().AddMonths(1))
                            {
                                MessageBox.Show("Data angajarii este mai mica decat luna de salarizare", MessageBox.icoError, "");
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


                    InserareAngajat(Session["Marca"].ToString(), ds.Tables[1], ds.Tables[2]);

                    //Florin 2019.06.24
                    //Mihnea 2019.06.13
                    int tip_pass = 0;
                    tip_pass = Convert.ToInt32(Dami.ValoareParam("Parola_creare_user", "0"));

                    int creareUtilizator = (Session["MP_CreareUtilizator"] as int? ?? 0);                                

                    if (creareUtilizator == 1)
                    {//Radu 05.12.2019 - crearea utilizatorului se va face numai daca a fost bifata optiunea din popup Sablon
                        string pass = General.Nz(ds.Tables[1].Rows[0]["F10017"], "").ToString();
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
                            case 2:             //parola este ultimele 6 caractere din CNP
                                if (pass.Length >= 6)
                                    pass = pass.Substring(pass.Length - 6);
                                break;
                            default:
                                //parola este cnp-ul
                                break;
                        }

                        if (pass == "") pass = "0";

                        string userNume = "";
                        //Radu 09.04.2020
                        string paramUser = Dami.ValoareParam("AlcatuireNumeUtilizator", "");
                        if (paramUser.Length > 0)
                        {
                            DataTable dtUserParam = General.IncarcaDT("SELECT " + paramUser + " FROM F100 WHERE F10003 = " + ds.Tables[1].Rows[0]["F10003"].ToString(), null);
                            userNume = dtUserParam.Rows[0][0].ToString();
                        }
                        else
                        {
                            if (General.Nz(ds.Tables[1].Rows[0]["F10008"], "").ToString() != "" || General.Nz(ds.Tables[1].Rows[0]["F10009"], "").ToString() != "")
                                userNume = General.Nz(ds.Tables[1].Rows[0]["F10009"], "").ToString().Replace("-", "").Replace(" ", "") + "." + General.Nz(ds.Tables[1].Rows[0]["F10008"], "").ToString().Replace("-", "").Replace(" ", "");                            
                        }
                        //daca numele de utilizator exista, adaugam un nr in coada
                        DataTable dtUser = General.IncarcaDT("SELECT COUNT(*) FROM USERS WHERE UPPER(F70104) like UPPER('" + userNume + "%')", null);
                        if (Convert.ToInt32(dtUser.Rows[0][0].ToString()) != 0)
                            userNume += (Convert.ToInt32(dtUser.Rows[0][0].ToString()) + 1).ToString();

                        //Radu 09.04.2020
                        string resetareParola = Dami.ValoareParam("ResetareParola", "0");
                        string mail = "NULL", numeComplet = "NULL";
                        if (ds.Tables[1].Rows[0]["F100894"] != null && ds.Tables[1].Rows[0]["F100894"].ToString().Length > 0)
                            mail = ds.Tables[1].Rows[0]["F100894"].ToString();
                        numeComplet = General.Nz(ds.Tables[1].Rows[0]["F10008"], "").ToString() + " "  + General.Nz(ds.Tables[1].Rows[0]["F10009"], "").ToString();

                        //Radu 29.10.2019 - se scoate INSERT-ul in relGrupUser2
                        //General.ExecutaNonQuery($@"
                        //    BEGIN
                        //        INSERT INTO USERS (F70101, F70102, F70103, F70104, F10003, USER_NO, TIME) VALUES(701, (SELECT MAX(COALESCE(F70102,0)) + 1 FROM USERS), @1, @2, @3, @4, {General.CurrentDate()})
                        //        INSERT INTO ""relGrupUser2""(""IdGrup"", ""IdUser"") VALUES(11, (SELECT MAX(COALESCE(F70102,1)) FROM USERS));
                        //    END;", new object[] { cls.EncryptString(Constante.cheieCriptare, pass, Constante.ENCRYPT), userNume, Session["Marca"], Session["UserId"] });
                        General.ExecutaNonQuery($@"
                        BEGIN
                            INSERT INTO USERS (F70101, F70102, F70103, F70104, F70113, F10003, ""Mail"", ""NumeComplet"", ""IdLimba"", USER_NO, TIME) VALUES(701, @1, @2, @3, @4, @5, @6, @7, 'RO', @8, {General.CurrentDate()})                            
                        END;", new object[] { Dami.NextId("USERS").ToString(), cls.EncryptString(Constante.cheieCriptare, pass, Constante.ENCRYPT), userNume, resetareParola, Session["Marca"], mail, numeComplet, Session["UserId"] });
                    }

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
                    //DateTime dataModif = Convert.ToDateTime(General.ExecutaScalar("SELECT F10023 FROM F100 WHERE F10003=" + General.Nz(Session["Marca"], -99), null));
                    //if (dataModif != Convert.ToDateTime(ds.Tables[1].Rows[0]["F10023"]))
                    //    calcCO = true;
                }

                //Florin 2021.03.08
                // || ds.Tables[i].TableName == "Admin_Beneficii" || ds.Tables[i].TableName == "Admin_Echipamente"
                for (int i = 1; i < ds.Tables.Count; i++)
                {//Radu 10.06.2019
                    if (ds.Tables[i].TableName == "Admin_Medicina" || ds.Tables[i].TableName == "Admin_Sanctiuni" || ds.Tables[i].TableName == "Admin_Cursuri" || ds.Tables[i].TableName == "F100Studii")
                        SalvareSpeciala(ds.Tables[i].TableName);
                    else
                    {
                        //Florin 2020.08.19
                        if (Dami.ValoareParam("SalvareFisierInDisc") == "1" && (ds.Tables[i].TableName == "Atasamente" || ds.Tables[i].TableName == "tblFisiere"))
                        {
                            switch(ds.Tables[i].TableName)
                            {
                                case "tblFisiere":
                                    {
                                        DataRow[] arr = ds.Tables[i].Select("Tabela = 'F100' AND Id = " + Session["Marca"].ToString());
                                        if (arr != null && arr.Count() > 0 && arr[0] != null)
                                        {
                                            DataRow dr = arr[0];
                                            string numeFisier = General.CreazaFisierInDisc(General.Nz(dr["FisierNume"], "Fisier").ToString(), dr["Fisier"], ds.Tables[i].TableName);
                                            dr["Fisier"] = null;
                                            dr["FisierNume"] = numeFisier;
                                        }
                                    }
                                    break;
                                case "Atasamente":
                                    {
                                        Dictionary<int, Personal.Atasamente.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Atasamente"] as Dictionary<int, Personal.Atasamente.metaUploadFile>;
                                        if (lstFiles != null)
                                        {
                                            foreach (var l in lstFiles)
                                            {
                                                DataRow[] arr = ds.Tables[i].Select("IdAuto = " + l.Key);
                                                if (arr != null && arr.Count() > 0 && arr[0] != null)
                                                {
                                                    DataRow dr = arr[0];
                                                    string numeFisier = General.CreazaFisierInDisc(General.Nz(dr["FisierNume"], "Fisier").ToString(), dr["Attach"], ds.Tables[i].TableName);
                                                    dr["Attach"] = null;
                                                    dr["FisierNume"] = numeFisier;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                        }

                        General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                    }
                }


                //Florin 2018-10-30
                //calculam CO daca se modifica data plecare
                //if (calcCO)   Radu 22.05.2020 - calculul CO sa se efectueze la fiecare salvare, deoarece se pot modifica multi factori care influenteaza zilele de CO
                if (!esteNou)
                {
                    try
                    {
                        int an = DateTime.Now.Year;
                        if (ds != null && ds.Tables.Count > 1 && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Rows[0]["F10023"] != DBNull.Value) 
                            an = Convert.ToDateTime(ds.Tables[1].Rows[0]["F10023"]).Year;                        
                        if (an == 2100)
                            an = DateTime.Now.Year;
                        General.CalculCO(an, Convert.ToInt32(General.Nz(Session["Marca"], -99)));
                    }
                    catch (Exception) { }
                }


                //Florin 2020.10.02
                //salvam postul
                General.SalveazaPost(Session["Marca"], Session["MP_IdPost"], DateTime.Now);


                //Radu 15.01.2020
                string[] arrParam = new string[] { HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority, General.Nz(Session["IdClient"], "1").ToString(), General.Nz(Session["IdLimba"], "RO").ToString() };
                int marca = Convert.ToInt32(Session["Marca"] ?? -99);
                int marcaUser = Convert.ToInt32(Session["User_Marca"] ?? -99);
                int idUser = Convert.ToInt32(Session["UserId"] ?? -99);

                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    NotifAsync.TrimiteNotificare("Personal.Lista", (int)Constante.TipNotificare.Notificare, "SELECT Z.*, " + (esteNou ? "1" : "2") + @" AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM F100 Z WHERE F10003=" + marca.ToString(), "F100", marca, idUser, marcaUser, arrParam);
                });

                //Florin 2019.09.23
                GolireVariabile();
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
            try
            {
                string sql = "SELECT * FROM \"" + tabela + "\"";
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
                            if (!dt.Columns[k].AutoIncrement && dt.Columns[k].ColumnName != "Modificabil" && dt.Columns[k].ColumnName != "IdAuto")
                            {
                                string val = "";
                                if (dr[k].GetType() == typeof(int) || dr[k].GetType() == typeof(decimal))
                                    val = dr[k] == null ? "null" : dr[k].ToString();
                                if (dr[k].GetType() == typeof(string))
                                    val = dr[k] == null ? "null" : "'" + dr[k].ToString() + "'";
                                if (dr[k].GetType() == typeof(DateTime))
                                    val = dr[k] == null ? "null" : General.ToDataUniv((DateTime)dr[k]);
                                if (val.Length <= 0)
                                    val = "null";
                                sir += ",\"" + dt.Columns[k].ColumnName + "\" = " + val;
                            }
                        }
                        sql += sir.Substring(1) + " WHERE \"IdAuto\" = " + dt.Rows[i]["IdAuto"].ToString();
                        General.ExecutaNonQuery(sql, null);
           
                        int idAuto = Convert.ToInt32(dt.Rows[i]["IdAuto"].ToString());
                        if (tabela == "Admin_Beneficii")
                        {
                            Dictionary<int, Personal.Beneficii.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, Personal.Beneficii.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                        if (tabela == "Admin_Medicina")
                        {
                            Dictionary<int, Personal.Medicina.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, Personal.Medicina.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                        if (tabela == "Admin_Sanctiuni")
                        {
                            Dictionary<int, Personal.Sanctiuni.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, Personal.Sanctiuni.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                        if (tabela == "Admin_Cursuri")
                        {
                            Dictionary<int, Personal.Cursuri.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, Personal.Cursuri.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                        if (tabela == "F100Studii")
                        {
                            Dictionary<int, Personal.StudiiNou.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Studii"] as Dictionary<int, Personal.StudiiNou.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                        if (tabela == "Admin_Echipamente")
                        {
                            Dictionary<int, Personal.Echipamente.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Echipamente"] as Dictionary<int, Personal.Echipamente.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto))
                            {
                                sql = "DELETE FROM \"tblFisiere\" WHERE \"Tabela\" = '" + tabela + "' AND \"Id\" = " + dt.Rows[i]["IdAuto"].ToString();
                                General.ExecutaNonQuery(sql, null);
                                General.LoadFile(lstFiles[idAuto].UploadedFileName.ToString(), lstFiles[idAuto].UploadedFile, tabela, idAuto);
                            }
                        }
                    }
                    else
                    {//INSERT
                        DataRow dr = dt.Select("IdAuto = " + dt.Rows[i]["IdAuto"].ToString()).FirstOrDefault();
                        sql = "INSERT INTO \"" + tabela + "\" (";
                        string sir = "";
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            if (!dt.Columns[k].AutoIncrement && dt.Columns[k].ColumnName != "Modificabil" && dt.Columns[k].ColumnName != "IdAuto")
                                sir += ",\"" + dt.Columns[k].ColumnName + "\"";
                        }

                        string rez1 = " OUTPUT Inserted.IdAuto ";
                        if (Constante.tipBD == 2) rez1 = "";

                        sql += sir.Substring(1) + ") " + rez1 + " VALUES (";

                        sir = "";
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            if (!dt.Columns[k].AutoIncrement && dt.Columns[k].ColumnName != "Modificabil" && dt.Columns[k].ColumnName != "IdAuto")
                            {
                                string val = "";
                                if (dr[k].GetType() == typeof(int) || dr[k].GetType() == typeof(decimal))
                                    val = dr[k] == null ? "null" : dr[k].ToString();
                                if (dr[k].GetType() == typeof(string))
                                    val = dr[k] == null ? "null" : "'" + dr[k].ToString() + "'";
                                if (dr[k].GetType() == typeof(DateTime))
                                    val = dr[k] == null ? "null" : General.ToDataUniv((DateTime)dr[k]);

                                //Florin 2019.09.17
                                //if (val.Length <= 0)
                                //    val = "''";
                                if (val.Length <= 0)
                                    val = "null";

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
                        if (tabela == "Admin_Beneficii")
                        {
                            Dictionary<int, Personal.Beneficii.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Beneficii"] as Dictionary<int, Personal.Beneficii.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                                General.LoadFile(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);
                        }
                        if (tabela == "Admin_Medicina")
                        {
                            Dictionary<int, Personal.Medicina.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Medicina"] as Dictionary<int, Personal.Medicina.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                                General.LoadFile(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);                    
                        }
                        if (tabela == "Admin_Sanctiuni")
                        {
                            Dictionary<int, Personal.Sanctiuni.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Sanctiuni"] as Dictionary<int, Personal.Sanctiuni.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                                General.LoadFile(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);                   
                        }
                        if (tabela == "Admin_Cursuri")
                        {
                            Dictionary<int, Personal.Cursuri.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Cursuri"] as Dictionary<int, Personal.Cursuri.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                                General.LoadFile(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);
                        }
                        if (tabela == "F100Studii")
                        {
                            Dictionary<int, Personal.StudiiNou.metaUploadFile> lstFiles = Session["List_DocUpload_MP_Studii"] as Dictionary<int, Personal.StudiiNou.metaUploadFile>;
                            if (lstFiles != null && lstFiles.ContainsKey(idAuto1))
                                General.LoadFile(lstFiles[idAuto1].UploadedFileName.ToString(), lstFiles[idAuto1].UploadedFile, tabela, idAuto);
                        }
                    }
                }
                if (tabela == "Admin_Beneficii")
                    Session["List_DocUpload_MP_Beneficii"] = null;

                if (tabela == "Admin_Medicina")
                    Session["List_DocUpload_MP_Medicina"] = null;
            
                if (tabela == "Admin_Sanctiuni")
                    Session["List_DocUpload_MP_Sanctiuni"] = null;

                if (tabela == "Admin_Cursuri")
                    Session["List_DocUpload_MP_Cursuri"] = null;

                if (tabela == "F100Studii")
                    Session["List_DocUpload_MP_Studii"] = null;

                if (tabela == "Admin_Echipamente")
                    Session["List_DocUpload_MP_Echipamente"] = null;

                if (General.Nz(Session["FisiereDeSters"],"").ToString() != "")
                {
                    string[] arr = Session["FisiereDeSters"].ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string l in arr)
                    {
                        if (File.Exists(HostingEnvironment.MapPath("~/" + l)))
                            File.Delete(HostingEnvironment.MapPath("~/" + l));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void Initializare(ref DataSet ds)
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
                        case "F10096":
                            row100[x] = idSablon;
                            rowComb[x] = idSablon;
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
                    if (dt.Columns[i].ColumnName != "F09903" && dt.Columns[i].ColumnName != "F099985" && dt.Columns[i].ColumnName != "F099986" && dt.Columns[i].ColumnName != "F09922" && dt.Columns[i].ColumnName != "F09996"
                        && dt.Columns[i].ColumnName != "F09908" && dt.Columns[i].ColumnName != "F09909" && dt.Columns[i].ColumnName != "USER_NO" && dt.Columns[i].ColumnName != "TIME")
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

                

                if (Dami.ValoareParam("MP_FolosesteOrganigrama") == "1" && Convert.ToString(Session["MP_IdPost"]) != "")
                {
                    DataRow dr = General.IncarcaDR($@"SELECT * FROM ""Org_Posturi"" WHERE Id=@1 AND {General.TruncateDate("DataInceput")} <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= {General.TruncateDate("DataSfarsit")}", new object[] { Session["MP_IdPost"] });
                    if (dr != null)
                    {
                        //Functia
                        if (dr["IdFunctie"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10071"] = dr["IdFunctie"];
                            ds.Tables["F100"].Rows[0]["F10071"] = dr["IdFunctie"];
                        }
                            
                        //cor
                        if (dr["CodCOR"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10098"] = dr["CodCOR"];
                            ds.Tables["F100"].Rows[0]["F10098"] = dr["CodCOR"];
                        }
                        
                        //Nivel functie - se face automat in Page_init din Personal/Contract

                        //structura organizatorica
                        if (dr["F10002"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10002"] = dr["F10002"];
                            ds.Tables["F100"].Rows[0]["F10002"] = dr["F10002"];
                        }
                        if (dr["F10004"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10004"] = dr["F10004"];
                            ds.Tables["F100"].Rows[0]["F10004"] = dr["F10004"];
                        }
                        if (dr["F10005"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10005"] = dr["F10005"];
                            ds.Tables["F100"].Rows[0]["F10005"] = dr["F10005"];
                        }
                        if (dr["F10006"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10006"] = dr["F10006"];
                            ds.Tables["F100"].Rows[0]["F10006"] = dr["F10006"];
                        }
                        if (dr["F10007"] != DBNull.Value)
                        {
                            ds.Tables[0].Rows[0]["F10007"] = dr["F10007"];
                            ds.Tables["F100"].Rows[0]["F10007"] = dr["F10007"];
                        }

                        Session["MP_SalariulMinPost"] = Convert.ToInt32(General.Nz(dr["SalariuMin"],0));
                        //Florin 2021.03.03 #8
                        General.AdaugaDosar(ref ds, Session["Marca"]);
                        General.AdaugaBeneficiile(ref ds, Session["Marca"]);
                        General.AdaugaEchipamente(ref ds, Session["Marca"]);
                    }
                }

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
                int marcaInit = -99;
                int marcaFin = -99;
              
                marcaInit = Convert.ToInt32(dt100.Rows[0]["F10003"].ToString());
                marcaFin = marcaInit;

                string strSQL = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'MP_MarcaFaraSecventa'";
                DataTable dtSec = General.IncarcaDT(strSQL, null);
                if (dtSec != null && dtSec.Rows.Count > 0 && dtSec.Rows[0][0] != null && dtSec.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtSec.Rows[0][0].ToString()) == 1)
                {
                    DataTable dtMarca = General.IncarcaDT("SELECT MAX(F10003) + 1 FROM F100 where f10003 <= coalesce((select cast(\"Valoare\" as integer) from \"tblParametrii\" where \"Nume\" = 'MP_LimitaMaximaMarca') , 10000000)", null);
                    if (dtMarca != null && dtMarca.Rows.Count > 0 && dtMarca.Rows[0][0] != null && dtMarca.Rows[0][0].ToString().Length > 0)
                        marcaFin = Convert.ToInt32(dtMarca.Rows[0][0].ToString());
                    if (marcaInit != marcaFin)
                    {
                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar("SELECT COUNT(*) FROM F100 WHERE F10003 =@1", new object[] { marcaInit }), 0));
                        if (cnt != 0)
                        {
                            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                            dt100.Rows[0]["F10003"] = marcaFin;
                            dt1001.Rows[0]["F10003"] = marcaFin;

                            dt100.Rows[0]["F100985"] = marcaFin;

                            if (ds.Tables.Contains("Admin_Activitati") && ds.Tables["Admin_Activitati"].Rows.Count > 0)
                                ds.Tables["Admin_Activitati"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("F100Adrese") && ds.Tables["F100Adrese"].Rows.Count > 0)
                                ds.Tables["F100Adrese"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("Atasamente") && ds.Tables["Atasamente"].Rows.Count > 0)
                                ds.Tables["Atasamente"].Rows[0]["IdEmpl"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Atestate") && ds.Tables["Admin_Atestate"].Rows.Count > 0)
                                ds.Tables["Admin_Atestate"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Beneficii") && ds.Tables["Admin_Beneficii"].Rows.Count > 0)
                                ds.Tables["Admin_Beneficii"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("F100Cartele2") && ds.Tables["F100Cartele2"].Rows.Count > 0)
                                ds.Tables["F100Cartele2"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("F100CentreCost2") && ds.Tables["F100CentreCost2"].Rows.Count > 0)
                                ds.Tables["F100CentreCost2"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("F100Contacte") && ds.Tables["F100Contacte"].Rows.Count > 0)
                                ds.Tables["F100Contacte"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("F100Contracte2") && ds.Tables["F100Contracte2"].Rows.Count > 0)
                                ds.Tables["F100Contracte2"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Cursuri") && ds.Tables["Admin_Cursuri"].Rows.Count > 0)
                                ds.Tables["Admin_Cursuri"].Rows[0]["Marca"] = marcaFin;
                            General.ExecutaNonQuery("UPDATE F112 SET F11203 = " + marcaFin + " WHERE F11203 = " + marcaInit, null);
                            if (ds.Tables.Contains("Admin_Echipamente") && ds.Tables["Admin_Echipamente"].Rows.Count > 0 )
                                ds.Tables["Admin_Echipamente"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Evaluare") && ds.Tables["Admin_Evaluare"].Rows.Count > 0)
                                ds.Tables["Admin_Evaluare"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Evolutie") && ds.Tables["Admin_Evolutie"].Rows.Count > 0)
                                ds.Tables["Admin_Evolutie"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Experienta") && ds.Tables["Admin_Experienta"].Rows.Count > 0)
                                ds.Tables["Admin_Experienta"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("relGrupAngajat2") && ds.Tables["relGrupAngajat2"].Rows.Count > 0)
                                ds.Tables["relGrupAngajat2"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Limbi") && ds.Tables["Admin_Limbi"].Rows.Count > 0)
                                ds.Tables["Admin_Limbi"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Medicina") && ds.Tables["Admin_Medicina"].Rows.Count > 0)
                                ds.Tables["Admin_Medicina"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("F110") && ds.Tables["F110"].Rows.Count > 0)
                                ds.Tables["F110"].Rows[0]["F11003"] = marcaFin;
                            if (ds.Tables.Contains("Admin_Sanctiuni") && ds.Tables["Admin_Sanctiuni"].Rows.Count > 0)
                                ds.Tables["Admin_Sanctiuni"].Rows[0]["Marca"] = marcaFin;
                            if (ds.Tables.Contains("F100Studii") && ds.Tables["F100Studii"].Rows.Count > 0)
                                ds.Tables["F100Studii"].Rows[0]["F10003"] = marcaFin;
                            if (ds.Tables.Contains("F100Supervizori2") && ds.Tables["F100Supervizori2"].Rows.Count > 0)
                                ds.Tables["F100Supervizori2"].Rows[0]["F10003"] = marcaFin;
                            General.ExecutaNonQuery("UPDATE F111 SET F11103 = " + marcaFin + " WHERE F11103 = " + marcaInit, null);

                            Session["MP_Mesaj"] = "Angajatului i-a fost atribuita o noua marca: " + marcaFin;
                        }
                    }
                }   

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
                string strSQL = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'MP_MarcaFaraSecventa'";
                DataTable dtSec = General.IncarcaDT(strSQL, null);
                if (dtSec != null && dtSec.Rows.Count > 0 && dtSec.Rows[0][0] != null && dtSec.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dtSec.Rows[0][0].ToString()) == 1)
                {//se va lua max + 1 din F100
                    DataTable dtMarca = General.IncarcaDT("SELECT MAX(F10003) + 1 FROM F100 where f10003 <= coalesce((select cast(\"Valoare\" as integer) from \"tblParametrii\" where \"Nume\" = 'MP_LimitaMaximaMarca') , 10000000)", null);
                    if (dtMarca != null && dtMarca.Rows.Count > 0 && dtMarca.Rows[0][0] != null && dtMarca.Rows[0][0].ToString().Length > 0)
                        id = Convert.ToInt32(dtMarca.Rows[0][0].ToString());                    
                }
                else
                {//se va citi din secventa
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
                DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
                DataTable dt = new DataTable();
                if (dtSec != null && dtSec.Rows.Count > 0)
                {
                    dt = dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl = '" + numeTab + "'") != null && dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl='" + numeTab + "'").Count() > 0 ? dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl = '" + numeTab + "'").CopyToDataTable() : null;
                }
                else
                    return;

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
                DataTable dtSec = HttpContext.Current.Session["SecuritatePersonal"] as DataTable;
                DataTable dt = new DataTable();
                if (dtSec != null && dtSec.Rows.Count > 0)
                {
                    dt = dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'") != null && dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'").Count() > 0 ? dtSec.Select("(IdForm = 'Personal.Lista' OR IdForm = 'Personal.DateAngajat') AND IdControl like '%_I%'").CopyToDataTable() : null;
                }
                else
                    return null;

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

        private void AdaugaValorile()
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                string[] tabs = { "DateIdentificare", "Contract", "DateGenerale", "Detasari", "Diverse", "Documente", "Studii", "Banca" };
                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");

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
                lstCtr.Add("txtSalariu", salariu);
                lstCtr.Add("deDataModifSal", "F100991");
                lstCtr.Add("cmbCategAng1", "F10061");
                lstCtr.Add("cmbCategAng2", "F10062");
                lstCtr.Add("txtLocAnt", "F10078");                
                lstCtr.Add("cmbLocatieInt", "F100966");
                lstCtr.Add("cmbGrilaSal", "F1001136");
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
                lstCtr.Add("chkCalcDed", "F10056");
                lstCtr.Add("txtPerProbaZL", "F100975");
                lstCtr.Add("txtPerProbaZC", "F1001063");
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

                //lstCtr.Add("hfNrLuni", "F100935");
                //lstCtr.Add("hfNrAni", "F100936");
                lstCtr.Add("txtNrLuni", "F100935");
                lstCtr.Add("txtNrZile", "F100936");
                //Radu 12.09.2019 - caz special
                lstCtr.Add("cmbNivelFunctie", "F71813");

                lstCtr.Add("deDataModTipCtr", "F1001137");
                lstCtr.Add("deDataModDurCtr", "F1001138");
                lstCtr.Add("deDataConsemn", "F1001109");

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

                lstDS.Add("chk1", "F1001125");
                lstDS.Add("chk2", "F1001126");
                lstDS.Add("chk3", "F1001127");
                lstDS.Add("chk4", "F1001128");
                lstDS.Add("chk5", "F1001129");
                #endregion

                //Diverse
                #region Diverse
                Dictionary<String, String> lstDV = new Dictionary<string, string>();
                lstDV.Add("txtNr", "F10011");
                lstDV.Add("txtNorma", "F10043");
                lstDV.Add("deData", "FX1");
                lstDV.Add("txtLocNastere", "F100980");
                lstDV.Add("cmbStudiiDiv", "F10050");
                lstDV.Add("txtStudiiDet", "F1001131");
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
                lstDO.Add("cmbResedinta", "F1001071");
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
                lstDO.Add("txtCalif1", "F1001132");
                lstDO.Add("txtCalif2", "F1001133");
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
                lstDO.Add("txtCamp1", "F100902");
                lstDO.Add("txtCamp2", "F100903");
                lstDO.Add("txtCamp3", "F100904");
                lstDO.Add("txtMarcaVeche", "F100943");
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
  
                                if (colName == "F71813")
                                {
                                    //Radu 12.09.2019 - salvare nivel functie in F718
                                    if (ds.Tables[1].Rows[0]["F10071"] != null && ds.Tables[1].Rows[0]["F10071"].ToString().Length > 0 && ctl.Value != null)
                                        General.ExecutaNonQuery("UPDATE F718 SET F71813 = " + (ctl.Value ?? "NULL") + " WHERE F71802 = " + ds.Tables[1].Rows[0]["F10071"].ToString(), null);
                                    continue;
                                }                         

                                DataTable dt = new DataTable();
                                if (cols1.Contains(colName)) dt = ds.Tables[1];
                                if (cols2.Contains(colName)) dt = ds.Tables[2];

                                if (ctl != null && General.Nz(dt.Rows[0][colName], "").ToString() != General.Nz(ctl.Value, "").ToString())
                                {   
                                    dt.Rows[0][colName] = ctl.Value ?? DBNull.Value;
                                }

                                if (dt.Rows[0][colName].GetType() == typeof(DateTime))
                                {
                                    DateTime data = Convert.ToDateTime(ctl.Value ?? new DateTime(2100, 1, 1));
                                    dt.Rows[0][colName] = new DateTime(data.Year, data.Month, data.Day);
                                }

                                DataTable dt2 = new DataTable();
                                if (cols3.Contains(colName)) dt2 = ds.Tables[0];
                                if (ctl != null && General.Nz(dt2.Rows[0][colName], "").ToString() != General.Nz(ctl.Value, "").ToString())
                                {
                                    dt2.Rows[0][colName] = ctl.Value ?? DBNull.Value;
                                }

                                if (dt2.Rows[0][colName].GetType() == typeof(DateTime))
                                {
                                    DateTime data = Convert.ToDateTime(ctl.Value ?? new DateTime(2100, 1, 1));
                                    dt2.Rows[0][colName] = new DateTime(data.Year, data.Month, data.Day);
                                }

                                switch (colName)
                                {
                                    case "F100271":
                                        if (hfDate.Contains("DataValabInvalid"))
                                            ds.Tables[1].Rows[0]["F100271"] = Convert.ToDateTime(hfDate["DataValabInvalid"]);
                                        break;
                                    case "F100933":
                                        if (hfDate.Contains("DeLaData"))
                                            ds.Tables[1].Rows[0]["F100933"] = Convert.ToDateTime(hfDate["DeLaData"]);
                                        break;
                                    case "F100934":
                                        if (hfDate.Contains("LaData"))
                                            ds.Tables[1].Rows[0]["F100934"] = Convert.ToDateTime(hfDate["LaData"]);
                                        break;
                                    case "F10023":
                                        if (hfDate.Contains("UltimaZiLucr"))
                                            ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(hfDate["UltimaZiLucr"]);
                                        break;
                                    case "F100993":
                                        if (hfDate.Contains("DataPlecarii"))
                                            ds.Tables[1].Rows[0]["F100993"] = Convert.ToDateTime(hfDate["DataPlecarii"]);
                                        break;
                                    case "F100935":
                                        if (hfDate.Contains("NrLuni"))
                                            ds.Tables[1].Rows[0]["F100935"] = Convert.ToInt32(hfDate["NrLuni"]);
                                        break;
                                    case "F100936":
                                        if (hfDate.Contains("NrZile"))
                                            ds.Tables[1].Rows[0]["F100936"] = Convert.ToInt32(hfDate["NrZile"]);
                                        break;
                                    default:
                                        break;
                                }

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

        private void GolireVariabile()
        {
            try
            {
                Session["Marca"] = null;
                Session["InformatiaCurentaPersonal"] = null;
                Session["MP_CautaAdresa"] = null;
                Session["DocUpload_MP_Atasamente"] = null;
                Session["Marca_Atribut"] = null;
                Session["MP_Avans"] = null;
                Session["MP_Avans_Tab"] = null;
                Session["DocUpload_MP_Beneficii"] = null;
                Session["List_DocUpload_MP_Beneficii"] = null;
                Session["DocUpload_MP_Echipamente"] = null;
                Session["List_DocUpload_MP_Echipamente"] = null;
                Session["DocUpload_MP_Medicina"] = null;
                Session["List_DocUpload_MP_Medicina"] = null;
                Session["DocUpload_MP_Sanctiuni"] = null;
                Session["List_DocUpload_MP_Sanctiuni"] = null;
                Session["DocUpload_MP_Studii"] = null;
                Session["List_DocUpload_MP_Studii"] = null;
                Session["AdresaSelectata"] = null;
                Session["CodCORSelectat"] = null;
                Session["esteNou"] = null;
                Session["IdSablon"] = "";
                Session["InformatiaCurentaPersonalCalcul"] = null;
                Session["MP_DiferentaLuni"] = "";
                Session["MP_Grila"] = "";
                Session["MP_ComboTN"] = "";
                Session["MP_ComboDTM"] = "";
                Session["MP_AreContract"] = "";
                Session["MP_DataSfarsit36"] = "";
                Session["MP_SalMin"] = "";
                Session["MP_NvlFunc"] = "";
                Session["AdresaCompusa"] = null;
                Session["AdresaSelectata"] = null;
                Session["MP_NuPermiteCNPInvalid"] = null;
                Session["MP_Candidat"] = null;
                Session["PreluareDate"] = null;
                Session["DateIdentificare_Fisier"] = null;
                Session["MP_ComboTipDoc"] = null;
                Session["Admin_Sanctiuni"] = null;
                Session["Sporuri_cmbMaster1"] = null;
                Session["Tarife_cmbMaster"] = null;

                //Florin 2020.08.18
                Session["FisiereDeSters"] = "";

                //Florin 2020.08.20
                Session["List_DocUpload_MP_Atasamente"] = null;

                //Florin 2020.10.02
                Session["MP_IdPost"] = null;
                Session["MP_SalariulMinPost"] = 0;

                Session["DocUpload_MP_Dosar"] = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}