<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersIntretinere.ascx.cs" Inherits="WizOne.Personal.PersIntretinere" %>


<script type="text/javascript">
    function OnTextChangedPI(s, e) {
        var tb = grDatePersIntr.GetEditor("F11006");
        var newItem = s.GetValue();

        if (newItem.length == 13) {
            luna = parseInt(newItem.substr(3, 2));
            ziua = newItem.substr(5, 2);

            an = 0;
            tempAn = parseInt(newItem.substr(1, 2));

            switch (newItem[0]) {
                case '1':
                case '2':
                case '7':
                case '8':
                    an = 1900 + tempAn;
                    break;

                case '3':
                case '4':
                    an = 1800 + tempAn;
                    break;

                case '5':
                case '6':
                    an = 2000 + tempAn;
                    break;
            }
            var dataN = new Date(an, luna - 1, ziua);
            tb.SetValue(dataN);
        }
        grDatePersIntr.PerformCallback(s.GetValue());
    }

    function OnEndCallbackPI(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
            var tb = grDatePersIntr.GetEditor("F11006");
            tb.SetValue(null);
        }
    }
</script>
<body>

    <table width="100%">
        <tr>
            <td >
                <dx:ASPxGridView ID="grDatePersIntr" runat="server" ClientInstanceName="grDatePersIntr" Width="100%" AutoGenerateColumns="false"  OnDataBinding="grDatePersIntr_DataBinding" OnInitNewRow="grDatePersIntr_InitNewRow" 
                    OnRowInserting="grDatePersIntr_RowInserting" OnRowUpdating="grDatePersIntr_RowUpdating" OnRowDeleting="grDatePersIntr_RowDeleting" OnCellEditorInitialize="grDatePersIntr_CellEditorInitialize"
                    OnCustomCallback="grDatePersIntr_CustomCallback">        
                    <SettingsBehavior AllowFocusedRow="true" />
                    <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
                    <ClientSideEvents CustomButtonClick="function(s, e) { grDatePersIntr_CustomButtonClick(s, e); }" ContextMenu="ctx"  EndCallback="OnEndCallbackPI"/> 
                    <SettingsEditing Mode="Inline" />         
                    <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>
                    <Columns>
                        <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid"/>
                        <dx:GridViewDataTextColumn FieldName="F11010" Name="F11010" Caption="Nume"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="F11005" Name="F11005" Caption="Prenume"  Width="150px" />
                        <dx:GridViewDataTextColumn FieldName="F11012" Name="F11012" Caption="CNP"  Width="120px" />
                        <dx:GridViewDataDateColumn FieldName="F11006" Name="F11006" Caption="Data nasterii"  Width="90px" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="F11007" Name="F11007" Caption="Id"  Width="50px" Visible="false" />
                        <dx:GridViewDataCheckColumn FieldName="F11016" Name="F11016" Caption="Intretinut"  Width="50px"  />
                        <dx:GridViewDataCheckColumn FieldName="F11017" Name="F11017" Caption="Coasigurat"  Width="50px" >
                            <PropertiesCheckEdit ValueChecked="False" ValueUnchecked="True" />
                        </dx:GridViewDataCheckColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="F11004" Name="F11004" Caption="Relatie"  Width="80px" >
                            <PropertiesComboBox TextField="F71104" ValueField="F71102" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataComboBoxColumn FieldName="F11014" Name="F11014" Caption="Grad invaliditate"  Width="100px" >
                            <PropertiesComboBox TextField="F71504" ValueField="F71502" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataDateColumn FieldName="F11015" Name="F11015" Caption="Invaliditate pana la" Width="90px" HeaderStyle-Wrap="True" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="F11018" Name="F11018" Caption="Data inceput coasigurare" Width="90px" HeaderStyle-Wrap="True" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="F11019" Name="F11019" Caption="Data sfarsit coasigurare" Width="90px" HeaderStyle-Wrap="True" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="F11020" Name="F11020" Caption="Nr doc coasigurare"  Width="100px" />
                        <dx:GridViewDataDateColumn FieldName="F11021" Name="F11021" Caption="Data doc coasigurare"  Width="90px" HeaderStyle-Wrap="True" >
                                <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
                        </dx:GridViewDataDateColumn>
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