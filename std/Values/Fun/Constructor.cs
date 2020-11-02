using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class Constructor : BaseValueImpl, IType, Fun {
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

		public Instance MakeInstance(params Value[] values) {
			Instance result = new Instance(this);

			for (Int32 i = 0; i < values.Length; i++) {
				result.items[i] = values[i];
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

			throw new LumenException("fne");
		}

		public Value Run(Scope e, params Value[] arguments) {
			if (this.Arguments.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			return this.MakeInstance(arguments);
		}

		public Boolean HasMixin(Module typeClass) {
			return this.Parent.HasMixin(typeClass);
		}
	}

	public class UserDefinedConstructor : BaseValueImpl, IType, Fun {
		public Fun InternalConstructor { get; set; }

		public String Name {
			get => this.InternalConstructor.Name;
			set => this.InternalConstructor.Name = value;
		}

		public override IType Type => Prelude.Function;

		public List<IPattern> Arguments {
			get => this.InternalConstructor.Arguments;
			set => this.InternalConstructor.Arguments = value;
		}

		public UserDefinedConstructor(Fun ctor, Module parent) {
			this.InternalConstructor = ctor;
			this.Parent = parent;
		}

		public Module Parent { get; set; }

		public Value GetMember(String name, Scope scope) {
			return this.Parent.GetMember(name, scope);
		}

		public Boolean HasMixin(Module typeClass) {
			return this.Parent.HasMixin(typeClass);
		}

		public Boolean IsParentOf(Value value) {
			IType parent = value.Type;

			if (parent == this) {
				return true;
			}

			return false;
		}

		public Value Run(Scope e, params Value[] args) {
			return this.InternalConstructor.Run(e, args);
		}

		public void SetMember(String name, Value value, Scope scope) {
			throw new NotImplementedException();
		}

		public override String ToString() {
			return $"{this.Name}";
		}

		public Boolean TryGetMember(String name, out Value result) {
			return this.Parent.TryGetMember(name, out result);
		}
	}
}
