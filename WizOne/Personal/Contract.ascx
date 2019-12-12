<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contract.ascx.cs" Inherits="WizOne.Personal.Contract" %>



<script type="text/javascript">

    function OnTextChangedHandlerCtr(s) {
        
        switch (s.name) {
            case "deUltimaZiLucr":
                {
                    var DateTo = new Date(s.GetDate());
                    DateTo.setHours(0, 0, 0, 0);
                    var DateAng = new Date(deDataAng.GetDate());
                    DateAng.setHours(0, 0, 0, 0);

                    if (DateTo < DateAng) {
                        swal({
                            title: "", text: "Data plecarii este mai mica decat data angajarii",
                            type: "warning"
                        });
                    }
                    else {
                        DateTo.setDate(DateTo.getDate() + 1);
                        deDataPlecarii.SetDate(DateTo);
                    }

                    CalcGrila(txtGrila.GetValue());
                }
                break;
            case "deDataPlecarii":
                {
                    var DateTo = new Date(s.GetDate());
                    DateTo.setHours(0, 0, 0, 0);
                    var DateAng = new Date(deDataAng.GetDate());
                    DateAng.setHours(0, 0, 0, 0);

                    if (DateTo < DateAng) {
                        swal({
                            title: "", text: "Data plecarii este mai mica decat data angajarii",
                            type: "warning"
                        });
                    }
                    else {
                        if (DateTo.getTime() != DateMax.getTime()) {
                            DateTo.setDate(DateTo.getDate() - 1);
                            deUltimaZiLucr.SetDate(DateTo);
                        }
                    }
                }
                break;
            case "deDataAng":
            case "deDataCtrInt":
                {
                    var DateCtr = new Date(deDataCtrInt.GetDate());
                    DateCtr.setHours(0, 0, 0, 0);
                    var DateAng = new Date(deDataAng.GetDate());
                    DateAng.setHours(0, 0, 0, 0);

                    if (DateCtr >= DateAng) {
                        swal({
                            title: "", text: "Data contract intern trebuie sa fie anterioara datei angajarii!",
                            type: "warning"
                        });
                    }

                    pnlLoading.Show();
                    pnlCtlContract.PerformCallback(s.name + ";" + s.GetDate());
                    
                    if (cmbDurCtr.GetValue() == 2) {
                        var dtAng = new Date(deDataAng.GetDate());
                        var dtTemp = new Date(dtAng.getFullYear(), dtAng.getMonth(), dtAng.getDate(), 0, 0, 0, 0);
                        deDeLaData.SetValue(dtTemp);
                        var dateDeLa = new Date(deDeLaData.GetDate());
                        var dateLa = new Date(deLaData.GetDate());
                        CalculLuniSiZile(dateDeLa, dateLa);
                        Validare36Luni();
                    }
                }
                break;
            case "deDeLaData":
            case "deLaData":
                {                
                    var dateDeLa = new Date(deDeLaData.GetDate());
                    dateDeLa.setHours(0, 0, 0, 0);
                    var dateLa = new Date(deLaData.GetDate());
                    dateLa.setHours(0, 0, 0, 0);

                    if (dateDeLa > dateLa) {
                        swal({
                            title: "", text: "Data start este ulterioara celei de final!",
                            type: "warning"
                        });
                    }
                    
                    if (s.name == "deLaData"
                        && (dateDeLa.getFullYear() != 2100 || dateDeLa.getMonth() != 1 || dateDeLa.getDate() != 1)
                        && (dateLa.getFullYear() != 2100 || dateLa.getMonth() != 1 || dateLa.getDate() != 1)) {

                        CalculLuniSiZile(dateDeLa, dateLa);

                        deUltimaZiLucr.SetValue(dateLa);
                        var dtTmp = new Date(dateLa.getFullYear(), dateLa.getMonth(), dateLa.getDate(), 0, 0, 0, 0);
                        dtTmp.setDate(dtTmp.getDate() + 1);
                        deDataPlecarii.SetValue(dtTmp);
                        
                        Validare36Luni();
                    }
                    CompletareZile(cmbNivelFunctie);
                    ValidareZile(1);
                }
                break;
        }
    }

    function OnValueChangedHandlerCtr(s) {
        pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue());
    }

    function OnClickCtr(s) {
        pnlLoading.Show();
        pnlCtlContract.PerformCallback(s.name);
    }

    function GoToIstoricCtr(s) {
        strUrl = getAbsoluteUrl + "Avs/Istoric.aspx?qwe=" + s.name;
        popGenIst.SetHeaderText("Istoric modificari contract");
        popGenIst.SetContentUrl(strUrl);
        popGenIst.Show();
    }

    function OnClickCautaCOR(s) {
        window.open('CautaCOR.aspx', '', 'height=400,width=600,left=' + (window.outerWidth / 2 + window.screenX - 300) + ', top=' + (window.outerHeight / 2 + window.screenY - 200));
    }


    window.CompleteazaCOR = function () {
        pnlCtlContract.PerformCallback("btnCautaCOR");
    }

    function OnEndCallbackCtr(s, e) {
        if (s.cpAlertMessage != null) {
            swal({
                title: "", text: s.cpAlertMessage,
                type: "warning"
            });
            s.cpAlertMessage = null;
        }
        pnlLoading.Hide();
    }

    function CalcVechimeComp(s) {
        var ani = "00";
        var luni = "00";
        if (txtVechCompAni.GetValue() != "") ani = txtVechCompAni.GetValue();
        if (txtVechCompLuni.GetValue() != "") luni = txtVechCompLuni.GetValue();
        ani = ('00' + ani).substring(ani.length);
        luni = ('00' + luni).substring(luni.length);
        txtVechimeCompanie.SetValue(ani + luni);
    }

    function CalcVechimeCarte(s) {
        var ani = "00";
        var luni = "00";
        if (txtVechCarteMuncaAni.GetValue() != "") ani = txtVechCarteMuncaAni.GetValue();
        if (txtVechCarteMuncaLuni.GetValue() != "") luni = txtVechCarteMuncaLuni.GetValue();
        ani = ('00' + ani).substring(ani.length);
        luni = ('00' + luni).substring(luni.length);
        txtVechimeCarte.SetValue(ani + luni);
    }

    function CalcVechime(s)
    {
        var azi = new Date();
        var primaAng = deDataPrimeiAng.GetValue();

        var ani = (azi.getFullYear() - primaAng.getFullYear());
        var luni = (azi.getMonth() - primaAng.getMonth());
        if (azi.getMonth() < primaAng.getMonth() || (azi.getMonth() == primaAng.getMonth() && azi.getDate() < primaAng.getDate())) {
            luni = 12 - (primaAng.getMonth() - azi.getMonth());
            ani = ani - 1;
        }


        if (azi.getDate() < primaAng.getDate()) {
                luni = luni - 1;
        }
        
        ani = ('00' + ani.toString()).substring(ani.toString().length);
        luni = ('00' + luni.toString()).substring(luni.toString().length);

        txtVechCarteMuncaAni.SetValue(ani);
        txtVechCarteMuncaLuni.SetValue(luni);
        txtVechimeCarte.SetValue(ani + luni);

        CalcGrila(txtGrila.GetValue());
    }
    function txtGrila_TextChanged(s) {
        CalcGrila(s.GetValue());

        pnlLoading.Show();
        pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue()); 
    }


    function CalcGrila(val) {
        //pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue());  
        if (val == null || val.length <= 0) {
            txtZileCOCuvAnCrt.SetValue("");
            return;
        }
        var nrAni = parseInt(txtVechCarteMuncaAni.GetValue());
        var nrLuni = parseInt(txtVechCarteMuncaLuni.GetValue());
        var dif = parseInt("<%=Session["MP_DiferentaLuni"] %>");
        var nrLuniFinal = nrLuni + dif;
        if (nrLuniFinal >= 12) {
            nrAni++;
            nrLuniFinal -= 12;
        }
        var gasit = 0;
        var vechime = 100 * nrAni + nrLuniFinal;
        var grila = "<%=Session["MP_Grila"] %>";
        var resG = grila.split(";");
        for (var i = 0; i < resG.length; i++) {
            var linieG = resG[i].split(",");
            if (parseInt(linieG[0]) == parseInt(val)) {
                var valMin = parseInt(linieG[1]);
                var valMax = parseInt(linieG[2]);
                if (valMin <= vechime && vechime <= valMax) {
                    txtZileCOCuvAnCrt.SetValue(parseInt(linieG[3]));
                    gasit = 1;
                    break;
                }
            }
        }
        if (gasit == 0)
            txtZileCOCuvAnCrt.SetValue("");
    }

    function dateDiffInDays(a, b) {
        const _MS_PER_DAY = 1000 * 60 * 60 * 24;

        // Discard the time and time-zone information.
        const utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
        const utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());
        return Math.floor((utc2 - utc1) / _MS_PER_DAY);
    }

    var getDaysInMonth = function (month, year) {       
        return new Date(year, month + 1, 0).getDate();   
    };

    //function CalculLuniSiZile(dtInc, dtSf) {    
    //    var ani = (dtSf.getFullYear() - dtInc.getFullYear());
    //    var luni = (dtSf.getMonth() - dtInc.getMonth());
    //    var zile = (dtSf.getDate() - dtInc.getDate());
        
    //    if (dtSf.getMonth() < dtInc.getMonth()) {
    //        luni = 12 - (dtSf.getMonth() - dtInc.getMonth());
    //        ani = ani - 1;
    //    }

    //    if (dtSf.getDate() < dtInc.getDate()) {
    //        luni = luni - 1;
    //        var dtTmp = Date.UTC(dtSf.getFullYear(), dtSf.getMonth() - 1, dtInc.getDate());
    //        zile = dateDiffInDays(new Date(dtTmp), dtSf);
    //    }

    //    txtNrLuni.SetValue((ani * 12 + luni).toString());
    //    txtNrZile.SetValue(zile);
    //}

    function CalculLuniSiZile(dtInc, dtSf) { 
        var arNrZileInLuna = [];

        // determin nr zile calendaristice in luna:
        var odtT1 = new Date(dtInc.getFullYear(), dtInc.getMonth() , dtInc.getDate(), 0, 0, 0, 0);
        var odtT2 = new Date(dtSf.getFullYear(), dtSf.getMonth(), dtSf.getDate(), 0, 0, 0, 0);
        for (var odtDt = odtT1; odtDt <= odtT2;)
        {
            odtD = new Date(
                odtDt.getMonth() == 12 ? odtDt.getFullYear() + 1 : odtDt.getFullYear(),
                odtDt.getMonth() == 12 ? 1 : odtDt.getMonth() + 1, 1, 0, 0, 0, 0);

            arNrZileInLuna.push(getDaysInMonth(odtDt.getMonth(), odtDt.getFullYear()));
            odtDt = odtD;
        }

        var nrLuni = 0;
        var nrZile = 0;
        if (dtSf != new Date(2100, 1, 1, 0, 0, 0, 0) && dtInc != new Date(2100, 1, 1, 0, 0, 0, 0))         
            nrZile = dateDiffInDays(dtInc, dtSf) + 1;

        for (var nI = 0; nI < arNrZileInLuna.length && nrZile >= arNrZileInLuna[nI]; nI++)
        {
            nrZile -= arNrZileInLuna[nI];
            nrLuni++;
        }

	    if (cmbExcIncet.GetValue() == "" || cmbExcIncet.GetValue() == 0) {
            txtNrLuni.SetValue(nrLuni);
            txtNrZile.SetValue(nrZile);
        }
        else {
            txtNrLuni.SetValue("0");
            txtNrZile.SetValue("0");
        }
    }


    function cmbExcIncet_SelectedIndexChanged() {
        if (cmbExcIncet.GetValue() != "" && cmbExcIncet.GetValue() != 0) {
            txtNrLuni.SetValue("0");
            txtNrZile.SetValue("0");
        }
        else {
            var dateDeLa = new Date(deDeLaData.GetDate());
            var dateLa = new Date(deLaData.GetDate());
            CalculLuniSiZile(dateDeLa, dateLa);
            Validare36Luni();
        }
    }

    function SetNorma(s) {      
        if (s.name == "cmbTipAng") {
            cmbTimpPartial.ClearItems();
            hfTipAngajat.Set('TipAng', s.GetValue());
        }      
        
        switch (cmbTipAng.GetSelectedItem().value) {
            case 0:                 //angajat permanent
                {        
                    if (s.name == "cmbTipAng") {
                        for (var i = 6; i <= 8; i++) {
                            var option = new Array(i, i);
                            cmbTimpPartial.AddItem(option);
                        }
                        cmbTimpPartial.SetValue(6);
                    }

                    if (cmbNorma.GetValue() == "")
                        cmbNorma.SetSelectedIndex(2);
                    if (cmbTimpPartial.GetValue() == "")
                        cmbTimpPartial.SetValue(cmbNorma.GetValue());
                    if (cmbNorma.GetValue() < cmbTimpPartial.GetValue()) {
                        swal({ title: "", text: "Timpul partial este mai mare decat norma!", type: "warning" });
                        cmbTimpPartial.SetValue(cmbNorma.GetValue());
                    }

                    cmbTipNorma.ClearItems();
                    var tipN = "<%=Session["MP_ComboTN"] %>";
                    var resN = tipN.split(";");
                    for (var i = 0; i < resN.length; i++) {
                        var linieN = resN[i].split(",");
                        if (linieN[0] == 1) {                           
                            cmbTipNorma.AddItem(linieN[1], Number(linieN[0]));
                        }
                    }

                    cmbTipNorma.SetSelectedIndex(0);
                    //pnlCtlContract.PerformCallback("cmbTipNorma;1");
                 
                    cmbDurTimpMunca.ClearItems();
                    var dtm = "<%=Session["MP_ComboDTM"] %>";
                    var res = dtm.split(";");                   
                    for (var i = 0; i < res.length; i++) {
                        var linie = res[i].split(",");
                        if (linie[2] == 1) {          
                            cmbDurTimpMunca.AddItem(linie[1], Number(linie[0]));
                        }
                    }

                    if (16 <= txtVarsta.GetValue() && txtVarsta.GetValue() < 18)
                        cmbDurTimpMunca.SetSelectedIndex(1);
                    else {
                        if (cmbTimpPartial.GetValue() == 6)
                            cmbDurTimpMunca.SetSelectedIndex(1);
                        if (cmbTimpPartial.GetValue() == 7)
                            cmbDurTimpMunca.SetSelectedIndex(2);
                        if (cmbTimpPartial.GetValue() == 8)
                            cmbDurTimpMunca.SetSelectedIndex(0);
                    }

                    cmbIntRepTimpMunca.SetSelectedIndex(0);
                    cmbIntRepTimpMunca.SetEnabled(false);
                    txtNrOre.SetValue(0);
                    txtNrOre.SetEnabled(false);
                    
                }
                break;
            case 2:                 //angajat timp partial
                {
                    if (s.name == "cmbTipAng") {
                        for (var i = 1; i <= 7; i++) {
                            var option = new Array(i, i);
                            cmbTimpPartial.AddItem(option);
                        }
                        cmbTimpPartial.SetValue(1);
                    }

                    if (cmbNorma.GetValue() == "")
                        cmbNorma.SetSelectedIndex(2);
                    if (cmbTimpPartial.GetValue() == "")
                        cmbTimpPartial.SetValue(cmbNorma.GetValue() - 1);   
                    if (cmbNorma.GetValue() <= cmbTimpPartial.GetValue()) {
                        swal({ title: "", text: "Timpul partial este mai mare decat norma!", type: "warning" });
                        cmbTimpPartial.SetValue(cmbNorma.GetValue() - 1);
                    }

                    //cmbTipNorma.SetValue(2);
                    //pnlCtlContract.PerformCallback("cmbTipNorma;2");

                    cmbTipNorma.ClearItems();
                    var tipN = "<%=Session["MP_ComboTN"] %>";
                    var resN = tipN.split(";");
                    for (var i = 0; i < resN.length; i++) {
                        var linieN = resN[i].split(",");
                        if (linieN[0] == 2) {
                            cmbTipNorma.AddItem(linieN[1], Number(linieN[0]));
                        }
                    }
                    cmbTipNorma.SetSelectedIndex(0);

                    cmbDurTimpMunca.ClearItems();
                    var dtm = "<%=Session["MP_ComboDTM"] %>" ;
                    var res = dtm.split(";");               
                    for (var i = 0; i < res.length; i++) {
                        var linie = res[i].split(",");
                        if (linie[2] == 2) {            
                            cmbDurTimpMunca.AddItem(linie[1], Number(linie[0]));
                        }
                    }                       
                    cmbDurTimpMunca.SetSelectedIndex(0);

                    cmbIntRepTimpMunca.SetEnabled(true);               
                    txtNrOre.SetEnabled(true);
                }
                break;
            default:
                cmbIntRepTimpMunca.SetEnabled(true);
                txtNrOre.SetEnabled(true);
                break;
        }
        if (cmbTipAng.GetSelectedItem().value == 0) {
            cmbIntRepTimpMunca.SetSelectedIndex(0);
            cmbIntRepTimpMunca.SetEnabled(false);
        }
        else
            cmbIntRepTimpMunca.SetEnabled(true);        

        if (s.name == "cmbTimpPartial") {         
            pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue());
            if (16 <= txtVarsta.GetValue() && txtVarsta.GetValue() < 18)
                cmbDurTimpMunca.SetSelectedIndex(1);
            else {
                if (s.GetValue() == 6)
                    cmbDurTimpMunca.SetSelectedIndex(1);
                if (s.GetValue() == 7)
                    cmbDurTimpMunca.SetSelectedIndex(2);
                if (s.GetValue() == 8)
                    cmbDurTimpMunca.SetSelectedIndex(0);
            }
            VerifSalariu(txtSalariu.GetValue(), s.GetValue());
        }
    }

    function ValidareNrOre(s) {
        if (cmbDurTimpMunca.GetValue() == 2 && txtNrOre.GetValue() > 30)
            swal({ title: "", text: "Numar invalid de ore pe luna/saptamana (max 30)!", type: "warning" });

        if (cmbDurTimpMunca.GetValue() == 1 && txtNrOre.GetValue() > 40)
            swal({ title: "", text: "Numar invalid de ore pe luna/saptamana (max 40)!", type: "warning" });     

        if (s.name == "cmbTipNorma") {
            cmbDurTimpMunca.SetValue(null);
            if (cmbTipAng.GetSelectedItem().value == 0 && cmbTipNorma.GetSelectedItem().value == 2) {
                swal({ title: "", text: "Pentru un angajat permanent, tipul de norma nu poate fi 'Cu timp partial'!", type: "warning" });
                cmbTipNorma.SetValue(1);
                pnlCtlContract.PerformCallback("cmbTipNorma;1");
            }
            if (cmbTipAng.GetSelectedItem().value == 2 && cmbTipNorma.GetSelectedItem().value == 1) {
                swal({ title: "", text: "Pentru un angajat cu timp partial, tipul de norma nu poate fi 'intreaga'!", type: "warning" });
                cmbTipNorma.SetValue(2);
                pnlCtlContract.PerformCallback("cmbTipNorma;2");
            }
            pnlCtlContract.PerformCallback(s.name + ";" + s.GetValue());
        }
    }

    function cmbIntRepTimpMunca_SelectedIndexChanged(s) {
        hfIntRepTM.Set('IntRepTM', s.GetValue());
        if (cmbIntRepTimpMunca.GetValue() == 2 || cmbIntRepTimpMunca.GetValue() == 3)
            txtNrOre.SetEnabled(true);
        else
            txtNrOre.SetEnabled(false);
    }

    function cmbGradInvalid_SelectedIndexChanged(s) {
        if (cmbGradInvalid.GetSelectedIndex() > 0)
            deDataValabInvalid.SetEnabled(true);
        else {
            deDataValabInvalid.SetEnabled(false);
            var dtTmp = new Date(2100, 1, 1, 0, 0, 0, 0)
            deDataValabInvalid.SetValue(dtTmp);
        }
        CompletareZile(cmbNivelFunctie);
        ValidareZile(1);
    }

    function cmbDurataContract_SelectedIndexChanged() {
        Validare36Luni();
        CompletareZile(cmbNivelFunctie);
        ValidareZile(1);
    }

    function Validare36Luni() {
        
        if (cmbDurCtr.GetValue() == 1) {
            var dtTmp = new Date(2100, 1, 1, 0, 0, 0, 0)

            deDeLaData.SetEnabled(false);
            deLaData.SetEnabled(false);
            deUltimaZiLucr.SetEnabled(false);
            deDataPlecarii.SetEnabled(false);

            deDeLaData.SetValue(dtTmp);
            deLaData.SetValue(dtTmp);
            deUltimaZiLucr.SetValue(dtTmp);
            deDataPlecarii.SetValue(dtTmp);

            txtNrZile.SetValue("");
            txtNrLuni.SetValue("");
        }

        if (cmbDurCtr.GetValue() == 2) {

            deDeLaData.SetEnabled(true);
            deLaData.SetEnabled(true);
            deUltimaZiLucr.SetEnabled(true);
            deDataPlecarii.SetEnabled(true);
            if (<%=Session["MP_AreContract"] %> == 0)
                deDeLaData.SetValue(deDataAng.GetValue());

            if (deDeLaData.GetValue() != "" && deLaData.GetValue() != "") {
                if (txtNrLuni.GetValue() > 36 || (txtNrLuni.GetValue() == 36 && txtNrZile.GetValue() > 0)) {
                    swal({ title: "", text: "Durata maxima a unui contract pe perioada determinata nu poate depasi 36 de luni", type: "warning" });
                }
                else {
                    var ds36 = new Date("<%=Session["MP_DataSfarsit36"] %>");
                    if (ds36 < deLaData.GetValue())
                        swal({ title: "", text: "Nu puteti prelungi un contract pe perioada determinata mai mult de 36 luni. Mai puteti prelungi contractul pana la data de " + ds36, type: "warning" });
                }
            }
        }
    }

    function cmbPrel_SelectedIndexChanged() {
        
        if (cmbPrel.GetValue() == 1) {
            deDeLaData.SetValue(deLaData.GetValue());
        }
    }



    function chkFunctieBaza_CheckedChanged(){
        if (chkbx4.GetValue() == 0)
            chkbx5.SetValue(0);
    }

    function chkScutitImp_CheckedChanged() {
        
        if (chkbx1.GetValue() == 0) {
            cmbMotivScutit.SetEnabled(false);
            cmbMotivScutit.SetValue(0);
        }
        else {
            if (chkbx5.GetValue() == 0)
                cmbMotivScutit.SetEnabled(true);
            else {
                chkbx1.SetValue(0);
                swal({ title: "", text: "Mai intai debifati Calcul deduceri FB!", type: "warning" });
            }
        }
    }

    function chkScutitCAS_CheckedChanged() {
        if (chkbx6.GetValue() == 0) {
            cmbMotivScutitCAS.SetEnabled(false);
            cmbMotivScutitCAS.SetValue(0);
        }
        else {            
            cmbMotivScutitCAS.SetEnabled(true);        
        }
    }

    function chkCalcDed_CheckedChanged() {
        if (chkbx5.GetValue() == 1) {
            if (chkbx1.GetValue() == 1) {
                chkbx5.SetValue(0);
                swal({ title: "", text: "Angajatul este scutit de impozit, nu i se pot calcula deduceri!", type: "warning" });
            }
        }
    }

    function txtSalariu_TextChanged(s) {      
        VerifSalariu(s.GetValue(), cmbTimpPartial.GetValue());
    }
 
    function VerifSalariu(sal, timp) {
        if (sal == null || sal.length <= 0)      
            return;        
        var salMin = parseInt("<%=Session["MP_SalMin"] %>");
        if (parseInt(salMin) * parseInt(timp) / 8 > parseInt(sal) && cmbIntRepTimpMunca.GetValue() <= 1 && (cmbTipCtrMunca.GetValue() == 1 || cmbTipCtrMunca.GetValue() == 2
            || cmbTipCtrMunca.GetValue() == 3 || cmbTipCtrMunca.GetValue() == 4 || cmbTipCtrMunca.GetValue() == 33 || cmbTipCtrMunca.GetValue() == 34))
            swal({ title: "", text: "Salariul introdus este mai mic decat cel minim raportat la norma si conditiile salariale ale angajatului!", type: "warning" });
    }

    function CompletareZile(s) { 
        debugger;
        var nvlFunc = "<%=Session["MP_NvlFunc"] %>";
        var resNF = nvlFunc.split(";");
        for (var i = 0; i < resNF.length; i++) {
            var linieNF = resNF[i].split(",");
            if (parseInt(linieNF[0]) == parseInt(s.GetValue())) {
                if (linieNF[1].length > 0)
                    txtPerProbaZL.SetValue(linieNF[1]);
                else
                    txtPerProbaZL.SetValue("0");
                if (linieNF[2].length > 0)
                    txtPerProbaZC.SetValue(linieNF[2]);
                else
                    txtPerProbaZC.SetValue("0");
                if (linieNF[3].length > 0)
                    txtNrZilePreavizDemisie.SetValue(linieNF[3]);
                else
                    txtNrZilePreavizDemisie.SetValue("0");
                if (linieNF[4].length > 0)
                    txtNrZilePreavizConc.SetValue(linieNF[4]);
                else
                    txtNrZilePreavizConc.SetValue("0");            
            }
        }
    }

    function ValidareZile(tip) {
        var mesaj = "";      
        var conducere = false;
        var nvlFunc = "<%=Session["MP_NvlFunc"] %>";
        var resNF = nvlFunc.split(";");
        if (cmbNivelFunctie.GetValue() != null && cmbNivelFunctie.GetValue() != "")
            for (var i = 0; i < resNF.length; i++) {
                var linieNF = resNF[i].split(",");
                if (parseInt(linieNF[0]) == parseInt(cmbNivelFunctie.GetValue())) {
                    if (parseInt(linieNF[5]) == 1)
                        conducere = true;
                }
            }
        
        if (cmbDurCtr.GetValue() == 2 && txtPerProbaZL.GetValue() != "" && txtNrLuni.GetValue() != "" && txtNrZile.GetValue() != "") {
            if (txtNrLuni.GetValue() != "0" || txtNrZile.GetValue() != "0") {
                if (txtNrLuni.GetValue() < 3 && parseInt(txtPerProbaZL.GetValue()) > 5) {
                    mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 5 zile!\n";
                    if (tip == 1)
                        txtPerProbaZL.SetValue("5");
                }
                if (txtNrLuni.GetValue() >= 3 && txtNrLuni.GetValue() < 6 && parseInt(txtPerProbaZL.GetValue()) > 15) {
                    mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 15 zile!\n";
                    if (tip == 1)
                        txtPerProbaZL.SetValue("15");
                }
                if (txtNrLuni.GetValue() >= 6 && !conducere && parseInt(txtPerProbaZL.GetValue()) > 30) {
                    mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 30 zile!\n";
                    if (tip == 1)
                        txtPerProbaZL.SetValue("30");
                }
                if (txtNrLuni.GetValue() >= 6 && conducere && parseInt(txtPerProbaZL.GetValue()) > 45) {
                    mesaj += "Perioada de proba (zile lucratoare): - valoarea maxima cf. legii este de 45 zile!\n";
                    if (tip == 1)
                        txtPerProbaZL.SetValue("45");
                }
                //txtPerProbaZC.SetValue("0");
            }
        }     
  
        if (cmbDurCtr.GetValue() == 1 && txtPerProbaZC.GetValue() != "") {
            if (!conducere && parseInt(txtPerProbaZC.GetValue()) > 90) {
                mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 90 zile!\n";
                if (tip == 1)
                    txtPerProbaZC.SetValue("90");
            }
            if (conducere && parseInt(txtPerProbaZC.GetValue()) > 120) {
                mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 120 zile!\n";
                if (tip == 1)
                    txtPerProbaZC.SetValue("120");
            }
            if ((cmbGradInvalid.GetValue() == 2 || cmbGradInvalid.GetValue() == 3) && parseInt(txtPerProbaZC.GetValue()) > 30) {
                mesaj += "Perioada de proba (zile calendaristice): - valoarea maxima cf. legii este de 30 zile!\n";
                if (tip == 1)
                    txtPerProbaZC.SetValue("30");
            }
            //txtPerProbaZL.SetValue("0");
        }
     
        if (txtNrZilePreavizDemisie.GetValue() != "") {
            if (!conducere && parseInt(txtNrZilePreavizDemisie.GetValue()) > 20) {
                mesaj += "Numar zile preaviz demisie: - valoarea maxima cf. legii este de 20 zile!\n";
                if (tip == 1)
                    txtNrZilePreavizDemisie.SetValue("20");
            }
            if (conducere && parseInt(txtNrZilePreavizDemisie.GetValue()) > 45) {
                mesaj += "Numar zile preaviz demisie: - valoarea maxima cf. legii este de 45 zile!\n";
                if (tip == 1)
                    txtNrZilePreavizDemisie.SetValue("45");
            }           
        }    
       
        if (txtNrZilePreavizConc.GetValue() == null || txtNrZilePreavizConc.GetValue() == "" || parseInt(txtNrZilePreavizConc.GetValue()) < 20) {
            mesaj += "Numar zile preaviz concediere: - valoarea minima cf. legii este de 20 zile!\n";
            if (tip == 1)
                txtNrZilePreavizConc.SetValue("20");
        }
        
        if (mesaj.length > 0)
            swal({ title: "", text: mesaj, type: "warning" });
    }

</script>

<body>

    <table width="100%">
		<tr>
			<td align="left">					
			</td>
		</tr>			
	</table>
				


   <dx:ASPxCallbackPanel ID = "Contract_pnlCtl" ClientIDMode="Static" ClientInstanceName="pnlCtlContract" runat="server" OnCallback="pnlCtlContract_Callback" SettingsLoadingPanel-Enabled="false">
       <ClientSideEvents EndCallback="function (s,e) { OnEndCallbackCtr(s,e); }" />
      <PanelCollection>
        <dx:PanelContent>
    <asp:DataList  ID="Contract_DataList" runat="server">  
        <ItemTemplate>
			<div>
            <tr>
             <td>
			  <fieldset class="fieldset-auto-width">
				<legend id="lgContract" runat="server" class="legend-font-size">Contract</legend>
				<table id="lgContractTable" runat="server" width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrCtrInt" Width="100" runat="server"  Text="Nr. ctr. intern" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrCtrInt"  Width="100" runat="server" Text='<%# Eval("F100985") %>'  TabIndex="1" AutoPostBack="false" >
							</dx:ASPxTextBox >
						</td>
                        <td >
                            <dx:ASPxButton ID="btnCtrInt" ClientInstanceName="btnCtrInt" ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"  PaddingRight="10px"/>
                            </dx:ASPxButton>
						</td>	
						<td >
                            <dx:ASPxButton ID="btnCtrIntIst" ClientInstanceName="btnCtrIntIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px" />
                            </dx:ASPxButton>
                        </td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDataCtrInt" runat="server"  Text="Data ctr. intern"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDataCtrInt" ClientInstanceName="deDataCtrInt" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="2" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100986") %>'  AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>
						<td>		
							<dx:ASPxLabel  ID="lblDataAng" runat="server"  Text="Data angajarii"></dx:ASPxLabel >	
						</td>
						<td>
							<dx:ASPxDateEdit  ID="deDataAng" ClientIDMode="Static" ClientInstanceName="deDataAng" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="3" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10022") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							</dx:ASPxDateEdit>										
						</td>
                        <td>
                            <dx:ASPxButton ID="btnDataAng" ClientInstanceName="btnDataAng"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnDataAngIst" ClientInstanceName="btnDataAngIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblTermenRevisal" runat="server"  Text="Termen depunere Revisal"></dx:ASPxLabel >	
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deTermenRevisal" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" ReadOnly="true"  Enabled="false"  AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />                               
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblTipCtrMunca" runat="server"  Text="Tip contract munca"></dx:ASPxLabel>	
						</td>
						<td>	
							<dx:ASPxComboBox DataSourceID="dsTCM"  Value='<%#Eval("F100984") %>' ID="cmbTipCtrMunca" Width="100"  runat="server" DropDownStyle="DropDown" TabIndex="4"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" >
							</dx:ASPxComboBox>
						</td>
					</tr>	
                    
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDurCtr" runat="server"  Text="Durata contract" ></dx:ASPxLabel>	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsDC"  Value='<%#Eval("F1009741") %>'  ID="cmbDurCtr"  ClientInstanceName="cmbDurCtr" Width="100" runat="server" TabIndex="5" DropDownStyle="DropDown"  TextField="F08903" ValueField="F08902" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbDurataContract_SelectedIndexChanged(); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>
						<td >		
							<dx:ASPxLabel  ID="lblDeLaData" runat="server"  Text="De la data"></dx:ASPxLabel>
						</td>
						<td>			
							<dx:ASPxDateEdit  ID="deDeLaData" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100933") %>'  TabIndex="6" AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                 <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							</dx:ASPxDateEdit>					
						</td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblLaData" runat="server"  Text="La data"></dx:ASPxLabel>	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deLaData" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100934") %>' TabIndex="7" AutoPostBack="false"   >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrLuni" Width="100" runat="server"  Text="Nr. luni" ></dx:ASPxLabel>	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrLuni" ClientInstanceName="txtNrLuni"  Width="100" runat="server" ReadOnly="true" AutoPostBack="false" ></dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrZile" Width="100" runat="server"  Text="Nr. zile" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrZile" ClientInstanceName="txtNrZile" Width="100" runat="server" ReadOnly="true"  AutoPostBack="false" ></dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblPrel" runat="server"  Text="Prelungire contract" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsPC"  Value='<%#Eval("F100938") %>' ID="cmbPrel" ClientInstanceName="cmbPrel"  Width="100" runat="server" TabIndex="8" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbPrel_SelectedIndexChanged(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblExcIncet" runat="server"  Text="Exceptie incetare" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsEI"  Value='<%#Eval("F100929") %>'  ID="cmbExcIncet"  Width="100" runat="server" DropDownStyle="DropDown" TabIndex="9"  TextField="F09403" ValueField="F09402" ValueType="System.Int32">
                                 <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbExcIncet_SelectedIndexChanged(s); }" />
							</dx:ASPxComboBox>
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCASSAngajat" runat="server"  Text="Casa sanatate" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCASS"  Value='<%#Eval("F1003900") %>' ID="cmbCASSAngajat"  Width="100" runat="server" DropDownStyle="DropDown" TabIndex="10" TextField="F06303" ValueField="F06302" ValueType="System.Int32" >
                                
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnCASS" ClientInstanceName="btnCASS"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCASSIst" ClientInstanceName="btnCASSIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCASSAngajator" runat="server"  Text="CASS Angajator" Visible="false" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCASS"  Value='<%#Eval("F1003907") %>' ID="cmbCASSAngajator" Visible="false"  Width="100" runat="server" DropDownStyle="DropDown"  TextField="F06303" ValueField="F06302" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblSalariu" Width="100" runat="server"  Text="Salariu" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtSalariu"  Width="100" runat="server"  DisplayFormatString="N0"  TabIndex="11" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ txtSalariu_TextChanged(s); }" />                                
							</dx:ASPxTextBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnSalariu" ClientInstanceName="btnSalariu"  ClientIDMode="Static"  Width="5" Height="5" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" runat="server" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnSalariuIst" ClientInstanceName="btnSalariuIst"  ClientIDMode="Static"  Width="5" Height="5" Font-Size="1px"  RenderMode="Link" ToolTip="Istoric modificari" runat="server" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifSal" runat="server"  Text="Data modificare salariu"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifSal" Width="100"  runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="12" EditFormatString="dd.MM.yyyy" Value='<%# Bind("F100991") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCategAng1" runat="server"  Text="Categorie angajat 1" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCategAng_61"  Value='<%#Eval("F10061") %>' ID="cmbCategAng1"  Width="100" TabIndex="13" runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCategAng2" runat="server"  Text="Categorie angajat 2" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCategAng_62"  Value='<%#Eval("F10062") %>' ID="cmbCategAng2"  Width="100" TabIndex="14" runat="server" DropDownStyle="DropDown"  TextField="F72404" ValueField="F72402" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblLocAnt" Width="100" runat="server"  Text="Loc anterior" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtLocAnt"  Width="100" runat="server" Text='<%# Eval("F10078") %>' TabIndex="15" AutoPostBack="false" >
                               
							</dx:ASPxTextBox >
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblLocatieInt" Width="100" runat="server"  Text="Locatie interna" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsLocatieInt"  Value='<%#Eval("F100966") %>' ID="cmbLocatieInt"  Width="100" TabIndex="16" runat="server" DropDownStyle="DropDown"  TextField="LOCATIE" ValueField="NUMAR" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>
                        <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkSalMin"  runat="server" Width="150" Text="Salariu minim conform studii superioare"  TextAlign="Left" TabIndex="17"  Checked='<%#  Eval("F1001117") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001117"))%>'  ClientInstanceName="chkbx7" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
                       <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkConstr"  runat="server" Width="150" Text="Calcul activitate constructii (indiferent de venit)" TextAlign="Left" TabIndex="18" Enabled="false"  Checked='<%#  Eval("F1001118") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001118"))%>' ClientInstanceName="chkbx8" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
					             				
				</table>
                <asp:ObjectDataSource runat="server" ID="dsTCM" TypeName="WizOne.Module.General" SelectMethod="GetTipContract" />
                <asp:ObjectDataSource runat="server" ID="dsDC"  TypeName="WizOne.Module.General" SelectMethod="GetDurataContract" />
                <asp:ObjectDataSource runat="server" ID="dsPC"  TypeName="WizOne.Module.General" SelectMethod="GetPrelungireContract" />
                <asp:ObjectDataSource runat="server" ID="dsEI"  TypeName="WizOne.Module.General" SelectMethod="GetExceptieIncetare" />
                <asp:ObjectDataSource runat="server" ID="dsCASS"  TypeName="WizOne.Module.General" SelectMethod="GetCASS" />
                <asp:ObjectDataSource runat="server" ID="dsCategAng_61"  TypeName="WizOne.Module.General" SelectMethod="GetCategAng_61" />
                <asp:ObjectDataSource runat="server" ID="dsCategAng_62"  TypeName="WizOne.Module.General" SelectMethod="GetCategAng_62" />
                <asp:ObjectDataSource runat="server" ID="dsLocatieInt"  TypeName="WizOne.Module.General" SelectMethod="GetLocatieInt" />
			  </fieldset>
			  <fieldset class="fieldset-auto-width">
				<legend id="lgTipM" runat="server" class="legend-font-size">Tip munca</legend>
				<table id="lgTipMTable" runat="server" width="60%">	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblTipAng" Width="100" runat="server"  Text="Tip angajat" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsTA"  Value='<%#Eval("F10010") %>' ID="cmbTipAng" Width="130" TabIndex="19" ClientInstanceName="cmbTipAng" runat="server" DropDownStyle="DropDown"  TextField="F71604" ValueField="F71602"  ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ SetNorma(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblTimpPartial"  Width="100" runat="server"  Text="Timp partial" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox  DataSourceID="dsTP"  ID="cmbTimpPartial" Value='<%#Eval("F10043") %>' Width="100" TabIndex="20" runat="server" ClientInstanceName="cmbTimpPartial" TextField="Denumire" ValueField="Id"   AutoPostBack="false" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ SetNorma(s); }" />
							</dx:ASPxComboBox>
						</td>
                    </tr>
                    <tr>
						<td >
							<dx:ASPxLabel  ID="lblNorma"  Width="100" runat="server"  Text="Norma" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsN"  Value='<%#Eval("F100973") %>' ID="cmbNorma" Width="100" runat="server" TabIndex="21" ClientInstanceName="cmbNorma" TextField="Denumire" ValueField="Id"  AutoPostBack="false" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ SetNorma(s); }" />
							</dx:ASPxComboBox>
						</td>
						<td>	
                            <dx:ASPxButton ID="btnNorma" ClientInstanceName="btnNorma" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>									
						</td>
                        <td>
                            <dx:ASPxButton ID="btnNormaIst" ClientInstanceName="btnNormaIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                    </tr>
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifNorma" runat="server"  Text="Data modificare norma"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifNorma" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="22" EditFormatString="dd.MM.yyyy" Value='<%# Bind("F100955") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr>
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblTipNorma" runat="server"  Text="Tip norma" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox   ID="cmbTipNorma" TabIndex="23" Width="130" ClientInstanceName="cmbTipNorma" runat="server" DropDownStyle="DropDown"  TextField="F09203" ValueField="F09202" ValueType="System.Int32" >
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ ValidareNrOre(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>	
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblDurTimpMunca" runat="server"  Text="Durata timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox    ID="cmbDurTimpMunca" Width="130" TabIndex="24" ClientInstanceName="cmbDurTimpMunca" runat="server" DropDownStyle="DropDown"  TextField="F09103" ValueField="F09102" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ ValidareNrOre(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>   
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblRepTimpMunca" runat="server"  Text="Repartizare timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsRTM"  Value='<%#Eval("F100928") %>'  ID="cmbRepTimpMunca" Width="130" runat="server" TabIndex="25" DropDownStyle="DropDown"  TextField="F09303" ValueField="F09302" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
					</tr>    
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblIntervRepTimpMunca" runat="server"  Text="Interval repartizare timp munca" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsIRTM"  Value='<%#Eval("F100939") %>' ID="cmbIntRepTimpMunca" Width="130" TabIndex="26" ClientInstanceName="cmbIntRepTimpMunca" runat="server" DropDownStyle="DropDown"  TextField="F09603" ValueField="F09602"  ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbIntRepTimpMunca_SelectedIndexChanged(s); }" />
							</dx:ASPxComboBox >
						</td>
					</tr>     
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNrOre" Width="100" runat="server"  Text="Nr ore pe luna/saptamana" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxTextBox  ID="txtNrOre"  Width="75" runat="server" ClientInstanceName="txtNrOre" Text='<%# Bind("F100964") %>' TabIndex="27" AutoPostBack="false" >
                                <ClientSideEvents TextChanged="function(s,e){ ValidareNrOre(s); }" />
							</dx:ASPxTextBox >
						</td>
					</tr>  
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblCOR" runat="server"  Text="COR" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsCOR"  ID="cmbCOR"   Enabled="false" Width="130"  runat="server" DropDownStyle="DropDown" DropDownWidth ="700"  TextField="F72204" ValueField="F72202" ValueType="System.Int32" >
                                <Columns>
                                    <dx:ListBoxColumn FieldName="F72202" Caption="Cod COR" Width="100px" />
                                    <dx:ListBoxColumn FieldName="F72204" Caption="Descriere" Width="600px" />
                                </Columns>
							</dx:ASPxComboBox>	
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnCautaCOR" ClientInstanceName="btnCautaCOR" ClientIDMode="Static" TabIndex="28"  Width="20" Height="20" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Cauta cod COR" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCautaCOR(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/lupa.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>				
						</td>
						<td>	
                            <dx:ASPxButton ID="btnCOR" ClientInstanceName="btnCOR" ClientIDMode="Static"  Width="20" Height="20" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>									
						</td>
                        <td>
                            <dx:ASPxButton ID="btnCORIst" ClientInstanceName="btnCORIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr> 
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifCOR" runat="server"  Text="Data modificare COR"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifCOR" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="29" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100956") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                               
							</dx:ASPxDateEdit>										
						</td>
					</tr>  
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblFunctie" runat="server"  Text="Functie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsFunctie"  Value='<%#Eval("F10071") %>' ID="cmbFunctie" Width="130" TabIndex="30" runat="server" DropDownStyle="DropDown"  TextField="F71804" ValueField="F71802" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnFunc" ClientInstanceName="btnFunc"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link"  ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnFuncIst" ClientInstanceName="btnFuncIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>  
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblNivelFunctie" runat="server"  Text="Nivel functie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox  ID="cmbNivelFunctie" Width="130" TabIndex="31" runat="server" DropDownStyle="DropDown"  TextField="Denumire" ValueField="Id" ValueType="System.Int32">
                                 <ClientSideEvents SelectedIndexChanged="function(s,e){  CompletareZile(s);  ValidareZile(1); }" />
							</dx:ASPxComboBox >
						</td>
					</tr> 
					<tr>				
						<td>		
							<dx:ASPxLabel  ID="lblDataModifFunctie" runat="server"  Text="Data modificare functie"></dx:ASPxLabel >	
						</td>
						<td>	
							<dx:ASPxDateEdit  ID="deDataModifFunctie" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" TabIndex="32" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100992") %>' AutoPostBack="false"  >
                                <CalendarProperties FirstDayOfWeek="Monday" />
                                
							</dx:ASPxDateEdit>										
						</td>
					</tr> 
					<tr>				
						<td >
							<dx:ASPxLabel  ID="lblMeserie" runat="server"  Text="Meserie" ></dx:ASPxLabel >	
						</td>	
						<td>
							<dx:ASPxComboBox DataSourceID="dsMeserie"  Value='<%#Eval("F10029") %>' ID="cmbMeserie" Width="130" TabIndex="33" runat="server" DropDownStyle="DropDown"  TextField="F71704" ValueField="F71702" ValueType="System.Int32">
                                
							</dx:ASPxComboBox >
						</td>
                        <td>
                            <dx:ASPxButton ID="btnMeserie" ClientInstanceName="btnMeserie"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false" >
                                <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
                        <td>
                            <dx:ASPxButton ID="btnMeserieIst" ClientInstanceName="btnMeserieIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                <Paddings PaddingLeft="10px"/>
                            </dx:ASPxButton>
                        </td>
					</tr>  
                    <tr>
                        <td>
                            <dx:ASPxCheckBox ID="chkFunctieBaza"  runat="server" Width="150" Text="Functie de baza" TextAlign="Left" TabIndex="34"  Checked='<%#  Eval("F10032") == DBNull.Value ? false : Convert.ToBoolean(Eval("F10032"))%>' ClientInstanceName="chkbx4" >
                                <ClientSideEvents CheckedChanged="function(s,e){ chkFunctieBaza_CheckedChanged(s); }" />
                            </dx:ASPxCheckBox>
                        </td>

				    </tr>         
                   <tr>
                        <td>
                            <dx:ASPxCheckBox ID="chkCalcDed"  runat="server" Width="150" Text="Calcul deduceri FB" TextAlign="Left" TabIndex="35"  Checked='<%#  Eval("F10048") == DBNull.Value ? false : Convert.ToBoolean(Eval("F10048"))%>' ClientInstanceName="chkbx5" >
                                <ClientSideEvents CheckedChanged="function(s,e){ chkCalcDed_CheckedChanged(s); }" />
                            </dx:ASPxCheckBox>
                        </td>

				    </tr>   
                        <tr>
                            <td>
                                <dx:ASPxCheckBox ID="chkScutitImp" runat="server" Width="150" Text="Scutit impozit" TextAlign="Left" TabIndex="36"  Checked='<%#  Eval("F10026") == DBNull.Value ? false : Convert.ToBoolean(Eval("F10026"))%>' ClientInstanceName="chkbx1">
                                    <ClientSideEvents CheckedChanged="function(s,e){ chkScutitImp_CheckedChanged(s); }" />
                                </dx:ASPxCheckBox>
                            </td>
                        </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblMotivScutit" Width="100" runat="server"  Text="Motiv scutire impozit" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxComboBox DataSourceID="dsMSI"  Value='<%#Eval("F1001098") %>' ID="cmbMotivScutit"  ClientInstanceName="cmbMotivScutit" TabIndex="37" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F80404" ValueField="F80403" AutoPostBack="false"  ValueType="System.Int32" >
                                        
							    </dx:ASPxComboBox>
						    </td>               
					    </tr>
                        <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkScutitCAS" runat="server" Width="150" Text="Scutit de la pragul minim CASS si CAS asigurat" TextAlign="Left" TabIndex="38" ClientInstanceName="chkbx6">
                                    <ClientSideEvents CheckedChanged="function(s,e){ chkScutitCAS_CheckedChanged(s); }" />
                                </dx:ASPxCheckBox>
                            </td>
                        </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblMotivScutitCAS" Width="100" runat="server"  Text="Motiv scutire de la pragul minim CASS si CAS asigurat" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxComboBox DataSourceID="dsMSCAS"  Value='<%#Eval("F1001096") %>' ID="cmbMotivScutitCAS"  ClientInstanceName="cmbMotivScutitCAS" TabIndex="39" Width="130"  runat="server" DropDownStyle="DropDown"  TextField="F80204" ValueField="F80203" AutoPostBack="false"  ValueType="System.Int32" >
                                        
							    </dx:ASPxComboBox>
						    </td>               
					    </tr>                    
				</table>
                <asp:ObjectDataSource runat="server" ID="dsTA" TypeName="WizOne.Module.General" SelectMethod="GetTipAngajat" />   
                <asp:ObjectDataSource runat="server" ID="dsN" TypeName="WizOne.Module.General" SelectMethod="GetNorma"/>
                <asp:ObjectDataSource runat="server" ID="dsTP" TypeName="WizOne.Module.General" SelectMethod="GetTimpPartial">
                    <SelectParameters>
                            <asp:Parameter Name="tip"  Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
                <asp:ObjectDataSource runat="server" ID="dsRTM" TypeName="WizOne.Module.General" SelectMethod="GetRepartizareTimpMunca"/>
                <asp:ObjectDataSource runat="server" ID="dsIRTM" TypeName="WizOne.Module.General" SelectMethod="GetIntervalRepartizareTimpMunca"/>
                <asp:ObjectDataSource runat="server" ID="dsCOR" TypeName="WizOne.Module.General" SelectMethod="GetCOR"/>
                <asp:ObjectDataSource runat="server" ID="dsFunctie" TypeName="WizOne.Module.General" SelectMethod="GetFunctie"/>
                <asp:ObjectDataSource runat="server" ID="dsMeserie" TypeName="WizOne.Module.General" SelectMethod="GetMeserie"/>
			  </fieldset>
            </td>
           <td valign="top" width="310">      
			      <fieldset >
				    <legend id="lgPerioada" runat="server"  class="legend-font-size">Perioada</legend>
				    <table id="lgPerioadaTable" runat="server"  width="60%">	
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblPerioadaProba" width="125" runat="server"  Text="Perioada de proba" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblZL" runat="server"  Text="zile lucratoare" ></dx:ASPxLabel >
                            </td>
                            <td align="right">
							    <dx:ASPxTextBox  ID="txtPerProbaZL" Width="35" TabIndex="40" runat="server" Text='<%# Eval("F100975") %>' AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ ValidareZile(0); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest1" runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
                                <dx:ASPxLabel  ID="lblZC" runat="server"  Text="zile calendaristice" ></dx:ASPxLabel >
                            </td>
                            <td align="right">
							    <dx:ASPxTextBox  ID="txtPerProbaZC" Width="35"  runat="server" TabIndex="41" Text='<%# Eval("F1001063") %>' AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ ValidareZile(0); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblNrZilePreavizDemisie" runat="server"  Text="Nr zile preaviz demisie" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtNrZilePreavizDemisie" Width="75"  runat="server" TabIndex="42" Text='<%# Eval("F1009742") %>' AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ ValidareZile(0); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblNrZilePreavizConc" runat="server"  Text="Nr zile preaviz concediere" ></dx:ASPxLabel >	
						    </td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtNrZilePreavizConc" Width="75"  runat="server" TabIndex="43" Text='<%# Eval("F100931") %>' AutoPostBack="false" >
                                    <ClientSideEvents TextChanged="function(s,e){ ValidareZile(0); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
				    </table>
			        </fieldset>
			        <fieldset >
				    <legend id="lgDataInc" runat="server" class="legend-font-size">Data incetare</legend>
				        <table id="lgDataIncTable" runat="server" width="60%">	
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblUltimaZiLucr" runat="server"  Text="Ultima zi lucrata"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deUltimaZiLucr" ClientIDMode="Static" ClientInstanceName="deUltimaZiLucr" TabIndex="44" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F10023") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" /> 
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblMotivPlecare" Width="100" runat="server"  Text="Motiv plecare" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsMP"  Value='<%#Eval("F10025") %>' ID="cmbMotivPlecare"  Width="100" TabIndex="45" runat="server" DropDownStyle="DropDown"  TextField="F72104" ValueField="F72102" AutoPostBack="false"  ValueType="System.Int32" >
                                        
							        </dx:ASPxComboBox>
						        </td>
                                <td>
                                    <dx:ASPxButton ID="btnMotivPl" ClientInstanceName="btnMotivPl"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px"  RenderMode="Link" ToolTip="Modificari contract" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ OnClickCtr(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/edit.png"></Image>
                                        <Paddings PaddingLeft="10px" PaddingRight="10px"/>
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton ID="btnMotivPlIst" ClientInstanceName="btnMotivPlIst"  ClientIDMode="Static"  Width="5" Height="5" runat="server" Font-Size="1px" RenderMode="Link" ToolTip="Istoric modificari" oncontextMenu="ctx(this,event)" AutoPostBack="false">
                                        <ClientSideEvents Click="function(s,e){ GoToIstoricCtr(s); }" />
                                        <Image Url="../Fisiere/Imagini/Icoane/istoric.png"></Image>
                                        <Paddings PaddingLeft="10px"/>
                                    </dx:ASPxButton>
                                </td>
					        </tr>
					        <tr>				
						        <td>
							        <dx:ASPxLabel  ID="lblDataPlecarii" runat="server"  Text="Data plecarii"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataPlecarii" ClientIDMode="Static" ClientInstanceName="deDataPlecarii" Width="100"  TabIndex="46" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100993") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        <ClientSideEvents DateChanged="function(s,e){ OnTextChangedHandlerCtr(s); }" />
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataReintegr" runat="server"  Text="Data reintegrare"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataReintegr" Width="100" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" TabIndex="47" Value='<%# Eval("F100930") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                       
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
					        <tr>				
						        <td >
							        <dx:ASPxLabel  ID="lblGradInvalid" Width="100" runat="server"  Text="Grad invaliditate" ></dx:ASPxLabel >	
						        </td>	
						        <td>
							        <dx:ASPxComboBox DataSourceID="dsGI"  Value='<%#Eval("F10027") %>' ID="cmbGradInvalid" ClientInstanceName="cmbGradInvalid" Width="100" TabIndex="48" runat="server" DropDownStyle="DropDown" TextField="Denumire" ValueField="Id" AutoPostBack="false" ValueType="System.Int32" >
                                        <ClientSideEvents SelectedIndexChanged="function(s,e){ cmbGradInvalid_SelectedIndexChanged(s); }" />
							        </dx:ASPxComboBox>
						        </td>
					        </tr>
					        <tr>				
						        <td>		
							        <dx:ASPxLabel  ID="lblDataValabInvalid" runat="server"  Text="Data valabilitate invaliditate"></dx:ASPxLabel >	
						        </td>
						        <td>	
							        <dx:ASPxDateEdit  ID="deDataValabInvalid" Width="100" runat="server" ClientInstanceName="deDataValabInvalid" TabIndex="49" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F100271") %>' AutoPostBack="false"  >
                                        <CalendarProperties FirstDayOfWeek="Monday" />
                                        
							        </dx:ASPxDateEdit>										
						        </td>
					        </tr>
                        <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkBifaPensionar" runat="server" Width="150" Text="Pensionar" TextAlign="Left" TabIndex="50"  Checked='<%#  Eval("F10037") == DBNull.Value ? false : Convert.ToBoolean(Eval("F10037"))%>' ClientInstanceName="chkbx2" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
                        <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkCotaForfetara" runat="server" Width="150" Text="Cota forfetara" TextAlign="Left" TabIndex="51"  Checked='<%#  Eval("F1001069") == DBNull.Value ? false : Convert.ToBoolean(Eval("F1001069"))%>' ClientInstanceName="chkbx9" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
                        <tr>
                            <td colspan="2">
                                <dx:ASPxCheckBox ID="chkBifaDetasat"  runat="server" Width="150" Text="Detasat de la alt angajator" TextAlign="Left" TabIndex="52"  Checked='<%#  Eval("F100954") == DBNull.Value ? false : Convert.ToBoolean(Eval("F100954"))%>' ClientInstanceName="chkbx3" >
                                    
                                </dx:ASPxCheckBox>
                            </td>

				        </tr>
                        <tr>
                            <td>
                                <dx:ASPxLabel ID="lblCtrRadiat" runat="server" Text="Contract radiat"></dx:ASPxLabel>
                            </td>
                            <td>
                                <dx:ASPxRadioButtonList ID="rbCtrRadiat" runat="server" ClientInstanceName="rbCtrRadiat" TabIndex="53" RepeatDirection="Horizontal">
                                    <Items>
                                        <dx:ListEditItem Text="DA" Value="1" />
                                        <dx:ListEditItem Text="NU" Value="0" />
                                    </Items>
                                </dx:ASPxRadioButtonList>
                            </td>

					    </tr>
				        </table>
                        <asp:ObjectDataSource runat="server" ID="dsMP" TypeName="WizOne.Module.General" SelectMethod="GetMotivPlecare"/>
                        <asp:ObjectDataSource runat="server" ID="dsGI" TypeName="WizOne.Module.General" SelectMethod="ListaMP_GradInvaliditate"/>
                        <asp:ObjectDataSource runat="server" ID="dsMSI" TypeName="WizOne.Module.General" SelectMethod="GetMotivScutireImpozit"/>
                        <asp:ObjectDataSource runat="server" ID="dsMSCAS" TypeName="WizOne.Module.General" SelectMethod="GetMotivScutireCAS"/>
			        </fieldset>           
             </td>
              <td valign="top">   
			      <fieldset class="fieldset-auto-width">
				    <legend id="lgSitCOCtr" runat="server" class="legend-font-size">Situatie CO</legend>
				    <table id="lgSitCOCtrTable" runat="server" width="60%">	
				       <tr>				
						   <td >
							    <dx:ASPxLabel  ID="lblVechimeComp" width="150" runat="server"  Text="Vechime in companie" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCompAni"  runat="server"  Text="ani" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCompAni" ClientInstanceName="txtVechCompAni" Width="25" TabIndex="54" runat="server" AutoPostBack="false" MaxLength="2">
                                    <ClientSideEvents TextChanged="function(s,e){ CalcVechimeComp(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest2" width="150" runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCompLuni" runat="server"  Text="luni" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCompLuni" ClientInstanceName="txtVechCompLuni" Width="25" TabIndex="55" runat="server" AutoPostBack="false" MaxLength="2">
                                    <ClientSideEvents TextChanged="function(s,e){ CalcVechimeComp(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblVechimeCarteMunca" width="150" runat="server"  Text="Vechime pe cartea de munca" ></dx:ASPxLabel >	
						    </td>	
						    <td align="left">
                                <dx:ASPxLabel  ID="lblVechCarteMuncaAni" runat="server"  Text="ani" ></dx:ASPxLabel >
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCarteMuncaAni" Width="25"  runat="server" AutoPostBack="false" TabIndex="56" MaxLength="2">
                                    <ClientSideEvents TextChanged="function(s,e){ CalcVechimeCarte(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>
						    <td >
							    <dx:ASPxLabel  ID="lblTest3" width="150"  runat="server"  Text=" " ></dx:ASPxLabel >	
						    </td>
						    <td align="left">
                                 <dx:ASPxLabel  ID="lblVechCarteMuncaLuni" runat="server"  Text="luni" ></dx:ASPxLabel >                                
                            </td>
                            <td align="left">
							    <dx:ASPxTextBox  ID="txtVechCarteMuncaLuni" Width="25"  runat="server" AutoPostBack="false" TabIndex="57" MaxLength="2" >
                                    <ClientSideEvents TextChanged="function(s,e){ CalcVechimeCarte(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblGrila" runat="server"  Text="Grila" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtGrila" Width="75"  runat="server" Text='<%# Eval("F10072") %>' TabIndex="58" AutoPostBack="false" >
                                       <ClientSideEvents TextChanged="function(s,e){ txtGrila_TextChanged(s); }" />
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOFidel" runat="server"  Text="Zile CO fidelitate" ></dx:ASPxLabel >	
						    </td>
                            <td></td>	                            	
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOFidel" Width="75"  runat="server" Text='<%# Eval("F100640") %>' TabIndex="59" AutoPostBack="false" >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOAnAnt" runat="server"  Text="Zile CO an anterior" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOAnAnt" Width="75"  runat="server" Text='<%# Eval("F100996") %>' TabIndex="60" ClientEnabled="false" AutoPostBack="false" >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOCuvAnCrt" runat="server"  Text="Zile CO cuvenite cf. grila" ></dx:ASPxLabel >	
						    </td>	
                            <td></td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOCuvAnCrt" Width="75"  runat="server" Text='<%# Eval("F100642") %>' TabIndex="61" AutoPostBack="false" >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZileCOAnCrt" runat="server"  Text="Zile CO an curent" ></dx:ASPxLabel >	
						    </td>	
                            <td></td>	
						    <td>
							    <dx:ASPxTextBox  ID="txtZileCOAnCrt" Width="75"  runat="server" Text='<%# Eval("F100995") %>' TabIndex="62" AutoPostBack="false" >
                                    
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZLP" runat="server"  Text="Zile libere platite" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZLP" Width="75"  runat="server" ReadOnly="true" AutoPostBack="false" TabIndex="63" >                                
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td >
							    <dx:ASPxLabel  ID="lblZLPCuv" runat="server"  Text="Zile libere platite cuvenite" ></dx:ASPxLabel >	
						    </td>
                            <td></td>		
						    <td>
							    <dx:ASPxTextBox  ID="txtZLPCuv" Width="75"  runat="server" ReadOnly="true" AutoPostBack="false" TabIndex="64">                                   
							    </dx:ASPxTextBox>
						    </td>
					    </tr>
					    <tr>				
						    <td>		
							    <dx:ASPxLabel  ID="lblDataPrimeiAng" runat="server"  Text="Data primei angajari"></dx:ASPxLabel >	
						    </td>
                            <td></td>	
						    <td>	
							    <dx:ASPxDateEdit  ID="deDataPrimeiAng" ClientInstanceName="deDataPrimeiAng" Width="85" TabIndex="65" runat="server" DisplayFormatString="dd.MM.yyyy" EditFormatString="dd.MM.yyyy" Value='<%# Eval("F1001049") %>' AutoPostBack="false"  >
                                    <CalendarProperties FirstDayOfWeek="Monday" />
                                    <ClientSideEvents DateChanged="function(s,e){ CalcVechime(s); }" />
							    </dx:ASPxDateEdit>										
						    </td>
					    </tr>
				    </table>
			        </fieldset>
                </td>
                </tr>      
			</div>

            <dx:ASPxTextBox ID="txtVechimeCompanie" ClientInstanceName="txtVechimeCompanie" runat="server" ClientVisible="false" Text='<%# Eval("F100643") %>' />
            <dx:ASPxTextBox ID="txtVechimeCarte" ClientInstanceName="txtVechimeCarte" runat="server" ClientVisible="false" Text='<%# Eval("F100644") %>' />

            <dx:ASPxTextBox ID="hfNrLuni" ClientInstanceName="hfNrLuni" runat="server" ClientVisible="false" />
            <dx:ASPxTextBox ID="hfNrAni" ClientInstanceName="hfNrAni" runat="server" ClientVisible="false" />
        </ItemTemplate>
    </asp:DataList>
            </dx:PanelContent>
          </PanelCollection>
        </dx:ASPxCallbackPanel>    
  <dx:ASPxHiddenField runat="server" ID="hfTipAngajat" ClientInstanceName="hfTipAngajat" />
  <dx:ASPxHiddenField runat="server" ID="hfIntRepTM" ClientInstanceName="hfIntRepTM" />
</body>