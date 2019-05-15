
var plugin = overwolf.windows.getMainWindow().plugin;

var lastListData;

getGames();

function getGames()
{
    document.getElementsByClassName("loading")[0].classList.add("active");
    plugin.get().getRecentGames(function (result, dataStr) {
        if (result == "OK!") { //This means the window just opened and there is a match waiting to be shown

            lastListData = JSON.parse(dataStr);
            console.log("Got match list data.");
            console.log(lastListData);

            processGameList();

            document.getElementsByClassName("loading")[0].classList.remove("active");
        }
    });
}

function processGameList() {
    while (document.getElementsByClassName("replays-box").length > 1) { //Let the last iitem (the original one) stay
        document.getElementsByClassName("replays-box")[0].remove(); 
    }

    for (var i = 0; i < lastListData.games.length; i++) {
        var game = lastListData.games[i];
        var gameNode = document.getElementById("listGameToCopy").cloneNode(true);
        document.getElementsByClassName("replays-games")[0].appendChild(gameNode);
        gameNode.classList.remove("hidden");

        gameNode.getElementsByClassName("replays-place")[0].innerText = game.ranking;
        gameNode.getElementsByClassName("replays-game-mode")[0].innerText = game.mode;
        gameNode.getElementsByClassName("replays-box-year")[0].innerText = game.date;
        gameNode.getElementsByClassName("replays-box-time")[0].innerText = game.time;
        gameNode.getElementsByClassName("replays-box-icon")[0].children[0].setAttribute("src", "assets/img/maps/" + game.mapName + ".jpg");

        if (game.detailed) {
            gameNode.getElementsByClassName("replays-warning-icon")[0].classList.add("hidden");
        } else {
            gameNode.getElementsByClassName("replays-warning-icon")[0].classList.remove("hidden");
        }
       

        gameNode.setAttribute("index", i);
    }
}

var lastMapData;

function elementClicked(e) {

    document.getElementById('replaysPopup').classList.add('hidden');

    var count = 0;
    var cNode = e.target;

    //Max 10 times deep to avoid an infinite loop
    while (cNode.classList.contains("replays-box") == false && count < 10) {
        cNode = cNode.parentNode;
        count++;
    }

    Array.prototype.forEach.call(document.getElementsByClassName("replays-box"),function (ele) {
        if(ele!=cNode) ele.classList.remove("clicked");
    });
    cNode.classList.add("clicked");

    var game = lastListData.games[cNode.getAttribute("index")];

    document.getElementsByClassName("replays-map-name")[0].innerText = game.mapName;
    document.getElementsByClassName("replays-placed-num")[0].innerText = game.ranking;
    document.getElementsByClassName("replays-placed-mode")[0].innerText = game.mode;
    document.getElementsByClassName("replays-killed-number")[1].innerText = game.kills;
    document.getElementsByClassName("replays-distance-number")[1].innerText = game.distance;
    document.getElementsByClassName("replays-date-year")[0].innerText = game.date;
    document.getElementsByClassName("replays-date-time")[0].innerText = game.time;

    document.getElementById("loading2").classList.add("active");
    document.getElementsByClassName("replays-warning")[0].classList.add("hidden");

    plugin.get().getMapReplay(game.id, function (res, datares) {
        if (res == "OK!") {
            document.getElementsByClassName("replays-warning")[0].classList.add("hidden");

            lastMapData = JSON.parse(datares);
            console.log(lastMapData);
            updateMapPreview(game);

        } else {
            console.log("Error when requesting map events!");
            lastMapData = undefined;
            updateMapPreview(game);
            document.getElementsByClassName("replays-warning")[0].classList.remove("hidden");
        }
        document.getElementById("loading2").classList.remove("active");
    });
}

function markerClicked(e) {
    var count = 0;
    var cNode = e.target;

    //Max 10 times deep to avoid an infinite loop
    while (cNode.classList.contains("map-pin") == false && count < 10) {
        cNode = cNode.parentNode;
        count++;
    }

    var currentEvent = lastMapData.bigEvents[cNode.getAttribute("index")];

    document.getElementsByClassName("replays-popup")[0].classList.remove("hidden");
    document.getElementsByClassName("replays-video")[0].firstElementChild.setAttribute("src", currentEvent.videoFile);

    switch (currentEvent.type) {
        case "landed":
            document.getElementsByClassName("event-type")[0].innerText = "Landing";
            break;
        case "endOfGame":
            document.getElementsByClassName("event-type")[0].innerText = "End of the game";
            break;
        case "takeDamage":
            document.getElementsByClassName("event-type")[0].innerText = "Damage taken";
            break;
        case "dealDamage":
            document.getElementsByClassName("event-type")[0].innerText = "Damage dealt";
            break;
        case "kill":
            document.getElementsByClassName("event-type")[0].innerText = "Kill";
            break;
        default:
            document.getElementsByClassName("event-type")[0].innerText = currentEvent.type;
    }
    

}

function updateMapPreview(game) {

    var positionInfo = document.getElementsByClassName("replays-screen")[0].getBoundingClientRect();   
    var width = positionInfo.width - 2;

    document.getElementsByClassName("replays-screen-content")[0].style.backgroundImage = "url(assets/img/maps/" + game.mapName + ".jpg)"; 

    while (document.getElementsByClassName("map-pin").length > 1) { //Let the first iitem (the original one) stay
        document.getElementsByClassName("map-pin")[1].remove();
    }

    if (lastMapData != undefined) {
      

        for (var i = 0; i < lastMapData.bigEvents.length; i++) {
            var markerNode = document.getElementsByClassName("map-pin")[0].cloneNode(true);

            document.getElementsByClassName("replays-map-markers")[0].appendChild(markerNode);

            markerNode.style.left = (((lastMapData.bigEvents[i].point.x / 1000.0) * width - 25) + "px");
            markerNode.style.top = (((lastMapData.bigEvents[i].point.y / 1000.0) * width - 56) + "px");
            markerNode.setAttribute("index", i);
            markerNode.childNodes[3].setAttribute("src", "assets/img/icons/" + lastMapData.bigEvents[i].type + ".png");
            markerNode.classList.remove("hidden");



        }
    }

   
  

    drawCanvas();

    document.getElementsByClassName("replays-screen-content")[0].classList.remove("hidden");
}


var c = document.getElementById("mapOverlay");
var ctx = c.getContext("2d");

function drawCanvas() {


    var positionInfo = document.getElementsByClassName("replays-screen")[0].getBoundingClientRect();
    var width = positionInfo.width - 2;
    c.width = width;
    c.height = width;
    ctx.clearRect(0, 0, c.width, c.height);

    if (lastMapData == undefined) return;

    ctx.lineWidth = 5;
    ctx.strokeStyle = "#2538aa";
    ctx.setLineDash([13, 8]);
    if (lastMapData.planelocations.length > 0) {
        ctx.beginPath();
        ctx.moveTo((lastMapData.planelocations[0].x / 1000.0) * width, (lastMapData.planelocations[0].y / 1000.0) * width);
        for (var i = 1; i < lastMapData.planelocations.length; i++) {
            ctx.lineTo((lastMapData.planelocations[i].x / 1000.0) * width, (lastMapData.planelocations[i].y / 1000.0) * width);
        }
        ctx.stroke();
    }

    ctx.lineWidth = 5;
    ctx.strokeStyle = "#0080FF";
    ctx.setLineDash([]);
    if (lastMapData.flyLocations.length > 0) {
        ctx.beginPath();
        ctx.moveTo((lastMapData.flyLocations[0].x / 1000.0) * width, (lastMapData.flyLocations[0].y / 1000.0) * width);
        for (var i = 1; i < lastMapData.flyLocations.length; i++) {
            ctx.lineTo((lastMapData.flyLocations[i].x / 1000.0) * width, (lastMapData.flyLocations[i].y / 1000.0) * width);
        }
        ctx.stroke();
    }

    var lastLocationType = undefined;


    if (lastMapData.locations.length > 0) {

        for (var i = 1; i < lastMapData.locations.length; i++) {
            if (lastLocationType != lastMapData.locationsVehicle[i]) {
                lastLocationType = lastMapData.locationsVehicle[i];
                try { ctx.stroke(); } catch{ }
                ctx.beginPath();
                if (lastMapData.locationsVehicle[i] == true) { //Is in vehicle
                    ctx.lineWidth = 5;
                    ctx.setLineDash([7, 7]);
                    ctx.strokeStyle = "#e8e8e8";
                } else {
                    ctx.lineWidth = 5;
                    ctx.setLineDash([]);
                    ctx.strokeStyle = "#FFFFFF";
                }
                ctx.moveTo((lastMapData.locations[i].x / 1000.0) * width, (lastMapData.locations[i].y / 1000.0) * width);


            }
            ctx.lineTo((lastMapData.locations[i].x / 1000.0) * width, (lastMapData.locations[i].y / 1000.0) * width);
        }

        ctx.stroke();
    }
   
}


if (localStorage["hideReplaysTip"] == "true") {
    document.getElementsByClassName("replays-tip")[0].classList.add("hidden");
}

function hideTip() {
    localStorage["hideReplaysTip"] = "true";
    document.getElementsByClassName("replays-tip")[0].classList.add("hidden");    
}

