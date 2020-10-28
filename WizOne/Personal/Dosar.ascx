<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Dosar.ascx.cs" Inherits="WizOne.Personal.Dosar" %>



<body>

    <table style="width:100%">
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateDosar" runat="server" ClientInstanceName="grDateDosar" ClientIDMode="Static" Width="60%" AutoGenerateColumns="false" KeyFieldName="F10003;IdObiect" OnDataBinding="grDateDosar_DataBinding"  OnInitNewRow="grDateDosar_InitNewRow"
                    OnRowInserting="grDateDosar_RowInserting" OnRowUpdating="grDateDosar_RowUpdating" OnRowDeleting="grDateDosar_RowDeleting" OnHtmlEditFormCreated="grDateDosar_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { OnCustomButtonClickDosar(s, e); }" EndCallback="function(s,e) { OnEndCallbackDosar(s,e); }" ContextMenu="ctx" />                                      
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Denumire document" Width="450px" >
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" Width="150px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="FisierNume" Name="FisierNume" Caption="Nume fisier"  Width="350px" Settings-ShowEditorInBatchEditMode="false" />
                        <dx:GridViewDataTextColumn FieldName="FisierExtensie" Name="FisierExtensie" Caption="Extensie fisier"  Width="150px" Settings-ShowEditorInBatchEditMode="false" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Angajat"  Width="75px" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataDateColumn FieldName="Obligatoriu" Name="Obligatoriu" Caption="Obligatoriu" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataDateColumn FieldName="AreFisier" Name="AreFisier" Caption="AreFisier" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />
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
                                        <td style="padding-left:10px !important;" colspan="2">Nume beneficiu</td>
                                    </tr>
                                    <tr><td style="padding:10px !important;" colspan="2"><dx:ASPxComboBox ID="cmbNumeBen" runat="server" Width="250px" ValueField="IdObiect" DropDownWidth="200" TextField="NumeCompus" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdObiect") %>' />
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"  colspan="2">Descriere</td>                                      
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"  colspan="2"><dx:ASPxTextBox ID="txtDesc" runat="server" Width="250px" Value='<%# Bind("Descriere") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp;</label>
                                            <dx:ASPxUploadControl ID="btnDocUploadDosar" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadDosar" OnFileUploadComplete="btnDocUploadDosar_FileUploadComplete" ValidationSettings-ShowErrors="true">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>
                                                <ClientSideEvents FileUploadComplete="function(s,e) { OnEndUploadDosar(s); }" />
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

    <script>

        function OnCustomButtonClickDosar(s, e) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=13&id=' + s.GetRowKey(s.GetFocusedRowIndex()), '_blank ')
        }

        function OnEndUploadDosar(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function OnEndCallbackDosar(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

    </script>

</body>