<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="FormCreate.aspx.cs" Inherits="WizOne.Posturi.FormCreate" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        
        function OnEndCallback(s, e) {
            pnlLoading.Hide();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
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
                        <label id="lblAngFiltru" runat="server" style="display:inline-block;">Formular</label>
                        <dx:ASPxComboBox ID="cmbForm" ClientInstanceName="cmbForm" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"
                                    CallbackPageSize="15" EnableCallbackMode="true"  >                              
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
                </tr>
            </table>
          
             <table width="90%"> 
			        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnRowInserting="grDate_RowInserting" OnRowUpdating="grDate_RowUpdating" OnRowDeleting="grDate_RowDeleting"  >
                        <SettingsBehavior AllowFocusedRow="true" />
                        <Settings ShowFilterRow="False" ShowColumnHeaders="true" />
                        <ClientSideEvents ContextMenu="ctx" />
                        <SettingsEditing Mode="Inline" />
                        <SettingsResizing ColumnResizeMode="Control" Visualization="Live"/>                        
				        <Columns>                      
                            <dx:GridViewCommandColumn Width="150px" ShowDeleteButton="true" ShowEditButton="true" ShowNewButtonInHeader="true" VisibleIndex="0" ButtonType="Image" Caption=" "  Name="butoaneGrid"/>
					        <dx:GridViewDataTextColumn FieldName="Rand" Name="Rand" Caption="Rand"  Width="50px"/>
					        <dx:GridViewDataTextColumn FieldName="Pozitie" Name="Pozitie" Caption="Pozitie"  Width="50px" />
                            <dx:GridViewDataComboBoxColumn FieldName="TipControl" Name="TipControl" Caption="Tip control" Width="100px" >
                                <Settings SortMode="DisplayText" />
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                            </dx:GridViewDataComboBoxColumn>
					        <dx:GridViewDataTextColumn FieldName="NumeEticheta" Name="NumeEticheta" Caption="Nume eticheta"  Width="250px" />
                            <dx:GridViewDataComboBoxColumn FieldName="ColoanaBD" Name="ColoanaBD" Caption="Coloana din BD" Width="150px" >
                                <Settings SortMode="DisplayText" />
                                <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.String" DropDownStyle="DropDown" />
                            </dx:GridViewDataComboBoxColumn>
				        </Columns>
			        </dx:ASPxGridView>
              </table>
       

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
