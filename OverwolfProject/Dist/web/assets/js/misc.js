function dragMove() {
    overwolf.windows.getCurrentWindow(function (result) {
        if (result.status == "success" && result.window.state !== "Maximized") {
            overwolf.windows.dragMove(result.window.id);
        }
    });
};

function closeWindow() {
    overwolf.windows.getCurrentWindow(function (result) {
        if (result.status == "success") {
            overwolf.windows.close(result.window.id);
        }
    });
};

function minimize() {
    overwolf.windows.getCurrentWindow(function (result) {
        if (result.status == "success") {
            overwolf.windows.minimize(result.window.id);
        }
    });
};

function hide() {
    overwolf.windows.getCurrentWindow(function (result) {
        if (result.status == "success") {
            overwolf.windows.hide(result.window.id);
        }
    });
};

function showSupport() {
    window.location.href = "overwolf://settings/support";
};