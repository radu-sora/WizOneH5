using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using static WizOne.Module.Dami;

namespace WizOne.Pagini
{
    public partial class CalculVenit : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesApp(this.Page);

                #region Traducere

                string ctlPost = Request.Params["__EVENTTARGET"];
                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                #endregion

                //txtTitlu.Text = General.VarSession("Titlu").ToString();
                txtTitlu.Text = Dami.TraduCuvant("Simulator venit");

                cmbAng.DataSource = General.GetPersonalRestrans(Convert.ToInt32(General.Nz(Session["UserId"],-99)), "", 1);
                cmbAng.DataBind();

                DataTable dt = General.GetVariabileVB(-99);
                txtCassPro.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["CASS"], 0));
                txtCasPro.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["CAS"], 0));
                txtImpPro.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["ProcImp"], 0));  
                txtSalMediu.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["SalMediu"], 0));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        {
            try
            {
                string tip = e.Parameter;

                switch (tip)
                {
                    case "1":
                        {
                            DataTable dt = General.GetVariabileVB(Convert.ToInt32(General.Nz(cmbAng.Value, -99)));
                            cmbPers.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["NrDed"], 0));
                            txtTipAng.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["TipAng"], 0));
                            chkScutit.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["Scutit"], 0));
                            //txtRez1.Text =
                            //    "Scuttit           " + Convert.ToInt32(General.Nz(dt.Rows[0]["Scutit"], 0)).ToString() + Environment.NewLine +
                            //    "Tip Angajat       " + Convert.ToInt32(General.Nz(dt.Rows[0]["TipAng"], 0)) + Environment.NewLine +
                            //    "Salariul Mediu    " + Convert.ToInt32(General.Nz(dt.Rows[0]["SalMediu"], 0));
                        }
                        break;
                    case "2":
                        {
                            if (txtVenitCal.Value == null || txtCasPro.Value == null || txtCassPro.Value == null || txtImpPro.Value == null || cmbPers.Value == null)
                            {
                                pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipsesc date");
                                return;
                            }

                            int tipVenit = Convert.ToInt32(General.Nz(cmbTip.Value, 1));
                            decimal venit = Convert.ToDecimal(txtVenitCal.Value);

                            decimal venitCalculat = 0m;
                            string text = "";

                            string strSql = $@"SELECT {General.Nz(txtCassPro.Value, 0)} AS CASS, {General.Nz(txtCasPro.Value, 0)} AS CAS, {General.Nz(cmbPers.Value, 0)} AS ""NrDed"", {Convert.ToInt32(General.Nz(chkScutit.Value, 0))} AS ""Scutit"", {General.Nz(txtTipAng.Value, 0)} AS ""TipAng"", {General.Nz(txtSalMediu.Value, 0)} AS ""SalMediu"", {General.Nz(txtImpPro.Value, 0)} AS ""ProcImp"" ";
                            DataTable dt = General.IncarcaDT(strSql, null);

                            General.CalcSalariu(tipVenit, venit, Convert.ToInt32(General.Nz(cmbAng.Value, -99)), out venitCalculat, out text, dt, Convert.ToInt32(General.Nz(txtTicheteTotal.Value,0)));

                            txtVenitRez.Value = venitCalculat;

                            string[] arr = text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string l in arr)
                            {
                                string[] arrTxt = l.Split('=');
                                switch(General.Nz(arrTxt[0],"").ToString().ToUpper())
                                {
                                    case "CAS":
                                        txtCas.Value = arrTxt[1];
                                        break;
                                    case "CASS":
                                        txtCass.Value = arrTxt[1];
                                        break;
                                    case "DEDUCERE":
                                        txtDed.Value = arrTxt[1];
                                        break;
                                    case "IMPOZIT":
                                        txtImp.Value = arrTxt[1];
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                //pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }


        //protected void pnlCtl_Callback(object source, CallbackEventArgsBase e)
        //{
        //    try
        //    {
        //        string tip = e.Parameter;

        //        switch (tip)
        //        {
        //            case "1":
        //                {
        //                    DataTable dt = General.GetVariabileVB(Convert.ToInt32(General.Nz(cmbAng.Value,-99)));
        //                    cmbPers.Value = Convert.ToInt32(General.Nz(dt.Rows[0]["NrDed"], 0));
        //                    txtRez1.Text = 
        //                        "Scuttit           " + Convert.ToInt32(General.Nz(dt.Rows[0]["Scutit"], 0)).ToString() + Environment.NewLine +
        //                        "Tip Angajat       " + Convert.ToInt32(General.Nz(dt.Rows[0]["TipAng"], 0)) + Environment.NewLine +
        //                        "Salariul Mediu    " + Convert.ToInt32(General.Nz(dt.Rows[0]["SalMediu"], 0));
        //                }
        //                break;
        //            case "2":
        //                {
        //                    //if (cmbAng.Value == null)
        //                    //{
        //                    //    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Lipseste angajatul");
        //                    //    return;
        //                    //}

        //                    //if ((txtVenitBrut.Value == null && txtVenitNet.Value == null) || (txtVenitBrut.Value != null && txtVenitNet.Value != null))
        //                    //{
        //                    //    pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Introduceti doar un venit");
        //                    //    return;
        //                    //}

        //                    //int tipVenit = 1;
        //                    //decimal venit = 0;

        //                    //if (txtVenitBrut.Value != null)
        //                    //{
        //                    //    tipVenit = 1;
        //                    //    venit = Convert.ToDecimal(txtVenitBrut.Value);
        //                    //}

        //                    //if (txtVenitNet.Value != null)
        //                    //{
        //                    //    tipVenit = 2;
        //                    //    venit = Convert.ToDecimal(txtVenitNet.Value);
        //                    //}

        //                    int tipVenit = Convert.ToInt32(General.Nz(cmbTip.Value,1));
        //                    decimal venit = Convert.ToDecimal(txtVenitCal.Value);

        //                    decimal venitCalculat = 0m;
        //                    string text = "";
        //                    General.CalcSalariu(tipVenit, venit, Convert.ToInt32(General.Nz(cmbAng.Value, -99)), out venitCalculat, out text);

        //                    //if (tipVenit == 1)
        //                    //    txtVenitNet.Value = venitCalculat;
        //                    //else
        //                    //    txtVenitBrut.Value = venitCalculat;

        //                    txtVenitRez.Value = venitCalculat;

        //                    string[] arr = text.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        //                    foreach(string l in arr)
        //                    {
        //                        string[] arrTxt = l.Split('=');
        //                        txtRez1.Text += arrTxt[0] + Environment.NewLine;
        //                        txtRez2.Text += arrTxt[1] + Environment.NewLine;
        //                    }
        //                }
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //pnlCtl.JSProperties["cpAlertMessage"] = ex.ToString();
        //        General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
        //    }
        //}


    }
}