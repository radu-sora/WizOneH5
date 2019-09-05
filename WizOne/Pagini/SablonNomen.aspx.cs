﻿using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class SablonNomen : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        string msgError = string.Empty;

        public class metaTblNomenConfig_SelectStr
        {
            public int Id { get; set; }
            public string SelectStr { get; set; }
            public string NumeCamp { get; set; }
            public string NumeCampDuplicat { get; set; }
            public object dt { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                Dami.AccesApp();


                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup")>=0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$")+1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    DataSet ds = new DataSet();
                    string gridKey = "";

                    DataTable dt = General.IncarcaDT("SELECT * FROM \"" + Session["Sablon_Tabela"] + "\"", null);

                    DataColumn[] keys = dt.PrimaryKey;
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        gridKey += ";" + keys[i].ToString();
                    }

                    //determinam campurile care formeaza cheia primara
                    string strSql = @"SELECT COLUMN_NAME
                                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                                    WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
                                    AND TABLE_NAME = '" + Session["Sablon_Tabela"] + "' ";

                    if (Constante.tipBD == 2)
                    {
                        strSql = @"SELECT B.column_name
                                    FROM all_constraints A
                                    INNER JOIN all_cons_columns B ON A.constraint_name = B.constraint_name AND A.owner = B.owner
                                    WHERE A.owner = '" + Constante.BD + "' AND B.table_name = '" + Session["Sablon_Tabela"] + "' AND A.constraint_type = 'P'";
                    }
                    DataTable dtKey = General.IncarcaDT(strSql, null);
                    if (dtKey.Rows.Count > 0)
                    {
                        //DataColumn[] keys = new DataColumn[dtKey.Rows.Count];
                        keys = new DataColumn[dtKey.Rows.Count];

                        for (int i = 0; i < dtKey.Rows.Count; i++)
                        {
                            keys[i] = dt.Columns[dtKey.Rows[i][0].ToString()];
                            gridKey = gridKey + ";" + dtKey.Rows[i][0].ToString();
                        }

                        dt.PrimaryKey = keys;
                    }

                    Session["InformatiaCurenta"] = dt;

                    DataTable dtCfg = General.IncarcaDT("SELECT * FROM \"tblNomenConfig\" WHERE \"Tabela\"= '" + Session["Sablon_Tabela"] + "'", null);
                    string sqlSec = $@"SELECT X.""IdColoana"", MIN(X.""Vizibil"") AS ""Vizibil"", MIN(X.""Blocat"") AS ""Blocat"" FROM (
                                    SELECT A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                    FROM ""Securitate"" A
                                    INNER JOIN ""relGrupUser"" B ON A.""IdGrup"" = B.""IdGrup""
                                    WHERE B.""IdUser"" = @1 AND A.""IdForm"" = '{"tbl." + Session["Sablon_Tabela"].ToString()}' AND A.""IdControl"" = 'grDate'
                                    UNION
                                    SELECT A.""IdColoana"", A.""Vizibil"", A.""Blocat""
                                    FROM ""Securitate"" A
                                    WHERE A.""IdGrup"" = -1 AND A.""IdForm"" = '{"tbl." + Session["Sablon_Tabela"].ToString()}' AND A.""IdControl"" = 'grDate') X
                                    GROUP BY X.""IdColoana"" ";
                    //DataTable dtSec = General.IncarcaDT(sqlSec, new string[] { "tbl." + Session["Sablon_Tabela"].ToString(), Session["UserId"].ToString() });
                    DataTable dtSec = General.IncarcaDT(sqlSec, new string[] { Session["UserId"].ToString() });

                    List<metaTblNomenConfig_SelectStr> lst = new List<metaTblNomenConfig_SelectStr>();
                    int x = 1;

                    //adaugam coloanele la grid
                    foreach (DataColumn col in dt.Columns)
                    {
                        //aplicam securitatea
                        bool vizibil = true;
                        bool blocat = false;

                        if (dtSec.Rows.Count > 0 && dtSec.Select("IdColoana='" + col.ColumnName + "'").Count() > 0)
                        {
                            vizibil = Convert.ToBoolean(dtSec.Select("IdColoana='" + col.ColumnName + "'")[0]["Vizibil"]);
                            blocat = Convert.ToBoolean(dtSec.Select("IdColoana='" + col.ColumnName + "'")[0]["Blocat"]);
                        }

                        //tabela de configurare a nomenclatorului
                        int tipCamp = 0;
                        DataRow dr = null;
                        if (dtCfg.Rows.Count > 0 && dtCfg.Select("Camp='" + col.ColumnName + "'").Count() > 0)
                        {
                            dr = dtCfg.Select("Camp='" + col.ColumnName + "'")[0];
                            tipCamp = Convert.ToInt32(dr["TipCamp"] == DBNull.Value ? 0 : dr["TipCamp"]);
                            if (dr != null && tipCamp == 2 && dr["SursaCombo"].ToString() == "") tipCamp = 0;
                            if (vizibil) vizibil = Convert.ToInt32(dr["Vizibil"] ?? 0) == 1 ? true : false;
                        }

                        
                        //0 - General; 1 - CheckBox; 2 - ComboBox; 3 - Date; 4 - Memo; 5 - ColorPicker; 6 - Text; 7 - Numeric


                        switch (tipCamp)
                        {
                            case 0:                             //General
                                {
                                    GridViewDataColumn c = new GridViewDataColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    
                                    if (col.DataType.Name.ToLower() == "int16" || col.DataType.Name.ToLower() == "int32" || col.DataType.Name.ToLower() == "int64")
                                        c.Width = Unit.Pixel(70);
                                    else
                                        c.Width = Unit.Pixel(200);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 1:                             //CheckBox
                                {
                                    GridViewDataCheckColumn c = new GridViewDataCheckColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(70);
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 2:                             //ComboBox
                                {
                                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(200);

                                    c.PropertiesComboBox.AllowNull = true;

                                    if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                    {
                                        //optimizare - cazul de la Circuite (de ex: Ptj_Circuite)
                                        //mai multe coloane pot avea aceeasi sursa de date)
                                        string sursa = (dr["SursaCombo"] ?? "").ToString().Trim();
                                        DataTable dtCmb = new DataTable();

                                        if (sursa != "" && lst.Where(p => p.SelectStr == sursa).Count() > 0)
                                        {
                                            //dtCmb = ds.Tables["dt" + lst.Where(p => p.SelectStr == sursa).FirstOrDefault().Id];
                                            //dtCmb = ds.Tables[lst.Where(p => p.SelectStr == sursa).FirstOrDefault().NumeCamp];

                                            var ent = lst.Where(p => p.SelectStr == sursa).FirstOrDefault();
                                            if (ent != null)
                                            {
                                                lst.Add(new metaTblNomenConfig_SelectStr { Id = x, SelectStr = sursa, NumeCamp = c.FieldName, NumeCampDuplicat = ent.NumeCamp, dt = null });
                                                dtCmb = ent.dt as DataTable;
                                            }
                                        }
                                        else
                                        {
                                            dtCmb = General.IncarcaDT(sursa, null);
                                            //dtCmb.TableName = "dt" + x;
                                            dtCmb.TableName = c.FieldName;
                                            lst.Add(new metaTblNomenConfig_SelectStr { Id = x, SelectStr = sursa, NumeCamp = c.FieldName, NumeCampDuplicat="", dt = dtCmb });
                                            ds.Tables.Add(dtCmb);
                                            x++;
                                        }

                                        //DataTable dtCmb = General.IncarcaDT(dr["SursaCombo"].ToString(), null);
                                        c.PropertiesComboBox.DataSource = dtCmb;
                                        c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
                                        c.PropertiesComboBox.ValueType = dtCmb.Columns[0].GetType();
                                        switch (dtCmb.Columns.Count)
                                        {
                                            case 1:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
                                                break;
                                            default:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
                                                break;
                                        }

                                    }

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 3:                             //Date
                                {
                                    GridViewDataDateColumn c = new GridViewDataDateColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(100);
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 4:                             //Memo
                                {
                                    GridViewDataMemoColumn c = new GridViewDataMemoColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(200);
                                    
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 5:                             //Color
                                {
                                    GridViewDataColorEditColumn c = new GridViewDataColorEditColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(100);
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 6:                             //Text
                                {
                                    GridViewDataTextColumn c = new GridViewDataTextColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;

                                    c.Width = Unit.Pixel(200);
                                    if (col.MaxLength != -1) c.PropertiesTextEdit.MaxLength = col.MaxLength;
                                    //if (col.ColumnName == "F70103") c.PropertiesTextEdit.Password = true;
                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 7:                             //Numeric
                                {
                                    GridViewDataSpinEditColumn c = new GridViewDataSpinEditColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;

                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(70);

                                    grDate.Columns.Add(c);
                                }
                                break;
                            case 8:                             //ComboBox cu coloane multiple
                                {
                                    GridViewDataComboBoxColumn c = new GridViewDataComboBoxColumn();
                                    c.Name = col.ColumnName;
                                    c.FieldName = col.ColumnName;
                                    c.Caption = Dami.TraduCuvant(col.ColumnName);
                                    if (cmp.IndexOf(col.ColumnName.ToUpper() + ",") >= 0)
                                        c.Visible = false;
                                    else
                                        c.Visible = vizibil;
                                    c.ReadOnly = blocat;
                                    c.Width = Unit.Pixel(200);
                                    
                                    c.PropertiesComboBox.AllowNull = true;

                                    if (dr != null && dr["SursaCombo"].ToString() != "" && c.Visible == true)
                                    {
                                        //optimizare - cazul de la Circuite (de ex: Ptj_Circuite)
                                        //mai multe coloane pot avea aceeasi sursa de date)
                                        string sursa = (dr["SursaCombo"] ?? "").ToString().Trim();
                                        DataTable dtCmb = new DataTable();

                                        if (sursa != "" && lst.Where(p => p.SelectStr == sursa).Count() > 0)
                                        {
                                            //dtCmb = ds.Tables["dt" + lst.Where(p => p.SelectStr == sursa).FirstOrDefault().Id];
                                            //dtCmb = ds.Tables[lst.Where(p => p.SelectStr == sursa).FirstOrDefault().NumeCamp];

                                            var ent = lst.Where(p => p.SelectStr == sursa).FirstOrDefault();
                                            if (ent != null)
                                            {
                                                lst.Add(new metaTblNomenConfig_SelectStr { Id = x, SelectStr = sursa, NumeCamp = c.FieldName, NumeCampDuplicat = ent.NumeCamp, dt = null });
                                                dtCmb = ent.dt as DataTable;
                                            }
                                        }
                                        else
                                        {
                                            dtCmb = General.IncarcaDT(sursa, null);
                                            //dtCmb.TableName = "dt" + x;
                                            dtCmb.TableName = c.FieldName;
                                            lst.Add(new metaTblNomenConfig_SelectStr { Id = x, SelectStr = sursa, NumeCamp = c.FieldName, NumeCampDuplicat = "", dt = dtCmb });
                                            ds.Tables.Add(dtCmb);
                                            x++;
                                        }

                                        //DataTable dtCmb = General.IncarcaDT(dr["SursaCombo"].ToString(), null);
                                        c.PropertiesComboBox.DataSource = dtCmb;
                                        c.PropertiesComboBox.ValueField = dtCmb.Columns[0].ColumnName;
                                        c.PropertiesComboBox.ValueType = dtCmb.Columns[0].GetType();                                        
                                        switch (dtCmb.Columns.Count)
                                        {
                                            case 1:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[0].ColumnName;
                                                break;
                                            default:
                                                c.PropertiesComboBox.TextField = dtCmb.Columns[1].ColumnName;
                                                break;
                                        }

                                        for (int i = 1; i < dtCmb.Columns.Count; i++)
                                        {
                                            ListBoxColumn column = new ListBoxColumn();
                                            column.FieldName = dtCmb.Columns[i].ColumnName;
                                            c.PropertiesComboBox.Columns.Add(column);
                                        }

                                    }

                                    grDate.Columns.Add(c);
                                }
                                break;
                        }
                    }

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "Id";
                    if (gridKey != "") grDate.KeyFieldName = gridKey.Substring(1);
                    grDate.DataBind();

                    Session["SurseCombo"] = ds;
                    Session["SurseCombo_BIS"] = lst;

                    //ds.Clear();
                    //ds = null;
                }
                else
                {
                    DataSet ds = Session["SurseCombo"] as DataSet;
                    List<metaTblNomenConfig_SelectStr> lst = Session["SurseCombo_BIS"] as List<metaTblNomenConfig_SelectStr>;
                    foreach (var c in grDate.Columns)
                    {
                        try
                        {
                            GridViewDataColumn col = (GridViewDataColumn)c;
                            if (col.Visible == false) continue;

                            col.Caption = Dami.TraduCuvant(col.FieldName);

                            //in cazul in care este Combobox, adaugam sursele de date retinute in variabila de sesiune
                            var ent = lst.Where(p => p.NumeCamp == col.FieldName).FirstOrDefault();
                            if (ent == null) continue;
                            if (ent.dt != null)
                            {
                                GridViewDataComboBoxColumn cmb = (GridViewDataComboBoxColumn)c;
                                cmb.PropertiesComboBox.DataSource = ent.dt;
                            }
                            else
                            {
                                if (ent.NumeCampDuplicat != "")
                                {
                                    var entBis = lst.Where(p => p.NumeCamp == ent.NumeCampDuplicat).FirstOrDefault();
                                    if (entBis != null && entBis.dt != null)
                                    {
                                        GridViewDataComboBoxColumn cmb = (GridViewDataComboBoxColumn)c;
                                        cmb.PropertiesComboBox.DataSource = entBis.dt;
                                    }

                                }
                            }



                            //DataTable dt = ds.Tables[col.FieldName];
                            //if (dt != null)
                            //{
                            //    GridViewDataComboBoxColumn cmb = (GridViewDataComboBoxColumn)c;
                            //    cmb.PropertiesComboBox.DataSource = dt;
                            //}
                        }
                        catch (Exception) { }

                        //if (c.GetType().ToString() == "DevExpress.Web.GridViewDataColumn")
                        //{
                        //    string m = (string)HttpContext.GetGlobalResourceObject("General", ((GridViewDataColumn)c).FieldName.Replace(" ", "").Replace("!", "").Replace("/", ""), new CultureInfo(Session["IdLimba"]));
                        //    if (m != null && m != "") ((GridViewDataColumn)c).Caption = m;
                        //}
                    }

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow found = dt.Rows.Find(keys);
                found.Delete();

                e.Cancel = true;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        //{
        //    try
        //    {
        //        string strSql = "SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND COLUMN_DEFAULT IS NOT NULL";
        //        if (Constante.tipBD == 2)
        //            strSql = "SELECT COLUMN_NAME, DATA_DEFAULT AS COLUMN_DEFAULT FROM COLS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND DATA_DEFAULT IS NOT NULL";
        //        DataTable dt = Session["InformatiaCurenta"] as DataTable;
        //        //DataTable dtDef = General.IncarcaDT(@"SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @1 AND COLUMN_DEFAULT IS NOT NULL ", new object[] { Session["Sablon_Tabela"] });
        //        DataTable dtDef = General.IncarcaDT(strSql, null);
        //        object[] row = new object[dt.Columns.Count];
        //        int x = 0;
        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (!col.AutoIncrement)
        //            {
        //                if (grDate.Columns[col.ColumnName] == null || grDate.Columns[col.ColumnName].Visible == false)
        //                {
        //                    ////punem valoare default daca are
        //                    //DataRow[] lst = dtDef.Select("COLUMN_NAME='" + col.ColumnName + "'");
        //                    //if (lst.Count() > 0 && General.Nz(lst[0]["COLUMN_DEFAULT"], "").ToString() != "")
        //                    //{
        //                    //    if (lst[0]["COLUMN_DEFAULT"].ToString().Replace("(", "").Replace(")", "").Replace("N'", "").ToUpper() == "GETDATE")
        //                    //        row[x] = DateTime.Now;
        //                    //    else
        //                    //        row[x] = lst[0]["COLUMN_DEFAULT"].ToString().Replace("(", "").Replace(")", "").Replace("N'", "");
        //                    //}

        //                    switch (col.ColumnName.ToUpper())
        //                    {
        //                        case "IDAUTO":
        //                            ////row[x] = dt.AsEnumerable().Max(p => p.Field<int>("IdAuto")) + 1;
        //                            //row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
        //                            row[x] = Dami.NextId(Session["Sablon_Tabela"].ToString());
        //                            break;
        //                        case "USER_NO":
        //                            row[x] = Session["UserId"];
        //                            break;
        //                        case "TIME":
        //                            row[x] = DateTime.Now;
        //                            break;
        //                        default:
        //                            //suprascriem valoarea default cu id automat daca asa a fost setat
        //                            DataRow dr = General.IncarcaDR(@"SELECT * FROM ""tblNomenConfig"" WHERE ""Tabela"" = @1 AND ""Camp"" = @2 AND COALESCE(""IdAutomat"",0)=1", new object[] { Session["Sablon_Tabela"], col.ColumnName });
        //                            if (General.Nz(dr, "").ToString() != "")
        //                                row[x] = Dami.NextId(Session["Sablon_Tabela"].ToString());
        //                            else
        //                                row[x] = e.NewValues[col.ColumnName];
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    switch (col.ColumnName.ToUpper())
        //                    {
        //                        case "IDAUTO":
        //                            //row[x] = dt.AsEnumerable().Max(p => p.Field<int>("IdAuto")) + 1;
        //                            //row[x] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
        //                            row[x] = Dami.NextId(Session["Sablon_Tabela"].ToString());
        //                            break;
        //                        case "USER_NO":
        //                            row[x] = Session["UserId"];
        //                            break;
        //                        case "TIME":
        //                            row[x] = DateTime.Now;
        //                            break;
        //                        default:
        //                            row[x] = e.NewValues[col.ColumnName];
        //                            break;
        //                    }
        //                }
        //            }

        //            x++;
        //        }

        //        dt.Rows.Add(row);
        //        e.Cancel = true;
        //        grDate.CancelEdit();
        //        Session["InformatiaCurenta"] = dt;
        //        grDate.DataSource = dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        msgError = ex.Message;
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        //{
        //    try
        //    {
        //        object[] keys = new object[e.Keys.Count];
        //        for (int i = 0; i < e.Keys.Count; i++)
        //        { keys[i] = e.Keys[i]; }

        //        DataTable dt = Session["InformatiaCurenta"] as DataTable;
        //        DataRow row = dt.Rows.Find(keys);

        //        foreach (DataColumn col in dt.Columns)
        //        {
        //            if (!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0))
        //            {
        //                var edc = e.NewValues[col.ColumnName];
        //                row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
        //            }

        //        }

        //        e.Cancel = true;
        //        grDate.CancelEdit();
        //        Session["InformatiaCurenta"] = dt;
        //        grDate.DataSource = dt;

        //    }
        //    catch (Exception ex)
        //    {
        //        msgError = ex.Message;
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                string strSql = "SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND COLUMN_DEFAULT IS NOT NULL";
                if (Constante.tipBD == 2)
                    strSql = "SELECT COLUMN_NAME, DATA_DEFAULT AS COLUMN_DEFAULT FROM COLS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND DATA_DEFAULT IS NOT NULL";

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataTable dtDef = General.IncarcaDT(strSql, null);
                object[] row = new object[dt.Columns.Count];
                int x = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "IDAUTO":
                                row[x] = Dami.NextId(Session["Sablon_Tabela"].ToString());
                                break;
                            case "USER_NO":
                                row[x] = Session["UserId"];
                                break;
                            case "TIME":
                                row[x] = DateTime.Now;
                                break;
                            default:
                                {
                                    //Florin 2019.09.04
                                    //Punem valoare default (numele tabelei) pt tabelele din WizSalary in campul 01, De exemplu F02601 = 26
                                    try
                                    {
                                        string numeTabela = General.Nz(Session["Sablon_Tabela"], "").ToString();
                                        if (col.ColumnName.ToUpper() == numeTabela + "01" && numeTabela != "" && numeTabela.Substring(0, 1).ToUpper() == "F" && General.IsNumeric(numeTabela.Substring(1)) && dt.Columns[numeTabela + "01"] != null)
                                            row[x] = Convert.ToInt32(numeTabela.Substring(1));
                                        else
                                        {
                                            //suprascriem valoarea default cu id automat daca asa a fost setat
                                            if (grDate.Columns[col.ColumnName] == null || grDate.Columns[col.ColumnName].Visible == false)
                                            {
                                                DataRow dr = General.IncarcaDR(@"SELECT * FROM ""tblNomenConfig"" WHERE ""Tabela"" = @1 AND ""Camp"" = @2 AND COALESCE(""IdAutomat"",0)=1", new object[] { Session["Sablon_Tabela"], col.ColumnName });
                                                if (General.Nz(dr, "").ToString() != "")
                                                    row[x] = Dami.NextId(Session["Sablon_Tabela"].ToString());
                                                else
                                                    row[x] = e.NewValues[col.ColumnName];
                                            }
                                            else
                                                row[x] = e.NewValues[col.ColumnName];
                                        }
                                    }
                                    catch (Exception) { }
                                }
                            break;
                        }
                    }

                    x++;
                }

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                object[] keys = new object[e.Keys.Count];
                for (int i = 0; i < e.Keys.Count; i++)
                { keys[i] = e.Keys[i]; }

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow row = dt.Rows.Find(keys);

                foreach (DataColumn col in dt.Columns)
                {
                    if (!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0))
                    {
                        //LeonardM 14.01.2018
                        //modificare pentru salvare col autoincrement ok
                        DataRow dr = General.IncarcaDR(@"SELECT * FROM ""tblNomenConfig"" WHERE ""Tabela"" = @1 AND ""Camp"" = @2 AND COALESCE(""IdAutomat"",0)=1", new object[] { Session["Sablon_Tabela"], col.ColumnName });
                        if (General.Nz(dr, "").ToString() == "")
                        {
                            var edc = e.NewValues[col.ColumnName];
                            row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                        }
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;

            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;

                string strSql = "SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND COLUMN_DEFAULT IS NOT NULL";
                if (Constante.tipBD == 2)
                    strSql = "SELECT COLUMN_NAME, DATA_DEFAULT AS COLUMN_DEFAULT FROM COLS WHERE TABLE_NAME = '" + Session["Sablon_Tabela"] + "' AND DATA_DEFAULT IS NOT NULL";

                DataTable dtDef = General.IncarcaDT(strSql, null);
                //DataTable dtDef = General.IncarcaDT(@"SELECT COLUMN_NAME, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @1 AND COLUMN_DEFAULT IS NOT NULL ", new object[] { Session["Sablon_Tabela"] });
                for(int i =0; i < dtDef.Rows.Count; i++)
                {
                    string colNume = dtDef.Rows[i]["COLUMN_NAME"].ToString();
                    if (grDate.Columns[colNume] != null && grDate.Columns[colNume].Visible == true)
                    {
                        //punem valoare default daca are
                        DataRow[] lst = dtDef.Select("COLUMN_NAME='" + colNume + "'");
                        if (lst.Count() > 0 && General.Nz(lst[0]["COLUMN_DEFAULT"], "").ToString() != "")
                        {
                            if (lst[0]["COLUMN_DEFAULT"].ToString().Replace("(", "").Replace(")", "").Replace("N'", "").ToUpper() == "GETDATE")
                                e.NewValues[colNume] = DateTime.Now;
                            else
                                e.NewValues[colNume] = lst[0]["COLUMN_DEFAULT"].ToString().Replace("(", "").Replace(")", "").Replace("N'", "");
                        }
                    }
                }


                //id automat daca asa a fost setat
                DataRow dr = General.IncarcaDR(@"SELECT * FROM ""tblNomenConfig"" WHERE ""Tabela"" = @1 AND COALESCE(""IdAutomat"",0)=1", new object[] { Session["Sablon_Tabela"] });
                if (General.Nz(dr, "").ToString() != "")
                {
                    int ert = Dami.NextId(Session["Sablon_Tabela"].ToString());
                    if (ert == -99)
                        e.NewValues[dr["Camp"]] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>(dr["Camp"].ToString())), 0)) + 1;
                    else
                        e.NewValues[dr["Camp"]] = Dami.NextId(Session["Sablon_Tabela"].ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM " + Session["Sablon_Tabela"], null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, Session["Sablon_Tabela"].ToString());

                #region Generare SetAngajatiDetail
                /*LeonardM 11.11.2017
                 * generare seturi angajati detail*/
                if (dt.TableName == "Eval_SetAngajati")
                {
                    string sqlDelete, sqlDeleteTemp = string.Empty;
                    string sqlInsert, sqlInsertTemp = string.Empty;

                    //Radu 24.04.2018 - se transforma tabela Eval_SetAngajatiDetail in view, deoarece altfel nu sunt actualizate automat informatiile despre angajati
                    //sqlDelete = @"delete from ""Eval_SetAngajatiDetail"" where ""IdSetAng"" = {0}";
                    //sqlInsert = @"insert into ""Eval_SetAngajatiDetail""(""IdSetAng"", ""Id"", ""Cod"", ""Denumire"")
                    //              select ang.""IdSetAng"", cte.""Id"", cte.""Cod"", cte.""Denumire""
                    //              from ""Eval_SetAngajati"" ang
                    //              join ({0}) cte on 1 = 1
                    //              where ang.""IdSetAng"" = {1}";

                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    Eval_SetAngajati clsSetAngajati = new Eval_SetAngajati(dr);
                    //    sqlDeleteTemp = sqlDelete;
                    //    sqlDeleteTemp = string.Format(sqlDeleteTemp, clsSetAngajati.IdSetAng);
                    //    General.ExecutaNonQuery(sqlDeleteTemp, null);

                    //    sqlInsertTemp = sqlInsert;
                    //    sqlInsertTemp = string.Format(sqlInsertTemp, clsSetAngajati.SelectQuery, clsSetAngajati.IdSetAng);
                    //    General.ExecutaNonQuery(sqlInsertTemp, null);
                    //}

                    string sqltmp = "";
                    if (Constante.tipBD == 2)
                        sqltmp = "SELECT COUNT(*) CNT FROM user_tables WHERE TABLE_NAME = 'Eval_SetAngajatiDetail'";
                    else
                    {
                        sqltmp = "SELECT COUNT(*) AS CNT, max(table_type) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME  = 'Eval_SetAngajatiDetail'";
                    }
                    DataTable dtTemp = General.IncarcaDT(sqltmp, null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0 && dtTemp.Rows[0][0].ToString() == "1")
                    {
                        if (Constante.tipBD == 2)
                            sqlDelete = "DROP TABLE \"Eval_SetAngajatiDetail\"";
                        else
                            sqlDelete = "DROP " + (dtTemp.Rows[0][1].ToString().ToUpper().Contains("TABLE") ? "TABLE" : "VIEW") + " \"Eval_SetAngajatiDetail\"";

                        General.ExecutaNonQuery(sqlDelete, null);
                    }
                    if (Constante.tipBD == 1)
                        sqlInsert = "CREATE VIEW Eval_SetAngajatiDetail AS ";
                    else
                        sqlInsert = "CREATE OR REPLACE VIEW \"Eval_SetAngajatiDetail\"(\"IdSetAng\", \"Id\", \"Cod\", \"Denumire\") AS ";

                    sqlInsertTemp = "";
                    int i = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        Eval_SetAngajati clsSetAngajati = new Eval_SetAngajati(dr);
                        sqlInsertTemp += clsSetAngajati.SelectQuery.ToUpper().Replace("SELECT ", "SELECT " + clsSetAngajati.IdSetAng + " AS \"IdSetAng\", ");
                        i++;
                        if (i < dt.Rows.Count)
                            sqlInsertTemp += " UNION ";
                    }
                    sqlInsert += sqlInsertTemp;
                    bool valid = General.ExecutaNonQuery(sqlInsert, null);
                    if (!valid)
                    {
                        MessageBox.Show("Eroare la salvare!", MessageBox.icoSuccess);
                        return;
                    }


                }
                /*end LeonardM 11.11.2017*/
                #endregion
                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnRenunta_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                dt.RejectChanges();
                
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void upPanel_Unload(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        RegisterUpdatePanel((UpdatePanel)sender);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void RegisterUpdatePanel(UpdatePanel panel)
        //{
        //    try
        //    {
        //        var sType = typeof(ScriptManager);
        //        var mInfo = sType.GetMethod("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel", BindingFlags.NonPublic | BindingFlags.Instance);
        //        if (mInfo != null)
        //            mInfo.Invoke(ScriptManager.GetCurrent(Page), new object[] { panel });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void cmbExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                gridExport.WritePdfToResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbExport_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                gridExport.WritePdfToResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbExport_ButtonClick(object source, ButtonEditClickEventArgs e)
        {
            try
            {
                gridExport.WritePdfToResponse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void popSec_WindowCallback(object source, PopupWindowCallbackArgs e)
        {
            try
            {
                string[] data = e.Parameter.Split('|');
                int idx = Convert.ToInt32(data[1]);
                Session["SecuritateCamp"] = grDate.Columns[idx].Name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomErrorText(object sender, ASPxGridViewCustomErrorTextEventArgs e)
        {
            try
            {
                e.ErrorText = msgError;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
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