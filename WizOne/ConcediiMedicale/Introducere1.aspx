<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Introducere.aspx.cs" Inherits="WizOne.ConcediiMedicale.Introducere" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

</script>
	<style type="text/css">
		.fieldset-auto-width {
				display: inline-block;                         
		}
        .legend-font-size
        {
            font-size:15px;
            font-weight:bold;
        }
	</style>
    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>

			        <div>
                        <tr >
                             <td  valign="top">
			                  <fieldset class="fieldset-auto-width" >
				                <legend class="legend-font-size">Tip concediu</legend>
                                <table width="60%" >
                                    <tr>
                                        <td align="center">

                                        </td>
                                    </tr>    
                                </table>
                              </fieldset>
                               <fieldset class="fieldset-auto-width">                  
                                <legend class="legend-font-size">Concediu boala</legend>            
                                <table width="60%" >    
                                    <tr>
                                        <td align="center">

                                        </td>
                                    </tr>
  
                                </table>
                              </fieldset>
                               <fieldset class="fieldset-auto-width">                    
                                <legend class="legend-font-size">Detalii</legend>            
                                <table width="60%" >    
                                    <tr>
                                        <td align="center">

                                        </td>
                                    </tr>
  
                                </table>
                              </fieldset >
                            </td> 

                             <td  valign="top">
                               <fieldset class="fieldset-auto-width">
                                <legend class="legend-font-size">Cod indemnizatie</legend>
                                <table width="60%" >
                                    <tr>
                                        <td align="center">

                                        </td>
                                    </tr>    
                                </table>
                              </fieldset> 
                              <fieldset class="fieldset-auto-width">                                               
                                <legend class="legend-font-size">Cod transfer</legend>            
                                <table width="60%" >    
                                    <tr>
                                        <td align="center">

                                        </td>
                                    </tr>
  
                                </table>
                              </fieldset >
                              <fieldset class="fieldset-auto-width">                                               
                                <legend class="legend-font-size">Altele</legend>            
                                <table width="60%" >    
                                    <tr>
                                        <td align="center">

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