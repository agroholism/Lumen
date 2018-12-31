using System;
using System.Collections.Generic;
using System.Text;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal sealed class Lexer {
		private const String operatorsString = "+-\\*/%(){}=<>!&|;:?,[]^.@~#";
		private const String validSymbols = "0123456789abcdef";
		private static IDictionary<String, Token> operatorsDictionary = new Dictionary<String, Token>() {
			["+"] = new Token(TokenType.PLUS, Op.PLUS),
			["-"] = new Token(TokenType.MINUS, Op.MINUS),
			["*"] = new Token(TokenType.STAR, Op.STAR),
			["/"] = new Token(TokenType.SLASH, Op.SLASH),
			["%"] = new Token(TokenType.MOD, Op.MOD),
			["//"] = new Token(TokenType.DIV, Op.DIV),
			["("] = new Token(TokenType.LPAREN),
			[")"] = new Token(TokenType.RPAREN),
			["="] = new Token(TokenType.EQEQ, Op.EQL),
			["<"] = new Token(TokenType.LT, Op.LT),
			[">"] = new Token(TokenType.GT, Op.GT),
			["?"] = new Token(TokenType.QUESTION),
			[":"] = new Token(TokenType.COLON),
			["!"] = new Token(TokenType.EXCL),
			["~"] = new Token(TokenType.TILDE, Op.BNOT),
			["<<"] = new Token(TokenType.BLEFT, Op.LSH),
			[">>"] = new Token(TokenType.BRIGTH, Op.RSH),
			["^"] = new Token(TokenType.BXOR, Op.BXOR),
			["{"] = new Token(TokenType.DO),
			["}"] = new Token(TokenType.END),
			["&"] = new Token(TokenType.AMP, Op.BAND),
			["|"] = new Token(TokenType.BAR, Op.BOR),
			["=>"] = new Token(TokenType.LAMBDA),
			["**"] = new Token(TokenType.POW, Op.POW),
			["++"] = new Token(TokenType.INC, "++"),
			["--"] = new Token(TokenType.DEC, "--"),
			["==="] = new Token(TokenType.EQEQEQ, "==="),
			["!=="] = new Token(TokenType.EXCLEQEQ, "!=="),
			["<=>"] = new Token(TokenType.LTEQGT, Op.SHIP),
			[":="] = new Token(TokenType.EQ, ":="),
			["+="] = new Token(TokenType.OPEQ, Op.APLUS),
			["-="] = new Token(TokenType.OPEQ, Op.AMINUS),
			["/="] = new Token(TokenType.OPEQ, Op.ASLASH),
			["*="] = new Token(TokenType.OPEQ, Op.ASTAR),
			["**="] = new Token(TokenType.OPEQ, Op.APOW),
			["%="] = new Token(TokenType.OPEQ, Op.AMOD),
			["/="] = new Token(TokenType.EXCLEQ, Op.NOT_EQL),
			["<="] = new Token(TokenType.LTEQ, Op.LTEQ),
			[">="] = new Token(TokenType.GTEQ, Op.GTEQ),
			["=~"] = new Token(TokenType.EQMATCH, Op.MATCH),
			["!~"] = new Token(TokenType.EQNOTMATCH, Op.NOT_MATCH),
			[";"] = new Token(TokenType.EOC, ";"),
			[","] = new Token(TokenType.SPLIT, ","),
			["."] = new Token(TokenType.DOT, "."),
			[".."] = new Token(TokenType.DOTDOT, Op.DOTE),
			["..."] = new Token(TokenType.DOTDOTDOT, Op.DOTI),
			["["] = new Token(TokenType.LBRACKET, "["),
			["]"] = new Token(TokenType.RBRACKET, "]"),
			["|>"] = new Token(TokenType.FPIPE, "|>"),
			["//="] = new Token(TokenType.OPEQ, "//="),
			["~="] = new Token(TokenType.OPEQ, "~="),
			["<<="] = new Token(TokenType.OPEQ, "<<="),
			["^="] = new Token(TokenType.OPEQ, "^="),
			["|="] = new Token(TokenType.OPEQ, "|="),
			["&="] = new Token(TokenType.OPEQ, "&="),
			[">>="] = new Token(TokenType.OPEQ, ">>="),
			[".?"] = new Token(TokenType.OPTIONAl, ".?"),
			["@"] = new Token(TokenType.DECO, "@")
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
				Char current = Peek(0);

				if (Char.IsDigit(current)) {
					Number();
				}
				else if (current == '"') {
					String();
				}
				else if (Char.IsLetter(current) || current == '_' || current == '$') {
					Word();
				}
				else if (operatorsString.IndexOf(current) != -1) {
					Operator();
				}
				else if (current == '\n') {
					Tabs();
				}
				else {
					Next();
				}
			}

			return this.tokens;
		}

		private void String() {
			Next();

			StringBuilder builder = new StringBuilder();

			Int32 x = 0;

			Char current = Peek(0);

			List<String> substitutes = new List<String>();

			while (true) {
				if (current == '\\') {
					current = Next();
					switch (current) {
						case '#':
							current = Next();
							builder.Append('#');
							continue;
						case '"':
							current = Next();
							builder.Append('"');
							continue;
						case '\r':
							Next();
							current = Next();
							continue;
						case 'f':
							current = Next();
							builder.Append('\f');
							continue;
						case '\\':
							current = Next();
							builder.Append('\\');
							continue;
						case '0':
							current = Next();
							builder.Append('\0');
							continue;
						case 'a':
							current = Next();
							builder.Append('\a');
							continue;
						case 'b':
							current = Next();
							builder.Append('\b');
							continue;
						case 'r':
							current = Next();
							builder.Append('\r');
							continue;
						case 'v':
							current = Next();
							builder.Append('\v');
							continue;
						case 'e':
							current = Next();
							builder.Append(Environment.NewLine);
							continue;
						case 'n':
							current = Next();
							builder.Append('\n');
							continue;
						case 't':
							current = Next();
							builder.Append('\t');
							continue;
						case 'u':
							if (Peek(1) != '{') {
								Next();
								builder.Append(Char.ConvertFromUtf32(Convert.ToInt32(NumberAnalyze())));
							}
							else {
								String i = "";
								Next();
								current = Next();

								while (true) {
									if (current == '"') {
										throw new Lumen.Lang.Std.Exception("consumed } with \\u ", stack: null) {
											line = this.line,
											file = this.file
										};
									}

									if (current == '}') {
										current = Next();
										break;
									}
									i += current;
									current = Next();
								}

								foreach (String j in i.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) {
									builder.Append(Char.ConvertFromUtf32(Convert.ToInt32(j, 16)));
								}
							}
							continue;
						default:
							builder.Append('\\');
							continue;
					}
				}

				if (current == '{') {
					current = Next();
					builder.Append("{{");
				}

				if (current == '}') {
					current = Next();
					builder.Append("}}");
				}

				if (current == '#') {
					current = Next();

					if (current == '"') {
						builder.Append('#');
						break;
					}

					StringBuilder buffer = new StringBuilder();
					if (current == '{') {
						current = Next();
						while (current != '}') {
							CheckOutOfRange();
							buffer.Append(current);
							current = Next();
							while (current == '}' && !Provider.IsCompleted(buffer.ToString())) {
								buffer.Append('}');
								current = Next();
							}
						}
						current = Next();
					}
					else {
						while (true) {
							CheckOutOfRange();

							while (current == '(') {
								buffer.Append(current);
								current = Next();
								while (!Provider.IsCompleted(buffer.ToString())) {
									CheckOutOfRange();
									buffer.Append(current);
									current = Next();
								}
							}

							if (!Char.IsLetterOrDigit(current)
								&& current != '_'
								&& current != '$'
								&& current != '?'
								&& current != '!'
								&& current != '.') {
								break;
							}

							buffer.Append(current);
							current = Next();
						}
					}
					String s = buffer.ToString();
					Int32 position = substitutes.IndexOf(s);

					if (position == -1) {
						substitutes.Add(s);
						//if(builder.ToString().EndsWith("}"))
						//	builder.Append("\b ");
						builder.Append($"{{{x}}}");
						x++;
					}
					else {
						builder.Append($"{{{position}}}");
					}
					continue;
				}

				if (current == '"') {
					break;
				}

				CheckOutOfRange();

				builder.Append(current);

				current = Next();
			}

			current = Next();

			AddToken(TokenType.TEXT, builder.ToString());
			/*AddToken(TokenType.MOD);
			AddToken(TokenType.NEW);
			AddToken(TokenType.WORD, "List");
			AddToken(TokenType.DO);
			foreach (Token i in new Lexer(System.String.Join(",", substitutes), this.file).Tokenization()) {
				this.AddToken(i);
			}
			AddToken(TokenType.END);*/
		}

		private void CheckOutOfRange() {
			if (this.position >= this.length) {
				throw new Lumen.Lang.Std.Exception("consumed symbol '\"'") {
					line = this.line,
					file = this.file
				};
			}
		}

		private void Tabs() {
			this.line++;
			if (this.tokens.Count > 0) {
				Char current = Next();

				while (Char.IsWhiteSpace(current)) {
					if (current == '\n') {
						this.line++;
					}

					current = Next();
				}

				TokenType typeOfLast = this.tokens[this.tokens.Count - 1].Type;

				if (typeOfLast == TokenType.RAISE || typeOfLast == TokenType.BREAK || typeOfLast == TokenType.RETURN) {
					AddToken(TokenType.NUMBER, "1");
				}
				else if ((current == '[' || current == '(' || current == '.') && (typeOfLast == TokenType.BNUMBER ||
				  typeOfLast == TokenType.INTERTEXT
				  || typeOfLast == TokenType.IWORDS
				  || typeOfLast == TokenType.NUMBER
				  || typeOfLast == TokenType.RBRACKET
				  || typeOfLast == TokenType.RPAREN
				  || typeOfLast == TokenType.SIMPLETEXT
				  || typeOfLast == TokenType.TEXT
				  || typeOfLast == TokenType.WORD
				  || typeOfLast == TokenType.END)) {
					AddToken(TokenType.EOC);
				}
			}
		}

		private void Regex() {
			Next();
			Next();

			StringBuilder buffer = new StringBuilder();

			Char current = Peek(0);

			while (true) {
				if (current == '\\') {
					current = Next();

					switch (current) {
						case '/':
							current = Next();
							buffer.Append('/');
							continue;
						default:
							buffer.Append('\\');
							continue;
					}
				}

				if (current == '/') {
					break;
				}

				buffer.Append(current);

				current = Next();
			}

			Next();

			AddToken(TokenType.REGEXP, buffer.ToString());
		}

		private void Word() {
			StringBuilder buffer = new StringBuilder();

			Char current = Peek(0);

			while (true) {
				if (!Char.IsLetterOrDigit(current)
					&& current != '_'
					&& current != '$'
					&& current != '?'
					&& current != '!') {
					break;
				}

				buffer.Append(current);

				current = Next();
			}

			String word = buffer.ToString();

			switch (word) {
				case "__LINE__":
					AddToken(TokenType.NUMBER, this.line.ToString());
					break;
				case "__FILE__":
					AddToken(TokenType.TEXT, this.file);
					break;
				case "record":
					AddToken(TokenType.TYPE);
					break;
				case "final":
					AddToken(TokenType.FINAL);
					break;
				case "but":
					AddToken(TokenType.BUT);
					break;
				case "new":
					AddToken(TokenType.NEW);
					break;
				case "raise":
					AddToken(TokenType.RAISE);
					break;
				case "module":
					AddToken(TokenType.MODULE);
					break;
				case "enum":
					AddToken(TokenType.ENUM);
					break;
				case "for":
					AddToken(TokenType.FOR);
					break;
				case "once":
					AddToken(TokenType.ONCE);
					break;
				case "is":
					AddToken(TokenType.IS);
					break;
				case "try":
					AddToken(TokenType.TRY);
					break;
				case "except":
					AddToken(TokenType.CATCH);
					break;
				case "finally":
					AddToken(TokenType.FINALLY);
					break;
				case "using":
					AddToken(TokenType.USING);
					break;
				case "in":
					AddToken(TokenType.IN);
					break;
				case "if":
					AddToken(TokenType.IF);
					break;
				case "else":
					AddToken(TokenType.ELSE);
					break;
				case "while":
					AddToken(TokenType.WHILE);
					break;
				case "do":
					AddToken(TokenType.DO);
					break;
				case "end":
					AddToken(TokenType.END);
					break;
				case "break":
					AddToken(TokenType.BREAK);
					break;
				case "next":
					AddToken(TokenType.CONTINUE);
					break;
				case "let":
					AddToken(TokenType.LET);
					break;
				case "return":
					AddToken(TokenType.RETURN);
					break;
				case "or":
					AddToken(TokenType.OR);
					break;
				case "include":
					AddToken(TokenType.INCLUDE);
					break;
				case "xor":
					AddToken(TokenType.XOR);
					break;
				case "and":
					AddToken(TokenType.AND);
					break;
				case "not":
					AddToken(TokenType.EXCL);
					break;
				case "ref":
					AddToken(TokenType.REF);
					break;
				case "auto":
					AddToken(TokenType.AUTO);
					break;
				default:
					AddToken(TokenType.WORD, word);
					break;
			}
		}

		private void HEXNumber() {
			// Буфер для сохранения числа.
			StringBuilder buffer = new StringBuilder().Append(Peek(0)).Append(Peek(1)).Append("|");

			Next();
			Next();

			// Текущий.
			Char current = Next();

			while (Char.IsLetterOrDigit(current)) {
				// Добавляем.
				buffer.Append(current);
				// Берём следующий.
				current = Next();
				while (current == '_') {
					current = Next();
				}
			}

			// Добавляем токен.
			AddToken(TokenType.HARDNUMBER, buffer.ToString());
		}

		private void Operator() {
			// Берём текущий символ.
			Char current = Peek(0);

			// Комменты.
			if (current == '#') {
				Next();
				MultilineComment();
				return;
			}

			// А тут у нас собираются операторы.
			StringBuilder buffer = new StringBuilder(current + "");
			current = Next();

			while (true) {
				if (!operatorsDictionary.ContainsKey(buffer.ToString() + current)) {
					AddToken(operatorsDictionary[buffer.ToString()]);
					return;
				}
				buffer.Append(current);
				current = Next();
			}
		}

		private void MultilineComment() {
			// Берём текущий.
			Char current = Peek(0);

			StringBuilder sb = new StringBuilder();

			while (true) {
				// Итакпанятна.
				if ("\r\n\0".IndexOf(current) != -1) {
					break;
				}
				// Тоже.
				current = Next();
				sb.Append(current);
			}
			AddToken(TokenType.DOC, sb.ToString());
			// Есть вопросы?
			Next();
		}

		private void Number() {
			StringBuilder buffer = new StringBuilder();
			Char current = Peek(0);

			if (Peek(2) == 'x') {
				HEXNumber();
				return;
			}

			Boolean isScientic = false;

			while (true) {
				if (current == '.') {
					if (!Char.IsDigit(Peek(1)) && Peek(1) != '.') {
						AddToken(TokenType.NUMBER, buffer.ToString());
						AddToken(TokenType.DOT);
						Next();
						return;
					}
					if (Peek(1) == '.') {
						if (Peek(2) == '.') {
							AddToken(TokenType.NUMBER, buffer.ToString());
							AddToken(TokenType.DOTDOTDOT);
							Next();
							Next();
							Next();
							return;
						}
						else {
							AddToken(TokenType.NUMBER, buffer.ToString());
							AddToken(TokenType.DOTDOT);
							Next();
							Next();
							return;
						}
					}
					// Не, ну логично же.
					if (buffer.ToString().IndexOf('.') != -1) {
						throw new Lumen.Lang.Std.Exception("лишняя точка < литерал num", stack: null);
					}
				}
				else if (current == '_') {
					current = Next();
					continue;
				}
				else if (!Char.IsDigit(current)) {
					if (current == 'e') {
						isScientic = true;
						buffer.Append(current);
						current = Next();
						if (current == '-') {
							buffer.Append(current);
							current = Next();
						}
						else if (current == '+') {
							buffer.Append(current);
							current = Next();
						}
						continue;
					}
					break;
				}
				buffer.Append(current);
				current = Next();
			}

			if(current == 'b') {
				Next();
				AddToken(TokenType.BIG_NUMBER, buffer.ToString());
				return;
			}


			if (isScientic) {
				AddToken(TokenType.NUMBER, Double.Parse(buffer.Replace(".", ",").ToString(), System.Globalization.NumberStyles.Any).ToString());
			}
			else {
				AddToken(TokenType.NUMBER, buffer.Replace(".", ",").ToString());
			}
		}

		private Double NumberAnalyze() {
			StringBuilder buffer = new StringBuilder();
			Char current = Peek(0);

			if (Peek(2) == 'x') {
				String numberBase = Peek(0) + "" + Peek(1);

				Next();
				Next();

				current = Next();

				while (Char.IsLetterOrDigit(current)) {
					buffer.Append(current);
					current = Next();
					while (current == '_') {
						current = Next();
					}
				}

				return Convert.ToDouble(Converter.FromN(buffer.ToString(), numberBase));
			}

			Boolean isScientic = false;

			while (true) {
				if (current == '.') {
					// Не, ну логично же.
					if (buffer.ToString().IndexOf('.') != -1) {
						throw new Lumen.Lang.Std.Exception("лишняя точка < литерал num", stack: null);
					}
				}
				else if (current == '_') {
					current = Next();
					continue;
				}
				else if (!Char.IsDigit(current)) {
					if (current == 'e') {
						isScientic = true;
						buffer.Append(current);
						current = Next();
						if (current == '-') {
							buffer.Append(current);
							current = Next();
						}
						else if (current == '+') {
							buffer.Append(current);
							current = Next();
						}
						continue;
					}
					break;
				}
				buffer.Append(current);
				current = Next();
			}


			return Double.Parse(buffer.ToString().Replace(".", ","), System.Globalization.NumberStyles.Any);
		}

		private Char Next() {
			this.position++;
			return Peek(0);
		}

		private Char Peek(Int32 relativePosition) {
			// Да  нет-нет.
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
			AddToken(type, "");
		}

		private void AddToken(TokenType type, String text) {
			this.tokens.Add(new Token(type, text, this.line));
		}
	}
}
