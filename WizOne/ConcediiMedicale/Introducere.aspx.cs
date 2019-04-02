using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.ConcediiMedicale
{
    public partial class Introducere : System.Web.UI.Page
    {
        decimal timpPartial = 0;
        bool inactiveazaDeLaLa = false;
        protected void Page_Init(object sender, EventArgs e)
        {
            //DataList1.DataSource = General.IncarcaDT("SELECT * FROM F300 WHERE F10003 = " + (Session["MarcaCM"] != null ? Session["MarcaCM"].ToString() : "-99"), null);
            //DataList1.DataBind();


            //ASPxComboBox cmbTipConcediu = DataList1.Items[0].FindControl("cmbTipConcediu") as ASPxComboBox;
            //ASPxRadioButton rbConcInit = DataList1.Items[0].FindControl("rbConcInit") as ASPxRadioButton;

            cmbTipConcediu.DataSource = General.GetTipConcediu();
            cmbTipConcediu.DataBind();

            cmbLocPresc.DataSource = General.GetLocPrescriere();
            cmbLocPresc.DataBind();

            cmbCC.DataSource = General.GetCentriCost();
            cmbCC.DataBind();

            cmbCT1.DataSource = General.GetCoduriTransfer();
            cmbCT1.DataBind();

            cmbCT2.DataSource = General.GetCoduriTransfer();
            cmbCT2.DataBind();

            cmbCT3.DataSource = General.GetCoduriTransfer();
            cmbCT3.DataBind();

            cmbCT4.DataSource = General.GetCoduriTransfer();
            cmbCT4.DataBind();

            cmbCT5.DataSource = General.GetCoduriTransfer();
            cmbCT5.DataBind();

            cmbCT6.DataSource = General.GetCoduriTransfer();
            cmbCT6.DataBind();

            DataTable dtAngajati = new DataTable();
            if (!IsPostBack)
            {
                dtAngajati = General.IncarcaDT(SelectAngajati(), null);
                cmbAng.DataSource = dtAngajati;
                Session["CM_Angajati"] = dtAngajati;
                cmbAng.DataBind();
                cmbAng.SelectedIndex = -1;

                cmbTipConcediu.SelectedIndex = 0;
                OnSelChangeTip();

                rbConcInit.Checked = true;

                OnUpdateCcNo();
               


            }
            else
            {
                dtAngajati = Session["CM_Angajati"] as DataTable;
                cmbAng.DataSource = dtAngajati;
                cmbAng.DataBind();
                if (Session["MarcaCM"] != null)
                    cmbAng.SelectedIndex = Convert.ToInt32(Session["MarcaCM"].ToString());
                else
                    cmbAng.SelectedIndex = -1;

                //if (Session["CM_StartDate"] != null)
                //    deDeLaData.Value = Session["CM_StartDate"] as DateTime?;
                //if (Session["CM_EndDate"] != null)
                //    deLaData.Value = Session["CM_EndDate"] as DateTime?;
               
                string sql = "SELECT F01012, F01011 FROM F010";
                DataTable dtLC = General.IncarcaDT(sql, null);

                DateTime dtTmp = Convert.ToDateTime(Session["CM_StartDate"]);
                if (dtTmp.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString()))
                {
                    rbOptiune1.Visible = true;
                    rbOptiune2.Visible = true;
                    rbOptiune1.Checked = true;
                    btnMZ.Enabled = false;
                }
                else
                {
                    rbOptiune1.Visible = false;
                    rbOptiune2.Visible = false;
                    btnMZ.Enabled = true;
                }
                InitWorkingDays();
            }
        }


        void OnOK()
        {
            //ASPxComboBox cmbLocPresc = DataList1.Items[0].FindControl("cmbLocPresc") as ASPxComboBox;
            //ASPxComboBox cmbTipConcediu = DataList1.Items[0].FindControl("cmbTipConcediu") as ASPxComboBox;
            //ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            //ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
            //ASPxDateEdit deData = DataList1.Items[0].FindControl("deData") as ASPxDateEdit;
            //ASPxDateEdit deDataAviz = DataList1.Items[0].FindControl("deDataAviz") as ASPxDateEdit;
            //ASPxTextBox txtCodIndemn = DataList1.Items[0].FindControl("txtCodIndemn") as ASPxTextBox;
            //ASPxTextBox txtBCCM = DataList1.Items[0].FindControl("txtBCCM") as ASPxTextBox;
            //ASPxTextBox txtZBC = DataList1.Items[0].FindControl("txtZBC") as ASPxTextBox;           
            //ASPxTextBox txtMedic = DataList1.Items[0].FindControl("txtMedic") as ASPxTextBox;
            //ASPxTextBox txtCodDiag = DataList1.Items[0].FindControl("txtCodDiag") as ASPxTextBox;
            //ASPxTextBox txtCodUrgenta = DataList1.Items[0].FindControl("txtCodUrgenta") as ASPxTextBox;
            //ASPxTextBox txtCodInfCont = DataList1.Items[0].FindControl("txtCodInfCont") as ASPxTextBox;
            //ASPxTextBox txtSerie = DataList1.Items[0].FindControl("txtSerie") as ASPxTextBox;
            //ASPxTextBox txtNr = DataList1.Items[0].FindControl("txtNr") as ASPxTextBox;
            //ASPxTextBox txtMZ = DataList1.Items[0].FindControl("txtMZ") as ASPxTextBox;
            //ASPxTextBox txtCNP = DataList1.Items[0].FindControl("txtCNP") as ASPxTextBox;
            //ASPxTextBox txtNrAviz = DataList1.Items[0].FindControl("txtNrAviz") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtCT1 = DataList1.Items[0].FindControl("txtCT1") as ASPxTextBox;
            //ASPxTextBox txtCT2 = DataList1.Items[0].FindControl("txtCT2") as ASPxTextBox;
            //ASPxTextBox txtCT3 = DataList1.Items[0].FindControl("txtCT3") as ASPxTextBox;
            //ASPxTextBox txtCT4 = DataList1.Items[0].FindControl("txtCT4") as ASPxTextBox;
            //ASPxTextBox txtCT5 = DataList1.Items[0].FindControl("txtCT5") as ASPxTextBox;
            //ASPxTextBox txtCT6 = DataList1.Items[0].FindControl("txtCT6") as ASPxTextBox;
            //ASPxTextBox txtCC = DataList1.Items[0].FindControl("txtCC") as ASPxTextBox;
            //ASPxRadioButton rbConcInit = DataList1.Items[0].FindControl("rbConcInit") as ASPxRadioButton;
            //ASPxCheckBox chkStagiu = DataList1.Items[0].FindControl("chkStagiu") as ASPxCheckBox;

            DataTable dtMARDEF = Session["MARDEF"] as DataTable;


            double suma_4450_subplafon = 0;        //?????????

            double procent, vechime;

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);

            string sqlParam = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'COD_MED_EXCL_90'";
            DataTable dtParam = General.IncarcaDT(sqlParam, null);
            string codExcl90 = "";
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)            
                codExcl90 =dtParam.Rows[0][0].ToString().Trim();           

            if (Session["MarcaCM"] == null)
            {
                MessageBox.Show(Dami.TraduCuvant("Nu ati ales niciun angajat!"));
                return;
            }

            int marca = Convert.ToInt32(Session["MarcaCM"].ToString());
            bool avans = false;


            if (txtCodIndemn.Text.Trim().Length <= 0)
            {
                MessageBox.Show(Dami.TraduCuvant("Nu ati completat codul de indemnizatie!"));
                return;
            }

            if (txtCodDiag.Text.Trim().Length <= 0)
            {
                if (txtCodIndemn.Text.Trim() == "15")
                    txtCodDiag.Text = "RM";
                else
                {
                    MessageBox.Show(Dami.TraduCuvant("Nu ati completat codul diagnostic!"));
                    return;
                }
            }

            if (deDeLaData.Value == null)
            {
                MessageBox.Show(Dami.TraduCuvant("Data invalida!"));
                return;
            }
            else
            {
                DateTime dt = Convert.ToDateTime(deDeLaData.Value);
                if (dt.Month != Convert.ToInt32(dtLC.Rows[0][0].ToString()))
                {

                    if (dt.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString()))
                        avans = true;
                    else
                    {
                        MessageBox.Show(Dami.TraduCuvant("Data de start este anterioara lunii curente!"));
                        return;
                    }
                }
            }

            if (deLaData.Value == null)
            {
                MessageBox.Show(Dami.TraduCuvant("Data invalida!"));
                return;
            }


            int cc = Convert.ToInt32(txtCC.Text);

            DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + marca, null);

            vechime = ConvertVechime(dtAng.Rows[0]["F100644"].ToString());
            
            int tip = Convert.ToInt32(cmbTipConcediu.Value);
            
            bool bErr = false;
            string szErrMsg = "In urma validarilor efectuate au rezultat urmatoarele avertizari:\n";

            if (txtSerie.Text.Trim().Length <= 0)   // seria CM
            {
                bErr = true;
                szErrMsg += "\n- nu ati completat Seria certificatului medical !";
            }

            if (txtNr.Text.Trim().Length <= 0 || Convert.ToInt32(txtNr.Text.Trim()) == 0) // numarul CM
            {
                bErr = true;
                szErrMsg += "\n- nu ati completat Numarul certificatului medical !";
            }
                       
            if (deData.Value == null)  
            {
                bErr = true;
                szErrMsg += "\n- nu ati completat Data certificatului medical !";
            }

            if (txtCodIndemn.Text.Trim() == "04")
            {
                if (deDataAviz.Value == null)  
                {
                    bErr = true;
                    szErrMsg += "\n- nu ati completat Data aviz. directia de sanatate publica !";
                }
            }

            //verificare ca data acordarii CM sa nu fie anterioara datei de start
            //insa numai pentru CM initiale
            DateTime dtStart = Convert.ToDateTime(deDeLaData.Value);
            DateTime dtEnd = Convert.ToDateTime(deLaData.Value);
            DateTime dtAcord = Convert.ToDateTime(deData.Value);

             if (dtAcord < dtStart && rbConcInit.Checked)
            {
                bErr = true;
                szErrMsg += "\n- data acordarii este anterioara celei de start !";
            }

            //verificare daca mai exista combinatia serie & nr CM in F300; de asemenea, daca aceasta combinatie e diferita de cea a concediului in continuare, daca exista
            DataTable dtTranz = GetConcediuInitial(0, avans);

            string codB = "", codP = "";
            codB = txtSCMInit.Text;
            codP = txtNrCMInit.Text; 

            if (txtSerie.Text == codB && txtNr.Text == codP)
            {
                bErr = true;
                szErrMsg += "\n- Nr./Serie certificat medical curent este identic cu Nr./Serie certificat medical anterior!";
            }
            else
                if (dtTranz != null && dtTranz.Rows.Count > 0)
                {                   
                    for (int i = 0; i < dtTranz.Rows.Count; i++)
                    {
                        if (dtTranz.Rows[i]["F300601"].ToString().Length > 0 && dtTranz.Rows[i]["F300601"].ToString() == txtSerie.Text
                          && dtTranz.Rows[i]["F300602"].ToString().Length > 0 && dtTranz.Rows[i]["F300602"].ToString() == txtNr.Text)
                            {
                                bErr = true;
                                szErrMsg += "\n- exista deja un CM cu aceeasi serie si acelasi numar !";
                                break;
                            }
                    }
                }


            string szCoduriIndemnizatie = ",01,02,03,04,05,06,07,08,09,10,11,12,13,14,15,";
            string cod = ",";
            if (txtCodIndemn.Text.Trim().Length <= 0)   // cod indemnizatie
            {
                bErr = true;
                szErrMsg += "\n- nu ati completat codul de indemnizatie !";

            }
            else
            {               
                cod += txtCodIndemn.Text.Trim() + ",";
                if (!szCoduriIndemnizatie.Contains(cod))
                {
                    bErr = true;
                    szErrMsg += "\n- codul de indemnizatie completat incorect, valori acceptate\n\t (01,02,03,04,05,06,07,08,09,10,11,12,13,14,15) !";
                }
            }
            cod = cod.Replace(",", "");
            if (cod == "06" && 2 != tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru codul de indemnizatie 06 CM selectat este incorect, valori admise CM urgenta !";
            }
            if (cod != "06" && 2 == tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru CM urgenta codul de indemnizatie este incorect, valoare acceptata 06 !";
            }

            if (cod == "05" && 10 != tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru codul de indemnizatie 05 CM selectat este incorect, valori admise CM infecto-contagioase !";
            }
            if (cod != "05" && 10 == tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru CM infecto-contagioase codul de indemnizatie este incorect, valoare acceptata 05 !";
            }

            if (cod == "09" && 8 != tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru codul de indemnizatie 09 CM selectat este incorect, valori admise CM Ingrijire copil<7a/<18a hand !";
            }
            if (cod != "09" && 8 == tip)
            {
                bErr = true;
                szErrMsg += "\n- pentru CM Ingrijire copil<7a/<18a hand codul de indemnizatie este incorect, valoare acceptata 09 !";
            }

            if (2 == tip)   // CM urrgente
            {
                if (txtCodUrgenta.Text.Trim().Length <= 0)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru CM urgenta nu ati completat codul de urgenta aferent !";
                }
                else if (Convert.ToInt32(txtCodUrgenta.Text.Trim()) > 175 || Convert.ToInt32(txtCodUrgenta.Text.Trim()) < 1)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru CM urgenta codul de urgenta este incorect, valori acceptate de la 1 la 175 !";
                }
            }

            if (10 == tip)  // CM infecto-contagioase
            {
                if (txtCodInfCont.Text.Trim().Length <= 0)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru CM infecto-contagioase nu ati completat codul de infecto-contagioase aferent !";
                }
                else if (Convert.ToInt32(txtCodInfCont.Text.Trim()) > 36 || Convert.ToInt32(txtCodInfCont.Text.Trim()) < 1)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru CM infecto-contagioase codul de infecto-contagioase incorect, valori acceptate de la 1 la 36 !";
                }
            }

            if (12 == tip)  // CM TBC
            {
                if (txtCodInfCont.Text.Trim().Length > 0)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru CM TBC codul de infecto-contagioase nu trebuie completat !";
                }
            }
            
            if (cod == "09")
            {
                if (txtCNP.Text.Length < 13)
                {
                    bErr = true;
                    szErrMsg += "\n- pentru cod indemnizatie 09 nu ati completat CNP copil !";
                }
            }

            int lLocPrescriere = Convert.ToInt32(cmbLocPresc.SelectedItem == null ? 0 : cmbLocPresc.SelectedItem.Value);
            if (1 == lLocPrescriere)    // medic de famile, incapacitate temporara de munca
            {
                int diff = (dtEnd - dtStart).Days;
                if (diff + 1 > 10 && cod != "08" && cod != "15")
                {
                    bErr = true;
                    szErrMsg += "\n- concediul medical eliberat pentru incapacitate temporara de munca de catre medicul de familie nu poate depasi 10 zile !";
                }
            }

            double suma4449 = 0, suma4450 = 0;
            //CString mzCM = "";
            //m_Edit_MediaZilnicaCM.GetWindowText(mzCM);

            int code = 0;   

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) >= 0)
                code = Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString());

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) >= 0)
                code = Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString());

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) >= 0)
                code = Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString());

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) >= 0)
                code = Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString());

            bool CM90 = false;
            if (!codExcl90.Contains(code.ToString()))
            {
                int diff = (dtEnd - dtStart).Days;
                int nrZileLC = diff + 1;

                int zile90 = GetZileMed(marca);

                if (zile90 + nrZileLC > 90)
                {
                    CM90 = true;
                    if (txtNrAviz.Text.Trim().Length <= 0)
                    {
                        bErr = true;
                        szErrMsg += "\n- trebuie sa completati campul 'Nr. aviz medic expert'\ndeoarece durata concediilor medicale pe ultimul an depaseste 90 de zile !";
                    }
                }
            }
 

            if (bErr)
            {
                //szErrMsg += "\n\nDoriti sa reveniti la CM pentru completarea informatiilor omise ?";
                //if (IDYES == AfxMessageBox(szErrMsg, MB_APPLMODAL | MB_ICONQUESTION | MB_YESNO))
                MessageBox.Show(Dami.TraduCuvant(szErrMsg));
                    return;
            }

            
            if (!avans)
            {
            int zile2 = 0;
            int zile = 0;
            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) >= 0)
            {
                zile = Convert.ToInt32(txtCT1.Text);
                zile2 = Convert.ToInt32(txtCT2.Text);
                if (zile > 0 || (zile == 0 && zile2 == 0))
                {
                    procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) / 100);
                    suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));                    
                    AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()), zile, cc, procent, 0, true, marca, false);
                }
                }
            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) >= 0)
            {
                zile = Convert.ToInt32(txtCT2.Text);
                if (zile > 0)
                    {
                        procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) / 100);  
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                    AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()), zile, cc, procent, 0, true, marca, false);
                }
                }
            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) >= 0)
            {
                    zile = Convert.ToInt32(txtCT3.Text);
                if (zile > 0)
                    {
                        procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) / 100);
                        // mihad 19.03.2018
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                    //
                    AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()), zile, cc, procent, 0, true, marca, false);
                }
                }
            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) >= 0)
            {
                    zile = Convert.ToInt32(txtCT1.Text);
                if (zile >= 0)  //Radu 27.05.2015	- am pus >= ca sa poata fi introduse concedii medicale cu Cod 4 de 0 ZL
                    {
                        procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) / 100);
                        // mihad 19.03.2018
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                    //
                    AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()), zile, cc, procent, 0, true, marca, false);
                }
                }

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC5"].ToString()) >= 0)
            {
                zile = Convert.ToInt32(txtCT5.Text);
                if (zile > 0)
                    {
                        // cod 4449 in loc de 4450 pentru concediile pe perioada 01.01.2018 - 30.06.2018
                        if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()) == 4450)
                        {
                            DataTable dt069 = General.IncarcaDT("SELECT * FROM F069 WHERE F06904 = " + dtLC.Rows[0][1].ToString() + " AND F06905 = " + dtLC.Rows[0][0].ToString(), null);
                            
                            // venituri mici ce le iese suma CM mai mica decat plafon
                            suma4449 = rotunjire(2, 1, (((3131.0 / Convert.ToInt32(dt069.Rows[0]["F06907"].ToString())) * 0.35) * zile * 0.105));
                            suma4450 = rotunjire(2, 1, suma4450);

                            if (rotunjire(2, 1, (suma4450 * 0.25)) < suma4449)
                            {
                                suma_4450_subplafon = suma4450;
                            }
                            //
                            else
                            {
                                suma_4450_subplafon = 0;

                                
                                if (dtStart.Year == 2018 && dtStart.Month <= 6)
                                    dtMARDEF.Rows[0]["CODE5"] = 4449;
                                else
                                {
                                    // caut daca nu e cod special		
                                    string szCoduri = "";
                                    dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CM_TARIF35'", null);
                                    if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                                        szCoduri = dtParam.Rows[0][0].ToString(); 

                                    if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && dtStart.Year == 2018 && dtStart.Month <= 9)
                                    {
                                        dtMARDEF.Rows[0]["CODE5"] = 4449;
                                    }
                                    else
                                    {
                                        // caut initialul daca nu e in perioada
                                        string serie_i = "", numar_i = "";
                                        serie_i = txtSCMInit.Text;
                                        numar_i = txtNrCMInit.Text;
                                        if (serie_i.Length > 0 && numar_i.Length > 0)
                                        {
                                            string szT = "";
                                            if (Constante.tipBD == 2)
									            szT = "SELECT TO_CHAR(F94037,'DD/MM/YYYY') FROM F940 WHERE F940601 = '{0}' AND F940602 = '{1}' AND F94003 = {2} AND F94010 >= 4401 AND F94010 < 4449 ORDER BY F94010";
                                            else
                                                szT = "SELECT CONVERT(VARCHAR,F94037,103) FROM F940 WHERE F940601 = '{0}' AND F940602 = '{1}' AND F94003 = {2} AND F94010 >= 4401 AND F94010 < 4449 ORDER BY F94010";

                                            sql = "";
                                            sql =  string.Format(szT, serie_i, numar_i, marca);

                                            DataTable dtIstoric = General.IncarcaDT(sql, null);
                                                                                  
                                            if (dtIstoric != null && dtIstoric.Rows.Count > 0)
                                            {
                                                for (int i = 0; i < dtIstoric.Rows.Count; i++)
                                                {
                                                    DateTime data_i = new DateTime(Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(6, 4)), Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(3, 2)), Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(0, 2)));
                                                   
                                                    if (data_i.Year == 2018 && data_i.Month <= 6)
                                                        dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                    else
                                                    {
                                                        if ((Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0) && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && dtStart.Year == 2018 && dtStart.Month <= 9)
                                                        {
                                                            dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC5"].ToString()) / 100);
                        DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE5"].ToString(), null);
                        
                        if (dtF300!= null && dtF300.Rows.Count > 0)
                        {
                            if (!(chkStagiu.Checked && (Convert.ToInt32(dtF300.Rows[0]["F30010"].ToString()) == 4450 || Convert.ToInt32(dtF300.Rows[0]["F30010"].ToString()) == 4449)))       
                            {
                                string sName = dtMARDEF.Rows[0]["NAME"].ToString();
                                if (!(Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || sName.Contains("AMBP")))
                                {
                                    dtF300.Rows[0]["F30013"] = Convert.ToInt32(dtF300.Rows[0]["F30013"].ToString()) + zile;
                                    dtF300.Rows[0]["F30015"] = Convert.ToInt32(dtF300.Rows[0]["F30015"].ToString()) + suma_4450_subplafon;
                                    sql = "UPDATE F300 SET F30013 = {0}, F30015 = {1} WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE5"].ToString();
                                    sql = string.Format(sql, dtF300.Rows[0]["F30013"].ToString(), dtF300.Rows[0]["F30015"].ToString());
                                    General.ExecutaNonQuery(sql, null);
                                }
                            }
                        }
                        else
                        AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()), zile, cc, procent, 0, true, marca, false);

                    }
                }

            if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) >= 0)
            {
                    zile = Convert.ToInt32(txtCT6.Text);
                    if (zile > 0)
                    {
                        procent = GetMARpercent(Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString()), vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) / 100);
                        DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE6"].ToString(), null);

                        if (dtF300 != null && dtF300.Rows.Count > 0)
                        {
                            string sz;
                            dtF300.Rows[0]["F300611"] = Convert.ToInt32(cmbLocPresc.Value);
                            dtF300.Rows[0]["F300612"] = Convert.ToInt32(txtBCCM.Text);
                            dtF300.Rows[0]["F300613"] = Convert.ToInt32(txtZBC.Text);
                            dtF300.Rows[0]["F300614"] = Convert.ToInt32(txtMZ.Text);
                            dtF300.Rows[0]["F300615"] = txtNrAviz.Text;
                            dtF300.Rows[0]["F300616"] = txtMedic.Text;
                            dtF300.Rows[0]["F300617"] = txtCNP.Text;
                        
                            string sName = dtMARDEF.Rows[0]["NAME"].ToString();
                            if (!(Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || sName.Contains("AMBP")))
                                {
                                    dtF300.Rows[0]["F30013"] = Convert.ToInt32(dtF300.Rows[0]["F30013"].ToString()) + zile;
                                    sql = "UPDATE F300 SET F30013 = {0}, F300611 = {1}, F300612 = {2}, F300613 = {3}, F300614 = {4}, F300615 = {5}, F300616 = {6}, F300617 = {7} WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE6"].ToString();
                                    sql = string.Format(sql, dtF300.Rows[0]["F30013"].ToString(), dtF300.Rows[0]["F300611"].ToString(), dtF300.Rows[0]["F300612"].ToString(), dtF300.Rows[0]["F300613"].ToString(), dtF300.Rows[0]["F300614"].ToString(),
                                         dtF300.Rows[0]["F300615"].ToString(), dtF300.Rows[0]["F300616"].ToString(), dtF300.Rows[0]["F300617"].ToString());
                                    General.ExecutaNonQuery(sql, null);
                        }
                            }
                            else
                            AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString()), zile, cc, procent, 0, true, marca, false);
                        
                    }
                }
            }
            else
            {
                int zile = Convert.ToInt32(txtNrZile.Text);
                AddConcediu(0, zile, cc, 0, 0, false, marca, true);
            }
            Session["MARDEF"] = dtMARDEF;
            MessageBox.Show("Proces realizat cu succes!");
        }
        
        bool AddConcediu(int cod, int zile, int cc, double proc, double suma_4450_subplafon, bool bFTarif, int marca, bool avans)
        {
            double suma = 0, tarif = 0;

            string sqlAng = "SELECT * FROM F100 WHERE F10003 = " + marca;
            DataTable dtAng = General.IncarcaDT(sqlAng, null);

            //ASPxComboBox cmbLocPresc = DataList1.Items[0].FindControl("cmbLocPresc") as ASPxComboBox;
            //ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            //ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
            //ASPxDateEdit deDataAviz = DataList1.Items[0].FindControl("deDataAviz") as ASPxDateEdit;
            //ASPxDateEdit deData = DataList1.Items[0].FindControl("deData") as ASPxDateEdit;
            //ASPxTextBox txtSerie = DataList1.Items[0].FindControl("txtSerie") as ASPxTextBox;
            //ASPxTextBox txtNr = DataList1.Items[0].FindControl("txtNr") as ASPxTextBox;
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtDetalii = DataList1.Items[0].FindControl("txtDetalii") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtCodIndemn = DataList1.Items[0].FindControl("txtCodIndemn") as ASPxTextBox;
            //ASPxTextBox txtCodDiag = DataList1.Items[0].FindControl("txtCodDiag") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;
            //ASPxTextBox txtCodUrgenta = DataList1.Items[0].FindControl("txtCodUrgenta") as ASPxTextBox;
            //ASPxTextBox txtCodInfCont = DataList1.Items[0].FindControl("txtCodInfCont") as ASPxTextBox;
            //ASPxTextBox txtBCCM = DataList1.Items[0].FindControl("txtBCCM") as ASPxTextBox;
            //ASPxTextBox txtZBC = DataList1.Items[0].FindControl("txtZBC") as ASPxTextBox;
            //ASPxTextBox txtMZ = DataList1.Items[0].FindControl("txtMZ") as ASPxTextBox;
            //ASPxTextBox txtNrAviz = DataList1.Items[0].FindControl("txtNrAviz") as ASPxTextBox;
            //ASPxTextBox txtMedic = DataList1.Items[0].FindControl("txtMedic") as ASPxTextBox;
            //ASPxTextBox txtCNP = DataList1.Items[0].FindControl("txtCNP") as ASPxTextBox;
            //ASPxCheckBox chkCalcul = DataList1.Items[0].FindControl("chkCalcul") as ASPxCheckBox;
            //ASPxCheckBox chkStagiu = DataList1.Items[0].FindControl("chkStagiu") as ASPxCheckBox;



            DateTime dtStart = Convert.ToDateTime(deDeLaData.Value);
            DateTime dtEnd = Convert.ToDateTime(deLaData.Value);
            DateTime dtData = Convert.ToDateTime(deData.Value);
            DateTime dtAviz = Convert.ToDateTime(deDataAviz.Value);
            DateTime dtDataCMInit = Convert.ToDateTime(deDataCMInit.Value);

            string detalii = txtDetalii.Text;
            string BCCM = txtBCCM.Text.Length <= 0 ? "0" : txtBCCM.Text;
            string ZileBCCM = txtZBC.Text.Length <= 0 ? "0" : txtZBC.Text;
            string MZCM = txtMZ.Text.Length <= 0 ? "0" : txtMZ.Text;

            if (cod == 4450)
                suma += suma_4450_subplafon;

            if (cc == 0 || cc == 9999)
            {
                if (dtAng.Rows[0]["F10053"] != null && dtAng.Rows[0]["F10053"].ToString().Length > 0 &&  Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString())  != 0 && Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString()) != 9999)
                    cc = Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString());
                else
                {
                    string sqlDep = "SELECT F00615 FROM F006 WHERE F00607 = " + Convert.ToInt32(dtAng.Rows[0]["F10007"].ToString());
                    DataTable dtDep = General.IncarcaDT(sqlDep, null);
                    if (dtDep != null && dtDep.Rows.Count > 0 && dtDep.Rows[0][0] != null && dtDep.Rows[0][0].ToString().Length > 0)
                        cc = Convert.ToInt32(dtDep.Rows[0][0].ToString());  
                }
            }

            if (bFTarif)
                tarif = Convert.ToDouble(txtMZ.Text);
            
            if (chkCalcul.Checked)
            {
                detalii = "CALCUL ZILE MANUAL";
                if (chkStagiu.Checked)
                {
                    detalii += " / NU ARE STAGIU COTIZARE";
                    //DM.Zile5 = 0;
                    tarif = 0;
                    BCCM = "0";
                    ZileBCCM = "0";
                    MZCM = "0";
                }
            }
            else if (chkStagiu.Checked)
            {
                detalii = "NU ARE STAGIU COTIZARE";
                //DM.Zile5 = 0;
                tarif = 0;
                BCCM = "0";
                ZileBCCM = "0";
                MZCM = "0";
            }

            string cmpAvans = "", valAvans = "";
            if (avans)
            {
                cmpAvans = ", ZILE, ZILE1, ZILE2, ZILE3, ZILE5, ZILE6, TIP, OPTIUNE, F30051, F30052";
                valAvans = ", {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, 0, 0";
                valAvans = string.Format(valAvans, (txtNrZile.Text.Length <= 0 ? "0" : txtNrZile.Text), (txtCT1.Text.Length <= 0 ? "0" : txtCT1.Text), (txtCT2.Text.Length <= 0 ? "0" : txtCT2.Text), (txtCT3.Text.Length <= 0 ? "0" : txtCT3.Text),
                    (txtCT5.Text.Length <= 0 ? "0" : txtCT5.Text), (txtCT6.Text.Length <= 0 ? "0" : txtCT6.Text), cmbTipConcediu.SelectedItem.Value.ToString(), (rbOptiune1.Checked ? "1" : "2"));
            }


            if (dtAviz.Year <= 1900)
                dtAviz = new DateTime(2100, 1, 1);

            string sql = "INSERT INTO " + (avans ? "F300_CM_AVANS" : "F300" ) + " (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30012, F30013, F30014, F30015, F30021, F30022, F30023, F30036, F30037, F30038, F30050, " +
                " F300601, F300602, F300603,  F30053, F300618, F30039, F30040, F30042, F30035, F300606, F300607, F300619, F300608, F300609, F300610, F300611, F300612, F300613, F300614, F300615, F300616, F300617, F300621, " + (avans ? cmpAvans : "") + ") ";

            sql += "VALUES (300, 1, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, '{18}', '{19}', {20}, {21}, {22}, {23}, {24}, '{25}', {26}, '{27}', '{28}', '{29}', '{30}', '{31}', " 
                + " '{32}',  {33}, {34}, {35}, {36}, '{37}', '{38}', '{39}' {40} )";

            sql = string.Format(sql, dtAng.Rows[0]["F10003"].ToString(), dtAng.Rows[0]["F10004"].ToString(), dtAng.Rows[0]["F10005"].ToString(), dtAng.Rows[0]["F10006"].ToString(), dtAng.Rows[0]["F10007"].ToString(), //4
                cod, 1, tarif.ToString(new CultureInfo("en-US")), zile, proc.ToString(new CultureInfo("en-US")), suma.ToString(new CultureInfo("en-US")), 0, 0, 0, //13
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') +  "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)" 
                : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //14
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //15
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 'dd/mm/yyyy')"), cc, txtSerie.Text, txtNr.Text, //19
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 'dd/mm/yyyy')"), (txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text), //21
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 'dd/mm/yyyy')"), 0, 0, detalii, //25
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"), txtSCMInit.Text, txtCodIndemn.Text, txtCodDiag.Text,  //29
                txtNrCMInit.Text, txtCodUrgenta.Text, txtCodInfCont.Text, (cmbLocPresc.SelectedItem == null ? "0" : cmbLocPresc.SelectedItem.Value.ToString()), BCCM, ZileBCCM, MZCM, txtNrAviz.Text, txtMedic.Text, txtCNP.Text,
                (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 103)"
                : "TO_DATE('" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 'dd/mm/yyyy')"), (avans ? valAvans : "")); //41


	        if(!((chkStagiu.Checked && (cod == 4450 || cod == 4449))))		
		        if(General.ExecutaNonQuery(sql, null))
		        {	
			        if(chkStagiu.Checked)
			        {
                         int nn = 0;

                        string sqlParam = "SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'BCCM'";
                        DataTable dtParam = General.IncarcaDT(sqlParam, null);
                        if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
                        {
                            nn = Convert.ToInt32(dtParam.Rows[0][0].ToString()) - 1;
                            string sqltmp = "UPDATE F100 SET F10069{0} = 0.0 WHERE F10003 = {1}";
                            sqltmp = string.Format(sqltmp, nn, marca);
                            if (!General.ExecutaNonQuery(sqltmp, null))
                                MessageBox.Show(Dami.TraduCuvant("Nu am putut actualiza Tariful CM pe angajat!"));
                        }
                    }			        
		        }
		        else
		        {
                    MessageBox.Show(Dami.TraduCuvant("Tranzactia nu a fost adaugata!"));     
			        return false;
		        }
	        return true;
        }

        void OnUpdateZ1()
        {
            //ASPxTextBox txtCT1 = DataList1.Items[0].FindControl("txtCT1") as ASPxTextBox;
            //ASPxTextBox txtCT2 = DataList1.Items[0].FindControl("txtCT2") as ASPxTextBox;
            //ASPxTextBox txtCT3 = DataList1.Items[0].FindControl("txtCT3") as ASPxTextBox;
            //ASPxTextBox txtCT5 = DataList1.Items[0].FindControl("txtCT5") as ASPxTextBox;
            //ASPxTextBox txtCT6 = DataList1.Items[0].FindControl("txtCT6") as ASPxTextBox;
            //ASPxTextBox txtNrZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxCheckBox chkCalcul = DataList1.Items[0].FindControl("chkCalcul") as ASPxCheckBox;
            //ASPxDateEdit deDeLaData  = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            //ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;

            int Z2 = 0, Z3 = 0;
            if (txtCT1.Text.Length <= 0) return;
            int ZL = 0;
            bool trans = int.TryParse(txtNrZile.Text, out ZL);

            int ZLA = 0;
            int.TryParse(txtZCMAnt.Text, out ZLA);

            int Z1 = 0;
            int.TryParse(txtCT1.Text, out Z1);
            
            DataTable dtMARDEF = new DataTable();
            if (Session["MARDEF"] == null)
            {
                int no = Convert.ToInt32(cmbTipConcediu.SelectedItem.Value);
                string sql = "SELECT * FROM MARDEF WHERE NO = " + no;
                dtMARDEF = General.IncarcaDT(sql, null);
                Session["MARDEF"] = dtMARDEF;
            }
            else
                dtMARDEF = Session["MARDEF"] as DataTable;

            int ZileAng = Convert.ToInt32(Session["ZileAng"].ToString());

            int code1 = (dtMARDEF.Rows[0]["CODE1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) : 0);
            int code2 = (dtMARDEF.Rows[0]["CODE2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) : 0);
            int code3 = (dtMARDEF.Rows[0]["CODE3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) : 0);
            int code4 = (dtMARDEF.Rows[0]["CODE4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) : 0);

            int type = (code1 > 0 ? 1 : 0) * 8 + (code2 > 0 ? 1 : 0) * 4 + (code3 > 0 ? 1 : 0) * 2 + (code4 > 0 ? 1 : 0);

            int lim = 31;

            switch (type)
            {
                case 14:
                    if (dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP"))
                    {
                        lim = 3;
                    }
                    break;
                case 12:
                    lim = ZileAng;
                    break;
                default:
                    lim = 0;
                    break;
            }

            if (!trans || (((Z1 + ZLA) > lim) && (Z1 > 0)) && !chkCalcul.Checked)
            {
                string msg = "Introduceti un numar de zile ( Zile < 3 + Zile luna anterioara ) <= {0} !";
                msg = string.Format(msg, lim);
                pnlCtl.JSProperties["cpAlertMessage"] = msg;
                txtCT1.Text = "";
                txtCT1.Focus();
                return;
            }
            if (Z1 > ZL)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = "Introduceti un numar de zile mai mic sau egal cu numarul total de zile !";
                txtCT1.Text = "";
                txtCT1.Focus();
                return;
            }

            int nMin;
            int nrZL = 0;
            DateTime  dtb = DateTime.Now, dte = DateTime.Now;

            if (Session["CM_StartDate"] != null)
            {
                dtb = Convert.ToDateTime(Session["CM_StartDate"]);
            }

            if (Session["CM_EndDate"] != null)
            {
                dte = Convert.ToDateTime(Session["CM_EndDate"]);
            }

            switch (type)
            {
                case 14:
                    nMin = Math.Min(ZileAng - ZLA - 1, (dte - dtb).Days);
                
                    if (nMin > 0)
                    {
                        for (DateTime dt = dtb; dt <= dtb.AddDays(nMin); dt = dt.AddDays(1))
                        {
                            if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(dt))
                                nrZL++;
                        }
                        Z2 = (nrZL - Z1) * (nrZL - Z1 > 0 ? 1 : 0);
                    }
                    else
                        Z2 = 0;
                    Z3 = ZL - Z2 - Z1;
                    break;
                case 12:
                    Z2 = ZL - Z1;
                    Z3 = 0;
                    break;
                default:
                    Z2 = 0;
                    Z3 = 0;
                    break;
            }

            txtCT2.Text = Z2.ToString();
            txtCT3.Text = Z3.ToString();

            int total = 0;
            string s = dtMARDEF.Rows[0]["NAME"].ToString();
            if (Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || s.Contains("AMBP"))
            {
                total = 1;
            }
            else
            {
                int add1 = (dtMARDEF.Rows[0]["ADD1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD1"].ToString()) : 0);
                int add2 = (dtMARDEF.Rows[0]["ADD2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD2"].ToString()) : 0);
                int add3 = (dtMARDEF.Rows[0]["ADD3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD3"].ToString()) : 0);
                int add4 = (dtMARDEF.Rows[0]["ADD4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD4"].ToString()) : 0);

                if (add1 == 1)
                {
                    total += txtCT1.Text.Length > 0 ? Convert.ToInt32(txtCT1.Text) : 0;
                }
                if (add2 == 1)
                {
                    total += txtCT2.Text.Length > 0 ? Convert.ToInt32(txtCT2.Text) : 0;
                }
                if (add3 == 1)
                {
                    total += txtCT3.Text.Length > 0 ? Convert.ToInt32(txtCT3.Text) : 0;
                }
                if (add4 == 1)
                {
                    total += txtNrZile.Text.Length > 0 ? Convert.ToInt32(txtNrZile.Text) : 0;
                }
            }
            txtCT5.Text = total.ToString();
            txtCT6.Text = total.ToString();            
        }

        void OnUpdateZL()
        {
            //ASPxTextBox txtNrZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtCT1 = DataList1.Items[0].FindControl("txtCT1") as ASPxTextBox;
            //ASPxTextBox txtCT2 = DataList1.Items[0].FindControl("txtCT2") as ASPxTextBox;
            //ASPxTextBox txtCT3 = DataList1.Items[0].FindControl("txtCT3") as ASPxTextBox;
            //ASPxTextBox txtCT5 = DataList1.Items[0].FindControl("txtCT5") as ASPxTextBox;
            //ASPxTextBox txtCT6 = DataList1.Items[0].FindControl("txtCT6") as ASPxTextBox;

            if (txtNrZile.Text.Length <= 0) return;
            int ZL = 0;
            int.TryParse(txtNrZile.Text, out ZL);

            int ZLA = 0;
            int.TryParse(txtZCMAnt.Text, out ZLA);

            DataTable dtMARDEF = new DataTable();
            if (Session["MARDEF"] == null)
            {
                int no = Convert.ToInt32(cmbTipConcediu.SelectedItem.Value);
                string sql = "SELECT * FROM MARDEF WHERE NO = " + no;
                dtMARDEF = General.IncarcaDT(sql, null);
                Session["MARDEF"] = dtMARDEF;
            }
            else
                dtMARDEF = Session["MARDEF"] as DataTable;

            int code1 = (dtMARDEF.Rows[0]["CODE1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) : 0);
            int code2 = (dtMARDEF.Rows[0]["CODE2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) : 0);
            int code3 = (dtMARDEF.Rows[0]["CODE3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) : 0);
            int code4 = (dtMARDEF.Rows[0]["CODE4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) : 0);

            int type = (code1 > 0 ? 1 : 0) * 8 + (code2 > 0 ? 1 : 0) * 4 + (code3 > 0 ? 1 : 0) * 2 + (code4 > 0 ? 1 : 0);
            int ZileAng = Convert.ToInt32(Session["ZileAng"].ToString());
            int nMin, Z1 = 0;

            switch (type)
            {
                case 14:
                    if (dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP"))
                    {
                        nMin = Math.Min(3 - ZLA, ZL);
                        Z1 = nMin * (nMin > 0 ? 1 : 0);
                    }
                    break;
                case 12:
                    nMin = Math.Min(ZileAng - ZLA, ZL);
                    Z1 = nMin * (nMin > 0 ? 1 : 0);
                    break;
                default:
                    Z1 = 0;
                    break;
            }

            txtCT1.Text = Z1.ToString();
            //OnUpdateZ1();
            
            int total = 0;
            string s = dtMARDEF.Rows[0]["NAME"].ToString();
            if (Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || s.Contains("AMBP"))
            {
                total = 1;
            }
            else
            {
                int add1 = (dtMARDEF.Rows[0]["ADD1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD1"].ToString()) : 0);
                int add2 = (dtMARDEF.Rows[0]["ADD2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD2"].ToString()) : 0);
                int add3 = (dtMARDEF.Rows[0]["ADD3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD3"].ToString()) : 0);
                int add4 = (dtMARDEF.Rows[0]["ADD4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD4"].ToString()) : 0);

                if (add1 == 1)
                {
                    total += txtCT1.Text.Length > 0 ? Convert.ToInt32(txtCT1.Text) : 0;
                }
                if (add2 == 1)
                {
                    total += txtCT2.Text.Length > 0 ? Convert.ToInt32(txtCT2.Text) : 0;
                }
                if (add3 == 1)
                {
                    total += txtCT3.Text.Length > 0 ? Convert.ToInt32(txtCT3.Text) : 0;
                }
                if (add4 == 1)
                {
                    total += txtNrZile.Text.Length > 0 ? Convert.ToInt32(txtNrZile.Text) : 0;
                }
            }
            txtCT5.Text = total.ToString();
            txtCT6.Text = total.ToString();
        }

        private void OnSelChangeTip()
        {
            //ASPxComboBox cmbTipConcediu = DataList1.Items[0].FindControl("cmbTipConcediu") as ASPxComboBox;
            //ASPxComboBox cmbCT1 = DataList1.Items[0].FindControl("cmbCT1") as ASPxComboBox;
            //ASPxComboBox cmbCT2 = DataList1.Items[0].FindControl("cmbCT2") as ASPxComboBox;
            //ASPxComboBox cmbCT3 = DataList1.Items[0].FindControl("cmbCT3") as ASPxComboBox;
            //ASPxComboBox cmbCT4 = DataList1.Items[0].FindControl("cmbCT4") as ASPxComboBox;
            //ASPxComboBox cmbCT5 = DataList1.Items[0].FindControl("cmbCT5") as ASPxComboBox;
            //ASPxComboBox cmbCT6 = DataList1.Items[0].FindControl("cmbCT6") as ASPxComboBox;
            //ASPxTextBox txtCodIndemn = DataList1.Items[0].FindControl("txtCodIndemn") as ASPxTextBox;
            //ASPxTextBox txtCodDiag = DataList1.Items[0].FindControl("txtCodDiag") as ASPxTextBox;
            //ASPxRadioButton rbZileCal = DataList1.Items[0].FindControl("rbZileCal") as ASPxRadioButton;
            //ASPxRadioButton rbZileFNUASS = DataList1.Items[0].FindControl("rbZileFNUASS") as ASPxRadioButton;

            int no = Convert.ToInt32(cmbTipConcediu.SelectedItem.Value);
            string sql = "SELECT * FROM MARDEF WHERE NO = " + no;
            DataTable dtMARDEF = General.IncarcaDT(sql, null);
                                    
            txtCodIndemn.Text = "";          

            if (dtMARDEF.Rows[0]["CODIND"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODIND"].ToString()) > 0)
            {
                txtCodIndemn.Text = dtMARDEF.Rows[0]["CODIND"].ToString().PadLeft(2, '0');
            }
            //OnKillfocus93CodIndemnizatie();

            cmbCT1.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString());
            cmbCT2.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString());
            cmbCT3.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString());
            cmbCT4.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString());
            cmbCT5.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString());
            cmbCT6.Value = Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString());


            if (txtCodIndemn.Text == "15")
                txtCodDiag.Text = "RM";
            else
                txtCodDiag.Text = "";

            if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP") && (dtMARDEF.Rows[0]["CODE1"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 ||
                dtMARDEF.Rows[0]["CODE2"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 || dtMARDEF.Rows[0]["CODE3"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0))
            {
                rbZileCal.Text = "5 zile calendaristice";
                rbZileFNUASS.Text = "0 zile (din FNUASS)";
                rbZileCal.Checked = true;
            }
            else
            {
                if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP"))
                {
                    rbZileCal.Text = "5 zile calendaristice";
                    rbZileFNUASS.Text = "0 zile (din FNUASS)";
                }
                else
                {
                    rbZileCal.Text = "3 zile calendaristice";
                    rbZileFNUASS.Text = "0 zile (din AMBP)";
                }
            }
            Session["MARDEF"] = dtMARDEF;
            OnZileAng();
        }

        void OnSelStartDate()
        {
            Session["CM_StartDate"] = Convert.ToDateTime(deDeLaData.Value);
            InitWorkingDays();
        }

        void OnSelEndDate()
        {
            Session["CM_EndDate"] = Convert.ToDateTime(deLaData.Value);
            OnSelStartDate();
        }

        void InitWorkingDays()
        {
            //ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;
            //ASPxDateEdit deLaData = DataList1.Items[0].FindControl("deLaData") as ASPxDateEdit;
            //ASPxTextBox txtNrZile = DataList1.Items[0].FindControl("txtNrZile") as ASPxTextBox;
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtCT1 = DataList1.Items[0].FindControl("txtCT1") as ASPxTextBox;
            //ASPxTextBox txtCT2 = DataList1.Items[0].FindControl("txtCT2") as ASPxTextBox;
            //ASPxTextBox txtCT3 = DataList1.Items[0].FindControl("txtCT3") as ASPxTextBox;
            //ASPxTextBox txtCT5 = DataList1.Items[0].FindControl("txtCT5") as ASPxTextBox;
            //ASPxTextBox txtCT6 = DataList1.Items[0].FindControl("txtCT6") as ASPxTextBox;

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);

            DateTime dtTmp = Convert.ToDateTime(Session["CM_StartDate"]);
            if (dtTmp.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString()))
            {
                rbOptiune1.Visible = true;
                rbOptiune2.Visible = true;
                rbOptiune1.Checked = true;
                btnMZ.Enabled = false;
            }
            else
            {
                rbOptiune1.Visible = false;
                rbOptiune2.Visible = false;
                btnMZ.Enabled = true;
            }

            DataTable dtMARDEF = new DataTable();
            if (Session["MARDEF"] == null)
            {
                int no = Convert.ToInt32(cmbTipConcediu.SelectedItem.Value);
                sql = "SELECT * FROM MARDEF WHERE NO = " + no;
                dtMARDEF = General.IncarcaDT(sql, null);
                Session["MARDEF"] = dtMARDEF;
            }
            else
                dtMARDEF = Session["MARDEF"] as DataTable;
            DateTime dtb, dte;
            
            if (Session["CM_StartDate"] != null)
            {
                dtb = Convert.ToDateTime(Session["CM_StartDate"]);
            }
            else
            {
                txtNrZile.Text = "";
                OnUpdateZL();
                return;
            }
            
            if (Session["CM_EndDate"] != null)
            {
                dte = Convert.ToDateTime(Session["CM_EndDate"]);
            }
            else
            {
                txtNrZile.Text = "";
                OnUpdateZL();
                return;
            }
            
            int dow;
            int nrZL = 0, nrZC = 0;
            for (DateTime dt = dtb; dt <= dte; dt = dt.AddDays(1))
            {
                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(dt))
                    nrZL++;
                nrZC++;
            }

            int nrZL3 = 0;
            int ZLA = 0;
            int.TryParse(txtZCMAnt.Text, out ZLA);
   
            if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP") && (dtMARDEF.Rows[0]["CODE1"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 ||
                dtMARDEF.Rows[0]["CODE2"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 || dtMARDEF.Rows[0]["CODE3"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0))
            {
                int nDays = 0;
                for (DateTime dt = dtb; dt <= dte && nDays < (5 - ZLA); dt = dt.AddDays(1))
                {
                    if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(dt))
                        nrZL3++;
                    nDays++;
                }
            }
            else
            {
                int code1 = (dtMARDEF.Rows[0]["CODE1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) : 0);
                int code2 = (dtMARDEF.Rows[0]["CODE2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) : 0);
                int code3 = (dtMARDEF.Rows[0]["CODE3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) : 0);
                int code4 = (dtMARDEF.Rows[0]["CODE4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) : 0);
                
                int type = (code1 > 0 ? 1 : 0) * 8 + (code2 > 0 ? 1 : 0) * 4 + (code3 > 0 ? 1 : 0) * 2 + (code4 > 0 ? 1 : 0);
                int nDays = 0;
                int ZileAng = Convert.ToInt32(Session["ZileAng"].ToString());
                switch (type)
                {
                    case 14:
                        nDays = 0;
                        for (DateTime dt = dtb; dt <= dte && (dt - dtb).Days < 3 - ZLA; dt = dt.AddDays(1)) 
                        {
                            if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(dt))
                                 nrZL3++;
                            nDays++;
                        }
                        nrZL3 = Math.Min((3 - ZLA) * ((3 - ZLA) > 0 ? 1 : 0), nrZL3);
                        break;
                    case 12:
                        for (DateTime dt = dtb; dt <= dte && (dt - dtb).Days < ZileAng - ZLA; dt = dt.AddDays(1))
                        {
                            if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(dt))
                                nrZL3++;
                        }
                        nrZL3 = Math.Min((ZileAng - ZLA) * ((ZileAng - ZLA) > 0 ? 1 : 0), nrZL3);
                        break;
                }
            }
            txtNrZile.Text = nrZL.ToString(); OnUpdateZL();
            txtCT1.Text = nrZL3.ToString(); OnUpdateZ1();
            string s;
            int total = 0;
            
            s = dtMARDEF.Rows[0]["NAME"].ToString();
            if (Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 3 || Convert.ToInt32(dtMARDEF.Rows[0]["NO"].ToString()) == 4 || s.Contains("AMBP"))
            {
                total = 1;
            }
            else
            {
                int add1 = (dtMARDEF.Rows[0]["ADD1"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD1"].ToString()) : 0);
                int add2 = (dtMARDEF.Rows[0]["ADD2"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD2"].ToString()) : 0);
                int add3 = (dtMARDEF.Rows[0]["ADD3"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD3"].ToString()) : 0);
                int add4 = (dtMARDEF.Rows[0]["ADD4"] != null ? Convert.ToInt32(dtMARDEF.Rows[0]["ADD4"].ToString()) : 0);

                if (add1 == 1)
                {
                    total += txtCT1.Text.Length > 0 ? Convert.ToInt32(txtCT1.Text) : 0;
                }
                if (add2 == 1)
                {
                    total += txtCT2.Text.Length > 0 ? Convert.ToInt32(txtCT2.Text) : 0;
                }
                if (add3 == 1)
                {
                    total += txtCT3.Text.Length > 0 ? Convert.ToInt32(txtCT3.Text) : 0;
                }
                if (add4 == 1)
                {
                    total += txtNrZile.Text.Length > 0 ? Convert.ToInt32(txtNrZile.Text) : 0;
                }
            }
            txtCT5.Text = total.ToString();
            txtCT6.Text = total.ToString();            
        }

        bool IsHoliday(DateTime dt)
        {
            string sql = "SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') +  "/" + dt.Year + "', 103)" 
                : "TO_DATE('" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year + "', 'dd/mm/yyyy')");
            DataTable table = General.IncarcaDT(sql, null);
            if (table == null || table.Rows.Count <= 0 || table.Rows[0][0] == null || table.Rows[0][0].ToString().Length <= 0 || table.Rows[0][0].ToString() == "0")
                return false;
            else
                return true; 
        }


        void OnUpdateZLP()
        {
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxRadioButton rbConcCont = DataList1.Items[0].FindControl("rbConcCont") as ASPxRadioButton;
            //ASPxRadioButton rbConcInit = DataList1.Items[0].FindControl("rbConcInit") as ASPxRadioButton;

            bool T = false;            
            if (txtZCMAnt.Text.Length > 0)
                T = true;
            rbConcCont.Checked = T;
            rbConcInit.Checked = !T;
            InitWorkingDays();

            if (T)
                On93Initial();
            else
                On93Prel();
        }

        double ConvertVechime(string vechime)
        {
            string an, luna;

            if (vechime.Substring(0, 1) == "0")
                an = vechime.Substring(1, 1);
            else
                an = vechime.Substring(0, 2);

            luna = vechime.Substring(2, 2);
            int l = Convert.ToInt32(luna);
            int a = Convert.ToInt32(an);
            return (double)((double)l / 12.0 + (double)a);
        }


        void OnSelChangeCostCenter()
        {
            int pos, No;
            //ASPxComboBox cmbCC = DataList1.Items[0].FindControl("cmbCC") as ASPxComboBox;
            //ASPxTextBox txtCC = DataList1.Items[0].FindControl("txtCC") as ASPxTextBox;

            if (cmbCC.SelectedIndex > -1)
            {
                No = Convert.ToInt32(cmbCC.SelectedItem.Value);
                txtCC.Text = No.ToString();
            }
        }

        void OnUpdateCcNo()
        {
            //ASPxTextBox txtCC = DataList1.Items[0].FindControl("txtCC") as ASPxTextBox;
            //ASPxComboBox cmbCC = DataList1.Items[0].FindControl("cmbCC") as ASPxComboBox;

            string csText = txtCC.Text;
            int no = -1;
            int.TryParse(csText, out no);

            if (no >= 0)
                cmbCC.Value = no;
        }


        void OnZileAng()
        {
            //ASPxRadioButton rbZileCal = DataList1.Items[0].FindControl("rbZileCal") as ASPxRadioButton;
            //ASPxRadioButton rbZileFNUASS = DataList1.Items[0].FindControl("rbZileFNUASS") as ASPxRadioButton;

            if (rbZileCal.Checked)
                Session["ZileAng"] = "5";
            if (rbZileFNUASS.Checked)
                Session["ZileAng"] = "0";

            InitWorkingDays();
        }

        void On93Prel()
        {
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;
            //ASPxDateEdit deDeLaData = DataList1.Items[0].FindControl("deDeLaData") as ASPxDateEdit;

            txtZCMAnt.Enabled = true;
            txtSCMInit.Enabled = true;
            txtNrCMInit.Enabled = true;

  
        }

        void On93Initial()
        {
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;

            txtZCMAnt.Enabled = false;
            txtSCMInit.Enabled = false;
            txtNrCMInit.Enabled = false;
        }


        void OnButtonVizualizareZileCMdinIstoric()
        {             
            if (Session["CM_Preluare"] != null && Convert.ToInt32(Session["CM_Preluare"].ToString()) == 1)
            {
                txtZCMAnt.Text = Session["ZileCMAnterior"].ToString();
                string[] param = Session["SerieNrCMInitial"].ToString().Split(' ');
                txtSCMInit.Text = param[0];
                txtNrCMInit.Text = param[1];
                txtBCCM.Text = Session["BazaCalculCM"].ToString();
                txtZBC.Text = Session["ZileBazCalcul"].ToString();
                txtMZ.Text = Session["MediaZilnica"].ToString();

                On93Prel();
            }
            
        }

        void OnButtonMedie()
        {

            //Process process = new Process();
            //string fisierExp = HostingEnvironment.MapPath("~/CM/CM.exe"); 

            //process.StartInfo.FileName = fisierExp;
            //string arg = "1 {0} {1} {2}";
            //string data = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" +  DateTime.Now.Year.ToString();
            //string fisierCM = "CM1_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0')
            //                + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + ".tmp";
            //arg = string.Format(arg, Session["MarcaCM"].ToString(), data, fisierCM);
            //process.StartInfo.Arguments = arg;
            //process.StartInfo.ErrorDialog = true;
            //process.Start();
            //process.WaitForExit();

            double BCCM = 0, Medie6 = 0;
            int ZBCCM = 0;
            Module.ConcediiMedicale.CreateDetails(Convert.ToInt32(Session["MarcaCM"].ToString()), "", out Medie6, out BCCM, out ZBCCM);

            txtBCCM.Text = BCCM.ToString();
            txtZBC.Text = ZBCCM.ToString();
            txtMZ.Text = Medie6.ToString();



        }

     

        void OnKillfocus93CNPCopil()
        {
            //ASPxTextBox txtCNP = DataList1.Items[0].FindControl("txtCNP") as ASPxTextBox;            
            if (!General.VerificaCNP(txtCNP.Text))
            {               
                pnlCtl.JSProperties["cpAlertMessage"] = "Atentie! CNP invalid!";
                txtCNP.Focus();
            }
        }
        

        void OnKillfocus93CodIndemnizatie()
        {
            //ASPxTextBox txtCodIndemn = DataList1.Items[0].FindControl("txtCodIndemn") as ASPxTextBox;
            //ASPxComboBox cmbTipConcediu = DataList1.Items[0].FindControl("cmbTipConcediu") as ASPxComboBox;

            DataTable dtMARDEF = new DataTable();
            if (Session["MARDEF"] == null)
            {
                int no = Convert.ToInt32(cmbTipConcediu.SelectedItem.Value);
                string sql = "SELECT * FROM MARDEF WHERE NO = " + no;
                dtMARDEF = General.IncarcaDT(sql, null);
                Session["MARDEF"] = dtMARDEF;
            }
            else
                dtMARDEF = Session["MARDEF"] as DataTable;

            if (txtCodIndemn.Text.Length > 0)
	        {
                int cod_ind = -1;
                if (!int.TryParse(txtCodIndemn.Text, out cod_ind))
                    return;
			  
			    if (cod_ind == 15)
                    txtCodIndemn.Text ="RM";

                int no = -1;
                for (int i = 0; i < dtMARDEF.Rows.Count; i++)
                    if (Convert.ToInt32(dtMARDEF.Rows[i]["CODIND"].ToString()) == cod_ind)
                    {
                        no = Convert.ToInt32(dtMARDEF.Rows[i]["NO"].ToString());
                        break;
                    }

                if (no < 0)
                {
                    pnlCtl.JSProperties["cpAlertMessage"] = "Codul nu exista in baza de date!";
                    return;
                }

                cmbTipConcediu.Value = no;
                OnSelChangeTip();
	        }
        }


        void On93Zilemanual()
        {
            //ASPxCheckBox chkCalcul = DataList1.Items[0].FindControl("chkCalcul") as ASPxCheckBox;
            //ASPxTextBox txtCT2 = DataList1.Items[0].FindControl("txtCT2") as ASPxTextBox;
            //ASPxTextBox txtCT3 = DataList1.Items[0].FindControl("txtCT3") as ASPxTextBox;
            //ASPxTextBox txtCT4 = DataList1.Items[0].FindControl("txtCT4") as ASPxTextBox;
            //ASPxTextBox txtCT5 = DataList1.Items[0].FindControl("txtCT5") as ASPxTextBox;
            //ASPxTextBox txtCT6 = DataList1.Items[0].FindControl("txtCT6") as ASPxTextBox;

            if (chkCalcul.Checked)
            {
                txtCT2.Enabled = true;
                txtCT3.Enabled = true;
                txtCT4.Enabled = true;
                txtCT5.Enabled = true;
                txtCT6.Enabled = true;
            }
            else
            {
                txtCT2.Enabled = false;
                txtCT3.Enabled = false;
                txtCT4.Enabled = false;
                txtCT5.Enabled = false;
                txtCT6.Enabled = false;
                OnUpdateZLP();
            }

        }

        
        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["ConcediiMedicale"] as DataSet;
            switch (param[0])
            {
                case "cmbTipConcediu":
                    OnSelChangeTip();
                    break;
                case "deDeLaData":
                    OnSelStartDate();
                    break;
                case "deLaData":
                    OnSelEndDate();
                    break;
                case "rbConcInit":
                    On93Initial();
                    break;
                case "rbConcCont":
                    On93Prel();
                    break;
                case "cmbCC":
                    OnSelChangeCostCenter();
                    break;
                case "chkCalcul":
                    On93Zilemanual();
                    break;
                case "txtCNP":
                    OnKillfocus93CNPCopil();
                    break;
                case "txtCodIndemn":
                    OnKillfocus93CodIndemnizatie();
                    break;
                case "txtCC":
                    OnUpdateCcNo();
                    break;
                case "txtCT1":
                    OnUpdateZ1();
                    break;
                case "txtNrZile":
                    OnUpdateZL();
                    break;
                case "txtZCMAnt":
                    OnUpdateZLP();
                    break;
                case "rbZileCal":
                    OnZileAng();
                    break;
                case "rbZileFNUASS":
                    OnZileAng();
                    break;
                case "PreluareCM":
                    OnButtonVizualizareZileCMdinIstoric();
                    break;
                case "btnMZ":
                    OnButtonMedie();
                    break;
            }

        }

 

        double GetMARpercent(int Table_No, double Vechime)
        {
            string sql, sql_tmp;

            sql_tmp = "SELECT  TABLE_NO,LINE_NO,SEN_INF,SEN_SUP,PERCENT FROM MARTABLE";            
            sql_tmp += " WHERE TABLE_NO={0} AND SEN_INF<={1} AND SEN_SUP>{1} ";
            sql = string.Format(sql_tmp, Table_No, Vechime.ToString(new CultureInfo("en-US")));

            DataTable dt = General.IncarcaDT(sql, null);

            if (dt == null || dt.Rows.Count <= 0 || dt.Rows[0][0] == null)
                return -1;
            
            return Convert.ToDouble(dt.Rows[0]["PERCENT"].ToString());

        }

        int GetZileMed(int marca)
        {

            string sql, sql_tmp, szCoduri = "";          
            int  days = 0;


            if (Constante.tipBD == 2)
                sql_tmp = " SELECT DISTINCT TO_CHAR(F30037,'DD/MM/YYYY') AS DI, TO_CHAR(F30038,'DD/MM/YYYY') AS DSF FROM F300 WHERE (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
            else
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR, F30037, 103) AS DI, CONVERT(VARCHAR, F30038, 103) AS DSF FROM F300 WHERE (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";

            sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF)) AND F30003 = {0} AND F30010 <> 0 ";   //Radu 14.07.2016 - codul diferit de 0

            //Radu 14.11.2013 - coduri excluse din calculul celor 90 de zile
            DataTable dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'COD_MED_EXCL_90'", null);
            if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null && dtParam.Rows[0][0].ToString().Length > 0)
            { 
                szCoduri = dtParam.Rows[0][0].ToString().Trim();
                sql_tmp += "AND F30010 NOT IN ({1})";
                sql = string.Format(sql_tmp, marca, szCoduri);
            }
            else
                sql = string.Format(sql_tmp, marca);

            DataTable dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dtStart = new DateTime(Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(0, 2)));
                    DateTime dtSfarsit = new DateTime(Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(0, 2)));
                                        
                    days += (dtSfarsit - dtStart).Days + 1;
                }

            sql = " SELECT F01011, F01012 FROM F010";
            DataTable dt010 = General.IncarcaDT(sql, null);

            int an_c, luna_c, an_ant;

            an_c = Convert.ToInt32(dt010.Rows[0][0].ToString());
            luna_c = Convert.ToInt32(dt010.Rows[0][1].ToString());
     
            an_ant = an_c - 1;

            if (Constante.tipBD == 2)
                sql_tmp = " SELECT DISTINCT TO_CHAR(F94037,'DD/MM/YYYY') AS DI, TO_CHAR(F94038,'DD/MM/YYYY') AS DSF FROM F940 WHERE (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
            else
                sql_tmp = " SELECT DISTINCT  CONVERT(VARCHAR, F94037, 103) AS DI, CONVERT(VARCHAR, F94038, 103) AS DSF FROM F940 WHERE (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
            sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF)) AND F94003 = {0} AND F94010 <> 0 ";   //Radu 14.07.2016 - codul diferit de 0
            sql_tmp += " AND ((YEAR ={1} AND MONTH < {2}) OR (YEAR = {3} AND MONTH >= {2}))";

            //Radu 14.11.2013 - coduri excluse din calculul celor 90 de zile	
            if (szCoduri.Length > 0)
            {
                sql_tmp += "AND F94010 NOT IN ({4})";
                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, luna_c, szCoduri);
            }
            else
                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, luna_c);

            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dtStart = new DateTime(Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(0, 2)));
                    DateTime dtSfarsit = new DateTime(Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(0, 2)));

                    days += (dtSfarsit - dtStart).Days + 1;

                }

            return days;

        }

        DataTable GetConcediuInitial(int marca, bool avans)
        {
            string sql = "", sql_tmp;           

            if (Constante.tipBD == 2)
                sql_tmp = " SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, TO_CHAR(F30037,'DD/MM/YYYY') AS F30037, TO_CHAR(F30038,'DD/MM/YYYY') AS F30038, F300606, F300608 FROM F300 WHERE ";
            else
                sql_tmp = " SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, CONVERT(VARCHAR, F30037, 103) AS F30037, CONVERT(VARCHAR, F30038, 103) AS F30038, F300606, F300608 FROM F300 WHERE ";
            sql_tmp += " (F30010 IN (SELECT CODE1 FROM MARDEF) OR F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF)) ";
            //Radu 06.01.2015 - daca marca = 0 se aduc toate contractele
            if (marca != 0)
            {
                sql_tmp += " AND F30003 = {0} ";
                sql = string.Format(sql_tmp, marca);
            }

            if (avans)
            {
                if (Constante.tipBD == 2)
                    sql_tmp += "UNION ALL SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, TO_CHAR(F30037,'DD/MM/YYYY') AS F30037, TO_CHAR(F30038,'DD/MM/YYYY') AS F30038, F300606, F300608 FROM F300_CM_AVANS WHERE ";
                else
                    sql_tmp += "UNION ALL SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, CONVERT(VARCHAR, F30037, 103) AS F30037, CONVERT(VARCHAR, F30038, 103) AS F30038, F300606, F300608 FROM F300_CM_AVANS WHERE ";
                sql_tmp += " (F30010 IN (SELECT CODE1 FROM MARDEF) OR F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF)) ";
                //Radu 06.01.2015 - daca marca = 0 se aduc toate contractele
                if (marca != 0)
                {
                    sql_tmp += " AND F30003 = {0} ";
                    sql = string.Format(sql_tmp, marca, marca);
                }
            }


            if (marca == 0)
            {
                sql_tmp += " ORDER BY F30037";
                sql = sql_tmp;
            }
            else
            {
                sql += " ORDER BY F30037";
            }



            return General.IncarcaDT(sql, null);

        }

        double rotunjire(int tip, int cantit, double nr)
        {
            if (cantit == 0)
                cantit = 1;

            nr = Math.Round(nr, 5);

            switch (tip)
            {
                case 1: 
                    return (Math.Floor(Math.Floor(nr - 0.1) / cantit) + 1) * cantit;

                case 2: 
                    {
                        double add = (Math.Floor(Math.Floor(nr - 0.1) / cantit) + 1) * cantit;
                        double sub = (Math.Ceiling(Math.Ceiling(nr + 0.1) / cantit) - 1) * cantit;
                        if (Math.Abs(add - nr) <= Math.Abs(sub - nr))              
                            return add;
                        else
                            return sub;
                    }

                case 3: 
                    return (Math.Ceiling(Math.Ceiling(nr + 0.1) / cantit) - 1) * cantit;

            }
            return cantit;
        }

        private string SelectAngajati()
        {
            string strSql = "";

            try
            {
                string op = "+";
                if (Constante.tipBD == 2) op = "||";

                strSql = $@"SELECT A.F10003, A.F10008 {op} ' ' {op} A.F10009 AS ""NumeComplet"", 
                        X.F71804 AS ""Functia"", F.F00305 AS ""Subcompanie"",G.F00406 AS ""Filiala"",H.F00507 AS ""Sectie"",I.F00608 AS ""Departament"" 
                        FROM (
                        SELECT A.F10003
                        FROM F100 A
                        WHERE A.F10003 = {(Session["Marca"] == null ? "-99" : Session["Marca"].ToString())}
                        UNION
                        SELECT A.F10003
                        FROM F100 A
                        INNER JOIN ""F100Supervizori"" B ON A.F10003=B.F10003
                        WHERE B.""IdUser""= {Session["UserId"]}) B
                        INNER JOIN F100 A ON A.F10003=B.F10003
                        LEFT JOIN F718 X ON A.F10071=X.F71802
                        LEFT JOIN F003 F ON A.F10004 = F.F00304
                        LEFT JOIN F004 G ON A.F10005 = G.F00405
                        LEFT JOIN F005 H ON A.F10006 = H.F00506
                        LEFT JOIN F006 I ON A.F10007 = I.F00607";

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return strSql;
        }

        protected void cmbAng_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ASPxRadioButton rbZileCal = DataList1.Items[0].FindControl("rbConcInit") as ASPxRadioButton;
            //ASPxRadioButton rbZileFNUASS = DataList1.Items[0].FindControl("rbConcInit") as ASPxRadioButton;

            int marcaN = Convert.ToInt32(cmbAng.SelectedItem.Value);
            int marcaV = (Session["MarcaCM"] != null ? Convert.ToInt32(Session["MarcaCM"].ToString()) : -1);

            if (marcaV != -1 && marcaN != marcaV)                            
                GolesteCtrl();            

            Session["MarcaCM"] = Convert.ToInt32(cmbAng.SelectedItem.Value);

            string sql = "SELECT F10033 FROM F100 WHERE F10003 = " + Session["MarcaCM"].ToString();
            DataTable dtAng = General.IncarcaDT(sql, null);
            if (dtAng.Rows[0][0] != null && Convert.ToInt32(dtAng.Rows[0][0].ToString()) == 1)
                rbZileCal.Checked = true;
            else
            {
                rbZileFNUASS.Checked = true;
            }
            //OnZileAng();
            On93Initial();
           
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                OnOK();
            }
            catch (Exception ex)
            {
                //ArataMesaj("Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void GolesteCtrl()
        {
            cmbTipConcediu.SelectedIndex = 0;
            OnSelChangeTip();
            deDeLaData.Value = null;
            deLaData.Value = null;
            txtNrZile.Text = "";
            txtSerie.Text = "";
            txtNr.Text = "";
            deData.Value = null;
            rbConcInit.Checked = true;
            txtBCCM.Text = "";
            txtZBC.Text = "";
            txtMZ.Text = "";
            chkStagiu.Checked = false;
            txtZCMAnt.Text = "";
            txtSCMInit.Text = "";
            txtNrCMInit.Text = "";
            cmbLocPresc.SelectedIndex = -1;
            txtNrAviz.Text = "";
            deDataAviz.Value = null;
            txtMedic.Text = "";
            txtCNP.Text = "";
            txtCC.Text = "9999";
            OnUpdateCcNo();
            txtDetalii.Text = "";
            txtCodIndemn.Text = "";
            txtCodDiag.Text = "";
            txtCodUrgenta.Text = "";
            txtCodInfCont.Text = "";
            //rbZileCal.Checked = true;
            //rbZileCal.Text = "x zile calendaristice";
            chkCalcul.Checked = false;
            cmbCT1.SelectedIndex = -1;
            txtCT1.Text = "";
            cmbCT2.SelectedIndex = -1;
            txtCT2.Text = "";
            cmbCT3.SelectedIndex = -1;
            txtCT3.Text = "";
            cmbCT4.SelectedIndex = -1;
            txtCT4.Text = "";
            cmbCT5.SelectedIndex = -1;
            txtCT5.Text = "";
            cmbCT6.SelectedIndex = -1;
            txtCT6.Text = "";
            rbOptiune1.Visible = false;
            rbOptiune1.Visible = false;
            btnMZ.Enabled = true;
            Session["CM_StartDate"] = null;
            Session["CM_EndDate"] = null;
        }

    }




}