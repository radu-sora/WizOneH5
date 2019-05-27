<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tarife.ascx.cs" Inherits="WizOne.Personal.Tarife" %>

<script type="text/javascript">  

    function OnSelectedIndexChanged(s, e, visibleIndex) {
        var cmbChild;
        if (visibleIndex < 0)
            cmbChild = eval('cmbChild_new');
        else
            cmbChild = eval('cmbChild_' + visibleIndex);
        cmbChild.PerformCallback(s.GetValue());
    }

    
</script>
<body>

    <dx:ASPxGridView ID="grDateTarife" runat="server" ClientInstanceName="grDateTarife" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateTarife_DataBinding" 
          OnRowInserting="grDateTarife_RowInserting" OnRowUpdating="grDateTarife_RowUpdating" OnCellEditorInitialize="grDateTarife_CellEditorInitialize"  >        
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents  ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />                       
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			<dx:GridViewDataTextColumn FieldName="DenCateg" Caption="Categorie" VisibleIndex="1">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbMaster" runat="server" DataSourceID="adsMaster" ValueType="System.Int32" ValueField="F01104" TextField="F01107" OnInit="cmbMaster_Init">
					</dx:ASPxComboBox>
                     <asp:ObjectDataSource runat="server" ID="adsMaster" TypeName="WizOne.Module.General" SelectMethod="GetCategTarife" />                    
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
			<dx:GridViewDataTextColumn FieldName="DenTarif" Caption="Tarif" VisibleIndex="2">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbChild" runat="server" DataSourceID="asdChild" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild_Callback" OnInit="cmbChild_Init">                        
					</dx:ASPxComboBox>	  
                    <asp:ObjectDataSource runat="server" ID="asdChild" TypeName="WizOne.Module.General" SelectMethod="GetTarife" > 
                        <SelectParameters>
                             <asp:Parameter Name="categ"  Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="F01104" VisibleIndex="3" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/> 
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
            <NewButton Image-ToolTip="Rand nou">
                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                <Styles>
                    <Style Paddings-PaddingLeft="5px" Paddings-PaddingRight="5px" />
                </Styles>
            </NewButton>
        </SettingsCommandButton>
    </dx:ASPxGridView>
 


</body>