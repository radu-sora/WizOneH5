using System;
using DevExpress.XtraReports.UI;
using System.Linq;
using System.Web;

namespace WizOne.Reports
{
    public partial class AvsXDec_OrdinDeplasare : DevExpress.XtraReports.UI.XtraReport
    {

        public AvsXDec_OrdinDeplasare()
        {
            InitializeComponent();
        }


        private void RaportForm8_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            int DocumentId = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_IdDocument"].ToString()); 
            int F10003 = Convert.ToInt32(HttpContext.Current.Session["AvsXDec_Marca"].ToString());

            for (int i = 0; i < this.Parameters.Count; i++)
                this.Parameters[i].Visible = false;
        }

        private void PrintFata_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRSubreport)sender).ReportSource.Parameters["DocumentId"].Value =
                    Convert.ToInt32(HttpContext.Current.Session["AvsXDec_IdDocument"].ToString());
            ((XRSubreport)sender).ReportSource.Parameters["F10003"].Value =
                    Convert.ToInt32(HttpContext.Current.Session["AvsXDec_Marca"].ToString());
        }

        private void PrintVerso_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRSubreport)sender).ReportSource.Parameters["DocumentId"].Value =
                   Convert.ToInt32(HttpContext.Current.Session["AvsXDec_IdDocument"].ToString());
            ((XRSubreport)sender).ReportSource.Parameters["F10003"].Value =
                    Convert.ToInt32(HttpContext.Current.Session["AvsXDec_Marca"].ToString());
        }

    }
}
 