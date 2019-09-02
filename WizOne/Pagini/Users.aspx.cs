﻿using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using ProceseSec;

namespace WizOne.Pagini
{
    public partial class Users : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                string sqlLimba = @"SELECT ""Id"", ""Denumire"" FROM ""tblLimbi"" ORDER BY ""Denumire"" ";
                string sqlAng = @"SELECT F10003 AS ""Marca"", F10008 + ' ' + F10009 AS ""NumeComplet"" FROM ""F100"" ";

                if (Constante.tipBD == 2)
                {
                    sqlAng = @"SELECT F10003 AS ""Marca"", F10008 & ' ' & F10009 AS ""NumeComplet"" FROM ""F100"" ";
                }

                DataTable dtLmb = General.IncarcaDT(sqlLimba, null);
                GridViewDataComboBoxColumn colLmb = (grDate.Columns["IdLimba"] as GridViewDataComboBoxColumn);
                colLmb.PropertiesComboBox.DataSource = dtLmb;

                DataTable dtCmbAng = General.IncarcaDT(sqlAng, null);
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtCmbAng;

                if (!IsPostBack)
                {
                    //Radu 08.05.2019 - sa se afiseze doar utilizatorii de WizOne
                    string cond = "";
                    //if (Constante.tipBD == 2)
                    //    cond = "case when (SELECT COUNT(*) CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from \"relGrupUser\" where \"IdUser\" = F70102) > 0 then 0  else  1 end ELSE  1   END AS TIP ";
                    //else
                    //    cond = "case when (SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from relGrupUser where IdUser = F70102) > 0 then 0  else 1  end  ELSE 1   END  AS TIP ";
                    if (Constante.tipBD == 2)
                        cond = "case when (SELECT COUNT(*) AS CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('COMPACC')) = 1 THEN  case when (select Count(*) from compacc where F70203 = F70102) > 0 or f70102 = 1 then 1 else 0  end  ELSE 0   END  AS TIP  ";
                    else
                        cond = "case when (SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('COMPACC')) = 1 THEN  case when (select Count(*) from compacc where F70203 = F70102) > 0 or f70102 = 1 then 1 else 0  end  ELSE 0   END  AS TIP  ";


                    DataTable dt = General.IncarcaDT(@"SELECT * FROM (SELECT A.*, " + cond + " FROM USERS A) B WHERE TIP = 0 ", null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["F70102"] };

                    Session["InformatiaCurenta"] = dt;

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "F70102";
                    grDate.DataBind();
                }
                else
                {
                    foreach (var c in grDate.Columns)
                    {
                        try
                        {
                            GridViewDataColumn col = (GridViewDataColumn)c;
                            col.Caption = Dami.TraduCuvant(col.FieldName, General.Nz(col.Caption,"").ToString());
                        }
                        catch (Exception) { }
                    }

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }

                grDate.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaCO", "10"));
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

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                string pwd = "";

                if (e.NewValues["F70103"] != null)
                {
                    CriptDecript prc = new CriptDecript();
                    pwd = prc.EncryptString(Constante.cheieCriptare, e.NewValues["F70103"].ToString(), 1);
                }
                else
                {
                    e.Cancel = true;
                }

                bool err = false;

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.NewRow();

                //Radu 19.08.2019 - verificare sa nu existe alta inregistrare cu marca nou introdusa
                if (e.NewValues["F10003"] != null)
                {
                    DataRow[] rows = dt.Select("F10003 = " + e.NewValues["F10003"].ToString());
                    if (rows != null && rows.Count() > 0)
                    {
                        grDate.JSProperties["cpAlertMessage"] = "Marca selectata este deja alocata altui utilizator!";
                        err = true;
                    }
                }
                if (!err)
                {
                    dr["F70101"] = e.NewValues["F70101"] ?? 701;
                    dr["F70102"] = e.NewValues["F70102"] ?? DBNull.Value;
                    dr["F70103"] = pwd;
                    dr["F70104"] = e.NewValues["F70104"] ?? DBNull.Value;
                    dr["F70105"] = e.NewValues["F70105"] ?? DBNull.Value;
                    dr["F70111"] = e.NewValues["F70111"] ?? 0;
                    dr["F70112"] = e.NewValues["F70112"] ?? 0;
                    dr["F70113"] = e.NewValues["F70113"] ?? 0;
                    dr["F70114"] = e.NewValues["F70114"] ?? 0;
                    dr["F70121"] = e.NewValues["F70121"] ?? 999;
                    dr["F70122"] = e.NewValues["F70122"] ?? DateTime.Now;
                    dr["F70123"] = e.NewValues["F70123"] ?? DBNull.Value;

                    dr["IdLimba"] = e.NewValues["IdLimba"] ?? "RO";
                    dr["F10003"] = e.NewValues["F10003"] ?? DBNull.Value;
                    dr["NumeComplet"] = e.NewValues["NumeComplet"] ?? DBNull.Value;
                    dr["Mail"] = e.NewValues["Mail"] ?? DBNull.Value;
                    dr["SchimbaParola"] = e.NewValues["SchimbaParola"] ?? 0;
                    dr["Parola"] = e.NewValues["Parola"] ?? DBNull.Value;
                    dr["PINInfoChiosc"] = e.NewValues["PINInfoChiosc"] ?? DBNull.Value;

                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
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

                bool err = false;

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                //Radu 19.08.2019 - verificare sa nu existe alta inregistrare cu marca nou introdusa           
                if (e.NewValues["F10003"] != null)
                {
                    DataRow[] rows = dt.Select("F10003 = " + e.NewValues["F10003"].ToString() + " AND F70102 <> " + e.NewValues["F70102"].ToString());
                    if (rows != null && rows.Count() > 0)
                    {
                        grDate.JSProperties["cpAlertMessage"] = "Marca selectata este deja alocata altui utilizator!";
                        err = true;
                    }
                }

                //dr["F70103"] = e.NewValues["F70103"];
                if (!err)
                {
                    dr["F70104"] = e.NewValues["F70104"] ?? DBNull.Value;
                    dr["F70105"] = e.NewValues["F70105"] ?? DBNull.Value;
                    dr["F70111"] = e.NewValues["F70111"] ?? 0;
                    dr["F70112"] = e.NewValues["F70112"] ?? 0;
                    dr["F70113"] = e.NewValues["F70113"] ?? 0;
                    dr["F70114"] = e.NewValues["F70114"] ?? 0;
                    dr["F70121"] = e.NewValues["F70121"] ?? 999;
                    dr["F70122"] = e.NewValues["F70122"] ?? DateTime.Now;
                    dr["F70123"] = e.NewValues["F70123"] ?? DBNull.Value;

                    dr["IdLimba"] = e.NewValues["IdLimba"] ?? "RO";
                    dr["F10003"] = e.NewValues["F10003"] ?? DBNull.Value;
                    dr["NumeComplet"] = e.NewValues["NumeComplet"] ?? DBNull.Value;
                    dr["Mail"] = e.NewValues["Mail"] ?? DBNull.Value;
                    dr["SchimbaParola"] = e.NewValues["SchimbaParola"] ?? 0;
                    dr["Parola"] = e.NewValues["Parola"] ?? DBNull.Value;
                    dr["PINInfoChiosc"] = e.NewValues["PINInfoChiosc"] ?? DBNull.Value;

                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                }
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["F70102"] = Dami.NextId("Users");
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
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""USERS"" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, "USERS");

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

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void confirmButton_Click(object sender, EventArgs e)
        {
            try
            {

                CriptDecript prc = new CriptDecript();
                string pwd = prc.EncryptString(Constante.cheieCriptare, cnpsw.Text, 1);

                int index = grDate.EditingRowVisibleIndex;
                string kp = grDate.GetRowValues(index, "F70102").ToString();

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(kp);

                if (General.VarSession("ParolaComplexa").ToString() == "1")
                {
                    var ras = General.VerificaComplexitateParola(cnpsw.Text);
                    if (ras != "")
                    {
                        MessageBox.Show(ras, MessageBox.icoWarning, "Parola invalida");
                        return;
                    }
                }

                General.AddUserIstoric();

                dr["F70103"] = pwd;
                Session["InformatiaCurenta"] = dt;
                ASPxPopupControl1.ShowOnPageLoad = false;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void UpdatePasswordField(string newPassword)
        //{
        //    try
        //    {
        //        int index = grDate.EditingRowVisibleIndex;
        //        DataTable dt = Session["InformatiaCurenta"] as DataTable;
        //        dt.Rows[index]["F70103"] = newPassword;
        //        Session["InformatiaCurenta"] = dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


    }
}