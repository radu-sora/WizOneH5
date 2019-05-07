<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="WizOne.Pagini.Users" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

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
                    <td colspan="5">
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%"
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomCallback="grDate_CustomCallback" >
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="True" ShowGroupPanel="True" VerticalScrollBarMode="Visible" />
                            <SettingsSearchPanel Visible="True" />
                            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" Init="OnInitGrid" ContextMenu="ctx" />
                            <SettingsEditing Mode="Inline" />
                            <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

                            <Columns>
                                <dx:GridViewCommandColumn ShowSelectCheckbox="True" ShowClearFilterButton="true" VisibleIndex="0" SelectAllCheckboxMode="AllPages" />

                                <dx:GridViewDataTextColumn FieldName="F70101" Name="F70101" Caption="Nr. tabela" VisibleIndex="1" Visible="false" ShowInCustomizationForm="false"/>
                                <dx:GridViewDataTextColumn FieldName="F70102" Name="F70102" Caption="Id" VisibleIndex="2"/>
                                <dx:GridViewDataTextColumn FieldName="F70104" Name="F70104" Caption="Nume" VisibleIndex="3" Settings-AutoFilterCondition="Contains"/>
                                <dx:GridViewDataTextColumn FieldName="F70103" Name="F70103" Caption="Parola" VisibleIndex="4"  Width="90">
                                    <PropertiesTextEdit Password="True" ClientInstanceName="psweditor" />
                                    <EditItemTemplate>
                                        <dx:ASPxTextBox ID="pswtextbox"  Width="85" runat="server" Text='<%# Bind("F70103") %>' Visible='<%# grDate.IsNewRowEditing %>' Password="True">
                                            <ClientSideEvents Validation="function(s,e){e.isValid = s.GetText().length>5;}" />
                                        </dx:ASPxTextBox>
                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="popup.ShowAtElement(this); return false;" Visible='<%#!grDate.IsNewRowEditing%>'>Edit parola</asp:LinkButton>
                                    </EditItemTemplate>
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" VisibleIndex="5">
                                    <PropertiesComboBox TextField="NumeComplet" ValueField="Marca" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Nume Complet" VisibleIndex="6" ReadOnly="false" Settings-AutoFilterCondition="Contains"/>

                                <dx:GridViewDataTextColumn FieldName="F70105" Name="F70105" Caption="Id" VisibleIndex="7"/>
                                <dx:GridViewDataCheckColumn FieldName="F70111" Name="F70111" Caption="Parola Expira" VisibleIndex="8"/>
                                <dx:GridViewDataTextColumn FieldName="F70121" Name="F70121" Caption="in...zile" VisibleIndex="9"/>
                                <dx:GridViewDataCheckColumn FieldName="F70112" Name="F70112" Caption="Parola complexa" VisibleIndex="10"/>
                                <dx:GridViewDataCheckColumn FieldName="F70113" Name="F70113" Caption="Resetare parola" VisibleIndex="11"/>
                                <dx:GridViewDataCheckColumn FieldName="F70114" Name="F70114" Caption="Utilizator blocat" VisibleIndex="12"/>

                                <dx:GridViewDataTextColumn FieldName="F70123" Name="F70123" Caption="Cale salvari" VisibleIndex="13"/>
                                <dx:GridViewDataTextColumn FieldName="Mail" Name="Mail" Caption="Mail" VisibleIndex="14"/>
                                <dx:GridViewDataComboBoxColumn FieldName="IdLimba" Name="IdLimba" Caption="Limba" VisibleIndex="15">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataCheckColumn FieldName="SchimbaParola" Name="SchimbaParola" Caption="Schimba Parola" VisibleIndex="16"/>
                                <dx:GridViewDataTextColumn FieldName="Parola" Name="Parola" Caption="Parola" VisibleIndex="17"/>
                                <dx:GridViewDataTextColumn FieldName="PINInfoChiosc" Name="PINInfoChiosc" Caption="PINInfoChiosc" VisibleIndex="18"/>

                                <dx:GridViewDataTextColumn FieldName="F70122" Name="F70122" Caption="Data editare" VisibleIndex="19" Visible="false" ShowInCustomizationForm="false"/>
                                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" VisibleIndex="20" Visible="false" ShowInCustomizationForm="false"/>
                                <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" VisibleIndex="21" Visible="false" ShowInCustomizationForm="false"/>

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

            <dx:ASPxPopupControl ID="ASPxPopupControl1" runat="server" HeaderText="Edit parola" ClientInstanceName="popup">
                <ClientSideEvents CloseUp="function(s, e) {
                    npsw.SetText('');
                    cnpsw.SetText('');
                    cnpsw.Validate();
                    }" />
                <ContentCollection>
                    <dx:PopupControlContentControl ID="Popupcontrolcontentcontrol1" runat="server">
                        <table>
                            <tr>
                                <td style="vertical-align:top;"><label style="width:100px; font-weight:normal;">Parola noua</label></td>
                                <td>
                                    <dx:ASPxTextBox ID="npsw" runat="server" Password="True" ClientInstanceName="npsw" />
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align:top;"><label style="width:100px; font-weight:normal;">Repeta parola</label></td>
                                <td>
                                    <dx:ASPxTextBox ID="cnpsw" runat="server" Password="True" ClientInstanceName="cnpsw">
                                        <ClientSideEvents Validation="function(s, e) {e.isValid = (s.GetText() == npsw.GetText());}" />
                                        <ValidationSettings ErrorDisplayMode="ImageWithText" ErrorText="Parola nu coincide" ErrorTextPosition="Bottom" ErrorFrameStyle-ErrorTextPaddings-PaddingTop="10px">
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2"><dx:ASPxButton ID="confirmButton" runat="server" Text="Ok" AutoPostBack="False" OnClick="confirmButton_Click" /></td>
                            </tr>
                        </table>

                    </dx:PopupControlContentControl>
                </ContentCollection>
            </dx:ASPxPopupControl>


    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>


</asp:Content>
