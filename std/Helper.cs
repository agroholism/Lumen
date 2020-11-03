using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public static class Helper {
		public static Int32 Index(Int32 index, Int32 count) {
			if (index < 0) {
				return count + index;
			}
			return index;
		}

		public static Value Error(String message) {
			return Prelude.Error.constructor.MakeInstance(new Text(message));
		}

		public static LumenException ConvertError(IType fromType, IType targetType, Scope scope) {
			return Prelude.ConvertError.constructor.MakeInstance(fromType, targetType).ToException(scope);
		}

		public static Value CallMethod(this Value self, String name, Scope scope) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Run(new Scope(scope), self);
		}

		public static Value CallMethod(this Value self, String name, Scope scope, Value arg) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Run(new Scope(scope), self, arg);
		}

		public static Value CallMethodFlip(this Value self, String name, Scope scope, Value arg) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Run(new Scope(scope), arg, self);
		}

		public static IConstructor CreateConstructor(String name, Module baseType, List<String> fields) {
			if (fields.Count == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Constructor result = new Constructor(name, baseType, fields.ToList());

			return result;
		}

		public static IConstructor CreateConstructor(String name, Module baseType, params String[] fields) {
			if (fields.Length == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Constructor result = new Constructor(name, baseType, fields.ToList());

			return result;
		}

		public static Instance CreateSome(Value value) {
			Instance result = new Instance(Prelude.Some);

			result.Items[0] = value;

			return result;
		}

		public static Instance CreatePair(Value key, Value value) {
			Instance result = new Instance(MapModule.PairModule.ctor as Constructor);

			result.Items[0] = key;
			result.Items[1] = value;

			return result;
		}

		public static Value CreatePair(KeyValuePair<Value, Value> pair) {
			return new List(pair.Key, pair.Value);
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
			foreach (Char i in first) {
				if (second.Contains(i)) {
					c++;
				}
			}

			return c / (first.Length + second.Length - c);
		}
	}
}
