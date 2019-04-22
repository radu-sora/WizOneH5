using DevExpress.Web;
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

namespace WizOne.Pagini
{
    public partial class Delegari : System.Web.UI.Page
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

                string idHR = Dami.ValoareParam("Cereri_IDuriRoluriHR", "-99");
                string sqlHr = $@"SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE ""IdUser""={Session["UserId"]} AND ""IdSuper"" IN ({idHR}) GROUP BY ""IdUser"" ";
                DataTable dtHr = General.IncarcaDT(sqlHr, null);
                bool esteHr = false;
                if (dtHr != null && dtHr.Rows.Count > 0) esteHr = true;

                string sqlUsr = @"SELECT F70102, F70104 FROM USERS";
                DataTable dtDg = General.IncarcaDT(sqlUsr, null);
                GridViewDataComboBoxColumn colDg = (grDate.Columns["IdDelegat"] as GridViewDataComboBoxColumn);
                colDg.PropertiesComboBox.DataSource = dtDg;

                string sqlMod = @"SELECT ""Id"", ""Denumire"" FROM ""tblModule"" ";
                DataTable dtMo = General.IncarcaDT(sqlMod, null);
                GridViewDataComboBoxColumn colMo = (grDate.Columns["IdModul"] as GridViewDataComboBoxColumn);
                colMo.PropertiesComboBox.DataSource = dtMo;

                if (!esteHr)
                    grDate.Columns["IdUser"].Visible = false;
                else
                {
                    //sqlUsr += " WHERE F70102=" + Session["UserId"];
                    grDate.Columns["IdUser"].Visible = true;

                    DataTable dtUsr = General.IncarcaDT(sqlUsr, null);
                    GridViewDataComboBoxColumn colUsr = (grDate.Columns["IdUser"] as GridViewDataComboBoxColumn);
                    colUsr.PropertiesComboBox.DataSource = dtUsr;
                }

                if (!IsPostBack)
                {
                    string sqlTbl = @"SELECT * FROM ""tblDelegari"" ";
                    if (!esteHr) { sqlTbl += " WHERE \"IdUser\"=" + Session["UserId"]; }
                    DataTable dt = General.IncarcaDT(sqlTbl, null);
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["IdUser"], dt.Columns["IdDelegat"], dt.Columns["IdModul"] };

                    Session["InformatiaCurenta"] = dt;

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "IdUser;IdDelegat;IdModul";
                    grDate.DataBind();
                }
                else
                {
                    foreach (var c in grDate.Columns)
                    {
                        try
                        {
                            GridViewDataColumn col = (GridViewDataColumn)c;
                            col.Caption = Dami.TraduCuvant(col.FieldName);
                        }
                        catch (Exception) { }
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

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.NewRow();

                if (grDate.Columns["IdUser"].Visible == false)
                    dr["IdUser"] = Session["UserId"];
                else
                    dr["IdUser"] = e.NewValues["IdUser"];

                dr["IdDelegat"] = e.NewValues["IdDelegat"];
                dr["IdModul"] = e.NewValues["IdModul"];
                dr["DataInceput"] = e.NewValues["DataInceput"];
                dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                dt.Rows.Add(dr);
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

                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow dr = dt.Rows.Find(keys);

                if (e.NewValues["IdUser"] != null) dr["IdUser"] = e.NewValues["IdUser"];
                dr["IdDelegat"] = e.NewValues["IdDelegat"];
                dr["IdModul"] = e.NewValues["IdModul"];
                dr["DataInceput"] = e.NewValues["DataInceput"];
                dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

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
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                e.NewValues["IdModul"] = 1;
                e.NewValues["DataInceput"] = DateTime.Now;
                e.NewValues["DataSfarsit"] = new DateTime(2100,1,1);
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
                //da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""tblDelegari"" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;
                General.SalveazaDate(dt, "tblDelegari");

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






    }
}