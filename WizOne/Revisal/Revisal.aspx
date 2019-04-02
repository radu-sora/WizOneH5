<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Revisal.aspx.cs" Inherits="WizOne.Revisal.Revisal" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function OnTextChangedHandler(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
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
	<style type="text/css">
        .legend-border
        {
             border: 0;
        }
	</style>
    <body>
        <table width="100%">
                <tr>
                    <td align="right">
                        <dx:ASPxButton ID="btnSalvare" ClientInstanceName="btnSalvare" ClientIDMode="Static" runat="server" Text="Salvare parametri" OnClick="btnSalvare_Click" oncontextMenu="ctx(this,event)">                         
                            <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                              <ClientSideEvents Click="function(s, e) {
                                                pnlLoading.Show();
                                                e.processOnServer = true;
                                            }" />
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnPreg" ClientInstanceName="btnPreg" ClientIDMode="Static" runat="server" Text="Pregatire date" OnClick="btnPreg_Click" oncontextMenu="ctx(this,event)">                         
                            <Image Url="~/Fisiere/Imagini/Icoane/intre.png"></Image>
                              <ClientSideEvents Click="function(s, e) {
                                                pnlLoading.Show();
                                                e.processOnServer = true;
                                            }" />
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" Text="Genereaza XML" OnClick="btnGen_Click" oncontextMenu="ctx(this,event)">                         
                            <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                              <ClientSideEvents Click="function(s, e) {                                              
                                                e.processOnServer = true;
                                            }" />
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
          <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }"  />
          <PanelCollection>
            <dx:PanelContent>
			<div>
                <tr align="left">
                 <td style="display:none" valign="top">
                   <fieldset >                     
                    <legend class="legend-font-size">Configurare angajati inactivi</legend> 
                    <table width="10%" >
                        <tr>
                            <td align="left">                             
                                    <dx:ASPxCheckBox ID="chkInactivi"  runat="server" Width="120"  Text="Inclusiv inactivi" TextAlign="Left" ClientInstanceName="chkInactivi" >
                                    </dx:ASPxCheckBox>   
                             </td>                    
						    <td >
							    <dx:ASPxLabel  ID="lblInactivi" Width="100" runat="server"  Text="Inactivi din data: " ></dx:ASPxLabel >	
							    <dx:ASPxDateEdit  ID="deDataInactivi" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxDateEdit>	
						    </td>						
                        </tr>         
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Configurare perioada</legend> 
                    <table width="10%" >
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblDeLa" runat="server" Text="De la" >                                
                                </dx:ASPxLabel>
							    <dx:ASPxDateEdit  ID="deDeLa" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							    </dx:ASPxDateEdit>	
                            </td>
                            <td>
                                <dx:ASPxLabel ID="lblLa" runat="server" Text="La" >                                
                                </dx:ASPxLabel> 
						        <dx:ASPxDateEdit  ID="deLa" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							     </dx:ASPxDateEdit>
                            </td>
                        </tr>
                        <tr>
	                        <td align="left">                             
                                <dx:ASPxCheckBox ID="chkCalc"  runat="server" Width="100"  Text="Calculul e final" TextAlign="Left" ClientInstanceName="chkCalc" >
                                </dx:ASPxCheckBox>   
                             </td>  	
                            <td align="left">
                                <dx:ASPxLabel ID="lblDataSpor" Width="180" runat="server" Text="Data modif. sporului" >                                
                                </dx:ASPxLabel> 
						        <dx:ASPxDateEdit  ID="deDataSpor" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandler(s); }" />
							     </dx:ASPxDateEdit>
                            </td>
                        </tr>
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Configurare Data sf. contract per. determ./Data incetare</legend> 
                    <table width="10%" >
                        <tr>
                            <td>                              
                                <dx:ASPxRadioButton ID="rbDataSf1" Width="300" runat="server" RepeatDirection="Vertical"  Text="Data sfarsit contract = Data incetare" Enabled="true"  ClientInstanceName="rbDataSf1"
                                     GroupName="DataSfarsit"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbDataSf2"  Width="300" runat="server" RepeatDirection="Vertical"  Text="Data sfarsit contract = Data incetare - 1 zi" Enabled="true" ClientInstanceName="rbDataSf2" 
                                     GroupName="DataSfarsit">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>	
                        </tr>                   
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Configurare Sporuri</legend> 
                    <table width="10%" >
                        <tr>
                            <td>                              
                                <dx:ASPxRadioButton ID="rbSporuri1" Width="300" runat="server" RepeatDirection="Vertical"  Text="Sporuri calculate" Enabled="true"  ClientInstanceName="rbSporuri1"
                                     GroupName="SporuriCalcCuv"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbSporuri2"  Width="300" runat="server" RepeatDirection="Vertical"  Text="Sporuri cuvenite" Enabled="true" ClientInstanceName="rbSporuri2" 
                                     GroupName="SporuriCalcCuv">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>	
                        </tr>
                        <tr>
                            <dx:ASPxLabel ID="lblSpor" runat="server" Text="Angajati noi" >  </dx:ASPxLabel>
                        </tr>
                       <tr>
                            <td>                              
                                <dx:ASPxRadioButton ID="rbSporuriAngNoi1" Width="300" runat="server" RepeatDirection="Vertical"  Text="Sporuri cuvenite" Enabled="true"  ClientInstanceName="rbSporuriAngNoi1"
                                     GroupName="SporuriCalcCuvAng"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbSporuriAngNoi2"  Width="300" runat="server" RepeatDirection="Vertical"  Text="Spor nedeclarat 0" Enabled="true" ClientInstanceName="rbSporuriAngNoi2" 
                                     GroupName="SporuriCalcCuvAng">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>	
                        </tr>                                                         
                    </table>
                  </fieldset >
                   <fieldset >                     
                    <legend class="legend-font-size">Configurare Modificari in avans</legend> 
                    <table width="10%" >
                        <tr>
                            <td>                              
                                <dx:ASPxRadioButton ID="rbModif1" Width="300" runat="server" RepeatDirection="Vertical"  Text="Modificarile in intervalul [DeLa ; La]" Enabled="true"  ClientInstanceName="rbModif1"
                                     GroupName="ModifAvans"> 
                                    <ClientSideEvents   CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                    
                                </dx:ASPxRadioButton>
                                <dx:ASPxRadioButton ID="rbModif2"  Width="300" runat="server" RepeatDirection="Vertical"  Text="Modificarile in intervalul [DeLa ; DeLa + 19 zile]" Enabled="true" ClientInstanceName="rbModif2" 
                                     GroupName="ModifAvans">
                                    <ClientSideEvents  CheckedChanged="function(s,e){ OnValueChangedHandler(s); }" />                                  
                                </dx:ASPxRadioButton>
                            </td>	
                        </tr>                   
                    </table>
                  </fieldset >
                 </td>
            </tr>      
		</div>
            </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>   

</asp:Content>