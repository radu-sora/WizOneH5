using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using WizOne.Generatoare.Reports.Models;

namespace WizOne.Generatoare.Reports.Pages
{
    public partial class ReportView : Page
    {
        private string _userId
        {
            get { return Request.QueryString["UserId"] ?? (Session["UserId"] as int? ?? 0).ToString(); }
        }
        private int _reportId
        {            
            get
            {
                int reportId;

                return int.TryParse(Request.QueryString["ReportId"], out reportId) ? reportId : Session["ReportId"] as int? ?? 0;
            }
        }
        private bool _serverPrint
        {
            get { return (Request.QueryString["PrintareAutomata"] ?? (Session["PrintareAutomata"] as int? ?? 0).ToString()) == "1"; }
        }
        private int _reportUserId
        {
            get { return Session["ReportUserId"] as int? ?? 0; }
            set { Session["ReportUserId"] = value; }
        }
        private dynamic _reportParams
        {
            get { return Session["ReportParams"]; }
            set { Session["ReportParams"] = value; }
        }
        private dynamic _chartOptions
        {
            get { return Session["ChartOptions"]; }
            set { Session["ChartOptions"] = value; }
        }
        private XtraReport _report
        {
            get { return Session["Report"] as XtraReport ?? (_report = new XtraReport()); }
            set { Session["Report"] = value; }
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

        protected short ReportType { get; set; }
        protected short ToolbarType { get; set; }
        protected string ExportOptions { get; set; }
        protected short ChartStatus { get; set; }

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
            return JsonConvert.SerializeObject(new
            {
                SearchPanel = aspxGridView.SettingsSearchPanel.Visible,
                GroupPanel = aspxGridView.Settings.ShowGroupPanel,
                HeaderFilter = aspxGridView.Settings.ShowHeaderFilterButton,
                FilterRow = aspxGridView.Settings.ShowFilterRow,
                FilterRowMenu = aspxGridView.Settings.ShowFilterRowMenu,
                Footer = aspxGridView.Settings.ShowFooter,
                PapeSize = aspxGridView.SettingsPager.PageSize,
                Layout = aspxGridView.SaveClientLayout()
            });
        }

        private void LoadASPxGridViewLayout(ASPxGridView aspxGridView, object layout)
        {
            if (layout != null && (layout as string).Length > 0)
            {
                dynamic gridLayout = JObject.Parse(layout as string);

                aspxGridView.SettingsSearchPanel.Visible = gridLayout.SearchPanel;
                aspxGridView.Settings.ShowGroupPanel = gridLayout.GroupPanel;
                aspxGridView.Settings.ShowHeaderFilterButton = gridLayout.HeaderFilter;
                aspxGridView.Settings.ShowFilterRow = gridLayout.FilterRow;
                aspxGridView.Settings.ShowFilterRowMenu = gridLayout.FilterRowMenu;
                aspxGridView.Settings.ShowFooter = gridLayout.Footer;
                aspxGridView.LoadClientLayout((string)gridLayout.Layout);
            }
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

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Reset session data                    
                    Session.Remove("ReportUserId");
                    Session.Remove("ReportParams");
                    Session.Remove("ChartOptions");
                    Session.Remove("Report");

                    // Load data
                    if (_reportId == 0)
                        throw new Exception("No report id found");

                    var entities = new ReportsEntities();
                    var report = entities.Reports.Find(_reportId);

                    if (report == null)
                        throw new Exception($"No report found for id {_reportId}");

                    if (report.LayoutData == null)
                        throw new Exception($"No layout found for report id {_reportId}");

                    using (var memStream = new MemoryStream(report.LayoutData))
                        _report.LoadLayoutFromXml(memStream);

                    // Set internal params                    
                    var parameters = _report.ObjectStorage.OfType<SqlDataSource>().
                        SelectMany(ds => ds.Queries).SelectMany(q => q.Parameters).
                        Where(p => p.Type != typeof(Expression));

                    foreach (var param in parameters)
                    {
                        var name = param.Name.TrimStart('@');
                        var value = Request.QueryString[name] ?? Session[name];

                        if (value != null)
                            param.Value = Convert.ChangeType(value, param.Type);                        
                    }

                    // Init controls                   
                    var userId = Convert.ToInt32(_userId);
                    var reportGroupUser = report.ReportGroupUsers.SingleOrDefault(rgu => rgu.UserId == userId);

                    // For client side customization
                    ReportType = report.ReportTypeId;
                    ToolbarType = ConfigurationManager.AppSettings["EsteTactil"] == "true" ? // Temp fix until this param can be stored at user group level.
                        reportGroupUser?.ToolbarType ?? 0 : (short)0; // 0 - full items, 1 - only Print, Customize layout & Exit
                    ExportOptions = reportGroupUser?.ExportOptions ?? "*"; // "pdf,image[...]" or "*" to display all options.

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

                                // For client side customization                                
                                ChartStatus = (short)((bool)_chartOptions.Options.O5 ? 2 : 1); // 0 - None, 1 - Hidden, 2 - Visible

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
                                _report.FillDataSource();

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
                                _report.FillDataSource();

                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;
                            CustomTableGridView.DataBind();

                            LoadASPxGridViewLayout(CustomTableGridView, _richText.Tag);
                        }
                    }

                    // Customize report viewer UI
                    WebDocumentViewer.MenuItems.Add(new WebDocumentViewerMenuItem()
                    {
                        Text = "Exit",
                        ImageClassName = "dxrd-image-exit",
                        JSClickAction = "function() { onExitButtonClick(); }",
                        Container = MenuItemContainer.Toolbar
                    });

                    if (_serverPrint) // Send to server default printer & exit
                    {
                        new ReportPrintTool(_report).Print();
                        Response.Redirect(Request.UrlReferrer?.LocalPath ?? "~/");
                    }
                    else // Open the report
                        WebDocumentViewer.OpenReport(_report);
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

                        // Process command
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
                            catch (Exception ex)
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
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as undeleted only                                                        
                            }
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
                            catch (Exception ex)
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
                                        var oldParams = _report.Parameters.OfType<Parameter>().ToDictionary(p => p.Name, p => p.Value);

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

                            CustomCubePivotGrid.DataSource = _pivotGrid.DataSource;
                            CustomCubePivotGrid.DataMember = _pivotGrid.DataMember;

                            // Refresh chart control
                            CustomCubePivotGrid.JSProperties["cpRefreshChart"] = GetWebChartRefreshState();
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
                            catch (Exception ex)
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
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as undeleted only                                                        
                            }
                        }

                        if (commandName != "delete")
                        {
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

                                LoadASPxGridViewLayout(CustomTableGridView, richText.Tag);
                            }
                            else if (commandName == "load")
                            {
                                // Set UI params & load data
                                if (_report.Parameters.Count > 0)
                                {
                                    if (_reportParams != null)
                                    {
                                        var oldParams = _report.Parameters.OfType<Parameter>().ToDictionary(p => p.Name, p => p.Value);

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
                            }
                            else // For save, print and all native callbacks
                            {
                                CustomTableGridView.DataSource = _report.DataSource;
                                CustomTableGridView.DataMember = _report.DataMember;
                            }
                        }

                        // Process command (second part)
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
                            catch (Exception ex)
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
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as unexported only
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log & redirect to error page
                // For now, redirect to main page only                
                if (!IsCallback)
                    Response.Redirect(Request.UrlReferrer?.LocalPath ?? "~/");
            }
        }       
    }
}