﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameOutPeste.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameOutPeste" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnTextChangedHandlerOutPeste(s) {
        pnlCtlOutPeste.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerOutPeste(s) {
        pnlCtlOutPeste.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>

				

   <dx:ASPxCallbackPanel ID = "pnlCtlOutPeste" ClientIDMode="Static" ClientInstanceName="pnlCtlOutPeste" runat="server" OnCallback="pnlCtlOutPeste_Callback" SettingsLoadingPanel-Enabled="false">
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
							        <dx:ASPxLabel  ID="lblDifRapOutPeste"  Width="120"  runat="server"  Text="Diferenta raportare"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDifRapOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteDiferentaRaportare") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMinPlataOutPeste"  Width="120"  runat="server"  Text="Minim pt plata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deMinPlataOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteMinPlata") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMaxPlatitOutPeste"  Width="120"  runat="server"  Text="Maximum platit"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deMaxPlatitOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteMaxPlata") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
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
							        <dx:ASPxLabel  ID="lblTrimitePlatitOutPeste" runat="server"  Width="150" Text="Transfer timp platit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOutPeste"  Width="170"  Value='<%#Eval("OUTPesteCampPlatit") %>' ID="cmbOUTPesteCampPlatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOutPeste(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimiteNeplatitOutPeste" runat="server"  Width="150" Text="Transfer timp neplatit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOutPeste"  Width="170"  Value='<%#Eval("OUTPesteCampNeplatit") %>' ID="cmbOUTPesteCampNeplatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOutPeste(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsOutPeste" TypeName="WizOne.Module.General" SelectMethod="GetPtj_AliasFOrdonat" />
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