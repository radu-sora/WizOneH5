<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="RapoarteRevisal.aspx.cs" Inherits="WizOne.Revisal.RapoarteRevisal" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

    }
</script>
	<style type="text/css">
        .legend-border
        {
             border: 0;
        }
	</style>
    <body>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" ClientSideEvents-EndCallback="OnEndCallback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>

			<div>
                <tr>
                 <td  valign="top">
                   <fieldset  >
                    <legend class="legend-font-size">Lista Rapoarte Revisal</legend>
                    <table width="10%" >
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnRegSal" ClientInstanceName="btnRegSal" ClientIDMode="Static" runat="server" Text="Registru salariati" Width="10" Height="10" OnClick="btnRegSal_Click" oncontextMenu="ctx(this,event)">                        
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr> 
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnContrSal" ClientInstanceName="btnContrSal" ClientIDMode="Static" runat="server" Text="Contracte per salariat" Width="10" Height="10"  OnClick="btnContrSal_Click" oncontextMenu="ctx(this,event)">                    
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr> 
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnRapSal" ClientInstanceName="btnRapSal" ClientIDMode="Static" runat="server" Text="Raport pe salariat" Width="10" Height="10"  OnClick="btnRapSal_Click" oncontextMenu="ctx(this,event)">                                
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>                         
                    </table>
                  </fieldset >
                   <fieldset border="0">                     
                    <legend class="legend-border">Lista angajati</legend>            
                    <table width="10%" >
                        <tr>
                            <td align="center">

                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnCont" ClientInstanceName="btnCont" ClientIDMode="Static" runat="server" Text="Continua" Width="10" Height="10"  OnClick="btnCont_Click" oncontextMenu="ctx(this,event)" >                              
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnRen" ClientInstanceName="btnRen" ClientIDMode="Static" runat="server" Text="Renunta" Width="10" Height="10" OnClick="btnRen_Click" oncontextMenu="ctx(this,event)" >                                
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
  
                    </table>
                  </fieldset >
                </td> 
            </tr>      
		</div>
            </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

</asp:Content>