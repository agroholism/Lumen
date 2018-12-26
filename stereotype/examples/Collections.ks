# <summary> Представляет узел связанного списка </summary>
type LinkedListNode {
    var value
    var next_node: Optional[LinkedListNode]
    
    fun initialize(value, next_node=null) {
        this.value = value
        this.next_node = next_node
    }
    
    fun to_s() {
        return this.value.to_s()
    }
}

type LinkedList {
    in var first: Optional[LinkedListNode]
    in var last: Optional[LinkedListNode]
    in var count: Number
    
    fun initialize() {
        this.first = null
        this.last = null
        this.count = 0
    }
    
    fun +=(value) {
        this.append(value)
        return this
    }
    
    fun append(value) {
        if this.first is null {
            this.first = new LinkedListNode(value)
            this.last = this.first
        } else {
            this.last.next_node = new LinkedListNode(value)
            this.last = this.last.next_node
        }
        this.count = 1 + this.count
    }
    
    fun prepend(value) {
        this.first = new LinkedListNode(value, this.first)
        this.count = 1 + this.count
    }
    
    fun each(func) {
        return this.to_i().each(func)
    }
    
    fun each!(func) {
        for node in this.to_nodes_i() {
            node.value = func(node.value)
        }
    }
    
    fun contains?(value) {
        return this.to_i().contains?(value)
    }
    
    fun to_l() {
        return this.to_i().to_l()
    }
        
    fun to_i() {
        return this.to_nodes_i().each(i => i.value)
    }
    
    fun to_nodes_i() {
        var current = this.first
        return iterator(() =>  {
            once self.current = current
            if self.current == null {
                break
            }
            var result = self.current
            self.current = self.current.next_node
            return result
        })
    }
    
    fun to_s() {
        if this.count == 0 {
            return "[]"
        }
        var result = "["
        var node = this.first
        while node.next_node is not null {
            result += node.value + ", "
            node = node.next_node
        }
        return result + node + "]"
    }

    type fun implicit(obj) {
        var v = new LinkedList()
        for i in obj.to_l() {
            v += i
        }
        return v
    }
} 

var lst: LinkedList = 3..7
lst.each!(x => x * x)
print(lst)

final class LinkedListNode {
	public Value
	public Next
	
	fun this(val, nex = null) {
		this.Value = val
		this.Next = nex
	}
	
	fun ToString() => this.Value
}

final class LinkedList {
	public First { get; private set; }
	public Last { get; private set; }
	public Count { get; private set if value % 1 == 0; } : Number
	
	fun this() { 
		this::Count = 0
	}
	
	fun Append(val) {
		if this.First is null {
			this::First = new LinkedListNode(val)
			this::Last = this.First
		} else {
			this.Last.Next = new LinkedListNode(val)
			this::Last = this.Last.Next
		}
		this::Count = ++this.Count
	}
	
	fun Prepend(val) {
		this::First = new LinkedListNode(val, this::First)
		this::Count = ++this.Count
	}
	
	fun Contains?(val) {
		item = this.First
		while item.Next is LinkedListNode {
			if item.Value == val:
				return true
			item = item.Next
		}
		return item.Value == val
	}
	
	fun Each(func) {
		res = new LinkedList()
		item = this.First
		while item.Next is LinkedListNode {
			res.Append(func(item.Value))
			item = item.Next
		}
		res.Append(func(item.Value))
		return res
	}
	
	fun Each!(func) {
		item = this.First
		while item.Next is LinkedListNode {
			item.Value = func(item.Value)
			item = item.Next
		}
		item.Value = func(item.Value)
	}
	
	fun Select(func) {
		res = []
		item = this.First
		while item.Next is LinkedListNode {
			res += func(item.Value)
			item = item.Next
		}
		res += func(item.Value)
		return res
	}
	
	fun Where(func) {
		res = []
		item = this.First
		while item.Next is LinkedListNode {
			if func(item.Value):
				res += item.Value
			item = item.Next
		}
		if func(item.Value):
			res += item.Value
		return res
	}
	
	fun ToString() {
		if this.Count == 0:
			return "[]"
		res = "["
		this = this.First
		while this.Next is LinkedListNode {
			res += this.Value + ", "
			this = this.Next
		}
		res + this + "]"
	}
	
	fun ToList() {
		res = []
		if this.Count == 0:
			return res
		this = this.First
		while this.Next is LinkedListNode {
			res += this.Value
			this = this.Next
		}
		res += this.Value
		return res
	}
}

final class IterableLinkedList : Iterable {
	private count : Number
	private current : LinkedListNode
	private pointer : Number
	
	fun this(ll : LinkedList) {
		this::count = ll.Count
		this::current = new LinkedListNode(null, ll.First)
		this::pointer = 0
	}
	
	fun moveNext() {
		while this::pointer != this::count {
			this::current = this::current.Next
			this::pointer = ++this::pointer
			return true
		}
		return false
	}
	
	fun current() {
		return this::current
	}
}

LinkedList.GetIterator = () => new IterableLinkedList(this)

final class Map {
	public keys
	public values
	
	fun this() {
		this.keys = []
		this.values = []
	}
	
	fun Set(key, val) {
		if (i = this.keys.IndexOf(key)) != -1 {
			this.values[i] = val
		} else {
			this.keys.Add!(key)
			this.values.Add!(val)
		}
	}
	
	fun Get(key) {
		if (i = this.keys.IndexOf(key)) == -1:
			reise "key not exists"
		else:
			this.values[i]
	}
	
	fun ToString() => this.ToList()
	
	fun ToList() => this.keys.Zip(this.values)
}

final class IterableMap : Iterable {
	public current
	public map
	private pointer : Number
	
	fun this(ll : Map) {
		this.current = null
		this.map = ll
		this::pointer = 0
	}
	
	fun moveNext() {
		while this::pointer != *this.map.keys {
			this.current = this.map.Get(this.map.keys[this::pointer])
			this::pointer = ++this::pointer
			return true
		}
		return false
	}
	
	fun current() {
		return this.current
	}
}

final class Tuple2 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	
	fun this(item1, item2) {
		this::Item1 = item1
		this::Item2 = item2
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2})"
}

final class Tuple3 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	public Item3 { get; private set; }
	
	fun this(item1, item2, item3) {
		this::Item1 = item1
		this::Item2 = item2
		this::Item3 = item3
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2}, {this.Item3})"
}

final class Tuple4 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	public Item3 { get; private set; }
	public Item4 { get; private set; }
	
	fun this(item1, item2, item3, item4) {
		this::Item1 = item1
		this::Item2 = item2
		this::Item3 = item3
		this::Item4 = item4
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4})"
}

final class Tuple5 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	public Item3 { get; private set; }
	public Item4 { get; private set; }
	public Item5 { get; private set; }
	
	fun this(item1, item2, item3, item4, item5) {
		this::Item1 = item1
		this::Item2 = item2
		this::Item3 = item3
		this::Item4 = item4
		this::Item5 = item5
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5})"
}

final class Tuple6 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	public Item3 { get; private set; }
	public Item4 { get; private set; }
	public Item5 { get; private set; }
	public Item6 { get; private set; }
	
	fun this(item1, item2, item3, item4, item5, item6) {
		this::Item1 = item1
		this::Item2 = item2
		this::Item3 = item3
		this::Item4 = item4
		this::Item5 = item5
		this::Item6 = item6
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5}, {this.Item6})"
}

final class Tuple7 {
	public Item1 { get; private set; }
	public Item2 { get; private set; }
	public Item3 { get; private set; }
	public Item4 { get; private set; }
	public Item5 { get; private set; }
	public Item6 { get; private set; }
	public Item7 { get; private set; }
	
	fun this(item1, item2, item3, item4, item5, item6, item7) {
		this::Item1 = item1
		this::Item2 = item2
		this::Item3 = item3
		this::Item4 = item4
		this::Item5 = item5
		this::Item6 = item6
		this::Item7 = item7
	}
	
	fun ToString() => 
		$"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5}, {this.Item6}, {this.Item7})"
}

final class Set {
	public Value { get; private set; }
	public Count { get; private set; } : Number
	
	fun this() {
		this::Value = []
		this::Count = 0
	}
	
	fun Add(val) {
		if this::Value.Contains?(val):
			return false
		else:
			this::Value.Add!(val)
		this::Count = ++this.Count
		return true
	}
	
	fun ToString() => this::Value
}

Core.List.ToLinkedList = () => {
	result = new LinkedList()
	for i in this: result.Append(i)
	return result
}
=============================================
type LinkedListNode {
    var value
    var next_node
    
    fun this(val, next_node = null) {
        this.value = val
        this.next_node = next_node
    }
    
    fun get_value() {
        this.value
    }
    
    fun get_next_node() {
        this.next_node
    }
    
    fun to_s() { 
        this.value
    }
}

type LinkedList {
    var first
    var last
    var count
    
    fun this() { 
        this.count = 0
    }
    
    fun append(val) {
        if this.first is null {
            this.first = new LinkedListNode(val)
            this.last = this.first
        } else {
            this.last.next_node = new LinkedListNode(val)
            this.last = this.last.next_node
        }
        this.count = ++this.count
    }
    
    fun prepend(val) {
        this.first = new LinkedListNode(val, this.first)
        this.count = ++this.count
    }
    
    fun contains?(val) {
        item = this.first
        while item.next_node is LinkedListNode {
            if item.value == val:
                return true
            item = item.next_node
        }
        return item.value == val
    }
    
    fun each(func) {
        res = new LinkedList()
        item = this.First
        while item.Next is LinkedListNode {
            res.Append(func(item.Value))
            item = item.Next
        }
        res.Append(func(item.Value))
        return res
    }
    
    fun Each!(func) {
        item = this.First
        while item.Next is LinkedListNode {
            item.Value = func(item.Value)
            item = item.Next
        }
        item.Value = func(item.Value)
    }
    
    fun Select(func) {
        res = []
        item = this.First
        while item.Next is LinkedListNode {
            res += func(item.Value)
            item = item.Next
        }
        res += func(item.Value)
        return res
    }
    
    fun Where(func) {
        res = []
        item = this.First
        while item.Next is LinkedListNode {
            if func(item.Value):
                res += item.Value
            item = item.Next
        }
        if func(item.Value):
            res += item.Value
        return res
    }
    
    fun ToString() {
        if this.Count == 0:
            return "[]"
        res = "["
        this = this.First
        while this.Next is LinkedListNode {
            res += this.Value + ", "
            this = this.Next
        }
        res + this + "]"
    }
    
    fun ToList() {
        res = []
        if this.Count == 0:
            return res
        this = this.First
        while this.Next is LinkedListNode {
            res += this.Value
            this = this.Next
        }
        res += this.Value
        return res
    }
}

final class IterableLinkedList : Iterable {
    private count : Number
    private current : LinkedListNode
    private pointer : Number
    
    fun this(ll : LinkedList) {
        this::count = ll.Count
        this::current = new LinkedListNode(null, ll.First)
        this::pointer = 0
    }
    
    fun moveNext() {
        while this::pointer != this::count {
            this::current = this::current.Next
            this::pointer = ++this::pointer
            return true
        }
        return false
    }
    
    fun current() {
        return this::current
    }
}

LinkedList.GetIterator = () => new IterableLinkedList(this)

final class Map {
    public keys
    public values
    
    fun this() {
        this.keys = []
        this.values = []
    }
    
    fun Set(key, val) {
        if (i = this.keys.IndexOf(key)) != -1 {
            this.values[i] = val
        } else {
            this.keys.Add!(key)
            this.values.Add!(val)
        }
    }
    
    fun Get(key) {
        if (i = this.keys.IndexOf(key)) == -1:
            reise "key not exists"
        else:
            this.values[i]
    }
    
    fun ToString() => this.ToList()
    
    fun ToList() => this.keys.Zip(this.values)
}

final class IterableMap : Iterable {
    public current
    public map
    private pointer : Number
    
    fun this(ll : Map) {
        this.current = null
        this.map = ll
        this::pointer = 0
    }
    
    fun moveNext() {
        while this::pointer != *this.map.keys {
            this.current = this.map.Get(this.map.keys[this::pointer])
            this::pointer = ++this::pointer
            return true
        }
        return false
    }
    
    fun current() {
        return this.current
    }
}

final class Tuple2 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    
    fun this(item1, item2) {
        this::Item1 = item1
        this::Item2 = item2
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2})"
}

final class Tuple3 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    public Item3 { get; private set; }
    
    fun this(item1, item2, item3) {
        this::Item1 = item1
        this::Item2 = item2
        this::Item3 = item3
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2}, {this.Item3})"
}

final class Tuple4 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    public Item3 { get; private set; }
    public Item4 { get; private set; }
    
    fun this(item1, item2, item3, item4) {
        this::Item1 = item1
        this::Item2 = item2
        this::Item3 = item3
        this::Item4 = item4
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4})"
}

final class Tuple5 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    public Item3 { get; private set; }
    public Item4 { get; private set; }
    public Item5 { get; private set; }
    
    fun this(item1, item2, item3, item4, item5) {
        this::Item1 = item1
        this::Item2 = item2
        this::Item3 = item3
        this::Item4 = item4
        this::Item5 = item5
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5})"
}

final class Tuple6 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    public Item3 { get; private set; }
    public Item4 { get; private set; }
    public Item5 { get; private set; }
    public Item6 { get; private set; }
    
    fun this(item1, item2, item3, item4, item5, item6) {
        this::Item1 = item1
        this::Item2 = item2
        this::Item3 = item3
        this::Item4 = item4
        this::Item5 = item5
        this::Item6 = item6
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5}, {this.Item6})"
}

final class Tuple7 {
    public Item1 { get; private set; }
    public Item2 { get; private set; }
    public Item3 { get; private set; }
    public Item4 { get; private set; }
    public Item5 { get; private set; }
    public Item6 { get; private set; }
    public Item7 { get; private set; }
    
    fun this(item1, item2, item3, item4, item5, item6, item7) {
        this::Item1 = item1
        this::Item2 = item2
        this::Item3 = item3
        this::Item4 = item4
        this::Item5 = item5
        this::Item6 = item6
        this::Item7 = item7
    }
    
    fun ToString() => 
        $"({this.Item1}, {this.Item2}, {this.Item3}, {this.Item4}, {this.Item5}, {this.Item6}, {this.Item7})"
}

final class Set {
    public Value { get; private set; }
    public Count { get; private set; } : Number
    
    fun this() {
        this::Value = []
        this::Count = 0
    }
    
    fun Add(val) {
        if this::Value.Contains?(val):
            return false
        else:
            this::Value.Add!(val)
        this::Count = ++this.Count
        return true
    }
    
    fun ToString() => this::Value
}

Core.List.ToLinkedList = () => {
    result = new LinkedList()
    for i in this: result.Append(i)
    return result
}
