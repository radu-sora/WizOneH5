<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Diverse.ascx.cs" Inherits="WizOne.Personal.Diverse" %>


<body>
    <table width="100%">
		<tr>
			<td align="left">					
			</td>			
		
		</tr>			
	</table>
				

   <dx:ASPxCallbackPanel ID = "Diverse_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlDiverse" runat="server" OnCallback="pnlCtlDiverse_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="Diverse_DataList" runat="server">
        <ItemTemplate>
			<div>
            <tr>  
                <td  valign="top">         
			      <fieldset  class="fieldset-auto-width">
				        <legend class="legend-font-size">Contract</legend>
				        <table width="60%">	
				            <tr>				
					            <td>		
						            <dx:ASPxLabel  ID="lblData" runat="server"  Text="Data"></dx:ASPxLabel >	
					            </td>
					            <td>	
						            <dx:ASPxDateEdit  ID="deData" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("FX1") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
						            </dx:ASPxDateEdit>										
					            </td>
				            </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNr"  Width="100"  runat="server"  Text="Nr."></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNr" Width="150"  runat="server" Text='<%# Eval("F10011") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNorma"  Width="100"  runat="server"  Text="Norma"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNorma" Width="150"  runat="server" Text='<%# Eval("F10043") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
				        </table>                    
			        </fieldset>
			      <fieldset  class="fieldset-auto-width">
				        <legend class="legend-font-size">Studii</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblLocNastere"  Width="100"  runat="server"  Text="Locul nasterii"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLocNastere" Width="150"  runat="server" Text='<%# Eval("F100980") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>
							        <dx:ASPxLabel  ID="lblStudii" Width="100" runat="server"  Text="Studii" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsStudii" Width="150"  Value='<%#Eval("F10050") %>' ID="cmbStudiiDiv"   runat="server" DropDownStyle="DropDown"  TextField="F71204" ValueField="F71202" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblStudiiDet"  Width="100"  runat="server"  Text="Studii detaliate"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtStudiiDet" Width="150"  runat="server" Text='<%# Eval("F100902") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>
							        <dx:ASPxLabel  ID="lblFunctie" Width="100" runat="server"  Text="Functie" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox  DataSourceID="dsFunctie" Width="150" Value='<%#Eval("F10071") %>' ID="cmbFunctieDiv"   runat="server" DropDownStyle="DropDown"  TextField="F71804" ValueField="F71802" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>					  
					        <tr>				
						        <td>
							        <dx:ASPxLabel  ID="lblNivel" Width="100" runat="server"  Text="Nivel" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsNivel" Width="150" Value='<%#Eval("F10029") %>' ID="cmbNivel"   runat="server" DropDownStyle="DropDown"  TextField="F71704" ValueField="F71702" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsStudii" TypeName="WizOne.Module.General" SelectMethod="GetStudii"/>
                      <asp:ObjectDataSource runat="server" ID="dsFunctie" TypeName="WizOne.Module.General" SelectMethod="GetFunctie"/>
                      <asp:ObjectDataSource runat="server" ID="dsNivel" TypeName="WizOne.Module.General" SelectMethod="GetMeserie"/>
			        </fieldset>
			      <fieldset  class="fieldset-auto-width">
				        <legend class="legend-font-size">Situatie CO</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblZileCOFidel"  Width="100"  runat="server"  Text="Zile CO fidelitate"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtZileCOFidel" Width="150"  runat="server" Text='<%# Eval("F100640") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblZileCOAnAnt"  Width="100"  runat="server"  Text="Zile CO an anterior"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtZileCOAnAnt" Width="150"  runat="server" Text='<%# Eval("F100996") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblZileCOCuvAnC"  Width="100"  runat="server"  Text="Zile CO cuvenite cf. grila"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtZileCOCuvAnC" Width="150"  runat="server" Text='<%# Eval("F100642") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblZileCOAnCrt" runat="server"  Text="Zile CO an curent" ></dx:ASPxLabel >	
						        </td>	
                                <td></td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtZileCOAnCrt" Width="75"  runat="server" Text='<%# Eval("F100995") %>' AutoPostBack="false" >
                                    
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVechimeComp"  Width="100"  runat="server"  Text="Vechime in companie"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVechimeComp" Width="150"  runat="server" Text='<%# Eval("F100643") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblVechimeCarteMunca"  Width="100"  runat="server"  Text="Vechime pe cartea de munca"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtVechimeCarteMunca" Width="150"  runat="server" Text='<%# Eval("F100644") %>' AutoPostBack="false" >
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
				        </table>                    
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