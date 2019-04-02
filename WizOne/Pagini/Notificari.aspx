<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notificari.aspx.cs" Inherits="WizOne.Pagini.Notificari" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notificari</title>

    <script type="text/javascript" src="../Fisiere/js/utils.js" ></script>

    
    <link rel="stylesheet" type="text/css" href="../Fisiere/MsgBox/sweetalert.css" runat="server" />
    <script type="text/javascript" src="../Fisiere/MsgBox/sweetalert.min.js"></script>

    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick_Ntf(s, e) {
            switch (e.buttonID) {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode_Ntf);
                    break;
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDeleteMode_Ntf);
                    break;
                case "btnClone":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCloneMode_Ntf);
                    break;
            }
        }

        function GoToEditMode_Ntf(Value) {
            //grDate.PerformCallback("btnEdit;" + Value);
            var frm = getParameterByName('IdForm');
            window.parent.popGen.SetContentUrl('../Pagini/NotificariDetaliu.aspx?tip=Edit&id=' + Value + '&IdForm=' + frm);
            window.parent.popGen.SetHeaderText('Notificari - Detaliu');

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

        function GoToCloneMode_Ntf(Value) {
            //grDate.PerformCallback("btnClone;" + Value);
            var frm = getParameterByName('IdForm');
            window.parent.popGen.SetContentUrl('../Pagini/NotificariDetaliu.aspx?tip=Clone&id=' + Value + '&IdForm=' + frm);
            popGen.SetHeaderText('Notificari - Detaliu');
        }
    </script>


</head>
<body>
    <form id="form1" runat="server">


        <table width="100%">
            <tr>
                <td align="right">
                    <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                    </dx:ASPxButton>
                </td>
            </tr>
            <tr>
                <td>
                    <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" OnCustomCallback="grDate_CustomCallback" >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="false" AllowSelectSingleRowOnly="true" />
                        <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                        <SettingsSearchPanel Visible="False" />
                        <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick_Ntf" ContextMenu="ctx" />
                        <Columns>
                            <dx:GridViewCommandColumn Width="70px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                                <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                        <Image ToolTip="Edit" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                    <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                        <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/Sterge.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                    <dx:GridViewCommandColumnCustomButton ID="btnClone">
                                        <Image ToolTip="Duplicare" Url="~/Fisiere/Imagini/Icoane/clone.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                                </CustomButtons>
                            </dx:GridViewCommandColumn>
                            <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id" ReadOnly="true" Width="50px" />
                            <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" ReadOnly="true" />
                            <dx:GridViewDataCheckColumn FieldName="Activ" Name="Activ" Caption="Activ" ReadOnly="true" Width="50px" />
                        </Columns>
                    </dx:ASPxGridView>
                    
                </td>
            </tr>
        </table>


    </form>
</body>
</html>
