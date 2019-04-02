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

                    Initializare(ds);
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

                    if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
                    {//DNATA
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
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                //SqlDataAdapter da = new SqlDataAdapter();
                //SqlCommandBuilder cb = new SqlCommandBuilder();
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM F100 ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(ds.Tables[1]);
                //da.Dispose();
                //da = null;

                //da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM F1001 ", null);
                //cb = new SqlCommandBuilder(da);
                //da.Update(ds.Tables[2]);
                //da.Dispose();
                //da = null;

                if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
                {//DNATA
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
                    if (ds.Tables[0].Rows[0]["F100939"] == null || ds.Tables[0].Rows[0]["F100939"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100939"].ToString() == "0")
                        mesaj += " - interval repartizare timp munca" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100964"] == null || ds.Tables[0].Rows[0]["F100964"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100964"].ToString() == "0")
                        mesaj += " - nr ore pe luna/saptamana" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10098"] == null || ds.Tables[0].Rows[0]["F10098"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10098"].ToString() == "0")
                        mesaj += " - COR" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F10071"] == null || ds.Tables[0].Rows[0]["F10071"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10071"].ToString() == "0")
                        mesaj += " - functie" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F1001063"] == null || ds.Tables[0].Rows[0]["F1001063"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F1001063"].ToString() == "0")
                        mesaj += " - perioada de proba (zile lucratoare)" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100975"] == null || ds.Tables[0].Rows[0]["F100975"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100975"].ToString() == "0")
                        mesaj += " - perioada de proba (zile calendaristice)" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F1009742"] == null || ds.Tables[0].Rows[0]["F1009742"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F1009742"].ToString() == "0")
                        mesaj += " - nr zile preaviz demisie" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100931"] == null || ds.Tables[0].Rows[0]["F100931"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100931"].ToString() == "0")
                        mesaj += " - nr zile preaviz concediere" + Environment.NewLine;
                    if (ds.Tables[0].Rows[0]["F100642"] == null || ds.Tables[0].Rows[0]["F100642"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F100642"].ToString() == "0")
                        mesaj += " - zile CO cuvenite an curent" + Environment.NewLine;
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


                    if (ds.Tables[0].Rows[0]["F10079"] == null || ds.Tables[0].Rows[0]["F10079"].ToString().Length <= 0 || ds.Tables[0].Rows[0]["F10079"].ToString() == "0")
                        mesaj += " - punct de lucru" + Environment.NewLine;
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

                string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'NuPermiteCNPInvalid'";
                DataTable dt = General.IncarcaDT(sql, null);
                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 && Convert.ToInt32(dt.Rows[0][0].ToString()) == 1)
                    if (!General.VerificaCNP(ds.Tables[1].Rows[0]["F10017"].ToString()))
                    {
                        MessageBox.Show("CNP invalid!", MessageBox.icoError);
                        return;
                    }

                DateIdentificare dateId = new DateIdentificare();
                int varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                if (varsta < 16)
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu puteti angaja o persoana cu varsta mai mica de 16 ani!"), MessageBox.icoError);
                    return;
                }

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
                    Contract ctr = new Contract();
                    DateTime dataRevisal = ctr.SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
                    if (dataRevisal.Date < DateTime.Now.Date)
                    {
                        MessageBox.Show("Termen depunere Revisal depasit!", MessageBox.icoError);
                        return;
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
                {
                    //da = new SqlDataAdapter();
                    //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"" + ds.Tables[i].TableName + "\"", null);
                    //cb = new SqlCommandBuilder(da);
                    //da.Update(ds.Tables[i]);
                    //da.Dispose();
                    //da = null;
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

                //MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);

                //DataTable dtDept = General.IncarcaDT("SELECT F00608 FROM F006 WHERE F00607 = " + ds.Tables[0].Rows[0]["F10007"].ToString(), null);
                //lblDateAngajat.Text = ds.Tables[0].Rows[0]["F10008"].ToString() + " " + ds.Tables[0].Rows[0]["F10009"].ToString() + ", Marca: " + ds.Tables[0].Rows[0]["F10003"].ToString()
                //                    + ", Deptartament: " + dtDept.Rows[0]["F00608"].ToString();

                Response.Redirect("~/Personal/Lista.aspx", false);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void Initializare(DataSet ds)
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
            dt100.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };

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
            dt1001.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };

            if (dtComb.Rows.Count > 0)
                dtComb.Rows.RemoveAt(0);
            dtComb.Rows.Add(rowComb);
            dtComb.PrimaryKey = new DataColumn[] { dt.Columns["F10003"] };

            for (int i = 0; i < dt.Columns.Count; i++)
                if (dt.Columns[i].ColumnName != "F09903" && dt.Columns[i].ColumnName != "F099985" && dt.Columns[i].ColumnName != "F09908" && dt.Columns[i].ColumnName != "F09909" && dt.Columns[i].ColumnName != "USER_NO" && dt.Columns[i].ColumnName != "TIME")
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

        protected void InserareAngajat(string marca, DataTable dt100, DataTable dt1001)
        {

            //SqlDataAdapter da = new SqlDataAdapter();
            //SqlCommandBuilder cb = new SqlCommandBuilder();
            //da = new SqlDataAdapter();
            //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM F100", null);
            //cb = new SqlCommandBuilder(da);
            //da.Update(dt100);
            //da.Dispose();
            //da = null;
            General.SalveazaDate(dt100, "F100");

            //da = new SqlDataAdapter();
            //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM F1001", null);
            //cb = new SqlCommandBuilder(da);
            //da.Update(dt1001);
            //da.Dispose();
            //da = null;
            General.SalveazaDate(dt1001, "F1001");
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
            catch (Exception)
            {
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
            return lista;
        }

        protected void ASPxPageControl2_ActiveTabChanged(object source, TabControlEventArgs e)
        {
            string ert = "safdasdf";
        }

        protected void ASPxPageControl2_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                ASPxPageControl ctl = sender as ASPxPageControl;
                if (ctl == null) return;
                TabPage tab = ctl.ActiveTabPage;
                //TabPage tab = e.Tab;
                
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }
    }
}