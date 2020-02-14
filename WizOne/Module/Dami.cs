using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ProceseSec;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DevExpress.Web;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace WizOne.Module
{
    public class Dami
    {
        public class metaGeneral
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        public class metaGeneral2
        {
            public string Id { get; set; }
            public string Denumire { get; set; }
        }

        //public static string CnnWeb()
        //{
        //    string cnn = "";

        //    try
        //    {
        //        CriptDecript prc = new CriptDecript();
        //        cnn = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Dami", "CnnWeb");
        //    }

        //    return cnn;
        //}


        public static void InitCnn()
        {
            try
            {
                CriptDecript prc = new CriptDecript();
                Constante.cnnWeb = prc.EncryptString(Constante.cheieCriptare, ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString, 2);
                Constante.cnnRap = ConfigurationManager.ConnectionStrings["cnRap"]?.ConnectionString ?? ConfigurationManager.ConnectionStrings["cnWeb"].ConnectionString;
                Constante.cnnRap = prc.EncryptString(Constante.cheieCriptare, Constante.cnnRap, 2);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string TraduCuvant(string nume, string cuvant = "")
        {
            string rez = cuvant;

            try
            {
                //var ert = (string)HttpContext.GetLocalResourceObject("General", nume.ToString().Replace(" ", "").Replace("!", "").Replace("/", "").Replace("-", "").Replace("+", ""), new CultureInfo(General.VarSession("IdLimba").ToString()));
                //var ert = HttpContext.GetLocalResourceObject("../App_LocalResources/General", "btnSave");
                //var ert = HttpContext.GetLocalResourceObject("General", "btnSave");
                //rez = (string)HttpContext.GetLocalResourceObject("General", nume.ToString().Replace(" ", "").Replace("!", "").Replace("/", "").Replace("-", "").Replace("+", ""), new CultureInfo(General.VarSession("IdLimba").ToString()));
                //if (General.Nz(rez,"").ToString() == "")
                //{

                string idLimba = "RO";
                if (HttpContext.Current == null || HttpContext.Current.Session["IdLimba"] == null || HttpContext.Current.Session["IdLimba"].ToString().Trim() == "")
                    idLimba = Dami.ValoareParam("IdLimba");
                else
                    idLimba = General.Nz(HttpContext.Current.Session["IdLimba"],"RO").ToString();

                rez = (string)HttpContext.GetGlobalResourceObject("General", nume.ToString().Replace(" ", "").Replace("!", "").Replace("/", "").Replace("-", "").Replace("+", "").Replace(".", ""), new CultureInfo(idLimba));

                    if (rez == null || rez == "")
                    {
                        if (cuvant != "")
                            rez = cuvant;
                        else
                            rez = nume;
                    }
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static string TraduMeniu(string nume, string cuvant = "")
        {
            string rez = cuvant;

            try
            {
                rez = (string)HttpContext.GetGlobalResourceObject("Meniu", nume.ToString().Replace(" ", "").Replace("!", "").Replace("/", "").Replace("-", ""), new CultureInfo(General.VarSession("IdLimba").ToString()));
                if (rez == null || rez == "")
                {
                    if (cuvant != "")
                        rez = cuvant;
                    else
                        rez = nume;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }



        //public static void Securitate(ContentPlaceHolder pag)
        //{
        //    try
        //    {
        //        string filtru = "";

        //        string nume = pag.Page.ToString().Replace("ASP.", "").Replace("_aspx", "").Replace("_", ".");

        //        if (nume.ToLower() == "sablon")
        //        {
        //            nume = "tbl." + HttpContext.Current.Session["Sablon_Tabela"];
        //            filtru = " AND \"IdColoana\"='-'";
        //        }

        //        //Florin 2018-05-18
        //        //S-a modificat din MIN in MAX
        //        string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
        //                        SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
        //                        FROM ""Securitate"" A
        //                        INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
        //                        WHERE B.""IdUser"" = @2 AND LOWER(A.""IdForm"") = @1 {0}
        //                        UNION
        //                        SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
        //                        FROM ""Securitate"" A
        //                        WHERE A.""IdGrup"" = -1 AND LOWER(A.""IdForm"") = @1 {0}) X
        //                        GROUP BY X.""IdControl"", X.""IdColoana""";
        //        strSql = string.Format(strSql, filtru);
        //        DataTable dt = General.IncarcaDT(strSql, new string[] { nume, HttpContext.Current.Session["UserId"].ToString() });

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            try
        //            {
        //                if (dr["IdColoana"].ToString() == "-")
        //                {
        //                    WebControl ctl = pag.FindControl(dr["IdControl"].ToString()) as WebControl;
        //                    if (ctl != null)
        //                    {
        //                        ctl.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                        ctl.Enabled = (Convert.ToInt32(dr["Blocat"]) == 1 ? false : true);
        //                    }
        //                    else
        //                    {                                      
        //                        //verificam daca nu cumva se gaseste intr-un container de tip ASPxCallbackPanel
        //                        foreach (ASPxCallbackPanel pnlCtl in pag.Controls.OfType<ASPxCallbackPanel>())
        //                        {
        //                            WebControl ctl2 = pnlCtl.FindControl(dr["IdControl"].ToString()) as WebControl;
        //                            if (ctl2 != null)
        //                            {
        //                                ctl2.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                                ctl2.Enabled = (Convert.ToInt32(dr["Blocat"]) == 1 ? false : true);
        //                            }
        //                            else
        //                            {
        //                                if (dr["IdControl"].ToString().Length >= 3 && dr["IdControl"].ToString().Substring(0, 3) == "lbl")
        //                                {
        //                                    HtmlGenericControl ctl1 = pnlCtl.FindControl(dr["IdControl"].ToString()) as HtmlGenericControl;
        //                                    if (ctl1 != null)
        //                                    {
        //                                        ctl1.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                                    }
        //                                }
        //                            }
        //                            //verificam daca nu cumva se gaseste intr-un container de tip ASPxRoundPanel
        //                            foreach (ASPxRoundPanel pnlCtl2 in pnlCtl.Controls.OfType<ASPxRoundPanel>())
        //                            {
        //                                WebControl ctl3 = pnlCtl2.FindControl(dr["IdControl"].ToString()) as WebControl;
        //                                if (ctl3 != null)
        //                                {
        //                                    ctl3.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                                    ctl3.Enabled = (Convert.ToInt32(dr["Blocat"]) == 1 ? false : true);
        //                                }
        //                                else
        //                                {
        //                                    if (dr["IdControl"].ToString().Length >= 3 && dr["IdControl"].ToString().Substring(0, 3) == "lbl")
        //                                    {
        //                                        HtmlGenericControl ctl1 = pnlCtl2.FindControl(dr["IdControl"].ToString()) as HtmlGenericControl;
        //                                        if (ctl1 != null)
        //                                        {
        //                                            ctl1.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                                        }
        //                                    }
        //                                }
        //                            }

        //                        }



        //                        //var wsx = pag.Controls.OfType<ASPxCallbackPanel>();
        //                        //var xcv = pag.Controls.OfType<ASPxCallbackPanel>().Where(p => p.ID.ToLower().Contains("pnlCtl"));                                
        //                    }
        //                }
        //                else
        //                {
        //                    ASPxGridView ctl = pag.FindControl(dr["IdControl"].ToString()) as ASPxGridView;
        //                    if (ctl != null)
        //                    { 
        //                        GridViewDataColumn col = ctl.Columns[dr["IdColoana"].ToString()] as GridViewDataColumn;
        //                        if (col != null)
        //                        {
        //                            col.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                            col.ReadOnly = (Convert.ToInt32(dr["Blocat"]) == 1 ? true : false);
        //                        }
        //                        else
        //                        {
        //                            //verificam daca sunt butoane in interiorul gridului
        //                            GridViewCommandColumn column = ctl.Columns["butoaneGrid"] as GridViewCommandColumn;
        //                            GridViewCommandColumnCustomButton button = column.CustomButtons[dr["IdColoana"].ToString()] as GridViewCommandColumnCustomButton;
        //                            if (button != null)
        //                            {
        //                                if (Convert.ToInt32(dr["Vizibil"]) == 1)
        //                                    button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
        //                                else
        //                                    button.Visibility = GridViewCustomButtonVisibility.Invisible;
        //                            }
        //                            else
        //                            {
        //                                //Florin 2018.08.16
        //                                //atunci este buton BuiltIn al Devexpress-ului
        //                                if (dr["IdColoana"].ToString().ToLower() == "btnedit")
        //                                {
        //                                    column.ShowEditButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                continue;
        //                General.MemoreazaEroarea(ex + Environment.NewLine + 
        //                    General.Nz(dr["IdControl"], "").ToString() + Environment.NewLine +
        //                    General.Nz(dr["IdColoana"], "").ToString(), "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}




        //Florin 2019.11.25
        //am modificat functia astfel incat daca labelul are acelasi nume ca si controlul aasociat sa-l ascunda automat; Ex: lblCateg - cmbCateg

        public static void Securitate(ContentPlaceHolder pag)
        {
            try
            {
                string filtru = "";

                string nume = pag.Page.ToString().Replace("ASP.", "").Replace("_aspx", "").Replace("_", ".");

                if (nume.ToLower() == "sablon")
                {
                    nume = "tbl." + HttpContext.Current.Session["Sablon_Tabela"];
                    filtru = " AND \"IdColoana\"='-'";
                }

                //Florin 2018-05-18
                //S-a modificat din MIN in MAX
                string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = @2 AND LOWER(A.""IdForm"") = @1 {0}
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND LOWER(A.""IdForm"") = @1 {0}) X
                                GROUP BY X.""IdControl"", X.""IdColoana""";
                strSql = string.Format(strSql, filtru);
                DataTable dt = General.IncarcaDT(strSql, new string[] { nume, HttpContext.Current.Session["UserId"].ToString() });

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        bool vizibil = Convert.ToBoolean(General.Nz(dr["Vizibil"],0));
                        bool blocat = Convert.ToBoolean(General.Nz(dr["Blocat"],0));
                        string idCtl = General.Nz(dr["IdControl"],"").ToString();
                        string idCol = General.Nz(dr["IdColoana"],"").ToString();

                        if (idCol == "-")
                        {
                            dynamic ctl = pag.FindControl(idCtl);
                            if (ctl != null)
                            {
                                //daca parintele este un div bootstrap, atunci ascundem div-ul cu tot cu controale
                                dynamic pnl = ctl.Parent;
                                if (pnl.GetType() == typeof(HtmlGenericControl) && pnl.Attributes["class"].IndexOf("col-") >= 0)
                                {
                                    pnl.Style["display"] = "none";
                                }
                                else
                                {
                                    ctl.Visible = vizibil;
                                    ctl.Enabled = !blocat;

                                    if (idCtl.Length > 3)
                                    {
                                        string idLbl = "lbl" + idCtl.Substring(3);
                                        dynamic lbl = pag.FindControl(idLbl);
                                        if (lbl != null) lbl.Visible = vizibil;
                                    }
                                }
                            }
                            else
                            {
                                //verificam daca nu cumva se gaseste intr-un container de tip ASPxCallbackPanel
                                foreach (ASPxCallbackPanel pnlCtl in pag.Controls.OfType<ASPxCallbackPanel>())
                                {
                                    dynamic ctl2 = pnlCtl.FindControl(idCtl);
                                    if (ctl2 != null)
                                    {
                                        //daca parintele este un div bootstrap, atunci ascundem div-ul cu tot cu controale
                                        dynamic pnl = ctl2.Parent;
                                        if (pnl.GetType() == typeof(HtmlGenericControl) && pnl.Attributes["class"].IndexOf("col-") >= 0)
                                        {
                                            pnl.Style["display"] = "none";
                                        }
                                        else
                                        {
                                            ctl2.Visible = vizibil;
                                            ctl2.Enabled = !blocat;

                                            if (idCtl.Length > 3)
                                            {
                                                string idLbl = "lbl" + idCtl.Substring(3);
                                                dynamic lbl = pnlCtl.FindControl(idLbl);
                                                if (lbl != null) lbl.Visible = vizibil;
                                            }
                                        }

                                        break;
                                    }

                                    //verificam daca nu cumva se gaseste intr-un container de tip ASPxRoundPanel
                                    foreach (ASPxRoundPanel pnlCtl2 in pnlCtl.Controls.OfType<ASPxRoundPanel>())
                                    {
                                        dynamic ctl3 = pnlCtl2.FindControl(idCtl);
                                        if (ctl3 != null)
                                        {
                                            //daca parintele este un div bootstrap, atunci ascundem div-ul cu tot cu controale
                                            dynamic pnl = ctl3.Parent;
                                            if (pnl.GetType() == typeof(HtmlGenericControl) && pnl.Attributes["class"].IndexOf("col-") >= 0)
                                            {
                                                pnl.Style["display"] = "none";
                                            }
                                            else
                                            {
                                                ctl3.Visible = vizibil;
                                                ctl3.Enabled = !blocat;

                                                if (idCtl.Length > 3)
                                                {
                                                    string idLbl = "lbl" + idCtl.Substring(3);
                                                    dynamic lbl = pnlCtl2.FindControl(idLbl);
                                                    if (lbl != null) lbl.Visible = vizibil;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }                            
                            }
                        }
                        else
                        {
                            ASPxGridView ctl = pag.FindControl(idCtl) as ASPxGridView;
                            if (ctl != null)
                            {
                                GridViewDataColumn col = ctl.Columns[idCol] as GridViewDataColumn;
                                if (col != null)
                                {
                                    col.Visible = vizibil;
                                    col.ReadOnly = blocat;
                                }
                                else
                                {
                                    //verificam daca sunt butoane in interiorul gridului
                                    GridViewCommandColumn column = ctl.Columns["butoaneGrid"] as GridViewCommandColumn;
                                    GridViewCommandColumnCustomButton button = column.CustomButtons[idCol] as GridViewCommandColumnCustomButton;
                                    if (button != null)
                                    {
                                        if (vizibil)
                                            button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                                        else
                                            button.Visibility = GridViewCustomButtonVisibility.Invisible;
                                    }
                                    else
                                    {
                                        //Florin 2018.08.16
                                        //atunci este buton BuiltIn al Devexpress-ului
                                        if (idCol.ToLower() == "btnedit")
                                        {
                                            column.ShowEditButton = vizibil;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //verificam daca nu cumva se gaseste intr-un container de tip ASPxCallbackPanel
                                foreach (ASPxCallbackPanel pnlCtl in pag.Controls.OfType<ASPxCallbackPanel>())
                                {
                                    dynamic ctl4 = pnlCtl.FindControl(idCtl);
                                    if (ctl4 != null)
                                    {
                                        if (idCol == "-")
                                        {
                                            dynamic ctl5 = pag.FindControl(idCtl);
                                            if (ctl5 != null)
                                            {
                                                ctl5.Visible = vizibil;
                                                ctl5.Enabled = !blocat;

                                                if (idCtl.Length > 3)
                                                {
                                                    string idLbl = "lbl" + idCtl.Substring(3);
                                                    dynamic lbl = pag.FindControl(idLbl);
                                                    if (lbl != null) lbl.Visible = vizibil;
                                                }
                                            }                                      
                                        }
                                        else
                                        {
                                            ASPxGridView ctl6 = pnlCtl.FindControl(idCtl) as ASPxGridView;
                                            if (ctl6 != null)
                                            {
                                                GridViewDataColumn col = ctl6.Columns[idCol] as GridViewDataColumn;
                                                if (col != null)
                                                {
                                                    col.Visible = vizibil;
                                                    col.ReadOnly = blocat;
                                                }
                                                else
                                                {
                                                    //verificam daca sunt butoane in interiorul gridului
                                                    GridViewCommandColumn column = ctl6.Columns["butoaneGrid"] as GridViewCommandColumn;
                                                    GridViewCommandColumnCustomButton button = column.CustomButtons[idCol] as GridViewCommandColumnCustomButton;
                                                    if (button != null)
                                                    {
                                                        if (vizibil)
                                                            button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                                                        else
                                                            button.Visibility = GridViewCustomButtonVisibility.Invisible;
                                                    }
                                                    else
                                                    {
                                                        //Florin 2018.08.16
                                                        //atunci este buton BuiltIn al Devexpress-ului
                                                        if (idCol.ToLower() == "btnedit")
                                                        {
                                                            column.ShowEditButton = vizibil;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string www = ex.Message;
                        continue;
                        //General.MemoreazaEroarea(ex + Environment.NewLine +
                        //    General.Nz(dr["IdControl"], "").ToString() + Environment.NewLine +
                        //    General.Nz(dr["IdColoana"], "").ToString(), "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        public static void Securitate(Page pag)
        {
            try
            {
                string filtru = "";

                string nume = pag.Page.ToString().Replace("ASP.", "").Replace("_aspx", "").Replace("_", ".");

                if (nume.ToLower() == "sablon")
                {
                    nume = "tbl." + HttpContext.Current.Session["Sablon_Tabela"];
                    filtru = " AND \"IdColoana\"='-'";
                }

                string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = @2 AND A.""IdForm"" = @1 {0}
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = @1 {0}) X
                                GROUP BY X.""IdControl"", X.""IdColoana""";
                strSql = string.Format(strSql, filtru);
                DataTable dt = General.IncarcaDT(strSql, new string[] { nume, HttpContext.Current.Session["UserId"].ToString() });

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["IdColoana"].ToString() == "-")
                        {
                            WebControl ctl = pag.FindControl(dr["IdControl"].ToString()) as WebControl;
                            if (ctl != null)
                            {
                                ctl.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                ctl.Enabled = (Convert.ToInt32(dr["Blocat"]) == 1 ? false : true);
                            }
                        }
                        else
                        {
                            ASPxGridView ctl = pag.FindControl(dr["IdControl"].ToString()) as ASPxGridView;
                            if (ctl != null)
                            {
                                GridViewDataColumn col = ctl.Columns[dr["IdColoana"].ToString()] as GridViewDataColumn;
                                if (col != null)
                                {
                                    col.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                    col.ReadOnly = (Convert.ToInt32(dr["Blocat"]) == 1 ? true : false);
                                }
                                else
                                {
                                    //verificam daca sunt butoane in interiorul gridului
                                    GridViewCommandColumn column = ctl.Columns["butoaneGrid"] as GridViewCommandColumn;
                                    GridViewCommandColumnCustomButton button = column.CustomButtons[dr["IdColoana"].ToString()] as GridViewCommandColumnCustomButton;
                                    if (button != null)
                                    {
                                        if (Convert.ToInt32(dr["Vizibil"]) == 1)
                                            button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                                        else
                                            button.Visibility = GridViewCustomButtonVisibility.Invisible;
                                    }
                                    else
                                    {
                                        //Florin 2018.08.16
                                        //atunci este buton BuiltIn al Devexpress-ului
                                        if (dr["IdColoana"].ToString().ToLower() == "btnedit")
                                        {
                                            column.ShowEditButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //General.MemoreazaEroarea(ex + Environment.NewLine +
                        //    General.Nz(dr["IdControl"], "").ToString() + Environment.NewLine +
                        //    General.Nz(dr["IdColoana"], "").ToString(), "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public static void Securitate(ASPxGridView grDate)
        {
            try
            {
                string filtru = "";

                Page pag = grDate.Page;
                string nume = pag.Page.ToString().Replace("ASP.", "").Replace("_aspx", "").Replace("_", ".");

                //Radu 20.11.2019
                if (nume.ToLower() == "personal.dateangajat")
                {
                    Control parent = grDate.Parent;
                    nume = parent.TemplateControl.ToString().Replace("ASP.", "").Replace("_ascx", "").Replace("_", ".");
                }

                if (nume.ToLower() == "sablon")
                {
                    nume = "tbl." + HttpContext.Current.Session["Sablon_Tabela"];
                    filtru = " AND \"IdColoana\"='-'";
                }

                string strSql = @"SELECT X.""IdControl"", X.""IdColoana"", MAX(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                WHERE B.""IdUser"" = @2 AND A.""IdForm"" = @1 {0}
                                UNION
                                SELECT A.""IdControl"", A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                FROM ""Securitate"" A
                                WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = @1 {0}) X
                                GROUP BY X.""IdControl"", X.""IdColoana""";
                strSql = string.Format(strSql, filtru);
                DataTable dt = General.IncarcaDT(strSql, new string[] { nume, HttpContext.Current.Session["UserId"].ToString() });

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        if (dr["IdColoana"].ToString() != "-")
                        {
                            //ASPxGridView ctl = pag.FindControl(dr["IdControl"].ToString()) as ASPxGridView;
                            ASPxGridView ctl = grDate;
                            if (ctl != null)
                            {
                                GridViewDataColumn col = ctl.Columns[dr["IdColoana"].ToString()] as GridViewDataColumn;
                                if (col != null)
                                {
                                    col.Visible = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                    col.ReadOnly = (Convert.ToInt32(dr["Blocat"]) == 1 ? true : false);
                                }
                                else
                                {
                                    //verificam daca sunt butoane in interiorul gridului
                                    GridViewCommandColumn column = ctl.Columns["butoaneGrid"] as GridViewCommandColumn;                                
                                    GridViewCommandColumnCustomButton button = column.CustomButtons[dr["IdColoana"].ToString()] as GridViewCommandColumnCustomButton;
                                    if (button != null)
                                    {
                                        if (Convert.ToInt32(dr["Vizibil"]) == 1)
                                            button.Visibility = GridViewCustomButtonVisibility.AllDataRows;
                                        else
                                            button.Visibility = GridViewCustomButtonVisibility.Invisible;
                                    }
                                    else
                                    {
                                        //Florin 2018.08.16
                                        //atunci este buton BuiltIn al Devexpress-ului
                                        if (dr["IdColoana"].ToString().ToLower() == "btnedit")
                                        {
                                            column.ShowEditButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                        }
                                        if (dr["IdColoana"].ToString().ToLower() == "btnsterge")
                                        {
                                            column.ShowDeleteButton = (Convert.ToInt32(dr["Vizibil"]) == 1 ? true : false);
                                        }
                                    }
                                    
                            
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                        //General.MemoreazaEroarea(ex + Environment.NewLine +
                        //    General.Nz(dr["IdControl"], "").ToString() + Environment.NewLine +
                        //    General.Nz(dr["IdColoana"], "").ToString(), "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public static void AccesApp()
        {
            try
            {
                if (Constante.esteTactil)
                {
                    if (HttpContext.Current.Session["SecApp"].ToString() != "OK_Tactil")
                        HttpContext.Current.Response.Redirect("~/Default.aspx");
                }
                else
                {
                    if (HttpContext.Current.Session["SecApp"].ToString() != "OK")
                        HttpContext.Current.Response.Redirect("~/Default.aspx");
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void AccesTactil()
        {
            try
            {
                if (HttpContext.Current.Session["SecApp"].ToString() != "OK_Tactil")
                    HttpContext.Current.Response.Redirect("~/DefaultTactil.aspx");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static void AccesAdmin()
        {
            try
            {
                if (General.VarSession("EsteAdmin").ToString() == "0")
                    HttpContext.Current.Response.Redirect("Default.aspx");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public static string SelectPontaj()
        {
            string strSql = "";

            try
            {
                if (Constante.tipBD == 1)
                    strSql = "SELECT 1 AS X UNION SELECT 2 UNION SELECT 3";
                else
                    strSql = "SELECT 1 AS X FROM DUAL UNION SELECT 2 FROM DUAL UNION SELECT 3 FROM DUAL";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        //public static string SelectCereri(int totiAngajatii = 0)
        //{
        //    string sqlFinal = "";

        //    try
        //    {
        //        // 78  -  Inlocuitor/Delegat
        //        // 77  -  Drepturi depline
        //        // 76  -  Fara supervizor (cazul cand pe circuit, in loc de id supervizor, se pune codul de user (F70102)


        //        string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
        //        string selectInloc = "-99";
        //        if (Dami.ValoareParam("InlocuitorulVedeCererile", "0") == "1")
        //        {
        //            selectInloc = $@"SELECT Y.F70102 FROM ""Ptj_Cereri"" X
        //                        INNER JOIN USERS Y ON X.F10003=Y.F10003
        //                        WHERE X.""IdStare""=3 AND X.""Inlocuitor"" = {(HttpContext.Current.Session["User_Marca"])}
        //                        AND CONVERT(date,X.""DataInceput"") <= CONVERT(date,GetDate()) AND CONVERT(date,GetDate()) <= CONVERT(date,X.""DataSfarsit"")";
        //            if (Constante.tipBD == 2)
        //                selectInloc = $@"SELECT Y.F70102 FROM ""Ptj_Cereri"" X
        //                        INNER JOIN USERS Y ON X.F10003=Y.F10003
        //                        WHERE X.""IdStare""=3 AND X.""Inlocuitor"" = {(HttpContext.Current.Session["User_Marca"])}
        //                        AND TRUNC(X.""DataInceput"") <= TRUNC(SYSDATE) AND TRUNC(SYSDATE) <= TRUNC(X.""DataSfarsit"")";
        //        }

        //        //select 1 - toate cererile mele ca angajat
        //        //select 2 - toate cererile care nu sunt ale mele dar pt care sunt pe circuit
        //        //select 3 - toate cererile pt care sunt inlocuitor sau delegat

        //        //OBSOLETE
        //        //select 4 - taote cererile pt care am drepturi depline indiferent daca sunt sau nu pe circuit; id-urile care au drepturi depline sunt specificate in parametrii

        //        //Radu 16.02.2018
        //        string idStare = "(1, 2, 3, 4)";
        //        string condSuplim = "";
        //        if (totiAngajatii == 1)
        //        {
        //            idStare = "(1, 2, 3)";
        //            condSuplim = $@" AND A.""IdStare"" IN (1, 2, 3) ";
        //        }
        //        //Radu 10.10.2019 - am scos conditia {General.TruncateDate("A.DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("A.DataSfarsit")} si am inlocuit cu 1 = 1
        //        string strSql = $@"SELECT A.*, 0 AS ""Rol"",
        //                        CASE WHEN (A.""IdStare"" IN (-1, 0, 3) OR B.""IdCerere"" IS NULL) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
        //                        FROM ""Ptj_Cereri"" A
        //                        LEFT JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]} AND B.""IdStare"" IN {idStare}
        //                        WHERE A.F10003 = {HttpContext.Current.Session["User_Marca"]}
        //                        UNION
        //                        SELECT A.*, CASE WHEN C.""Id"" IS NOT NULL THEN C.""Id"" ELSE (CASE WHEN B.""IdSuper"" > 0 THEN 76 ELSE C.""Id"" END) END AS ""Rol"",
        //                        CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN (A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
        //                        FROM ""Ptj_Cereri"" A
        //                        INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdStare"" <> -1 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}
        //                        LEFT JOIN ""tblSupervizori"" C ON (-1 * B.""IdSuper"")= C.""Id""     --AND COALESCE(C.""ModululCereriAbsente"", 0) = 1
        //                        WHERE A.F10003 <> {HttpContext.Current.Session["User_Marca"]} {condSuplim}
        //                        UNION
        //                        SELECT A.*, 78 AS ""Rol"",
        //                        CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
        //                        FROM ""Ptj_Cereri"" A
        //                        INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""IdStare"" <> -1 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
        //                        (
        //                        {selectInloc}
        //                        )
        //                        WHERE 1 = 1  {condSuplim}
        //                        UNION
        //                        SELECT A.*, 78 AS Rol,
        //                        CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
        //                        FROM ""Ptj_Cereri"" A
        //                        INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""IdStare"" <> -1 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
        //                        (SELECT ""IdUser"" FROM ""tblDelegari"" WHERE COALESCE(""IdModul"",-99)=1 AND ""IdDelegat""={HttpContext.Current.Session["UserId"]} AND ""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= ""DataSfarsit"") {condSuplim}";

        //        if (totiAngajatii == 3)
        //            strSql = $@"SELECT DISTINCT A.*, 77 AS ""Rol"", CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE 1 END AS ""Actiune""
        //                       FROM ""Ptj_Cereri"" A
        //                       INNER JOIN ""F100Supervizori"" B ON A.F10003 = B.F10003 AND B.""IdSuper"" IN ({idHR}) AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}";

        //        //Florin 2019.09.25 - optimizare
        //        //Anulare_Valoare, Anulare_NrZile se transforma din subselecturi in LEFT JOIN
        //        //s-a modificat filtrarea dupa sirul vid pt oracle

        //        string campIntervalOrar = $@"
        //                        CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
        //                        SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")) + ':' +
        //                        SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")) + ' - ' +
        //                        SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")) + ':' +
        //                        SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")) + '; ' ELSE '' END";
        //        if (Constante.tipBD == 2)
        //            campIntervalOrar = $@"
        //                        CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
        //                        TO_CHAR(""OraInceput"", 'HH24') || ':' || TO_CHAR(""OraInceput"", 'MM') || ' - ' || TO_CHAR(""OraSfarsit"", 'HH24') || ':' || TO_CHAR(""OraSfarsit"", 'MM') || '; ' ELSE '' END";

        //        #region OLD

        //        //sqlFinal = @"SELECT A.""Id"", B.F10003, B.F10008 {1} ' ' {1} B.F10009 AS ""NumeAngajat"", A.""IdAbsenta"", A.""DataInceput"", A.""DataSfarsit"", B.F100901 AS EID,
        //        //                CASE WHEN E.""Alias"" IS NULL OR E.""Alias""='' THEN E.""Denumire"" ELSE E.""Alias"" END AS ""RolDenumire"",
        //        //                A.""Rol"", A.""Actiune"", A.""Inlocuitor"", COALESCE(C.""AdaugaAtasament"",0) AS ""AdaugaAtasament"",
        //        //                CASE WHEN C.""IdTipOre"" = 1 THEN A.""NrZile"" ELSE null END AS ""NrZile"", 
        //        //                CASE WHEN C.""IdTipOre"" = 0 THEN A.""NrOre"" ELSE NULL END AS ""NrOre"", 
        //        //                A.""Observatii"", D.F10008 {1} ' ' {1} D.F10009 AS ""NumeInlocuitor"", A.""IdStare"", 
        //        //                CASE WHEN A.""TrimiteLa"" = -13 THEN 'Banca' ELSE CASE WHEN A.""TrimiteLa""= -14 THEN 'Plata' ELSE Q.""Denumire"" END END AS ""TrimiteLa"", 
        //        //                A.""Comentarii"", C.""Compensare"", C.""CompensareBanca"", C.""CompensarePlata"",
        //        //                M.""Denumire"" AS ""CompensareBancaDenumire"", N.""Denumire"" AS ""CompensarePlataDenumire"",
        //        //                CASE WHEN ""CampExtra1"" IS NOT NULL AND ""CampExtra1"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=1) + '=' + ""CampExtra1"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra2"" IS NOT NULL AND ""CampExtra2"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=2) + '=' + ""CampExtra2"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra3"" IS NOT NULL AND ""CampExtra3"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=3) + '=' + ""CampExtra3"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra4"" IS NOT NULL AND ""CampExtra4"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=4) + '=' + ""CampExtra4"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra5"" IS NOT NULL AND ""CampExtra5"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=5) + '=' + ""CampExtra5"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra6"" IS NOT NULL AND ""CampExtra6"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=6) + '=' + ""CampExtra6"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra7"" IS NOT NULL AND ""CampExtra7"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=7) + '=' + ""CampExtra7"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra8"" IS NOT NULL AND ""CampExtra8"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=8) + '=' + ""CampExtra8"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra9"" IS NOT NULL AND ""CampExtra9"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=9) + '=' + ""CampExtra9"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra10"" IS NOT NULL AND ""CampExtra10"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=10) + '=' + ""CampExtra10"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra11"" IS NOT NULL AND ""CampExtra11"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=11) + '=' + ""CampExtra11"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra12"" IS NOT NULL AND ""CampExtra12"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=12) + '=' + ""CampExtra12"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra13"" IS NOT NULL AND ""CampExtra13"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=13) + '=' + ""CampExtra13"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra14"" IS NOT NULL AND ""CampExtra14"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=14) + '=' + ""CampExtra14"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra15"" IS NOT NULL AND ""CampExtra15"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=15) + '=' + ""CampExtra15"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra16"" IS NOT NULL AND ""CampExtra16"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=16) + '=' + ""CampExtra16"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra17"" IS NOT NULL AND ""CampExtra17"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=17) + '=' + ""CampExtra17"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra18"" IS NOT NULL AND ""CampExtra18"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=18) + '=' + ""CampExtra18"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra19"" IS NOT NULL AND ""CampExtra19"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=19) + '=' + ""CampExtra19"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""CampExtra20"" IS NOT NULL AND ""CampExtra20"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=20) + '=' + ""CampExtra20"" + '; ' ELSE '' END +
        //        //                CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
        //        //                SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")) + ':' +
        //        //                SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")) + ' - ' +
        //        //                SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")) + ':' +
        //        //                SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")) + '; ' ELSE '' END                                 
        //        //                AS ""DateConcatenate"",
        //        //                (SELECT TOP 1 Valoare FROM Ptj_CereriDrepturi DR WHERE (DR.IdAbs=A.IdAbsenta OR DR.IdAbs = -13) AND (DR.IdStare=A.IdStare OR DR.IdStare = -13) AND (DR.IdRol=A.Rol OR DR.IdRol = -13) AND (DR.IdActiune=3 OR DR.IdActiune = -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Anulare_Valoare,
        //        //                (SELECT TOP 1 NrZile FROM Ptj_CereriDrepturi DR WHERE (DR.IdAbs=A.IdAbsenta OR DR.IdAbs = -13) AND (DR.IdStare=A.IdStare OR DR.IdStare = -13) AND (DR.IdRol=A.Rol OR DR.IdRol = -13) AND (DR.IdActiune=3 OR DR.IdActiune = -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Anulare_NrZile,
        //        //                COALESCE(A.""CampBifa"",0) AS ""CampBifa""
        //        //                FROM ({0}) A
        //        //                INNER JOIN F100 B ON A.F10003 = B.F10003
        //        //                INNER JOIN ""Ptj_tblAbsente"" C ON A.""IdAbsenta"" = C.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" M ON C.""CompensareBanca"" = M.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" N ON C.""CompensarePlata"" = N.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" Q ON A.""TrimiteLa"" = Q.""Id""
        //        //                LEFT JOIN F100 D ON A.""Inlocuitor"" = D.F10003
        //        //                LEFT JOIN ""tblSupervizori"" E ON A.""Rol"" = E.""Id""
        //        //                WHERE 1=1 ";

        //        //if (Constante.tipBD == 2)
        //        //{
        //        //    sqlFinal = @"SELECT A.""Id"", B.F10003, B.F10008 {1} ' ' {1} B.F10009 AS ""NumeAngajat"", A.""IdAbsenta"", A.""DataInceput"", A.""DataSfarsit"",  B.F100901 AS EID,
        //        //                CASE WHEN E.""Alias"" IS NULL OR E.""Alias""='' THEN E.""Denumire"" ELSE E.""Alias"" END AS ""RolDenumire"",
        //        //                A.""Rol"", A.""Actiune"", A.""Inlocuitor"", COALESCE(C.""AdaugaAtasament"",0) AS ""AdaugaAtasament"",
        //        //                CASE WHEN C.""IdTipOre"" = 1 THEN A.""NrZile"" ELSE null END AS ""NrZile"", 
        //        //                CASE WHEN C.""IdTipOre"" = 0 THEN A.""NrOre"" ELSE NULL END AS ""NrOre"", 
        //        //                A.""Observatii"", D.F10008 {1} ' ' {1} D.F10009 AS ""NumeInlocuitor"", A.""IdStare"", 
        //        //                CASE WHEN A.""TrimiteLa"" = -13 THEN 'Banca' ELSE CASE WHEN A.""TrimiteLa""= -14 THEN 'Plata' ELSE Q.""Denumire"" END END AS ""TrimiteLa"", 
        //        //                A.""Comentarii"", C.""Compensare"", C.""CompensareBanca"", C.""CompensarePlata"",
        //        //                M.""Denumire"" AS ""CompensareBancaDenumire"", N.""Denumire"" AS ""CompensarePlataDenumire"",
        //        //                CASE WHEN ""CampExtra1"" IS NOT NULL AND ""CampExtra1"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=1) {1} '=' {1} ""CampExtra1"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra2"" IS NOT NULL AND ""CampExtra2"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=2) {1} '=' {1} ""CampExtra2"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra3"" IS NOT NULL AND ""CampExtra3"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=3) {1} '=' {1} ""CampExtra3"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra4"" IS NOT NULL AND ""CampExtra4"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=4) {1} '=' {1} ""CampExtra4"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra5"" IS NOT NULL AND ""CampExtra5"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=5) {1} '=' {1} ""CampExtra5"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra6"" IS NOT NULL AND ""CampExtra6"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=6) {1} '=' {1} ""CampExtra6"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra7"" IS NOT NULL AND ""CampExtra7"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=7) {1} '=' {1} ""CampExtra7"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra8"" IS NOT NULL AND ""CampExtra8"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=8) {1} '=' {1} ""CampExtra8"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra9"" IS NOT NULL AND ""CampExtra9"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=9) {1} '=' {1} ""CampExtra9"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra10"" IS NOT NULL AND ""CampExtra10"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=10) {1} '=' {1} ""CampExtra10"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra11"" IS NOT NULL AND ""CampExtra11"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=11) {1} '=' {1} ""CampExtra11"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra12"" IS NOT NULL AND ""CampExtra12"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=12) {1} '=' {1} ""CampExtra12"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra13"" IS NOT NULL AND ""CampExtra13"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=13) {1} '=' {1} ""CampExtra13"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra14"" IS NOT NULL AND ""CampExtra14"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=14) {1} '=' {1} ""CampExtra14"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra15"" IS NOT NULL AND ""CampExtra15"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=15) {1} '=' {1} ""CampExtra15"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra16"" IS NOT NULL AND ""CampExtra16"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=16) {1} '=' {1} ""CampExtra16"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra17"" IS NOT NULL AND ""CampExtra17"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=17) {1} '=' {1} ""CampExtra17"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra18"" IS NOT NULL AND ""CampExtra18"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=18) {1} '=' {1} ""CampExtra18"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra19"" IS NOT NULL AND ""CampExtra19"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=19) {1} '=' {1} ""CampExtra19"" {1} '; ' ELSE '' END {1}
        //        //                CASE WHEN ""CampExtra20"" IS NOT NULL AND ""CampExtra20"" <> '' THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=20) {1} '=' {1} ""CampExtra20"" {1} '; ' ELSE '' END {1} 
        //        //                CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
        //        //                TO_CHAR(""OraInceput"", 'HH24') || ':' || TO_CHAR(""OraInceput"", 'MM') || ' - ' || TO_CHAR(""OraSfarsit"", 'HH24') || ':' || TO_CHAR(""OraSfarsit"", 'MM') || '; ' ELSE '' END
        //        //                AS ""DateConcatenate"",
        //        //                V.""Valoare""  AS Anulare_Valoare,
        //        //                Z.""NrZile""  AS Anulare_NrZile,
        //        //                COALESCE(A.""CampBifa"",0) AS ""CampBifa""
        //        //                FROM ({0}) A
        //        //                INNER JOIN F100 B ON A.F10003 = B.F10003
        //        //                INNER JOIN ""Ptj_tblAbsente"" C ON A.""IdAbsenta"" = C.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" M ON C.""CompensareBanca"" = M.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" N ON C.""CompensarePlata"" = N.""Id""
        //        //                LEFT JOIN ""Ptj_tblAbsente"" Q ON A.""TrimiteLa"" = Q.""Id""
        //        //                LEFT JOIN F100 D ON A.""Inlocuitor"" = D.F10003
        //        //                LEFT JOIN ""tblSupervizori"" E ON A.""Rol"" = E.""Id""
        //        //                left join  (SELECT * FROM ""Ptj_CereriDrepturi"" DR WHERE  ROWNUM = 1 ORDER BY DR.""IdAbs"" DESC, DR.""IdRol"" DESC, DR.""IdStare"" DESC ) V on (V.""IdAbs"" = A.""IdAbsenta"" OR V.""IdAbs"" = -13) AND (V.""IdStare"" = A.""IdStare"" OR V.""IdStare"" = -13) AND (V.""IdRol"" = A.""Rol"" OR V.""IdRol"" = -13) AND (V.""IdActiune"" = 3 OR V.""IdActiune"" = -13)  
        //        //                left join  (SELECT * FROM ""Ptj_CereriDrepturi"" DR WHERE  ROWNUM = 1 ORDER BY DR.""IdAbs"" DESC, DR.""IdRol"" DESC, DR.""IdStare"" DESC ) Z on (Z.""IdAbs"" = A.""IdAbsenta"" OR Z.""IdAbs"" = -13) AND (Z.""IdStare"" = A.""IdStare"" OR Z.""IdStare"" = -13) AND (Z.""IdRol"" = A.""Rol"" OR Z.""IdRol"" = -13) AND (Z.""IdActiune"" = 3 OR Z.""IdActiune"" = -13)
        //        //                WHERE 1 =1 ";
        //        //}								

        //        #endregion

        //        sqlFinal = $@"SELECT A.""Id"", B.F10003, B.F10008 {Dami.Operator()} ' ' {Dami.Operator()} B.F10009 AS ""NumeAngajat"", A.""IdAbsenta"", A.""DataInceput"", A.""DataSfarsit"", B.F100901 AS EID,
        //                        CASE WHEN E.""Alias"" IS NULL OR E.""Alias""='' THEN E.""Denumire"" ELSE E.""Alias"" END AS ""RolDenumire"",
        //                        A.""Rol"", A.""Actiune"", A.""Inlocuitor"", COALESCE(C.""AdaugaAtasament"",0) AS ""AdaugaAtasament"",
        //                        CASE WHEN C.""IdTipOre"" = 1 THEN A.""NrZile"" ELSE null END AS ""NrZile"", 
        //                        CASE WHEN C.""IdTipOre"" = 0 THEN A.""NrOre"" ELSE NULL END AS ""NrOre"", 
        //                        A.""Observatii"", D.F10008 {Dami.Operator()} ' ' {Dami.Operator()} D.F10009 AS ""NumeInlocuitor"", A.""IdStare"", 
        //                        CASE WHEN A.""TrimiteLa"" = -13 THEN 'Banca' ELSE CASE WHEN A.""TrimiteLa""= -14 THEN 'Plata' ELSE Q.""Denumire"" END END AS ""TrimiteLa"", 
        //                        A.""Comentarii"", C.""Compensare"", C.""CompensareBanca"", C.""CompensarePlata"",
        //                        M.""Denumire"" AS ""CompensareBancaDenumire"", N.""Denumire"" AS ""CompensarePlataDenumire"",
        //                        CASE WHEN ""CampExtra1"" IS NOT NULL {General.FiltrulCuNull("CampExtra1")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=1) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra1"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra2"" IS NOT NULL {General.FiltrulCuNull("CampExtra2")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=2) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra2"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra3"" IS NOT NULL {General.FiltrulCuNull("CampExtra3")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=3) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra3"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra4"" IS NOT NULL {General.FiltrulCuNull("CampExtra4")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=4) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra4"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra5"" IS NOT NULL {General.FiltrulCuNull("CampExtra5")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=5) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra5"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra6"" IS NOT NULL {General.FiltrulCuNull("CampExtra6")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=6) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra6"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra7"" IS NOT NULL {General.FiltrulCuNull("CampExtra7")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=7) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra7"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra8"" IS NOT NULL {General.FiltrulCuNull("CampExtra8")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=8) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra8"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra9"" IS NOT NULL {General.FiltrulCuNull("CampExtra9")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=9) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra9"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra10"" IS NOT NULL {General.FiltrulCuNull("CampExtra10")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=10) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra10"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra11"" IS NOT NULL {General.FiltrulCuNull("CampExtra11")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=11) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra11"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra12"" IS NOT NULL {General.FiltrulCuNull("CampExtra12")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=12) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra12"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra13"" IS NOT NULL {General.FiltrulCuNull("CampExtra13")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=13) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra13"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra14"" IS NOT NULL {General.FiltrulCuNull("CampExtra14")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=14) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra14"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra15"" IS NOT NULL {General.FiltrulCuNull("CampExtra15")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=15) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra15"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra16"" IS NOT NULL {General.FiltrulCuNull("CampExtra16")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=16) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra16"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra17"" IS NOT NULL {General.FiltrulCuNull("CampExtra17")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=17) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra17"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra18"" IS NOT NULL {General.FiltrulCuNull("CampExtra18")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=18) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra18"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra19"" IS NOT NULL {General.FiltrulCuNull("CampExtra19")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=19) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra19"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
        //                        CASE WHEN ""CampExtra20"" IS NOT NULL {General.FiltrulCuNull("CampExtra20")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=20) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra20"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()} 
        //                        {campIntervalOrar}
        //                        AS ""DateConcatenate"", DR.""Valoare"" AS ""Anulare_Valoare"", DR.""NrZile"" AS ""Anulare_NrZile"", COALESCE(A.""CampBifa"",0) AS ""CampBifa""
        //                        FROM ({strSql}) A
        //                        INNER JOIN F100 B ON A.F10003 = B.F10003
        //                        INNER JOIN ""Ptj_tblAbsente"" C ON A.""IdAbsenta"" = C.""Id""
        //                        LEFT JOIN ""Ptj_tblAbsente"" M ON C.""CompensareBanca"" = M.""Id""
        //                        LEFT JOIN ""Ptj_tblAbsente"" N ON C.""CompensarePlata"" = N.""Id""
        //                        LEFT JOIN ""Ptj_tblAbsente"" Q ON A.""TrimiteLa"" = Q.""Id""
        //                        LEFT JOIN F100 D ON A.""Inlocuitor"" = D.F10003
        //                        LEFT JOIN ""tblSupervizori"" E ON A.""Rol"" = E.""Id""
        //                        LEFT JOIN (SELECT W.*, ROW_NUMBER() OVER(partition by W.""IdAbs"", W.""IdRol"", W.""IdStare"", W.""IdActiune"" ORDER BY W.""IdAbs"" DESC, W.""IdRol"" DESC, W.""IdStare"" DESC, W.""IdActiune"" DESC) ""IdRow"" FROM ""Ptj_CereriDrepturi"" W) DR ON (DR.""IdAbs"" = A.""IdAbsenta"" OR DR.""IdAbs"" = -13) AND (DR.""IdStare"" = A.""IdStare"" OR DR.""IdStare"" = -13) AND (DR.""IdRol"" = A.""Rol"" OR DR.""IdRol"" = -13) AND (DR.""IdActiune"" = 3 OR DR.""IdActiune"" = -13) AND DR.""IdRow"" <= 1
        //                        WHERE 1=1 ";
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return sqlFinal;
        //}

        public static string SelectCereri(int totiAngajatii = 0)
        {
            string sqlFinal = "";

            try
            {
                // 78  -  Inlocuitor/Delegat
                // 77  -  Drepturi depline
                // 76  -  Fara supervizor (cazul cand pe circuit, in loc de id supervizor, se pune codul de user (F70102)


                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string selectInloc = "-99";
                if (Dami.ValoareParam("InlocuitorulVedeCererile", "0") == "1")
                {
                    selectInloc = $@"SELECT Y.F70102 FROM ""Ptj_Cereri"" X
                                INNER JOIN USERS Y ON X.F10003=Y.F10003
                                WHERE X.""IdStare""=3 AND X.""Inlocuitor"" = {(HttpContext.Current.Session["User_Marca"])}
                                AND CONVERT(date,X.""DataInceput"") <= CONVERT(date,GetDate()) AND CONVERT(date,GetDate()) <= CONVERT(date,X.""DataSfarsit"")";
                    if (Constante.tipBD == 2)
                        selectInloc = $@"SELECT Y.F70102 FROM ""Ptj_Cereri"" X
                                INNER JOIN USERS Y ON X.F10003=Y.F10003
                                WHERE X.""IdStare""=3 AND X.""Inlocuitor"" = {(HttpContext.Current.Session["User_Marca"])}
                                AND TRUNC(X.""DataInceput"") <= TRUNC(SYSDATE) AND TRUNC(SYSDATE) <= TRUNC(X.""DataSfarsit"")";
                }

                //select 1 - toate cererile mele ca angajat
                //select 2 - toate cererile care nu sunt ale mele dar pt care sunt pe circuit
                //select 3 - toate cererile pt care sunt inlocuitor sau delegat

                //OBSOLETE
                //select 4 - taote cererile pt care am drepturi depline indiferent daca sunt sau nu pe circuit; id-urile care au drepturi depline sunt specificate in parametrii

                //Radu 16.02.2018
                string idStare = "(1, 2, 3, 4)";
                string condSuplim = "";
                if (totiAngajatii == 1)
                {
                    idStare = "(1, 2, 3)";
                    condSuplim = $@" AND A.""IdStare"" IN (1, 2, 3) ";
                }
                //Radu 10.10.2019 - am scos conditia {General.TruncateDate("A.DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("A.DataSfarsit")} si am inlocuit cu 1 = 1
                string strSql = $@"SELECT A.*, 0 AS ""Rol"",
                                CASE WHEN (A.""IdStare"" IN (-1, 0, 3) OR B.""IdCerere"" IS NULL) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""Ptj_Cereri"" A
                                LEFT JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]} AND B.""IdStare"" IN {idStare}
                                WHERE A.F10003 = {HttpContext.Current.Session["User_Marca"]}
                                UNION
                                SELECT A.*, CASE WHEN C.""Id"" IS NOT NULL THEN C.""Id"" ELSE (CASE WHEN B.""IdSuper"" > 0 THEN 76 ELSE C.""Id"" END) END AS ""Rol"",
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN (A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdStare"" <> -1 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}
                                LEFT JOIN ""tblSupervizori"" C ON (-1 * B.""IdSuper"")= C.""Id""     --AND COALESCE(C.""ModululCereriAbsente"", 0) = 1
                                WHERE A.F10003 <> {HttpContext.Current.Session["User_Marca"]} {condSuplim}
                                UNION
                                SELECT A.*, 78 AS ""Rol"",
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""IdStare"" <> -1 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
                                (
                                {selectInloc}
                                )
                                WHERE 1 = 1  {condSuplim}
                                UNION
                                SELECT A.*, 78 AS Rol,
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""Ptj_Cereri"" A
                                INNER JOIN ""Ptj_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""IdStare"" <> -1 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
                                (SELECT ""IdUser"" FROM ""tblDelegari"" WHERE COALESCE(""IdModul"",-99)=1 AND ""IdDelegat""={HttpContext.Current.Session["UserId"]} AND ""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= ""DataSfarsit"") {condSuplim}";

                if (totiAngajatii == 3)
                    strSql = $@"SELECT DISTINCT A.*, 77 AS ""Rol"", CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE 1 END AS ""Actiune""
                               FROM ""Ptj_Cereri"" A
                               INNER JOIN ""F100Supervizori"" B ON A.F10003 = B.F10003 AND B.""IdSuper"" IN ({idHR}) AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}";

                //Florin 2019.09.25 - optimizare
                //Anulare_Valoare, Anulare_NrZile se transforma din subselecturi in LEFT JOIN
                //s-a modificat filtrarea dupa sirul vid pt oracle

                //Florin 2019.11.08
                //am anulat LEFT JOIN si am revenit la subselecturi


                string campIntervalOrar = $@"
                    CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
                    SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraInceput"")) + ':' +
                    SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraInceput"")) + ' - ' +
                    SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(HOUR, ""OraSfarsit"")) + ':' +
                    SUBSTRING('00', 1, 2 - LEN(CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")))) + CONVERT(nvarchar(2), DATEPART(MINUTE, ""OraSfarsit"")) + '; ' ELSE '' END";
                string strDrepturi = $@"
                    (SELECT TOP 1 Valoare FROM Ptj_CereriDrepturi DR WHERE DR.IdAbs IN (A.IdAbsenta,-13) AND DR.IdStare IN (A.IdStare, -13) AND DR.IdRol IN (A.Rol, -13) AND DR.IdActiune IN (3, -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Anulare_Valoare,
                    (SELECT TOP 1 NrZile  FROM Ptj_CereriDrepturi DR WHERE DR.IdAbs IN (A.IdAbsenta,-13) AND DR.IdStare IN (A.IdStare, -13) AND DR.IdRol IN (A.Rol, -13) AND DR.IdActiune IN (3, -13) ORDER BY DR.IdAbs DESC, DR.IdRol DESC, DR.IdStare DESC) AS Anulare_NrZile";

                if (Constante.tipBD == 2)
                {
                    campIntervalOrar = $@"
                                CASE WHEN ""OraInceput"" IS NOT NULL AND ""OraSfarsit"" IS NOT NULL THEN 
                                TO_CHAR(""OraInceput"", 'HH24') || ':' || TO_CHAR(""OraInceput"", 'MM') || ' - ' || TO_CHAR(""OraSfarsit"", 'HH24') || ':' || TO_CHAR(""OraSfarsit"", 'MM') || '; ' ELSE '' END";
                    strDrepturi = $@"
                        (SELECT ""Valoare"" FROM ""Ptj_CereriDrepturi"" DR WHERE DR.""IdAbs"" IN (A.""IdAbsenta"",-13) AND DR.""IdStare"" IN (A.""IdStare"", -13) AND DR.""IdRol"" IN (A.""Rol"", -13) AND DR.""IdActiune"" IN (3, -13) AND ROWNUM <=1) AS ""Anulare_Valoare"",
                        (SELECT ""NrZile""  FROM ""Ptj_CereriDrepturi"" DR WHERE DR.""IdAbs"" IN (A.""IdAbsenta"",-13) AND DR.""IdStare"" IN (A.""IdStare"", -13) AND DR.""IdRol"" IN (A.""Rol"", -13) AND DR.""IdActiune"" IN (3, -13) AND ROWNUM <=1) AS ""Anulare_NrZile"" ";
                }

                sqlFinal = $@"SELECT A.""Id"", B.F10003, B.F10008 {Dami.Operator()} ' ' {Dami.Operator()} B.F10009 AS ""NumeAngajat"", A.""IdAbsenta"", A.""DataInceput"", A.""DataSfarsit"", B.F100901 AS EID,
                                CASE WHEN E.""Alias"" IS NULL OR E.""Alias""='' THEN E.""Denumire"" ELSE E.""Alias"" END AS ""RolDenumire"",
                                A.""Rol"", A.""Actiune"", A.""Inlocuitor"", COALESCE(C.""AdaugaAtasament"",0) AS ""AdaugaAtasament"",
                                CASE WHEN C.""IdTipOre"" = 1 THEN A.""NrZile"" ELSE null END AS ""NrZile"", 
                                CASE WHEN C.""IdTipOre"" = 0 THEN A.""NrOre"" ELSE NULL END AS ""NrOre"", 
                                A.""Observatii"", D.F10008 {Dami.Operator()} ' ' {Dami.Operator()} D.F10009 AS ""NumeInlocuitor"", A.""IdStare"", 
                                CASE WHEN A.""TrimiteLa"" = -13 THEN 'Banca' ELSE CASE WHEN A.""TrimiteLa""= -14 THEN 'Plata' ELSE Q.""Denumire"" END END AS ""TrimiteLa"", 
                                A.""Comentarii"", C.""Compensare"", C.""CompensareBanca"", C.""CompensarePlata"",
                                M.""Denumire"" AS ""CompensareBancaDenumire"", N.""Denumire"" AS ""CompensarePlataDenumire"",
                                CASE WHEN ""CampExtra1"" IS NOT NULL {General.FiltrulCuNull("CampExtra1")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=1) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra1"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra2"" IS NOT NULL {General.FiltrulCuNull("CampExtra2")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=2) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra2"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra3"" IS NOT NULL {General.FiltrulCuNull("CampExtra3")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=3) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra3"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra4"" IS NOT NULL {General.FiltrulCuNull("CampExtra4")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=4) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra4"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra5"" IS NOT NULL {General.FiltrulCuNull("CampExtra5")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=5) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra5"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra6"" IS NOT NULL {General.FiltrulCuNull("CampExtra6")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=6) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra6"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra7"" IS NOT NULL {General.FiltrulCuNull("CampExtra7")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=7) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra7"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra8"" IS NOT NULL {General.FiltrulCuNull("CampExtra8")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=8) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra8"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra9"" IS NOT NULL {General.FiltrulCuNull("CampExtra9")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=9) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra9"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra10"" IS NOT NULL {General.FiltrulCuNull("CampExtra10")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=10) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra10"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra11"" IS NOT NULL {General.FiltrulCuNull("CampExtra11")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=11) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra11"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra12"" IS NOT NULL {General.FiltrulCuNull("CampExtra12")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=12) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra12"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra13"" IS NOT NULL {General.FiltrulCuNull("CampExtra13")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=13) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra13"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra14"" IS NOT NULL {General.FiltrulCuNull("CampExtra14")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=14) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra14"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra15"" IS NOT NULL {General.FiltrulCuNull("CampExtra15")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=15) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra15"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra16"" IS NOT NULL {General.FiltrulCuNull("CampExtra16")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=16) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra16"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra17"" IS NOT NULL {General.FiltrulCuNull("CampExtra17")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=17) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra17"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra18"" IS NOT NULL {General.FiltrulCuNull("CampExtra18")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=18) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra18"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra19"" IS NOT NULL {General.FiltrulCuNull("CampExtra19")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=19) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra19"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()}
                                CASE WHEN ""CampExtra20"" IS NOT NULL {General.FiltrulCuNull("CampExtra20")} THEN (SELECT ""Denumire"" FROM ""Ptj_tblAbsenteConfig"" WHERE ""IdAbsenta""=1 AND ""IdCampExtra""=20) {Dami.Operator()} '=' {Dami.Operator()} ""CampExtra20"" {Dami.Operator()} '; ' ELSE '' END {Dami.Operator()} 
                                {campIntervalOrar}
                                AS ""DateConcatenate"", COALESCE(A.""CampBifa"",0) AS ""CampBifa"", {strDrepturi}
                                FROM ({strSql}) A
                                INNER JOIN F100 B ON A.F10003 = B.F10003
                                INNER JOIN ""Ptj_tblAbsente"" C ON A.""IdAbsenta"" = C.""Id""
                                LEFT JOIN ""Ptj_tblAbsente"" M ON C.""CompensareBanca"" = M.""Id""
                                LEFT JOIN ""Ptj_tblAbsente"" N ON C.""CompensarePlata"" = N.""Id""
                                LEFT JOIN ""Ptj_tblAbsente"" Q ON A.""TrimiteLa"" = Q.""Id""
                                LEFT JOIN F100 D ON A.""Inlocuitor"" = D.F10003
                                LEFT JOIN ""tblSupervizori"" E ON A.""Rol"" = E.""Id""
                                WHERE 1=1 ";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlFinal;
        }



        public static string SelectCereriDiverse(int totiAngajatii = 0)
        {
            string sqlFinal = "";

            try
            {
                // 78  -  Inlocuitor/Delegat
                // 77  -  Drepturi depline
                // 76  -  Fara supervizor (cazul cand pe circuit, in loc de id supervizor, se pune codul de user (F70102)


                string op = "+";
                if (Constante.tipBD == 2)
                    op = "||";				
                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string selectInloc = "-99";
                if (Dami.ValoareParam("InlocuitorulVedeCererile", "0") == "1")
                {
                    selectInloc = $@"SELECT Y.F70102 FROM ""Ptj_Cereri"" X
                                INNER JOIN USERS Y ON X.F10003=Y.F10003
                                WHERE X.""IdStare""=3 AND X.""Inlocuitor"" = {(HttpContext.Current.Session["User_Marca"])}
                                AND CONVERT(date,X.""DataInceput"") <= {General.CurrentDate()} AND {General.CurrentDate()} <= CONVERT(date,X.""DataSfarsit"")";
                }

                //select 1 - toate cererile mele ca angajat
                //select 2 - toate cererile care nu sunt ale mele dar pt care sunt pe circuit
                //select 3 - toate cererile pt care sunt inlocuitor sau delegat

                string idStare = "(1, 2, 3, 4)";
                string condSuplim = "";
                if (totiAngajatii == 1)
                {
                    idStare = "(1, 2, 3)";
                    condSuplim = $@" AND A.""IdStare"" IN (1, 2, 3) ";
                }

                string strSql = $@"SELECT A.*, 0 AS ""Rol"",
                                CASE WHEN (A.""IdStare"" IN (-1, 0, 3) OR B.""IdCerere"" IS NULL) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""MP_Cereri"" A
                                LEFT JOIN ""MP_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]} AND B.""IdStare"" IN {idStare}
                                WHERE A.F10003 = {HttpContext.Current.Session["User_Marca"]}
                                UNION
                                SELECT A.*, CASE WHEN C.""Id"" IS NOT NULL THEN C.""Id"" ELSE (CASE WHEN B.""IdSuper"" > 0 THEN 76 ELSE C.""Id"" END) END AS ""Rol"",
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN (A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""MP_Cereri"" A
                                INNER JOIN ""MP_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""Pozitie"" <> 0 AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}
                                LEFT JOIN ""tblSupervizori"" C ON (-1 * B.""IdSuper"")= C.""Id""
                                WHERE A.F10003 <> {HttpContext.Current.Session["User_Marca"]} {condSuplim}
                                UNION
                                SELECT A.*, 78 AS ""Rol"",
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""MP_Cereri"" A
                                INNER JOIN ""MP_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
                                (
                                {selectInloc}
                                )
                                WHERE 1=1 {condSuplim}
                                UNION
                                SELECT A.*, 78 AS ""Rol"",
                                CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE (CASE WHEN(A.""Pozitie"" + 1) = B.""Pozitie"" THEN 1 ELSE 0 END) END AS ""Actiune""
                                FROM ""MP_Cereri"" A
                                INNER JOIN ""MP_CereriIstoric"" B ON A.""Id"" = B.""IdCerere"" AND B.""IdSuper"" <> 0 AND B.""Pozitie"" <> 0 AND B.""IdUser"" IN 
                                (SELECT ""IdUser"" FROM ""tblDelegari"" WHERE COALESCE(""IdModul"",-99)=1 AND ""IdDelegat""={HttpContext.Current.Session["UserId"]} AND ""DataInceput"" <= {General.CurrentDate()} AND {General.CurrentDate()} <= ""DataSfarsit"") {condSuplim}";

                if (totiAngajatii == 3)
                    strSql = $@"SELECT A.*, 77 AS ""Rol"", CASE WHEN A.""IdStare"" IN (-1, 0, 3) THEN 0 ELSE 1 END AS ""Actiune""
                               FROM ""MP_Cereri"" A
                               INNER JOIN ""F100Supervizori"" B ON A.F10003 = B.F10003 AND B.""IdSuper"" IN ({idHR}) AND B.""IdUser"" = {HttpContext.Current.Session["UserId"]}";


                sqlFinal = @"SELECT A.""Id"", B.F10003, B.F10008 {1} ' ' {1} B.F10009 AS ""NumeAngajat"", A.""IdTipCerere"",
                                CASE WHEN E.""Alias"" IS NULL OR E.""Alias""='' THEN E.""Denumire"" ELSE E.""Alias"" END AS ""RolDenumire"",
                                A.""Rol"", A.""Actiune"", A.""Inlocuitor"",
                                D.F10008 {1} ' ' {1} D.F10009 AS ""NumeInlocuitor"", A.""IdStare"", 
                                C.""Denumire"" AS ""TipCerere"", A.""Descriere"", A.""Raspuns"", A.""Culoare""
                                FROM ({0}) A
                                INNER JOIN F100 B ON A.F10003 = B.F10003
                                INNER JOIN ""MP_tblTipCerere"" C ON A.""IdTipCerere"" = C.""Id""
                                LEFT JOIN F100 D ON A.""Inlocuitor"" = D.F10003
                                LEFT JOIN ""tblSupervizori"" E ON A.""Rol"" = E.""Id""
                                WHERE 1=1 ";


                sqlFinal = string.Format(sqlFinal, strSql, op);

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return sqlFinal;
        }



        public static string SelectEvaluare()
        {
            string strSql = "";

            try
            {
                strSql = Evaluare.GetEvalLista(Convert.ToInt32(HttpContext.Current.Session["UserId"].ToString()), -99, -99, new DateTime(1900, 1, 1), new DateTime(1900, 1, 1), -99, -99, 0);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static string SelectDecont()
        {
            string strSql = "";

            try
            {
                if (Constante.tipBD == 1)
                    strSql = "SELECT 1 AS X UNION SELECT 2 UNION SELECT 3";
                else
                    strSql = "SELECT 1 AS X FROM DUAL UNION SELECT 2 FROM DUAL UNION SELECT 3 FROM DUAL";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }
        public static string SelectCurs()
        {
            string strSql = "";

            try
            {
                if (Constante.tipBD == 1)
                    strSql = "SELECT 1 AS X UNION SELECT 2 UNION SELECT 3";
                else
                    strSql = "SELECT 1 AS X FROM DUAL UNION SELECT 2 FROM DUAL UNION SELECT 3 FROM DUAL";

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static string SelectAngajati()
        {
            string strSql = "";

            try
            {
                strSql = $@"SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS NumeComplet, G.F00406 AS Filiala, H.F00507 AS Sectie, I.F00608 AS Departament
                        FROM F100 A
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        public static string NumeLuna(int NrLuna, int tip = 0, string limba = "RO")
        {
            string luna = "Ianuarie";

            try
            {
                if (limba == "EN")
                {
                    switch (NrLuna)
                    {
                        case 1:
                            luna = "January";
                            break;
                        case 2:
                            luna = "February";
                            break;
                        case 3:
                            luna = "March";
                            break;
                        case 4:
                            luna = "April";
                            break;
                        case 5:
                            luna = "May";
                            break;
                        case 6:
                            luna = "June";
                            break;
                        case 7:
                            luna = "July";
                            break;
                        case 8:
                            luna = "August";
                            break;
                        case 9:
                            luna = "September";
                            break;
                        case 10:
                            luna = "October";
                            break;
                        case 11:
                            luna = "November";
                            break;
                        case 12:
                            luna = "December";
                            break;
                        default:
                            luna = "January";
                            break;
                    }
                }

                if (limba == "RO")
                {
                    switch (NrLuna)
                    {
                        case 1:
                            luna = "Ianuarie";
                            break;
                        case 2:
                            luna = "Februarie";
                            break;
                        case 3:
                            luna = "Martie";
                            break;
                        case 4:
                            luna = "Aprilie";
                            break;
                        case 5:
                            luna = "Mai";
                            break;
                        case 6:
                            luna = "Iunie";
                            break;
                        case 7:
                            luna = "Iulie";
                            break;
                        case 8:
                            luna = "August";
                            break;
                        case 9:
                            luna = "Septembrie";
                            break;
                        case 10:
                            luna = "Octombrie";
                            break;
                        case 11:
                            luna = "Noiembrie";
                            break;
                        case 12:
                            luna = "Decembrie";
                            break;
                        default:
                            luna = "Ianuarie";
                            break;
                    }
                }

                switch (tip)
                {
                    case 0:
                        //NOP
                        break;
                    case 1:
                        luna = luna.Substring(0, 3);
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return luna;
        }

        public static int ZiSapt(string nume)
        {
            try
            {
                int zi = 1;
                if (nume != null)
                {
                    switch (nume.ToLower())
                    {
                        case "monday":
                            zi = 1;
                            break;
                        case "tuesday":
                            zi = 2;
                            break;
                        case "wednesday":
                            zi = 3;
                            break;
                        case "thursday":
                            zi = 4;
                            break;
                        case "friday":
                            zi = 5;
                            break;
                        case "saturday":
                            zi = 6;
                            break;
                        case "sunday":
                            zi = 7;
                            break;
                    }
                }

                return zi;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return 1;
            }
        }


        public static string NumeZi(int nrZi, int tip = 1, string limba = "RO")
        {//Radu 27.11.2019 - am adaugat limba engleza
            try
            {
                string zi = "Luni";

                if (limba == "EN")
                {
                    switch (nrZi)
                    {
                        case 1:
                            zi = "Monday";
                            break;
                        case 2:
                            zi = "Tuesday";
                            break;
                        case 3:
                            zi = "Wednesday";
                            break;
                        case 4:
                            zi = "Thursday";
                            break;
                        case 5:
                            zi = "Friday";
                            break;
                        case 6:
                            zi = "Saturday";
                            break;
                        case 7:
                            zi = "Sunday";
                            break;
                        default:
                            zi = "Monday";
                            break;
                    }
                    if (tip == 2)
                        if (nrZi == 2 || nrZi == 4 || nrZi == 6 || nrZi == 7)
                            zi = zi.Substring(0, 2);
                        else
                            zi = zi.Substring(0, 1);
                }

                if (limba == "RO")
                {
                    switch (nrZi)
                    {
                        case 1:
                            zi = "Luni";
                            break;
                        case 2:
                            zi = "Marti";
                            break;
                        case 3:
                            zi = "Miercuri";
                            break;
                        case 4:
                            zi = "Joi";
                            break;
                        case 5:
                            zi = "Vineri";
                            break;
                        case 6:
                            zi = "Sambata";
                            break;
                        case 7:
                            zi = "Duminica";
                            break;
                        default:
                            zi = "Luni";
                            break;
                    }
                    if (tip == 2)
                        if (nrZi == 2 || nrZi == 3)
                            zi = zi.Substring(0, 2);
                        else
                            zi = zi.Substring(0, 1);
                }  

                return TraduCuvant(zi);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return "Ianuarie";
            }
        }

        public static string NumeZi(int an, int luna, int ziua, int tip)
        {
            try
            {
                // tip = 0 descrierea lunga
                // tip = 1 descriere scurta
                // tip = 2 prima litera

                string zi = "00".Substring(0, 2 - ziua.ToString().Length) + ziua.ToString() + "/" + luna.ToString() + "/" + an.ToString();

                if (General.IsDate(zi))
                {
                    string nume = Convert.ToDateTime(zi).DayOfWeek.ToString();
                    string numeRO = "";

                    switch (nume.ToLower())
                    {
                        case "monday":
                            numeRO = "Luni";
                            break;
                        case "tuesday":
                            numeRO = "Marti";
                            break;
                        case "wednesday":
                            numeRO = "Miercuri";
                            break;
                        case "thursday":
                            numeRO = "Joi";
                            break;
                        case "friday":
                            numeRO = "Vineri";
                            break;
                        case "saturday":
                            numeRO = "Sambata";
                            break;
                        case "sunday":
                            numeRO = "Duminica";
                            break;
                    }

                    switch (tip)
                    {
                        case 1:
                            return TraduCuvant(numeRO);
                        case 2:
                            return TraduCuvant(numeRO.Substring(0, 3));
                        case 3:
                            if (numeRO == "Marti" || numeRO == "Miercuri")
                            {
                                return TraduCuvant(numeRO.Substring(0, 2));
                            }
                            else
                            {
                                return TraduCuvant(numeRO.Substring(0, 1));
                            }
                    }

                    return numeRO;
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return "";
            }
        }



        //public static string PaginaWeb()
        //{
        //    string pag = HttpContext.Current.Session["PaginaWeb"].ToString();

        //    try
        //    {
        //        if (HttpContext.Current.Session["PaginaWeb"].ToString().ToLower().IndexOf("sablon") >= 0) pag = "tbl." + HttpContext.Current.Session["Sablon_Tabela"].ToString();

        //        //string url = HttpContext.Current.Request.Url.LocalPath.Substring(1).Replace("/",".");
        //        //if (url.ToLower().IndexOf("sablon") >= 0) pag = "tbl." + HttpContext.Current.Session["Sablon_Tabela"].ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return pag;
        //}

        public static string Profil(int id = -99)
        {
            string rez = "";

            try
            {
                DataTable dtProfile = new DataTable();

                if (id != -99)
                {
                    dtProfile = General.IncarcaDT(@"SELECT * FROM ""tblProfile"" WHERE ""Id""=@1", new object[] { id });
                }
                else
                {
                    string strSql = @"SELECT TOP 1 * FROM ""tblProfile"" WHERE ""Pagina""=@1 AND ""Activ""=1 AND ""Implicit""=1 ";
                    if (Constante.tipBD == 2) strSql = @"SELECT * FROM ""tblProfile"" WHERE ""Pagina""=@1 AND ""Activ""=1 AND ""Implicit""=1"" AND ROWNUM<=1 ";
                    //dtProfile = General.IncarcaDT(strSql, new object[] { Dami.PaginaWeb() });
                    dtProfile = General.IncarcaDT(strSql, new object[] { General.Nz(HttpContext.Current.Session["PaginaWeb"], "").ToString().Replace("\\", ".") });
                }

                if (dtProfile.Rows.Count != 0 && (dtProfile.Rows[0]["Continut"] ?? "").ToString() != "" ) rez = dtProfile.Rows[0]["Continut"].ToString();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static string ValoareParam(string strParam, string replaceValue = "")
        {
            string rez = "";

            try
            {
                DataTable dt = new DataTable();
                DataRow dr = null;

                if (HttpContext.Current != null && HttpContext.Current.Session["tblParam"] != null)
                {
                    dt = HttpContext.Current.Session["tblParam"] as DataTable;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] arr = dt.Select("Nume='" + strParam + "'");
                        if (arr.Count() == 0)
                            return replaceValue;
                        else
                            dr = arr[0];
                    }
                    else
                    {
                        dt = General.IncarcaDT(@"SELECT ""Valoare"", ""Criptat"" FROM ""tblParametrii"" WHERE ""Nume""=@1", new object[] { strParam });
                        if (dt.Rows.Count == 0)
                            return replaceValue;
                        else
                            dr = dt.Rows[0];
                    }
                }
                else
                {
                    dt = General.IncarcaDT(@"SELECT ""Valoare"", ""Criptat"" FROM ""tblParametrii"" WHERE ""Nume""=@1", new object[] { strParam });
                    if (dt.Rows.Count == 0)
                        return replaceValue;
                    else
                        dr = dt.Rows[0];
                }

                //DataRow dr = dt.Rows[0];

                if (dr["Criptat"].ToString() != "" && Convert.ToInt32(dr["Criptat"] ?? 0) == 1)
                {
                    CriptDecript prc = new CriptDecript();
                    rez = prc.EncryptString("WizOne2016", (dr["Valoare"] ?? "").ToString(), 2);
                }
                else
                    rez = (dr["Valoare"] ?? "").ToString();

                if (rez.Trim() == "" && replaceValue != "") rez = replaceValue;


            //if (dt != null)
            //{
            //    DataRow[] dr = dt.Select("Nume='" + strParam + "'");
            //    if (dr.Count() > 0)
            //    {
            //        if (dr[0]["Criptat"].ToString() != "" && Convert.ToInt32(dr[0]["Criptat"] ?? 0) == 1)
            //        {
            //            CriptDecript prc = new CriptDecript();
            //            rez = prc.EncryptString("WizOne2016", (dr[0]["Valoare"] ?? "").ToString(), 2);
            //        }
            //        else
            //            rez = (dr[0]["Valoare"] ?? "").ToString();
            //    }

            //    if (rez.Trim() == "" && replaceValue != "") rez = replaceValue;
            //}

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }

        public static int NextId(string tabela, int nrInreg = 1)
        {
            int id = 1;

            try
            {
                string seq = tabela + "_SEQ";

                //daca tabela se gaseste in switch-ul de mai jos se ia secventa indicata acolo, daca nu se ia cea implicita de deasupra de forma Tabela + _SEQ
                //exista cazul in care pt aceeasi tabela exista mai multe secvente
                switch (tabela.ToUpper())
                {
                    case "PTJ_CERERI":
                        seq = "Ptj_Cereri_Id_SEQ";
                        break;
                    case "ORGANIGRAMA":
                        seq = "Organigrama_Id_SEQ";
                        break;
                    case "F100":
                        seq = "F100MARCA";
                        break;
                    case "MP_CERERI":
                        seq = "MP_Cereri_SEQ";
                        break;
                    case "F300":
                        seq = "F300_SEQ";
                        break;
                    case "BP_PRIME":
                        seq = "BP_Prime_SEQ";
                        break;
                    case "OBIINDIVIDUALE":
                        seq = "ObiIndividuale_SEQ";
                        break;
                }

                ///LeonardM 16.10.2017
                ///in momentul in care avem de a face cu tabele ce tin de modulul de Evaluare
                ///atunci intentia e ca pentru coloanele de id sa preluam valoarea cu ajutorul procedurii GetNextId
                ///
                if (tabela.Contains("Eval"))
                {
                    #region varianta prin procedura GetNextId
                    string sqlQuery = string.Empty;
                    if (Constante.tipBD == 1)           //SQL
                    {
                        sqlQuery = "exec \"GetNextId\" '{0}', {1}";
                        sqlQuery = string.Format(sqlQuery, tabela, nrInreg);
                        General.ExecutaNonQuery(sqlQuery, null);
                    }
                    else
                    {
                        //Oracle
                        sqlQuery = "exec \"GetNextId\" ('{0}', {1})";
                        sqlQuery = string.Format(sqlQuery, tabela, nrInreg);
                        General.ExecutaNonQueryOracle("\"GetNextId\"", new object[] { "tableName=" + tabela, "nrInreg=" + nrInreg });
                    }

                    sqlQuery = "select \"NextId\" from \"TableSYSInfo_NextId\" where \"TableName\" ='{0}'";
                    sqlQuery = string.Format(sqlQuery, tabela);
                    id = Convert.ToInt32(General.ExecutaScalar(sqlQuery, null));
                    #endregion
                }
                else
                {
                    #region varianta standard/ secvente
                    if (Constante.tipBD == 1)                   //SQL
                    {
                        int vers = Convert.ToInt32(ValoareParam("VersiuneSQL", "2008"));

                        if (vers > 2008)
                        {
                            //Radu 09.01.2018
                            //int cnt = Convert.ToInt32(General.ExecutaScalar(@"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[@1]') AND type = 'SO'", new object[] { seq }));
                            int cnt = Convert.ToInt32(General.ExecutaScalar("SELECT COUNT(*) FROM sys.objects WHERE name = '" + seq + "' AND type = 'SO'", null));
                            if (cnt == 0)
                            {
                                //id = Convert.ToInt32(General.ExecutaScalar("SELECT MAX(COALESCE(Id,0)) FROM " + tabela, null)) + 1;
                                id = -99;
                            }
                            else
                                //Radu 16.04.2018
                                //id = Convert.ToInt32(General.ExecutaScalar($@"SELECT NEXT VALUE FOR @1", new object[] { seq }));
                                id = Convert.ToInt32(General.ExecutaScalar($@"SELECT NEXT VALUE FOR " + seq, null));
                        }
                        else
                        {
                            id = Convert.ToInt32(General.ExecutaScalar($@"SELECT ""IdAutoCereri"" FROM ""tblConfig"" WHERE ""Id"" = 1", null)) + 1;
                            General.ExecutaNonQuery($@"UPDATE ""tblConfig"" SET ""IdAutoCereri"" = {id} WHERE ""Id"" = 1", null);
                        }
                    }
                    else                                   //Oracle
                    {
                        id = Convert.ToInt32(General.ExecutaScalar($@"SELECT ""{seq}"".NEXTVAL FROM DUAL", null));
                    }
                    #endregion
                }
            }
            catch (Exception)
            {
                // srvGeneral.MemoreazaEroarea(ex.ToString(), "srvGeneral", new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return id;
        }

        public static DateTime DataDrepturi(int tip, int nrZile, DateTime dtInc, int f10003=-99, int idRol = 0)
        {
            DateTime zi = new DateTime(2111,11,11);

            try
            {
                switch (tip)
                {
                    case 1:                     //zi curenta
                        zi = DateTime.Now;
                        break;
                    case 2:                     //prima zi din saptamana
                        {
                            int delta = DayOfWeek.Monday - DateTime.Now.DayOfWeek;
                            zi = DateTime.Now.AddDays(delta);
                        }
                        break;
                    case 3:                     //ultima zi din saptamana
                        {
                            int delta = 7 - Convert.ToInt32(DateTime.Now.DayOfWeek);
                            zi = DateTime.Now.AddDays(delta);
                        }
                        break;
                    case 4:                     //prima zi din luna curenta
                        zi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        break;
                    case 5:                     //ultima zi din luna curenta
                        zi = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                        break;
                    case 6:                     //prima zi din luna de lucru
                        {
                            int an = Convert.ToInt32(Dami.ValoareParam("AnLucru", DateTime.Now.Year.ToString()));
                            int luna = Convert.ToInt32(Dami.ValoareParam("LunaLucru", DateTime.Now.Month.ToString()));
                            zi = new DateTime(an, luna, 1);
                        }
                        break;
                    case 7:                     //ultima zi din luna de lucru
                        {
                            int an = Convert.ToInt32(Dami.ValoareParam("AnLucru", DateTime.Now.Year.ToString()));
                            int luna = Convert.ToInt32(Dami.ValoareParam("LunaLucru", DateTime.Now.Month.ToString()));
                            zi = new DateTime(an, luna, DateTime.DaysInMonth(an, luna));
                        }
                        break;
                    case 8:                     //prima zi din an
                        zi = new DateTime(DateTime.Now.Year, 1, 1);
                        break;
                    case 9:                     //ultima zi din an
                        zi = new DateTime(DateTime.Now.Year, 12, 31);
                        break;
                    case 13:                    //pontaj in curs de aprobare
                        {
                            int idStare = 1;
                            DataRow drCum = General.IncarcaDR(@"SELECT * FROM ""Ptj_Cumulat"" WHERE F10003=@1 AND ""An""=@2 AND ""Luna""=@3", new object[] { f10003, dtInc.Year, dtInc.Month });
                            if (drCum != null && drCum["IdStare"] != null) idStare = Convert.ToInt32(General.Nz(drCum["IdStare"], 1));

                            DataTable dt = General.IncarcaDT(
                                $@"SELECT COALESCE(C.""IdRol"",0) AS ""IdRol""
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                WHERE C.""IdSuper""={HttpContext.Current.Session["UserId"]} AND COALESCE(C.""IdRol"",0) <= 3 AND B.F10003={f10003}
                                UNION
                                SELECT COALESCE(C.""IdRol"",0) AS ""IdRol""
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                                WHERE J.""IdUser""={HttpContext.Current.Session["UserId"]} AND COALESCE(C.""IdRol"",0) <= 3 AND B.F10003={f10003}
                                ORDER BY ""IdRol"" DESC", null);

                            if (dt != null && dt.Rows.Count > 0) idRol = Convert.ToInt32(General.Nz(dt.Rows[0]["IdRol"],0));

                            if ((idRol == 0 && (idStare == 1 || idStare == 4)) ||
                                (idRol == 1 && (idStare == 1 || idStare == 4)) ||
                                (idRol == 2 && (idStare == 1 || idStare == 2 || idStare == 4 || idStare == 6)) ||
                                (idRol == 3)
                                )
                            {
                                zi = new DateTime(1900, 1, 1);
                            }
                            else
                            {
                                zi = new DateTime(2222, 12, 13);
                            }


                            //if (drCum != null && (Convert.ToInt32(General.Nz(drCum["IdStare"], 1)) == 2 || Convert.ToInt32(General.Nz(drCum["IdStare"], 1)) == 3 || Convert.ToInt32(General.Nz(drCum["IdStare"], 1)) == 5 || Convert.ToInt32(General.Nz(drCum["IdStare"], 1)) == 7))
                            //    zi = new DateTime(2222, 12, 13);
                            //else
                            //    zi = new DateTime(1900, 1, 1);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return zi.AddDays(nrZile);
        }

        public static string Operator()
        {
            string op = "+";

            try
            {
                if (Constante.tipBD == 2) op = "||";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return op;
        }

        public static List<metaGeneral> ListaAni(int anStart = 1990, int anEnd = 2200)
        {
            try
            {
                List<metaGeneral> list = new List<metaGeneral>();
                for (int i = anStart; i <= anEnd; i++)
                {
                    list.Add(new metaGeneral() { Id = i, Denumire = i.ToString() });
                }

                return list;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static List<metaGeneral> ListaActivi()
        {
            try
            {
                List<metaGeneral> list = new List<metaGeneral>();
                list.Add(new metaGeneral() { Id = 1, Denumire = Dami.TraduCuvant("Toti") });
                list.Add(new metaGeneral() { Id = 2, Denumire = Dami.TraduCuvant("Activi") });
                list.Add(new metaGeneral() { Id = 3, Denumire = Dami.TraduCuvant("Plecati") });
                list.Add(new metaGeneral() { Id = 4, Denumire = Dami.TraduCuvant("In avans") });
                //5 - acest id este ocupat in srvPersonal
                list.Add(new metaGeneral() { Id = 6, Denumire = Dami.TraduCuvant("Candidat") });

                return list;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static List<metaGeneral2> ListaLimbi()
        {
            try
            {
                List<metaGeneral2> list = new List<metaGeneral2>();
                list.Add(new metaGeneral2() { Id = "RO", Denumire = "Română" });
                list.Add(new metaGeneral2() { Id = "EN", Denumire = "English" });
                list.Add(new metaGeneral2() { Id = "FR", Denumire = "Français" });
                list.Add(new metaGeneral2() { Id = "ES", Denumire = "Español" });
                list.Add(new metaGeneral2() { Id = "DE", Denumire = "Deutsch" });
                list.Add(new metaGeneral2() { Id = "IT", Denumire = "Italiano" });
                list.Add(new metaGeneral2() { Id = "BG", Denumire = "български" });
                list.Add(new metaGeneral2() { Id = "RU", Denumire = "русский" });

                return list;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }

        public static int Varsta(DateTime dataNasterii)
        {
            int rez = 0;

            try
            {
                rez = Convert.ToInt32((DateTime.Now - dataNasterii).TotalDays / 365);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return rez;
        }


    }
}