<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="TrimitereFluturasi.aspx.cs" Inherits="WizOne.Pagini.TrimitereFluturasi" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function EmptyFields(s, e) {
            pnlCtl.PerformCallback('EmptyFields');
        }

        function OnEndCallback(s, e) {
            if (s.cpAlertMessage != null) {
                swal({
                    title: "Atentie !", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
            pnlLoading.Hide();
        }

        var __focusedCtl = null;
        function onGotFocus(s, e) {
            __focusedCtl = s;
        }

    </script>

</asp:Content>

 

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">  
        .centerText input {  
            text-align: center;  
        }  
    </style>

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">  
                <dx:ASPxButton ID="btnTrimitere" ClientInstanceName="btnTrimitere" ClientIDMode="Static" runat="server" Text="Trimitere" AutoPostBack="true" OnClick="btnTrimitere_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/mail.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnGenerare" ClientInstanceName="btnGenerare" ClientIDMode="Static" runat="server" Text="Generare documente" AutoPostBack="true" OnClick="btnGenerare_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/finalizare.png"></Image>
                </dx:ASPxButton>  
                <dx:ASPxButton ID="btnSalvare" ClientInstanceName="btnSalvare" ClientIDMode="Static" runat="server" Text="Salvare" AutoPostBack="true" OnClick="btnSalvare_Click" oncontextMenu="ctx(this,event)" >
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
                    <ClientSideEvents EndCallback="function (s,e) { OnEndCallback(s,e); }" CallbackError="function (s,e) { pnlLoading.Hide(); }" BeginCallback="function (s,e) { pnlLoading.Show(); }" />
                    <PanelCollection>
                        <dx:PanelContent>

                            <table style="margin-left:15px;">
                                <tr>
                                    <td>
                                         <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px">Supervizor</label>
                                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlLoading.Show(); pnlCtl.PerformCallback('cmbRol'); }" />
                                            </dx:ASPxComboBox>
                                        </div>                      
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" SelectInputTextOnClick="true"
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
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblCateg" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Categorie</label>
                                            <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
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
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblMail" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Are e-mail</label>
                                            <dx:ASPxComboBox ID="cmbMail" ClientInstanceName="cmbMail" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                             </dx:ASPxComboBox>
                                        </div> 
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblParola" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Are parola</label>
                                            <dx:ASPxComboBox ID="cmbParola" ClientInstanceName="cmbParola" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                             </dx:ASPxComboBox>
                                        </div>                                         
                                    </td> 
                                 </tr>
                                 <tr>
                                     <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblAnLuna" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Luna/An</label>                                     
                                            <dx:ASPxDateEdit ID="txtAnLuna" ClientInstanceName="txtAnLuna" ClientIDMode="Static" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >               
                                                <CalendarProperties FirstDayOfWeek="Monday" />
                                            </dx:ASPxDateEdit>
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
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" />                   
                    <ClientSideEvents ContextMenu="ctx"  />
                    <Columns>
                        
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains" Width="70" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Caption="Angajat" ReadOnly="true" FixedStyle="Left" VisibleIndex="3" Width="200px" Settings-AutoFilterCondition="Contains"/>
              
                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true"  Width="150" />
                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true"  Width="150"/>
                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Width="150" />
                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Width="150"/>
                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Width="150"/>
                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Categorie" Caption="Categorie" ReadOnly="true" Width="100" />
                        <dx:GridViewDataTextColumn FieldName="Email" Caption="E-mail" ReadOnly="true" Width="150" />
                        <dx:GridViewDataTextColumn FieldName="F10016" Caption="Parola" ReadOnly="true" Visible="false" Width="50" />
                        <dx:GridViewDataCheckColumn FieldName="AreMail"  Caption="Are e-mail" ReadOnly="true"  Width="80px"  />
                        <dx:GridViewDataCheckColumn FieldName="AreParola"  Caption="Are parola" ReadOnly="true"  Width="80px"  />
                    </Columns>
                    
                </dx:ASPxGridView>

                <br />
    
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <dx:ASPxCardView ID="crView" ClientInstanceName="crView" runat="server" Width="50%"
                    AutoGenerateColumns="False" KeyFieldName="Id" >           
                    <Columns>
                        <dx:CardViewColumn FieldName="Id" />
                        <dx:CardViewColumn FieldName="Denumire" />  
                    </Columns>
                    <CardLayoutProperties>
                        <Items>                            
                            <dx:CardViewColumnLayoutItem ColumnName="Id" Visible="false"/>
                            <dx:CardViewColumnLayoutItem ColumnName="Denumire" ShowCaption="False" />              
                        </Items>
                    </CardLayoutProperties>        
                    <SettingsBehavior AllowSelectByCardClick="true" />
                    <SettingsPager>  
                        <SettingsTableLayout RowsPerPage="8" />  
                    </SettingsPager>
                    <Styles>
                        <FlowCard CssClass="flowCardStyle"></FlowCard>
                        <Card Width="100" Height="10px" />
                    </Styles>
                </dx:ASPxCardView>
            </td>
        </tr>

       <tr>
            <td>
                <dx:ASPxRoundPanel ID="pnlCorp" runat="server" ShowHeader="true" ShowCollapseButton="true" AllowCollapsingByHeaderClick="true" Width="100%" HeaderText="Text mail" CssClass="pnlAlign indentTop10" >
                    <HeaderStyle Font-Bold="true" />
                    <PanelCollection>
                        <dx:PanelContent>
                            <table>
                                <tr>
                                    <td><label id="lblSubiect" runat="server">Subiect</label></td>
                                    <td>
                                        <dx:ASPxTextBox ID="txtSubiect" runat="server" ClientInstanceName="txtSubiect" ClientIDMode="Static" Width="390px">
                                            <ClientSideEvents GotFocus="onGotFocus" />    
                                        </dx:ASPxTextBox>
                                    </td>
                                </tr>
                            </table>

                            <dx:ASPxHtmlEditor ID="txtContinut" runat="server" ClientInstanceName="txtContinut" Height="370px" Width="100%">
                                <ClientSideEvents GotFocus="onGotFocus" />
                                <Settings AllowHtmlView ="false" />
                                <SettingsDialogs>
                                    <InsertImageDialog>
                                        <SettingsImageUpload UploadFolder="~/UploadFiles/Images/">
                                            <ValidationSettings AllowedFileExtensions=".jpe,.jpeg,.jpg,.gif,.png" MaxFileSize="500000">
                                            </ValidationSettings>
                                        </SettingsImageUpload>
                                    </InsertImageDialog>
                                </SettingsDialogs>
                            </dx:ASPxHtmlEditor>

                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxRoundPanel>
                </td>
        </tr>
        <tr>
            <td>
                <div style="float:left; padding-right:15px; padding-bottom:10px;">
                    <label id="lblLog" runat="server">Log</label>
                    <dx:ASPxMemo ID="txtLog" runat="server" Width="800px" Height="200px" ReadOnly="true" meta:resourcekey="txtLog"></dx:ASPxMemo>                                        
                </div>
            </td> 
        </tr>
    </table>




</asp:Content>
