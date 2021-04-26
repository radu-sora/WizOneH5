using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Eval
{
    public partial class ListaObiective : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    Session["EvalLista_DSObiective"] = General.IncarcaDT(@"SELECT * FROM ""Eval_Obiectiv"" ");
                    Session["EvalLista_DSObiectiveXActivitate"] = General.IncarcaDT(@"SELECT * FROM ""Eval_ObiectivXActivitate"" ");
                    Session["EvalLista_DSSetAngajat"] = General.IncarcaDT(@"SELECT * FROM ""Eval_SetAngajati"" ");
                }

                GridViewDataComboBoxColumn colObiective = (grDate.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);
                GridViewDataComboBoxColumn colActivitati = (grDate.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                GridViewDataComboBoxColumn colSetAngajati = (grDate.Columns["IdSetAngajat"] as GridViewDataComboBoxColumn);
                colObiective.PropertiesComboBox.DataSource = Session["EvalLista_DSObiective"];
                colActivitati.PropertiesComboBox.DataSource = Session["EvalLista_DSObiectiveXActivitate"];
                colSetAngajati.PropertiesComboBox.DataSource = Session["EvalLista_DSSetAngajat"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

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

                int IdListaObiective = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                if (!IsPostBack)
                {
                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcam header-ul
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_ListaObiectiv"" WHERE ""IdLista""=@1 ", new object[] { IdListaObiective });
                                if (dtHead.Rows.Count > 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["IdLista"].ToString();
                                    txtCodLista.Text = dtHead.Rows[0]["CodLista"].ToString();
                                    txtDenLista.Text = dtHead.Rows[0]["DenLista"].ToString();
                                }
                            }
                            break;
                    }

                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Eval_ListaObiectivDet"" WHERE ""IdLista"" = @1", new object[] { IdListaObiective });
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                    Session["InformatiaCurenta"] = dt;
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "Id";
                    grDate.DataBind();
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
                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_ListaObiectiv";
                Response.Redirect("~/Pagini/SablonLista", false);
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
                int id = Convert.ToInt32(Session["Sablon_CheiePrimara"]);
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_ListaObiectiv"" WHERE ""IdLista"" = @1", new object[] { id });

                switch (Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT MAX(COALESCE(""IdLista"",0)) FROM ""Eval_ListaObiectiv"" "), 0)) + 1;
                            DataRow drHead = dtHead.NewRow();
                            drHead["IdLista"] = id;
                            drHead["CodLista"] = txtCodLista.Text;
                            drHead["DenLista"] = txtDenLista.Text;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            foreach(DataRow dr in dt.Rows)
                            {
                                dr["IdLista"] = id;
                            }
                        }
                        break;
                    case "Edit":
                        dtHead.Rows[0]["CodLista"] = txtCodLista.Text;
                        dtHead.Rows[0]["DenLista"] = txtDenLista.Text;
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr.RowState != DataRowState.Deleted)
                                dr["IdLista"] = id;
                        }
                        break;
                }

                General.SalveazaDate(dtHead, "Eval_ListaObiectiv");
                General.SalveazaDate(dt, "Eval_ListaObiectivDet");

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_ListaObiectiv";
                Response.Redirect("~/Pagini/SablonLista", false);
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

                row["IdObiectiv"] = e.NewValues["IdObiectiv"] ?? DBNull.Value;
                row["IdActivitate"] = e.NewValues["IdActivitate"] ?? DBNull.Value;
                row["IdSetAngajat"] = e.NewValues["IdSetAngajat"] ?? DBNull.Value;
                row["Target"] = e.NewValues["Target"] ?? DBNull.Value;
                row["Ordine"] = e.NewValues["Ordine"] ?? DBNull.Value;
                row["Vizibil"] = e.NewValues["Vizibil"] ?? DBNull.Value;
                row["TIME"] = DateTime.Now;
                row["USER_NO"] = Session["UserId"];

                dt.Rows.Add(row);
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
                DataRow row = dt.Rows.Find(keys);
                DataColumn[] colPrimaryKey = dt.PrimaryKey;
                foreach (DataColumn col in dt.Columns)
                {
                    if ((!col.AutoIncrement && (cmp.IndexOf(col.ColumnName.ToUpper() + ",") < 0)) && colPrimaryKey.Where(p => p.ColumnName == col.ColumnName).Count() == 0)
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
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["Id"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private int GetCurrentObjective()
        {
            object id = null;
            if (hf.TryGet("CurrentObjective", out id))
                return Convert.ToInt32(id);
            return -1;
        }

        protected void grDate_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {
            try
            {
                if (!grDate.IsEditing || e.Column.FieldName != "IdActivitate") return;
                object val = -99;

                if (e.VisibleIndex == -2147483647)
                {
                    val = GetCurrentObjective();
                }
                else
                {
                    if (e.KeyValue == DBNull.Value || e.KeyValue == null) return;
                    val = grDate.GetRowValuesByKeyValue(e.KeyValue, "IdObiectiv");
                    if (val == DBNull.Value) return;
                }

                ASPxComboBox combo = e.Editor as ASPxComboBox;
                IncarcaActivitati(combo, Convert.ToInt32(val));

                combo.Callback += new CallbackEventHandlerBase(cmbAct_OnCallback);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void IncarcaActivitati(ASPxComboBox cmb, int idObi)
        {
            DataTable dt = Session["EvalLista_DSObiectiveXActivitate"] as DataTable;
            DataTable dtFiltru = dt.Select("IdObiectiv = " + idObi).CopyToDataTable();
            cmb.DataSource = dtFiltru;
            cmb.DataBind();
        }

        void cmbAct_OnCallback(object source, CallbackEventArgsBase e)
        {
            if (string.IsNullOrEmpty(e.Parameter)) return;
            IncarcaActivitati(source as ASPxComboBox, Convert.ToInt32(e.Parameter));
        }
    }
}