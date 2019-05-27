﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Test1.ascx.cs" Inherits="WizOne.Personal.Test1" %>



<script type="text/javascript">

    function StartUpload() {
        pnlLoading.Show();
    }

    function EndUploadDI(s) {
        pnlLoading.Hide();
        pnlCtlDateIdent.PerformCallback("img");
    }




    //function OnTextChangedHandlerDI(s) {
    //    debugger
    //    pnlCtlDateIdent.PerformCallback(s.name + ";" + s.GetText());
    //}
    //function OnValueChangedHandlerDI(s) {
    //    pnlCtlDateIdent.PerformCallback(s.name + ";" + s.GetValue());
    //}
    //function OnClickDI(s) {
    //    pnlLoading.Show();
    //    pnlCtlDateIdent.PerformCallback(s.name);
    //}

    //function OnConfirm()
    //{
    //    pnlCtlDateIdent.PerformCallback("PreluareDate");
    //}



    function GoToIstoricDI(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnEndCallbackDI(s, e) {

        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }

        if (s.cp_InfoMessage != null) {
            swal({
                title: "Informare", text: s.cp_InfoMessage,
                type: "info", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Nu", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    pnlCtlDateIdent.PerformCallback("PreluareDate");
                }
            });
            s.cp_InfoMessage = null;
        }
    }

</script>

<body>

	<style type="text/css">
		.fieldset-auto-width {
				display: inline-block;                         
		}
        .legend-font-size
        {
            font-size:15px;
            font-weight:bold;
        }
	</style>

   <dx:ASPxCallbackPanel ID = "Test1_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlDateIdent" runat="server" OnCallback="pnlCtlDateIdent_Callback"  SettingsLoadingPanel-Enabled="false">
      <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackDI(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>

             <div>
              <tr align="left">
             <td  valign="bottom">
			  <fieldset class="fieldset-auto-width">
				<legend id="lgFoto" runat="server" class="legend-font-size">Fotografie</legend>
				<table width="200" height="200"  valign="bottom">
                    <tr>
                        <td align="left"  valign="bottom">
                            <img  Height="180" HorizontalAlignment="Center" ID="img" runat="server"  VerticalAlignment="Center" Width="170" />
                        </td>
                    </tr>
                    <tr style="display:inline-block;" halign="right" valign="bottom">
					    <td >
                            <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="false" HorizontalAlignment="Center"
                                BrowseButton-Text="Incarca"  FileUploadMode="OnPageLoad" UploadMode="Advanced"  AutoStartUpload="true"  ToolTip="Incarca fotografie" ShowTextBox="false"
                                ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                <BrowseButton>
                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png" Width="16px" Height="16px"></Image>                                    
                                </BrowseButton>
                                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUploadDI(s); }" />
                            </dx:ASPxUploadControl>
                        </td>   
                        <td >
                            <dx:ASPxButton ID="btnSterge" runat="server" ToolTip="Sterge fotografie" HorizontalAlignment="Center" Text="Sterge" OnClick="btnDoc_Click"  Height="28">
                                <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                                <Paddings PaddingLeft="0px" PaddingRight="0px" />
                                <ClientSideEvents Click="function(s,e) { e.processOnServer = true; pnlCtlDateIdent.PerformCallback('btnSterge'); }" />
                            </dx:ASPxButton>

					    </td>
                    </tr>	                    
				</table>
			  </fieldset>                    	

            </td>
                 
        
        <asp:DataList  ID="Test1_DataList" runat="server">           
         <ItemTemplate> 

           <td  valign="top">
          
			  <fieldset class="fieldset-auto-width">
				<legend id="lgIdent" runat="server" class="legend-font-size">Date unice de identificare</legend>
				<table width="40%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblMarca" runat="server"  Text="Marca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtMarcaDI" runat="server" Text='<%# Eval("F10003") %>'  >
							</dx:ASPxTextBox >
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCNP" runat="server"  Text="CNP/CUI" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtCNPDI" runat="server" Text='<%# Eval("F10017") %>'   >
							</dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblMarcaUnica" runat="server"  Text="Marca unica"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtMarcaUnica" runat="server" Text='<%# Eval("F1001036") %>' ReadOnly="true"   ></dx:ASPxTextBox >										
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblEID" runat="server"  Text="EID"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtEIDDI" runat="server" Text='<%# Eval("F100901") %>'   >
							</dx:ASPxTextBox >										
						</td>
					</tr>					
				</table>
			  </fieldset>

			  <fieldset class="fieldset-auto-width">
				<legend id="lgSex" runat="server" class="legend-font-size">Data nasterii si Sex</legend>
				<table width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDataNasterii" Width="100" runat="server"  Text="Data nasterii" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxDateEdit  ID="deDataNasterii"  Enabled="true" Width="100" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" runat="server" Value='<%# Eval("F10021") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
							</dx:ASPxDateEdit>
						</td>
                    </tr>
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblVarsta"  Width="100" runat="server"  Text="Varsta" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtVarsta" Width="75" Enabled="false" runat="server"    ></dx:ASPxTextBox >
						</td>
                    </tr>
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblSex" runat="server" Text="Sex" >
                                
                            </dx:ASPxLabel>
                        </td>


                        <td>
                            <dx:ASPxRadioButton ID="chkM" Width="75" runat="server" Text="Masculin" Enabled="true"  ClientInstanceName="chkbx1"
                                 GroupName="Sex">
                                
                            </dx:ASPxRadioButton>
                        </td>
                        <td>
                            <dx:ASPxRadioButton ID="chkF"  Width="75" runat="server" Text="Feminin" Enabled="true" ClientInstanceName="chkbx2" 
                                 GroupName="Sex">
                                
                            </dx:ASPxRadioButton>
                        </td>

					</tr>						
				</table>
			  </fieldset>
          
        
	
			  <fieldset class="fieldset-auto-width">
				<legend id="lgNume" runat="server" class="legend-font-size">Nume si prenume</legend>
				<table width="40%">	
					<tr>				
						<td>	
							<dx:ASPxLabel  ID="lblNume" runat="server" Text="Nume"></dx:ASPxLabel >	
						</td>
						<td>		
							<dx:ASPxTextBox  ID="txtNume" runat="server" Text='<%# Bind("F10008") %>'   >
                               
							</dx:ASPxTextBox >						
						</td>
                        <td>
                            <dx:ASPxButton ID="btnNume" ClientInstanceName="btnNume"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  ToolTip="Modificari contract"  RenderMode="Link" oncontextMenu="ctx(this,event)" >
                              
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnNumeIst" ClientInstanceName="btnNumeIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                               
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblPrenume" runat="server"  Text="Prenume"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxTextBox  ID="txtPrenume" runat="server" ClientInstanceName="txtPrenume"  Text='<%# Eval("F10009") %>'    >
                                
							</dx:ASPxTextBox >										
						</td>
                        <td>
                            <dx:ASPxButton ID="btnPrenume" ClientInstanceName="btnPrenume" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnPrenumeIst" ClientInstanceName="btnPrenumeIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" >
                                
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td>	
							<dx:ASPxLabel  ID="lblNumeAnt" runat="server" Text="Nume anterior"></dx:ASPxLabel >	
						</td>
						<td>		
							<dx:ASPxTextBox  ID="txtNumeAnt" runat="server" Text='<%# Eval("F100905") %>'>
                               
							</dx:ASPxTextBox >						
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifNume" runat="server"  Text="Data modificare nume"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifNume" Width="100" runat="server"  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100906") %>'   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>   
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblStareCivila" Width="100" runat="server"  Text="Stare civila" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsStareCivila"   Value='<%#Eval("F10046") %>' ID="cmbStareCivila"   runat="server" DropDownStyle="DropDown"  TextField="F71004" ValueField="F71002"   ValueType="System.Int32" >
                               
							</dx:ASPxComboBox>
						</td>
					</tr>                                     	
                    				
				</table>
                <asp:ObjectDataSource runat="server" ID="dsStareCivila" TypeName="WizOne.Module.General" SelectMethod="GetStareCivila"/>
			    </fieldset>
               
               </td> 
         </ItemTemplate>
      </asp:DataList>

             </tr>
			</div>	
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
</body>		











