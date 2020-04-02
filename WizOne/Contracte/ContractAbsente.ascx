<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractAbsente.ascx.cs" Inherits="WizOne.Contracte.ContractAbsente" %>


<script type="text/javascript">

    function OnTextChangedHandlerCtrAbs(s) {
        pnlCtlContractAbsente.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerCtrAbs(s) {
        pnlCtlContractAbsente.PerformCallback(s.name + ";" + s.GetValue());
    }

</script>
<body>
	<style type="text/css">
        .legend-font-size
        {
            font-size:15px;
            font-weight:bold;
        }
	</style>
				

   <dx:ASPxCallbackPanel ID = "pnlCtlContractAbsente" ClientIDMode="Static" ClientInstanceName="pnlCtlContractAbsente" runat="server" OnCallback="pnlCtlContractAbsente_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Ora schimb</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblOraIn"  Width="70"  runat="server"  Text="Ora Sch. IN: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="txtOraIn" Width="70"  runat="server" Value='<%# Eval("OraInSchimbare") %>' AutoPostBack="false">     
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
						        <td>		
							        <dx:ASPxLabel  ID="lblOraOut"  Width="90"  runat="server"  Text="Ora Sch. OUT: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="txtOraOut" Width="70"  runat="server" Value='<%# Eval("OraOutSchimbare") %>' AutoPostBack="false"  >    
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkOreSup"  runat="server" Width="80" Text="Ore sup. ?" TextAlign="Right"  Checked='<%#  Eval("OreSup") == DBNull.Value ? false : Convert.ToBoolean(Eval("OreSup"))%>' ClientInstanceName="chkbx1" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
						        <td >
							        <dx:ASPxLabel  ID="lblAfis" runat="server"  Width="80" Text="Afisare Ore: " ></dx:ASPxLabel >	
						        </td>	
						        <td colspan="2">
							        <dx:ASPxComboBox DataSourceID="dsAfis"  Value='<%#Eval("Afisare") %>' ID="cmbAfis"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>                            				        
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsAfis" TypeName="WizOne.Module.General" SelectMethod="ListaAfisare"/>
			        </fieldset>
			      <fieldset >
				        <legend class="legend-font-size">Setari</legend>
				        <table width="60%">	
					        <tr>				
                                <td >
                                    <dx:ASPxCheckBox ID="chkScadeVal0"  runat="server" Width="150" Text="Scade ore normale" TextAlign="Right"  Checked='<%#  Eval("ScadeVal0") == DBNull.Value ? false : Convert.ToBoolean(Eval("ScadeVal0"))%>' ClientInstanceName="chkbx2" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                                <td colspan="2">
                                    <dx:ASPxCheckBox ID="chkScadeFara"  runat="server" Width="170" Text="Scade fara depasire norma" TextAlign="Right"  Checked='<%#  Eval("ScadeFaraDepasireNorma") == DBNull.Value ? false : Convert.ToBoolean(Eval("ScadeFaraDepasireNorma"))%>' ClientInstanceName="chkbx3" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />                                        
                                    </dx:ASPxCheckBox>
                                </td>
						        <td colspan="2">
							        <dx:ASPxLabel  ID="lblRap" runat="server"  Width="140" Text="Tip raportare ore noapte: " >                                       
							        </dx:ASPxLabel >	
						        </td>	
						        <td >
							        <dx:ASPxComboBox DataSourceID="dsRap"  Value='<%#Eval("TipRaportareOreNoapte") %>' ID="cmbRap"   runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
                            <tr>
                                <td >
                                    <dx:ASPxCheckBox ID="chkPontareAuto"  runat="server" Width="150" Text="Initializare automata:" TextAlign="Right"  Checked='<%#  Eval("PontareAutomata") == DBNull.Value ? false : Convert.ToBoolean(Eval("PontareAutomata"))%>' ClientInstanceName="chkbx4" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerCtrAbs(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>                             
						        <td >
							        <dx:ASPxLabel  ID="lblOraInInit" runat="server"  Width="70" Text="Ora intrare: " >                                       
							        </dx:ASPxLabel >	
						        </td>
                                <td>
    							    <dx:ASPxTimeEdit  ID="txtInInit" Width="70"  runat="server" Value='<%# Eval("OraInInitializare") %>' AutoPostBack="false"  >
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxTimeEdit>
                                </td>                                
						        <td >
							        <dx:ASPxLabel  ID="lblOraOutInit" runat="server"  Width="70" Text="Ora iesire: " >                                        
							        </dx:ASPxLabel >	
						        </td>
                                <td>
    							    <dx:ASPxTimeEdit  ID="txtOutInit" Width="70"  runat="server" Value='<%# Eval("OraOutInitializare") %>' AutoPostBack="false"  >
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtrAbs(s); }" />
							        </dx:ASPxTimeEdit>
                                </td>
                            </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsRap" TypeName="WizOne.Module.General" SelectMethod="ListaRaportare"/>
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
    <table width="40%">
     <tr>
        <th class="legend-font-size">Drepturi absente pe contract</th>
        <dx:ASPxGridView ID="grDateCtrAbs" runat="server" ClientInstanceName="grDateCtrAbs" ClientIDMode="Static" Width="50%" AutoGenerateColumns="false"  OnDataBinding="grDateCtrAbs_DataBinding" OnInitNewRow="grDateCtrAbs_InitNewRow"
                OnRowInserting="grDateCtrAbs_RowInserting" OnRowUpdating="grDateCtrAbs_RowUpdating" OnRowDeleting="grDateCtrAbs_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDateCtrAbs_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="75px" Visible="false"/>                                    
                <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" Width="250px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>   
                <dx:GridViewDataCheckColumn FieldName="ZL" Name="ZL" Caption="Zile Lucr."  Width="60px"  />
                <dx:GridViewDataCheckColumn FieldName="S" Name="S" Caption="Sambata"  Width="60px"  />
                <dx:GridViewDataCheckColumn FieldName="D" Name="D" Caption="Duminica"  Width="60px"  />
                <dx:GridViewDataCheckColumn FieldName="SL" Name="SL" Caption="Sarb. Legale"  Width="60px"  />
                <dx:GridViewDataCheckColumn FieldName="InPontajAnual" Name="InPontajAnual" Caption="Istoric extins"  Width="60px"  />
                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />              
            </Columns>
            <SettingsCommandButton>
                <UpdateButton ButtonType="Link" Text="Actualizeaza">
                    <Styles>
                        <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
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
                <DeleteButton Image-ToolTip="Sterge">
                    <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                </DeleteButton>
                <NewButton Image-ToolTip="Rand nou">
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                    <Styles>
                        <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                    </Styles>
                </NewButton>
            </SettingsCommandButton>
        </dx:ASPxGridView>
    </tr>
   </table>
</body>