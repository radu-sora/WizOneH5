<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateGenerale.ascx.cs" Inherits="WizOne.Personal.DateGenerale" %>



<script type="text/javascript">

    function OnTextChangedHandlerDG(s) {
        pnlCtlDateGen.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerDG(s) {
        pnlCtlDateGen.PerformCallback(s.name + ";" + s.GetValue());
    }

    function OnEndCallbackDG(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

</script>
<body>

			<table width="100%">
				<tr>
					<td align="left">					
					</td>			
		
				</tr>			
			</table>
				

   <dx:ASPxCallbackPanel ID = "pnlCtlDateGen" ClientIDMode="Static" ClientInstanceName="pnlCtlDateGen" runat="server" OnCallback="pnlCtlDateGen_Callback"  SettingsLoadingPanel-Enabled="false">
      <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackDG(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
        <asp:DataList ID="DateGenListView" runat="server">
         <ItemTemplate>
             <div>
              <tr>
                <td  valign="top">          
			      <fieldset >
				    <legend id="lgIdentGen" runat="server" class="legend-font-size">Date unice de identificare</legend>
				    <table id="lgIdentGenTable" runat="server" width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblMarca" runat="server"  Text="Marca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtMarca" runat="server" ReadOnly="true" AutoPostBack="false" ></dx:ASPxTextBox >
						</td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblCNP" runat="server"  Text="CNP/CUI"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxTextBox  ID="txtCNP" runat="server" Text='<%# Bind("F10017") %>'  AutoPostBack="false"  >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >					
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblCNPVechi" runat="server"  Text="CNP vechi"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtCNPVechi" runat="server" Text='<%# Bind("F100171") %>' AutoPostBack="false"  >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >										
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblEID" runat="server"  Text="EID"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtEID" runat="server" Text='<%# Bind("F100901") %>'  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >										
						</td>
					</tr>		
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblNrCtr" runat="server"  Text="Nr. contract"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtNrCtr" runat="server" Text='<%# Bind("F100985") %>'  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >										
						</td>
					</tr>                    			
				    </table>
			      </fieldset>             
			      <fieldset >
				    <legend id="lgNumeGen" runat="server" class="legend-font-size">Numele si prenumele</legend>
				    <table id="lgNumeGenTable" runat="server" width="60%">	
					<tr>				
						<td>	
							<dx:ASPxLabel  ID="lblNume" runat="server" Text="Nume"></dx:ASPxLabel >	
						</td>
						<td>		
							<dx:ASPxTextBox  ID="txtNumeDG" runat="server" Text='<%# Bind("F10008") %>'  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >						
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblPrenume" runat="server"  Text="Prenume"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtPrenumeDG" runat="server"  Text='<%# Bind("F10009") %>'  AutoPostBack="false"  >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >										
						</td>
					</tr>
					<tr>				
						<td>	
							<dx:ASPxLabel  ID="lblNumeAnt" runat="server" Text="Nume ant."></dx:ASPxLabel >	
						</td>
						<td>		
							<dx:ASPxTextBox  ID="txtNumeAntDG" runat="server" Text='<%# Bind("F100905") %>'  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxTextBox >						
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblPanaLa" runat="server"  Text="Pana la data"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="dePanaLa" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100906") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxDateEdit>										
						</td>
					</tr>  		
				    </table>        
			      </fieldset>
                </td>
                 <td  valign="top"> 
			      <fieldset>
				    <legend id="lgSexGen" runat="server" class="legend-font-size">Data nasterii si Sex</legend>
				    <table id="lgSexGenTable" runat="server" width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDataNasterii" Width="100" runat="server"  Text="Data nasterii" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxDateEdit  ID="deDataNasteriiDG"  Enabled="false" Width="100" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" runat="server" Value='<%# Bind("F10021") %>'  AutoPostBack="false" >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxDateEdit>
						</td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblSex" runat="server" Text="Sex" >
                                
                            </dx:ASPxLabel>
                        </td>


                        <td>
                            <dx:ASPxRadioButton ID="chkM" Width="75" runat="server" Text="Masculin" Enabled="false"  ClientInstanceName="chkbx1"
                                 GroupName="Sex">
                            </dx:ASPxRadioButton>
                        </td>
                        <td>
                            <dx:ASPxRadioButton ID="chkF"  Width="75" runat="server" Text="Feminin" Enabled="false" ClientInstanceName="chkbx2" 
                                 GroupName="Sex">
                            </dx:ASPxRadioButton>
                        </td>

					</tr>						
				    </table>
			      </fieldset>        
 			      <fieldset >
				    <legend id="lgAng" runat="server" class="legend-font-size">Date despre angajare</legend>			
				    <table id="lgAngTable" runat="server" width="60%">	

					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblStructOrg" runat="server"  Text="Alege structura organizatorica" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsStructOrg"  ID="cmbStructOrg" runat="server"   DropDownStyle="DropDown"  ValueField="IdAuto" ValueType="System.Int32"   AutoPostBack="false" OnInit="cmbStructOrg_Init">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="F00305" Caption="Subcompanie" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00304" Caption="IdSubcompanie" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="F00406" Caption="Filiala" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00405" Caption="IdFiliala" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="F00507" Caption="Sectie" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00506" Caption="IdSectie" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="F00608" Caption="Departament" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00607" Caption="IdDepartament" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="F00709" Caption="Subdepartament" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00708" Caption="IdSubdepartament" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="F00810" Caption="Birou" Width="130px" />
                                    <dx:ListBoxColumn FieldName="F00809" Caption="IdBirou" Width="130px" Visible="false"/>
                                    <dx:ListBoxColumn FieldName="IdAuto" Caption="NrCrt" Width="130px" Visible="false"/>
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDG(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>                           
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCompanie" runat="server"  Text="Companie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbCompanie" runat="server" DropDownStyle="DropDown" TextField="F00204" ValueField="F00202" ValueType="System.Int32" OnInit="cmbCompanie_Init"></dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblSubcompanie" runat="server"  Text="Subcompanie"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxComboBox DataSourceID="dsStructOrg"  Enabled="false"  ID="cmbSubcompanie" runat="server" DropDownStyle="DropDown" TextField="F00305" ValueField="F00304" ValueType="System.Int32" OnInit="cmbSubcompanie_Init"></dx:ASPxComboBox >					
						</td>
					</tr>
					<tr>				
						<td>	
							<dx:ASPxLabel  ID="lblFiliala" runat="server" Text="Filiala"></dx:ASPxLabel >	
						</td>
						<td>		
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbFiliala" runat="server" DropDownStyle="DropDown" TextField="F00406" ValueField="F00405" ValueType="System.Int32" OnInit="cmbFiliala_Init"></dx:ASPxComboBox >						
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblSectie" runat="server"  Text="Sectie"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbSectie" runat="server" DropDownStyle="DropDown" TextField="F00507" ValueField="F00506" ValueType="System.Int32" OnInit="cmbSectie_Init"></dx:ASPxComboBox >										
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDepartament" runat="server"  Text="Departament"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbDepartament" runat="server" DropDownStyle="DropDown"  TextField="F00608" ValueField="F00607" ValueType="System.Int32" OnInit="cmbDepartament_Init"></dx:ASPxComboBox >										
						</td>
					</tr>
                    
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblSubdept" runat="server"  Text="Subdepartament"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbSubdept" runat="server" DropDownStyle="DropDown"  TextField="F00709" ValueField="F00708" ValueType="System.Int32" OnInit="cmbSubdept_Init"></dx:ASPxComboBox >										
						</td>
					</tr>    
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblBirouEchipa" runat="server"  Text="Birou/Echipa"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsStructOrg" Enabled="false"  ID="cmbBirouEchipa" runat="server" DropDownStyle="DropDown"  TextField="F00810" ValueField="F00809" ValueType="System.Int32" OnInit="cmbBirouEchipa_Init"></dx:ASPxComboBox >										
						</td>
					</tr> 
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataAng" runat="server"  Text="Data angajarii"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataAngDG" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10022") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
							</dx:ASPxDateEdit>										
						</td>
					</tr>     
				    <tr>				
					    <td>		
						    <dx:ASPxLabel  ID="lblUltimaZiLucr" runat="server"  Text="Ultima zi plata"></dx:ASPxLabel >	
					    </td>
					    <td>	
						    <dx:ASPxDateEdit  ID="deUltimaZiLucrDG" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10023") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDG(s); }" />
						    </dx:ASPxDateEdit>										
					    </td>
				    </tr>
				    <tr>				
					    <td >
						    <dx:ASPxLabel  ID="lblMotivPlecare" Width="100" runat="server"  Text="Motiv plecare" ></dx:ASPxLabel >	
					    </td>	
					    <td>
						    <dx:ASPxComboBox DataSourceID="dsMP"  Value='<%#Eval("F10025") %>' ID="cmbMotivPlecareDG"   runat="server" DropDownStyle="DropDown"  TextField="F72104" ValueField="F72102" AutoPostBack="false"  ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDG(s); }" />
						    </dx:ASPxComboBox>
					    </td>
				    </tr>     
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblTimpPartial"  Width="100" runat="server"  Text="Timp partial" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox  DataSourceID="dsTP"  ID="cmbTimpPartialDG" Value='<%#Eval("F10043") %>' runat="server" TextField="Denumire" ValueField="Id"   AutoPostBack="false" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDG(s); }" />
							</dx:ASPxComboBox>
						</td>
                    </tr>     
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblNorma"  Width="100" runat="server"  Text="Norma" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsN"  Value='<%#Eval("F100973") %>' ID="cmbNormaDG" Width="100" runat="server"   TextField="Denumire" ValueField="Id"  AutoPostBack="false" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtr(s); }" />
							</dx:ASPxComboBox>
						</td>
                    </tr>   
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCategAng" runat="server"  Text="Categorie angajat 1" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCategAng_61"  Value='<%#Eval("F10061") %>' ID="cmbCategAng"   runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDG(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>                                                                  
				    </table>	  	
                    <asp:ObjectDataSource runat="server" ID="dsStructOrg" TypeName="WizOne.Module.General" SelectMethod="GetStructOrgModifGen" > 
                        <SelectParameters>
                             <asp:Parameter Name="data"  Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>  
                      <asp:ObjectDataSource runat="server" ID="dsTP" TypeName="WizOne.Module.General" SelectMethod="GetTimpPartial">
                            <SelectParameters>
                                    <asp:Parameter Name="tip"  Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>                       
                    <asp:ObjectDataSource runat="server" ID="dsMP" TypeName="WizOne.Module.General" SelectMethod="GetMotivPlecare"/>                    
                    <asp:ObjectDataSource runat="server" ID="dsN" TypeName="WizOne.Module.General" SelectMethod="GetNorma"/>
                    <asp:ObjectDataSource runat="server" ID="dsCategAng_61"  TypeName="WizOne.Module.General" SelectMethod="GetCategAng_61" />
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












