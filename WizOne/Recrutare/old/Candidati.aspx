<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Candidati.aspx.cs" Inherits="WizOne.Recrutare.Candidati" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Font-Size="14px" Font-Bold="True" ForeColor="#00578A" Font-Underline="True" meta:resourcekey="txtTitluResource1" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                    pnlLoading.Show();
                    e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <asp:FormView ID="pnl" runat="server">
        <ItemTemplate>


    <div class="Absente_divOuter">
        <div class="Absente_Cereri_CampuriSup" id="divCanal" runat="server">
            <label id="lblCanal" runat="server" style="display:inline-block;">Canal de recrutare</label>
            <dx:ASPxComboBox ID="cmbCanal" runat="server" ClientInstanceName="cmbCanal" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divNume" runat="server">
            <label id="lblNume" runat="server" style="display:inline-block;">Nume</label>
            <dx:ASPxTextBox ID="txtNume" runat="server" Width="150px" />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divPrenume" runat="server">
            <label id="lblPrenume" runat="server" style="display:inline-block;">Prenume</label>
            <dx:ASPxTextBox ID="txtPrenume" runat="server" Width="150px" />
        </div>
    </div>

    <div class="Absente_divOuter">
        <div class="Absente_Cereri_CampuriSup" id="divLoc" runat="server">
            <label id="lblLoc" runat="server" style="display:inline-block;">Localitate</label>
            <dx:ASPxTextBox ID="txtLoc" runat="server" Width="150px" />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divJud" runat="server">
            <label id="lblJud" runat="server" style="display:inline-block;">Judet</label>
            <dx:ASPxTextBox ID="txtJud" runat="server" Width="150px" />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divMail" runat="server">
            <label id="lblMail" runat="server" style="display:inline-block;">Mail</label>
            <dx:ASPxTextBox ID="txtMail" runat="server" Width="150px" />
        </div>
    </div>

    <div class="div_ver divObs">
        <label id="lblAdr" runat="server">Adresa completa</label>
        <dx:ASPxMemo ID="txtAdr" runat="server" Width="500px" Height="100px"></dx:ASPxMemo>
    </div>

    <div class="div_ver divObs">
        <label id="lblObs" runat="server">Observatii</label>
        <dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="100px"></dx:ASPxMemo>
    </div>
    

        </ItemTemplate>
    </asp:FormView>


</asp:Content>
