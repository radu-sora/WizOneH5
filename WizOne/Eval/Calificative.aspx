<%@ Page Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Calificative.aspx.cs" Inherits="WizOne.Eval.Calificative" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/sgSt.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="false" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/new.png" />
                    <ClientSideEvents Click="function(s, e) { OnNewClick(s, e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this, event)">
                    <ClientSideEvents Click="function(s, e){
                                    pnlLoading.Show();
                                    e.processOnServer = true;
                                }" />
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <table>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblId" runat="server" Text="Id" Width="30px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"  />
            </td>
            <td>
                <dx:ASPxLabel ID="lblCodSet" runat="server" Text="Cod Set:" Width="50px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtCodSet" runat="server" Width="150px" />
            </td>
            <td>
                <dx:ASPxLabel ID="lblDenSet" runat="server" Text="Set:" Width="50px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtDenSet" runat="server" Width="250px" />
            </td>
        </tr>
    </table>

    <table width="100%">
        <tr>
            <td colspan="5">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%"
                    AutoGenerateColumns="false" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize" 
                    OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomErrorText="grDate_CustomErrorText">
                    <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="true" HorizontalScrollBarMode="Auto" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents ContextMenu="ctx" />
                    <SettingsEditing Mode="Inline" />

                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption="" />
                        <dx:GridViewDataTextColumn FieldName="IdSet" Name="IdSet" Caption="IdSet" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IdCalificativ" Name="IdCalificativ" Caption="IdCalificativ" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" Width="250px" VisibleIndex="1" />
                        <dx:GridViewDataTextColumn FieldName="Nota" Name="Nota" Caption="Nota" Width="100px" VisibleIndex="2">
                            <PropertiesTextEdit DisplayFormatString="n">
                                <MaskSettings Mask="<0..999g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="RatingMin" Name="RatingMin" Caption="Rating Min" Width="100px" VisibleIndex="3">
                             <PropertiesTextEdit DisplayFormatString="n">
                                <MaskSettings Mask="<0..999g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="RatingMax" Name="RatingMax" Caption="Rating Max" Width="100px" VisibleIndex="4">
                            <PropertiesTextEdit DisplayFormatString="n">
                                <MaskSettings Mask="<0..999g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Ordine" Name="Ordine" Caption="Ordine" Width="100px" VisibleIndex="5">
                            <PropertiesTextEdit DisplayFormatString="n">
                                <MaskSettings Mask="<0..999g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Explicatii" Name="Explicatii" Caption="Explicatii" Width="400px" VisibleIndex="6" />
                    </Columns>

                    <SettingsCommandButton>
                        <UpdateButton>
                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </UpdateButton>
                        <CancelButton>
                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                        </CancelButton>

                        <EditButton>
                            <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                        <DeleteButton>
                            <Image Url="../Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                        </DeleteButton>
                    </SettingsCommandButton>

                </dx:ASPxGridView>
                <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid" />
            </td>
        </tr>
    </table>
</asp:Content>