<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WizOne.Default" culture="auto" uiculture="auto" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>WizOne</title>

    <link rel="stylesheet" type="text/css" href="fisiere/css/login.css" />

    <link rel="stylesheet" type="text/css" href="Fisiere/MsgBox/sweetalert.css" />
    <script src="Fisiere/MsgBox/sweetalert.min.js"></script>

    <script type="text/javascript" src="https://www.google.com/recaptcha/api.js" async defer></script>
</head>
<body>
    <form id="form1" runat="server">

        <div class="outer">
            <div id="divAd" style="text-align:center;">
                <span id="lblRaspuns" runat="server" visible="false" class="inner" style="text-align:center; margin-top:70px;"></span>
            </div>
            <div id="divRas" runat="server">
                <div class="inner">
                    <span id="lblPan1" runat="server">Utilizator</span>
                    <asp:TextBox ID="txtPan1" runat="server" TabIndex="1" MaxLength="50" 
                        meta:resourcekey="txtPan1Resource1"></asp:TextBox>
                    <div class="divRfv">
                        <asp:RequiredFieldValidator id="rfv1" ControlToValidate="txtPan1" 
                                ValidationGroup="IntroGrup" ErrorMessage="Lipseste utilizatorul" runat="Server" 
                                meta:resourcekey="rfv1Resource1" />
                        <asp:RequiredFieldValidator id="rfv3" 
                                ControlToValidate="txtPan1" ValidationGroup="lnkGrup" 
                                ErrorMessage="Lipseste utilizatorul" runat="Server" 
                                meta:resourcekey="rfv3Resource1" />
                    </div>
                    <span id="lblPan2" runat="server">Parola</span>
                    <asp:TextBox ID="txtPan2" runat="server" TabIndex="2" MaxLength="50" 
                        TextMode="Password" meta:resourcekey="txtPan2Resource1" autocomplete="off"></asp:TextBox>
                    <asp:RequiredFieldValidator id="rfv2" ControlToValidate="txtPan2" 
                        ValidationGroup="IntroGrup" ErrorMessage="Introdu parola" runat="Server" 
                        meta:resourcekey="rfv2Resource1" />
                </div>
                <asp:LinkButton ID="lnkUitat" runat="server"  TabIndex="-1" Width="180px"
                    ValidationGroup="lnkGrup" onclick="lnkUitat_Click"
                    meta:resourcekey="lnkUitatResource1">Am uitat parola</asp:LinkButton>
                <asp:Button ID="btnOk" runat="server" Text="OK" TabIndex="3" 
                    ValidationGroup="IntroGrup" onclick="btnOk_Click" 
                    meta:resourcekey="btnOkResource1" />          


                <div id="divOuter" runat="server" class="captcha">
		        </div>

                <div id ="divText" runat="server" class="innerlogare">
                    <dx:ASPxLabel ID="lblTxt" runat="server" />
                </div>
            </div>
		</div>

        <script type="text/javascript">
            if (document.getElementById("txtPan1"))
                document.getElementById("txtPan1").focus();   
        </script>

    </form>
</body>
</html>
