<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ZLP.aspx.cs" Inherits="WizOne.Absente.ZLP" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function GoToCalculationMode(s, e) {
            var txt = 'Vreti sa faceti calculul pentru angajatul selectat ?';
            if (cmbAng.GetSelectedIndex() == -1) txt = 'Sunteti sigur ca doriti calcul pentru toti angajatii ?';

            swal({
                title: '', text: txt,
                type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da', cancelButtonText: 'Renunta', closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback(s.name);
                }
            });
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
            var height = Math.max(0, document.documentElement.clientHeight) - 180;
            grDate.SetHeight(height);
        }

        function OnEndCallback(s, e) {
            AdjustSize();
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
                        <dx:ASPxButton ID="btnZLP" ClientInstanceName="btnZLP" ClientIDMode="Static" runat="server" Text="Calcul ZLP" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function (s, e) { GoToCalculationMode(s, e); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnSIZLP" ClientInstanceName="btnSIZLP" ClientIDMode="Static" runat="server" Text="Calcul SI ZLP" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function (s, e) { GoToCalculationMode(s, e); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function (s, e) { OnNewClick(s, e); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
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
                <tr>
                    <td colspan="2">
                        <div style="display:inline-block; line-height:22px; vertical-align:middle; padding:15px 0px 15px 0px;">
                            <label id="lblAn" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Anul</label>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbAn" ClientInstanceName="cmbAn" ClientIDMode="Static" runat="server" Width="100px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                            </div>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" ClientIDMode="Static" runat="server" Width="100px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                            </div>
                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajat</label>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                            CallbackPageSize="15" AllowNull="true" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                    </Columns>
                                </dx:ASPxComboBox>
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
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%"
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomCallback="grDate_CustomCallback" >
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="False" ShowGroupPanel="False" VerticalScrollBarMode="Visible" />
                            <SettingsSearchPanel Visible="False" />
                            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" Init="OnInitGrid" ContextMenu="ctx" />

                            <Columns>
                                <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" >
                                      <Settings SortMode="DisplayText" />
                                    <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="An" Name="An" Caption="An">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>

                                <dx:GridViewDataTextColumn FieldName="Cuvenite" Name="Cuvenite" Caption="Cuvenite" ReadOnly="true" >
                                    <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N0" MaskSettings-IncludeLiterals="None" MaskSettings-PromptChar="c">
                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" />
                                    </PropertiesTextEdit>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="SoldAnterior" Name="SoldAnterior" Caption="Sold Anterior" >
                                    <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N1" MaskSettings-IncludeLiterals="None">
                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" />
                                    </PropertiesTextEdit>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Efectuate" Name="Efectuate" Caption="Efectuate" >
                                    <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N1" MaskSettings-IncludeLiterals="None">
                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" />
                                    </PropertiesTextEdit>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="CuveniteAn" Name="CuveniteAn" Caption="Cuvenite An" ReadOnly="true" >
                                    <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N0" MaskSettings-IncludeLiterals="None">
                                        <ValidationSettings ErrorDisplayMode="ImageWithTooltip" />
                                    </PropertiesTextEdit>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" />
                                <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                            </Columns>

                            <SettingsCommandButton>
                                <UpdateButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </UpdateButton>
                                <CancelButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                </CancelButton>

                                <EditButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                    <Styles>
                                        <Style Paddings-PaddingRight="5px" />
                                    </Styles>
                                </EditButton>
                                <DeleteButton>
                                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                </DeleteButton>
                            </SettingsCommandButton>
                        </dx:ASPxGridView>
                        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid"></dx:ASPxGridViewExporter>
                    </td>
                </tr>
            </table>

    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>
