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

                btnCC.ToolTip = Dami.TraduCuvant("Modificari contract");
                btnCC.ToolTip = Dami.TraduCuvant("Istoric modificari");
                btnCC.ToolTip = Dami.TraduCuvant("Modificari contract");
                btnCC.ToolTip = Dami.TraduCuvant("Istoric modificari");

                #endregion


                DataTable table = new DataTable();

                DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                table = ds.Tables[0];

                DataTable dtSrc = General.GetStructOrgModif(Convert.ToDateTime(ds.Tables[0].Rows[0]["F10022"].ToString()));
                cmbStru.DataSource = dtSrc;
                cmbStru.DataBind();

                string sql = @"SELECT * FROM F062";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F062", "F06204");
                DataTable dtCC = General.IncarcaDT(sql, null);
                cmbCC.DataSource = dtCC;
                cmbCC.DataBind();

                sql = @"SELECT * FROM F080";
                if (Constante.tipBD == 2)
                    sql = General.SelectOracle("F080", "F08002");
                DataTable dtPL = General.IncarcaDT(sql, null);
                cmbPL.DataSource = dtPL;
                cmbPL.DataBind();

                DataTable dtBir = General.IncarcaDT("SELECT CAST(F00809 AS INT) AS F00809, F00810 FROM F008 " + (Convert.ToInt32(General.Nz(table.Rows[0]["F100958"], "0")) <= 0 ? ""
                        : " WHERE F00808 = " + Convert.ToInt32(General.Nz(table.Rows[0]["F100958"], "0"))), null);
                cmbBir.DataSource = dtBir;
                cmbBir.DataBind();


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
                    }
                }

                if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
                {//DNATA
                    cmbStru.BackColor = Color.LightGray;
                    //cmbPL.BackColor = Color.LightGray;
                }
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
                }
                else
                {
                    txtCom.Value = "";
                    ds.Tables[0].Rows[0]["F10002"] = 0;
                    ds.Tables[1].Rows[0]["F10002"] = 0;
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
                    txtSub.Value = "";
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
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "Structura";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
        }







    }
}