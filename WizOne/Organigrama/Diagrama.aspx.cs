using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using WizOne.Module;

namespace WizOne.Organigrama
{
    public partial class Diagrama : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp();

                if (!IsPostBack)
                {
                    txtTitlu.Text = General.VarSession("Titlu").ToString();

                    #region Traducere
                    string ctlPost = Request.Params["__EVENTTARGET"];
                    if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                    btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");

                    lblDtVig.InnerText = Dami.TraduCuvant("Data Selectie");
                    lblParinte.InnerText = Dami.TraduCuvant("Superior");
                    lblPost.InnerText = Dami.TraduCuvant("Incepand de la");
                    lblLimbi.InnerText = Dami.TraduCuvant("Limba");
                    lblNivel.InnerText = Dami.TraduCuvant("Nr niveluri");
                    lblAfis.InnerText = Dami.TraduCuvant("Afisare ultimul nivel");
                    lblParinte.InnerText = Dami.TraduCuvant("Superior");

                    chkPlan.Text = Dami.TraduCuvant("Plan HC");
                    chkAprobat.Text = Dami.TraduCuvant("HC Aprobat");
                    chkEfectiv.Text = Dami.TraduCuvant("HC Efectiv");

                    btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                    #endregion

                    txtDtVig.Value = DateTime.Now;
                    txtNivel.Value = 3;

                    DataTable dtPar = General.IncarcaDT(@"SELECT ""Camp"", ""Denumire"" FROM ""Org_tblParinte"" ", null);
                    cmbParinte.DataSource = dtPar;
                    cmbParinte.DataBind();
                    if (dtPar.Rows.Count > 0)
                        cmbParinte.SelectedIndex = 0;

                    DataTable dtPost = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""IdSuperior"" FROM ""Org_Posturi"" WHERE {General.TruncateDate("DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("DataSfarsit")}");
                    cmbPost.DataSource = dtPost;
                    cmbPost.DataBind();
                    DataRow[] arr = dtPost.Select("IdSuperior=0");
                    if (arr.Length > 0)
                        cmbPost.Value = Convert.ToInt32(arr[0]["Id"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private string IncarcaGrid(DateTime dtRef, int nivel, int idPost)
        {
            string numeFis = "";

            try
            {
                string strSql = "";

                if (Constante.tipBD == 1)
                {
                    //strSql = $@"WITH tree AS  
                    //        (
                    //        SELECT Id, IdSuperior, NivelIerarhic, 1 as Level,
                    //        COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN,Denumire) AS DenumireEN, 
                    //        COALESCE(NumeGrupRO, Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN,Denumire) AS NumeGrupEN
                    //        FROM Org_Posturi as parent 
                    //        WHERE Id = {idPost} AND CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = {activ}
                    //        UNION ALL
                    //        SELECT child.Id, child.IdSuperior, child.NivelIerarhic, parent.Level + 1,
                    //        COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN,child.Denumire) AS DenumireEN,
                    //        COALESCE(child.NumeGrupRO, child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN,child.Denumire) AS NumeGrupEN
                    //        FROM Org_Posturi as child
                    //        JOIN tree parent on parent.Id = child.IdSuperior
                    //        WHERE CONVERT(DATE, child.DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = {activ}
                    //        )
                    //        SELECT Id, IdSuperior, NivelIerarhic, Level,
                    //        CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {("1" == "1" ? "DenumireRO" : "NumeGrupRO")} ELSE DenumireRO END AS DenumireRO,
                    //        CASE WHEN (Level = {nivel} AND (SELECT COUNT(*) FROM Org_Posturi X WHERE X.{cmbParinte.Value} = tree.Id) <> 0) THEN {("1" == "1" ? "DenumireEN" : "NumeGrupEN")} ELSE DenumireEN END AS DenumireEN,
                    //        dbo.[DamiHC](1, Id, {General.ToDataUniv(dtRef.Date)}) AS PlanHC, 
                    //        dbo.[DamiHC](2, Id, {General.ToDataUniv(dtRef.Date)}) AS HCAprobat, 
                    //        dbo.[DamiHC](3, Id, {General.ToDataUniv(dtRef.Date)}) AS HCEfectiv
                    //        FROM tree
                    //        where Level <= {nivel}";

                    string camp = "";
                    if (General.Nz(cmbAfisare.Value, 1).ToString() == "1")
                        camp += "Denumire" + General.Nz(cmbLimbi.Value, "RO").ToString();
                    else
                        camp += "NumeGrup" + General.Nz(cmbLimbi.Value, "RO").ToString();

                    if (chkPlan.Checked) camp += $" + ', Plan: ' + CONVERT(nvarchar(10),dbo.[DamiHC](1, Id, {General.ToDataUniv(dtRef.Date)}))";
                    if (chkAprobat.Checked) camp += $" + ', Aprobat: ' + CONVERT(nvarchar(10),dbo.[DamiHC](2, Id, {General.ToDataUniv(dtRef.Date)}))";
                    if (chkEfectiv.Checked) camp += $" + ', Efectiv: ' + CONVERT(nvarchar(10),dbo.[DamiHC](3, Id, {General.ToDataUniv(dtRef.Date)}))";

                    strSql = $@"WITH tree AS  
                            (
                            SELECT Id, IdSuperior, NivelIerarhic, 1 as Level,
                            COALESCE(DenumireRO, Denumire) AS DenumireRO, COALESCE(DenumireEN,Denumire) AS DenumireEN, 
                            COALESCE(NumeGrupRO, Denumire) AS NumeGrupRO, COALESCE(NumeGrupEN,Denumire) AS NumeGrupEN
                            FROM Org_Posturi as parent 
                            WHERE Id = {idPost} AND CONVERT(DATE, DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, DataSfarsit, 103) AND Stare = 1
                            UNION ALL
                            SELECT child.Id, child.IdSuperior, child.NivelIerarhic, parent.Level + 1,
                            COALESCE(child.DenumireRO, child.Denumire) AS DenumireRO, COALESCE(child.DenumireEN,child.Denumire) AS DenumireEN,
                            COALESCE(child.NumeGrupRO, child.Denumire) AS NumeGrupRO, COALESCE(child.NumeGrupEN,child.Denumire) AS NumeGrupEN
                            FROM Org_Posturi as child
                            JOIN tree parent on parent.Id = child.IdSuperior
                            WHERE CONVERT(DATE, child.DataInceput, 103)<=CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) AND CONVERT(DATE, {General.ToDataUniv(dtRef.Date)},103) <= CONVERT(DATE, child.DataSfarsit, 103) AND child.Stare = 1
                            )
                            SELECT Id, IdSuperior, NivelIerarhic, Level, {camp} AS Denumire
                            FROM tree
                            where Level <= {nivel}
                            ORDER BY Level, IdSuperior";
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
                                   " and \"Stare\" = 1";     //Radu 07.02.2017

                }

                DataTable dt = General.IncarcaDT(strSql, null);

                dgPost.NodeDataSource = dt;
                dgPost.DataBind();
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return numeFis;
        }

        protected void pnlCall_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            try
            {
                IncarcaGrid(Convert.ToDateTime(txtDtVig.Value), Convert.ToInt32(General.Nz(txtNivel.Value, 1)), Convert.ToInt32(General.Nz(cmbPost.Value, 1)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
    }
}