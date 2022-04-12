<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="IstoricModificari.aspx.cs" Inherits="WizOne.Pagini.IstoricModificari" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script language="javascript" type="text/javascript">
        
        function OnValueChangedHandler(s) { 
            pnlCtl.PerformCallback(s.name + ";" + s.GetText());
        } 

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

        function OnFiltruClick(s)
        {
            if (cmbTabela.GetValue() == null) {
                swal({
                    title: "", text: "Nu ati selectat tabela!",
                    type: "warning"
                });
                return;
            }


            if (deDataInceput.GetValue() == null && deDataSfarsit.GetValue() == null && cmbTipOp.GetValue() == null && cmbUtilWin.GetValue() == null && cmbUtilWSWO.GetValue() == null && cmbNumeCalc.GetValue() == null) {
                swal({
                    title: "Atentie", text: "Tabelele de WizTrace pot avea milioane de inregistrari.\n Sigur doriti afisarea datelor fara niciun filtru?",
                    type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Da!", cancelButtonText: "Nu", closeOnConfirm: true
                }, function (isConfirm) {
                    if (isConfirm) {
                        pnlLoading.Show();
                        pnlCtl.PerformCallback("FaraFiltru");
                    }
                });
            }
            else {
                pnlLoading.Show();
                pnlCtl.PerformCallback(s.name);
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
            <table style="width:55%;">
                <tr>
                    <td style="padding-right:15px !important;">
                        <dx:ASPxLabel id="lblDataInceput" runat="server" style="display:inline-block;" Text="Data inceput"></dx:ASPxLabel>
						<dx:ASPxDateEdit  ID="deDataInceput" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                            <CalendarProperties FirstDayOfWeek="Monday" />
						</dx:ASPxDateEdit>
                    </td>
                    <td style="padding-right:15px !important;">
                        <dx:ASPxLabel id="lblDataSfarsit" runat="server" style="display:inline-block;" Text="Data sfarsit"></dx:ASPxLabel>
						<dx:ASPxDateEdit  ID="deDataSfarsit" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" AutoPostBack="false">
                            <CalendarProperties FirstDayOfWeek="Monday" />
						</dx:ASPxDateEdit>
                    </td>
                    <td style="padding-right:15px !important;">
                        <dx:ASPxLabel  ID="lblTabela" runat="server"  style="display:inline-block;"  Text="Tabela"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbTabela" runat="server" ClientInstanceName="cmbTabela" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                            <ClientSideEvents SelectedIndexChanged="function(s,e){ OnValueChangedHandler(s); }" />
                        </dx:ASPxComboBox>
                    </td>
                    <td style="padding-right:15px !important;">
                        <dx:ASPxLabel  ID="lblTipOp" runat="server"  style="display:inline-block;"  Text="Tip operatie"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbTipOp" runat="server" ClientInstanceName="cmbTipOp" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150"  
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>
   					<td style="padding-right:15px !important;">
                        <dx:ASPxLabel  id="lblUtilWin" runat="server" style="display:inline-block;" Text="Utilizator Windows"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbUtilWin" runat="server" ClientInstanceName="cmbUtilWin" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>
					<td style="padding-right:15px !important;">
                        <dx:ASPxLabel  id="lblUtilWSWO" runat="server" style="display:inline-block;" Text="Utilizator WS/WO"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbUtilWSWO" runat="server" ClientInstanceName="cmbUtilWSWO" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>
					<td style="padding-right:15px !important;">
                        <dx:ASPxLabel  id="lblNumeCalc" runat="server" style="display:inline-block;" Text="Nume calculator"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbNumeCalc" runat="server" ClientInstanceName="cmbNumeCalc" ClientIDMode="Static" Width="150px" ValueField="Id" DropDownWidth="150" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltru" ClientInstanceName="btnFiltru" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) { OnFiltruClick(s); }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        </dx:ASPxButton>
                    </td>
                    <td>
                        <dx:ASPxButton ID="btnFiltruSterge" ClientInstanceName="btnFiltruSterge" ClientIDMode="Static" runat="server" AutoPostBack="false" oncontextMenu="ctx(this,event)">
                            <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                pnlCtl.PerformCallback(s.name);
                            }" />
                            <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        </dx:ASPxButton>
                    </td>                    	
                </tr>
            </table>
            <br />
            <table style="width:100%;">
                <tr>
                    <td >
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  >
                            <SettingsBehavior AllowFocusedRow="true"  />
                            <Settings ShowFilterRow="true" ShowColumnHeaders="true" />  
                            <SettingsEditing Mode="Inline" />  
                            <ClientSideEvents  ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />           
                            <Columns>                       
						        <dx:GridViewDataTextColumn FieldName="Tabela" Name="Tabela" Caption="Tabela" ReadOnly="true" Width="100px" VisibleIndex="1" />
                                <dx:GridViewDataTextColumn FieldName="COL_ID1" Name="COL_ID1" Caption="Cheie primara" ReadOnly="true" Width="100px" VisibleIndex="2" />
                                <dx:GridViewDataTextColumn FieldName="VAL_ID1" Name="VAL_ID1" Caption="Val. cheie primara" ReadOnly="true" Width="100px" VisibleIndex="3" />
						        <dx:GridViewDataTextColumn FieldName="Nume_Camp" Name="Nume_Camp" Caption="Denumire camp" ReadOnly="true" Width="100px" VisibleIndex="4" />
						        <dx:GridViewDataTextColumn FieldName="Cod_Op" Name="Cod_Op" Caption="Cod op." ReadOnly="true" Width="100px" VisibleIndex="5" />
						        <dx:GridViewDataTextColumn FieldName="VAL_OLD" Name="VAL_OLD" Caption="Valoare veche" ReadOnly="true" Width="100px" VisibleIndex="6" />
						        <dx:GridViewDataTextColumn FieldName="VAL_NEW" Name="VAL_NEW" Caption="Valoare noua" ReadOnly="true" Width="100px" VisibleIndex="7" />
						        <dx:GridViewDataTextColumn FieldName="Data" Name="Data" Caption="Data" ReadOnly="true" Width="100px" VisibleIndex="8" />
						        <dx:GridViewDataTextColumn FieldName="USER_WIN" Name="USER_WIN" Caption="Utilizator Windows" ReadOnly="true" Width="100px" VisibleIndex="9" />
						        <dx:GridViewDataTextColumn FieldName="USER_WS" Name="USER_WS" Caption="Utilizator WS" ReadOnly="true" Width="100px" VisibleIndex="10" />					
                                <dx:GridViewDataTextColumn FieldName="COMPUTER_NAME" Name="COMPUTER_NAME" Caption="Nume calculator" ReadOnly="true" Width="100px" VisibleIndex="11" />	
                                <dx:GridViewDataTextColumn FieldName="IdAuto" Name="IdAuto" Caption="IdAuto" Visible="false" ReadOnly="true" Width="100px"  />	
                            </Columns>
                        </dx:ASPxGridView>
                    </td>
                </tr>
            </table>

          </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxCallbackPanel>


    

</asp:Content>
