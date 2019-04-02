<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Posturi.aspx.cs" Inherits="WizOne.Organigrama.Posturi" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function StartUpload() {
            //pnlLoading.Show();
        }

        function EndUpload(s) {
            //pnlLoading.Hide();
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function NivelHay(s)
        {
            txtSalMin.SetValue(s.GetSelectedItem().texts[1]);
            txtSalMed.SetValue(s.GetSelectedItem().texts[2]);
            txtSalMax.SetValue(s.GetSelectedItem().texts[3]);
        }

        function NivelIerarhic(s)
        {
            var nvl = s.GetSelectedItem().texts[2].replace('N-', '');
            if (nvl == 'N') nvl=0;
            txtNivelIer.SetValue('N-' + (Number(nvl) + 1));
            hfNivelIer.Set('val','N-' + (Number(nvl) + 1));

            cmbSupFunc.SetValue(s.GetValue());
            cmbSupFunc.SetText(s.GetText());
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
            pnlLoading.Hide();
        }

        function ShowDoc()
        {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tbl=3&id=' + <%=Session["IdAuto"] %>, '_blank ')
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSterge" ClientInstanceName="btnSterge" ClientIDMode="Static" runat="server" Text="Sterge" OnClick="btnSterge_Click" oncontextMenu="ctx(this,event)" >
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

                                        <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
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
                        <dx:ASPxTextBox ID="txtDen" runat="server" Width="300px" />
                    </div>        
		            <label id="lblDtInc" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Inceput</label>
                    <div style="float:left; padding-right:10px;">
                        <dx:ASPxDateEdit ID="txtDtInc" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Enabled="false" />
                    </div>
		            <label id="lblDtSf" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Sfarsit</label>
                    <div style="float:left; padding-right:10px;">
                        <dx:ASPxDateEdit ID="txtDtSf" runat="server" Width="95px" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Enabled="false" />
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblDenRO" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Denumire raport romana</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtDenRO" runat="server" Width="300px" MaxLength="300" />
                    </div>        

		            <label id="lblDenEN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Denumire raport engleza</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtDenEN" runat="server" Width="300px" MaxLength="300" />
                    </div>        
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblGrupRO" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nume grup romana</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtGrupRO" runat="server" Width="300px" MaxLength="300" />
                    </div>        

		            <label id="lblGrupEN" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Nume grup engleza</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtGrupEN" runat="server" Width="300px" MaxLength="300" />
                    </div>        
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Subcomp.</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                        </dx:ASPxComboBox>
                    </div>
        
                    <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:85px;">Filiala</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Sectie</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                        </dx:ASPxComboBox>
                    </div>
        
                    <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:85px;">Dept.</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="300px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblSup" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:100px;">Superior administrativ</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSup" runat="server" Width="300px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" 
                                    IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { NivelIerarhic(s); }" />
                            <Columns>
                                <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="130px" />
                                <dx:ListBoxColumn FieldName="Denumire" Caption="Post" Width="130px" />
                                <dx:ListBoxColumn FieldName="NivelIerarhic" Caption="Nivel" Width="50px" />
                                <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
        
                    <label id="lblSupFunc" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:85px;">Superior functional</label>
                    <div style="float:left; padding-right:15px;">    
                        <dx:ASPxComboBox ID="cmbSupFunc" ClientInstanceName="cmbSupFunc" ClientIDMode="Static" runat="server" Width="300px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" 
                                    IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                            <Columns>
                                <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="130px" />
                                <dx:ListBoxColumn FieldName="Denumire" Caption="Post" Width="130px" />
                                <dx:ListBoxColumn FieldName="NivelIerarhic" Caption="Nivel" Width="50px" />
                                <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>
                </div>

                <div class="Absente_divOuter margin_top15">
                    <label id="lblFunc" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Functie</label>
                    <dx:ASPxComboBox ID="cmbFunc" runat="server" Width="300px" ValueField="F71802" TextField="F71804" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)"
                        IncrementalFilteringMode="Contains" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0}" />
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

		            <label id="lblPLan" runat="server" style="display:inline-block; float:left; padding-right:15px;">Plan HC</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtPlan" runat="server" Width="60px"/>
                    </div>
                	<label id="lblHCAprobat" runat="server" style="display:inline-block; float:left; padding-right:15px;">HC Aprobat</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxTextBox ID="txtHCAProbat" runat="server" Width="60px"/>
                    </div></div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblAtr" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Atribute speciale</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxMemo ID="txtAtr" runat="server" Width="700px" Height="100px" />
                    </div>   
                </div>
    
                <div class="Absente_divOuter margin_top15">
		            <label id="lblCrt" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Criterii evaluare</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxMemo ID="txtCrt" runat="server" Width="700px" Height="100px" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblObs" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Observatii</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxMemo ID="txtObs" runat="server" Width="700px" Height="100px" />
                    </div>   
                </div>

                
                <div class="Absente_divOuter margin_top15" style="display:none;">
		            <label id="lblCom" runat="server" style="display:inline-block; float:left; padding-right:15px; width:100px;">Competente</label>
                    <dx:ASPxGridView ID="grDate" runat="server" Width="700px">
                        <Columns>
                            <dx:GridViewDataTextColumn FieldName="IdPost" Caption="1" ReadOnly="true" Visible="false" VisibleIndex="1" />
                            <dx:GridViewDataTextColumn FieldName="IdGrupCompetenta" Caption="Grup Competenta" VisibleIndex="2" />
                        </Columns>
                    </dx:ASPxGridView>
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf1" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns" >Beneficiul 1</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf1" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf2" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 2</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf2" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf3" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 3</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf3" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf4" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 4</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf4" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf5" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 5</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf5" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf6" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 6</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf6" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf7" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 7</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf7" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf8" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 8</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf8" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf9" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 9</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf9" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

                <div class="Absente_divOuter margin_top15">
		            <label id="lblBenf10" runat="server" style="float:left; padding-right:15px; width:100px;" class="ascuns">Beneficiul 10</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbBenf10" runat="server" Width="300px" ValueField="ObiectId" TextField="ObiectDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" CssClass="ascuns" />
                    </div>   
                </div>

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


</asp:Content>
