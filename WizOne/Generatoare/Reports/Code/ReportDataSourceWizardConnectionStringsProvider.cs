using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using System.Collections.Generic;

namespace WizOne.Generatoare.Reports.Code
{
    public class ReportDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        public Dictionary<string, string> GetConnectionDescriptions()
        {
            Dictionary<string, string> connections = new Dictionary<string, string>();

            connections.Add("ReportsConnection", "Reports Connection");

            return connections;
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            return null; // This force the XtraReport to store only connection name with no parameters and thus, connection parameters from web.config will be used.
        }
    }
}