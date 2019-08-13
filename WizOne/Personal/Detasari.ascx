<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detasari.ascx.cs" Inherits="WizOne.Personal.Detasari" %>

<body>

   <dx:ASPxCallbackPanel ID = "Detasari_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlDet" runat="server" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>

    <asp:DataList ID="Detasari_DataList" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend id="lgDet" runat="server" class="legend-font-size">Detasari</legend>
				        <table id="lgDetTable" runat="server" width="60%">	
					        <tr>	
						        <td>
							        <dx:ASPxLabel  ID="lblNumeAngajator" runat="server"  Text="Nume angajator" Width="120" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeAngajator" runat="server" Text='<%# Bind("F100918") %>'  AutoPostBack="false" >
							        </dx:ASPxTextBox >
						        </td>
                                <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>
							        <dx:ASPxLabel  ID="lblCUI" runat="server"  Text="CUI" Width="80" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCUI" runat="server" Text='<%# Bind("F100919") %>'  AutoPostBack="false" Width="100" >
							        </dx:ASPxTextBox >
						        </td> 
                                <td><label style="display:inline-block;">&nbsp; </label></td>                                                                			
						        <td>
							        <dx:ASPxLabel  ID="lblNationalitate" Width="100" runat="server"  Text="Nationalitate" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsNationalitate" Width="120"  Value='<%#Eval("F100920") %>' ID="cmbNationalitate"   runat="server" DropDownStyle="DropDown"  TextField="F73304" ValueField="F73302" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>                
					        </tr>
                            <tr>
						        <td>	
							        <dx:ASPxLabel  ID="lblDataInceputDet" runat="server"  Text="Data inceput" Width="120"></dx:ASPxLabel >	
						        </td>
						        <td>			
							        <dx:ASPxDateEdit  ID="deDataInceputDet" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100915") %>'  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>					
						        </td>
					            <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>		
							        <dx:ASPxLabel  ID="lblDataSfarsitDet" runat="server"  Text="Data sfarsit" Width="80"></dx:ASPxLabel >	
						        </td>
						        <td>			
							        <dx:ASPxDateEdit  ID="deDataSfarsitDet" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100916") %>'  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>					
						        </td>
					            <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>		
							        <dx:ASPxLabel  ID="lblDataIncetareDet" runat="server"  Text="Data incetare" Width="100"></dx:ASPxLabel >	
						        </td>
						        <td>			
							        <dx:ASPxDateEdit  ID="deDataIncetareDet" Width="120" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100917") %>'  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>					
						        </td>
					        </tr>

				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsNationalitate" TypeName="WizOne.Module.General" SelectMethod="GetF733"/>
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>

    <dx:ASPxGridView ID="grDateDetasari" runat="server" ClientInstanceName="grDateDetasari" ClientIDMode="Static" Width="50%" AutoGenerateColumns="false"  OnDataBinding="grDateDetasari_DataBinding" >
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
        <Columns>
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F11204" Name="F11204" Caption="Nume angajator"  Width="150px"/>
            <dx:GridViewDataTextColumn FieldName="F11205" Name="F11205" Caption="CUI"  Width="100px"/>
            <dx:GridViewDataComboBoxColumn FieldName="F11206" Name="F11206" Caption="Nationalitate" ReadOnly="true" Width="250px" >
                <PropertiesComboBox TextField="F73304" ValueField="F73302" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
                                                                                        
            <dx:GridViewDataDateColumn FieldName="F11207" Name="F11207" Caption="Data inceput"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="F11208" Name="F11208" Caption="Data sfarsit"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="F11209" Name="F11209" Caption="Data incetare"  Width="100px" >                      
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
              
        </Columns>
    </dx:ASPxGridView>

       <dx:ASPxCallbackPanel ID = "pnlCtlMutare" ClientIDMode="Static" ClientInstanceName="pnlCtlMutare" runat="server" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
            <asp:DataList ID="Mutare_DataList" runat="server">
                <ItemTemplate>
			        <div>
                    <tr>
                    <td  valign="top">

			      <fieldset >
				        <legend id="lgMutare" runat="server" class="legend-font-size">Export mutare</legend>
				        <table id="lgMutareTable" runat="server" width="60%">	
					        <tr>	
						        <td>	
							        <dx:ASPxLabel  ID="lblDataMutare" runat="server"  Text="Data mutare" Width="120"></dx:ASPxLabel >	
						        </td>
						        <td>			
							        <dx:ASPxDateEdit  ID="deDataMutare" Width="130" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001072") %>'  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>					
						        </td>
                                <td><label style="display:inline-block;">&nbsp; </label></td>  
						        <td>
							        <dx:ASPxLabel  ID="lblNumeAngExp" runat="server"  Text="Nume angajator" Width="100" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeAngExp" runat="server" Text='<%# Bind("F1001073") %>'  Width="120"  AutoPostBack="false" >
							        </dx:ASPxTextBox >
						        </td>
                                <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>
							        <dx:ASPxLabel  ID="lblTemeiLegal" Width="80" runat="server"  Text="Temei legal" ></dx:ASPxLabel >	
						        </td>	
					            <td>
							        <dx:ASPxTextBox  ID="txtTemeiLegal" runat="server" Text='<%# Bind("F1001075") %>'  AutoPostBack="false" >
							        </dx:ASPxTextBox >
						        </td>    
					        </tr>
                            <tr>
					        <td>
							        <dx:ASPxLabel  ID="lblMutareExp" Width="100" runat="server"  Text="Tip mutare" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTipMutare" Width="130"  Value='<%#Eval("F1001076") %>' ID="cmbMutareExp"   runat="server" DropDownStyle="DropDown"  TextField="F73703" ValueField="F73702" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td><label style="display:inline-block;">&nbsp; </label></td>    
					            <td>
							        <dx:ASPxLabel  ID="lblCUIExp" runat="server"  Text="CUI" Width="100" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCUIExp" runat="server" Text='<%# Bind("F1001074") %>'  AutoPostBack="false" Width="120" >
							        </dx:ASPxTextBox >
						        </td> 
					        </tr>

				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsTipMutare" TypeName="WizOne.Module.General" SelectMethod="GetF737"/>
			        </fieldset>
			      <fieldset >
				        <legend id="lgPrel" runat="server" class="legend-font-size">Preluare mutare</legend>
				        <table id="lgPrelTable" runat="server" width="60%">	
					        <tr>	
						        <td>	
							        <dx:ASPxLabel  ID="lblDataPrel" runat="server"  Text="Data preluare" Width="120"></dx:ASPxLabel >	
						        </td>
						        <td>			
							        <dx:ASPxDateEdit  ID="deDataPrel" Width="130" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001080") %>'  AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
							        </dx:ASPxDateEdit>					
						        </td>
                                 <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>
							        <dx:ASPxLabel  ID="lblNumeAngPrel" runat="server"  Text="Nume angajator" Width="100" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtNumeAngPrel" runat="server" Text='<%# Bind("F1001079") %>'  Width="120"  AutoPostBack="false" >
							        </dx:ASPxTextBox >
						        </td>      
					        </tr>
                            <tr>						          
						        <td>
							        <dx:ASPxLabel  ID="lblMutarePrel" Width="100" runat="server"  Text="Tip mutare" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTipMutare" Width="130"  Value='<%#Eval("F1001081") %>' ID="cmbMutarePrel"   runat="server" DropDownStyle="DropDown"  TextField="F73703" ValueField="F73702" AutoPostBack="false"  ValueType="System.Int32" >
							        </dx:ASPxComboBox>
						        </td>
                                <td><label style="display:inline-block;">&nbsp; </label></td>
						        <td>
							        <dx:ASPxLabel  ID="lblCUIPrel" runat="server"  Text="CUI" Width="100" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxTextBox  ID="txtCUIPrel" runat="server" Text='<%# Bind("F1001078") %>'  AutoPostBack="false" Width="120" >
							        </dx:ASPxTextBox >
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