using DevExpress.Web;
using DevExpress.XtraReports.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Web.UI;
using WizOne.Module;
using System.Data;

namespace WizOne.Tactil
{
    public partial class MainTactil : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Dami.AccesTactil();

                lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
                lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

                spnLuna.MinValue = 1;
                spnLuna.MaxValue = 12;

                spnAnul.MinValue = 2015;
                spnAnul.MaxValue = 2025;



                if (!IsPostBack)
                {
                    spnLuna.Value = Convert.ToInt32(Dami.ValoareParam("LunaLucru"));
                    spnAnul.Value = Convert.ToInt32(Dami.ValoareParam("AnLucru"));

                    DataTable dt = General.IncarcaDT("SELECT * FROM \"tblConfigTactil\" WHERE LOWER(\"Nume\") = 'fluturas' ORDER BY \"Ordine\"", null);
                    if (dt.Rows[0]["IdParam"] != null && dt.Rows[0]["IdParam"].ToString().Length > 0)
                    {
                        Session["FluturasGeneral"] = dt.Rows[0]["IdParam"].ToString();
                        //lblPrint.Visible = false;
                    }
                }
                //else
                //{
                //    if (Session["FluturasGeneral"] != null && Session["FluturasGeneral"].ToString().Length > 0)
                //        lblPrint.Visible = false;
                //}



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnkPre_Click(object sender, EventArgs e)
        {
            try
            {
                //if (VerificaFluturasLog() == 0)
                //{
                //    MessageBox.Show("Ati atins numarul maxim de imprimari pentru acest tip de fluturas si pentru luna si anul selectate!", MessageBox.icoWarning, "");
                //}
                //else
                //{
                //    ScrieInFluturasLog();

                if (!VerifLuna(Convert.ToInt32(spnLuna.Value), Convert.ToInt32(spnAnul.Value)))
                {
                    MessageBox.Show("Luna selectata este ulterioara lunii de lucru!", MessageBox.icoWarning, "");
                    return;
                }

                if (Session["FluturasGeneral"] != null && Session["FluturasGeneral"].ToString().Length > 0)
                {
                    var reportId = Convert.ToInt32(Session["FluturasGeneral"].ToString());                    
                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(reportId);
                    var reportParams = new
                    {
                        Angajat = Session["User_Marca"].ToString(),
                        An = Convert.ToInt32(spnAnul.Value ?? Dami.ValoareParam("AnLucru")),
                        Luna = Convert.ToInt32(spnLuna.Value ?? Dami.ValoareParam("LunaLucru"))
                    };

                    // New report access interface
                    Wizrom.Reports.Code.ReportProxy.View(reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);
                }
                else
                {
                    Session["Fluturas_An"] = Convert.ToInt32(spnAnul.Value ?? Dami.ValoareParam("AnLucru"));
                    Session["Fluturas_Luna"] = Convert.ToInt32(spnLuna.Value ?? Dami.ValoareParam("LunaLucru"));
                    Session["PrintDocument"] = "FluturasHarting";
                    Session["PaginaWeb"] = "Tactil/MainTactil.aspx";
                    Response.Redirect("~/Reports/ImprimaTactil.aspx", false);
                }
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void lnlPri_Click(object sender, EventArgs e)
        {
            try
            {              
                if (!VerifLuna(Convert.ToInt32(spnLuna.Value), Convert.ToInt32(spnAnul.Value)))
                {
                    MessageBox.Show("Luna selectata este ulterioara lunii de lucru!", MessageBox.icoWarning, "");
                    return;
                }

                if (Session["FluturasGeneral"] != null && Session["FluturasGeneral"].ToString().Length > 0)
                {
                    var reportId = Convert.ToInt32(Session["FluturasGeneral"].ToString());
                    var reportSettings = Wizrom.Reports.Pages.Manage.GetReportSettings(reportId);
                    var reportParams = new
                    {
                        Angajat = Session["User_Marca"].ToString(),
                        An = Convert.ToInt32(spnAnul.Value ?? Dami.ValoareParam("AnLucru")),
                        Luna = Convert.ToInt32(spnLuna.Value ?? Dami.ValoareParam("LunaLucru"))
                    };

                    // New report access interface
                    Wizrom.Reports.Code.ReportProxy.View(reportId, reportSettings.ToolbarType, reportSettings.ExportOptions, reportParams);
                }
                else
                {
                    if (VerificaFluturasLog() == 0)
                    {
                        MessageBox.Show("Ati atins numarul maxim de imprimari pentru acest tip de fluturas si pentru luna si anul selectate!", MessageBox.icoWarning, "");
                    }
                    else
                    {
                        ScrieInFluturasLog();

                        Session["Fluturas_An"] = Convert.ToInt32(spnAnul.Value ?? Dami.ValoareParam("AnLucru"));
                        Session["Fluturas_Luna"] = Convert.ToInt32(spnLuna.Value ?? Dami.ValoareParam("LunaLucru"));
                        Reports.FluturasHarting dlreport = new Reports.FluturasHarting();
                        dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                        dlreport.Margins.Top = 10;
                        dlreport.Margins.Bottom = 10;
                        dlreport.Margins.Left = 50;
                        dlreport.Margins.Right = 50;
                        dlreport.PrintingSystem.ShowMarginsWarning = false;
                        dlreport.ShowPrintMarginsWarning = false;

                        //Florin 2020.05.22
                        string numeImprimanta = Dami.ValoareParam("TactilImprimantaFluturas").Trim();
                        if (numeImprimanta == "")
                            numeImprimanta = Dami.ValoareParam("TactilImprimanta").Trim();
                        if (numeImprimanta != "")
                            dlreport.PrinterName = numeImprimanta;

                        dlreport.CreateDocument();

                        ReportPrintTool pt = new ReportPrintTool(dlreport);
                        pt.Print();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            try
            {
                //Radu 24.04.2020
                string tip = Dami.ValoareParam("TipInfoChiosc", "0");
                if (tip == "0")
                    Response.Redirect("../DefaultTactil.aspx", false);
                else if (tip == "1" || tip == "2")
                    Response.Redirect("../DefaultTactilFaraCard.aspx", false);
                else if (tip == "3")
                    Response.Redirect("../DefaultTactilExtra.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        public int VerificaFluturasLog()
        {
            int areDrepturi = 1;

            try
            {
                string usrGrup = Dami.ValoareParam("LimitarePrintareFluturas", "");
                if (usrGrup != "")
                {
                    string strSql = $@"SELECT CASE WHEN (COUNT(*) >= COALESCE((SELECT COALESCE(""NrMaxPrintari"",0) FROM ""DocUnire"" WHERE ""Id""=1),0)) THEN 0 ELSE 1 END AS ""AreDpreturi"" 
                                    FROM (
                                    SELECT distinct A.""IdUser"", A.""An"", A.""Luna"", A.""IdAuto"" 
                                    FROM ""tblFluturasLog"" A
                                    INNER JOIN ""relGrupUser"" B ON A.""IdUser""=B.""IdUser""
                                    INNER JOIN F010 C ON 1=1
                                    WHERE A.""IdUser""={Session["UserId"]} AND A.""An""=C.F01011 AND A.""Luna""=C.F01012 AND A.""IdRaport""=1 AND B.""IdGrup"" IN ({usrGrup})
                                    ) X";
                    areDrepturi = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, null),1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }

            return areDrepturi;

        }

        public void ScrieInFluturasLog()
        {
            try
            {
                if (Dami.ValoareParam("LimitarePrintareFluturas", "") != "")
                {
                    General.ExecutaNonQuery(
                        $@"INSERT INTO tblFluturasLog(IdUser, F10003, An, Luna, IdRaport, USER_NO, TIME)
                        SELECT TOP 1 {Session["UserId"]}, {Session["User_Marca"]}, F01011, F01012, 1, {Session["UserId"]}, GetDate() FROM F010", null);
                }
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
                Response.Redirect("../Tactil/Main.aspx", false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        private bool VerifLuna(int luna, int anul)
        {
            if (Convert.ToInt32(Dami.ValoareParam("AnLucru")) < anul || (Convert.ToInt32(Dami.ValoareParam("AnLucru")) == anul && Convert.ToInt32(Dami.ValoareParam("LunaLucru")) < luna))
                return false;
            else
                return true;
        }



    }
}