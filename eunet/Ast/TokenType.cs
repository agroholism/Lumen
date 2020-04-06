namespace Argent.Xenon.Ast {
	enum TokenType {
		TEXT,
		NUMBER,

		IDENTIFIER,

		DOT,
		DOTDOT,
		DOTDOTDOT,

		RBRACE,
		LBRACE,

		RPAREN,
		LPAREN,

		PLUS,
		MINUS,
		SLASH,
		STAR,

		EQ,
		NOTEQ,
		LESS,
		LESSEQ,
		GREATER,
		GREATEREQ,

		AND,
		NOT,
		OR,
		XOR,

		BREAK,
		RETURN,

		SPLIT,
		EOC,
		EOF,
		EQUALS,
		LBRACKET,
		RBRACKET,
		FUNCTION,
		END,
		TYPE,
		IF,
		ELSE,
		ELIF,
		THEN,
		AUTO,
		FOR,
		IN,
		DO,
		QUESTION,
		DQ,
		EXIT,
		TO,
		YIELD,
		AS,
		COLON,
		VAR,
		IMMUT
	}
}
