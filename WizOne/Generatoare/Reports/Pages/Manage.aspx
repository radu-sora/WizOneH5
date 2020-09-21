﻿<%@ Page Title="Manage Reports" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Wizrom.Reports.Pages.Manage" %>

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
                <dx:ASPxButton ID="ExitButton" runat="server" Text="Iesire" Image-Url="~/Fisiere/Imagini/Icoane/iesire.png" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) { ASPxClientUtils.DeleteCookie('ReportsGridViewCookies'); }" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <dx:ASPxGridView ID="ReportsGridView" ClientInstanceName="reportsGridView" runat="server" AutoGenerateColumns="False" Width="100%"
        DataSourceID="ReportsDataSource" KeyFieldName="Id"
        OnDataBinding="ReportsGridView_DataBinding">
        <Settings ShowFilterRow="True" VerticalScrollBarMode="Auto" ShowFilterRowMenu="true" ShowGroupPanel="True" />
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
            <dx:GridViewCommandColumn Caption=" " Name="butoaneGrid" ShowClearFilterButton="true" Width="60px">
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="ReportEditButton" Image-Url="~/Fisiere/Imagini/Icoane/edit.png" Image-ToolTip="Editare" Text=" " />
                    <dx:GridViewCommandColumnCustomButton ID="ReportDeleteButton" Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" Image-ToolTip="Stergere" Text=" " />
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Name" Caption="Denumire" Width="25%">
                <PropertiesTextEdit>
                    <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                        <RequiredField IsRequired="True" ErrorText="Denumirea este obligatorie" />
                    </ValidationSettings>
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Description" Caption="Descriere" />
            <dx:GridViewDataComboBoxColumn FieldName="TypeId" Caption="Tip raport" Width="150px">
                <PropertiesComboBox DataSourceID="ReportTypesDataSource" ValueField="ReportTypeId" TextField="Name">
                    <ValidationSettings Display="Dynamic" ErrorDisplayMode="Text" ErrorTextPosition="Bottom" SetFocusOnError="true">
                        <RequiredField IsRequired="True" ErrorText="Tipul raportului este obligatoriu" />
                    </ValidationSettings>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataComboBoxColumn FieldName="IdModul" Caption="Modul" Width="150px">
                <PropertiesComboBox  ValueField="Id" TextField="Denumire">     
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
                    onReportEditButtonClick(e.visibleIndex);
                } else if (e.buttonID == 'ReportDeleteButton') {
                    onReportDeleteButtonClick(e.visibleIndex);
                }
            }" />
    </dx:ASPxGridView>
                
    <asp:ObjectDataSource ID="ReportsDataSource" runat="server" TypeName="Wizrom.Reports.Pages.Manage" DataObjectTypeName="Wizrom.Reports.Pages.Manage+ReportViewModel"
        SelectMethod="GetReports" InsertMethod="AddReport" UpdateMethod="SetReport" DeleteMethod="DelReport">        
    </asp:ObjectDataSource>
    <ef:EntityDataSource ID="ReportTypesDataSource" runat="server" ContextTypeName="Wizrom.Reports.Models.ReportsEntities" EntitySetName="ReportTypes">
    </ef:EntityDataSource>
           
    <script>
        // Globals

        // Main functions
        $(window).on('load', function () {
            resizeGridView(reportsGridView, 170, true);
        });

        $(window).on('resize', function () {
            resizeGridView(reportsGridView, 170, false);
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
