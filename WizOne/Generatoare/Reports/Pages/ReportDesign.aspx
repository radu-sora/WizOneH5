<%@ Page Title="Design Report" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" ViewStateMode="Disabled" CodeBehind="ReportDesign.aspx.cs" Inherits="WizOne.Generatoare.Reports.Pages.ReportDesign" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxRichEdit.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxRichEdit" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v18.1.Web, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v18.1.Web, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts.Web.Designer" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraReports.v18.1.Web.WebForms, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Modal dialogs -->

    <!-- Page content -->
    <table class="report-template">
        <tr>
            <td id="customLayoutSection">
                <!-- Toolbar -->
                <table>
                    <tr>                                     
                        <td>
                            <dx:ASPxComboBox ID="ChartTypeComboBox" ClientInstanceName="chartTypeComboBox" runat="server" DropDownStyle="DropDown" Caption="Chart type" Theme="Mulberry"
                                ValueType="System.Int32">                                
                                <ClientSideEvents                                    
                                    ValueChanged="function(s, e) {                                                                
                                        onChangeChartOptions();
                                    }" />                        
                            </dx:ASPxComboBox>
                        </td>
                        <td>
                            <div class="dropdown">
                              <button class="btn btn-default btn-sm dropdown-toggle" type="button" id="optionsDropdownMenu" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                                Options
                                <span class="caret"></span>
                              </button>
                              <ul class="dropdown-menu" aria-labelledby="optionsDropdownMenu">
                                <dx:ASPxCheckBoxList ID="ChartOptionsCheckBoxList" ClientInstanceName="chartOptionsCheckBoxList" runat="server" RepeatColumns="1" Theme="Mulberry">                                    
                                    <Border BorderStyle="None" />
                                    <FocusedStyle CssClass="dx-checkboxlist-focus-none" />
                                    <ClientSideEvents
                                        SelectedIndexChanged="function(s, e) {
                                            onChangeChartOptions();
                                        }" />
                                </dx:ASPxCheckBoxList>                               
                              </ul>
                            </div>
                        </td>
                        <td>
                            <button type="button" class="btn btn-default btn-sm" title="Save layout" onclick="onCustomLayoutSaveButtonClick()">
                                <span class="glyphicon glyphicon-saved" aria-hidden="true"></span>
                            </button>
                        </td>                        
                        <td>
                            <button type="button" class="btn btn-default btn-sm" title="Design" onclick="onCustomLayoutDesignButtonClick()">
                                <span class="glyphicon glyphicon-file" aria-hidden="true"></span>
                            </button>                            
                        </td>
                    </tr>
                </table>

                <!-- Client area -->

                <!-- Document customization -->
                <dx:ASPxCallbackPanel ID="CustomDocumentCallbackPanel" ClientInstanceName="customDocumentCallbackPanel" runat="server" Theme="Mulberry">
                    <PanelCollection>
                        <dx:PanelContent runat="server">                        
                            <dx:ASPxRichEdit ID="CustomDocumentRichEdit" ClientInstanceName="customDocumentRichEdit" runat="server" Theme="Mulberry" Width="100%"
                                ShowConfirmOnLosingChanges="false">
                                <RibbonTabs>
                                    <dx:RERFileTab>
                                        <Groups>
                                            <dx:RERFileCommonGroup>
                                                <Items>
                                                    <dx:RERNewCommand Size="Large" />
                                                </Items>
                                            </dx:RERFileCommonGroup>
                                        </Groups>
                                    </dx:RERFileTab>
                                    <dx:RERHomeTab>
                                        <Groups>
                                            <dx:RERUndoGroup>
                                                <Items>
                                                    <dx:RERUndoCommand />
                                                    <dx:RERRedoCommand />
                                                </Items>
                                            </dx:RERUndoGroup>
                                            <dx:RERClipboardGroup>
                                                <Items>
                                                    <dx:RERPasteCommand Size="Large" />
                                                    <dx:RERCutCommand />
                                                    <dx:RERCopyCommand />
                                                </Items>
                                            </dx:RERClipboardGroup>
                                            <dx:RERFontGroup>
                                                <Items>
                                                    <dx:RERFontNameCommand>
                                                        <PropertiesComboBox DropDownStyle="DropDown" Width="150px" />
                                                    </dx:RERFontNameCommand>
                                                    <dx:RERFontSizeCommand>
                                                        <PropertiesComboBox DropDownStyle="DropDown" Width="60px" />
                                                    </dx:RERFontSizeCommand>
                                                    <dx:RERIncreaseFontSizeCommand />
                                                    <dx:RERDecreaseFontSizeCommand />
                                                    <dx:RERChangeCaseCommand />
                                                    <dx:RERFontBoldCommand />
                                                    <dx:RERFontItalicCommand />
                                                    <dx:RERFontUnderlineCommand />
                                                    <dx:RERFontStrikeoutCommand />
                                                    <dx:RERFontSuperscriptCommand />
                                                    <dx:RERFontSubscriptCommand />
                                                    <dx:RERFontColorCommand AutomaticColorItemCaption="Automatic" AutomaticColorItemValue="0" Color="Black" EnableAutomaticColorItem="True" EnableCustomColors="True" />
                                                    <dx:RERFontBackColorCommand AutomaticColor="" AutomaticColorItemCaption="No Color" AutomaticColorItemValue="16777215" EnableAutomaticColorItem="True" EnableCustomColors="True" />
                                                    <dx:RERClearFormattingCommand />
                                                </Items>
                                            </dx:RERFontGroup>
                                            <dx:RERParagraphGroup>
                                                <Items>
                                                    <dx:RERBulletedListCommand />
                                                    <dx:RERNumberingListCommand />
                                                    <dx:RERMultilevelListCommand />
                                                    <dx:RERDecreaseIndentCommand />
                                                    <dx:RERIncreaseIndentCommand />
                                                    <dx:RERShowWhitespaceCommand />
                                                    <dx:RERAlignLeftCommand />
                                                    <dx:RERAlignCenterCommand />
                                                    <dx:RERAlignRightCommand />
                                                    <dx:RERAlignJustifyCommand />
                                                    <dx:RERParagraphLineSpacingCommand />
                                                    <dx:RERParagraphBackColorCommand AutomaticColor="" AutomaticColorItemCaption="No Color" AutomaticColorItemValue="16777215" EnableAutomaticColorItem="True" EnableCustomColors="True" />
                                                </Items>
                                            </dx:RERParagraphGroup>                        
                                            <dx:RERStylesGroup>
                                                <Items>
                                                    <dx:RERChangeStyleCommand MaxColumnCount="10" MaxTextWidth="65px" MinColumnCount="2">
                                                        <PropertiesDropDownGallery RowCount="3" />
                                                    </dx:RERChangeStyleCommand>
                                                </Items>
                                            </dx:RERStylesGroup>
                                            <dx:REREditingGroup>
                                                <Items>
                                                    <dx:RERFindCommand />
                                                    <dx:RERReplaceCommand />
                                                    <dx:RERSelectAllCommand />
                                                </Items>
                                            </dx:REREditingGroup>
                                        </Groups>
                                    </dx:RERHomeTab>
                                    <dx:RERInsertTab>
                                        <Groups>
                                            <dx:RERPagesGroup>
                                                <Items>
                                                    <dx:RERInsertPageBreakCommand Size="Large" />
                                                </Items>
                                            </dx:RERPagesGroup>
                                            <dx:RERTablesGroup>
                                                <Items>
                                                    <dx:RERInsertTableCommand Size="Large" />
                                                </Items>
                                            </dx:RERTablesGroup>
                                            <dx:RERIllustrationsGroup>
                                                <Items>
                                                    <dx:RERInsertPictureCommand Size="Large" />
                                                </Items>
                                            </dx:RERIllustrationsGroup>
                                            <dx:RERLinksGroup>
                                                <Items>
                                                    <dx:RERShowBookmarksFormCommand Size="Large" />
                                                    <dx:RERShowHyperlinkFormCommand Size="Large" />
                                                </Items>
                                            </dx:RERLinksGroup>
                                            <dx:RERHeaderAndFooterGroup>
                                                <Items>
                                                    <dx:REREditPageHeaderCommand Size="Large" />
                                                    <dx:REREditPageFooterCommand Size="Large" />
                                                    <dx:RERInsertPageNumberFieldCommand Size="Large" />
                                                    <dx:RERInsertPageCountFieldCommand Size="Large" />
                                                </Items>
                                            </dx:RERHeaderAndFooterGroup>
                                            <dx:RERSymbolsGroup>
                                                <Items>
                                                    <dx:RERShowSymbolFormCommand Size="Large" />
                                                </Items>
                                            </dx:RERSymbolsGroup>
                                        </Groups>
                                    </dx:RERInsertTab>
                                    <dx:RERPageLayoutTab>
                                        <Groups>
                                            <dx:RERPageSetupGroup>
                                                <Items>
                                                    <dx:RERPageMarginsCommand Size="Large" />
                                                    <dx:RERChangeSectionPageOrientationCommand Size="Large" />
                                                    <dx:RERChangeSectionPaperKindCommand Size="Large" />
                                                    <dx:RERSetSectionColumnsCommand Size="Large" />
                                                    <dx:RERInsertBreakCommand Size="Large" />
                                                </Items>
                                            </dx:RERPageSetupGroup>
                                            <dx:RERBackgroundGroup>
                                                <Items>
                                                    <dx:RERChangePageColorCommand AutomaticColor="Transparent" AutomaticColorItemCaption="No Color" AutomaticColorItemValue="16777215" Color="Transparent" EnableAutomaticColorItem="True" EnableCustomColors="True" Size="Large" />
                                                </Items>
                                            </dx:RERBackgroundGroup>
                                        </Groups>
                                    </dx:RERPageLayoutTab>
                                    <dx:RERMailMergeTab>
                                        <Groups>
                                            <dx:RERInsertFieldsGroup>
                                                <Items>
                                                    <dx:RERCreateFieldCommand Size="Large" />
                                                    <dx:RERInsertMergeFieldCommand Size="Large" />
                                                </Items>
                                            </dx:RERInsertFieldsGroup>
                                        </Groups>
                                    </dx:RERMailMergeTab>
                                    <dx:RERViewTab>
                                        <Groups>
                                            <dx:RERShowGroup>
                                                <Items>
                                                    <dx:RERToggleShowHorizontalRulerCommand Size="Large" />
                                                </Items>
                                            </dx:RERShowGroup>
                                            <dx:RERViewGroup>
                                                <Items>
                                                    <dx:RERToggleFullScreenCommand Size="Large" />
                                                </Items>
                                            </dx:RERViewGroup>
                                        </Groups>
                                    </dx:RERViewTab>
                                </RibbonTabs>
                            </dx:ASPxRichEdit>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>

                <!-- Cube customization -->
                <table>
                    <tr>
                        <td>
                            <dx:ASPxPivotGrid ID="CustomCubePivotGrid" ClientInstanceName="customCubePivotGrid" runat="server" EncodeHtml="false" Theme="Mulberry"
                                OnCustomCallback="CustomCubePivotGrid_CustomCallback">
                                <OptionsView DataHeadersDisplayMode="Popup" DataHeadersPopupMinCount="3" />                                
                                <OptionsFilter NativeCheckBoxes="False" />                                
                                <OptionsCustomization CustomizationFormStyle="Excel2007" />
                                <OptionsChartDataSource DataProvideMode="UseCustomSettings" />
                                <ClientSideEvents
                                    EndCallback="function(s, e) { 
                                        if (s.cpRefreshChart) {     
                                            onChartControlInit(s.cpChartOptions);
                                            delete s.cpChartOptions;                                                            
                                            delete s.cpRefreshChart;
                                        }
                                    }" />
                            </dx:ASPxPivotGrid>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:WebChartControl ID="CustomCubeWebChartControl" ClientInstanceName="customCubeWebChartControl" runat="server" Theme="Mulberry" Width="750px"
                                DataSourceID="CustomCubePivotGrid" SeriesDataMember="Series" CrosshairEnabled="True">
                                <BorderOptions Visibility="False" />
                                <Legend MaxHorizontalPercentage="30" />                                
                                <SeriesTemplate ArgumentDataMember="Arguments" ValueDataMembersSerializable="Values" ArgumentScaleType="Qualitative">
                                    <ViewSerializable>
                                        <dx:SideBySideBarSeriesView />
                                    </ViewSerializable>
                                    <LabelSerializable>
                                        <dx:SideBySideBarSeriesLabel>
                                            <FillStyle>
                                                <OptionsSerializable>
                                                    <dx:SolidFillOptions />
                                                </OptionsSerializable>
                                            </FillStyle>
                                            <PointOptionsSerializable>
                                                <dx:PointOptions />
                                            </PointOptionsSerializable>
                                        </dx:SideBySideBarSeriesLabel>
                                    </LabelSerializable>
                                </SeriesTemplate>
                                <DiagramSerializable>
                                    <dx:XYDiagram>
                                        <AxisX VisibleInPanesSerializable="-1">
                                            <Label Staggered="True" />
                                        </AxisX>
                                        <AxisY VisibleInPanesSerializable="-1">
                                        </AxisY>
                                    </dx:XYDiagram>
                                </DiagramSerializable>    
                                <FillStyle>
                                    <OptionsSerializable>
                                        <dx:SolidFillOptions />
                                    </OptionsSerializable>
                                </FillStyle>                                                                                          
                            </dx:WebChartControl>
                        </td>                        
                    </tr>
                </table>        
                               
                <!-- Table customization -->
                <dx:ASPxGridView ID="CustomTableGridView" ClientInstanceName="customTableGridView" runat="server" ViewStateMode="Enabled" Theme="Mulberry" Width="100%"
                    OnClientLayout="CustomTableGridView_ClientLayout">
                    <Settings ShowHeaderFilterButton="true" />
                    <SettingsBehavior EnableRowHotTrack="true" EnableCustomizationWindow="true" />                                       
                    <SettingsContextMenu Enabled="true">
                        <RowMenuItemVisibility NewRow="false" EditRow="false" DeleteRow="false" />
                    </SettingsContextMenu>                                        
                    <Styles>
                        <Header Font-Bold="true" Wrap="True" />
                    </Styles>                                
                </dx:ASPxGridView>
                <dx:ASPxGridViewExporter ID="CustomTableGridViewExporter" runat="server" GridViewID="CustomTableGridView"
                    TopMargin="0" BottomMargin="0" LeftMargin="0" RightMargin="0">
                </dx:ASPxGridViewExporter>
            </td>
            <td>
                <dx:ASPxCallbackPanel ID="ReportDesignerCallbackPanel" ClientInstanceName="reportDesignerCallbackPanel" runat="server" Theme="Mulberry">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxReportDesigner ID="ReportDesigner" ClientInstanceName="reportDesigner" runat="server" CssClass="pull-right"
                                OnSaveReportLayout="ReportDesigner_SaveReportLayout">
                                <ClientSideEvents   
                                    Init="function(s, e) {
                                        onReportDesignerInit();
                                    }"                       
                                    CustomizeMenuActions="function(s, e) {
                                        onCustomizeMenu(e.Actions);
                                    }" 
                                    PreviewCustomizeMenuActions="function(s, e) {                                        
                                        onCustomizeMenu(e.Actions, true);
                                    }"                                     
                                    CustomizeParameterEditors="function(s, e) {
                                        onCustomizeParameter(e.parameter, e.info);
                                    }"
                                    CustomizeSaveAsDialog="function(s, e) { 
                                        onCustomizeSaveAsDialog(e);
                                    }"
                                    ExitDesigner="function(s, e) {
                                        onExitButtonClick();
                                    }" />
                            </dx:ASPxReportDesigner>   
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
        </tr>
    </table>
    
    <script type="text/html" id="dx-date-simple">
        <div data-bind="dxDateBox: { value: value.extend({ throttle: 500 }), closeOnValueChange: true, type: 'date', disabled: disabled }, dxValidator: { validationRules: validationRules || [] }"></div>
    </script>
    <script type="text/html" id="dxrd-savereport-dialog-content-simple">
        <div data-bind="dxTextBox: { value: $data.reportName }"></div>
    </script>
    <script>
        // Globals
        var reportType = '<%: ReportType %>';
        var customLayoutVisible;
        var customLayoutChanged;

        // Main functions
        function onReportDesignerInit() {
            var designerModel = reportDesigner.designerModel;
            var previewModel = designerModel.reportPreviewModel;

            designerModel.tabPanel.collapsed(true); // Hidden by default

            if (reportType == 3 || reportType == 4) { // For Cube and Table.
                // Hide report client area.
                previewModel.tabPanel.collapsed.subscribe(function (state) {
                    if (customLayoutVisible) {
                        reportDesigner.SetWidth(previewModel.tabPanel.headerWidth());
                    }
                });

                // Override parameters init.
                previewModel.parametersModel._initialize = previewModel.parametersModel.initialize;
                previewModel.parametersModel.initialize = function (params) {
                    var model = this;

                    model._initialize(params);

                    if (customLayoutVisible) {
                        if (params.parameters.length) { // Show only the parameters panel.
                            $('#' + reportDesigner.name + ' div.dxrd-right-tabs div[title="' + previewModel.exportModel.tabInfo.text + '"]').hide();
                            $('#' + reportDesigner.name + ' div.dxrd-right-tabs div[title="' + previewModel.searchModel.tabInfo.text + '"]').hide();

                            reportDesigner.SetWidth(previewModel.tabPanel.headerWidth()); // Hide report client area.
                            reportDesigner.SetVisible(true);
                        }

                        refreshCustomLayoutSection(true);
                    }
                };

                // Override parameters commands.
                previewModel.parametersModel._submit = previewModel.parametersModel.submit;
                previewModel.parametersModel.submit = function () {
                    var model = this;

                    if (customLayoutVisible) {
                        refreshCustomLayoutSection();
                    } else {
                        model._submit();
                    }
                };

                previewModel.parametersModel._restore = previewModel.parametersModel.restore;
                previewModel.parametersModel.restore = function () {
                    var model = this;

                    if (customLayoutVisible) {
                        model._parameters.forEach(function (param) {
                            model._shouldProcessParameter(param) && (param.lookUpValues(param._originalLookUpValues), param.initialize(param._originalValue, model.parameterHelper))
                        });

                        refreshCustomLayoutSection();
                    } else {
                        model._restore();
                    }
                };
            }
        }

        function onCustomizeMenu(actions, preview) {
            if (preview) {
                for (var action = 0; action < actions.length; action++) {
                    if (actions[action].text == 'Compose document text' ||
                        actions[action].text == 'Customize cube layout' ||
                        actions[action].text == 'Customize table layout') {
                        actions.splice(action, 1);
                    }
                }
            } else {
                for (var action = 0; action < actions.length; action++) {
                    if (actions[action].text == 'New' ||
                        actions[action].text == 'New via Wizard' ||
                        actions[action].text == 'Open') {
                        actions[action].visible = false;
                    }
                }
            }
        }

        function onCustomizeParameter(parameter, info) {
            if (parameter.type == 'System.DateTime') {
                info.editor = { header: 'dx-date-simple' };
            }
        }

        function onCustomizeSaveAsDialog(data) {
            data.Popup.title = 'Save As...';
            data.Popup.width(400);
            data.Popup.height(180);
            data.Customize('dxrd-savereport-dialog-content-simple', data.Popup.model());
        }

        function onChartControlInit(chartOptions) {
            initChartOptions(chartOptions ? JSON.parse(chartOptions) : null);
            initChartControl(chartOptions ? JSON.parse(chartOptions) : null);
        }

        function onExitButtonClick() {
            window.history.back();
        }

        function onChangeChartOptions() {
            var selectedIndex = chartTypeComboBox.GetSelectedIndex();

            if (selectedIndex > -1) {
                var optionsSelectedValues = {};
                var optionItem;
                var options = chartOptionsCheckBoxList.GetItemCount();

                for (var option = 0; option < options; option++) {
                    optionItem = chartOptionsCheckBoxList.GetItem(option);
                    optionsSelectedValues[optionItem.value] = optionItem.selected;
                }

                initChartControl({ 'Type': chartTypeComboBox.GetValue(), 'Options': optionsSelectedValues });
            }
        }

        function onCustomLayoutSaveButtonClick() {
            var commandName = '#save';
            var commandParams = {};

            if (reportType == 2) { // Document
                customDocumentCallbackPanel.PerformCallback(commandName + JSON.stringify(commandParams), function () { // Save rich edit rtf data
                    if (customDocumentCallbackPanel.cpDocumentSaved) {
                        swal({
                            title: 'Success',
                            text: 'The document was successfully saved.',
                            type: 'success'
                        });
                        delete customDocumentCallbackPanel.cpDocumentSaved;
                    }
                });
            } else if (reportType == 3) { // Cube
                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams), function () { // Save pivot grid layout
                    if (customCubePivotGrid.cpLayoutSaved) {
                        swal({
                            title: 'Success',
                            text: 'The template was successfully saved.',
                            type: 'success'
                        });
                        delete customCubePivotGrid.cpLayoutSaved;
                    }
                });
            } else if (reportType == 4) { // Table
                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams), function () { // Save grid view layout
                    if (customTableGridView.cpLayoutSaved) {
                        swal({
                            title: 'Success',
                            text: 'The template was successfully saved.',
                            type: 'success'
                        });
                        delete customTableGridView.cpLayoutSaved;
                    }
                });
            }

            customLayoutChanged = true;
        }

        function onCustomLayoutDesignButtonClick() {
            showCustomLayoutSection(false);
        }

        function showCustomLayoutSection(visible) {
            customLayoutVisible = visible;

            if (visible) {
                var isModified = reportDesigner.IsModified();

                reportDesigner.PerformCallback(null, function () { // Load report layout on server first.
                    if (reportDesigner.cpReportLoaded) {
                        // Hide designer 
                        reportDesigner.SetVisible(false);

                        // Switch customization layout
                        if (reportType == 2) { // Document        
                            refreshCustomLayoutSection();

                            ASPxClientControl.AdjustControls(customDocumentRichEdit.GetMainElement());
                            $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(1)').hide(); // Menu item specific to cube layout
                            $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(2)').hide(); // Idem
                            $('#customLayoutSection > table:nth-child(6)').hide(); // Cube layout
                            customTableGridView.SetVisible(false);
                        } else if (reportType == 3) { // Cube                         
                            reportDesigner.ShowPreview(); // Show report preview deffered.

                            if (reportDesigner.cpHasChart) {
                                $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(1)').show();
                                $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(2)').show();
                                customCubeWebChartControl.SetVisible(true);
                            } else {
                                $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(1)').hide();
                                $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(2)').hide();
                                customCubeWebChartControl.SetVisible(false);
                            }

                            customDocumentCallbackPanel.SetVisible(false);
                            customTableGridView.SetVisible(false);
                        } else if (reportType == 4) { // Table                            
                            reportDesigner.ShowPreview(); // Show report preview deffered.

                            customDocumentCallbackPanel.SetVisible(false);
                            $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(1)').hide(); // Menu item specific to cube layout
                            $('#customLayoutSection > table:nth-child(1) > tbody > tr > td:nth-child(2)').hide(); // Idem
                            $('#customLayoutSection > table:nth-child(6)').hide(); // Cube layout
                        }

                        $('#customLayoutSection').show();
                        delete reportDesigner.cpReportLoaded;
                    }

                    if (isModified) { // isDirty is reseted after PerformCallback but undo engine is in place. I think this is a bug.
                        reportDesigner.designerModel.isDirty(true);
                    }
                });
            } else {
                $('#customLayoutSection').hide();
                reportDesigner.designerModel.reportPreviewModel.reportPreview.previewVisible(false);
                reportDesigner.mainElement.style.width = '100%';
                reportDesigner.SetVisible(true);
                reportDesigner.AdjustControl();

                if (customLayoutChanged) {
                    reportDesignerCallbackPanel.PerformCallback();
                    customLayoutChanged = false;
                }
            }
        }

        function refreshCustomLayoutSection(init) {
            var commandName = init ? '#init' : '#load';
            var commandParams = {};

            if ((reportType == 3 || reportType == 4) && !init) {
                // Process selected params            
                var paramsSelectedValues = {};
                var paramModel = reportDesigner.designerModel.reportPreviewModel.parametersModel;
                var paramName;
                var paramValue;
                var params = paramModel._parameters.length;

                for (var param = 0; param < params; param++) {
                    paramName = paramModel._parameters[param].path;
                    paramValue = paramModel[paramName]();
                    paramsSelectedValues[paramName] = paramValue;
                }

                commandParams = { 'ReportParams': paramsSelectedValues };
            }

            // Send command to server            
            if (reportType == 2) { // Document
                customDocumentCallbackPanel.PerformCallback(commandName + JSON.stringify(commandParams));
            } else if (reportType == 3) { // Cube
                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams));
            } else if (reportType == 4) { // Table
                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams));
            }
        }

        function initChartOptions(chartOptions) {
            if (chartOptions) {
                var selectedOptionItems = [];
                var optionItem;
                var options = chartOptionsCheckBoxList.GetItemCount();

                chartTypeComboBox.SetValue(chartOptions.Type);
                chartOptionsCheckBoxList.UnselectAll();

                for (var option = 0; option < options; option++) {
                    optionItem = chartOptionsCheckBoxList.GetItem(option);

                    if (chartOptions.Options[optionItem.value]) {
                        selectedOptionItems.push(optionItem);
                    }
                }

                chartOptionsCheckBoxList.SelectItems(selectedOptionItems);
            }
        }

        function initChartControl(chartOptions) {
            if (chartOptions) {
                // Send command to server
                var commandName = '#options';
                var commandParams = { 'ChartOptions': chartOptions };

                customCubeWebChartControl.PerformCallback(commandName + JSON.stringify(commandParams));
            } else {
                customCubeWebChartControl.PerformCallback();
            }
        }
    </script>    
</asp:Content>
