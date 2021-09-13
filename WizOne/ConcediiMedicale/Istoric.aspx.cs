using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Diagnostics;
using System.Text;

namespace WizOne.ConcediiMedicale
{
    public partial class Istoric : System.Web.UI.Page
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                if (Session["CM_HR"] == null || Session["CM_HR"].ToString() != "1")
                {
                    grDate.Columns["Suma"].Visible = false;
                    grDate.Columns["BCCM"].Visible = false;
                    grDate.Columns["ZBCCM"].Visible = false;
                    grDate.Columns["MediaZilnica"].Visible = false;
                }

                if (!IsPostBack)
                {
                    int marca = -99;
                    Session["ZileCMAnterior"] = 0;
                    if (Session["MarcaCM"] != null)
                        marca = Convert.ToInt32(Session["MarcaCM"].ToString());
                    IncarcaGrid(marca);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }
 

        private void IncarcaGrid(int marca)
        {
            try
            {

                DataTable dtIst = GetHistoricalCM(marca);
                
                dtIst.PrimaryKey = new DataColumn[] { dtIst.Columns["IdAuto"] };
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dtIst;
                grDate.DataBind();

                GridViewDataTextColumn col = (grDate.Columns["ZileCalendaristice"] as GridViewDataTextColumn);
                col.HeaderStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

        }

        protected void grDate_HtmlDataCellPrepared(object sender, ASPxGridViewTableDataCellEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }
        


        DataTable GetHistoricalCM(int marca)
        {
	        string szSQL = "", szT = "";
            string c606 = "-1", c608 = "-1", c601 = "-1", c602 = "-1";
            DataTable dtCM = new DataTable();

            try
            {

                szT = "SELECT F940606, F940608, F940601, F940602, F94036 ";

                szT += " FROM F940, F021, F010 ";
                szT += "WHERE F94010 = F02104 AND ";

                szT += "DATEPART(MONTH,F94036) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(YEAR,F94036) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(MONTH,F94037) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(YEAR,F94037) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "F94003 = {0} AND F01002 = (SELECT F10002 FROM F100 WHERE F10003 = {0}) AND ";
                szT += "(F94010 IN (select CODE1 from mardef) OR F94010 IN (select CODE2 from mardef) OR F94010 IN (select CODE3 from mardef) OR F94010 IN (select CODE4 from mardef)) ";
                szT += " UNION ";
                szT += "SELECT F300606 AS F940606, F300608 AS F940608, F300601 AS F940601, F300602 AS F940602, F30036 AS F94036 ";
                szT += " FROM F300, F021 ";
                szT += "WHERE F30010 = F02104 AND ";
                szT += "F30003 = {0} AND ";
                szT += "(F30010 IN (select CODE1 from mardef) OR F30010 IN (select CODE2 from mardef) OR F30010 IN (select CODE3 from mardef) OR F30010 IN (select CODE4 from mardef)) ";
                szT += "ORDER BY F94036 DESC ";

                szSQL = string.Format(szT, marca);
                DataTable dtIst = General.IncarcaDT(szSQL, null);


                if (dtIst != null && dtIst.Rows.Count > 0)
                {
                    c606 = (dtIst.Rows[0]["F940606"] == DBNull.Value ? "" : dtIst.Rows[0]["F940606"].ToString()).ToString();
                    c608 = (dtIst.Rows[0]["F940608"] == DBNull.Value ? "" : dtIst.Rows[0]["F940608"].ToString()).ToString();
                    if (c606.Length < 2)
                    {
                        c606 = dtIst.Rows[0]["F940601"].ToString();
                        c608 = dtIst.Rows[0]["F940602"].ToString();
                    }
                }

                szT = "";

                if (Constante.tipBD == 2)
                {
                    szT = "SELECT rownum as \"IdAuto\", f94003, max(94010), max(F02105) AS \"TipCM\", TO_CHAR(F94038, 'dd/mm/yyyy') AS \"DataSfarsit\", TO_CHAR(F94037, 'dd/mm/yyyy') AS \"DataStart\", TO_CHAR(SUM(F94038-F94037+1), 'dd/mm/yyyy') AS \"ZileCalendaristice\", "
                        + " SUM(F94013) as \"ZileLucratoare\", SUM(F94015) AS \"Suma\", F940601 || ' ' || F940602 as \"SerieNrCM\", F940606 || ' ' || F940608 AS \"SerieNrCMInit\" ";
                    szT += ", F940612 as BCCM, F940613 AS ZBCCM, F940614 as \"MediaZilnica\" ";
                    szT += "FROM F940, F021, F010 ";
                    szT += "WHERE F94010 = F02104 AND ";
                    szT += "TO_CHAR(F94036, 'MM-YYYY') = TO_CHAR(ADD_MONTHS(TO_DATE(F01012 || '-' || F01011, 'MM-YYYY'),-1), 'MM-YYYY') AND ";
                    szT += "TO_CHAR(F94037, 'MM-YYYY') = TO_CHAR(ADD_MONTHS(TO_DATE(F01012 || '-' || F01011, 'MM-YYYY'),-1), 'MM-YYYY') AND ";
                    szT += "F94003 = {0} AND F01002 = (SELECT F10002 FROM F100 WHERE F10003 = {0}) AND ";
                    szT += "(F94010 IN (select CODE1 from mardef) OR F94010 IN (select CODE2 from mardef) OR F94010 IN (select CODE3 from mardef) OR F94010 IN (select CODE4 from mardef)) ";
                    szT += "group by f94003, F94036, TO_CHAR(F94037, 'dd/mm/yyyy'), TO_CHAR(F94038, 'dd/mm/yyyy'), F940601 || ' ' || F940602, F940606 || ' ' || F940608 , F940612, F940613, F940614";
                    szT += " ORDER BY F94036 ";
                }

                else
                {
                    szT = "SELECT ROW_NUMBER() OVER (ORDER BY F94036) as \"IdAuto\", f94003, max(F94010) as F94010, max(F02105) AS \"TipCM\", CONVERT(VARCHAR,F94038,103) AS \"DataSfarsit\", CONVERT(VARCHAR,F94037,103) AS \"DataStart\", CONVERT(VARCHAR,SUM(DATEDIFF(day, F94037, F94038)+1),103) AS \"ZileCalendaristice\","
                        + " SUM(F94013) as \"ZileLucratoare\", SUM(F94015) AS \"Suma\", F940601 + ' ' + F940602 as \"SerieNrCM\", F940606 + ' ' + F940608 AS \"SerieNrCMInit\" ";

                    szT += ", F940612 as BCCM, F940613 AS ZBCCM, F940614 as \"MediaZilnica\", F940620 AS MedieZileBazaCalcul, F94036 ";

                    szT += "FROM F940, F021, F010 ";
                    szT += "WHERE F94010 = F02104 AND ";

                    //szT += "DATEPART(MONTH,F94036) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                    //szT += "DATEPART(YEAR,F94036) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                    //szT += "DATEPART(MONTH,F94037) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                    //szT += "DATEPART(YEAR,F94037) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";

                    szT += " ((F940606 = '" + c606 + "' AND F940608 = '" + c608 + "') OR (F940601 = '" + c606 + "' AND F940602 = '" + c608 + "')) AND ";

                    szT += "F94003 = {0} AND F01002 = (SELECT F10002 FROM F100 WHERE F10003 = {0}) AND ";
                    szT += "(F94010 IN (select CODE1 from mardef) OR F94010 IN (select CODE2 from mardef) OR F94010 IN (select CODE3 from mardef) OR F94010 IN (select CODE4 from mardef)) ";
                    szT += "group by f94003, F94036, CONVERT(VARCHAR,F94037,103), CONVERT(VARCHAR,F94038,103), F940601 + ' ' + F940602, F940606 + ' ' + F940608 , F940612, F940613, F940614, F940620 ";

                    szT += " UNION ";
                    szT += "SELECT 100 + ROW_NUMBER() OVER (ORDER BY F30036) as \"IdAuto\", F30003 AS f94003, F30010 AS F94010, F02105 as TipCM, CONVERT(VARCHAR,F30038,103) AS DataSfarsit, CONVERT(VARCHAR,F30037,103) AS DataStart, CONVERT(VARCHAR,DATEDIFF(day, F30037, F30038)+1,103) AS ZileCalendaristice, F30013 AS ZileLucratoare, F30015 AS Suma, F300601 + ' ' + F300602 AS SerieNrCM, F300606  + ' ' +  F300608 AS SerieNrCMInit ";

                    szT += ", F300612 AS BCCM, F300613 AS ZBCCM, F300614 AS MediaZilnica, F300620 AS MedieZileBazaCalcul, F30036 AS F94036 ";

                    szT += " FROM (SELECT F30003, min(F30010) as F30010, f30037, f30038, sum (F30013) as F30013, sum(F30015) as f30015, F300601, F300602, F300606, F300608, ";
                    szT += "  max(F300612) as f300612, max(f300613) as F300613, max(f300614) as f300614, max(f300620) as f300620, min(f30036) as f30036 from f300 where f30003 = {0} ";
                    szT += "  AND(F30010 IN (select CODE1 from mardef) OR F30010 IN (select CODE2 from mardef) OR F30010 IN (select CODE3 from mardef) OR F30010 IN (select CODE4 from mardef)) ";
                    szT += " group by F30003,F300601, F300602, F300606, F300608, f30037, f30038) as F300, F021 ";
                    szT += "WHERE F30010 = F02104  ";    

                    szT += " UNION ";

                    szT += " SELECT 200 + ROW_NUMBER() OVER (ORDER BY DataCM) as \"IdAuto\", F10003 AS f94003, Cod AS F94010, F02105 as TipCM, CONVERT(VARCHAR,DataSfarsit,103) AS DataSfarsit, CONVERT(VARCHAR,DataInceput,103) AS ";
                    szT += " DataStart, CONVERT(VARCHAR, DATEDIFF(day, DataInceput, DataSfarsit) + 1, 103) AS ZileCalendaristice, NrZile AS ZileLucratoare, Suma, SerieCM +' ' + NumarCM AS SerieNrCM, SerieCMInitial  +' ' + NumarCMInitial AS ";
                    szT += " SerieNrCMInit, BazaCalculCM AS BCCM, ZileBazaCalculCM AS ZBCCM, MedieZilnicaCM AS MediaZilnica, MedieZileBazaCalcul, DataCM AS F94036 ";
                    szT += " FROM CM_Cereri, F021 WHERE Cod = F02104 AND F10003 = {0} AND ";
                    szT += " (Cod IN(select CODE1 from mardef) OR Cod IN(select CODE2 from mardef) OR Cod IN(select CODE3 from mardef) OR Cod IN(select CODE4 from mardef)) and year(DataInceput) = (SELECT F01011 FROM F010) AND Month(DataInceput) = (SELECT F01012 FROM F010)   ";

                    szT += "ORDER BY F94036 DESC ";

                }


                szSQL = string.Format(szT, marca);

                 dtCM = General.IncarcaDT(szSQL, null);
                DataTable dt010 = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
                int anLucru = Convert.ToInt32(dt010.Rows[0]["F01011"].ToString());
                int lunaLucru = Convert.ToInt32(dt010.Rows[0]["F01012"].ToString());

                DateTime odtDate = new DateTime();
                if (Session["CM_StartDate"] != null)
                    odtDate = Convert.ToDateTime(Session["CM_StartDate"]);
                else
                    odtDate = new DateTime(anLucru, lunaLucru, 1);

                int nrZileConcPrec = 0;
                if (dtCM != null && dtCM.Rows.Count > 0)
                    for (int i = 0; i < dtCM.Rows.Count; i++)
                    {
                        //if (dtCM.Rows[i]["TipCM"].ToString().Contains("CM"))
                        //    dtCM.Rows[i]["TipCM"] = "CM boala (unitate/FNUASS)";

                        if (dtCM.Rows[i]["DataSfarsit"] != null && dtCM.Rows[i]["DataSfarsit"].ToString().Length >= 8)
                        {
                            if (dtCM.Rows[i]["SerieNrCMInit"] != null && dtCM.Rows[i]["SerieNrCMInit"].ToString().Trim().Length > 0)   //concediul curent e legat de precedentul; se aduna zilele
                                nrZileConcPrec += Convert.ToInt32(dtCM.Rows[i]["ZileCalendaristice"].ToString());
                            else                                                //concediul curent nu e legat de precedentul
                                nrZileConcPrec = Convert.ToInt32(dtCM.Rows[i]["ZileCalendaristice"].ToString());

                            DateTime odtBackDate = new DateTime(Convert.ToInt32(dtCM.Rows[i]["DataSfarsit"].ToString().Substring(6, 4)), Convert.ToInt32(dtCM.Rows[i]["DataSfarsit"].ToString().Substring(3, 2)), Convert.ToInt32(dtCM.Rows[i]["DataSfarsit"].ToString().Substring(0, 2)));


                            if (odtDate == odtBackDate.AddDays(1))
                            {
                                Session["ZileCMAnterior"] = nrZileConcPrec;
                                //Session["BazaCalculCM"] = Convert.ToDouble(dtCM.Rows[i]["BCCM"].ToString());
                                //Session["ZileBazCalcul"] = Convert.ToDouble(dtCM.Rows[i]["ZBCCM"].ToString());
                                //Session["MediaZilnica"] = Convert.ToDouble(dtCM.Rows[i]["MediaZilnica"].ToString());

                                //if (dtCM.Rows[i]["SerieNrCMInit"] != null && dtCM.Rows[i]["SerieNrCMInit"].ToString().Trim().Length > 0)
                                //{
                                //    Session["SerieNrCMInitial"] = dtCM.Rows[i]["SerieNrCMInit"].ToString();
                                //}
                                //else
                                //{
                                //    Session["SerieNrCMInitial"] = dtCM.Rows[i]["SerieNrCM"].ToString();
                                //}
                            }

                        }

                    }
                //Session["CM_Preluare"] = 1;

            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                return null;

            }
            return dtCM;
        }

        //protected void btnPreluare_Click(object sender, EventArgs e)
        protected void btnPreluare_Click(object sender, EventArgs e)
        {
            try
            {
                //if (Convert.ToInt32(General.Nz(Session["ZileCMAnterior"], "0").ToString()) == 0)
                //    ArataMesaj("Datele din certificatul medical nu pot fi preluate!\nNu exista certificat medical in ultima zi a lunii anterioare!");
                //grDate.JSProperties["cpAlertMessage"] = "Datele din certificatul medical nu pot fi preluate!\nNu exista certificat medical in ultima zi a lunii anterioare!";
                //MessageBox.Show("Datele din certificatul medical nu pot fi preluate!\nNu exista certificat medical in ultima zi a lunii anterioare!");
                //else
                {
                    List<object> lst = grDate.GetSelectedFieldValues(new string[] { "BCCM", "ZBCCM", "MediaZilnica", "SerieNrCM", "SerieNrCMInit", "DataStart", "MedieZileBazaCalcul" });
                    if (lst == null || lst.Count() == 0 || lst[0] == null)
                    {

                        grDate.JSProperties["cpAlertMessage"] = "Nu exista date selectate";
                        return;
                    }

                    object[] arr = lst[0] as object[];

                    Session["BazaCalculCM"] = Convert.ToDouble(arr[0].ToString());
                    Session["ZileBazaCalcul"] = Convert.ToDouble(arr[1].ToString());
                    Session["MediaZilnica"] = Convert.ToDouble(arr[2].ToString());
                    Session["MedieZileBazaCalculCM"] = Convert.ToDouble(arr[6].ToString());

                    //if (arr[4] != null && arr[4].ToString().Trim().Length > 0)
                    //{
                    //    Session["SerieNrCMInitial"] = arr[4].ToString();
                    //}
                    //else
                    {
                        Session["SerieNrCMInitial"] = arr[3].ToString();
                    }

                    Session["DataCMICalculCM"] = new DateTime(Convert.ToInt32(arr[5].ToString().Substring(6, 4)), Convert.ToInt32(arr[5].ToString().Substring(3, 2)), Convert.ToInt32(arr[5].ToString().Substring(0, 2)));

                    Session["CM_Preluare"] = 1;
                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close();", true);
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            //btnPreluare_Click();
        }

    }
}