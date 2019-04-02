<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameInSub.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameInSub" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnTextChangedHandlerInSub(s) {
        pnlCtlInSub.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerInSub(s) {
        pnlCtlInSub.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>

				

   <dx:ASPxCallbackPanel ID = "pnlCtlInSub" ClientIDMode="Static" ClientInstanceName="pnlCtlInSub" runat="server" OnCallback="pnlCtlInSub_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Setari</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDifRap"  Width="120"  runat="server"  Text="Diferenta raportare"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDifRap" Width="60"  runat="server" Value='<%# Eval("INSubDiferentaRaportare") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerInSub(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMinPlata"  Width="120"  runat="server"  Text="Minim pt plata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deMinPlata" Width="60"  runat="server" Value='<%# Eval("INSubMinPlata") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerInSub(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMaxPlatit"  Width="120"  runat="server"  Text="Maximum platit"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deMaxPlatit" Width="60"  runat="server" Value='<%# Eval("INSubMaxPlata") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerInSub(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
				        </table>
			        </fieldset>
                </td>
                <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Transfer</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimitePlatit" runat="server"  Width="150" Text="Transfer timp platit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsInSub"  Width="170"  Value='<%#Eval("INSubCampPlatit") %>' ID="cmbINSubCampPlatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerInSub(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimiteNeplatit" runat="server"  Width="150" Text="Transfer timp neplatit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsInSub"  Width="170"  Value='<%#Eval("INSubCampNeplatit") %>' ID="cmbINSubCampNeplatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerInSub(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsInSub" TypeName="WizOne.Module.General" SelectMethod="GetPtj_AliasFOrdonat" />
			        </fieldset>
                 </td> 
   
                </tr>                     
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
</body>