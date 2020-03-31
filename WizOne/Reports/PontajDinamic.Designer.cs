namespace WizOne.Reports
{
    partial class PontajDinamic
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
            DevExpress.XtraReports.UI.XRSummary xrSummary1 = new DevExpress.XtraReports.UI.XRSummary();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.lblAntet = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPerioada = new DevExpress.XtraReports.UI.XRLabel();
            this.lblTitlu = new DevExpress.XtraReports.UI.XRLabel();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.lblSemnatura = new DevExpress.XtraReports.UI.XRLabel();
            this.lblPagina = new DevExpress.XtraReports.UI.XRPageInfo();
            this.f10003 = new DevExpress.XtraReports.Parameters.Parameter();
            this.luna = new DevExpress.XtraReports.Parameters.Parameter();
            this.an = new DevExpress.XtraReports.Parameters.Parameter();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.idUser = new DevExpress.XtraReports.Parameters.Parameter();
            this.alMeu = new DevExpress.XtraReports.Parameters.Parameter();
            this.idRol = new DevExpress.XtraReports.Parameters.Parameter();
            this.idDept = new DevExpress.XtraReports.Parameters.Parameter();
            this.idLimba = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.idAngajat = new DevExpress.XtraReports.Parameters.Parameter();
            this.idSubcompanie = new DevExpress.XtraReports.Parameters.Parameter();
            this.idFiliala = new DevExpress.XtraReports.Parameters.Parameter();
            this.idSectie = new DevExpress.XtraReports.Parameters.Parameter();
            this.idStare = new DevExpress.XtraReports.Parameters.Parameter();
            this.idContract = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.idSubdept = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.HeightF = 0F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblAntet,
            this.lblPerioada,
            this.lblTitlu});
            this.TopMargin.HeightF = 170F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblAntet
            // 
            this.lblAntet.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.lblAntet.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 77F);
            this.lblAntet.Multiline = true;
            this.lblAntet.Name = "lblAntet";
            this.lblAntet.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblAntet.SizeF = new System.Drawing.SizeF(796.9999F, 74.99998F);
            this.lblAntet.StylePriority.UseFont = false;
            this.lblAntet.StylePriority.UseTextAlignment = false;
            xrSummary1.FormatString = "{0:#}";
            this.lblAntet.Summary = xrSummary1;
            this.lblAntet.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblPerioada
            // 
            this.lblPerioada.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.lblPerioada.LocationFloat = new DevExpress.Utils.PointFloat(0F, 54F);
            this.lblPerioada.Name = "lblPerioada";
            this.lblPerioada.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblPerioada.SizeF = new System.Drawing.SizeF(797.0001F, 23F);
            this.lblPerioada.StylePriority.UseFont = false;
            this.lblPerioada.StylePriority.UseTextAlignment = false;
            this.lblPerioada.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblTitlu
            // 
            this.lblTitlu.Font = new System.Drawing.Font("Times New Roman", 14F);
            this.lblTitlu.LocationFloat = new DevExpress.Utils.PointFloat(0F, 30.99999F);
            this.lblTitlu.Name = "lblTitlu";
            this.lblTitlu.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblTitlu.SizeF = new System.Drawing.SizeF(797.0001F, 23.00001F);
            this.lblTitlu.StylePriority.UseFont = false;
            this.lblTitlu.StylePriority.UseTextAlignment = false;
            this.lblTitlu.Text = "Foaie colectiva de prezenta";
            this.lblTitlu.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 33.33333F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblSemnatura
            // 
            this.lblSemnatura.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.lblSemnatura.LocationFloat = new DevExpress.Utils.PointFloat(3.178914E-05F, 0F);
            this.lblSemnatura.Multiline = true;
            this.lblSemnatura.Name = "lblSemnatura";
            this.lblSemnatura.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 120, 5, 0, 100F);
            this.lblSemnatura.SizeF = new System.Drawing.SizeF(796.9999F, 23F);
            this.lblSemnatura.StylePriority.UseBorders = false;
            this.lblSemnatura.StylePriority.UsePadding = false;
            this.lblSemnatura.StylePriority.UseTextAlignment = false;
            this.lblSemnatura.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            this.lblSemnatura.WordWrap = false;
            // 
            // lblPagina
            // 
            this.lblPagina.Format = "Pagina {0} din {1}";
            this.lblPagina.LocationFloat = new DevExpress.Utils.PointFloat(1.589457E-05F, 0F);
            this.lblPagina.Name = "lblPagina";
            this.lblPagina.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 20, 0, 0, 100F);
            this.lblPagina.SizeF = new System.Drawing.SizeF(797.0001F, 18F);
            this.lblPagina.StylePriority.UsePadding = false;
            this.lblPagina.StylePriority.UseTextAlignment = false;
            this.lblPagina.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // f10003
            // 
            this.f10003.Name = "f10003";
            // 
            // luna
            // 
            this.luna.Name = "luna";
            // 
            // an
            // 
            this.an.Name = "an";
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // idUser
            // 
            this.idUser.Name = "idUser";
            // 
            // alMeu
            // 
            this.alMeu.Name = "alMeu";
            // 
            // idRol
            // 
            this.idRol.Name = "idRol";
            // 
            // idDept
            // 
            this.idDept.Name = "idDept";
            // 
            // idLimba
            // 
            this.idLimba.Name = "idLimba";
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblPagina});
            this.PageFooter.HeightF = 35.41667F;
            this.PageFooter.Name = "PageFooter";
            // 
            // idAngajat
            // 
            this.idAngajat.Name = "idAngajat";
            // 
            // idSubcompanie
            // 
            this.idSubcompanie.Name = "idSubcompanie";
            // 
            // idFiliala
            // 
            this.idFiliala.Name = "idFiliala";
            // 
            // idSectie
            // 
            this.idSectie.Name = "idSectie";
            // 
            // idStare
            // 
            this.idStare.Name = "idStare";
            // 
            // idContract
            // 
            this.idContract.Name = "idContract";
            // 
            // ReportFooter
            // 
            this.ReportFooter.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblSemnatura});
            this.ReportFooter.HeightF = 23.00002F;
            this.ReportFooter.Name = "ReportFooter";
            this.ReportFooter.StylePriority.UseBorders = false;
            // 
            // idSubdept
            // 
            this.idSubdept.Name = "idSubdept";
            // 
            // PontajDinamic
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.PageFooter,
            this.ReportFooter});
            this.DefaultPrinterSettingsUsing.UseLandscape = true;
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(15, 15, 170, 33);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.RequestParameters = false;
            this.Version = "17.1";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel lblPerioada;
        private DevExpress.XtraReports.UI.XRLabel lblTitlu;
        private DevExpress.XtraReports.Parameters.Parameter f10003;
        private DevExpress.XtraReports.Parameters.Parameter luna;
        private DevExpress.XtraReports.Parameters.Parameter an;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private DevExpress.XtraReports.Parameters.Parameter idUser;
        private DevExpress.XtraReports.Parameters.Parameter alMeu;
        private DevExpress.XtraReports.Parameters.Parameter idRol;
        private DevExpress.XtraReports.Parameters.Parameter idDept;
        private DevExpress.XtraReports.Parameters.Parameter idLimba;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.Parameters.Parameter idAngajat;
        private DevExpress.XtraReports.Parameters.Parameter idSubcompanie;
        private DevExpress.XtraReports.Parameters.Parameter idFiliala;
        private DevExpress.XtraReports.Parameters.Parameter idSectie;
        private DevExpress.XtraReports.Parameters.Parameter idStare;
        private DevExpress.XtraReports.Parameters.Parameter idContract;
        private DevExpress.XtraReports.UI.XRPageInfo lblPagina;
        private DevExpress.XtraReports.UI.XRLabel lblSemnatura;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel lblAntet;
        private DevExpress.XtraReports.Parameters.Parameter idSubdept;
    }
}
