<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="WizOne.WebForm2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxGridView ID="Grid" runat="server" KeyFieldName="ID" OnBatchUpdate="Grid_BatchUpdate" SettingsEditing-BatchEditSettings-HighlightDeletedRows="false"
         OnRowDeleting="Grid_RowDeleting">
        <Columns>

            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>

            <dx:GridViewDataColumn FieldName="C1" />
            <dx:GridViewDataSpinEditColumn FieldName="C2" />
            <dx:GridViewDataTextColumn FieldName="C3" />
            <dx:GridViewDataCheckColumn FieldName="C4" />
            <dx:GridViewDataDateColumn FieldName="C5" />
        </Columns>
        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
        <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
        <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
        <SettingsSearchPanel Visible="false" />
        <SettingsLoadingPanel Mode="ShowAsPopup" />
                                        <SettingsCommandButton>
                                            <EditButton Image-ToolTip="Edit">
                                                <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                                                <Styles>
                                                    <Style Paddings-PaddingRight="5px" />
                                                </Styles>
                                            </EditButton>
                                            <DeleteButton Image-ToolTip="Sterge">
                                                <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                                            </DeleteButton>
                                            <NewButton Image-ToolTip="Rand nou">
                                                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                                                <Styles>
                                                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                                                </Styles>
                                            </NewButton>
                                        </SettingsCommandButton>
    </dx:ASPxGridView>

</asp:Content>
