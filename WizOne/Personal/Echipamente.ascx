<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Echipamente.ascx.cs" Inherits="WizOne.Personal.Echipamente" %>



<body>

    <table width="100%">
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateEchipamente" runat="server" ClientInstanceName="grDateEchipamente" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateEchipamente_DataBinding"  OnInitNewRow="grDateEchipamente_InitNewRow" 
                            OnRowInserting="grDateEchipamente_RowInserting" OnRowUpdating="grDateEchipamente_RowUpdating" OnRowDeleting="grDateEchipamente_RowDeleting" OnCustomUnboundColumnData="grDateEchipamente_CustomUnboundColumnData" OnHtmlEditFormCreated="grDateEchipamente_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateEchipamente_CustomButtonClick(s, e); }" EndCallback="function(s,e) { OnEndCallbackEchipamente(s,e); }" ContextMenu="ctx" /> 
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
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Nume echipament" Width="250px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Caracteristica" Name="Caracteristica" Caption="Caracteristica echipament"  Width="250px" />
                        <dx:GridViewDataDateColumn FieldName="DataPrimire" Name="DataPrimire" Caption="Data primire"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataPredare" Name="DataPredare" Caption="Data predare"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataExpirare" Name="DataExpirare" Caption="Data expirare"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="DurataUtil" Name="DurataUtil" Caption="Durata utilizare" ReadOnly="true"  Width="175px" UnboundType="String" />  
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
                                        <td id="lblNume" runat="server" style="padding-left:10px !important;" colspan="4">Nume echipament</td>
                                    </tr>
                                    <tr><td style="padding:10px !important;" colspan="4"><dx:ASPxComboBox ID="cmbNume" runat="server" Width="250px" ValueField="IdObiect" DropDownWidth="200" TextField="NumeCompus" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdObiect") %>' />
                                    </tr>
                                    <tr>
                                        <td id="lblDataPrimire" runat="server" style="padding:10px !important;">Data primire</td>
                                        <td id="lblDataPredare" runat="server" style="padding:10px !important;">Data predare</td>
                                        <td id="lblDataExp" runat="server" style="padding:10px !important;">Data expirare</td>
                                        <td id="lblLaDosar" runat="server" style="padding:10px !important;">Indosariat</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataPrim" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataPrimire") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataPre" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataPredare") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataExp" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataExpirare") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chkLaDosar" runat="server" Checked='<%#  Eval("EsteLaDosar") == DBNull.Value ? false : Convert.ToBoolean(Eval("EsteLaDosar"))%>' /></td>
                                    </tr>
                                    <tr>
                                        <td id="lblCaract" runat="server" style="padding:10px !important;"  colspan="4">Caracteristica echipament</td>                                      
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"  colspan="4"><dx:ASPxTextBox ID="txtCaract" runat="server" Width="250px" Value='<%# Bind("Caracteristica") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;" colspan="4">
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

        function grDateEchipamente_CustomButtonClick(s, e) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=18&id=' + s.GetRowKey(s.GetFocusedRowIndex()), '_blank ')
        }

        function EndUpload(s) {
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function OnEndCallbackEchipamente(s, e) {
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