using System;

namespace Lumen.Lang {
	public class Instance : BaseValueImpl {
		public Value[] Items { get; }
		public override IType Type { get; }

		public Instance(Constructor type) {
			this.Items = new Value[type.Fields.Count];
			this.Type = type;
		}

		public Boolean TryGetField(String name, out Value result) {
			Int32 index = (this.Type as Constructor).Fields.IndexOf(name);

			if (index != -1) {
				result = this.Items[index];
				return true;
			}

			result = null;
			return false;
		}

		public Value GetField(String name) {
			return this.TryGetField(name, out Value result)
				? result
				: throw new LumenException(Exceptions.INSTANCE_OF_DOES_NOT_CONTAINS_FIELD.F(this.Type, name));
		}

		public override Boolean Equals(Object obj) {
			if (obj is Instance otherInstance
				&& otherInstance.Type == this.Type
				&& otherInstance.Items.Length == this.Items.Length) {

				for (Int32 i = 0; i < this.Items.Length; i++) {
					if (!this.Items[i].Equals(otherInstance.Items[i])) {
						return false;
					}
				}

				return true;
			}

			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return base.GetHashCode();
		}

		public override String ToString() {
			if (this.Type.TryGetMember("toText", out Value value)
				&& value.TryConvertToFunction(out Fun converter)) {
				return converter.Call(new Scope(), this).ToString();
			}

			return "(" + this.Type.ToString() + " " + String.Join<Value>(" ", this.Items) + ")";
		}
	}
}