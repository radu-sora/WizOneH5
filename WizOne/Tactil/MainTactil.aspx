<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="MainTactil.aspx.cs" Inherits="WizOne.Tactil.MainTactil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link rel="stylesheet" type="text/css" href="../Fisiere/css/tactil.css" />

    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>
        
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
            <td align="center"><label runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</label></td>
            <td align="right">
                <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <table style="width:100%;" >
        <tr>
            <td width="130"></td>
            <td width="275" align="center">
                <label style="font-size:30px;">Luna</label>
                <dx:ASPxSpinEdit ID="spnLuna" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center" ButtonStyle-Width="75"   style="font-size:30px;" />                                    
            </td>
            <td width="120"></td>
            <td width="300" id="tdDataSf" runat="server" align="center">
                <label style="font-size:30px;">Anul</label>
                <dx:ASPxSpinEdit ID="spnAnul" runat="server" Width="200px"  Height="75"  HorizontalAlign="Center" ButtonStyle-Width="75"  style="font-size:30px;" />                                      
            </td>
            <td width="350"></td>
        </tr>
    </table>
    <div class="row text-center align-center">

        <div class="col-sm-4">
            <div class="badgeTactil">
                <asp:LinkButton runat="server" ID="lnkPre" OnClick="lnkPre_Click">
                    <div>
                        <img src ="../Fisiere/Imagini/bdgPtj.jpg" alt = "Fluturas Preview" />
                    </div>
                </asp:LinkButton>
                <h3>Fluturas Preview</h3>
            </div>
        </div>

        <div class="col-sm-4">
            <div class="badgeTactil" id="lblPrint" runat="server">
                <asp:LinkButton runat="server" ID="lnlPri" OnClick="lnlPri_Click">
                    <div>
                        <img src ="../Fisiere/Imagini/bdgCer.jpg" alt = "Fluturas Printare" />
                    </div>
                </asp:LinkButton>
                <h3>Fluturas Printare</h3>
            </div>
        </div>   

    </div>
       
</asp:Content>
