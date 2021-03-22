<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true" CodeBehind="Cereri.aspx.cs" Inherits="WizOne.Avs.Cereri" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnIstoric":
                    //alert(s.GetRowKey(e.visibleIndex));
                    //grDate.GetRowValues(s.GetFocusedRowIndex(), 'Id', GoToIstoric);
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnDetalii":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDetalii);
                    break;
                case "btnArata":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=5&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToDetalii(Value) {
            strUrl = getAbsoluteUrl + "Avs/Detalii.aspx?qwe=" + Value;
            popGen.SetHeaderText("Detalii");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=9&id=' + Value, '_blank ')
        }

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

        var newItem = 0;
        function OnEndCallbackComp(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        } 

        function OnTextChangedComp(s, e) {
            var val = s.GetValue();
            if (val < 0) {
                swal({
                    title: "", text: "Suma nu poate fi negativa!",
                    type: "warning"
                });
                var tb = grDateComponente.GetEditor("Suma");
                tb.SetValue("0");
            }
        }

    function OnSelectedIndexChanged(s, e, visibleIndex) {    
        var cmbChild;
        if (visibleIndex < 0) 
            cmbChild = eval('cmbChild_new');        
        else 
            cmbChild = eval('cmbChild_' + visibleIndex);
        cmbChild.PerformCallback(s.GetValue());
   
    }

    function OnEndCallbackTarife(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
            }
        }

        function OnSelectedIndexChanged1(s, e, visibleIndex) {
            var cmbChild;
            if (visibleIndex < 0)
                cmbChild = eval('cmbChild1_new');
            else
                cmbChild = eval('cmbChild1_' + visibleIndex);
            cmbChild.PerformCallback(s.GetValue());
        }

        function OnSelectedIndexChanged2(s, e, visibleIndex) {
            var cmbChild;
            if (visibleIndex < 0)
                cmbChild = eval('cmbChild2_new');
            else
                cmbChild = eval('cmbChild2_' + visibleIndex);
            cmbChild.PerformCallback(s.GetValue());
        }  

        function OnEndCallbackSporuri(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnEndCallbackSpTr(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function StartUpload() {
            //pnlLoading.Show();
        }

        function EndUpload(s) {
            //pnlLoading.Hide();
            lblDoc.innerText = s.cpDocUploadName;
            s.cpDocUploadName = null;
        }

        function chkDet_CheckedChanged(s) {
            if (s.name == "chk2") {
                if (s.GetValue() == 1)
                    chk3.SetValue(0);
            }
            if (s.name == "chk3") {
                if (s.GetValue() == 1)
                    chk2.SetValue(0);
            }
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
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                   <ClientSideEvents Click="function(s, e) {                       
                        window.history.back();                        
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>
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

              <table width="40%" align="top">	      
                <tr>
                    <td id="divRol" runat="server">
                        <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px;">Roluri</label>
                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="250px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(8); }" />
                        </dx:ASPxComboBox>
                    </td>

                    <td>
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" DropDownWidth="1100px" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                <dx:ListBoxColumn FieldName="Companie" Caption="Compania" Width="130px" />
                                <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompania" Width="130px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
                            </Columns>
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(9); }" />
                        </dx:ASPxComboBox>
                    </td>

                    <td>
                        <label id="lblAtr" runat="server" style="display:inline-block;">Atribut</label>
                        <dx:ASPxComboBox ID="cmbAtribute" runat="server" ClientInstanceName="cmbAtribute" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(1); }" />
                        </dx:ASPxComboBox>
                    </td>

                    <td>
                        <label id="lblDataMod" runat="server" style="display:inline-block;">Data modificarii</label>
                        <dx:ASPxDateEdit ID="txtDataMod" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >  
                            <CalendarProperties FirstDayOfWeek="Monday" /> 
                            <ClientSideEvents ValueChanged="function(s,e){ pnlCtl.PerformCallback(7) }" />                        
                        </dx:ASPxDateEdit>
                    </td>
                   <td >
                        <label id="lblDataRevisal"   runat="server" style="display:inline-block;">Termen Revisal</label>
                        <dx:ASPxDateEdit ID="deDataRevisal" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" Enabled="false" EditFormat="Custom" >                                 
                        </dx:ASPxDateEdit>
                    </td>
                   <td>
                       <label id="lblGen"   runat="server" style="display:inline-block;"></label>
                        <dx:ASPxCheckBox ID="chkGen"  runat="server" Width="150" Text="Cu generare document"  TextAlign="Left" Checked="false" ClientVisible="false"  ClientInstanceName="chkGen" >                             
                        </dx:ASPxCheckBox>
                    </td>
               </tr>
            </table>

            <table width="40%">
                <tr>
                    <td id="lbl1Act" runat="server"  >
                        <dx:ASPxLabel  ID="lblTxt3Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb1Act" runat="server" ClientInstanceName="cmb1Act" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl2Act" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt4Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb2Act" runat="server" ClientInstanceName="cmb2Act" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl3Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt7Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb3Act" runat="server" ClientInstanceName="cmb3Act" ClientIDMode="Static" Width="50px" ValueField="Id" DropDownWidth="50" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl4Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt8Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb4Act" runat="server" ClientInstanceName="cmb4Act" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl5Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt9Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb5Act" runat="server" ClientInstanceName="cmb5Act" ClientIDMode="Static" Width="200px" ValueField="Id" DropDownWidth="200" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl6Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt10Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb6Act" runat="server" ClientInstanceName="cmb6Act" ClientIDMode="Static" Width="125px" ValueField="Id" DropDownWidth="125" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl7Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt11Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb7Act" runat="server" ClientInstanceName="cmb7Act" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl13Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt13Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb8Act" runat="server" ClientInstanceName="cmb8Act" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl8Act" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt1Act" runat="server"  Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt1Act" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl9Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt2Act" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt2Act" runat="server"  Visible="false" >
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl10Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt5Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxDateEdit ID="de1Act" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td id="lbl11Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt6Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxDateEdit ID="de2Act" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td id="lbl14Act" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt14Act" runat="server"  Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt3Act" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl15Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt15Act" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt4Act" runat="server"  Visible="false" >
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl12Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt12Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
						<dx:ASPxComboBox ID="cmbStructOrgAct" runat="server" ClientInstanceName="cmbStructOrgStrAct" ClientIDMode="Static" Width="600px"  ValueField="IdAuto"  ValueType="System.Int32"  AutoPostBack="false" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F00305" Caption="Subcompanie" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00304" Caption="IdSubcompanie" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00406" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00405" Caption="IdFiliala" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00507" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00506" Caption="IdSectie" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00608" Caption="Departament" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00607" Caption="IdDepartament" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00709" Caption="Subdepartament" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00708" Caption="IdSubdepartament" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00810" Caption="Birou" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00809" Caption="IdBirou" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="IdAuto" Caption="NrCrt" Width="130px" Visible="false"/>
                            </Columns>
						</dx:ASPxComboBox>
                    </td>
               </tr>
                <tr>
                    <td id="lbl1Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt3Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb1Nou" runat="server" ClientInstanceName="cmb1Nou" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl2Nou" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt4Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb2Nou" runat="server" ClientInstanceName="cmb2Nou" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl3Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt7Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb3Nou" runat="server" ClientInstanceName="cmb3Nou" ClientIDMode="Static" Width="50px" ValueField="Id" DropDownWidth="50" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl4Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt8Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb4Nou" runat="server" ClientInstanceName="cmb4Nou" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl5Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt9Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb5Nou" runat="server" ClientInstanceName="cmb5Nou" ClientIDMode="Static" Width="200px" ValueField="Id" DropDownWidth="200" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl6Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt10Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb6Nou" runat="server" ClientInstanceName="cmb6Nou" ClientIDMode="Static" Width="125px" ValueField="Id" DropDownWidth="125" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl7Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt11Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb7Nou" runat="server" ClientInstanceName="cmb7Nou" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl14Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt14Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb8Nou" runat="server" ClientInstanceName="cmb8Nou" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl8Nou" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt1Nou" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt1Nou" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl9Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt2Nou" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt2Nou" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox> 
                    </td>
                    <td id="lbl10Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt5Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxDateEdit ID="de1Nou" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents  ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td id="lbl11Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt6Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxDateEdit ID="de2Nou" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td id="lbl15Nou" runat="server" colspan="2">
                        <dx:ASPxLabel  ID="lblTxt15Nou" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt3Nou" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox>
                    </td>
                    <td id="lbl16Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt16Nou" runat="server"   Visible="false"></dx:ASPxLabel >
                        <dx:ASPxTextBox ID="txt4Nou" runat="server"  Visible="false">
                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandler(s); }" />
                        </dx:ASPxTextBox> 
                    </td>
                    <td id="lbl12Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt12Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
						<dx:ASPxComboBox ID="cmbStructOrgNou" runat="server" ClientInstanceName="cmbStructOrgStrNou" ClientIDMode="Static" Width="600px"  ValueField="IdAuto"  ValueType="System.Int32"  AutoPostBack="false" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F00305" Caption="Subcompanie" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00304" Caption="IdSubcompanie" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00406" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00405" Caption="IdFiliala" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00507" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00506" Caption="IdSectie" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00608" Caption="Departament" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00607" Caption="IdDepartament" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00709" Caption="Subdepartament" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00708" Caption="IdSubdepartament" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="F00810" Caption="Birou" Width="130px" />
                                <dx:ListBoxColumn FieldName="F00809" Caption="IdBirou" Width="130px" Visible="false"/>
                                <dx:ListBoxColumn FieldName="IdAuto" Caption="NrCrt" Width="130px" Visible="false"/>
                            </Columns>
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
						</dx:ASPxComboBox >
                    </td>
                    <td id="lbl13Nou" runat="server">
                        <dx:ASPxLabel  ID="lblTxt13Nou" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxDateEdit ID="de3Nou" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <ClientSideEvents ValueChanged="function(s,e){ OnValueChangedHandler(s); }" />
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </td>
                  </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxCheckBox ID="chk1"  runat="server" Width="200" Text="Platit de angajator actual" TextAlign="Left"   ClientInstanceName="chk1" >                                     
                            </dx:ASPxCheckBox>
                        </td>
                        <td colspan="3">
                            <dx:ASPxCheckBox ID="chk2"  runat="server" Width="200" Text="Detasat in Romania din state UE/NON UE" TextAlign="Left"  ClientInstanceName="chk2" >
                                <ClientSideEvents CheckedChanged="function(s,e){ chkDet_CheckedChanged(s); }" />
                            </dx:ASPxCheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxCheckBox ID="chk4"  runat="server" Width="200" Text="Formular A1" TextAlign="Left"  ClientInstanceName="chk4" >                         
                            </dx:ASPxCheckBox>
                        </td>
                        <td colspan="3">
                            <dx:ASPxCheckBox ID="chk3"  runat="server" Width="200" Text="Detasat din Romania in state UE/NON UE" TextAlign="Left"   ClientInstanceName="chk3" >
                                <ClientSideEvents CheckedChanged="function(s,e){ chkDet_CheckedChanged(s); }" />
                            </dx:ASPxCheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxCheckBox ID="chk5"  runat="server" Width="200" Text="Acord de securitate sociala" TextAlign="Left"   ClientInstanceName="chk5" >                                       
                            </dx:ASPxCheckBox>
                        </td>         
                    </tr>
                   <tr>
                    <dx:ASPxGridView ID="grDateComponente" runat="server" ClientInstanceName="grDateComponente" ClientIDMode="Static" Width="30%" AutoGenerateColumns="false"  OnDataBinding="grDateComponente_DataBinding" 
                        OnRowUpdating="grDateComponente_RowUpdating" OnCellEditorInitialize="grDateComponente_CellEditorInitialize">        
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                        <ClientSideEvents  ContextMenu="ctx" EndCallback="OnEndCallbackComp"/> 
                        <SettingsEditing Mode="Inline" />       
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />          
                            <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Componenta" Width="250px" ReadOnly="true"  /> 
                            <dx:GridViewDataTextColumn FieldName="Suma" Name="Suma" Caption="Suma" Width="100px"  />    
                            <dx:GridViewDataTextColumn FieldName="F02104" Name="F02104" Caption="F02104" Width="100px" Visible="false" ShowInCustomizationForm="false"  /> 
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
                            <NewButton Image-ToolTip="Rand nou">
                                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                <Styles>
                                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                </Styles>
                            </NewButton>
                        </SettingsCommandButton>
                    </dx:ASPxGridView>
                   <dx:ASPxGridView ID="grDateTarife" runat="server" ClientInstanceName="grDateTarife" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateTarife_DataBinding" 
                          OnRowInserting="grDateTarife_RowInserting" OnRowUpdating="grDateTarife_RowUpdating"  >        
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                        <ClientSideEvents  ContextMenu="ctx" EndCallback="OnEndCallbackTarife"/> 
                        <SettingsEditing Mode="Inline" />     
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			                <dx:GridViewDataTextColumn FieldName="DenCateg" Caption="Categorie" VisibleIndex="1">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbMaster" runat="server" DataSourceID="adsMaster" ValueType="System.Int32" ValueField="F01104" TextField="F01107" OnInit="cmbMaster_Init">
					                </dx:ASPxComboBox>
                                     <asp:ObjectDataSource runat="server" ID="adsMaster" TypeName="WizOne.Module.General" SelectMethod="GetCategTarife" >                    
                                        <SelectParameters>
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
			                <dx:GridViewDataTextColumn FieldName="DenTarif" Caption="Tarif" VisibleIndex="2">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbChild" runat="server" DataSourceID="asdChild" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild_Callback" OnInit="cmbChild_Init">                        
					                </dx:ASPxComboBox>	  
                                    <asp:ObjectDataSource runat="server" ID="asdChild" TypeName="WizOne.Module.General" SelectMethod="GetTarife" > 
                                        <SelectParameters>
                                             <asp:Parameter Name="categ"  Type="String" />
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="F01104" VisibleIndex="3" Visible="false"/>
                            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/> 
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
                            <NewButton Image-ToolTip="Rand nou">
                                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                <Styles>
                                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                </Styles>
                            </NewButton>
                        </SettingsCommandButton>
                    </dx:ASPxGridView>
                    <dx:ASPxGridView ID="grDateSporuri1" runat="server" ClientInstanceName="grDateSporuri1" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateSporuri1_DataBinding" 
                          OnRowUpdating="grDateSporuri1_RowUpdating" >        
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                        <ClientSideEvents  ContextMenu="ctx" EndCallback="OnEndCallbackSporuri"/> 
                        <SettingsEditing Mode="Inline" />         
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			                <dx:GridViewDataTextColumn FieldName="Spor" Caption="Spor" VisibleIndex="1">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbMaster1" runat="server" DataSourceID="adsMaster1" ValueType="System.Int32" ValueField="F02504" TextField="F02505" OnInit="cmbMaster1_Init">
					                </dx:ASPxComboBox>
                                     <asp:ObjectDataSource runat="server" ID="adsMaster1" TypeName="WizOne.Module.General" SelectMethod="GetSporuri" >                    
                                        <SelectParameters>
                                             <asp:Parameter Name="param"  Type="String" />
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                     </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
			                <dx:GridViewDataTextColumn FieldName="Tarif" Caption="Tarif" VisibleIndex="2">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbChild1" runat="server" DataSourceID="adsChild1" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild1_Callback" OnInit="cmbChild1_Init">                        
					                </dx:ASPxComboBox>	  
                                    <asp:ObjectDataSource runat="server" ID="adsChild1" TypeName="WizOne.Module.General" SelectMethod="GetTarifeSp" > 
                                        <SelectParameters>
                                             <asp:Parameter Name="categ"  Type="String" />
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="F02504" VisibleIndex="3" Visible="false"/>
                            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/>            
                            <dx:GridViewDataTextColumn FieldName="Id" VisibleIndex="5" Visible="false"/>      
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
                        </SettingsCommandButton>
                    </dx:ASPxGridView>
 
                    <dx:ASPxGridView ID="grDateSporuri2" runat="server" ClientInstanceName="grDateSporuri2" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateSporuri2_DataBinding" 
                          OnRowUpdating="grDateSporuri2_RowUpdating"  >        
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                        <ClientSideEvents  ContextMenu="ctx" EndCallback="OnEndCallbackSporuri"/> 
                        <SettingsEditing Mode="Inline" />                       
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			                <dx:GridViewDataTextColumn FieldName="Spor" Caption="Spor" VisibleIndex="1">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbMaster2" runat="server" DataSourceID="adsMaster2" ValueType="System.Int32" ValueField="F02504" TextField="F02505" OnInit="cmbMaster2_Init">
					                </dx:ASPxComboBox>
                                     <asp:ObjectDataSource runat="server" ID="adsMaster2" TypeName="WizOne.Module.General" SelectMethod="GetSporuri" >                    
                                        <SelectParameters>
                                             <asp:Parameter Name="param"  Type="String" />
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                     </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
			                <dx:GridViewDataTextColumn FieldName="Tarif" Caption="Tarif" VisibleIndex="2">
				                <EditItemTemplate>
					                <dx:ASPxComboBox ID="cmbChild2" runat="server" DataSourceID="adsChild2" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild2_Callback" OnInit="cmbChild2_Init">                        
					                </dx:ASPxComboBox>	  
                                    <asp:ObjectDataSource runat="server" ID="adsChild2" TypeName="WizOne.Module.General" SelectMethod="GetTarifeSp" > 
                                        <SelectParameters>
                                             <asp:Parameter Name="categ"  Type="String" />
                                             <asp:Parameter Name="data"  Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
				                </EditItemTemplate>
			                </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="F02504" VisibleIndex="3" Visible="false"/>
                            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/> 
                            <dx:GridViewDataTextColumn FieldName="Id" VisibleIndex="5" Visible="false"/>      
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
                        </SettingsCommandButton>
                    </dx:ASPxGridView>
                    <dx:ASPxGridView ID="grDateSporTran" runat="server" ClientInstanceName="grDateSporTran" ClientIDMode="Static" Width="30%" AutoGenerateColumns="false"  OnDataBinding="grDateSporTran_DataBinding" 
                           OnRowUpdating="grDateSporTran_RowUpdating"   >        
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                        <ClientSideEvents  ContextMenu="ctx" EndCallback="OnEndCallbackSpTr"/> 
                        <SettingsEditing Mode="Inline" />  
                        <Columns>
                            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />   
                            <dx:GridViewDataTextColumn FieldName="Cod" Name="Cod" Caption="Cod"  Width="75px"  ReadOnly="true"/>
                            <dx:GridViewDataComboBoxColumn FieldName="Spor" Name="Spor" Caption="Spor" Width="150px" ReadOnly="false" >
                                <PropertiesComboBox TextField="F02105" ValueField="F02104" ValueType="System.Int32" DropDownStyle="DropDown" />                
                            </dx:GridViewDataComboBoxColumn>                 
                             <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="100px" Visible="false"/>
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
                        </SettingsCommandButton>
                    </dx:ASPxGridView>

                </tr>
                <tr>
                    <td>
                        <label id="lblExpl" runat="server">Explicatii</label>
                        <dx:ASPxMemo ID="txtExpl" runat="server"  Width="250px" Height="100px" ></dx:ASPxMemo>
                    </td>
                    <td style="padding-right:10px;" >
                   
                        <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px" 
                            BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="Incarca document" ShowTextBox="false"
                            ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocUploadResource1">
                            <BrowseButton>
                                <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                            </BrowseButton>
                            <ValidationSettings ShowErrors="False"></ValidationSettings>

                            <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                        </dx:ASPxUploadControl>
                    </td>
                    <td style="padding-right:10px;">   
                        <dx:ASPxButton ID="btnDocSterge" runat="server" ToolTip="Sterge document" AutoPostBack="false" Height="28px"   meta:resourcekey="btnDocStergeResource1">
                            <Image Url="../Fisiere/Imagini/Icoane/sterge.png" Width="16px" Height="16px"></Image>
                            <Paddings PaddingLeft="0px" PaddingRight="0px" />
                            <ClientSideEvents Click="function(s,e) { pnlCtl.PerformCallback(10); }" />
                        </dx:ASPxButton>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                        <label id="lblDocument" runat="server">Document</label>
                        <dx:ASPxTextBox ID="txtDocument" runat="server"  Width="250px" Height="21px" ></dx:ASPxTextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <label id="lblDoc" clientidmode="Static" runat="server" style="display:inline-block; margin-bottom:0px; margin-top:4px; padding:0; height:22px; line-height:22px; vertical-align:text-bottom;">&nbsp; </label>
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
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(11); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <label id="lblAtrFiltru" runat="server" style="display:inline-block;">Atribut</label>
                        <dx:ASPxComboBox ID="cmbAtributeFiltru" runat="server" ClientInstanceName="cmbAtributeFiltru" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >                                                     
                        </dx:ASPxComboBox>
                    </td>                    								

					<td>		
                        <label id="lblStare" runat="server" style="display:inline-block;">Stare</label>	
                        <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare" ID="checkComboBoxStare" Width="210px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn"
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
                    <td valign="bottom">
                        <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(5);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </td>
                    <td valign="bottom">
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
                       <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared"
                            OnCustomUnboundColumnData="grDate_CustomUnboundColumnData" OnCustomButtonInitialize="grDate_CustomButtonInitialize">
                            <SettingsBehavior AllowFocusedRow="true"  />
                            <Settings ShowFilterRow="False" ShowColumnHeaders="true" />  
                            <SettingsEditing Mode="Inline" />    
                             <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />            
                            <Columns>
                                <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                                    <CustomButtons>
                                        <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                            <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton ID="btnDetalii">
                                            <Image ToolTip="Detalii" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                        </dx:GridViewCommandColumnCustomButton>
                                        <dx:GridViewCommandColumnCustomButton ID="btnArata">
                                            <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                        </dx:GridViewCommandColumnCustomButton>
                                    </CustomButtons>
                                </dx:GridViewCommandColumn>
                                <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" Visible="false" />
                                <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="200px" />
                                <dx:GridViewDataTextColumn FieldName="IdAtribut" Name="IdAtribut" Caption="IdAtribut" ReadOnly="true" Width="50px" Visible="false" />
					            <dx:GridViewDataTextColumn FieldName="NumeAtribut" Name="NumeAtribut" Caption="Atribut" ReadOnly="true"  Width="200px" />					
					            <dx:GridViewDataDateColumn FieldName="DataModif" Name="DataModif" Caption="Data modificarii" ReadOnly="true"  Width="100px">
                                     <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                </dx:GridViewDataDateColumn>
					            <dx:GridViewDataTextColumn FieldName="DataRevisal" Name="DataRevisal" Caption="Termen Revisal" ReadOnly="true"  Width="100px"  UnboundType="String" />	
					            <dx:GridViewDataTextColumn FieldName="ValoareNoua" Name="ValoareNoua" Caption="Valoare" ReadOnly="true"  Width="100px" />		
                                <dx:GridViewDataTextColumn FieldName="SalariulNet" Name="SalariulNet" Caption="Salariul Net" ReadOnly="true"  Width="100px" />
                                <dx:GridViewDataCheckColumn FieldName="ScutitImpozit" Name="ScutitImpozit" Caption="Scutit Impozit" ReadOnly="true" Width="70px"  />
                                <dx:GridViewDataTextColumn FieldName="Motiv" Name="Motiv" Caption="Motiv" ReadOnly="true" Width="150px" />
                                <dx:GridViewDataTextColumn FieldName="Explicatii" Name="Explicatii" Caption="Explicatii" ReadOnly="true" Width="300px" />
                                <dx:GridViewDataCheckColumn FieldName="Actualizat" Name="Actualizat" Caption="Actualizat" ReadOnly="true" Width="70px"  />
                                <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="250px" VisibleIndex="2" >
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare" ReadOnly="true" Width="75px" Visible="false" />
                                <dx:GridViewDataTextColumn FieldName="IdAtribut" Name="IdAtribut" Caption="IdAtribut" ReadOnly="true" Width="75px" Visible="false" />
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
