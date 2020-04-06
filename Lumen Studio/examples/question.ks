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

let data = [18.4, 17.6, 13.2, 28.9, 8.1, 38.4, 26.4, 26.3, 15.7, 24.1, 5.7, 19.9, 18.4, 12.1, 26.4, 28.1, 4.4, 12.8, 8.1, 9.2, 23.1, 17.2, 12.6, 10.5, 28.1, 13.8, 20.8, 12.3, 16.0, 17.6, 15.1, 13.8, 25.2, 8.5, 31.6, 19.0, 26.7, 26.5, 18.5, 29.1, 24.0, 12.3, 13.5, 34.2, 15.5, 25.8, 26.6, 16.1, 23.2, 17.1, 18.0, 20.5, 12.1, 15.6, 20.9, 11.8, 13.2, 17.4, 18.3, 20.5]
let n = 60
let delta = 0.05 // rounding of i to 0.x
let k = Number.round (n ^ 0.5) 0
let range data = 
    (List.max data) - List.min data

let getRanges data =
    let rng = range data
    
    let i = rng / k  + delta
    
    let ranges = [||]
    
    let current = ref (List.min data)
    let lowb = ref (List.min data)
    let upb = ref (!lowb + i)
    for _ in 0..k:
        Array.add ([!lowb, !upb]) ranges
        lowb <- !lowb + i + 0.1
        upb <- !upb + i + 0.1
    return ranges
    
let normalizeRanges ranges =
    let normalized = [||]
    for i in ranges:
        Array.add ([i.[0] - 0.5, i.[1] - 0.5]) normalized
    return normalized

let len data = 
    Stream.count data (x -> x)

let getFrequences ranges data = 
    let freqs = [||]
    for i in ranges:
        Array.add (len data.[x -> i.[0] <= x <= i.[1]]) freqs
    return freqs

let getAverrages ranges data =
    let avgs = [||]
    for i in ranges:
        Array.add ((i.[0] + i.[1]) / 2) avgs
    return avgs

let smthngelse freqs avgs =
    let result = [||]
    for j in 0..k:
        Array.add (avgs.[j] * freqs.[j]) result
    return result

let evalSubstr avgs mean = 
    List.toArray (for i in avgs: (i - mean)^2)

let evalProds avgs mean freqs = 
    let subs = evalSubstr avgs mean
    let prods = [||]
    for i in 0..k:
        Array.add (freqs.[i] * subs.[i]) prods
    return prods 

let ranges = getRanges data
let normalizedranges = normalizeRanges ranges

print "Диапазоны"
for i in ranges:
    print (Number.toText i.[0] + "-" + Number.toText i.[1])
print "Исправленные диапазоны"
for i in normalizedranges:
    print (Number.toText i.[0] + "-" + Number.toText i.[1])
let freqs = getFrequences normalizedranges data
let avgs = getAverrages normalizedranges data
print "Частоты диапазонов"
for i in freqs:
    print (i)
print "Средние диапазонов"
for i in avgs:
    print (i)
let prod = smthngelse freqs avgs
print "fi*xii"
for i in prod:
    print (i)
let sumofprod = prod / (_+_)
print ("Сумма fi*xii: " + sumofprod)
print ("Среднее: " + sumofprod / n)

print "(x-x ср)^2"
for i in evalSubstr avgs (sumofprod / n):
    print (i)

print "f*(x-x ср)^2"
for i in evalProds avgs (sumofprod / n) freqs:
    print (i)
print ("Сумма: " + (evalProds avgs (sumofprod / n) freqs) / (_+_)) 
let disp = (evalProds avgs (sumofprod / n) freqs) / (_+_) / n
print ("Дисперсия: " + disp) 
print ("Стандартное откл: " + disp^0.5) 
print ("Коэффициент вариации (%): " + (disp^0.5) / (sumofprod / n) * 100)
print ("Ошибка среднего: " + (disp^0.5) / (n^0.5))







let array = List.toArray

let getRanges dataMin datai datak =
    [| for j in 0..datak: [dataMin + (datai + 0.1) * j, dataMin + datai + (datai + 0.1) * j] |]
    
let normalizeRanges ranges =
    [| for [f, s] in ranges: [f - 0.5, s - 0.5] |]

let getFrequences ranges data = 
    [| for [b, e] in ranges: *data.[x -> b <= x <= e] |]

let getAverrages ranges =
    [| for [b, e] in ranges: (b + e) / 2 |]

let smthngelse freqs avgs datak =
    [| for j in 0..datak: avgs.[j] * freqs.[j] |]

let evalSubstr avgs mean = 
    [| for i in avgs: (i - mean)^2 |]

let evalProds substractions freqs datak = 
    [| for i in 0..datak: freqs.[i] * substractions.[i] |]

let writeIt title dataStream =
    print title
    for i in dataStream:
        print i 

// Данные
let data = [18.4, 17.6, 13.2, 28.9, 8.1, 38.4, 26.4, 26.3, 15.7, 24.1, 5.7, 19.9, 18.4, 12.1, 26.4, 28.1, 4.4, 12.8, 8.1, 9.2, 23.1, 17.2, 12.6, 10.5, 28.1, 13.8, 20.8, 12.3, 16.0, 17.6, 15.1, 13.8, 25.2, 8.5, 31.6, 19.0, 26.7, 26.5, 18.5, 29.1, 24.0, 12.3, 13.5, 34.2, 15.5, 25.8, 26.6, 16.1, 23.2, 17.1, 18.0, 20.5, 12.1, 15.6, 20.9, 11.8, 13.2, 17.4, 18.3, 20.5]
// Размер выборки
let n = *data
// Кооэффициент для округления шага
let delta = 0.05 // rounding of i to 0.x
// Число групп
let k = Number.round (n ^ 0.5) 0
// Минимальное значение
let dataMin = List.min data
// Максимальное значение
let dataMax = List.max data
// Размах
let rng = dataMax - dataMin
// Шаг
let step = rng / k + delta
// Диапазоны
let ranges = getRanges dataMin step k 
// Исправленные диапазоны
let normalizedranges = normalizeRanges ranges
// Частоты
let freqs = getFrequences normalizedranges data
// Средние диапазонов
let avgs = getAverrages normalizedranges
// Произведние частот на средние диапазонов
let prod = smthngelse freqs avgs k 
// Сумма произведений частот на средние диапазонов
let sumofprod = prod / (_+_)
// Среднее произведений частот на средние диапазонов
let mean = sumofprod / n
// Разность между средним диапазонов и средним произведений частот на средние диапазонов в квадрате
let substractions = evalSubstr avgs mean
// Произведение частоты на разность между средним диапазонов и средним произведений частот на средние диапазонов в квадрате
let productions = evalProds substractions freqs k
let disp = productions / (_+_) / n
let sd = disp^0.5

writeIt "Диапазоны" ranges
writeIt "Исправленные диапазоны" normalizedranges
writeIt "Частоты диапазонов" freqs
writeIt "Средние диапазонов" avgs
writeIt "fi*xii" prod
print ("Сумма fi*xii: " + sumofprod)
print ("Среднее: " + mean)
writeIt "(x-x ср)^2" (evalSubstr avgs (sumofprod / n))
writeIt "f*(x-x ср)^2" productions
print ("Сумма: " + productions / (_+_))
print ("Дисперсия: " + disp) 
print ("Стандартное откл: " + sd) 
print ("Коэффициент вариации (%): " + (sd / mean * 100)
print ("Ошибка среднего: " + (sd / n^0.5))