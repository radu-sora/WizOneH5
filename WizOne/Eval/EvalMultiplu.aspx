<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Cadru.Master" CodeBehind="EvalMultiplu.aspx.cs" Inherits="WizOne.Eval.EvalMultiplu" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table style="width:100%">
        <tr>
            <td class="pull-left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td class="pull-right">    
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this, event)" Theme="MaterialCompactOrangeBase" >
                </dx:ASPxButton>
            </td>
        </tr>
    </table>

    <label id="lblAng" runat="server" style="float:left; margin-right:25px;">Va rugam sa selectati angajatul</label>
    <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" TextFormatString="{0} {1}" Theme="MaterialCompactOrangeBase">
        <Columns>
            <dx:ListBoxColumn FieldName="Id" Caption="Marca" Width="80px"/>
            <dx:ListBoxColumn FieldName="Denumire" Caption="Angajat" Width="170px"/>
        </Columns>
        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnSelectedIndexChanged(s,e); }"/>
    </dx:ASPxComboBox>

    <br />
    <br />

    <div class="flex-container">
        <dx:ASPxCallbackPanel ID="pnlSec" ClientIDMode="Static" ClientInstanceName="pnlSec" runat="server" SettingsLoadingPanel-Enabled="false" OnCallback="pnlSec_Callback">
            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" CallbackError="function(s,e){ alert('Eroare'); }" />
            <PanelCollection>
                <dx:PanelContent>
                    <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divSec">
                        <div class="panel-heading">Sectiune</div>
                        <div class="panel-body">
                            <dx:ASPxRadioButtonList ID="ctlSec" ClientInstanceName="ctlSec" ClientIDMode="Static" runat="server" ValueField="Sectiune" TextField="Sectiune" RepeatColumns="1" RepeatLayout="Table" Border-BorderWidth="0" AutoPostBack="false" Theme="MaterialCompactOrangeBase">
                                <ClientSideEvents ValueChanged="function(s,e) { OnSelectedIndexChanged(s,e); }" />
                            </dx:ASPxRadioButtonList>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <dx:ASPxCallbackPanel ID="pnlSub" ClientIDMode="Static" ClientInstanceName="pnlSub" runat="server" SettingsLoadingPanel-Enabled="false" OnCallback="pnlSub_Callback">
            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" />
            <PanelCollection>
                <dx:PanelContent>
                    <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divSub">
                        <div class="panel-heading">Subsectiune</div>
                        <div class="panel-body">
                            <dx:ASPxRadioButtonList ID="ctlSub" ClientInstanceName="ctlSub" ClientIDMode="Static" runat="server" ValueField="Subsectiune" TextField="Subsectiune" RepeatColumns="1" RepeatLayout="Table" Border-BorderWidth="0" AutoPostBack="false" Theme="MaterialCompactOrangeBase">
                                <ClientSideEvents ValueChanged="function(s,e) { OnSelectedIndexChanged(s,e); }" />
                            </dx:ASPxRadioButtonList>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <dx:ASPxCallbackPanel ID="pnlCtg" ClientIDMode="Static" ClientInstanceName="pnlCtg" runat="server" SettingsLoadingPanel-Enabled="false" OnCallback="pnlCtg_Callback">
            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" />
            <PanelCollection>
                <dx:PanelContent>
                    <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divCtg">
                        <div class="panel-heading">Categorie Competente</div>
                        <div class="panel-body">
                            <dx:ASPxRadioButtonList ID="ctlCtg" ClientInstanceName="ctlCtg" ClientIDMode="Static" runat="server" ValueField="IdCategorie" TextField="DenCategorie" RepeatColumns="1" RepeatLayout="Table" Border-BorderWidth="0" AutoPostBack="false" Theme="MaterialCompactOrangeBase">
                                <ClientSideEvents ValueChanged="function(s,e) { OnSelectedIndexChanged(s,e); }" />
                            </dx:ASPxRadioButtonList>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <dx:ASPxCallbackPanel ID="pnlCom" ClientIDMode="Static" ClientInstanceName="pnlCom" runat="server" SettingsLoadingPanel-Enabled="false" OnCallback="pnlCom_Callback">
            <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" />
            <PanelCollection>
                <dx:PanelContent>
                    <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divCom">
                        <div class="panel-heading">Competente</div>
                        <div class="panel-body">
                            <dx:ASPxRadioButtonList ID="ctlCom" ClientInstanceName="ctlCom" ClientIDMode="Static" runat="server" ValueField="IdCompetenta" TextField="DenCompetenta" RepeatColumns="1" RepeatLayout="Table" Border-BorderWidth="0" AutoPostBack="false" Theme="MaterialCompactOrangeBase">
                                <ClientSideEvents ValueChanged="function(s,e) { SetCtl(ctlCal); }" />
                            </dx:ASPxRadioButtonList>
                        </div>
                    </div>

                    <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divCal">
                        <div class="panel-heading">Calificativ</div>
                        <div class="panel-body">
                            <dx:ASPxRadioButtonList ID="ctlCal" ClientInstanceName="ctlCal" ClientIDMode="Static" runat="server" ValueField="Nota" TextField="Valoare" RepeatColumns="1" RepeatLayout="Table" Border-BorderWidth="0" AutoPostBack="false" Theme="MaterialCompactOrangeBase">
                                <ClientSideEvents ValueChanged="function(s,e) {  SetCtl(ctlObs); }" />
                            </dx:ASPxRadioButtonList>
                        </div>
                    </div>
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

        <div class="panel panel-default ascuns pnlUmbra margin_right50" id="divObs">
            <div class="panel-heading">Observatii</div>
            <div class="panel-body">
                <dx:ASPxMemo ID="ctlObs" ClientInstanceName="ctlObs" ClientIDMode="Static" runat="server" Width="500px" Height="100px" Theme="MaterialCompactOrangeBase"/>
            </div>
        </div>

        <div class="ascuns" id="divSave">
            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" Theme="MaterialCompactOrangeBase">
                <ClientSideEvents Click="function(s,e) { OnSaveClick(s,e); }" />
            </dx:ASPxButton>
        </div>
    </div>


    <dx:ASPxCallback ID="pmlCallback" ClientInstanceName="pmlCallback" ClientIDMode="Static" runat="server" OnCallback="pmlCallback_Callback">
        <ClientSideEvents  EndCallback="function(s,e) { OnAngEndCallback(s,e); }" />
    </dx:ASPxCallback>

    <script>
        function OnSaveClick(s, e) {
            var vals = JSON.stringify({
                ang: cmbAng.GetValue(),
                sec: ctlSec.GetValue(),
                sub: ctlSub.GetValue(),
                ctg: ctlCtg.GetValue(),
                com: ctlCom.GetValue(),
                cal: ctlCal.GetValue(),
                obs: ctlObs.GetValue(),
                ctgDen: (ctlCtg.GetSelectedItem() != null ? ctlCtg.GetSelectedItem().text : ""),
                comDen: (ctlCom.GetSelectedItem() != null ? ctlCom.GetSelectedItem().text : ""),
                calDen: (ctlCal.GetSelectedItem() != null ? ctlCal.GetSelectedItem().text : "")
            })

            //ascundem si stergem tot
            var arr = ["Sec", "Sub", "Ctg", "Com", "Cal", "Obs", "Save"];
            for (var i = 0; i < arr.length; i++) {
                var ctl = ASPxClientControl.GetControlCollection().Get("ctl" + arr[i]);
                if (ctl != null)
                    ctl.SetValue(null);
                $("#div" + arr[i]).animate({ opacity: "hide", left: "-85" }, "fast");
            }

            cmbAng.SetValue(null);
            pmlCallback.PerformCallback(vals);
        }

        function OnSelectedIndexChanged(s, e) {
            switch (s.name) {
                case "cmbAng":
                    pnlSec.PerformCallback();
                    break;
                case "ctlSec":
                    pnlSub.PerformCallback(ctlSec.GetValue());
                    break;
                case "ctlSub":
                    pnlCtg.PerformCallback(ctlSec.GetValue() + ';' + ctlSub.GetValue());
                    break;
                case "ctlCtg":
                    pnlCom.PerformCallback(ctlSec.GetValue() + ';' + ctlSub.GetValue() + ';' + ctlCtg.GetValue());
                    break;
            }
        }

        function OnEndCallback(s, e) {
            SetCtl(s);
        }

        function SetCtl(s) {
            var nume = s.name.replace('pnl', '').replace('ctl', '');
            var arr = ["Sec", "Sub", "Ctg", "Com", "Cal", "Obs", "Save"];
            var idx = arr.indexOf(nume);

            for (var i = idx; i < arr.length; i++) {
                if (idx == i || idx == 5)
                    $("#div" + arr[i]).animate({ opacity: "show", left: "-75", display: "inline-block" }, "slow");
                else
                    $("#div" + arr[i]).animate({ opacity: "hide", left: "-85" }, "fast");

                var ctl = ASPxClientControl.GetControlCollection().Get("ctl" + arr[i]);
                if (ctl != null)
                    ctl.SetValue(null);
            }
        }


        function OnAngEndCallback(s) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !",
                    text: s.cpAlertMessage,
                    type: "warning"
                });
                delete s.cpAlertMessage;
            }
        }
    </script>
</asp:Content>