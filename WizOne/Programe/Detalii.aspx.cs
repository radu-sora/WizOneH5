using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Programe
{
    public partial class Detalii : System.Web.UI.Page
    {
        int idPrg = -99;
        bool esteNou = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];

                #endregion

                idPrg = Convert.ToInt32(General.Nz(Session["IdProgram"], -99));
                if (idPrg == -99)
                    esteNou = true;

                if (!IsPostBack)
                {
                    #region Incarcam DataSet-ul
                    DataSet ds = new DataSet();
                    string[] tbls = { "Ptj_Programe", "Ptj_ProgrameAlteOre", "Ptj_ProgrameOreNoapte", "Ptj_ProgramePauze", "Ptj_ProgrameTrepte", };
                    for(int i = 0; i < tbls.Length; i++)
                    {
                        string cmp = "IdProgram";
                        string cheie = "IdAuto";
                        if (i == 0)
                        {
                            cmp = cheie = "Id";
                        }

                        DataTable dt = General.IncarcaDT($@"SELECT * FROM ""{tbls[i]}"" WHERE ""{cmp}""=@1", new object[] { idPrg });
                        dt.TableName = tbls[i];
                        dt.PrimaryKey = new DataColumn[] { dt.Columns[cheie] };
                        ds.Tables.Add(dt);
                    }

                    Session["InformatiaCurenta"] = ds;
                    pnlTab.DataSource = ds.Tables["Ptj_Programe"];
                    pnlTab.DataBind();
                    #endregion

                    #region Incarcam ComboBox-urile
                    DataTable dtLa = General.IncarcaDT(@"SELECT ""Coloana"" AS ""Denumire"", COALESCE(""Alias"", ""Coloana"") AS ""Alias""  FROM ""Ptj_tblAdmin"" ORDER BY COALESCE(""Alias"", ""Coloana"")");
                    cmbONCamp.DataSource = dtLa;
                    cmbONCamp.DataBind();
                    cmbOSCamp.DataSource = dtLa;
                    cmbOSCamp.DataBind();
                    cmbOSCampSub.DataSource = dtLa;
                    cmbOSCampSub.DataBind();
                    cmbOSCampPeste.DataSource = dtLa;
                    cmbOSCampPeste.DataBind();
                    cmbInPlatit.DataSource = dtLa;
                    cmbInPlatit.DataBind();
                    cmbInNeplatit.DataSource = dtLa;
                    cmbInNeplatit.DataBind();
                    cmbInPlatitTr.DataSource = dtLa;
                    cmbInPlatitTr.DataBind();
                    cmbInNeplatitTr.DataSource = dtLa;
                    cmbInNeplatitTr.DataBind();
                    cmbOutPlatit.DataSource = dtLa;
                    cmbOutPlatit.DataBind();
                    cmbOutNeplatit.DataSource = dtLa;
                    cmbOutNeplatit.DataBind();
                    cmbOutPlatitTr.DataSource = dtLa;
                    cmbOutPlatitTr.DataBind();
                    cmbOutNeplatitTr.DataSource = dtLa;
                    cmbOutNeplatitTr.DataBind();

                    GridViewDataComboBoxColumn colNoapte = (grDateNoapte.Columns["Camp"] as GridViewDataComboBoxColumn);
                    colNoapte.PropertiesComboBox.DataSource = dtLa;

                    GridViewDataComboBoxColumn colAlte = (grDateAlte.Columns["Camp"] as GridViewDataComboBoxColumn);
                    colAlte.PropertiesComboBox.DataSource = dtLa;
                    #endregion

                    #region Incarcam Gridurile
                    grDateNoapte.KeyFieldName = "IdAuto";
                    grDateNoapte.DataSource = ds.Tables["Ptj_ProgrameOreNoapte"];
                    grDateNoapte.DataBind();

                    grDateAlte.KeyFieldName = "IdAuto";
                    grDateAlte.DataSource = ds.Tables["Ptj_ProgrameAlteOre"];
                    grDateAlte.DataBind();

                    grDatePauze.KeyFieldName = "IdAuto";
                    grDatePauze.DataSource = ds.Tables["Ptj_ProgramePauze"];
                    grDatePauze.DataBind();

                    DataTable dtNoRows = General.IncarcaDT(@"SELECT * FROM ""Ptj_ProgrameTrepte"" WHERE 1=2");
                    grDateIntrare.KeyFieldName = "IdAuto";
                    if (ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut='InPeste'").Length > 0)
                        grDateIntrare.DataSource = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut='InPeste'").CopyToDataTable();
                    else
                        grDateIntrare.DataSource = dtNoRows;
                    grDateIntrare.DataBind();

                    grDateIesire.KeyFieldName = "IdAuto";
                    if (ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut='OUTSub'").Length > 0)
                        grDateIesire.DataSource = ds.Tables["Ptj_ProgrameTrepte"].Select("TipInOut='OUTSub'").CopyToDataTable();
                    else
                        grDateIesire.DataSource = dtNoRows;
                    grDateIesire.DataBind();
                    #endregion

                    if (ds.Tables["Ptj_Programe"].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables["Ptj_Programe"].Rows[0];

                        txtId.Value = dr["Id"];
                        txtDenumire.Value = General.Nz(dr["Denumire"], null);
                        txtDenumireScurta.Value = General.Nz(dr["DenumireScurta"], null);
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void pnlCall_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                if (idPrg == -99)
                    idPrg = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) + 1 FROM ""Ptj_Contracte""", null), 0));

                string[] grids = { "grDateNoapte", "grDateAlte", "grDatePauze", "grDateIntrare", "grDateIesire" };

                for (int i = 0; i < grids.Length; i++)
                {
                    ASPxGridView grDate = pnlTab.FindControl(grids[i]) as ASPxGridView;
                    if (grDate != null)
                        grDate.UpdateEdit();
                }

                #region Salvam Ptj_Programe

                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Ptj_Programe"];

                DataRow dr = dt.NewRow();
                if (esteNou)
                    dr["Id"] = idPrg;
                else
                    dr = dt.Rows[0];

                dr["Denumire"] = txtDenumire.Value ?? DBNull.Value;
                dr["DenumireScurta"] = txtDenumireScurta.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                for(int i = 0; i < dt.Columns.Count; i++)
                {
                    string colNume = dt.Columns[i].ColumnName;
                    dynamic ctl = pnlTab.FindNestedControlByFieldName(colNume);
                    if (ctl != null)
                    {
                        dr[colNume] = ctl.Value;
                    }
                }

                if (esteNou)
                    dt.Rows.Add(dr);

                #endregion

                General.SalveazaDate(dt, "Ptj_Programe");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameAlteOre"], "Ptj_ProgrameAlteOre");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameOreNoapte"], "Ptj_ProgrameOreNoapte");
                General.SalveazaDate(ds.Tables["Ptj_ProgramePauze"], "Ptj_ProgramePauze");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameTrepte"], "Ptj_ProgrameTrepte");
                ASPxWebControl.RedirectOnCallback("~/Programe/Lista.aspx");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateNoapte_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateAlte_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDatePauze_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIntrare_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIesire_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grDate = sender as ASPxGridView;
                int idx = Convert.ToInt32(grDate.ID.Replace("grDate", ""));

                grDate.CancelEdit();
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];
                if (dt == null) return;

                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;

                    if (upd.NewValues["IdProgram"] == null) continue;

                    DataRow dr = dt.NewRow();

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        string numeCol = de.Key.ToString();
                        dynamic newValue = upd.NewValues[numeCol];
                        if (newValue == null)
                        {
                            dr[numeCol] = DBNull.Value;
                        }
                        else
                        {
                            switch (dr.Table.Columns[numeCol].DataType.ToString())
                            {
                                case "System.DateTime":
                                    if (Convert.ToDateTime(newValue).Year == 100)
                                        dr[numeCol] = ChangeToCurrentYear(newValue);
                                    else
                                        dr[numeCol] = newValue;
                                    break;
                                default:
                                    dr[numeCol] = newValue;
                                    break;
                            }
                        }
                    }

                    dr["IdContract"] = idPrg;
                    dr["TipSchimb"] = idx;
                    dr["IdProgram"] = upd.NewValues["IdProgram"];
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);
                }


                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    foreach (DictionaryEntry de in upd.NewValues)
                    {
                        string numeCol = de.Key.ToString();
                        dynamic oldValue = upd.OldValues[numeCol];
                        dynamic newValue = upd.NewValues[numeCol];
                        if (oldValue != null && upd.OldValues[numeCol].GetType() == typeof(System.DBNull))
                            oldValue = null;
                    }

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dr["TipSchimb"] = idx;
                    dr["IdProgram"] = upd.NewValues["IdProgram"] ?? 9999;
                    if (upd.NewValues["OraInceput"] != null)
                        dr["OraInceput"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraInceput"]));
                    else
                        dr["OraInceput"] = DBNull.Value;
                    if (upd.NewValues["OraInceputDeLa"] != null)
                        dr["OraInceputDeLa"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraInceputDeLa"]));
                    else
                        dr["OraInceputDeLa"] = DBNull.Value;
                    if (upd.NewValues["OraInceputLa"] != null)
                        dr["OraInceputLa"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraInceputLa"]));
                    else
                        dr["OraInceputLa"] = DBNull.Value;
                    if (upd.NewValues["OraSfarsit"] != null)
                        dr["OraSfarsit"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraSfarsit"]));
                    else
                        dr["OraSfarsit"] = DBNull.Value;
                    if (upd.NewValues["OraSfarsitDeLa"] != null)
                        dr["OraSfarsitDeLa"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraSfarsitDeLa"]));
                    else
                        dr["OraSfarsitDeLa"] = DBNull.Value;
                    if (upd.NewValues["OraSfarsitLa"] != null)
                        dr["OraSfarsitLa"] = ChangeToCurrentYear(Convert.ToDateTime(upd.NewValues["OraSfarsitLa"]));
                    else
                        dr["OraSfarsitLa"] = DBNull.Value;
                    dr["ModVerificare"] = upd.NewValues["ModVerificare"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                }


                //daca avem linii modificate
                for (int i = 0; i < e.DeleteValues.Count; i++)
                {
                    ASPxDataDeleteValues upd = e.DeleteValues[i] as ASPxDataDeleteValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dr.Delete();
                }

                e.Handled = true;
                Session["InformatiaCurenta"] = ds;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }

        private DateTime ChangeToCurrentYear(DateTime val)
        {
            DateTime dt = val;

            try
            {
                dt = new DateTime(2100, 1, 1, val.Hour, val.Minute, 0);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }


    }
}