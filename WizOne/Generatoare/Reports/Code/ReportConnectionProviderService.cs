using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;

namespace WizOne.Generatoare.Reports.Code
{
    public class ReportConnectionProviderService : IConnectionProviderService
    {
        public SqlDataConnection LoadConnection(string connectionName)
        {            
            return new SqlDataConnection(connectionName, new ReportDataSourceWizardConnectionStringsProvider().GetDataConnectionParameters(connectionName));
        }
    }
}