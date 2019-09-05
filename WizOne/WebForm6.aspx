<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="WebForm6.aspx.cs" Inherits="WizOne.WebForm6" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>
        function grCC_OnNewClick(s, e) {
            grCC.AddNewRow();
        }

        function grCC_OnCancelClick(s, e) {
            grCC.CancelEdit();
        }

        function grCC_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDeleteCC":
                    grCC.PerformCallback('btnDeleteCC;' + s.GetRowKey(e.visibleIndex));
                    break;
            }
        }
    </script>

                                <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="true" 
                                OnCustomCallback="grCC_CustomCallback" OnBatchUpdate="grCC_BatchUpdate" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared" 
                                OnParseValue="grCC_ParseValue" OnCellEditorInitialize="grCC_CellEditorInitialize" OnCustomErrorText="grCC_CustomErrorText" >
                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" VerticalScrollableHeight="130" />
                                <SettingsSearchPanel Visible="false" />
                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                                <ClientSideEvents ContextMenu="ctx" CustomButtonClick="grCC_CustomButtonClick" EndCallback="function(s,e) { OnEndCallback(s,e); }"/>

                                <Columns>
                                    <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true" >
                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC">
                                                <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" ShowInCustomizationForm="false" Width="150px" VisibleIndex="1" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="2" Visible="true">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDownList">
                                            <ValidationSettings>
                                                <RequiredField IsRequired="true" ErrorText="Camp obligatoriu" />
                                            </ValidationSettings>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="3" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProiectChanged(s); }"></ClientSideEvents>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdSubproiect" Name="IdSubproiect" Caption="SubProiect" Width="250px" VisibleIndex="4" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnSubproiectChanged(s); }" EndCallback="OnSubEndCallback"/>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="5" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        <ClientSideEvents EndCallback="OnActEndCallback" />
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="6" Visible="false">
                                        <PropertiesComboBox TextField="Dept" ValueField="IdDept" ValueType="System.Int32" DropDownStyle="DropDown">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Dept" Caption="Dept" Width="130px" />
                                            </Columns>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="7" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="8" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>


                                    <dx:GridViewDataTextColumn FieldName="NrOre1" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>" />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>


                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre2" Name="NrOre2" Caption="NrOre2" Width="100px" Visible="false" VisibleIndex="10" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre3" Name="NrOre3" Caption="NrOre3" Width="100px" Visible="false" VisibleIndex="11" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre4" Name="NrOre4" Caption="NrOre4" Width="100px" Visible="false" VisibleIndex="12" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre5" Name="NrOre5" Caption="NrOre5" Width="100px" Visible="false" VisibleIndex="13" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre6" Name="NrOre6" Caption="NrOre6" Width="100px" Visible="false" VisibleIndex="14" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre7" Name="NrOre7" Caption="NrOre7" Width="100px" Visible="false" VisibleIndex="15" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre8" Name="NrOre8" Caption="NrOre8" Width="100px" Visible="false" VisibleIndex="16" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre9" Name="NrOre9" Caption="NrOre9" Width="100px" Visible="false" VisibleIndex="17" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre10" Name="NrOre10" Caption="NrOre10" Width="100px" Visible="false" VisibleIndex="18" PropertiesSpinEdit-MinValue="0" PropertiesSpinEdit-MaxValue="24" />



                                    <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                                </Columns>
                                
                                <SettingsCommandButton>
                                    <NewButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                    </NewButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>
                                </SettingsCommandButton>

                            </dx:ASPxGridView>



</asp:Content>
