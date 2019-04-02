<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajEchipa.aspx.cs" Inherits="WizOne.Pontaj.PontajEchipa" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<%@ Register assembly="DevExpress.Web.ASPxPivotGrid.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web.ASPxPivotGrid" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">


        function eventKeyPress(evt, s) {
            var cell = grDate.GetFocusedCell();
            if (!cell) return;
            var col = cell.column.fieldName;
            var f10003 = grDate.GetRowKey(cell.rowVisibleIndex);
            
            txtCol.Set('coloana', col);
            txtCol.Set('f10003', f10003);

            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();

            var cell = grDate.GetFocusedCell();
            var col = cell.column.fieldName;
            if (col.length >= 4 && col.substr(0, 4) == 'Ziua') {
                if (key == 43)              //tasta plus  
                {
                    if (typeof grDate.cp_ZileLucrate[f10003] != "undefined" && grDate.cp_ZileLucrate[f10003] != null && grDate.cp_ZileLucrate[f10003].indexOf(',' + col) >= 0) {
                        pnlLoading.Show();
                        grDate.GetRowValues(grDate.GetFocusedRowIndex(), 'IdStare', OnGetRowValues);
                    }
                    else {
                        swal({
                            title: "Atentie !", text: "Clientul este inactiv",
                            type: "warning"
                        });
                    }
                }
            }
        }

        function OnGetRowValues(Value) {
            
            if ((cmbRol.GetValue() == 0 && (Value == 1 || Value == 4)) ||
                (cmbRol.GetValue() == 1 && (Value == 1 || Value == 4)) ||
                (cmbRol.GetValue() == 2 && (Value == 1 || Value == 2 || Value == 4 || Value == 6)) ||
                (cmbRol.GetValue() == 3))
            {
                popUpModif.Show();
                popUpModif.PerformCallback();
            }

            pnlLoading.Hide();

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

            pnlCtl.PerformCallback('EmptyFields');
        }

        function OnClickDetaliat(s,e)
        {
            var edc = txtCol.Get('coloana');
            var edc1 = txtCol.Get('f10003');
            

            if (txtCol.Get('coloana') || txtCol.Get('f10003'))
            {
                var colSel = txtCol.Get('coloana');
                if (colSel.length >= 4 && colSel.substr(0, 4).toLowerCase() == 'ziua') {
                    pnlLoading.Show();

                    var idxPag = grDate.GetPageIndex();
                    var idxRow = grDate.GetFocusedRowIndex();
                    grDate.PerformCallback(s.name + ";" + txtCol.Get('f10003') + ";" + colSel + ";" + idxPag + ";" + idxRow);
                }
                else
                {
                    swal({
                        title: "Atentie !", text: "Trebuie sa selectati o coloana care afiseaza ziua",
                        type: "warning"
                    });
                    e.processOnServer = false;
                }
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

        //function OnInit(s, e) 
        //{
        //    alert('aaa');
        //    popUpInit.Hide();
        //    pnlLoading.Show();
        //    e.processOnServer = true;
        //}

        function OnModif(s, e) {
            var texts = "";
            $('#<% =pnlValuri.ID %> input[type="text"]').each(function () {
                texts += ";" + $(this).attr('id') + "=" + $(this).val();
            });

            txtCol.Set('valuri', texts);
            popUpModif.Hide();
            pnlLoading.Show();
            e.processOnServer = true;
        }

        function EmptyVal(s, e) {
            $('#<% =pnlValuri.ID %> input[type="text"]').val('');
        }

        function EmptyCmbAbs(s, e) {
            cmbTipAbs.SetValue(null);
        }

        function OnInit(s, e) {
            AdjustSize();
            //document.getElementById("gridContainer").style.visibility = "";
        }
        function OnEndCallback(s, e) {
            AdjustSize();
        }
        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            var height = Math.max(0, document.documentElement.clientHeight) - 330;
            grDate.SetHeight(height);
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
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="true" OnClick="btnRespins_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                    <ClientSideEvents Click="function (s,e) { 
                        pnlLoading.Show();
                        e.processOnServer = true;
                     }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="true" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                    <ClientSideEvents Click="function (s,e) { 
                        pnlLoading.Show();
                        e.processOnServer = true;
                     }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpInit.Show(); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnStergePontari" ClientInstanceName="btnStergePontari" ClientIDMode="Static" runat="server" Text="Sterge Pontari" AutoPostBack="true" OnClick="btnStergePontari_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnTransfera" ClientInstanceName="btnTransfera" ClientIDMode="Static" runat="server" Text="Transfera" AutoPostBack="true" OnClick="btnTransfera_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/duplicare.png"></Image>
                </dx:ASPxButton>                
                <dx:ASPxButton ID="btnPeAng" ClientInstanceName="btnPeAng" ClientIDMode="Static" runat="server" Text="Pontaj pe Angajat" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/n2.png"></Image>
                    <ClientSideEvents Click="function (s,e) { OnClickDetaliat(s,e); }" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPeZi" ClientInstanceName="btnPeZi" ClientIDMode="Static" runat="server" Text="Pontaj pe Zi" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
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
                                             <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" oncontextMenu="ctx(this,event)" >
                                                 <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtAnLuna'); }" />
                                                 <CalendarProperties FirstDayOfWeek="Monday" />
                                            </dx:ASPxDateEdit>
                                        </div>
                                        <div style="float:left; padding-right:15px; vertical-align:middle; display:inline-block;">
                                            <label id="lblRol" runat="server" style="float:left; padding-right:15px; width:80px;">Roluri</label>
                                            <dx:ASPxComboBox ID="cmbRol" ClientInstanceName="cmbRol" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                                                <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRol'); }" />
                                            </dx:ASPxComboBox>
                                        </div>
                                        <div style="float:left; padding-right:15px;">
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
                                            <label id="lblStare" runat="server" style="display:inline-block; float:left; padding-right:25px; width:80px;">Stare</label>
                                            <dx:ASPxComboBox ID="cmbStare" ClientInstanceName="cmbStare" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"/>
                                        </div>
                                    </td>
                                </tr>                                
                                <tr>
                                    <td>
                                        <div style="float:left; padding-right:15px;padding-bottom:10px;">
                                            <label id="lblCtr" runat="server" style="display:inline-block; float:left; padding-right:25px; width:80px;">Contract</label>
                                            <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
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
                                            <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
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

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnCustomCallback="grDate_CustomCallback" OnDataBound="grDate_DataBound" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="True" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" />
                    <ClientSideEvents ContextMenu="ctx" RowDblClick="function(s, e) { OnClickDetaliat(s, e); }" Init="OnInit" EndCallback="OnEndCallback" />
                    <Columns>
                        
                        <dx:GridViewCommandColumn Width="30px" VisibleIndex="0" ButtonType="Image" Caption=" " ShowSelectCheckbox="true" FixedStyle="Left" SelectAllCheckboxMode="AllPages" />

                        <dx:GridViewDataTextColumn FieldName="IdStare" Name="IdStare" Caption="Id Stare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataComboBoxColumn FieldName="StareDenumire" Name="StareDenumire" Caption="Stare" ReadOnly="true" Width="100px" FixedStyle="Left" VisibleIndex="1" CellStyle-HorizontalAlign="Center" />

                        <dx:GridViewDataTextColumn FieldName="F10003" Caption="Marca" ReadOnly="true" ShowInCustomizationForm="false" FixedStyle="Left" VisibleIndex="2" Settings-AutoFilterCondition="Contains" />
                        <dx:GridViewDataTextColumn FieldName="AngajatNume" Caption="Angajat" ReadOnly="true" ShowInCustomizationForm="false"  FixedStyle="Left" VisibleIndex="3" Width="150px" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="Norma" Caption="Norma" ReadOnly="true" ShowInCustomizationForm="false" FixedStyle="Left" VisibleIndex="3" Width="80px"/>
                        <dx:GridViewDataTextColumn FieldName="DescContract" Caption="Contract" ReadOnly="true" ShowInCustomizationForm="false" FixedStyle="Left" VisibleIndex="3" Width="150px" Settings-AutoFilterCondition="Contains"/>
                        <dx:GridViewDataTextColumn FieldName="F06205" Caption="Centrul de Cost" ReadOnly="true" ShowInCustomizationForm="false" FixedStyle="Left" VisibleIndex="3" Width="150px" Settings-AutoFilterCondition="Contains"/>
                          
                        <dx:GridViewDataTextColumn FieldName="Companie" Caption="Companie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Subcompanie" Caption="Subcompanie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Filiala" Caption="Filiala" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Sectie" Caption="Sectie" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Dept" Caption="Dept" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Subdept" Caption="Subdept" ReadOnly="true" Visible="false" />
                        <dx:GridViewDataTextColumn FieldName="Birou" Caption="Birou" ReadOnly="true" Visible="false" />
                                            
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
                        


                    </Columns>
                    
                </dx:ASPxGridView>

                <br />
    
            </td>
        </tr>
    </table>



    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Parametrii initializare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="true" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        popUpInit.Hide();
                                        pnlLoading.Show();
                                        e.processOnServer = true;
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
                                <dx:ASPxCheckBox ID="chkCCCu" ClientInstanceName="chkCCCu" runat="server" Text="cu norma pe centrii de cost" TextAlign="Right" />
                                <br />
                                <dx:ASPxCheckBox ID="chkInOut" ClientInstanceName="chkInOut" runat="server" Text="cu In-Out din contract" TextAlign="Right" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>


 

    <dx:ASPxPopupControl ID="popUpModif" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpModifArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="550px" Height="200px" HeaderText="Modificare pontaj"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpModif" EnableHierarchyRecreation="false" OnWindowCallback="popUpModif_WindowCallback">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <div class="row">
                        <div class="col-sm-12">
                            <div style="display:inline-table; float:right;">
                                <dx:ASPxButton ID="btnModif" runat="server" Text="Salveaza" AutoPostBack="false" OnClick="btnModif_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        e.processOnServer = false;
                                        OnModif(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <span style="font-weight:bold; font-size:14px;">Absente de tip zi</span>
                                <br />
                                <br />
                            </div>
                        </div>
                        <div class="row" style="text-align:center;">
                            <div class="col-md-12">
                                <div style="display:inline-table;">
                                    <dx:ASPxComboBox ID="cmbTipAbs" runat="server" ClientIDMode="Static" ClientInstanceName="cmbTipAbs" Width="200px" DropDownWidth="350px" ValueField="Id" TextField="DenumireScurta" AutoPostBack="false" TextFormatString="{0}">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="DenumireScurta" Caption="Id" Width="50" />
                                            <dx:ListBoxColumn FieldName="Denumire" Caption="Denumire" Width="200" />
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) {
                                                    e.processOnServer = false;
                                                    EmptyVal(s,e);
                                                }" />
                                    </dx:ASPxComboBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <br /><br /><br />
                                <span style="font-weight:bold; font-size:14px;">Absente de tip ora</span>
                            </div>
                        </div>
                        <div class="row" id="pnlValuri" runat="server" style="margin:20px 50px 50px 50px;">
                        </div>
                    </div>
                    <dx:ASPxHiddenField ID="txtValuri" runat="server"></dx:ASPxHiddenField>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>
