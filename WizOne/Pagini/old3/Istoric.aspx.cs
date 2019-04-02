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
                    int tip = Convert.ToInt32(General.Nz(Request["tip"], -99)); //Radu 13.08.2018
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

        private void IncarcaGrid(int idCerere, int tip = 1, int f10003 = -99, int an = -1, int luna = -1)
        {
            try
            {
                //tip = 1  din cererei
                //tip = 2  din pontaj
                //tip = 3 - Cereri diverse Radu 13.08.2018

                string op = "+";
                if (Constante.tipBD == 2) op = "||";
                string tabela = "";

                //Florin 2018.08.21
                switch(tip)
                {
                    case 1:
                    case 2:
                        tabela = "Ptj_CereriIstoric";
                        break;
                    case 3:
                        tabela = "MP_CereriIstoric";
                        break;
                    case 4:
                        tabela = "BP_Istoric";
                        break;
                    case 5:
                        tabela = "Avs_CereriIstoric";
                        break;
                    default:
                        tabela = "Ptj_CereriIstoric";
                        break;
                }

                //Radu 12.10.2018
                string cmp = @"A.""IdCerere""";
                if (tip == 4 || tip == 5)
                    cmp = @"A.""Id""";

                string strSql = @"SELECT A.""IdAuto"", {3}, A.""IdCircuit"", A.""IdSuper"", A.""IdStare"", A.""Culoare"", D.""Denumire"" AS ""NumeStare"",
                            COALESCE(A.""Pozitie"",0) AS ""Pozitie"", A.""DataAprobare"",
                            CASE WHEN COALESCE(J.""NumeComplet"",' ') <> ' ' THEN J.""NumeComplet"" ELSE (CASE WHEN COALESCE(K.F10008,' ') = ' ' THEN J.F70104 ELSE K.F10008 {1} ' ' {1} K.F10009 END) END AS ""Inlocuitor"",
                            CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {1} ' ' {1} C.F10009 END) END as ""Nume""
                            FROM ""{2}"" A
                            LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
                            LEFT JOIN F100 C ON B.F10003 = C.F10003
                            LEFT JOIN ""Ptj_tblStari"" D ON A.""IdStare""=D.""Id""
                            LEFT JOIN USERS J ON a.""IdUserInlocuitor"" = j.F70102
                            LEFT JOIN F100 K ON J.F10003 = K.F10003
                            WHERE {3} = {0}
                            ORDER BY A.""Pozitie"" ";

                strSql = string.Format(strSql, idCerere, op, tabela, cmp);

                if (tip == 2)
                {
                    strSql = @"SELECT A.""IdAuto"", A.F10003, A.""An"", A.""Luna"", A.""IdSuper"", A.""IdStare"", D.""Culoare"", A.""DataAprobare"", D.""Denumire"" AS ""NumeStare"",
                        CASE WHEN COALESCE(B.""NumeComplet"",' ') <> ' ' THEN B.""NumeComplet"" ELSE (CASE WHEN COALESCE(C.F10008,' ') = ' ' THEN B.F70104 ELSE C.F10008 {3} ' ' {3} C.F10009 END) END as ""Nume""
                        FROM ""Ptj_CumulatIstoric"" A
                        LEFT JOIN USERS B ON A.""IdUser"" = B.F70102
                        LEFT JOIN F100 C ON B.F10003 = C.F10003
                        LEFT JOIN ""Ptj_tblStariPontaj"" D ON A.""IdStare""=D.""Id""
                        WHERE A.""IdStare"" <>1 AND A.F10003 = {0} AND ""An""={1} AND A.""Luna""={2}
                        ORDER BY A.""DataAprobare""";

                    strSql = string.Format(strSql, f10003, an, luna, op);
                }

                DataTable dt = General.IncarcaDT(strSql, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

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