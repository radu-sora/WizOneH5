<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Istoric.aspx.cs" Inherits="WizOne.Pagini.Istoric" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Istoric</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"  >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="Nume" Name="Nume" Caption="Nume" ReadOnly="true" Width="200px" />
                            <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare" ReadOnly="true" Width="100px" Visible="false" />
                            <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="50px" >
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataDateColumn FieldName="DataAprobare" Name="DataAprobare" Caption="DataAprobare" ReadOnly="true" Width="50px" >
                                <PropertiesDateEdit EditFormat="DateTime" DisplayFormatString="dd/MM/yyyy HH:mm:ss"/>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="Inlocuitor" Name="Inlocuitor" Caption="Inlocuitor" ReadOnly="true" Width="150px" />                            
                        </Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
