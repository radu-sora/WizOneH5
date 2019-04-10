<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="InchidereLuna.aspx.cs" Inherits="WizOne.Pagini.InchidereLuna" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
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
          <PanelCollection>
            <dx:PanelContent>

			<div>
                <tr>
                 <td  valign="top">
               <fieldset  >
                <legend class="legend-font-size">Inchidere luna</legend>
                <table width="10%" >
                    <tr>
                        <td align="center">
                            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salvare luna" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)">
                                <ClientSideEvents Click="function(s,e){ OnClick(s); }" /> 
                                <Paddings Padding="0px" />
                            </dx:ASPxButton>
                        </td>
                    </tr>    
                </table>
              </fieldset >
               <fieldset border="0">                     
                <legend class="legend-border"></legend>            
                <table width="10%" >    
                    <tr>
                        <td align="center">
                            <dx:ASPxButton ID="btnClose" ClientInstanceName="btnClose" ClientIDMode="Static" runat="server" Text="Inchidere luna" Width="10" Height="10" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                                <ClientSideEvents Click="function (s, e) { OnClick(s, e); }" /> 
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