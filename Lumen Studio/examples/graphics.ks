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
    holst.String("O", FONT, Color.Black, 175, 200)
    #holst.String("y = sin(x)", FONT, Color.Red, 35, 0)
    #holst.String("y = cos(x)", FONT, Color.Green, 357, 0)
    window.show_dialog()
}

main()

#ref visual
open Visual

type _ = Counter x 

let counter = Counter 0

let counterLabel = Label.create ()
counterLabel.position [| 100; 50 |]
counterLabel.text ("Counter = " + counter.x)
counterLabel.foreColor Color.Red

let refresh () =
    counterLabel.text ("Counter = " + counter.x)

let makeButton text x y onClick =
    let res = Button.create ()
    res.style Style.Flat
    res.position [| x; y |]
    res.text text
    res.onClick onClick
    res

let incBtn = makeButton "+" 50 200 b -> 
    counter.x <- counter.x + 1
    refresh ()

let decBtn = makeButton "-" 150 200 b -> 
    counter.x <- counter.x - 1
    refresh ()

let form = Form.create ()
form.addButton incBtn
form.addButton decBtn
form.addLabel counterLabel
form.show ()