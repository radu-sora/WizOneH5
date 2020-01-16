using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using WizOne.Module;
using Wizrom.Reports.Code;

namespace Wizrom.Reports.Pages
{
    public partial class Manage : Page
    {
        public class ReportViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public short TypeId { get; set; }
            public bool Restricted { get; set; }
        }

        public class ReportSettingsViewModel
        {
            public string ExportOptions { get; set; }
            public short ToolbarType { get; set; }
        }

        public IEnumerable<ReportViewModel> GetReports()
        {
            var reports = General.RunSqlQuery<ReportViewModel>(
                "SELECT [DynReportId] AS [Id], [Name], [Description], [DynReportTypeId] AS [TypeId], [HasPassword] AS Restricted " +
                "FROM [DynReports] " +
                "WHERE [DynReportId] IN (SELECT [IdRaport] FROM [RapoarteGrupuriUtilizatori] WHERE [IdUser] = @1)", Session["UserId"]);

            return reports;
        }

        public void AddReport(ReportViewModel report)
        {            
            var reportId = General.RunSqlScalar<int>("INSERT INTO [DynReports]([Name], [Description], [DynReportTypeId], [RegUserId]) VALUES (@1, @2, @3, @4)", "DynReportId",
                report.Name, report.Description, report.TypeId, Session["UserId"].ToString());

            // For adding new reports into user groups if necessary. There is a different approach in Oracle involving RapoarteGrupuriUtilizatori view.
            //General.RunSqlColumn<int>("SELECT DISTINCT [IdGrup] FROM [relGrupUser] WHERE [IdUser] = @1", Session["UserId"])?.ToList().ForEach(groupId =>
            //{
            //    General.RunSqlScalar<int>("INSERT INTO [relGrupRaport2]([IdGrup], [IdRaport]) VALUES (@1, @2)", null, groupId, reportId);
            //});
        }

        public void SetReport(ReportViewModel report)
        {
            General.RunSqlScalar<int>("UPDATE [DynReports] SET [Name] = @1, [Description] = @2, [DynReportTypeId] = @3 WHERE [DynReportId] = @4", null,
                report.Name, report.Description, report.TypeId, report.Id);
        }

        public void DelReport(ReportViewModel report)
        {
            General.RunSqlScalar<int>("DELETE FROM [DynReports] WHERE [DynReportId] = @1", null, report.Id);
            // For removing reports from user groups if necessary. There is a different approach in Oracle involving RapoarteGrupuriUtilizatori view.
            //General.RunSqlScalar<int>("DELETE FROM [relGrupRaport2] WHERE [IdRaport] = @1", null, report.Id);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Dami.AccesApp();

            #region Traducere
            string ctlPost = Request.Params["__EVENTTARGET"];

            if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0)
                Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

            TitleLabel.Text = Dami.TraduCuvant("Modifica sau creaza rapoarte noi");
            ReportNewButton.Text = Dami.TraduCuvant("ReportNewButton", "Raport nou");
            ReportViewButton.Text = Dami.TraduCuvant("ReportViewButton", "Afisare");
            ReportDesignButton.Text = Dami.TraduCuvant("ReportDesignButton", "Design");
            ExitButton.Text = Dami.TraduCuvant("ExitButton", "Iesire");            

            foreach (var col in ReportsGridView.Columns.OfType<GridViewDataColumn>())
            {                
                col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);                
            }
            #endregion

            if ((Session["EsteAdmin"] ?? "0").ToString() == "0")
                ReportsGridView.Columns[0].Visible = false;

            ReportsGridView.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaRap", "10"));            
        }
        
        protected void ReportsGridView_DataBinding(object sender, EventArgs e)
        {
            (sender as ASPxGridView).ForceDataRowType(typeof(ReportViewModel));
            ReportsGridView.FocusedRowIndex = -1;
        }

        protected void ReportViewButton_Click(object sender, EventArgs e)
        {
            var selectedValues = ReportsGridView.GetSelectedFieldValues(new string[] { "Id" });

            if (selectedValues.Count > 0)
            {
                try
                {
                    var reportSettings = General.RunSqlSingle<ReportSettingsViewModel>(
                        "SELECT [ExtensiiPermise] AS [ExportOptions], [MeniuRestrans] AS [ToolbarType] " +
                        "FROM [RapoarteGrupuriUtilizatori] " +
                        "WHERE [IdRaport] = @1 AND [IdUser] = @2", selectedValues[0], Session["UserId"]);

                    if (reportSettings != null)
                        ReportProxy.View((int)selectedValues[0], reportSettings.ToolbarType, reportSettings.ExportOptions);
                    else
                        ReportProxy.View((int)selectedValues[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                }
            }            
        }

        protected void ReportDesignButton_Click(object sender, EventArgs e)
        {
            var selectedValues = ReportsGridView.GetSelectedFieldValues(new string[] { "Id" });

            if (selectedValues.Count > 0)
            {
                try
                {
                    ReportProxy.Design((int)selectedValues[0]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex, MessageBox.icoError, "Atentie !");
                    General.MemoreazaEroarea(ex, Path.GetFileName(Page.AppRelativeVirtualPath), new StackTrace().GetFrame(0).GetMethod().Name);
                }
            }
        }     
    }
}