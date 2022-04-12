var Timer = setTimeout(function () { }, 20000);

function ResetTimeout() {
    if (Timer) {
        clearTimeout(Timer);
        Timer = setTimeout(function () { location.href = getAbsoluteUrl + "/Default.aspx"; }, 10000);
    }
    else
        Timer = setTimeout(function () { location.href = getAbsoluteUrl + "/Default.aspx"; }, 10000);
}

if (navigator.userAgent.indexOf('MSIE') > -1)
    document.onmousemove = ResetTimeout;
else
    window.onmousemove = ResetTimeout;
window.onclick = ResetTimeout;


//location.href = "http:\\www.google.ro";