<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Avs.Detalii" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Detalii</title>

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
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%"  >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />    
                        <Columns>	
                            <dx:GridViewDataTextColumn FieldName="Coloana0" Name="Coloana0" Caption="Coloana0" ReadOnly="true" Width="50px" Visible="false"/>
                            <dx:GridViewDataTextColumn FieldName="Coloana1" Name="Coloana1" Caption="Coloana1" ReadOnly="true" Width="150px" />
                            <dx:GridViewDataTextColumn FieldName="Coloana2" Name="Coloana2" Caption="Coloana2" ReadOnly="true" Width="150px" />
						</Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
