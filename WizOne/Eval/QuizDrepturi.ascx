<%@ Control Language="C#" AutoEventWireup="true"  CodeBehind="QuizDrepturi.ascx.cs" Inherits="WizOne.Eval.QuizDrepturi" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<body>
    <table>
        <tr>
            <td>
                <dx:ASPxGridView ID="grDateDrepturi" runat="server" ClientInstanceName-="grDateDrepturi" ClientIDMode="Static" OnRowDeleting="grDateDrepturi_RowDeleting" OnDataBinding="grDateDrepturi_DataBinding"
                    AutoGenerateColumns="false" OnRowInserting="grDateDrepturi_RowInserting" OnRowUpdating="grDateDrepturi_RowUpdating" OnInitNewRow="grDateDrepturi_InitNewRow">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="false" ShowColumnHeaders="true" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsEditing Mode="Inline" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="IdQuiz" Name="IdQuiz" Caption="IdQuiz" Visible="false" />
                        <dx:GridViewDataComboBoxColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie" VisibleIndex="1">
                            <PropertiesComboBox ValueType="System.Int32" DropDownStyle="DropDown" >
                                <Items>
                                    <dx:ListEditItem Text="1" Value="1" />
                                    <dx:ListEditItem Text="2" Value="2" />
                                    <dx:ListEditItem Text="3" Value="3" />
                                    <dx:ListEditItem Text="4" Value="4" />
                                    <dx:ListEditItem Text="5" Value="5" />
                                    <dx:ListEditItem Text="6" Value="6" />
                                    <dx:ListEditItem Text="7" Value="7" />
                                    <dx:ListEditItem Text="8" Value="8" />
                                    <dx:ListEditItem Text="9" Value="9" />
                                    <dx:ListEditItem Text="10" Value="10" />
                                </Items>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="PozitieVizibila" Name="PozitieVizibila" Caption="Ce pozitie vede" VisibleIndex="2">
                            <PropertiesComboBox ValueType="System.Int32" DropDownStyle="DropDown">
                                <Items>
                                    <dx:ListEditItem Text="1" Value="1" />
                                    <dx:ListEditItem Text="2" Value="2" />
                                    <dx:ListEditItem Text="3" Value="3" />
                                    <dx:ListEditItem Text="4" Value="4" />
                                    <dx:ListEditItem Text="5" Value="5" />
                                    <dx:ListEditItem Text="6" Value="6" />
                                    <dx:ListEditItem Text="7" Value="7" />
                                    <dx:ListEditItem Text="8" Value="8" />
                                    <dx:ListEditItem Text="9" Value="9" />
                                    <dx:ListEditItem Text="10" Value="10" />
                                </Items>
                            </PropertiesComboBox>
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