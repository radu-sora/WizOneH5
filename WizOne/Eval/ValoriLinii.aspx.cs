using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class ValoriLinii : System.Web.UI.Page
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

                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                if (!IsPostBack)
                {
                    DataSet ds = new DataSet();
                    string gridKey = "";
                    string sqlQuery = string.Empty;
                    sqlQuery = "select * from \"Eval_tblTipValoriLinii\" where \"Id\" = {0}";
                    sqlQuery = string.Format(sqlQuery, id);
                    DataTable dt = General.IncarcaDT(sqlQuery, null);

                    DataColumn[] keys = dt.PrimaryKey;
                    for(int i=0;i<keys.Count(); i++)
                    {
                        gridKey += ";" + keys[i].ToString();
                    }

                    Session["InformatiaCurenta"] = dt;

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "IdAuto";
                    if (gridKey != "") grDate.KeyFieldName = gridKey.Substring(1);
                    grDate.DataBind();

                    sqlQuery = @"select * from ""Eval_tblTipValori"" where ""Id"" = {0}";
                    sqlQuery = string.Format(sqlQuery, id);
                    DataTable dtValoare = General.IncarcaDT(sqlQuery, null);

                    if (dtValoare != null && dtValoare.Rows.Count > 0 && dtValoare.Rows[0][0] != null)
                    {
                        txtId.Text = dtValoare.Rows[0]["Id"].ToString();
                        txtValoare.Text = dtValoare.Rows[0]["Denumire"].ToString();
                    }
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
                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_tblTipValori";
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
                //string sqlQuery = string.Empty;
                //int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                //DataTable dt = Session["InformatiaCurenta"] as DataTable;

                //#region salvare Eval_ObiectivXActivitate
                //SqlDataAdapter da = new SqlDataAdapter();
                //da.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_tblTipValoriLinii\" ", null);
                //SqlCommandBuilder cb = new SqlCommandBuilder(da);
                //da.Update(dt);

                //da.Dispose();
                //da = null;

                //HttpContext.Current.Session["Sablon_Tabela"] = "Eval_tblTipValori";
                //Response.Redirect("~/Pagini/SablonLista.aspx", false);
                //#endregion


                string sqlQuery = string.Empty;
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                sqlQuery = "select * from \"Eval_tblTipValori\" where \"Id\" = {0} ";
                sqlQuery = string.Format(sqlQuery, id);
                DataTable dtHead = General.IncarcaDT(sqlQuery, null);

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Dami.NextId("Eval_tblTipValori");
                            DataRow drHead = dtHead.NewRow();
                            drHead["Id"] = id;
                            drHead["Denumire"] = txtValoare.Text;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            int nrInreg = dt.Rows.Count;
                            int Id = Dami.NextId("Eval_tblTipValoriLinii", nrInreg);
                            foreach (DataRow dr in dt.Rows)
                            {
                                dr["Id"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        dtHead.Rows[0]["Denumire"] = txtValoare.Text;
                        foreach (DataRow dr in dt.Rows)
                        {
                            dr["Id"] = id;
                        }
                        break;
                }

                #region salvare Eval_tblTipValori
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter daHead = new SqlDataAdapter();
                    daHead.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_tblTipValori\" ", null);
                    SqlCommandBuilder cbHead = new SqlCommandBuilder(daHead);
                    daHead.Update(dtHead);
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_tblTipValori\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cbbHead = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dtHead);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }
                #endregion

                #region salvare Eval_tblTipValoriLinii
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_tblTipValoriLinii\" ", null);
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Update(dt);

                    da.Dispose();
                    da = null;
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_tblTipValoriLinii\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dt);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }
                #endregion

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_tblTipValori";
                Response.Redirect("~/Pagini/SablonLista.aspx", false);



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
                row["Id"] = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                row["Valoare"] = e.NewValues["Valoare"];
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
                    if ((!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0)) && colPrimaryKey.Where(p => p.ColumnName == col.ColumnName).Count() == 0)
                    {
                        var edc = e.NewValues[col.ColumnName];
                        row[col.ColumnName] = e.NewValues[col.ColumnName] ?? DBNull.Value;
                    }
                    if (col.ColumnName == "Id")
                        row[col.ColumnName] = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
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
                e.NewValues["Id"] = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
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