using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

                if (!IsPostBack)
                {
                    DataTable dtCmp = General.IncarcaDT("SELECT F00202, F00204 FROM F002", null);
                    cmbCmp.DataSource = dtCmp;
                    cmbCmp.DataBind();
                    cmbCmp.SelectedIndex = -1;
                    if (dtCmp != null && dtCmp.Rows.Count > 1)
                        pnlComp.Visible = true;

                    //string strSql = $@"SELECT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                    //            FROM (
                    //            SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                    //            FROM ""Avs_Cereri"" A
                    //            INNER JOIN F100 B ON A.F10003=B.F10003
                    //            WHERE ""IdStare""=3 AND B.F10002={cmbCmp.Value}
                    //            UNION
                    //            SELECT A.F10003, COALESCE(A.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(A.F10009,'') AS ""NumeComplet"" 
                    //            FROM F100 A
                    //            WHERE A.F10025=900 AND A.F10002={cmbCmp.Value}) X
                    //            LEFT JOIN F100 A ON X.F10003=A.F10003
                    //            LEFT JOIN F004 G ON A.F10005 = G.F00405
                    //            LEFT JOIN F005 H ON A.F10006 = H.F00506
                    //            LEFT JOIN F006 I ON A.F10007 = I.F00607";


                    string strSql = $@"SELECT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
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

                    DataTable dtAng = General.IncarcaDT(strSql, null);
                    Session["Acte_Ang"] = dtAng;
                    cmbAng.DataSource = dtAng;
                    cmbAng.DataBind();

                    //in cazul in care se sterge atasamentul din managemetul de personal
                    General.ExecutaNonQuery(@"UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=NULL WHERE ""IdAutoAtasamente"" NOT IN (SELECT ""IdAuto"" FROM ""Atasamente"")", null);
                }
                else
                {
                    DataTable dtAng = Session["Acte_Ang"] as DataTable;
                    cmbAng.DataSource = dtAng;
                    cmbAng.DataBind();
                }
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
                {
                    //Florin2019.07.15
                    NameValueCollection lst = HttpUtility.ParseQueryString((Session["Filtru_ActeAditionale"] ?? "").ToString());
                    if (lst.Count > 0)
                    {
                        if (General.Nz(lst["Cmp"], "").ToString() != "") cmbCmp.Value = Convert.ToInt32(lst["Cmp"]);
                        if (General.Nz(lst["Tip"], "").ToString() != "")
                        {
                            switch (General.Nz(lst["Tip"], "").ToString())
                            {
                                case "9":
                                    cmbTip.SelectedIndex = 0;
                                    break;
                                case "0":
                                    cmbTip.SelectedIndex = 1;
                                    break;
                                case "1":
                                    cmbTip.SelectedIndex = 2;
                                    break;
                            }
                        }
                        cmbTip_ValueChanged(null, null);
                        if (General.Nz(lst["Ang"], "").ToString() != "") cmbAng.Value = Convert.ToInt32(lst["Ang"]);
                        if (General.Nz(lst["Status"], "").ToString() != "") cmbStatus.Value = Convert.ToInt32(lst["Status"]);

                        if (General.Nz(lst["Data"], "").ToString() != "") txtData.Value = Convert.ToDateTime(lst["Data"]);
                        if (General.Nz(lst["Depasire"], "").ToString() != "") txtDepasire.Value = Convert.ToDateTime(lst["Depasire"]);

                        Session["Filtru_ActeAditionale"] = "";
                    }

                    btnFiltru_Click(null, null);
                    //IncarcaGrid();
                }
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
                grDate.Selection.UnselectAll();
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
                cmbCmp.SelectedIndex = -1;

                IncarcaGrid();
                grDate.Selection.UnselectAll();
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

                //Florin 2019.09.11
                ////Florin 2019.09.09
                //if (General.Nz(cmbTip.Value, 9).ToString() != "9")
                //{
                //    if (General.Nz(cmbTip.Value, 9).ToString() == "2")
                //        filtru = " AND \"CandidatAngajat\"= 1";
                //    else
                //        filtru = " AND \"Candidat\"= " + cmbTip.Value;
                //}


                switch(General.Nz(cmbTip.Value, 9).ToString())
                {
                    case "0":
                        filtru = " AND \"Candidat\"= 0";
                        break;
                    case "1":
                        filtru = " AND \"Candidat\"= 1 AND \"CandidatAngajat\"= 0";
                        break;
                    case "2":
                        filtru = " AND \"Candidat\"= 1 AND \"CandidatAngajat\"= 1";
                        break;
                    case "9":
                        //NOP
                        break;
                }


                if (cmbAng.Value != null) filtru += @" AND ""F10003""= " + cmbAng.Value;
                if (txtData.Value != null) filtru += " AND \"DataModif\" = " + General.ToDataUniv(Convert.ToDateTime(txtData.Value));
                if (txtDepasire.Value != null) filtru += " AND \"TermenDepasire\" = " + General.ToDataUniv(Convert.ToDateTime(txtDepasire.Value));

                if (cmbCmp.Value != null) companie = " AND B.F10002 = " + cmbCmp.Value;

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

                string filtruSup = "";
                string idExcluseCircuitDoc = General.Nz(General.ExecutaScalar($@"SELECT Valoare FROM ""tblParametrii"" WHERE ""Nume""= 'IdExcluseCircuitDoc'", null),"").ToString();
                if (idExcluseCircuitDoc != "")
                    filtruSup = $@" AND A.""IdAtribut"" NOT IN ({idExcluseCircuitDoc})";

                #region OLD

                               //////dt = General.IncarcaDT($@"
                               //////    SELECT {cmp} AS IdAuto, X.*, 
                               //////    CASE WHEN X.Candidat = 0 THEN J.DocNr ELSE A.F100985 END AS DocNr, 
                               //////    CASE WHEN X.Candidat = 0 THEN J.DocData ELSE A.F100986 END AS DocData,
                               //////    CASE WHEN COALESCE(J.IdStare,0)=3 THEN 1 ELSE 0 END AS Tiparit,
                               //////    CASE WHEN COALESCE(J.IdStare,0)=4 THEN 1 ELSE 0 END AS Semnat,
                               //////    CASE WHEN COALESCE(J.IdStare,0)=5 THEN 1 ELSE 0 END AS Revisal
                               //////    FROM (
                               //////    SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                               //////    CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END AS CORCod,
                               //////    CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END AS FunctieId,
                               //////    CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END AS Norma,
                               //////    CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END AS Salariul,
                               //////    NULL AS Spor,
                               //////    CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END AS Structura,
                               //////    CASE WHEN COALESCE(DurataTimpMunca, 0) = 1 THEN 1 ELSE 0 END AS CIMDet,
                               //////    CASE WHEN COALESCE(DurataTimpMunca, 0) = 2 THEN 1 ELSE 0 END AS CIMNed,
                               //////    CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END AS Motiv
                               //////    FROM Avs_Cereri A
                               //////    LEFT JOIN F100 B ON A.F10003 = B.F10003
                               //////    WHERE A.IdStare = 3
                               //////    UNION
                               //////    SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                               //////    null, null, null, null, null, null, null, null, null
                               //////    FROM F100 A
                               //////    WHERE A.F10025 = 900) X
                               //////    LEFT JOIN F100 A ON X.F10003 = A.F10003
                               //////    LEFT JOIN F004 G ON A.F10005 = G.F00405
                               //////    LEFT JOIN F005 H ON A.F10006 = H.F00506
                               //////    LEFT JOIN F006 I ON A.F10007 = I.F00607
                               //////    LEFT JOIN Admin_NrActAd J ON X.F10003=J.F10003
                               //////    WHERE 1=1 {filtru}", null);





                               ////dt = General.IncarcaDT($@"
                               ////        SELECT * FROM (
                               ////        SELECT {cmp} AS Cheie, X.*,
                               ////        (SELECT MIN(ColData) FROM (
                               ////        SELECT CASE WHEN Candidat = 1 THEN 
                               ////        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,F10022) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               ////        ELSE '2100-01-01' END AS ColData 
                               ////        UNION
                               ////        SELECT CASE WHEN Motiv = 1 THEN  
                               ////        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=F100993 AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               ////        ELSE '2100-01-01' END AS ColData  
                               ////        UNION
                               ////        SELECT CASE WHEN Salariul = 1 THEN 
                               ////        (SELECT Zi FROM (
                               ////        SELECT Zi, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as IdAuto 
                               ////        FROM tblZile WHERE Zi>=DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays)) x
                               ////        WHERE IdAuto=19)
                               ////        ELSE '2100-01-01' END AS ColData 
                               ////        UNION
                               ////        SELECT CASE WHEN CORCod=1 OR FunctieId = 1 OR CIMDet=1 OR CIMNed=1 THEN 
                               ////        (SELECT TOP 1 Zi FROM tblZile WHERE Zi<DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               ////        ELSE '2100-01-01' END AS ColData 
                               ////        ) x) AS TermenDepasire
                               ////        FROM (
                               ////        SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                               ////        MAX(CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END) AS CORCod,
                               ////        MAX(CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END) AS FunctieId,
                               ////        MAX(CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END) AS Norma,
                               ////        MAX(CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END) AS Salariul,
                               ////        MAX(0) AS Spor,
                               ////        MAX(CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END) AS Structura,
                               ////        MAX(CASE WHEN COALESCE(DurataContract, 0) = 2 THEN 1 ELSE 0 END) AS CIMDet,
                               ////        MAX(CASE WHEN COALESCE(DurataContract, 0) = 1 THEN 1 ELSE 0 END) AS CIMNed,
                               ////        MAX(CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END) AS Motiv,
                               ////        J.DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                               ////        J.IdAuto AS IdAutoAct, 
                               ////        CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas,
                               ////        (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                               ////        FROM Avs_Cereri AA
                               ////        LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                               ////        LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                               ////        WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,-99)=COALESCE(J.DocData,-99)
                               ////        GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                               ////        FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993
                               ////        FROM Avs_Cereri A
                               ////        LEFT JOIN F100 B ON A.F10003 = B.F10003
                               ////        LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                               ////        WHERE A.IdStare = 3
                               ////        GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993
                               ////        UNION
                               ////        SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                               ////        0, 0, 0, 0, 0, 0, 0, 0, 0, 
                               ////        A.F100985, A.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                               ////        J.IdAuto AS IdAutoAct,
                               ////        CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                               ////        A.F10022, A.F100993
                               ////        FROM F100 A
                               ////        LEFT JOIN Admin_NrActAd J ON A.F10003=J.F10003
                               ////        WHERE A.F10025 = 900) X
                               ////        ) AS Y
                               ////        WHERE 1=1 " + filtru, null);

                               //if (Constante.tipBD == 1)
                               //{

                               //    #region SQL

                               //    strSql = $@"
                               //            SELECT * FROM (
                               //            SELECT {cmp} AS ""Cheie"", X.*,
                               //            (SELECT MIN(""ColData"") FROM (
                               //            SELECT CASE WHEN ""Candidat"" = 1 THEN 
                               //            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,F10022) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               //            ELSE '2100-01-01' END AS ColData 
                               //            UNION
                               //            SELECT CASE WHEN Motiv = 1 THEN X.DataModif ELSE '2100-01-01' END AS ColData  
                               //            UNION
                               //            SELECT CASE WHEN Salariul = 1 THEN 
                               //            (SELECT Zi FROM (
                               //            SELECT Zi, CONVERT(int,ROW_NUMBER() OVER (ORDER BY (SELECT 1))) as IdAuto 
                               //            FROM tblZile WHERE Zi>=DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays)) x
                               //            WHERE IdAuto=20)
                               //            ELSE '2100-01-01' END AS ColData 
                               //            UNION
                               //            SELECT CASE WHEN CORCod=1 OR FunctieId = 1 OR CIMDet=1 OR CIMNed=1 THEN 
                               //            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<DataModif AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               //            ELSE '2100-01-01' END AS ColData 
                               //            UNION
                               //            SELECT CASE WHEN ""Norma"" = 1 THEN 
                               //            (SELECT TOP 1 Zi FROM tblZile WHERE Zi<=DATEADD(d,-1,X.DataModif) AND ZiSapt<=5 AND Zi NOT IN (SELECT day FROM Holidays) ORDER BY Zi Desc)
                               //            ELSE '2100-01-01' END AS ColData 
                               //            ) x) AS TermenDepasire
                               //            FROM (
                               //            SELECT A.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, A.DataModif, 0 AS Candidat,
                               //            MAX(CASE WHEN COALESCE(CORCod, 0) > 0 THEN 1 ELSE 0 END) AS CORCod,
                               //            MAX(CASE WHEN COALESCE(FunctieId, 0) > 0 THEN 1 ELSE 0 END) AS FunctieId,
                               //            MAX(CASE WHEN COALESCE(Norma, 0) > 0 THEN 1 ELSE 0 END) AS Norma,
                               //            MAX(CASE WHEN COALESCE(SalariulBrut, 0) > 0 OR COALESCE(SalariulNet, 0) > 0 THEN 1 ELSE 0 END) AS Salariul,
                               //            MAX(0) AS Spor,
                               //            MAX(CASE WHEN COALESCE(SubcompanieId, 0) > 0 OR COALESCE(FilialaId, 0) > 0 OR COALESCE(SectieId, 0) > 0 OR COALESCE(DeptId, 0) > 0 THEN 1 ELSE 0 END) AS Structura,
                               //            MAX(CASE WHEN COALESCE(DurataContract, 0) = 2 THEN 1 ELSE 0 END) AS CIMDet,
                               //            MAX(CASE WHEN COALESCE(DurataContract, 0) = 1 THEN 1 ELSE 0 END) AS CIMNed,
                               //            MAX(CASE WHEN COALESCE(MotivId, 0) > 0 THEN 1 ELSE 0 END) AS Motiv,
                               //            J.DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                               //            J.IdAuto AS IdAutoAct, 
                               //            CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas,
                               //            (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                               //            FROM Avs_Cereri AA
                               //            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                               //            LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                               //            WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,'1900-01-01')=COALESCE(J.DocData,'1900-01-01')
                               //            GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                               //            FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993
                               //            FROM Avs_Cereri A
                               //            LEFT JOIN F100 B ON A.F10003 = B.F10003
                               //            LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                               //            WHERE A.IdStare = 3
                               //            GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993, J.Candidat
                               //            UNION
                               //            SELECT A.F10003, COALESCE(A.F10008, '') + ' ' + COALESCE(A.F10009, '') AS NumeComplet, A.F10022, 1 AS Candidat,
                               //            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                               //            A.F100985, A.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                               //            J.IdAuto AS IdAutoAct,
                               //            CASE WHEN (SELECT COUNT(*) FROM tblFisiere FIS WHERE FIS.Tabela='Admin_NrActAd' AND FIS.Id=J.IdAuto AND FIS.EsteCerere=0) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                               //            A.F10022, A.F100993
                               //            FROM F100 A
                               //            LEFT JOIN Admin_NrActAd J ON A.F10003=J.F10003
                               //            WHERE A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) X
                               //            ) AS Y
                               //            WHERE 1=1 " + filtru;

                               //    #endregion
                               //}
                               //else
                               //{

                               //    #region Orcl

                               //    strSql = $@"
                               //            SELECT * FROM (
                               //            SELECT {cmp} AS ""Cheie"", X.*,
                               //            CASE WHEN ""Candidat"" = 1 then 
                               //            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= (F10022 - 1) AND ""ZiSapt"" <= 5)
                               //            when ""Motiv"" = 1 then X.""DataModif""
                               //            WHEN ""Norma""=1 THEN
                               //            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= (X.""DataModif"" - 1) AND ""ZiSapt"" <= 5)
                               //            when ""Salariul"" = 1 then
                               //            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= x.""DataModif"" + 19 AND ""ZiSapt"" <= 5)
                               //            WHEN ""CORCod"" = 1 OR ""FunctieId"" = 1 OR ""CIMDet"" = 1 OR ""CIMNed"" = 1 THEN
                               //            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= x.""DataModif"" - 1 AND ""ZiSapt"" <= 5)
                               //            ELSE TO_DATE('01-JAN-2100', 'DD-MM-YYYY') END AS ""TermenDepasire""
                               //            FROM(
                               //            SELECT A.F10003, COALESCE(B.F10008, '') || ' ' || COALESCE(B.F10009, '') AS ""NumeComplet"", A.""DataModif"", 0 AS ""Candidat"",
                               //            MAX(CASE WHEN COALESCE(""CORCod"", 0) > 0 THEN 1 ELSE 0 END) AS ""CORCod"",
                               //            MAX(CASE WHEN COALESCE(""FunctieId"", 0) > 0 THEN 1 ELSE 0 END) AS ""FunctieId"",
                               //            MAX(CASE WHEN COALESCE(""Norma"", 0) > 0 THEN 1 ELSE 0 END) AS ""Norma"",
                               //            MAX(CASE WHEN COALESCE(""SalariulBrut"", 0) > 0 OR COALESCE(""SalariulNet"", 0) > 0 THEN 1 ELSE 0 END) AS ""Salariul"",
                               //            MAX(0) AS ""Spor"",
                               //            MAX(CASE WHEN COALESCE(""SubcompanieId"", 0) > 0 OR COALESCE(""FilialaId"", 0) > 0 OR COALESCE(""SectieId"", 0) > 0 OR COALESCE(""DeptId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Structura"",
                               //            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 2 THEN 1 ELSE 0 END) AS ""CIMDet"",
                               //            MAX(CASE WHEN COALESCE(""DurataContract"", 0) = 1 THEN 1 ELSE 0 END) AS ""CIMNed"",
                               //            MAX(CASE WHEN COALESCE(""MotivId"", 0) > 0 THEN 1 ELSE 0 END) AS ""Motiv"",
                               //            CAST(J.""DocNr"" AS varchar2(20)) AS ""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                               //            J.""IdAuto"" AS ""IdAutoAct"", 
                               //            CASE WHEN (SELECT COUNT(*) FROM ""tblFisiere"" FIS WHERE FIS.""Tabela""='Admin_NrActAd' AND FIS.""Id""=J.""IdAuto"" AND FIS.""EsteCerere""=0) = 0 THEN 0 ELSE 1 END AS ""AreAtas"",
                               //            (SELECT ',' || LISTAGG(AA.""Id"", ',') WITHIN GROUP (ORDER BY AA.""Id"") AS ""Id""
                               //            FROM ""Avs_Cereri"" AA
                               //            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                               //            LEFT JOIN ""Admin_NrActAd"" JJ ON AA.""IdActAd""=JJ.""IdAuto""
                               //            WHERE AA.""IdStare"" = 3 AND AA.F10003=A.F10003 AND AA.""DataModif""=A.""DataModif"" AND COALESCE(JJ.""DocNr"",-99)=COALESCE(J.""DocNr"",-99) 
                               //            AND NVL(JJ.""DocData"",'01-01-2000') = NVL(J.""DocData"",'01-01-2000')
                               //            ) AS ""IdAvans"", B.F10022, B.F100993
                               //            FROM ""Avs_Cereri"" A
                               //            LEFT JOIN F100 B ON A.F10003 = B.F10003
                               //            LEFT JOIN ""Admin_NrActAd"" J ON A.""IdActAd""=J.""IdAuto""
                               //            WHERE A.""IdStare"" = 3
                               //            GROUP BY A.F10003, B.F10008, B.F10009, A.""DataModif"", J.""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0), COALESCE(J.""Semnat"",0), COALESCE(J.""Revisal"",0), J.""IdAuto"", B.F10022, B.F100993, J.""Candidat""
                               //            UNION
                               //            SELECT A.F10003, COALESCE(A.F10008, '') || ' ' || COALESCE(A.F10009, '') AS ""NumeComplet"", A.F10022, 1 AS ""Candidat"",
                               //            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                               //            A.F100985, A.F100986, COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                               //            J.""IdAuto"" AS ""IdAutoAct"",
                               //            CASE WHEN (SELECT COUNT(*) FROM ""tblFisiere"" FIS WHERE FIS.""Tabela""='Admin_NrActAd' AND FIS.""Id""=J.""IdAuto"" AND FIS.""EsteCerere""=0) = 0 THEN 0 ELSE 1 END AS ""AreAtas"", ',-1' AS ""IdAvans"",
                               //            A.F10022, A.F100993
                               //            FROM F100 A
                               //            LEFT JOIN ""Admin_NrActAd"" J ON A.F10003=J.F10003
                               //            WHERE A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) X
                               //            ) 
                               //            WHERE 1=1 " + filtru;


                               //            //GROUP BY AA.""Id"", AA.F10003, BB.F10008, BB.F10009, AA.""DataModif"", JJ.""DocNr"", JJ.""DocData"", COALESCE(JJ.""Tiparit"", 0), COALESCE(JJ.""Semnat"", 0), COALESCE(JJ.""Revisal"", 0), JJ.""IdAuto""

                               //    #endregion
                               //}

                               #endregion


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
                            SELECT CASE WHEN (Salariul = 1 OR Spor = 1) THEN 
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
                            SELECT CASE WHEN ""Norma"" = 1 THEN 
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
                            CONVERT(nvarchar(10),J.DocNr) AS DocNr, J.DocData, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct, 
                            CASE WHEN (SELECT COUNT(*) FROM Atasamente FIS WHERE FIS.IdAuto=J.IdAutoAtasamente) = 0 THEN 0 ELSE 1 END AS AreAtas,
                            (SELECT ',' + CONVERT(nvarchar(20),COALESCE(AA.Id, '')) 
                            FROM Avs_Cereri AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN Admin_NrActAd JJ ON AA.IdActAd=JJ.IdAuto
                            WHERE AA.IdStare = 3 AND AA.F10003=A.F10003 AND AA.DataModif=A.DataModif AND COALESCE(JJ.DocNr,-99)=COALESCE(J.DocNr,-99) AND COALESCE(JJ.DocData,'1900-01-01')=COALESCE(J.DocData,'1900-01-01')
                            GROUP BY AA.Id, AA.F10003, BB.F10008, BB.F10009, AA.DataModif, JJ.DocNr, JJ.DocData, COALESCE(JJ.Tiparit,0), COALESCE(JJ.Semnat,0), COALESCE(JJ.Revisal,0), JJ.IdAuto
                            FOR XML PATH ('')) AS IdAvans, B.F10022, B.F100993, J.IdAutoAtasamente,
                            0 AS CandidatAngajat
                            FROM Avs_Cereri A
                            INNER JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN Admin_NrActAd J ON A.IdActAd=J.IdAuto
                            WHERE A.IdStare = 3 AND A.DataModif >= '2019-01-01' {companie} {filtruSup}
                            GROUP BY A.F10003, B.F10008, B.F10009, A.DataModif, J.DocNr, J.DocData, COALESCE(J.Tiparit,0), COALESCE(J.Semnat,0), COALESCE(J.Revisal,0), J.IdAuto, B.F10022, B.F100993, J.Candidat, J.IdAutoAtasamente, J.Revisal, B.F10025
                            UNION
                            SELECT B.F10003, COALESCE(B.F10008, '') + ' ' + COALESCE(B.F10009, '') AS NumeComplet, B.F10022, 1 AS Candidat,
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                            CONVERT(nvarchar(10),B.F100985) AS DocNr, B.F100986, COALESCE(J.Tiparit,0) AS Tiparit, COALESCE(J.Semnat,0) AS Semnat, COALESCE(J.Revisal,0) AS Revisal,
                            J.IdAuto AS IdAutoAct,
                            CASE WHEN (SELECT COUNT(*) FROM Atasamente FIS WHERE FIS.IdAuto=J.IdAutoAtasamente) = 0 THEN 0 ELSE 1 END AS AreAtas, ',-1' AS IdAvans,
                            B.F10022, B.F100993, J.IdAutoAtasamente,
                            CASE WHEN (COALESCE(B.F10025,-99) IN (0,999) AND COALESCE(J.Revisal,0) = 1) THEN 1 ELSE 0 END AS CandidatAngajat
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
                            WHEN ""Norma""=1 THEN
                            (SELECT max(""Zi"") FROM ""tblZile"" join holidays on ""tblZile"".""Zi"" = holidays.day  WHERE ""Zi"" <= (X.""DataModif"" - 1) AND ""ZiSapt"" <= 5)
                            when ""Salariul"" = 1 OR ""Spor"" = 1 then
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
                            CAST(J.""DocNr"" AS varchar2(20)) AS ""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"", 
                            CASE WHEN (SELECT COUNT(*) FROM ""Atasamente"" FIS WHERE FIS.""IdAuto""=J.""IdAutoAtasamente"") = 0 THEN 0 ELSE 1 END AS ""AreAtas"",
                            (SELECT ',' || LISTAGG(AA.""Id"", ',') WITHIN GROUP (ORDER BY AA.""Id"") AS ""Id""
                            FROM ""Avs_Cereri"" AA
                            LEFT JOIN F100 BB ON AA.F10003 = BB.F10003
                            LEFT JOIN ""Admin_NrActAd"" JJ ON AA.""IdActAd""=JJ.""IdAuto""
                            WHERE AA.""IdStare"" = 3 AND AA.F10003=A.F10003 AND AA.""DataModif""=A.""DataModif"" AND COALESCE(JJ.""DocNr"",-99)=COALESCE(J.""DocNr"",-99) 
                            AND NVL(JJ.""DocData"",'01-01-2000') = NVL(J.""DocData"",'01-01-2000')
                            ) AS ""IdAvans"", B.F10022, B.F100993, J.""IdAutoAtasamente""
                            FROM ""Avs_Cereri"" A
                            INNER JOIN F100 B ON A.F10003 = B.F10003
                            LEFT JOIN ""Admin_NrActAd"" J ON A.""IdActAd""=J.""IdAuto""
                            WHERE A.""IdStare"" = 3 AND A.""DataModif"" >= TO_DATE('01-01-2019', 'DD-MM-YYYY') {companie}
                            GROUP BY A.F10003, B.F10008, B.F10009, A.""DataModif"", J.""DocNr"", J.""DocData"", COALESCE(J.""Tiparit"",0), COALESCE(J.""Semnat"",0), COALESCE(J.""Revisal"",0), J.""IdAuto"", B.F10022, B.F100993, J.""Candidat"", J.""IdAutoAtasamente""
                            UNION
                            SELECT A.F10003, COALESCE(A.F10008, '') || ' ' || COALESCE(A.F10009, '') AS ""NumeComplet"", A.F10022, 1 AS ""Candidat"",
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 
                            A.F100985, A.F100986, COALESCE(J.""Tiparit"",0) AS ""Tiparit"", COALESCE(J.""Semnat"",0) AS ""Semnat"", COALESCE(J.""Revisal"",0) AS ""Revisal"",
                            J.""IdAuto"" AS ""IdAutoAct"",
                            CASE WHEN (SELECT COUNT(*) FROM ""Atasamente"" FIS WHERE FIS.""IdAuto""=J.""IdAutoAtasamente"") = 0 THEN 0 ELSE 1 END AS ""AreAtas"", ',-1' AS ""IdAvans"",
                            A.F10022, A.F100993, J.""IdAutoAtasamente""
                            FROM F100 A
                            LEFT JOIN ""Admin_NrActAd"" J ON A.F10003=J.F10003
                            WHERE (A.F10025 = 900 OR COALESCE(J.""Candidat"",0) = 1) {companie}) X
                            ) 
                            WHERE 1=1 " + filtru;


                    //GROUP BY AA.""Id"", AA.F10003, BB.F10008, BB.F10009, AA.""DataModif"", JJ.""DocNr"", JJ.""DocData"", COALESCE(JJ.""Tiparit"", 0), COALESCE(JJ.""Semnat"", 0), COALESCE(JJ.""Revisal"", 0), JJ.""IdAuto""

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

                    //if (arr[0] == "cmbTip")
                    //{
                    //    string sursa = "";

                    //    switch (General.Nz(cmbTip.Value, "").ToString())
                    //    {
                    //        case "0":
                    //            sursa = $@" SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                    //            FROM ""Avs_Cereri"" A
                    //            LEFT JOIN F100 B ON A.F10003=B.F10003
                    //            WHERE ""IdStare""=3";
                    //            break;
                    //        case "1":
                    //            sursa = $@"SELECT A.F10003, COALESCE(A.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(A.F10009,'') AS ""NumeComplet"" 
                    //            FROM F100 A
                    //            WHERE A.F10025=900";
                    //            break;
                    //        default:
                    //            sursa = $@"SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                    //            FROM ""Avs_Cereri"" A
                    //            LEFT JOIN F100 B ON A.F10003=B.F10003
                    //            WHERE ""IdStare""=3
                    //            UNION
                    //            SELECT A.F10003, COALESCE(A.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(A.F10009,'') AS ""NumeComplet"" 
                    //            FROM F100 A
                    //            WHERE A.F10025=900";
                    //            break;
                    //    }

                    //    string strSql = $@"SELECT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                    //            FROM ({sursa}) X
                    //            LEFT JOIN F100 A ON X.F10003=A.F10003
                    //            LEFT JOIN F004 G ON A.F10005 = G.F00405
                    //            LEFT JOIN F005 H ON A.F10006 = H.F00506
                    //            LEFT JOIN F006 I ON A.F10007 = I.F00607";

                    //    DataTable dtAng = General.IncarcaDT(strSql, null);
                    //    cmbAng.DataSource = dtAng;
                    //    cmbAng.DataBind();
                    //    return;
                    //}

                    List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "IdAvans", "Tiparit", "Semnat", "Revisal", "NumeComplet", "DocData", "Candidat", "TermenDepasire", "Motiv" });
                    if (General.Nz(arr[0], "").ToString() != "btnDocUpload" && General.Nz(arr[0], "").ToString() != "btnSterge")
                    {
                        if (lst == null || lst.Count() == 0 || lst[0] == null) return;
                    }

                    switch (arr[0])
                    {
                        case "btnDocUpload":
                            {
                                if (arr[1] == "")
                                {
                                    grDate.JSProperties["cpAlertMessage"] = "Nu se poate adauga atasament deoarece nu exista numar";
                                    return;
                                }
                                else
                                    msg = Dami.TraduCuvant("proces realizat cu succes");

                                //else
                                //    IncarcaGrid();
                            }
                            break;
                        case "btnSterge":
                            {
                                //string sqlFis = $@"BEGIN
                                //    DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                                //    UPDATE ""Admin_NrActAd"" SET ""Semnat""=0 WHERE ""IdAuto""=@2;
                                //    END;";


                                //General.ExecutaNonQuery($@"DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0", new object[] { "Admin_NrActAd", arr[1] });
                                General.ExecutaNonQuery($@"
                                    BEGIN
                                        DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
                                        DELETE FROM ""Atasamente"" WHERE ""IdAuto""=@2;
                                        UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=NULL WHERE ""IdAutoAtasamente""=@2;
                                    END;", new object[] { "Atasamente", arr[1] });

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
                                        if (Convert.ToInt32(General.Nz(obj[12], 0)) != 0)
                                        {
                                            msg += obj[8] + " - " + Dami.TraduCuvant("atribuirea de numar se face doar prin editare") + System.Environment.NewLine;
                                            continue;
                                        }


                                        if (General.Nz(obj[2], "").ToString() == "")
                                        {
                                            //Florin 2019.09.09
                                            //exista cazul in care candidatii nu au nr si data document setat in managementul de personal
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
                                                VALUES(@1, COALESCE((SELECT MAX(COALESCE(DocNr,0)) FROM Admin_NrActAd WHERE F10003=@1 AND COALESCE(Candidat,0)=0),0) + 1, {General.CurrentDate()},@2, @3, {General.CurrentDate()}, @4, @5);",
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
                                                    id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2 AND COALESCE(""Candidat"",0)=0),0) + 1, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[1]))}, @3, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[11]))}, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                    new object[] { "int", obj[0], Session["UserId"], obj[10] }), 0));

                                                    //id = General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    //VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2),0) + 1, {General.CurrentDate()}, TO_DATE(@3, 'DD-MM-YYYY'), @4, {General.CurrentDate()}, TO_DATE(@5, 'DD-MM-YYYY'), @6) RETURNING ""IdAuto"" INTO @out_1",
                                                    //new object[] { "int", obj[0], General.ToDataOrcl(obj[1]).ToUpper(), Session["UserId"], General.ToDataOrcl(obj[11]).ToUpper(), obj[10] });

                                                    //id = General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    //VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2),0) + 1, {General.CurrentDate()}, TO_DATE(@3, 'DD-MM-YYYY'), @4, {General.CurrentDate()}, TO_DATE(@5, 'DD-MM-YYYY'), @6) RETURNING ""IdAuto"" INTO @out_1",
                                                    //new object[] { "int", obj[0], General.ToDataOrcl(obj[1]), Session["UserId"], General.ToDataOrcl(obj[11]), obj[10] });

                                                    //id = General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    //VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2),0) + 1, {General.CurrentDate()},TO_DATE(@3,'DD-MM-YYYY'), @4, {General.CurrentDate()}, TO_DATE(@5,'DD-MM-YYYY'), @6) RETURNING ""IdAuto"" INTO @out_1;",
                                                    //new object[] { "int", obj[0], General.ToDataUniv(Convert.ToDateTime(obj[1])), Session["UserId"], General.ToDataUniv(Convert.ToDateTime(obj[11])), obj[10] });

                                                    //id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                    //VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2),0) + 1, {General.CurrentDate()},SYSDATE, @3, {General.CurrentDate()}, SYSDATE, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                    //new object[] { "int",  obj[0], Session["UserId"], obj[10] }),0));

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

                                //if (msg != "")
                                //{
                                //    grDate.JSProperties["cpAlertMessage"] = msg;
                                //    IncarcaGrid();

                                //    grDate.Selection.UnselectAll();
                                //}
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

                                        //string strSql = $@"IF (SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE ""IdAuto""=@1)=0
                                        //        INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"",""Tiparit"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                        //        VALUES(@4, @2, @3, 1, @5, @6, {General.CurrentDate()}, @7, @8)
                                        //        ELSE
                                        //        UPDATE ""Admin_NrActAd"" SET ""Tiparit""=1 WHERE ""IdAuto""=@1";

                                        ////List<object> lst = grDate.GetSelectedFieldValues(new string[] { "F10003", "DataModif", "DocNr", "IdAutoAct", "IdAvans", "Tiparit", "Semnat", "Revisal", "NumeComplet", "DocData" });
                                        ////DataTable dt = General.IncarcaDT($@"UPDATE ""Admin_NrActAd"" SET Tiparit=1 WHERE ""IdAuto""=@1", new object[] { General.Nz(obj[3],-99) });
                                        //DataTable dt = General.IncarcaDT(strSql, new object[] { General.Nz(obj[3], -99), obj[2], obj[9], obj[0], obj[1], Session["UserId"], obj[11], obj[10] });

                                        //daca este candidat inainte trebuie sa salvam in baza de date
                                        if (General.Nz(obj[10],"0").ToString() == "1")
                                        {
                                            int id = -99;

                                            if (Constante.tipBD == 1)
                                            {
                                                DataTable dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                OUTPUT Inserted.IdAuto
                                                VALUES(@1, 0, @6, @2, @3, {General.CurrentDate()}, @4, @5);",
                                                new object[] { obj[0], obj[1], Session["UserId"], obj[11], obj[10], obj[1] });

                                                //DataTable dt = General.IncarcaDT($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                //OUTPUT Inserted.IdAuto
                                                //VALUES(@1, COALESCE((SELECT MAX(COALESCE(DocNr,0)) FROM Admin_NrActAd WHERE F10003=@1),0) + 1, {General.CurrentDate()},@2, @3, {General.CurrentDate()}, @4, @5);",
                                                //new object[] { obj[0], obj[1], Session["UserId"], obj[11], obj[10] });

                                                if (dt.Rows.Count > 0)
                                                    id = Convert.ToInt32(General.Nz(dt.Rows[0][0], -99));
                                            }
                                            else
                                            {
                                                id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                VALUES(@2, 0, @6, {General.ToDataUniv(Convert.ToDateTime(obj[1]))}, @3, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[11]))}, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                new object[] { "int", obj[0], Session["UserId"], obj[10], obj[1] }), 0));

                                                //id = Convert.ToInt32(General.Nz(General.DamiOracleScalar($@"INSERT INTO ""Admin_NrActAd""(F10003, ""DocNr"", ""DocData"", ""DataModificare"", USER_NO, TIME, ""TermenDepasireRevisal"", ""Candidat"") 
                                                //VALUES(@2, COALESCE((SELECT MAX(COALESCE(""DocNr"",0)) FROM ""Admin_NrActAd"" WHERE F10003=@2),0) + 1, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[1]))}, @3, {General.CurrentDate()}, {General.ToDataUniv(Convert.ToDateTime(obj[11]))}, @4) RETURNING ""IdAuto"" INTO @out_1",
                                                //new object[] { "int", obj[0], Session["UserId"], obj[10] }), 0));
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

                                //grDate.JSProperties["cpAlertMessage"] = msg;
                                //IncarcaGrid();

                                //grDate.Selection.UnselectAll();
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


                                        //Florin 2019.05.27

                                        //DataTable dt = General.IncarcaDT($@"UPDATE ""Admin_NrActAd"" SET ""Semnat""=1 WHERE ""IdAuto""=@1", new object[] { obj[3] });
                                        //string strSql = $@"UPDATE ""Admin_NrActAd"" SET ""Semnat""=1, ""Candidat""=0 WHERE ""IdAuto""=@1;";
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
                                                Cereri pag = new Cereri();
                                                pag.TrimiteInF704(Convert.ToInt32(General.Nz(dr["Id"], -99)));
                                                if (Convert.ToInt32(General.Nz(dr["IdAtribut"], -99)) == 2)
                                                    General.ModificaFunctieAngajat(Convert.ToInt32(dr["F10003"]), Convert.ToInt32(General.Nz(dr["FunctieId"], -99)), Convert.ToDateTime(dr["DataModif"]), new DateTime(2100, 1, 1));
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

                                        //Florin 2019.05.27
                                        //s-a mutat in butonul se Semnat

                                        //string strSql = $@"UPDATE ""Admin_NrActAd"" SET ""Revisal""=1 WHERE ""IdAuto""=@1;";

                                        ////cazul cand este candidat
                                        //if (Convert.ToInt32(General.Nz(obj[10], 0)) == 1)
                                        //{
                                        //    DateTime dtLucru = General.DamiDataLucru();
                                        //    if (Convert.ToDateTime(obj[1]).Year == dtLucru.Year && Convert.ToDateTime(obj[1]).Month == dtLucru.Month)
                                        //        strSql += $@"UPDATE F100 SET F10025=0 WHERE F10003=@2;";
                                        //    else
                                        //        strSql += $@"UPDATE F100 SET F10025=999 WHERE F10003=@2;";
                                        //}

                                        //General.ExecutaNonQuery("BEGIN " + strSql + " END;", new object[] { obj[3], obj[0], Session["UserId"] });

                                        //if (Dami.ValoareParam("FinalizareCuActeAditionale") == "1")
                                        //{
                                        //    DataTable dtAvs = General.IncarcaDT($@"SELECT * FROM ""Avs_Cereri"" WHERE ""IdActAd""=@1", new object[] { obj[3] });
                                        //    for (int x = 0; x < dtAvs.Rows.Count; x++)
                                        //    {
                                        //        //cazul cand este angajat
                                        //        DataRow dr = dtAvs.Rows[x];
                                        //        Cereri pag = new Cereri();
                                        //        pag.TrimiteInF704(Convert.ToInt32(General.Nz(dr["Id"], -99)));
                                        //        if (Convert.ToInt32(General.Nz(dr["IdAtribut"], -99)) == 2)
                                        //            General.ModificaFunctieAngajat(Convert.ToInt32(dr["F10003"]), Convert.ToInt32(General.Nz(dr["FunctieId"], -99)), Convert.ToDateTime(dr["DataModif"]), new DateTime(2100, 1, 1));
                                        //    }
                                        //}
                                        msg += obj[8] + " - " + Dami.TraduCuvant("proces realizat cu succes") + System.Environment.NewLine;

                                    }
                                    catch (Exception ex)
                                    {
                                        string ert = ex.Message;
                                    }
                                }
                            }
                            break;
                    }

                    grDate.JSProperties["cpAlertMessage"] = msg;
                    IncarcaGrid();

                    grDate.Selection.UnselectAll();

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
                //var id = e.Keys["Id"];
                int id = Convert.ToInt32(e.Keys["Id"]);

                object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "F10003", "DataModif", "Revisal", "TermenDepasire", "IdAvans", "Semnat" }) as object[];

                if (Convert.ToInt32(General.Nz(obj[3], 0)) != 0 || Convert.ToInt32(General.Nz(obj[6], 0)) != 0)
                {
                    grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("In acest stadiu nu mai sunt permise modificari");
                    //grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu se poate modifica. Informatie trimisa in Revisal");
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

                    int cnt = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COUNT(*) FROM ""Admin_NrActAd"" WHERE ""DocNr""=@1 AND F10003=@2 AND ""IdAuto""<>@3", new object[] { docNr, General.Nz(obj[1],-99), General.Nz(obj[0],-99) }),0));
                    if (cnt > 0)
                        grDate.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numarul de document exista deja");

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
                        btnUpload.ClientSideEvents.FilesUploadStart = string.Format("function(s,e) {{ ValidareUpload(s,'{0}'); }}", General.Nz(e.GetValue("IdAutoAct"), -99));
                        btnUpload.ClientSideEvents.FileUploadComplete = string.Format("function(s,e) {{ grDate.PerformCallback('btnDocUpload;{0}'); }}", e.GetValue("IdAutoAct"));
                    }

                    GridViewDataColumn colSterge = grDate.Columns["colAtas"].Columns["colSterge"] as GridViewDataColumn;
                    ASPxButton btnSterge = grDate.FindRowCellTemplateControl(e.VisibleIndex, colSterge, "btnSterge") as ASPxButton;
                    if (btnSterge != null)
                        btnSterge.ClientSideEvents.Click = string.Format("function(s,e) {{ grDate.PerformCallback('btnSterge;{0}'); }}", e.GetValue("IdAutoAtasamente"));

                    GridViewDataColumn colArata = grDate.Columns["colAtas"].Columns["colArata"] as GridViewDataColumn;
                    ASPxButton btnArata = grDate.FindRowCellTemplateControl(e.VisibleIndex, colArata, "btnArata") as ASPxButton;
                    if (btnArata != null)
                        btnArata.ClientSideEvents.Click = string.Format("function(s,e) {{ GoToAtasMode({0}); }}", e.GetValue("IdAutoAtasamente"));
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

                #region cazul in care se salveaza si in tblFisiere

                //if (General.Nz(obj[3], "").ToString() == "")
                //{
                //    if (Constante.tipBD == 1)
                //        strSql = $@"
                //            BEGIN
                //                DECLARE @IdAuto TABLE (IdAuto int);

                //                INSERT INTO ""Atasamente""(""IdEmpl"", ""IdCategory"", ""DateAttach"", ""Attach"", ""DescrAttach"", USER_NO, TIME) 
                //                OUTPUT inserted.IdAuto INTO @IdAuto
                //                VALUES( @8, 999, {General.CurrentDate()}, @3, @4, @6, {General.CurrentDate()});

                //                INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                //                SELECT @1, (SELECT IdAuto FROM @IdAuto), 0, @3, @4, @5, @6, {General.CurrentDate()};

                //                UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=(SELECT IdAuto FROM @IdAuto) WHERE ""IdAuto""=@2;
                //            END;";
                //    else
                //        strSql = $@"
                //            BEGIN
                //                DECLARE param_IdAuto number;

                //                INSERT INTO ""Atasamente""(""IdEmpl"", ""IdCategory"", ""DateAttach"", ""Attach"", ""DescrAttach"", USER_NO, TIME) 
                //                VALUES( @8, 999, {General.CurrentDate()}, @3, @4, @6, {General.CurrentDate()})
                //                RETURNING ""IdAuto"" INTO param_IdAuto;

                //                INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                //                SELECT @1, param_IdAuto, 0, @3, @4, @5, @6, {General.CurrentDate()};

                //                UPDATE ""Admin_NrActAd"" SET ""IdAutoAtasamente""=param_IdAuto WHERE ""IdAuto""=@2;
                //            END;";
                //}
                //else
                //{
                //    strSql = $@"
                //        BEGIN
                //            UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@6 AND ""EsteCerere""=0;
                //            UPDATE ""Atasamente"" SET ""Attach""=@3, ""DescrAttach""=@4 WHERE ""IdAuto""=@6;
                //        END;";
                //}

                #endregion


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

        //protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        //{
        //    try
        //    {
        //        object[] obj = grDate.GetRowValues(grDate.FocusedRowIndex, new string[] { "IdAutoAct", "Semnat" }) as object[];

        //        if (Convert.ToInt32(General.Nz(obj[0], -99)) == -99) return;

        //        //string sqlFis = $@"
        //        //            BEGIN
        //        //            if ((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0) = 0)
        //        //                BEGIN                                
        //        //                    INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //        //                    SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")}
        //        //                    UPDATE ""Admin_NrActAd"" SET ""Semnat""=1 WHERE ""IdAuto""=@2;
        //        //                END
        //        //            ELSE
        //        //                UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
        //        //            END;";

        //        string sqlFis = $@"
        //                    BEGIN
        //                    if ((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0) = 0)                               
        //                        INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //                        SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")}
        //                    ELSE
        //                        UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
        //                    END;";

        //        if (Constante.tipBD == 2)
        //            sqlFis = $@"
        //                BEGIN
        //                    INSERT INTO ""tblFisiere""(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //                    SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {(Constante.tipBD == 1 ? "" : " FROM DUAL")};
        //                EXCEPTION
        //                    WHEN DUP_VAL_ON_INDEX THEN
        //                    UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2 AND ""EsteCerere""=0;
        //                END; ";

        //        General.ExecutaNonQuery(sqlFis, new object[] { "Admin_NrActAd", obj[0], e.UploadedFile.FileBytes, e.UploadedFile.FileName, e.UploadedFile.ContentType, Session["UserId"] });
        //        //General.ExecutaNonQuery(sqlFis, new object[] { "Admin_NrActAd", obj[0], null, e.UploadedFile.FileName, e.UploadedFile.ContentType, Session["UserId"] });

        //        //btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
        //        //if (Convert.ToInt32(General.Nz(obj[0],-99)) == -99)
        //        //    grDate.JSProperties["cpAlertMessage"] = "Nu se poate adauga atasament deoarece nu exista numar";

        //        //IncarcaGrid();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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


                if (General.Nz(obj[10], "").ToString() == "" || General.Nz(obj[11], "").ToString() == "")
                {
                    MessageBox.Show("Nu exista numar si data document", MessageBox.icoWarning, "Operatie anulata");
                    return;
                }


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
                        if (Convert.ToInt32(General.Nz(obj[2], 0)) == 1 || Convert.ToInt32(General.Nz(obj[3], 0)) == 1 || Convert.ToInt32(General.Nz(obj[4], 0)) == 1 || Convert.ToInt32(General.Nz(obj[5], 0)) == 1 || Convert.ToInt32(General.Nz(obj[6], 0)) == 1 || Convert.ToInt32(General.Nz(obj[7], 0)) == 1 || Convert.ToInt32(General.Nz(obj[8], 0)) == 1 || Convert.ToInt32(General.Nz(obj[9], 0)) == 1)
                        {
                            if (General.Nz(obj[12], "").ToString() == "")
                            {
                                MessageBox.Show("Nu exista data modificare", MessageBox.icoWarning, "Operatie anulata");
                                return;
                            }
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


                //Florin 2019.07.15
                #region Salvam Filtrul

                string req = "";
                if (cmbCmp.Value != null) req += "&Cmp=" + cmbCmp.Value;
                if (cmbTip.Value != null) req += "&Tip=" + cmbTip.Value;
                if (cmbAng.Value != null) req += "&Ang=" + cmbAng.Value;
                if (cmbStatus.Value != null) req += "&Status=" + cmbStatus.Value;
                if (txtData.Value != null) req += "&Data=" + txtData.Value;
                if (txtDepasire.Value != null) req += "&Depasire=" + txtDepasire.Value;

                Session["Filtru_ActeAditionale"] = req;

                #endregion



                Session["ReportId"] = Convert.ToInt32(idRap);
                string url = "../Generatoare/Reports/Pages/ReportView.aspx?Angajat=" + obj[0] + param;
                Response.Redirect(url);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbTip_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                string sursa = "";
                string companie = "";
                if (General.Nz(cmbCmp.Value, "").ToString() != "") companie = " AND B.F10002=" + cmbCmp.Value;

                switch (General.Nz(cmbTip.Value, "").ToString())
                {
                    case "0":
                        sursa = $@" SELECT A.F10003, COALESCE(B.F10008,'') {Dami.Operator()} ' ' {Dami.Operator()} COALESCE(B.F10009,'') AS ""NumeComplet""
                            FROM ""Avs_Cereri"" A
                            INNER JOIN F100 B ON A.F10003=B.F10003
                            WHERE ""IdStare""=3 {companie}";
                        break;
                    case "1":
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

                string strSql = $@"SELECT DISTINCT X.F10003, X.""NumeComplet"", G.F00406 AS ""Filiala"", H.F00507 AS ""Sectie"", I.F00608 AS ""Departament""
                            FROM ({sursa}) X
                            LEFT JOIN F100 A ON X.F10003=A.F10003
                            LEFT JOIN F004 G ON A.F10005 = G.F00405
                            LEFT JOIN F005 H ON A.F10006 = H.F00506
                            LEFT JOIN F006 I ON A.F10007 = I.F00607";

                DataTable dtAng = General.IncarcaDT(strSql, null);
                Session["Acte_Ang"] = dtAng;
                cmbAng.DataSource = dtAng;
                cmbAng.DataBind();
                cmbAng.SelectedIndex = -1;

                IncarcaGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void cmbComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cmbTip_ValueChanged(null, null);
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
        //            MessageBox.Show("Proces realizat cu succes", MessageBox.icoWarning, "");
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