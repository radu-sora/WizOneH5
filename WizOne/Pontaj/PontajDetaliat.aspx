<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajDetaliat.aspx.cs" Inherits="WizOne.Pontaj.PontajDetaliat" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <table style="width:100%;">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" Visible="false">
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)">
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPtjEchipa" ClientInstanceName="btnPtjEchipa" ClientIDMode="Static" runat="server" Text="Pontajul Echipei" AutoPostBack="false" OnClick="btnPtjEchipa_Click" oncontextMenu="ctx(this,event)">
                    <Image Url="~/Fisiere/Imagini/Icoane/chooser.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnRespins'); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnAproba'); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function (s,e) { popUpInit.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDelete" ClientInstanceName="btnDelete" ClientIDMode="Static" runat="server" Text="Sterge" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnDelete'); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRecalc" ClientInstanceName="btnRecalc" ClientIDMode="Static" runat="server" Text="Recalculeaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { popUpRecalc.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) { grDate.UpdateEdit(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)">
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-top:15px; display:inline-block; width:100%;">
                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
                    <ClientSideEvents 
                        EndCallback="function (s,e) { pnlLoading.Hide(); }" 
                        CallbackError="function (s,e) { pnlLoading.Hide(); }" 
                        BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxRoundPanel ID="pnlFiltrare" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Setare filtru de selectie" Width="100%">
                                <HeaderStyle Font-Bold="true" />
                                <ClientSideEvents CollapsedChanged="function (s,e) { AdjustSize(); }"  />
                                <PanelCollection>
                                    <dx:PanelContent>
                                        <div id="divPeAng" runat="server" class="ptj_pe_zi">
                                            <div class="ptj_filtru">
                                                <label id="lblAnLuna" runat="server">Luna/An</label>
                                                    <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" >
                                                        <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtAnLuna'); }" />
                                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblRolAng" runat="server">Roluri</label>
                                                <dx:ASPxComboBox ID="cmbRolAng" ClientInstanceName="cmbRolAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRolAng'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblAng" runat="server">Angajat</label>
                                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" SelectInputTextOnClick="true" TextFormatString="{0} {1}" >
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                                    </Columns>
                                                    <Buttons>
                                                        <dx:EditButton Position="Left">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                        <dx:EditButton Position="Right">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s, e) { OnCmbAngButtonClick(s,e); }"/>
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblPtjAng" runat="server">Tip inregistrare</label>
                                                <dx:ASPxComboBox ID="cmbPtjAng" ClientInstanceName="cmbPtjAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                                            </div>
                                             <div class="ptj_filtru" style="display:inline-block;">
                                                <dx:ASPxButton ID="btnFiltruAng" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                                    <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnFiltru'); }" />
                                                </dx:ASPxButton>
    	                                        <div class="hovercard" id="divHovercardAng" runat="server">
			                                        <div class="hovercard-container">
				                                        <div class="hovercard-arrow"></div>
				                                        <div class="hovercard-box">									
					                                        <div class="hovercard-body">
						                                        Pentru vizualizare apasati butonul Filtru
					                                        </div>
				                                        </div>
			                                        </div>
		                                        </div>
                                            </div>                                        
                                        </div>

                                        <div id="divPeZi" runat="server" class="ptj_pe_zi">
                                            <div class="ptj_filtru">
                                                <label id="lblZiua" runat="server" class="lw">Data</label>
                                                <dx:ASPxDateEdit ID="txtZiua" runat="server" Width="150px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" AutoPostBack="false">
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                                    <Buttons>
                                                        <dx:EditButton Position="Left">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                        <dx:EditButton Position="Right">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s, e) { OnTxtZiuaButtonClick(s,e); }" ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtZiua'); }"  />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblRolZi" runat="server" class="lw">Roluri</label>
                                                <dx:ASPxComboBox ID="cmbRolZi" ClientInstanceName="cmbRolZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRolZi'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblAngZi" runat="server" class="lw">Angajat</label>
                                                <dx:ASPxComboBox ID="cmbAngZi" ClientInstanceName="cmbAngZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" SelectInputTextOnClick="true"
                                                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                                    </Columns>
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblStare" runat="server" class="lw">Stare</label>
                                                <dx:ASPxComboBox ID="cmbStare" ClientInstanceName="cmbStare" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"  />
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblCtr" runat="server" oncontextMenu="ctx(this,event)" class="lw">Contract</label>
                                                <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"  />
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblSub" runat="server" oncontextMenu="ctx(this,event)" class="lw">Subcomp.</label>
                                                <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblFil" runat="server" oncontextMenu="ctx(this,event)" class="lw">Filiala</label>
                                                <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblSec" runat="server" oncontextMenu="ctx(this,event)" class="lw">Sectie</label>
                                                <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblDept" runat="server" oncontextMenu="ctx(this,event)" class="lw">Dept.</label>
                                                <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblSubDept" runat="server" oncontextMenu="ctx(this,event)" class="lw">Subdept.</label>
                                                <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblBirou" runat="server" oncontextMenu="ctx(this,event)" class="lw">Birou</label>
                                                <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblPtjZi" runat="server" oncontextMenu="ctx(this,event)" class="lw">Tip inregistrare</label>
                                                <dx:ASPxComboBox ID="cmbPtjZi" ClientInstanceName="cmbPtjZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div class="ptj_filtru">
                                                <label id="lblCateg" runat="server" oncontextMenu="ctx(this,event)" class="lw">Categorie</label>
                                                <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
                                            </div>
                                            <div style="float:left; padding:0px 15px; position:relative;">
                                                <dx:ASPxButton ID="btnFiltruZi" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                                    <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnFiltru'); }" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                                    <ClientSideEvents Click="function(s, e) { EmptyFields(); }" />
                                                </dx:ASPxButton>
    	                                        <div class="hovercard" id="divHovercardZi" runat="server">
			                                        <div class="hovercard-container">
				                                        <div class="hovercard-arrow"></div>
				                                        <div class="hovercard-box">									
					                                        <div class="hovercard-body">
						                                        Pentru vizualizare apasati butonul Filtru
					                                        </div>
				                                        </div>
			                                        </div>
		                                        </div>
                                            </div>
                                        </div>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
        </tr>
        <tr>
            <td colspan="2" id="tdGridTotaluri" runat="server" style="margin-top:15px !important;">
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-top:15px;">
                <br />
                <dx:ASPxHiddenField ID="hfRowIndex" runat="server" ClientInstanceName="hfRowIndex" ClientIDMode="Static"></dx:ASPxHiddenField>
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"
                    OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlRowPrepared="grDate_HtmlRowPrepared" 
                    OnBatchUpdate="grDate_BatchUpdate" OnDataBound="grDate_DataBound" OnCellEditorInitialize="grDate_CellEditorInitialize">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" 
                        BatchEditEndEditing="function(s,e) { OnGridBatchEditEndEditing(s,e); }"
                        BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }"
                        Init="function(s,e) { OnGridInit(); }"
                        EndCallback="function(s,e) { OnGridEndCallback(s); }"
                        CustomButtonClick="function(s,e) { grDate_CustomButtonClick(s,e); }"
                         
                        />
                    <Styles>
                        <BatchEditModifiedCell BackColor="Transparent">
                        </BatchEditModifiedCell>
                    </Styles>
                    <Columns>
                        <dx:GridViewCommandColumn VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" Visible="false" FixedStyle="Left" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnGoToCC">
                                    <Image ToolTip="Centrii de Cost" Url="~/Fisiere/Imagini/Icoane/stare.png" />
                                </dx:GridViewCommandColumnCustomButton>                                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Cheia" Caption=" " ReadOnly="true" Visible="true" ShowInCustomizationForm="true" VisibleIndex="2" FixedStyle="Left" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="150px" VisibleIndex="3" Visible="false" ShowInCustomizationForm="false" PropertiesTextEdit-ClientSideEvents-ValueChanged="" FixedStyle="Left" />

                        <dx:GridViewDataTextColumn FieldName="ZiLibera" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiLiberaLegala" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiSapt" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F10022" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F10023" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Afisare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ValActive" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />                   
                    </Columns>                 
                </dx:ASPxGridView>
            </td>
        </tr>
    </table>

    <table id="tblCC" runat="server" class="hidden" style="width:100%;">
        <tr>
            <td class="pull-left">
                <br />
                <dx:ASPxLabel ID="lblZiuaCC" runat="server" ClientIDMode="Static" ClientInstanceName="lblZiuaCC" Font-Bold="true" Text="" />
            </td>
            <td class="pull-right">
                <br />
                <dx:ASPxButton ID="btnSaveCC" ClientInstanceName="btnSaveCC" ClientIDMode="Static" runat="server" Text="Salveaza CC" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        grCC.UpdateEdit();
                        grDate.PerformCallback('btnFiltru');
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <dx:ASPxHiddenField ID="ccValori" runat="server" ClientInstanceName="ccValori" ClientIDMode="Static"></dx:ASPxHiddenField>
                <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"
                    OnCustomCallback="grCC_CustomCallback" OnBatchUpdate="grCC_BatchUpdate" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared" OnCustomColumnDisplayText="grCC_CustomColumnDisplayText">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" VerticalScrollableHeight="130" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <ClientSideEvents 
                        ContextMenu="ctx" 
                        CustomButtonClick="function(s,e) { grCC_CustomButtonClick(s,e); }" 
                        BatchEditStartEditing="function(s,e) { OnGridCCBatchEditStartEditing(s,e); }" 
                        BatchEditEndEditing="function(s,e) { OnGridCCBatchEditEndEditing(s,e); }" 
                        EndCallback="function(s,e) { OnGridEndCallback(s); }" />

                    <Columns>
                        <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC">
                                    <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" ShowInCustomizationForm="false" Width="150px" VisibleIndex="1" Visible="false">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="2" Visible="true">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDownList">
                                <ValidationSettings>
                                    <RequiredField IsRequired="true" ErrorText="Camp obligatoriu" />
                                </ValidationSettings>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="3" Visible="false" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProiectChanged(s); }"></ClientSideEvents>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdSubproiect" Name="IdSubproiect" Caption="SubProiect" Width="250px" VisibleIndex="4" Visible="false">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnSubproiectChanged(s); }" EndCallback="function(s, e) { OnSubEndCallback(); }"/>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="5" Visible="false">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                            <ClientSideEvents EndCallback="function(s, e) { OnActEndCallback(); }" />
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="6" Visible="false">
                            <PropertiesComboBox TextField="Dept" ValueField="IdDept" ValueType="System.Int32" DropDownStyle="DropDown">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Dept" Caption="Dept" Width="130px" />
                                </Columns>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="7" Visible="false" >
                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                            </PropertiesTimeEdit>
                        </dx:GridViewDataTimeEditColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="8" Visible="false" >
                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                            </PropertiesTimeEdit>
                        </dx:GridViewDataTimeEditColumn>

                        <dx:GridViewDataTextColumn FieldName="NrOre1" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre2" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre3" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre4" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre5" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre6" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre7" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre8" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre9" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NrOre10" Width="100px">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<00..23>:<00..59>"  />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                    </Columns>
                    <SettingsCommandButton>
                        <NewButton>
                            <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                        </NewButton>
                        <DeleteButton>
                            <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                        </DeleteButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>
            </td>
        </tr>
    </table>

    <dx:ASPxPopupControl ID="popUpRecalc" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpRecalcArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="450px" Height="200px" HeaderText="Parametrii recalcul"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpRecalc" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td class="pull-right" colspan="4">
                                <dx:ASPxButton ID="btnRecalcParam" runat="server" Text="Recalcul" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { OnRecalcParam(); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td  style="padding:15px;">
                               <dx:ASPxLabel ID="lblDataInc" runat="server" Text="Data Inceput"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                </dx:ASPxDateEdit>
                            </td>
                            <td style="padding:15px;">
                               <dx:ASPxLabel ID="lblDataSf" runat="server" Text="Data Sfarsit"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                </dx:ASPxDateEdit>
                            </td>
                        </tr>
                        <tr>
                            <td>
                               <dx:ASPxLabel ID="lblMarcaIn" runat="server" Text="Marca Inceput"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtMarcaInc" runat="server" Width="100px" />
                            </td>
                            <td>
                               <dx:ASPxLabel ID="lblMarcaSf" runat="server" Text="Marca Sfarsit"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtMarcaSf" runat="server" Width="100px" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>    

    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Parametrii initializare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td class="pull-right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="false" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        e.processOnServer = false;
                                        OnInitPtj(e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:70px;">
                                <dx:ASPxCheckBox ID="chkNormaZL" ClientInstanceName="chkNormaZL" runat="server" Text="cu norma zile lucratoare" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSD" ClientInstanceName="chkNormaSD" runat="server" Text="cu norma sambata si duminica" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSL" ClientInstanceName="chkNormaSL" runat="server" Text="cu norma sarbatori legale" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkCCCu" ClientInstanceName="chkCCCu" runat="server" Text="cu norma pe centrii de cost" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkInOut" ClientInstanceName="chkInOut" runat="server" Text="cu In-Out din contract" TextAlign="Right" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <script>

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnGoToCC":
                    var cheie = s.GetRowKey(s.GetFocusedRowIndex());
                    lblZiuaCC.SetText('Centrii de cost - Ziua ' + cheie);
                    ccValori.Set('cheia', cheie);
                    grCC.PerformCallback('btnCC;' + cheie);
                    break;
            }
        }

        function grCC_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDeleteCC":
                    grCC.PerformCallback('btnDeleteCC;' + s.GetRowKey(e.visibleIndex));
                    break;
            }
        }

        var oldRowIndex = -1;
        var valPro = null;
        var lastPro = null;
        var lastSub = null;
        var rowIndex = -1;

        function OnProiectChanged(cmbPro) {
            if (grCC.GetEditor("IdSubproiect").InCallback()) {
                lastPro = cmbPro.GetValue().toString();
                valPro = cmbPro.GetValue().toString();
            }
            else
                grCC.GetEditor("IdSubproiect").PerformCallback(cmbPro.GetValue().toString());
        }

        function OnSubEndCallback() {
            if (lastPro) {
                grCC.GetEditor("IdSubproiect").PerformCallback(lastPro);
                lastPro = null;
            }
        }

        function OnSubproiectChanged(cmbSub) {
            if (grCC.GetEditor("IdActivitate").InCallback())
                lastSub = cmbSub.GetValue().toString();
            else
                grCC.GetEditor("IdActivitate").PerformCallback(cmbSub.GetValue().toString());
        }

        function OnActEndCallback() {
            if (lastSub) {
                grCC.GetEditor("IdActivitate").PerformCallback(lastSub);
                lastSub = null;
            }
        }

        function OnGridBatchEditStartEditing(s, e) {
            var keyIndex = s.GetColumnByField("Cheia").index;
            var key = e.rowValues[keyIndex].value;

            if (s.batchEditApi.HasChanges() && oldRowIndex != key) {
                s.UpdateEdit();
            }

            oldRowIndex = key;

            var tip = getQueryVariable("tip");
            var dtCurr = new Date(2200, 12, 31);
            if (tip == 1) {
                var luna = txtAnLuna.GetValue();
                dtCurr = new Date(luna.getFullYear(), luna.getMonth(), key);
            }
            else {
                var luna = txtZiua.GetValue();
                dtCurr = new Date(luna.getFullYear(), luna.getMonth(), luna.getDate());
            }

            var time = <%= Session["Ptj_DataBlocare"] %>;
            var dtBlocare = new Date(Number(time.toString().substring(0, 4)), Number(time.toString().substring(4, 6)) - 1, Number(time.toString().substring(6)));

            if (dtBlocare >= dtCurr)
                e.cancel = true;

            if (typeof s.cp_cellsDrepturi[key] != "undefined" && s.cp_cellsDrepturi[key] != null && s.cp_cellsDrepturi[key] == 0) {
                e.cancel = true;
            }

            if (typeof s.cp_PoateModifica[key] != "undefined" && s.cp_PoateModifica[key] != null) {
                switch (s.cp_PoateModifica[key]) {
                    case -33:
                        e.cancel = true;
                        swal({
                            title: "Atentie", text: "Rolul dumneavoastra nu permite stergerea",
                            type: "warning"
                        });
                        break;
                    case 1:
                        //NOP
                        break;
                    case 2:
                        swal({
                            title: "Atentie", text: "Pontare venita din cereri",
                            type: "warning"
                        });
                        break;
                    case 3:
                        e.cancel = true;
                        swal({
                            title: "Atentie", text: "Nu este permisa stergerea pontarilor venite din cereri",
                            type: "warning"
                        });
                        break;
                }
            }

            var col = e.focusedColumn.fieldName;

            if (col.length >= 6 && col.substr(0, 6) == 'ValTmp') {

                if (typeof s.cp_cellsToDisable[key] != "undefined" && s.cp_cellsToDisable[key] != null && s.cp_cellsToDisable[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
                    var ert = '';
                }
                else
                    e.cancel = true;

                if (typeof s.cp_ValSec[key] != "undefined" && s.cp_ValSec[key] != null && s.cp_ValSec[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
                    e.cancel = true;
                }
            }

            if (col.length >= 3 && '<%: lstInOut %>'.indexOf(col + ";") >= 0)
                e.cancel = true; 

            if (col.length >= 6 && col.substr(0, 6) == 'ValAbs') {
                var cmb = grDate.GetEditor('ValAbs');
                if (cmb) {
                    cmb.ClearItems();
                    if (typeof s.cp_ValAbsente[key] != "undefined") {
                        var str = s.cp_ValAbsente[key];
                        if (str) {
                            var arr = str.substring(1).split(",");
                            for (var i = 0; i < arr.length; i++) {
                                if (arr[i] != "undefined" && arr[i] != "") {
                                    var denum = arr[i].split("=");
                                    if (denum.length >= 1) {
                                        if (denum.length == 1)
                                            cmb.AddItem('', denum[0]);
                                        else {
                                            var fullDen = new Array(denum[0], denum[1]);
                                            cmb.AddItem(fullDen);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            DamiPrograme(e);

        }

        function OnGridBatchEditEndEditing(s, e) {
            if (!s.batchEditApi.GetEditCellInfo().column) return;

            var col = s.batchEditApi.GetEditCellInfo().column.fieldName;
            var arr = "In1,In2,In3,In4,In5,In6,In7,In8,In9,In10,In11,In12,In13,In14,In15,In16,In17,In18,In19,In20,Out1,Out2,Out3,Out4,Out5,Out6,Out7,Out8,Out9,Out10,Out11,Out12,Out13,Out14,Out15,Out16,Out17,Out18,Out19,Out20,";

            if (col.length >= 4) {
                switch (col.substr(0, 6)) {
                    case "ValTmp":
                        {
                            var column = s.batchEditApi.GetEditCellInfo().column;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValAbs", null, null, true);

                                var keyIndex = s.GetColumnByField("Cheia").index;
                                var key = e.rowValues[keyIndex].value;
                                var idAfisare = 1;
                                if (typeof s.cp_Afisare[key] != "undefined" && s.cp_Afisare[key] != null) {
                                    idAfisare = s.cp_Afisare[key]
                                }
                                OnEditMode(e, e.visibleIndex, idAfisare);
                            }
                        }
                        break;
                    case "ValAbs":
                        {
                            var column = grDate.GetColumnByField("ValAbs");
                            if (!e.rowValues[column.index]) return;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValStr", newVal, null, true);

                                for (i = 0; i <= 20; i++) {
                                    grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValTmp" + i, null, null, true);
                                }
                            }
                        }
                        break;
                }
            }
        }

        function OnRecalcParam() {
            if (txtDataInc.GetText() == '' || txtDataSf.GetText() == '' || txtMarcaInc.GetText() == '' || txtMarcaSf.GetText() == '') {
                swal({
                    title: "Date insuficiente", text: "Lipsesc date pentru recalcul",
                    type: "warning"
                });
            }
            else {
                popUpRecalc.Hide();

                var jsDateInc = txtDataInc.GetDate();
                var yearInc = jsDateInc.getFullYear();
                var monthInc = jsDateInc.getMonth() + 1;
                var dayInc = jsDateInc.getDate();

                var jsDateSf = txtDataSf.GetDate();
                var yearSf = jsDateSf.getFullYear();
                var monthSf = jsDateSf.getMonth() + 1;
                var daySf = jsDateSf.getDate();

                //Radu 15.01.2020
                var time = <%= Session["Ptj_DataBlocare"] %>;
                var data = txtDataInc.GetValue();
                var dtBlocare = new Date(Number(time.toString().substring(0, 4)), Number(time.toString().substring(4, 6)) - 1, Number(time.toString().substring(6)));
                var dtInc = new Date(data.getFullYear(), data.getMonth(), data.getDate());
                if (dtInc <= dtBlocare) {
                    swal({
                        title: "Atentie", text: "Pontajul este blocat pana la data de " + dtBlocare.getDate() + "/" + (dtBlocare.getMonth() + 1) + "/" + dtBlocare.getFullYear() + "! \n Doriti sa continuati?",
                        type: "info", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Nu", closeOnConfirm: true
                    }, function (isConfirm) {
                        if (isConfirm) {
                            grDate.PerformCallback("btnRecalcParam;" + dayInc + "/" + monthInc + "/" + yearInc + ";" + daySf + "/" + monthSf + "/" + yearSf + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
                        }
                    });
                }
                else
                    grDate.PerformCallback("btnRecalcParam;" + dayInc + "/" + monthInc + "/" + yearInc + ";" + daySf + "/" + monthSf + "/" + yearSf + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
            }
        }

        function eventKeyPress(evt, s) {
            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();
            
            if (!s.IsEditing()) {
                var cell = grDate.GetFocusedCell();
                var col = cell.column.fieldName;
                if (col.length >= 3) {
                    switch (col.substr(0, 3)) {
                        case "In1":
                        case "In2":
                        case "In3":
                        case "In4":
                        case "In5":
                        case "In6":
                        case "In7":
                        case "In8":
                        case "In9":
                        case "Out":
                            {
                                if ('<%: lstInOut %>'.indexOf(col + ";") >= 0) {
                                    //NOP
                                }
                                else {


                                    if (key == 45)              // scade o zi   tasta -
                                    {
                                        var dt = grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName);
                                        if (cell.key == dt.getDate() || cell.key == (dt.getDate() - 1)) {
                                            grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                            var dtCurr = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate() - 1, dt.getHours(), dt.getMinutes(), 0, 0);
                                            grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, dtCurr);
                                            grDate.batchEditApi.EndEdit();
                                            alert('Proces realizat cu succes');
                                        }
                                    }
                                    else if (key == 43)        // adauga o zi  tasta +
                                    {
                                        var dt = grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName);
                                        if (cell.key == dt.getDate() || cell.key == (dt.getDate() + 1)) {
                                            grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                            var dtCurr = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate() + 1, dt.getHours(), dt.getMinutes(), 0, 0);
                                            grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, dtCurr);
                                            grDate.batchEditApi.EndEdit();
                                            alert('Proces realizat cu succes');
                                        }
                                    }
                                    else if (key == 93)         ////insereaza celula   tasta   ]
                                    {
                                        var idx = 21;
                                        var col = cell.column.fieldName;
                                        if (col.substr(0, 2).toLowerCase() == 'in' && col.length <= 4)
                                            idx = Number(col.substr(2));
                                        if (col.substr(0, 3).toLowerCase() == 'out' && col.length <= 5)
                                            idx = Number(col.substr(3));

                                        grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                        for (var i = 20; i > idx; i--) {
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + i));
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "In" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + (i - 1).toString()));
                                        }

                                        if (col.substr(0, 2).toLowerCase() == 'in')
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + i));

                                        grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, null);
                                        grDate.batchEditApi.EndEdit();
                                    }
                                    else if (key == 91)         // sterge celula pe care este, daca este goala tasta [
                                    {
                                        if (grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName) != null)
                                            return;

                                        var idx = 21;
                                        var col = cell.column.fieldName;
                                        if (col.substr(0, 2).toLowerCase() == 'in' && col.length <= 4)
                                            idx = Number(col.substr(2));
                                        if (col.substr(0, 3).toLowerCase() == 'out' && col.length <= 5)
                                            idx = Number(col.substr(3));

                                        grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);

                                        if (col.substr(0, 2).toLowerCase() == 'in')
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "In" + idx, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + idx));

                                        grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + idx, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + (idx + 1).toString()));

                                        for (var i = (idx + 1); i <= 20; i++) {
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "In" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + i));
                                            grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + (i + 1).toString()));
                                        }

                                        grDate.batchEditApi.SetCellValue(inOutIndex, "Out20", null);
                                        grDate.batchEditApi.EndEdit();
                                    }
                                }
                            }
                    }
                }
            }
        }

        function EmptyFields() {
            cmbAng.SetValue(null);
            cmbAngZi.SetValue(null);
            cmbCtr.SetValue(null);
            cmbStare.SetValue(null);

            cmbSub.SetValue(null);
            cmbSec.SetValue(null);
            cmbFil.SetValue(null);
            cmbDept.SetValue(null);
            cmbSubDept.SetValue(null);
            cmbBirou.SetValue(null);
            cmbCateg.SetValue(null);

            pnlCtl.PerformCallback('EmptyFields');
        }

        function grCC_OnNewClick(s, e) {
            grCC.AddNewRow();
        }

        function grCC_OnCancelClick(s, e) {
            grCC.CancelEdit();
        }

        var timeColumnField = "";
        function OnGridCCBatchEditStartEditing(s, e) {

            timeColumnField = e.focusedColumn.fieldName;

            if (timeColumnField.substring(0, 5) == "NrOre") {
                var timeColumn = s.GetColumnByField(timeColumnField);
                if (!e.rowValues.hasOwnProperty(timeColumn.index))
                    return;
                var cellInfo = e.rowValues[timeColumn.index];
                cellInfo.value = minutesToString(cellInfo.value);
            }
        }

        function OnGridCCBatchEditEndEditing(s, e) {
            if (timeColumnField.substring(0, 5) == "NrOre") {
                var timeColumn = s.GetColumnByField(timeColumnField);
                if (!e.rowValues.hasOwnProperty(timeColumn.index))
                    return;
                var cellInfo = e.rowValues[timeColumn.index];
                cellInfo.value = stringToMinutes(s.GetEditValue(timeColumnField));
            }
        }

        function minutesToString(mins) {
            mins = parseInt(mins);
            var hours = Math.floor(mins / 60).toString();
            if (hours.length == 1)
                hours = "0" + hours;
            return hours + ":" + mins % 60;
        }

        function stringToMinutes(s) {
            var hours = s.split(':')[0];
            var mins = s.split(':')[1];
            return parseInt(hours) * 60 + parseInt(mins);
        }

        function OnInitPtj(e) {
            popUpInit.Hide();
            pnlLoading.Show();
            e.processOnServer = true;
        }

        function OnGridInit() {
            window.addEventListener('resize', function () {
                AdjustSize();
            })

            AdjustSize();
        }

        function AdjustSize() {
            var dif = 200 + pnlFiltrare.GetHeight();

            if (typeof grDateTotaluri !== "undefined" && ASPxClientUtils.IsExists(grDateTotaluri))
                dif = dif + grDateTotaluri.GetHeight();

            if (<%=Session["PontajulAreCC"] %> == 1)
                dif = dif + (grCC.GetHeight() + 50);

            var height = Math.max(0, document.documentElement.clientHeight) - dif;
            grDate.SetHeight(height);
        }

        function getQueryVariable(variable) {
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (pair[0] == variable) { return pair[1]; }
            }
            return (false);
        }

        function OnGridEndCallback(s) {
            if (s.cpAlertMessage) {
                swal({
                    title: "",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }

        function cmbContract_SelectedIndexChanged_Client(s, e) {
            var idCtr = s.GetSelectedItem().value;
            LoadPrograme(idCtr);
        }

        function DamiPrograme(e) {
            if (typeof cmbProgram !== "undefined" && ASPxClientUtils.IsExists(cmbProgram)) {
                currentEditableVisibleIndex = e.visibleIndex;
                var idCtr = grDate.batchEditApi.GetCellValue(currentEditableVisibleIndex, "IdContract");
                var idPrg = grDate.batchEditApi.GetCellValue(currentEditableVisibleIndex, "IdProgram");
                LoadPrograme(idCtr);

                if (cmbProgram.FindItemByValue(idPrg))
                    cmbProgram.SetSelectedItem(cmbProgram.FindItemByValue(idPrg));
                else
                    e.cancel = true;
            }
        }

        function LoadPrograme(idCtr) {
            if (typeof cmbProgram !== "undefined" && ASPxClientUtils.IsExists(cmbProgram)) {
                var key = grDate.GetRowKey(grDate.GetFocusedRowIndex());
                let programe = <%=Session["Json_Programe"] %>;
                var arr = programe.filter(function (item) { return item.idContract == idCtr && item.ziSapt == grDate.cp_ZiSapt[key] });

                cmbProgram.ClearItems();

                for (var i = 0; i < arr.length; i++) {
                    cmbProgram.AddItem(arr[i].program, Number(arr[i].idProgram));
                }
            }
        }

        function OnEditMode(e, idx, Value) {

            var valStr = "";

            for (i = 0; i <= 20; i++) {
                var val = 0;
                var valOre = 0;

                var column = grDate.GetColumnByField("ValTmp" + i);
                if (!column) continue;
                if (!e.rowValues[column.index]) continue;
                val = e.rowValues[column.index].value;

                if (val) {
                    var mm = val.getMinutes();
                    var hh = val.getHours();

                    if (mm != 0 || hh != 0) {
                        switch (Value) {
                            case 1:
                                valOre = hh
                                break;
                            case 2:
                                valOre = hh + '.' + ("00" + mm).slice(-2);
                                break;
                            case 3:
                                valOre = hh + '.' + ("00" + Math.round((mm / 60) * 100)).slice(-2);;
                                break;
                            default:
                                valOre = hh
                                break;
                        }

                        var str = column.name;
                        var n = str.indexOf("_");
                        if (n >= 0)
                            valStr += "/" + valOre + column.name.substring(n + 1);
                        else
                            valStr += "/" + valOre + column.name;
                    }
                }
            }

            if (valStr.length > 0) valStr = valStr.substring(1);

            grDate.batchEditApi.SetCellValue(idx, "ValStr", valStr, null, true);
        }

        function OnCmbAngButtonClick(s, e) {
            if (e.buttonIndex == 0) {
                if (s.GetSelectedIndex() > 0)
                    s.SetSelectedIndex(s.GetSelectedIndex() - 1);
            }
            if (e.buttonIndex == 1) {
                if (s.GetSelectedIndex() < s.GetItemCount())
                    s.SetSelectedIndex(s.GetSelectedIndex() + 1);
            }
            grDate.PerformCallback('btnFiltru'); 
        }

        function OnTxtZiuaButtonClick(s, e) {
            var valZiua = new Date(s.GetDate());
            var dtTmp = new Date(valZiua.getFullYear(), valZiua.getMonth(), valZiua.getDate(), 0, 0, 0, 0);
            if (e.buttonIndex == 0) 
                dtTmp.setDate(dtTmp.getDate() - 1);
            if (e.buttonIndex == 1)
                dtTmp.setDate(dtTmp.getDate() + 1);
            s.SetValue(dtTmp);
            grDate.PerformCallback('btnFiltru'); 
        }
    </script>



</asp:Content>
