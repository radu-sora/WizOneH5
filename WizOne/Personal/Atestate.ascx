<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Atestate.ascx.cs" Inherits="WizOne.Personal.Atestate" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateAtestate" runat="server" ClientInstanceName="grDateAtestate" ClientIDMode="Static" Width="95%" AutoGenerateColumns="False"  OnDataBinding="grDateAtestate_DataBinding"  OnInitNewRow="grDateAtestate_InitNewRow"
                    OnRowInserting="grDateAtestate_RowInserting" OnRowUpdating="grDateAtestate_RowUpdating" OnRowDeleting="grDateAtestate_RowDeleting">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateAtestate_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>

<SettingsPopup>
<HeaderFilter MinHeight="140px"></HeaderFilter>
</SettingsPopup>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="3" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false" VisibleIndex="0"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdTipAutorizatie" Name="IdTipAutorizatie" Caption="Tip autorizatie" Width="100px" VisibleIndex="4" >
                            <PropertiesComboBox TextField="TipAutorizatie" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NrAutorizatie" Name="NrAutorizatie" Caption="Nr Autorizatie"  Width="250px" VisibleIndex="5" />
                        <dx:GridViewDataTextColumn FieldName="TalonAutorizatie" Name="TalonAutorizatie" Caption="Talon Autorizatie"  Width="250px" VisibleIndex="6" />
                        <dx:GridViewDataTextColumn FieldName="Categorie" Name="Categorie" Caption="Categorie" VisibleIndex="9" Width="100px" >
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" VisibleIndex="1" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" VisibleIndex="2" />
                        <dx:GridViewDataDateColumn Caption="Data Ultima Viza" FieldName="DataUltimaViza" Name="DataUltimaViza" ShowInCustomizationForm="True" VisibleIndex="7" Width="100px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                            </PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn Caption="Data Expirare Talon" FieldName="DataExpirareTalon" Name="DataExpirareTalon" ShowInCustomizationForm="True" VisibleIndex="8" Width="100px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" EnableFocusedStyle="False">
                            </PropertiesDateEdit>
                            <HeaderStyle Wrap="True" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn Caption="Grupa" FieldName="Grupa" Name="Grupa" ShowInCustomizationForm="True" VisibleIndex="10" Width="100px">
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn Caption="Nr Proces Verbal" FieldName="NrProcesVerbal" Name="NrProcesVerbal" ShowInCustomizationForm="True" VisibleIndex="11" Width="150px">
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