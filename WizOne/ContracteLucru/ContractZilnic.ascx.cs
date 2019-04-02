using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ContracteLucru
{
    public partial class ContractZilnic : System.Web.UI.UserControl
    {

        DataTable dtPrg = new DataTable();
        DataTable dtCtrSch = new DataTable();
        protected void Page_Init(object sender, EventArgs e)
        {       
            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            table = ds.Tables[0];
            DataList0.DataSource = table;
            DataList0.DataBind();

            DataList1.DataSource = table;
            DataList1.DataBind();

            DataList2.DataSource = table;
            DataList2.DataBind();

            DataList3.DataSource = table;
            DataList3.DataBind();

            DataList4.DataSource = table;
            DataList4.DataBind();

            DataList5.DataSource = table;
            DataList5.DataBind();

            DataList6.DataSource = table;
            DataList6.DataBind();

            DataList7.DataSource = table;
            DataList7.DataBind();

            DataList8.DataSource = table;
            DataList8.DataBind();
            
            grDate0.DataBind();
            grDate1.DataBind();
            grDate2.DataBind();
            grDate3.DataBind();
            grDate4.DataBind();
            grDate5.DataBind();
            grDate6.DataBind();
            grDate7.DataBind();
            grDate8.DataBind();
                      

            //cmbSchimb_SelectedIndexChanged(0, grDate0, DataList0);
            //cmbSchimb_SelectedIndexChanged(1, grDate1, DataList1);
            //cmbSchimb_SelectedIndexChanged(2, grDate2, DataList2);
            //cmbSchimb_SelectedIndexChanged(3, grDate3, DataList3);
            //cmbSchimb_SelectedIndexChanged(4, grDate4, DataList4);
            //cmbSchimb_SelectedIndexChanged(5, grDate5, DataList5);
            //cmbSchimb_SelectedIndexChanged(6, grDate6, DataList6);
            //cmbSchimb_SelectedIndexChanged(7, grDate7, DataList7);
            //cmbSchimb_SelectedIndexChanged(8, grDate8, DataList8);

        }


        private void IncarcaGrid(ASPxGridView grDate, int tip, DataList dl)
        {          

            string sqlFinal = "SELECT * FROM \"Ptj_ContracteSchimburi\" WHERE \"IdContract\" = " + Session["IdContract"].ToString() + " AND \"TipSchimb\" = " + tip;
            DataTable dt = new DataTable();

            dtCtrSch = General.IncarcaDT("SELECT * FROM \"Ptj_ContracteSchimburi\"", null);
            dtCtrSch.Clear();

            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            dt = General.IncarcaDT(sqlFinal, null);
            grDate.KeyFieldName = "IdAuto";
            if (ds.Tables.Contains("Ptj_ContracteSchimburi"))
            {
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = " + tip);
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate.DataSource = dtCtrSch;
            }
            else
            {
                dt = General.IncarcaDT("SELECT * FROM \"Ptj_ContracteSchimburi\" WHERE \"IdContract\" = " + Session["IdContract"].ToString(), null);
                dt.TableName = "Ptj_ContracteSchimburi";
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                ds.Tables.Add(dt);
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = " + tip);
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate.DataSource = dtCtrSch;
                Session["InformatiaCurentaContracte"] = ds;
            }

            string sql = "SELECT convert(varchar(5), \"OraIntrare\", 108) AS \"OraIntrare1\", convert(varchar(5), \"OraIesire\", 108) AS \"OraIesire1\", a.* FROM \"Ptj_Programe\" a";
            if (Constante.tipBD == 2)
                sql = "SELECT TO_CHAR(\"OraIntrare\", 'HH24:mi') AS \"OraIntrare1\", TO_CHAR(\"OraIesire\", 'HH24:mi') AS \"OraIesire1\", a.* FROM \"Ptj_Programe\" a";
            dtPrg = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colPrg = (grDate.Columns["IdProgram"] as GridViewDataComboBoxColumn);
            colPrg.PropertiesComboBox.DataSource = dtPrg;

            DataTable dtVer = General.ListaModVerif();
            GridViewDataComboBoxColumn colVer = (grDate.Columns["ModVerificare"] as GridViewDataComboBoxColumn);
            colVer.PropertiesComboBox.DataSource = dtVer;
                        
        }



        #region grDate0
        protected void grDate0_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate0, 0, DataList0);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic0_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb0":
                    ds.Tables[0].Rows[0]["TipSchimb0"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(0, grDate0, DataList0);
                    break;
                case "cmbProg0":
                    ds.Tables[0].Rows[0]["Program0"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }

        protected void grDate0_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate0_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 0;
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "ORAINCEPUT":  
                                GridViewDataComboBoxColumn colPrg = (grDate0.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();                               
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);        
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate0.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }
                

                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate0.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 0");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate0.DataSource = dtCtrSch;
                    grDate0.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate0_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate0.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate0.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate0.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDate0.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 0");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate0.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate0_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate0.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 0");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate0.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion

        #region grDate1
        protected void grDate1_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate1, 1, DataList1);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic1_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb1":
                    ds.Tables[0].Rows[0]["TipSchimb1"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(1, grDate1, DataList1);
                    break;
                case "cmbProg1":
                    ds.Tables[0].Rows[0]["Program1"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }

        protected void grDate1_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate1_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 1;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate1.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate1.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }


                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate1.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 1");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate1.DataSource = dtCtrSch;
                    grDate1.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate1_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate1.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate1.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate1.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                } 

                if (valid)
                {
                    e.Cancel = true;
                    grDate1.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 1");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate1.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate1_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate1.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 1");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate1.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion


        #region grDate2
        protected void grDate2_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate2, 2, DataList2);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic2_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb2":
                    ds.Tables[0].Rows[0]["TipSchimb2"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(2, grDate2, DataList2);
                    break;
                case "cmbProg2":
                    ds.Tables[0].Rows[0]["Program2"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate2_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate2_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 2;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate2.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate2.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

   
                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate2.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 2");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate2.DataSource = dtCtrSch;
                    grDate2.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate2_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate2.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate2.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate2.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }

  
                if (valid)
                {
                    e.Cancel = true;
                    grDate2.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 2");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate2.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate2_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate2.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 2");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate2.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion


        #region grDate3
        protected void grDate3_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate3, 3, DataList3);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic3_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb3":
                    ds.Tables[0].Rows[0]["TipSchimb3"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(3, grDate3, DataList3);
                    break;
                case "cmbProg3":
                    ds.Tables[0].Rows[0]["Program3"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate3_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate3_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 3;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate3.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate3.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

       

                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate3.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 3");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate3.DataSource = dtCtrSch;
                    grDate3.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate3_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate3.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate3.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate3.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }


                if (valid)
                {
                    e.Cancel = true;
                    grDate3.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 3");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate3.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate3_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate3.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 3");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate3.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion


        #region grDate4
        protected void grDate4_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate4, 4, DataList4);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic4_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb4":
                    ds.Tables[0].Rows[0]["TipSchimb4"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(4, grDate4, DataList4);
                    break;
                case "cmbProg4":
                    ds.Tables[0].Rows[0]["Program4"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate4_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate4_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 4;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate4.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate4.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

  
                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate4.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 4");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate4.DataSource = dtCtrSch;
                    grDate4.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate4_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate4.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate4.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate4.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }


                if (valid)
                {
                    e.Cancel = true;
                    grDate4.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 4");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate4.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate4_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate4.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 4");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate4.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion


        #region grDate5
        protected void grDate5_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate5, 5, DataList5);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic5_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb5":
                    ds.Tables[0].Rows[0]["TipSchimb5"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(5, grDate5, DataList5);
                    break;
                case "cmbProg5":
                    ds.Tables[0].Rows[0]["Program5"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate5_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate5_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 5;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate5.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate5.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate5.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 5");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate5.DataSource = dtCtrSch;
                    grDate5.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate5_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate5.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate5.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate5.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDate5.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 5");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate5.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate5_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate5.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 5");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate5.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion

        #region grDate6
        protected void grDate6_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate6, 6, DataList6);                
            }
            catch (Exception)
            {

                throw;
            }
        }


        protected void pnlCtlCtrZilnic6_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb6":
                    ds.Tables[0].Rows[0]["TipSchimb6"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(6, grDate6, DataList6);
                    break;
                case "cmbProg6":
                    ds.Tables[0].Rows[0]["Program6"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate6_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate6_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 6;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate6.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate6.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }



                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate6.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 6");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate6.DataSource = dtCtrSch;
                    grDate6.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate6_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate6.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate6.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate6.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDate6.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 6");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate6.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate6_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate6.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 6");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate6.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion


        #region grDate7
        protected void grDate7_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate7, 7, DataList7);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic7_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb7":
                    ds.Tables[0].Rows[0]["TipSchimb7"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(7, grDate7, DataList7);
                    break;
                case "cmbProg7":
                    ds.Tables[0].Rows[0]["Program7"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate7_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate7_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 7;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate7.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate7.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

 
                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate7.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 7");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate7.DataSource = dtCtrSch;
                    grDate7.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate7_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate7.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate7.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate7.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }


                if (valid)
                {
                    e.Cancel = true;
                    grDate7.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 7");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate7.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate7_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate7.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 7");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate7.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion

        #region grDate8
        protected void grDate8_DataBinding(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid(grDate8, 8, DataList8);
            }
            catch (Exception)
            {

                throw;
            }
        }
        protected void pnlCtlCtrZilnic8_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            object valoare = (param[1] == "null" ? null : param[1]);
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "cmbSchimb8":
                    ds.Tables[0].Rows[0]["TipSchimb8"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    cmbSchimb_SelectedIndexChanged(8, grDate8, DataList8);
                    break;
                case "cmbProg8":
                    ds.Tables[0].Rows[0]["Program8"] = (valoare ?? DBNull.Value);
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
            }
        }
        protected void grDate8_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
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
            catch (Exception ex)
            {

            }
        }

        protected void grDate8_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
                bool valid = true;
                string errMsg = "";
                object[] row = new object[ds.Tables["Ptj_ContracteSchimburi"].Columns.Count];
                int x = 0;
                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDCONTRACT":
                                row[x] = Session["IdContract"];
                                break;
                            case "TIPSCHIMB":
                                row[x] = 8;
                                break;
                            case "ORAINCEPUT":
                                GridViewDataComboBoxColumn colPrg = (grDate8.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraIn = dr[0].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                                break;
                            case "ORASFARSIT":
                                colPrg = (grDate8.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                                index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                                dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                                string[] oraOut = dr[1].ToString().Split(':');
                                row[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                                break;
                            case "IDAUTO":
                                row[x] = Convert.ToInt32(General.Nz(ds.Tables["Ptj_ContracteSchimburi"].AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            case "DENUMIRE":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste nume schimb. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            case "IDPROGRAM":
                                if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                                {
                                    valid = false;
                                    errMsg += "Lipseste programul de lucru. ";
                                }
                                else
                                    row[x] = e.NewValues[col.ColumnName];
                                break;
                            default:
                                row[x] = e.NewValues[col.ColumnName];
                                break;
                        }
                    }

                    x++;
                }

                if (valid)
                {
                    ds.Tables["Ptj_ContracteSchimburi"].Rows.Add(row);
                    e.Cancel = true;
                    grDate8.CancelEdit();
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 8");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate8.DataSource = dtCtrSch;
                    grDate8.KeyFieldName = "IdAuto";
                    Session["InformatiaCurentaContracte"] = ds;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate8_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {

                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }
                bool valid = true;
                string errMsg = "";
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                foreach (DataColumn col in ds.Tables["Ptj_ContracteSchimburi"].Columns)
                {
                    if (!col.AutoIncrement && grDate8.Columns[col.ColumnName].Visible)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                    switch (col.ColumnName.ToUpper())
                    {
                        case "ORAINCEPUT":
                            GridViewDataComboBoxColumn colPrg = (grDate8.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            int index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            DataRow dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraIn = dr[0].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraIn[0]), Convert.ToInt32(oraIn[1]), 0);
                            break;
                        case "ORASFARSIT":
                            colPrg = (grDate8.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            index = (int)colPrg.PropertiesComboBox.Items.FindByValue(e.NewValues["IdProgram"]).Value;
                            dr = dtPrg.Select("Id = " + index).FirstOrDefault();
                            string[] oraOut = dr[1].ToString().Split(':');
                            row[col.ColumnName] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt32(oraOut[0]), Convert.ToInt32(oraOut[1]), 0);
                            break;
                        case "DENUMIRE":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste nume schimb. ";
                            }
                            break;
                        case "IDPROGRAM":
                            if (e.NewValues[col.ColumnName] == null || e.NewValues[col.ColumnName].ToString().Length <= 0)
                            {
                                valid = false;
                                errMsg += "Lipseste programul de lucru. ";
                            }
                            break;
                    }

                }

                if (valid)
                {
                    e.Cancel = true;
                    grDate8.CancelEdit();
                    Session["InformatiaCurentaContracte"] = ds;
                    DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 8");
                    dtCtrSch.Rows.Clear();
                    for (int i = 0; i < source.Count(); i++)
                        dtCtrSch.ImportRow(source[i]);
                    grDate8.DataSource = dtCtrSch;
                }
                else
                    ArataMesaj(Dami.TraduCuvant(errMsg));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        protected void grDate8_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;

                DataRow row = ds.Tables["Ptj_ContracteSchimburi"].Rows.Find(keys);

                row.Delete();

                e.Cancel = true;
                grDate8.CancelEdit();
                Session["InformatiaCurentaContracte"] = ds;
                DataRow[] source = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb = 8");
                dtCtrSch.Rows.Clear();
                for (int i = 0; i < source.Count(); i++)
                    dtCtrSch.ImportRow(source[i]);
                grDate8.DataSource = dtCtrSch;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }
        #endregion

        private void cmbSchimb_SelectedIndexChanged(int nr, ASPxGridView grDate, DataList dl)
        {
            try
            {
                ASPxComboBox cmbSchimb = dl.Items[0].FindControl("cmbSchimb" + nr) as ASPxComboBox;
                ASPxComboBox cmbProg = dl.Items[0].FindControl("cmbProg" + nr) as ASPxComboBox;

                if (Convert.ToInt32(cmbSchimb.Value ?? 1) == 1)
                {
                    cmbProg.Visible = true;
                    //grDate.ClientVisible = false;
                    grDate.Visible = false;
                    //grDate.Enabled = false;
                }
                else
                {
                    cmbProg.Visible = false;
                    //grDate.ClientVisible = true;
                    grDate.Visible = true;
                    //grDate.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void ArataMesaj(string mesaj)
        {
            pnlCtlCtrZilnic0.Controls.Add(new LiteralControl());
            WebControl script = new WebControl(HtmlTextWriterTag.Script);
            pnlCtlCtrZilnic0.Controls.Add(script);
            script.Attributes["id"] = "dxss_123456";
            script.Attributes["type"] = "text/javascript";
            script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        }

    }
}