<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm3.aspx.cs" Inherits="WizOne.WebForm3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
    <dx:ASPxGridView runat="server" ID="Grid" KeyFieldName="ID" OnRowUpdating="Grid_RowUpdating"
        OnParseValue="Grid_ParseValue" AutoGenerateColumns="False" ClientIDMode="AutoID"
        OnRowInserting="Grid_RowInserting" OnCellEditorInitialize="Grid_CellEditorInitialize">
        <Columns>
            <dx:GridViewDataTextColumn FieldName="Time" Width="150px">
                <PropertiesTextEdit Width="150px">
                    <MaskSettings Mask="<-100..100>\.<00..23>:<00..59>:<00..59>" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewCommandColumn ShowEditButton="true" ShowNewButton="True"/>
        </Columns>
        <SettingsPager Mode="ShowAllRecords">
        </SettingsPager>
    </dx:ASPxGridView>
        </div>
    </form>
</body>
</html>
