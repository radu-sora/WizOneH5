<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Invitatie.aspx.cs" Inherits="WizOne.Eval.Invitatie" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">
    
        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'IdStare', GoToDeleteMode);
                    break;
            }
        }


        function GoToDeleteMode(Value) {
            if (Value == 0 || Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata sau respinsa",
                    type: "warning"
                });
            }
            else {
                swal({
                    title: "Sunteti sigur/a ?", text: "Invitatia va fi anulata !",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, anuleaza!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete");
                    }
                });
            }
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnTrimite(s, e) {
            grDate.PerformCallback("btnTrimite");
            //cmbQuiz.SetValue(null);
            //cmbAng.SetValue(null);
        }

        function OnFiltru(s, e) {
            grDate.PerformCallback("btnFiltru");
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de respingere ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge");
                    }
                });
            }
            else
            {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de aprobare ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba");
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

    </script>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" Visible ="false" >
                    <ClientSideEvents Click="function(s, e) {
                       OnRespinge(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" Visible ="false" >
                    <ClientSideEvents Click="function(s, e) {
                        OnAproba(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="Absente_divOuter">
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblPar" runat="server" style="display:inline-block;">Utilizator</label>
                        <dx:ASPxComboBox ID="cmbPar" ClientInstanceName="cmbPar" ClientIDMode="Static" runat="server" Width="200px" DropDownWidth="350px" ValueField="F70102" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" AllowNull="true"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}">
                            <Columns>
                                <dx:ListBoxColumn FieldName="F70102" Caption="Id User" Width="80px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Nume" Width="250px" />
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="80px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <dx:ASPxRadioButtonList ID="rbTip" runat="server" RepeatDirection="Vertical" >
                            <Items>
                                <dx:ListEditItem Value="1" Text="invita la propria evaluare" Selected="true" />
                                <dx:ListEditItem Value="2" Text="participa la evaluarea lui" />
                            </Items>
                        </dx:ASPxRadioButtonList>
                    </div>
                    
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Nume Coleg</label>
                        <dx:ASPxComboBox ID="cmbUsr" ClientInstanceName="cmbUsr" ClientIDMode="Static" runat="server" Width="200px" DropDownWidth="350px" ValueField="F70102" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" AllowNull="true"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
                            <Columns>
                                <dx:ListBoxColumn FieldName="F70102" Caption="Id User" Width="80px" />
                                <dx:ListBoxColumn FieldName="NumeComplet" Caption="Nume" Width="250px" />
                                <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="80px" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblQuiz" runat="server" style="display:inline-block;">Tip feedback</label>
						<dx:ASPxComboBox ID="cmbTip" ClientInstanceName="cmbTip" ClientIDMode="Static" runat="server" Width="200px" DropDownWidth="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true">
                        <Items>
                            <dx:ListEditItem Value="1" Text="Anual" Selected="true" />
                            <dx:ListEditItem Value="2" Text="Proiect" />
                        </Items>
                        </dx:ASPxComboBox>
                    </div>

                    <div class="Absente_Cereri_CampuriSup" id ="pnlStare" runat="server" visible="false">
                        <label id="lblStare" runat="server" style="display:inline-block;">Stare</label>
                        <dx:ASPxComboBox ID="cmbStare" ClientInstanceName="cmbStare" ClientIDMode="Static" runat="server" Width="100px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                            <Items>
                                <dx:ListEditItem Text = "De aprobat" Value = "1" Selected = "true" />
                                <dx:ListEditItem Text = "- Toate -" Value = "-9" />
                            </Items>
                        </dx:ASPxComboBox>
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <br /><br />
                        <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                OnFiltru(s,e);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <br /><br />
                        <dx:ASPxButton ID="btnTrimite" runat="server" Text="Trimite Solicitare" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                OnTrimite(s,e);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/validare.png"></Image>
                        </dx:ASPxButton>
                    </div>
                </div>
				<br /><br />
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="True" HorizontalScrollBarMode="Auto"  />
                    <SettingsSearchPanel Visible="true" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                    <Columns>

                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewCommandColumn Width="40px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Anulare" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

						<dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="250px" VisibleIndex="2" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="Evaluat" Name="Evaluat" Caption="Evaluat" ReadOnly="true" Width="250px" VisibleIndex="3" Settings-AutoFilterCondition="Contains" />

                        <dx:GridViewDataTextColumn FieldName="Quiz" Name="Quiz" Caption="Categorie feedback" ReadOnly="true" Width="250px" VisibleIndex="4" />

                        <dx:GridViewDataTextColumn FieldName="User" Name="User" Caption="Evaluator" ReadOnly="true" Width="250px" VisibleIndex="5" Settings-AutoFilterCondition="Contains" />
						
                        <dx:GridViewDataTextColumn FieldName="Tip" Name="Tip" Caption="Tip" ReadOnly="true" Width="200px" VisibleIndex="6" />
                    </Columns>
                    
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>


</asp:Content>
