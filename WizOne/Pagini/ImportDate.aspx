<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ImportDate.aspx.cs" Inherits="WizOne.Pagini.ImportDate" %>


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
    }

    function OnEndCallback(s, e) {
        cmbSablon.PerformCallback();
    }

    function OnClickViz(s, e) {    
        popUpViz.Show();
        e.processOnServer = true;
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
                        <dx:ASPxButton ID="btnViz" ClientInstanceName="btnViz" ClientIDMode="Static" runat="server" Text="Detalii sablon" AutoPostBack="true" OnClick="btnViz_Click"  oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                            <ClientSideEvents Click="function(s,e){ OnClickViz(s, e); }" /> 
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
            </tr>
                
        </table>
			<div>
                <tr>
                 <td  valign="top">
                    <fieldset  >
                    <legend class="legend-font-size">Import date</legend>
                    <table width="10%" >
                        <tr>
                            <td align="center">
                                <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
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
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblSablon" Width="100" runat="server"  Text="Selectare sablon import" ></dx:ASPxLabel >	
							    <dx:ASPxComboBox Width="100%"  ID="cmbSablon" ClientInstanceName="cmbSablon"  runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false"  ValueType="System.Int32" OnCallback="cmbSablon_Callback" >                                                              
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { grDateNomen.PerformCallback(); }" />
							    </dx:ASPxComboBox>
						    </td>
					    </tr>                    
                    </table>
                    </fieldset >
                    <fieldset border="0">                     
                    <legend class="legend-border">Sabloane</legend>            
                    <table width="30%" >    
                        <tr>
                            <td align="left">
                                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding" OnInitNewRow="grDate_InitNewRow" OnCustomCallback="grDate_CustomCallback"
                                    OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting" >
                                    <SettingsBehavior AllowFocusedRow="true" />
                                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />                                   
                                    <SettingsEditing Mode="Inline" />      
                                    <ClientSideEvents ContextMenu="ctx" />                                
                                    <Columns>
                                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />                                    
                                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="75px" />
                                        <dx:GridViewDataTextColumn FieldName="NumeSablon" Name="NumeSablon" Caption="Nume sablon"  Width="100px"/>                                                                                                                                                           
                                        <dx:GridViewDataTextColumn FieldName="NumeTabela" Name="NumeTabela" Caption="Nume tabela"  Width="100px"/>                                       
                                    </Columns>
                                    <SettingsCommandButton>
                                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                                            <Styles>
                                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
                                                </Style>
                                            </Styles>
                                        </UpdateButton>
                                        <CancelButton ButtonType="Link" Text="Renunta">
                                        </CancelButton>

                                        <EditButton Image-ToolTip="Edit">
                                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                                            <Styles>
                                                <Style Paddings-PaddingRight="5px" />
                                            </Styles>
                                        </EditButton>
                                        <DeleteButton Image-ToolTip="Sterge">
                                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                                        </DeleteButton>
                                        <NewButton Image-ToolTip="Rand nou">
                                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                            <Styles>
                                                <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                            </Styles>
                                        </NewButton>
                                    </SettingsCommandButton>
                                </dx:ASPxGridView>
                            </td>
                        </tr>  
                    </table>
                    </fieldset >

                    <fieldset border="0">                     
                    <legend class="legend-border">Previzualizare</legend>            
                    <table width="70%" >    
                        <tr>
                            <td align="left">
                                <dx:ASPxGridView ID="grDateViz" runat="server" ClientInstanceName="grDateViz" Width="100%"   >
                                    <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" AllowSort="false" />
                                    <Settings ShowFilterRow="false" ShowGroupPanel="False" />
                                    <SettingsSearchPanel Visible="False" />    
                                    <Columns>	              
						            </Columns>
                                </dx:ASPxGridView>
                    
                            </td>
                        </tr> 
                    </table>
                    </fieldset >
                </td> 
            </tr>      
		</div>

    </body>


    <dx:ASPxPopupControl ID="popUpViz" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpVizArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="700px" Height="500px" HeaderText="Previzualizare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpViz" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnIesire" runat="server" Text="Iesire" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        popUpViz.Hide();
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxGridView ID="grDateNomen" runat="server" ClientInstanceName="grDateNomen" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnDataBinding="grDateNomen_DataBinding" OnInitNewRow="grDateNomen_InitNewRow" OnCustomCallback="grDateNomen_CustomCallback"
                                    OnRowInserting="grDateNomen_RowInserting" OnRowUpdating="grDateNomen_RowUpdating" OnRowDeleting="grDateNomen_RowDeleting">
                                    <SettingsBehavior AllowFocusedRow="true" />
                                    <Settings ShowFilterRow="true" ShowColumnHeaders="true"  />                                   
                                    <SettingsEditing Mode="Inline" />      
                                    <ClientSideEvents ContextMenu="ctx" EndCallback="OnEndCallback"/>                                
                                    <Columns>
                                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />                                    
                                        <dx:GridViewDataComboBoxColumn FieldName="ColoanaFisier" Name="ColoanaFisier" Caption="Coloana din fisier" Width="150px" >
                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.String" DropDownStyle="DropDown" >
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn> 
                                        <dx:GridViewDataComboBoxColumn FieldName="ColoanaBD" Name="ColoanaBD" Caption="Coloana din BD" Width="150px" >
                                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.String" DropDownStyle="DropDown" >
                                            </PropertiesComboBox>
                                        </dx:GridViewDataComboBoxColumn> 
                                        <dx:GridViewDataCheckColumn FieldName="Obligatoriu" Name="Obligatoriu" Caption="Obligatoriu"  Width="50px"  />
                                        <dx:GridViewDataTextColumn FieldName="ValoareImplicita" Name="ValoareImplicita" Caption="Valoare implicita"  Width="250px" />
                                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="75px" Visible="false"/>
                                    </Columns>
                                    <SettingsCommandButton>
                                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                                            <Styles>
                                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
                                                </Style>
                                            </Styles>
                                        </UpdateButton>
                                        <CancelButton ButtonType="Link" Text="Renunta">
                                        </CancelButton>

                                        <EditButton Image-ToolTip="Edit">
                                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                                            <Styles>
                                                <Style Paddings-PaddingRight="5px" />
                                            </Styles>
                                        </EditButton>
                                        <DeleteButton Image-ToolTip="Sterge">
                                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                                        </DeleteButton>
                                        <NewButton Image-ToolTip="Rand nou">
                                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                            <Styles>
                                                <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                            </Styles>
                                        </NewButton>
                                    </SettingsCommandButton>
                                </dx:ASPxGridView>
                            </td>
                        </tr> 
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>


</asp:Content>