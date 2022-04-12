<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="SablonLista.aspx.cs" Inherits="WizOne.Pagini.SablonLista" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch(e.buttonID)
            {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode);
                    break;
                case "btnDelete":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDeleteMode);
                    break;
                case "btnClone":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCloneMode);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToDeleteMode(Value) {
            swal({
                title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback("btnDelete;" + Value);
                }
            });
        }

        function GoToCloneMode(Value) {
            grDate.PerformCallback("btnClone;" + Value);
        }


    </script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" OnClick="btnNew_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnDataBinding="grDate_DataBinding" OnAutoFilterCellEditorInitialize="grDate_AutoFilterCellEditorInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="true" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="70px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                    <Image ToolTip="Modifica" Url="~/Fisiere/Imagini/Icoane/edit.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnDelete">
                                    <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                                <dx:GridViewCommandColumnCustomButton ID="btnClone">
                                    <Image ToolTip="Duplicare" Url="~/Fisiere/Imagini/Icoane/clone.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                    </Columns>

                </dx:ASPxGridView>
                    
            </td>
        </tr>
    </table>

</asp:Content>
