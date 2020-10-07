<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Diagrama.aspx.cs" Inherits="WizOne.Organigrama.Diagrama" %>

<%@ Register Assembly="DevExpress.Web.ASPxDiagram.v20.1, Version=20.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxDiagram" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width:100%;">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                    <label id="lblDtVig" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Data selectie</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit id="txtDtVig" ClientIDMode="Static" ClientInstanceName="txtDtVig" runat="server" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Width="100px" AllowNull="false" />
                    </div>

                    <label id="lblParinte" runat="server" style="display:inline-block; float:left; padding-right:15px;">Superior</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbParinte" runat="server" Width="130px" ValueField="Camp" TextField="Denumire" />
                    </div>

                    <label id="Label1" runat="server" style="display:inline-block; float:left; padding-right:15px;">De la postul</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbPost" runat="server" Width="130px" ValueField="Id" TextField="Denumire" />
                    </div>

                    <label id="lblLimbi" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Alege limba</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbLimbi" runat="server" AutoPostBack ="false">
                            <Items>
                                <dx:ListEditItem Value="RO" Text="Romana" Selected="true" />
                                <dx:ListEditItem Value="EN" Text="Engleza" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <label id="lblNivel" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Alege nr niveluri</label>
                    <div style="float:left; padding-right:15px;">
                         <dx:ASPxSpinEdit ID="txtNivel" runat="server" Width="100px" DecimalPlaces="0" MaxLength="3" MinValue="1" MaxValue="100" />
                    </div>

                    <label id="lblAfis" runat="server" style="display:inline-block; float:left; padding-right:15px;">Afisare ultimul nivel</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbAfisare" runat="server" AutoPostBack ="false">
                            <Items>
                                <dx:ListEditItem Value="1" Text="Nume Post" Selected="true" />
                                <dx:ListEditItem Value="2" Text="Nume Grup" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxCheckBox ID="chkPlan" Text="Plan HC" runat="server" Checked="true" />
                        <dx:ASPxCheckBox ID="chkAprobat" Text="HC Aprobat" runat="server" Checked="true" />
                        <dx:ASPxCheckBox ID="chkEfectiv" Text="HC Efectiv" runat="server" Checked="true" />
                    </div>

                    <div style="float:left;">
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxDiagram runat="server" ID="dgPost" SimpleView="true" Width="100%" Height="100%">
                    <Mappings>
                        <Node Key="Id" ParentKey="IdSuperior" Text="DenumireRO" />
                    </Mappings>
                </dx:ASPxDiagram>
            </td>
        </tr>
    </table>



</asp:Content>
