<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SituatieCO.ascx.cs" Inherits="WizOne.Personal.SituatieCO" %>




<body>
    <td>
        <tr halign="center">
            <dx:ASPxButton ID="btnCalc" ClientInstanceName="btnCalc" ClientIDMode="Static" runat="server" Text="Calcul CO" OnClick="btnCalc_Click" AutoPostBack="true"  >
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnCalcSI" ClientInstanceName="btnCalcSI" ClientIDMode="Static" runat="server" Text="Calcul SI" OnClick="btnCalcSI_Click" AutoPostBack="true"  >
            </dx:ASPxButton>
        </tr>
        <tr>
        <dx:ASPxGridView ID="grDateSituatieCO" runat="server" ClientInstanceName="grDateSituatieCO" ClientIDMode="Static" Width="60%" AutoGenerateColumns="false"  >
            <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
            <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />    
            <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

        </dx:ASPxGridView>
        </tr>
    </td>

</body>