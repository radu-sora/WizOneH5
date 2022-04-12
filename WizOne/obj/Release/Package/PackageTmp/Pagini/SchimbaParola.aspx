<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SchimbaParola.aspx.cs" Inherits="WizOne.Pagini.SchimbaParola" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta charset="utf-8" />
    <meta http-equiv="x-ua-compatible" content="ie=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>WizOne ver 1.0</title>
    <link type="text/css" rel="stylesheet" href="../Fisiere/css/login.css" />
    

    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">

        <div class="outer">
            <div class="ssSchimba">
                <div>
                    <span>Parola veche</span>
                    <asp:TextBox ID="txtPan1" runat="server" TabIndex="1" MaxLength="50" TextMode="Password"></asp:TextBox>
                </div>
                <div>
                    <span>Parola noua</span>
                    <asp:TextBox ID="txtPan2" runat="server" TabIndex="2" MaxLength="50" TextMode="Password"></asp:TextBox>
                </div>
                <div>
                    <span>Confirma parola</span>
                    <asp:TextBox ID="txtPan3" runat="server" TabIndex="3" MaxLength="50" TextMode="Password"></asp:TextBox>
                </div>
            </div>

            <asp:Button ID="btnOk" runat="server" Text="OK" TabIndex="3" ValidationGroup="IntroGrup" onclick="btnOk_Click" />
		</div>


        <script type="text/javascript" language="javascript">
            document.getElementById("txtPan1").focus();   
        </script>


    </form>
</body>
</html>
