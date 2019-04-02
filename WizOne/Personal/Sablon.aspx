<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="Sablon.aspx.cs" Inherits="WizOne.Personal.Sablon" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

    <body>
        <form id="form1" runat="server">
            <tr>

                    <td>Alege sablon:</td>
                    <td>
				        <dx:ASPxComboBox    ID="cmbSablon"  runat="server" DropDownStyle="DropDown"  DropDownHeight="150"  DropDownWidth="100"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="true" OnSelectedIndexChanged="cmbSablon_SelectedIndexChanged" >
				        </dx:ASPxComboBox>
                    </td>

            </tr>
            <tr align="right">
                <dx:ASPxButton ID="btnOK" runat="server" Text="Salveaza"  AutoPostBack="false" >
                    <ClientSideEvents Click="function(s, e){
                        window.close();
                        window.opener.location.href = 'DateAngajat.aspx';
                        }" />                                        
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>               
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" >
                    <ClientSideEvents Click="function(s, e){                        
                        window.close();}" />
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </tr>
        </form>
    </body>

