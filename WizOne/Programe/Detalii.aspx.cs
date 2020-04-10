using DevExpress.Web;
using System;
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
                    pnlGen.DataSource = ds.Tables["Ptj_Programe"];
                    pnlGen.DataBind();

                    //pnlGen.FindItemByFieldName("PauzaMin").Visible = false;

                    DataTable dtLa = General.IncarcaDT(@"SELECT ""Coloana"" AS ""Denumire"", COALESCE(""Alias"", ""Coloana"") AS ""Alias""  FROM ""Ptj_tblAdmin"" ORDER BY COALESCE(""Alias"", ""Coloana"")");
                    cmbONCamp.DataSource = dtLa;
                    cmbONCamp.DataBind();
                    cmbOSCamp.DataSource = dtLa;
                    cmbOSCamp.DataBind();
                    cmbOSCampSub.DataSource = dtLa;
                    cmbOSCampSub.DataBind();
                    cmbOSCampPeste.DataSource = dtLa;
                    cmbOSCampPeste.DataBind();

                    grDateNoapte.KeyFieldName = "IdAuto";
                    grDateNoapte.DataSource = ds.Tables["Ptj_ProgrameOreNoapte"];
                    grDateNoapte.DataBind();

                    grDateAlte.KeyFieldName = "IdAuto";
                    grDateAlte.DataSource = ds.Tables["Ptj_ProgrameAlteOre"];
                    grDateAlte.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
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


        protected void pnlCall_Callback(object source, CallbackEventArgsBase e)
        {
            //string[] param = e.Parameter.Split(';');
            //DataSet ds = Session["InformatiaCurentaPrograme"] as DataSet;
            //switch (param[0])
            //{
            //    case "txtId":
            //        ds.Tables[0].Rows[0]["Id"] = param[1];
            //        Session["InformatiaCurentaPrograme"] = ds;
            //        break;
            //    case "txtNume":
            //        ds.Tables[0].Rows[0]["Denumire"] = param[1];
            //        Session["InformatiaCurentaPrograme"] = ds;
            //        break;
            //    case "txtDenScurta":
            //        ds.Tables[0].Rows[0]["DenumireScurta"] = param[1];
            //        Session["InformatiaCurentaPrograme"] = ds;
            //        break;   
            //}
        }

        protected void tabCtl_Callback(object sender, CallbackEventArgsBase e)
        {
        }

        protected void grDateNoapte_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {

        }

        protected void grDateAlte_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {

        }
    }



}