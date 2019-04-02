<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateGenerale.ascx.cs" Inherits="WizOne.Recrutare.DateGenerale" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>



    <div class="Absente_divOuter">
        <div class="Absente_Cereri_CampuriSup" id="divCanal" runat="server">
            <label id="lblCanal" runat="server" style="display:inline-block;">Canal de recrutare</label>
            <dx:ASPxComboBox ID="cmbCanal" runat="server" ClientInstanceName="cmbCanal" ClientIDMode="Static" Width="150px" Value='<%# Bind("CanalRecrutare") %>' OnDataBinding="cmbCanal_DataBinding"
                ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divNume" runat="server">
            <label id="lblNume" runat="server" style="display:inline-block;">Nume</label>
            <dx:ASPxTextBox ID="txtNume" runat="server" Width="150px" Text='<%# Bind("Nume") %>' />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divPrenume" runat="server">
            <label id="lblPrenume" runat="server" style="display:inline-block;">Prenume</label>
            <dx:ASPxTextBox ID="txtPrenume" runat="server" Width="150px" Text='<%# Bind("Prenume") %>' />
        </div>
    </div>

    <div class="Absente_divOuter">
        <div class="Absente_Cereri_CampuriSup" id="divLoc" runat="server">
            <label id="lblLoc" runat="server" style="display:inline-block;">Localitate</label>
            <dx:ASPxTextBox ID="txtLoc" runat="server" Width="150px" Text='<%# Bind("Localitate") %>' />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divJud" runat="server">
            <label id="lblJud" runat="server" style="display:inline-block;">Judet</label>
            <dx:ASPxTextBox ID="txtJud" runat="server" Width="150px" Text='<%# Bind("Judet") %>' />
        </div>
        <div class="Absente_Cereri_CampuriSup" id="divMail" runat="server">
            <label id="lblMail" runat="server" style="display:inline-block;">Mail</label>
            <dx:ASPxTextBox ID="txtMail" runat="server" Width="150px" Text='<%# Bind("Mail") %>' />
        </div>
    </div>

    <div class="div_ver divObs">
        <label id="lblAdr" runat="server">Adresa completa</label>
        <dx:ASPxMemo ID="txtAdr" runat="server" Width="500px" Height="100px" Text='<%# Bind("AdresaCompleta") %>'></dx:ASPxMemo>
    </div>

    <div class="div_ver divObs">
        <label id="lblObs" runat="server">Observatii</label>
        <dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="100px" Text='<%# Bind("Observatii") %>'></dx:ASPxMemo>
    </div>
    


