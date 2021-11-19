namespace  WizOne.Reports
{
    partial class AvsXDec_OrdinDeplasare
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPageBreak1 = new DevExpress.XtraReports.UI.XRPageBreak();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.PrintFata = new DevExpress.XtraReports.UI.XRSubreport();
            this.DocumentId = new DevExpress.XtraReports.Parameters.Parameter();
            this.F10003 = new DevExpress.XtraReports.Parameters.Parameter();
            this.DetailReport1 = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail2 = new DevExpress.XtraReports.UI.DetailBand();
            this.PrintVerso = new DevExpress.XtraReports.UI.XRSubreport();
            this.xrPageBreak2 = new DevExpress.XtraReports.UI.XRPageBreak();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Expanded = false;
            this.Detail.HeightF = 0F;
            this.Detail.MultiColumn.Mode = DevExpress.XtraReports.UI.MultiColumnMode.UseColumnCount;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 2.833328F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak1});
            this.BottomMargin.HeightF = 4.374981F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageBreak1
            // 
            this.xrPageBreak1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrPageBreak1.Name = "xrPageBreak1";
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1});
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageBreak2,
            this.PrintFata});
            this.Detail1.HeightF = 25.83333F;
            this.Detail1.Name = "Detail1";
            // 
            // PrintFata
            // 
            this.PrintFata.LocationFloat = new DevExpress.Utils.PointFloat(1.907349E-05F, 0F);
            this.PrintFata.Name = "PrintFata";
            this.PrintFata.ReportSource = new  WizOne.Reports.AvsXDec_OrdinDeplasare_Fata();
            this.PrintFata.SizeF = new System.Drawing.SizeF(574F, 23F);
            this.PrintFata.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.PrintFata_BeforePrint);
            // 
            // DocumentId
            // 
            this.DocumentId.Name = "DocumentId";
            // 
            // F10003
            // 
            this.F10003.Name = "F10003";
            // 
            // DetailReport1
            // 
            this.DetailReport1.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail2});
            this.DetailReport1.Level = 1;
            this.DetailReport1.Name = "DetailReport1";
            // 
            // Detail2
            // 
            this.Detail2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.PrintVerso});
            this.Detail2.HeightF = 28.125F;
            this.Detail2.Name = "Detail2";
            // 
            // PrintVerso
            // 
            this.PrintVerso.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 0F);
            this.PrintVerso.Name = "PrintVerso";
            this.PrintVerso.ReportSource = new  WizOne.Reports.AvsXDec_OrdinDeplasare_Verso();
            this.PrintVerso.SizeF = new System.Drawing.SizeF(574F, 23F);
            this.PrintVerso.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.PrintVerso_BeforePrint);
            // 
            // xrPageBreak2
            // 
            this.xrPageBreak2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 21F);
            this.xrPageBreak2.Name = "xrPageBreak2";
            // 
            // AvsXDec_OrdinDeplasare
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.DetailReport,
            this.DetailReport1});
            this.DefaultPrinterSettingsUsing.UseLandscape = true;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(4, 5, 3, 4);
            this.PageHeight = 827;
            this.PageWidth = 583;
            this.PaperKind = System.Drawing.Printing.PaperKind.A5;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.DocumentId,
            this.F10003});
            this.RequestParameters = false;
            this.Version = "14.1";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.RaportForm8_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.XRSubreport PrintFata;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak1;
        private DevExpress.XtraReports.Parameters.Parameter DocumentId;
        private DevExpress.XtraReports.Parameters.Parameter F10003;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport1;
        private DevExpress.XtraReports.UI.DetailBand Detail2;
        private DevExpress.XtraReports.UI.XRSubreport PrintVerso;
        private DevExpress.XtraReports.UI.XRPageBreak xrPageBreak2;
    }
}
