using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lumen.Lang;

namespace Lumen.Lmi {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!$&|;:?,[]^.~$@#";
		private static readonly IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["()"] = new Token(TokenType.VOID, "()"),

			["@"] = new Token(TokenType.ANNOTATION, "@"),
			["#"] = new Token(TokenType.ANNOTATION, "#"),

			["?"] = new Token(TokenType.QUESTION, "?"),
			["~"] = new Token(TokenType.TILDE, Constants.BNOT),

			["+"] = new Token(TokenType.PLUS, Constants.PLUS),
			["-"] = new Token(TokenType.MINUS, Constants.MINUS),
			["*"] = new Token(TokenType.STAR, Constants.STAR),
			["/"] = new Token(TokenType.SLASH, Constants.SLASH),
			["!"] = new Token(TokenType.BANG, "!"),

			["%"] = new Token(TokenType.MODULUS, Constants.MOD),
			["^"] = new Token(TokenType.POWER, Constants.POW),
			["&"] = new Token(TokenType.AMP, Constants.BAND),
			["|"] = new Token(TokenType.BAR, Constants.BOR),

			["<<"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "<<"),
			[">>"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, ">>"),

			["<<="] = new Token(TokenType.SHIFT_LEFT, "<<="),
			[">>-"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, ">>-"),
			["-<"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "-<"),
			["-<<"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "-<<"),
			["<*"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "<*"),
			["<*>"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "<*>"),
			["<$>"] = new Token(TokenType.MIDDLE_PRIORITY_RIGTH, "<$>"),

			["("] = new Token(TokenType.PAREN_OPEN),
			[")"] = new Token(TokenType.PAREN_CLOSE),

			["="] = new Token(TokenType.EQUALS, Constants.EQUALS),
			[".<"] = new Token(TokenType.DOT_LESS, ".<"),
			["<"] = new Token(TokenType.LESS, Constants.LT),
			[">"] = new Token(TokenType.GREATER, Constants.GT),
			["<>"] = new Token(TokenType.NOT_EQUALS, Constants.NOT_EQUALS),
			["<="] = new Token(TokenType.LESS_EQUALS, Constants.LESS_EQUALS),
			[">="] = new Token(TokenType.GREATER_EQUALS, Constants.GREATER_EQUALS),
			["<=>"] = new Token(TokenType.SHIP, Constants.SHIP),

			["=~"] = new Token(TokenType.MATCH_EQUALS, Constants.MATCH_EQUALS),
			["!~"] = new Token(TokenType.NOT_MATCH_EQUALS, Constants.NOT_MATCH_EQUALS),

			["->"] = new Token(TokenType.LAMBDA, "->"),
			["<-"] = new Token(TokenType.ASSIGN, "<-"),

			["::"] = new Token(TokenType.COLON2, "::"),

			[":"] = new Token(TokenType.COLON, ":"),
			[";"] = new Token(TokenType.EOC, ";"),
			[","] = new Token(TokenType.SPLIT, ","),
			["."] = new Token(TokenType.DOT, "."),

			[".."] = new Token(TokenType.DOT2, Constants.RANGE_EXCLUSIVE),
			["..."] = new Token(TokenType.DOT3, Constants.RANGE_INCLUSIVE),

			["["] = new Token(TokenType.LIST_OPEN, "["),
			["]"] = new Token(TokenType.COLLECTION_CLOSE, "]"),

			["@["] = new Token(TokenType.ARRAY_OPEN, "@["),
			["#["] = new Token(TokenType.SEQ_OPEN, "#["),

			["|>"] = new Token(TokenType.FORWARD_PIPE, "|>"),
			["<|"] = new Token(TokenType.BACKWARD_PIPE, "<|")
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
				else {
					this.Next();
				}
			}

			if (this.level != 0) {
				for (Int32 i = 0; i < this.level; i += 4) {
					this.AddToken(TokenType.BLOCK_END);
				}
			}

			return this.tokens;
		}

		private static Boolean IsValidChar(Char ch, Int32 numberBase) {
			return "0123456789ABCDEF".Substring(0, numberBase).IndexOf(Char.ToUpper(ch)) != -1;
		}

		private String NumberBase(Int32 numberBase, Int32 maxLength = -1) {
			Char currentChar = this.Peek();
			String number = "";

			while (IsValidChar(currentChar, numberBase)) {
				number += currentChar;
				currentChar = this.Next();

				if (number.Length == maxLength) {
					break;
				}
			}

			return number;
		}

		// Reads hexadecimal unicode code with length from current position 
		// and returns appropriate string
		private String UnicodeCode(Int32 length, Int32 numberBase) {
			this.Next();
			String code = this.NumberBase(numberBase, length);

			if (code.Length < length) {
				throw new LumenException($"character '{this.Peek()}' is invalid for char code", this.line, this.file);
			}

			try {
				return Char.ConvertFromUtf32(Convert.ToInt32(code, numberBase));
			}
			catch {
				throw new LumenException($"value '{code}' is invalid code of character", this.line, this.file);
			}
		}

		// Reads hexadecimal unicode code with length from current position 
		// and returns appropriate string
		private String UnicodeCode(Char until, Int32 numberBase) {
			Char currentChar = this.Next();
			List<String> charCodes = new List<String>();

			while (Char.IsWhiteSpace(currentChar)) {
				currentChar = this.Next();
			}

			while (true) {
				charCodes.Add(this.NumberBase(numberBase));
				currentChar = this.Peek();

				if (currentChar == until) {
					break;
				}

				if (!Char.IsWhiteSpace(currentChar)) {
					throw new LumenException($"character '{currentChar}' is invalid for code", this.line, this.file);
				}

				while (Char.IsWhiteSpace(currentChar)) {
					currentChar = this.Next();
				}

				if (currentChar == until) {
					break;
				}
			}

			this.Next();

			return String.Concat(charCodes.Select(charCode => {
				try {
					return Char.ConvertFromUtf32(Convert.ToInt32(charCode, numberBase));
				}
				catch {
					throw new LumenException($"value '{charCode}' is invalid code of character", this.line, this.file);
				}
			}));
		}

		private void Text(Boolean isRaw = false) {
			this.Next();

			Int32 line = this.line;

			StringBuilder builder = new StringBuilder();

			Int32 x = 0;

			Char current = this.Peek(0);

			List<String> substitutes = new List<String>();

			while (true) {
				if (!isRaw && current == '\\') {
					current = this.Next();

					switch (current) {
						case '"':
							current = this.Next();
							builder.Append('"');
							continue;
						case '\r':
							current = this.Next();
							if (current == '\n') {
								current = this.Next();
							}
							continue;
						case '\n':
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
						case 'd':
							if (this.Peek(1) == '(') {
								this.Next();
								builder.Append(this.UnicodeCode(')', 10));
							}
							else {
								builder.Append(this.UnicodeCode(4, 10));
							}
							current = this.Peek();
							continue;
						case 'o':
							if (this.Peek(1) == '(') {
								this.Next();
								builder.Append(this.UnicodeCode(')', 8));
							}
							else {
								builder.Append(this.UnicodeCode(4, 8));
							}
							current = this.Peek();
							continue;
						case 'x':
							if (this.Peek(1) == '(') {
								this.Next();
								builder.Append(this.UnicodeCode(')', 16));
							}
							else {
								builder.Append(this.UnicodeCode(4, 16));
							}
							current = this.Peek();
							continue;
						case '(':
							if (current == '"') {
								builder.Append('(');
								break;
							}

							StringBuilder modifiers = new StringBuilder();
							StringBuilder buffer = new StringBuilder();

							Int32 level = 1;
							current = this.Next();
							while (true) {
								this.CheckOutOfRange();

								if (current == '(') {
									level++;
								}

								if (current == ')') {
									level--;
								}

								if (level == 0) {
									break;
								}

								if (current == ':' && level == 1) {
									current = this.Next();
									while (current != ')') {
										modifiers.Append(current);
										current = this.Next();
									}

									level--;
									if (level == 0) {
										break;
									}
								}

								buffer.Append(current);
								current = this.Next();
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
							throw new LumenException($"unknown escape character '\\{current}'", this.line, this.file);
					}
				}

				if (!isRaw && current == '{') {
					current = this.Next();
					builder.Append("{{");
				}

				else if (!isRaw && current == '}') {
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

			if(isRaw) {
				this.AddToken(TokenType.TEXT, builder.ToString());
				return;
			}

			if (substitutes.Count == 0) {
				this.AddToken(TokenType.TEXT, builder.ToString().F());
			}
			else {
				this.AddToken(TokenType.PAREN_OPEN);
				this.AddToken(TokenType.TEXT, builder.ToString());

				this.AddToken(TokenType.MODULUS, Constants.MOD);
				this.AddToken(TokenType.LIST_OPEN);

				List<Token> zzz = new Lexer(String.Join(", ", substitutes), this.file).Tokenization();
				foreach (Token i in zzz) {
					this.AddToken(i);
				}
				this.AddToken(TokenType.COLLECTION_CLOSE);
				this.AddToken(TokenType.PAREN_CLOSE);
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

			if (current == '\n' || current == '\r') {
				return;
			}

			if (newLevel < this.level) {
				for (Int32 i = 0; i < this.level - newLevel; i += 4) {
					this.AddToken(TokenType.BLOCK_END);
				}
			}
			else if (newLevel > this.level) {
				for (Int32 i = 0; i < newLevel - this.level; i += 4) {
					this.AddToken(TokenType.BLOCK_START);
				}
			}

			this.level = newLevel;

			if (this.tokens.Count > 0 && this.CanInsert()) {
				this.AddToken(TokenType.EOC);
			}
		}

		private Boolean CanInsert() {
			TokenType last = this.tokens[this.tokens.Count - 1].Type;
			return last != TokenType.BLOCK_START
				&& last != TokenType.FUN
				&& last != TokenType.AND
				&& last != TokenType.BAR
				&& last != TokenType.ARRAY_OPEN
				&& last != TokenType.AS
				&& last != TokenType.ASSIGN
				&& last != TokenType.BACKWARD_PIPE
				&& last != TokenType.POWER
				&& last != TokenType.DOT
				&& last != TokenType.DOT2
				&& last != TokenType.DOT3
				&& last != TokenType.ARRAY_OPEN
				&& last != TokenType.MATCH_EQUALS
				&& last != TokenType.NOT_MATCH_EQUALS
				&& last != TokenType.EQUALS
				&& last != TokenType.FOR
				&& last != TokenType.FORWARD_PIPE
				&& last != TokenType.GREATER_EQUALS
				&& last != TokenType.IF
				&& last != TokenType.IN
				&& last != TokenType.LAMBDA
				&& last != TokenType.LIST_OPEN
				&& last != TokenType.LET
				&& last != TokenType.LESS
				&& last != TokenType.LESS_EQUALS
				&& last != TokenType.MATCH
				&& last != TokenType.MINUS
				&& last != TokenType.MODULUS
				&& last != TokenType.COLON
				&& last != TokenType.SPLIT
				&& last != TokenType.ELSE
				&& last != TokenType.PLUS
				&& last != TokenType.EOC;
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			if(current == 'r' && this.Peek(1) == '"') {
				this.Next();
				this.Text(true);
				return;
			}

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
				case "LINE":
					this.AddToken(TokenType.NUMBER, this.line.ToString());
					break;
				case "FILE":
					this.AddToken(TokenType.TEXT, this.file);
					break;

				case "for":
					this.AddToken(TokenType.FOR);
					break;
				case "fun":
					this.AddToken(TokenType.FUN);
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
				case "try":
					this.AddToken(TokenType.TRY);
					break;
				case "except":
					this.AddToken(TokenType.EXCEPT);
					break;
				case "ensure":
					this.AddToken(TokenType.ENSURE);
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
				case "when":
					this.AddToken(TokenType.WHEN);
					break;
				case "redo":
					this.AddToken(TokenType.REDO);
					break;
				case "retry":
					this.AddToken(TokenType.RETRY);
					break;
				case "assert":
					this.AddToken(TokenType.ASSERT);
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
					this.Comment();
					return;
				}

				if (current == '*') {
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
					Token currentOperator = operatorsDictionary[buffer.ToString()];

					if (this.tokens.Count > 0) {
						TokenType last = this.tokens[this.tokens.Count - 1].Type;

						if (currentOperator.Type != TokenType.PAREN_OPEN
							&& last == TokenType.EOC) {
							this.tokens.RemoveAt(this.tokens.Count - 1);
						}

						if (last == TokenType.BLOCK_START
							&& !this.IsStartlineToken(currentOperator.Type)) {
							this.tokens.RemoveAt(this.tokens.Count - 1);
							this.level -= 4;
						}
					}

					this.AddToken(currentOperator);
					return;
				}
			}
		}

		private Boolean IsStartlineToken(TokenType tokenType) {
			return tokenType == TokenType.BAR || tokenType == TokenType.ANNOTATION;
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

		private void Comment() {
			Char current = this.Peek(0);

			while (true) {
				if ("\r\n\0".IndexOf(current) != -1) {
					break;
				}
				current = this.Next();
			}

			this.Next();
		}

		private void MultilineComment() {
			Char current = this.Peek(0);

			while (true) {
				if (current == '*' && this.Peek(1) == '/') {
					this.Next();
					break;
				}
				current = this.Next();
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
							this.AddToken(TokenType.DOT3);
							this.Next();
							this.Next();
							this.Next();
							return;
						}
						else {
							this.AddToken(TokenType.NUMBER, buffer.ToString());
							this.AddToken(TokenType.DOT2);
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

						if (current == '-' || current == '+') {
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
				current = this.Next();
			}

			if (Double.TryParse(buffer.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
				this.AddToken(TokenType.NUMBER, result.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
			}
			else {
				throw new LumenException(Exceptions.INCORRECT_NUMBER_LITERAL, line: this.line, fileName: this.file);
			}
		}

		private Char Next(Int32 times = 1) {
			this.position += times;
			return this.Peek();
		}

		private Char Peek(Int32 relativePosition = 0) {
			Int32 position = this.position + relativePosition;

			if (position >= this.length) {
				return '\0';
			}

			return this.source[position];
		}

		private void AddToken(Token token) {
			this.AddToken(token.Type, token.Text);
		}

		private void AddToken(TokenType type, String text = "") {
			this.tokens.Add(new Token(type, text, this.line));
		}
	}
}
