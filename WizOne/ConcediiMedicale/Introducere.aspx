<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Introducere.aspx.cs" Inherits="WizOne.ConcediiMedicale.Introducere" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

    function OnTextChangedHandler(s) {
        pnlCtl.PerformCallback(s.name + ";" +s.GetText());
    }
    function OnValueChangedHandler(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
	}

    function OnClick(s) {       
        pnlCtl.PerformCallback(s.name);
    }

	function OnEndCallback(s, e) {
        pnlLoading.Hide();
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

    function GoToViewHistory(s) {
        strUrl = getAbsoluteUrl + "ConcediiMedicale/Istoric.aspx";
        popGenIst.SetHeaderText("Vizualizare CM luna anterioara");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    window.PreluareCM = function () {
        pnlCtl.PerformCallback("PreluareCM");
	}

    function StartUpload() {
        pnlLoading.Show();
    }

    function EndUpload(s) {
        pnlLoading.Hide();
        pnlCtl.PerformCallback("doc");
    }
    
</script>

<body>

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">

                <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="false" HorizontalAlignment="Center"
                BrowseButton-Text="Incarca document"  FileUploadMode="OnPageLoad" UploadMode="Advanced"  AutoStartUpload="true"  ToolTip="Incarca document" ShowTextBox="false"
                ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                <BrowseButton>
                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png" Width="16px" Height="16px"></Image>                                    
                </BrowseButton>
                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
			 </dx:ASPxUploadControl> 
				</td>
			 <td align="right" width="180">
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" meta:resourcekey="btnBackResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
	    <br /> 


   <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>

			<div>
            <tr>
             <td width="500">

	<dx:ASPxRoundPanel ID="pnlDateGen" ClientInstanceName="pnlDateGen" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Date generale concediu medical" Width="100%">
		<HeaderStyle Font-Bold="true" />
   
		<PanelCollection>
			<dx:PanelContent>
				 <table width="30%">	
					<tr>
						<td>
							<div style="float:left; padding-bottom:15px;">
								<label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
							</div>
						</td>
						<td colspan="2">							
							<dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
									CallbackPageSize="15"  OnSelectedIndexChanged="cmbAng_SelectedIndexChanged" TextFormatString="{0} {1}" ItemStyle-Wrap="True">
								<Columns>
									<dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
									<dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
									<dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
									<dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
									<dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
									<dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
								</Columns> 
								<ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							</dx:ASPxComboBox>							
						</td>	
                        <td>
                            <dx:ASPxRadioButton ID="rbProgrNorm" Width="160" runat="server" Text="CM program normal"   ClientInstanceName="rbProgrNorm"
                                 GroupName="Program">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>
                        <td colspan="2">
                            <dx:ASPxRadioButton ID="rbProgrTure" Width="150" runat="server" Text="CM program in ture"   ClientInstanceName="rbProgrTure"
                                 GroupName="Program">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>
					</tr>
			

						<tr>
						<td>	
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxLabel  ID="lblTipConcediu" runat="server" Width="100"  Text="Tip concediu"></dx:ASPxLabel >	
							</div>
						</td>
						<td>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxComboBox   ID="cmbTipConcediu" Width="140"  runat="server" DropDownStyle="DropDown"  TextField="NAME" ValueField="NO" ValueType="System.Int32" >
									<ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
								</dx:ASPxComboBox>
							</div>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblCodIndemn" Width="100" runat="server"  Text="Cod indemnizatie" ></dx:ASPxLabel >	
						</td>
                        <td>						 
							<dx:ASPxTextBox  ID="txtCodIndemn" Width="100"  MaxLength="10" runat="server" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblLocPresc" Width="100" runat="server"  Text="Loc prescriere" ></dx:ASPxLabel >	
						</td>
                        <td>				
							<dx:ASPxComboBox  ID="cmbLocPresc" Width="100"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id"  ValueType="System.Int32">
                                <ClientSideEvents  />
							</dx:ASPxComboBox >
						</td>

					</tr>
					<tr>
						<td>		
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxLabel  ID="lblDeLaData" runat="server"   Width="100" Text="Data inceput"></dx:ASPxLabel >	
							</div>
						</td>
                        <td>								
							<dx:ASPxDateEdit  ID="deDeLaData" Width="140" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"   AutoPostBack="false"  >
								<CalendarProperties FirstDayOfWeek="Monday" />
								<ClientSideEvents  ValueChanged ="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>					
						</td>
						<td>					
							<dx:ASPxLabel  ID="lblLaData" runat="server"  Width="100" Text="Data sfarsit"></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxDateEdit  ID="deLaData" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
								<CalendarProperties FirstDayOfWeek="Monday" />
								<ClientSideEvents ValueChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>										
						</td>
						<td>									
							<dx:ASPxLabel  ID="lblNrZile" Width="100" runat="server"  Text="Nr. zile" ></dx:ASPxLabel >	
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtNrZile"  Width="100" runat="server" ReadOnly="true"  AutoPostBack="false" ></dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxLabel  ID="lblSerie" runat="server" Width="100"  Text="Serie CM" ></dx:ASPxLabel >	
							</div>
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtSerie" Width="140"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
                        <td>				
							<dx:ASPxLabel  ID="lblNr" runat="server" Width="100"  Text="Numar CM" ></dx:ASPxLabel >	
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtNr" Width="100"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
                        <td>						
							<dx:ASPxLabel  ID="lblData" runat="server" Width="100" Text="Data CM"></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxDateEdit  ID="deData" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents  />
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>
						<td >
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxLabel  ID="lblCodDiag" Width="100" runat="server"  Text="Cod diagnostic" ></dx:ASPxLabel >	
							</div>
						</td>
                        <td>						  
							<dx:ASPxTextBox  ID="txtCodDiag" Width="140"  MaxLength="3"  runat="server" AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>				
						<td >
							<dx:ASPxLabel  ID="lblCodUrgenta" Width="100" runat="server"  Text="Cod urgenta" ></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxTextBox  ID="txtCodUrgenta" Width="100"  MaxLength="3" runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>				
						<td >
							<dx:ASPxLabel  ID="lblCodInfCont" Width="100" runat="server"  Text="Cod infecto-contag." ></dx:ASPxLabel >	
						</td>
                        <td>						  
							<dx:ASPxTextBox  ID="txtCodInfCont" Width="100"  MaxLength="3" runat="server" AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
					</tr>
					<tr>
                        <td colspan="2">
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxCheckBox ID="chkUrgenta" runat="server" Width="140" Text="Urgenta - procent 100%" TextAlign="Left" ClientInstanceName="chkUrgenta">
									<ClientSideEvents  />
								</dx:ASPxCheckBox>
							</div>
                        </td>
					</tr>  

					        <tr>						
						        <td >
									<div style="float:left; padding-bottom:15px;" id="divCT1" runat="server">
										<dx:ASPxLabel  ID="lblCT1" Width="100" runat="server"  Text="Cod transfer 1" ></dx:ASPxLabel >	
									</div>
								</td>
								<td colspan="2">
							        <dx:ASPxComboBox    ID="cmbCT1"  Width="220"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						        </td>
                                <td>						  
							        <dx:ASPxTextBox  ID="txtCT1" Width="50"  runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>	
						        <td >
									<div style="float:left; padding-bottom:15px;" id="divCT2" runat="server">
										<dx:ASPxLabel  ID="lblCT2" Width="100" runat="server"  Text="Cod transfer 2" ></dx:ASPxLabel >	
									</div>
								</td>
								<td colspan="2">						    
							        <dx:ASPxComboBox  ID="cmbCT2"  Width="220"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						
							        <dx:ASPxTextBox  ID="txtCT2" Width="50"  runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>	
						        <td >
									<div style="float:left; padding-bottom:15px;" id="divCT3" runat="server">
										<dx:ASPxLabel  ID="lblCT3" Width="100" runat="server"  Text="Cod transfer 3" ></dx:ASPxLabel >	
									</div>
								</td>
								<td colspan="2">						
							        <dx:ASPxComboBox   ID="cmbCT3"  Width="220"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						  
							        <dx:ASPxTextBox  ID="txtCT3" Width="50"   runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>	
						        <td >
									<div style="float:left; padding-bottom:15px;" id="divCT4" runat="server">
										<dx:ASPxLabel  ID="lblCT4" Width="100" runat="server"  Text="Cod transfer 4" ></dx:ASPxLabel >	
									</div>
								</td>
								<td colspan="2">					  
							        <dx:ASPxComboBox   ID="cmbCT4"  Width="220"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						  
							        <dx:ASPxTextBox  ID="txtCT4" Width="50"  runat="server" AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>		
						        <td >
									<div style="float:left; padding-bottom:15px;" id="divCT5" runat="server">
										<dx:ASPxLabel  ID="lblCT5" Width="100" runat="server"  Text="Cod transfer 5" ></dx:ASPxLabel >	
									</div>
								</td>
								<td colspan="2">						  
							        <dx:ASPxComboBox   ID="cmbCT5"  Width="220"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						    
							        <dx:ASPxTextBox  ID="txtCT5" Width="50" runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>

                    <tr>
                        <td>
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxRadioButton ID="rbConcInit" Width="125" runat="server" Text="Initial"   ClientInstanceName="rbConcInit"
									 GroupName="Concedii">
									<ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
								</dx:ASPxRadioButton>
							</div>
						</td>
                        <td>
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxRadioButton ID="rbConcCont"  Width="125" runat="server" Text="Continuare"  ClientInstanceName="rbConcCont" 
									 GroupName="Concedii">
									<ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
								</dx:ASPxRadioButton>
							</div>
						</td>
                       <td>                    
                            <dx:ASPxButton ID="btnCMAnt" ClientInstanceName="btnCMAnt"   Width="75" ClientIDMode="Static"  runat="server"   Text="CM luna anterioara" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ window.open('Istoric.aspx','','height=500,width=1000,left='+(window.outerWidth / 2 + window.screenX - 300)+', top=' + (window.outerHeight / 2 + window.screenY - 200)); }" />                                
                            </dx:ASPxButton>
                        </td>

					</tr>
					<tr>
                        <td>	
							<div style="float:left; padding-bottom:15px;">
								<dx:ASPxLabel  ID="lblSCMInit" runat="server" Width="100" Text="Serie CM initial" ></dx:ASPxLabel >	
							</div>
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtSCMInit" Width="140"  MaxLength="10" runat="server"   AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblNrCMInit" runat="server" Width="100" Text="Numar CM initial"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtNrCMInit" Width="100"  MaxLength="15" runat="server"   AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>										
						</td>
                        <td>						
							<dx:ASPxLabel  ID="lblDataCMInit" runat="server" Width="100" Text="Data CM initial"></dx:ASPxLabel >	
						</td>
						<td>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxDateEdit  ID="deDataCMInit" Width="100"  runat="server"  DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
									<CalendarProperties FirstDayOfWeek="Monday" />
									<ClientSideEvents  />
								</dx:ASPxDateEdit>										
							</div>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblZCMAnt" runat="server" Width="100" Text="Zile CM initial" ></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtZCMAnt" Width="75"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
					</tr>

					<tr>
                        <td>   
							<div style="float:left; padding-bottom:15px;" id="divbtnMZ" runat="server">
								<dx:ASPxButton ID="btnMZ" ClientInstanceName="btnMZ" Width="75"  ClientIDMode="Static"  runat="server"   Text="Media zilnica" oncontextMenu="ctx(this,event)" AutoPostBack="false">
									<ClientSideEvents Click="function(s,e){pnlLoading.Show(); OnClick(s); }" />                                
								</dx:ASPxButton>
							</div>
                        </td>
					</tr>
					<tr>				
						<td>
							<div style="float:left; padding-bottom:15px;" id="divlblBCCM" runat="server">
								<dx:ASPxLabel  ID="lblBCCM" runat="server"  Width="100" Text="Baza calcul CM" ></dx:ASPxLabel >	
							</div>
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtBCCM" Width="140"  runat="server" AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
                        <td>					
							<dx:ASPxLabel  ID="lblZBC" runat="server" Width="105"  Text="Zile baza calcul CM" ></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtZBC" Width="100"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblMZBC" runat="server" Width="100"  Text="Medie zile baza calcul"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtMZBC" Width="100"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>										
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblMZ" runat="server" Width="100"  Text="Medie zilnica CM"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxTextBox  ID="txtMZ" Width="75"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents  />
							</dx:ASPxTextBox>										
						</td>
					</tr> 

                    <tr>
                        <td>
                            <dx:ASPxCheckBox ID="chkStagiu" runat="server" Width="175" Text="Nu are stagiu de cotizare" Visible="false" TextAlign="Right" ClientInstanceName="chkStagiu">
                                <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxCheckBox>
                        </td>
                    </tr>
                    <tr>
                       <td>
                            <dx:ASPxRadioButton ID="rbOptiune1" Width="125" runat="server" Text="Media zilnica pt. CM cf. O 158/2005"  Visible="false"  ClientInstanceName="rbOptiune1"
                                 GroupName="Avans">
                            </dx:ASPxRadioButton>
						</td>            
                       <td colspan="2">
                            <dx:ASPxRadioButton ID="rbOptiune2" Width="140" runat="server" Text="Media zilnica pt. AMBP cf. L 346/2002"   Visible="false" ClientInstanceName="rbOptiune2"
                                 GroupName="Avans">
                            </dx:ASPxRadioButton>
						</td>
                     </tr>
        
				</table>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>

	<dx:ASPxRoundPanel ID="pnlDateAd" ClientInstanceName="pnlDateAd" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Date aditionale concediu medical" Width="100%">
		<HeaderStyle Font-Bold="true" />
   
		<PanelCollection>
			<dx:PanelContent>
			 
				    <table width="30%">	
 						<tr>				
							<td >
								<div style="float:left; padding-bottom:15px;">
									<dx:ASPxLabel  ID="lblNrAviz" runat="server"  Width="100" Text="Nr. aviz medic expert" ></dx:ASPxLabel >	
								</div>
							</td>
							<td>			
									<dx:ASPxTextBox  ID="txtNrAviz" Width="140"   MaxLength="10" runat="server"  AutoPostBack="false" >
										<ClientSideEvents  />
									</dx:ASPxTextBox>
							
							</td>									
							<td >
								<dx:ASPxLabel  ID="lblDataAviz" runat="server" Width="100" Text="Data aviz Dir. Sanatate Publica" ></dx:ASPxLabel >	
							</td>
							<td>					
								<dx:ASPxDateEdit  ID="deDataAviz" Width="125"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
									<CalendarProperties FirstDayOfWeek="Monday" />
									<ClientSideEvents  />
								</dx:ASPxDateEdit>	
							</td>
						</tr>
 						<tr>				
							<td>		
								<dx:ASPxLabel  ID="lblMedic" runat="server" Width="100" Text="Medic curant"></dx:ASPxLabel >	
							</td>
							<td>					
								<dx:ASPxTextBox  ID="txtMedic" Width="140"  MaxLength="40"  runat="server"  AutoPostBack="false" >
									<ClientSideEvents  />
								</dx:ASPxTextBox>										
							</td>										
							<td>		
								<dx:ASPxLabel  ID="lblCNP" runat="server" Width="100"  Text="CNP/CIS copil"></dx:ASPxLabel >	
							</td>
							<td>						
								<dx:ASPxComboBox  ID="cmbCNPCopil" Width="125"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id"  ValueType="System.String">
									<ClientSideEvents  />
								</dx:ASPxComboBox >										
							</td>
						</tr>  




					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblCC" Width="75" runat="server"  Text="Centru cost"  Visible="false"></dx:ASPxLabel >	

							    <dx:ASPxTextBox  ID="txtCC" Width="50" MaxLength="10"  runat="server" Text="9999" AutoPostBack="false" Visible="false">
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox>
						    </td>
                            <td>		
							    <dx:ASPxComboBox   ID="cmbCC"  Width="200"  runat="server" DropDownStyle="DropDown" Visible="false"  TextField="F06205" ValueField="F06204" AutoPostBack="false"  ValueType="System.Int32" >
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							    </dx:ASPxComboBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblDetalii" Width="75" runat="server"  Text="Detalii" Visible="false"></dx:ASPxLabel >	
						    </td>
                            <td>						
							    <dx:ASPxTextBox  ID="txtDetalii" Width="200"  runat="server" Visible="false"  AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
				    </table>
                    
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>
            </td>
           <td valign = "top" width="500">      
			      <fieldset  >               
 				    <table width = "15%" >

                     <dx:ASPxPanel ID = "pnlZile" runat= "server" >
 
                         <PanelCollection >
 
                             <dx:PanelContent runat = "server" >
 
                                 <table width= "20%" >
 
                                     <tr >
 
                                          <td >
 
                                             <dx:ASPxRadioButton ID = "rbZileCal" Width= "150" runat= "server" Visible="false" Text= "x zile calendaristice" ClientInstanceName= "rbZileCal"
 
                                                  GroupName= "Zile" >
 
                                                 <ClientSideEvents CheckedChanged= "function(s,e){ OnValueChangedHandler(s); }" />
 
                                             </dx:ASPxRadioButton>
                                        </td>
					                </tr>
					                <tr>
                                        <td>
                                            <dx:ASPxRadioButton ID = "rbZileFNUASS" Width= "150" runat= "server" Visible="false" Text= "0 zile"  ClientInstanceName= "rbZileFNUASS"
 
                                                  GroupName= "Zile" >
 
                                                 <ClientSideEvents CheckedChanged= "function(s,e){ OnValueChangedHandler(s); }" />
 
                                             </dx:ASPxRadioButton>
                                        </td> 
					                </tr>
					                <tr>
                                        <td>
                                            <dx:ASPxCheckBox ID = "chkCalcul" runat= "server" Width= "150" Visible="false" Text= "Calcul zile manual" TextAlign= "Right" ClientInstanceName= "chkCalcul" >
 
                                                 <ClientSideEvents ValueChanged= "function(s,e){ OnValueChangedHandler(s); }" />
 
                                             </dx:ASPxCheckBox>
                                        </td>                                                               
                                    </tr>     
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
				    </table>
			        </fieldset>
			        <fieldset  >
 
                    
  				        <table width = "20%" >
  

                              <tr >
  
                                  <td >
  
                                      <dx:ASPxLabel ID = "lblCT6" Width= "100" runat= "server" Visible="false"  Text= "Cod transfer 6" ></dx:ASPxLabel >	
						   
							        <dx:ASPxComboBox ID = "cmbCT6"  Width= "150"  runat= "server" DropDownStyle= "DropDown" Visible="false"   TextField= "F02105" ValueField= "F02104" AutoPostBack= "false"  ValueType= "System.Int32" >
  
                                          <ClientSideEvents SelectedIndexChanged= "function(s,e){ OnValueChangedHandler(s); }" />
  
                                      </dx:ASPxComboBox>
						    </td>
                            <td>						    
							        <dx:ASPxTextBox ID = "txtCT6" Width= "50" Enabled= "false" Visible="false"  runat= "server"  AutoPostBack= "false" >
  
                                          <ClientSideEvents TextChanged= "function(s,e){ OnTextChangedHandler(s); }" />
  
                                      </dx:ASPxTextBox>
						        </td> 
					        </tr>                                                                                                                                            	
				        </table>
                        
			        </fieldset>           
             </td>
              <td valign="top">   

                </td>
             </tr>      
	</div>

            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>
</body>

</asp:Content>