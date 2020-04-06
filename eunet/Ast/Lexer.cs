using System;
using System.Collections.Generic;
using System.Text;

namespace Argent.Xenon.Ast {
	class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!$&|;:?,[]^.~$";
		private static readonly IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["{"] = new Token(TokenType.LBRACE),
			["}"] = new Token(TokenType.RBRACE),

			["("] = new Token(TokenType.LPAREN),
			[")"] = new Token(TokenType.RPAREN),

			["+"] = new Token(TokenType.PLUS),
			["-"] = new Token(TokenType.MINUS),
			["/"] = new Token(TokenType.SLASH),
			["*"] = new Token(TokenType.STAR),

			["="] = new Token(TokenType.EQ),
			["=="] = new Token(TokenType.EQUALS),
			["!="] = new Token(TokenType.NOTEQ),
			[">"] = new Token(TokenType.GREATER),
			["<"] = new Token(TokenType.LESS),
			[">="] = new Token(TokenType.GREATEREQ),
			["<="] = new Token(TokenType.LESSEQ),

			["?"] = new Token(TokenType.QUESTION),
			["??"] = new Token(TokenType.DQ),

			[","] = new Token(TokenType.SPLIT),
			["."] = new Token(TokenType.DOT),
			[":"] = new Token(TokenType.COLON),
			[";"] = new Token(TokenType.EOC),
		};
		private readonly String source;
		private readonly Int32 length;
		private readonly List<Token> tokens;
		private Int32 position;
		private Int32 line;
		private readonly String file;

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
				else if (Char.IsLetter(current) || current == '_') {
					this.Word();
				}
				else if (operatorsString.IndexOf(current) != -1) {
					this.Operator();
				}
				else if (current == '\n') {
					this.NewLine();
				}
				else {
					this.Next();
				}
			}

			return this.tokens;
		}

		private void Text() {
			this.Next();

			Int32 line = this.line;

			StringBuilder builder = new StringBuilder();

			Char current = this.Peek(0);
			while (current != '"') {
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
						default:
							builder.Append('\\');
							continue;
					}
				}

				this.CheckOutOfRange(line);

				builder.Append(current);

				current = this.Next();
			}

			this.Next();

			this.AddToken(TokenType.TEXT, builder.ToString());
		}

		private void CheckOutOfRange(Int32 line = -1) {
			if (this.position >= this.length) {
				throw new XenonException("consumed symbol '\"'", line: line, fileName: this.file);
			}
		}

		private void NewLine() {
			this.Next();
			this.line++;

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
			return /*last != TokenType.DO
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
				&& last != TokenType.EQNOTMATCH*/
			 last != TokenType.XOR
				&& last != TokenType.STAR
				&& last != TokenType.SLASH
				&& last != TokenType.OR
				&& last != TokenType.NOTEQ
				&& last != TokenType.NOT
				&& last != TokenType.LPAREN
				&& last != TokenType.LESSEQ
				&& last != TokenType.LESS
				&& last != TokenType.LBRACE
				&& last != TokenType.GREATEREQ
				&& last != TokenType.GREATER
				&& last != TokenType.EQ
				&& last != TokenType.DOTDOTDOT
				&& last != TokenType.DOTDOT
				&& last != TokenType.DOT
				&& last != TokenType.AND
				&& last != TokenType.SPLIT
				&& last != TokenType.MINUS
				&& last != TokenType.PLUS
				&& last != TokenType.EOC;
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = this.Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_') {
					break;
				}

				buffer.Append(current);

				current = this.Next();
			}

			String word = buffer.ToString();

			switch (word) {
				case "and":
					this.AddToken(TokenType.AND);
					break;
				case "or":
					this.AddToken(TokenType.OR);
					break;
				case "yield":
					this.AddToken(TokenType.YIELD);
					break;
				case "xor":
					this.AddToken(TokenType.XOR);
					break;
				case "not":
					this.AddToken(TokenType.NOT);
					break;
				case "as":
					this.AddToken(TokenType.AS);
					break;
				case "for":
					this.AddToken(TokenType.FOR);
					break;
				case "exit":
					this.AddToken(TokenType.EXIT);
					break;
				case "in":
					this.AddToken(TokenType.IN);
					break;
				case "do":
					this.AddToken(TokenType.DO);
					break;
				case "function":
					this.AddToken(TokenType.FUNCTION);
					break;
				case "end":
					this.AddToken(TokenType.END);
					break;
				case "return":
					this.AddToken(TokenType.RETURN);
					break;
				case "var":
					this.AddToken(TokenType.VAR);
					break;
				case "mut":
					this.AddToken(TokenType.IMMUT);
					break;
				case "if":
					this.AddToken(TokenType.IF);
					break;
				case "then":
					this.AddToken(TokenType.THEN);
					break;
				case "elif":
					this.AddToken(TokenType.ELIF);
					break;
				case "else":
					this.AddToken(TokenType.ELSE);
					break;
				case "type":
					this.AddToken(TokenType.TYPE);
					break;
				case "auto":
					this.AddToken(TokenType.AUTO);
					break;
				default:
					this.AddToken(TokenType.IDENTIFIER, word);
					break;
			}
		}

		private void Operator() {
			Char current = this.Peek(0);

			if (current == '-') {
				current = this.Peek(1);

				if (current == '-') {
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

					if (this.tokens[this.tokens.Count - 1].Type == TokenType.EOC) {
						this.tokens.RemoveAt(this.tokens.Count - 1);
					}

					this.AddToken(new Token(t.Type, t.Text));
					return;
				}
			}
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
						throw new XenonException("Exceptions.INCORRECT_NUMBER_LITERAL", line: this.line, fileName: this.file);
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

			if (Double.TryParse(buffer.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
				this.AddToken(TokenType.NUMBER, result.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
			}
			else {
				throw new XenonException("Exceptions.INCORRECT_NUMBER_LITERAL", line: this.line, fileName: this.file);
			}
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
