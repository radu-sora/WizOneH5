﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="NotaLichidare.aspx.cs" Inherits="WizOne.Personal.NotaLichidare" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        

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


        function OnGridBatchEditEndEditing(s, e) {

        }
        function OnGridBatchEditStartEditing(s, e) {

        }

        function OnGridDetBatchEditEndEditing(s, e) {

        }
        function OnGridDetBatchEditStartEditing(s, e) {
            var keyIndex = s.GetColumnByField("IdNotaLichidare").index;
            var key = e.rowValues[keyIndex].value;

            var sir = "<%=Session["NL_Stare"] %>";
            var res = sir.split(";");
            for (var i = 0; i < res.length; i++) {
                var linie = res[i].split(",");
                if (linie[0] == key && (linie[1] == -1 || linie [1] == 3)) {
                    e.cancel = true;
                }
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRap" ClientInstanceName="btnRap" ClientIDMode="Static" runat="server" Text="Raport" OnClick="btnRap_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
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
            <table style="width:55%;">
                <tr>
 
                    <td>
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                            </Columns>
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(9); }" />
                        </dx:ASPxComboBox>
                    </td>
			        <td align="left">		
                        <label id="lblStare" runat="server" style="display:inline-block;">Stare nota</label>	
                        <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare" ID="checkComboBoxStare" Width="210px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                                    <Border BorderStyle="None" />
                                    <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />             
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
   					<td style="padding-right:15px !important;">
                        <dx:ASPxLabel  id="lblStareDatorii" runat="server" style="display:inline-block;" Text="Stare datorii"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbStareDatorii" runat="server" ClientInstanceName="cmbStareDatorii" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback(s.name); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(s.name);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        </dx:ASPxButton>
                    </td>                    	
                </tr>
            </table>
            <br />


            <table style="width:70%;">
                <tr>
                    <td >
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnBatchUpdate="grDate_BatchUpdate"  >
                            <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                            <Settings ShowFilterRow="false" ShowColumnHeaders="true" />  
                             <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" /> 
                            <ClientSideEvents  ContextMenu="ctx"   
                                BatchEditEndEditing="function(s,e) { OnGridBatchEditEndEditing(s,e); }"
                                BatchEditStartEditing="function(s,e) { OnGridBatchEditStartEditing(s,e); }"                                
                                EndCallback="function(s,e) { OnEndCallback(s,e); }" />     
                            <Styles>
                                <BatchEditModifiedCell BackColor="Transparent">
                                </BatchEditModifiedCell>
                            </Styles> 
                            <Columns>     
                               <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" ReadOnly="true"  Width="100px" VisibleIndex="1">           
                                    <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown" />
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataTextColumn FieldName="NrDoc" Name="NrDoc" Caption="Nr. document" ReadOnly="true" Width="100px" VisibleIndex="2" />    
                                 <dx:GridViewDataDateColumn FieldName="DataDoc" Name="DataDoc" Caption="Data document" ReadOnly="true"  Width="100px" VisibleIndex="3" >
                                        <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                                </dx:GridViewDataDateColumn>                               
                                <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare"  Width="100px" VisibleIndex="4">                            
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                </dx:GridViewDataComboBoxColumn>
				                <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii"  Width="100px" VisibleIndex="5" />
                                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px"  />	
                                <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px"  />	
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false"  Width="100px"  />	
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>


            <table style="width:70%;">
                <tr>
                    <td >
                        <dx:ASPxGridView ID="grDateDet" runat="server" ClientInstanceName="grDateDet" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"   OnBatchUpdate="grDateDet_BatchUpdate"    >
                           <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control"  />
                            <Settings ShowFilterRow="false" ShowColumnHeaders="true" />  
                            <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                            <ClientSideEvents  ContextMenu="ctx"           
                                BatchEditEndEditing="function(s,e) { OnGridDetBatchEditEndEditing(s,e); }"
                                BatchEditStartEditing="function(s,e) { OnGridDetBatchEditStartEditing(s,e); }"                                   
                                EndCallback="function(s,e) { OnEndCallback(s,e); }" />    
                            <Styles>
                                <BatchEditModifiedCell BackColor="Transparent">
                                </BatchEditModifiedCell>
                            </Styles>                            
                            <Columns>     
						        <dx:GridViewDataTextColumn FieldName="IdNotaLichidare" Name="IdNotaLichidare" Caption="Id" ReadOnly="true" Width="100px" VisibleIndex="1" />
                                <dx:GridViewDataTextColumn FieldName="Rol" Name="Rol" Caption="Rol" ReadOnly="true" Width="100px" VisibleIndex="2" />    
                                <dx:GridViewDataTextColumn FieldName="Valoare" Name="Valoare" Caption="Valoare"  Width="100px" VisibleIndex="3" />                                   
                                <dx:GridViewDataTextColumn FieldName="Datorii" Name="Datorii" Caption="Datorii"  Width="100px" VisibleIndex="4" />    
				                <dx:GridViewDataTextColumn FieldName="Comentarii" Name="Comentarii" Caption="Comentarii"  Width="100px" VisibleIndex="5" />
                                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px"  />	
                                <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px"  />	
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false"  Width="100px"  />	
                            </Columns>                           
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
