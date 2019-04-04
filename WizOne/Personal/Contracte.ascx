<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contracte.ascx.cs" Inherits="WizOne.Personal.Contracte" %>



<body>

    <dx:ASPxButton ID="btnAct" ClientInstanceName="btnAct" ClientIDMode="Static" runat="server" OnClick="btnAct_Click"  AutoPostBack="false"   RenderMode="Link">
	    <Image Url="../Fisiere/Imagini/Icoane/m5.png"></Image>
    </dx:ASPxButton>
    <dx:ASPxGridView ID="grDateContracte" runat="server" ClientInstanceName="grDateContracte" ClientIDMode="Static" Width="80%" AutoGenerateColumns="false"  OnDataBinding="grDateContracte_DataBinding" OnInitNewRow="grDateContracte_InitNewRow"
        OnRowInserting="grDateContracte_RowInserting" OnRowUpdating="grDateContracte_RowUpdating" OnRowDeleting="grDateContracte_RowDeleting" OnCommandButtonInitialize="grDateContracte_CommandButtonInitialize">
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
        <ClientSideEvents CustomButtonClick="function(s, e) { grDateContracte_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />                       
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca"  Width="75px" Visible="false"/>
            <dx:GridViewDataComboBoxColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="250px" >
                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
                                                                                        
            <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput"  Width="100px" >
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit"  Width="100px" >  
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
            </dx:GridViewDataDateColumn>
            <dx:GridViewDataCheckColumn FieldName="Modificabil" Name="Modificabil" Caption="Modificabil" ReadOnly="true"  Visible="false"  Width="70px"  />
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



</body>