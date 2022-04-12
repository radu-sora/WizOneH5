
var valPro = null;
var lastPro = null;
var lastSub = null;

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
    var col = e.focusedColumn.fieldName;

    if (col.length >= 6 && col.substr(0, 6) == 'ValTmp') {
        var keyIndex = s.GetColumnByField("Cheia").index;
        var key = e.rowValues[keyIndex].value;

        if (typeof s.cp_cellsToDisable[key] != "undefined" && s.cp_cellsToDisable[key].indexOf(col.replace('Tmp', '') + ";") >= 0) {
            var ert = '';
        }
        else
            e.cancel = true;
    }
}

function OnBatchEditEndEditing(s, e) {
    var col = s.batchEditApi.GetEditCellInfo().column.fieldName;

    if (col.length >= 4) {
        switch (col.substr(0, 6)) {
            case "ValTmp":
                {
                    var column = s.batchEditApi.GetEditCellInfo().column;

                    var oldVal = s.batchEditApi.GetCellValue(e.visibleIndex, col);
                    var newVal = e.rowValues[column.index].value;

                    if (oldVal != newVal) {
                        grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValAbs", null, null, true);
                        s.GetRowValues(e.visibleIndex, "Afisare", function (Value) {
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

                    for (i = 0; i <= 20; i++) {
                        grDate.batchEditApi.SetCellValue(e.visibleIndex, "ValTmp" + i, null, null, true);
                    }
                }
                break;
        }
    }

}

function OnEditMode(e, idx, Value) {
    var valStr = "";
    debugger;
    for (i = 0; i <= 20; i++) {
        var val = 0;
        var valOre = 0;

        var column = grDate.GetColumnByField("ValTmp" + i);
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

function OnPtjEchipa(s, e) {
    /*
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
    */
}

function grCC_OnNewClick(s, e) {
    grCC.AddNewRow();
}

function grCC_OnCancelClick(s, e) {
    grCC.CancelEdit();
}

function OnClickCC(s, e) {
    if (grDate.GetFocusedRowIndex() != -1) {
        //pnlLoading.Show();
        grCC.PerformCallback('btnCC');
    }
    else {
        swal({
            title: "", text: "Nu exista linie selectata",
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


