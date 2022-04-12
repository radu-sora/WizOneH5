<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Experienta.ascx.cs" Inherits="WizOne.Personal.Experienta" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateExperienta" runat="server" ClientInstanceName="grDateExperienta" ClientIDMode="Static" Width="90%" AutoGenerateColumns="false"  OnDataBinding="grDateExperienta_DataBinding"  OnInitNewRow="grDateExperienta_InitNewRow"
                            OnRowInserting="grDateExperienta_RowInserting" OnRowUpdating="grDateExperienta_RowUpdating" OnRowDeleting="grDateExperienta_RowDeleting" OnCustomUnboundColumnData="grDateExperienta_CustomUnboundColumnData">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateExperienta_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Tip experienta"  Width="150px" >
                            <Settings SortMode="DisplayText" />
                            <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Companie" Name="Companie" Caption="Companie"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="Oras" Name="Oras" Caption="Oras"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="Domeniu" Name="Domeniu" Caption="Domeniu"  Width="150px" />
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="Vechime" Name="Vechime" Caption="Vechime" ReadOnly="true"  Width="100px" UnboundType="String" />
                        <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie"  Width="100px" />
                        <dx:GridViewDataTextColumn FieldName="Responsabilitati" Name="Responsabilitati" Caption="Responsabilitati"  Width="100px" />   
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