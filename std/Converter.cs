using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public static class Converter {
		public static Boolean ToBoolean(this Value value) {
			return value is Logical logical ? (Boolean)logical : value != Const.UNIT;
		}

		public static Double ToDouble(this Value value, Scope scope) {
			return value is Number number ? number.value : ToDouble(ToNum(value, scope), scope);
		}

		public static Int32 ToInt(this Value value, Scope scope) {
			if (value is Number number) {
				return number.value % 1 == 0 ? (Int32)number.value :
					throw new LumenException("required integer number");
			}

			return ToInt(ToNum(value, scope), scope);
		}

		public static Boolean TryConvertToFunction(this Value value, out Fun function) {
			switch (value) {
				case Fun _function:
					function = _function;
					return true;

				case Module module when module.TryGetMember("<init>", out Value init):
					return init.TryConvertToFunction(out function);

				default:
					function = null;
					return false;
			}
		}

		public static Fun ToFunction(this Value value, Scope scope) {
			return value.TryConvertToFunction(out Fun result) ? result :
				throw Helper.ConvertError(value.Type, Prelude.Function, scope);
		}

		public static Dictionary<Value, Value> ToDictionary(this Value value, Scope scope) {
			return value is Map map ? map.InternalValue :
				throw Helper.ConvertError(value.Type, Prelude.Map, scope);
		}

		public static List<Value> ToList(this Value value, Scope scope) {
			return value is Array array ? array.InternalValue :
				throw Helper.ConvertError(value.Type, Prelude.List, scope);
		}

		public static IEnumerable<Value> ToStream(this Value val, Scope scope) {
			if (val is Stream stream) {
				return stream.InternalValue;
			}

			if (val.Type.HasImplementation(Prelude.Collection)
				&& val.Type.TryGetMember("toStream", out Value converterPrototype)
				&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				return converter.Run(new Scope(scope), val).ToStream(scope);
			}

			throw Helper.ConvertError(val.Type, Prelude.Stream, scope);
		}

		public static LumenException ToException(this Value value, Scope scope) {
			String message = value.Type.GetMember("message", scope).ToFunction(scope)
				.Run(new Scope(scope), value).ToString();

			return new LumenException(message) {
				LumenObject = value
			};
		}

		internal static LinkedList ToLinkedList(this Value value, Scope scope) {
			return value is List list ? list.Value :
				throw Helper.ConvertError(value.Type, Prelude.List, scope);
		}

		private static Number ToNum(this Value value, Scope scope) {
			if (value is Number number) {
				return number;
			}

			if (value.Type.TryGetMember("toNumber", out Value converterPrototype)
					&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				return ToNum(converter.Run(new Scope(scope), value), scope);
			}

			throw Helper.ConvertError(value.Type, Prelude.Number, scope);
		}
	}
}
