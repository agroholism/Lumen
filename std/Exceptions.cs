using System;

namespace Lumen.Lang.Std {
	public static class Exceptions {
		public const String TYPE_ERROR = "expected value of type '{0}', passed value of type '{1}'";
		public const String CONVERT_ERROR = "can not convert a value of type '{0}' to value of type '{1}'";
	}

	public static class Utils {
		public static String F(this String val, params Object[] vals) {
			return String.Format(val, vals);
		}
	}
}
