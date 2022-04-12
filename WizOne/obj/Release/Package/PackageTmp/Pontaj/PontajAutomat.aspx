<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajAutomat.aspx.cs" Inherits="WizOne.Pontaj.PontajAutomat" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnAct" ClientInstanceName="btnAct" ClientIDMode="Static" runat="server" Text="Actualizeaza absenta" AutoPostBack="false" OnClick="btnAct_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSte" ClientInstanceName="btnSte" ClientIDMode="Static" runat="server" Text="Sterge absenta" AutoPostBack="false" OnClick="btnSte_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
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
                        <label id="lblCtrInc" runat="server" style="display:inline-block;">Contract Inceput</label>
                        <dx:ASPxSpinEdit ID="txtCtrInc" runat="server" Width="100px" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup" id="div1" runat="server">
                        <label id="lblCtrSf" runat="server" style="display:inline-block;">Contract Sfarsit</label>
                        <dx:ASPxSpinEdit ID="txtCtrSf" runat="server" Width="100px" />
                    </div>

                </div>
                <br /><br />
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup" id="div2" runat="server">
                        <label id="lblZiua" runat="server" style="display:inline-block;">Ziua</label>
                        <dx:ASPxDateEdit ID="txtZiua" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup" id="div3" runat="server">
                        <label id="lblAbs" runat="server" style="display:inline-block;">Tip Cerere</label>
                        <dx:ASPxComboBox ID="cmbAbs" runat="server" ClientInstanceName="cmbAbs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>

                </div>

            </td>
        </tr>
    </table>

</asp:Content>
