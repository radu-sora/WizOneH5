<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Posturi.aspx.cs" Inherits="WizOne.Organigrama.Posturi" %>

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
                <dx:ASPxButton ID="btnSterge" ClientInstanceName="btnSterge" ClientIDMode="Static" runat="server" Text="Sterge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s,e) { OnStergeClick(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/Salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent>

                <div class="Absente_divOuter margin_top15">
                    <dx:ASPxHiddenField ID="txtIdAuto" runat="server" />
		            <label id="lblId" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Id</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtId" runat="server" Enabled="false" Width="70px" />
                    </div>
		            <label id="lblActiv" runat="server" style="display:inline-block; float:left; padding-right:15px;">Activ</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxCheckBox ID="chkActiv" runat="server" Checked="true" />
                    </div>
		            <label id="lblBuget" runat="server" style="display:inline-block; float:left; padding-right:15px;">Cod buget</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtCodBuget" runat="server" Width="120px" />
                    </div>
                    <div style="float:left; padding-right:15px;">
                        <table>
                            <tr>
                                <td style="padding-right:10px;">
                                    <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                        BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                        ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                        <BrowseButton>
                                            <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                        </BrowseButton>
                                        <ValidationSettings ShowErrors="False"></ValidationSettings>

                                        <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                    </dx:ASPxUploadControl>
                                </td>
                                <td style="padding-right:10px;">
                                    <dx:ASPxButton ID="btnDocSterge" runat="server" ToolTip="sterge document" AutoPostBack="false" Height="28px">
                                        <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                                        <ClientSideEvents Click="function(s,e) { pnlCtl.PerformCallback(5); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td style="padding-right:10px;">
                                    <dx:ASPxButton ID="btnDocArata" runat="server" ToolTip="arata document" AutoPostBack="false" Height="28px">
                                        <Image Url="../Fisiere/Imagini/Icoane/arata.png" Width="16px" Height="16px"></Image>
                                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                                        <ClientSideEvents Click="function(s,e) { ShowDoc(); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
        
		            <label id="lblDen" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Denumire</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtDen" ClientInstanceName="txtDen" runat="server" Width="300px">
                            <ClientSideEvents TextChanged="function(s,e) { OnDenumireChanged(s,e); }" />
                        </dx:ASPxTextBox>
                    </div>        
		            <label id="lblDtInc" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Inceput</label>
                    <div style="float:left; padding-right:10px;">
                        <dx:ASPxDateEdit ID="txtDtInc" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" />
                    </div>
		            <label id="lblDtSf" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Sfarsit</label>
                    <div style="float:left; padding-right:10px;">
                        <dx:ASPxDateEdit ID="txtDtSf" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" />
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblDenRO" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Denumire post romana</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtDenRO" ClientInstanceName="txtDenRO" runat="server" Width="300px" MaxLength="300" />
                    </div>        

		            <label id="lblDenEN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Denumire post engleza</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtDenEN" ClientInstanceName="txtDenEN" runat="server" Width="300px" MaxLength="300" />
                    </div>        
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblGrupRO" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nume grup romana</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtGrupRO" ClientInstanceName="txtGrupRO" runat="server" Width="300px" MaxLength="300" />
                    </div>        

		            <label id="lblGrupEN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nume grup engleza</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtGrupEN" ClientInstanceName="txtGrupEN" runat="server" Width="300px" MaxLength="300" />
                    </div>        
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblCmp" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Companie</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbCmp" ClientInstanceName="cmbCmp" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdCompanie" TextField="Companie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbCmp'); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Subcomp.</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Filiala</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Sectie</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Dept.</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <label id="lblSubdept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Subdept.</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSubdept'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblBirou" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Birou</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdBirou" TextField="Birou" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbBirou'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblSup" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Superior administrativ</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSup" runat="server" Width="300px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true"
                                    IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { NivelIerarhic(s); }" />
                            <ItemStyle Wrap="True" />
                            <Columns>
                                <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="80px" />
                                <dx:ListBoxColumn FieldName="Denumire" Caption="Post" Width="250px" />
                                <dx:ListBoxColumn FieldName="NivelIerarhic" Caption="Nivel" Width="50px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="180px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="180px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="180px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
        
                    <label id="lblSupFunc" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Superior functional</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSupFunc" ClientInstanceName="cmbSupFunc" ClientIDMode="Static" runat="server" Width="300px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" 
                                    IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <ItemStyle Wrap="True" />
                            <Columns>
                                <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="80px" />
                                <dx:ListBoxColumn FieldName="Denumire" Caption="Post" Width="250px" />
                                <dx:ListBoxColumn FieldName="NivelIerarhic" Caption="Nivel" Width="50px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="180px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="180px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="180px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblFunc" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Functie</label>
                    <div style="float:left; padding-right:15px;"> 
                        <dx:ASPxComboBox ID="cmbFunc" runat="server" Width="300px" ValueField="F71802" TextField="F71804" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)"
                        IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0}" />
                    </div>
                        
                    <label id="lblStudiiSup" runat="server" style="display:inline-block; float:left; padding-right:15px; width:120px;">Studii Superioare</label>
                    <div style="float:left; padding-right:15px;"> 
                        <dx:ASPxCheckBox ID="chkStudii" runat="server" />
                    </div>

                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblNivelHay" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nivel Hay</label>
                    <div style="float:left; padding-right:25px;">
                        <dx:ASPxComboBox ID="cmbHay" ClientInstanceName="cmbHay" runat="server" Width="70px" ValueField="Id" TextField="Id" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)"
                                    IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0}"  >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { NivelHay(s); }" />
                            <Columns>
                                <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="80px" />
                                <dx:ListBoxColumn FieldName="SalariuMin" Caption="Salariu Min" Width="150px" />
                                <dx:ListBoxColumn FieldName="SalariuMedian" Caption="Salariu Median" Width="150px" />
                                <dx:ListBoxColumn FieldName="SalariuMax" Caption="Salariu Max" Width="150px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
		            <label id="lblSalMin" runat="server" style="display:inline-block; float:left; padding-right:10px; width:90px;">Salariul min</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxSpinEdit ID="txtSalMin" ClientInstanceName="txtSalMin" runat="server" Width="70px" DecimalPlaces="0" />
                    </div>        
		            <label id="lblSalMed" runat="server" style="display:inline-block; float:left; padding-right:15px;">median</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxSpinEdit ID="txtSalMed" ClientInstanceName="txtSalMed" runat="server" Width="70px" DecimalPlaces="0" />
                    </div>        
		            <label id="lblSalMax" runat="server" style="display:inline-block; float:left; padding-right:15px;">max</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxSpinEdit ID="txtSalMax" ClientInstanceName="txtSalMax" runat="server" Width="70px" DecimalPlaces="0" MaxLength="6" />
                    </div>
		            <label id="lblNivelIer" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nivel ierarhic</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtNivelIer" runat="server" Width="60px" ClientEnabled="false"/>
                        <dx:ASPxHiddenField ID="hfNivelIer" runat="server" />
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblCor" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Cod COR</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbCor" runat="server" Width="400px" ValueField="F72202" TextField="F72204" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" IncrementalFilteringMode="Contains"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F72202" Caption="Cod" Width="100px" />
                                <dx:ListBoxColumn FieldName="F72204" Caption="Denumire" Width="400px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblPozitii" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nr pozitii</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtPozitii" ClientInstanceName="txtPozitii" runat="server" Width="60px" ClientEnabled="false" ReadOnly="true"/>
                    </div>
                	<label id="lblPozitiiAprobate" runat="server" style="display:inline-block; float:left; padding-right:15px;">Nr pozitii aprobate</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtPozitiiAprobate" ClientInstanceName="txtPozitiiAprobate" runat="server" Width="60px" ClientEnabled="false" ReadOnly="true"/>
                    </div>
                    <dx:ASPxButton ID="btnPozitii" runat="server" ToolTip="istoric numar pozitii" AutoPostBack="false" Height="28px" Text="...">
                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                        <ClientSideEvents Click="function(s,e) { popUpIstoric.Show(); }" />
                    </dx:ASPxButton>
                </div>
                <br /><br />

                <dx:ASPxRoundPanel ID="pnlFiltrare" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="true" AllowCollapsingByHeaderClick="true" HeaderText="Alege documentele pentru dosarul personal" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <div class="Absente_divOuter margin_top15">
                                <div style="float:left; padding-right:15px;">  
                                    <dx:ASPxCheckBoxList ID="chkDosar" ClientInstanceName="chkDosar" runat="server" ValueField="Id" TextField="Denumire" RepeatColumns="6" RepeatLayout="Table" />
                                </div>
                            </div>
                            <div id="divDosar" runat="server" style="width:815px;"></div>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>

                <dx:ASPxRoundPanel ID="ASPxRoundPanel2" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="true" AllowCollapsingByHeaderClick="true" HeaderText="Alege echipamentele" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <div class="Absente_divOuter margin_top15">
                                <div style="float:left; padding-right:15px;">  
                                    <dx:ASPxCheckBoxList ID="chkEchip" ClientInstanceName="chkEchip" runat="server" ValueField="IdObiect" TextField="NumeCompus" RepeatColumns="6" RepeatLayout="Table" />
                                </div>
                            </div>
                            <div id="divEchip" runat="server" style="width:815px;"></div>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>

                <dx:ASPxRoundPanel ID="ASPxRoundPanel3" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="true" AllowCollapsingByHeaderClick="true" HeaderText="Alege beneficiile" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <div class="Absente_divOuter margin_top15">
                                <div style="float:left; padding-right:15px;">  
                                    <dx:ASPxCheckBoxList ID="chkBenef" ClientInstanceName="chkBenef" runat="server" ValueField="IdObiect" TextField="NumeCompus" RepeatColumns="6" RepeatLayout="Table" />
                                </div>
                            </div>
                            <div id="divBenef" runat="server" style="width:815px;"></div>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>

                <dx:ASPxRoundPanel ID="ASPxRoundPanel1" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" Collapsed="true" AllowCollapsingByHeaderClick="true" HeaderText="Alege campurile aditionale" CssClass="pnlAlign indentBottom10" Width="100%">
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <div class="Absente_divOuter margin_top15">
                                <div style="float:left; padding-right:15px;">  
                                    <dx:ASPxCheckBoxList ID="chkExtra" ClientInstanceName="chkExtra" runat="server" ValueField="Id" TextField="Eticheta" RepeatColumns="6" RepeatLayout="Table" >
                                        <CaptionSettings Position="Top" />
                                        <ClientSideEvents SelectedIndexChanged="function(s,e) { OnChckSelectedIndexChanged(s,e); }" />
                                    </dx:ASPxCheckBoxList>
                                </div>
                            </div>
                            <div id="divExtra" runat="server" style="width:815px;"></div>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>

                <div class="Absente_divOuter margin_top15" style="display:none;">
		            <label id="lblCom" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Competente</label>
                    <dx:ASPxGridView ID="grDate" runat="server" Width="700px">
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="IdPost" Caption="1" ReadOnly="true" Visible="false" VisibleIndex="1" />
                            <dx:GridViewDataTextColumn FieldName="IdGrupCompetenta" Caption="Grup Competenta" VisibleIndex="2" />
                        </Columns>
                    </dx:ASPxGridView>
                </div>

                <br /><br /><br /><br />

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    <dx:ASPxPopupControl ID="popUpIstoric" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Right" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="800px" Height="500px" HeaderText="Istoric numar pozitii aprobate"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpIstoric" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel6" runat="server">

                    <dx:ASPxGridView ID="grDateIstoric" runat="server" ClientInstanceName="grDateIstoric" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" KeyFieldName="IdAuto" 
                         OnInitNewRow="grDateIstoric_InitNewRow" OnRowUpdating="grDateIstoric_RowUpdating" OnRowDeleting="grDateIstoric_RowDeleting" OnRowInserting="grDateIstoric_RowInserting" >
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowStatusBar="Hidden" />
                        <SettingsEditing Mode="Inline" />
                        <ClientSideEvents EndCallback="function(s,e) { OnEndCallbackGridIstoric(s,e); }" ContextMenu="ctx" />
                        <Columns>
                            <dx:GridViewCommandColumn Width="25px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />                                    
                                                           
                            <dx:GridViewDataSpinEditColumn FieldName="Pozitii" Name="Pozitii" Caption="Pozitii">
                                <PropertiesSpinEdit MaxValue="99" MinValue="1" MaxLength="2">
                                    <ValidationSettings>
                                        <RequiredField IsRequired="true" ErrorText="Acest camp este obligatoriu" />
                                    </ValidationSettings>
                                </PropertiesSpinEdit>
                            </dx:GridViewDataSpinEditColumn>
                            <dx:GridViewDataSpinEditColumn FieldName="PozitiiAprobate" Name="PozitiiAprobate" Caption="Pozitii Aprobate">
                                <PropertiesSpinEdit MaxValue="99" MinValue="1" MaxLength="2">
                                    <ValidationSettings>
                                        <RequiredField IsRequired="true" ErrorText="Acest camp este obligatoriu" />
                                    </ValidationSettings>
                                </PropertiesSpinEdit>
                            </dx:GridViewDataSpinEditColumn>
                            <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput">
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                                    <ValidationSettings>
                                        <RequiredField IsRequired="true" ErrorText="Acest camp este obligatoriu" />
                                    </ValidationSettings>
                                </PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit">
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                                    <ValidationSettings>
                                        <RequiredField IsRequired="true" ErrorText="Acest camp este obligatoriu" />
                                    </ValidationSettings>
                                </PropertiesDateEdit>
                            </dx:GridViewDataDateColumn>
                            
                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" ShowInCustomizationForm="false" /> 
                            <dx:GridViewDataTextColumn FieldName="IdPost" Name="IdPost" Caption="IdPost"  Width="75px" Visible="false" ShowInCustomizationForm="false" /> 
                        </Columns>

                        <SettingsCommandButton>
                            <UpdateButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                <Styles>
                                    <Style Paddings-PaddingRight="5px" />
                                </Styles>
                            </UpdateButton>
                            <CancelButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                            </CancelButton>

                            <EditButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                <Styles>
                                    <Style Paddings-PaddingRight="5px" />
                                </Styles>
                            </EditButton>
                            <DeleteButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                            </DeleteButton>
                            <NewButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                            </NewButton>
                        </SettingsCommandButton>

                    </dx:ASPxGridView>

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

        function NivelHay(s) {
            txtSalMin.SetValue(s.GetSelectedItem().texts[1]);
            txtSalMed.SetValue(s.GetSelectedItem().texts[2]);
            txtSalMax.SetValue(s.GetSelectedItem().texts[3]);
        }

        function NivelIerarhic(s) {
            var nvl = s.GetSelectedItem().texts[2].replace('N-', '');
            if (nvl == 'N') nvl = 0;
            txtNivelIer.SetValue('N-' + (Number(nvl) + 1));
            hfNivelIer.Set('val', 'N-' + (Number(nvl) + 1));

            cmbSupFunc.SetValue(s.GetValue());
            cmbSupFunc.SetText(s.GetText());
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
            OnChckSelectedIndexChanged();
        }

        function ShowDoc() {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tbl=3&id=' + <%=Session["IdAuto"] %>, '_blank ')
        }

        function OnEndCallbackGridIstoric(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }

            if (s.cpPozitii != null) {
                txtPozitii.SetValue(s.cpPozitii);
            }
            if (s.cpPozitiiAprobate != null) {
                txtPozitiiAprobate.SetValue(s.cpPozitiiAprobate);
            }
        }

        function OnStergeClick() {
            swal({
                title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Sigur doriti continuarea procesului de stergere ?'),
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm)
                    pnlCtl.PerformCallback('btnSterge');
            });
        }

        var valDen = txtDen.GetValue();
        function OnDenumireChanged(s, e) {
            alert(valDen);
            alert(txtDenRO.GetValue());
            if (txtDenRO.GetValue() == null || txtDenRO.GetValue() == valDen)
                txtDenRO.SetValue(txtDen.GetValue());
            if (txtDenEN.GetValue() == null || txtDenEN.GetValue() == valDen)
                txtDenEN.SetValue(txtDen.GetValue());
            if (txtGrupRO.GetValue() == null || txtGrupRO.GetValue() == valDen)
                txtGrupRO.SetValue(txtDen.GetValue());
            if (txtGrupEN.GetValue() == null || txtGrupEN.GetValue() == valDen)
                txtGrupEN.SetValue(txtDen.GetValue());
        }

        function CloseGridLookup() {
            cmbCampExtra.ConfirmCurrentSelection();
            cmbCampExtra.HideDropDown();
            cmbCampExtra.Focus();

            var val = cmbCampExtra.GetValue();
            if (val != null) {
                for (var i = 1; i <= 20; i++)
                {
                    var div = document.getElementById("divCampExtra" + val[i]);
                    if (div != null) {
                        if (val.indexOf(val[i]) > 0)
                            div.classList.remove("ascuns");
                        else
                            div.classList.add("ascuns");
                    }
                }
            }
        }

        function OnChckSelectedIndexChanged() {
            var val = chkExtra.GetSelectedValues();
            if (val != null) {
                for (var i = 1; i <= 20; i++) {
                    var div = document.getElementById("divCampExtra" + i);
                    if (div != null) {
                        if (val.indexOf(i.toString()) >= 0)
                            div.classList.remove("ascuns");
                        else
                            div.classList.add("ascuns");
                    }
                }
            }
        }

    </script>

</asp:Content>
