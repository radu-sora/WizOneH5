<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ExportSAP.aspx.cs" Inherits="WizOne.Pagini.ExportSAP" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

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



			<div>
                <tr>
                 <td  valign="top">
                   <fieldset  >
                    <legend id="lblExportSAP" runat="server" class="legend-font-size">Export SAP</legend>
                    <table width="10%" >
                        <tr>
                            <td align="center">
                                <dx:ASPxButton ID="btnExportSAP" ClientInstanceName="btnExportSAP" ClientIDMode="Static" runat="server" Text="Export SAP" Width="10" Height="10" OnClick="btnExportSAP_Click" AutoPostBack="true" oncontextMenu="ctx(this,event)">                            
                                    <Paddings Padding="0px" />
                                </dx:ASPxButton>
                            </td>
                        </tr> 
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chk1"  runat="server" Width="150" Text="CheckBox1" TextAlign="Left"  ClientInstanceName="chk1"/>                      
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chk2"  runat="server" Width="150" Text="CheckBox2" TextAlign="Left"  ClientInstanceName="chk2"/>                      
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chk3"  runat="server" Width="150" Text="CheckBox3" TextAlign="Left"  ClientInstanceName="chk3"/>                      
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chk4"  runat="server" Width="150" Text="CheckBox4" TextAlign="Left" ClientVisible="false"  ClientInstanceName="chk4"/>                      
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chk5"  runat="server" Width="150" Text="CheckBox5" TextAlign="Left" ClientVisible="false"  ClientInstanceName="chk5"/>                      
                            </td>
                        </tr>
                    </table>
                  </fieldset >
                </td> 
            </tr>      
		</div>


    </body>

</asp:Content>