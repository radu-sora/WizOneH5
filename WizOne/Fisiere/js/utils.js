var __reqSec = "";
var __colGrid = "";
var __sourceId = "";

function ctx(s, e)
{ 
    if (e.objectType == "header")
    {
        //var idCtl = "grDate";
        var idCtl = e.htmlEvent.currentTarget.id.split("_")[0];
        __sourceId = idCtl;
        __reqSec = "IdControl=" + idCtl + "&IdColoana=" + s.columns[e.index].fieldName;
        __colGrid = s.columns[e.index].fieldName;
        mnuCtx.ShowAtPos(ASPxClientUtils.GetEventX(e.htmlEvent), ASPxClientUtils.GetEventY(e.htmlEvent));
    }
    else
    {
        e = e || window.event;
        var targ = e.currentTarget || e.target || e.srcElement;
        if (targ.nodeType == 3) targ = targ.parentNode;
        e.preventDefault();
     
        

        var sir = ["IdControl", ""];
        if (__reqSec.length > 0)
            sir = __reqSec.split("=");       

        if (targ.id == "pnlContent1_ContentPlaceHolder1_ASPxPageControl2") {
            if (sir[1].length <= 0 || e.srcElement.id.length <= 0 || (e.srcElement.id.length > 0 && e.srcElement.id.indexOf(sir[1]) == -1)) {
                if (e.srcElement.innerText != "") {
                    if ((e.srcElement.id.length >= 3 && e.srcElement.id.substring(0, 3) == "lbl")
                        || (e.srcElement.id.length >= 2 && e.srcElement.id.substring(0, 2) == "lg")
                        || (e.srcElement.id.length >= 3 && e.srcElement.id.substring(0, 3) == "chk")
                        || (e.srcElement.id.length >= 3 && e.srcElement.id.substring(0, 3) == "btn")
                        || (e.srcElement.id.length >= 6 && e.srcElement.id.substring(0, 6) == "grDate") && e.srcElement.id.indexOf('lbl') != -1 )
                        __reqSec = "IdControl=" + e.srcElement.id;
                    else
                        __reqSec = "IdControl=" + e.srcElement.innerText;
                }
                else
                    __reqSec = "IdControl=" + e.srcElement.id;
            }
        }
        else
            if (sir[1].length <= 0 || targ.id.length <= 0 || (targ.id.length > 0 && targ.id.indexOf(sir[1]) == -1)) 
                __reqSec = "IdControl=" + targ.id;
        

        var dc = ASPxClientUtils.GetEventX(e);
        mnuCtx.ShowAtPos(ASPxClientUtils.GetEventX(e), ASPxClientUtils.GetEventY(e));
    }
}


function OnItemPressed(s, e) {
    
    var nme = e.item.name;

    //var idx = e.item.index;
    //popGen.Hide();

    
    var strUrl = "";

    switch (nme) {
        case "securitate":       
            strUrl = getAbsoluteUrl + "/Pagini/Securitate.aspx?" + __reqSec;
            popGen.SetHeaderText("Securitate");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
            break;
        case "notificari":
            //strUrl = "Pagini/Notificari.aspx?Tip=1";
            strUrl = getAbsoluteUrl + '/Pagini/Notificari.aspx?Tip=1';
            popGen.SetHeaderText("Notificari");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
            break;
        case "validari":
            strUrl = getAbsoluteUrl + "/Pagini/Notificari.aspx?Tip=2";
            popGen.SetHeaderText("Validari");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
            break;
        case "alerte":
            strUrl = getAbsoluteUrl + "/Pagini/Notificari.aspx?Tip=3";
            popGen.SetHeaderText("Alerte");
            popGen.SetContentUrl(strUrl);
            popGen.Show();
            break;
        case "profile":
            var tmpDate = document.getElementById("grDate");
            if (tmpDate != null) {
                popGen.SetContentUrl(getAbsoluteUrl + "/Pagini/Profile.aspx");
                popGen.SetHeaderText("Profile");
                popGen.Show();
                callBackProfile.PerformCallback("grDate");
            }
            else {
                var tmpDatePers = document.getElementById(__sourceId);
                if (tmpDatePers != null && __sourceId.substring(0, 6) == "grDate") {
                    popGen.SetContentUrl(getAbsoluteUrl + "/Pagini/Profile.aspx");
                    popGen.SetHeaderText("Profile");
                    popGen.Show();
                    callBackProfile.PerformCallback(__sourceId);
                }
                else
                    swal({ title: '', text: 'Nu exista obiect pentru care sa se seteze profil', type: 'warning' });
            }
            break;
        case "colChooser":
            var tmpDate = document.getElementById("grDate");
            
            if (tmpDate != null) {
                if (grDate.IsCustomizationWindowVisible())
                    grDate.HideCustomizationWindow();
                else
                    grDate.ShowCustomizationWindow();
            }
            break;
        case "colHide":
            var tmpDate = document.getElementById("grDate");

            if (tmpDate != null && __colGrid != "") {
                grDate.PerformCallback("colHide;" + __colGrid);
            }
            break;
    }

}


function OnNewClick(s, e) {
    grDate.AddNewRow();
}


/*LeonardM 20.10.2017
new Drepturi*/
function OnNewClickDrepturi(s, e) {
    grDateDrepturi.AddNewRow();
}


function OnCancelClick(s, e) {
    grDate.CancelEdit();
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
