<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Contracte.Detalii" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" ScrollBars="None" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
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
                                        <dx:ASPxTextBox ID="txtId" Width="50" runat="server" ClientEnabled="false" oncontextMenu="ctx(this,event)"/>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumire" runat="server" Text="Denumire" Width="70"/>
                                        <dx:ASPxTextBox ID="txtDenumire" Width="400" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)"/>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxPageControl ID="tabCtr" ClientInstanceName="tabCtr" runat="server" Width="100%" Height="100%" CssClass="dxtcFixed" ActiveTabIndex="0" AutoPostBack="false" OnCallback="tabCtr_Callback">
                                <TabPages>
                                    <dx:TabPage Text="Absente">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl1" runat="server">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOraSchIn" runat="server" Text="Ora Sch. In" Width="75"/>
							                                <dx:ASPxTimeEdit ID="txtOraSchIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOraSchOut" runat="server" Text="Ora Sch. Out" Width="80"/>
							                                <dx:ASPxTimeEdit ID="txtOraSchOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOreSup" runat="server" Text="Ore suplimentare" Width="120"/>
                                                            <dx:ASPxCheckBox ID="chkOreSup" runat="server" AutoPostBack="false"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblAfisare" runat="server" Text="Afisare Ore" Width="130"/>
							                                <dx:ASPxComboBox ID="cmbAfisare" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150" oncontextMenu="ctx(this,event)">
                                                                <Items>
                                                                    <dx:ListEditItem Text="Trunchiere la ore" Value="1" />
                                                                    <dx:ListEditItem Text="Cu minute" Value="2" />
                                                                    <dx:ListEditItem Text="Cu zecimale" Value="3" />
                                                                </Items>
							                                </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-12" style="margin:10px 0px;">
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOraInInit" runat="server" Text="Ora Intrare" Width="75"/>
                                                            <dx:ASPxTimeEdit  ID="txtOraIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOraOut" runat="server" Text="Ora Iesire" Width="80"/>
                                                            <dx:ASPxTimeEdit  ID="txtOraOut" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false" oncontextMenu="ctx(this,event)"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblPontareAuto" runat="server" Text="Initializare automata" Width="120"/>
                                                            <dx:ASPxCheckBox ID="chkPontareAuto" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)"/>
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblRap" runat="server" Text="Raportare ore noapte" Width="130"/>
    							                            <dx:ASPxComboBox ID="cmbRap" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="150" oncontextMenu="ctx(this,event)">
                                                                <Items>
                                                                    <dx:ListEditItem Text="Pe inceput de schimb" Value="1" />
                                                                    <dx:ListEditItem Text="Pe sfarsit de schimb" Value="2" />
                                                                </Items>
                                                            </dx:ASPxComboBox>
                                                        </div>
                                                    </div>
                                                </div>
                                    
                                                <dx:ASPxGridView ID="grDateAbs" runat="server" ClientInstanceName="grDateAbs" ClientIDMode="Static" Width="900px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" VerticalScrollBarMode="Auto" />
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


                                                <div class="row">
                                                    <div class="col-md-12" style="margin:15px 0px;">
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblDuplica" runat="server" Text="Duplica" Width="60"/>
							                                <dx:ASPxComboBox ID="cmbZiDeLa" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false" Width="100">
                                                                <Items>
                                                                    <dx:ListEditItem Text="Luni" Value="1" />
                                                                    <dx:ListEditItem Text="Marti" Value="2" />
                                                                    <dx:ListEditItem Text="Miercuri" Value="3" />
                                                                    <dx:ListEditItem Text="Joi" Value="4" />
                                                                    <dx:ListEditItem Text="Vineri" Value="5" />
                                                                    <dx:ListEditItem Text="Sambata" Value="6" />
                                                                    <dx:ListEditItem Text="Duminica" Value="7" />
                                                                    <dx:ListEditItem Text="Sarbatori legale" Value="8" />
                                                                </Items>
							                                </dx:ASPxComboBox> 
                                                        </div>
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblPentru" runat="server" Text="pentru" Width="50"/>
                                                            <dx:ASPxDropDownEdit ID="cmbZiPentru" ClientInstanceName="cmbZiPentru" Width="250px" runat="server" AnimationType="None">
                                                                <DropDownWindowStyle BackColor="#EDEDED" />
                                                                <DropDownWindowTemplate>
                                                                    <dx:ASPxListBox Width="100%" Height="220px" ID="listBox" ClientInstanceName="checkListBox1" SelectionMode="CheckColumn" runat="server" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                                        <Border BorderStyle="None" />
                                                                        <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                                                        <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged1(s,e); }" />
                                                                        <Items>
                                                                            <dx:ListEditItem Text="(Selectie toate)" />
                                                                            <dx:ListEditItem Text="Luni" Value="1" />
                                                                            <dx:ListEditItem Text="Marti" Value="2" />
                                                                            <dx:ListEditItem Text="Miercuri" Value="3" />
                                                                            <dx:ListEditItem Text="Joi" Value="4" />
                                                                            <dx:ListEditItem Text="Vineri" Value="5" />
                                                                            <dx:ListEditItem Text="Sambata" Value="6" />
                                                                            <dx:ListEditItem Text="Duminica" Value="7" />
                                                                            <dx:ListEditItem Text="Sarbatori legale" Value="8" />
                                                                        </Items>
                                                                    </dx:ASPxListBox>
                                                                    <table style="width: 100%">
                                                                        <tr>
                                                                            <td style="padding: 4px">
                                                                                <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                                                    <ClientSideEvents Click="function(s, e){ cmbZiPentru.HideDropDown(); }" />
                                                                                </dx:ASPxButton>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                </DropDownWindowTemplate>
                                                                <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues1(s); }" DropDown="function(s, e){ SynchronizeListBoxValues1(s); }" />
                                                            </dx:ASPxDropDownEdit>
                                                        </div>
                                                        <dx:ASPxButton ID="btnDuplica" runat="server" Text="Duplica" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                            <Image Url="~/Fisiere/Imagini/Icoane/clone.png"></Image>
                                                            <ClientSideEvents Click="function(s, e) { tabCtr.PerformCallback('btnDuplica'); }" />
                                                        </dx:ASPxButton>
                                                    </div>
                                                </div>

                                                <dx:ASPxGridView ID="grDate1" runat="server" ClientInstanceName="grDate1" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
                                                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" BatchEditSettings-HighlightDeletedRows="false" />
                                                    <SettingsSearchPanel Visible="false" />
                                                    <SettingsPager Mode="ShowAllRecords"></SettingsPager>
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
                                                <dx:ASPxGridView ID="grDate2" runat="server" ClientInstanceName="grDate2" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                <dx:ASPxGridView ID="grDate3" runat="server" ClientInstanceName="grDate3" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                <dx:ASPxGridView ID="grDate4" runat="server" ClientInstanceName="grDate4" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate4); }" />
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
                                                <dx:ASPxGridView ID="grDate5" runat="server" ClientInstanceName="grDate5" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate5); }" />
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
                                                <dx:ASPxGridView ID="grDate6" runat="server" ClientInstanceName="grDate6" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate6); }" />
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
                                                <dx:ASPxGridView ID="grDate7" runat="server" ClientInstanceName="grDate7" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate7); }" />
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
                                                <dx:ASPxGridView ID="grDate8" runat="server" ClientInstanceName="grDate8" ClientIDMode="Static" Width="1000px" AutoGenerateColumns="false" OnBatchUpdate="grDate_BatchUpdate">
                                                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                                    <Settings ShowFilterRow="False" ShowGroupPanel="False" ShowStatusBar="Hidden" />
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
                                                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnProgramSelectedIndexChanged(s,grDate8); }" />
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

        var textSeparator = ",";
        //first one
        function OnListBoxSelectionChanged1(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState1();
            UpdateText1();
        }
        function UpdateSelectAllItemState1() {
            IsAllSelected1() ? checkListBox1.SelectIndices([0]) : checkListBox1.UnselectIndices([0]);
        }
        function IsAllSelected1() {
            var selectedDataItemCount = checkListBox1.GetItemCount() - (checkListBox1.GetItem(0).selected ? 0 : 1);
            return checkListBox1.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText1() {
            var selectedItems = checkListBox1.GetSelectedItems();
            cmbZiPentru.SetText(GetSelectedItemsText1(selectedItems));
        }
        function SynchronizeListBoxValues1(dropDown) {
            checkListBox1.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts1(texts);
            checkListBox1.SelectValues(values);
            UpdateSelectAllItemState1();
            UpdateText1(); // for remove non-existing texts
        }
        function GetSelectedItemsText1(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts1(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox1.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }

    </script>

</asp:Content>