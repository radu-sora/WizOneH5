<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="Detalii.aspx.cs" Inherits="WizOne.Programe.Detalii" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dx:ASPxCallbackPanel ID="ASPxCallbackPanel1" ClientIDMode="Static" ClientInstanceName="pnlCtl" ScrollBars="None" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false">
        <ClientSideEvents EndCallback="function (s,e) { OnPanelEndCallback(); }"/>
        <PanelCollection>
            <dx:PanelContent>
		        <table style="width:100%">
			        <tr>
                        <td class="pull-left">
                            <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
                        </td>			
				        <td class="pull-right">
                            <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                                <ClientSideEvents Click="function(s, e) {
                                    pnlLoading.Show();
                                    pnlCtl.PerformCallback('btnSave');
                                }" />
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                            </dx:ASPxButton>				
					        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" PostBackUrl="Lista.aspx" >
						        <Image Url="../Fisiere/Imagini/Icoane/iesire.png"></Image>
					        </dx:ASPxButton>
				        </td>		
			        </tr>	
                    <tr>
                        <td colspan="2">
                            <div class="row">
                                <div class="col-md-12" style="margin-bottom:20px;">
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblId" runat="server" Text="Id" Width="30"/>
                                        <dx:ASPxTextBox ID="txtId" Width="50" runat="server" ClientEnabled="false"/>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumire" runat="server" Text="Denumire" Width="70"/>
                                        <dx:ASPxTextBox ID="txtDenumire" Width="400" runat="server" AutoPostBack="false"/>
                                    </div>
                                    <div class="ctl_inline">
                                        <dx:ASPxLabel ID="lblDenumireScurta" runat="server" Text="Denumire scurta" Width="110"/>
                                        <dx:ASPxTextBox ID="txtDenumireScurta" Width="100" runat="server" AutoPostBack="false"/>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr> 
                    <tr>
                        <td colspan="2">
                            <dx:ASPxPageControl ID="tabCtr" ClientInstanceName="tabCtr" runat="server" Width="100%" Height="100%" CssClass="dxtcFixed" ActiveTabIndex="0" AutoPostBack="false" OnCallback="tabCtr_Callback">
                                <TabPages>
                                    <dx:TabPage Text="Date Generale">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl1" runat="server">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="ctl_inline">
                                                            <dx:ASPxLabel ID="lblOraSchIn" runat="server" Text="Ora Sch. In" Width="75"/>
							                                <dx:ASPxTimeEdit ID="txtOraSchIn" runat="server" AutoPostBack="false" Width="50" SpinButtons-ShowIncrementButtons="false"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </dx:ContentControl>
                                        </ContentCollection>
                                    </dx:TabPage>
                                </TabPages>
                            </dx:ASPxPageControl>
                        </td>
                    </tr>
		        </table>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>
</asp:Content>	