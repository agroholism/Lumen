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

		public static LumenException IndexOutOfRange() {
			return Prelude.IndexOutOfRange.MakeExceptionInstance();
		}

		public static LumenException InvalidOperation(String message) {
			return Prelude.InvalidOperation.MakeExceptionInstance(new Text(message));
		}

		public static LumenException CreateConvertError(IType fromType, IType targetType) {
			return Prelude.ConvertError.MakeExceptionInstance(fromType, targetType);
		}

		public static Value CallMethod(this Value self, String name, Scope scope) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Call(new Scope(scope), self);
		}

		public static Value CallMethod(this Value self, String name, Scope scope, Value arg) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Call(new Scope(scope), self, arg);
		}

		public static Value CallMethodFlip(this Value self, String name, Scope scope, Value arg) {
			return self.Type.GetMember(name, scope).ToFunction(scope).Call(new Scope(scope), arg, self);
		}

		public static IConstructor CreateConstructor(String name, Module baseType, List<String> fields) {
			if (fields.Count == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Dictionary<String, List<IType>> dict = new();
			foreach(var i in fields) {
				dict.Add(i, new List<IType>());
			}

			Constructor result = new Constructor(name, baseType, dict);

			return result;
		}

		public static IConstructor CreateConstructor(String name, Module baseType, Dictionary<String, List<IType>> fields) {
			if (fields.Count == 0) {
				return new SingletonConstructor(name, baseType);
			}

			Constructor result = new Constructor(name, baseType, fields);

			return result;
		}

		public static IConstructor CreateConstructor(String name, Module baseType, params String[] fields) {
			if (fields.Length == 0) {
				return new SingletonConstructor(name, baseType);
			}


			Dictionary<String, List<IType>> dict = new();
			foreach (var i in fields) {
				dict.Add(i, new List<IType>());
			}


			Constructor result = new Constructor(name, baseType, dict);

			return result;
		}

		public static Instance Success(Value value) {
			Instance result = new Instance(Prelude.Success);
			result.Items[0] = value;
			return result;
		}

		public static Instance Failed(Value value) {
			Instance result = new Instance(Prelude.Failed);
			result.Items[0] = value;
			return result;
		}

		public static Instance CreateSome(Value value) {
			Instance result = new Instance(Prelude.Some);

			result.Items[0] = value;

			return result;
		}

		public static Instance CreatePair(Value key, Value value) {
			Instance result = new Instance(MutMapModule.PairModule.ctor as Constructor);

			result.Items[0] = key;
			result.Items[1] = value;

			return result;
		}

		public static Value CreatePair(KeyValuePair<Value, Value> pair) {
			return new List(pair.Key, pair.Value);
		}

		internal static Value FromSeq(IType type, IEnumerable<Value> values, Scope scope) {
			if (type == Prelude.List) {
				return new List(LinkedList.Create(values));
			}

			if (type == Prelude.MutArray) {
				return new MutArray(values.ToList());
			}

			if (type == Prelude.Seq) {
				return new Seq(values);
			}

			return (type.GetMember("fromSeq", scope) as Fun).Call(new Scope(scope), new Seq(values));
		}

		public static Value MakePartial(Fun function, Value[] vals) {
			return new PartialFun {
				InnerFunction = function,
				Args = vals,
				restArgs = function.Parameters.Count - vals.Length
			};
		}

		public static Int32 Levenshtein(String s, String t) {
			Int32 n = s.Length;
			Int32 m = t.Length;
			Int32[,] d = new Int32[n + 1, m + 1];

			// Verify arguments.
			if (n == 0) {
				return m;
			}

			if (m == 0) {
				return n;
			}

			// Initialize arrays.
			for (Int32 i = 0; i <= n; d[i, 0] = i++) {
			}

			for (Int32 j = 0; j <= m; d[0, j] = j++) {
			}

			// Begin looping.
			for (Int32 i = 1; i <= n; i++) {
				for (Int32 j = 1; j <= m; j++) {
					// Compute cost.
					Int32 cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost
					);
				}
			}
			// Return cost.
			return d[n, m];
		}

		internal static LumenException InvalidArgument(String name, String message) {
			return Prelude.InvalidArgument.MakeExceptionInstance(new Text(name), new Text(message));
		}
	}
}
