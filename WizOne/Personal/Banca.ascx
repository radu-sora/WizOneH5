<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Banca.ascx.cs" Inherits="WizOne.Personal.Banca" %>


<body>
    <table width="100%">
		<tr>
			<td align="left">					
			</td>					
		</tr>			
	</table>
				

   <dx:ASPxCallbackPanel ID = "Banca_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlBanca" runat="server" OnCallback="pnlCtlBanca_Callback" SettingsLoadingPanel-Enabled="false">
      <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackBanca(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="Banca_DataList" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Cont salariu</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblIBANSal"  Width="100"  runat="server"  Text="IBAN"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtIBANSal" Width="250"  runat="server" Text='<%# Eval("F10020") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e) { ValidareIBAN(s,e); }" />
							        </dx:ASPxTextBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnContSal" ClientInstanceName="btnContSal"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickBanca(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnContSalIst" ClientInstanceName="btnContSalIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricBanca(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNrCard" Width="100" runat="server"  Text="Nr. card"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNrCard" Width="250"  runat="server" Text='<%# Eval("F10055") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblBancaSal" Width="100" runat="server"  Text="Banca" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsBanca" Width="250"  Value='<%#Eval("F10018") %>' ID="cmbBancaSal"   runat="server" DropDownStyle="DropDown"  TextField="F07509" ValueField="F07503" AutoPostBack="false"  ValueType="System.Int32" >
							            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerBanca(s); }" />
                                    </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblSucursalaSal" Width="100" runat="server"  Text="Sucursala" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox Width="250" Value='<%#Eval("F10019") %>' ID="cmbSucSal"   runat="server" DropDownStyle="DropDown"  TextField="F07505" ValueField="F07504" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsBanca" TypeName="WizOne.Module.General" SelectMethod="GetBanci"/>
			        </fieldset>
			      <fieldset >
				        <legend class="legend-font-size">Cont garantii</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblIBANGar"  Width="100"  runat="server"  Text="IBAN"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtIBANGar" Width="250"  runat="server" Text='<%# Eval("F1001028") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e) { ValidareIBAN(s,e); }" />
							        </dx:ASPxTextBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnContGar" ClientInstanceName="btnContGar"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                        <ClientSideEvents Click="function(s,e){ OnClickBanca(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnContGarIst" ClientInstanceName="btnContGarIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricBanca(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblBancaGar" Width="100" runat="server"  Text="Banca" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsBanca" Width="250"  Value='<%#Eval("F1001026") %>' ID="cmbBancaGar"   runat="server" DropDownStyle="DropDown"  TextField="F07509" ValueField="F07503" AutoPostBack="false"  ValueType="System.Int32" >
							            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerBanca(s); }" />
                                    </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblSucursalaGar" Width="100" runat="server"  Text="Sucursala" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox Width="250" Value='<%#Eval("F1001027") %>' ID="cmbSucGar"   runat="server" DropDownStyle="DropDown"  TextField="F07505" ValueField="F07504" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="ObjectDataSource1" TypeName="WizOne.Module.General" SelectMethod="GetBanci"/>
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


<script type="text/javascript">

    function OnClickBanca(s) {
        pnlLoading.Show();
        pnlCtlBanca.PerformCallback(s.name);
    }

    function GoToIstoricBanca(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnEndCallbackBanca(s, e) {

        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
        pnlLoading.Hide();
    }

    function ValidareIBAN(s, e) {
        if (s.GetText().length > 0) {
            if (s.GetText().length != 24) {
                swal({
                    title: "Atentie !", text: "Lungime cont IBAN invalida",
                    type: "warning"
                });
            }
            else {
                if (!IBAN.isValid(s.GetValue().toUpperCase()))
                    swal({ title: "Atentie !", text: "Cont IBAN invalid", type: "warning" });
            }
        }
    }

    function OnValueChangedHandlerBanca(s) {
        swal({ title: "Va rugam asteptati !", text: "Se incarca sucursalele", type: "warning" });
        pnlLoading.Show();
        pnlCtlBanca.PerformCallback(s.name + ";" + s.GetValue());
    }

</script>
