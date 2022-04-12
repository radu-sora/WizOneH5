<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="SablonNomenRec.aspx.cs" Inherits="WizOne.Pagini.SablonNomenRec" %>




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
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%" AutoGenerateColumns="false" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize"
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomErrorText="grDate_CustomErrorText">
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="Control" />
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                            <SettingsSearchPanel Visible="True" />
                            <ClientSideEvents ContextMenu="ctx" />
                            <SettingsEditing Mode="Inline" />

                            <Columns>
                                <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
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
