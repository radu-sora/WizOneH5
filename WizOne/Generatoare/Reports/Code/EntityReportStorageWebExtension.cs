﻿using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wizrom.Reports.Models;
using Wizrom.Reports.Pages;

namespace Wizrom.Reports.Code
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
            var canSet = false;

            try
            {
                using (var entities = new ReportsEntities())
                {
                    int reportId = int.Parse(url);

                    canSet = entities.Reports.Any(r => r.ReportId == reportId);
                }
            }
            catch (Exception ex)
            {
                // Log error here
            }

            return canSet;
        }

        public override Dictionary<string, string> GetUrls()
        {
            var urls = null as Dictionary<string, string>;

            try
            {
                using (var entities = new ReportsEntities())
                    urls = entities.Reports.Where(r => r.ReportTypeId == 1 || r.ReportTypeId == 2). // Only report & document types for 'Report Source Url' property.
                        ToDictionary(r => r.ReportId.ToString(), r => r.Name);
            }
            catch (Exception ex)
            {
                // Log error here
            }

            return urls;
        }

        public override byte[] GetData(string url)
        {
            var data = null as byte[];

            try
            {
                using (var entities = new ReportsEntities())
                    data = entities.Reports.Find(int.Parse(url))?.LayoutData;

                if (data == null)
                {
                    using (var xtraReport = new XtraReport())
                    {
                        xtraReport.Name = "MissingReport";
                        xtraReport.DisplayName = "Missing report definition";

                        using (var memStream = new MemoryStream())
                        {
                            xtraReport.SaveLayoutToXml(memStream);
                            data = memStream.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error here
            }

            return data;
        }

        public override void SetData(XtraReport xtraReport, string url)
        {
            try
            {
                using (var entities = new ReportsEntities())
                {
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
            }
            catch (Exception ex)
            {
                // Log error here
            }
        }

        public override string SetNewData(XtraReport xtraReport, string defaultUrl)
        {
            var reportId = null as string;

            try
            {
                using (var entities = new ReportsEntities())
                {
                    var sourceReport = !string.IsNullOrEmpty(xtraReport.Tag as string) ? // Save as existing report. 
                        entities.Reports.Find(int.Parse(xtraReport.Tag as string)) : null; // Save new subreport.
                    var newReport = null as Report;

                    entities.Reports.Add(newReport = new Report()
                    {
                        Name = defaultUrl,
                        Description = sourceReport != null ? sourceReport.Description : "Autogenerated subreport",
                        ReportTypeId = sourceReport?.ReportTypeId ?? 1, // Report
                        RegUserId = sourceReport?.RegUserId
                    });
                    entities.SaveChanges();

                    // Update internal data
                    xtraReport.Name = Design.GetReportName(defaultUrl);
                    xtraReport.DisplayName = defaultUrl;
                    xtraReport.Tag = newReport.ReportId;

                    // Save layout changes
                    using (var memStream = new MemoryStream())
                    {
                        xtraReport.SaveLayoutToXml(memStream);
                        newReport.LayoutData = memStream.GetBuffer();
                    }

                    entities.SaveChanges();
                    reportId = newReport.ReportId.ToString();
                }
            }
            catch (Exception ex)
            {
                // Log error here
            }

            return reportId;
        }
    }
}
