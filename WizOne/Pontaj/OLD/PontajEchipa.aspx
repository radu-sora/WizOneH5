﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajEchipa.aspx.cs" Inherits="WizOne.Pontaj.PontajEchipa" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" Visible="false" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                
                <dx:ASPxButton ID="btnPeAng" ClientInstanceName="btnPeAng" ClientIDMode="Static" runat="server" Text="Pontaj pe Angajat" AutoPostBack="true" OnClick="btnPeAng_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPeZi" ClientInstanceName="btnPeZi" ClientIDMode="Static" runat="server" Text="Pontaj pe Zi" AutoPostBack="true" OnClick="btnPeZi_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <br />

                <dx:ASPxPivotGrid ID="grDate1" ClientInstanceName="grDate1" runat="server" Width="100%" OptionsCustomization-AllowFilter="false" Visible="false" >
                    <OptionsView ShowDataHeaders="False" ShowColumnHeaders="False" ShowRowHeaders="true" ShowColumnGrandTotals="False" />
                    <Fields>

                        <dx:PivotGridField ID="fieldF10003" FieldName="F10003" Area="RowArea" Caption="Marca" Width="100" AreaIndex="1" />
                        <dx:PivotGridField ID="fNumeComplet" FieldName="NumeComplet" Area="RowArea" Caption="Angajat" Width="200" AreaIndex="2" />


                        <dx:PivotGridField ID="fieldZiua" FieldName="Ziua" Area="ColumnArea" Caption="Ziua" GroupInterval="DateDay"  AreaIndex="3"/>

                        <dx:PivotGridField ID="fieldValStr" FieldName="ValStr" Area="DataArea" Caption="ValStr" SummaryType="Max"  AreaIndex="4" />
                        
                        <dx:PivotGridField ID="fF1" FieldName="F1" Caption="F1" Area="RowArea" Width="100" AreaIndex="5" />
                        <dx:PivotGridField ID="fF2" FieldName="F2" Caption="F2" Area="RowArea" Width="100" AreaIndex="6" />
                        <dx:PivotGridField ID="fF3" FieldName="F3" Caption="F3" Area="RowArea" Width="100" AreaIndex="7" />
                        <dx:PivotGridField ID="fF4" FieldName="F4" Caption="F4" Area="RowArea" Width="100" AreaIndex="8" />
                        <dx:PivotGridField ID="fF5" FieldName="F5" Caption="F5" Area="RowArea" Width="100" AreaIndex="9" />
                        <dx:PivotGridField ID="fF6" FieldName="F6" Caption="F6" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF7" FieldName="F7" Caption="F7" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF8" FieldName="F8" Caption="F8" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF9" FieldName="F9" Caption="F9" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF10" FieldName="F10" Caption="F10" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF11" FieldName="F11" Caption="F11" Area="RowArea" Width="100"/>
                        <dx:PivotGridField ID="fF12" FieldName="F12" Caption="F12" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF13" FieldName="F13" Caption="F13" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF14" FieldName="F14" Caption="F14" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF15" FieldName="F15" Caption="F15" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF16" FieldName="F16" Caption="F16" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF17" FieldName="F17" Caption="F17" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF18" FieldName="F18" Caption="F18" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF19" FieldName="F19" Caption="F19" Area="RowArea" Width="100" />
                        <dx:PivotGridField ID="fF20" FieldName="F20" Caption="F20" Area="RowArea" Width="100" />

                    </Fields>

                    <OptionsView HorizontalScrollBarMode="Visible" />
                    <OptionsPager RowsPerPage="15" ColumnsPerPage="10" />
                </dx:ASPxPivotGrid>

                <br />
    
            </td>
        </tr>
    </table>


</asp:Content>
