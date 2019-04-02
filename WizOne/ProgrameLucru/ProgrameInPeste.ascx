<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameInPeste.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameInPeste" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnTextChangedHandlerInPeste(s) {
        pnlCtlInPeste.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerInPeste(s) {
        pnlCtlInPeste.PerformCallback(s.name + ";" + s.GetValue());
    }
</script>
<body>

				

   <dx:ASPxCallbackPanel ID = "pnlCtlInPeste" ClientIDMode="Static" ClientInstanceName="pnlCtlInPeste" runat="server" OnCallback="pnlCtlInPeste_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Diferenta admisa</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDifRapPeste"  Width="120"  runat="server"  Text="Diferenta raportare"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDifRapPeste" Width="60"  runat="server" Value='<%# Eval("INPesteDiferentaRaportare") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerInPeste(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDifPlata"  Width="120"  runat="server"  Text="Diferenta plata"></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDifPlata" Width="60"  runat="server" Value='<%# Eval("INPesteDiferentaPlata") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerInPeste(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
					        </tr>
				        </table>
			        </fieldset>
                </td>
                <td  valign="top">
			      <fieldset>
				        <legend class="legend-font-size">Transfer</legend>
				        <table width="60%">	
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimitePlatitPeste" runat="server"  Width="150" Text="Transfer timp platit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsInPeste"  Width="170"  Value='<%#Eval("INPesteCampPlatit") %>' ID="cmbINPesteCampPlatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerInPeste(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblTrimiteNeplatitPeste" runat="server"  Width="150" Text="Transfer timp neplatit la" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsInPeste"  Width="170"  Value='<%#Eval("INPesteCampNeplatit") %>' ID="cmbINPesteCampNeplatit"   runat="server" DropDownStyle="DropDown"  TextField="Alias" ValueField="Denumire" ValueType="System.String">
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandlerInPeste(s); }" />
							        </dx:ASPxComboBox >
						        </td>
					        </tr>
				        </table>
                      <asp:ObjectDataSource runat="server" ID="dsInPeste" TypeName="WizOne.Module.General" SelectMethod="GetPtj_AliasFOrdonat" />
			        </fieldset>
                 </td> 
   
                </tr>                     
			</div>
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>


    <table width="80%">
     <tr>
        <th class="legend-font-size">Trepte</th>
        <dx:ASPxGridView ID="grDateInPeste" runat="server" ClientInstanceName="grDateInPeste" ClientIDMode="Static" Width="35%" AutoGenerateColumns="false"  OnDataBinding="grDateInPeste_DataBinding" OnInitNewRow="grDateInPeste_InitNewRow"
                OnRowInserting="grDateInPeste_RowInserting" OnRowUpdating="grDateInPeste_RowUpdating" OnRowDeleting="grDateInPeste_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDateInPeste_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="Program"  Width="75px" Visible="false"/>                                    
                <dx:GridViewDataTextColumn FieldName="TipInOut" Name="TipInOut" Caption="TipInOut"  Width="75px" Visible="false"/>
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Timp de la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Timp la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="Valoare" Name="Valoare" Caption="Valoare" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
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