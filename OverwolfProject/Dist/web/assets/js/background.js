var mainWindow; //Contains a reference to the mainWindow (In game)

var plugin = new OverwolfPlugin("AlecaPUBGWrapper", true);

var g_interestedInFeatures = [
    'match',
    'rank',
    'location',
    'me',
    'team',
    'phase',
    'map',
    'roster',
    'kill',
    'death',
    'killer',
    'match'
];

var status = "none";
var currentTeam = "[]";
var currentMap = "";

var myName;


var alreadyRequesting = false;
var soloDetected = false;

var newGameForHeatmap = false;

var skipPlayerCheck = false; //True if too much time has passed. Skips the full team check

function setFeatures() {
    setTimeout(function () { enableRecording(); }, 3000);
    overwolf.games.events.setRequiredFeatures(g_interestedInFeatures, function (info) {
        if (info.status == "error") {
            /*console.log("Could not set required features: " + info.reason);
            console.log("Trying in 2 seconds");*/
            window.setTimeout(setFeatures, 2000);
            return;
        }

        console.log("Set required features:");
        console.log(JSON.stringify(info));
    });
}

setFeatures();

hideMain();

overwolf.games.events.onInfoUpdates2.addListener(function (infoUpdateChange) {
    //console.log("Info Update: " + JSON.stringify(infoUpdateChange));
    switch (infoUpdateChange.feature) {
        case "phase":
            lastPhase = infoUpdateChange.info.game_info.phase;
            switch (infoUpdateChange.info.game_info.phase) {
                case "loading_screen":
                    teamMembers = "[]";
                    currentMap = "";
                    alreadyRequesting = false;
                    soloDetected = false;
                    console.log("Detected loading screen!");
                    skipPlayerCheck = false;
                    hideEverything();
                 
                    break;
                case "airfield":
                    status = "loading";
                    console.log("Detected airfield!");
                                      

                    setTimeout(function () { showLoader();}, 10);  
                    
                    setTimeout(requestGame, 7000);    
                    setTimeout(function () {
                        skipPlayerCheck = true;
                    }, 10000); //If 30 secs, it won´t wait anymore for a team
                    break;
                case "landed":
                    console.log("Player landed");

                    if (recording) {
                        extendTime = true;
                    } else {
                        autoRecordClip(5000, 5000);
                    }
                    if (getEventImportance("landed") >= getEventImportance(currentEventType)) {
                        afterFinishCallback = function () {
                            currentEventType = "landed";
                            plugin.get().addNewEvent(lastVideoPath, "landed", "", function (res) { });
                            console.log("Addd landedEvent");
                        };
                    }

                    break;
                case "freefly":
                    console.log("Player flying");
                    
                    plugin.get().addNewEvent(lastVideoPath, "airJump", "", function (res) { });
                    
                    break;
                case "airfield":
                   /* setTimeout(function () {
                        if (alreadyRequesting == false) { //In solo mode, there is no teams event, so we launch it form here
                           currentTeam = ["asddas"];
                            status = "playing";
                            soloDetected = true;
                            alreadyRequesting = true;
                          //  requestGame();
                        }
                    }, 5000);*/
                    break;
            }
            console.log("Changed phase to " + infoUpdateChange.info.game_info.phase);
            break;
        case "map":

                console.log("Detected map: " + infoUpdateChange.info.match_info.map);
                currentMap = infoUpdateChange.info.match_info.map;
            //}

            break;
        case "roster":

            break;
        case "location":
            if (lastGameRequestedSuccessfully == false) {
                console.log("Skipping location event since the game was not requested successfully!");
                return;
            }
            var loc = JSON.parse(infoUpdateChange.info.game_info.location);
            plugin.get().setLastPosition(loc.x+"", loc.y+"", loc.z+"", function (res) { });
            break;
        case "team":
            var team = JSON.parse(infoUpdateChange.info.match_info.nicknames).team_members;
            if (team.length > 0) {
                if (status == "loading") {
                    console.log("Team detected: " + JSON.stringify(team));
                    currentTeam = team;
                    status = "playing";
                    alreadyRequesting = true; //So that the solo mode stops working
                   // requestGame();
                }
              
            }
            break;
        case "me":
            if (lastPhase == "landed" && infoUpdateChange.info.me.inVehicle!=undefined) {
                if (infoUpdateChange.info.me.inVehicle != lastVehicle) {
                    lastVehicle = infoUpdateChange.info.me.inVehicle;
                    if (infoUpdateChange.info.me.inVehicle == "true") {
                        console.log("Entered vehicle");
                        plugin.get().addNewEvent("", "inVehicle", "", function (res) { });

                    } else {
                        console.log("Exited vehicle");
                        plugin.get().addNewEvent("", "outVehicle", "", function (res) { });

                    }
                }
            }            
            break;
        case "kill":

            break;
        case "match":
            var info = infoUpdateChange.info.match_info;
            if (info.match_id != undefined) {
                if (lastGameRequestedSuccessfully == false) {
                    console.log("Skipping matchID event since the game was not requested successfully!");
                    return;
                }
                plugin.get().addNewEvent("", "id", info.match_id, function (res) { });
                console.log("Got game ID!");
            }
            break;
        default:
            console.log("Uknown infoUpdate type: " + infoUpdateChange.feature + ". Data: " + JSON.stringify(infoUpdateChange.info));
            break;
    }
});

var lastVehicle = "false";

var lastPhase="";


overwolf.games.onGameLaunched.addListener(
    function (value) {
        console.log("Game started: " + value);
        if (value.id == "109061") {  //PUBG launched
            setFeatures();
        }
    }
);

function getStoredName() {
    plugin.get().getSettings(function (res, username, lowSpec) {
        return username;
    });
   
}

var lastGameRequestedSuccessfully = false;

function requestGame() {

    
    if (status=="none") {
        console.log("Status seems to be back to none. Cancelling...!")
        return;
    }    

    retryGetInfo(10, 1500, function (info) {
        lastGameRequestedSuccessfully = false;

        var gameMode = info.res.match_info.mode;
        var map = info.res.match_info.map;
        var teamMembers = JSON.stringify(JSON.parse(info.res.match_info.nicknames).team_members);

        if (info.res.me == undefined) {
            myName = getStoredName();
        } else {
            myName = info.res.me.name;
        }
               
        if (info.res.game_info.phase == "lobby") {
            console.log("Player is at the lobby phase, cancelling request!");
            return;
        }
        if (gameMode == "squad" && JSON.parse(teamMembers).length != 4 && skipPlayerCheck==false) {
            console.log("Team mode is squad but less than 4 players detected. Trying again in 1 second...");
            setTimeout(requestGame, 1000);
            return;
        }
        if (gameMode == "duo" && JSON.parse(teamMembers).length != 2 && skipPlayerCheck == false) {
            console.log("Team mode is squad but less than 2 players detected. Trying again in 1 second...");
            setTimeout(requestGame, 1000);
            return;
        }

        if (JSON.parse(teamMembers).length == 0) {
            console.log("Could not find any players. Trying again in 1 second...");
            setTimeout(requestGame, 1000);
            return;
        }

        if (gameMode == undefined) {
            if (JSON.parse(teamMembers).length == 4) gameMode = "squad";
            if (JSON.parse(teamMembers).length == 3) gameMode = "squad";
            if (JSON.parse(teamMembers).length == 2) gameMode = "duo";
            if (JSON.parse(teamMembers).length == 1) gameMode = "solo";
        }

        if (teamMembers == "[]") teamMembers = JSON.stringify([myName]); //If no members detected, use only the player who is playing.

     

        plugin.get().requestNewGame(gameMode, map, teamMembers, function (success) {
            hideLoader();
            if (success == "OK!") {
                lastGameRequestedSuccessfully = true;
                console.log("Successfully requested a new game!");
                newGameForHeatmap = true;
                currentEventType = "";
                showMain();
            } else {
                console.log("Didn´t receive the correct repsonse when requesting a new game from the plugin: "+success);
            }
        })
    });
    
}

function requestTestGame() {
    myName = "alecamar";
    plugin.get().requestNewGame("squad", "Savage_Main", "[\"alecamar\",\"briannmb\",\"EmptyBottleGoGo\",\"DecebaluxRex\"]", function (success) {
        if (success == "OK!") {
            console.log("Successfully requested a new test game!");
            newGameForHeatmap = true;
            currentEventType = "";
            showMain();
        } else {
            console.log("Didn´t receive the correct repsonse when requesting a new test game from the plugin. " + success);
        }
    })
}

function retryGetInfo(numberOfTimesLeft,interval, callback) {
    overwolf.games.events.getInfo(function (a) {
        if (a.status == "error" || (a.res.match_info.mode==undefined && numberOfTimesLeft>0)) {
            console.log("Failed to get infoUpdates, " + numberOfTimesLeft + " retries left!");
            console.log(a);
            if (numberOfTimesLeft > 0) {
                setTimeout(function () { retryGetInfo(numberOfTimesLeft - 1, interval, callback); }, interval);
            } else {
                console.log("numberOfTimesLeft is 0, not trying anymore!");
            }
        } else {
            callback(a);
        }

    })
}

var inGameShown = false;

var isAuth = false;

var currentGameReloadNeeded = false; //If it is set to true, the inGame window will load the current game and set this to false upon initialization



plugin.initialize(function (status) {
    if (status == false) {
        showErrorToast("Plugin couldn't be loaded??");
        return;
    } else {
        plugin.get().initialize(function (response) {  
            if (response != "OK!") console.log("Plugin intialization error!");

            overwolf.windows.onMainWindowRestored.addListener(function () {
                plugin.get().getSettings(function (res, username, lowSpec) {
                    if (username == "#IAmUndefinedLogrono#") {
                        showSettings();
                    } else {
                        showMain();
                    }
                });
            });

        });
    }
    getSourceArg();
});

var sourceArg;

function showSettings() {
    overwolf.windows.obtainDeclaredWindow("Settings", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
}



function getSourceArg() {

    launchSettingsIfNeccessary();

    var args = window.location.href.split('?');

    if (args.length < 2) {
        console.log("Opened from overwolf!");
        sourceArg = "overwolf";

        plugin.get().getSettings(function (res, username, lowSpec) {    
            if (username == "#IAmUndefinedLogrono#") {
                showSettings();   
            } else {
                showMain();
            }                      
        });
        return;
    }

    for (var i = 0; i < args[1].split('&').length; i++) {
        var line = args[1].split('&')[i];
        if (line.split('=')[0] == "source") {
            sourceArg = line.split('=')[1];
            switch (sourceArg) {
                case "odk":
                    console.log("Launched from the developer console!");
                    break;
                case "gamelaunchevent":
                    console.log("Launched from in-game!");
                    plugin.get().getSettings(function (res, username, lowSpec) {
                        if (username == "#IAmUndefinedLogrono#") {
                            showSettings();
                        }
                    });
                    break;
                default:
                  
                    break;
            }
            console.log("Source arg: " + sourceArg);
            break;
        }
    }
}

function launchSettingsIfNeccessary() {
    plugin.get().getSettings(function (res, username, lowSpec) {
        if (username == "#IAmUndefinedLogrono#") {
            showSettings();
        }
    });
}

var mainShown = false;
function togglemain() {
    if (mainShown) {
        hideMain();
  
    } else {
        showMain();
    }
}

var replaysShown = false;
function toggleReplays() {
    if (replaysShown) {
        hideReplays();

    } else {
        showReplays();
    }
}

var heatmapShown = false;
function toggleHeatmap() {
    if (heatmapShown) {
        hideHeatmap();

    } else {
        showHeatmap();
    }
}

function showMain() {
    overwolf.windows.obtainDeclaredWindow("Main", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
    mainShown = true;
}

function hideMain() {
    overwolf.windows.obtainDeclaredWindow("Main", function (arg) {
        overwolf.windows.hide(arg.window.id, function () {
        });
    });
    mainShown = false;
}



function showLoader() {
    overwolf.windows.obtainDeclaredWindow("Loader", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });

}

function hideLoader() {
    overwolf.windows.obtainDeclaredWindow("Loader", function (arg) {
        overwolf.windows.close(arg.window.id, function () {
        });
    });
}

overwolf.games.events.onNewEvents.addListener(function (newEvents) {
    if (lastGameRequestedSuccessfully == false) {
        console.log("Skipping event since the game was not requested successfully!");
        return;
    }
    // console.log("Event: " + JSON.stringify(newEvents));
    newEvents.events.forEach(function (obj) {
        switch (obj.name) {


            case "matchEnded":
                if (extendTime == true) return;
                //Fall to next intended!!!
            case "matchSummary": 
                console.log("Match ended");
                status = "none";

                if (getEventImportance("endOfGame") >= getEventImportance(currentEventType) || recording==false) {
                    currentEventType = "endOfGame";
                    afterFinishCallback = function () {
                        plugin.get().addNewEvent(lastVideoPath, "endOfGame", "", function (res) { });
                        console.log("Addd endOfGameEvent");
                    };
                }     
                if (recording) {
                    extendTime = true;    
                    console.log("Extending recording time because of matchEnded");
                } else {
                    autoRecordClip(10000, 4000);
                }
                                               
                break;
            case "kill":
                console.log("Player killed");

                if (getEventImportance("kill") >= getEventImportance(currentEventType) || recording == false) {
                    afterFinishCallback = function () {
                        currentEventType = "kill";
                        plugin.get().addNewEvent(lastVideoPath, "kill", "", function (res) { });
                        console.log("Addd killEvent");
                    };
                }    
                if (recording) {
                    extendTime = true;
                    console.log("Extending recording time because of playerKilled");
                } else {
                    autoRecordClip(10000, 5000);
                }


                break;
            case "death":
                console.log("Player died");

                if (getEventImportance("death") >= getEventImportance(currentEventType) || recording == false) {
                    afterFinishCallback = function () {
                        currentEventType = "death";
                        plugin.get().addNewEvent(lastVideoPath, "death", "", function (res) { });
                        console.log("Addd deathEvent");
                    };
                }

                if (recording) {
                    extendTime = true;
                    console.log("Extending recording time because of playerDied");
                } else {
                    autoRecordClip(12000, 4000);
                }

                break;

            case "damageTaken":

                if (lastPhase != "landed") return;
                console.log("Player took damage");

                if (getEventImportance("takeDamage") >= getEventImportance(currentEventType) || recording == false) {
                    afterFinishCallback = function () {
                        currentEventType = "takeDamage";
                        plugin.get().addNewEvent(lastVideoPath, "takeDamage", "", function (res) { });
                        console.log("Addd takeDamageEvent");
                    };
                }
                if (recording) {
                    extendTime = true;
                    console.log("Extending recording time because of playerTookDamage");
                } else {
                    autoRecordClip(5000, 5000);
                }


                break;

            case "damage_dealt":


                if (lastPhase != "landed") return;
                console.log("Player dealt damage");

                if (getEventImportance("dealDamage") >= getEventImportance(currentEventType) || recording == false) {
                    afterFinishCallback = function () {
                        currentEventType = "dealDamage";
                        plugin.get().addNewEvent(lastVideoPath, "dealDamage", "", function (res) { });
                        console.log("Addd dealDamageEvent");
                    };
                }

                if (recording) {
                    extendTime = true;
                    console.log("Extending recording time because of playerDealtDamage");
                } else {
                    autoRecordClip(7000, 7000);
                }


                break;
            case "jump":
                break;
            default:
                console.log("Uknown event type: " + obj.name + ". Data: " + JSON.stringify(obj.data));
                break;
        }
    });

});


var currentEventType = "";
var currentEventData = "";

var lastVideoPath;
var recording = false;
var extendTime = false; //Set to true if a video is being recorded and you want to extend the time
var afterFinishCallback;


function getEventImportance(evName) {
    if (evName == "kill") return 3;
    if (evName == "death") return 4;
    if (evName == "takeDamage") return 0;
    if (evName == "dealDamage") return 1;
    if (evName == "land") return 2;
    if (evName == "endOfGame") return 5;

    return -1;
}

function enableRecording()
{        
    overwolf.media.replays.turnOn(recordingSettings, function (result) {
        if (result.status == "success") {            
            console.log("Started the replay system successfully!");
            console.log(result);
        } else {
            console.log("Could not enable recordings: " + result.error);
        }
    });
}

function disableRecording() {
    overwolf.media.replays.turnOff("video", function (result) {
        if (result.status == "success") {
            console.log("Stopped the replay system successfully!");
            console.log(result);
        } else {
            console.log("Could not stop the replay system: " + result.error);
        }
    });
}




function autoRecordClip(preTime, postTime) {
    console.log("Starting a new recording! Reason: " + currentEventType);
    recording = true;
    overwolf.media.replays.startCapture(preTime, function (result) {
        if (result.status == "success") {
            extendTime = false;

            lastVideoPath = result.path;     
            setTimeout(function () {
                stopClip(result.url,postTime);
            }, postTime);
        } else {
            console.log("Error when starting capture: " + result.error);
            recording = false;
        }       
    });
}

function stopClip(id,postTime) {
    if (extendTime == true) {
        console.log("Extending clip extension...");
        extendTime = false;
        setTimeout(function () {
            stopClip(id, postTime);
        }, postTime);
    } else {
        overwolf.media.replays.stopCapture(id, function (result2) {
            if (result2.status == "success") {              
                console.log("Recording stopped! Saved to: " + lastVideoPath);
            } else {
                console.log("Error when stopping capture: " + result2.error);
            }
            try {
                displayNotification();
                afterFinishCallback();
            } catch{ }
            currentEventType = "";
            recording = false;
        });
    }   
}



var recordingSettings = {
    "settings": {
        "video": { "buffer_length": 90000 },
        "audio": {
            "mic": {
                "volume": 100,
                "enabled": true
            },
            "game": {
                "volume": 75,
                "enabled": true
            }
        },
        "peripherals": { "capture_mouse_cursor": "both" }
    }
};

function hideEverything() {
    overwolf.windows.obtainDeclaredWindow("Main", function (arg) {
        overwolf.windows.hide(arg.window.id, function () {
        });
    });

    overwolf.windows.obtainDeclaredWindow("Heatmap", function (arg) {
        overwolf.windows.close(arg.window.id, function () {
        });
    });

    overwolf.windows.obtainDeclaredWindow("Replays", function (arg) {
        overwolf.windows.close(arg.window.id, function () {
        });
    });
    overwolf.windows.obtainDeclaredWindow("Settings", function (arg) {
        overwolf.windows.close(arg.window.id, function () {
        });
    });
}

function showNotification() {
    overwolf.windows.obtainDeclaredWindow("Notification", function (arg) {
        overwolf.windows.restore(arg.window.id, function () {
        });
    });
}

function hideNotification() {
    overwolf.windows.obtainDeclaredWindow("Notification", function (arg) {
        overwolf.windows.close(arg.window.id, function () {
        });
    });
}

function displayNotification() {
    showNotification();
    setTimeout(function () { hideNotification(); },7500);
}
