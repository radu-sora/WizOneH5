var __reqSec = "";
var __colGrid = "";

function ctx(s, e)
{
    if (e.objectType == "header")
    {
        var idCtl = "grDate";
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
     
        debugger;
        if (targ.id == "pnlContent1_ContentPlaceHolder1_ASPxPageControl2") {
            if (e.srcElement.innerText != "")
                __reqSec = "IdControl=" + e.srcElement.innerText;
            else
                __reqSec = "IdControl=" + e.srcElement.id;
        }
        else
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
                callBackProfile.PerformCallback();
            }
            else {
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
