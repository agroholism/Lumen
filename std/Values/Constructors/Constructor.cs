using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	public class Constructor : BaseValueImpl, IConstructor, Fun {
		public String Name { get; set; }
		public Dictionary<String, List<IType>> Fields { get; private set; }
		public override IType Type => Prelude.Function;

		public List<IPattern> Parameters { get; set; }
		public Type Parent { get; set; }

		public Constructor(String name, Type parent, Dictionary<String, List<IType>> fields) {
			this.Parent = parent;
			this.Name = name;
			this.Fields = fields;
			this.Parameters = fields.Select(i => (IPattern)new TypesPattern(i.Key, i.Value)).ToList();
		}

		public Constructor(String name, Type parent, params String[] fields) {
			this.Parent = parent;
			this.Name = name;
			Dictionary<String, List<IType>> dict = new();
			foreach (var i in fields) {
				dict.Add(i, new List<IType>());
			}

			this.Fields = dict;
			this.Parameters = dict.Select(i => (IPattern)new TypesPattern(i.Key, i.Value)).ToList();
		}

		public Value MakeInstance(params Value[] values) {
			Instance result = new Instance(this);

			Int32 current = 0;
			foreach(var i in this.Fields) {
				if(!i.Value.All(j => j.IsParentOf(values[current]))) {
					throw new Exception($"wait value of type {String.Join(", ", i.Value)} given {values[current].Type}");
				}

				result.Items[current] = values[current];
				current++;
			}

			return result;
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

		public Value GetMember(String name) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			throw new LumenException($"fne {name} {this.Name}");
		}

		public virtual Value Call(Scope e, params Value[] arguments) {
			if (this.Parameters.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			return this.MakeInstance(arguments);
		}

		public Boolean HasImplementation(Class typeClass) {
			return this.Parent.HasImplementation(typeClass);
		}

		public Boolean HasMember(String name) {
			return this.Parent.HasMember(name);
		}
	}
}
