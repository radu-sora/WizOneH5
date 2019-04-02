<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="WizOne.Tactil.Main" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" type="text/css" href="../Fisiere/css/tactil.css" />
    <script src="../Scripts/jquery-3.1.1.min.js"></script>


    <script>
        $(function () {
            $("body").on('click keypress', function () {
                ResetThisSession();
            });
        });

        var timeInSecondsAfterSessionOut = 30;
        var secondTick = 0;

        function ResetThisSession() {
            secondTick = 0;
        }

        function StartThisSessionTimer() {
            secondTick++;
            var timeLeft = ((timeInSecondsAfterSessionOut - secondTick) / 60).toFixed(0); // in minutes
            timeLeft = timeInSecondsAfterSessionOut - secondTick;
            $("#spanTimeLeft").html(timeLeft);

            if (secondTick >= timeInSecondsAfterSessionOut) {
                clearTimeout(tick);
                window.location = "../DefaultTactil.aspx";
                return;
            }
            tick = setTimeout("StartThisSessionTimer()", 1000);
        }

        StartThisSessionTimer();
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
        <table style="width:100%;">
            <tr>
                <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
            </tr>
        </table>

        <table style="width:100%;">
            <tr>
                <td align="left"><Label runat="server"  id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
                <td align="center"><Label  runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
                <td align="right">
                    <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Log Out" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>

        <div class="row text-center align-center">

            <div class="col-sm-4">
                <div class="badgeTactil">
                    <asp:LinkButton runat="server" ID="lnkFlut" OnClick="lnkFlut_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgPtj.jpg" alt = "Fluturas" />
                        </div>
                    </asp:LinkButton>
                    <h3>Fluturas</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil">
                    <asp:LinkButton runat="server" ID="lnkAdev" OnClick="lnkAdev_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgPtj.jpg" alt = "Adeverinte" />
                        </div>
                    </asp:LinkButton>
                    <h3>Adeverinte</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil"  runat="server" ID="divCereriCO">
                    <asp:LinkButton runat="server" ID="lnkCereri" OnClick="lnkCereri_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Cereri concediu odihna" />
                        </div>
                    </asp:LinkButton>
                    <h3>Cereri concediu odihna</h3>
                </div>
            </div>

        </div>

        <div class="row text-center align-center">

            <div class="col-sm-4">
                <div class="badgeTactil"  runat="server" ID="divBiletVoie">
                    <asp:LinkButton runat="server" ID="lnkBiletVoie" OnClick="lnkBiletVoie_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Bilet voie" />
                        </div>
                    </asp:LinkButton>
                    <h3>Bilet voie</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil"  runat="server" ID="divPlanifCO">
                    <asp:LinkButton runat="server" ID="lnkPlanif" OnClick="lnkPlanif_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Planificare CO anual" />
                        </div>
                    </asp:LinkButton>
                    <h3>Planificare CO anual</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil"  runat="server" ID="divIstCereri">
                    <asp:LinkButton runat="server" ID="lnkIst" OnClick="lnkIst_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCrs.jpg" alt = "Istoric cereri" />
                        </div>
                    </asp:LinkButton>
                    <h3>Istoric cereri</h3>
                </div>
            </div>

        </div>


        <div class="row text-center align-center">

            <div class="col-sm-4">
                <div class="badgeTactil">
                    <asp:LinkButton runat="server" ID="LinkButton1" CommandArgument="7" OnClick="lnkRap_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgPtj.jpg" alt = "Adeverinte" />
                        </div>
                    </asp:LinkButton>
                    <h3>Raport 7</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil">
                    <asp:LinkButton runat="server" ID="LinkButton2" CommandArgument="8" OnClick="lnkRap_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Planificare CO anual" />
                        </div>
                    </asp:LinkButton>
                    <h3>Raport 8</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil">
                    <asp:LinkButton runat="server" ID="LinkButton3" CommandArgument="9" OnClick="lnkRap_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCrs.jpg" alt = "Istoric cereri" />
                        </div>
                    </asp:LinkButton>
                    <h3>Raport 9</h3>
                </div>
            </div>

        </div>
        <div class="row text-center align-center">

            <div class="col-sm-4">
                <div class="badgeTactil" runat="server" ID="divPontaj">
                    <asp:LinkButton runat="server" ID="lnkPontaj" OnClick="lnkPontaj_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgPtj.jpg" alt = "Pontaj" />
                        </div>
                    </asp:LinkButton>
                    <h3>Pontaj</h3>
                </div>
            </div>

            <div class="col-sm-4">
                <div class="badgeTactil" runat="server" ID="divCereriDiv">
                    <asp:LinkButton runat="server" ID="lnkCereriDiv" OnClick="lnkCereriDiv_Click">
                        <div>
                            <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Cereri diverse" />
                        </div>
                    </asp:LinkButton>
                    <h3>Cereri diverse</h3>
                </div>
            </div>
        </div>

</asp:Content>
