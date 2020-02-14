<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.Absente.Lista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnPlanif":
                    grDate.PerformCallback("btnPlanif;0");
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnDivide":
                    grDate.GetRowValues(e.visibleIndex, 'Id;IdStare;DataInceput;DataSfarsit', GoToDivide);
                    //popUpDivide.Show();
                    break;
                case "btnCerere":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCerereMode);
                    break;
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;
            }
        }


        function OnMotivRespingere(s,e)
        {
            if (ASPxClientUtils.Trim(txtMtv.GetText()) == '')
            {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Pentru a putea respinge este nevoie de un motiv"),
                    type: "warning"
                });
            }
            else
            {
                popUpMotiv.Hide();
                grDate.PerformCallback('btnRespinge;' + txtMtv.GetText());
                txtMtv.SetText('');
            }
        }


        function GoToDeleteMode(Value) {

            if (Value == 0 || Value == -1) {
                swal({
                    title: trad_string(limba, 'Operatie nepermisa'), text: trad_string(limba, 'Nu puteti anula o cerere deja anulata sau respinsa'),
                    type: "warning"
                });
            }
            else {
                swal({
                    title: trad_string(limba, "Sunteti sigur/a ?"), text: trad_string(limba, "Cererea va fi anulata !"),
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: trad_string(limba, "Da, continua!"), cancelButtonText: trad_string(limba, "Renunta"), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete;" + Value);
                    }
                });
            }
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=1&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=1&id=' + Value, '_blank ')
        }

        function CloseDeferedWindow() {
            popUpDivide.Hide();
        }


        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, ""), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }

            AdjustSize();
        }

        function GoToDivide(values) {
            if (values[1] != 3) {
                swal({
                    title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Se pot diviza numai cererile in starea aprobat"),
                    type: "warning"
                });
            }
            else {
                if (Math.round(Math.abs((values[3].getTime() - values[2].getTime()) / (24 * 60 * 60 * 1000))) + 1 < 2) {
                    swal({
                        title: trad_string(limba, "Operatie nepermisa"), text: trad_string(limba, "Intervalul ales trebuie sa fie de minim 2 zile"),
                        type: "warning"
                    });
                }
                else {
                    txtDataInc.value = values[2].getTime();
                    txtDataSf.value = values[3].getTime();
                    //txtDataDivide.value = txtDataInc.value;
                    txtDataDivide.SetValue(null);
                    popUpDivide.Show();
                }
            }
        }

        function OKDivideClick(s, e) {
            if (txtDataDivide.GetDate() == null) {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Lipseste data cu care se divide cererea"),
                    type: "warning"
                });
            }
            else {
                var txtDiv = txtDataDivide.GetDate().getTime();
                if (txtDataInc.value <= txtDiv && txtDiv <= txtDataSf.value)
                {
                    txtDataInc.value = null;
                    txtDataSf.value = null;
                    txtDataDivide.value = null;
                    popUpDivide.Hide();
                    e.processOnServer = true;
                }
                else
                {
                    swal({
                        title: trad_string(limba, ""), text: trad_string(limba, "Data nu este in intervalul din cerere"),
                        type: "warning"
                    });

                    e.processOnServer = false;
                }
            }            
        }

        function SetComboViz()
        {
            if (typeof cmbRol !== 'undefined') {
                if (cmbViz.GetValue() == 3)
                {
                    cmbRol.SetEnabled(false);
                    cmbRol.SetValue(-1);
                }
                else
                    cmbRol.SetEnabled(true);
            }
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de respingere ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge; ");
                    }
                });
            }
            else
            {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: trad_string(limba, 'Sunteti sigur/a ?'), text: trad_string(limba, 'Vreti sa continuati procesul de aprobare ?'),
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: trad_string(limba, 'Da, continua!'), cancelButtonText: trad_string(limba, 'Renunta'), closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba;6");
                    }
                });
            }
            else {
                swal({
                    title: trad_string(limba, ""), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }


        function StartUpload() {
            //pnlLoading.Show();
        }

        function EndUpload(s) {
            //pnlLoading.Hide();
            //lblDoc.innerText = s.cpDocUploadName;
            //s.cpDocUploadName = null;
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


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnSolNoua" ClientInstanceName="btnSolNoua" Font-Bold="true" ClientIDMode="Static" runat="server" Text="Solicitare noua" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnIstoricExtins" ClientInstanceName="btnIstoricExtins" ClientIDMode="Static" runat="server" Text="Istoric Extins" AutoPostBack="true" OnClick="btnIstoricExtins_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/istoric.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespinge" ClientInstanceName="btnRespinge" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                       OnRespinge(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) { OnAproba(s,e); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                    <label id="lblViz" runat="server" style="display:inline-block; float:left; padding:0px 15px;"></label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbViz" ClientInstanceName="cmbViz" ClientIDMode="Static" runat="server" Width="150px" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s,e) { SetComboViz(); }" Init="function(s,e) { SetComboViz(); }" />
                        </dx:ASPxComboBox>
                    </div>
                    <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px;">Roluri</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>
                    <label id="lblStare" runat="server" style="display:inline-block; float:left; padding-right:15px;">Stare</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDropDownEdit ClientInstanceName="cmbStare" ID="cmbStare" Width="150px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBoxStare" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="170px">
                                    <Border BorderStyle="None" />
                                    <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />
                                    <Items>
                                        <dx:ListEditItem Text="(Selectie toate)" />
                                        <dx:ListEditItem Text="Solicitat" Value="1" />
                                        <dx:ListEditItem Text="In Curs" Value="2" />
                                        <dx:ListEditItem Text="Aprobat" Value="3" />
                                        <dx:ListEditItem Text="Respins" Value="0" />
                                        <dx:ListEditItem Text="Anulat" Value="-1" />
                                        <dx:ListEditItem Text="Planificat" Value="4" />
                                    </Items>
                                    <ClientSideEvents SelectedIndexChanged="OnListBoxSelectionChanged" />
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
                            <ClientSideEvents TextChanged="SynchronizeListBoxValues" DropDown="SynchronizeListBoxValues" />
                        </dx:ASPxDropDownEdit>
                    </div>
                    <label id="lblDtInc" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Inceput</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit ID="txtDtInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>
                    <label id="lblDtSf" runat="server" style="display:inline-block; float:left; padding-right:15px;">Data Sfarsit</label>
                    <div style="float:left; padding-right:15px;">
                        <dx:ASPxDateEdit ID="txtDtSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>                    
                    <div style="float:left;">
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowUpdating="grDate_RowUpdating" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomButtonInitialize="grDate_CustomButtonInitialize" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="True" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents 
                        CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" 
                        EndCallback="function(s,e) { OnEndCallback(s,e); }"
                        Init="OnInitGrid"  />
                    <Columns>

                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewCommandColumn Width="160px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="~/Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnDivide">
                                    <Image ToolTip="Divide" Url="~/Fisiere/Imagini/Icoane/divide.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnCerere">
                                    <Image ToolTip="Arata cerere" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnPlanif">
                                    <Image ToolTip="Transforma in solicitat" Url="~/Fisiere/Imagini/Icoane/notif.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="250px" VisibleIndex="2" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="Actiune" Name="Actiune" Caption="Actiune" ReadOnly="true" Width="150px" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Rol" Name="Rol" Caption="Rol" ReadOnly="true" Width="150px" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Name="Marca" Caption="Marca" ReadOnly="true" Width="100px" VisibleIndex="3" Settings-AutoFilterCondition="Equals" />
                        <dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="250px" VisibleIndex="4" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="EID" Name="EID" Caption="EID" ReadOnly="true" Width="100px" VisibleIndex="5" Settings-AutoFilterCondition="Equals" />

                        <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" ReadOnly="true" Width="250px" VisibleIndex="6">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" ReadOnly="true" Width="100px" VisibleIndex="7" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" ReadOnly="true" Width="100px" VisibleIndex="8" >
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="NrZile" Name="NrZile" Caption="Nr. zile" ReadOnly="true" Width="70px" VisibleIndex="9" />
                        <dx:GridViewDataTextColumn FieldName="NrOre" Name="NrOre" Caption="Nr. ore" ReadOnly="true" Width="70px" VisibleIndex="10" />
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" Width="250px" VisibleIndex="11" >
                            <EditFormSettings Visible="False"/>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeInlocuitor" Name="NumeInlocuitor" Caption="Inlocuitor" ReadOnly="true" Width="250px" ShowInCustomizationForm="false" VisibleIndex="12" />
                        
                        <dx:GridViewDataTextColumn FieldName="TrimiteLa" Name="TrimiteLa" Caption="Tip aditional solicitare" Width="250px" VisibleIndex="14" />
                        <dx:GridViewDataTextColumn FieldName="Comentarii" Name="Comentarii" Caption="Comentarii" Width="250px" VisibleIndex="15" />
                        <dx:GridViewDataTextColumn FieldName="DateConcatenate" Name="DateConcatenate" Caption="Informatii aditionale" ReadOnly="true" Width="250px" VisibleIndex="16" />
                        
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="17" />
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="18" />
                        <dx:GridViewDataTextColumn FieldName="Inlocuitor" Name="Inlocuitor" Caption="Inlocuitor" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="19" />

                        <dx:GridViewDataTextColumn FieldName="Compensare" Name="Compensare" Caption="Trimite la" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="20" />
                        <dx:GridViewDataTextColumn FieldName="CompensarePlata" Name="CompensarePlata" Caption="Trimite la plata" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="21" />
                        <dx:GridViewDataTextColumn FieldName="CompensarePlataDenumire" Name="CompensarePlataDenumire" Caption="Trimite la plata" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="22" />
                        <dx:GridViewDataTextColumn FieldName="CompensareBanca" Name="CompensareBanca" Caption="Trimite la banca" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="23" />
                        <dx:GridViewDataTextColumn FieldName="CompensareBancaDenumire" Name="CompensareBancaDenumire" Caption="Trimite la banca" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="24" />

                        <dx:GridViewDataTextColumn FieldName="AdaugaAtasament" Name="AdaugaAtasament" Caption="Adauga Atasament" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="25" />

                        <dx:GridViewDataTextColumn FieldName="Anulare_Valoare" Name="Anulare_Valoare" Caption="Anulare - tip" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="26" />
                        <dx:GridViewDataTextColumn FieldName="Anulare_NrZile" Name="Anulare_NrZile" Caption="Anulare - nr zile" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="27" />

                        <dx:GridViewDataTextColumn FieldName="CampBifa" Name="CampBifa" Caption="CampBifa" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false" VisibleIndex="28" />

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
                                        <td>Observatii</td>
                                        <td style="padding-left:10px !important;">Comentarii</td>
                                    </tr>
                                    <tr>
                                        <td><dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="150" Text='<%# Bind("Observatii") %>' OnInit="oObservatiiMemo_Init" /></td>
                                        <td style="padding:10px !important;"><dx:ASPxMemo ID="txtCom" runat="server" Width="500px" Height="150" Text='<%# Bind("Comentarii") %>' OnInit="comentariiMemo_Init" /></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <div style="float:left; margin-right:230px; margin-bottom:15px;">
                                                <dx:ASPxLabel ID="lblInl" runat="server" ClientIDMode="Static" ClientInstanceName="lblInl" Text="Inlocuitor" CssClass="label_left"></dx:ASPxLabel>
                                                <dx:ASPxComboBox ID="cmbInl" runat="server" ClientInstanceName="cmbInl" ClientIDMode="Static" Width="215px" ValueField="F10003" DropDownWidth="200" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" />
                                            </div>
                                            <div>
                                                <dx:ASPxLabel ID="lblCps" runat="server" ClientIDMode="Static" ClientInstanceName="lblCps" Text="Tip aditional solicitare" CssClass="label_left"></dx:ASPxLabel>
                                                <dx:ASPxComboBox ID="cmbCps" runat="server" ClientInstanceName="cmbCps" ClientIDMode="Static" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <dx:ASPxUploadControl ID="btnDocUpload" runat="server" ClientIDMode="Static" ShowProgressPanel="true" Height="28px"
                                                BrowseButton-Text="Incarca Document" FileUploadMode="OnPageLoad" UploadMode="Advanced" AutoStartUpload="true" ToolTip="incarca document" ShowTextBox="false"
                                                ClientInstanceName="UploadImage" OnFileUploadComplete="btnDocUpload_FileUploadComplete" ValidationSettings-ShowErrors="false" meta:resourcekey="btnDocUploadResource1">
                                                <BrowseButton>
                                                    <Image Url="../Fisiere/Imagini/Icoane/incarca.png"></Image>
                                                </BrowseButton>
                                                <ValidationSettings ShowErrors="False"></ValidationSettings>

                                                <ClientSideEvents FilesUploadStart="StartUpload" FileUploadComplete="function(s,e) { EndUpload(s); }" />
                                            </dx:ASPxUploadControl>
                                            <dx:ASPxCheckBox ID="chkBifa" runat="server" Text="Camp bifa" Checked='<%#DataBinder.GetPropertyValue(Container.DataItem,"CampBifa").ToString()=="1"%>'></dx:ASPxCheckBox>
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

    
    <dx:ASPxPopupControl ID="popUpDivide" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpDivideArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="300px" Height="200px" HeaderText="Alege data de divizare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpDivide" EnableHierarchyRecreation="false">
            <ContentCollection>
                <dx:PopupControlContentControl runat="server" SupportsDisabledAttribute="True">
                    <dx:ASPxPanel ID="pnlDivide" runat="server" DefaultButton="btnOKDivide">
                        <PanelCollection>
                            <dx:PanelContent runat="server">
                                <table width="100%">
                                    <tr style="float:right; text-align:right; margin-bottom:30px;">
                                        <td>
                                            <dx:ASPxButton ID="btnOKDivide" runat="server" Text="Divide" AutoPostBack="false" OnClick="btnOKDivide_Click" HorizontalAlign="Right" oncontextMenu="ctx(this,event)" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/divide.png"></Image>
                                                <ClientSideEvents Click="function (s,e) { OKDivideClick(s,e); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                    <tr align="center">
                                        <td>
                                            <dx:ASPxDateEdit ID="txtDataDivide" ClientIDMode="Static" ClientInstanceName="txtDataDivide" runat="server" Width="120px">
                                                <CalendarProperties FirstDayOfWeek="Monday" />
                                            </dx:ASPxDateEdit>
                                            <br />
                                            <dx:ASPxCheckBox ID="chkAnulare" ClientInstanceName="chkAnulare" runat="server" Text="Anulare concediu incepand cu aceasta data" TextAlign="Right" />
                                            <dx:ASPxHiddenField ID="txtDataSf" runat="server" ClientIDMode="Static" ClientInstanceName="txtDataSf" />
                                            <dx:ASPxHiddenField ID="txtDataInc" runat="server" ClientIDMode="Static" ClientInstanceName="txtDataInc" />
                                        </td>
                                    </tr>
                                </table>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxPanel>
                </dx:PopupControlContentControl>
            </ContentCollection>
    </dx:ASPxPopupControl>


    <dx:ASPxPopupControl ID="popUpMotiv" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpMotivArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="650px" Height="200px" HeaderText="Motiv respingere"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpMotiv" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRespingeMtv" runat="server" Text="Respinge" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnMotivRespingere(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="color: #666666;font-family: Tahoma; font-size: 10px;">
                                <dx:ASPxMemo ID="txtMtv" runat="server" ClientIDMode="Static" ClientInstanceName="txtMtv" Width="630px" Height="180px"></dx:ASPxMemo>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>


    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>


</asp:Content>
