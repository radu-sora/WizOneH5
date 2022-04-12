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

         function Goleste() {
             cmbAng.SetValue(null);
             cmbPers.SetValue(0);
             chkScutit.SetValue(0);
             txtTipAng.SetValue(null);
             txtTicheteNr.SetValue(0);
             txtTicheteVal.SetValue(0);
             txtTicheteTotal.SetValue(0);

             txtVenitRez.SetValue(null);
             txtCas.SetValue(null);
             txtCass.SetValue(null);
             txtImp.SetValue(null);
             txtDed.SetValue(null);
         }

         function CalcTichete()
         {
             txtTicheteTotal.SetValue(Math.round(txtTicheteNr.GetValue() * txtTicheteVal.GetValue()));
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
                    <ClientSideEvents Click="function(s,e) { pnlCtl.PerformCallback(2); }" />
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
                        <tr>
                            <td><dx:ASPxLabel id="ASPxLabel11" runat="server" Text="Alegeti" Width="150px"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxComboBox ID="cmbTip" runat="server" AutoPostBack="false" Width="250px">
                                    <Items>
                                        <dx:ListEditItem Text="Venitul brut la Venitul Net" Value="1" Selected="true" />
                                        <dx:ListEditItem Text="Venitul net la Venitul brut" Value="2" />
                                    </Items>
                                    <ClientSideEvents ValueChanged="function(s,e) { Goleste(); }" />
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table>
                        <tr>
                            <td>
                                <dx:ASPxRadioButton ID="rbSimplu" runat="server" AutoPostBack="false" Text="Simplu" GroupName="TipGrup" Checked="true">
                                    <ClientSideEvents CheckedChanged="function(s,e) { Goleste(); }" />
                                </dx:ASPxRadioButton>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxRadioButton ID="rbAng" runat="server" AutoPostBack="false" Text="Angajat" GroupName="TipGrup"></dx:ASPxRadioButton>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="Marca" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false">
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="Marca" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px"  />
                                        <dx:ListBoxColumn FieldName="Subcompanie" Caption="Subcompanie" Width="130px"  />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px"  />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px"  />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px"  />
                                    </Columns>
                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback(1); }" />
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table>
                        <tr>
                            <td><dx:ASPxLabel id="lblVenitCal" runat="server" Text="Venit de calculat" Width="150px"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtVenitCal" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons>
                                    <ClientSideEvents KeyDown="function(s,e) { Goleste(); }" />
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="lblVenitRez" runat="server" Text="Venit rezultat" Width="100px"/></td>
                            <td>
                                <dx:ASPxTextBox ID="txtVenitRez" runat="server" ReadOnly="true" ClientEnabled="false" Width="100px" ClientInstanceName="txtVenitRez">
                                    <DisabledStyle BackColor="LightGray" ForeColor="Black"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblCasPro" runat="server" Text="Procent CAS"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtCasPro" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="lblCas" runat="server" Text="CAS"/></td>
                            <td>
                                <dx:ASPxTextBox ID="txtCas" runat="server" ReadOnly="true" ClientEnabled="false" Width="100px" ClientInstanceName="txtCas">
                                    <DisabledStyle BackColor="LightGray" ForeColor="Black"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblCassPro" runat="server" Text="Procent CASS"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtCassPro" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="lblCass" runat="server" Text="CASS"/></td>
                            <td>
                                <dx:ASPxTextBox ID="txtCass" runat="server" ReadOnly="true" ClientEnabled="false" Width="100px" ClientInstanceName="txtCass">
                                    <DisabledStyle BackColor="LightGray" ForeColor="Black"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblImpProc" runat="server" Text="Procent Impozit"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtImpPro" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="lblImp" runat="server" Text="Impozit"/></td>
                            <td>
                                <dx:ASPxTextBox ID="txtImp" runat="server" ReadOnly="true" ClientEnabled="false" Width="100px" ClientInstanceName="txtImp">
                                    <DisabledStyle BackColor="LightGray" ForeColor="Black"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblPer" runat="server" Text="Nr. pers. in intretinere"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxComboBox ID="cmbPers" ClientInstanceName="cmbPers"  ClientIDMode="Static" runat="server" AutoPostBack="false">
                                    <Items>
                                        <dx:ListEditItem Value="0" Text="Nici o persoana" Selected="true" />
                                        <dx:ListEditItem Value="1" Text="1 persoana" />
                                        <dx:ListEditItem Value="2" Text="2 persoane" />
                                        <dx:ListEditItem Value="3" Text="3 persoane" />
                                        <dx:ListEditItem Value="4" Text="4 persoane sau mai multe" />
                                    </Items>
                                </dx:ASPxComboBox>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="lblDed" runat="server" Text="Deducere"/></td>
                            <td>
                                <dx:ASPxTextBox ID="txtDed" runat="server" ReadOnly="true" ClientEnabled="false" Width="100px" ClientInstanceName="txtDed">
                                    <DisabledStyle BackColor="LightGray" ForeColor="Black"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblSalMediu" runat="server" Text="Salariul Mediu"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtSalMediu" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblTipAng" runat="server" Text="Tip Angajat"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtTipAng" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblScutit" runat="server" Text="Scutit"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxCheckBox ID="chkScutit" ClientInstanceName="chkScutit" runat="server" Text="" AutoPostBack="false" />
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblTicheteNr" runat="server" Text="Nr. tichete de masa"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtTicheteNr" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false" ClientInstanceName="txtTicheteNr" Number="0" NumberType="Integer">
                                    <SpinButtons ShowIncrementButtons="true"></SpinButtons>
                                    <ClientSideEvents ValueChanged="function(s,e) { CalcTichete(); }" />
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblTicheteVal" runat="server" Text="Valoare tichet"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtTicheteVal" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false" ClientInstanceName="txtTicheteVal" Number="0" NumberType="Float" DecimalPlaces="2">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons>
                                    <ClientSideEvents ValueChanged="function(s,e) { CalcTichete(); }" />
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="lblTicheteTotal" runat="server" Text="Valoare totala tichete de masa"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtTicheteTotal" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false" ClientInstanceName="txtTicheteTotal" Number="0" NumberType="Integer">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td></td>
                            <td></td>
                        </tr>

                    </table>
                </div>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

</asp:Content>
