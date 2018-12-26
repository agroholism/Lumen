using gui

fun create(height, width) {
    var window = new Form(height=height, width=width)
    var holst = new Graphics(height, width)
    window.add(holst)
    window.show()
    return holst
}
