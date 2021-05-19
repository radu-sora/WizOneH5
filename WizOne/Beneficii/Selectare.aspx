<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Selectare.aspx.cs" Inherits="WizOne.Beneficii.Selectare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDateBen_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnArataBen":
                    grDateBen.GetRowValues(e.visibleIndex, 'IdBeneficiu', GoToAtasMode);
                    break;
            }
        }

        function grDateSes_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnArataSes":
                    grDateSes.GetRowValues(e.visibleIndex, 'IdBeneficiu', GoToAtasMode);
                    break;
            }
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=19&id=' + Value, '_blank ')
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
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 
    <br /> 
        <table width="60%">   
        <tr>
            <td>
                <fieldset class="fieldset-auto-width">
                	<legend id="lgBen" runat="server" class="legend-font-size">Beneficii active</legend>                  
                </fieldset>
            </td>                    	
        </tr>
    </table>
    <br />
    <table width="60%">   
        <tr>
            <td>
                <label id="lblBen" runat="server" style="display:inline-block;">In acest moment, urmatoarele Beneficii sunt active:</label>                  
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDateBen" runat="server" ClientInstanceName="grDateBen" ClientIDMode="Static"  AutoGenerateColumns="false"  >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDateBen_CustomButtonClick"  ContextMenu="ctx"  />
                    <Columns>
                        <dx:GridViewDataComboBoxColumn FieldName="Id" Name="Id" Caption="Beneficiu" ReadOnly="true"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" ReadOnly="true"  Width="250px" />	
					    <dx:GridViewDataDateColumn FieldName="LaData" Name="LaData" Caption="Valabilitate" ReadOnly="true"   Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewCommandColumn Width="90px" ButtonType="Image" Caption="Atasament" Name="butoaneGrid" >
                            <CustomButtons>                               
                                <dx:GridViewCommandColumnCustomButton ID="btnArataBen">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>   
                    </Columns>                  
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>
    <br />
       <table width="60%">   
        <tr>
            <td>
                <fieldset class="fieldset-auto-width">
                	<legend id="lgSes" runat="server" class="legend-font-size">Sesiune activa pentru selectarea Beneficiilor</legend>                  
                </fieldset>
            </td>                    	
        </tr>
    </table>
    <br />
    <table width="60%">   
        <tr>
            <td>
                <label id="lblSes" runat="server" style="display:inline-block;"></label>                  
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDateSes" runat="server" ClientInstanceName="grDateSes" ClientIDMode="Static"  AutoGenerateColumns="false"  >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDateSes_CustomButtonClick"  ContextMenu="ctx"  />
                    <Columns>
                        <dx:GridViewDataComboBoxColumn FieldName="Id" Name="Id" Caption="Beneficiu" ReadOnly="true"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                             <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" ReadOnly="true"  Width="250px" />	
                        <dx:GridViewCommandColumn Width="90px" ButtonType="Image" Caption="Atasament" Name="butoaneGrid" >
                            <CustomButtons>                               
                                <dx:GridViewCommandColumnCustomButton ID="btnArataSes">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>   
                        <dx:GridViewCommandColumn Width="100px" ButtonType="Image" Caption="Selecteaza" ShowSelectCheckbox="true" SelectAllCheckboxMode="None" />
                    </Columns>                  
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>
    <br />
        <table width="60%">   
        <tr>
            <td>
                <dx:ASPxButton ID="btnValidare" ClientInstanceName="btnValidare" ClientIDMode="Static" runat="server" AutoPostBack="false" Text="Validare selectie Beneficii" oncontextMenu="ctx(this,event)" OnClick="btnValidare_Click">                    
                    
                </dx:ASPxButton>                             
            </td>                    	
        </tr>
    </table>
</asp:Content>
