<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WizOne.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="true"  >
                <SettingsEditing Mode="Batch"/>

                <Columns>
                    <dx:GridViewDataTextColumn FieldName="Id" ReadOnly="false" Visible="true" ShowInCustomizationForm="false" />
                    <dx:GridViewDataTextColumn FieldName="Denumire" ReadOnly="false" Visible="true" ShowInCustomizationForm="false" />
                    <dx:GridViewDataTextColumn FieldName="Descriere" UnboundType="String" Width="150px" Caption="test"></dx:GridViewDataTextColumn>
                    <dx:GridViewDataTimeEditColumn FieldName="NrOre1_Tmp" Name="NrOre1_Tmp" Caption="NrOre2" Width="100px" Visible="true" ReadOnly="false"></dx:GridViewDataTimeEditColumn>
                    <dx:GridViewDataTextColumn FieldName="Id_Tmp" ReadOnly="false" Visible="true" ShowInCustomizationForm="false" />
                </Columns>

            </dx:ASPxGridView>
        </div>
    </form>
</body>
</html>
