using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WizOne.Generatoare.Reports.Models;

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
            return new Dictionary<string, string>(); // Not permitted to display reports list on designer!
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
            var sourceReport = entities.Reports.Find(int.Parse((string)xtraReport.Tag ?? "0"));

            if (sourceReport != null)
            {
                var report = new Report()
                {
                    Name = defaultUrl,
                    Description = sourceReport.Description,
                    ReportTypeId = sourceReport.ReportTypeId,
                    RegUserId = sourceReport.RegUserId
                };

                entities.Reports.Add(report);
                entities.SaveChanges();

                // Update internal data
                xtraReport.Name = defaultUrl.
                    Split(' ').
                    Where(val => val.Length > 0).
                    Select(val => string.Concat(char.ToUpper(val[0]), val.Substring(1))).
                    Aggregate((curr, next) => curr + next);
                xtraReport.DisplayName = defaultUrl;
                xtraReport.Tag = report.ReportId;

                using (var memStream = new MemoryStream())
                {
                    xtraReport.SaveLayoutToXml(memStream);
                    report.LayoutData = memStream.GetBuffer();
                }

                entities.SaveChanges();

                return report.ReportId.ToString();
            }

            return null;
        }
    }
}
