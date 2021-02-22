<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Introducere.aspx.cs" Inherits="WizOne.ConcediiMedicale.Introducere" %>


<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v19.1" %>
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
            <td align="right">

                <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="false" HorizontalAlignment="Center"
                BrowseButton-Text="Incarca document"  FileUploadMode="OnPageLoad" UploadMode="Advanced"  AutoStartUpload="true"  ToolTip="Incarca document" ShowTextBox="false"
                ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                <BrowseButton>
                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png" Width="16px" Height="16px"></Image>                                    
                </BrowseButton>
                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
			 </dx:ASPxUploadControl>                     
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>



   <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>

			<div>
            <tr>
             <td width="500">
			  <fieldset class="fieldset-auto-width">
				<legend class="legend-font-size">Date generale concediu</legend>
				<table width="30%">	
					<tr>
						<td>
							<label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
						</td>
						<td colspan="2">
							<dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
									CallbackPageSize="15"  OnSelectedIndexChanged="cmbAng_SelectedIndexChanged" TextFormatString="{0} {1}" >
								<Columns>
									<dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
									<dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
									<dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
									<dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
									<dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
									<dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
								</Columns>                            
							</dx:ASPxComboBox>
						</td>	
					</tr>
					<tr>
                        <td>
                            <dx:ASPxRadioButton ID="rbProgrNorm" Width="150" runat="server" Text="CM program normal" Enabled="true"  ClientInstanceName="rbProgrNorm"
                                 GroupName="Program">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>
                        <td>
                            <dx:ASPxRadioButton ID="rbProgrTure" Width="150" runat="server" Text="CM program in ture" Enabled="true"  ClientInstanceName="rbProgrTure"
                                 GroupName="Program">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>
					</tr>
					<tr>
						<td>		
							<dx:ASPxLabel  ID="lblTipConcediu" runat="server" Width="75"  Text="Tip concediu"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxComboBox   ID="cmbTipConcediu" Width="150"  runat="server" DropDownStyle="DropDown"  TextField="NAME" ValueField="NO" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							</dx:ASPxComboBox>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblCodIndemn" Width="100" runat="server"  Text="Cod indemnizatie" ></dx:ASPxLabel >	
						</td>
                        <td>						 
							<dx:ASPxTextBox  ID="txtCodIndemn" Width="50"  MaxLength="10" runat="server" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
						<td >
							<dx:ASPxLabel  ID="lblLocPresc" Width="105" runat="server"  Text="Prescris de" ></dx:ASPxLabel >	
						</td>
                        <td>				
							<dx:ASPxComboBox  ID="cmbLocPresc" Width="100"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id"  ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							</dx:ASPxComboBox >
						</td>

					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblSerie" runat="server" Width="100"  Text="Serie CM" ></dx:ASPxLabel >	
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtSerie" Width="100"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>				
							<dx:ASPxLabel  ID="lblNr" runat="server" Width="75"  Text="Numar CM" ></dx:ASPxLabel >	
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtNr" Width="80"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>						
							<dx:ASPxLabel  ID="lblData" runat="server" Width="75" Text="Data CM"></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxDateEdit  ID="deData" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>
						<td>			
							<dx:ASPxLabel  ID="lblDeLaData" runat="server"   Width="75" Text="Data inceput"></dx:ASPxLabel >	
						</td>
                        <td>								
							<dx:ASPxDateEdit  ID="deDeLaData" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"   AutoPostBack="false"  >
								<CalendarProperties FirstDayOfWeek="Monday" />
								<ClientSideEvents  ValueChanged ="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>					
						</td>
						<td>					
							<dx:ASPxLabel  ID="lblLaData" runat="server"  Width="75" Text="Data sfarsit"></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxDateEdit  ID="deLaData" Width="80"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
								<CalendarProperties FirstDayOfWeek="Monday" />
								<ClientSideEvents ValueChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>										
						</td>
						<td>									
							<dx:ASPxLabel  ID="lblNrZile" Width="75" runat="server"  Text="Nr. zile" ></dx:ASPxLabel >	
						</td>
                        <td>					
							<dx:ASPxTextBox  ID="txtNrZile"  Width="50" runat="server" ReadOnly="true"  AutoPostBack="false" ></dx:ASPxTextBox >
						</td>
					</tr>
					        <tr>	
								<td>
									<dx:ASPxLabel  ID="lblCalcul" Width="100" runat="server"  Text="Calcul zile manual" ></dx:ASPxLabel >	
								</td>
						        <td >
							        <dx:ASPxLabel  ID="lblCT1" Width="100" runat="server"  Text="Cod transfer 1" ></dx:ASPxLabel >	
								</td>
								<td>
							        <dx:ASPxComboBox    ID="cmbCT1"  Width="150"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
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
								<td></td>
						        <td >
							        <dx:ASPxLabel  ID="lblCT2" Width="100" runat="server"  Text="Cod transfer 2" ></dx:ASPxLabel >	
								</td>
								<td>						    
							        <dx:ASPxComboBox  ID="cmbCT2"  Width="150"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						
							        <dx:ASPxTextBox  ID="txtCT2" Width="50"  Enabled="false" runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>	
								<td></td>
						        <td >
							        <dx:ASPxLabel  ID="lblCT3" Width="100" runat="server"  Text="Cod transfer 3" ></dx:ASPxLabel >	
								</td>
								<td>						
							        <dx:ASPxComboBox   ID="cmbCT3"  Width="150"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						  
							        <dx:ASPxTextBox  ID="txtCT3" Width="50"  Enabled="false" runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>	
								<td></td>
						        <td >
							        <dx:ASPxLabel  ID="lblCT4" Width="100" runat="server"  Text="Cod transfer 4" ></dx:ASPxLabel >	
								</td>
								<td>					  
							        <dx:ASPxComboBox   ID="cmbCT4"  Width="150"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						  
							        <dx:ASPxTextBox  ID="txtCT4" Width="50" Enabled="false"  runat="server" AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					        <tr>		
								<td></td>
						        <td >
							        <dx:ASPxLabel  ID="lblCT5" Width="100" runat="server"  Text="Cod transfer 5" ></dx:ASPxLabel >	
								</td>
								<td>						  
							        <dx:ASPxComboBox   ID="cmbCT5"  Width="150"  runat="server" DropDownStyle="DropDown"  TextField="F02105" ValueField="F02104" AutoPostBack="false"  ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
							        </dx:ASPxComboBox>
						    </td>
                            <td>						    
							        <dx:ASPxTextBox  ID="txtCT5" Width="50" Enabled="false"  runat="server"  AutoPostBack="false" >
                                        <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							        </dx:ASPxTextBox>
						        </td> 
					        </tr>
					<tr>
						<td >
							<dx:ASPxLabel  ID="lblCodDiag" Width="100" runat="server"  Text="Cod diagnostic" ></dx:ASPxLabel >	
						</td>
                        <td>						  
							<dx:ASPxTextBox  ID="txtCodDiag" Width="50"  MaxLength="3"  runat="server" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>				
						<td >
							<dx:ASPxLabel  ID="lblCodUrgenta" Width="100" runat="server"  Text="Cod urgenta" ></dx:ASPxLabel >	
						</td>
                        <td>						
							<dx:ASPxTextBox  ID="txtCodUrgenta" Width="50"  MaxLength="3" runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>				
						<td >
							<dx:ASPxLabel  ID="lblCodInfCont" Width="105" runat="server"  Text="Cod infecto-contag." ></dx:ASPxLabel >	
						</td>
                        <td>						  
							<dx:ASPxTextBox  ID="txtCodInfCont" Width="50"  MaxLength="3" runat="server" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
					</tr>                                                          				             				
	
                    <tr>
                        <td>
                            <dx:ASPxRadioButton ID="rbConcInit" Width="125" runat="server" Text="Initial" Enabled="true"  ClientInstanceName="rbConcInit"
                                 GroupName="Concedii">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>
                        <td>
                            <dx:ASPxRadioButton ID="rbConcCont"  Width="125" runat="server" Text="Continuare" Enabled="true" ClientInstanceName="rbConcCont" 
                                 GroupName="Concedii">
                                <ClientSideEvents CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxRadioButton>
						</td>

					</tr>
					<tr>
						<td >
							<dx:ASPxLabel  ID="lblZCMAnt" runat="server" Width="125" Text="Zile CM initial" ></dx:ASPxLabel >	
					
							<dx:ASPxTextBox  ID="txtZCMAnt" Width="50"  runat="server" Enabled="false"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>					
							<dx:ASPxLabel  ID="lblSCMInit" runat="server" Width="125" Text="Serie CM initial" ></dx:ASPxLabel >	
					
							<dx:ASPxTextBox  ID="txtSCMInit" Width="100"  MaxLength="10" runat="server" Enabled="false"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblNrCMInit" runat="server" Width="125" Text="Numar CM initial"></dx:ASPxLabel >	
						
							<dx:ASPxTextBox  ID="txtNrCMInit" Width="100"  MaxLength="15" runat="server" Enabled="false"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>										
						</td>
                        <td>						
							<dx:ASPxLabel  ID="lblDataCMInit" runat="server" Width="100" Text="Data CM initial"></dx:ASPxLabel >	
						
							<dx:ASPxDateEdit  ID="deDataCMInit" Width="80"  runat="server"  Enabled="false" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>
                        <td>                    
                            <dx:ASPxButton ID="btnMZ" ClientInstanceName="btnMZ" Width="75"  ClientIDMode="Static"  runat="server"   Text="Media zilnica" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClick(s); }" />                                
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblBCCM" runat="server"  Width="125" Text="Baza calcul CM" ></dx:ASPxLabel >	
					
							<dx:ASPxTextBox  ID="txtBCCM" Width="50"  runat="server" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>					
							<dx:ASPxLabel  ID="lblZBC" runat="server" Width="125"  Text="Zile baza calcul CM" ></dx:ASPxLabel >	
						
							<dx:ASPxTextBox  ID="txtZBC" Width="50"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblMZBC" runat="server" Width="125"  Text="Medie zile baza calcul"></dx:ASPxLabel >	
						
							<dx:ASPxTextBox  ID="txtMZBC" Width="50"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>										
						</td>
                        <td>							
							<dx:ASPxLabel  ID="lblMZ" runat="server" Width="125"  Text="Medie zilnica CM"></dx:ASPxLabel >	
						
							<dx:ASPxTextBox  ID="txtMZ" Width="50"  runat="server"  AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
							</dx:ASPxTextBox>										
						</td>
					</tr> 
                    <tr>
                        <td>
                            <dx:ASPxCheckBox ID="chkStagiu" runat="server" Width="175" Text="Nu are stagiu de cotizare" TextAlign="Right" ClientInstanceName="chkStagiu">
                                <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            </dx:ASPxCheckBox>
                        </td>
                    </tr>
                    <tr>
                       <td>
                            <dx:ASPxRadioButton ID="rbOptiune1" Width="125" runat="server" Text="Media zilnica pt. CM cf. O 158/2005" Enabled="true" Visible="false"  ClientInstanceName="rbOptiune1"
                                 GroupName="Avans">
                            </dx:ASPxRadioButton>
						</td>
                     </tr>
                    <tr>
                       <td>
                            <dx:ASPxRadioButton ID="rbOptiune2" Width="125" runat="server" Text="Media zilnica pt. AMBP cf. L 346/2002" Enabled="true"  Visible="false" ClientInstanceName="rbOptiune2"
                                 GroupName="Avans">
                            </dx:ASPxRadioButton>
						</td>
                     </tr>
                    <tr>

                        <td>                    
                            <dx:ASPxButton ID="btnCMAnt" ClientInstanceName="btnCMAnt"   Width="75" ClientIDMode="Static"  runat="server"   Text="CM luna anterioara" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ window.open('Istoric.aspx','','height=500,width=1000,left='+(window.outerWidth / 2 + window.screenX - 300)+', top=' + (window.outerHeight / 2 + window.screenY - 200)); }" />                                
                            </dx:ASPxButton>
                        </td>

					</tr>

					</table>
                                                                                                                                                                                    				
				  </fieldset>
			      <fieldset class="fieldset-auto-width">
				    <legend class="legend-font-size">Date aditionale CM</legend>
				    <table width="20%">	
 						<tr>				
							<td >
								<dx:ASPxLabel  ID="lblNrAviz" runat="server"  Width="125" Text="Nr. aviz medic expert" ></dx:ASPxLabel >	
							</td>
							<td>				
								<dx:ASPxTextBox  ID="txtNrAviz" Width="100"   MaxLength="10" runat="server"  AutoPostBack="false" >
									<ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
								</dx:ASPxTextBox>
							</td>
						</tr>
 						<tr>				
							<td >
								<dx:ASPxLabel  ID="lblDataAviz" runat="server" Width="125" Text="Data aviz Dir. Sanatate Publica" ></dx:ASPxLabel >	
							</td>
							<td>					
								<dx:ASPxDateEdit  ID="deDataAviz" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
									<CalendarProperties FirstDayOfWeek="Monday" />
									<ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
								</dx:ASPxDateEdit>	
							</td>
						</tr>
 						<tr>				
							<td>		
								<dx:ASPxLabel  ID="lblMedic" runat="server" Width="125" Text="Medic curant"></dx:ASPxLabel >	
							</td>
							<td>					
								<dx:ASPxTextBox  ID="txtMedic" Width="100"  MaxLength="40"  runat="server"  AutoPostBack="false" >
									<ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
								</dx:ASPxTextBox>										
							</td>
						</tr> 
 						<tr>				
							<td>		
								<dx:ASPxLabel  ID="lblCNP" runat="server" Width="125"  Text="CNP/CIS copil"></dx:ASPxLabel >	
							</td>
							<td>						
								<dx:ASPxComboBox  ID="cmbCNPCopil" Width="100"  runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id"  ValueType="System.Int32">
									<ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
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
                    
			     </fieldset>
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
 
                                             <dx:ASPxRadioButton ID = "rbZileCal" Width= "150" runat= "server" Visible="false" Text= "x zile calendaristice" Enabled= "true"  ClientInstanceName= "rbZileCal"
 
                                                  GroupName= "Zile" >
 
                                                 <ClientSideEvents CheckedChanged= "function(s,e){ OnValueChangedHandler(s); }" />
 
                                             </dx:ASPxRadioButton>
                                        </td>
					                </tr>
					                <tr>
                                        <td>
                                            <dx:ASPxRadioButton ID = "rbZileFNUASS" Width= "150" runat= "server" Visible="false" Text= "0 zile" Enabled= "true"  ClientInstanceName= "rbZileFNUASS"
 
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