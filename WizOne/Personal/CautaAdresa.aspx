<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CautaAdresa.aspx.cs" Inherits="WizOne.Personal.CautaAdresa" %>



<script language="javascript" type="text/javascript">

    window.onunload = function (e) {
        opener.CompleteazaAdresa(); 
    };

    window.onbeforeunload = function (e) {
        opener.CompleteazaAdresa();
    };


</script>
    <body>
        <form id="form1" runat="server">
			<tr>				
				<td >
					<dx:ASPxLabel  ID="lblLocalitate" runat="server"  Text="Localitate" ></dx:ASPxLabel >	

					<dx:ASPxTextBox  ID="txtLocalitate" Width="125"  runat="server" AutoPostBack="false" >                        
					</dx:ASPxTextBox>
				</td>
			</tr>
			<tr>				
				<td >
					<dx:ASPxLabel  ID="lblArtera" runat="server"  Text="Artera" ></dx:ASPxLabel >	

					<dx:ASPxTextBox  ID="txtArtera" Width="125"  runat="server" AutoPostBack="false" >                        
					</dx:ASPxTextBox>
		
                    <dx:ASPxButton ID="btnFiltru" runat="server"  OnClick="btnFiltru_Click">                                     
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>               
                    </dx:ASPxButton>
                </td>
			</tr>
            <dx:ASPxGridView ID="grDateCautaAdresa" runat="server" ClientInstanceName="grDateCautaAdresa" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  >
                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />                 
                <Columns>                                                                                                        
                    <dx:GridViewDataTextColumn FieldName="Artera" Name="Artera" Caption="Artera"  Width="125px" />
                    <dx:GridViewDataTextColumn FieldName="LocSatSect" Name="LocSatSect" Caption="Localitate"  Width="125px" />
                    <dx:GridViewDataTextColumn FieldName="MunOraCom" Name="MunOraCom" Caption="Mun/Oras/Com"  Width="125px"/>
                    <dx:GridViewDataTextColumn FieldName="Judet" Name="Judet" Caption="Judet"  Width="125px" />
                    <dx:GridViewDataTextColumn FieldName="SirutaSat" Name="SirutaSat" Caption="SirutaSat"  Width="125px"  Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="SirutaOras" Name="SirutaOras" Caption="SirutaOras"  Width="125px" Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="SirutaJudet" Name="SirutaJudet" Caption="SirutaJudet"  Width="125px"  Visible="false"/>
                    <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto"  Width="125px"  Visible="false"/>
                </Columns>
            </dx:ASPxGridView>



            <tr align="right">
                <dx:ASPxButton ID="btnOK" runat="server" Text="Salveaza" OnClick="btnOK_Click"  AutoPostBack="true" >                                     
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>               
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" >
                    <ClientSideEvents Click="function(s, e){                        
                        window.close();}" />
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </tr>
        </form>
    </body>

