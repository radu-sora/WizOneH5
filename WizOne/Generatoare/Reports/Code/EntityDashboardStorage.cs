using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using WizOne.Module;
using Wizrom.Reports.Models;

namespace Wizrom.Reports.Code
{
    public class EntityDashboardStorage : IEditableDashboardStorage
    {
        public IEnumerable<DashboardInfo> GetAvailableDashboardsInfo()
        {
            throw new Exception("Not permitted to display dasboards list on designer!");
        }        

        public XDocument LoadDashboard(string id)
        {
            var dashboardXDoc = null as XDocument;

            try
            {
                var reportSession = null as dynamic;

                if (id.Length == 32)
                {
                    reportSession = ReportProxy.GetSession(id);
                    id = reportSession.ReportId.ToString();
                }                

                using (var entities = new ReportsEntities())
                {
                    var report = entities.Reports.Find(int.Parse(id));

                    if (report != null && report.LayoutData != null)
                        dashboardXDoc = XDocument.Parse(Encoding.UTF8.GetString(report.LayoutData));
                }

                if (reportSession != null)
                {
                    using (var dashboard = new Dashboard())
                    {
                        dashboard.LoadFromXDocument(dashboardXDoc);

                        // Set internal params            
                        var values = reportSession.ParamList;
                        var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                        var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                        var parameters = dashboard.DataSources.OfType<SqlDataSource>().
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

                        //Florin 2022.01.07         #1074
                        foreach (var ds in dashboard.DataSources.OfType<DashboardSqlDataSource>())
                        {
                            ds.DataProcessingMode = DataProcessingMode.Client;
                            ds.ConnectionOptions.CommandTimeout = Dami.TimeOutSecunde("TimeOutSecundeDashboards");
                        }

                        dashboardXDoc = dashboard.SaveToXDocument();
                    }
                }
            }
            catch (Exception)
            {
                // Log error here
            }

            return dashboardXDoc; // If null, this will throw exception in dashboard render!
        }

        public void SaveDashboard(string id, XDocument dashboardXDoc)
        {            
            try
            {
                using (var entities = new ReportsEntities())
                {
                    var report = entities.Reports.Find(int.Parse(id));

                    if (report != null)
                    {
                        report.LayoutData = Encoding.UTF8.GetBytes(dashboardXDoc.ToString());
                        entities.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                // Log error here
            }
        }

        public string AddDashboard(XDocument dashboardXDoc, string name)
        {
            var reportId = null as string;

            try
            {
                using (var entities = new ReportsEntities())
                {
                    // TODO: Use DevExpress.DashboardCommon.Dashboard object for layout configuration.
                    var sourceReport = entities.Reports.Find(int.Parse(dashboardXDoc.Descendants("Storage").Attributes("Id").First().Value));

                    if (sourceReport != null)
                    {
                        var report = new Report()
                        {
                            Name = name,
                            Description = sourceReport.Description,
                            ReportTypeId = sourceReport.ReportTypeId,
                            //OnStartPage = sourceReport.OnStartPage,
                            RegUserId = sourceReport.RegUserId
                        };

                        entities.Reports.Add(report);
                        entities.SaveChanges();

                        // Update internal data
                        dashboardXDoc.Descendants("Title").Attributes("Text").First().Value = name;
                        dashboardXDoc.Descendants("Storage").Attributes("Id").First().Value = report.ReportId.ToString();

                        // Save layout changes
                        report.LayoutData = Encoding.UTF8.GetBytes(dashboardXDoc.ToString());
                        entities.SaveChanges();
                        reportId = report.ReportId.ToString();
                    }
                }
            }
            catch (Exception)
            {
                // Log error here
            }

            return reportId;
        }
    }
}