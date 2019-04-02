<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Aprobare.aspx.cs" Inherits="WizOne.BP.Aprobare" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnCerere":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCerereMode);
                    break;
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;
            }
        }


        function GoToDeleteMode(Value) {

            if (Value == 0 || Value == -1) {
                swal({
                    title: trad_string(limba, 'Operatie nepermisa'), text: trad_string(limba, 'Nu puteti anula o cerere deja anulata sau respinsa'),
                    type: "warning"
                });
            }
            else {
                swal({
                    title: trad_string(limba, "Sunteti sigur/a ?"), text: trad_string(limba, "Cererea va fi anulata !"),
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: trad_string(limba, "Da, continua!"), cancelButtonText: trad_string(limba, "Renunta"), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=4&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=2&id=' + Value, '_blank ')
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de respingere ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge; ");
                    }
                });
            }
            else
            {
                swal({
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de aprobare ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba;6");
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function EmptyFields(s, e) {
            cmbAng.SetValue(null);
            cmbAnul.SetValue(null);
            cmbLuna.SetValue(null);
            checkComboBoxStare.SetValue(null);
            cmbNivel.SetValue(null);
            cmbUser.SetValue(null);
            //pnlCtl.PerformCallback('EmptyFields');
        }


        var textSeparator = ";";
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
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        OnAproba(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="Absente_divOuter" style="margin:15px 0px;">				

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" meta:resourcekey="cmbAngResource1" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" meta:resourcekey="ListBoxColumnResource1" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" meta:resourcekey="ListBoxColumnResource2" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" meta:resourcekey="ListBoxColumnResource3" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" meta:resourcekey="ListBoxColumnResource4" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" meta:resourcekey="ListBoxColumnResource5" />
                                <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" meta:resourcekey="ListBoxColumnResource6" />
                            </Columns>
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { window.setTimeout(DelayedCallback('COUNTRY'), 1000); }" />
                        </dx:ASPxComboBox>
                    </div>


					<div class="Absente_Cereri_CampuriSup">
						<label id="lblAnul" runat="server" style="display:inline-block;">Anul</label> 
                        <dx:ASPxComboBox ID="cmbAnul" ClientInstanceName="cmbAnul" ClientIDMode="Static" runat="server" ValueField="Id" TextField="Denumire" Width="100px" AutoPostBack="true" OnSelectedIndexChanged="cmbAnul_SelectedIndexChanged" />
                    </div>

					<div class="Absente_Cereri_CampuriSup">
						<label id="lblLuna" runat="server" style="display:inline-block;">Luna</label>
                        <dx:ASPxComboBox ID="cmbLuna" ClientInstanceName="cmbLuna" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="true"  OnSelectedIndexChanged="cmbLuna_SelectedIndexChanged" />
                    </div>

					<div class="Absente_Cereri_CampuriSup">
                        <label id="lblStare" runat="server" style="display:inline-block;">Stare</label>	
                        <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare" ID="checkComboBoxStare" Width="210px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" Height="150"
                                    runat="server">
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
                    </div>


					
					<div class="Absente_Cereri_CampuriSup">
						<label id="lblNivel" runat="server" style="display:inline-block;">Nivel aprobare</label>
                        <dx:ASPxComboBox ID="cmbNivel" ClientInstanceName="cmbNivel" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>		

					<div class="Absente_Cereri_CampuriSup">
						<label id="lblUser" runat="server" style="display:inline-block;">Utilizator</label>
                        <dx:ASPxComboBox ID="cmbUser" ClientInstanceName="cmbUser" ClientIDMode="Static" runat="server" Width="150px" ValueField="F70102" TextField="F70104" ValueType="System.Int32" AutoPostBack="false" />
                    </div>	
										
					<div style="float:left; padding:30px 15px 0px 15px;">
						<dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
							<Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
							<ClientSideEvents Click="function(s, e) {
											pnlLoading.Show();
											e.processOnServer = true;
										}" />
						</dx:ASPxButton>
					</div>

					<div style="float:left; padding:30px 15px 0px 15px;">
						<dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
							<Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
							<ClientSideEvents Click="EmptyFields" />
						</dx:ASPxButton>
					</div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnRowUpdating="grDate_RowUpdating" OnCustomCallback="grDate_CustomCallback" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="True" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>

                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="50px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

						<dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="70px" VisibleIndex="2"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px" VisibleIndex="3" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataTextColumn FieldName="An" Name="An" Caption="An" ReadOnly="true" Width="75px" VisibleIndex="4" />
                         <dx:GridViewDataTextColumn FieldName="Luna" Name="Luna" Caption="Luna" ReadOnly="true" Width="75px"  Visible="false"/>
						<dx:GridViewDataTextColumn FieldName="LunaDesc" Name="Luna" Caption="Luna" ReadOnly="true" Width="100px" VisibleIndex="5" />
						<dx:GridViewDataTextColumn FieldName="Angajat" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="200px" VisibleIndex="6" Settings-AutoFilterCondition="Contains" />
						<dx:GridViewDataTextColumn FieldName="SumaNeta" Name="SumaNeta" Caption="Suma" ReadOnly="true" Width="100px" VisibleIndex="7" />
						<dx:GridViewDataTextColumn FieldName="Moneda" Name="Moneda" Caption="Moneda" ReadOnly="true" Width="100px" VisibleIndex="8" />
						<dx:GridViewDataTextColumn FieldName="Curs" Name="Curs" Caption="Curs" ReadOnly="true" Width="100px" VisibleIndex="9" />
						<dx:GridViewDataTextColumn FieldName="TotalNet" Name="TotalNet" Caption="Total" ReadOnly="true" Width="100px" VisibleIndex="10" />
						<dx:GridViewDataTextColumn FieldName="Tip" Name="Tip" Caption="Tip prima" ReadOnly="true" Width="100px" VisibleIndex="11" />
                        <dx:GridViewDataTextColumn FieldName="Avans" Name="Avans" Caption="Avans/Lichidare" ReadOnly="true" Width="100px" VisibleIndex="12" />	
                        <dx:GridViewDataComboBoxColumn FieldName="USER_NO" Name="USER_NO" Caption="Utilizator" ReadOnly="true" Width="150px" VisibleIndex="13" >
                            <PropertiesComboBox TextField="F70104" ValueField="F70102" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Explicatie" Name="Explicatie" Caption="Explicatie" ReadOnly="true" Width="300px" VisibleIndex="14" />	
                    </Columns>                                     
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>

    

</asp:Content>
