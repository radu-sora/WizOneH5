<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WizOne.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
                                  
    <dx:ASPxGridView ID="grDateAbs" runat="server" ClientInstanceName="grDateAbs" ClientIDMode="Static" AutoGenerateColumns="false" OnBatchUpdate="grDateAbs_BatchUpdate" SettingsPager-PageSize="5" OnRowDeleting="grDateAbs_RowDeleting">
        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true"/>
        <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Visible" ShowStatusBar="Hidden" />

        <Columns>
            <dx:GridViewCommandColumn FixedStyle="Left" ShowDeleteButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true"/>
                                            
            <dx:GridViewDataComboBoxColumn FieldName="IdAbsenta" Name="IdAbsenta" Caption="Absenta" Width="350px" >
                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>   
            <dx:GridViewDataCheckColumn FieldName="ZL" Name="ZL" Caption="Zile lucratoare" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
            <dx:GridViewDataCheckColumn FieldName="S" Name="S" Caption="Sambata" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
            <dx:GridViewDataCheckColumn FieldName="D" Name="D" Caption="Duminica" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
            <dx:GridViewDataCheckColumn FieldName="SL" Name="SL" Caption="Sarb. Legale" Width="90px" HeaderStyle-HorizontalAlign="Center"/>
            <dx:GridViewDataCheckColumn FieldName="InPontajAnual" Name="InPontajAnual" Caption="Istoric extins" Width="90px" HeaderStyle-HorizontalAlign="Center"/>

            <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract" Visible="false" Width="90px" ShowInCustomizationForm="false"/> 
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" Width="90px" ShowInCustomizationForm="false"/>      
            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" Width="90px" ShowInCustomizationForm="false" />						
            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" Width="90px" ShowInCustomizationForm="false" />              
        </Columns>
        <SettingsCommandButton>
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
                                
</asp:Content>
