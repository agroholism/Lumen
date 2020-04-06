using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Lumen.Lang.Expressions;

namespace ldoc {
    public class Program {
        public static Boolean LightMode = true;

        public static void Main(String[] args) {
            if (args.Length == 0) {
                String fileName = Console.ReadLine();
                List<Token> tokens = new Lexer(File.ReadAllText(fileName), fileName).Tokenization();
                List<Expression> lst = new Parser(tokens, fileName).Parsing();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?><docs></docs>");

                foreach (Expression expression in lst) {
                    GenerateXML(expression, doc);
                }

                doc.Save("out.xml");
            }
        }

        public static List<CommentParseResult> Parse(String code) {
            List<Token> tokens = new Lexer(code, "").Tokenization();
            List<Expression> lst = new Parser(tokens, "").Parsing();

            List<CommentParseResult> res = new List<CommentParseResult>();
            foreach (Expression expression in lst) {
                List<CommentParseResult> r = AnalyzeExpression(expression);
                if(r != null) {
                    res.AddRange(r);
                }
            }

            return res;
        }

        public static void GenerateXML(Expression expression, XmlDocument doc, String baseName=null) {
            if(expression is DocComment de) {
                XmlElement root = doc.DocumentElement;
                if (de.Inner is FunctionDeclaration fd) {
                    CommentParseResult parseResult = ParseComment(de.doc);
                    XmlElement funcNode = doc.CreateElement("function");

                    XmlAttribute name = doc.CreateAttribute("name");
                    name.Value = (baseName == null ? "" : baseName + ".") + fd.NameFunction;

                    XmlAttribute summary = doc.CreateAttribute("summary");
                    summary.Value = parseResult.Summary;

                    XmlAttribute decla = doc.CreateAttribute("declaration");
                    decla.Value = RestoreFunctionDeclaration(fd);

                    funcNode.Attributes.Append(name);
                    funcNode.Attributes.Append(summary);
                    funcNode.Attributes.Append(decla);

                    root.AppendChild(funcNode);
                }
            }
        }

        public static List<CommentParseResult> AnalyzeExpression(Expression expression, String baseName = null) {
            if (expression is DocComment de) {
                if (de.Inner is FunctionDeclaration fd) {
                    CommentParseResult parseResult = ParseComment(de.doc);
                    parseResult.Declaration = RestoreFunctionDeclaration(fd);
                    parseResult.Name = (baseName == null ? "" : baseName + ".") + fd.NameFunction;
                    parseResult.Type = Type.FUNCTION;
                    return new List<CommentParseResult> { parseResult };
                } 

                if(de.Inner is ModuleDeclaration md) {
                    List<CommentParseResult> res = new List<CommentParseResult>();

                    CommentParseResult parseResult = ParseComment(de.doc);
                    parseResult.Declaration = "module " + md.moduleName + " = ...";
                    parseResult.Name = (baseName == null ? "" : baseName + ".") + md.moduleName;
                    parseResult.Type = Type.MODULE;
                    res.Add(parseResult);

                    foreach (Expression i in md.moduleExpressions) {
                        List<CommentParseResult> x = AnalyzeExpression(i, (baseName == null ? "" : baseName + ".") + md.moduleName);
                        if (x != null) {
                            res.AddRange(x);
                        }
                    }

                    return res;
                }

                if (de.Inner is TypeDeclaration td) {
                    List<CommentParseResult> res = new List<CommentParseResult>();

                    CommentParseResult parseResult = ParseComment(de.doc);
                    parseResult.Declaration = "type " + td.name
						+ " = " + String.Concat(" | ", td.constructors.Select(x => x.Name + " " + String.Join(" ", x.Parameters)));
                    parseResult.Name = (baseName == null ? "" : baseName + ".") + td.name;
                    parseResult.Type = Type.MODULE;
                    res.Add(parseResult);

                    foreach(var i in td.constructors) {
                        parseResult = new CommentParseResult();
                        parseResult.Name = (baseName == null ? "" : baseName + ".") + td.name + "." + td.constructors;
                        parseResult.Declaration = parseResult.Declaration;
                        parseResult.Type = Type.CONSTRUCTOR;
                        res.Add(parseResult);
                    }

                    foreach (Expression i in td.members) {
                        List<CommentParseResult> x = AnalyzeExpression(i, (baseName == null ? "" : baseName + ".") + td.name);
                        if (x != null) {
                            res.AddRange(x);
                        }
                    }

                    return res;
                }
            }

            if (expression is FunctionDeclaration fd1) {
                CommentParseResult parseResult = new CommentParseResult();
                parseResult.Declaration = RestoreFunctionDeclaration(fd1);
                parseResult.Name = (baseName == null ? "" : baseName + ".") + fd1.NameFunction;
                parseResult.Type = Type.FUNCTION;
                return new List<CommentParseResult> { parseResult };
            }

            if (expression is ModuleDeclaration md1) {
                List<CommentParseResult> res = new List<CommentParseResult>();

                CommentParseResult parseResult = new CommentParseResult();
                parseResult.Declaration = "module " + md1.moduleName + " = ...";
                parseResult.Name = (baseName == null ? "" : baseName + ".") + md1.moduleName;
                parseResult.Type = Type.MODULE;
                res.Add(parseResult);

                foreach (Expression i in md1.moduleExpressions) {
                    List<CommentParseResult> x = AnalyzeExpression(i, (baseName == null ? "" : baseName + ".") + md1.moduleName);
                    if (x != null) {
                        res.AddRange(x);
                    }
                }

                return res;
            }

            if (expression is TypeDeclaration td1) {
                List<CommentParseResult> res = new List<CommentParseResult>();

                CommentParseResult parseResult = new CommentParseResult();
                parseResult.Declaration = "type " + td1.name + " = " + String.Join(" | ", td1.constructors.Select(x => x.Name + " " + String.Join(" ", x.Parameters)).ToArray());
                parseResult.Name = (baseName == null ? "" : baseName + ".") + td1.name;
                parseResult.Type = Type.MODULE;
                res.Add(parseResult);

                foreach (var i in td1.constructors) {
                    parseResult = new CommentParseResult();
                    parseResult.Name = (baseName == null ? "" : baseName + ".") + td1.name + "." + i.Name;
                    parseResult.Declaration = parseResult.Declaration;
                    parseResult.Type = Type.CONSTRUCTOR;
                    res.Add(parseResult);
                }

                foreach (Expression i in td1.members) {
                    List<CommentParseResult> x = AnalyzeExpression(i, (baseName == null ? "" : baseName + ".") + td1.name);
                    if (x != null) {
                        res.AddRange(x);
                    }
                }

                return res;
            }

            return null;
        }

        public static String RestoreFunctionDeclaration(FunctionDeclaration declaration) {
            StringBuilder result = new StringBuilder();
            result.Append("let ").Append(declaration.NameFunction).Append(" ");

            result.Append(String.Join(" ", declaration.arguments));

            return result.ToString() + "= ...";
        }

        public static CommentParseResult ParseComment(String comment) {
            Match x = Regex.Match(comment, "<summary>(.*)?</summary>");

            if(!x.Success && LightMode) {
                return new CommentParseResult {
                    Example = null,
                    Parameters = null,
                    Returns = null,
                    Summary = comment
                };
            }

            String summary = Regex.Match(comment, "<summary>(.*)?</summary>").Groups[1].Value;
            String returns = Regex.Match(comment, "<returns>(.*)?</returns>").Groups[1].Value;
            String example = Regex.Match(comment, "<example>(.*)?</example>").Groups[1].Value;
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            foreach(Match param in Regex.Matches(comment, "<param name=\"(.*)?\">(.*)?</param>")) {
                if (param.Success) {
                    parameters.Add(param.Groups[1].Value, param.Groups[2].Value);
                }
            }

            return new CommentParseResult {
                Example = example,
                Parameters = parameters,
                Returns = returns,
                Summary = summary
            };
        }

        public class Metadata {
            public List<CommentParseResult> Results { get; } = new List<CommentParseResult>();
        }

        public class CommentParseResult {
            public Type Type { get; set; }

            public String Name { get; set; }
            public String Declaration { get; set; }

            public String Summary { get; set; }
            public String Returns { get; set; }
            public String Example { get; set; }
            public Dictionary<String, String> Parameters { get; set; }
        }

        public enum Type {
            MODULE,
            FUNCTION,
            CONSTRUCTOR
        }
    }
}
    

