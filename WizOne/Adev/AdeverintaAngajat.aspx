<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="AdeverintaAngajat.aspx.cs" Inherits="WizOne.Adev.AdeverintaAngajat" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">
        function OnValueChangedHandler(s) {
            
            pnlCtl.PerformCallback();
        }
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
                        <dx:ASPxButton ID="btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" Text="Genereaza" OnClick="btnGen_Click" oncontextMenu="ctx(this,event)">                         
                            <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>

                    </td>
        </table>

       <dx:ASPxCallbackPanel ID = "pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
          <PanelCollection>
            <dx:PanelContent>
			<div>
                <tr align="left">
                 <td   valign="top">
                   <fieldset  >
                    <legend class="legend-font-size">Adeverinte</legend>         
                    <table width="10%" >  
                        <tr>
                            <td>
                                <label id="lblAdev" runat="server" style="display:inline-block;">Tip adeverinta</label>
                                <dx:ASPxComboBox ID="cmbAdev" runat="server" ClientInstanceName="cmbAdev" ClientIDMode="Static" Width="175px" ValueField="Id" DropDownWidth="175" 
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >  
                                    <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />     
                                </dx:ASPxComboBox>
                            </td>
                        </tr>  
                        <tr>
                            <td>
                                <label id="lblAng" runat="server" style="display:inline-block;">Angajat</label>
                                <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="175px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                        CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" >                                                                    
                                    <Columns>
                                        <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                        <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                        <dx:ListBoxColumn FieldName="Functia" Caption="Functia" Width="130px" />
                                    </Columns>                            
                                </dx:ASPxComboBox>
                            </td>
                        </tr>  
                        <tr>
                            <td>
                                <label id="lblAnul" runat="server" Visible="false" style="display:inline-block;">Anul</label>
                                <dx:ASPxComboBox ID="cmbAnul" runat="server" ClientInstanceName="cmbAnul" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100"  Visible="false"
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >     
                                </dx:ASPxComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label id="lblLuna" runat="server" Visible="false" style="display:inline-block;">Luna</label>
                                <dx:ASPxComboBox ID="cmbLuna" runat="server" ClientInstanceName="cmbLuna" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100"  Visible="false"
                                    TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >     
                                </dx:ASPxComboBox>
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