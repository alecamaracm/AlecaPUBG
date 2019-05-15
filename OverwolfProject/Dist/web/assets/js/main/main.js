
var plugin = overwolf.windows.getMainWindow().plugin;

var displayingMatch = false; //This is used to avoid the event and the lastRequest to be used at the same time

plugin.get().requestLastRoasterInfo(function (result, dataStr) {
    if (result == "OK!") { //This means the window just opened and there is a match waiting to be shown

        if (displayingMatch == true) return;

        displayingMatch = true;
        setTimeout(function () {
            displayingMatch = false;
        }, 5000); //In 5 seconds no other match can be shown
        var data = JSON.parse(dataStr);
        console.log("Got match data from lastRequest.");
        console.log(data);

        showMatchResponse(data);
    }
});


plugin.get().gameDataReady.addListener(function (dataStr) {

    if (displayingMatch == true) return;

    displayingMatch = true;
    setTimeout(function () {
        displayingMatch = false;
    }, 5000); //In 5 seconds no other match can be shown

    var data = JSON.parse(dataStr);
    console.log("Got match data from event.");
    console.log(data);

    showMatchResponse(data);
});

function showMatchResponse(data) {

    document.getElementsByClassName("main-menu-item main-menu-btn")[0].classList.remove("heatmap-disabled");
    document.getElementsByClassName("main-players-default")[0].classList.add("hidden");

    var myName = overwolf.windows.getMainWindow().myName;

    //Show all the used players and hide the unused ones
    for (var i = 0; i < 4; i++) {
        if (i > data.length - 1) {
            document.getElementsByClassName("main-player")[i].classList.add("player-hidden");
        } else {
            document.getElementsByClassName("main-player")[i].classList.remove("player-hidden");
        }      
    }

    for (var i = 0; i < data.length; i++) {
        var currentData = data[i];
        var player = document.getElementsByClassName("main-player")[i];

        //Set background picture
        player.getElementsByClassName("main-player-img")[0].firstElementChild.setAttribute("src", "assets/img/Final/"+currentData.skinNumber+".png");

        //Set name stuff
        player.getElementsByClassName("main-player-name")[0].firstElementChild.innerHTML = currentData.unsername;
        if (myName == currentData.unsername) {
            document.getElementsByClassName("main-player-name")[i].classList.add("player-current");
        } else {
            document.getElementsByClassName("main-player-name")[i].classList.remove("player-current");
        }

        //Set wins,losses and winrate
        player.getElementsByClassName("main-wins")[0].lastElementChild.innerText = currentData.wins;
        player.getElementsByClassName("main-losses")[0].lastElementChild.innerText = currentData.losses;
        player.getElementsByClassName("main-percent")[0].innerText = currentData.winPercentage + "%"

        //Set KDA and DMG
        player.getElementsByClassName("main-section-2")[0].firstElementChild.lastElementChild.innerText = currentData.kda
        player.getElementsByClassName("main-section-2")[0].lastElementChild.lastElementChild.innerText = currentData.damagePerGame

        //Set Ranking
        player.getElementsByClassName("main-rank-icon")[0].firstElementChild.setAttribute("src", "assets/img/rankings/" + currentData.rankTitle + ".png");
        player.getElementsByClassName("main-rank-name")[0].innerText = currentData.rankTitle;
        player.getElementsByClassName("main-sp")[0].innerText = currentData.SP;

        //My title
        player.getElementsByClassName("main-rank-title")[0].innerText = currentData.myTitle;
    }
}


function showHeatmap() {
    overwolf.windows.obtainDeclaredWindow("Heatmap", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
}

function hideHeatmap() {
    overwolf.windows.obtainDeclaredWindow("Heatmap", function (arg) {
        overwolf.windows.minimize(arg.window.id, function () {
        });
    });
}

function showReplays() {
    overwolf.windows.obtainDeclaredWindow("Replays", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
}

function hideReplays() {
    overwolf.windows.obtainDeclaredWindow("Replays", function (arg) {
        overwolf.windows.minimize(arg.window.id, function () {
        });
    });
}

function showSettings() {
    overwolf.windows.obtainDeclaredWindow("Settings", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
}

function replaysClicked() {
    showReplays();
}

function heatmapClicked() {

    if (document.getElementsByClassName("main-menu-item main-menu-btn")[0].classList.contains("heatmap-disabled")) return;
    showHeatmap();
}


function settingsClicked() {
    showSettings();
}

overwolf.settings.registerHotKey(
    "alecapubg_showhide",
    function (arg) {
        if (arg.status == "success") {
            overwolf.windows.obtainDeclaredWindow("Main", function (res) {
                if (res.window.stateEx == "normal" || res.window.stateEx == "maximized") {
                    overwolf.windows.obtainDeclaredWindow("Main", function (arg) {
                        overwolf.windows.hide(arg.window.id, function () {
                        });
                    });
                } else {
                    overwolf.windows.obtainDeclaredWindow("Main", function (arg) {
                        overwolf.windows.restore(arg.window.id, function () {
                        });
                    });
                }
            });
        }
    }
);



overwolf.settings.registerHotKey(
    "alecapubg_heatmap_showhide",
    function (arg) {
        if (arg.status == "success") {

            if (document.getElementsByClassName("main-menu-item main-menu-btn")[0].classList.contains("heatmap-disabled")) return;
            overwolf.windows.obtainDeclaredWindow("Heatmap", function (res) {
                if (res.window.stateEx == "normal" || res.window.stateEx == "maximized") {
                    overwolf.windows.obtainDeclaredWindow("Heatmap", function (arg) {
                        overwolf.windows.hide(arg.window.id, function () {
                        });
                    });
                } else {
                    overwolf.windows.obtainDeclaredWindow("Heatmap", function (arg) {
                        overwolf.windows.restore(arg.window.id, function () {
                        });
                    });
                }
            });
        }
    }
);

overwolf.settings.registerHotKey(
    "alecapubg_replays_showhide",
    function (arg) {
        if (arg.status == "success") {
            overwolf.windows.obtainDeclaredWindow("Replays", function (res) {
                if (res.window.stateEx == "normal" || res.window.stateEx == "maximized") {
                    overwolf.windows.obtainDeclaredWindow("Replays", function (arg) {
                        overwolf.windows.close(arg.window.id, function () {
                        });
                    });
                } else {
                    overwolf.windows.obtainDeclaredWindow("Replays", function (arg) {
                        overwolf.windows.restore(arg.window.id, function () {
                        });
                    });
                }
            });
        }
    }
);

setMenuHotkeys();

function setMenuHotkeys() {
    overwolf.settings.getHotKey("alecapubg_heatmap_showhide", function (res) {
        document.getElementsByClassName("main-menu-item-title")[0].innerText = "Landing heatmap (" + res.hotkey + ")";

    });

    overwolf.settings.getHotKey("alecapubg_replays_showhide", function (res) {
        document.getElementsByClassName("main-menu-item-title")[1].innerText = "Match replays (" + res.hotkey + ")";
    });

    overwolf.settings.getHotKey("alecapubg_showhide", function (res) {
        document.getElementsByClassName("main_hotkey")[0].innerText = res.hotkey;
    });
}

overwolf.settings.OnHotKeyChanged.addListener(function (res) {
    if (res.source == "alecapubg_heatmap_showhide") {
        document.getElementsByClassName("main-menu-item-title")[0].innerText = "Landing heatmap (" + res.hotkey + ")";
    }
    if (res.source == "alecapubg_replays_showhide") {
        document.getElementsByClassName("main-menu-item-title")[1].innerText = "Landing heatmap (" + res.hotkey + ")";
    }
    if (res.source == "alecapubg_showhide") {
        document.getElementsByClassName("main_hotkey")[0].innerText = res.hotkey;
    }
});




main_hotkey