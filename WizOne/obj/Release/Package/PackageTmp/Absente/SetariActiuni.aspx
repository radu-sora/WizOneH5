<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="SetariActiuni.aspx.cs" Inherits="WizOne.Absente.SetariActiuni" %>

        <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

            <script type="text/javascript">
            </script>

        </asp:Content>


        <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
            <table width="100%">
                <tr>
                    <td align="left">
                        <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
                    </td>
                    <td align="right">
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
                            <label id="lblViz" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Actiune</label>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbAct" ClientInstanceName="cmbAct" ClientIDMode="Static" runat="server" Width="250px" AutoPostBack="false" >
                                    <Items>
                                        <dx:ListEditItem Text = "Aprobare" Value = "1" Selected = "true" />
                                        <dx:ListEditItem Text = "Respingere" Value = "2" />
                                        <dx:ListEditItem Text = "Anulare" Value = "3" />
                                        <dx:ListEditItem Text = "Solicitare" Value = "4" />
                                    </Items>
                                </dx:ASPxComboBox>
                            </div>
                            <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px;">Stare</label>
                            <div style="float:left; padding-right:15px;">
                                <dx:ASPxComboBox ID="cmbStr" ClientInstanceName="cmbStr" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
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
                    <td colspan="5">
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%" OnCellEditorInitialize="grDate_CellEditorInitialize"
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomErrorText="grDate_CustomErrorText" >
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" />
                            <SettingsSearchPanel Visible="True" />
                            <ClientSideEvents ContextMenu="ctx" />
                            <SettingsEditing Mode="Inline" />

                            <Columns>
                                <dx:GridViewCommandColumn ShowSelectCheckbox="false" ShowClearFilterButton="true" VisibleIndex="0" SelectAllCheckboxMode="None" Width="50px" ShowDeleteButton="true" ShowEditButton="true" ButtonType="Image" Caption=" " />

                                <dx:GridViewDataComboBoxColumn FieldName="IdAbs" Name="IdAbs" Caption="Absenta" VisibleIndex="1" ReadOnly="true">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="IdRol" Name="IdRol" Caption="Rol" VisibleIndex="2" ReadOnly="true">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="Valoare" Name="Valoare" Caption="Valoare" VisibleIndex="3">
                                    <PropertiesComboBox ValueType="System.Int32" DropDownStyle="DropDown">
                                        <Items>
                                            <dx:ListEditItem Text="zi curenta" Value="1" />
                                            <dx:ListEditItem Text="prima zi din saptamana" Value="2" />
                                            <dx:ListEditItem Text="ultima zi din saptamana" Value="3" />
                                            <dx:ListEditItem Text="prima zi din luna curenta" Value="4" />
                                            <dx:ListEditItem Text="ultima zi din luna curenta" Value="5" />
                                            <dx:ListEditItem Text="prima zi din luna de lucru" Value="6" />
                                            <dx:ListEditItem Text="ultima zi din luna de lucru" Value="7" />
                                            <dx:ListEditItem Text="prima zi din an" Value="8" />
                                            <dx:ListEditItem Text="ultima zi din an" Value="9" />
                                            <dx:ListEditItem Text="pontaj inchis" Value="13" />
                                            <dx:ListEditItem Text="2100" Value="49" />
                                        </Items>
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataSpinEditColumn FieldName="NrZile" Name="NrZile" Caption="Nr. zile" VisibleIndex="4">
                                </dx:GridViewDataSpinEditColumn>
                                
                                <dx:GridViewDataTextColumn FieldName="IdActiune" Name="IdActiune" Caption="Actiune" ReadOnly="true" Width="50px" Visible="false" VisibleIndex="5" ShowInCustomizationForm="false" />
                                <dx:GridViewDataTextColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" Width="50px" Visible="false" VisibleIndex="6" ShowInCustomizationForm="false" />

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



        </asp:Content>
