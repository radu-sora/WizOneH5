<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Cereri.aspx.cs" Inherits="WizOne.Avs.Cereri" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnIstoric":
                    //alert(s.GetRowKey(e.visibleIndex));
                    //grDate.GetRowValues(s.GetFocusedRowIndex(), 'Id', GoToIstoric);
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=5&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
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
                    title: "Atentie !", text: s.cpAlertMessage,
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
                                CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
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
                </tr>
            </table>

            <table width="40%">
                            <tr>
                    <td id="lbl1Act" runat="server">
                        <dx:ASPxLabel  ID="lblTxt3Act" runat="server"  style="display:inline-block;" Visible="false"></dx:ASPxLabel >
                        <dx:ASPxComboBox ID="cmb1Act" runat="server" ClientInstanceName="cmb1Act" ClientIDMode="Static" Width="250px" ValueField="Id" DropDownWidth="250" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                        </dx:ASPxComboBox>
                    </td>
                    <td id="lbl2Act" runat="server">
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
                    <td id="lbl8Act" runat="server">
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
                    <td id="lbl2Nou" runat="server">
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
                    <td id="lbl8Nou" runat="server">
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
               </tr>
                <tr>
                    <td>
                        <label id="lblExpl" runat="server">Explicatii</label>
                        <dx:ASPxMemo ID="txtExpl" runat="server"  Width="250px" Height="100px" ></dx:ASPxMemo>
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
                            OnCustomUnboundColumnData="grDate_CustomUnboundColumnData" >
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
