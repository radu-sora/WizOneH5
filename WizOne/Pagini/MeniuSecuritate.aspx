<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MeniuSecuritate.aspx.cs" Inherits="WizOne.Pagini.MeniuSecuritate" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
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
    </table>
    <table>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblId" runat="server" Text="Grup de angajati" Width="130px"></dx:ASPxLabel>
            </td>
            <td>
                <dx:ASPxComboBox ID="cmbGr" runat="server" ClientInstanceName="cmbGr" ClientIDMode="Static" OnSelectedIndexChanged="cmbGr_SelectedIndexChanged" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="true" />
            </td>
        </tr>
    </table>
    
    <dx:ASPxTreeList ID="grDate" ClientInstanceName="grDate" ClientIDMode="Static" runat="server" AutoGenerateColumns="False" Width="100%" Height="100%" KeyFieldName="IdMeniu" ParentFieldName="Parinte" >
        <Settings GridLines="Both" />
        <SettingsBehavior ExpandCollapseAction="NodeDblClick" AutoExpandAllNodes="true" />
        <SettingsEditing Mode="Inline" AllowNodeDragDrop="false" AllowRecursiveDelete="false" ConfirmDelete="false" />
        <SettingsSelection Enabled="True" AllowSelectAll="true" />

        <Columns>
            <dx:TreeListTextColumn FieldName="IdMeniu" Visible="false" />
            <dx:TreeListTextColumn FieldName="Nume" VisibleIndex="2" />
            <dx:TreeListTextColumn FieldName="Bifat" Visible="false" />
        </Columns>
    </dx:ASPxTreeList>


</asp:Content>