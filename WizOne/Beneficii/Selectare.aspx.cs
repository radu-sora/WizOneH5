using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Beneficii
{
    public partial class Selectare : System.Web.UI.Page
    {

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

      

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");
                lblBen.InnerText = Dami.TraduCuvant("In acest moment, urmatoarele Beneficii sunt active:", "In acest moment, urmatoarele Beneficii sunt active:");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnValidare.Text = Dami.TraduCuvant("btnValidare", "Validare selectie Beneficii");



                foreach (GridViewColumn c in grDateBen.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                foreach (GridViewColumn c in grDateSes.Columns)
                {
                    try
                    {
                        if (c.GetType() == typeof(GridViewDataColumn))
                        {
                            GridViewDataColumn col = c as GridViewDataColumn;
                            col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        }
                    }
                    catch (Exception) { }
                }

                #endregion

                txtTitlu.Text = Dami.TraduCuvant("Vizualizare si selectare Beneficii", "Vizualizare si selectare Beneficii");


                DataTable dtBen = General.IncarcaDT(SelectBeneficii(), null);
                GridViewDataComboBoxColumn colBen = (grDateBen.Columns["Id"] as GridViewDataComboBoxColumn);
                colBen.PropertiesComboBox.DataSource = dtBen;
                GridViewDataComboBoxColumn colBenSes = (grDateSes.Columns["Id"] as GridViewDataComboBoxColumn);
                colBenSes.PropertiesComboBox.DataSource = dtBen;

                if (!IsPostBack)
                {
                    Session["Select_GridBen"] = null;
                    Session["Select_GridSes"] = null;
                    DataTable dtSes = General.IncarcaDT("SELECT COUNT(*) FROM Ben_Sesiuni WHERE F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99) + "  AND DataInceput <= GETDATE() AND GETDATE() <= DataSfarsit ", null);
                    if (dtSes == null || dtSes.Rows.Count <= 0 || Convert.ToInt32(dtSes.Rows[0][0].ToString()) <= 0)
                    {
                        string msg = "In acest moment nu este disponibila o sesiune pentru selectarea Beneficiilor. Vei primi un e-mail de informare atunci cand urmatoarea sesiune va deveni activa.";
                        lblSes.InnerText = Dami.TraduCuvant(msg);
                        grDateSes.Visible = false;
                        btnValidare.Visible = false;
                        Session["SelectareBeneficii"] = 0;
                        Session["SelectareText"] = msg;
                    }
                    else
                    {
                        IncarcaGridSes();                        
                        DataTable dt = Session["SelectBen_Ses"] as DataTable;
                        string msg = "Sesiunea pentru selectarea Beneficiilor este deschisa pana in data de " + Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).ToShortDateString() 
                            + ".\n Selectarea Beneficiilor se face pentru perioda " + Convert.ToDateTime(dt.Rows[0]["DataInceput"].ToString()).ToShortDateString() + " - " +
                             Convert.ToDateTime(dt.Rows[0]["DataSfarsit"].ToString()).ToShortDateString() + ".";               
                        switch (Convert.ToInt32(dt.Rows[0]["IdStare"].ToString()))
                        {
                            case 1:
                                msg += "\nPana in acest moment nu ti-ai exprimat optiunea privind selectarea Beneficiilor. Te rugam sa faci acest lucru pana la expirarea sesiunii.";
                                Session["SelectareBeneficii"] = 1;
                                Session["SelectareText"] = msg;
                                lblSes.InnerText = Dami.TraduCuvant(msg);
                                break;
                            case 2:
                                msg += "\nOptiunea ta privind selectarea Beneficiilor este in curs de aprobare. Vei primi un e-mail in momentul in care solicitarea ta va fi validata.";
                                Session["SelectareBeneficii"] = 2;
                                Session["SelectareText"] = msg;
                                lblSes.InnerText = Dami.TraduCuvant(msg);
                                break;
                            case 3:
                                msg += "\nOptiunea ta privind selectarea Beneficiilor a fost aprobata!";
                                Session["SelectareBeneficii"] = 3;
                                Session["SelectareText"] = msg;
                                lblSes.InnerText = Dami.TraduCuvant(msg);
                                btnValidare.Enabled = false;
                                break;
                            case 4:
                                msg += "\nOptiunea ta privind selectarea Beneficiilor a fost respinsa! Motivul este urmatorul:" + dt.Rows[0]["Motiv"].ToString() + ". Te rugam sa efectuezi o noua selectie pana la expirarea sesiunii.";
                                Session["SelectareBeneficii"] = 4;
                                Session["SelectareText"] = msg;
                                lblSes.InnerText = Dami.TraduCuvant(msg);
                                btnValidare.Enabled = false;
                                break;

                        }
                    }
                }
                else
                {
                    string msg = Session["SelectareText"].ToString();
                    lblSes.InnerText = Dami.TraduCuvant(msg);
                    if (Session["SelectareBeneficii"].ToString() == "0")
                    {
                        grDateSes.Visible = false;
                        btnValidare.Visible = false;
                    }
                    if (Session["SelectareBeneficii"].ToString() == "3" || Session["SelectareBeneficii"].ToString() == "4")
                        btnValidare.Enabled = false;
                }

                IncarcaGridBen();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnValidare_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> lst = grDateSes.GetSelectedFieldValues(new string[] { "Id", "Descriere" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {

                    grDateSes.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                    return;
                }

                object[] arr = lst[0] as object[];

                DataTable dt = Session["SelectBen_Ses"] as DataTable;

                General.ExecutaNonQuery("UPDATE \"Ben_Sesiuni\" SET \"Descriere\" = '" + arr[1].ToString() + "', IdBeneficiu = " + arr[0].ToString()
                    + ", IdStare = 2  WHERE \"IdSesiune\" = " + dt.Rows[0]["IdSesiune"].ToString() + " AND F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99), null);

                Session["SelectareBeneficii"] = 2;
                string msg = Session["SelectareText"].ToString();
                string[] param = msg.Split('\n');
                lblSes.InnerText = Dami.TraduCuvant(param[0] + "\n" + param[1] + "\n" + "Optiunea ta privind selectarea Beneficiilor este in curs de aprobare. Vei primi un e-mail in momentul in care solicitarea ta va fi validata.");

                MessageBox.Show(Dami.TraduCuvant("Proces realizat cu succes!"), MessageBox.icoSuccess);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        private void IncarcaGridBen()
        {
            DataTable dt = new DataTable();

            try
            {


                if (Session["Select_GridBen"] == null)
                {
                    dt = SelectGridBen();
                    grDateBen.DataSource = dt;
                    grDateBen.KeyFieldName = "Id";
                    grDateBen.DataBind();
                    Session["Select_GridBen"] = dt;
                }
                else
                {
                    grDateBen.KeyFieldName = "Id";

                    dt = Session["Select_GridBen"] as DataTable;
                    grDateBen.DataSource = dt;
                    grDateBen.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGridSes()
        {
            DataTable dt = new DataTable();

            try
            {


                if (Session["Select_GridSes"] == null)
                {
                    dt = SelectGridSes();
                    grDateSes.DataSource = dt;
                    grDateSes.KeyFieldName = "Id";
                    grDateSes.DataBind();
                    Session["Select_GridSes"] = dt;
                }
                else
                {
                    grDateSes.KeyFieldName = "Id";

                    dt = Session["Select_GridSes"] as DataTable;
                    grDateSes.DataSource = dt;
                    grDateSes.DataBind();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private string SelectBeneficii()
        {
            string strSql = "";

            try
            {
                strSql = @"select CAST (a.""Id"" AS INT) as ""Id"", a.""Denumire"", a.""DeLaData"", a.""LaData"", a.""Descriere""
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal') ORDER BY a.""Denumire""";


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

     
        public DataTable SelectGridBen()
        {

            DataTable q = null;

            try
            {
                string strSql = "";

               strSql = @"select a.""IdCategorie"", a.""Id"", a.""Denumire"", a.""DeLaData"", a.""LaData"", a.""Descriere"", a.USER_NO, a.TIME, a.ValoareEstimata
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal')
                                AND a.""DeLaData"" <= getdate() and getdate() <= a.""LaData""
                                ORDER BY a.""Denumire""";

                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable SelectGridSes()
        {

            DataTable q = null;

            try
            {
                string strSql = "";

                strSql = "SELECT IdAuto, F10003, IdSesiune, IdBeneficiu, DataInceput, DataSfarsit, IdStare, DataInceputBen, DataSfarsitBen, Descriere, Motiv, USER_NO, TIME FROM Ben_Sesiuni WHERE DataInceput <= GETDATE() AND GETDATE() <= DataSfarsit AND F10003 = " + Convert.ToInt32(Session["User_Marca"] ?? -99);
                DataTable dt = General.IncarcaDT(strSql, null);
                Session["SelectBen_Ses"] = dt;

                strSql = @"select a.""Id"", a.""Descriere"", a.USER_NO, a.TIME
                                from ""Admin_Obiecte"" a
                                inner join ""Admin_Categorii"" b on a.""IdCategorie"" = b.""Id""
                                where b.""IdArie"" = (select ""Valoare"" from ""tblParametrii"" where ""Nume"" = 'ArieTabBeneficiiDinPersonal')
                                AND a.""DeLaData"" <=GETDATE() AND GETDATE() <= a.""LaData""
                                ORDER BY a.""Denumire""";


                q = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }



    }
}