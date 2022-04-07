<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="DocumentAvans.aspx.cs" Inherits="WizOne.AvansXDecont.DocumentAvans" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

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
        }

        function OnGrDateEndCallback(s, e) {
            pnlLoading.Hide();
            pnlCtl.PerformCallback('SumaAvans');
        }

        function OnMotivRespingere() {
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

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'DocumentId;DocumentDetailId', GoToDoc);
                    break;
            }
        }
        function GoToDoc(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=22&id=' + Value, '_blank ')
        }


        var textSeparator = ";";
        function OnListBoxSelectionChanged(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState();
            UpdateText();
        }
        function UpdateSelectAllItemState() {
            IsAllSelected() ? checkListBox.SelectIndices([0]) : checkListBox.UnselectIndices([0]);
        }
        function IsAllSelected() {
            var selectedDataItemCount = checkListBox.GetItemCount() - (checkListBox.GetItem(0).selected ? 0 : 1);
            return checkListBox.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText() {
            var selectedItems = checkListBox.GetSelectedItems();
            cmbTip.SetText(GetSelectedItemsText(selectedItems));
        }
        function SynchronizeListBoxValues(dropDown, args) {
            checkListBox.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts(texts);
            checkListBox.SelectValues(values);
            UpdateSelectAllItemState();
            UpdateText();
        }
        function GetSelectedItemsText(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }
    </script>

    <table style="width:100%">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/Salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aprobare" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>				
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge"  AutoPostBack="false" oncontextMenu="ctx(this,event)" >              
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                    <ClientSideEvents Click="function(s, e) {
                        popUpMotiv.Show();
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
							<label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Angajat</label>
							<div style="float:left; padding-right:15px;">    
								<dx:ASPxComboBox ID="cmbAngajat" ClientInstanceName="cmbAngajat" ClientIDMode="Static" runat="server" Width="290" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
									<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbAngajat'); }" />
								</dx:ASPxComboBox>
							</div>
						</div>

						<div class="Absente_divOuter margin_top15">
						
							<label id="lblNrOrdin" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nr. ordin</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtNrOrdinDeplasare" ClientInstanceName="txtNrOrdinDeplasare" runat="server" ClientEnabled="false" Width="100px">
								</dx:ASPxTextBox>
							</div>        
							<label id="lblNume" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px; padding-left:190px;">Nume</label>
							<div style="float:left; padding-right:15px; padding-left:85px;">
								<dx:ASPxTextBox ID="txtNumeComplet" ClientInstanceName="txtNumeComplet" ClientEnabled="false" runat="server" Width="270px">
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
								<dx:ASPxTextBox ID="txtLocMunca" ClientInstanceName="txtLocMunca" ClientEnabled="false" runat="server" Width="270px">
								</dx:ASPxTextBox>
							</div> 
						</div>	

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblIBAN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;" oncontextMenu="ctx(this,event)">IBAN</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtContIban" ClientInstanceName="txtContIban" ClientEnabled="false" runat="server" Width="290px" oncontextMenu="ctx(this,event)">
								</dx:ASPxTextBox>
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
				
							<label id="lblLocDepl" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Locul deplasarii</label>
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
								<dx:ASPxComboBox ID="cmbTransportType" ClientInstanceName="cmbTransportType" ClientIDMode="Static" runat="server"  Width="100" ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
									<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbTransportType'); }" />
								</dx:ASPxComboBox>
							</div>					
						</div>

						<div class="Absente_divOuter margin_top15">
				
							<label id="lblMotiv" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Motiv deplasare</label>
							<div style="float:left; padding-right:15px;">
								<dx:ASPxTextBox ID="txtActionReason" ClientInstanceName="txtActionReason" runat="server" Width="670px">
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
							<label id="lblDtSos" runat="server" style="display:inline-block; float:left; padding-right:15px; padding-left:35px;">Data sosirii din delegatie</label>
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
					
								<label id="lblMoneda" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Moneda avans/decont</label>
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxComboBox ID="cmbMonedaAvans" ClientInstanceName="cmbMonedaAvans" ClientIDMode="Static" runat="server" Width="90"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
										<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbMonedaAvans'); }" />
									</dx:ASPxComboBox>
								</div>
								<label id="lblModPlata" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Modalitate plata</label>
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxComboBox ID="cmbModPlata" ClientInstanceName="cmbModPlata" ClientIDMode="Static" runat="server" Width="120"  ValueField="DictionaryItemId" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
										<ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbModPlata'); }" />
									</dx:ASPxComboBox>
								</div>			
								<label id="lblDiurna" runat="server" style="display:inline-block; float:left; padding-right:15px;">Deplasare cu diurna</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxCheckBox ID="chkIsDiurna" runat="server" Checked="false" >
										<ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('chkIsDiurna'); }" />
									</dx:ASPxCheckBox>
								</div>								
							</div>
							
						
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>	


                <dx:ASPxRoundPanel ID="pnlCheltEst" ClientInstanceName="pnlCheltEst" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Cheltuieli estimate" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
					
								<div style="float:left; padding-right:15px;">    
									<dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="45%" AutoGenerateColumns="false"  OnInitNewRow="grDate_InitNewRow" 
										 OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize">
										<SettingsBehavior AllowFocusedRow="true" />
										<Settings ShowFilterRow="False" ShowColumnHeaders="true" /> 
										<ClientSideEvents CustomButtonClick="function(s, e) { grDate_CustomButtonClick(s, e); }" EndCallback="function(s,e) { OnGrDateEndCallback(s,e); }" ContextMenu="ctx" />    
										<SettingsEditing Mode="EditFormAndDisplayRow" />
										<SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
										<Columns>
											<dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >
												<CustomButtons>
													<dx:GridViewCommandColumnCustomButton ID="btnAtasament">
														<Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
													</dx:GridViewCommandColumnCustomButton>
												</CustomButtons>
											</dx:GridViewCommandColumn>	
											<dx:GridViewDataComboBoxColumn FieldName="DictionaryItemId" Name="DictionaryItemId" Caption="Cheltuiala" Width="200px" >
												<Settings SortMode="DisplayText" />
												<PropertiesComboBox TextField="DictionaryItemName" ValueField="DictionaryItemId" ValueType="System.Int32" DropDownStyle="DropDown" />
											</dx:GridViewDataComboBoxColumn>			
											<dx:GridViewDataTextColumn FieldName="Amount" Name="Amount" Caption="Valoare">
												   <PropertiesTextEdit DisplayFormatString="n2" />
											</dx:GridViewDataTextColumn>
											<dx:GridViewDataTextColumn FieldName="FreeTxt" Name="FreeTxt" Caption="Detalii"/>
										
											<dx:GridViewDataTextColumn FieldName="areFisier" Name="areFisier" Caption="areFisier" Visible="false" ShowInCustomizationForm="false"/>
											<dx:GridViewDataTextColumn FieldName="DocumentId" Name="DocumentId" Caption="DocumentId" Visible="false" ShowInCustomizationForm="false"/>
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
															<td id="lblChelt" runat="server" style="padding-left:10px !important;">Cheltuiala</td>
															<td id="lblVal" runat="server" style="padding-left:10px !important;">Valoare</td>
														</tr>
														<tr>
															<td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbChelt" runat="server" Width="200px" ValueField="DictionaryItemId" DropDownWidth="200" TextField="DictionaryItemName" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("DictionaryItemId") %>' />
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtVal" runat="server" Width="200px" DisplayFormatString="N2" Value='<%# Bind("Amount") %>' /></td>
														</tr>
														<tr>
															<td id="lblDet" runat="server" style="padding-left:10px !important;">Detalii</td>
														</tr>
														<tr>
															<td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtDet" runat="server" Width="200px" Value='<%# Bind("FreeTxt") %>' /></td>
														</tr>
														<tr>
															<td style="padding:10px !important;" colspan="2">
																<label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
																<dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
																	BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
																	ClientInstanceName="btnDocUpload" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
																	<BrowseButton>
																		<Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
																	</BrowseButton>
																	<ValidationSettings ShowErrors="False"></ValidationSettings>

																	<ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
																</dx:ASPxUploadControl>
															</td>
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


                <dx:ASPxRoundPanel ID="pnlDateAv" ClientInstanceName="pnlDateAv" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="false" AllowCollapsingByHeaderClick="true" HeaderText="Date avans" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

							<div class="Absente_divOuter margin_top15">
								<label id="lblValEst" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Val. estimata</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxTextBox ID="txtValEstimata" ClientInstanceName="txtValEstimata" runat="server" Width="100px"  DisplayFormatString="N2" ClientEnabled="false">
										<ClientSideEvents TextChanged="function(s, e) { pnlCtl.PerformCallback('txtValEstimata'); }" />										
									</dx:ASPxTextBox>
								</div>
							</div>	
							<div class="Absente_divOuter margin_top15">	
								<label id="lblValAvsSol" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Valoare avans solicitat</label>
								<div style="float:left; padding-right:15px;">
									<dx:ASPxTextBox ID="txtValAvans" ClientInstanceName="txtValAvans" runat="server" Width="100px" DisplayFormatString="N2">
										<ClientSideEvents TextChanged="function(s, e) { pnlCtl.PerformCallback('txtValAvans'); }" />
									</dx:ASPxTextBox>
								</div> 	
							</div>	
							<div class="Absente_divOuter margin_top15">
								<label id="lblDtScad" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data scadenta</label>
								<div style="float:left; padding-right:10px;">
									<dx:ASPxDateEdit ID="dtDueDate" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" />
								</div>
							</div>	
							<div class="Absente_divOuter margin_top15">
								<label id="lblRez" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Rezervari</label>
								<div style="float:left; padding-right:15px;">    
    								<dx:ASPxDropDownEdit ClientInstanceName="cmbTip" ID="cmbTip" Width="210px" runat="server" AnimationType="None">
        								<DropDownWindowStyle BackColor="#EDEDED" />
        								<DropDownWindowTemplate>
            								<dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                								<Border BorderStyle="None" />
                								<BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />             
                								<ClientSideEvents SelectedIndexChanged="OnListBoxSelectionChanged" />
            								</dx:ASPxListBox>
            								<table style="width: 100%">
                								<tr>
                    								<td style="padding: 4px">
                        								<dx:ASPxButton ID="btnClose" AutoPostBack="False" runat="server" Text="Inchide" style="float: right">
                            								<ClientSideEvents Click="function(s, e){ cmbTip.HideDropDown(); }" />
                        								</dx:ASPxButton>
                    								</td>
                								</tr>
            								</table>
        								</DropDownWindowTemplate>
        								<ClientSideEvents TextChanged="SynchronizeListBoxValues"  />
    								</dx:ASPxDropDownEdit>
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


</asp:Content>
