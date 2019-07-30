<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Suspendari.ascx.cs" Inherits="WizOne.Personal.Suspendari" %>



<script type="text/javascript">

    function OnEndCallbackSusp(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }

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
						    <dx:ASPxComboBox ID="cmbMotivSuspendare" runat="server" DropDownStyle="DropDown" TabIndex="1" TextField="F09003" ValueField="F09002" AutoPostBack="false" ValueType="System.Int32" Width="250" />
					    </td>
                        <td>
					        <dx:ASPxButton ID="btnSalveazaSusp" runat="server" AutoPostBack="false"  RenderMode="Link">  
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
						    <dx:ASPxDateEdit  ID="deDataInceputSusp" Width="120" runat="server" TabIndex="2" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
				    <tr>
					    <td>		
						    <dx:ASPxLabel ID="lblDataSfarsitSusp" runat="server" Text="Data sfarsit suspendare"/>	
					    </td>
					    <td>			
						    <dx:ASPxDateEdit  ID="deDataSfarsitSusp" Width="120" runat="server" TabIndex="3" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
				    <tr>
					    <td>		
						    <dx:ASPxLabel ID="lblDataIncetareSusp" runat="server" Text="Data incetare suspendare"/>	
					    </td>
					    <td>			
						    <dx:ASPxDateEdit ID="deDataIncetareSusp" Width="120" runat="server" TabIndex="4" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                                <CalendarProperties FirstDayOfWeek="Monday" />
						    </dx:ASPxDateEdit>					
					    </td>
				    </tr>
			    </table>

                <dx:ASPxGridView ID="grDateSuspendari" runat="server" ClientInstanceName="grDateSuspendari" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  /> 
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewDataComboBoxColumn FieldName="F11104" Name="F11104" Caption="Motiv" ReadOnly="true" Width="250px" >
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
                </dx:ASPxGridView>


            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>





