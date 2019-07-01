<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WizOne.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script>
        function SaveAll()
        {
            gridA.UpdateEdit();
            gridB.UpdateEdit();
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>

        <dx:ASPxButton ID="btnSave" runat="server" Text="Salveaza All" AutoPostBack="false">
            <ClientSideEvents Click="function(s,e) { SaveAll(); }" />
        </dx:ASPxButton>


        <dx:ASPxGridView ID="gridB" ClientInstanceName="gridB" runat="server" AutoGenerateColumns="false" Settings-ShowStatusBar="Hidden"
            OnBatchUpdate="grid2_BatchUpdate" OnRowInserting="gridB_RowInserting" OnRowUpdating="gridB_RowUpdating" OnRowDeleting="gridB_RowDeleting">
            <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
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
    
    </div>
    </form>
</body>
</html>
