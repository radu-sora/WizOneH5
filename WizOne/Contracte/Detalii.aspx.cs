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
        protected void Page_Init(object sender, EventArgs e)
        {
            //string id = Session["IdContract"] as string;
            //string esteNou = Session["ContractNou"] as string;

            //DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            //DataTable dtCtr = new DataTable();

            //if (ds == null)
            //{
            //    ds = new DataSet();
            //    if (esteNou != "1")
            //    {
            //        dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Contracte\" WHERE \"Id\" = " + id, null);
            //    }
            //    else
            //    {
            //        dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Contracte\" WHERE \"Id\" = (SELECT MAX(\"Id\") FROM \"Ptj_Contracte\")", null);
            //        object[] rowCtr = new object[dtCtr.Columns.Count];

            //        int x = 0;
            //        foreach (DataColumn col in dtCtr.Columns)
            //        {
            //            switch (col.ColumnName.ToUpper())
            //            {
            //                case "ID":
            //                    rowCtr[x] = Convert.ToInt32(Session["IdContract"].ToString());
            //                    break;
            //                case "ORESUP":
            //                    rowCtr[x] = 0;
            //                    break;
            //                case "IDAUTO":
            //                    //Florin 2019.09.06
            //                    //rowCtr[x] = Convert.ToInt32(General.Nz(dtCtr.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
            //                    rowCtr[x] = Convert.ToInt32(General.Nz(dtCtr.Compute("max([IdAuto])", string.Empty), 0)) + 1;
            //                    break;
            //                case "USER_NO":
            //                    rowCtr[x] = Session["UserId"];
            //                    break;
            //                case "TIME":
            //                    rowCtr[x] = DateTime.Now;
            //                    break;
            //                case "TIPCONTRACT":
            //                    rowCtr[x] = 1;
            //                    break;
            //            }
            //            x++;
            //        }
            //        if (dtCtr.Rows.Count > 0)
            //            dtCtr.Rows.RemoveAt(0);
            //        dtCtr.Rows.Add(rowCtr);
            //        dtCtr.PrimaryKey = new DataColumn[] { dtCtr.Columns["IdAuto"] };
            //    }

            //    //Florin 2019.10.28
            //    dtCtr.TableName = "Ptj_Contracte";
            //    //
            //    ds.Tables.Add(dtCtr);
            //    Session["InformatiaCurentaContracte"] = ds;
            //}

            //if (!IsPostBack)
            //{
            //    DataTable table = new DataTable();
            //    table = ds.Tables[0];
            //    DataList1.DataSource = table;
            //    DataList1.DataBind();

            //    ASPxTextBox txtId = DataList1.Items[0].FindControl("txtId") as ASPxTextBox;
            //    txtId.Enabled = false;

            //    //if (esteNou != "1")
            //    //{
            //    //    ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
            //    //    cmbTip.Enabled = false;
            //    //}

            //    ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
            //    cmbTip.Value = Convert.ToInt32(table.Rows[0]["TipContract"]);
            //}

        }

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
                    dtAbs.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };
                    DataTable dtSch = General.IncarcaDT(@"SELECT * FROM ""Ptj_ContracteSchimburi"" WHERE ""IdContract""=@1", new object[] { idCtr });
                    dtSch.TableName = "Ptj_ContracteSchimburi";
                    dtSch.PrimaryKey = new DataColumn[] { dt.Columns["IdAuto"] };

                    ds.Tables.Add(dt);
                    ds.Tables.Add(dtAbs);
                    ds.Tables.Add(dtSch);
                    Session["InformatiaCurenta"] = ds;

                    if (dt.Rows.Count > 0)
                    {
                        txtId.Value = dt.Rows[0]["Id"];
                        txtDenumire.Value = dt.Rows[0]["Denumire"];
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;


                //ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
                //if (Convert.ToInt32(cmbTip.Value ?? 1) == 2)
                //{
                //    bool valid = true;
                //    for (int i = 0; i < ds.Tables.Count; i++)
                //    {
                //        if (ds.Tables[i].TableName == "Ptj_ContracteCiclice")
                //        {
                //            for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                //            {
                //                if (ds.Tables[i].Rows[j]["IdContractZilnic"] == null || ds.Tables[i].Rows[j]["IdContractZilnic"].ToString().Length <= 0)
                //                {
                //                    valid = false;
                //                    break;
                //                }
                //            }
                //        }
                //    }

                //    if (!valid)
                //    {
                //        MessageBox.Show(Dami.TraduCuvant("Lipsesc date. Fiecare zi din ciclu trebuie sa aiba un contract zilnic."));
                //        return;
                //    }

                //}

                if (Session["ContractNou"] != null && Session["ContractNou"].ToString().Length > 0 && Session["ContractNou"].ToString() == "1")
                {
                    InserareContract(Session["IdContract"].ToString(), ds.Tables[0]);
                    Session["ContractNou"] = "0";
                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                }


                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void InserareContract(string id, DataTable dt)
        {
            General.SalveazaDate(dt, "Ptj_Contracte");
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            switch (param[0])
            {
                case "txtId":
                    ds.Tables[0].Rows[0]["Id"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "txtNume":
                    ds.Tables[0].Rows[0]["Denumire"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    break;
                case "cmbTip":
                    ds.Tables[0].Rows[0]["TipContract"] = param[1];
                    Session["InformatiaCurentaContracte"] = ds;
                    //cmbTip_SelectedIndexChanged(param[1]);
                    break;   
            }
        }

        protected void grDateAbs_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}