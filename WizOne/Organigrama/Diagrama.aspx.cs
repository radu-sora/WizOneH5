using System;
using System.Collections.Generic;
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

                    chkPlan.Text = Dami.TraduCuvant("Pozitii");
                    chkAprobat.Text = Dami.TraduCuvant("Pozitii Aprobate");
                    chkEfectiv.Text = Dami.TraduCuvant("Pozitii Efective");

                    btnFiltru.Text = Dami.TraduCuvant("btnFiltru", "Filtru");
                    #endregion

                    if (!IsPostBack)
                    {
                        txtNivel.Value = 3;

                        DataTable dtPar = General.IncarcaDT(@"SELECT ""Camp"", ""Denumire"" FROM ""Org_tblParinte"" ", null);
                        cmbParinte.DataSource = dtPar;
                        cmbParinte.DataBind();

                        DataTable dtPost = General.IncarcaDT($@"SELECT ""Id"", ""Denumire"", ""IdSuperior"" FROM ""Org_Posturi"" WHERE {General.TruncateDate("DataInceput")} <= {General.CurrentDate()} AND {General.CurrentDate()} <= {General.TruncateDate("DataSfarsit")}");
                        cmbPost.DataSource = dtPost;
                        cmbPost.DataBind();

                        if (Session["Filtru_Posturi"] == null)
                        {
                            txtDtVig.Value = DateTime.Now;
                            if (dtPar.Rows.Count > 0) cmbParinte.SelectedIndex = 0;
                            DataRow[] arr = dtPost.Select("IdSuperior=0");
                            if (arr.Length > 0) cmbPost.Value = Convert.ToInt32(arr[0]["Id"]);
                        }
                        else
                        {
                            Dictionary<string, object> dic = Session["Filtru_Posturi"] as Dictionary<string, object>;
                            if (dic != null)
                            {
                                txtDtVig.Value = (DateTime?)dic["Ziua"];
                                cmbParinte.Value = dic["Parinte"];
                                cmbPost.Value = (int?)dic["IdPost"];
                            }
                        }
                    }
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
                    string camp = "";
                    if (General.Nz(cmbAfisare.Value, 1).ToString() == "1")
                        camp += "Denumire" + General.Nz(cmbLimbi.Value, "RO").ToString();
                    else
                        camp += "NumeGrup" + General.Nz(cmbLimbi.Value, "RO").ToString();

                    if (chkPlan.Checked) camp += $" + ', Plan: ' + CONVERT(nvarchar(10),COALESCE(W.Pozitii,0))";
                    if (chkAprobat.Checked) camp += $" + ', Aprobat: ' + CONVERT(nvarchar(10),COALESCE(W.PozitiiAprobate,0))";
                    if (chkEfectiv.Checked) camp += $" + ', Efectiv: ' + CONVERT(nvarchar(10),COALESCE(W.Activi,0))";
                    if (chkAngajati.Checked) camp += $@" + ', ' + COALESCE(CASE WHEN H.Nr > 1 THEN CONVERT(nvarchar(500),H.Nr) + ' pers' ELSE CASE WHEN H.Nr = 1 THEN
                            (SELECT Y.F10008 + ' ' + Y.F10009
                            FROM Org_relPostAngajat X 
                            INNER JOIN F100 Y ON X.F10003=Y.F10003
                            OUTER APPLY DamiDataPlecare(X.F10003, {General.ToDataUniv(dtRef.Date)}) Z
                            WHERE X.DataInceput <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= X.DataSfarsit
                            AND (Y.F10022 <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= Z.DataPlecare) AND 
                            NOT (Y.F100922 <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= (CASE WHEN COALESCE(Y.F100924, '2100-01-01') = '2100-01-01' THEN Y.F100923 ELSE Y.F100924 END))
                            AND X.IdPost=A.Id) END END,'')";

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
                            FROM tree A
                            OUTER APPLY dbo.DamiPozitii(A.Id, {General.ToDataUniv(dtRef.Date)}) W
                            LEFT JOIN 
                            (SELECT X.IdPost, COUNT(*) AS Nr
                            FROM Org_relPostAngajat X 
                            INNER JOIN F100 Y ON X.F10003=Y.F10003
                            OUTER APPLY DamiDataPlecare(X.F10003, {General.ToDataUniv(dtRef.Date)}) Z
                            WHERE X.DataInceput <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= X.DataSfarsit
                            AND (Y.F10022 <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= Z.DataPlecare) AND 
                            NOT (Y.F100922 <= {General.ToDataUniv(dtRef.Date)} AND {General.ToDataUniv(dtRef.Date)} <= (CASE WHEN COALESCE(Y.F100924, '2100-01-01') = '2100-01-01' THEN Y.F100923 ELSE Y.F100924 END)) 
                            GROUP BY X.IdPost) H ON A.Id = H.IdPost
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