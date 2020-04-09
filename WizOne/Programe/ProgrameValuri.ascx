<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameValuri.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameValuri" %>


<script type="text/javascript">
    function OnTextChangedHandlerPrgVal(s) {
        pnlCtlPrgVal.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerPrgVal(s) {
        pnlCtlPrgVal.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>				

   <dx:ASPxCallbackPanel ID = "pnlCtlPrgVal" ClientIDMode="Static" ClientInstanceName="pnlCtlPrgVal" runat="server" OnCallback="pnlCtlPrgVal_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Valori din pontaj</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVal0"  Width="50"  runat="server"  Text="Val0: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVal0" Width="300"  runat="server" Text='<%# Eval("Val0") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPrgVal(s); }" />
							        </dx:ASPxTextBox>
						        </td>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire0" runat="server"  Width="100" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("Val0Rot") %>' ID="cmb0"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerPrgVal(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVal1"  Width="50"  runat="server"  Text="Val1: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVal1" Width="300"  runat="server" Text='<%# Eval("Val1") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPrgVal(s); }" />
							        </dx:ASPxTextBox>
						        </td>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire1" runat="server"  Width="100" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("Val1Rot") %>' ID="cmb1"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerPrgVal(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVal2"  Width="50"  runat="server"  Text="Val2: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVal2" Width="300"  runat="server" Text='<%# Eval("Val2") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPrgVal(s); }" />
							        </dx:ASPxTextBox>
						        </td>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire2" runat="server"  Width="100" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("Val2Rot") %>' ID="cmb2"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerPrgVal(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVal3"  Width="50"  runat="server"  Text="Val3: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVal3" Width="300"  runat="server" Text='<%# Eval("Val3") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPrgVal(s); }" />
							        </dx:ASPxTextBox>
						        </td>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire3" runat="server"  Width="100" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("Val3Rot") %>' ID="cmb3"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerPrgVal(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVal4"  Width="50"  runat="server"  Text="Val4: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVal4" Width="300"  runat="server" Text='<%# Eval("Val4") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPrgVal(s); }" />
							        </dx:ASPxTextBox>
						        </td>				
						        <td >
							        <dx:ASPxLabel  ID="lblRotunjire4" runat="server"  Width="100" Text="Rotunjire: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("Val4Rot") %>' ID="cmb4"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerPrgVal(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsRotunjire" TypeName="WizOne.Module.General" SelectMethod="ListaRotunjirePrgLucru" />
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