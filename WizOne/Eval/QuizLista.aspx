<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="QuizLista.aspx.cs" Inherits="WizOne.Eval.QuizLista" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width:100%;">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">
                <dx:ASPxButton ID="btnNew360" runat="server" Text="Nou 360" OnClick="btnNew360_Click" Visible="false" >
                    <Image Url="../Fisiere/Imagini/Icoane/New.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDuplicare" runat="server" Text="Duplicare" AutoPostBack="false">
                    <Image Url="../Fisiere/Imagini/Icoane/duplicare.png" />
                    <ClientSideEvents Click="function(s,e) { grDate.PerformCallback('btnDuplicare'); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnNew" runat="server" Text="Nou" OnClick="btnNew_Click" >
                    <Image Url="../Fisiere/Imagini/Icoane/New.png" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu ="ctx(this, event)" >
                    <Image Url="../Fisiere/Imagini/Icoane/iesire.png" />
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
             <td colspan="2" style="margin-top:15px; display:inline-block; width:100%;">
                <div class="row">
                    <div class="col-md-12" style="margin-bottom:20px;">
                        <div class="ctl_inline">
                            <dx:ASPxLabel ID="lblPeriod" Text="Perioada:" runat="server" Width="80" />
                            <dx:ASPxComboBox ID="cmbPerioada" ClientInstanceName="cmbPerioada" Width="150" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" />
                        </div>
                        <div class="ctl_inline">
                            <dx:ASPxLabel ID="lblDtInceput" runat="server" Text="Data inceput" Width="80" />
                            <dx:ASPxDateEdit ID="txtDtInc" ClientInstanceName="txtDtInc" Width="100" runat="server" AutoPostBack="false" >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                            </dx:ASPxDateEdit>
                        </div>
                        <div class="ctl_inline">
                            <dx:ASPxLabel ID="lblDtSfarsit" runat="server" Text="Data sfarsit" Width="80" />
                            <dx:ASPxDateEdit ID="txtDtSf" ClientInstanceName="txtDtSf" Width="100" runat="server" AutoPostBack="false" >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                            </dx:ASPxDateEdit>
                        </div>
                        <div class="ctl_inline">
                            <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" AutoPostBack="false">
                                <Image Url="../Fisiere/Imagini/Icoane/lupa.png" />
                                <ClientSideEvents Click="function(s,e) { grDate.PerformCallback('btnFiltru'); }" />
                            </dx:ASPxButton>
                        </div>
                        <div class="ctl_inline">
                            <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                                <ClientSideEvents Click="function(s,e) { EmptyFields(); }" />
                            </dx:ASPxButton>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-top:15px;">
                <dx:ASPXGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  
                    OnCustomCallback="grDate_CustomCallback">
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" />
                    <Settings ShowFilterRow="true" ShowGroupPanel="false" />
                    <SettingsSearchPanel Visible="true" />
                    <ClientSideEvents CustomButtonClick="function(s,e) { grDate_CustomButtonClick(s,e); }" ContextMenu="ctx" />
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


    <script>
        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnEdit":
                    grDate.PerformCallback("btnEdit");
                    break;
                case "btnSterge":
                    swal({
                        title: "Sunteti sigur/a ?", text: "Informatia va fi stearsa si nu va putea fi recuperata !",
                        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, sterge!", cancelButtonText: "Renunta", closeOnConfirm: true
                    }, function (isConfirm) {
                        if (isConfirm) {
                            grDate.PerformCallback("btnSterge");
                        }
                    });
                    break;
            }
        }

        function EmptyFields(s, e) {
            cmbPerioada.SetValue(null);
            txtDtInc.SetValue(null);
            txtDtSf.SetValue(null);
        }
    </script>
</asp:Content>

