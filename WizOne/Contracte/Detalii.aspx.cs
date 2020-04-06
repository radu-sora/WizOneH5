using DevExpress.Web;
using DevExpress.Web.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Contracte
{
    public partial class Detalii : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup")>=0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$")+1).Replace("a", "");


                #endregion

                if (!IsPostBack)
                {
                    DataTable dtCmb = General.IncarcaDT(@"SELECT ""Id"", ""Denumire"" FROM ""Ptj_tblAbsente"" ORDER BY ""Denumire""  ", null);
                    GridViewDataComboBoxColumn colAbs = (grDateAbs.Columns["IdAbsenta"] as GridViewDataComboBoxColumn);
                    colAbs.PropertiesComboBox.DataSource = dtCmb;

                    int idCtr = Convert.ToInt32(General.Nz(Session["IdContract"],-99));
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

                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];

                        txtId.Value = dr["Id"];
                        txtDenumire.Value = General.Nz(dr["Denumire"], null);

                        txtOraSchIn.Value = General.Nz(dr["OraInSchimbare"], null);
                        txtOraSchOut.Value = General.Nz(dr["OraOutSchimbare"], null);
                        chkOreSup.Value = Convert.ToBoolean(General.Nz(dr["OreSup"], false));
                        cmbAfisare.Value = General.Nz(dr["Afisare"], null);
                        cmbRap.Value = General.Nz(dr["TipRaportareOreNoapte"], null);
                        chkPontareAuto.Value = General.Nz(dr["PontareAutomata"], false);
                        txtOraIn.Value = General.Nz(dr["OraInInitializare"], null);
                        txtOraOut.Value = General.Nz(dr["OraOutInitializare"], null);
                    }

                    grDateAbs.KeyFieldName = "IdAuto";
                    grDateAbs.DataSource = dtAbs;
                    grDateAbs.DataBind();

                    string sqlPrg = @"SELECT Id, Denumire, convert(varchar(5), OraIntrare, 108) AS OraIntrare, convert(varchar(5), OraIesire, 108) AS OraIesire FROM Ptj_Programe";
                    if (Constante.tipBD == 2)
                        sqlPrg = @"SELECT ""Id"", ""Denumire"", TO_CHAR(""OraIntrare"", 'HH24:mi') AS ""OraIntrare"", TO_CHAR(""OraIesire"", 'HH24:mi') AS ""OraIesire"" FROM ""Ptj_Programe""";
                    DataTable dtPrg = General.IncarcaDT(sqlPrg, null);

                    for (int i = 1; i <= 8; i++)
                    {
                        ASPxGridView grDate = tabCtr.FindControl("grDate" + i) as ASPxGridView;
                        if (grDate != null)
                        {
                            GridViewDataComboBoxColumn colPrg = (grDate.Columns["IdProgram"] as GridViewDataComboBoxColumn);
                            colPrg.PropertiesComboBox.DataSource = dtPrg;

                            grDate.KeyFieldName = "IdAuto";
                            grDate.DataSource = dtSch.Select("TipSchimb=" + i).CopyToDataTable();
                            grDate.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {           
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;


        //        ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
        //        if (Convert.ToInt32(cmbTip.Value ?? 1) == 2)
        //        {
        //            bool valid = true;
        //            for (int i = 0; i < ds.Tables.Count; i++)
        //            {
        //                if (ds.Tables[i].TableName == "Ptj_ContracteCiclice")
        //                {
        //                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
        //                    {
        //                        if (ds.Tables[i].Rows[j]["IdContractZilnic"] == null || ds.Tables[i].Rows[j]["IdContractZilnic"].ToString().Length <= 0)
        //                        {
        //                            valid = false;
        //                            break;
        //                        }
        //                    }
        //                }
        //            }

        //            if (!valid)
        //            {
        //                MessageBox.Show(Dami.TraduCuvant("Lipsesc date. Fiecare zi din ciclu trebuie sa aiba un contract zilnic."));
        //                return;
        //            }

        //        }

        //        if (Session["ContractNou"] != null && Session["ContractNou"].ToString().Length > 0 && Session["ContractNou"].ToString() == "1")
        //        {
        //            InserareContract(Session["IdContract"].ToString(), ds.Tables[0]);
        //            Session["ContractNou"] = "0";
        //        }

        //        for (int i = 0; i < ds.Tables.Count; i++)
        //        {
        //            General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
        //        }


        //        MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //protected void InserareContract(string id, DataTable dt)
        //{
        //    General.SalveazaDate(dt, "Ptj_Contracte");
        //}

        //protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        //{
        //    string[] param = e.Parameter.Split(';');
        //    DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
        //    switch (param[0])
        //    {
        //        case "txtId":
        //            ds.Tables[0].Rows[0]["Id"] = param[1];
        //            Session["InformatiaCurentaContracte"] = ds;
        //            break;
        //        case "txtNume":
        //            ds.Tables[0].Rows[0]["Denumire"] = param[1];
        //            Session["InformatiaCurentaContracte"] = ds;
        //            break;
        //        case "cmbTip":
        //            ds.Tables[0].Rows[0]["TipContract"] = param[1];
        //            Session["InformatiaCurentaContracte"] = ds;
        //            //cmbTip_SelectedIndexChanged(param[1]);
        //            break;   
        //    }
        //}

        protected void grDateAbs_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                bool suntModif = false;

                grDateAbs.CancelEdit();
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteAbsente"];
                if (dt == null) return;

                int idCtr = Convert.ToInt32(General.Nz(Session["IdContract"], -99));

                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
                    DataRow dr = dt.NewRow();

                    dr["IdContract"] = idCtr;
                    dr["IdAbsenta"] = upd.NewValues["IdAbsenta"] ?? -99;
                    dr["ZL"] = upd.NewValues["ZL"] ?? 9999;
                    dr["SL"] = upd.NewValues["SL"] ?? DBNull.Value;
                    dr["S"] = upd.NewValues["S"] ?? DBNull.Value;
                    dr["D"] = upd.NewValues["D"] ?? DBNull.Value;
                    dr["InPontajAnual"] = upd.NewValues["InPontajAnual"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);

                    suntModif = true;
                }


                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dr["IdAbsenta"] = upd.NewValues["IdAbsenta"] ?? -99;
                    dr["ZL"] = upd.NewValues["ZL"] ?? 9999;
                    dr["SL"] = upd.NewValues["SL"] ?? DBNull.Value;
                    dr["S"] = upd.NewValues["S"] ?? DBNull.Value;
                    dr["D"] = upd.NewValues["D"] ?? DBNull.Value;
                    dr["InPontajAnual"] = upd.NewValues["InPontajAnual"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    suntModif = true;
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

                    dt.Rows.Remove(dr);

                    suntModif = true;
                }

                if (suntModif == true)
                    General.SalveazaDate(dt, "Ptj_ContracteAbsente");

                e.Handled = true;

                Session["PtjCC"] = ds;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }

        protected void grDateSch_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {
                bool suntModif = false;
                ASPxGridView grDate = sender as ASPxGridView;
                int idx = Convert.ToInt32(grDate.ID.Replace("grDate", ""));

                grDate.CancelEdit();
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Ptj_ContracteSchimburi"].Select("TipSchimb=" + idx).CopyToDataTable();
                if (dt == null) return;

                int idCtr = Convert.ToInt32(General.Nz(Session["IdContract"], -99));
                

                //daca avem linii noi
                for (int i = 0; i < e.InsertValues.Count; i++)
                {
                    ASPxDataInsertValues upd = e.InsertValues[i] as ASPxDataInsertValues;
                    DataRow dr = dt.NewRow();

                    dr["IdContract"] = idCtr;
                    dr["TipSchimb"] = idx;
                    dr["IdProgram"] = upd.NewValues["IdProgram"] ?? 9999;
                    dr["OraInceput"] = upd.NewValues["OraInceput"] ?? DBNull.Value;
                    dr["OraInceputDeLa"] = upd.NewValues["OraInceputDeLa"] ?? DBNull.Value;
                    dr["OraInceputLa"] = upd.NewValues["OraInceputLa"] ?? DBNull.Value;
                    dr["OraSfarsit"] = upd.NewValues["OraSfarsit"] ?? DBNull.Value;
                    dr["OraSfarsitDeLa"] = upd.NewValues["OraSfarsitDeLa"] ?? DBNull.Value;
                    dr["OraSfarsitLa"] = upd.NewValues["OraSfarsitLa"] ?? DBNull.Value;
                    dr["ModVerificare"] = upd.NewValues["ModVerificare"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    dt.Rows.Add(dr);

                    suntModif = true;
                }


                //daca avem linii modificate
                for (int i = 0; i < e.UpdateValues.Count; i++)
                {
                    ASPxDataUpdateValues upd = e.UpdateValues[i] as ASPxDataUpdateValues;

                    object[] keys = new object[upd.Keys.Count];
                    for (int x = 0; x < upd.Keys.Count; x++)
                    { keys[x] = upd.Keys[x]; }

                    DataRow dr = dt.Rows.Find(keys);
                    if (dr == null) continue;

                    dr["TipSchimb"] = idx;
                    dr["IdProgram"] = upd.NewValues["IdProgram"] ?? 9999;
                    dr["OraInceput"] = upd.NewValues["OraInceput"] ?? DBNull.Value;
                    dr["OraInceputDeLa"] = upd.NewValues["OraInceputDeLa"] ?? DBNull.Value;
                    dr["OraInceputLa"] = upd.NewValues["OraInceputLa"] ?? DBNull.Value;
                    dr["OraSfarsit"] = upd.NewValues["OraSfarsit"] ?? DBNull.Value;
                    dr["OraSfarsitDeLa"] = upd.NewValues["OraSfarsitDeLa"] ?? DBNull.Value;
                    dr["OraSfarsitLa"] = upd.NewValues["OraSfarsitLa"] ?? DBNull.Value;
                    dr["ModVerificare"] = upd.NewValues["ModVerificare"] ?? DBNull.Value;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    suntModif = true;
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

                    dt.Rows.Remove(dr);

                    suntModif = true;
                }

                if (suntModif == true)
                    General.SalveazaDate(dt, "Ptj_ContracteSchimburi");

                e.Handled = true;

                Session["PtjCC"] = ds;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                e.Handled = true;
            }
        }



        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DataSet ds = Session["InformatiaCurenta"] as DataSet;
        //        DataTable dt = ds.Tables["Ptj_Contracte"];

        //        DataRow dr = dt.Rows[0];
        //        dr["Denumire"] = txtDenumire.Value ?? DBNull.Value;
        //        dr["OraInSchimbare"] = txtOraSchIn.Value ?? DBNull.Value;
        //        dr["OraOutSchimbare"] = txtOraSchOut.Value ?? DBNull.Value;
        //        dr["OreSup"] = chkOreSup.Value ?? DBNull.Value;
        //        dr["Afisare"] = cmbAfisare.Value ?? DBNull.Value;
        //        dr["TipRaportareOreNoapte"] = cmbRap.Value ?? DBNull.Value;
        //        dr["PontareAutomata"] = chkPontareAuto.Value ?? DBNull.Value;
        //        dr["OraInInitializare"] = txtOraIn.Value ?? DBNull.Value;
        //        dr["OraOutInitializare"] = txtOraOut.Value ?? DBNull.Value;
        //        dr["USER_NO"] = Session["UserId"];
        //        dr["TIME"] = DateTime.Now;

        //        General.SalveazaDate(dt, "Ptj_Contracte");

        //        grDateAbs.UpdateEdit();

        //        for(int i = 1; i < 8; i++)
        //        {
        //            ASPxGridView grDate = tabCtr.FindControl("grDate" + i) as ASPxGridView;
        //            if (grDate != null)
        //            {
        //                grDate.UpdateEdit();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
        //    }
        //}

        protected void pnlSectiune_Callback(object sender, CallbackEventArgsBase e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurenta"] as DataSet;
                DataTable dt = ds.Tables["Ptj_Contracte"];

                DataRow dr = dt.Rows[0];
                dr["Denumire"] = txtDenumire.Value ?? DBNull.Value;
                dr["OraInSchimbare"] = txtOraSchIn.Value ?? DBNull.Value;
                dr["OraOutSchimbare"] = txtOraSchOut.Value ?? DBNull.Value;
                dr["OreSup"] = chkOreSup.Value ?? DBNull.Value;
                dr["Afisare"] = cmbAfisare.Value ?? DBNull.Value;
                dr["TipRaportareOreNoapte"] = cmbRap.Value ?? DBNull.Value;
                dr["PontareAutomata"] = chkPontareAuto.Value ?? DBNull.Value;
                dr["OraInInitializare"] = txtOraIn.Value ?? DBNull.Value;
                dr["OraOutInitializare"] = txtOraOut.Value ?? DBNull.Value;
                dr["USER_NO"] = Session["UserId"];
                dr["TIME"] = DateTime.Now;

                General.SalveazaDate(dt, "Ptj_Contracte");

                grDateAbs.UpdateEdit();

                for (int i = 1; i < 8; i++)
                {
                    ASPxGridView grDate = tabCtr.FindControl("grDate" + i) as ASPxGridView;
                    if (grDate != null)
                    {
                        grDate.UpdateEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }
    }
}