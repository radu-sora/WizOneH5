<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameDateGenerale.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameDateGenerale" %>


<script type="text/javascript">

    function OnTextChangedHandlerDateGen(s) {
        pnlCtlDateGen.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerDateGen(s) {
        pnlCtlDateGen.PerformCallback(s.name + ";" + s.GetValue());
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
				

   <dx:ASPxCallbackPanel ID = "pnlCtlDateGen" ClientIDMode="Static" ClientInstanceName="pnlCtlDateGen" runat="server" OnCallback="pnlCtlDateGen_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Validitate</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataInc"  Width="100"  runat="server"  Text="Data Inceput: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDataInc" Width="100"  runat="server" Value='<%# Eval("DataInceput") %>' AutoPostBack="false" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" >                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataSf"  Width="100"  runat="server"  Text="Data Sfarsit: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDataSf" Width="100"  runat="server" Value='<%# Eval("DataSfarsit") %>' AutoPostBack="false" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" >                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
				        </table>
			        </fieldset>
			      <fieldset>
				        <legend class="legend-font-size">Normare</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblNorma"  Width="100"  runat="server"  Text="Norma: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtNorma" Width="40"  runat="server" Text='<%# Eval("Norma") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxTextBox>
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTipPont" runat="server"  Width="120" Text="Tip pontare: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsTipPont"  Width="200" ID="cmbTipPont" runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerDateGen(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblPauza"  Width="100"  runat="server"  Text="Pauza min: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="dePauza" Width="60"  runat="server" Value='<%# Eval("PauzaMin") %>' AutoPostBack="false"  > 
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsTipPont" TypeName="WizOne.Module.General" SelectMethod="ListaTipPontare" />
			        </fieldset>
                 </td> 
            <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Setari</legend>
				        <table width="60%">	
                            <tr>
                                <td>
                                    <dx:ASPxCheckBox ID="chkNoapte"  runat="server" Width="100" Text="De noapte ?" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"DeNoapte").ToString()=="1"%>' ClientInstanceName="chkNoapte" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerDateGen(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
				        </table>                      
			        </fieldset>
			      <fieldset>
				        <legend class="legend-font-size">Interval orar</legend>
				        <table width="60%">	
                            <tr>
                                <td>
                                    <dx:ASPxCheckBox ID="chkFlex"  runat="server" Width="100" Text="Flexibil ?" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"Flexibil").ToString()=="1"%>' ClientInstanceName="chkFlex" >
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerDateGen(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
                            </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblOraIn"  Width="70"  runat="server"  Text="Ora Intrare: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="deOraIn" Width="60"  runat="server" Value='<%# Eval("OraIntrare") %>' AutoPostBack="false"  >     
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxTimeEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblOraOut"  Width="70"  runat="server"  Text="Ora Iesire: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTimeEdit  ID="deOraOut" Width="60"  runat="server" Value='<%# Eval("OraIesire") %>' AutoPostBack="false"  > 
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerDateGen(s); }" />
							        </dx:ASPxTimeEdit>
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