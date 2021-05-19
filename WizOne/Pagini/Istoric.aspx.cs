using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;

namespace WizOne.Pagini
{
    public partial class Istoric : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                DataTable dtStari = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"", ""Culoare"" FROM ""Ptj_tblStari"" ", null);
                GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                colStari.PropertiesComboBox.DataSource = dtStari;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();
                //Dami.AccesAdmin();

                if (!IsPostBack)
                {
                    int qwe = Convert.ToInt32(General.Nz(Request["qwe"],-99));
                    int tip = Convert.ToInt32(General.Nz(Request["tip"], -99));
                    IncarcaGrid(qwe, tip);
                }

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);

                        //var ert = c.GetType();
                        //if (c.GetType() == typeof(GridViewDataColumn))
                        //{
                        //    GridViewDataColumn col = c as GridViewDataColumn;
                        //    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                        //}
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid(int id, int tip = 1, int f10003 = -99, int an = -1, int luna = -1)
        {
            try
            {
                //tip = 1  din cererei
                //tip = 2  din pontaj
                //tip = 3 - Cereri diverse Radu 13.08.2018


                DataTable dt = new DataTable();

                string strSql = $@"SELECT A.""IdAuto"", A.""@camp"", A.""IdCircuit"", A.""IdSuper"", A.""IdStare"", A.""Culoare"", D.""Denumire"" AS ""NumeStare"",
                            COALESCE(A.""Pozitie"",0) AS ""Pozitie"", A.""DataAprobare"",
                            CASE WHEN COALESCE(J.""NumeComplet"",' ') <> ' ' THEN J.""NumeComplet"" ELSE (CASE WHEN COALESCE(K.F10008,' ') = ' ' THEN J.F70104 ELSE K.F10008 {Dami.Operator()} ' ' {Dami.Operator()} K.F10009 END) END AS ""Inlocuitor"",
                            CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {Dami.Operator()} ' ' {Dami.Operator()} C.F10009 END) END as ""Nume"",
                            '' AS Stare
                            FROM ""@tabela"" A
                            LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
                            LEFT JOIN F100 C ON B.F10003 = C.F10003
                            LEFT JOIN ""Ptj_tblStari"" D ON A.""IdStare""=D.""Id""
                            LEFT JOIN USERS J ON a.""IdUserInlocuitor"" = j.F70102
                            LEFT JOIN F100 K ON J.F10003 = K.F10003
                            WHERE A.""@camp"" = @1
                            ORDER BY A.""Pozitie"" ";


                switch (tip)
                {
                    case 1:
                        strSql = strSql.Replace("@tabela", "Ptj_CereriIstoric");
                        strSql = strSql.Replace("@camp", "IdCerere");
                        dt = General.IncarcaDT(strSql, new object[] { id });
                        break;
                    case 2:
                        //tabela = "Ptj_CereriIstoric";
                        strSql = $@"SELECT A.""IdAuto"", A.F10003, A.""An"", A.""Luna"", A.""IdSuper"", A.""IdStare"", D.""Culoare"", A.""DataAprobare"", D.""Denumire"" AS ""NumeStare"",
                        CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {Dami.Operator()} ' ' {Dami.Operator()} C.F10009 END) END as ""Nume"", 
                        '' AS Stare
                        FROM ""Ptj_CumulatIstoric"" A
                        LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
                        LEFT JOIN F100 C ON B.F10003 = C.F10003
                        LEFT JOIN ""Ptj_tblStariPontaj"" D ON A.""IdStare""=D.""Id""
                        WHERE A.""IdStare"" <>1 AND A.F10003 = @1 AND ""An""=@2 AND A.""Luna""=@3
                        ORDER BY A.""DataAprobare""";
                        dt = General.IncarcaDT(strSql, new object[] { f10003, an, luna });
                        break;
                    case 3:
                        strSql = strSql.Replace("@tabela", "MP_CereriIstoric");
                        strSql = strSql.Replace("@camp", "IdCerere");
                        dt = General.IncarcaDT(strSql, new object[] { id });
                        break;
                    case 4:
                        strSql = strSql.Replace("@tabela", "BP_Istoric");
                        strSql = strSql.Replace("@camp", "Id");
                        dt = General.IncarcaDT(strSql, new object[] { id });
                        break;
                    case 5:
                        strSql = strSql.Replace("@tabela", "Avs_CereriIstoric");
                        strSql = strSql.Replace("@camp", "Id");
                        dt = General.IncarcaDT(strSql, new object[] { id });
                        break;
                    case 6:             //Eval_RaspunsIstoric
                        strSql = $@"SELECT (A.""IdAuto"" * 10) AS ""IdCompus"", a.""IdAuto"", a.""IdQuiz"", a.F10003, a.""IdQuiz"" AS ""IdCerere"", (-1 * a.""IdSuper"") AS ""IdSuper"", a.""IdUser"",
                                CASE WHEN a.""IdSuper"" = 0 THEN 'Evaluare Angajat' ELSE CASE WHEN a.""IdSuper"" < 0 THEN 'Evaluare ' {Dami.Operator()} D.""Alias"" ELSE 'Evaluare ' {Dami.Operator()} B.F70104 END END AS ""Stare"",
                                COALESCE(a.""Aprobat"",0) AS ""Aprobat"",
                                a.""Culoare"", COALESCE(a.""Pozitie"",0) AS ""Pozitie"", a.""DataAprobare"",
                                CASE WHEN (C.F10008 IS NULL OR C.F10008 = '') THEN B.F70104 ELSE C.F10008 {Dami.Operator()} ' ' {Dami.Operator()} C.F10009 END AS ""Nume"",
                                A.""Pozitie"" AS ""IdStare"", '' AS ""Inlocuitor""
                                FROM ""Eval_Raspuns"" X
                                INNER JOIN ""Eval_RaspunsIstoric"" A ON A.""IdQuiz""= X.""IdQuiz"" AND A.F10003=X.F10003
                                LEFT JOIN USERS B ON A.""IdUser""=B.F70102
                                LEFT JOIN F100 C ON B.F10003=C.F10003
                                LEFT JOIN ""tblSupervizori"" D ON (-1 * a.""IdSuper"") = D.""Id""
                                LEFT JOIN ""Eval_Drepturi"" H ON A.""IdQuiz""=H.""IdQuiz"" AND A.""Pozitie""=H.""Pozitie"" AND H.""PozitieVizibila""=0
                                WHERE X.""IdAuto"" =@1 AND COALESCE(h.""PozitieVizibila"",1)=1 
                                ORDER BY A.""Pozitie"", (A.""IdAuto"" * 10) ";

                        grDate.Columns["Stare"].Visible = true;
                        grDate.Columns["IdStare"].Visible = false;
                        dt = General.IncarcaDT(strSql, new object[] { id });
                        break;
                    case 7:         //Org_DateIstoric
                        string op = "+";
                        if (Constante.tipBD == 2) op = "||";
                        strSql = "SELECT a.\"IdAuto\", a.\"Id\", a.\"IdCircuit\", a.\"IdUser\", a.\"IdStare\", a.\"Aprobat\", a.\"Culoare\", CASE WHEN a.\"Pozitie\" is null THEN 0 ELSE a.\"Pozitie\" END AS \"Pozitie\", "
                            + " a.\"DataAprobare\", CASE WHEN j2.F10008 IS NULL or j2.F10008 = '' THEN j1.F70104 ELSE  j2.F10008 " + op + " ' ' " + op + " j2.F10009 END as \"Nume\", "
                            + " CASE WHEN j8.F10008 IS NULL or j8.F10008 = '' THEN j7.F70104 ELSE  j8.F10008 " + op + " ' ' " + op + " j8.F10009 END as \"Inlocuitor\", '' AS \"Stare\" "
                            + " FROM \"Org_DateIstoric\" a "
                            + " JOIN USERS j1 on a.\"IdUser\" = j1.F70102 "
                            + " LEFT JOIN F100 j2 ON j1.F10003 = j2.F10003 "
                            + " LEFT JOIN USERS j7 on a.\"IdUserInlocuitor\" = j7.F70102 "
                            + " LEFT JOIN F100 j8 on j7.F10003 = j8.F10003 "
                            + " WHERE a.\"Id\" = " + id + " order by a.\"Pozitie\" ";                        
                        dt = General.IncarcaDT(strSql, null);
                        break;
                    case 8:     //Curs_CereriIstoric
                        op = "+";
                        if (Constante.tipBD == 2) op = "||";
                        strSql = "SELECT a.\"IdAuto\", a.\"IdCerere\", a.\"IdCircuit\", a.\"IdSuper\", a.\"IdStare\", '' as \"Stare\", a.\"Aprobat\", a.\"Culoare\", CASE WHEN a.\"Pozitie\" is null THEN 0 ELSE a.\"Pozitie\" END AS \"Pozitie\", "
                             + " a.\"DataAprobare\", CASE WHEN COALESCE(B.\"NumeComplet\",' ') <> ' ' THEN B.\"NumeComplet\" ELSE (CASE WHEN c.F10003 IS NULL THEN b.F70104 ELSE  c.F10008 " + op + " ' ' " + op + " c.F10009 END) END AS \"Nume\", '' AS \"Inlocuitor\" "
                             + " FROM \"Curs_CereriIstoric\" a "
                             + " LEFT JOIN USERS b on a.\"IdUSer\" = b.F70102 "
                             + " LEFT JOIN F100 c on b.F10003 = c.F10003 " 
                             + " WHERE a.\"IdCerere\" = " + id + " order by a.\"Pozitie\" ";
                        dt = General.IncarcaDT(strSql, null);                        
                        break;
                    case 9:
                        //tabela = "CM_CereriIstoric";
                        strSql = "SELECT a.\"IdAuto\", a.\"IdCerere\", a.\"IdCircuit\", a.\"IdSuper\", a.\"IdStare\", '' as \"Stare\", a.\"Aprobat\", a.\"Culoare\", CASE WHEN a.\"Pozitie\" is null THEN 0 ELSE a.\"Pozitie\" END AS \"Pozitie\", "
                             + " a.\"DataAprobare\", CASE WHEN c.F10003 IS NULL THEN b.F70104 ELSE  c.F10008 + ' ' + c.F10009 END AS \"Nume\", '' AS \"Inlocuitor\" "
                             + " FROM \"CM_CereriIstoric\" a "
                             + " LEFT JOIN USERS b on a.\"IdUSer\" = b.F70102 "
                             + " LEFT JOIN F100 c on b.F10003 = c.F10003 "
                             + " WHERE a.\"IdCerere\" = " + id + " order by a.\"Pozitie\" ";
                        dt = General.IncarcaDT(strSql, null);
                        break;
                    default:
                        //tabela = "Ptj_CereriIstoric";
                        break;
                }

                //dt = General.IncarcaDT(strSql, null);
                //dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                //grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }


        //private void IncarcaGrid(int idCerere, int tip = 1, int f10003 = -99, int an = -1, int luna = -1)
        //{
        //    try
        //    {
        //        //tip = 1  din cererei
        //        //tip = 2  din pontaj
        //        //tip = 3 - Cereri diverse Radu 13.08.2018

        //        string op = "+";
        //        if (Constante.tipBD == 2) op = "||";
        //        string tabela = "";

        //        //Florin 2018.08.21
        //        switch(tip)
        //        {
        //            case 1:
        //            case 2:
        //                tabela = "Ptj_CereriIstoric";
        //                break;
        //            case 3:
        //                tabela = "MP_CereriIstoric";
        //                break;
        //            case 4:
        //                tabela = "BP_Istoric";
        //                break;
        //            case 5:
        //                tabela = "Avs_CereriIstoric";
        //                break;
        //            default:
        //                tabela = "Ptj_CereriIstoric";
        //                break;
        //        }

        //        //Radu 12.10.2018
        //        string cmp = @"A.""IdCerere""";
        //        if (tip == 4 || tip == 5)
        //            cmp = @"A.""Id""";

        //        string strSql = @"SELECT A.""IdAuto"", {3}, A.""IdCircuit"", A.""IdSuper"", A.""IdStare"", A.""Culoare"", D.""Denumire"" AS ""NumeStare"",
        //                    COALESCE(A.""Pozitie"",0) AS ""Pozitie"", A.""DataAprobare"",
        //                    CASE WHEN COALESCE(J.""NumeComplet"",' ') <> ' ' THEN J.""NumeComplet"" ELSE (CASE WHEN COALESCE(K.F10008,' ') = ' ' THEN J.F70104 ELSE K.F10008 {1} ' ' {1} K.F10009 END) END AS ""Inlocuitor"",
        //                    CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {1} ' ' {1} C.F10009 END) END as ""Nume""
        //                    FROM ""{2}"" A
        //                    LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
        //                    LEFT JOIN F100 C ON B.F10003 = C.F10003
        //                    LEFT JOIN ""Ptj_tblStari"" D ON A.""IdStare""=D.""Id""
        //                    LEFT JOIN USERS J ON a.""IdUserInlocuitor"" = j.F70102
        //                    LEFT JOIN F100 K ON J.F10003 = K.F10003
        //                    WHERE {3} = {0}
        //                    ORDER BY A.""Pozitie"" ";

        //        strSql = string.Format(strSql, idCerere, op, tabela, cmp);

        //        if (tip == 2)
        //        {
        //            strSql = @"SELECT A.""IdAuto"", A.F10003, A.""An"", A.""Luna"", A.""IdSuper"", A.""IdStare"", D.""Culoare"", A.""DataAprobare"", D.""Denumire"" AS ""NumeStare"",
        //                CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {3} ' ' {3} C.F10009 END) END as ""Nume""
        //                FROM ""Ptj_CumulatIstoric"" A
        //                LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
        //                LEFT JOIN F100 C ON B.F10003 = C.F10003
        //                LEFT JOIN ""Ptj_tblStariPontaj"" D ON A.""IdStare""=D.""Id""
        //                WHERE A.""IdStare"" <>1 AND A.F10003 = {0} AND ""An""={1} AND A.""Luna""={2}
        //                ORDER BY A.""DataAprobare""";

        //            strSql = string.Format(strSql, f10003, an, luna, op);
        //        }

        //        DataTable dt = General.IncarcaDT(strSql, null);
        //        dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
        //        grDate.KeyFieldName = "IdAuto";
        //        grDate.DataSource = dt;
        //        grDate.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //}


        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
                if (e.DataColumn.FieldName == "IdStare")
                {
                    GridViewDataComboBoxColumn colStari = (grDate.Columns["IdStare"] as GridViewDataComboBoxColumn);
                    DataTable dt = colStari.PropertiesComboBox.DataSource as DataTable;

                    string idStare = e.GetValue("IdStare").ToString();
                    if (idStare == "") return;
                    DataRow[] lst = dt.Select("Id=" + idStare);
                    if (lst.Count() > 0 && lst[0]["Culoare"] != null)
                    {
                        e.Cell.BackColor = System.Drawing.ColorTranslator.FromHtml(lst[0]["Culoare"].ToString());
                    }
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