using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using DevExpress.Web;
using System.IO;

namespace WizOne.Personal
{
    public partial class Contracte : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            grDateContracte.DataBind();
            //grDateContracte.AddNewRow();
            foreach (dynamic c in grDateContracte.Columns)
            {
                try
                {
                    c.Caption = Dami.TraduCuvant(c.Caption);
                }
                catch (Exception) { }
            }
            grDateContracte.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("Actualizeaza");
            grDateContracte.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("Renunta");
            grDateContracte.SettingsCommandButton.DeleteButton.Image.ToolTip = Dami.TraduCuvant("Sterge");
            grDateContracte.SettingsCommandButton.DeleteButton.Image.AlternateText = Dami.TraduCuvant("Sterge");
            grDateContracte.SettingsCommandButton.NewButton.Image.ToolTip = Dami.TraduCuvant("Rand nou");
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

        private void IncarcaGrid()
        {
            string valMin = "100000";
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" ='ValMinView'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                valMin = dtParam.Rows[0][0].ToString();

            string sqlFinal = "SELECT a.*, CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Contracte\" a WHERE F10003 = " + Session["Marca"].ToString();
            if (Constante.tipBD == 2)
                sqlFinal = "SELECT " + General.SelectListaCampuriOracle("F100Contracte2", "IdContract") + ", CASE WHEN a.\"IdAuto\" < " + valMin + " THEN 1 ELSE 0 END AS \"Modificabil\" FROM \"F100Contracte\" a WHERE F10003 = " + Session["Marca"].ToString();
            DataTable dt = new DataTable();
            //string sqlFinal = "SELECT * FROM \"F100Contracte2\" WHERE F10003 = " + Session["Marca"].ToString();
            //DataTable dt = new DataTable();
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (ds.Tables.Contains("F100Contracte2"))
            {
                dt = ds.Tables["F100Contracte2"];
            }
            else
            {
                dt = General.IncarcaDT(sqlFinal, null);
                dt.TableName = "F100Contracte2";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
            }
            grDateContracte.KeyFieldName = "IdAuto";
            grDateContracte.DataSource = dt;

            string sql = @"SELECT * FROM ""Ptj_Contracte"" ";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("Ptj_Contracte", "Id");
            DataTable dtCtr = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colCtr = (grDateContracte.Columns["IdContract"] as GridViewDataComboBoxColumn);
            colCtr.PropertiesComboBox.DataSource = dtCtr;

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
                                AND TRUNC(X.""Ziua"") >= (select to_date('01-' || case F01012 when 1 then 'JAN' when 2 then 'FEB' when 3 then 'MAR' when 4 then 'APR' when 5 then 'MAY' when 6 then 'JUN' when 7 then 'JUL' when 8 then 'AUG' when 9 then 'SEP' when 10 then 'OCT' when 11 then 'NOV' when 12 then 'DEC' end || '-' ||  F01011,'DD-MM-YYYY') from F010)
                                AND (SELECT COUNT(*) FROM ""Ptj_Cumulat"" C WHERE X.F10003=C.F10003 
                                AND C.""An"" = TO_NUMBER(TO_CHAR(X.""Ziua"",'YYYY')) AND C.""Luna"" = TO_NUMBER(TO_CHAR(X.""Ziua"",'mm')) AND C.""IdStare"" <> 5 AND C.""IdStare"" <> 7) <> 0";
                }

                strSql = string.Format(strSql, Session["Marca"].ToString());

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
                            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
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
                                row[x] = Session["Marca"];
                                break;
                            case "IDAUTO":
                                if (Constante.tipBD == 1)
                                    row[x] = Convert.ToInt32(General.Nz(ds.Tables["F100Contracte2"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                else
                                    row[x] = Dami.NextId("F100Contracte2");
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
                        if (Convert.ToDateTime(e.NewValues["DataInceput"]) > Convert.ToDateTime(e.NewValues["DataSfarsit"]))
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

                    DateTime ? dtSf = null;
                    if (General.Nz(obj[1], "").ToString() != "")
                        dtSf = Convert.ToDateTime(obj[1]);

                    if (grDateContracte.EditingRowVisibleIndex != i && dtInc != null && dtSf != null && e.NewValues["DataInceput"] != null && e.NewValues["DataSfarsit"] != null)
                    {
                        try
                        {
                            if (Convert.ToDateTime(dtInc) <= Convert.ToDateTime(e.NewValues["DataSfarsit"]) && Convert.ToDateTime(e.NewValues["DataInceput"]) <= Convert.ToDateTime(dtSf))
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
                    if (!col.AutoIncrement && grDateContracte.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDateContracte.CancelEdit();
                Session["InformatiaCurentaPersonal"] = ds;
                grDateContracte.DataSource = ds.Tables["F100Contracte2"];
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
    }
}