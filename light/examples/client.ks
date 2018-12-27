using socket

var port: Number = input("Port: ").to_n()

while true {
    var client: TCPSocket = TCPSocket.client("localhost", port)
    client.connect()
    client.send(input("Input data: "))
    print("Result: #{client.gets()}")
}