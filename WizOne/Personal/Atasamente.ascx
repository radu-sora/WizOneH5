<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Atasamente.ascx.cs" Inherits="WizOne.Personal.Atasamente" %>



<script language="javascript" type="text/javascript">

    function grDateAtasamente_CustomButtonClick(s, e) {
        switch (e.buttonID) {
            case "btnAtasament":
                pnlLoading.Show();
                grDateAtasamente.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierAtasMode);
                break;
        }
    }

    function GoToFisierAtasMode(Value) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=4&id=' + Value, '_blank ')
        pnlLoading.Hide();
    }

    function StartUpload() {
        //pnlLoading.Show();
    }

    function EndUpload(s) {
        //pnlLoading.Hide();
        lblDoc.innerText = s.cpDocUploadName;
        s.cpDocUploadName = null;
    }

</script>

<body>

    <table width="80%">
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateAtasamente" runat="server" ClientInstanceName="grDateAtasamente" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnDataBinding="grDateAtasamente_DataBinding"  OnInitNewRow="grDateAtasamente_InitNewRow" 
                     OnRowInserting="grDateAtasamente_RowInserting" OnRowUpdating="grDateAtasamente_RowUpdating" OnRowDeleting="grDateAtasamente_RowDeleting" OnHtmlEditFormCreated="grDateAtasamente_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true" /> 
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateAtasamente_CustomButtonClick(s, e); }" ContextMenu="ctx" />    
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdCategory" Name="IdCategory" Caption="Categorie" Width="150px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NameCategory" ValueField="IdCategory" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>					
					    <dx:GridViewDataDateColumn FieldName="DateAttach" Name="DateAttach" Caption="Data" Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
					    <dx:GridViewDataTextColumn FieldName="DescrAttach" Name="DescrAttach" Caption="Descriere"/>		
                        
                        <dx:GridViewDataTextColumn FieldName="IdEmpl" Name="IdEmpl" Caption="Angajat" Visible="false" ShowInCustomizationForm="false"/>
					    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="Time" Caption="Time" Visible="false" ShowInCustomizationForm="false" />
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

                    <Templates>
                        <EditForm>
                            <div style="padding: 4px 3px 4px">
                                <table>
                                    <tr>
                                        <td style="padding-left:10px !important;">Descriere</td>
                                        <td style="padding-left:10px !important;">Categorie</td>
                                    </tr>
                                    <tr>
                                        <td rowspan="5" style="vertical-align:top;padding:10px !important;"><dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="150" Text='<%# Bind("DescrAttach") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbCateg" runat="server" Width="215px" ValueField="IdCategory" DropDownWidth="200" TextField="NameCategory" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdCategory") %>' />
                                            
                                           
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Data</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataDoc" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DateAttach") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadAtas" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadAtas" OnFileUploadComplete="btnDocUploadAtas_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">
                                            <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                                <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </EditForm>
                    </Templates>

                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table> 



</body>
