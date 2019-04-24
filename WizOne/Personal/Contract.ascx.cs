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

namespace WizOne.Personal
{
    public partial class Contract : System.Web.UI.UserControl
    {
        decimal timpPartial = 0;
        bool inactiveazaDeLaLa = false;
        protected void Page_Init(object sender, EventArgs e)
        {

            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            DataList1.DataSource = table;
            DataList1.DataBind();



            ASPxTextBox txtZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
            ASPxTextBox txtLuni = DataList1.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
            ASPxDateEdit deDeLa = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            ASPxDateEdit deLa = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
            ASPxDateEdit deTermenRevisal = DataList1.Items[0].FindControl("deTermenRevisal") as ASPxDateEdit;
            deTermenRevisal.Value = SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));

            int nrLuni = 0, nrZile = 0;

            CalculLuniSiZile(Convert.ToDateTime(deDeLa.Date), Convert.ToDateTime(deLa.Date), out nrLuni, out nrZile);
            txtZile.Value = nrZile;
            txtLuni.Value = nrLuni;
            //txtZile.Text = Convert.ToInt32((Convert.ToDateTime(deLa.Date) - Convert.ToDateTime(deDeLa.Date)).TotalDays).ToString();
            //txtLuni.Text = (((Convert.ToDateTime(deLa.Date).Year - Convert.ToDateTime(deDeLa.Date).Year) * 12) + Convert.ToDateTime(deLa.Date).Month - Convert.ToDateTime(deDeLa.Date).Month).ToString();
            //cmbTipAngajat_SelectedIndexChanged(ds.Tables[1].Rows[0]["F10043"].ToString());

            ASPxTextBox txtVechCompAni = DataList1.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
            ASPxTextBox txtVechCompLuni = DataList1.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaAni = DataList1.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaLuni = DataList1.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;

            if (ds.Tables[1].Rows[0]["F100643"] != null && ds.Tables[1].Rows[0]["F100643"].ToString().Length >= 2)
            {
                txtVechCompAni.Text = ds.Tables[1].Rows[0]["F100643"].ToString().Substring(0, 2);
                txtVechCompLuni.Text = ds.Tables[1].Rows[0]["F100643"].ToString().Substring(2, 2);
            }
            else
            {
                txtVechCompAni.Text = "00";
                txtVechCompLuni.Text = "00";
            }

            if (ds.Tables[1].Rows[0]["F100644"] != null && ds.Tables[1].Rows[0]["F100644"].ToString().Length >= 2)
            {
                txtVechCarteMuncaAni.Text = ds.Tables[1].Rows[0]["F100644"].ToString().Substring(0, 2);
                txtVechCarteMuncaLuni.Text = ds.Tables[1].Rows[0]["F100644"].ToString().Substring(2, 2);
            }
            else
            {
                txtVechCarteMuncaAni.Text = "00";
                txtVechCarteMuncaLuni.Text = "00";
            }			
			
            ASPxComboBox cmbCOR = DataList1.Items[0].FindControl("cmbCOR") as ASPxComboBox;
            cmbCOR.Value = Convert.ToInt32((ds.Tables[1].Rows[0]["F10098"] == null || ds.Tables[1].Rows[0]["F10098"].ToString().Length <= 0 ? "0" : ds.Tables[1].Rows[0]["F10098"].ToString()));
                        
            ds.Tables[1].Rows[0]["F100935"] = nrLuni;
            ds.Tables[1].Rows[0]["F100936"] = nrZile;
            Session["InformatiaCurentaPersonal"] = ds;

            string[] etichete = new string[57] { "lblNrCtrInt", "lblDataCtrInt", "lblDataAng", "lblTipCtrMunca", "lblDurCtr", "lblDeLaData", "lblLaData", "lblNrLuni", "lblNrZile", "lblPrel", "lblExcIncet","lblCASSAngajat",
                                                 "lblCASSAngajator", "lblSalariu", "lblDataModifSa", "lblCategAng1", "lblCategAng2", "lblLocAnt", "lblLocatieInt", "lblTipAng", "lblTimpPartial", "lblNorma", "lblDataModifNorma",
                                                 "lblTipNorma", "lblDurTimpMunca", "lblRepTimpMunca", "lblIntervRepTimpMunca", "lblNrOre", "lblCOR", "lblDataModifCOR", "lblFunctie", "lblDataModifFunctie", "lblMeserie",
                                                 "lblPerioadaProba", "lblZL", "lblZC", "lblNrZilePreavizDemisie", "lblNrZilePreavizConc", "lblUltimaZiLucr", "lblMotivPlecare", "lblDataPlecarii", "lblDataReintegr", "lblGradInvalid",
                                                 "lblDataValabInvalid", "lblVechimeComp", "lblVechCompAni", "lblVechCompLuni", "lblVechimeCarteMunca", "lblVechCarteMuncaAni", "lblVechCarteMuncaLuni", "lblGrila", "lblZileCOFidel",
                                                 "lblZileCOAnAnt", "lblZileCOCuvAnCrt", "lblZLP", "lblZLPCuv", "lblDataPrimeiAng"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] bife = new string[4] { "chkFunctieBaza",  "chkScutitImp", "chkBifaPensionar", "chkBifaDetasat"};
            for (int i = 0; i < bife.Count(); i++)
            {
                ASPxCheckBox chk = DataList1.Items[0].FindControl(bife[i]) as ASPxCheckBox;
                chk.Text = Dami.TraduCuvant(chk.Text);
            }


            string[] butoane = new string[19] { "btnCtrInt", "btnCtrIntIst", "btnDataAng", "btnDataAngIst", "btnCASS", "btnCASSIst", "btnSalariu", "btnSalariuIst", "btnNorma", "btnNormaIst", "btnCautaCOR", "btnCOR", "btnCORIst",
                                                "btnFunc", "btnFuncIst", "btnMeserie", "btnMeserieIst", "btnMotivPl", "btnMotivPlIst"};
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = DataList1.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }

            if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
            {//DNATA
                string[] lstTextBox = new string[8] { "txtNrCtrInt", "txtSalariu", "txtPerProbaZL", "txtPerProbaZC", "txtNrZilePreavizDemisie", "txtNrZilePreavizConc",
                                                    "txtZileCOCuvAnCrt", "txtNrOre"};
                for (int i = 0; i < lstTextBox.Count(); i++)
                {
                    ASPxTextBox txt = DataList1.Items[0].FindControl(lstTextBox[i]) as ASPxTextBox;
                    txt.BackColor = Color.LightGray;
                }

                string[] lstDateEdit = new string[4] { "deDataCtrInt", "deDataAng", "deDeLaData", "deLaData" };
                for (int i = 0; i < lstDateEdit.Count(); i++)
                {
                    ASPxDateEdit de = DataList1.Items[0].FindControl(lstDateEdit[i]) as ASPxDateEdit;
                    de.BackColor = Color.LightGray;
                }


                string[] lstComboBox = new string[11] { "cmbTipCtrMunca", "cmbDurCtr", "cmbTipAng", "cmbTimpPartial", "cmbNorma", "cmbTipNorma", "cmbDurTimpMunca", "cmbRepTimpMunca",
                                                "cmbIntervRepTimpMunca", "cmbCOR", "cmbFunctie"};
                for (int i = 0; i < lstComboBox.Count(); i++)
                {
                    ASPxComboBox cmb = DataList1.Items[0].FindControl(lstComboBox[i]) as ASPxComboBox;
                    cmb.BackColor = Color.LightGray;
                }

                if (!IsPostBack)
                {
                    SetariNorma();
                }

            }

            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));

        }


        private void SetariNorma()
        {
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            DateIdentificare dateId = new DateIdentificare();
            int varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
            if (varsta >= 16 && varsta < 18)
            {
                ASPxComboBox cmbNorma = DataList1.Items[0].FindControl("cmbNorma") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DataList1.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                ASPxTextBox txtNrOre = DataList1.Items[0].FindControl("txtNrOre") as ASPxTextBox;

                cmbNorma.Value = 6;
                cmbNorma.ReadOnly = true;
                cmbTimpPartial.Value = 6;

                if (txtNrOre.Text.Length > 0 && Convert.ToInt32(txtNrOre.Text) > 30)
                {
                    txtNrOre.Text = "";
                    ds.Tables[0].Rows[0]["F100964"] = DBNull.Value;
                    ds.Tables[2].Rows[0]["F100964"] = DBNull.Value;
                }

                ds.Tables[0].Rows[0]["F100973"] = 6;
                ds.Tables[1].Rows[0]["F100973"] = 6;
                ds.Tables[0].Rows[0]["F10043"] = 6;
                ds.Tables[1].Rows[0]["F10043"] = 6;
                Session["InformatiaCurentaPersonal"] = ds;
            }
        }

        protected void pnlCtlContract_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            //string url = "~/Avs/Istoric.aspx";
            switch (param[0])
            {
                case "txtNrCtrInt":
                    ds.Tables[0].Rows[0]["F100985"] = param[1];
                    ds.Tables[1].Rows[0]["F100985"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataCtrInt":
                    deDataCrtIntern_EditValueChanged(ds.Tables[1].Rows[0]["F100986"]);
                    string[] data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100986"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100986"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;

                    DateTime dataCtr = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    string strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataCtr.Year;
                    if (Constante.tipBD == 2)
                        strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataCtr.Year;
                    DataTable dtHolidays = General.IncarcaDT(strSql, null);
                    bool ziLibera = EsteZiLibera(dataCtr, dtHolidays);
                    if (dataCtr.DayOfWeek.ToString().ToLower() == "saturday" || dataCtr.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data contract intern este intr-o zi nelucratoare!");
                    }

                    break;
                case "deDataAng":
                    deDataCrtIntern_EditValueChanged(ds.Tables[1].Rows[0]["F100986"]);
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F10022"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    ASPxDateEdit deTermenRevisal = DataList1.Items[0].FindControl("deTermenRevisal") as ASPxDateEdit;
                    deTermenRevisal.Value = SetDataRevisal(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
                    if (Session["esteNou"] != null && Session["esteNou"].ToString().Length > 0 && Session["esteNou"].ToString() == "true")
                    {
                        int val = 1;
                        string sql = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'TermenDepasireRevisal'";
                        DataTable dt = General.IncarcaDT(sql, null);
                        if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0] != null && dt.Rows[0][0].ToString().Length > 0)
                            val = Convert.ToInt32(dt.Rows[0][0].ToString());
                        if (Convert.ToDateTime(deTermenRevisal.Value).Date < DateTime.Now.Date && val == 1)
                            pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Termen depunere Revisal depasit!");
                    }

                    DateTime dataAng = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    strSql = "SELECT CONVERT(DATE, DAY, 103) AS DAY FROM HOLIDAYS WHERE YEAR(DAY) = " + dataAng.Year;
                    if (Constante.tipBD == 2)
                        strSql = "SELECT TRUNC(DAY) AS DAY FROM HOLIDAYS WHERE EXTRACT(YEAR FROM DAY) = " + dataAng.Year;
                    dtHolidays = General.IncarcaDT(strSql, null);                  
                    ziLibera = EsteZiLibera(dataAng, dtHolidays);
                    if (dataAng.DayOfWeek.ToString().ToLower() == "saturday" || dataAng.DayOfWeek.ToString().ToLower() == "sunday" || ziLibera)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data angajarii este intr-o zi nelucratoare!");
                    }

                    break;
                case "cmbTipCtrMunca":
                    ds.Tables[0].Rows[0]["F100984"] = param[1];
                    ds.Tables[1].Rows[0]["F100984"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbDurCtr":
                    cmbDurataContract_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F1009741"] = param[1];
                    ds.Tables[1].Rows[0]["F1009741"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDeLaData":
                    deDeLaData_LostFocus(ds.Tables[1].Rows[0]["F100933"]);
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deLaData":
                    deLaData_LostFocus(ds.Tables[1].Rows[0]["F100934"]);
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100934"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100934"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbPrel":
                    cmbPrel_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F100938"] = param[1];
                    ds.Tables[1].Rows[0]["F100938"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbExcIncet":
                    ds.Tables[0].Rows[0]["F100929"] = param[1];
                    ds.Tables[1].Rows[0]["F100929"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCASSAngajat":
                    ds.Tables[0].Rows[0]["F1003900"] = param[1];
                    ds.Tables[1].Rows[0]["F1003900"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCASSAngajator":
                    ds.Tables[0].Rows[0]["F1003907"] = param[1];
                    ds.Tables[1].Rows[0]["F1003907"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtSalariu":
                    ds.Tables[0].Rows[0]["F100699"] = param[1];
                    ds.Tables[1].Rows[0]["F100699"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataModifSal":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100991"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100991"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCategAng1":
                    ds.Tables[0].Rows[0]["F10061"] = param[1];
                    ds.Tables[1].Rows[0]["F10061"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCategAng2":
                    ds.Tables[0].Rows[0]["F10062"] = param[1];
                    ds.Tables[1].Rows[0]["F10062"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtLocAnt":
                    ds.Tables[0].Rows[0]["F10078"] = param[1];
                    ds.Tables[1].Rows[0]["F10078"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbLocatieInt":
                    ds.Tables[0].Rows[0]["F100966"] = param[1];
                    ds.Tables[2].Rows[0]["F100966"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTipAng":
                    cmbTipAngajat_SelectedIndexChanged(ds.Tables[1].Rows[0]["F10043"].ToString());
                    ds.Tables[0].Rows[0]["F10010"] = param[1];
                    ds.Tables[1].Rows[0]["F10010"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTimpPartial":
                    DateIdentificare dateId = new DateIdentificare();
                    int varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                    if (varsta >= 16 && varsta < 18 && Convert.ToInt32(param[1]) > 6)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timp partial invalid (max 6 pentru minori peste 16 ani)!");
                        SetariNorma();
                        return;
                    }
                    cmbTimpPartial_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F10043"] = param[1];
                    ds.Tables[1].Rows[0]["F10043"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbNorma":
                    cmbNorma_LostFocus();
                    ds.Tables[0].Rows[0]["F100973"] = param[1];
                    ds.Tables[1].Rows[0]["F100973"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataModifNorma":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100955"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F100955"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTipNorma":
                    dateId = new DateIdentificare();
                    varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                    if (varsta >= 16 && varsta < 18 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100964"].ToString()) > 30 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 30 pentru minori peste 16 ani)!");
                        SetariNorma();
                        return;
                    }
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["F100926"].ToString()) == 1 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100964"].ToString()) > 40)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 40 pentru norma intreaga)!");
                        return;
                    }
                    ds.Tables[0].Rows[0]["F100926"] = param[1];
                    ds.Tables[1].Rows[0]["F100926"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbDurTimpMunca":
                    ds.Tables[0].Rows[0]["F100927"] = param[1];
                    ds.Tables[1].Rows[0]["F100927"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbRepTimpMunca":
                    ds.Tables[0].Rows[0]["F100928"] = param[1];
                    ds.Tables[1].Rows[0]["F100928"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbIntervRepTimpMunca":
                    cmbIntervalRepartizareTimpMunca_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F100939"] = param[1];
                    ds.Tables[1].Rows[0]["F100939"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrOre":
                    dateId = new DateIdentificare();
                    varsta = dateId.Varsta(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10021"].ToString()));
                    if (varsta >= 16 && varsta < 18 && Convert.ToInt32(param[1]) > 30 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 30 pentru minori peste 16 ani)!");
                        SetariNorma();
                        return;
                    }
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["F100926"].ToString()) == 1 && Convert.ToInt32(ds.Tables[0].Rows[0]["F100939"].ToString()) == 2 && Convert.ToInt32(param[1]) > 40)
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Numar invalid de ore pe luna/saptamana (max 40 pentru norma intreaga)!");                       
                        return;
                    }

                    ds.Tables[0].Rows[0]["F100964"] = param[1];
                    ds.Tables[2].Rows[0]["F100964"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCOR":
                    ds.Tables[0].Rows[0]["F10098"] = param[1];
                    ds.Tables[1].Rows[0]["F10098"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataModifCOR":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100956"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F100956"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbFunctie":
                    ds.Tables[0].Rows[0]["F10071"] = param[1];
                    ds.Tables[1].Rows[0]["F10071"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataModifFunctie":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100992"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100992"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbMeserie":
                    ds.Tables[0].Rows[0]["F10029"] = param[1];
                    ds.Tables[1].Rows[0]["F10029"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "chkFunctieBaza":
                    ds.Tables[0].Rows[0]["F10048"] = (param[1] == "true" ? 1 : 0);
                    ds.Tables[1].Rows[0]["F10048"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtPerProbaZL":
                    ds.Tables[0].Rows[0]["F1001063"] = param[1];
                    ds.Tables[2].Rows[0]["F1001063"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtPerProbaZC":
                    ds.Tables[0].Rows[0]["F100975"] = param[1];
                    ds.Tables[1].Rows[0]["F100975"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrZilePreavizDemisie":
                    ds.Tables[0].Rows[0]["F1009742"] = param[1];
                    ds.Tables[1].Rows[0]["F1009742"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrZilePreavizConc":
                    ds.Tables[0].Rows[0]["F100931"] = param[1];
                    ds.Tables[1].Rows[0]["F100931"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deUltimaZiLucr":
                    //Florin 2018.11.12
                    //deUltZiLucr_LostFocus();
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F10023"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[0].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(1);
                    ds.Tables[1].Rows[0]["F10023"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(1);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbMotivPlecare":
                    ds.Tables[0].Rows[0]["F10025"] = param[1];
                    ds.Tables[1].Rows[0]["F10025"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataPlecarii":
                    //Florin 2018.11.12
                    //deDataPlecarii_EditValueChanged(ds.Tables[1].Rows[0]["F100993"]);
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[0].Rows[0]["F10023"] = (Convert.ToInt32(data[2]) == 2100 ? new DateTime(2100, 1, 1) : new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(-1));
                    ds.Tables[1].Rows[0]["F100993"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F10023"] = (Convert.ToInt32(data[2]) == 2100 ? new DateTime(2100, 1, 1) : new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0])).AddDays(-1));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataReintegr":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100930"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100930"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbGradInvalid":
                    cmbGradInvaliditate_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F10027"] = param[1];
                    ds.Tables[1].Rows[0]["F10027"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataValabInvalid":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100271"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100271"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "chkScutitImp":
                    ds.Tables[0].Rows[0]["F10026"] = (param[1] == "true" ? 1 : 0);
                    ds.Tables[1].Rows[0]["F10026"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "chkBifaPensionar":
                    ds.Tables[0].Rows[0]["F10037"] = (param[1] == "true" ? 1 : 0);
                    ds.Tables[1].Rows[0]["F10037"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "chkBifaDetasat":
                    ds.Tables[0].Rows[0]["F100954"] = (param[1] == "true" ? 1 : 0);
                    ds.Tables[2].Rows[0]["F100954"] = (param[1] == "true" ? 1 : 0);
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtVechCompAni":
                    txtVechimeInCompanieAni_LostFocus();
                    break;
                case "txtVechCompLuni":
                    txtVechimeInCompanieLuni_LostFocus();
                    break;
                case "txtVechCarteMuncaAni":
                    txtVechimePeCarteaMuncaAni_LostFocus();
                    break;
                case "txtVechCarteMuncaLuni":
                    txtVechimePeCarteaMuncaLuni_LostFocus();
                    break;

                case "txtZileCOFidel":
                    ds.Tables[0].Rows[0]["F100640"] = param[1];
                    ds.Tables[1].Rows[0]["F100640"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtZileCOAnAnt":
                    ds.Tables[0].Rows[0]["F100641"] = param[1];
                    ds.Tables[1].Rows[0]["F100641"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtZileCOCuvAnCrt":
                    ds.Tables[0].Rows[0]["F100642"] = param[1];
                    ds.Tables[1].Rows[0]["F100642"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;

                case "deDataPrimeiAng":
                    txtDataPrimeiAngajari_LostFocus();
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F1001049"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[2].Rows[0]["F1001049"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
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
                    if (lst != null)
                    {
                        object[] codCOR = lst[0] as object[];
                        Session["CodCORSelectat"] = null;
                        ASPxComboBox cmbCOR = DataList1.Items[0].FindControl("cmbCOR") as ASPxComboBox;
                        cmbCOR.Value = Convert.ToInt32(codCOR[0].ToString());
                        ds.Tables[0].Rows[0]["F10098"] = codCOR[0];
                        ds.Tables[1].Rows[0]["F10098"] = codCOR[0];
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
            }

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

        private void cmbTipAngajat_SelectedIndexChanged(string F10043)
        {
            try
            {
                ASPxComboBox cmbTipAngajat = DataList1.Items[0].FindControl("cmbTipAng") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DataList1.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                ASPxComboBox cmbTipNorma = DataList1.Items[0].FindControl("cmbTipNorma") as ASPxComboBox;
                ASPxComboBox cmbDurTimpMunca = DataList1.Items[0].FindControl("cmbDurTimpMunca") as ASPxComboBox;
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (Convert.ToInt32(cmbTipAngajat.SelectedItem.Value) == 0)
                {
                    if (cmbTimpPartial.SelectedItem == null)
                        cmbTimpPartial.SelectedIndex = 0;
                    cmbTimpPartial.SelectedItem.Value = timpPartial > 0 ? timpPartial : Convert.ToDecimal(F10043);
                    cmbTimpPartial.Enabled = false;
                }
                else
                {
                    cmbTimpPartial.SelectedIndex = 0;
                    cmbTimpPartial.Enabled = true;
                    if (Convert.ToInt32(cmbTipAngajat.SelectedItem.Value) == 2)
                    {
                        cmbTipNorma.Value = 2;
                        cmbDurTimpMunca.Value = 5;
                        ds.Tables[0].Rows[0]["F100926"] = 2;
                        ds.Tables[1].Rows[0]["F100926"] = 2;                                            
                        ds.Tables[0].Rows[0]["F100927"] = 5;
                        ds.Tables[1].Rows[0]["F100927"] = 5;
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void cmbIntervalRepartizareTimpMunca_SelectedIndexChanged()
        {
            try
            {
                ASPxComboBox cmbIntervalRepartizareTimpMunca = DataList1.Items[0].FindControl("cmbIntervRepTimpMunca") as ASPxComboBox;
                ASPxTextBox txtNrOreLunaSaptamana = DataList1.Items[0].FindControl("txtNrOre") as ASPxTextBox;
                if (Convert.ToInt32(cmbIntervalRepartizareTimpMunca.SelectedItem.Value) == 2 || Convert.ToInt32(cmbIntervalRepartizareTimpMunca.SelectedItem.Value) == 3)
                    txtNrOreLunaSaptamana.Enabled = true;
                else
                    txtNrOreLunaSaptamana.Enabled = false;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void cmbDurataContract_SelectedIndexChanged()
        {
            try
            {
                VerificaNrLuni();
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void VerificaNrLuni()
        {
            try
            {
                ASPxComboBox cmbDurataContract = DataList1.Items[0].FindControl("cmbDurCtr") as ASPxComboBox;
                ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deDataAngajarii = DataList1.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
                ASPxDateEdit deDataPlecarii = DataList1.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
                ASPxDateEdit deUltZiLucr = DataList1.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
                ASPxTextBox txtZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
                ASPxTextBox txtLuni = DataList1.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
                ASPxTextBox txtNrCtrIntern = DataList1.Items[0].FindControl("txtNrCtrInt") as ASPxTextBox;

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
                        inactiveazaDeLaLa = true;

                        deDeLaData.Enabled = true;
                        deLaData.Enabled = true;

                        if (deDeLaData.Value == null)
                            pnlCtlContract.JSProperties["cpAlertMessage"] =  Dami.TraduCuvant("Completati perioada pentru contractul pe perioada determinata!");

                        if (Convert.ToInt32(txtLuni.Text) > 36 || (Convert.ToInt32(txtLuni.Text) == 36 && Convert.ToInt32(txtZile.Text) > 0))
                            pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Durata maxima a unui contract pe perioada determinata nu poate depasi 36 de luni");
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(deLaData.Value) - Convert.ToDateTime(deDeLaData.Value);

                            if (Convert.ToInt32(Session["Marca"].ToString()) != -99 && txtNrCtrIntern.Text != "")
                            {
                                string mesaj = General.NumarLuniContract(Convert.ToInt32(Session["Marca"].ToString()), txtNrCtrIntern.Text, Convert.ToDateTime(deDeLaData.Value), Convert.ToDateTime(deLaData.Value), Convert.ToInt32(ts.TotalDays + 1));
                                if (mesaj != null && mesaj != "") pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant(mesaj).Substring(1);   
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private void deDataPlecarii_EditValueChanged(object F100993)
        {
            try
            {                
                ASPxDateEdit deDataAngajarii = DataList1.Items[0].FindControl("deDataAng") as ASPxDateEdit;
                ASPxDateEdit deDataPlecarii = DataList1.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
                ASPxDateEdit deUltZiLucr = DataList1.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
                DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (Convert.ToDateTime(deDataPlecarii.Value) != dt)
                {
                    deUltZiLucr.Value = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1);       
                    ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1).Date;                
                    ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).AddDays(-1).Date;
                    Session["InformatiaCurentaPersonal"] = ds;
                }
                else
                {
                    deUltZiLucr.Value = Convert.ToDateTime(deDataPlecarii.Value);
                    ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).Date;
                    ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deDataPlecarii.Value).Date;
                    Session["InformatiaCurentaPersonal"] = ds;
                }

                if (Convert.ToDateTime(deDataAngajarii.Value) != null)
                    if (Convert.ToDateTime(deDataPlecarii.Value) < Convert.ToDateTime(deDataAngajarii.Value))
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data plecarii este anterioara datei angajarii!");
                        deDataPlecarii.Value = (F100993 == null ? dt : Convert.ToDateTime(F100993));
                    }

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void deDeLaData_LostFocus(object F100933)
        {
            try
            {
                ActualizareZileLuni();
                VerificaNrLuni();

                ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
                DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                if (Convert.ToDateTime(deLaData.Value) != null)
                    if (Convert.ToDateTime(deLaData.Value) < Convert.ToDateTime(deDeLaData.Value))
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                        deDeLaData.Value = (F100933 == null ? dt : Convert.ToDateTime(F100933));
                        ds.Tables[0].Rows[0]["F100933"] = (F100933 == null ? dt.Date : Convert.ToDateTime(F100933).Date);
                        ds.Tables[1].Rows[0]["F100933"] = (F100933 == null ? dt.Date : Convert.ToDateTime(F100933).Date);
                        Session["InformatiaCurentaPersonal"] = ds;
                    }

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void deLaData_LostFocus(object F100934)
        {
            try
            {
                ActualizareZileLuni();
                VerificaNrLuni();

                ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
                DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

                if (Convert.ToDateTime(deDeLaData.Value) != null)
                    if (Convert.ToDateTime(deLaData.Value) < Convert.ToDateTime(deDeLaData.Value))
                    {
                        pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                        deLaData.Value = (F100934 == null ? dt : Convert.ToDateTime(F100934));
                        ds.Tables[0].Rows[0]["F100934"] = (F100934 == null ? dt.Date : Convert.ToDateTime(F100934).Date);
                        ds.Tables[1].Rows[0]["F100934"] = (F100934 == null ? dt.Date : Convert.ToDateTime(F100934).Date);
                        Session["InformatiaCurentaPersonal"] = ds;
                    }

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void ActualizareZileLuni()
        {
            try
            {
                int nrLuni = 0, nrZile = 0;
                ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
                ASPxDateEdit deDataPlecarii = DataList1.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
                ASPxDateEdit deUltZiLucr = DataList1.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
                ASPxTextBox txtNrZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
                ASPxTextBox txtNrLuni = DataList1.Items[0].FindControl("txtNrLuni") as ASPxTextBox;
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (!deLaData.Value.Equals(new DateTime(2100, 1, 1)))
                {

                    deUltZiLucr.Value = Convert.ToDateTime(deLaData.Value).AddDays(-1);
                    deDataPlecarii.Value = deLaData.Value;
                    ds.Tables[0].Rows[0]["F10023"] = Convert.ToDateTime(deLaData.Value).AddDays(-1).Date;
                    ds.Tables[1].Rows[0]["F10023"] = Convert.ToDateTime(deLaData.Value).AddDays(-1).Date;                    
                    ds.Tables[0].Rows[0]["F100993"] = Convert.ToDateTime(deLaData.Value).Date;
                    ds.Tables[1].Rows[0]["F100993"] = Convert.ToDateTime(deLaData.Value).Date;                   
                }

                //if (deDeLaData.EditValue != null && deLaData.EditValue != null)
                if ((!deDeLaData.Value.Equals(new DateTime(2100, 1, 1))) && (!deLaData.Value.Equals(new DateTime(2100, 1, 1))))
                {
                    //Radu 20.01.2017 - comentat tot si adaugat CalculLuniSiZile
                    

                    CalculLuniSiZile(Convert.ToDateTime(deDeLaData.Value), Convert.ToDateTime(deLaData.Value), out nrLuni, out nrZile);
                    txtNrZile.Value = nrZile;
                    txtNrLuni.Value = nrLuni;
                }
                ds.Tables[0].Rows[0]["F100935"] = nrLuni;
                ds.Tables[0].Rows[0]["F100936"] = nrZile;
                ds.Tables[1].Rows[0]["F100935"] = nrLuni;
                ds.Tables[1].Rows[0]["F100936"] = nrZile;
                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
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
            if (dataSfarsit != new DateTime(2100, 1, 1, 0, 0, 0) && dataStart != new DateTime(2100, 1, 1, 0, 0, 0))
                nrZile = (int)(dataSfarsit - dataStart).TotalDays + 1;

            for (int nI = 0; nI < arNrZileInLuna.Count && nrZile >= arNrZileInLuna[nI]; nI++)
            {
                nrZile -= arNrZileInLuna[nI];
                nrLuni++;
            }

        }

        private void deUltZiLucr_LostFocus()
        {
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            ASPxDateEdit deDataPlecarii = DataList1.Items[0].FindControl("deDataPlecarii") as ASPxDateEdit;
            ASPxDateEdit deUltZiLucr = DataList1.Items[0].FindControl("deUltimaZiLucr") as ASPxDateEdit;
            deDataPlecarii.Value = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1);
            ds.Tables[0].Rows[0]["F100993"] = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1).Date;
            ds.Tables[1].Rows[0]["F100993"] = Convert.ToDateTime(deUltZiLucr.Value).AddDays(1).Date;
            Session["InformatiaCurentaPersonal"] = ds;
        }

        private void cmbGradInvaliditate_SelectedIndexChanged()
        {
            ASPxComboBox cmbGradInvalid = DataList1.Items[0].FindControl("cmbGradInvalid") as ASPxComboBox;
            ASPxDateEdit dataValabilInvalid = DataList1.Items[0].FindControl("deDataValabInvalid") as ASPxDateEdit;
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (cmbGradInvalid.SelectedIndex > 0)
            {
                dataValabilInvalid.Enabled = true;
            }
            else
            {
                dataValabilInvalid.Enabled = false;
                dataValabilInvalid.Value = new DateTime(2100, 1, 1);
                ds.Tables[0].Rows[0]["F100271"] = new DateTime(2100, 1, 1);
                ds.Tables[1].Rows[0]["F100271"] = new DateTime(2100, 1, 1);
                Session["InformatiaCurentaPersonal"] = ds;
            }
        }

       private void txtVechimeInCompanieAni_LostFocus()
        {
            ASPxTextBox txtVechCompAni = DataList1.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
            ASPxTextBox txtVechCompLuni = DataList1.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
            if (txtVechCompAni.Text.Length > 0)
                txtVechCompAni.Text = txtVechCompAni.Text.PadLeft(2, '0');
            else
                txtVechCompAni.Text = "00";
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            ds.Tables[0].Rows[0]["F100643"] = txtVechCompAni.Text + (txtVechCompLuni.Text.Length > 0 ? txtVechCompLuni.Text.PadLeft(2, '0') : "00");
            ds.Tables[1].Rows[0]["F100643"] = txtVechCompAni.Text + (txtVechCompLuni.Text.Length > 0 ? txtVechCompLuni.Text.PadLeft(2, '0') : "00");
            Session["InformatiaCurentaPersonal"] = ds;

        }

        private void txtVechimeInCompanieLuni_LostFocus()
        {
            ASPxTextBox txtVechCompAni = DataList1.Items[0].FindControl("txtVechCompAni") as ASPxTextBox;
            ASPxTextBox txtVechCompLuni = DataList1.Items[0].FindControl("txtVechCompLuni") as ASPxTextBox;
            if (txtVechCompLuni.Text.Length > 0)
                txtVechCompLuni.Text = txtVechCompLuni.Text.PadLeft(2, '0');
            else
                txtVechCompLuni.Text = "00";
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            ds.Tables[0].Rows[0]["F100643"] = (txtVechCompAni.Text.Length > 0 ? txtVechCompAni.Text.PadLeft(2, '0') : "00") + txtVechCompLuni.Text;
            ds.Tables[1].Rows[0]["F100643"] = (txtVechCompAni.Text.Length > 0 ? txtVechCompAni.Text.PadLeft(2, '0') : "00") + txtVechCompLuni.Text;
            Session["InformatiaCurentaPersonal"] = ds;
        }

        private void txtVechimePeCarteaMuncaAni_LostFocus()
        {
            ASPxTextBox txtVechCarteMuncaAni = DataList1.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaLuni = DataList1.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
            if (txtVechCarteMuncaAni.Text.Length > 0)
                txtVechCarteMuncaAni.Text = txtVechCarteMuncaAni.Text.PadLeft(2, '0');
            else
                txtVechCarteMuncaAni.Text = "00";
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            ds.Tables[0].Rows[0]["F100644"] = txtVechCarteMuncaAni.Text + (txtVechCarteMuncaLuni.Text.Length > 0 ? txtVechCarteMuncaLuni.Text.PadLeft(2, '0') : "00");
            ds.Tables[1].Rows[0]["F100644"] = txtVechCarteMuncaAni.Text + (txtVechCarteMuncaLuni.Text.Length > 0 ? txtVechCarteMuncaLuni.Text.PadLeft(2, '0') : "00");
            Session["InformatiaCurentaPersonal"] = ds;
        }

        private void txtVechimePeCarteaMuncaLuni_LostFocus()
        {
            ASPxTextBox txtVechCarteMuncaAni = DataList1.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
            ASPxTextBox txtVechCarteMuncaLuni = DataList1.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
            if (txtVechCarteMuncaLuni.Text.Length > 0)
                txtVechCarteMuncaLuni.Text = txtVechCarteMuncaLuni.Text.PadLeft(2, '0');
            else
                txtVechCarteMuncaLuni.Text = "00";
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            ds.Tables[0].Rows[0]["F100644"] = (txtVechCarteMuncaAni.Text.Length > 0 ? txtVechCarteMuncaAni.Text.PadLeft(2, '0') : "00") + txtVechCarteMuncaLuni.Text;
            ds.Tables[1].Rows[0]["F100644"] = (txtVechCarteMuncaAni.Text.Length > 0 ? txtVechCarteMuncaAni.Text.PadLeft(2, '0') : "00") + txtVechCarteMuncaLuni.Text;
            Session["InformatiaCurentaPersonal"] = ds;
        }

        private void txtDataPrimeiAngajari_LostFocus()
        {
            try
            {
                ASPxDateEdit deDataPrimeiAng = DataList1.Items[0].FindControl("deDataPrimeiAng") as ASPxDateEdit;
                ASPxTextBox txtVechCarteMuncaAni = DataList1.Items[0].FindControl("txtVechCarteMuncaAni") as ASPxTextBox;
                ASPxTextBox txtVechCarteMuncaLuni = DataList1.Items[0].FindControl("txtVechCarteMuncaLuni") as ASPxTextBox;
                DateTime dt = DateTime.Now;
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (deDataPrimeiAng.Value != null)
                {
                    int diff = (dt.Month - Convert.ToDateTime(deDataPrimeiAng.Value).Month) + 12 * (dt.Year - Convert.ToDateTime(deDataPrimeiAng.Value).Year);
                    string luni = "", ani = "";
                    luni = (diff / 12).ToString().PadLeft(2, '0');
                    txtVechCarteMuncaAni.Value = luni;
                    ani = (diff - 12 * (diff / 12)).ToString().PadLeft(2, '0');
                    txtVechCarteMuncaLuni.Value = ani;
                    ds.Tables[0].Rows[0]["F100644"] = (ani.Length > 0 ? ani.PadLeft(2, '0') : "00") + luni;
                    ds.Tables[1].Rows[0]["F100644"] = (ani.Length > 0 ? ani.PadLeft(2, '0') : "00") + luni;
                    Session["InformatiaCurentaPersonal"] = ds;
                }

            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void deDataCrtIntern_EditValueChanged(object F100986)
        {
            ASPxDateEdit deDataAngajarii = DataList1.Items[0].FindControl("deDataAng") as ASPxDateEdit;
            ASPxDateEdit deDataCrtIntern = DataList1.Items[0].FindControl("deDataCtrInt") as ASPxDateEdit;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            if (deDataAngajarii.Value != null)
                if (Convert.ToDateTime(deDataCrtIntern.Value) >= Convert.ToDateTime(deDataAngajarii.Value))
                {
                    pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data contract intern trebuie sa fie anterioara datei angajarii!");
                    deDataCrtIntern.Value = (F100986 == null ? dt : Convert.ToDateTime(F100986));
                    ds.Tables[0].Rows[0]["F100986"] = (F100986 == null ? dt.Date : Convert.ToDateTime(F100986).Date);
                    ds.Tables[1].Rows[0]["F100986"] = (F100986 == null ? dt.Date : Convert.ToDateTime(F100986).Date);
                    Session["InformatiaCurentaPersonal"] = ds;
                }
        }

        private void cmbNorma_LostFocus()
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                ASPxComboBox cmbNorma = DataList1.Items[0].FindControl("cmbNorma") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DataList1.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                if (cmbNorma.Value == null || (cmbNorma.Value.ToString().Trim().Length <= 0))
                {
                    pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu ati completat norma!");
                    cmbNorma.SelectedIndex = cmbNorma.Items.Count - 1;
                    cmbNorma.Focus();
                    ds.Tables[0].Rows[0]["F100973"] = cmbNorma.Items.Count - 1;
                    ds.Tables[1].Rows[0]["F100973"] = cmbNorma.Items.Count - 1;
                }
                else
                {
                    cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
                    ds.Tables[0].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
                    ds.Tables[1].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
                }


                if (cmbNorma.Value != null)
                {
                    cmbTimpPartial.SelectedItem.Value = Convert.ToInt32(cmbNorma.Value.ToString());
                    ds.Tables[0].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
                    ds.Tables[1].Rows[0]["F10043"] = Convert.ToInt32(cmbNorma.Value.ToString());
                }
                else
                {
                    cmbTimpPartial.SelectedItem.Value = 1;
                    ds.Tables[0].Rows[0]["F10043"] = 1;
                    ds.Tables[1].Rows[0]["F10043"] = 1;
                }

                Session["InformatiaCurentaPersonal"] = ds;
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        private void cmbTimpPartial_SelectedIndexChanged()
        {
            try
            {
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                ASPxComboBox cmbNorma = DataList1.Items[0].FindControl("cmbNorma") as ASPxComboBox;
                ASPxComboBox cmbTimpPartial = DataList1.Items[0].FindControl("cmbTimpPartial") as ASPxComboBox;
                if (cmbTimpPartial.Value != null && cmbNorma.Value != null && Convert.ToInt32(cmbNorma.Value.ToString()) < Convert.ToInt32(cmbTimpPartial.Value.ToString()))
                {
                    pnlCtlContract.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Timpul partial este mai mare decat norma!");
                    cmbTimpPartial.SelectedItem.Value = 1;
                    ds.Tables[0].Rows[0]["F10043"] = 1;
                    ds.Tables[1].Rows[0]["F10043"] = 1;
                    Session["InformatiaCurentaPersonal"] = ds;
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private void cmbPrel_SelectedIndexChanged()
        {
            try
            {
                VerificaNrLuni();

                ASPxComboBox cmbPrelungireContract = DataList1.Items[0].FindControl("cmbPrel") as ASPxComboBox;
                ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
                ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
                ASPxTextBox txtNrCtrIntern = DataList1.Items[0].FindControl("txtNrCtrInt") as ASPxTextBox;
                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                if (Convert.ToInt32(cmbPrelungireContract.SelectedIndex) == 1)
                {
                    inactiveazaDeLaLa = true;

                    deDeLaData.Enabled = true;
                    deLaData.Enabled = true;

                    string data = "CONVERT(VARCHAR, F09506, 103)";
                    if (Constante.tipBD == 2)
                        data = "TO_CHAR(F09506, 'dd/mm/yyyy')";

                    DataTable dt = General.IncarcaDT("SELECT " + data + " FROM F095 WHERE F09503 = " + Session["Marca"].ToString() + " AND F09504 = " + txtNrCtrIntern.Text, null);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string[] param = dt.Rows[0]["F09506"].ToString().Split('/');
                        deDeLaData.Value = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
                        ds.Tables[0].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
                        ds.Tables[1].Rows[0]["F100933"] = new DateTime(Convert.ToInt32(param[2]), Convert.ToInt32(param[1]), Convert.ToInt32(param[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                        
                    
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

        }

        private void ModifAvans(int atribut)
        {
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "Contract";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
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
        //    pnlCtlContract.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtlContract.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));

        //}

    }
}