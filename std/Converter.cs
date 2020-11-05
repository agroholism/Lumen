using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public static class Converter {
		public static Boolean ToBoolean(this Value value) {
			return value is Logical logical ? logical : value != Const.UNIT;
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
				throw Helper.CreateConvertError(value.Type, Prelude.Function).ToException(scope);
		}

		public static Dictionary<Value, Value> ToDictionary(this Value value, Scope scope) {
			return value is Map map ? map.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.Map).ToException(scope);
		}

		public static List<Value> ToList(this Value value, Scope scope) {
			return value is Array array ? array.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException(scope);
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

			throw Helper.CreateConvertError(val.Type, Prelude.Stream).ToException(scope);
		}

		public static Boolean TryConvertToException(this Value value, out LumenException result) {
			if (value is Text) {
				result = new LumenException(value.ToString());
				return true;
			}

			if (value is IExceptionConstructor exc) {
				result = exc.MakeExceptionInstance();
				return true;
			}

			if (value is LumenException lev) {
				result = lev;
				return true;
			}

			result = null;
			return false;
		}

		public static LumenException ToException(this Value value, Scope scope) {
			return value.TryConvertToException(out LumenException result) ? result 
				: Helper.CreateConvertError(value.Type, Prelude.Collection).ToException(scope);
		}

		internal static LinkedList ToLinkedList(this Value value, Scope scope) {
			return value is List list ? list.Value :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException(scope);
		}

		private static Number ToNum(this Value value, Scope scope) {
			if (value is Number number) {
				return number;
			}

			if (value.Type.TryGetMember("toNumber", out Value converterPrototype)
					&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				return ToNum(converter.Run(new Scope(scope), value), scope);
			}

			throw Helper.CreateConvertError(value.Type, Prelude.Number).ToException(scope);
		}
	}
}
