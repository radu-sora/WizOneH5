<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.CereriDiverse.Lista" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">

        var limba = "<%= Session["IdLimba"] %>";

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToIstoric);
                    break;
                case "btnCerere":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCerereMode);
                    break;
                case "btnAtasament":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToAtasMode);
                    break;
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
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=3&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
        }

        function GoToCerereMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=1&tbl=1&id=' + Value, '_blank ')
        }

        function GoToAtasMode(Value) {
            window.open(getAbsoluteUrl + 'Pagini/Fisiere.aspx?tip=0&tbl=2&id=' + Value, '_blank ')
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
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
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
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
                    title: trad_string(limba, "Atentie !"), text: trad_string(limba, "Nu exista linii selectate"),
                    type: "warning"
                });
            }
        }

        function EmptyFields(s, e) {
            cmbViz.SetValue(null);
            cmbTip.SetValue(null);
            cmbRol.SetValue(null);

            txtDataInc.SetValue(null);
            txtDataSf.SetValue(null);
            cmbStare.SetValue(null);

            //pnlCtl.PerformCallback('EmptyFields');
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


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" ClientInstanceName="btnRespinge" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                       OnRespinge(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        OnAproba(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSolNoua" ClientInstanceName="btnSolNoua" ClientIDMode="Static" runat="server" Text="Solicitare noua" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="Absente_divOuter" style="margin:15px 0px;">
				
					<div class="Absente_Cereri_CampuriSup">
						<label id="lblViz" runat="server" style="display:inline-block;">Vizualizare</label>
                        <dx:ASPxComboBox ID="cmbViz" ClientInstanceName="cmbViz" ClientIDMode="Static" runat="server" Width="150px" AutoPostBack="false" />
                    </div>

					<div class="Absente_Cereri_CampuriSup">
						<label id="lblTip" runat="server" style="display:inline-block;">Tip cerere</label>
                        <dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>
					
					<div class="Absente_Cereri_CampuriSup">
						<label id="lblRol" runat="server" style="display:inline-block;">Rol</label>
                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>
					
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDataInc" runat="server" style="display:inline-block;">Data Inceput</label>
                        <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataIncResource1" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(3); }" />
                        </dx:ASPxDateEdit>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDataSf" runat="server" style="display:inline-block;">Data Sfarsit</label>
                        <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" meta:resourcekey="txtDataSfResource1" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback(6); }" />
                        </dx:ASPxDateEdit>
                    </div>

					<div class="Absente_Cereri_CampuriSup">
						<label id="lblStare" runat="server" style="display:inline-block;">Stare</label>
                        <dx:ASPxDropDownEdit ClientInstanceName="cmbStare" ID="cmbStare" Width="150px" runat="server" AnimationType="None">
                            <DropDownWindowStyle BackColor="#EDEDED" />
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" runat="server" Height="200px">
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
                                            <dx:ASPxButton ID="ASPxButton1" AutoPostBack="False" runat="server" Text="Inchide" style="float: right">
                                                <ClientSideEvents Click="function(s, e){ cmbStare.HideDropDown(); }" />
                                            </dx:ASPxButton>
                                        </td>
                                    </tr>
                                </table>
                            </DropDownWindowTemplate>
                            <ClientSideEvents TextChanged="SynchronizeListBoxValues" DropDown="SynchronizeListBoxValues" />
                        </dx:ASPxDropDownEdit>						
					</div>
										
					<div style="float:left; padding:30px 15px 0px 15px;">
						<dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
							<Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
							<ClientSideEvents Click="function(s, e) {
											pnlLoading.Show();
											e.processOnServer = true;
										}" />
						</dx:ASPxButton>
					</div>

					<div style="float:left; padding:30px 15px 0px 15px;">
						<dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
							<Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
							<ClientSideEvents Click="EmptyFields" />
						</dx:ASPxButton>
					</div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnRowUpdating="grDate_RowUpdating" OnCustomCallback="grDate_CustomCallback" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCommandButtonInitialize="grDate_CommandButtonInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="True" HorizontalScrollBarMode="Auto" ShowFilterRowMenu="true"  />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
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
                                <dx:GridViewCommandColumnCustomButton ID="btnCerere">
                                    <Image ToolTip="Arata cerere" Url="~/Fisiere/Imagini/Icoane/arata.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnAtasament">
                                    <Image ToolTip="Arata atasamentul" Url="~/Fisiere/Imagini/Icoane/view.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

						<dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="70px" VisibleIndex="2"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="250px" VisibleIndex="3" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            <Settings FilterMode="DisplayText" />
                        </dx:GridViewDataComboBoxColumn>
						<dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="NumeAngajat" Caption="Angajat" ReadOnly="true" Width="200px" VisibleIndex="4" Settings-AutoFilterCondition="Contains" />
						<dx:GridViewDataTextColumn FieldName="TipCerere" Name="TipCerere" Caption="Tip Cerere" ReadOnly="true" Width="150px" VisibleIndex="5" />
						<dx:GridViewDataTextColumn FieldName="Descriere" Name="Descriere" Caption="Cerere" ReadOnly="true" Width="350px" VisibleIndex="6" />
						<dx:GridViewDataTextColumn FieldName="Raspuns" Name="Raspuns" Caption="Raspuns" ReadOnly="true" Width="350px" VisibleIndex="7" />
                        <dx:GridViewDataBinaryImageColumn FieldName="Atasament" Visible="false" Width="150">
                            <PropertiesBinaryImage ImageHeight="150" ImageWidth="225" EnableServerResize="True">
                                <EditingSettings Enabled="true" UploadSettings-UploadValidationSettings-MaxFileSize="4194304"/>
                            </PropertiesBinaryImage>
                        </dx:GridViewDataBinaryImageColumn>  						
						
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
                                        <td>Raspuns</td>
                                        <td>Atasament</td>
                                    </tr>
                                    <tr>
                                        <td><dx:ASPxMemo ID="txtRsp" runat="server" Width="500px" Height="150" Text='<%# Bind("Raspuns") %>' OnInit="oRaspunsMemo_Init" /></td>
                                        <td>
                                            <dx:ASPxBinaryImage ID="binImg" ClientInstanceName="binImg" Width="200" Height="200" ShowLoadingImage="true"  OnValueChanged="binImg_ValueChanged"  runat="server" >                                         
                                                <EditingSettings Enabled="true">
                                                    <UploadSettings>
                                                        <UploadValidationSettings MaxFileSize="4194304"></UploadValidationSettings>
                                                    </UploadSettings>
                                                </EditingSettings>
                                            </dx:ASPxBinaryImage>
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

    

</asp:Content>
