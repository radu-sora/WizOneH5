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
                        <tr>
                            <td><dx:ASPxLabel id="ASPxLabel11" runat="server" Text="Alegeti" Width="150px"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxComboBox ID="cmbTip" runat="server" AutoPostBack="false">
                                    <Items>
                                        <dx:ListEditItem Text="Venitul brut la Venitul Net" Value="1" Selected="true" />
                                        <dx:ListEditItem Text="Venitul net la Venitul brut" Value="2" />
                                    </Items>
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table>
                        <tr>
                            <td>
                                <dx:ASPxRadioButton ID="rbSimplu" runat="server" AutoPostBack="false" Text="Simplu" GroupName="TipGrup" Checked="true"></dx:ASPxRadioButton>
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
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <table>
                        <tr>
                            <td><dx:ASPxLabel id="ASPxLabel1" runat="server" Text="Venit de calculat" Width="150px"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="ASPxSpinEdit1" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="ASPxLabel2" runat="server" Text="Venit rezultat" Width="100px"/></td>
                            <td>
                                <dx:ASPxTextBox ID="rezVenit" runat="server" ReadOnly="true" Enabled="false" Width="100px">
                                    <DisabledStyle BackColor="LightGray"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="ASPxLabel3" runat="server" Text="Procent CAS"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="ASPxSpinEdit2" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="ASPxLabel4" runat="server" Text="CAS"/></td>
                            <td>
                                <dx:ASPxTextBox ID="ASPxTextBox1" runat="server" ReadOnly="true" Enabled="false" Width="100px">
                                    <DisabledStyle BackColor="LightGray"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="ASPxLabel5" runat="server" Text="Procent CASS"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="ASPxSpinEdit3" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="ASPxLabel6" runat="server" Text="CASS"/></td>
                            <td>
                                <dx:ASPxTextBox ID="ASPxTextBox2" runat="server" ReadOnly="true" Enabled="false" Width="100px">
                                    <DisabledStyle BackColor="LightGray"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="ASPxLabel7" runat="server" Text="Procent Impozit"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxSpinEdit ID="ASPxSpinEdit4" runat="server" Width="100px" MinValue="0" MaxValue="999999" AutoPostBack="false">
                                    <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                </dx:ASPxSpinEdit>
                            </td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td><dx:ASPxLabel id="ASPxLabel8" runat="server" Text="Impozit"/></td>
                            <td>
                                <dx:ASPxTextBox ID="ASPxTextBox3" runat="server" ReadOnly="true" Enabled="false" Width="100px">
                                    <DisabledStyle BackColor="LightGray"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td><br /><dx:ASPxLabel id="ASPxLabel9" runat="server" Text="Nr. pers. in intretinere"/></td>
                            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                            <td>
                                <dx:ASPxComboBox ID="cmbPers" runat="server" AutoPostBack="false">
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
                            <td><dx:ASPxLabel id="ASPxLabel10" runat="server" Text="Deducere"/></td>
                            <td>
                                <dx:ASPxTextBox ID="ASPxTextBox4" runat="server" ReadOnly="true" Enabled="false" Width="100px">
                                    <DisabledStyle BackColor="LightGray"></DisabledStyle>
                                </dx:ASPxTextBox>
                            </td>
                        </tr>
                    </table>
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
