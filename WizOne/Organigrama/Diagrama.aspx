<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Diagrama.aspx.cs" Inherits="WizOne.Organigrama.Diagrama" %>

<%@ Register Assembly="DevExpress.Web.ASPxDiagram.v20.1, Version=20.1.7.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxDiagram" TagPrefix="dx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width:100%;">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnOrg" runat="server" Text="Organigrama" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s,e) { window.history.back(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/stare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="Absente_divOuter" style="display:flex; align-items:flex-end; margin:15px 0px;">
                    
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDtVig" runat="server" style="display:inline-block;">Data selectie</label>
                        <dx:ASPxDateEdit id="txtDtVig" ClientIDMode="Static" ClientInstanceName="txtDtVig" runat="server" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Width="100px" AllowNull="false" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblParinte" runat="server" style="display:inline-block;">Superior</label>
                        <dx:ASPxComboBox ID="cmbParinte" runat="server" Width="200px" ValueField="Camp" TextField="Denumire" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblPost" runat="server" style="display:inline-block;">Incepand de la</label>
                        <dx:ASPxComboBox ID="cmbPost" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblLimbi" runat="server" style="display:inline-block;">Limba</label>
                        <dx:ASPxComboBox ID="cmbLimbi" runat="server" AutoPostBack ="false" Width="100px">
                            <Items>
                                <dx:ListEditItem Value="RO" Text="Romana" Selected="true" />
                                <dx:ListEditItem Value="EN" Text="Engleza" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblNivel" runat="server" style="display:inline-block;">Nr niveluri</label>
                         <dx:ASPxSpinEdit ID="txtNivel" runat="server" Width="50px" DecimalPlaces="0" MaxLength="3" MinValue="1" MaxValue="100" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAfis" runat="server" style="display:inline-block;">Afisare ultimul nivel</label>
                        <dx:ASPxComboBox ID="cmbAfisare" runat="server" AutoPostBack ="false" Width="100px">
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
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s,e) { pnlCall.PerformCallback(); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxCallbackPanel ID="pnlCall" runat="server" ClientInstanceName="pnlCall" OnCallback="pnlCall_Callback">
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxDiagram ID="dgPost" ClientInstanceName="dgPost" runat="server" SimpleView="true" SettingsToolbox-Visibility="Collapsed" SettingsGrid-Visible="false">
                                <SettingsAutoLayout Type="Tree" />
                                <Mappings>
                                    <Node Key="Id" ParentKey="IdSuperior" Text="Denumire" />
                                </Mappings>
                            </dx:ASPxDiagram>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
        </tr>
    </table>

    <script>
        dgPost.SetFullscreenMode(true);
    </script>


</asp:Content>
