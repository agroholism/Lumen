using gui

var window = new Form(height=500, width=500)
var graph = new Graphics(500, 500)
window.add(graph)
window.show()

graph.clear()

var j = 0
for i in 20..500 do
    graph.rectangle(Color.rainbow(j), i, 20, i, i)
    graph.refresh()
    sleep(100)
    if j >= 1: j = 0
    else: j = j + 0.01
end
