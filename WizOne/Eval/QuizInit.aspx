<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Cadru.Master" CodeBehind="QuizInit.aspx.cs" Inherits="WizOne.Eval.QuizInit" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server" >
    <script language="javascript" type="text/javascript">
        function OnInit(s, e) {            
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de initiere ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnInit;6");
                    }
                });
            }
            else {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
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
    <table style="width:100%;">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnModifEval" ClientInstanceName="btnModifEval" ClientIDMode="Static" runat="server" Text="Modif. Evaluator" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpModif.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" runat="server" ClientInstanceName="btnInit" Text="Initiaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e){
                        OnInit(s,e);
                        }" />
                    <Image Url="../Fisiere/Imagini/Icoane/adauga.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" 
                    PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu ="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <table>
        <tr>
            <td style="padding-right:10px;">
                <dx:ASPxLabel ID="lblQuiz" Text="Chestionar" runat="server" />
            </td>
            <td style="padding-right:20px;">
                <dx:ASPxComboBox Width="300" ID="cmbQuiz" ClientInstanceName="cmbQuiz" ClientIDMode="Static" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" DropDownWidth="500">
                    <Columns>
                        <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="50px" />
                        <dx:ListBoxColumn FieldName="Denumire" Caption="Denumire" Width="450px" />
                    </Columns>                
                </dx:ASPxComboBox>
            </td>
            <td style="padding-right:10px;">
                <dx:ASPxLabel ID="lblAngajat" runat="server" Text="Angajat" />
            </td>
            <td style="padding-right:20px;">
                <dx:ASPxComboBox ID="cmbAngajat" Width="200" runat="server" ClientInstanceName="cmbAngajat" ClientIDMode="Static" DropDownStyle="DropDown" TextField="NumeComplet" ValueField="F10003" AutoPostBack="false" ValueType="System.Int32" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}">
                    <Columns>
                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="50px" />
                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="230px" />
                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="100px" />
                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                        <dx:ListBoxColumn FieldName="Departament" Caption="Departament" Width="130px" />
                    </Columns>
                </dx:ASPxComboBox>
            </td>
            <td style="padding-right:10px;">
                <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/lupa.png" />
                </dx:ASPxButton>
            </td>
            <td>
                <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge filtru" OnClick="btnFiltruSterge_Click" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/lupaDel.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <br />
    <table>
        <tr>
            <td colspan="2">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static"
                    AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" 
                    OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true"
                        ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="true" HorizontalScrollBarMode="Auto" />
                    <SettingsEditing Mode="EditFormAndDisplayRow" />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />  
                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                        <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare" ReadOnly="true" Width="100px" VisibleIndex="1" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Angajat" ReadOnly="true" Width="250px" VisibleIndex="2" />
                        <dx:GridViewDataTextColumn FieldName="Quiz" Name="Quiz" Caption="Chestionar" ReadOnly="true" Width="500px" VisibleIndex="3" />
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" ReadOnly="true" Width="100px" VisibleIndex="4">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" ReadOnly="true" Width="100px" VisibleIndex="5">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="F10003" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Finalizat" Name="Finalizat" Caption="Finalizat" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Culoare" Name="Culoare" Caption="Culoare" ReadOnly="true" Visible="false" />
                    </Columns>
                </dx:ASPxGridView>
            </td>
        </tr>
    </table>

    <dx:ASPxPopupControl ID="popUpModif" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top" Modal="true"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="450px" Height="220px" HeaderText="Modificare evaluator"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpModif" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnModif" runat="server" Text="Modif. Evaluator" AutoPostBack="true" OnClick="btnModif_Click">
                                    <ClientSideEvents Click="function(s, e) {
                                        popUpModif.Hide();
                                        pnlLoading.Show();
                                        e.processOnServer = true;
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%;">
                                <dx:ASPxLabel ID="lblQuizModif" runat="server" Text="Chestionar" />
                                <dx:ASPxComboBox ID="cmbQuizModif" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" Width="100%" CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}">
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Id" Caption="Id" Width="70px" />
                                        <dx:ListBoxColumn FieldName="Denumire" Caption="Denumire" Width="250px" />
                                    </Columns>
                                </dx:ASPxComboBox>
                                <br /><br />
                                <dx:ASPxLabel ID="lblUserOld" runat="server" Text="Evaluator vechi" />
                                <dx:ASPxComboBox ID="cmbUserOld" runat="server" DropDownStyle="DropDown" TextField="F70104" ValueField="F70102" ValueType="System.Int32" AutoPostBack="false" Width="100%"/>
                                <br /><br />
                                <dx:ASPxLabel ID="lblUserNew" runat="server" Text="Evaluator nou" />
                                <dx:ASPxComboBox ID="cmbUserNew" runat="server" DropDownStyle="DropDown" TextField="F70104" ValueField="F70102" ValueType="System.Int32" AutoPostBack="false" Width="100%"/>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>