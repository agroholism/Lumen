using gui

const HEIGHT = 300
const WIDTH = 300
const WINDOW = new Form(height=HEIGHT + 40, width=WIDTH + 17)
const HOLST = new Graphics(HEIGHT, WIDTH)

var (i, j, k, q_1, w, e, koef, x_m, y_m, x_n, y_n, p, q, x_0, y_0) = [1, 1, 1, 3, -2, -1.5, 0.5, 0, 0, 0, 0, 0, 0, 0, 0]
while j < WIDTH {
    p = w + j * (q_1 / WIDTH)
    k = 1
    while k <= HEIGHT {
        q = k * (q_1 / HEIGHT) + e 
        x_n = 0
        y_n = 0
        i = 1
        while i <= 110 {
            x_0 = x_n
            y_0 = y_n
            x_n = (x_n ** 2) - (y_n ** 2) + p 
            y_n = 2 * x_0 * y_n + q 
            if x_n ** 2 + y_n ** 2 > 4: break 1
            i += 1
        }
        HOLST.rectangle(Color.rgb(i + 50, i + 100, i + 140), WIDTH-j, k, 1, 1)
        k += 1
    }
    j += 1
}



WINDOW.add(HOLST)
WINDOW.show()