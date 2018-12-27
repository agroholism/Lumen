fun logik(patterns, value) {
    if patterns.contains?(value) {
        return true
    }
    
    for i in patterns {
        if i is Function and i(value) {
			return true
        }
    }
}

fun switch(body, value) {
    var case = ...args => self.fall and not (self.fall = not (*args == 0 or logik(args, value)))
    case.fall = true
    body(case)
}