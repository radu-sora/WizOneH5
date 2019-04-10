<%@ Control Language="C#" AutoEventWireup="true"  CodeBehind="QuizCircuit.ascx.cs" Inherits="WizOne.Eval.QuizCircuit" %>


<body>
    <table>
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateCircuit" runat="server" ClientInstanceName =" grDateCircuit" ClientIDMode="Static" OnRowDeleting="grDateCircuit_RowDeleting" OnDataBinding="grDateCircuit_DataBinding"
                    AutoGenerateColumns="false" OnRowInserting="grDateCircuit_RowInserting" OnRowUpdating="grDateCircuit_RowUpdating" OnInitNewRow="grDateCircuit_InitNewRow">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsEditing Mode="Inline" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" />

                        <dx:GridViewDataComboBoxColumn FieldName="Super1" Name="Super1" Caption="Super1" Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="Super2" Name="Super2" Caption="Super2" Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="Super3" Name="Super3" Caption="Super3" Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="Super4" Name="Super4" Caption="Super4" Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="Super5" Name="Super5" Caption="Super5" Width="150px">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataCheckColumn FieldName="RespectaOrdinea" Name="RespectaOrdinea" Caption="Respecta Ordinea"  Width="70px"  />
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