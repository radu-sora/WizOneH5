using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using System.Collections.Generic;

namespace WizOne.Generatoare.Reports.Code
{
    public class ReportDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        public Dictionary<string, string> GetConnectionDescriptions()
        {
            return new Dictionary<string, string>() { { "ReportsConnection", "Reports Connection" } };
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            return new CustomStringConnectionParameters(Module.Constante.tipBD == 1 ? "XpoProvider=MSSqlServer;" : "XpoProvider=ODPManaged;" + Module.Constante.cnnWeb);
        }
    }
}