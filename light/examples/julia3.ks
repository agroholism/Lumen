using gui

const HEIGHT = 900
const WIDTH = 900
const RE = -0.74543
const IM = -0.11301
const ZOOM = 1
const MAX_ITERATIONS = 30000
const WINDOW = new Form(height=HEIGHT + 40, width=WIDTH + 17)
const HOLST = new Graphics(HEIGHT, WIDTH)

var (newRe, newIm, oldRe, oldIm, x) = [0] * 5

while x < WIDTH {
    var y = 0
    while y < HEIGHT {
        (newRe, newIm) = [1.5 * (x - WIDTH / 2) / (0.5 * ZOOM * WIDTH), (y - HEIGHT / 2) / (0.5 * ZOOM * HEIGHT)]
        var (i, j) = [0, 0]
        while i < MAX_ITERATIONS {
            (oldRe, oldIm) = [newRe, newIm]
            (newRe, newIm) = [oldRe * oldRe - oldIm * oldIm + RE, 2 * oldRe * oldIm + IM]
            if (newRe * newRe + newIm * newIm) > 4: break 1
            (i, j) = [i + 1, if j >= 1: 0 else: j + 0.01]
        }
        HOLST.rectangle(Color.rainbow(j), x, y, 1, 1)
        y += 1
    }
    x += 1
}

WINDOW.add(HOLST)
WINDOW.show()