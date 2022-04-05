// listen for page resize
export function resizeListener(dotnethelper) {
    window.onresize = (() => {
        let browserWidth = window.innerWidth;
        dotnethelper.invokeMethodAsync('UpdateWidth', browserWidth).then(() => {
            // success, do nothing
        }).catch(error => {
            console.log("Error during browser resize: " + error);
        });
    });
}

export function currentWidth() {
    return window.innerWidth;
}
