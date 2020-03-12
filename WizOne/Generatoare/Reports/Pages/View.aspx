<%@ Page Title="View Report" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" ViewStateMode="Disabled" CodeBehind="View.aspx.cs" Inherits="Wizrom.Reports.Pages.View" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Modal dialogs -->

    <!-- Page content -->                  
    <table class="report-view-template">
        <tr>
            <td>
                <dx:ASPxCallbackPanel ID="WebDocumentViewerCallbackPanel" ClientInstanceName="webDocumentViewerCallbackPanel" runat="server" Theme="Mulberry" Height="100%">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <dx:ASPxWebDocumentViewer ID="WebDocumentViewer" ClientInstanceName="webDocumentViewer" runat="server" Height="100%">
                                <SettingsTabPanel Position="Left" />
                                <ClientSideEvents    
                                    Init="function(s, e) {
                                        onDocumentViewerInit();
                                    }" 
                                    CustomizeMenuActions="function(s, e) {
                                        onCustomizeMenu(e.Actions);
                                    }"
                                    CustomizeParameterEditors="function(s, e) {
                                        onCustomizeParameter(e.parameter, e.info);
                                    }" />
                            </dx:ASPxWebDocumentViewer>          
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
            <td id="customLayoutSection">
                <!-- Toolbar -->
                <table>
                    <tr>
                        <td>
                            <dx:ASPxComboBox ID="LayoutComboBox" ClientInstanceName="layoutComboBox" runat="server" DropDownStyle="DropDown" Caption="Layout name" Theme="Mulberry"
                                DataSourceID="ReportsUsersDataSource" ValueField="ReportUserId" ValueType="System.Int32" TextField="Name">
                                <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" SetFocusOnError="true" ValidationGroup="LayoutGroup">
                                    <RequiredField IsRequired="True" ErrorText="Layout name is required" />
                                </ValidationSettings>                            
                                <ClientSideEvents 
                                    Init="function(s, e) {
                                        s.InsertItem(0, '[Base layout]', 0);
                                        s.SetSelectedIndex(0);
                                    }"
                                    ValueChanged="function(s, e) {                                                                
                                        onChangeLayout();
                                    }" />                        
                            </dx:ASPxComboBox>
                            <ef:EntityDataSource ID="ReportsUsersDataSource" runat="server" ContextTypeName="Wizrom.Reports.Models.ReportsEntities" EntitySetName="ReportsUsers"
                                Where="it.ReportId = @ReportId AND it.RegUserId = @RegUserId" OrderBy="it.Name">
                                <WhereParameters>
                                    <asp:Parameter Name="ReportId" Type="Int32" />    
                                    <asp:Parameter Name="RegUserId" Type="String" />
                                </WhereParameters>
                            </ef:EntityDataSource>                            
                        </td>
                        <td>
                            <button type="button" class="btn btn-default btn-sm" title="Save layout" onclick="onCustomLayoutSaveButtonClick()">
                                <span class="glyphicon glyphicon-saved" aria-hidden="true"></span>
                            </button>
                        </td>
                        <td>
                            <button id="customLayoutDeleteButton" type="button" class="btn btn-danger btn-sm" disabled title="Delete layout" onclick="onCustomLayoutDeleteButtonClick()">
                                <span class="glyphicon glyphicon-remove" aria-hidden="true"></span>
                            </button>
                        </td>
                        <td class="hide">
                            <button type="button" class="btn btn-default btn-sm" title="Print" onclick="onCustomLayoutPrintButtonClick()">
                                <span class="glyphicon glyphicon-print" aria-hidden="true"></span>
                            </button>
                        </td>
                        <td>
                            <div class="dropdown">
                              <button class="btn btn-default btn-sm dropdown-toggle" type="button" id="exportDropdownMenu" title="Export to" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                                <span class="glyphicon glyphicon-floppy-save" aria-hidden="true"></span>
                                <span class="caret"></span>
                              </button>
                              <ul class="dropdown-menu" aria-labelledby="exportDropdownMenu" onclick="onCustomLayoutExportButtonClick()">
                                <li><a data-type="0">PDF</a></li>
                                <li><a data-type="1">XLS</a></li>
                                <li><a data-type="2">XLSX</a></li>
                                <li><a data-type="3">RTF</a></li>
                                <li><a data-type="4">CSV</a></li>
                              </ul>
                            </div>                            
                        </td>
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
                            <button type="button" class="btn btn-default btn-sm" title="Exit" onclick="onExitButtonClick()">
                                <span class="glyphicon glyphicon-log-out" aria-hidden="true"></span>
                            </button>                            
                        </td>
                        <td>
                            <label><%: ReportName %></label>
                        </td>
                    </tr>
                </table>

                <!-- Client area -->

                <!-- Cube customization -->
                <table>
                    <tr>
                        <td>
                            <dx:ASPxPivotGrid ID="CustomCubePivotGrid" ClientInstanceName="customCubePivotGrid" runat="server" EncodeHtml="false" Theme="Mulberry"
                                OnPopupMenuCreated="CustomCubePivotGrid_PopupMenuCreated">
                                <OptionsView DataHeadersDisplayMode="Popup" DataHeadersPopupMinCount="3" />                                
                                <OptionsFilter NativeCheckBoxes="False" />                                
                                <OptionsCustomization CustomizationFormStyle="Excel2007" />
                                <OptionsChartDataSource DataProvideMode="UseCustomSettings" />
                                <ClientSideEvents
                                    PopupMenuItemClick="function(s, e) {
                                        onCustomCubePopupMenuItemClick(e.MenuItemName, e.FieldID);
                                    }"
                                    EndCallback="function(s, e) { 
                                        if (s.cpRefreshChart) {     
                                            onChartControlInit(s.cpChartOptions);
                                            delete s.cpChartOptions;                                                            
                                            delete s.cpRefreshChart;
                                        }
                                    }" />
                            </dx:ASPxPivotGrid>
                            <dx:ASPxPivotGridExporter ID="CustomCubePivotGridExporter" runat="server" ASPxPivotGridID="CustomCubePivotGrid" 
                                OptionsPrint-PageSettings-Landscape="True">                                
                            </dx:ASPxPivotGridExporter>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <dx:WebChartControl ID="CustomCubeWebChartControl" ClientInstanceName="customCubeWebChartControl" runat="server" Theme="Mulberry" Width="750px" Height="200px"
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
                    OnFillContextMenuItems="CustomTableGridView_FillContextMenuItems">
                    <Settings ShowFilterBar="Auto" VerticalScrollBarMode="Auto" HorizontalScrollBarMode="Auto" />                    
                    <SettingsBehavior EnableRowHotTrack="true" EnableCustomizationWindow="true" AllowEllipsisInText="true" AllowFixedGroups="true" />
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live" />
                    <SettingsContextMenu Enabled="true">                        
                        <RowMenuItemVisibility NewRow="false" EditRow="false" DeleteRow="false" Refresh="false" />
                    </SettingsContextMenu>
                    <SettingsCustomizationDialog Enabled="true" />
                    <SettingsFilterControl ViewMode="VisualAndText" AllowHierarchicalColumns="true" ShowAllDataSourceColumns="true" ShowOperandTypeButton="true" />
                    <SettingsPager PageSize="30">
                        <PageSizeItemSettings Visible="true" />
                    </SettingsPager>
                    <Styles>
                        <Header Font-Bold="true" Wrap="True" />
                    </Styles>  
                    <ClientSideEvents
                        ContextMenu="function(s, e) {                            
                            onCustomTableContextMenu(e.objectType, e.index, e.menu);
                        }"                        
                        ContextMenuItemClick="function(s, e) {
                            onCustomTableContextMenuItemClick(e.item);
                        }" />
                </dx:ASPxGridView>
                <dx:ASPxGridViewExporter ID="CustomTableGridViewExporter" runat="server" GridViewID="CustomTableGridView" 
                    TopMargin="0" BottomMargin="0" LeftMargin="0" RightMargin="0" Landscape="true">                                
                </dx:ASPxGridViewExporter>
            </td>            
        </tr>
    </table>
   
    <script type="text/html" id="dx-date-simple">
        <div data-bind="dxDateBox: { value: value.extend({ rateLimit: 500 }), closeOnValueChange: true, type: 'date', disabled: disabled }, dxValidator: { validationRules: validationRules || [] }"></div>
    </script>
    <script>
        // Globals
        var reportType = <%: ReportType %>;
        var toolbarType = <%: ToolbarType %>;
        var exportOptions = '<%: ExportOptions %>';
        var chartStatus = <%: ChartStatus %>;
        var customLayoutSectionVisible = null;
        var paramsSelectedValues = {};

        // Main functions       
        $(window).on('beforeunload', function () {
            $.ajax({
                type: 'POST',
                data: { close: true },
                async: false
            });
            return;
        });

        function onControlsInitialized(s, e) {
            // Validate document ready
            if (e.isCallback) {
                return;
            }

            // Initialize UI            
            if (reportType == 3 || reportType == 4) { // For Cube and Table, display custom layout section with the toolbar and hide report client area.                
                if (reportType == 4) {
                    $(window).on('load', function () {
                        resizeGridView(customTableGridView, 177, true);
                    });

                    $(window).on('resize', function () {
                        resizeGridView(customTableGridView, 177, false);
                    });                    
                }

                showCustomLayoutSection(true);
            }            
        }

        function onDocumentViewerInit() {
            // Set valid export options
            var validExportOptions = exportOptions.split(',');
            var allExportOptions = { pdf: 0, xls: 1, xlsx: 2, rtf: 3, csv: 4, docx: -1, html: -1, mht: -1, image: -1, textExportOptions: -1 };
            var rptExportOptionsModel = webDocumentViewer.previewModel.reportPreview.exportOptionsModel;          
            var rptExportOptions = rptExportOptionsModel();
            var clsExportOptions = $('#customLayoutSection > table td > div > ul[aria-labelledby="exportDropdownMenu"] > li');

            for (var optionName in allExportOptions) {
                if (validExportOptions.indexOf(optionName) == -1 && validExportOptions.indexOf('*') == -1) {
                    delete rptExportOptions[optionName];
                    clsExportOptions.find('> a[data-type="' + allExportOptions[optionName] + '"]').parent().remove();
                }
            }

            rptExportOptionsModel(rptExportOptions);           
            
            // Setup validation (confirmation) actions
            var validateParam = webDocumentViewer.parametersInfo.parameters.filter(function (param) { return param.Name == 'Validate'; })[0];

            if (validateParam && !validateParam.Visible) {
                var items = webDocumentViewer.previewModel.actionLists.toolbarItems();
                var confirmItemsId = [DevExpress.Report.Preview.ActionId.Print, DevExpress.Report.Preview.ActionId.PrintPage, DevExpress.Report.Preview.ActionId.ExportTo];
                var confirmItems = items.filter(function (item) { return confirmItemsId.indexOf(item.id) > -1 });
                        
                for (var item in confirmItems) {
                    confirmItems[item]._clickAction = confirmItems[item].clickAction;
                    confirmItems[item].clickAction = function (p) {
                        var self = this;

                        self._clickAction(p);

                        if (!p || (p && p.itemData.format)) {
                            setTimeout(function () {
                                showActionConfirmationModal(self.id);
                            }, 2000);
                        }
                    };
                }
            }            

            if (customLayoutSectionVisible != null && !customLayoutSectionVisible) {
                // Set selected params
                var viewer = webDocumentViewer;
                var paramName;
                var paramValue;
                var params = viewer.parametersInfo.parameters.length;

                for (var param = 0; param < params; param++) {
                    paramName = viewer.parametersInfo.parameters[param].Name;
                    paramValue = paramsSelectedValues[paramName];
                    viewer.previewModel.parametersModel[paramName](paramValue);
                }

                // Display the report
                if (params) {
                    $('#' + viewer.name + ' div.dxrd-right-tabs div[title="' + viewer.previewModel.parametersModel.tabInfo.text + '"]').hide();
                    viewer.previewModel.exportModel.tabInfo.active(true);
                    viewer.previewModel.tabPanel.collapsed(true);
                    viewer.previewModel.parametersModel.submit();
                }
            }
        }

        function onCustomizeMenu(actions) {
            if (toolbarType == 1) {
                for (var action = 0; action < actions.length; action++) {
                    if (actions[action].id != DevExpress.Report.Preview.ActionId.Print &&
                        actions[action].imageClassName != 'dxrd-image-run-wizard' &&
                        actions[action].imageClassName != 'dxrd-image-exit') {
                        actions[action].visible = false;
                    } else if (actions[action].id == DevExpress.Report.Preview.ActionId.Print) {
                        actions[action].hasSeparator = false;
                    }
                }
            }
        }

        function onCustomizeParameter(parameter, info) {
            if (parameter.type == 'System.DateTime') {
                info.editor = { header: 'dx-date-simple' };                
            }
        }        

        function onChartControlInit(chartOptions) {
            initChartOptions(chartOptions ? JSON.parse(chartOptions) : null);
            initChartControl(chartOptions ? JSON.parse(chartOptions) : null);
        }

        function onChangeLayout() {
            var selectedIndex = layoutComboBox.GetSelectedIndex();

            if (selectedIndex > -1) {
                refreshCustomLayoutSection(true);
            }

            if (selectedIndex > 0) {
                $('#customLayoutDeleteButton').prop('disabled', false);
            } else {
                $('#customLayoutDeleteButton').prop('disabled', true);
            }
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
            saveCustomLayout();
        }

        function onCustomLayoutDeleteButtonClick() {
            deleteCustomLayout();
        }

        function onCustomLayoutPrintButtonClick() {
            printCustomLayout();
        }

        function onCustomLayoutExportButtonClick() {            
            var selectedType = $('li>a:hover').data('type');

            exportCustomLayout(selectedType);
        }

        function onCustomLayoutShowButtonClick() {
            showCustomLayoutSection(true);
        }

        function onExitButtonClick() {   
            window.history.back();
        }

        function onCustomCubePopupMenuItemClick(itemName, fieldId) {
            var isActionItem = itemName[0] != '#'; // No action sign.

            if (isActionItem) {
                var itemNameParts = itemName.split(':', 2);

                if (itemNameParts.length == 1) {
                    itemNameParts.push(item.GetChecked());
                }

                // Send command to server
                var commandName = '#menu';
                var commandParams = { 'FieldId': fieldId, 'Option': itemNameParts[0], 'Value': itemNameParts[1] };

                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams));                
            }
        }        

        function onCustomTableContextMenu(type, index, menu) {
            if (type == 'header') {
                customTableGridView.contextMenuColumnIndex = index;

                for (var optionName in customTableGridView.cpContextMenuOptions) {
                    menu.GetItemByName(optionName).SetChecked(
                        customTableGridView.cpContextMenuOptions[optionName].indexOf(-1) > -1 || // Grid level
                        customTableGridView.cpContextMenuOptions[optionName].indexOf(index) > -1); // Column level                    
                }

                for (var optionName in customTableGridView.cpContextMenuHiddenOptions) {
                    menu.GetItemByName(optionName).SetVisible(
                        customTableGridView.cpContextMenuHiddenOptions[optionName].indexOf(index) == -1);
                }
            } else {
                delete customTableGridView.contextMenuColumnIndex;
            }
        }

        function onCustomTableContextMenuItemClick(item) {
            var isCustomItem = JSON.stringify(item.menu.cpItemsCommands).indexOf(item.name) == -1;
            var isActionItem = item.name[0] != '#'; // No action sign.

            if (isCustomItem && isActionItem) {
                var itemNameParts = item.name.split(':', 2);

                if (itemNameParts.length == 1) {
                    itemNameParts.push(item.GetChecked());
                }

                // Send command to server
                var commandName = '#menu';
                var commandParams = { 'ColumnIndex': customTableGridView.contextMenuColumnIndex, 'Option': itemNameParts[0], 'Value': itemNameParts[1] };

                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams));                
            }
        }

        function saveCustomLayout() {
            // Validate required data
            if (ASPxClientEdit.ValidateGroup('LayoutGroup')) {
                // Send command to server
                var commandName = '#save';
                var commandParams = layoutComboBox.GetSelectedIndex() == -1 ? { 'LayoutName': layoutComboBox.GetValue() } : {};

                if (reportType == 3) { // Cube
                    customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                        if (customCubePivotGrid.cpLayoutSaved) {
                            if (customCubePivotGrid.cpNewReportUserId) {
                                var newIndex = layoutComboBox.AddItem(layoutComboBox.GetText(), customCubePivotGrid.cpNewReportUserId);

                                layoutComboBox.SetSelectedIndex(newIndex); // New layout
                                onChangeLayout();
                                delete customCubePivotGrid.cpNewReportUserId;
                            }

                            swal({
                                title: 'Success',
                                text: 'The template "' + layoutComboBox.GetText() + '" was successfully saved.',
                                type: 'success'
                            });
                            delete customCubePivotGrid.cpLayoutSaved;
                        }
                    });
                } else if (reportType == 4) { // Table
                    customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                        if (customTableGridView.cpLayoutSaved) {
                            if (customTableGridView.cpNewReportUserId) {
                                var newIndex = layoutComboBox.AddItem(layoutComboBox.GetText(), customTableGridView.cpNewReportUserId);

                                layoutComboBox.SetSelectedIndex(newIndex); // New layout
                                onChangeLayout();
                                delete customTableGridView.cpNewReportUserId;
                            }

                            swal({
                                title: 'Success',
                                text: 'The template "' + layoutComboBox.GetText() + '" was successfully saved.',
                                type: 'success'
                            });
                            delete customTableGridView.cpLayoutSaved;
                        }
                    });
                }
            }
        }

        function deleteCustomLayout() {
            var selectedItem = layoutComboBox.GetSelectedItem();

            if (selectedItem) {
                swal({
                    title: 'Are you sure?',
                    text: 'The template "' + selectedItem.text + '" will be deleted!',
                    type: 'warning',
                    confirmButtonColor: '#D9534F',
                    confirmButtonText: 'Yes',
                    closeOnConfirm: true,
                    showCancelButton: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        // Send command to server
                        var commandName = '#delete';
                        var commandParams = { 'ReportUserId': layoutComboBox.GetValue() };

                        if (reportType == 3) { // Cube
                            customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                                if (customCubePivotGrid.cpLayoutDeleted) {
                                    layoutComboBox.SetSelectedIndex(0); // Base layout
                                    layoutComboBox.RemoveItem(selectedItem.index);
                                    onChangeLayout();
                                    delete customCubePivotGrid.cpLayoutDeleted;
                                }
                            });
                        } else if (reportType == 4) { // Table
                            customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                                if (customTableGridView.cpLayoutDeleted) {
                                    layoutComboBox.SetSelectedIndex(0); // Base layout
                                    layoutComboBox.RemoveItem(selectedItem.index);
                                    onChangeLayout();
                                    delete customTableGridView.cpLayoutDeleted;
                                }
                            });
                        }
                    }
                });
            }
        }

        function printCustomLayout() {
            // Send command to server
            var commandName = '#print';
            var commandParams = {};

            if (reportType == 3) { // Cube
                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                    if (customCubePivotGrid.cpLayoutPrinted) {
                        showCustomLayoutSection(false);
                        delete customCubePivotGrid.cpLayoutPrinted;
                    }
                });
            } else if (reportType == 4) { // Table
                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                    if (customTableGridView.cpLayoutPrinted) {
                        showCustomLayoutSection(false);
                        delete customTableGridView.cpLayoutPrinted;
                    }
                });
            }
        }

        function exportCustomLayout(type) {
            // Send command to server
            var commandName = '#export';
            var commandParams = { 'Type': type };

            if (reportType == 3) { // Cube
                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                    if (customCubePivotGrid.cpLayoutExportedTo) {
                        window.open('Download.ashx?FileName=' + customCubePivotGrid.cpLayoutExportedTo);
                        delete customCubePivotGrid.cpLayoutExportedTo;
                    }
                });
            } else if (reportType == 4) { // Table
                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams), function () {
                    if (customTableGridView.cpLayoutExportedTo) {
                        window.open('Download.ashx?FileName=' + customTableGridView.cpLayoutExportedTo);
                        delete customTableGridView.cpLayoutExportedTo;
                    }
                });
            }
        }

        function refreshCustomLayoutSection(init) {
            // Process selected params            
            var viewer = webDocumentViewer;
            var paramName;
            var paramValue;
            var params = viewer.parametersInfo.parameters.length;

            paramsSelectedValues = {};

            for (var param = 0; param < params; param++) {
                paramName = viewer.parametersInfo.parameters[param].Name;
                paramValue = viewer.previewModel.parametersModel[paramName]();
                
                if (paramValue instanceof Date) { // Fix JS date issue when render as JSON string.
                    paramsSelectedValues[paramName] = new Date();
                    paramsSelectedValues[paramName].setTime(paramValue.getTime() + (paramValue.getTimezoneOffset() * (-1) * 60 * 1000));
                } else {
                    paramsSelectedValues[paramName] = paramValue;
                }
            }

            // Send command to server
            var commandName = init ? '#init' : '#load';
            var commandParams = init ?
                { 'ReportUserId': layoutComboBox.GetSelectedIndex() > -1 ? layoutComboBox.GetValue() : 0 } :
                { 'ReportParams': paramsSelectedValues }

            if (reportType == 3) { // Cube
                customCubePivotGrid.PerformCallback(commandName + JSON.stringify(commandParams));
            } else if (reportType == 4) { // Table
                customTableGridView.PerformCallback(commandName + JSON.stringify(commandParams));
            }
        }

        function showCustomLayoutSection(visible) {
            var viewer = webDocumentViewer;

            if (visible) {
                viewer.SetWidth(viewer.previewModel.tabPanel.headerWidth());

                viewer.previewModel.tabPanel.collapsed.subscribe(function (state) {
                    viewer.SetWidth(viewer.previewModel.tabPanel.headerWidth());
                });

                if (!viewer.previewModel.parametersModel.isEmpty()) {
                    // Override parameters commands
                    viewer.previewModel.parametersModel.submit = function () {
                        refreshCustomLayoutSection();
                    };

                    viewer.previewModel.parametersModel.restore = function () {
                        var model = this;

                        model._parameters.forEach(function (param) {
                            model._shouldProcessParameter(param) && (param.lookUpValues(param._originalLookUpValues), param.initialize(param._originalValue, model.parameterHelper))
                        });

                        refreshCustomLayoutSection();
                    };
                }

                if (reportType == 3) { // Cube
                    if (chartStatus == 0) { // None
                        $('#customLayoutSection > table:first-child > tbody > tr:first-child > td:nth-child(6)').hide();
                        $('#customLayoutSection > table:first-child > tbody > tr:first-child > td:nth-child(7)').hide();
                        customCubeWebChartControl.SetVisible(false);
                    } else if (chartStatus == 1) { // Hidden by default
                        customCubeWebChartControl.SetVisible(false);
                    }

                    customTableGridView.SetVisible(false);
                } else if (reportType == 4) { // Table
                    $('#customLayoutSection > table:first-child > tbody > tr:first-child > td:nth-child(6)').hide();
                    $('#customLayoutSection > table:first-child > tbody > tr:first-child > td:nth-child(7)').hide();
                    $('#customLayoutSection > table:nth-child(2)').hide();
                }

                $('#customLayoutSection').show();
                layoutComboBox.AdjustControl();

                if (viewer.previewModel.parametersModel.isEmpty()) {
                    viewer.SetVisible(false);
                } else {
                    $('#' + viewer.name + ' div.dxrd-right-tabs div[title="' + viewer.previewModel.parametersModel.tabInfo.text + '"]').show();
                    $('#' + viewer.name + ' div.dxrd-right-tabs div[title="' + viewer.previewModel.exportModel.tabInfo.text + '"]').hide();
                    $('#' + viewer.name + ' div.dxrd-right-tabs div[title="' + viewer.previewModel.searchModel.tabInfo.text + '"]').hide();
                    viewer.previewModel.parametersModel.tabInfo.active(true);
                }
            } else {
                $('#customLayoutSection').hide();
                viewer.SetVisible(true);
                webDocumentViewerCallbackPanel.PerformCallback();
            }

            customLayoutSectionVisible = visible;
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

                customCubeWebChartControl.SetVisible(chartOptions.Options.O5);
                customCubeWebChartControl.PerformCallback(commandName + JSON.stringify(commandParams));
            } else {
                customCubeWebChartControl.PerformCallback();
            }
        }

        function showActionConfirmationModal(actionId) {
            var messages = {};

            messages[DevExpress.Report.Preview.ActionId.Print] = 'Printarea a fost efectuata cu succes?';
            messages[DevExpress.Report.Preview.ActionId.PrintPage] = 'Printarea paginii a fost efectuata cu succes?';
            messages[DevExpress.Report.Preview.ActionId.ExportTo] = 'Exportul a fost efectuat cu succes?';

            swal({
                title: 'Confirmare', text: messages[actionId],
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da', cancelButtonText: 'Renunta', closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    // Send command to server
                    var reportPreview = webDocumentViewer.previewModel.reportPreview;
                    var paramsModel = webDocumentViewer.previewModel.parametersModel;
                    var validateParam = paramsModel._parameters.filter(function (param) { return param.path == 'Validate'; })[0];

                    validateParam.value(true);

                    if (!paramsModel.parametersLoading()) {
                        //paramsModel.parametersLoading(true);
                        var result = reportPreview.startBuild();

                        result && result.done(function () {
                            //paramsModel.parametersLoading(false);
                            validateParam.value(false);
                        });
                    }
                }
            });
        }

        function resizeGridView(gridView, offset, init) {
            var newHeight = getScreenHeight() - offset;

            if (init) {
                gridView.SetHeight(newHeight);
            } else {
                if (gridView.GetHeight() != newHeight) {
                    gridView.SetHeight(newHeight);
                }
            }
        }

        function getScreenHeight() {
            var height = 0;

            if (typeof (window.innerWidth) == 'number') {
                height = window.innerHeight;
            } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                height = document.documentElement.clientHeight;
            } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
                height = document.body.clientHeight;
            }

            return height;
        }
    </script>    

    <dx:ASPxGlobalEvents ID="globalEvents" runat="server">
        <ClientSideEvents ControlsInitialized="onControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>
