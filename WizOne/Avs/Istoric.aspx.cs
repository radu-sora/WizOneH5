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

namespace WizOne.Avs
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
                Dami.AccesApp();

                if (!IsPostBack)
                {
                    //string[] param = Session["IstoricDateContract"].ToString().Split(';');
                    string qwe = Convert.ToString(General.Nz(Request["qwe"], -99));
                    IncarcaGrid(Convert.ToInt32(Session["Marca"].ToString()), GetAtribut(qwe));
                }
                else
                {
                    DataTable dt = Session["Istoric_Grid"] as DataTable;
                    grDate.KeyFieldName = "NumeAngajat;DataModif";
                    grDate.DataSource = dt;
                    grDate.DataBind();
                }
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }
        }

        private int GetAtribut(string qwe)
        {          
            int atribut = -99;
            try
            {
                switch (qwe)
                {
                    case "btnSalariuIst":
                        atribut = (int)Constante.Atribute.Salariul;
                        break;
                    case "btnFuncIst":
                        atribut = (int)Constante.Atribute.Functie;
                        break;
                    case "btnCORIst":
                        atribut = (int)Constante.Atribute.CodCOR;
                        break;
                    case "btnMotivPlIst":
                        atribut = (int)Constante.Atribute.MotivPlecare;
                        break;
                    //case "btnOrgIst":
                    //    atribut = (int)Constante.Atribute.Organigrama;
                    //    break;
                    case "btnNormaIst":
                        atribut = (int)Constante.Atribute.Norma;
                        break;
                    case "btnCtrIntIst":
                        atribut = (int)Constante.Atribute.ContrIn;
                        break;
                    case "btnDataAngIst":
                        atribut = (int)Constante.Atribute.DataAngajarii;
                        break;
                    case "btnTitluAcadIst":
                        atribut = (int)Constante.Atribute.TitluAcademic;
                        break;
                    case "btnCCIst":
                        atribut = (int)Constante.Atribute.CentrulCost;
                        break;
                    case "btnPLIst":
                        atribut = (int)Constante.Atribute.PunctLucru;
                        break;
                    case "btnMeserieIst":
                        atribut = (int)Constante.Atribute.Meserie;
                        break;
                    case "btnNumeIst":
                    case "btnPrenumeIst":
                        atribut = (int)Constante.Atribute.NumePrenume;
                        break;
                    case "btnCASSIst":
                        atribut = (int)Constante.Atribute.CASS;
                        break;
                    case "btnStudiiIst":
                        atribut = (int)Constante.Atribute.Studii;
                        break;
                    case "btnContSalIst":
                        atribut = (int)Constante.Atribute.BancaSalariu;
                        break;
                    case "btnContGarIst":
                        atribut = (int)Constante.Atribute.BancaGarantii;
                        break;
                    //case "btnLimbiIst":
                    //    atribut = (int)Constante.Atribute.LimbiStraine;
                    //    break;
                    case "btnDocIdIst":
                        atribut = (int)Constante.Atribute.DocId;
                        break;
                    case "btnPermisIst":
                        atribut = (int)Constante.Atribute.PermisAuto;
                        break;
                }
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

            return atribut;
        }

        private void IncarcaGrid(int marca, int atribut)
        {
            try
            {
                //string data = "";
                //if (Constante.tipBD == 1)
                //{
                //    data = "CONVERT(VARCHAR, a.F10022, 103)";
                //}
                //else
                //{
                //    data = "TO_CHAR(a.F10022, 'dd/mm/yyyy')";
                //}
                string sql = "SELECT * FROM F100 WHERE F10003 = " + marca.ToString();
                DataTable dt = General.IncarcaDT(sql, null);
                string numeAng = dt.Rows[0]["F10008"].ToString() + " " + dt.Rows[0]["F10009"].ToString();

                sql = "SELECT * FROM F010";
                dt = General.IncarcaDT(sql, null);
                int luna = Convert.ToInt32(dt.Rows[0]["F01012"].ToString());
                int an = Convert.ToInt32(dt.Rows[0]["F01011"].ToString());

                List<int> lst = new List<int>(1);
                lst.Add(atribut);

                DataTable dtIst = GetIstoricDateContract(marca, lst, luna + 1 > 12 ? 1 : luna, luna + 1 > 12 ? an + 1 : an, numeAng, 0);

                //grDate.DataSource = null;
                dtIst.PrimaryKey = new DataColumn[] { dtIst.Columns["IdAuto"] };
                grDate.KeyFieldName = "IdAuto";
                grDate.DataSource = dtIst;
                grDate.DataBind();
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



        public DataTable GetIstoricDateContract(int F10003, List<int> lst, int luna, int an, string numeAng, int param)
        {
            DataTable dt = null;

            try
            {
                string strSql = "", strSqlComp = "";
                string camp100 = "", camp910 = "", camp910_1 = "", camp910_2 = "", campF704 = "", campTest = "";
                string campV = "", campN = "", numeAtr = "";
                string tabelaC = "", tabelaI = "";
                string sql_tmp = "";
                int nr = 0;

                string criptat = "";
                //if (Constante.tipBD == 2)
                //    criptat = "case when (SELECT COUNT(*) CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from \"relGrupUser\" where \"IdUser\" = F704.USER_NO) > 0 then 0  else  1 end ELSE  1   END AS CRIPTAT ";
                //else
                //    criptat = "case when (SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('RELGRUPUSER')) = 1 THEN  case when (select Count(*) from relGrupUser where IdUser = F704.USER_NO) > 0 then 0  else 1  end  ELSE 1   END  AS CRIPTAT ";
                if (Constante.tipBD == 2)
                    criptat = "case when (SELECT COUNT(*) AS CNT FROM user_tables WHERE UPPER(TABLE_NAME) LIKE UPPER('COMPACC')) = 1 THEN  case when (select Count(*) from compacc where F70203 = F704.USER_NO) > 0 or F704.USER_NO = 1 then 1 else 0  end  ELSE 0   END  AS CRIPTAT  ";
                else
                    criptat = "case when (SELECT COUNT(*) AS CNT FROM INFORMATION_SCHEMA.TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('COMPACC')) = 1 THEN  case when (select Count(*) from compacc where F70203 = F704.USER_NO) > 0 or F704.USER_NO = 1 then 1 else 0  end  ELSE 0   END  AS CRIPTAT  ";



                if (Constante.tipBD == 1)
                    strSqlComp = "select NumeAngajat, NumeAtribut, DataModif, ValV, ValN, Explicatii, CONVERT(INTEGER, ROW_NUMBER() OVER(order by DataModif desc)) AS IdAuto, TipCon, MotivSusp, DataInceput, DataSfarsit, DataIncetare, Utilizator, Data, CRIPTAT from (";
                else
                    strSqlComp = "select \"NumeAngajat\", \"NumeAtribut\", \"DataModif\", \"ValV\", \"ValN\", \"Explicatii\", TO_NUMBER(ROW_NUMBER() OVER (order by \"DataModif\" desc)) as \"IdAuto\", \"TipCon\", \"MotivSusp\", \"DataInceput\", \"DataSfarsit\", \"DataIncetare\", \"Utilizator\", \"Data\", CRIPTAT from (";



                string codValV704 = "", codValN = "", codValV100 = "", campValV910 = "", campValN910 = "", codValV910 = "", codValN910 = "", groupBy = "", cond = "";


                string conditieCOR = " case when (b.year < 2010 or (b.year = 2011 and b.month <12)) then 5 "
                        + "  when(b.year = 2011 and b.month = 12) or(b.year >= 2012 and b.year < 2016) or(b.year = 2016 and b.month < 4) "
                        + "  or(b.year = 2016 and b.month = 4 and coalesce(b.F910956, {0}) < {1}) then 6 "
                        + "  when(b.year = 2016 and b.month = 4 and coalesce(b.F910956, {0}) >= {1}) or(b.year = 2016 and b.month > 4) "
                        + "  or(b.year >= 2017 and b.year < 2019) or(b.year = 2019 and b.month < 6) then 7  else 8 end";
                conditieCOR = string.Format(conditieCOR, General.ToDataUniv(new DateTime(2100, 1, 1)), General.ToDataUniv(new DateTime(2016, 4, 17)));
                              
                string conditieCOR100 = "case when f100956 is null then (SELECT MAX(F72206) FROM F722) when F100956 < {0} then 5 when {0} <= f100956 and f100956 < {1} then 6 "
                                        + " when {1} <= f100956 and f100956 < {2} then 7 else 8 end";
                conditieCOR100 = string.Format(conditieCOR100, General.ToDataUniv(new DateTime(2011, 12, 1)), General.ToDataUniv(new DateTime(2016, 4, 17)), General.ToDataUniv(new DateTime(2019, 6, 1)));

                string test = "select CONVERT(DATETIME, '01/' + convert(varchar, b.MONTH) + '/' + convert(varchar, b.year) , 103) as DM,  {21} as ValV, "
                            + "{32} {22} As ValN {33} from "
                            + "(select a.year, a.month, a.F91003, a.F91098, b.F9101082, b.f910956 {23} "
                            + "from f910 a left join f9101 b on a.month = b.month and a.year = b.year and   a.F91003 = b.F91003) a "
                            + "left join "
                            + "(select a.year, a.month, a.F91003, a.F91098, b.F9101082, b.f910956 {23} from   f910 a    left "
                            + "join f9101 b on a.month = b.month and a.year = b.year and   a.F91003 = b.F91003) b on a.F91003 = b.f91003 "
                            + "where a.F91003 = " + F10003 + "  and DATEDIFF(month, CONVERT(DATETIME, '01/' + convert(varchar, a.MONTH) + '/' + convert(varchar, a.year) , 103), CONVERT(DATETIME, '01/' + convert(varchar, b.MONTH) + '/' + "
                            + "convert(varchar, b.year), 103)) = 1";
                if (Constante.tipBD == 2)
                    test = "select TO_DATE('01/' || TO_CHAR(b.MONTH) || '/' || TO_CHAR(b.year) , 103) as DM,  {21} as \"ValV\", "
                                + "{32} {22} As \"ValN\" {33} from "
                                + "(select a.year, a.month, a.F91003, a.F91098, b.F9101082, b.f910956 {23} "
                                + "from f910 a left join f9101 b on a.month = b.month and a.year = b.year and   a.F91003 = b.F91003) a "
                                + "left join "
                                + "(select a.year, a.month, a.F91003, a.F91098, b.F9101082, b.f910956 {23} from   f910 a    left "
                                + "join f9101 b on a.month = b.month and a.year = b.year and   a.F91003 = b.F91003) b on a.F91003 = b.f91003 "
                                + "where a.F91003 = " + F10003 + " and "
                                + "MONTHS_BETWEEN(TO_DATE('01/' || TO_CHAR(b.MONTH) || '/' || TO_CHAR(b.year), 'dd/mm/yyyy'), "
                                + "TO_DATE('01/' || TO_CHAR(a.MONTH) || '/' || TO_CHAR(a.year) , 'dd/mm/yyyy')) = 1";               

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.CodCOR)))
                {
                    codValV704 = " COALESCE(F1001082, {0}) AS \"CodValV\",";    //27
                    codValV704 = string.Format(codValV704, conditieCOR100);
                    codValN = "(SELECT MAX(F72206) FROM F722) AS \"CodValN\",";                         //28
                    codValV100 = "COALESCE(F9101082, {0}) AS \"CodValV\",";    //29
                    codValV100 = string.Format(codValV100, conditieCOR.Replace("b.year", "f910.year").Replace("b.month", "F910.month").Replace("b.F910956", "F9101.F910956"));
                    campValV910 = "\"CodValV\",";   //30
                    campValN910 = "\"CodValN\",";   //31
                    codValV910 = "COALESCE(a.F9101082, {0}) as \"CodValV\",";    //32
                    codValV910 = string.Format(codValV910, conditieCOR);
                    codValN910 = ", COALESCE(b.F9101082, {0}) AS \"CodValN\"";    //33
                    codValN910 = string.Format(codValN910, conditieCOR);
                    groupBy = " \"CodValV\", \"CodValN\",";   //34
                    cond = "or \"CodValV\" <> \"CodValN\"";  //35
                 
                }

                if (Constante.tipBD == 1)
                    sql_tmp = "select '{0}' AS NumeAngajat, '{1}' AS NumeAtribut, DataModif, {2} as ValV, {3} as ValN, Explicatii, CONVERT(INTEGER, ROW_NUMBER() OVER(order by DataModif desc)) AS IdAuto, '' AS TipCon, '' as MotivSusp, null as DataInceput,null as DataSfarsit, null as DataIncetare, Utilizator, Data, criptat from "
                            + "(select F70406 as DataModif, {4} As ValV, {27} {5} As ValN, {28} F70410 AS Explicatii, (SELECT F70104 FROM USERS WHERE F70102 = F704.USER_NO) AS Utilizator, F704.TIME AS Data, " + criptat
                            + "from f704 left join {6} on {6}.F10003 = F70403 left join F1001 on {6}.F10003 = F1001.F10003 where F70403 = {7} and F70404 = {8} AND F70406 >= CONVERT(DATETIME, '01/' + convert(varchar, {9}) + '/' + convert(varchar, {10}) , 103) and {11} <>  {12} "
                            + "union "
                            + "select CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, F01011) , 103) as DataModif, {13} As ValV, {29}  {14} As ValN,  {28} '' AS Explicatii, (SELECT F70104 FROM USERS WHERE F70102 = F100.USER_NO) AS Utilizator, F100.TIME AS Data, " + criptat.Replace("F704.USER_NO", "{15}.USER_NO")
                            + "from {15} left join f010 on 1=1 left join {16} on {15}.F10003 = {16}.F91003 left join F1001 on {15}.f10003 = f1001.f10003 left join f9101 on {16}.f91003 = f9101.f91003 where {15}.F10003 = {17} and  "
                            + "DATEDIFF(month, CONVERT(DATETIME, '01/' + convert(varchar, {16}.MONTH) + '/' + convert(varchar, {16}.year) , 103), "
                            + "CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, f01011) , 103)) = 1 and {18} <> {19} "
                            + "union "
                            + "select CONVERT(DATETIME, '01/' + convert(varchar, MONTH) + '/' + convert(varchar, year) , 103) as DataModif, ValV,  {30} "
                            + "ValN, {31} '' as explicatii, (SELECT F70104 FROM USERS WHERE F70102 = MAX({20}.USER_NO)) AS Utilizator, MAX(TIME) AS Data, " + criptat.Replace("F704.USER_NO", "MAX({20}.USER_NO)") + " from {20}, " 
                            + "(" + test + ") Test  where F91003 = {26} AND F91025 IN (0, 900, 999) "
                            + "and CONVERT(DATETIME, '01/' + convert(varchar, MONTH) + '/' + convert(varchar, year) , 103) = DM and (ValV <> ValN {35}) group by ValV, ValN, {34}"
                            + "CONVERT(DATETIME, '01/' + convert(varchar, MONTH) + '/' + convert(varchar, year) , 103)) t  {24} {25}   ";
                else
                    sql_tmp = "select '{0}' AS \"NumeAngajat\", '{1}' AS \"NumeAtribut\", TO_NUMBER(ROW_NUMBER() OVER (order by \"DataModif\" desc)) as \"IdAuto\", \"DataModif\", {2} as \"ValV\",  {3} as \"ValN\", \"Explicatii\",  '' AS \"TipCon\", '' as \"MotivSusp\", null as \"DataInceput\", null as \"DataSfarsit\", null as \"DataIncetare\", \"Utilizator\", \"Data\", CRIPTAT  from "
                            + "(select F70406 as \"DataModif\", {4} As \"ValV\", {27} {5} As \"ValN\", {28} F70410 AS \"Explicatii\", (SELECT F70104 FROM USERS WHERE F70102 = F704.USER_NO) AS \"Utilizator\", F704.TIME AS \"Data\", " + criptat
                            + "from f704 left join {6} on {6}.F10003 = F70403 left join F1001 ON {6}.F10003=F1001.F10003 where F70403 = {7} and F70404 = {8} AND F70406 >= TO_DATE('01/' || TO_CHAR({9}) || '/' || TO_CHAR({10}), 'dd/mm/yyyy') and {11} <> {12} "
                            + "union "
                            + "select TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(F01011), 'dd/mm/yyyy') as \"DataModif\", {13} As \"ValV\", {29}  {14} As \"ValN\", {28} '' AS \"Explicatii\", (SELECT F70104 FROM USERS WHERE F70102 = F100.USER_NO) AS \"Utilizator\", F100.TIME AS \"Data\", " + criptat.Replace("F704.USER_NO", "{15}.USER_NO")
                            + "from {15} left join f010 on 1=1 left join {16} on {15}.F10003 = {16}.F91003 left join F1001 on {15}.f10003 = f1001.f10003 left join f9101 on {16}.f91003 = f9101.f91003  where {15}.F10003 = {17} and  "
                            + "MONTHS_BETWEEN(TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(f01011) , 'dd/mm/yyyy'), "
                            + "TO_DATE('01/' || TO_CHAR({16}.MONTH) || '/' || TO_CHAR({16}.year) , 'dd/mm/yyyy')) = 1 and {18} <> {19} "
                            + "union "
                            + "select TO_DATE('01/' || TO_CHAR(MONTH) || '/' || TO_CHAR(year), 'dd/mm/yyyy') as \"DataModif\", \"ValV\",  {30} "
                            + "\"ValN\", {31} '' as \"Explicatii\", (SELECT F70104 FROM USERS WHERE F70102 = MAX({20}.USER_NO)) AS \"Utilizator\", MAX(TIME) AS \"Data\", " + criptat.Replace("F704.USER_NO", "MAX({20}.USER_NO)") + " from {20}, " 
                            + "(" + test + ") Test  where F91003 = {26} AND F91025 IN (0, 900, 999) "
                            + "and TO_DATE('01/' || TO_CHAR(MONTH) || '/' || TO_CHAR(year), 'dd/mm/yyyy') = DM and (\"ValV\" <> \"ValN\" {35}) group by \"ValV\", \"CodValV\", {34}"
                            + "TO_DATE('01/' || TO_CHAR(MONTH) || '/' || TO_CHAR(year), 'dd/mm/yyyy')) t   {24} {25}   ";


                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.Salariul)))
                {
                    string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                    string salariu_i = salariu.Replace("F100", "F910");
                    if (Constante.tipBD == 1)
                    {
                        camp100 = "CONVERT(INTEGER, " + salariu + ")";
                        camp910 = "CONVERT(INTEGER, " + salariu_i + ")";
                        camp910_1 = "CONVERT(INTEGER, a." + salariu_i + ")";
                        camp910_2 = "CONVERT(INTEGER, b." + salariu_i + ")";
                        campV = "CONVERT(VARCHAR, ValV)";
                        campN = "CONVERT(VARCHAR, ValN)";
                        campTest = ", " + salariu_i;
                    }
                    else
                    {
                        camp100 = "CAST(" + salariu + " AS INTEGER)";
                        camp910 = "CAST(" + salariu_i + " AS INTEGER)";
                        camp910_1 = "CAST(a." + salariu_i + " AS INTEGER)";
                        camp910_2 = "CAST(b." + salariu_i + " AS INTEGER)";
                        campV = "TO_CHAR(\"ValV\")";
                        campN = "TO_CHAR(\"ValN\")";
                    }
                    campF704 = "F70407";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    numeAtr = "Salariu";
                    nr = 1;

                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.Functie)))
                {
                    camp100 = "f10071";
                    camp910 = "f91071"; camp910_1 = "a.f91071"; camp910_2 = "b.f91071";
                    campF704 = "F70407";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",F91071 ";
                    nr = 2;
                    campV = "(select f71804 from f718 where f71802=\"ValV\")";
                    campN = "(select f71804 from f718 where f71802=\"ValN\")";
                    numeAtr = "Functia interna";

                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.CodCOR)))
                {
                    camp100 = "F10098";
                    camp910 = "F91098"; camp910_1 = "a.F91098"; camp910_2 = "b.F91098";
                    campF704 = "F70407";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = " ";
                    nr = 3;
                   
                    campV = "(select f72204 from f722 where f72202=\"ValV\" and F72206 = \"CodValV\")";
                    campN = "(select f72204 from f722 where f72202=\"ValN\" and F72206 = \"CodValN\")";
              
                    numeAtr = "COR";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.MotivPlecare)))
                {
                    camp100 = "f10025";
                    camp910 = "f91025"; camp910_1 = "a.f91025"; camp910_2 = "b.f91025";
                    campF704 = "F70407";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",F91025";
                    nr = 4;
                    //Florin 2019.09.30

                    //campV = "(select F09804 from F098, F721 where f09802=F72106 and F72102=\"ValV\")";
                    //campN = "(select F09804 from F098, F721 where f09802=F72106 and F72102=\"ValN\")";

                    campV = "(select TOP 1 F09804 from F098, F721 where f09802=F72106 and F72102=\"ValV\")";
                    campN = "(select TOP 1 F09804 from F098, F721 where f09802=F72106 and F72102=\"ValN\")";
                    if (Constante.tipBD == 2)
                    {
                        campV = "(select F09804 from F098, F721 where ROWNUM <= 1 AND f09802=F72106 and F72102=\"ValV\")";
                        campN = "(select F09804 from F098, F721 where ROWNUM <= 1 AND f09802=F72106 and F72102=\"ValN\")";
                    }

                    numeAtr = "Motiv plecare";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.Organigrama)))
                {
                    if (Constante.tipBD == 1)
                    {
                        camp100 = "convert(varchar, f10004) + '/' + convert(varchar, f10005) + '/' + convert(varchar, f10006) + '/' + convert(varchar, f10007)";
                        camp910 = "convert(varchar, f91004) + '/' + convert(varchar, f91005) + '/' + convert(varchar, f91006) + '/' + convert(varchar, f91007)";
                        camp910_1 = "convert(varchar, a.f91004) + '/' + convert(varchar, a.f91005) + '/' + convert(varchar, a.f91006) + '/' + convert(varchar, a.f91007)";
                        camp910_2 = "convert(varchar, b.f91004) + '/' + convert(varchar, b.f91005) + '/' + convert(varchar, b.f91006) + '/' + convert(varchar, b.f91007)";
                        campF704 = "convert(varchar, F70414) + '/' + convert(varchar, f70415) + '/' + convert(varchar, f70416) + '/' + convert(varchar, f70417)";
                        nr = 5;
                        campV = "((select F00305 from F003 where f00304=CONVERT(INTEGER, SUBSTRING(ValV, 1,CHARINDEX('/', ValV) - 1))) + ' / ' + "
                              + "(select F00406 from F004 where f00405=CONVERT(INTEGER, SUBSTRING(SUBSTRING(ValV, CHARINDEX('/', ValV) + 1, LEN(ValV) - CHARINDEX('/', ValV) - 1) "
                              + ", 1,CHARINDEX('/', SUBSTRING(ValV, CHARINDEX('/', ValV) + 1, LEN(ValV) - CHARINDEX('/', ValV) - 1)) - 1))) + ' / ' + "
                              + "(select F00507 from F005 where f00506=CONVERT(INTEGER, SUBSTRING(SUBSTRING(ValV, 1, len(ValV) - CHARINDEX('/', REVERSE(ValV))), "
                              + "len(SUBSTRING(ValV, 1, len(ValV) - CHARINDEX('/', REVERSE(ValV)))) - CHARINDEX('/', REVERSE(SUBSTRING(ValV, 1, len(ValV) - CHARINDEX('/', REVERSE(ValV))))) + 1 + 1 , "
                              + "len(SUBSTRING(ValV, 1, len(ValV) - CHARINDEX('/', REVERSE(ValV))))))) + ' / ' + "
                              + "(select F00608 from F006 where f00607=CONVERT(INTEGER, SUBSTRING(ValV,len(ValV) - CHARINDEX('/', REVERSE(ValV)) + 1 + 1 ,len(ValV)))))";
                        campN = "((select F00305 from F003 where f00304=CONVERT(INTEGER, SUBSTRING(ValN, 1,CHARINDEX('/', ValN) - 1))) + ' / ' + "
                              + "(select F00406 from F004 where f00405=CONVERT(INTEGER, SUBSTRING(SUBSTRING(ValN, CHARINDEX('/', ValN) + 1, LEN(ValN) - CHARINDEX('/', ValN) - 1) "
                              + ", 1,CHARINDEX('/', SUBSTRING(ValN, CHARINDEX('/', ValN) + 1, LEN(ValN) - CHARINDEX('/', ValN) - 1)) - 1))) + ' / ' + "
                              + "(select F00507 from F005 where f00506=CONVERT(INTEGER, SUBSTRING(SUBSTRING(ValN, 1, len(ValN) - CHARINDEX('/', REVERSE(ValN))), "
                              + "len(SUBSTRING(ValN, 1, len(ValN) - CHARINDEX('/', REVERSE(ValN)))) - CHARINDEX('/', REVERSE(SUBSTRING(ValN, 1, len(ValN) - CHARINDEX('/', REVERSE(ValN))))) + 1 + 1 , "
                              + "len(SUBSTRING(ValN, 1, len(ValN) - CHARINDEX('/', REVERSE(ValN))))))) + ' / ' + "
                              + "(select F00608 from F006 where f00607=CONVERT(INTEGER, SUBSTRING(ValN,len(ValN) - CHARINDEX('/', REVERSE(ValN)) + 1 + 1 ,len(ValN)))))";
                    }
                    else
                    {
                        camp100 = "f10004 || '/' || f10005 || '/' || f10006 || '/' || f10007";
                        camp910 = "f91004 || '/' || f91005 || '/' || f91006 || '/' || f91007";
                        camp910_1 = "a.f91004 || '/' || a.f91005 || '/' || a.f91006 || '/' || a.f91007";
                        camp910_2 = "b.f91004 || '/' || b.f91005 || '/' || b.f91006 || '/' || b.f91007";
                        campF704 = "F70414 || '/' || f70415 || '/' || f70416 || '/' || f70417";
                        nr = 5;
                        campV = "((select F00305 from F003 where f00304=TO_NUMBER(SUBSTR(\"ValV\", 0, INSTR(\"ValV\", '/')-1))) || ' / ' || "
                              + "(select F00406 from F004 where f00405=TO_NUMBER(SUBSTR(\"ValV\", INSTR(\"ValV\", '/')+1, INSTR(\"ValV\", '/', 1,2)-INSTR(\"ValV\", '/')-1))) || ' / ' || "
                              + "(select F00507 from F005 where f00506=TO_NUMBER(SUBSTR(\"ValV\", INSTR(\"ValV\", '/',1,2)+1, INSTR(\"ValV\", '/', 1,3)-INSTR(\"ValV\", '/',1,2)-1))) || ' / ' || "
                              + "(select F00608 from F006 where f00607=TO_NUMBER(SUBSTR(\"ValV\", INSTR(\"ValV\", '/',1,3)+1, LENGTH(\"ValV\")+1-INSTR(\"ValV\", '/',1,3)-1))))";
                        campN = "((select F00305 from F003 where f00304=TO_NUMBER(SUBSTR(\"ValN\", 0, INSTR(\"ValN\", '/')-1))) || ' / ' || "
                              + "(select F00406 from F004 where f00405=TO_NUMBER(SUBSTR(\"ValN\", INSTR(\"ValN\", '/')+1, INSTR(\"ValN\", '/', 1,2)-INSTR(\"ValN\", '/')-1))) || ' / ' || "
                              + "(select F00507 from F005 where f00506=TO_NUMBER(SUBSTR(\"ValN\", INSTR(\"ValN\", '/',1,2)+1, INSTR(\"ValN\", '/', 1,3)-INSTR(\"ValN\", '/',1,2)-1))) || ' / ' || "
                              + "(select F00608 from F006 where f00607=TO_NUMBER(SUBSTR(\"ValN\", INSTR(\"ValN\", '/',1,3)+1, LENGTH(\"ValN\")+1-INSTR(\"ValN\", '/',1,3)-1))))";
                    }
                    numeAtr = "Organigrama";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91004 , f91005 , f91006 , f91007";

                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                //if (lst.Count == 0 || lst.Contains(50))
                //{
                //    camp100 = "f10006";
                //    camp910 = "f91006"; camp910_1 = "a.f91006"; camp910_2 = "b.f91006";
                //    campF704 = "F70416";
                //    nr = 5;
                //    campV = "(select F00507 from F005 where f00506=\"ValV\")";
                //    campN = "(select F00507 from F005 where f00506=\"ValN\")";
                //    numeAtr = "Sectie";
                //    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                //       camp100, F10003.ToString(), camp910, camp100, camp910_1, camp910_2, F10003.ToString(), F10003.ToString());

                //    if (!strSqlComp.Contains(" union "))
                //        strSqlComp += strSql;
                //    else
                //        strSqlComp += " union " + strSql;                
                //}

                //if (lst.Count == 0 || lst.Contains(51))
                //{
                //    camp100 = "f10007";
                //    camp910 = "f91007"; camp910_1 = "a.f91007"; camp910_2 = "b.f91007";
                //    campF704 = "F70417";
                //    nr = 5;
                //    campV = "(select F00608 from F006 where f00607=\"ValV\")";
                //    campN = "(select F00608 from F006 where f00607=\"ValN\")";
                //    numeAtr = "Departament";
                //    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                //       camp100, F10003.ToString(), camp910, camp100, camp910_1, camp910_2, F10003.ToString(), F10003.ToString());

                //    if (!strSqlComp.Contains(" union "))
                //        strSqlComp += strSql;
                //    else
                //        strSqlComp += " union " + strSql; 
                //}

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.Norma)))
                {
                    camp100 = "F10043";
                    camp910 = "F91043"; camp910_1 = "a.F91043"; camp910_2 = "b.F91043";
                    campF704 = "F70407";
                    nr = 6;
                    if (Constante.tipBD == 1)
                    {
                        campV = "CONVERT(VARCHAR, ValV)";
                        campN = "CONVERT(VARCHAR, ValN)";
                    }
                    else
                    {
                        campV = "TO_CHAR(\"ValV\")";
                        campN = "TO_CHAR(\"ValN\")";
                    }
                    numeAtr = "Norma";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",F91043";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.ContrIn)))
                {
                    if (Constante.tipBD == 1)
                    {
                        camp100 = "f100985 + '/' + CONVERT(VARCHAR, F100986, 104)";
                        camp910 = "f910985 + '/' + CONVERT(VARCHAR, F910986, 104)"; camp910_1 = "a.f910985 + '/' + CONVERT(VARCHAR, a.F910986, 104)"; camp910_2 = "b.f910985 + '/' + CONVERT(VARCHAR, b.F910986, 104)";
                        campF704 = "CONVERT(VARCHAR, F70407) + '/' + CONVERT(VARCHAR, F70406, 104)";
                    }
                    else
                    {
                        camp100 = "f100985 || '/' || TO_CHAR(F100986, 'dd.mm.yyyy')";
                        camp910 = "f910985 || '/' || TO_CHAR(F910986, 'dd.mm.yyyy')"; camp910_1 = "a.f910985 || '/' || TO_CHAR(a.F910986, 'dd.mm.yyyy')"; camp910_2 = "b.f910985 || '/' || TO_CHAR(b.F910986, 'dd.mm.yyyy')";
                        campF704 = "TO_CHAR(F70407) || '/' || TO_CHAR(F70406, 'dd.mm.yyyy')";
                    }
                    nr = 8;
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f910985, f910986";
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Nr/data contract intern";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.CentrulCost)))
                {
                    camp100 = "f10053";
                    camp910 = "f91053"; camp910_1 = "a.f91053"; camp910_2 = "b.f91053";
                    campF704 = "F70407";
                    campTest = ",f91053";
                    nr = 14;
                    campV = "(select F06205 from F062 where f06204=\"ValV\")";
                    campN = "(select F06205 from F062 where f06204=\"ValN\")";
                    numeAtr = "Centru de cost";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.PunctLucru)))
                {
                    camp100 = "f10079";
                    camp910 = "f91079"; camp910_1 = "a.f91079"; camp910_2 = "b.f91079";
                    campF704 = "F70407";
                    nr = 16;
                    campV = "(select F08003 from F080 where f08002=\"ValV\")";
                    campN = "(select F08003 from F080 where f08002=\"ValN\")";
                    numeAtr = "Punct de lucru";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91079";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.Meserie)))
                {
                    camp100 = "f10029";
                    camp910 = "f91029"; camp910_1 = "a.f91029"; camp910_2 = "b.f91029";
                    campF704 = "F70407";
                    nr = 22;
                    campV = "(select F71704 from F717 where F71702=\"ValV\")";
                    campN = "(select F71704 from F717 where F71702=\"ValN\")";
                    numeAtr = "Meserie";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91029";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                //if (lst.Count == 0 || lst.Contains(Convert.ToInt32(srvAvans.Atribute.PrelungireCIM)))
                //{
                //    numeAtr = "Prelungire contract";
                //    if (Constante.tipBD == 1)
                //        strSql = "select '" + numeAng + "' AS NumeAngajat, '" + numeAtr + "' AS NumeAtribut, DataModif, '' as ValV, '' as ValN, '' as Explicatii, CONVERT(INTEGER, ROW_NUMBER() OVER(order by DataModif desc)) AS IdAuto, TipCon, '' as MotivSusp, DataInceput, null as DataSfarsit, DataIncetare from ( "
                //                + "select DataModif, '' as ValV, '' As ValN, '' AS Explicatii, case when convert(varchar, DataSfarsitCIM, 103) = '01/01/2100' THEN 'Nedeterminat' else 'Determinat' END as TipCon, " 
                //                + "DataInceputCIM as DataInceput, DataSfarsitCIM as DataIncetare, null As DataSfarsit, '' as MotivSusp "
                //                + "from Avs_Cereri, f100 where F100.F10003 = Avs_Cereri.F10003 and Avs_Cereri.F10003 = " + F10003.ToString() + " and Avs_Cereri.IdAtribut in (25,26) AND DataModif >= CONVERT(DATETIME, '01/' + convert(varchar, " + luna + ") + '/' + convert(varchar, " + an + ") , 103) and F100933 <>  DataInceputCIM "
                //                + "union "
                //                + "select CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, F01011) , 103) as DataModif, '' As ValV, '' As ValN, '' AS Explicatii, case when F1009741 = 1 then 'Nedeterminat' else 'Determinat' END as TipCon, "
                //                + "F100933 AS DataInceput, F100934 AS DataIncetare, null As DataSfarsit, '' as MotivSusp "
                //                + "from f100, f010, f095 where F10003 = " + F10003.ToString() + " and F10003 = F09503 and "
                //                + "DATEDIFF(month, CONVERT(DATETIME, '01/' + convert(varchar, DATEPART(month, F095.time)) + '/' + convert(varchar, DATEPART(year, f095.time)) , 103), "
                //                + "CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, f01011) , 103)) = 1 and F100933 <> F09505 "
                //                + "union " 
                //                + "SELECT Time as DataModif, '' as ValV, '' as ValN, '' as Explicatii, F09511 AS TipCon, "
                //                + "F09505 as DataInceput , F09506 as DataIncetare, null as DataSfarsit, '' as MotivSusp "
                //                + "FROM F095 WHERE F09503 = " + F10003.ToString() + ") t";
                //    else
                //        strSql = "select '" + numeAng + "' AS \"NumeAngajat\", '" + numeAtr + "' AS \"NumeAtribut\", TO_NUMBER(ROW_NUMBER() OVER (order by \"DataModif\" desc)) as \"IdAuto\", \"DataModif\", '' as \"ValV\", '' as \"ValN\", '' as \"Explicatii\",  \"TipCon\", '' as \"MotivSusp\", \"DataInceput\", null as \"DataSfarsit\", \"DataIncetare\"  from ("
                //                + "select  \"DataModif\", '' As \"ValV\", '' As \"ValN\", '' AS \"Explicatii\", case when TO_CHAR(\"DataSfarsitCIM\", 'dd/mm/yyyy') = '01/01/2100' THEN 'Nedeterminat' else 'Determinat' END as TipCon,  "
                //                + "\"DataInceputCIM\" as \"DataInceput\", \"DataSfarsitCIM\" as \"DataIncetare\", null As \"DataSfarsit\", '' AS \"MotivSusp\" "
                //                + "from \"Avs_Cereri\", f100 where F100.F10003 = \"Avs_Cereri\".F10003 and \"Avs_Cereri\".F10003 = " + F10003.ToString() + " and \"Avs_Cereri\".\"IdAtribut\" in (25,26) AND \"DataModif\" >= TO_DATE('01/' || TO_char(" + luna + ") || '/' || TO_char(" + an + ") , 'dd/mm/yyyy') and F100933 <>  \"DataInceputCIM\" "
                //                + "union "
                //                + "select TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(F01011), 'dd/mm/yyyy') as \"DataModif\", '' As \"ValV\", '' As \"ValN\", '' AS \"Explicatii\", case when F1009741 = 1 then 'Nedeterminat' else 'Determinat' END as \"TipCon\", "
                //                + "F100933 AS \"DataInceput\", F100934 AS \"DataIncetare\", null As \"DataSfarsit\", '' as \"MotivSusp\" "
                //                + "from f100, f010, f095 where F10003 = " + F10003.ToString() + " and F10003 = F09503 and "
                //                + "MONTHS_BETWEEN(TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(f01011) , 'dd/mm/yyyy'), "
                //                + "TO_DATE('01/' || TO_CHAR(EXTRACT(month FROM F095.TIME)) || '/' || TO_CHAR(EXTRACT(year FROM F095.TIME)) , 'dd/mm/yyyy')) = 1 and F100933 <> F09505 "
                //                + "union "  
                //                + "SELECT \"DataModif\", '' as \"ValV\", '' as \"ValN\", '' as \"Explicatii\", F09511 AS \"TipCon\", "
                //                + " F09505 as \"DataInceput\" , F09506 as \"DataIncetare\", null as \"DataSfarsit\", '' as \"MotivSusp\" "
                //                + "FROM F095 WHERE F09503 = " + F10003.ToString() + ") t";
                //    if (!strSqlComp.Contains(" union "))
                //        strSqlComp += strSql;
                //    else
                //        strSqlComp += " union " + strSql;                
                //}


                if (lst.Count == 0 || lst.Contains(Convert.ToInt32(Constante.Atribute.PrelungireCIM)))
                {
                    numeAtr = "Prelungire contract";
                    if (Constante.tipBD == 1)
                        strSql = "select '" + numeAng + "' AS NumeAngajat, '" + numeAtr + "' AS NumeAtribut, DataModif, '' as ValV, '' as ValN, '' as Explicatii, CONVERT(INTEGER, ROW_NUMBER() OVER(order by DataModif desc)) AS IdAuto, TipCon, '' as MotivSusp, DataInceput, null as DataSfarsit, DataIncetare from ( "
                                + "select CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, F01011) , 103) as DataModif, '' As ValV, '' As ValN, '' AS Explicatii, case when F1009741 = 1 then 'Nedeterminat' else 'Determinat' END as TipCon, "
                                + "F100933 AS DataInceput, F100934 AS DataIncetare, null As DataSfarsit, '' as MotivSusp "
                                + "from f100, f010, f095 where F10003 = " + F10003.ToString() + " and F10003 = F09503 and "
                                + "DATEDIFF(month, CONVERT(DATETIME, '01/' + convert(varchar, DATEPART(month, F095.time)) + '/' + convert(varchar, DATEPART(year, f095.time)) , 103), "
                                + "CONVERT(DATETIME, '01/' + convert(varchar, F01012) + '/' + convert(varchar, f01011) , 103)) = 1 and F100933 <> F09505 "
                                + "union "
                                + "SELECT Time as DataModif, '' as ValV, '' as ValN, '' as Explicatii, F09511 AS TipCon, "
                                + "F09505 as DataInceput , F09506 as DataIncetare, null as DataSfarsit, '' as MotivSusp "
                                + "FROM F095 WHERE F09503 = " + F10003.ToString() + ") t";
                    else
                        strSql = "select '" + numeAng + "' AS \"NumeAngajat\", '" + numeAtr + "' AS \"NumeAtribut\", TO_NUMBER(ROW_NUMBER() OVER (order by \"DataModif\" desc)) as \"IdAuto\", \"DataModif\", '' as \"ValV\", '' as \"ValN\", '' as \"Explicatii\",  \"TipCon\", '' as \"MotivSusp\", \"DataInceput\", null as \"DataSfarsit\", \"DataIncetare\"  from ("
                                + "select TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(F01011), 'dd/mm/yyyy') as \"DataModif\", '' As \"ValV\", '' As \"ValN\", '' AS \"Explicatii\", case when F1009741 = 1 then 'Nedeterminat' else 'Determinat' END as \"TipCon\", "
                                + "F100933 AS \"DataInceput\", F100934 AS \"DataIncetare\", null As \"DataSfarsit\", '' as \"MotivSusp\" "
                                + "from f100, f010, f095 where F10003 = " + F10003.ToString() + " and F10003 = F09503 and "
                                + "MONTHS_BETWEEN(TO_DATE('01/' || TO_CHAR(F01012) || '/' || TO_CHAR(f01011) , 'dd/mm/yyyy'), "
                                + "TO_DATE('01/' || TO_CHAR(EXTRACT(month FROM F095.TIME)) || '/' || TO_CHAR(EXTRACT(year FROM F095.TIME)) , 'dd/mm/yyyy')) = 1 and F100933 <> F09505 "
                                + "union "
                                + "SELECT \"DataModif\", '' as \"ValV\", '' as \"ValN\", '' as \"Explicatii\", F09511 AS \"TipCon\", "
                                + " F09505 as \"DataInceput\" , F09506 as \"DataIncetare\", null as \"DataSfarsit\", '' as \"MotivSusp\" "
                                + "FROM F095 WHERE F09503 = " + F10003.ToString() + ") t";
                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }


                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.NumePrenume))
                {
                    if (Constante.tipBD == 1)
                    {
                        camp100 = "F10008 + ' ' + F10009";
                        camp910 = "F91008 + ' ' + F91009"; camp910_1 = "a.f91008 + ' ' + a.f91009"; camp910_2 = "b.f91008 + ' ' + b.f91009";
                        campF704 = "Nume + ' ' + Prenume";
                    }
                    else
                    {
                        camp100 = "F10008 || ' ' || F10009";
                        camp910 = "F91008 || ' ' || F91009"; camp910_1 = "a.f91008 || ' ' || a.f91009"; camp910_2 = "b.f91008 || ' ' || b.f91009";
                        campF704 = "\"Nume\" || ' ' || \"Prenume\"";
                    }

                    nr = 101;
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Nume si prenume";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",F91008, F91009";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.CASS))
                {
                    camp100 = "f1003900";
                    camp910 = "f9103900"; camp910_1 = "a.f9103900"; camp910_2 = "b.f9103900";
                    campF704 = "F70407";
                    nr = 102;
                    campV = "(select F06303 from F063 where F06302=\"ValV\")";
                    campN = "(select F06303 from F063 where F06302=\"ValN\")";
                    numeAtr = "CASS";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f9103900";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.Studii))
                {
                    camp100 = "f10050";
                    camp910 = "f91050"; camp910_1 = "a.f91050"; camp910_2 = "b.f91050";
                    campF704 = "F70407";
                    nr = 103;
                    campV = "(select F71204 from F712 where F71202=\"ValV\")";
                    campN = "(select F71204 from F712 where F71202=\"ValN\")";
                    numeAtr = "Studii";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91050";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.BancaSalariu))
                {
                    camp100 = "f10020";
                    camp910 = "f91020"; camp910_1 = "a.f91020"; camp910_2 = "b.f91020";
                    campF704 = "F70431";
                    nr = 104;
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Cont salariu";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91020";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.BancaGarantii))
                {
                    camp100 = "F1001028";
                    camp910 = "F9101028"; camp910_1 = "a.F9101028"; camp910_2 = "b.F9101028";
                    campF704 = "F70435";
                    nr = 105;
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Cont garantii";
                    tabelaC = "F100";  // tabelaC = "F1001";
                    tabelaI = "F910";  // tabelaI = "F9101";
                    campTest = ",F9101028";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.DocId))
                {
                    camp100 = "f10052";
                    camp910 = "f91052"; camp910_1 = "a.f91052"; camp910_2 = "b.f91052";
                    campF704 = "SerieNrID";
                    if (Constante.tipBD == 2) campF704 = "\"SerieNrID\"";
                    nr = 107;
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Document identitate";
                    tabelaC = "F100";
                    tabelaI = "F910";
                    campTest = ",f91052";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                if (lst.Count == 0 || lst.Contains((int)Constante.Atribute.PermisAuto))
                {
                    camp100 = "F1001001";
                    camp910 = "F9101001"; camp910_1 = "a.F9101001"; camp910_2 = "b.F9101001";
                    campF704 = "NrPermis";
                    if (Constante.tipBD == 2) campF704 = "\"NrPermis\"";
                    nr = 108;
                    campV = "\"ValV\"";
                    campN = "\"ValN\"";
                    numeAtr = "Permis auto";
                    tabelaC = "F100"; // tabelaC = "F1001";
                    tabelaI = "F910"; // tabelaI = "F9101";
                    campTest = ",F9101001";
                    //strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                    //    camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, tabelaI, tabelaI, F10003.ToString(), F10003.ToString(),
                    //    codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    strSql = string.Format(sql_tmp, numeAng, numeAtr, campV, campN, camp100, campF704, tabelaC, F10003.ToString(), nr.ToString(), luna, an, camp100, campF704, camp910,
                        camp100, tabelaC, tabelaI, F10003.ToString(), camp910, camp100, tabelaI, camp910_1, camp910_2, campTest, "", "", F10003.ToString(),
                        codValV704, codValN, codValV100, campValV910, campValN910, codValV910, codValN910, groupBy, cond);

                    if (!strSqlComp.Contains(" union "))
                        strSqlComp += strSql;
                    else
                        strSqlComp += " union " + strSql;
                }

                strSqlComp += ") t";

                if (param == 1)
                {
                    string sqltmp = "SELECT \"NumeAngajat\", \"DataModif\",  CASE WHEN \"NumeAtribut\" = 'Salariu' THEN \"ValN\" ELSE NULL END AS \"Salariu\",  CASE WHEN \"NumeAtribut\" = 'Functia interna' THEN \"ValN\" ELSE NULL END AS \"Functie\", "
                        + " CASE WHEN \"NumeAtribut\" = 'COR' THEN \"ValN\" ELSE NULL END AS COR, CASE WHEN \"NumeAtribut\" = 'Organigrama' THEN \"ValN\" ELSE NULL END AS \"Departament\", CASE WHEN \"NumeAtribut\" = 'Nume si prenume' THEN \"ValN\" ELSE NULL END AS \"NumePrenume\" FROM (" + strSqlComp + ") u";
                    strSqlComp = sqltmp;

                    sqltmp = "SELECT \"NumeAngajat\", \"DataModif\", MAX(\"Salariu\") AS \"Salariu\", MAX(\"Functie\") AS \"Functie\", MAX(COR) AS COR, MAX(\"Departament\") AS \"Departament\", MAX(\"NumePrenume\") AS \"NumePrenume\" FROM (" + strSqlComp + ") v GROUP BY \"NumeAngajat\", \"DataModif\"";
                    strSqlComp = sqltmp;

                }

                dt = General.IncarcaDT(strSqlComp, null);
       
            }
            catch(Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);

            }

            foreach (DataColumn col in dt.Columns)
                col.ReadOnly = false;

            if (param == 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    if (dt.Rows[i]["CRIPTAT"].ToString() == "1")
                        dt.Rows[i]["Utilizator"] = DecryptUser(dt.Rows[i]["Utilizator"].ToString());
            }

            Session["Istoric_Grid"] = dt;

            return dt;
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



        //<dx:GridViewDataTextColumn FieldName = "TipCon" Name="TipCon" Caption="Tip contract" ReadOnly="true" Width="100px" />
        //<dx:GridViewDataTextColumn FieldName = "MotivSusp" Name="MotivSusp" Caption="Motiv suspendare" ReadOnly="true" Width="150px" />
        //<dx:GridViewDataDateColumn FieldName = "DataInceput" Name="DataInceput" Caption="Data inceput" ReadOnly="true" Width="100px" >
        //    <PropertiesDateEdit DisplayFormatString = "dd/MM/yyyy" ></ PropertiesDateEdit >
        //</ dx:GridViewDataDateColumn>
        //<dx:GridViewDataDateColumn FieldName = "DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" ReadOnly="true" Width="100px" >
        //    <PropertiesDateEdit DisplayFormatString = "dd/MM/yyyy" ></ PropertiesDateEdit >
        //</ dx:GridViewDataDateColumn>
        //<dx:GridViewDataDateColumn FieldName = "DataIncetare" Name="DataIncetare" Caption="Data incetare" ReadOnly="true" Width="100px" >
        //    <PropertiesDateEdit DisplayFormatString = "dd/MM/yyyy" ></ PropertiesDateEdit >
        //</ dx:GridViewDataDateColumn>



    }
}