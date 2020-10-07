﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Data;
using System.IO;
using System.Diagnostics;
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.Internal;
using DevExpress.Spreadsheet;
using System.Web.Hosting;
using DevExpress.Web.ASPxDiagram;

namespace WizOne.Organigrama
{
    public partial class Diagrama : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                txtDtVig.Value = DateTime.Now;

                DataTable dtPar = General.IncarcaDT(@"SELECT ""Camp"", ""Denumire"" FROM ""Org_tblParinte"" ", null);
                cmbParinte.DataSource = dtPar;
                cmbParinte.DataBind();

                DataTable dtPost = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"" FROM ""Org_Posturi"" WHERE {General.TruncateDate("DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("DataSfarsit")}");
                cmbPost.DataSource = dtPost;
                cmbPost.DataBind();
                DataRow[] arr = dtPost.Select("IdSuperior=0");
                if (arr.Length > 0)
                    cmbPost.Value = arr[0];

                //DataTable dt = General.IncarcaDT(
                //    @"WITH tree AS  
                //    (
                //    SELECT Id, IdSuperior, NivelIerarhic, 1 as Level,
                //    COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN, Denumire) AS DenumireEN,
                //    COALESCE(NumeGrupRO, Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN, Denumire) AS NumeGrupEN
                //    FROM Org_Posturi as parent
                //    WHERE Id = 1 AND CONVERT(DATE, DataInceput, 103) <= CONVERT(DATE, CONVERT(date, '2020-10-06'), 103) AND CONVERT(DATE, CONVERT(date, '2020-10-06'), 103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = 1
                //    UNION ALL
                //    SELECT child.Id, child.IdSuperior, child.NivelIerarhic, parent.Level + 1,
                //    COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN, child.Denumire) AS DenumireEN,
                //    COALESCE(child.NumeGrupRO, child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN, child.Denumire) AS NumeGrupEN
                //    FROM Org_Posturi as child
                //    JOIN tree parent on parent.Id = child.IdSuperior
                //    WHERE CONVERT(DATE, child.DataInceput, 103) <= CONVERT(DATE, CONVERT(date, '2020-10-06'), 103) AND CONVERT(DATE, CONVERT(date, '2020-10-06'), 103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = 1
                //    )
                //    SELECT Id, IdSuperior, NivelIerarhic, Level,
                //    CASE WHEN(Level = 3 AND(SELECT COUNT(*) FROM Org_Posturi X WHERE X.IdSuperior = tree.Id) <> 0) THEN DenumireRO ELSE DenumireRO END AS DenumireRO,
                //    CASE WHEN(Level = 3 AND(SELECT COUNT(*) FROM Org_Posturi X WHERE X.IdSuperior = tree.Id) <> 0) THEN DenumireEN ELSE DenumireEN END AS DenumireEN,
                //    dbo.[DamiHC](1, Id, CONVERT(date, '2020-10-06')) AS PlanHC,
                //    dbo.[DamiHC](2, Id, CONVERT(date, '2020-10-06')) AS HCAprobat,
                //    dbo.[DamiHC](3, Id, CONVERT(date, '2020-10-06')) AS HCEfectiv
                //    FROM tree
                //    where Level <= 3");
                //ASPxDiagram diagram = new ASPxDiagram();
                //diagram.ID = "Diagrama123";
                //diagram.SettingsToolbox.Visibility = DiagramPanelVisibility.Collapsed;
                //diagram.Width = new Unit(100, UnitType.Percentage);
                //diagram.Height = new Unit(100, UnitType.Percentage);
                //diagram.SimpleView = true;
                //diagram.Mappings.Node.Key = "Id";
                //diagram.Mappings.Node.ParentKey = "IdSuperior";
                //diagram.Mappings.Node.Text = "DenumireRO";

                //List<Posturi> lst = new List<Posturi>();
                //for (int i = 0; i < dt.Rows.Count; i++)
                //{
                //    lst.Add(new Posturi { Id = Convert.ToInt32(dt.Rows[i]["Id"]), IdSuperior = Convert.ToInt32(dt.Rows[i]["IdSuperior"]), Denumire = Convert.ToString(dt.Rows[i]["DenumireRO"]) });
                //}

                ////diagram.NodeDataSource = lst;
                //pnlCont.Controls.Add(diagram);

                //dgPost.NodeDataSource = dt;
                //dgPost.DataBind();

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
                IncarcaGrid(Convert.ToDateTime(txtDtVig.Value), Convert.ToInt32(General.Nz(txtNivel.Value, 1)), 1, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private string IncarcaGrid(DateTime dtRef, int nivel, int idPost, int activ)
        {
            string numeFis = "";

            try
            {
                string strSql = "";

                if (Constante.tipBD == 1)
                {
                    strSql = $@"WITH tree AS  
                            (
                            SELECT Id, IdSuperior, NivelIerarhic, 1 as Level,
                            COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN,Denumire) AS DenumireEN, 
                            COALESCE(NumeGrupRO, Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN,Denumire) AS NumeGrupEN
                            FROM Org_Posturi as parent 
                            WHERE Id = {idPost} AND CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = {activ}
                            UNION ALL
                            SELECT child.Id, child.IdSuperior, child.NivelIerarhic, parent.Level + 1,
                            COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN,child.Denumire) AS DenumireEN,
                            COALESCE(child.NumeGrupRO, child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN,child.Denumire) AS NumeGrupEN
                            FROM Org_Posturi as child
                            JOIN tree parent on parent.Id = child.IdSuperior
                            WHERE CONVERT(DATE, child.DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = {activ}
                            )
                            SELECT Id, IdSuperior, NivelIerarhic, Level,
                            CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {("1" == "1" ? "DenumireRO" : "NumeGrupRO")} ELSE DenumireRO END AS DenumireRO,
                            CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {("1" == "1" ? "DenumireEN" : "NumeGrupEN")} ELSE DenumireEN END AS DenumireEN,
                            dbo.[DamiHC](1, Id, {General.ToDataUniv(dtRef.Date)}) AS PlanHC, 
                            dbo.[DamiHC](2, Id, {General.ToDataUniv(dtRef.Date)}) AS HCAprobat, 
                            dbo.[DamiHC](3, Id, {General.ToDataUniv(dtRef.Date)}) AS HCEfectiv
                            FROM tree
                            where Level <= {nivel}";
                }
                else
                {
                    string strRef = dtRef.Day.ToString().PadLeft(2, Convert.ToChar("0")) + "-" + General.NumeLuna(dtRef.Month, 1, "EN") + "-" + dtRef.Year.ToString();

                    strSql = "select \"Id\", \"Denumire\",\"IdSuperior\",\"NivelIerarhic\", Level, NVL(NR_ANG.NR, 0) as \"NrAngajati\"  from \"Org_Posturi\" " +

                                    //Radu 07.06.2017 - pentru a aduce nr. de angajati pe post
                                    " LEFT JOIN (select \"IdPost\", count(*)as NR from \"Org_relPostAngajat\" group by \"IdPost\") NR_ANG on NR_ANG.\"IdPost\"  = \"Org_Posturi\".\"Id\" " +

                                    " where level<=" + nivel +
                                    " and TRUNC(\"DataInceput\")<=to_date('" + strRef + "','DD-MM-YYYY') AND to_date('" + strRef + "','DD-MM-YYYY') <=TRUNC(\"DataSfarsit\") " +
                                    " start with \"Id\"=" + idPost + " connect by nocycle prior \"Id\" = \"IdSuperior\" " +      //Radu 26.05.2016 - am adaugat nocycle pentru a opri bucla infinita
                                   " and TRUNC(\"DataInceput\")<=to_date('" + strRef + "','DD-MM-YYYY') AND to_date('" + strRef + "','DD-MM-YYYY') <=TRUNC(\"DataSfarsit\") " + //Radu 09.01.2017 - conditie pentru a nu returna decat un post activ								
                                   " and \"Stare\" = " + activ;     //Radu 07.02.2017

                }

                DataTable dt = General.IncarcaDT(strSql, null);

                dgPost.NodeDataSource = dt;
                dgPost.DataBind();

                //int x = 0;

                //for(int i = 0; i < dt.Rows.Count; i++)
                //{
                //    x = i + 1;
                //    DataRow dr = dt.Rows[i];
                //    ws2.Cells["A" + x].Value = General.Nz(dr["Id"],"").ToString();
                //    if (General.Nz(dr["Id"], "").ToString() == idPost.ToString())
                //        ws2.Cells["B" + x].Value = 0;
                //    else
                //        ws2.Cells["B" + x].Value = General.Nz(dr["IdSuperior"], "").ToString();

                //    string den = General.Nz(dr["DenumireRO"], "").ToString();
                //    //string den = General.Nz(dr["Denumire"], "").ToString();
                //    //if (rbRO.Checked) den = General.Nz(dr["DenumireRO"], "").ToString();
                //    if (General.Nz(cmbLimbi.Value,"RO").ToString() == "EN") den = General.Nz(dr["DenumireEN"], "").ToString();

                //    string hc = "";
                //    if (chkPlan.Checked) hc += ",Plan: " + General.Nz(dr["PlanHC"],0);
                //    if (chkAprobat.Checked) hc += ",Aprobat: " + General.Nz(dr["HCAprobat"], 0);
                //    if (chkEfectiv.Checked) hc += ",Efectiv: " + General.Nz(dr["HCEfectiv"], 0);
                //    if (hc != "") hc = " (" + hc.Substring(1) + ")";

                //    ws2.Cells["C" + x].Value = den + hc;
                //}


            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return numeFis;
        }


    }
}