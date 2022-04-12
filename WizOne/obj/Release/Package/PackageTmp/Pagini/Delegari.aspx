<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Delegari.aspx.cs" Inherits="WizOne.Pagini.Delegari" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
            <table width="100%">
                <tr>
                    <td align="left">
                        <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
                    </td>
                    <td align="right">
                        <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function (s, e) { OnNewClick(s, e); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                e.processOnServer = true;
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%" 
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" >
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                            <SettingsSearchPanel Visible="True" />
                            <ClientSideEvents ContextMenu="ctx" />
                            <SettingsEditing Mode="Inline" />

                            <Columns>
                                <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" VisibleIndex="0" SelectAllCheckboxMode="AllPages" />
                                <dx:GridViewDataComboBoxColumn FieldName="IdUser" Name="User" Caption="User" VisibleIndex="1">
                                    <PropertiesComboBox TextField="F70104" ValueField="F70102" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="IdDelegat" Name="Delegat" Caption="Delegat" VisibleIndex="2">
                                    <PropertiesComboBox TextField="F70104" ValueField="F70102" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="IdModul" Name="Modulul" Caption="Modulul" VisibleIndex="3">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                
                                <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" Width="80px" VisibleIndex="4" >
                                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                </dx:GridViewDataDateColumn>
                                <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" Width="80px" VisibleIndex="5" >
                                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                </dx:GridViewDataDateColumn>

                                <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                            </Columns>

                            <SettingsCommandButton>
                                <UpdateButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </UpdateButton>
                                <CancelButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                </CancelButton>

                                <EditButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </EditButton>
                                <DeleteButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                </DeleteButton>
                            </SettingsCommandButton>
                        </dx:ASPxGridView>
                        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid"></dx:ASPxGridViewExporter>
                    </td>
                </tr>
            </table>



</asp:Content>
