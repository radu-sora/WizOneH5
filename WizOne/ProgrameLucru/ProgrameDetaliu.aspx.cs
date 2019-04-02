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

namespace WizOne.ProgrameLucru
{
    public partial class ProgrameDetaliu : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {

            string id = Session["IdProgram"] as string;
            string esteNou = Session["ProgramNou"] as string;

            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            DataTable dtCtr = new DataTable();

            if (ds == null)
            {
                ds = new DataSet();
                if (esteNou != "1")
                {
                    dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Programe\" WHERE \"Id\" = " + id, null);
                }
                else
                {
                    dtCtr = General.IncarcaDT("SELECT * FROM \"Ptj_Programe\" WHERE \"Id\" = (SELECT MAX(\"Id\") FROM \"Ptj_Programe\")", null);
                    object[] rowCtr = new object[dtCtr.Columns.Count];

                    int x = 0;
                    foreach (DataColumn col in dtCtr.Columns)
                    {
                        switch (col.ColumnName.ToUpper())
                        {
                            case "ID":
                                rowCtr[x] = Convert.ToInt32(Session["IdProgram"].ToString());
                                break;
                            case "DENOAPTE":
                                rowCtr[x] = 0;
                                break;
                            case "FLEXIBIL":
                                rowCtr[x] = 0;
                                break;
                            case "NORMA":
                                rowCtr[x] = 8;
                                break;
                            case "TIPPONTARE":
                                rowCtr[x] = 1;
                                break;
                            case "PAUZADEDUSA":
                                rowCtr[x] = 0;
                                break;
                            case "ONVALSTR":
                                rowCtr[x] = 1;
                                break;
                            case "DATAINCEPUT":
                                rowCtr[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                break;
                            case "DATASFARSIT":
                                rowCtr[x] = new DateTime(2200, 12, 31);
                                break;
                            case "ORAINTRARE":
                                rowCtr[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0);
                                break;
                            case "ORAIESIRE":
                                rowCtr[x] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0);
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
                Session["InformatiaCurentaPrograme"] = ds;
            }
            if (!IsPostBack)
            {
                DataTable table = new DataTable();
                table = ds.Tables[0];
                DataList1.DataSource = table;
                DataList1.DataBind();

                ASPxTextBox txtId = DataList1.Items[0].FindControl("txtId") as ASPxTextBox;
                txtId.Enabled = false;
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
                tabPage1.Name = "ProgrameDateGenerale";
                tabPage1.Text = "Date generale";
                Control ctrl1 = new Control();
                ctrl1 = this.LoadControl(tabPage1.Name + ".ascx");
                tabPage1.Controls.Add(ctrl1);
                this.ASPxPageControl2.TabPages.Add(tabPage1);

                TabPage tabPage2 = new TabPage();
                tabPage2.Name = "ProgrameOreNormale";
                tabPage2.Text = "Ore Normale";
                Control ctrl2 = new Control();
                ctrl2 = this.LoadControl(tabPage2.Name + ".ascx");
                tabPage2.Controls.Add(ctrl2);
                this.ASPxPageControl2.TabPages.Add(tabPage2);

                TabPage tabPage3 = new TabPage();
                tabPage3.Name = "ProgrameOreSup";
                tabPage3.Text = "Ore Suplimentare";
                Control ctrl3 = new Control();
                ctrl3 = this.LoadControl(tabPage3.Name + ".ascx");
                tabPage3.Controls.Add(ctrl3);
                this.ASPxPageControl2.TabPages.Add(tabPage3);

                TabPage tabPage4 = new TabPage();
                tabPage4.Name = "ProgrameAlteOre";
                tabPage4.Text = "Ore Noapte";
                Control ctrl4 = new Control();
                ctrl4 = this.LoadControl(tabPage4.Name + ".ascx");
                tabPage4.Controls.Add(ctrl4);
                this.ASPxPageControl2.TabPages.Add(tabPage4);

                TabPage tabPage5 = new TabPage();
                tabPage5.Name = "ProgramePauza";
                tabPage5.Text = "Pauze";
                Control ctrl5 = new Control();
                ctrl5 = this.LoadControl(tabPage5.Name + ".ascx");
                tabPage5.Controls.Add(ctrl5);
                this.ASPxPageControl2.TabPages.Add(tabPage5);

                TabPage tabPage6 = new TabPage();
                tabPage6.Name = "ProgrameInSub";
                tabPage6.Text = "Intrare anticipata";
                Control ctrl6 = new Control();
                ctrl6 = this.LoadControl(tabPage6.Name + ".ascx");
                tabPage6.Controls.Add(ctrl6);
                this.ASPxPageControl2.TabPages.Add(tabPage6);

                TabPage tabPage7 = new TabPage();
                tabPage7.Name = "ProgrameInPeste";
                tabPage7.Text = "Intrare tarzie";
                Control ctrl7 = new Control();
                ctrl7 = this.LoadControl(tabPage7.Name + ".ascx");
                tabPage7.Controls.Add(ctrl7);
                this.ASPxPageControl2.TabPages.Add(tabPage7);

                TabPage tabPage8 = new TabPage();
                tabPage8.Name = "ProgrameOutSub";
                tabPage8.Text = "Iesire anticipata";
                Control ctrl8 = new Control();
                ctrl8 = this.LoadControl(tabPage8.Name + ".ascx");
                tabPage8.Controls.Add(ctrl8);
                this.ASPxPageControl2.TabPages.Add(tabPage8);

                TabPage tabPage9 = new TabPage();
                tabPage9.Name = "ProgrameOutPeste";
                tabPage9.Text = "Iesire tarzie";
                Control ctrl9 = new Control();
                ctrl9 = this.LoadControl(tabPage9.Name + ".ascx");
                tabPage9.Controls.Add(ctrl9);
                this.ASPxPageControl2.TabPages.Add(tabPage9);

                TabPage tabPage10 = new TabPage();
                tabPage10.Name = "ProgrameValuri";
                tabPage10.Text = "Val-uri din pontaj";
                Control ctrl10 = new Control();
                ctrl10 = this.LoadControl(tabPage10.Name + ".ascx");
                tabPage10.Controls.Add(ctrl10);
                this.ASPxPageControl2.TabPages.Add(tabPage10);

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
                DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
                //SqlDataAdapter da = new SqlDataAdapter();
                //SqlCommandBuilder cb = new SqlCommandBuilder();


                if (Session["ProgramNou"] != null && Session["ProgramNou"].ToString().Length > 0 && Session["ProgramNou"].ToString() == "1")
                {
                    InserareProgram(Session["IdProgram"].ToString(), ds.Tables[0]);
                    Session["ProgramNou"] = "0";
                }

                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    //da = new SqlDataAdapter();
                    //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"" + ds.Tables[i].TableName + "\"", null);
                    //cb = new SqlCommandBuilder(da);
                    //da.Update(ds.Tables[i]);
                    //da.Dispose();
                    //da = null;
                    General.SalveazaDate(ds.Tables[i], ds.Tables[i].TableName);
                }


                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {

            }
        }

        protected void InserareProgram(string id, DataTable dt)
        {

            //SqlDataAdapter da = new SqlDataAdapter();
            //SqlCommandBuilder cb = new SqlCommandBuilder();
            //da = new SqlDataAdapter();
            //da.SelectCommand = General.DamiSqlCommand("SELECT TOP 0 * FROM \"Ptj_Programe\"", null);
            //cb = new SqlCommandBuilder(da);
            //da.Update(dt);
            //da.Dispose();
            //da = null;
            General.SalveazaDate(dt, "Ptj_Programe");
        }


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            switch (param[0])
            {
                case "txtId":
                    ds.Tables[0].Rows[0]["Id"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtNume":
                    ds.Tables[0].Rows[0]["Denumire"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;
                case "txtDenScurta":
                    ds.Tables[0].Rows[0]["DenumireScurta"] = param[1];
                    Session["InformatiaCurentaPrograme"] = ds;
                    break;   
            }
        }
   




    }



}