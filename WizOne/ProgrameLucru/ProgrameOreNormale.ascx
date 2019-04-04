<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameOreNormale.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameOreNormale" %>


<script type="text/javascript">
    function OnValueChangedHandlerOreNormale(s) {
        pnlCtlOreNormale.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>				

   <dx:ASPxCallbackPanel ID = "pnlCtlOreNormale" ClientIDMode="Static" ClientInstanceName="pnlCtlOreNormale" runat="server" OnCallback="pnlCtlOreNormale_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Setari</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblRotunjire"  Width="100"  runat="server"  Text="Rotunjire: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsRotunjire"  Width="200"  Value='<%#Eval("ONRotunjire") %>' ID="cmbRotunjire"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreNormale(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblONCamp" runat="server"  Width="120" Text="Transfer la: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTransfer"  Width="200"  Value='<%#Eval("ONCamp") %>' ID="cmbONCamp"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerOreNormale(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsRotunjire" TypeName="WizOne.Module.General" SelectMethod="ListaRotunjirePrgLucru" />
                      <asp:ObjectDataSource runat="server" ID="dsTransfer" TypeName="WizOne.Module.General" SelectMethod="GetPtj_AliasFOrdonat" />
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