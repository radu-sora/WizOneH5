<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MeniuDetaliu.aspx.cs" Inherits="WizOne.Pagini.MeniuDetaliu" %>

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

            <dx:TreeListComboBoxColumn FieldName="Imagine" Width="150" VisibleIndex="3">
                <PropertiesComboBox ValueField="Denumire" TextField="Denumire" ImageUrlField="CaleImg" DropDownStyle="DropDown" DisplayImageSpacing="10px">
                    <ItemStyle ImageSpacing="10px" />
                    <ItemImage Width="16" Height="16" />
                </PropertiesComboBox>
            </dx:TreeListComboBoxColumn>

            <dx:TreeListComboBoxColumn FieldName="IdNomen" Caption="Pagina" VisibleIndex="4">
                <CellStyle HorizontalAlign="Left"></CellStyle>
                <PropertiesComboBox ValueField="Id" ValueType="System.String" TextFormatString="{0} ({1})" DropDownStyle="DropDown" DropDownWidth="600">
                    <Columns>
                        <dx:ListBoxColumn FieldName="Nume" />
                        <dx:ListBoxColumn FieldName="Pagina" />
                        <dx:ListBoxColumn FieldName="Id" Visible="false" />
                        <dx:ListBoxColumn FieldName="Descriere" Visible="false" />
                    </Columns>
                    <ClientSideEvents SelectedIndexChanged="function(s, e) { OnStateChanged(s,e); }"></ClientSideEvents>
                </PropertiesComboBox>
            </dx:TreeListComboBoxColumn>

            <dx:TreeListTextColumn FieldName="Ordine" Width="1" VisibleIndex="5" >
                <CellStyle HorizontalAlign="Center"></CellStyle>
            </dx:TreeListTextColumn>
            <dx:TreeListCheckColumn FieldName="Stare" Width="1" VisibleIndex="6" />
            <dx:TreeListCheckColumn FieldName="StareMobil" Width="1" VisibleIndex="7" />
            <dx:TreeListTextColumn FieldName="NumeMobil" Width="100" VisibleIndex="8">
                <PropertiesTextEdit MaxLength="20" />
            </dx:TreeListTextColumn>
            <dx:TreeListTextColumn FieldName="OrdineMobil" Width="1" VisibleIndex="9" >
                <CellStyle HorizontalAlign="Center"></CellStyle>
            </dx:TreeListTextColumn>

            <dx:TreeListCommandColumn ShowNewButtonInHeader="true" Width="100" VisibleIndex="10" >
                <EditButton Visible="true" Image-Url="~/Fisiere/Imagini/Icoane/edit.png" />                        
                <NewButton Visible="true" Image-Url="~/Fisiere/Imagini/Icoane/New.png" />                        
                <DeleteButton Visible="true" Image-Url="~/Fisiere/Imagini/Icoane/sterge.png" />                        
                <UpdateButton Image-Url="~/Fisiere/Imagini/Icoane/salveaza.png" />                        
                <CancelButton Image-Url="~/Fisiere/Imagini/Icoane/renunta.png" />                         
            </dx:TreeListCommandColumn>
        </Columns>
    </dx:ASPxTreeList>

    <script type="text/javascript">
        function OnStateChanged(cmb) {
            var arr = cmb.GetSelectedItem();
            if (arr != null)
            {
                if (grDate.GetEditValue("Nume") == null || grDate.GetEditValue("Nume") == "") {
                    //grDate.SetEditValue("Nume", arr.texts[0]);
                    //Radu 20.01.2021 - din vers 20 DevExpress nu mai merge cum trebuie SetEditValue
                    var nume = grDate.GetEditor("Nume");
                    nume.SetValue(arr.texts[0]);
                }
                if (grDate.GetEditValue("Descriere") == null || grDate.GetEditValue("Descriere") == "") {
                    //grDate.SetEditValue("Descriere", arr.texts[1]);
                    var desc = grDate.GetEditor("Descriere");
                    desc.SetValue(arr.texts[1]);
                }
            }
        }

    </script>

</asp:Content>