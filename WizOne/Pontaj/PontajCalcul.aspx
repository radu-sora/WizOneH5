<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajCalcul.aspx.cs" Inherits="WizOne.Pontaj.PontajCalcul" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnCalc" ClientInstanceName="btnCalc" ClientIDMode="Static" runat="server" Text="Calcul formule" AutoPostBack="false" OnClick="btnCalc_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                    <ClientSideEvents Click="function (s,e) { 
                        pnlLoading.Show();
                        e.processOnServer = true;
                     }" />
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />
				
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup" id="divRol" runat="server">
						<label id="lblAnLuna" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Luna/An</label>
						 <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" >
                             <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>

                </div>
            </td>
        </tr>
    </table>

</asp:Content>
