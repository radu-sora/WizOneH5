<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajSpecial.aspx.cs" Inherits="WizOne.Pontaj.PontajSpecial" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">



    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">  
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Initializare" AutoPostBack="true" OnClick="btnInit_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnExit" ClientInstanceName="btnExit" ClientIDMode="Static" runat="server" Text="Iesire" AutoPostBack="true" PostBackUrl="../Pagini/MainPage.aspx" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/iesire.png"></Image>
                </dx:ASPxButton>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <dx:ASPxCallbackPanel ID="pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtl" runat="server" OnCallback="pnlCtl_Callback" SettingsLoadingPanel-Enabled="false" >
                    <ClientSideEvents EndCallback="function (s,e) { pnlLoading.Hide(); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <table style="margin-left:15px;">                                              
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"
                                                        CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}"  >
                                                <Columns>
                                                    <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                    <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                                </Columns>
                                            </dx:ASPxComboBox>
                                        </div>                               
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Subcomp.</label>
                                            <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Filiala</label>
                                            <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Sectie</label>
                                            <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Dept.</label>
                                            <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSubDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Subdept.</label>
                                            <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblBirou" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Birou</label>
                                            <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                             </dx:ASPxComboBox>
                                        </div>   
                                         <div style="float:left; padding-right:15px;" >
                                             <label id="Label1" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:75px;" > </label>                 
                                             <label id="Label2" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:75px;" > </label>    
                                             <label id="Label3" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;" > </label>  
                                        </div>                                                                                                                                                             
                                    </td> 
                                 </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;  padding-bottom:10px;" >
                                            <label id="lblDeLa" runat="server" style="display:inline-block; float:left;  min-width:54px; width:80px;">De la</label>
						                    <dx:ASPxDateEdit  ID="dtDataStart" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>	
                                         </div>  
                                         <div style="float:left; padding-right:15px;">
                                             <label id="lblLa" runat="server" style="display:inline-block; float:left;   width:70px;">La</label>
						                    <dx:ASPxDateEdit  ID="dtDataSfarsit" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>
                                         </div>
                                         <div style="float:left; padding-right:15px;">
                                            <label id="lblSablon" runat="server" style="display:inline-block; float:left;  min-width:54px; width:60px;">Sablon</label>
                                            <dx:ASPxComboBox ID="cmbSablon" ClientInstanceName="cmbSablon" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSablon'); }" />                                 
                                            </dx:ASPxComboBox>
                                         </div>
                                     </td>   
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblNumeSablon" runat="server" style="display:inline-block; float:left;  min-width:75px; width:80px;">Nume sablon</label>
							                <dx:ASPxTextBox  ID="txtNumeSablon" style="display:inline-block; float:left; width:100px;" runat="server"  AutoPostBack="false" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblNrZileSablon" runat="server" style="display:inline-block; float:left;  min-width:54px; width:70px;">Nr. zile</label>
                                            <dx:ASPxComboBox ID="cmbNrZileSablon" ClientInstanceName="cmbNrZileSablon" ClientIDMode="Static" runat="server" Width="50px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbNrZileSablon'); }" />
                                                <Items>
                                                    <dx:ListEditItem Value="1" Text="1" />
                                                    <dx:ListEditItem Value="2" Text="2" />
                                                    <dx:ListEditItem Value="3" Text="3" />
                                                    <dx:ListEditItem Value="4" Text="4" />
                                                    <dx:ListEditItem Value="5" Text="5" />
                                                    <dx:ListEditItem Value="6" Text="6" />
                                                    <dx:ListEditItem Value="7" Text="7" />
                                                    <dx:ListEditItem Value="8" Text="8" />
                                                    <dx:ListEditItem Value="9" Text="9" />
                                                    <dx:ListEditItem Value="10" Text="10" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                         </div> 
                                        <div style="float:left; padding-right:15px;">
							                <dx:ASPxTextBox  ID="txtZiua1" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua2" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua3" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua4" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua5" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua6" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua7" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua8" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua9" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                            <dx:ASPxTextBox  ID="txtZiua10" style="display:inline-block; float:left; width:75px;" runat="server" Visible="false"  AutoPostBack="false" />
                                       </div>
                                        <div style="float:left;padding-right:15px;">
                                            <dx:ASPxButton ID="btnSablon" runat="server"  RenderMode="Link" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                                <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback('btnSablon'); }" />
                                            </dx:ASPxButton>
                                        </div>  
                                        <div style="float:left;padding-right:15px;">
                                            <dx:ASPxButton ID="btnSterge" runat="server"  RenderMode="Link" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                                <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                                                <ClientSideEvents Click="function(s, e) { pnlCtl.PerformCallback('btnSterge'); }" />
                                            </dx:ASPxButton>
                                        </div> 
                                    </td>
                                </tr>
                            </table>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>


                <div style="float:left; padding:0px 15px;">
                    <dx:ASPxButton ID="btnFiltru" runat="server" Text="Filtru" OnClick="btnFiltru_Click" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupa.png"></Image>
                        <ClientSideEvents Click="function(s, e) {
                                        pnlLoading.Show();
                                        e.processOnServer = true;
                                    }" />
                    </dx:ASPxButton>
                </div>

                <div style="float:left;">
                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        <ClientSideEvents Click="EmptyFields" />
                    </dx:ASPxButton>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">

                <br />

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCustomCallback="grDate_CustomCallback" OnDataBound="grDate_DataBound" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" />                   
                    <ClientSideEvents ContextMenu="ctx"  />
                    <Columns>
                        
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="250px" Settings-AutoFilterCondition="Contains"/>
              
                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true"  Width="200" />
                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true"  Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Width="200" />
                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Width="200"/>
                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Width="100" />

                    </Columns>
                    
                </dx:ASPxGridView>

                <br />
    
            </td>
        </tr>
    </table>




</asp:Content>
