
var canvas = document.getElementById("heatmap");
var ctx = canvas.getContext("2d");


var cs = getComputedStyle(document.getElementById("heatmap"));
var CVwidth = parseInt(cs.getPropertyValue('width'), 10);
var CVheight = parseInt(cs.getPropertyValue('height'), 10);
canvas.width = CVwidth;
canvas.height = CVheight;

var landingData;

var colours = ["rgba(255, 0, 0, 0.65)",
    "rgba(0, 255, 0, 0.65)",
    "rgba(0, 0, 255, 0.65)",
    "rgba(128, 0, 128, 0.75)"]

var colours2 = ["rgba(255, 0, 0, 0.1)",
    "rgba(0, 255, 0, 0.1)",
    "rgba(0, 0, 255, 0.1)",
    "rgba(128, 0, 128, 0.1)"]

var meEnabled = [false, false, false, false];

var alphas = [1, 1, 1, 1];
var alphasTo = [1, 1, 1, 1];

var circleSize = 30;

function updateCanvas(onlyClear) {


    ctx.clearRect(0, 0, canvas.width, canvas.height);

    if (landingData == undefined) return;
    if (onlyClear == true) return;

    for (var i = 0; i < landingData.players.length; i++) {
    
        ctx.globalAlpha = alphas[i];

        var grd = ctx.createRadialGradient(0, 0, 0, 0, 0, circleSize);
        grd.addColorStop(0, colours[i]);
        grd.addColorStop(0.7, colours2[i]);
        grd.addColorStop(1, "rgba(255,255,255,0)");


        landingData.players[i].dataPoints.forEach(function (point) {
            ctx.translate((point.x / 1000.0) * canvas.width, (point.y / 1000.0) * canvas.height);

            ctx.beginPath();
            ctx.arc(0, 0, circleSize, 0 * Math.PI, 2 * Math.PI)
            ctx.fillStyle = grd;
            ctx.fill();

            ctx.translate(-(point.x / 1000.0) * canvas.width, -(point.y / 1000.0) * canvas.height);
        });
    }
}

setInterval(function () {
    for (var i = 0; i < 4; i++) {
        if (alphas[i] > alphasTo[i]) {
            alphas[i] -= 0.13;
            if (alphas[i] < alphasTo[i]) alphas[i] = alphasTo[i];
        } else {
            alphas[i] += 0.13;
            if (alphas[i] > alphasTo[i]) alphas[i] = alphasTo[i];
        }

        updateCanvas(false);
    }
}, 20);