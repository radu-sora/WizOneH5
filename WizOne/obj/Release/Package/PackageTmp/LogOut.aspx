<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOut.aspx.cs" Inherits="WizOne.LogOut" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title id="txtVers" runat="server">WizOne</title>
    <link rel="stylesheet" type="text/css" href="fisiere/css/login.css" />
</head>
<body>
    <form id="form1" runat="server">

        <div class="outer">
            <div class="logOut">
                <h3 id="txtTitlu" runat="server">Ati fost delogat cu succes !</h3>
                <a href="Default.aspx" target="_self" id="lnkBack" runat="server">Inpoi in site</a>
            </div>
		</div>
    </form>
</body>
</html>
