<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Selectare.aspx.cs" Inherits="WizOne.Beneficii.Selectare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDateBen_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnArataBen":
                    grDateBen.GetRowValues(e.visibleIndex, 'IdBeneficiu', GoToAtasMode);
                    break;   
                case "btnArataAngBen":
                    grDateBen.GetRowValues(e.visibleIndex, 'F10003', GoToAtasAngMode);
                    break;      
            }
        }

        function grDateSes_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnArataSes":
                    grDateSes.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;     
                case "btnArataAngSes":
                    grDateSes.GetRowValues(e.visibleIndex, 'F10003', GoToAtasAngMode);
                    break;  
            }
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=19&id=' + Value, '_blank ')
        }

        function GoToAtasAngMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=21&id=' + Value, '_blank ')
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

<dx:ASPxRoundPanel ID="pnlBen" ClientInstanceName="pnlBen" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Beneficii active" Width="100%">
    <HeaderStyle Font-Bold="true" />
   
    <PanelCollection>
        <dx:PanelContent>

    <br />
    <table width="60%">   
        <tr>
            <td>
                <label id="lblBen" runat="server" style="display:inline-block;">In acest moment, urmatoarele Beneficii sunt active:</label>                  
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="48%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDateBen" runat="server" ClientInstanceName="grDateBen" ClientIDMode="Static"  AutoGenerateColumns="false"  OnRowUpdating="grDateBen_RowUpdating">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />  
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDateBen_CustomButtonClick"  ContextMenu="ctx"  />
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  > 
                            <CustomButtons> 
                                <dx:GridViewCommandColumnCustomButton ID="btnArataAngBen">
                                    <Image ToolTip="Arata document angajat" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                                   
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdBeneficiu" Name="IdBeneficiu" Caption="Beneficiu" ReadOnly="true"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" ReadOnly="true"  Width="500px" />	
					    <dx:GridViewDataDateColumn FieldName="DataSfarsitBen" Name="DataSfarsitBen" Caption="Valabilitate" ReadOnly="true"   Width="100px" >
                             <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewCommandColumn Width="90px" ButtonType="Image" Caption="Atasament" Name="butoaneGrid" >
                            <CustomButtons>                               
                                <dx:GridViewCommandColumnCustomButton ID="btnArataBen">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>  
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003"   Width="50px" ReadOnly="true" Visible="false"  ShowInCustomizationForm="false"/>	
                    </Columns> 
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>       
                    </SettingsCommandButton>   
                        <Templates>
                            <EditForm>
                                <div style="padding: 4px 3px 4px">
                                    <table class="auto-style8">                              
                                    <tr>
                                        <td style="padding:10px !important;" colspan="3">
                                            <dx:ASPxUploadControl ID="btnDocUploadBen" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="Incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadBen" OnFileUploadComplete="btnDocUploadBen_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                            </dx:ASPxUploadControl>
                                        </td>
                                    </tr>     
                                        <tr>
                                            <td class="auto-style3" style="padding:10px;">
                                                <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                    <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" runat="server" ReplacementType="EditFormUpdateButton" />
                                                    <dx:ASPxGridViewTemplateReplacement ID="CancelButton" runat="server" ReplacementType="EditFormCancelButton" />
                                                </div>
                                            </td>
                                        </tr>                                        
                                    </table>
                                </div>
                            </EditForm>
                        </Templates>                    
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>
    <br />
        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>

<dx:ASPxRoundPanel ID="pnlSes" ClientInstanceName="pnlSes" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" HeaderText="Sesiune activa pentru selectarea Beneficiilor" Width="100%">
    <HeaderStyle Font-Bold="true" />
    <PanelCollection>
        <dx:PanelContent>

    <br />
    <table width="60%">   
        <tr>
            <td>
                <dx:ASPxLabel ID ="lblSes" runat="server" style="display:inline-block;" EncodeHtml="False" />              
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="48%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDateSes" runat="server" ClientInstanceName="grDateSes" ClientIDMode="Static"  AutoGenerateColumns="false"  OnRowUpdating="grDateSes_RowUpdating">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDateSes_CustomButtonClick"  ContextMenu="ctx"   EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"  > 
                            <CustomButtons> 
                                <dx:GridViewCommandColumnCustomButton ID="btnArataAngSes">
                                    <Image ToolTip="Arata document angajat" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                                   
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Id" Name="Id" Caption="Beneficiu" ReadOnly="true"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Descriere" ReadOnly="true"  Width="500px" />	
                        <dx:GridViewDataDateColumn FieldName="DataInceputBen" Name="DataInceputBen" Caption="Data inceput valabilitate beneficiu" HeaderStyle-Wrap="True"  Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsitBen" Name="DataSfarsitBen" Caption="Data sfarsit valabilitate beneficiu"  HeaderStyle-Wrap="True"   Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataDateColumn> 
                        <dx:GridViewCommandColumn Width="90px" ButtonType="Image" Caption="Atasament" Name="butoaneGrid" >
                            <CustomButtons>                               
                                <dx:GridViewCommandColumnCustomButton ID="btnArataSes">
                                    <Image ToolTip="Arata document" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>                       
                        <dx:GridViewCommandColumn Width="100px" ButtonType="Image" Caption="Selecteaza" ShowSelectCheckbox="true" SelectAllCheckboxMode="None" />
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003"   Width="50px" ReadOnly="true" Visible="false"  ShowInCustomizationForm="false"/>	
                    </Columns>     
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>       
                    </SettingsCommandButton>   
                        <Templates>
                            <EditForm>
                                <div style="padding: 4px 3px 4px">
                                    <table class="auto-style8">                              
                                    <tr>
                                        <td style="padding:10px !important;" colspan="3">
                                            <dx:ASPxUploadControl ID="btnDocUploadSes" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="Incarca document" ShowTextBox="false"
                                                ClientInstanceName="btnDocUploadSes" OnFileUploadComplete="btnDocUploadSes_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                            </dx:ASPxUploadControl>
                                        </td>
                                    </tr>     
                                        <tr>
                                            <td class="auto-style3" style="padding:10px;">
                                                <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                    <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" runat="server" ReplacementType="EditFormUpdateButton" />
                                                    <dx:ASPxGridViewTemplateReplacement ID="CancelButton" runat="server" ReplacementType="EditFormCancelButton" />
                                                </div>
                                            </td>
                                        </tr>                                        
                                    </table>
                                </div>
                            </EditForm>
                        </Templates>                      
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

        </dx:PanelContent>
    </PanelCollection>
</dx:ASPxRoundPanel>
</asp:Content>
