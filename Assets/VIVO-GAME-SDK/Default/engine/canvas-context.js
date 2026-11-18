const callbacks = [];
let isTriggered = false;
const canvasContext = {
    addCreatedListener(callback) {
        if (isTriggered) {
            callback();
        }
        else {
            callbacks.push(callback);
        }
    },
    _triggerCallback() {
        isTriggered = true;
        callbacks.forEach(v => v());
    },
};

window.wasmsdk.canvasContext = canvasContext;