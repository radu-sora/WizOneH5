<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="CursuriInregistrare.aspx.cs" Inherits="WizOne.Curs.CursuriInregistrare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToArataMode);
                    break;
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnCurs":
                    popUpCurs.Show();
                    pnlCtlCurs.PerformCallback();
                    break;
                case "btnSesiune":
                    popUpSesiune.Show();
                    pnlCtlSesiune.PerformCallback();
                    break;
            }
        }

        function GoToArataMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=15&id=' + Value, '_blank ');
        }

        function GoToDeleteMode(Value) {
            if (Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Cererea a fost deja anulata!",
                    type: "warning"
                });
            }
            else {
                swal({
                    title: "Sunteti sigur/a ?", text: "Cererea va fi anulata !",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, anuleaza!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=8&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }   


        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

     


        var textSeparator = ",";
        function OnListBoxSelectionChanged(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState();
            UpdateText();
        }
        function UpdateSelectAllItemState() {
            IsAllSelected() ? checkListBox.SelectIndices([0]) : checkListBox.UnselectIndices([0]);
        }
        function IsAllSelected() {
            var selectedDataItemCount = checkListBox.GetItemCount() - (checkListBox.GetItem(0).selected ? 0 : 1);
            return checkListBox.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText() {
            var selectedItems = checkListBox.GetSelectedItems();
            checkComboBoxStare.SetText(GetSelectedItemsText(selectedItems));
        }
        function SynchronizeListBoxValues(dropDown, args) {
            checkListBox.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts(texts);
            checkListBox.SelectValues(values);
            UpdateSelectAllItemState();
            UpdateText();
        }
        function GetSelectedItemsText(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }

        function EndUpload(s) {
            //lblDoc.innerText = s.cpDocUploadName;
            //s.cpDocUploadName = null;
            pnlLoading.Hide();
        }

        function StartUpload() {
            pnlLoading.Show();
        }


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 

    <br /> 

    <table width="40%">
        <tr>
            <td>
                <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" 
                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource3" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource6" />
                    </Columns>
                    <ClientSideEvents SelectedIndexChanged="function(s, e) {
                         cmbCurs.PerformCallback(s.GetValue()); }" />
                </dx:ASPxComboBox>
            </td>
            <td colspan="2">
                <label id="lblCurs" runat="server" style="display:inline-block;">Curs</label>
                <dx:ASPxComboBox ID="cmbCurs" runat="server" ClientInstanceName="cmbCurs" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbCurs_Callback"  >
                    <ClientSideEvents SelectedIndexChanged="function(s, e) {
                         cmbSesiune.PerformCallback(s.GetValue()); }" />
                </dx:ASPxComboBox>
            </td>
            <td colspan="2">
                <label id="lblSesiune" runat="server" style="display:inline-block;">Sesiune</label>
                <dx:ASPxComboBox ID="cmbSesiune" runat="server" ClientInstanceName="cmbSesiune" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  OnCallback="cmbSesiune_Callback" >
                </dx:ASPxComboBox>
            </td>
            </tr>
            <tr>
                <td>
                    <label id="lblObs" runat="server">Observatii</label>
                    <dx:ASPxMemo ID="txtObs" runat="server"  Width="250px" Height="100px" ></dx:ASPxMemo>
                </td>
                <td style="padding-right:10px;" >
                    <dx:ASPxUploadControl ID="btnDoc" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                        BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                        ClientInstanceName="UploadImage" OnFileUploadComplete="btnDoc_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocResource1">
                        <BrowseButton>
                            <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                        </BrowseButton>
                        <ValidationSettings ShowErrors="False"></ValidationSettings>

                        <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                    </dx:ASPxUploadControl>
                </td>   
                <td style="padding-right:10px;" >
                    <dx:ASPxButton ID="btnDocSterge" runat="server" ToolTip="sterge document" AutoPostBack="false" Height="28px" meta:resourcekey="btnDocStergeResource1">
                        <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                        <ClientSideEvents Click="function(s,e) { grDate.PerformCallback('btnDocSterge'); }" />
                    </dx:ASPxButton>
             
                    <dx:ASPxButton ID="btnArataDoc" runat="server" ToolTip="arata document" AutoPostBack="false" Height="28px" meta:resourcekey="btnArataDocResource1">
                        <Image Url="../Fisiere/Imagini/Icoane/arata.png" Width="16px" Height="16px"></Image>
                        <Paddings PaddingLeft="0px" PaddingRight="0px" />
                        <ClientSideEvents Click="function(s,e) { window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=16&id=-99', '_blank '); }" />
                    </dx:ASPxButton>             

                </td>
                <td>
                    <dx:ASPxCheckBox ID="chkListaAsteptare"  runat="server" Width="150" Text="Lista asteptare"  TextAlign="Left"  ClientInstanceName="chkbxLA" >                                    
                    </dx:ASPxCheckBox>
                </td>
                <td>  
                    <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Height="28px"  AutoPostBack="true"  OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" meta:resourcekey="btnSaveResource1" >    
                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
    </table>


    <table width="58%">   
        <tr>
            <td align="left">
                <label id="lblAngFiltru" runat="server" style="display:inline-block;">Angajat</label>
                <dx:ASPxComboBox ID="cmbAngFiltru" ClientInstanceName="cmbAngFiltru" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                        CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="80px" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="200px" />
                        <dx:ListBoxColumn FieldName="Subcompanie" Caption="Divizie" Width="90px" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Directie" Width="90px" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Dept" Width="90px" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Birou" Width="90px" />
                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="90px" />
                    </Columns>
                </dx:ASPxComboBox>
            </td>

            <td align="left"  colspan="2">
                <label id="lblCursFiltru" runat="server" style="display:inline-block;">Curs</label>
                <dx:ASPxComboBox ID="cmbCursFiltru" runat="server" ClientInstanceName="cmbCursFiltru" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >    
                    <ClientSideEvents SelectedIndexChanged="function(s, e) {
                         cmbSesiuneFiltru.PerformCallback(s.GetValue()); }" />                    
                </dx:ASPxComboBox>
            </td>   
            <td align="left">
                <label id="lblSesiuneFiltru" runat="server" style="display:inline-block;">Sesiune</label>
                <dx:ASPxComboBox ID="cmbSesiuneFiltru" runat="server" ClientInstanceName="cmbSesiuneFiltru" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbSesiuneFiltru_Callback" >                           
                </dx:ASPxComboBox>
            </td>             

			<td align="left">		
                <label id="lblStare" runat="server" style="display:inline-block;">Stare</label>	
                <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare" ID="checkComboBoxStare" Width="210px" runat="server" AnimationType="None">
                    <DropDownWindowStyle BackColor="#EDEDED" />
                    <DropDownWindowTemplate>
                        <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                            <Border BorderStyle="None" />
                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                            <Items>
                                <dx:ListEditItem Text="(Selectie toate)" />
                                <dx:ListEditItem Text="Solicitat" Value="1" />
                                <dx:ListEditItem Text="In Curs" Value="2" />
                                <dx:ListEditItem Text="Aprobat" Value="3" />
                                <dx:ListEditItem Text="Respins" Value="4" />
                                <dx:ListEditItem Text="Anulat" Value="5" />
                            </Items>
                            <ClientSideEvents SelectedIndexChanged="OnListBoxSelectionChanged" />
                        </dx:ASPxListBox>
                        <table style="width: 100%">
                            <tr>
                                <td style="padding: 4px">
                                    <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Inchide" style="float: right">
                                        <ClientSideEvents Click="function(s, e){ checkComboBoxStare.HideDropDown(); }" />
                                    </dx:ASPxButton>
                                </td>
                            </tr>
                        </table>
                    </DropDownWindowTemplate>
                    <ClientSideEvents TextChanged="SynchronizeListBoxValues" DropDown="SynchronizeListBoxValues" />
                </dx:ASPxDropDownEdit>

            </td>
            <td align="bottom">
                <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="bottom">
                <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                </dx:ASPxButton>
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="70%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize"
                    OnRowUpdating="grDate_RowUpdating">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="false" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" Caption=" " ShowEditButton="true" Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>                                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajatul" ReadOnly="true" Width="200px">
                            <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdCurs" Name="IdCurs" Caption="Curs" ReadOnly="true" Width="200px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewCommandColumn Width="75px" ButtonType="Image" ShowEditButton="false"  Caption="Detalii curs">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnCurs" Visibility="BrowsableRow">
                                    <Image ToolTip="Curs" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdSesiune" Name="IdSesiune" Caption="Sesiune" ReadOnly="true" Width="200px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewCommandColumn Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Detalii sesiune">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnSesiune" Visibility="BrowsableRow">
                                    <Image ToolTip="Sesiune" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" ReadOnly="true" Width="200px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="eListaAsteptare" Name="eListaAsteptare" Caption="Lista Asteptare" ReadOnly="true" Width="110px">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataTextColumn>

                        <dx:GridViewDataCheckColumn FieldName="Certificat" Name="Certificat" Caption="Certificat"  Width="80px" Visible="false"  ShowInCustomizationForm="false"  />   
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false"  ShowInCustomizationForm="false"/>
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                    </SettingsCommandButton>  
                    <Templates>
                        <EditForm>
                            <div style="padding: 4px 3px 4px">
                                <table>                           
                                    <tr>
                                        <td style="padding:10px !important;" colspan="2">
                                            <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
                                            <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUpload" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
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

    <dx:ASPxPopupControl ID="popUpCurs" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpCursArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="200px" HeaderText="Detalii curs"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpCurs" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
           <dx:ASPxCallbackPanel ID = "pnlCtlCurs" ClientIDMode="Static" ClientInstanceName="pnlCtlCurs" runat="server" OnCallback="pnlCtlCurs_Callback" SettingsLoadingPanel-Enabled="false">          
              <PanelCollection>
                <dx:PanelContent>
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnCloseCurs" runat="server" Text="Inchide" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { popUpCurs.Hide(); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblDenumire" runat="server" ClientIDMode="Static" ClientInstanceName="lblDenumire" Width="100px" Text="Denumire" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtDenumire" runat="server" ClientIDMode="Static" ClientInstanceName="txtDenumire" Width="200px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblComen" runat="server" ClientIDMode="Static" ClientInstanceName="lblComen" Width="100px" Text="Comentarii" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtComen" runat="server" ClientIDMode="Static" ClientInstanceName="txtComen" Width="200px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                    </table>
                    </dx:PanelContent>
                  </PanelCollection>
                </dx:ASPxCallbackPanel>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxPopupControl ID="popUpSesiune" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpSesiuneArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Detalii sesiune"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpSesiune" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
               <dx:ASPxCallbackPanel ID = "pnlCtlSesiune" ClientIDMode="Static" ClientInstanceName="pnlCtlSesiune" runat="server" OnCallback="pnlCtlSesiune_Callback" SettingsLoadingPanel-Enabled="false">          
                  <PanelCollection>
                    <dx:PanelContent>
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnCloseSesiune" runat="server" Text="Inchide" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { popUpSesiune.Hide(); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblDenumireS" runat="server" ClientIDMode="Static" ClientInstanceName="lblDenumireS" Width="100px" Text="Denumire" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtDenumireS" runat="server" ClientIDMode="Static" ClientInstanceName="txtDenumireS" Width="200px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblDataInc" runat="server" ClientIDMode="Static" ClientInstanceName="lblDataInc" Width="100px" Text="Data inceput" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtDataInc" runat="server" ClientIDMode="Static" ClientInstanceName="txtDataInc" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblOraInc" runat="server" ClientIDMode="Static" ClientInstanceName="lblOraInc" Width="100px" Text="Ora inceput" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtOraInc" runat="server" ClientIDMode="Static" ClientInstanceName="txtOraInc" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblDataSf" runat="server" ClientIDMode="Static" ClientInstanceName="lblDataSf" Width="100px" Text="Data sfarsit" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtDataSf" runat="server" ClientIDMode="Static" ClientInstanceName="txtDatasf" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblOraSf" runat="server" ClientIDMode="Static" ClientInstanceName="lblOraSf" Width="100px" Text="Ora sfarsit" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtOraSf" runat="server" ClientIDMode="Static" ClientInstanceName="txtOraSf" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblTematica" runat="server" ClientIDMode="Static" ClientInstanceName="lblTematica" Width="100px" Text="Tematica" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtTematica" runat="server" ClientIDMode="Static" ClientInstanceName="txtTematica" Width="200px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblOrganizator" runat="server" ClientIDMode="Static" ClientInstanceName="lblOrganizator" Width="100px" Text="Organizator" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtOrganizator" runat="server" ClientIDMode="Static" ClientInstanceName="txtOrganizator" Width="200px" Enabled="false" ></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblTrainer" runat="server" ClientIDMode="Static" ClientInstanceName="lblTrainer" Width="100px" Text="Trainer" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtTrainer" runat="server" ClientIDMode="Static" ClientInstanceName="txtTrainer" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblLocatie" runat="server" ClientIDMode="Static" ClientInstanceName="lblLocatie" Width="100px" Text="Locatie" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtLocatie" runat="server" ClientIDMode="Static" ClientInstanceName="txtLocatie" Width="100px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxLabel ID="lblObservatii" runat="server" ClientIDMode="Static" ClientInstanceName="lblObservatii" Width="100px" Text="Observatii" ></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxTextBox ID="txtObservatii" runat="server" ClientIDMode="Static" ClientInstanceName="txtObservatii" Width="200px" Enabled="false"></dx:ASPxTextBox>
                            </td>
                        </tr>
                    </table>
                    </dx:PanelContent>
                  </PanelCollection>
                </dx:ASPxCallbackPanel>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>
