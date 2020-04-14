using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                Session["PaginaWeb"] = "Programe.Detalii";
                txtTitlu.Text = General.VarSession("Titlu").ToString();
                #endregion

                idPrg = Convert.ToInt32(General.Nz(Session["IdProgram"], -99));
                if (idPrg == -99)
                    esteNou = true;

                if (!IsPostBack)
                {
                    #region Incarcam DataSet-ul
                    DataSet ds = new DataSet();
                    string[] tbls = { "Ptj_Programe", "Ptj_ProgrameAlteOre", "Ptj_ProgrameOreNoapte", "Ptj_ProgramePauza", "Ptj_ProgrameTrepte", };
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
                    ctlONCamp.DataSource = dtLa;
                    ctlONCamp.DataBind();
                    ctlOSCamp.DataSource = dtLa;
                    ctlOSCamp.DataBind();
                    ctlOSCampSub.DataSource = dtLa;
                    ctlOSCampSub.DataBind();
                    ctlOSCampPeste.DataSource = dtLa;
                    ctlOSCampPeste.DataBind();
                    ctlINSubCampPlatit.DataSource = dtLa;
                    ctlINSubCampPlatit.DataBind();
                    ctlINSubCampNeplatit.DataSource = dtLa;
                    ctlINSubCampNeplatit.DataBind();
                    ctlINPesteCampPlatit.DataSource = dtLa;
                    ctlINPesteCampPlatit.DataBind();
                    ctlINPesteCampNeplatit.DataSource = dtLa;
                    ctlINPesteCampNeplatit.DataBind();
                    ctlOUTSubCampPlatit.DataSource = dtLa;
                    ctlOUTSubCampPlatit.DataBind();
                    ctlOUTSubCampNeplatit.DataSource = dtLa;
                    ctlOUTSubCampNeplatit.DataBind();
                    ctlOUTPesteCampPlatit.DataSource = dtLa;
                    ctlOUTPesteCampPlatit.DataBind();
                    ctlOUTPesteCampNeplatit.DataSource = dtLa;
                    ctlOUTPesteCampNeplatit.DataBind();

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
                    grDatePauze.DataSource = ds.Tables["Ptj_ProgramePauza"];
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

        //protected object GetLayoutItem(Control c)
        //{
        //    Control current = c;
        //    for (; ; )
        //    {
        //        current = current.Parent;
        //        if (current.GetType() == typeof(TableLayoutItemControl))
        //            return (current as LayoutItemControl).LayoutItem;
        //    }
        //}

        protected void pnlCall_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                if (idPrg == -99)
                    idPrg = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) + 1 FROM ""Ptj_Programe""", null), 0));

                grDateNoapte.UpdateEdit();
                grDateAlte.UpdateEdit();
                grDatePauze.UpdateEdit();
                grDateIntrare.UpdateEdit();
                grDateIesire.UpdateEdit();

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
                        if (General.IsDate(ctl.Value) && ctl.Value != null && Convert.ToDateTime(ctl.Value).Year == 100)
                            dr[colNume] = General.ChangeToCurrentYear(Convert.ToDateTime(ctl.Value));
                        else
                            dr[colNume] = ctl.Value ?? DBNull.Value;
                    }
                }

                if (esteNou)
                    dt.Rows.Add(dr);

                #endregion

                General.SalveazaDate(dt, "Ptj_Programe");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameAlteOre"], "Ptj_ProgrameAlteOre");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameOreNoapte"], "Ptj_ProgrameOreNoapte");
                General.SalveazaDate(ds.Tables["Ptj_ProgramePauza"], "Ptj_ProgramePauza");
                General.SalveazaDate(ds.Tables["Ptj_ProgrameTrepte"], "Ptj_ProgrameTrepte");
                ASPxWebControl.RedirectOnCallback("~/Programe/Lista.aspx");
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                ASPxGridView grDate = sender as ASPxGridView;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("IdProgram", idPrg.ToString());
                
                string numeTabela = "";
                switch (grDate.ID)
                {
                    case "grDateNoapte":
                        numeTabela = "Ptj_ProgrameOreNoapte";
                        break;
                    case "grDateAlte":
                        numeTabela = "Ptj_ProgrameAlteOre";
                        break;
                    case "grDatePauze":
                        numeTabela = "Ptj_ProgramePauza";
                        break;
                    case "grDateIntrare":
                        numeTabela = "Ptj_ProgrameTrepte";
                        dic.Add("TipInOut", "InPeste");
                        break;
                    case "grDateIesire":
                        numeTabela = "Ptj_ProgrameTrepte";
                        dic.Add("TipInOut", "OUTSub");
                        break;
                }
                General.BatchUpdate(sender, e, numeTabela, dic);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}