using socket
using async

var port = input("Port: ").to_n()
var server = TCPSocket.server("localhost", port)

var state = new { i = 0 }

while true {
    if state.i < 5 {
        (@async () => {
            var handler = server.accept()
            var data = handler.gets()
            print("Input: #data")
            handler.send(data.reverse())
            handler.shutdown()
            handler.close()
            state.i = state.i - 1
        })()
        state.i = state.i + 1 
    }
    sleep(100)
}