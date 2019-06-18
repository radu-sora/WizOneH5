<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Sanctiuni.ascx.cs" Inherits="WizOne.Personal.Sanctiuni" %>




<script language="javascript" type="text/javascript">

    function grDateSanctiuni_CustomButtonClick(s, e) {
        switch (e.buttonID) {
            case "btnAtasament":
                pnlLoading.Show();
                grDateSanctiuni.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierSancMode);
                break;
        }
    }

    function GoToFisierSancMode(Value) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=6&id=' + Value, '_blank ')
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

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateSanctiuni" runat="server" ClientInstanceName="grDateSanctiuni" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false" OnDataBinding="grDateSanctiuni_DataBinding" OnInitNewRow="grDateSanctiuni_InitNewRow" 
                    OnRowInserting="grDateSanctiuni_RowInserting" OnRowUpdating="grDateSanctiuni_RowUpdating" OnRowDeleting="grDateSanctiuni_RowDeleting" OnHtmlEditFormCreated="grDateSanctiuni_HtmlEditFormCreated">        
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />    
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateSanctiuni_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="EditFormAndDisplayRow" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Sanctiune aplicata"  Width="250px" >
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput" Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="ValoareAbsoluta" Name="ValoareAbsoluta" Caption="Valoare"  Width="150px"  />                        
                        <dx:GridViewDataTextColumn FieldName="ValoareProcent" Name="ValoareProcent" Caption="Valoare %"  Width="150px"  />  
                        <dx:GridViewDataMemoColumn FieldName="Descriere" Name="Descriere" Caption="Descriere"/>

                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
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
                                        <td style="padding-left:10px !important;">Descriere</td>
                                        <td style="padding-left:10px !important;" colspan="2">Sanctiune aplicata</td>
                                    </tr>
                                    <tr>
                                        <td rowspan="5" style="vertical-align:top;padding:10px !important;"><dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="150" Text='<%# Bind("Descriere") %>' /></td>
                                        <td style="padding:10px !important;" colspan="2"><dx:ASPxComboBox ID="cmbObi" runat="server" Width="215px" ValueField="IdObiect" DropDownWidth="200" TextField="NumeCompus" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdObiect") %>' />
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Data inceput</td>
                                        <td style="padding:10px !important;">Data sfarsit</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataInc" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataInceput") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataSf" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataSfarsit") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Valoare</td>
                                        <td>Valoare %</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtVal" runat="server" Width="110" Value='<%# Bind("ValoareAbsoluta") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtProc" runat="server" Width="110" Value='<%# Bind("ValoareProcent") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadSanc" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadSanc" OnFileUploadComplete="btnDocUploadSanc_FileUploadComplete" ValidationSettings-ShowErrors="false">
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