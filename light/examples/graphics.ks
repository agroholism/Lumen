using gui

const GRID_SIZE = 25
const WINDOW_HEIGHT = 500
const WINDOW_WIDTH = 366
const FONT = new Font(name=:consolas, size=10, style=:regular)
const RANGE = -5..10

var window = new Form(height=WINDOW_HEIGHT, width=WINDOW_WIDTH)
var holst = new Graphics(WINDOW_HEIGHT, WINDOW_WIDTH)

fun sin(i) {
    return [i * 50+15, Math.sin(i) * 50 + 200]
}

fun cos(i) {
    return [i * 50+18, Math.cos(i) * 50 + 200]
}

fun sqr(i) {
    return [i * 50+175, -(i**2) * 50 + 200]
}

fun tg(i) {
    return [i * 50+175, Math.tan(i) * 50 + 200]
}

fun exp(i) {
    return [i * 50+175, Math.exp(i) * 50 + 200]
}

fun mexp(i) {
    return [i * 50+175, -Math.exp(i) * 50 + 200]
}

fun draw_grid() {
    for i in 0..50 {
        holst.line(Color.Blue, i * GRID_SIZE, 0, i * GRID_SIZE, WINDOW_HEIGHT)
        holst.line(Color.Blue, 0, i * GRID_SIZE, WINDOW_WIDTH, i * GRID_SIZE)
    }
}

fun main() {
    window.add(holst)
    holst.clear()
    draw_grid()
    holst.curve(Color.Red, ...RANGE.each(sin))
    holst.curve(Color.Green, ...RANGE.each(cos))
    holst.curve(Color.Pink, ...RANGE.each(sqr))
    holst.curve(Color.Yellow, ...RANGE.each(tg))
    holst.curve(Color.Black, ...RANGE.each(exp))
    holst.curve(Color.Black, ...RANGE.each(mexp))
    holst.string("O", FONT, Color.Black, 175, 200)
    #holst.string("y = sin(x)", FONT, Color.Red, 35, 0)
    #holst.string("y = cos(x)", FONT, Color.Green, 357, 0)
    window.show_dialog()
}

main()