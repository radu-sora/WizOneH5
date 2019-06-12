<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Sporuri.ascx.cs" Inherits="WizOne.Personal.Sporuri" %>

<script type="text/javascript">  

    var newItem = 0;

    function OnSelectedIndexChanged1(s, e, visibleIndex) {
        var cmbChild;
        if (visibleIndex < 0)
            cmbChild = eval('cmbChild1_new');
        else
            cmbChild = eval('cmbChild1_' + visibleIndex);
        cmbChild.PerformCallback(s.GetValue());

        newItem = s.GetValue();
        for (var index = grDateSporuri1.GetTopVisibleIndex(); index < grDateSporuri1.GetVisibleRowsOnPage(); index++) {
            if (index != visibleIndex)
                grDateSporuri1.GetRowValues(index, "F02504", OnCallbackSp1);
        }
    }

    function OnSelectedIndexChanged2(s, e, visibleIndex) {
        var cmbChild;
        if (visibleIndex < 0)
            cmbChild = eval('cmbChild2_new');
        else
            cmbChild = eval('cmbChild2_' + visibleIndex);
        cmbChild.PerformCallback(s.GetValue());

        newItem = s.GetValue();
        for (var index = grDateSporuri2.GetTopVisibleIndex(); index < grDateSporuri2.GetVisibleRowsOnPage(); index++) {
            if (index != visibleIndex)
                grDateSporuri2.GetRowValues(index, "F02504", OnCallbackSp);
        }
    }

    function OnCallbackSp1(value) {
        if (value == newItem) {
            swal({
                title: "Atentie !", text: "Acest spor a mai fost deja atribuit acestui angajat!",
                type: "warning"
            });
            var tb = grDateSporuri1.GetEditor("Spor");
            tb.SetValue(null);
        }
    }

    function OnCallbackSp2(value) {
        if (value == newItem) {
            swal({
                title: "Atentie !", text: "Acest spor a mai fost deja atribuit acestui angajat!",
                type: "warning"
            });
            var tb = grDateSporuri2.GetEditor("Spor");
            tb.SetValue(null);
        }
    }
    
</script>
<body>

    <dx:ASPxGridView ID="grDateSporuri1" runat="server" ClientInstanceName="grDateSporuri1" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateSporuri1_DataBinding" 
          OnRowUpdating="grDateSporuri1_RowUpdating" >        
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents  ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />         
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			<dx:GridViewDataTextColumn FieldName="Spor" Caption="Spor" VisibleIndex="1">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbMaster1" runat="server" DataSourceID="adsMaster1" ValueType="System.Int32" ValueField="F02504" TextField="F02505" OnInit="cmbMaster1_Init">
					</dx:ASPxComboBox>
                     <asp:ObjectDataSource runat="server" ID="adsMaster1" TypeName="WizOne.Module.General" SelectMethod="GetSporuri" >                    
                        <SelectParameters>
                             <asp:Parameter Name="param"  Type="String" />
                        </SelectParameters>
                     </asp:ObjectDataSource>
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
			<dx:GridViewDataTextColumn FieldName="Tarif" Caption="Tarif" VisibleIndex="2">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbChild1" runat="server" DataSourceID="adsChild1" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild1_Callback" OnInit="cmbChild1_Init">                        
					</dx:ASPxComboBox>	  
                    <asp:ObjectDataSource runat="server" ID="adsChild1" TypeName="WizOne.Module.General" SelectMethod="GetTarifeSp" > 
                        <SelectParameters>
                             <asp:Parameter Name="categ"  Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="F02504" VisibleIndex="3" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/>            
            <dx:GridViewDataTextColumn FieldName="Id" VisibleIndex="5" Visible="false"/>      
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
 
    <dx:ASPxGridView ID="grDateSporuri2" runat="server" ClientInstanceName="grDateSporuri2" ClientIDMode="Static" Width="40%" AutoGenerateColumns="false"  OnDataBinding="grDateSporuri2_DataBinding" 
          OnRowUpdating="grDateSporuri2_RowUpdating"  >        
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents  ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />                       
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="false" ShowEditButton="true" ShowNewButtonInHeader="false" VisibleIndex="0" ButtonType="Image" Caption=" " />  
			<dx:GridViewDataTextColumn FieldName="Spor" Caption="Spor" VisibleIndex="1">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbMaster2" runat="server" DataSourceID="adsMaster2" ValueType="System.Int32" ValueField="F02504" TextField="F02505" OnInit="cmbMaster2_Init">
					</dx:ASPxComboBox>
                     <asp:ObjectDataSource runat="server" ID="adsMaster2" TypeName="WizOne.Module.General" SelectMethod="GetSporuri" >                    
                        <SelectParameters>
                             <asp:Parameter Name="param"  Type="String" />
                        </SelectParameters>
                     </asp:ObjectDataSource>
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
			<dx:GridViewDataTextColumn FieldName="Tarif" Caption="Tarif" VisibleIndex="2">
				<EditItemTemplate>
					<dx:ASPxComboBox ID="cmbChild2" runat="server" DataSourceID="adsChild2" ValueType="System.Int32" ValueField="F01105" TextField="F01107" OnCallback="cmbChild2_Callback" OnInit="cmbChild2_Init">                        
					</dx:ASPxComboBox>	  
                    <asp:ObjectDataSource runat="server" ID="adsChild2" TypeName="WizOne.Module.General" SelectMethod="GetTarifeSp" > 
                        <SelectParameters>
                             <asp:Parameter Name="categ"  Type="String" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
				</EditItemTemplate>
			</dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="F02504" VisibleIndex="3" Visible="false"/>
            <dx:GridViewDataTextColumn FieldName="F01105" VisibleIndex="4" Visible="false"/> 
            <dx:GridViewDataTextColumn FieldName="Id" VisibleIndex="5" Visible="false"/>      
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