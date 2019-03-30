using System;

namespace Lumen.Lang {
    public static class Exceptions {
        /// <summary> code L001 </summary>
        public const String INCORRECT_NUMBER_LITERAL = "incorrect number literal";
        public const String CAN_NOT_TO_CHANGE_BINDING = "can not to change binding '{0}'";
        public const String WAIT_ANOTHER_TOKEN = "wait token '{0}' given token '{1}'";
        public const String UNEXCEPTED_TOKEN = "unexcepted token '{0}'";
        public const String UNCLOSED_LIST_LITERAL = "unclosed list literal";
        public const String UNCLOSED_STRING_LITERAL = "unclosed string literal";
        public const String UNCLOSED_ARRAY_LITERAL = "unclosed array literal";
        public const String UNCLOSED_LAMBDA = "unclosed lambda";

        public const String NOT_A_FUNCTION = "'{0}' is not a function";
        public const String MODULE_DOES_NOT_CONTAINS_FUNCTION = "function '{0}' does not exists in module '{1}'";
        public const String UNKNOWN_IDENTIFITER = "unknown identifiter '{0}'";
        public const String TYPE_ERROR = "expected value of type '{0}', passed value of type '{1}'";
        public const String CONVERT_ERROR = "can not convert a value of type '{0}' to value of type '{1}'";

        public const String INDEX_OUT_OF_RANGE = "index out of range";
    }

    public static class Utils {
        public static String F(this String val, params Object[] vals) {
            return String.Format(val, vals);
        }
    }
}
