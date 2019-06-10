<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProgrameAlteOre.ascx.cs" Inherits="WizOne.ProgrameLucru.ProgrameAlteOre" %>



<body>


    <table width="80%">
     <tr>
        <th class="legend-font-size">Setari ore de noapte</th>
        <dx:ASPxGridView ID="grDateAlteOre" runat="server" ClientInstanceName="grDateAlteOre" ClientIDMode="Static" Width="20%" AutoGenerateColumns="false"  OnDataBinding="grDateAlteOre_DataBinding" OnInitNewRow="grDateAlteOre_InitNewRow"
                OnRowInserting="grDateAlteOre_RowInserting" OnRowUpdating="grDateAlteOre_RowUpdating" OnRowDeleting="grDateAlteOre_RowDeleting">        
            <SettingsBehavior AllowFocusedRow="true" />
            <Settings ShowFilterRow="False" ShowColumnHeaders="true"  />  
            <ClientSideEvents CustomButtonClick="function(s, e) { grDateAlteOre_CustomButtonClick(s, e); }" ContextMenu="ctx" /> 
            <SettingsEditing Mode="Inline" />                       
            <Columns>
                <dx:GridViewCommandColumn Width="75px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                <dx:GridViewDataTextColumn FieldName="IdProgram" Name="IdProgram" Caption="Program"  Width="75px" Visible="false"/>                                    
                <dx:GridViewDataComboBoxColumn FieldName="Rotunjire" Name="Rotunjire" Caption="Rotunjire" Width="200px" >
                    <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                </dx:GridViewDataComboBoxColumn>  
                <dx:GridViewDataDateColumn FieldName="OraInceput" Name="OraInceput" Caption="Ora Inceput" Width="60px" >  
                    <PropertiesDateEdit DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom">
                        <DropDownButton Visible="False">
                        </DropDownButton>
                        <ClientSideEvents DropDown="function(s, e) {  s.HideDropDown();   }" />                        
                    </PropertiesDateEdit>  
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="OraSfarsit" Name="OraSfarsit" Caption="Ora Sfarsit" Width="60px"  >  
                    <PropertiesDateEdit DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom">
                        <DropDownButton Visible="False">
                        </DropDownButton>
                        <ClientSideEvents DropDown="function(s, e) {  s.HideDropDown();   }" />                        
                    </PropertiesDateEdit>  
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="ValMin" Name="ValMin" Caption="Val Min" Width="60px"  >  
                    <PropertiesDateEdit DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom">
                        <DropDownButton Visible="False">
                        </DropDownButton>
                        <ClientSideEvents DropDown="function(s, e) {  s.HideDropDown();   }" />                        
                    </PropertiesDateEdit>  
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="ValMax" Name="ValMax" Caption="Val Max" Width="60px" >  
                    <PropertiesDateEdit DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom">
                        <DropDownButton Visible="False">
                        </DropDownButton>
                        <ClientSideEvents DropDown="function(s, e) {  s.HideDropDown();   }" />                        
                    </PropertiesDateEdit>  
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataDateColumn FieldName="ValFixa" Name="ValFixa" Caption="Val Fixa" Width="60px" >  
                    <PropertiesDateEdit DisplayFormatString="HH:mm" EditFormatString="HH:mm" EditFormat="Custom">
                        <DropDownButton Visible="False">
                        </DropDownButton>
                        <ClientSideEvents DropDown="function(s, e) {  s.HideDropDown();   }" />                        
                    </PropertiesDateEdit>  
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataTextColumn FieldName="Multiplicator" Name="Multiplicator" Caption="Multiplicator" Width="75px">
                    <PropertiesTextEdit DisplayFormatInEditMode="true" DisplayFormatString="N2"></PropertiesTextEdit>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn FieldName="Camp" Name="Camp" Caption="Trimite la" Width="200px" >
                    <PropertiesComboBox TextField="Alias" ValueField="Denumire" ValueType="System.String" DropDownStyle="DropDown" />
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