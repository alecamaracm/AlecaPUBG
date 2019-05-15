
//var plugin = overwolf.windows.getMainWindow().plugin;



var count = 0;

setInterval(function () {
    ChangeSize();
}, 20);


function ChangeSize() {
    if (count <= 20) {
        document.getElementsByClassName("n-container")[0].style.transform = "translate(-" + Math.round(410-(410/20)*count)+"px)";
    }
    if (count >= 250 && count<300) {
        document.getElementsByClassName("n-container")[0].style.transform = "translate(-" + Math.round((410 / 20) * (count-250)) + "px)";
    }
    if(count<310)count++;
}