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

namespace WizOne.Pagini
{
    public partial class IstoricModificari : System.Web.UI.Page
    {
        int F10003 = -99;

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

                DataTable dtTabele = GetTabele();             
                cmbTabela.DataSource = dtTabele;
                cmbTabela.DataBind();
                             

                DataTable dtTipOp = new DataTable();
                dtTipOp.Columns.Add("Id", typeof(int));
                dtTipOp.Columns.Add("Denumire", typeof(string));

                dtTipOp.Rows.Add(1, "Update");
                dtTipOp.Rows.Add(2, "Insert");
                dtTipOp.Rows.Add(3, "Delete");

                cmbTipOp.DataSource = dtTipOp;
                cmbTipOp.DataBind();
                if (IsPostBack)
                {
                    DataTable dt = Session["WizTrace_Grid"] as DataTable;
                    grDate.KeyFieldName = "IdAuto";
                    grDate.DataSource = dt;
                    grDate.DataBind();

                    cmbUtilWin.DataSource = Session["WizTrace_UtilWin"] as DataTable;
                    cmbUtilWin.DataBind();
                    
                    cmbUtilWSWO.DataSource = Session["WizTrace_UtilWSWO"] as DataTable;
                    cmbUtilWSWO.DataBind();
                    
                    cmbNumeCalc.DataSource = Session["WizTrace_NumeCalc"] as DataTable;
                    cmbNumeCalc.DataBind();
                }           

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }




        public DataTable GetTabele()
        {
            DataTable q = null;

            string sql = "";

            if (Constante.tipBD == 2)
                sql = "SELECT ROWNUM AS \"Id\", SUBSTR(UPPER(TABLE_NAME), 4, LENGTH(UPPER(TABLE_NAME)) - 3) as \"Denumire\" FROM user_tables WHERE UPPER(TABLE_NAME) LIKE 'WT_%' ORDER BY \"Denumire\"";
            else
                sql = "SELECT CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS Id, SUBSTRING(UPPER(TABLE_NAME), 4, LEN(UPPER(TABLE_NAME)) -3) as Denumire FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE 'WT_%' ORDER BY Denumire";

            try
            {
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }



        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string ctrl = e.Parameter.Split(';')[0];
                switch(ctrl)
                {
                    case "cmbTabela":
                        cmbTabela_SelectedIndexChanged(e.Parameter.Split(';')[1]);
                        break;
                    case "btnFiltru": 
                        IncarcaGrid();
                        break;
                    case "btnFiltruSterge":
                        StergeFiltre();
                        break;
                    case "FaraFiltru":
                        IncarcaGridFaraFiltru();
                        break;
                } 
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void cmbTabela_SelectedIndexChanged(string text)
        {
            try
            {
                if (text.Length > 0)
                {
                    cmbUtilWin.DataSource = null;
                    cmbUtilWSWO.DataSource = null;
                    cmbNumeCalc.DataSource = null;

                    Session["WizTrace_UtilWin"] = GetUserWin(text);
                    cmbUtilWin.DataSource = Session["WizTrace_UtilWin"] as DataTable;
                    cmbUtilWin.DataBind();

                    Session["WizTrace_UtilWSWO"] = GetUserWS(text);
                    cmbUtilWSWO.DataSource = Session["WizTrace_UtilWSWO"] as DataTable;
                    cmbUtilWSWO.DataBind();

                    Session["WizTrace_NumeCalc"] = GetCompName(text);
                    cmbNumeCalc.DataSource = Session["WizTrace_NumeCalc"] as DataTable;
                    cmbNumeCalc.DataBind();   
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

                if ((deDataInceput.Value != null && deDataSfarsit.Value == null) || (deDataInceput.Value == null && deDataSfarsit.Value != null))
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati selectat intervalul de timp!");
                    return;
                }

                string filtru = "";

                if (deDataInceput.Value != null)
                {
                    DateTime dataSt = Convert.ToDateTime(deDataInceput.Value);
                    DateTime dataSf = Convert.ToDateTime(deDataSfarsit.Value);
                    if (Constante.tipBD == 1)
                        filtru = " WHERE CONVERT(DATE, DATA) >= CONVERT(DATE, '" + dataSt.Day.ToString().PadLeft(2, '0') + "/" + dataSt.Month.ToString().PadLeft(2, '0') + "/" + dataSt.Year.ToString() + "', 103) AND CONVERT(DATE, DATA) <= CONVERT(DATE, '" + dataSf.Day.ToString().PadLeft(2, '0') + "/" + dataSf.Month.ToString().PadLeft(2, '0') + "/" + dataSf.Year.ToString() + "', 103) ";
                    else
                        filtru = " WHERE TRUNC(DATA) >= TRUNC(TO_DATE('" + dataSt.Day.ToString().PadLeft(2, '0') + "/" + dataSt.Month.ToString().PadLeft(2, '0') + "/" + dataSt.Year.ToString() + "', 'dd/mm/yyyy')) AND TRUNC(DATA) <= TRUNC(TO_DATE('" + dataSf.Day.ToString().PadLeft(2, '0') + "/" + dataSf.Month.ToString().PadLeft(2, '0') + "/" + dataSf.Year.ToString() + "', 'dd/mm/yyyy')) ";
                }
                if (cmbTipOp.Value != null)
                    filtru += (filtru.Length <= 0 ? " WHERE " : " AND ") + "COD_OP = '" + cmbTipOp.Text.Substring(0, 1) + "' ";


                if (cmbUtilWin.Value != null)
                    filtru += (filtru.Length <= 0 ? " WHERE " : " AND ") + "USER_WIN = '" + cmbUtilWin.Text + "' ";

                if (cmbUtilWSWO.Value != null)
                    filtru += (filtru.Length <= 0 ? " WHERE " : " AND ") + "USER_WS = '" + cmbUtilWSWO.Text + "' ";

                if (cmbNumeCalc.Value != null)
                    filtru += (filtru.Length <= 0 ? " WHERE " : " AND ") + "COMPUTER_NAME = '" + cmbNumeCalc.Text + "' ";                

                DataTable dt = GetTrace(cmbTabela.Text, filtru);
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;              
                grDate.DataBind();
                Session["WizTrace_Grid"] = dt;   

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGridFaraFiltru()
        {
            try
            {
                DataTable dt = GetTrace(cmbTabela.Text, "");
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dt;
                grDate.DataBind();
                Session["WizTrace_Grid"] = dt;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }        
        }


        protected void StergeFiltre()
        {
            deDataInceput.Value = null;
            deDataSfarsit.Value = null;
            cmbTipOp.Value = null;
            cmbTipOp.SelectedIndex = -1;
            cmbUtilWin.Value = null;
            cmbUtilWin.SelectedIndex = -1;
            cmbUtilWSWO.Value = null;
            cmbUtilWSWO.SelectedIndex = -1;
            cmbNumeCalc.Value = null;
            cmbUtilWSWO.SelectedIndex = -1;
            cmbTabela.Value = null;
            cmbTabela.SelectedIndex = -1;
        }




        public DataTable GetUserWin(string tabela)
        {
            DataTable q = null;

            string sql = "";

            if (Constante.tipBD == 2)
                sql = "SELECT ROWNUM AS \"Id\", \"Denumire\" FROM (SELECT DISTINCT USER_WIN as \"Denumire\" FROM WT_" + tabela + ") a ORDER BY \"Denumire\"";
            else
                sql = "SELECT CONVERT(int, ROW_NUMBER() OVER (ORDER BY Denumire)) AS Id, Denumire FROM (SELECT DISTINCT USER_WIN as Denumire FROM WT_" + tabela + ") a ORDER BY Denumire";

            try
            {
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetUserWS(string tabela)
        {
            DataTable q = null;

            string sql = "";

            if (Constante.tipBD == 2)
                sql = "SELECT ROWNUM AS \"Id\", \"Denumire\" FROM (SELECT DISTINCT F70104 as \"Denumire\" FROM WT_" + tabela + " LEFT JOIN USERS ON F70102=USER_WS) a ORDER BY \"Denumire\"";
            else
                sql = "SELECT CONVERT(int, ROW_NUMBER() OVER (ORDER BY Denumire)) AS Id, Denumire FROM (SELECT DISTINCT F70104 as Denumire FROM WT_" + tabela + " LEFT JOIN USERS ON F70102=USER_WS) a ORDER BY Denumire";

            try
            {
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }

        public DataTable GetCompName(string tabela)
        {
            DataTable q = null;

            string sql = "";



            if (Constante.tipBD == 2)
                sql = "SELECT ROWNUM AS \"Id\", \"Denumire\" FROM (SELECT DISTINCT COMPUTER_NAME as \"Denumire\" FROM WT_" + tabela + ") a ORDER BY \"Denumire\"";
            else
                sql = "SELECT CONVERT(int, ROW_NUMBER() OVER (ORDER BY Denumire)) AS Id, Denumire FROM (SELECT DISTINCT COMPUTER_NAME as Denumire FROM WT_" + tabela + ") a ORDER BY Denumire";

            try
            {
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return q;
        }



        public DataTable GetTrace(string tabela, string filtru)
        {
            DataTable q = null;

            string sql = "";

            string criptat = "";
            if (Constante.tipBD == 2)
                criptat = "case when (SELECT COUNT(*) CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from \"relGrupUser\" where \"IdUser\" = USER_WS) > 0 then 0  else  1 end ELSE  1   END AS CRIPTAT ";
            else
                criptat = "case when (SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from relGrupUser where IdUser = USER_WS) > 0 then 0  else 1  end  ELSE 1   END  AS CRIPTAT ";


            if (Constante.tipBD == 2)
                sql = "SELECT ROWNUM AS \"IdAuto\", Tabela, Nume_Camp, Cod_Op, VAL_NEW, VAL_OLD, TO_CHAR(Data, 'dd/mm/yyyy HH24:mi:ss') AS Data, USER_WIN, (SELECT F70104 FROM USERS WHERE F70102 = USER_WS) as USER_WS, COMPUTER_NAME, " + criptat + " FROM WT_" + tabela + " " + filtru + " ORDER BY DATA";
            else
                sql = "SELECT CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) AS IdAuto, Tabela, Nume_Camp, Cod_Op, VAL_NEW, VAL_OLD,  CONVERT(VARCHAR, Data, 103) + ' ' + CONVERT(VARCHAR, Data, 108) AS Data, USER_WIN, (SELECT F70104 FROM USERS WHERE F70102 = USER_WS) as USER_WS, COMPUTER_NAME, " + criptat + " FROM WT_" + tabela + " " + filtru + " ORDER BY DATA";

            try
            {
                q = General.IncarcaDT(sql, null);
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            foreach (DataColumn col in q.Columns)
                col.ReadOnly = false;

            for (int i = 0; i < q.Rows.Count; i++)
                if (q.Rows[i]["CRIPTAT"].ToString() == "1")
                    q.Rows[i]["USER_WS"] = DecryptUser(q.Rows[i]["USER_WS"].ToString());

            return q;
        }

        protected string DecryptUser(string user)
        {
            try
            {
                string userHex = "";
                for (int i = 0; i < user.Length; i++)
                {
                    if (i % 2 != 0 && i < user.Length - 1)
                        userHex += user[i] + " ";
                    else
                        userHex += user[i];
                }

                string[] param = userHex.Split(' ');
                byte[] ba = new byte[param.Length];
                for (int j = 0; j < param.Length; j++)
                {
                    int num = Int32.Parse(param[j], System.Globalization.NumberStyles.HexNumber);
                    ba[j] = (byte)(num ^ 0xf0);
                }

                string result = System.Text.Encoding.UTF8.GetString(ba);


                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }



    }


}