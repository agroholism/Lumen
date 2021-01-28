using System;

namespace Lumen.Lang {
	public static class Constants {
		public const String YIELD_VALUE_SPECIAL_NAME = "[yield-value]";
		public const String YIELD_EXCEPTION_SPECIAL_NAME = "[yield-exception]";
		public const String LAST_EXCEPTION_SPECIAL_NAME = "[last-exception]";

		public const String NOT = "not";
		public const String AND = "and";
		public const String OR = "or";
		public const String XOR = "xor";

		public const String EQUALS = "=";
		public const String NOT_EQUALS = "<>";
		public const String LT = "<";
		public const String GT = ">";
		public const String LESS_EQUALS = "<=";
		public const String GREATER_EQUALS = ">=";
		public const String SHIP = "<=>";

		public const String PLUS = "+";
		public const String UPLUS = "+@";
		public const String MINUS = "-";
		public const String UMINUS = "-@";
		public const String STAR = "*";
		public const String USTAR = "*@";
		public const String SLASH = "/";

		public const String BNOT = "op_bnot";
		public const String BAND = "op_band";
		public const String BOR = "op_bor";
		public const String UNARY_XOR = "op_bxor";
		public const String UBXOR = "op_ubxor";
		public const String LSH = "op_lsh";
		public const String RSH = "op_rsh";

		public const String GETI = "getIndex";
		public const String SETI = "setIndex";

		public const String MOD = "op_mod";
		public const String DIV = "op_div";

		public const String POW = "^";

		public const String APLUS = "op_aplus";
		public const String AMINUS = "op_aminus";
		public const String ASTAR = "op_astar";
		public const String ASLASH = "op_aslash";
		public const String ABAND = "op_aband";
		public const String APOW = "op_apow";
		public const String AMOD = "op_amod";

		public const String MATCH_EQUALS = "op_match";
		public const String NOT_MATCH_EQUALS = "op_not_match";

		public const String RANGE_EXCLUSIVE = "rangeExcl";
		public const String RANGE_INCLUSIVE = "rangeIncl";
	}
}
