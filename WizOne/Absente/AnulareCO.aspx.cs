using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.SqlClient;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;

namespace WizOne.Absente
{
    public partial class AnulareCO : System.Web.UI.Page
    {
        //int F10003 = -99;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                //if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Constante.IdLimba = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion
                
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnAnulare.Text = Dami.TraduCuvant("btnAnulare", "Anulare");

                if (!IsPostBack)
                    txtAnLuna.Value = DateTime.Now;

                DataTable dtPer = new DataTable();
                dtPer.Columns.Add("Id", typeof(int));
                dtPer.Columns.Add("Denumire", typeof(string));

                dtPer.Rows.Add(1, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 1).ToString() + " inclusiv");
                dtPer.Rows.Add(2, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 2).ToString() + " inclusiv");
                dtPer.Rows.Add(3, "Mai vechi");

                cmbPerioada.DataSource = dtPer;
                cmbPerioada.DataBind();
                if (IsPostBack)
                {
                    DataTable dt = Session["AnulareCO_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
                else
                {
                    IncarcaGrid();
                }

                DataTable dtAng = General.IncarcaDT(Dami.SelectAngajati(), null);
                GridViewDataComboBoxColumn colAng = (grDate.Columns["F10003"] as GridViewDataComboBoxColumn);
                colAng.PropertiesComboBox.DataSource = dtAng;        
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        internal static string SelectAngajati()
        {
            string strSql = "";

            try
            {
                strSql = $@"SELECT A.F10003, A.F10008 {Dami.Operator()} ' ' {Dami.Operator()} A.F10009 AS NumeComplet, G.F00406 AS Filiala, H.F00507 AS Sectie, I.F00608 AS Departament
                        FROM F100 A
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, "Dami", new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }


        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string ctrl = e.Parameter.Split(';')[0];
                switch (ctrl)
                {
                    case "txtAnLuna":
                        IncarcaGrid();
                        break;
                    case "btnAnulare":
                        Anulare();
                        break;
                } 
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }



        private void IncarcaGrid()
        {
            try
            {

                if (txtAnLuna.Value == null)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat luna si anul!");
                    return;
                }

                if (IsPostBack)
                {
                    DataTable dtPer = new DataTable();
                    dtPer.Columns.Add("Id", typeof(int));
                    dtPer.Columns.Add("Denumire", typeof(string));

                    dtPer.Rows.Add(1, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 1).ToString() + " inclusiv");
                    dtPer.Rows.Add(2, "Pana in " + (Convert.ToDateTime(txtAnLuna.Value).Year - 2).ToString() + " inclusiv");
                    dtPer.Rows.Add(3, "Mai vechi");
                }

                string sql = "";
                DataTable dt = General.IncarcaDT(sql, null);
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;              
                grDate.DataBind();
                Session["AnulareCO_Grid"] = dt;   

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void Anulare()
        {
            try
            {
                List<int> lstMarci = new List<int>();
                List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003" });
                if (lst == null || lst.Count() == 0 || lst[0] == null)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat niciun angajat!");
                    return;
                }

                for (int i = 0; i < lst.Count(); i++)
                {
                    lstMarci.Add(Convert.ToInt32(General.Nz(lst[i], -99)));
                }

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            IncarcaGrid();
        }


    }


}