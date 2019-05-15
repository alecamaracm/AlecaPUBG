
var plugin = overwolf.windows.getMainWindow().plugin;

justOpened(false);

var progressBarEnabled = true;

var currentProgres = 0;
var animationProgress = 0;

var progressGoal = 0;

overwolf.windows.onStateChanged.addListener(function (event) {

    if (event.window_state == "normal" && event.window_previous_state == "minimized" && event.window_name == "Heatmap") {
        justOpened(true);
    }

});


function justOpened(fromEvent) {
    if (overwolf.windows.getMainWindow().newGameForHeatmap) {
        overwolf.windows.getMainWindow().newGameForHeatmap = false;
        document.getElementsByClassName("heatmap-loading-box")[0].classList.add("active");

        for (var i = 0; i < 4; i++) document.getElementsByClassName("player-name")[i].innerText = "- - -";
        landingData = undefined;
        updateCanvas(true);
        currentProgres = 0;
        animationProgress = 0;
        progressGoal - 0;
        document.getElementsByClassName("heatmap-warning")[0].classList.add("hidden");
        progressBarEnabled = true;
        requestNewHeatMap();
    } else {
        if (fromEvent == true && overwolf.windows.getMainWindow().status=="none")document.getElementsByClassName("heatmap-warning")[0].classList.remove("hidden");
    }

}

plugin.get().heatmapDataReady.addListener(function (data) {
    console.log("Got landing data: " + data);
    progressBarEnabled = false;
    document.getElementsByClassName("heatmap-loading-box")[0].classList.remove("active");
    landingData = JSON.parse(data);

    for (var i = 0; i < landingData.players.length; i++) document.getElementsByClassName("player-name")[i].innerText = landingData.players[i].name;



    updateCanvas(false);
});

function requestNewHeatMap() {
    console.log("Requesting new heatmap!");
    plugin.get().requestHeatmapData(function (map) {
        document.getElementsByClassName("heatmap-map-box")[0].className = "heatmap-map-box map" + map;
        document.getElementById("mapTitle").innerText = map;

        switch (map) {
            case "Sanhok":
                document.getElementsByClassName("map-size")[0].childNodes[2].innerText = "4km x 4km (Small)";
                break;
            case "Miramar":
                document.getElementsByClassName("map-size")[0].childNodes[2].innerText = "8km x 8km (Large)";
                break;
            case "Vikendi":
                document.getElementsByClassName("map-size")[0].childNodes[2].innerText = "6km x 6km (Medium)";
                break;
            case "Erangel":
                document.getElementsByClassName("map-size")[0].childNodes[2].innerText = "8km x 8km (Large)";
                break;
            default:
                document.getElementsByClassName("map-size")[0].childNodes[2].innerText = "-km x -km";
                break;
        }
    });
}

$(".player").on({
    'mouseenter': selectPlayer,
    'mouseleave': unselectPlayer
});

function selectPlayer(e) {
    if (e.target.getAttribute("index") < 0 || e.target.getAttribute("index") > 3 || e.target.getAttribute("index") == undefined || e.target.getAttribute("index") == "") return;
    alphasTo[0] = 0;
    alphasTo[1] = 0;
    alphasTo[2] = 0;
    alphasTo[3] = 0;
    alphasTo[e.target.getAttribute("index")] = 1;
    updateCanvas(false);
    e.stopPropagation();
}

function unselectPlayer(e) {
    alphasTo[0] = 1;
    alphasTo[1] = 1;
    alphasTo[2] = 1;
    alphasTo[3] = 1;
    updateCanvas(false);
    e.stopPropagation();
}


