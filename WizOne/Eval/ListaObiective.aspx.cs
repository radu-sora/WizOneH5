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
    public partial class ListaObiective : System.Web.UI.Page
    {
        string cmp = "USER_NO,TIME,IDAUTO,";
        int tmpObi = -99;

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
            public int ParentId { get; set; }
        }

        List<metaDate> lstObiective = new List<metaDate>();
        List<metaDate> lstActivitati = new List<metaDate>();
        List<metaDate> lstSetAngajati = new List<metaDate>();
        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                DataTable dtObiective = General.IncarcaDT("select * from \"Eval_Obiectiv\" ", null);
                DataTable dtActivitati = General.IncarcaDT("select act.* from \"Eval_ObiectivXActivitate\" act join \"Eval_Obiectiv\" ob on act.\"IdObiectiv\" = ob.\"IdObiectiv\" ", null);
                DataTable dtSetAnagajati = General.IncarcaDT("select * from \"Eval_SetAngajati\" ", null);


                lstObiective = new List<metaDate>();
                lstActivitati = new List<metaDate>();
                lstSetAngajati = new List<metaDate>();

                foreach(DataRow rwObiectiv in dtObiective.Rows)
                {
                    metaDate clsObiectiv = new metaDate();
                    clsObiectiv.Id = Convert.ToInt32(rwObiectiv["IdObiectiv"].ToString());
                    clsObiectiv.Denumire = rwObiectiv["Obiectiv"].ToString();
                    lstObiective.Add(clsObiectiv);
                }
                Session["EvalLista_DSObiective"] = lstObiective;

                foreach(DataRow rwActivitate in dtActivitati.Rows)
                {
                    metaDate clsActivitate = new metaDate();
                    clsActivitate.Id = Convert.ToInt32(rwActivitate["IdActivitate"].ToString());
                    clsActivitate.Denumire = rwActivitate["Activitate"].ToString();
                    clsActivitate.ParentId = Convert.ToInt32(rwActivitate["IdObiectiv"].ToString());
                    lstActivitati.Add(clsActivitate);
                }
                Session["EvalLista_DSObiectiveXActivitate"] = lstActivitati;

                foreach (DataRow rwSetAngajat in dtSetAnagajati.Rows)
                {
                    metaDate clsSetAngajat = new metaDate();
                    clsSetAngajat.Id = Convert.ToInt32(rwSetAngajat["IdSetAng"].ToString());
                    clsSetAngajat.Denumire = rwSetAngajat["DenSet"].ToString();
                    lstSetAngajati.Add(clsSetAngajat);
                }
                Session["EvalLista_DSSetAngajat"] = lstSetAngajati;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie! ");
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
                if(!IsPostBack)
                {
                    switch (Session["Sablon_TipActiune"].ToString())
                    {
                        case "New":
                            break;
                        case "Edit":
                        case "Clone":
                            {
                                //incarcam header-ul
                                DataTable dtHead = General.IncarcaDT(@"SELECT * FROM ""Eval_ListaObiectiv"" WHERE ""IdLista""=@1 ", new string[] { IdListaObiective.ToString() });
                                if (dtHead.Rows.Count > 0)
                                {
                                    txtId.Text = dtHead.Rows[0]["IdLista"].ToString();
                                    txtCodLista.Text = dtHead.Rows[0]["CodLista"].ToString();
                                    txtDenLista.Text = dtHead.Rows[0]["DenLista"].ToString();
                                }
                            }
                            break;
                    }

                    DataSet ds = new DataSet();
                    string gridKey = "";
                    string sqlQuery = string.Empty;
                    sqlQuery = "select * from \"Eval_ListaObiectivDet\" where \"IdLista\" ={0}";
                    sqlQuery = string.Format(sqlQuery, IdListaObiective);
                    DataTable dt = General.IncarcaDT(sqlQuery, null);

                    DataColumn[] keys = dt.PrimaryKey;
                    for (int i = 0; i < keys.Count(); i++)
                        gridKey += ";" + keys[i].ToString();

                    Session["InformatiaCurenta"] = dt;

                    GridViewDataComboBoxColumn colObiective = (grDate.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colActivitati = (grDate.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colSetAngajati = (grDate.Columns["IdSetAngajat"] as GridViewDataComboBoxColumn);
                    colObiective.PropertiesComboBox.DataSource = lstObiective;
                    colActivitati.PropertiesComboBox.DataSource = lstActivitati;
                    colSetAngajati.PropertiesComboBox.DataSource = lstSetAngajati;

                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.KeyFieldName = "Id";
                    if (gridKey != "") grDate.KeyFieldName = gridKey.Substring(1);
                    grDate.DataBind();
                }
                else
                {
                    lstActivitati = new List<metaDate>();
                    lstObiective = new List<metaDate>();
                    lstSetAngajati = new List<metaDate>();

                    lstObiective = Session["EvalLista_DSObiective"] as List<metaDate>;
                    lstActivitati = Session["EvalLista_DSObiectiveXActivitate"] as List<metaDate>;
                    lstSetAngajati = Session["EvalLista_DSSetAngajat"] as List<metaDate>;
                    GridViewDataComboBoxColumn colObiective = (grDate.Columns["IdObiectiv"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colActivitati = (grDate.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                    GridViewDataComboBoxColumn colSetAngajati = (grDate.Columns["IdSetAngajat"] as GridViewDataComboBoxColumn);
                    colObiective.PropertiesComboBox.DataSource = lstObiective;
                    colActivitati.PropertiesComboBox.DataSource = lstActivitati;
                    colSetAngajati.PropertiesComboBox.DataSource = lstSetAngajati;

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
                sqlQuery = "select * from \"Eval_ListaObiectiv\" where \"IdLista\" = {0} ";
                sqlQuery = string.Format(sqlQuery, id);
                DataTable dtHead = General.IncarcaDT(sqlQuery, null);

                switch(Session["Sablon_TipActiune"].ToString())
                {
                    case "New":
                    case "Clone":
                        {
                            id = Dami.NextId("Eval_ListaObiectiv");
                            DataRow drHead = dtHead.NewRow();
                            drHead["IdLista"] = id;
                            drHead["CodLista"] = txtCodLista.Text;
                            drHead["DenLista"] = txtDenLista.Text;
                            drHead["TIME"] = DateTime.Now;
                            drHead["USER_NO"] = Session["UserId"];
                            dtHead.Rows.Add(drHead);

                            int nrInreg = dt.Rows.Count;
                            int Id = Dami.NextId("Eval_ListaObiectivDet", nrInreg);
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

                #region salvare Eval_ListaObiectiv
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter daHead = new SqlDataAdapter();
                    daHead.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_ListaObiectiv\" ", null);
                    SqlCommandBuilder cbHead = new SqlCommandBuilder(daHead);
                    daHead.Update(dtHead);
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_ListaObiectiv\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cbbHead = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dtHead);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }
                #endregion

                #region salvare Eval_ListaObiectivDet
                if (Constante.tipBD == 1)
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = General.DamiSqlCommand("select top 0 * from \"Eval_ListaObiectivDet\" ", null);
                    SqlCommandBuilder cb = new SqlCommandBuilder(da);
                    da.Update(dt);

                    da.Dispose();
                    da = null;
                }
                else
                {
                    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Eval_ListaObiectivDet\" WHERE ROWNUM = 0", null);
                    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                    oledbAdapter.Update(dt);
                    oledbAdapter.Dispose();
                    oledbAdapter = null;

                }
                #endregion

                HttpContext.Current.Session["Sablon_Tabela"] = "Eval_ListaObiectiv";
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
                int x = Dami.NextId("Eval_ListaObiectivDet");
                row["Id"] = x;
                row["IdObiectiv"] = e.NewValues["IdObiectiv"];
                row["IdActivitate"] = e.NewValues["IdActivitate"] ?? DBNull.Value;
                row["IdSetAngajat"] = e.NewValues["IdSetAngajat"];
                row["Target"] = e.NewValues["Target"];
                row["Ordine"] = e.NewValues["Ordine"];
                row["Vizibil"] = e.NewValues["Vizibil"];
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
                DataTable dt = Session["InformatiaCurenta"] as DataTable;
                e.NewValues["TIME"] = DateTime.Now;
                e.NewValues["USER_NO"] = Session["UserId"];
                e.NewValues["Id"] = dt.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomErrorText(object sender, DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs e)
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

        protected void Unnamed_ItemRequestedByValue(object source, ListEditItemRequestedByValueEventArgs e)
        {
            try
            {
                //int id;
                //if (e.Value == null || !int.TryParse(e.Value.ToString(), out id))
                //    return;
                //ASPxComboBox editor = source as ASPxComboBox;
                //var query = lstActivitati.Where(p => p.ParentId == id);
                //editor.DataSource = query.ToList();
                //editor.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Unnamed_ItemsRequestedByFilterCondition(object source, ListEditItemsRequestedByFilterConditionEventArgs e)
        {
            try
            {
                //ASPxComboBox editor = source as ASPxComboBox;
                //List<metaDate> query;
                //var take = e.EndIndex - e.BeginIndex + 1;
                //var skip = e.BeginIndex;
                //int countryValue = GetCurrentObjective();
                //if (countryValue > -1)
                //    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter) && p.ParentId == countryValue).OrderBy(p => p.Id).ToList<metaDate>();
                //else
                //    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter)).OrderBy(p => p.Id).ToList<metaDate>();
                //editor.DataSource = query;
                //editor.DataBind();
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
                //if (e.Column.FieldName == "IdObiectiv")
                //    e.Editor.ClientInstanceName = "ObjectiveEditor";
                //if (e.Column.FieldName != "IdActivitate")
                //    return;
                //var editor = (ASPxComboBox)e.Editor;
                //editor.ClientInstanceName = "ActivityEditor";
                //editor.ClientSideEvents.EndCallback = "cmbActivity_EndCallBack";

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

                //string idObi = (string)val;

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
            //if (string.IsNullOrEmpty(idObi)) return;

            cmb.DataSource = lstActivitati.Where(p => p.ParentId == idObi);
            cmb.DataBind();

            //List<string> cities = GetCities(country);
            //cmb.Items.Clear();
            //foreach (string city in cities)
            //    cmb.Items.Add(city);
        }

        //List<string> GetCities(string country)
        //{
        //    using (var context = new WorldCitiesContext())
        //        return context.Cities.Where(c => c.Country.CountryName == country).OrderBy(c => c.CityName).Select(c => c.CityName).ToList();
        //}

        void cmbAct_OnCallback(object source, CallbackEventArgsBase e)
        {
            if (string.IsNullOrEmpty(e.Parameter)) return;
            IncarcaActivitati(source as ASPxComboBox, Convert.ToInt32(e.Parameter));
        }


        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                //GridViewDataComboBoxColumn colAct = (grDate.Columns["IdActivitate"] as GridViewDataComboBoxColumn);
                //colAct.PropertiesComboBox.DataSource = lstActivitati.Where(p => p.ParentId == 1);

                //ASPxComboBox editor = source as ASPxComboBox;
                //List<metaDate> query;
                //var take = e.EndIndex - e.BeginIndex + 1;
                //var skip = e.BeginIndex;
                //int countryValue = GetCurrentObjective();
                //if (countryValue > -1)
                //    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter) && p.ParentId == countryValue).OrderBy(p => p.Id).ToList<metaDate>();
                //else
                //    query = lstActivitati.Where(p => p.Denumire.Contains(e.Filter)).OrderBy(p => p.Id).ToList<metaDate>();
                //editor.DataSource = query;
                //editor.DataBind();

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}