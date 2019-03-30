using System;

namespace ldoc {
    public static class Exceptions {
        /// <summary> code L001 </summary>
        public const String INCORRECT_NUMBER_LITERAL = "incorrect number literal";

        public const String NOT_A_FUNCTION = "'{0}' is not a function";
        public const String MODULE_DOES_NOT_CONTAINS_FUNCTION = "function '{0}' does not exists in module '{1}'";
        public const String UNKNOWN_IDENTIFITER = "unknown identifiter '{0}'";
        public const String TYPE_ERROR = "expected value of type '{0}', passed value of type '{1}'";
        public const String CONVERT_ERROR = "can not convert a value of type '{0}' to value of type '{1}'";
    }

    public static class Utils {
        public static String F(this String val, params Object[] vals) {
            return String.Format(val, vals);
        }
    }
}
