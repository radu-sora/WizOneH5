<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultTactil.aspx.cs" Inherits="WizOne.DefaultTactil" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>WizOne ver 1.0</title>

    <link rel="stylesheet" type="text/css" href="Fisiere/css/login.css" />
    <link rel="stylesheet" type="text/css" href="Fisiere/css/tactil.css" />

    <link rel="stylesheet" type="text/css" href="Fisiere/MsgBox/sweetalert.css" />
    <script src="Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">

        <div class="outer">
            <div class="inner">
                Pentru accesarea aplicatiei va rugam apropiati cardul de cititor
            </div>
            <input type="text" id="txtPan1" name="txtPan1" runat="server" autofocus="autofocus" autocomplete="off" class="hide" maxlength="15" onserverchange="txtPan1_TextChanged" onblur="this.focus()"  />
		</div>
        

        <script type="text/javascript">
            document.getElementById("txtPan1").focus();   
        </script>

    </form>
</body>
</html>
