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
                btnAct.Text = Dami.TraduCuvant("btnAct", "Actualizeaza");

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
                if (txtDataInc.Value == null || txtDataSf.Value == null || txtMarcaInc.Value == null || txtMarcaSf.Value == null || (chkCtr.Checked == false && chkStr.Checked == false && chkNrm.Checked == false && chkPerAng.Checked == false && chkRecalc.Checked == false && chkCC.Checked == false))
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "");
                    return;
                }

                if (Convert.ToInt32(txtMarcaInc.Value) > Convert.ToInt32(txtMarcaSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Marca inceput este mai mare decat marca sfarsit !"), MessageBox.icoError, "");
                    return;
                }

                if (Convert.ToDateTime(txtDataInc.Value) > Convert.ToDateTime(txtDataSf.Value))
                {
                    MessageBox.Show(Dami.TraduCuvant("Data inceput este mai mare decat data sfarsit !"), MessageBox.icoError, "");
                    return;
                }

                if (ActualizeazaDateGenerale(Convert.ToInt32(Session["UserId"]), Convert.ToInt32(txtMarcaInc.Value), Convert.ToInt32(txtMarcaSf.Value), 
                    Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), Convert.ToBoolean(chkCtr.Checked), 
                    Convert.ToBoolean(chkNrm.Checked), Convert.ToBoolean(chkStr.Checked), Convert.ToBoolean(chkPerAng.Checked), Convert.ToBoolean(chkRecalc.Checked), Convert.ToBoolean(chkCC.Checked)))
                    MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes !"), MessageBox.icoSuccess, "");
                else
                    MessageBox.Show(Dami.TraduCuvant("Eroare in proces!"), MessageBox.icoError, "");

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

                ziIn = new DateTime(ziIn.Year, ziIn.Month, ziIn.Day);
                ziSf = new DateTime(ziSf.Year, ziSf.Month, ziSf.Day);

                string ziInceput = General.ToDataUniv(ziIn.Year, ziIn.Month, 1);
                string ziSfarsit = General.ToDataUniv(ziSf.Year, ziSf.Month, 99);

                string sqlPrg = "";

                if (Constante.tipBD == 1)
                {
                    //Florin 2019.12.05 - am adaugat Ptj_IstoricVal
                    if (chkPerAng == true)
                    {
                        string strDel = $@"
                            BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Meu', {Session["UserId"]}, {General.CurrentDate()}
                                FROM Ptj_Intrari A
                                INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                ) B 
                                ON A.F10003=B.F10003 AND A.Ziua> B.DATA_PLECARII AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit} AND CONVERT(date,DATA_PLECARII) <> '2100-01-01';

                                DELETE A
                                FROM Ptj_Intrari A
                                INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                ) B 
                                ON A.F10003=B.F10003 AND A.Ziua> B.DATA_PLECARII AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit} AND CONVERT(date,DATA_PLECARII) <> '2100-01-01';
                            END;";

                        ras = General.ExecutaNonQuery(strDel, null);

                        strDel = $@"
                            BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Meu', {Session["UserId"]}, {General.CurrentDate()}
                                FROM Ptj_Intrari A
                                INNER JOIN(SELECT F10003, F10022 FROM f100 WHERE CONVERT(date, F10022) <> '2100-01-01') B
                                ON A.F10003 = B.F10003 AND A.Ziua < B.F10022 AND A.F10003 >= {angIn}
                                AND A.F10003 <= {angSf}
                                AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit};

                                DELETE A
                                FROM Ptj_Intrari A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE CONVERT(date,F10022) <> '2100-01-01') B 
                                ON A.F10003=B.F10003 AND A.Ziua< B.F10022 AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit};
                            END;";

                        ras = General.ExecutaNonQuery(strDel, null);
                    }

                    if (chkRecalc)
                    {
                        string sqlCum = $@"
                            SELECT A.F10003, YEAR(X.""Zi"") AS ""An"", MONTH(X.""Zi"") AS ""Luna"" 
                            FROM ""tblZile"" X
                            INNER JOIN F100 A ON {angIn} <= A.F10003 AND A.F10003 <= {angSf} AND A.F10022 <= X.""Zi"" AND X.""Zi"" <= A.F10023
                            WHERE {General.ToDataUniv(ziIn)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(ziSf)}
                            GROUP BY A.F10003, YEAR(X.""Zi""), MONTH(X.""Zi"")";

                        if (Constante.tipBD == 2)
                            sqlCum = $@"
                            SELECT A.F10003, TO_NUMBER(TO_CHAR(X.""Zi"", 'YYYY')) AS ""An"", TO_NUMBER(TO_CHAR(X.""Zi"", 'MM')) AS ""Luna"" 
                            FROM ""tblZile"" X
                            INNER JOIN F100 A ON {angIn} <= A.F10003 AND A.F10003 <= {angSf} AND A.F10022 <= X.""Zi"" AND X.""Zi"" <= A.F10023
                            WHERE {General.ToDataUniv(ziIn)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(ziSf)}
                            GROUP BY A.F10003, TO_NUMBER(TO_CHAR(X.""Zi"", 'YYYY')), TO_NUMBER(TO_CHAR(X.""Zi"", 'MM'))";

                        DataTable dt = General.IncarcaDT(sqlCum, null);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            General.CalculFormuleCumulat(Convert.ToInt32(dt.Rows[i]["F10003"]), Convert.ToInt32(dt.Rows[i]["An"]), Convert.ToInt32(dt.Rows[i]["Luna"]));
                        }
                        ras = true;
                    }



                    if (chkCtr || chkNrm || chkStr || chkCC)
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
                                inn += " LEFT JOIN DamiNorma_Table dnt ON dnt.F10003=A.F10003 AND dnt.dt=A.Ziua";
                        }
                        if (chkCC == true)
                        {
                            act += @",A.F06204Default=
                                    COALESCE(
                                    (SELECT MAX(C.IdCentruCost) AS F06204Default FROM F100CentreCost C WHERE A.F10003 = C.F10003 AND CAST(C.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(C.DataSfarsit AS Date))
                                    ,ISNULL(dc.CC, B.F10053)
                                    )";
                            inn += " OUTER APPLY dbo.DamiCC(A.F10003, A.Ziua) dc";
                        }
                        if (chkStr == true)
                        {
                            act += ",A.F10002=G.F00603, A.F10004=G.F00604, A.F10005=G.F00605, A.F10006=G.F00606, A.F10007=G.F00607";
                            inn += " OUTER APPLY dbo.DamiDept(A.F10003, A.Ziua) dd " +
                                   " LEFT JOIN F006 G ON G.F00607 = dd.Dept";
                            //Florin 2018.10.23
                            if (Dami.ValoareParam("TipCalculDate") == "2")
                                inn += " LEFT JOIN DamiDept_Table ddt ON ddt.F10003=A.F10003 AND ddt.dt=A.Ziua";
                        }


                        if (act != "") act = act.Substring(1);
                        strSql = string.Format(strSql, angIn, angSf, General.ToDataUniv(ziIn), General.ToDataUniv(ziSf), act, inn);

                        //Florin 2019.12.10 - daca este contract, actualizam si programul
                        if (chkCtr == true)
                            sqlPrg = $@"UPDATE X
                                    SET X.IdProgram=
                                    CASE WHEN (COALESCE(X.""ZiLiberaLegala"",0) = 1 AND Y.""TipSchimb8"" = 1) THEN  COALESCE(Y.""Program8"", Y.""Program0"") ELSE
                                    CASE (X.""ZiSapt"")
                                    WHEN 1 THEN(CASE WHEN COALESCE(Y.""TipSchimb1"", 1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"") END)
                                    WHEN 2 THEN(CASE WHEN COALESCE(Y.""TipSchimb2"", 1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"") END)
                                    WHEN 3 THEN(CASE WHEN COALESCE(Y.""TipSchimb3"", 1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"") END)
                                    WHEN 4 THEN(CASE WHEN COALESCE(Y.""TipSchimb4"", 1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"") END)
                                    WHEN 5 THEN(CASE WHEN COALESCE(Y.""TipSchimb5"", 1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"") END)
                                    WHEN 6 THEN(CASE WHEN COALESCE(Y.""TipSchimb6"", 1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"") END)
                                    WHEN 7 THEN(CASE WHEN COALESCE(Y.""TipSchimb7"", 1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"") END)
                                    END END
                                    FROM Ptj_Intrari X
                                    INNER JOIN Ptj_Contracte Y ON X.IdContract = Y.Id
                                    WHERE @1 <= X.F10003 AND X.F10003 <= @2 AND @3 <= X.Ziua AND X.Ziua <= @4";
                    }
                }
                else
                {
                    if (chkPerAng == true)
                    {
                        string strDel = 
                            $@"BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Meu', {Session["UserId"]}, {General.CurrentDate()}
                                FROM ""Ptj_Intrari"" A
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (select f100.F10003, NVL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                ) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" > B.DATA_PLECARII AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= TRUNC(A.""Ziua"") AND TRUNC(A.""Ziua"") <= {ziSfarsit} AND TRUNC(B.DATA_PLECARII) <> TO_DATE('01-01-2100','DD-MM-YYYY'));

                                DELETE FROM ""Ptj_Intrari"" 
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (select f100.F10003, NVL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403
                                ) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" > B.DATA_PLECARII AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= TRUNC(A.""Ziua"") AND TRUNC(A.""Ziua"") <= {ziSfarsit} AND TRUNC(B.DATA_PLECARII) <> TO_DATE('01-01-2100','DD-MM-YYYY'));
                            END;";

                        ras = General.ExecutaNonQuery(strDel, null);

                        strDel = 
                            $@"BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Pontajul Meu', {Session["UserId"]}, {General.CurrentDate()}
                                FROM ""Ptj_Intrari"" A
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE  TRUNC(F10022) <> TO_DATE('01-01-2100','DD-MM-YYYY')) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" < B.F10022 AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.""Ziua"" AND A.""Ziua"" <= {ziSfarsit});

                                DELETE FROM ""Ptj_Intrari"" 
                                WHERE ""IdAuto"" IN 
                                (SELECT A.""IdAuto""
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN (SELECT F10003, F10022 FROM f100 WHERE  TRUNC(F10022) <> TO_DATE('01-01-2100','DD-MM-YYYY')) B 
                                ON A.F10003=B.F10003 AND A.""Ziua"" < B.F10022 AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.""Ziua"" AND A.""Ziua"" <= {ziSfarsit});
                            END;";

                        ras = General.ExecutaNonQuery(strDel, null);
                    }

                    if (chkRecalc)
                    {
                        string sqlCum = $@"
                            SELECT A.F10003, YEAR(X.""Zi"") AS ""An"", MONTH(X.""Zi"") AS ""Luna"" 
                            FROM ""tblZile"" X
                            INNER JOIN F100 A ON {angIn} <= A.F10003 AND A.F10003 <= {angSf} AND A.F10022 <= X.""Zi"" AND X.""Zi"" <= A.F10023
                            WHERE {General.ToDataUniv(ziIn)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(ziSf)}
                            GROUP BY A.F10003, YEAR(X.""Zi""), MONTH(X.""Zi"")";

                        if (Constante.tipBD == 2)
                            sqlCum = $@"
                            SELECT A.F10003, TO_NUMBER(TO_CHAR(X.""Zi"", 'YYYY')) AS ""An"", TO_NUMBER(TO_CHAR(X.""Zi"", 'MM')) AS ""Luna"" 
                            FROM ""tblZile"" X
                            INNER JOIN F100 A ON {angIn} <= A.F10003 AND A.F10003 <= {angSf} AND A.F10022 <= X.""Zi"" AND X.""Zi"" <= A.F10023
                            WHERE {General.ToDataUniv(ziIn)} <= X.""Zi"" AND X.""Zi"" <= {General.ToDataUniv(ziSf)}
                            GROUP BY A.F10003, TO_NUMBER(TO_CHAR(X.""Zi"", 'YYYY')), TO_NUMBER(TO_CHAR(X.""Zi"", 'MM'))";

                        DataTable dt = General.IncarcaDT(sqlCum, null);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            General.CalculFormuleCumulat(Convert.ToInt32(dt.Rows[i]["F10003"]), Convert.ToInt32(dt.Rows[i]["An"]), Convert.ToInt32(dt.Rows[i]["Luna"]));
                        }
                        ras = true;
                    }


                    if (chkCtr || chkNrm || chkStr || chkCC)
                    {
                        strSql += @"UPDATE ""Ptj_Intrari"" A
                                SET {4} 
                                WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.""Ziua"" AND A.""Ziua"" <= {3};";

                        if (chkCtr == true) act += ",A.\"IdContract\"=(SELECT MAX(B.\"IdContract\") AS \"IdContract\" FROM \"F100Contracte\" B WHERE A.F10003 = B.F10003 AND CAST(B.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(B.\"DataSfarsit\" AS Date))";
                        if (chkNrm == true) act += ",A.\"Norma\"=COALESCE(\"DamiNorma\"(A.F10003, A.\"Ziua\"), (SELECT C.F10043 FROM F100 C WHERE C.F10003=A.F10003))";
                        if (chkStr == true) act += ",A.F10007=COALESCE(\"DamiDept\"(A.F10003, A.\"Ziua\"), (SELECT C.F10007 FROM F100 C WHERE C.F10003=A.F10003))";
                        if (chkCC == true)
                            act += ",A.\"F06204Default\"=CASE WHEN NOT EXISTS(SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(C.\"DataSfarsit\" AS Date)) THEN COALESCE(\"DamiCC\"(A.F10003, A.\"Ziua\"), (SELECT C.F10053 FROM F100 C WHERE C.F10003=A.F10003)) ELSE "
                                  + " (SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(C.\"DataSfarsit\" AS Date)) END";

                        if (chkStr == true)
                        {
                            string strStruct = @"UPDATE ""Ptj_Intrari"" A
                                            SET 
                                            A.F10002 = (SELECT C.F00603 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10004 = (SELECT C.F00604 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10005 = (SELECT C.F00605 FROM F006 C WHERE A.F10007=C.F00607),
                                            A.F10006 = (SELECT C.F00606 FROM F006 C WHERE A.F10007=C.F00607)
                                            WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.""Ziua"" AND A.""Ziua"" <= {3};";

                            strSql += strStruct;
                        }

                        if (act != "") act = act.Substring(1);
                        strSql = string.Format(strSql, angIn, angSf, General.ToDataUniv(ziIn), General.ToDataUniv(ziSf), act);
                        strSql = "BEGIN " + strSql + " END;";
                    }

                    //Florin 2019.12.10 - daca este contract, actualizam si programul
                    if (chkCtr == true)
                        sqlPrg = $@"UPDATE ""Ptj_Intrari"" X
                                SET X.""IdProgram"" =
                                (SELECT
                                CASE WHEN(COALESCE(X.""ZiLiberaLegala"",0) = 1 AND Y.""TipSchimb8"" = 1) THEN COALESCE(Y.""Program8"", Y.""Program0"") ELSE
                                CASE(X.""ZiSapt"")
                                WHEN 1 THEN(CASE WHEN COALESCE(Y.""TipSchimb1"", 1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"") END)
                                WHEN 2 THEN(CASE WHEN COALESCE(Y.""TipSchimb2"", 1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"") END)
                                WHEN 3 THEN(CASE WHEN COALESCE(Y.""TipSchimb3"", 1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"") END)
                                WHEN 4 THEN(CASE WHEN COALESCE(Y.""TipSchimb4"", 1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"") END)
                                WHEN 5 THEN(CASE WHEN COALESCE(Y.""TipSchimb5"", 1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"") END)
                                WHEN 6 THEN(CASE WHEN COALESCE(Y.""TipSchimb6"", 1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"") END)
                                WHEN 7 THEN(CASE WHEN COALESCE(Y.""TipSchimb7"", 1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"") END)
                                END END
                                FROM ""Ptj_Contracte"" Y WHERE X.""IdContract"" = Y.""Id"")
                                WHERE @1 <= X.F10003 AND X.F10003 <= @2 AND @3 <= X.""Ziua"" AND X.""Ziua"" <= @4";
                }

                if (strSql.Length > 0)
                    ras = General.ExecutaNonQuery(strSql, null);

                if (sqlPrg.Length > 0)
                    General.ExecutaNonQuery(sqlPrg, new object[] { angIn, angSf, ziIn, ziSf });

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