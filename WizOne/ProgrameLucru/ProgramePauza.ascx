<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgramePauza.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgramePauza" %>


<script type="text/javascript">

    function OnTextChangedHandlerPtjPauza(s) {
        pnlCtlPtjPauza.PerformCallback(s.name + ";" + s.GetText());
    }

    function OnValueChangedHandlerPtjPauza(s) {
        pnlCtlPtjPauza.PerformCallback(s.name + ";" + s.GetValue());
    }

</script>
<body>

   <dx:ASPxCallbackPanel ID = "pnlCtlPtjPauza" ClientIDMode="Static" ClientInstanceName="pnlCtlPtjPauza" runat="server" OnCallback="pnlCtlPtjPauza_Callback" SettingsLoadingPanel-Enabled="false">
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList ID="DataList1" runat="server">
        <ItemTemplate>
			<div>
            <tr>
            <td  valign="top">
			      <fieldset >
				        <legend class="legend-font-size">Pauza</legend>
				        <table width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblTimpPauza"  Width="70"  runat="server"  Text="Timp pauza: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deTimpPauza" Width="70"  runat="server" Value='<%# Eval("PauzaTimp") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerPtjPauza(s); }" />
							        </dx:ASPxDateEdit>
						        </td>
                                <td>
                                    <dx:ASPxCheckBox ID="chkPauzaDedusa"  runat="server" Width="100" Text="Pauza dedusa" TextAlign="Left" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"PauzaDedusa").ToString()=="1"%>' ClientInstanceName="chkbx1" >                                     
                                        <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandlerPtjPauza(s); }" />
                                    </dx:ASPxCheckBox>
                                </td>
						        <td>		
							        <dx:ASPxLabel  ID="lblOreMinLucr"  Width="120"  runat="server"  Text="Ore minim lucrate: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deOreMinLucr" Width="70"  runat="server" Value='<%# Eval("OreLucrateMin") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerPtjPauza(s); }" />
							        </dx:ASPxDateEdit>
						        </td>

						        <td >
							        <dx:ASPxLabel  ID="lblPauzaScutita" runat="server"  Width="80" Text="Pauza scutita: " ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxDateEdit  ID="dePauzaScutita" Width="70"  runat="server" Value='<%# Eval("PauzaScutita") %>' AutoPostBack="false"  DisplayFormatString="HH:mm" EditFormatString="HH:mm">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerPtjPauza(s); }" />
							        </dx:ASPxDateEdit>
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

    <table width="80%">
     <tr>
        <th class="legend-font-size">Marje</th>
        <dx:ASPxGridView ID="grDatePauza" runat="server" ClientInstanceName="grDatePauza" ClientIDMode="Static" Width="20%" AutoGenerateColumns="false"  OnDataBinding="grDatePauza_DataBinding" OnInitNewRow="grDatePauza_InitNewRow"
                OnRowInserting="grDatePauza_RowInserting" OnRowUpdating="grDatePauza_RowUpdating" OnRowDeleting="grDatePauza_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDatePauza_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="Program"  Width="75px" Visible="false"/>                                    
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora Inceput de la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora Inceput la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Sfarsit de la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Sfarsit la" Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataCheckColumn FieldName="TimpDedus" Name="TimpDedus" Caption="Timp dedus"  Width="70px"  />
                <dx:GridViewDataDateColumn FieldName="TimpMin" Name="TimpMin" Caption="Timp min." Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataDateColumn FieldName="TimpMax" Name="TimpMax" Caption="Timp max." Width="60px" PropertiesDateEdit-DisplayFormatString="HH:mm"  PropertiesDateEdit-EditFormatString="HH:mm" />  
                <dx:GridViewDataCheckColumn FieldName="FaraMarja" Name="FaraMarja" Caption="Fara Marja"  Width="70px"  />
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