<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProfileDetaliu.aspx.cs" Inherits="WizOne.Pagini.ProfileDetaliu" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile Detaliu</title>

    <link type="text/css" rel="stylesheet" href="../fisiere/css/style.css" />
    <link type="text/css" rel="stylesheet" href="../fisiere/css/teme.css" />
    <link type="text/css" rel="stylesheet" href="../fisiere/css/widgets.css" />
    <script type="text/javascript" src="../Fisiere/js/utils.js"></script>

    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>
    <script>

        function OnNewClick(s, e) {
            grDate.AddNewRow();
            }
    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sm1" runat="server"></asp:ScriptManager>

        <div style="width:100%; text-align:right; margin-bottom:10px;">
            <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                <ClientSideEvents Click="function (s, e) { OnNewClick(s, e); }" />
                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" >
                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
            </dx:ASPxButton>
        </div>
        <%--  --%>
        <div class="div_hor">
            <label id="lblId" runat="server" style="padding-left:0;">Id</label>
            <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"></dx:ASPxTextBox>
            <label id="lblDenumire" runat="server">Denumire</label>
            <dx:ASPxTextBox ID="txtDenumire" runat="server" Width="275px"></dx:ASPxTextBox>
            <label id="lblActiv" runat="server">Activ</label>
            <dx:ASPxCheckBox ID="chkActiv" runat="server" Checked="true" AllowGrayed="false" EnableViewState="false"></dx:ASPxCheckBox>
            <label id="lblImplicit" runat="server">Implicit</label>
            <dx:ASPxCheckBox ID="chkImplicit" runat="server" Checked="true" AllowGrayed="false" EnableViewState="false"></dx:ASPxCheckBox>
        </div>

        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" AutoGenerateColumns="false" OnRowDeleting="grDate_RowDeleting" OnRowInserting="grDate_RowInserting"  OnInitNewRow="grDate_InitNewRow" >
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowColumnHeaders="true" />
            <SettingsSearchPanel Visible="False" />
            <Columns>
                <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataComboBoxColumn FieldName="IdGrup" Name="IdGrup" Caption="Grup" Visible="true">
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
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

                <DeleteButton>
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                </DeleteButton>
            </SettingsCommandButton>
        </dx:ASPxGridView>


    </form>
</body>
</html>
