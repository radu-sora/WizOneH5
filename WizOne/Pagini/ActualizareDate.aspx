<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ActualizareDate.aspx.cs" Inherits="WizOne.Pagini.ActualizareDate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <script language="javascript" type="text/javascript">

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
         checkComboBoxModul.SetText(GetSelectedItemsText(selectedItems));
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
                <dx:ASPxButton ID="btnAct" ClientInstanceName="btnAct" ClientIDMode="Static" runat="server" Text="Actualizeaza" AutoPostBack="false" OnClick="btnAct_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { 
                        pnlLoading.Show();
                        e.processOnServer = true;
                     }" />
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />
				
                <div class="Absente_divOuter">

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblUserVechi" runat="server" style="display:inline-block;">Inlocuieste </label>
                        <dx:ASPxComboBox ID="cmbUserVechi" runat="server" ClientInstanceName="cmbUserVechi" ClientIDMode="Static" Width="215px" ValueField="F70102" DropDownWidth="200" 
                            TextField="F70104" ValueType="System.Int32" AutoPostBack="false"  >
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblUserNou" runat="server" style="display:inline-block;"> cu </label>
                        <dx:ASPxComboBox ID="cmbUserNou" runat="server" ClientInstanceName="cmbUserNou" ClientIDMode="Static" Width="215px" ValueField="F70102" DropDownWidth="200" 
                            TextField="F70104" ValueType="System.Int32" AutoPostBack="false"  >                            
                        </dx:ASPxComboBox>
                    </div>
                    
                    <div class="Absente_Cereri_CampuriSup" id="divRol" runat="server">
                        <label id="lblCtrInc" runat="server" style="display:inline-block;"> incepand cu data de </label>
                        <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>
                </div>
                <br /><br />
                 <div class="Absente_divOuter">		
                     <div class="Absente_Cereri_CampuriSup">
                        <label id="lblModul" runat="server" style="display:inline-block;">Modul</label>	
                        <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxModul" ID="checkComboBoxModul" Width="210px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn"
                                    runat="server">
                                    <Border BorderStyle="None" />
                                    <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                    <Items>
                                        <dx:ListEditItem Text="(Selectie toate)" />
                                        <dx:ListEditItem Text="Supervizori" Value="1" />
                                        <dx:ListEditItem Text="Organigrama" Value="2" />
                                        <dx:ListEditItem Text="Cereri" Value="3" />
                                        <dx:ListEditItem Text="Avans" Value="4" />
                                        <dx:ListEditItem Text="Prime" Value="5" />
                                        <dx:ListEditItem Text="Cereri diverse" Value="6" />
                                        <dx:ListEditItem Text="Evaluare" Value="7" />
                                    </Items>
                                    <ClientSideEvents SelectedIndexChanged="OnListBoxSelectionChanged" />
                                </dx:ASPxListBox>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="padding: 4px">
                                            <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Inchide" style="float: right">
                                                <ClientSideEvents Click="function(s, e){ checkComboBoxModul.HideDropDown(); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </DropDownWindowTemplate>
                            <ClientSideEvents TextChanged="SynchronizeListBoxValues" DropDown="SynchronizeListBoxValues" />
                        </dx:ASPxDropDownEdit>
                         </div>
                    </div>

            </td>
        </tr>
    </table>

</asp:Content>
