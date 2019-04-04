<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MainPage.aspx.cs" Inherits="WizOne.Pagini.MainPage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


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


</asp:Content>
