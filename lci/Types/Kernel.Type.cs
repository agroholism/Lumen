using System;
using System.Linq;
using System.Collections.Generic;

namespace StandartLibrary {
	[Serializable]
	internal class TypeType : KType {
		public new Dictionary<String, Value> Attributes {
			get {
				Dictionary<String, Value> result = new Dictionary<string, Value>();
				foreach(KeyValuePair<String, Value> i in this.typeAttributes) {
					result.Add(i.Key, i.Value);
				}
				foreach (KeyValuePair<String, Fun> i in this.attributes) {
					result.Add(i.Key, i.Value);
				}
				return result;
			}
		}

		public TypeType() {
			this.meta = new TypeMetadata {
				Fields = new String[0],
				Name = "Kernel.Type"
			};

			Set("new", new LambdaFun((e, args) => {
				KType result = new KType();

				String name = args[0].ToString(e);
				List<String> fields = Converter.ToList(args[1], e).Select(i => i.ToString(e)).ToList();

				/*result.meta = new TypeMetadata {
					Name = name,
					BaseType = 
				};
				*/
				return result;
			}));

			/*SetAttribute("get_superclass", new LambdaFun((e, args) => {
				KType one = (KType)e.This;
				return one.meta.BaseType;
			}));*/

			/*SetAttribute("get_methods", new LambdaFun((e, args) => {
				KType one = (KType)e.This;
				return new List(one.Attributes.Select(i => (Value)(KString)i.Key).ToList());
			}));*/
		}
	}

	/*internal class FunctionMatch : Value, Isable {
		FunctionMetadata fmtd;

		public FunctionMatch(FunctionMetadata fmtd) {
			this.fmtd = fmtd;
		}

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public Value Clone() {
			return this;
		}

		public KType Type => null;

		public bool Match(Value value) {
			if (value is UserFun uf && uf.meta.args.Count == fmtd.args.Count) {
				for (Int32 i = 0; i < fmtd.args.Count; i++) {
					if(fmtd.args[i].type != uf.meta.args[i].type) {
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public string ToString(Scope e) {
			return "<><><>";
		}
	}

	public class ListTemplate : Value, Isable {
		Isable one;

		public ListTemplate(Isable one) {
			this.one = one;
		}

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public Value Clone() {
			return this;
		}

		public KType Type => null;

		public bool Match(Value value) {
			if (value is KList list) {
				foreach (var i in KernelConverter.ToList(list))
					if (!one.Match(i))
						return false;
				return true;
			}
			return false;
		}

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public string ToString(Scope e) {
			return one + "[]";
		}
	}

	internal class IntTemplate : Value, Isable {
		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public Value Clone() {
			return new IntTemplate();
		}

		public KType Type => null;

		public bool Match(Value value) {
			if (value is Number fix)
				return fix.value % 1 == 0;
			return false;
		}

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public string ToString(Scope e) {
			return "Int";
		}
	}*/
}
