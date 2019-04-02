<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajOne.aspx.cs" Inherits="WizOne.Pontaj.PontajOne" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>
        var grIsEditing = false;

        function grid_FocusedRowChanged(s, e) {

            if (grIsEditing)
                s.UpdateEdit();

            grIsEditing = false;
        }

        function OnBatchEditStartEditing(s, e) {

            var keyIndex = s.GetColumnByField("Cheia").index;
            var key = e.rowValues[keyIndex].value;

            var col = e.focusedColumn.fieldName;

            if (col.length >= 6 && col.substr(0, 6) == 'ValTmp') {

                if (typeof s.cp_ValSec[key] != "undefined" && s.cp_ValSec[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
                    e.cancel = true;
                }
            }

            if (col.length >= 6 && col.substr(0, 6) == 'ValAbs')
            {

                var cmb = grDate.GetEditor('ValAbs');
                if (cmb) {
                    cmb.ClearItems();
                    if (typeof s.cp_ValAbsente[key] != "undefined") {
                        var str = s.cp_ValAbsente[key];
                        if (str) {
                            var arr = str.substring(1).split(",");
                            for (var i = 0; i < arr.length; i++) {
                                if (arr[i] != "undefined" && arr[i] != "") {
                                    var denum = arr[i].split("=");
                                    if (denum.length >= 1) {
                                        if (denum.length == 1)
                                            cmb.AddItem('', denum[0]);
                                        else {
                                            var fullDen = new Array(denum[0], denum[1]);
                                            cmb.AddItem(fullDen);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }

        function OnBatchEditEndEditing(s, e) {
            grIsEditing = true;

            var col = s.batchEditApi.GetEditCellInfo().column.fieldName;

            if (col.length >= 4) {
                switch (col.substr(0, 6)) {
                    case "ValTmp":
                        {
                            var column = s.batchEditApi.GetEditCellInfo().column;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValAbs", null, null, true);
                                s.GetRowValues(e.visibleIndex, "Afisare", function (Value) {
                                    OnEditMode(e, e.visibleIndex, Value);
                                });
                            }
                        }
                        break;
                    case "ValAbs":
                        {
                            var column = grDate.GetColumnByField("ValAbs");
                            if (!e.rowValues[column.index]) return;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValStr", newVal, null, true);

                                for (i = 0; i <= 20; i++) {
                                    grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValTmp" + i, null, null, true);
                                }
                            }
                        }
                        break;
                }
            }

        }

        function OnEditMode(e, idx, Value) {
            var valStr = "";

            for (i = 0; i <= 20; i++) {
                var val = 0;
                var valOre = 0;

                var column = grDate.GetColumnByField("ValTmp" + i);
                if (!e.rowValues[column.index]) continue;
                val = e.rowValues[column.index].value;

                if (val) {
                    var mm = val.getMinutes();
                    var hh = val.getHours();

                    if (mm != 0 || hh != 0) {
                        switch (Value) {
                            case 1:
                                valOre = hh
                                //valOre = Math.floor(val / 60);
                                break;
                            case 2:
                                valOre = hh + ':' + ("00" + mm).slice(-2);
                                //valOre = Math.floor(val / 60) + '.' + ("00" + (val % 60)).slice(-2);
                                break;
                            case 3:
                                valOre = hh + '.' + ("00" + Math.round((mm / 60) * 100)).slice(-2);;
                                //valOre = Math.round((val / 60) * 100) / 100;
                                break;
                            default:
                                valOre = hh
                                break;
                        }

                        var str = column.name;
                        var n = str.indexOf("_");
                        if (n >= 0)
                            valStr += "/" + valOre + column.name.substring(n+1);
                        else
                            valStr += "/" + valOre + column.name;
                    }
                }
            }

            if (valStr.length > 0) valStr = valStr.substring(1);

            grDate.batchEditApi.SetCellValue(idx, "ValStr", valStr, null, true);
        }

        function eventKeyPress(evt, s) {
            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();

            if (!s.IsEditing()) {
                var cell = grDate.GetFocusedCell();
                var col = cell.column.fieldName;
                if (col.length >= 3) {
                    switch (col.substr(0, 3)) {
                        case "In1":
                        case "In2":
                        case "In3":
                        case "In4":
                        case "In5":
                        case "In6":
                        case "In7":
                        case "In8":
                        case "In9":
                        case "Out":
                            {
                                if (key == 45)              // scade o zi
                                {
                                    grDate.PerformCallback('dayMinus;' + cell.column.fieldName);
                                }
                                else if (key == 43)        // adauga o zi
                                {
                                    grDate.PerformCallback('dayPlus;' + cell.column.fieldName);
                                }
                                else if (key == 93)         ////insereaza celula   tasta   ]
                                {
                                    grDate.PerformCallback('cellPlus;' + cell.column.fieldName);
                                }
                                else if (key == 91)         // sterge celula pe care este, daca este goala tasta [
                                {
                                    grDate.PerformCallback('cellMinus;' + cell.column.fieldName);
                                }
                            }
                    }
                }
            }
        }

        function EmptyFields(s, e) {
            cmbAng.SetValue(null);
            cmbCtr.SetValue(null);
            cmbStare.SetValue(null);

            cmbSub.SetValue(null);
            cmbSec.SetValue(null);
            cmbFil.SetValue(null);
            cmbDept.SetValue(null);
            cmbSubDept.SetValue(null);
            cmbBirou.SetValue(null);
        }

        function grCC_OnNewClick(s, e) {
            grCC.AddNewRow();
        }

        function grCC_OnCancelClick(s, e) {
            grCC.CancelEdit();
        }

        function OnInit(s, e) {
            popUpInit.Hide();
            pnlLoading.Show();
            e.processOnServer = true;
        }

    </script>


    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" Visible="false" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="true" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpInit.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        grDate.UpdateEdit();
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <div style="float:left; line-height:22px; vertical-align:middle;">
                    
                    <div style="float:left; padding-right:15px;">
                        <label id="lblAnLuna" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Luna/An</label>
                            <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>
                    
                    <div style="float:left; padding-right:15px; vertical-align:middle; display:none;">
                        <label id="lblRolAng" runat="server" style="float:left; padding-right:15px;">Roluri</label>
                        <dx:ASPxComboBox ID="cmbRolAng" ClientInstanceName="cmbRolAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </div>
                    
                    <div style="float:left; padding-right:15px; display:none;">
                        <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" OnButtonClick="cmbAng_ButtonClick" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                            </Columns>
                            <Buttons>
                                <dx:EditButton Position="Left">
                                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                </dx:EditButton>
                                <dx:EditButton Position="Right">
                                    <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                </dx:EditButton>
                            </Buttons>
                            <ClientSideEvents ButtonClick="function(s, e) {
                                                        pnlLoading.Show();
                                                        e.processOnServer = true;
                                                    }" />
                        </dx:ASPxComboBox>
                    </div>

                    <label ID="txtStare" runat="server" Width="110" style="float:left; margin-right:15px; width:110px; height:26px; text-align:center; border:solid 1px gray; color:#000000;"></label>
                </div>


                <div style="float:left; padding:0px 15px;">
                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        <ClientSideEvents Click="function(s, e) {
                                        pnlLoading.Show();
                                        e.processOnServer = true;
                                    }" />
                    </dx:ASPxButton>
                </div>

                <div style="float:left; display:none;">
                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        <ClientSideEvents Click="EmptyFields" />
                    </dx:ASPxButton>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlRowPrepared="grDate_HtmlRowPrepared" OnBatchUpdate="grDate_BatchUpdate" OnDataBound="grDate_DataBound" OnCellEditorInitialize="grDate_CellEditorInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" 
                        BatchEditEndEditing="OnBatchEditEndEditing" 
                        BatchEditStartEditing="OnBatchEditStartEditing"
                        FocusedRowChanged="grid_FocusedRowChanged"
                        RowDblClick="function(s, e) {
                        ccValori.Set('cheia',s.GetRowKey(s.GetFocusedRowIndex()));
                        if (grCC)
                            grCC.PerformCallback('btnCC');
                        }"   />
                    <Styles>
                        <BatchEditModifiedCell BackColor="Transparent">
                        </BatchEditModifiedCell>
                    </Styles>
                    <Columns>
                        
                        <dx:GridViewDataTextColumn FieldName="Cheia" Caption=" " ReadOnly="true" Visible="true" ShowInCustomizationForm="true" FixedStyle="Left" VisibleIndex="0" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="150px" VisibleIndex="3" Visible="false" ShowInCustomizationForm="false" PropertiesTextEdit-ClientSideEvents-ValueChanged="" />

                        <dx:GridViewDataTextColumn FieldName="ZiLibera" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiLiberaLegala" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiSapt" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F10022" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F10023" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Afisare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ValActive" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                    </Columns>
                    
                </dx:ASPxGridView>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <table width="100%">
                    <tr>
                        <td>
                            <dx:ASPxHiddenField ID="ccValori" runat="server" ClientInstanceName="ccValori" ClientIDMode="Static"></dx:ASPxHiddenField>
                            <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="false" OnRowInserting="grCC_RowInserting"
                                OnRowUpdating="grCC_RowUpdating" OnInitNewRow="grCC_InitNewRow" OnCustomCallback="grCC_CustomCallback" OnRowDeleting="grCC_RowDeleting" OnAfterPerformCallback="grCC_AfterPerformCallback"
                                OnRowValidating="grCC_RowValidating" OnCellEditorInitialize="grCC_CellEditorInitialize" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared" OnCommandButtonInitialize="grCC_CommandButtonInitialize" >
                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" />
                                <SettingsEditing Mode="Inline" />
                                <SettingsSearchPanel Visible="false" />
                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                <ClientSideEvents ContextMenu="ctx" />

                                <Columns>
                                    <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " ShowNewButtonInHeader="true" />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" ShowInCustomizationForm="false" Width="150px" VisibleIndex="1" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="2" Visible="false">
                                        <PropertiesComboBox TextField="F06205" ValueField="F06204" ValueType="System.Int32" DropDownStyle="DropDown" />
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="3" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProiectChanged(s); }"></ClientSideEvents>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdSubproiect" Name="IdSubproiect" Caption="SubProiect" Width="250px" VisibleIndex="4" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnSubproiectChanged(s); }" EndCallback="OnSubEndCallback"/>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="5" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        <ClientSideEvents EndCallback="OnActEndCallback" />
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="6" Visible="false">
                                        <PropertiesComboBox TextField="Dept" ValueField="IdDept" ValueType="System.Int32" DropDownStyle="DropDown">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Dept" Caption="Dept" Width="130px" />
                                            </Columns>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="7" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                            
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="8" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                            
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre1" Name="NrOre1" Caption="NrOre1" Width="100px" Visible="false" VisibleIndex="9" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre2" Name="NrOre2" Caption="NrOre2" Width="100px" Visible="false" VisibleIndex="10" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre3" Name="NrOre3" Caption="NrOre3" Width="100px" Visible="false" VisibleIndex="11" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre4" Name="NrOre4" Caption="NrOre4" Width="100px" Visible="false" VisibleIndex="12" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre5" Name="NrOre5" Caption="NrOre5" Width="100px" Visible="false" VisibleIndex="13" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre6" Name="NrOre6" Caption="NrOre6" Width="100px" Visible="false" VisibleIndex="14" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre7" Name="NrOre7" Caption="NrOre7" Width="100px" Visible="false" VisibleIndex="15" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre8" Name="NrOre8" Caption="NrOre8" Width="100px" Visible="false" VisibleIndex="16" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre9" Name="NrOre9" Caption="NrOre9" Width="100px" Visible="false" VisibleIndex="17" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre10" Name="NrOre10" Caption="NrOre10" Width="100px" Visible="false" VisibleIndex="18" />

                                    <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                                </Columns>
                                
                                <SettingsCommandButton>
                                    <UpdateButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                        <Styles>
                                            <Style Paddings-PaddingRight="5px" />
                                        </Styles>
                                    </UpdateButton>
                                    <CancelButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                    </CancelButton>

                                    <EditButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                        <Styles>
                                            <Style Paddings-PaddingRight="5px" />
                                        </Styles>
                                    </EditButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>

                                    <NewButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                    </NewButton>
                                </SettingsCommandButton>

                            </dx:ASPxGridView>

                        </td>
                    </tr>
                </table>


            </td>
        </tr>
    </table>
   
    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="200px" HeaderText="Parametrii initializare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="false" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        e.processOnServer = false;
                                        OnInit(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:70px;">
                                <dx:ASPxCheckBox ID="chkNormaZL" ClientInstanceName="chkNormaZL" runat="server" Text="cu norma zile lucratoare" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSD" ClientInstanceName="chkNormaSD" runat="server" Text="cu norma sambata si duminica" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSL" ClientInstanceName="chkNormaSL" runat="server" Text="cu norma sarbatori legale" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkCCCu" ClientInstanceName="chkCCCu" runat="server" Text="cu norma pe centrii de cost" TextAlign="Right" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>
