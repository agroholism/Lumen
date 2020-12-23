using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal sealed class Parser {
		private List<Token> tokens;
		private Int32 size;
		private String file;
		private Int32 position;
		private Int32 line;

		internal Parser(List<Token> tokens, String fileName) {
			this.file = fileName;
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

					Expression ret =
						new Return(!this.IsValidToken() ? UnitLiteral.Instance : this.Expression());

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), ret, UnitLiteral.Instance);
					}

					return ret;
				case TokenType.RAISE: // kk
					this.Match(TokenType.RAISE);
					Int32 line = this.line;

					Expression inraise = !this.IsValidToken() ? UnitLiteral.Instance : this.Expression();

					Expression from = this.Match(TokenType.FROM) ? this.Expression() : null;

					Expression raise = new Raise(inraise, from, this.file, line);

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), raise, UnitLiteral.Instance);
					}

					return raise;
				case TokenType.MATCH:
					this.Match(TokenType.MATCH);
					return this.ParseMatch();
				case TokenType.TRY:
					this.Match(TokenType.TRY);
					return this.ParseTry();
				case TokenType.USE:
					this.Match(TokenType.USE);
					return this.ParseUse();
				case TokenType.ANNOTATION:
					this.Match(TokenType.ANNOTATION);
					return this.ParseWithAnnotation();
				case TokenType.NEXT:
					this.Match(TokenType.NEXT);
					Expression nextExpression = Next.Instance;

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), nextExpression, UnitLiteral.Instance);
					}

					return nextExpression;
				case TokenType.BREAK:
					this.Match(TokenType.BREAK);

					Expression breakExpression =
						new Break(this.LookMatch(0, TokenType.WORD) ?
						this.Consume(TokenType.WORD).Text : null);

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), breakExpression, UnitLiteral.Instance);
					}

					return breakExpression;
				case TokenType.REDO:
					this.Match(TokenType.REDO);

					Expression redoExpression =
						new Redo(this.LookMatch(0, TokenType.WORD) ?
						this.Consume(TokenType.WORD).Text : null);

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), redoExpression, UnitLiteral.Instance);
					}

					return redoExpression;
				case TokenType.RETRY:
					this.Match(TokenType.RETRY);

					Expression retryExpression = new Retry();

					if (this.Match(TokenType.WHEN)) {
						return new Condition(this.Expression(), retryExpression, UnitLiteral.Instance);
					}

					return retryExpression;
				case TokenType.TYPE:
					this.Match(TokenType.TYPE);
					return this.ParseTypeDeclaration();
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
				case TokenType.ASSERT:
					this.Match(TokenType.ASSERT);
					Int32 currentLine = this.line;
					return new Assert(this.Expression(), this.file, currentLine);
				default:
					return this.LogikOr();
			}
		}

		private Expression ParseWithAnnotation() {
			String name = this.Consume(TokenType.WORD).Text;
			this.Match(TokenType.EOC);

			if (name == "intern") {
				Expression innerExpression = this.Expression();

				return new InternWithBindingDeclaration(innerExpression);
			}

			throw new LumenException($"unknown annotation {name}", this.line, this.file);
		}

		private Expression ParseTry() {
			this.Match(TokenType.COLON);
			Expression tryBody = this.Expression();
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

			if (this.Match(TokenType.ENSURE)) {
				this.Match(TokenType.COLON);

				finallyBody = this.Expression();
			}

			return new ExceptionHandling(tryBody, patterns, finallyBody);
		}

		private Expression ParseUse() {
			Int32 savedLine = this.line;

			IPattern pattern = this.ParsePattern();

			this.Consume(TokenType.EQUALS);

			Expression assignable = this.Expression();
			this.Match(TokenType.COLON);

			Expression body = this.Expression();

			return new UseStatement(pattern, assignable, body, this.file, savedLine);
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
			Boolean expectBlockEnd = this.Match(TokenType.BLOCK_START);

			List<ConstructorMetadata> defaultConstructors = new List<ConstructorMetadata>();

			if (!expectBlockEnd) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			while (this.Match(TokenType.BAR)) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			if (defaultConstructors.Count == 0) {
				defaultConstructors.Add(new ConstructorMetadata(typeName, new()));
			}

			List<Expression> derivings = new List<Expression>();
			List<Expression> members = new List<Expression>();

			if (this.Match(TokenType.BLOCK_START) || expectBlockEnd) {
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
			Boolean expectBlockEnd = this.Match(TokenType.BLOCK_START);

			List<ConstructorMetadata> defaultConstructors = new List<ConstructorMetadata>();

			if (!expectBlockEnd) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			while (this.Match(TokenType.BAR)) {
				this.ParseDefaultConstructor(defaultConstructors);
			}

			List<Expression> derivings = new List<Expression>();
			List<Expression> members = new List<Expression>();

			if (this.Match(TokenType.BLOCK_START) || expectBlockEnd) {
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
			Dictionary<String, List<Expression>> paramters = new Dictionary<String, List<Expression>>();
			String constructorName = this.Consume(TokenType.WORD).Text;

			while (this.LookMatch(0, TokenType.WORD) || this.LookMatch(0, TokenType.PAREN_OPEN)) {
				Boolean hasParens = this.Match(TokenType.PAREN_OPEN);

				String parameterName = this.Consume(TokenType.WORD).Text;
				List<Expression> types = new List<Expression>();

				if (this.Match(TokenType.COLON)) {
					do {
						types.Add(this.Primary());
					} while (this.Match(TokenType.SPLIT));
				}

				paramters.Add(parameterName, types);

				if(hasParens) {
					this.Consume(TokenType.PAREN_CLOSE);
				}
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

			return new Match(matchedExpression, patterns);
		}

		private Boolean IsValidRangeToken() {
			return this.IsValidToken()
				|| this.LookMatch(0, TokenType.MINUS)
				|| this.LookMatch(0, TokenType.PLUS)
				|| this.LookMatch(0, TokenType.STAR)
				|| this.LookMatch(0, TokenType.NOT)
				|| this.LookMatch(0, TokenType.TILDE)
				|| this.LookMatch(0, TokenType.POWER)
				|| this.LookMatch(0, TokenType.AMP);
		}

		private IPattern ParsePattern() {
			IPattern result = null;

			if (this.Match(TokenType.DOT2)) {
				if (!this.IsValidRangeToken()) {
					result = new RangePattern(result, null, false);
				}
				else {
					result = new RangePattern(result, this.ParsePattern(), false);
				}
			}

			if (this.Match(TokenType.DOT3)) {
				if (!this.IsValidRangeToken()) {
					result = new RangePattern(result, null, true);
				}
				else {
					result = new RangePattern(result, this.ParsePattern(), true);
				}
			}

			if (this.Match(TokenType.PAREN_OPEN)) {
				result = this.ParsePattern();
				this.Match(TokenType.PAREN_CLOSE);
			}
			else if (this.Match(TokenType.VOID)) {
				result = UnitPattern.Instance;
			}
			else if (this.Match(TokenType.ARRAY_OPEN)) {
				if (this.Match(TokenType.ARRAY_CLOSE)) {
					result = EmptyArrayPattern.Instance;
				}
				else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.ARRAY_CLOSE)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new ArrayPattern(subpatterns);
				}
			}
			else if (this.Match(TokenType.LIST_OPEN)) {
				if (this.Match(TokenType.LIST_CLOSE)) {
					result = EmptyListPattern.Instance;
				}
				else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.LIST_CLOSE)) {
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

						if (this.Match(TokenType.PAREN_OPEN)) {
							while (!this.Match(TokenType.PAREN_CLOSE)) {
								arguments.Add(this.Expression());
								this.Match(TokenType.SPLIT);
							}
						}

						List<IPattern> subpatterns = new List<IPattern>();
						while (!this.LookMatch(0, TokenType.PAREN_CLOSE)
							&& !this.LookMatch(0, TokenType.LIST_CLOSE)
							&& !this.LookMatch(0, TokenType.LAMBDA)
							&& !this.LookMatch(0, TokenType.EQUALS)) {
							subpatterns.Add(this.ParsePattern());
						}
						return new ActivePattern(new IdExpression(name, this.file, this.line), arguments, subpatterns);
					}
					else {
						result = new NamePattern(name);
					}
				}
				else {
					Expression constructor = new IdExpression(name, this.file, this.line);
					while (this.Match(TokenType.DOT)) {
						String fname = this.Consume(TokenType.WORD).Text;
						constructor = new DotOperator(constructor, fname, this.file, this.line);
					}

					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.LookMatch(0, TokenType.EQUALS) && !this.LookMatch(0, TokenType.LIST_CLOSE) && !this.LookMatch(0, TokenType.PAREN_CLOSE) && !this.LookMatch(0, TokenType.LAMBDA) && !this.LookMatch(0, TokenType.BAR)) {
						subpatterns.Add(this.ParsePattern());
					}

					result = new DeconstructPattern(constructor, subpatterns);
				}
			}

			if (result is ValuePattern) {
				if (this.Match(TokenType.DOT2)) {
					if (!this.IsValidToken()) {
						result = new RangePattern(result, null, false);
					}
					else {
						result = new RangePattern(result, this.ParsePattern(), false);
					}
				}

				if (this.Match(TokenType.DOT3)) {
					if (!this.IsValidToken()) {
						result = new RangePattern(result, null, true);
					}
					else {
						result = new RangePattern(result, this.ParsePattern(), true);
					}
				}
			}

			if (this.Match(TokenType.COLON2)) {
				result = new HeadTailPattern(result, this.ParsePattern());
			}

			if (this.Match(TokenType.COLON)) {
				List<Expression> exps = new List<Expression>();

				do {
					if (this.Match(TokenType.NOT)) {
						exps.Add(this.Primary());
						result = new NotPattern(new TypePattern(result, exps));
						exps = new List<Expression>();
					}
					else {
						exps.Add(this.Primary());
					}
				} while (this.Match(TokenType.SPLIT));

				if (exps.Count != 0) {
					result = new TypePattern(result, exps);
				}
			}

			if (this.Match(TokenType.AS)) {
				String ide = this.Consume(TokenType.WORD).Text;
				result = new AsPattern(result, ide);
			}

			if (this.Match(TokenType.WHEN)) {
				Expression exp = this.Expression();
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

				return new Import(pathBuild.ToString(), /*isFrom*/true, importAll, entities, this.file, this.line);
			}

			// import x... (!from)
			StringBuilder pathBuilder = new StringBuilder(this.Consume(TokenType.WORD).Text);

			while (this.Match(TokenType.DOT)) {
				pathBuilder.Append("\\").Append(this.Consume(TokenType.WORD).Text);
			}

			return new Import(pathBuilder.ToString(), false, false, null, this.file, this.line);
		}

		private Expression ParseModule() {
			String name = this.Consume(TokenType.WORD).Text;
			List<Expression> declarations = new List<Expression>();
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
				return new BindingDeclaration(pattern, this.Expression(), this.file, this.line);
			}

			String name = pattern?.ToString() ?? this.Consume(this.GetToken(0).Type).Text;

			List<IPattern> arguments = new List<IPattern>();

			if (this.Match(TokenType.DOT_LESS)) {
				while (!this.Match(TokenType.GREATER)) {
					ContextPattern typeArgName = new ContextPattern(this.Consume(TokenType.WORD).Text);

					if (this.Match(TokenType.IMPLEMENTS)) {
						do {
							typeArgName.AddImplements(Bitwise());
						} while (this.Match(TokenType.SPLIT));
					}

					arguments.Add(typeArgName);
					this.Match(TokenType.SPLIT);
				}
			}

			while (!this.LookMatch(0, TokenType.EQUALS) && !this.LookMatch(0, TokenType.EOC) && !this.LookMatch(0, TokenType.BLOCK_END)) {
				arguments.Add(this.ParsePattern());
			}

			Expression body = null;

			if (this.Match(TokenType.EQUALS)) {
				body = this.Expression();
			}

			return new FunctionDeclaration(name, arguments, body, line, this.file);
		}

		private ForCycle ParseFor() {
			IPattern pattern = this.ParsePattern();

			this.Consume(TokenType.IN);

			Expression container = this.Expression();

			Expression when = this.Match(TokenType.WHEN) ? this.Expression() : null;

			String cycleName = this.Match(TokenType.AS) ? this.Consume(TokenType.WORD).Text : null;

			this.Match(TokenType.COLON);

			Expression body = this.Expression();

			if (when != null) {
				body = new Condition(when, body, UnitLiteral.Instance);
			}

			return new ForCycle(cycleName, pattern, container, body);
		}

		private Expression ParseIf() {
			if (this.Match(TokenType.MATCH)) {
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

				return new ConditionMatch(pattern, assinableExpression, trueBody, falseBody);
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
			if (this.Match(TokenType.MATCH)) {
				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.EQUALS);
				Expression assinableExpression = this.Expression();

				String cycleName = this.Match(TokenType.AS) ? this.Consume(TokenType.WORD).Text : null;

				this.Match(TokenType.COLON);
				Expression body = this.Expression();

				return new WhileMatch(cycleName, pattern, assinableExpression, body);
			}
			else {
				Expression condition = this.Expression();
				String cycleName = this.Match(TokenType.AS) ? this.Consume(TokenType.WORD).Text : null;
				this.Match(TokenType.COLON);
				Expression body = this.Expression();
				return new WhileCycle(cycleName, condition, body);
			}
		}

		private Expression LogikOr() {
			Expression result = this.LogikXor();

			while (true) {
				if (this.Match(TokenType.OR)) {
					result = new BinaryOperator(result, this.LogikXor(), Constants.OR, this.line, this.file);
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
					result = new BinaryOperator(result, this.LogikAnd(), Constants.XOR, this.line, this.file);
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
					result = new BinaryOperator(result, this.Assigment(), Constants.AND, this.line, this.file);
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

				return new BinaryOperator(new InOperator(exp, this.FunctionalOperators(), -1, ""), UnitLiteral.Instance, Constants.NOT, lineNumber, this.file);
			}

			if (this.Match(TokenType.IN)) {
				Int32 lineNumber = this.line;
				return new InOperator(exp, this.FunctionalOperators(), lineNumber, this.file);
			}

			if (this.Match(TokenType.ASSIGN)) {
				Int32 lineNumber = this.line;
				return new BinaryOperator(exp, this.Expression(), "<-", lineNumber, this.file);
			}

			return exp;
		}

		private Expression FunctionalOperators() {
			Expression expr = this.Range();

			while (true) {
				Token current = this.GetToken(0);

				if (current.Type == TokenType.MIDDLE_PRIORITY_RIGTH) {
					this.Match(current.Type);

					expr = new BinaryOperator(expr, this.Range(), current.Text, this.line, this.file);
					current = this.GetToken(0);
					continue;
				}

				if (expr is not BlockE && this.Match(TokenType.FORWARD_PIPE)) {
					expr = new Applicate(this.Range(), new List<Expression> { expr }, this.file, this.line);
					continue;
				}

				if (expr is not BlockE &&  this.Match(TokenType.BACKWARD_PIPE)) {
					expr = new Applicate(expr, new List<Expression> { this.Range() }, this.file, this.line);
					continue;
				}

				break;
			}

			return expr;
		}

		private Expression Range() {
			Expression result = this.Equality();

			if (this.Match(TokenType.DOT2)) {
				if (!this.IsValidRangeToken()) {
					return new RangeOperator(result, null, false, this.line, this.file);
				}

				return new RangeOperator(result, this.Equality(), false, this.line, this.file);
			}

			if (this.Match(TokenType.DOT3)) {
				if (!this.IsValidRangeToken()) {
					return new RangeOperator(result, null, true, this.line, this.file);
				}

				return new RangeOperator(result, this.Equality(), true, this.line, this.file);
			}

			return result;
		}

		private Expression Equality() {
			Expression result = this.Conditional();

			if (this.Match(TokenType.EQUALS) || this.Match(TokenType.NOT_EQUALS)) {
				result = this.ChainedEquality(result, this.GetToken(-1), false);
			}

			if (this.Match(TokenType.MATCH_EQUALS) || this.Match(TokenType.NOT_MATCH_EQUALS) || this.Match(TokenType.SHIP)) {
				result = new BinaryOperator(result, this.Conditional(), this.GetToken(-1).Text, this.line, this.file);
			}

			return result;
		}

		private Expression ChainedEquality(Expression leftExpression, Token token, Boolean fromConditional) {
			Int32 line = this.GetToken(-1).Line;

			BinaryOperator result = new BinaryOperator(leftExpression, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.file);

			if (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryOperator(result, new BinaryOperator(result.expressionTwo, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.file), Constants.AND, line, this.file);
			}

			while (this.Match(token.Type)) {
				line = this.GetToken(-1).Line;
				result = new BinaryOperator(result, new BinaryOperator(((BinaryOperator)result.expressionTwo).expressionTwo, fromConditional ? this.Bitwise() : this.Conditional(), token.Text, line, this.file), Constants.AND, line, this.file);
			}

			return result;
		}

		private Expression Conditional() {
			Expression expr = this.Bitwise();

			while (true) {
				if (this.Match(TokenType.LESS) || this.Match(TokenType.LESS_EQUALS) || this.Match(TokenType.GREATER) || this.Match(TokenType.GREATER_EQUALS)) {
					expr = this.ChainedEquality(expr, this.GetToken(-1), true);
				}
				break;
			}
			return expr;
		}

		private Expression Bitwise() {
			Expression expr = this.Additive();

			while (this.MatchAny(TokenType.SHIFT_LEFT, TokenType.SHIFT_RIGTH)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Additive(), operation, line, this.file);
			}

			return expr;
		}

		private Expression Additive() {
			Expression expr = this.Multiplicate();

			while (this.MatchAny(TokenType.PLUS, TokenType.MINUS)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Multiplicate(), operation, line, this.file);
			}

			return expr;
		}

		private Expression Multiplicate() {
			Expression expr = this.Exponentiation();

			while (this.MatchAny(TokenType.STAR, TokenType.SLASH, TokenType.MODULUS)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				expr = new BinaryOperator(expr, this.Exponentiation(), operation, line, this.file);
			}

			return expr;
		}

		private Expression Exponentiation() {
			Expression expr = this.Unary();

			while (true) {
				if (this.Match(TokenType.POWER)) {
					Int32 line = this.GetToken(-1).Line;
					expr = new BinaryOperator(expr, this.Unary(), Constants.POW, line, this.file);
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

			if (this.Match(TokenType.DOT2)) {
				if (!this.IsValidRangeToken()) {
					return new RangeOperator(null, null, false, this.line, this.file);
				}

				return new RangeOperator(null, this.Equality(), false, this.line, this.file);
			}

			if (this.Match(TokenType.DOT3)) {
				if (!this.IsValidRangeToken()) {
					return new RangeOperator(null, null, true, this.line, this.file);
				}

				return new RangeOperator(null, this.Equality(), true, this.line, this.file);
			}

			if (this.Match(TokenType.NOT)) {
				return new BinaryOperator(this.DoubleColon(), null, "not", this.line, this.file);
			}

			if (this.MatchAny(TokenType.MINUS, TokenType.NOT, TokenType.BANG,
				TokenType.PLUS, TokenType.STAR, TokenType.TILDE, TokenType.POWER, TokenType.AMP)) {
				Int32 line = this.GetToken(-1).Line;
				String operation = this.GetToken(-1).Text;
				return new BinaryOperator(this.DoubleColon(), null, operation, line, this.file);
			}

			return this.DoubleColon();
		}

		private Expression DoubleColon() {
			Expression result = this.Application();

			if (this.Match(TokenType.COLON2)) {
				Int32 lineNumber = this.line;

				Expression right = this.Expression();

				result = new ConsOperator(result, right, this.file, lineNumber);
			}

			return result;
		}

		private Boolean IsValidToken() {
			return this.LookMatch(0, TokenType.PAREN_OPEN)
				|| this.LookMatch(0, TokenType.BANG)
				|| this.LookMatch(0, TokenType.LIST_OPEN)
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

			List<Expression> typeParams = new List<Expression>();
			if (this.Match(TokenType.DOT_LESS)) {
				while (!this.Match(TokenType.GREATER)) {
					typeParams.Add(this.Bitwise());
					this.Match(TokenType.SPLIT);
				}
			}

			if (this.IsValidToken() || typeParams.Count != 0) {
				List<Expression> args = typeParams;

				while (this.IsValidToken()) {
					args.Add(this.Dot());
				}

				result = new Applicate(result, args, this.file, this.line);
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
						res = new DotOperator(res, this.Consume(TokenType.WORD).Text, this.file, line);
					}
					else if (this.LookMatch(0, TokenType.LIST_OPEN)) {
						res = this.Slice(res);
					}

					continue;
				}
				break;
			}

			return res;
		}

		private Expression Slice(Expression sliced) {
			while (this.Match(TokenType.LIST_OPEN)) {
				List<Expression> indices = new List<Expression>();

				while (!this.Match(TokenType.LIST_CLOSE)) {
					indices.Add(this.Expression());
					this.Match(TokenType.SPLIT);
				}

				sliced = new Indexation(sliced, indices, this.line, this.file);
			}

			if (this.Match(TokenType.ASSIGN)) {
				Expression assignable = this.Expression();

				return new IndexationAssign(sliced as Indexation, assignable, this.line, this.file);
			}

			return sliced;
		}

		private Expression Primary() {
			Token Current = this.GetToken(0);

			if (this.Match(TokenType.WORD)) {
				return new IdExpression(Current.Text, this.file, Current.Line);
			}

			if (this.LookMatch(0, TokenType.LIST_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.LIST_OPEN);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.LIST_CLOSE);

				return new ListGenerator(new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body));
			}

			if (this.LookMatch(0, TokenType.ARRAY_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.ARRAY_OPEN);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.ARRAY_CLOSE);

				return new ArrayGenerator(new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body));
			}

			// Lists
			if (this.Match(TokenType.LIST_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.LIST_CLOSE)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_LIST_LITERAL, line: Current.Line, fileName: this.file);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.LIST_CLOSE)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new ListE(elements);
			}

			if (this.Match(TokenType.YIELD)) {
				if (this.Match(TokenType.FROM)) {
					return new YieldFrom(this.Expression());
				}

				if (this.LookMatch(0, TokenType.BLOCK_END) || this.LookMatch(0, TokenType.PAREN_CLOSE) || this.Match(TokenType.EOC) || this.Match(TokenType.EOF)) {
					return new Yield(UnitLiteral.Instance);
				}

				return new Yield(this.Expression());
			}

			// Tail recursion
			if (this.Match(TokenType.TAIL_REC)) {
				List<Expression> args = new List<Expression>();

				while (this.IsValidToken()) {
					args.Add(this.Dot());
				}

				Expression res = new Tailrec(args, this.file, this.line);
				return res;
			}

			// Arrays
			if (this.Match(TokenType.ARRAY_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.ARRAY_CLOSE)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_ARRAY_LITERAL, line: Current.Line, fileName: this.file);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.ARRAY_CLOSE)) {
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
			if (this.LookMatch(0, TokenType.PAREN_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.PAREN_OPEN);
				this.Match(TokenType.FOR);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.PAREN_CLOSE);

				return new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body);
			}

			if (this.Match(TokenType.FUN)) {
				Boolean expectBlockEnd = this.Match(TokenType.BLOCK_START);

				if (this.LookMatch(0, TokenType.BAR)) {
					return this.ParseLambdaMatch(expectBlockEnd);
				}

				List<IPattern> patterns = new List<IPattern>();

				if (this.Match(TokenType.DOT_LESS)) {
					while (!this.Match(TokenType.GREATER)) {
						ContextPattern typeArgName = new ContextPattern(this.Consume(TokenType.WORD).Text);

						if (this.Match(TokenType.IMPLEMENTS)) {
							do {
								typeArgName.AddImplements(Bitwise());
							} while (this.Match(TokenType.SPLIT));
						}

						patterns.Add(typeArgName);
						this.Match(TokenType.SPLIT);
					}
				}

				while (!this.Match(TokenType.LAMBDA)) {
					patterns.Add(this.ParsePattern());
				}

				return new LambdaLiteral(patterns, this.Expression());
			}

			if (this.Match(TokenType.VOID)) {
				return UnitLiteral.Instance;
			}

			if (this.Match(TokenType.PAREN_OPEN)) {
				Expression result = this.Expression();

				if (this.LookMatch(0, TokenType.SPLIT) && result is RangeOperator rangeOperator) {
					this.Match(TokenType.SPLIT);
					rangeOperator.AddStep(this.Expression());
				}

				this.Match(TokenType.PAREN_CLOSE);
				return result;
			}

			if (this.Match(TokenType.TEXT)) {
				return new TextLiteral(Current.Text);
			}

			return this.BlockExpression();
		}

		private Expression ParseLambdaMatch(Boolean expectBlockEnd) {
			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			while (this.Match(TokenType.BAR)) {
				IPattern pattern = this.ParsePattern();
				this.Match(TokenType.LAMBDA);

				Expression body = this.Expression();

				patterns.Add(pattern, body);
			}

			if (expectBlockEnd) {
				this.Match(TokenType.BLOCK_END);
			}

			return new LambdaLiteral(new List<IPattern> { new NamePattern("<x>") },
				new Match(new IdExpression("<x>", this.file, this.line), patterns));
		}

		private Expression BlockExpression() {
			if (this.LookMatch(0, TokenType.BLOCK_START)) {
				return this.AloneORBlock();
			}

			if (this.Match(TokenType.EOC)) {
				return UnitLiteral.Instance;
			}

			throw new LumenException(Exceptions.UNEXCEPTED_TOKEN.F(this.GetToken(0).Type), fileName: this.file, line: this.line);
		}

		private Expression AloneORBlock() {
			BlockE block = new BlockE();
			this.Match(TokenType.BLOCK_START);

			Int32 line = this.line;
			while (!this.Match(TokenType.BLOCK_END)) {
				block.Add(this.Expression());
				this.Match(TokenType.EOC);
			}

			return block;
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
				throw new LumenException(Exceptions.WAIT_ANOTHER_TOKEN.F(type.ToString(), Current.Type.ToString()), fileName: this.file, line: this.line);
			}

			this.position++;
			return Current;
		}
	}
}
// 1229 -> 1143 -> 961