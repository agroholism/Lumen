using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public class Instance : IObject {
        public Value[] items;

        /// <summary> Parent object, always must be Constructor </summary>
        public IObject Parent { get; set; }

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
            return false;
        }

        public override String ToString() {
            return this.ToString(null);
        }

        public IObject Type => this.Parent;

        public Instance(Constructor parent) {
            this.items = new Value[parent.Fields.Count];
            this.Parent = parent;
        }

        public Boolean TryGetField(String name, out Value result) {
            Int32 index = (this.Parent as Constructor).Fields.IndexOf(name);

            if (index != -1) {
                result = this.items[index];
                return true;
            }

            if (this.Parent != null) {
                if (this.Parent.TryGetField(name, out result)) {
                    return true;
                }
            }

            result = null;
            return false;
        }

        public Value GetField(String name, Scope e) {
            Int32 index = (this.Parent as Constructor).Fields.IndexOf(name);

            if (index != -1) {
                return this.items[index];
            }

            if (this.Parent != null) {
                if (this.Parent.TryGetField(name, out Value result)) {
                    return result;
                }
            }

            throw new LumenException($"Module does not contains a field {name}");
        }

        public void SetField(String name, Value value, Scope e) {
            Int32 index = (this.Parent as Constructor).Fields.IndexOf(name);

            if (index != -1) {
                this.items[index] = value;
            }
        }

        public Int32 CompareTo(Object obj) {
            throw new NotImplementedException();
        }

        public String ToString(Scope e) {
            if (this.TryGetField("String", out Value value)) {
                return ((Fun)value).Run(new Scope(e), this).ToString(e);
            }

            throw new LumenException("невозможно преобразовать объект к типу std.String");
        }

        public Value Clone() {
            return this;
        }

        public override Boolean Equals(Object obj) {
            if (obj is Value value) {
                if (value is Instance objn) {
                    if (objn.Type == this.Type) {
                        for (Int32 i = 0; i < this.items.Length; i++) {
                            if (!this.items[i].Equals(objn.items[i])) {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }

            return base.Equals(obj);
        }

        public override Int32 GetHashCode() {
            return base.GetHashCode();
        }
    }
}