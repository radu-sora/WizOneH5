using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Absente
{
    public partial class SetariActiuni : System.Web.UI.Page
    {
        string msgError = string.Empty;

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

                lblViz.InnerText = Dami.TraduCuvant("Actiune");
                lblRol.InnerText = Dami.TraduCuvant("Stare");
                btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                GridViewDataComboBoxColumn colVal = (grDate.Columns["Valoare"] as GridViewDataComboBoxColumn);
                foreach (ListEditItem item in colVal.PropertiesComboBox.Items)
                {
                    item.Text = Dami.TraduCuvant(item.Text, item.Text);
                }

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                DataTable dtAbs = General.IncarcaDT(General.SqlCuSelectieToate(@"SELECT ""Id"", ""Denumire"", 1 AS ""Ordin"" FROM ""Ptj_tblAbsente"" "), null);
                GridViewDataComboBoxColumn colAbs = (grDate.Columns["IdAbs"] as GridViewDataComboBoxColumn);
                colAbs.PropertiesComboBox.DataSource = dtAbs;

                string sqlRol = @"SELECT TOP 100 ""Id"", CASE WHEN ""Alias"" IS NOT NULL AND ""Alias"" <> '' THEN ""Alias"" ELSE ""Denumire"" END AS ""Denumire"", 1 AS ""Ordin"" FROM ""tblSupervizori"" ORDER BY ""Denumire"" ";
                if (Constante.tipBD == 2)
                    sqlRol = @"SELECT ""Id"", CASE WHEN ""Alias"" IS NOT NULL AND ""Alias"" <> '' THEN ""Alias"" ELSE ""Denumire"" END AS ""Denumire"", 1 AS ""Ordin"" FROM ""tblSupervizori"" WHERE ROWNUM <=100 ORDER BY ""Denumire"" ";


                DataTable dtRol = General.IncarcaDT(General.SqlCuSelectieToate(sqlRol), null);
                GridViewDataComboBoxColumn colRol = (grDate.Columns["IdRol"] as GridViewDataComboBoxColumn);
                colRol.PropertiesComboBox.DataSource = dtRol;

                if (!IsPostBack)
                {
                    DataTable dtStr = General.IncarcaDT(General.SqlCuSelectieToate(@"SELECT ""Id"", ""Denumire"", 1 AS ""Ordin"" FROM ""Ptj_tblStari"" "), null);
                    cmbStr.DataSource = dtStr;
                    cmbStr.DataBindItems();
                    cmbStr.SelectedIndex = 0;
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

                dr["IdAbs"] = e.NewValues["IdAbs"];
                dr["IdActiune"] = Convert.ToInt32(cmbAct.Value ?? -1);
                dr["IdStare"] = Convert.ToInt32(cmbStr.Value ?? -1);
                dr["IdRol"] = e.NewValues["IdRol"];
                dr["Valoare"] = e.NewValues["Valoare"] ?? DBNull.Value;
                dr["NrZile"] = e.NewValues["NrZile"] ?? DBNull.Value;
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
                DataRow dr = dt.Rows.Find(keys);

                dr["Valoare"] = e.NewValues["Valoare"];
                dr["NrZile"] = e.NewValues["NrZile"];
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

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
                e.NewValues["IdActiune"] = Convert.ToInt32(cmbAct.Value ?? -1);
                e.NewValues["IdStare"] = Convert.ToInt32(cmbStr.Value ?? -1);
                e.NewValues["NrZile"] = 0;
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
                //if (Constante.tipBD == 1)
                //{                    
                //    SqlDataAdapter da = new SqlDataAdapter();
                //    da.SelectCommand = General.DamiSqlCommand(@"SELECT TOP 0 * FROM ""Ptj_CereriDrepturi"" ", null);
                //    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //    da.Update(dt);

                //    da.Dispose();
                //    da = null;
                //}
                //else
                //{
                //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Ptj_CereriDrepturi\" WHERE ROWNUM = 0", null);
                //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                //    oledbAdapter.Update(dt);
                //    oledbAdapter.Dispose();
                //    oledbAdapter = null;
                //}
                General.SalveazaDate(dt, "Ptj_CereriDrepturi");

                IncarcaGrid();

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

        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            try
            {
                string filtru = "";
                if (cmbStr.SelectedIndex != -1) filtru = @" AND ""IdStare""=" + cmbStr.Value;

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Ptj_CereriDrepturi"" WHERE ""IdActiune""={cmbAct.Value ?? 1} " + filtru, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["IdAbs"], dt.Columns["IdActiune"], dt.Columns["IdRol"], dt.Columns["IdStare"] };

                Session["InformatiaCurenta"] = dt;

                grDate.DataSource = Session["InformatiaCurenta"];
                grDate.KeyFieldName = "IdAbs; IdActiune; IdRol; IdStare";
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IdAbs" || e.Column.FieldName == "IdRol") e.Editor.ReadOnly = false;
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
    }
}