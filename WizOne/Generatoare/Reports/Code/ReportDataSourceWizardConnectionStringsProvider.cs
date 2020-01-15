using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;

namespace Wizrom.Reports.Code
{
    public class ReportDataSourceWizardConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider
    {
        private static string _xpoConnectionString;

        public Dictionary<string, string> GetConnectionDescriptions()
        {
            return new Dictionary<string, string>() { { "ReportsConnection", "Reports Connection" } };
        }

        public DataConnectionParametersBase GetDataConnectionParameters(string name)
        {
            return new CustomStringConnectionParameters(_xpoConnectionString);
        }

        public static void RegisterGlobalConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Invalid connection string value");

            var isOracle = connectionString.IndexOf("Initial Catalog", StringComparison.OrdinalIgnoreCase) == -1;
            var provider = isOracle ? "XpoProvider=ODPManaged;" : "XpoProvider=MSSqlServer;";

            _xpoConnectionString = provider + connectionString;
        }
    }
}