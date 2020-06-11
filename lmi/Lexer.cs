using System;
using System.Collections.Generic;
using System.Text;

using Lumen.Lang;

namespace Lumen.Lmi {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!$&|;:?,[]^.~$";
		private static readonly IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["()"] = new Token(TokenType.VOID, "()"),

			["?"] = new Token(TokenType.QUESTION, "?"),
			["~"] = new Token(TokenType.TILDE, Op.BNOT),

			["+"] = new Token(TokenType.PLUS, Op.PLUS),
			["-"] = new Token(TokenType.MINUS, Op.MINUS),
			["*"] = new Token(TokenType.STAR, Op.STAR),
			["/"] = new Token(TokenType.SLASH, Op.SLASH),
			["!"] = new Token(TokenType.BANG, "!"),

			["%"] = new Token(TokenType.MOD, Op.MOD),
			["^"] = new Token(TokenType.BXOR, Op.POW),
			["&"] = new Token(TokenType.AMP, Op.BAND),
			["|"] = new Token(TokenType.BAR, Op.BOR),

			["<<"] = new Token(TokenType.BLEFT, Op.LSH),
			[">>"] = new Token(TokenType.BRIGTH, Op.RSH),

			["<<="] = new Token(TokenType.BLEFT, "<<="),
			[">>-"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, ">>-"),
			[">>="] = new Token(TokenType.BIND, ">>="),

			["=>"] = new Token(TokenType.CONTEXT, Op.RSH),

			["("] = new Token(TokenType.LPAREN),
			[")"] = new Token(TokenType.RPAREN),

			["<$"] = new Token(TokenType.NOT_EQUALS),
			["<$>"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH),

			["="] = new Token(TokenType.EQUALS, Op.EQUALS),
			["<"] = new Token(TokenType.LT, Op.LT),
			[">"] = new Token(TokenType.GT, Op.GT),
			["<*"] = new Token(TokenType.NOT_EQUALS, Op.NOT_EQL),
			["<*>"] = new Token(TokenType.APPLICATIVEF, Op.NOT_EQL),
			["<>"] = new Token(TokenType.NOT_EQUALS, Op.NOT_EQL),
			["<="] = new Token(TokenType.LTEQ, Op.LTEQ),
			[">="] = new Token(TokenType.GTEQ, Op.GTEQ),
			["<=>"] = new Token(TokenType.SHIP, Op.SHIP),

			["=~"] = new Token(TokenType.EQMATCH, Op.MATCH),
			["!~"] = new Token(TokenType.EQNOTMATCH, Op.NOT_MATCH),

			["->"] = new Token(TokenType.LAMBDA, Op.STAR),
			["<-"] = new Token(TokenType.ASSIGN, "<-"),

			["::"] = new Token(TokenType.COLONCOLON),

			[":"] = new Token(TokenType.COLON),
			[";"] = new Token(TokenType.EOC, ";"),
			[","] = new Token(TokenType.SPLIT, ","),
			["."] = new Token(TokenType.DOT, "."),

			[".."] = new Token(TokenType.DOTDOT, Op.RANGE_EXCLUSIVE),
			["..."] = new Token(TokenType.DOTDOTDOT, Op.RANGE_INCLUSIVE),

			["["] = new Token(TokenType.LBRACKET, "["),
			["]"] = new Token(TokenType.RBRACKET, "]"),

			["[|"] = new Token(TokenType.ARRAY_OPEN, "[|"),
			["|]"] = new Token(TokenType.ARRAY_CLOSED, "|]"),

			["[<"] = new Token(TokenType.ATTRIBUTE_OPEN, "[<"),
			[">]"] = new Token(TokenType.ATTRIBUTE_CLOSE, ">]"),

			["|>"] = new Token(TokenType.FPIPE, "|>"),
			["<|"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "<|")
		};
		private readonly String source;
		private readonly Int32 length;
		private readonly List<Token> tokens;
		private Int32 position;
		private Int32 line;
		private readonly String file;
		private Int32 level;

		internal Lexer(String source, String file) {
			this.file = file;
			this.source = source;
			this.length = source.Length;
			this.tokens = new List<Token>();
			this.position = 0;
			this.line = 1;
		}

		public List<Token> Tokenization() {
			while (this.position < this.length) {
				Char current = this.Peek(0);

				if (Char.IsDigit(current)) {
					this.Number();
				}
				else if (current == '"') {
					this.Text();
				}
				else if (Char.IsLetter(current) || current == '_' || current == '\'') {
					this.Word();
				}
				else if (operatorsString.IndexOf(current) != -1) {
					this.Operator();
				}
				else if (current == '\n') {
					this.Tabs();
				}
				else if (current == '@') {
					this.Attribute();
				}
				else {
					this.Next();
				}
			}

			if (this.level != 0) {
				for (Int32 i = 0; i < this.level; i += 4) {
					this.AddToken(TokenType.END);
				}
			}

			return this.tokens;
		}

		private void Attribute() {
			Next(); // @
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_' && current != '\'') {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

			AddToken(TokenType.ATTRIBUTE, word);
		}

		private void Text() {
			this.Next();

			Int32 line = this.line;

			StringBuilder builder = new StringBuilder();

			Int32 x = 0;

			Char current = this.Peek(0);

			List<String> substitutes = new List<String>();

			while (true) {
				if (current == '\\') {
					current = this.Next();
					switch (current) {
						case '"':
							current = this.Next();
							builder.Append('"');
							continue;
						case '\r': // edit this
							this.Next();
							current = this.Next();
							continue;
						case 'f':
							current = this.Next();
							builder.Append('\f');
							continue;
						case '\\':
							current = this.Next();
							builder.Append('\\');
							continue;
						case '0':
							current = this.Next();
							builder.Append('\0');
							continue;
						case 'a':
							current = this.Next();
							builder.Append('\a');
							continue;
						case 'b':
							current = this.Next();
							builder.Append('\b');
							continue;
						case 'r':
							current = this.Next();
							builder.Append('\r');
							continue;
						case 'v':
							current = this.Next();
							builder.Append('\v');
							continue;
						case 'e':
							current = this.Next();
							builder.Append(Environment.NewLine);
							continue;
						case 'n':
							current = this.Next();
							builder.Append('\n');
							continue;
						case 't':
							current = this.Next();
							builder.Append('\t');
							continue;
						case '(':
							// ...\("
							if (current == '"') {
								builder.Append('(');
								break;
							}

							StringBuilder modifiers = new StringBuilder();
							StringBuilder buffer = new StringBuilder();

							Int32 level = 1;
							current = this.Next();
							while (level > 0 && current != ')') {
								this.CheckOutOfRange();
								buffer.Append(current);
								current = this.Next();

								if (current == '(') {
									level++;
								}

								if (current == ')') {
									level--;
								}

								if (current == ':' && level == 1) {
									current = this.Next();
									while (current != ')') {
										modifiers.Append(current);
										current = this.Next();
									}
									level--;
								}
							}

							current = this.Next();

							String s = buffer.ToString();
							Int32 position = substitutes.IndexOf(s);

							String modif = modifiers.ToString();
							if (position == -1) {
								substitutes.Add(s);
								if (modif.Length == 0) {
									builder.Append($"{{{x}}}");
								}
								else {
									builder.Append($"{{{x}:{modif}}}");
								}

								x++;
							}
							else {
								if (modif.Length == 0) {
									builder.Append($"{{{position}}}");
								}
								else {
									builder.Append($"{{{position}:{modif}}}");
								}
							}
							continue;
						default:
							builder.Append('\\');
							continue;
					}
				}

				if (current == '{') {
					current = this.Next();
					builder.Append("{{");
				}

				if (current == '}') {
					current = this.Next();
					builder.Append("}}");
				}

				if (current == '"') {
					break;
				}

				this.CheckOutOfRange(line);

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			if (substitutes.Count == 0) {
				this.AddToken(TokenType.TEXT, builder.ToString().F());
			}
			else {
				this.AddToken(TokenType.LPAREN);
				this.AddToken(TokenType.TEXT, builder.ToString());

				this.AddToken(TokenType.MOD, Op.MOD);
				this.AddToken(TokenType.LBRACKET);

				List<Token> zzz = new Lexer(String.Join(", ", substitutes), this.file).Tokenization();
				foreach (Token i in zzz) {
					this.AddToken(i);
				}
				this.AddToken(TokenType.RBRACKET);
				this.AddToken(TokenType.RPAREN);
			}
		}

		private void CheckOutOfRange(Int32 line = -1) {
			if (this.position >= this.length) {
				throw new LumenException("consumed symbol '\"'", line: line, fileName: this.file);
			}
		}

		private void Tabs() {
			this.line++;

			Int32 newLevel = 0;

			Char current = this.Next();
			while (true) {
				if (current == ' ') {
					newLevel++;
				}
				else if (current == '\t') {
					newLevel += 4;
				}
				else {
					break;
				}
				current = this.Next();
			}

			if (newLevel < this.level) {
				for (Int32 i = 0; i < this.level - newLevel; i += 4) {
					this.AddToken(TokenType.END);
				}
			}

			if (newLevel > this.level) {
				for (Int32 i = 0; i < newLevel - this.level; i += 4) {
					this.AddToken(TokenType.DO);
				}
			}

			this.level = newLevel;

			if (this.tokens.Count > 0) {
				TokenType typeOfLast = this.tokens[this.tokens.Count - 1].Type;

				if (typeOfLast == TokenType.BREAK || typeOfLast == TokenType.RETURN) {
					this.AddToken(TokenType.NUMBER, "1");
				}
				else if (this.CanInsert()) {
					this.AddToken(TokenType.EOC);
				}
			}
		}

		private Boolean CanInsert() {
			TokenType last = this.tokens[this.tokens.Count - 1].Type;
			return last != TokenType.DO
				&& last != TokenType.AMP
				&& last != TokenType.AND
				&& last != TokenType.BAR
				&& last != TokenType.BLEFT
				&& last != TokenType.ARRAY_OPEN
				&& last != TokenType.AS
				&& last != TokenType.ASSIGN
				&& last != TokenType.ATTRIBUTE_OPEN
				&& last != TokenType.BIND
				&& last != TokenType.BPIPE
				&& last != TokenType.BRIGTH
				&& last != TokenType.BXOR
				&& last != TokenType.DOT
				&& last != TokenType.DOTDOT
				&& last != TokenType.DOTDOTDOT
				&& last != TokenType.ARRAY_OPEN
				&& last != TokenType.EQMATCH
				&& last != TokenType.EQNOTMATCH
				&& last != TokenType.EQUALS
				&& last != TokenType.FOR
				&& last != TokenType.FPIPE
				&& last != TokenType.FUNCTORBIND
				&& last != TokenType.GT
				&& last != TokenType.GTEQ
				&& last != TokenType.IF
				&& last != TokenType.IN
				&& last != TokenType.IS
				&& last != TokenType.LAMBDA
				&& last != TokenType.LBRACKET
				&& last != TokenType.LET
				&& last != TokenType.LT
				&& last != TokenType.LTEQ
				&& last != TokenType.MATCH
				&& last != TokenType.MINUS
				&& last != TokenType.MOD
				&& last != TokenType.COLON
				&& last != TokenType.ELSE
				&& last != TokenType.PLUS
				&& last != TokenType.EOC;
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_' && current != '\'') {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

			switch (word) {
				case "__LINE__":
					this.AddToken(TokenType.NUMBER, this.line.ToString());
					break;
				case "__FILE__":
					this.AddToken(TokenType.TEXT, this.file);
					break;
				case "for":
					this.AddToken(TokenType.FOR);
					break;
				case "import":
					this.AddToken(TokenType.IMPORT);
					break;
				case "yield":
					this.AddToken(TokenType.YIELD);
					break;
				case "from":
					this.AddToken(TokenType.FROM);
					break;
				case "use":
					this.AddToken(TokenType.USE);
					break;
				case "raise":
					this.AddToken(TokenType.RAISE);
					break;
				case "is":
					this.AddToken(TokenType.IS);
					break;
				case "type":
					this.AddToken(TokenType.TYPE);
					break;
				case "implements":
					this.AddToken(TokenType.IMPLEMENTS);
					break;
				case "as":
					this.AddToken(TokenType.AS);
					break;
				case "match":
					this.AddToken(TokenType.MATCH);
					break;
				case "tailrec":
					this.AddToken(TokenType.TAIL_REC);
					break;
				case "in":
					this.AddToken(TokenType.IN);
					break;
				case "if":
					this.AddToken(TokenType.IF);
					break;
				case "else":
					this.AddToken(TokenType.ELSE);
					break;
				case "while":
					this.AddToken(TokenType.WHILE);
					break;
				case "let":
					this.AddToken(TokenType.LET);
					break;
				case "return":
					this.AddToken(TokenType.RETURN);
					break;
				case "module":
					this.AddToken(TokenType.MODULE);
					break;
				case "or":
					this.AddToken(TokenType.OR);
					break;
				case "xor":
					this.AddToken(TokenType.XOR);
					break;
				case "and":
					this.AddToken(TokenType.AND);
					break;
				case "not":
					this.AddToken(TokenType.NOT);
					break;
				case "break":
					this.AddToken(TokenType.BREAK);
					break;
				case "next":
					this.AddToken(TokenType.NEXT);
					break;
				default:
					this.AddToken(TokenType.WORD, word);
					break;
			}
		}

		private void Operator() {
			Char current = this.Peek(0);

			if (current == '/') {
				current = this.Peek(1);

				if (current == '/') {
					this.MultilineComment();
					return;
				}

				current = this.Peek(0);
			}

			StringBuilder buffer = new StringBuilder();

			while (true) {
				buffer.Append(current);
				current = this.Next();
				if (!operatorsDictionary.ContainsKey(buffer.ToString() + current)) {
					Token t = operatorsDictionary[buffer.ToString()];

					if (t.Type != TokenType.LPAREN && this.tokens.Count > 0 && 
						this.tokens[this.tokens.Count - 1].Type == TokenType.EOC) {
						this.tokens.RemoveAt(this.tokens.Count - 1);
					}

					if (this.tokens.Count > 0 &&
						this.tokens[this.tokens.Count - 1].Type == TokenType.DO) {
						this.tokens.RemoveAt(this.tokens.Count - 1);
						this.level -= 4;
					}

					this.AddToken(new Token(t.Type, t.Text));
					return;
				}
			}
		}

		private void SuperText() {
			Int32 line = this.line;

			StringBuilder builder = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (current == ']' && this.Peek(1) == '>') {
					this.Next();
					this.Next();
					break;
				}

				this.CheckOutOfRange(line);

				builder.Append(current);

				current = this.Next();
			}

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private void MultilineComment() {
			Char current = this.Peek(0);

			StringBuilder sb = new StringBuilder();

			while (true) {
				if ("\r\n\0".IndexOf(current) != -1) {
					break;
				}
				current = this.Next();
				sb.Append(current);
			}
			this.Next();
		}

		private void Number() {
			StringBuilder buffer = new StringBuilder();
			Char current = this.Peek(0);

			while (true) {
				if (current == '.') {
					if (!Char.IsDigit(this.Peek(1)) && this.Peek(1) != '.') {
						this.AddToken(TokenType.NUMBER, buffer.ToString());
						this.AddToken(TokenType.DOT);
						this.Next();
						return;
					}

					if (this.Peek(1) == '.') {
						if (this.Peek(2) == '.') {
							this.AddToken(TokenType.NUMBER, buffer.ToString());
							this.AddToken(TokenType.DOTDOTDOT);
							this.Next();
							this.Next();
							this.Next();
							return;
						}
						else {
							this.AddToken(TokenType.NUMBER, buffer.ToString());
							this.AddToken(TokenType.DOTDOT);
							this.Next();
							this.Next();
							return;
						}
					}

					if (buffer.ToString().IndexOf('.') != -1) {
						throw new LumenException(Exceptions.INCORRECT_NUMBER_LITERAL, line: this.line, fileName: this.file);
					}
				}
				else if (current == '_') {
					current = this.Next();
					continue;
				}
				else if (!Char.IsDigit(current)) {
					if (current == 'e' || current == 'E') {
						buffer.Append(current);
						current = this.Next();
						if (current == '-') {
							buffer.Append(current);
							current = this.Next();
						}
						else if (current == '+') {
							buffer.Append(current);
							current = this.Next();
						}
						continue;
					}
					break;
				}
				buffer.Append(current);
				current = this.Next();
			}

			String literal = "";
			while (Char.IsLetter(current)) {
				literal += current;
				current = Next();
			}

			if (literal.Length > 0) {
				if (Double.TryParse(buffer.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result1)) {
					this.AddToken(TokenType.LPAREN);
					this.AddToken(TokenType.WORD, literal);
					this.AddToken(TokenType.NUMBER, result1.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
					this.AddToken(TokenType.RPAREN);
				}
				else {
					throw new LumenException(Exceptions.INCORRECT_NUMBER_LITERAL, line: this.line, fileName: this.file);
				}
				return;
			}

			if (Double.TryParse(buffer.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
				this.AddToken(TokenType.NUMBER, result.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
			}
			else {
				throw new LumenException(Exceptions.INCORRECT_NUMBER_LITERAL, line: this.line, fileName: this.file);
			}
		}

		private Double NumberAnalyze() {
			StringBuilder buffer = new StringBuilder();
			Char current = this.Peek(0);

			if (this.Peek(2) == 'x') {
				String numberBase = this.Peek(0) + "" + this.Peek(1);

				this.Next();
				this.Next();

				current = this.Next();

				while (Char.IsLetterOrDigit(current)) {
					buffer.Append(current);
					current = this.Next();
					while (current == '_') {
						current = this.Next();
					}
				}

				return Convert.ToDouble(Converter.FromN(buffer.ToString(), numberBase));
			}

			while (true) {
				if (current == '.') {
					if (buffer.ToString().IndexOf('.') != -1) {
						throw new LumenException("incorrect Number literal", line: this.line, fileName: this.file);
					}
				}
				else if (current == '_') {
					current = this.Next();
					continue;
				}
				else if (!Char.IsDigit(current)) {
					if (current == 'e') {

						buffer.Append(current);
						current = this.Next();
						if (current == '-') {
							buffer.Append(current);
							current = this.Next();
						}
						else if (current == '+') {
							buffer.Append(current);
							current = this.Next();
						}
						continue;
					}
					break;
				}
				buffer.Append(current);
				current = this.Next();
			}


			return Double.Parse(buffer.ToString().Replace(".", ","), System.Globalization.NumberStyles.Any);
		}

		private Char Next() {
			this.position++;
			return this.Peek(0);
		}

		private Char Peek(Int32 relativePosition) {
			Int32 position = this.position + relativePosition;

			if (position >= this.length) {
				return '\0';
			}

			return this.source[position];
		}

		private void AddToken(Token token) {
			token.Line = this.line;
			this.tokens.Add(token);
		}

		private void AddToken(TokenType type) {
			this.AddToken(type, "");
		}

		private void AddToken(TokenType type, String text) {
			this.tokens.Add(new Token(type, text, this.line));
		}
	}
}
