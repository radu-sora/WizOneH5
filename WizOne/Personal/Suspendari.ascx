<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Suspendari.ascx.cs" Inherits="WizOne.Personal.Suspendari" %>



<script type="text/javascript">

    function OnEndCallbackSusp(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }

        pnlLoading.Hide();
    }

    function OnEndCallbackGridSusp(s, e) {
        pnlCtlSusp.PerformCallback("ActSusp");
        pnlLoading.Hide();
    }
</script>


    <dx:ASPxCallbackPanel ID = "pnlCtlSusp" ClientIDMode="Static" ClientInstanceName="pnlCtlSusp" runat="server" OnCallback="pnlCtlSusp_Callback" SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents BeginCallback="function (s,e) { pnlLoading.Show(); }" EndCallback="function (s,e) { OnEndCallbackSusp(s,e); }" />
        <PanelCollection>
            <dx:PanelContent>

			    <table style="padding:15px; margin:15px;">	
				    <tr>
					    <td>
						    <dx:ASPxLabel  ID="lblMtvSusp" Width="170" runat="server" Text="Motiv suspendare"/>
					    </td>	
					    <td>
						    <dx:ASPxComboBox ID="cmbMotivSuspendare" ClientInstanceName="cmbMotivSuspendare" runat="server" DropDownStyle="DropDown" TabIndex="1" TextField="F09003" ValueField="F09002" AutoPostBack="false" ValueType="System.Int32" Width="250" />
					    </td>
                        <td>
					        <dx:ASPxButton ID="btnSalveazaSusp" runat="server" AutoPostBack="false" Visible="false"  RenderMode="Link">  
                                <ClientSideEvents Click="function(s,e){ pnlCtlSusp.PerformCallback(s.name) }" />
						        <Image Url="../Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                <Paddings PaddingLeft="10px" />
					        </dx:ASPxButton>
                        </td>
				    </tr>
				    <tr>
					    <td>		
						    <dx:ASPxLabel ID="lblDataInceputSusp" runat="server" Text="Data inceput suspendare"/>	
					    </td>
					    <td>			
						    <dx:ASPxDateEdit  ID="deDataInceputSusp" Width="120" ClientInstanceName="deDataInceputSusp" runat="server" TabIndex="2" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
				    <tr>
					    <td>		
						    <dx:ASPxLabel ID="lblDataSfarsitSusp" runat="server" Text="Data sfarsit suspendare"/>	
					    </td>
					    <td>			
						    <dx:ASPxDateEdit  ID="deDataSfarsitSusp" Width="120" ClientInstanceName="deDataSfarsitSusp" runat="server" TabIndex="3" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
				    <tr>
					    <td>		
						    <dx:ASPxLabel ID="lblDataIncetareSusp" runat="server" Text="Data incetare suspendare"/>	
					    </td>
					    <td>			
						    <dx:ASPxDateEdit ID="deDataIncetareSusp" Width="120" ClientInstanceName="deDataIncetareSusp" runat="server" TabIndex="4" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
			    </table>

                <dx:ASPxGridView ID="grDateSuspendari" runat="server" ClientInstanceName="grDateSuspendari" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"
                    OnRowUpdating="grDateSuspendari_RowUpdating" OnRowInserting="grDateSuspendari_RowInserting" OnInitNewRow="grDateSuspendari_InitNewRow">
                    <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackGridSusp(s,e); }" />
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  /> 
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataComboBoxColumn FieldName="F11104" Name="F11104" Caption="Motiv"  Width="250px" >
                            <PropertiesComboBox TextField="F09003" ValueField="F09002" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                                                                                        
                        <dx:GridViewDataTextColumn FieldName="F11103" Name="F11103" Caption="Marca"  Width="75px" Visible="false"/>
                        <dx:GridViewDataDateColumn FieldName="F11105" Name="F11105" Caption="Data inceput"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="F11106" Name="F11106" Caption="Data sfarsit"  Width="100px" >                      
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="F11107" Name="F11107" Caption="Data incetare"  Width="100px" >  
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


            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>





