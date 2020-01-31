using System;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;

namespace Wizrom.Reports.Code
{
    public class ReportsDbContext : DbContext
    {       
        private static string _entityConnectionString;

        public ReportsDbContext() : base(_entityConnectionString)
        {            
        }

        public static void RegisterGlobalConnectionString(string reportsPath, string connectionString)
        {
            if (string.IsNullOrEmpty(reportsPath))
                throw new ArgumentException("Invalid reports path value");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Invalid connection string value");

            reportsPath = reportsPath.Replace('/', '.').Replace('\\', '.').Trim(new char[] { '.' });

            var isOracle = connectionString.IndexOf("Initial Catalog", StringComparison.OrdinalIgnoreCase) == -1;
            var metadata = $"res://*/{reportsPath}.Reports.Models.Reports{(isOracle ? "Ora" : "")}Model.ssdl|" +
                $"res://*/{reportsPath}.Reports.Models.ReportsModel.csdl|" +
                $"res://*/{reportsPath}.Reports.Models.ReportsModel.msl";
            var provider = isOracle ? "Oracle.ManagedDataAccess.Client" : "System.Data.SqlClient";

            _entityConnectionString = (new EntityConnectionStringBuilder()
            {
                Metadata = metadata,
                Provider = provider,
                ProviderConnectionString = connectionString
            }).ToString();
        }
    }
}