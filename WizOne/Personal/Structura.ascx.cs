using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web;
using WizOne.Module;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Drawing;

namespace WizOne.Personal
{
    public partial class Structura : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Traducere

                lgStruc.InnerText = Dami.TraduCuvant("Structura organizatorica");

                lblStru.InnerText = Dami.TraduCuvant("Structura");
                lblCom.InnerText = Dami.TraduCuvant("Companie");
                lblSub.InnerText = Dami.TraduCuvant("Subcompanie");
                lblFil.InnerText = Dami.TraduCuvant("Filiala");
                lblSec.InnerText = Dami.TraduCuvant("Sectie");
                lblDept.InnerText = Dami.TraduCuvant("Departament");
                lblSubdept.InnerText = Dami.TraduCuvant("Subdepartament");
                lblBir.InnerText = Dami.TraduCuvant("Birou/Echipa");
                lblCC.InnerText = Dami.TraduCuvant("Centru cost");
                lblPL.InnerText = Dami.TraduCuvant("Punct de lucru");
                lblCAEN.InnerText = Dami.TraduCuvant("CAEN");
                lblUnitStat.InnerText = Dami.TraduCuvant("Unitate locala statistica");

                btnCC.ToolTip = Dami.TraduCuvant("Modificari contract");
                btnCC.ToolTip = Dami.TraduCuvant("Istoric modificari");
                btnCC.ToolTip = Dami.TraduCuvant("Modificari contract");
                btnCC.ToolTip = Dami.TraduCuvant("Istoric modificari");

                #endregion


                DataTable table = new DataTable();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                table = ds.Tables[0];
             
                //DataTable dtSrc = General.GetStructOrgModif(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
                DataTable dtSrc = General.GetStructOrgModif(DateTime.Now);
                cmbStru.DataSource = dtSrc;
                cmbStru.DataBind();

                string dataRef = DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString();
                string sql = "";         
                if (!IsPostBack)
                {//Radu 01.11.2019 
                    DataRow[] drCC = dtSrc.Select("F00607=" + table.Rows[0]["F10007"].ToString());
                    int cc = (drCC != null && drCC.Count() > 0 && drCC[0]["CC"] != null && drCC[0]["CC"].ToString().Length > 0 ? Convert.ToInt32(drCC[0]["CC"].ToString()) : 9999);
                    sql = @"SELECT * FROM F062 WHERE F06208 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F06209 " +
                        (cc != 9999 ? " AND F06204 = " + cc  : "" );
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("F062", "F06204") + " WHERE F06208 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F06209 " +
                            (cc != 9999 ? " AND F06204 = " + cc : ""); ;
                    DataTable dtCC = General.IncarcaDT(sql, null);
                    cmbCC.DataSource = dtCC;
                    cmbCC.DataBind();

                    string centreCost = "";
                    for (int i = 0; i < dtCC.Rows.Count; i++)
                    {
                        centreCost += dtCC.Rows[i]["F06204"].ToString() + "," + dtCC.Rows[i]["F06205"].ToString();
                        if (i < dtCC.Rows.Count - 1)
                            centreCost += ";";
                    }
                    Session["MP_ComboCC"] = centreCost;
                }
                else
                {
                    int dep = Convert.ToInt32(ds.Tables[0].Rows[0]["F10007"].ToString());                    
                    DataRow[] drCC = dtSrc.Select("F00607=" + dep);
                    int cc = (drCC != null && drCC.Count() > 0 && drCC[0]["CC"] != null && drCC[0]["CC"].ToString().Length > 0 ? Convert.ToInt32(drCC[0]["CC"].ToString()) : 9999);
                    if (hfCC.Contains("CC")) cc = Convert.ToInt32(General.Nz(hfCC["CC"], 9999));
                    sql = @"SELECT * FROM F062 WHERE F06208 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F06209 " +
                        (cc != 9999 ? " AND F06204 = " + cc : "");
                    if (Constante.tipBD == 2)
                        sql = General.SelectOracle("F062", "F06204") + " WHERE F06208 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F06209 " +
                            (cc != 9999 ? " AND F06204 = " + cc : ""); ;
                    DataTable dtCC = General.IncarcaDT(sql, null);
                    cmbCC.DataSource = dtCC;
                    cmbCC.DataBind();
                }

                sql = @"SELECT * FROM F080 WHERE F08020 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F08021";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F080", "F08002") + " WHERE F08020 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F08021 ";
                DataTable dtPL = General.IncarcaDT(sql, null);
                cmbPL.DataSource = dtPL;
                cmbPL.DataBind();

                sql = @"SELECT * FROM LOCATIE_MUNCA";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("LOCATIE_MUNCA", "ID_LOCATIE");
                DataTable dtLoc = General.IncarcaDT(sql, null);
                cmbLocatie.DataSource = dtLoc;
                cmbLocatie.DataBind();

                DataTable dtBir = General.IncarcaDT("SELECT CAST(F00809 AS INT) AS F00809, F00810 FROM F008 WHERE 1=1 " + (Convert.ToInt32(General.Nz(table.Rows[0]["F100958"], "0")) <= 0 
                    ? (Constante.tipBD == 1 ? " AND F00814 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00815" : " AND F00814 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00815")
                    : " AND F00808 = " + Convert.ToInt32(General.Nz(table.Rows[0]["F100958"], "0"))
                    + (Constante.tipBD == 1 ? " AND F00814 <= CONVERT(DATETIME, '" + dataRef + "', 103) AND CONVERT(DATETIME, '" + dataRef + "', 103) <= F00815" : " AND F00814 <= TO_DATE('" + dataRef + "', 'dd/mm/yyyy') AND TO_DATE('" + dataRef + "', 'dd/mm/yyyy') <= F00815")), null);
                cmbBir.DataSource = dtBir;
                cmbBir.DataBind();

                sql = @"SELECT * FROM F801";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F801", "F80103");
                DataTable dtCAEN = General.IncarcaDT(sql, null);
                cmbCAEN.DataSource = dtCAEN;
                cmbCAEN.DataBind();

                sql = @"SELECT * FROM F803";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F803", "F80303");
                DataTable dtUnit = General.IncarcaDT(sql, null);
                cmbUnitStat.DataSource = dtUnit;
                cmbUnitStat.DataBind();


                if (!Page.IsCallback)
                {
                    int f10003 = -99;
                    if (table != null && table.Rows.Count > 0) f10003 = Convert.ToInt32(General.Nz(table.Rows[0]["F10003"], -99));
                    DataTable dtF100 = General.GetStructOrgAng(f10003);
                    if (dtF100 != null && dtF100.Rows.Count > 0)
                    {
                        txtCom.Text = General.Nz(dtF100.Rows[0]["F00204"], "").ToString();
                        txtSub.Text = General.Nz(dtF100.Rows[0]["F00305"], "").ToString();
                        txtFil.Text = General.Nz(dtF100.Rows[0]["F00406"], "").ToString();
                        txtSec.Text = General.Nz(dtF100.Rows[0]["F00507"], "").ToString();
                        txtDept.Text = General.Nz(dtF100.Rows[0]["F00608"], "").ToString();
                        txtSubdept.Text = General.Nz(dtF100.Rows[0]["F00709"], "").ToString();
                        cmbBir.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F100959"], "0"));
                        cmbCC.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F10053"], "0"));
                        cmbPL.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F10079"], "0"));
                        cmbLocatie.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F1001046"], "0"));
                        cmbCAEN.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F1001095"], "0"));
                        cmbUnitStat.Value = Convert.ToInt32(General.Nz(table.Rows[0]["F1001097"], "0"));
                    }
                }

                if (Dami.ValoareParam("ValidariPersonal") == "1")
                {
                    List<int> lst = new List<int>();
                    if (Session["MP_CuloareCampOblig"] != null)
                        lst = Session["MP_CuloareCampOblig"] as List<int>;
                    cmbStru.BackColor = (lst.Count > 0 ? Color.FromArgb(lst[0], lst[1], lst[2]) : Color.LightGray);
                    //cmbPL.BackColor = Color.LightGray;
                }           
                General.SecuritatePersonal(pnlCtlStruct, Convert.ToInt32(Session["UserId"].ToString()));
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }


        protected void cmbStructOrg_SelectedIndexChanged()
        {
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;

            ListEditItem itm = cmbStru.SelectedItem;

            if (itm != null)
            {
                if (General.Nz(itm.GetFieldValue("F00202"), "").ToString() != "")
                {
                    txtCom.Value = itm.GetFieldValue("F00204");
                    ds.Tables[0].Rows[0]["F10002"] = itm.GetFieldValue("F00202");
                    ds.Tables[1].Rows[0]["F10002"] = itm.GetFieldValue("F00202");
                    ds.Tables[2].Rows[0]["F10002"] = itm.GetFieldValue("F00202");
                }
                else
                {
                    txtCom.Value = "";
                    ds.Tables[0].Rows[0]["F10002"] = 0;
                    ds.Tables[1].Rows[0]["F10002"] = 0;
                    ds.Tables[2].Rows[0]["F10002"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00304"), "").ToString() != "")
                {
                    txtSub.Value = itm.GetFieldValue("F00305");
                    ds.Tables[0].Rows[0]["F10004"] = itm.GetFieldValue("F00304");
                    ds.Tables[1].Rows[0]["F10004"] = itm.GetFieldValue("F00304");
                }
                else
                {
                    txtSub.Value = "";
                    ds.Tables[0].Rows[0]["F10004"] = 0;
                    ds.Tables[1].Rows[0]["F10004"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00405"), "").ToString() != "")
                {
                    txtFil.Value = itm.GetFieldValue("F00406");
                    ds.Tables[0].Rows[0]["F10005"] = itm.GetFieldValue("F00405");
                    ds.Tables[1].Rows[0]["F10005"] = itm.GetFieldValue("F00405");
                }
                else
                {
                    txtFil.Value = "";
                    ds.Tables[0].Rows[0]["F10005"] = 0;
                    ds.Tables[1].Rows[0]["F10005"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00506"), "").ToString() != "")
                {
                    txtSec.Value = itm.GetFieldValue("F00507");
                    ds.Tables[0].Rows[0]["F10006"] = itm.GetFieldValue("F00506");
                    ds.Tables[1].Rows[0]["F10006"] = itm.GetFieldValue("F00506");
                }
                else
                {
                    txtSec.Value = "";
                    ds.Tables[0].Rows[0]["F10006"] = 0;
                    ds.Tables[1].Rows[0]["F10006"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00607"), "").ToString() != "")
                {
                    txtDept.Value = itm.GetFieldValue("F00608");
                    ds.Tables[0].Rows[0]["F10007"] = itm.GetFieldValue("F00607");
                    ds.Tables[1].Rows[0]["F10007"] = itm.GetFieldValue("F00607");
                    //Radu 26.08.2019
                    if (General.Nz(itm.GetFieldValue("CC"), "").ToString() != "0")
                    {
                        cmbCC.Value = itm.GetFieldValue("CC");
                        ds.Tables[0].Rows[0]["F10053"] = itm.GetFieldValue("CC");
                        ds.Tables[1].Rows[0]["F10053"] = itm.GetFieldValue("CC");
                    }
                }
                else
                {
                    txtDept.Value = "";
                    ds.Tables[0].Rows[0]["F10007"] = 0;
                    ds.Tables[1].Rows[0]["F10007"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00708"), "").ToString() != "")
                {
                    txtSubdept.Value = itm.GetFieldValue("F00709");
                    ds.Tables[0].Rows[0]["F100958"] = itm.GetFieldValue("F00708");
                    ds.Tables[2].Rows[0]["F100958"] = itm.GetFieldValue("F00708");
                }
                else
                {
                    txtSubdept.Value = "";
                    ds.Tables[0].Rows[0]["F100958"] = 0;
                    ds.Tables[2].Rows[0]["F100958"] = 0;
                }
                if (General.Nz(itm.GetFieldValue("F00809"), "").ToString() != "")
                {
                    cmbBir.Value = Convert.ToInt32(General.Nz(itm.GetFieldValue("F00809"), ""));
                    ds.Tables[0].Rows[0]["F100959"] = itm.GetFieldValue("F00809");
                    ds.Tables[2].Rows[0]["F100959"] = itm.GetFieldValue("F00809");
                }
                else
                {
                    cmbBir.Value = 0;
                    ds.Tables[0].Rows[0]["F100959"] = 0;
                    ds.Tables[2].Rows[0]["F100959"] = 0;
                }
               
         
                DataTable dtBir = General.IncarcaDT("SELECT CAST(F00809 AS INT) AS F00809, F00810 FROM F008 " + (Convert.ToInt32(General.Nz(ds.Tables[0].Rows[0]["F100958"], "0")) <= 0 ? ""
                    : " WHERE F00808 = " + Convert.ToInt32(General.Nz(ds.Tables[0].Rows[0]["F100958"], "0"))), null);
                cmbBir.DataSource = dtBir;
                cmbBir.DataBind();
            }
            Session["InformatiaCurentaPersonal"] = ds;
        }

        protected void pnlCtlStruct_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "cmbStru":
                    cmbStructOrg_SelectedIndexChanged();
                    break;
                case "cmbBir":
                    ds.Tables[0].Rows[0]["F100959"] = param[1];
                    ds.Tables[2].Rows[0]["F100959"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCC":
                    ds.Tables[0].Rows[0]["F10053"] = param[1];
                    ds.Tables[1].Rows[0]["F10053"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbPL":
                    ds.Tables[0].Rows[0]["F10079"] = param[1];
                    ds.Tables[1].Rows[0]["F10079"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbLocatie":
                    ds.Tables[0].Rows[0]["F1001046"] = param[1];
                    ds.Tables[2].Rows[0]["F1001046"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCAEN":
                    ds.Tables[0].Rows[0]["F1001095"] = param[1];
                    ds.Tables[2].Rows[0]["F1001095"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbUnitStat":
                    ds.Tables[0].Rows[0]["F1001097"] = param[1];
                    ds.Tables[2].Rows[0]["F1001097"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "btnCC":
                    ModifAvans((int)Constante.Atribute.CentrulCost);
                    break;
                case "btnPL":
                    ModifAvans((int)Constante.Atribute.PunctLucru);
                    break;
            }

        }

        private void ModifAvans(int atribut)
        {
            string strRol = Avs.Cereri.DamiRol(Convert.ToInt32(General.Nz(Session["Marca"], -99)), atribut);
            if (strRol == "")
            {
                if (Page.IsCallback)
                {
                    //DateIdentificare_pnlCtl.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Nu aveti drepturi pentru aceasta operatie");
                }
                else
                    MessageBox.Show("Nu aveti drepturi pentru aceasta operatie", MessageBox.icoWarning, "Atentie");
            }
            else
            {
                string url = "~/Avs/Cereri.aspx";
                Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString() + ";" + strRol;
                Session["MP_Avans"] = "true";
                Session["MP_Avans_Tab"] = "Structura";
                if (Page.IsCallback)
                    ASPxWebControl.RedirectOnCallback(url);
                else
                    Response.Redirect(url, false);
            }
        }







    }
}







