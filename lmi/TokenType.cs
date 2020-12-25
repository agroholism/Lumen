namespace Lumen.Lmi {
	public enum TokenType {
        NUMBER,
        WORD,
        TEXT,

        COLON,
        COLON2,

        PLUS,
        MINUS,
        STAR,
        SLASH,
        POWER,
        MODULUS,

        ASSIGN, // <-
        LAMBDA, // ->

        TILDE,
        AMP,
        BAR,
        BANG,

        MIDDLE_PRIORITY_RIGTH,

        SHIFT_LEFT,
        SHIFT_RIGTH,

        SHIP,
        MATCH_EQUALS,
        NOT_MATCH_EQUALS,
        EQUALS,
        NOT_EQUALS,
        LESS,
        LESS_EQUALS,
        GREATER,
        GREATER_EQUALS,

        NOT,
        OR,
        AND,
        XOR,

        DOT,
        DOT2,
        DOT3,

        PAREN_OPEN,
        PAREN_CLOSE,

        BLOCK_START,
        BLOCK_END,

        LIST_OPEN,
        COLLECTION_CLOSE,

        ARRAY_OPEN,
        ARRAY_CLOSE,

        SPLIT,

        EOC,
        EOF,

        VOID,

        FORWARD_PIPE,
        BACKWARD_PIPE,

        LET,
        RETURN,
        IF,
        ELSE,
        WHILE,
        FOR,
        IN,
        BREAK,
        MODULE,
        NEXT,
        MATCH,
        TYPE,
        IMPORT,
        AS,
        TAIL_REC,
        IMPLEMENTS,
		REC,
		YIELD,
		FROM,
		ATTRIBUTE,
		QUESTION,
		USE,
		RAISE,
		FUN,
		TRY,
		EXCEPT,
		ENSURE,
		WHEN,
		REDO,
		RETRY,
		ANNOTATION,
		DOT_LESS,
		ASSERT,
		SEQ_OPEN
	}
}
