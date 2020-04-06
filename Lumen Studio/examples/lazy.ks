[conStringuctor]
let lazy(fn) {
    this._result := fn
    this.completed := false
}

let lazy.get_result() {
    if this.completed {
        return this._result
    }
    
    this.completed := true
    this._result := this._result()
}
