<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="WizOne.Pagini.MainPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script>
        pnlLoading.Show();
    </script>

    <dx:ASPxDockManager runat="server" ID="dockManager" ClientInstanceName="dockManager" FreezeLayout="true" OnClientLayout="dockManager_ClientLayout">
    </dx:ASPxDockManager>

    <div id="divPanel" class="divPanel faraZIndex" runat="server"></div>

    <dx:ASPxPopupControl ID="popUpPass" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static" Modal="true"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top" OnWindowCallback="popUpPass_WindowCallback"
        EnableViewState="False" PopupElementID="popUpPassArea" PopupHorizontalAlign="WindowCenter" SettingsLoadingPanel-Enabled="true"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="150px" HeaderText="Parola Raport"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpPass" EnableHierarchyRecreation="false">
        <ClientSideEvents EndCallback="function(s, e) { onPassEndCallback(s); }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td class="pull-right">
                                <dx:ASPxButton ID="btnRapPass" ClientInstanceName="btnRapPass" runat="server" Text="Afisare" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { onPassRapButtonClick(); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:20px;">
                                <dx:ASPxLabel ID="lblRap" runat="server" Text="Introduceti parola raport" />
                                <dx:ASPxTextBox ID="txtRapPass" ClientInstanceName="txtRapPass" runat="server" Width="280" Password="true" >
                                    <ClientSideEvents KeyPress="function(s, e) { onPassKeyPress(e); }" />
                                 </dx:ASPxTextBox>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxGlobalEvents ID="glob" runat="server">
        <ClientSideEvents ControlsInitialized="function(s, e) { pnlLoading.Hide(); }" />
    </dx:ASPxGlobalEvents>

    <script>
        var selectedReportUrl;

        function onRapButtonClick(url) {
            selectedReportUrl = url;
            popUpPass.Show();            
        }

        function onPassEndCallback(s) {
            if (s.cpAlertMessage) {
                swal({
                    title: 'Atentie !',
                    text: s.cpAlertMessage,
                    type: 'warning'
                });
                delete s.cpAlertMessage;
            } else {
                window.location.href = selectedReportUrl;
            }
        }

        function onPassKeyPress(e) {
            if (e.htmlEvent.keyCode == 13) {
                ASPxClientUtils.PreventEventAndBubble(e.htmlEvent);
                onPassRapButtonClick();
            }
        }

        function onPassRapButtonClick() {
            if (!txtRapPass.GetText()) {
                swal({
                    title: 'Atentie !',
                    text: 'Lipsesc date',
                    type: 'warning'
                });
            }
            else {
                popUpPass.Hide();
                pnlLoading.Show();                
                popUpPass.PerformCallback();
            }
        }
    </script>
</asp:Content>
