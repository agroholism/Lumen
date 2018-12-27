# <region CONSTANTS>

const DEFAULT_STRING = ""
const DEFAULT_NUMBER = 0
const PI = 3.141592653589793
const E = 2.718281828459045
const EOL = "\e"
const INF_POS_SEQ = 0..Number.POS_INF
const MAX_NUM = Number.MAX
const MIN_NUM = Number.MIN

# </region>

# <region IO-FUNCTIONS>

fun read_number(prompt=DEFAULT_STRING): Optional[Number] {
    try: input(prompt).to_n() except: null
}

fun read_bool(prompt=DEFAULT_STRING): Optional[Boolean] {
    try: Boolean.parse(input(prompt)) except: null
}

fun read_string2(prompt1=DEFAULT_STRING, prompt2=DEFAULT_STRING) {
    new List { input(prompt1), input(prompt2) }
}

fun read_number2(prompt1=DEFAULT_STRING, prompt2=DEFAULT_STRING)  {
    new List { read_number(prompt1), read_number(prompt2) }
}

fun read_bool2(prompt1=DEFAULT_STRING, prompt2=DEFAULT_STRING) {
    new List { read_bool(prompt1), read_bool(prompt2) }
}

fun write(...params) {
    print(...params, onend=DEFAULT_STRING)
}

fun writeln(...params) {
    print(...params)
}

fun write_format(str, ...params) {
    write(str % params)
}

fun writeln_format(str, ...params) {
    writeln(str % params)
}

fun printf(str, ...params) {
    writeln_format(str, ...params)
}

# </region>

fun range(a, b, step=1) {
    if step == 1 {
        return a..b
    }
    return (a..b).step(step)
}

# NOT WORKING WITH REAL, ONLY INTEGER
# AND GIVEN FOR ONE MORE
fun partition(a, b, n) {
    return (a..b).step((b-a)/n)
}

fun seq_random(n, a, b) {
    (for i in 1..n: Math.random(a, b))
}

fun list() {
    args
}

fun make_list(size, value=null) {
    [value] * size
}

fun fill_random!(list, up=10, low=0) {
    list.each!(i => Math.random(low, up))
}

fun fill_random(n, up=10, low=0) {
    var result = if n is Number: make_list(n) else: n
    fill_random!(result)
    result
}

fun max() {
    args.max()
}

fun min() {
    args.min()
}

fun sum() {
    args.reduce(_ + _)
}

fun mul() {
    args.reduce(_ * _)
}

fun iterate(i1, f) {
    (for i in INF_POS_SEQ: i1 = f(i1))
}

# NOT WORKING
fun iterate2(first, second, nex) {
    (for i in INF_POS_SEQ {
        nxt = nex(first, second)
        first = second
        second = nxt
    })
}

fun seq_gen(count: Number, i1, f) {
    iterate(i1, f).take(count)
}

fun seq_while(i1, f, s) {
    iterate(i1, f).take_while(s)
}

# NOT WORKING
# eRROR IN CLOSURe
fun seq_double_gen(i1, i2, f, s) {
    (for i in INF_POS_SEQ {
        t = i2
        i2 = f(i1, i2)
        i1 = t  
        if i1 >= s:
            break 1
        i1
    })
}

fun ord(s: String): Number { 
    s.chars[0] 
}

fun chr(c: Number): String { 
    c.char
}

fun read_seq_string(n, prompt: String=DEFAULT_STRING) {
    range(1, n).each(() => input(prompt))
}

fun read_seq_number(n, prompt: String=DEFAULT_STRING) {
    range(1, n).each(() => input(prompt).to_n())
}

fun read_seq_bool(n, prompt: String=DEFAULT_STRING) {
    range(1, n).each(() => Boolean.parse(input(prompt)))
}

fun read_seq_string_while(cond: Predicate[String], prompt: String=DEFAULT_STRING) {
    (for i in INF_POS_SEQ: {
        i = input(prompt)
        if not cond(i): 
            break
        i
    })
}

fun read_seq_number_while(cond: Predicate[Number], prompt: String=DEFAULT_STRING) {
    (for i in INF_POS_SEQ: {
        i = input(prompt).to_n()
        if not cond(i): 
            break
        i
    })
}

fun read_seq_bool_while(cond: Predicate[Boolean], prompt: String=DEFAULT_STRING) {
    (for i in INF_POS_SEQ: {
        i = Boolean.parse(input(prompt))
        if not cond(i): 
            break
        i
    })
}

# <region FILE FUNCTIONS>

fun open(name) {
    new File(name)
}

fun delete(file) {
    File.delete(file)
}

fun dir(file) {
    File.dir(file)
}

fun exist?(file) {
    File.exist?(file)
}

fun append(file, text) {
    file.add!(text)
}


#</region>



fun callable?(obj) {
	obj.respond?("()")
}

fun rad_to_deg(x: Number) => x * 180 / PI

fun deg_to_rad(x: Number) => x * PI / 180





namespace std

[const] 
let NL: str = ...
[const] 
let EL: str = ...

[const] 
let NaN: num = ...
[const] 
let POS_INF: num = ...
[const] 
let NEG_INF: num = ...
[const] 
let PI: num = ...
[const] 
let E: num = ...

let print(...args: any, onend: any=NL, sep: any=" ", file=null, write: any=false): void

let input(message: any=EL): str

[comparable]
record num = ...

[pure] 
let num.op_uexcl(): num
let num.op_uplus()
let num.op_uminus()
let num.op_bnot()

let num.op_range([num] other)
let num.op_range_excl([num] other)
let num.op_plus([num] other)
let num.op_minus([num] other)
let num.op_divide([num] other)
let num.op_mul([num] other)
let num.op_div([num] other)
let num.op_mod([num] other)
let num.op_pow(other)
let num.op_lt(other)
let num.op_gt(other)
let num.op_lteq(other)
let num.op_gteq(other)
let num.op_eql(other)
let num.op_noteql(other)
let num.op_bor(other)
let num.op_band(other)
let num.op_excl(other)
let num.op_lsh(other)
let num.op_rsh(other)

# Выполняет action this раз, передавая всё новое значение this
fun num.times(action)
# Возвращает Seq, аналогичный times
fun num.count(func)
# Округление
fun num.round(to)
# Модуль
fun num.abs()
fun num.sqrt()
fun num.sin()
fun num.cos()
fun num.tan()
fun num.get_is_int()
fun num.get_is_odd()
fun num.get_is_even()
fun num.get_char()
fun num.str(base=10)

record clonable
let comparable

[comparable]
record bool = ...
let bool.str()
let bool.num()

[comparable]
record str = ...

[comparable]
record vec = ...

# возвращает новый вектор
let vec(...args, from=nil, to=nil, step:=if len is not nil: (from-to)/len, len=nil): vec

# возвращает длину вектора
let vec.op_ustar(): num

# делает поверхностную копию вектора, добавляет other в его конец и возвращает его
[pure] 
let vec.op_plus(other: vec|seq): vec

# находит разность векторов как разность множеств
[pure] 
let vec.op_minus(other: vec|seq): vec

# возвращает неглубокую копию вектора
[pure] 
let vec.clone(): vec