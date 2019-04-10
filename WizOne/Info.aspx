<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Info.aspx.cs" Inherits="WizOne.Info" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>



<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>WizOne ver 1.0</title>

    <link rel="stylesheet" type="text/css" href="fisiere/css/tactil.css" />

    <link rel="stylesheet" type="text/css" href="Fisiere/MsgBox/sweetalert.css" />
    <script src="Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" >
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/jquery-3.1.1.min.js" />
                <asp:ScriptReference Path="~/Scripts/bootstrap.min.js" />  
                <asp:ScriptReference Path="~/Fisiere/js/utils.js" />
                <asp:ScriptReference Path="~/Fisiere/MsgBox/sweetalert.min.js" />
            </Scripts>
        </asp:ScriptManager>     
             
        <dx:ASPxLoadingPanel ID="pnlLoading" ClientInstanceName="pnlLoading" runat="server" Modal="true" HorizontalAlign="Center" ImageSpacing="10" VerticalAlign="Middle">
            <Image Url="~/Fisiere/Imagini/loading.gif" Height="40px" Width="40px"></Image>
        </dx:ASPxLoadingPanel>

        <dx:ASPxPanel ID="pnlHeader" runat="server" FixedPosition="WindowTop" Width="100%" BackColor="#FFFFFF" Height="75px" Collapsible="true" SettingsAdaptivity-CollapseAtWindowInnerWidth="700">
            <SettingsAdaptivity CollapseAtWindowInnerWidth="700"></SettingsAdaptivity>
            <PanelCollection>
                <dx:PanelContent runat="server">
                    <a href="#" onclick="location.href='../Pagini/MainPage.aspx'"><img src="~/Fisiere/Imagini/WizOne.png" alt="WizOne" style="float:left; height:45px; margin-left:15px; margin-top:4px;" runat="server" /></a>

                </dx:PanelContent>
            </PanelCollection>
            <BorderBottom BorderColor="#CCCCCC" BorderStyle="Solid" BorderWidth="1px" />
        </dx:ASPxPanel>

        <dx:ASPxPanel ID="pnlContent1" runat="server" Width="100%" Paddings-Padding="10px" >
            <PanelCollection>
                <dx:PanelContent runat="server">

                    <div class="centerBdg">

                        <div id="divPanel" class="divPanel faraZIndex" runat="server">

                            <div class="badgeContainer">
                                <asp:LinkButton runat="server" ID="lnkFlu" OnClick="lnkPrev_Click">
                                    <div>
                                        <img src ="../Fisiere/Imagini/bdgDec.jpg" alt = "Fluturas" />
                                    </div>
                                </asp:LinkButton>
                                <h3>Fluturas</h3> 
                            </div>

                            <div class="badgeContainer">
                                <asp:LinkButton runat="server" ID="lnkPri" OnClick="lnkPrev_Click">
                                    <div>
                                        <img src ="../Fisiere/Imagini/bdgDec.jpg" alt = "Fluturas Printare" />
                                    </div>
                                </asp:LinkButton>
                                <h3>Fluturas Printare</h3>
                            </div>

                            <div class="badgeContainer">
                                <asp:LinkButton runat="server" ID="lnkLog" OnClick="lnkPrev_Click">
                                    <div>    
                                        <img src ="../Fisiere/Imagini/bdgDec.jpg" alt = "Log Out" />
                                    </div>
                                </asp:LinkButton>
                                <h3>Log Out</h3>
                            </div>

                        </div>
		            </div>
                    
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxPanel>

        

    </form>
</body>
</html>
