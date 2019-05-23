using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;
using DevExpress.Data;
using System.Web.Hosting;
using System.Data.OleDb;

namespace WizOne.ContracteLucru
{
    public partial class ContractDetaliu : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {

            string id = Session["IdContract"] as string;
            string esteNou = Session["ContractNou"] as string;

            DataSet ds = Session["InformatiaCurentaContracte"] as DataSet;
            DataTable dtCtr = new DataTable();

            if (ds == null)
            {
                ds = new DataSet();
                if (esteNou != "1")
                {
                    dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Contracte\" WHERE \"Id\" = " + id, null);
                }
                else
                {
                    dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Contracte\" WHERE \"Id\" = (SELECT MAX(\"Id\") FROM \"Ptj_Contracte\")", null);
                    object[] rowCtr = new object[dtCtr.Columns.Count];

                    int x = 0;
                    foreach (DataColumn col in dtCtr.Columns)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "ID":
                                rowCtr[x] = Convert.ToInt32(Session["IdContract"].ToString());
                                break;
                            case "ORESUP":
                                rowCtr[x] = 0;
                                break;
                            case "IDAUTO":
                                rowCtr[x] = Convert.ToInt32(General.Nz(dtCtr.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                                break;
                            case "USER_NO":
                                rowCtr[x] = Session["UserId"];
                                break;
                            case "TIME":
                                rowCtr[x] = DateTime.Now;
                                break;
                        }
                        x++;
                    }
                    dtCtr.Rows.RemoveAt(0);
                    dtCtr.Rows.Add(rowCtr);
                    dtCtr.PrimaryKey = new DataColumn[] { dtCtr.Columns["IdAuto"] };
                }

                ds.Tables.Add(dtCtr);
                Session["InformatiaCurentaContracte"] = ds;
            }

            if (!IsPostBack)
            {
                DataTable table = new DataTable();
                table = ds.Tables[0];
                DataList1.DataSource = table;
                DataList1.DataBind();

                ASPxTextBox txtId = DataList1.Items[0].FindControl("txtId") as ASPxTextBox;
                txtId.Enabled = false;

                if (esteNou != "1")
                {
                    ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
                    cmbTip.Enabled = false;
                }
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup")>=0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$")+1).Replace("a", "");


                #endregion

                TabPage tabPage1 = new TabPage();
                tabPage1.Name = "ContractAbsente";
                tabPage1.Text = "Absente";
                Control ctrl1 = new Control();
                ctrl1 = this.LoadControl(tabPage1.Name + ".ascx");
                tabPage1.Controls.Add(ctrl1);
                this.ASPxPageControl2.TabPages.Add(tabPage1);

                TabPage tabPage3 = new TabPage();
                tabPage3.Name = "ContractCiclic";
                tabPage3.Text = "Ciclicitate";
                Control ctrl3 = new Control();
                ctrl3 = this.LoadControl(tabPage3.Name + ".ascx");
                tabPage3.Controls.Add(ctrl3);
                this.ASPxPageControl2.TabPages.Add(tabPage3);

                //TabPage tabPage4 = new TabPage();
                //tabPage4.Name = "ContractVal";
                //tabPage4.Text = "Val-uri";
                //Control ctrl4 = new Control();
                //ctrl4 = this.LoadControl(tabPage4.Name + ".ascx");
                //tabPage4.Controls.Add(ctrl4);
                //this.ASPxPageControl2.TabPages.Add(tabPage4);

                TabPage tabPage2 = new TabPage();
                tabPage2.Name = "ContractZilnic";
                tabPage2.Text = "Schimburi contract";
                Control ctrl2 = new Control();
                ctrl2 = this.LoadControl(tabPage2.Name + ".ascx");
                tabPage2.Controls.Add(ctrl2);
                this.ASPxPageControl2.TabPages.Add(tabPage2);

                ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
                string esteNou = Session["ContractNou"] as string;

                if (esteNou != "1")
                {
                    if (Convert.ToInt32(cmbTip.Value ?? 1) == 1)
                    {
                        tabPage1.Visible = true;
                        tabPage2.Visible = true;
                        tabPage3.Visible = false;
                    }
                    else
                    {
                        tabPage1.Visible = false;
                        tabPage2.Visible = false;
                        tabPage3.Visible = true;
                    }
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


                ASPxComboBox cmbTip = DataList1.Items[0].FindControl("cmbTip") as ASPxComboBox;
                if (Convert.ToInt32(cmbTip.Value ?? 1) == 2)
                {
                    bool valid = true;
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        if (ds.Tables[i].TableName == "Ptj_ContracteCiclice")
                        {
                            for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                            {
                                if (ds.Tables[i].Rows[j]["IdContractZilnic"] == null || ds.Tables[i].Rows[j]["IdContractZilnic"].ToString().Length <= 0)
                                {
                                    valid = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (!valid)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date. Fiecare zi din ciclu trebuie sa aiba un contract zilnic."));
                        return;
                    }

                }

                if (Session["ContractNou"] != null && Session["ContractNou"].ToString().Length > 0 && Session["ContractNou"].ToString() == "1")
                {
                    InserareContract(Session["IdContract"].ToString(), ds.Tables[0]);
                    Session["ContractNou"] = "0";
                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    //if (Constante.tipBD == 1)
                    //{
                    //    SqlDataAdapter da = new SqlDataAdapter();
                    //    SqlCommandBuilder cb = new SqlCommandBuilder();
                    //    da = new SqlDataAdapter();
                    //    da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"" + ds.Tables[i].TableName + "\"", null);
                    //    cb = new SqlCommandBuilder(da);
                    //    da.Update(ds.Tables[i]);
                    //    da.Dispose();
                    //    da = null;
                    //}
                    //else
                    //{
                    //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
                    //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"" + ds.Tables[i].TableName + "\" WHERE ROWNUM = 0", null);
                    //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
                    //    oledbAdapter.Update(ds.Tables[i]);
                    //    oledbAdapter.Dispose();
                    //    oledbAdapter = null;            

                    //}
                    General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                }


                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {

            }
        }

        protected void InserareContract(string id, DataTable dt)
        {
            //if (Constante.tipBD == 1)
            //{
            //    SqlDataAdapter da = new SqlDataAdapter();
            //    SqlCommandBuilder cb = new SqlCommandBuilder();
            //    da = new SqlDataAdapter();
            //    da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"Ptj_Contracte\"", null);
            //    cb = new SqlCommandBuilder(da);
            //    da.Update(dt);
            //    da.Dispose();
            //    da = null;
            //}
            //else
            //{
            //    OracleDataAdapter oledbAdapter = new OracleDataAdapter();
            //    oledbAdapter.SelectCommand = General.DamiOleDbCommand("SELECT * FROM \"Ptj_Contracte\" WHERE ROWNUM = 0", null);
            //    OracleCommandBuilder cb = new OracleCommandBuilder(oledbAdapter);
            //    oledbAdapter.Update(dt);
            //    oledbAdapter.Dispose();
            //    oledbAdapter = null;

            //}
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

        //private void cmbTip_SelectedIndexChanged(string param)
        //{
        //    try
        //    {
        //        if (Convert.ToInt32(param) == 1)
        //        {
        //            this.ASPxPageControl2.TabPages[0].Visible = true;
        //            this.ASPxPageControl2.TabPages[3].Visible = true;
        //            this.ASPxPageControl2.TabPages[1].Visible = false;
        //        }
        //        else
        //        {
        //            this.ASPxPageControl2.TabPages[0].Visible = false;
        //            this.ASPxPageControl2.TabPages[3].Visible = false;
        //            this.ASPxPageControl2.TabPages[1].Visible = true;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}




    }



}