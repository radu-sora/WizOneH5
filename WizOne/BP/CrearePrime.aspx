<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CrearePrime.aspx.cs" Inherits="WizOne.BP.CrearePrime" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        var limba = "<%= Session["IdLimba"] %>";
        function OnValueChangedHandler(s) {
            pnlCtl.PerformCallback(2 + ";" + s.name + ";" + s.GetValue());
        }

        function OnTextChangedHandler(s) {
            pnlCtl.PerformCallback(2 + ";" + s.name + ";" + s.GetText());
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        pnlCtl.PerformCallback(4);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" />
        <PanelCollection>
            <dx:PanelContent>

            <table width="40%">
                <tr>
                    <td>
                        <dx:ASPxLabel  ID="lblAn" runat="server"  style="display:inline-block;"  Text="An"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbAn" runat="server" ClientInstanceName="cmbAn" ClientIDMode="Static" Width="70px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblLuna" runat="server"  style="display:inline-block;"  Text="Luna"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbLuna" runat="server" ClientInstanceName="cmbLuna" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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
                    <td>
                        <dx:ASPxLabel  ID="lblTip" runat="server"  style="display:inline-block;" Text="Tip"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbTip" runat="server" ClientInstanceName="cmbTip" ClientIDMode="Static" Width="125px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblAvsLch" runat="server"  style="display:inline-block;"  Text="Avans/Lichidare"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbAvsLch" runat="server" ClientInstanceName="cmbAvsLch" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td> 
                    <td>
                        <dx:ASPxLabel  ID="lblMoneda" runat="server"  style="display:inline-block;"  Text="Moneda"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbMoneda" runat="server" ClientInstanceName="cmbMoneda" ClientIDMode="Static" Width="70px" ValueField="Id" DropDownWidth="100" 
                            TextField="Abreviere" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>                    
                    <td>
                        <dx:ASPxLabel  ID="lblCurs" runat="server"  style="display:inline-block;"  Text="Curs"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txtCurs" runat="server" Width="70px" >
                        </dx:ASPxTextBox>
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblSumaNeta" runat="server"  style="display:inline-block;"  Text="Suma"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txtSumaNeta" runat="server" Width="70px" >
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox> 
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblRONNet" runat="server"   style="display:inline-block;" Text="Total"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txtRONNet" runat="server"  ReadOnly="true" Width="70px" >
                        </dx:ASPxTextBox> 
                    </td> 
                    <td>
                        <label id="lblExpl" runat="server" style="display:inline-block;">Explicatii</label>
                        <dx:ASPxMemo ID="txtExpl" runat="server" Width="350px"  Height="60" ></dx:ASPxMemo>
                    </td>
               </tr>


            </table>
            <p></p>
            <p></p>
            <p></p>
            <p></p>
            <table width="40%">
                <tr>

                    <td>
                        <label id="lblAngFiltru" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAngFiltru" ClientInstanceName="cmbAngFiltru" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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
                    <td>
                        <dx:ASPxLabel  ID="lblAnFil" runat="server"  style="display:inline-block;"  Text="An"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbAnFil" runat="server" ClientInstanceName="cmbAnFil" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblLunaFil" runat="server"  style="display:inline-block;"  Text="Luna"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmbLunaFil" runat="server" ClientInstanceName="cmbLunaFil" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>                   								

					<td>		
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
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(5);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(6);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        </dx:ASPxButton>
                    </td>                    	
                </tr>
            </table>
            <table>
                <tr>
                    <td >
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"  OnCustomCallback="grDate_CustomCallback">
                            <SettingsBehavior AllowFocusedRow="true"  />
                            <Settings ShowFilterRow="False" ShowColumnHeaders="true" />  
                            <SettingsEditing Mode="Inline" />  
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
						        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Visible="false" Width="70px" VisibleIndex="2"/>
                                <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="150px" VisibleIndex="3" >
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                    <Settings FilterMode="DisplayText" />
                                </dx:GridViewDataComboBoxColumn>
						        <dx:GridViewDataTextColumn FieldName="An" Name="An" Caption="An" ReadOnly="true" Width="75px" VisibleIndex="4" />
						        <dx:GridViewDataTextColumn FieldName="LunaDesc" Name="Luna" Caption="Luna" ReadOnly="true" Width="100px" VisibleIndex="5" />
						        <dx:GridViewDataTextColumn FieldName="Angajat" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="300px" VisibleIndex="6" Settings-AutoFilterCondition="Contains" />
						        <dx:GridViewDataTextColumn FieldName="SumaNeta" Name="SumaNeta" Caption="Suma" ReadOnly="true" Width="100px" VisibleIndex="7" />
						        <dx:GridViewDataTextColumn FieldName="Moneda" Name="Moneda" Caption="Moneda" ReadOnly="true" Width="100px" VisibleIndex="8" />
						        <dx:GridViewDataTextColumn FieldName="Curs" Name="Curs" Caption="Curs" ReadOnly="true" Width="100px" VisibleIndex="9" />
						        <dx:GridViewDataTextColumn FieldName="TotalNet" Name="TotalNet" Caption="Total" ReadOnly="true" Width="100px" VisibleIndex="10" />
						        <dx:GridViewDataTextColumn FieldName="Tip" Name="Tip" Caption="Tip prima" ReadOnly="true" Width="100px" VisibleIndex="11" />					
                                <dx:GridViewDataTextColumn FieldName="Avans" Name="Avans" Caption="Avans/Lichidare" ReadOnly="true" Width="100px" VisibleIndex="12" />	
                                <dx:GridViewDataTextColumn FieldName="Explicatie" Name="Explicatie" Caption="Explicatie" ReadOnly="true" Width="300px" VisibleIndex="13" />	
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
