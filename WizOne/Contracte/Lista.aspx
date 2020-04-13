<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Lista.aspx.cs" Inherits="WizOne.Contracte.Lista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table style="width:100%">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnNew" ClientInstanceName="btnNew" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="true" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2"> 
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" SettingsPager-PageSize="50">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="false" AllowSelectSingleRowOnly="true" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="False" />
                    <SettingsSearchPanel Visible="False" />
                    <ClientSideEvents ContextMenu="ctx"
                        CustomButtonClick="function(s,e) { grDate_CustomButtonClick(s,e); }" 
                        EndCallback="function(s,e) { onGridEndCallback(s); }" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="50px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnDuplica">
                                    <Image ToolTip="Duplica" Url="~/Fisiere/Imagini/Icoane/clone.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnSterge">
                                    <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Id" Name="Id" Caption="Id"  Width="50px"/>
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Nume" />
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                    </Columns>
                </dx:ASPxGridView>                    
            </td>
        </tr>
    </table>

    <script>
        function grDate_CustomButtonClick(s, e) {
            if (e.buttonID == "btnSterge")
            {
                    swal({
                        title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
                    }, function (isConfirm) {
                        if (isConfirm) {
                            grDate.PerformCallback(e.buttonID + ";" + s.GetRowKey(e.visibleIndex));
                        }
                    });
            }
            else
                grDate.PerformCallback(e.buttonID + ";" + s.GetRowKey(e.visibleIndex));
        }

        function onGridEndCallback(s) {
            if (s.cpAlertMessage) {
                swal({
                    title: "Atentie",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }

    </script>
</asp:Content>