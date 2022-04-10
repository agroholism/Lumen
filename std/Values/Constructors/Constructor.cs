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
		public Module Parent { get; set; }

		public Constructor(String name, Module parent, Dictionary<String, List<IType>> fields) {
			this.Parent = parent;
			this.Name = name;
			this.Fields = fields;
			this.Parameters = fields.Select(i => (IPattern)new TypesPattern(i.Key, i.Value)).ToList();
		}

		public Constructor(String name, Module parent, params String[] fields) {
			this.Parent = parent;
			this.Name = name;
			Dictionary<String, List<IType>> dict = new();
			foreach (var i in fields) {
				dict.Add(i, new List<IType>());
			}

			this.Fields = dict;
			this.Parameters = dict.Select(i => (IPattern)new TypesPattern(i.Key, i.Value)).ToList();
		}

		public IValue MakeInstance(params IValue[] values) {
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

		public void SetMember(String name, IValue value, Scope scope) {

		}

		public override String ToString() {
			return $"{this.Name}";
		}

		public Boolean TryGetMember(String name, out IValue result) {
			return this.Parent.TryGetMember(name, out result);
		}

		public Boolean IsParentOf(IValue value) {
			IType parent = value.Type;

			if (parent == this) {
				return true;
			}

			return false;
		}

		public IValue GetMember(String name, Scope scope) {
			if (this.TryGetMember(name, out IValue result)) {
				return result;
			}

			throw new LumenException($"fne {name} {this.Name}");
		}

		public virtual IValue Call(Scope e, params IValue[] arguments) {
			if (this.Parameters.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			return this.MakeInstance(arguments);
		}

		public Boolean HasImplementation(Module typeClass) {
			return this.Parent.HasImplementation(typeClass);
		}
	}
}
