<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractCiclic.ascx.cs" Inherits="WizOne.ContracteLucru.ContractCiclic" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>
<script type="text/javascript">

    function OnTextChangedHandlerCtrCiclic(s) {
        pnlCtlCtrCiclic.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerCtrCiclic(s) {
        pnlCtlCtrCiclic.PerformCallback(s.name + ";" + s.GetValue());
    }

</script>
<body>			

   <dx:ASPxCallbackPanel ID = "pnlCtlCtrCiclic" ClientIDMode="Static" ClientInstanceName="pnlCtlCtrCiclic" runat="server" OnCallback="pnlCtlCtrCiclic_Callback" SettingsLoadingPanel-Enabled="false">
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
							        <dx:ASPxLabel  ID="lblLung"  Width="120"  runat="server"  Text="Lungime ciclu (in zile): "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxTextBox  ID="txtLung" Width="50"  runat="server" Text='<%# Eval("CicluLungime") %>' AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerCtrCiclic(s); }" />
							        </dx:ASPxTextBox>
						        </td>
						        <td>		
							        <dx:ASPxLabel  ID="lblDataInc"  Width="100"  runat="server"  Text="Data inceput ciclu: "></dx:ASPxLabel >	
						        </td>
						        <td>
							        <dx:ASPxDateEdit  ID="deDataInc" Width="100"  runat="server" Value='<%# Eval("CicluDataInceput") %>' AutoPostBack="false"  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy">                                         
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtrCiclic(s); }" />
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

   <table width="60%">
     <tr>
        <th class="legend-font-size">Contracte zilnice</th>
        <dx:ASPxGridView ID="grDateCtrCiclic" runat="server" ClientInstanceName="grDateCtrCiclic" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateCtrCiclic_DataBinding" OnInitNewRow="grDateCtrCiclic_InitNewRow"
                OnRowInserting="grDateCtrCiclic_RowInserting" OnRowUpdating="grDateCtrCiclic_RowUpdating" OnRowDeleting="grDateCtrCiclic_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDateCtrCiclic_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Program"  Width="100px" Visible="false" />                                    
                <dx:GridViewDataTextColumn FieldName="ZiCiclu" Name="ZiCiclu" Caption="Zi"  Width="75px" ReadOnly="true"/>
                <dx:GridViewDataComboBoxColumn FieldName="IdContractZilnic" Name="IdContractZilnic" Caption="Contract" Width="250px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>   
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