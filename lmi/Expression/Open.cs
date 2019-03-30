using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;

namespace Lumen.Light {
    public class Reference : Expression {
        public String pathToFile;
        public Int32 line;
        private readonly String fileName;

        public Reference(String pathToFile, String fileName, Int32 line) {
            this.pathToFile = pathToFile;
            this.fileName = fileName;
            this.line = line;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return this;
        }

        public Value Eval(Scope e) {
            String path = this.pathToFile + 
                (File.Exists(AppDomain.CurrentDomain.BaseDirectory + this.pathToFile + ".dll") || File.Exists(this.pathToFile + ".dll") ? ".dll" : ".lm");

            String fullPath = new FileInfo(path).FullName;

            if (Prelude.GlobalImportCache.ContainsKey(fullPath)) {
                foreach (KeyValuePair<String, Value> i in Prelude.GlobalImportCache[fullPath]) {
                    e.Set(i.Key, i.Value);
                }

                return Const.UNIT;
            }

            // make
            if (path.EndsWith(".dll")) {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + path)) {
                    Assembly ass = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + path);
                    ass.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
                    return Const.UNIT;
                }
                Assembly a = Assembly.Load(path);
                a.GetType("Main").GetMethod("Import").Invoke(null, new Object[] { e, "" });
                return Const.UNIT;
            }

            // TODO
            Scope x = new Scope(e);
            x.AddUsing(Prelude.Instance);
            Parser p = new Parser(new Lexer(File.ReadAllText(fullPath), this.fileName).Tokenization(), this.fileName);
            var exps = p.Parsing();
            exps.Select(i => i.Eval(x));
            Dictionary<String, Value> included = new Dictionary<String, Value>();
            foreach (KeyValuePair<String, Value> i in x.variables) {
                included.Add(i.Key, i.Value);
                e.Set(i.Key, i.Value);
            }

            Prelude.GlobalImportCache[fullPath] = included;

            return Const.UNIT;
        }
    }
}