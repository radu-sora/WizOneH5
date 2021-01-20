using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace WizOne.Personal
{
    public partial class Contract : System.Web.UI.UserControl
    {
        //decimal timpPartial = 0;
        //bool inactiveazaDeLaLa = false;


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
            //txtZile.Text = Convert.ToInt32((Convert.ToDateTime(deLa.Date) - Convert.ToDateTime(deDeLa.Date)).TotalDays).ToString();
            //txtLuni.Text = (((Convert.ToDateTime(deLa.Date).Year - Convert.ToDateTime(deDeLa.Date).Year) * 12) + Convert.ToDateTime(deLa.Date).Month - Convert.ToDateTime(deDeLa.Date).Month).ToString();
            //cmbTipAngajat_SelectedIndexChanged(ds.Tables[1].Rows[0]["F10043"].ToString());

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
                //cmbTimpPartial.DataSource = General.GetTimpPartial(Convert.ToInt32(ds.Tables[0].Rows[0]["F10010"].ToString()));
                //cmbTimpPartial.DataBind();

                //Radu 18.09.2019
                ObjectDataSource cmbTimpPartialDataSource = cmbTimpPartial.NamingContainer.FindControl("dsTP") as ObjectDataSource;
                cmbTimpPartialDataSource.SelectParameters.Clear();
                cmbTimpPartialDataSource.SelectParameters.Add("tip", ds.Tables[0].Rows[0]["F10010"].ToString());
                cmbTimpPartial.DataBindItems();
                //Florin 2019.09.05
                //if (General.Nz(ds.Tables[0].Rows[0]["F10043"],"").ToString() != "")
                //    cmbTimpPartial.Value = Convert.ToInt32(ds.Tables[0].Rows[0]["F10043"]);

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

                //cmbTimpPartial.DataSource = General.GetTimpPartial(tipAng);
                //cmbTimpPartial.DataBind();

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
            
            //SetDurataTimpMunca();

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
                string[] lstTextBox = new string[7] { "txtNrCtrInt", "txtSalariu", "txtPerProbaZL", "txtPerProbaZC", "txtNrZilePreavizDemisie", "txtNrZilePreavizConc", "txtNrOre"};   //"txtZileCOCuvAnCrt",
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
                //Florin 2019.05.31
                //if (!IsPostBack)
                //{
                //    SetariNorma();
                //}

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

            General.SecuritatePersonal(Contract_DataList, Convert.ToInt32(Session["UserId"].ToString()));

        }

        //private void SetariNorma()
        //{
        //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //    //DateIdentificare dateId = new DateIdentificare();
        //    int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
        //    if (varsta >= 16 && varsta < 18)
        //    {
        //        ASPxComboBox cmbNorma = Contract_DataList.Items[0].FindControl("cmbNorma") as ASPxComboBox;
        //        ASPxComboBox cmbTimpPartial = Contract_DataList.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
        //        ASPxTextBox txtNrOre = Contract_DataList.Items[0].FindControl("txtNrOre") as ASPxTextBox;

        //        cmbNorma.Value = 6;
        //        cmbNorma.ReadOnly = true;
        //        cmbTimpPartial.Value = 6;

        //        if (txtNrOre.Text.Length > 0 && Convert.ToInt32(txtNrOre.Text) > 30)
        //        {
        //            txtNrOre.Text = "";
        //            ds.Tables[0].Rows[0]["F100964"] = DBNull.Value;
        //            ds.Tables[2].Rows[0]["F100964"] = DBNull.Value;
        //        }

        //        ds.Tables[0].Rows[0]["F100973"] = 6;
        //        ds.Tables[1].Rows[0]["F100973"] = 6;
        //        ds.Tables[0].Rows[0]["F10043"] = 6;
        //        ds.Tables[1].Rows[0]["F10043"] = 6;
        //        Session["InformatiaCurentaPersonal"] = ds;
        //    }
        //}

        protected void pnlCtlContract_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

            //string[] data = param[1].Split('.');


            switch (param[0])
            {
                case "deDataCtrInt":
                    ASPxDateEdit deDataCtrInt = Contract_DataList.Items[0].FindControl("deDataCtrInt") as ASPxDateEdit;
                    DateTime dataCtr = deDataCtrInt.Date;
                    //DateTime dataCtr = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
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
                    //deDataCrtIntern_EditValueChanged(ds.Tables[1].Rows[0]["F100986"]);
                    //data = param[1].Split('.');
                    //ds.Tables[0].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    //ds.Tables[1].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    //Session["InformatiaCurentaPersonal"] = ds;
                    ASPxDateEdit deTermenRevisal = Contract_DataList.Items[0].FindControl("deTermenRevisal") as ASPxDateEdit;
                    ASPxDateEdit deDataAng = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                    //deTermenRevisal.Value = SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
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

                    //DateTime dataAng = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
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
                //case "cmbNivelFunctie":
                //    CompletareZile(Convert.ToInt32(param[1]));
                //    break;
                //case "cmbDurCtr":
                //    cmbDurataContract_SelectedIndexChanged();
                //    //ds.Tables[0].Rows[0]["F1009741"] = param[1];
                //    //ds.Tables[1].Rows[0]["F1009741"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDeLaData":
                //    deDeLaData_LostFocus(ds.Tables[1].Rows[0]["F100933"]);
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deLaData":
                //    deLaData_LostFocus(ds.Tables[1].Rows[0]["F100934"]);
                //    //data = param[1].Split('.');
                //    //ds.Tables[0].Rows[0]["F100934"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    //ds.Tables[1].Rows[0]["F100934"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbPrel":
                //    cmbPrel_SelectedIndexChanged();
                //    //ds.Tables[0].Rows[0]["F100938"] = param[1];
                //    //ds.Tables[1].Rows[0]["F100938"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbExcIncet":
                //    ds.Tables[0].Rows[0]["F100929"] = param[1];
                //    ds.Tables[1].Rows[0]["F100929"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbCASSAngajat":
                //    ds.Tables[0].Rows[0]["F1003900"] = param[1];
                //    ds.Tables[1].Rows[0]["F1003900"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbCASSAngajator":
                //    ds.Tables[0].Rows[0]["F1003907"] = param[1];
                //    ds.Tables[1].Rows[0]["F1003907"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtSalariu":
                //    ds.Tables[0].Rows[0]["F100699"] = param[1];
                //    ds.Tables[1].Rows[0]["F100699"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataModifSal":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100991"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100991"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbCategAng1":
                //    ds.Tables[0].Rows[0]["F10061"] = param[1];
                //    ds.Tables[1].Rows[0]["F10061"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbCategAng2":
                //    ds.Tables[0].Rows[0]["F10062"] = param[1];
                //    ds.Tables[1].Rows[0]["F10062"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtLocAnt":
                //    ds.Tables[0].Rows[0]["F10078"] = param[1];
                //    ds.Tables[1].Rows[0]["F10078"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbLocatieInt":
                //    ds.Tables[0].Rows[0]["F100966"] = param[1];
                //    ds.Tables[2].Rows[0]["F100966"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbTipAng":
                //    cmbTipAngajat_SelectedIndexChanged(ds.Tables[1].Rows[0]["F10043"].ToString());
                //    //ds.Tables[0].Rows[0]["F10010"] = param[1];
                //    //ds.Tables[1].Rows[0]["F10010"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbTimpPartial":
                //    //int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                //    //if (varsta >= 16 && varsta < 18 && Convert.ToInt32(param[1]) > 6)
                //    //{
                //    //    Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timp partial invalid (max 6 pentru minori peste 16 ani)!");
                //    //    SetariNorma();
                //    //    return;
                //    //}
                //    //cmbTimpPartial_SelectedIndexChanged();
                //    //ds.Tables[0].Rows[0]["F10043"] = param[1];
                //    //ds.Tables[1].Rows[0]["F10043"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbNorma":
                //    cmbNorma_LostFocus();
                //    //ds.Tables[0].Rows[0]["F100973"] = param[1];
                //    //ds.Tables[1].Rows[0]["F100973"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataModifNorma":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100955"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[2].Rows[0]["F100955"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbTipNorma":
                //    //dateId = new DateIdentificare();
                //    int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                //    if (varsta >= 16 && varsta < 18 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100964"].ToString()) > 30 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2)
                //    {
                //        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 30 pentru minori peste 16 ani)!");
                //        //SetariNorma();
                //        return;
                //    }
                //    if (Convert.ToInt32(ds.Tables[0].Rows[0]["F100926"].ToString()) == 1 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100964"].ToString()) > 40)
                //    {
                //        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 40 pentru norma intreaga)!");
                //        return;
                //    }
                //    ds.Tables[0].Rows[0]["F100926"] = param[1];
                //    ds.Tables[1].Rows[0]["F100926"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbDurTimpMunca":
                //    ds.Tables[0].Rows[0]["F100927"] = param[1];
                //    ds.Tables[1].Rows[0]["F100927"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbRepTimpMunca":
                //    ds.Tables[0].Rows[0]["F100928"] = param[1];
                //    ds.Tables[1].Rows[0]["F100928"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbIntervRepTimpMunca":
                //    cmbIntervalRepartizareTimpMunca_SelectedIndexChanged();
                //    //ds.Tables[0].Rows[0]["F100939"] = param[1];
                //    //ds.Tables[1].Rows[0]["F100939"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtNrOre":
                //    //dateId = new DateIdentificare();
                //    int varsta = Dami.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                //    if (varsta >= 16 && varsta < 18 && Convert.ToInt32(param[1]) > 30 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2)
                //    {
                //        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 30 pentru minori peste 16 ani)!");
                //        //SetariNorma();
                //        return;
                //    }
                //    if (Convert.ToInt32(ds.Tables[0].Rows[0]["F100926"].ToString()) == 1 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2 && Convert.ToInt32(param[1]) > 40)
                //    {
                //        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 40 pentru norma intreaga)!");                       
                //        return;
                //    }

                //    ds.Tables[0].Rows[0]["F100964"] = param[1];
                //    ds.Tables[2].Rows[0]["F100964"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbCOR":
                //    ds.Tables[0].Rows[0]["F10098"] = param[1];
                //    ds.Tables[1].Rows[0]["F10098"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataModifCOR":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100956"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[2].Rows[0]["F100956"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbFunctie":
                //    ds.Tables[0].Rows[0]["F10071"] = param[1];
                //    ds.Tables[1].Rows[0]["F10071"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataModifFunctie":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100992"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100992"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbMeserie":
                //    ds.Tables[0].Rows[0]["F10029"] = param[1];
                //    ds.Tables[1].Rows[0]["F10029"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "chkFunctieBaza":
                //    ds.Tables[0].Rows[0]["F10048"] = (param[1] == "true" ? 1 : 0);
                //    ds.Tables[1].Rows[0]["F10048"] = (param[1] == "true" ? 1 : 0);
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtPerProbaZL":
                //    ds.Tables[0].Rows[0]["F1001063"] = param[1];
                //    ds.Tables[2].Rows[0]["F1001063"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtPerProbaZC":
                //    ds.Tables[0].Rows[0]["F100975"] = param[1];
                //    ds.Tables[1].Rows[0]["F100975"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtNrZilePreavizDemisie":
                //    ds.Tables[0].Rows[0]["F1009742"] = param[1];
                //    ds.Tables[1].Rows[0]["F1009742"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtNrZilePreavizConc":
                //    ds.Tables[0].Rows[0]["F100931"] = param[1];
                //    ds.Tables[1].Rows[0]["F100931"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deUltimaZiLucr":
                //    //Florin 2018.11.12
                //    //deUltZiLucr_LostFocus();
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F10023"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[0].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(1);
                //    ds.Tables[1].Rows[0]["F10023"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(1);
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbMotivPlecare":
                //    ds.Tables[0].Rows[0]["F10025"] = param[1];
                //    ds.Tables[1].Rows[0]["F10025"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataPlecarii":
                //    //Florin 2018.11.12
                //    //deDataPlecarii_EditValueChanged(ds.Tables[1].Rows[0]["F100993"]);
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[0].Rows[0]["F10023"] = (Convert.ToInt32(data[2]) == 2100 ? new DateTime(2100, 1, 1) : new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(-1));
                //    ds.Tables[1].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F10023"] = (Convert.ToInt32(data[2]) == 2100 ? new DateTime(2100, 1, 1) : new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(-1));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataReintegr":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100930"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100930"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "cmbGradInvalid":
                //    cmbGradInvaliditate_SelectedIndexChanged();
                //    //ds.Tables[0].Rows[0]["F10027"] = param[1];
                //    //ds.Tables[1].Rows[0]["F10027"] = param[1];
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataValabInvalid":
                //    data = param[1].Split('.');
                //    ds.Tables[0].Rows[0]["F100271"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    ds.Tables[1].Rows[0]["F100271"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "chkScutitImp":
                //    ds.Tables[0].Rows[0]["F10026"] = (param[1] == "true" ? 1 : 0);
                //    ds.Tables[1].Rows[0]["F10026"] = (param[1] == "true" ? 1 : 0);
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "chkBifaPensionar":
                //    ds.Tables[0].Rows[0]["F10037"] = (param[1] == "true" ? 1 : 0);
                //    ds.Tables[1].Rows[0]["F10037"] = (param[1] == "true" ? 1 : 0);
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "chkBifaDetasat":
                //    ds.Tables[0].Rows[0]["F100954"] = (param[1] == "true" ? 1 : 0);
                //    ds.Tables[2].Rows[0]["F100954"] = (param[1] == "true" ? 1 : 0);
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtVechCompAni":
                //    txtVechimeInCompanieAni_LostFocus();
                //    break;
                //case "txtVechCompLuni":
                //    txtVechimeInCompanieLuni_LostFocus();
                //    break;
                //case "txtVechCarteMuncaAni":
                //    txtVechimePeCarteaMuncaAni_LostFocus();
                //    break;
                //case "txtVechCarteMuncaLuni":
                //    txtVechimePeCarteaMuncaLuni_LostFocus();
                //    break;
                //case "txtGrila":
                //    ds.Tables[0].Rows[0]["F10072"] = param[1];
                //    ds.Tables[1].Rows[0]["F10072"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtZileCOFidel":
                //    ds.Tables[0].Rows[0]["F100640"] = param[1];
                //    ds.Tables[1].Rows[0]["F100640"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtZileCOAnAnt":
                //    ds.Tables[0].Rows[0]["F100641"] = param[1];
                //    ds.Tables[1].Rows[0]["F100641"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "txtZileCOCuvAnCrt":
                //    ds.Tables[0].Rows[0]["F100642"] = param[1];
                //    ds.Tables[1].Rows[0]["F100642"] = param[1];
                //    Session["InformatiaCurentaPersonal"] = ds;
                //    break;
                //case "deDataPrimeiAng":
                //    txtDataPrimeiAngajari_LostFocus();
                //    //data = param[1].Split('.');
                //    //ds.Tables[0].Rows[0]["F1001049"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    //ds.Tables[2].Rows[0]["F1001049"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                //    //Session["InformatiaCurentaPersonal"] = ds;
                //    break;
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
            //string sql = "select a.f10003, Convert(int, F02615) from f100 a left join "
            //    + " (select ISNULL(convert(int, substring('" + vechime + "', 1, 2)), 0) * 12 + ISNULL(convert(int, substring('" + vechime + "', 3, 2)), 0)  as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003 "
            //    + " left join F026 c on convert(int, " + grila + ") = c.F02604 and(convert(int, c.F02610 / 100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int, c.F02611 / 100) * 12) "
            //    + " where a.f10003 = " + Session["Marca"].ToString();
            //if (Constante.tipBD == 2)
            //    sql = "select a.f10003, TO_NUMBER(TRUNC(F02615))  from F100 a "
            //       + " left join(select nvl(to_number(substr('" + vechime + "',1,2)),0) *12 + nvl(to_number(substr('" + vechime + "', 3, 2)), 0) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003 "
            //       + "  left join F026 c on " + grila + " = c.F02604 and(to_number(c.F02610 / 100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611 / 100) * 12) where a.f10003 = " + Session["Marca"].ToString();

            string calcLuni = "ISNULL(convert(int, substring('" + vechime + "', 1, 2)), 0) * 12 + ISNULL(convert(int, substring('" + vechime + "', 3, 2)), 0)";
            if (Constante.tipBD == 2)
                calcLuni = "nvl(to_number(substr('" + vechime + "',1,2)),0) *12 + nvl(to_number(substr('" + vechime + "', 3, 2)), 0)";

            string sql = "select  Convert(int, F02615) from  "
                + " F026 c where convert(int, " + grila + ") = c.F02604 and(convert(int, c.F02610 / 100) * 12) <= " + calcLuni + " and " + calcLuni + " < (convert(int, c.F02611 / 100) * 12) ";               
            if (Constante.tipBD == 2)
                sql = "select TO_NUMBER(TRUNC(F02615))  from  "
                   + " F026 c where " + grila + " = c.F02604 and(to_number(c.F02610 / 100) * 12) <= " + calcLuni + " and " + calcLuni + " < (to_number(c.F02611 / 100) * 12) ";

            DataTable dtGrila = General.IncarcaDT(sql, null);
            //if (dtGrila != null && dtGrila.Rows.Count > 0 && dtGrila.Rows[0][0] != null && dtGrila.Rows[0][0].ToString().Length > 0)
            //    txtZileCOCuvAnCrt.Text = dtGrila.Rows[0][0].ToString();
            //else
            //    txtZileCOCuvAnCrt.Text = "";
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

        //private void cmbTipAngajat_SelectedIndexChanged(string F10043)
        //{
        //    try
        //    {
        //        ASPxComboBox cmbTipAngajat = Contract_DataList.Items[0].FindControl("cmbTipAng") as ASPxComboBox;
        //        ASPxComboBox cmbTimpPartial = Contract_DataList.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
        //        ASPxComboBox cmbTipNorma = Contract_DataList.Items[0].FindControl("cmbTipNorma") as ASPxComboBox;
        //        ASPxComboBox cmbDurTimpMunca = Contract_DataList.Items[0].FindControl("cmbDurTimpMunca") as ASPxComboBox;
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (Convert.ToInt32(cmbTipAngajat.SelectedItem.Value) == 0)
        //        {
        //            if (cmbTimpPartial.SelectedItem == null)
        //                cmbTimpPartial.SelectedIndex = 0;
        //            cmbTimpPartial.SelectedItem.Value = timpPartial > 0 ? timpPartial : Convert.ToDecimal(F10043);
        //            cmbTimpPartial.Enabled = false;
        //        }
        //        else
        //        {
        //            cmbTimpPartial.SelectedIndex = 0;
        //            cmbTimpPartial.Enabled = true;
        //            if (Convert.ToInt32(cmbTipAngajat.SelectedItem.Value) == 2)
        //            {
        //                cmbTipNorma.Value = 2;
        //                cmbDurTimpMunca.Value = 5;
        //                ds.Tables[0].Rows[0]["F100926"] = 2;
        //                ds.Tables[1].Rows[0]["F100926"] = 2;                                            
        //                ds.Tables[0].Rows[0]["F100927"] = 5;
        //                ds.Tables[1].Rows[0]["F100927"] = 5;
        //                Session["InformatiaCurentaPersonal"] = ds;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void cmbIntervalRepartizareTimpMunca_SelectedIndexChanged()
        //{
        //    try
        //    {
        //        ASPxComboBox cmbIntervalRepartizareTimpMunca = Contract_DataList.Items[0].FindControl("cmbIntervRepTimpMunca") as ASPxComboBox;
        //        ASPxTextBox txtNrOreLunaSaptamana = Contract_DataList.Items[0].FindControl("txtNrOre") as ASPxTextBox;
        //        if (Convert.ToInt32(cmbIntervalRepartizareTimpMunca.SelectedItem.Value) == 2 || Convert.ToInt32(cmbIntervalRepartizareTimpMunca.SelectedItem.Value) == 3)
        //            txtNrOreLunaSaptamana.Enabled = true;
        //        else
        //            txtNrOreLunaSaptamana.Enabled = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void cmbDurataContract_SelectedIndexChanged()
        //{
        //    try
        //    {
        //        //VerificaNrLuni();
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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

                    //if (deDeLaData.Value == null || Convert.ToDateTime(deDeLaData.Value) == new DateTime(2100, 1, 1))
                    //{
                        DataTable dt = General.IncarcaDT("SELECT * FROM F095 WHERE F09503 = " + Session["Marca"].ToString(), null);

                        if (dt == null || dt.Rows.Count == 0)
                        {
                            deDeLaData.Value = Convert.ToDateTime(deDataAngajarii.Value);
                            ds.Tables[0].Rows[0]["F100933"] = Convert.ToDateTime(deDataAngajarii.Value).Date;
                            ds.Tables[1].Rows[0]["F100933"] = Convert.ToDateTime(deDataAngajarii.Value).Date;
                            Session["InformatiaCurentaPersonal"] = ds;
                        }

                    //}

                    deDeLaData.Enabled = true;
                    deLaData.Enabled = true;

                    if (deDeLaData.Value != null && deLaData.Value != null)
                    {
                        //inactiveazaDeLaLa = true;

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

        //private void deDataPlecarii_EditValueChanged(object F100993)
        //{
        //    try
        //    {                
        //        ASPxDateEdit deDataAngajarii = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
        //        ASPxDateEdit deDataPlecarii = Contract_DataList.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
        //        ASPxDateEdit deUltZiLucr = Contract_DataList.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
        //        DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (Convert.ToDateTime(deDataPlecarii.Value) != dt)
        //        {
        //            deUltZiLucr.Value = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1);       
        //            ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1).Date;                
        //            ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1).Date;
        //            Session["InformatiaCurentaPersonal"] = ds;
        //        }
        //        else
        //        {
        //            deUltZiLucr.Value = Convert.ToDateTime(deDataPlecarii.Value);
        //            ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).Date;
        //            ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).Date;
        //            Session["InformatiaCurentaPersonal"] = ds;
        //        }

        //        if (Convert.ToDateTime(deDataAngajarii.Value) != null)
        //            if (Convert.ToDateTime(deDataPlecarii.Value) < Convert.ToDateTime(deDataAngajarii.Value))
        //            {
        //                Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data plecarii este anterioara datei angajarii!");
        //                deDataPlecarii.Value = (F100993 == null ? dt : Convert.ToDateTime(F100993));
        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void deDeLaData_LostFocus(object F100933)
        //{
        //    try
        //    {
        //        ActualizareZileLuni();
        //        VerificaNrLuni();

        //        ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
        //        ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
        //        DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

        //        if (Convert.ToDateTime(deLaData.Value) != null)
        //            if (Convert.ToDateTime(deLaData.Value) < Convert.ToDateTime(deDeLaData.Value))
        //            {
        //                Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
        //                deDeLaData.Value = (F100933 == null ? dt : Convert.ToDateTime(F100933));
        //                ds.Tables[0].Rows[0]["F100933"] = (F100933 == null ? dt.Date : Convert.ToDateTime(F100933).Date);
        //                ds.Tables[1].Rows[0]["F100933"] = (F100933 == null ? dt.Date : Convert.ToDateTime(F100933).Date);
        //                Session["InformatiaCurentaPersonal"] = ds;
        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void deLaData_LostFocus(object F100934)
        //{
        //    try
        //    {
        //        //ActualizareZileLuni();
        //        VerificaNrLuni();

        //        //ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
        //        //ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
        //        //DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
        //        //DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

        //        //if (Convert.ToDateTime(deDeLaData.Value) != null)
        //        //    if (Convert.ToDateTime(deLaData.Value) < Convert.ToDateTime(deDeLaData.Value))
        //        //    {
        //        //        Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
        //        //        deLaData.Value = (F100934 == null ? dt : Convert.ToDateTime(F100934));
        //        //        ds.Tables[0].Rows[0]["F100934"] = (F100934 == null ? dt.Date : Convert.ToDateTime(F100934).Date);
        //        //        ds.Tables[1].Rows[0]["F100934"] = (F100934 == null ? dt.Date : Convert.ToDateTime(F100934).Date);
        //        //        Session["InformatiaCurentaPersonal"] = ds;
        //        //    }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void ActualizareZileLuni()
        //{
        //    try
        //    {
        //        //int nrLuni = 0, nrZile = 0;
        //        ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
        //        ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
        //        ASPxDateEdit deDataPlecarii = Contract_DataList.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
        //        ASPxDateEdit deUltZiLucr = Contract_DataList.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
        //        ASPxTextBox txtNrZile = Contract_DataList.Items[0].FindControl("txtNrZile") as ASPxTextBox;
        //        ASPxTextBox txtNrLuni = Contract_DataList.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (!deLaData.Value.Equals(new DateTime(2100, 1, 1)))
        //        {

        //            deUltZiLucr.Value = Convert.ToDateTime(deLaData.Value).AddDays(-1);
        //            deDataPlecarii.Value = deLaData.Value;
        //            ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deLaData.Value).AddDays(-1).Date;
        //            ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deLaData.Value).AddDays(-1).Date;                    
        //            ds.Tables[0].Rows[0]["F100993"] = Convert.ToDateTime(deLaData.Value).Date;
        //            ds.Tables[1].Rows[0]["F100993"] = Convert.ToDateTime(deLaData.Value).Date;                   
        //        }

        //        ////if (deDeLaData.EditValue != null && deLaData.EditValue != null)
        //        //if ((!deDeLaData.Value.Equals(new DateTime(2100, 1, 1))) && (!deLaData.Value.Equals(new DateTime(2100, 1, 1))))
        //        //{
        //        //    //Radu 20.01.2017 - comentat tot si adaugat CalculLuniSiZile
                    

        //        //    CalculLuniSiZile(Convert.ToDateTime(deDeLaData.Value), Convert.ToDateTime(deLaData.Value), out nrLuni, out nrZile);
        //        //    txtNrZile.Value = nrZile;
        //        //    txtNrLuni.Value = nrLuni;
        //        //}
        //        //ds.Tables[0].Rows[0]["F100935"] = nrLuni;
        //        //ds.Tables[0].Rows[0]["F100936"] = nrZile;
        //        //ds.Tables[1].Rows[0]["F100935"] = nrLuni;
        //        //ds.Tables[1].Rows[0]["F100936"] = nrZile;
        //        Session["InformatiaCurentaPersonal"] = ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

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

        //private void deUltZiLucr_LostFocus()
        //{
        //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //    ASPxDateEdit deDataPlecarii = Contract_DataList.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
        //    ASPxDateEdit deUltZiLucr = Contract_DataList.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
        //    deDataPlecarii.Value = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1);
        //    ds.Tables[0].Rows[0]["F100993"] = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1).Date;
        //    ds.Tables[1].Rows[0]["F100993"] = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1).Date;
        //    Session["InformatiaCurentaPersonal"] = ds;
        //}

        //private void cmbGradInvaliditate_SelectedIndexChanged()
        //{
        //    ASPxComboBox cmbGradInvalid = Contract_DataList.Items[0].FindControl("cmbGradInvalid") as ASPxComboBox;
        //    ASPxDateEdit dataValabilInvalid = Contract_DataList.Items[0].FindControl("deDataValabInvalid") as ASPxDateEdit;
        //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //    if (cmbGradInvalid.SelectedIndex > 0)
        //    {
        //        dataValabilInvalid.Enabled = true;
        //    }
        //    else
        //    {
        //        dataValabilInvalid.Enabled = false;
        //        dataValabilInvalid.Value = new DateTime(2100, 1, 1);
        //        ds.Tables[0].Rows[0]["F100271"] = new DateTime(2100, 1, 1);
        //        ds.Tables[1].Rows[0]["F100271"] = new DateTime(2100, 1, 1);
        //        Session["InformatiaCurentaPersonal"] = ds;
        //    }
        //}

       //private void txtVechimeInCompanieAni_LostFocus()
       // {
       //     ASPxTextBox txtVechCompAni = Contract_DataList.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
       //     ASPxTextBox txtVechCompLuni = Contract_DataList.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
       //     if (txtVechCompAni.Text.Length > 0)
       //         txtVechCompAni.Text = txtVechCompAni.Text.PadLeft(2, '0');
       //     else
       //         txtVechCompAni.Text = "00";
       //     DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
       //     ds.Tables[0].Rows[0]["F100643"] = txtVechCompAni.Text + (txtVechCompLuni.Text.Length > 0 ? txtVechCompLuni.Text.PadLeft(2, '0') : "00");
       //     ds.Tables[1].Rows[0]["F100643"] = txtVechCompAni.Text + (txtVechCompLuni.Text.Length > 0 ? txtVechCompLuni.Text.PadLeft(2, '0') : "00");
       //     Session["InformatiaCurentaPersonal"] = ds;

       // }

       // private void txtVechimeInCompanieLuni_LostFocus()
       // {
       //     ASPxTextBox txtVechCompAni = Contract_DataList.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
       //     ASPxTextBox txtVechCompLuni = Contract_DataList.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
       //     if (txtVechCompLuni.Text.Length > 0)
       //         txtVechCompLuni.Text = txtVechCompLuni.Text.PadLeft(2, '0');
       //     else
       //         txtVechCompLuni.Text = "00";
       //     DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
       //     ds.Tables[0].Rows[0]["F100643"] = (txtVechCompAni.Text.Length > 0 ? txtVechCompAni.Text.PadLeft(2, '0') : "00") + txtVechCompLuni.Text;
       //     ds.Tables[1].Rows[0]["F100643"] = (txtVechCompAni.Text.Length > 0 ? txtVechCompAni.Text.PadLeft(2, '0') : "00") + txtVechCompLuni.Text;
       //     Session["InformatiaCurentaPersonal"] = ds;
       // }
        
       // private void txtVechimePeCarteaMuncaAni_LostFocus()
       // {
       //     ASPxTextBox txtVechCarteMuncaAni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
       //     ASPxTextBox txtVechCarteMuncaLuni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
       //     if (txtVechCarteMuncaAni.Text.Length > 0)
       //         txtVechCarteMuncaAni.Text = txtVechCarteMuncaAni.Text.PadLeft(2, '0');
       //     else
       //         txtVechCarteMuncaAni.Text = "00";
       //     DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
       //     ds.Tables[0].Rows[0]["F100644"] = txtVechCarteMuncaAni.Text + (txtVechCarteMuncaLuni.Text.Length > 0 ? txtVechCarteMuncaLuni.Text.PadLeft(2, '0') : "00");
       //     ds.Tables[1].Rows[0]["F100644"] = txtVechCarteMuncaAni.Text + (txtVechCarteMuncaLuni.Text.Length > 0 ? txtVechCarteMuncaLuni.Text.PadLeft(2, '0') : "00");
       //     Session["InformatiaCurentaPersonal"] = ds;
       // }

       // private void txtVechimePeCarteaMuncaLuni_LostFocus()
       // {
       //     ASPxTextBox txtVechCarteMuncaAni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
       //     ASPxTextBox txtVechCarteMuncaLuni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
       //     if (txtVechCarteMuncaLuni.Text.Length > 0)
       //         txtVechCarteMuncaLuni.Text = txtVechCarteMuncaLuni.Text.PadLeft(2, '0');
       //     else
       //         txtVechCarteMuncaLuni.Text = "00";
       //     DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
       //     ds.Tables[0].Rows[0]["F100644"] = (txtVechCarteMuncaAni.Text.Length > 0 ? txtVechCarteMuncaAni.Text.PadLeft(2, '0') : "00") + txtVechCarteMuncaLuni.Text;
       //     ds.Tables[1].Rows[0]["F100644"] = (txtVechCarteMuncaAni.Text.Length > 0 ? txtVechCarteMuncaAni.Text.PadLeft(2, '0') : "00") + txtVechCarteMuncaLuni.Text;
       //     Session["InformatiaCurentaPersonal"] = ds;
       // }


        //private void txtDataPrimeiAngajari_LostFocus()
        //{
        //    try
        //    {
        //        ASPxDateEdit deDataPrimeiAng = Contract_DataList.Items[0].FindControl("deDataPrimeiAng") as ASPxDateEdit;
        //        ASPxTextBox txtVechCarteMuncaAni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
        //        ASPxTextBox txtVechCarteMuncaLuni = Contract_DataList.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
        //        DateTime dt = DateTime.Now;
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (deDataPrimeiAng.Value != null)
        //        {
        //            int diff = (dt.Month - Convert.ToDateTime(deDataPrimeiAng.Value).Month) + 12 * (dt.Year - Convert.ToDateTime(deDataPrimeiAng.Value).Year);
        //            string luni = "", ani = "";
        //            luni = (diff / 12).ToString().PadLeft(2, '0');
        //            txtVechCarteMuncaAni.Value = luni;
        //            ani = (diff - 12 * (diff / 12)).ToString().PadLeft(2, '0');
        //            txtVechCarteMuncaLuni.Value = ani;
        //            ds.Tables[0].Rows[0]["F100644"] = (ani.Length > 0 ? ani.PadLeft(2, '0') : "00") + luni;
        //            ds.Tables[1].Rows[0]["F100644"] = (ani.Length > 0 ? ani.PadLeft(2, '0') : "00") + luni;
        //            Session["InformatiaCurentaPersonal"] = ds;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}

        //private void deDataCrtIntern_EditValueChanged(object F100986)
        //{
        //    ASPxDateEdit deDataAngajarii = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
        //    ASPxDateEdit deDataCrtIntern = Contract_DataList.Items[0].FindControl("deDataCtrInt") as ASPxDateEdit;
        //    DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
        //    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //    if (deDataAngajarii.Value != null)
        //        if (Convert.ToDateTime(deDataCrtIntern.Value) >= Convert.ToDateTime(deDataAngajarii.Value))
        //        {
        //            Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data contract intern trebuie sa fie anterioara datei angajarii!");
        //            deDataCrtIntern.Value = (F100986 == null ? dt : Convert.ToDateTime(F100986));
        //            ds.Tables[0].Rows[0]["F100986"] = (F100986 == null ? dt.Date : Convert.ToDateTime(F100986).Date);
        //            ds.Tables[1].Rows[0]["F100986"] = (F100986 == null ? dt.Date : Convert.ToDateTime(F100986).Date);
        //            Session["InformatiaCurentaPersonal"] = ds;
        //        }
        //}

        //private void cmbNorma_LostFocus()
        //{
        //    try
        //    {
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        ASPxComboBox cmbNorma = Contract_DataList.Items[0].FindControl("cmbNorma") as ASPxComboBox;
        //        ASPxComboBox cmbTimpPartial = Contract_DataList.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
        //        if (cmbNorma.Value == null || (cmbNorma.Value.ToString().Trim().Length <= 0))
        //        {
        //            Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat norma!");
        //            cmbNorma.SelectedIndex = cmbNorma.Items.Count - 1;
        //            cmbNorma.Focus();
        //            ds.Tables[0].Rows[0]["F100973"] = cmbNorma.Items.Count - 1;
        //            ds.Tables[1].Rows[0]["F100973"] = cmbNorma.Items.Count - 1;
        //        }
        //        else
        //        {
        //            cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
        //            ds.Tables[0].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
        //            ds.Tables[1].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
        //        }


        //        if (cmbNorma.Value != null)
        //        {
        //            cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
        //            ds.Tables[0].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
        //            ds.Tables[1].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
        //        }
        //        else
        //        {
        //            cmbTimpPartial.SelectedItem.Value = 1;
        //            ds.Tables[0].Rows[0]["F10043"] = 1;
        //            ds.Tables[1].Rows[0]["F10043"] = 1;
        //        }

        //        Session["InformatiaCurentaPersonal"] = ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


        //private void cmbTimpPartial_SelectedIndexChanged()
        //{
        //    try
        //    {
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        ASPxComboBox cmbNorma = Contract_DataList.Items[0].FindControl("cmbNorma") as ASPxComboBox;
        //        ASPxComboBox cmbTimpPartial = Contract_DataList.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
        //        if (cmbTimpPartial.Value != null && cmbNorma.Value != null && Convert.ToInt32(cmbNorma.Value.ToString()) < Convert.ToInt32(cmbTimpPartial.Value.ToString()))
        //        {
        //            Contract_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timpul partial este mai mare decat norma!");
        //            cmbTimpPartial.SelectedItem.Value = 1;
        //            ds.Tables[0].Rows[0]["F10043"] = 1;
        //            ds.Tables[1].Rows[0]["F10043"] = 1;
        //            Session["InformatiaCurentaPersonal"] = ds;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //}

        //private void cmbPrel_SelectedIndexChanged()
        //{
        //    try
        //    {
        //        VerificaNrLuni();

        //        ASPxComboBox cmbPrelungireContract = Contract_DataList.Items[0].FindControl("cmbPrel") as ASPxComboBox;
        //        ASPxDateEdit deDeLaData = Contract_DataList.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
        //        ASPxDateEdit deLaData = Contract_DataList.Items[0].FindControl("deLaData") as ASPxDateEdit;
        //        ASPxTextBox txtNrCtrIntern = Contract_DataList.Items[0].FindControl("txtNrCtrInt") as ASPxTextBox;
        //        DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
        //        if (Convert.ToInt32(cmbPrelungireContract.SelectedIndex) == 1)
        //        {
        //            inactiveazaDeLaLa = true;

        //            deDeLaData.Enabled = true;
        //            deLaData.Enabled = true;

        //            string data = "CONVERT(VARCHAR, F09506, 103)";
        //            if (Constante.tipBD == 2)
        //                data = "TO_CHAR(F09506, 'dd/mm/yyyy')";

        //            DataTable dt = General.IncarcaDT("SELECT " + data + " FROM F095 WHERE F09503 = " + Session["Marca"].ToString() + " AND F09504 = " + txtNrCtrIntern.Text, null);

        //            if (dt != null && dt.Rows.Count > 0)
        //            {
        //                string[] param = dt.Rows[0]["F09506"].ToString().Split('/');
        //                deDeLaData.Value = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
        //                ds.Tables[0].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
        //                ds.Tables[1].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
        //                Session["InformatiaCurentaPersonal"] = ds;
        //            }
                        
                    
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
        //    }

        //}

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
            //ScriptManager.RegisterStartupScript(this, typeof(string), "OPEN_WINDOW", "var Mleft = (screen.width/2)-(760/2);var Mtop = (screen.height/2)-(700/2);window.open( '~/Avs/Istoric.aspx', null, 'height=700,width=760,status=yes,toolbar=no,scrollbars=yes,menubar=no,location=no,top=\'+Mtop+\', left=\'+Mleft+\'' );", true);
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "popup", "window.open('~/Avs/Istoric.aspx','_blank','menubar=no')", true);
            //~/Avs/Istoric
            Response.Write("  <script language='javascript'> window.open('Sablon.aspx','','width=1020, Height=720,fullscreen=1,location=0,scrollbars=1,menubar=1,toolbar=1'); </script>");

        }

        //private void ArataMesaj(string mesaj)
        //{
        //    Contract_pnlCtl.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    Contract_pnlCtl.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        //}




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

                //bool esteNou = false;
                //if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                //    esteNou = true;

                //string strSql = General.SelectCalculCO(an, f10003, filtruIns, f10022, f10072, vechime, esteNou);
                //Radu 21.04.2020

                string strSql ="select * from calculCO(" + f10003 + ", CONVERT(date,'" + an + "-12-31'), 1, " + f10072 + ")";
                DataTable dtCO = General.IncarcaDT(strSql, null);

                //DataRow dtCO = General.IncarcaDR(@"SELECT * FROM ""Ptj_tblZileCO"" WHERE F10003=@1 AND ""An""=@2", new object[] { f10003, an });
                if (dtCO != null && dtCO.Rows.Count > 0)
                {
                    //F100642
                    //txtZileCOCuvAnCrt
                    ASPxTextBox txtZileCOCuvAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOCuvAnCrt") as ASPxTextBox;
                    if (txtZileCOCuvAnCrt != null)
                        txtZileCOCuvAnCrt.Value = General.Nz(dtCO.Rows[0]["ZileCuveniteAn"], txtZileCOCuvAnCrt.Text).ToString();
                        //txtZileCOCuvAnCrt.Value = General.Nz(dtCO["CuveniteAn"], txtZileCOCuvAnCrt.Text).ToString();
                    //F100995
                    //txtZileCOAnCrt
                    ASPxTextBox txtZileCOAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOAnCrt") as ASPxTextBox;
                    if (txtZileCOAnCrt != null)
                        txtZileCOAnCrt.Value = General.Nz(dtCO.Rows[0]["ZileCuvenite"], "").ToString();
                        //txtZileCOAnCrt.Value = General.Nz(dtCO["Cuvenite"], "").ToString();
                    //F100996
                    //txtZileCOAnAnt
                    ASPxTextBox txtZileCOAnAnt = Contract_DataList.Items[0].FindControl("txtZileCOAnAnt") as ASPxTextBox;
                    if (txtZileCOAnAnt != null)
                        txtZileCOAnAnt.Value = General.Nz(dtCO.Rows[0]["ZileAnPrecedent"], "").ToString();
                        //txtZileCOAnAnt.Value = General.Nz(dtCO["SoldAnterior"], "").ToString();
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


        //private void CalculCO()
        //{
        //    try
        //    {
        //        ASPxDateEdit deDataAng = Contract_DataList.Items[0].FindControl("deDataAng") as ASPxDateEdit;
        //        if (deDataAng == null) return;

        //        int an = DateTime.Now.Year;
        //        DateTime f10022 = deDataAng.Date;

        //        string dtInc = an.ToString() + "-01-01";
        //        string dtSf = an.ToString() + "-12-31";

        //        string strSql = "";
        //        string filtruIns = "";
        //        string f10003 = "-99";

        //        f10003 = Session["Marca"].ToString();
        //        filtruIns = " AND F10003=" + Session["Marca"].ToString();

        //        if (Constante.tipBD == 1)
        //        {

        //            //daca nu exista inseram linie goala si apoi updatam
        //            strSql += "insert into Ptj_tblZileCO(F10003, An, USER_NO, TIME) " +
        //            " select F10003, " + an + ", " + HttpContext.Current.Session["UserId"] + ", GetDate() from F100 where F10003 not in (select F10003 from Ptj_tblZileCO where An=" + an + ") " +
        //            " and " + General.ToDataUniv(f10022) + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023" + filtruIns + ";";


        //            strSql += "with xx as " +
        //            " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
        //            " (select a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
        //            " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107='2100-01-01' then b.f11106 else b.f11107 end  " +
        //            " and b.f11105 <= case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end) " +
        //            " group by a.f11103, a.f11105, case when a.f11107='2100-01-01' then a.f11106 else a.f11107 end, a.time) t " +
        //            " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
        //            " case when f111.f11107='2100-01-01' then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
        //            " union all " +
        //            " select f10003 Marca, datainceput de_la_data, datasfarsit la_data " +
        //            " from ptj_cereri inner join Ptj_tblAbsente on Ptj_Cereri.IdAbsenta = Ptj_tblAbsente.id " +
        //            " where ptj_cereri.IdStare=3 and isnull(Ptj_tblAbsente.AbsenteCFPInCalculCO,0) = 1  ), " +
        //            "  yy as " +
        //            " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
        //            " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //            " group by a.marca, a.de_la_data, a.la_data), " +
        //            " f111_2 as  " +
        //            " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
        //            " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //            " group by a.marca, a.de_la_data, a.la_data) " +
        //            " update x set x.Cuvenite = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +
        //            " ROUND((case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
        //            " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END)) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
        //            " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
        //            " (datediff(day,(CASE WHEN cast(" + General.ToDataUniv(f10022) + " as date) < '" + dtInc + "' THEN '" + dtInc + "' ELSE cast(" + General.ToDataUniv(f10022) + " as date) END) " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
        //            " ,(CASE WHEN cast(f10023 as date) < '" + dtSf + "' THEN cast(f10023 as date) ELSE '" + dtSf + "' END))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
        //            " - (SELECT COALESCE(SUM(datediff(d,CASE WHEN F11105 < CONVERT(date,'" + an + "-01-01') THEN CONVERT(date,'" + an + "-01-01') else F11105 END,CASE WHEN F11107 > CONVERT(date,'" + an + "-12-31') THEN CONVERT(date,'" + an + "-12-31') else F11107 END)) + 1,0) from f111_2 A where f11103=" + f10003 + " and F11105 <= F11107 AND (year(F11105)=" + an + " or year(F11107)=" + an + "))" +
        //            " ) " +
        //            " /CONVERT(float,365),0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
        //            " from F100 a " +
        //            " left join (select ISNULL(convert(int,substring(F100644,1,2)),0) * 12 + ISNULL(convert(int,substring(F100644,3,2)),0) + DATEDIFF (MONTH,  " +
        //            " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
        //            " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
        //            " left join F026 c on convert(int,a.F10072) = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
        //            " where " + General.ToDataUniv(f10022) + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " from Ptj_tblZileCO x " +
        //            " where x.An=" + an + filtruIns + ";";


        //            //la fel ca mai sus - fara ponderea cu nr de zile lucrate in an
        //            strSql += "update x set x.CuveniteAn = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +
        //            " (case when a.F100642 is null or a.F100642 = 0 then c.F02615 else a.F100642 end " +                                                   //nr zile cuvenite conform grilei
        //            " + (CASE WHEN ISNULL(a.F10027,0)>=2 THEN Convert(int,isnull((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
        //            " ) as ZileCuvenite " +
        //            " from F100 a " +
        //            " left join (select ISNULL(convert(int,substring(F100644,1,2)),0) * 12 + ISNULL(convert(int,substring(F100644,3,2)),0) + DATEDIFF (MONTH,  " +
        //            " (select convert(nvarchar(4),F01011) + '-' + convert(nvarchar(4),F01012) + '-01' from F010),'" + dtSf + "' " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
        //            " ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma
        //            " left join F026 c on convert(int,a.F10072) = c.F02604 and (convert(int,c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (convert(int,c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
        //            " where " + General.ToDataUniv(f10022) + " <= '" + dtSf + "' and '" + dtInc + "' <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " from Ptj_tblZileCO x " +
        //            " where x.An=" + an + filtruIns + ";";
        //        }
        //        else
        //        {
        //            dtInc = "01-01-" + an.ToString();
        //            dtSf = "31-12-" + an.ToString();

        //            //daca nu exista inseram linie goala si apoi updatam
        //            strSql += "insert into \"Ptj_tblZileCO\"(F10003, \"An\", USER_NO, TIME) " +
        //            " select F10003, " + an + ", " + HttpContext.Current.Session["UserId"] + ", SYSDATE from F100 where F10003 not in (select F10003 from \"Ptj_tblZileCO\" where \"An\"=" + an + ") " +
        //            " and " + General.ToDataUniv(f10022) + " <= to_date('" + dtSf + "','DD-MM-YYYY') and to_date('" + dtInc + "','DD-MM-YYYY') <= F10023" + filtruIns + ";";

        //            strSql += "update \"Ptj_tblZileCO\" x set x.\"Cuvenite\" = ( " +
        //                    " with xx as " +
        //                    " (select f111.f11103 Marca, f111.f11105 de_la_data, case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end la_data from f111 inner join  " +
        //                    " (select a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end f11107, a.time, max(b.time) timp from F111 a inner join f111 b " +
        //                    " on a.F11103 = b.F11103 and  (a.f11105 <= case when b.f11107=to_date('01-01-2100','DD-MM-YYYY') then b.f11106 else b.f11107 end  " +
        //                    " and b.f11105 <= case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end) " +
        //                    " group by a.f11103, a.f11105, case when a.f11107=to_date('01-01-2100','DD-MM-YYYY') then a.f11106 else a.f11107 end, a.time) t " +
        //                    " on f111.f11103 = t.f11103 and f111.f11105 = t.f11105 and  " +
        //                    " case when f111.f11107=to_date('01-01-2100','DD-MM-YYYY') then f111.f11106 else f111.f11107 end = t.f11107 and f111.time = t.timp " +
        //                    " union all " +
        //                    " select f10003 Marca, \"DataInceput\" de_la_data, \"DataSfarsit\" la_data " +
        //                    " from \"Ptj_Cereri\" inner join \"Ptj_tblAbsente\" on \"Ptj_Cereri\".\"IdAbsenta\" = \"Ptj_tblAbsente\".\"Id\" " +
        //                    " where \"Ptj_Cereri\".\"IdStare\"=3 and COALESCE(\"Ptj_tblAbsente\".\"AbsenteCFPInCalculCO\",0) = 1  ), " +
        //                    "  yy as " +
        //                    " (select distinct a.marca, min(b.de_la_data) de_la_data, max(b.la_data) la_data from xx a " +
        //                    " inner join xx b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //                    " group by a.marca, a.de_la_data, a.la_data), " +
        //                    " f111_2 as  " +
        //                    " (select distinct a.marca f11103, min(b.de_la_data) f11105, max(b.la_data)  f11107 from yy a " +
        //                    " inner join yy b on a.marca = b.marca and (a.de_la_data <= b.la_data and b.de_la_data <= a.la_data) " +
        //                    " group by a.marca, a.de_la_data, a.la_data) " +
        //                    " select y.ZileCuvenite from " +
        //            " (select a.F10003, " +
        //            " ROUND((case when a.F100642 is null or a.F100642 = 0 then c.F02615 else TO_NUMBER(a.F100642) end " +                                                   //nr zile cuvenite conform grilei
        //            " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END)) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
        //            " * " +                                                                 //aceste zile cuvenite se inmultesc cu ce urmeaza
        //            " (least(trunc(f10023),to_date('31-12-" + an + "','DD-MM-YYYY') " +        //luam min dintre ultima zi lucrata si sfarsitul anului de referinta
        //            " ) - greatest(trunc(" + General.ToDataUniv(f10022) + "),to_date('01-01-" + an + "','DD-MM-YYYY'))+1 " +  //luam maxim dintre prima zi lucrata di prima zi a anului de referinta
        //            " - nvl(b.cfp,0) " +                                                   //scadem zilele de concediu fara plata luate in anul de referinta
        //            " - (select COALESCE(SUM(least(trunc(F11107),to_date('31-12-" + an + "','DD-MM-YYYY')) - greatest(trunc(f11105),to_date('01-01-" + an + "','DD-MM-YYYY')) + 1),0) from f111 A where f11103=" + f10003 + " and F11105 <= F11107 AND (to_Char(F11105,'yyyy')='" + an + "' or to_Char(F11107,'yyyy')='" + an + "')) " +
        //            " ) " +
        //            " /365,0) as ZileCuvenite " +                                           //impartim totul la 365 de zile si apoi se inmulteste cu nr de zile cuvenite, de mai sus
        //            " from F100 a " +
        //            " left join (select nvl(to_number(substr(F100644,1,2)),0) * 12 + nvl(to_number(substr(F100644,3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-12-" + an + "','DD-MM-YYYY'), " +
        //            " (select to_date('01/' ||  F01012 || '/' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
        //            " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
        //            " left join F026 c on a.F10072 = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
        //            " left join ((select F10003, nvl(sum(least(trunc(\"DataSfarsit\"),to_date('31-12-" + an + "','DD-MM-YYYY')-1) - greatest(trunc(\"DataInceput\"),to_date('01-01-" + an + "','DD-MM-YYYY'))+1),0) as cfp from \"Ptj_Cereri\" where \"IdAbsenta\" in (SELECT \"Id\" from \"Ptj_tblAbsente\" where \"AbsenteCFPInCalculCO\"=1) and \"IdStare\"=3 AND (to_Char(\"DataInceput\",'YYYY') ='" + an + "' OR to_Char(\"DataSfarsit\",'YYYY') ='" + an + "') group by f10003)) b on a.F10003 = b.F10003 " +  //se calcuelaza nr de cfp avute in anul de referinta
        //            " where " + General.ToDataUniv(f10022) + " <= to_date('31-12-" + an + "','DD-MM-YYYY') and to_date('01-01-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " where x.\"An\"=" + an + filtruIns + ";";

        //            strSql += "update \"Ptj_tblZileCO\" x set x.\"CuveniteAn\" = (select y.ZileCuvenite from " +
        //            " (select a.F10003, " +
        //            " (case when a.F100642 is null or a.F100642 = 0 then c.F02615 else TO_NUMBER(a.F100642) end " +                                                   //nr zile cuvenite conform grilei
        //            " + (CASE WHEN NVL(a.F10027,0)>=2 THEN to_number(nvl((select \"Valoare\" from \"tblParametrii\" where \"Nume\"='NrZilePersoanaDizabilitatiSauMaiMica18Ani'),3)) ELSE 0 END) " +               //daca este pers. cu dizabilitati mai se adauga 3 zile
        //            " ) as ZileCuvenite " +
        //            " from F100 a " +
        //            " left join (select nvl(to_number(substr(F100644,1,2)),0) * 12 + nvl(to_number(substr(F100644,3,2)),0) + trunc(MONTHS_BETWEEN (to_date('31-12-" + an + "','DD-MM-YYYY'), " +
        //            " (select to_date('01/' || F01012 || '/' ||  F01011,'DD-MM-YYYY') from F010) " +  //luam ca data de referinta luna de lucru, pt ca in WizSalary la inchidere de luna, se adauga automat o luna in campul - experienta in firma
        //            " ) + 1 ) as CalcLuni, F10003 from F100) d on a.F10003 = d.F10003  " +             //se calculeaza nr de luni de experienta cu care a intrat in firma, la care se adauga nr de luni pe care le-a lucrat in firma + luna de lucru deschisa pt ca functia MONTHS_BETWEEN nu tine cont de ea
        //            " left join F026 c on a.F10072 = c.F02604 and (to_number(c.F02610/100) * 12) <= d.CALCLUNI and d.CALCLUNI < (to_number(c.F02611/100) * 12) " +                                                                                                              //se obtine nr de zile cuenveite din tabela de grile conform vechimei obtinute mai sus
        //            " where " + General.ToDataUniv(f10022) + " <= to_date('31-12-" + an + "','DD-MM-YYYY') and to_date('01-01-" + an + "','DD-MM-YYYY') <= F10023 ) y where y.F10003=x.F10003) " +   //se calcuelaza totul pt angajatii activi in anul de referinta
        //            " where x.\"An\"=" + an + filtruIns + ";";

        //        }

        //        strSql = "BEGIN " + strSql + " END;";

        //        General.ExecutaNonQuery(strSql, null);

        //        //if (cuActualizareInF100)
        //        //{
        //        //    strSql = "UPDATE a SET  a.F100642 = b.\"CuveniteAn\", a.F100995 = b.\"Cuvenite\", a.F100996 = b.\"SoldAnterior\" FROM F100 a,  \"Ptj_tblZileCO\" b WHERE a.F10003 = B.F10003 AND b.\"An\" =  " + an;
        //        //    General.ExecutaNonQuery(strSql, null);
        //        //}

        //        DataRow dtCO = General.IncarcaDR(@"SELECT * FROM ""Ptj_tblZileCO"" WHERE F10003=@1 AND ""An""=@2", new object[] { f10003, an });
        //        if (dtCO != null)
        //        {
        //            //F100642
        //            //txtZileCOCuvAnCrt
        //            ASPxTextBox txtZileCOCuvAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOCuvAnCrt") as ASPxTextBox;
        //            if (txtZileCOCuvAnCrt != null)
        //                txtZileCOCuvAnCrt.Value = General.Nz(dtCO["CuveniteAn"],"").ToString();
        //            //F100995
        //            //txtZileCOAnCrt
        //            ASPxTextBox txtZileCOAnCrt = Contract_DataList.Items[0].FindControl("txtZileCOAnCrt") as ASPxTextBox;
        //            if (txtZileCOAnCrt != null)
        //                txtZileCOAnCrt.Value = General.Nz(dtCO["Cuvenite"], "").ToString();
        //            //F100996
        //            //txtZileCOAnAnt
        //            ASPxTextBox txtZileCOAnAnt = Contract_DataList.Items[0].FindControl("txtZileCOAnAnt") as ASPxTextBox;
        //            if (txtZileCOAnAnt != null)
        //                txtZileCOAnAnt.Value = General.Nz(dtCO["SoldAnterior"], "").ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
        //        //General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


    }
}