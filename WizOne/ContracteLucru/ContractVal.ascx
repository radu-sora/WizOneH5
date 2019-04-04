<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContractVal.ascx.cs" Inherits="WizOne.ContracteLucru.ContractVal" %>



<body>


    <table width="60%">
     <tr>
        <th class="legend-font-size">Asignare rezultat in Val-uri</th>
        <dx:ASPxGridView ID="grDateCtrVal" runat="server" ClientInstanceName="grDateCtrVal" ClientIDMode="Static" Width="20%" AutoGenerateColumns="false"  OnDataBinding="grDateCtrVal_DataBinding" OnInitNewRow="grDateCtrVal_InitNewRow"
                OnRowInserting="grDateCtrVal_RowInserting" OnRowUpdating="grDateCtrVal_RowUpdating" OnRowDeleting="grDateCtrVal_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDateCtrVal_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="75px" Visible="false"/>                                    
                <dx:GridViewDataComboBoxColumn FieldName="Val_uri" Name="Val_uri" Caption="Val_uri" Width="175px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  
                <dx:GridViewDataComboBoxColumn FieldName="F_uri" Name="F_uri" Caption="F_uri" Width="60px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn> 
                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>      
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
    </tr>
   </table>
</body>