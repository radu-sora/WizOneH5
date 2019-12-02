<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaDocumente.aspx.cs" Inherits="WizOne.Personal.ListaDocumente" %>


     <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnPrint":
                    grDate.PerformCallback(e.visibleIndex);
                    break;   
            }
        }
    </script>

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
                        <ClientSideEvents  CustomButtonClick="grDate_CustomButtonClick" />
                        <Columns>	
                            <dx:GridViewCommandColumn Width="50px" VisibleIndex="0" ButtonType="Image"  Caption=" " Name="butoaneGrid" >
                                <CustomButtons>                  
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


    </form>
</body>

