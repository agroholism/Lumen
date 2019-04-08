using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lumen.Lang {
    public sealed class Prelude : Module {
        #region Fields
        public static TypeClass Ord { get; } = new OrdModule();
        public static TypeClass Sequence { get; } = new SequenceModule();
        public static TypeClass Functor { get; } = new Functor();
        public static TypeClass Monad { get; } = new Monad();
        public static TypeClass Applicative { get; } = new Applicative();

        public static Module Any { get; } = new AnyModule();

        public static Module Array { get; } = new ArrayModule();
        public static Module Function { get; } = new Function();
        public static Module Number { get; } = new NumberModule();
        public static Module Map { get; } = new MapClass();
        public static Module Null { get; } = new RVoid();
        public static Module Text { get; } = new TextModule();
        public static Module List { get; } = new ListModule();

        public static Module DateTime { get; } = new DateTimeModule();

        public static Module Boolean { get; } = new BooleanModule();

        public static IObject Fail { get; private set; }
        public static Module Option { get; } = new Option();
        public static IObject None { get; } = (Option as Option).None;
        public static Constructor Some { get; } = (Option as Option).Some;

        public static Prelude Instance { get; } = new Prelude();

        #endregion
        private static readonly Random mainRandomObject = new Random();
        public static Dictionary<String, Dictionary<String, Value>> GlobalImportCache { get; } = new Dictionary<String, Dictionary<String, Value>>();

        private Prelude() {
            ConstructFail();

            this.SetField("prelude", this);

            this.SetField("Ord", Ord);
            this.SetField("Functor", Functor);
            this.SetField("Monad", Monad);

            this.SetField("Fail", Fail);

            this.SetField("Option", Option);
            this.SetField("Some", (Option as Option).Some);
            this.SetField("None", (Option as Option).None);

            this.SetField("List", List);

            this.SetField("Array", Array);
            this.SetField("DateTime", DateTime);
            this.SetField("Number", Number);
            this.SetField("Boolean", Boolean);
            this.SetField("Text", Text);
            this.SetField("Function", Function);
            this.SetField("Sequence", Sequence);

            this.SetField("Map", Map);

            this.SetField("true", Const.TRUE);
            this.SetField("false", Const.FALSE);

            this.SetField("PI", (Number)Math.PI);
            this.SetField("E", (Number)Math.E);

            this.SetField("writeFile", new LambdaFun((scope, args) => {
                String fileName = scope["fileName"].ToString(scope);
                String text = scope["text"].ToString(scope);

                try {
                    File.WriteAllText(fileName, text);
                    return Helper.CreateSome(new Text(fileName));
                } catch {
                    return Prelude.None;
                }
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("text"),
                    new NamePattern("fileName")
                }
            });

            this.SetField("readFile", new LambdaFun((scope, args) => {
                String fileName = scope["fileName"].ToString(scope);

                if (File.Exists(fileName)) {
                    try {
                        return Helper.CreateSome(new Text(File.ReadAllText(fileName)));
                    } catch {
                        return Prelude.None;
                    }
                }

                return Prelude.None;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fileName")
                }
            });

            this.SetField("readLines", new LambdaFun((scope, args) => {
                String fileName = scope["fileName"].ToString(scope);

                if (File.Exists(fileName)) {
                    try {
                        return new List(LinkedList.Create(File.ReadAllLines(fileName).Select(i => new Text(i))));
                    } catch {
                        return new List(LinkedList.Empty);
                    }
                }

                return new List(LinkedList.Empty);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fileName")
                }
            });

            this.SetField("readArray", new LambdaFun((scope, args) => {
                String fileName = scope["fileName"].ToString(scope);

                if (File.Exists(fileName)) {
                    try {
                        return new Array(File.ReadAllLines(fileName).Select(i => new Text(i) as Value).ToList());
                    } catch {
                        return new Array();
                    }
                }

                return new Array();
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fileName")
                }
            });

            this.SetField("createFile", new LambdaFun((scope, args) => {
                String fileName = scope["fileName"].ToString(scope);
                try {
                    File.Create(fileName).Close();
                    return Helper.CreateSome(new Text(fileName));
                } catch {
                    return Prelude.None;
                }
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fileName")
                }
            });

            this.SetField("print", new LambdaFun((scope, args) => {
                Value x = scope["x"];

                Console.WriteLine(x.ToString(scope));

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("x")
                }
            });

            this.SetField("read", new LambdaFun((scope, args) => {
                return new Text(Console.ReadLine());
            }));
        }

        private static void ConstructFail() {
            Module FailModule = new Module("_") {
                Parent = Any
            };

            FailModule.SetField("String", new LambdaFun((scope, args) => {
                IObject obj = scope["this"] as IObject;
                if(obj.TryGetField("message", out Value result)) {
                    return new Text($"Failed with message '{result.ToString(scope)}'");
                }

                throw new LumenException("failed in fail.tos");
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            }, null);

            Fail = Helper.CreateConstructor("prelude.Fail", FailModule, new[] { "message" });
        }

        public static Value DeconstructSome(Value some, Scope scope) {
            if(Some.IsParentOf(some)) {
                IObject someInstance = some as IObject;
                if(someInstance.TryGetField("x", out Value result)) {
                    return result;
                }
            }

            return some;
        }
    }
}
