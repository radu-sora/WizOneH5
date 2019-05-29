﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SporuriTranzactii.ascx.cs" Inherits="WizOne.Personal.SporuriTranzactii" %>




<body>

    <dx:ASPxGridView ID="grDateSporTran" runat="server" ClientInstanceName="grDateSporTran" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateSporTran_DataBinding" 
           OnRowUpdating="grDateSporTran_RowUpdating"  >        
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents  ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />                       
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />   
            <dx:GridViewDataTextColumn FieldName="Cod" Name="Cod" Caption="Cod"  Width="100px"  ReadOnly="true"/>
            <dx:GridViewDataComboBoxColumn FieldName="Spor" Name="Spor" Caption="Spor" Width="250px" >
                <PropertiesComboBox TextField="F02105" ValueField="F02104" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>                 
             <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="100px" Visible="false"/>
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
        </SettingsCommandButton>
    </dx:ASPxGridView>



</body>