<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PlanificareSchimburi.aspx.cs" Inherits="WizOne.Pontaj.PlanificareSchimburi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">       
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Export" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpExport.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/ExportToXls.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="False" oncontextMenu="ctx(this,event)">                                
                    <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="width:100%;">
                <br /><br />
                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { pnlLoading.Hide(); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <dx:ASPxRoundPanel ID="pnlFiltrare" ClientInstanceName="pnlFiltrare" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Setare filtru de selectie" CssClass="pnlAlign indentright20" Width="100%">
                                <HeaderStyle Font-Bold="true" />
                                <ClientSideEvents CollapsedChanged="function (s,e) { AdjustSize(); }"  />
                                <PanelCollection>
                                    <dx:PanelContent>

                                        <div class="row">
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divDtInc" runat="server">
                                                <label id="lblDtInc" runat="server" oncontextMenu="ctx(this,event)">Data Inceput</label><br />
                                                <dx:ASPxDateEdit ID="txtDtInc" ClientInstanceName="txtDtInc" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                                    <ClientSideEvents ValueChanged="function(s,e) { VerificaInterval(s,e); }" />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divDtSf" runat="server">
                                                <label id="lblDtSf" runat="server" oncontextMenu="ctx(this,event)">Data Sfarsit</label><br />
                                                <dx:ASPxDateEdit ID="txtDtSf" ClientInstanceName="txtDtSf" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                                    <ClientSideEvents ValueChanged="function(s,e) { VerificaInterval(s,e); }" />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divAng" runat="server">
                                                <label id="lblAng" runat="server" oncontextMenu="ctx(this,event)">Angajat</label><br />
                                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" SelectInputTextOnClick="true"
                                                            CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                                    <Columns>
                                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                                    </Columns>
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divCtr" runat="server">
                                                <label id="lblCtr" runat="server" oncontextMenu="ctx(this,event)">Contract</label><br />

                                                <dx:ASPxDropDownEdit ClientInstanceName="cmbCtr" ID="cmbCtr" Width="250px" runat="server" AnimationType="None">
                                                    <DropDownWindowStyle BackColor="#EDEDED" />
                                                    <DropDownWindowTemplate>
                                                        <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox1" SelectionMode="CheckColumn" runat="server" TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                                            <Border BorderStyle="None" />
                                                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged1(); }" />
                                                        </dx:ASPxListBox>
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td style="padding: 4px">
                                                                    <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                                        <ClientSideEvents Click="function(s, e){ checkComboBox1.HideDropDown(); }" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </DropDownWindowTemplate>
                                                    <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues1(); }" DropDown="function(s, e){ SynchronizeListBoxValues1(); }" />
                                                </dx:ASPxDropDownEdit>

                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSub" runat="server">
                                                <label id="lblSub" runat="server" oncontextMenu="ctx(this,event)">Subcomp.</label><br />
                                                <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divFil" runat="server">
                                                <label id="lblFil" runat="server" oncontextMenu="ctx(this,event)">Filiala</label><br />
                                                <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSec" runat="server">
                                                <label id="lblSec" runat="server" oncontextMenu="ctx(this,event)">Sectie</label><br />
                                                <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divDept" runat="server">
                                                <label id="lblDept" runat="server" oncontextMenu="ctx(this,event)">Dept.</label><br />
                                                <dx:ASPxDropDownEdit ClientInstanceName="cmbDept" ID="cmbDept" Width="250px" runat="server" AnimationType="None">
                                                    <DropDownWindowStyle BackColor="#EDEDED" />
                                                    <DropDownWindowTemplate>
                                                        <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox2" SelectionMode="CheckColumn" runat="server" ValueField="IdDept" TextField="Dept" ValueType="System.Int32">
                                                            <Border BorderStyle="None" />
                                                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged2(); }" />
                                                        </dx:ASPxListBox>
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td style="padding: 4px">
                                                                    <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                                        <ClientSideEvents Click="function(s, e){ checkComboBox2.HideDropDown(); }" />
                                                                    </dx:ASPxButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </DropDownWindowTemplate>
                                                    <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues2(); }" DropDown="function(s, e){ SynchronizeListBoxValues2(); }" />
                                                </dx:ASPxDropDownEdit>
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divSubDept" runat="server">
                                                <label id="lblSubDept" runat="server" oncontextMenu="ctx(this,event)">Subdept.</label><br />
                                                <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="250px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divBirou" runat="server">
                                                <label id="lblBirou" runat="server" oncontextMenu="ctx(this,event)">Birou</label><br />
                                                <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="250px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div class="col-xl-2 col-lg-3 col-md-4 col-sm-6 col-xs-12" style="margin-bottom:8px;position: inherit" id="divCateg" runat="server">
                                                <label id="lblCateg" runat="server" oncontextMenu="ctx(this,event)">Categorie</label><br />
                                                <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-9 col-md-8 col-sm-6" style="margin-bottom:8px;"></div>
                                            <div class="col-lg-3 col-md-4 col-sm-6" style="margin-bottom:8px;">
                                                <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                                    <ClientSideEvents Click="function(s, e) { OnClickFiltru(s,e); }" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                                    <ClientSideEvents Click="function(s,e) { EmptyFields(); }"/>
                                                </dx:ASPxButton>
                                            </div>
                                        </div>


                                    </dx:PanelContent>
                                </PanelCollection>
                            </dx:ASPxRoundPanel>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCustomCallback="grDate_CustomCallback">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" AutoFilterCondition="Contains" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false"  />
                    <ClientSideEvents ContextMenu="ctx" 
                        Init="function(s,e) { OnGridInit(); }" 
                        EndCallback="function(s,e) { OnGridEndCallback(s); }"
                        />                    
                    <Columns>
                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="AngajatNume" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="250px" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="Contract" Caption="Contract" ReadOnly="true" FixedStyle="Left" VisibleIndex="4" Width="250px" />
                        <dx:GridViewDataComboBoxColumn FieldName="Ziua1" Name="Ziua1" Caption="Ziua1" Width="250px" VisibleIndex="5" UnboundType="Integer" ReadOnly="false" >
                            <PropertiesComboBox TextField="Denumire" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Ziua2" Name="Ziua2" Caption="Ziua2" Width="250px" VisibleIndex="6" UnboundType="Integer" ReadOnly="false" >
                            <PropertiesComboBox TextField="Denumire" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Ziua3" Name="Ziua3" Caption="Ziua3" Width="250px" VisibleIndex="7" UnboundType="Integer" ReadOnly="false" >
                            <PropertiesComboBox TextField="Denumire" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>                        
                        <dx:GridViewDataTextColumn FieldName="ZileGri" Caption="ZileGri" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                    </Columns>
                </dx:ASPxGridView>
                <br />
            </td>
        </tr>
    </table>

    <script>
        function EmptyFields() {
            cmbAng.SetValue(null);
            cmbCtr.SetValue(null);

            cmbSub.SetValue(null);
            cmbSec.SetValue(null);
            cmbFil.SetValue(null);
            cmbDept.SetValue(null);
            cmbSubDept.SetValue(null);
            cmbBirou.SetValue(null);
            cmbCateg.SetValue(null);

            pnlCtl.PerformCallback('EmptyFields');
        }

        function OnGridInit() {
            window.addEventListener('resize', function () {
                AdjustSize();
            })

            AdjustSize();
        }

        function AdjustSize() {
            var dif = 200 + pnlFiltrare.GetHeight();

            if (typeof grDateTotaluri !== "undefined" && ASPxClientUtils.IsExists(grDateTotaluri))
                dif = dif + grDateTotaluri.GetHeight();

            if (<%=Session["PontajulAreCC"] %> == 1)
                dif = dif + (grCC.GetHeight() + 50);

            var height = Math.max(0, document.documentElement.clientHeight) - dif;
            grDate.SetHeight(height);
        }

        function OnGridEndCallback(s) {
            if (s.cpAlertMessage) {
                swal({
                    title: "",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }


        var textSeparator = ",";
        //first one
        function OnListBoxSelectionChanged1(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState1();
            UpdateText1();
        }
        function UpdateSelectAllItemState1() {
            IsAllSelected1() ? checkListBox1.SelectIndices([0]) : checkListBox1.UnselectIndices([0]);
        }
        function IsAllSelected1() {
            var selectedDataItemCount = checkListBox1.GetItemCount() - (checkListBox1.GetItem(0).selected ? 0 : 1);
            return checkListBox1.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText1() {
            var selectedItems = checkListBox1.GetSelectedItems();
            checkComboBox1.SetText(GetSelectedItemsText1(selectedItems));
        }
        function SynchronizeListBoxValues1(dropDown, args) {
            checkListBox1.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts1(texts);
            checkListBox1.SelectValues(values);
            UpdateSelectAllItemState1();
            UpdateText1(); // for remove non-existing texts
        }
        function GetSelectedItemsText1(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts1(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox1.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }
        //second one
        function OnListBoxSelectionChanged2(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState2();
            UpdateText2();

            if (cmbSec.GetValue() != null)
                pnlCtl.PerformCallback('cmbDept');
        }
        function UpdateSelectAllItemState2() {
            IsAllSelected2() ? checkListBox2.SelectIndices([0]) : checkListBox2.UnselectIndices([0]);
        }
        function IsAllSelected2() {
            var selectedDataItemCount = checkListBox2.GetItemCount() - (checkListBox2.GetItem(0).selected ? 0 : 1);
            return checkListBox2.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText2() {
            var selectedItems = checkListBox2.GetSelectedItems();
            checkComboBox2.SetText(GetSelectedItemsText2(selectedItems));
        }
        function SynchronizeListBoxValues2(dropDown, args) {
            checkListBox2.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts2(texts);
            checkListBox2.SelectValues(values);
            UpdateSelectAllItemState2();
            UpdateText2(); // for remove non-existing texts
        }
        function GetSelectedItemsText2(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetValuesByTexts2(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox2.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }

        function OnFocusedCellChanging(s, e) {
            if (e.cellInfo.column.fieldName.indexOf("Ziua") < 0) {
                e.cancel = true;
                return;
            }

            var val = s.batchEditApi.GetCellValue(e.cellInfo.rowVisibleIndex, e.cellInfo.column.fieldName);
            var ctr = s.batchEditApi.GetCellValue(e.cellInfo.rowVisibleIndex, "Contract");
            
            if (val < 0)
                e.cancel = true;
            else {
                //LoadPrograme(s,e.cellInfo.column.fieldName);
                var indice = e.cellInfo.column.fieldName.replace("Ziua", "");
                var key = Number(indice) - 1;
                let programe = <%= Session["Json_Programe"] %>;
                var arr = programe.filter(function (item) { return item.IdAuto > 0 && item.ZiSapt == grDate.cp_ZiSapt[key] && item.Contract == ctr });

                var cmb = s.GetEditor(e.cellInfo.column.fieldName);
                cmb.ClearItems();

                for (var i = 0; i < arr.length; i++) {
                    cmb.AddItem(arr[i].Denumire, Number(arr[i].IdAuto));
                }
            }
        }

<%--        function LoadPrograme(s,fieldName) {
            var indice = fieldName.replace("Ziua", "");
            var key = Number(indice) - 1;
            let programe = <%= Session["Json_Programe"] %>;
            var arr = programe.filter(function (item) { return item.IdAuto > 0 && item.ZiSapt == grDate.cp_ZiSapt[key] });

            var cmb = s.GetEditor(fieldName);
            cmb.ClearItems();

            for (var i = 0; i < arr.length; i++) {
                cmb.AddItem(arr[i].Denumire, Number(arr[i].IdAuto));
            }
        }--%>

        function VerificaInterval(s, e) {
            var tmpInc = new Date(txtDtInc.GetDate());
            var tmpSf = new Date(txtDtSf.GetDate());
            var dtInc = new Date(tmpInc.getFullYear(), tmpInc.getMonth(), tmpInc.getDate(), 0, 0, 0, 0);
            var dtSf = new Date(tmpSf.getFullYear(), tmpSf.getMonth(), tmpSf.getDate(), 0, 0, 0, 0);
            var dif = (dtSf - dtInc) / (1000 * 60 * 60 * 24);
            if (dif > 30) {
                txtDtSf.SetValue(txtDtInc.GetValue());
                swal({
                    title: "Atentie",
                    text: "Diferenta intre data inceput si cea de sfarsit trebuie sa fie mai mica de 31 zile",
                    type: "warning"
                });
            }
        }

        function OnClickFiltru(s, e) {
            if (txtDtInc.GetDate() > txtDtSf.GetDate()) {
                txtDtSf.SetValue(txtDtInc.GetValue());
                swal({
                    title: "Atentie",
                    text: "Data de sfarsit este mai mica decat data de inceput",
                    type: "warning"
                });
            }
            else
                grDate.PerformCallback('btnFiltru');
        }

    </script>
</asp:Content>
