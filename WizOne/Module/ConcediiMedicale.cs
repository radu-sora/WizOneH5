using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using DevExpress.Web;
using System.Globalization;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WizOne.Module
{

    public static class ConcediiMedicale
    {

        public struct CM
        {
            public int[] an;
            public int[] luna;
            public int[] zl;
            public int[] zileLucrate;
            public int[] nrzileCM;
            public int[] tzs;
            public string[] stComplet;
            public double[] total;
            public double[] bps;
            public double[] bpsRON;
            public double[] AMBP;
            public int tzsTotal;
            public double tzsTotal6;
            public int bpsTOTAL;
            public double bpsTOTAL6;
            public double bpsTOTALRON6;
            public double AMBP_Total;
            public double bpsTOTAL6e;
            public int Medie7;
            public int zl12;
        }


        public static void CreateDetails(int marca, string strCNP_marci, out double Medie6, out double BCCM, out int ZBCCM)
        {

            Medie6 = 0;
            BCCM = 0;
            ZBCCM = 0;
            try
            {

                double dblBPS, dblBPSRON, dblBPS6 = 0, dblBPSRON6 = 0, dblMedieZilnica, dblMedieZilnica6 /*dblCol11*/;
                bool calculez, bAllowRedistr;

                bool bAreStagiu;

                int nZileLunaCurentaPropusa;


                double Medie10;

                //'dblCol11 = 0
                double nTZS = 0, nTZS6 = 0, SumaZile12 = 0, SumaDblRON, SumaDbl;
                int nStageCount = 0;

                int nTmp, nTmp1, nTmpTZSLuna, TmpZileLucrate, TmpZileCM, TmpZileLuna, zileLucrate10 = 0, nrzileCM10 = 0, nrZileLuna10 = 0;

                string szTxtMedieZilnica6_10, szValMedieZilnica6_10, szTmpTxt;

          
                int[] zileLucrate = new int[13];
                int[] nrzileCM = new int[13];
                int[] nrzileLuna = new int[13], zileRedistribDebug= new int[13];
                bool[] bUseLine = new bool[13], bUseLineDebug = new bool[13];

                CM arCM = new CM();

                double[] BCCAS = new double[13];
                double[] VALCM = new double[13];
                double[] SumaZi = new double[13], SumaZiRON= new double[12], ardTOTAL = new double[13];

                int i, j, nLunaFractionata;
                int nZileLunaStart = 0, nZileLunaStart6;
                //nZileLunaStart6 = Day(dtTmp) - DateTime.Now.Day;
                //nZileLunaStart6 = CalculZL(nZileLunaStart6, dtStart6);

                // nZileLunaStart = nZileLunaStart6;

                string szBazaCMFUNASS = "", szBazaCMAMBP, szNN, szPP, szZAMBP, szCASTOT, szBCCAS;
                string szsql = "";
                int nZileLunaCurenta = 0, month, year, nchkMedie6Luni = 1;       
                int zileDiff_Stagiu_ZL = 0, zileDiff = 0, nStageCount_ZL_10 = 0, nStageCount_TZS_10 = 0, nStageCount_ZL_6 = 0, nStageCount_TZS_6 = 0;
                bool bHasDiffZL = false;

                int zileLucrate6 = 0, nrzileCM6 = 0, nrZileLuna6 = 0;

                //if (Day(dtCurenta) > 1)
                //{
                //    nZileLunaCurenta = Day(dtCurenta);
                //    szTmp = "01" & "/" & strLuna & "/" & strAn;
                //    dtTmp = DateValue(szTmp);
                //    nZileLunaCurenta = CalculZL(nZileLunaCurenta, dtTmp);
                //}
                //else
                //{
                    nZileLunaCurenta = 0;
                //}

                Dictionary<String, String> lista = LoadParameters();
                if (lista.ContainsKey("BAZA_CMFNUASS"))
                    szBazaCMFUNASS = lista["BAZA_CMFNUASS"].Replace("F200", "F920");

                if (lista.ContainsKey("BAZA_CMAMBP"))
                    szBazaCMAMBP = lista["BAZA_CMAMBP"].Replace("F200", "F920");

                szNN = lista["NN"].Replace("F200", "F920");
                szPP = lista["PP"].Replace("F200", "F920");

                if (lista.ContainsKey("ZAMBP2") && lista["ZAMBP2"].Length > 0)
                {
                    szZAMBP = lista["ZAMBP2"].Replace("F200", "F920");
                    szPP += " + " + szZAMBP;
                }

                if (lista.ContainsKey("ZAMBP") && lista["ZAMBP"].Length > 0)
                {
                    szZAMBP = lista["ZAMBP"].Replace("F200", "F920");
                    szPP += " + " + szZAMBP;
                }

                szCASTOT = lista["BASS"].Replace("F200", "F920");

                string sql = " SELECT F01011, F01012 FROM F010";
                DataTable dt010 = General.IncarcaDT(sql, null);
                int anC = Convert.ToInt32(dt010.Rows[0][0].ToString());
                int lunaC = Convert.ToInt32(dt010.Rows[0][1].ToString());

                month = Convert.ToInt32(dt010.Rows[0][1].ToString()) - 1;
                year = Convert.ToInt32(dt010.Rows[0][0].ToString());


                if (Constante.tipBD == 2)
                {
                    szsql = "SELECT COUNT(*) AS CNT FROM USER_TABLES WHERE TABLE_NAME = 'TMP_" + marca + "_CM'";
                    DataTable dtTemp = General.IncarcaDT(szsql, null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && Convert.ToInt32(dtTemp.Rows[0][0].ToString()) > 0)
                    {
                        szsql = "TRUNCATE TABLE TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
                    }
                    else
                    {
                        szsql = "CREATE GLOBAL TEMPORARY TABLE TMP_" + marca + "_CM";
                        szsql += " (LUNA NUMBER(10,0),";
                        szsql += " AN NUMBER(10,0),";
                        szsql += " SUM1_NN NUMBER(22,5),";
                        szsql += " SUM2_PP NUMBER(22,5),";
                        szsql += " SUM3_BCCAS_2017 NUMBER(22,5),";
                        szsql += " SUM3_BCCAS_2018 NUMBER(22,5),";
                        szsql += " SUM4_CASTOT_2017 NUMBER(22, 5),";
                        szsql += " SUM4_CASTOT_2018 NUMBER(22, 5)";
                        szsql += " ) ON COMMIT PRESERVE ROWS";
                        General.ExecutaNonQuery(szsql, null);
                    }

                    szsql = "INSERT INTO TMP_" + marca + "_CM (LUNA, AN, SUM1_NN, SUM2_PP, SUM3_BCCAS_2017, SUM3_BCCAS_2018, SUM4_CASTOT_2017, SUM4_CASTOT_2018)";
                    szsql += " SELECT F06905, F06904, 0, 0, 0, 0, 0, 0 FROM";
                    szsql += " (SELECT ROW_NUMBER() OVER (ORDER BY F06904 DESC, F06905 DESC) AS RN, F06905, F06904";
                    szsql += " From F069, F010";
                    szsql += " WHERE F06904 <= F01011 AND F06905 <= (CASE WHEN F01011 = F06904 THEN F01012 else 12 END)) t";
                    szsql += " WHERE RN > 1 AND RN < 14";
                    General.ExecutaNonQuery(szsql, null);
                }
                else
                {
                    szsql = "SELECT COUNT(*) AS CNT FROM tempdb.sys.objects where name like '#TMP_" + marca + "_CM%'";
                    DataTable dtTemp = General.IncarcaDT(szsql, null);
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && Convert.ToInt32(dtTemp.Rows[0][0].ToString()) > 0)
                    {
                        szsql = "TRUNCATE TABLE #TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
                        szsql = "DROP TABLE #TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
                    }

                    szsql = "CREATE TABLE #TMP_" + marca + "_CM";
                    szsql += " (LUNA NUMERIC(10,0),";
                    szsql += " AN NUMERIC(10,0),";
                    szsql += " SUM1_NN NUMERIC(22,5),";
                    szsql += " SUM2_PP NUMERIC(22,5),";
                    szsql += " SUM3_BCCAS_2017 NUMERIC(22,5),";
                    szsql += " SUM3_BCCAS_2018 NUMERIC(22,5),";
                    szsql += " SUM4_CASTOT_2017 NUMERIC(22, 5),";
                    szsql += " SUM4_CASTOT_2018 NUMERIC(22, 5)";
                    szsql += " )";


                    General.ExecutaNonQuery(szsql, null);

                    szsql = "INSERT INTO #TMP_" + marca + "_CM (LUNA, AN, SUM1_NN, SUM2_PP, SUM3_BCCAS_2017, SUM3_BCCAS_2018, SUM4_CASTOT_2017, SUM4_CASTOT_2018)";
                    szsql += " SELECT F06905, F06904, 0, 0, 0, 0, 0, 0 FROM";
                    szsql += " (SELECT ROW_NUMBER() OVER (ORDER BY F06904 DESC, F06905 DESC) AS RN, F06905, F06904";
                    szsql += " From F069, F010";
                    szsql += " WHERE F06904 <= F01011 AND F06905 <= (CASE WHEN F01011 = F06904 THEN F01012 else 12 END)) t";
                    szsql += " WHERE RN > 1 AND RN < 14";


                    General.ExecutaNonQuery(szsql, null);
                }

                string szBCCAS_2017, szCASTOT_2017 = "", szBCCAS_2018, szCASTOT_2018 = "";
                if (szBazaCMFUNASS.Length > 4)
                {
                    szBCCAS = " 0 ";
                    szCASTOT = szBazaCMFUNASS;
                    szBCCAS_2017 = " 0 ";
                    szBCCAS_2018 = " 0 ";
                    szCASTOT_2017 = szBazaCMFUNASS;
                    szCASTOT_2018 = szBazaCMFUNASS;
                }
                else
                {
                    szsql = "SELECT F80003 FROM F800 WHERE F80002 = 'CASIGM'";
                    DataTable dtTemp = General.IncarcaDT(szsql, null);

                    szsql = "0";
                    if (dtTemp != null && dtTemp.Rows.Count > 0 && dtTemp.Rows[0][0] != null && dtTemp.Rows[0][0].ToString().Length > 0)
                        szsql = dtTemp.Rows[0][0].ToString();


                    szsql = "SELECT F01311 FROM F013 WHERE F01304=" + szsql;
                    dtTemp = General.IncarcaDT(szsql, null);

                    szBCCAS_2018 = "F92004" + (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) >= 41 ? (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) + 59).ToString() : "0" + (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) + 59).ToString());

                    szsql = "SELECT MAX(F901311) AS F01311 FROM F9013 WHERE F901304=" + lista["SOMA"];
                    dtTemp = General.IncarcaDT(szsql, null);

                    szBCCAS_2017 = "F92004" + (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) >= 41 ? (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) + 59).ToString() : "0" + (Convert.ToInt32(dtTemp.Rows[0]["F01311"].ToString()) + 59).ToString());
                }


                szCASTOT_2017 = szCASTOT_2017.Replace("F92004309", "F92004231");
                if (szCASTOT_2017.Contains("F92004231"))
                {
                    szBCCAS_2017 = szBCCAS_2017.Replace("+F92004231", "");
                    szBCCAS_2017 = szBCCAS_2017.Replace("+ F92004231", "");
                    szBCCAS_2017 = szBCCAS_2017.Replace("F92004231", "0");
                }

                szCASTOT_2018 = szCASTOT_2018.Replace("F92004231", "F92004309");
                if (szCASTOT_2018.Contains("F92004309"))
                {
                    szBCCAS_2018 = szBCCAS_2018.Replace("+F92004309", "");
                    szBCCAS_2018 = szBCCAS_2018.Replace("+ F92004309", "");
                    szBCCAS_2018 = szBCCAS_2018.Replace("F92004309", "0");
                }

                if (Constante.tipBD == 2)
                {
                    szsql = "MERGE INTO TMP_" + marca + "_CM USING (SELECT MONTH, YEAR, SUM(" + szNN + ") AS S1, SUM(" + szPP + ") AS S2, ";
                    szsql += " SUM(" + szBCCAS_2017 + ") AS S37, SUM(" + szBCCAS_2018 + ") AS S38, SUM(" + szCASTOT_2017 + ") AS S47, SUM(" + szCASTOT_2018 + ") AS S48 ";
                    szsql += " FROM F920 WHERE F92003 IN (" + strCNP_marci + ") GROUP BY MONTH, YEAR) ta ON (ta.MONTH = LUNA AND ta.YEAR = AN) ";
                    szsql += " WHEN MATCHED THEN ";
                    szsql += " UPDATE SET SUM1_NN = S1, SUM2_PP = S2, SUM3_BCCAS_2017 = S37, SUM4_CASTOT_2017 = S47, SUM3_BCCAS_2018 = S38, SUM4_CASTOT_2018 = S48";
                    General.ExecutaNonQuery(szsql, null);

                    szsql = "MERGE INTO TMP_" + marca + "_CM USING (SELECT MONTH, YEAR, SUM(ZILE_NN) AS S1, SUM(ZILE_PP) AS S2, SUM(BAZA_MEDIE_CM)  As S4 ";
                    szsql += " FROM F9_ISTORICVENITURI WHERE MARCA IN (" + strCNP_marci + ") GROUP BY MONTH, YEAR) ta ON (ta.MONTH = LUNA AND ta.YEAR = AN) ";
                    szsql += " WHEN MATCHED THEN UPDATE SET SUM1_NN = SUM1_NN + S1, SUM2_PP = SUM2_PP + S2, ";
                    szsql += " SUM4_CASTOT_2017 = SUM4_CASTOT_2017 + S4, SUM4_CASTOT_2018 = SUM4_CASTOT_2018 + S4";
                    General.ExecutaNonQuery(szsql, null);
                }
                else
                {
                    szsql = ";MERGE #TMP_" + marca + "_CM USING (SELECT MONTH, YEAR, SUM(" + szNN + ") AS S1, SUM(" + szPP + ") AS S2, ";
                    szsql += " SUM(" + szBCCAS_2017 + ") AS S37, SUM(" + szBCCAS_2018 + ") AS S38, SUM(" + szCASTOT_2017 + ") AS S47, SUM(" + szCASTOT_2018 + ") AS S48 ";
                    szsql += " FROM F920 WHERE F92003 IN (" + strCNP_marci + ") GROUP BY MONTH, YEAR) ta ON (ta.MONTH = LUNA AND ta.YEAR = AN) ";
                    szsql += " WHEN MATCHED THEN ";
                    szsql += " UPDATE SET SUM1_NN = S1, SUM2_PP = S2, SUM3_BCCAS_2017 = S37, SUM4_CASTOT_2017 = S47, SUM3_BCCAS_2018 = S38, SUM4_CASTOT_2018 = S48;";
                    General.ExecutaNonQuery(szsql, null);

                    szsql = ";MERGE #TMP_" + marca + "_CM USING (SELECT MONTH, YEAR, SUM(ZILE_NN) AS S1, SUM(ZILE_PP) AS S2, SUM(BAZA_MEDIE_CM)  As S4 ";
                    szsql += " FROM F9_ISTORICVENITURI WHERE MARCA IN (" + strCNP_marci + ") GROUP BY MONTH, YEAR) ta ON (ta.MONTH = LUNA AND ta.YEAR = AN) ";
                    szsql += " WHEN MATCHED THEN UPDATE SET SUM1_NN = SUM1_NN + S1, SUM2_PP = SUM2_PP + S2, ";
                    szsql += " SUM4_CASTOT_2017 = SUM4_CASTOT_2017 + S4, SUM4_CASTOT_2018 = SUM4_CASTOT_2018 + S4;";
                    General.ExecutaNonQuery(szsql, null);
                }


                calculez = false;

                arCM.an = new int[13];
                arCM.luna = new int[13];
                arCM.zl = new int[13];
                arCM.zileLucrate = new int[13];
                arCM.nrzileCM = new int[13];
                arCM.tzs = new int[13];
                arCM.stComplet = new string[13];
                arCM.total = new double[13];
                arCM.bps = new double[13];
                arCM.bpsRON = new double[13];
                arCM.AMBP = new double[13];


                long zile_lucrate_12 = 0, zile_lucratoare_6 = 0, zile_lucratoare_ul = 0;
                int nrluni_stagiu;

                if ((anC == 2018 && lunaC >= 7) || anC > 2018)
                    nrluni_stagiu = 6;
                else
                    nrluni_stagiu = 1;

               
                for (i = 12; i >= 1; i--)
                {
                    if (month == 0)
                    {
                        month = 12;
                        year = year - 1;
                    }
                    arCM.luna[i] = month;
                    arCM.an[i] = year;

                    nTmp = 0;
                    DataTable dt069 = General.IncarcaDT("SELECT F06907 FROM F069 WHERE F06904=" + year + " AND F06905=" + month, null);
                    if (dt069 != null && dt069.Rows.Count > 0 && dt069.Rows[0][0] != null && dt069.Rows[0][0].ToString().Length > 0)
                        nTmp = Convert.ToInt32(dt069.Rows[0][0].ToString());



                    if (nTmp == 0)
                    {
                        DateTime dtData = new DateTime(year, month, 1);
                        nTmp = CalculZLLuna(dtData);
                    }


                    if (Constante.tipBD == 2)
                    {
                        szsql = "SELECT SUM1_NN, SUM2_PP, CASE WHEN AN < 2018 THEN SUM3_BCCAS_2017 else SUM3_BCCAS_2018 END, CASE WHEN AN < 2018 THEN SUM4_CASTOT_2017 else SUM4_CASTOT_2018 END ";
                        szsql += " FROM TMP_" + marca + "_CM WHERE AN = " + year + " AND LUNA = " + month;
                    }
                    else
                    {
                        szsql = "SELECT SUM1_NN, SUM2_PP, CASE WHEN AN < 2018 THEN SUM3_BCCAS_2017 else SUM3_BCCAS_2018 END, CASE WHEN AN < 2018 THEN SUM4_CASTOT_2017 else SUM4_CASTOT_2018 END ";
                        szsql += " FROM #TMP_" + marca + "_CM WHERE AN = " + year + " AND LUNA = " + month;
                    }

                    DataTable dt = General.IncarcaDT(szsql, null);

                    switch (i)
                    {
                        case 6:
                            zileLucrate6 = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            nrzileCM6 = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);
                            nrZileLuna6 = nTmp;

                            zileLucrate[i] = zileLucrate6;
                            nrzileCM[i] = nrzileCM6;


                            BCCAS[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][2] != null && dt.Rows[0][2].ToString().Length > 0 ? Convert.ToDouble(dt.Rows[0][2].ToString()) : 0);
                            VALCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][3] != null && dt.Rows[0][3].ToString().Length > 0 ? Convert.ToDouble(dt.Rows[0][3].ToString()) : 0);

                            if (nZileLunaCurenta == 0 || nchkMedie6Luni == 2)
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nTmp;
                                zileLucrate[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                                nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);
                            }
                            else
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nTmp;
                                if (BCCAS[i] + VALCM[i] > 0)
                                    zileLucrate[i] = (nZileLunaStart <= (zileLucrate6 + nrzileCM6) ? nZileLunaStart : (zileLucrate6 + nrzileCM6));
                                else
                                    zileLucrate[i] = 0;

                                nrzileCM[i] = 0;
                            }


                            if (zileLucrate[i] + nrzileCM[i] > nTmp)
                                zileLucrate[i] = nTmp - nrzileCM[i];

                            if (i > 6)
                                zile_lucratoare_6 = nTmp + zile_lucratoare_6;

                            if (i > 11)
                                zile_lucratoare_ul = nTmp + zile_lucratoare_ul;

                            zile_lucrate_12 = zileLucrate[i] + nrzileCM[i] + zile_lucrate_12;

                            arCM.zileLucrate[i] = zileLucrate[i];
                            arCM.nrzileCM[i] = nrzileCM[i];
                            arCM.tzs[i] = zileLucrate[i] + nrzileCM[i];
                            arCM.stComplet[i] = arCM.zl[i] == arCM.tzs[i] ? "DA" : arCM.zl[i] > arCM.tzs[i] ? "NU" : "DA (Err)";

                            nTmp = zileLucrate6 + nrzileCM6;
                            if (nTmp != 0)
                                SumaZi[i] = (BCCAS[i] + VALCM[i]) / nTmp;
                            else
                                SumaZi[i] = 0;

                            if (nZileLunaCurenta == 0 || nchkMedie6Luni == 2)
                                arCM.total[i] = (int)(BCCAS[i] + VALCM[i]);
                            else
                                arCM.total[i] = (int)(zileLucrate[i] * SumaZi[i]);
                            break;
                        case 2:
                            zileLucrate10 = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            nrzileCM10 = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);
                            nrZileLuna10 = nTmp;


                            zileLucrate[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);


                            BCCAS[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][2] != null && dt.Rows[0][2].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][2].ToString()) : 0);
                            VALCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][3] != null && dt.Rows[0][3].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][3].ToString()) : 0);

                            if (nZileLunaCurenta == 0 || nchkMedie6Luni == 1)
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nTmp;
                                zileLucrate[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                                nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            }
                            else
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nTmp;
                                if (BCCAS[i] + VALCM[i] > 0)
                                    zileLucrate[i] = (nZileLunaStart <= (zileLucrate10 + nrzileCM10) ? nZileLunaStart : (zileLucrate10 + nrzileCM10));
                                else
                                    zileLucrate[i] = 0;

                                nrzileCM[i] = 0;
                            }


                            if (zileLucrate[i] + nrzileCM[i] > nTmp)
                                zileLucrate[i] = nTmp - nrzileCM[i];

                            if (i > 6)
                                zile_lucratoare_6 = nTmp + zile_lucratoare_6;

                            if (i > 11)
                                zile_lucratoare_ul = nTmp + zile_lucratoare_ul;

                            zile_lucrate_12 = zileLucrate[i] + nrzileCM[i] + zile_lucrate_12;

                            arCM.zileLucrate[i] = zileLucrate[i];
                            arCM.nrzileCM[i] = nrzileCM[i];


                            arCM.tzs[i] = zileLucrate[i] + nrzileCM[i];
                            arCM.stComplet[i] = arCM.zl[i] == arCM.tzs[i] ? "DA" : arCM.zl[i] > arCM.tzs[i] ? "NU" : "DA (Err)";


                            nTmp = zileLucrate10 + nrzileCM10;
                            if (nTmp != 0)
                                SumaZi[i] = (BCCAS[i] + VALCM[i]) / nTmp;
                            else

                                SumaZi[i] = 0;

                            if (nZileLunaCurenta == 0 || nchkMedie6Luni == 1)
                                arCM.total[i] = (int)(BCCAS[i] + VALCM[i]);
                            else
                                arCM.total[i] = (int)(zileLucrate[i] * SumaZi[i]);

                            break;
                        case 12:
                            zileLucrate[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);

                            if (nZileLunaCurenta == 0)
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nTmp;
                                nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);
                            }
                            else
                            {
                                arCM.zl[i] = nTmp;
                                nrzileLuna[i] = nZileLunaCurenta; // nZileLunaCurentaPropusa;
                                zileLucrate[i] = nZileLunaCurenta; // nZileLunaCurentaPropusa;
                                nrzileCM[i] = 0;
                            }


                            if (zileLucrate[i] + nrzileCM[i] > nTmp)
                                zileLucrate[i] = nTmp - nrzileCM[i];

                            if (i > 6)
                                zile_lucratoare_6 = nTmp + zile_lucratoare_6;

                            if (i > 11)
                                zile_lucratoare_ul = nTmp + zile_lucratoare_ul;

                            zile_lucrate_12 = zileLucrate[i] + nrzileCM[i] + zile_lucrate_12;

                            arCM.zileLucrate[i] = zileLucrate[i];
                            arCM.nrzileCM[i] = nrzileCM[i];


                            arCM.tzs[i] = zileLucrate[i] + nrzileCM[i];
                            arCM.stComplet[i] = arCM.zl[i] == arCM.tzs[i] ? "DA" : arCM.zl[i] > arCM.tzs[i] ? "NU" : "DA (Err)";


                            BCCAS[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][2] != null && dt.Rows[0][2].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][2].ToString()) : 0);
                            VALCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][3] != null && dt.Rows[0][3].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][3].ToString()) : 0);

                            if (nZileLunaCurenta == 0)
                                arCM.total[i] = (int)(BCCAS[i] + VALCM[i]);
                            else
                                arCM.total[i] = (int)SumaZile12;
                            break;
                        default:
                            arCM.zl[i] = nTmp;
                            nrzileLuna[i] = nTmp;

                            zileLucrate[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][0].ToString()) : 0);
                            nrzileCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][1] != null && dt.Rows[0][1].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][1].ToString()) : 0);

                            if (zileLucrate[i] + nrzileCM[i] > nTmp)
                                zileLucrate[i] = nTmp - nrzileCM[i];

                            if (i > 6)
                                zile_lucratoare_6 = nTmp + zile_lucratoare_6;

                            if (i > 11)
                                zile_lucratoare_ul = nTmp + zile_lucratoare_ul;

                            zile_lucrate_12 = zileLucrate[i] + nrzileCM[i] + zile_lucrate_12;


                            arCM.tzs[i] = zileLucrate[i] + nrzileCM[i];
                            arCM.stComplet[i] = arCM.zl[i] == arCM.tzs[i] ? "DA" : arCM.zl[i] > arCM.tzs[i] ? "NU" : "DA (Err)";


                            arCM.zileLucrate[i] = zileLucrate[i];
                            arCM.nrzileCM[i] = nrzileCM[i];

                            BCCAS[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][2] != null && dt.Rows[0][2].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][2].ToString()) : 0);
                            VALCM[i] = (dt != null && dt.Rows.Count > 0 && dt.Rows[0][3] != null && dt.Rows[0][3].ToString().Length > 0 ? Convert.ToInt32(dt.Rows[0][3].ToString()) : 0);

                            if (arCM.tzs[i] != 0)
                                SumaZi[i] = (BCCAS[i] + VALCM[i]) / arCM.tzs[i];
                            else
                                SumaZi[i] = 0;

                            arCM.total[i] = (int)(BCCAS[i] + VALCM[i]);
                            break;
                    }

                    if (i > 12 - 6)
                    {
                        if ((year > 2005) || (year == 2005 && month >= 7))
                        {
                            dblBPSRON6 = dblBPSRON6 + arCM.bpsRON[i];
                            calculez = true;
                        }
                        else
                            dblBPS6 = dblBPS6 + arCM.bps[i];
                    }



                    if (arCM.zl[i] == arCM.tzs[i])
                        nStageCount = nStageCount + 1;

                    if (i > 12 - 10)
                        nTZS = nTZS + arCM.tzs[i];
                    if (i > 12 - 6)
                        nTZS6 = nTZS6 + arCM.tzs[i];


                    month = month - 1;


                }

                for (i = 12; i >= 1; i--)
                {
                    bUseLine[i] = false;
                    bUseLineDebug[i] = false;
                    zileRedistribDebug[i] = 0;
                }

                if (nZileLunaCurenta == 0)
                {
                    for (i = 12; i >= 1; i--)
                        if (arCM.tzs[i] > 0 || (arCM.tzs[i] == 0 && (ExistZileStagiu(i, arCM) == true)))
                            if ((i > 6 && nchkMedie6Luni == 1) || (i > 2 && nchkMedie6Luni == 2))
                                if (arCM.zl[i] >= arCM.tzs[i])
                                    zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL + (arCM.zl[i] - arCM.tzs[i]);
                }
                else
                {
                    for (i = 12; i >= 1; i--)
                        if (arCM.tzs[i] > 0 || (arCM.tzs[i] == 0 && (ExistZileStagiu(i, arCM) == true)))
                            if ((i > 6 && nchkMedie6Luni == 1) || (i > 2 && nchkMedie6Luni == 2))
                                if (arCM.zl[i] >= arCM.tzs[i])
                                    zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL + (arCM.zl[i] - arCM.tzs[i]);
                }



                if (nZileLunaCurenta != 0) //am luna fractionata
                {
                    if (nchkMedie6Luni == 1) //medie pe 6 luni
                    {
                        for (i = 12; i >= 6; i--)
                        {
                            bUseLine[i] = true; //se utilizeaza linia la medie
                            bUseLineDebug[i] = true;
                            zileRedistribDebug[i] = zileLucrate[i] + nrzileCM[i];
                        }
                    }
                    else
                    {
                        for (i = 12; i >= 2; i--)
                        {
                            bUseLine[i] = true; //se utilizeaza linia la medie
                            bUseLineDebug[i] = true;
                            zileRedistribDebug[i] = zileLucrate[i] + nrzileCM[i];
                        }
                    }
                }
                else //nu am luna fractionata
                {
                    if (nchkMedie6Luni == 1)  //medie pe 6 luni
                    {
                        for (i = 12; i >= 7; i--)
                        {
                            bUseLine[i] = true; //se utilizeaza linia la medie
                            bUseLineDebug[i] = true;
                            zileRedistribDebug[i] = zileLucrate[i] + nrzileCM[i];
                        }
                    }
                    else
                    {
                        for (i = 12; i >= 3; i--)
                        {
                            bUseLine[i] = true; //se utilizeaza linia la medie
                            bUseLineDebug[i] = true;
                            zileRedistribDebug[i] = zileLucrate[i] + nrzileCM[i];
                        }
                    }
                }


                if (zileDiff_Stagiu_ZL > 0)
                {
                    if (nchkMedie6Luni == 1)
                    {
                        j = 6;
                        nLunaFractionata = 6;
                    }
                    else
                    {
                        j = 2;
                        nLunaFractionata = 2;
                    }


                    while (j >= 1 && zileDiff_Stagiu_ZL > 0)
                    {
                        switch (nLunaFractionata)
                        {
                            case 6:
                                TmpZileLucrate = zileLucrate6;
                                TmpZileCM = nrzileCM6;
                                TmpZileLuna = nrZileLuna6;
                                break;
                            case 2:
                                TmpZileLucrate = zileLucrate10;
                                TmpZileCM = nrzileCM10;
                                TmpZileLuna = nrZileLuna10;
                                break;
                            default:
                                TmpZileLucrate = zileLucrate[j];
                                TmpZileCM = nrzileCM[j];
                                TmpZileLuna = nrzileLuna[j];
                                break;
                        }

                        nTmpTZSLuna = TmpZileLucrate + TmpZileCM;
                        bAllowRedistr = arCM.zl[j] > 0 && (arCM.tzs[j] > 0 || (arCM.tzs[j] == 0 && (ExistZileStagiu(j, arCM) == true))) ? true : false;


                        if (bAllowRedistr && j == nLunaFractionata)
                        {

                            if (nZileLunaCurenta != 0)
                            {

                                if (nTmpTZSLuna > arCM.tzs[j])
                                {
                                    bUseLineDebug[j] = true;

                                    if (zileDiff_Stagiu_ZL + arCM.tzs[j] >= nTmpTZSLuna)
                                    {
                                        nTmp = nTmpTZSLuna - arCM.tzs[j];

                                    }
                                    else
                                    {
                                        nTmp = zileDiff_Stagiu_ZL;
                                    }

                                    zileRedistribDebug[j] = nTmp;
                                    zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL - nTmp;
                                }
                                else
                                {
                                    bUseLineDebug[j] = true;
                                    zileRedistribDebug[j] = 0;
                                    zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL - 0;
                                }
                            }
                            else
                            {
                                bUseLineDebug[j] = true;
                                nTmp1 = zileDiff_Stagiu_ZL <= arCM.tzs[j] ? zileDiff_Stagiu_ZL : arCM.tzs[j];
                                zileRedistribDebug[j] = nTmp1;
                                zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL - arCM.tzs[j];

                                if (zileDiff_Stagiu_ZL <= 0)
                                {
                                }
                            }
                        }
                        else if (bAllowRedistr && j != nLunaFractionata)
                        {
                            bUseLineDebug[j] = true;
                            nTmp1 = zileDiff_Stagiu_ZL <= arCM.tzs[j] ? zileDiff_Stagiu_ZL : arCM.tzs[j];
                            zileRedistribDebug[j] = nTmp1;
                            zileDiff_Stagiu_ZL = zileDiff_Stagiu_ZL - arCM.tzs[j];

                            if (zileDiff_Stagiu_ZL <= 0)
                            {
                            }
                        }


                        j = j - 1;
                    }

                }


                if (nZileLunaCurenta == 0)
                {
                    month = lunaC - 1;
                    year = anC;
                }
                else
                {
                    month = lunaC;
                    year = anC;
                }

                dblBPS = 0;
                dblBPSRON = 0;
                dblBPS6 = 0;
                dblBPSRON6 = 0;
                nStageCount = 0;
                nTZS = 0;
                nTZS6 = 0;
                double tCol7;
                int lastCol4;
                double ValSalM_069;

                tCol7 = 0;
                
                string strVarSalMin, strVarNrSalMin;
                double valSalMin, valSalM;              
                int nrSalMin;
                valSalMin = 0;
                nrSalMin = 12;
                strVarSalMin = "";

                if (lista.ContainsKey("SAL_MIN"))
                    strVarSalMin = lista["SAL_MIN"];

                strVarNrSalMin = "";

                if (lista.ContainsKey("NR_SAL_MIN"))
                    strVarNrSalMin = lista["NR_SAL_MIN"];

                if (strVarSalMin.Length > 0)
                    valSalMin = Convert.ToInt32(strVarSalMin);

                if (strVarNrSalMin.Length > 0)
                    nrSalMin = Convert.ToInt32(strVarNrSalMin);

                for (i = 12; i >= 1; i--)
                {
                    calculez = false;
                    if (month == 0)
                    {
                        month = 12;
                        year = year - 1;
                    }

                    DataTable dtTmp = General.IncarcaDT("SELECT F06911 FROM F069 WHERE F06904=" + year + " AND F06905=" + month, null);
                    ValSalM_069 = (dtTmp != null && dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null && dtTmp.Rows[0][0].ToString().Length > 0 ? Convert.ToDouble(dtTmp.Rows[0][0].ToString()) : 0);

                    if (nZileLunaCurenta == 0)
                    {

                        if (i > 12 - 10)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                            {
                                arCM.bps[i] = 0;
                                arCM.bpsRON[i] = TestBazaCalcul(year, month, i, arCM);
                            }
                            else
                            {
                                arCM.bpsRON[i] = 0;
                                arCM.bps[i] = TestBazaCalcul(year, month, i, arCM);
                            }
                        }
                        else
                        {
                            arCM.bps[i] = 0;
                            arCM.bpsRON[i] = 0;
                        }

                        //cele implicite, care sunt luate intotdeauna la 10 luni
                        if (i > 12 - 10)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                                dblBPSRON = dblBPSRON + arCM.bpsRON[i];
                            else
                                dblBPS = dblBPS + arCM.bps[i];

                            nTZS = nTZS + arCM.tzs[i];
                            nStageCount_ZL_10 = nStageCount_ZL_10 + arCM.zl[i];
                            nStageCount_TZS_10 = nStageCount_TZS_10 + arCM.tzs[i];
                        }


                        // cele care sunt luate functie de redistribuire == la 10 luni
                        if ((i <= 12 - 10) && (bUseLine[i] == true))
                        {
                            nTZS = nTZS + arCM.tzs[i];
                            dblBPS = dblBPS + arCM.bps[i];

                            if (arCM.bpsRON[i] != 0)
                                dblBPSRON = dblBPSRON + arCM.bpsRON[i];
                            nStageCount_ZL_10 = nStageCount_ZL_10 + arCM.zl[i];
                            nStageCount_TZS_10 = nStageCount_TZS_10 + arCM.tzs[i];
                        }

                        //cele implicite, care sunt luate intotdeauna la 6 luni
                        if (i > 12 - 6)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                            {
                                dblBPSRON6 = dblBPSRON6 + arCM.bpsRON[i];
                                calculez = true;
                            }
                            else
                                dblBPS6 = dblBPS6 + arCM.bps[i];

                            nTZS6 = nTZS6 + arCM.tzs[i];
                            nStageCount_ZL_6 = nStageCount_ZL_6 + arCM.zl[i];
                            nStageCount_TZS_6 = nStageCount_TZS_6 + arCM.tzs[i];
                        }

                        // cele care sunt luate functie de redistribuire
                        if ((i <= 12 - 6) && (bUseLine[i] == true))
                        {
                            if (arCM.bps[i] != 0)
                                dblBPS6 = dblBPS6 + arCM.bps[i];
                            if (arCM.bpsRON[i] != 0)
                            {
                                dblBPSRON6 = dblBPSRON6 + arCM.bpsRON[i];
                                calculez = true;
                            }
                            nTZS6 = nTZS6 + arCM.tzs[i];
                            nStageCount_ZL_6 = nStageCount_ZL_6 + arCM.zl[i];
                            nStageCount_TZS_6 = nStageCount_TZS_6 + arCM.tzs[i];
                        }
                    }
                    else // cu luna fractionata
                    {
                        if (i >= 12 - 10)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                            {
                                arCM.bps[i] = 0;
                                arCM.bpsRON[i] = TestBazaCalcul(year, month, i, arCM);
                            }
                            else
                            {
                                arCM.bpsRON[i] = 0;
                                arCM.bps[i] = TestBazaCalcul(year, month, i, arCM);
                            }
                        }
                        else
                        {
                            arCM.bps[i] = 0;
                            arCM.bpsRON[i] = 0;
                        }


                        //cele implicite, care sunt luate intotdeauna la 10 luni
                        if (i >= 12 - 10)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                                dblBPSRON = dblBPSRON + arCM.bpsRON[i];
                            else
                                dblBPS = dblBPS + arCM.bps[i];
                            nTZS = nTZS + arCM.tzs[i];
                            nStageCount_ZL_10 = nStageCount_ZL_10 + arCM.zl[i];
                            nStageCount_TZS_10 = nStageCount_TZS_10 + arCM.tzs[i];
                        }


                        //cele care sunt luate functie de redistribuire == la 10 luni
                        if ((i < 12 - 10) && (bUseLine[i] == true))
                        {
                            nTZS = nTZS + arCM.tzs[i];
                            if (arCM.bps[i] != 0)
                                dblBPS = dblBPS + arCM.bps[i];

                            if (arCM.bpsRON[i] != 0)
                                dblBPSRON = dblBPSRON + arCM.bpsRON[i];

                            nStageCount_ZL_10 = nStageCount_ZL_10 + arCM.zl[i];
                            nStageCount_TZS_10 = nStageCount_TZS_10 + arCM.tzs[i];
                        }



                        //cele implicite, care sunt luate intotdeauna la 6 luni
                        if (i >= 12 - 6)
                        {
                            if ((year > 2005) || (year == 2005 && month >= 7))
                            {
                                dblBPSRON6 = dblBPSRON6 + arCM.bpsRON[i];
                                calculez = true;
                            }
                            else
                                dblBPS6 = dblBPS6 + arCM.bps[i];
                            nTZS6 = nTZS6 + arCM.tzs[i];
                            nStageCount_ZL_6 = nStageCount_ZL_6 + arCM.zl[i];
                            nStageCount_TZS_6 = nStageCount_TZS_6 + arCM.tzs[i];
                        }

                        //cele care sunt luate functie de redistribuire
                        if ((i < 12 - 6) && (bUseLine[i] == true))
                        {
                            if (arCM.bps[i] != 0)
                                dblBPS6 = dblBPS6 + arCM.bps[i];

                            if (arCM.bpsRON[i] != 0)
                            {
                                dblBPSRON6 = dblBPSRON6 + arCM.bpsRON[i];
                                calculez = true;
                            }


                            nTZS6 = nTZS6 + arCM.tzs[i];
                            nStageCount_ZL_6 = nStageCount_ZL_6 + arCM.zl[i];
                            nStageCount_TZS_6 = nStageCount_TZS_6 + arCM.tzs[i];
                        }
                    }


                    month--;
                    tCol7 = tCol7 + arCM.tzs[i];

                    if (ValSalM_069 == 0)
                        ValSalM_069 = Convert.ToDouble((year == 2007 ? 390 : (year == 2008 && month >= 0 && month <= 8 ? 500 : (year == 2008 && month >= 9 && month <= 11 ? 540 : (year == 2009 || year == 2010 ? 600 : (year == 2011 ? 670 : (year == 2012 || (year == 2013 && month == 0) ? 700 : (year == 2013 && month >= 1 && month <= 5 ? 750 : (year == 2013 && month >= 6 ? 800 : (year == 2014 && month <= 5 ? 850 : (year == 2014 && month >= 6 ? 900 : (year == 2015 && month <= 5 ? 975 : ((year == 2015 && month >= 6) || (year == 2016 && month <= 3) ? 1050 : (year == 2016 && month >= 4 ? 1250 : valSalMin))))))))))))));

                    valSalM = ValSalM_069 * nrSalMin;

                    if (szBazaCMFUNASS.Length > 4)
                    {
                    //    if (Not IsNumeric(arCM.total[i])) 
                    //    {
                    //arCM.AMBP[i] = "-";
                    //    }
                    //    else
                    //    {
                        switch (arCM.an[i])
                        {
                            case 2015:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2014:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2013:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2012:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2011:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2010:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2009:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2008:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2007:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                            case 2006:
                                if (arCM.luna[i] <= 10)
                                    arCM.AMBP[i] = arCM.total[i];
                                else
                                    arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);

                                break;
                            default:
                                arCM.AMBP[i] = (arCM.total[i] > valSalM ? valSalM : arCM.total[i]);
                                break;
                        }
                //}
                    }
                    else
                    {
                //if (Not IsNumeric(arCM.bpsRON[i])) 
                //        {
                //    arCM.AMBP[i] = "-";
                //}
                //        else
                //        {
                        switch (arCM.an[i])
                        {
                            case 2015:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2014:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2013:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2012:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2011:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2010:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2009:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2008:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2007:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;
                            case 2006:
                                if (arCM.luna[i] <= 10)
                                    arCM.AMBP[i] = arCM.bpsRON[i];
                                else
                                    arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);

                                break;
                            default:
                                arCM.AMBP[i] = (arCM.bpsRON[i] > valSalM ? valSalM : arCM.bpsRON[i]);
                                break;


                        }

                //}
                    }
            //arCM.AMBP[i] = Format(arCM.AMBP[i], "#,##0.00");
            if (calculez)
                arCM.AMBP_Total = Convert.ToDouble(arCM.AMBP_Total +  arCM.AMBP[i]);

        }

        //arCM.AMBP_Total = Format(arCM.AMBP_Total, "#,###.00");

     
     for (i = 1; i <= 12; i++)
    {
         //arCM.total[i] = Format(arCM.total[i], "#,##0.00");
        //arCM.bps[i] = Format(arCM.bps[i], "#,##0.00");
                       
         //if (arCM.bpsRON[i] != "-" )
           // arCM.bpsRON[i] = Format(arCM.bpsRON[i], "#,##0.00");
          
                
         //if InStr(1, arCM.bpsRON[i], ".", vbTextCompare) = Len(arCM.bpsRON[i])
         //   arCM.bpsRON[i] = Format(arCM.bpsRON[i], "#,##0.00");



    }

    lastCol4 = arCM.zl12;

    
     if (nTZS != 0)
        {
         SumaDblRON = dblBPSRON;
         SumaDbl = dblBPS;
        
             SumaDbl = SumaDbl / 10000;
             //szTmpTxt = Format(SumaDbl, "#,##0");
             //SumaDbl = CStr(szTmpTxt);
         
        
         SumaDbl = SumaDblRON + SumaDbl;
         dblMedieZilnica = SumaDbl / nTZS;
        }
        else
         dblMedieZilnica = 0;
            
   if (nTZS6 != 0) 
    {
        SumaDblRON = dblBPSRON6;
        SumaDbl = dblBPS6;
    
             SumaDbl = SumaDbl / 10000;
             //szTmpTxt = Format(SumaDbl, "#,##0");
             //SumaDbl = CStr(szTmpTxt);
       
        
        SumaDbl = SumaDblRON + SumaDbl;
         dblMedieZilnica6 = SumaDbl / nTZS;
    }
        else
         dblMedieZilnica6 = 0;
      
       
     //arCM.lblCM = Iif(listWorkers.ListItems(listWorkers.SelectedItem.Index).ListSubItems(4).Text = "...", "", listWorkers.ListItems(listWorkers.SelectedItem.Index).ListSubItems(4).Text)
     Medie6 = dblMedieZilnica6;
     Medie10 = dblMedieZilnica;
       
    if (nchkMedie6Luni == 1) 
        {
         if (tCol7 >= 22)
             bAreStagiu = true;
            else
            {
             bAreStagiu = false;
            }
            
            long nrluc;
            if (nrluni_stagiu == 1) 
                nrluc = zile_lucratoare_ul;
            else
                nrluc = zile_lucratoare_6;
           
            if (zile_lucrate_12 < nrluc) 
            {
                //arCM.Label62.Visible = true;
               // if (nrluni_stagiu == 1) 
                    //arCM.Label62 = "ATENTIE ! Angajatul are stagiu " & zile_lucrate_12 & " zile in ultimele 12 luni, mai mic decat necesarul de " & zile_lucratoare_ul & " de zile lucratoare in ultima luna ! (stagiu valabil pana la 01.07.2018 cf. Ordin 8/2018 din MO 190/2018)"
               // else
                    //arCM.Label62 = "ATENTIE ! Angajatul are stagiu " & zile_lucrate_12 & " zile in ultimele 12 luni, mai mic decat necesarul de " & zile_lucratoare_6 & " de zile lucratoare in ultimele 6 luni !"
                
                //stagiu_print = "NU";
            }
            else
            {
                //arCM.Label62.Visible = false;
                //stagiu_print = "DA";
            }
        
         arCM.tzsTotal6 = nTZS6;
        
         arCM.bpsTOTAL6 = dblBPS6;
         //arCM.bpsTOTAL6 = Format(arCM.bpsTOTAL6, "#,##0.00");
        
         arCM.bpsTOTALRON6 = dblBPSRON6;
         //arCM.bpsTOTALRON6 = Format(arCM.bpsTOTALRON6, "#,##0.00");
        
            // if (SumaDblRON > 0)          
                //arCM.total = Format(CDbl(Format(dblBPS6 / 10000, "#,##0")), "#,##0.0000") & " + " & Format(dblBPSRON6, "#,##0.0000") & " = " & Format(SumaDbl, "#,##0.0000")
           // else
                //arCM.TOTAL.Caption = Format(dblBPS6, "#,##0.00") & " + " & Format(dblBPSRON6, "#,##0.00") & " = " & Format(SumaDbl, "#,##0.00")
           
         //arCM.TOTAL.Caption = Format(CDbl(Format(dblBPS6 / 10000, "#,##0")), "#,##0.0000") & " + " & Format(dblBPSRON6, "#,##0.0000") & " = " & Format(SumaDbl, "#,##0.0000")
        

        //arCM.SumaTotalVerde.Caption = Format(SumaDbl, "#,##0.0000") 'RON total
        
         //szTxtMedieZilnica6_10 = "Media zilnica pentru CM propusa la 6 luni:"
         if (nTZS6 != 0) 
        {
             //if (SumaDblRON > 0) 
               //szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS6 & " = " & Format(SumaDbl / nTZS6, "#,##0.00")
               // else
               //szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS6 & " = " & Format(SumaDbl / nTZS6, "#,##0.00")
               
            //szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS6 & " = " & Format(SumaDbl / nTZS6, "#,##0.00")
            
             //Medie6 = CDbl(Format(SumaDbl / nTZS6, "#,##0.00"))
            }
            else
        {
           szValMedieZilnica6_10 = "0";
            Medie6 = 0;
        }
      }  
      else
   {

          if (tCol7 >= 22)  
             bAreStagiu = true;
            else
            //arCM.lblARENARE6.ForeColor = 255
             bAreStagiu = false;

                    long nrluc;
            if (nrluni_stagiu == 1) 
                nrluc = zile_lucratoare_ul;
            else
                nrluc = zile_lucratoare_6;
           

            //if (zile_lucrate_12 < nrluc) 
                //arCM.Label62.Visible = True
               // if (nrluni_stagiu == 1) 
                    //arCM.Label62 = "ATENTIE ! Angajatul are stagiu " & zile_lucrate_12 & " zile in ultimele 12 luni, mai mic decat necesarul de " & zile_lucratoare_ul & " de zile lucratoare in ultima luna ! (stagiu valabil pana la 01.07.2018 cf. Ordin 8/2018 din MO 190/2018)"
               // else
                    //arCM.Label62 = "ATENTIE ! Angajatul are stagiu " & zile_lucrate_12 & " zile in ultimele 12 luni, mai mic decat necesarul de " & zile_lucratoare_6 & " de zile lucratoare in ultimele 6 luni !"
               
                //stagiu_print = "NU"
           // else
                //arCM.Label62.Visible = False
                //stagiu_print = "DA"
          
            
         arCM.tzsTotal6 = nTZS;
        
         arCM.bpsTOTAL6e = dblBPS;
       // arCM.bpsTOTAL6").Value = Format(arCM.bpsTOTAL6").Value, "#,##0.00")
             
         arCM.bpsTOTALRON6 = dblBPSRON;
      // arCM.bpsTOTALRON6 = Format(arCM.bpsTOTALRON6").Value, "#,##0.00")
             
         //if (SumaDblRON > 0) 
          // arCM.TOTAL.Caption = Format(CDbl(Format(dblBPS / 10000, "#,##0")), "#,##0.0000") & " + " & Format(dblBPSRON, "#,##0.0000") & " = " & Format(SumaDbl, "#,##0.0000")
            //else
            // arCM.TOTAL.Caption = Format(dblBPS, "#,##0.0000") & " + " & Format(dblBPSRON, "#,##0.00") & " = " & Format(SumaDbl, "#,##0.00")
         
        //arCM.TOTAL.Caption = Format(CDbl(Format(dblBPS / 10000, "#,##0")), "#,##0.0000") & " + " & Format(dblBPSRON, "#,##0.0000") & " = " & Format(SumaDbl, "#,##0.0000")

         //arCM.SumaTotalVerde.Caption = Format(SumaDbl, "#,##0.0000") 'RON total
                                                   
        // szTxtMedieZilnica6_10 = "Media zilnica pentru CM propusa la 10 luni:"
         if (nTZS != 0) 
        {
             //if (SumaDblRON > 0) 
               //szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS & " = " & Format(SumaDbl / nTZS, "#,##0.00")
                //else
              // szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS & " = " & Format(SumaDbl / nTZS, "#,##0.00")
                
             //szValMedieZilnica6_10 = Format(SumaDbl, "#,##0.00") & " / " & nTZS & " = " & Format(SumaDbl / nTZS, "#,##0.00")
             //Medie10 = CDbl(Format(SumaDbl / nTZS, "#,##0.0000"))
            
              }
            else
            {
            szValMedieZilnica6_10 = "0";
            Medie10 = 0;
            }
    
       }
    
     //arCM.fld_Medie6.Text = szValMedieZilnica6_10
    
     //   'Linii doar ptr teste
     //szTmpTxt = "Linii : "
     //For i = 12 To 1 Step -1
     //    if (bUseLineDebug[i] = True) Then
     //        szTmpTxt = szTmpTxt & "  L_" & CStr[i] & "=" & CStr(zileRedistribDebug[i]) & " "
     //       End if
     //Next i

    // arCM.Label47.Caption = szTmpTxt
    
     //if (nchkMedie6Luni == 1) 
     //    //arCM.Label49.Caption = "ZL = " & CStr(nStageCount_ZL_6) & "  TZS = " & CStr(nStageCount_TZS_6)
     //   else
     //    //arCM.Label49.Caption = "ZL = " & CStr(nStageCount_ZL_10) & "  TZS = " & CStr(nStageCount_TZS_10)
        
       
     //    arCM.Medie7 = 0;
     //     if (arCM.tzsTotal6 > 0) 
     //       arCM.Medie7 = Convert.ToDouble(arCM.AMBP_Total / arCM.tzsTotal6);
     //     Medie7 = CDbl(arCM.Medie7;
         
        //arCM.Medie7 = Format(CDbl(arCM.AMBP_Total").Value), "#,##0.00") & " / " & Format(arCM.tzsTotal6").Value, "#,##0") & " = " & Format(arCM.Medie7").Value, "#,##0.00")
       // 'Linii doar ptr teste
       
 
        //if (szBazaCMAMBP.Length > 4) 
        //    Call CreateDetails1(nMarca, strCNP_marci)
     
            
        
                if (Constante.tipBD == 2) 
                {
                    szsql = "SELECT COUNT(*) AS CNT FROM USER_TABLES WHERE TABLE_NAME = 'TMP_" + marca + "_CM'";
                    DataTable dtTmp = General.IncarcaDT(szsql, null);
         
                    if (dtTmp != null && dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null && dtTmp.Rows[0][0].ToString().Length > 0)
                        {
                        szsql = "TRUNCATE TABLE TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
   
                    }
                }
                else
                {
                    szsql = "SELECT COUNT(*) AS CNT FROM tempdb.sys.objects where name like '#TMP_" + marca + "_CM%'";
                    DataTable dtTmp = General.IncarcaDT(szsql, null);
                    if (dtTmp != null && dtTmp.Rows.Count > 0 && dtTmp.Rows[0][0] != null && dtTmp.Rows[0][0].ToString().Length > 0)
                    {
                        szsql = "TRUNCATE TABLE #TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
                        szsql = "DROP TABLE #TMP_" + marca + "_CM";
                        General.ExecutaNonQuery(szsql, null);
                    }
                }


                BCCM = arCM.AMBP_Total;
                ZBCCM = (int)arCM.tzsTotal6;

               //BazaCalcul1 = CDbl(arCM.AMBP_Total);
               //BazaCalcul2 = CDbl(arCM.BPSTOTALRON6);
               //NumarZileBazaCalcul = CInt(arCM.tzsTotal6);
   
            


			}
			catch(Exception ex)
			{
                General.MemoreazaEroarea(ex, "CreateDetails", new StackTrace().GetFrame(0).GetMethod().Name);
            }
		
		}


        public static Dictionary<String, String> LoadParameters()
        {
            Dictionary<String, String> lista = new Dictionary<string, string>();

            string sql = "SELECT  \"Nume\" AS ETICHETA, \"Valoare\" AS VALOARE FROM \"tblParametrii\" WHERE \"Nume\" IN ('BAZA_CMFNUASS', 'BAZA_CMAMBP', 'NN', 'PP', 'ZAMBP2', 'ZAMBP', 'BASS', 'SOMA')";
            DataTable dtParam = General.IncarcaDT(sql, null);
            if (dtParam != null && dtParam.Rows.Count > 0)
                for (int i = 0; i < dtParam.Rows.Count; i++)
                    if (!lista.ContainsKey(dtParam.Rows[i]["ETICHETA"].ToString()))
                        lista.Add(dtParam.Rows[i]["ETICHETA"].ToString(), dtParam.Rows[i]["VALOARE"].ToString());
            return lista;
        }

        public static int CalculZLLuna(DateTime dtDate)
        {

            DateTime dtCrt;
            int nCrtZL, nDay, i;
            DayOfWeek nCrtDay;

            dtCrt = dtDate;
            dtCrt = dtCrt.AddDays(DateTime.DaysInMonth(dtDate.Year, dtDate.Month) - 1);

            nCrtZL = dtCrt.Day;
            nDay = dtCrt.Day;
            dtCrt = dtDate;

            for (i= nDay; i >= 1; i--)
            {
                nCrtDay = dtCrt.DayOfWeek;
                if (nCrtDay == DayOfWeek.Saturday || nCrtDay == DayOfWeek.Sunday)

                    nCrtZL = nCrtZL - 1;
                else
                {
                    if (Exists(dtCrt))
                        nCrtZL = nCrtZL - 1;    
                }
                dtCrt = dtCrt.AddDays(1); 
            }
            return nCrtZL;

        }


        public static bool Exists(DateTime dtDate)
        {

            DateTime dtTest, dtTmpTest;            
            bool bTest;
            int i;

            bTest = false;
            dtTmpTest = dtDate;

            DataTable dt = General.IncarcaDT("SELECT " + (Constante.tipBD == 1 ? "CONVERT(VARCHAR, DAY, 103)" : "TO_CHAR(DAY, 'dd/mm/yyyy')") + " FROM HOLIDAYS", null);
            for (i = 0; i < dt.Rows.Count; i++)
            {
                dtTest = new DateTime(Convert.ToInt32(dt.Rows[0][0].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[0][0].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[0][0].ToString().Substring(0, 2)));
                if (dtTest == dtTmpTest)
                {
                    bTest = true;
                    break;
                }
            }
              return bTest;
        }



        public static bool ExistZileStagiu(int nInt, CM arCM)
        {

            bool bIs = false;
            int i;

            for (i = (nInt - 1); i >= 1; i--)
            {


                if (arCM.tzs[i] > 0)
                {
                    bIs = true;
                    break;
                }
                   
       
            }
            return bIs;
        }



        public static double TestBazaCalcul(int crtYear, int crtMonth, int crtIndex, CM arCM)
        {

            double dRes = 0, dLimita = 12445959;

            if (crtYear < 2002)
                dLimita = 12445959;
            else if (crtYear == 2002)
                dLimita = 16746000;
            else if (crtYear == 2003)
                dLimita = 34810000;
            else if (crtYear == 2004)
                dLimita = 38410000;
            else if (crtYear == 2005 && crtMonth < 7)
                dLimita = 46055000;
            else if (crtYear == 2005 && crtMonth <= 12 && crtMonth >= 7)
                dLimita = 4605;
            else if (crtYear >= 2006)
                dLimita = 999999999;

            if (dLimita < arCM.total[crtIndex])
                dRes = dLimita;
            else
                dRes = arCM.total[crtIndex];

            return dRes;
        }


    }


	
}