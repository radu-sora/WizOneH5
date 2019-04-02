<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajDetaliat.aspx.cs" Inherits="WizOne.Pontaj.PontajDetaliat" %>
<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script type="text/javascript">


        var lastPro = null;
        function OnProiectChanged(cmbPro) {
            cmbPro.PerformCallback();
            //if (grCC.GetEditor("IdSubproiect").InCallback())
            //    lastPro = cmbPro.GetValue().toString();
            //else
            //    grCC.GetEditor("IdSubproiect").PerformCallback(cmbPro.GetValue().toString());
        }


        function OnEndCallback(s, e) {
            //if (lastPro) {
            //    grCC.GetEditor("IdSubproiect").PerformCallback(lastPro);
            //    lastPro = null;
            //}
        }




        function OnBatchEditStartEditing(s, e)
        {
            var col = e.focusedColumn.fieldName;

            if (col.length >= 6 && col.substr(0,6) == 'ValTmp')
            {
                var keyIndex = s.GetColumnByField("Cheia").index;
                var key = e.rowValues[keyIndex].value;

                if (typeof s.cp_cellsToDisable[key] != "undefined" && s.cp_cellsToDisable[key].indexOf(col.replace('Tmp','') + ";") >= 0) e.cancel = true;
            }
        }

        function OnBatchEditEndEditing(s, e) {
            var col = s.batchEditApi.GetEditCellInfo().column.fieldName;

            if (col.length >= 4)
            {
                switch (col.substr(0, 6))
                {
                    case "ValTmp":
                        {
                            var column = s.batchEditApi.GetEditCellInfo().column;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal)
                            {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValAbs", null, null, true);
                                s.GetRowValues(e.visibleIndex, "Afisare", function (Value)
                                {
                                    OnEditMode(e, e.visibleIndex, Value);
                                });
                            }
                        }
                        break;
                    case "ValAbs":
                        {
                            var column = grDate.GetColumnByField("ValAbs");
                            if (!e.rowValues[column.index]) return;
                            var val = e.rowValues[column.index].value;

                            grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValStr", val, null, true);

                            for (i = 0; i <= 20; i++)
                            {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValTmp" + i, null, null, true);
                            }
                        }
                        break;
                }
            }

        }

        function OnEditMode(e, idx, Value)
        {
            var valStr = "";

            for (i = 0; i <= 20; i++) {
                var val = 0;
                var valOre = 0;

                var column = grDate.GetColumnByField("ValTmp" + i);
                if (!e.rowValues[column.index]) continue;
                val = e.rowValues[column.index].value;

                if (val)
                {
                    var mm = val.getMinutes();
                    var hh = val.getHours();

                    if (mm != 0 || hh != 0) {
                        switch (Value) {
                            case 1:
                                valOre = hh
                                //valOre = Math.floor(val / 60);
                                break;
                            case 2:
                                valOre = hh + ':' + ("00" + mm).slice(-2);
                                //valOre = Math.floor(val / 60) + '.' + ("00" + (val % 60)).slice(-2);
                                break;
                            case 3:
                                valOre = hh + '.' + ("00" + Math.round((mm / 60) * 100)).slice(-2);;
                                //valOre = Math.round((val / 60) * 100) / 100;
                                break;
                            default:
                                valOre = hh
                                break;
                        }

                        valStr += "/" + valOre + column.name;
                    }
                }
            }

            if (valStr.length > 0) valStr = valStr.substring(1);

            grDate.batchEditApi.SetCellValue(idx, "ValStr", valStr, null, true);
        }



        //function OnRecalcParam(s, e) {
        //    if (txtDataInc.GetText() == '' || txtDataSf.GetText() == '' || txtMarcaInc.GetText() == '' || txtMarcaSf.GetText() == '') {
        //        swal({
        //            title: "Data insuficiente", text: "Lipsesc date pentru recalcul",
        //            type: "warning"
        //        });
        //    }
        //    else {
        //        popUpRecalc.Hide();
        //        grDate.PerformCallback('btnRecalcParam;' + txtDataInc.GetText() + ";" + txtDataSf.GetText() + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
        //    }
        //}



        function OnRecalcParam(s, e) {
            if (txtDataInc.GetText() == '' || txtDataSf.GetText() == '' || txtMarcaInc.GetText() == '' || txtMarcaSf.GetText() == '') {
                swal({
                    title: "Data insuficiente", text: "Lipsesc date pentru recalcul",
                    type: "warning"
                });
            }
            else {
                popUpRecalc.Hide();

                var jsDateInc = txtDataInc.GetDate();
                var yearInc = jsDateInc.getFullYear();
                var monthInc = jsDateInc.getMonth() + 1;
                var dayInc = jsDateInc.getDate();

                var jsDateSf = txtDataSf.GetDate();
                var yearSf = jsDateSf.getFullYear();
                var monthSf = jsDateSf.getMonth() + 1;
                var daySf = jsDateSf.getDate();

                //grDate.PerformCallback("btnRecalcParam;" + txtDataInc.GetText() + ";" + txtDataSf.GetText() + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
                grDate.PerformCallback("btnRecalcParam;" + dayInc + "/" + monthInc + "/" + yearInc + ";" + daySf + "/" + monthSf + "/" + yearSf + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
            }
        }



        function eventKeyPress(evt, s)
        {
            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();

            if (!s.IsEditing())
            {
                var cell = grDate.GetFocusedCell();
                var col = cell.column.fieldName;
                if (col.length >= 3)
                {
                    switch(col.substr(0,3))
                    {
                        case "In1":
                        case "In2":
                        case "In3":
                        case "In4":
                        case "In5":
                        case "In6":
                        case "In7":
                        case "In8":
                        case "In9":
                        case "Out":
                            {
                                if (key == 45)              // scade o zi
                                {
                                    grDate.PerformCallback('dayMinus;' + cell.column.fieldName);
                                }
                                else if (key == 43)        // adauga o zi
                                {
                                    grDate.PerformCallback('dayPlus;' + cell.column.fieldName);
                                }
                                else if (key == 93)         ////insereaza celula   tasta   ]
                                {
                                    grDate.PerformCallback('cellPlus;' + cell.column.fieldName);
                                }
                                else if (key == 91)         // sterge celula pe care este, daca este goala tasta [
                                {
                                    grDate.PerformCallback('cellMinus;' + cell.column.fieldName);
                                }
                            }
                    }
                }                
            }
        }

        function EmptyFields(s, e) {
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

        function OnPtjEchipa(s, e)
        {
            if (grDate.batchEditApi.HasChanges())
            {
                swal({
                    title: "Atentie !", text: "Aveti date nesalvate.",
                    type: "warning"
                });
                e.processOnServer = false;
            }
            else
            {
                e.processOnServer = true;
            }
        }

        function grCC_OnNewClick(s, e) {
            grCC.AddNewRow();
        }

        function grCC_OnCancelClick(s, e) {
            grCC.CancelEdit();
        }

        function OnClickCC(s,e)
        {
            if (grDate.GetFocusedRowIndex() != -1)
            {
                //pnlLoading.Show();
                grCC.PerformCallback('btnCC');
            }
            else
            {
                swal({
                    title: "Atentie !", text: "Nu exista linie selectata",
                    type: "warning"
                });
            }
        }

        //function OnDateModif(s, e)
        //{
        //    var de = grCC.GetEditValue("De");
        //    var la = grCC.GetEditValue("La");
            
        //    var difMin = Math.round(Math.abs((de.getTime() - la.getTime()) / (60 * 1000)))
        //    //var dif = new Date(la.getFullYear(), la.getMonth(), la.getDay(), difMin / 60, difMin % 60);

        //    grCC.SetEditValue("NrOre", difMin);
        //    //grCC.SetEditValue("NrOreUnbound", dif);
        //}

        //function OnNrOre(s, e)
        //{
        //    var ore = grCC.GetEditValue("NrOreUnbound");
        //    var mm = ore.getMinutes();
        //    var hh = ore.getHours();

        //    grCC.SetEditValue("NrOre", ((hh * 60) + mm));
        //}


    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" Visible="false" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnCC" ClientInstanceName="btnCC" ClientIDMode="Static" runat="server" Text="Centru de cost" AutoPostBack="false" oncontextMenu="ctx(this,event)" Visible="false" >
                    <ClientSideEvents Click="function(s, e) { OnClickCC(s,e); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/view.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPtjEchipa" ClientInstanceName="btnPtjEchipa" ClientIDMode="Static" runat="server" Text="Pontajul Echipei" AutoPostBack="false" OnClick="btnPtjEchipa_Click" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) { OnPtjEchipa(s,e); }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/chooser.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="true" OnClick="btnRespins_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="true" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        grDate.PerformCallback('btnInit');
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/initializare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnDelete" ClientInstanceName="btnDelete" ClientIDMode="Static" runat="server" Text="Sterge" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        grDate.PerformCallback('btnDelete');
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/sterge.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRecalc" ClientInstanceName="btnRecalc" ClientIDMode="Static" runat="server" Text="Recalculeaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                       popUpRecalc.Show();
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespinge" ClientInstanceName="btnRenunta" ClientIDMode="Static" runat="server" Text="Renunta" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        grDate.CancelEdit();
                    }" />
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
                        pnlLoading.Show();
                        grDate.UpdateEdit();
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


                            <div id="divPeAng" runat="server" style="float:left; display:none; line-height:22px; vertical-align:middle;">
                    
                                <div style="float:left; padding-right:15px;">
                                    <label id="lblAnLuna" runat="server" style="display:inline-block; float:left; padding:0px 15px;">Luna/An</label>
                                     <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" EditFormatString="MM/yyyy" EditFormat="Custom" />
                                </div>
                    
                                <div style="float:left; padding-right:15px; vertical-align:middle; display:inline-block;">
                                    <label id="lblRolAng" runat="server" style="float:left; padding-right:15px;">Roluri</label>
                                    <dx:ASPxComboBox ID="cmbRolAng" ClientInstanceName="cmbRolAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false" />
                                </div>
                    
                                <div style="float:left; padding-right:15px;">
                                    <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajat</label>
                                    <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
                                                CallbackPageSize="15" EnableCallbackMode="true" TextFormatString="{0} {1}" OnButtonClick="cmbAng_ButtonClick" >
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="F10003" Caption="Marca" Width="130px" />
                                            <dx:ListBoxColumn FieldName="NumeComplet" Caption="Angajat" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                            <dx:ListBoxColumn FieldName="Departament" Caption="Dept" Width="130px" />
                                        </Columns>
                                        <Buttons>
                                            <dx:EditButton Position="Left">
                                                <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                            </dx:EditButton>
                                            <dx:EditButton Position="Right">
                                                <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                            </dx:EditButton>
                                        </Buttons>
                                        <ClientSideEvents ButtonClick="function(s, e) {
                                                                    pnlLoading.Show();
                                                                    e.processOnServer = true;
                                                                }" />
                                    </dx:ASPxComboBox>
                                </div>

                                <label ID="txtStare" runat="server" Width="110" style="float:left; margin-right:15px; width:110px; height:26px; text-align:center; border:solid 1px gray; color:#000000;"></label>
                            </div>

                            <div id="divPeZi" runat="server" style="float:left; display:none; line-height:22px; vertical-align:middle; padding:5px 0px 15px 0px;">
                                <table style="margin-left:15px;">
                                    <tr>
                                        <td>
                                            <div style="float:left; padding-right:35px; padding-bottom:10px;">
                                                <label id="lblZiua" runat="server" style="display:inline-block; float:left; padding-right:25px; width:80px;">Data</label>
                                                <dx:ASPxDateEdit ID="txtZiua" runat="server" Width="130px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" OnButtonClick="txtZiua_ButtonClick">
                                                    <Buttons>
                                                        <dx:EditButton Position="Left">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                        <dx:EditButton Position="Right">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s, e) {
                                                                    pnlLoading.Show();
                                                                    e.processOnServer = true;
                                                                }" />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblRolZi" runat="server" style="display:inline-block; float:left; padding-right:15px; padding-left:21px; width:80px;">Roluri</label>
                                                <dx:ASPxComboBox ID="cmbRolZi" ClientInstanceName="cmbRolZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdRol" TextField="RolDenumire" ValueType="System.Int32" AutoPostBack="false"  />
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblAngZi" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                                <dx:ASPxComboBox ID="cmbAngZi" ClientInstanceName="cmbAngZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false"
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
                                            <div style="float:left; padding-right:15px;">
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
                            </div>


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
                    <dx:ASPxButton ID="btnFiltruSterge" runat="server" Text="Sterge Filtru" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                        <Image Url="~/Fisiere/Imagini/Icoane/lupaDel.png"></Image>
                        <ClientSideEvents Click="EmptyFields" />
                    </dx:ASPxButton>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlRowPrepared="grDate_HtmlRowPrepared" OnBatchUpdate="grDate_BatchUpdate" OnDataBound="grDate_DataBound" OnCellEditorInitialize="grDate_CellEditorInitialize" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" BatchEditEndEditing="OnBatchEditEndEditing" BatchEditStartEditing="OnBatchEditStartEditing"   />
                    <Styles>
                        <BatchEditModifiedCell BackColor="Transparent">
                        </BatchEditModifiedCell>
                    </Styles>
                    <Columns>
                        
                        <dx:GridViewDataTextColumn FieldName="Cheia" Caption=" " ReadOnly="true" Visible="true" ShowInCustomizationForm="true" FixedStyle="Left" VisibleIndex="0" />
                        <dx:GridViewDataTextColumn FieldName="NumeComplet" Name="Angajat" Caption="Angajat" ReadOnly="true" Width="150px" VisibleIndex="3" Visible="false" ShowInCustomizationForm="false" PropertiesTextEdit-ClientSideEvents-ValueChanged="" />

                        <dx:GridViewDataTextColumn FieldName="ZiLibera" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiLiberaLegala" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ZiSapt" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                        <dx:GridViewDataTextColumn FieldName="F10022" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="F10023" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="IdStare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="Afisare" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                        <dx:GridViewDataTextColumn FieldName="ValActive" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                    </Columns>
                    
                </dx:ASPxGridView>

            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br /><br />

                <table width="100%">
                    <tr>
                        <td align="right">
                            <dx:ASPxButton ID="btnNewCC" ClientInstanceName="btnNewCC" ClientIDMode="Static" runat="server" Text="Nou" AutoPostBack="False" oncontextMenu="ctx(this,event)" >
                                <ClientSideEvents Click="function (s, e) { grCC_OnNewClick(s, e); }" />
                                <Image Url="~/Fisiere/Imagini/Icoane/New.png"></Image>
                            </dx:ASPxButton>
                            <dx:ASPxButton ID="btnSaveCC" ClientInstanceName="btnSaveCC" ClientIDMode="Static" runat="server" Text="Salveaza" OnClick="btnSaveCC_Click" oncontextMenu="ctx(this,event)">
                                <ClientSideEvents Click="function(s, e) {
                                pnlLoading.Show();
                                e.processOnServer = true;
                                }" />
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                            </dx:ASPxButton>
                        </td>
                    </tr>
                    <tr>
                        <td>

                            <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="false" OnRowInserting="grCC_RowInserting" 
                                OnRowUpdating="grCC_RowUpdating" OnInitNewRow="grCC_InitNewRow" OnCustomCallback="grCC_CustomCallback" OnRowDeleting="grCC_RowDeleting" OnAfterPerformCallback="grCC_AfterPerformCallback" OnHtmlEditFormCreated="grCC_HtmlEditFormCreated"
                                OnRowValidating="grCC_RowValidating" OnCellEditorInitialize="grCC_CellEditorInitialize" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared" OnCommandButtonInitialize="grCC_CommandButtonInitialize" >
                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" />
                                <SettingsEditing Mode="EditFormAndDisplayRow" />
                                <SettingsSearchPanel Visible="false" />
                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                <ClientSideEvents ContextMenu="ctx" />

                                <Columns>
                                    <dx:GridViewCommandColumn Width="80px" ShowDeleteButton="true" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" ShowInCustomizationForm="false" Width="150px" VisibleIndex="1" />

                                    <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="2" Visible="false" />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="3" Visible="false" />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdSubproiect" Name="IdSubproiect" Caption="SubProiect" Width="250px" VisibleIndex="4" Visible="false" />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="5" Visible="false" />

                                    <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="6" Visible="false"/>

                                    <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="7" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                            
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>

                                    <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="8" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                            
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre1" Name="NrOre1" Caption="NrOre1" Width="100px" Visible="false" VisibleIndex="9" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre2" Name="NrOre2" Caption="NrOre2" Width="100px" Visible="false" VisibleIndex="10" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre3" Name="NrOre3" Caption="NrOre3" Width="100px" Visible="false" VisibleIndex="11" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre4" Name="NrOre4" Caption="NrOre4" Width="100px" Visible="false" VisibleIndex="12" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre5" Name="NrOre5" Caption="NrOre5" Width="100px" Visible="false" VisibleIndex="13" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre6" Name="NrOre6" Caption="NrOre6" Width="100px" Visible="false" VisibleIndex="14" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre7" Name="NrOre7" Caption="NrOre7" Width="100px" Visible="false" VisibleIndex="15" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre8" Name="NrOre8" Caption="NrOre8" Width="100px" Visible="false" VisibleIndex="16" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre9" Name="NrOre9" Caption="NrOre9" Width="100px" Visible="false" VisibleIndex="17" />
                                    <dx:GridViewDataSpinEditColumn FieldName="NrOre10" Name="NrOre10" Caption="NrOre10" Width="100px" Visible="false" VisibleIndex="18" />

                                    <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                                </Columns>
                                
                                <SettingsCommandButton>
                                    <UpdateButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png" AlternateText="Save" ToolTip="Actualizeaza" />
                                        <Styles>
                                            <Style Paddings-PaddingRight="5px" />
                                        </Styles>
                                    </UpdateButton>
                                    <CancelButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/renunta.png" AlternateText="Renunta" ToolTip="Renunta" />
                                    </CancelButton>

                                    <EditButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/edit.png" AlternateText="Edit" ToolTip="Edit" />
                                        <Styles>
                                            <Style Paddings-PaddingRight="5px" />
                                        </Styles>
                                    </EditButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>
                                </SettingsCommandButton>


                                <Templates>
                                    
                                    <EditForm>
                                        <div style="padding: 4px 3px 4px">
                                            <table>
                                                <tr>
                                                    <td>Stare</td>
                                                    <td>CC</td>
                                                    <td>Proiect</td>
                                                    <td>SubProiect</td>
                                                    <td>Activitate</td>
                                                    <td>Dept</td>
                                                    <td>De</td>
                                                    <td>La</td>
                                                    <td>NrOre1</td>
                                                    <td>NrOre2</td>
                                                    <td>NrOre3</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <dx:ASPxComboBox ID="cmbStr" runat="server" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"/>
                                                        <dx:ASPxComboBox ID="cmbF062" runat="server" ValueField="F06204" TextField="F06205" ValueType="System.Int32" AutoPostBack="false"/>
                                                        <dx:ASPxComboBox ID="cmbPro" runat="server" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" OnCallback="cmbPro_Callback">
                                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProiectChanged(s); }"></ClientSideEvents>
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxComboBox ID="cmbSub" runat="server" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false">
                                                            <ClientSideEvents EndCallback="OnEndCallback"/>
                                                        </dx:ASPxComboBox>
                                                        <dx:ASPxComboBox ID="cmbAct" runat="server" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false"/>
                                                        <dx:ASPxComboBox ID="cmbDpt" runat="server" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false"/>
                                                    </td>
                                                </tr>


                                            </table>
                                        </div>
                                    </EditForm>
                                </Templates>



                            </dx:ASPxGridView>

                        </td>
                    </tr>
                </table>


            </td>
        </tr>
    </table>
    
    <dx:ASPxPopupControl ID="popUpRecalc" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpRecalcArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="450px" Height="200px" HeaderText="Parametrii recalcul"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpRecalc" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel1" runat="server">
                    <table>
                        <tr>
                            <td align="right" colspan="4">
                                <dx:ASPxButton ID="btnRecalcParam" runat="server" Text="Recalcul" AutoPostBack="false" >
                                    <ClientSideEvents Click="function(s, e) {
                                        OnRecalcParam(s,e);
                                    }" />
                                    <Image Url="~/Fisiere/Imagini/Icoane/calcul.png"></Image>
                                </dx:ASPxButton>
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td  style="padding:15px;">
                               <dx:ASPxLabel ID="lblDataINc" runat="server" Text="Data Inceput"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" />
                            </td>
                            <td style="padding:15px;">
                               <dx:ASPxLabel ID="lblDataSf" runat="server" Text="Data Sfarsit"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                               <dx:ASPxLabel ID="lblMarcaIn" runat="server" Text="Marca Inceput"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtMarcaInc" runat="server" Width="100px" />
                            </td>
                            <td>
                               <dx:ASPxLabel ID="lblMarcaSf" runat="server" Text="Marca Sfarsit"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxSpinEdit ID="txtMarcaSf" runat="server" Width="100px" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
        
    

</asp:Content>
