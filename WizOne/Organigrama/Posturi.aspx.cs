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
                Dami.AccesApp();

                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");
                btnExit.Text = Dami.TraduCuvant("btnExit", "Iesire");
                btnSave.Text = Dami.TraduCuvant("btnSave", "Salveaza");

                btnDocUpload.ToolTip = Dami.TraduCuvant("incarca document");
                btnDocSterge.ToolTip = Dami.TraduCuvant("sterge document");

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
                        cmbSup.Value = dr["IdSuperior"];
                        cmbSupFunc.Value = dr["IdSuperiorFunctional"];
                        txtNivelIer.Value = dr["NivelIerarhic"];
                        hfNivelIer["val"] = txtNivelIer.Value;
                        txtPlan.Text = General.Nz(dr["PlanHC"],"").ToString();
                        txtHCAProbat.Text = General.Nz(dr["HCAprobat"], "").ToString();
                        cmbHay.Value = dr["NivelHay"];
                        txtSalMin.Value = dr["SalariuMin"];
                        txtSalMed.Value = dr["SalariuMed"];
                        txtSalMax.Value = dr["SalariuMax"];
                        txtDtInc.Value = dr["DataInceput"];
                        txtDtSf.Value = dr["DataSfarsit"];
                        txtCodBuget.Text = General.Nz(dr["CodBuget"],"").ToString();
                        cmbCor.Value = dr["CodCOR"];

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
                    }
                    else
                    {
                        txtDtInc.Value = Convert.ToDateTime(Session["DataVigoare"]);
                        txtDtSf.Value = new DateTime(2100, 1, 1);
                    }

                    AdaugaDosar();
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

                AdaugaCampuriExtra(dr);
                AdaugaBeneficiile(dr);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
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
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public void Iesire()
        {
            try
            {
                Response.Redirect("~/Organigrama/Lista.aspx", false);
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
                if (txtPlan.Visible == true && txtPlan.Value == null) strErr += ", " + Dami.TraduCuvant("plan HC");
                if (txtHCAProbat.Visible == true && txtHCAProbat.Value == null) strErr += ", " + Dami.TraduCuvant("HC aprobat");
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
                DateTime dtInc = Convert.ToDateTime(Session["DataVigoare"]);
                DateTime dtSf = new DateTime(2100, 1, 1);
                string sqlIns = "";
                string sqlUpd = "";


                sqlIns = @"INSERT INTO ""Org_Posturi""(
                    ""Id"", ""Denumire"",""DataInceput"", ""DataSfarsit"", F10002, F10004, F10005, F10006, F10007, ""Stare"", ""IdSuperior"", ""IdSuperiorFunctional"", 
                    ""NivelIerarhic"", ""PlanHC"", ""NivelHay"", ""SalariuMin"", ""SalariuMed"", ""SalariuMax"", 
                    ""CodBuget"", ""CodCOR"", USER_NO, TIME, ""DenumireRO"", ""DenumireEN"", ""NumeGrupRO"", ""NumeGrupEN"", ""HCAprobat"", ""IdFunctie"",
                    ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"", 
                    ""CampExtra1"", ""CampExtra2"", ""CampExtra3"", ""CampExtra4"", ""CampExtra5"", ""CampExtra6"", ""CampExtra7"", ""CampExtra8"", ""CampExtra9"", ""CampExtra10"", 
                    ""CampExtra11"", ""CampExtra12"", ""CampExtra13"", ""CampExtra14"", ""CampExtra15"", ""CampExtra16"", ""CampExtra17"", ""CampExtra18"", ""CampExtra19"", ""CampExtra20"") 
                    OUTPUT Inserted.IdAuto
                    SELECT @2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15,@16,@17,@18,@19,@20,@21,@22,@23,@24,@25,@26,@27,@28,@29,@30,@31,@32,@33,@34,@35,@36,@37,@38,@39,@40,@41,@42,@43,@44,@45,@46,@47,@48,@49,@50,@51,@52,@53,@54,@55,@56,@57,@58,@59";


                if (General.Nz(Session["IdAuto"], "-99").ToString() == "-97")
                {
                    //daca este post nou
                    id = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(Id),0) FROM ""Org_Posturi"" ", null), 0)) + 1;
                    dtInc = Convert.ToDateTime(txtDtInc.Value);
                    dtSf = Convert.ToDateTime(txtDtSf.Value);
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
                    if ((drModif != null && drModif["modifStruc"].ToString() == "0" && drModif["modifCor"].ToString() == "0" && drModif["modifFunctia"].ToString() == "0")
                        || Convert.ToDateTime(txtDtInc.Value).Date == Convert.ToDateTime(Session["DataVigoare"]).Date)
                    {
                        //daca intra in vigoare cu aceeasi data ca data inceput
                        sqlIns = "";

                        sqlUpd = $@"UPDATE ""Org_Posturi"" SET
                            ""Denumire""=@3, F10002=@6, F10004=@7, F10005=@8, F10006=@9, F10007=@10, ""Stare""=@11, ""IdSuperior""=@12, ""IdSuperiorFunctional""=@13, 
                            ""NivelIerarhic""=@14, ""PlanHC""=@15, ""NivelHay""=@16, ""SalariuMin""=@17, ""SalariuMed""=@18, ""SalariuMax""=@19, 
                            ""CodBuget""=@20, ""CodCOR""=@21, USER_NO=@22, TIME=@23, ""DenumireRO""=@24, ""DenumireEN""=@25, ""NumeGrupRO""=@26, ""NumeGrupEN""=@27, ""HCAprobat""=@28, ""IdFunctie""=@29,
                            ""IdBeneficiu1""=@30, ""IdBeneficiu2""=@31, ""IdBeneficiu3""=@32, ""IdBeneficiu4""=@33, ""IdBeneficiu5""=@34, ""IdBeneficiu6""=@35, ""IdBeneficiu7""=@36, ""IdBeneficiu8""=@37, ""IdBeneficiu9""=@38, ""IdBeneficiu10""=@39,
                            ""CampExtra1""=@40, ""CampExtra2""=@41, ""CampExtra3""=@42, ""CampExtra4""=@43, ""CampExtra5""=@44, ""CampExtra6""=@45, ""CampExtra7""=@46, ""CampExtra8""=@47, ""CampExtra9""=@48, ""CampExtra10""=@49, 
                            ""CampExtra11""=@50, ""CampExtra12""=@51, ""CampExtra13""=@52, ""CampExtra14""=@53, ""CampExtra15""=@54, ""CampExtra16""=@55, ""CampExtra17""=@56, ""CampExtra18""=@57, ""CampExtra19""=@58, ""CampExtra20""=@59
                            WHERE ""IdAuto"" = @1";
                    }
                    else
                    {
                        //daca intra in vigoare cu o alta data decat data inceput
                        dtInc = Convert.ToDateTime(Session["DataVigoare"]);

                        sqlUpd = $@"UPDATE ""Org_Posturi"" SET
                                ""DataSfarsit""={General.ToDataUniv(Convert.ToDateTime(Session["DataVigoare"]).AddDays(-1))}
                                WHERE ""IdAuto"" = @1";
                    }

                    General.SchimbaInPlanificat(Convert.ToDateTime(Session["DataVigoare"]), id, Convert.ToInt32(drModif["modifStruc"]), Convert.ToInt32(drModif["modifCor"]), Convert.ToInt32(drModif["modifFunctia"]), 0);
                }

                int idAuto = Convert.ToInt32(General.Nz(Session["IdAuto"], -97));

                object[] objs = new object[] {
                    Session["IdAuto"], id, txtDen.Text, dtInc, dtSf, General.Nz(cmbCmp.Value,1), cmbSub.Value, cmbFil.Value, cmbSec.Value, cmbDept.Value, 1, cmbSup.Value, cmbSupFunc.Value,
                    hfNivelIer.Contains("val") && General.Nz(hfNivelIer["val"],"").ToString() != "" ? hfNivelIer["val"] : "N",
                    General.Nz(txtPlan.Text,"").ToString() == "" ? "null" : txtPlan.Text,
                    cmbHay.Value, txtSalMin.Value, txtSalMed.Value, txtSalMax.Value,
                    txtCodBuget.Value, cmbCor.Value, Session["UserId"], DateTime.Now,
                    General.Nz(txtDenRO.Text,"").ToString() == "" ? null : txtDenRO.Text,
                    General.Nz(txtDenEN.Text,"").ToString() == "" ? null : txtDenEN.Text,
                    General.Nz(txtGrupRO.Text,"").ToString() == "" ? null : txtGrupRO.Text,
                    General.Nz(txtGrupEN.Text,"").ToString() == "" ? null : txtGrupEN.Text,
                    General.Nz(txtHCAProbat.Text,"").ToString() == "" ? null : txtHCAProbat.Text,
                    cmbFunc.Value };

                for (int i = 1; i <= 10; i++)
                {
                    Array.Resize(ref objs, objs.Length + 1);
                    objs[objs.Length - 1] = divBenef.FindControl("cmbBenef" + i) != null ? ((ASPxComboBox)divBenef.FindControl("cmbBenef" + i)).Value : null;
                }

                for (int i = 1; i <= 20; i++)
                {
                    Array.Resize(ref objs, objs.Length + 1);
                    objs[objs.Length - 1] = divExtra.FindControl("cmpExtra" + i) != null ? ((ASPxMemo)divExtra.FindControl("cmpExtra" + i)).Value : null;
                }

                if (sqlUpd != "")
                    General.ExecutaNonQuery(sqlUpd, objs);

                if (sqlIns != "")
                    idAuto = Convert.ToInt32(General.Nz(General.ExecutaScalar(sqlIns, objs), 1));

                ////daca este post nou
                //if (sqlIns != "")
                //    AdaugaInF718(General.Nz(txtDen.Text, "").ToString());

                //salvam documentul
                if (Session["Posturi_Upload"] != null)
                {
                    metaCereriDate itm = Session["Posturi_Upload"] as metaCereriDate;
                    if (itm.UploadedFile != null)
                    {
                        string sqlFis = $@"INSERT INTO ""tblFisiere""(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
                            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

                        General.ExecutaNonQuery(sqlFis, new object[] { "Org_Posturi", idAuto, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
                    }
                }

                string sqlDosar = "";
                //salvam Dosarul Personal
                foreach(var item in lstDosar.SelectedValues)
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

                Iesire();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        //protected void btnSave_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string strErr = "";

        //        if (txtDen.Visible == true && txtDen.Value == null) strErr += ", " + Dami.TraduCuvant("denumire");
        //        if (cmbCmp.Visible == true && cmbCmp.Value == null) strErr += ", " + Dami.TraduCuvant("companie");
        //        if (cmbSub.Visible == true && cmbSub.Value == null) strErr += ", " + Dami.TraduCuvant("subcompanie");
        //        if (cmbFil.Visible == true && cmbFil.Value == null) strErr += ", " + Dami.TraduCuvant("filiala");
        //        if (cmbSec.Visible == true && cmbSec.Value == null) strErr += ", " + Dami.TraduCuvant("sectie");
        //        if (cmbDept.Visible == true && cmbDept.Value == null) strErr += ", " + Dami.TraduCuvant("departament");
        //        if (cmbSup.Visible == true && cmbSup.Value == null) strErr += ", " + Dami.TraduCuvant("post superior");
        //        if (cmbSupFunc.Visible == true && cmbSupFunc.Value == null) strErr += ", " + Dami.TraduCuvant("post superior functional");
        //        if (hfNivelIer.Visible == true && (!hfNivelIer.Contains("val") || General.Nz(hfNivelIer["val"], "").ToString() == "")) strErr += ", " + Dami.TraduCuvant("nivel ierarhic");
        //        if (txtPlan.Visible == true && txtPlan.Value == null) strErr += ", " + Dami.TraduCuvant("plan HC");
        //        if (txtHCAProbat.Visible == true && txtHCAProbat.Value == null) strErr += ", " + Dami.TraduCuvant("HC aprobat");
        //        if (txtDtInc.Visible == true && txtDtInc.Value == null) strErr += ", " + Dami.TraduCuvant("data inceput");
        //        if (txtDtSf.Visible == true && txtDtSf.Value == null) strErr += ", " + Dami.TraduCuvant("data sfarsit");
        //        if (cmbCor.Visible == true && cmbCor.Value == null) strErr += ", " + Dami.TraduCuvant("cod COR");
        //        if (cmbFunc.Visible == true && cmbFunc.Value == null) strErr += ", " + Dami.TraduCuvant("functia");

        //        if (strErr != "")
        //        {
        //            MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoError, "");
        //            return;
        //        }

        //        DataTable dt = General.IncarcaDT(@"SELECT * FROM ""Org_Posturi"" WHERE ""IdAuto""=@1", new object[] { General.Nz(Session["IdAuto"], "-97") });
        //        DataRow dr = dt.NewRow();

        //        bool nou = true;
        //        if (General.Nz(Session["IdAuto"], "-99").ToString() == "-97")
        //        {
        //            //daca este post nou
        //            nou = true;
        //            dr["Id"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(Id),0) FROM ""Org_Posturi"" ", null), 0)) + 1;
        //            dr["DataInceput"] = txtDtInc.Value;
        //            dr["DataSfarsit"] = txtDtSf.Value;
        //        }
        //        else
        //        {
        //            //daca este post existent
        //            var drOrg = dt.Rows[0];
        //            if (Convert.ToDateTime(drOrg["DataInceput"]).Date == Convert.ToDateTime(Session["DataVigoare"]).Date)
        //            {
        //                //daca intra in vigoare cu aceeasi data ca data inceput
        //                nou = false;
        //                dr = dt.Rows[0];
        //            }
        //            else
        //            {
        //                //daca intra in vigoare cu o alta data decat data inceput
        //                nou = true;
        //                dr.ItemArray = drOrg.ItemArray.Clone() as object[];
        //                drOrg["DataSfarsit"] = Convert.ToDateTime(Session["DataVigoare"]).AddDays(-1);
        //                dr["DataInceput"] = Convert.ToDateTime(Session["DataVigoare"]);
        //            }
        //        }

        //        if (!nou)
        //            dr = dt.Rows[0];
        //        else
        //            dr["Id"] = Convert.ToInt32(General.Nz(General.ExecutaScalar(@"SELECT COALESCE(MAX(Id),0) FROM ""Org_Posturi"" ", null), 0)) + 1;

        //        dr["Denumire"] = txtDen.Text;
        //        dr["F10002"] = cmbCmp.Value ?? DBNull.Value;
        //        dr["F10004"] = cmbSub.Value ?? DBNull.Value;
        //        dr["F10005"] = cmbFil.Value ?? DBNull.Value;
        //        dr["F10006"] = cmbSec.Value ?? DBNull.Value;
        //        dr["F10007"] = cmbDept.Value ?? DBNull.Value;
        //        dr["Stare"] = 1;
        //        dr["IdSuperior"] = cmbSup.Value ?? DBNull.Value;
        //        dr["IdSuperiorFunctional"] = cmbSupFunc.Value ?? DBNull.Value;
        //        if (hfNivelIer.Contains("val")) dr["NivelIerarhic"] = General.Nz(hfNivelIer["val"], "");
        //        dr["PlanHC"] = txtPlan.Text;
        //        dr["NivelHay"] = cmbHay.Value ?? DBNull.Value;
        //        dr["SalariuMin"] = txtSalMin.Value ?? DBNull.Value;
        //        dr["SalariuMed"] = txtSalMed.Value ?? DBNull.Value;
        //        dr["SalariuMax"] = txtSalMax.Value ?? DBNull.Value;
        //        dr["CodBuget"] = txtCodBuget.Text;
        //        dr["CodCOR"] = cmbCor.Value ?? DBNull.Value;

        //        dr["IdBeneficiu1"] = cmbBenf1.Value ?? DBNull.Value;
        //        dr["IdBeneficiu2"] = cmbBenf2.Value ?? DBNull.Value;
        //        dr["IdBeneficiu3"] = cmbBenf3.Value ?? DBNull.Value;
        //        dr["IdBeneficiu4"] = cmbBenf4.Value ?? DBNull.Value;
        //        dr["IdBeneficiu5"] = cmbBenf5.Value ?? DBNull.Value;
        //        dr["IdBeneficiu6"] = cmbBenf6.Value ?? DBNull.Value;
        //        dr["IdBeneficiu7"] = cmbBenf7.Value ?? DBNull.Value;
        //        dr["IdBeneficiu8"] = cmbBenf8.Value ?? DBNull.Value;
        //        dr["IdBeneficiu9"] = cmbBenf9.Value ?? DBNull.Value;
        //        dr["IdBeneficiu10"] = cmbBenf10.Value ?? DBNull.Value;

        //        dr["IdAuto"] = 100000000;

        //        dr["USER_NO"] = Session["UserId"];
        //        dr["TIME"] = DateTime.Now;

        //        //daca este post nou
        //        if (General.Nz(Session["IdAuto"], "-99").ToString() == "-97")
        //        {
        //            AdaugaInF718(General.Nz(dr["Denumire"], "").ToString());
        //        }

        //        int idAuto = Convert.ToInt32(General.Nz(Session["IdAuto"], 1));

        //        if (nou)
        //        {
        //            dt.Rows.Add(dr);
        //            idAuto = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState == DataRowState.Added).Max(p => p.Field<int?>("IdAuto")), 0));
        //        }

        //        //salvam documentul
        //        if (Session["Posturi_Upload"] != null)
        //        {
        //            metaCereriDate itm = Session["Posturi_Upload"] as metaCereriDate;
        //            if (itm.UploadedFile != null)
        //            {
        //                string sqlFis = 
        //                    $@"INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //                    VALUES(@1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()})";

        //                General.ExecutaNonQuery(sqlFis, new object[] { "Org_Posturi", idAuto, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
        //            }
        //        }

        //        General.SalveazaDate(dt, "Org_Posturi");

        //        var ert = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState == DataRowState.Added).Max(p => p.Field<int?>("IdAuto")), 0));

        //        Iesire();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        protected void btnSterge_Click(object sender, EventArgs e)
        {
            try
            {

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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }
        
        //protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        //{
        //    try
        //    {
        //        string tip = e.Parameter;
        //        DataTable dtAbs = new DataTable();

        //        switch (tip)
        //        {
        //            case "4":               //btnSave
        //                {
        //                    SalveazaDate(2);
        //                }
        //                break;
        //            case "5":               //btnDocSterge
        //                {
        //                    metaCereriDate itm = new metaCereriDate();
        //                    if (Session["CereriDiverse_Upload"] != null) itm = Session["CereriDiverse_Upload"] as metaCereriDate;

        //                    itm.UploadedFile = null;
        //                    itm.UploadedFileName = null;
        //                    itm.UploadedFileExtension = null;

        //                    Session["CereriDiverse_Upload"] = itm;

        //                    lblDoc.InnerHtml = "&nbsp;";
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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

        //private void SalveazaDate(int tip=1)
        //{
        //    try
        //    {
        //        string strErr = "";

        //        if (cmbTip.Value == null) strErr += ", " + Dami.TraduCuvant("tip cerere");
        //        if (txtDesc.Value == null) strErr += ", " + Dami.TraduCuvant("cerere");

        //        if (strErr != "")
        //        {
        //            if (tip == 1)
        //                MessageBox.Show(Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1), MessageBox.icoWarning);
        //            else
        //                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date") + ":" + strErr.Substring(1);

        //            return;
        //        }

        //        string sqlIst = SelectCereriIstoric(Convert.ToInt32(Session["UserId"]), -1);

        //        string sqlPre = @"INSERT INTO ""MP_Cereri""(""Id"", F10003, ""IdTipCerere"", ""Descriere"", ""Raspuns"", ""IdStare"", ""IdCircuit"", ""UserIntrod"", ""Culoare"", ""Inlocuitor"", ""TotalCircuit"", ""Pozitie"", USER_NO, TIME) 
        //                        OUTPUT Inserted.Id, Inserted.IdStare ";

        //        string sqlCer = CreazaSelectCuValori();

        //        string strGen = "BEGIN TRAN " +
        //                        sqlIst + "; " +
        //                        sqlPre +
        //                        sqlCer + (Constante.tipBD == 1 ? "" : " FROM DUAL") + "; " +
        //                        "COMMIT TRAN";

        //        //Radu 18.07.2018
        //        string mesaj = AdaugaCerere(Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(cmbTip.Value), Convert.ToInt32(Session["User_Marca"] ?? -99), (txtDesc.Value == null ? "NULL" : "'" + txtDesc.Value.ToString() + "'"), sqlCer);



        //        //string msg = Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " +  General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
        //        //if (msg != "" && msg.Substring(0,1) == "2")
        //        //{
        //        //    if (tip == 1)
        //        //        MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
        //        //    else
        //        //        pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);

        //        //    return;
        //        //}
        //        //else
        //        //{
        //        //    int idCer = 1;
        //        //    DataTable dtCer = new DataTable();

        //        //    try
        //        //    {
        //        //        dtCer = General.IncarcaDT(strGen, null);
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        General.ExecutaNonQuery("ROLLBACK TRAN", null);
        //        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //        //        return;
        //        //    }


        //        //    if (dtCer.Rows.Count > 0) idCer = Convert.ToInt32(dtCer.Rows[0]["Id"]);


        //        //    #region Adaugare atasament

        //        //    if (Session["CereriDiverse_Upload"] != null)
        //        //    {
        //        //        metaCereriDate itm = Session["CereriDiverse_Upload"] as metaCereriDate;
        //        //        if (itm.UploadedFile != null)
        //        //        {
        //        //            string sqlFis = $@"INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //        //            SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

        //        //            General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idCer, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
        //        //        }
        //        //    }

        //        //    #endregion

        //        //    Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""MP_Cereri"" WHERE ""Id""=" + idCer, "MP_Cereri", idCer, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
        //        //}

        //        pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Proces realizat cu succes");
        //        ASPxPanel.RedirectOnCallback("~/CereriDiverse/Lista.aspx");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private string CreazaSelectCuValori()
        //{
        //    string sqlCer = "";

        //    try
        //    {
        //        string idCircuit = General.Nz(cmbTip.Value, -99).ToString();
        //        string sqlIdCerere = @"(SELECT COALESCE(MAX(COALESCE(""Id"",0)),0) + 1 FROM ""MP_Cereri"") ";
        //        string sqlTotal = @"(SELECT COUNT(*) FROM ""MP_CereriIstoric"" WHERE ""IdCerere""=" + sqlIdCerere + ")";
        //        string sqlIdStare = @"(SELECT ""IdStare"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";
        //        string sqlPozitie = @"(SELECT ""Pozitie"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";
        //        string sqlCuloare = @"(SELECT ""Culoare"" FROM ""MP_CereriIstoric"" WHERE ""Aprobat""=1 AND ""IdCerere""=" + sqlIdCerere + ")";

        //        sqlCer = @"SELECT " +
        //                        sqlIdCerere + " AS \"Id\", " +
        //                        Session["User_Marca"] + " AS \"F10003\", " +
        //                        cmbTip.Value + " AS \"IdTipCerere\", " +
        //                        (txtDesc.Value == null ? "NULL" : "'" + txtDesc.Value.ToString() + "'") + " AS \"Descriere\", " +
        //                        "NULL AS \"Raspuns\", " +
        //                        (sqlIdStare == null ? "NULL" : sqlIdStare.ToString()) + " AS \"IdStare\", " +
        //                        (idCircuit) + " AS \"IdCircuit\", " +
        //                        Session["UserId"] + " AS \"UserIntrod\", " +
        //                        (sqlCuloare == null ? "NULL" : sqlCuloare) + " AS \"Culoare\", " +
        //                        "NULL AS \"Inlocuitor\", " +
        //                        (sqlTotal == null ? "NULL" : sqlTotal) + " AS \"TotalCircuit\", " +
        //                        (sqlPozitie == null ? "NULL" : sqlPozitie) + " AS \"Pozitie\", " +
        //                        Session["UserId"] + " AS \"UserIntrod\", " + 
        //                        General.CurrentDate() + " AS TIME ";

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return sqlCer;
        //}

        //private string SelectCereriIstoric(int f10003, int idInloc)
        //{
        //    string sqlIst = "";

        //    try
        //    {
        //        string op = "+";
        //        string tipData = "nvarchar";

        //        if (Constante.tipBD == 2)
        //        {
        //            op = "||";
        //            tipData = "varchar2";
        //        }

        //        //exceptand primul element, selectul de mai jos intoarce un string cu toti actorii de pe circuit
        //        string sqlCir = $@"SELECT CAST(COALESCE(0,0) AS {tipData}(10)) {op} ';' {op} COALESCE(CAST({Session["UserId"]} AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super1"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super2"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super3"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super4"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super5"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super6"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super7"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super8"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super9"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super10"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super11"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super12"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super13"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super14"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super15"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super16"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super17"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super18"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super19"" AS {tipData}(10)) {op} ';','') {op} COALESCE(CAST(""Super20"" AS {tipData}(10)) {op} ';','') FROM ""MP_CereriCircuit"" WHERE ""IdTipCerere""=" + General.Nz(cmbTip.Value, -99);
        //        string ids = (General.ExecutaScalar(sqlCir, null) ?? "").ToString();

        //        if (ids != "")
        //        {
        //            string[] lstId = ids.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //            int idx = 0;
        //            List<string> lstSql = new List<string>();
        //            string strSql = "";

        //            //cand CumulareAcelasiSupervizor este:
        //            // 0 - (se rezolva punand particula ALL in UNION) se pun toti supervizorii chiar daca se repeta; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  3;   8;   3;   9;
        //            // 1 - (se trateaza separat, dupa bucla for) daca uramtorul in circuit este acelasi user, se salveaza doar o singura data; ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;   3;   9;
        //            // 2 - (se rezolva scotand particula ALL din UNION) user-ul se salveaza doar o singura data indiferent de cate ori este pe circuit sau pe ce pozitie este;  ex: circuit -> 3;  3;  8;   3;   9;  rezulta -> 3;  8;  9;
        //            string paramCumul = Dami.ValoareParam("CumulareAcelasiSupervizor", "0");
        //            string strUnion = "ALL";
        //            if (paramCumul == "2") strUnion = "";

        //            for (int i = 1; i < lstId.Count(); i++)
        //            {
        //                string strTmp = "";

        //                //daca Supervizorul este angajatul          
        //                if (lstId[i].ToString() == "0") strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", F70102 AS \"IdUser\", " + lstId[i].ToString() + " AS \"IdSuper\", 0 AS \"Inlocuitor\" FROM USERS WHERE F10003=" + f10003;

        //                //daca Supervizorul este id de utilizator                 
        //                if (Convert.ToInt32(lstId[i].ToString()) > 0) strTmp += " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", " + lstId[i].ToString() + " AS \"IdUser\", 76 AS \"IdSuper\", 0 AS \"Inlocuitor\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

        //                //daca Supervizorul este din nommenclatorul tblSupervizori (este cu minus)
        //                //se foloseste union pt a acoperi si cazul in care user-ul logat este deja un superviozr pt acest angajat;
        //                if (Convert.ToInt32(lstId[i].ToString()) < 0)
        //                {
        //                    strTmp = @" UNION {4} SELECT TOP 1 {3} AS ""Index"", IdUser, {1} AS IdSuper, 0 AS ""Inlocuitor"" FROM (
        //                                SELECT TOP 1 IdUser FROM F100Supervizori WHERE F10003 = {0} AND IdSuper = (-1 * {1}) AND IdUser = {2}
        //                                UNION ALL
        //                                SELECT TOP 1 IdUser FROM F100Supervizori WHERE F10003 = {0} AND IdSuper = (-1 * {1})
        //                                ) x ";
        //                    if (Constante.tipBD == 2)
        //                    {
        //                        strTmp = @" UNION {4} SELECT {3} AS ""Index"", ""IdUser"", {1} AS ""IdSuper"", 0 AS ""Inlocuitor"" FROM (
        //                            SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ""IdUser"" = {2} AND ROWNUM<=1
        //                            UNION ALL
        //                            SELECT ""IdUser"" FROM ""F100Supervizori"" WHERE F10003 = {0} AND ""IdSuper"" = (-1 * {1}) AND ROWNUM<=1
        //                            ) x  WHERE ROWNUM<=1";
        //                    }

        //                    strTmp = string.Format(strTmp, f10003, Convert.ToInt32(lstId[i]), HttpContext.Current.Session["UserId"], idx, strUnion);
        //                }

        //                idx++;
        //                strSql += strTmp;
        //                lstSql.Add(strTmp);

        //                //inseram inlocuitorul pe a doua pozitie din circuit
        //                if (idx == 1 && Convert.ToInt32(lstId[0] ?? "0") == 1 && idInloc != -1)
        //                {
        //                    string strInloc = " UNION " + strUnion + " SELECT " + idx + " AS \"Index\", F70102 AS \"IdUser\", -78 AS \"IdSuper\", 1 AS \"Inlocuitor\" FROM USERS WHERE F10003=" + idInloc;

        //                    idx++;
        //                    strSql += strInloc;
        //                    lstSql.Add(strInloc);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return sqlIst;
        //}

        //public string AdaugaCerere(int idUser, int idTip, int f10003, string cerere, string sqlCer)
        //{
        //    int idUrm = -99;

        //    try
        //    {

        //        string msg = Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Validare, sqlCer + ", " + General.CurrentDate() + " AS TIME," + Session["UserId"] + " AS USER_NO , 1 AS \"Actiune\", 1 AS \"IdStareViitoare\", '' AS \"Comentarii\" " + (Constante.tipBD == 1 ? "" : " FROM DUAL"), "", -99, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));
        //        if (msg != "" && msg.Substring(0, 1) == "2")
        //        {
        //            if (idTip == 1)
        //                MessageBox.Show(msg.Substring(2), MessageBox.icoWarning);
        //            else
        //                pnlCtl.JSProperties["cpAlertMessage"] = msg.Substring(2);

        //            return "";
        //        }
        //        else
        //        {

        //            idUrm = Convert.ToInt32(Dami.NextId("MP_Cereri"));

        //            string sql = "SELECT * FROM \"MP_CereriCircuit\" WHERE \"IdTipCerere\" = " + idTip;
        //            DataTable dtCircuit = General.IncarcaDT(sql, null);


        //            if (dtCircuit == null || dtCircuit.Rows.Count <= 0) return "Atributul nu are circuit alocat.";

        //            int idCircuit = -99;
        //            idCircuit = Convert.ToInt32(dtCircuit.Rows[0]["IdAuto"].ToString());

        //            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = 1";
        //            DataTable dtStari = General.IncarcaDT(sql, null);
        //            string culoare = "#FFFFFFFF";
        //            if (dtStari != null && dtStari.Rows.Count > 0 && dtStari.Rows[0]["Culoare"] != null && dtStari.Rows[0]["Culoare"].ToString().Length > 0)
        //                culoare = dtStari.Rows[0]["Culoare"].ToString();

        //            //adaugam headerul

        //            int total = 0;
        //            int idStare = -1;
        //            int idStareCereri = 2;
        //            int pozUser = 1;

        //            //aflam totalul de users din circuit
        //            for (int i = 1; i <= 20; i++)
        //            {
        //                var idSuper = (dtCircuit.Rows[0]["Super" + i.ToString()] == DBNull.Value ? null : dtCircuit.Rows[0]["Super" + i.ToString()]);
        //                if (idSuper != null && Convert.ToInt32(idSuper) != -99)
        //                {
        //                    //ne asiguram ca exista user pentru supervizorul din circuit
        //                    if (Convert.ToInt32(idSuper) < 0)
        //                    {
        //                        int idSpr = Convert.ToInt32(idSuper);
        //                        sql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = {0} AND \"IdSuper\" = {1}";
        //                        sql = string.Format(sql, f10003, (-1 * idSpr).ToString());
        //                        DataTable dtUser = General.IncarcaDT(sql, null);
        //                        if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == null || dtUser.Rows[0]["IdUser"].ToString().Length <= 0)
        //                        {
        //                            continue;
        //                        }
        //                    }

        //                    total++;
        //                }

        //            }


        //            //adaugam istoricul
        //            int poz = 0;
        //            int idUserCalc = -99;

        //            for (int i = 1; i <= 20; i++)
        //            {

        //                var valId = (dtCircuit.Rows[0]["Super" + i.ToString()] == DBNull.Value ? null : dtCircuit.Rows[0]["Super" + i.ToString()]);
        //                if (valId != null && Convert.ToInt32(valId) != -99)
        //                {
        //                    //poz++;

        //                    //IdUser
        //                    if (Convert.ToInt32(valId) == 0)
        //                    {
        //                        //idUserCalc = idUser;
        //                        sql = "SELECT * FROM USERS WHERE F10003 = " + f10003;
        //                        DataTable dtUtiliz = General.IncarcaDT(sql, null);
        //                        if (dtUtiliz != null && dtUtiliz.Rows.Count > 0 && dtUtiliz.Rows[0]["F70102"] != null && dtUtiliz.Rows[0]["F70102"].ToString().Length > 0)
        //                        {
        //                            idUserCalc = Convert.ToInt32(dtUtiliz.Rows[0]["F70102"].ToString());
        //                        }
        //                    }
        //                    if (Convert.ToInt32(valId) > 0) idUserCalc = Convert.ToInt32(valId);
        //                    if (Convert.ToInt32(valId) < 0)
        //                    {
        //                        int idSpr = Convert.ToInt32(valId);
        //                        //ne asiguram ca exista user pentru supervizorul din circuit
        //                        sql = "SELECT * FROM \"F100Supervizori\" WHERE F10003 = {0} AND \"IdSuper\" = {1}";
        //                        sql = string.Format(sql, f10003, (-1 * idSpr).ToString());
        //                        DataTable dtUser = General.IncarcaDT(sql, null);
        //                        if (dtUser == null || dtUser.Rows.Count <= 0 || dtUser.Rows[0]["IdUser"] == null || dtUser.Rows[0]["IdUser"].ToString().Length <= 0)
        //                        {
        //                            continue;
        //                        }
        //                        else
        //                        {
        //                            idUserCalc = Convert.ToInt32(dtUser.Rows[0]["IdUser"].ToString());
        //                        }
        //                    }

        //                    poz += 1;

        //                    int usr = Convert.ToInt32(valId);
        //                    string sqlIst = "INSERT INTO \"MP_CereriIstoric\" (\"IdCerere\", \"IdCircuit\", \"IdSuper\", \"Pozitie\", \"Inlocuitor\", \"IdUser\", \"Aprobat\", \"DataAprobare\", \"IdStare\", \"Culoare\", USER_NO, TIME, \"IdTipCerere\") "
        //                        + " VALUES ({0}, {1}, {2}, {3}, 0, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";

        //                    string aprobat = "NULL";
        //                    string dataAprobare = "NULL";
        //                    idStare = -1;
        //                    if (idUserCalc == idUser)
        //                    {
        //                        pozUser = poz;
        //                        if (poz == 1) idStare = 1;
        //                        if (poz == total) idStare = 3;

        //                        idStareCereri = idStare;

        //                        aprobat = "1";
        //                        dataAprobare = (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE");
        //                        sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStare;
        //                        DataTable dtStareIst = General.IncarcaDT(sql, null);
        //                        culoare = "#FFFFFFFF";
        //                        if (dtStareIst != null && dtStareIst.Rows.Count > 0 && dtStareIst.Rows[0]["Culoare"] != null)
        //                            culoare = dtStareIst.Rows[0]["Culoare"].ToString();
        //                    }

        //                    sqlIst = string.Format(sqlIst, idUrm, idCircuit, valId.ToString(), poz, idUserCalc, aprobat, dataAprobare, (idStare < 0 ? "NULL": idStare.ToString()), (idStare < 0 ? "NULL" : "'" + culoare + "'"), idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idTip);


        //                    General.ExecutaNonQuery(sqlIst, null);
        //                }

        //            }

        //            sql = "SELECT * FROM \"Ptj_tblStari\" WHERE \"Id\" = " + idStareCereri;
        //            DataTable dtCuloare = General.IncarcaDT(sql, null);
        //            culoare = "#FFFFFFFF";
        //            if (dtCuloare != null && dtCuloare.Rows.Count > 0 && dtCuloare.Rows[0]["Culoare"] != null)
        //                culoare = dtCuloare.Rows[0]["Culoare"].ToString();

        //            string sqlCereri = "INSERT INTO \"MP_Cereri\" (\"Id\", F10003, \"IdTipCerere\", \"Descriere\", \"Raspuns\", \"IdCircuit\", \"Pozitie\", \"UserIntrod\", \"IdStare\", \"Culoare\", \"TotalCircuit\", USER_NO, TIME) "
        //                + " VALUES ({0}, {1}, {2}, {3}, NULL, {4}, {5}, {6}, {8}, '{9}', {10}, {6}, {7})";
        //            sqlCereri = string.Format(sqlCereri, idUrm, f10003, idTip, cerere, idCircuit, pozUser, idUser, (Constante.tipBD == 1 ? "GETDATE()" : "SYSDATE"), idStareCereri, culoare, total);

        //            General.ExecutaNonQuery(sqlCereri, null);

        //            #region  Atasamente start

        //            //adauga atasamentul
        //            if (Session["CereriDiverse_Upload"] != null)
        //            {
        //                metaCereriDate itm = Session["CereriDiverse_Upload"] as metaCereriDate;
        //                if (itm.UploadedFile != null)
        //                {
        //                    string sqlFis = $@"INSERT INTO tblFisiere(""Tabela"", ""Id"", ""EsteCerere"", ""Fisier"", ""FisierNume"", ""FisierExtensie"", USER_NO, TIME) 
        //                    SELECT @1, @2, 0, @3, @4, @5, @6, {General.CurrentDate()} " + (Constante.tipBD == 1 ? "" : " FROM DUAL");

        //                    //General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idUrm, itm.UploadedFile, itm.UploadedFileName, itm.UploadedFileExtension, Session["UserId"] });
        //                    General.ExecutaNonQuery(sqlFis, new object[] { "MP_Cereri", idUrm, itm.UploadedFile, itm.UploadedFileName, "." + itm.UploadedFileName.ToString().Split('.')[itm.UploadedFileName.ToString().Split('.').Length - 1], Session["UserId"] });
        //                }
        //            }


        //            Notif.TrimiteNotificare("CereriDiverse.Lista", (int)Constante.TipNotificare.Notificare, @"SELECT *, 1 AS ""Actiune"", 1 AS ""IdStareViitoare"" FROM ""MP_Cereri"" WHERE ""Id""=" + idUrm, "MP_Cereri", idUrm, Convert.ToInt32(Session["UserId"] ?? -99), Convert.ToInt32(Session["User_Marca"] ?? -99));

        //            #endregion
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //    return "Proces finalizat cu succes.";
        //}



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
                        //cmbSubDept.Value = null;
                        break;
                    case "cmbSub":
                        cmbFil.Value = null;
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        //cmbSubDept.Value = null;
                        break;
                    case "cmbFil":
                        cmbSec.Value = null;
                        cmbDept.Value = null;
                        //cmbSubDept.Value = null;
                        break;
                    case "cmbSec":
                        cmbDept.Value = null;
                        //cmbSubDept.Value = null;
                        break;
                    case "cmbDept":
                        //cmbSubDept.Value = null;
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
                HtmlGenericControl divRow = new HtmlGenericControl("div");
                divRow.Attributes["class"] = "row";
                HtmlGenericControl divCol = new HtmlGenericControl("div");
                divCol.Attributes["class"] = "col-md-12";

                DataTable dt = General.IncarcaDT($@"SELECT * FROM ""Org_tblConfig"" WHERE COALESCE(""Vizibil"",0)=1", null);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
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
                }

                divRow.Controls.Add(divCol);
                divExtra.Controls.Add(divRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaBeneficiile(DataRow dr)
        {
            try
            {
                string strSql = $@"SELECT B.""Id"" AS ""IdCateg"", B.""Denumire"" AS ""Eticheta"", C.""Id"", C.""Denumire"", B.""OrdineInPosturi""
                    FROM ""Admin_Arii"" A
                    INNER JOIN ""Admin_Categorii"" B ON A.""Id"" = B.""IdArie""
                    INNER JOIN ""Admin_Obiecte"" C ON B.""Id"" = C.""IdCategorie""
                    WHERE (A.""Denumire"" = 'Beneficii' OR A.""Denumire"" = 'atribute posturi') AND B.""OrdineInPosturi"" IS NOT NULL AND ""OrdineInPosturi"" <> 0
                    ORDER BY B.""OrdineInPosturi"", B.""Id"", B.""Denumire"" ";
                DataTable dtBen = General.IncarcaDT(strSql, null);

                HtmlGenericControl divRow = new HtmlGenericControl("div");
                divRow.Attributes["class"] = "row";

                DataTable dt = dtBen.DefaultView.ToTable(true, new string[] { "IdCateg", "Eticheta" });
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HtmlGenericControl divCol = new HtmlGenericControl("div");
                    divCol.Attributes["class"] = "col-md-6";

                    HtmlGenericControl lbl = new HtmlGenericControl("label");
                    lbl.Style["width"] = "100%";
                    lbl.Style["margin-top"] = "20px";
                    lbl.InnerText = Dami.TraduCuvant(General.Nz(dt.Rows[i]["Eticheta"], "").ToString());

                    ASPxComboBox cmb = new ASPxComboBox();
                    cmb.ID = "cmbBenef" + (i + 1).ToString();
                    cmb.Width = Unit.Pixel(300);
                    cmb.AutoPostBack = false;
                    cmb.AllowNull = true;
                    cmb.ValueField = "Id";
                    cmb.ValueType = typeof(System.Int32);
                    cmb.TextField = "Denumire";
                    DataView view = dtBen.DefaultView;
                    view.RowFilter = "IdCateg=" + Convert.ToInt32(General.Nz(dt.Rows[i]["IdCateg"], -99));
                    cmb.DataSource = view;
                    cmb.DataBind();

                    if (dr != null)
                        cmb.Value = dr["IdBeneficiu" + (i + 1).ToString()];

                    divCol.Controls.Add(lbl);
                    divCol.Controls.Add(cmb);

                    divRow.Controls.Add(divCol);
                }

                divBenef.Controls.Add(divRow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void AdaugaDosar()
        {
            try
            {
                //<dx:ASPxCheckBoxList ID="checkBoxList" runat="server" DataSourceID="ProgLanguages"
                //    ValueField="ID" TextField="Name" RepeatColumns="4" RepeatLayout="Flow" >
                //    <CaptionSettings Position="Top" />
                //</dx:ASPxCheckBoxList>

                DataTable dtTab = General.IncarcaDT($@"
                    SELECT C.""Id"", C.""Denumire"", CASE WHEN D.""IdObiect"" IS NULL THEN 0 ELSE 1 END AS ""Bifat""
                    FROM ""Admin_Arii"" A
                    INNER JOIN ""Admin_Categorii"" B ON A.""Id""=B.""IdArie""
                    INNER JOIN ""Admin_Obiecte"" C ON B.""Id""=C.""IdCategorie""
                    LEFT JOIN ""Org_PosturiDosar"" D ON C.""Id"" = D.""IdObiect""
                    WHERE A.""Id"" = (SELECT COALESCE(""Valoare"",'') FROM ""tblParametrii"" WHERE ""Nume""='ArieTabDosarPersonalDinPersonal')
                    AND B.""Denumire""='Documente Post'", null);

                lstDosar.DataSource = dtTab;
                lstDosar.DataBind();

                //ASPxCheckBoxList lst = new ASPxCheckBoxList();
                //lst.ID = "lstDosar";
                //lst.TextField = "Denumire";
                //lst.ValueField = "Id";
                //lst.RepeatColumns = 1;
                //lst.RepeatDirection = RepeatDirection.Vertical;
                //lst.RepeatLayout = RepeatLayout.Table;
                //lst.DataSource = dtTab;
                //lst.DataBind();

                //divDosar.Controls.Add(lst);
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
    }
}