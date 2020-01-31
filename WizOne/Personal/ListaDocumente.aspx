<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaDocumente.aspx.cs" Inherits="WizOne.Personal.ListaDocumente" %>
     
<body>
    <form id="form1" runat="server">
        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" OnCustomCallback="grDate_CustomCallback" Width="100%">
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />    
                        <ClientSideEvents  
                            CustomButtonClick="function(s, e) {
                                onCustomButtonClick(s, e);
                            }" />
                        <Columns>	
                            <dx:GridViewCommandColumn Width="50px" VisibleIndex="0" ButtonType="Image"  Caption=" " Name="butoaneGrid" >
                                <CustomButtons>                
                                    <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                        <Image ToolTip="Arata" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                    </dx:GridViewCommandColumnCustomButton>                                    
                                    <dx:GridViewCommandColumnCustomButton ID="btnPrint">
                                        <Image ToolTip="Imprima" Url="~/Fisiere/Imagini/Icoane/print.png" />
                                    </dx:GridViewCommandColumnCustomButton>                               
                                </CustomButtons>
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Nr. Crt." ReadOnly="true" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" ReadOnly="true" Width="200px" />  
						</Columns>
                    </dx:ASPxGridView>                    
                </td>
            </tr>
        </table>

        <script>
            function onCustomButtonClick(s, e) {
                s.PerformCallback(JSON.stringify({
                    reportId: s.GetRowKey(e.visibleIndex),
                    command: e.buttonID
                }), function () {  
                    if (s.cpReportUrl) {
                        window.open(s.cpReportUrl);          
                        delete s.cpReportUrl;
                    }                 
                });
            }                 
        </script>
    </form>    
</body>

