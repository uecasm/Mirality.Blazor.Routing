var component = null;

var unloadhandler = function(event) {
    event.preventDefault();
    event.returnValue = "There are unsaved changes on this page.  Do you want to leave?";
    component.invokeMethodAsync("NotifyExitAttempt");
}

export function lock(cb) {
    window.addEventListener("beforeunload", unloadhandler);
    component = cb;
}

export function unlock() {
    window.removeEventListener("beforeunload", unloadhandler);
    component = null;
}