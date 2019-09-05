<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contracte.ascx.cs" Inherits="WizOne.Personal.Contracte" %>

<script type="text/javascript">  

 
    function OnEndCallbackContracte(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }
    
</script>

<body>

    <dx:ASPxButton ID="btnAct" ClientInstanceName="btnAct" ClientIDMode="Static" runat="server" OnClick="btnAct_Click"  AutoPostBack="false"   RenderMode="Link">
	    <Image Url="../Fisiere/Imagini/Icoane/m5.png"></Image>
    </dx:ASPxButton>
    <dx:ASPxGridView ID="grDateContracte" runat="server" ClientInstanceName="grDateContracte" ClientIDMode="Static" AutoGenerateColumns="false" Width="800px"  OnDataBinding="grDateContracte_DataBinding" OnInitNewRow="grDateContracte_InitNewRow"
        OnRowInserting="grDateContracte_RowInserting" OnRowUpdating="grDateContracte_RowUpdating" OnRowDeleting="grDateContracte_RowDeleting" OnCommandButtonInitialize="grDateContracte_CommandButtonInitialize">
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />   
        <ClientSideEvents CustomButtonClick="function(s, e) { grDateContracte_CustomButtonClick(s, e); }" ContextMenu="ctx" EndCallback="OnEndCallbackContracte"/> 
        <SettingsEditing Mode="Inline" />        
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
        <Columns>
            <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />

            <dx:GridViewDataComboBoxColumn FieldName="IdContract" Name="IdContract" Caption="Contract"  Width="350px" >
                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>
                                                                                        
            <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data inceput" >
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                    <ValidationSettings>
                        <RequiredField IsRequired="true" ErrorText="Camp obligatoriu" />
                    </ValidationSettings>
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>

            <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data sfarsit" >  
                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy">
                    <ValidationSettings>
                        <RequiredField IsRequired="true" ErrorText="Camp obligatoriu" />
                    </ValidationSettings>
                </PropertiesDateEdit>
            </dx:GridViewDataDateColumn>

            <dx:GridViewDataCheckColumn FieldName="Modificabil" Name="Modificabil" Caption="Modificabil" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" ShowInCustomizationForm="false" />						
            <dx:GridViewDataDateColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" ShowInCustomizationForm="false" />    
            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ShowInCustomizationForm="false"/>
            <dx:GridViewDataTextColumn FieldName="F10003" Name="F10003" Caption="Marca" Visible="false" ShowInCustomizationForm="false"/>            
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