<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detasari.ascx.cs" Inherits="WizOne.Personal.Detasari" %>

<script type="text/javascript">
    function chkDet_CheckedChanged(s) {  
        if (s.name == "chk2") {
            if (s.GetValue() == 1)
                chk3.SetValue(0);
        }
        if (s.name == "chk3") {
            if (s.GetValue() == 1)
                chk2.SetValue(0);
        }
	}

	function SchimbareTara(s) {
		if (s.GetValue() == 1) {
            chk2.SetEnabled(false);
            chk3.SetEnabled(false);
            chk4.SetEnabled(false);
            chk5.SetEnabled(false);
		}
		else {
            chk2.SetEnabled(true);
            chk3.SetEnabled(true);
            chk4.SetEnabled(true);
            chk5.SetEnabled(true);
        }
	}

    function OnEndCallbackGridDet(s, e) {
        pnlCtlSusp.PerformCallback("ActDet");
        pnlLoading.Hide();
    }

</script>

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
							        <dx:ASPxLabel  ID="lblNationalitate" Width="100" runat="server"  Text="Tara detasare" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsNationalitate" Width="120"  Value='<%#Eval("F100920") %>' ID="cmbNationalitate"   runat="server" DropDownStyle="DropDown"  TextField="F73304" ValueField="F73302" AutoPostBack="false"  ValueType="System.Int32" >
										<ClientSideEvents SelectedIndexChanged="function(s,e){ SchimbareTara(s); }" />
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
							        <dx:ASPxLabel  ID="lblDataSfarsitDet" runat="server"  Text="Data sfarsit estimata" Width="80"></dx:ASPxLabel >	
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
                            <tr>
                                <td colspan="2">
                                    <dx:ASPxCheckBox ID="chk1"  runat="server" Width="200" Text="Platit de angajator actual" TextAlign="Left" Checked='<%#  Eval("F1001125") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001125"))%>'   ClientInstanceName="chk1" >                                     
                                    </dx:ASPxCheckBox>
                                </td>
                                <td colspan="3">
                                    <dx:ASPxCheckBox ID="chk2"  runat="server" Width="200" Text="Detasat in Romania din state UE/NON UE" TextAlign="Left" Checked='<%#  Eval("F1001126") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001126"))%>'  ClientInstanceName="chk2" >
                                        <ClientSideEvents CheckedChanged="function(s,e){ chkDet_CheckedChanged(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <dx:ASPxCheckBox ID="chk4"  runat="server" Width="200" Text="Formular A1" TextAlign="Left" Checked='<%#  Eval("F1001128") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001128"))%>'  ClientInstanceName="chk4" >                         
                                    </dx:ASPxCheckBox>
                                </td>
                                <td colspan="3">
                                    <dx:ASPxCheckBox ID="chk3"  runat="server" Width="200" Text="Detasat din Romania in state UE/NON UE" TextAlign="Left"  Checked='<%#  Eval("F1001127") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001127"))%>'  ClientInstanceName="chk3" >
                                        <ClientSideEvents CheckedChanged="function(s,e){ chkDet_CheckedChanged(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <dx:ASPxCheckBox ID="chk5"  runat="server" Width="200" Text="Acord de securitate sociala" TextAlign="Left"  Checked='<%#  Eval("F1001129") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001129"))%>' ClientInstanceName="chk5" >                                       
                                    </dx:ASPxCheckBox>
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

    <dx:ASPxGridView ID="grDateDetasari" runat="server" ClientInstanceName="grDateDetasari" ClientIDMode="Static" Width="50%" AutoGenerateColumns="false"  OnDataBinding="grDateDetasari_DataBinding" 
		 OnRowInserting="grDateDetasari_RowInserting" OnRowUpdating="grDateDetasari_RowUpdating" >
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
		<ClientSideEvents EndCallback="function (s,e) { OnEndCallbackGridDet(s,e); }" />
        <Columns>
			<dx:GridViewCommandColumn Width="150px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F11204" Name="F11204" Caption="Nume angajator"  Width="150px"/>
            <dx:GridViewDataTextColumn FieldName="F11205" Name="F11205" Caption="CUI"  Width="100px"/>
            <dx:GridViewDataComboBoxColumn FieldName="F11206" Name="F11206" Caption="Tara detasare" ReadOnly="true" Width="250px" >
				<Settings SortMode="DisplayText" />
                <PropertiesComboBox TextField="F73304" ValueField="F73302" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
                                                                                        
            <dx:GridViewDataDateColumn FieldName="F11207" Name="F11207" Caption="Data inceput"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="F11208" Name="F11208" Caption="Data sfarsit estimata"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="F11209" Name="F11209" Caption="Data incetare"  Width="100px" >                      
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
              
        </Columns>
        <SettingsCommandButton>
            <UpdateButton ButtonType="Link" Text="Actualizeaza">
                <Styles>
                    <Style Paddings-PaddingRight="10" Paddings-PaddingTop="20">
                    </Style>
                </Styles>
            </UpdateButton>
            <CancelButton ButtonType="Link" Text="Renunta">
            </CancelButton>

            <EditButton Image-ToolTip="Edit">
                <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                <Styles>
                    <Style Paddings-PaddingRight="5px" />
                </Styles>
            </EditButton> 
            <NewButton Image-ToolTip="Rand nou">
                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                <Styles>
                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                </Styles>
            </NewButton>
        </SettingsCommandButton>
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