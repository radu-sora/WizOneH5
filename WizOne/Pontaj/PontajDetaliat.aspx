<%@ Page Title="" Language="C#" MasterPageFile="~/Cadru.Master" AutoEventWireup="true" CodeBehind="PontajDetaliat.aspx.cs" Inherits="WizOne.Pontaj.PontajDetaliat" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script>

        function grDate_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnGoToCC":
                    var cheie= s.GetRowKey(s.GetFocusedRowIndex());
                    lblZiuaCC.SetText('Centrii de cost - Ziua ' + cheie);
                    ccValori.Set('cheia', cheie);
                    grCC.PerformCallback('btnCC;' + cheie);
                    break;
            }
        }

        function grCC_CustomButtonClick(s, e) {
            switch (e.buttonID) {
                case "btnDeleteCC":
                    grCC.PerformCallback('btnDeleteCC;' + s.GetRowKey(e.visibleIndex));
                    break;
            }
        }
        

        //var grIsEditing = false;

        var oldRowIndex = -1;

        function grid_FocusedRowChanged(s, e) {
            
            //if (grIsEditing)
            //    s.UpdateEdit();

            //grIsEditing = false;
        }

        var valPro = null;
        var lastPro = null;
        var lastSub = null;
        var rowIndex = -1;

        function OnProiectChanged(cmbPro) {
            if (grCC.GetEditor("IdSubproiect").InCallback()) {
                lastPro = cmbPro.GetValue().toString();
                valPro = cmbPro.GetValue().toString();
            }
            else
                grCC.GetEditor("IdSubproiect").PerformCallback(cmbPro.GetValue().toString());
        }


        function OnSubEndCallback(s, e) {
            if (lastPro) {
                grCC.GetEditor("IdSubproiect").PerformCallback(lastPro);
                lastPro = null;
            }
        }

        function OnSubproiectChanged(cmbSub) {
            if (grCC.GetEditor("IdActivitate").InCallback())
                lastSub = cmbSub.GetValue().toString();
            else
                grCC.GetEditor("IdActivitate").PerformCallback(cmbSub.GetValue().toString());
        }

        function OnActEndCallback(s, e) {
            if (lastSub) {
                grCC.GetEditor("IdActivitate").PerformCallback(lastSub);
                lastSub = null;
            }
        }

        function OnBatchEditStartEditing(s, e) {
            //alert(s.batchEditApi.HasChanges());
           
            var keyIndex = s.GetColumnByField("Cheia").index;
            var key = e.rowValues[keyIndex].value;

            //alert(oldRowIndex);
            //alert(key);

            if (s.batchEditApi.HasChanges() && oldRowIndex != key)
                s.UpdateEdit();

            oldRowIndex = key;

            //Florin 2019.07.19 Begin
            var tip = getQueryVariable("tip");
            var dtCurr = new Date(2200, 12, 31);
            if (tip == 1) {
                var luna = txtAnLuna.GetValue();
                dtCurr = new Date(luna.getFullYear(), luna.getMonth(), key);
            }
            else {
                var luna = txtZiua.GetValue();
                dtCurr = new Date(luna.getFullYear(), luna.getMonth(), luna.getDate());
            }

            var time = <%= Session["Ptj_DataBlocare"] %>;
            var dtBlocare = new Date(Number(time.toString().substring(0, 4)), Number(time.toString().substring(4, 6)) - 1, Number(time.toString().substring(6)));

            if (dtBlocare >= dtCurr)
                e.cancel = true;
            //Florin 2019.07.19 End

            if (typeof s.cp_cellsDrepturi[key] != "undefined" && s.cp_cellsDrepturi[key] != null && s.cp_cellsDrepturi[key] == 0) {
                e.cancel = true;
            }

            if (typeof s.cp_PoateModifica[key] != "undefined" && s.cp_PoateModifica[key] != null) {
                switch(s.cp_PoateModifica[key])
                {
                    case -33:
                        e.cancel = true;
                        swal({
                            title: "Atentie", text: "Rolul dumneavoastra nu permite stergerea",
                            type: "warning"
                        });
                        break;
                    case 1:
                        //NOP
                        break;
                    case 2:
                        swal({
                            title: "Atentie", text: "Pontare venita din cereri",
                            type: "warning"
                        });
                        break;
                    case 3:
                        e.cancel = true;
                        swal({
                            title: "Atentie", text: "Nu este permisa stergerea pontarilor venite din cereri",
                            type: "warning"
                        });
                        break;
                }
            }

            var col = e.focusedColumn.fieldName;

            if (col.length >= 6 && col.substr(0, 6) == 'ValTmp') {

                if (typeof s.cp_cellsToDisable[key] != "undefined" && s.cp_cellsToDisable[key] != null && s.cp_cellsToDisable[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
                    var ert = '';
                }
                else
                    e.cancel = true;

                if (typeof s.cp_ValSec[key] != "undefined" && s.cp_ValSec[key] != null && s.cp_ValSec[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
                    e.cancel = true;
                }
            }            
            if (col.length >= 6 && col.substr(0, 6) == 'ValAbs')
            {
                var cmb = grDate.GetEditor('ValAbs');
                if (cmb) {
                    cmb.ClearItems();
                    if (typeof s.cp_ValAbsente[key] != "undefined") {
                        var str = s.cp_ValAbsente[key];
                        if (str) {
                            var arr = str.substring(1).split(",");
                            for (var i = 0; i < arr.length; i++) {
                                if (arr[i] != "undefined" && arr[i] != "")
                                {
                                    var denum = arr[i].split("=");
                                    if (denum.length >= 1) {
                                        if (denum.length == 1)
                                            cmb.AddItem('', denum[0]);
                                        else
                                        {
                                            var fullDen = new Array(denum[0], denum[1]);
                                            cmb.AddItem(fullDen);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            DamiPrograme(e);
            
        }

        function OnBatchEditEndEditing(s, e) {
            //grIsEditing = true;

            if (!s.batchEditApi.GetEditCellInfo().column) return;

            var col = s.batchEditApi.GetEditCellInfo().column.fieldName;
            var arr = "In1,In2,In3,In4,In5,In6,In7,In8,In9,In10,In11,In12,In13,In14,In15,In16,In17,In18,In19,In20,Out1,Out2,Out3,Out4,Out5,Out6,Out7,Out8,Out9,Out10,Out11,Out12,Out13,Out14,Out15,Out16,Out17,Out18,Out19,Out20,";

            if (col.length >= 4) {
                switch (col.substr(0, 6)) {
                    case "ValTmp":
                        {
                            var column = s.batchEditApi.GetEditCellInfo().column;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValAbs", null, null, true);

                                var keyIndex = s.GetColumnByField("Cheia").index;
                                var key = e.rowValues[keyIndex].value;
                                var idAfisare = 1;
                                if (typeof s.cp_Afisare[key] != "undefined" && s.cp_Afisare[key] != null) {
                                    idAfisare =  s.cp_Afisare[key]
                                }
                                OnEditMode(e, e.visibleIndex, idAfisare);
                                //s.GetRowValues(e.visibleIndex, "Afisare", function (Value) {
                                //    OnEditMode(e, e.visibleIndex, Value);
                                //});
                            }
                        }
                        break;
                    case "ValAbs":
                        {                           
                            var column = grDate.GetColumnByField("ValAbs");
                            if (!e.rowValues[column.index]) return;

                            var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                            var newVal = e.rowValues[column.index].value;

                            if (oldVal != newVal) {
                                grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValStr", newVal, null, true);

                                for (i = 0; i <= 20; i++) {
                                    grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValTmp" + i, null, null, true);
                                }
                            }
                        }
                        break;
                }
            }
        }

        function OnEditMode(e, idx, Value) {
            
            var valStr = "";

            for (i = 0; i <= 20; i++) {
                var val = 0;
                var valOre = 0;

                var column = grDate.GetColumnByField("ValTmp" + i);
                if (!column) continue;
                if (!e.rowValues[column.index]) continue;
                val = e.rowValues[column.index].value;

                if (val) {
                    var mm = val.getMinutes();
                    var hh = val.getHours();

                    if (mm != 0 || hh != 0) {
                        switch (Value) {
                            case 1:
                                valOre = hh
                                //valOre = Math.floor(val / 60);
                                break;
                            case 2:
                                valOre = hh + '.' + ("00" + mm).slice(-2);
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

                        var str = column.name;
                        var n = str.indexOf("_");
                        if (n >= 0)
                            valStr += "/" + valOre + column.name.substring(n+1);
                        else
                            valStr += "/" + valOre + column.name;
                    }
                }
            }

            if (valStr.length > 0) valStr = valStr.substring(1);

            grDate.batchEditApi.SetCellValue(idx, "ValStr", valStr, null, true);
        }

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

                grDate.PerformCallback("btnRecalcParam;" + dayInc + "/" + monthInc + "/" + yearInc + ";" + daySf + "/" + monthSf + "/" + yearSf + ";" + txtMarcaInc.GetText() + ";" + txtMarcaSf.GetText());
            }
        }



        function eventKeyPress(evt, s) {
            var evt = evt || event;
            var key = evt.keyCode || evt.which;
            inOutIndex = s.GetFocusedRowIndex();
            
            if (!s.IsEditing()) {
                var cell = grDate.GetFocusedCell();
                var col = cell.column.fieldName;
                if (col.length >= 3) {
                    switch (col.substr(0, 3)) {
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
                                if (key == 45)              // scade o zi   tasta -
                                {
                                    var dt = grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName);
                                    if (cell.key == dt.getDate() || cell.key == (dt.getDate() - 1)) {
                                        grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                        var dtCurr = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate() - 1, dt.getHours(), dt.getMinutes(), 0, 0);
                                        grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, dtCurr);
                                        grDate.batchEditApi.EndEdit();
                                        alert('Proces realizat cu succes');
                                    }
                                }
                                else if (key == 43)        // adauga o zi  tasta +
                                {
                                    var dt = grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName);
                                    if (cell.key == dt.getDate() || cell.key == (dt.getDate() + 1)) {
                                        grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                        var dtCurr = new Date(dt.getFullYear(), dt.getMonth(), dt.getDate() + 1, dt.getHours(), dt.getMinutes(), 0, 0);
                                        grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, dtCurr);
                                        grDate.batchEditApi.EndEdit();
                                        alert('Proces realizat cu succes');
                                    }
                                }
                                else if (key == 93)         ////insereaza celula   tasta   ]
                                {
                                    var idx = 21;
                                    var col = cell.column.fieldName;
                                    if (col.substr(0, 2).toLowerCase() == 'in' && col.length <= 4)
                                        idx = Number(col.substr(2));
                                    if (col.substr(0, 3).toLowerCase() == 'out' && col.length <= 5)
                                        idx = Number(col.substr(3));

                                    grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                    for (var i = 20; i > idx; i--)
                                    {
                                        grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + i));
                                        grDate.batchEditApi.SetCellValue(inOutIndex, "In" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + (i-1).toString()));
                                    }

                                    if (col.substr(0, 2).toLowerCase() == 'in')
                                    grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + i));

                                    grDate.batchEditApi.SetCellValue(inOutIndex, cell.column.fieldName, null);
                                    grDate.batchEditApi.EndEdit();
                                }
                                else if (key == 91)         // sterge celula pe care este, daca este goala tasta [
                                {
                                    if (grDate.batchEditApi.GetCellValue(inOutIndex, cell.column.fieldName) != null)
                                        return;

                                    var idx = 21;
                                    var col = cell.column.fieldName;
                                    if (col.substr(0, 2).toLowerCase() == 'in' && col.length <= 4)
                                        idx = Number(col.substr(2));
                                    if (col.substr(0, 3).toLowerCase() == 'out' && col.length <= 5)
                                        idx = Number(col.substr(3));

                                    grDate.batchEditApi.StartEdit(inOutIndex, cell.rowVisibleIndex);
                                    
                                    if (col.substr(0, 2).toLowerCase() == 'in')
                                        grDate.batchEditApi.SetCellValue(inOutIndex, "In" + idx, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + idx));

                                    grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + idx, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + (idx + 1).toString()));

                                    for (var i = (idx + 1); i <= 20; i++)
                                    {
                                        grDate.batchEditApi.SetCellValue(inOutIndex, "In" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "Out" + i));
                                        grDate.batchEditApi.SetCellValue(inOutIndex, "Out" + i, grDate.batchEditApi.GetCellValue(inOutIndex, "In" + (i + 1).toString()));
                                    }

                                    grDate.batchEditApi.SetCellValue(inOutIndex, "Out20", null);
                                    grDate.batchEditApi.EndEdit();
                                }
                            }
                    }
                }
            }
        }

        function EmptyFields(s, e) {
            cmbAng.SetValue(null);
            cmbAngZi.SetValue(null);
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

        function OnPtjEchipa(s, e) {
            
            if (grDate.batchEditApi.HasChanges()) {
                swal({
                    title: "", text: "Aveti date nesalvate.",
                    type: "warning"
                });
                e.processOnServer = false;
            }
            else {
                e.processOnServer = true;
            }
            
        }

        function grCC_OnNewClick(s, e) {
            grCC.AddNewRow();
        }

        function grCC_OnCancelClick(s, e) {
            grCC.CancelEdit();
        }

        var timeColumnField = "";
        function onBatchEditStartEditing(s, e) {
            
            timeColumnField = e.focusedColumn.fieldName; 

            if (timeColumnField.substring(0, 5) == "NrOre") {
                var timeColumn = s.GetColumnByField(timeColumnField);
                if (!e.rowValues.hasOwnProperty(timeColumn.index))
                    return;
                var cellInfo = e.rowValues[timeColumn.index];
                cellInfo.value = minutesToString(cellInfo.value);
            }
        }

        function onBatchEditEndEditing(s, e) {
            if (timeColumnField.substring(0, 5) == "NrOre") {
                var timeColumn = s.GetColumnByField(timeColumnField);
                if (!e.rowValues.hasOwnProperty(timeColumn.index))
                    return;
                var cellInfo = e.rowValues[timeColumn.index];
                cellInfo.value = stringToMinutes(s.GetEditValue(timeColumnField));
            }
        }

        function minutesToString(mins) {
            mins = parseInt(mins);
            var hours = Math.floor(mins / 60).toString();
            if (hours.length == 1)
                hours = "0" + hours;
            return hours + ":" + mins % 60;
        }

        function stringToMinutes(s) {
            var hours = s.split(':')[0];
            var mins = s.split(':')[1];
            return parseInt(hours) * 60 + parseInt(mins);
        }

        function OnInitPtj(s, e) {
            popUpInit.Hide();
            pnlLoading.Show();
            e.processOnServer = true;
        }

        function OnInit(s, e) {
            AdjustSize();
        }

        function OnControlsInitialized(s, e) {
            ASPxClientUtils.AttachEventToElement(window, "resize", function (evt) {
                AdjustSize();
            });
        }
        function AdjustSize() {
            var dif = 230;
            var div = document.getElementById('divPeAng');
            var style = window.getComputedStyle(div);
            if (style.display === 'none')
                dif = 340;
            var height = Math.max(0, document.documentElement.clientHeight) - dif;
            if (<%=Session["PontajulAreCC"] %> == 1) 
                var height = Math.max(0, document.documentElement.clientHeight) - 470;

            grDate.SetHeight(height);
        }

        function getQueryVariable(variable) {
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (pair[0] == variable) { return pair[1]; }
            }
            return (false);
        }

        function OnEndCallback(s, e) {
            AdjustSize();
            if (s.cpAlertMessage != null) {
                swal({
                    title: "", text: s.cpAlertMessage,
                    type: "warning"
                });
                s.cpAlertMessage = null;
            }
        }

        function cmbContract_SelectedIndexChanged_Client(s, e) {
            var idCtr = s.GetSelectedItem().value;
            LoadPrograme(idCtr);
        }

        function DamiPrograme(e) {
            if (typeof cmbProgram !== "undefined" && ASPxClientUtils.IsExists(cmbProgram)) {
                currentEditableVisibleIndex = e.visibleIndex;
                var idCtr = grDate.batchEditApi.GetCellValue(currentEditableVisibleIndex, "IdContract");
                var idPrg = grDate.batchEditApi.GetCellValue(currentEditableVisibleIndex, "IdProgram");
                LoadPrograme(idCtr);
                
                if (cmbProgram.FindItemByValue(idPrg))
                    cmbProgram.SetSelectedItem(cmbProgram.FindItemByValue(idPrg));
                else {
                    cmbProgram.SetSelectedIndex(-1);

                    cmbProgram.SetText("");
                    cmbProgram.SetValue(null);
                }
            }
        }

        function LoadPrograme(idCtr) {
            if (typeof cmbProgram !== "undefined" && ASPxClientUtils.IsExists(cmbProgram)) {
                let programe = <%=Session["Json_Programe"] %>;
                var arr = programe.filter(function (item) { return item.idContract == idCtr });
                
                cmbProgram.ClearItems();

                var rez = "";
                for (var i = 0; i < arr.length; i++) {
                    cmbProgram.AddItem(arr[i].program, Number(arr[i].idProgram));
                }
            }
        }


    </script>


    <table width="100%">
        <tr>
            <td align="left">
                <dx:ASPxLabel ID="txtTitlu" runat="server" Text="" Font-Size="14px" Font-Bold="true" ForeColor="#00578a" Font-Underline="true" />
            </td>
            <td align="right">
                <dx:ASPxButton ID="btnBack" ClientInstanceName="btnBack" ClientIDMode="Static" runat="server" Text="Inapoi" AutoPostBack="true" OnClick="btnBack_Click" oncontextMenu="ctx(this,event)" Visible="false" >
                    <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png"></Image>
                </dx:ASPxButton>

                <dx:ASPxButton ID="btnPrint" ClientInstanceName="btnPrint" ClientIDMode="Static" runat="server" Text="Imprima" AutoPostBack="true" OnClick="btnPrint_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/print.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnPtjEchipa" ClientInstanceName="btnPtjEchipa" ClientIDMode="Static" runat="server" Text="Pontajul Echipei" AutoPostBack="false" OnClick="btnPtjEchipa_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/chooser.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnRespins" ClientInstanceName="btnRespins" ClientIDMode="Static" runat="server" Text="Respinge" AutoPostBack="true" OnClick="btnRespins_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/renunta.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnAproba" ClientInstanceName="btnAproba" ClientIDMode="Static" runat="server" Text="Aproba" AutoPostBack="true" OnClick="btnAproba_Click" oncontextMenu="ctx(this,event)" >
                    <Image Url="~/Fisiere/Imagini/Icoane/aprobare.png"></Image>
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnInit" ClientInstanceName="btnInit" ClientIDMode="Static" runat="server" Text="Init" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function (s,e) { popUpInit.Show(); }" />
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
                <dx:ASPxButton ID="btnSave" ClientInstanceName="btnSave" ClientIDMode="Static" runat="server" Text="Salveaza" AutoPostBack="false" oncontextMenu="ctx(this,event)" >
                    <ClientSideEvents Click="function(s, e) {
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
                                     <dx:ASPxDateEdit ID="txtAnLuna" runat="server" Width="100px" DisplayFormatString="MM/yyyy" PickerType="Months" EditFormatString="MM/yyyy" EditFormat="Custom" >
                                         <ClientSideEvents ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtAnLuna'); }" />
                                         <CalendarProperties FirstDayOfWeek="Monday" />
                                    </dx:ASPxDateEdit>
                                </div>
                    
                                <div style="float:left; padding-right:15px; vertical-align:middle; display:inline-block;">
                                    <label id="lblRolAng" runat="server" style="float:left; padding-right:15px;">Roluri</label>
                                    <dx:ASPxComboBox ID="cmbRolAng" ClientInstanceName="cmbRolAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRolAng'); }" />
                                    </dx:ASPxComboBox>
                                </div>
                    
                                <div style="float:left; padding-right:15px;">
                                    <label id="lblAng" runat="server" style="display:inline-block; float:left; padding-right:15px;">Angajat</label>
                                    <dx:ASPxComboBox ID="cmbAng" ClientInstanceName="cmbAng" ClientIDMode="Static" runat="server" Width="250px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" SelectInputTextOnClick="true"
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
                                        <ClientSideEvents ButtonClick="function(s, e) { pnlLoading.Show(); e.processOnServer = true; }"/>
                                    </dx:ASPxComboBox>
                                </div>

                                <div style="float:left; padding-right:15px; vertical-align:middle; display:inline-block;">
                                    <label id="lblTip" runat="server" style="float:left; padding-right:15px;">Tip inregistrare</label>
                                    <dx:ASPxComboBox ID="cmbPtjAng" ClientInstanceName="cmbPtjAng" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
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
                                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                                    <Buttons>
                                                        <dx:EditButton Position="Left">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgSt.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                        <dx:EditButton Position="Right">
                                                            <Image Url="~/Fisiere/Imagini/Icoane/sgDr.png" Height="20px" Width="12px"></Image>
                                                        </dx:EditButton>
                                                    </Buttons>
                                                    <ClientSideEvents ButtonClick="function(s, e) {pnlLoading.Show();e.processOnServer = true;}" ValueChanged="function(s, e) { pnlCtl.PerformCallback('txtZiua'); }"  />
                                                </dx:ASPxDateEdit>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblRolZi" runat="server" style="display:inline-block; float:left; padding-right:15px; padding-left:21px; width:80px;">Roluri</label>
                                                <dx:ASPxComboBox ID="cmbRolZi" ClientInstanceName="cmbRolZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbRolZi'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblAngZi" runat="server" style="display:inline-block; float:left; padding-right:15px; width:80px;">Angajat</label>
                                                <dx:ASPxComboBox ID="cmbAngZi" ClientInstanceName="cmbAngZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="F10003" TextField="NumeComplet" ValueType="System.Int32" AutoPostBack="false" SelectInputTextOnClick="true"
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
                                                <label id="lblCtr" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:25px; width:80px;">Contract</label>
                                                <dx:ASPxComboBox ID="cmbCtr" ClientInstanceName="cmbCtr" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)"  />
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblSub" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; width:80px;">Subcomp.</label>
                                                <dx:ASPxComboBox ID="cmbSub" ClientInstanceName="cmbSub" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubcompanie" TextField="Subcompanie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSub'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblFil" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; width:80px;">Filiala</label>
                                                <dx:ASPxComboBox ID="cmbFil" ClientInstanceName="cmbFil" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdFiliala" TextField="Filiala" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbFil'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblSec" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; width:80px;">Sectie</label>
                                                <dx:ASPxComboBox ID="cmbSec" ClientInstanceName="cmbSec" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSectie" TextField="Sectie" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbSec'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="float:left; padding-right:15px;padding-bottom:10px;">
                                                <label id="lblDept" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Dept.</label>
                                                <dx:ASPxComboBox ID="cmbDept" ClientInstanceName="cmbDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdDept" TextField="Dept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" >
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { pnlCtl.PerformCallback('cmbDept'); }" />
                                                </dx:ASPxComboBox>
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblSubDept" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; min-width:75px; width:80px;">Subdept.</label>
                                                <dx:ASPxComboBox ID="cmbSubDept" ClientInstanceName="cmbSubDept" ClientIDMode="Static" runat="server" Width="150px" ValueField="IdSubDept" TextField="SubDept" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblBirou" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; min-width:48px; width:80px;">Birou</label>
                                                <dx:ASPxComboBox ID="cmbBirou" ClientInstanceName="cmbBirou" ClientIDMode="Static" runat="server" Width="150px" ValueField="F00809" TextField="F00810" ValueType="System.Int32" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />
                                            </div>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblPtjZi" runat="server" oncontextMenu="ctx(this,event)" style="float:left; padding-right:15px;">Tip inregistrare</label>
                                                <dx:ASPxComboBox ID="cmbPtjZi" ClientInstanceName="cmbPtjZi" ClientIDMode="Static" runat="server" Width="150px" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" oncontextMenu="ctx(this,event)" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style="float:left; padding-right:15px;">
                                                <label id="lblCateg" runat="server" oncontextMenu="ctx(this,event)" style="display:inline-block; float:left; padding-right:15px; min-width:54px; width:80px;">Categorie</label>
                                                <dx:ASPxComboBox ID="cmbCateg" ClientInstanceName="cmbCateg" ClientIDMode="Static" runat="server" Width="250px" ValueField="Id" TextField="Denumire" ValueType="System.String" AutoPostBack="false" AllowNull="true" oncontextMenu="ctx(this,event)" />                                
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
            <td colspan="2" id="tdGridTotaluri" runat="server">
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <br />
                <dx:ASPxHiddenField ID="hfRowIndex" runat="server" ClientInstanceName="hfRowIndex" ClientIDMode="Static"></dx:ASPxHiddenField>
                <dx:ASPxGridView ID="grDate" runat="server" ClientInstanceName="grDate" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" 
                    OnCustomCallback="grDate_CustomCallback" OnHtmlDataCellPrepared="grDate_HtmlDataCellPrepared" OnHtmlRowPrepared="grDate_HtmlRowPrepared" 
                    OnBatchUpdate="grDate_BatchUpdate" OnDataBound="grDate_DataBound" OnCellEditorInitialize="grDate_CellEditorInitialize" OnCustomJSProperties="grDate_CustomJSProperties" >
                    <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                    <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                    <SettingsSearchPanel Visible="false" />
                    <SettingsLoadingPanel Mode="ShowAsPopup" />
                    <ClientSideEvents ContextMenu="ctx" 
                        BatchEditEndEditing="OnBatchEditEndEditing" 
                        BatchEditStartEditing="OnBatchEditStartEditing" 
				        FocusedRowChanged="grid_FocusedRowChanged"
                        Init="OnInit" 
                        
                        EndCallback="OnEndCallback"
                        CustomButtonClick="grDate_CustomButtonClick"  />
                    <Styles>
                        <BatchEditModifiedCell BackColor="Transparent">
                        </BatchEditModifiedCell>
                    </Styles>
                    <Columns>
                        <dx:GridViewCommandColumn FixedStyle="Left" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" Visible="false" >
                            <CustomButtons>
                                <dx:GridViewCommandColumnCustomButton ID="btnGoToCC">
                                    <Image ToolTip="Centrii de Cost" Url="~/Fisiere/Imagini/Icoane/stare.png" />
                                </dx:GridViewCommandColumnCustomButton>                                
                            </CustomButtons>
                        </dx:GridViewCommandColumn>
                        <dx:GridViewDataTextColumn FieldName="Cheia" Caption=" " ReadOnly="true" Visible="true" ShowInCustomizationForm="true" FixedStyle="Left" VisibleIndex="2" />
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
                <table width="100%" id="tblCC" runat="server" class="ascuns">
                    <tr>
                        <td align="left" style="width:100%;">
                            <br /><br />
                            <dx:ASPxLabel ID="lblZiuaCC" runat="server" ClientIDMode="Static" ClientInstanceName="lblZiuaCC" Font-Bold="true" Visible="true" Text="" />
                            <br /><br />
                        </td>
                        <td align="right">
                            <dx:ASPxButton ID="btnSaveCC" ClientInstanceName="btnSaveCC" ClientIDMode="Static" runat="server" Text="Salveaza CC" AutoPostBack="false" oncontextMenu="ctx(this,event)" Visible="true" >
                                <ClientSideEvents Click="function(s, e) {
                                    grCC.UpdateEdit();
                                    grDate.PerformCallback('btnFiltru');
                                }" />
                                <Image Url="~/Fisiere/Imagini/Icoane/salveaza.png"></Image>
                            </dx:ASPxButton>
                            <br /><br />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <dx:ASPxHiddenField ID="ccValori" runat="server" ClientInstanceName="ccValori" ClientIDMode="Static"></dx:ASPxHiddenField>
                            <dx:ASPxGridView ID="grCC" runat="server" ClientInstanceName="grCC" ClientIDMode="Static" Width="100%" AutoGenerateColumns="false" Visible="true" 
                                OnCustomCallback="grCC_CustomCallback" OnBatchUpdate="grCC_BatchUpdate" OnHtmlDataCellPrepared="grCC_HtmlDataCellPrepared" OnCustomColumnDisplayText="grCC_CustomColumnDisplayText" >
                                <SettingsBehavior AllowSelectByRowClick="true" AllowFocusedRow="true" AllowSelectSingleRowOnly="false" EnableCustomizationWindow="true" ColumnResizeMode="Control" />
                                <Settings ShowFilterRow="False" ShowColumnHeaders="true" ShowGroupPanel="False" HorizontalScrollBarMode="Auto" ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" VerticalScrollBarStyle="VirtualSmooth" VerticalScrollableHeight="130" />
                                <SettingsSearchPanel Visible="false" />
                                <SettingsLoadingPanel Mode="ShowAsPopup" />
                                <SettingsEditing Mode="Batch" BatchEditSettings-EditMode="Cell" BatchEditSettings-StartEditAction="Click" BatchEditSettings-ShowConfirmOnLosingChanges="false" />
                                <ClientSideEvents ContextMenu="ctx" CustomButtonClick="grCC_CustomButtonClick" BatchEditStartEditing="onBatchEditStartEditing" BatchEditEndEditing="onBatchEditEndEditing" EndCallback="function(s,e) { OnEndCallback(s,e); }" />

                                <Columns>
                                    <dx:GridViewCommandColumn FixedStyle="Left" ShowEditButton="true" VisibleIndex="0" ButtonType="Image" Caption=" " Name="butoaneGrid" Width="50px" ShowNewButtonInHeader="true" >
                                        <CustomButtons>
                                            <dx:GridViewCommandColumnCustomButton ID="btnDeleteCC">
                                                <Image ToolTip="Sterge" Url="~/Fisiere/Imagini/Icoane/sterge.png" />
                                            </dx:GridViewCommandColumnCustomButton>
                                        </CustomButtons>
                                    </dx:GridViewCommandColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdStare" Name="IdStare" Caption="Stare" ReadOnly="true" ShowInCustomizationForm="false" Width="150px" VisibleIndex="1" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" />
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="F06204" Name="F06204" Caption="Centrul de cost" Width="250px" VisibleIndex="2" Visible="true">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDownList">
                                            <ValidationSettings>
                                                <RequiredField IsRequired="true" ErrorText="Camp obligatoriu" />
                                            </ValidationSettings>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdProiect" Name="IdProiect" Caption="Proiect" Width="250px" VisibleIndex="3" Visible="false" >
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProiectChanged(s); }"></ClientSideEvents>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdSubproiect" Name="IdSubproiect" Caption="SubProiect" Width="250px" VisibleIndex="4" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" EnableSynchronization="False" IncrementalFilteringMode="StartsWith">
                                            <ClientSideEvents  SelectedIndexChanged="function(s, e) { OnSubproiectChanged(s); }" EndCallback="OnSubEndCallback"/>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>

                                    <dx:GridViewDataComboBoxColumn FieldName="IdActivitate" Name="IdActivitate" Caption="Activitate" Width="250px" VisibleIndex="5" Visible="false">
                                        <PropertiesComboBox TextField="Denumire" ValueField="Id" ValueType="System.Int32" DropDownStyle="DropDown" >
                                        <ClientSideEvents EndCallback="OnActEndCallback" />
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataComboBoxColumn FieldName="IdDept" Name="Dept" Caption="Departament" Width="250px" VisibleIndex="6" Visible="false">
                                        <PropertiesComboBox TextField="Dept" ValueField="IdDept" ValueType="System.Int32" DropDownStyle="DropDown">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="Filiala" Caption="Filiala" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Sectie" Caption="Sectie" Width="130px" />
                                                <dx:ListBoxColumn FieldName="Dept" Caption="Dept" Width="130px" />
                                            </Columns>
                                        </PropertiesComboBox>
                                    </dx:GridViewDataComboBoxColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="De" Name="De" Caption="De" Width="100px" VisibleIndex="7" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>
                                    <dx:GridViewDataTimeEditColumn FieldName="La" Name="La" Caption="La" Width="100px" VisibleIndex="8" Visible="false" >
                                        <PropertiesTimeEdit DisplayFormatInEditMode="true" DisplayFormatString="HH:mm" EditFormat="DateTime" EditFormatString="HH:mm">
                                        </PropertiesTimeEdit>
                                    </dx:GridViewDataTimeEditColumn>

                                    <dx:GridViewDataTextColumn FieldName="NrOre1" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre2" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre3" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre4" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre5" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre6" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre7" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre8" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre9" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>
                                    <dx:GridViewDataTextColumn FieldName="NrOre10" Width="100px">
                                        <PropertiesTextEdit>
                                            <MaskSettings Mask="<00..23>:<00..59>"  />
                                        </PropertiesTextEdit>
                                    </dx:GridViewDataTextColumn>


                                    <dx:GridViewDataTextColumn FieldName="F10003" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="Ziua" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="IdAuto" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="TIME" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />
                                    <dx:GridViewDataTextColumn FieldName="USER_NO" ReadOnly="true" Visible="false" ShowInCustomizationForm="false" />

                                </Columns>
                                
                                <SettingsCommandButton>
                                    <NewButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/new.png" AlternateText="Adauga" ToolTip="Adauga" />
                                    </NewButton>
                                    <DeleteButton>
                                        <Image Url="~/Fisiere/Imagini/Icoane/sterge.png" AlternateText="Sterge" ToolTip="Sterge" />
                                    </DeleteButton>
                                </SettingsCommandButton>

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
                                <dx:ASPxDateEdit ID="txtDataInc" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                </dx:ASPxDateEdit>
                            </td>
                            <td style="padding:15px;">
                               <dx:ASPxLabel ID="lblDataSf" runat="server" Text="Data Sfarsit"></dx:ASPxLabel> 
                            </td>
                            <td>
                                <dx:ASPxDateEdit ID="txtDataSf" runat="server" Width="100px" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                </dx:ASPxDateEdit>
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

    <dx:ASPxPopupControl ID="popUpInit" runat="server" AllowDragging="False" AllowResize="False" ClientIDMode="Static"
        CloseAction="CloseButton" ContentStyle-HorizontalAlign="Center" ContentStyle-VerticalAlign="Top"
        EnableViewState="False" PopupElementID="popUpInitArea" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" ShowFooter="False" ShowOnPageLoad="false" Width="350px" Height="220px" HeaderText="Parametrii initializare"
        FooterText=" " CloseOnEscape="True" ClientInstanceName="popUpInit" EnableHierarchyRecreation="false">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <asp:Panel ID="Panel2" runat="server">
                    <table style="width:100%;">
                        <tr>
                            <td align="right">
                                <dx:ASPxButton ID="btnInitParam" runat="server" Text="Init" AutoPostBack="false" OnClick="btnInitParam_Click" >
                                    <ClientSideEvents Click="function(s, e) {
                                        e.processOnServer = false;
                                        OnInitPtj(s,e);
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

    <dx:ASPxGlobalEvents ID="ge" runat="server">
        <ClientSideEvents ControlsInitialized="OnControlsInitialized" />
    </dx:ASPxGlobalEvents>

</asp:Content>
