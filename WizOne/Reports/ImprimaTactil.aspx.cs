using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class ImprimaTactil : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dami.AccesTactil();

            lblMarca.InnerText = "MARCA: " + Session["User_Marca"].ToString();
            lblNume.InnerText = "NUME: " + Session["User_NumeComplet"].ToString();

            #region Traducere
            string ctlPost = Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0) Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

            //btnBack.Text = Dami.TraduCuvant("btnBack", "Inapoi");

            #endregion

            string titlu = "";
            if (Session["TactilAdeverinte"] != null)
                switch (Session["TactilAdeverinte"].ToString())
                {
                    case "Angajat":
                        titlu = "Adeverinta de angajat";
                        break;
                    case "Practica":
                        titlu = "Adeverinta de practica";
                        break;
                    case "Gradinita":
                        titlu = "Adeverinta de cresa/gradinita";
                        break;
                    case "Medic":
                        titlu = "Adeverinta de sanatate/medic";
                        break;
                }

            //txtTitlu.Text = Dami.TraduCuvant(titlu);

            rvGeneral.ReportTypeName = "WizOne.Reports." + Session["PrintDocument"];
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                string tip = General.Nz(Request["tip"], "").ToString();
                if (General.Nz(Session["PaginaWeb"], "").ToString() != "") Response.Redirect("~/" + General.Nz(Session["PaginaWeb"], "").ToString() + (tip == "" ? "" : "?tip=" + tip), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["PrintDocument"] != null && !Session["PrintDocument"].ToString().Contains("Adeverinta"))
                {
                    if (VerificaFluturasLog() == 0)
                    {
                        MessageBox.Show("Ati atins numarul maxim de imprimari pentru acest tip de fluturas si pentru luna si anul selectate!", MessageBox.icoWarning, "");
                    }
                    else
                    {
                        ScrieInFluturasLog();

                        Reports.FluturasHarting dlreport = new Reports.FluturasHarting();
                        dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                        dlreport.Margins.Top = 10;
                        dlreport.Margins.Bottom = 10;
                        dlreport.Margins.Left = 50;
                        dlreport.Margins.Right = 50;
                        dlreport.PrintingSystem.ShowMarginsWarning = false;
                        dlreport.ShowPrintMarginsWarning = false;
                        dlreport.CreateDocument();
                        
                        ReportPrintTool pt = new ReportPrintTool(dlreport);

                        pt.Print();
                    }
                }
                else
                {
                    if (Session["TactilAdeverinte"].ToString() == "Medic")
                    {
                        Reports.AdeverintaMedic dlreport = new Reports.AdeverintaMedic();
                        dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                        dlreport.Margins.Top = 10;
                        dlreport.Margins.Bottom = 10;
                        dlreport.Margins.Left = 50;
                        dlreport.Margins.Right = 50;
                        dlreport.PrintingSystem.ShowMarginsWarning = false;
                        dlreport.ShowPrintMarginsWarning = false;
                        dlreport.CreateDocument();
                        ReportPrintTool pt = new ReportPrintTool(dlreport);
                        pt.Print();
                        General.LogAdeverinta(dlreport);
                    }
                    else
                    {
                        Reports.AdeverintaGenerala dlreport = new Reports.AdeverintaGenerala();
                        dlreport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                        dlreport.Margins.Top = 10;
                        dlreport.Margins.Bottom = 10;
                        dlreport.Margins.Left = 50;
                        dlreport.Margins.Right = 50;
                        dlreport.PrintingSystem.ShowMarginsWarning = false;
                        dlreport.ShowPrintMarginsWarning = false;
                        dlreport.CreateDocument();
                        ReportPrintTool pt = new ReportPrintTool(dlreport);
                        pt.Print();
                        General.LogAdeverinta(dlreport);
                    }
                }
                btnBack_Click(sender, e);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }
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
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
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
                    areDrepturi = Convert.ToInt32(General.Nz(General.ExecutaScalar(strSql, null), 1));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                General.MemoreazaEroarea(ex, System.IO.Path.GetFileName(Page.AppRelativeVirtualPath), new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name);
            }

            return areDrepturi;

        }


    }
}