<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Cadru.Master" CodeBehind="ActivitatiObiective.aspx.cs" Inherits="WizOne.Eval.ActivitatiObiective" %>


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
                <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { OnNewClick(s, e); }" />
                    <Image Url="../Fisiere/Imagini/Icoane/new.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e){
                        pnlLoading.Show();
                        e.processOnServer = true;
                        }" />
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
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
                <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false" />
            </td>
            <td>
                <dx:ASPxLabel ID="lblObiectiv" runat="server" Text="Obiectiv" Width="50px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtObiectiv" runat="server" Width="250px" Enabled="true" />
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
                    <Settings ShowFilterRow="True" ShowGroupPanel="true" HorizontalScrollBarMode="Auto" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents ContextMenu="ctx" />
                    <SettingsEditing Mode="Inline" />

                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="IdActivitate" Name="IdActivitate" Caption="IdActivitate" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IdObiectiv" Name="IdObiectiv" Caption="IdObiectiv" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="Activitate" Name="Activitate" Caption="Activitate" Width="550px" VisibleIndex="1" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" />
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