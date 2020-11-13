<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuizAngajatGrup.ascx.cs" Inherits="WizOne.Eval.QuizAngajatGrup" %>

<body>
    <table>
        <tr>
            <td>
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting"
                    OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" AutoGenerateColumns="false" OnDataBinding="grDate_DataBinding">
                     <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="false" ShowColumnHeaders="true" />
                    <ClientSideEvents ContextMenu="ctx" />    
                    <SettingsEditing Mode="Inline" />               
                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" />

                        <dx:GridViewDataComboBoxColumn FieldName="IdGrup" Name="IdGrup" Caption="Grup Angajati" Width="200px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" />
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton Image-ToolTip="Actualizeaza">
                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton Image-ToolTip="Renunta">
                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                        </CancelButton>

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
            </td>
        </tr>
    </table>
</body>