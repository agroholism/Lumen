using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class ModuleDeclaration : Expression {
        internal String name;
        internal List<Expression> moduleExpressions;
		internal String fileName;
		internal Int32 lineNumber;

        public ModuleDeclaration(String name, List<Expression> moduleExpressions) {
            this.name = name;
            this.moduleExpressions = moduleExpressions;
        }
 
        public Value Eval(Scope scope) {
            Scope moduleScope = new Scope(scope);

			foreach (Expression expression in this.moduleExpressions) {
				expression.Eval(moduleScope);
			}

			Module moduleValue = new Module(this.name);

            foreach (KeyValuePair<String, Value> i in moduleScope.variables) {
                moduleValue.SetMember(i.Key, i.Value);
            }

            // Add rename

            scope[this.name] = moduleValue;

            return Const.UNIT;
        }

        public Expression Closure(ClosureManager manager) {
			manager.Declare(this.name);

			ClosureManager newManager = manager.Clone();

            return new ModuleDeclaration(this.name, this.moduleExpressions.Select(i => i.Closure(newManager)).ToList());
        }

		public override String ToString() {
			String result = $"module {this.name} where{Environment.NewLine}";

			foreach(Expression i in this.moduleExpressions) {
				String[] inner = i.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				result += $"\t{String.Join(Environment.NewLine + "\t", inner)}{Environment.NewLine}";
			}

			return result;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorTerminalResult(this.Eval(scope));
		}

		/* public static void Rename(Module m, Scope e) {
             foreach (KeyValuePair<String, Value> i in m.scope.variables) {
                 if (i.Value is Module type) {
                     type.meta.Name = m.name.Split('.')[0] + "." + type.meta.Name;
                     foreach (KeyValuePair<String, Fun> j in type.attributes) {
                         if (j.Value is LambdaFun lfun) {
                             if (lfun.Attributes.ContainsKey("name")) {
                                 lfun.Set("name", (String)(type.meta.Name.Split('.')[0] + "." + lfun.Get("name", e)), e);
                             }
                         } else if (j.Value is UserFun sfun) {
                             if (sfun.Attributes.ContainsKey("name")) {
                                 sfun.Set("name", (String)(type.meta.Name.Split('.')[0] + "." + sfun.Get("name", e)), e);
                             }
                         }
                     }
                 } else if (i.Value is UserFun sfun) {
                     sfun.Set("name", (String)(m.name.Split('.')[0] + "." + sfun.Get("name", e)), e);
                 } else if (i.Value is LambdaFun lfun) {
                     lfun.Set("name", (String)(m.name.Split('.')[0] + "." + lfun.Get("name", e)), e);
                 } else if (i.Value is Module md) {
                     md.name = m.name.Split('.')[0] + "." + md.name;
                     Rename(md, e);
                 }
             }
         }*/
	}
}