using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public class Module : IObject {
        public Dictionary<String, Value> variables;
        public String name;
        public Boolean isNotScope;

        public IObject Parent { get; set; }

        public List<TypeClass> TypeClasses { get; } = new List<TypeClass>();

        public Module() {
            this.variables = new Dictionary<String, Value>();
        }

        public Module(String name) {
            this.variables = new Dictionary<String, Value>();
            this.name = name;
        }

        public Boolean Contains(String name) {
            return this.variables.ContainsKey(name);
        }

        public IObject Type => Prelude.Any;

        public Int32 CompareTo(Object obj) {
            return 0;
        }

        public Value Clone() {
            return this;
        }

        public String ToString(Scope e) {
            return this.name;
        }

        public override String ToString() {
            return this.name;
        }

        public Boolean IsDerivedFrom(TypeClass typeClass) {
			return this.TypeClasses.Contains(typeClass);
		}

        public Value GetField(String name, Scope e) {
            if (this.TryGetField(name, out Value result)) {
                return result;
            }

            foreach(var i in this.TypeClasses) {
                if(i.TryGetField(name, out result)) {
                    return result;
                }
            }

            if (this.Parent != null) {
                if (this.Parent.TryGetField(name, out result)) {
                    return result;
                }
            }

            throw new LumenException($"Module does not contains a field {name}");
        }

        public void SetField(String name, Value value, Scope e=null) {
            this.variables[name] = value;
        }

        public Boolean TryGetField(String name, out Value result) {
            if (this.variables.TryGetValue(name, out result)) {
                return true;
            }

            foreach (var i in this.TypeClasses) {
                if (i.TryGetField(name, out result)) {
                    return true;
                }
            }

            if (this.Parent != null) {
                if (this.Parent.TryGetField(name, out result)) {
                    return true;
                }
            }

            return false;
        }

        public Boolean IsParentOf(Value value) {
            if (value is IObject parent) {
                while (true) {
                    if (parent.Parent != null) {
                        parent = parent.Parent;
                        if (parent == this) {
                            return true;
                        }
                    } else {
                        break;
                    }
                }
            }

            if (value.Type == this) {
                return true;
            }

            return false;
        }

        public void Derive(TypeClass mixin) {
            if (!mixin.IsTypeImplement(this)) {
                throw new LumenException("Type not implement type class");
            }

            this.TypeClasses.Add(mixin);
        }
    }
}
