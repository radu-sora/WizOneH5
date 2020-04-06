<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Contracte.Detalii" %>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<dx:ASPxCallbackPanel ID="pnlSectiune" ClientIDMode="Static" ClientInstanceName="pnlSectiune" ScrollBars="Vertical" runat="server" OnCallback="pnlSectiune_Callback" SettingsLoadingPanel-Enabled="false">
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
                                pnlSectiune.PerformCallback('btnSave');
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
                        <div  style="margin:15px 0px; display:inline-block;">
                            <div class="ctl_inline">
                                <label id="lblId" runat="server">Id</label>
                                <dx:ASPxTextBox ID="txtId" Width="50" runat="server" ClientEnabled="false"/>
                            </div>
                            <div class="ctl_inline">
                                <label id="lblDenumire" runat="server">Denumire</label>
                                <dx:ASPxTextBox ID="txtDenumire" Width="400" runat="server" AutoPostBack="false"/>
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <dx:ASPxPageControl ID="tabCtr" ClientInstanceName="tabCtr" runat="server" Width="100%" Height="100%" CssClass="dxtcFixed" ActiveTabIndex="0">
                            <TabPages>
                                <dx:TabPage Text="Absente">
                                    <ContentCollection>
                                        <dx:ContentControl ID="ContentControl1" runat="server">
                                            <div  style="margin:15px 0px; display:inline-block;">
                                                <div class="ctl_inline">
                                                    <label id="lblOraSchIn" runat="server">Ora Schimbare In</label>
							                        <dx:ASPxTimeEdit ID="txtOraSchIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblOraSchOut" runat="server">Ora Schimbare Out</label>
							                        <dx:ASPxTimeEdit ID="txtOraSchOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblOreSup" runat="server">Ore suplimentare</label>
                                                    <dx:ASPxCheckBox ID="chkOreSup" runat="server" AutoPostBack="false"/>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblAfisare" runat="server">Afisare ore</label>
							                        <dx:ASPxComboBox ID="cmbAfisare" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150">
                                                        <Items>
                                                            <dx:ListEditItem Text="Trunchiere la ore" Value="1" />
                                                            <dx:ListEditItem Text="Cu minute" Value="2" />
                                                            <dx:ListEditItem Text="Cu zecimale" Value="3" />
                                                        </Items>
							                        </dx:ASPxComboBox>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblRap" runat="server">Tip raportare ore noapte</label>
    							                    <dx:ASPxComboBox ID="cmbRap" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150">
                                                        <Items>
                                                            <dx:ListEditItem Text="Pe inceput de schimb" Value="1" />
                                                            <dx:ListEditItem Text="Pe sfarsit de schimb" Value="2" />
                                                        </Items>
                                                    </dx:ASPxComboBox>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblPontareAuto" runat="server">Initializare automata</label>
                                                    <dx:ASPxCheckBox ID="chkPontareAuto" runat="server" AutoPostBack="false"/>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblOraInInit" runat="server">Ora intrare</label>
                                                    <dx:ASPxTimeEdit  ID="txtOraIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                                </div>
                                                <div class="ctl_inline">
                                                    <label id="lblOraOut" runat="server">Ora iesire</label>
                                                    <dx:ASPxTimeEdit  ID="txtOraOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                                </div>
                                            </div>
                                    
                                            <dx:ASPxGridView ID="grDateAbs" runat="server" ClientInstanceName="grDateAbs" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDateAbs_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <SettingsPager Mode="ShowAllRecords" />
                                                <ClientSideEvents ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
                                                    <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" Width="350px" >
                                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                                    </dx:GridViewDataComboBoxColumn>   
                                                    <dx:GridViewDataCheckColumn FieldName="ZL" Name="ZL" Caption="Zile lucratoare" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
                                                    <dx:GridViewDataCheckColumn FieldName="S" Name="S" Caption="Sambata" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
                                                    <dx:GridViewDataCheckColumn FieldName="D" Name="D" Caption="Duminica" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
                                                    <dx:GridViewDataCheckColumn FieldName="SL" Name="SL" Caption="Sarb. Legale" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
                                                    <dx:GridViewDataCheckColumn FieldName="InPontajAnual" Name="InPontajAnual" Caption="Istoric extins" Width="90px" HeaderStyle-HorizontalAlign="Center"/>

                                                    <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/> 
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
                                
                                        </dx:ContentControl>
                                    </ContentCollection>
                                </dx:TabPage>
                                <dx:TabPage Text="Programe">
                                    <ContentCollection>
                                        <dx:ContentControl ID="ContentControl2" runat="server">

                                            <dx:ASPxGridView ID="grDate1" runat="server" ClientInstanceName="grDate1" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsPager Mode="ShowAllRecords"/>
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Luni" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                           
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate2" runat="server" ClientInstanceName="grDate2" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Marti" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate2); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate3" runat="server" ClientInstanceName="grDate3" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Miercuri" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate3); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate4" runat="server" ClientInstanceName="grDate4" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Joi" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate5" runat="server" ClientInstanceName="grDate5" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Vineri" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate6" runat="server" ClientInstanceName="grDate6" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Sambata" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate7" runat="server" ClientInstanceName="grDate7" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Duminica" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                            <dx:ASPxGridView ID="grDate8" runat="server" ClientInstanceName="grDate8" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnBatchUpdate="grDateSch_BatchUpdate">
                                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" />
                                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                <SettingsSearchPanel Visible="false" />
                                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                                <ClientSideEvents BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }" ContextMenu="ctx" />
                                                <Columns>
                                                    <dx:GridViewBandColumn Caption="Sarbatori Legale" HeaderStyle-ForeColor="DarkBlue" HeaderStyle-BackColor="Transparent" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Large">
                                                        <Columns>
                                                            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>   
               
                                                            <dx:GridViewDataComboBoxColumn FieldName="IdProgram" Name="IdProgram" Caption="Program" Width="300px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" TextFormatString="{1}">
                                                                    <Columns>
                                                                        <dx:ListBoxColumn FieldName="Id" Width="50" />
                                                                        <dx:ListBoxColumn FieldName="Denumire" Width="300" />
                                                                        <dx:ListBoxColumn FieldName="OraIntrare" Caption="Ora intrare" Width="80" />
                                                                        <dx:ListBoxColumn FieldName="OraIesire" Caption="Ora iesire" Width="80" />
                                                                    </Columns>
                                                                    <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate1); }" />
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora In" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputDeLa" Name="OraInceputDeLa" Caption="Ora In De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraInceputLa" Name="OraInceputLa" Caption="Ora In La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Out" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitDeLa" Name="OraSfarsitDeLa" Caption="Ora Out De La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataTimeEditColumn FieldName="OraSfarsitLa" Name="OraSfarsitLa" Caption="Ora Out La" Width="80px">
                                                                <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" SpinButtons-ShowIncrementButtons="false"/>
                                                            </dx:GridViewDataTimeEditColumn>
                                                            <dx:GridViewDataComboBoxColumn FieldName="ModVerificare" Name="ModVerificare" Caption="Verificare" Width="150px">
                                                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                                                    <Items>
                                                                        <dx:ListEditItem Value="1" Text="Intrare" />
                                                                        <dx:ListEditItem Value="2" Text="Iesire" />
                                                                        <dx:ListEditItem Value="3" Text="Intrare si iesire" />
                                                                        <dx:ListEditItem Value="4" Text="Intrare sau iesire" />
                                                                    </Items>
                                                                </PropertiesComboBox>
                                                            </dx:GridViewDataComboBoxColumn>  

                                                            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="TipSchimb" Name="TipSchimb" Caption="Schimb" Visible="false" ShowInCustomizationForm="false"/> 
                                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false"/>
                                                            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />  
                                                        </Columns>
                                                    </dx:GridViewBandColumn>
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
                                            <br /><br />
                                        </dx:ContentControl>
                                    </ContentCollection>
                                </dx:TabPage>
                            </TabPages>
                        </dx:ASPxPageControl>
                    </td>
                </tr>
            </table>
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxCallbackPanel>

    <script>
        var currentRowIndex = -1;

        function OnGridBatchEditStartEditing(s, e) {
            currentRowIndex = e.visibleIndex;
            var col = e.focusedColumn.fieldName;
            if (col == "OraInceput" || col == "OraSfarsit") {
                e.cancel = true;
                return;
            }
        }

        function OnProgramSelectedIndexChanged(s, grid) {
            var arr = s.GetSelectedItem();
            if (arr != null) {
                var azi = new Date();

                var objIn = arr.texts[2].split(':');
                var oraIn = new Date(azi.getFullYear(), azi.getMonth(), azi.getDate(), objIn[0], objIn[1], 0, 0);
                grid.batchEditApi.SetCellValue(currentRowIndex, 'OraInceput', oraIn);

                var objOut = arr.texts[3].split(':');
                var oraOut = new Date(azi.getFullYear(), azi.getMonth(), azi.getDate(), objOut[0], objOut[1], 0, 0);
                grid.batchEditApi.SetCellValue(currentRowIndex, 'OraSfarsit', oraOut);
            }
        }

        function OnPanelEndCallback() {
            pnlLoading.Hide();
        }

    </script>

</asp:Content>