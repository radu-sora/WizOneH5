using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

namespace WizOne.Pagini
{
    public partial class Contr2 : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Init(object sender, EventArgs e)
        {
            DataTable dtTabele = General.IncarcaDT(SelectAngajati());             
            cmbAng.DataSource = dtTabele;
            cmbAng.DataBind();
            cmbAng.SelectedIndex = 0;
            grDateContracte.DataBind();
  
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Dami.AccesApp();

    

                          

      

                
                //if (IsPostBack)
                //{
                //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                //    if (ds != null)
                //    {
                //        DataTable dt = ds.Tables["F100Contracte2"];
                //        grDateContracte.KeyFieldName = "IdAuto";
                //        grDateContracte.DataSource = dt;
                //        grDateContracte.DataBind();
                //    }

                //}
                //else
                //{
                //    Session["InformatiaCurentaPersonal"] = null;
                //    cmbAng.SelectedIndex = 0;
                //    IncarcaGrid();
                //}              
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Contracte\" a WHERE F10003 = " + Convert.ToInt32(cmbAng.Value);
            if (Constante.tipBD == 2)
                sqlFinal = "SELECT " + General.SelectListaCampuriOracle("F100Contracte2", "IdContract") + ", CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Contracte\" a WHERE F10003 = " + Convert.ToInt32(cmbAng.Value);

            string sql = @"SELECT * FROM ""Ptj_Contracte"" ORDER BY ""Denumire""";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Ptj_Contracte", "Id") + " ORDER BY \"Denumire\"";
            DataTable dtCtr = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colCtr = (grDateContracte.Columns["IdContract"] as GridViewDataComboBoxColumn);
            colCtr.PropertiesComboBox.DataSource = dtCtr;


            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            DataTable dt = new DataTable();
            if (ds != null && ds.Tables.Contains("F100Contracte2"))
            {
                dt = ds.Tables["F100Contracte2"];
            }
            else
            {
                ds = new DataSet();
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Contracte2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateContracte.KeyFieldName = "IdAuto";
            grDateContracte.DataSource = dt;


            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;

        }

        protected void grDateContracte_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string SelectAngajati(string filtru = "")
        {
            string strSql = "";

            try
            {
                string semn = "+";
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2)
                {
                    semn = "||";
                    cmp = "ROWNUM";
                }

                strSql = @"SELECT {2} AS ""IdAuto"", X.* FROM (
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                WHERE J.""IdUser"" = {0} ) X WHERE F10025 IN (0, 999) ORDER BY X.""NumeComplet"" ";

                strSql = string.Format(strSql, Session["UserId"].ToString(), semn, cmp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void grDateContracte_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Contracte2"];
                if (Constante.tipBD == 1)
                {
                    if (dt.Columns["IdAuto"] != null)
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            int max = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                            e.NewValues["IdAuto"] = max;
                        }
                        else
                            e.NewValues["IdAuto"] = 1;
                    }
                }
                else
                    e.NewValues["IdAuto"] = Dami.NextId("F100Contracte2");

                e.NewValues["Modificabil"] = 1;
                e.NewValues["DataInceput"] = DateTime.Now;

                //Florin 2019.09.26
                DataTable dtF100 = ds.Tables["F100"];
                if (dtF100 != null && dtF100.Rows.Count > 0)
                {
                    e.NewValues["DataInceput"] = dtF100.Rows[0]["F10022"];
                    e.NewValues["DataSfarsit"] = new DateTime(2100, 1, 1);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAct_Click(object sender, EventArgs e)
        {
            //Florin 2018.03.15

            try
            {
                string strSql = "";

                if (Constante.tipBD == 1)
                {
                    strSql = @"UPDATE B
                                SET B.IdContract = A.IdContract
                                FROM F100Contracte A
                                INNER JOIN Ptj_Intrari B ON A.F10003=B.F10003 AND CONVERT(date,A.DataInceput) <= CONVERT(date,B.Ziua) AND CONVERT(date,B.Ziua) <= CONVERT(date,A.DataSfarsit)
                                INNER JOIN Ptj_Cumulat C ON A.F10003=C.F10003 AND C.An = YEAR(B.Ziua) AND C.Luna = MONTH(B.Ziua) AND C.IdStare <> 5 AND C.IdStare <> 7
                                WHERE A.F10003= {0} AND CONVERT(date,B.Ziua) >= CONVERT(date,(SELECT CONVERT(nvarchar(4),F01011) + '-' + CONVERT(nvarchar(4),F01012) + '-01' FROM F010))";
                }
                else
                {
                    strSql = @"UPDATE ""Ptj_Intrari"" X
                                SET X.""IdContract"" = (SELECT A.""IdContract"" FROM ""F100Contracte"" A WHERE A.F10003=X.F10003 AND TRUNC(A.""DataInceput"") <= TRUNC(X.""Ziua"") 
                                AND TRUNC(X.""Ziua"") <= TRUNC(A.""DataSfarsit""))
                                WHERE X.F10003={0}
                                AND TRUNC(X.""Ziua"") >= (select to_date('01-' ||  F01012 || '-' ||  F01011,'DD-MM-YYYY') from F010)
                                AND (SELECT COUNT(*) FROM ""Ptj_Cumulat"" C WHERE X.F10003=C.F10003 
                                AND C.""An"" = TO_NUMBER(TO_CHAR(X.""Ziua"",'YYYY')) AND C.""Luna"" = TO_NUMBER(TO_CHAR(X.""Ziua"",'mm')) AND C.""IdStare"" <> 5 AND C.""IdStare"" <> 7) <> 0";
                }

                strSql = string.Format(strSql, Convert.ToInt32(cmbAng.Value));

                General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }



        protected void grDateContracte_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                //if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                //{
                //    e.Cancel = true;
                //    grDateContracte.CancelEdit();
                //    return;
                //}

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;


                if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                {
                    try
                    {
                        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                        {
                            grDateContracte.JSProperties["cpAlertMessage"] = "Data inceput mai mare decat data sfarsit!";
                            e.Cancel = true;
                            grDateContracte.CancelEdit();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (e.NewValues["DataInceput"] == null || e.NewValues["DataSfarsit"] == null)
                {
                    grDateContracte.JSProperties["cpAlertMessage"] = "Nu ati completat " + (e.NewValues["DataInceput"] == null ? "data inceput" : "data sfarsit") + "!";
                    e.Cancel = true;
                    grDateContracte.CancelEdit();
                    return;
                }

                for (int i = 0; i <= grDateContracte.VisibleRowCount - 1; i++)
                {
                    object[] obj = grDateContracte.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                    //DateTime? dtInc = Convert.ToDateTime(obj[0]);
                    //DateTime? dtSf = Convert.ToDateTime(obj[1]);

                    DateTime? dtInc = null;
                    if (General.Nz(obj[0], "").ToString() != "")
                        dtInc = Convert.ToDateTime(obj[0]);

                    DateTime? dtSf = null;
                    if (General.Nz(obj[1], "").ToString() != "")
                        dtSf = Convert.ToDateTime(obj[1]);

                    if (dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                    {
                        try
                        {
                            if (Convert.ToDateTime(dtInc).Date <= Convert.ToDateTime(e.NewValues["DataSfarsit"]).Date && Convert.ToDateTime(e.NewValues["DataInceput"]).Date <= Convert.ToDateTime(dtSf).Date)
                            {
                                grDateContracte.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                                e.Cancel = true;
                                grDateContracte.CancelEdit();
                                return;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                object[] row = new object[ds.Tables["F100Contracte2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Contracte2"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Convert.ToInt32(cmbAng.Value);
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Contracte2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100Contracte2");
                                break;
                            case "DATAINCEPUT":
                            case "DATASFARSIT":
                                row[x] = Convert.ToDateTime(e.NewValues[col.ColumnName]).Date;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                ds.Tables["F100Contracte2"].Rows.Add(row);
                e.Cancel = true;
                grDateContracte.CancelEdit();
                grDateContracte.DataSource = ds.Tables["F100Contracte2"];
                grDateContracte.KeyFieldName = "IdAuto";
                //grDateBeneficii.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;
                General.SalveazaDate(ds.Tables[0], ds.Tables[0].TableName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateContracte_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    e.Cancel = true;
                    grDateContracte.CancelEdit();
                    return;
                }

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }


                if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                {
                    try
                    {
                        if (Convert.ToDateTime(e.NewValues["DataInceput"]).Date > Convert.ToDateTime(e.NewValues["DataSfarsit"]).Date)
                        {
                            grDateContracte.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                            e.Cancel = true;
                            grDateContracte.CancelEdit();
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (e.NewValues["DataInceput"] == null || e.NewValues["DataSfarsit"] == null)
                {
                    grDateContracte.JSProperties["cpAlertMessage"] = "Nu ati completat " + (e.NewValues["DataInceput"] == null ? "data inceput" : "data sfarsit") + "!";
                    e.Cancel = true;
                    grDateContracte.CancelEdit();
                    return;
                }

                for (int i = 0; i <= grDateContracte.VisibleRowCount - 1; i++)
                {
                    object[] obj = grDateContracte.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                    //DateTime? dtInc = Convert.ToDateTime(obj[0]);
                    //DateTime? dtSf = Convert.ToDateTime(obj[1]) : null;

                    DateTime? dtInc = null;
                    if (General.Nz(obj[0], "").ToString() != "")
                        dtInc = Convert.ToDateTime(obj[0]);

                    DateTime? dtSf = null;
                    if (General.Nz(obj[1], "").ToString() != "")
                        dtSf = Convert.ToDateTime(obj[1]);

                    if (grDateContracte.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                    {
                        try
                        {
                            if (Convert.ToDateTime(dtInc).Date <= Convert.ToDateTime(e.NewValues["DataSfarsit"]).Date && Convert.ToDateTime(e.NewValues["DataInceput"]).Date <= Convert.ToDateTime(dtSf).Date)
                            {
                                grDateContracte.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                                e.Cancel = true;
                                grDateContracte.CancelEdit();
                                return;
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Contracte2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Contracte2"].Columns)
                {
                    if (!col.AutoIncrement && grDateContracte.Columns[col.ColumnName] != null && grDateContracte.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        if (col.ColumnName.ToUpper() == "DATAINCEPUT" || col.ColumnName.ToUpper() == "DATASFARSIT")
                            row[col.ColumnName] = Convert.ToDateTime(e.NewValues[col.ColumnName]).Date;
                        else
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateContracte.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateContracte.DataSource = ds.Tables["F100Contracte2"];
                General.SalveazaDate(ds.Tables[0], ds.Tables[0].TableName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateContracte_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                {
                    e.Cancel = true;
                    grDateContracte.CancelEdit();
                    return;
                }

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Contracte2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateContracte.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateContracte.DataSource = ds.Tables["F100Contracte2"];
                General.SalveazaDate(ds.Tables[0], ds.Tables[0].TableName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateContracte_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                //string[] fields = { "Modificabil" };
                DataRowView values = grDateContracte.GetRow(e.VisibleIndex) as DataRowView;

                if (values != null)
                {
                    string modif = values.Row["Modificabil"].ToString();

                    if (modif == "0")
                    {
                        if (e.ButtonType == ColumnCommandButtonType.Edit || e.ButtonType == ColumnCommandButtonType.Delete)

                            e.Visible = false;

                    }
                }
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                Session["InformatiaCurentaPersonal"] = null;
                grDateContracte.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }


}