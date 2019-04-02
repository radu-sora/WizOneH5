﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Candidati.aspx.cs" Inherits="WizOne.Recrutare.Candidati" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" EnableViewState="true" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        function OnItemClick(s,e)
        {
            pnlCall.PerformCallback(s.GetSelectedItem().name);
        }

        function OnTabSelectionChanged(s) {
            //pnlCall.PerformCallback();
        }
    </script>

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
    
    <dx:ASPxCallbackPanel ID="pnlCall" runat="server" ClientInstanceName="pnlCall" OnCallback="pnlCall_Callback">
        <PanelCollection>
            <dx:PanelContent>
              
                <dx:ASPxTabControl ID="pnlTab" runat="server" OnActiveTabChanged="pnlTab_ActiveTabChanged" AutoPostBack="true" Visible="false" >
                    <ClientSideEvents />
                    <Tabs>
                        <dx:Tab Name="tabGen" Text="Date Generale"></dx:Tab>
                        <dx:Tab Name="tabExp" Text="Experienta"></dx:Tab>
                        <dx:Tab Name="tabStu" Text="Studii"></dx:Tab>
                        <dx:Tab Name="tabCom" Text="Competente"></dx:Tab>
                        <dx:Tab Name="tabLim" Text="Limbi straine"></dx:Tab>
                        <dx:Tab Name="tabAta" Text="Atasamente"></dx:Tab>
                        <dx:Tab Name="tabBen" Text="Beneficii solicitate"></dx:Tab>
                    </Tabs>
                </dx:ASPxTabControl>

                <dx:ASPxMenu ID="mMain" runat="server" AllowSelectItem="True" ShowPopOutImages="True" AutoPostBack="false">
                    <ClientSideEvents ItemClick="OnItemClick" />
                    <Items>
                        <dx:MenuItem Name="tabGen" Text="Date Generale"/>
                        <dx:MenuItem Name="tabExp" Text="Experienta"/>
                        <dx:MenuItem Name="tabStu" Text="Studii"/>
                        <dx:MenuItem Name="tabCom" Text="Competente"/>
                        <dx:MenuItem Name="tabLim" Text="Limbi straine"/>
                        <dx:MenuItem Name="tabAta" Text="Atasamente"/>
                        <dx:MenuItem Name="tabBen" Text="Beneficii solicitate"/>
                    </Items>
                </dx:ASPxMenu>



                <dx:ASPxPanel ID="pnlGeneral" runat="server" />

                </dx:PanelContent>
            </PanelCollection>
    </dx:ASPxCallbackPanel>


</asp:Content>
