#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal sealed class Parser {
		private List<Token> tokens;
		private Int32 size;
		private String fileName;
		private Int32 position;
		private Int32 line;

		internal Parser(List<Token> tokens, String fileName) {
			this.fileName = fileName;
			this.tokens = tokens;
			this.size = tokens.Count;
			this.line = 0;
		}

		internal List<Expression> Parse() {
			List<Expression> result = new List<Expression>();

			while (!this.Match(TokenType.EOF)) {
				result.Add(this.Expression());
				this.Match(TokenType.EOC);
			}

			return result;
		}

		private Expression Expression() {
			while (this.Match(TokenType.EOC)) {

			}

			Token current = this.GetToken(0);

			switch (current.Type) {
				case TokenType.RETURN:
					this.Match(TokenType.RETURN);
					return new Return(this.Expression());
				case TokenType.YIELD:
					this.Match(TokenType.YIELD);
					if (this.GetToken(0).Text == "sequence") {
						this.Consume(TokenType.IDENTIFIER);
						return new YieldSequence(this.Expression());
					}
					return new Yield(this.Expression());
				case TokenType.EXIT:
					Int32 line = this.line;
					this.Match(TokenType.EXIT);
					return this.Match(TokenType.EOC) ? new Exit(new ValueE(Nil.NilIns), line, this.fileName) : new Exit(this.Expression(), line, this.fileName);
				case TokenType.FUNCTION:
					return this.ParseFunctionDeclaration();
				case TokenType.TYPE:
					return this.ParseTypeDeclaration();
				case TokenType.VAR:
					return this.ParseVarDeclaration();
				case TokenType.IF:
					return this.ParseIf();
				case TokenType.FOR:
					return this.ParseFor();
				default:
					return this.LogikOr();
			}
		}

		private Expression ParseVarDeclaration() {
			this.Match(TokenType.VAR);
			String name = this.Consume(TokenType.IDENTIFIER).Text;
			this.Consume(TokenType.COLON);
			Expression type = this.ParseType();
			Int32 line = this.line;
			this.Consume(TokenType.EQ);
			Expression val = this.Expression();
			return new VariableDeclaration(type, name, val, line, this.fileName);
		}

		private Expression ParseFor() {
			BlockE ParseBlock() {
				List<Expression> expressions = new List<Expression>();
				while (!this.Match(TokenType.END)) {
					expressions.Add(this.Expression());
					while (this.Match(TokenType.EOC)) {

					}
				}
				return new BlockE(expressions);
			}

			this.Consume(TokenType.FOR);
			String nameVariable = this.Consume(TokenType.IDENTIFIER).Text;

			Match(TokenType.IN);

			Expression containerExpression = Expression();

			Match(TokenType.DO);

			Expression expression = ParseBlock();

			return new ForExpression(nameVariable, containerExpression, expression);
		}

		private Expression ParseTypeDeclaration() {
			this.Match(TokenType.TYPE);

			String functionName = this.Consume(TokenType.IDENTIFIER).Text;

			if (!this.LookMatch(0, TokenType.LPAREN)) {
				Expression type = new IdExpression("type", this.line, this.fileName);
				Int32 line = this.line;
				this.Consume(TokenType.EQ);
				Expression assignable = this.Expression();

				return new VariableDeclaration(type, functionName, assignable, line, this.fileName);
			}

			throw new NotImplementedException();
		}

		private Expression ParseIf() {
			BlockE ParseBlock() {
				List<Expression> expressions = new List<Expression>();
				while (!this.LookMatch(0, TokenType.ELIF) && !this.LookMatch(0, TokenType.END) && !this.LookMatch(0, TokenType.ELSE)) {
					expressions.Add(this.Expression());
					while (this.Match(TokenType.EOC)) {

					}
				}
				return new BlockE(expressions);
			}

			this.Consume(TokenType.IF);
			Dictionary<Expression, Expression> conditionalBlocks = new Dictionary<Expression, Expression>();

			Expression condition = this.Expression();
			this.Consume(TokenType.THEN);
			conditionalBlocks[condition] = ParseBlock();

			while (this.Match(TokenType.ELIF)) {
				condition = this.Expression();
				this.Consume(TokenType.THEN);
				conditionalBlocks[condition] = ParseBlock();
			}

			Expression falseBody = new ValueE(0);
			if (this.Match(TokenType.ELSE)) {
				falseBody = ParseBlock();
			}

			this.Consume(TokenType.END);

			return new IfExpression(conditionalBlocks, falseBody);
		}

		private Expression? ParseType() {
			Expression? res = null;

			if (this.Match(TokenType.IMMUT)) {
				res = new ImmutExpression(ParseType());
			}

			if (this.LookMatch(0, TokenType.IDENTIFIER)) {
				String name = this.Consume(TokenType.IDENTIFIER).Text;
				res = new IdExpression(name, this.line, this.fileName);
			}

			if (this.Match(TokenType.FUNCTION)) {
				res = new IdExpression("function", this.line, this.fileName);
			}

			if (this.Match(TokenType.TYPE)) {
				res = new IdExpression("type", this.line, this.fileName);
			}

			if (Match(TokenType.QUESTION) && res != null) {
				res = new NullableExpression(res);
			}

			return res;
		}

		private FunctionSpecifier GetSpecifier(String name) {
			return name switch
			{
				"before" => FunctionSpecifier.BEFORE,
				"after" => FunctionSpecifier.AFTER,
				_ => FunctionSpecifier.NONE
			};
		}

		private Expression ParseFunctionDeclaration() {
			this.Match(TokenType.FUNCTION);

			String functionName = this.Consume(TokenType.IDENTIFIER).Text;

			if (!this.LookMatch(0, TokenType.LPAREN)) {
				Expression type = new IdExpression("function", this.line, this.fileName);
				Int32 line = this.line;
				this.Consume(TokenType.EQ);
				Expression assignable = this.Expression();

				return new VariableDeclaration(type, functionName, assignable, line, this.fileName);
			}

			Dictionary<String, Expression> args = new Dictionary<String, Expression>();
			if (this.Match(TokenType.LPAREN)) {
				while (!this.Match(TokenType.RPAREN)) {
					Expression? paramType = this.ParseType();
					String paramName = this.Consume(TokenType.IDENTIFIER).Text;
					this.Match(TokenType.SPLIT);
					args[paramName] = paramType;
				}
			}

			List<Expression> specifiers = new List<Expression>();

			if (this.Match(TokenType.COLON)) {
				specifiers.Add(Expression());
				while (this.Match(TokenType.SPLIT)) {
					specifiers.Add(Expression());
				}
				Match(TokenType.EOC);
			}

			List<Expression> exps = new List<Expression>();
			while (!this.Match(TokenType.END)) {
				exps.Add(this.Expression());
				this.Match(TokenType.EOC);
			}
			return new FunctionDeclaration(functionName, args, new BlockE(exps), specifiers);
		}

		private Expression LogikOr() {
			Expression result = this.LogikXor();

			while (true) {
				if (this.Match(TokenType.OR)) {
					result = new BinaryExpression(result, this.LogikXor(), Operation.OR, this.line, this.fileName);
					continue;
				}
				break;
			}

			if (this.Match(TokenType.DQ)) {
				return new DoubleQuestion(result, Expression());
			}

			return result;
		}

		private Expression LogikXor() {
			Expression result = this.LogikAnd();

			while (true) {
				if (this.Match(TokenType.XOR)) {
					result = new BinaryExpression(result, this.LogikAnd(), Operation.XOR, this.line, this.fileName);
					continue;
				}
				break;
			}
			return result;
		}

		private Expression LogikAnd() {
			Expression result = this.Assigment();

			while (true) {
				if (this.Match(TokenType.AND)) {
					result = new BinaryExpression(result, this.Assigment(), Operation.AND, this.line, this.fileName);
					continue;
				}
				break;
			}
			return result;
		}

		private Expression Assigment() {
			if (this.LookMatch(0, TokenType.IDENTIFIER) && this.LookMatch(1, TokenType.EQ)) {
				String identifier = this.Consume(TokenType.IDENTIFIER).Text;
				this.Match(TokenType.EQ);

				Int32 line = this.line;
				return new Assigment(identifier, this.Expression(), line, this.fileName);
			}

			Expression result = this.Equality();

			return result;
		}

		private Expression Equality() {
			Expression result = this.Conditional();


			if (this.Match(TokenType.EQUALS)) {
				result = this.ChainedEquality(result, this.GetToken(-1), Operation.EQUALS, false);
			}
			else if (this.Match(TokenType.NOTEQ)) {
				result = this.ChainedEquality(result, this.GetToken(-1), Operation.NOT_EQUALS, false);
			}

			return result;
		}

		private Expression ChainedEquality(Expression leftExpression, Token token, String op, Boolean fromConditional) {
			Int32 line = this.GetToken(-1).Line;

			BinaryExpression result = new BinaryExpression(leftExpression, fromConditional ? this.Bitwise() : this.Conditional(), op, line, this.fileName);

			if (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryExpression(result, new BinaryExpression(result.expressionTwo ?? new ValueE(0), fromConditional ? this.Bitwise() : this.Conditional(), op, line, this.fileName), Operation.AND, line, this.fileName);
			}

			while (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryExpression(result, new BinaryExpression((result.expressionTwo as BinaryExpression)!.expressionTwo, fromConditional ? this.Bitwise() : this.Conditional(), op, line, this.fileName), Operation.AND, line, this.fileName);
			}

			return result;
		}

		private Expression Conditional() {
			Expression expr = this.Bitwise();

			if (this.Match(TokenType.LESS)) {
				expr = this.ChainedEquality(expr, this.GetToken(-1), Operation.LESS, false);
			}
			else if (this.Match(TokenType.LESSEQ)) {
				expr = this.ChainedEquality(expr, this.GetToken(-1), Operation.LESSEQ, false);
			}
			else if (this.Match(TokenType.GREATER)) {
				expr = this.ChainedEquality(expr, this.GetToken(-1), Operation.GREATER, false);
			}
			else if (this.Match(TokenType.GREATEREQ)) {
				expr = this.ChainedEquality(expr, this.GetToken(-1), Operation.GREATEREQ, false);
			}

			return expr;
		}

		private Expression Bitwise() {
			Expression expr = this.Additive();
			/*
						while (this.MatchAny(TokenType.BLEFT, TokenType.BRIGTH)) {
							Int32 line = this.GetToken(-1).Line;
							String operation = GetToken(-1).Text;
							expr = new BinaryExpression(expr, this.Additive(), operation, line, this.fileName);
						}
						*/
			return expr;
		}

		private Expression Additive() {
			Expression expr = this.Multiplicate();

			while (this.Match(TokenType.PLUS)) {
				Int32 line = this.GetToken(-1).Line;
				expr = new BinaryExpression(expr, this.Multiplicate(), Operation.PLUS, line, this.fileName);
			}

			while (this.Match(TokenType.MINUS)) {
				Int32 line = this.GetToken(-1).Line;
				expr = new BinaryExpression(expr, this.Multiplicate(), Operation.SUBSTRACT, line, this.fileName);
			}

			return expr;
		}

		private Expression Multiplicate() {
			Expression expr = this.Exponentiation();

			while (this.Match(TokenType.STAR)) {
				Int32 line = this.GetToken(-1).Line;
				expr = new BinaryExpression(expr, this.Multiplicate(), Operation.MUL, line, this.fileName);
			}

			while (this.Match(TokenType.SLASH)) {
				Int32 line = this.GetToken(-1).Line;
				expr = new BinaryExpression(expr, this.Multiplicate(), Operation.DIV, line, this.fileName);
			}

			return expr;
		}

		private Expression Exponentiation() {
			Expression expr = this.Unary();
			/*
						while (true) {
							if (this.Match(TokenType.BXOR)) {
								Int32 line = this.GetToken(-1).Line;
								expr = new BinaryExpression(expr, this.Unary(), Op.POW, line, this.fileName);
								continue;
							}

							break;
						}
						*/
			return expr;
		}

		private Expression Unary() {
			if (this.Match(TokenType.MINUS)) {
				Int32 line = this.GetToken(-1).Line;
				return new BinaryExpression(this.Application(), null, Operation.UNARY_MINUS, line, this.fileName);
			}

			return this.Application();
		}

		private Expression Application() {
			Expression result = this.Colon();

			result = this.Slice(result);

			if (this.Match(TokenType.LPAREN)) {
				List<Expression> args = new List<Expression>();

				while (!this.Match(TokenType.RPAREN)) {
					args.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}

				result = new Applicate(result, args, this.line, this.fileName);
			}

			return result;
		}

		private Expression Slice(Expression sliced) {
			while (this.Match(TokenType.LBRACKET)) {
				List<Expression> indices = new List<Expression>();

				while (!this.Match(TokenType.RBRACKET)) {
					indices.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}

				sliced = new GetIndexE(sliced, indices, this.line, this.fileName);
			}

			if (this.Match(TokenType.EQ)) {
				return new SetIndex(sliced, this.Expression());
			}

			return sliced;
		}

		private Expression Colon() {
			Expression result = As();

			while (Match(TokenType.DOT)) {
				String fieldName = Consume(TokenType.IDENTIFIER).Text;
				result = new ColonExpression(result, fieldName, this.line, this.fileName);
			}

			return result;
		}

		private Expression As() {
			Expression result = this.Primary();

			while (Match(TokenType.AS)) {
				Boolean isSafe = this.Match(TokenType.QUESTION);
				Expression? type = this.ParseType();
				result = new AsExpression(result, type, isSafe, this.line, this.fileName);
			}

			return result;
		}

		private Expression Primary() {
			Token Current = this.GetToken(0);

			if (this.Match(TokenType.IDENTIFIER)) {
				if (this.LookMatch(0, TokenType.IDENTIFIER) || this.LookMatch(0, TokenType.QUESTION)) {
					Expression variableType = new IdExpression(Current.Text, 0, "");
					if (this.Match(TokenType.QUESTION)) {
						variableType = new NullableExpression(variableType);
					}
					String variableName = this.Consume(TokenType.IDENTIFIER).Text;
					Int32 line = this.line;
					this.Consume(TokenType.EQ);
					Expression assignableExpression = this.Expression();

					return new VariableDeclaration(variableType, variableName, assignableExpression, line, this.fileName);
				}

				return new IdExpression(Current.Text, Current.Line, this.fileName);
			}

			if(this.Match(TokenType.IMMUT)) {
				return new IdExpression("immut", line, fileName);
			}

			// Lists
			if (this.Match(TokenType.LBRACE)) {
				List<Expression> elements = new List<Expression>();

				while (!this.Match(TokenType.RBRACE)) {
					elements.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}

				return new SequenceE(elements);
			}

			// Number literals
			if (this.Match(TokenType.NUMBER)) {
				return new ValueE(Double.Parse(Current.Text, System.Globalization.NumberFormatInfo.InvariantInfo));
			}

			if (this.Match(TokenType.LPAREN)) {
				Expression result = this.Expression();
				this.Match(TokenType.RPAREN);
				return result;
			}

			if (this.Match(TokenType.TEXT)) {
				return new StringE(Current.Text);
			}

			return new ValueE(Nil.NilIns);
		}

		private Boolean Match(TokenType type) {
			Token current = this.GetToken(0);

			if (type != current.Type) {
				return false;
			}

			this.line = current.Line;
			this.position++;
			return true;
		}

		private Boolean MatchAny(params TokenType[] type) {
			Token current = this.GetToken(0);

			if (!type.Any(i => i == current.Type)) {
				return false;
			}

			this.line = current.Line;
			this.position++;
			return true;
		}

		private Boolean LookMatch(Int32 pos, TokenType type) {
			return this.GetToken(pos).Type == type;
		}

		private Token GetToken(Int32 offset) {
			Int32 position = this.position + offset;

			if (position >= this.size) {
				return new Token(TokenType.EOF, null);
			}

			return this.tokens[position];
		}

		private Token Consume(TokenType type) {
			Token Current = this.GetToken(0);
			this.line = Current.Line;

			if (type != Current.Type) {
				throw new XenonException("wait another token", fileName: this.fileName, line: this.line);
			}

			this.position++;
			return Current;
		}
	}
}