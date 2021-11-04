<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Cereri.aspx.cs" Inherits="WizOne.Absente.Cereri" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>

        var limba = "<%= Session["IdLimba"] %>";

        function StartUpload() {
            //pnlLoading.Show();
        }

        function EndUpload(s) {
            //pnlLoading.Hide();
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function ShowAbsDesc(tip) {
            if (tip == 1) {
                document.getElementById('txtAbsDesc').style.visibility = "visible";
            }
            else {
                document.getElementById('txtAbsDesc').style.visibility = "hidden";
            }
        }
     
        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                if (s.cpSuccessMessage != null)
                    swal({
                        title: trad_string(limba, ""), text: s.cpAlertMessage,
                        type: "success"
                    });
                else
                    swal({
                        title: trad_string(limba, ""), text: s.cpAlertMessage,
                        type: "warning"
                    });
                s.cpAlertMessage = null;
                s.cpSuccessMessage = null;
            }
            if (s.cp_InfoMessage != null) {
                swal({
                    title: "Avertisment", text: s.cp_InfoMessage,
                    type: "info", showCancelButton: false, confirmButtonColor: "#DD6B55", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        window.history.back();
                    }
                });
                s.cp_InfoMessage = null;
            }
        }        

        function VerifInterval(s, e) {
            if (cmbOraInc.GetValue() && cmbOraSf.GetValue()) {
                var oraInc = Number(cmbOraInc.GetValue().substring(0, 2)) * 60 + Number(cmbOraInc.GetValue().substring(3, 5));
                var oraSf = Number(cmbOraSf.GetValue().substring(0, 2)) * 60 + Number(cmbOraSf.GetValue().substring(3, 5));

                if (oraInc == oraSf) {
                    s.SetValue("");
                    swal({
                        title: "", text: "Ora inceput este egala cu ora sfarsit",
                        type: "warning"
                    });
                }

                var dif = 0;
                if (oraInc < oraSf)
                    diff = oraSf - oraInc;
                else
                    diff = ((24 * 60) - oraInc) + oraSf;

                var rez = diff/60;
                txtNrOre.SetValue(rez.toFixed(4));
                var dt = new Date(2200, 1, 1, diff/60, diff%60);
                txtNrOreTime.SetDate(dt);
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="page-content">
        <div class="page-content-header">
            <div>
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" meta:resourcekey="txtTitluResource1" />
            </div>
            <div>
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" meta:resourcekey="btnBackResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnIstoricExtins" ClientInstanceName="btnIstoricExtins" ClientIDMode="Static" runat="server" Text="Istoric Extins" OnClick="btnIstoricExtins_Click" CssClass="hidden-xs hidden-sm" oncontextMenu="ctx(this,event)" meta:resourcekey="btnIstoricExtinsResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/istoric.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" PostBackUrl="../Pagini/MainPage.aspx" CssClass="hidden-xs hidden-sm" oncontextMenu="ctx(this,event)" meta:resourcekey="btnExitResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </div>
        </div>
        <div class="max-width-lg">
            <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" meta:resourcekey="pnlCtlResource1" >
                <SettingsLoadingPanel Enabled="False" />
                <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                <PanelCollection>
                    <dx:PanelContent meta:resourcekey="PanelContentResource1">
                    
                        <div class="row row-fix">
                            <dx:ASPxPanel ID="divRol" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent>                                
                                        <dx:ASPxLabel ID="lblRol" runat="server" AssociatedControlID="cmbRol" Text="Roluri" Font-Bold="true" />
                                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="100%" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbRolResource1" >
                                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(7); }" />
                                        </dx:ASPxComboBox>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <dx:ASPxLabel ID="lblAng" runat="server" AssociatedControlID="cmbAng" Text="Angajat" Font-Bold="true" />
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="100%" 
                                    CssClass="dx-combobox-adaptive dx-combobox-adaptive-hide-column1 dx-combobox-adaptive-hide-column3 dx-combobox-adaptive-hide-column4"
                                    ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbAng_Callback"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" meta:resourcekey="cmbAngResource1" >
                                    <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource3" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource6" />
                                    </Columns>                                    
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(1); }" ButtonClick="function(s, e) { e.buttonIndex ? s.PerformCallback('Toti') : s.PerformCallback('Activi'); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <div class="Absente_Cereri_AbsDesc">
                                    <img src="../Fisiere/Imagini/Icoane/info.png" alt="Info absenta" onMouseOver="ShowAbsDesc(1)" onMouseOut="ShowAbsDesc(0)" />                                    
                                </div>
                                <div id="txtAbsDesc" runat="server">Fara descriere</div>
                                <dx:ASPxLabel ID="lblTip" runat="server" AssociatedControlID="cmbAbs" Text="Tip Cerere" Font-Bold="true" />                                
                                <dx:ASPxComboBox ID="cmbAbs" runat="server" ClientInstanceName="cmbAbs" ClientIDMode="Static" Width="100%" ValueField="Id" DropDownWidth="200" 
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbAbsResource1" >
                                    <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { typeof txtNrOre !== 'undefined' && txtNrOre.SetValue(); typeof txtNrOreTime !== 'undefined' && txtNrOreTime.SetValue(); pnlCtl.PerformCallback(2); }" />
                                </dx:ASPxComboBox>
                            </div>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">                                
                                <dx:ASPxLabel ID="lblDataInc" runat="server" AssociatedControlID="txtDataInc" Text="Data Inceput" Font-Bold="true" />
                                <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100%" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" PickerDisplayMode="Auto" meta:resourcekey="txtDataIncResource1" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                    <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(3); }" />
                                </dx:ASPxDateEdit>
                            </div>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <dx:ASPxLabel ID="lblDataSf" runat="server" AssociatedControlID="txtDataSf" Text="Data Sfarsit" Font-Bold="true" />
                                <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100%" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" PickerDisplayMode="Auto" meta:resourcekey="txtDataSfResource1" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                    <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(6); }" />
                                </dx:ASPxDateEdit>
                            </div>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6 file-uploader">
                                <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ClientInstanceName="UploadImage" ShowProgressPanel="true"
                                    FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                    OnFileUploadComplete="btnDocUpload_FileUploadComplete" meta:resourcekey="btnDocUploadResource1">
                                    <BrowseButton Text="">
                                        <Image Url="../Fisiere/Imagini/Icoane/incarca.png" />                                                                                
                                    </BrowseButton>                                    
                                    <ValidationSettings ShowErrors="False" />
                                    <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s, e) { EndUpload(s); }" />                                                
                                </dx:ASPxUploadControl>
                                <dx:ASPxButton ID="btnDocSterge" runat="server" ToolTip="sterge document" AutoPostBack="false" meta:resourcekey="btnDocStergeResource1">
                                    <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px" />
                                    <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(5); }" />
                                </dx:ASPxButton>
                                <label id="lblDoc" clientidmode="Static" runat="server"></label>
                            </div>
                        </div>                        
                        <div class="row row-fix">
                            <dx:ASPxPanel ID="divInloc" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent> 
                                        <dx:ASPxLabel ID="lblInl" runat="server" AssociatedControlID="cmbInloc" Text="Inlocuitor" Font-Bold="true" />
                                        <dx:ASPxComboBox ID="cmbInloc" runat="server" Width="100%"
                                            CssClass="dx-combobox-adaptive dx-combobox-adaptive-hide-column1 dx-combobox-adaptive-hide-column3 dx-combobox-adaptive-hide-column4"
                                            ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                            CallbackPageSize="15" EnableCallbackMode="True" TextFormatString="{0} {1}" AllowNull="true" meta:resourcekey="cmbInlocResource1" >
                                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource7" />
                                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource8" />
                                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource9" />
                                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource10" />
                                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource11" />
                                                <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource12" />
                                            </Columns>
                                        </dx:ASPxComboBox>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                            <div class="col-lg-2 col-md-4 col-sm-4 col-xs-6">                                
                                <dx:ASPxLabel ID="lblNrZile" runat="server" AssociatedControlID="txtNrZile" Text="Nr. zile" Font-Bold="true" />
                                <dx:ASPxTextBox ID="txtNrZile" runat="server" Width="100%" ReadOnly="true" meta:resourcekey="txtNrZileResource1" />
                            </div>
                            <dx:ASPxPanel ID="divNrOre" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent> 
                                        <dx:ASPxLabel ID="lblNrOre" runat="server" AssociatedControlID="txtNrOre" Text="Nr. ore" Font-Bold="true" />
                                        <dx:ASPxSpinEdit ID="txtNrOre" ClientInstanceName="txtNrOre" runat="server" Width="100%" ClientVisible="false" MinValue="0" MaxValue="999">
                                            <SpinButtons ShowIncrementButtons="false" />
                                        </dx:ASPxSpinEdit>
                                        <dx:ASPxTimeEdit ID="txtNrOreTime" ClientInstanceName="txtNrOreTime" runat="server" Width="100%" ClientVisible="false" ClientEnabled="false">
                                            <SpinButtons ShowIncrementButtons="false" />                                    
                                        </dx:ASPxTimeEdit>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                            <dx:ASPxPanel ID="divOraInc" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent> 
                                        <dx:ASPxLabel ID="lblOraInc" runat="server" AssociatedControlID="cmbOraInc" Text="Ora Inceput" Font-Bold="true" />
                                        <dx:ASPxComboBox ID="cmbOraInc" ClientInstanceName="cmbOraInc" runat="server" Width="100%" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s, e); pnlCtl.PerformCallback(8); }" />
                                        </dx:ASPxComboBox>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                            <dx:ASPxPanel ID="divOraSf" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent> 
                                        <dx:ASPxLabel ID="lblOraSf" runat="server" AssociatedControlID="cmbOraSf" Text="Ora Sfarsit" Font-Bold="true" />
                                        <dx:ASPxComboBox ID="cmbOraSf" ClientInstanceName="cmbOraSf" runat="server" Width="100%" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                                            <SettingsAdaptivity Mode="OnWindowInnerWidth" SwitchToModalAtWindowInnerWidth="1024" />
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s, e); pnlCtl.PerformCallback(8); }" />
                                        </dx:ASPxComboBox>
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                            <dx:ASPxPanel ID="divNrZileViitor" runat="server" CssClass="col-lg-2 col-md-4 col-sm-4 col-xs-6">
                                <PanelCollection>
                                    <dx:PanelContent> 
                                        <dx:ASPxTextBox ID="txtNrZileViitor" runat="server" Width="100%" ReadOnly="true" />
                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxPanel>
                        </div>

                        <div id="divDateExtra" runat="server" />

                        <div class="row row-fix">
                            <div class="col-lg-4 col-md-8 col-sm-8 col-xs-12">
                                <dx:ASPxLabel ID="lblObs" runat="server" AssociatedControlID="txtObs" Text="Observatii" Font-Bold="true" />
                                <dx:ASPxMemo ID="txtObs" runat="server" Width="100%" Height="100px" meta:resourcekey="txtObsResource1" />
                            </div>                            
                        </div>
                        <div class="row row-fix">
                            <div class="col-xs-12">
                                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" meta:resourcekey="btnSaveResource1" >
                                    <ClientSideEvents Click="function(s, e) {
                                        pnlLoading.Show();
                                        pnlCtl.PerformCallback(4);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                        
                    </dx:PanelContent>
                </PanelCollection>
            </dx:ASPxCallbackPanel>
        </div>        
    </div>
</asp:Content>
