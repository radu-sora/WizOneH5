using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Contracte
{
    public partial class Detalii : Page
    {
        int idCtr = -99;
        bool esteNou = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                Session["PaginaWeb"] = "Programe.Lista";
                txtTitlu.Text = General.VarSession("Titlu").ToString();
                #endregion

                idCtr = Convert.ToInt32(General.Nz(Session["IdContract"], -99));
                if (idCtr == -99)
                    esteNou = true;

                if (!IsPostBack)
                {
                    #region Incarcam DataSet-ul
                    DataSet ds = new DataSet();
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Ptj_Contracte"" WHERE ""Id""=@1", new object[] { idCtr });
                    dt.TableName = "Ptj_Contracte";
                    dt.PrimaryKey = new DataColumn[] { dt.Columns["Id"] };
                    DataTable dtAbs = General.IncarcaDT(@"SELECT * FROM ""Ptj_ContracteAbsente"" WHERE ""IdContract""=@1", new object[] { idCtr });
                    dtAbs.TableName = "Ptj_ContracteAbsente";
                    dtAbs.PrimaryKey = new DataColumn[] { dtAbs.Columns["IdAuto"] };
                    DataTable dtSch = General.IncarcaDT(@"SELECT * FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""=@1", new object[] { idCtr });
                    dtSch.TableName = "Ptj_ContracteSchimburi";
                    dtSch.PrimaryKey = new DataColumn[] { dtSch.Columns["IdAuto"] };

                    ds.Tables.Add(dt);
                    ds.Tables.Add(dtAbs);
                    ds.Tables.Add(dtSch);
                    Session["InformatiaCurenta"] = ds;

                    pnlTab.DataSource = ds.Tables["Ptj_Contracte"];
                    pnlTab.DataBind();
                    #endregion

                    #region Incarcam ComboBox-urile
                    DataTable dtCmb = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ORDER BY ""Denumire""  ", null);
                    GridViewDataComboBoxColumn colAbs = (grDateAbs.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
                    colAbs.PropertiesComboBox.DataSource = dtCmb;
                    #endregion

                    #region Incarcam Gridurile
                    grDateAbs.KeyFieldName = "IdAuto";
                    grDateAbs.DataSource = dtAbs;
                    grDateAbs.DataBind();
                    IncarcaGriduriSchimburi();
                    #endregion

                    if (ds.Tables["Ptj_Contracte"].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables["Ptj_Contracte"].Rows[0];

                        txtId.Value = dr["Id"];
                        txtDenumire.Value = General.Nz(dr["Denumire"], null);
                    }
                }
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
                switch (grDate.ID)
                {
                    case "grDateAbs":
                        dic.Add("IdContract", idCtr.ToString());
                        General.BatchUpdate(sender, e, "Ptj_ContracteAbsente", dic);
                        break;
                    default:
                        int idx = Convert.ToInt32(grDate.ID.Replace("grDate", ""));
                        dic.Add("IdContract", idCtr.ToString());
                        dic.Add("TipSchimb", idx.ToString());
                        General.BatchUpdate(sender, e, "Ptj_ContracteSchimburi", dic);
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }

        protected void pnlCall_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                switch (e.Parameter)
                {
                    case "btnSave":
                        {
                            if (idCtr == -99)
                                idCtr = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) + 1 FROM ""Ptj_Contracte""", null), 0));

                            grDateAbs.UpdateEdit();

                            for (int i = 1; i < 8; i++)
                            {
                                ASPxGridView grDate = pnlPrg.FindNestedControlByFieldName("grDate" + i) as ASPxGridView;
                                if (grDate != null)
                                    grDate.UpdateEdit();
                            }

                            //salvam in Ptj_Contracte
                            DataSet ds = Session["InformatiaCurenta"] as DataSet;
                            DataTable dt = ds.Tables["Ptj_Contracte"];
                            DataTable dtSch = ds.Tables["Ptj_ContracteSchimburi"];

                            DataRow dr = dt.NewRow();
                            if (esteNou)
                                dr["Id"] = idCtr;
                            else
                                dr = dt.Rows[0];

                            dr["Denumire"] = txtDenumire.Value ?? DBNull.Value;
                            dr["OreSup"] = chkOreSup.Value ?? DBNull.Value;
                            dr["TipRaportareOreNoapte"] = cmbRap.Value ?? DBNull.Value;
                            dr["PontareAutomata"] = chkPontareAuto.Value ?? DBNull.Value;
                            if (txtOraSchIn.Value != null)
                                dr["OraInSchimbare"] = General.ChangeToCurrentYear(txtOraSchIn.DateTime);
                            else
                                dr["OraInSchimbare"] = DBNull.Value;
                            if (txtOraSchOut.Value != null)
                                dr["OraOutSchimbare"] = General.ChangeToCurrentYear(txtOraSchOut.DateTime);
                            else
                                dr["OraOutSchimbare"] = DBNull.Value;
                            if (txtOraIn.Value != null)
                                dr["OraInInitializare"] = General.ChangeToCurrentYear(txtOraIn.DateTime);
                            else
                                dr["OraInInitializare"] = DBNull.Value;
                            if (txtOraOut.Value != null)
                                dr["OraOutInitializare"] = General.ChangeToCurrentYear(txtOraOut.DateTime);
                            else
                                dr["OraOutInitializare"] = DBNull.Value;

                            for (int i = 1; i <= 8; i++)
                            {
                                int cnt = dtSch.Select("TipSchimb=" + i).Length;
                                switch (cnt)
                                {
                                    case 0:
                                        dr["TipSchimb" + i] = DBNull.Value;
                                        dr["Program" + i] = DBNull.Value;
                                        break;
                                    case 1:
                                        dr["TipSchimb" + i] = 1;
                                        DataTable dtTmp = dtSch.Select("TipSchimb=" + i).CopyToDataTable();
                                        dr["Program" + i] = dtTmp.Rows[0]["IdProgram"];
                                        break;
                                    default:
                                        dr["TipSchimb" + i] = 2;
                                        dr["Program" + i] = DBNull.Value;
                                        break;
                                }
                            }

                            dr["USER_NO"] = Session["UserId"];
                            dr["TIME"] = DateTime.Now;

                            if (esteNou)
                                dt.Rows.Add(dr);

                            General.SalveazaDate(dt, "Ptj_Contracte");
                            General.SalveazaDate(ds.Tables["Ptj_ContracteAbsente"], "Ptj_ContracteAbsente");
                            General.SalveazaDate(ds.Tables["Ptj_ContracteSchimburi"], "Ptj_ContracteSchimburi");

                            ASPxWebControl.RedirectOnCallback("~/Contracte/Lista.aspx");
                        }
                        break;
                    case "btnDuplica":
                        {
                            if (cmbZiDeLa.Value != null && cmbZiPentru.Value != null)
                            {
                                if (idCtr == -99)
                                    idCtr = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) + 1 FROM ""Ptj_Contracte""", null), 0));

                                ASPxGridView grDate = pnlPrg.FindNestedControlByFieldName("grDate" + cmbZiDeLa.Value) as ASPxGridView;
                                if (grDate != null)
                                    grDate.UpdateEdit();

                                string schDes = cmbZiPentru.Text.Replace(";", ",").Replace("Luni", "1").Replace("Marti", "2").Replace("Miercuri", "3").Replace("Joi", "4").Replace("Vineri", "5").Replace("Sambata", "6").Replace("Duminica", "7").Replace("Sarbatori legale", "8").Replace(cmbZiDeLa.Value + ",", "");
                                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];

                                //stergem liniile existente
                                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                                {
                                    DataRow dr = dt.Rows[i];
                                    if (dr.RowState != DataRowState.Deleted)
                                    {
                                        if (schDes.IndexOf(dr["TipSchimb"].ToString()) >= 0)
                                            dr.Delete();
                                    }
                                }

                                //adaugam liniile
                                DataTable dtZi = dt.Select("TipSchimb = " + cmbZiDeLa.Value).CopyToDataTable();
                                string[] arr = schDes.Split(',');
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    for (int j = 0; j < dtZi.Rows.Count; j++)
                                    {
                                        DataRow dr = dt.NewRow();
                                        DataRow drOri = dtZi.Rows[j];

                                        dr["IdContract"] = idCtr;
                                        dr["TipSchimb"] = arr[i];
                                        dr["IdProgram"] = drOri["IdProgram"];
                                        dr["OraInceput"] = drOri["OraInceput"];
                                        dr["OraInceputDeLa"] = drOri["OraInceputDeLa"];
                                        dr["OraInceputLa"] = drOri["OraInceputLa"];
                                        dr["OraSfarsit"] = drOri["OraSfarsit"];
                                        dr["OraSfarsitDeLa"] = drOri["OraSfarsitDeLa"];
                                        dr["OraSfarsitLa"] = drOri["OraSfarsitLa"];
                                        dr["ModVerificare"] = drOri["ModVerificare"];
                                        dr["USER_NO"] = Session["UserId"];
                                        dr["TIME"] = DateTime.Now;

                                        dt.Rows.Add(dr);
                                    }
                                }

                                Session["InformatiaCurenta"] = ds;
                                IncarcaGriduriSchimburi();

                                DataTable dtCmb = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ORDER BY ""Denumire""  ", null);
                                GridViewDataComboBoxColumn colAbs = (grDateAbs.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
                                colAbs.PropertiesComboBox.DataSource = dtCmb;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGriduriSchimburi()
        {
            try
            {
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dtSch = ds.Tables["Ptj_ContracteSchimburi"];

                string sqlPrg = @"SELECT Id, Denumire, convert(varchar(5), OraIntrare, 108) AS OraIntrare, convert(varchar(5), OraIesire, 108) AS OraIesire FROM Ptj_Programe";
                if (Constante.tipBD == 2)
                    sqlPrg = @"SELECT ""Id"", ""Denumire"", TO_CHAR(""OraIntrare"", 'HH24:mi') AS ""OraIntrare"", TO_CHAR(""OraIesire"", 'HH24:mi') AS ""OraIesire"" FROM ""Ptj_Programe""";
                DataTable dtPrg = General.IncarcaDT(sqlPrg, null);

                string sqlNoRows = @"SELECT TOP 0 * FROM ""Ptj_ContracteSchimburi""";
                if (Constante.tipBD == 2)
                    sqlNoRows = @"SELECT * FROM ""Ptj_ContracteSchimburi"" WHERE ROWNUM < 1";
                DataTable dtNoRows = General.IncarcaDT(sqlNoRows, null);
                dtNoRows.TableName = "Ptj_NoRows";
                dtNoRows.PrimaryKey = new DataColumn[] { dtNoRows.Columns["IdAuto"] };

                for (int i = 1; i <= 8; i++)
                {
                    ASPxGridView grDate = pnlPrg.FindNestedControlByFieldName("grDate" + i) as ASPxGridView;
                    if (grDate != null)
                    {
                        GridViewDataComboBoxColumn colPrg = (grDate.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                        colPrg.PropertiesComboBox.DataSource = dtPrg;

                        grDate.KeyFieldName = "IdAuto";
                        if (dtSch.Select("TipSchimb=" + i).Length > 0)
                            grDate.DataSource = dtSch.Select("TipSchimb=" + i).CopyToDataTable();
                        else
                            grDate.DataSource = dtNoRows;
                        grDate.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void tabCtr_Callback(object sender, CallbackEventArgsBase e)
        //{
        //    try
        //    {
        //        if (cmbZiDeLa.Value != null && cmbZiPentru.Value != null)
        //        {
        //            if (idCtr == -99)
        //                idCtr = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(""Id""),0) + 1 FROM ""Ptj_Contracte""", null), 0));

        //            ASPxGridView grDate = tabCtr.FindControl("grDate" + cmbZiDeLa.Value) as ASPxGridView;
        //            if (grDate != null)
        //                grDate.UpdateEdit();

        //            string schDes = cmbZiPentru.Text.Replace(";", ",").Replace("Luni", "1").Replace("Marti", "2").Replace("Miercuri", "3").Replace("Joi", "4").Replace("Vineri", "5").Replace("Sambata", "6").Replace("Duminica", "7").Replace("Sarbatori legale", "8").Replace(cmbZiDeLa.Value + ",", "");
        //            DataSet ds = Session["InformatiaCurenta"] as DataSet;
        //            DataTable dt = ds.Tables["Ptj_ContracteSchimburi"];

        //            //stergem liniile existente
        //            for (int i = dt.Rows.Count - 1; i >= 0; i--)
        //            {
        //                DataRow dr = dt.Rows[i];
        //                if (dr.RowState != DataRowState.Deleted)
        //                {
        //                    if (schDes.IndexOf(dr["TipSchimb"].ToString()) >= 0)
        //                        dr.Delete();
        //                }
        //            }

        //            //adaugam liniile
        //            DataTable dtZi = dt.Select("TipSchimb = " + cmbZiDeLa.Value).CopyToDataTable();
        //            string[] arr = schDes.Split(',');
        //            for (int i = 0; i < arr.Length; i++)
        //            {
        //                for (int j = 0; j < dtZi.Rows.Count; j++)
        //                {
        //                    DataRow dr = dt.NewRow();
        //                    DataRow drOri = dtZi.Rows[j];

        //                    dr["IdContract"] = idCtr;
        //                    dr["TipSchimb"] = arr[i];
        //                    dr["IdProgram"] = drOri["IdProgram"];
        //                    dr["OraInceput"] = drOri["OraInceput"];
        //                    dr["OraInceputDeLa"] = drOri["OraInceputDeLa"];
        //                    dr["OraInceputLa"] = drOri["OraInceputLa"];
        //                    dr["OraSfarsit"] = drOri["OraSfarsit"];
        //                    dr["OraSfarsitDeLa"] = drOri["OraSfarsitDeLa"];
        //                    dr["OraSfarsitLa"] = drOri["OraSfarsitLa"];
        //                    dr["ModVerificare"] = drOri["ModVerificare"];
        //                    dr["USER_NO"] = Session["UserId"];
        //                    dr["TIME"] = DateTime.Now;

        //                    dt.Rows.Add(dr);
        //                }
        //            }

        //            Session["InformatiaCurenta"] = ds;
        //            IncarcaGriduriSchimburi();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}
    }
}