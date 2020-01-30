<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CalculVenit.aspx.cs" Inherits="WizOne.Pagini.CalculVenit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script>
         function OnEndCallback(s, e) {
             if (s.cpAlertMessage != null) {
                 swal({
                     title: "", text: s.cpAlertMessage,
                     type: "warning"
                 });
                 s.cpAlertMessage = null;
             }
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
                <dx:ASPxButton ID="btnCalcul" ClientInstanceName="btnCalcul" ClientIDMode="Static" runat="server" Text="Calculeaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                    <ClientSideEvents Click="function(s,e) { pnlCtl.PerformCallback(); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    <br /><br />

    <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" />
        <PanelCollection>
            <dx:PanelContent>
                <div class="tbl_margin">
		        <table>
                    <tr align="left">
                        <td>
                            <dx:ASPxLabel id="lblAng" runat="server" Text="Angajat"/>
                        </td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <dx:ASPxLabel ID="lblVenitBrut" runat="server" Text="Venitul Brut"/>	
                        </td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <dx:ASPxLabel ID="lblVenitNet" runat="server"  Text="Venitul Net"/>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="Marca" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="Marca" Caption="Marca" Width="130px" />
                                    <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px"  />
                                    <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="130px"  />
                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px"  />
                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px"  />
                                    <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px"  />
                                </Columns>
                            </dx:ASPxComboBox>
                        </td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <br />
                            <dx:ASPxSpinEdit ID="txtVenitBrut" ClientInstanceName="txtVenitBrut" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                            </dx:ASPxSpinEdit>
                        </td>
                        <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                        <td>
                            <br />
                            <dx:ASPxSpinEdit ID="txtVenitNet" ClientInstanceName="txtVenitNet" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                            </dx:ASPxSpinEdit>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                            <br />
                            <dx:ASPxLabel id="lblRez" runat="server" Text="Rezultate calcul"/>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                            <dx:ASPxLabel id="txtRez1" runat="server" Width="100px"/>
                            <dx:ASPxLabel id="txtRez2" runat="server" Width="100px"/>
                        </td>
                    </tr>      
	            </table>
                </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

</asp:Content>
