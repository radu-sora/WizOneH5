<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajEchipa.aspx.cs" Inherits="WizOne.Pontaj.PontajEchipa" %>


<%@ Register TagPrefix="dx" Namespace="DevExpress.Web" Assembly="DevExpress.Web.v19.1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width:100%;">      
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnIstoricAprobare" ClientInstanceName="btnIstoricAprobare" ClientIDMode="Static" runat="server" Text="Istoric aprobare" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/view.png"></Image>
                    <ClientSideEvents Click="function(s, e) { OnIstoricAprobare(s, e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnValidare" ClientInstanceName="btnValidare" ClientIDMode="Static" runat="server" Text="Validare" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { grDate.PerformCallback('btnValidare'); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRefuza" ClientInstanceName="btnRefuza" ClientIDMode="Static" runat="server" Text="Refuza" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpMotivSapt.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Export" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpExport.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/ExportToXls.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                    <ClientSideEvents Click="function(s, e) { OnRespinge(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents  Click="function(s, e) { grDate.PerformCallback('btnAproba'); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpInit.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnStergePontari" ClientInstanceName="btnStergePontari" ClientIDMode="Static" runat="server" Text="Sterge Pontari" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                    <ClientSideEvents  Click="function(s, e) { grDate.PerformCallback('btnStergePontari'); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnTransfera" ClientInstanceName="btnTransfera" ClientIDMode="Static" runat="server" Text="Transfera" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/duplicare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickTransfera(s,e); }"/>
                </dx:ASPxButton>                
                <dx:ASPxButton ID="btnPeAng" ClientInstanceName="btnPeAng" ClientIDMode="Static" runat="server" Text="Pontaj pe Angajat" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/n2.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickDetaliat(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPeZi" ClientInstanceName="btnPeZi" ClientIDMode="Static" runat="server" Text="Pontaj pe Zi" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickDetaliat(s,e); }" />
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-top:15px; display:inline-block; width:100%;">
                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { pnlLoading.Hide(); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                          <dx:ASPxRoundPanel ID="pnlFiltrare" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="" CssClass="pnlAlign indentright20" Width="100%">
                              <HeaderStyle Font-Bold="true" />
                              <ClientSideEvents CollapsedChanged="function (s,e) { AdjustSize(); }"  />
                            <PanelCollection>
                                <dx:PanelContent>

                            <div class="row">
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divAnLuna" runat="server">
                                    <label id="lblAnLuna" runat="server" oncontextMenu="ctx(this,event)">Luna/An</label><br />
                                    <dx:ASPxDateEdit ID="txtAnLuna" ClientInstanceName="txtAnLuna" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >
                                        <ClientSideEvents ValueChanged="function(s, e) { cmbAng.PerformCallback('txtAnLuna'); }" />
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                    </dx:ASPxDateEdit>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divRol" runat="server">
                                    <label id="lblRol" runat="server" oncontextMenu="ctx(this,event)">Roluri</label><br />
                                    <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { cmbAng.PerformCallback('cmbRol'); }" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divAng" runat="server">
                                    <label id="lblAng" runat="server" oncontextMenu="ctx(this,event)">Angajat</label><br />
                                    <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbAng_Callback" oncontextMenu="ctx(this,event)" SelectInputTextOnClick="true"
                                                 TextFormatString="{0} {1}"  >
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                            <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                        </Columns>
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divStare" runat="server">
                                    <label id="lblStare" runat="server" oncontextMenu="ctx(this,event)">Stare</label><br />
                                    <dx:ASPxComboBox ID="cmbStare" ClientInstanceName="cmbStare" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"/>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divCtr" runat="server">
                                    <label id="lblCtr" runat="server" oncontextMenu="ctx(this,event)">Contract</label><br />

                                    <dx:ASPxDropDownEdit ClientIDMode="AutoID" ClientInstanceName="checkComboBox1" ID="cmbCtr" Width="250px" runat="server" AnimationType="None">
                                        <DropDownWindowStyle BackColor="#EDEDED" />
                                        <DropDownWindowTemplate>
                                            <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox1" SelectionMode="CheckColumn" runat="server" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                <Border BorderStyle="None" />
                                                <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                                <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged1(s,e); }" />
                                            </dx:ASPxListBox>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="padding: 4px">
                                                        <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                            <ClientSideEvents Click="function(s, e){ checkComboBox1.HideDropDown(); }" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </DropDownWindowTemplate>
                                        <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues1(s,e); }" DropDown="function(s, e){ SynchronizeListBoxValues1(s,e); }" />
                                    </dx:ASPxDropDownEdit>

                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSub" runat="server">
                                    <label id="lblSub" runat="server" oncontextMenu="ctx(this,event)">Subcomp.</label><br />
                                    <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCallbackStructura(s); }" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divFil" runat="server">
                                    <label id="lblFil" runat="server" oncontextMenu="ctx(this,event)">Filiala</label><br />
                                    <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" OnCallback="cmbFil_Callback" oncontextMenu="ctx(this,event)" >
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCallbackStructura(s); }" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSec" runat="server">
                                    <label id="lblSec" runat="server" oncontextMenu="ctx(this,event)">Sectie</label><br />
                                    <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" OnCallback="cmbSec_Callback" oncontextMenu="ctx(this,event)" >
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnCallbackStructura(s); }" />
                                    </dx:ASPxComboBox>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divDept" runat="server">
                                    <label id="lblDept" runat="server" oncontextMenu="ctx(this,event)">Dept.</label><br />
                                    <dx:ASPxDropDownEdit ClientIDMode="AutoID" ClientInstanceName="checkComboBox2" ID="cmbDept" Width="250px" runat="server" AnimationType="None">
                                        <DropDownWindowStyle BackColor="#EDEDED" />
                                        <DropDownWindowTemplate>
                                            <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox2" SelectionMode="CheckColumn" runat="server" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" OnCallback="listBox_Callback">
                                                <Border BorderStyle="None" />
                                                <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                                <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged2(s,e); }" />
                                            </dx:ASPxListBox>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td style="padding: 4px">
                                                        <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                            <ClientSideEvents Click="function(s, e){ checkComboBox2.HideDropDown(); }" />
                                                        </dx:ASPxButton>
                                                    </td>
                                                </tr>
                                            </table>
                                        </DropDownWindowTemplate>
                                        <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues2(s,e); }" DropDown="function(s, e){ SynchronizeListBoxValues2(s,e); }" />
                                    </dx:ASPxDropDownEdit>
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSubDept" runat="server">
                                    <label id="lblSubDept" runat="server" oncontextMenu="ctx(this,event)">Subdept.</label><br />
                                    <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" OnCallback="cmbSubDept_Callback" oncontextMenu="ctx(this,event)" />
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divBirou" runat="server">
                                    <label id="lblBirou" runat="server" oncontextMenu="ctx(this,event)">Birou</label><br />
                                    <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="250px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                </div>
                                <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divCateg" runat="server">
                                    <label id="lblCateg" runat="server" oncontextMenu="ctx(this,event)">Categorie</label><br />
                                    <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-9 col-md-8 col-sm-6" style="margin-bottom:8px;"></div>
                                <div class="col-lg-3 col-md-4 col-sm-6" style="margin-bottom:8px;" id="rowHovercard" runat="server">
                                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                        <ClientSideEvents Click="function(s, e) { grDate.PerformCallback('btnFiltru'); }" />
                                    </dx:ASPxButton>
                                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                        <ClientSideEvents Click="function(s,e) { EmptyFields(); }" />
                                    </dx:ASPxButton>
    	                            <div class="hovercard" id="divHovercard" runat="server">
			                            <div class="hovercard-container">
				                            <div class="hovercard-arrow">
				                            </div>
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
            <td colspan="2" style="margin-top:15px;">
                <br />
                <dx:ASPxHiddenField ID="txtCol" runat="server" ClientInstanceName="txtCol" ClientIDMode="Static"></dx:ASPxHiddenField>
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCustomCallback="grDate_CustomCallback" OnDataBound="grDate_DataBound" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" AutoFilterCondition="Contains" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" />
                    <ClientSideEvents ContextMenu="ctx" 
                        RowDblClick="function(s, e) { OnClickDetaliat(s, e); }" 
                        Init="function(s,e) { OnGridInit(); }"
                        EndCallback="function(s, e) { OnEndCallback(s); }" />                    
                    <Columns>
                        
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />
                                                                     
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" FixedStyle="Left" VisibleIndex="1" CellStyle-HorizontalAlign="Center" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="AngajatNume" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="150px" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="Norma" Caption="Norma" ReadOnly="true" FixedStyle="Left" VisibleIndex="4" Width="80px"/>
                        <dx:GridViewDataTextColumn FieldName="DescContract" Caption="Contract" ReadOnly="true" FixedStyle="Left" VisibleIndex="5" Width="150px" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="F06205" Caption="Centrul de Cost" ReadOnly="true" FixedStyle="Left" VisibleIndex="6" Width="150px" Settings-AutoFilterCondition="Contains"/>
                          
                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Visible="false" />

                        <dx:GridViewDataTextColumn FieldName="Functie" Caption="Functie" ReadOnly="true" Visible="false" />

                        <dx:GridViewDataTextColumn FieldName="F100901" Caption="EID" ReadOnly="true" Visible="false" />

                        <dx:GridViewDataTextColumn FieldName="Ziua1" Caption="1" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua2" Caption="2" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua3" Caption="3" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua4" Caption="4" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua5" Caption="5" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua6" Caption="6" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua7" Caption="7" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua8" Caption="8" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua9" Caption="9" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua10" Caption="10" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua11" Caption="11" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua12" Caption="12" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua13" Caption="13" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua14" Caption="14" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="Ziua15" Caption="15" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua16" Caption="16" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua17" Caption="17" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua18" Caption="18" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua19" Caption="19" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua20" Caption="20" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua21" Caption="21" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua22" Caption="22" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua23" Caption="23" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua24" Caption="24" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua25" Caption="25" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua26" Caption="26" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua27" Caption="27" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua28" Caption="28" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua29" Caption="29" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua30" Caption="30" ReadOnly="true"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua31" Caption="31" ReadOnly="true"/>

                        <dx:GridViewDataTextColumn FieldName="Culoare" Caption="Stare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZileGri" Caption="ZileGri" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                    </Columns>
                    
                </dx:ASPxGridView>
            </td>
        </tr>
    </table>

    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Parametri initializare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="true" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        popUpInit.Hide();
                                        pnlLoading.Show();
                                        e.processOnServer = true;
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

    <dx:ASPxPopupControl ID="popUpModif" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="550px" Height="200px" HeaderText="Modificare pontaj"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpModif" EnableHierarchyRecreation="false" OnWindowCallback="popUpModif_WindowCallback">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <div class="row">
                        <div class="col-sm-12">
                            <div style="display:inline-table; float:right;">
                                <dx:ASPxButton ID="btnModif" runat="server" Text="Salveaza" AutoPostBack="false">
                                    <ClientSideEvents Click="function(s, e) { OnModif(s,e); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                </dx:ASPxButton>
                            </div>
                        </div>
                        <div class="col-md-12 margin_top15">
                            <h5><b>Absente de tip zi</b></h5>
                        </div>
                        <div class="col-md-12 margin_top15">
                            <dx:ASPxComboBox ID="cmbTipAbs" runat="server" ClientIDMode="Static" ClientInstanceName="cmbTipAbs" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="DenumireScurta" AutoPostBack="false" TextFormatString="{0}" AllowNull="true" CssClass="aspxComboBox_center">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="DenumireScurta" Caption="Id" Width="50" />
                                    <dx:ListBoxColumn FieldName="Denumire" Caption="Denumire" Width="200" />
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { e.processOnServer = false; EmptyVal(s,e); }" />
                            </dx:ASPxComboBox>
                        </div>
                        <div class="col-md-12 margin_top15">
                            <h5><b>Absente de tip ora</b></h5>
                        </div>
                        <div class="col-md-12" id="pnlValuri" runat="server" style="margin:20px 50px 50px 50px;"/>
                    </div>
                    <dx:ASPxHiddenField ID="txtValuri" runat="server"></dx:ASPxHiddenField>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="popUpExport" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpExportArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Export pontaj"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpExport" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel3" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnExp" runat="server" Text="Export" AutoPostBack="true" OnClick="btnExp_Click" >
                                    <ClientSideEvents Click="function(s, e) { popUpExport.Hide(); e.processOnServer = true; }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/ExportToXls.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:70px;">
                                <dx:ASPxCheckBox ID="chkTotaluri" ClientInstanceName="chkTotaluri" runat="server" Text="totaluri"  Checked="true" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkOre" ClientInstanceName="chkOre" runat="server" Text="ore intrare si iesire" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkPauza" ClientInstanceName="chkPauza" runat="server" Text="pauza" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkLinie" ClientInstanceName="chkLinie" runat="server" Text="afisare pe o singura linie"  Checked="true" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkRoluri" ClientInstanceName="chkRoluri" runat="server" Text="toate rolurile" TextAlign="Right" />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel4" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { OnMotivRespingere(s,e); }" />
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

    <dx:ASPxPopupControl ID="popUpMotivSapt" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivSaptArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv refuz"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotivSapt" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel5" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRefuzaMtv" runat="server" Text="Refuza" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { OnMotivRefuza(s,e); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtvSapt" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtvSapt" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="popUpIstoricAprobare" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="800px" Height="500px" HeaderText="Istoric aprobare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpIstoricAprobare" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel6" runat="server">
                        <table width="100%" >    
                            <tr>
                                <td align="left">
                                    <dx:ASPxGridView ID="grDateIstoric" runat="server" ClientInstanceName="grDateIstoric" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDateIstoric_CustomCallback">
                                        <SettingsBehavior AllowFocusedRow="true" />
                                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />                                   
                                        <SettingsEditing Mode="Inline" />      
                                        <ClientSideEvents ContextMenu="ctx" />                                
                                        <Columns>
                                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="false" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />                                    
                                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" />                                
                                            <dx:GridViewDataTextColumn FieldName="IdSuper" Name="IdSuper" Caption="IdSuper"  Width="75px" Visible="false" />  
                                            <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare"  Width="75px" Visible="false" />       
                                            <dx:GridViewDataTextColumn FieldName="Nume" Name="Nume" Caption="Nume"  Width="200px" />
                                            <dx:GridViewDataTextColumn FieldName="NumeStare" Name="NumeStare" Caption="Stare"  Width="100px"  />
                                            <dx:GridViewDataDateColumn FieldName="DataAprobare" Name="DataAprobare" Caption="Data aprobare" Width="100px" >         
                                                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                            </dx:GridViewDataDateColumn>
                                            <dx:GridViewDataTextColumn FieldName="IdStare" Name="IdStare" Caption="IdStare"  Width="75px" Visible="false" />
                                        </Columns> 
                                    </dx:ASPxGridView>
                                </td>
                            </tr>  
                        </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <script type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function eventKeyPress(evt, s) {
            var cell = grDate.GetFocusedCell();
            if (!cell) return;
            var col = cell.column.fieldName;
            var f10003 = grDate.GetRowKey(cell.rowVisibleIndex);

            txtCol.Set('coloana', col);
            txtCol.Set('f10003', f10003);

            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();

            var cell = grDate.GetFocusedCell();
            var col = cell.column.fieldName;

            if (col.length >= 4 && col.substr(0, 4) == 'Ziua') {
                if (key == 43)              //tasta plus  
                {
                    var time = grDate.cpDataBlocare;
                    var luna = txtAnLuna.GetValue();

                    var dtBlocare = new Date(Number(time.toString().substring(0, 4)), Number(time.toString().substring(4, 6)) - 1, Number(time.toString().substring(6)));
                    var dtCurr = new Date(luna.getFullYear(), luna.getMonth(), col.replace('Ziua', ''));

                    if (dtBlocare < dtCurr) {
                        if (typeof grDate.cp_ZileLucrate[f10003] != "undefined" && grDate.cp_ZileLucrate[f10003] != null && grDate.cp_ZileLucrate[f10003].indexOf(',' + col) >= 0) {
                            pnlLoading.Show();
                            grDate.GetRowValues(grDate.GetFocusedRowIndex(), 'IdStare', OnGetRowValues);
                        }
                        else {
                            swal({
                                title: "Conexiune pierduta !", text: "Listele nu au fost actualizate, va rugam reintrati.",
                                type: "warning"
                            });
                        }
                    }
                }
            }
        }

        function OnGetRowValues(Value) {

            if ((cmbRol.GetValue() == 0 && (Value == 1 || Value == 4)) ||
                (cmbRol.GetValue() == 1 && (Value == 1 || Value == 4)) ||
                (cmbRol.GetValue() == 2 && (Value == 1 || Value == 2 || Value == 4 || Value == 6)) ||
                (cmbRol.GetValue() == 3)) {
                    popUpModif.Show();
                    popUpModif.PerformCallback('popUpModif;');
            }

            pnlLoading.Hide();
        }

        function EmptyFields(s, e) {
            cmbAng.SetValue(null);
            checkComboBox1.SetValue(null);
            cmbStare.SetValue(null);

            cmbSub.SetValue(null);
            cmbSec.SetValue(null);
            cmbFil.SetValue(null);
            checkComboBox2.SetValue(null);
            cmbSubDept.SetValue(null);
            cmbBirou.SetValue(null);
            cmbCateg.SetValue(null);

            checkListBox2.PerformCallback();
        }

        function OnClickDetaliat(s, e) {
            if (s.name == 'btnPeAng') {
                pnlLoading.Show();
                grDate.PerformCallback(s.name + ";" + txtCol.Get('f10003') + ";" + txtCol.Get('coloana') + ";" + grDate.GetPageIndex() + ";" + grDate.GetFocusedRowIndex());
            }
            else {
                if (txtCol.Get('coloana') || txtCol.Get('f10003')) {
                    var colSel = txtCol.Get('coloana');
                    if (colSel.length >= 4 && colSel.substr(0, 4).toLowerCase() == 'ziua') {
                        pnlLoading.Show();
                        grDate.PerformCallback(s.name + ";" + txtCol.Get('f10003') + ";" + colSel + ";" + grDate.GetPageIndex() + ";" + grDate.GetFocusedRowIndex());
                    }
                    else {
                        swal({
                            title: "", text: "Trebuie sa selectati o coloana care afiseaza ziua",
                            type: "warning"
                        });
                        e.processOnServer = false;
                    }
                }
                else {
                    swal({
                        title: "", text: "Nu exista celula selectata",
                        type: "warning"
                    });
                    e.processOnServer = false;
                }
            }
        }

        function OnClickTransfera(s, e) {
            var luna = txtAnLuna.GetValue();
            swal({
                title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Sigur doriti transferul pontajului pentru luna ' + (luna.getMonth() + 1) + '/' + luna.getFullYear() + '?'),
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback("btnTransfera;");
                }
            });
        }

        function OnModif(s, e) {

            var texts = "";
            $('#<% =pnlValuri.ID %> input[type="text"]').each(function () {
                texts += ";" + $(this).attr('id') + "=" + $(this).val();
            });

            txtCol.Set('valuri', texts);
            popUpModif.Hide();
            grDate.PerformCallback('btnModif;');
        }

        function EmptyVal(s, e) {
            $('#<% =pnlValuri.ID %> input[type="text"]').val('');
        }

        function EmptyCmbAbs(s, e) {
            cmbTipAbs.SetValue(null);
        }

        function OnEndCallback(s) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, ""),
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }

            if (s.cp_MesajProces != null) {
                swal({
                    title: "Confirmare proces", text: s.cp_MesajProces,
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: trad_string(limba, "Da, continua!"), cancelButtonText: trad_string(limba, "Renunta"), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("ProcesConfirmare");
                    }
                });
                delete s.cp_MesajProces;
            }
        }
        function OnGridInit() {
            window.addEventListener('resize', function () {
                AdjustSize();
            })

            AdjustSize();
        }
        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight) - 200 - pnlFiltrare.GetHeight();
            grDate.SetHeight(height);
        }

        function OnRespinge(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de respingere ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge; ");
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnMotivRespingere(s, e) {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
                    type: "warning"
                });
            }
            else {
                popUpMotiv.Hide();
                grDate.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
            }
        }

        function OnMotivRefuza(s, e) {
            if (ASPxClientUtils.Trim(txtMtvSapt.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
                    type: "warning"
                });
            }
            else {
                popUpMotivSapt.Hide();
                grDate.PerformCallback('btnRefuza;' + txtMtvSapt.GetText());
                txtMtvSapt.SetText('');
            }
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
            checkComboBox1.SetText(GetSelectedItemsText1(selectedItems));
        }
        function SynchronizeListBoxValues1(dropDown, args) {
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
        //second one
        function OnListBoxSelectionChanged2(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState2();
            UpdateText2();

            if (cmbSec.GetValue() != null)
            {
                cmbSubDept.PerformCallback();
                cmbSubDept.SetValue(null);
            }
        }
        function UpdateSelectAllItemState2() {
            IsAllSelected2() ? checkListBox2.SelectIndices([0]) : checkListBox2.UnselectIndices([0]);
        }
        function IsAllSelected2() {
            var selectedDataItemCount = checkListBox2.GetItemCount() - (checkListBox2.GetItem(0).selected ? 0 : 1);
            return checkListBox2.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText2() {
            var selectedItems = checkListBox2.GetSelectedItems();
            checkComboBox2.SetText(GetSelectedItemsText2(selectedItems));
        }
        function SynchronizeListBoxValues2(dropDown, args) {
            checkListBox2.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts2(texts);
            checkListBox2.SelectValues(values);
            UpdateSelectAllItemState2();
            UpdateText2(); // for remove non-existing texts
        }
        function GetSelectedItemsText2(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts2(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox2.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }

        function OnIstoricAprobare(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                popUpIstoricAprobare.Show();
                grDateIstoric.PerformCallback("btnIstoricAprobare;");
            }
            else {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu ati selectat niciun angajat!"),
                    type: "warning"
                });
            }
        }

        function OnCallbackStructura(s,e){
            switch(s.name)
            {
                case "cmbSub":
                    cmbFil.PerformCallback();

                    cmbFil.SetValue(null);
                    cmbSec.SetValue(null);
                    checkComboBox2.SetValue(null);
                    cmbSubDept.SetValue(null);
                    break;
                case "cmbFil":
                    cmbSec.PerformCallback();

                    cmbSec.SetValue(null);
                    checkComboBox2.SetValue(null);
                    cmbSubDept.SetValue(null);
                    break;
                case "cmbSec":
                    checkListBox2.PerformCallback();

                    checkComboBox2.SetValue(null);
                    cmbSubDept.SetValue(null);
                    break;
            }
        }

    </script>

</asp:Content>

