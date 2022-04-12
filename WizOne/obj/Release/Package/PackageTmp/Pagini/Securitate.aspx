<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Securitate.aspx.cs" Inherits="WizOne.Pagini.Securitate" %>



<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

        <script type="text/javascript">
            function grSec_OnNewClick(s, e) {
                grSec.AddNewRow();
            }

            function grSec_OnCancelClick(s, e) {
                grSec.CancelEdit();
            }
        </script>

        <table style="width:100%;">
            <tr style="float:right;">
                <td>
                    <dx:ASPxButton ID="btnNewSec" runat="server" Text="Nou" AutoPostBack="false"  >
                        <ClientSideEvents Click="function (s, e) { grSec_OnNewClick(s, e); }" />
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnSaveSec" runat="server" Text="Salveaza" OnClick="btnSaveSec_Click">
                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                    </dx:ASPxButton>
                   

                </td>
            </tr>
            <tr>
                <td colspan="5">
                    <dx:ASPxGridView ID="grSec" runat="server" ClientInstanceName="grSec"  OnRowDeleting="grSec_RowDeleting" Width="100%" OnAutoFilterCellEditorInitialize="grSec_AutoFilterCellEditorInitialize"
                        OnRowInserting="grSec_RowInserting" OnRowUpdating="grSec_RowUpdating" OnInitNewRow="grSec_InitNewRow" >
                        <SettingsBehavior AllowFocusedRow="True" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />

                        <Columns>
                            <dx:GridViewCommandColumn Width="50px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />
                            <dx:GridViewDataTextColumn FieldName="IdForm" Name="Pagina" Caption="Pagina" Visible="false" />
                            <dx:GridViewDataTextColumn FieldName="IdControl" Name="Control" Caption="Control" Visible="false" />
                            <dx:GridViewDataTextColumn FieldName="IdColoana" Name="Coloana" Caption="Coloana" Visible="false" />
                            <dx:GridViewDataComboBoxColumn FieldName="IdGrup" Name="Grup" Caption="Grup" Visible="true">
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown">
                                </PropertiesComboBox>
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataCheckColumn FieldName="Vizibil" Name="Vizibil" Caption="Vizibil" Visible="true" Width="50px" />
                            <dx:GridViewDataCheckColumn FieldName="Blocat" Name="Blocat" Caption="Blocat" Visible="true" Width="50px" />
                        </Columns>

                        <SettingsCommandButton>
                            <UpdateButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                <Styles>
                                    <Style Paddings-PaddingRight="5px" />
                                </Styles>
                            </UpdateButton>
                            <CancelButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                            </CancelButton>

                            <EditButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                <Styles>
                                    <Style Paddings-PaddingRight="5px" />
                                </Styles>
                            </EditButton>
                            <DeleteButton>
                                <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                            </DeleteButton>
                        </SettingsCommandButton>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
