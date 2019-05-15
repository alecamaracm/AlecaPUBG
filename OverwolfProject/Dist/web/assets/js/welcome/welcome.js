
var plugin = overwolf.windows.getMainWindow().plugin;

var lastListData;

function shouldClose() {
    if (document.getElementById("username").value == "") {
        document.getElementsByClassName("w-warning")[0].classList.add("show-warning");

        setTimeout(function () { document.getElementsByClassName("w-warning")[0].classList.remove("show-warning"); }, 3000);
    } else {
        exitScreen();
    }
}


plugin.get().getSettings(function (res, username, lowSpec) {
    if (username == "#IAmUndefinedLogrono#") {
        document.getElementById("username").value = "";
    } else {
        document.getElementById("username").value = username;
    }
});

function exitScreen() {
    if (document.getElementById("username").value == "") return;

    document.getElementsByClassName("w-warning")[0].classList.remove("show-warning");

    plugin.get().getSettings(function (res, oldname, lowSpecX) {
        plugin.get().setSettings(document.getElementById("username").value, "false", function (result) {
            if (result == "OK!") {
                if (oldname != document.getElementById("username").value) overwolf.windows.getMainWindow().showMain();
                closeWindow();
            }
        });
    });

   


}


overwolf.settings.getHotKey("alecapubg_showhide", function (res) {
    document.getElementById("hotkeySpan").innerText = res.hotkey;
});

overwolf.settings.OnHotKeyChanged.addListener(function (res) {
    if (res.source == "alecapubg_showhide") {
        document.getElementById("hotkeySpan").innerText = res.hotkey;
    }
});