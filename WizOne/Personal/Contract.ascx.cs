using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Personal
{
    public partial class Contract : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            Contract_DataList.DataSource = table;
            Contract_DataList.DataBind();

            ASPxTextBox txtZile = Contract_DataList.Items[0].FindControl("txtNrZile") as ASPxTextBox;
            ASPxTextBox txtLuni = Contract_DataList.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
            ASPxDateEdit deDeLa = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            ASPxDateEdit deLa = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
            ASPxDateEdit deTermenRevisal = Contract_DataList.Items[0].FindControl("deTermenRevisal") as ASPxDateEdit;
            deTermenRevisal.Value = SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));

            int nrLuni = 0, nrZile = 0;

            CalculLuniSiZile(Convert.ToDateTime(deDeLa.Date), Convert.ToDateTime(deLa.Date), out nrLuni, out nrZile);
            txtZile.Value = nrZile;
            txtLuni.Value = nrLuni;

            ASPxTextBox txtVechCompAni = Contract_DataList.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
            ASPxTextBox txtVechCompLuni = Contract_DataList.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaAni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaLuni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;

            ASPxComboBox cmbIntervRepTimpMunca = Contract_DataList.Items[0].FindControl("cmbIntRepTimpMunca") as ASPxComboBox;
            ASPxComboBox cmbMotivScutit = Contract_DataList.Items[0].FindControl("cmbMotivScutit") as ASPxComboBox;
            ASPxComboBox cmbMotivScutitCAS = Contract_DataList.Items[0].FindControl("cmbMotivScutitCAS") as ASPxComboBox;
            ASPxCheckBox chkScutitCAS = Contract_DataList.Items[0].FindControl("chkScutitCAS") as ASPxCheckBox;
            ASPxCheckBox chkConstr = Contract_DataList.Items[0].FindControl("chkConstr") as ASPxCheckBox;

            ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;

            ASPxComboBox cmbTipAngajat = Contract_DataList.Items[0].FindControl("cmbTipAng") as ASPxComboBox;

            if (ds.Tables[1].Rows[0]["F100643"] != null && ds.Tables[1].Rows[0]["F100643"].ToString().Length >= 4)
            {
                txtVechCompAni.Text = ds.Tables[1].Rows[0]["F100643"].ToString().Substring(0, 2);
                txtVechCompLuni.Text = ds.Tables[1].Rows[0]["F100643"].ToString().Substring(2, 2);
            }
            else
            {
                txtVechCompAni.Text = "00";
                txtVechCompLuni.Text = "00";
            }

            if (ds.Tables[1].Rows[0]["F100644"] != null && ds.Tables[1].Rows[0]["F100644"].ToString().Length >= 4)
            {
                txtVechCarteMuncaAni.Text = ds.Tables[1].Rows[0]["F100644"].ToString().Substring(0, 2);
                txtVechCarteMuncaLuni.Text = ds.Tables[1].Rows[0]["F100644"].ToString().Substring(2, 2);
            }
            else
            {
                txtVechCarteMuncaAni.Text = "00";
                txtVechCarteMuncaLuni.Text = "00";
            }


          
            if (cmbTipAngajat.Value == null || Convert.ToInt32(cmbTipAngajat.Value.ToString()) == 0)
            {
                //cmbIntervRepTimpMunca.SelectedIndex = 0;
                cmbIntervRepTimpMunca.ClientEnabled = false;
            }
            if (cmbMotivScutit.Value == null || Convert.ToInt32(cmbMotivScutit.Value.ToString()) == 0)
            {
                //cmbMotivScutit.SelectedIndex = 0;
                cmbMotivScutit.ClientEnabled = false;
            }
            if (cmbMotivScutitCAS.Value == null ||Convert.ToInt32(cmbMotivScutitCAS.Value.ToString()) == 0)
            {
                chkScutitCAS.Checked = false;
                cmbMotivScutitCAS.ClientEnabled = false;
            }
            else
            {
                chkScutitCAS.Checked = true;
                cmbMotivScutitCAS.ClientEnabled = true;
            }

            ASPxComboBox cmbTimpPartial = Contract_DataList.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
            ASPxComboBox cmbDurTimpMunca = Contract_DataList.Items[0].FindControl("cmbDurTimpMunca") as ASPxComboBox;
            ASPxComboBox cmbTipNorma = Contract_DataList.Items[0].FindControl("cmbTipNorma") as ASPxComboBox;
            ASPxComboBox cmbIntRepTimpMunca = Contract_DataList.Items[0].FindControl("cmbIntRepTimpMunca") as ASPxComboBox;
            ASPxTextBox txtNrOre = Contract_DataList.Items[0].FindControl("txtNrOre") as ASPxTextBox;
            ASPxTextBox txtSalariu = Contract_DataList.Items[0].FindControl("txtSalariu") as ASPxTextBox;
            if (!IsPostBack)
            {
                //Radu 18.09.2019
                ObjectDataSource cmbTimpPartialDataSource = cmbTimpPartial.NamingContainer.FindControl("dsTP") as ObjectDataSource;
                cmbTimpPartialDataSource.SelectParameters.Clear();
                cmbTimpPartialDataSource.SelectParameters.Add("tip", ds.Tables[0].Rows[0]["F10010"].ToString());
                cmbTimpPartial.DataBindItems();

                cmbDurTimpMunca.DataSource = General.GetDurataTimpMunca(ds.Tables[0].Rows[0]["F100926"].ToString());
                cmbDurTimpMunca.DataBind();
                //Florin 2019.09.05
                if (General.Nz(ds.Tables[0].Rows[0]["F100927"], "").ToString() != "")
                    cmbDurTimpMunca.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["F100927"]);

                cmbTipNorma.DataSource = General.GetTipNorma(Convert.ToInt32(ds.Tables[0].Rows[0]["F10010"].ToString()) == 0 ? "1" : "2");
                cmbTipNorma.DataBind();
                //Florin 2019.09.05
                if (General.Nz(ds.Tables[0].Rows[0]["F100926"], "").ToString() != "")
                    cmbTipNorma.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["F100926"]);

                if (ds.Tables[0].Rows[0]["F10010"] == null || Convert.ToInt32(ds.Tables[0].Rows[0]["F10010"].ToString()) == 0)
                {
                    cmbIntRepTimpMunca.ClientEnabled = false;
                    txtNrOre.ClientEnabled = false;
                    txtNrOre.Text = "0";
                }

                if (ds.Tables[0].Rows[0]["F100939"] == null || Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 0 || Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 1)
                {
                    txtNrOre.ClientEnabled = false;
                    txtNrOre.Text = "0";
                }
              
                if (ds.Tables[0].Rows[0]["F1009741"] != null && Convert.ToInt32(ds.Tables[0].Rows[0]["F1009741"].ToString()) == 1)
                {
                    deDeLaData.ClientEnabled = false;
                    deLaData.ClientEnabled = false;
                }
                CalculZLP();

                string salariu = Dami.ValoareParam("REVISAL_SAL", "F100699");
                txtSalariu.Text = ds.Tables[0].Rows[0][salariu].ToString();

                CalculCO();

            }
            else
            {
                int tipAng = Convert.ToInt32(ds.Tables[0].Rows[0]["F10010"].ToString());
                if (hfTipAngajat.Contains("TipAng")) tipAng = Convert.ToInt32(General.Nz(hfTipAngajat["TipAng"], -1));
                //Radu 18.09.2019
                ObjectDataSource cmbTimpPartialDataSource = cmbTimpPartial.NamingContainer.FindControl("dsTP") as ObjectDataSource;
                cmbTimpPartialDataSource.SelectParameters.Clear();
                cmbTimpPartialDataSource.SelectParameters.Add("tip", tipAng.ToString());
                cmbTimpPartial.DataBindItems();

                cmbDurTimpMunca.DataSource = General.GetDurataTimpMunca(tipAng == 0 ? "1" : "2");
                cmbDurTimpMunca.DataBind();

                cmbTipNorma.DataSource = General.GetTipNorma(tipAng == 0 ? "1" : "2");
                cmbTipNorma.DataBind();

                if (tipAng == 0)
                {
                    cmbIntRepTimpMunca.ClientEnabled = false;
                    txtNrOre.ClientEnabled = false;
                    txtNrOre.Text = "0";
                }
                else
                {
                    cmbIntRepTimpMunca.ClientEnabled = true;
                    txtNrOre.ClientEnabled = true;
                }

                if (hfIntRepTM.Contains("IntRepTM") && (Convert.ToInt32(General.Nz(hfIntRepTM["IntRepTM"], 0)) == 0 || Convert.ToInt32(General.Nz(hfIntRepTM["IntRepTM"], 0)) == 1))
                {
                    txtNrOre.ClientEnabled = false;
                    txtNrOre.Text = "0";
                }
                else
                    txtNrOre.ClientEnabled = true;
            }

            ASPxComboBox cmbNivelFunctie = Contract_DataList.Items[0].FindControl("cmbNivelFunctie") as ASPxComboBox;
            cmbNivelFunctie.DataSource = General.IncarcaDT("SELECT * FROM \"tblNivelFunctie\" ORDER BY \"Denumire\"", null);
            cmbNivelFunctie.DataBind();

            if (!IsPostBack)
            {
                DataTable dtDTM = General.IncarcaDT("SELECT * FROM F091", null);
                string dtm = "";
                for (int i = 0; i < dtDTM.Rows.Count; i++)
                {
                    dtm += dtDTM.Rows[i]["F09102"].ToString() + "," + dtDTM.Rows[i]["F09103"].ToString() + "," + dtDTM.Rows[i]["F09105"].ToString();
                    if (i < dtDTM.Rows.Count - 1)
                        dtm += ";";
                }
                Session["MP_ComboDTM"] = dtm;

                DataTable dtTN = General.IncarcaDT("SELECT * FROM F092", null);
                string tipN = "";
                for (int i = 0; i < dtTN.Rows.Count; i++)
                {
                    tipN += dtTN.Rows[i]["F09202"].ToString() + "," + dtTN.Rows[i]["F09203"].ToString() ;
                    if (i < dtTN.Rows.Count - 1)
                        tipN += ";";
                }
                Session["MP_ComboTN"] = tipN;

                DataTable dtGrila = General.IncarcaDT("SELECT CAST(F02604 AS INT) AS F02604, CAST(F02610 AS INT) as F02610, CAST(F02611 AS INT) as F02611, CAST(F02615 AS INT) as F02615 FROM F026", null);
                string grila = "";
                for (int i = 0; i < dtGrila.Rows.Count; i++)
                {
                    grila += (dtGrila.Rows[i]["F02604"] as int? ?? 0).ToString() + "," + (dtGrila.Rows[i]["F02610"] as decimal? ?? 0).ToString() + "," + (dtGrila.Rows[i]["F02611"] as decimal? ?? 0).ToString() + "," + (dtGrila.Rows[i]["F02615"] as decimal? ?? 0).ToString();
                    if (i < dtGrila.Rows.Count - 1)
                        grila += ";";
                }
                Session["MP_Grila"] = grila;

                string gr = (ds.Tables[0].Rows[0]["F10072"] as string ?? "0").ToString();
                CalcGrila(gr.Length <= 0 ? "0" : gr);

                string sql = " select DATEDIFF(MONTH, (select convert(nvarchar(4), F01011) + '-' + convert(nvarchar(4), F01012) + '-01' from F010),  '" + DateTime.Now.Year + "-12-31')";
                if (Constante.tipBD == 2)
                    sql = " select trunc(MONTHS_BETWEEN(to_date('31/12/" + DateTime.Now.Year + "', 'DD/MM/YYYY'),  (select to_date('01/' || F01012 || '/' || F01011, 'DD/MM/YYYY') from F010))  ) FROM DUAL";
                DataTable dtDif = General.IncarcaDT(sql, null);
                Session["MP_DiferentaLuni"] = dtDif.Rows[0][0].ToString();

                Session["MP_SalMin"] = Dami.ValoareParam("SAL_MIN", "0");


                DataTable dtFunc = General.GetFunctie();
                if (dtFunc != null && dtFunc.Rows.Count > 0)
                {
                    DataRow[] drFunc = dtFunc.Select("F71802 = " + (ds.Tables[0].Rows[0]["F10071"] as int? ?? 0).ToString());
                    if (drFunc != null && drFunc.Count() > 0 && drFunc[0]["F71813"] != null && drFunc[0]["F71813"].ToString().Length > 0)
                    {
                        cmbNivelFunctie.Value = Convert.ToInt32(drFunc[0]["F71813"].ToString());
                        if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                            CompletareZile(Convert.ToInt32(drFunc[0]["F71813"].ToString()));
                    }
                }

                DataTable dtNvlFunc = General.IncarcaDT("SELECT \"Id\", \"NrZileLucrProba\", \"NrZileCalProba\", \"NrZileDemisie\", \"NrZileConcediere\", \"Conducere\" FROM \"tblNivelFunctie\"", null);
                string nvlFunc = "";
                for (int i = 0; i < dtNvlFunc.Rows.Count; i++)
                {
                    nvlFunc += dtNvlFunc.Rows[i]["Id"].ToString() + "," + (dtNvlFunc.Rows[i]["NrZileLucrProba"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["NrZileCalProba"] as int? ?? 0).ToString() + "," 
                        + (dtNvlFunc.Rows[i]["NrZileDemisie"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["NrZileConcediere"] as int? ?? 0).ToString() + "," + (dtNvlFunc.Rows[i]["Conducere"] as int? ?? 0).ToString();
                    if (i < dtNvlFunc.Rows.Count - 1)
                        nvlFunc += ";";
                }
                Session["MP_NvlFunc"] = nvlFunc;

            }
            
            ASPxRadioButtonList rbCtrRadiat = Contract_DataList.Items[0].FindControl("rbCtrRadiat") as ASPxRadioButtonList;
            rbCtrRadiat.Value = General.Nz(table.Rows[0]["F1001077"], 0).ToString();
            rbCtrRadiat.Items[0].Text = Dami.TraduCuvant(rbCtrRadiat.Items[0].Text);
            rbCtrRadiat.Items[1].Text = Dami.TraduCuvant(rbCtrRadiat.Items[1].Text);

            DataTable dtComp = General.IncarcaDT("SELECT * FROM F002 WHERE F00202 = " + ds.Tables[0].Rows[0]["F10002"].ToString(), null);
            if ((dtComp.Rows[0]["F00287"] != null && dtComp.Rows[0]["F00287"].ToString() == "1") || (dtComp.Rows[0]["F00288"] != null && dtComp.Rows[0]["F00288"].ToString() == "1"))
                chkConstr.ClientEnabled = true;

            ASPxComboBox cmbCOR = Contract_DataList.Items[0].FindControl("cmbCOR") as ASPxComboBox;
            cmbCOR.Value = Convert.ToInt32((ds.Tables[1].Rows[0]["F10098"] == null || ds.Tables[1].Rows[0]["F10098"].ToString().Length <= 0 ? "0" : ds.Tables[1].Rows[0]["F10098"].ToString()));
                        
            ds.Tables[1].Rows[0]["F100935"] = nrLuni;
            ds.Tables[1].Rows[0]["F100936"] = nrZile;
            Session["InformatiaCurentaPersonal"] = ds;

            string[] etichete = new string[64] { "lblNrCtrInt", "lblDataCtrInt", "lblDataAng", "lblTipCtrMunca", "lblDurCtr", "lblDeLaData", "lblLaData", "lblNrLuni", "lblNrZile", "lblPrel", "lblExcIncet","lblCASSAngajat",
                                                 "lblCASSAngajator", "lblSalariu", "lblDataModifSal", "lblCategAng1", "lblCategAng2", "lblLocAnt", "lblLocatieInt", "lblTipAng", "lblTimpPartial", "lblNorma", "lblDataModifNorma",
                                                 "lblTipNorma", "lblDurTimpMunca", "lblRepTimpMunca", "lblIntervRepTimpMunca", "lblNrOre", "lblCOR", "lblDataModifCOR", "lblFunctie", "lblDataModifFunctie", "lblMeserie",
                                                 "lblPerioadaProba", "lblZL", "lblZC", "lblNrZilePreavizDemisie", "lblNrZilePreavizConc", "lblUltimaZiLucr", "lblMotivPlecare", "lblDataPlecarii", "lblDataReintegr", "lblGradInvalid",
                                                 "lblDataValabInvalid", "lblVechimeComp", "lblVechCompAni", "lblVechCompLuni", "lblVechimeCarteMunca", "lblVechCarteMuncaAni", "lblVechCarteMuncaLuni", "lblGrila", "lblZileCOFidel",
                                                 "lblZileCOAnAnt", "lblZileCOCuvAnCrt", "lblZLP", "lblZLPCuv", "lblDataPrimeiAng", "lblMotivScutit", "lblMotivScutitCAS", "lblCtrRadiat", "lblTermenRevisal", "lblNivelFunctie", "lblZileCOAnCrt", "lblGrilaSal"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = Contract_DataList.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }
            
            string[] bife = new string[9] { "chkFunctieBaza",  "chkScutitImp", "chkBifaPensionar", "chkBifaDetasat", "chkCalcDed", "chkScutitCAS", "chkSalMin", "chkConstr", "chkCotaForfetara"};
            for (int i = 0; i < bife.Count(); i++)
            {
                ASPxCheckBox chk = Contract_DataList.Items[0].FindControl(bife[i]) as ASPxCheckBox;
                chk.Text = Dami.TraduCuvant(chk.Text);
            }


            string[] butoane = new string[19] { "btnCtrInt", "btnCtrIntIst", "btnDataAng", "btnDataAngIst", "btnCASS", "btnCASSIst", "btnSalariu", "btnSalariuIst", "btnNorma", "btnNormaIst", "btnCautaCOR", "btnCOR", "btnCORIst",
                                                "btnFunc", "btnFuncIst", "btnMeserie", "btnMeserieIst", "btnMotivPl", "btnMotivPlIst"};
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = Contract_DataList.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }

            if (Dami.ValoareParam("ValidariPersonal") == "1")
            {
                string[] lstTextBox = new string[8] { "txtNrCtrInt", "txtSalariu", "txtPerProbaZL", "txtPerProbaZC", "txtNrZilePreavizDemisie", "txtNrZilePreavizConc",
                                                    "txtZileCOCuvAnCrt", "txtNrOre"};
                for (int i = 0; i < lstTextBox.Count(); i++)
                {
                    ASPxTextBox txt = Contract_DataList.Items[0].FindControl(lstTextBox[i]) as ASPxTextBox;
                    List<int> lst = new List<int>();
                    if (Session["MP_CuloareCampOblig"] != null)
                        lst = Session["MP_CuloareCampOblig"] as List<int>;
                    txt.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                }

                string[] lstDateEdit = new string[4] { "deDataCtrInt", "deDataAng", "deDeLaData", "deLaData" };
                for (int i = 0; i < lstDateEdit.Count(); i++)
                {
                    ASPxDateEdit de = Contract_DataList.Items[0].FindControl(lstDateEdit[i]) as ASPxDateEdit;
                    List<int> lst = new List<int>();
                    if (Session["MP_CuloareCampOblig"] != null)
                        lst = Session["MP_CuloareCampOblig"] as List<int>;
                    de.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                }


                string[] lstComboBox = new string[10] { "cmbTipCtrMunca", "cmbDurCtr", "cmbTipAng", "cmbTimpPartial", "cmbNorma", "cmbTipNorma", "cmbDurTimpMunca", "cmbRepTimpMunca",
                                                "cmbIntRepTimpMunca", "cmbCOR"};
                for (int i = 0; i < lstComboBox.Count(); i++)
                {
                    ASPxComboBox cmb = Contract_DataList.Items[0].FindControl(lstComboBox[i]) as ASPxComboBox;
                    List<int> lst = new List<int>();
                    if (Session["MP_CuloareCampOblig"] != null)
                        lst = Session["MP_CuloareCampOblig"] as List<int>;
                    cmb.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                }
            }

            HtmlGenericControl lgContract = Contract_DataList.Items[0].FindControl("lgContract") as HtmlGenericControl;
            lgContract.InnerText = Dami.TraduCuvant("Contract");
            HtmlGenericControl lgTipM = Contract_DataList.Items[0].FindControl("lgTipM") as HtmlGenericControl;
            lgTipM.InnerText = Dami.TraduCuvant("Tip munca");
            HtmlGenericControl lgPerioada = Contract_DataList.Items[0].FindControl("lgPerioada") as HtmlGenericControl;
            lgPerioada.InnerText = Dami.TraduCuvant("Perioada");
            HtmlGenericControl lgDataInc = Contract_DataList.Items[0].FindControl("lgDataInc") as HtmlGenericControl;
            lgDataInc.InnerText = Dami.TraduCuvant("Data incetare");
            HtmlGenericControl lgSitCOCtr = Contract_DataList.Items[0].FindControl("lgSitCOCtr") as HtmlGenericControl;
            lgSitCOCtr.InnerText = Dami.TraduCuvant("Situatie CO");

            ASPxComboBox cmbPost = Contract_DataList.Items[0].FindControl("cmbPost") as ASPxComboBox;
            if (!IsPostBack)
            {
                //Florin 2020.10.02
                ASPxComboBox cmbFunctie = Contract_DataList.Items[0].FindControl("cmbFunctie") as ASPxComboBox;
                string sqlPost = $@"SELECT ""Id"", ""Denumire"", ""SalariuMin"", ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"" FROM ""Org_Posturi"" WHERE ""IdFunctie""={General.Nz(cmbFunctie.Value, -99)} AND {General.TruncateDate("DataInceput")} <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= {General.TruncateDate("DataSfarsit")}";
                Session["MP_cmbPost"] = General.IncarcaDT(sqlPost);
                cmbPost.DataSource = Session["MP_cmbPost"];
                cmbPost.DataBind();

                General.AflaIdPost();
                cmbPost.Value = Session["MP_IdPost"];
            }
            else if (Contract_pnlCtl.IsCallback) {
                cmbPost.DataSource = Session["MP_cmbPost"];
                cmbPost.DataBind();
            }

            if (Dami.ValoareParam("MP_FolosesteOrganigrama") == "1")
            {
                //Functie
                ASPxComboBox cmbFunctie = Contract_DataList.Items[0].FindControl("cmbFunctie") as ASPxComboBox;
                if (cmbFunctie != null)
                    cmbFunctie.ClientEnabled = false;
                ASPxButton btnFunc = Contract_DataList.Items[0].FindControl("btnFunc") as ASPxButton;
                if (btnFunc != null)
                    btnFunc.ClientEnabled = false;
                ASPxButton btnFuncIst = Contract_DataList.Items[0].FindControl("btnFuncIst") as ASPxButton;
                if (btnFuncIst != null)
                    btnFuncIst.ClientEnabled = false;
                if (cmbNivelFunctie != null)
                    cmbNivelFunctie.ClientEnabled = false;
                ASPxDateEdit deDataModifFunctie = Contract_DataList.Items[0].FindControl("deDataModifFunctie") as ASPxDateEdit;
                if (deDataModifFunctie != null)
                    deDataModifFunctie.ClientEnabled = false;

                //COR
                if (cmbCOR != null)
                    cmbCOR.ClientEnabled = false;
                ASPxButton btnCautaCOR = Contract_DataList.Items[0].FindControl("btnCautaCOR") as ASPxButton;
                if (btnCautaCOR != null)
                    btnCautaCOR.ClientEnabled = false;
                ASPxButton btnCOR = Contract_DataList.Items[0].FindControl("btnCOR") as ASPxButton;
                if (btnCOR != null)
                    btnCOR.ClientEnabled = false;
                ASPxButton btnCORIst = Contract_DataList.Items[0].FindControl("btnCORIst") as ASPxButton;
                if (btnCORIst != null)
                    btnCORIst.ClientEnabled = false;
                ASPxDateEdit deDataModifCOR = Contract_DataList.Items[0].FindControl("deDataModifCOR") as ASPxDateEdit;
                if (deDataModifCOR != null)
                    deDataModifCOR.ClientEnabled = false;
            }

            General.SecuritatePersonal(Contract_DataList, Convert.ToInt32(Session["UserId"].ToString()));
        }

        protected void pnlCtlContract_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;


            switch (param[0])
            {
                case "deDataCtrInt":
                    ASPxDateEdit deDataCtrInt = Contract_DataList.Items[0].FindControl("deDataCtrInt") as ASPxDateEdit;
                    DateTime dataCtr = deDataCtrInt.Date;
                    string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataCtr.Year;
                    if (Constante.tipBD == 2)
                        strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataCtr.Year;
                    DataTable dtHolidays = General.IncarcaDT(strSql, null);
                    bool ziLibera = EsteZiLibera(dataCtr, dtHolidays);
                    if (dataCtr.DayOfWeek.ToString().ToLower() == "saturday" || dataCtr.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                    {
                        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data contract intern este intr-o zi nelucratoare!");
                    }

                    break;
                case "deDataAng":
                    ASPxDateEdit deTermenRevisal = Contract_DataList.Items[0].FindControl("deTermenRevisal") as ASPxDateEdit;
                    ASPxDateEdit deDataAng = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                    deTermenRevisal.Value = SetDataRevisal(deDataAng.Date);

                    if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    {
                        int val = 1;
                        string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TermenDepasireRevisal'";
                        DataTable dt = General.IncarcaDT(sql, null);
                        if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                            val = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (Convert.ToDateTime(deTermenRevisal.Value).Date < DateTime.Now.Date && val == 1)
                            Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                    }

                    DateTime dataAng = deDataAng.Date;
                    strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataAng.Year;
                    if (Constante.tipBD == 2)
                        strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataAng.Year;
                    dtHolidays = General.IncarcaDT(strSql, null);                  
                    ziLibera = EsteZiLibera(dataAng, dtHolidays);
                    if (dataAng.DayOfWeek.ToString().ToLower() == "saturday" || dataAng.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                    {
                        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data angajarii este intr-o zi nelucratoare!");
                    }

                    CalculCO();

                    break;
                case "cmbTipNorma":
                    SetDurataTimpMunca();
                    break;
                case "txtGrila":
                    CalculCO();
                    break;
                case "cmbTimpPartial":
                    DataTable dtZL = General.IncarcaDT("SELECT * FROM F069 WHERE F06904 = (SELECT F01011 FROM F010) AND F06905 = (SELECT F01012 FROM F010)", null);
                    DataTable dtTarife = General.IncarcaDT("SELECT * FROM F011", null);
                    int poz = 0, valoare = 0;
                    int zile_lucratoare_luna = Convert.ToInt32(dtZL.Rows[0]["F06907"].ToString());
                    int ore_lucratoare_luna = zile_lucratoare_luna * Convert.ToInt32(param[1]);
                    bool gasit = false;
                    for (int i = 0; i < dtTarife.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(General.Nz(dtTarife.Rows[i]["F01114"], "0").ToString()) == 1 && Convert.ToDecimal(General.Nz(dtTarife.Rows[i]["F01108"], "0").ToString()) == ore_lucratoare_luna)                        
                        {
                            gasit = true;
                            poz = Convert.ToInt32(dtTarife.Rows[i]["F01104"].ToString());
                            valoare = Convert.ToInt32(dtTarife.Rows[i]["F01105"].ToString());
                            break;
                        }
                    }
                    if (gasit)
                    {
                        string sir = ds.Tables[0].Rows[0]["F10067"].ToString();
                        string sirNou = "";
                        for (int i = 0; i < sir.Length; i++)
                            if (i == poz - 1)
                                sirNou += valoare.ToString();
                            else
                                sirNou += sir[i];

                        ds.Tables[0].Rows[0]["F10067"] = sirNou;
                        ds.Tables[1].Rows[0]["F10067"] = sirNou;
                    }
                    else
                        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu a fost gasit tarif corespunzator normei selectate! Setati tariful manual!");
                    break;
                case "btnCtrInt":
                    ModifAvans((int)Constante.Atribute.ContrIn);
                    break;
                case "btnDataAng":
                    ModifAvans((int)Constante.Atribute.DataAngajarii);
                    break;
                case "btnSalariu":
                    ModifAvans((int)Constante.Atribute.Salariul);
                    break;
                case "btnCASS":
                    ModifAvans((int)Constante.Atribute.CASS);
                    break;
                case "btnNorma":
                    ModifAvans((int)Constante.Atribute.Norma);
                    break;
                case "btnCOR":
                    ModifAvans((int)Constante.Atribute.CodCOR);
                    break;
                case "btnMeserie":
                    ModifAvans((int)Constante.Atribute.Meserie);
                    break;
                case "btnFunc":
                    ModifAvans((int)Constante.Atribute.Functie);
                    break;
                case "btnMotivPl":
                    ModifAvans((int)Constante.Atribute.MotivPlecare);
                    break;
                case "btnSalariuIst":
                    //GoToIstoric((int)Constante.Atribute.Salariul);
                    break;
                case "btnCautaCOR":
                    List<object> lst = Session["CodCORSelectat"] as List<object>;
                    if (lst != null && lst.Count > 0)
                    {
                        object[] codCOR = lst[0] as object[];
                        Session["CodCORSelectat"] = null;
                        ASPxComboBox cmbCOR = Contract_DataList.Items[0].FindControl("cmbCOR") as ASPxComboBox;
                        cmbCOR.Value = Convert.ToInt32(codCOR[0].ToString());
                        ds.Tables[0].Rows[0]["F10098"] = codCOR[0];
                        ds.Tables[1].Rows[0]["F10098"] = codCOR[0];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "cmbPost":
                    {
                        ASPxComboBox cmbPost = Contract_DataList.Items[0].FindControl("cmbPost") as ASPxComboBox;
                        Session["MP_IdPost"] = cmbPost.Value;
                        Contract_pnlCtl.JSProperties["cpControl"] = "cmbPost";
                        DataTable dtPost = cmbPost.DataSource as DataTable;
                        DataRow drPost = null;
                        if (dtPost != null)
                        {
                            DataRow[] arr = dtPost.Select("Id=" + cmbPost.Value);
                            if (arr.Count() > 0)
                                drPost = arr[0];
                        }

                        if (drPost != null)
                        {
                            Session["MP_SalariulMinPost"] = drPost["SalariuMin"];
                            General.AdaugaBeneficiile(ref ds, Session["Marca"], drPost);
                        }

                        General.AdaugaDosar(ref ds, Session["Marca"]);
                    }
                    break;
                case "cmbFunctie":
                    {
                        //Florin 2020.10.02
                        ASPxComboBox cmbPost = Contract_DataList.Items[0].FindControl("cmbPost") as ASPxComboBox;
                        ASPxComboBox cmbFunctie = Contract_DataList.Items[0].FindControl("cmbFunctie") as ASPxComboBox;
                        string sqlPost = $@"SELECT ""Id"", ""Denumire"", ""SalariuMin"", ""IdBeneficiu1"", ""IdBeneficiu2"", ""IdBeneficiu3"", ""IdBeneficiu4"", ""IdBeneficiu5"", ""IdBeneficiu6"", ""IdBeneficiu7"", ""IdBeneficiu8"", ""IdBeneficiu9"", ""IdBeneficiu10"" FROM ""Org_Posturi"" WHERE ""IdFunctie""={General.Nz(cmbFunctie.Value, -99)} AND {General.TruncateDate("DataInceput")} <= {General.CurrentDate(true)} AND {General.CurrentDate(true)} <= {General.TruncateDate("DataSfarsit")}";
                        Session["MP_cmbPost"] = General.IncarcaDT(sqlPost);
                        cmbPost.DataSource = Session["MP_cmbPost"];
                        cmbPost.DataBind();
                        cmbPost.Value = null;
                    }
                    break;
            }
        }

        protected void CalcGrila(string grila)
        {
            ASPxTextBox txtVechimeCarte = Contract_DataList.Items[0].FindControl("txtVechimeCarte") as ASPxTextBox;
            ASPxTextBox txtZileCOCuvAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOCuvAnCrt") as ASPxTextBox;
            string vechime = "0000";
            if (txtVechimeCarte.Text != null && txtVechimeCarte.Text.Length >= 4)
            {
                vechime = txtVechimeCarte.Text;
            }

            string calcLuni = "ISNULL(convert(int, substring('" + vechime + "', 1, 2)), 0) * 12 + ISNULL(convert(int, substring('" + vechime + "', 3, 2)), 0)";
            if (Constante.tipBD == 2)
                calcLuni = "nvl(to_number(substr('" + vechime + "',1,2)),0) *12 + nvl(to_number(substr('" + vechime + "', 3, 2)), 0)";

            string sql = "select  Convert(int, F02615) from  "
                + " F026 c where convert(int, " + grila + ") = c.F02604 and(convert(int, c.F02610 / 100) * 12) <= " + calcLuni + " and " + calcLuni + " < (convert(int, c.F02611 / 100) * 12) ";               
            if (Constante.tipBD == 2)
                sql = "select TO_NUMBER(TRUNC(F02615))  from  "
                   + " F026 c where " + grila + " = c.F02604 and(to_number(c.F02610 / 100) * 12) <= " + calcLuni + " and " + calcLuni + " < (to_number(c.F02611 / 100) * 12) ";

            DataTable dtGrila = General.IncarcaDT(sql, null);
        }
        


        protected void SetDurataTimpMunca()
        {
            ASPxComboBox cmbDurTimpMunca = Contract_DataList.Items[0].FindControl("cmbDurTimpMunca") as ASPxComboBox;
            ASPxComboBox cmbTipNorma = Contract_DataList.Items[0].FindControl("cmbTipNorma") as ASPxComboBox;

            ObjectDataSource cmbDTMDataSource = cmbDurTimpMunca.NamingContainer.FindControl("dsDTM") as ObjectDataSource;

            cmbDTMDataSource.SelectParameters.Clear();
            cmbDTMDataSource.SelectParameters.Add("param", cmbTipNorma.SelectedIndex.ToString()); 
            cmbDurTimpMunca.DataBindItems();

        }

        public DateTime SetDataRevisal(DateTime data)
        {

            string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + data.Year + " UNION SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + (data.Year - 1).ToString();
            if (Constante.tipBD == 2)
                strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + data.Year + " UNION SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY)  = " + (data.Year - 1).ToString();
            DataTable dtHolidays = General.IncarcaDT(strSql, null);
            DateTime dataRevisal = data.AddDays(-1);
            bool ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
            while (dataRevisal.DayOfWeek.ToString().ToLower() == "saturday" || dataRevisal.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
            {
                dataRevisal = dataRevisal.AddDays(-1);
                ziLibera = EsteZiLibera(dataRevisal, dtHolidays);
            }

            return dataRevisal;
        }

        private bool EsteZiLibera(DateTime data, DataTable dtHolidays)
        {
            bool ziLibera = false;
            for (int z = 0; z < dtHolidays.Rows.Count; z++)
                if (Convert.ToDateTime(dtHolidays.Rows[z][0].ToString()) == data)
                {
                    ziLibera = true;
                    break;
                }
            return ziLibera;
        }

        private void VerificaNrLuni()
        {
            try
            {
                ASPxComboBox cmbDurataContract = Contract_DataList.Items[0].FindControl("cmbDurCtr") as ASPxComboBox;
                ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deDataAngajarii = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
                ASPxDateEdit deDataPlecarii = Contract_DataList.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
                ASPxDateEdit deUltZiLucr = Contract_DataList.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
                ASPxTextBox txtZile = Contract_DataList.Items[0].FindControl("txtNrZile") as ASPxTextBox;
                ASPxTextBox txtLuni = Contract_DataList.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
                ASPxTextBox txtNrCtrIntern = Contract_DataList.Items[0].FindControl("txtNrCtrInt") as ASPxTextBox;

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                if (Convert.ToInt32(cmbDurataContract.SelectedItem.Value) == 1)
                {
                    deDeLaData.Enabled = false;
                    deDeLaData.Value = new DateTime(2100, 1, 1);
                    deLaData.Enabled = false;
                    deLaData.Value = new DateTime(2100, 1, 1);

                    ds.Tables[0].Rows[0]["F100933"] = new DateTime(2100, 1, 1);
                    ds.Tables[1].Rows[0]["F100933"] = new DateTime(2100, 1, 1);
                    ds.Tables[0].Rows[0]["F100934"] = new DateTime(2100, 1, 1);
                    ds.Tables[1].Rows[0]["F100934"] = new DateTime(2100, 1, 1);


                    deUltZiLucr.Enabled = false;
                    deUltZiLucr.Value = new DateTime(2100, 1, 1);
                    deDataPlecarii.Enabled = false;
                    deDataPlecarii.Value = new DateTime(2100, 1, 1);

                    ds.Tables[0].Rows[0]["F100993"] = new DateTime(2100, 1, 1);
                    ds.Tables[0].Rows[0]["F10023"] = new DateTime(2100, 1, 1);
                    ds.Tables[1].Rows[0]["F100993"] = new DateTime(2100, 1, 1);
                    ds.Tables[1].Rows[0]["F10023"] = new DateTime(2100, 1, 1);

                    Session["InformatiaCurentaPersonal"] = ds;

                    txtZile.Text = "";
                    txtLuni.Text = "";
                }
                else if (Convert.ToInt32(cmbDurataContract.Value) == 2)
                {

                    deUltZiLucr.Enabled = true;
                    deDataPlecarii.Enabled = true;

                    DataTable dt = General.IncarcaDT("SELECT * FROM F095 WHERE F09503 = " + Session["Marca"].ToString(), null);

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        deDeLaData.Value = Convert.ToDateTime(deDataAngajarii.Value);
                        ds.Tables[0].Rows[0]["F100933"] = Convert.ToDateTime(deDataAngajarii.Value).Date;
                        ds.Tables[1].Rows[0]["F100933"] = Convert.ToDateTime(deDataAngajarii.Value).Date;
                        Session["InformatiaCurentaPersonal"] = ds;
                    }

                    deDeLaData.Enabled = true;
                    deLaData.Enabled = true;

                    if (deDeLaData.Value != null && deLaData.Value != null)
                    {
                        deDeLaData.Enabled = true;
                        deLaData.Enabled = true;

                        if (deDeLaData.Value == null)
                            Contract_pnlCtl.JSProperties["cpAlertMessage"] =  Dami.TraduCuvant("Completati perioada pentru contractul pe perioada determinata!");

                        if (Convert.ToInt32(txtLuni.Text) > 36 || (Convert.ToInt32(txtLuni.Text) == 36 && Convert.ToInt32(txtZile.Text) > 0))
                            Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Durata maxima a unui contract pe perioada determinata nu poate depasi 36 de luni");
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(deLaData.Value) - Convert.ToDateTime(deDeLaData.Value);

                            if (Convert.ToInt32(Session["Marca"].ToString()) != -99 && txtNrCtrIntern.Text != "")
                            {
                                string mesaj = General.NumarLuniContract(Convert.ToInt32(Session["Marca"].ToString()), txtNrCtrIntern.Text, Convert.ToDateTime(deDeLaData.Value), Convert.ToDateTime(deLaData.Value), Convert.ToInt32(ts.TotalDays + 1));
                                if (mesaj != null && mesaj != "") Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj).Substring(1);   
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
               // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        public void CalculLuniSiZile(DateTime dataStart, DateTime dataSfarsit, out int nrLuni, out int nrZile)
        {
            DateTime odtT1, odtT2, odtD;
            List<int> arNrZileInLuna = new List<int>();

            // determin nr zile calendaristice in luna:
            odtT1 = new DateTime(dataStart.Year, dataStart.Month, 1, 0, 0, 0);
            odtT2 = new DateTime(dataSfarsit.Year, dataSfarsit.Month, 1, 0, 0, 0);
            for (DateTime odtDt = odtT1; odtDt <= odtT2;)
            {
                odtD = new DateTime(
                    odtDt.Month == 12 ? odtDt.Year + 1 : odtDt.Year,
                    odtDt.Month == 12 ? 1 : odtDt.Month + 1, 1, 0, 0, 0);

                int odtsDf = (int)(odtD - odtDt).TotalDays;
                arNrZileInLuna.Add((int)(odtsDf));
                odtDt = odtD;
            }

            nrLuni = 0;
            nrZile = 0;
            if (dataSfarsit.Date != new DateTime(2100, 1, 1, 0, 0, 0) && dataStart.Date != new DateTime(2100, 1, 1, 0, 0, 0))
                nrZile = (int)(dataSfarsit.Date - dataStart.Date).TotalDays + 1;

            for (int nI = 0; nI < arNrZileInLuna.Count && nrZile >= arNrZileInLuna[nI]; nI++)
            {
                nrZile -= arNrZileInLuna[nI];
                nrLuni++;
            }

        }

        private void ModifAvans(int atribut)
        {
            string strRol = Avs.Cereri.DamiRol(Convert.ToInt32(General.Nz(Session["Marca"], -99)), atribut);
            if (strRol == "")
            {
                if (Page.IsCallback)
                    Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie");
                else
                    MessageBox.Show("Nu aveti drepturi pentru aceasta operatie", MessageBox.icoWarning, "Atentie");
            }
            else
            {
                string url = "~/Avs/Cereri.aspx";
                Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString() + ";" + strRol;
                Session["MP_Avans"] = "true";
                Session["MP_Avans_Tab"] = "Contract";
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
        }

        private void GoToIstoric(int atribut)
        {
            Session["IstoricDateContract"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Response.Write("  <script language='javascript'> window.open('Sablon.aspx','','width=1020, Height=720,fullscreen=1,location=0,scrollbars=1,menubar=1,toolbar=1'); </script>");
        }

        //Florin 2019.07.01
        //Functie asemanatoare cu cea din General.CalculCO cu diferenta ca nu sunt date in F100, F10022 se trimite ca valoare si se intorc datele pt a fi puse pe forma

        private void CalculCO()
        {
            try
            {
                ASPxDateEdit deDataAng = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                if (deDataAng == null) return;

                ASPxTextBox txtGrila = Contract_DataList.Items[0].FindControl("txtGrila") as ASPxTextBox;
                ASPxTextBox txtVechCarteMuncaAni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
                ASPxTextBox txtVechCarteMuncaLuni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
                ASPxTextBox txtVechCompAni = Contract_DataList.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
                ASPxTextBox txtVechCompLuni = Contract_DataList.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
                if (txtGrila == null) return;

                int an = DateTime.Now.Year;
                DateTime f10022 = deDataAng.Date;
                string f10072 = txtGrila.Text;

                string vechime = "";

                string paramVechime = Dami.ValoareParam("MP_VechimeCalculCO", "1");

                if (paramVechime == "1")
                    vechime = (txtVechCarteMuncaAni.Text.Length > 0 ? txtVechCarteMuncaAni.Text.PadLeft(2, '0') : "00") + (txtVechCarteMuncaLuni.Text.Length > 0 ? txtVechCarteMuncaLuni.Text.PadLeft(2, '0') : "00"); 
                else
                    vechime = (txtVechCompAni.Text.Length > 0 ? txtVechCompAni.Text.PadLeft(2, '0') : "00") + (txtVechCompLuni.Text.Length > 0 ? txtVechCompLuni.Text.PadLeft(2, '0') : "00"); 


                string dtInc = an.ToString() + "-01-01";
                string dtSf = an.ToString() + "-12-31";

                string filtruIns = " AND F10003=" + Session["Marca"].ToString();
                string f10003 = Session["Marca"].ToString();
                string strSql ="select * from calculCO(" + f10003 + ", CONVERT(date,'" + an + "-12-31'), 1, " + f10072 + ")";
                DataTable dtCO = General.IncarcaDT(strSql, null);

                if (dtCO != null && dtCO.Rows.Count > 0)
                {
                    //F100642
                    //txtZileCOCuvAnCrt
                    ASPxTextBox txtZileCOCuvAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOCuvAnCrt") as ASPxTextBox;
                    if (txtZileCOCuvAnCrt != null)
                        txtZileCOCuvAnCrt.Value = General.Nz(dtCO.Rows[0]["ZileCuveniteAn"], txtZileCOCuvAnCrt.Text).ToString();

                    //F100995
                    //txtZileCOAnCrt
                    ASPxTextBox txtZileCOAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOAnCrt") as ASPxTextBox;
                    if (txtZileCOAnCrt != null)
                        txtZileCOAnCrt.Value = General.Nz(dtCO.Rows[0]["ZileCuvenite"], "").ToString();

                    //F100996
                    //txtZileCOAnAnt
                    ASPxTextBox txtZileCOAnAnt = Contract_DataList.Items[0].FindControl("txtZileCOAnAnt") as ASPxTextBox;
                    if (txtZileCOAnAnt != null)
                        txtZileCOAnAnt.Value = General.Nz(dtCO.Rows[0]["ZileAnPrecedent"], "").ToString();
                }
                else
                    CalcGrila(f10072);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void CalculZLP()
        {
            try
            {
                ASPxDateEdit deDataAng = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                if (deDataAng == null) return;           

                int an = DateTime.Now.Year;
                DateTime f10022 = deDataAng.Date; 
             
                string dtInc = an.ToString() + "-01-01";
                string dtSf = an.ToString() + "-12-31";

                string filtruIns = " AND F10003=" + Session["Marca"].ToString();
                string f10003 = Session["Marca"].ToString();

                bool esteNou = false;
                if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    esteNou = true;

                Absente.ZLP pagZLP = new Absente.ZLP();

                pagZLP.CalculZLP(an, f10003, filtruIns, f10022, esteNou);

                int nrZLP = 0;
                DataTable dtParam = General.IncarcaDT(Constante.tipBD == 1 ? "SELECT ISNULL((SELECT CONVERT(int, Valoare) FROM tblParametrii WHERE Nume = 'NumarZileLiberePlatite'), 0)" 
                    : "SELECT nvl((SELECT TO_NUMBER(\"Valoare\") FROM \"tblParametrii\" WHERE \"Nume\" = 'NumarZileLiberePlatite'), 0) from dual", null);
                if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                    nrZLP = Convert.ToInt32(dtParam.Rows[0][0].ToString());

                DataRow dtZLP = General.IncarcaDR(@"SELECT * FROM ""Ptj_tblZLP"" WHERE F10003=@1 AND ""An""=@2", new object[] { f10003, an });
                if (dtZLP != null)
                {                    
                    ASPxTextBox txtZLP = Contract_DataList.Items[0].FindControl("txtZLP") as ASPxTextBox;
                    if (txtZLP != null)
                        txtZLP.Value = General.Nz(dtZLP["Cuvenite"], nrZLP).ToString();
               
                    ASPxTextBox txtZLPCuv = Contract_DataList.Items[0].FindControl("txtZLPCuv") as ASPxTextBox;
                    if (txtZLPCuv != null)
                        txtZLPCuv.Value = General.Nz(dtZLP["CuveniteAn"], "").ToString();          
                }
                else
                {
                    ASPxTextBox txtZLP = Contract_DataList.Items[0].FindControl("txtZLP") as ASPxTextBox;
                    if (txtZLP != null)
                        txtZLP.Value = nrZLP.ToString();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
            }
        }

        private void CompletareZile(int nivel)
        {
            string sql = "SELECT * FROM \"tblNivelFunctie\" WHERE Id = " + nivel;
            DataTable dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
            {
                ASPxTextBox txtPerProbaZL = Contract_DataList.Items[0].FindControl("txtPerProbaZL") as ASPxTextBox;
                ASPxTextBox txtPerProbaZC = Contract_DataList.Items[0].FindControl("txtPerProbaZC") as ASPxTextBox;
                ASPxTextBox txtNrZilePreavizDemisie = Contract_DataList.Items[0].FindControl("txtNrZilePreavizDemisie") as ASPxTextBox;
                ASPxTextBox txtNrZilePreavizConc = Contract_DataList.Items[0].FindControl("txtNrZilePreavizConc") as ASPxTextBox;

                txtPerProbaZL.Text = (dt.Rows[0]["NrZileLucrProba"] as int? ?? 0).ToString();
                txtPerProbaZC.Text = (dt.Rows[0]["NrZileCalProba"] as int? ?? 0).ToString();
                txtNrZilePreavizDemisie.Text = (dt.Rows[0]["NrZileDemisie"] as int? ?? 0).ToString();
                txtNrZilePreavizConc.Text = (dt.Rows[0]["NrZileConcediere"] as int? ?? 0).ToString();
            }
        }
    
    }
}