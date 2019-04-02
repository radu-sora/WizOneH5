<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Experienta.ascx.cs" Inherits="WizOne.Recrutare.Experienta" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


    <dx:ASPxGridView ID="grDate" runat="server" Width="100%" AutoGenerateColumns="false" OnInitNewRow="grDate_InitNewRow" OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting">
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
            
            <dx:GridViewDataComboBoxColumn FieldName="IdObiect" Name="IdObiect" Caption="Tip experienta" Width="250px" >
                <PropertiesComboBox TextField="NumeCompus" ValueField="IdObiect" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataTextColumn FieldName="Companie" Name="Companie" Caption="Companie" Width="250px" />
            <dx:GridViewDataTextColumn FieldName="Oras" Name="Oras" Caption="Oras" Width="250px" />
            <dx:GridViewDataTextColumn FieldName="Domeniu" Name="Domeniu" Caption="Domeniu" Width="250px" />
            <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput" Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie" Width="250px" />
            <dx:GridViewDataMemoColumn FieldName="Responsabilitati" Name="Responsabilitati" Caption="Responsabilitati" Width="250px" />
            
            <dx:GridViewDataTextColumn FieldName="Marca" Name="Marca" Caption="Angajat" Visible="false" ShowInCustomizationForm="false"/>
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>               
            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />                                             
        </Columns>
        <SettingsCommandButton>
            <UpdateButton>
                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                <Styles>
                    <Style Paddings-PaddingRight="5px" />
                </Styles>
            </UpdateButton>
            <CancelButton>
                <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
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
