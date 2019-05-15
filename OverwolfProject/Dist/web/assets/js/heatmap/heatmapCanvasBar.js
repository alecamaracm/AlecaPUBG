
var canvas2 = document.getElementById("heatmap-loader");
var ctx2 = canvas2.getContext("2d");


var cs2 = getComputedStyle(document.getElementsByClassName("heatmap-loading")[0]);
var CVwidth2 = parseInt(cs2.getPropertyValue('width'), 10);
var CVheight2 = parseInt(cs2.getPropertyValue('height'), 10);


setInterval(updateCanvas2, 20); 



var grd2 = ctx.createLinearGradient(0, 0, 140, 0);
grd2.addColorStop(0, "#1976d2");
grd2.addColorStop(0.5, "white");
grd2.addColorStop(1, "#1976d2");

function updateCanvas2() {

    if (progressGoal > currentProgres)currentProgres += 0.001;
    if (currentProgres > 1) currentProgres = 1;

    if (progressBarEnabled == false) return;

    animationProgress += 2;
    if (animationProgress > canvas2.width*currentProgres) animationProgress = -140;

    ctx2.clearRect(0, 0, canvas.width, canvas.height);

    ctx2.translate(animationProgress / currentProgres, 0);
        
    ctx2.fillStyle = grd2;
    ctx2.fillRect(0 - animationProgress / currentProgres, 2, (canvas.width - 2) * currentProgres, 17);
   
    ctx2.translate(-animationProgress / currentProgres, 0);
}

plugin.get().heatmapProgressReady.addListener(function (data, total) {
    if ((data / total) * 1.45 > progressGoal) {
        progressGoal = (data / total)*1.45;
    }
    if (progressGoal > 1) progressGoal = 1;
});