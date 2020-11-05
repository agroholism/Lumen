using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public static class Exceptions {
		/// <summary> code L001 </summary>
		public const String INCORRECT_NUMBER_LITERAL = "incorrect number literal";

		public const String WAIT_ANOTHER_TOKEN = "wait token \"{0}\" given token \"{1}\"";
		public const String UNEXCEPTED_TOKEN = "unexcepted token \"{0}\"";
		public const String UNCLOSED_LIST_LITERAL = "unclosed list literal";
		public const String UNCLOSED_STRING_LITERAL = "unclosed string literal";
		public const String UNCLOSED_ARRAY_LITERAL = "unclosed array literal";
		public const String UNCLOSED_LAMBDA = "unclosed lambda";

		public const String CAN_NOT_EXTEND_VALUE_OF_TYPE = "can not extend value of type \"{0}\"";
		public const String IDENTIFIER_IS_ALREADY_EXISTS = "identifier \"{0}\" is already exists in this scope";
		public const String IDENTIFIER_IS_ALREADY_EXISTS_IN_MODULE = "identifier \"{0}\" is already exists in module \"{1}\"";

		public const String NOT_A_FUNCTION = "value of type \"{0}\" is not a function";
		public const String MODULE_DOES_NOT_CONTAINS_FUNCTION = "function \"{0}\" does not exists in module \"{1}\"";
		public const String UNKNOWN_NAME = "unknown name \"{0}\"";
		public const String TYPE_ERROR = "expected value of type \"{0}\", passed value of type \"{1}\"";
		public const String CONVERT_ERROR = "can not convert a value of type \"{0}\" to value of type \"{1}\"";

		public const String PACKAGE_DOES_NOT_EXISTS = "package \"{0}\" does not exists";
		public const String MODULE_DOES_NOT_EXISTS = "module \"{0}\" does not exists";

		public const String FUNCTION_CAN_NOT_BE_APPLIED = "function with signature {0} can not be applied";

		public const String NAME_CAN_NOT_BE_DEFINED = "name can not be defined";
		public const String NAME_OF_BINDING_SHOULD_STARTS_WITH_SMALL_LETTER = "name of binding should starts with small letter: \"{0}\"";

		public const String TYPE_NOT_IMPLEMENTED_TYPE_CLASS = "type \"{0}\", does not implement type class \"{1}\"";

		public const String FUNCTION_IS_NOT_IMPLEMENTED_FOR_TYPE = "function \"{0}\" is not implemented for type \"{1}\"";

		public const String ASSERT_IS_BROKEN = "assert is broken";

		public const String INSTANCE_OF_DOES_NOT_CONTAINS_FIELD = "instance of \"{0}\" does not contains field \"{1}\"";

		public const String INDEX_OUT_OF_RANGE = "index out of range";
	}

	public static class Utils {
		public static String F(this String val, params Object[] vals) {
			return String.Format(val, vals);
		}

		public static String ArgumentsToString(IEnumerable<Expressions.Expression> exps) {
			return String.Join(" ", exps);
		}

		public static String Bodify(Expression body) {
			return body.ToString();
		}
	}
}
