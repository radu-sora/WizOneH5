<%@ Page Title="Manage Reports" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Wizrom.Reports.Pages.Manage" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">   
    <!-- Modal dialogs -->    

    <!-- Page content -->
    <div class="page-content">
        <div class="page-content-header">
            <div>
                <dx:ASPxLabel ID="TitleLabel" runat="server" Text="Modifica sau creaza rapoarte noi" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </div>
            <div>
                <dx:ASPxButton ID="ReportNewButton" ClientIDMode="Static" ClientInstanceName="btnNew" runat="server" Text="Raport nou" Image-Url="~/Fisiere/Imagini/Icoane/adauga.png" AutoPostBack="false" CssClass="hidden-xs hidden-sm" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pageControl.onReportNewButtonClick();
                    }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="ReportViewButton" runat="server" Text="Afisare" Image-Url="~/Fisiere/Imagini/Icoane/arata.png" OnClick="ReportViewButton_Click" oncontextMenu="ctx(this,event)" />
                <dx:ASPxButton ID="ReportDesignButton" ClientIDMode="Static" ClientInstanceName="btnDesign" runat="server" Text="Design" Image-Url="~/Fisiere/Imagini/Icoane/schimba.png" CssClass="hidden-xs hidden-sm" OnClick="ReportDesignButton_Click" oncontextMenu="ctx(this,event)" />
                <dx:ASPxButton ID="ExitButton" runat="server" Text="Iesire" Image-Url="~/Fisiere/Imagini/Icoane/iesire.png" PostBackUrl="~/Pagini/MainPage.aspx" CssClass="hidden-xs hidden-sm" oncontextMenu="ctx(this,event)" />
            </div>        
        </div>        
        <div class="page-content-data invisible">
            <dx:ASPxGridView ID="ReportsGridView" ClientInstanceName="reportsGridView" runat="server" Width="100%" 
                CssClass="dx-grid-adaptive dx-grid-adaptive-hide-desktop-search dx-grid-adaptive-hide-header dx-grid-adaptive-hide-column1 dx-grid-adaptive-hide-column5"
                DataSourceID="ReportsDataSource" AutoGenerateColumns="False" KeyFieldName="Id"
                OnDataBinding="ReportsGridView_DataBinding">
                <Settings ShowFilterRow="True" VerticalScrollBarMode="Auto" />        
                <SettingsAdaptivity AdaptivityMode="HideDataCellsWindowLimit" AdaptiveDetailColumnCount="1" HideDataCellsAtWindowInnerWidth="1024" />
                <SettingsSearchPanel Visible="true" />
                <SettingsEditing Mode="Inline" />        
                <SettingsBehavior AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" AllowFocusedRow="true" />
                <SettingsPager Mode="ShowPager" />
                <SettingsCommandButton>
                    <ClearFilterButton Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Sterge filtre" Text=" " />
                    <UpdateButton Image-Url="~/Fisiere/Imagini/Icoane/salveaza.png" Image-ToolTip="Salveaza" Text=" " />
                    <CancelButton Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Renunta" Text=" " />
                </SettingsCommandButton>
                <Styles>
                    <Header Font-Bold="true" Wrap="True" />                    
                </Styles>                
                <Columns>
                    <dx:GridViewCommandColumn Caption=" " Name="butoaneGrid" ShowClearFilterButton="true" Width="60px">                        
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="ReportEditButton" Image-Url="~/Fisiere/Imagini/Icoane/edit.png" Image-ToolTip="Editare" Text=" " />
                            <dx:GridViewCommandColumnCustomButton ID="ReportDeleteButton" Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Stergere" Text=" " />
                        </CustomButtons>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="Name" Caption="Denumire" Width="250px">
                        <PropertiesTextEdit>
                            <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                                <RequiredField IsRequired="True" ErrorText="Denumirea este obligatorie" />
                            </ValidationSettings>
                        </PropertiesTextEdit>
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Description" Caption="Descriere" />
                    <dx:GridViewDataComboBoxColumn FieldName="TypeId" Caption="Tip raport" Width="150px">
                        <PropertiesComboBox DataSourceID="ReportTypesDataSource" ValueField="ReportTypeId" TextField="Name">
                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                            <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                                <RequiredField IsRequired="True" ErrorText="Tipul raportului este obligatoriu" />
                            </ValidationSettings>
                        </PropertiesComboBox>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataCheckColumn FieldName="Restricted" Caption="Parola" Width="70px">
                        <PropertiesCheckEdit DisplayTextChecked="Da" DisplayTextUnchecked="Nu" />
                    </dx:GridViewDataCheckColumn>

                    <dx:GridViewDataTextColumn FieldName="Id" Visible="false" ShowInCustomizationForm="false" />
                </Columns>        
                <ClientSideEvents
                    CustomButtonClick="function(s, e) {
                        if (e.buttonID == 'ReportEditButton') {
                            pageControl.onReportEditButtonClick(e.visibleIndex);
                        } else if (e.buttonID == 'ReportDeleteButton') {
                            pageControl.onReportDeleteButtonClick(e.visibleIndex);
                        }
                    }" />                
            </dx:ASPxGridView>    
        </div>        
    </div>
    
    <asp:ObjectDataSource ID="ReportsDataSource" runat="server" TypeName="Wizrom.Reports.Pages.Manage" DataObjectTypeName="Wizrom.Reports.Pages.Manage+ReportViewModel"
        SelectMethod="GetReports" InsertMethod="AddReport" UpdateMethod="SetReport" DeleteMethod="DelReport">        
    </asp:ObjectDataSource>
    <ef:EntityDataSource ID="ReportTypesDataSource" runat="server" ContextTypeName="Wizrom.Reports.Models.ReportsEntities" EntitySetName="ReportTypes">
    </ef:EntityDataSource>
           
    <script>   
        /* Page control */
        var pageControl = {
            /* Data */
            pageContent: null,
            /* Interface */
            init: function () {
                var self = this;

                self.pageContent = $('.page-content');
                ASPxClientControl.GetControlCollection().ControlsInitialized.AddHandler(function (s, e) {
                    self.onControlsInitialized(e);
                });                
            },            
            /* Events */
            onControlsInitialized: function (e) {
                if (!e.isCallback) { // Validate document ready
                    this.pageContent.find('> div[class*="invisible"]').removeClass('invisible'); // To hide DX controls UI init issues.
                }
            },            
            onReportNewButtonClick: function () {
                reportsGridView.AddNewRow();
            },
            onReportEditButtonClick: function (rowIndex) {
                reportsGridView.StartEditRow(rowIndex);
            },
            onReportDeleteButtonClick: function (rowIndex) {
                reportsGridView.GetRowValues(rowIndex, 'Name', function (value) {
                    swal({
                        title: 'Sunteti sigur/a ?', text: 'Raportul "' + value + '" va fi sters si nu va putea fi recuperat!',
                        type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, sterge!', cancelButtonText: 'Renunta', closeOnConfirm: true
                    }, function (isConfirm) {
                        if (isConfirm) {
                            reportsGridView.DeleteRow(rowIndex);
                        }
                    });
                });
            }
        };

        pageControl.init();
    </script>  
</asp:Content>
