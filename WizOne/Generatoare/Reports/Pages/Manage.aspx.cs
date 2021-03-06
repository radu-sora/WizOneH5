using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
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
            public int ModuleId { get; set; }
            public bool Restricted { get; set; }
            public byte[] Photo { get; set; }
        }        

        public class ReportSettingsViewModel
        {            
            public string ExportOptions { get; set; }
            public short ToolbarType { get; set; }
        }
        
        public static List<ReportViewModel> GetReports(short type = 1)
        {            
            var reports = General.RunSqlQuery<ReportViewModel>(
                "SELECT DISTINCT r.[DynReportId] AS [Id], r.[Name], r.[Description], r.[DynReportTypeId] AS [TypeId], det.[IdModul] AS [ModuleId], det.[AreParola] AS Restricted, r.[Photo] AS Photo " +
                "FROM [DynReports] r " +
                "INNER JOIN [RapoarteGrupuriUtilizatori] rgu ON r.[DynReportId] = rgu.[IdRaport] AND rgu.[IdUser] = @1 " +
                "LEFT JOIN [tblRapoarteDetalii] det ON r.[DynReportId] = det.[IdRaport] " + 
               $"WHERE r.[DynReportTypeId] {(type == 1 ? "<>" : "=")} 5 ORDER BY Id", HttpContext.Current.Session["UserId"]);  //Radu 15.07.2020 && 13.10.2021

            return reports;
        }

        public void AddReport(ReportViewModel report)
        {           
            //Florin 2020.04.09 - #329 - GitHub
            var reportId = General.RunSqlScalar<int>("INSERT INTO [DynReports]([Name], [Description], [DynReportTypeId], [RegUserId], [Photo]) VALUES (@1, @2, @3, @4, CONVERT(VARBINARY(MAX), @5))", "DynReportId",
                report.Name, report.Description, (report.TypeId != 0 ? report.TypeId : 5), Session["UserId"].ToString(), report.Photo); 

            //Radu 15.07.2020
            General.RunSqlScalar<int>($"INSERT INTO [tblRapoarteDetalii] ([IdRaport], [IdModul], [AreParola], [USER_NO], [TIME]) VALUES (@1, @2, @3, @4, @5)", null,
                                reportId, report.ModuleId, report.Restricted, Session["UserId"], DateTime.Now);
        }

        public void SetReport(ReportViewModel report)
        {            
            General.RunSqlScalar<int>("UPDATE [DynReports] SET [Name] = @1, [Description] = @2, [DynReportTypeId] = @3, [Photo] = @5 WHERE [DynReportId] = @4", null,
                report.Name, report.Description, (report.TypeId != 0 ? report.TypeId : 5), report.Id, report.Photo);            
            // For updating existing reports from user groups if necessary.
            General.RunSqlColumn<int>(
                "SELECT DISTINCT [IdGrup] FROM [relGrupUser] WHERE [IdUser] = @1 " +
                "UNION " +
                "SELECT DISTINCT [IdGrup] FROM [relGrupRaport2] WHERE [IdRaport] = @2", Session["UserId"], report.Id).ForEach(groupId =>
            {
                //Radu 15.07.2020
                //if (General.RunSqlScalar<int>($"UPDATE [relGrupRaport2] SET [AreParola] = @1 WHERE [IdGrup] = @2 AND [IdRaport] = @3", null, report.Restricted, groupId, report.Id) == 0)
                //    General.RunSqlScalar<int>($"INSERT INTO [relGrupRaport2] ([IdGrup], [IdRaport], [AreParola], [USER_NO], [TIME]) VALUES (@1, @2, @3, @4, @5)", null, 
                //       groupId, report.Id, report.Restricted, Session["UserId"], DateTime.Now);
                if (General.RunSqlScalar<int>($"UPDATE [tblRapoarteDetalii] SET [IdModul] = @1, [AreParola] = @2 WHERE [IdRaport] = @3", null, report.ModuleId, report.Restricted, report.Id) == 0)
                    General.RunSqlScalar<int>($"INSERT INTO [tblRapoarteDetalii] ([IdRaport], [IdModul], [AreParola], [USER_NO], [TIME]) VALUES (@1, @2, @3, @4, @5)", null, 
                       report.Id, report.ModuleId, report.Restricted, Session["UserId"], DateTime.Now);
            });            
        }

        public void DelReport(ReportViewModel report)
        {
            General.RunSqlScalar<int>("DELETE FROM [DynReports] WHERE [DynReportId] = @1", null, report.Id);
            // For removing reports from user groups if necessary.
            //Radu 15.07.2020
            //General.RunSqlScalar<int>($"DELETE FROM [relGrupRaport2] WHERE [IdRaport] = @1", null, report.Id);            
            General.RunSqlScalar<int>("DELETE FROM [tblRapoarteDetalii] WHERE [IdRaport] = @1", null, report.Id);
        }

        public static ReportSettingsViewModel GetReportSettings(int reportId)
        {
            return General.RunSqlSingle<ReportSettingsViewModel>(
                "SELECT [ExtensiiPermise] AS [ExportOptions], [MeniuRestrans] AS [ToolbarType] " +
                "FROM [RapoarteGrupuriUtilizatori] " +
                "WHERE [IdRaport] = @1 AND [IdUser] = @2", reportId, HttpContext.Current.Session["UserId"]) ?? new ReportSettingsViewModel
                {
                    ExportOptions = "*",
                    ToolbarType = 0 // For brevity
                };
        }

        private bool IsMobileDevice
        {
            get
            {
                var userAgent = Request.ServerVariables["HTTP_USER_AGENT"];
                var devices = new string[] { "iPhone", "iPad", "Android", "Windows Phone" }; // Add more devices

                return devices.Any(d => userAgent.Contains(d));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Dami.AccesApp(this.Page);            

            if (!IsPostBack)
            {
                #region Traducere
                string ctlPost = Request.Params["__EVENTTARGET"];

                if (!string.IsNullOrEmpty(ctlPost) && ctlPost.IndexOf("LangSelectorPopup") >= 0)
                    Session["IdLimba"] = ctlPost.Substring(ctlPost.LastIndexOf("$") + 1).Replace("a", "");

                TitleLabel.Text = IsMobileDevice ? Dami.TraduCuvant("Lista rapoarte") : Dami.TraduCuvant("Modifica sau creaza rapoarte noi");
                ReportNewButton.Text = Dami.TraduCuvant("ReportNewButton", "Raport nou");
                ReportViewButton.Text = Dami.TraduCuvant("ReportViewButton", "Afisare");
                ReportDesignButton.Text = Dami.TraduCuvant("ReportDesignButton", "Design");
                ExitButton.Text = Dami.TraduCuvant("ExitButton", "Iesire");

                foreach (var col in ReportsGridView.Columns.OfType<GridViewDataColumn>())
                    col.Caption = Dami.TraduCuvant(col.FieldName ?? col.Caption, col.Caption);
                #endregion

                //Radu 14.07.2020
                ReportsGridView.SettingsCookies.Enabled = true;
                ReportsGridView.SettingsCookies.StoreFiltering = true;
                ReportsGridView.SettingsCookies.CookiesID = "ReportsGridViewCookies";

                ReportsGridView.SettingsPager.PageSize = Convert.ToInt32(Dami.ValoareParam("NrRanduriPePaginaRap", "10"));
                if (General.VarSession("EsteAdmin").ToString() == "0") Dami.Securitate(ReportsGridView);

                //Radu 16.09.2020
                if (Dami.ValoareParam("Rap_GrupareImplicitaModule", "0") == "1")
                    ReportsGridView.GroupBy(ReportsGridView.Columns["IdModul"]);
            }

            //Radu 15.07.2020
            string sql = @"SELECT * FROM tblModule ORDER BY Denumire";
            if (Constante.tipBD == 2)
                sql = General.SelectOracle("tblModule", "Id") + " ORDER BY \"Denumire\"";
            DataTable dtModul = General.IncarcaDT(sql, null);
            GridViewDataComboBoxColumn colModul = (ReportsGridView.Columns["ModuleId"] as GridViewDataComboBoxColumn);
            colModul.PropertiesComboBox.DataSource = dtModul;

            CardViewComboBoxColumn colModul2 = (ReportsCardView.Columns["ModuleId"] as CardViewComboBoxColumn);
            colModul2.PropertiesComboBox.DataSource = dtModul;

            //Radu 13.10.2021 - #1016
            // tip = 1  fara Dashboards
            // tip = 2  numai Dashboards        
            var type = Convert.ToInt16(General.Nz(Request["tip"], 1));

            //Radu 19.11.2021 - #1053
            if (type == 2)
            {
                //ReportsGridView.ClientVisible = false;
                Panel1.Visible = false;
                ReportsCardView.SettingsEditing.Mode = CardViewEditingMode.EditForm;               
                CardViewColumn colModulCV = (ReportsCardView.Columns["ModuleId"] as CardViewColumn);
                ReportsCardView.GroupBy(colModulCV);
                ReportNewButton.ClientVisible = false;
            }
            else
            {
                //ReportsCardView.ClientVisible = false;
                Panel2.Visible = false;
                CardNewButton.ClientVisible = false;
                (ReportsGridView.Columns["TypeId"] as GridViewDataComboBoxColumn).PropertiesComboBox.DataSource = General.IncarcaDT(
                    $@"SELECT DynReportTypeId AS Id, Name FROM DynReportTypes WHERE DynReportTypeId <> 5", null);
            }
        }

        protected void ReportsGridView_DataBinding(object sender, EventArgs e)
        {
            (sender as ASPxGridView).ForceDataRowType(typeof(ReportViewModel));
            ReportsGridView.FocusedRowIndex = -1;
        }

        //Radu 19.11.2021 - #1053
        protected void ReportsCardView_DataBinding(object sender, EventArgs e)
        {
            (sender as ASPxCardView).ForceDataRowType(typeof(ReportViewModel));           
        }

        protected void ReportViewButton_Click(object sender, EventArgs e)
        {
            var type = Convert.ToInt16(General.Nz(Request["tip"], 1));

            var selectedValues = (type == 1 ? ReportsGridView.GetSelectedFieldValues(new string[] { "Id" }) : ReportsCardView.GetSelectedFieldValues(new string[] { "Id" }));

            if (selectedValues.Count > 0)
            {
                try
                {
                    var reportSettings = GetReportSettings((int)selectedValues[0]);
                    
                    ReportProxy.View((int)selectedValues[0], reportSettings.ToolbarType, reportSettings.ExportOptions);
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
            var type = Convert.ToInt16(General.Nz(Request["tip"], 1));

            var selectedValues = (type == 1 ? ReportsGridView.GetSelectedFieldValues(new string[] { "Id" }) : ReportsCardView.GetSelectedFieldValues(new string[] { "Id" }));

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