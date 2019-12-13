<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CalculVenit.aspx.cs" Inherits="WizOne.Pagini.CalculVenit" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">

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

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" ClientSideEvents-EndCallback="OnEndCallback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>
		
                 <tr align="left">
                         <td  valign="top">
                               <fieldset  class="fieldset-auto-width">  
                               <legend class="legend-font-size">Date initiale</legend>
                                <table width="10%" >
					                <tr>				
						                <td >
							                <dx:ASPxLabel  ID="lblVenitBrut" Width="100" runat="server"  Text="Venitul Brut" ></dx:ASPxLabel>	
						                </td>	
						                <td>
							                <dx:ASPxTextBox  ID="txtVenitBrut" ClientInstanceName="txtVenitBrut"  Width="100" runat="server" ReadOnly="true" AutoPostBack="false" ></dx:ASPxTextBox >
						                </td>
					                </tr>
					                <tr>				
						                <td >
							                <dx:ASPxLabel  ID="lblVenitNet" Width="100" runat="server"  Text="Venitul Net" ></dx:ASPxLabel >	
						                </td>	
						                <td>
							                <dx:ASPxTextBox  ID="txtVenitNet" ClientInstanceName="txtVenitNet" Width="100" runat="server" ReadOnly="true"  AutoPostBack="false" ></dx:ASPxTextBox >
						                </td>
					                </tr>  
                                    <tr>
                                        <td>
                                            <dx:ASPxLabel id="lblAng" runat="server" Text="Angajat"></dx:ASPxLabel>
                                        </td>
                                        <td>
                                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" >
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
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
                              </fieldset >
                                            
                         </td>   
                         <td  valign="top">     
                             <fieldset class="fieldset-auto-width">
                                 <legend class="legend-font-size">Date rezultante</legend>
                                <table width="10%" >    
                                    <tr>
                                        <td align="center">
                 
                                        </td>
                                    </tr>
  
                                </table>
                              </fieldset >
                        </td> 
                  </tr>      
	
                </dx:PanelContent>
            </PanelCollection>
        </dx:ASPxCallbackPanel>

    </body>

</asp:Content>