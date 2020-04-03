using DevExpress.Web;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
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
                        chkOreSup.Value = General.Nz(dr["OreSup"], null);
                        cmbAfisare.Value = General.Nz(dr["Afisare"], null);
                        cmbRap.Value = General.Nz(dr["TipRaportareOreNoapte"], null);
                        chkPontareAuto.Value = General.Nz(dr["PontareAutomata"], null);
                        txtOraIn.Value = General.Nz(dr["OraInInitializare"], null);
                        txtOraOut.Value = General.Nz(dr["OraOutInitializare"], null);
                    }

                    grDateAbs.KeyFieldName = "IdAuto";
                    grDateAbs.DataSource = dtAbs;
                    grDateAbs.DataBind();
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

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void grDateSch_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }
    }
}