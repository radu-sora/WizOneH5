<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Sanctiuni.ascx.cs" Inherits="WizOne.Personal.Sanctiuni" %>

<style type="text/css">
    .auto-style2 {
        height: 44px;
        width: 128px;
    }
    .auto-style3 {
        width: 128px;
    }
    .auto-style6 {
        height: 48px;
        width: 102px;
    }
    .auto-style8 {
        width: 1311px;
    }
    .auto-style9 {
        width: 102px;
        height: 44px;
    }
    .auto-style18 {
        height: 44px;
        width: 7px;
    }
    .auto-style22 {
        width: 128px;
        height: 52px;
    }
    .auto-style23 {
        height: 48px;
        width: 128px;
    }
    .auto-style25 {
        width: 102px;
        height: 52px;
    }
    .auto-style26 {
        height: 44px;
    }
    .auto-style30 {
        width: 78px;
    }
    .auto-style31 {
        width: 78px;
        height: 44px;
    }
    .auto-style33 {
        height: 48px;
    }
</style>

<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateSanctiuni" runat="server" ClientInstanceName="grDateSanctiuni" ClientIDMode="Static" Width="80%" AutoGenerateColumns="False" OnDataBinding="grDateSanctiuni_DataBinding" OnInitNewRow="grDateSanctiuni_InitNewRow" 
                    OnRowInserting="grDateSanctiuni_RowInserting" OnRowUpdating="grDateSanctiuni_RowUpdating" OnRowDeleting="grDateSanctiuni_RowDeleting" OnHtmlEditFormCreated="grDateSanctiuni_HtmlEditFormCreated">        
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />    
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateSanctiuni_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="EditFormAndDisplayRow" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

<SettingsPopup>
<HeaderFilter MinHeight="140px"></HeaderFilter>
</SettingsPopup>
                    <Columns>
                        <dx:GridViewCommandColumn Width="100px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="3" ButtonType="Image" Caption=" " Name="butoaneGrid">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Sanctiune aplicata"  Width="280px" VisibleIndex="4" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput" Width="100px" VisibleIndex="5" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" Width="100px" VisibleIndex="6" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="ValoareAbsoluta" Name="ValoareAbsoluta" Caption="Valoare sanctiune"  Width="70px" VisibleIndex="7"  >                        
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="ValoareProcent" Name="ValoareProcent" Caption="Valoare %"  Width="70px" VisibleIndex="8"  />  
                        <dx:GridViewDataMemoColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" VisibleIndex="9" Width="230px"/>

                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false" VisibleIndex="0"/>
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" VisibleIndex="1" />
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" VisibleIndex="2" />
                        <dx:GridViewDataTextColumn Caption="Nr. inreg. sesizare" FieldName="NrInregSesizare" Name="NrInregSesizare" VisibleIndex="10" Width="70px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data Inreg. Sesizare" FieldName="DataInregSesizare" Name="DataInregSesizare" ShowInCustomizationForm="True" VisibleIndex="11" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataMemoColumn Caption="Materiale doveditoare" FieldName="MaterialeDoveditoare" Name="MaterialeDoveditoare" ShowInCustomizationForm="True" VisibleIndex="12" Width="200px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataMemoColumn>
                        <dx:GridViewDataTextColumn Caption="Nr. inreg. comisie" FieldName="NrInregComisie" Name="NrInregComisie" ShowInCustomizationForm="True" VisibleIndex="13" Width="70px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data inreg. comisie" FieldName="DataInregComisie" Name="DataInregComisie" ShowInCustomizationForm="True" VisibleIndex="14" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Nr. inreg. convocare" FieldName="NrInregConvocare" Name="NrInregConvocare" ShowInCustomizationForm="True" VisibleIndex="15" Width="70px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data inreg. convocare" FieldName="DataInregConvocare" Name="DataInregConvocare" ShowInCustomizationForm="True" VisibleIndex="16" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn Caption="Data cercetare" FieldName="DataCercetare" Name="DataCercetare" ShowInCustomizationForm="True" VisibleIndex="17" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Nr. proces verbal" FieldName="NrProcesCercetare" Name="NrProcesCercetare" ShowInCustomizationForm="True" VisibleIndex="18" Width="70px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data proces verbal" FieldName="DataProcesCercetare" Name="DataProcesCercetare" ShowInCustomizationForm="True" VisibleIndex="19" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Nr. decizie" FieldName="NrDecizie" Name="NrDecizie" ShowInCustomizationForm="True" VisibleIndex="20" Width="70px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data decizie" FieldName="DataDecizie" Name="DataDecizie" ShowInCustomizationForm="True" VisibleIndex="21" Width="100px">
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn Caption="Data radiere sanctiune" FieldName="DataRadiereSanctiune" Name="DataRadiereSanctiune" ShowInCustomizationForm="True" VisibleIndex="22" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn Caption="Data comunicare decizie" FieldName="DataComunicareDecizie" Name="DataComunicareDecizie" ShowInCustomizationForm="True" VisibleIndex="23" Width="100px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Componenta Comisie" FieldName="ComponentaComisie" Name="ComponentaComisie" ShowInCustomizationForm="True" VisibleIndex="24" Width="250px">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                    </Columns>
                    
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
<Paddings PaddingTop="10px" PaddingRight="10px"></Paddings>
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Renunta">
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" >
<Paddings PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </EditButton>
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                        </DeleteButton>
                        <NewButton Image-ToolTip="Rand nou">
                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                            <Styles>
                                <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" >
<Paddings PaddingLeft="5px" PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </NewButton>
                    </SettingsCommandButton>

                    <Templates>
                        <EditForm>
                            <div style="padding: 4px 3px 4px">
                                <table class="auto-style8">
                                    <tr>
                                        <td id="lblDesc" runat="server"  style="padding-left:10px;" class="auto-style2">Descriere</td>
                                        <td id="lblSanc" runat="server"  style="padding-left:10px;" colspan="2" class="auto-style26">Sanctiune aplicata</td>
                                        <td style="padding: 10px !important;" colspan="2">
                                            <dx:ASPxComboBox ID="cmbObi" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="NumeCompus" Value='<%# Bind("IdObiect") %>' ValueField="IdObiect" ValueType="System.Int32" Width="225px" />
                                            <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                            <td class="auto-style18" style="padding:10px;">&nbsp;</td>
                                            <td class="auto-style18">&nbsp;</td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td rowspan="3" style="vertical-align:top;padding:10px;" class="auto-style3"><dx:ASPxMemo ID="txtDesc" runat="server" Width="500px" Height="150" Text='<%# Bind("Descriere") %>' /></td>
                                        <td id="lblDataCercet" runat="server" style="padding:10px;" aria-multiline="True" class="auto-style9">Data cercetarii</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataCercetare" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataCercetare") %>' Width="110px" />
                                            </td>
                                            <td id="lblDataInc" runat="server" class="auto-style9" style="padding: 10px;">Data inceput</td>
                                            <td class="auto-style9" style="padding:10px;">
                                                <dx:ASPxDateEdit ID="txtDataInc" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataInceput") %>' Width="110" />
                                            </td>
                                            <td id="lblDataSf" runat="server" class="auto-style9" style="padding:10px;">Data sfarsit</td>
                                            <td class="auto-style6" style="padding:10px;">
                                                <dx:ASPxDateEdit ID="txtDataSf" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataInceput") %>' Width="110" />
                                            </td>
                                            <td></td>
                                   </tr>
                                    <tr>
                                        <td aria-multiline="True" class="auto-style31" style="padding:10px;">&nbsp;</td>
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">&nbsp;</td>
                                        <td id="lblVal" runat="server" class="auto-style18" style="padding:10px;">Valoare sanctiune</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtVal" ClientInstanceName="txtVal" runat="server" Width="110px" Value='<%# Bind("ValoareAbsoluta") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                        <td id="lblValProc" runat="server" class="auto-style18">Valoare%</td>
                                        <td class="auto-style30" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtProc" ClientInstanceName="txtProc" runat="server" Width="110px" Value='<%# Bind("ValoareProcent") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style25" style="padding:10px;" aria-multiline="True">
                                            &nbsp;</td>
                                        <td class="auto-style9" style="padding:10px;" aria-multiline="True">
                                            &nbsp;</td>
                                        <td class="auto-style9" style="padding:10px;">
                                            &nbsp;</td>
                                        <td class="auto-style9" style="padding:10px;" aria-multiline="True">
                                            &nbsp;</td>
                                        <td class="auto-style9" style="padding:10px;">
                                            &nbsp;</td>
                                        <td class="auto-style9" style="padding:10px;">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td id="lblMatDov" runat="server" aria-multiline="True" class="auto-style22" style="padding:10px;">Materiale doveditoare</td>
                                        <td id="lblNrInregSes" runat="server" aria-multiline="True" class="auto-style9" style="padding:10px;">Numar inregistrare sesizare</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtNrInregSesizare" ClientInstanceName="txtNrInregSesizare" runat="server" Width="110px" Value='<%# Bind("NrInregSesizare") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                        <td id="lblNrInregConv" runat="server" class="auto-style6" style="padding:10px;">Numar inregistrare convocare</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtNrInregConvocare" ClientInstanceName="txtNrInregConvocare" runat="server" Width="110px" Value='<%# Bind("NrInregConvocare") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                        <td id="lblNrDecizie" runat="server" class="auto-style6" style="padding:10px;">Numar decizie</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtNrDecizie" ClientInstanceName="txtNrDecizie" runat="server" Width="110px" Value='<%# Bind("NrDecizie") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style23" rowspan="3" style="padding:10px;">
                                            <dx:ASPxMemo ID="txtMaterialeDoveditoare" runat="server" Height="150px" Text='<%# Bind("MaterialeDoveditoare") %>' Width="500px" />
                                        </td>
                                        <td id="lblDataInregSes" runat="server" class="auto-style25" style="padding:10px;" aria-multiline="True">
                                            Data inregistrare sesizare</td>
                                        <td class="auto-style25" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataInregSesizare" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataInregSesizare") %>' Width="110px" />
                                        </td>
                                        <td id="lblDataInregConv" runat="server" class="auto-style25" style="padding:10px;">
                                            Data inregistrare convocare</td>
                                        <td class="auto-style25" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataInregConvocare" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataInregConvocare") %>' Width="110px" />
                                        </td>
                                        <td id="lblDataDecizie" runat="server" class="auto-style25" style="padding:10px;">Data decizie
                                        </td>
                                        <td class="auto-style25" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataDecizie" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataDecizie") %>' Width="110px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                        <td class="auto-style6" style="padding:10px;">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td id="lblNrInregComisie" runat="server" class="auto-style6" style="padding:10px;">Numar inregistrare comisie
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtNrInregComisie" ClientInstanceName="txtNrInregComisie" runat="server" Width="110px" Value='<%# Bind("NrInregComisie") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>  
                                            </dx:ASPxSpinEdit>
                                        </td>
                                        <td id="lblNrProcVerb" runat="server" class="auto-style6" style="padding:10px;">
                                            Numar proces verbal</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxSpinEdit ID="txtNrProcesCercetare" ClientInstanceName="txtNrProcesCercetare" runat="server" Width="110px" Value='<%# Bind("NrProcesCercetare") %>' DecimalPlaces="0">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons>
                                            </dx:ASPxSpinEdit>
                                        </td>
                                        <td id="lblDataComDecizie" runat="server" class="auto-style6" style="padding:10px;">
                                            Data comunicare decizie</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataComunicareDecizie" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataComunicareDecizie") %>' Width="110px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="1" style="padding:10px;" class="auto-style33">
                                            <label id="lblDoc" runat="server" clientidmode="Static" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">
                                            &nbsp;
                                            </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadSanc" runat="server" AutoStartUpload="true" BrowseButton-Text="Incarca Document" ClientIDMode="Static" ClientInstanceName="btnDocUploadSanc" FileUploadMode="OnPageLoad" Height="28px" OnFileUploadComplete="btnDocUploadSanc_FileUploadComplete" ShowProgressPanel="true" ShowTextBox="false" ToolTip="incarca document" UploadMode="Advanced" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png">
                                                    </Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False">
                                                </ValidationSettings>
                                                <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
                                        </td>
                                        <td id="lblDataInregComisie" runat="server" class="auto-style25" style="padding:10px;">Data inregistrare comisie</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataInregComisie" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataInregComisie") %>' Width="110px" />
                                        </td>
                                        <td id="lblDataProcVerb" runat="server" class="auto-style9" style="padding:10px;">Data proces verbal</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataProcesCercetare" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataProcesCercetare") %>' Width="110px" />
                                        </td>
                                        <td id="lblDataRadSanc" runat="server" class="auto-style9" style="padding:10px;">Data radiere sanctiune</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="txtDataRadiereSanctiune" runat="server" EditFormat="Date" EditFormatString="dd/MM/yyyy" DisplayFormatString="dd/MM/yyyy" UseMaskBehavior="true" Value='<%# Bind("DataRadiereSanctiune") %>' Width="110px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style33" colspan="1" style="padding:10px;">&nbsp;</td>
                                        <td id="lblCompComisie" runat="server" class="auto-style6" style="padding:10px;">Componenta comisiei</td>
                                        <td class="auto-style6" style="padding:10px;" colspan="5">
                                            <dx:ASPxMemo ID="txtComponentaComisie" ClientInstanceName="txtComponentaComisie" ClientIDMode="Static" runat="server" Height="150px" Text='<%# Bind("ComponentaComisie") %>' Width="500px" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="auto-style3" style="padding:10px;">
                                            <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" runat="server" ReplacementType="EditFormUpdateButton" />
                                                <dx:ASPxGridViewTemplateReplacement ID="CancelButton" runat="server" ReplacementType="EditFormCancelButton" />
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

<script>

    function grDateSanctiuni_CustomButtonClick(s, e) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=6&id=' + s.GetRowKey(s.GetFocusedRowIndex()), '_blank ')
    }

    function EndUpload(s) {
        lblDoc.innerText = s.cpDocUploadName;
        s.cpDocUploadName = null;
    }

</script>