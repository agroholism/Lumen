using gui

var window = Form.new(height=340 width=317)
var holst = Graphics.new(300, 300)
window.add(holst)
holst.clear()
var h = 300
var w = 300
var cRe = -0.74543
var cIm = -0.11301
var newRe = 0
var newIm = 0
var oldRe = 0
var oldIm = 0
var zoom = 2
var moveX = 0
var moveY = 0
var maxIterations = 300
var x = 0
while x < w {
    var y = 0
    while y < h {
        newRe = ((1.5 * (x - w / 2)) / (0.5 * zoom * w)) + moveX
        newIm = ((y - h / 2) / (0.5 * zoom * h)) + moveY
        var i = 0
        while i < maxIterations {
            oldRe = newRe
            oldIm = newIm
            newRe = ((oldRe * oldRe) - (oldIm * oldIm)) + cRe 
            newIm = (2 * oldRe * oldIm) + cIm
            if (newRe * newRe + newIm * newIm) > 4: break 1
            i = i + 1
        }
        holst.rectangle(Color.rgb(0, (i * 9) % 255, (i * 9) % 255), x, y, 1, 1)
        y += 1
    }
    x += 1
}
window.show()