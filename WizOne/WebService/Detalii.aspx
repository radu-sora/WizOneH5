<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.WebService.Detalii" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Detalii ordin</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Export" AutoPostBack="true" OnClick="btnExport_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/ExportToXls.png"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"  >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="true" />
                        <Settings ShowFilterRow="true" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />    
                        <Columns>	
                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="NumeCompanie" Name="NumeCompanie" Caption="Nume companie" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="NumeTichet" Name="NumeTichet" Caption="Nume tichet" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataComboBoxColumn FieldName="NumePersContact" Name="NumePersContact" Caption="Nume pers. contact" ReadOnly="true" Width="100px" >
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                </PropertiesComboBox>
                             </dx:GridViewDataComboBoxColumn>  
                            <dx:GridViewDataTextColumn FieldName="CodAdresaWiz" Name="CodAdresaWiz" Caption="Cod adresa Wiz" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="NumeAng" Name="NumeAng" Caption="Nume angajat" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="PrenumeAng" Name="PrenumeAng" Caption="Prenume angajat" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="CNP" Name="CNP" Caption="CNP" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="NrTichete" Name="NrTichete" Caption="Nr. tichete" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="Departament" Name="Departament" Caption="Departament" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="Responsabil" Name="Responsabil" Caption="Responsabil" ReadOnly="true" Width="100px" />
						</Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
