using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;
using System.Web;

namespace WizOne.Reports
{
    public partial class AdeverintaSanatate : DevExpress.XtraReports.UI.XtraReport
    {
        public AdeverintaSanatate()
        {
            InitializeComponent();

        }

        private void AdeverintaSanatate_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string path = HttpContext.Current.Session["AdevSanatate_Cale"].ToString();
            xrRichText1.LoadFile(path, XRRichTextStreamType.XmlText);
        }
    }
}
