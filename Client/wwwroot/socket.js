var socket;

export function connect(dotnethelper, url) {
    socket = new WebSocket(url);
    console.log(url)
    socket.onmessage = async function (event) {
        const text = await new Response(event.data).text()
        dotnethelper.invokeMethodAsync('MessageReceived', text).then(() => {
            // success, do nothing
        }).catch(error => {
            console.log("Error during browser resize: " + error);
        });
    };
}

export function close() {
    if (socket && socket.readyState === WebSocket.OPEN) {
        socket.close(1000, "Closing from client");
    }
}
