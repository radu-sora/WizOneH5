<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameOutPeste.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameOutPeste" %>


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
							        <dx:ASPxTimeEdit  ID="deDifRapOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteDiferentaRaportare") %>' AutoPostBack="false"  >
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMinPlataOutPeste"  Width="120"  runat="server"  Text="Minim pt plata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="deMinPlataOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteMinPlata") %>' AutoPostBack="false"  >     
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblMaxPlatitOutPeste"  Width="120"  runat="server"  Text="Maximum platit"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="deMaxPlatitOutPeste" Width="60"  runat="server" Value='<%# Eval("OUTPesteMaxPlata") %>' AutoPostBack="false"  >  
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOutPeste(s); }" />
							        </dx:ASPxTimeEdit>
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