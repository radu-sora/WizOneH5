﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajEchipa.aspx.cs" Inherits="WizOne.Pontaj.PontajEchipa" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">

        function eventKeyPress(evt, s) {
            var cell = grDate.GetFocusedCell();
            var col = cell.column.fieldName;
            var f10003 = grDate.GetRowKey(cell.rowVisibleIndex);
            
            txtCol.Set('coloana', col);
            txtCol.Set('f10003', f10003);
        }

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
        }

        function OnClickDetaliat(s,e)
        {
            var edc = txtCol.Get('coloana');
            var edc1 = txtCol.Get('f10003');

            if (txtCol.Get('coloana') || txtCol.Get('f10003'))
            {
                pnlLoading.Show();
                e.processOnServer = true;
            }
            else
            {
                swal({
                    title: "Atentie !", text: "Nu exista celula selectata",
                    type: "warning"
                });
                e.processOnServer = false;
            }
        }

        function OnInit(s, e) 
        {
            popUpInit.Hide();

            if (grDate.cpAngajatiPlecatiCuPontari)
            {
                swal(
                    {
                        title: "Angajati plecati", text: "Urmatorii angajati au plecat si au postari care vor fi sterse.\nMarca: " + grDate.cpAngajatiPlecatiCuPontari,
                        type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", cancelButtonText: "Continua fara stergere", confirmButtonText: "Continua cu stergere", closeOnConfirm: true
                        },
                        function (isConfirm)
                        {
                            if (isConfirm)
                            {
                                txtCol.Set('mod', 1);
                                pnlLoading.Show();
                                e.processOnServer = true;
                            }
                            else
                            {
                                txtCol.Set('mod', 0);
                                pnlLoading.Show();
                                e.processOnServer = true;
                            }
                        });
            }
            else
            {
                txtCol.Set('mod', 0);
                pnlLoading.Show();
                e.processOnServer = true;
            }
            //grDate.PerformCallback('btnInitParam;' + chkNormaZL.GetChecked() + ";" + chkNormaSD.GetChecked() + ";" + chkNormaSL.GetChecked());
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
                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) { popUpInit.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnTransfera" ClientInstanceName="btnTransfera" ClientIDMode="Static" runat="server" Text="Transfera" AutoPostBack="true" OnClick="btnTransfera_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/duplicare.png"></Image>
                </dx:ASPxButton>                
                <dx:ASPxButton ID="btnPeAng" ClientInstanceName="btnPeAng" ClientIDMode="Static" runat="server" Text="Pontaj pe Angajat" AutoPostBack="false" OnClick="btnPeAng_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/n2.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickDetaliat(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPeZi" ClientInstanceName="btnPeZi" ClientIDMode="Static" runat="server" Text="Pontaj pe Zi" AutoPostBack="false" OnClick="btnPeZi_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickDetaliat(s,e); }" />
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
                                        <div style="float:left; padding-right:65px; padding-bottom:10px;">
                                            <label id="lblAnLuna" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Luna/An</label>
                                             <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" />
                                        </div>
                                        <div style="float:left; padding-right:15px; vertical-align:middle; display:inline-block;">
                                            <label id="lblRol" runat="server" style="float:left; padding-right:15px; width:80px;">Roluri</label>
                                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRolAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                            <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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
                                            <label id="lblStare" runat="server" style="display:inline-block; float:left; padding-right:25px; width:80px;">Stare</label>
                                            <dx:ASPxComboBox ID="cmbStare" ClientInstanceName="cmbStare" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  />
                                        </div>
                                    </td>
                                </tr>                                
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;padding-bottom:10px;">
                                            <label id="lblCtr" runat="server" style="display:inline-block; float:left; padding-right:25px; width:80px;">Contract</label>
                                            <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"  />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSub" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Subcomp.</label>
                                            <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblFil" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Filiala</label>
                                            <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSec" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Sectie</label>
                                            <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px; padding-bottom:10px;">
                                            <label id="lblDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Dept.</label>
                                            <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblSubDept" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Subdept.</label>
                                            <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" />
                                        </div>
                                        <div style="float:left; padding-right:15px;">
                                            <label id="lblBirou" runat="server" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Birou</label>
                                            <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" />
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

                <dx:ASPxHiddenField ID="txtCol" runat="server" ClientInstanceName="txtCol" ClientIDMode="Static"></dx:ASPxHiddenField>

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" >
                    <SettingsBehavior ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True"  />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" />

                    <Columns>
                        
                        <dx:GridViewDataComboBoxColumn FieldName="StareDenumire" Name="StareDenumire" Caption="Stare" ReadOnly="true" Width="100px" FixedStyle="Left" VisibleIndex="0" CellStyle-HorizontalAlign="Center" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" ShowInCustomizationForm="false" FixedStyle="Left" VisibleIndex="0" />
                        <dx:GridViewDataTextColumn FieldName="AngajatNume" Caption="Angajat" ReadOnly="true" ShowInCustomizationForm="false"  FixedStyle="Left" VisibleIndex="3" Width="150px"/>
                        <dx:GridViewDataTextColumn FieldName="Norma" Caption="Norma" ReadOnly="true" ShowInCustomizationForm="false"  FixedStyle="Left" VisibleIndex="3" Width="80px"/>
                        <dx:GridViewDataTextColumn FieldName="DescContract" Caption="Contract" ReadOnly="true" ShowInCustomizationForm="false"  FixedStyle="Left" VisibleIndex="3" Width="150px"/>
                                              
                        <dx:GridViewDataTextColumn FieldName="Ziua1" Caption="1" ReadOnly="true" VisibleIndex="11" />
                        <dx:GridViewDataTextColumn FieldName="Ziua2" Caption="2" ReadOnly="true" VisibleIndex="12" />
                        <dx:GridViewDataTextColumn FieldName="Ziua3" Caption="3" ReadOnly="true" VisibleIndex="13" />
                        <dx:GridViewDataTextColumn FieldName="Ziua4" Caption="4" ReadOnly="true" VisibleIndex="14" />
                        <dx:GridViewDataTextColumn FieldName="Ziua5" Caption="5" ReadOnly="true" VisibleIndex="15"/>
                        <dx:GridViewDataTextColumn FieldName="Ziua6" Caption="6" ReadOnly="true" VisibleIndex="16" />
                        <dx:GridViewDataTextColumn FieldName="Ziua7" Caption="7" ReadOnly="true" VisibleIndex="17" />
                        <dx:GridViewDataTextColumn FieldName="Ziua8" Caption="8" ReadOnly="true" VisibleIndex="18" />
                        <dx:GridViewDataTextColumn FieldName="Ziua9" Caption="9" ReadOnly="true" VisibleIndex="19" />
                        <dx:GridViewDataTextColumn FieldName="Ziua10" Caption="10" ReadOnly="true" VisibleIndex="20" />
                        <dx:GridViewDataTextColumn FieldName="Ziua11" Caption="11" ReadOnly="true" VisibleIndex="21" />
                        <dx:GridViewDataTextColumn FieldName="Ziua12" Caption="12" ReadOnly="true" VisibleIndex="22" />
                        <dx:GridViewDataTextColumn FieldName="Ziua13" Caption="13" ReadOnly="true" VisibleIndex="23" />
                        <dx:GridViewDataTextColumn FieldName="Ziua14" Caption="14" ReadOnly="true" VisibleIndex="24" />
                        <dx:GridViewDataTextColumn FieldName="Ziua15" Caption="15" ReadOnly="true" VisibleIndex="25" />
                        <dx:GridViewDataTextColumn FieldName="Ziua16" Caption="16" ReadOnly="true" VisibleIndex="26" />
                        <dx:GridViewDataTextColumn FieldName="Ziua17" Caption="17" ReadOnly="true" VisibleIndex="27" />
                        <dx:GridViewDataTextColumn FieldName="Ziua18" Caption="18" ReadOnly="true" VisibleIndex="28" />
                        <dx:GridViewDataTextColumn FieldName="Ziua19" Caption="19" ReadOnly="true" VisibleIndex="29" />
                        <dx:GridViewDataTextColumn FieldName="Ziua20" Caption="20" ReadOnly="true" VisibleIndex="30" />
                        <dx:GridViewDataTextColumn FieldName="Ziua21" Caption="21" ReadOnly="true" VisibleIndex="31" />
                        <dx:GridViewDataTextColumn FieldName="Ziua22" Caption="22" ReadOnly="true" VisibleIndex="32" />
                        <dx:GridViewDataTextColumn FieldName="Ziua23" Caption="23" ReadOnly="true" VisibleIndex="33" />
                        <dx:GridViewDataTextColumn FieldName="Ziua24" Caption="24" ReadOnly="true" VisibleIndex="34" />
                        <dx:GridViewDataTextColumn FieldName="Ziua25" Caption="25" ReadOnly="true" VisibleIndex="35" />
                        <dx:GridViewDataTextColumn FieldName="Ziua26" Caption="26" ReadOnly="true" VisibleIndex="36" />
                        <dx:GridViewDataTextColumn FieldName="Ziua27" Caption="27" ReadOnly="true" VisibleIndex="37" />
                        <dx:GridViewDataTextColumn FieldName="Ziua28" Caption="28" ReadOnly="true" VisibleIndex="38" />
                        <dx:GridViewDataTextColumn FieldName="Ziua29" Caption="29" ReadOnly="true" VisibleIndex="39" />
                        <dx:GridViewDataTextColumn FieldName="Ziua30" Caption="30" ReadOnly="true" VisibleIndex="40" />
                        <dx:GridViewDataTextColumn FieldName="Ziua31" Caption="31" ReadOnly="true" VisibleIndex="41" />


                        <dx:GridViewDataTextColumn FieldName="Culoare" Caption="Stare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="StareDenumire" Caption="StareDenumire" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />


                    </Columns>
                    
                </dx:ASPxGridView>

                <br />
    
            </td>
        </tr>
    </table>



    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="200px" HeaderText="Parametrii recalcul"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="false" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        e.processOnServer = false;
                                        OnInit(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td style="width:100%; padding-left:70px;">
                                <dx:ASPxCheckBox ID="chkNormaZL" ClientInstanceName="chkNormaZL" runat="server" Text="cu norma zile lucratoare" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSD" ClientInstanceName="chkNormaSD" runat="server" Text="cu norma sambata si duminica" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkNormaSL" ClientInstanceName="chkNormaSL" runat="server" Text="cu norma sarbatori legale" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkCCCu" ClientInstanceName="chkCCCu" runat="server" Text="pe centrii de cost cu norma" TextAlign="Right" Visible="false" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

</asp:Content>
