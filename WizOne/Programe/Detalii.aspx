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
                                    pnlCall.PerformCallback('btnSave');
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
                                        <dx:ASPxTextBox ID="txtDenumire" ClientIDMode="Static" Width="400" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                                            <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="Este obligatoriu" Display="Dynamic"/>
                                        </dx:ASPxTextBox>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumireScurta" runat="server" Text="Denumire scurta" Width="110"/>
                                        <dx:ASPxTextBox ID="txtDenumireScurta" ClientIDMode="Static" Width="100" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)"/>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxFormLayout ID="pnlTab" runat="server" AlignItemCaptionsInAllGroups="True" UseDefaultPaddings="true" AlignItemCaptions="true" Width="100%" Theme="Metropolis" ShowItemCaptionColon="false">
                                <SettingsAdaptivity></SettingsAdaptivity>
                                <Items>
                                    <dx:TabbedLayoutGroup>
                                        <SettingsTabPages EnableTabScrolling="false" />
                                        <Items>
                                            <dx:LayoutGroup Name="tabGeneral" Caption="Date Generale" UseDefaultPaddings="true" ColumnCount="3">
                                                <Items>
                                                    <dx:LayoutGroup Caption="Valabilitate" ColCount="2" SettingsItemCaptions-Location="Top">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Data Inceput" FieldName="DataInceput">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxDateEdit ID="ctlDataInceput" ClientIDMode="Static" runat="server" AutoPostBack="false" Width="100" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)">                                         
                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                            <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="Este obligatoriu"/>
							                                            </dx:ASPxDateEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Tip Pontare" FieldName="TipPontare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlTipPontare" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Denumire" ValueField="Id" ValueType="System.Int32" oncontextMenu="ctx(this,event)">
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
                                                            <dx:LayoutItem Caption="Data Sfarsit" FieldName="DataSfarsit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxDateEdit ID="ctlDataSfarsit" ClientIDMode="Static" runat="server" AutoPostBack="false" Width="100" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)">                                         
                                                                            <CalendarProperties FirstDayOfWeek="Monday" />
                                                                            <ValidationSettings RequiredField-IsRequired="true" RequiredField-ErrorText="Este obligatoriu"></ValidationSettings>
							                                            </dx:ASPxDateEdit>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Norma" FieldName="Norma">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxSpinEdit ID="ctlNorma" ClientIDMode="Static" runat="server" Width="100" SpinButtons-ShowIncrementButtons="false" MaxLength="4" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Flexibil" FieldName="Flexibil">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxCheckBox ID="ctlFlexibil" ClientIDMode="Static" runat="server" ToggleSwitchDisplayMode="Always" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ore de noapte?" FieldName="DeNoapte">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxCheckBox ID="ctlDeNoapte" ClientIDMode="Static" runat="server" ToggleSwitchDisplayMode="Always" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ora Intrare" FieldName="OraIntrare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOraIntrare" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Pauza minima" FieldName="PauzaMin">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlPauzaMin" ClientIDMode="Static" runat="server" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Ora Iesire" FieldName="OraIesire">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOraIesire" ClientInstanceName="txtOraOut" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
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
							                                            <dx:ASPxComboBox ID="ctlONRotunjire" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Denumire" ValueField="Id" ValueType="System.Int32" oncontextMenu="ctx(this,event)">
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
							                                            <dx:ASPxComboBox ID="ctlONCamp" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
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
							                                            <dx:ASPxComboBox ID="ctlOSRotunjire" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Denumire" ValueField="Id" ValueType="System.Int32" oncontextMenu="ctx(this,event)">
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
							                                            <dx:ASPxComboBox ID="ctlOSCamp" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Val Min" FieldName="OSValMin">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOSValMin" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Trimite ce este sub val. min. la" FieldName="OSCampSub">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOSCampSub" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Val Max" FieldName="OSValMax">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOSValMax" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Trimite ce este peste val. max. la" FieldName="OSCampPeste">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOSCampPeste" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Name="tabOreNoapte" Caption="Ore Noapte" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDateNoapte" runat="server" ClientInstanceName="grDateNoapte" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate" oncontextMenu="ctx(this,event)" Theme="Metropolis">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="200px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDownList">
                                                                                <Items>
                                                                                    <dx:ListEditItem Value="1" Text="rotunjire la minute" />
                                                                                    <dx:ListEditItem Value="2" Text="rotunjire la ora" />
                                                                                    <dx:ListEditItem Value="3" Text="trunchiere la ora" />
                                                                                    <dx:ListEditItem Value="4" Text="rotunjire la 45 minute" />
                                                                                    <dx:ListEditItem Value="5" Text="rotunjire la 10 minute" />
                                                                                    <dx:ListEditItem Value="6" Text="rotunjire la 5 minute" />
                                                                                </Items>
                                                                            </PropertiesComboBox>
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
                                                                        <dx:GridViewDataSpinEditColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px" PropertiesSpinEdit-SpinButtons-ClientVisible="false" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="9" PropertiesSpinEdit-MaxLength="1"/>
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="150px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDownList" />
                                                                        </dx:GridViewDataComboBoxColumn>

                                                                        <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="IdProgram" Visible="false" ShowInCustomizationForm="false"/>
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
                                            <dx:LayoutGroup Name="tabAlte" Caption="Alte Ore" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDateAlte" runat="server" ClientInstanceName="grDateAlte" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate" oncontextMenu="ctx(this,event)" Theme="Metropolis">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="200px" >
                                                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDownList">
                                                                                <Items>
                                                                                    <dx:ListEditItem Value="1" Text="rotunjire la minute" />
                                                                                    <dx:ListEditItem Value="2" Text="rotunjire la ora" />
                                                                                    <dx:ListEditItem Value="3" Text="trunchiere la ora" />
                                                                                    <dx:ListEditItem Value="4" Text="rotunjire la 45 minute" />
                                                                                    <dx:ListEditItem Value="5" Text="rotunjire la 10 minute" />
                                                                                    <dx:ListEditItem Value="6" Text="rotunjire la 5 minute" />
                                                                                </Items>
                                                                            </PropertiesComboBox>
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
                                                                        <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="150px" >
                                                                            <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDownList" />
                                                                        </dx:GridViewDataComboBoxColumn> 

                                                                        <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="IdProgram" Visible="false" ShowInCustomizationForm="false"/>
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
                                            <dx:LayoutGroup Name="tabPauze" Caption="Pauze" SettingsItemCaptions-Location="Top">
                                                <Items>
                                                    <dx:LayoutGroup Caption="" SettingsItemCaptions-Location="Left" ColumnCount="4">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Timp pauza" FieldName="PauzaTimp">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlPauzaTimp" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Pauza dedusa" FieldName="PauzaDedusa">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxCheckBox ID="ctlPauzaDedusa" ClientIDMode="Static" runat="server" ToggleSwitchDisplayMode="Always" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Orwe minim lucrate" FieldName="OreLucrateMin">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOreLucrateMin" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Pauza scutita" FieldName="PauzaScutita">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlPauzaScutita" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
                                                    <dx:LayoutItem Caption="">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDatePauze" runat="server" ClientInstanceName="grDatePauza" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate" Theme="Metropolis" oncontextMenu="ctx(this,event)">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                             
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora Inceput de la" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora Inceput la" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora sfarsit" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora sfarsit de la" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora sfarsit la" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataCheckColumn FieldName="TimpDedus" Name="TimpDedus" Caption="Timp dedus"  Width="110px" PropertiesCheckEdit-ToggleSwitchDisplayMode="Always"/>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="TimpMin" Name="TimpMin" Caption="Timp min" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="TimpMax" Name="TimpMax" Caption="Timp max" Width="110px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataCheckColumn FieldName="FaraMarja" Name="FaraMarja" Caption="Fara Marja"  Width="110px"  />

                                                                        <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="IdProgram" Visible="false" ShowInCustomizationForm="false"/>
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
                                            <dx:LayoutGroup Name="tabIntrare" Caption="Intrare" SettingsItemCaptions-Location="Top" ColumnCount="3">
                                                <Items>
                                                    <dx:LayoutGroup Caption="Anticipata" SettingsItemCaptions-Location="Left">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Diferenta raportare" FieldName="INSubDiferentaRaportare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlINSubDiferentaRaportare" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Minim pentru plata" FieldName="INSubMinPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlINSubMinPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Maximum platit" FieldName="INSubMaxPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlINSubMaxPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom"  oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp platit la" FieldName="INSubCampPlatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlINSubCampPlatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp neplatit la" FieldName="INSubCampNeplatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlINSubCampNeplatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
                                                    <dx:LayoutGroup Caption="Tarzie" SettingsItemCaptions-Location="Left">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Diferenta raportare" FieldName="INPesteDiferentaRaportare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlINPesteDiferentaRaportare" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Diferenta plata" FieldName="INPesteDiferentaPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlINPesteDiferentaPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp platit la" FieldName="INPesteCampPlatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlINPesteCampPlatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp neplatit la" FieldName="INPesteCampNeplatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlINPesteCampNeplatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="" CaptionSettings-Location="Top">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                <dx:ASPxGridView ID="grDateIntrare" runat="server" ClientInstanceName="grDateIntrare" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate" Theme="Metropolis" oncontextMenu="ctx(this,event)">
                                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                    <SettingsSearchPanel Visible="false" />
                                                                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                    <SettingsPager Mode="ShowAllRecords" />
                                                                    <ClientSideEvents ContextMenu="ctx" />
                                                                    <Columns>
                                                                        <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Timp de la" Width="150px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Timp la" Width="150px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>
                                                                        <dx:GridViewDataTimeEditColumn FieldName="Valoare" Name="Valoare" Caption="Valoare" Width="150px">
                                                                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                        </dx:GridViewDataTimeEditColumn>

                                                                        <dx:GridViewDataTextColumn FieldName="TipInOut" Name="TipInOut" Caption="TipInOut" Visible="false" ShowInCustomizationForm="false"/>
                                                                        <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="IdProgram" Visible="false" ShowInCustomizationForm="false"/>
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
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Name="tabIesire" Caption="Iesire" SettingsItemCaptions-Location="Top" ColumnCount="3">
                                                <Items>
                                                    <dx:LayoutGroup Caption="Anticipata" SettingsItemCaptions-Location="Left">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Diferenta raportare" FieldName="OUTSubDiferentaRaportare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOUTSubDiferentaRaportare" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Diferenta plata" FieldName="OUTSubDiferentaPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOUTSubDiferentaPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp platit la" FieldName="OUTSubCampPlatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOUTSubCampPlatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp neplatit la" FieldName="OUTSubCampNeplatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOUTSubCampNeplatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="" CaptionSettings-Location="Top">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                                                        <dx:ASPxGridView ID="grDateIesire" runat="server" ClientInstanceName="grDateIesire" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate" Theme="Metropolis" oncontextMenu="ctx(this,event)">
                                                                            <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                                            <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
                                                                            <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                                            <SettingsSearchPanel Visible="false" />
                                                                            <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                                            <SettingsPager Mode="ShowAllRecords" />
                                                                            <ClientSideEvents ContextMenu="ctx" />
                                                                            <Columns>
                                                                                <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                                                <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Timp de la" Width="150px">
                                                                                    <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                                </dx:GridViewDataTimeEditColumn>
                                                                                <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Timp la" Width="150px">
                                                                                    <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                                </dx:GridViewDataTimeEditColumn>
                                                                                <dx:GridViewDataTimeEditColumn FieldName="Valoare" Name="Valoare" Caption="Valoare" Width="150px">
                                                                                    <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                                                </dx:GridViewDataTimeEditColumn>

                                                                                <dx:GridViewDataTextColumn FieldName="TipInOut" Name="TipInOut" Caption="TipInOut" Visible="false" ShowInCustomizationForm="false"/>
                                                                                <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="IdProgram" Visible="false" ShowInCustomizationForm="false"/>
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
                                                    <dx:LayoutGroup Caption="Tarzie" SettingsItemCaptions-Location="Left">
                                                        <Items>
                                                            <dx:LayoutItem Caption="Diferenta raportare" FieldName="OUTPesteDiferentaRaportare">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOUTPesteDiferentaRaportare" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Minim pentru plata" FieldName="OUTPesteMinPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOUTPesteMinPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Maximum platit" FieldName="OUTPesteMaxPlata">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxTimeEdit ID="ctlOUTPesteMaxPlata" ClientIDMode="Static" runat="server" Width="100" DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp platit la" FieldName="OUTPesteCampPlatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOUTPesteCampPlatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                            <dx:LayoutItem Caption="Transfer timp neplatit la" FieldName="OUTPesteCampNeplatit">
                                                                <LayoutItemNestedControlCollection>
                                                                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
							                                            <dx:ASPxComboBox ID="ctlOUTPesteCampNeplatit" ClientIDMode="Static" runat="server" DropDownStyle="DropDownList" TextField="Alias" ValueField="Denumire" ValueType="System.String" oncontextMenu="ctx(this,event)"/>
                                                                    </dx:LayoutItemNestedControlContainer>
                                                                </LayoutItemNestedControlCollection>
                                                            </dx:LayoutItem>
                                                        </Items>
                                                    </dx:LayoutGroup>
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