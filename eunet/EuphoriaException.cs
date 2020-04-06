using System;

namespace Argent.Xenon {
	public class XenonException : Exception {
		public Int32 line;
		public String file;
		public String functionName;

		public XenonException(String message, String functionName = null, Int32 line = -1, String fileName = null) : base(message) {
			this.line = line;
			this.file = fileName;
			this.functionName = functionName;
		}
	}

	public static class Exceptions {
		public const String ATTEMPT_TO_REDEFINE = "attempt to redefine '{0}'";
		public const String TYPE_CHECK_FAILURE = "type check failure '{0}'";
		public const String HAS_NOT_BEEN_DECLARED = "'{0}' has not been declared";
		public const String HAS_NOT_BEEN_ASSIGNED = "'{0}' has not been assigned a value";

		public const String INCORRECT_LENGTH = "incorrect length";

		public const String INVALID_OPERATION = "invalid operations";

		public const String VALUE_NOT_CALLABLE = "value is not callable";

		public const String WAIT_A_TYPE = "wait a type id";

		public static String Format(this String input, params Object[] args) {
			return String.Format(input, args);
		}
	}
}
