<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudiiNou.ascx.cs" Inherits="WizOne.Personal.StudiiNou" %>


<script type="text/javascript">
    function OnIndexChangedStudii(s, e) {
        var val = s.GetValue();
        if (val >= 0) {        
            var tb = grDateStudii.GetEditor("NivelISCED");
            var sir = "<%=Session["MP_ComboStudii"] %>";
            var res = sir.split(";");
            for (var i = 0; i < res.length; i++) {
                var linie = res[i].split(",");
                if (linie[0] == val) {
                    tb.SetValue(linie[1]);
                    break;
                }
            }  
        }
    }

    function grDateStudii_CustomButtonClick(s, e) {
        switch (e.buttonID) {
            case "btnAtasament":
                pnlLoading.Show();
                grDateStudii.GetRowValues(e.visibleIndex, 'IdAuto', GoToFisierStudiiMode);
                break;
        }
    }

    function GoToFisierStudiiMode(Value) {
        window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=11&id=' + Value, '_blank ')
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

    function OnValueChanged(s, e) {
        var val = s.GetValue(); 
        if (val >= 0) {   
            var sir = "<%=Session["MP_ComboStudii"] %>";
            var res = sir.split(";");
            for (var i = 0; i < res.length; i++) {
                var linie = res[i].split(",");
                if (linie[0] == val) {
                    txtNivISCED.SetValue(linie[1]);
                    break;
                }
            }  
        }   
    }
</script>

<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateStudii" runat="server" ClientInstanceName="grDateStudii" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnDataBinding="grDateStudii_DataBinding"  OnInitNewRow="grDateStudii_InitNewRow"
                            OnRowInserting="grDateStudii_RowInserting" OnRowUpdating="grDateStudii_RowUpdating" OnRowDeleting="grDateStudii_RowDeleting" OnCustomUnboundColumnData="grDateStudii_CustomUnboundColumnData"  OnCellEditorInitialize="grDateStudii_CellEditorInitialize" OnHtmlEditFormCreated="grDateStudii_HtmlEditFormCreated">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateStudii_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
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
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>

                        <dx:GridViewDataComboBoxColumn FieldName="IdTipInvatamant" Name="IdTipInvatamant" Caption="Tip invatamant"  Width="125px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdNivel" Name="IdNivel" Caption="Nivel studii"  Width="125px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="F71204" ValueField="F71202" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NivelISCED" Name="NivelISCED" Caption="Nivel ISCED" ReadOnly="true"   Width="75px" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdTipInstitutie" Name="IdTipInstitutie" Caption="Tip institutie de invatamant"  Width="175px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeInstitutie" Name="NumeInstitutie" Caption="Nume institutie"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="NrClase" Name="NrClase" Caption="Numar clase"  Width="75px" />
                        <dx:GridViewDataComboBoxColumn FieldName="SirutaLocalitate" Name="SirutaLocalitate" Caption="Localitate" Width="125px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Nivel3" ValueField="SIRUTA" ValueType="System.Int32" DropDownStyle="DropDown">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Nivel3" Caption="Localitate/Sat/Sector" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Nivel2" Caption="Comuna/Oras/Municipiu" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Nivel1" Caption="Judet" Width="130px" />
                                </Columns>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DeLaData" Name="DeLaData" Caption="De la data"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="LaData" Name="LaData" Caption="La data"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="Perioada" Name="Perioada" Caption="Perioada" ReadOnly="true"  Width="75px" UnboundType="String" />
                        <dx:GridViewDataTextColumn FieldName="Specializare" Name="Specializare" Caption="Specializare"  Width="150px" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdProfil" Name="IdProfil" Caption="Profil"  Width="75px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdDomeniu" Name="IdDomeniu" Caption="Domeniu studiat"  Width="100px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Calificare" Name="Calificare" Caption="Calificare"  Width="150px" />                         
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
                                <table class="auto-style8">
                                    <tr>
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">Tip invatamant</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbTipInv" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="Denumire" Value='<%# Bind("IdTipInvatamant") %>' ValueField="Id" ValueType="System.Int32" Width="225px" />
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">Nivel studii</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbNivStudii" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="F71204" Value='<%# Bind("IdNivel") %>' ValueField="F71202" ValueType="System.Int32" Width="225px" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnValueChanged(s, e); }" />
                                            </dx:ASPxComboBox>
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">Nivel ISCED</td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtNivISCED" ClientInstanceName="txtNivISCED" runat="server" Width="100" ReadOnly="true" Value='<%# Bind("NivelISCED") %>' /></td>
                                    </tr>
                                    <tr>                                       
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">Tip institutie de invatamant</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbTipInst" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="Denumire" Value='<%# Bind("IdTipInstitutie") %>' ValueField="Id" ValueType="System.Int32" Width="225px" />
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">Nume institutie</td>
                                         <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtNumeInst" runat="server" Width="225" Value='<%# Bind("NumeInstitutie") %>' /></td>
                                        <td class="auto-style6" style="padding:10px;">Numar clase</td>
                                        <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtNrClase" runat="server" Width="100" Value='<%# Bind("NrClase") %>' /></td>  
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">Localitate</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbLocalitate" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="Nivel3" Value='<%# Bind("SirutaLocalitate") %>' ValueField="SIRUTA" ValueType="System.Int32" Width="225px" >
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Nivel3" Caption="Localitate/Sat/Sector" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Nivel2" Caption="Comuna/Oras/Municipiu" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Nivel1" Caption="Judet" Width="130px" />
                                                </Columns>
                                            </dx:ASPxComboBox>
                                        </td>                                        
                                    </tr>  
                                    <tr>
                                        <td class="auto-style6" style="padding:10px;">Specializare</td>
                                         <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtSpec" runat="server" Width="225" Value='<%# Bind("Specializare") %>' /></td>
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">Profil</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbProfil" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="Denumire" Value='<%# Bind("IdProfil") %>' ValueField="Id" ValueType="System.Int32" Width="225px" />
                                        </td>
                                        <td aria-multiline="True" class="auto-style9" style="padding:10px;">Domeniu studiat</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxComboBox ID="cmbDomeniu" runat="server" AutoPostBack="false" DropDownWidth="200" TextField="Denumire" Value='<%# Bind("IdDomeniu") %>' ValueField="Id" ValueType="System.Int32" Width="225px" />
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">Calificare</td>
                                         <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtCalif" runat="server" Width="225" Value='<%# Bind("Calificare") %>' /></td>
                                    </tr>
                                    <tr>            
                                        <td class="auto-style6" style="padding:10px;">
                                            De la data</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="deDeLaData" runat="server" EditFormat="Date" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" PickerType="Months" UseMaskBehavior="true" Value='<%# Bind("DeLaData") %>' Width="110px" />
                                        </td>
                                        <td class="auto-style6" style="padding:10px;">
                                            La data</td>
                                        <td class="auto-style6" style="padding:10px;">
                                            <dx:ASPxDateEdit ID="deLaData" runat="server" EditFormat="Date" EditFormatString="MM/yyyy" DisplayFormatString="MM/yyyy" PickerType="Months" UseMaskBehavior="true" Value='<%# Bind("LaData") %>' Width="110px" />
                                        </td>                          
                                        <td colspan="1" style="padding:10px;" class="auto-style33">
                                            <label id="lblDoc" runat="server" clientidmode="Static" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">
                                            &nbsp;
                                            </label>
                                            <dx:ASPxUploadControl ID="btnDocUploadStudii" runat="server" AutoStartUpload="true" BrowseButton-Text="Incarca Document" ClientIDMode="Static" ClientInstanceName="btnDocUploadStudii" FileUploadMode="OnPageLoad" Height="28px" OnFileUploadComplete="btnDocUploadStudii_FileUploadComplete" ShowProgressPanel="true" ShowTextBox="false" ToolTip="Incarca document" UploadMode="Advanced" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png">
                                                    </Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False">
                                                </ValidationSettings>
                                                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
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