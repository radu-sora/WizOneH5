<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameOreSup.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameOreSup" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnTextChangedHandlerOreSup(s) {
        pnlCtlOreSup.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerOreSup(s) {
        pnlCtlOreSup.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>

				

   <dx:ASPxCallbackPanel ID = "pnlCtlOreSup" ClientIDMode="Static" ClientInstanceName="pnlCtlOreSup" runat="server" OnCallback="pnlCtlOreSup_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Valori</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire" runat="server"  Width="70" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOSRotunjire"  Width="150"  Value='<%#Eval("OSRotunjire") %>' ID="cmbOSRotunjire"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreSup(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblValMin"  Width="70"  runat="server"  Text="Val. min.: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deValMin" Width="60"  runat="server" Value='<%# Eval("OSValMin") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOreSup(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblValMax"  Width="70"  runat="server"  Text="Val. max.: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deValMax" Width="60"  runat="server" Value='<%# Eval("OSValMax") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerOreSup(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsOSRotunjire" TypeName="WizOne.Module.General" SelectMethod="ListaRotunjirePrgLucru" />
			        </fieldset>
                </td>
                <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Transfer</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimite" runat="server"  Width="220" Text="Trimite la: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOSCamp"  Width="170"  Value='<%#Eval("OSCamp") %>' ID="cmbOSCamp"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreSup(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimiteSub" runat="server"  Width="220" Text="Trimite ce este sub val. min. la: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOSCamp"  Width="170"  Value='<%#Eval("OSCampSub") %>' ID="cmbOSCampSub"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreSup(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimitePeste" runat="server"  Width="220" Text="Trimite ce este peste val. max. la: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsOSCamp"  Width="170"  Value='<%#Eval("OSCampPeste") %>' ID="cmbOSCampPeste"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreSup(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsOSCamp" TypeName="WizOne.Module.General" SelectMethod="GetPtj_AliasFOrdonat" />
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