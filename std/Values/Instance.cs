using System;

namespace Lumen.Lang {
	public class Instance : BaseValueImpl {
        public Value[] items;

        public override IType Type { get; }

		public Instance(Constructor type) {
            this.items = new Value[type.Fields.Count];
            this.Type = type;
        }

        public Boolean TryGetField(String name, out Value result) {
            Int32 index = (this.Type as Constructor).Fields.IndexOf(name);

            if (index != -1) {
                result = this.items[index];
                return true;
            }

            if (this.Type != null) {
                if (this.Type.TryGetMember(name, out result)) {
                    return true;
                }
            }

            result = null;
            return false;
        }

        public Value GetField(String name, Scope e) {
            if(this.TryGetField(name, out Value result)) {
				return result;
			}

            throw new LumenException(Exceptions.INSTANCE_OF_DOES_NOT_CONTAINS_FIELD.F(this.Type, name));
        }

        public void SetField(String name, Value value, Scope e) {
            // throw
        }

        public override Value Clone() {
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

		public override String ToString() {
			if (this.TryGetField("toText", out Value value)) {
				return ((Fun)value).Run(new Scope(), this).ToString();
			}

			return "(" + this.Type.ToString() + " " + String.Join<Value>(" ", this.items) + ")";
		}
	}
}