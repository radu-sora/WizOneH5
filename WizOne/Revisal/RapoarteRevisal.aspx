<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="RapoarteRevisal.aspx.cs" Inherits="WizOne.Revisal.RapoarteRevisal" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
    function OnEndCallback(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "Atentie !", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    } 

    function OnClickMeniu(s, e) {
        if (e.item.name == 'RegSal') {
            document.getElementById("btnRegSal").style.display = "";
            document.getElementById("btnRapSal").style.display = "none";
            document.getElementById("grDateReg").style.display = "";
            document.getElementById("grDateRap").style.display = "none";
        }
        else if (e.item.name == 'RapSal') {
            document.getElementById("btnRegSal").style.display = "none";
            document.getElementById("btnRapSal").style.display = "";
            document.getElementById("grDateReg").style.display = "none";
            grDateRap.SetVisible(true);
            document.getElementById("grDateRap").style.display = "";
        }                                                    

    }


    function OnEndCallback(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
    }


</script>
    <body>

        <table width="100%">
                <tr>
                    <td align="right">    
                        <dx:ASPxButton ID="btnRegSal" ClientInstanceName="btnRegSal" ClientIDMode="Static"  runat="server"  Text="Genereaza registru" Width="180" OnClick="btnRegSal_Click"  oncontextMenu="ctx(this,event)"> 
                       
                        </dx:ASPxButton>                        
                        <dx:ASPxButton ID="btnRapSal" ClientInstanceName="btnRapSal" ClientIDMode="Static"  runat="server"  Text="Genereaza registru salariat" Width="180"  OnClick="btnRapSal_Click"  oncontextMenu="ctx(this,event)">                                                       
                     
                        </dx:ASPxButton>
                        <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="~/Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                            <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                        </dx:ASPxButton>
                    </td>
        </table>
        <table >
            <div >
                <dx:ASPxMenu ID="Menu" runat="server" Width="30%" ItemAutoWidth="true" ShowPopOutImages="True" AllowSelectItem="true">     
                    <Items>
                        <dx:MenuItem Name="RegSal" Text="Registru salariati" ToolTip="Registru salariati">                        
                        </dx:MenuItem>
                        <dx:MenuItem Name="RapSal" Text="Raport pe salariat" ToolTip="Raport pe salariat">
                            <Image>
                            </Image>
                        </dx:MenuItem>                        
                    </Items>
                    <ClientSideEvents ItemClick="function(s, e) {OnClickMeniu(s, e)}" />
                </dx:ASPxMenu>
            </div>
        </table>
        <table  width="100%">
            <tr>
                <td >
                    <br />
                    <div style="float:left; padding-right:15px;  padding-bottom:10px;">
                        <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px">Supervizor</label>
                        <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" >
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); grDateReg.PerformCallback('cmbRol;' + s.GetValue()); grDateRap.PerformCallback('cmbRol;' + s.GetValue()); pnlLoading.Hide()}" />
                        </dx:ASPxComboBox>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <dx:ASPxGridView ID="grDateReg" runat="server" ClientInstanceName="grDateReg" Style="display:inline-block" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnCustomCallback="grDateReg_CustomCallback" >
                        <SettingsBehavior AllowSelectByRowClick="false" AllowFocusedRow="false" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                        <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="False" VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" />                   
                        <ClientSideEvents ContextMenu="ctx"  />
                        <Columns>                       

                            <dx:GridViewDataTextColumn FieldName="Marca" Caption="Marca" ReadOnly="true" FixedStyle="Left"  Visible="false" Settings-AutoFilterCondition="Contains" />
                            <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left"  Width="200px" Settings-AutoFilterCondition="Contains">              
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="CNP" Caption="CNP" ReadOnly="true"  Width="150" Settings-AutoFilterCondition="Contains" >
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Nationalitate" Caption="Nationalitate" ReadOnly="true"  Width="150" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Adresa" Caption="Adresa" ReadOnly="true" Width="500" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Id" Caption="Nr. crt." ReadOnly="true" Width="70">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="TipDurata" Caption="Tip durata" ReadOnly="true" Width="100" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="COR" Caption="COR" ReadOnly="true" Width="400" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Stare" Caption="Stare" ReadOnly="true" Width="175" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>

                        </Columns>
                    
                    </dx:ASPxGridView>

                  
    
                </td>
            </tr>
            <tr>
                <td colspan="2">
                  
                    <dx:ASPxGridView ID="grDateRap" runat="server" ClientInstanceName="grDateRap" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false"  OnCustomCallback="grDateRap_CustomCallback" >
                        <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="true" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                        <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="False" VerticalScrollBarMode="Visible" VerticalScrollableHeight="500" />                   
                        <ClientSideEvents ContextMenu="ctx"  EndCallback="function(s,e) { OnEndCallback(s,e); }"  />
                        <Columns>    
                            <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                            <dx:GridViewDataTextColumn FieldName="Marca" Caption="Marca" ReadOnly="true" FixedStyle="Left"  Visible="false" Settings-AutoFilterCondition="Contains" />
                            <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left"  Width="400px" Settings-AutoFilterCondition="Contains">              
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="CNP" Caption="CNP" ReadOnly="true"  Width="150" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Nationalitate" Caption="Nationalitate" ReadOnly="true"  Width="150" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Adresa" Caption="Adresa" ReadOnly="true" Width="920" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>
                            <dx:GridViewDataTextColumn FieldName="Id" Caption="Nr. crt." ReadOnly="true" Visible ="false" Width="50"/>
                            <dx:GridViewDataTextColumn FieldName="Radiat" Caption="Radiat" ReadOnly="true" Width="100" Settings-AutoFilterCondition="Contains">
                                <Settings AllowHeaderFilter="True" AllowAutoFilter="False" SortMode="DisplayText" FilterMode="DisplayText" />
                            </dx:GridViewDataTextColumn>

                        </Columns>
                    
                    </dx:ASPxGridView>

                    <br />
    
                </td>
            </tr>
        </table>


    </body>

</asp:Content>