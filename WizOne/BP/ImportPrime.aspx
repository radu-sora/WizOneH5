<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ImportPrime.aspx.cs" Inherits="WizOne.BP.ImportPrime" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function StartUpload() {
    }

    function EndUpload(s) {
        s.cpDocUploadName = null;
        grDate.PerformCallback();
    }

</script>
	<style type="text/css">
        .legend-border
        {
             border: 0;
        }
	</style>
    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnImport" ClientInstanceName="btnImport" ClientIDMode="Static" runat="server" Text="Import" AutoPostBack="true" OnClick="btnImport_Click"  oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/incarca.png"></Image>
                        </dx:ASPxButton>     
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>
			<div>
                <tr>
                    <td valign="top">
                        <fieldset>
                            <legend class="legend-font-size">Import prime</legend>
                            <table width="10%">
                                <tr>
                                    <td align="center" >
                                        <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" ToolTip="Exporta document"  AutoPostBack="true" OnClick="btnExport_Click" oncontextMenu="ctx(this,event)">
                                            <Image Url="~/Fisiere/Imagini/Icoane/m3.png"></Image>
                                        </dx:ASPxButton>
                                    </td>
                                      <td align="center" >
                                        <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"  NullText="Import"
                                            BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="Incarca document" ShowTextBox="false"
                                            ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocUploadResource1">
                                            <BrowseButton>
                                                <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                            </BrowseButton>
                                            <ValidationSettings ShowErrors="False"></ValidationSettings>

                                            <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                        </dx:ASPxUploadControl>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset>
                            <table width="10%">
                                <tr>
                                    <td>
                                        <dx:ASPxLabel  ID="lblAn" runat="server"   Text="An"></dx:ASPxLabel >
                                        <dx:ASPxComboBox ID="cmbAn" runat="server" ClientInstanceName="cmbAn" ClientIDMode="Static" Width="70px" ValueField="Id" DropDownWidth="100" 
                                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                        </dx:ASPxComboBox>
                                    </td>
                                    <td>
                                        <dx:ASPxLabel  ID="lblLuna" runat="server"  Text="Luna"></dx:ASPxLabel >
                                        <dx:ASPxComboBox ID="cmbLuna" runat="server" ClientInstanceName="cmbLuna" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="150" 
                                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                        </dx:ASPxComboBox>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <fieldset border="0">
                            <legend class="legend-border"></legend>
                            <table width="30%">
                                <tr>
                                    <td align="left">
                                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding" OnCustomCallback="grDate_CustomCallback">
                                            <SettingsBehavior AllowFocusedRow="false" />
                                            <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                                            <SettingsEditing Mode="Inline" />
                                            <ClientSideEvents ContextMenu="ctx" />
                                            <Columns>
                                                <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Marca" Width="100px">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Nume" Name="Nume" Caption="Nume" Width="200px">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="Suma" Name="Suma" Caption="Suma neta" Width="100px">
                                                </dx:GridViewDataTextColumn>
                                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Width="75px" Visible="false" />
                                            </Columns>
                                        </dx:ASPxGridView>
                                    </td>
                                </tr>                        
                                <tr>
                                    <td align="left">
                                        <dx:ASPxLabel ID="lblPrima" Width="150" runat="server" Text="Selectati tipul primei"></dx:ASPxLabel>
                                        <dx:ASPxComboBox Width="150" ID="cmbPrima" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32">
                                        </dx:ASPxComboBox>                             
                                        <dx:ASPxLabel ID="lblAvs" Width="150" runat="server" Text="Avans/Lichidare"></dx:ASPxLabel>
                                        <dx:ASPxComboBox Width="150" ID="cmbAvs" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32">
                                        </dx:ASPxComboBox>
                                    </td>
                                </tr>
                               
                            </table>
                        </fieldset>
                    </td> 
            </tr>      
		</div>

    </body>

</asp:Content>