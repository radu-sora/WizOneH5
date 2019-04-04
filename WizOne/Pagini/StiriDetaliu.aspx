<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="StiriDetaliu.aspx.cs" Inherits="WizOne.Pagini.StiriDetaliu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <div class="div_hor">
        <label style="padding-left:0;">Id</label>
        <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false"></dx:ASPxTextBox>
        <label>Data Inceput</label>
        <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date">
            <CalendarProperties FirstDayOfWeek="Monday" />
        </dx:ASPxDateEdit>
        <label>Data Sfarsit</label>
        <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date">
            <CalendarProperties FirstDayOfWeek="Monday" />
        </dx:ASPxDateEdit>
        <label>Activ</label>
        <dx:ASPxCheckBox ID="chkActiv" runat="server" Checked="true" AllowGrayed="false" EnableViewState="false"></dx:ASPxCheckBox>
    </div>
    <div class="div_hor">
        <label style="padding-left:0;">Denumire</label>
        <dx:ASPxTextBox ID="txtDenumire" runat="server" Width="300px"></dx:ASPxTextBox>
        <label>Limba</label>
        <dx:ASPxComboBox ID="cmbLimbi" runat="server" ClientInstanceName="cmbLimbi" AutoPostBack="false" Width="120px" ValueField="Id" ValueType="System.String" TextField="Denumire" AllowNull="false" />
    </div>
    <br />
    <br />

    <dx:ASPxHtmlEditor ID="txtContinut" runat="server" Height="370px">
        <SettingsDialogs>
            <InsertImageDialog>
                <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                    <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                    </ValidationSettings>
                </SettingsImageUpload>
            </InsertImageDialog>
        </SettingsDialogs>
    </dx:ASPxHtmlEditor>
    



</asp:Content>
