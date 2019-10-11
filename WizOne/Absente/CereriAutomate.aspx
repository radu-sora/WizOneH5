<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="CereriAutomate.aspx.cs" Inherits="WizOne.Absente.CereriAutomate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function EmptyFields(s,e) {
            cmbAng.SetValue(null);
            cmbCtr.SetValue(null);
            cmbStare.SetValue(null);

            cmbSub.SetValue(null);
            cmbSec.SetValue(null);
            cmbFil.SetValue(null);
            cmbDept.SetValue(null);
            cmbSubDept.SetValue(null);
            cmbBirou.SetValue(null);
            cmbCateg.SetValue(null);

            pnlCtl.PerformCallback('EmptyFields');
        }


        $(document).ready(function () {
            bindButton();
        });
        function bindButton() {
            $('#<%=btnGen.ClientID%>').on('click', function () {
            $('#<%=lblProgres.ClientID%>').html('Procesare...');
        });
        }


        function VerifInterval(s, e) {
            if (cmbOraInc.GetValue() && cmbOraSf.GetValue()) {
                var oraInc = Number(cmbOraInc.GetValue().substring(0, 2)) * 60 + Number(cmbOraInc.GetValue().substring(3, 5));
                var oraSf = Number(cmbOraSf.GetValue().substring(0, 2)) * 60 + Number(cmbOraSf.GetValue().substring(3, 5));
   
                if (oraInc == oraSf) {
                    s.SetValue("");
                    swal({
                        title: "", text: "Ora inceput este egala cu ora sfarsit",
                        type: "warning"
                    });
                }

                var dif = 0;
                if (oraInc < oraSf)
                    diff = oraSf - oraInc;
                else
                    diff = ((24 * 60) - oraInc) + oraSf;

                var rez = diff / 60;
                txtNrOre.SetValue(rez.toFixed(4));
                txtNrOreInMinute.SetValue(diff);
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
                <dx:ASPxButton ID="btnGen" ClientInstanceName="btnGen" ClientIDMode="Static" runat="server" Text="Generare" AutoPostBack="true" OnClick="btnGen_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        e.processOnServer = true;
                    }" />
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
                                            <label id="lblRol" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Roluri</label>
                                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="Rol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRol'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblAbs" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Tip cerere</label>
                                            <dx:ASPxComboBox ID="cmbAbs" ClientInstanceName="cmbAbs" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbAbs' + ';' + s.GetValue()); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblDataInc" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Data inceput</label>
						                    <dx:ASPxDateEdit  ID="dtDataInc" Width="150" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                               <ClientSideEvents  ValueChanged="function(s, e) { pnlCtl.PerformCallback('dtDataInc' + ';' + s.GetValue()); }"/>
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>
                                         </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblDataSf" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Data sfarsit</label>
						                    <dx:ASPxDateEdit  ID="dtDataSf" Width="150" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy"  AutoPostBack="false"  >
                                                 <ClientSideEvents  ValueChanged="function(s, e) { pnlCtl.PerformCallback('dtDataSf' + ';' + s.GetValue()); }"/>
                                                    <CalendarProperties FirstDayOfWeek="Monday" />                                                   
							                </dx:ASPxDateEdit>
                                         </div>   
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblNr" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Nr. zile</label>
                                            <dx:ASPxSpinEdit ID="txtNr" style="display:inline-block; float:left; width:75px;" runat="server" AutoPostBack="false"/>  
                                        </div>  


                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblNrOre" runat="server" style="display:none;">Nr. ore</label>
                                            <dx:ASPxSpinEdit ID="txtNrOre" ClientInstanceName="txtNrOre" runat="server" style="display:inline-block; float:left; width:75px;" ClientVisible="false" MinValue="0" MaxValue="999">
                                                <SpinButtons ShowIncrementButtons="false"></SpinButtons> 
                                            </dx:ASPxSpinEdit>
                                            <dx:ASPxTextBox ID="txtNrOreInMinute" ClientInstanceName="txtNrOreInMinute" runat="server" style="display:inline-block; float:left; width:75px;" ClientVisible="false" ClientEnabled="false" />
                                        </div>

                                       <div style="float:left; padding-right:15px;">
                                            <label id="lblOraInc" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:90px;">Ora Inceput</label>
                                            <dx:ASPxComboBox ID="cmbOraInc" ClientInstanceName="cmbOraInc" runat="server" Width="75px" Visible="false" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s,e); }" />
                                            </dx:ASPxComboBox>
                                        </div>

                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblOraSf" runat="server" style="display:inline-block; float:left; padding-right:15px; width:90px;">Ora Sfarsit</label>
                                            <dx:ASPxComboBox ID="cmbOraSf" ClientInstanceName="cmbOraSf" runat="server" Width="75px" Visible="false" ValueField="Denumire" TextField="Denumire" ValueType="System.String" AutoPostBack="false" DropDownStyle="DropDownList">
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { VerifInterval(s,e);  }" />
                                            </dx:ASPxComboBox>
                                        </div>

                                         <div style="float:left; padding-right:15px;">
                                            <dx:ASPxRadioButton ID="rbPrel" runat="server" Text="Preluare manuala"   ClientInstanceName="rbPrel" RepeatDirection="Horizontal" GroupName="Prel1">                                             
                                             </dx:ASPxRadioButton>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <dx:ASPxRadioButton ID="rbPrel1" runat="server" Text="Preluare automata" ClientInstanceName="rbPrel1" RepeatDirection="Horizontal" GroupName="Prel1">                                              
                                             </dx:ASPxRadioButton>
                                         </div>
                                      </td>
                                    </tr>
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
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblCateg" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Categorie</label>
                                            <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="150px" ValueField="F72402" TextField="F72404" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
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
                                            <label id="lblCtr" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Contract</label>
                                            <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
                                        </div>                                                                                                                                                            
                                    </td> 
                                 </tr>
                                    <tr>
                                        <td>
                                            <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                                <label id="lblObs" runat="server">Observatii</label>
                                                <dx:ASPxMemo ID="txtObs" runat="server" Width="500px" Height="100px" meta:resourcekey="txtObsResource1"></dx:ASPxMemo>                                        
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
        <tr>
            <td>
                <div style="float:left; padding-right:15px; padding-bottom:10px;">
                    <asp:UpdatePanel ID="ProgressUpdatePanel" runat="server" UpdateMode="Conditional" OnUnload="UpdatePanel_Unload">
                        <ContentTemplate>                       
                            <asp:Label ID="lblProgres" runat="server" Text="" />
                        </ContentTemplate>
                    </asp:UpdatePanel>                                 
                </div>
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
    <dx:ASPxLoadingPanel ID="LoadingPanel" runat="server" ClientInstanceName="LoadingPanel"  Modal="True">
    </dx:ASPxLoadingPanel>

</asp:Content>
