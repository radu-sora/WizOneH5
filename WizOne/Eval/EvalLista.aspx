<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Cadru.Master" CodeBehind="EvalLista.aspx.cs" Inherits="WizOne.Eval.EvalLista" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">
        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnEdit":
                    hfVisibleIndex.Set('Index', e.visibleIndex);
                    grDate.GetRowValues(e.visibleIndex, 'IdAuto', GoToEditMode);
                    break;
                case "btnIstoric":
                    grDate.GetRowValues(e.visibleIndex, 'IdAuto', GoToIstoric);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToIstoric(Value) {
            strUrl = getAbsoluteUrl + "Pagini/Istoric.aspx?tip=6&qwe=" + Value;
            popGen.SetHeaderText("Istoric");
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
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">     
                <dx:ASPxButton ID="btnModif" ClientInstanceName="btnModif" ClientIDMode="Static" runat="server" Text="Modifica stare" AutoPostBack="false" oncontextMenu="ctx(this, event)"  >
                    <ClientSideEvents Click="function(s, e) {
                        grDate.PerformCallback('btnModif');
                    }" />
                    <Image Url="../Fisiere/Imagini/Icoane/renunta.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true"
                    PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <dx:ASPxLabel Width="80" ID="lblQuiz" Text="Chestionar:" runat="server" />
            </td>
            <td width="180">
                <dx:ASPxComboBox Width="150" ID="cmbQuiz" ClientInstanceName="cmbQuiz" ClientIDMode="Static" runat="server" DropDownStyle="DropDown"
                    TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" />
            </td>
            <td>
                <dx:ASPxLabel ID="lblAngajat" Width="80" runat="server" Text="Angajat:" />
            </td>
            <td width="130">
                <dx:ASPxComboBox ID="cmbAngajat" Width="100" runat="server" ClientInstanceName="cmbAngajat" ClientIDMode="Static" DropDownStyle="DropDown"
                    TextField="NumeComplet" ValueField="F10003" AutoPostBack="false" ValueType="System.Int32" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}">
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Departament" Width="130px" />
                    </Columns>
                </dx:ASPxComboBox>
            </td>
            <td>
                <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru:" OnClick="btnFiltru_Click" oncontextMenu="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/lupa.png" />
                </dx:ASPxButton>
            </td>
            <td>
                <dx:ASPxLabel ID="lblIntre" runat="server" Width="20" />
            </td>
            <td width="130">
                <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge filtru:" OnClick="btnFiltruSterge_Click" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/lupaDel.png" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel Width="80" ID="lblDataInceput" runat="server" Text="Data inceput:" />
            </td>
            <td width="180">
                <dx:ASPxDateEdit ID="dtDataInceput" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" AutoPostBack="false" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>
            </td>
            <td>
                <dx:ASPxLabel Width="80" ID="lblDataSfarsit" runat="server" Text="Data sfarsit:" />
            </td>
            <td width="130">
                <dx:ASPxDateEdit ID="dtDataSfarsit" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" AutoPostBack="false" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>
            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxLabel Width="80" ID="lblNivel" Visible="false" runat="server" Text="Nivelul: " />
            </td>
            <td width="180"> 
                <dx:ASPxComboBox Width="150" ID="cmbNivel" ClientInstanceName="cmbNivel" ClientIDMode="Static" runat="server" Visible="false" DropDownStyle="DropDown"
                    TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" />
            </td>
            <td>
                <dx:ASPxLabel Width="80" ID="lblRoluri" Visible="false" runat="server" Text="Roluri: " />
            </td>
            <td width="130">
                <dx:ASPxComboBox Width="150" ID="cmbRoluri" ClientInstanceName="cmbRoluri" ClientIDMode="Static" runat="server" Visible="false" DropDownStyle="DropDown"
                    TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" />
            </td>
        </tr>
    </table>

    <table width="100%">
        <tr>
            <td colspan="2">
                <dx:ASPxHiddenField runat="server" ID="hfVisibleIndex" ClientInstanceName="hfVisibleIndex" />
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%"
                    AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback"
                    OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true"
                        ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="false" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" ShowFilterBar="Visible" />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="OnEndCallback"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="60px" VisibleIndex="0" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid">
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnIstoric">
                                    <Image ToolTip="Istoric" Url="../Fisiere/Imagini/Icoane/motive.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Completeaza" Url="../Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="Id" VisibleIndex="0" Width="50px" ReadOnly="true">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" VisibleIndex="1" Width="450px" ReadOnly="true">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare" Width="250px" ReadOnly="true" VisibleIndex="2">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Utilizator" Name="Utilizator" Caption="Utilizator" VisibleIndex="3" Width="200px" ReadOnly="true">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="DenumireCategorie" Name="DenumireCategorie" Caption="Categorie" Width="150px" ReadOnly="true" Visible="true" VisibleIndex="4">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="DenPerioada" Name="DenPerioada" Caption="Interval" Width="170px" ReadOnly="true" Visible="true" VisibleIndex="5">
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="Rating" Name="Rating" Caption="Rating" VisibleIndex="7" ReadOnly="true">
                            <PropertiesComboBox TextField="Denumire" ValueField="Nota" ValueType="System.Int32" DropDownStyle="DropDown"/>
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Observatii" Name="Observatii" Caption="Observatii" Width="250px" VisibleIndex="8" ReadOnly="true">
                            <EditFormSettings Visible="False" />
                            <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" />
                            <SettingsHeaderFilter Mode="CheckedList" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" Width="50px" ReadOnly="true" />   
                        <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="PozitiePeCircuit" Name="PozitiePeCircuit" Caption="PozitiePeCircuit" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Finalizat" Name="Finalizat" Caption="Finalizat" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="CategorieQuiz" Name="CategorieQuiz" Caption="CategorieQuiz" ReadOnly="true" Visible="false" />
                    </Columns>

                    <Templates>
                        <EditForm>
                            <div style="padding: 4px 3px 4px">
                                <table>
                                    <tr>
                                        <td>Observatii</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="150" Text='<%# Bind("Observatii") %>' OnInit="txtObs_Init" />
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
</asp:Content>