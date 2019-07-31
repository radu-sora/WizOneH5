<%@ Page Title="Manage Reports" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="WizOne.Generatoare.Reports.Pages.Reports" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">   
    <!-- Modal dialogs -->    

    <!-- Page content -->
    <table style="width:100%; margin-bottom:10px">
        <tr>
            <td style="float:left">
                <dx:ASPxLabel ID="TitleLabel" runat="server" Text="Modifica sau creaza rapoarte noi" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td style="float:right">
                <dx:ASPxButton ID="ReportNewButton" ClientIDMode="Static" ClientInstanceName="btnNew" runat="server" Text="Raport nou" Image-Url="~/Fisiere/Imagini/Icoane/adauga.png" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        onReportNewButtonClick();
                    }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="ReportViewButton" runat="server" Text="Afisare" Image-Url="~/Fisiere/Imagini/Icoane/arata.png" OnClick="ReportViewButton_Click" oncontextMenu="ctx(this,event)" />
                <dx:ASPxButton ID="ReportDesignButton" ClientIDMode="Static" ClientInstanceName="btnDesign" runat="server" Text="Design" Image-Url="~/Fisiere/Imagini/Icoane/schimba.png" OnClick="ReportDesignButton_Click" oncontextMenu="ctx(this,event)" />
                <dx:ASPxButton ID="ExitButton" runat="server" Text="Iesire" Image-Url="~/Fisiere/Imagini/Icoane/iesire.png" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" />
            </td>
        </tr>
    </table>
    <dx:ASPxGridView ID="ReportsGridView" ClientInstanceName="reportsGridView" runat="server" AutoGenerateColumns="False" Width="100%"
        DataSourceID="ReportsDataSource" KeyFieldName="ReportId"
        OnDataBinding="ReportsGridView_DataBinding">
        <Settings ShowFilterRow="True" VerticalScrollBarMode="Auto" />
        <SettingsEditing Mode="Inline" />
        <SettingsBehavior AllowSelectByRowClick="true" AllowSelectSingleRowOnly="true" AllowFocusedRow="true" />
        <SettingsCommandButton>
            <ClearFilterButton Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Sterge filtre" Text=" " />
            <UpdateButton Image-Url="~/Fisiere/Imagini/Icoane/salveaza.png" Image-ToolTip="Salveaza" Text=" " />
            <CancelButton Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Renunta" Text=" " />
        </SettingsCommandButton>
        <Styles>
            <Header Font-Bold="true" Wrap="True" />
        </Styles>
        <Columns>
            <dx:GridViewCommandColumn ShowClearFilterButton="true" Caption=" " VisibleIndex="0" Width="50px" Name="butoaneGrid">
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="ReportEditButton" Image-Url="~/Fisiere/Imagini/Icoane/edit.png" Image-ToolTip="Editare" Text=" " />
                    <dx:GridViewCommandColumnCustomButton ID="ReportDeleteButton" Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Stergere" Text=" " />
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Name" Caption="Denumire" VisibleIndex="1" Width="25%">
                <PropertiesTextEdit>
                    <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                        <RequiredField IsRequired="True" ErrorText="Denumirea este obligatorie" />
                    </ValidationSettings>
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Description" Caption="Descriere" VisibleIndex="2" />
            <dx:GridViewDataComboBoxColumn FieldName="ReportTypeId" Caption="Tip raport" VisibleIndex="3" Width="150px">
                <PropertiesComboBox DataSourceID="ReportTypesDataSource" ValueField="ReportTypeId" TextField="Name">
                    <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                        <RequiredField IsRequired="True" ErrorText="Tipul raportului este obligatoriu" />
                    </ValidationSettings>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn> 
            <dx:GridViewDataTextColumn FieldName="ReportId" Visible="false" VisibleIndex="4" ShowInCustomizationForm="false" />
        </Columns>
        <ClientSideEvents
            CustomButtonClick="function(s, e) {
                if (e.buttonID == 'ReportEditButton') {
                    onReportEditButtonClick(e.visibleIndex);
                } else if (e.buttonID == 'ReportDeleteButton') {
                    onReportDeleteButtonClick(e.visibleIndex);
                }
            }" />
    </dx:ASPxGridView>
                
    <ef:EntityDataSource ID="ReportsDataSource" runat="server" ContextTypeName="WizOne.Generatoare.Reports.Models.ReportsEntities" EntitySetName="Reports" Include="ReportGroupUsers"
        EnableFlattening="False" EnableInsert="True" EnableUpdate="True" EnableDelete="True" 
        Where="it.ReportTypeId != 5 && EXISTS(SELECT 1 FROM it.ReportGroupUsers AS u WHERE u.UserId = @UserId)">  
        <WhereParameters>
            <asp:SessionParameter Name="UserId" Type="Int32" SessionField="UserId" />
        </WhereParameters>
    </ef:EntityDataSource>
    <ef:EntityDataSource ID="ReportTypesDataSource" runat="server" ContextTypeName="WizOne.Generatoare.Reports.Models.ReportsEntities" EntitySetName="ReportTypes"
        Where="it.ReportTypeId != 5">
    </ef:EntityDataSource>
           
    <script>
        // Globals

        // Main functions
        $(window).on('load', function () {
            resizeGridView(reportsGridView, 172, true);
        });

        $(window).on('resize', function () {
            resizeGridView(reportsGridView, 172, false);
        });

        function onReportNewButtonClick() {
            reportsGridView.AddNewRow();
        }

        function onReportEditButtonClick(visibleIndex) {
            reportsGridView.StartEditRow(visibleIndex);
        }

        function onReportDeleteButtonClick(visibleIndex) {
            reportsGridView.GetRowValues(visibleIndex, 'Name', function (value) {                
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Raportul "' + value + '" va fi sters si nu va putea fi recuperat!',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, sterge!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        reportsGridView.DeleteRow(visibleIndex);
                    }
                });
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
</asp:Content>
