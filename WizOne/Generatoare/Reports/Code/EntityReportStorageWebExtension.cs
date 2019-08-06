using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WizOne.Generatoare.Reports.Models;
using WizOne.Generatoare.Reports.Pages;

namespace WizOne.Generatoare.Reports.Code
{
    public class EntityReportStorageWebExtension : ReportStorageWebExtension
    {
        public override bool IsValidUrl(string url)
        {
            int reportId;

            return int.TryParse(url, out reportId);
        }

        public override bool CanSetData(string url)
        {
            var entities = new ReportsEntities();
            int reportId = int.Parse(url);

            return entities.Reports.Any(r => r.ReportId == reportId);
        }

        public override Dictionary<string, string> GetUrls()
        {
            var entities = new ReportsEntities();
            
            return entities.Reports.Where(r => r.ReportTypeId == 5). // Only subreports for 'Report Source Url' property.
                ToDictionary(r => r.ReportId.ToString(), r => r.Name);            
        }

        public override byte[] GetData(string url)
        {
            var entities = new ReportsEntities();
            var report = entities.Reports.Find(int.Parse(url));

            if (report != null)
                return report.LayoutData;

            return null;
        }

        public override void SetData(XtraReport xtraReport, string url)
        {
            var entities = new ReportsEntities();
            var report = entities.Reports.Find(int.Parse(url));

            if (report != null)
            {
                using (var memStream = new MemoryStream())
                {
                    xtraReport.SaveLayoutToXml(memStream);
                    report.LayoutData = memStream.GetBuffer();
                }

                entities.SaveChanges();
            }
        }

        public override string SetNewData(XtraReport xtraReport, string defaultUrl)
        {
            var entities = new ReportsEntities();
            var sourceReport = !string.IsNullOrEmpty((string)xtraReport.Tag) ? // Save as existing report. 
                entities.Reports.Find(int.Parse((string)xtraReport.Tag)) : (Report)null; // Save new subreport.
            var report = (Report)null;

            entities.Reports.Add(report = new Report() {
                Name = defaultUrl,
                Description = sourceReport?.Description,
                ReportTypeId = sourceReport?.ReportTypeId ?? 5, // Subreport
                RegUserId = sourceReport?.RegUserId
            });
            entities.SaveChanges();

            // Update internal data
            xtraReport.Name = ReportDesign.GetReportName(defaultUrl);
            xtraReport.DisplayName = defaultUrl;
            xtraReport.Tag = report.ReportId;

            // Save layout changes
            using (var memStream = new MemoryStream())
            {
                xtraReport.SaveLayoutToXml(memStream);
                report.LayoutData = memStream.GetBuffer();
            }

            entities.SaveChanges();
            
            return report.ReportId.ToString();                   
        }
    }
}
