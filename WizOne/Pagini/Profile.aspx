<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="WizOne.Pagini.Profile" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>

    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script type="text/javascript">

        function grDate_CustomButtonClick_Ntf(s, e) {
            switch (e.buttonID) {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode_Ntf);
                    break;
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDeleteMode_Ntf);
                    break;
                case "btnShow":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToShowMode_Ntf);
                    break;
            }
        }

        function GoToEditMode_Ntf(Value) {
            //grDate.PerformCallback("btnEdit;" + Value);
            var frm = getParameterByName('IdForm');
            window.parent.popGen.SetContentUrl('../Pagini/ProfileDetaliu.aspx?tip=Edit&id=' + Value);
        }

        function GoToDeleteMode_Ntf(Value) {
            swal({
                title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback("btnDelete;" + Value);
                }
            });
        }

        function GoToShowMode_Ntf(Value) {
            grDate.PerformCallback("btnShow;" + Value);
        }

        function ClosePopWindow() {
            window.parent.popGen.Hide();
        }


    </script>


</head>
<body>
    <form id="form1" runat="server">


        <table style="width:100%;">
            <tr>
                <td style="float:right; text-align:right;">
                    <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" OnClick="btnNew_Click" HorizontalAlign="Right" oncontextMenu="ctx(this,event)" Visible="false" >
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnArata" runat="server" Text="Arata" OnClick="btnArata_Click" HorizontalAlign="Right" oncontextMenu="ctx(this,event)" Visible="true" >
                        <Image Url="~/Fisiere/Imagini/Icoane/intre.png"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnCustomCallback="grDate_CustomCallback" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize" >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />
                        <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick_Ntf" ContextMenu="ctx" />
                        <Columns>
                            <dx:GridViewCommandColumn Width="70px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                                <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="btnShow">
                                        <Image ToolTip="Arata" Url="~/Fisiere/Imagini/Icoane/intre.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                    <dx:GridViewCommandColumnCustomButton ID="btnEdit" Visibility="Invisible">
                                        <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                    <dx:GridViewCommandColumnCustomButton ID="btnDelete" Visibility="Invisible">
                                        <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/Sterge.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                </CustomButtons>
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" ReadOnly="true" />
                            <dx:GridViewDataCheckColumn FieldName="Activ" Name="Activ" Caption="Activ" ReadOnly="true" Width="50px" />
                            <dx:GridViewDataCheckColumn FieldName="Implicit" Name="Implicit" Caption="Implicit" ReadOnly="true" Width="50px" />                            
                        </Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
