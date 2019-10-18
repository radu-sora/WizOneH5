<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="WizOne.Pagini.MainPage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        function OnClickRapButton(s) {
            alert(s.name);
            popUpPass.Show();
            hfRap.Set('NumeRap', s.name); 
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxGlobalEvents ID="glob" runat="server">
        <ClientSideEvents ControlsInitialized="function (s, e) { pnlLoading.Hide(); }" />
    </dx:ASPxGlobalEvents>

    <script type="text/javascript">
        pnlLoading.Show();
    </script>


    <dx:ASPxDockManager runat="server" ID="dockManager" ClientInstanceName="dockManager" FreezeLayout="true" OnClientLayout="dockManager_ClientLayout">
    </dx:ASPxDockManager>

    <div id="divPanel" class="divPanel faraZIndex" runat="server"></div>


    <div id="pnlMsgWelcome" runat="server" style="display:none;">
        <div id="myModal" class="modal fade" role="dialog">
            <div class="modal-dialog">

                <!-- Modal content-->
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h4 class="modal-title">Bine Ai Venit !</h4>
                    </div>
                    <div class="modal-body center-block text-center">
                        <img src="../Fisiere/Imagini/Logo_PeliFilip.png" alt="PeliFilip" /><br /><br /><br />
                        <p>Peli Filip SCA va informeaza ca datele dumneavoastra cu caracter personal sunt prelucrate in contextul utilizarii acestei aplicatii. </p>
                        <p>Mai multe informatii despre cum prelucram datele dumneavoastra, precum si despre drepturile de care beneficiati in legatura cu prelucrarea puteti gasi in Nota de informare accesibila in <a href="../Fisiere/DocumentPeliFilip.txt" target="_blank" style="text-decoration:underline; color:#0026ff;">UCM</a></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Inchide</button>
                    </div>
                </div>

            </div>
        </div>
    </div>


    <dx:ASPxPopupControl ID="popUpPass" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpPassArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Parola Raport"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpPass" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRapPass" runat="server" Text="Afisare" AutoPostBack="true" OnClick="btnRapPass_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        popUpPass.Hide();
                                        pnlLoading.Show();
                                        e.processOnServer = true;
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:70px;">
                                <dx:ASPxTextBox ID="txtRapPass" ClientInstanceName="txtRapPass" runat="server" Width="150" />
                                <dx:ASPxHiddenField ID="hfRap" runat="server" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>


</asp:Content>
