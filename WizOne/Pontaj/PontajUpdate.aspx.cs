using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajUpdate : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnAct.Text = Dami.TraduCuvant("btnAct", "Actualizeaza absenta");

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    txtMarcaInc.Value = 1;
                    txtMarcaSf.Value = 999999;
                    txtDataInc.Value = DateTime.Now;
                    txtDataSf.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnAct_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDataInc.Value == null || txtDataSf.Value == null || txtMarcaInc.Value == null || txtMarcaSf.Value == null || (chkCtr.Checked == false && chkStr.Checked == false && chkNrm.Checked == false && chkPerAng.Checked == false && chkRecalc.Checked == false))
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "Atentie !");
                    return;
                }

                if (Convert.ToInt32(txtMarcaInc.Value) > Convert.ToInt32(txtMarcaSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Marca inceput este mai mare decat marca sfarsit !"), MessageBox.icoError, "Atentie !");
                    return;
                }

                if (Convert.ToDateTime(txtDataInc.Value) > Convert.ToDateTime(txtDataSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Data inceput este mai mare decat data sfarsit !"), MessageBox.icoError, "Atentie !");
                    return;
                }

                if (ActualizeazaDateGenerale(Convert.ToInt32(Session["UserId"]), Convert.ToInt32(txtMarcaInc.Value), Convert.ToInt32(txtMarcaSf.Value), 
                    Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), Convert.ToBoolean(chkCtr.Checked), 
                    Convert.ToBoolean(chkNrm.Checked), Convert.ToBoolean(chkStr.Checked), Convert.ToBoolean(chkPerAng.Checked), Convert.ToBoolean(chkRecalc.Checked), false))
                    MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes !"), MessageBox.icoSuccess, "Atentie !");
                else
                    MessageBox.Show(Dami.TraduCuvant("Eroare in proces!"), MessageBox.icoError, "Atentie !");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public bool ActualizeazaDateGenerale(int idUser, int angIn, int angSf, DateTime ziIn, DateTime ziSf, bool chkCtr, bool chkNrm, bool chkStr, bool chkPerAng, bool chkRecalc, bool chkCC)
        {
            bool ras = false;

            try
            {
                string strSql = "";
                string act = "";
                string inn = "";

                if (Constante.tipBD == 1)
                {
                    //Radu 12.02.2019
                    if (chkPerAng == true)
                    {
                        string ziInceput = General.ToDataUniv(ziIn.Year, ziIn.Month, 1);
                        string ziSfarsit = General.ToDataUniv(ziSf.Year, ziSf.Month, 99);                       


                        string strDel = @"DELETE A
                                    FROM Ptj_Intrari A
                                    INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                    WHERE CONVERT(date,ISNULL(MODIF.DATA, f10023)) >= {0} AND CONVERT(date,ISNULL(MODIF.DATA, f10023)) <> '2100-01-01') B 
                                    ON A.F10003=B.F10003 AND A.Ziua> B.DATA_PLECARII AND A.F10003 >= {1} AND A.F10003 <= {2}";

                        strDel = string.Format(strDel, ziInceput, angIn, angSf);
                        ras = General.ExecutaNonQuery(strDel, null);
                        
                        strDel = @"DELETE A
                                    FROM Ptj_Intrari A
                                    INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE CONVERT(date,f10022) <= {0} AND CONVERT(date,F10022) <> '2100-01-01') B 
                                    ON A.F10003=B.F10003 AND A.Ziua< B.F10022 AND A.F10003 >= {1} AND A.F10003 <= {2}";

                        strDel = string.Format(strDel, ziSfarsit, angIn, angSf);
                        ras = General.ExecutaNonQuery(strDel, null);
                    }

                    //Radu 12.02.2019
                    if (chkRecalc == true)
                    {
                        int an = ziIn.Year;
                        int luna = ziIn.Month;

                        string sql = "SELECT F10003 FROM F100 WHERE F10025 IN (0, 999) AND " + angIn + " <= F10003 AND F10003 <= " + angSf;
                        DataTable dt = General.IncarcaDT(sql, null);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            while (an < ziSf.Year || (an == ziSf.Year && luna <= ziSf.Month))
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                    General.CalculFormuleCumulat(Convert.ToInt32(dt.Rows[i][0].ToString()), an, luna);
                                luna++;
                                if (luna > 12)
                                {
                                    luna = 1;
                                    an++;
                                }
                            }
                        }
                        ras = true;
                    }



                    if (chkCtr || chkNrm || chkStr)
                    {
                        strSql += @"UPDATE A 
                                SET {4} 
                                FROM Ptj_Intrari A
                                INNER JOIN F100 B ON A.F10003=B.F10003
                                {5}
                                WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.Ziua AND A.Ziua <= {3};";

                        if (chkCtr == true) act += ",A.IdContract=(SELECT MAX(B.IdContract) AS IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CAST(B.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(B.DataSfarsit AS Date))";
                        if (chkNrm == true)
                        {
                            act += ",A.Norma=ISNULL(dn.Norma, B.F10043)";
                            inn += " OUTER APPLY dbo.DamiNorma(A.F10003, A.Ziua) dn \n";
                            //Florin 2018.10.23
                            if (Dami.ValoareParam("TipCalculDate") == "2")
                                inn += "LEFT JOIN DamiNorma_Table dnt ON dnt.F10003=A.F10003 AND dnt.dt=A.Ziua";
                        }
                        //if (chkCC == true)
                        //{
                        //    act += ",A.F06204Default=CASE WHEN NOT EXISTS(SELECT MAX(C.IdCentruCost) AS F06204Default FROM F100CentreCost C WHERE A.F10003 = C.F10003 AND CAST(C.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(C.DataSfarsit AS Date)) THEN ISNULL(dc.rez, B.F10053)) ELSE "
                        //            + " (SELECT MAX(C.IdCentruCost) AS F06204Default FROM F100CentreCost C WHERE A.F10003 = C.F10003 AND CAST(C.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(C.DataSfarsit AS Date)) END";
                        //    inn += " OUTER APPLY dbo.DamiCC(A.F10003, A.Ziua) dc";
                        //}
                        if (chkStr == true)
                        {
                            act += ",A.F10002=G.F00603, A.F10004=G.F00604, A.F10005=G.F00605, A.F10006=G.F00606, A.F10007=G.F00607";
                            inn += " OUTER APPLY dbo.DamiDept(A.F10003, A.Ziua) dd " +
                                   " LEFT JOIN F006 G ON G.F00607 = dd.Dept";
                            //Florin 2018.10.23
                            if (Dami.ValoareParam("TipCalculDate") == "2")
                                inn += "LEFT JOIN DamiDept_Table ddt ON ddt.F10003=A.F10003 AND ddt.dt=A.Ziua";
                        }


                        if (act != "") act = act.Substring(1);
                        strSql = string.Format(strSql, angIn, angSf, General.ToDataUniv(ziIn), General.ToDataUniv(ziSf), act, inn);
                    }

                }
                else
                {

                    //Radu 12.02.2019
                    if (chkPerAng == true)
                    {
                        string ziInceput = General.ToDataUniv(ziIn.Year, ziIn.Month, 1);
                        string ziSfarsit = General.ToDataUniv(ziSf.Year, ziSf.Month, 99);
            

                        string strDel = @"DELETE FROM ""Ptj_Intrari"" 
                                        WHERE ""IdAuto"" IN 
                                        (SELECT A.""IdAuto""
                                        FROM ""Ptj_Intrari"" A
                                        INNER JOIN (select f100.F10003, NVL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                        WHERE TRUNC(NVL(MODIF.DATA, f10023)) >= {0} AND TRUNC(NVL(MODIF.DATA, f10023)) <> TO_DATE('01-JAN-2100','DD-MON-YYYY')) B 
                                        ON A.F10003=B.F10003 AND A.""Ziua"" > B.DATA_PLECARII AND A.F10003 >= {1} AND A.F10003 <= {2})";

                        strDel = string.Format(strDel, ziInceput, angIn, angSf);
                        ras = General.ExecutaNonQuery(strDel, null);

                        strDel = @"DELETE FROM ""Ptj_Intrari"" 
                                        WHERE ""IdAuto"" IN 
                                        (SELECT A.""IdAuto""
                                        FROM ""Ptj_Intrari"" A
                                        INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE TRUNC(f10022) <= {0} AND TRUNC(F10022) <> TO_DATE('01-JAN-2100','DD-MON-YYYY')) B 
                                        ON A.F10003=B.F10003 AND A.""Ziua"" < B.F10022 AND A.F10003 >= {1} AND A.F10003 <= {2})";

                        strDel = string.Format(strDel, ziSfarsit, angIn, angSf);
                        ras = General.ExecutaNonQuery(strDel, null);
                    }

                    //Radu 12.02.2019
                    if (chkRecalc == true)
                    {
                        int an = ziIn.Year;
                        int luna = ziIn.Month;

                        string sql = "SELECT F10003 FROM F100 WHERE F10025 IN (0, 999) AND " + angIn + " <= F10003 AND F10003 <= " + angSf;
                        DataTable dt = General.IncarcaDT(sql, null);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            while (an < ziSf.Year || (an == ziSf.Year && luna <= ziSf.Month))
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                    General.CalculFormuleCumulat(Convert.ToInt32(dt.Rows[i][0].ToString()), an, luna);
                                luna++;
                                if (luna > 12)
                                {
                                    luna = 1;
                                    an++;
                                }
                            }
                        }
                        ras = true;
                    }


                    if (chkCtr || chkNrm || chkStr)
                    {
                        strSql += @"UPDATE ""Ptj_Intrari"" A
                                SET {4} 
                                WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.""Ziua"" AND A.""Ziua"" <= {3};";

                        if (chkCtr == true) act += ",A.\"IdContract\"=(SELECT MAX(B.\"IdContract\") AS \"IdContract\" FROM \"F100Contracte\" B WHERE A.F10003 = B.F10003 AND CAST(B.\"DataInceput\" AS \"Date\") <= CAST(A.\"Ziua\" AS \"Date\") AND CAST(A.\"Ziua\" AS \"Date\") <= CAST(B.\"DataSfarsit\" AS \"Date\"))";
                        if (chkNrm == true) act += ",A.\"Norma\"=COALESCE(\"DamiNorma\"(A.F10003, A.\"Ziua\"), (SELECT C.F10043 FROM F100 C WHERE C.F10003=A.F10003))";
                        if (chkStr == true) act += ",A.F10007=COALESCE(\"DamiDept\"(A.F10003, A.\"Ziua\"), (SELECT C.F10007 FROM F100 C WHERE C.F10003=A.F10003))";
                        //if (chkCC == true)
                        //    act += ",A.\"F06204Default\"=CASE WHEN NOT EXISTS(SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS \"Date\") <= CAST(A.\"Ziua\" AS \"Date\") AND CAST(A.\"Ziua\" AS \"Date\") <= CAST(C.\"DataSfarsit\" AS \"Date\")) THEN COALESCE(\"DamiCC\"(A.F10003, A.\"Ziua\"), (SELECT C.F10053 FROM F100 C WHERE C.F10003=A.F10003)) ELSE "
                        //          + " (SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS \"Date\") <= CAST(A.\"Ziua\" AS \"Date\") AND CAST(A.\"Ziua\" AS \"Date\") <= CAST(C.\"DataSfarsit\" AS \"Date\")) END";

                        if (chkStr == true)
                        {
                            string strStruct = @"UPDATE ""Ptj_Intrari"" A
                                            SET 
                                            A.F10002 = (SELECT C.F00603 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10004 = (SELECT C.F00604 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10005 = (SELECT C.F00605 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10006 = (SELECT C.F00606 FROM F006 C WHERE A.F10007=C.F00607)
                                            FROM A 
                                            WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.""Ziua"" AND A.""Ziua"" <= {3};";

                            strSql += strStruct;
                        }

                        if (act != "") act = act.Substring(1);
                        strSql = string.Format(strSql, angIn, angSf, General.ToDataUniv(ziIn), General.ToDataUniv(ziSf), act);
                        strSql = "BEGIN " + strSql + " END;";
                    }
                }

                if (strSql.Length > 0)
                    ras = General.ExecutaNonQuery(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;

        }



    }
}