<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="IstoricModificari.aspx.cs" Inherits="WizOne.Avs.IstoricModificari" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        
        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        } 

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
    </table>
    

    <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
        <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" />
        <PanelCollection>
            <dx:PanelContent>

            <table width="40%">
                <tr>

                    <td>
                        <label id="lblAngFiltru" runat="server" style="display:inline-block;">Angajat</label>
                        <dx:ASPxComboBox ID="cmbAngFiltru" ClientInstanceName="cmbAngFiltru" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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
                   								

                    <td>
                        <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(5);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(6);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        </dx:ASPxButton>
                    </td>                    	
                </tr>
            </table>
            <table>
                <tr>
                    <td >
						<dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"   >
							<SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="NextColumn" />
							<Settings ShowFilterRow="False" HorizontalScrollBarMode="Auto"  />                
							<Columns>                            
								<dx:GridViewDataTextColumn FieldName="NumeAngajat" Name="Nume" Caption="Nume angajat"  Width="200px"/>
								<dx:GridViewDataDateColumn FieldName="DataModif" Name="DataModif" Caption="Data Modificare" ReadOnly="true" Width="100px" >
									<PropertiesDateEdit DisplayFormatString="dd/MM/yyyy"></PropertiesDateEdit>
								</dx:GridViewDataDateColumn>
								<dx:GridViewDataTextColumn FieldName="Salariu" Name="Salariu" Caption="Salariu"  Width="80px" />
								<dx:GridViewDataTextColumn FieldName="Functie" Name="Functie" Caption="Functie"  Width="250px" />
								<dx:GridViewDataTextColumn FieldName="COR" Name="COR" Caption="COR"  Width="250px" />
								<dx:GridViewDataTextColumn FieldName="Departament" Name="Departament" Caption="Organigrama"  Width="300px" />
                                <dx:GridViewDataTextColumn FieldName="NumePrenume" Name="NumePrenume" Caption="Nume si prenume"  Width="200px" />
							</Columns>
						</dx:ASPxGridView>
                    </td>
                </tr>
            </table>

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
