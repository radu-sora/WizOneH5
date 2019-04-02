<%@ Page Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ListaObiective.aspx.cs" Inherits="WizOne.Eval.ListaObiective" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script language="javascript" type="text/javascript">

        var idObi = null;

        function OnObiectivChanged(cmbObi) {
            if (grDate.GetEditor("IdActivitate").InCallback())
                idObi = cmbObi.GetValue().toString();
            else
                grDate.GetEditor("IdActivitate").PerformCallback(cmbObi.GetValue().toString());

            var ert = cmbObi.GetValue();
            hf.Set("CurrentObjective", ert);
            var wswx = grDate.GetEditor("IdActivitate");
        }

        function OnEndCallback(s, e) {
            if (idObi) {
                grDate.GetEditor("IdActivitate").PerformCallback(idObi);
                idObi = null;
            }
        }



        //var currentEditingIndex;
        //var lastObjective;
        //var isCustomCascadingCallback = false;

        //function RefreshData(objectiveValue) {
        //    hf.Set("CurrentObjective", objectiveValue);
        //    ActivityEditor.PerformCallback();
        //}

        //function cmbObiectiv_SelectedIndexChanged(s, e) {
        //    grDate.PerformCallback();
        //    //lastObjective = s.GetValue();
        //    //isCustomCascadingCallback = true;
        //    //RefreshData(lastObjective);
        //}

        //function cmbActivity_EndCallBack(s, e) {
        //    if (isCustomCascadingCallback) {
        //        if (s.GetItemCount() > 0)
        //            grDate.BatchEditApi.SetCellValue(currentEditingIndex, "IdActivitate", s.GetItem(0).value);
        //        isCustomCascadingCallback = false;
        //    }
        //}

        //function OnBatchEditStartEditing(s, e) {
        //    currentEditingIndex = e.visibleIndex;
        //    var currentObjective = grid.BatchEditApi.GetCellValue(currentEditingIndex, "IdObiectiv");
        //    if (currentObjective != lastObjective && e.focusedColumn.fieldName == "IdActivitate" && currentObjective != null) {
        //        lastObjective = currentObjective;
        //        RefreshData(currentObjective);
        //    }
        //}
    </script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/sgSt.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="false" oncontextMenu="ctx(this, event)">
                    <ClientSideEvents Click="function(s, e) { OnNewClick(s, e); }" />
                    <Image Url="../Fisiere/Imagini/Icoane/new.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this, event)">
                    <ClientSideEvents Click="function(s, e){
                            pnlLoading.Show();
                            e.processOnServer = true;
                        }" />
                    <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this, event)">
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <table>
        <tr>
            <td>
                <dx:ASPxLabel ID="lblId" runat="server" Text="Id" Width="30px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtId" runat="server" Width="50px" Enabled="false" />
            </td>
            <td>
                <dx:ASPxLabel ID="lblCodLista" runat="server" Text="Cod Lista" Width="50px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtCodLista" runat="server" Width="150px" />
            </td>
            <td>
                <dx:ASPxLabel ID="lblDenLista" runat="server" Text="Denumire" Width-="50px" />
            </td>
            <td>
                <dx:ASPxTextBox ID="txtDenLista" runat="server" Width="250px" />
            </td>
        </tr>
    </table>
        
    <table width="100%">
        <tr>
            <td colspan="5">
                <dx:ASPxHiddenField runat="server" ID="hf" ClientInstanceName="hf" />
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" OnRowDeleting="grDate_RowDeleting" Width="100%" 
                    AutoGenerateColumns="false" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize" OnCellEditorInitialize="grDate_CellEditorInitialize"
                    OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnInitNewRow="grDate_InitNewRow" OnCustomErrorText="grDate_CustomErrorText">
                    <SettingsBehavior AllowFocusedRow="true" EnableCustomizationWindow="true" AllowSelectByRowClick="true" ColumnResizeMode="NextColumn" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="true" HorizontalScrollBarMode="Auto" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents ContextMenu="ctx" />
                    <SettingsEditing Mode="Inline" />

                    <Columns>
                        <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                        <dx:GridViewDataTextColumn FieldName="IdLista" Name="IdLista" Caption="IdLista" Visible="false" />
                        
                        <dx:GridViewDataComboBoxColumn FieldName="IdObiectiv" Name="IdObiectiv" Caption="Obiectiv" Width="250px" VisibleIndex="1" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" EnableSynchronization="false" IncrementalFilteringMode="StartsWith" DropDownStyle="DropDown" >
                                <ValidationSettings RequiredField-IsRequired="true" Display="None" />
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnObiectivChanged(s); }" />
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="2">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="false" IncrementalFilteringMode="StartsWith">
                                <ClientSideEvents EndCallback="OnEndCallback" />
                            </PropertiesComboBox>
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataComboBoxColumn FieldName="IdSetAngajat" Name="IdSetAngajat" Caption="Set angajat" Width="250px" VisibleIndex="3">
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableCallbackMode="true" />
                        </dx:GridViewDataComboBoxColumn>

                        <dx:GridViewDataTextColumn FieldName="Target" Name="Target" Caption="Target" Width="150px" VisibleIndex="4" >
                            <PropertiesTextEdit DisplayFormatString="n2" >
                                <MaskSettings Mask="<0..999g>.<0..99g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataTextColumn FieldName="Ordine" Name="Ordine" Caption="Ordine" Width="150px" VisibleIndex="5" >
                            <PropertiesTextEdit DisplayFormatString="n2" >
                                <MaskSettings Mask="<0..999g>" />
                            </PropertiesTextEdit>
                        </dx:GridViewDataTextColumn>
                        <dx:GridViewDataCheckColumn FieldName="Vizibil" Name="Vizibil" Caption="Vizibil" Width="70px" VisibleIndex="6"  />
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" Visible="false" />
                    </Columns>
                    <SettingsCommandButton>
                        <UpdateButton>
                            <Image Url="../Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </UpdateButton>
                        <CancelButton>
                            <Image Url="../Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                        </CancelButton>

                        <EditButton>
                            <Image Url="../Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                        <DeleteButton>
                            <Image Url="../Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                        </DeleteButton>
                    </SettingsCommandButton>
                </dx:ASPxGridView>
                <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="grid" />
            </td>
        </tr>
    </table>
</asp:Content>