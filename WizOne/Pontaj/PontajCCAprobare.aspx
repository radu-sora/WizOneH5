<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajCCAprobare.aspx.cs" Inherits="WizOne.Pontaj.PontajCCAprobare" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script type="text/javascript">

        function OnRespinge(s, e)
        {
            if (grCC.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de respingere ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grCC.PerformCallback("btnRespinge");
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
            if (grCC.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de aprobare ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grCC.PerformCallback("btnAproba");
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

        
        function EmptyFields(s, e) {
            cmbAng.SetValue(null);
            cmbDpt.SetValue(null);
            cmbPro.SetValue(null);
            cmbCC.SetValue(null);
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


    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                       OnRespinge(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
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
                <br /><br />
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAnLuna" runat="server" style="display:inline-block;">Luna/An</label>
                            <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                            </dx:ASPxDateEdit>
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" SelectInputTextOnClick="true"
                                    CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >
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
                        <label id="lblCC" runat="server" style="display:inline-block;">Centru cost</label>
                        <dx:ASPxComboBox ID="cmbCC" ClientInstanceName="cmbCC" ClientIDMode="Static" runat="server" Width="150px" ValueField="F06204" TextField="F06205" ValueType="System.Int32" AutoPostBack="false" />
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblPro" runat="server" style="display:inline-block;;">Proiect</label>
                        <dx:ASPxComboBox ID="cmbPro" ClientInstanceName="cmbPro" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblDpt" runat="server" style="display:inline-block;">Departament</label>
                        <dx:ASPxComboBox ID="cmbDpt" ClientInstanceName="cmbDpt" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <br /><br />
                        <dx:ASPxButton ID="ASPxButton1" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                            <ClientSideEvents Click="function(s, e) {
                                            pnlLoading.Show();
                                            e.processOnServer = true;
                                        }" />
                        </dx:ASPxButton>
                    </div>

                    <div class="Absente_Cereri_CampuriSup">
                        <br /><br />
                        <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                            <ClientSideEvents Click="EmptyFields" />
                        </dx:ASPxButton>
                    </div>

                </div>

                <br /><br />

            </td>
        </tr>
        <tr>
            <td colspan="2">

                <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared"  Width="100%" OnCustomCallback="grCC_CustomCallback" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />

                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare"  VisibleIndex="1" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Angajat"  VisibleIndex="2" ReadOnly="true" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="Ziua" Name="Ziua" Caption="Ziua" VisibleIndex="3" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost"  VisibleIndex="4" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect"  VisibleIndex="5" ReadOnly="true" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="IdDept" Name="IdDept" Caption="Departament"  VisibleIndex="6" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="De" Name="De" Caption="De"  VisibleIndex="7" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="La" Name="La" Caption="La"  VisibleIndex="8" ReadOnly="true" />
                        <dx:GridViewDataTextColumn FieldName="NrOre" Name="NrOre" Caption="NrOre"  VisibleIndex="9" ReadOnly="true" />

                        <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                    </Columns>
                </dx:ASPxGridView>

            </td>
        </tr>
    </table>


</asp:Content>
