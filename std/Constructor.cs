using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    public class Constructor : IObject, Fun {
        public String Name { get; set; }
        public IObject Parent { get; set; }
        public List<String> Fields { get; set; }

        public IObject Type => Prelude.Function;

        public List<IPattern> Arguments { get; set; }

        public Constructor(String name, IObject parent, List<String> fields) {
            this.Parent = parent;
            this.Name = name;
            this.Fields = fields;
            this.Arguments = fields.Select(i => new NamePattern(i) as IPattern).ToList();
        }

        public Instance MakeInstance(params Value[] values) {
            Instance result = new Instance(this);

            for (Int32 i = 0; i < this.Fields.Count; i++) {
                result.SetField(this.Fields[i], values[i], null);
            }

            return result;
        }

        public Value Clone() {
            return this;
        }

        public Int32 CompareTo(Object obj) {
            throw new NotImplementedException();
        }

        public void SetField(String name, Value value, Scope scope) {

        }

        public String ToString(Scope e) {
            return $"{this.Name}";
        }

        public Boolean TryGetField(String name, out Value result) {
            return this.Parent.TryGetField(name, out result);
        }

        public Boolean IsParentOf(Value value) {
            if (value is IObject parent) {
                while (true) {
                    if (parent != null && parent.Parent != null) {
                        parent = parent.Parent;
                        if (parent == this) {
                            return true;
                        }
                    } else {
                        break;
                    }
                }
            }
            return false;
        }

        public Value GetField(String name, Scope scope) {
            if (this.TryGetField(name, out var result)) {
                return result;
            }

            throw new LumenException("fne");
        }

        public Value Run(Scope e, params Value[] args) {
            return this.MakeInstance(args);
        }
    }

    public class SingletonConstructor : IObject {
        public String Name { get; set; }
        public IObject Parent { get; set; }

        public IObject Type => Prelude.Function;

        public SingletonConstructor(String name, IObject parent) {
            this.Parent = parent;
            this.Name = name;
        }

        public Value GetField(String name, Scope scope) {
            if (this.TryGetField(name, out var result)) {
                return result;
            }

            throw new LumenException("fne");
        }

        public Value Clone() {
            return this;
        }

        public Int32 CompareTo(Object obj) {
            throw new NotImplementedException();
        }

        public void SetField(String name, Value value, Scope scope) {

        }

        public String ToString(Scope e) {
            return $"{this.Name}";
        }

        public Boolean TryGetField(String name, out Value result) {
            return this.Parent.TryGetField(name, out result);
        }

        public Boolean IsParentOf(Value value) {
            if (value is IObject parent) {
                while (true) {
                    if (parent != null && parent.Parent != null) {
                        parent = parent.Parent;
                        if (parent == this) {
                            return true;
                        }
                    } else {
                        break;
                    }
                }
            }
            return false;
        }
    }
}
