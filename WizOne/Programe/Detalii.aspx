<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Programe.Detalii" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxCallbackPanel ID="pnlCall" ClientIDMode="Static" ClientInstanceName="pnlCall" ScrollBars="None" runat="server" OnCallback="pnlCall_Callback" SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents EndCallback="function (s,e) { OnPanelEndCallback(); }"/>
        <PanelCollection>
            <dx:PanelContent>
		        <table style="width:100%">
			        <tr>
                        <td class="pull-left">
                            <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
                        </td>			
				        <td class="pull-right">
                            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                                <ClientSideEvents Click="function(s, e) {
                                    pnlLoading.Show();
                                    pnlCtl.PerformCallback('btnSave');
                                }" />
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                            </dx:ASPxButton>				
					        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="Lista.aspx" >
						        <Image Url="../Fisiere/Imagini/Icoane/iesire.png"></Image>
					        </dx:ASPxButton>
				        </td>		
			        </tr>	
                    <tr>
                        <td colspan="2">
                            <div class="row">
                                <div class="col-md-12" style="margin-bottom:20px;">
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblId" runat="server" Text="Id" Width="30"/>
                                        <dx:ASPxTextBox ID="txtId" Width="50" runat="server" ClientEnabled="false"/>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumire" runat="server" Text="Denumire" Width="70"/>
                                        <dx:ASPxTextBox ID="txtDenumire" Width="400" runat="server" AutoPostBack="false"/>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumireScurta" runat="server" Text="Denumire scurta" Width="110"/>
                                        <dx:ASPxTextBox ID="txtDenumireScurta" Width="100" runat="server" AutoPostBack="false"/>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="2">
                            <dx:ASPxFormLayout ID="pnlGen" runat="server" AlignItemCaptionsInAllGroups="True" UseDefaultPaddings="true" AlignItemCaptions="true" Width="100%" Theme="Office365">
                                <SettingsAdaptivity></SettingsAdaptivity>
                                <Items>
                                    <dx:TabbedLayoutGroup>
                                        <SettingsTabPages EnableTabScrolling="false" />
                                        <Items>
                                            <dx:LayoutGroup Caption="Date Generale" UseDefaultPaddings="true" ColumnCount="3">
                                                <Items>
                                                    <dx:LayoutGroup Caption="Valabilitate" ColCount="2" SettingsItemCaptions-Location="Top">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Data Inceput" FieldName="DataInceput">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxDateEdit ID="txtDtInc" runat="server" AutoPostBack="false" Width="100" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom">                                         
                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                            <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="Este obligatoriu"></ValidationSettings>
							                                            </dx:ASPxDateEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Data Sfarsit" FieldName="DataSfarsit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxDateEdit ID="txtDtSf" runat="server" AutoPostBack="false" Width="100" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom">                                         
                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                            <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="Este obligatoriu"></ValidationSettings>
							                                            </dx:ASPxDateEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Norma" FieldName="Norma">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxSpinEdit ID="txtNorma" runat="server" SpinButtons-ShowIncrementButtons="false">
                                                                        </dx:ASPxSpinEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Tip Pontare" FieldName="TipPontare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbTipPont" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                                            <Items>
                                                                                <dx:ListEditItem Value="1" Text="Pontare automata" />
                                                                                <dx:ListEditItem Value="2" Text="Pontare automata la minim o citire card" />
                                                                                <dx:ListEditItem Value="3" Text="Pontare doar prima intrare si ultima iesire" />
                                                                                <dx:ListEditItem Value="4" Text="Pontare toate intrarile si iesirile" />
                                                                                <dx:ListEditItem Value="5" Text="Pontare prima intrare, ultima iesire - pauze > x minute" />
                                                                            </Items>
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Pauza minima" FieldName="PauzaMin">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="txtPauza" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"> 
							                                            </dx:ASPxTimeEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ore de noapte?" FieldName="DeNoapte">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxCheckBox ID="chkNoapte" runat="server">
                                                                        </dx:ASPxCheckBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Flexibil" FieldName="Flexibil">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxCheckBox ID="chkFlex"  runat="server" />
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ora Intrare" FieldName="OraIntrare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="txtOraIn" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"> 
							                                            </dx:ASPxTimeEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ora Iesire" FieldName="OraIesire">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="txtOraOut" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"> 
							                                            </dx:ASPxTimeEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
                                                    
                                                    <dx:LayoutGroup Caption="Ore Normale" SettingsItemCaptions-Location="Top">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Rotunjire" FieldName="ONRotunjire">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbONRotunjire" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                                            <Items>
                                                                                <dx:ListEditItem Value="1" Text="rotunjire la minute" />
                                                                                <dx:ListEditItem Value="2" Text="rotunjire la ora" />
                                                                                <dx:ListEditItem Value="3" Text="trunchiere la ora" />
                                                                                <dx:ListEditItem Value="4" Text="rotunjire la 45 minute" />
                                                                                <dx:ListEditItem Value="5" Text="rotunjire la 10 minute" />
                                                                                <dx:ListEditItem Value="6" Text="rotunjire la 5 minute" />
                                                                            </Items>
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer la" FieldName="ONCamp">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbONCamp" runat="server" DropDownStyle="DropDown" TextField="Alias" ValueField="Denumire" ValueType="System.Int32">
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>

                                                    <dx:LayoutGroup Caption="Ore Suplimentare" ColumnCount="2" SettingsItemCaptions-Location="Top">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Rotunjire" FieldName="OSRotunjire">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbOSRotunjire" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                                            <Items>
                                                                                <dx:ListEditItem Value="1" Text="rotunjire la minute" />
                                                                                <dx:ListEditItem Value="2" Text="rotunjire la ora" />
                                                                                <dx:ListEditItem Value="3" Text="trunchiere la ora" />
                                                                                <dx:ListEditItem Value="4" Text="rotunjire la 45 minute" />
                                                                                <dx:ListEditItem Value="5" Text="rotunjire la 10 minute" />
                                                                                <dx:ListEditItem Value="6" Text="rotunjire la 5 minute" />
                                                                            </Items>
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer la" FieldName="OSCamp">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbOSCamp" runat="server" DropDownStyle="DropDown" TextField="Alias" ValueField="Denumire" ValueType="System.Int32">
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Val Min" FieldName="OSValMin">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="txtOSValMin" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"> 
							                                            </dx:ASPxTimeEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Trimite ce este sub val. min. la" FieldName="OSCampSub">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbOSCampSub" runat="server" DropDownStyle="DropDown" TextField="Alias" ValueField="Denumire" ValueType="System.Int32">
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Val Max" FieldName="OSValMax">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="txtOSValMax" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"> 
							                                            </dx:ASPxTimeEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Trimite ce este peste val. max. la" FieldName="OSCampPeste">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="cmbOSCampPeste" runat="server" DropDownStyle="DropDown" TextField="Alias" ValueField="Denumire" ValueType="System.Int32">
							                                            </dx:ASPxComboBox>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Ore Noapte" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDateNoapte" runat="server" ClientInstanceName="grDateNoapte" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDateNoapte_BatchUpdate">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="350px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn>   
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                                                                            <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                                                                        </dx:GridViewDataTextColumn>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                                                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                                    </Columns>
                                                                    <SettingsCommandButton>
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
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>                                                    
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Alte Ore" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDateAlte" runat="server" ClientInstanceName="grDateAlte" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDateAlte_BatchUpdate">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="350px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn>   
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                                                                            <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                                                                        </dx:GridViewDataTextColumn>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                                                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                                    </Columns>
                                                                    <SettingsCommandButton>
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
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>                                                    
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Pauze" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="ASPxGridView2" runat="server" ClientInstanceName="grDateNoapte" ClientIDMode="Static" AutoGenerateColumns="false">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="350px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn>   
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                                                                            <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                                                                        </dx:GridViewDataTextColumn>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                                                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                                    </Columns>
                                                                    <SettingsCommandButton>
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
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>                                                    
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Intrare" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="ASPxGridView3" runat="server" ClientInstanceName="grDateNoapte" ClientIDMode="Static" AutoGenerateColumns="false">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="350px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn>   
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                                                                            <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                                                                        </dx:GridViewDataTextColumn>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                                                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                                    </Columns>
                                                                    <SettingsCommandButton>
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
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>                                                    
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Iesire" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="ASPxGridView4" runat="server" ClientInstanceName="grDateNoapte" ClientIDMode="Static" AutoGenerateColumns="false">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="350px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn>   
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="80px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                                                                            <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                                                                        </dx:GridViewDataTextColumn>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                                                                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                                    </Columns>
                                                                    <SettingsCommandButton>
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
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>                                                    
                                                </Items>
                                            </dx:LayoutGroup>

                                        </Items>
                                    </dx:TabbedLayoutGroup>
                                </Items>
                            </dx:ASPxFormLayout>
                        </td>
                    </tr>
		        </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

    <script>
        function OnPanelEndCallback() {
            pnlLoading.Hide();
        }
    </script>
</asp:Content>	