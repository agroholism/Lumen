using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public static class Helper {
		public static Value CallMethod(this Value self, String name, Scope scope) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Run(new Scope(scope), self);
		}

		public static Value CallMethod(this Value self, String name, Scope scope, Value arg) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Run(new Scope(scope), self, arg);
		}

		public static IType CreateConstructor(String name, Module baseType, List<String> fields) {
			if (fields.Count == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Constructor result = new Constructor(name, baseType, fields.ToList());

			return result;
		}

		public static IType CreateConstructor(String name, Module baseType, String[] fields) {
			if (fields.Length == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Constructor result = new Constructor(name, baseType, fields.ToList());

			return result;
		}

		public static Instance CreateSome(Value value) {
			Instance result = new Instance(Prelude.Some);

			result.items[0] = value;

			return result;
		}

		public static Instance CreatePair(Value key, Value value) {
			Instance result = new Instance(MapModule.PairModule.ctor as Constructor);

			result.items[0] = key;
			result.items[1] = value;

			return result;
		}

		public static Instance CreatePair(KeyValuePair<Value, Value> pair) {
			Instance result = new Instance(MapModule.PairModule.ctor as Constructor);

			result.items[0] = pair.Key;
			result.items[1] = pair.Value;

			return result;
		}

		internal static Value FromStream(IType type, IEnumerable<Value> values, Scope scope) {
			if (type == Prelude.List) {
				return new List(LinkedList.Create(values));
			}

			if (type == Prelude.Array) {
				return new Array(values.ToList());
			}

			if (type == Prelude.Stream) {
				return new Stream(values);
			}

			return (type.GetMember("fromStream", scope) as Fun).Run(new Scope(scope), new Stream(values));
		}

		public static Value MakePartial(Fun function, Value[] vals) {
			return new PartialFun {
				InnerFunction = function,
				Args = vals,
				restArgs = function.Arguments.Count - vals.Length
			};
		}

		public static Double Tanimoto(String first, String second) {
			first = first.ToLower();
			second = second.ToLower();

			Int32 c = 0;
			foreach(Char i in first) {
				if(second.Contains(i)) {
					c++;
				}
			}

			return c / (first.Length + second.Length - c);
		}
	}
}
