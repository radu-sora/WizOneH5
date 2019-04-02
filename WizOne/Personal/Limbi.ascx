<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Limbi.ascx.cs" Inherits="WizOne.Personal.Limbi" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateLimbi" runat="server" ClientInstanceName="grDateLimbi" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateLimbi_DataBinding"  OnInitNewRow="grDateLimbi_InitNewRow"
                    OnRowInserting="grDateLimbi_RowInserting" OnRowUpdating="grDateLimbi_RowUpdating" OnRowDeleting="grDateLimbi_RowDeleting">        
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />             
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateLimbi_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />                                            
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdLimba" Name="IdLimba" Caption="Limba"  Width="100px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="IdAuto" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn> 
                         <dx:GridViewDataTextColumn FieldName="Nivel" Name="Nivel" Caption="Nivel vorbit"  Width="100px" />
                         <dx:GridViewDataTextColumn FieldName="NrAniVorbit" Name="NrAniVorbit" Caption="Nr. ani vorbit"  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false"  Width="100px" />						
                        <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false"  Width="100px" />
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Renunta">
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