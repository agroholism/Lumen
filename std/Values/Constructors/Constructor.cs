using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class Constructor : BaseValueImpl, IConstructor, Fun {
		public String Name { get; set; }
		public List<String> Fields { get; private set; }
		public override IType Type => Prelude.Function;

		public List<IPattern> Arguments { get; set; }
		public Module Parent { get; set; }

		public Constructor(String name, Module parent, List<String> fields) {
			this.Parent = parent;
			this.Name = name;
			this.Fields = fields;
			this.Arguments = fields.Select(i => new NamePattern(i) as IPattern).ToList();
		}

		public Value MakeInstance(params Value[] values) {
			Instance result = new Instance(this);

			for (Int32 i = 0; i < values.Length; i++) {
				result.Items[i] = values[i];
			}

			return result;
		}

		public void SetMember(String name, Value value, Scope scope) {

		}

		public override String ToString() {
			return $"{this.Name}";
		}

		public Boolean TryGetMember(String name, out Value result) {
			return this.Parent.TryGetMember(name, out result);
		}

		public Boolean IsParentOf(Value value) {
			IType parent = value.Type;

			if (parent == this) {
				return true;
			}

			return false;
		}

		public Value GetMember(String name, Scope scope) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			throw new LumenException($"fne {name} {this.Name}");
		}

		public Value Run(Scope e, params Value[] arguments) {
			if (this.Arguments.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			return this.MakeInstance(arguments);
		}

		public Boolean HasImplementation(Module typeClass) {
			return this.Parent.HasImplementation(typeClass);
		}
	}
}
