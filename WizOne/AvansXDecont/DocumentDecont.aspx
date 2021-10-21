<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="DocumentDecont.aspx.cs" Inherits="WizOne.AvansXDecont.DocumentDecont" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width:100%">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDocOrig" ClientInstanceName="btnDocOrig" ClientIDMode="Static" runat="server" Text="Documente originale" OnClick="btnDocOrig_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/chooser.png"></Image>
                </dx:ASPxButton>	
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare"  AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/Salveaza.png"></Image>
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        pnlCtl.PerformCallback('btnSave');
                    }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aprobare" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>				
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge"  AutoPostBack="false" oncontextMenu="ctx(this,event)" >              
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                    <ClientSideEvents Click="function(s, e) {
                        OnMotivRespingere();
                    }" />
                </dx:ASPxButton>				
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent>
			
                <dx:ASPxRoundPanel ID="pnlDateGen" ClientInstanceName="pnlDateGen" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Date generale" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblNrOrdin" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nr. decont</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtDecontNo" ClientInstanceName="txtDecontNo" runat="server" ClientEnabled="false" Width="100px">
								</dx:ASPxTextBox>
							</div>        
							<label id="lblNume" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nume</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtNumeComplet" ClientInstanceName="txtNumeComplet" ClientEnabled="false" runat="server" Width="300px">
								</dx:ASPxTextBox>
							</div> 
						</div>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Departament</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtDepartament" ClientInstanceName="txtDepartament" ClientEnabled="false" runat="server" Width="290px">
								</dx:ASPxTextBox>
							</div>        
							<label id="lblLocMunca" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Loc munca</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtLocMunca" ClientInstanceName="txtLocMunca" ClientEnabled="false" runat="server" Width="250px">
								</dx:ASPxTextBox>
							</div> 
						</div>	

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblIBAN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">IBAN</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtContIban" ClientInstanceName="txtContIban" ClientEnabled="false" runat="server" Width="290px">
								</dx:ASPxTextBox>
							</div>   
						</div>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblAvs" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Avansuri</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtAvansFolosit" ClientInstanceName="txtAvansFolosit" ClientEnabled="false" Font-Bold="true" ForeColor="Red" runat="server" Width="400px">
								</dx:ASPxTextBox>
							</div>   
						</div>

						<div class="Absente_divOuter margin_top15">
							<label id="lblDocAvs" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Avans</label>
							<div style="float:left; padding-right:15px;">    
								<dx:ASPxComboBox ID="cmbDocAvans" ClientInstanceName="cmbDocAvans" ClientIDMode="Static" runat="server"  ValueField="DocumentId" Width="600" TextField="DocumentId" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="DocumentId" Caption="Nr. document" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                                        <dx:ListBoxColumn FieldName="DocumentDate" Caption="Data" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                                        <dx:ListBoxColumn FieldName="DocumentTypeId" Caption="DocumentTypeId" Visible="false" meta:resourcekey="ListBoxColumnResource3" />
                                        <dx:ListBoxColumn FieldName="DocumentTypeName" Caption="Tip" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                                        <dx:ListBoxColumn FieldName="PaymentAmount" Caption="Valoare" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                                        <dx:ListBoxColumn FieldName="CurrencyId" Caption="CurrencyId" Visible="false" meta:resourcekey="ListBoxColumnResource6" />
										<dx:ListBoxColumn FieldName="CurrencyCode" Caption="Valuta" Width="130px" meta:resourcekey="ListBoxColumnResource7" />
                                    </Columns> 
									<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDocAvans'); }" />
								</dx:ASPxComboBox>
							</div>
						</div>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>	

                <dx:ASPxRoundPanel ID="pnlDateDepl" ClientInstanceName="pnlDateDepl" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Date deplasare" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblLocDepl" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Loc</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtLocatie" ClientInstanceName="txtLocatie" runat="server" >
								</dx:ASPxTextBox>
							</div>        
							<label id="lblTipDepl" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Tip deplasare</label>
							<div style="float:left; padding-right:15px;">    
								<dx:ASPxComboBox ID="cmbActionType" ClientInstanceName="cmbActionType" ClientIDMode="Static" runat="server"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
									<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbActionType'); }" />
								</dx:ASPxComboBox>
							</div>
							<label id="lblTipTrans" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Tip transport</label>
							<div style="float:left; padding-right:15px;">    
								<dx:ASPxComboBox ID="cmbTransportType" ClientInstanceName="cmbTransportType" ClientIDMode="Static" runat="server"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
									<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbTransportType'); }" />
								</dx:ASPxComboBox>
							</div>					
						</div>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblMotiv" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Motiv deplasare</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtActionReason" ClientInstanceName="txtActionReason" runat="server" Width="500px">
								</dx:ASPxTextBox>
							</div>       

						</div>	

						<div class="Absente_divOuter margin_top15">
							<label id="lblDtPlec" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data plecare</label>
							<div style="float:left; padding-right:10px;">
								<dx:ASPxDateEdit ID="txtStartDate" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" >
									<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtStartDate'); }" />
								</dx:ASPxDateEdit>
							</div>
							<label id="lblOraPlec" runat="server" style="display:inline-block; float:left; padding-right:15px;">Ora plecare</label>
							<div style="float:left; padding-right:10px;">
								<dx:ASPxTimeEdit  ID="txtOraPlecare" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)">
									<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtOraPlecare'); }" />
								</dx:ASPxTimeEdit>
							</div>
							<label id="lblDtSos" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data sosire</label>
							<div style="float:left; padding-right:10px;">
								<dx:ASPxDateEdit ID="txtEndDate" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" >
									<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtEndDate'); }" />
								</dx:ASPxDateEdit>
							</div>
							<label id="lblDtSf" runat="server" style="display:inline-block; float:left; padding-right:15px;">Ora sosire</label>
							<div style="float:left; padding-right:10px;">
								<dx:ASPxTimeEdit  ID="txtOraSosire" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)">
									<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtOraSosire'); }" />
								</dx:ASPxTimeEdit>
							</div>					
								
						</div>
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>	

                <dx:ASPxRoundPanel ID="pnlDatePlata" ClientInstanceName="pnlDatePlata" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Date plata" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
					
								<label id="lblMoneda" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Moneda</label>
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxComboBox ID="cmbMonedaAvans" ClientInstanceName="cmbMonedaAvans" ClientIDMode="Static" runat="server"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
										<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbMonedaAvans'); }" />
									</dx:ASPxComboBox>
								</div>
								<label id="lblModPlata" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Modalitate plata</label>
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxComboBox ID="cmbModPlata" ClientInstanceName="cmbModPlata" ClientIDMode="Static" runat="server"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
										<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbModPlata'); }" />
									</dx:ASPxComboBox>
								</div>			
								<label id="lblDiurna" runat="server" style="display:inline-block; float:left; padding-right:15px;">Diurna</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxCheckBox ID="chkIsDiurna" runat="server" Checked="false" >
										<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('chkIsDiurna'); }" />
									</dx:ASPxCheckBox>
								</div>								
							</div>
							
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>	


                <dx:ASPxRoundPanel ID="pnlDocJust" ClientInstanceName="pnlDocJust" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Documente justificative" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
					
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxGridView ID="grDateDocJust" runat="server" ClientInstanceName="grDateDocJust" ClientIDMode="Static" Width="65%" AutoGenerateColumns="false"   OnInitNewRow="grDateDocJust_InitNewRow" OnCommandButtonInitialize="grDateDocJust_CommandButtonInitialize"
										 OnRowInserting="grDateDocJust_RowInserting" OnRowUpdating="grDateDocJust_RowUpdating" OnRowDeleting="grDateDocJust_RowDeleting" OnHtmlEditFormCreated="grDateDocJust_HtmlEditFormCreated" >
										<SettingsBehavior AllowFocusedRow="true" />
										<Settings ShowFilterRow="False" ShowColumnHeaders="true" /> 
										<ClientSideEvents CustomButtonClick="function(s, e) { grDateDocJust_CustomButtonClick(s, e); }" EndCallback="function(s,e) { OnEndCallback(s,e); }" ContextMenu="ctx" />    
										<SettingsEditing Mode="EditFormAndDisplayRow" />
										<SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
										<Columns>
											<dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >
												<CustomButtons>
													<dx:GridViewCommandColumnCustomButton ID="btnAtasament">
														<Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/info.png" />
													</dx:GridViewCommandColumnCustomButton>
												</CustomButtons>
											</dx:GridViewCommandColumn>	
											<dx:GridViewDataTextColumn FieldName="Furnizor" Name="Furnizor" Caption="Furnizor"/>
											<dx:GridViewDataComboBoxColumn FieldName="DictionaryItemId" Name="DictionaryItemId" Caption="Document" Width="200px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>		
											<dx:GridViewDataTextColumn FieldName="DocNumberDecont" Name="DocNumberDecont" Caption="Numar"/>
											<dx:GridViewDataDateColumn FieldName="DocDateDecont" Name="DocDateDecont" Caption="Data"  Width="100px"  >
												<PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
											</dx:GridViewDataDateColumn>
											<dx:GridViewDataComboBoxColumn FieldName="CurrencyId" Name="CurrencyId" Caption="Moneda" Width="100px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>
											<dx:GridViewDataTextColumn FieldName="TotalPayment" Name="TotalPayment" Caption="Valoare">
												<PropertiesTextEdit DisplayFormatString="n2" />
											</dx:GridViewDataTextColumn>
											<dx:GridViewDataTextColumn FieldName="BugetLine" Name="BugetLine" Caption="Linie buget"/>
											<dx:GridViewDataComboBoxColumn FieldName="ExpenseTypeId" Name="ExpenseTypeId" Caption="Tip cheltuiala" Width="150px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>
											<dx:GridViewDataTextColumn FieldName="FreeTxt" Name="FreeTxt" Caption="Detalii"/>
										
											<dx:GridViewDataTextColumn FieldName="areFisier" Name="areFisier" Caption="areFisier" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="DocumentId" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="IdDocument" Name="IdDocument" Caption="IdDocument" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentDetailId" Name="DocumentDetailId" Caption="DocumentDetailId" Visible="false" ShowInCustomizationForm="false"/>
									
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

										<Templates>
											<EditForm>
												<div style="padding: 4px 3px 4px">
													<table>
														<tr>
															<td id="lblFurn" runat="server" style="padding-left:10px !important;">Furnizor</td>
															<td id="lblDocument" runat="server" style="padding-left:10px !important;">Document</td>
															<td id="lblNr" runat="server" style="padding-left:10px !important;">Numar</td>
															<td id="lblData" runat="server" style="padding-left:10px !important;">Data</td>
														</tr>
														<tr>
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtFurn" runat="server" Width="200px" Value='<%# Bind("Furnizor") %>' /></td>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbDoc" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("DictionaryItemId") %>' />
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNr" runat="server" Width="200px" Value='<%# Bind("DocNumberDecont") %>' /></td>
															<td style="padding:10px !important;"><dx:ASPxDateEdit ID="deData" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DocDateDecont") %>' /></td>
														</tr>
														<tr>
															<td id="lblMoneda" runat="server" style="padding-left:10px !important;">Moneda</td>
															<td id="lblVal" runat="server" style="padding-left:10px !important;">Valoare</td>
															<td id="lblLinbug" runat="server" style="padding-left:10px !important;">Linie buget</td>
															<td id="lblChelt" runat="server" style="padding-left:10px !important;">Tip cheltuiala</td>
														</tr>
														<tr>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbMoneda" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("CurrencyId") %>' />
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtVal" DisplayFormatString="N2" runat="server" Width="200px" Value='<%# Bind("TotalPayment") %>' /></td>
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtLinBug" runat="server" Width="200px" Value='<%# Bind("BugetLine") %>' /></td>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbChelt" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("ExpenseTypeId") %>' />
														</tr>
														<tr>
															<td id="lblDet" runat="server" style="padding-left:10px !important;">Detalii</td>
														</tr>
														<tr>
															<td style="padding:10px !important;" colspan="3" ><dx:ASPxTextBox ID="txtDet" runat="server" Width="600px" Value='<%# Bind("FreeTxt") %>' /></td>
														</tr>		
														<tr>
															<td style="padding:10px !important;">
																<div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
																	<dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
																	<dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
																</div>
															</td>
														</tr>
													</table>
												</div>
											</EditForm>
										</Templates>

									</dx:ASPxGridView>
								</div>							
							</div>
							
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>

                <dx:ASPxRoundPanel ID="pnlEstChelt" ClientInstanceName="pnlEstChelt" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Estimare cheltuieli" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
					
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxGridView ID="grDateEstChelt" runat="server" ClientInstanceName="grDateEstChelt" ClientIDMode="Static" Width="45%" AutoGenerateColumns="false"  OnInitNewRow="grDateEstChelt_InitNewRow"
										 OnRowInserting="grDateEstChelt_RowInserting" OnRowUpdating="grDateEstChelt_RowUpdating" OnRowDeleting="grDateEstChelt_RowDeleting" >
										<SettingsBehavior AllowFocusedRow="true" />
										<Settings ShowFilterRow="False" ShowColumnHeaders="true" /> 
										<ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" ContextMenu="ctx" />    
										<SettingsEditing Mode="Inline" />
										<SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
										<Columns>
											<dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >
		
											</dx:GridViewCommandColumn>	

											<dx:GridViewDataComboBoxColumn FieldName="DictionaryItemId" Name="DictionaryItemId" Caption="Tip" Width="200px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>		

											<dx:GridViewDataComboBoxColumn FieldName="CurrencyId" Name="CurrencyId" Caption="Moneda" Width="100px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>
											<dx:GridViewDataTextColumn FieldName="TotalPayment" Name="TotalPayment" Caption="Valoare">
												<PropertiesTextEdit DisplayFormatString="n2" />
											</dx:GridViewDataTextColumn>
											<dx:GridViewDataTextColumn FieldName="BugetLine" Name="BugetLine" Caption="Linie buget"/>
											<dx:GridViewDataTextColumn FieldName="FreeTxt" Name="FreeTxt" Caption="Detalii"/>
										
											<dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="DocumentId" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="IdDocument" Name="IdDocument" Caption="IdDocument" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentDetailId" Name="DocumentDetailId" Caption="DocumentDetailId" Visible="false" ShowInCustomizationForm="false"/>
						
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
								</div>							
							</div>
							
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>


                <dx:ASPxRoundPanel ID="pnlDateDec" ClientInstanceName="pnlDateDec" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Date decont" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
								<label id="lblValDec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Val. decontata</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxTextBox ID="txtValDecont" ClientInstanceName="txtValDecont" ClientEnabled="false" runat="server" Width="200px" DisplayFormatString="N2">
										<ClientSideEvents TextChanged="function(s, e) { pnlCtl.PerformCallback('txtValDecont'); }" />
									</dx:ASPxTextBox>
								</div>
							</div>	
							<div class="Absente_divOuter margin_top15">	
								<label id="lblValAvs" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Val. avans</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxTextBox ID="txtValAvans" ClientInstanceName="txtValAvans" ClientEnabled="false" runat="server" Width="200px" DisplayFormatString="N2">
										<ClientSideEvents TextChanged="function(s, e) { pnlCtl.PerformCallback('txtValAvans'); }" />
									</dx:ASPxTextBox>
								</div> 	
							</div>	
							<div class="Absente_divOuter margin_top15">	
								<label id="lblPlRec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Val. plata/recuperat</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxTextBox ID="txtValPlataBanca" ClientInstanceName="txtValPlataBanca" ClientEnabled="false" runat="server" Width="200px" DisplayFormatString="N2">
										<ClientSideEvents TextChanged="function(s, e) { pnlCtl.PerformCallback('txtValPlataBanca'); }" />
									</dx:ASPxTextBox>
								</div> 	
							</div>						
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>	
				
                <dx:ASPxRoundPanel ID="pnlPlataBanca" ClientInstanceName="pnlPlataBanca" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Restituire avans neutilizat" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
					
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxGridView ID="grDatePlataBanca" runat="server" ClientInstanceName="grDatePlataBanca" ClientIDMode="Static" Width="45%" AutoGenerateColumns="false"  OnInitNewRow="grDatePlataBanca_InitNewRow" OnCommandButtonInitialize="grDatePlataBanca_CommandButtonInitialize"
										 OnRowInserting="grDatePlataBanca_RowInserting" OnRowUpdating="grDatePlataBanca_RowUpdating" OnRowDeleting="grDatePlataBanca_RowDeleting" OnHtmlEditFormCreated="grDatePlataBanca_HtmlEditFormCreated" >
										<SettingsBehavior AllowFocusedRow="true" />
										<Settings ShowFilterRow="False" ShowColumnHeaders="true" /> 
										<ClientSideEvents CustomButtonClick="function(s, e) { grDatePlataBanca_CustomButtonClick(s, e); }" EndCallback="function(s,e) { OnEndCallback(s,e); }" ContextMenu="ctx" />    
										<SettingsEditing Mode="EditFormAndDisplayRow" />
										<SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
										<Columns>
											<dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >
												<CustomButtons>
													<dx:GridViewCommandColumnCustomButton ID="btnAtas">
														<Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/info.png" />
													</dx:GridViewCommandColumnCustomButton>
												</CustomButtons>
											</dx:GridViewCommandColumn>	
											<dx:GridViewDataComboBoxColumn FieldName="DictionaryItemId" Name="DictionaryItemId" Caption="Document" Width="200px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>		
											<dx:GridViewDataTextColumn FieldName="DocNumberDecont" Name="DocNumberDecont" Caption="Numar"/>
											<dx:GridViewDataDateColumn FieldName="DocDateDecont" Name="DocDateDecont" Caption="Data"  Width="100px"  >
												<PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
											</dx:GridViewDataDateColumn>
											<dx:GridViewDataComboBoxColumn FieldName="CurrencyId" Name="CurrencyId" Caption="Moneda" Width="100px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>
											<dx:GridViewDataTextColumn FieldName="TotalPayment" Name="TotalPayment" Caption="Valoare">
												<PropertiesTextEdit DisplayFormatString="n2" />
											</dx:GridViewDataTextColumn>

										
											<dx:GridViewDataTextColumn FieldName="areFisierPlataBanca" Name="areFisierPlataBanca" Caption="areFisierPlataBanca" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="DocumentId" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="IdDocument" Name="IdDocument" Caption="IdDocument" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentDetailId" Name="DocumentDetailId" Caption="DocumentDetailId" Visible="false" ShowInCustomizationForm="false"/>
											
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

										<Templates>
											<EditForm>
												<div style="padding: 4px 3px 4px">
													<table>
														<tr>
															<td id="lblDocument" runat="server" style="padding-left:10px !important;">Document</td>
															<td id="lblNr" runat="server" style="padding-left:10px !important;">Numar</td>
															<td id="lblData" runat="server" style="padding-left:10px !important;">Data</td>
														</tr>
														<tr>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbDoc" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("DictionaryItemId") %>' />
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNr" runat="server" Width="200px" Value='<%# Bind("DocNumberDecont") %>' /></td>
															<td style="padding:10px !important;"><dx:ASPxDateEdit ID="deData" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DocDateDecont") %>' /></td>
														</tr>
														<tr>
															<td id="lblMoneda" runat="server" style="padding-left:10px !important;">Moneda</td>
															<td id="lblVal" runat="server" style="padding-left:10px !important;">Valoare</td>
														</tr>
														<tr>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbMoneda" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("CurrencyId") %>' />
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtVal" DisplayFormatString="N2" runat="server" Width="200px" Value='<%# Bind("TotalPayment") %>' /></td>
														</tr>						
														<tr>
															<td style="padding:10px !important;">
																<div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
																	<dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
																	<dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
																</div>
															</td>
														</tr>
													</table>
												</div>
											</EditForm>
										</Templates>

									</dx:ASPxGridView>
								</div>							
							</div>
							
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>
			
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnMotivRespingere(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtv" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtv" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>


    <script>
        var limba = "<%= Session["IdLimba"] %>";
        function EndUpload(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
		}

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
			}
			pnlLoading.Hide();
			if (s.cp_InfoMessage != null) {
                swal({
                    title: "Avertisment", text: s.cp_InfoMessage,
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", closeOnConfirm: true
                }, function (isConfirm) {
						if (isConfirm) {
                            pnlLoading.Show();
							pnlCtl.PerformCallback('btnSaveConf');
                    }
                });
                s.cp_InfoMessage = null;
            }
            
		}

        function OnMotivRespingere(s, e) {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Nu ati completat motivul refuzului pentru respingere documente!"),
                    type: "warning"
                });
            }
            else {
                popUpMotiv.Hide();
                pnlCtl.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
            }
        }

        function grDateDocJust_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId;DocumentDetailId', GoToDoc);
                    break;
            }
		}

        function grDatePlataBanca_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId;DocumentDetailId', GoToDoc);
                    break;
            }
        }

        function GoToDoc(Value) {
            strUrl = getAbsoluteUrl + "AvansXDecont/relUploadDocumente.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Documente");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }
    </script>

</asp:Content>
