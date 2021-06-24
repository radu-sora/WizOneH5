﻿using DevExpress.Web;
using ProceseSec;
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
        double sumaReducereTimpLucru = 0;
        //decimal timpPartial = 0;
        //bool inactiveazaDeLaLa = false;
        protected void Page_Init(object sender, EventArgs e)
        {
            //DataList1.DataSource = General.IncarcaDT("SELECT * FROM F300 WHERE F10003 = " + (Session["MarcaCM"] != null ? Session["MarcaCM"].ToString() : "-99"), null);
            //DataList1.DataBind();

            txtTitlu.Text = Dami.TraduCuvant("Inregistrare concediu medical");
            if (Session["CM_Id"] != null)
                txtTitlu.Text = Dami.TraduCuvant("Detalii concediu medical");

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

            if (Session["CM_HR"] == null || Session["CM_HR"].ToString() != "1")
            {
                lblZCMAnt.ClientVisible = false;
                txtZCMAnt.ClientVisible = false;
                btnMZ.ClientVisible = false;
                lblBCCM.ClientVisible = false;
                txtBCCM.ClientVisible = false;
                lblZBC.ClientVisible = false;
                txtZBC.ClientVisible = false;
                lblMZBC.ClientVisible = false;
                txtMZBC.ClientVisible = false;
                lblMZ.ClientVisible = false;
                txtMZ.ClientVisible = false;
                divbtnMZ.Visible = false;
                divlblBCCM.Visible = false;
                btnCMAnt.ClientVisible = false;
            }

            if (Session["CM_Stare"] != null && Convert.ToInt32(Session["CM_Stare"].ToString()) >= 3)
            {
                btnSave.ClientVisible = false;
            }

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);
            deDeLaData.MinDate = new DateTime(Convert.ToInt32(dtLC.Rows[0][1].ToString()), Convert.ToInt32(dtLC.Rows[0][0].ToString()), 1);
            deLaData.MinDate = new DateTime(Convert.ToInt32(dtLC.Rows[0][1].ToString()), Convert.ToInt32(dtLC.Rows[0][0].ToString()), 1);
            if (Session["CM_StartDate"] != null)            
                deLaData.MaxDate = new DateTime(Convert.ToDateTime(Session["CM_StartDate"].ToString()).Year, Convert.ToDateTime(Session["CM_StartDate"].ToString()).Month, DateTime.DaysInMonth(Convert.ToDateTime(Session["CM_StartDate"].ToString()).Year, Convert.ToDateTime(Session["CM_StartDate"].ToString()).Month));
                  
            if (!IsPostBack)
            {
                dtAngajati = General.IncarcaDT(SelectAngajati(), null);
                cmbAng.DataSource = dtAngajati;
                Session["CM_Angajati"] = dtAngajati;
                cmbAng.DataBind();
                //if (Session["CM_Marca"] == null)
                //    cmbAng.SelectedIndex = -1;
                //else
                //    cmbAng.SelectedIndex = Convert.ToInt32(Session["CM_Marca"].ToString());

                cmbTipConcediu.SelectedIndex = 0;
                OnSelChangeTip();

                rbConcInit.Checked = true;
                rbConcCont.Checked = false;
                rbProgrNorm.Checked = true;

                OnUpdateCcNo();
                Session["CM_NrZile"] = null;
                Session["CM_Grid"] = null;
                Session["CM_Document"] = null;

                if (Session["CM_Id"] == null)
                    AfisareCalculManual(false);    

            }
            else
            {
                DataTable dtCopil = Session["CM_CNPCopil"] as DataTable;
                cmbCNPCopil.DataSource = dtCopil;
                cmbCNPCopil.DataBind();

                dtAngajati = Session["CM_Angajati"] as DataTable;
                cmbAng.DataSource = dtAngajati;
                cmbAng.DataBind();
                //if (Session["MarcaCM"] != null)
                //    cmbAng.SelectedIndex = Convert.ToInt32(Session["MarcaCM"].ToString());
                //else
                //    cmbAng.SelectedIndex = -1;

                //if (Session["CM_StartDate"] != null)
                //    deDeLaData.Value = Session["CM_StartDate"] as DateTime?;
                //if (Session["CM_EndDate"] != null)
                //    deLaData.Value = Session["CM_EndDate"] as DateTime?;


                DateTime dtTmp = Convert.ToDateTime(Session["CM_StartDate"]);
                if (dtTmp.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString()) && Session["CM_HR"] != null && Session["CM_HR"].ToString() == "1")
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

                if (rbProgrNorm.Checked)
                    AfisareCalculManual(false);
                else
                    AfisareCalculManual(true);
            }

            //if (cmbTipConcediu.Value != null && Convert.ToInt32(cmbTipConcediu.Value) == 9)
            //{
            //    lblCNP.ClientVisible = true;
            //    cmbCNPCopil.ClientVisible = true;
            //}
            //else
            //{
            //    lblCNP.ClientVisible = false;
            //    cmbCNPCopil.ClientVisible = false;
            //}

            if (Session["CM_NrZile"] != null)
                txtNrZile.Text = Session["CM_NrZile"].ToString();

            if (!IsPostBack && Session["CM_Id"] != null)
            {
                DataTable dtCM = General.IncarcaDT("SELECT * FROM CM_Cereri WHERE Id = " + Session["CM_Id"].ToString(), null);
                cmbAng.ClientEnabled = false;
                cmbAng.Value = Convert.ToInt32(dtCM.Rows[0]["F10003"].ToString());
                cmbTipConcediu.Value = Convert.ToInt32(dtCM.Rows[0]["TipConcediu"].ToString());
                if (Convert.ToInt32(dtCM.Rows[0]["TipProgram"] == DBNull.Value ? "0" : dtCM.Rows[0]["TipProgram"].ToString()) == 1)
                    rbProgrNorm.Checked = true;
                else
                    rbProgrTure.Checked = true;

                AfisareCalculManual(rbProgrNorm.Checked ? false : true);

                Session["MarcaCM"] = Convert.ToInt32(cmbAng.SelectedItem.Value);

                cmbCNPCopil.DataSource = General.IncarcaDT("SELECT F11012 as Id, F11012 as Denumire FROM F010, F110 WHERE F01002 = F11002 AND ((DATEPART(yyyy,F11006)+18) * 100 + DATEPART(mm,F11006)) >= (F01011 * 100 + F01012) AND F11003 = " + dtCM.Rows[0]["F10003"].ToString(), null);
                cmbCNPCopil.DataBind();

                txtCodIndemn.Text = (dtCM.Rows[0]["CodIndemnizatie"] == DBNull.Value ? "" : dtCM.Rows[0]["CodIndemnizatie"].ToString()).PadLeft(2, '0');
                cmbLocPresc.Value = Convert.ToInt32(dtCM.Rows[0]["Prescris"] == DBNull.Value ? "0" : dtCM.Rows[0]["Prescris"].ToString());
                txtSerie.Text = dtCM.Rows[0]["SerieCM"] == DBNull.Value ? "" : dtCM.Rows[0]["SerieCM"].ToString();
                txtNr.Text = dtCM.Rows[0]["NumarCM"] == DBNull.Value ? "" : dtCM.Rows[0]["NumarCM"].ToString();
                deData.Value = Convert.ToDateTime(dtCM.Rows[0]["DataCM"] == DBNull.Value ? "01/01/2100" : dtCM.Rows[0]["DataCM"].ToString());
                deDeLaData.Value = Convert.ToDateTime(dtCM.Rows[0]["DataInceput"] == DBNull.Value ? "01/01/2100" : dtCM.Rows[0]["DataInceput"].ToString());
                deLaData.Value = Convert.ToDateTime(dtCM.Rows[0]["DataSfarsit"] == DBNull.Value ? "01/01/2100" : dtCM.Rows[0]["DataSfarsit"].ToString());
                txtNrZile.Text = dtCM.Rows[0]["NrZile"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZile"].ToString();
                cmbCT1.Value = Convert.ToInt32(dtCM.Rows[0]["CodTransfer1"] == DBNull.Value ? "0" : dtCM.Rows[0]["CodTransfer1"].ToString());
                cmbCT2.Value = Convert.ToInt32(dtCM.Rows[0]["CodTransfer2"] == DBNull.Value ? "0" : dtCM.Rows[0]["CodTransfer2"].ToString());
                cmbCT3.Value = Convert.ToInt32(dtCM.Rows[0]["CodTransfer3"] == DBNull.Value ? "0" : dtCM.Rows[0]["CodTransfer3"].ToString());
                cmbCT4.Value = Convert.ToInt32(dtCM.Rows[0]["CodTransfer4"] == DBNull.Value ? "0" : dtCM.Rows[0]["CodTransfer4"].ToString());
                cmbCT5.Value = Convert.ToInt32(dtCM.Rows[0]["CodTransfer5"] == DBNull.Value ? "0" : dtCM.Rows[0]["CodTransfer5"].ToString());
                txtCT1.Text = dtCM.Rows[0]["NrZileCT1"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZileCT1"].ToString();
                txtCT2.Text = dtCM.Rows[0]["NrZileCT2"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZileCT2"].ToString();
                txtCT3.Text = dtCM.Rows[0]["NrZileCT3"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZileCT3"].ToString();
                txtCT4.Text = dtCM.Rows[0]["NrZileCT4"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZileCT4"].ToString();
                txtCT5.Text = dtCM.Rows[0]["NrZileCT5"] == DBNull.Value ? "" : dtCM.Rows[0]["NrZileCT5"].ToString();
                txtCodDiag.Text = dtCM.Rows[0]["CodDiagnostic"] == DBNull.Value ? "" : dtCM.Rows[0]["CodDiagnostic"].ToString();
                txtCodUrgenta.Text = dtCM.Rows[0]["CodUrgenta"] == DBNull.Value ? "" : dtCM.Rows[0]["CodUrgenta"].ToString();
                txtCodInfCont.Text = dtCM.Rows[0]["CodInfectoContag"] == DBNull.Value ? "" : dtCM.Rows[0]["CodInfectoContag"].ToString();
                if (Convert.ToInt32(dtCM.Rows[0]["Initial"] == DBNull.Value ? "0" : dtCM.Rows[0]["Initial"].ToString()) == 1)
                {
                    rbConcInit.Checked = true;
                    rbConcCont.Checked = false;
                }
                else
                {
                    rbConcInit.Checked = false;
                    rbConcCont.Checked = true;
                }
                txtZCMAnt.Text = dtCM.Rows[0]["ZileCMInitial"] == DBNull.Value ? "" : dtCM.Rows[0]["ZileCMInitial"].ToString();
                txtSCMInit.Text = dtCM.Rows[0]["SerieCMInitial"] == DBNull.Value ? "" : dtCM.Rows[0]["SerieCMInitial"].ToString();
                txtNrCMInit.Text = dtCM.Rows[0]["NumarCMInitial"] == DBNull.Value ? "" : dtCM.Rows[0]["NumarCMInitial"].ToString();
                deDataCMInit.Value = Convert.ToDateTime(dtCM.Rows[0]["DataCMInitial"] == DBNull.Value ? "01/01/2100" : dtCM.Rows[0]["DataCMInitial"].ToString());
                txtBCCM.Text = dtCM.Rows[0]["BazaCalculCM"] == DBNull.Value ? "" : dtCM.Rows[0]["BazaCalculCM"].ToString();
                txtZBC.Text = dtCM.Rows[0]["ZileBazaCalculCM"] == DBNull.Value ? "" : dtCM.Rows[0]["ZileBazaCalculCM"].ToString();
                txtMZBC.Text = dtCM.Rows[0]["MedieZileBazaCalcul"] == DBNull.Value ? "" : dtCM.Rows[0]["MedieZileBazaCalcul"].ToString();
                txtMZ.Text = dtCM.Rows[0]["MedieZilnicaCM"] == DBNull.Value ? "" : dtCM.Rows[0]["MedieZilnicaCM"].ToString();
                chkStagiu.Checked = Convert.ToInt32(dtCM.Rows[0]["Stagiu"] == DBNull.Value ? "0" : dtCM.Rows[0]["Stagiu"].ToString()) == 1 ? true : false;
                chkUrgenta.Checked = Convert.ToInt32(dtCM.Rows[0]["Urgenta"] == DBNull.Value ? "0" : dtCM.Rows[0]["Urgenta"].ToString()) == 1 ? true : false;
                txtNrAviz.Text = dtCM.Rows[0]["NrAvizMedicExpert"] == DBNull.Value ? "" : dtCM.Rows[0]["NrAvizMedicExpert"].ToString();
                deDataAviz.Value = Convert.ToDateTime(dtCM.Rows[0]["DataAvizDSP"] == DBNull.Value ? "01/01/2100" : dtCM.Rows[0]["DataAvizDSP"].ToString());
                txtMedic.Text = dtCM.Rows[0]["MedicCurant"] == DBNull.Value ? "" : dtCM.Rows[0]["MedicCurant"].ToString();
                cmbCNPCopil.Value = dtCM.Rows[0]["CNPCopil"] == DBNull.Value ? null : dtCM.Rows[0]["CNPCopil"].ToString();
            }

            txtNrAviz.MaxLength = 10;
            txtMedic.MaxLength = 40;
            txtCodIndemn.MaxLength = 10;
            txtCodDiag.MaxLength = 3;
            txtCodUrgenta.MaxLength = 3;
            txtCodInfCont.MaxLength = 3;
            txtSCMInit.MaxLength = 10;
            txtNrCMInit.MaxLength = 15;
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
                Response.Redirect("~/ConcediiMedicale/Aprobare", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        string OnOK()
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
                codExcl90 = dtParam.Rows[0][0].ToString().Trim();

            if (cmbAng.Value == null)
            {             
                return Dami.TraduCuvant("Nu ati ales niciun angajat!");
            }

            int marca = Convert.ToInt32(Session["MarcaCM"].ToString());
            bool avans = false;


            if (txtCodIndemn.Text.Trim().Length <= 0)
            {            
                return Dami.TraduCuvant("Nu ati completat codul de indemnizatie!");
            }

            if (txtCodDiag.Text.Trim().Length <= 0)
            {
                if (txtCodIndemn.Text.Trim() == "15")
                    txtCodDiag.Text = "RM";
                else
                {          
                    return Dami.TraduCuvant("Nu ati completat codul diagnostic!");
                }
            }

            if (deDeLaData.Value == null)
            {            
                return Dami.TraduCuvant("Data invalida!");
            }
            else
            {
                DateTime dt = Convert.ToDateTime(deDeLaData.Value);
                if (dt.Month != Convert.ToInt32(dtLC.Rows[0][0].ToString()))
                {

                    if (dt.Year > Convert.ToInt32(dtLC.Rows[0][1].ToString()) || (dt.Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && dt.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                        avans = true;
                    else
                    {
                        return Dami.TraduCuvant("Data de start este anterioara lunii curente!");
                    }
                }
            }

            if (deLaData.Value == null)
            {
                return Dami.TraduCuvant("Data invalida!");
            }

            if (Convert.ToDateTime(deDeLaData.Value).Year > Convert.ToDateTime(deLaData.Value).Year)
            {
                return Dami.TraduCuvant("Data de sfarsit este anterioara datei de inceput!");
            }

            if (Convert.ToDateTime(deDeLaData.Value).Year != Convert.ToDateTime(deLaData.Value).Year || Convert.ToDateTime(deDeLaData.Value).Month != Convert.ToDateTime(deLaData.Value).Month)
            {
                return Dami.TraduCuvant("Data de sfarsit trebuie sa fie in aceeasi luna cu data de inceput!");
            }


            int cc = Convert.ToInt32(txtCC.Text);

            DataTable dtAng = General.IncarcaDT("SELECT * FROM F100 WHERE F10003 = " + marca, null);

            vechime = ConvertVechime(dtAng.Rows[0]["F100644"].ToString());

            int tip = Convert.ToInt32(cmbTipConcediu.Value);

            bool bErr = false;
            string szErrMsg = "In urma validarilor efectuate au rezultat urmatoarele avertizari:" + System.Environment.NewLine;

            if (txtSerie.Text.Trim().Length <= 0)   // seria CM
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- nu ati completat Seria certificatului medical !";
            }

            if (txtNr.Text.Trim().Length <= 0 || Convert.ToInt32(txtNr.Text.Trim()) == 0) // numarul CM
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- nu ati completat Numarul certificatului medical !";
            }

            if (deData.Value == null)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine+ "- nu ati completat Data certificatului medical !";
            }

            if (txtCodIndemn.Text.Trim() == "04")
            {
                if (deDataAviz.Value == null)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- nu ati completat Data aviz. directia de sanatate publica !";
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
                szErrMsg += System.Environment.NewLine + "- data acordarii este anterioara celei de start !";
            }

            //verificare daca mai exista combinatia serie & nr CM in F300; de asemenea, daca aceasta combinatie e diferita de cea a concediului in continuare, daca exista
            DataTable dtTranz = GetConcediuInitial(0, avans);

            string codB = "", codP = "";
            codB = txtSCMInit.Text;
            codP = txtNrCMInit.Text;

            if (txtSerie.Text == codB && txtNr.Text == codP)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- Nr./Serie certificat medical curent este identic cu Nr./Serie certificat medical anterior!";
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
                        szErrMsg += System.Environment.NewLine + "- exista deja un CM cu aceeasi serie si acelasi numar !";
                        break;
                    }
                }
            }

            DataTable dtCM = General.IncarcaDT("SELECT * FROM CM_Cereri");
            for (int i = 0; i < dtCM.Rows.Count; i++)
            {
                if (Session["CM_Id"] == null &&
                    dtCM.Rows[i]["SerieCM"].ToString().Length > 0 && dtCM.Rows[i]["SerieCM"].ToString() == txtSerie.Text
                  && dtCM.Rows[i]["NumarCM"].ToString().Length > 0 && dtCM.Rows[i]["NumarCM"].ToString() == txtNr.Text)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- exista deja un CM cu aceeasi serie si acelasi numar !";
                    break;
                }
            }

            if (rbConcCont.Checked)
            {
                if (deDataCMInit.Value == null)
                {
                    return Dami.TraduCuvant("Data concediului initial este invalida!");
                }
            }
            else
                deDataCMInit.Value = deDeLaData.Value;


            string szCoduriIndemnizatie = ",01,02,03,04,05,06,07,08,09,10,11,12,13,14,15,51,91,";
            string cod = ",";
            if (txtCodIndemn.Text.Trim().Length <= 0)   // cod indemnizatie
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- nu ati completat codul de indemnizatie !";

            }
            else
            {
                cod += txtCodIndemn.Text.Trim() + ",";
                if (!szCoduriIndemnizatie.Contains(cod))
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- codul de indemnizatie completat incorect, valori acceptate " + System.Environment.NewLine + "  (01,02,03,04,05,06,07,08,09,10,11,12,13,14,15,51,91) !";
                }
            }
            cod = cod.Replace(",", "");
            if (cod == "06" && 2 != tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru codul de indemnizatie 06 CM selectat este incorect, valori admise CM urgenta !";
            }
            if (cod != "06" && 2 == tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru CM urgenta codul de indemnizatie este incorect, valoare acceptata 06 !";
            }

            if (cod == "05" && 10 != tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru codul de indemnizatie 05 CM selectat este incorect, valori admise CM infecto-contagioase !";
            }
            if (cod != "05" && 10 == tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru CM infecto-contagioase codul de indemnizatie este incorect, valoare acceptata 05 !";
            }

            if (cod == "09" && 8 != tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru codul de indemnizatie 09 CM selectat este incorect, valori admise CM Ingrijire copil<7a/<18a hand !";
            }
            if (cod != "09" && 8 == tip)
            {
                bErr = true;
                szErrMsg += System.Environment.NewLine + "- pentru CM Ingrijire copil<7a/<18a hand codul de indemnizatie este incorect, valoare acceptata 09 !";
            }

            if (2 == tip)   // CM urgente
            {
                if (txtCodUrgenta.Text.Trim().Length <= 0)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru CM urgenta nu ati completat codul de urgenta aferent !";
                }
                else if (Convert.ToInt32(txtCodUrgenta.Text.Trim()) > 177 || Convert.ToInt32(txtCodUrgenta.Text.Trim()) < 1)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru CM urgenta codul de urgenta este incorect, valori acceptate de la 1 la 177 !";
                }
            }

            if (10 == tip)  // CM infecto-contagioase
            {
                if (txtCodInfCont.Text.Trim().Length <= 0)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru CM infecto-contagioase nu ati completat codul de infecto-contagioase aferent !";
                }
                else if (Convert.ToInt32(txtCodInfCont.Text.Trim()) > 36 || Convert.ToInt32(txtCodInfCont.Text.Trim()) < 1)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru CM infecto-contagioase codul de infecto-contagioase incorect, valori acceptate de la 1 la 36 !";
                }
            }

            if (12 == tip)  // CM TBC
            {
                if (txtCodInfCont.Text.Trim().Length > 0)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru CM TBC codul de infecto-contagioase nu trebuie completat !";
                }
            }

            if (cod == "09")
            {
                if (cmbCNPCopil.Value == null || cmbCNPCopil.Value.ToString().Length < 13)
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- pentru cod indemnizatie 09 nu ati completat CNP copil !";
                }
            }

            int lLocPrescriere = Convert.ToInt32(cmbLocPresc.SelectedItem == null ? 0 : cmbLocPresc.SelectedItem.Value);
            if (1 == lLocPrescriere)    // medic de famile, incapacitate temporara de munca
            {
                int diff = (dtEnd - dtStart).Days;
                if (diff + 1 > 10 && cod != "08" && cod != "15")
                {
                    bErr = true;
                    szErrMsg += System.Environment.NewLine + "- concediul medical eliberat pentru incapacitate temporara de munca de catre medicul de familie nu poate depasi 10 zile !";
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

            //bool CM90 = false;
            if (!codExcl90.Contains(code.ToString()))
            {
                int diff = (dtEnd - dtStart).Days;
                int nrZileLC = diff + 1;

                int limitazile = 90;
                int zile = GetZileMed_cod_ind(marca, Convert.ToDateTime(deLaData.Value), txtCodIndemn.Text, (cmbCNPCopil.Value ?? "").ToString(), out limitazile);

                if (zile + nrZileLC > limitazile)
                {
                    //CM90 = true;
                    if (txtNrAviz.Text.Trim().Length <= 0)
                    {
                        bErr = true;
                        szErrMsg += System.Environment.NewLine + "- trebuie sa completati campul 'Nr. aviz medic expert'" + System.Environment.NewLine + "deoarece durata concediilor medicale pe ultimul an depaseste " + limitazile + " de zile !";
                    }
                }
            }


            if (bErr)
            {
                //szErrMsg += "\n\nDoriti sa reveniti la CM pentru completarea informatiilor omise ?";
                //if (IDYES == AfxMessageBox(szErrMsg, MB_APPLMODAL | MB_ICONQUESTION | MB_YESNO))
             
                return szErrMsg;
            }

            if (deData.Value == null)
            {
                return Dami.TraduCuvant("Data invalida!");
            }
            DateTime odtCarantina = new DateTime(2020, 7, 21, 0, 0, 0);
            int tab_procent = Convert.ToInt32(dtMARDEF.Rows[0]["TABLE_NO"].ToString());
            if (chkUrgenta.Checked)
                tab_procent = 3;
            if (Convert.ToInt32(txtCodIndemn.Text) == 7 && Convert.ToDateTime(deData.Value) < odtCarantina)
                tab_procent = 1;

            DataTable dt069 = General.IncarcaDT("SELECT * FROM F069 WHERE F06904 = " + dtLC.Rows[0][1].ToString() + " AND F06905 = " + dtLC.Rows[0][0].ToString(), null);

            if (!avans && Session["CM_HR"] != null && Session["CM_HR"].ToString() == "1")
            {
                int zile2 = 0;
                int zile = 0;
                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) >= 0 && (Convert.ToInt32(txtCodIndemn.Text) != 5 || txtCodDiag.Text != "064"))
                {
                    zile = Convert.ToInt32(txtCT1.Text);
                    zile2 = Convert.ToInt32(txtCT2.Text);
                    if (zile > 0 || (zile == 0 && zile2 == 0))
                    {
                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC1"].ToString()) / 100);
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                        suma4450 = rotunjire(2, 1, suma4450);
                        AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()), zile, cc, procent, 0, true, marca, false);
                    }
                }
                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) >= 0)
                {
                    if (Convert.ToInt32(txtCodIndemn.Text) == 5 && txtCodDiag.Text == "064")
                        zile = Convert.ToInt32(txtCT1.Text) + Convert.ToInt32(txtCT2.Text);
                    else
                        zile = Convert.ToInt32(txtCT2.Text);
                    if (zile > 0)
                    {
                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC2"].ToString()) / 100);
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                        suma4450 = rotunjire(2, 1, suma4450);
                        AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()), zile, cc, procent, 0, true, marca, false);
                    }
                }
                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) >= 0)
                {
                    zile = Convert.ToInt32(txtCT3.Text);
                    if (zile > 0)
                    {
                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC3"].ToString()) / 100);
                        // mihad 19.03.2018
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                        suma4450 = rotunjire(2, 1, suma4450);
                        AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()), zile, cc, procent, 0, true, marca, false);
                    }
                }
                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) >= 0)
                {
                    zile = Convert.ToInt32(Session["CM_NrZile"] == null ? txtNrZile.Text : Session["CM_NrZile"].ToString());
                    if (zile >= 0)  //Radu 27.05.2015	- am pus >= ca sa poata fi introduse concedii medicale cu Cod 4 de 0 ZL
                    {
                        int cod_ind = Convert.ToInt32(txtCodIndemn.Text);
                        double mntz = Convert.ToDouble(txtMZBC.Text);
                        if (cod_ind == 10 && mntz > 0)
                        {// reducere timp de lucru
                            sumaReducereTimpLucru = (Convert.ToDouble(txtMZ) * mntz);
                            sumaReducereTimpLucru = sumaReducereTimpLucru - (Convert.ToInt32(dtAng.Rows[0]["F100699"].ToString()) * 0.75);
                            if (sumaReducereTimpLucru > (Convert.ToDouble(txtMZ) * mntz * 0.25))
                                sumaReducereTimpLucru = Convert.ToDouble(txtMZ) * mntz * 0.25;
                            sumaReducereTimpLucru = rotunjire(2, 1, (sumaReducereTimpLucru / Convert.ToInt32(dt069.Rows[0]["F06907"].ToString())) * zile);
                        }
                        else
                            sumaReducereTimpLucru = 0;

                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC4"].ToString()) / 100);
                        // mihad 19.03.2018
                        suma4450 += (Convert.ToDouble(txtMZ.Text.Trim()) * zile * (procent / 100));
                        suma4450 = rotunjire(2, 1, suma4450);
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


                                if ((dtStart.Year == 2018 && dtStart.Month <= 6) || dtStart.Year < 2018)
                                    dtMARDEF.Rows[0]["CODE5"] = 4449;
                                else
                                {
                                    // caut daca nu e cod special		
                                    string szCoduri = "";
                                    dtParam = General.IncarcaDT("SELECT \"Valoare\" FROM \"tblParametrii\" WHERE \"Nume\" = 'CM_TARIF35'", null);
                                    if (dtParam != null && dtParam.Rows.Count > 0 && dtParam.Rows[0][0] != null)
                                        szCoduri = dtParam.Rows[0][0].ToString();

                                    if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0 && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && ((dtStart.Year == 2018 && dtStart.Month <= 9) || dtStart.Year < 2018))
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
                                            sql = string.Format(szT, serie_i, numar_i, marca);

                                            DataTable dtIstoric = General.IncarcaDT(sql, null);

                                            if (dtIstoric != null && dtIstoric.Rows.Count > 0)
                                            {
                                                for (int i = 0; i < dtIstoric.Rows.Count; i++)
                                                {
                                                    DateTime data_i = new DateTime(Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(6, 4)), Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(3, 2)), Convert.ToInt32(dtIstoric.Rows[i][0].ToString().Substring(0, 2)));

                                                    if ((data_i.Year == 2018 && data_i.Month <= 6) || dtStart.Year < 2018)
                                                        dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                    else
                                                    {
                                                        if ((Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0) && szCoduri.Contains(dtMARDEF.Rows[0]["CODE4"].ToString()) && ((dtStart.Year == 2018 && dtStart.Month <= 9) || dtStart.Year < 2018))
                                                        {
                                                            dtMARDEF.Rows[0]["CODE5"] = 4449;
                                                        }
                                                        else
                                                            suma_4450_subplafon = suma4450;
                                                    }
                                                }
                                            }
                                            else
                                                suma_4450_subplafon = suma4450;
                                        }
                                        else
                                            suma_4450_subplafon = suma4450;
                                    }
                                }
                            }
                        }

                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC5"].ToString()) / 100);
                        DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE5"].ToString(), null);

                        foreach (DataColumn col in dtF300.Columns)
                            col.ReadOnly = false;

                        if (dtF300 != null && dtF300.Rows.Count > 0)
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
                            if (Convert.ToInt32(dtF300.Rows[0]["F300620"].ToString()) > 0 && Convert.ToInt32(dtF300.Rows[0]["F300607"].ToString()) == 10 && sumaReducereTimpLucru > 0)
                                dtF300.Rows[0]["F30015"] = Convert.ToInt32(dtF300.Rows[0]["F30015"].ToString()) + sumaReducereTimpLucru;
                        }
                        else
                            AddConcediu(Convert.ToInt32(dtMARDEF.Rows[0]["CODE5"].ToString()), zile, cc, procent, 0, true, marca, false);

                        //CR Ambasada SUA
                        string data1Amb = "CONVERT(DATETIME,'" + Convert.ToDateTime(deDeLaData.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(deDeLaData.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(deDeLaData.Value).Year.ToString() + "',103)";
                        string data2Amb = "CONVERT(DATETIME,'" + Convert.ToDateTime(deLaData.Value).Day.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(deLaData.Value).Month.ToString().PadLeft(2, '0') + "/" + Convert.ToDateTime(deLaData.Value).Year.ToString() + "',103)";
                        string sqlAmb = "UPDATE F300 SET F30042 = '{0} ' + F30042 WHERE F30003 = {1} AND F30037 = {2} AND F30038 = {3} AND CONVERT(VARCHAR, F30010) IN ('{4}', '{5}', '{6}', '{7}')";
                        sqlAmb = string.Format(sqlAmb, dtMARDEF.Rows[0]["CODE5"].ToString(), marca, data1Amb, data2Amb, dtMARDEF.Rows[0]["CODE1"].ToString(), dtMARDEF.Rows[0]["CODE2"].ToString(),
                            dtMARDEF.Rows[0]["CODE3"].ToString(), dtMARDEF.Rows[0]["CODE4"].ToString());
                        General.ExecutaNonQuery(sqlAmb, null);
                    }
                }

                if (Convert.ToInt32(dtMARDEF.Rows[0]["CODE6"].ToString()) > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) >= 0)
                {
                    zile = Convert.ToInt32(txtCT6.Text);
                    if (zile > 0)
                    {
                        procent = GetMARpercent(tab_procent, vechime) * (Convert.ToInt32(dtMARDEF.Rows[0]["PERC6"].ToString()) / 100);
                        DataTable dtF300 = General.IncarcaDT("SELECT * FROM F300 WHERE F30003 = " + marca + " AND F30010 = " + dtMARDEF.Rows[0]["CODE6"].ToString(), null);

                        if (dtF300 != null && dtF300.Rows.Count > 0)
                        {
                            //string sz;
                            dtF300.Rows[0]["F300611"] = Convert.ToInt32(cmbLocPresc.Value);
                            dtF300.Rows[0]["F300612"] = Convert.ToInt32(txtBCCM.Text);
                            dtF300.Rows[0]["F300613"] = Convert.ToInt32(txtZBC.Text);
                            dtF300.Rows[0]["F300614"] = Convert.ToInt32(txtMZ.Text);
                            dtF300.Rows[0]["F300615"] = txtNrAviz.Text;
                            dtF300.Rows[0]["F300616"] = txtMedic.Text;
                            dtF300.Rows[0]["F300617"] = (cmbCNPCopil.Value ?? "").ToString();
                            dtF300.Rows[0]["F300620"] = Convert.ToDouble(txtMZBC.Text);


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
                int zile = Convert.ToInt32(Session["CM_NrZile"] == null ? txtNrZile.Text : Session["CM_NrZile"].ToString());
                AddConcediu(0, zile, cc, 0, 0, false, marca, avans);
            }
            Session["MARDEF"] = dtMARDEF;
            //MessageBox.Show("Proces realizat cu succes!");
            return "";
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

            string BCCM = "0", ZileBCCM = "0", MZCM = "0", MNTZ = "0";

            if (!avans || rbConcCont.Checked)
            {
                BCCM = txtBCCM.Text.Length <= 0 ? "0" : txtBCCM.Text;
                ZileBCCM = txtZBC.Text.Length <= 0 ? "0" : txtZBC.Text;
                MZCM = txtMZ.Text.Length <= 0 ? "0" : txtMZ.Text;
                MNTZ = txtMZBC.Text.Length <= 0 ? "0" : txtMZBC.Text;
            }

            if (cod == 4450)
                suma += suma_4450_subplafon;

            if (cc == 0 || cc == 9999)
            {
                if (dtAng.Rows[0]["F10053"] != null && dtAng.Rows[0]["F10053"].ToString().Length > 0 && Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString()) != 0 && Convert.ToInt32(dtAng.Rows[0]["F10053"].ToString()) != 9999)
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

            if (Convert.ToInt32(MNTZ) > 0 && Convert.ToInt32(txtCodIndemn.Text) == 10 && sumaReducereTimpLucru > 0)
                suma = sumaReducereTimpLucru;

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
                valAvans = string.Format(valAvans, Session["CM_NrZile"] == null ? txtNrZile.Text : Session["CM_NrZile"].ToString(), (txtCT1.Text.Length <= 0 ? "0" : txtCT1.Text), (txtCT2.Text.Length <= 0 ? "0" : txtCT2.Text), (txtCT3.Text.Length <= 0 ? "0" : txtCT3.Text),
                    (txtCT5.Text.Length <= 0 ? "0" : txtCT5.Text), (txtCT6.Text.Length <= 0 ? "0" : txtCT6.Text), cmbTipConcediu.SelectedItem.Value.ToString(), (rbOptiune1.Checked ? "1" : "2"));
            }


            if (dtAviz.Year <= 1900)
                dtAviz = new DateTime(2100, 1, 1);

            //string sql = "INSERT INTO " + (avans ? "F300_CM_AVANS" : "F300" ) + " (F30001, F30002, F30003, F30004, F30005, F30006, F30007, F30010, F30011, F30012, F30013, F30014, F30015, F30021, F30022, F30023, F30036, F30037, F30038, F30050, " +
            //    " F300601, F300602, F300603,  F30053, F300618, F30039, F30040, F30042, F30035, F300606, F300607, F300619, F300608, F300609, F300610, F300611, F300612, F300613, F300614, F300615, F300616, F300617, F300621, " + (avans ? cmpAvans : "") + ") ";


            string sql = "INSERT INTO CM_Cereri (Id, F10003, TipProgram, TipConcediu, CodIndemnizatie, SerieCM, NumarCM, DataCM, Prescris, DataInceput, DataSfarsit, NrZile, CodDiagnostic, CodUrgenta, CodInfectoContag, Initial, ZileCMInitial, SerieCMInitial, NumarCMInitial, DataCMInitial, " +
                     " CodTransfer1, CodTransfer2, CodTransfer3,  CodTransfer4, CodTransfer5, NrZileCT1, NrZileCT2, NrZileCT3, NrZileCT4, NrZileCT5, BazaCalculCM, ZileBazaCalculCM, MedieZileBazaCalcul, MedieZilnicaCM, NrAvizMedicExpert, DataAvizDSP, MedicCurant, CNPCopil, IdStare, Document, Urgenta, Suma, Tarif, Cod, USER_NO, TIME) ";


            sql += "VALUES ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, '{17}', '{18}', {19}, "
                + " {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32},  {33}, '{34}', {35}, '{36}', '{37}', {38}, {39}, {40}, {41}, {42}, {43}, {44}, {45} )";

            //sql = string.Format(sql, dtAng.Rows[0]["F10003"].ToString(), dtAng.Rows[0]["F10004"].ToString(), dtAng.Rows[0]["F10005"].ToString(), dtAng.Rows[0]["F10006"].ToString(), dtAng.Rows[0]["F10007"].ToString(), //4
            //    cod, 1, tarif.ToString(new CultureInfo("en-US")), zile, proc.ToString(new CultureInfo("en-US")), suma.ToString(new CultureInfo("en-US")), 0, 0, 0, //13
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') +  "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)" 
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //14
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //15
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 'dd/mm/yyyy')"), cc, txtSerie.Text, txtNr.Text, //19
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 'dd/mm/yyyy')"), (txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text), //21
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 'dd/mm/yyyy')"), 0, 0, detalii, //25
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"), txtSCMInit.Text, txtCodIndemn.Text, txtCodDiag.Text,  //29
            //    txtNrCMInit.Text, txtCodUrgenta.Text, txtCodInfCont.Text, (cmbLocPresc.SelectedItem == null ? "0" : cmbLocPresc.SelectedItem.Value.ToString()), BCCM, ZileBCCM, MZCM, txtNrAviz.Text, txtMedic.Text, (cmbCNPCopil.Value ?? "").ToString(),
            //    (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 103)"
            //    : "TO_DATE('" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 'dd/mm/yyyy')"), (avans ? valAvans : "")); //41
            int id = -99;
            if (Session["CM_Id"] != null)
            {
                General.ExecutaNonQuery("DELETE FROM CM_Cereri WHERE Id = " + Session["CM_Id"].ToString(), null);
                id = Convert.ToInt32(Session["CM_Id"].ToString());
            }
            else if (Session["CM_Id_Nou"] != null)
                id = Convert.ToInt32(Session["CM_Id_Nou"].ToString());
            else
                id = Dami.NextId("CM_Cereri");

            DataTable dtF = new DataTable();
            dtF = General.IncarcaDT("SELECT * FROM \"tblFisiere\"", null);
            if (Session["CM_Id"] != null && dtF.Select("Tabela = 'CM_Cereri' AND Id = " + Session["CM_Id"].ToString()).Count() == 1)
                Session["CM_Document"] = 1;

            sql = string.Format(sql, id, Session["MarcaCM"].ToString(), (rbProgrNorm.Checked ? "1" : "0"), Convert.ToInt32(cmbTipConcediu.Value ?? 0), txtCodIndemn.Text, txtSerie.Text, //5
                txtNr.Text, "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)", Convert.ToInt32(cmbLocPresc.Value ?? 0), //8
                "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)", "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)", //10
                zile, txtCodDiag.Text.Length <= 0 ? "0" : txtCodDiag.Text, txtCodUrgenta.Text.Length <= 0 ? "NULL" : txtCodUrgenta.Text, txtCodInfCont.Text.Length <= 0 ? "NULL" : txtCodInfCont.Text, (rbConcInit.Checked ? "1" : "0"), txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text, //16
                txtSCMInit.Text, txtNrCMInit.Text, "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + (dtDataCMInit.Year < 1900 ? 2100 : dtDataCMInit.Year).ToString() + "', 103)", //19
                Convert.ToInt32(cmbCT1.Value ?? 0), Convert.ToInt32(cmbCT2.Value ?? 0), Convert.ToInt32(cmbCT3.Value ?? 0), Convert.ToInt32(cmbCT4.Value ?? 0), Convert.ToInt32(cmbCT5.Value ?? 0), //24
                txtCT1.Text.Length <= 0 ? "0" : txtCT1.Text, txtCT2.Text.Length <= 0 ? "0" : txtCT2.Text, txtCT3.Text.Length <= 0 ? "0" : txtCT3.Text, txtCT4.Text.Length <= 0 ? "0" : txtCT4.Text, txtCT5.Text.Length <= 0 ? "0" : txtCT5.Text, //29
                txtBCCM.Text.Length <= 0 ? "0" : txtBCCM.Text.ToString(new CultureInfo("en-US")), txtZBC.Text.Length <= 0 ? "0" : txtZBC.Text, txtMZBC.Text.Length <= 0 ? "0" : txtMZBC.Text.ToString(new CultureInfo("en-US")), txtMZ.Text.Length <= 0 ? "0" : txtMZ.Text.Replace(',', '.').ToString(new CultureInfo("en-US")), //33
                txtNrAviz.Text, "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + (dtAviz.Year < 1900 ? 2100 : dtAviz.Year).ToString() + "', 103)", txtMedic.Text, //36
                (cmbCNPCopil.Value ?? ""), 1, (Session["CM_Document"] == null ? 0 : 1), (chkUrgenta.Checked ? "1" : "0"), suma.ToString(new CultureInfo("en-US")), tarif.ToString(new CultureInfo("en-US")), cod, Convert.ToInt32(Session["UserId"].ToString()), "GETDATE()"); //43

            //cod, 1, tarif.ToString(new CultureInfo("en-US")), zile, proc.ToString(new CultureInfo("en-US")), suma.ToString(new CultureInfo("en-US")), 0, 0, 0, //13
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //14
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"),  //15
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtEnd.Day.ToString().PadLeft(2, '0') + "/" + dtEnd.Month.ToString().PadLeft(2, '0') + "/" + dtEnd.Year.ToString() + "', 'dd/mm/yyyy')"), cc, txtSerie.Text, txtNr.Text, //19
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtData.Day.ToString().PadLeft(2, '0') + "/" + dtData.Month.ToString().PadLeft(2, '0') + "/" + dtData.Year.ToString() + "', 'dd/mm/yyyy')"), (txtZCMAnt.Text.Length <= 0 ? "0" : txtZCMAnt.Text), //21
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtAviz.Day.ToString().PadLeft(2, '0') + "/" + dtAviz.Month.ToString().PadLeft(2, '0') + "/" + dtAviz.Year.ToString() + "', 'dd/mm/yyyy')"), 0, 0, detalii, //25
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtStart.Day.ToString().PadLeft(2, '0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + dtStart.Year.ToString() + "', 'dd/mm/yyyy')"), txtSCMInit.Text, txtCodIndemn.Text, txtCodDiag.Text,  //29
            //txtNrCMInit.Text, txtCodUrgenta.Text, txtCodInfCont.Text, (cmbLocPresc.SelectedItem == null ? "0" : cmbLocPresc.SelectedItem.Value.ToString()), BCCM, ZileBCCM, MZCM, txtNrAviz.Text, txtMedic.Text, (cmbCNPCopil.Value ?? "").ToString(),
            //(Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 103)"
            //: "TO_DATE('" + dtDataCMInit.Day.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Month.ToString().PadLeft(2, '0') + "/" + dtDataCMInit.Year.ToString() + "', 'dd/mm/yyyy')"), (avans ? valAvans : "")); //41




            if (!((chkStagiu.Checked && (cod == 4450 || cod == 4449))))
                if (General.ExecutaNonQuery(sql, null))
                {
                    if (chkStagiu.Checked)
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
                    if (Session["CM_Id"] == null)
                    {
                        sql = "INSERT INTO CM_CereriIstoric(IdCerere, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare) VALUES(" + id + ", 1, " + Session["UserId"].ToString() + ", 1, (SELECT Culoare FROM CM_tblStari WHERE Id = 1), 1, GETDATE())";
                        General.ExecutaNonQuery(sql, null);
                        if (Session["CM_HR"] != null && Session["CM_HR"].ToString() == "1")
                            sql = "INSERT INTO CM_CereriIstoric(IdCerere, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare) VALUES(" + id + ", 3, " + Session["UserId"].ToString() + ", 2, (SELECT Culoare FROM CM_tblStari WHERE Id = 3), 1, GETDATE())";
                        else
                            sql = "INSERT INTO CM_CereriIstoric(IdCerere, Pozitie) VALUES(" + id + ", 2)";
                        General.ExecutaNonQuery(sql, null);
                    }                   
                    else
                    {
                        if (Session["CM_HR"] != null && Session["CM_HR"].ToString() == "1")
                        {
                            General.ExecutaNonQuery("DELETE FROM CM_CereriIstoric WHERE IdCerere = " + id + " AND Pozitie = 2", null);
                            sql = "INSERT INTO CM_CereriIstoric(IdCerere, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare) VALUES(" + id + ", 3, " + Session["UserId"].ToString() + ", 2, (SELECT Culoare FROM CM_tblStari WHERE Id = 3), 1, GETDATE())";
                            General.ExecutaNonQuery(sql, null);
                        }
                        else
                        {
                            General.ExecutaNonQuery("DELETE FROM CM_CereriIstoric WHERE IdCerere = " + id + " AND Pozitie = 1", null);
                            sql = "INSERT INTO CM_CereriIstoric(IdCerere, IdStare, IdUser, Pozitie, Culoare, Aprobat, DataAprobare) VALUES(" + id + ", 1, " + Session["UserId"].ToString() + ", 1, (SELECT Culoare FROM CM_tblStari WHERE Id = 1), 1, GETDATE())";
                            General.ExecutaNonQuery(sql, null);
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
            bool trans = int.TryParse((Session["CM_NrZile"] == null ? "0" : Session["CM_NrZile"].ToString()), out ZL);

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

            DataTable dtComp = General.IncarcaDT("SELECT F00292 FROM F002", null);

            if (!trans || (((Z1 + ZLA) > lim) && (Z1 > 0)) && !chkCalcul.Checked && (dtComp.Rows[0][0] == null || dtComp.Rows[0][0].ToString() != "1"))
            {
                string msg = "Introduceti un numar de zile ( Zile < 3 + Zile luna anterioara ) <= {0} !";
                msg = string.Format(msg, lim);
                pnlCtl.JSProperties["cpAlertMessage"] = msg;
                txtCT1.Text = "";
                txtCT1.Focus();
                return;
            }
            if (Z1 > ZL && ZL != 0 && rbProgrTure.Checked)
            {
                pnlCtl.JSProperties["cpAlertMessage"] = "Introduceti un numar de zile mai mic sau egal cu numarul total de zile !";
                txtCT1.Text = "";
                txtCT1.Focus();
                return;
            }

            int nMin;
            int nrZL = 0;
            DateTime dtb = DateTime.Now, dte = DateTime.Now;

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
                    total += Session["CM_NrZile"] != null ? Convert.ToInt32(Session["CM_NrZile"].ToString()) : 0;
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

            if (Session["CM_NrZile"] == null) return;
            int ZL = 0;
            int.TryParse(Session["CM_NrZile"].ToString(), out ZL);

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
                    total += Session["CM_NrZile"] != null ? Convert.ToInt32(Session["CM_NrZile"].ToString()) : 0;
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

            if (dtMARDEF.Rows[0]["CODIND"] != null && dtMARDEF.Rows[0]["CODIND"].ToString().Length > 0 && Convert.ToInt32(dtMARDEF.Rows[0]["CODIND"].ToString()) > 0)
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

            if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP") && ((dtMARDEF.Rows[0]["CODE1"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0) ||
                (dtMARDEF.Rows[0]["CODE2"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0) || (dtMARDEF.Rows[0]["CODE3"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0)))
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

            //if (no == 9)
            //{
            //    lblCNP.ClientVisible = true;
            //    cmbCNPCopil.ClientVisible = true;
            //}
            //else
            //{
            //    lblCNP.ClientVisible = false;
            //    cmbCNPCopil.ClientVisible = false;
            //}
        }
        void OnSelAngajat(string marca)
        {
            DataTable dt = General.IncarcaDT("SELECT F11012 as Id, F11012 as Denumire FROM F010, F110 WHERE F01002 = F11002 AND ((DATEPART(yyyy,F11006)+18) * 100 + DATEPART(mm,F11006)) >= (F01011 * 100 + F01012) AND F11003 = " + marca, null);
            cmbCNPCopil.DataSource = dt;            
            cmbCNPCopil.DataBind();
            Session["CM_CNPCopil"] = dt;
        }


        void OnSelStartDate()
        {
            Session["CM_StartDate"] = Convert.ToDateTime(deDeLaData.Value);
            InitWorkingDays();
            deData.Value = deDeLaData.Value;

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);
            deLaData.MaxDate = new DateTime(Convert.ToDateTime(deDeLaData.Value).Year, Convert.ToDateTime(deDeLaData.Value).Month, DateTime.DaysInMonth(Convert.ToDateTime(deDeLaData.Value).Year, Convert.ToDateTime(deDeLaData.Value).Month));
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
                btnMZ.ClientVisible = false;
                lblBCCM.ClientVisible = false;
                lblZBC.ClientVisible = false;
                lblMZBC.ClientVisible = false;
                lblMZ.ClientVisible = false;
                txtBCCM.ClientVisible = false;
                txtZBC.ClientVisible = false;
                txtMZBC.ClientVisible = false;
                txtMZ.ClientVisible = false;
            }
            else
            {
                rbOptiune1.Visible = false;
                rbOptiune2.Visible = false;
                if (Session["CM_HR"] != null && Session["CM_HR"].ToString() == "1")
                {
                    btnMZ.Enabled = true;
                    btnMZ.ClientVisible = true;
                    lblBCCM.ClientVisible = true;
                    lblZBC.ClientVisible = true;
                    lblMZBC.ClientVisible = true;
                    lblMZ.ClientVisible = true;
                    txtBCCM.ClientVisible = true;
                    txtZBC.ClientVisible = true;
                    txtMZBC.ClientVisible = true;
                    txtMZ.ClientVisible = true;
                }
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

            //int dow;
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

            if ((dtMARDEF.Rows[0]["CODE2"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0) || (dtMARDEF.Rows[0]["CODE3"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0)
                || (dtMARDEF.Rows[0]["CODE4"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE4"].ToString()) > 0))
            {
                if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP") && ((dtMARDEF.Rows[0]["CODE1"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE1"].ToString()) > 0) ||
                    (dtMARDEF.Rows[0]["CODE2"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE2"].ToString()) > 0) || (dtMARDEF.Rows[0]["CODE3"] != null && Convert.ToInt32(dtMARDEF.Rows[0]["CODE3"].ToString()) > 0)))
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
            }
            else
            {
                nrZL3 = nrZL;
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
                    total += Session["CM_NrZile"] != null ? Convert.ToInt32(Session["CM_NrZile"].ToString()) : 0;
                }
            }
            txtCT5.Text = total.ToString();
            txtCT6.Text = total.ToString();
            Session["CM_NrZile"] = txtNrZile.Text;
        }

        bool IsHoliday(DateTime dt)
        {
            string sql = "SELECT COUNT(*) FROM HOLIDAYS WHERE DAY = " + (Constante.tipBD == 1 ? "CONVERT(DATETIME, '" + dt.Day.ToString().PadLeft(2, '0') + "/" + dt.Month.ToString().PadLeft(2, '0') + "/" + dt.Year + "', 103)"
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
            int No;
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

            if (rbZileCal.Checked)
            {
                if (!dtMARDEF.Rows[0]["NAME"].ToString().Contains("AMBP"))
                    Session["ZileAng"] = "5";
                else
                    Session["ZileAng"] = "3";
            }
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
            deDataCMInit.Enabled = true;

            int nrLines, i;
            int year, month, day;

            DateTime nDate1 = Convert.ToDateTime(deDeLaData.Value);
            year = nDate1.Year;
            nDate1 = nDate1.AddYears(-1);
            month = nDate1.Month;
            day = nDate1.Day;

            if (day < 1)
                return;

            // Radu 01.03.2016
            int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (year % 4 == 0)
                monthDays[1] = 29;


            day--;
            if (day == 0)
            {
                month--;
                if (month == 0)
                {
                    month = 12;
                    year--;
                }
                day = monthDays[month - 1];
            }

            DateTime dt1 = new DateTime(year, month, day, 0, 0, 0);

            string sql = "SELECT F01012, F01011 FROM F010";
            DataTable dtLC = General.IncarcaDT(sql, null);
            bool avans = false;
            DateTime dt = Convert.ToDateTime(deDeLaData.Value);
            if (dt.Month != Convert.ToInt32(dtLC.Rows[0][0].ToString()))
            {

                if (dt.Year > Convert.ToInt32(dtLC.Rows[0][1].ToString()) || (dt.Year == Convert.ToInt32(dtLC.Rows[0][1].ToString()) && dt.Month > Convert.ToInt32(dtLC.Rows[0][0].ToString())))
                    avans = true;

            }

            DataTable dtConc = GetConcediuInitial(Convert.ToInt32(Session["MarcaCM"].ToString()), avans);

            int nDays = 0, nCDays = 0;             
            string codBoala, codParafa, codBoala_I, codParafa_I, BCCM, ZBCCM, MZCM, MNTZ, Data_CMI;
            int nData_CMI;

            if (dtConc != null && dtConc.Rows.Count > 0)
            {
                for (i = 0; i < dtConc.Rows.Count; i++)
                {
                    DateTime nDate2 = Convert.ToDateTime(dtConc.Rows[0]["F30038"].ToString());
                    year = nDate2.Year; 
                    nDate2 = nDate2.AddYears(-1);
                    month = nDate2.Month; 
                    day = nDate2.Day;
                    DateTime dt2 = new DateTime(year, month, day, 0, 0, 0);
                    
                    int dow;
                    DateTime dtb = new DateTime(2100, 1, 1), dte = new DateTime(2100, 1, 1), dtcmi = new DateTime(2100, 1, 1);
                    if (dtConc.Rows[0]["F30037"] != null)  
                    {
                        DateTime nDate = Convert.ToDateTime(dtConc.Rows[0]["F30037"].ToString());
                        year = nDate.Year;
                        nDate = nDate.AddYears(-1);
                        month = nDate.Month;
                        nDate = nDate.AddMonths(-1);
                        day = nDate.Day;
                        dtb = new DateTime(year, month, day, 0, 0, 0);
                    }

                    if (dtConc.Rows[0]["F30038"] != null)
                    {
                        DateTime nDate = Convert.ToDateTime(dtConc.Rows[0]["F30038"].ToString());
                        year = nDate.Year;
                        nDate = nDate.AddYears(-1);
                        month = nDate.Month;
                        nDate = nDate.AddMonths(-1);
                        day = nDate.Day;
                        dte = new DateTime(year, month, day, 0, 0, 0);
                    }
                    
                    if (dtConc.Rows[0]["F300621"] != null)
                    {
                        DateTime nDate = Convert.ToDateTime(dtConc.Rows[0]["F300621"].ToString());
                        year = nDate.Year;
                        nDate = nDate.AddYears(-1);
                        month = nDate.Month;
                        nDate = nDate.AddMonths(-1);
                        day = nDate.Day;
                        dtcmi = new DateTime(year, month, day, 0, 0, 0);
                    }
                    if (dtcmi.Year == 1900 || dtcmi.Year == 2100)
                        dtcmi =new DateTime(dtb.Year, dtb.Month, dtb.Day, 0, 0, 0);



                    //Radu 09.10.2014 - e concediu initial
                    if ((dtConc.Rows[0]["F300606"] == null || dtConc.Rows[0]["F300606"].ToString().Length <= 0) && (dtConc.Rows[0]["F300608"] == null || dtConc.Rows[0]["F300608"].ToString().Length <= 0))
                    {
                        codBoala = dtConc.Rows[0]["F300601"].ToString();
                        codParafa = dtConc.Rows[0]["F300602"].ToString();
                        codBoala_I = dtConc.Rows[0]["F300601"].ToString();
                        codParafa_I = dtConc.Rows[0]["F300602"].ToString();
                        BCCM = Convert.ToDouble(dtConc.Rows[0]["F300612"].ToString()).ToString("0.##");                       
                        ZBCCM = Convert.ToInt32(Convert.ToDouble(dtConc.Rows[0]["F300613"].ToString())).ToString();
                        MZCM = Convert.ToDouble(dtConc.Rows[0]["F300614"].ToString()).ToString("0.##");
                        MNTZ = Convert.ToDouble(dtConc.Rows[0]["F300620"].ToString()).ToString("0.##");
                        // mihad 07.12.2018
                        //Data_CMI.Format("%02d/%02d/%04d", dtcmi.GetDay(), dtcmi.GetMonth(), dtcmi.GetYear());
                        nCDays = 0;
                    }
                    // mihad 06.11.2014
                    else
                        if (i == 0)
                            nCDays += Convert.ToInt32(Session["ZileCMAnterior"].ToString());



                    //Radu 02.10.2014 - daca exista informatii pe concediu initial, se iau acestea; daca nu se ia seria si nr concediului curent
                    if (dtConc.Rows[0]["F300606"] != null && dtConc.Rows[0]["F300606"].ToString().Length > 0 && dtConc.Rows[0]["F300608"] != null && dtConc.Rows[0]["F300608"].ToString().Length > 0)               
                    {
                        codBoala = dtConc.Rows[0]["F300606"].ToString();
                        codParafa = dtConc.Rows[0]["F300608"].ToString();
                        if (Session["ZileCMAnterior"] != null && Session["ZileCMAnterior"].ToString() == "0")    // le mai completez doar daca nu am ales eu din fereastra altceva
                        {
                            codBoala_I = dtConc.Rows[0]["F300606"].ToString();
                            codParafa_I = dtConc.Rows[0]["F300608"].ToString();
                        }
                    }
                    else
                    {
                        codBoala = dtConc.Rows[0]["F300601"].ToString(); 
                        codParafa = dtConc.Rows[0]["F300602"].ToString();
                        codBoala_I = dtConc.Rows[0]["F300606"].ToString();
                        codParafa_I = dtConc.Rows[0]["F300608"].ToString();
                    }
                    BCCM = Convert.ToDouble(dtConc.Rows[0]["F300612"].ToString()).ToString("0.##");
                    ZBCCM = Convert.ToInt32(Convert.ToDouble(dtConc.Rows[0]["F300613"].ToString())).ToString();
                    MZCM = Convert.ToDouble(dtConc.Rows[0]["F300614"].ToString()).ToString("0.##");
                    MNTZ = Convert.ToDouble(dtConc.Rows[0]["F300620"].ToString()).ToString("0.##");
                    // mihad 07.12.2018
                    //Data_CMI.Format("%02d/%02d/%04d", dtcmi.GetDay(), dtcmi.GetMonth(), dtcmi.GetYear());

                    //}

                    for (DateTime date = dtb; date <= dte; dt = date.AddDays(1))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday && !IsHoliday(date))
                            nDays++;
                        nCDays++;
                    }
       

                    if (dt1 == dt2)
                    {
                        txtSerie.Text = codBoala;
                        txtNr.Text = codParafa;
                        if (Session["ZileCMAnterior"] != null && Session["ZileCMAnterior"].ToString() == "0")   // le mai completez doar daca nu am ales eu din fereastra altceva
                        {
                            txtSCMInit.Text = codBoala;
                            txtNrCMInit.Text = codParafa;
                        }
                        txtBCCM.Text = BCCM;
                        txtZBC.Text = ZBCCM;
                        txtMZ.Text = MZCM;
                        txtMZBC.Text = MNTZ;
                        deDataCMInit.Value = dtcmi;
                        txtZCMAnt.Text = nCDays.ToString();

                    }       
                }
                Session["ZileCMAnterior"] = 0; 
            }

        }
    

        void On93Initial()
        {
            //ASPxTextBox txtZCMAnt = DataList1.Items[0].FindControl("txtZCMAnt") as ASPxTextBox;
            //ASPxTextBox txtSCMInit = DataList1.Items[0].FindControl("txtSCMInit") as ASPxTextBox;
            //ASPxTextBox txtNrCMInit = DataList1.Items[0].FindControl("txtNrCMInit") as ASPxTextBox;

            txtZCMAnt.Enabled = false;
            txtSCMInit.Enabled = false;
            txtNrCMInit.Enabled = false;
            deDataCMInit.Enabled = false;
        }


        void OnButtonVizualizareZileCMdinIstoric()
        {
            if (Session["CM_Preluare"] != null && Convert.ToInt32(Session["CM_Preluare"].ToString()) == 2 && Session["CM_Id"] != null && Session["CM_Id"].ToString().Length > 0)
            {
                string[] param = Session["SerieNrCMInitial"].ToString().Split(' ');
                txtSCMInit.Text = param[0];
                txtNrCMInit.Text = param[1];
            }

            if (Session["CM_Preluare"] != null && Convert.ToInt32(Session["CM_Preluare"].ToString()) == 1)
            {
                txtZCMAnt.Text = Session["ZileCMAnterior"].ToString();
                string[] param = Session["SerieNrCMInitial"].ToString().Split(' ');
                txtSerie.Text = param[0];
                txtNr.Text = param[1];
                txtSCMInit.Text = param[0];
                txtNrCMInit.Text = param[1];
                txtBCCM.Text = Session["BazaCalculCM"].ToString();
                txtZBC.Text = Session["ZileBazCalcul"].ToString();
                txtMZ.Text = Session["MediaZilnica"].ToString();

                txtMZBC.Text = Session["MedieZilnicaBazaCalculCM"].ToString();
                DateTime? dt = Session["DataCMICalculCM"] as DateTime?;
                deDataCMInit.Value = dt;

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

            double BCCM = 0, Medie6 = 0, MNTZ = 0;
            int ZBCCM = 0;
            string strCNP_marci = "";

            DataTable dt = General.IncarcaDT("SELECT F10003 FROM F100 WHERE F10017 = (SELECT F10017 FROM F100 WHERE F10003 = " + Session["MarcaCM"].ToString() + ")", null);
            for (int i = 0; i < dt.Rows.Count; i++)
                strCNP_marci += "," + dt.Rows[i][0].ToString();
            Module.ConcediiMedicale.CreateDetails(Convert.ToInt32(Session["MarcaCM"].ToString()), strCNP_marci.Substring(1), out Medie6, out BCCM, out ZBCCM, out MNTZ);

            txtBCCM.Text = BCCM.ToString(new CultureInfo("en-US"));
            txtZBC.Text = ZBCCM.ToString();
            txtMZ.Text = Medie6.ToString("0.##").ToString(new CultureInfo("en-US"));
            txtMZBC.Text = MNTZ.ToString("0.##").ToString(new CultureInfo("en-US"));
        }

     

        void OnKillfocus93CNPCopil()
        {
            //ASPxTextBox txtCNP = DataList1.Items[0].FindControl("txtCNP") as ASPxTextBox;            
            //if (!General.VerificaCNP((cmbCNPCopil.Value ?? "").ToString()))
            //{               
            //    pnlCtl.JSProperties["cpAlertMessage"] = "Atentie! CNP invalid!";
            //    cmbCNPCopil.Focus();
            //}
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
                case "cmbAng":
                    OnSelAngajat(param[1]);
                    break;
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
                //case "cmbCNPCopil":
                //    OnKillfocus93CNPCopil();
                //    break;
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
                case "rbProgrNorm":
                    if (param[1] == "true")
                        AfisareCalculManual(false);
                    else
                        AfisareCalculManual(true);
                    break;
                case "rbProgrTure":
                    if (param[1] == "true")
                        AfisareCalculManual(true);
                    else
                        AfisareCalculManual(false);
                    break;

            }

            if (rbProgrNorm.Checked)
                AfisareCalculManual(false);
            else
                AfisareCalculManual(true);

            if (rbConcInit.Checked)
                On93Initial();
            else
            {
                txtZCMAnt.Enabled = true;
                txtSCMInit.Enabled = true;
                txtNrCMInit.Enabled = true;
                deDataCMInit.Enabled = true;
            }

            if (Session["CM_NrZile"] != null)
                txtNrZile.Text = Session["CM_NrZile"].ToString();
        }

 
        private void AfisareCalculManual(bool afisare)
        {
            lblCT1.ClientVisible = afisare;
            cmbCT1.ClientVisible = afisare;
            txtCT1.ClientVisible = afisare;
            lblCT2.ClientVisible = afisare;
            cmbCT2.ClientVisible = afisare;
            txtCT2.ClientVisible = afisare;
            lblCT3.ClientVisible = afisare;
            cmbCT3.ClientVisible = afisare;
            txtCT3.ClientVisible = afisare;
            lblCT4.ClientVisible = afisare;
            cmbCT4.ClientVisible = afisare;
            txtCT4.ClientVisible = afisare;
            lblCT5.ClientVisible = afisare;
            cmbCT5.ClientVisible = afisare;
            txtCT5.ClientVisible = afisare;

            divCT1.Visible = afisare;
            divCT2.Visible = afisare;
            divCT3.Visible = afisare;
            divCT4.Visible = afisare;
            divCT5.Visible = afisare;
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
        int GetZileMed_cod_ind(int marca, DateTime di_med, string cod_ind, string CNPCopil, out int limitazile)
        {
            string sql, sql_tmp, line = "";
            int days = 0;

            DateTime ddi_med = di_med.AddYears(-1);

            if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE (F300607 = '01' OR  ";
                sql_tmp += " F300607 = '05' OR F300607 = '06' OR F300607 = '10') AND F30003 = {0} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca);
            }
            else if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE (F300607 = '12' OR  ";
                sql_tmp += " F300607 = '13' OR F300607 = '14') AND F30003 = {0} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca);
            }
            else
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F30037,103) AS DI, CONVERT(VARCHAR,F30038,103) AS DSF FROM F300 WHERE F300607 = '{0}' ";
                sql_tmp += " AND F30003 = {1} AND F30010 <> 0 ";
                sql_tmp += " AND (F30010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F30010 IN (SELECT CODE2 FROM MARDEF) OR F30010 IN (SELECT CODE3 FROM MARDEF) OR F30010 IN (SELECT CODE4 FROM MARDEF))";

                sql= string.Format(sql_tmp, cod_ind, marca);

                if (Convert.ToInt32(cod_ind) == 9)
                {
                    sql += " AND F300617 = '" + CNPCopil + "'";
                }
            }

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
            string data = "CONVERT(DATETIME,'" + di_med.Day.ToString().PadLeft(2, '0') + "/" + di_med.Month.ToString().PadLeft(2, '0') + "/" + di_med.Year.ToString() + "',103)";

            if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE (F940607 = '01' OR  ";
                sql_tmp += " F940607 = '05' OR F940607 = '06' OR F940607 = '10') AND F94003 = {0} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={1} AND MONTH < {2}) OR (YEAR = {3} AND MONTH >= {2})) AND F94038 >= DATEADD(MONTH, -12, {4}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";                

                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, data);
            }
            else  if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE (F940607 = '12' OR  ";
                sql_tmp += " F940607 = '13' OR F940607 = '14') AND F94003 = {0} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={1} AND MONTH < {2}) OR (YEAR = {3} AND MONTH >= {2})) AND F94038 >= DATEADD(MONTH, -12, {4}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, marca, an_c, luna_c, an_ant, data);
            }
            else
            {
                sql_tmp = " SELECT DISTINCT CONVERT(VARCHAR,F94037,103) AS DI, CONVERT(VARCHAR,F94038,103) AS DSF FROM F940 WHERE F940607 = '{0}' ";
                sql_tmp += " AND F94003 = {1} AND F94010 <> 0 ";
                sql_tmp += " AND ((YEAR ={2} AND MONTH < {3}) OR (YEAR = {4} AND MONTH >= {3})) AND F94038 >= DATEADD(MONTH, -12, {5}) ";
                sql_tmp += " AND (F94010 IN (SELECT CODE1 FROM MARDEF) OR  ";
                sql_tmp += " F94010 IN (SELECT CODE2 FROM MARDEF) OR F94010 IN (SELECT CODE3 FROM MARDEF) OR F94010 IN (SELECT CODE4 FROM MARDEF))";

                sql = string.Format(sql_tmp, cod_ind, marca, an_c, luna_c, an_ant, data);

                if (Convert.ToInt32(cod_ind) == 9)
                {
                    sql += " AND F940617 = '" + CNPCopil + "'";
                }
            }

            dt = General.IncarcaDT(sql, null);

            if (dt != null && dt.Rows.Count > 0)
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime dtStart = new DateTime(Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DI"].ToString().Substring(0, 2)));
                    if (ddi_med > dtStart)
                        dtStart = ddi_med;
                    DateTime dtSfarsit = new DateTime(Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(6, 4)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(3, 2)), Convert.ToInt32(dt.Rows[i]["DSF"].ToString().Substring(0, 2)));

                    days += (dtSfarsit - dtStart).Days + 1;
                }

            if (Convert.ToInt32(cod_ind) == 9)
                limitazile = 45;
            else if (Convert.ToInt32(cod_ind) == 8)
                    limitazile = 126;
                 else if (Convert.ToInt32(cod_ind) == 15)
                            limitazile = 120;
                      else if (Convert.ToInt32(cod_ind) == 12 || Convert.ToInt32(cod_ind) == 13 || Convert.ToInt32(cod_ind) == 14)
                                limitazile = 365;
                           else if (Convert.ToInt32(cod_ind) == 1 || Convert.ToInt32(cod_ind) == 5 || Convert.ToInt32(cod_ind) == 6 || Convert.ToInt32(cod_ind) == 10)
                                    limitazile = 90;
                                else
                                    limitazile = 99999;

            return days;
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
                sql_tmp = " SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, TO_CHAR(F30037,'DD/MM/YYYY') AS F30037, TO_CHAR(F30038,'DD/MM/YYYY') AS F30038, F300606, F300608, F300620, F300621 FROM F300 WHERE ";
            else
                sql_tmp = " SELECT DISTINCT F300601, F300602, F300612, F300613, F300614, CONVERT(VARCHAR, F30037, 103) AS F30037, CONVERT(VARCHAR, F30038, 103) AS F30038, F300606, F300608, F300620, F300621 FROM F300 WHERE ";
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
                string msg = OnOK();

                string url = "", param = "";
                CriptDecript prc = new CriptDecript();
                if (Session["CM_Aprobare"] != null && Session["CM_Aprobare"].ToString() == "1")
                {
                    //param = prc.EncryptString(Constante.cheieCriptare, "Aprobare", 1);
                    param = "2";
                    url = "~/ConcediiMedicale/Aprobare?tip=" + param;
                }
                else
                {
                    //param = prc.EncryptString(Constante.cheieCriptare, "Introducere", 1);
                    param = "1";
                    url = "~/ConcediiMedicale/Aprobare?tip=" + param;
                }

                if (msg.Length <= 0)
                {
                    if (Page.IsCallback)
                        ASPxWebControl.RedirectOnCallback(url);
                    else
                        Response.Redirect(url, false);
                }
                else
                {
                    MessageBox.Show(msg, MessageBox.icoError);

                    txtNrZile.Text = Session["CM_NrZile"] == null ? txtNrZile.Text : Session["CM_NrZile"].ToString();
                }
            }
            catch (Exception ex)
            {
                //ArataMesaj("");
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
            rbConcCont.Checked = false;
            txtBCCM.Text = "";
            txtZBC.Text = "";
            txtMZ.Text = "";
            chkStagiu.Checked = false;
            chkUrgenta.Checked = false;
            txtZCMAnt.Text = "";
            txtSCMInit.Text = "";
            txtNrCMInit.Text = "";
            cmbLocPresc.SelectedIndex = -1;
            txtNrAviz.Text = "";
            deDataAviz.Value = null;
            txtMedic.Text = "";
            cmbCNPCopil.Value = null;
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

        protected void btnDocUpload_FileUploadComplete(object sender, DevExpress.Web.FileUploadCompleteEventArgs e)
        {
            try
            {

                string sql = "SELECT * FROM \"tblFisiere\"";
                DataTable dt = new DataTable();                   
                dt = General.IncarcaDT(sql, null);          
                            
                DataRow dr = null;
                if (Session["CM_Id"] == null || (Session["CM_Id"] != null && dt.Select("Tabela = 'CM_Cereri' AND Id = " + Session["CM_Id"].ToString()).Count() == 0))
                {
                    dr = dt.NewRow();
                    dr["Tabela"] = "CM_Cereri";
                    dr["Id"] = Session["CM_Id"] == null ? Dami.NextId("CM_Cereri") : Convert.ToInt32(Session["CM_Id"].ToString());
                    Session["CM_Id_Nou"] = Convert.ToInt32(dr["Id"].ToString());
                    dr["Fisier"] = btnDocUpload.UploadedFiles[0].FileBytes;
                    dr["FisierNume"] = btnDocUpload.UploadedFiles[0].FileName;
                    dr["FisierExtensie"] = btnDocUpload.UploadedFiles[0].ContentType;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;
                    if (Constante.tipBD == 1)
                        dr["IdAuto"] = Convert.ToInt32(General.Nz(dt.AsEnumerable().Where(p => p.RowState != DataRowState.Deleted).Max(p => p.Field<int?>("IdAuto")), 0)) + 1;
                    else
                        dr["IdAuto"] = Dami.NextId("tblFisiere");
                    dr["EsteCerere"] = 0;
                    dt.Rows.Add(dr);

                    if (Session["CM_Id"] != null)
                        General.ExecutaNonQuery("UPDATE CM_Cereri SET Document = 1 WHERE Id = " + Session["CM_Id"].ToString(), null);
                }
                else
                {
                    dr = dt.Select("Tabela = 'CM_Cereri' AND Id = " + Session["CM_Id"].ToString()).FirstOrDefault();
                    dr["Fisier"] = btnDocUpload.UploadedFiles[0].FileBytes;
                    dr["FisierNume"] = btnDocUpload.UploadedFiles[0].FileName;
                    dr["FisierExtensie"] = btnDocUpload.UploadedFiles[0].ContentType;
                    dr["USER_NO"] = Session["UserId"];
                    dr["TIME"] = DateTime.Now;

                    General.ExecutaNonQuery("UPDATE CM_Cereri SET Document = 1 WHERE Id = " + Session["CM_Id"].ToString(), null);
                }

                General.SalveazaDate(dt, "tblFisiere");
                Session["CM_Document"] = "1";
                MemoryStream ms = new MemoryStream(btnDocUpload.UploadedFiles[0].FileBytes);

                pnlCtl.JSProperties["cpAlertMessage"] = "Proces realizat cu succes!";          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


    }




    //<legend class="legend-font-size">Coduri concediu</legend>
    // <legend class="legend-font-size">Coduri transfer</legend>



}