﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="relSesiuneCosturi.aspx.cs" Inherits="WizOne.Curs.relSesiuneCosturi" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="lblTitlu" runat="server"></title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" >

    </script>
    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script language="javascript" type="text/javascript">
        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxButton ID="btnSave"  runat="server" RenderMode="Button" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)" >
                        <ClientSideEvents Click="function(s, e) {
                            e.processOnServer = true;
                            window.close();
                        }" />
                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                    </dx:ASPxButton>

                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnRowUpdating="grDate_RowUpdating" OnRowInserting="grDate_RowInserting" OnRowDeleting="grDate_RowDeleting" OnInitNewRow="grDate_InitNewRow"  >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" AllowSort="false" />
                        <SettingsEditing Mode="EditFormAndDisplayRow" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />
                         <ClientSideEvents  ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />
                        <Columns>
                            <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" SelectAllCheckboxMode="AllPages" />
                            <dx:GridViewCommandColumn Width="90px" VisibleIndex="1" ButtonType="Image" ShowEditButton="true" ShowDeleteButton="true" ShowNewButtonInHeader="true" Caption=" " Name="butoaneGrid" /> 
                            <dx:GridViewDataTextColumn FieldName="IdCurs" Name="IdCurs" Caption="IdCurs" ReadOnly="true" Width="50px" Visible="false"/>
                            <dx:GridViewDataTextColumn FieldName="IdSesiune" Name="IdSesiune" Caption="IdSesiune" ReadOnly="true" Width="50px" Visible="false"/>
                            <dx:GridViewDataComboBoxColumn FieldName="IdCost" Name="IdCost" Caption="Cost"  Width="150px">
                                <PropertiesComboBox TextField="DenumireCost" ValueField="IdCost" ValueType="System.Int32" DropDownStyle="DropDown" />
                                <Settings FilterMode="DisplayText" />
                            </dx:GridViewDataComboBoxColumn>                               
                            <dx:GridViewDataTextColumn FieldName="Valoare" Name="Valoare" Caption="Valoare"  Width="50px"/>
                            <dx:GridViewDataComboBoxColumn FieldName="IdFurnizor" Name="IdFurnizor" Caption="Furnizor"  Width="150px">
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                <Settings FilterMode="DisplayText" />
                            </dx:GridViewDataComboBoxColumn>
                            <dx:GridViewDataComboBoxColumn FieldName="IdCentruCost" Name="IdCentruCost" Caption="Centru cost"  Width="150px">
                                <PropertiesComboBox TextField="CentruCost" ValueField="IdCentruCost" ValueType="System.Int32" DropDownStyle="DropDown" />
                                <Settings FilterMode="DisplayText" />
                            </dx:GridViewDataComboBoxColumn>

                            <dx:GridViewDataTextColumn FieldName="IdTipCost" Name="IdTipCost" Caption="IdTipCost" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                            <dx:GridViewDataTextColumn FieldName="NumeTipCost" Name="NumeTipCost" Caption="NumeTipCost" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />

                            <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" ReadOnly="true" Width="75px" Visible="false" ShowInCustomizationForm="false" />
                            <dx:GridViewDataTextColumn FieldName="USER_NO" Name="USER_NO" Caption="USER_NO" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                            <dx:GridViewDataTextColumn FieldName="TIME" Name="TIME" Caption="TIME" ReadOnly="true" Width="50px" Visible="false" ShowInCustomizationForm="false"/>
                        </Columns>
                    <SettingsCommandButton>
                        <UpdateButton ButtonType="Link" Text="Actualizeaza">
                            <Styles>
                                <Style Paddings-PaddingRight="10" Paddings-PaddingTop="10" Font-Bold="true">
                                </Style>
                            </Styles>
                        </UpdateButton>
                        <CancelButton ButtonType="Link" Text="Anulare"  Image-ToolTip="Anulare">
                            <Styles>
                                <Style Font-Bold="true" />
                            </Styles>
                        </CancelButton>

                        <EditButton Image-ToolTip="Edit">
                            <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" />
                            <Styles>
                                <Style Paddings-PaddingRight="5px" />
                            </Styles>
                        </EditButton>
                        <DeleteButton Image-ToolTip="Sterge">
                            <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" />
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


    </form>
</body>
</html>
