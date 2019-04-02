<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Istoric.aspx.cs" Inherits="WizOne.Avs.Istoric" %>
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
                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="Nr. Crt." ReadOnly="true" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="200px" />
                            <dx:GridViewDataTextColumn FieldName="NumeAtribut" Name="NumeAtribut" Caption="Atribut" ReadOnly="true" Width="120px" />
                            <dx:GridViewDataDateColumn FieldName="DataModif" Name="DataModif" Caption="Data Modificare" ReadOnly="true" Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataTextColumn FieldName="ValV" Name="ValV" Caption="Valoare veche" ReadOnly="true" Width="200px" />
                            <dx:GridViewDataTextColumn FieldName="ValN" Name="ValN" Caption="Valoare noua" ReadOnly="true" Width="200px" />
                            <dx:GridViewDataTextColumn FieldName="Explicatii" Name="Explicatii" Caption="Explicatii" ReadOnly="true" Width="300px" />
                            <dx:GridViewDataTextColumn FieldName="Utilizator" Name="Utilizator" Caption="Utilizator" ReadOnly="true" Width="120px" />
                            <dx:GridViewDataDateColumn FieldName="Data" Name="Data" Caption="Data" ReadOnly="true" Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataTextColumn FieldName="CRIPTAT" Name="CRIPTAT" Caption="CRIPTAT" ReadOnly="true" Width="10px" Visible="false" />
						</Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
