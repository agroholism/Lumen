type Lazy {
    var result
    var completed?
    
    fun initialize(result) {
        this.result = result
        this.completed? = false
    }
    
    fun get_result() {
        if this.completed?{
            return this.result
        }
        this.completed? = true
        this.result = this.result()
    }
}

fun lazy(func) {
    return new Lazy(func)
}