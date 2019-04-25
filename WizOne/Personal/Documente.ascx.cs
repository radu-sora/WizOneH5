using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;
using System.Drawing;

namespace WizOne.Personal
{
    public partial class Documente : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            DataTable table = new DataTable();

            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            table = ds.Tables[0];

            DataList1.DataSource = table;
            DataList1.DataBind();

            ASPxComboBox cmbTara = DataList1.Items[0].FindControl("cmbTara") as ASPxComboBox;
            ASPxComboBox cmbTipDoc = DataList1.Items[0].FindControl("cmbTipDoc") as ASPxComboBox;

            cmbTipDoc.DataSource = General.GetTipDoc(Convert.ToInt32(table.Rows[0]["F100987"] == DBNull.Value ? "0" : table.Rows[0]["F100987"].ToString()));
            cmbTipDoc.DataBindItems();

            string[] etichete = new string[37] { "lblTara", "lblCetatenie", "lblTipAutMunca", "lblDataInc", "lblDataSf", "lblNumeMama", "lblNumeTata", "lblTipDoc", "lblSerieNr", "lblEmisDe", "lblLocNastere", "lblDataELib", "lblDataExp",
                                                 "lblNrPermisMunca", "lblDataPermisMunca", "lblNrCtrIntVechi", "lblDataCtrIntVechi", "lblDetaliiCtrAngajat", "lblCateg", "lblDataEmitere", "lblDataExpirare", "lblNr", "lblPermisEmisDe",
                                                 "lblStudii", "lblCalif1", "lblCalif2", "lblTitluAcademic", "lblDedSomaj", "lblNrCarteMunca", "lblSerieCarteMunca", "lblDataCarteMunca", "lblLivret", "lblElibDe", "lblDeLaData", "lblLaData",
                                                 "lblGrad", "lblOrdin"};
            for (int i = 0; i < etichete.Count(); i++)
            {
                ASPxLabel lbl = DataList1.Items[0].FindControl(etichete[i]) as ASPxLabel;
                lbl.Text = Dami.TraduCuvant(lbl.Text) + ": ";
            }

            string[] butoane = new string[8] { "btnDocId", "btnDocIdIst", "btnPermis", "btnPermisIst", "btnStudii", "btnStudiiIst", "btnTitluAcad", "btnTitluAcadIst"  };
            for (int i = 0; i < butoane.Count(); i++)
            {
                ASPxButton btn = DataList1.Items[0].FindControl(butoane[i]) as ASPxButton;
                btn.ToolTip = Dami.TraduCuvant(btn.ToolTip);
            }

            if (Convert.ToInt32(General.Nz(Session["IdClient"], 1)) == 22)
            {//DNATA
                string[] lstTextBox = new string[2] { "txtSerieNr", "txtEmisDe" };
                for (int i = 0; i < lstTextBox.Count(); i++)
                {
                    ASPxTextBox txt = DataList1.Items[0].FindControl(lstTextBox[i]) as ASPxTextBox;
                    txt.BackColor = Color.LightGray;
                }

                string[] lstDateEdit = new string[2] { "deDataElib", "deDataExp" };
                for (int i = 0; i < lstDateEdit.Count(); i++)
                {
                    ASPxDateEdit de = DataList1.Items[0].FindControl(lstDateEdit[i]) as ASPxDateEdit;
                    de.BackColor = Color.LightGray;
                }


                string[] lstComboBox = new string[1] { "cmbTipDoc" };
                for (int i = 0; i < lstComboBox.Count(); i++)
                {
                    ASPxComboBox cmb = DataList1.Items[0].FindControl(lstComboBox[i]) as ASPxComboBox;
                    cmb.BackColor = Color.LightGray;
                }

            }

            General.SecuritatePersonal(DataList1, Convert.ToInt32(Session["UserId"].ToString()));

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //ASPxPageControl ctl = this.Parent as ASPxPageControl;
                //if (ctl == null) return;
                //TabPage tab = ctl.ActiveTabPage;
                ////TabPage tab = e.Tab;

                //if (tab.Name == "Documente" && Session["PreluareDate"] != null && Session["PreluareDate"].ToString() == "1")
                //{
                //    Session["PreluareDate"] = 0;
                //    for (int j = 0; j < tab.Controls[0].Controls.Count; j++)
                //    {
                //        if (tab.Controls[0].Controls[j].GetType() == typeof(DevExpress.Web.ASPxCallbackPanel))
                //        {
                //            ASPxCallbackPanel cb = tab.Controls[0].Controls[j] as ASPxCallbackPanel;
                //            for (int k = 0; k < cb.Controls.Count; k++)
                //            {
                //                if (cb.Controls[k].GetType() == typeof(DataList))
                //                {
                //                    DataList dl = cb.Controls[k] as DataList;
                //                    DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
                //                    DataTable table = ds.Tables[0];
                //                    dl.DataSource = table;
                //                    dl.DataBind();
                //                    break;
                //                }
                //            }
                //            break;
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath));
            }
        }

        protected void pnlCtlDocumente_Callback(object source, CallbackEventArgsBase e)
        {
            string[] param = e.Parameter.Split(';');
            DataSet ds = Session["InformatiaCurentaPersonal"] as DataSet;
            switch (param[0])
            {
                case "cmbTara":
                    cmbTara_SelectedIndexChanged();
                    ds.Tables[0].Rows[0]["F100987"] = param[1];
                    ds.Tables[1].Rows[0]["F100987"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCetatenie":
                    ds.Tables[0].Rows[0]["F100981"] = param[1];
                    ds.Tables[1].Rows[0]["F100981"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTipAutMunca":
                    ds.Tables[0].Rows[0]["F100911"] = param[1];
                    ds.Tables[1].Rows[0]["F100911"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataInc":
                    string[] data = param[1].Split('.');
                    if (deDataInceput_EditValueChanged(ds.Tables[1].Rows[0]["F100912"]))
                    {
                        ds.Tables[0].Rows[0]["F100912"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F100912"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "deDataSf":
                    data = param[1].Split('.');
                    if (deDataSfarsit_EditValueChanged(ds.Tables[1].Rows[0]["F100913"]))
                    {
                        ds.Tables[0].Rows[0]["F100913"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F100913"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "txtNumeMama":
                    ds.Tables[0].Rows[0]["F100988"] = param[1];
                    ds.Tables[1].Rows[0]["F100988"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNumeTata":
                    ds.Tables[0].Rows[0]["F100989"] = param[1];
                    ds.Tables[1].Rows[0]["F100989"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTipDoc":
                    ds.Tables[0].Rows[0]["F100983"] = param[1];
                    ds.Tables[1].Rows[0]["F100983"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtSerieNr":
                    ds.Tables[0].Rows[0]["F10052"] = param[1];
                    ds.Tables[1].Rows[0]["F10052"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtEmisDe":
                    ds.Tables[0].Rows[0]["F100521"] = param[1];
                    ds.Tables[1].Rows[0]["F100521"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtLocNastere":
                    ds.Tables[0].Rows[0]["F100980"] = param[1];
                    ds.Tables[1].Rows[0]["F100980"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataElib":
                    if (deDataEliberarii_LostFocus())
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F100522"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F100522"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "deDataExp":
                    if (deDataExpirarii_LostFocus())
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F100963"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[2].Rows[0]["F100963"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "txtNrPermisMunca":
                    ds.Tables[0].Rows[0]["F100982"] = param[1];
                    ds.Tables[1].Rows[0]["F100982"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataPermisMunca":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["F100994"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["F100994"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrCtrIntVechi":
                    ds.Tables[0].Rows[0]["F100940"] = param[1];
                    ds.Tables[1].Rows[0]["F100940"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtDataCtrIntVechi":
                    ds.Tables[0].Rows[0]["F100941"] = param[1];
                    ds.Tables[1].Rows[0]["F100941"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtDetaliiCtrAngajat":
                    ds.Tables[0].Rows[0]["F100942"] = param[1];
                    ds.Tables[1].Rows[0]["F100942"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbCateg":
                    ds.Tables[0].Rows[0]["F10028"] = param[1];
                    ds.Tables[1].Rows[0]["F10028"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataEmitere":
                    if (deDataEmiterePermis_EditValueChanged(ds.Tables[1].Rows[0]["F10024"]))
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F10024"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F10024"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "deDataExpirare":
                    if (deDataExpirarePermis_EditValueChanged(ds.Tables[2].Rows[0]["F1001000"]))
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F1001000"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[2].Rows[0]["F1001000"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "txtNr":
                    ds.Tables[0].Rows[0]["F1001001"] = param[1];
                    ds.Tables[2].Rows[0]["F1001001"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtPermisEmisDe":
                    ds.Tables[0].Rows[0]["F1001002"] = param[1];
                    ds.Tables[2].Rows[0]["F1001002"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbStudiiDoc":
                    ds.Tables[0].Rows[0]["F10050"] = param[1];
                    ds.Tables[1].Rows[0]["F10050"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtCalif1":
                    ds.Tables[0].Rows[0]["F100903"] = param[1];
                    ds.Tables[1].Rows[0]["F100903"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtCalif2":
                    ds.Tables[0].Rows[0]["F100904"] = param[1];
                    ds.Tables[1].Rows[0]["F100904"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbTitluAcademic":
                    ds.Tables[0].Rows[0]["F10051"] = param[1];
                    ds.Tables[1].Rows[0]["F10051"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "cmbDedSomaj":
                    ds.Tables[0].Rows[0]["F10073"] = param[1];
                    ds.Tables[1].Rows[0]["F10073"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtNrCarteMunca":
                    ds.Tables[0].Rows[0]["F10012"] = param[1];
                    ds.Tables[1].Rows[0]["F10012"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtSerieCarteMunca":
                    ds.Tables[0].Rows[0]["F10013"] = param[1];
                    ds.Tables[1].Rows[0]["F10013"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDataCarteMunca":
                    data = param[1].Split('.');
                    ds.Tables[0].Rows[0]["FX1"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    ds.Tables[1].Rows[0]["FX1"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtLivret":
                    ds.Tables[0].Rows[0]["F100571"] = param[1];
                    ds.Tables[1].Rows[0]["F100571"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtElibDe":
                    ds.Tables[0].Rows[0]["F100572"] = param[1];
                    ds.Tables[1].Rows[0]["F100572"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "deDeLaDataLivMil":
                    if (txtLivretMilitarDeLaData_EditValueChanged(ds.Tables[1].Rows[0]["F100573"]))
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F100573"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F100573"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "deLaDataLivMil":
                    if (txtLivretMilitarLaData_EditValueChanged(ds.Tables[1].Rows[0]["F100574"]))
                    {
                        data = param[1].Split('.');
                        ds.Tables[0].Rows[0]["F100574"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        ds.Tables[1].Rows[0]["F100574"] = new DateTime(Convert.ToInt32(data[2]), Convert.ToInt32(data[1]), Convert.ToInt32(data[0]));
                        Session["InformatiaCurentaPersonal"] = ds;
                    }
                    break;
                case "txtGrad":
                    ds.Tables[0].Rows[0]["F100575"] = param[1];
                    ds.Tables[1].Rows[0]["F100575"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "txtOrdin":
                    ds.Tables[0].Rows[0]["F100576"] = param[1];
                    ds.Tables[1].Rows[0]["F100576"] = param[1];
                    Session["InformatiaCurentaPersonal"] = ds;
                    break;
                case "btnDocId":
                    ModifAvans((int)Constante.Atribute.DocId);
                    break;
                case "btnPermis":
                    ModifAvans((int)Constante.Atribute.PermisAuto);
                    break;
                case "btnStudii":
                    ModifAvans((int)Constante.Atribute.Studii);
                    break;
                case "btnTitluAcad":
                    ModifAvans((int)Constante.Atribute.TitluAcademic);
                    break;
            }

        }


        private bool deDataEliberarii_LostFocus()
        {
            bool valid = true;
            try
            {
                ASPxDateEdit deDataExpirarii = DataList1.Items[0].FindControl("deDataExp") as ASPxDateEdit;
                ASPxDateEdit deDataEliberarii = DataList1.Items[0].FindControl("deDataElib") as ASPxDateEdit;
                if (deDataExpirarii.Value != null)
                {
                    if (Convert.ToDateTime(deDataEliberarii.Value).Date > Convert.ToDateTime(deDataExpirarii.Value).Date)
                    {
                        pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data eliberarii BI/CI este ulterioara datei expirarii!");     
                        deDataEliberarii.Value = null;
                        valid = false;
                    }
                    if (Convert.ToDateTime(deDataEliberarii.Value).Date > DateTime.Now.Date)
                    {
                        pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data eliberarii BI/CI este ulterioara zilei curente!");
                        deDataEliberarii.Value = null;
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        private bool deDataExpirarii_LostFocus()
        {
            bool valid = true;
            try
            {
                ASPxDateEdit deDataExpirarii = DataList1.Items[0].FindControl("deDataExp") as ASPxDateEdit;
                ASPxDateEdit deDataEliberarii = DataList1.Items[0].FindControl("deDataElib") as ASPxDateEdit;
                if (deDataEliberarii.Value != null)
                {
                    if (Convert.ToDateTime(deDataEliberarii.Value) > Convert.ToDateTime(deDataExpirarii.Value))
                    {
                        pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data expirarii BI/CI nu poate fi mai mica decat data eliberarii!");
                        deDataExpirarii.Value = null;
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
            return valid;
        }

        private void SeteazaCetatenie()
        {
            try
            {
                //F733 ent = cmbTara.SelectedItem as F733;

                //if (ent != null)
                //{
                //    srvPersonal ctxPersonal = new srvPersonal();
                //    LoadOperation<metaTipDoc> loTipDocument = ctxPersonal.Load<metaTipDoc>(ctxPersonal.GetTipDocQuery(Convert.ToInt32(ent.F73302)), LoadBehavior.RefreshCurrent, lc =>
                //    {
                //        if (lc != null && lc.Entities.Count() > 0)
                //            //cmbTipDocument.ItemsSource = lc.Entities.Distinct().OrderBy(p=> p.Denumire);
                //            cmbTipDocument.ItemsSource = lc.Entities.Distinct().OrderBy(p => p.Id);

                //        //cmbTipDocument.SelectedIndex = 0;

                //    }, null);

                //    srvBuiltIn ctx = new srvBuiltIn();
                //    LoadOperation<F732> loCategoriePermis = ctx.Load<F732>(ctx.GetF732Query().Where(p => p.F73202 == (ent.F73306 == null ? -99 : ent.F73306)), LoadBehavior.RefreshCurrent, lc =>
                //    {
                //        string cet = "";
                //        if (lc != null && lc.Entities.Count() > 0)
                //            cet = lc.Entities.FirstOrDefault().F73204.ToString();

                //        cmbCetatenie.ItemsSource = lc.Entities.OrderBy(p => p.F73204);
                //        cmbCetatenie.SelectedIndex = 0;

                //        if (ent.F73306 == 3)
                //        {
                //            cmbAutorizatie.IsEnabled = true;
                //            deDataInceput.IsEnabled = true;
                //            deDataSfarsit.IsEnabled = true;
                //        }
                //        else
                //        {
                //            cmbAutorizatie.IsEnabled = false;
                //            deDataInceput.IsEnabled = false;
                //            deDataSfarsit.IsEnabled = false;
                //        }
                //    }, null);
                //}
                //else
                //{
                //    cmbTipDocument.ItemsSource = null;
                //    cmbCetatenie.ItemsSource = null;
                //}
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private void cmbTara_SelectedIndexChanged()
        {
            try
            {
                SeteazaCetatenie();
            }
            catch (Exception ex)
            {
                //Constante.ctxGeneral.MemoreazaInfo(ex.ToString(), this.ToString(), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

        }


        private bool deDataInceput_EditValueChanged(object F100912)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit deDataInceput = DataList1.Items[0].FindControl("deDataInc") as ASPxDateEdit;
            ASPxDateEdit deDataSfarsit = DataList1.Items[0].FindControl("deDataSf") as ASPxDateEdit;

            if (deDataSfarsit.Value != null)
                if (Convert.ToDateTime(deDataSfarsit.Value) < Convert.ToDateTime(deDataInceput.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                    deDataInceput.Value = (F100912 == DBNull.Value ? dt : Convert.ToDateTime(F100912));
                    valid = false;
                }
            return valid;
        }

        private bool deDataSfarsit_EditValueChanged(object F100913)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit deDataInceput = DataList1.Items[0].FindControl("deDataInc") as ASPxDateEdit;
            ASPxDateEdit deDataSfarsit = DataList1.Items[0].FindControl("deDataSf") as ASPxDateEdit;

            if (deDataSfarsit.Value != null)
                if (Convert.ToDateTime(deDataSfarsit.Value) < Convert.ToDateTime(deDataInceput.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                    deDataInceput.Value = (F100913 == DBNull.Value ? dt : Convert.ToDateTime(F100913));
                    valid = false;
                }
            return valid;
        }

        private bool deDataEmiterePermis_EditValueChanged(object F10024)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit deDataEmiterePermis = DataList1.Items[0].FindControl("deDataEmitere") as ASPxDateEdit;
            ASPxDateEdit deDataExpirarePermis = DataList1.Items[0].FindControl("deDataExpirare") as ASPxDateEdit;

            if (deDataExpirarePermis.Value != null)
                if (Convert.ToDateTime(deDataExpirarePermis.Value) < Convert.ToDateTime(deDataEmiterePermis.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data expirarii este anterioara datei emiterii!");
                    deDataEmiterePermis.Value = (F10024 == DBNull.Value ? dt : Convert.ToDateTime(F10024));
                    valid = false;
                }
            return valid;
        }

        private bool deDataExpirarePermis_EditValueChanged(object F1001000)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit deDataEmiterePermis = DataList1.Items[0].FindControl("deDataEmitere") as ASPxDateEdit;
            ASPxDateEdit deDataExpirarePermis = DataList1.Items[0].FindControl("deDataExpirare") as ASPxDateEdit;

            if (deDataExpirarePermis.Value != null)
                if (Convert.ToDateTime(deDataExpirarePermis.Value) < Convert.ToDateTime(deDataEmiterePermis.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data expirarii este anterioara datei emiterii!");
                    deDataEmiterePermis.Value = (F1001000 == DBNull.Value ? dt : Convert.ToDateTime(F1001000));
                    valid = false;
                }
            return valid;
        }


        private bool txtLivretMilitarDeLaData_EditValueChanged(object F100573)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit txtLivretMilitarDeLaData = DataList1.Items[0].FindControl("deDeLaDataLivMil") as ASPxDateEdit;
            ASPxDateEdit txtLivretMilitarLaData = DataList1.Items[0].FindControl("deLaDataLivMil") as ASPxDateEdit;

            if (txtLivretMilitarLaData.Value != null)
                if (Convert.ToDateTime(txtLivretMilitarLaData.Value) < Convert.ToDateTime(txtLivretMilitarDeLaData.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                    txtLivretMilitarDeLaData.Value = (F100573 == DBNull.Value ? dt : Convert.ToDateTime(F100573));
                    valid = false;
                }

            return valid;
        }

        private bool txtLivretMilitarLaData_EditValueChanged(object F100574)
        {
            bool valid = true;
            DateTime dt = new DateTime(2100, 1, 1, 0, 0, 0);
            ASPxDateEdit txtLivretMilitarDeLaData = DataList1.Items[0].FindControl("deDeLaDataLivMil") as ASPxDateEdit;
            ASPxDateEdit txtLivretMilitarLaData = DataList1.Items[0].FindControl("deLaDataLivMil") as ASPxDateEdit;

            if (txtLivretMilitarLaData.Value != null)
                if (Convert.ToDateTime(txtLivretMilitarLaData.Value) < Convert.ToDateTime(txtLivretMilitarDeLaData.Value))
                {
                    pnlCtlDocumente.JSProperties["cpAlertMessage"] = Dami.TraduCuvant("Data start este ulterioara celei de final!");
                    txtLivretMilitarDeLaData.Value = (F100574 == DBNull.Value ? dt : Convert.ToDateTime(F100574));
                    valid = false;
                }

            return valid;
        }

        //private void ArataMesaj(string mesaj)
        //{
        //    pnlCtlDocumente.Controls.Add(new LiteralControl());
        //    WebControl script = new WebControl(HtmlTextWriterTag.Script);
        //    pnlCtlDocumente.Controls.Add(script);
        //    script.Attributes["id"] = "dxss_123456";
        //    script.Attributes["type"] = "text/javascript";
        //    script.Controls.Add(new LiteralControl("var str = '" + mesaj + "'; alert(str);"));
        //}

        private void ModifAvans(int atribut)
        {
            string url = "~/Avs/Cereri.aspx";
            Session["Marca_Atribut"] = Session["Marca"].ToString() + ";" + atribut.ToString();
            Session["MP_Avans"] = "true";
            Session["MP_Avans_Tab"] = "Documente";
            if (Page.IsCallback)
                ASPxWebControl.RedirectOnCallback(url);
            else
                Response.Redirect(url, false);
        }
    }
}