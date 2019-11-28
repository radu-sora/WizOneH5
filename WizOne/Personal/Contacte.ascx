<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contacte.ascx.cs" Inherits="WizOne.Personal.Contacte" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateContacte" runat="server" ClientInstanceName="grDateContacte" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateContacte_DataBinding"  OnInitNewRow="grDateContacte_InitNewRow"
                    OnRowInserting="grDateContacte_RowInserting" OnRowUpdating="grDateContacte_RowUpdating" OnRowDeleting="grDateContacte_RowDeleting">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateContacte_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                        <dx:GridViewDataComboBoxColumn FieldName="IdContact" Name="IdContact" Caption="Tip contact" Width="100px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Valoare" Name="Valoare" Caption="Valoare"  Width="250px" />
                        <dx:GridViewDataTextColumn FieldName="Interior" Name="Interior" Caption="Interior"  Width="250px" />
                         <dx:GridViewDataComboBoxColumn FieldName="IdLocatie" Name="IdLocatie" Caption="Locatie"  Width="100px" >
                            <PropertiesComboBox TextField="LOCATIE" ValueField="NUMAR" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>          
                        <dx:GridViewDataCheckColumn FieldName="BifaTrimitereEmail" Name="BifaTrimitereEmail" Caption="BifaTrimitereEmail"  Width="70px"  />
                        <dx:GridViewDataTextColumn FieldName="ParolaPDF" Name="ParolaPDF" Caption="ParolaPDF" >
                            <PropertiesTextEdit Password="true">
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                         <dx:GridViewDataComboBoxColumn FieldName="EstePrincipal" Name="EstePrincipal" Caption="Principal"  Width="75px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn> 
                        <dx:GridViewDataTextColumn FieldName="ValoareString" Name="ValoareString" Caption="ValoareString" Visible="false"  Width="100px" />	
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