<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.Organigrama.Lista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dx:ASPxHiddenField ID="hf" runat="server" ClientIDMode="Static" ClientInstanceName="hf" />
    <table style="width:100%">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnExport" ClientInstanceName="btnExport" ClientIDMode="Static" runat="server" Text="Diagrama" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s,e) { grDate.PerformCallback(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDuplicare" ClientInstanceName="btnDuplicare" ClientIDMode="Static" runat="server" Text="Duplicare" OnClick="btnDuplicare_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/duplicare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnModifStruc" ClientInstanceName="btnModifStruc" ClientIDMode="Static" runat="server" Text="Modifica" AutoPostBack="true" OnClick="btnModif_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/schimba.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNou" ClientInstanceName="btnNou" ClientIDMode="Static" runat="server" Text="Adauga" OnClick="btnNou_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                    <label id="lblDtVig" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Data selectie</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit id="txtDtVig" ClientIDMode="Static" ClientInstanceName="txtDtVig" runat="server" DisplayFormatString="dd/MM/yyyy" EditFormat="Date" EditFormatString="dd/MM/yyyy" Width="100px" />
                    </div>

                    <div style="display:none;">
                        <label id="lblActiv" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Post activ</label>
                        <div style="float:left; padding-right:15px;">
                            <dx:ASPxCheckBox id="chkActiv" runat="server" TextAlign="Left" />
                        </div>
                    </div>

                    <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajati</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDropDownEdit ClientInstanceName="cmbStare" ID="cmbStare" Width="150px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBoxStare" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                                    <Border BorderStyle="None" />
                                    <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                    <Items>
                                        <dx:ListEditItem Text="(Selectie toti)" Value="4" />
                                        <dx:ListEditItem Text="Activi" Value="1" />
                                        <dx:ListEditItem Text="Activi suspendati" Value="2" />
                                        <dx:ListEditItem Text="Candidati" Value="3" />
                                    </Items>
                                    <ClientSideEvents SelectedIndexChanged="function(s, e){ OnListBoxSelectionChanged(s,e) }" />
                                </dx:ASPxListBox>
                                <table style="width: 100%">
                                    <tr>
                                        <td style="padding: 4px">
                                            <dx:ASPxButton ID="btnInchide" AutoPostBack="False" runat="server" Text="Inchide" style="float: right">
                                                <ClientSideEvents Click="function(s, e){ cmbStare.HideDropDown(); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </DropDownWindowTemplate>
                            <ClientSideEvents TextChanged="function(s, e){ SynchronizeListBoxValues(s,e) }" DropDown="function(s, e){ SynchronizeListBoxValues(s,e) }"/>
                        </dx:ASPxDropDownEdit>
                    </div>

                    <label id="lblParinte" runat="server" style="display:inline-block; float:left; padding-right:15px;">Superior</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbParinte" runat="server" Width="130px" ValueField="Camp" TextField="Denumire" />
                    </div>

                    <div style="float:left;">
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExpand" runat="server" Text="Expand" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s,e) { grDate.ExpandAll(); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/stare.png"></Image>
                        </dx:ASPxButton>
                    </div>
                    <div style="float:left; line-height:16px; vertical-align:middle; margin-top:5px;">
                        <label id="lgActiv" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Legenda angajati: Activ</label>
                        <div style="width:16px; height:16px; background-color:#c8ffc8;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                        <label id="lgSuspendat" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Activ suspendat</label>
                        <div style="width:16px; height:16px; background-color:#ffffc8;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                        <label id="lgCandidati" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Candidat</label>
                        <div style="width:16px; height:16px; background-color:#96c8fa;float:left; margin-left:0px; border:solid 2px #e6e6e6;"></div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <table style="width:100%">
        <tr>
            <td colspan="2">
                <dx:ASPxTreeList ID="grDate" runat="server" ClientInstanceName="grDate" SettingsEditing-AllowNodeDragDrop="true" Height="100%" KeyFieldName="Id" AutoGenerateColumns="true" OnHtmlRowPrepared="grDate_HtmlRowPrepared" OnCustomCallback="grDate_CustomCallback" >
                    <SettingsBehavior AllowFocusedNode="true" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Enabled="true" />
                    <SettingsEditing AllowNodeDragDrop="true" />
                    <Settings GridLines="Both" />
                    <ClientSideEvents EndDragNode="function(s, e) { OnEndDragNode(s,e); }" EndCallback="function(s,e) { onGridEndCallback(s); }" />
                    <Columns>
                        
                        <dx:TreeListDataColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" ReadOnly="true" Width="150px" VisibleIndex="1" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />
                        <dx:TreeListDataColumn FieldName="Id" Name="Cod" Caption="Cod" ReadOnly="true" Width="150px" VisibleIndex="2" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />
                        <dx:TreeListCheckColumn FieldName="Activ" Caption="Activ" Width="50px" VisibleIndex="3" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" Visible="false" ShowInCustomizationForm="false"  />

                        <dx:TreeListDataColumn FieldName="PozitiiPlanificate" Name="PozitiiPlanificate" Caption="Planificate" ReadOnly="true" Width="70px" VisibleIndex="15" />
                        <dx:TreeListDataColumn FieldName="PozitiiAprobate" Name="PozitiiAprobate" Caption="Aprobate" ReadOnly="true" Width="70px" VisibleIndex="16" />
                        <dx:TreeListDataColumn FieldName="AngajatiActivi" Name="AngajatiActivi" Caption="Activi" ReadOnly="true" Width="70px" VisibleIndex="17" />
                        <dx:TreeListDataColumn FieldName="AngajatiSuspendati" Name="AngajatiSuspendati" Caption="Activ suspendat" ReadOnly="true" Width="70px" VisibleIndex="18" />
                        <dx:TreeListDataColumn FieldName="Candidati" Name="Candidati" Caption="Candidati" ReadOnly="true" Width="70px" VisibleIndex="19" />

                        <dx:TreeListDataColumn FieldName="Companie" Name="Companie" Caption="Companie" ReadOnly="true" Width="250px" VisibleIndex="5" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList"/>
                        <dx:TreeListDataColumn FieldName="Subcompanie" Name="Subcompanie" Caption="Subcompanie" ReadOnly="true" Width="250px" VisibleIndex="6" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />
                        <dx:TreeListDataColumn FieldName="Filiala" Name="Filiala" Caption="Filiala" ReadOnly="true" Width="250px" VisibleIndex="7" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />
                        <dx:TreeListDataColumn FieldName="Sectie" Name="Sectie" Caption="Sectie" ReadOnly="true" Width="250px" VisibleIndex="8" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />
                        <dx:TreeListDataColumn FieldName="Dept" Name="Dept" Caption="Dept" ReadOnly="true" Width="250px" VisibleIndex="9" AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" SettingsHeaderFilter-Mode="CheckedList" />

                        <dx:TreeListDataColumn FieldName="IdSuperior" Visible="false" ShowInCustomizationForm="false" VisibleIndex="10" />
                        <dx:TreeListDataColumn FieldName="IdAuto" Visible="false" ShowInCustomizationForm="false" VisibleIndex="19" />
                        <dx:TreeListDataColumn FieldName="StareAngajat" Visible="false" ShowInCustomizationForm="false" VisibleIndex="20" />
                    </Columns>
                </dx:ASPxTreeList>

            </td>
        </tr>
    </table>


    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter" OnWindowCallback="popUpMotiv_WindowCallback"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="450px" Height="170px" HeaderText="Motivul modificarii"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ClientSideEvents EndCallback ="function(s,e) { onPopUpEndCallback(s); }" />
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td colspan="2" align="right">
                                <dx:ASPxButton ID="btnOkModif" ClientInstanceName="btnOkModif" runat="server" AutoPostBack="false" Text="Modifica">
                                    <ClientSideEvents Click="function(s, e) { OnModifStruc(s,e); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/creion.png"></Image>
                                </dx:ASPxButton>
                                &nbsp;&nbsp;&nbsp;
                                <dx:ASPxButton ID="btnInchide" runat="server" Text="Renunta" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) { popUpMotiv.Hide(); }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                                </dx:ASPxButton>
                                <br /><br />
                            </td>
                        </tr>
                        <tr>
                            <td><span id="lblMotiv" runat="server">Alege Motivul</span></td>
                            <td>
                                <dx:ASPxComboBox ID="cmbMotiv" ClientInstanceName="cmbMotiv" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"><br /><dx:ASPxCheckBox ID="chkStruc" ClientInstanceName="chkStruc" ClientIDMode="Static" runat="server" Text="Doriti modificarea structurii organizatorice cu cea a noului post superior" /></td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <script type="text/javascript">

        function OnEndDragNode(s, e) {
            var jsDate = txtDtVig.GetDate();

            if (jsDate.getDate() == 1) {
                grDate.GetNodeValues(e.nodeKey, "IdAuto;AngajatiActivi", GetNodeValueOri);

                var nodeKeys = s.GetVisibleNodeKeys();
                for (var i = 0; i < nodeKeys.length; i++) {
                    if (s.GetNodeHtmlElement(nodeKeys[i]) == e.targetElement) {
                        var targetNodeKey = nodeKeys[i];
                        grDate.GetNodeValues(targetNodeKey, "IdAuto", GetNodeValueDes);
                        break;
                    }
                }
            }
            else {
                swal({
                    title: "Operatie nepermisa", text: "Data de selectie trebuie sa fie prima zi din luna !",
                    type: "warning"
                });
            }

            e.cancel = true;
        }

        function GetNodeValueOri(selectedValues) {
            hf.Set("Nod", selectedValues);
        }
        function GetNodeValueDes(selectedValues) {
            hf.Set("Target", selectedValues);
            popUpMotiv.Show();
        }

        function OnModifStruc(s, e) {
            if (!cmbMotiv.GetValue("Id")) {
                //return false;
                swal({
                    title: "Operatie nepermisa", text: "Pentru a putea modifica este nevoie de un motiv",
                    type: "warning"
                });
            }
            else {
                if (hf.Get("Nod")[1] != "" && chkStruc.GetValue()) {
                    swal({
                        title: "Atentie", text: "Postul are unul sau mai multi angajati alocati si din acest motiv nu se poate actualiza structura.",
                        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, continua!", cancelButtonText: "Renunta", closeOnConfirm: true
                    }, function (isConfirm) {
                            if (isConfirm) {
                                chkStruc.SetValue(false);
                                //popUpMotiv.Hide();
                                //return true;
                                popUpMotiv.PerformCallback();
                        }
                    });
                }
                else
                    popUpMotiv.PerformCallback();
                    //return true;
            }

            popUpMotiv.Hide();
        }

        function OnOkLevel(s, e) {
            popUpLevel.Hide();
        }

        function onGridEndCallback(s) {
            if (s.cpReportUrl) {
                window.location.href = s.cpReportUrl;
                delete s.cpReportUrl;
            }
        }

        function onPopUpEndCallback(s) {
            grDate.PerformCallback('PopUp');
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

    </script>

</asp:Content>
