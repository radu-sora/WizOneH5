using DevExpress.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Wizrom.Reports.Code;
using Wizrom.Reports.Models;

namespace Wizrom.Reports.Pages
{
    public partial class View : ReportSessionPage
    {
        private int _reportId
        {
            get { return ReportSession.ReportId; }
        }
        private string _userId
        {
            get { return ReportSession.UserId; }
        }
        private int _reportUserId
        {
            get { return ReportSession.DataCache.ReportUserId; }
            set { ReportSession.DataCache.ReportUserId = value; }
        }
        private dynamic _reportParams
        {
            get { return ReportSession.DataCache.ReportParams; }
            set { ReportSession.DataCache.ReportParams = value; }
        }
        private dynamic _chartOptions
        {
            get { return ReportSession.DataCache.ChartOptions; }
            set { ReportSession.DataCache.ChartOptions = value; }
        }
        private XtraReport _report
        {
            get { return ReportSession.DataCache.Report; }
        }
        private XRPivotGrid _pivotGrid
        {
            get { return _report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRPivotGrid>().FirstOrDefault(); }
        }
        private XRChart _chart
        {
            get { return _report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRChart>().FirstOrDefault(); }
        }
        private XRRichText _richText
        {
            get { return _report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRRichText>().FirstOrDefault(); }
        }        

        // For client side customization
        protected string ReportName
        {
            get; private set;
        }
        protected short ReportType
        {
            get; private set;
        }
        protected short ToolbarType
        {
            get { return ReportSession.ToolbarType; }
        }
        protected string ExportOptions
        {
            get { return ReportSession.ExportOptions; }
        }
        protected short ChartStatus // 0 - None, 1 - Hidden, 2 - Visible
        {
            get { return (short)(_chartOptions != null ? ((bool)_chartOptions.Options.O5 ? 2 : 1) : 0); }
        }

        private void LoadASPxPivotGridLayoutFromXRPivotGrid(ASPxPivotGrid aspxPivotGrid, XRPivotGrid xrPivotGrid)
        {
            if (aspxPivotGrid != null && xrPivotGrid != null)
            {
                using (var objectMemStream = new MemoryStream())
                {
                    var xmlXtraSerializer = new XmlXtraSerializer();

                    xmlXtraSerializer.SerializeObject(xrPivotGrid, objectMemStream, "PivotGrid");
                    objectMemStream.Position = 0;

                    using (var collapsedMemStream = new MemoryStream())
                    {
                        (xrPivotGrid as IPivotGridDataContainer).Data.SaveCollapsedStateToStream(collapsedMemStream);
                        collapsedMemStream.Position = 0;

                        xmlXtraSerializer.DeserializeObject(aspxPivotGrid, objectMemStream, "PivotGrid");                        
                        aspxPivotGrid.Data.LoadCollapsedStateFromStream(collapsedMemStream);
                        aspxPivotGrid.Data.LayoutChanged();

                        // This is reseted to default after DeserializeObject.
                        // xrPivotGrid.OptionsLayout.ResetOptions & aspxPivotGrid.OptionsLayout.ResetOptions has no effect.
                        aspxPivotGrid.OptionsCustomization.CustomizationFormStyle = CustomizationFormStyle.Excel2007; 
                    }
                }
            }
        }

        private void LoadXRPivotGridLayoutFromASPxPivotGrid(XRPivotGrid xrPivotGrid, ASPxPivotGrid aspxPivotGrid)
        {
            if (xrPivotGrid != null && aspxPivotGrid != null)
            {
                using (var objectMemStream = new MemoryStream())
                {
                    var xmlXtraSerializer = new XmlXtraSerializer();

                    xmlXtraSerializer.SerializeObject(aspxPivotGrid, objectMemStream, "PivotGrid");
                    objectMemStream.Position = 0;

                    using (var collapsedMemStream = new MemoryStream())
                    {
                        aspxPivotGrid.Data.SaveCollapsedStateToStream(collapsedMemStream);
                        collapsedMemStream.Position = 0;

                        xmlXtraSerializer.DeserializeObject(xrPivotGrid, objectMemStream, "PivotGrid");
                        (xrPivotGrid as IPivotGridDataContainer).Data.LoadCollapsedStateFromStream(collapsedMemStream);
                        (xrPivotGrid as IPivotGridDataContainer).Data.LayoutChanged();
                    }
                }
            }
        }

        private string SaveXRChartOptions(XRChart xrChart, XRPivotGrid xrPivotGrid)
        {
            if (xrChart != null && xrPivotGrid != null)
            {
                return JsonConvert.SerializeObject(new
                {
                    Type = (int)SeriesViewFactory.GetViewType(xrChart.SeriesTemplate.View),
                    Options = new
                    {
                        O1 = (xrChart.SeriesTemplate.LabelsVisibility == DefaultBoolean.True),
                        O2 = xrPivotGrid.OptionsChartDataSource.ProvideDataByColumns,
                        O3 = xrPivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals,
                        O4 = xrPivotGrid.OptionsChartDataSource.ProvideRowGrandTotals,
                        O5 = xrChart.Visible
                    }
                });
            }

            return null;
        }

        private void LoadWebChartOptions(WebChartControl webChart, ASPxPivotGrid aspxPivotGrid, dynamic chartOptions)
        {
            if (chartOptions != null && (chartOptions as JObject).HasValues)
            {
                webChart.SeriesTemplate.ChangeView((ViewType)chartOptions.Type);
                webChart.SeriesTemplate.LabelsVisibility = (bool)chartOptions.Options.O1 ? DefaultBoolean.True : DefaultBoolean.False;
                aspxPivotGrid.OptionsChartDataSource.ProvideDataByColumns = chartOptions.Options.O2;
                aspxPivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals = chartOptions.Options.O3;
                aspxPivotGrid.OptionsChartDataSource.ProvideRowGrandTotals = chartOptions.Options.O4;
            }
        }

        private void LoadXRChartOptions(XRChart xrChart, ASPxPivotGrid aspxPivotGrid, dynamic chartOptions)
        {
            if (chartOptions != null && (chartOptions as JObject).HasValues)
            {
                if (xrChart != null)
                {
                    xrChart.SeriesTemplate.ChangeView((ViewType)chartOptions.Type);
                    xrChart.SeriesTemplate.LabelsVisibility = (bool)chartOptions.Options.O1 ? DefaultBoolean.True : DefaultBoolean.False;
                    xrChart.Visible = (bool)chartOptions.Options.O5;
                }

                aspxPivotGrid.OptionsChartDataSource.ProvideDataByColumns = chartOptions.Options.O2;
                aspxPivotGrid.OptionsChartDataSource.ProvideColumnGrandTotals = chartOptions.Options.O3;
                aspxPivotGrid.OptionsChartDataSource.ProvideRowGrandTotals = chartOptions.Options.O4;
            }
        }

        private string SaveASPxGridViewLayout(ASPxGridView aspxGridView)
        {
            var contextMenuOptions = new Dictionary<string, List<int>>();

            aspxGridView.Columns.OfType<GridViewDataColumn>().ToList().ForEach(col =>
            {
                var groupInterval = col.Settings.GroupInterval;

                if (groupInterval == ColumnGroupInterval.Default)
                {
                    if (col.GetType() == typeof(GridViewDataDateColumn))
                        groupInterval = ColumnGroupInterval.Date;
                    else
                        groupInterval = ColumnGroupInterval.Value;
                }
                                                
                contextMenuOptions.GetValue("GroupByColumn:" + (int)groupInterval).Add(col.Index);

                if (col.Settings.AllowHeaderFilter == DefaultBoolean.True)
                    contextMenuOptions.GetValue("HeaderFilter").Add(col.Index);

                var headerFilterMode = col.SettingsHeaderFilter.Mode;

                if (headerFilterMode == GridHeaderFilterMode.Default)
                {
                    if (col.GetType() == typeof(GridViewDataDateColumn))
                        headerFilterMode = GridHeaderFilterMode.DateRangePicker;
                    else
                        headerFilterMode = GridHeaderFilterMode.List;
                }
                
                contextMenuOptions.GetValue("HeaderFilterMode:" + (int)headerFilterMode).Add(col.Index);

                if (col.Settings.AllowCellMerge == DefaultBoolean.True)
                    contextMenuOptions.GetValue("CellMerge").Add(col.Index);

                if (col.FixedStyle == GridViewColumnFixedStyle.Left)
                    contextMenuOptions.GetValue("FixedColumn").Add(col.Index);

            });

            return JsonConvert.SerializeObject(new
            {
                SearchPanel = aspxGridView.SettingsSearchPanel.Visible,
                GroupPanel = aspxGridView.Settings.ShowGroupPanel,
                FilterRow = aspxGridView.Settings.ShowFilterRow,
                FilterRowMenu = aspxGridView.Settings.ShowFilterRowMenu,
                FilterRowMode = aspxGridView.SettingsBehavior.FilterRowMode,
                FixedGroups = aspxGridView.SettingsBehavior.AllowFixedGroups,
                MergeGroups = aspxGridView.SettingsBehavior.MergeGroupsMode,
                GroupFooter = aspxGridView.Settings.ShowGroupFooter,
                AlternatingRowColor = aspxGridView.Styles.AlternatingRow.Enabled,
                GridLines = aspxGridView.Settings.GridLines,
                Footer = aspxGridView.Settings.ShowFooter,
                PapeSize = aspxGridView.SettingsPager.PageSize,
                Layout = aspxGridView.SaveClientLayout(),
                ContextMenuOptions = contextMenuOptions,
                GroupSummary = aspxGridView.GroupSummary.Select(si => new { si.FieldName, si.SummaryType, si.ShowInGroupFooterColumn }).ToList(),
                TotalSummary = aspxGridView.TotalSummary.Select(si => new { si.FieldName, si.SummaryType }).ToList()
            });
        }

        private void LoadASPxGridViewLayout(ASPxGridView aspxGridView, object layout, bool partial)
        {
            var contextMenuOptions = new Dictionary<string, List<int>>();
            var contextMenuHiddenOptions = new Dictionary<string, List<int>>();

            if (layout != null && (layout as string).Length > 0) // Set new layout
            {
                try
                {
                    dynamic gridLayout = JObject.Parse(layout as string);

                    if (!partial)
                    {
                        aspxGridView.SettingsSearchPanel.Visible = gridLayout.SearchPanel;
                        aspxGridView.Settings.ShowGroupPanel = gridLayout.GroupPanel;
                        aspxGridView.Settings.ShowFilterRow = gridLayout.FilterRow;
                        aspxGridView.Settings.ShowFilterRowMenu = gridLayout.FilterRowMenu;
                        aspxGridView.Settings.ShowFooter = gridLayout.Footer;
                        aspxGridView.LoadClientLayout((string)gridLayout.Layout);
                        aspxGridView.GroupSummary.Clear();
                        aspxGridView.GroupSummary.AddRange((gridLayout.GroupSummary.ToObject<List<dynamic>>() as List<dynamic>).
                            Select(si => new ASPxSummaryItem((string)si.FieldName, (SummaryItemType)(int)si.SummaryType) { ShowInGroupFooterColumn = (string)si.ShowInGroupFooterColumn }).ToList());
                        aspxGridView.TotalSummary.Clear();
                        aspxGridView.TotalSummary.AddRange((gridLayout.TotalSummary.ToObject<List<dynamic>>() as List<dynamic>).
                            Select(si => new ASPxSummaryItem((string)si.FieldName, (SummaryItemType)(int)si.SummaryType)).ToList());
                    }

                    aspxGridView.SettingsBehavior.FilterRowMode = gridLayout.FilterRowMode;
                    aspxGridView.SettingsBehavior.AllowFixedGroups = gridLayout.FixedGroups;
                    aspxGridView.SettingsBehavior.MergeGroupsMode = gridLayout.MergeGroups;
                    aspxGridView.Settings.ShowGroupFooter = gridLayout.GroupFooter;
                    aspxGridView.Styles.AlternatingRow.Enabled = gridLayout.AlternatingRowColor;
                    aspxGridView.Settings.GridLines = gridLayout.GridLines;                    

                    contextMenuOptions = gridLayout.ContextMenuOptions.ToObject<Dictionary<string, List<int>>>() as Dictionary<string, List<int>>;

                    foreach (var option in contextMenuOptions)
                    {
                        if (option.Key.StartsWith("GroupByColumn"))
                        {
                            option.Value.ForEach(col =>
                            {
                                (aspxGridView.Columns[col] as GridViewDataColumn).Settings.GroupInterval = (ColumnGroupInterval)Convert.ToInt32(option.Key.Split(':')[1]);
                            });
                        }

                        if (option.Key.Equals("HeaderFilter"))
                        {
                            option.Value.ForEach(col =>
                            {
                                (aspxGridView.Columns[col] as GridViewDataColumn).Settings.AllowHeaderFilter = DefaultBoolean.True;
                            });
                        }

                        if (option.Key.StartsWith("HeaderFilterMode"))
                        {
                            option.Value.ForEach(col =>
                            {
                                (aspxGridView.Columns[col] as GridViewDataColumn).SettingsHeaderFilter.Mode = (GridHeaderFilterMode)Convert.ToInt32(option.Key.Split(':')[1]);
                            });
                        }

                        if (option.Key.Equals("CellMerge"))
                        {
                            option.Value.ForEach(col =>
                            {
                                (aspxGridView.Columns[col] as GridViewDataColumn).Settings.AllowCellMerge = DefaultBoolean.True;
                            });
                        }

                        if (option.Key.Equals("FixedColumn"))
                        {
                            option.Value.ForEach(col =>
                            {
                                (aspxGridView.Columns[col] as GridViewDataColumn).FixedStyle = GridViewColumnFixedStyle.Left;
                            });
                        }
                    }
                }
                catch { } // In case if layout has an old structure, just ignore it.
            }
            else // Get current layout
            {
                dynamic gridLayout = JObject.Parse(SaveASPxGridViewLayout(aspxGridView));

                contextMenuOptions = gridLayout.ContextMenuOptions.ToObject<Dictionary<string, List<int>>>() as Dictionary<string, List<int>>;
            }

            // For client side customization
            contextMenuOptions["ShowFilterRow:" + (int)aspxGridView.SettingsBehavior.FilterRowMode] = new List<int>() { -1 };

            if (aspxGridView.SettingsBehavior.AllowFixedGroups)
                contextMenuOptions["FixedGroups"] = new List<int>() { -1 };

            if (aspxGridView.SettingsBehavior.MergeGroupsMode == GridViewMergeGroupsMode.Always)
                contextMenuOptions["MergeGroups"] = new List<int>() { -1 };

            contextMenuOptions["GroupFooter:" + (int)aspxGridView.Settings.ShowGroupFooter] = new List<int>() { -1 };

            if (aspxGridView.Styles.AlternatingRow.Enabled == DefaultBoolean.True)
                contextMenuOptions["AlternatingRowColor"] = new List<int>() { -1 };

            contextMenuOptions["GridLines:" + (int)aspxGridView.Settings.GridLines] = new List<int>() { -1 };

            aspxGridView.Columns.OfType<GridViewDataColumn>().ToList().ForEach(col =>
            {
                if (col.GetType() == typeof(GridViewDataDateColumn))
                {
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.Value).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.DisplayText).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.Alphabetical).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("HeaderFilterMode:" + (int)GridHeaderFilterMode.List).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("HeaderFilterMode:" + (int)GridHeaderFilterMode.CheckedList).Add(col.Index);
                }
                else
                {
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.Date).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.DateMonth).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.DateYear).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("GroupByColumn:" + (int)ColumnGroupInterval.DateRange).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("HeaderFilterMode:" + (int)GridHeaderFilterMode.DateRangePicker).Add(col.Index);
                    contextMenuHiddenOptions.GetValue("HeaderFilterMode:" + (int)GridHeaderFilterMode.DateRangeCalendar).Add(col.Index);
                }
            });

            aspxGridView.JSProperties["cpContextMenuOptions"] = contextMenuOptions;
            aspxGridView.JSProperties["cpContextMenuHiddenOptions"] = contextMenuHiddenOptions;
        }

        private string GetCallbackCommandName()
        {
            var data = Request["__CALLBACKPARAM"];

            if (!string.IsNullOrEmpty(data))
            {
                string customCallbackMarker = "#";
                int startIndex = data.IndexOf(customCallbackMarker) + customCallbackMarker.Length;

                if (startIndex >= customCallbackMarker.Length)
                {
                    int stopIndex = data.IndexOf('{', startIndex);

                    if (startIndex > -1 && stopIndex > -1 && startIndex < stopIndex)
                        return data.Substring(startIndex, stopIndex - startIndex);
                }
            }

            return string.Empty;
        }

        private JObject GetCallbackCommandParams(string commandName)
        {
            var data = Request["__CALLBACKPARAM"];

            if (!string.IsNullOrEmpty(data) && !string.IsNullOrEmpty(commandName))
            {
                int startIndex = data.IndexOf(commandName + "{") + commandName.Length;
                int stopIndex = data.LastIndexOf('}') + 1;

                if (startIndex > -1 && stopIndex > -1 && startIndex < stopIndex)
                    return JObject.Parse(data.Substring(startIndex, stopIndex - startIndex));
            }

            return null;
        }

        private bool GetWebChartRefreshState()
        {
            return _chart != null &&
                   !Request["__CALLBACKPARAM"].Contains(":FS|") &&
                   !Request["__CALLBACKPARAM"].Contains(":PREFILTER|Show") &&
                   !Request["__CALLBACKPARAM"].Contains(":PREFILTER|Hide"); // Native callbacks that do not affect chart display
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
            try
            {
                if (!IsPostBack)
                {
                    // Load data
                    if (_reportId == 0) // TODO: Evaluate this and _userId in all places.
                        throw new Exception("No report id found");

                    var entities = new ReportsEntities();
                    var report = entities.Reports.Find(_reportId);

                    if (report == null)
                        throw new Exception($"No report found for id {_reportId}");

                    if (report.LayoutData == null)
                        throw new Exception($"No layout found for report id {_reportId}");

                    if (report.ReportTypeId != 5) // Report based template
                    {
                        using (var memStream = new MemoryStream(report.LayoutData))
                            _report.LoadLayoutFromXml(memStream);

                        // Set internal params            
                        var values = ReportSession.ParamList;
                        var implicitValues = values.Implicit.GetType().GetProperties() as PropertyInfo[];
                        var explicitValues = values.Explicit?.GetType().GetProperties() as PropertyInfo[];
                        var parameters = _report.ObjectStorage.OfType<SqlDataSource>().
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

                        // For client side customization
                        ReportName = report.Name;
                        ReportType = report.ReportTypeId;

                        // Init controls
                        DashboardTemplate.Visible = false;

                        if (report.ReportTypeId == 3) // Cube
                        {
                            if (_pivotGrid != null)
                            {
                                ReportsUsersDataSource.WhereParameters["ReportId"].DefaultValue = _reportId.ToString();
                                ReportsUsersDataSource.WhereParameters["RegUserId"].DefaultValue = _userId;

                                if (_chart != null)
                                {
                                    // Init chart options
                                    _chartOptions = JObject.Parse(SaveXRChartOptions(_chart, _pivotGrid));

                                    ChartTypeComboBox.Items.AddRange(Enum.GetValues(typeof(ViewType)).OfType<ViewType>().
                                        Select(vt => new ListEditItem() { Value = (int)vt, Text = vt.ToString() }).ToList());
                                    ChartTypeComboBox.Value = (int)_chartOptions.Type;

                                    ChartOptionsCheckBoxList.Items.Add("Show Point Labels", "O1").Selected = _chartOptions.Options.O1;
                                    ChartOptionsCheckBoxList.Items.Add("Generate Series from Columns", "O2").Selected = _chartOptions.Options.O2;
                                    ChartOptionsCheckBoxList.Items.Add("Show Column Grand Totals", "O3").Selected = _chartOptions.Options.O3;
                                    ChartOptionsCheckBoxList.Items.Add("Show Row Grand Totals", "O4").Selected = _chartOptions.Options.O4;
                                    ChartOptionsCheckBoxList.Items.Add("Show Chart", "O5").Selected = _chartOptions.Options.O5;

                                    // Init chart control
                                    LoadWebChartOptions(CustomCubeWebChartControl, CustomCubePivotGrid, _chartOptions);
                                }

                                if (_report.Parameters.Count == 0)
                                {
                                    _report.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService()); // Temp fix only for FillDataSource here
                                    _report.FillDataSource();
                                }

                                if (_pivotGrid.Fields.Count == 0)
                                    _pivotGrid.RetrieveFields(); // Retrieve all data source fields into filter area by default. Can be customized later.

                                LoadASPxPivotGridLayoutFromXRPivotGrid(CustomCubePivotGrid, _pivotGrid);

                                CustomCubePivotGrid.DataSource = _pivotGrid.DataSource;
                                CustomCubePivotGrid.DataMember = _pivotGrid.DataMember;
                            }
                        }
                        else if (report.ReportTypeId == 4) // Table
                        {
                            if (_richText != null)
                            {
                                ReportsUsersDataSource.WhereParameters["ReportId"].DefaultValue = _reportId.ToString();
                                ReportsUsersDataSource.WhereParameters["RegUserId"].DefaultValue = _userId;

                                if (_report.Parameters.Count == 0)
                                {
                                    _report.PrintingSystem.AddService(typeof(IConnectionProviderService), new ReportConnectionProviderService()); // Temp fix only for FillDataSource here
                                    _report.FillDataSource();
                                }

                                CustomTableGridView.DataSource = _report.DataSource;
                                CustomTableGridView.DataMember = _report.DataMember;
                                CustomTableGridView.DataBind();

                                LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, false);
                            }
                        }

                        // Customize report viewer UI
                        WebDocumentViewer.MobileMode = report.ReportTypeId <= 2 ? IsMobileDevice : false; // Mobile mode only for Report & Document type
                        WebDocumentViewer.MenuItems.Add(new WebDocumentViewerMenuItem()
                        {
                            Text = "Exit",
                            ImageClassName = WebDocumentViewer.MobileMode ? "dxrd-image-exit mobile" : "dxrd-image-exit",
                            JSClickAction = "function() { onExitButtonClick(); }",
                            Container = MenuItemContainer.Toolbar
                        });

                        // Open the report
                        WebDocumentViewer.OpenReport(_report);
                    }
                    else // Dashboard template
                    {
                        // Set internal params
                        // They are set into EntityDashboardStorage.

                        // For client side customization
                        // ...

                        // Init controls
                        ReportTemplate.Visible = false;

                        // Customize dashboard viewer UI
                        // ...                        

                        // Open the dashboard                        
                        DashboardViewer.InitialDashboardId = Request.QueryString["id"];
                    }
                }                
                else if (WebDocumentViewerCallbackPanel.IsCallback)
                {
                    if (_report.Name == string.Empty)
                        throw new Exception("No report found in this session");

                    var report = new XtraReport();

                    using (var memStream = new MemoryStream())
                    {
                        _report.SaveLayoutToXml(memStream);
                        memStream.Position = 0;
                        report.LoadLayoutFromXml(memStream);
                    }

                    report.DataSource = _report.DataSource;
                    report.DataMember = _report.DataMember;

                    // Customize report viewer UI
                    WebDocumentViewer.MenuItems.Add(new WebDocumentViewerMenuItem()
                    {
                        Text = "Customize layout",
                        ImageClassName = "dxrd-image-run-wizard",
                        JSClickAction = "function() { onCustomLayoutShowButtonClick(); }",
                        Container = MenuItemContainer.Toolbar
                    });

                    WebDocumentViewer.MenuItems.Add(new WebDocumentViewerMenuItem()
                    {
                        Text = "Exit",
                        ImageClassName = "dxrd-image-exit",
                        JSClickAction = "function() { onExitButtonClick(); }",
                        Container = MenuItemContainer.Toolbar
                    });

                    // Open the report                    
                    WebDocumentViewer.OpenReport(report);
                }
                else if (CustomCubePivotGrid.IsCallback || CustomCubePivotGrid.IsPrefilterPopupVisible)
                {
                    if (_pivotGrid != null)
                    {
                        var commandName = GetCallbackCommandName();
                        var commandParams = GetCallbackCommandParams(commandName) as dynamic;

                        // Save params
                        if (commandName == "init")
                            _reportUserId = (int)commandParams.ReportUserId;

                        if (commandName == "load")
                            _reportParams = commandParams.ReportParams;

                        var entities = new ReportsEntities();

                        // Process command (first part)
                        if (commandName == "save")
                        {
                            try
                            {
                                if (commandParams.LayoutName != null) // New report user layout
                                {
                                    if (_reportId == 0)
                                        throw new Exception("No report id found");

                                    if (_userId == "0")
                                        throw new Exception("No user id found");

                                    var reportUser = new ReportUser()
                                    {
                                        ReportId = _reportId,
                                        Name = commandParams.LayoutName,
                                        RegUserId = _userId
                                    };

                                    // Set chart/grid options
                                    LoadXRChartOptions(_chart, CustomCubePivotGrid, _chartOptions);
                                    LoadXRPivotGridLayoutFromASPxPivotGrid(_pivotGrid, CustomCubePivotGrid);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        reportUser.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.ReportsUsers.Add(reportUser);
                                    entities.SaveChanges();
                                    CustomCubePivotGrid.JSProperties["cpLayoutSaved"] = true;
                                    CustomCubePivotGrid.JSProperties["cpNewReportUserId"] = reportUser.ReportUserId;
                                    _reportUserId = reportUser.ReportUserId;
                                }
                                else if (_reportUserId == 0) // Save to report layout
                                {
                                    if (_reportId == 0)
                                        throw new Exception("No report id found");

                                    var report = entities.Reports.Find(_reportId);

                                    // Set chart/grid options                                    
                                    LoadXRChartOptions(_chart, CustomCubePivotGrid, _chartOptions);
                                    LoadXRPivotGridLayoutFromASPxPivotGrid(_pivotGrid, CustomCubePivotGrid);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        report.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.SaveChanges();
                                    CustomCubePivotGrid.JSProperties["cpLayoutSaved"] = true;
                                }
                                else // Save to report user layout
                                {
                                    var reportUser = entities.ReportsUsers.Find(_reportUserId);

                                    // Set chart/grid options                                    
                                    LoadXRChartOptions(_chart, CustomCubePivotGrid, _chartOptions);
                                    LoadXRPivotGridLayoutFromASPxPivotGrid(_pivotGrid, CustomCubePivotGrid);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        reportUser.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.SaveChanges();
                                    CustomCubePivotGrid.JSProperties["cpLayoutSaved"] = true;
                                }
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unsaved only                            
                            }
                        }
                        else if (commandName == "delete")
                        {
                            try
                            {
                                if (_reportUserId == 0)
                                    throw new Exception($"No layout found for report user id {_reportUserId}");

                                var reportUser = entities.ReportsUsers.Find(_reportUserId);

                                if (reportUser != null)
                                {
                                    entities.ReportsUsers.Remove(reportUser);
                                    entities.SaveChanges();
                                    CustomCubePivotGrid.JSProperties["cpLayoutDeleted"] = true;
                                }
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as undeleted only                                                        
                            }
                        }
                        else if (commandName == "menu")
                        {
                            if (commandParams.Option == "SummaryType")
                                CustomCubePivotGrid.Fields[(string)commandParams.FieldId].SummaryType = (PivotSummaryType)commandParams.Value;
                        }
                        else if (commandName == "print")
                        {
                            try
                            {
                                // Set chart/grid options                                
                                LoadXRChartOptions(_chart, CustomCubePivotGrid, _chartOptions);
                                LoadXRPivotGridLayoutFromASPxPivotGrid(_pivotGrid, CustomCubePivotGrid);
                                CustomCubePivotGrid.JSProperties["cpLayoutPrinted"] = true;
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unprinted only                            
                            }
                        }

                        if (commandName != "delete")
                        {
                            // Load layout from XRPivotGrid if available or set data source
                            if (commandName == "init")
                            {
                                var layoutData = _reportUserId == 0 ? // Base layout
                                    entities.Reports.Find(_reportId)?.LayoutData : // Load base layout
                                    entities.ReportsUsers.Find(_reportUserId)?.LayoutData; // Load custom user layout

                                if (layoutData == null)
                                    throw new Exception($"No layout found for report id {_reportId} or report user id {_reportUserId}");

                                var report = new XtraReport();

                                using (var memStream = new MemoryStream(layoutData))
                                    report.LoadLayoutFromXml(memStream);

                                var pivotGrid = report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRPivotGrid>().FirstOrDefault();
                                var chart = report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRChart>().FirstOrDefault();

                                // Return chart options on client for chart control & chart options controls init
                                CustomCubePivotGrid.JSProperties["cpChartOptions"] = SaveXRChartOptions(chart, pivotGrid);

                                pivotGrid.DataSource = _pivotGrid.DataSource;

                                if (pivotGrid.Fields.Count == 0)
                                    pivotGrid.RetrieveFields(); // Retrieve all data source fields into filter area by default. Can be customized later.

                                LoadASPxPivotGridLayoutFromXRPivotGrid(CustomCubePivotGrid, pivotGrid);
                            }
                            else if (commandName == "load")
                            {
                                // Set UI params & load data                                
                                if (_report.Parameters.Count > 0)
                                {
                                    if (_reportParams != null)
                                    {
                                        var oldParams = _report.Parameters.OfType<DevExpress.XtraReports.Parameters.Parameter>().ToDictionary(p => p.Name, p => p.Value);

                                        foreach (var param in _report.Parameters)
                                            param.Value = _reportParams[param.Name].Value;

                                        _report.FillDataSource();

                                        // Restore default params values (for reset params button after print mode)
                                        foreach (var param in _report.Parameters)
                                            param.Value = oldParams[param.Name];
                                    }
                                }
                                else
                                    _report.FillDataSource();
                            }

                            // For save, menu, print, export and all native callbacks
                            CustomCubePivotGrid.DataSource = _pivotGrid.DataSource;
                            CustomCubePivotGrid.DataMember = _pivotGrid.DataMember;

                            // Refresh chart control
                            CustomCubePivotGrid.JSProperties["cpRefreshChart"] = GetWebChartRefreshState();
                        }

                        // Process command (second part - layout & data dependent)
                        if (commandName == "export")
                        {
                            try
                            {
                                string fileName = string.Empty;

                                CustomCubePivotGridExporter.OptionsPrint.PageSettings.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);

                                switch ((int)commandParams.Type)
                                {
                                    case 0:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.pdf");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomCubePivotGridExporter.ExportToPdf(fileStream);
                                        }
                                        break;
                                    case 1:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.xls");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomCubePivotGridExporter.ExportToXls(fileStream);
                                        }
                                        break;
                                    case 2:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.xlsx");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomCubePivotGridExporter.ExportToXlsx(fileStream);
                                        }
                                        break;
                                    case 3:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.rtf");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomCubePivotGridExporter.ExportToRtf(fileStream);
                                        }
                                        break;
                                    case 4:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.csv");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomCubePivotGridExporter.ExportToCsv(fileStream);
                                        }
                                        break;
                                }

                                if (fileName.Length > 0)
                                    CustomCubePivotGrid.JSProperties["cpLayoutExportedTo"] = Path.GetFileName(fileName);
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unexported only
                            }
                        }
                    }
                }
                else if (CustomCubeWebChartControl.IsCallback)
                {
                    var commandName = GetCallbackCommandName();
                    var commandParams = GetCallbackCommandParams(commandName) as dynamic;

                    // Save params
                    if (commandName == "options")
                        _chartOptions = commandParams.ChartOptions;

                    // Set chart options
                    LoadWebChartOptions(CustomCubeWebChartControl, CustomCubePivotGrid, _chartOptions);

                    // Set pivot grid data for chart control
                    CustomCubePivotGrid.DataSource = _pivotGrid.DataSource;
                    CustomCubePivotGrid.DataMember = _pivotGrid.DataMember;
                }
                else if (CustomTableGridView.IsCallback)
                {
                    if (_richText != null)
                    {
                        var commandName = GetCallbackCommandName();
                        var commandParams = GetCallbackCommandParams(commandName) as dynamic;

                        // Save params
                        if (commandName == "init")
                            _reportUserId = (int)commandParams.ReportUserId;

                        if (commandName == "load")
                            _reportParams = commandParams.ReportParams;

                        var entities = new ReportsEntities();

                        // Process command (first part)                        
                        if (commandName == "save")
                        {
                            try
                            {
                                if (commandParams.LayoutName != null) // New report user layout
                                {
                                    if (_reportId == 0)
                                        throw new Exception("No report id found");

                                    if (_userId == "0")
                                        throw new Exception("No user id found");

                                    var reportUser = new ReportUser()
                                    {
                                        ReportId = _reportId,
                                        Name = commandParams.LayoutName,
                                        RegUserId = _userId
                                    };

                                    LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);
                                    _richText.Tag = SaveASPxGridViewLayout(CustomTableGridView);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        reportUser.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.ReportsUsers.Add(reportUser);
                                    entities.SaveChanges();
                                    CustomTableGridView.JSProperties["cpLayoutSaved"] = true;
                                    CustomTableGridView.JSProperties["cpNewReportUserId"] = reportUser.ReportUserId;
                                    _reportUserId = reportUser.ReportUserId;
                                }
                                else if (_reportUserId == 0) // Save to report layout
                                {
                                    if (_reportId == 0)
                                        throw new Exception("No report id found");

                                    var report = entities.Reports.Find(_reportId);

                                    LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);
                                    _richText.Tag = SaveASPxGridViewLayout(CustomTableGridView);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        report.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.SaveChanges();
                                    CustomTableGridView.JSProperties["cpLayoutSaved"] = true;
                                }
                                else // Save to report user layout
                                {
                                    var reportUser = entities.ReportsUsers.Find(_reportUserId);

                                    LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);
                                    _richText.Tag = SaveASPxGridViewLayout(CustomTableGridView);

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        reportUser.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.SaveChanges();
                                    CustomTableGridView.JSProperties["cpLayoutSaved"] = true;
                                }
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unsaved only                            
                            }
                        }
                        else if (commandName == "delete")
                        {
                            try
                            {
                                if (_reportUserId == 0)
                                    throw new Exception($"No layout found for report user id {_reportUserId}");

                                var reportUser = entities.ReportsUsers.Find(_reportUserId);

                                if (reportUser != null)
                                {
                                    entities.ReportsUsers.Remove(reportUser);
                                    entities.SaveChanges();
                                    CustomTableGridView.JSProperties["cpLayoutDeleted"] = true;
                                }
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as undeleted only                                                        
                            }
                        }
                        else if (commandName == "menu")
                        {
                            var column = CustomTableGridView.Columns[(int)commandParams.ColumnIndex];

                            LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);

                            // Column level
                            if (commandParams.Option == "GroupByColumn")
                                (column as GridViewDataColumn).Settings.GroupInterval = (ColumnGroupInterval)commandParams.Value;
                            else if (commandParams.Option == "HeaderFilter")
                                (column as GridViewDataColumn).Settings.AllowHeaderFilter = (bool)commandParams.Value ? DefaultBoolean.True : DefaultBoolean.False;
                            else if (commandParams.Option == "HeaderFilterMode")
                                (column as GridViewDataColumn).SettingsHeaderFilter.Mode = (GridHeaderFilterMode)commandParams.Value;
                            else if (commandParams.Option == "CellMerge")
                                (column as GridViewDataColumn).Settings.AllowCellMerge = (bool)commandParams.Value ? DefaultBoolean.True : DefaultBoolean.False;
                            else if (commandParams.Option == "FixedColumn")
                                (column as GridViewDataColumn).FixedStyle = (bool)commandParams.Value ? GridViewColumnFixedStyle.Left : GridViewColumnFixedStyle.None;

                            // Grid level
                            else if (commandParams.Option == "ShowFilterRow")
                                CustomTableGridView.SettingsBehavior.FilterRowMode = (GridViewFilterRowMode)commandParams.Value;
                            else if (commandParams.Option == "FixedGroups")
                                CustomTableGridView.SettingsBehavior.AllowFixedGroups = commandParams.Value;
                            else if (commandParams.Option == "MergeGroups")
                                CustomTableGridView.SettingsBehavior.MergeGroupsMode = (bool)commandParams.Value ? GridViewMergeGroupsMode.Always : GridViewMergeGroupsMode.Disabled;
                            else if (commandParams.Option == "GroupFooter")
                                CustomTableGridView.Settings.ShowGroupFooter = (GridViewGroupFooterMode)commandParams.Value;
                            else if (commandParams.Option == "AlternatingRowColor")
                                CustomTableGridView.Styles.AlternatingRow.Enabled = (bool)commandParams.Value ? DefaultBoolean.True : DefaultBoolean.False;
                            else if (commandParams.Option == "GridLines")
                                CustomTableGridView.Settings.GridLines = (System.Web.UI.WebControls.GridLines)commandParams.Value;

                            _richText.Tag = SaveASPxGridViewLayout(CustomTableGridView);
                        }

                        // Load layout from XRRichText if available or set data source
                        if (commandName == "init")
                        {
                            var layoutData = _reportUserId == 0 ? // Base layout
                                entities.Reports.Find(_reportId)?.LayoutData : // Load base layout
                                entities.ReportsUsers.Find(_reportUserId)?.LayoutData; // Load custom user layout

                            if (layoutData == null)
                                throw new Exception($"No layout found for report id {_reportId} or report user id {_reportUserId}");

                            var report = new XtraReport();

                            using (var memStream = new MemoryStream(layoutData))
                                report.LoadLayoutFromXml(memStream);

                            var richText = report.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRRichText>().FirstOrDefault();

                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;

                            LoadASPxGridViewLayout(CustomTableGridView, richText.Tag, false);
                        }
                        else if (commandName == "load")
                        {
                            // Set UI params & load data
                            if (_report.Parameters.Count > 0)
                            {
                                if (_reportParams != null)
                                {
                                    var oldParams = _report.Parameters.OfType<DevExpress.XtraReports.Parameters.Parameter>().ToDictionary(p => p.Name, p => p.Value);

                                    foreach (var param in _report.Parameters)
                                        param.Value = _reportParams[param.Name].Value;

                                    _report.FillDataSource();

                                    // Restore default params values (for reset params button after print mode)
                                    foreach (var param in _report.Parameters)
                                        param.Value = oldParams[param.Name];
                                }
                            }
                            else
                                _report.FillDataSource();

                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;
                            CustomTableGridView.DataBind();

                            LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);
                        }
                        else if (commandName != "delete") // For save, menu, print, export and all native callbacks
                        {
                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;
                            CustomTableGridView.DataBind(); // Only for MergeGroupsMode

                            LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag, true);
                        }

                        // Process command (second part - layout & data dependent)
                        if (commandName == "print")
                        {
                            try
                            {
                                using (var memStream = new MemoryStream())
                                {
                                    CustomTableGridViewExporter.WriteRtf(memStream, new DevExpress.XtraPrinting.RtfExportOptions { ExportMode = DevExpress.XtraPrinting.RtfExportMode.SingleFile });
                                    _richText.Rtf = Encoding.UTF8.GetString(memStream.GetBuffer());
                                }

                                CustomTableGridView.JSProperties["cpLayoutPrinted"] = true;
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unprinted only                            
                            }
                        }
                        else if (commandName == "export")
                        {
                            try
                            {
                                string fileName = string.Empty;

                                switch ((int)commandParams.Type)
                                {
                                    case 0:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.pdf");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomTableGridViewExporter.WritePdf(fileStream);
                                        }
                                        break;
                                    case 1:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.xls");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomTableGridViewExporter.WriteXls(fileStream);
                                        }
                                        break;
                                    case 2:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.xlsx");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomTableGridViewExporter.WriteXlsx(fileStream);
                                        }
                                        break;
                                    case 3:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.rtf");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomTableGridViewExporter.WriteRtf(fileStream);
                                        }
                                        break;
                                    case 4:
                                        {
                                            fileName = Context.Server.MapPath($"~/Temp/{_report.Name}.csv");

                                            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                                            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                                                CustomTableGridViewExporter.WriteCsv(fileStream);
                                        }
                                        break;
                                }

                                if (fileName.Length > 0)
                                    CustomTableGridView.JSProperties["cpLayoutExportedTo"] = Path.GetFileName(fileName);
                            }
                            catch (Exception)
                            {
                                // Log error
                                // For now, mark as unexported only
                            }
                        }
                    }
                }                
            }
            catch (Exception)
            {
                // Log error here                
                if (!IsPostBack) // Close the page
                    Response.Redirect(Request.UrlReferrer?.LocalPath ?? "~/");
                else // Or, page loaded, show the error
                    throw;
            }
        }

        protected void CustomCubePivotGrid_PopupMenuCreated(object sender, PivotPopupMenuCreatedEventArgs e)
        {
            if (e.MenuType == PivotGridPopupMenuType.HeaderMenu)
            {
                var item = e.Menu.Items.Add("Summary Type", "SummaryType");

                item.BeginGroup = true;
                item.Items.AddRange(new MenuItem[]
                {
                    new MenuItem("Count", item.Name + ":0"),
                    new MenuItem("Sum", item.Name + ":1"),
                    new MenuItem("Min", item.Name + ":2"),
                    new MenuItem("Max", item.Name + ":3"),
                    new MenuItem("Average", item.Name + ":4"),
                    new MenuItem("StdDev", item.Name + ":5"),
                    new MenuItem("StdDevp", item.Name + ":6"),
                    new MenuItem("Var", item.Name + ":7"),
                    new MenuItem("Varp", item.Name + ":8")
                });
                item.Name = "#" + item.Name; // No action sign.
            }            
        }

        protected void CustomTableGridView_FillContextMenuItems(object sender, ASPxGridViewContextMenuEventArgs e)
        {
            if (e.MenuType == GridViewContextMenuType.Columns)
            {
                // Column level
                var item = e.Items[e.Items.IndexOfCommand(GridViewContextMenuCommand.GroupByColumn)];

                item.Items.AddRange(new GridViewContextMenuItem[]
                {
                    new GridViewContextMenuItem("Value", item.Name + ":1") { GroupName = item.Name },
                    new GridViewContextMenuItem("Display Text", item.Name + ":7") { GroupName = item.Name },
                    new GridViewContextMenuItem("Alphabetical", item.Name + ":6") { GroupName = item.Name },
                    new GridViewContextMenuItem("Date", item.Name + ":2") { GroupName = item.Name },
                    new GridViewContextMenuItem("Date Month", item.Name + ":3") { GroupName = item.Name },
                    new GridViewContextMenuItem("Date Year", item.Name + ":4") { GroupName = item.Name },
                    new GridViewContextMenuItem("Date Range", item.Name + ":5") { GroupName = item.Name }
                });                

                item = e.Items.Add("Show Header Filter", "HeaderFilter");
                item.BeginGroup = true;
                item.GroupName = item.Name;
                item.Items.AddRange(new GridViewContextMenuItem[] 
                {
                    new GridViewContextMenuItem("List", item.Name + "Mode:1") { GroupName = item.Name + "Mode" },
                    new GridViewContextMenuItem("Checked List", item.Name + "Mode:2") { GroupName = item.Name + "Mode" },
                    new GridViewContextMenuItem("Date Range Picker", item.Name + "Mode:3") { GroupName = item.Name + "Mode" },
                    new GridViewContextMenuItem("Date Range Calendar", item.Name + "Mode:4") { GroupName = item.Name + "Mode" }
                });

                item = e.Items.Add("Allow Cell Merge", "CellMerge");
                item.GroupName = item.Name;                

                item = e.Items.Add("Fix This Column", "FixedColumn");
                item.GroupName = item.Name;                

                // Grid level
                item = e.Items[e.Items.IndexOfCommand(GridViewContextMenuCommand.ShowFilterRow)];
                item.Items.AddRange(new GridViewContextMenuItem[]
                {
                    new GridViewContextMenuItem("Auto", item.Name + ":0") { GroupName = item.Name },
                    new GridViewContextMenuItem("On Enter", item.Name + ":1") { GroupName = item.Name }
                });

                item = e.Items.Add("Allow Fixed Groups", "FixedGroups");
                item.GroupName = item.Name;
                item.Visible = false;

                item = e.Items.Add("Allow Merge Groups", "MergeGroups");
                item.GroupName = item.Name;
                item.Visible = false;

                item = e.Items.Add("Show Group Footer", "GroupFooter");
                item.BeginGroup = true;
                item.Items.AddRange(new GridViewContextMenuItem[]
                {
                    new GridViewContextMenuItem("Hidden", item.Name + ":0") { GroupName = item.Name },
                    new GridViewContextMenuItem("Visible If Expanded", item.Name + ":1") { GroupName = item.Name },
                    new GridViewContextMenuItem("Visible Always", item.Name + ":2") { GroupName = item.Name }
                });
                item.Name = "#" +item.Name; // No action sign.

                item = e.Items.Add("Alternating Row Color", "AlternatingRowColor");
                item.GroupName = item.Name;

                item = e.Items.Add("Grid Lines", "GridLines");
                item.Items.AddRange(new GridViewContextMenuItem[]
                {
                    new GridViewContextMenuItem("None", item.Name + ":0") { GroupName = item.Name },
                    new GridViewContextMenuItem("Horizontal", item.Name + ":1") { GroupName = item.Name },
                    new GridViewContextMenuItem("Vertical", item.Name + ":2") { GroupName = item.Name },
                    new GridViewContextMenuItem("Both", item.Name + ":3") { GroupName = item.Name }
                });
                item.Name = "#" + item.Name; // No action sign.
            }
        }        
    }
}