<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CereriDiverseTactil.aspx.cs" Inherits="WizOne.Tactil.CereriDiverseTactil" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>        
        function DelayedCallback(strCallbackName) {
            pnlCtl.PerformCallback(1);
        }

        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }
    </script>

    <table style="width:100%;">
        <tr>
            <td style="text-align:right; padding-right:20px;"><span id="spanTimeLeft"></span> seconds left</td>
        </tr>
    </table>
    <table style="width:100%;">
        <tr>
            <td align="left">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgBack.png"></Image>
                </dx:ASPxButton>
            </td>
            <td align="left"><Label runat="server" id="lblMarca" style="font-weight: bold;font-size:20px">MARCA: </Label> </td>
            <td align="center"><Label runat="server" id="lblNume" style="font-weight: bold;font-size:20px">NUME:</Label></td>
            <td align="right">
                <dx:ASPxButton ID="btnLogOut" ClientInstanceName="btnLogOut" ClientIDMode="Static"  RenderMode="Link" runat="server" ToolTip="Deconectare" AutoPostBack="true" OnClick="btnLogOut_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="../Fisiere/Imagini/bdgOut.jpg"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    
    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" meta:resourcekey="pnlCtlResource1">
        <SettingsLoadingPanel Enabled="False"></SettingsLoadingPanel>
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
        <PanelCollection>
            <dx:PanelContent meta:resourcekey="PanelContentResource1">                   
                <table style="width:100%;" >
                    <tr>
                        <td width="130"></td>
                        <td width="400" align="left" id="td1" runat="server">
                            <label id="Label1" runat="server" style="font-size:20px;">Tip cerere</label>   
                        </td>  
                         <td width="300" align="left">                    
                         </td>
                    </tr>
                    <tr>
                        <td width="130"></td>
                        <td width="400" align="left" id="tdSelAbs" runat="server">
                                <dx:ASPxComboBox ID="cmbTip" runat="server" ClientInstanceName="cmbTip" ClientIDMode="Static" Width="400" ValueField="Id" DropDownWidth="400" DropDownHeight="300"  style="font-size:20px;" Height="75" 
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" meta:resourcekey="cmbAbsResource1" ButtonStyle-Width="75"   >
                                    <ItemStyle Font-Size="XX-Large" />                           
                                </dx:ASPxComboBox>
                        </td>  
                         <td width="300" align="left">
                            <label id="lblDesc" runat="server" style="font-size:20px;">Descriere</label>
                            <dx:ASPxMemo ID="txtDesc" runat="server" Width="400px" style="font-size:20px;" Height="100px" meta:resourcekey="txtObsResource1"></dx:ASPxMemo>
                         </td>
                    </tr>
                </table>

                <div class="row text-center align-center">

                    <div class="col-sm-4">
                        <div class="badgeTactil">
                            <asp:LinkButton runat="server" ID="lnkSave" OnClick="lnkSave_Click"  OnClientClick="function (s,e) { pnlLoading.Show(); }">                                                
                                <div>
                                    <img src ="../Fisiere/Imagini/bdgDec.jpg" alt = "Trimite la aprobare" />
                                </div>
                            </asp:LinkButton>
                            <h3>Trimite la aprobare</h3>
                        </div>
                    </div>
                </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
    
</asp:Content>
