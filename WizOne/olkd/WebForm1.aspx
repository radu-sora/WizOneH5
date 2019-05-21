<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WizOne.WebForm1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script>
        function Salveaza()
        {
            grid1.UpdateEdit();
            grid2.UpdateEdit();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxButton ID="btnSave" runat="server" Text="Salveaza All" AutoPostBack="false">
        <ClientSideEvents Click="function(s,e) { Salveaza(); }" />
    </dx:ASPxButton>

    <dx:ASPxGridView ID="grid1" ClientInstanceName="grid1" runat="server" OnBatchUpdate="grid1_BatchUpdate" AutoGenerateColumns="false">
        <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />

        <Columns>
            <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid1" Width="50px" ShowNewButtonInHeader="true" >
                <CustomButtons>
                    <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC1">
                        <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                    </dx:GridViewCommandColumnCustomButton>
                </CustomButtons>
            </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Id" Caption="Id"/>
            <dx:GridViewDataTextColumn FieldName="Denumire" Caption="Denumire"/>
        </Columns>

    </dx:ASPxGridView>

    <dx:ASPxGridView ID="grid2" ClientInstanceName="grid2" runat="server" OnBatchUpdate="grid2_BatchUpdate" AutoGenerateColumns="false">
        <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />

        <Columns>
                                    <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid2" Width="50px" ShowNewButtonInHeader="true" >
                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC2">
                                                <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>
            <dx:GridViewDataTextColumn FieldName="Id" Caption="Id"/>
            <dx:GridViewDataTextColumn FieldName="Denumire" Caption="Denumire"/>
        </Columns>
                                <SettingsCommandButton>
                                    <NewButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                    </NewButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>
                                </SettingsCommandButton>
    </dx:ASPxGridView>

                            <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="true" 
                                 >
                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" VerticalScrollableHeight="130" />
                                <SettingsSearchPanel Visible="false" />
                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                                <ClientSideEvents ContextMenu="ctx"  />

                                <Columns>
                                    <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true" >
                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC">
                                                <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>

                                    <dx:GridViewDataTextColumn FieldName="Id" Caption="Id"/>
                                    <dx:GridViewDataTextColumn FieldName="Denumire" Caption="Denumire"/>
                                </Columns>
                                
                                <SettingsCommandButton>
                                    <NewButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                    </NewButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>
                                </SettingsCommandButton>

                            </dx:ASPxGridView>

</asp:Content>
