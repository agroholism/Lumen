using gui

var window = new Form(height=60, width=168)
var holst = new Graphics(60, 168)
var font = new Font(:consolas, 10)

fun main() {
    window.add(holst)
    window.show()
    while true {
        holst.clear()
        holst.string("#DateTime.now", font, Color.Blue, 0, 0)
        holst.refresh()
        sleep(100)
    }
}

main()