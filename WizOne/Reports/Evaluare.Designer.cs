namespace WizOne.Reports
{
    partial class Evaluare
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
            this.lblTitlu = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.xrPageInfo2 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.xrPictureBox1 = new DevExpress.XtraReports.UI.XRPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 19.79167F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblTitlu,
            this.xrPictureBox1});
            this.TopMargin.HeightF = 96.875F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblTitlu
            // 
            this.lblTitlu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(154)))), ((int)(((byte)(99)))));
            this.lblTitlu.Font = new System.Drawing.Font("Times New Roman", 16F);
            this.lblTitlu.LocationFloat = new DevExpress.Utils.PointFloat(0F, 27.16667F);
            this.lblTitlu.Name = "lblTitlu";
            this.lblTitlu.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblTitlu.SizeF = new System.Drawing.SizeF(503.125F, 59.70832F);
            this.lblTitlu.StylePriority.UseBackColor = false;
            this.lblTitlu.StylePriority.UseFont = false;
            this.lblTitlu.StylePriority.UsePadding = false;
            this.lblTitlu.StylePriority.UseTextAlignment = false;
            this.lblTitlu.Text = "Formular de apreciere a performanţei individuale";
            this.lblTitlu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // BottomMargin
            // 
            this.BottomMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo2,
            this.xrPageInfo1});
            this.BottomMargin.HeightF = 47F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrPageInfo2
            // 
            this.xrPageInfo2.Format = "{0:dd.MM.yyyy}";
            this.xrPageInfo2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 15.99998F);
            this.xrPageInfo2.Name = "xrPageInfo2";
            this.xrPageInfo2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo2.PageInfo = DevExpress.XtraPrinting.PageInfo.DateTime;
            this.xrPageInfo2.SizeF = new System.Drawing.SizeF(225.0423F, 18F);
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Format = "Pagina {0} din {1}";
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(593.7495F, 15.99998F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(156.2505F, 18F);
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // xrPictureBox1
            // 
            this.xrPictureBox1.LocationFloat = new DevExpress.Utils.PointFloat(503.125F, 27.16667F);
            this.xrPictureBox1.Name = "xrPictureBox1";
            this.xrPictureBox1.SizeF = new System.Drawing.SizeF(246.875F, 59.70832F);
            this.xrPictureBox1.Sizing = DevExpress.XtraPrinting.ImageSizeMode.StretchImage;
            // 
            // Evaluare
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin});
            this.DefaultPrinterSettingsUsing.UseLandscape = true;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(49, 28, 97, 47);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.RequestParameters = false;
            this.Version = "17.1";
            this.BeforePrint += new System.Drawing.Printing.PrintEventHandler(this.Evaluare_BeforePrint);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel lblTitlu;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo2;
        private DevExpress.XtraReports.UI.XRPageInfo xrPageInfo1;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox1;
    }
}
