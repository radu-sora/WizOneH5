using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.Office;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.ClientControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using WizOne.Generatoare.Reports.Models;

namespace WizOne.Generatoare.Reports.Pages
{
    public partial class ReportDesign : Page
    {
        private int _reportId
        {
            get { return Session["ReportId"] as int? ?? 0; }
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
        private string _gridTempLayout
        {
            get { return Session["GridTempLayout"] as string; }
            set { Session["GridTempLayout"] = value; }
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

        private string SaveASPxGridViewLayout(ASPxGridView aspxGridView, string layout)
        {
            return JsonConvert.SerializeObject(new
            {
                SearchPanel = aspxGridView.SettingsSearchPanel.Visible,
                GroupPanel = aspxGridView.Settings.ShowGroupPanel,
                HeaderFilter = aspxGridView.Settings.ShowHeaderFilterButton,
                FilterRow = aspxGridView.Settings.ShowFilterRow,
                FilterRowMenu = aspxGridView.Settings.ShowFilterRowMenu,
                Footer = aspxGridView.Settings.ShowFooter,
                Layout = layout ?? aspxGridView.SaveClientLayout()
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

        private SqlDataSource GetSqlDataSourceForMetadata(object dataSource, string dataMember)
        {
            if (dataSource != null && (dataSource as SqlDataSource).Queries.Count > 0 && dataMember != null)
            {
                var resultDataSource = new SqlDataSource();
                var sql = "SELECT ";
                var resultSet = (dataSource as IListSource).GetList() as ResultSet;
                var resultTable = dataMember.Length > 0 ? resultSet.Tables.First(tbl => tbl.TableName == dataMember) : resultSet.Tables.First();

                resultTable.Columns.ForEach(col =>
                {
                    sql += $"0 AS [{col.Name}], ";
                });

                sql = sql.TrimEnd(new char[] { ',', ' ' });

                resultDataSource.LoadFromXml((dataSource as SqlDataSource).SaveToXml());
                resultDataSource.Queries.Clear();
                resultDataSource.Queries.Add(new CustomSqlQuery(dataMember.Length > 0 ? dataMember : "Query", sql));
                resultDataSource.Fill();

                return resultDataSource;
            }

            return null;
        }

        private string GetRtf(byte[] rtfData)
        {
            var rtf = string.Empty;

            if (rtfData != null)
            {
                // Process RTF fields i.e. convert MERGEFIELD fields to embedded fields.                
                var fieldIndex = 0;

                rtf = Encoding.UTF8.GetString(rtfData);

                while ((fieldIndex = rtf.IndexOf(@"{\field", fieldIndex)) > -1)
                {
                    var endFieldIndex = rtf.IndexOf("}}}", fieldIndex);
                    var fieldData = rtf.Substring(fieldIndex, endFieldIndex - fieldIndex + 3);
                    var mergeFieldIndex = fieldData.IndexOf("MERGEFIELD");

                    if (mergeFieldIndex > -1)
                    {
                        var reportFieldData = "[" + fieldData.Substring(mergeFieldIndex + 11).Split('}')[0].Trim() + "]";

                        rtf = rtf.Replace(fieldData, reportFieldData);
                    }
                    else
                        fieldIndex = endFieldIndex;
                }                              
            }

            return rtf;
        }

        private string GetReportName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var invalidChars = Path.GetInvalidFileNameChars().Concat(
                    Path.GetInvalidPathChars().Where(c => !Path.GetInvalidFileNameChars().Contains(c)));

                name = string.Concat(name.Where(c => !invalidChars.Contains(c))); // Remove invalid file name & path chars.
                name = name. // Transform in camel case with no spaces.
                    Split(' ').
                    Where(val => val.Length > 0).
                    Select(val => string.Concat(char.ToUpper(val[0]), val.Substring(1))).
                    Aggregate((curr, next) => curr + next);
            }

            return name;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack || ReportDesignerCallbackPanel.IsCallback)
                {
                    // Reset session data                                        
                    Session.Remove("ReportParams");
                    Session.Remove("ChartOptions");
                    Session.Remove("GridTempLayout");
                    Session.Remove("Report");

                    // Load data
                    if (_reportId == 0)
                        throw new Exception("No report id found");

                    var entities = new ReportsEntities();
                    var report = entities.Reports.Find(_reportId);

                    if (report == null)
                        throw new Exception($"No report found for id {_reportId}");

                    var xtraReport = new XtraReport();

                    // Update or create default layout by report type
                    if (report.LayoutData != null)
                    {
                        using (var memStream = new MemoryStream(report.LayoutData))
                            xtraReport.LoadLayoutFromXml(memStream);

                        if (report.ReportTypeId == 2) // Document
                        {
                            var detailBand = xtraReport.Bands.OfType<DetailBand>().FirstOrDefault();

                            if (detailBand != null)
                            {
                                var richTexts = detailBand.Controls.OfType<XRRichText>();

                                if (richTexts.Count() == 0)
                                {
                                    var defaultXRRichTextHeight = 100;

                                    if (detailBand.HeightF < defaultXRRichTextHeight)
                                        detailBand.HeightF += defaultXRRichTextHeight - detailBand.HeightF;

                                    detailBand.Controls.Add(new XRRichText()
                                    {
                                        Size = new Size(detailBand.Right, defaultXRRichTextHeight)
                                    });
                                }
                            }
                        }
                        else if (report.ReportTypeId == 3) // Cube
                        {
                            var detailBand = xtraReport.Bands.OfType<DetailBand>().FirstOrDefault();

                            if (detailBand != null)
                            {
                                var pivotGrids = detailBand.Controls.OfType<XRPivotGrid>();

                                if (pivotGrids.Count() == 0)
                                {
                                    var defaultXRPivotGridHeight = 100;

                                    if (detailBand.HeightF < defaultXRPivotGridHeight)
                                        detailBand.HeightF += defaultXRPivotGridHeight - detailBand.HeightF;

                                    detailBand.Controls.Add(new XRPivotGrid()
                                    {
                                        Size = new Size(detailBand.Right, defaultXRPivotGridHeight)
                                    });
                                }
                            }
                        }
                        else if (report.ReportTypeId == 4) // Table
                        {
                            var detailBand = xtraReport.Bands.OfType<DetailBand>().FirstOrDefault();

                            if (detailBand != null)
                            {
                                var richTexts = detailBand.Controls.OfType<XRRichText>();

                                if (richTexts.Count() == 0)
                                {
                                    var defaultXRRichTextHeight = 100;

                                    if (detailBand.HeightF < defaultXRRichTextHeight)
                                        detailBand.HeightF += defaultXRRichTextHeight - detailBand.HeightF;

                                    detailBand.Controls.Add(new XRRichText()
                                    {
                                        Size = new Size(detailBand.Right, defaultXRRichTextHeight)
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        if (report.ReportTypeId == 2) // Document
                        {
                            var detailBand = new DetailBand();

                            xtraReport.Bands.Add(detailBand);

                            detailBand.Controls.Add(new XRRichText()
                            {
                                Size = new Size(detailBand.Right, detailBand.Bottom)
                            });
                        }
                        else if (report.ReportTypeId == 3) // Cube
                        {
                            var defaultXRPivotGridHeight = 100;
                            var defaultXRChartHeight = 150;
                            var detailBand = new DetailBand()
                            {
                                HeightF = defaultXRPivotGridHeight + defaultXRChartHeight
                            };

                            xtraReport.Bands.Add(detailBand);

                            detailBand.Controls.Add(new XRPivotGrid()
                            {
                                Size = new Size(detailBand.Right, defaultXRPivotGridHeight)
                            });

                            var xrChart = new XRChart()
                            {
                                TopF = defaultXRPivotGridHeight,
                                Size = new Size(detailBand.Right, defaultXRChartHeight),
                                DataSource = detailBand.Controls.OfType<XRPivotGrid>().First(),
                                SeriesDataMember = "Series"
                            };

                            xrChart.Series.Add(new Series()
                            {
                                ArgumentDataMember = "Arguments"
                            });

                            detailBand.Controls.Add(xrChart);
                        }
                        else if (report.ReportTypeId == 4) // Table
                        {
                            var defaultXRRichTextHeight = 100;
                            var detailBand = new DetailBand()
                            {
                                HeightF = defaultXRRichTextHeight
                            };

                            xtraReport.Bands.Add(detailBand);

                            detailBand.Controls.Add(new XRRichText()
                            {
                                Size = new Size(detailBand.Right, defaultXRRichTextHeight)
                            });
                        }

                        // Set internal report id
                        xtraReport.Tag = _reportId;
                    }

                    // Update internal name of the report
                    xtraReport.Name = GetReportName(report.Name);
                    xtraReport.DisplayName = report.Name;

                    // Update internal name of the subreports available in this report
                    foreach (var subreport in ReportDesigner.Subreports)
                    {
                        //var subReportId = Convert.ToInt32(subreport.Key);
                        //var subReportName = mainContext.Reports.Find(subReportId)?.Name;
                        // ...                        
                    }

                    // Save layout changes
                    using (var memStream = new MemoryStream())
                    {
                        xtraReport.SaveLayoutToXml(memStream);
                        report.LayoutData = memStream.GetBuffer();
                    }

                    entities.SaveChanges();

                    // Init controls                   
                    ReportType = report.ReportTypeId; // For client side customization

                    if (report.ReportTypeId == 3) // Cube
                    {
                        var pivotGrid = xtraReport.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRPivotGrid>().FirstOrDefault();

                        // Init chart options           
                        ChartTypeComboBox.Items.AddRange(Enum.GetValues(typeof(ViewType)).OfType<ViewType>().
                            Select(vt => new ListEditItem() { Value = (int)vt, Text = vt.ToString() }).ToList());

                        ChartOptionsCheckBoxList.Items.Add("Show Point Labels", "O1");
                        ChartOptionsCheckBoxList.Items.Add("Generate Series from Columns", "O2");
                        ChartOptionsCheckBoxList.Items.Add("Show Column Grand Totals", "O3");
                        ChartOptionsCheckBoxList.Items.Add("Show Row Grand Totals", "O4");
                        ChartOptionsCheckBoxList.Items.Add("Show Chart", "O5");

                        // Bind to empty datasource to load columns definition.
                        if (pivotGrid.Fields.Count == 0)
                            pivotGrid.RetrieveFields(); // Retrieve all data source fields into filter area by default. Can be customized later.

                        LoadASPxPivotGridLayoutFromXRPivotGrid(CustomCubePivotGrid, pivotGrid); // Load layout if exists.
                    }
                    else if (report.ReportTypeId == 4) // Table
                    {
                        // Bind to empty datasource to load columns definition into viewstate. This is for grouping feature.
                        var richText = xtraReport.Bands.OfType<DetailBand>().FirstOrDefault()?.Controls.OfType<XRRichText>().FirstOrDefault();

                        CustomTableGridView.DataSource = xtraReport.DataSource;
                        CustomTableGridView.DataMember = xtraReport.DataMember;
                        CustomTableGridView.DataBind();

                        LoadASPxGridViewLayout(CustomTableGridView, richText?.Tag); // Load layout if exists.
                    }

                    // Customize report designer UI
                    if (report.ReportTypeId == 2) // Document
                    {
                        ReportDesigner.MenuItems.Add(new ClientControlsMenuItem()
                        {
                            Text = "Compose document text",
                            ImageClassName = "dxrd-image-run-wizard",
                            JSClickAction = "function() { showCustomLayoutSection(true); }",
                            Container = MenuItemContainer.Toolbar
                        });
                    }
                    else if (report.ReportTypeId == 3) // Cube
                    {
                        ReportDesigner.MenuItems.Add(new ClientControlsMenuItem()
                        {
                            Text = "Customize cube layout",
                            ImageClassName = "dxrd-image-run-wizard",
                            JSClickAction = "function() { showCustomLayoutSection(true); }",
                            Container = MenuItemContainer.Toolbar
                        });
                    }
                    else if (report.ReportTypeId == 4) // Table
                    {
                        ReportDesigner.MenuItems.Add(new ClientControlsMenuItem()
                        {
                            Text = "Customize table layout",
                            ImageClassName = "dxrd-image-run-wizard",
                            JSClickAction = "function() { showCustomLayoutSection(true); }",
                            Container = MenuItemContainer.Toolbar
                        });
                    }

                    // Open the report
                    ReportDesigner.OpenReport(_reportId.ToString());
                }
                else if (CustomDocumentCallbackPanel.IsCallback || CustomDocumentRichEdit.IsCallback)
                {
                    if (_richText != null)
                    {
                        var commandName = GetCallbackCommandName();

                        if (_reportId == 0)
                            throw new Exception("No report id found");

                        // Process command
                        if (commandName == "save")
                        {
                            try
                            {
                                var rtf = GetRtf(CustomDocumentRichEdit.SaveCopy(DevExpress.XtraRichEdit.DocumentFormat.Rtf));

                                if (_richText.Rtf != rtf)
                                {
                                    if (_reportId == 0)
                                        throw new Exception("No report id found");

                                    var entities = new ReportsEntities();
                                    var report = entities.Reports.Find(_reportId);

                                    _richText.Rtf = rtf;

                                    // Save report layout changes
                                    using (var memStream = new MemoryStream())
                                    {
                                        _report.SaveLayoutToXml(memStream);
                                        report.LayoutData = memStream.GetBuffer();
                                    }

                                    entities.SaveChanges();
                                }

                                CustomDocumentCallbackPanel.JSProperties["cpDocumentSaved"] = true;
                            }
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as unsaved only                    
                            }
                        }
                        else if (commandName == "load")
                        {
                            if (_richText.Rtf.Length > 0)
                            {
                                DocumentManager.CloseDocument(_reportId.ToString());
                                CustomDocumentRichEdit.Open(_reportId.ToString(), DevExpress.XtraRichEdit.DocumentFormat.Rtf, () =>
                                {
                                    return Encoding.UTF8.GetBytes(_richText.Rtf);
                                });
                            }
                            else
                                CustomDocumentRichEdit.New();
                        }

                        // Set data source                        
                        CustomDocumentRichEdit.DataSource = GetSqlDataSourceForMetadata(_report.DataSource, _report.DataMember);
                        CustomDocumentRichEdit.DataMember = _report.DataMember;
                        CustomDocumentRichEdit.DataBind();
                    }
                }
                else if (CustomCubePivotGrid.IsCallback || CustomCubePivotGrid.IsPrefilterPopupVisible)
                {
                    if (_pivotGrid != null)
                    {
                        var commandName = GetCallbackCommandName();
                        var commandParams = GetCallbackCommandParams(commandName) as dynamic;

                        // Save params
                        if (commandName == "load")
                            _reportParams = commandParams.ReportParams;

                        var entities = new ReportsEntities();

                        // Process command
                        if (commandName == "save")
                        {
                            try
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
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as unsaved only                            
                            }
                        }

                        // Load layout from XRPivotGrid if available or set data source
                        if (commandName == "init")
                        {
                            // Return chart options on client for chart control & chart options controls init
                            CustomCubePivotGrid.JSProperties["cpChartOptions"] = SaveXRChartOptions(_chart, _pivotGrid);

                            if (_pivotGrid.Fields.Count == 0)
                                _pivotGrid.RetrieveFields(); // Retrieve all data source fields into filter area by default. Can be customized later.

                            LoadASPxPivotGridLayoutFromXRPivotGrid(CustomCubePivotGrid, _pivotGrid);                            
                        }
                        else if (commandName == "load")
                        {
                            // Set UI params & load data
                            if (_report.Parameters.Count > 0)
                            {
                                if (_reportParams != null && (_reportParams as JObject).HasValues)
                                {
                                    var oldParams = _report.Parameters.OfType<Parameter>().ToDictionary(p => p.Name, p => p.Value);

                                    foreach (var param in _report.Parameters)
                                        param.Value = _reportParams[param.Name].Value;

                                    _report.FillDataSource();

                                    // Restore default params values (do not override default params on save command)
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
                            _reportParams = null;

                        if (commandName == "load")
                            _reportParams = commandParams.ReportParams;

                        var entities = new ReportsEntities();

                        // Process command
                        if (commandName == "save")
                        {
                            try
                            {
                                if (_reportId == 0)
                                    throw new Exception("No report id found");

                                var report = entities.Reports.Find(_reportId);

                                _richText.Tag = SaveASPxGridViewLayout(CustomTableGridView, _gridTempLayout);

                                // Save report layout changes
                                using (var memStream = new MemoryStream())
                                {
                                    _report.SaveLayoutToXml(memStream);
                                    report.LayoutData = memStream.GetBuffer();
                                }

                                entities.SaveChanges();
                                CustomTableGridView.JSProperties["cpLayoutSaved"] = true;
                            }
                            catch (Exception ex)
                            {
                                // Log error
                                // For now, mark as unsaved only                            
                            }
                        }

                        // Load layout from XRRichText if available or set data source
                        if (commandName == "init" || commandName == "load")
                        {
                            // Set UI params & load data
                            if (_report.Parameters.Count > 0)
                            {
                                if (_reportParams != null && (_reportParams as JObject).HasValues)
                                {
                                    var oldParams = _report.Parameters.OfType<Parameter>().ToDictionary(p => p.Name, p => p.Value);

                                    foreach (var param in _report.Parameters)
                                        param.Value = _reportParams[param.Name].Value;

                                    _report.FillDataSource();

                                    // Restore default params values (do not override default params on save command)
                                    foreach (var param in _report.Parameters)
                                        param.Value = oldParams[param.Name];
                                }
                            }
                            else
                                _report.FillDataSource();

                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;
                            CustomTableGridView.DataBind();

                            LoadASPxGridViewLayout(CustomTableGridView, commandName == "init" ? _richText.Tag : _gridTempLayout);
                        }
                        else // For save and all native callbacks
                        {
                            CustomTableGridView.DataSource = _report.DataSource;
                            CustomTableGridView.DataMember = _report.DataMember;

                            CustomTableGridView.LoadClientLayout(_gridTempLayout);
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

        protected void CustomTableGridView_ClientLayout(object sender, ASPxClientLayoutArgs e)
        {
            if (e.LayoutMode == ClientLayoutMode.Saving)
                _gridTempLayout = e.LayoutData;
        }

        protected void ReportDesigner_SaveReportLayout(object sender, SaveReportLayoutEventArgs e)
        {
            try
            {
                _report = null;

                using (var memStream = new MemoryStream(e.ReportLayout))
                    _report.LoadLayoutFromXml(memStream);

                ReportDesigner.JSProperties["cpHasChart"] = _chart != null;
                ReportDesigner.JSProperties["cpReportLoaded"] = true;
            }
            catch (Exception ex)
            {
                // Log error
                // For now, mark as unloaded only
            }
        }
    }
}