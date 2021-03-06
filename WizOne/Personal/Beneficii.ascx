<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Beneficii.ascx.cs" Inherits="WizOne.Personal.Beneficii" %>

<body>

    <table width="100%">
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateBeneficii" runat="server" ClientInstanceName="grDateBeneficii" ClientIDMode="Static" Width="65%" AutoGenerateColumns="false"  OnDataBinding="grDateBeneficii_DataBinding"  OnInitNewRow="grDateBeneficii_InitNewRow"
                    OnRowInserting="grDateBeneficii_RowInserting" OnRowUpdating="grDateBeneficii_RowUpdating" OnRowDeleting="grDateBeneficii_RowDeleting" OnHtmlEditFormCreated="grDateBeneficii_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateBeneficii_CustomButtonClick(s, e); }" EndCallback="function(s,e) { OnEndCallbackBeneficii(s,e); }" ContextMenu="ctx" />                                      
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
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Nume beneficiu"  Width="250px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Caracteristica" Name="Caracteristica" Caption="Caracteristica echipament"  Width="250px" />
                        <dx:GridViewDataDateColumn FieldName="DataPrimire" Name="DataPrimire" Caption="Data primire"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataExpirare" Name="DataExpirare" Caption="Data expirare"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="FisierNume" Name="FisierNume" Caption="Nume Fisier"/>
                        <dx:GridViewDataCheckColumn FieldName="VineDinPosturi" Name="VineDinPosturi" Caption="Obligatoriu" />
                        <dx:GridViewDataCheckColumn FieldName="EsteLaDosar" Name="EsteLaDosar" Caption="Indosariat" />

                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />
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
                                        <td id="lblNume" runat="server" style="padding-left:10px !important;" colspan="3">Nume beneficiu</td>
                                    </tr>
                                    <tr><td style="padding:10px !important;" colspan="3"><dx:ASPxComboBox ID="cmbNumeBen" runat="server" Width="250px" ValueField="IdObiect" DropDownWidth="200" TextField="NumeCompus" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdObiect") %>' />
                                    </tr>
                                    <tr>
                                        <td id="lblDataPrimire" runat="server" style="padding:10px !important;">Data primire</td>
                                        <td id="lblDataExp" runat="server" style="padding:10px !important;">Data expirare</td>
                                        <td id="lblLaDosar" runat="server" style="padding:10px !important;">Indosariat</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataPrim" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataPrimire") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataExp" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataExpirare") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chkLaDosar" runat="server" Checked='<%#  Eval("EsteLaDosar") == DBNull.Value ? false : Convert.ToBoolean(Eval("EsteLaDosar"))%>' /></td>
                                    </tr>
                                    <tr>
                                        <td id="lblCaract" runat="server" style="padding:10px !important;" colspan="3">Caracteristica echipament</td>                                      
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="3"><dx:ASPxTextBox ID="txtCaract" runat="server" Width="250px" Value='<%# Bind("Caracteristica") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="3">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadBen" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadBen" OnFileUploadComplete="btnDocUploadBen_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                                <ClientSideEvents FileUploadComplete="function(s,e) { EndUpload(s); }" />
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

        function grDateBeneficii_CustomButtonClick(s, e) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=10&id=' + s.GetRowKey(s.GetFocusedRowIndex()), '_blank ')
        }

        function EndUpload(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function OnEndCallbackBeneficii(s, e) {
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