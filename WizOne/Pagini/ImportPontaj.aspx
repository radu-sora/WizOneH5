<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" Async="true"  CodeBehind="ImportPontaj.aspx.cs" Inherits="WizOne.Pagini.ImportPontaj" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    var limba = "<%= Session["IdLimba"] %>";

    function OnClick(s) {   
        swal({
            title: 'Sunteti sigur/a ?', text: 'Vreti sa continuati procesul?',
            type: 'warning', showCancelButton: true, confirmButtonColor: '#DD6B55', confirmButtonText: 'Da, continua!', cancelButtonText: 'Renunta', closeOnConfirm: true
        }, function (isConfirm) {
            if (isConfirm) {
               
                pnlCtl.PerformCallback(s.name);
            }
        });
    }

    function OnEndCallback(s, e) {
        pnlLoading.Hide();
        if (s.cpAlertMessage != null) {
            swal({
                title: trad_string(limba, "Atentie !"), text: s.cpAlertMessage,
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
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
           <ClientSideEvents EndCallback="function(s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
          <PanelCollection>
            <dx:PanelContent>

			<div>
                <tr >
                    <td>
                        <dx:ASPxLabel  ID="lblStart" runat="server"  style="display:inline-block;"  Text="Data start"></dx:ASPxLabel >
						<dx:ASPxDateEdit  ID="deDataStart" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"   AutoPostBack="false"  >
                            <CalendarProperties FirstDayOfWeek="Monday" />
						</dx:ASPxDateEdit>
                    </td>
                    <td>
                        <dx:ASPxLabel  ID="lblSfarsit" runat="server"  style="display:inline-block;"  Text="Data sfarsit"></dx:ASPxLabel >
						<dx:ASPxDateEdit  ID="deDataSfarsit" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"   AutoPostBack="false"  >
                            <CalendarProperties FirstDayOfWeek="Monday" />
						</dx:ASPxDateEdit>
                    </td>
                </tr>
                <tr>
                 <td  valign="top">
                   <fieldset  >
                    <legend class="legend-font-size">Import pontaj</legend>
                    <table width="10%" >
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Import" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)">
                                    <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
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