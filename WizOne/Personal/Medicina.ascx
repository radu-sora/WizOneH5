<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Medicina.ascx.cs" Inherits="WizOne.Personal.Medicina" %>



<script language="javascript" type="text/javascript">

    function grDateMedicina_CustomButtonClick(s, e) {
        switch (e.buttonID) {
            case "btnAtasament":
                pnlLoading.Show();
                grDateMedicina.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierMedMode);
                break;
        }
    }

    function GoToFisierMedMode(Value) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=5&id=' + Value, '_blank ')
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
            <td>
                <dx:ASPxGridView ID="grDateMedicina" runat="server" ClientInstanceName="grDateMedicina" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false" OnDataBinding="grDateMedicina_DataBinding" OnInitNewRow="grDateMedicina_InitNewRow"  OnCustomCallback="grDateMedicina_CustomCallback"
                    OnRowInserting="grDateMedicina_RowInserting" OnRowUpdating="grDateMedicina_RowUpdating" OnRowDeleting="grDateMedicina_RowDeleting" OnHtmlEditFormCreated="grDateMedicina_HtmlEditFormCreated" OnCellEditorInitialize="grDateMedicina_CellEditorInitialize">        
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  /> 
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateMedicina_CustomButtonClick(s, e); }" ContextMenu="ctx" />    
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
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Medicina muncii/PSI"  Width="250px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataElib" Name="DataElib" Caption="Data eliberarii"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataExp" Name="DataExp" Caption="Data expirarii"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="SerieNrDoc" Name="SerieNrDoc" Caption="Serie si nr. doc."  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="Emitent" Name="Emitent" Caption="Emitent"  Width="150px" />
                        <dx:GridViewDataMemoColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"/>

                        <dx:GridViewDataDateColumn FieldName="DataElibControlMed" Name="DataElibControlMed" Caption="Data eliberare control medical"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="PerioadaValab" Name="PerioadaValab" Caption="Perioada valabilitate"  Width="100px" />
                        <dx:GridViewDataDateColumn FieldName="DataUrmControl" Name="DataUrmControl" Caption="Data urmatorului control"  Width="100px" ReadOnly="true" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdLocatie" Name="IdLocatie" Caption="Locatie"  Width="150px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Manager" Name="Manager" Caption="Manager direct"  Width="150px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="SectorAlim" Name="SectorAlim" Caption="Sector alimentar"  Width="100px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>                        
                        <dx:GridViewDataTextColumn FieldName="AlteRiscuri" Name="AlteRiscuri" Caption="Alte riscuri"  Width="250px" />
                        <dx:GridViewDataComboBoxColumn FieldName="RezultatExamen" Name="RezultatExamen" Caption="Rezultat examen"  Width="100px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                         <dx:GridViewDataCheckColumn FieldName="Risc1" Name="Risc1" Caption="Auto/ Categoria …"  Width="50px"  />
                         <dx:GridViewDataCheckColumn FieldName="Risc2" Name="Risc2" Caption="Lucrul la inaltime"  Width="50px"  />
                         <dx:GridViewDataCheckColumn FieldName="Risc3" Name="Risc3" Caption="Lucrul in ture de noapte"  Width="50px"  />
                         <dx:GridViewDataCheckColumn FieldName="Risc4" Name="Risc4" Caption="Lucrul la casca"  Width="50px"  />
                         <dx:GridViewDataCheckColumn FieldName="Risc5" Name="Risc5" Caption="Zgomot"  Width="50px"  />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat" Visible="false" ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
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
                                        <td style="padding-left:10px !important;">Observatii</td>
                                        <td style="padding-left:10px !important;">Medicina muncii/PSI</td>
                                        <td style="padding:10px !important;">Data eliberarii</td>
                                        <td style="padding:10px !important;">Data expirarii</td>
                                        <td style="padding:10px !important;">Serie si nr. doc.</td>
                                        <td style="padding:10px !important;">Emitent</td>
                                    </tr>
                                    <tr>
                                        <td rowspan="5" style="vertical-align:top;padding:10px !important;"><dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="150" Text='<%# Bind("Observatii") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbObi" runat="server" Width="215px" ValueField="IdObiect" DropDownWidth="200" TextField="NumeCompus" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdObiect") %>' />
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataElib" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataElib") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataExp" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataExp") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtSerie" runat="server" Width="100" Value='<%# Bind("SerieNrDoc") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtEmi" runat="server" Width="200" Value='<%# Bind("Emitent") %>' /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Data eliberare control medical</td>
                                        <td style="padding:10px !important;">Perioada valabilitate</td>
                                        <td style="padding:10px !important;">Data urmatorului control</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataElibCtrlMed" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataElibControlMed") %>' >
                                                                                 <ClientSideEvents  DateChanged="function(s,e) { grDateMedicina.PerformCallback('valab'); }"/>
                                                                             </dx:ASPxDateEdit></td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtValab" runat="server" Width="100" Value='<%# Bind("PerioadaValab") %>' >
                                                                                <ClientSideEvents TextChanged="function(s,e) { grDateMedicina.PerformCallback('valab'); }"/>
                                                                             </dx:ASPxTextBox></td>
                                        <td style="padding:10px !important;"><dx:ASPxDateEdit ID="txtDataUrmCtrl" runat="server" EditFormatString="dd/MM/yyyy" EditFormat="Date" Width="110" Value='<%# Bind("DataUrmControl") %>' ReadOnly="true" /></td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Locatie</td>
                                        <td style="padding:10px !important;">Manager direct</td>
                                        <td style="padding:10px !important;">Sector alimentar</td>
                                        <td style="padding:10px !important;">Rezultat examen</td>
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbLocatie" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdLocatie") %>' />
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbManager" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("Manager") %>' />
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbSectAlim" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("SectorAlim") %>' />                                                                           
                                        <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbRez" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("RezultatExamen") %>' />
                                    </tr>
                                    <tr>
                                        <td style="padding:10px !important;">Alte riscuri</td>
                                        <td style="padding:10px !important;">Riscuri</td>                                   
                                    </tr>    
                                    <tr>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtAlteRiscuri" runat="server" Width="500" Value='<%# Bind("AlteRiscuri") %>' /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chk1"  runat="server" Width="150" Text="Auto/ Categoria ..."  TextAlign="Left"  Checked='<%#  Eval("Risc1") == DBNull.Value ? false : Convert.ToBoolean(Eval("Risc1"))%>'  ClientInstanceName="chkbx1" /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chk2"  runat="server" Width="150" Text="Lucrul la inaltime"  TextAlign="Left"  Checked='<%#  Eval("Risc2") == DBNull.Value ? false : Convert.ToBoolean(Eval("Risc2"))%>'  ClientInstanceName="chkbx2" /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chk3"  runat="server" Width="150" Text="Lucrul in ture de noapte"  TextAlign="Left"  Checked='<%#  Eval("Risc3") == DBNull.Value ? false : Convert.ToBoolean(Eval("Risc3"))%>'  ClientInstanceName="chkbx3" /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chk4"  runat="server" Width="150" Text="Lucrul la casca"  TextAlign="Left"  Checked='<%#  Eval("Risc4") == DBNull.Value ? false : Convert.ToBoolean(Eval("Risc4"))%>'  ClientInstanceName="chkbx4" /></td>
                                        <td style="padding:10px !important;"><dx:ASPxCheckBox ID="chk5"  runat="server" Width="150" Text="Zgomot"  TextAlign="Left"  Checked='<%#  Eval("Risc5") == DBNull.Value ? false : Convert.ToBoolean(Eval("Risc5"))%>'  ClientInstanceName="chkbx5" /></td>

                                        
                                    
                               



                                     </tr>                                    
                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
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