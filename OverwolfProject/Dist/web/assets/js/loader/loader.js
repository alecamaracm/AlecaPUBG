
var canvas = document.getElementById("progress-bar");
var ctx = canvas.getContext("2d");

updateProgressBar();

var progress = 0.0;

setInterval(update, 20);

function update() {
    progress += 100.0 / (20 * 50);
    if (progress > 100) {
        progress = 100;
        clearInterval(update)
    }
    updateProgressBar();
}

function updateProgressBar() {
    
    ctx.fillStyle = "#194c8d";
    ctx.fillRect(2, 12, 295*(progress/100), 108);
}

function closeThisPls() {
    close();
}

overwolf.settings.getHotKey("alecapubg_showhide", function (res) {
    document.getElementById("hotkey").innerText = res.hotkey;
});

overwolf.settings.OnHotKeyChanged.addListener(function (res) {
    if (res.source == "alecapubg_showhide") {
        document.getElementById("hotkey").innerText = res.hotkey;
    }
});