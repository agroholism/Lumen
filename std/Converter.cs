using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public static class Converter {
		public static LumenException From(this LumenException value, LumenException other) {
			value.Cause = other;
			return value;
		}

		public static Boolean IsFailed(this IValue value) {
			return Prelude.Failed.IsParentOf(value);
		}

		public static Boolean IsSuccess(this IValue value) {
			return Prelude.Success.IsParentOf(value);
		}

		public static Boolean ToBoolean(this IValue value) {
			return value is Logical logical ? logical : value != Const.UNIT;
		}

		public static Double ToDouble(this IValue value, Scope scope) {
			return value is Number number ? number.value : ToDouble(ToNum(value, scope), scope);
		}

		public static Int32 ToInt(this IValue value, Scope scope) {
			if (value is Number number) {
				return number.value % 1 == 0 ? (Int32)number.value :
					throw new LumenException("required integer number");
			}

			return ToInt(ToNum(value, scope), scope);
		}

		public static Boolean TryConvertToFunction(this IValue value, out Fun function) {
			switch (value) {
				case Fun _function:
					function = _function;
					return true;

				case Module module when module.TryGetMember("<init>", out IValue init):
					return init.TryConvertToFunction(out function);

				default:
					function = null;
					return false;
			}
		}

		public static Fun ToFunction(this IValue value, Scope scope) {
			return value.TryConvertToFunction(out Fun result) ? result :
				throw Helper.CreateConvertError(value.Type, Prelude.Function).ToException();
		}

		public static Dictionary<IValue, IValue> ToDictionary(this IValue value, Scope scope) {
			return value is MutMap map ? map.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.MutMap).ToException();
		}

		public static List<IValue> ToList(this IValue value, Scope scope) {
			return value is MutArray array ? array.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException();
		}

		public static Boolean TryConvertToSeq(this IValue value, Scope scope, out IEnumerable<IValue> result) {
			if (value is Flow stream) {
				result = stream.InternalValue;
				return true;
			}

			if (value is IEnumerable<IValue> numberRange) {
				result = numberRange;
				return true;
			}

			if (value.Type.HasImplementation(Prelude.Collection)
				&& value.Type.TryGetMember("toSeq", out IValue converterPrototype)
				&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				result = converter.Call(new Scope(scope), value).ToFlow(scope);
				return true;
			}

			result = null;
			return false;
		}

		public static IEnumerable<IValue> ToFlow(this IValue value, Scope scope) {
			if(value.TryConvertToSeq(scope, out var result)) {
				return result;
			}

			throw Helper.CreateConvertError(value.Type, Prelude.Flow).ToException();
		}

		public static Boolean TryConvertToException(this IValue value, out LumenException result) {
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

		public static LumenException ToException(this IValue value) {
			return value.TryConvertToException(out LumenException result) ? result 
				: Helper.CreateConvertError(value.Type, Prelude.Exception).ToException();
		}

		internal static LinkedList ToLinkedList(this IValue value, Scope scope) {
			return value is List list ? list.Value :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException();
		}

		private static Number ToNum(this IValue value, Scope scope) {
			if (value is Number number) {
				return number;
			}

			if (value.Type.TryGetMember("toNumber", out IValue converterPrototype)
					&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				return ToNum(converter.Call(new Scope(scope), value), scope);
			}

			throw Helper.CreateConvertError(value.Type, Prelude.Number).ToException();
		}

		public static Future ToFuture(this IValue value, Scope scope) {
			if(value is Future future) {
				return future;
			} 

			throw Helper.CreateConvertError(value.Type, Prelude.Future).ToException();
		}
	}
}
