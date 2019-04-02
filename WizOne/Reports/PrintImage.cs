using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;
using System.Web;

namespace WizOne.Reports
{
    public partial class PrintImage : DevExpress.XtraReports.UI.XtraReport
    {
        public PrintImage()
        {
            InitializeComponent();
            xrPictureBox1.Image = Image.FromFile(HttpContext.Current.Session["PrintImage"].ToString());

        }

    }
}
