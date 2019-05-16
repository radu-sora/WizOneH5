<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WizOne.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        function Salveaza()
        {
            gridA.UpdateEdit();
            gridB.UpdateEdit();
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxButton ID="btnSave" runat="server" Text="Salveaza All" AutoPostBack="false">
        <ClientSideEvents Click="function(s,e) { Salveaza(); }" />
    </dx:ASPxButton>


    <dx:ASPxGridView ID="gridB" ClientInstanceName="gridB" runat="server" AutoGenerateColumns="false"
    OnBatchUpdate="grid2_BatchUpdate" OnRowInserting="gridB_RowInserting" OnRowUpdating="gridB_RowUpdating" OnRowDeleting="gridB_RowDeleting">
    <SettingsEditing Mode="Batch"></SettingsEditing>
    <Columns>
        <dx:GridViewCommandColumn ShowNewButtonInHeader="true" ShowDeleteButton="true" />
        <dx:GridViewDataColumn FieldName="Id" Caption="Id" />
        <dx:GridViewDataColumn FieldName="Denumire" Caption="Denumire" />
    </Columns>

</dx:ASPxGridView>


<dx:ASPxGridView ID="gridA" ClientInstanceName="gridA" runat="server" AutoGenerateColumns="false"
    OnBatchUpdate="grid1_BatchUpdate" OnRowInserting="gridA_RowInserting" OnRowUpdating="gridA_RowUpdating" OnRowDeleting="gridA_RowDeleting">
    <SettingsEditing Mode="Batch"></SettingsEditing>
    <Columns>
        <dx:GridViewCommandColumn ShowNewButtonInHeader="true" ShowDeleteButton="true" />
        <dx:GridViewDataColumn FieldName="Id" Caption="Id" />
        <dx:GridViewDataColumn FieldName="Denumire" Caption="Denumire" />
    </Columns>
</dx:ASPxGridView>


</asp:Content>
