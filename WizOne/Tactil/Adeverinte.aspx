<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Adeverinte.aspx.cs" Inherits="WizOne.Tactil.Adeverinte" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" type="text/css" href="../Fisiere/css/tactil.css" />
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>    
        function AspLoading()
        {
            pnlLoading.Show();
        }
    </script>
    
    <table style="width:100%;">
        <tr>
            <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
        </tr>
    </table>

    <table style="width:100%;">
        <tr>
            <td align="left">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgBack.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left"><Label runat="server"  id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
            <td align="center"><Label  runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <div class="row text-center align-center" id="pnlGen" runat="server">
    </div>

</asp:Content>
