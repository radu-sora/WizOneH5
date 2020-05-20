<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultTactilExtra.aspx.cs" Inherits="WizOne.DefaultTactilExtra" culture="auto" meta:resourcekey="PageResource1" uiculture="auto" %>

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
    <form id="form1" runat="server">
        <script type="text/javascript">           

            //document.getElementById("txtPan1").focus();
            function RetinePIN(nr) {  
                var value = hfPIN.Get("PIN");  
                if (value == null)
                    hfPIN.Set('PIN', String(nr));
                else
                    hfPIN.Set('PIN', String(value) + String(nr));
            }
        </script>

        <div id="panouExt" runat="server"  class="outer">
            <div  class="inner">
                Pentru accesarea aplicatiei va rugam apropiati cardul de cititor
            </div>
		</div>
        <div id="butoane" runat="server" class="innerbuttonscard">
            <table>
                <tr>
                    <td colspan="3">
                         <input type = "text" id="txtPan1" name="txtPan1" runat="server"  autocomplete="off" class="hide" maxlength="15" onserverchange="txtPan1_TextChanged" onblur="this.focus()"  />
                    </td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxButton ID="btn7" ClientInstanceName="btn7" ClientIDMode="Static" ClientVisible="false"  TabIndex="7" runat="server" Height="30px" Text="7" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn7" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(7); }" />      
                                <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
            
                    <td>
                        <dx:ASPxButton ID="btn8" ClientInstanceName="btn8" ClientIDMode="Static" ClientVisible="false"  TabIndex="8" runat="server" Height="30px" Text="8" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn8" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(8); }" />      
                                <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
          
                    <td>
                        <dx:ASPxButton ID="btn9" ClientInstanceName="btn9" ClientIDMode="Static" ClientVisible="false"  TabIndex="9" runat="server" Height="30px" Text="9" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn9" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(9); }" />           
                                <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                </tr>
      
                <tr>
                    <td>
                        <dx:ASPxButton ID="btn4" ClientInstanceName="btn4" ClientIDMode="Static" ClientVisible="false"  TabIndex="4" runat="server" Height="30px" Text="4" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn4" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(4); }" />     
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btn5" ClientInstanceName="btn5" ClientIDMode="Static" ClientVisible="false"  TabIndex="5" runat="server" Height="30px" Text="5" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn5" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(5); }" />        
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btn6" ClientInstanceName="btn6" ClientIDMode="Static" ClientVisible="false"  TabIndex="6" runat="server" Height="30px" Text="6" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn6" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(6); }" />    
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                </tr>
           
                <tr>
                    <td>
                        <dx:ASPxButton ID="btn1" ClientInstanceName="btn1" ClientIDMode="Static" ClientVisible="false"  TabIndex="11" runat="server" Height="30px" Text="1" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn1" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(1); }" />    
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btn2" ClientInstanceName="btn2" ClientIDMode="Static" ClientVisible="false"  TabIndex="12" runat="server" Height="30px" Text="2" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn2" >
                                <ClientSideEvents Click="function(s, e) { RetinePIN(2); }" />      
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btn3" ClientInstanceName="btn3" ClientIDMode="Static" ClientVisible="false"  TabIndex="13" runat="server" Height="30px" Text="3"  style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn3" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(3); }" />     
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                </tr>

                <tr>
                    <td>
 
                    </td>
                    <td>
                        <dx:ASPxButton ID="btn0" ClientInstanceName="btn0" ClientIDMode="Static" ClientVisible="false"  TabIndex="10" runat="server" Height="30px" Text="0" style="font-size:30px;text-align:center" AutoPostBack="false" CssClass="divider" RenderMode="Outline" oncontextMenu="ctx(this,event)" meta:resourcekey="btn0" >
                                    <ClientSideEvents Click="function(s, e) { RetinePIN(0); }" />     
                            <Paddings PaddingBottom="10px" PaddingRight="20px" />
                        </dx:ASPxButton>
                    </td>
                    <td>
      
                    </td>
                </tr>
        
                <tr>                  
                    <td colspan="3">
                        <dx:ASPxButton ID="btnLog" ClientInstanceName="btnLog" ClientIDMode="Static" ClientVisible="false"  TabIndex="10" runat="server" Width="260px" Height="30px" Text="Logare" style="font-size:30px;text-align:center" CssClass="divider" AutoPostBack="true" OnClick="btnLog_Click" oncontextMenu="ctx(this,event)"  RenderMode="Outline" meta:resourcekey="btnLog" >                                                                          
                            <Paddings PaddingBottom="10px" PaddingLeft="30px"  />
                        </dx:ASPxButton>
                    </td>                 
                </tr> 
            </table>
		</div>
   
        

    <dx:ASPxHiddenField ID="hfPIN" runat="server"></dx:ASPxHiddenField>        

    </form>
</body>
</html>
