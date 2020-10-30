<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="FormLista.aspx.cs" Inherits="WizOne.Posturi.FormLista" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode);
                    break;
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToDeleteMode(Value) {
            if (Value == 0 || Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata sau respinsa",
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
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=7&qwe=" + Value;
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


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" runat="server" Text="Respinge" OnClick="btnRespinge_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
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
                <dx:ASPxButton ID="btnNou"  runat="server" Text="Adauga" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpNou.Show(); pnlCtl.PerformCallback(cmbFormNou.GetValue()); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/new.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDuplica"  runat="server" Text="Duplica" OnClick="btnDuplica_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/duplicare.png"></Image>
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
            <td id="divRol" runat="server">
                <label id="lblForm" runat="server" style="display:inline-block;">Formular</label>
                <dx:ASPxComboBox ID="cmbForm" ClientInstanceName="cmbForm" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
            </td>

            <td runat="server">
                <label id="lblDataInc" runat="server" style="display:inline-block;">Data inceput</label>
				<dx:ASPxDateEdit  ID="dtDataInceput" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                        <CalendarProperties FirstDayOfWeek="Monday" />                                                   
				</dx:ASPxDateEdit>  
            </td>
            <td runat="server">
                <label id="lblDataSf" runat="server" style="display:inline-block;">Data sfarsit</label>
				<dx:ASPxDateEdit  ID="dtDataSfarsit" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                        <CalendarProperties FirstDayOfWeek="Monday" />                                                   
				</dx:ASPxDateEdit>
            </td>
            <td align="left">
                <label id="lblNivel" runat="server" style="display:inline-block;">Nivel</label>
                <dx:ASPxComboBox ID="cmbNivel" runat="server" ClientInstanceName="cmbNivel" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >                           
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
            <td align="left">
                <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left">
                <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                </dx:ASPxButton>
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>                     
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca" ReadOnly="true" Width="50px" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="50px" />
                        <dx:GridViewDataTextColumn FieldName="DescFormular" Name="DescFormular" Caption="Formular"  Width="200px" />
                        <dx:GridViewDataTextColumn FieldName="DescPost" Name="DescPost" Caption="Post"  Width="100px" />
			            <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Angajat"  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="DescRol" Name="DescRol" Caption="Rol"  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="Locatie" Name="Locatie" Caption="Locatie"  Width="100px" />
					    <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data vigoare"  Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare"  Width="150px"  >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare" ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdFormular" Name="IdFormular" Caption="IdFormular" ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdPost" Name="IdPost" Caption="IdPost" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdCircuit" Name="IdCircuit" Caption="IdCircuit" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdRol" Name="IdRol" Caption="IdRol" ReadOnly="true" Width="50px" Visible="false"  ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie" ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="UserIntrod" Name="UserIntrod" Caption="UserIntrod" ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false"/>
                        <dx:GridViewDataTextColumn FieldName="TotalCircuit" Name="TotalCircuit" Caption="TotalCircuit" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="PoateModifica" Name="PoateModifica" Caption="PoateModifica" ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false"/>
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
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>
    
    <dx:ASPxPopupControl ID="popUpNou" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpNouArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Selectare tip formular"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpNou" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">

                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server"  OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { pnlLoading.Hide(); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <asp:Panel ID="Panel1" runat="server">
                                <table style="width:100%;">
                                    <tr>
                                        <td align="right">
                                            <dx:ASPxButton ID="btnSave" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSalvare_Click" >
                                                <ClientSideEvents Click="function(s, e) { popUpNou.Hide(); e.processOnServer = true; }" />
                                                <Image Url="~/Fisiere/Imagini/Icoane/save.png"></Image>
                                            </dx:ASPxButton>
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:100%;">
                                            <label id="lblDataVig" runat="server"  >Data vigoare</label>
                                            <br />
				                            <dx:ASPxDateEdit  ID="deDataVig" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" ClientInstanceName="deDataVig"  AutoPostBack="false" CssClass="aspxComboBox_center" >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
				                            </dx:ASPxDateEdit>
                                            <br />
                                            <label id="lblFormNou" runat="server" >Formular</label>
                                            <br />
                                            <dx:ASPxComboBox ID="cmbFormNou" runat="server" ClientIDMode="Static" ClientInstanceName="cmbFormNou" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="Denumire" AutoPostBack="false"  AllowNull="true" CssClass="aspxComboBox_center">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(s.GetValue()); }" />
                                            </dx:ASPxComboBox>
                                            <br />
                                            <label id="lblRecrut" runat="server"  >Cerere recrutare</label>
                                            <br />
                                            <dx:ASPxComboBox ID="cmbRecrut" runat="server" ClientIDMode="Static"  ClientInstanceName="cmbRecrut" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="PostDenumire" AutoPostBack="false"  AllowNull="true" CssClass="aspxComboBox_center">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="50" />
                                                    <dx:ListBoxColumn FieldName="PostDenumire" Caption="Post" Width="200" />
                                                    <dx:ListBoxColumn FieldName="Companie" Caption="Companie" Width="100" />
                                                    <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="100" />
                                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Directie" Width="100" />
                                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Dept" Width="100" />
                                                    <dx:ListBoxColumn FieldName="Departament" Caption="Birou" Width="100" />
                                                    <dx:ListBoxColumn FieldName="Solicitant" Caption="Solicitant" Width="150" />
                                                    <dx:ListBoxColumn FieldName="Total" Caption="Total" Width="50" />
                                                    <dx:ListBoxColumn FieldName="Finalizate" Caption="Finalizate" Width="50" />
                                                    <dx:ListBoxColumn FieldName="InCurs" Caption="InCurs" Width="50" />
                                                    <dx:ListBoxColumn FieldName="Ramase" Caption="Ramase" Width="50" />		
                                                </Columns>
                                            </dx:ASPxComboBox>
                                            <br />
                                            <label id="lblAng" runat="server" >Angajat</label>
                                            <br />
                                            <dx:ASPxComboBox ID="cmbAng" runat="server"  ClientIDMode="Static" ClientInstanceName="cmbAng" Width="200px" DropDownWidth="350px" ValueField="F10003" TextField="NumeComplet" AutoPostBack="false"  AllowNull="true" CssClass="aspxComboBox_center">
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="NumeComplet" Caption="Nume complet" Width="200" />
                                                    <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="90" />
                                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Dept" Width="90" />
                                                    <dx:ListBoxColumn FieldName="DataFunctie" Caption="Data preluare fct." Width="90" />                                                    
                                                </Columns>
                                            </dx:ASPxComboBox>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>
