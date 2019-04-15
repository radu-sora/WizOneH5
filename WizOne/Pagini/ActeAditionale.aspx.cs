﻿using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Avs;
using WizOne.Module;

namespace WizOne.Pagini
{
    public partial class ActeAditionale : System.Web.UI.Page
    {

        bool esteHr = false;

        public class metaDate
        {
            public int Id { get; set; }
            public string Denumire { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            try
            {
                Session["PaginaWeb"] = "Pagini.ActeAditionale";

                string strSql = $@"SELECT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                                FROM (
                                SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                                FROM ""Avs_Cereri"" A
                                LEFT JOIN F100 B ON A.F10003=B.F10003
                                WHERE ""IdStare""=3
                                UNION
                                SELECT A.F10003, COALESCE(A.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(A.F10009,'') AS ""NumeComplet"" 
                                FROM F100 A
                                WHERE A.F10025=900) X
                                LEFT JOIN F100 A ON X.F10003=A.F10003
                                LEFT JOIN F004 G ON A.F10005 = G.F00405
                                LEFT JOIN F005 H ON A.F10006 = H.F00506
                                LEFT JOIN F006 I ON A.F10007 = I.F00607";
                DataTable dtAng = General.IncarcaDT(strSql, null);
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

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

                foreach (dynamic c in grDate.Columns)
                {
                    try
                    {
                        c.Caption = Dami.TraduCuvant(c.FieldName ?? c.Caption, c.Caption);
                    }
                    catch (Exception) { }
                }

                #endregion

                if (!IsPostBack)
                    IncarcaGrid();
                else
                {
                    grDate.DataSource = Session["InformatiaCurenta"];
                    grDate.DataBind();
                }

                if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(grDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        protected void btnFiltru_Click(object sender, EventArgs e)
        {
            try
            {
                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnFiltruSterge_Click(object sender, EventArgs e)
        {
            try
            {
                cmbTip.Value = null;
                cmbAng.Value = null;
                cmbStatus.Value = null;
                txtData.Value = null;

                IncarcaGrid();
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
                string filtru = "";
                if (General.Nz(cmbTip.Value,9).ToString() != "9") filtru = " AND \"Candidat\"= " + cmbTip.Value;
                if (cmbAng.Value != null) filtru += @" AND ""F10003""= " + cmbAng.Value;
                if (txtData.Value != null) filtru += " AND \"DataModif\" = " + General.ToDataUniv(Convert.ToDateTime(txtData.Value));
                if (txtDepasire.Value != null) filtru += " AND \"TermenDepasire\" = " + General.ToDataUniv(Convert.ToDateTime(txtDepasire.Value));

                switch (Convert.ToInt32(General.Nz(cmbStatus.Value,0)))
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


                string cmp = "CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1)))";
                if (Constante.tipBD == 2) cmp = " CAST(ROWNUM AS INT) ";

                #region OLD

                ////dt = General.IncarcaDT($@"
                ////    SELECT {cmp} AS IdAuto, X.*, 
                ////    CASE WHEN X.Candidat = 0 THEN J.DocNr ELSE A.F100985 END AS DocNr, 
                ////    CASE WHEN X.Candidat = 0 THEN J.DocData ELSE A.F100986 END AS DocData,
                ////    CASE WHEN COALESCE(J.IdStare,0)=3 THEN 1 ELSE 0 END AS Tiparit,
                ////    CASE WHEN COALESCE(J.IdStare,0)=4 THEN 1 ELSE 0 END AS Semnat,
                ////    CASE WHEN COALESCE(J.IdStare,0)=5 THEN 1 ELSE 0 END AS Revisal
                ////    FROM (
                ////    SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                ////    CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END AS CORCod,
                ////    CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END AS FunctieId,
                ////    CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END AS Norma,
                ////    CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END AS Salariul,
                ////    NULL AS Spor,
                ////    CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END AS Structura,
                ////    CASE WHEN COALESCE(DurataTimpMunca, 0) = 1 THEN 1 ELSE 0 END AS CIMDet,
                ////    CASE WHEN COALESCE(DurataTimpMunca, 0) = 2 THEN 1 ELSE 0 END AS CIMNed,
                ////    CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END AS Motiv
                ////    FROM Avs_Cereri A
                ////    LEFT JOIN F100 B ON A.F10003 = B.F10003
                ////    WHERE A.IdStare = 3
                ////    UNION
                ////    SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                ////    null, null, null, null, null, null, null, null, null
                ////    FROM F100 A
                ////    WHERE A.F10025 = 900) X
                ////    LEFT JOIN F100 A ON X.F10003 = A.F10003
                ////    LEFT JOIN F004 G ON A.F10005 = G.F00405
                ////    LEFT JOIN F005 H ON A.F10006 = H.F00506
                ////    LEFT JOIN F006 I ON A.F10007 = I.F00607
                ////    LEFT JOIN Admin_NrActAd J ON X.F10003=J.F10003
                ////    WHERE 1=1 {filtru}", null);





                //dt = General.IncarcaDT($@"
                //        SELECT * FROM (
                //        SELECT {cmp} AS Cheie, X.*,
                //        (SELECT MIN(ColData) FROM (
                //        SELECT CASE WHEN Candidat = 1 THEN 
                //        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,F10022) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                //        ELSE '2100-01-01' END AS ColData 
                //        UNION
                //        SELECT CASE WHEN Motiv = 1 THEN  
                //        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=F100993 AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                //        ELSE '2100-01-01' END AS ColData  
                //        UNION
                //        SELECT CASE WHEN Salariul = 1 THEN 
                //        (SELECT Zi FROM (
                //        SELECT Zi, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as IdAuto 
                //        FROM tblZile WHERE Zi>=DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays)) x
                //        WHERE IdAuto=19)
                //        ELSE '2100-01-01' END AS ColData 
                //        UNION
                //        SELECT CASE WHEN CORCod=1 OR FunctieId = 1 OR CIMDet=1 OR CIMNed=1 THEN 
                //        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                //        ELSE '2100-01-01' END AS ColData 
                //        ) x) AS TermenDepasire
                //        FROM (
                //        SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                //        MAX(CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END) AS CORCod,
                //        MAX(CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END) AS FunctieId,
                //        MAX(CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END) AS Norma,
                //        MAX(CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END) AS Salariul,
                //        MAX(0) AS Spor,
                //        MAX(CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END) AS Structura,
                //        MAX(CASE WHEN COALESCE(DurataContract, 0) = 2 THEN 1 ELSE 0 END) AS CIMDet,
                //        MAX(CASE WHEN COALESCE(DurataContract, 0) = 1 THEN 1 ELSE 0 END) AS CIMNed,
                //        MAX(CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END) AS Motiv,
                //        J.DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                //        J.IdAuto AS IdAutoAct, 
                //        CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas,
                //        (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                //        FROM Avs_Cereri AA
                //        LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                //        LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                //        WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,-99)=COALESCE(J.DocData,-99)
                //        GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                //        FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993
                //        FROM Avs_Cereri A
                //        LEFT JOIN F100 B ON A.F10003 = B.F10003
                //        LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                //        WHERE A.IdStare = 3
                //        GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993
                //        UNION
                //        SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                //        0, 0, 0, 0, 0, 0, 0, 0, 0, 
                //        A.F100985, A.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                //        J.IdAuto AS IdAutoAct,
                //        CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                //        A.F10022, A.F100993
                //        FROM F100 A
                //        LEFT JOIN Admin_NrActAd J ON A.F10003=J.F10003
                //        WHERE A.F10025 = 900) X
                //        ) AS Y
                //        WHERE 1=1 " + filtru, null);

                #endregion

                if (Constante.tipBD == 1)
                {

                    #region SQL

                    dt = General.IncarcaDT($@"
                            SELECT * FROM (
                            SELECT {cmp} AS ""Cheie"", X.*,
                            (SELECT MIN(""ColData"") FROM (
                            SELECT CASE WHEN ""Candidat"" = 1 THEN 
                            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,F10022) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                            ELSE '2100-01-01' END AS ColData 
                            UNION
                            SELECT CASE WHEN Motiv = 1 THEN X.DataModif ELSE '2100-01-01' END AS ColData  
                            UNION
                            SELECT CASE WHEN Salariul = 1 THEN 
                            (SELECT Zi FROM (
                            SELECT Zi, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as IdAuto 
                            FROM tblZile WHERE Zi>=DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays)) x
                            WHERE IdAuto=19)
                            ELSE '2100-01-01' END AS ColData 
                            UNION
                            SELECT CASE WHEN CORCod=1 OR FunctieId = 1 OR CIMDet=1 OR CIMNed=1 THEN 
                            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                            ELSE '2100-01-01' END AS ColData 
                            ) x) AS TermenDepasire
                            FROM (
                            SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                            MAX(CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END) AS CORCod,
                            MAX(CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END) AS FunctieId,
                            MAX(CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END) AS Norma,
                            MAX(CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END) AS Salariul,
                            MAX(0) AS Spor,
                            MAX(CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END) AS Structura,
                            MAX(CASE WHEN COALESCE(DurataContract, 0) = 2 THEN 1 ELSE 0 END) AS CIMDet,
                            MAX(CASE WHEN COALESCE(DurataContract, 0) = 1 THEN 1 ELSE 0 END) AS CIMNed,
                            MAX(CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END) AS Motiv,
                            J.DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct, 
                            CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas,
                            (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                            FROM Avs_Cereri AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                            WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,-99)=COALESCE(J.DocData,-99)
                            GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                            FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993
                            FROM Avs_Cereri A
                            LEFT JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                            WHERE A.IdStare = 3
                            GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993, J.Candidat
                            UNION
                            SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                            A.F100985, A.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct,
                            CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                            A.F10022, A.F100993
                            FROM F100 A
                            LEFT JOIN Admin_NrActAd J ON A.F10003=J.F10003
                            WHERE A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) X
                            ) AS Y
                            WHERE 1=1 " + filtru, null);

                    #endregion
                }
                else
                {

                    #region Orcl

                    dt = General.IncarcaDT($@"
                            SELECT * FROM (
                            SELECT {cmp} AS ""Cheie"", X.*,
                            (SELECT MIN(""ColData"") FROM (
                            SELECT CASE WHEN ""Candidat"" = 1 THEN 
                            (SELECT ""Zi"" FROM ""tblZile"" WHERE ""Zi""<=(F10022-1) AND ""ZiSapt""<=5 AND ""Zi"" NOT IN (SELECT day FROM Holidays) AND ROWNUM<=1 ORDER BY ""Zi"" Desc)
                            ELSE TO_DATE('DD-MON-YYYY','01-JAN-2100') END AS ""ColData"" 
                            UNION
                            SELECT CASE WHEN ""Motiv"" = 1 THEN X.""DataModif"" ELSE TO_DATE('DD-MON-YYYY','01-JAN-2100') END AS "" ColData""   
                            UNION
                            SELECT CASE WHEN "" Salariul""  = 1 THEN 
                            (SELECT ""Zi"" 
                            FROM ""tblZile""  WHERE ""Zi"" >=""DataModif""  AND ""ZiSapt"" <=5 AND ""Zi""  NOT IN (SELECT day FROM Holidays) AND ROWNUM=19)
                            ELSE TO_DATE('DD-MON-YYYY','01-JAN-2100') END AS ColData 
                            UNION
                            SELECT CASE WHEN ""CORCod""=1 OR ""FunctieId"" = 1 OR ""CIMDet""=1 OR ""CIMNed""=1 THEN 
                            (SELECT TOP 1 ""Zi"" FROM ""tblZile"" WHERE ""Zi""<""DataModif"" AND ""ZiSapt""<=5 AND ""Zi"" NOT IN (SELECT day FROM Holidays) ORDER BY ""Zi"" Desc)
                            ELSE TO_DATE('DD-MON-YYYY','01-JAN-2100') END AS ""ColData""
                            ) x) AS ""TermenDepasire""
                            FROM (
                            SELECT A.F10003, COALESCE(B.F10008, '') || ' ' || COALESCE(B.F10009, '') AS ""NumeComplet"", A.""DataModif"", 0 AS ""Candidat"",
                            MAX(CASE WHEN COALESCE(""CORCod"", 0) > 0 THEN 1 ELSE 0 END) AS ""CORCod"",
                            MAX(CASE WHEN COALESCE(""FunctieId"", 0) > 0 THEN 1 ELSE 0 END) AS ""FunctieId"",
                            MAX(CASE WHEN COALESCE(""Norma"", 0) > 0 THEN 1 ELSE 0 END) AS ""Norma"",
                            MAX(CASE WHEN COALESCE(""SalariulBrut"", 0) > 0 OR COALESCE(""SalariulNet"", 0) > 0 THEN 1 ELSE 0 END) AS ""Salariul"",
                            MAX(0) AS ""Spor"",
                            MAX(CASE WHEN COALESCE(""SubcompanieId"", 0) > 0 OR COALESCE(""FilialaId"", 0) > 0 OR COALESCE(""SectieId"", 0) > 0 OR COALESCE(""DeptId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Structura"",
                            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 2 THEN 1 ELSE 0 END) AS ""CIMDet"",
                            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 1 THEN 1 ELSE 0 END) AS ""CIMNed"",
                            MAX(CASE WHEN COALESCE(""MotivId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Motiv"",
                            CAST(J.""DocNr"" AS varchar2(20)) AS ""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""tblFisiere"" FIS WHERE FIS.""Tabela""='Admin_NrActAd' AND FIS.""Id""=J.""IdAuto"" AND FIS.""EsteCerere""=0) = 0 THEN 0 ELSE 1 END AS ""AreAtas"",
                            (SELECT LISTAGG(AA.""Id"", ',') WITHIN GROUP (ORDER BY AA.""Id"") AS ""Id""
                            FROM ""Avs_Cereri"" AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN ""Admin_NrActAd"" JJ ON AA.""IdActAd""=JJ.""IdAuto""
                            WHERE AA.""IdStare"" = 3 AND AA.F10003=A.F10003 AND AA.""DataModif""=A.""DataModif"" AND COALESCE(JJ.""DocNr"",-99)=COALESCE(J.""DocNr"",-99) AND JJ.""DocData""=COALESCE(J.""DocData"",TO_DATE('DD-MON-YYYY','01-JAN-1950'))
                            GROUP BY AA.""Id"", AA.F10003, BB.F10008, BB.F10009, AA.""DataModif"", JJ.""DocNr"", JJ.""DocData"", COALESCE(JJ.""Tiparit"",0), COALESCE(JJ.""Semnat"",0), COALESCE(JJ.""Revisal"",0), JJ.""IdAuto""
                            ) AS ""IdAvans"", B.F10022, B.F100993
                            FROM ""Avs_Cereri"" A
                            LEFT JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN ""Admin_NrActAd"" J ON A.""IdActAd""=J.""IdAuto""
                            WHERE A.""IdStare"" = 3
                            GROUP BY A.F10003, B.F10008, B.F10009, A.""DataModif"", J.""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0), COALESCE(J.""Semnat"",0), COALESCE(J.""Revisal"",0), J.""IdAuto"", B.F10022, B.F100993, J.""Candidat""
                            UNION
                            SELECT A.F10003, COALESCE(A.F10008, '') || ' ' || COALESCE(A.F10009, '') AS ""NumeComplet"", A.F10022, 1 AS ""Candidat"",
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                            A.F100985, A.F100986, COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"",
                            CASE WHEN (SELECT COUNT(*) FROM ""tblFisiere"" FIS WHERE FIS.""Tabela""='Admin_NrActAd' AND FIS.""Id""=J.""IdAuto"" AND FIS.""EsteCerere""=0) = 0 THEN 0 ELSE 1 END AS ""AreAtas"", ',-1' AS ""IdAvans"",
                            A.F10022, A.F100993
                            FROM F100 A
                            LEFT JOIN ""Admin_NrActAd"" J ON A.F10003=J.F10003
                            WHERE A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) X
                            ) 
                            WHERE 1=1 " + filtru, null);

                    #endregion
                }

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

        protected void grDate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            try
            {
                string msg = "";
                string str = e.Parameters;
                if (str != "")
                {
                    string[] arr = e.Parameters.Split(';');
                    if (arr.Length != 2 || arr[0] == "")
                    {
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Parametrii insuficienti");
                        //MessageBox.Show("Insuficienti parametrii", MessageBox.icoError, "Eroare !");
                        return;
                    }

                    List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "IdAvans", "Tiparit", "Semnat", "Revisal", "NumeComplet", "DocData", "Candidat", "TermenDepasire" });
                    if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                    switch (arr[0])
                    {
                        case "btnDocUpload":
                            {
                                if (arr[1] == "")
                                    grDate.JSProperties["cpAlertMessage"] = "Nu se poate adauga atasament deoarece nu exista numar";
                                else
                                    IncarcaGrid();
                            }
                            break;
                        case "btnSterge":
                            {
                                //string sqlFis = $@"BEGIN
                                //    DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                                //    UPDATE ""Admin_NrActAd"" SET ""Semnat""=0 WHERE ""IdAuto""=@2;
                                //    END;";

                                object[] obj = lst[0] as object[];
                                General.ExecutaNonQuery($@"DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0", new object[] { "Admin_NrActAd", arr[1] });
                            }
                            break;
                        case "btnNr":
                            {
                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (Convert.ToInt32(General.Nz(obj[2], 0)) != 0)
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

                                        if (Convert.ToInt32(General.Nz(obj[2], 0)) == 0)
                                        {
                                            DataTable dt = new DataTable();
                                            int id = -99;

                                            if (Constante.tipBD == 1)
                                            {
                                                dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                OUTPUT Inserted.IdAuto
                                                VALUES(@1, COALESCE((SELECT MAX(COALESCE(DocNr,0)) FROM Admin_NrActAd WHERE F10003=@1),0) + 1, {General.CurrentDate()},@2, @3, {General.CurrentDate()}, @4, @5);",
                                                new object[] { obj[0], obj[1], Session["UserId"], obj[11], obj[10] });

                                                if (dt.Rows.Count > 0)
                                                {
                                                    id = Convert.ToInt32(General.Nz(dt.Rows[0][0], -99));
                                                    //int id = Convert.ToInt32(dt.Rows[0]["IdAuto"]);
                                                    //General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1" + obj[4] + ")", new object[] { dt.Rows[0][0] });
                                                }
                                            }
                                            else
                                            {
                                                id = General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                VALUES(@2, COALESCE((SELECT MAX(COALESCE(DocNr,0)) FROM Admin_NrActAd WHERE F10003=@1),0) + 1, {General.CurrentDate()},@3, @4, {General.CurrentDate()}, @5, @6) RETURNING ""IdAuto"" INTO @out_1;",
                                                new object[] { "int",  obj[0], obj[1], Session["UserId"], obj[11], obj[10] });
                                            }

                                            if (Convert.ToInt32(General.Nz(id,-99)) != -99)
                                                General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1" + obj[4] + ")", new object[] { id });


                                            msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;
                                        }
                                    }
                                    catch (Exception) { }
                                }

                                if (msg != "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = msg;
                                    IncarcaGrid();

                                    grDate.Selection.UnselectAll();
                                }
                            }
                            break;
                        case "btnTiparit":
                            {
                                //List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "Tiparit", "Semnat", "Revisal", "NumeComplet" });
                                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (Convert.ToInt32(General.Nz(obj[2], 0)) == 0)
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

                                        string strSql = $@"IF (SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE ""IdAuto""=@1)=0
                                                INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"",""Tiparit"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                VALUES(@4, @2, @3, 1, @5, @6, {General.CurrentDate()}, @7, @8)
                                                ELSE
                                                UPDATE ""Admin_NrActAd"" SET ""Tiparit""=1 WHERE ""IdAuto""=@1";

                                        //List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "IdAvans", "Tiparit", "Semnat", "Revisal", "NumeComplet", "DocData" });
                                        //DataTable dt = General.IncarcaDT($@"UPDATE ""Admin_NrActAd"" SET Tiparit=1 WHERE ""IdAuto""=@1", new object[] { General.Nz(obj[3],-99) });
                                        DataTable dt = General.IncarcaDT(strSql, new object[] { General.Nz(obj[3], -99), obj[2], obj[9], obj[0], obj[1], Session["UserId"], obj[11], obj[10] });
                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;

                                    }
                                    catch (Exception) { }
                                }

                                grDate.JSProperties["cpAlertMessage"] = msg;
                                IncarcaGrid();

                                grDate.Selection.UnselectAll();
                            }
                            break;
                        case "btnSemnat":
                            {
                                //string msg = "";
                                //List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "Tiparit", "Semnat", "Revisal", "NumeComplet" });
                                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (Convert.ToInt32(General.Nz(obj[2], 0)) == 0)
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

                                        DataTable dt = General.IncarcaDT($@"UPDATE ""Admin_NrActAd"" SET ""Semnat""=1 WHERE ""IdAuto""=@1", new object[] { obj[3] });
                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;

                                    }
                                    catch (Exception) { }
                                }

                                grDate.JSProperties["cpAlertMessage"] = msg;
                                IncarcaGrid();

                                grDate.Selection.UnselectAll();
                            }
                            break;
                        case "btnFinalizat":
                            {
                                //string msg = "";
                                //List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "Tiparit", "Semnat", "Revisal", "NumeComplet" });
                                //if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                                for (int i = 0; i < lst.Count(); i++)
                                {
                                    try
                                    {
                                        object[] obj = lst[i] as object[];
                                        if (Convert.ToInt32(General.Nz(obj[2], 0)) == 0)
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

                                        string strSql = $@"UPDATE ""Admin_NrActAd"" SET Revisal=1 WHERE ""IdAuto""=@1;";

                                        //cazul cand este candidat
                                        if (Convert.ToInt32(General.Nz(obj[10], 0)) == 1)
                                        {
                                            DateTime dtLucru = General.DamiDataLucru();
                                            if (Convert.ToDateTime(obj[1]).Year == dtLucru.Year && Convert.ToDateTime(obj[1]).Month == dtLucru.Month)
                                                strSql += $@"UPDATE F100 SET F10025=0 WHERE F10003=@2;";
                                            else
                                                strSql += $@"UPDATE F100 SET F10025=999 WHERE F10003=@2;";
                                        }

                                        //copiem atasamentul si in personal
                                        strSql += $@"
                                            INSERT INTO ""Atasamente""(""IdEmpl"", ""IdCategory"", ""DateAttach"", ""Attach"", ""DescrAttach"", USER_NO, TIME) 
                                            SELECT @2, 999, {General.CurrentDate()}, ""Fisier"", ""FisierNume"", @3, {General.CurrentDate()} 
                                            FROM ""tblFisiere"" WHERE ""Tabela""='Admin_NrActAd' AND ""Id""=@1;";

                                        General.ExecutaNonQuery("BEGIN " + strSql + " END;", new object[] { obj[3], obj[0], Session["UserId"] });

                                        if (Dami.ValoareParam("FinalizareCuActeAditionale") == "1")
                                        {
                                            DataTable dtAvs = General.IncarcaDT($@"SELECT * FROM ""Avs_Cereri"" WHERE ""IdActAd""=@1", new object[] { obj[3] });
                                            for (int x = 0; x < dtAvs.Rows.Count; x++)
                                            {
                                                //cazul cand este angajat
                                                DataRow dr = dtAvs.Rows[x];
                                                Cereri pag = new Cereri();
                                                pag.TrimiteInF704(Convert.ToInt32(General.Nz(dr["Id"], -99)));
                                                if (Convert.ToInt32(General.Nz(dr["IdAtribut"], -99)) == 2)
                                                    General.ModificaFunctieAngajat(Convert.ToInt32(dr["F10003"]), Convert.ToInt32(General.Nz(dr["FunctieId"], -99)), Convert.ToDateTime(dr["DataModif"]), new DateTime(2100, 1, 1));
                                            }
                                        }
                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;

                                    }
                                    catch (Exception ex)
                                    {
                                        string ert = ex.Message;
                                    }
                                }

                                grDate.JSProperties["cpAlertMessage"] = msg;
                                IncarcaGrid();

                                grDate.Selection.UnselectAll();
                            }
                            break;
                    }

                    IncarcaGrid();
                }
            }
            catch (Exception ex)
            {
                grDate.JSProperties["cpAlertMessage"] = ex.Message;
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDate_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                //var id = e.Keys["Id"];
                int id = Convert.ToInt32(e.Keys["Id"]);

                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "F10003", "DataModif", "Revisal", "TermenDepasire", "IdAvans" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[3], 0)) != 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica. Informatie trimisa in Revisal");
                    //MessageBox.Show("Informatie trimisa in revisal.", MessageBox.icoWarning, "Nu se poate modifica !");
                }
                else
                {
                    object docNr = null;
                    object docData = null;

                    ASPxTextBox txtDocNr = grDate.FindEditFormTemplateControl("txtDocNr") as ASPxTextBox;
                    if (txtDocNr != null && txtDocNr.Value != null) docNr = txtDocNr.Value;

                    ASPxDateEdit txtDocData = grDate.FindEditFormTemplateControl("txtDocData") as ASPxDateEdit;
                    if (txtDocData != null && txtDocData.Value != null) docData = txtDocData.Value;

                    //string strSql = $@"IF (SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE ""IdAuto""=@1)=0
                    //INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME) 
                    //VALUES(@4, @2, @3, @5, @6, {General.CurrentDate()})
                    //ELSE
                    //UPDATE ""Admin_NrActAd"" SET ""DocNr""=@2, ""DocData""=@3 WHERE ""IdAuto""=@1";

                    if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99)
                    {
                        DataTable dt = new DataTable();
                        //int id = -99;
                        id = -99;

                        if (Constante.tipBD == 1)
                        {
                            dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"") 
                                            OUTPUT Inserted.IdAuto
                                            VALUES(@1, @2, @3, @4, {Session["UserId"]}, {General.CurrentDate()}, @5);",
                                            new object[] { obj[1], docNr, docData, obj[2], obj[4] });

                            if (dt.Rows.Count > 0)
                            {
                                id = Convert.ToInt32(General.Nz(dt.Rows[0][0],-99));
                                //General.ExecutaNonQuery($@"UPDATE ""Avs_Cereri"" SET ""IdActAd""=@1 WHERE ""Id"" IN (-1" + obj[5] + ")", new object[] { dt.Rows[0][0] });
                            }
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
                        strSql = $@"UPDATE ""Admin_NrActAd"" SET ""DocNr""=@2, ""DocData""=@3 WHERE ""IdAuto""=@1";

                        if (docNr == null)
                            strSql = $@"
                                BEGIN
                                    UPDATE ""Avs_Cereri"" SET ""IdActAd""=NULL WHERE ""IdActAd""=@1;
                                    DELETE FROM ""Admin_NrActAd"" WHERE ""IdAuto""=@1;
                                END;";

                        General.ExecutaNonQuery(strSql, new object[] { obj[0], docNr, docData, obj[1], obj[2], Session["UserId"] });
                    }
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
                        cmb.Items.Add(Dami.TraduCuvant("BIfat"), chk.PropertiesCheckEdit.ValueChecked);
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
                        btnUpload.ClientSideEvents.FilesUploadStart = string.Format("function(s,e) {{ ValidareUpload({0}); }}", General.Nz(e.GetValue("IdAutoAct"), -99));
                        btnUpload.ClientSideEvents.FileUploadComplete = string.Format("function(s,e) {{ grDate.PerformCallback('btnDocUpload;{0}'); }}", e.GetValue("IdAutoAct"));
                    }

                    GridViewDataColumn colSterge = grDate.Columns["colAtas"].Columns["colSterge"] as GridViewDataColumn;
                    ASPxButton btnSterge = grDate.FindRowCellTemplateControl(e.VisibleIndex, colSterge, "btnSterge") as ASPxButton;
                    if (btnSterge != null)
                        btnSterge.ClientSideEvents.Click = string.Format("function(s,e) {{ grDate.PerformCallback('btnSterge;{0}'); }}", e.GetValue("IdAutoAct"));

                    GridViewDataColumn colArata = grDate.Columns["colAtas"].Columns["colArata"] as GridViewDataColumn;
                    ASPxButton btnArata = grDate.FindRowCellTemplateControl(e.VisibleIndex, colArata, "btnArata") as ASPxButton;
                    if (btnArata != null)
                        btnArata.ClientSideEvents.Click = string.Format("function(s,e) {{ GoToAtasMode({0}); }}", e.GetValue("IdAutoAct"));
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
                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "Semnat" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

                //string sqlFis = $@"
                //            BEGIN
                //            if ((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0) = 0)
                //                BEGIN                                
                //                    INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                //                    SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")}
                //                    UPDATE ""Admin_NrActAd"" SET ""Semnat""=1 WHERE ""IdAuto""=@2;
                //                END
                //            ELSE
                //                UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                //            END;";

                string sqlFis = $@"
                            BEGIN
                            if ((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0) = 0)                               
                                INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                                SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")}
                            ELSE
                                UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                            END;";
                General.ExecutaNonQuery(sqlFis, new object[] { "Admin_NrActAd", obj[0], e.UploadedFile.FileBytes, e.UploadedFile.FileName, e.UploadedFile.ContentType, Session["UserId"] });

                //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
                //if (Convert.ToInt32(General.Nz(obj[0],-99)) == -99)
                //    grDate.JSProperties["cpAlertMessage"] = "Nu se poate adauga atasament deoarece nu exista numar";

                //IncarcaGrid();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //Session["ReportId"] = 1;
                //Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?Angajat=460&F10003=460&Validate=0", false);
                //return;

                string param = "";
                string paramRaport = "";
                List<Object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "Motiv", "CIMDet", "CIMNed", "CORCod", "FunctieId", "Norma", "Salariul", "Spor", "Structura", "DocNr", "DocData", "DataModif", "Candidat" });

                if (lst == null || lst.Count() == 0 || lst[0] == null) return;

                object[] obj = lst[0] as object[];

                    //if(Convert.ToInt32(General.Nz(lst[1],0)) == 1)
                    //{
                    //    Session["ReportId"] = Dami.ValoareParam("RaportActeAditionale_Incetare");
                    //    Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?Angajat=460&NrDecizie=" + lst[10] + "&DataDecizie=" + lst[11]);
                    //}
                    //else
                    //{
                    //    if (Convert.ToInt32(General.Nz(lst[2], 0)) == 1 || Convert.ToInt32(General.Nz(lst[3], 0)) == 1)
                    //    {
                    //        Session["ReportId"] = Dami.ValoareParam("RaportActeAditionale_CIM");
                    //        Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?Angajat=" + lst[0] + "&F10003=" + lst[0] + "&Validate=0");
                    //    }
                    //    else
                    //    {
                    //        if (Convert.ToInt32(General.Nz(lst[4], 0)) == 1 || Convert.ToInt32(General.Nz(lst[3], 5)) == 1 ||
                    //            Convert.ToInt32(General.Nz(lst[6], 0)) == 1 || Convert.ToInt32(General.Nz(lst[7], 5)) == 1 ||
                    //            Convert.ToInt32(General.Nz(lst[8], 0)) == 1 || Convert.ToInt32(General.Nz(lst[9], 5)) == 1)
                    //        {
                    //            Session["ReportId"] = Dami.ValoareParam("RaportActeAditionale_ModificariCIM");
                    //            Response.Redirect("../Generatoare/Reports/Pages/ReportView.aspx?Angajat=460&DataModificare=" + lst[12]);
                    //        }
                    //    }
                    //}

                if (Convert.ToInt32(General.Nz(obj[1], 0)) == 1)
                {
                    DateTime ziua = Convert.ToDateTime(obj[11]);
                    paramRaport = "RaportActeAditionale_Incetare";
                    param = "&NrDecizie=" + obj[10] + "&DataDecizie=" + +ziua.Year + "-" + ziua.Month.ToString().PadLeft(2, '0') + "-" + ziua.Day.ToString().PadLeft(2, '0');
                }
                else
                {
                    if (Convert.ToInt32(General.Nz(obj[13], 0)) == 1)
                    {
                        paramRaport = "RaportActeAditionale_CIM";
                        param = "&F10003=" + obj[0] + "&Validate=0";
                    }
                    else
                    {
                        if (Convert.ToInt32(General.Nz(obj[4], 0)) == 1 || Convert.ToInt32(General.Nz(obj[3], 5)) == 1 || Convert.ToInt32(General.Nz(obj[6], 0)) == 1 || Convert.ToInt32(General.Nz(obj[7], 5)) == 1 || Convert.ToInt32(General.Nz(obj[8], 0)) == 1 || Convert.ToInt32(General.Nz(obj[9], 5)) == 1)
                        {
                            DateTime ziua = Convert.ToDateTime(obj[12]);
                            paramRaport = "RaportActeAditionale_ModificariCIM";
                            param = "&DataModificare=" + ziua.Year + "-" + ziua.Month.ToString().PadLeft(2, '0') + "-" + ziua.Day.ToString().PadLeft(2,'0');
                        }
                    }
                }

                string idRap = Dami.ValoareParam(paramRaport);
                if (idRap == "")
                {
                    MessageBox.Show("Nu este setat raport", MessageBox.icoWarning, "Operatie anulata");
                    return;
                }

                if (param == "")
                {
                    MessageBox.Show("Nu exista parametrii", MessageBox.icoWarning, "Operatie anulata");
                    return;
                }

                Session["ReportId"] = Convert.ToInt32(idRap);
                string url = "../Generatoare/Reports/Pages/ReportView.aspx?Angajat=" + obj[0] + param;
                Response.Redirect(url, false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //protected void btnNr_Click(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        string msg = "";
        //        string strSql = "";
        //        List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct" });
        //        if (lst == null || lst.Count() == 0 || lst[0] == null) return;

        //        for (int i = 0; i < lst.Count(); i++)
        //        {
        //            object[] arr = lst[i] as object[];
        //            if (Convert.ToInt32(General.Nz(arr[2], 0)) == 0)
        //            {
        //                strSql += $@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME) 
        //                VALUES(@1, COALESCE((SELECT MAX(COALESCE(DocNr,0)) FROM Admin_NrActAd WHERE F10003=@1),0) + 1, {General.CurrentDate()},@2, @3, {General.CurrentDate()})";
        //                General.ExecutaNonQuery(strSql, new object[] { arr[0], arr[1], Session["UserId"] });
        //            }
        //        }

        //        if (strSql != "")
        //        {
        //            MessageBox.Show("Proces realizat cu succes", MessageBox.icoWarning, "Atentie !");
        //            //grDate.JSProperties["cpAlertMessage"] = msg;
        //            grDate.DataBind();

        //            grDate.Selection.UnselectAll();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}



    }
}