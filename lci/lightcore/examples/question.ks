using gui
using async
using win32lib

const STATE = new {
    showed? = true
}

fun button_click() {
    var result = 
        msg_box("Вам больше 18 лет?", 
                "Внимание!", 
                MsgBoxTypes.WARN_YES_NO)
    print(if result == MsgBoxResult.YES "All ok!" else "Run!")
}

var window = new Form()
window.add(new Button(
    text="Click me!"
    on_click=button_click
    position=[100, 200]
    back_color=Color.WHITE
    style=:flat
))
set_window_text(window.ptr, "My Win32 caption!")

@async fun design() {
    var j = 0

    while STATE.showed? {
        window.back_color = Color.rainbow(j)
        j = if j >= 1: 0 else: j + 0.01
        sleep(100)
    }
}

design()

window.show_dialog()
STATE.showed? = false