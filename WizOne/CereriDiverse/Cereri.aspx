<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Cereri.aspx.cs" Inherits="WizOne.CereriDiverse.Cereri" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
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

        function DelayedCallback(strCallbackName) {
            pnlCtl.PerformCallback(1);
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
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
						<label id="lblTip" runat="server" style="display:inline-block;">Tip cerere</label>
                        <dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
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

                <div class="div_ver divObs">
                    <label id="lblDesc" runat="server">Descriere</label>
                    <dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="100px" meta:resourcekey="txtObsResource1"></dx:ASPxMemo>
                </div>

            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
