using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using WizOne.Module;
using Wizrom.Reports.Models;

namespace Wizrom.Reports.Code
{
    public class ReportProxy
    {
        public class ViewDataCache : IDisposable
        {
            private XtraReport _report;            

            public int ReportUserId { get; set; }
            public object ReportParams { get; set; }
            public object ChartOptions { get; set; }
            public XtraReport Report
            {
                get { return _report ?? (_report = new XtraReport()); }
            }

            public void Dispose()
            {
                ReportUserId = 0;
                ReportParams = null;
                ChartOptions = null;
                _report?.Dispose();
                _report = null;
            }
        }

        public class DesignDataCache : IDisposable
        {
            private XtraReport _report;

            public object ReportParams { get; set; }
            public object ChartOptions { get; set; }
            public string GridTempLayout { get; set; }
            public XtraReport Report
            {
                get { return _report ?? (_report = new XtraReport()); }
                set
                {
                    _report?.Dispose();
                    _report = value;
                }
            }

            public void Dispose()
            {
                ReportParams = null;
                ChartOptions = null;
                GridTempLayout = string.Empty;
                _report?.Dispose();
                _report = null;
            }
        }

        private static readonly string[] SESSION_PAGE_NAMES = new string[] { "Reports/Pages/View", "Reports/Pages/Design", "Reports/Pages/Print.ashx" };

        private static string _pagesPath;
        
        public static void Register(string reportsPath, string appConnectionString, string srcConnectionString)
        {            
            if (string.IsNullOrEmpty(reportsPath))
                throw new ArgumentException("Invalid reports path value");

            if (string.IsNullOrEmpty(appConnectionString))
                throw new ArgumentException("Invalid app connection string value");

            if (string.IsNullOrEmpty(srcConnectionString))
                throw new ArgumentException("Invalid src connection string value");

            // DX
            DefaultReportDesignerContainer.RegisterDataSourceWizardConnectionStringsProvider<ReportDataSourceWizardConnectionStringsProvider>(true);
            DefaultReportDesignerContainer.EnableCustomSql();
            ReportStorageWebExtension.RegisterExtensionGlobal(new EntityReportStorageWebExtension());
            // Reports
            DynamicModuleUtility.RegisterModule(typeof(ReportSessionModule));
            ReportsDbContext.RegisterGlobalConnectionString(reportsPath, appConnectionString);
            ReportDataSourceWizardConnectionStringsProvider.RegisterGlobalConnectionString(srcConnectionString);
            // If none throws an exception then ...
            _pagesPath = $"~/{reportsPath.Trim(new char[] { ' ', '/', '\\' })}/Reports/Pages/";
        }

        private static string GetUrl(int reportId, string userId, short toolbarType, string exportOptions, object paramList, bool oncePerGroup)
        {
            if (reportId == 0)
                throw new ArgumentException("Invalid report id value");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("Invalid user id value");

            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>> ??
                new Dictionary<string, Dictionary<string, object>>();
            var group = HttpContext.Current.Request.Url.LocalPath;
            var id = Guid.NewGuid().ToString("N");
            var session = new // Anonymous type to enforce read-only members
            {
                ReportId = reportId,
                UserId = userId,
                ToolbarType = toolbarType, // 0 - full items, 1 - only Print, Customize layout & Exit
                ExportOptions = exportOptions ?? "*", // "pdf,image[...]" or "*" to display all options
                ParamList = paramList,
                DataCache = new ViewDataCache(),
                OncePerGroup = oncePerGroup
            };
            var sessionsGroup = null as Dictionary<string, object>;

            if (reportsSessionsGroups.TryGetValue(group, out sessionsGroup))
                sessionsGroup.Add(id, session);
            else
                reportsSessionsGroups.Add(group, new Dictionary<string, object>() { { id, session } });            

            HttpContext.Current.Session["ReportsSessionsGroups"] = reportsSessionsGroups;

            return $"{_pagesPath}View?id={id}";
        }
        private static string GetUrl(int reportId, object paramList, bool oncePerGroup)
        {
            if (reportId == 0)
                throw new ArgumentException("Invalid report id value");

            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>> ??
                new Dictionary<string, Dictionary<string, object>>();
            var group = HttpContext.Current.Request.Url.LocalPath;
            var id = Guid.NewGuid().ToString("N");
            var session = new // Anonymous type to enforce read-only members
            {
                ReportId = reportId,
                ParamList = paramList,
                DataCache = null as object, // No cache for print handler
                OncePerGroup = oncePerGroup
            };
            var sessionsGroup = null as Dictionary<string, object>;

            if (reportsSessionsGroups.TryGetValue(group, out sessionsGroup))
                sessionsGroup.Add(id, session);
            else
                reportsSessionsGroups.Add(group, new Dictionary<string, object>() { { id, session } });            

            HttpContext.Current.Session["ReportsSessionsGroups"] = reportsSessionsGroups;

            return $"{_pagesPath}Print.ashx?id={id}";
        }
        private static string GetUrl(int reportId, bool oncePerGroup)
        {
            if (reportId == 0)
                throw new ArgumentException("Invalid report id value");

            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>> ??
                new Dictionary<string, Dictionary<string, object>>();
            var group = HttpContext.Current.Request.Url.LocalPath;
            var id = Guid.NewGuid().ToString("N");
            var session = new // Anonymous type to enforce read-only members
            {
                ReportId = reportId,
                DataCache = new DesignDataCache(), // Only for Document, Cube and Table report types
                OncePerGroup = oncePerGroup
            };
            var sessionsGroup = null as Dictionary<string, object>;

            if (reportsSessionsGroups.TryGetValue(group, out sessionsGroup))
                sessionsGroup.Add(id, session);
            else
                reportsSessionsGroups.Add(group, new Dictionary<string, object>() { { id, session } });            

            HttpContext.Current.Session["ReportsSessionsGroups"] = reportsSessionsGroups;

            return $"{_pagesPath}Design?id={id}";
        }

        public static string GetViewUrl(int reportId, string userId, short toolbarType = 0, string exportOptions = "*", object paramList = null)
        {
            paramList = new
            {
                Implicit = new { UserId = HttpContext.Current?.Session?["UserId"] },
                Explicit = paramList
            };
            
            return GetUrl(reportId, userId, toolbarType, exportOptions, paramList, oncePerGroup: true);
        }
        public static string GetViewUrl(int reportId, short toolbarType = 0, string exportOptions = "*", object paramList = null)
        {
            var userId = HttpContext.Current?.Session?["UserId"]?.ToString();
            
            return GetViewUrl(reportId, userId, toolbarType, exportOptions, paramList);
        }
        
        public static void View(int reportId, string userId, short toolbarType = 0, string exportOptions = "*", object paramList = null)
        {
            paramList = new
            {
                Implicit = new { UserId = HttpContext.Current?.Session?["UserId"] },
                Explicit = paramList
            };

            HttpContext.Current.Response.Redirect(GetUrl(reportId, userId, toolbarType, exportOptions, paramList, oncePerGroup: false));
        }
        public static void View(int reportId, short toolbarType = 0, string exportOptions = "*", object paramList = null)
        {
            var userId = HttpContext.Current?.Session?["UserId"]?.ToString();

            View(reportId, userId, toolbarType, exportOptions, paramList);
        }

        public static string GetPrintUrl(int reportId, object paramList = null)
        {
            paramList = new
            {
                Implicit = new { UserId = HttpContext.Current?.Session?["UserId"] },
                Explicit = paramList
            };

            return GetUrl(reportId, paramList, oncePerGroup: true);
        }

        public static void Print(int reportId, object paramList = null)
        {
            if (reportId == 0)
                throw new ArgumentException("Invalid report id value");            

            using (var entities = new ReportsEntities())
            using (var xtraReport = new XtraReport())
            {
                // Load data
                var report = entities.Reports.Find(reportId);

                if (report == null)
                    throw new Exception($"No report found for id {reportId}");

                if (report.LayoutData == null)
                    throw new Exception($"No layout found for report id {reportId}");

                using (var memStream = new MemoryStream(report.LayoutData))
                    xtraReport.LoadLayoutFromXml(memStream);

                // Set internal params
                var values = new
                {
                    Implicit = new { UserId = HttpContext.Current?.Session?["UserId"] },
                    Explicit = paramList
                };                
                var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                var parameters = xtraReport.ObjectStorage.OfType<SqlDataSource>().
                    SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                    Where(p => p.Type != typeof(Expression));

                foreach (var param in parameters)
                {
                    var name = param.Name.TrimStart('@');
                    var value = explicitValues?.SingleOrDefault(p => p.Name == name)?.GetValue(values.Explicit) ??
                        implicitValues.SingleOrDefault(p => p.Name == name)?.GetValue(values.Implicit);

                    if (value != null)
                        param.Value = Convert.ChangeType(value, param.Type);
                }

                xtraReport.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService()); // Temp fix only for Print here
                xtraReport.PaperKind = System.Drawing.Printing.PaperKind.A4;
                //Florin 2020.12.11
                if (Convert.ToInt32(HttpContext.Current.Session["IdClient"]) != Convert.ToInt32(IdClienti.Clienti.TMK))
                {
                    //Florin 2020.06.10
                    xtraReport.Margins.Top = 10;
                    xtraReport.Margins.Bottom = 10;
                    xtraReport.Margins.Left = 50;
                    xtraReport.Margins.Right = 50;
                }
                xtraReport.PrintingSystem.ShowMarginsWarning = false;
                xtraReport.ShowPrintMarginsWarning = false;

                string numeImprimanta = Dami.ValoareParam("TactilImprimanta").Trim();
                if (numeImprimanta != "")
                    xtraReport.PrinterName = numeImprimanta;

                xtraReport.CreateDocument();

                using (var reportPrint = new ReportPrintTool(xtraReport))
                    reportPrint.Print(); // Send to default server printer
            }
        }

        public static string GetDesignUrl(int reportId)
        {
            return GetUrl(reportId, oncePerGroup: true);
        }

        public static void Design(int reportId)
        {
            HttpContext.Current.Response.Redirect(GetUrl(reportId, oncePerGroup: false));
        }

        public static object GetSession()
        {
            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>>;
            var id = HttpContext.Current.Request.QueryString["id"];

            if (reportsSessionsGroups == null)
                throw new Exception("No reports sessions found");

            if (string.IsNullOrEmpty(id))
                throw new Exception("No report session id found");

            var session = null as object;

            foreach (var sessionsGroup in reportsSessionsGroups)
            {
                if (sessionsGroup.Value.TryGetValue(id, out session))
                    break;                
            }

            if (session == null)
                throw new Exception($"No report session found for id {id}");

            return session;
        }

        public static void RemoveSession()
        {
            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>>;
            var group = string.Empty;
            var id = HttpContext.Current.Request.QueryString["id"];

            if (reportsSessionsGroups == null)
                throw new Exception("No reports sessions found");

            if (string.IsNullOrEmpty(id))
                throw new Exception("No report session id found");

            var currentSession = null as dynamic;

            foreach (var sessionsGroup in reportsSessionsGroups)
            {
                if (sessionsGroup.Value.TryGetValue(id, out currentSession))
                {
                    group = sessionsGroup.Key;
                    break;
                }
            }

            if (currentSession == null)
                throw new Exception($"No report session found for id {id}");

            var sessions = reportsSessionsGroups[group];

            if (currentSession.OncePerGroup)
            {                
                foreach (dynamic session in sessions)
                    session.Value.DataCache?.Dispose();

                reportsSessionsGroups.Remove(group);
            }
            else
            {
                currentSession.DataCache?.Dispose();
                sessions.Remove(id);

                if (sessions.Count == 0)
                    reportsSessionsGroups.Remove(group);
            }            
        }

        public static void RecycleSession()
        {
            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");

            var reportsSessionsGroups = HttpContext.Current.Session["ReportsSessionsGroups"] as Dictionary<string, Dictionary<string, object>>;
            var id = HttpContext.Current.Request.QueryString["id"];

            if (reportsSessionsGroups == null)
                throw new Exception("No reports sessions found");

            if (string.IsNullOrEmpty(id))
                throw new Exception("No report session id found");

            var session = null as dynamic;

            foreach (var sessionsGroup in reportsSessionsGroups)
            {
                if (sessionsGroup.Value.TryGetValue(id, out session))
                    break;
            }

            if (session == null)
                throw new Exception($"No report session found for id {id}");

            session.DataCache?.Dispose();
        }

        public static void ClearSessions()
        {
            if (HttpContext.Current?.Session == null)
                throw new Exception("Invalid HTTP context");            
            
            if (HttpContext.Current.Request.RequestType == WebRequestMethods.Http.Get)
            {                
                var urlSegments = HttpContext.Current.Request.Url.Segments;
                var pageName = string.Concat(urlSegments.Skip(Math.Max(0, urlSegments.Count() - 3)));

                if (!SESSION_PAGE_NAMES.Contains(pageName))
                {
                    // Assuming that all sessions was recycled ...
                    (HttpContext.Current.Session["ReportsSessionsGroups"] as IDictionary)?.Clear();
                    HttpContext.Current.Session.Remove("ReportsSessionsGroups");
                    // TODO: FIX04 - Clear recycled or unused sessions only.
                }
            }
        }
    }
}