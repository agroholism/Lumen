using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    internal class ModuleDeclaration : Expression {
        internal String moduleName;
        internal List<Expression> moduleExpressions;

        public ModuleDeclaration(String moduleName, List<Expression> moduleExpressions) {
            this.moduleName = moduleName;
            this.moduleExpressions = moduleExpressions;
        }
 
        public Value Eval(Scope scope) {
            Scope moduleScope = new Scope(scope);

            Module moduleValue = new Module(this.moduleName);

            foreach (Expression expression in this.moduleExpressions) {
                expression.Eval(moduleScope);
            }

            foreach (KeyValuePair<String, Value> i in moduleScope.variables) {
                moduleValue.SetMember(i.Key, i.Value);
            }

            // Add rename

            scope[this.moduleName] = moduleValue;

            return Const.UNIT;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.moduleName);

			ClosureManager manager2 = manager.Clone();

            return new ModuleDeclaration(this.moduleName, this.moduleExpressions.Select(i => i.Closure(manager2)).ToList());
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