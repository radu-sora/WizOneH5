<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajCCAprobare.aspx.cs" Inherits="WizOne.Pontaj.PontajCCAprobare" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


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
                        e.processOnServer = true;
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
                        e.processOnServer = true;
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

    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnRespinge" runat="server" Text="Respinge" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnRespinge_Click" >
                    <ClientSideEvents Click="function(s, e) {
                        e.processOnServer = false;
                       OnRespinge(s,e);
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" runat="server" Text="Aproba" AutoPostBack="false" oncontextMenu="ctx(this,event)" OnClick="btnAproba_Click" >
                    <ClientSideEvents Click="function(s, e) {
                        e.processOnServer = false;
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
                            <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" />
                    </div>
                    <div class="Absente_Cereri_CampuriSup">
                        <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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

                <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared"  Width="100%" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" />
                    <SettingsEditing Mode="Inline" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" />

                    <Columns>
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" Width="150px" VisibleIndex="1" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="1" Visible="false">
                            <PropertiesComboBox TextField="F06205" ValueField="F06204" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="2" Visible="false">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="3" Visible="false">
                            <PropertiesComboBox TextField="Dept" ValueField="IdDept" ValueType="System.Int32" DropDownStyle="DropDown">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Dept" Caption="Dept" Width="130px" />
                                </Columns>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="4" >
                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" />
                        </dx:GridViewDataTimeEditColumn>
                        <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="5" >
                            <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm" />
                        </dx:GridViewDataTimeEditColumn>
                        <dx:GridViewDataSpinEditColumn FieldName="NrOre" Name="NrOre" Caption="NrOre" Width="100px" VisibleIndex="7" />

                        <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

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

            </td>
        </tr>
    </table>


</asp:Content>
