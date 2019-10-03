<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="EvalBulk.aspx.cs" Inherits="WizOne.Eval.EvalBulk" %>



<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    
    <script language="javascript" type="text/javascript">
    

        function GoToDeleteMode(Value) {
            if (Value == 0 || Value == -1) {
                swal({
                    title: "Operatie nepermisa", text: "Nu puteti anula o cerere deja anulata sau respinsa",
                    type: "warning"
                });
            }
            else {
                swal({
                    title: "Sunteti sigur/a ?", text: "Invitatia va fi anulata !",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da, anuleaza!", cancelButtonText: "Renunta", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnDelete");
                    }
                });
            }
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function OnTrimite(s, e) {
            grDate.PerformCallback("btnTrimite");
            cmbQuiz.SetValue(null);
            cmbAng.SetValue(null);
        }

        function OnFiltru(s, e) {
            grDate.PerformCallback("btnFiltru");
        }

        function OnRespinge(s, e)
        {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de respingere ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        if (grDate.cpParamMotiv == "1")
                            popUpMotiv.Show();
                        else
                            grDate.PerformCallback("btnRespinge");
                    }
                });
            }
            else
            {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
        }

        function OnAproba(s, e) {
            if (grDate.GetSelectedRowCount() > 0) {
                swal({
                    title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul de aprobare ?',
                    type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        grDate.PerformCallback("btnAproba");
                    }
                });
            }
            else {
                swal({
                    title: "Atentie", text: "Nu exista linii selectate",
                    type: "warning"
                });
            }
        }

    </script>
</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />
                <span id="txt1" runat="server">Pentru chestionarul</span>
                <br /><br />
                <dx:ASPxComboBox ID="cmbQuiz" runat="server" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" Width="350"  />
                <br />
                <span id="txt2" runat="server">trimite invitatie la toti angajatii care au aceeasi/acelasi</span>
                <br /><br />
                <dx:ASPxComboBox ID="cmbTip" runat="server" AutoPostBack="false" />

				<br /><br />

            </td>
        </tr>
        <tr>
            <td>
                <dx:ASPxButton ID="btnTrimite" runat="server" Text="Trimite invitatie" OnClick="btnTrimite_Click">
                    <ClientSideEvents Click="function(s,e) { pnlLoading.Show(); e.processOnServer = true; }" />
                </dx:ASPxButton>
            </td>
        </tr>
    </table>


</asp:Content>
