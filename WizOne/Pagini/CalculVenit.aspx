<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CalculVenit.aspx.cs" Inherits="WizOne.Pagini.CalculVenit" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

</script>

    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" ClientSideEvents-EndCallback="OnEndCallback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>
		
                 <tr align="left">
                         <td  valign="top">
                               <fieldset  class="fieldset-auto-width">                        
                                <table width="10%" >
                                    <tr>
      
                                    </tr>    
                                </table>
                              </fieldset >
                                            
                         </td>   
                         <td  valign="top">     
                             <fieldset class="fieldset-auto-width">
                                <table width="10%" >    
                                    <tr>
                                        <td align="center">
                 
                                        </td>
                                    </tr>
  
                                </table>
                              </fieldset >
                        </td> 
                  </tr>      
	
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

</asp:Content>