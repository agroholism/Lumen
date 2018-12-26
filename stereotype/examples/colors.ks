using gui

fun on_click(sender)

var window = new Form {
    text = "My first Klischee GUI"
    back_color = Color.Red
    
    new Button { 
        text = "Click me!"
        back_color = Color.White
        position = [100, 200]
        style = :flat
        on_click = on_click
    }
}

fun on_click(sender) {
    once self.j = 0
    window.back_color = Color.rainbow(self.j)
    if self.j >= 1: 
        self.j = 0
    else:
        self.j = self.j + 0.01
}

window.show()