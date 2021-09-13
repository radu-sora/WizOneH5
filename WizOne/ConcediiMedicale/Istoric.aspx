<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Istoric.aspx.cs" Inherits="WizOne.ConcediiMedicale.Istoric" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vizualizare CM luna anterioara</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<script language="javascript" type="text/javascript">

    window.onunload = function (e) {
        opener.PreluareCM(); 
    };

    window.onbeforeunload = function (e) {
        opener.PreluareCM();
    };

    function OnEndCallback(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }
    function OnPreluare(s, e) {
<%--        var nr = "<%=Session["ZileCMAnterior"] %>";
        if (nr == 0) {
            swal({
                title: "", text: "Datele din certificatul medical nu pot fi preluate!\nNu exista certificat medical in ultima zi a lunii anterioare!",
                type: "warning"
            });
            e.processOnServer = false;
        }
        else--%>
            e.processOnServer = true;
    }

</script>

<body>
    <form id="form1" runat="server">
        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxButton ID="btnPreluare" ClientInstanceName="btnPreluare" ClientIDMode="Static" runat="server" Text="Preluare" AutoPostBack="true" OnClick="btnPreluare_Click" oncontextMenu="ctx(this,event)" >
                       
                    </dx:ASPxButton>
                </td>
            </tr> 
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"  OnCustomCallback="grDate_CustomCallback" >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False"  />
                        <SettingsSearchPanel Visible="False" />    
                        <ClientSideEvents ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                        <Columns>	
                            <dx:GridViewCommandColumn Width="100px" ButtonType="Image" Caption="Selecteaza" ShowSelectCheckbox="true" SelectAllCheckboxMode="None" />
                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Visible="false" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="TipCM" Name="TipCM" Caption="TipCM" ReadOnly="true" Width="180px" />
                            <dx:GridViewDataTextColumn FieldName="DataStart" Name="DataStart" Caption="Data start" ReadOnly="true" Width="70px" />
                            <dx:GridViewDataTextColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" ReadOnly="true" Width="70px" />
                            <dx:GridViewDataTextColumn FieldName="ZileCalendaristice" Name="ZileCalendaristice" Caption="Nr. zile calendaristice" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="ZileLucratoare" Name="ZileLucratoare" Caption="Zile lucratoare" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="Suma" Name="Suma" Caption="Suma" ReadOnly="true" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="SerieNrCM" Name="SerieNrCM" Caption="Serie si nr. CM" ReadOnly="true" Width="120px" />
                            <dx:GridViewDataTextColumn FieldName="SerieNrCMInit" Name="SerieNrCMInit" Caption="Serie si nr. CM initial" ReadOnly="true" Width="120px" Visible="false" />
                            <dx:GridViewDataTextColumn FieldName="BCCM" Name="BCCM" Caption="Baza calcul CM" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="ZBCCM" Name="ZBCCM" Caption="Zile baza calcul CM" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="MediaZilnica" Name="MediaZilnica" Caption="Media zilnica" ReadOnly="true" Width="100px" />
                            <dx:GridViewDataTextColumn FieldName="MedieZileBazaCalcul" Name="MedieZileBazaCalcul" Caption="Media zile baza calcul" ReadOnly="true" Width="100px" />
						</Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
