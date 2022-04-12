<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Aprobare.aspx.cs" Inherits="WizOne.Beneficii.Aprobare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'IdBeneficiu', GoToAtasMode);
                    break;
                case "btnArataAng":
                    grDate.GetRowValues(e.visibleIndex, 'F10003', GoToAtasAngMode);
                    break;
            }
        }



        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=19&id=' + Value, '_blank ')
        }

        function GoToAtasAngMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=21&id=' + Value, '_blank ')
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


        function OnRespinge(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de respingere ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        popUpMotiv.Show();                     
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnMotivRespingere(s, e) {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '') {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
                    type: "warning"
                });
            }
            else {
                popUpMotiv.Hide();
                grDate.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
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


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" ClientInstanceName="btnRespinge" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        OnRespinge(s,e);                        
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" runat="server" Text="Aproba" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave"  runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br /> 
    <table width="60%">   
        <tr>
            <td align="left">
                <label id="lblAngFiltru" runat="server" visible="false" style="display:inline-block;">Angajat</label>
                <dx:ASPxComboBox ID="cmbAngFiltru"  visible="false" ClientInstanceName="cmbAngFiltru" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
                    </Columns>                            
                </dx:ASPxComboBox>
            </td>
            <td align="left">
                <label id="lblSesFiltru"  visible="false" runat="server" style="display:inline-block;">Sesiune</label>
                <dx:ASPxComboBox ID="cmbSesiuneFiltru"  visible="false" runat="server" ClientInstanceName="cmbSesiuneFiltru" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >                           
                </dx:ASPxComboBox>
            </td>                    								

			<td align="left">		
                <label id="lblStare" runat="server"  visible="false" style="display:inline-block;">Stare</label>	
                <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare"  visible="false" ID="checkComboBoxStare" Width="210px" runat="server" AnimationType="None">
                    <DropDownWindowStyle BackColor="#EDEDED" />
                    <DropDownWindowTemplate>
                        <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                            <Border BorderStyle="None" />
                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                            <Items>
                                <dx:ListEditItem Text="(Selectie toate)" />
                                <dx:ListEditItem Text="Neselectat" Value="1" />
                                <dx:ListEditItem Text="Selectat" Value="2" />
                                <dx:ListEditItem Text="Aprobat" Value="3" />
                                <dx:ListEditItem Text="Respins" Value="4" />
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
            <td>
                <label id="lblDeLa" runat="server"  visible="false" style="display:inline-block;">Data inceput</label>
                <dx:ASPxDateEdit ID="txtDataInc"  visible="false" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>
            </td>
            <td>
                <label id="lblLa" runat="server"  visible="false"  style="display:inline-block;">Data sfarsit</label>
                <dx:ASPxDateEdit ID="txtDataSf"  visible="false" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>                    
            </td> 

            <td align="left">
                <label id="lblF" runat="server"  visible="false" style="display:inline-block;"></label>
                <dx:ASPxButton ID="btnFiltru"  visible="false" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" Text="Filtru" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left">
                <label id="lblSF" runat="server"  visible="false" style="display:inline-block;"></label>
                <dx:ASPxButton ID="btnFiltruSterge"  visible="false" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" Text="Sterge filtru" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                </dx:ASPxButton>
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="30px"  ButtonType="Image" ShowEditButton="false" Visible="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons> 
                                <dx:GridViewCommandColumnCustomButton ID="btnArataAng">
                                    <Image ToolTip="Arata document angajat" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                                   
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                         <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" ReadOnly="true" Width="150px" >
                            <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdSesiune" Name="IdSesiune" Caption="Sesiune" ReadOnly="true" Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataComboBoxColumn FieldName="IdBeneficiu" Name="IdBeneficiu" Caption="Beneficiu"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" ReadOnly="true"  Width="150px" >	
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
					    <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput sesiune" ReadOnly="true" HeaderStyle-Wrap="True"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataDateColumn>
					    <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit sesiune" ReadOnly="true"  HeaderStyle-Wrap="True" Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn>	
                       
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false"  ShowInCustomizationForm="false"/>
                    </Columns>                  
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table> 


    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnMotivRespingere(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtv" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtv" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>
