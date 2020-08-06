<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="RapoarteRevisal.aspx.cs" Inherits="WizOne.Revisal.RapoarteRevisal" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function OnEndCallback(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }

    function OnRenunta() {
        document.getElementById('grAng').style.display = 'none';
        document.getElementById('legAng').InnerText = "Lista angajati";
    }

    function OnRaport() {
        document.getElementById('grAng').style.display = 'block';
        document.getElementById('legAng').innerText = "Lista angajati - Raport pe salariat";
    }


</script>
    <body>

        <table width="100%">
                <tr>
                    <td align="right">      
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback"  ClientSideEvents-EndCallback="OnEndCallback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>

			<div>
                <tr>
                 <td  valign="top">
                   <fieldset  >
                    <legend class="legend-font-size">Lista Rapoarte Revisal</legend>
                    <table width="16%" >
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRegSal" ClientInstanceName="btnRegSal" ClientIDMode="Static" runat="server" Text="Registru salariati" Width="180"  OnClick="btnRegSal_Click" oncontextMenu="ctx(this,event)">                        
                                   
                                </dx:ASPxButton>
                            </td>
                        </tr>
                        <tr>
                             <td>&nbsp;</td>
                        </tr> 
                        <tr>
                             <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnRapSal" ClientInstanceName="btnRapSal" ClientIDMode="Static" runat="server" Text="Raport pe salariat" Width="180"  AutoPostBack="false" oncontextMenu="ctx(this,event)">                                
                                   <ClientSideEvents Click="function(s,e){ OnRaport(); }"    />
                                </dx:ASPxButton>
                            </td>
                        </tr>                         
                    </table>
                  </fieldset >
                   <fieldset id="grAng" runat="server" border="0" style="display:none">                     
                    <legend id="legAng" runat="server" class="legend-border">Lista angajati</legend>            
                    <table width="10%" >
                        <tr>
                            <td align="center">
							    <dx:ASPxComboBox   ID="cmbAng"  Width="350" runat="server" DropDownStyle="DropDown" TabIndex="9"  TextField="Salariat" ValueField="CNP" ValueType="System.String"/>  
                            </td>
                        </tr>
                        <tr>
                             <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnCont" ClientInstanceName="btnCont" ClientIDMode="Static" runat="server" Text="Continua" Width="180"  OnClick="btnCont_Click" oncontextMenu="ctx(this,event)" >                              
                                   
                                </dx:ASPxButton>
                            </td>
                        </tr>
                        <tr>
                             <td>&nbsp;</td>
                        </tr>
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnRen" ClientInstanceName="btnRen" ClientIDMode="Static" runat="server" Text="Renunta" Width="180"  AutoPostBack="false" oncontextMenu="ctx(this,event)" >                                
                                    <ClientSideEvents Click="function(s,e){ OnRenunta(); }"    />
                                </dx:ASPxButton>
                            </td>
                        </tr>
  
                    </table>
                  </fieldset>
                </td> 
            </tr>      
		</div>
            </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

</asp:Content>