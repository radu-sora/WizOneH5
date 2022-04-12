<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ImprimaTactil.aspx.cs" Inherits="WizOne.Reports.ImprimaTactil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width:100%;">
        <tr style="height:40px; vertical-align:top;">
            <td align="left"><Label runat="server" id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
            <td align="center"><Label runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Printare" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxDocumentViewer ID="rvGeneral" runat="server" Height="1150px" Width="100%" />
            </td>
        </tr>
    </table>

</asp:Content>
