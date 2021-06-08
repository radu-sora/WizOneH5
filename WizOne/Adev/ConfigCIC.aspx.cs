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
using System.Data.SqlClient;
using System.Data.OleDb;

namespace WizOne.Adev
{
    public partial class ConfigCIC : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                if (!IsPostBack)
                {
                    GridViewDataDateColumn col = (grDate.Columns["DATA_NASTERE"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    col = (grDate.Columns["MATERNITATE_DELA"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    col = (grDate.Columns["MATERNITATE_PANALA"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    col = (grDate.Columns["INDEMNIZATIE_DELA"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    col = (grDate.Columns["INDEMNIZATIE_PANALA"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    col = (grDate.Columns["DATA_APROBARE"] as GridViewDataDateColumn);
                    col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;

                    int marca = -99;
                    if (Session["MarcaConfigCIC"] != null)
                        marca = Convert.ToInt32(Session["MarcaConfigCIC"].ToString());

                    PregAdevCIC(marca);
                    IncarcaGrid();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                DataTable dtIst = General.IncarcaDT("SELECT * FROM ADEVERINTE_CIC_DATE WHERE EMITERE = 1", null);
         
                Session["InformatiaCurentaConfigCIC"] = dtIst;
                grDate.KeyFieldName = "MARCA;DATA_NASTERE";
                grDate.DataSource = dtIst;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                if (Session["MarcaConfigCIC"] != null)
                    e.NewValues["MARCA"] = Convert.ToInt32(Session["MarcaConfigCIC"].ToString());                           
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurentaConfigCIC"] as DataTable;

                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    row[x] = e.NewValues[col.ColumnName];
                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                grDate.KeyFieldName = "MARCA;DATA_NASTERE";
                grDate.DataSource = dt;

                Session["InformatiaCurentaConfigCIC"] = dt;
                SalvareDate();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurentaConfigCIC"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && grDate.Columns[col.ColumnName] != null && grDate.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaConfigCIC"] = dt;
                grDate.DataSource = dt;
                SalvareDate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurentaConfigCIC"] as DataTable;

                DataRow row = dt.Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurentaConfigCIC"] = dt;
                grDate.DataSource = dt;
                SalvareDate();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void PregAdevCIC(int marca)
        {
            try
            {
                string sql = "UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 0";
                General.ExecutaNonQuery(sql, null);

                sql = "SELECT F01012, F01011 FROM F010";
                DataTable entLC = General.IncarcaDT(sql, null);

                string data_luna = "";
                data_luna = "01/" + entLC.Rows[0]["F01012"].ToString().PadLeft(2, '0') + "/" + entLC.Rows[0]["F01011"].ToString();

                if (Constante.tipBD == 2)
                    sql = "SELECT TO_CHAR(MAX(F11006),'DD/MM/YYYY') FROM F110 WHERE F11003 = " + marca;
                else
                    sql = "SELECT CONVERT(VARCHAR, MAX(F11006),103) FROM F110 WHERE F11003 = " + marca;

                DataTable copil = General.IncarcaDT(sql, null);

                if (copil != null && copil.Rows.Count > 0 && copil.Rows[0][0].ToString().Length > 8)
                {
                    sql = "DELETE FROM ADEVERINTE_CIC_DATE WHERE MARCA = " + marca + " AND DATA_NASTERE < (SELECT MAX(F11006) FROM F110 WHERE F11003 = " + marca + ")";

                    if (Constante.tipBD == 2)
                    {
                        General.ExecutaNonQuery(sql, null);
                        General.ExecutaNonQuery("COMMIT", null);

                        sql = "INSERT INTO ADEVERINTE_CIC_DATE (MARCA, NUME, DATA_NASTERE, MATERNITATE_DELA, MATERNITATE_PANALA, INDEMNIZATIE_DELA, INDEMNIZATIE_PANALA, DATA_APROBARE, EMITERE) "
                            + " SELECT F10003, F10008 || ' ' || F10009 AS NUME, MAX(F11006) AS DN, DINC, NVL(d3.DSF,d.DSF)"
                            + ", null, null, CASE WHEN F10076 > MAX(F11006) AND F10076 < TO_DATE('01/01/2100','DD/MM/YYYY') THEN F10076 ELSE null "
                            + "END AS DA, 1 FROM F100 LEFT JOIN (SELECT F94003, MIN(F94037) AS DINC, MAX(F94038) AS DSF FROM F940 "
                            + "WHERE F94003 = " + marca + " AND F94010 IN (4409, 4410) AND F94037 > (TO_DATE('" + copil.Rows[0][0].ToString() + "', 'DD/MM/YYYY') - 126) AND F940607=8 "
                            + "GROUP BY F94003) d ON F10003=F94003 "
                            + "LEFT JOIN (SELECT F30003, MAX(F30038) AS DSF FROM F300 "
                            + "WHERE F30003 = " + marca + " AND F30010 IN (4409, 4410) AND F300607=8 "
                            + "GROUP BY F30003) d3 ON F10003=F30003"
                            + ", F110 WHERE F10003 = F11003 AND F10003 = " + marca + " AND NOT EXISTS "
                            + " (SELECT * FROM ADEVERINTE_CIC_DATE WHERE MARCA = " + marca + ") "
                            + " GROUP BY F10003, F10008 || ' ' || F10009, F10076, DINC, d.DSF, d3.DSF";
                        General.ExecutaNonQuery(sql, null);
                        sql = "UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 1 WHERE MARCA = " + marca;
                        General.ExecutaNonQuery(sql, null);
                        General.ExecutaNonQuery("COMMIT", null);
                    }
                    else
                    {
                        General.ExecutaNonQuery(sql, null);

                        sql = "INSERT INTO ADEVERINTE_CIC_DATE (MARCA, NUME, DATA_NASTERE, MATERNITATE_DELA, MATERNITATE_PANALA, INDEMNIZATIE_DELA, INDEMNIZATIE_PANALA, DATA_APROBARE, EMITERE) "
                            + " SELECT F10003, F10008 + ' ' + F10009 AS NUME, MAX(F11006) AS DN, DINC, ISNULL(d3.DSF,d.DSF)"
                            + ", null, null, CASE WHEN F10076 > MAX(F11006) AND F10076 < CONVERT(DATETIME, '01/01/2100',103) THEN F10076 ELSE null "
                            + " END AS DA, 1 FROM F100  LEFT JOIN (SELECT F94003, MIN(F94037) AS DINC, MAX(F94038) AS DSF FROM F940 "
                            + "WHERE F94003 = " + marca + " AND F94010 IN (4409, 4410) AND F94037 > (CONVERT(DATETIME, '" + copil.Rows[0][0].ToString() + "', 103) - 126) AND F940607=8 "
                            + "GROUP BY F94003) d ON F10003=F94003 "
                            + "LEFT JOIN (SELECT F30003, MAX(F30038) AS DSF FROM F300 "
                            + "WHERE F30003 = " + marca + " AND F30010 IN (4409, 4410) AND F300607=8 "
                            + "GROUP BY F30003) d3 ON F10003=F30003"
                            + ", F110 WHERE F10003 = F11003 AND F10003 = " + marca + " AND NOT EXISTS "
                            + " (SELECT * FROM ADEVERINTE_CIC_DATE WHERE MARCA = " + marca + ") "
                            + " GROUP BY F10003, F10008 + ' ' + F10009, F10076, DINC, d.DSF, d3.DSF";
                        General.ExecutaNonQuery(sql, null);
                        sql = "UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 1 WHERE MARCA = " + marca;
                        General.ExecutaNonQuery(sql, null);
                    }
                }
                else
                {
                    if (Constante.tipBD == 2)
                    {
                        sql = "INSERT INTO ADEVERINTE_CIC_DATE (MARCA, NUME, DATA_NASTERE, MATERNITATE_DELA, MATERNITATE_PANALA, INDEMNIZATIE_DELA, INDEMNIZATIE_PANALA, DATA_APROBARE, EMITERE) "
                            + " SELECT F10003, F10008 || ' ' || F10009 AS NUME, TO_DATE('" + data_luna + "', 'DD/MM/YYYY') AS DN, null, null, "
                            + "null, null, null, "
                            + "1 FROM F100 WHERE F10003 = " + marca + " AND NOT EXISTS "
                            + " (SELECT * FROM ADEVERINTE_CIC_DATE WHERE MARCA = " + marca + ") "
                            + " GROUP BY F10003, F10008 || ' ' || F10009";
                        General.ExecutaNonQuery(sql, null);
                        sql = "UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 1 WHERE MARCA = " + marca;
                        General.ExecutaNonQuery(sql, null);
                        General.ExecutaNonQuery("COMMIT", null);
                    }
                    else
                    {
                        sql = "INSERT INTO ADEVERINTE_CIC_DATE (MARCA, NUME, DATA_NASTERE, MATERNITATE_DELA, MATERNITATE_PANALA, INDEMNIZATIE_DELA, INDEMNIZATIE_PANALA, DATA_APROBARE, EMITERE) "
                            + " SELECT F10003, F10008 + ' ' + F10009 AS NUME, CONVERT(DATETIME, '" + data_luna + "', 103) AS DN, null, null, "
                            + "null, null, null, "
                            + "1 FROM F100 WHERE F10003 = " + marca + " AND NOT EXISTS "
                            + " (SELECT * FROM ADEVERINTE_CIC_DATE WHERE MARCA = " + marca + ") "
                            + " GROUP BY F10003, F10008 + ' ' + F10009";
                        General.ExecutaNonQuery(sql, null);
                        sql = "UPDATE ADEVERINTE_CIC_DATE SET EMITERE = 1 WHERE MARCA = " + marca;
                        General.ExecutaNonQuery(sql, null);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private void SalvareDate()
        {
            try
            {
                DataTable dt = Session["InformatiaCurentaConfigCIC"] as DataTable;
                General.SalveazaDate(dt, "ADEVERINTE_CIC_DATE");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnGen_Click(object sender, EventArgs e)
        {
            try
            {
                Adeverinta frm = new Adeverinta();
                frm.btnGen_Click(sender,e);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Adev", new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}