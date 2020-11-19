<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Form1.aspx.cs" Inherits="WizOne.Eval.Form1" %>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxGridView ID="grDateTabela" runat="server" SettingsPager-PageSize="50" ClientInstanceName="grDateTabela" ClientIDMode="Static" AutoGenerateColumns="false" KeyFieldName="IdQuiz;IdLinie;Coloana"
            OnRowInserting="grDateTabela_RowInserting" OnRowUpdating="grDateTabela_RowUpdating" OnRowDeleting="grDateTabela_RowDeleting" OnInitNewRow="grDateTabela_InitNewRow">
        <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
        <Settings ShowFilterRow="false" ShowGroupPanel="false" HorizontalScrollBarMode="Auto" />
        <SettingsSearchPanel Visible="false" />
        <ClientSideEvents ContextMenu="ctx" />
        <SettingsEditing Mode="Inline" />
        <Columns>
            <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />

            <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" ShowInCustomizationForm="false" />
            <dx:GridViewDataTextColumn FieldName="IdLinie" Name="IdLinie" Caption="IdLinie" Visible="false" ShowInCustomizationForm="false" />
            <dx:GridViewDataComboBoxColumn FieldName="Coloana" Name="Coloana" Caption="Coloana" Width="120">
                <PropertiesComboBox>
                    <Items>
                        <dx:ListEditItem Value="1" Text="Coloana 1" />
                        <dx:ListEditItem Value="2" Text="Coloana 2" />
                        <dx:ListEditItem Value="3" Text="Coloana 3" />
                        <dx:ListEditItem Value="4" Text="Coloana 4" />
                        <dx:ListEditItem Value="5" Text="Coloana 5" />
                        <dx:ListEditItem Value="6" Text="Coloana 6" />
                    </Items>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataSpinEditColumn FieldName="Lungime" Name="Lungime" Caption="Lungime" Width="100" PropertiesSpinEdit-MinValue="10" PropertiesSpinEdit-MaxValue="1000" PropertiesSpinEdit-SpinButtons-ShowIncrementButtons="false"/>
            <dx:GridViewDataTextColumn FieldName="Alias" Name="Alias" Caption="Alias" Width="250"/>
                                                                                        
        </Columns>
        <SettingsCommandButton>
            <EditButton>
                <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                <Styles>
                    <Style Paddings-PaddingRight="5px" />
                </Styles>
            </EditButton>
	        <DeleteButton>
		        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
	        </DeleteButton>
                                                                                        
            <UpdateButton>
                <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                <Styles>
                    <Style Paddings-PaddingRight="5px" />
                </Styles>
            </UpdateButton>
            <CancelButton>
                <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
            </CancelButton>

            <NewButton>
                <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
            </NewButton>
        </SettingsCommandButton>
    </dx:ASPxGridView>

</asp:Content>
