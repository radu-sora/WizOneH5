<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Cursuri.ascx.cs" Inherits="WizOne.Personal.Cursuri" %>

<script language="javascript" type="text/javascript">

    function grDateCursuri_CustomButtonClick(s, e) {
        switch (e.buttonID) {
            case "btnAtasament":
                pnlLoading.Show();
                grDateCursuri.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierCursMode);
                break;
        }
    }

    function GoToFisierCursMode(Value) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=12&id=' + Value, '_blank ')
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
                <dx:ASPxGridView ID="grDateCursuri" runat="server" ClientInstanceName="grDateCursuri" ClientIDMode="Static" Width="100%" AutoGenerateColumns="False"  OnDataBinding="grDateCursuri_DataBinding"  OnInitNewRow="grDateCursuri_InitNewRow"
                    OnRowInserting="grDateCursuri_RowInserting" OnRowUpdating="grDateCursuri_RowUpdating" OnRowDeleting="grDateCursuri_RowDeleting" OnCommandButtonInitialize="grDateCursuri_CommandButtonInitialize" OnHtmlEditFormCreated="grDateCursuri_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateCursuri_CustomButtonClick(s, e); }" ContextMenu="ctx"/> 
                    <SettingsEditing Mode="EditFormAndDisplayRow" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <SettingsPopup>
                        <HeaderFilter MinHeight="140px"></HeaderFilter>
                    </SettingsPopup>
                    <Columns>
                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false" ReadOnly="True"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdTipCurs" Name="IdTipCurs" Caption="Tip curs" Width="100px"  >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="TipCurs" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Nume complet"  Width="150px"  />
                        <dx:GridViewDataTextColumn FieldName="Info" Name="Info" Caption="Info" Width="100px" >
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px"  ShowInCustomizationForm="True" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                            </PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px"  ShowInCustomizationForm="True" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                            </PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Numar zile" FieldName="NrZile" Name="NrZile"  Width="70px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px"  />						
                        <dx:GridViewDataDateColumn Caption="TIME" FieldName="TIME" Name="TIME" Width="100px" Visible="False">
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Numar Ore" FieldName="NrOre" Name="NrOre"  Width="70px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn Caption="Descriere curs" FieldName="IdDescriereCurs" Name="IdDescriereCurs"  Width="150px" ShowInCustomizationForm="True">
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="DescriereCurs" ValueField="IdAuto" DropDownStyle="DropDown" ValueType="System.Int32">
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn Caption="Nume furnizor" FieldName="NumeFurnizor" Name="NumeFurnizor" Width="100px" ShowInCustomizationForm="True">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="TemaCurs" Name="TemaCurs" Caption="Tema curs"  Width="100px"  ShowInCustomizationForm="True" >
                            <PropertiesTextEdit EnableFocusedStyle="False">
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn FieldName="DataCurs" Name="DataCurs" Caption="Data curs"  Width="100px" ShowInCustomizationForm="True" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                            </PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Buget" FieldName="Buget" Name="Buget" Width="70px" ShowInCustomizationForm="True">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn Caption="Moneda" FieldName="IdMoneda" Name="IdMoneda"  Width="75px" ShowInCustomizationForm="True">
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Abreviere" ValueField="Id" DropDownStyle="DropDown" ValueType="System.Int32">
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn Caption="Nume operator" FieldName="Operator" Name="Operator"  Width="150px" ShowInCustomizationForm="True">
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" DropDownStyle="DropDown" ValueType="System.Int32">
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn Caption="Perioada amortizare" FieldName="PerioadaAmortizare" Name="PerioadaAmortizare"  Width="100px" ShowInCustomizationForm="True">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data Expirare Autorizare" FieldName="AutorizareExpirare" Name="AutorizareExpirare"  Width="100px" ShowInCustomizationForm="True">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                            </PropertiesDateEdit>
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Modificabil" FieldName="Modificabil" Name="Modificabil" Visible="False" >
                        </dx:GridViewDataTextColumn>
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
                                <Style Paddings-PaddingRight="5px" >
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
                                </Style>
                            </Styles>
                        </NewButton>
                    </SettingsCommandButton>
                    <Templates>
                        <EditForm>
                            <div style="padding: 4px 3px 4px">
                                <table>
                                    <tr>
                                        <td style="padding-left:10px !important;">Tip curs</td>
                                        <td style="padding:10px !important;"  >Nume complet</td> 
                                        <td style="padding:10px !important;"  >Info</td> 
                                        <td style="padding:10px !important;"  >Descriere curs</td>
                                        <td style="padding:10px !important;">Data inceput</td>
                                        <td style="padding:10px !important;">Data sfarsit</td>
                                        <td style="padding:10px !important;">Numar zile</td>
                                        <td style="padding:10px !important;">Numar ore</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" ><dx:ASPxComboBox ID="cmbTipCurs" runat="server" Width="250px" ValueField="IdAuto" DropDownWidth="200" TextField="TipCurs" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdTipCurs") %>' />
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNumeComplet" runat="server" Width="250px" Value='<%# Bind("NumeComplet") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtInfo" runat="server" Width="250px" Value='<%# Bind("Info") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxComboBox ID="cmbDescriere" runat="server" Width="250px" ValueField="IdAuto" DropDownWidth="200" TextField="DescriereCurs" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdDescriereCurs") %>' />
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deDataInceput" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataInceput") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deDataSfarsit" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataSfarsit") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNrZile" runat="server" Width="100px" Value='<%# Bind("NrZile") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNrOre" runat="server" Width="100px" Value='<%# Bind("NrOre") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left:10px !important;">Nume furnizor</td>
                                        <td style="padding:10px !important;">Tema curs</td>
                                        <td style="padding:10px !important;">Nume operator</td>
                                        <td style="padding:10px !important;">Perioada amortizare</td>
                                        <td style="padding:10px !important;">Data curs</td>
                                        <td style="padding:10px !important;">Data expirare autorizare</td>
                                        <td style="padding:10px !important;">Buget</td>
                                        <td style="padding:10px !important;">Moneda</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtNumeFurnizor" runat="server" Width="250px" Value='<%# Bind("NumeFurnizor") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtTema" runat="server" Width="250px" Value='<%# Bind("TemaCurs") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxComboBox ID="cmbOperator" runat="server" Width="250px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("Operator") %>' />
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtPerAmortiz" runat="server" Width="250px" Value='<%# Bind("PerioadaAmortizare") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deDataCurs" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataCurs") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="deDataExpAut" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("AutorizareExpirare") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxTextBox ID="txtBuget" runat="server" Width="100px" Value='<%# Bind("Buget") %>' /></td>
                                        <td style="padding:10px !important;" ><dx:ASPxComboBox ID="cmbMoneda" runat="server" Width="100px" ValueField="Id" DropDownWidth="200" TextField="Abreviere" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdMoneda") %>' />
                                    </tr>

                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadAtas" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="Incarca document" ShowTextBox="false"
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