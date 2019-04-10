<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="CautaCOR.aspx.cs" Inherits="WizOne.Personal.CautaCOR" %>



<script language="javascript" type="text/javascript">

    window.onunload = function (e) {
        opener.CompleteazaCOR(); 
    };

    window.onbeforeunload = function (e) {
        opener.CompleteazaCOR();
    };

</script>
    <body>
        <form id="form1" runat="server">
			<tr>				
				<td >
					<dx:ASPxLabel  ID="lblCodCOR" runat="server"  Text="Cod COR" ></dx:ASPxLabel >	

					<dx:ASPxTextBox  ID="txtCodCOR" Width="125"  runat="server" AutoPostBack="false" >                        
					</dx:ASPxTextBox>
				</td>
			</tr>
			<tr>				
				<td >
					<dx:ASPxLabel  ID="lblDenumire" runat="server"  Text="Denumire" ></dx:ASPxLabel >	

					<dx:ASPxTextBox  ID="txtDenumire" Width="125"  runat="server" AutoPostBack="false" >                        
					</dx:ASPxTextBox>
		
                    <dx:ASPxButton ID="btnFiltru" runat="server"  OnClick="btnFiltru_Click">                                     
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>               
                    </dx:ASPxButton>
                </td>
			</tr>
            <dx:ASPxGridView ID="grDateCautaCOR" runat="server" ClientInstanceName="grDateCautaCOR" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  >
                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
                <Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />                 
                <Columns>                                                                                                        
                    <dx:GridViewDataTextColumn FieldName="F72202" Name="CodCOR" Caption="Cod COR"  Width="100px" />
                    <dx:GridViewDataTextColumn FieldName="F72204" Name="Denumire" Caption="Denumire"  Width="350px" />
                    <dx:GridViewDataTextColumn FieldName="F72206" Name="Versiune" Caption="Versiune"  Width="75px"/>
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

