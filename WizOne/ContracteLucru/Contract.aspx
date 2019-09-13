<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Contract.aspx.cs" Inherits="WizOne.ContracteLucru.Contract" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID){
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode);
                    break;
                case "btnDuplica":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToCloneMode);
                    break;
                case "btnSterge":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDeleteMode);
                    break;
            }
        }

        function GoToEditMode(Value) {
            grDate.PerformCallback("btnEdit;" + Value);
        }

        function GoToCloneMode(Value) {
            grDate.PerformCallback("btnDuplica;" + Value);
        }

        function GoToDeleteMode(Value) {
            swal({
                title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
            }, function (isConfirm) {
                if (isConfirm) {
                    grDate.PerformCallback("btnSterge;" + Value);
                }
            });
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
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnDataBinding="grDate_DataBinding" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="false" AllowSelectSingleRowOnly="true" />
                    <Settings ShowFilterRow="True" ShowGroupPanel="False" />
                    <SettingsSearchPanel Visible="False" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick"  ContextMenu="ctx"/>
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
                        <dx:GridViewDataComboBoxColumn FieldName="TipContract" Name="TipContract" Caption="Tip contract" >
                            <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                        </dx:GridViewDataComboBoxColumn>
                        <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="75px" Visible="false"/>
                    </Columns>
                </dx:ASPxGridView>                    
            </td>
        </tr>
    </table>
</asp:Content>