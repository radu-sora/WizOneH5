<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudiiNou.ascx.cs" Inherits="WizOne.Personal.StudiiNou" %>




<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDateStudii" runat="server" ClientInstanceName="grDateStudii" ClientIDMode="Static" Width="90%" AutoGenerateColumns="false"  OnDataBinding="grDateStudii_DataBinding"  OnInitNewRow="grDateStudii_InitNewRow"
                            OnRowInserting="grDateStudii_RowInserting" OnRowUpdating="grDateStudii_RowUpdating" OnRowDeleting="grDateStudii_RowDeleting" OnCustomUnboundColumnData="grDateStudii_CustomUnboundColumnData">
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDateStudii_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
                    <SettingsEditing Mode="Inline" />   
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat"  Width="75px" Visible="false"/>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>

                        <dx:GridViewDataComboBoxColumn FieldName="IdTipInvatamant" Name="IdTipInvatamant" Caption="Tip invatamant"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdNivel" Name="IdNivel" Caption="Nivel studii"  Width="150px" >
                            <PropertiesComboBox TextField="F71204" ValueField="F71202" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NivelISCED" Name="NivelISCED" Caption="Nivel ISCED" ReadOnly="true"   Width="50px" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdTipInstitutie" Name="IdTipInstitutie" Caption="Tip institutie de invatamant"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="NumeInstitutie" Name="NumeInstitutie" Caption="Nume institutie"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="NrClase" Name="NrClase" Caption="Numar clase"  Width="50px" />
                        <dx:GridViewDataComboBoxColumn FieldName="SirutaLocalitate" Name="SirutaLocalitate" Caption="Localitate" Width="200px" >
                            <PropertiesComboBox TextField="Nivel3" ValueField="SIRUTA" ValueType="System.Int32" DropDownStyle="DropDown">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Nivel3" Caption="Localitate/Sat/Sector" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Nivel2" Caption="Comuna/Oras/Municipiu" Width="130px" />
                                    <dx:ListBoxColumn FieldName="Nivel1" Caption="Judet" Width="130px" />
                                </Columns>
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="DeLaData" Name="DeLaData" Caption="De la data"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="LaData" Name="LaData" Caption="La data"  Width="100px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="Perioada" Name="Perioada" Caption="Perioada" ReadOnly="true"  Width="100px" UnboundType="String" />
                        <dx:GridViewDataTextColumn FieldName="Specializare" Name="Specializare" Caption="Specializare"  Width="250px" />
                        <dx:GridViewDataComboBoxColumn FieldName="IdProfil" Name="IdProfil" Caption="Profil"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="IdDomeniu" Name="IdDomeniu" Caption="Domeniu studiat"  Width="150px" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="Calificare" Name="Calificare" Caption="Calificare"  Width="250px" />                         
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