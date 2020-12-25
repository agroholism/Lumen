using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public static class Converter {
		public static LumenException From(this LumenException value, LumenException other) {
			value.Cause = other;
			return value;
		}

		public static Boolean IsFailed(this Value value) {
			return Prelude.Failed.IsParentOf(value);
		}

		public static Boolean IsSuccess(this Value value) {
			return Prelude.Success.IsParentOf(value);
		}

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
				throw Helper.CreateConvertError(value.Type, Prelude.Function).ToException();
		}

		public static Dictionary<Value, Value> ToDictionary(this Value value, Scope scope) {
			return value is MutMap map ? map.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.MutMap).ToException();
		}

		public static List<Value> ToList(this Value value, Scope scope) {
			return value is MutArray array ? array.InternalValue :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException();
		}

		public static Boolean TryConvertToSeq(this Value value, Scope scope, out IEnumerable<Value> result) {
			if (value is Seq stream) {
				result = stream.InternalValue;
				return true;
			}

			if (value is NumberRange numberRange) {
				result = numberRange;
				return true;
			}

			if (value.Type.HasImplementation(Prelude.Collection)
				&& value.Type.TryGetMember("toSeq", out Value converterPrototype)
				&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				result = converter.Call(new Scope(scope), value).ToSeq(scope);
				return true;
			}

			result = null;
			return false;
		}

		public static IEnumerable<Value> ToSeq(this Value value, Scope scope) {
			if(value.TryConvertToSeq(scope, out var result)) {
				return result;
			}

			throw Helper.CreateConvertError(value.Type, Prelude.Seq).ToException();
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

		public static LumenException ToException(this Value value) {
			return value.TryConvertToException(out LumenException result) ? result 
				: Helper.CreateConvertError(value.Type, Prelude.Collection).ToException();
		}

		internal static LinkedList ToLinkedList(this Value value, Scope scope) {
			return value is List list ? list.Value :
				throw Helper.CreateConvertError(value.Type, Prelude.List).ToException();
		}

		private static Number ToNum(this Value value, Scope scope) {
			if (value is Number number) {
				return number;
			}

			if (value.Type.TryGetMember("toNumber", out Value converterPrototype)
					&& converterPrototype.TryConvertToFunction(out Fun converter)) {
				return ToNum(converter.Call(new Scope(scope), value), scope);
			}

			throw Helper.CreateConvertError(value.Type, Prelude.Number).ToException();
		}

		public static Future ToFuture(this Value value, Scope scope) {
			if(value is Future future) {
				return future;
			} 

			throw Helper.CreateConvertError(value.Type, Prelude.Future).ToException();
		}
	}
}
