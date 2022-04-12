<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultTactilFaraCard.aspx.cs" Inherits="WizOne.DefaultTactilFaraCard" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <title>WizOne ver 1.0</title>

    <link rel="stylesheet" type="text/css" href="Fisiere/css/login.css" />
    <link rel="stylesheet" type="text/css" href="Fisiere/css/tactil.css" />

    <link rel="stylesheet" type="text/css" href="Fisiere/MsgBox/sweetalert.css" />
    <script src="Fisiere/MsgBox/sweetalert.min.js"></script>

</head>

<body>

    <form id="form1" runat="server" class="outerbuttons">
        <script type="text/javascript">
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

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" meta:resourcekey="pnlCtlResource1" >
        <SettingsLoadingPanel Enabled="False"></SettingsLoadingPanel>
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }"  />
        <PanelCollection>
            <dx:PanelContent meta:resourcekey="PanelContentResource1">

                <div class="innerbuttons">
                    <table>
                        <tr>
                            <td>
                                <dx:ASPxButton ID="btn7" ClientInstanceName="btn7" ClientIDMode="Static" runat="server" Height="30px" Text="7" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider"  RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn7" >
                                        <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(7); }" />   
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
            
                            <td>
                                <dx:ASPxButton ID="btn8" ClientInstanceName="btn8" ClientIDMode="Static" runat="server" Height="30px" Text="8" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn8" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(8); }" />      
                                     <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
          
                            <td>
                                <dx:ASPxButton ID="btn9" ClientInstanceName="btn9" ClientIDMode="Static" runat="server" Height="30px" Text="9" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn9" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(9); }" />           
                                     <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
      
                        <tr>
                            <td>
                                <dx:ASPxButton ID="btn4" ClientInstanceName="btn4" ClientIDMode="Static" runat="server" Height="30px" Text="4" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn4" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(4); }" />     
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btn5" ClientInstanceName="btn5" ClientIDMode="Static" runat="server" Height="30px" Text="5" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn5" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(5); }" />        
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btn6" ClientInstanceName="btn6" ClientIDMode="Static" runat="server" Height="30px" Text="6" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn6" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(6); }" />    
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>
           
                        <tr>
                            <td>
                                <dx:ASPxButton ID="btn1" ClientInstanceName="btn1" ClientIDMode="Static" runat="server" Height="30px" Text="1" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn1" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(1); }" />    
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btn2" ClientInstanceName="btn2" ClientIDMode="Static" runat="server" Height="30px" Text="2" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn2" >
                                        <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(2); }" />      
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                            <td>
                                <dx:ASPxButton ID="btn3" ClientInstanceName="btn3" ClientIDMode="Static" runat="server" Height="30px" Text="3"  style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn3" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(3); }" />     
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                        </tr>

                        <tr>
                            <td>
 
                            </td>
                            <td>
                                <dx:ASPxButton ID="btn0" ClientInstanceName="btn0" ClientIDMode="Static" runat="server" Height="30px" Text="0" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn0" >
                                          <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback(0); }" />     
                                    <Paddings PaddingBottom="10px" PaddingRight="20px" />
                                </dx:ASPxButton>
                            </td>
                            <td>
      
                            </td>
                        </tr>
        
                        <tr>                  
                            <td colspan="3">
                                <dx:ASPxButton ID="btnLog" ClientInstanceName="btnLog" ClientIDMode="Static" runat="server" Width="260px" Height="30px" Text="Logare" style="font-size:30px;text-align:center" CssClass="divider" AutoPostBack="false" oncontextMenu="ctx(this,event)"  RenderMode="Outline" meta:resourcekey="btnLog" >
                                         <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback('btnLog'); }" />                                 
                                  <Paddings PaddingBottom="10px" PaddingLeft="30px"  />
                                </dx:ASPxButton>
                            </td>                 
                        </tr>
                    </table>
		        </div>
              </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>        
    </form>


</body>
</html>
