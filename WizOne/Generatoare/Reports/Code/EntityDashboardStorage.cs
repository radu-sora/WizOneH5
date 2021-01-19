using DevExpress.DashboardWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
                using (var entities = new ReportsEntities())
                {
                    var report = entities.Reports.Find(int.Parse(id));

                    if (report != null && report.LayoutData != null)
                        dashboardXDoc = XDocument.Parse(Encoding.UTF8.GetString(report.LayoutData));
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