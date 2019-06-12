<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Componente.ascx.cs" Inherits="WizOne.Personal.Componente" %>

<script type="text/javascript">  
    var newItem = 0;

    function OnValueChangedComp(s, e) {
        gasit = false;
        newItem = s.GetValue();
        for (var index = grDateComponente.GetTopVisibleIndex(); index < grDateComponente.GetVisibleRowsOnPage(); index++) {  
            grDateComponente.GetRowValues(index, "F02104", OnCallbackComp);          
        }  
        
    }

    function OnCallbackComp(value) {
        if (value == newItem) {
            swal({
                title: "Atentie !", text: "Codul a mai fost deja atribuit acestui angajat!",
                type: "warning"
            });
            var cb = grDateComponente.GetEditor("F02104"); 
            cb.SetValue(null);
        }
    }

    function OnTextChangedComp(s, e) {
        var val = s.GetValue();
        if (val < 0) {
            swal({
                title: "Atentie !", text: "Suma nu poate fi negativa!",
                type: "warning"
            });       
            var tb = grDateComponente.GetEditor("Suma"); 
            tb.SetValue("0");
        }
    }
</script>
<body>

    <dx:ASPxGridView ID="grDateComponente" runat="server" ClientInstanceName="grDateComponente" ClientIDMode="Static" Width="30%" AutoGenerateColumns="false"  OnDataBinding="grDateComponente_DataBinding" 
          OnRowInserting="grDateComponente_RowInserting" OnRowUpdating="grDateComponente_RowUpdating" OnCellEditorInitialize="grDateComponente_CellEditorInitialize">        
        <SettingsBehavior AllowFocusedRow="true" />
        <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
        <ClientSideEvents  ContextMenu="ctx" /> 
        <SettingsEditing Mode="Inline" />       
        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
        <Columns>
            <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />          
            <dx:GridViewDataComboBoxColumn FieldName="F02104" Name="F02104" Caption="Componenta" Width="250px" >
                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
            </dx:GridViewDataComboBoxColumn>         
            <dx:GridViewDataTextColumn FieldName="Suma" Name="Suma" Caption="Suma"    Width="100px"  />          
              
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