using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Globalization;
using DevExpress.Web.Data;

namespace WizOne.Organigrama
{
    public partial class Posturi : System.Web.UI.Page
    {

        int nrPost = 0;

        public class metaCereriDate
        {
            public object UploadedFile { get; set; }
            public object UploadedFileName { get; set; }
            public object UploadedFileExtension { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnPrint.Text = Dami.TraduCuvant("btnPrint", "Imprima");
                btnSterge.Text = Dami.TraduCuvant("btnSterge", "Sterge");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");
                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                
                btnDocUpload.ToolTip = Dami.TraduCuvant("incarca document");
                btnDocSterge.ToolTip = Dami.TraduCuvant("sterge document");
                btnDocArata.ToolTip = Dami.TraduCuvant("arata document");

                lblId.InnerText = Dami.TraduCuvant("Id");
                lblActiv.InnerText = Dami.TraduCuvant("Activ");
                lblBuget.InnerText = Dami.TraduCuvant("Cod buget");
                lblDen.InnerText = Dami.TraduCuvant("Denumire");
                lblDtInc.InnerText = Dami.TraduCuvant("Data Inceput");
                lblDtSf.InnerText = Dami.TraduCuvant("Data Sfarsit");
                lblDenRO.InnerText = Dami.TraduCuvant("Denumire post romana");
                lblDenEN.InnerText = Dami.TraduCuvant("Denumire post engleza");
                lblGrupRO.InnerText = Dami.TraduCuvant("Nume grup romana");
                lblGrupEN.InnerText = Dami.TraduCuvant("Nume grup engleza");
                lblCmp.InnerText = Dami.TraduCuvant("Companie");
                lblSub.InnerText = Dami.TraduCuvant("Subcomp");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectiune");
                lblDept.InnerText = Dami.TraduCuvant("Dept");
                lblSubdept.InnerText = Dami.TraduCuvant("Subdept");
                lblBirou.InnerText = Dami.TraduCuvant("Birou");
                lblSup.InnerText = Dami.TraduCuvant("Superior administrativ");
                lblSupFunc.InnerText = Dami.TraduCuvant("Superiro functional");
                lblFunc.InnerText = Dami.TraduCuvant("Functie");
                lblStudiiSup.InnerText = Dami.TraduCuvant("Studii Superioare");
                lblNivelHay.InnerText = Dami.TraduCuvant("Nivel Hay");
                lblSalMin.InnerText = Dami.TraduCuvant("Salariul min");
                lblSalMax.InnerText = Dami.TraduCuvant("max");
                lblNivelIer.InnerText = Dami.TraduCuvant("Nivel ierarhic");
                lblCor.InnerText = Dami.TraduCuvant("Cod COR");
                lblPozitii.InnerText = Dami.TraduCuvant("Nr pozitii");
                lblPozitiiAprobate.InnerText = Dami.TraduCuvant("Nr pozitii aprobate");
                lblCom.InnerText = Dami.TraduCuvant("Competente");

                pnlDosar.HeaderText = Dami.TraduCuvant("Alege documentele pentru dosarul personal");
                pnlEchip.HeaderText = Dami.TraduCuvant("Alege echipamentele");
                pnlBenef.HeaderText = Dami.TraduCuvant("Alege beneficiile");
                pnlExtra.HeaderText = Dami.TraduCuvant("Alege campurile aditionale");

                popUpIstoric.HeaderText = Dami.TraduCuvant("Istoric numar pozitii aprobate");

                foreach (var col in grDate.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                foreach (var col in grDateIstoric.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);

                #endregion

                txtTitlu.Text = General.VarSession("Titlu").ToString();

                DataTable dtCor = General.IncarcaDT($@"SELECT F72202, F72204 FROM F722 WHERE F72206 = (select max(f72206) from f722) ORDER BY F72204", null);
                cmbCor.DataSource = dtCor;
                cmbCor.DataBind();

                DataTable dtSup = PostSuperior(Convert.ToDateTime(Session["DataVigoare"]));
                nrPost = dtSup.Rows.Count;

                cmbSup.DataSource = dtSup;
                cmbSup.DataBind();

                cmbSupFunc.DataSource = cmbSup.DataSource;
                cmbSupFunc.DataBind();

                DataTable dtHay = General.IncarcaDT($@"SELECT * FROM ""Org_tblNivelHay"" ", null);
                cmbHay.DataSource = dtHay;
                cmbHay.DataBind();

                DataTable dtFunc = General.IncarcaDT($@"SELECT * FROM F718 ORDER BY F71804", null);
                cmbFunc.DataSource = dtFunc;
                cmbFunc.DataBind();

                DataRow dr = null;
                string campuriExtra = "";

                //daca este modificare incarcam valorile din baza de date
                if (!IsPostBack)
                {
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { General.Nz(Session["IdAuto"], "-97") });

                    if (dt.Rows.Count > 0)
                    {
                        dr = dt.Rows[0];
                        if (General.Nz(Session["Org_Duplicare"],0).ToString() == "1")
                            txtDen.Text = General.Nz(dr["Denumire"], "").ToString() + " - Copie";
                        else
                        {
                            txtId.Value = dr["Id"];
                            txtDen.Text = General.Nz(dr["Denumire"], "").ToString();
                        }

                        cmbCmp.Value = dr["F10002"];
                        cmbSub.Value = dr["F10004"];
                        cmbFil.Value = dr["F10005"];
                        cmbSec.Value = dr["F10006"];
                        cmbDept.Value = dr["F10007"];
                        cmbSubDept.Value = dr["IdSubdept"];
                        cmbBirou.Value = dr["IdBirou"];
                        cmbSup.Value = dr["IdSuperior"];
                        cmbSupFunc.Value = dr["IdSuperiorFunctional"];
                        txtNivelIer.Value = dr["NivelIerarhic"];
                        hfNivelIer["val"] = txtNivelIer.Value;

                        cmbHay.Value = dr["NivelHay"];
                        txtSalMin.Value = dr["SalariuMin"];
                        txtSalMed.Value = dr["SalariuMed"];
                        txtSalMax.Value = dr["SalariuMax"];
                        txtDtInc.Value = dr["DataInceput"];
                        txtDtSf.Value = dr["DataSfarsit"];
                        txtCodBuget.Text = General.Nz(dr["CodBuget"],"").ToString();
                        cmbCor.Value = dr["CodCOR"];
                        chkStudii.Value = Convert.ToBoolean(General.Nz(dr["StudiiSuperioare"],0));

                        txtDenRO.Text = General.Nz(dr["DenumireRO"], "").ToString();
                        txtDenEN.Text = General.Nz(dr["DenumireEN"], "").ToString();

                        txtGrupRO.Text = General.Nz(dr["NumeGrupRO"], "").ToString();
                        txtGrupEN.Text = General.Nz(dr["NumeGrupEN"], "").ToString();

                        cmbFunc.Value = dr["IdFunctie"];

                        //incarcam documentul
                        DataTable dtDoc = General.IncarcaDT(@"SELECT * FROM ""tblFisiere"" WHERE ""Tabela""='Org_Posturi' AND ""Id""=@1", new object[] { Session["IdAuto"] });
                        if (dtDoc.Rows.Count > 0)
                        {
                            metaCereriDate itm = new metaCereriDate();

                            itm.UploadedFile = dtDoc.Rows[0]["Fisier"];
                            itm.UploadedFileName = dtDoc.Rows[0]["FisierNume"];
                            itm.UploadedFileExtension = dtDoc.Rows[0]["FisierExtensie"];

                            Session["Posturi_Upload"] = itm;
                        }

                        if (General.Nz(Session["Org_Duplicare"], 0).ToString() == "1")
                        {
                            Session["Org_Duplicare"] = "0";
                            Session["IdAuto"] = -97;
                        }

                        campuriExtra = General.Nz(dr["CampuriExtra"],"").ToString();
                    }
                    else
                    {
                        txtDtInc.Value = Convert.ToDateTime(Session["DataVigoare"]).Date;
                        txtDtSf.Value = new DateTime(2100, 1, 1);
                    }

                    //incarcam pozitii
                    DataTable dtPoz = General.IncarcaDT(@"SELECT * FROM ""Org_PosturiPozitii"" WHERE ""IdPost""=@1 ORDER BY ""DataInceput""", new object[] { txtId.Value });
                    Session["Org_PosturiPozitii"] = dtPoz;
                    grDateIstoric.DataSource = Session["Org_PosturiPozitii"];
                    grDateIstoric.DataBind();
                    if (dtPoz.Rows.Count > 0)
                    {
                        DateTime dtVig = Convert.ToDateTime(Session["DataVigoare"]);
                        DataRow[] arr = dtPoz.Select("DataInceput <= #" + dtVig.ToString("yyyy-MM-dd") + "# AND #" + dtVig.ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                        if (arr.Length > 0)
                        {
                            DataRow drPoz = arr[0];
                            txtPozitii.Text = General.Nz(drPoz["Pozitii"], "").ToString();
                            txtPozitiiAprobate.Text = General.Nz(drPoz["PozitiiAprobate"], "").ToString();
                        }
                    }
                } else if (grDateIstoric.IsCallback)
                {
                    grDateIstoric.DataSource = Session["Org_PosturiPozitii"];
                    grDateIstoric.DataBind();
                }


                //incarcam structura organizatorica
                cmbCmp.DataSource = General.IncarcaDT(@"SELECT F00202 AS ""IdCompanie"", F00204 AS ""Companie"" FROM F002", null);
                cmbCmp.DataBind();
                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003", null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                    cmbDept.DataBind();
                }
                else
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                    cmbDept.DataBind();
                }

                //Florin #710
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT(@"SELECT F00809 AS ""IdBirou"", F00810 AS ""Birou"" FROM F008", null);
                cmbBirou.DataBind();

                AdaugaDosar();
                AdaugaCampuriExtra(dr);
                AdaugaEchipamente();
                AdaugaBeneficiile();

                if (!IsPostBack)
                {
                    //campuri extra
                    foreach (ListEditItem item in chkExtra.Items)
                    {
                        if (campuriExtra.IndexOf(item.Value.ToString()) >= 0)
                            item.Selected = true;
                    }

                    for (int i = 1; i <= 20; i++)
                    {
                        HtmlGenericControl div = divExtra.FindControl("divCampExtra" + i) as HtmlGenericControl;
                        if (div != null)
                        {
                            if (campuriExtra.IndexOf(i.ToString()) >= 0)
                                div.Attributes["class"] = "col-md-12";
                            else
                                div.Attributes["class"] = "col-md-12 ascuns";
                        }
                    }

                    //dosar personal
                    string idsDosar = General.Nz(General.ExecutaScalar(@"SELECT ',' + CONVERT(nvarchar(10),""IdObiect"") FROM ""Org_PosturiDosar"" WHERE ""IdPost""=(SELECT ""Id"" FROM ""Org_Posturi"" WHERE ""IdAuto""=@1) FOR XML PATH ('')", new object[] { General.Nz(Session["IdAuto"], "-97") }), "").ToString();
                    foreach (ListEditItem item in chkDosar.Items)
                    {
                        if ((idsDosar + ",").IndexOf("," + item.Value.ToString() + ",") >= 0)
                            item.Selected = true;
                    }

                    //echipamente
                    string idsEchip = General.Nz(General.ExecutaScalar(@"SELECT ',' + CONVERT(nvarchar(10),""IdObiect"") FROM ""Org_PosturiEchipamente"" WHERE ""IdPost""=(SELECT ""Id"" FROM ""Org_Posturi"" WHERE ""IdAuto""=@1) FOR XML PATH ('')", new object[] { General.Nz(Session["IdAuto"], "-97") }), "").ToString();
                    foreach (ListEditItem item in chkEchip.Items)
                    {
                        if ((idsEchip + ",").IndexOf("," + item.Value.ToString() + ",") >= 0)
                            item.Selected = true;
                    }

                    //beneficii
                    string idsBenef = General.Nz(General.ExecutaScalar(@"SELECT ',' + CONVERT(nvarchar(10),""IdObiect"") FROM ""Org_PosturiBeneficii"" WHERE ""IdPost""=(SELECT ""Id"" FROM ""Org_Posturi"" WHERE ""IdAuto""=@1) FOR XML PATH ('')", new object[] { General.Nz(Session["IdAuto"], "-97") }), "").ToString();
                    foreach (ListEditItem item in chkBenef.Items)
                    {
                        if ((idsBenef + ",").IndexOf("," + item.Value.ToString() + ",") >= 0)
                            item.Selected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            string ert = "";
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                GolireVariabile();
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback("~/Organigrama/Lista.aspx");
                else
                    Response.Redirect("~/Organigrama/Lista", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string strErr = "";

                if (txtDen.Visible == true && txtDen.Value == null) strErr += ", " + Dami.TraduCuvant("denumire");
                if (cmbCmp.Visible == true && cmbCmp.Value == null) strErr += ", " + Dami.TraduCuvant("companie");
                if (cmbSub.Visible == true && cmbSub.Value == null) strErr += ", " + Dami.TraduCuvant("subcompanie");
                if (cmbFil.Visible == true && cmbFil.Value == null) strErr += ", " + Dami.TraduCuvant("filiala");
                if (cmbSec.Visible == true && cmbSec.Value == null) strErr += ", " + Dami.TraduCuvant("sectie");
                if (cmbDept.Visible == true && cmbDept.Value == null) strErr += ", " + Dami.TraduCuvant("departament");
                if (cmbSup.Visible == true && cmbSup.Value == null && nrPost > 0) strErr += ", " + Dami.TraduCuvant("post superior");
                if (cmbSupFunc.Visible == true && cmbSupFunc.Value == null && nrPost > 0) strErr += ", " + Dami.TraduCuvant("post superior functional");
                if (hfNivelIer.Visible == true && (!hfNivelIer.Contains("val") || General.Nz(hfNivelIer["val"], "").ToString() == "") && nrPost > 0) strErr += ", " + Dami.TraduCuvant("nivel ierarhic");
                if (txtDtInc.Visible == true && txtDtInc.Value == null) strErr += ", " + Dami.TraduCuvant("data inceput");
                if (txtDtSf.Visible == true && txtDtSf.Value == null) strErr += ", " + Dami.TraduCuvant("data sfarsit");
                if (cmbCor.Visible == true && cmbCor.Value == null) strErr += ", " + Dami.TraduCuvant("cod COR");
                if (cmbFunc.Visible == true && cmbFunc.Value == null) strErr += ", " + Dami.TraduCuvant("functia");

                if (strErr != "")
                {
                    MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoError, "");
                    return;
                }

                int id = 1;
                DateTime dtInc = Convert.ToDateTime(Session["DataVigoare"]).Date;
                DateTime dtSf = new DateTime(2100, 1, 1);
                string sqlIns = "";
                string sqlUpd = "";

                sqlIns = "DUMY";

                if (General.Nz(Session["IdAuto"], "-99").ToString() == "-97")
                {
                    //daca este post nou
                    id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(Id),0) FROM ""Org_Posturi"" ", null), 0)) + 1;
                    dtInc = Convert.ToDateTime(txtDtInc.Value).Date;
                    dtSf = Convert.ToDateTime(txtDtSf.Value).Date;
                }
                else
                {

                    id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT Id FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { Session["IdAuto"] }), 1));

                    DataRow drModif = General.IncarcaDR(
                        @"SELECT 
                        CASE WHEN (F10002=@2 AND F10004=@3 AND F10005=@4 AND F10006=@5 AND F10007=@6) THEN 0 ELSE 1 END AS ""modifStruc"",
                        CASE WHEN ""CodCOR"" = @7 THEN 0 ELSE 1 END AS ""modifCor"",
                        CASE WHEN ""IdFunctie"" = @8 THEN 0 ELSE 1 END AS ""modifFunctia""
                        FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { Session["IdAuto"], 1, cmbSub.Value, cmbFil.Value, cmbSec.Value, cmbDept.Value, cmbCor.Value, cmbFunc.Value ?? -99 });

                    //daca este post existent
                    if ((drModif != null && drModif["modifStruc"].ToString() == "0" && drModif["modifCor"].ToString() == "0" && drModif["modifFunctia"].ToString() == "0") || Convert.ToDateTime(txtDtInc.Value).Date == Convert.ToDateTime(Session["DataVigoare"]).Date)
                    {
                        //daca intra in vigoare cu aceeasi data ca data inceput
                        sqlIns = "";
                        sqlUpd = "DUMY";
                    }
                    else
                    {
                        //daca intra in vigoare cu o alta data decat data inceput
                        dtInc = Convert.ToDateTime(Session["DataVigoare"]).Date;
                        sqlUpd = $@"UPDATE ""Org_Posturi"" SET ""DataSfarsit""={General.ToDataUniv(Convert.ToDateTime(Session["DataVigoare"]).AddDays(-1))} WHERE ""IdAuto"" = @1";
                    }

                    //Florin 2021.03.11
                    dtSf = Convert.ToDateTime(txtDtSf.Value).Date;

                    //Florin 2021.03.12
                    int seInteresecteaza = Convert.ToInt32(General.Nz(General.ExecutaScalar($@"SELECT COUNT(*) FROM Org_Posturi WHERE Id = @1 AND DataInceput <= @4 AND @3 <= DataSfarsit AND IdAuto <> @2", new object[] { id, Session["IdAuto"], dtInc, dtSf }), 0));
                    if (seInteresecteaza > 0)
                    {
                        MessageBox.Show(Dami.TraduCuvant("Intervalul se intersecteaza cu altul deja existent"), MessageBox.icoError, "");
                        return;
                    }

                    General.SchimbaInPlanificat(Convert.ToDateTime(Session["DataVigoare"]), id, Convert.ToInt32(drModif["modifStruc"]), Convert.ToInt32(drModif["modifCor"]), Convert.ToInt32(drModif["modifFunctia"]), 0);
                }


                //Florin #725 - verificam daca sunt angajati asignati pe acest post
                int nrAng = Convert.ToInt32(General.Nz(General.ExecutaScalar(
                    $@"SELECT COUNT(*) AS Nr
                    FROM Org_relPostAngajat X 
                    INNER JOIN F100 Y ON X.F10003=Y.F10003
                    OUTER APPLY DamiDataPlecare(X.F10003, {General.ToDataUniv(dtInc)}) Z
                    WHERE X.DataInceput <= {General.ToDataUniv(dtInc)} AND {General.ToDataUniv(dtInc)} <= X.DataSfarsit
                    AND (Y.F10022 <= {General.ToDataUniv(dtInc)} AND {General.ToDataUniv(dtInc)} <= Z.DataPlecare) AND 
                    NOT (Y.F100922 <= {General.ToDataUniv(dtInc)} AND {General.ToDataUniv(dtInc)} <= (CASE WHEN COALESCE(Y.F100924, '2100-01-01') = '2100-01-01' THEN Y.F100923 ELSE Y.F100924 END)) 
                    AND X.IdPost={id}"), 0));

                if (nrAng > 0)
                {
                    DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { General.Nz(Session["IdAuto"], "-97") });
                    if (dt.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(General.Nz(dt.Rows[0]["IdFunctie"],-99)) != Convert.ToInt32(General.Nz(cmbFunc.Value, -99)) || 
                            Convert.ToInt32(General.Nz(dt.Rows[0]["CodCOR"], -99)) != Convert.ToInt32(General.Nz(cmbCor.Value, -99)) || 
                            Convert.ToInt32(General.Nz(dt.Rows[0]["F10007"], -99)) != Convert.ToInt32(General.Nz(cmbDept.Value, -99)))
                        {
                            MessageBox.Show("Nu se pot salva modificarile deoarece exista angajati asignati pe acest post");
                            return;
                        }
                    }
                }


                int idAuto = Convert.ToInt32(General.Nz(Session["IdAuto"], -97));

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("IdAuto", Session["IdAuto"]);
                dic.Add("Id", id);
                dic.Add("Denumire", txtDen.Text);
                dic.Add("DataInceput", dtInc);
                dic.Add("DataSfarsit", dtSf);
                dic.Add("F10002", General.Nz(cmbCmp.Value, 1));
                dic.Add("F10004", cmbSub.Value);
                dic.Add("F10005", cmbFil.Value);
                dic.Add("F10006", cmbSec.Value);
                dic.Add("F10007", cmbDept.Value);
                dic.Add("IdSubdept", cmbSubDept.Value);
                dic.Add("IdBirou", cmbBirou.Value);
                dic.Add("Stare", 1);
                dic.Add("IdSuperior", cmbSup.Value);
                dic.Add("IdSuperiorFunctional", cmbSupFunc.Value);
                dic.Add("NivelIerarhic", hfNivelIer.Contains("val") && General.Nz(hfNivelIer["val"], "").ToString() != "" ? hfNivelIer["val"] : "N");
                dic.Add("NivelHay", cmbHay.Value);
                dic.Add("SalariuMin", txtSalMin.Value);
                dic.Add("SalariuMed", txtSalMed.Value);
                dic.Add("SalariuMax", txtSalMax.Value);
                dic.Add("CodBuget", txtCodBuget.Value);
                dic.Add("CodCOR", cmbCor.Value);
                dic.Add("DenumireRO", txtDenRO.Text);
                dic.Add("DenumireEN", txtDenEN.Text);
                dic.Add("NumeGrupRO", txtGrupRO.Text);
                dic.Add("NumeGrupEN", txtGrupEN.Text);
                dic.Add("IdFunctie", cmbFunc.Value);
                dic.Add("StudiiSuperioare", chkStudii.Value);
                dic.Add("CampuriExtra", String.Join(",", chkExtra.SelectedValues.Cast<string>().Select(p => p.ToString()).ToArray()));
                dic.Add("USER_NO", Session["UserId"]);
                dic.Add("TIME", DateTime.Now);

                object[] objs = new object[dic.Count + 30];
                string campuriInsert = "";
                string campuriUpdate = "";
                string valoriInsert = "";
                int x = 0;
                foreach (var item in dic)
                {
                    objs[x] = item.Value;
                    if (x > 0)
                    {
                        campuriInsert += $@",""{item.Key}""";
                        valoriInsert += $",@{x + 1}";
                    }
                    if (x > 1)
                        campuriUpdate += $@",""{item.Key}""=@{x + 1}";
                    x++;
                }

                //Florin 2020.01.21 - nu mai exista beneficiile, s-au mutat in tabela separatat conform tichet #8
                //for (int i = 1; i <= 10; i++)
                //{
                //    objs[x] = divBenef.FindControl("cmbBenef" + i) != null ? ((ASPxComboBox)divBenef.FindControl("cmbBenef" + i)).Value : null;
                //    campuriInsert += $@",""IdBeneficiu{i}""";
                //    campuriUpdate += $@",""IdBeneficiu{i}""=@{x + 1}";
                //    valoriInsert += $",@{x + 1}";
                //    x++;
                //}

                for (int i = 1; i <= 20; i++)
                {
                    objs[x] = divExtra.FindControl("cmpExtra" + i) != null ? ((ASPxMemo)divExtra.FindControl("cmpExtra" + i)).Value : null;
                    campuriInsert += $@",""CampExtra{i}""";
                    campuriUpdate += $@",""CampExtra{i}""=@{x + 1}";
                    valoriInsert += $",@{x + 1}";
                    x++;
                }

                if (sqlUpd != "")
                {
                    if (sqlUpd == "DUMY")
                        sqlUpd = $@"UPDATE ""Org_Posturi"" SET {campuriUpdate.Substring(1)} WHERE ""IdAuto"" = @1";
                    General.ExecutaNonQuery(sqlUpd, objs);
                }

                if (sqlIns != "")
                {
                    sqlIns = $@"INSERT INTO ""Org_Posturi""({campuriInsert.Substring(1)}) OUTPUT Inserted.IdAuto SELECT {valoriInsert.Substring(1)}";
                    idAuto = Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlIns, objs), 1));
                }

                //salvam documentul
                string sqlFis = "";
                metaCereriDate itm = Session["Posturi_Upload"] as metaCereriDate;
                if (itm != null && itm.UploadedFile != null)
                {
                    sqlFis = $@"
                        BEGIN
                            IF((SELECT COUNT(*) FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2) = 0)
                            INSERT INTO ""tblFisiere""(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} {General.FromDual()}
                            ELSE
                            UPDATE ""tblFisiere"" SET ""Fisier""=@3, ""FisierNume""=@4, ""FisierExtensie""=@5 WHERE ""Tabela""=@1 AND ""Id""=@2
                        END";
                    General.ExecutaNonQuery(sqlFis, new object[] { "Org_Posturi", idAuto, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                }
                else
                {
                    sqlFis = $@"DELETE FROM ""tblFisiere"" WHERE ""Tabela""=@1 AND ""Id""=@2";
                    General.ExecutaNonQuery(sqlFis, new object[] { "Org_Posturi", idAuto });
                }

                //salvam Dosarul Personal
                string sqlDosar = "";
                foreach (var item in chkDosar.SelectedValues)
                {

                    sqlDosar += $@"INSERT INTO ""Org_PosturiDosar""(""IdPost"", ""IdObiect"") VALUES({id},{item});" + Environment.NewLine;
                }

                if (sqlDosar != "")
                {
                    General.ExecutaNonQuery(
                        $@"BEGIN
                        DELETE FROM ""Org_PosturiDosar"" WHERE ""IdPost""={id};
                        {sqlDosar}
                        END;", null);
                }

                //actualizam doarul personal al tuturor angajatilor care sunt pe acest post
                General.ExecutaNonQuery($@"
                    BEGIN
                        DELETE FROM Atasamente WHERE IdEmpl IN (SELECT F10003 FROM Org_relPostAngajat WHERE IdPost=@1) AND Attach IS NULL;
                        INSERT INTO Atasamente(IdEmpl, IdCategory, VineDinPosturi, DateAttach, USER_NO, TIME)
                        SELECT A.F10003, C.IdObiect, 1, GetDate(), {Session["UserId"]}, GetDate() FROM Org_relPostAngajat A
                        INNER JOIN Org_PosturiDosar C ON A.IdPost=C.IdPost
                        LEFT JOIN Atasamente B ON A.F10003 = B.IdEmpl AND B.IdCategory=C.IdObiect
                        WHERE A.IdPost=@1 AND B.IdCategory IS NULL
                        GROUP BY A.F10003, C.IdObiect;
                    END;", new object[] { id });


                //salvam echipamentele
                string sqlEchip = "";
                foreach (var item in chkEchip.SelectedValues)
                {

                    sqlEchip += $@"INSERT INTO ""Org_PosturiEchipamente""(""IdPost"", ""IdObiect"") VALUES({id},{item});" + Environment.NewLine;
                }

                if (sqlEchip != "")
                {
                    General.ExecutaNonQuery(
                        $@"BEGIN
                        DELETE FROM ""Org_PosturiEchipamente"" WHERE ""IdPost""={id};
                        {sqlEchip}
                        END;", null);
                }

                //actualizam echipamentele tuturor angajatilor care sunt pe acest post
                General.ExecutaNonQuery($@"
                    BEGIN
                        DELETE FROM Admin_Echipamente WHERE Marca IN (SELECT F10003 FROM Org_relPostAngajat WHERE IdPost=@1) AND DataPrimire IS NULL AND Caracteristica IS NULL;
                        INSERT INTO Admin_Echipamente(Marca, IdObiect, USER_NO, TIME)
                        SELECT A.F10003, C.IdObiect, {Session["UserId"]}, GetDate() FROM Org_relPostAngajat A
                        INNER JOIN Org_PosturiEchipamente C ON A.IdPost=C.IdPost
                        LEFT JOIN Admin_Echipamente B ON A.F10003 = B.Marca AND B.IdObiect=C.IdObiect
                        WHERE A.IdPost=@1 AND B.IdObiect IS NULL
                        GROUP BY A.F10003, C.IdObiect;
                    END;", new object[] { id });


                //salvam beneficiile
                string sqlBenef = "";
                foreach (var item in chkBenef.SelectedValues)
                {

                    sqlBenef += $@"INSERT INTO ""Org_PosturiBeneficii""(""IdPost"", ""IdObiect"") VALUES({id},{item});" + Environment.NewLine;
                }

                if (sqlBenef != "")
                {
                    General.ExecutaNonQuery(
                        $@"BEGIN
                        DELETE FROM ""Org_PosturiBeneficii"" WHERE ""IdPost""={id};
                        {sqlBenef}
                        END;", null);
                }

                //actualizam beneficiile tuturor angajatilor care sunt pe acest post
                General.ExecutaNonQuery($@"
                    BEGIN
                        DELETE FROM Admin_Beneficii WHERE Marca IN (SELECT F10003 FROM Org_relPostAngajat WHERE IdPost=@1) AND DataPrimire IS NULL AND Caracteristica IS NULL;
                        INSERT INTO Admin_Beneficii(Marca, IdObiect, USER_NO, TIME)
                        SELECT A.F10003, C.IdObiect, {Session["UserId"]}, GetDate() FROM Org_relPostAngajat A
                        INNER JOIN Org_PosturiBeneficii C ON A.IdPost=C.IdPost
                        LEFT JOIN Admin_Beneficii B ON A.F10003 = B.Marca AND B.IdObiect=C.IdObiect
                        WHERE A.IdPost=@1 AND B.IdObiect IS NULL
                        GROUP BY A.F10003, C.IdObiect;
                    END;", new object[] { id });


                DataTable dtPoz = Session["Org_PosturiPozitii"] as DataTable;
                if (dtPoz != null)
                {
                    for (int i = 0; i < dtPoz.Rows.Count; i++)
                    {
                        if (dtPoz.Rows[i].RowState != DataRowState.Deleted && Convert.ToInt32(dtPoz.Rows[i]["IdPost"]) == -99)
                            dtPoz.Rows[i]["IdPost"] = id;
                    }
                    General.SalveazaDate(dtPoz, "Org_PosturiPozitii");
                    Session["Org_PosturiPozitii"] = null;
                }

                btnBack_Click(sender, e);
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
                int idRap = Convert.ToInt32(Dami.ValoareParam("IdRaportFisaPost", "-99"));
                if (idRap != -99 && Convert.ToInt32(General.Nz(Session["IdAuto"], "-99")) != -99)
                {
                    var reportParams = new
                    {
                        IdAutoPost = Convert.ToInt32(General.Nz(Session["IdAuto"], "-99"))
                    };

                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(idRap);
                    var reportUrl = Wizrom.Reports.Code.ReportProxy.GetViewUrl(idRap, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);

                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Org_FisaPost", "window.location.href = \"" + ResolveClientUrl(reportUrl) + "\"", true);
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
                metaCereriDate itm = new metaCereriDate();
                if (Session["Posturi_Upload"] != null) itm = Session["Posturi_Upload"] as metaCereriDate;

                itm.UploadedFile = btnDocUpload.UploadedFiles[0].FileBytes;
                itm.UploadedFileName = btnDocUpload.UploadedFiles[0].FileName;
                itm.UploadedFileExtension = btnDocUpload.UploadedFiles[0].ContentType;

                Session["Posturi_Upload"] = itm;

                btnDocUpload.JSProperties["cpDocUploadName"] = btnDocUpload.UploadedFiles[0].FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                switch (e.Parameter)
                {
                    case "cmbCmp":
                        cmbSub.Value = null;
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        break;
                    case "cmbSub":
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        break;
                    case "cmbFil":
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        break;
                    case "cmbSec":
                        cmbDept.Value = null;
                        break;
                    case "cmbDept":
                        //Florin #710
                        cmbSubDept.Value = null;
                        cmbBirou.Value = null;
                        break;
                    case "5":
                        metaCereriDate itm = new metaCereriDate();
                        if (Session["Posturi_Upload"] != null) itm = Session["Posturi_Upload"] as metaCereriDate;

                        itm.UploadedFile = null;
                        itm.UploadedFileName = null;
                        itm.UploadedFileExtension = null;

                        Session["Posturi_Upload"] = itm;
                        lblDoc.InnerHtml = "&nbsp;";
                        break;
                    case "btnSterge":
                        {
                            General.ExecutaNonQuery(
                                @"BEGIN
                                DELETE FROM ""Org_relPostAngajat"" WHERE ""IdPost""=@1;
                                DELETE FROM ""Org_relPostBeneficiu"" WHERE ""IdPost""=@1;
                                DELETE FROM ""Org_relPostRol"" WHERE ""IdPost""=@1;
                                DELETE FROM ""Org_PosturiPozitii"" WHERE ""IdPost""=@1;
                                DELETE FROM ""Org_PosturiDosar"" WHERE ""IdPost""=@1;
                                DELETE FROM ""Org_Posturi"" WHERE ""Id""=@1;
                                END;", new object[] { txtId.Value });
                            btnBack_Click(null, null);
                        }
                        break;
                }

                cmbSub.DataSource = General.IncarcaDT(@"SELECT F00304 AS ""IdSubcompanie"", F00305 AS ""Subcompanie"" FROM F003 WHERE F00303=" + General.Nz(cmbCmp.Value, -99), null);
                cmbSub.DataBind();
                cmbFil.DataSource = General.IncarcaDT(@"SELECT F00405 AS ""IdFiliala"", F00406 AS ""Filiala"" FROM F004 WHERE F00404=" + General.Nz(cmbSub.Value, -99), null);
                cmbFil.DataBind();
                cmbSec.DataSource = General.IncarcaDT(@"SELECT F00506 AS ""IdSectie"", F00507 AS ""Sectie"" FROM F005 WHERE F00505=" + General.Nz(cmbFil.Value, -99), null);
                cmbSec.DataBind();
                if (cmbSub.Value == null && cmbFil.Value == null && cmbSec.Value == null)
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006", null);
                    cmbDept.DataBind();
                }
                else
                {
                    cmbDept.DataSource = General.IncarcaDT(@"SELECT F00607 AS ""IdDept"", F00608 AS ""Dept"" FROM F006 WHERE F00606=" + General.Nz(cmbSec.Value, -99), null);
                    cmbDept.DataBind();
                }

                //Florin #710
                cmbSubDept.DataSource = General.IncarcaDT(@"SELECT F00708 AS ""IdSubDept"", F00709 AS ""SubDept"" FROM F007 WHERE F00707=" + General.Nz(cmbDept.Value, -99), null);
                cmbSubDept.DataBind();
                cmbBirou.DataSource = General.IncarcaDT(@"SELECT F00809 AS ""IdBirou"", F00810 AS ""Birou"" FROM F008", null);
                cmbBirou.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private DataTable PostSuperior(DateTime dtVig)
        {
            DataTable dt = new DataTable();

            try
            {
                string strSql = $@"SELECT COALESCE(A.""Id"",1) AS ""Id"", A.""Denumire"", A.""NivelIerarhic"", B.F00204 AS ""Companie"", C.F00305 AS ""Subcompanie"", D.F00406 as ""Filiala"", E.F00507 AS ""Sectie"", F.F00608 AS ""Departament""
                    FROM ""Org_Posturi"" A
                    LEFT JOIN F002 B ON A.F10002 = B.F00202
                    LEFT JOIN F003 C ON A.F10004 = C.F00304
                    LEFT JOIN F004 D ON A.F10005 = D.F00405
                    LEFT JOIN F005 E ON A.F10006 = E.F00506
                    LEFT JOIN F006 F ON A.F10007 = F.F00607
                    WHERE {General.TruncateDate("A.DataInceput")} <= {General.ToDataUniv(dtVig)} AND {General.ToDataUniv(dtVig)} <= {General.TruncateDate("A.DataSfarsit")}  ";
                dt = General.IncarcaDT(strSql, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return dt;
        }

        public void AdaugaInF718(string denumire)
        {
            try
            {
                string strSql = 
                    @"IF ((SELECT COUNT(*) FROM F718 WHERE F71804=@1)=0)
                    INSERT INTO F718(F71801, F71802, F71804, USER_NO, TIME) VALUES(718, (SELECT COALESCE(MAX(COALESCE(F71802,0)),0) FROM F718) + 1, @1, -9, GETDATE())";
                General.ExecutaNonQuery(strSql, new object[] { denumire });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaCampuriExtra(DataRow dr)
        {
            try
            {
                DataTable dt = Session["Org_CampuriExtra"] as DataTable;
                if (Session["Org_CampuriExtra"] == null)
                    dt = General.IncarcaDT($@"SELECT * FROM ""Org_tblConfig"" WHERE COALESCE(""Vizibil"",0)=1 ORDER BY COALESCE(""Ordine"",9999)", null);

                chkExtra.DataSource = dt;
                chkExtra.DataBind();

                HtmlGenericControl divRow = new HtmlGenericControl("div");
                divRow.Attributes["class"] = "row";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HtmlGenericControl divCol = new HtmlGenericControl("div");
                    divCol.Attributes["class"] = "col-md-12";
                    divCol.ID = "divCampExtra" + General.Nz(dt.Rows[i]["Id"], 1).ToString();
                    divCol.ClientIDMode = ClientIDMode.Static;

                    HtmlGenericControl lbl = new HtmlGenericControl("label");
                    lbl.Style["width"] = "100%";
                    lbl.Style["margin-top"] = "20px";
                    lbl.InnerText = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Eticheta"],"").ToString());

                    ASPxMemo txt = new ASPxMemo();
                    txt.ID = "cmpExtra" + General.Nz(dt.Rows[i]["Id"], 1).ToString();
                    txt.Width = Unit.Percentage(100);
                    txt.Height = Unit.Pixel(100);
                    if (dr != null)
                        txt.Value = dr["CampExtra" + General.Nz(dt.Rows[i]["Id"], 1).ToString()];

                    divCol.Controls.Add(lbl);
                    divCol.Controls.Add(txt);

                    divRow.Controls.Add(divCol);
                }

                
                divExtra.Controls.Add(divRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //private void AdaugaBeneficiile(DataRow dr)
        //{
        //    try
        //    {
        //        string strSql = $@"SELECT B.""Id"" AS ""IdCateg"", B.""Denumire"" AS ""Eticheta"", C.""Id"", C.""Denumire"", B.""OrdineInPosturi""
        //            FROM ""Admin_Arii"" A
        //            INNER JOIN ""Admin_Categorii"" B ON A.""Id"" = B.""IdArie""
        //            INNER JOIN ""Admin_Obiecte"" C ON B.""Id"" = C.""IdCategorie""
        //            WHERE (A.""Denumire"" = 'Beneficii' OR A.""Denumire"" = 'atribute posturi') AND B.""OrdineInPosturi"" IS NOT NULL AND ""OrdineInPosturi"" <> 0
        //            ORDER BY B.""OrdineInPosturi"", B.""Id"", B.""Denumire"" ";
        //        DataTable dtBen = General.IncarcaDT(strSql, null);

        //        HtmlGenericControl divRow = new HtmlGenericControl("div");
        //        divRow.Attributes["class"] = "row";

        //        DataTable dt = dtBen.DefaultView.ToTable(true, new string[] { "IdCateg", "Eticheta" });
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            HtmlGenericControl divCol = new HtmlGenericControl("div");
        //            divCol.Attributes["class"] = "col-md-6";

        //            HtmlGenericControl lbl = new HtmlGenericControl("label");
        //            lbl.Style["width"] = "100%";
        //            lbl.Style["margin-top"] = "20px";
        //            lbl.InnerText = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Eticheta"], "").ToString());

        //            ASPxComboBox cmb = new ASPxComboBox();
        //            cmb.ID = "cmbBenef" + (i + 1).ToString();
        //            cmb.Width = Unit.Pixel(300);
        //            cmb.AutoPostBack = false;
        //            cmb.AllowNull = true;
        //            cmb.ValueField = "Id";
        //            cmb.ValueType = typeof(System.Int32);
        //            cmb.TextField = "Denumire";
        //            DataView view = dtBen.DefaultView;
        //            view.RowFilter = "IdCateg=" + Convert.ToInt32(General.Nz(dt.Rows[i]["IdCateg"], -99));
        //            cmb.DataSource = view;
        //            cmb.DataBind();

        //            if (dr != null)
        //                cmb.Value = dr["IdBeneficiu" + (i + 1).ToString()];

        //            divCol.Controls.Add(lbl);
        //            divCol.Controls.Add(cmb);

        //            divRow.Controls.Add(divCol);
        //        }

        //        divBenef.Controls.Add(divRow);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        private void AdaugaDosar()
        {
            try
            {
                DataTable dt = General.IncarcaDT(@"SELECT ""IdCategory"" AS ""Id"", ""NameCategory"" AS ""Denumire"" FROM ""CategoriiAtasamente"" WHERE ""IdCategory"" <= 1000 ORDER BY ""NameCategory"" ");
                chkDosar.DataSource = dt;
                chkDosar.DataBind();

                //DataTable dtTab = General.IncarcaDT($@"
                //    SELECT C.""Id"", C.""Denumire"", CASE WHEN D.""IdObiect"" IS NULL THEN 0 ELSE 1 END AS ""Bifat""
                //    FROM ""Admin_Arii"" A
                //    INNER JOIN ""Admin_Categorii"" B ON A.""Id""=B.""IdArie""
                //    INNER JOIN ""Admin_Obiecte"" C ON B.""Id""=C.""IdCategorie""
                //    LEFT JOIN ""Org_PosturiDosar"" D ON C.""Id"" = D.""IdObiect"" AND ""IdPost""={General.Nz(txtId.Value, -99)}
                //    WHERE A.""Id"" = (SELECT COALESCE(""Valoare"",'') FROM ""tblParametrii"" WHERE ""Nume""='ArieTabDosarPersonalDinPersonal')
                //    AND B.""Denumire""='Documente Post'", null);

                //lstDosar.DataSource = dtTab;
                //lstDosar.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaEchipamente()
        {
            try
            {
                DataTable dt = General.GetObiecteDinArie("ArieTabEchipamenteDinPersonal");
                chkEchip.DataSource = dt;
                chkEchip.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaBeneficiile()
        {
            try
            {
                DataTable dt = General.GetObiecteDinArie("ArieTabBeneficiiDinPersonal");
                chkBenef.DataSource = dt;
                chkBenef.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lstDosar_DataBound(object sender, EventArgs e)
        {
            try
            {
                var lstDosar = (ASPxCheckBoxList)sender;
                DataTable dt = lstDosar.DataSource as DataTable;

                for (int i = 0; i < lstDosar.Items.Count; i++)
                {
                    lstDosar.Items[i].Selected = Convert.ToBoolean(General.Nz(dt.Rows[i]["Bifat"],0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_RowDeleting(object sender, DevExpress.Web.Data.ASPxDataDeletingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow found = dt.Rows.Find(e.Keys["IdAuto"]);
                found.Delete();

                PopuleazaPozitii(dt);

                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow[] arr = dt.Select("DataInceput <= #" + Convert.ToDateTime(e.NewValues["DataSfarsit"]).ToString("yyyy-MM-dd") + "# AND #" + Convert.ToDateTime(e.NewValues["DataInceput"]).ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                if (arr.Length == 0)
                {
                    DataRow dr = dt.NewRow();

                    dr["IdPost"] = txtId.Value ?? -99;
                    dr["Pozitii"] = e.NewValues["Pozitii"] ?? 0;
                    dr["PozitiiAprobate"] = e.NewValues["PozitiiAprobate"] ?? 0;
                    dr["DataInceput"] = e.NewValues["DataInceput"];
                    dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    dt.Rows.Add(dr);

                    PopuleazaPozitii(dt);
                }
                else
                    grDateIstoric.JSProperties["cpAlertMessage"] = "Acest interval se intersecteaza cu unul deja existent";

                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_RowUpdating(object sender, DevExpress.Web.Data.ASPxDataUpdatingEventArgs e)
        {
            try
            {
                DataTable dt = Session["Org_PosturiPozitii"] as DataTable;
                DataRow[] arr = dt.Select("IdAuto <> " + e.Keys["IdAuto"] + " AND DataInceput <= #" + Convert.ToDateTime(e.NewValues["DataSfarsit"]).ToString("yyyy-MM-dd") + "# AND #" + Convert.ToDateTime(e.NewValues["DataInceput"]).ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                if (arr.Length == 0)
                {
                    DataRow dr = dt.Rows.Find(e.Keys["IdAuto"]);

                    dr["Pozitii"] = e.NewValues["Pozitii"] ?? 0;
                    dr["PozitiiAprobate"] = e.NewValues["PozitiiAprobate"] ?? 0;
                    dr["DataInceput"] = e.NewValues["DataInceput"];
                    dr["DataSfarsit"] = e.NewValues["DataSfarsit"];
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    PopuleazaPozitii(dt);
                }
                else
                    grDateIstoric.JSProperties["cpAlertMessage"] = "Acest interval se intersecteaza cu unul deja existent";

                e.Cancel = true;
                grDateIstoric.CancelEdit();
                Session["Org_PosturiPozitii"] = dt;
                grDateIstoric.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void grDateIstoric_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            try
            {
                e.NewValues["IdPost"] = txtId.Value;
                if (General.Nz(Session["IdAuto"], "-99").ToString() == "-97")
                {
                    e.NewValues["DataInceput"] = DateTime.Now.Date;
                    e.NewValues["DataSfarsit"] = txtDtSf.Value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void GolireVariabile()
        {
            try
            {
                Session["DataVigoare"] = null;
                Session["IdAuto"] = -97;
                Session["Org_Duplicare"] = "0";
                Session["Posturi_Upload"] = null;
                Session["Org_PosturiPozitii"] = null;
                Session["Org_CampuriExtra"] = null;
                Session.Remove("DataVigoare");
                Session.Remove("IdAuto");
                Session.Remove("Org_Duplicare");
                Session.Remove("Posturi_Upload");
                Session.Remove("Org_PosturiPozitii");
                Session.Remove("Org_CampuriExtra");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void PopuleazaPozitii(DataTable dt)
        {
            try
            {
                DataRow[] arrNew = dt.Select("DataInceput <= #" + Convert.ToDateTime(Session["DataVigoare"]).ToString("yyyy-MM-dd") + "# AND #" + Convert.ToDateTime(Session["DataVigoare"]).ToString("yyyy-MM-dd") + "# <= DataSfarsit");
                if (arrNew.Length > 0)
                {
                    //txtPozitii.Text = General.Nz(arrNew[0]["Pozitii"], "").ToString();
                    //txtPozitiiAprobate.Text = General.Nz(arrNew[0]["PozitiiAprobate"], "").ToString();
                    grDateIstoric.JSProperties["cpPozitii"] = General.Nz(arrNew[0]["Pozitii"], "").ToString();
                    grDateIstoric.JSProperties["cpPozitiiAprobate"] = General.Nz(arrNew[0]["PozitiiAprobate"], "").ToString();
                }
                else
                {
                    grDateIstoric.JSProperties["cpPozitii"] = "";
                    grDateIstoric.JSProperties["cpPozitiiAprobate"] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

    }
}