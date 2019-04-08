using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace ldoc {
    internal sealed class Parser {
        private readonly List<Token> tokens;
        private readonly Int32 size;
        private readonly String fileName;
        private Int32 position;
        private Int32 line;

        internal Parser(List<Token> tokens, String fileName) {
            this.fileName = fileName;
            this.tokens = tokens;
            this.size = tokens.Count;
            this.line = 0;
        }

        internal List<Expression> Parsing() {
            List<Expression> result = new List<Expression>();

            while (!this.Match(TokenType.EOF)) {
                result.Add(this.Expression());
                this.Match(TokenType.EOC);
            }

            return result;
        }

        private Expression Expression() {
            StringBuilder sb = new StringBuilder();

            while (this.LookMatch(0, TokenType.DOC)) {
                sb.Append(this.Consume(TokenType.DOC).Text);
            }

            if(sb.Length > 0) {
                return new DocComment(this.Expression(), sb.ToString());
            }

            // [<attribute1; attribute2>]
            if (this.Match(TokenType.ATTRIBUTE_OPEN)) {
                Int32 line = this.line;

                List<Expression> attributes = new List<Expression>();

                while (!this.Match(TokenType.ATTRIBUTE_CLOSE)) {
                    attributes.Add(this.Expression());
                    this.Match(TokenType.EOC);

                    if (this.Match(TokenType.EOF)) {
                        throw new LumenException("attribute not closed", line: line, fileName: this.fileName);
                    }
                }

                this.Match(TokenType.EOC);

                Expression declaration = this.Expression();

                return new AttributeExpression(attributes, declaration);
            }

            if (this.Match(TokenType.RETURN)) {
                if (this.Match(TokenType.EOC) || this.Match(TokenType.EOF)) {
                    return new Return(UnitExpression.Instance);
                }

                return new Return(this.Expression());
            } else if (this.Match(TokenType.MATCH)) {
                return this.ParseMatch();
            } else if (this.Match(TokenType.TYPE)) {
                return this.ParseTypeDeclaration();
            } else if (this.Match(TokenType.OPEN)) {
                return new OpenModule(this.Expression());
            } else if (this.Match(TokenType.REF)) {
                return this.ParseRef();
            } else if (this.Match(TokenType.IF)) {
                return this.ParseIf();
            } else if (this.Match(TokenType.MODULE)) {
                return this.ParseModule();
            } else if (this.Match(TokenType.WHILE)) {
                return this.ParseWhile();
            } else if (this.Match(TokenType.LET)) {
                return this.ParseDeclaration();
            } else if (this.Match(TokenType.FOR)) {
                return this.ParseFor();
            } else {
                return this.LogikOr();
            }
        }

        private Expression ParseTypeDeclaration() {
            Dictionary<String, List<String>> constructors = new Dictionary<String, List<String>>();

            String name = this.Consume(TokenType.WORD).Text;

            this.Match(TokenType.EQUALS);

            while (!this.LookMatch(0, TokenType.WITH) && !this.Match(TokenType.EOC)) {
                List<String> fields = new List<String>();
                String constructorName = this.Consume(TokenType.WORD).Text;
                while (!this.Match(TokenType.BAR) && !this.LookMatch(0, TokenType.EOC) && !this.LookMatch(0, TokenType.WITH)) {
                    fields.Add(this.Consume(TokenType.WORD).Text);
                }
                constructors.Add(constructorName, fields);
            }

            List<Expression> members = new List<Expression>();

            if (this.Match(TokenType.WITH)) {
                this.Match(TokenType.DO);
                while (!this.Match(TokenType.END)) {
                    members.Add(this.Expression());
                    this.Match(TokenType.EOC);
                }
            }

            return new TypeDeclarationE(name, constructors, members);
        }

        private Expression ParseMatch() {
            Expression matchedExpression = this.Expression();
            this.Match(TokenType.COLON);
            this.Match(TokenType.EOC);

            Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

            while (this.LookMatch(0, TokenType.BAR)) {
                this.Match(TokenType.BAR);

                IPattern pattern = this.ParsePattern();
                this.Match(TokenType.EQUALS);

                Expression body = this.Expression();
                this.Match(TokenType.EOC);

                patterns.Add(pattern, body);
            }

            return new MatchE(matchedExpression, patterns);
        }

        private IPattern ParsePattern() {
            if (this.Match(TokenType.VOID)) {
                return new UnitPattern();
            }

            if (this.Match(TokenType.LBRACKET)) {
                if (this.Match(TokenType.RBRACKET)) {
                    return new EmptyListPattern();
                }

                List<IPattern> expressions = new List<IPattern>();
                while (!this.Match(TokenType.RBRACKET) && !this.Match(TokenType.EOF)) {
                    expressions.Add(this.ParsePattern());
                    this.Match(TokenType.EOC);
                }
                return new ListPattern(expressions);
            }

            if (this.LookMatch(0, TokenType.NUMBER)) {
                return new ValuePattern(this.Consume(TokenType.NUMBER).Text);
            }

            if (this.Match(TokenType.LPAREN)) {
                IPattern ptrn = this.ParsePattern();
                this.Match(TokenType.RPAREN);
                return ptrn;
            }

            if (this.LookMatch(0, TokenType.WORD)) {
                String id = this.Consume(TokenType.WORD).Text;

                if (id == "_" && this.LookMatch(0, TokenType.EQUALS)) {
                    return new DiscordPattern();
                }

                if (this.Match(TokenType.COLONCOLON)) {
                    String ns = this.Consume(TokenType.WORD).Text;
                    return new HeadTailPattern(id, ns);
                }

                if (new[] { "true", "false" }.Contains(id)) {
                    return new VariablePattern(id);
                }

                List<IPattern> subPatterns = new List<IPattern>();
                while (!this.LookMatch(0, TokenType.EQUALS) && !this.Match(TokenType.EOF) && !this.Match(TokenType.END)) {
                    if (this.LookMatch(0, TokenType.WORD)) {
                        subPatterns.Add(new NamePattern(this.Consume(TokenType.WORD).Text));
                    }

                    if (this.LookMatch(0, TokenType.NUMBER)) {
                        subPatterns.Add(new ValuePattern(this.Consume(TokenType.NUMBER).Text));
                    }
                }

                return new TypePattern(id, subPatterns);
            }

            throw new LumenException("unknown pattern", line: this.line, fileName: this.fileName);
        }

        private IPattern ParseArgPattern() {
            IPattern result = null;

            if (this.Match(TokenType.VOID)) {
                result = new UnitPattern();
            } else if (this.Match(TokenType.LBRACKET)) {
                if (this.Match(TokenType.RBRACKET)) {
                    result = new EmptyListPattern();
                } else {

                    List<IPattern> expressions = new List<IPattern>();
                    while (!this.Match(TokenType.RBRACKET) || this.Match(TokenType.EOF)) {
                        expressions.Add(this.ParseArgPattern());
                        this.Match(TokenType.EOC);
                    }

                    result = new ListPattern(expressions);
                }
            } else if (this.LookMatch(0, TokenType.NUMBER)) {
                result = new ValuePattern(this.Consume(TokenType.NUMBER).Text);
            } else if (this.Match(TokenType.LPAREN)) {
                String id = this.Consume(TokenType.WORD).Text;

                List<IPattern> subPatterns = new List<IPattern>();
                while (!this.LookMatch(0, TokenType.RPAREN) && !this.Match(TokenType.EOF)) {
                    subPatterns.Add(this.ParseArgPattern());
                }

                this.Match(TokenType.RPAREN);

                result = new TypePattern(id, subPatterns);
            } else if (this.LookMatch(0, TokenType.WORD)) {
                String id = this.Consume(TokenType.WORD).Text;

                if (id == "_" && this.LookMatch(0, TokenType.EQUALS)) {
                    result = new DiscordPattern();
                } else if (this.Match(TokenType.COLONCOLON)) {
                    String ns = this.Consume(TokenType.WORD).Text;
                    result = new HeadTailPattern(id, ns);
                } else if (new[] { "true", "false" }.Contains(id)) {
                    result = new VariablePattern(id);
                } else {
                    result = new NamePattern(id);
                }
            }

            if (this.Match(TokenType.AS)) {
                String ide = this.Consume(TokenType.WORD).Text;
                result = new AsPattern(result, ide);
            }

            while (this.Match(TokenType.BAR) && !this.Match(TokenType.EOF)) {
                IPattern second = this.ParseArgPattern();
                result = new OrPattern(result, second);
            }

            if (result == null) {
                throw new LumenException("unknown pattern", line: this.line, fileName: this.fileName);
            }

            return result;
        }

        private Expression ParseRef() {
            if (this.LookMatch(0, TokenType.TEXT)) {
                return new Reference(this.Consume(TokenType.TEXT).Text, this.fileName, this.line);
            }

            StringBuilder pathBuilder = new StringBuilder(this.Consume(TokenType.WORD).Text);

            while (this.Match(TokenType.DOT)) {
                pathBuilder.Append("\\").Append(this.Consume(TokenType.WORD).Text);
            }

            return new Reference(pathBuilder.ToString(), this.fileName, this.line);
        }

        private Expression ParseVariableDeclaration(String id) {
            Expression type;

            Expression expression = UnitExpression.Instance;

            type = null;

            if (this.Match(TokenType.COLON)) {
                type = this.ParseType();
            }

            if (this.Match(TokenType.EQUALS)) {
                expression = this.Expression();
            }

            return new VariableDeclaration(id, type, expression, this.fileName, this.line);
        }

        private Expression ParseModule() {
            String name = this.Consume(TokenType.WORD).Text;
            List<Expression> declarations = new List<Expression>();
            this.Match(TokenType.EQUALS);
            this.Match(TokenType.DO);
            while (!this.Match(TokenType.END)) {
                declarations.Add(this.Expression());
                this.Match(TokenType.EOC);
            }
            return new ModuleDeclaration(name, declarations);
        }

        #region
        /// <summary> Declaration with let </summary>
        private Expression ParseDeclaration() {
            Int32 line = this.line;

            String name;
            switch (this.GetToken(0).Type) {
                case TokenType.WORD:
                    name = this.Consume(TokenType.WORD).Text;
                    break;
                default:
                    name = this.GetToken(0).Text;
                    this.position++;
                    break;
            }

            List<IPattern> arguments;
            // It's variable
            if (this.LookMatch(0, TokenType.EQUALS)) {
                return this.ParseVariableDeclaration(name);
            }

            this.Match(TokenType.EOC);
            arguments = new List<IPattern>();
            while (!this.LookMatch(0, TokenType.EQUALS)) {
                arguments.Add(this.ParseArgPattern());
            }

            this.Match(TokenType.EQUALS);

            Expression body = this.Expression();

            return new FunctionDeclaration(name, arguments, body, line, this.fileName);
        }

        /// <summary> For cycle </summary>
        private Expression ParseFor() {
            Boolean varIsDeclared = this.Match(TokenType.LET);
            String varName = this.Consume(TokenType.WORD).Text;

            this.Consume(TokenType.IN);

            Expression container = this.Expression();

            this.Match(TokenType.COLON);

            Expression body = this.Expression();

            return new ForCycle(varName, null, varIsDeclared, container, body);
        }

        private Expression AloneORBlock() {
            BlockE Block = new BlockE();
            this.Match(TokenType.DO);

            Dictionary<String, Expression> items = new Dictionary<String, Expression>();

            if (this.LookMatch(1, TokenType.COLON) || this.LookMatch(1, TokenType.SPLIT)) {
                while (!this.Match(TokenType.END)) {
                    String name = this.Consume(TokenType.WORD).Text;
                    Expression exp;
                    if (this.Match(TokenType.COLON)) {
                        exp = this.Expression();
                    } else {
                        exp = UnitExpression.Instance;
                    }
                    items.Add(name, exp);
                    this.Match(TokenType.EOC);
                }

                // return new ObjectE(items);
            }

            Int32 line = this.line;
            while (!this.Match(TokenType.END)) {
                if (this.Match(TokenType.EOF)) {
                    throw new LumenException("пропущена закрывающая фигурная скобка") {
                        line = line,
                        file = this.fileName
                    };
                }

                Block.Add(this.Expression());
                this.Match(TokenType.EOC);
            }

            // Optimization
            if (Block.expressions.Count == 1) {
                if (Block.expressions[0] is VariableDeclaration vd) {
                    return vd.assignableExpression;
                } else {
                    return Block.expressions[0];
                }
            }

            return Block;
        }

        private Expression ParseIf() {
            Expression condition = this.Expression();

            this.Match(TokenType.COLON);

            Expression trueBody = this.Expression();

            Expression falseBody = UnitExpression.Instance;

            this.Match(TokenType.EOC);

            if (this.Match(TokenType.ELSE)) {
                this.Match(TokenType.COLON);
                falseBody = this.Expression();
            }

            return new ConditionE(condition, trueBody, falseBody);
        }

        private Expression ParseWhile() {
            Expression condition = this.Expression();
            this.Match(TokenType.COLON);
            Expression body = this.Expression();
            return new WhileExpression(condition, body);
        }

        #endregion

        private Expression LogikOr() {
            Expression result = this.LogikXor();

            while (true) {
                if (this.Match(TokenType.OR)) {
                    result = new BinaryExpression(result, this.LogikXor(), Op.OR, this.line, this.fileName);
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
                    result = new BinaryExpression(result, this.LogikAnd(), Op.XOR, this.line, this.fileName);
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
                    result = new BinaryExpression(result, this.Assigment(), Op.AND, this.line, this.fileName);
                    continue;
                }
                break;
            }
            return result;
        }

        private Expression Assigment() {
            if (this.LookMatch(0, TokenType.WORD) && this.LookMatch(1, TokenType.ASSIGN)) {
                String id = this.Consume(TokenType.WORD).Text;
                Int32 line = this.line;
                this.Consume(TokenType.ASSIGN);
                return new Assigment(id, this.Expression(), line, this.fileName);
            }

            return this.Is();
        }

        private Expression Is() {
            Expression expr = this.Range();

            while (this.Match(TokenType.FPIPE)) {
                expr = new Applicate(this.Range(), new List<Expression> { expr }, this.line, this.fileName);
            }

            if (this.Match(TokenType.IS)) {
                if (this.Match(TokenType.NOT)) {
                    return new BinaryExpression(new IsExpression(expr, this.ParseType()), null, "@not", this.line, this.fileName);
                } else {
                    return new IsExpression(expr, this.ParseType());
                }
            }

            return expr;
        }

        private Expression Range() {
            Expression result = this.Equality();

            if (this.Match(TokenType.DOTDOT)) {
                return new BinaryExpression(result, this.Equality(), Op.RANGE_EXCLUSIVE, this.line, this.fileName);
            }

            if (this.Match(TokenType.DOTDOTDOT)) {
                return new BinaryExpression(result, this.Equality(), Op.RANGE_INCLUSIVE, this.line, this.fileName);
            }

            return result;
        }

        private Expression Equality() {
            Expression result = this.Conditional();
            if (this.Match(TokenType.EQUALS)) {
                Int32 line = this.GetToken(-1).Line;
                result = new BinaryExpression(result, this.Conditional(), Op.EQUALS, line, this.fileName);
                if (this.Match(TokenType.EQUALS)) {
                    line = this.GetToken(-1).Line;
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)result).expressionTwo, this.BitwiseXor(), Op.EQUALS, line, this.fileName), Op.AND, line, this.fileName);
                }
                while (this.Match(TokenType.EQUALS)) {
                    line = this.GetToken(-1).Line;
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)((BinaryExpression)result).expressionTwo).expressionTwo, this.BitwiseXor(), Op.EQUALS, line, this.fileName), Op.AND, line, this.fileName);
                }
            }

            if (this.Match(TokenType.EQMATCH)) {
                result = new BinaryExpression(result, this.Conditional(), Op.MATCH, this.line, this.fileName);
                if (this.Match(TokenType.EQMATCH)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)result).expressionTwo, this.BitwiseXor(), "==", this.line, this.fileName), "and", this.line, this.fileName);
                }

                while (this.Match(TokenType.EQMATCH)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)((BinaryExpression)result).expressionTwo).expressionTwo, this.BitwiseXor(), "==", this.line, this.fileName), "and", this.line, this.fileName);
                }
            }

            if (this.Match(TokenType.EQNOTMATCH)) {
                result = new BinaryExpression(result, this.Conditional(), "!~", this.line, this.fileName);
                if (this.Match(TokenType.EQNOTMATCH)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)result).expressionTwo, this.BitwiseXor(), "!~", this.line, this.fileName), "and", this.line, this.fileName);
                }

                while (this.Match(TokenType.EQNOTMATCH)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)((BinaryExpression)result).expressionTwo).expressionTwo, this.BitwiseXor(), "!~", this.line, this.fileName), "and", this.line, this.fileName);
                }
            }

            if (this.Match(TokenType.NOT_EQUALS)) {
                result = new BinaryExpression(result, this.Conditional(), Op.NOT_EQL, this.line, this.fileName);
                if (this.Match(TokenType.NOT_EQUALS)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)result).expressionTwo, this.BitwiseXor(), "!=", this.line, this.fileName), "and", this.line, this.fileName);
                }

                while (this.Match(TokenType.NOT_EQUALS)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)((BinaryExpression)result).expressionTwo).expressionTwo, this.BitwiseXor(), "!=", this.line, this.fileName), "and", this.line, this.fileName);
                }
            }

            if (this.Match(TokenType.SHIP)) {
                result = new BinaryExpression(result, this.Conditional(), Op.SHIP, this.line, this.fileName);
                if (this.Match(TokenType.SHIP)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)result).expressionTwo, this.BitwiseXor(), "<=>", this.line, this.fileName), "and", this.line, this.fileName);
                }

                while (this.Match(TokenType.SHIP)) {
                    result = new BinaryExpression(result, new BinaryExpression(((BinaryExpression)((BinaryExpression)result).expressionTwo).expressionTwo, this.BitwiseXor(), "<=>", this.line, this.fileName), "and", this.line, this.fileName);
                }
            }

            return result;
        }

        private Expression Conditional() {
            Expression expr = this.BitwiseXor();

            while (true) {
                if (this.Match(TokenType.LT)) {
                    expr = new BinaryExpression(expr, this.BitwiseXor(), Op.LT, this.line, this.fileName);
                    if (this.Match(TokenType.LT)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)expr).expressionTwo, this.BitwiseXor(), "<", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    while (this.Match(TokenType.LT)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)((BinaryExpression)expr).expressionTwo).expressionTwo, this.BitwiseXor(), "<", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    continue;
                }

                if (this.Match(TokenType.LTEQ)) {
                    expr = new BinaryExpression(expr, this.BitwiseXor(), Op.LTEQ, this.line, this.fileName);
                    if (this.Match(TokenType.LTEQ)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)expr).expressionTwo, this.BitwiseXor(), "<=", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    while (this.Match(TokenType.LTEQ)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)((BinaryExpression)expr).expressionTwo).expressionTwo, this.BitwiseXor(), "<=", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    continue;
                }

                if (this.Match(TokenType.GT)) {
                    expr = new BinaryExpression(expr, this.BitwiseXor(), Op.GT, this.line, this.fileName);
                    if (this.Match(TokenType.GT)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)expr).expressionTwo, this.BitwiseXor(), ">", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    while (this.Match(TokenType.GT)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)((BinaryExpression)expr).expressionTwo).expressionTwo, this.BitwiseXor(), ">", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    continue;
                }

                if (this.Match(TokenType.GTEQ)) {
                    expr = new BinaryExpression(expr, this.BitwiseXor(), Op.GTEQ, this.line, this.fileName);
                    if (this.Match(TokenType.GTEQ)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)expr).expressionTwo, this.BitwiseXor(), ">=", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    while (this.Match(TokenType.GTEQ)) {
                        expr = new BinaryExpression(expr, new BinaryExpression(((BinaryExpression)((BinaryExpression)expr).expressionTwo).expressionTwo, this.BitwiseXor(), ">=", this.line, this.fileName), "and", this.line, this.fileName);
                    }

                    continue;
                }
                break;
            }
            return expr;
        }

        private Expression BitwiseXor() {
            Expression expr = this.BitwiseOr();

            if (this.Match(TokenType.BXOR)) {
                Int32 line = this.GetToken(-1).Line;
                expr = new BinaryExpression(expr, this.BitwiseOr(), Op.UNARY_XOR, line, this.fileName);
            }

            return expr;
        }

        private Expression BitwiseOr() {
            Expression expr = this.BitwiseAnd();
            return expr;
        }

        private Expression BitwiseAnd() {
            Expression expr = this.Bitwise();
            if (this.Match(TokenType.AMP)) {
                Int32 line = this.GetToken(-1).Line;
                expr = new BinaryExpression(expr, this.Bitwise(), Op.BAND, line, this.fileName);
            }
            return expr;
        }

        private Expression Bitwise() {
            // Вычисляем выражение
            Expression expr = this.Additive();
            while (true) {
                if (this.Match(TokenType.BLEFT)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Additive(), Op.LSH, line, this.fileName);
                    continue;
                }
                if (this.Match(TokenType.BRIGTH)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Additive(), Op.RSH, line, this.fileName);
                    continue;
                }
                break;
            }
            return expr;
        }

        private Expression Additive() {
            // Умножение uber alles
            Expression expr = this.Multiplicate();
            Boolean mem = this.allow;
            this.allow = true;
            while (true) {
                if (this.Match(TokenType.PLUS)) {
                    expr = new BinaryExpression(expr, this.Multiplicate(), Op.PLUS, this.line, this.fileName);
                    continue;
                }

                if (this.Match(TokenType.MINUS)) {
                    expr = new BinaryExpression(expr, this.Multiplicate(), Op.MINUS, this.line, this.fileName);
                    continue;
                }
                break;
            }
            this.allow = mem;
            return expr;
        }

        private Expression Multiplicate() {
            Expression expr = this.Exponentiation();
            Boolean mem = this.allow;
            this.allow = true;
            while (true) {
                if (this.Match(TokenType.STAR)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Unary(), Op.STAR, line, this.fileName);
                    continue;
                }

                if (this.Match(TokenType.SLASH)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Unary(), Op.SLASH, line, this.fileName);
                    continue;
                }

                if (this.Match(TokenType.MOD)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Unary(), Op.MOD, line, this.fileName);
                    continue;
                }
                break;
            }
            this.allow = mem;
            return expr;
        }

        private Expression Exponentiation() {
            Expression expr = this.Unary();

            while (true) {
                if (this.Match(TokenType.BXOR)) {
                    Int32 line = this.GetToken(-1).Line;
                    expr = new BinaryExpression(expr, this.Unary(), Op.POW, line, this.fileName);
                    continue;
                }

                break;
            }

            return expr;
        }

        private Expression Unary() {
            if (this.LookMatch(0, TokenType.DOTDOTDOT) && this.LookMatch(1, TokenType.WORD) && this.LookMatch(2, TokenType.LAMBDA)) {
                this.Match(TokenType.DOTDOTDOT);
                ArgumentMetadataGenerator arg = new ArgumentMetadataGenerator("*" + this.Consume(TokenType.WORD).Text, null, null);
                this.Match(TokenType.LAMBDA);
                return new AnonymeDefine(this.Expression());
            }

            if (this.Match(TokenType.DOTDOTDOT)) {
                return new SpreadE(this.Convertabli());
            }

            if (this.Match(TokenType.MINUS)) {
                return new BinaryExpression(this.Convertabli(), null, Op.UMINUS, this.line, this.fileName);
            }

            if (this.Match(TokenType.NOT)) {
                return new BinaryExpression(this.Convertabli(), null, Op.NOT, this.line, this.fileName);
            }

            if (this.Match(TokenType.PLUS)) {
                return new BinaryExpression(this.Convertabli(), null, Op.UPLUS, this.line, this.fileName);
            }

            if (this.Match(TokenType.STAR)) {
                return new BinaryExpression(this.Convertabli(), null, Op.USTAR, this.line, this.fileName);
            }

            /*if (Match(TokenType.SLASH)) {
				return new BinaryExpression(Convertabli(), null, "@/", this.line, this.fileName);
			}
			*/
            if (this.Match(TokenType.TILDE)) {
                return new BinaryExpression(this.Convertabli(), null, Op.BNOT, this.line, this.fileName);
            }

            if (this.Match(TokenType.BXOR)) {
                return new BinaryExpression(this.Convertabli(), null, Op.UNARY_XOR, this.line, this.fileName);
            }

            if (this.Match(TokenType.AMP)) {
                return new BinaryExpression(this.Convertabli(), null, Op.BAND, this.line, this.fileName);
            }

            return this.Convertabli();
        }

        private Expression Convertabli() {
            Expression result = this.Application();

            if (this.Match(TokenType.COLONCOLON)) {
                Expression right = this.Expression();

                result = new AddE(result, right);
            }

            return result;
        }

        Boolean allow = true;

        private Boolean StopToken() {
            return this.LookMatch(0, TokenType.LPAREN)
                || this.LookMatch(0, TokenType.BIG_NUMBER)
                || this.LookMatch(0, TokenType.BNUMBER)
                || this.LookMatch(0, TokenType.HARDNUMBER)
                || this.LookMatch(0, TokenType.LBRACKET)
                || this.LookMatch(0, TokenType.NUMBER)
                || this.LookMatch(0, TokenType.TEXT)
                || this.LookMatch(0, TokenType.VOID)
                || this.LookMatch(0, TokenType.WORD);
        }

        private Expression Application() {
            Expression res = this.Dot();

            if (res is IdExpression idExpression && idExpression.id == "_") {
                return res;
            }

            if (this.allow && this.StopToken()) {
                List<Expression> args = new List<Expression>();

                this.allow = false;
                while (this.StopToken()) {
                    args.Add(this.Dot());
                    this.Match(TokenType.SPLIT);
                }
                this.allow = true;
                res = new Applicate(res, args, this.line, this.fileName);
                this.Match(TokenType.EOC);
            }

            return res;
        }

        private Expression Slice(Expression inn = null) {
            Expression res = inn;
            while (this.LookMatch(0, TokenType.LBRACKET)) {
                res = this.Element(res);
                if (this.Match(TokenType.ASSIGN)) {
                    List<Expression> args = (res as DotApplicate).argumentsExperssions;
                    args.Add(this.Expression());
                    return new DotApplicate(new DotExpression(((res as DotApplicate).callable as DotExpression).expression, Op.SETI, this.fileName, this.line), args);
                }
            }

            if (this.LookMatch(0, TokenType.DOT)) {
                res = this.Dot(res);
            }

            return res;
        }

        private Expression Dot(Expression inn = null) {
            Expression res = inn ?? this.Primary();

            while (true) {
                Int32 line = this.line;
                if (this.Match(TokenType.DOT)) {
                    if (this.LookMatch(0, TokenType.WORD)) {
                        res = new DotExpression(res, this.Consume(TokenType.WORD).Text, this.fileName, line);
                    }

                    if (this.Match(TokenType.ASSIGN)) {
                        res = new DotAssigment((DotExpression)res, this.Expression(), this.fileName, this.line);
                    }

                    while (this.LookMatch(0, TokenType.LBRACKET)) {
                        res = this.Slice(res);
                    }

                    if (this.allow && this.StopToken()) {
                        List<Expression> args = new List<Expression>();

                        this.allow = false;
                        while (this.StopToken()) {
                            args.Add(this.Dot());
                        }
                        this.allow = true;
                        res = new DotApplicate(res, args);
                    }
                    continue;
                }
                break;
            }

            return res;
        }

        private Expression Primary() {
            Token Current = this.GetToken(0);

            if (this.Match(TokenType.WORD)) {
                if (this.Match(TokenType.LAMBDA)) {
                    ArgumentMetadataGenerator arg = new ArgumentMetadataGenerator(Current.Text, null, null);
                    Boolean mem = this.allow;
                    this.allow = true;
                    AnonymeDefine resx = new AnonymeDefine(this.Expression());
                    this.allow = mem;
                    return resx;
                }

                return new IdExpression(Current.Text, Current.Line, this.fileName);
            }

            if (this.Match(TokenType.LBRACKET)) {
                List<Expression> elements = new List<Expression>();
                while (!this.Match(TokenType.RBRACKET)) {
                    elements.Add(this.Expression());
                    this.Match(TokenType.EOC);
                }

                return new ListE(elements);
            }

            if (this.Match(TokenType.NUMBER)) {
                return new ValueE();
            }

            if (this.Match(TokenType.BIG_NUMBER)) {
                return new ValueE();
            }

            /* if (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.FOR)) {
                 this.Match(TokenType.LPAREN);
                 this.Match(TokenType.FOR);

                 this.Match(TokenType.VAR);
                 String varName = this.Consume(TokenType.WORD).Text;

                 this.Consume(TokenType.IN);

                 Expression Expressions = this.Expression();

                 this.Match(TokenType.COLON);

                 Expression Statement = this.Expression();

                 return new ListForGen(varName, Expressions, Statement);
             }*/

            // (x) =>
            if (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.WORD) && this.LookMatch(2, TokenType.RPAREN) && this.LookMatch(3, TokenType.LAMBDA)) {
                this.Match(TokenType.LPAREN);
                ArgumentMetadataGenerator arg = new ArgumentMetadataGenerator(this.Consume(TokenType.WORD).Text, null, null);
                this.Match(TokenType.RPAREN);
                if (this.Match(TokenType.LAMBDA)) {
                    Boolean mem = this.allow;
                    this.allow = true;
                    AnonymeDefine res = new AnonymeDefine(this.Expression());
                    this.allow = mem;
                    return res;
                }
            }

            // () ->
            if (this.LookMatch(0, TokenType.VOID) && this.LookMatch(1, TokenType.LAMBDA)) {
                this.Match(TokenType.VOID);
                this.Match(TokenType.LAMBDA);
                Boolean mem = this.allow;
                this.allow = true;
                AnonymeDefine res = new AnonymeDefine( this.Expression());
                this.allow = mem;
                return res;
            }

            // (x:
            if (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.WORD) && this.LookMatch(2, TokenType.COLON)) {
                this.Match(TokenType.LPAREN);
                Dictionary<String, Expression> flds = new Dictionary<String, Expression>();

                while (!this.Match(TokenType.RPAREN)) {
                    String name = this.Consume(TokenType.WORD).Text;
                    Expression exp;
                    if (this.Match(TokenType.COLON)) {
                        exp = this.Expression();
                    } else {
                        exp = UnitExpression.Instance;
                    }
                    flds.Add(name, exp);
                    this.Match(TokenType.EOC);
                }

                // return new ObjectE(flds);

                List<ArgumentMetadataGenerator> args = this.ParseArgs(TokenType.RPAREN);

                Expression returnedType = null;

                if (this.Match(TokenType.COLON)) {
                    returnedType = this.ParseType();
                    this.Match(TokenType.EOC);
                }

                this.Match(TokenType.LAMBDA);
                Boolean mem = this.allow;
                this.allow = true;
                AnonymeDefine res = new AnonymeDefine( this.Expression());
                this.allow = mem;
                return res;
            }

            // (x = 
            if (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.WORD) && this.LookMatch(2, TokenType.ASSIGN)) {
                this.Match(TokenType.LPAREN);
                String nameOfFirstArgument = this.Consume(TokenType.WORD).Text;
                this.Match(TokenType.ASSIGN);
                Expression exp = this.Expression();
                // (x = exp)

                Boolean x = !this.Match(TokenType.SPLIT);
                Boolean y = this.LookMatch(1, TokenType.COLON) || this.LookMatch(1, TokenType.LAMBDA);
                Boolean z = this.LookMatch(0, TokenType.RPAREN);
                if (x && (z && !y)) {
                    this.Match(TokenType.RPAREN);
                    return new Assigment(nameOfFirstArgument, exp, this.line, this.fileName);
                }

                List<ArgumentMetadataGenerator> args = this.ParseArgs(TokenType.RPAREN);

                args.Insert(0, new ArgumentMetadataGenerator(nameOfFirstArgument, null, exp));

                Expression returnedType = null;

                if (this.Match(TokenType.COLON)) {
                    returnedType = this.ParseType();
                    this.Match(TokenType.EOC);
                }

                this.Match(TokenType.LAMBDA);

                return new AnonymeDefine(this.Expression());
            }

            // (.: || (x, || (... .,
            if ((this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.WORD) && this.LookMatch(2, TokenType.SPLIT))
                || (this.LookMatch(0, TokenType.LPAREN) && this.LookMatch(1, TokenType.DOTDOTDOT) && this.LookMatch(3, TokenType.SPLIT))) {
                this.Match(TokenType.LPAREN);

                List<ArgumentMetadataGenerator> args = this.ParseArgs(TokenType.RPAREN);

                Boolean x = false;

                if (this.Match(TokenType.LAMBDA)) {
                    x = true;
                }

                Expression exp = this.Expression();

                if (x) {
                    return new AnonymeDefine(exp);
                } else {

                }
            }

            if (this.Match(TokenType.VOID)) {
                return UnitExpression.Instance;
            }

            if (this.Match(TokenType.LPAREN)) {
                Boolean mem = this.allow;
                this.allow = true;
                Expression result = this.Expression();

                this.allow = mem;
                this.Match(TokenType.RPAREN);
                return result;
            }

            if (this.Match(TokenType.TEXT)) {
                return new StringE(Current.Text);
            }



            return this.BlockExpression();
        }

        private Expression BlockExpression() {
            if (this.LookMatch(0, TokenType.DO)) {
                return this.AloneORBlock();
            }

            if (this.Match(TokenType.EOC)) {
                return UnitExpression.Instance;
            }

            throw new Exception();
        }

        private Expression Element(Expression res) {
            this.Match(TokenType.LBRACKET);

            List<Expression> Indices = new List<Expression>();

            if (this.Match(TokenType.RBRACKET)) {
                return new DotApplicate(new DotExpression(res, Op.GETI, this.fileName, this.line), Indices);
            }

            do {
                if (this.Match(TokenType.RBRACKET)) {
                    Indices.Add(UnitExpression.Instance);
                    break;
                } else if (this.Match(TokenType.SPLIT)) {
                    Indices.Add(UnitExpression.Instance);
                    if (this.Match(TokenType.RBRACKET)) {
                        Indices.Add(UnitExpression.Instance);
                        break;
                    }
                } else {
                    Indices.Add(this.Expression());
                    if (this.Match(TokenType.SPLIT)) {
                        if (this.Match(TokenType.RBRACKET)) {
                            Indices.Add(UnitExpression.Instance);
                            break;
                        }
                    }
                }
            } while (!this.Match(TokenType.RBRACKET));

            return new GetIndexE(res, Indices);
        }

        private Expression ParseType() {
            Expression result = new IdExpression(this.Consume(TokenType.WORD).Text, this.line, this.fileName);

            while (this.Match(TokenType.DOT)) {
                result = new DotExpression(result, this.Consume(TokenType.WORD).Text, this.fileName, this.line);
            }

            while (this.Match(TokenType.LBRACKET)) {
                List<Expression> exps = new List<Expression>();
                while (!this.Match(TokenType.RBRACKET)) {
                    exps.Add(this.Expression());
                    this.Match(TokenType.SPLIT);
                }
                result = new GetIndexE(result, exps);// new DotApplicate(new DotExpression(result, Op.GETI), exps);
            }

            return result;
        }

        private List<ArgumentMetadataGenerator> ParseArgs(TokenType border) {
            List<ArgumentMetadataGenerator> result = new List<ArgumentMetadataGenerator>();

            while (!this.Match(border)) {
                String nameOfArgument;
                if (this.Match(TokenType.DOTDOTDOT)) {
                    nameOfArgument = this.Consume(TokenType.WORD).Text;

                    if (this.Match(TokenType.ASSIGN)) {
                        result.Add(new ArgumentMetadataGenerator("*" + nameOfArgument, null, this.Expression()));
                    } else {
                        result.Add(new ArgumentMetadataGenerator("*" + nameOfArgument, null, null));
                    }

                    this.Match(TokenType.SPLIT);

                    continue;
                }

                nameOfArgument = this.Consume(TokenType.WORD).Text;

                result.Add(new ArgumentMetadataGenerator(nameOfArgument, null, null));

                this.Match(TokenType.SPLIT);

                if (this.Match(TokenType.EOF)) {
                    throw new LumenException("пропущена закрывающая круглая скобка");
                }
            }

            return result;
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

        private Boolean LookMatch(Int32 pos, TokenType type) {
            return this.GetToken(pos).Type == type;
        }

        private Token GetToken(Int32 NewPosition) {
            Int32 position = this.position + NewPosition;

            if (position >= this.size) {
                return new Token(TokenType.EOF, "");
            }

            return this.tokens[position];
        }

        private Token Consume(TokenType type) {
            Token Current = this.GetToken(0);
            this.line = Current.Line;

            if (type != Current.Type) {
                throw new LumenException($"wait token " + type.ToString() + " given " + Current.Type.ToString(), fileName: this.fileName, line: this.line);
            }

            this.position++;
            return Current;
        }
    }
}