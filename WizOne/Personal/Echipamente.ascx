<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Echipamente.ascx.cs" Inherits="WizOne.Personal.Echipamente" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateEchipamente" runat="server" ClientInstanceName="grDateEchipamente" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateEchipamente_DataBinding"  OnInitNewRow="grDateEchipamente_InitNewRow" 
                            OnRowInserting="grDateEchipamente_RowInserting" OnRowUpdating="grDateEchipamente_RowUpdating" OnRowDeleting="grDateEchipamente_RowDeleting" OnCustomUnboundColumnData="grDateEchipamente_CustomUnboundColumnData">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />         
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateEchipamente_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />                                                   
                    <Columns>
                        <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Nume echipament" Width="250px" >
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Caracteristica" Name="Caracteristica" Caption="Caracteristica echipament"  Width="250px" />
                        <dx:GridViewDataDateColumn FieldName="DataPrimire" Name="DataPrimire" Caption="Data primire"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataPredare" Name="DataPredare" Caption="Data predare"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataExpirare" Name="DataExpirare" Caption="Data expirare"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="DurataUtil" Name="DurataUtil" Caption="Durata utilizare" ReadOnly="true"  Width="175px" UnboundType="String" />    
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