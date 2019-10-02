<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Cereri.aspx.cs" Inherits="WizOne.Absente.Cereri" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

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

        function ShowAbsDesc(tip) {
            if (tip == 1) {
                document.getElementById('txtAbsDesc').style.visibility = "visible";
            }
            else {
                document.getElementById('txtAbsDesc').style.visibility = "hidden";
            }
        }

        function DelayedCallback(strCallbackName) {
            pnlCtl.PerformCallback(1);
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
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

        function OnCustomButtonClick(s, e) {
            if (e.buttonIndex == 0) {
                s.PerformCallback("Activi");
            }
            else if (e.buttonIndex == 1) {
                s.PerformCallback("Toti");
            }
        }

        function VerifInterval(s, e) {
            if (cmbOraInc.GetValue() && cmbOraSf.GetValue()) {
                var oraInc = Number(cmbOraInc.GetValue().substring(0, 2)) * 60 + Number(cmbOraInc.GetValue().substring(3, 5));
                var oraSf = Number(cmbOraSf.GetValue().substring(0, 2)) * 60 + Number(cmbOraSf.GetValue().substring(3, 5));
                if (oraInc >= oraSf) {
                    s.SetValue("");
                    swal({
                        title: "", text: "Ora inceput este mai mare decat ora sfarsit",
                        type: "warning"
                    });
                }
                else {
                    var dif = (oraSf - oraInc) / 60;
                    txtNrOre.SetValue(dif.toFixed(4));
                    txtNrOreInMinute.SetValue(oraSf - oraInc);
                }
            }
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" meta:resourcekey="txtTitluResource1" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" meta:resourcekey="btnBackResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnIstoricExtins" ClientInstanceName="btnIstoricExtins" ClientIDMode="Static" runat="server" Text="Istoric Extins" OnClick="btnIstoricExtins_Click" oncontextMenu="ctx(this,event)" meta:resourcekey="btnIstoricExtinsResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/istoric.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" meta:resourcekey="btnExitResource1" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" meta:resourcekey="pnlCtlResource1" >
<SettingsLoadingPanel Enabled="False"></SettingsLoadingPanel>

        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent meta:resourcekey="PanelContentResource1">
                    
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup" id="divRol" runat="server">
                        <label id="lblRol" runat="server" style="display:inline-block;">Roluri</label>
                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="250px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbRolResource1" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(7); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbAng_Callback"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" meta:resourcekey="cmbAngResource1" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource3" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                                <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource6" />
                            </Columns>
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { window.setTimeout(DelayedCallback('COUNTRY'), 1000); }" ButtonClick="OnCustomButtonClick" />
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblTip" runat="server" style="display:inline-block;">Tip Cerere</label>
                        <dx:ASPxComboBox ID="cmbAbs" runat="server" ClientInstanceName="cmbAbs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbAbsResource1" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(2); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label style="display:inline-block;">&nbsp; </label>
                        <div class="Absente_Cereri_AbsDesc" style="display:inline-block; width:100%;">
                            <img src="../Fisiere/Imagini/Icoane/info.png" alt="Info absenta" onMouseOver="ShowAbsDesc(1)" onMouseOut="ShowAbsDesc(0)" />
                            <div id="txtAbsDesc" runat="server">Fara descriere</div>
                        </div>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDataInc" runat="server" style="display:inline-block;">Data Inceput</label>
                        <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(3); }" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDataSf" runat="server" style="display:inline-block;">Data Sfarsit</label>
                        <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(6); }" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        
                        <table>
                            <tr>
                                <td colspan="3">
                                    <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding-right:10px;">
                                    <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                        BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                        ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocUploadResource1">
                                        <BrowseButton>
                                            <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                        </BrowseButton>
                                        <ValidationSettings ShowErrors="False"></ValidationSettings>

                                        <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                    </dx:ASPxUploadControl>
                                </td>
                                <td style="padding-right:10px;">
                                    <dx:ASPxButton ID="btnDocSterge" runat="server" ToolTip="sterge document" AutoPostBack="false" Height="28px" meta:resourcekey="btnDocStergeResource1">
                                        <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                                        <ClientSideEvents Click="function(s,e) { pnlCtl.PerformCallback(5); }" />
                                    </dx:ASPxButton>
                                </td>
                                <td valign="top">
                                    <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Height="28px" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" meta:resourcekey="btnSaveResource1" >
                                        <ClientSideEvents Click="function(s, e) {
                                            pnlLoading.Show();
                                            pnlCtl.PerformCallback(4);
                                        }" />
                                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </div>

                </div>

                <div id="divDateSup" runat="server" class="Absente_divOuter">

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblInl" runat="server" style="display:none;">Inlocuitor</label>
                        <dx:ASPxComboBox ID="cmbInloc" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" visible="false"
                                    CallbackPageSize="15" EnableCallbackMode="True" EnableViewState="false" TextFormatString="{0} {1}" AllowNull="true" meta:resourcekey="cmbInlocResource1" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource7" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource8" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource9" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource10" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource11" />
                                <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource12" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblNrZile" runat="server" style="display:inline-block;">Nr. zile</label>
                        <dx:ASPxTextBox ID="txtNrZile" runat="server" Width="70px" ReadOnly="true" meta:resourcekey="txtNrZileResource1" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblNrOre" runat="server" style="display:none;">Nr. ore</label>
                        <dx:ASPxSpinEdit ID="txtNrOre" ClientInstanceName="txtNrOre" runat="server" Width="70px" Visible="false" MinValue="0" MaxValue="8">
                            <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                        </dx:ASPxSpinEdit>
                        <dx:ASPxTextBox ID="txtNrOreInMinute" ClientInstanceName="txtNrOreInMinute" runat="server" Width="70px" Visible="false" ClientEnabled="false" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblOraInc" runat="server" style="display:none;">Ora Inceput</label>
                        <dx:ASPxComboBox ID="cmbOraInc" ClientInstanceName="cmbOraInc" runat="server" Width="100px" Visible="false" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s,e); }" />
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblOraSf" runat="server" style="display:none;">Ora Sfarsit</label>
                        <dx:ASPxComboBox ID="cmbOraSf" ClientInstanceName="cmbOraSf" runat="server" Width="100px" Visible="false" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s,e);  }" />
                        </dx:ASPxComboBox>
                    </div>

                    <dx:ASPxTextBox ID="txtNrZileViitor" runat="server" Width="70px" ReadOnly="true" Visible="false" meta:resourcekey="txtNrZileViitorResource1" />

                    <div id="divDateExtra" runat="server" />
                </div>

                <div class="div_ver divObs">
                    <label id="lblObs" runat="server">Observatii</label>
                    <dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="100px" meta:resourcekey="txtObsResource1"></dx:ASPxMemo>
                </div>

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
