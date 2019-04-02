<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MeniuDetaliu.aspx.cs" Inherits="WizOne.Pagini.MeniuDetaliu" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxTreeList.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxTreeList" tagprefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxHiddenField ID="txtMax" ClientInstanceName="txtMax" runat="server"></dx:ASPxHiddenField>
    <asp:HiddenField ID="txtMax2" runat="server" ClientIDMode="Static" />

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
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
    </table>
    <table>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblId" runat="server" Text="Id" Width="30px"></dx:ASPxLabel>
            </td>
            <td>
                <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"></dx:ASPxTextBox>
            </td>
            <td>
                <dx:ASPxLabel ID="lblDenumire" runat="server" Text="Denumire" Width="60px"></dx:ASPxLabel>
            </td>
            <td>
                <dx:ASPxTextBox ID="txtDenumire" runat="server" Width="350px"></dx:ASPxTextBox>
            </td>
            <td>
                <dx:ASPxLabel ID="lblActiv" runat="server" Text="Activ" Width="60px"></dx:ASPxLabel>
            </td>
            <td>
                <dx:ASPxCheckBox ID="chkActiv" runat="server" AllowGrayed="false" Checked="true" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <dx:ASPxLabel ID="lblDesc" runat="server" Text="Descriere" Width="100px"></dx:ASPxLabel>
                <dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="100px"></dx:ASPxMemo>
            </td>
        </tr>
    </table>

        <dx:ASPxTreeList ID="grDate" ClientInstanceName="grDate" ClientIDMode="Static" runat="server" AutoGenerateColumns="False" Width="100%" 
                KeyFieldName="IdMeniu" ParentFieldName="Parinte" OnInitNewNode="grDate_InitNewNode" OnNodeDeleting="grDate_NodeDeleting" OnNodeInserting="grDate_NodeInserting" OnNodeUpdating="grDate_NodeUpdating" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCommandColumnButtonInitialize="grDate_CommandColumnButtonInitialize" >
            <Settings GridLines="Both" />
            <SettingsBehavior ExpandCollapseAction="NodeDblClick" AutoExpandAllNodes="true" />
            <SettingsEditing Mode="Inline" AllowNodeDragDrop="true" AllowRecursiveDelete="true" ConfirmDelete="false" />
            <SettingsText ConfirmDelete ="Continuati procesul?" />

            <Columns>
                <dx:TreeListTextColumn FieldName="Id" Visible="false" />
                <dx:TreeListTextColumn FieldName="IdMeniu" Visible="false" VisibleIndex="0" />
                <dx:TreeListTextColumn FieldName="Nume" VisibleIndex="2" />
                <dx:TreeListTextColumn FieldName="Descriere" Visible="false" VisibleIndex="1" />

                <dx:TreeListComboBoxColumn FieldName="Imagine" VisibleIndex="3" >
                    <PropertiesComboBox ValueField="Denumire" TextField="Denumire" ImageUrlField="CaleImg" Width="300" DisplayImageSpacing="10px" DropDownStyle="DropDown">
                        <ItemStyle ImageSpacing="20px"></ItemStyle>
                    </PropertiesComboBox>
                </dx:TreeListComboBoxColumn>

                <dx:TreeListComboBoxColumn FieldName="IdNomen" Caption="Pagina" VisibleIndex="4" >
                    <PropertiesComboBox TextFormatString="{0} {1}" 
                         ValueField="Id" TextField="Pagina" ValueType="System.String" Width="300" DropDownWidth="600" DropDownStyle="DropDown">
                        <Columns>
                            <dx:ListBoxColumn FieldName="Nume" />
                            <dx:ListBoxColumn FieldName="Pagina" />
                            <dx:ListBoxColumn FieldName="Id" Visible="false" />
                            <dx:ListBoxColumn FieldName="Descriere" Visible="false" />
                        </Columns>
                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnStateChanged(s,e); }"></ClientSideEvents>
                    </PropertiesComboBox>
                </dx:TreeListComboBoxColumn>

                <dx:TreeListTextColumn FieldName="Ordine" Width="60" CellStyle-HorizontalAlign="Center" VisibleIndex="5" >
                    <CellStyle HorizontalAlign="Center"></CellStyle>
                </dx:TreeListTextColumn>
                <dx:TreeListCheckColumn FieldName="Stare" Width="60" CellStyle-HorizontalAlign="Center" VisibleIndex="6" >
                    <CellStyle HorizontalAlign="Center"></CellStyle>
                </dx:TreeListCheckColumn>
                <dx:TreeListCommandColumn ShowNewButtonInHeader="true" Width="100" VisibleIndex="7" >
                    <EditButton Visible="true">
                        <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                    </EditButton>
                    <NewButton Visible="true">
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                        <Styles>
                            <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                        </Styles>
                    </NewButton>
                    <DeleteButton Visible="True">
                        <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                    </DeleteButton>
                </dx:TreeListCommandColumn>
            </Columns>
        </dx:ASPxTreeList>

    <script type="text/javascript">
        function OnStateChanged(cmb) {
            var arr = cmb.GetSelectedItem();
            if (arr != null)
            {
                if (grDate.GetEditValue("Nume") == null || grDate.GetEditValue("Nume") == "") grDate.SetEditValue("Nume", arr.texts[0]);
                if (grDate.GetEditValue("Descriere") == null || grDate.GetEditValue("Descriere") == "") grDate.SetEditValue("Descriere", arr.texts[1]);
            }
        }

    </script>

</asp:Content>