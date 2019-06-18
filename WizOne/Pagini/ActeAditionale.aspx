<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ActeAditionale.aspx.cs" Inherits="WizOne.Pagini.ActeAditionale" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function OnMetodeClick(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                    grDate.PerformCallback(s.name + ";1");
            }
            else
            {
                swal({
                    title: trad_string(limba, "Atentie"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "info"
                });
            }
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=4&id=' + Value, '_blank ')
        }

        function ValidareUpload(s, Value) {
            if (Value == "-99") {
                s.Cancel();
                swal({
                    title: trad_string(limba, "Atentie"), text: trad_string(limba, "Nu se poate adauga atasament deoarece nu exista numar"),
                    type: "warning"
                });
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
            cmbStare.SetText(GetSelectedItemsText(selectedItems));
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







        function OnInitGrid(s, e) {
            AdjustSize();
        }

        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }

        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight) - 220;
            grDate.SetHeight(height);
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }

            AdjustSize();
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
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNr" ClientInstanceName="btnNr" ClientIDMode="Static" runat="server" Text="Atribuire numar" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/istoric.png"></Image>
                    <ClientSideEvents Click="function(s,e) { OnMetodeClick(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnTiparit" ClientInstanceName="btnTiparit" ClientIDMode="Static" runat="server" Text="Tiparit" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                    <ClientSideEvents Click="function(s,e) { OnMetodeClick(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSemnat" ClientInstanceName="btnSemnat" ClientIDMode="Static" runat="server" Text="Semnat" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/doc2.png"></Image>
                    <ClientSideEvents Click="function(s,e) { OnMetodeClick(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnFinalizat" ClientInstanceName="btnFinalizat" ClientIDMode="Static" runat="server" Text="Finalizat" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function(s,e) { OnMetodeClick(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="Absente_divOuter" style="margin:15px 0px;">

                    <div class="Absente_Cereri_CampuriSup" id="pnlComp" runat="server" visible="false">
                        <label id="lblCmp" runat="server" style="display:inline-block;">Companie</label>
                        <dx:ASPxComboBox ID="cmbCmp" ClientInstanceName="cmbCmp" ClientIDMode="Static" runat="server" Width="150px" AutoPostBack="true" ValueField="F00202" TextField="F00204" ValueType="System.Int32" AllowNull="false" OnSelectedIndexChanged="cmbComp_SelectedIndexChanged"/>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblTip" runat="server" style="display:inline-block;">Tip</label>
                        <dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" ClientIDMode="Static" runat="server" Width="150px" AutoPostBack="true" AllowNull="true" OnValueChanged="cmbTip_ValueChanged" >
                            <Items>
                                <dx:ListEditItem Text="(Selectie toate)" Value="9" />
                                <dx:ListEditItem Text="Angajat" Value="0" />
                                <dx:ListEditItem Text="Candidat" Value="1" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat/Candidat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" AllowNull="true" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblStatus" runat="server" style="display:inline-block;">Status</label>
                        <dx:ASPxComboBox ID="cmbStatus" ClientInstanceName="cmbStatus" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" >
                            <Items>
                                <dx:ListEditItem Text="Numar atribuit" Value="1" />
                                <dx:ListEditItem Text="Tiparit" Value="2" />
                                <dx:ListEditItem Text="Semnat" Value="3" />
                                <dx:ListEditItem Text="Trimis in Revisal" Value="4" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblData" runat="server" style="display:inline-block;">Data modificarii</label>
                        <dx:ASPxDateEdit ID="txtData" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" AllowNull="true" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDepasire" runat="server" style="display:inline-block;">Depasire Revisal</label>
                        <dx:ASPxDateEdit ID="txtDepasire" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" AllowNull="true" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup" style="margin-top:28px;">
                        <table>
                            <tr>
                                <td>
                                    <div style="float:left;">
                                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                                        </dx:ASPxButton>
                                    </div>
                                </td>
                                <td>&nbsp;&nbsp;</td>
                                <td>
                                    <div style="float:left;">
                                        <dx:ASPxButton ID="btnFiltruSterge" Text="Sterge Filtru" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="true" oncontextMenu="ctx(this,event)" OnClick="btnFiltruSterge_Click" >                    
                                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                        </dx:ASPxButton>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>

                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize" OnHtmlRowCreated="grDate_HtmlRowCreated" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents 
                        ContextMenu="ctx" 
                        EndCallback="function(s,e) { OnEndCallback(s,e); }"
                        Init="OnInitGrid"  />
                    <Columns>

                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        
                        <dx:GridViewCommandColumn Width="66px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca" ReadOnly="true" Width="80px" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Nume si prenume" ReadOnly="true" Width="250px" />
                        <dx:GridViewDataDateColumn FieldName="DataModif" Name="DataModif" Caption="Data modificarii" ReadOnly="true" Width="100px" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="TermenDepasire" Name="TermenDepasire" Caption="Termen depunere Revisal" ReadOnly="true" Width="140px" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataCheckColumn FieldName="Candidat" Name="Candidat" Caption="Candidat" ReadOnly="true" Width="70px"  />

                        <dx:GridViewBandColumn Caption="Atribute" HeaderStyle-HorizontalAlign="Center">
                            <Columns>
                                <dx:GridViewDataCheckColumn FieldName="CORCod" Name="COR" Caption="COR" ReadOnly="true" Width="80px" />
                                <dx:GridViewDataCheckColumn FieldName="FunctieId" Name="Functie" Caption="Functie" ReadOnly="true" Width="80px" />
                                <dx:GridViewDataCheckColumn FieldName="Norma" Name="Norma" Caption="Norma" ReadOnly="true" Width="80px" />
                                <dx:GridViewDataCheckColumn FieldName="Salariul" Name="Salariu" Caption="Salariu" ReadOnly="true" Width="80px" />
                                <dx:GridViewDataCheckColumn FieldName="Spor" Name="Spor" Caption="Spor" ReadOnly="true" Width="80px" />
                                <dx:GridViewDataCheckColumn FieldName="Structura" Name="Structura" Caption="Structura" ReadOnly="true" Width="80px"  />
                                <dx:GridViewDataCheckColumn FieldName="CIMDet" Name="CIMDet" Caption="CIM det." ReadOnly="true" Width="80px"  />
                                <dx:GridViewDataCheckColumn FieldName="CIMNed" Name="CIMNed" Caption="CIM nedet." ReadOnly="true" Width="80px"  />
                                <dx:GridViewDataCheckColumn FieldName="Motiv" Name="Motiv" Caption="Motiv plecare" ReadOnly="true" Width="80px"  />
                            </Columns>
                        </dx:GridViewBandColumn>

                        <dx:GridViewDataTextColumn FieldName="DocNr" Name="DocNr" Caption="Nr Doc." ReadOnly="true" Width="100px" />
                        <dx:GridViewDataDateColumn FieldName="DocData" Name="DocData" Caption="Data Doc." ReadOnly="true" Width="100px" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>

                        <dx:GridViewBandColumn Caption="Status" HeaderStyle-HorizontalAlign="Center">
                            <Columns>
                                <dx:GridViewDataCheckColumn FieldName="Tiparit" Name="Tiparit" Caption="Tiparit" ReadOnly="true" Width="80px"  />
                                <dx:GridViewDataCheckColumn FieldName="Semnat" Name="Semnat" Caption="Semnat" ReadOnly="true" Width="80px"  />
                                <dx:GridViewDataCheckColumn FieldName="Revisal" Name="Revisal" Caption="REVISAL" ReadOnly="true" Width="80px"  />
                            </Columns>
                        </dx:GridViewBandColumn>

                        <dx:GridViewBandColumn Caption="Atasamente" HeaderStyle-HorizontalAlign="Center" Name="colAtas">
                            <Columns>
                                <dx:GridViewDataCheckColumn FieldName="AreAtas" Name="AreAtas" Caption="Are Atas." ReadOnly="true" Width="80px"  />

                                <dx:GridViewDataTextColumn Width="100px" Caption="Incarca" CellStyle-HorizontalAlign="Center" Name="colUpload">
                                    <DataItemTemplate>
                                        <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ShowProgressPanel="true" Height="28px"
                                            BrowseButton-Text="" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                            OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false">
                                            <BrowseButton>
                                                <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                            </BrowseButton>
                                            <ValidationSettings ShowErrors="False"></ValidationSettings>
                                        </dx:ASPxUploadControl>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>

                                <dx:GridViewDataTextColumn Width="100px" Caption="Vizualizare" CellStyle-HorizontalAlign="Center" Name="colArata">
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnArata" runat="server" Text="" AutoPostBack="false" ToolTip="Arata document" oncontextMenu="ctx(this,event)" >
                                            <Image Url="~/Fisiere/Imagini/Icoane/arata.png"></Image>
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataTextColumn>
                                
                                <dx:GridViewDataColumn Width="100px" Caption="Stergere" CellStyle-HorizontalAlign="Center" Name="colSterge">
                                    <DataItemTemplate>
                                        <dx:ASPxButton ID="btnSterge" runat="server" Text="" AutoPostBack="false" ToolTip="Sterge document" oncontextMenu="ctx(this,event)" >
                                            <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                                           
                                        </dx:ASPxButton>
                                    </DataItemTemplate>
                                </dx:GridViewDataColumn>
                            </Columns>
                        </dx:GridViewBandColumn>

                        <dx:GridViewDataTextColumn FieldName="IdAutoAct" Name="IdAutoAct" Caption="IdAutoAct" ReadOnly="true" Width="80px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAvans" Name="IdAvans" Caption="IdAvans" ReadOnly="true" Width="80px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Cheie" Name="Cheie" Caption="Cheie" ReadOnly="true" Width="80px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAutoAtasamente" Name="IdAutoAtasamente" Caption="IdAutoAtasamente" ReadOnly="true" Width="80px" Visible="false" ShowInCustomizationForm="false" />

                    </Columns>

                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Renunta">
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
                                <table>
                                    <tr>
                                        <td>Nr document</td>
                                        <td style="padding-left:10px !important;">Data document</td>
                                    </tr>
                                    <tr>
                                        <td><dx:ASPxTextBox ID="txtDocNr" runat="server" Width="100px" Value='<%# Bind("DocNr") %>' /></td>
                                        <td style="padding:10px !important;">
                                            <dx:ASPxDateEdit ID="txtDocData" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" Value='<%# Bind("DocData") %>' >
                                                <CalendarProperties FirstDayOfWeek="Monday" />
                                            </dx:ASPxDateEdit>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                            </div>
                        </EditForm>
                    </Templates>
                    
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>


    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>


</asp:Content>
