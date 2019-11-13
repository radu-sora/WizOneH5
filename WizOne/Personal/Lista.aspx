<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.Personal.Lista" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Marca', GoToEditMode);
                    break;
                case "btnSterge":
                    grDate.GetRowValues(e.visibleIndex, 'Marca', GoToDeleteMode);
                    break;
                case "btnTransf":
                    grDate.GetRowValues(e.visibleIndex, 'Marca;Stare', GoToTransfMode);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToDeleteMode(Value) {
            swal({
                title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback("btnSterge;" + Value);
                }
            });
        }

        function GoToTransfMode(values) {
            if (values[1] == "Candidat")
                swal({
                    title: "Atentie!", text: "Sigur doriti transformarea candidatului in angajat?",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnTransf;" + values[0]);
                    }
                });
            else
                swal({
                    title: "Atentie!", text: "Persoana nu are starea de candidat!",
                    type: "info"
                });
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
		
        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }

            AdjustSize();
        }

        function OnInitGrid(s, e) {
            AdjustSize();
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            
            var height = Math.max(0, document.documentElement.clientHeight) - 200;
            grDate.SetHeight(height);
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
                <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" ClientInstanceName="btnNew" ClientIDMode="Static" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s,e){ popUpSablon.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
			<td align="left" Width="210">		               
                <dx:ASPxDropDownEdit ClientInstanceName="checkComboBoxStare" ID="checkComboBoxStare" Width="300" runat="server" AnimationType="None">
                    <DropDownWindowStyle BackColor="#EDEDED" />
                    <DropDownWindowTemplate>
                        <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170">
                            <Border BorderStyle="None" />
                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                            <Items>
                                <dx:ListEditItem Text="(Selectie toate)" Value="1"/>
                                <dx:ListEditItem Text="Activ" Value="2" />
                                <dx:ListEditItem Text="Activ suspendat" Value="3" />
                                <dx:ListEditItem Text="Inactiv" Value="4" />
                                <dx:ListEditItem Text="Candidat" Value="6" />
                                <dx:ListEditItem Text="Angajat in avans" Value="7" />
                                <dx:ListEditItem Text="Activ detasat" Value="8" />
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
                <dx:ASPxButton ID="btnFiltru" runat="server"  RenderMode="Link" OnClick="btnFiltru_Click" >
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2"> 
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="false" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true"  ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true" VerticalScrollBarMode="Visible" />
                    <SettingsSearchPanel Visible="False" />        
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" Init="OnInitGrid"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="50px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnSterge">
                                    <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnTransf">
                                    <Image ToolTip="Transforma candidat in angajat" Url="~/Fisiere/Imagini/Icoane/m5.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                    </Columns>

                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>

<dx:ASPxPopupControl ID="popUpSablon" runat="server" HeaderText="Sabloane" ClientInstanceName="popUpSablon" OnWindowCallback="popUpSablon_WindowCallback"
     AllowDragging="False" AllowResize="False" ClientIDMode="Static" CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="400px" Height="200px" 
        FooterText=" " CloseOnEscape="True" EnableHierarchyRecreation="false" >
    <ClientSideEvents EndCallback="OnEndCallback" />
    <ContentCollection>
        <dx:PopupControlContentControl ID="Popupcontrolcontentcontrol1" runat="server">

            <div class="row">
                <div class="col-md-12">
                    <div style="display:inline-table; float:right;">
                        <dx:ASPxButton ID="btnOK" ClientInstanceName="btnOKSablon" ClientIDMode="Static" runat="server"  ToolTip="Salveaza"  AutoPostBack="false">
                            <ClientSideEvents Click="function(s, e){
                                    popUpSablon.Hide();
                                     popUpSablon.PerformWindowCallback(popUpSablon.GetWindow(0), 'Salvare');
                                    }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                        </dx:ASPxButton>   
                        <label style="display:inline-block;">&nbsp; </label>                   
                        <dx:ASPxButton ID="btnCloseSablon" ClientInstanceName="btnCloseSablon" ClientIDMode="Static" runat="server" ToolTip="Inchide" AutoPostBack="false">
                            <ClientSideEvents Click="function(s, e){ popUpSablon.Hide();}" />
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>                    
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-md-1">
				        <dx:ASPxComboBox  DataSourceID="dsSablon"   ID="cmbSablon"  runat="server" DropDownStyle="DropDown" Width="200" DropDownHeight="150"  DropDownWidth="200"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" AutoPostBack="false"  >
                             <ClientSideEvents SelectedIndexChanged="function(s, e){ popUpSablon.PerformWindowCallback(popUpSablon.GetWindow(0), 'Sablon') }" />
				        </dx:ASPxComboBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-1">
	                 <dx:ASPxCheckBox ID="chkCandidat"  runat="server" Text="Candidat" TextAlign="Left"  ClientInstanceName="chkCandidat" >  
                          <ClientSideEvents CheckedChanged="function(s, e){ popUpSablon.PerformWindowCallback(popUpSablon.GetWindow(0), 'Candidat') }" />                   
                    </dx:ASPxCheckBox>
                </div>
            </div>
            <div class="row">
                <div class="col-md-1">
	                 <dx:ASPxLabel  ID="lblText" runat="server"  Text="" Width="400" style="text-align: left;"></dx:ASPxLabel >	
                </div>
            </div>
            <br />
           <asp:ObjectDataSource runat="server" ID="dsSablon" TypeName="WizOne.Module.General" SelectMethod="GetSablon" />
        </dx:PopupControlContentControl>
    </ContentCollection>
</dx:ASPxPopupControl>

    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>