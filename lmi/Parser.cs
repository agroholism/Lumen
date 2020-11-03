using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
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
			Token current = this.GetToken(0);

			switch (current.Type) {
				case TokenType.RETURN: // kk
					this.Match(TokenType.RETURN);

					if (this.LookMatch(0, TokenType.BLOCK_END) || this.Match(TokenType.EOC) || this.Match(TokenType.EOF)) {
						return new Return(UnitLiteral.Instance);
					}

					return new Return(this.Expression());
				case TokenType.RAISE: // kk
					this.Match(TokenType.RAISE);
					Int32 line = this.line;
					return new Raise(this.Expression(), this.fileName, line);
				case TokenType.MATCH:
					this.Match(TokenType.MATCH);
					return this.ParseMatch();
				case TokenType.TRY:
					this.Match(TokenType.TRY);
					return this.ParseTry();
				case TokenType.USE:
					this.Match(TokenType.USE);
					return this.ParseUse();
				case TokenType.NEXT:
					this.Match(TokenType.NEXT);
					return Next.Instance;
				case TokenType.BREAK:
					this.Match(TokenType.BREAK);

					if (this.LookMatch(0, TokenType.NUMBER)) {
						return new Break((Int32)Double.Parse(this.Consume(TokenType.NUMBER).Text));
					}

					return new Break(1);
				case TokenType.TYPE:
					this.Match(TokenType.TYPE);
					return this.ParseTypeDeclaration();
				case TokenType.OPEN:
					this.Match(TokenType.OPEN);
					return new OpenModule(this.Expression());
				case TokenType.IMPORT:
					this.Match(TokenType.IMPORT);
					return this.ParseImport();
				case TokenType.IF:
					this.Match(TokenType.IF);
					return this.ParseIf();
				case TokenType.MODULE:
					this.Match(TokenType.MODULE);
					return this.ParseModule();
				case TokenType.WHILE:
					this.Match(TokenType.WHILE);
					return this.ParseWhile();
				case TokenType.LET:
					this.Match(TokenType.LET);
					return this.ParseDeclaration();
				case TokenType.FOR:
					this.Match(TokenType.FOR);
					return this.ParseFor();
				default:
					return this.LogikOr();
			}
		}

		private Expression ParseTry() {
			this.Match(TokenType.COLON);
			Expression tryBody = Expression();
			this.Match(TokenType.EOC);
			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();
			Expression finallyBody = null;

			if (this.Match(TokenType.EXCEPT)) {
				this.Match(TokenType.COLON);

				while (this.Match(TokenType.BAR)) {
					IPattern pattern = this.ParsePattern();
					this.Match(TokenType.LAMBDA);

					Expression body = this.Expression();
					this.Match(TokenType.EOC);

					patterns.Add(pattern, body);
				}
			}

			if (this.Match(TokenType.FINALLY)) {
				this.Match(TokenType.COLON);

				finallyBody = Expression();
			}

			return new ExceptionHandling(tryBody, patterns, finallyBody);
		}

		private Expression ParseUse() {
			IPattern name = this.ParsePattern();
			this.Match(TokenType.EQUALS);
			Expression assignable = this.Expression();
			this.Match(TokenType.COLON);
			Expression body = this.Expression();

			return new UseStatement(name, assignable, body);
		}

		private Expression ParseTypeDeclaration() {
			// type typeName
			String typeName = this.Consume(TokenType.WORD).Text;
			// type typeName =
			this.Consume(TokenType.EQUALS);

			Token currentToken = this.GetToken(0);
			if (currentToken.Type == TokenType.WORD) {
				if (currentToken.Text == "variant") {
					this.Match(currentToken.Type);
					return this.ParseVariantDeclaration(typeName);
				}
				else if (currentToken.Text == "exception") {
					this.Match(currentToken.Type);
					return this.ParseExceptionDeclaration(typeName);
				}
				else if (currentToken.Text == "class") {
					this.Match(currentToken.Type);
					return this.ParseClassDeclaration(typeName);
				}
				else if (currentToken.Text == "alias") {
					this.Match(currentToken.Type);
					return this.ParseAliasDeclaration(typeName);
				}
			}

			return this.ParseVariantDeclaration(typeName);
		}

		private Expression ParseExceptionDeclaration(String typeName) {
			List<ConstructorMetadata> defaultConstructors = new List<ConstructorMetadata>();
			while (!this.LookMatch(0, TokenType.BLOCK_START)
				&& !this.Match(TokenType.EOC)
				&& !this.Match(TokenType.EOF)) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			if (defaultConstructors.Count == 0) {
				defaultConstructors.Add(new ConstructorMetadata(typeName, new List<String>()));
			}

			List<Expression> derivings = new List<Expression>();
			List<Expression> members = new List<Expression>();

			if (this.Match(TokenType.BLOCK_START)) {
				while (!this.Match(TokenType.BLOCK_END) && !this.Match(TokenType.EOF)) {
					if (this.Match(TokenType.IMPLEMENTS)) {
						derivings.Add(this.Expression());
					}
					else {
						members.Add(this.Expression());
					}
					this.Match(TokenType.EOC);
				}
			}

			return new ExceptionDeclaration(typeName, defaultConstructors, members, derivings);
		}

		private Expression ParseVariantDeclaration(String typeName) {
			List<ConstructorMetadata> defaultConstructors = new List<ConstructorMetadata>();
			while (!this.LookMatch(0, TokenType.BLOCK_START)
				&& !this.Match(TokenType.EOC)
				&& !this.Match(TokenType.EOF)) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			List<Expression> derivings = new List<Expression>();
			List<Expression> members = new List<Expression>();

			if (this.Match(TokenType.BLOCK_START)) {
				while (!this.Match(TokenType.BLOCK_END) && !this.Match(TokenType.EOF)) {
					if (this.Match(TokenType.IMPLEMENTS)) {
						derivings.Add(this.Expression());
					}
					else {
						members.Add(this.Expression());
					}
					this.Match(TokenType.EOC);
				}
			}

			return new VariantDeclaration(typeName, defaultConstructors, members, derivings);
		}

		private Expression ParseAliasDeclaration(String typeName) {
			throw new NotImplementedException();
		}

		private Expression ParseClassDeclaration(String typeName) {
			List<Expression> members = new List<Expression>();
			List<Expression> derivings = new List<Expression>();

			if (this.Match(TokenType.BLOCK_START)) {
				while (!this.Match(TokenType.BLOCK_END) && !this.Match(TokenType.EOF)) {
					if (this.Match(TokenType.IMPLEMENTS)) {
						derivings.Add(this.Expression());
					}
					else {
						members.Add(this.Expression());
					}
					this.Match(TokenType.EOC);
				}
			}

			return new ClassDeclaration(typeName, members, derivings);
		}

		private void ParseDefaultConstructor(List<ConstructorMetadata> defaultConstructors) {
			List<String> paramters = new List<String>();
			String constructorName = this.Consume(TokenType.WORD).Text;

			while (!this.Match(TokenType.BAR) && !this.LookMatch(0, TokenType.EOC) && !this.LookMatch(0, TokenType.BLOCK_START) && !this.Match(TokenType.EOF)) {
				paramters.Add(this.Consume(TokenType.WORD).Text);
			}

			defaultConstructors.Add(new ConstructorMetadata(constructorName, paramters));
		}

		private Expression ParseMatch() {
			Expression matchedExpression = this.Expression();
			this.Match(TokenType.COLON);
			this.Match(TokenType.EOC);

			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			while (this.LookMatch(0, TokenType.BAR)) {
				this.Match(TokenType.BAR);

				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.LAMBDA);

				Expression body = this.Expression();
				this.Match(TokenType.EOC);

				patterns.Add(pattern, body);
			}

			return new MatchE(matchedExpression, patterns);
		}

		private IPattern ParsePattern() {
			IPattern result = null;

			if (this.Match(TokenType.DOTDOT)) {
				if (!this.ValidToken()) {
					result = new RangePattern(result, null, false);
				}
				else {
					result = new RangePattern(result, this.ParsePattern(), false);
				}
			}

			if (this.Match(TokenType.DOTDOTDOT)) {
				if (!this.ValidToken()) {
					result = new RangePattern(result, null, true);
				}
				else {
					result = new RangePattern(result, this.ParsePattern(), true);
				}
			}

			if (this.Match(TokenType.LPAREN)) {
				result = this.ParsePattern();
				this.Match(TokenType.RPAREN);
			}
			else if (this.Match(TokenType.VOID)) {
				result = UnitPattern.Instance;
			}
			else if (this.Match(TokenType.ARRAY_OPEN)) {
				if (this.Match(TokenType.ARRAY_CLOSED)) {
					result = EmptyArrayPattern.Instance;
				}
				else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.ARRAY_CLOSED)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new ArrayPattern(subpatterns);
				}
			}
			else if (this.Match(TokenType.LBRACKET)) {
				if (this.Match(TokenType.RBRACKET)) {
					result = EmptyListPattern.Instance;
				}
				else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.RBRACKET)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new ListPattern(subpatterns);
				}
			}
			else if (this.LookMatch(0, TokenType.NUMBER)) {
				result = new ValuePattern(new Number(Double.Parse(this.Consume(TokenType.NUMBER).Text)));
			}
			else if (this.LookMatch(0, TokenType.TEXT)) {
				result = new ValuePattern(new Text(this.Consume(TokenType.TEXT).Text));
			}
			else if (this.LookMatch(0, TokenType.WORD)) {
				String name = this.Consume(TokenType.WORD).Text;

				if (name == "_") {
					result = DiscordPattern.Instance;
				}
				else if (name == "true") {
					result = new ValuePattern(Const.TRUE);
				}
				else if (name == "false") {
					result = new ValuePattern(Const.FALSE);
				}
				else if (Char.IsLower(name[0]) && !this.LookMatch(0, TokenType.DOT)) {
					if (this.Match(TokenType.QUESTION)) {
						List<Expression> arguments = new List<Expression>();

						if (this.Match(TokenType.LPAREN)) {
							while (!this.Match(TokenType.RPAREN)) {
								arguments.Add(this.Expression());
								this.Match(TokenType.SPLIT);
							}
						}

						List<IPattern> subpatterns = new List<IPattern>();
						while (!this.LookMatch(0, TokenType.RPAREN) && !this.LookMatch(0, TokenType.RBRACKET) && !this.LookMatch(0, TokenType.LAMBDA)) {
							subpatterns.Add(this.ParsePattern());
						}
						return new ActivePattern(new IdExpression(name, this.line, this.fileName), arguments, subpatterns);
					}
					else {
						result = new NamePattern(name);
					}
				}
				else {
					Expression constructor = new IdExpression(name, this.line, this.fileName);
					while (this.Match(TokenType.DOT)) {
						String fname = this.Consume(TokenType.WORD).Text;
						constructor = new DotOperator(constructor, fname, this.fileName, this.line);
					}

					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.LookMatch(0, TokenType.RBRACKET) && !this.LookMatch(0, TokenType.RPAREN) && !this.LookMatch(0, TokenType.LAMBDA) && !this.LookMatch(0, TokenType.BAR)) {
						subpatterns.Add(this.ParsePattern());
					}

					result = new DeconstructPattern(constructor, subpatterns);
				}
			}

			if (result is ValuePattern) {
				if (this.Match(TokenType.DOTDOT)) {
					if (!this.ValidToken()) {
						result = new RangePattern(result, null, false);
					}
					else {
						result = new RangePattern(result, this.ParsePattern(), false);
					}
				}

				if (this.Match(TokenType.DOTDOTDOT)) {
					if (!this.ValidToken()) {
						result = new RangePattern(result, null, true);
					}
					else {
						result = new RangePattern(result, this.ParsePattern(), true);
					}
				}
			}

			if (this.Match(TokenType.COLONCOLON)) {
				result = new HeadTailPattern(result, this.ParsePattern());
			}

			if (this.Match(TokenType.COLON)) {
				List<Expression> exps = new List<Expression> { this.Primary() };
				while (this.Match(TokenType.SPLIT)) {
					exps.Add(this.Primary());
				}
				result = new TypePattern(result, exps);
			}

			if (this.Match(TokenType.AS)) {
				String ide = this.Consume(TokenType.WORD).Text;
				result = new AsPattern(result, ide);
			}

			if (this.Match(TokenType.IF)) {
				Expression exp = this.Conditional();
				result = new WherePattern(result, exp);
			}

			while (this.Match(TokenType.BAR)) {
				IPattern second = this.ParsePattern();
				result = new OrPattern(result, second);
			}

			return result;
		}

		// TODO
		private Expression ParseImport() {
			// import x, || import x from
			if (this.LookMatch(1, TokenType.SPLIT) || this.LookMatch(1, TokenType.FROM)) {
				List<String> entities = new List<String>();
				Boolean importAll = false;

				// import * from
				if (this.Match(TokenType.STAR)) {
					importAll = true;
				}
				else {
					while (this.LookMatch(1, TokenType.SPLIT)) {
						entities.Add(this.Consume(TokenType.WORD).Text);
						this.Match(TokenType.SPLIT);
					}
					entities.Add(this.Consume(TokenType.WORD).Text);
				}

				this.Consume(TokenType.FROM);

				// name1.name2....
				StringBuilder pathBuild = new StringBuilder(this.Consume(TokenType.WORD).Text);

				while (this.Match(TokenType.DOT)) {
					pathBuild.Append("\\").Append(this.Consume(TokenType.WORD).Text);
				}

				return new Import(pathBuild.ToString(), /*isFrom*/true, importAll, entities, this.fileName, this.line);
			}

			// import x... (!from)
			StringBuilder pathBuilder = new StringBuilder(this.Consume(TokenType.WORD).Text);

			while (this.Match(TokenType.DOT)) {
				pathBuilder.Append("\\").Append(this.Consume(TokenType.WORD).Text);
			}

			return new Import(pathBuilder.ToString(), false, false, null, this.fileName, this.line);
		}

		private Expression ParseModule() {
			String name = this.Consume(TokenType.WORD).Text;
			List<Expression> declarations = new List<Expression>();
			this.Match(TokenType.WHERE);
			this.Match(TokenType.BLOCK_START);
			while (!this.Match(TokenType.BLOCK_END)) {
				declarations.Add(this.Expression());
				this.Match(TokenType.EOC);
			}
			return new ModuleDeclaration(name, declarations);
		}

		private Expression ParseDeclaration() {
			Int32 line = this.line;

			IPattern pattern = this.ParsePattern();

			if (this.Match(TokenType.EQUALS)) {
				return new BindingDeclaration(pattern, this.Expression(), this.fileName, this.line);
			}

			String name = pattern?.ToString() ?? this.Consume(this.GetToken(0).Type).Text;

			List<IPattern> arguments = new List<IPattern>();
			while (!this.LookMatch(0, TokenType.EQUALS) && !this.LookMatch(0, TokenType.EOC) && !this.LookMatch(0, TokenType.BLOCK_END)) {
				arguments.Add(this.ParsePattern());
			}

			Expression body = null;

			if (this.Match(TokenType.EQUALS)) {
				body = this.Expression();
			}

			return new FunctionDeclaration(name, arguments, body, line, this.fileName);
		}

		private ForCycle ParseFor() {
			IPattern pattern = this.ParsePattern();

			this.Consume(TokenType.IN);

			Expression container = this.Expression();

			this.Match(TokenType.COLON);

			Expression body = this.Expression();

			return new ForCycle(pattern, container, body);
		}

		private Expression ParseIf() {
			if (this.Match(TokenType.LET)) {
				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.EQUALS);
				Expression assinableExpression = this.Expression();

				this.Match(TokenType.COLON);

				Expression trueBody = this.Expression();

				Expression falseBody = UnitLiteral.Instance;

				this.Match(TokenType.EOC);

				if (this.Match(TokenType.ELSE)) {
					this.Match(TokenType.COLON);
					falseBody = this.Expression();
				}

				return new IfLet(pattern, assinableExpression, trueBody, falseBody);
			}
			else {

				Expression condition = this.Expression();

				this.Match(TokenType.COLON);

				Expression trueBody = this.Expression();

				Expression falseBody = UnitLiteral.Instance;

				this.Match(TokenType.EOC);

				if (this.Match(TokenType.ELSE)) {
					this.Match(TokenType.COLON);
					falseBody = this.Expression();
				}

				return new Condition(condition, trueBody, falseBody);
			}
		}

		private Expression ParseWhile() {
			if (this.Match(TokenType.LET)) {
				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.EQUALS);
				Expression assinableExpression = this.Expression();

				this.Match(TokenType.COLON);
				Expression body = this.Expression();

				return new WhileLet(pattern, assinableExpression, body);
			}
			else {
				Expression condition = this.Expression();
				this.Match(TokenType.COLON);
				Expression body = this.Expression();
				return new WhileExpression(condition, body);
			}
		}

		private Expression LogikOr() {
			Expression result = this.LogikXor();

			while (true) {
				if (this.Match(TokenType.OR)) {
					result = new BinaryOperator(result, this.LogikXor(), Constants.OR, this.line, this.fileName);
					continue;
				}
				break;
			}

			return result;
		}

		private Expression LogikXor() {
			Expression result = this.LogikAnd();

			while (true) {
				if (this.Match(TokenType.XOR)) {
					result = new BinaryOperator(result, this.LogikAnd(), Constants.XOR, this.line, this.fileName);
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
					result = new BinaryOperator(result, this.Assigment(), Constants.AND, this.line, this.fileName);
					continue;
				}
				break;
			}
			return result;
		}

		private Expression Assigment() {
			Expression exp = this.FunctionalOperators();

			if (this.LookMatch(0, TokenType.NOT) && this.LookMatch(1, TokenType.IN)) {
				this.Match(TokenType.NOT);
				this.Match(TokenType.IN);

				Int32 lineNumber = this.line;

				return new BinaryOperator(new InOperator(exp, this.FunctionalOperators(), -1, ""), UnitLiteral.Instance, Constants.NOT, lineNumber, this.fileName);
			}

			if (this.Match(TokenType.IN)) {
				Int32 lineNumber = this.line;
				return new InOperator(exp, this.FunctionalOperators(), lineNumber, this.fileName);
			}

			if (this.Match(TokenType.ASSIGN)) {
				Int32 lineNumber = this.line;
				return new BinaryOperator(exp, this.Expression(), "<-", lineNumber, this.fileName);
			}

			return exp;
		}

		private Expression FunctionalOperators() {
			Expression expr = this.Range();

			while (true) {
				Token current = this.GetToken(0);

				if (current.Type == TokenType.MIDDLE_PRIORITY_RIGTH) {
					this.Match(current.Type);

					expr = new BinaryOperator(expr, this.Range(), current.Text, this.line, this.fileName);
					current = this.GetToken(0);
					continue;
				}

				if (this.Match(TokenType.FPIPE)) {
					expr = new Applicate(this.Range(), new List<Expression> { expr }, this.line, this.fileName);
					continue;
				}

				if (this.Match(TokenType.BPIPE)) {
					expr = new Applicate(expr, new List<Expression> { this.Range() }, this.line, this.fileName);
					continue;
				}

				break;
			}

			return expr;
		}

		private Expression Range() {
			Expression result = this.Equality();

			if (this.Match(TokenType.DOTDOT)) {
				result = new BinaryOperator(result, this.Equality(), Constants.RANGE_EXCLUSIVE, this.line, this.fileName);
			}

			if (this.Match(TokenType.DOTDOTDOT)) {
				return new BinaryOperator(result, this.Equality(), Constants.RANGE_INCLUSIVE, this.line, this.fileName);
			}

			return result;
		}

		private Expression Equality() {
			Expression result = this.Conditional();

			if (this.Match(TokenType.EQUALS) || this.Match(TokenType.NOT_EQUALS)) {
				result = this.ChainedEquality(result, this.GetToken(-1), false);
			}

			if (this.Match(TokenType.EQMATCH) || this.Match(TokenType.EQNOTMATCH) || this.Match(TokenType.SHIP)) {
				result = new BinaryOperator(result, this.Conditional(), this.GetToken(-1).Text, this.line, this.fileName);
			}

			return result;
		}

		private Expression ChainedEquality(Expression leftExpression, Token token, Boolean fromConditional) {
			Int32 line = this.GetToken(-1).Line;

			BinaryOperator result = new BinaryOperator(leftExpression, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.fileName);

			if (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryOperator(result, new BinaryOperator(result.expressionTwo, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.fileName), Constants.AND, line, this.fileName);
			}

			while (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryOperator(result, new BinaryOperator(((BinaryOperator)result.expressionTwo).expressionTwo, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.fileName), Constants.AND, line, this.fileName);
			}

			return result;
		}

		private Expression Conditional() {
			Expression expr = this.Bitwise();

			while (true) {
				if (this.Match(TokenType.LT) || this.Match(TokenType.LTEQ) || this.Match(TokenType.GT) || this.Match(TokenType.GTEQ)) {
					expr = this.ChainedEquality(expr, this.GetToken(-1), true);
				}
				break;
			}
			return expr;
		}

		private Expression Bitwise() {
			Expression expr = this.Additive();

			while (this.MatchAny(TokenType.BLEFT, TokenType.BRIGTH)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Additive(), operation, line, this.fileName);
			}

			return expr;
		}

		private Expression Additive() {
			Expression expr = this.Multiplicate();

			while (this.MatchAny(TokenType.PLUS, TokenType.MINUS)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Multiplicate(), operation, line, this.fileName);
			}

			return expr;
		}

		private Expression Multiplicate() {
			Expression expr = this.Exponentiation();

			while (this.MatchAny(TokenType.STAR, TokenType.SLASH, TokenType.MOD)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Exponentiation(), operation, line, this.fileName);
			}

			return expr;
		}

		private Expression Exponentiation() {
			Expression expr = this.Unary();

			while (true) {
				if (this.Match(TokenType.BXOR)) {
					Int32 line = this.GetToken(-1).Line;
					expr = new BinaryOperator(expr, this.Unary(), Constants.POW, line, this.fileName);
					continue;
				}

				break;
			}

			return expr;
		}

		private Expression Unary() {
			if (this.Match(TokenType.FROM)) {
				return new From(this.Expression());
			}


			if (this.Match(TokenType.NOT)) {
				return new BinaryOperator(this.DoubleColon(), null, "not", this.line, this.fileName);
			}

			if (this.MatchAny(TokenType.MINUS, TokenType.NOT, TokenType.BANG,
				TokenType.PLUS, TokenType.STAR, TokenType.TILDE, TokenType.BXOR, TokenType.AMP)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				return new BinaryOperator(this.DoubleColon(), null, operation, line, this.fileName);
			}

			return this.DoubleColon();
		}

		private Expression DoubleColon() {
			Expression result = this.Application();

			if (this.Match(TokenType.COLONCOLON)) {
				Int32 lineNumber = this.line;

				Expression right = this.Expression();

				result = new ConsOperator(result, right, this.fileName, lineNumber);
			}

			return result;
		}

		private Boolean ValidToken() {
			return this.LookMatch(0, TokenType.LPAREN)
				|| this.LookMatch(0, TokenType.BIG_NUMBER)
				|| this.LookMatch(0, TokenType.BANG)
				|| this.LookMatch(0, TokenType.BNUMBER)
				|| this.LookMatch(0, TokenType.HARDNUMBER)
				|| this.LookMatch(0, TokenType.LBRACKET)
				|| this.LookMatch(0, TokenType.ARRAY_OPEN)
				|| this.LookMatch(0, TokenType.NUMBER)
				|| this.LookMatch(0, TokenType.TEXT)
				|| this.LookMatch(0, TokenType.VOID)
				|| this.LookMatch(0, TokenType.YIELD)
				|| this.LookMatch(0, TokenType.FUN)
				|| this.LookMatch(0, TokenType.WORD);
		}

		private Expression Application() {
			Expression result = this.Dot();

			if (this.ValidToken()) {
				List<Expression> args = new List<Expression>();

				while (this.ValidToken()) {
					args.Add(this.Dot());
				}

				result = new Applicate(result, args, this.line, this.fileName);
				// this.Match(TokenType.EOC);
			}

			return result;
		}

		private Expression Dot(Expression inn = null) {
			Expression res = inn ?? this.Primary();

			while (true) {
				Int32 line = this.line;
				// (...).
				if (this.Match(TokenType.DOT)) {
					// (...).x
					if (this.LookMatch(0, TokenType.WORD)) {
						res = new DotOperator(res, this.Consume(TokenType.WORD).Text, this.fileName, line);
					}
					else if (this.LookMatch(0, TokenType.LBRACKET)) {
						res = this.Slice(res);
					}

					continue;
				}
				break;
			}

			return res;
		}

		private Expression Slice(Expression sliced) {
			while (this.Match(TokenType.LBRACKET)) {
				List<Expression> indices = new List<Expression>();

				while (!this.Match(TokenType.RBRACKET)) {
					indices.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}

				sliced = new Indexation(sliced, indices, this.line, this.fileName);
			}

			if (this.Match(TokenType.ASSIGN)) {
				Expression assignable = this.Expression();

				return new IndexationAssign(sliced as Indexation, assignable, this.line, this.fileName);
			}

			return sliced;
		}

		private Expression Primary() {
			Token Current = this.GetToken(0);

			if (this.Match(TokenType.WORD)) {
				return new IdExpression(Current.Text, Current.Line, this.fileName);
			}

			if (this.LookMatch(0, TokenType.LBRACKET) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.LBRACKET);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.RBRACKET);

				return new ListGenerator(new SequenceGenerator(result.pattern, result.expression, result.body));
			}

			if (this.LookMatch(0, TokenType.ARRAY_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.ARRAY_OPEN);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.ARRAY_CLOSED);

				return new ArrayGenerator(new SequenceGenerator(result.pattern, result.expression, result.body));
			}

			// Lists
			if (this.Match(TokenType.LBRACKET)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.RBRACKET)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_LIST_LITERAL, line: Current.Line, fileName: this.fileName);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.RBRACKET)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new ListE(elements);
			}

			if (this.Match(TokenType.YIELD)) {
				if (this.Match(TokenType.FROM)) {
					return new YieldFrom(this.Expression());
				}

				if (this.LookMatch(0, TokenType.BLOCK_END) || this.LookMatch(0, TokenType.RPAREN) || this.Match(TokenType.EOC) || this.Match(TokenType.EOF)) {
					return new Yield(UnitLiteral.Instance);
				}

				return new Yield(this.Expression());
			}

			// Tail recursion
			if (this.Match(TokenType.TAIL_REC)) {
				List<Expression> args = new List<Expression>();

				while (this.ValidToken()) {
					args.Add(this.Dot());
				}

				Expression res = new TailRecursion(args, this.fileName, this.line);
				return res;
			}

			// Arrays
			if (this.Match(TokenType.ARRAY_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.ARRAY_CLOSED)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_ARRAY_LITERAL, line: Current.Line, fileName: this.fileName);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.ARRAY_CLOSED)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new ArrayLiteral(elements);
			}

			// Number literals
			if (this.Match(TokenType.NUMBER)) {
				return new ValueLiteral(Double.Parse(Current.Text, System.Globalization.NumberFormatInfo.InvariantInfo));
			}

			// Sequence generators
			if (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.LPAREN);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.RPAREN);

				return new SequenceGenerator(result.pattern, result.expression, result.body);
			}

			if (this.Match(TokenType.FUN)) {
				if (this.LookMatch(0, TokenType.BAR)) {
					return this.ParseLambdaMatch();
				}

				List<IPattern> patterns = new List<IPattern>();

				while (!this.Match(TokenType.LAMBDA)) {
					patterns.Add(this.ParsePattern());
				}

				return new LambdaLiteral(patterns, this.Expression());
			}

			if (this.Match(TokenType.VOID)) {
				return UnitLiteral.Instance;
			}

			if (this.Match(TokenType.LPAREN)) {
				Int32 line = this.line;
				Expression result = this.Expression();
				this.Match(TokenType.RPAREN);
				return result;
			}

			if (this.Match(TokenType.TEXT)) {
				return new TextLiteral(Current.Text);
			}

			return this.BlockExpression();
		}

		private Expression ParseLambdaMatch() {
			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			while (this.Match(TokenType.BAR)) {
				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.LAMBDA);

				Expression body = this.Expression();

				patterns.Add(pattern, body);
			}

			return new LambdaLiteral(new List<IPattern> { new NamePattern("<x>") },
				new MatchE(new IdExpression("<x>", this.line, this.fileName), patterns));
		}

		private Expression BlockExpression() {
			if (this.LookMatch(0, TokenType.BLOCK_START)) {
				return this.AloneORBlock();
			}

			if (this.Match(TokenType.EOC)) {
				return UnitLiteral.Instance;
			}

			throw new LumenException(Exceptions.UNEXCEPTED_TOKEN.F(this.GetToken(0).Type), fileName: this.fileName, line: this.line);
		}

		private Expression AloneORBlock() {
			BlockE Block = new BlockE();
			this.Match(TokenType.BLOCK_START);

			Int32 line = this.line;
			while (!this.Match(TokenType.BLOCK_END)) {
				if (this.Match(TokenType.EOF)) {
					throw new LumenException("пропущена закрывающая фигурная скобка", line, this.fileName);
				}

				Block.Add(this.Expression());
				this.Match(TokenType.EOC);
			}

			// Optimization
			if (Block.expressions.Count == 1) {
				if (Block.expressions[0] is BindingDeclaration vd) {
					return vd.assignableExpression;
				}
				else {
					return Block.expressions[0];
				}
			}

			return Block;
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
				throw new LumenException(Exceptions.WAIT_ANOTHER_TOKEN.F(type.ToString(), Current.Type.ToString()), fileName: this.fileName, line: this.line);
			}

			this.position++;
			return Current;
		}
	}
}
// 1229 -> 1143 -> 961