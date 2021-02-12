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

                //Radu 09.12.2019
                lblCtrInc.InnerText = Dami.TraduCuvant("Data Inceput");
                lblCtrSf.InnerText = Dami.TraduCuvant("Data Sfarsit");
                lblMarcaInc.InnerText = Dami.TraduCuvant("Marca Inceput");
                lblMarcaSf.InnerText = Dami.TraduCuvant("Marca Sfarsit");
                chkCtr.Text = Dami.TraduCuvant("Contract");
                chkStr.Text = Dami.TraduCuvant("Structura Org.");
                chkNrm.Text = Dami.TraduCuvant("Norma");
                chkPerAng.Text = Dami.TraduCuvant("Perioada angajare");
                chkRecalc.Text = Dami.TraduCuvant("Recalcul totaluri");
                chkCC.Text = Dami.TraduCuvant("Centrul de cost");
                chkSL.Text = Dami.TraduCuvant("Sarbatori legale");
                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                if (!IsPostBack)
                {
                    txtMarcaInc.Value = 1;
                    txtMarcaSf.Value = 999999;
                    txtDataInc.Value = DateTime.Now;
                    txtDataSf.Value = DateTime.Now;

                    GetDataBlocare(DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GetDataBlocare(DateTime dataInc)
        {
            string idRol = "-99";
            string strSql = $@"SELECT X.""IdRol"", X.""RolDenumire"" FROM ({SelectComun(dataInc)}) X 
                                WHERE X.F10022 <= {General.ToDataUniv(dataInc.Year, dataInc.Month, 99)} AND {General.ToDataUniv(dataInc.Year, dataInc.Month)} <= X.F10023
                                GROUP BY X.""IdRol"", X.""RolDenumire""
                                ORDER BY X.""RolDenumire"" ";
            DataTable dtRol = General.IncarcaDT(strSql, null);
            if (dtRol != null && dtRol.Rows.Count > 0)
            {
                idRol = "";
                for (int i = 0; i < dtRol.Rows.Count; i++)
                    idRol += dtRol.Rows[i]["IdRol"].ToString() + ",";
                idRol = idRol.Substring(0, idRol.Length - 1);
            }

            //Radu 09.01.2020
            string dataBlocare = "22001231";
            strSql = $@"SELECT COALESCE(MIN(Ziua),'2200-12-31') FROM Ptj_tblBlocarePontaj WHERE IdRol IN (" + idRol + ")";
            if (Constante.tipBD == 2)
                strSql = @"SELECT COALESCE(MIN(""Ziua""),TO_DATE('31-12-2200','DD-MM-YYYY')) FROM ""Ptj_tblBlocarePontaj"" WHERE ""IdRol"" IN (" + idRol + ")";
            DataTable dt = General.IncarcaDT(strSql, null);
            if (dt != null && dt.Rows.Count > 0 && General.Nz(dt.Rows[0][0], "").ToString() != "" && General.IsDate(dt.Rows[0][0]))
                dataBlocare = Convert.ToDateTime(dt.Rows[0][0]).Year + Convert.ToDateTime(dt.Rows[0][0]).Month.ToString().PadLeft(2, '0') + Convert.ToDateTime(dt.Rows[0][0]).Day.ToString().PadLeft(2, '0');
            Session["Ptj_DataBlocare"] = dataBlocare.ToString();
        }


        private string SelectComun(DateTime dtInc)
        {
            string strSql = "";
            try
            {
                string semn = "+";
                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2)
                {
                    semn = "||";
                    cmp = "ROWNUM";
                }

                DateTime dtData = dtInc;

                strSql = @"SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) AS int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Denumire"", '') AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN ""Ptj_tblRoluri"" S ON C.""IdRol""=S.""Id""
                                WHERE C.""IdSuper"" = {0}
                                UNION
                                SELECT B.F10003 AS F10003, A.F10008 {1} ' ' {1} a.F10009 AS ""NumeComplet"", A.F10008 AS ""Nume"", A.F10009 AS ""Prenume"", 
                                A.F10017 AS ""CNP"", A.F10022 AS ""DataAngajarii"",A.F10011 AS ""NrContract"", E.F00204 AS ""Companie"", F.F00305 AS ""Subcompanie"", 
                                G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament"", D.F71804 AS ""Functia"", 
                                CAST(COALESCE(A.F10043,0) as int) AS ""Norma"", A.F100901, A.F10022, A.F10023, COALESCE(C.""IdRol"",1) AS ""IdRol"", COALESCE(S.""Denumire"", '') AS ""RolDenumire"", COALESCE(A.F10025,0) AS F10025
                                FROM ""relGrupAngajat"" B
                                INNER JOIN ""Ptj_relGrupSuper"" C ON b.""IdGrup"" = c.""IdGrup""
                                INNER JOIN F100 A ON b.F10003 = a.F10003
                                INNER JOIN ""F100Supervizori"" J ON B.F10003 = J.F10003 AND C.""IdSuper"" = (-1 * J.""IdSuper"")
                                LEFT JOIN F718 D ON A.F10071 = D.F71802
                                LEFT JOIN F002 E ON A.F10002 = E.F00202
                                LEFT JOIN F003 F ON A.F10004 = F.F00304
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607
                                LEFT JOIN ""Ptj_tblRoluri"" S ON C.""IdRol""=S.""Id""
                                WHERE J.""IdUser"" = {0}";

                strSql = string.Format(strSql, Session["UserId"], semn, cmp);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void btnAct_Click(object sender, EventArgs e)
        {
            Act(1);
        }


        public bool ActualizeazaDateGenerale(int idUser, int angIn, int angSf, DateTime ziIn, DateTime ziSf, bool chkCtr, bool chkNrm, bool chkStr, bool chkPerAng, bool chkRecalc, bool chkCC, bool chkSL)
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
                string sqlCalc = "";


                //Radu 12.02.2021 - s-a revenit la functia DamiDataPlecare  
                //INNER JOIN (select f100.F10003, ISNULL(MODIF.DATA, f10023) DATA_PLECARII from f100 left join(select f70403, min(f70406) - 1 data from f704 where f70404 = 4 group by f70403) modif on F100.F10003 = MODIF.F70403 ) B ON A.F10003=B.F10003 AND

                if (Constante.tipBD == 1)
                {
                    //Florin 2019.12.05 - am adaugat Ptj_IstoricVal
                    if (chkPerAng == true)
                    { 
                        string strDel = $@"
                            BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare date pontaj', {Session["UserId"]}, {General.CurrentDate()}
                                FROM Ptj_Intrari A
                                OUTER APPLY dbo.DamiDataPlecare(A.F10003, A.Ziua) ddp
                                
                                WHERE A.Ziua> ddp.DataPlecare AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit} AND CONVERT(date,ddp.DataPlecare) <> '2100-01-01';

                                DELETE A
                                FROM Ptj_Intrari A
                                OUTER APPLY dbo.DamiDataPlecare(A.F10003, A.Ziua) ddp
                                WHERE A.Ziua> ddp.DataPlecare AND A.F10003 >= {angIn} AND A.F10003 <= {angSf} AND {ziInceput} <= A.Ziua AND A.Ziua <= {ziSfarsit} AND CONVERT(date,ddp.DataPlecare) <> '2100-01-01';
                            END;";

                        ras = General.ExecutaNonQuery(strDel, null);

                        strDel = $@"
                            BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare date pontaj', {Session["UserId"]}, {General.CurrentDate()}
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
                        General.CalculFormuleCumulat($@"{angIn} <= ent.F10003 AND ent.F10003 <= {angSf} AND {ziIn.Year * 100 + ziIn.Month} <= (ent.""An"" * 100 + ent.""Luna"") AND (ent.""An"" * 100 + ent.""Luna"") <= {ziSf.Year * 100 + ziSf.Month}");
                        ras = true;
                    }

                    if (chkCtr || chkNrm || chkStr || chkCC || chkSL)
                    {
                        strSql += @"UPDATE A 
                                SET {4} 
                                FROM Ptj_Intrari A
                                INNER JOIN F100 B ON A.F10003=B.F10003
                                {5}
                                WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.Ziua AND A.Ziua <= {3};";

                        if (chkCtr == true) act += ",A.IdContract = CASE WHEN COALESCE(A.ModifProgram,0) = 1 THEN A.IdContract ELSE (SELECT MAX(B.IdContract) AS IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CAST(B.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(B.DataSfarsit AS Date)) END";
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
                            //Florin 2020.07.02 - am adaugat subdept si birou
                            //Radu 03.02.2021 - am inlocuit DamiSubdept si DamiBirou
                            //OUTER APPLY dbo.DamiSubdept(A.F10003, A.Ziua) sd 
                            //OUTER APPLY dbo.DamiBirou(A.F10003, A.Ziua) br


                            act += ",A.F10002=G.F00603, A.F10004=G.F00604, A.F10005=G.F00605, A.F10006=G.F00606, A.F10007=G.F00607, A.F100958=dd.Subdept, A.F100959=dd.Birou";
                            inn += " OUTER APPLY dbo.DamiDept(A.F10003, A.Ziua) dd " +
                                   " LEFT JOIN F006 G ON G.F00607 = dd.Dept " +
                                   " LEFT JOIN F007 H ON H.F00708 = dd.Subdept " +
                                   " LEFT JOIN F008 I ON I.F00809 = dd.Birou ";
                            
                                
                            //Florin 2018.10.23
                            if (Dami.ValoareParam("TipCalculDate") == "2")
                                inn += " LEFT JOIN DamiDept_Table ddt ON ddt.F10003=A.F10003 AND ddt.dt=A.Ziua";
                        }
                        //Radu 07.04.2020
                        if (chkSL == true)
                        {
                            act += ",A.ZiLibera = 1, A.ZiLiberaLegala = 1";
                            inn += "JOIN HOLIDAYS ON CAST(DAY AS DATE)= CAST(A.Ziua AS DATE)";
                        }

                        if (act != "") act = act.Substring(1);
                        strSql = string.Format(strSql, angIn, angSf, General.ToDataUniv(ziIn), General.ToDataUniv(ziSf), act, inn);

                        //Florin 2020.09.07 - adaugat recalcul
                        //Florin 2019.12.10 - daca este contract, actualizam si programul
                        if (chkCtr == true)
                        {
                            sqlPrg = $@"UPDATE X
                                    SET X.IdProgram=
                                    CASE WHEN (COALESCE(X.""ZiLiberaLegala"",0) = 1 AND Y.""TipSchimb8"" = 1) THEN  COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                    CASE (X.""ZiSapt"")
                                    WHEN 1 THEN(CASE WHEN COALESCE(Y.""TipSchimb1"", 1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) END)
                                    WHEN 2 THEN(CASE WHEN COALESCE(Y.""TipSchimb2"", 1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) END)
                                    WHEN 3 THEN(CASE WHEN COALESCE(Y.""TipSchimb3"", 1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) END)
                                    WHEN 4 THEN(CASE WHEN COALESCE(Y.""TipSchimb4"", 1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) END)
                                    WHEN 5 THEN(CASE WHEN COALESCE(Y.""TipSchimb5"", 1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) END)
                                    WHEN 6 THEN(CASE WHEN COALESCE(Y.""TipSchimb6"", 1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) END)
                                    WHEN 7 THEN(CASE WHEN COALESCE(Y.""TipSchimb7"", 1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) END)
                                    END END
                                    FROM Ptj_Intrari X
                                    INNER JOIN Ptj_Contracte Y ON X.IdContract = Y.Id
                                    WHERE @1 <= X.F10003 AND X.F10003 <= @2 AND @3 <= X.Ziua AND X.Ziua <= @4 AND COALESCE(X.ModifProgram,0) = 0";

                            sqlCalc = $@"SELECT CONVERT(nvarchar(20),A.F10003) + ';' + CONVERT(nvarchar(10),A.Ziua,103) + '#'
                                    FROM Ptj_Intrari A
                                    INNER JOIN F100 B ON A.F10003=B.F10003                        
                                    WHERE @1 <= A.F10003 AND A.F10003 <= @2 AND @3 <= A.Ziua AND A.Ziua <= @4
                                    AND COALESCE(A.ModifProgram,0) <> 1 AND
                                    A.IdContract <> (SELECT MAX(B.IdContract) AS IdContract FROM F100Contracte B WHERE A.F10003 = B.F10003 AND CAST(B.DataInceput AS Date) <= CAST(A.Ziua AS Date) AND CAST(A.Ziua AS Date) <= CAST(B.DataSfarsit AS Date)) 
                                    FOR XML PATH('')";
                        }
                    }
                }
                else
                {
                    if (chkPerAng == true)
                    {
                        string strDel = 
                            $@"BEGIN
                                INSERT INTO ""Ptj_IstoricVal""(F10003, ""Ziua"", ""ValStr"", ""ValStrOld"", ""IdUser"", ""DataModif"", ""Observatii"", USER_NO, TIME)
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare date pontaj', {Session["UserId"]}, {General.CurrentDate()}
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
                                SELECT A.F10003, A.""Ziua"", NULL, A.""ValStr"", {Session["UserId"]}, {General.CurrentDate()}, 'Actualizare date pontaj', {Session["UserId"]}, {General.CurrentDate()}
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
                        General.CalculFormuleCumulat($@"{angIn} <= ent.F10003 AND ent.F10003 <= {angSf} AND {ziIn.Year * 100 + ziIn.Month} <= (ent.""An"" * 100 + ent.""Luna"") AND (ent.""An"" * 100 + ent.""Luna"") <= {ziSf.Year * 100 + ziSf.Month}");
                        ras = true;
                    }

                    if (chkCtr || chkNrm || chkStr || chkCC || chkSL)
                    {
                        strSql += @"UPDATE ""Ptj_Intrari"" A
                                SET {4} 
                                WHERE {0} <= A.F10003 AND A.F10003 <= {1} AND {2} <= A.""Ziua"" AND A.""Ziua"" <= {3};";

                        if (chkCtr == true) act += ",A.\"IdContract\" = CASE WHEN COALESCE(A.\"ModifProgram\",0) = 1 THEN A.\"IdContract\" ELSE (SELECT MAX(B.\"IdContract\") AS \"IdContract\" FROM \"F100Contracte\" B WHERE A.F10003 = B.F10003 AND CAST(B.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(B.\"DataSfarsit\" AS Date)) END";
                        if (chkNrm == true) act += ",A.\"Norma\"=COALESCE(\"DamiNorma\"(A.F10003, A.\"Ziua\"), (SELECT C.F10043 FROM F100 C WHERE C.F10003=A.F10003))";
                        if (chkStr == true) act += 
                                $@",A.F10007=COALESCE(""DamiDept""(A.F10003, A.""Ziua""), (SELECT C.F10007 FROM F100 C WHERE C.F10003=A.F10003))
                                   ,A.F100958=COALESCE(""DamiSubdept""(A.F10003, A.""Ziua""), (SELECT C.F100958 FROM F1001 C WHERE C.F10003=A.F10003))
                                   ,A.F100959=COALESCE(""DamiBirou""(A.F10003, A.""Ziua""), (SELECT C.F100959 FROM F1001 C WHERE C.F10003=A.F10003))";
                        if (chkCC == true)
                            act += ",A.\"F06204Default\"=CASE WHEN NOT EXISTS(SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(C.\"DataSfarsit\" AS Date)) THEN COALESCE(\"DamiCC\"(A.F10003, A.\"Ziua\"), (SELECT C.F10053 FROM F100 C WHERE C.F10003=A.F10003)) ELSE "
                                  + " (SELECT MAX(C.\"IdCentruCost\") AS \"F06204Default\" FROM \"F100CentreCost\" C WHERE A.F10003 = C.F10003 AND CAST(C.\"DataInceput\" AS Date) <= CAST(A.\"Ziua\" AS Date) AND CAST(A.\"Ziua\" AS Date) <= CAST(C.\"DataSfarsit\" AS Date)) END";

                        //Radu 07.04.2020
                        if (chkSL == true) act += ", A.\"ZiLibera\" = CASE WHEN NOT EXISTS(SELECT 1 FROM HOLIDAYS WHERE TRUNC(DAY) = TRUNC(A.\"Ziua\")) THEN 0 ELSE 1 END, A.\"ZiLiberaLegala\" = CASE WHEN NOT EXISTS(SELECT 1 FROM HOLIDAYS WHERE TRUNC(DAY) = TRUNC(A.\"Ziua\")) THEN 0 ELSE 1 END";


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

                    //Florin 2020.09.07 - adaugat recalcul
                    //Florin 2019.12.10 - daca este contract, actualizam si programul
                    if (chkCtr == true)
                    {
                        sqlPrg = $@"UPDATE ""Ptj_Intrari"" X
                                SET X.""IdProgram"" =
                                (SELECT
                                CASE WHEN(COALESCE(X.""ZiLiberaLegala"",0) = 1 AND Y.""TipSchimb8"" = 1) THEN COALESCE(Y.""Program8"", Y.""Program0"", -99) ELSE
                                CASE(X.""ZiSapt"")
                                WHEN 1 THEN(CASE WHEN COALESCE(Y.""TipSchimb1"", 1) = 1 THEN COALESCE(Y.""Program1"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 2 THEN(CASE WHEN COALESCE(Y.""TipSchimb2"", 1) = 1 THEN COALESCE(Y.""Program2"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 3 THEN(CASE WHEN COALESCE(Y.""TipSchimb3"", 1) = 1 THEN COALESCE(Y.""Program3"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 4 THEN(CASE WHEN COALESCE(Y.""TipSchimb4"", 1) = 1 THEN COALESCE(Y.""Program4"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 5 THEN(CASE WHEN COALESCE(Y.""TipSchimb5"", 1) = 1 THEN COALESCE(Y.""Program5"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 6 THEN(CASE WHEN COALESCE(Y.""TipSchimb6"", 1) = 1 THEN COALESCE(Y.""Program6"", Y.""Program0"", -99) ELSE -99 END)
                                WHEN 7 THEN(CASE WHEN COALESCE(Y.""TipSchimb7"", 1) = 1 THEN COALESCE(Y.""Program7"", Y.""Program0"", -99) ELSE -99 END)
                                END END
                                FROM ""Ptj_Contracte"" Y WHERE X.""IdContract"" = Y.""Id"")
                                WHERE @1 <= X.F10003 AND X.F10003 <= @2 AND @3 <= X.""Ziua"" AND X.""Ziua"" <= @4 AND COALESCE(X.""ModifProgram"",0) = 0";

                        sqlCalc = $@"SELECT LISTAGG(A.F10003 || ';' || TO_CHAR(A.""Ziua"",'DD/MM/YYYY'), '#') WITHIN GROUP (ORDER BY A.F10003, A.""Ziua"") 
                                FROM ""Ptj_Intrari"" A
                                INNER JOIN F100 B ON A.F10003=B.F10003                        
                                WHERE @1 <= A.F10003 AND A.F10003 <= @2 AND @3 <= A.""Ziua"" AND A.""Ziua"" <= @4
                                AND COALESCE(A.""ModifProgram"",0) <> 1 AND
                                A.""IdContract"" <> (SELECT MAX(B.""IdContract"") AS ""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= TRUNC(A.""Ziua"") AND TRUNC(A.""Ziua"") <= TRUNC(B.""DataSfarsit"")) ";
                    }
                }

                //Florin 2020.09.07 - adaugat recalcul
                string sirCalc = "";
                if (sqlCalc.Length > 0)
                    sirCalc = General.Nz(General.ExecutaScalar(sqlCalc, new object[] { angIn, angSf, ziIn, ziSf }), "").ToString();

                if (strSql.Length > 0)
                    ras = General.ExecutaNonQuery(strSql, null);

                if (sqlPrg.Length > 0)
                    General.ExecutaNonQuery(sqlPrg, new object[] { angIn, angSf, ziIn, ziSf });

                //Florin 2020.09.07 - adaugat recalcul
                if (sirCalc != "")
                {
                    string[] arr = sirCalc.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                    for(int i =0; i < arr.Length; i++)
                    {
                        string[] l = arr[i].Split(';');
                        if (l.Length == 2)
                        {
                            DateTime zi = DateTime.Now;
                            if (DateTime.TryParse(l[1], out zi))
                                General.CalculFormule(l[0], null, zi);   
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return ras;

        }

        protected void btnValStr_Click(object sender, EventArgs e)
        {
            try
            {
                General.SintaxaValStr();
                MessageBox.Show("Proces realizat cu succes", MessageBox.icoSuccess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            switch (e.Parameter.Split(';')[0])
            {
                case "btnAct":
                    Act(2);
                    break;
                case "txtDataInc":
                    GetDataBlocare(Convert.ToDateTime(e.Parameter.Split(';')[1]));
                    break;
            }
        }

        protected void Act(int param)
        {
            try
            {
                if (txtDataInc.Value == null || txtDataSf.Value == null || txtMarcaInc.Value == null || txtMarcaSf.Value == null || (chkCtr.Checked == false && chkStr.Checked == false && chkNrm.Checked == false && chkPerAng.Checked == false && chkRecalc.Checked == false && chkCC.Checked == false && chkSL.Checked == false))
                {
                    if (param == 1)
                        MessageBox.Show(Dami.TraduCuvant("Lipsesc date !"), MessageBox.icoError, "");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date !");
                    return;
                }

                if (Convert.ToInt32(txtMarcaInc.Value) > Convert.ToInt32(txtMarcaSf.Value))
                {
                    if (param == 1)
                        MessageBox.Show(Dami.TraduCuvant("Marca inceput este mai mare decat marca sfarsit !"), MessageBox.icoError, "");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Marca inceput este mai mare decat marca sfarsit !");
                    return;
                }

                if (Convert.ToDateTime(txtDataInc.Value) > Convert.ToDateTime(txtDataSf.Value))
                {
                    if (param == 1)
                        MessageBox.Show(Dami.TraduCuvant("Data inceput este mai mare decat data sfarsit !"), MessageBox.icoError, "");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data inceput este mai mare decat data sfarsit !");
                    return;
                }

                if (ActualizeazaDateGenerale(Convert.ToInt32(Session["UserId"]), Convert.ToInt32(txtMarcaInc.Value), Convert.ToInt32(txtMarcaSf.Value),
                    Convert.ToDateTime(txtDataInc.Value), Convert.ToDateTime(txtDataSf.Value), Convert.ToBoolean(chkCtr.Checked),
                    Convert.ToBoolean(chkNrm.Checked), Convert.ToBoolean(chkStr.Checked), Convert.ToBoolean(chkPerAng.Checked), Convert.ToBoolean(chkRecalc.Checked), Convert.ToBoolean(chkCC.Checked), Convert.ToBoolean(chkSL.Checked)))
                {
                    if (param == 1)
                        MessageBox.Show(Dami.TraduCuvant("Proces finalizat cu succes !"), MessageBox.icoSuccess, "");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces finalizat cu succes !");
                }
                else
                {
                    if (param == 1)
                        MessageBox.Show(Dami.TraduCuvant("Eroare in proces!"), MessageBox.icoError, "");
                    else
                        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Eroare in proces!");
                }

            }
            catch (Exception ex)
            {
                if (param == 1)
                    MessageBox.Show(ex.ToString(), MessageBox.icoError, "Atentie !");
                else
                    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(ex.ToString());
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }
}