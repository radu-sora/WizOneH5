using DevExpress.Web;
using Oracle.ManagedDataAccess.Client;
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

namespace WizOne.Eval
{
    public partial class ActivitatiObiective : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnNew.Text = Dami.TraduCuvant("btnNew", "Nou");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                grDate.SettingsCommandButton.UpdateButton.Text = Dami.TraduCuvant("btnUpdate", "Actualizeaza");
                grDate.SettingsCommandButton.CancelButton.Text = Dami.TraduCuvant("btnCancel", "Renunta");
                #endregion

                int IdObiectiv = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                if (!IsPostBack)
                {
                    DataSet ds = new DataSet();
                    string gridKey = "";
                    string sqlQuery = string.Empty;
                    sqlQuery = "select * from \"Eval_ObiectivXActivitate\" where \"IdObiectiv\" = {0}";
                    sqlQuery = string.Format(sqlQuery, IdObiectiv);
                    DataTable dt = General.IncarcaDT(sqlQuery, null);

                    DataColumn[] keys = dt.PrimaryKey;
                    for(int i=0;i<keys.Count(); i++)
                    {
                        gridKey += ";" + keys[i].ToString();
                    }

                    Session["InformatiaCurenta"] = dt;

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "IdActivitate";
                    if (gridKey != "") grDate.KeyFieldName = gridKey.Substring(1);
                    grDate.DataBind();

                    sqlQuery = @"select * from ""Eval_Obiectiv"" where ""IdObiectiv"" = {0}";
                    sqlQuery = string.Format(sqlQuery, IdObiectiv);
                    DataTable dtObiectiv = General.IncarcaDT(sqlQuery, null);

                    txtId.Text = dtObiectiv.Rows[0]["IdObiectiv"].ToString();
                    txtObiectiv.Text = dtObiectiv.Rows[0]["Obiectiv"].ToString();
                }
                else
                {
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_Obiectiv";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
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
                string sqlQuery = string.Empty;
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                sqlQuery = "select * from \"Eval_Obiectiv\" where \"IdObiectiv\" = {0}";
                sqlQuery = string.Format(sqlQuery, id);
                DataTable dtHead = General.IncarcaDT(sqlQuery, null);

                switch(Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Dami.NextId("EvalObiectiv");
                            DataRow drHead = dtHead.NewRow();
                            drHead["IdObiectiv"] = id;
                            drHead["Obiectiv"] = txtObiectiv.Text;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            int nrInreg = dt.Rows.Count;
                            int IdActivitate = Dami.NextId("Eval_ObiectivXActivitate", nrInreg);
                            foreach(DataRow dr in dt.Rows)
                            {
                                dr["IdObiectiv"] = id;
                                dr["IdActivitate"] = IdActivitate - Convert.ToInt32(General.Nz(dr["IdActivitate"], 0));
                            }

                        }
                        break;
                    case "Edit":
                        dtHead.Rows[0]["Obiectiv"] = txtObiectiv.Text;
                        dtHead.Rows[0]["IdObiectiv"] = txtId.Text;
                        break;
                }

                #region salvare Eval_Obiectiv
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter daHead = new SqlDataAdapter();
                    daHead.SelectCommand = General.DamiSqlCommand(@"select top 0 * from ""Eval_Obiectiv"" ", null);
                    SqlCommandBuilder cbHead = new SqlCommandBuilder(daHead);
                    daHead.Update(dtHead);
                    daHead.Dispose();
                    daHead = null;
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_Obiectiv\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cbbHead = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dtHead);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }
                #endregion

                #region salvare Eval_ObiectivXActivitate
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_ObiectivXActivitate\" ", null);
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Update(dt);

                    da.Dispose();
                    da = null;
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_ObiectivXActivitate\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dt);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_Obiectiv";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);
                #endregion
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
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
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

        protected void grDate_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataRow row = dt.NewRow();
                int x = dt.Rows.Count;
                row["IdActivitate"] = Dami.NextId("Eval_ObiectivXActivitate");
                row["Activitate"] = e.NewValues["Activitate"];
                row["IdObiectiv"] = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                row["TIME"] = DateTime.Now;
                row["USER_NO"] = Session["UserId"];

                dt.Rows.Add(row);
                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, MessageBox.icoError, "");
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
                DataColumn[] colPrimaryKey = dt.PrimaryKey;
                foreach (DataColumn col in dt.Columns)
                {
                    if ((!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0)) && colPrimaryKey.Where(p => p.ColumnName == col.ColumnName).Count() == 0 && col.ColumnName != "IdObiectiv")
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }

                }

                e.Cancel = true;
                grDate.CancelEdit();
                Session["InformatiaCurenta"] = dt;
                grDate.DataSource = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, MessageBox.icoError, "");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["IdObiectiv"] = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                e.NewValues["IdActivitate"] = Dami.NextId("Eval_ObiectivXActivitate");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomErrorText(object sender, DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs e)
        {
            try
            {
                //e.ErrorText = msgError;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        #region Methods

        #endregion
    }
}