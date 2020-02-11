<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ImportValori.aspx.cs" Inherits="WizOne.Pagini.ImportValori" %>


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
                        <dx:ASPxButton ID="btnNotif" ClientInstanceName="btnNotif" ClientIDMode="Static" runat="server" Text="Trimite notificare" AutoPostBack="true" OnClick="btnNotif_Click"  oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/notif.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSave_Click"  oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>
			<div>
                <tr>
                 <td  valign="top">
               <fieldset  >
                <legend class="legend-font-size">Import valori</legend>
                <table width="10%" >
                    <tr>
                        <td>
                            <dx:ASPxLabel ID="lblTip" runat="server" Text="Tip import:" >                                
                            </dx:ASPxLabel>
                            <dx:ASPxRadioButton ID="rbTip1" Width="75" runat="server" RepeatDirection="Horizontal"  Text="Obiective" Enabled="true"  ClientInstanceName="rbTip1"
                                    GroupName="Tip">                                    
                            </dx:ASPxRadioButton>                     
                            <dx:ASPxRadioButton ID="rbTip2"  Width="175" runat="server" RepeatDirection="Horizontal"  Text="Competente" Enabled="true" ClientInstanceName="rbTip2" 
                                    GroupName="Tip">                              
                            </dx:ASPxRadioButton>
                        </td>
                    </tr>
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
                </table>
              </fieldset >
               <fieldset border="0">                     
                <legend class="legend-border"></legend>            
                <table width="30%" >    
                    <tr>
                        <td align="left">
                            <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding" OnInitNewRow="grDate_InitNewRow" OnCustomCallback="grDate_CustomCallback"
                                OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
                                <SettingsBehavior AllowFocusedRow="true" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />                                   
                                <SettingsEditing Mode="Inline" />      
                                <ClientSideEvents ContextMenu="ctx" />                                
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
                                    <dx:GridViewDataComboBoxColumn FieldName="Tip" Name="Tip" Caption="Tip" Width="100px" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.String" DropDownStyle="DropDown" >
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>                                                                                                          
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
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
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblObiectiv" Width="100" runat="server"  Text="Obiectiv" ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="100%"  ID="cmbObiectiv"   runat="server" DropDownStyle="DropDown"  TextField="Obiectiv" ValueField="IdObiectiv" AutoPostBack="false"  ValueType="System.Int32" >                               
							</dx:ASPxComboBox>
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblComp" Width="100" runat="server"  Text="Competenta" ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="100%"  ID="cmbComp"   runat="server" DropDownStyle="DropDown"  TextField="DenCompetenta" ValueField="IdCompetenta" AutoPostBack="false"  ValueType="System.Int32" >                               
							</dx:ASPxComboBox>
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblPerioada" Width="100" runat="server"  Text="Perioada" ></dx:ASPxLabel >	
							<dx:ASPxComboBox Width="100%"  ID="cmbPerioada"   runat="server" DropDownStyle="DropDown"  TextField="DenPerioada" ValueField="IdPerioada" AutoPostBack="false"  ValueType="System.Int32" >                               
							</dx:ASPxComboBox>
						</td>
					</tr>
  
                </table>
              </fieldset >
                </td> 
            </tr>      
		</div>

    </body>

</asp:Content>