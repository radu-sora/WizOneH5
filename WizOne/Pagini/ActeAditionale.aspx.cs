using DevExpress.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Avs;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class ActeAditionale : Page
    {        
        private void IncarcaCmbAng(int? cmp, int? tip)
        {
            try
            {
                var strSql = string.Empty;

                if (cmp == -1 && tip == -1)
                {
                    strSql = $@"SELECT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                            FROM (
                            SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                            FROM ""Avs_Cereri"" A
                            INNER JOIN F100 B ON A.F10003=B.F10003
                            WHERE ""IdStare""=3
                            UNION
                            SELECT A.F10003, COALESCE(A.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(A.F10009,'') AS ""NumeComplet"" 
                            FROM F100 A
                            WHERE A.F10025=900) X
                            LEFT JOIN F100 A ON X.F10003=A.F10003
                            LEFT JOIN F004 G ON A.F10005 = G.F00405
                            LEFT JOIN F005 H ON A.F10006 = H.F00506
                            LEFT JOIN F006 I ON A.F10007 = I.F00607";
                }
                else
                {
                    var sursa = string.Empty;
                    var companie = string.Empty;

                    if (cmp != null) companie = " AND B.F10002=" + cmp;

                    switch (tip ?? -1)
                    {
                        case 0:
                            sursa = $@" SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                                    FROM ""Avs_Cereri"" A
                                    INNER JOIN F100 B ON A.F10003=B.F10003
                                    WHERE ""IdStare""=3 {companie}";
                            break;
                        case 1:
                            sursa = $@"SELECT B.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""                             
                                   FROM F100 B
                                   WHERE B.F10025=900 {companie}";
                            break;
                        default:
                            sursa = $@"SELECT DISTINCT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                                   FROM ""Avs_Cereri"" A
                                   INNER JOIN F100 B ON A.F10003=B.F10003
                                   WHERE ""IdStare""=3 {companie}
                                   UNION
                                   SELECT B.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet"" 
                                   FROM F100 B
                                   WHERE B.F10025=900 {companie}";
                            break;
                    }

                    strSql = $@"SELECT DISTINCT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""                        
                            FROM ({sursa}) X
                            LEFT JOIN F100 A ON X.F10003=A.F10003
                            LEFT JOIN F004 G ON A.F10005 = G.F00405
                            LEFT JOIN F005 H ON A.F10006 = H.F00506
                            LEFT JOIN F006 I ON A.F10007 = I.F00607";
                }

                cmbAng.DataSource = General.IncarcaDT(strSql, null);
                cmbAng.DataBind();
                cmbAng.SelectedIndex = -1;
                Session["Acte_Ang"] = cmbAng.DataSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void IncarcaGrid()
        {
            DataTable dt = new DataTable();

            try
            {
                string companie = "";
                string strSql = "";
                string filtru = "";
                var filter = JObject.Parse(Session["Filtru_ActeAditionale"] as string) as dynamic;

                if ((filter as JObject).HasValues)
                {
                    switch ((int?)filter.tip ?? 9)
                    {
                        case 0:
                            filtru = " AND \"Candidat\"= 0";
                            break;
                        case 1:
                            filtru = " AND \"Candidat\"= 1 AND \"CandidatAngajat\"= 0";
                            break;
                        case 2:
                            filtru = " AND \"Candidat\"= 1 AND \"CandidatAngajat\"= 1";
                            break;
                        case 9:
                            //NOP
                            break;
                    }

                    if (filter.ang != null) filtru += @" AND ""F10003""= " + (int)filter.ang;
                    if (filter.data != null) filtru += " AND \"DataModif\" = " + General.ToDataUniv((DateTime)filter.data);
                    if (filter.depasire != null) filtru += " AND \"TermenDepasire\" = " + General.ToDataUniv((DateTime)filter.depasire);

                    if (filter.cmp != null) companie = " AND B.F10002 = " + (int)filter.cmp;

                    switch ((int?)filter.status ?? 0)
                    {
                        case 1:         //Numar atribuit
                            filtru += @" AND COALESCE(""DocNr"",0) <> 0";
                            break;
                        case 2:         //Tiparit
                            filtru += @" AND COALESCE(""Tiparit"",0) <> 0";
                            break;
                        case 3:         //Semnat
                            filtru += @" AND COALESCE(""Semnat"",0) <> 0";
                            break;
                        case 4:         //Revisal
                            filtru += @" AND COALESCE(""Revisal"",0) <> 0";
                            break;
                    }
                }

                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2) cmp = " CAST(ROWNUM AS INT) ";

                string filtruSup = "";
                string idExcluseCircuitDoc = General.Nz(General.ExecutaScalar($@"SELECT Valoare FROM ""tblParametrii"" WHERE ""Nume""= 'IdExcluseCircuitDoc'", null), "").ToString();
                if (idExcluseCircuitDoc != "")
                    filtruSup = $@" AND A.""IdAtribut"" NOT IN ({idExcluseCircuitDoc})";

                if (Constante.tipBD == 1)
                {
                    #region SQL

                    strSql = $@"
                            SELECT * FROM (
                            SELECT {cmp} AS ""Cheie"", X.*,
                            (SELECT MIN(""ColData"") FROM (
                            SELECT CASE WHEN ""Candidat"" = 1 THEN 
                            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,F10022) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                            ELSE '2100-01-01' END AS ColData 
                            UNION
                            SELECT CASE WHEN Motiv = 1 THEN X.DataModif ELSE '2100-01-01' END AS ColData  
                            UNION
                            SELECT CASE WHEN (Salariul = 1 OR Spor = 1 OR SporVechime = 1) THEN 
                            (SELECT Zi FROM (
                            SELECT Zi, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as IdAuto 
                            FROM tblZile WHERE Zi>=DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays)) x
                            WHERE IdAuto=20)
                            ELSE '2100-01-01' END AS ColData 
                            UNION
                            SELECT CASE WHEN CORCod=1 OR FunctieId = 1 OR CIMDet=1 OR CIMNed=1 THEN 
                            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                            ELSE '2100-01-01' END AS ColData 
                            UNION
                            SELECT CASE WHEN ""Norma"" = 1 OR ""Suspendare""=1 OR ""SuspendareRev"" =1 THEN 
                            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,X.DataModif) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                            ELSE '2100-01-01' END AS ColData 
                            ) x) AS TermenDepasire
                            FROM (
                            SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                            MAX(CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END) AS CORCod,
                            MAX(CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END) AS FunctieId,
                            MAX(CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END) AS Norma,
                            MAX(CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END) AS Salariul,
                            MAX(CASE WHEN (COALESCE(Spor0,0) + COALESCE(Spor1,0) +COALESCE(Spor2,0) +COALESCE(Spor3,0) +COALESCE(Spor4,0) +COALESCE(Spor5,0) +COALESCE(Spor6,0) +COALESCE(Spor7,0) +COALESCE(Spor8,0) +COALESCE(Spor9,0) +COALESCE(Spor10,0) +COALESCE(Spor11,0) +COALESCE(Spor12,0) +COALESCE(Spor13,0) +COALESCE(Spor14,0) +COALESCE(Spor15,0) +COALESCE(Spor16,0) +COALESCE(Spor17,0) +COALESCE(Spor18,0) +COALESCE(Spor19,0)) > 0 THEN 1 ELSE 0 END ) AS Spor,
                            MAX(CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END) AS Structura,
                            MAX(CASE WHEN COALESCE(DurataContract, 0) = 2 THEN 1 ELSE 0 END) AS CIMDet,
                            MAX(CASE WHEN COALESCE(DurataContract, 0) = 1 THEN 1 ELSE 0 END) AS CIMNed,
                            MAX(CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END) AS Motiv,
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 30 THEN 1 ELSE 0 END) AS ""Suspendare"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 31 THEN 1 ELSE 0 END) AS ""SuspendareRev"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 32 THEN 1 ELSE 0 END) AS ""Detasare"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 33 THEN 1 ELSE 0 END) AS ""DetasareRev"",
                            CONVERT(nvarchar(10),J.DocNr) AS DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct, 
                            CASE WHEN (SELECT COUNT(*) FROM Atasamente FIS WHERE FIS.IdAuto=J.IdAutoAtasamente) = 0 THEN 0 ELSE 1 END AS AreAtas,
                            (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                            FROM Avs_Cereri AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                            WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,'1900-01-01')=COALESCE(J.DocData,'1900-01-01')
                            AND COALESCE((SELECT CHARINDEX(',' + CAST(AA.IdAtribut AS nvarchar(20)) + ',', ',' + Valoare + ',') FROM tblParametrii WHERE Nume='IdExcluseCircuitDoc'),0) = 0                            
                            GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                            FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993, J.IdAutoAtasamente,
                            0 AS CandidatAngajat, 
                            COALESCE((SELECT MAX(CASE WHEN B0.F02504 IS NOT NULL OR B1.F02504 IS NOT NULL OR B2.F02504 IS NOT NULL OR B3.F02504 IS NOT NULL OR B4.F02504 IS NOT NULL OR 
                            B5.F02504 IS NOT NULL OR B6.F02504 IS NOT NULL OR B7.F02504 IS NOT NULL OR B8.F02504 IS NOT NULL OR B9.F02504 IS NOT NULL THEN 1 ELSE 0 END) AS SporVechime
                            FROM F704 FA
                            LEFT JOIN F025 B0 ON FA.F704660 = B0.F02504 AND B0.F02504 > 10 AND B0.F02520 IN ('F100643','F100644') AND B0.F02521 > 0
                            LEFT JOIN F025 B1 ON FA.F704661 = B1.F02504 AND B1.F02504 > 10 AND B1.F02520 IN ('F100643','F100644') AND B1.F02521 > 0
                            LEFT JOIN F025 B2 ON FA.F704662 = B2.F02504 AND B2.F02504 > 10 AND B2.F02520 IN ('F100643','F100644') AND B2.F02521 > 0
                            LEFT JOIN F025 B3 ON FA.F704663 = B3.F02504 AND B3.F02504 > 10 AND B3.F02520 IN ('F100643','F100644') AND B3.F02521 > 0
                            LEFT JOIN F025 B4 ON FA.F704664 = B4.F02504 AND B4.F02504 > 10 AND B4.F02520 IN ('F100643','F100644') AND B4.F02521 > 0
                            LEFT JOIN F025 B5 ON FA.F704665 = B5.F02504 AND B5.F02504 > 10 AND B5.F02520 IN ('F100643','F100644') AND B5.F02521 > 0
                            LEFT JOIN F025 B6 ON FA.F704666 = B6.F02504 AND B6.F02504 > 10 AND B6.F02520 IN ('F100643','F100644') AND B6.F02521 > 0
                            LEFT JOIN F025 B7 ON FA.F704667 = B7.F02504 AND B7.F02504 > 10 AND B7.F02520 IN ('F100643','F100644') AND B7.F02521 > 0
                            LEFT JOIN F025 B8 ON FA.F704668 = B8.F02504 AND B8.F02504 > 10 AND B8.F02520 IN ('F100643','F100644') AND B8.F02521 > 0
                            LEFT JOIN F025 B9 ON FA.F704669 = B9.F02504 AND B9.F02504 > 10 AND B9.F02520 IN ('F100643','F100644') AND B9.F02521 > 0
                            WHERE FA.F70404=11 AND FA.F70410='Automat - grila' AND FA.F70403=A.F10003
							AND CAST((SELECT CONVERT(nvarchar(10),F01011) + '-' + CONVERT(nvarchar(10),F01012) + '-01' FROM F010) AS DATE) <= FA.F70406
							AND FA.F70406 < DATEADD(m,3,CAST((SELECT CONVERT(nvarchar(10),F01011) + '-' + CONVERT(nvarchar(10),F01012) + '-01' FROM F010) AS DATE))
							),0) AS SporVechime
                            FROM Avs_Cereri A
                            INNER JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                            WHERE A.IdStare = 3 AND A.DataModif >= '2019-01-01' {companie} {filtruSup}
                            GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993, J.Candidat, J.IdAutoAtasamente, B.F10025
                            UNION
                            SELECT B.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, B.F10022, 1 AS Candidat,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            CONVERT(nvarchar(10),B.F100985) AS DocNr, B.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct,
                            CASE WHEN (SELECT COUNT(*) FROM Atasamente FIS WHERE FIS.IdAuto=J.IdAutoAtasamente) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                            B.F10022, B.F100993, J.IdAutoAtasamente,
                            CASE WHEN (COALESCE(B.F10025,-99) <> 900 AND COALESCE(J.Semnat,0) = 1) THEN 1 ELSE 0 END AS CandidatAngajat, 0 AS SporVechime
                            FROM F100 B
                            LEFT JOIN Admin_NrActAd J ON B.F10003=J.F10003
                            WHERE (B.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) {companie}) X
                            ) AS Y
                            WHERE 1=1 " + filtru;

                    #endregion
                }
                else
                {
                    #region Orcl

                    strSql = $@"
                            SELECT * FROM (
                            SELECT {cmp} AS ""Cheie"", X.*,
                            CASE WHEN ""Candidat"" = 1 then 
                            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= (F10022 - 1) AND ""ZiSapt"" <= 5)
                            when ""Motiv"" = 1 then X.""DataModif""
                            WHEN ""Norma""=1 OR ""Suspendare""=1 OR ""SuspendareRev"" =1 THEN
                            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= (X.""DataModif"" - 1) AND ""ZiSapt"" <= 5)
                            when ""Salariul"" = 1 OR ""Spor"" = 1 OR ""SporVechime"" = 1 then
                            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= x.""DataModif"" + 19 AND ""ZiSapt"" <= 5)
                            WHEN ""CORCod"" = 1 OR ""FunctieId"" = 1 OR ""CIMDet"" = 1 OR ""CIMNed"" = 1 THEN
                            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= x.""DataModif"" - 1 AND ""ZiSapt"" <= 5)
                            ELSE TO_DATE('01-01-2100', 'DD-MM-YYYY') END AS ""TermenDepasire""
                            FROM(
                            SELECT A.F10003, COALESCE(B.F10008, '') || ' ' || COALESCE(B.F10009, '') AS ""NumeComplet"", A.""DataModif"", 0 AS ""Candidat"",
                            MAX(CASE WHEN COALESCE(""CORCod"", 0) > 0 THEN 1 ELSE 0 END) AS ""CORCod"",
                            MAX(CASE WHEN COALESCE(""FunctieId"", 0) > 0 THEN 1 ELSE 0 END) AS ""FunctieId"",
                            MAX(CASE WHEN COALESCE(""Norma"", 0) > 0 THEN 1 ELSE 0 END) AS ""Norma"",
                            MAX(CASE WHEN COALESCE(""SalariulBrut"", 0) > 0 OR COALESCE(""SalariulNet"", 0) > 0 THEN 1 ELSE 0 END) AS ""Salariul"",
                            MAX(CASE WHEN (COALESCE(""Spor0"",0) + COALESCE(""Spor1"",0) +COALESCE(""Spor2"",0) +COALESCE(""Spor3"",0) +COALESCE(""Spor4"",0) +COALESCE(""Spor5"",0) +COALESCE(""Spor6"",0) +COALESCE(""Spor7"",0) +COALESCE(""Spor8"",0) +COALESCE(""Spor9"",0) +COALESCE(""Spor10"",0) +COALESCE(""Spor11"",0) +COALESCE(""Spor12"",0) +COALESCE(""Spor13"",0) +COALESCE(""Spor14"",0) +COALESCE(""Spor15"",0) +COALESCE(""Spor16"",0) +COALESCE(""Spor17"",0) +COALESCE(""Spor18"",0) +COALESCE(""Spor19"",0)) > 0 THEN 1 ELSE 0 END ) AS ""Spor"",
                            MAX(CASE WHEN COALESCE(""SubcompanieId"", 0) > 0 OR COALESCE(""FilialaId"", 0) > 0 OR COALESCE(""SectieId"", 0) > 0 OR COALESCE(""DeptId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Structura"",
                            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 2 THEN 1 ELSE 0 END) AS ""CIMDet"",
                            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 1 THEN 1 ELSE 0 END) AS ""CIMNed"",
                            MAX(CASE WHEN COALESCE(""MotivId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Motiv"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 30 THEN 1 ELSE 0 END) AS ""Suspendare"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 31 THEN 1 ELSE 0 END) AS ""SuspendareRev"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 32 THEN 1 ELSE 0 END) AS ""Detasare"",
							MAX(CASE WHEN COALESCE(""IdAtribut"", 0) = 33 THEN 1 ELSE 0 END) AS ""DetasareRev"",
                            CAST(J.""DocNr"" AS varchar2(20)) AS ""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""Atasamente"" FIS WHERE FIS.""IdAuto""=J.""IdAutoAtasamente"") = 0 THEN 0 ELSE 1 END AS ""AreAtas"",
                            (SELECT ',' || LISTAGG(AA.""Id"", ',') WITHIN GROUP (ORDER BY AA.""Id"") AS ""Id""
                            FROM ""Avs_Cereri"" AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN ""Admin_NrActAd"" JJ ON AA.""IdActAd""=JJ.""IdAuto""
                            WHERE AA.""IdStare"" = 3 AND AA.F10003=A.F10003 AND AA.""DataModif""=A.""DataModif"" AND COALESCE(JJ.""DocNr"",-99)=COALESCE(J.""DocNr"",-99) 
                            AND NVL(JJ.""DocData"",'01-01-2000') = NVL(J.""DocData"",'01-01-2000')
                            AND COALESCE((SELECT INSTR(',' || CAST(AA.""IdAtribut"" AS varchar2(20)) || ',', ',' || ""Valoare"" || ',') FROM ""tblParametrii"" WHERE ""Nume"" ='IdExcluseCircuitDoc'),0) = 0
                            ) AS ""IdAvans"", B.F10022, B.F100993, J.""IdAutoAtasamente"",
                            0 AS ""CandidatAngajat"", 
                            COALESCE((SELECT MAX(CASE WHEN B0.F02504 IS NOT NULL OR B1.F02504 IS NOT NULL OR B2.F02504 IS NOT NULL OR B3.F02504 IS NOT NULL OR B4.F02504 IS NOT NULL OR 
                            B5.F02504 IS NOT NULL OR B6.F02504 IS NOT NULL OR B7.F02504 IS NOT NULL OR B8.F02504 IS NOT NULL OR B9.F02504 IS NOT NULL THEN 1 ELSE 0 END) AS ""SporVechime""
                            FROM F704 FA
                            LEFT JOIN F025 B0 ON FA.F704660 = B0.F02504 AND B0.F02504 > 10 AND B0.F02520 IN ('F100643','F100644') AND B0.F02521 > 0
                            LEFT JOIN F025 B1 ON FA.F704661 = B1.F02504 AND B1.F02504 > 10 AND B1.F02520 IN ('F100643','F100644') AND B1.F02521 > 0
                            LEFT JOIN F025 B2 ON FA.F704662 = B2.F02504 AND B2.F02504 > 10 AND B2.F02520 IN ('F100643','F100644') AND B2.F02521 > 0
                            LEFT JOIN F025 B3 ON FA.F704663 = B3.F02504 AND B3.F02504 > 10 AND B3.F02520 IN ('F100643','F100644') AND B3.F02521 > 0
                            LEFT JOIN F025 B4 ON FA.F704664 = B4.F02504 AND B4.F02504 > 10 AND B4.F02520 IN ('F100643','F100644') AND B4.F02521 > 0
                            LEFT JOIN F025 B5 ON FA.F704665 = B5.F02504 AND B5.F02504 > 10 AND B5.F02520 IN ('F100643','F100644') AND B5.F02521 > 0
                            LEFT JOIN F025 B6 ON FA.F704666 = B6.F02504 AND B6.F02504 > 10 AND B6.F02520 IN ('F100643','F100644') AND B6.F02521 > 0
                            LEFT JOIN F025 B7 ON FA.F704667 = B7.F02504 AND B7.F02504 > 10 AND B7.F02520 IN ('F100643','F100644') AND B7.F02521 > 0
                            LEFT JOIN F025 B8 ON FA.F704668 = B8.F02504 AND B8.F02504 > 10 AND B8.F02520 IN ('F100643','F100644') AND B8.F02521 > 0
                            LEFT JOIN F025 B9 ON FA.F704669 = B9.F02504 AND B9.F02504 > 10 AND B9.F02520 IN ('F100643','F100644') AND B9.F02521 > 0
                            WHERE FA.F70404=11 AND FA.F70410='Automat - grila' AND FA.F70403=A.F10003
                            AND TO_DATE((SELECT '01-' || F01012 || '-' || F01011 FROM F010), 'DD-MM-YYYY') <= FA.F70406
                            AND FA.F70406 < ADD_MONTHS(TO_DATE((SELECT '01-' || F01012 || '-' || F01011 FROM F010), 'DD-MM-YYYY'),3)
							),0) AS ""SporVechime""
                            FROM ""Avs_Cereri"" A
                            INNER JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN ""Admin_NrActAd"" J ON A.""IdActAd""=J.""IdAuto""
                            WHERE A.""IdStare"" = 3 AND A.""DataModif"" >= TO_DATE('01-01-2019', 'DD-MM-YYYY') {companie}
                            GROUP BY A.F10003, B.F10008, B.F10009, A.""DataModif"", J.""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0), COALESCE(J.""Semnat"",0), COALESCE(J.""Revisal"",0), J.""IdAuto"", B.F10022, B.F100993, J.""Candidat"", J.""IdAutoAtasamente"", B.F10025
                            UNION
                            SELECT A.F10003, COALESCE(A.F10008, '') || ' ' || COALESCE(A.F10009, '') AS ""NumeComplet"", A.F10022, 1 AS ""Candidat"",
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                            A.F100985, A.F100986, COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"",
                            CASE WHEN (SELECT COUNT(*) FROM ""Atasamente"" FIS WHERE FIS.""IdAuto""=J.""IdAutoAtasamente"") = 0 THEN 0 ELSE 1 END AS ""AreAtas"", ',-1' AS ""IdAvans"",
                            A.F10022, A.F100993, J.""IdAutoAtasamente"",
                            CASE WHEN (COALESCE(B.F10025,-99) <> 900 AND COALESCE(J.""Semnat"",0) = 1) THEN 1 ELSE 0 END AS ""CandidatAngajat"", 0 AS ""SporVechime""
                            FROM F100 A
                            LEFT JOIN ""Admin_NrActAd"" J ON A.F10003=J.F10003
                            WHERE (A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) {companie}) X
                            ) 
                            WHERE 1=1 " + filtru;

                    #endregion
                }

                dt = General.IncarcaDT(strSql, null);
                dt.PrimaryKey = new DataColumn[] { dt.Columns["Cheie"] };

                Session["InformatiaCurenta"] = dt;

                grDate.KeyFieldName = "Cheie";
                grDate.DataSource = dt;
                grDate.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
            finally
            {
                dt.Dispose();
                dt = null;
            }
        }        

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();                

                if (!IsPostBack)
                {
                    if (Session["PaginaWeb"] as string != "Pagini.ActeAditionale")
                    {
                        Session["PaginaWeb"] = "Pagini.ActeAditionale";
                        Session["Filtru_ActeAditionale"] = "{}";
                    }

                    #region Traducere
                    string ctlPost = Request.Params["__EVENTTARGET"];
                    if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                    btnNr.Text = Dami.TraduCuvant("btnNr", "Atribuire numar");
                    btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                    btnPrint.Text = Dami.TraduCuvant("btnPrint", "Imprima");

                    lblTip.InnerText = Dami.TraduCuvant("Tip");
                    lblAng.InnerText = Dami.TraduCuvant("Angajat/Candidat");
                    lblStatus.InnerText = Dami.TraduCuvant("Status");
                    lblData.InnerText = Dami.TraduCuvant("Data modificarii");

                    btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");

                    foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                        col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                    #endregion

                    cmbCmp.DataSource = General.IncarcaDT("SELECT F00202, F00204 FROM F002", null);
                    cmbCmp.DataBind();
                    cmbCmp.SelectedIndex = -1;

                    if ((cmbCmp.DataSource as DataTable)?.Rows.Count > 1)
                        pnlComp.Visible = true;

                    IncarcaCmbAng(-1, -1);                    

                    var filter = JObject.Parse(Session["Filtru_ActeAditionale"] as string) as dynamic;

                    if ((filter as JObject).HasValues)
                    {
                        cmbCmp.Value = (int?)filter.cmp;
                        cmbTip.Value = (int?)filter.tip;
                        cmbAng.Value = (int?)filter.ang;
                        cmbStatus.Value = (int?)filter.status;
                        txtData.Value = (DateTime?)filter.data;
                        txtDepasire.Value = (DateTime?)filter.depasire;                        
                    }

                    IncarcaGrid();

                    if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);                    

                    //in cazul in care se sterge atasamentul din managemetul de personal
                    General.ExecutaNonQuery(@"UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=NULL WHERE ""IdAutoAtasamente"" NOT IN (SELECT ""IdAuto"" FROM ""Atasamente"")", null);
                }
                else if (cmbAng.IsCallback)
                {
                    cmbAng.DataSource = Session["Acte_Ang"];
                    cmbAng.DataBind();
                }
                else if (grDate.IsCallback)
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbAng_Callback(object sender, CallbackEventArgsBase e)
        {            
            try
            {
                if (e.Parameter.Length > 0)
                {                    
                    var param = JObject.Parse(e.Parameter) as dynamic;

                    IncarcaCmbAng((int?)param.cmp, (int?)param.tip);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        
        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                if (e.Parameters.Length > 0)
                {                    
                    var parameters = e.Parameters.Split(';');

                    if (parameters.Length != 2 || parameters[0].Length == 0)
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        return;
                    }

                    var msg = string.Empty;
                    var command = General.Nz(parameters[0], "").ToString();
                    var param = parameters[1];
                    var lst = new List<object>();

                    if (command == "btnNr" || command == "btnTiparit" || command == "btnSemnat" || command == "btnFinalizat")
                    {
                        lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "IdAvans", "Tiparit", "Semnat", "Revisal", "NumeComplet", "DocData", "Candidat", "TermenDepasire", "Motiv", "Suspendare", "SuspendareRev", "Detasare", "DetasareRev" });

                        if (lst?.FirstOrDefault() == null) return;
                    }
                    else if (command == "btnPrint")
                    {
                        lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "Motiv", "CIMDet", "CIMNed", "CORCod", "FunctieId", "Norma", "Salariul", "Spor", "Structura", "DocNr", "DocData", "DataModif", "Candidat", "IdAutoAct", "SporVechime", "Suspendare", "SuspendareRev", "Detasare", "DetasareRev" });

                        if (lst?.FirstOrDefault() == null) return;
                    }

                    switch (command)
                    {
                        case "btnDocUpload":
                            {
                                if (param == "")
                                    msg = "Fisierul nu poate fi atasat deoarece nu exista numar";
                                else
                                    msg = Dami.TraduCuvant("proces realizat cu succes");
                            }
                            break;
                        case "btnSterge":
                            {
                                General.ExecutaNonQuery($@"
                                    BEGIN
                                        DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                                        DELETE FROM ""Atasamente"" WHERE ""IdAuto""=@2;
                                        UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=NULL WHERE ""IdAutoAtasamente""=@2;
                                    END;", new object[] { "Atasamente", param });

                                msg = Dami.TraduCuvant("proces realizat cu succes");
                            }
                            break;
                        case "btnNr":
                            {
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (General.Nz(obj[2], "").ToString() != "")
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("are deja numar") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[5], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este tiparita") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[6], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este semnata") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[7], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este finalizata") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[12], 0)) != 0 || Convert.ToInt32(General.Nz(obj[13], 0)) != 0 || Convert.ToInt32(General.Nz(obj[14], 0)) != 0 || Convert.ToInt32(General.Nz(obj[15], 0)) != 0 || Convert.ToInt32(General.Nz(obj[16], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("atribuirea de numar se face doar prin editare") + System.Environment.NewLine;
                                            continue;
                                        }


                                        if (General.Nz(obj[2], "").ToString() == "")
                                        {
                                            if (General.Nz(obj[10], "0").ToString() == "1")
                                            {
                                                msg += "Acest candidat nu are setat numarul si/sau data contract intern in managementul de personal";
                                            }
                                            else
                                            {
                                                DataTable dt = new DataTable();
                                                int id = -99;

                                                if (Constante.tipBD == 1)
                                                {
                                                    dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    OUTPUT Inserted.IdAuto
                                                    VALUES(@1, 
                                                    COALESCE((SELECT MAX(COALESCE(A.""DocNr"",0)) FROM ""Admin_NrActAd"" A
                                                    LEFT JOIN ""Avs_Cereri"" B ON A.""IdAuto""=B.""IdActAd"" AND B.""IdAtribut"" NOT IN (4, 30, 31, 32, 33)
                                                    WHERE A.F10003=@1 AND COALESCE(A.""Candidat"",0)=0),0) + 1, 
                                                    {General.CurrentDate()},@2, @3, {General.CurrentDate()}, @4, @5);",
                                                    new object[] { obj[0], obj[1], Session["UserId"], obj[11], obj[10] });

                                                    if (dt.Rows.Count > 0)
                                                        id = Convert.ToInt32(General.Nz(dt.Rows[0][0], -99));
                                                }
                                                else
                                                {
                                                    id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    VALUES(@2, 
                                                    COALESCE((SELECT MAX(COALESCE(A.""DocNr"",0)) FROM ""Admin_NrActAd"" A
                                                    LEFT JOIN ""Avs_Cereri"" B ON A.""IdAuto""=B.""IdActAd"" AND B.""IdAtribut"" NOT IN (4, 30, 31, 32, 33)
                                                    WHERE A.F10003=@1 AND COALESCE(A.""Candidat"",0)=0),0) + 1, 
                                                    {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[1]))}, @3, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[11]))}, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                    new object[] { "int", obj[0], Session["UserId"], obj[10] }), 0));
                                                }

                                                if (Convert.ToInt32(General.Nz(id, -99)) != -99)
                                                    General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1" + obj[4] + ")", new object[] { id });

                                                msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;
                                                if (Convert.ToDateTime(General.Nz(obj[1], 0)) < DateTime.Now)
                                                    msg += Dami.TraduCuvant("Atentie, data modificare este mai mica decat data documentului") + System.Environment.NewLine;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        string ert = ex.Message;
                                    }
                                }                                
                            }
                            break;
                        case "btnTiparit":
                            {
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (General.Nz(obj[2], "").ToString() == "")
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu exista numar") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[5], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este tiparita") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[6], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este semnata") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[7], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este finalizata") + System.Environment.NewLine;
                                            continue;
                                        }

                                        //daca este candidat inainte trebuie sa salvam in baza de date
                                        if (General.Nz(obj[10],"0").ToString() == "1")
                                        {
                                            int id = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT ""IdAuto"" FROM ""Admin_NrActAd"" WHERE F10003=@1 AND COALESCE(""Candidat"",0)=1", new object[] { obj[0] }),-99));

                                            if (id == -99)
                                            {
                                                if (Constante.tipBD == 1)
                                                {
                                                    DataTable dt = General.IncarcaDT($@"
                                                    INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    OUTPUT Inserted.IdAuto
                                                    VALUES(@1, 0, @6, @2, @3, {General.CurrentDate()}, @4, @5);",
                                                    new object[] { obj[0], obj[1], Session["UserId"], obj[11], obj[10], obj[1] });

                                                    if (dt.Rows.Count > 0)
                                                        id = Convert.ToInt32(General.Nz(dt.Rows[0][0], -99));
                                                }
                                                else
                                                {
                                                    id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    VALUES(@2, 0, @6, {General.ToDataUniv(Convert.ToDateTime(obj[1]))}, @3, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[11]))}, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                    new object[] { "int", obj[0], Session["UserId"], obj[10], obj[1] }), 0));
                                                }
                                            }

                                            if (Convert.ToInt32(General.Nz(id, -99)) != -99)
                                            {
                                                General.ExecutaNonQuery($@"
                                                    BEGIN
                                                        UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1{obj[4]});
                                                        UPDATE ""Admin_NrActAd"" SET ""Tiparit""=1 WHERE ""IdAuto""=@1;
                                                    END;", new object[] { id });
                                            }
                                        }
                                        else
                                            General.ExecutaNonQuery($@"UPDATE ""Admin_NrActAd"" SET ""Tiparit""=1 WHERE ""IdAuto""=@1", new object[] { General.Nz(obj[3], -99) });

                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;
                                    }
                                    catch (Exception) { }
                                }                               
                            }
                            break;
                        case "btnSemnat":
                            {                    
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (General.Nz(obj[2], "").ToString() == "")
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu exista numar") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[5], 0)) == 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu este tiparita") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[6], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este semnata") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[7], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este finalizata") + System.Environment.NewLine;
                                            continue;
                                        }

                                        string strSql = $@"UPDATE ""Admin_NrActAd"" SET ""Semnat""=1 WHERE ""IdAuto""=@1;";

                                        //cazul cand este candidat
                                        if (Convert.ToInt32(General.Nz(obj[10], 0)) == 1)
                                        {
                                            DateTime dtLucru = General.DamiDataLucru();
                                            if (Convert.ToDateTime(obj[1]).Year == dtLucru.Year && Convert.ToDateTime(obj[1]).Month == dtLucru.Month)
                                                strSql += $@"UPDATE F100 SET F10025=0 WHERE F10003=@2;";
                                            else
                                                strSql += $@"UPDATE F100 SET F10025=999 WHERE F10003=@2;";
                                        }

                                        General.ExecutaNonQuery("BEGIN " + strSql + " END;", new object[] { obj[3], obj[0], Session["UserId"] });

                                        if (Dami.ValoareParam("FinalizareCuActeAditionale") == "1")
                                        {
                                            DataTable dtAvs = General.IncarcaDT($@"SELECT * FROM ""Avs_Cereri"" WHERE ""IdActAd""=@1", new object[] { obj[3] });
                                            for (int x = 0; x < dtAvs.Rows.Count; x++)
                                            {
                                                //cazul cand este angajat
                                                DataRow dr = dtAvs.Rows[x];
                                                if (General.Nz(dr["IdAtribut"], -99).ToString() == "11" && General.Nz(dr["USER_NO"], -99).ToString() == "-89")
                                                {
                                                    //NOP - sporul de vechime nu se duce
                                                    //Florin 2019.11.25 - sporul de vechime nu se duce in F704; el este deja dus la inchidere luna si tot acolo se introduce si in Avs_Cereri
                                                }
                                                else
                                                {
                                                    Cereri pag = new Cereri();
                                                    pag.TrimiteInF704(Convert.ToInt32(General.Nz(dr["Id"], -99)));
                                                    if (Convert.ToInt32(General.Nz(dr["IdAtribut"], -99)) == 2)
                                                        General.ModificaFunctieAngajat(Convert.ToInt32(dr["F10003"]), Convert.ToInt32(General.Nz(dr["FunctieId"], -99)), Convert.ToDateTime(dr["DataModif"]), new DateTime(2100, 1, 1));
                                                }
                                            }
                                        }

                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;
                                    }
                                    catch (Exception) { }
                                }
                            }
                            break;
                        case "btnFinalizat":
                            {
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (General.Nz(obj[2], "").ToString() == "")
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu exista numar") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[5], 0)) == 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu este tiparita") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[6], 0)) == 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("nu este semanta") + System.Environment.NewLine;
                                            continue;
                                        }
                                        if (Convert.ToInt32(General.Nz(obj[7], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("este finalizata") + System.Environment.NewLine;
                                            continue;
                                        }

                                        DataTable dt = General.IncarcaDT($@"UPDATE ""Admin_NrActAd"" SET ""Revisal""=1 WHERE ""IdAuto""=@1", new object[] { obj[3] });
                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;
                                    }
                                    catch (Exception ex)
                                    {
                                        string ert = ex.Message;
                                    }
                                }
                            }
                            break;
                        case "btnFilter":
                            Session["Filtru_ActeAditionale"] = param;
                            break;
                        case "btnPrint":
                            {
                                try
                                {
                                    string paramRaport = "";
                                    string paramRaport_tmp = "";
                                    string ids = "";
                                    var idRap = null as string;
                                    var reportParams = null as object;                                                                  

                                    if (General.Nz(Dami.ValoareParam("RaportActeAditionale_PrintMultiplu"), 0).ToString() == "0")
                                    {
                                        object[] obj = lst[0] as object[];

                                        if (General.Nz(obj[10], "").ToString() == "" || General.Nz(obj[11], "").ToString() == "")
                                        {
                                            msg = "Nu exista numar si data document";
                                            break;
                                        }

                                        if (Convert.ToInt32(General.Nz(obj[1], 0)) == 1)
                                        {
                                            paramRaport = "RaportActeAditionale_Incetare";
                                            reportParams = new
                                            {
                                                Angajat = obj[0],
                                                NrDecizie = obj[10],
                                                DataDecizie = obj[11]
                                            };
                                        }
                                        else
                                        {
                                            if (Convert.ToInt32(General.Nz(obj[13], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_CIM";
                                                reportParams = new
                                                {
                                                    Angajat = obj[0],
                                                    F10003 = obj[0],
                                                    Validate = 0
                                                };
                                            }
                                            else
                                            {
                                                if (Convert.ToInt32(General.Nz(obj[2], 0)) == 1 || Convert.ToInt32(General.Nz(obj[3], 0)) == 1 || Convert.ToInt32(General.Nz(obj[4], 0)) == 1 || Convert.ToInt32(General.Nz(obj[5], 0)) == 1 || Convert.ToInt32(General.Nz(obj[6], 0)) == 1 || Convert.ToInt32(General.Nz(obj[7], 0)) == 1 || Convert.ToInt32(General.Nz(obj[8], 0)) == 1 || Convert.ToInt32(General.Nz(obj[9], 0)) == 1)
                                                {
                                                    if (General.Nz(obj[12], "").ToString() == "")
                                                    {
                                                        msg = "Nu exista data modificare";
                                                        break;
                                                    }

                                                    paramRaport = "RaportActeAditionale_ModificariCIM";
                                                    reportParams = new
                                                    {
                                                        Angajat = obj[0],
                                                        DataModificare = obj[12]
                                                    };
                                                }
                                            }
                                        }

                                        idRap = Dami.ValoareParam(paramRaport);
                                        if (idRap == "")
                                        {
                                            msg = "Nu este setat raportul";
                                            break;
                                        }

                                        if (paramRaport == "")
                                        {
                                            msg = "Nu exista parametrii";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < lst.Count(); i++)
                                        {
                                            object[] obj = lst[i] as object[];

                                            if (General.Nz(obj[10], "").ToString() == "" || General.Nz(obj[11], "").ToString() == "")
                                            {
                                                msg = "Exista inregistrare fara numar si data document";
                                                break;
                                            }

                                            if (Convert.ToInt32(General.Nz(obj[1], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_Incetare";
                                            }

                                            if (Convert.ToInt32(General.Nz(obj[13], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_CIM";
                                            }

                                            if (Convert.ToInt32(General.Nz(obj[2], 0)) == 1 || Convert.ToInt32(General.Nz(obj[3], 0)) == 1 || Convert.ToInt32(General.Nz(obj[4], 0)) == 1 || Convert.ToInt32(General.Nz(obj[5], 0)) == 1 || Convert.ToInt32(General.Nz(obj[6], 0)) == 1 || Convert.ToInt32(General.Nz(obj[7], 0)) == 1 || Convert.ToInt32(General.Nz(obj[8], 0)) == 1 || Convert.ToInt32(General.Nz(obj[9], 0)) == 1 || Convert.ToInt32(General.Nz(obj[15], 0)) == 1)
                                            {
                                                if (General.Nz(obj[12], "").ToString() == "")
                                                {
                                                    msg = "Nu exista data modificare";
                                                    break;
                                                }
                                                paramRaport = "RaportActeAditionale_ModificariCIM";
                                            }

                                            if (Convert.ToInt32(General.Nz(obj[16], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_Suspendare";
                                            }
                                            if (Convert.ToInt32(General.Nz(obj[17], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_SuspendareRevenire";
                                            }
                                            if (Convert.ToInt32(General.Nz(obj[18], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_Detasare";
                                            }
                                            if (Convert.ToInt32(General.Nz(obj[19], 0)) == 1)
                                            {
                                                paramRaport = "RaportActeAditionale_DetasareRevenire";
                                            }


                                            if (paramRaport_tmp != "" && paramRaport != paramRaport_tmp)
                                            {
                                                msg = "Trebuie sa selectati inregistrari care au acelasi tip de modificare";
                                                break;
                                            }

                                            paramRaport_tmp = paramRaport;
                                            //Florin 2019.12.02 - fortam sa aiba ids, pt ca candidatii se aduc dinamic din F100 si nu sunt inca in Admin_NrActAd
                                            if (Convert.ToInt32(General.Nz(obj[13], 0)) == 1)
                                            {
                                                if (General.Nz(obj[0], "").ToString() != "")
                                                    ids += "," + obj[0];
                                            }
                                            else
                                            {
                                                if (General.Nz(obj[14], "").ToString() != "")
                                                    ids += "," + obj[14];
                                            }
                                        }

                                        if (msg != "")
                                            break;

                                        if (ids == "")
                                        {
                                            object[] obj = lst[0] as object[];
                                            if (Convert.ToInt32(General.Nz(obj[13], 0)) == 1)
                                                msg = "Inainte de a imprima trebuie sa efectuati cel putin o operatie";
                                            else
                                                msg = "Nu exista inregistrari selectate";

                                            break;
                                        }

                                        idRap = Dami.ValoareParam(paramRaport);
                                        if (idRap == "")
                                        {
                                            msg = "Nu este setat raportul";
                                            break;
                                        }

                                        int IdSesiune = Convert.ToInt32(General.ExecutaScalar($@"SELECT NEXT VALUE FOR tmpIdPrint_Id_SEQ", null));
                                        if (Constante.tipBD == 2)
                                            IdSesiune = Convert.ToInt32(General.ExecutaScalar($@"SELECT ""tmpIdPrint_Id_SEQ"".NEXTVAL FROM DUAL", null));

                                        string sqlSes = "";
                                        string[] arrIds = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        for (int k = 0; k < arrIds.Length; k++)
                                        {
                                            sqlSes += $"UNION SELECT {IdSesiune}, {arrIds[k]} " + (Constante.tipBD == 2 ? " FROM DUAL" : "");
                                        }

                                        if (sqlSes != "")
                                            General.ExecutaNonQuery(@"INSERT INTO ""tmpIdPrint""(""IdSesiune"", ""Id"") " + sqlSes.Substring(6), null);

                                        reportParams = new
                                        {
                                            IdSesiune
                                        };
                                    }

                                    // New report access interface
                                    var reportId = Convert.ToInt32(idRap);
                                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(reportId);
                                    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);

                                    grDate.JSProperties["cpReportUrl"] = ResolveClientUrl(reportUrl);
                                }
                                catch (Exception ex)
                                {
                                    msg = ex.Message;                                    
                                    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                                }
                            }
                            break;
                    }

                    grDate.JSProperties["cpAlertMessage"] = msg;

                    if (command != "btnPrint")
                    {
                        IncarcaGrid();
                        grDate.Selection.UnselectAll();
                    }                     
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(e.Keys["Id"]);

                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "F10003", "DataModif", "Revisal", "TermenDepasire", "IdAvans", "Semnat", "Motiv", "Suspendare", "SuspendareRev", "Detasare", "DetasareRev" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[3], 0)) != 0 || Convert.ToInt32(General.Nz(obj[6], 0)) != 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In acest stadiu nu mai sunt permise modificari");
                }
                else
                {
                    object docNr = null;
                    object docData = null;

                    ASPxTextBox txtDocNr = grDate.FindEditFormTemplateControl("txtDocNr") as ASPxTextBox;
                    if (txtDocNr != null && txtDocNr.Value != null) docNr = txtDocNr.Value;

                    ASPxDateEdit txtDocData = grDate.FindEditFormTemplateControl("txtDocData") as ASPxDateEdit;
                    if (txtDocData != null && txtDocData.Value != null) docData = txtDocData.Value;

                    //Florin 2020.03.10 - nu se mai face verificarea pt atributele motiv plecare, Suspendare, Revenire Suspendare, Detasare, Revenire Detasare
                    if (General.Nz(obj[7], 0).ToString() == "0" && General.Nz(obj[8], 0).ToString() == "0" && General.Nz(obj[9], 0).ToString() == "0" && General.Nz(obj[10], 0).ToString() == "0" && General.Nz(obj[11], 0).ToString() == "0")
                    {
                        int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(
                            @"SELECT COUNT(*) FROM ""Admin_NrActAd"" A
                            INNER JOIN ""Avs_Cereri"" B ON A.""IdAuto""=B.""IdActAd"" AND B.""IdAtribut"" NOT IN (4, 30, 31, 32, 33)
                            WHERE A.""DocNr""=@1 AND A.F10003=@2 AND A.""IdAuto""<>@3", new object[] { docNr, General.Nz(obj[1], -99), General.Nz(obj[0], -99) }), 0));
                        if (cnt > 0)
                        {
                            grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numarul de document exista deja");
                            e.Cancel = true;
                            grDate.CancelEdit();

                            IncarcaGrid();
                            return;
                        }
                    }

                    if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99)
                    {
                        DataTable dt = new DataTable();
                        id = -99;

                        if (Constante.tipBD == 1)
                        {
                            dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"") 
                                            OUTPUT Inserted.IdAuto
                                            VALUES(@1, @2, @3, @4, {Session["UserId"]}, {General.CurrentDate()}, @5);",
                                            new object[] { obj[1], docNr, docData, obj[2], obj[4] });

                            if (dt.Rows.Count > 0)
                                id = Convert.ToInt32(General.Nz(dt.Rows[0][0],-99));
                        }
                        else
                        {
                            id = General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"") 
                                        VALUES(@2, @3, @4, @5, {Session["UserId"]}, {General.CurrentDate()}, @6) RETURNING ""IdAuto"" INTO @out_1",
                                        new object[] { "int", obj[1], docNr, docData, obj[2], obj[4] });
                        }

                        if (id != -99)
                            General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1" + obj[5] + ")", new object[] { id });
                    }
                    else
                    {
                        string strSql = "";
                        if (docNr == null || docData == null)
                            strSql = $@"
                                BEGIN
                                    UPDATE ""Avs_Cereri"" SET ""IdActAd""=NULL WHERE ""IdActAd""=@1;
                                    DELETE FROM ""Admin_NrActAd"" WHERE ""IdAuto""=@1;
                                END;";
                        else
                            strSql = $@"UPDATE ""Admin_NrActAd"" SET ""DocNr""=@2, ""DocData""=@3, ""Tiparit""=0 WHERE ""IdAuto""=@1";

                        General.ExecutaNonQuery(strSql, new object[] { obj[0], docNr, docData, obj[1], obj[2], Session["UserId"] });
                    }

                    if (Convert.ToDateTime(General.Nz(obj[2], 0)) < Convert.ToDateTime(docData))
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Atentie, data modificare este mai mica decat data documentului") + System.Environment.NewLine;

                }

                e.Cancel = true;
                grDate.CancelEdit();

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            try
            {
                ASPxComboBox cmb = e.Editor as ASPxComboBox;
                if (cmb != null)
                {
                    GridViewDataCheckColumn chk = e.Column as GridViewDataCheckColumn;
                    if (chk != null)
                    {
                        cmb.Items.Clear();
                        cmb.ValueType = chk.PropertiesCheckEdit.ValueType;
                        cmb.Items.Add(string.Empty, null);
                        cmb.Items.Add(Dami.TraduCuvant("Bifat"), chk.PropertiesCheckEdit.ValueChecked);
                        cmb.Items.Add(Dami.TraduCuvant("Nebifat"), chk.PropertiesCheckEdit.ValueUnchecked);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {
            try
            {
                if (e.RowType == GridViewRowType.Data)
                {
                    ASPxGridView grDate = sender as ASPxGridView;

                    GridViewDataColumn colUpload = grDate.Columns["colAtas"].Columns["colUpload"] as GridViewDataColumn;
                    ASPxUploadControl btnUpload = grDate.FindRowCellTemplateControl(e.VisibleIndex, colUpload, "btnDocUpload") as ASPxUploadControl;
                    if (btnUpload != null)
                    {
                        btnUpload.ClientSideEvents.FilesUploadStart = string.Format("function(s,e) {{ onValidateUpload(s,'{0}'); }}", General.Nz(e.GetValue("IdAutoAct"), -99));
                        btnUpload.ClientSideEvents.FileUploadComplete = string.Format("function(s,e) {{ grDate.PerformCallback('btnDocUpload;{0}'); }}", e.GetValue("IdAutoAct"));
                    }

                    GridViewDataColumn colSterge = grDate.Columns["colAtas"].Columns["colSterge"] as GridViewDataColumn;
                    ASPxButton btnSterge = grDate.FindRowCellTemplateControl(e.VisibleIndex, colSterge, "btnSterge") as ASPxButton;
                    if (btnSterge != null)
                        btnSterge.ClientSideEvents.Click = string.Format("function(s,e) {{ grDate.PerformCallback('btnSterge;{0}'); }}", e.GetValue("IdAutoAtasamente"));

                    GridViewDataColumn colArata = grDate.Columns["colAtas"].Columns["colArata"] as GridViewDataColumn;
                    ASPxButton btnArata = grDate.FindRowCellTemplateControl(e.VisibleIndex, colArata, "btnArata") as ASPxButton;
                    if (btnArata != null)
                        btnArata.ClientSideEvents.Click = string.Format("function(s,e) {{ onOpenAttachment({0}); }}", e.GetValue("IdAutoAtasamente"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {
                //se salveaza direct in tabela Atasamente ca sa se vada in tabul din personal
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "Semnat", "F10003", "IdAutoAtasamente", "DocNr", "DocData", "CIMDet", "CIMNed", "Candidat", "Motiv" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

                string strSql = "";
                string categ = "1002";
                if (General.Nz(obj[6], 0).ToString() == "1" || General.Nz(obj[7], 0).ToString() == "1" || General.Nz(obj[8], 0).ToString() == "1")
                    categ = "1001";
                else
                {
                    if (General.Nz(obj[9], 0).ToString() == "1")
                        categ = "1003";
                }

                if (General.Nz(obj[3], "").ToString() == "")
                {//Radu 08.08.2019 - am adaugat FisierNume si FisierExtensie in tabela Atasamente, deoarece, fara acestea, fisierele nu pot fi deschise
                    if (Constante.tipBD == 1)
                        strSql = $@"
                            BEGIN
                                DECLARE @IdAuto TABLE (IdAuto int);

                                INSERT INTO ""Atasamente""(""IdEmpl"", ""IdCategory"", ""DateAttach"", ""Attach"", ""DescrAttach"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                                OUTPUT inserted.IdAuto INTO @IdAuto
                                VALUES( @8, {categ}, {General.CurrentDate()}, @3, @4, @9, @10, @6, {General.CurrentDate()});
                            
                                UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=(SELECT IdAuto FROM @IdAuto) WHERE ""IdAuto""=@2;
                            END;";
                    else
                        strSql = $@"
                            BEGIN
                                DECLARE param_IdAuto number;

                                INSERT INTO ""Atasamente""(""IdEmpl"", ""IdCategory"", ""DateAttach"", ""Attach"", ""DescrAttach"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                                VALUES( @8, {categ}, {General.CurrentDate()}, @3, @4, @9, @10, @6, {General.CurrentDate()})
                                RETURNING ""IdAuto"" INTO param_IdAuto;
                            
                                UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=param_IdAuto WHERE ""IdAuto""=@2;
                            END;";
                }
                else
                {
                    strSql = $@"
                        BEGIN
                            UPDATE ""Atasamente"" SET ""Attach""=@3, ""DescrAttach""=@4 ,""FisierNume"" = @9, ""FisierExtensie"" = @10 WHERE ""IdAuto""=@6;
                        END;";
                }

                string numeFis = Path.GetFileNameWithoutExtension(e.UploadedFile.FileName);
                string extensie = Path.GetExtension(e.UploadedFile.FileName);
                object ext = e.UploadedFile.ContentType;
                string numeComplet = numeFis + "_" + obj[4] + "_" + Convert.ToDateTime(obj[5]).Year + "." + Convert.ToDateTime(obj[5]).Month.ToString().PadLeft(2, '0') + "." + Convert.ToDateTime(obj[5]).Day.ToString().PadLeft(2, '0') + extensie;
                General.ExecutaNonQuery(strSql, new object[] { "Atasamente", obj[0], e.UploadedFile.FileBytes, numeComplet, e.UploadedFile.ContentType, Session["UserId"], obj[3], obj[2], numeComplet, extensie });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }        
    }
}