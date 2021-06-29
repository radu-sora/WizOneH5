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

                GridViewDataDateColumn col = (grDate.Columns["ZileCalendaristice"] as GridViewDataDateColumn);
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
	        string szSQL, szT;

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

                szT += "DATEPART(MONTH,F94036) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(YEAR,F94036) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(MONTH,F94037) = DATEPART(MONTH,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                szT += "DATEPART(YEAR,F94037) = DATEPART(YEAR,DATEADD(mm,-1,CONVERT(DATETIME, '01-'+CONVERT(VARCHAR,F01012) + '-' + CONVERT(VARCHAR,F01011), 105))) AND ";
                
                szT += "F94003 = {0} AND F01002 = (SELECT F10002 FROM F100 WHERE F10003 = {0}) AND ";
                szT += "(F94010 IN (select CODE1 from mardef) OR F94010 IN (select CODE2 from mardef) OR F94010 IN (select CODE3 from mardef) OR F94010 IN (select CODE4 from mardef)) ";
                szT += "group by f94003, F94036, CONVERT(VARCHAR,F94037,103), CONVERT(VARCHAR,F94038,103), F940601 + ' ' + F940602, F940606 + ' ' + F940608 , F940612, F940613, F940614, F940620 ";

                szT += " UNION ";
                szT += "SELECT 100 + ROW_NUMBER() OVER (ORDER BY F30036) as \"IdAuto\", F30003 AS f94003, F30010 AS F94010, F02105 as TipCM, CONVERT(VARCHAR,F30038,103) AS DataSfarsit, CONVERT(VARCHAR,F30037,103) AS DataStart, CONVERT(VARCHAR,DATEDIFF(day, F30037, F30038)+1,103) AS ZileCalendaristice, F30013 AS ZileLucratoare, F30015 AS Suma, F300601 + ' ' + F300602 AS SerieNrCM, F300606  + ' ' +  F300608 AS SerieNrCMInit ";

                szT += ", F300612 AS BCCM, F300613 AS ZBCCM, F300614 AS MediaZilnica, F300620 AS MedieZileBazaCalcul, F30036 AS F94036 ";
               
                szT += " FROM F300, F021 ";
                szT += "WHERE F30010 = F02104 AND ";
                szT += "F30003 = {0} AND ";
                szT += "(F30010 IN (select CODE1 from mardef) OR F30010 IN (select CODE2 from mardef) OR F30010 IN (select CODE3 from mardef) OR F30010 IN (select CODE4 from mardef)) ";

                szT += " UNION ";

                szT += " SELECT 200 + ROW_NUMBER() OVER (ORDER BY DataCM) as \"IdAuto\", F10003 AS f94003, Cod AS F94010, TipConcediu as TipCM, CONVERT(VARCHAR,DataSfarsit,103) AS DataSfarsit, CONVERT(VARCHAR,DataInceput,103) AS ";
                szT += " DataStart, CONVERT(VARCHAR, DATEDIFF(day, DataInceput, DataSfarsit) + 1, 103) AS ZileCalendaristice, NrZile AS ZileLucratoare, Suma, SerieCM +' ' + NumarCM AS SerieNrCM, SerieCMInitial  +' ' + NumarCMInitial AS ";
                szT += " SerieNrCMInit, BazaCalculCM AS BCCM, ZileBazaCalculCM AS ZBCCM, MedieZilnicaCM AS MediaZilnica, MedieZileBazaCalcul, DataCM AS F94036 ";
                szT += " FROM CM_Cereri, F021 WHERE Cod = F02104 AND F10003 = 17 AND ";
                szT += " (Cod IN(select CODE1 from mardef) OR Cod IN(select CODE2 from mardef) OR Cod IN(select CODE3 from mardef) OR Cod IN(select CODE4 from mardef))  ";

                szT += "ORDER BY F94036 DESC ";                
             
            }     


            szSQL = string.Format(szT, marca);

            DataTable dtCM = General.IncarcaDT(szSQL, null);
            DataTable dt010 = General.IncarcaDT("SELECT F01012, F01011 FROM F010", null);
            int anLucru = Convert.ToInt32(dt010.Rows[0]["F01011"].ToString());
            int lunaLucru = Convert.ToInt32(dt010.Rows[0]["F01012"].ToString());

            DateTime odtDate = new DateTime(anLucru, lunaLucru, 1);

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
                            Session["BazaCalculCM"] = Convert.ToDouble(dtCM.Rows[i]["BCCM"].ToString());
                            Session["ZileBazCalcul"] = Convert.ToDouble(dtCM.Rows[i]["ZBCCM"].ToString());
                            Session["MediaZilnica"] = Convert.ToDouble(dtCM.Rows[i]["MediaZilnica"].ToString());

                            if (dtCM.Rows[i]["SerieNrCMInit"] != null && dtCM.Rows[i]["SerieNrCMInit"].ToString().Trim().Length > 0)
                            {
                                Session["SerieNrCMInitial"] = dtCM.Rows[i]["SerieNrCMInit"].ToString();
                            }
                            else
                            {
                                Session["SerieNrCMInitial"] = dtCM.Rows[i]["SerieNrCM"].ToString();
                            }
                        }

                    }

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