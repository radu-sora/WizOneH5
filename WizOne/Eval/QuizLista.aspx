<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="QuizLista.aspx.cs" Inherits="WizOne.Eval.QuizLista" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">


        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID)
            {
                case "btnEdit":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToEditMode);
                    break;
                case "btnSterge":
                    grDate.GetRowValues(e.visibleIndex, 'Id', GoToDeleteMode);
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
                    grDate.PerformCallback("btnSterge;" + Value);
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr style="column-span=2">
            <td style="text-align=left;width="23px">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right" >
                <dx:ASPxButton ID="btnNew360" runat="server" Text="Nou 360" OnClick="btnNew360_Click" Visible="false" >
                    <Image Url="../Fisiere/Imagini/Icoane/New.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDuplicare" runat="server" Text="Duplicare" OnClick="btnDuplicare_Click" >
                    <Image Url="../Fisiere/Imagini/Icoane/duplicare.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" OnClick="btnNew_Click" >
                    <Image Url="../Fisiere/Imagini/Icoane/New.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu ="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <table style="margin:15px 0px 15px 15px;">
        <tr>
            <td >
                <dx:ASPxLabel Width="80" ID="lblPeriod" Text="Perioada:" runat="server" />
            </td>
              <td width="180">
                <dx:ASPxComboBox Width="150" ID="cmbPerioada" ClientInstanceName="cmbPerioada" ClientIDMode="Static" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" />
            </td>
            <td >
                <dx:ASPxLabel ID="lblDtInceput" Width="80" runat="server" Text="Data inceput:" />
            </td>
            <td width="130">
                <dx:ASPxDateEdit ID="dtInceput" Width="100" runat="server" AutoPostBack="false" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>
            </td>
            <td>
                <dx:ASPxLabel ID="lblDtSfarsit" Width="80" runat="server" Text="Data sfarsit:" />
            </td>
            <td width="130">
                <dx:ASPxDateEdit ID="dtSfarsit" Width="100" runat="server" AutoPostBack="false" >
                    <CalendarProperties FirstDayOfWeek="Monday" />
                </dx:ASPxDateEdit>
            </td>
            <td>
                <dx:ASPxButton ID="btnFiltru" runat="server" OnClick="btnFiltru_Click">
                    <Image Url="../Fisiere/Imagini/Icoane/lupa.png" />
                </dx:ASPxButton>
            </td>
        </tr>
        </table>
    <table>
        <tr>
            <td />
        </tr>
        <tr>
            <td />
        </tr>
        </table>

    <table>
        <tr>
            <td colspan="7">
                <dx:ASPXGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  
                    OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="false" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents CustomButtonClick="grDate_CustomButtonClick" ContextMenu="ctx" />
                    <Columns>
                        <dx:GridViewCommandColumn Width="50px" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" >
                            <CustomButtons>
                                    <dx:GridViewCommandColumnCustomButton ID="btnEdit">
                                        <Image ToolTip="Modifica" Url="../Fisiere/Imagini/Icoane/edit.png" />
                                    </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnSterge">
                                    <Image ToolTip="Sterge" Url="../Fisiere/Imagini/Icoane/sterge.png" />
                                </dx:GridViewCommandColumnCustomButton>
                            </CustomButtons>
                        </dx:GridViewCommandColumn>

                        <dx:GridViewDataTextColumn FieldName="Id" Name ="Id" Caption="Id" VisibleIndex="0" Width="50px" />
                        <dx:GridViewDataTextColumn FieldName="Denumire" Name="Denumire" Caption="Denumire" VisibleIndex="1" Width="350px" />
                        <dx:GridViewDataTextColumn FieldName="Titlu" Name="Titlu" Caption="Titlu" VisibleIndex="2" Width="350px" />
                        <dx:GridViewDataDateColumn FieldName="DataInceput" Name="DataInceput" Caption="Data Inceput" VisibleIndex="3" Width="200px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataDateColumn FieldName="DataSfarsit" Name="DataSfarsit" Caption="Data Sfarsit" VisibleIndex="4" Width="200px">
                            <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                        </dx:GridViewDataDateColumn>
                        <dx:GridViewDataTextColumn FieldName="Perioada" Name="Perioada" Caption="Perioada" VisibleIndex="5" Width="200px" />
                    </Columns>
                </dx:ASPXGridView>
            </td>
        </tr>
    </table>
</asp:Content>