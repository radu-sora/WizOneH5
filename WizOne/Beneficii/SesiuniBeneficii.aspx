<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="SesiuniBeneficii.aspx.cs" Inherits="WizOne.Beneficii.SesiuniBeneficii" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnNomenclatorAng":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAng);
                    break;    
   
            }
        } 

        function GoToAng(Value) {
            strUrl = getAbsoluteUrl + "Beneficii/relSesiuniBen.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Grupuri angajati");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
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
        var index = -1;
        function OnBatchEditStartEditing(s, e) {
            index = e.visibleIndex;

        }

        function OnListBoxSelectionChanged(ListAng, args) {
            UpdateText();

        }

        function UpdateText() {
            var selectedItems = listBox.GetSelectedItems();
            checkbox.SetText(GetSelectedItemsText(selectedItems));

        }

        function SynchronizeListBoxValues(dropDown, args) {
            listBox.UnselectAll();
            var texts = dropDown.GetText().split(textSeparator);
            var values = GetValuesByTexts(texts);

            listBox.SelectValues(values);
            //    UpdateSelectAllItemState();  
            UpdateText(); // for remove non-existing texts  
        }

        function GetSelectedItemsText(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index >= 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }

        function GetValuesByTexts(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = listBox.FindItemByText(texts[i]);
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
                <dx:ASPxButton ID="btnInit"  runat="server" Text="Initiaza" OnClick="btnInit_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table> 

    <br /> 


    <table width="20%">   
        <tr>     
                <td>
                 <label id="lblDeLa" runat="server" style="display:inline-block;">De la data</label>
                    <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                        <CalendarProperties FirstDayOfWeek="Monday" />
                    </dx:ASPxDateEdit>
                </td>
                <td>
                    <label id="lblLa" runat="server" style="display:inline-block;">La data</label>
                    <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                        <CalendarProperties FirstDayOfWeek="Monday" />
                    </dx:ASPxDateEdit>                    
               </td> 

            <td align="left">
                <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltru_Click">                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left">
                <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                    <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                </dx:ASPxButton>
            </td>                    	
        </tr>
    </table>
    <br />
     <table width="100%"> 
        <tr>
           <td align="left">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"  AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"  OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize"
                    OnRowUpdating="grDate_RowUpdating" OnRowInserting="grDate_RowInserting" OnRowDeleting="grDate_RowDeleting" OnInitNewRow="grDate_InitNewRow">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="False" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true"  SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="1" ButtonType="Image" Caption=" "  Name="butoaneGrid"  >       
                          
                        </dx:GridViewCommandColumn>  
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire"   Width="200px"  >
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                         </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >         
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataDateColumn>
                       <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare"  Width="100px" ReadOnly="true">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                           <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn>                       
                        <dx:GridViewCommandColumn Name="colNomenclatorAng" Width="100px" ButtonType="Image" ShowEditButton="false"  Caption="Grupuri angajati">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnNomenclatorAng" Visibility="BrowsableRow">
                                    <Image ToolTip="Grupuri angajati" Url="~/Fisiere/Imagini/Icoane/info.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>                        
                        </dx:GridViewCommandColumn>               
                      

                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  ReadOnly="true" Width="75px" Visible="false"  ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false"  ShowInCustomizationForm="false"/>
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
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                        </DeleteButton>
                        <NewButton Image-ToolTip="Rand nou">
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                        <Styles>
                            <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                        </Styles>
                       </NewButton>
                    </SettingsCommandButton> 
                  
                </dx:ASPxGridView>                    
            </td>
        </tr>
     </table>    


</asp:Content>
