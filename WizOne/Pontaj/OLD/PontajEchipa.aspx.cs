using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Pontaj
{
    public partial class PontajEchipa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string cmp = "";
            for(int i =1; i<=60; i++)
            {
                cmp += "C.F" + i + ",";
            }
            DataTable dt = General.IncarcaDT($@"SELECT X.F10003, X.Ziua, X.ValStr, X.Norma, {cmp}
                                            A.F10008 + ' ' + A.F10009 AS NumeComplet, B.Denumire AS DescContract,
                                            F.F00305 AS Subcompanie, G.F00406 AS Filiala, H.F00507 AS Sectie, I.F00608 AS Departament,
                                            ISNULL(C.IdStare, 1) AS IdStare, ISNULL(D.Culoare, '#FFFFFFFF') AS Culoare
                                            FROM Ptj_Intrari X
                                            LEFT JOIN F100 A ON A.F10003 = X.F10003
                                            LEFT JOIN Ptj_Contracte B on B.Id = X.IdContract
                                            LEFT JOIN Ptj_Cumulat C ON C.F10003 = X.F10003 AND C.An = 2017 AND C.Luna = 7
                                            LEFT JOIN Ptj_tblStariPontaj D ON D.Id = ISNULL(C.IdStare, 1)
                                            LEFT JOIN F003 F ON X.F10004 = F.F00304
                                            LEFT JOIN F004 G ON X.F10005 = G.F00405
                                            LEFT JOIN F005 H ON X.F10006 = H.F00506
                                            LEFT JOIN F006 I ON X.F10007 = I.F00607
                                            WHERE YEAR(X.Ziua) = 2017 AND MONTH(X.Ziua) = 7", null);
            grDate.DataSource = dt;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Session["PrintDocument"] = "PontajDetaliat";
                //Session["PrintParametrii"] = cmbAng.Value + ";" + Convert.ToDateTime(txtAnLuna.Value).Year + ";" + Convert.ToDateTime(txtAnLuna.Value).Month;
                Response.Redirect("~/Reports/Imprima.aspx", true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected void btnPeAng_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnPeZi_Click(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public void Iesire()
        {
            try
            {
                Response.Redirect("~/Absente/Lista.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        public IEnumerable<metaAfijeazaPontaj> PontajAfiseaza(int idUser, int an, int luna, int alMeu, int F10003, int idRol, int idDept, int idAngajat, int idSubcompanie, int idFiliala, int idSectie, int idStare, int idContract, int idSubdept, int idBirou)
        {
            try
            {
                if (Constante.tipBD == 1)
                {

                    string zile = "";
                    string ZileAs = "";

                    string ziInc = an.ToString() + "-" + luna.ToString().PadLeft(2, '0') + "-01";
                    string ziSf = srvGeneral.ToDataUniv(an, luna, 99);

                    string strSql = "";
                    string ziCuloare = "";
                    string ziUnion = "";

                    //2015-12-11   s-a modificat din X. in Y. 
                    string strFiltru = "";
                    if (idContract != -99) strFiltru += " AND Y.IdContract = " + idContract.ToString();
                    if (idSubcompanie != -99) strFiltru += " AND Y.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND Y.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND Y.F10006 = " + idSectie.ToString();
                    if (idDept != -99) strFiltru += " AND Y.F10007 = " + idDept.ToString();
                    if (idStare != -99) strFiltru += " AND COALESCE(J.IdStare,1) = " + idStare.ToString();
                    if (idSubdept != -99) strFiltru += " AND B.F100958 = " + idSubdept.ToString();
                    if (idBirou != -99) strFiltru += " AND B.F100959 = " + idBirou.ToString();
                    if (idAngajat == -99)
                        strFiltru += GetF10003Roluri(idUser, an, luna, alMeu, F10003, idRol, 0, -99, idAngajat).Replace("AND A.F10003 IN", "AND X.F10003 IN");
                    else
                        strFiltru += " AND X.F10003=" + idAngajat;


                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                    {
                        string strZi = "[" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "]";
                        zile += ", " + strZi;
                        ZileAs += ", " + strZi + " AS Val" + i.ToString();
                        ziCuloare += "+ ISNULL(" + strZi + ",'#00FFFFFF') + ';'";
                        ziUnion += "union select '" + an + "-" + luna.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + i.ToString().PadLeft(2, Convert.ToChar("0")) + "' as ziua ";
                    }

                    if (ziCuloare.Length > 1) ziCuloare = ziCuloare.Substring(1);
                    if (ziUnion.Length > 6) ziUnion = ziUnion.Substring(6);

                    string selectCuloare = " SELECT (CASE WHEN (W.CuloareValoare IS NULL OR W.CuloareValoare = '') THEN '#00FFFFFF' ELSE W.CuloareValoare END) + ';' from ( " +
                            " SELECT M.Ziua, N.Linia,  " +
                            " CASE WHEN (datepart(dw,M.Ziua)=1 OR datepart(dw,M.Ziua)=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.Ziua)<>0) and (isnull(N.CuloareValoare,'#00FFFFFF') = '#00FFFFFF' or isnull(N.CuloareValoare,'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE isnull(N.CuloareValoare,'#00FFFFFF') END AS CuloareValoare " +
                            " FROM (" + ziUnion + ") M " +
                            " LEFT JOIN Ptj_Intrari N ON M.Ziua=N.Ziua AND N.F10003=pvt.F10003 AND N.Linia = pvt.Linia AND '" + ziInc + "' <= N.Ziua and N.Ziua <= " + ziSf + " " +
                            " ) W  " +
                            " FOR XML PATH('')";



                    strSql = "SELECT " + nrAng + " as NrAng, COALESCE(" + idRol + ",1) AS IdRol, " + luna + " AS Luna, " + an + " AS An, " +
                            " CAST((pvt.F10003 * CAST(10000 AS DECIMAL)) + (CASE WHEN COALESCE(F06204,-1) = -1 THEN 0 ELSE F06204 END) AS DECIMAL) AS IdAuto, " +
                            " CAST ((CASE WHEN COALESCE(F06204,-1) =-1 THEN 0 ELSE (pvt.F10003 * CAST(10000 AS DECIMAL)) END) AS DECIMAL) AS Parinte,  " +
                            " F100901, pvt.F10003, F06204, ZiuaInc, pvt.Linia, Norma, DataInceput, DataSfarsit, NumeComplet, CCDefault, IdContract, DescContract, OreSup, Afisare, F10002, F10004, F10005, F10006, F10007, F100958, F100959, USER_NO, IdStare, Culoare, " +
                            " Angajator, TipContract, EID, CategAngajat, " +
                            " (" + selectCuloare + ") AS ZiCuloare, " +
                            " CAST(AvansNorma AS int) AS AvansNorma, " +
                            " CASE WHEN Norma <> AvansNorma THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND YEAR(F70406)=" + an + " AND MONTH(F70406)=" + luna + ") ELSE '2100-01-01' END AS AvansData, " +
                            " G31, G32, G33, G34, G35, G36, G37, G38, G39, G40 " + ZileAs +
                            " FROM " +
                            " ( " +
                            " SELECT A.F100901, X.F10003, X.ValStr, X.Ziua, ISNULL(X.Linia,0) AS Linia, ISNULL(X.F06204,-1) AS F06204, " +
                            " CONVERT(datetime,'" + ziInc + "') AS ZiuaInc, " +
                            " A.F10022 AS DataInceput, ddp.rez AS DataSfarsit,  A.F10008 + ' ' + A.F10009 AS NumeComplet, C.Id AS IdContract, " +
                            " Y.Norma, Y.F06204Default AS CCDefault, Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, " +
                            " C.Denumire AS DescContract, ISNULL(C.OreSup,0) AS OreSup, ISNULL(C.Afisare,1) AS Afisare, " +
                            " B.F100958, B.F100959, " + idUser + " AS USER_NO, ISNULL(J.IdStare,1) AS IdStare,  " +
                            " J.F31 AS G31,J.F32 AS G32,J.F33 AS G33,J.F34 AS G34,J.F35 AS G35,J.F36 AS G36,J.F37 AS G37,J.F38 AS G38,J.F39 AS G39,J.F40 AS G40, " +
                            " ISNULL(K.Culoare,'#FFFFFFFF') AS Culoare, " +
                            " A.F10078 AS Angajator, DR.F08903 AS TipContract, US.F70104 AS EID, CA.F72404 AS CategAngajat, " +
                            " dn.rez AS AvansNorma " +
                            " FROM Ptj_Intrari X " +
                            " LEFT JOIN F100 A ON A.F10003=X.F10003 " +
                            " LEFT JOIN F1001 B ON A.F10003=B.F10003 " +
                            " LEFT JOIN (SELECT R.F10003, R.Linia, MIN(R.Ziua) AS ZiuaMin FROM Ptj_Intrari R WHERE YEAR(R.Ziua)=" + an + " AND MONTH(R.Ziua)= " + luna + " GROUP BY R.F10003, R.Linia) Q ON Q.F10003=A.F10003 AND Q.Linia=X.Linia " +
                            " LEFT JOIN Ptj_Intrari Y ON A.F10003=Y.F10003 AND Y.Ziua=Q.ZiuaMin AND Y.Linia=Q.Linia " +
                            " LEFT JOIN Ptj_Cumulat J ON J.F10003=A.F10003 AND J.An=" + an + " AND J.Luna=" + luna +
                            " LEFT JOIN Ptj_tblStariPontaj K ON K.Id = ISNULL(J.IdStare,1) " +
                            " LEFT JOIN Ptj_Contracte C on C.Id = Y.IdContract " +
                            " LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 " +
                            //" LEFT JOIN USERS US ON A.F10003 = US.F70102 " +
                            " LEFT JOIN USERS US ON A.F10003 = US.F10003 " +        //Radu 13.03.2017
                            " LEFT JOIN F724 CA ON A.F10061 = CA.F72402 " +
                            " OUTER APPLY dbo.DamiNorma(X.F10003, " + ziSf + ") dn " +
                            " OUTER APPLY dbo.DamiDataPlecare(X.F10003, " + ziSf + ") ddp " +
                            " WHERE YEAR(X.Ziua)=" + an + " AND MONTH(X.Ziua)= " + luna + strFiltru +
                            " ) AS source " +
                            " PIVOT " +
                            " ( " +
                            "     MAX(ValStr) " +
                            "     FOR Ziua IN ( " + zile.Substring(2) + ") " +
                            " ) AS pvt " +
                            " ORDER BY NumeComplet, F06204";

                    this.ObjectContext.CommandTimeout = 600;

                    IEnumerable<metaAfijeazaPontaj> ptj = this.ObjectContext.ExecuteStoreQuery<metaAfijeazaPontaj>(strSql).ToList();

                    return ptj;
                }
                else
                {
                    string zile = "";
                    string ZileAs = "";

                    string ziInc = srvGeneral.ToDataUniv(an, luna, 1);
                    string ziSf = srvGeneral.ToDataUniv(an, luna, 99);

                    string strSql = "";
                    string ziCuloare = "";
                    string ziUnion = "";

                    string strFiltru = "";
                    if (an != -99) strFiltru += " AND to_char(X.\"Ziua\",'yyyy') = " + an.ToString();
                    if (luna != -99) strFiltru += " AND to_char(X.\"Ziua\",'mm') = " + luna.ToString();
                    if (idContract != -99) strFiltru += @" AND (SELECT ""IdContract"" FROM ""F100Contracte"" B WHERE A.F10003 = B.F10003 AND TRUNC(B.""DataInceput"") <= " + ziSf + " AND " + ziInc + " <= TRUNC(B.\"DataSfarsit\")) = " + idContract.ToString();

                    if (idStare != -99) strFiltru += " AND COALESCE(J.\"IdStare\",1) = " + idStare.ToString();
                    if (idSubcompanie != -99) strFiltru += " AND A.F10004 = " + idSubcompanie.ToString();
                    if (idFiliala != -99) strFiltru += " AND A.F10005 = " + idFiliala.ToString();
                    if (idSectie != -99) strFiltru += " AND A.F10006 = " + idSectie.ToString();
                    if (idDept != -99) strFiltru += " AND A.F10007 = " + idDept.ToString();
                    if (idSubdept != -99) strFiltru += " AND B.F100958 = " + idSubdept.ToString();
                    if (idBirou != -99) strFiltru += " AND B.F100959 = " + idBirou.ToString();
                    if (idAngajat == -99)
                        strFiltru += GetF10003Roluri(idUser, an, luna, alMeu, F10003, idRol, 0, idDept, idAngajat);
                    else
                        strFiltru += " AND A.F10003=" + idAngajat;


                    for (int i = 1; i <= DateTime.DaysInMonth(an, luna); i++)
                    {
                        string dtOra = i.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + srvGeneral.NumeLuna(luna, 1, "EN") + "-" + an.ToString().Substring(2);
                        zile += ", '" + dtOra + "'";
                        ZileAs += ", '" + dtOra + "' AS \"Val" + i.ToString() + "\"";
                        ziCuloare += "|| COALESCE(\"Val" + i.ToString() + "\",'#00FFFFFF') || ';'";
                        ziUnion += "UNION SELECT " + srvGeneral.ToDataUniv(an, luna, i) + " AS \"Ziua\" FROM Dual ";
                    }


                    if (ziCuloare.Length > 1) ziCuloare = ziCuloare.Substring(2);
                    if (ziUnion.Length > 6) ziUnion = ziUnion.Substring(6);

                    //string selectCuloare = " SELECT F10003, \"Linia\", LISTAGG((CASE WHEN (W.\"CuloareValoare\" IS NULL OR W.\"CuloareValoare\" = '') THEN '#00FFFFFF' ELSE W.\"CuloareValoare\" END), ';')  " +
                    //        " WITHIN GROUP (ORDER BY \"Ziua\") AS \"CuloareValoare\"  " +
                    //        " from (  " +
                    //        " SELECT N.F10003, M.\"Ziua\", N.\"Linia\", " +
                    //        " CASE WHEN ((1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=6 OR (1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.\"Ziua\")<>0)  " +
                    //        " and (COALESCE(N.\"CuloareValoare\",'#00FFFFFF') = '#00FFFFFF' or COALESCE(N.\"CuloareValoare\",'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE COALESCE(N.\"CuloareValoare\",'#00FFFFFF') END AS \"CuloareValoare\" " +
                    //        " FROM (" + ziUnion + ") M  " +
                    //        " LEFT JOIN \"Ptj_Intrari\" N ON M.\"Ziua\"=N.\"Ziua\" AND " + ziInc + " <= N.\"Ziua\" and N.\"Ziua\" <= " + ziSf + "  " +
                    //        " ) W  GROUP BY F10003, \"Linia\" ";

                    //Radu 05.05.2016
                    string selectCuloare = " SELECT F10003, \"Linia\", LISTAGG((CASE WHEN (W.\"CuloareValoare\" IS NULL OR W.\"CuloareValoare\" = '') THEN '#00FFFFFF' ELSE W.\"CuloareValoare\" END), ';')  " +
                            " WITHIN GROUP (ORDER BY \"Ziua\") AS \"CuloareValoare\"  " +
                            " from (  " +
                            " SELECT F100.F10003, M.\"Ziua\", nvl(N.\"Linia\", 0) AS \"Linia\", " +
                            " CASE WHEN ((1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=6 OR (1 + TRUNC (M.\"Ziua\") - TRUNC (M.\"Ziua\", 'IW'))=7 OR (SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = M.\"Ziua\")<>0)  " +
                            " and (COALESCE(N.\"CuloareValoare\",'#00FFFFFF') = '#00FFFFFF' or COALESCE(N.\"CuloareValoare\",'#FFFFFFFF') = '#FFFFFFFF') THEN '#FF30EDCC' ELSE COALESCE(N.\"CuloareValoare\",'#00FFFFFF') END AS \"CuloareValoare\" " +
                            " FROM F100 LEFT JOIN (" + ziUnion + ") M  ON 1 = 1 " +
                            " LEFT JOIN \"Ptj_Intrari\" N ON M.\"Ziua\"=N.\"Ziua\" AND " + ziInc + " <= N.\"Ziua\" and N.\"Ziua\" <= " + ziSf + "  " +
                            " AND F100.F10003 = N.F10003) W  GROUP BY F10003, \"Linia\" ";

                    strSql = "SELECT " + nrAng + " as \"NrAng\", COALESCE(" + idRol + ",1) AS \"IdRol\", " + luna + " AS \"Luna\", " + an + " AS \"An\", " +
                            " CAST((pvt.F10003 * CAST(10000 AS DECIMAL)) + (CASE WHEN COALESCE(pvt.F06204,-1) = -1 THEN 0 ELSE F06204 END) AS NUMBER(22)) AS IdAuto, " +
                            " CAST ((CASE WHEN COALESCE(pvt.F06204,-1) =-1 THEN 0 ELSE (pvt.F10003 * CAST(10000 AS DECIMAL)) END) AS NUMBER(22)) AS Parinte,  " +
                            " CASE WHEN \"Norma\" <> \"AvansNorma\" THEN (SELECT MAX(F70406) FROM F704 WHERE F70403=pvt.F10003 AND F70404=6 AND to_number(to_char(F70406,'YYYY'))=" + an + " AND to_number(to_char(F70406,'MM'))=" + luna + ") ELSE to_date('01-JAN-2100','DD-MON-YYYY') END AS AvansData, " +
                            " pvt.* " +
                            " FROM " +
                            " ( " +
                            " SELECT A.F100901, X.F10003, X.\"ValStr\", X.\"Ziua\", coalesce(X.\"Linia\",0) AS \"Linia\", coalesce(X.F06204,-1) AS F06204, " +
                            " " + ziInc + " AS \"ZiuaInc\", " +
                            " A.F10022 AS \"DataInceput\",  TRUNC(\"DamiDataPlecare\"(X.F10003, " + ziSf + ")) AS \"DataSfarsit\",  A.F10008 || ' ' || A.F10009 AS \"NumeComplet\", C.\"Id\" AS \"IdContract\", " +
                            " Y.\"Norma\", Y.\"F06204Default\" AS \"CCDefault\", Y.F10002, Y.F10004, Y.F10005, Y.F10006, Y.F10007, " +
                            " C.\"Denumire\" AS \"DescContract\", COALESCE(C.\"OreSup\",0) AS \"OreSup\", COALESCE(C.\"Afisare\",1) AS \"Afisare\", " +
                            " B.F100958, B.F100959, " + idUser + " AS USER_NO, coalesce(J.\"IdStare\",1) AS \"IdStare\",  " +
                            " J.F31 AS G31,J.F32 AS G32,J.F33 AS G33,J.F34 AS G34,J.F35 AS G35,J.F36 AS G36,J.F37 AS G37,J.F38 AS G38,J.F39 AS G39,J.F40 AS G40, " +
                            " coalesce(K.\"Culoare\",'#FFFFFFFF') AS \"Culoare\", " +
                            " A.F10078 AS \"Angajator\", DR.F08903 AS \"TipContract\", US.F70104 AS EID, CA.F72404 AS \"CategAngajat\", " +
                            " \"DamiNorma\"(X.F10003, " + ziSf + ") AS \"AvansNorma\", V.\"CuloareValoare\" as ZiCuloare " +
                            " FROM \"Ptj_Intrari\" X " +
                            " LEFT JOIN F100 A ON A.F10003=X.F10003 " +
                            " LEFT JOIN F1001 B ON A.F10003=B.F10003 " +
                            " LEFT JOIN (SELECT R.F10003, MIN(R.\"Ziua\") AS ZiuaMin FROM \"Ptj_Intrari\" R WHERE to_number(to_char(R.\"Ziua\",'YYYY'))=" + an + " AND to_number(to_char(R.\"Ziua\",'MM'))= " + luna + " GROUP BY R.F10003) Q ON Q.F10003=A.F10003 " +
                            " LEFT JOIN \"Ptj_Intrari\" Y ON A.F10003=Y.F10003 AND Y.\"Ziua\"=Q.ZiuaMin " +
                            " LEFT JOIN \"Ptj_Cumulat\" J ON J.F10003=A.F10003 AND J.\"An\"=" + an + " AND J.\"Luna\"=" + luna +
                            " LEFT JOIN \"Ptj_tblStariPontaj\" K ON K.\"Id\" = coalesce(J.\"IdStare\",1) " +
                            " LEFT JOIN \"Ptj_Contracte\" C on C.\"Id\" = Y.\"IdContract\" " +
                            " LEFT JOIN F089 DR ON DR.F08902 = A.F1009741 " +
                            " LEFT JOIN USERS US ON A.F10003 = US.F70102 " +
                            " LEFT JOIN F724 CA ON A.F10061 = CA.F72402 " +
                            " LEFT JOIN (" + selectCuloare + ") V ON V.F10003=X.F10003 AND V.\"Linia\" = X.\"Linia\"  " +
                            " WHERE to_number(to_char(X.\"Ziua\",'YYYY'))=" + an + " AND to_number(to_char(X.\"Ziua\",'MM'))= " + luna + strFiltru +
                            " ) " +
                            " PIVOT " +
                            " ( " +
                            "     MAX(\"ValStr\") " +
                            "     FOR \"Ziua\" IN ( " + ZileAs.Substring(2) + ") " +
                            " ) pvt " +
                            " ORDER BY \"NumeComplet\", F06204";


                    this.ObjectContext.CommandTimeout = 600;

                    IEnumerable<metaAfijeazaPontaj> ptj = this.ObjectContext.ExecuteStoreQuery<metaAfijeazaPontaj>(strSql).ToList();

                    return ptj;
                }
            }
            catch (Exception ex)
            {
                srvGeneral.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
                return null;
            }
        }





    }
}