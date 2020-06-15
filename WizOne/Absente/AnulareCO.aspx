<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="AnulareCO.aspx.cs" Inherits="WizOne.Absente.AnulareCO" %>


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
                <dx:ASPxButton ID="btnAnulare" ClientInstanceName="btnAnulare" ClientIDMode="Static" runat="server" Text="Anulare" AutoPostBack="true" OnClick="btnAnulare_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>  
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
                        <label id="lblAnLuna" runat="server" oncontextMenu="ctx(this,event)">Luna/An</label><br />
                        <dx:ASPxDateEdit ID="txtAnLuna" ClientInstanceName="txtAnLuna" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >     
                            <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtAnLuna'); }" />
                            <CalendarProperties FirstDayOfWeek="Monday" />
                        </dx:ASPxDateEdit>
                    </td>
                    <td style="padding-right:15px !important;">
                        <dx:ASPxLabel  ID="lblPer" runat="server"  style="display:inline-block;"  Text="Perioada"></dx:ASPxLabel>
                        <dx:ASPxComboBox ID="cmbPerioada" runat="server" ClientInstanceName="cmbPerioada" ClientIDMode="Static" Width="100px" ValueField="Id" DropDownWidth="100" 
                            TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                        </dx:ASPxComboBox>
                    </td>                                      	
                </tr>
            </table>
            <br />
            <table style="width:57%;">
                <tr>
                    <td >
                        <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  >
                            <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                            <Settings ShowFilterRow="true" ShowColumnHeaders="true" />  
                            <SettingsEditing Mode="Inline" />  
                            <ClientSideEvents  ContextMenu="ctx" EndCallback="function(s,e) { OnEndCallback(s,e); }" />           
                            <Columns>  
                                <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />
                                 <dx:GridViewDataComboBoxColumn FieldName="F10003" Name="F10003" Caption="Angajat" ReadOnly="true" Width="250px" >           
                                     <Settings SortMode="DisplayText" />
                                    <PropertiesComboBox TextField="NumeComplet" ValueField="F10003" ValueType="System.Int32" DropDownStyle="DropDown">                                        
                                    </PropertiesComboBox>
                                </dx:GridViewDataComboBoxColumn>        
						        <dx:GridViewDataTextColumn FieldName="Stare" Name="Stare" Caption="Stare" ReadOnly="true" Width="150px"  />
                                <dx:GridViewDataTextColumn FieldName="ZileCO" Name="ZileCO" Caption="Zile CO (total)" ReadOnly="true" Width="100px"  />
                                <dx:GridViewDataTextColumn FieldName="ZileCOAnC" Name="ZileCOAnC" Caption="Zile CO (an curent)" ReadOnly="true" Width="100px"  />
                                <dx:GridViewDataTextColumn FieldName="ZileCOAnAnt" Name="ZileCOAnAnt" Caption="Zile CO (an anterior)" ReadOnly="true" Width="100px"  />
                                <dx:GridViewDataTextColumn FieldName="ZileCOAnAnt2" Name="ZileCOAnAnt2" Caption="Zile CO (an anterior 2)" ReadOnly="true" Width="100px"  />
                                <dx:GridViewDataTextColumn FieldName="ZileCOMaiVechi" Name="ZileCOMaiVechi" Caption="Zile CO (mai vechi)" ReadOnly="true" Width="100px"  />                    
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
