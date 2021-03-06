<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajUpdate.aspx.cs" Inherits="WizOne.Pontaj.PontajUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function OnClickAct(s, e) {
            var time = <%= Session["Ptj_DataBlocare"] %>;
            var data = txtDataInc.GetValue();
            var dtBlocare = new Date(Number(time.toString().substring(0, 4)), Number(time.toString().substring(4, 6)) - 1, Number(time.toString().substring(6)));
            var dtInc = new Date(data.getFullYear(), data.getMonth(), data.getDate());
            e.processOnServer = false;            
            if (dtInc <= dtBlocare) {
                swal({
                    title: "Atentie", text: "Pontajul este blocat pana la data de " + dtBlocare.getDate() + "/" + (dtBlocare.getMonth() + 1) + "/" + dtBlocare.getFullYear() + "! \n Doriti sa continuati?",
                    type: "info", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Nu", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        pnlCtl.PerformCallback('btnAct');
                    }
                });
            }
            else {
                pnlLoading.Show();
                e.processOnServer = true;
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
            pnlLoading.Hide();
        }

        function DataInceput(s, e) {
            var dtInc = new Date(s.GetDate());
            pnlCtl.PerformCallback('txtDataInc;' + dtInc.getDate() + '/' + (dtInc.getMonth() + 1) + '/' + dtInc.getFullYear()); 
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
        <ClientSideEvents  EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnValStr" ClientInstanceName="btnValStr" ClientIDMode="Static" runat="server" Text="Sintaxa ValStr" AutoPostBack="true" OnClick="btnValStr_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/m5.png" Width="16"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAct" ClientInstanceName="btnAct" ClientIDMode="Static" runat="server" Text="Actualizeaza" AutoPostBack="false" OnClick="btnAct_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) {                        
                        OnClickAct(s, e);                     
                     }" />
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />
				
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup" id="divRol" runat="server">
                        <label id="lblCtrInc" runat="server" style="display:inline-block;">Data Inceput</label>
                        <dx:ASPxDateEdit ID="txtDataInc" ClientInstanceName="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                            <ClientSideEvents DateChanged="function (s,e) { DataInceput (s, e)}" />
                        </dx:ASPxDateEdit>
                    </div>
                    <div class="Absente_Cereri_CampuriSup" id="div1" runat="server">
                        <label id="lblCtrSf" runat="server" style="display:inline-block;">Data Sfarsit</label>
                        <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Date" >
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </div>

                </div>
                <br /><br />
                <div class="Absente_divOuter">
                    
                    <div class="Absente_Cereri_CampuriSup" id="div2" runat="server">
                        <label id="lblMarcaInc" runat="server" style="display:inline-block;">Marca Inceput</label>
                        <dx:ASPxSpinEdit ID="txtMarcaInc" runat="server" Width="100px" />
                    </div>

                    <div class="Absente_Cereri_CampuriSup" id="div3" runat="server">
                        <label id="lblMarcaSf" runat="server" style="display:inline-block;">Marca Sfarsit</label>
                        <dx:ASPxSpinEdit ID="txtMarcaSf" runat="server" Width="100px" />
                    </div>

                </div>
                <br /><br />
                <table>
                    <tr>
                        <td><dx:ASPxCheckBox ID="chkCtr" runat="server" Text="Contract" TextAlign="Right" /></td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkStr" runat="server" Text="Structura Org." TextAlign="Right" /></td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkNrm" runat="server" Text="Norma" TextAlign="Right" /></td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkPerAng" runat="server" Text="Perioada angajare" TextAlign="Right" /></td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkRecalc" runat="server" Text="Recalcul totaluri" TextAlign="Right" /></td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkCC" runat="server" Text="Centrul de cost" TextAlign="Right" /></td>  
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td><dx:ASPxCheckBox ID="chkSL" runat="server" Text="Sarbatori legale" TextAlign="Right" /></td>   
                        
                    </tr>
                </table>

            </td>
        </tr>
    </table>

</asp:Content>
