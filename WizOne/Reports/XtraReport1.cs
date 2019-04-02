using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using WizOne.Module;

namespace WizOne.Reports
{
    public partial class XtraReport1 : DevExpress.XtraReports.UI.XtraReport
    {
        public XtraReport1()
        {
            InitializeComponent();
            
            this.DataSource = General.IncarcaDT("Select TOP 10 F10003 FROM F100", null);
        }

    }
}
