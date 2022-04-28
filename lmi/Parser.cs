using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

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
				result.Add(this.PrivateExpression());
				this.Match(TokenType.EOC);
			}

			return result;
		}

		private Expression PrivateExpression() {
			if (this.Match(TokenType.PRIVATE)) {
				Expression declaraion = this.Expression();

				if (declaraion is FunctionDeclaration or VariantDeclaration or BindingDeclaration or ModuleDeclaration) {
					return new PrivateDeclaration(declaraion);
				} else {
					throw new LumenException("can not use private modifier here");
				}
			}

			return this.Expression();
		}

		private Expression Expression() {
			Token current = this.GetToken(0);

			return current.Type switch {
				TokenType.RETURN => this.ParseReturn(),
				TokenType.MATCH => this.ParseMatch(),
				TokenType.TRY => this.ParseTry(),
				TokenType.USE => this.ParseUse(),
				TokenType.ANNOTATION => this.ParseWithAnnotation(),
				TokenType.NEXT => this.ParseNext(),
				TokenType.EXIT => this.ParseExit(),
				TokenType.REDO => this.ParseRedo(),
				TokenType.RETRY => this.ParseRetry(),
				TokenType.TYPE => this.ParseTypeDeclaration(),
				TokenType.IMPORT => this.ParseImport(),
				TokenType.IF => this.ParseIf(),
				TokenType.MODULE => this.ParseModule(),
				TokenType.LOOP => this.ParseLoop(),
				TokenType.LET => this.ParseDeclaration(),
				TokenType.FOR => this.ParseFor(),
				TokenType.ASSERT => this.ParseAssert(),
				_ => this.LogikOr(),
			};
		}

		private Expression ParseAssert() {
			this.Consume(TokenType.ASSERT);

			Int32 line = this.line;
			return new Assert(this.Expression(), this.file, line);
		}

		private Expression ParseRetry() {
			this.Consume(TokenType.RETRY);

			Expression retry = new Retry();

			return this.Match(TokenType.WHEN)
				? new Condition(this.Expression(), retry, UnitLiteral.Instance)
				: retry;
		}

		private Expression ParseRedo() {
			this.Consume(TokenType.REDO);

			Expression redo =
				new Redo(this.LookMatch(0, TokenType.WORD) ? this.Consume(TokenType.WORD).Text : null);

			return this.Match(TokenType.WHEN)
				? new Condition(this.Expression(), redo, UnitLiteral.Instance)
				: redo;
		}

		private Expression ParseExit() {
			this.Consume(TokenType.EXIT);

			Expression exit =
				new Break(this.LookMatch(0, TokenType.WORD) ? this.Consume(TokenType.WORD).Text : null);

			return this.Match(TokenType.WHEN)
				? new Condition(this.Expression(), exit, UnitLiteral.Instance)
				: exit;
		}

		private Expression ParseNext() {
			this.Consume(TokenType.NEXT);

			Expression next =
				new Next(this.LookMatch(0, TokenType.WORD) ? this.Consume(TokenType.WORD).Text : null);

			return this.Match(TokenType.WHEN)
				? new Condition(this.Expression(), next, UnitLiteral.Instance)
				: next;
		}

		private Expression ParseReturn() {
			this.Consume(TokenType.RETURN);

			// Tail recursion
			if (this.Match(TokenType.TAIL_REC)) {
				List<Expression> args = new List<Expression>();

				while (this.IsValidToken()) {
					args.Add(this.Dot());
				}

				Expression res = new Tailrec(args, this.file, this.line);
				return res;
			}

			Expression @return =
				new Return(!this.IsValidToken() ? UnitLiteral.Instance : this.Expression());

			return this.Match(TokenType.WHEN)
				? new Condition(this.Expression(), @return, UnitLiteral.Instance)
				: @return;
		}

		private Expression ParseWithAnnotation() {
			this.Consume(TokenType.ANNOTATION);
			String name = this.Consume(TokenType.WORD).Text;
			this.Match(TokenType.EOC);

			if (name == "intern") {
				Expression innerExpression = this.Expression();

				return new InternWithBindingDeclaration(innerExpression);
			}

			throw new LumenException($"unknown annotation {name}", this.line, this.file);
		}

		private Expression ParseTry() {
			this.Consume(TokenType.TRY);
			this.Match(TokenType.COLON);

			Expression tryBody = this.Expression();

			this.Match(TokenType.EOC);

			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();
			Expression ensure = null;

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

				ensure = this.Expression();
			}

			return new ExceptionHandling(tryBody, patterns, ensure);
		}

		private Expression ParseUse() {
			this.Consume(TokenType.USE);
			Int32 savedLine = this.line;

			IPattern pattern = this.ParsePattern();

			this.Consume(TokenType.EQUALS);

			Expression assignable = this.Expression();
			this.Match(TokenType.COLON);

			Expression body = this.Expression();

			return new UseStatement(pattern, assignable, body, this.file, savedLine);
		}

		private Expression ParseTypeDeclaration() {
			this.Consume(TokenType.TYPE);
			// type typeName
			String typeName = this.Consume(TokenType.WORD).Text;
			// type typeName =
			this.Consume(TokenType.EQUALS);

			Token currentToken = this.GetToken(0);
			if (currentToken.Type == TokenType.WORD) {
				if (currentToken.Text == "variant") {
					this.Match(currentToken.Type);
					return this.ParseVariantDeclaration(typeName);
				} else if (currentToken.Text == "exception") {
					this.Match(currentToken.Type);
					return this.ParseExceptionDeclaration(typeName);
				} else if (currentToken.Text == "class") {
					this.Match(currentToken.Type);
					return this.ParseClassDeclaration(typeName);
				} else if (currentToken.Text == "alias") {
					this.Match(currentToken.Type);
					return this.ParseAliasDeclaration(typeName);
				}
			} else if (currentToken.Type == TokenType.FUN) {
				return this.ParseFunctionalTypeDeclaration(typeName);
			}

			return this.ParseVariantDeclaration(typeName);
		}

		private Expression ParseFunctionalTypeDeclaration(string typeName) {
			return new FunctionalTypeDeclaration(typeName, this.Primary());
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
					} else {
						members.Add(this.PrivateExpression());
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
					} else {
						members.Add(this.PrivateExpression());
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

			String typeParam = null;

			if (this.LookMatch(0, TokenType.WORD)) {
				typeParam = this.Consume(TokenType.WORD).Text;
			}

			if (this.Match(TokenType.BLOCK_START)) {
				while (!this.Match(TokenType.BLOCK_END) && !this.Match(TokenType.EOF)) {
					if (this.Match(TokenType.IMPLEMENTS)) {
						derivings.Add(this.Expression());
					} else {
						members.Add(this.PrivateExpression());
					}
					this.Match(TokenType.EOC);
				}
			}

			return new ClassDeclaration(typeName, typeParam, members, derivings);
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

				if (hasParens) {
					this.Consume(TokenType.PAREN_CLOSE);
				}
			}

			defaultConstructors.Add(new ConstructorMetadata(constructorName, paramters));
		}

		private Expression ParseMatch() {
			this.Consume(TokenType.MATCH);

			Expression matchedExpression = this.Expression();

			this.Match(TokenType.COLON);
			this.Match(TokenType.EOC);

			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			while (this.Match(TokenType.BAR)) {
				IPattern pattern = this.ParsePattern();
				this.Consume(TokenType.LAMBDA);

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

		private Boolean IsPatternStopToken() {
			return this.LookMatch(0, TokenType.EQUALS)
				|| this.LookMatch(0, TokenType.COLLECTION_CLOSE)
				|| this.LookMatch(0, TokenType.PAREN_CLOSE)
				|| this.LookMatch(0, TokenType.LAMBDA)
				|| this.LookMatch(0, TokenType.BAR)
				|| this.LookMatch(0, TokenType.BLOCK_START)
				|| this.LookMatch(0, TokenType.COLON);
		}

		private IPattern ParsePattern() {
			IPattern result = null;

			if (this.Match(TokenType.NOT)) {
				return new NotPattern(this.ParsePattern());
			}

			if (this.Match(TokenType.ACTIVE_PATTERN_OPEN)) {
				Expression activePattern = this.Expression();
				this.Consume(TokenType.ACTIVE_PATTERN_CLOSE);

				List<IPattern> subpatterns = new List<IPattern>();

				while (!this.LookMatch(0, TokenType.EQUALS)
					&& !this.LookMatch(0, TokenType.COLLECTION_CLOSE)
					&& !this.LookMatch(0, TokenType.PAREN_CLOSE)
					&& !this.LookMatch(0, TokenType.LAMBDA)
					&& !this.LookMatch(0, TokenType.BAR)
					&& !this.LookMatch(0, TokenType.BLOCK_START)) {
					subpatterns.Add(this.ParsePattern());
				}

				return new ActivePattern(activePattern, subpatterns);
			}

			if (this.LookMatch(0, TokenType.DOT2) || this.LookMatch(1, TokenType.DOT3)) {
				Boolean isInclusive = this.Match(TokenType.DOT3);

				result = !this.IsValidRangeToken()
					? new RangePattern(result, null, isInclusive)
					: new RangePattern(result, this.ParsePattern(), isInclusive);
			}

			if (this.Match(TokenType.PAREN_OPEN)) {
				result = this.ParsePattern();

				List<IPattern> subpatterns = new List<IPattern> { result };
				if (this.Match(TokenType.SPLIT)) {
					while (!this.Match(TokenType.PAREN_CLOSE)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new SeqPattern(subpatterns);
				}

				this.Match(TokenType.PAREN_CLOSE);
			} else if (this.Match(TokenType.VOID)) {
				result = UnitPattern.Instance;
			} else if (this.Match(TokenType.ARRAY_OPEN)) {
				if (this.Match(TokenType.ARRAY_CLOSE)) {
					result = EmptyArrayPattern.Instance;
				} else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.ARRAY_CLOSE)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new ArrayPattern(subpatterns);
				}
			} else if (this.Match(TokenType.LIST_OPEN)) {
				if (this.Match(TokenType.COLLECTION_CLOSE)) {
					result = EmptyListPattern.Instance;
				} else {
					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.Match(TokenType.COLLECTION_CLOSE)) {
						subpatterns.Add(this.ParsePattern());
						this.Match(TokenType.SPLIT);
					}

					result = new ListPattern(subpatterns);
				}
			} else if (this.LookMatch(0, TokenType.NUMBER)) {
				result = new ValuePattern(new Number(Double.Parse(this.Consume(TokenType.NUMBER).Text)));
			} else if (this.LookMatch(0, TokenType.TEXT)) {
				result = new ValuePattern(new Text(this.Consume(TokenType.TEXT).Text));
			} else if (this.LookMatch(0, TokenType.WORD)) {
				String name = this.Consume(TokenType.WORD).Text;

				if (name == "_") {
					result = DiscardPattern.Instance;
				} else if (Char.IsLower(name[0]) && !this.LookMatch(0, TokenType.DOT)) {
					result = new NamePattern(name);
				} else {
					Expression constructor = new IdExpression(name, this.file, this.line);
					while (this.Match(TokenType.DOT)) {
						String fname = this.Consume(TokenType.WORD).Text;
						constructor = new DotOperator(constructor, fname, this.file, this.line);
					}

					List<IPattern> subpatterns = new List<IPattern>();

					while (!this.IsPatternStopToken()) {
						subpatterns.Add(this.ParsePattern());
					}

					result = new DeconstructPattern(constructor, subpatterns);
				}
			}

			if (result is ValuePattern) {
				if (this.LookMatch(0, TokenType.DOT2) || this.LookMatch(1, TokenType.DOT3)) {
					Boolean isInclusive = this.Match(TokenType.DOT3);

					result = !this.IsValidRangeToken()
						? new RangePattern(result, null, isInclusive)
						: new RangePattern(result, this.ParsePattern(), isInclusive);
				}
			}

			if (this.Match(TokenType.COLON2)) {
				result = new HeadTailPattern(result, this.ParsePattern());
			}

			if (this.Match(TokenType.COLON)) {
				List<Expression> exps = new List<Expression>();

				do {
					if (this.Match(TokenType.NOT)) {
						exps.Add(this.Dot());
						result = new NotPattern(new TypePattern(result, exps));
						exps = new List<Expression>();
					} else {
						exps.Add(this.Dot());
					}
				} while (this.Match(TokenType.SPLIT));

				if (exps.Count != 0) {
					result = new TypePattern(result, exps);
				}
			}

			if (this.Match(TokenType.AS)) {
				result = new AsPattern(result, this.Consume(TokenType.WORD).Text);
			}

			if (this.Match(TokenType.WHEN)) {
				result = new WhenPattern(result, this.Expression());
			}

			while (this.Match(TokenType.BAR)) {
				result = new OrPattern(result, this.ParsePattern());
			}

			return result;
		}

		// TODO
		private Expression ParseImport() {
			this.Consume(TokenType.IMPORT);

			List<String> ParseImportPath() {
				List<String> importPath = new List<String>();

				importPath.Add(this.Consume(TokenType.WORD).Text);
				while (this.Match(TokenType.DOT)) {
					importPath.Add(this.Consume(TokenType.WORD).Text);
				}

				return importPath;
			}

			String mainAlias = null;

			List<(String name, String alias)> importedNames = new List<(String name, String alias)>();

			// import x, || import x from
			if (this.LookMatch(1, TokenType.SPLIT) || this.LookMatch(3, TokenType.SPLIT) 
				|| this.LookMatch(1, TokenType.FROM) || this.LookMatch(3, TokenType.FROM)) {
				do {
					String name = this.Consume(TokenType.WORD).Text;
					String alias = null;
					if (this.Match(TokenType.AS)) {
						alias = this.Consume(TokenType.WORD).Text;
					}
					importedNames.Add((name, alias));
				} while (this.Match(TokenType.SPLIT));

				this.Consume(TokenType.FROM);
			}

			List<String> importPath = ParseImportPath();


			if (this.Match(TokenType.AS)) {
				if (importedNames.Count > 0) {
					throw new LumenException("expression \"from ... as ...\" is forbidden", this.line, this.file);
				}

				mainAlias = this.Consume(TokenType.WORD).Text;
			}

			return new Import(importPath, importedNames, mainAlias, this.file, this.line);
		}

		private Expression ParseModule() {
			this.Consume(TokenType.MODULE);

			String moduleName = this.Consume(TokenType.WORD).Text;
			this.Consume(TokenType.EQUALS);

			List<Expression> declarations = new List<Expression>();

			if (!this.Match(TokenType.BLOCK_START)) {
				throw new LumenException("module declaration require module body", this.line, this.file);
			}

			while (!this.Match(TokenType.BLOCK_END)) {
				declarations.Add(this.PrivateExpression());
				this.Match(TokenType.EOC);
			}

			return new ModuleDeclaration(moduleName, declarations);
		}

		private Expression ParseDeclaration() {
			this.Consume(TokenType.LET);
			Int32 line = this.line;

			Boolean isRec = false;

			Token currentToken = this.GetToken(0);

			if (currentToken.Type == TokenType.WORD && currentToken.Text == "rec") {
				this.Consume(TokenType.WORD);
				isRec = true;
			}

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
							typeArgName.AddImplements(this.Bitwise());
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

			if (isRec) {
				BindingDeclaration binding = new BindingDeclaration(new NamePattern(name),
						new IdExpression("rec", this.file, line), this.file, line);

				if (body is Block block) {
					block.expressions.Insert(0, binding);
				} else {
					body = new Block(new List<Expression> {
						binding, body
					});
				}
			}

			return new FunctionDeclaration(name, arguments, body, line, this.file);
		}

		private ForCycle ParseFor() {
			this.Consume(TokenType.FOR);
			IPattern pattern = this.ParsePattern();

			this.Consume(TokenType.IN);

			Expression container = this.Expression();

			String cycleName = this.Match(TokenType.AS) ? this.Consume(TokenType.WORD).Text : null;

			this.Match(TokenType.COLON);

			Expression body = this.Expression();

			return new ForCycle(cycleName, pattern, container, body);
		}

		private Expression ParseIf() {
			this.Consume(TokenType.IF);
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

		private Expression ParseLoop() {
			this.Consume(TokenType.LOOP);
			String cycleName = this.Match(TokenType.AS) ? this.Consume(TokenType.WORD).Text : null;
			this.Match(TokenType.EOC);
			if (this.LookMatch(0, TokenType.BAR)) {
				List<(Expression, Expression)> guardAndBodies = new List<(Expression, Expression)>();
				while (this.Match(TokenType.BAR)) {
					Expression guard = this.Expression();
					this.Match(TokenType.LAMBDA);
					Expression body = this.Expression();
					guardAndBodies.Add((guard, body));
				}

				return new DijkstraLoop(cycleName, guardAndBodies);
			}

			this.Match(TokenType.COLON);
			return new Loop(cycleName, this.Expression());
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

			if (this.Match(TokenType.IS)) {
				Int32 lineNumber = this.line;

				if (this.Match(TokenType.NOT)) {
					return new BinaryOperator(new IsOperator(exp, this.FunctionalOperators(), lineNumber, this.file), UnitLiteral.Instance, Constants.NOT, lineNumber, this.file);
				}

				return new IsOperator(exp, this.FunctionalOperators(), lineNumber, this.file);
			}

			if (this.Match(TokenType.ASSIGN)) {
				Int32 lineNumber = this.line;
				return new BinaryOperator(exp, this.Expression(), "<-", lineNumber, this.file);
			}

			return exp;
		}

		private Expression FunctionalOperators() {
			Expression expr = this.Range();

			while (expr is not Block) {
				Token current = this.GetToken(0);

				if (current.Type == TokenType.MIDDLE_PRIORITY_RIGTH) {
					this.Match(current.Type);

					expr = new BinaryOperator(expr, this.Range(), current.Text, this.line, this.file);
					continue;
				}

				if (this.Match(TokenType.FORWARD_PIPE)) {
					expr = new Applicate(this.Range(), new List<Expression> { expr }, this.file, this.line);
					continue;
				}

				if (this.Match(TokenType.BACKWARD_PIPE)) {
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
				|| this.LookMatch(0, TokenType.SEQ_OPEN)
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
					} else if (this.LookMatch(0, TokenType.LIST_OPEN)) {
						res = this.Slice(res);
					} else if (this.Match(TokenType.TYPE)) {
						res = new TypeOf(res, this.file, this.line);
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

				while (!this.Match(TokenType.COLLECTION_CLOSE)) {
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

				ForCycle result = this.ParseFor();

				this.Match(TokenType.COLLECTION_CLOSE);

				return new ListGenerator(new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body));
			}

			if (this.LookMatch(0, TokenType.ARRAY_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.ARRAY_OPEN);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.COLLECTION_CLOSE);

				return new ArrayGenerator(new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body));
			}

			// Sequence generators
			if (this.LookMatch(0, TokenType.PAREN_OPEN) && this.LookMatch(1, TokenType.FOR)) {
				this.Match(TokenType.PAREN_OPEN);

				ForCycle result = this.ParseFor();

				this.Match(TokenType.PAREN_CLOSE);

				return new SequenceGenerator(result.cycleName, result.pattern, result.expression, result.body);
			}

			// Lists
			if (this.Match(TokenType.LIST_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.COLLECTION_CLOSE)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_LIST_LITERAL, line: Current.Line, fileName: this.file);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.COLLECTION_CLOSE)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new ListE(elements);
			}

			// Arrays
			if (this.Match(TokenType.ARRAY_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.COLLECTION_CLOSE)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_ARRAY_LITERAL, line: Current.Line, fileName: this.file);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.COLLECTION_CLOSE)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new ArrayLiteral(elements);
			}

			// Sequences
			if (this.Match(TokenType.SEQ_OPEN)) {
				List<Expression> elements = new List<Expression>();

				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				while (!this.Match(TokenType.COLLECTION_CLOSE)) {
					if (this.Match(TokenType.EOF)) {
						throw new LumenException(Exceptions.UNCLOSED_ARRAY_LITERAL, line: Current.Line, fileName: this.file);
					}

					elements.Add(this.Expression());
					this.MatchAny(TokenType.SPLIT, TokenType.EOC);

					if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.COLLECTION_CLOSE)) {
						this.Match(TokenType.BLOCK_END);
					}
				}

				return new SeqLiteral(elements);
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

			if (this.Match(TokenType.YIELD)) {
				if (this.Match(TokenType.FROM)) {
					return new YieldFrom(this.Expression());

					/*return new ForCycle(null, new NamePattern("<>i"), this.Expression(), 
						new Yield(new IdExpression("<>i", this.file, line)));*/
				}

				if (this.LookMatch(0, TokenType.BLOCK_END) || this.LookMatch(0, TokenType.PAREN_CLOSE) || this.Match(TokenType.EOC) || this.Match(TokenType.EOF)) {
					return new Yield(UnitLiteral.Instance);
				}

				return new Yield(this.Expression());
			}

			if (this.Match(TokenType.RAISE)) {
				Int32 line = this.line;

				Expression inraise = !this.IsValidToken() ? UnitLiteral.Instance : this.Expression();

				Expression from = this.Match(TokenType.FROM) ? this.Expression() : null;

				Expression raise = new Raise(inraise, from, this.file, line);

				if (this.Match(TokenType.WHEN)) {
					return new Condition(this.Expression(), raise, UnitLiteral.Instance);
				}

				return raise;
			}

			// Number literals
			if (this.Match(TokenType.NUMBER)) {
				return new ValueLiteral(Double.Parse(Current.Text, System.Globalization.NumberFormatInfo.InvariantInfo));
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
								typeArgName.AddImplements(this.Bitwise());
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
				Boolean shouldExpectEndToken = this.Match(TokenType.BLOCK_START);

				Expression result = this.Expression();

				if (this.Match(TokenType.SPLIT) || (shouldExpectEndToken && this.Match(TokenType.EOC))) {
					List<Expression> elements = new List<Expression> { result };

					while (!this.Match(TokenType.PAREN_CLOSE)) {
						if (this.Match(TokenType.EOF)) {
							throw new LumenException(Exceptions.UNCLOSED_ARRAY_LITERAL, line: Current.Line, fileName: this.file);
						}

						elements.Add(this.Expression());
						this.MatchAny(TokenType.SPLIT, TokenType.EOC);

						if (shouldExpectEndToken && this.LookMatch(0, TokenType.BLOCK_END) && this.LookMatch(1, TokenType.PAREN_CLOSE)) {
							this.Match(TokenType.BLOCK_END);
						}
					}

					return new SeqLiteral(elements);
				}

				this.Consume(TokenType.PAREN_CLOSE);
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
			Block block = new Block();
			this.Match(TokenType.BLOCK_START);

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