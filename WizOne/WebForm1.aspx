<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WizOne.WebForm1" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
<script type="text/javascript">


</script>

                        <dx:ASPxGridLookup ID="cmbAng" runat="server" ClientIDMode="Static" ClientInstanceName="cmbAng" SelectionMode="Single" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                            KeyFieldName="F10003" TextFormatString="{0} {1}" Width="250px" CallbackPageSize="15" EnableCallbackMode="true" >
                            <ClearButton DisplayMode="OnHover" />
                            <ClientSideEvents ValueChanged="OnValueChanged" />
                            <Columns>
                                <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" /> 
                                <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" />
                                <dx:GridViewDataTextColumn FieldName="Filiala" />
                                <dx:GridViewDataTextColumn FieldName="Sectie" />
                                <dx:GridViewDataTextColumn FieldName="Departament" Caption="Dept" />
                                <dx:GridViewDataTextColumn FieldName="Functia" />
                                <dx:GridViewDataCheckColumn FieldName="AngajatActiv" Caption="Stare" />
                            </Columns>
                            <GridViewProperties>
                                <Settings ShowFilterRow="true" />
                                <SettingsBehavior AllowDragDrop="False" EnableRowHotTrack="True" />
                                <SettingsPager NumericButtonCount="3" EnableAdaptivity="true" />
                            </GridViewProperties>
                        </dx:ASPxGridLookup>				
    
</asp:Content>
