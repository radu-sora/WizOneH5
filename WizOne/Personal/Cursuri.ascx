<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Cursuri.ascx.cs" Inherits="WizOne.Personal.Cursuri" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateCursuri" runat="server" ClientInstanceName="grDateCursuri" ClientIDMode="Static" Width="80%" AutoGenerateColumns="False"  OnDataBinding="grDateCursuri_DataBinding"  OnInitNewRow="grDateCursuri_InitNewRow"
                    OnRowInserting="grDateCursuri_RowInserting" OnRowUpdating="grDateCursuri_RowUpdating" OnRowDeleting="grDateCursuri_RowDeleting" OnCommandButtonInitialize="grDateCursuri_CommandButtonInitialize">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateCursuri_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

<SettingsPopup>
<HeaderFilter MinHeight="140px"></HeaderFilter>
</SettingsPopup>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="4" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false" ReadOnly="True"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" VisibleIndex="0"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdTipCurs" Name="IdTipCurs" Caption="Tip curs" Width="100px" VisibleIndex="5" >
                            <PropertiesComboBox TextField="TipCurs" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="NumeComplet" Caption="Nume complet"  Width="250px" VisibleIndex="7" />
                        <dx:GridViewDataTextColumn FieldName="Info" Name="Info" Caption="Info" VisibleIndex="10" Width="150px" >
                            <PropertiesTextEdit Password="true">
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" VisibleIndex="1" />						
                        <dx:GridViewDataDateColumn Caption="TIME" FieldName="TIME" Name="TIME" VisibleIndex="2" Width="100px" Visible="False">
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Numar Ore" FieldName="NrOre" Name="NrOre" VisibleIndex="11" Width="100px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataComboBoxColumn Caption="Descriere curs" FieldName="IdDescriereCurs" Name="IdDescriereCurs" VisibleIndex="6" Width="250px" ShowInCustomizationForm="True">
                            <PropertiesComboBox TextField="DescriereCurs" ValueField="IdAuto">
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn Caption="Nume furnizor" FieldName="NumeFurnizor" Name="NumeFurnizor" VisibleIndex="8" Width="100px" ShowInCustomizationForm="True">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="TemaCurs" Name="TemaCurs" Caption="Tema curs"  Width="100px" VisibleIndex="9" ShowInCustomizationForm="True" >
                            <PropertiesTextEdit EnableFocusedStyle="False">
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn FieldName="DataCurs" Name="DataCurs" Caption="Data curs"  Width="100px" VisibleIndex="12" ShowInCustomizationForm="True" >
                            <PropertiesDateEdit DisplayFormatString="">
                            </PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Buget" FieldName="Buget" Name="Buget" VisibleIndex="13" Width="100px" ShowInCustomizationForm="True">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Perioada amortizare" FieldName="PerioadaAmortizare" Name="PerioadaAmortizare" VisibleIndex="14" Width="100px" ShowInCustomizationForm="True">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataDateColumn Caption="Data Expirare Autorizare" FieldName="AutorizareExpirare" Name="AutorizareExpirare" VisibleIndex="15" Width="100px" ShowInCustomizationForm="True">
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Modificabil" FieldName="Modificabil" Name="Modificabil" Visible="False" VisibleIndex="3">
                        </dx:GridViewDataTextColumn>
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
<Paddings PaddingTop="10px" PaddingRight="10px"></Paddings>
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Renunta">
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" >
<Paddings PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </EditButton>
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
                        </DeleteButton>
                        <NewButton Image-ToolTip="Rand nou">
                            <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                            <Styles>
                                <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" >
<Paddings PaddingLeft="5px" PaddingRight="5px"></Paddings>
                                </Style>
                            </Styles>
                        </NewButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table> 



</body>