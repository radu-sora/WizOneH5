<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="ProgrameDetaliu.aspx.cs" Inherits="WizOne.ProgrameLucru.ProgrameDetaliu" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<script type="text/javascript">

    function OnTextChangedHandlerPtjDet(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetText());
    }
    function OnValueChangedHandlerPtjDet(s) {
        pnlCtl.PerformCallback(s.name + ";" + s.GetValue());
    }

</script>

		<table width="100%">
			<tr>
				<td align="left">					
				</td>			
				<td align="right">
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSave_Click" oncontextMenu="ctx(this,event)">
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>				
					<dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="Programe.aspx" >
						<Image Url="../Fisiere/Imagini/Icoane/iesire.png"></Image>
					</dx:ASPxButton>

				</td>		
			</tr>			
		</table>
				


    <div>

    <dx:ASPxCallbackPanel ID ="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" />
        <PanelCollection>
            <dx:PanelContent>
            <asp:DataList ID="DataList1" runat="server">
                <ItemTemplate>
			        <div>
                    <tr>
                    <td  valign="top">
			              <fieldset >
				           <legend class="legend-font-size"></legend>

                            <table width="40%" align="top">	      
                                <tr>
						            <td>		
							            <dx:ASPxLabel  ID="lblId"  Width="20"  runat="server"  Text="Id: "></dx:ASPxLabel >	
						            </td>
						            <td>
							            <dx:ASPxTextBox  ID="txtId" Width="50"  runat="server" Text='<%# Eval("Id") %>' AutoPostBack="false" >
                                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPtjDet(s); }" />
							            </dx:ASPxTextBox>
						            </td>
						            <td>		
							            <dx:ASPxLabel  ID="lblNume"  Width="50"  runat="server"  Text="Nume: "></dx:ASPxLabel >	
						            </td>
						            <td>
							            <dx:ASPxTextBox  ID="txtNume" Width="350"  runat="server" Text='<%# Eval("Denumire") %>' AutoPostBack="false" >
                                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPtjDet(s); }" />
							            </dx:ASPxTextBox>
						            </td>
					                <td>		
							            <dx:ASPxLabel  ID="lblDenScurta"  Width="100"  runat="server"  Text="Denumire scurta: "></dx:ASPxLabel >	
						            </td>
						            <td>
							            <dx:ASPxTextBox  ID="txtDenScurta" Width="100"  runat="server" Text='<%# Eval("DenumireScurta") %>' AutoPostBack="false" >
                                            <ClientSideEvents TextChanged="function(s,e){ OnTextChangedHandlerPtjDet(s); }" />
							            </dx:ASPxTextBox>
						            </td>

                                </tr>
                                </table>
			        </fieldset>
                 </td> 
                </tr>      
			</div>
        </ItemTemplate>
    </asp:DataList>


          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>

        <dx:ASPxPageControl SkinID="None" Width="100%" EnableViewState="false" ID="ASPxPageControl2" ClientInstanceName="ASPxPageControl2"
            runat="server" ActiveTabIndex="0" TabSpacing="0px" CssClass="pcTemplates" 
            EnableHierarchyRecreation="True"   >

                        <TabPages>

                        </TabPages>     
        

            <Paddings Padding="0px" PaddingLeft="12px" />
            <ContentStyle Font-Names="Tahoma" Font-Overline="False" Font-Size="11px">
                <Paddings Padding="0px" />
            <Border BorderColor="#6DA0E7" BorderStyle="Solid" BorderWidth="1px" />
            </ContentStyle>
            </dx:ASPxPageControl>
            

    </div>
</asp:Content>	