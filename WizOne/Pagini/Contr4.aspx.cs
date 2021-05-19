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
    public partial class Contr4 : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Init(object sender, EventArgs e)
        {
            DataTable dtTabele = General.IncarcaDT(SelectAngajati());             
            cmbAng.DataSource = dtTabele;
            cmbAng.DataBind();
            cmbAng.SelectedIndex = 0;
            grDateSupervizori.DataBind();
  
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSupervizori_DataBinding(object sender, EventArgs e)
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


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                Session["InformatiaCurentaPersonal"] = null;
                grDateSupervizori.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Supervizori\" a WHERE F10003 = " + Convert.ToInt32(cmbAng.Value);
            DataTable dt = new DataTable();
            DataSet ds = HttpContext.Current.Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds != null && ds.Tables.Contains("F100Supervizori2"))
            {
                dt = ds.Tables["F100Supervizori2"];
            }
            else
            {
                ds = new DataSet();
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Supervizori2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }


            grDateSupervizori.KeyFieldName = "IdAuto";
            grDateSupervizori.DataSource = dt;
            //grDateSupervizori.DataBind();

            DataTable dtSuper = General.IncarcaDT(@"SELECT CAST(""Id"" AS INT) AS ""Id"", CASE WHEN ""Alias"" IS NULL THEN ""Denumire"" ELSE ""Alias"" END AS ""Denumire"" FROM ""tblSupervizori"" ORDER BY ""Denumire""", null);
            GridViewDataComboBoxColumn colSuper = (grDateSupervizori.Columns["IdSuper"] as GridViewDataComboBoxColumn);
            colSuper.PropertiesComboBox.DataSource = dtSuper;

            string sql = @"SELECT * FROM USERS ORDER BY F70104";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("USERS", "F70102") + " ORDER BY F70104";
            DataTable dtUser = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colUser = (grDateSupervizori.Columns["IdUser"] as GridViewDataComboBoxColumn);
            colUser.PropertiesComboBox.DataSource = dtUser;

            HttpContext.Current.Session["InformatiaCurentaPersonal"] = ds;
        }

        protected void grDateSupervizori_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                DataTable dt = ds.Tables["F100Supervizori2"];

                //string sqlFinal = "SELECT * FROM \"F100Supervizori2\" WHERE F10003 = " + Session["Marca"].ToString();
                //dt = General.IncarcaDT(sqlFinal, null);
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
                    e.NewValues["IdAuto"] = Dami.NextId("F100Supervizori2");

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
            catch (Exception)
            {
                //MessageBox.Show(this, ex, MessageBox.icoError, "");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDateSupervizori_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                //if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //{
                //    try
                //    {
                //        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                //        {
                //            grDateSupervizori.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateSupervizori.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateSupervizori.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateSupervizori.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (grDateSupervizori.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateSupervizori.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateSupervizori.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Supervizori2"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["F100Supervizori2"].Columns)
                {
                    if (!col.AutoIncrement && grDateSupervizori.Columns[col.ColumnName] != null && grDateSupervizori.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        protected void grDateSupervizori_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                //if (e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //{
                //    try
                //    {
                //        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
                //        {
                //            grDateSupervizori.JSProperties["cpAlertMessage"] = "Data inceput este mai mare decat data sfarsit!";
                //            e.Cancel = true;
                //            grDateSupervizori.CancelEdit();
                //            return;
                //        }
                //    }
                //    catch (Exception)
                //    {
                //    }
                //}

                //for (int i = 0; i <= grDateSupervizori.VisibleRowCount - 1; i++)
                //{
                //    object[] obj = grDateSupervizori.GetRowValues(i, new string[] { "DataInceput", "DataSfarsit" }) as object[];

                //    DateTime dtInc = Convert.ToDateTime(obj[0]);
                //    DateTime dtSf = Convert.ToDateTime(obj[1]);

                //    if (dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                //    {
                //        try
                //        {
                //            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
                //            {
                //                grDateSupervizori.JSProperties["cpAlertMessage"] = "Intervalul ales se intersecteaza cu altul deja existent!";
                //                e.Cancel = true;
                //                grDateSupervizori.CancelEdit();
                //                return;
                //            }
                //        }
                //        catch (Exception)
                //        {

                //        }
                //    }
                //}

                //string sqlFinal = "SELECT * FROM \"F100Supervizori2\" WHERE F10003 = " + Session["Marca"].ToString();
                //dt = General.IncarcaDT(sqlFinal, null);
                object[] row = new object[ds.Tables["F100Supervizori2"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["F100Supervizori2"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "F10003":
                                row[x] = Convert.ToInt32(cmbAng.Value);
                                break;
                            case "NUMECOMPLET":
                                row[x] = ds.Tables["F100"].Rows[0]["F10008"].ToString() + " " + ds.Tables["F100"].Rows[0]["F10009"].ToString();
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Supervizori2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100Supervizori2");
                                break;
                            case "DATAINCEPUT":
                                row[x] = new DateTime(1900, 1, 1);
                                break;
                            case "DATASFARSIT":
                                row[x] = new DateTime(2100, 1, 1);
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

                ds.Tables["F100Supervizori2"].Rows.Add(row);
                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                //Session["DateAngajat"] = dt;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];
                grDateSupervizori.KeyFieldName = "IdAuto";
                //grDateSupervizori.DataBind();
                //grDateSupervizori.AddNewRow();
                Session["InformatiaCurentaPersonal"] = ds;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSupervizori_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                DataRow row = ds.Tables["F100Supervizori2"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDateSupervizori.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateSupervizori.DataSource = ds.Tables["F100Supervizori2"];


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateSupervizori_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e)
        {
            if (e.VisibleIndex >= 0)
            {
                DataRowView values = grDateSupervizori.GetRow(e.VisibleIndex) as DataRowView;

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



    }


}