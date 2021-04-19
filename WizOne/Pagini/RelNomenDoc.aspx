<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="RelNomenDoc.aspx.cs" Inherits="WizOne.Pagini.RelNomenDoc" %>


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
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="50%"  OnDataBinding="grDate_DataBinding"
                            OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnHtmlEditFormCreated="grDate_HtmlEditFormCreated"  >
                            <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" />
                            <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                            <SettingsSearchPanel Visible="False" />
                            <ClientSideEvents ContextMenu="ctx" />
                            <SettingsEditing Mode="EditFormAndDisplayRow" />
                            <Columns>
                                <dx:GridViewCommandColumn  ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  />
                                <dx:GridViewDataComboBoxColumn FieldName="IdNomen1" Name="IdNomen1" Caption="Nomenclator 1" VisibleIndex="1">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                     <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataTextColumn FieldName="Val1" Name="Val1" Caption="Valoare 1"  Width="100px" VisibleIndex="2">
                                     <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataComboBoxColumn FieldName="IdNomen2" Name="IdNomen2" Caption="Nomenclator 2" VisibleIndex="3">
                                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                    </PropertiesComboBox>
                                     <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                </dx:GridViewDataComboBoxColumn>
                                <dx:GridViewDataTextColumn FieldName="Val2" Name="Val2" Caption="Valoare 2"  Width="100px" VisibleIndex="4">
                                     <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                </dx:GridViewDataTextColumn>
                                <dx:GridViewDataTextColumn FieldName="Document" Name="Document" Caption="Document"  Width="75px" Visible="false">
                                     <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                                </dx:GridViewDataTextColumn>

                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                                <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO"  Width="75px" Visible="false"/>
                                <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME"  Width="75px" Visible="false"/>        
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
                                <NewButton Image-ToolTip="Rand nou">
                                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                    <Styles>
                                        <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                    </Styles>
                                </NewButton>
                            </SettingsCommandButton>
                            <Templates>
                                <EditForm>
                                    <div style="padding: 4px 3px 4px">
                                        <table>
                                            <tr>
                                                <td id="lblNomen1" runat="server" style="padding-left:10px !important;">Nomenclator 1</td>
                                                <td id="lblVal1" runat="server" style="padding-left:10px !important;">Valoare 1</td>
                                                <td id="lblNomen2" runat="server" style="padding-left:10px !important;">Nomenclator 2</td>
                                                <td id="lblVal2" runat="server" style="padding-left:10px !important;">Valoare 2</td>
                                            </tr>
                                            <tr>
                                                <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbNomen1" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdNomen1") %>' />                                          
                                                 <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtVal1" runat="server" Width="200" Value='<%# Bind("Val1") %>' /></td>
                                                 <td style="padding:10px !important;"><dx:ASPxComboBox ID="cmbNomen2" runat="server" Width="215px" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Value='<%# Bind("IdNomen2") %>' />                                          
                                                 <td style="padding:10px !important;"><dx:ASPxTextBox ID="txtVal2" runat="server" Width="200" Value='<%# Bind("Val2") %>' /></td>
                                            </tr>

                                            <tr>
                                                <td colspan="5" style="vertical-align:top;padding:10px !important;">
                                                    <dx:ASPxHtmlEditor ID="txtContinut" runat="server" ClientInstanceName="txtContinut" Height="300px" Width="900"  Html='<%# Bind("Document") %>'>
                                                        <ClientSideEvents  />
                                                        <SettingsDialogs>
                                                            <InsertImageDialog>
                                                                <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                                                                    <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                                                                    </ValidationSettings>
                                                                </SettingsImageUpload>
                                                            </InsertImageDialog>
                                                        </SettingsDialogs>
                                                    </dx:ASPxHtmlEditor>
                                                </td>

                                            </tr>
                                            <tr>
                                                <td style="padding:10px !important;">
                                                    <div style="text-align: left; padding: 2px; font-weight:bold; font-size:32px;">
                                                        <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                                        <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton" runat="server"></dx:ASPxGridViewTemplateReplacement>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                </EditForm>
                            </Templates>

                        </dx:ASPxGridView>
                        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid"></dx:ASPxGridViewExporter>
                    </td>
                </tr>
            </table>



</asp:Content>
